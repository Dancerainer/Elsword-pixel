using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public static class NasodSprayRigV35 {
        public const string Plain="NUI_NASOD_SPRAY",Ice="NUI_NASOD_SPRAY_ICE";
        public const int PartCount=15;
        public static string Root(string npc) { return "Pixel/feedback-v35/"+(npc==Ice?"nasod-spray-ice/":"nasod-spray/"); }
        public static FairyPoseLibraryV33 Load(string npc) {
            if(npc!=Plain && npc!=Ice)throw new ArgumentException("Original Nasod Spray identity",nameof(npc));
            string pages=npc==Ice?"d9a28e06d674a6b3f35f000fe6102e8f2ac66ed54bca2e629a805593a9e4d21b":"1043dc9f990810d913507837593c1c7c110f86b8d47cd110c23270eff3d4a884";
            return RegisteredPartRigV35.Load(Root(npc),npc,pages,lib=>{
                if(lib.parts.Length!=PartCount || lib.tracks.Length!=22||lib.authoredFacing!=1||lib.nativeYaw!=0 ||
                    lib.motionSha256!=(npc==Ice?"468e51352782befecb5fd7d6696d2e855c825f3f478e7204aa2c4d8c74d960a5":"c57c35b2d4d90048b3bf653332d358dd0b28c108f290fa47087a512a533c98ad") || lib.parts[11].name!="nozzle" ||
                    lib.parts[13].name!="left-boot" || lib.parts[14].name!="right-boot")
                    throw new InvalidOperationException("Original asymmetrical Nasod Spray, four-port nozzle and independent metal feet");
                foreach(var t in lib.tracks)if(t.alias!=t.animation||t.loop||t.fps!=30||t.components!=5)
                    throw new InvalidOperationException("Original Nasod Spray source phase "+t.alias);
            });
        }
    }
    public sealed partial class DemoScene {
        sealed class NasodSprayViewV35 {
            internal SpriteRenderer carrier;internal Sprite original;internal FairyAuthoredPartsV33 parts;internal Transform artRoot;
            internal string motion,state,pose;internal float sourceClock,partClock,originalZ;internal int frame,facing;
        }
        readonly Dictionary<Enemy,NasodSprayViewV35> nasodSprayViewsV35=new Dictionary<Enemy,NasodSprayViewV35>();
        readonly Dictionary<string,FairyPoseLibraryV33> nasodSprayRigsV35=new Dictionary<string,FairyPoseLibraryV33>();
        readonly HashSet<Enemy> nasodSprayFailedV35=new HashSet<Enemy>();
        readonly List<Enemy> nasodSprayStaleV35=new List<Enemy>();
        void RememberNasodSprayV35(Enemy actor,SpriteRenderer carrier,string motion,string state,string pose,float clock,float drawClock,int facing,float z){
            if(!carrier || !carrier.sprite || !carrier.enabled || string.IsNullOrEmpty(motion)||string.IsNullOrEmpty(pose)||
                clock<0||float.IsNaN(clock)||float.IsInfinity(clock))return;
            if(!nasodSprayViewsV35.TryGetValue(actor,out var view)){view=new NasodSprayViewV35();nasodSprayViewsV35.Add(actor,view);}
            if(view.carrier!=carrier){view.parts?.Dispose();view.parts=null;}
            view.carrier=carrier;view.original=carrier.sprite;view.motion=motion;view.state=state;view.pose=pose;
            view.sourceClock=clock;view.partClock=drawClock;view.frame=Time.frameCount;view.facing=facing;view.originalZ=z;
        }
        partial void RecordPlainSpraySelectionV35(Enemy actor,SpriteRenderer renderer,PlainVariablePhaseBankR1330 bank,
            PlainVariablePhaseStateR1330 state,PlainVariablePhaseMotionR1330 motion,double clock,string poseId){
            if(actor.sourceNPC!=NasodSprayRigV35.Plain || bank.npc!=actor.sourceNPC || actor.kind!="nasod-spray" ||
                bank.sourceMotion.sha256!="c57c35b2d4d90048b3bf653332d358dd0b28c108f290fa47087a512a533c98ad" || !PlainProductSessionActiveR1320 ||
                (plainProductRouteR1320!=PlainProductResourceIndexR1320.Normal&&plainProductRouteR1320!=PlainProductResourceIndexR1320.Recycle))return;
            float z=state.state=="NASOD_SPRAY_DAMAGE_FLY_FRONT"?-45:0;
            RememberNasodSprayV35(actor,renderer,motion.animation,state.state,poseId,(float)clock,(float)clock,renderer.flipX?-1:1,z);
        }
        void RecordCoreSpraySelectionV35(Enemy actor,SpriteRenderer renderer,SprayWholeSelectionR203 selected){
            if(!CoreHardProductSessionActiveR1147||actor.sourceNPC!=NasodSprayRigV35.Ice||actor.kind!="nasod-spray-ice")return;
            var state=selected.State;
            // Core's registered whole deliberately has fixed facing. Its physical
            // damage-fly carrier supplies the native -45 degree display rotation.
            float clock=selected.SourceMotionClock;
            RememberNasodSprayV35(actor,renderer,state.Motion,state.Id,selected.SourcePose,clock,
                Mathf.Min(clock,state.MotionDuration),1,(float)state.OriginalAddRotateZ);
        }
        partial void CheckPlainSprayRendererV35(Renderer r,ref bool owned){
            if(owned)return;
            foreach(var v in nasodSprayViewsV35.Values)if(v.parts!=null&&v.parts.OwnsRenderer(r)){owned=true;return;}
        }
        internal bool VisitNasodSprayPartsV35(Enemy actor,Action<SpriteRenderer> visitor){
            if(actor==null||!nasodSprayViewsV35.TryGetValue(actor,out var v)||v.parts==null)return false;
            v.parts.VisitRenderers(visitor);return true;
        }
        internal bool NasodSprayOwnsRendererV35(Enemy actor,Renderer renderer){
            return actor!=null&&nasodSprayViewsV35.TryGetValue(actor,out var v)&&v.parts!=null&&v.parts.OwnsRenderer(renderer);
        }
        partial void UpdatePlainSprayViewsV35(){
            if(game==null||!actorsRoot||(!PlainProductSessionActiveR1320&&!CoreHardProductSessionActiveR1147)){
                if(nasodSprayViewsV35.Count+nasodSprayRigsV35.Count!=0)DisposePlainSprayViewsV35();
                return;
            }
            nasodSprayStaleV35.Clear();
            foreach(var pair in nasodSprayViewsV35){
                var actor=pair.Key;var v=pair.Value;var c=v.carrier;int i=game.enemies.IndexOf(actor);
                if(MonsterPixelOwnsActorV53(actor)){v.parts?.Dispose();nasodSprayStaleV35.Add(actor);continue;}
                if(v.frame!=Time.frameCount||!c||!MonsterTextureRedrawV22.SameSourceFrameV35(c.sprite,v.original)||i<0||i>=enemyViews.Count||enemyViews[i]!=c){
                    v.parts?.Dispose();nasodSprayStaleV35.Add(actor);continue;
                }
                if(nasodSprayFailedV35.Contains(actor))continue;
                try{
                    if(v.parts==null){
                        if(!nasodSprayRigsV35.TryGetValue(actor.sourceNPC,out var rig)){
                            rig=NasodSprayRigV35.Load(actor.sourceNPC);nasodSprayRigsV35.Add(actor.sourceNPC,rig);
                        }
                        v.parts=new FairyAuthoredPartsV33(c,rig,DestroyNativeWorldObject);
                        v.parts.VisitRenderers(r=>{if(!v.artRoot)v.artRoot=r.transform.parent;});
                    }
                    var pos=RenderEnemyPosition(actor);var world=actorsRoot.TransformPoint(new Vector3(pos.x,pos.y,0));
                    var foot=NativeWorldActive?ProjectNativePoint(world,actor.nativeDepth):world;
                    bool visible=c.enabled&&c.gameObject.activeInHierarchy;
                    v.parts.Apply(new FairyAuthoredPartsV33.Frame{alias=v.motion,sourceClock=v.partClock,facing=v.facing,visible=visible},foot);
                    if(actor.sourceNPC==NasodSprayRigV35.Plain && v.originalZ!=0)v.artRoot.localRotation=Quaternion.Euler(0,0,v.originalZ);
                    if(NativeWorldActive)v.parts.VisitRenderers(r=>BindNativeSceneOcclusionR1461(r,world,actor.nativeDepth));
                    if(visible&&actor.hp>0&&enemyBars[i].enabled&&v.parts.TryVisibleTop(out var top)){
                        var bar=enemyBars[i].transform.position;bar.y=top+8;enemyBars[i].transform.position=bar;
                        var debuff=enemyDebuffViews[i].transform.position;debuff.y=top+20;enemyDebuffViews[i].transform.position=debuff;
                    }
                }catch(Exception e){
                    v.parts?.Dispose();v.parts=null;nasodSprayFailedV35.Add(actor);
                    Debug.LogWarning("NASOD_SPRAY_ART_FALLBACK_V35 "+actor.sourceNPC+" "+e.Message);
                }
            }
            foreach(var a in nasodSprayStaleV35){nasodSprayViewsV35.Remove(a);nasodSprayFailedV35.Remove(a);}
        }
        partial void DisposePlainSprayViewsV35(){
            ClearNasodSprayBallsV35();
            foreach(var v in nasodSprayViewsV35.Values)v.parts?.Dispose();
            nasodSprayViewsV35.Clear();nasodSprayStaleV35.Clear();nasodSprayFailedV35.Clear();
            foreach(var r in nasodSprayRigsV35.Values)r.ReleaseOwned(DestroyNativeWorldObject);
            nasodSprayRigsV35.Clear();
        }
    }
}
