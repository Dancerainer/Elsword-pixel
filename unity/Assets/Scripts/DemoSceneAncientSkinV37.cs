using System;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        AncientSkinPresenterV37 ancientSkinV37;
        Enemy ancientSkinOwnerV37,ancientSkinBlockedV37;

        void ReleaseAncientSkinV37(){
            if(ancientSkinV37!=null)ancientSkinV37.Dispose();
            ancientSkinV37=null;ancientSkinOwnerV37=null;
        }
        bool TryUpdateAncientSkinV37(){
            if(game==null||!actorsRoot||!worldRoot||game.CurrentRoom.id!="swamp-3"||
               game.enemies.Count==0||enemyViews.Count==0||(game.paused&&openMenu=="dungeons")){
                ReleaseAncientSkinV37();ancientSkinBlockedV37=null;return false;
            }
            var enemy=game.enemies[0];var carrier=enemyViews[0];
            if(enemy.sourceNPC==AncientRigV35.Npc&&enemy.kind==AncientRigV35.Kind&&MonsterPixelRegisteredV53(enemy,out _)){
                ReleaseAncientSkinV37();ReleaseAncientActorV35();
                if(carrier)carrier.forceRenderingOff=true;
                return true;
            }
            if(game.SwampActorHiddenV39(enemy)){ReleaseAncientSkinV37();return true;}
            if(enemy.kind!=AncientRigV35.Kind||enemy.sourceNPC!=AncientRigV35.Npc||
               !Generic7OriginalBirthV28(enemy,0)||!game.Remaining3SourceProfileV28(enemy)){
                ReleaseAncientSkinV37();ancientSkinBlockedV37=null;return false;
            }
            // A missing/stale clock uses the game's selected carrier until a witnessed frame is available.
            if(ReferenceEquals(enemy,ancientSkinBlockedV37)||ancientWitnessFrameV35!=Time.frameCount||
               !ReferenceEquals(enemy,ancientClockOwnerV35)||!carrier||!carrier.sprite||
               string.IsNullOrEmpty(ancientAliasV35)||!libraries.TryGetValue(enemy.kind,out var lib)||
               !lib.clips.TryGetValue(ancientAliasV35,out var clip)||!lib.frames.TryGetValue(ancientAliasV35,out var originals)){
                ReleaseAncientSkinV37();return true;
            }
            try {
                if(ancientSkinV37!=null&&(ancientSkinOwnerV37!=enemy||ancientSkinV37.Carrier!=carrier))
                    ReleaseAncientSkinV37();
                if(ancientPartsV35!=null)ReleaseAncientActorV35();
                if(ancientSkinV37==null){ancientSkinV37=new AncientSkinPresenterV37(carrier,DestroyNativeWorldObject);ancientSkinOwnerV37=enemy;}
                var pos=RenderEnemyPosition(enemy);var world=actorsRoot.TransformPoint(new Vector3(pos.x,pos.y,0));
                var foot=NativeWorldActive?ProjectNativePoint(world,enemy.nativeDepth):world;
                if(!ancientSkinV37.Apply(ancientAliasV35,ancientClockV35,enemy.facing,clip,originals,foot,enemy.active&&!enemy.dormant,enemy)){
                    ReleaseAncientSkinV37();return true;
                }
                if(NativeWorldActive)BindNativeSceneOcclusionR1461(ancientSkinV37.Renderer,world,enemy.nativeDepth);
                if(enemy.hp>0&&enemyBars[0].enabled&&ancientSkinV37.Visible){
                    float top=ancientSkinV37.VisibleTop;
                    var bar=enemyBars[0].transform.position;bar.y=top+8;enemyBars[0].transform.position=bar;
                    var status=enemyDebuffViews[0].transform.position;status.y=top+20;enemyDebuffViews[0].transform.position=status;
                }
            } catch(Exception ex) {
                ReleaseAncientSkinV37();ancientSkinBlockedV37=enemy;
                Debug.LogWarning("ANCIENT_SKIN_FALLBACK_V37 "+ex.Message);
            }
            return true;
        }
    }
}
