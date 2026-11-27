using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class GuardianSmallDrawV35 {
            public int frame;public SpriteRenderer carrier;public Sprite original;public string poseId,state,motion;
            public float clock,duration;
        }
        sealed class GuardianSmallViewV35 {
            public SpriteRenderer carrier;public FairyAuthoredPartsV33 parts;
            public string poseId,state;
        }
        readonly Dictionary<Enemy,GuardianSmallDrawV35> guardianDrawsV35=new Dictionary<Enemy,GuardianSmallDrawV35>();
        readonly Dictionary<Enemy,GuardianSmallViewV35> guardianViewsV35=new Dictionary<Enemy,GuardianSmallViewV35>();
        readonly HashSet<Enemy> guardianSeenV35=new HashSet<Enemy>(),guardianFailedV35=new HashSet<Enemy>();
        readonly List<Enemy> guardianStaleV35=new List<Enemy>();
        FairyPoseLibraryV33 guardianBankV35;
        bool guardianTriedV35;
        // Called only after the original Plain variable-phase bank has selected
        // the actual motion, source clock and source Sprite for this exact actor.
        partial void RecordPlainGuardianSmallSelectionV35(Enemy actor,SpriteRenderer renderer,PlainVariablePhaseBankR1330 bank,
            PlainVariablePhaseStateR1330 state,PlainVariablePhaseMotionR1330 motion,double clock,string poseId){
            if(actor==null||actor.sourceNPC!=GuardianSmallRigV35.Npc||actor.kind!=GuardianSmallRigV35.Kind||
                bank.npc!=actor.sourceNPC||!renderer||!renderer.sprite)return;
            if(!guardianDrawsV35.TryGetValue(actor,out var d)){d=new GuardianSmallDrawV35();guardianDrawsV35.Add(actor,d);}
            d.frame=Time.frameCount;d.carrier=renderer;d.original=renderer.sprite;d.poseId=poseId;
            d.state=state.state;d.motion=motion.animation;d.clock=(float)clock;d.duration=(float)motion.sourceMotionDuration;
        }
        partial void CheckPlainGuardianSmallRendererV35(Renderer renderer,ref bool owned){
            if(owned)return;
            foreach(var row in guardianViewsV35.Values)if(row.parts.OwnsRenderer(renderer)){owned=true;return;}
        }
        void RemoveGuardianV35(Enemy actor){
            if(!guardianViewsV35.TryGetValue(actor,out var entry))return;
            guardianViewsV35.Remove(actor);entry.parts.Dispose();
        }
        partial void DisposePlainGuardianSmallViewsV35(){
            foreach(var row in guardianViewsV35.Values)row.parts.Dispose();
            guardianViewsV35.Clear();guardianDrawsV35.Clear();guardianSeenV35.Clear();guardianFailedV35.Clear();guardianStaleV35.Clear();
            if(guardianBankV35!=null)guardianBankV35.ReleaseOwned(DestroyNativeWorldObject);
            guardianBankV35=null;guardianTriedV35=false;
        }
        partial void UpdatePlainGuardianSmallViewsV35(){
            if(!PlainProductSessionActiveR1320||game==null||!actorsRoot||!worldRoot){
                if(guardianTriedV35||guardianDrawsV35.Count!=0)DisposePlainGuardianSmallViewsV35();return;
            }
            guardianSeenV35.Clear();
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var actor=game.enemies[i];if(MonsterPixelOwnsActorV53(actor)){RemoveGuardianV35(actor);continue;}if(actor.kind!=GuardianSmallRigV35.Kind||actor.sourceNPC!=GuardianSmallRigV35.Npc)continue;
                guardianSeenV35.Add(actor);if(guardianFailedV35.Contains(actor))continue;
                var carrier=enemyViews[i];
                try{
                    if(!guardianDrawsV35.TryGetValue(actor,out var d)||d.frame!=Time.frameCount||!carrier||d.carrier!=carrier||
                        carrier.sprite!=d.original){RemoveGuardianV35(actor);continue;}
                    if(!guardianTriedV35){guardianTriedV35=true;guardianBankV35=GuardianSmallRigV35.Load();}
                    if(guardianBankV35==null)continue;
                    var tr=guardianBankV35.Find(d.motion);if(tr==null||Math.Abs(tr.duration-d.duration)>.00001f)throw new InvalidOperationException("Original Guardian source duration "+d.motion);
                    if(guardianViewsV35.TryGetValue(actor,out var row)&&row.carrier!=carrier){RemoveGuardianV35(actor);row=null;}
                    if(row==null){row=new GuardianSmallViewV35{carrier=carrier,parts=new FairyAuthoredPartsV33(carrier,guardianBankV35,DestroyNativeWorldObject)};guardianViewsV35.Add(actor,row);}
                    var local=RenderEnemyPosition(actor);var foot=actorsRoot.TransformPoint(new Vector3(local.x,local.y,0));
                    var projected=NativeWorldActive?ProjectNativePoint(foot,actor.nativeDepth):foot;
                    var frame=new FairyAuthoredPartsV33.Frame{alias=d.motion,sourceClock=d.clock,facing=carrier.flipX?-1:1,
                        visible=actor.active&&!actor.dormant&&carrier.enabled&&carrier.gameObject.activeInHierarchy};
                    row.parts.Apply(frame,projected);row.parts.VisitRenderers(r=>BindNativeSceneOcclusionR1461(r,foot,actor.nativeDepth));
                    row.poseId=d.poseId;row.state=d.state;
                    if(actor.hp>0&&i<enemyBars.Count&&enemyBars[i]&&enemyBars[i].enabled&&row.parts.TryVisibleTop(out float top)){
                        var pos=enemyBars[i].transform.position;pos.y=top+8;enemyBars[i].transform.position=pos;
                        if(i<enemyDebuffViews.Count&&enemyDebuffViews[i]){pos=enemyDebuffViews[i].transform.position;pos.y=top+20;enemyDebuffViews[i].transform.position=pos;}
                    }
                }catch(Exception e){RemoveGuardianV35(actor);guardianFailedV35.Add(actor);Debug.LogWarning("GUARDIAN_SMALL_ART_FALLBACK_V35 "+e.Message);}
            }
            guardianStaleV35.Clear();foreach(var actor in guardianViewsV35.Keys)if(!guardianSeenV35.Contains(actor))guardianStaleV35.Add(actor);
            foreach(var actor in guardianStaleV35)RemoveGuardianV35(actor);
            guardianFailedV35.RemoveWhere(a=>!guardianSeenV35.Contains(a));guardianDrawsV35.Clear();
            if(guardianSeenV35.Count==0&&guardianTriedV35)DisposePlainGuardianSmallViewsV35();
        }
    }
}
