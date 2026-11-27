using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public static class CoreElitePartsV36 {
        public const string Guard="NUI_NASOD_ELITE_GUARD",Sheriff="NUI_NASOD_ELITE_SHERIFF";
        public const string Wally="NUI_WALLY_9TH_TYPE2";
        internal static bool Owns(string npc){return npc==Guard||npc==Sheriff||npc==Wally;}
        internal static int PartCount(string npc){return npc==Wally?16:npc==Guard?15:14;}
        public static string Root(string npc){
            if(!Owns(npc))throw new ArgumentException("Expected original Core guard or sheriff",nameof(npc));
            return "Pixel/feedback-v36/"+(npc==Wally?"wally-9th-type2/":npc==Guard?"core-elite-guard/":"core-elite-sheriff/");
        }
        public static FairyPoseLibraryV33 Load(string npc) {
            if(npc==Wally)return WallyType2PartsV36.LoadRig();
            bool guard=npc==Guard;
            string pin=guard?"fd078f924b40cedf053ea3d191642ae32a7d549759d0227195550d250e723224":"a980c9a53758d502a66687c9e8f08d36b14ce7d9efd3a16dd085f88f85546ab5";
            string ksm=guard?"ea7dc1e0fc86f886be64783b67b9ff68f41efc9159418d4dbbd04e997a3facb5":"cd3e8c55ae63fb979b43dfb1083c67a000b8ce322e7dc0ac83d7cacb433cd146";
            return RegisteredPartRigV35.Load(Root(npc),npc,pin,g=>{
                if(g.parts.Length!=(guard?15:14)||g.tracks.Length!=(guard?20:23)||g.authoredFacing!=1||g.nativeYaw!=0||g.motionSha256!=ksm)
                    throw new InvalidOperationException("Original guard/sheriff multipart registration.");
                foreach(var required in new[]{"head","torso","waist","near-boot","far-boot","near-grip","far-grip",guard?"near-blade":"gun"})
                    if(!Array.Exists(g.parts,p=>p.name==required))throw new InvalidOperationException("Missing Core mechanical part "+required);
                if(guard&&!Array.Exists(g.parts,p=>p.name=="shield"))throw new InvalidOperationException("Original purple shield.");
                foreach(var t in g.tracks)if(t.animation!=t.alias||t.fps!=24||t.components!=5||t.loop)
                    throw new InvalidOperationException("Original source motion "+t.alias);
            });
        }
    }

    public sealed partial class DemoScene {
        sealed class EliteViewV36 {
            internal SpriteRenderer carrier;internal Sprite source;internal FairyAuthoredPartsV33 parts;
            internal FairyPoseLibraryV33 library;internal string motion;
            internal float clock;internal int frame;internal bool failed;
            internal SpriteRenderer[] depthParts;internal float[] depthValues;
        }
        readonly Dictionary<Enemy,EliteViewV36> eliteViewsV36=new Dictionary<Enemy,EliteViewV36>();
        readonly Dictionary<string,FairyPoseLibraryV33> eliteLibrariesV36=new Dictionary<string,FairyPoseLibraryV33>();
        WallyType2DepthV36 wallyDepthV36;
        readonly List<Enemy> staleEliteV36=new List<Enemy>();
        void RecordCoreElitePartsV36(Enemy actor,SpriteRenderer carrier,string motion,double clock){
            if(!CoreElitePartsV36.Owns(actor.sourceNPC)||(!coreEntrySessionV21&&!(actor.sourceNPC==CoreElitePartsV36.Wally&&FactoryProductSessionActiveR1179))||!carrier||!carrier.enabled||!carrier.sprite)return;
            if(!eliteViewsV36.TryGetValue(actor,out var v)){v=new EliteViewV36();eliteViewsV36.Add(actor,v);}
            if(v.carrier&&v.carrier!=carrier){v.parts?.Dispose();v.parts=null;v.failed=false;}
            v.frame=Time.frameCount;v.carrier=carrier;v.source=carrier.sprite;v.motion=motion;v.clock=(float)clock;
        }
        internal bool VisitCoreElitePartsV36(Enemy actor,Action<SpriteRenderer> visitor) {
            if(!eliteViewsV36.TryGetValue(actor,out var view)||view.parts==null)return false;
            view.parts.VisitRenderers(visitor);return true;
        }
        bool CoreEliteOwnsRendererV36(Renderer renderer){
            foreach(var v in eliteViewsV36.Values)if(v.parts!=null&&v.parts.OwnsRenderer(renderer))return true;
            return false;
        }
        void DisposeCoreElitePartsV36(){
            foreach(var v in eliteViewsV36.Values)v.parts?.Dispose();eliteViewsV36.Clear();staleEliteV36.Clear();
            foreach(var lib in eliteLibrariesV36.Values)lib.ReleaseOwned(DestroyNativeWorldObject);eliteLibrariesV36.Clear();wallyDepthV36=null;
        }
        void UpdateCoreElitePartsV36(){
            if((!coreEntrySessionV21&&!FactoryProductSessionActiveR1179)||game==null||!actorsRoot){
                if(eliteViewsV36.Count!=0||eliteLibrariesV36.Count!=0)DisposeCoreElitePartsV36();
                return;
            }
            staleEliteV36.Clear();
            foreach(var pair in eliteViewsV36){
                var e=pair.Key;var v=pair.Value;int i=game.enemies.IndexOf(e);
                if(MonsterPixelOwnsActorV53(e)){v.parts?.Dispose();staleEliteV36.Add(e);continue;}
                if(v.frame!=Time.frameCount||i<0||i>=enemyViews.Count||enemyViews[i]!=v.carrier||!v.carrier||
                    !v.carrier.enabled||!MonsterTextureRedrawV22.SameSourceFrameV35(v.carrier.sprite,v.source)){
                    v.parts?.Dispose();staleEliteV36.Add(e);continue;
                }
                if(v.failed)continue;
                try{
                    if(v.parts==null){
                        if(!eliteLibrariesV36.TryGetValue(e.sourceNPC,out var lib)){
                            lib=CoreElitePartsV36.Load(e.sourceNPC);eliteLibrariesV36.Add(e.sourceNPC,lib);
                        }
                        v.library=lib;v.parts=new FairyAuthoredPartsV33(v.carrier,lib,DestroyNativeWorldObject);
                        if(e.sourceNPC==CoreElitePartsV36.Wally){
                            if(wallyDepthV36==null)wallyDepthV36=WallyType2DepthV36.Load(lib);
                            var list=new List<SpriteRenderer>(lib.parts.Length);v.parts.VisitRenderers(list.Add);
                            v.depthParts=list.ToArray();v.depthValues=new float[lib.parts.Length];
                        }
                    }
                    var track=v.library.Find(v.motion);
                    if(track==null)throw new InvalidOperationException("No original elite pose "+v.motion);
                    var pos=RenderEnemyPosition(e);var world=actorsRoot.TransformPoint(new Vector3(pos.x,pos.y,0));
                    bool visible=v.carrier.gameObject.activeInHierarchy;
                    v.parts.Apply(new FairyAuthoredPartsV33.Frame{alias=v.motion,sourceClock=Mathf.Clamp(v.clock,0,track.duration),
                        facing=e.facing<0?-1:1,visible=visible},NativeWorldActive?ProjectNativePoint(world,e.nativeDepth):world,
                        EliteWholeScaleV35);
                    if(v.depthParts!=null)wallyDepthV36.Apply(v.motion,v.clock,v.depthParts,v.depthValues);
                    if(NativeWorldActive)v.parts.VisitRenderers(r=>BindNativeSceneOcclusionR1461(r,world,e.nativeDepth));
                    if(visible&&e.hp>0&&i<enemyBars.Count&&enemyBars[i]&&enemyBars[i].enabled&&v.parts.TryVisibleTop(out float top)){
                        var bar=enemyBars[i].transform.position;bar.y=top+8;enemyBars[i].transform.position=bar;
                        if(i<enemyDebuffViews.Count&&enemyDebuffViews[i]){bar.y=top+20;enemyDebuffViews[i].transform.position=bar;}
                    }
                }catch(Exception error){
                    v.parts?.Dispose();v.parts=null;v.failed=true;Debug.LogWarning("CORE_ELITE_ART_FALLBACK_V36 "+e.sourceNPC+" "+error.Message);
                }
            }
            foreach(var actor in staleEliteV36)eliteViewsV36.Remove(actor);
        }
    }
}
