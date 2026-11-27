#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ElswordPixel.RegisteredRigPlayerGpuV35;

namespace ElswordPixel {
    public sealed partial class SourceMotionPlayerCheckV45 {
        [Serializable] sealed class BluePoruShot {
            public string label,sourceState,sourceMotion,sprite,resource,rgba;
            public float hp;
            public PixelProof before,after;
        }
        [Serializable] sealed class BluePoruReport { public List<BluePoruShot> draws=new List<BluePoruShot>(); }
        void BluePoruSampleV45(BluePoruReport result,string label){
            Call(scene,"RestoreNativeProjection");Call(scene,"RenderActors");
            Call(scene,"RenderCombatPresentation");Call(scene,"FinishNativeProjection");
            Need(view.enabled&&!view.forceRenderingOff&&view.sprite,"Live blue Poru body "+label);
            var actual=view.sprite;string original=null,resource=null;
            foreach(string file in new[]{BluePoruInkV45.Signed,BluePoruInkV45.Native,BluePoruInkV45.Ready}){
                string remapped=BluePoruInkV45.Resource(kind,file);
                if(actual.texture==Resources.Load<Texture2D>(remapped)){original=BluePoruInkV45.Prior(file);resource=remapped;break;}
            }
            Need(original!=null,"Actual signed or native blue Poru page "+actual.name+" / "+actual.texture.name);
            var screen=(Camera)Field(scene,"worldCamera");var renderers=new List<SpriteRenderer>{view};
            string rgba=actual.texture.isReadable?(string)Call(scene,"FeedbackNpcTextureRgbaV25",actual.texture):"";
            Need(!BluePoruInkV45.Before&&rgba==BluePoruInkV45.Pin(resource),"Exact revised blue Poru resource "+resource+" saw "+rgba+" on "+actual.texture.width+"x"+actual.texture.height);
            var sample=new BluePoruShot{label=label,sourceState=actor.npcState,sourceMotion=actor.clip,sprite=actual.name,resource=resource,rgba=rgba,hp=actor.hp};
            sample.after=CaptureAtResolution(screen,game,actor,renderers,dest,label+"-after",1280,720);
            var rect=actual.rect;var p=actual.pivot;
            var previous=Sprite.Create(Resources.Load<Texture2D>(original),rect,new Vector2(p.x/rect.width,p.y/rect.height),actual.pixelsPerUnit,0,SpriteMeshType.FullRect);
            try{view.sprite=previous;sample.before=CaptureAtResolution(screen,game,actor,renderers,dest,label+"-before",1280,720);}
            finally{view.sprite=actual;Destroy(previous);}
            Need(sample.before.count>0&&sample.after.count>0,"Both actual GPU bodies visible "+label);
            result.draws.Add(sample);
            File.WriteAllText(Path.Combine(dest,"blue-poru-ink.json"),JsonUtility.ToJson(result,true));
        }
        IEnumerable BluePoruCheckV45(){
            var profile=(NpcDefinition)Call(game,"NpcProfile",actor);
            var walk=Array.Find(profile.states,s=>s.sourceAnimation=="Walk");
            var wait=Array.Find(profile.states,s=>s.sourceAnimation=="Wait");
            Need(walk!=null&&wait!=null,"Original club Poru wait and walk");
            var fire=Array.Find(profile.states,s=>s.sourceAnimation=="AttackA_Fire");
            Need(fire!=null,"Original club swing");
            var results=new BluePoruReport();
            BluePoruSampleV45(results,"03-ready");
            Call(game,"EnterNpcState",actor,fire);BluePoruSampleV45(results,"04-swing");
            Call(game,"EnterNpcState",actor,wait);BluePoruSampleV45(results,"05-wait");
            Call(game,"EnterNpcState",actor,walk);BluePoruSampleV45(results,"06-walk");
            // The ordinary accepted hit and current carrier own the test. Keep clock,
            // camera, attack flash and facing identical while toggling paint in the capture.
            float x=actor.x-actor.facing*40;
            int damage=(int)Call(game,"ApplyHitToEnemyFromXR1417",actor,2,HitReaction.Launch,280f,x,5);
            Need(damage==2&&OriginalReactionV45.TryState(actor,out var pose)&&pose.motion=="DamageAirFlyBack","Rear airborne source reaction from a real hit");
            BluePoruSampleV45(results,"07-rear-launch");report.damaged=true;
            yield break;
        }
    }
}
#endif
