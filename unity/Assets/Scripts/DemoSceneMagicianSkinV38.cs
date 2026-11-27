using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed class MagicianSkinFrameV38 {
        public Enemy enemy;public SpriteRenderer renderer;public Sprite carrier,selected;
        public string alias,sourceAnimation,page;public int index,facing,unityFrame;
        public float clock;public Vector2 foot;public bool originalFlip;
        public Vector3 originalScale,originalPosition;
    }
    public sealed partial class DemoScene {
        readonly Dictionary<string,NativePoseSpriteBankV38> mageBanksV38=new Dictionary<string,NativePoseSpriteBankV38>();
        readonly Dictionary<SpriteRenderer,MagicianSkinFrameV38> mageWitnessV38=new Dictionary<SpriteRenderer,MagicianSkinFrameV38>();
        readonly HashSet<string> mageBlockedV38=new HashSet<string>();
        static bool ForestPlantIdentityV38(Enemy enemy){
            return (enemy.kind=="anger-forest-ent"&&enemy.sourceNPC=="NUI_ANGER_FOREST_ENT")||
                   (enemy.kind=="anger-forest-fairy"&&enemy.sourceNPC=="NUI_ANGER_FOREST_FAIRY");
        }
        bool MagicianScopeV38(Enemy enemy){
            if(enemy.kind=="cannon-big-down")return BattleProductSessionActiveR1325&&game.room==2&&enemy.sourceNPC=="NUI_CANNON_BIG_DOWN"&&
                enemy.sourceKeyCode==1&&enemy.active&&!enemy.dormant&&enemy.isGate;
            if(ForestPlantIdentityV38(enemy))return selectedDungeon==3&&Dungeon.sourceGraph==Alpha4NormalNativeRulesV28.Forest&&CurrentNpcContentSet==null;
            if(enemy.kind=="forest-dark-magician"&&enemy.sourceNPC=="NUI_FOREST_DARK_MAGICIAN")
                return selectedDungeon==3&&Dungeon.sourceGraph==Next3NormalNativeRulesV28.Forest&&CurrentNpcContentSet==null;
            return enemy.kind=="ghost-magician-ragged"&&enemy.sourceNPC=="NUI_GHOST_MAGICIAN_RAGGED"&&
                   CurrentNpcContentSet!=null&&CurrentNpcContentSet.sourceDungeon=="Dungeon_Elder_Below_Passage_Normal";
        }
        void ApplyMagicianSkinV38(Enemy enemy,SpriteRenderer renderer,string alias,float clock){
            if(MonsterPixelOwnsActorV53(enemy))return;
            if(game.paused&&openMenu=="dungeons")return;
            if(!renderer||!renderer.sprite||!MagicianScopeV38(enemy)||mageBlockedV38.Contains(enemy.kind)||
               !libraries.TryGetValue(enemy.kind,out var carrierLib)||!carrierLib.clips.TryGetValue(alias,out var oldClip)||
               !carrierLib.frames.TryGetValue(alias,out var oldFrames))return;
            try{
                if(!mageBanksV38.TryGetValue(enemy.kind,out var bank)){
                    if(enemy.kind=="cannon-big-down")bank=new DeferredCannonSkinV39(DestroyNativeWorldObject,spriteOffsets);
                    else if(ForestPlantIdentityV38(enemy))bank=new ForestPlantPoseBankV38(enemy.kind,enemy.sourceNPC,DestroyNativeWorldObject,spriteOffsets);
                    else bank=new MagicianSkinBankV38(enemy.kind,enemy.sourceNPC,DestroyNativeWorldObject,spriteOffsets);
                    mageBanksV38.Add(enemy.kind,bank);
                }
                if(!bank.Select(alias,clock,oldClip,oldFrames,renderer.sprite,out var clip,out var index,out var pose))return;
                if(!mageWitnessV38.TryGetValue(renderer,out var witness)){
                    witness=new MagicianSkinFrameV38();mageWitnessV38.Add(renderer,witness);
                }
                witness.enemy=enemy;witness.renderer=renderer;witness.carrier=renderer.sprite;
                witness.originalFlip=renderer.flipX;witness.originalPosition=renderer.transform.localPosition;
                witness.originalScale=renderer.transform.localScale;
                var foot=RenderEnemyPosition(enemy);
                witness.alias=alias;witness.sourceAnimation=clip.sourceAnimation;witness.index=index;witness.clock=clock;
                witness.facing=enemy.facing;witness.foot=foot;witness.selected=pose;witness.page=clip.frames[index].texture;witness.unityFrame=Time.frameCount;
                renderer.sprite=pose;renderer.flipX=enemy.facing!=bank.Data.authoredFacing;
                float scale=enemy.visualScale*bank.Data.logicalScale;
                renderer.transform.localScale=Vector3.one*scale;
                var position=foot+SpriteOffset(renderer)*scale;
                renderer.transform.localPosition=new Vector3(position.x,position.y,witness.originalPosition.z);
            }catch(Exception ex){
                mageBlockedV38.Add(enemy.kind);Debug.LogWarning("MAGICIAN_SKIN_FALLBACK_V38 "+enemy.kind+" "+ex.Message);
            }
        }
        public MagicianSkinFrameV38 MagicianFrameV38(SpriteRenderer renderer){
            return renderer&&mageWitnessV38.TryGetValue(renderer,out var selected)&&
                   selected.unityFrame==Time.frameCount&&ReferenceEquals(renderer.sprite,selected.selected)?selected:null;
        }
        float MagicianVisibleTopV38(Enemy enemy,SpriteRenderer renderer,float prior){
            var witness=MagicianFrameV38(renderer);
            return witness==null?prior:renderer.transform.localPosition.y+renderer.sprite.bounds.extents.y*Mathf.Abs(renderer.transform.localScale.y)-enemy.y;
        }
        void DisposeMagicianSkinsV38(){
            foreach(var witness in mageWitnessV38.Values){
                var r=witness.renderer;if(!r||!ReferenceEquals(r.sprite,witness.selected))continue;
                r.sprite=witness.carrier;r.flipX=witness.originalFlip;r.transform.localPosition=witness.originalPosition;r.transform.localScale=witness.originalScale;
            }
            foreach(var bank in mageBanksV38.Values)bank.Dispose();
            mageWitnessV38.Clear();mageBanksV38.Clear();mageBlockedV38.Clear();
        }
    }
}
