using System;
using System.Collections.Generic;
namespace ElswordPixel {
    // A bounded presentation feed is independent of diagnostic tracing. It
    // never owns hit testing, effect lifetime, sounds or state transitions.
    public sealed class NpcPresentationEvent {
        public int sequence,serial,facing;public string type,effect;
        public float time,x,y,z,scale,depth;public Enemy owner;
        // Read-only committed presentation geometry; does not change source simulation.
        public bool hasCommittedGeometry,hasSourceLandY;public float sourceDirX,sourceDirY,effectScaleX,effectScaleY,effectScaleZ,sourceLandY;
    }
    public sealed partial class GameModel {
        public readonly List<NpcPresentationEvent> npcPresentationEvents=new List<NpcPresentationEvent>();
        public int NpcPresentationGeneration {get;private set;}
        int npcPresentationSequence;
        // Read-only identity peek. Never allocates an audio/visibility actor ID.
        public bool TryGetExistingNpcSoundActorIdR661(Enemy actor,out int id){
            id=0;if(actor==null)return false;bool current=false;
            foreach(var enemy in enemies)if(object.ReferenceEquals(enemy,actor)){current=true;break;}
            if(!current)return false;return npcSoundActorIds.TryGetValue(actor,out id);
        }
        void ClearNpcPresentationEvents(){npcPresentationEvents.Clear();npcPresentationSequence=0;NpcPresentationGeneration++;ClearPlainShockParticleEventsR1480();}
        void RecordNpcPresentation(string type,NpcEffectInstance effect,float at){
            if(type=="hit-child"){
                // Spawn precedes the original DEPT_IMPACT_POINT adjustment.
                // Update its pending display event to the committed position.
                for(int i=npcPresentationEvents.Count-1;i>=0;i--){var p=npcPresentationEvents[i];if(p.serial==effect.serial&&p.type=="spawn"){p.x=effect.x;p.y=effect.y;p.z=effect.z;break;}}return;
            }
            if(type!="spawn"&&type!="hit")return;
            if(npcPresentationEvents.Count>=512)npcPresentationEvents.RemoveAt(0);
            npcPresentationEvents.Add(new NpcPresentationEvent{sequence=++npcPresentationSequence,serial=effect.serial,type=type,effect=effect.id,time=at<0?elapsed:at,x=effect.x,y=effect.y,z=effect.z,facing=effect.facing,scale=effect.ownerScale,depth=effect.owner==null?0:effect.owner.nativeDepth,owner=effect.owner,hasCommittedGeometry=true,sourceDirX=effect.dirX,sourceDirY=effect.dirY,effectScaleX=effect.scaleX,effectScaleY=effect.scaleY,effectScaleZ=effect.scaleZ,hasSourceLandY=effect.hasSourceLandY,sourceLandY=effect.sourceLandY});
        }
        void RecordNpcPresentationHit(NpcEffectInstance effect,float at,float x,float y,float z){
            RecordNpcPresentation("hit",effect,at);var p=npcPresentationEvents[npcPresentationEvents.Count-1];p.x=x;p.y=y;p.z=z;
        }
    }
}
