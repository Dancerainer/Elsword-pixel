using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcSoundAsset {public string id,sha256;}
    [Serializable] public sealed class NpcTimedSound {
        public string id,sound,sourceField,sourceName,candidateClock;
        public float sourceTime,candidateTime;public bool nativeClockOwnershipProven;
    }
    [Serializable] public sealed class NpcStateSoundTrack {
        public string npc,state,sourceAnimation;public float sourceAnimSpeed=1,duration;public bool loop;
        public NpcTimedSound[] events=new NpcTimedSound[0];
    }
    [Serializable] public sealed class NpcEffectSoundTrack {
        public string effect;public float lifetime;public NpcTimedSound[] events=new NpcTimedSound[0];
    }
    [Serializable] public sealed class NpcSoundDocument {
        public bool runtimeAdmitted,sourceMechanismsComplete;
        public NpcSoundAsset[] assets=new NpcSoundAsset[0];
        public NpcStateSoundTrack[] stateTracks=new NpcStateSoundTrack[0];
        public NpcEffectSoundTrack[] effectTracks=new NpcEffectSoundTrack[0];
    }
    // A production presentation event: independent of diagnostic trace flags.
    // Serial is monotonic over this GameModel's lifetime, including room/reset.
    [Serializable] public sealed class NpcSoundEvent {
        public long serial,entrySerial;public int worldSerial,actorSerial,effectSerial,loop;
        public string kind,npc,state,effect,eventId,sound,sourceField;
        public float time,localTime,sourceTime,x,y,z;
    }
    public sealed partial class GameModel {
        void QueueRegionScriptSound(string operation,Enemy owner,string state,object[] arguments) {
            foreach(var argument in arguments) {
                var filename=argument as string;
                if(filename==null || !filename.EndsWith(".ogg",StringComparison.OrdinalIgnoreCase))continue;
                var stem=System.IO.Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();
                QueueNpcSound(new NpcTimedSound {id=filename,sound=stem,sourceField=operation},
                    owner,++npcSoundEntrySerial,0,elapsed,state);
                return;
            }
        }
        const int NpcSoundQueueCapacity=1024;
        readonly Dictionary<string,NpcStateSoundTrack> npcStateSounds=new Dictionary<string,NpcStateSoundTrack>(StringComparer.Ordinal);
        readonly Dictionary<string,NpcEffectSoundTrack> npcEffectSounds=new Dictionary<string,NpcEffectSoundTrack>(StringComparer.Ordinal);
        sealed class NpcStateSoundCursor {internal NpcStateSoundTrack track;internal int next,loop;internal long entry;}
        sealed class NpcEffectSoundCursor {internal NpcEffectSoundTrack track;internal int next;internal long entry;}
        readonly Dictionary<Enemy,NpcStateSoundCursor> npcStateSoundCursors=new Dictionary<Enemy,NpcStateSoundCursor>();
        readonly Dictionary<NpcEffectInstance,NpcEffectSoundCursor> npcEffectSoundCursors=new Dictionary<NpcEffectInstance,NpcEffectSoundCursor>();
        readonly Dictionary<Enemy,int> npcSoundActorIds=new Dictionary<Enemy,int>();
        readonly Queue<NpcSoundEvent> npcSoundQueue=new Queue<NpcSoundEvent>();
        long npcSoundSerial,npcSoundEntrySerial,npcSoundDropped;int npcSoundWorldSerial,npcSoundActorSerial;
        public int PendingNpcSoundEvents {get{return npcSoundQueue.Count;}}
        public long NpcSoundEventsProduced {get{return npcSoundSerial;}}
        public long NpcSoundEventsDropped {get{return npcSoundDropped;}}
        public int NpcSoundWorldSerial {get{return npcSoundWorldSerial;}}
        public bool TryDequeueNpcSound(out NpcSoundEvent sound){if(npcSoundQueue.Count==0){sound=null;return false;}sound=npcSoundQueue.Dequeue();return true;}

        public void SetNpcSoundDefinitions(NpcSoundDocument document){
            var states=new Dictionary<string,NpcStateSoundTrack>(StringComparer.Ordinal);var effects=new Dictionary<string,NpcEffectSoundTrack>(StringComparer.Ordinal);
            var assets=new HashSet<string>(StringComparer.Ordinal);var ids=new HashSet<string>(StringComparer.Ordinal);
            if(document!=null){
                if(document.assets==null||document.stateTracks==null||document.effectTracks==null)throw new ArgumentException("Missing NPC sound arrays");
                foreach(var a in document.assets){if(a==null||string.IsNullOrEmpty(a.id)||!assets.Add(a.id)||string.IsNullOrEmpty(a.sha256)||a.sha256.Length!=64)throw new ArgumentException("Invalid NPC sound asset");foreach(char c in a.sha256)if(!Uri.IsHexDigit(c))throw new ArgumentException("Invalid NPC sound asset hash");}
                foreach(var track in document.stateTracks){
                    NpcStateDefinition source;
                    if(track==null||string.IsNullOrEmpty(track.npc)||string.IsNullOrEmpty(track.state)||!npcStates.TryGetValue(track.npc+"/"+track.state,out source)||states.ContainsKey(track.npc+"/"+track.state)||
                       !Finite(track.duration)||track.duration<=0||!Finite(track.sourceAnimSpeed)||track.sourceAnimSpeed<=0||track.sourceAnimation!=source.sourceAnimation||track.loop!=source.loop||Math.Abs(track.duration-source.duration)>.00001f||Math.Abs(track.sourceAnimSpeed-source.sourceAnimSpeed)>.00001f)
                        throw new ArgumentException("Invalid NPC state sound binding");
                    ValidateNpcSounds(track.events,assets,ids,track.duration,track.sourceAnimSpeed,"npc-state-pose-seconds");states.Add(track.npc+"/"+track.state,track);
                }
                foreach(var track in document.effectTracks){
                    NpcEffectDefinition source;
                    if(track==null||string.IsNullOrEmpty(track.effect)||!npcEffectDefinitions.TryGetValue(track.effect,out source)||effects.ContainsKey(track.effect)||!Finite(track.lifetime)||track.lifetime<=0||Math.Abs(track.lifetime-source.lifetime)>.00001f)
                        throw new ArgumentException("Invalid NPC effect sound binding");
                    ValidateNpcSounds(track.events,assets,ids,track.lifetime,1,"independent-effect-age-seconds");effects.Add(track.effect,track);
                }
            }
            // Validate everything before replacing live configuration/queue.
            CaptureNpcSourceSoundAssetsV17(assets);
            npcStateSounds.Clear();npcEffectSounds.Clear();foreach(var p in states)npcStateSounds.Add(p.Key,p.Value);foreach(var p in effects)npcEffectSounds.Add(p.Key,p.Value);
            ClearNpcSoundWorld();
            // Loading a document during an action never replays elapsed cues.
            foreach(var enemy in enemies){var state=NpcState(enemy);if(state!=null)BeginNpcStateSounds(enemy,state,false);}
            foreach(var effect in npcEffects)if(!effect.ended)BeginNpcEffectSounds(effect,false);
        }
        static void ValidateNpcSounds(NpcTimedSound[] events,HashSet<string> assets,HashSet<string> ids,float duration,float speed,string clock){
            if(events==null)throw new ArgumentException("Missing NPC sound events");float previous=-1;
            foreach(var e in events){
                if(e==null||string.IsNullOrEmpty(e.id)||!ids.Add(e.id)||!assets.Contains(e.sound??"")||string.IsNullOrEmpty(e.sourceField)||string.IsNullOrEmpty(e.sourceName)||e.candidateClock!=clock||
                   !Finite(e.sourceTime)||e.sourceTime<0||!Finite(e.candidateTime)||e.candidateTime<0||e.candidateTime<previous||e.candidateTime>duration+.000001f||Math.Abs(e.candidateTime-e.sourceTime/speed)>.00001f)
                    throw new ArgumentException("Invalid source NPC sound event");
                previous=e.candidateTime;
            }
        }
        void ClearNpcSoundWorld(){npcSoundQueue.Clear();npcStateSoundCursors.Clear();npcEffectSoundCursors.Clear();npcSoundActorIds.Clear();npcSoundWorldSerial++;}
        int NpcSoundActorId(Enemy owner){if(owner==null)return 0;int id;if(!npcSoundActorIds.TryGetValue(owner,out id)){id=++npcSoundActorSerial;npcSoundActorIds.Add(owner,id);}return id;}
        void QueueNpcSound(NpcTimedSound definition,Enemy owner,long entry,int loop,float time,string state,NpcEffectInstance effect=null){
            var sound=new NpcSoundEvent{serial=++npcSoundSerial,entrySerial=entry,worldSerial=npcSoundWorldSerial,actorSerial=NpcSoundActorId(owner),effectSerial=effect==null?0:effect.serial,loop=loop,
                kind=effect==null?"state":"effect",npc=owner==null?"":owner.sourceNPC,state=state??"",effect=effect==null?"":effect.id,eventId=definition.id,sound=definition.sound,sourceField=definition.sourceField,
                time=time,localTime=definition.candidateTime,sourceTime=definition.sourceTime,x=effect==null?(owner==null?player.x:owner.x):effect.x,y=effect==null?(owner==null?player.y:owner.y):effect.y,z=effect==null?0:effect.z};
            if(npcSoundQueue.Count>=NpcSoundQueueCapacity){npcSoundQueue.Dequeue();npcSoundDropped++;}npcSoundQueue.Enqueue(sound);
        }
        void CancelNpcStateSounds(Enemy enemy){if(enemy!=null)npcStateSoundCursors.Remove(enemy);}
        void BeginNpcStateSounds(Enemy enemy,NpcStateDefinition state,bool entry=true){
            CancelNpcStateSounds(enemy);NpcStateSoundTrack track;if(!npcStateSounds.TryGetValue(enemy.sourceNPC+"/"+state.id,out track))return;
            var cursor=new NpcStateSoundCursor{track=track,entry=++npcSoundEntrySerial};npcStateSoundCursors.Add(enemy,cursor);
            if(entry)AdvanceNpcStateSounds(enemy,state,0,0,elapsed);
            else while(cursor.next<track.events.Length&&track.events[cursor.next].candidateTime<=enemy.npcTime+.000001f)cursor.next++;
        }
        void AdvanceNpcStateSounds(Enemy enemy,NpcStateDefinition state,float from,float to,float packetStart){
            AdvanceNasodSprayCallbackV35(enemy,state,from,to,packetStart);
            AdvanceAlteraAirshipStatePresentationV20(enemy,state,from,to,packetStart);AdvanceMine2PresentationV21(enemy,state,from,to,packetStart);AdvancePlainOriginalEffectCallbacksR1315(enemy,state,from,to,packetStart);AdvancePlainOriginalElementCallbacksR1315(enemy,state,from,to,packetStart);
            NpcStateSoundCursor cursor;if(!npcStateSoundCursors.TryGetValue(enemy,out cursor)||cursor.track.state!=state.id)return;
            while(cursor.next<cursor.track.events.Length&&cursor.track.events[cursor.next].candidateTime<=to+.000001f){var e=cursor.track.events[cursor.next++];if(e.candidateTime<from-.000001f)continue;QueueNpcSound(e,enemy,cursor.entry,cursor.loop,packetStart+Math.Max(0,e.candidateTime-from),state.id);}
        }
        void AdvanceNpcLoopSounds(Enemy enemy,NpcStateDefinition state,float from,float dt,float packetStart){
            NpcStateSoundCursor cursor;if(!npcStateSoundCursors.TryGetValue(enemy,out cursor)){AdvanceAlteraAirshipLoopPresentationV20(enemy,state,from,dt,packetStart);AdvanceMine2PresentationLoopV21(enemy,state,from,dt,packetStart);return;}
            float left=dt,time=packetStart,position=from;
            while(left>0){float span=Math.Min(left,state.duration-position);AdvanceNpcStateSounds(enemy,state,position,position+span,time);left-=span;time+=span;position+=span;
                if(position>=state.duration-.000001f){cursor.next=0;cursor.loop++;position=0;AdvanceNpcStateSounds(enemy,state,0,0,time);}else break;
            }
        }
        void BeginNpcEffectSounds(NpcEffectInstance effect,bool entry=true){
            NpcEffectSoundTrack track;if(!npcEffectSounds.TryGetValue(effect.id,out track))return;
            var cursor=new NpcEffectSoundCursor{track=track,entry=++npcSoundEntrySerial};npcEffectSoundCursors[effect]=cursor;
            if(entry)AdvanceNpcEffectSounds(effect,0,0);else while(cursor.next<track.events.Length&&track.events[cursor.next].candidateTime<=effect.age+.000001f)cursor.next++;
        }
        void AdvanceNpcEffectSounds(NpcEffectInstance effect,float from,float to){
            NpcEffectSoundCursor cursor;if(effect.ended||!npcEffectSoundCursors.TryGetValue(effect,out cursor))return;
            while(cursor.next<cursor.track.events.Length&&cursor.track.events[cursor.next].candidateTime<=to+.000001f){var e=cursor.track.events[cursor.next++];if(e.candidateTime<from-.000001f)continue;QueueNpcSound(e,effect.owner,cursor.entry,0,effect.bornAt+e.candidateTime,"",effect);}
        }
        void EndNpcEffectSounds(NpcEffectInstance effect){npcEffectSoundCursors.Remove(effect);}
    }
}
