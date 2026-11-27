using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcSourceLandingEmission {
        public string npc,state,condition,effect,position,sound,sourceCallback,sourceSoundName,sourceInstructionsSha256;
        public bool sourceStateEndCallback;
    }
    public sealed partial class GameModel {
        readonly Dictionary<string,NpcSourceLandingEmission> npcSourceLandingV17=new Dictionary<string,NpcSourceLandingEmission>(StringComparer.Ordinal);
        readonly HashSet<string> npcSourceSoundAssetsV17=new HashSet<string>(StringComparer.Ordinal);
        sealed class SourceLandingStampV17 {internal string key;internal float time;}
        readonly Dictionary<Enemy,SourceLandingStampV17> npcSourceLandingStampsV17=new Dictionary<Enemy,SourceLandingStampV17>();

        // Called only after the sound document's full validation succeeds.
        // Reloading sounds invalidates old landing bindings until mechanisms
        // explicitly install their matching optional rows again.
        void CaptureNpcSourceSoundAssetsV17(IEnumerable<string> ids){
            npcSourceSoundAssetsV17.Clear();if(ids!=null)foreach(string id in ids)npcSourceSoundAssetsV17.Add(id);
            npcSourceLandingV17.Clear();npcSourceLandingStampsV17.Clear();
        }
        void ClearNpcSourceLandingStampsV17(){npcSourceLandingStampsV17.Clear();}

        public void SetNpcSourceLandingEmissionsV17(NpcSourceLandingEmission[] rows){
            var validated=new Dictionary<string,NpcSourceLandingEmission>(StringComparer.Ordinal);
            if(rows!=null)foreach(var row in rows){
                NpcStateDefinition source;NpcEffectDefinition effect;
                string key=row==null?null:row.npc+"/"+row.state+"/"+row.condition;
                if(row==null||string.IsNullOrEmpty(row.npc)||string.IsNullOrEmpty(row.state)||
                    row.condition!="SCT_FOOT_ON_LINE_TRUE"||row.position!="original-land-position"||
                    !row.sourceStateEndCallback||string.IsNullOrEmpty(row.sourceCallback)||
                    string.IsNullOrEmpty(row.sourceSoundName)||string.IsNullOrEmpty(row.sourceInstructionsSha256)||row.sourceInstructionsSha256.Length!=64||
                    validated.ContainsKey(key)||!npcStates.TryGetValue(row.npc+"/"+row.state,out source)||
                    !npcEffectDefinitions.TryGetValue(row.effect??"",out effect)||!npcSourceSoundAssetsV17.Contains(row.sound??""))
                    throw new ArgumentException("Invalid original landing callback binding.");
                bool transition=false;foreach(var item in source.sourceEventTransitions??new NpcSourceEventTransition[0])
                    if(item.condition==row.condition&&npcStates.ContainsKey(row.npc+"/"+item.nextState))transition=true;
                if(!transition)throw new ArgumentException("Landing callback has no real source ground transition.");
                foreach(char c in row.sourceInstructionsSha256)if(!Uri.IsHexDigit(c))throw new ArgumentException("Invalid original landing callback hash.");
                validated.Add(key,row);
            }
            npcSourceLandingV17.Clear();foreach(var item in validated)npcSourceLandingV17.Add(item.Key,item.Value);npcSourceLandingStampsV17.Clear();
        }

        // Called once for a successful actual physics transition. Interrupted
        // actions, death and ordinary animation-end never call this path.
        bool EmitNpcSourceLandingV17(Enemy enemy,string fromState,string toState,string condition){
            if(enemy==null||enemy.hp<=0||!enemy.active||enemy.dormant||!Grounded(enemy))return false;
            string key=enemy.sourceNPC+"/"+fromState+"/"+condition;NpcSourceLandingEmission row;
            if(!npcSourceLandingV17.TryGetValue(key,out row))return false;
            var source=npcStates[enemy.sourceNPC+"/"+fromState];
            if(!Array.Exists(source.sourceEventTransitions,t=>t.condition==condition&&t.nextState==toState))return false;
            SourceLandingStampV17 stamp;if(npcSourceLandingStampsV17.TryGetValue(enemy,out stamp)&&stamp.key==key&&stamp.time==elapsed)return false;
            npcSourceLandingStampsV17[enemy]=new SourceLandingStampV17{key=key,time=elapsed};
            var emission=new NpcEffectEmission{effect=row.effect,start=0,bone="",scaleX=1,scaleY=1,scaleZ=1,follow=false,ownerState=""};
            SpawnNpcEffect(enemy,npcEffectDefinitions[row.effect],emission,elapsed);
            var sound=new NpcTimedSound{id="source-landing/"+key,sound=row.sound,sourceField="LUA_STATE_END_FUNC/"+row.sourceCallback,sourceName=row.sourceSoundName,candidateClock="source-physics-transition",sourceTime=0,candidateTime=0};
            QueueNpcSound(sound,enemy,++npcSoundEntrySerial,0,elapsed,fromState);
            TraceNpc(enemy,"source-landing-emission");return true;
        }
    }
}
