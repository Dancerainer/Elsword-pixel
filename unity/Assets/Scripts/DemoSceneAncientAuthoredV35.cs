using System;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        FairyPoseLibraryV33 ancientLibraryV35;
        FairyAuthoredPartsV33 ancientPartsV35;
        Enemy ancientOwnerV35,ancientClockOwnerV35,ancientBlockedV35;
        SpriteRenderer ancientCarrierV35;
        string ancientAliasV35;
        float ancientClockV35;
        int ancientWitnessFrameV35=-1;

        void RecordAncientSelectionV35(Enemy e,string alias,float clock) {
            if(e.kind!=AncientRigV35.Kind || e.sourceNPC!=AncientRigV35.Npc)return;
            ancientClockOwnerV35=e;ancientAliasV35=alias;ancientClockV35=clock;ancientWitnessFrameV35=Time.frameCount;
        }
        bool AncientOwnsRendererV35(Renderer r) { return (ancientSkinV37!=null && ancientSkinV37.Renderer==r) || ancientPartsV35!=null && ancientPartsV35.OwnsRenderer(r); }
        void ReleaseAncientActorV35() {
            if(ancientPartsV35!=null)ancientPartsV35.Dispose();
            ancientPartsV35=null;ancientOwnerV35=null;ancientCarrierV35=null;
        }
        void DisposeAncientViewsV35() {
            ReleaseAncientSkinV37();ancientSkinBlockedV37=null;
            ReleaseAncientActorV35();
            if(ancientLibraryV35!=null)ancientLibraryV35.ReleaseOwned(DestroyNativeWorldObject);
            ancientLibraryV35=null;ancientClockOwnerV35=null;ancientBlockedV35=null;ancientWitnessFrameV35=-1;
        }
        bool AncientFrameV35(Enemy e,SpriteRenderer renderer,out FairyAuthoredPartsV33.Frame frame) {
            frame=default;
            if(ancientWitnessFrameV35!=Time.frameCount || !ReferenceEquals(ancientClockOwnerV35,e) || !renderer || !renderer.sprite ||
                string.IsNullOrEmpty(ancientAliasV35) || float.IsNaN(ancientClockV35) || float.IsInfinity(ancientClockV35) || ancientClockV35<0 ||
                !libraries.TryGetValue(e.kind,out var lib) || !lib.frames.TryGetValue(ancientAliasV35,out var originals) ||
                !lib.clips.TryGetValue(ancientAliasV35,out var clip))return false;
            var track=ancientLibraryV35.Find(ancientAliasV35);
            if(track==null)return false;
            float t=track.loop?Mathf.Repeat(ancientClockV35,track.duration):Mathf.Clamp(ancientClockV35,0,track.duration);
            int frameIndex=Mathf.Clamp(Mathf.FloorToInt(ancientClockV35*track.fps),0,track.count-1);
            if(track.loop)frameIndex=Mathf.Clamp(Mathf.FloorToInt(t*track.fps),0,track.count-1);
            if(originals.Length!=track.count || clip.fps!=track.fps || clip.loop!=track.loop || Mathf.Abs(clip.duration-track.duration)>.00001f ||
                !ReferenceEquals(originals[frameIndex],renderer.sprite))return false;
            frame=new FairyAuthoredPartsV33.Frame{alias=track.alias,sourceClock=ancientClockV35,facing=e.facing,
                visible=e.active && !e.dormant && renderer.enabled && renderer.gameObject.activeInHierarchy};
            return true;
        }
        void UpdateAncientViewsV35() {
            if(TryUpdateAncientSkinV37())return;
            if(game==null || !actorsRoot || !worldRoot || game.CurrentRoom.id!="swamp-3" || game.enemies.Count==0 ||
                enemyViews.Count==0 || (game.paused && openMenu=="dungeons")) {
                if(ancientPartsV35!=null || ancientLibraryV35!=null)DisposeAncientViewsV35();
                return;
            }
            var e=game.enemies[0];var renderer=enemyViews[0];
            if(MonsterPixelOwnsActorV53(e)){ReleaseAncientActorV35();return;}
            if(e.kind!=AncientRigV35.Kind || e.sourceNPC!=AncientRigV35.Npc || !Generic7OriginalBirthV28(e,0) ||
                !game.Remaining3SourceProfileV28(e) || ReferenceEquals(ancientBlockedV35,e)) { ReleaseAncientActorV35();return; }
            try {
                if(ancientLibraryV35==null)ancientLibraryV35=AncientRigV35.Load();
                if(!AncientFrameV35(e,renderer,out var frame)){ReleaseAncientActorV35();return;}
                if(ancientPartsV35!=null && (ancientOwnerV35!=e || ancientCarrierV35!=renderer))ReleaseAncientActorV35();
                if(ancientPartsV35==null) {
                    ancientPartsV35=new FairyAuthoredPartsV33(renderer,ancientLibraryV35,DestroyNativeWorldObject);
                    ancientOwnerV35=e;ancientCarrierV35=renderer;
                }
                var pos=RenderEnemyPosition(e);
                var world=actorsRoot.TransformPoint(new Vector3(pos.x,pos.y,0));
                ancientPartsV35.Apply(frame,NativeWorldActive?ProjectNativePoint(world,e.nativeDepth):world);
                if(NativeWorldActive)ancientPartsV35.VisitRenderers(r=>BindNativeSceneOcclusionR1461(r,world,e.nativeDepth));
                if(e.hp>0 && enemyBars[0].enabled && ancientPartsV35.TryVisibleTop(out float top)) {
                    var bar=enemyBars[0].transform.position;bar.y=top+8;enemyBars[0].transform.position=bar;
                    var status=enemyDebuffViews[0].transform.position;status.y=top+20;enemyDebuffViews[0].transform.position=status;
                }
            } catch(Exception ex) {
                ReleaseAncientActorV35();ancientBlockedV35=e;Debug.LogWarning("ANCIENT_AUTHORED_FALLBACK_V35 "+ex.Message);
            }
        }
    }
}
