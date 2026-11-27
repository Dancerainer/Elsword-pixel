using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed class GiantWilliamFrameV39 {
        public Enemy enemy;public SpriteRenderer renderer;public Sprite carrier;
        public GiantWilliamSelectionV39 selected;public int facing,unityFrame;public float clock;
        public Vector2 foot;public bool originalFlip;public Vector3 originalScale,originalPosition;
    }
    public sealed partial class DemoScene {
        readonly Dictionary<string,GiantWilliamBankV39> giantWilliamBanksV39=new Dictionary<string,GiantWilliamBankV39>();
        readonly Dictionary<SpriteRenderer,GiantWilliamFrameV39> giantWilliamWitnessV39=new Dictionary<SpriteRenderer,GiantWilliamFrameV39>();
        readonly HashSet<string> giantWilliamBlockedV39=new HashSet<string>();
        GiantWilliamBankV39 GiantWilliamBankForV39(Enemy enemy){
            if(!giantWilliamBanksV39.TryGetValue(enemy.kind,out var bank)){
                bank=new GiantWilliamBankV39(enemy.kind,enemy.sourceNPC,spriteOffsets,DestroyNativeWorldObject);
                giantWilliamBanksV39.Add(enemy.kind,bank);
            }
            return bank;
        }
        Sprite GiantWilliamIntroductionV39(Enemy enemy,Sprite original){
            if(!GiantWilliamBankV39.Actor(enemy.kind,enemy.sourceNPC))return original;
            try{var bank=GiantWilliamBankForV39(enemy);return bank.Stage(game.CurrentRoom.id)?bank.Introduction():original;}
            catch(Exception ex){giantWilliamBlockedV39.Add(enemy.kind);Debug.LogWarning("GIANT_WILLIAM_SKIN_FALLBACK_V39 "+enemy.kind+" "+ex.Message);return original;}
        }
        void ApplyGiantWilliamV39(Enemy enemy,SpriteRenderer renderer,string alias,float clock){
            if(MonsterPixelOwnsActorV53(enemy))return;
            if(game.paused&&openMenu=="dungeons"||!renderer||!renderer.sprite||!GiantWilliamBankV39.Actor(enemy.kind,enemy.sourceNPC)||
                giantWilliamBlockedV39.Contains(enemy.kind)||!libraries.TryGetValue(enemy.kind,out var carrierLib)||
                !carrierLib.clips.TryGetValue(alias,out var originalClip)||!carrierLib.frames.TryGetValue(alias,out var originalFrames))return;
            try{
                var bank=GiantWilliamBankForV39(enemy);
                if(!bank.Stage(game.CurrentRoom.id))return;
                var binding=bank.Binding(alias);if(binding==null)return;
                bool explicitHit=false; float explicitClock=0;
                if(OriginalReactionV45.TryPhysicalOrCommittedV45(enemy,out var originalHurt,out float hitClock)&&originalHurt.motion!=binding.motion){
                    var exact=bank.Binding("source-native-"+originalHurt.motion);
                    if(exact!=null){binding=exact;explicitHit=true;explicitClock=hitClock;}
                }else if(game.RubenWilliamAttackMotionV45(enemy,out var attackMotion,out float attackClock)){
                    var original=bank.Binding("source-native-"+attackMotion);
                    if(original!=null){binding=original;explicitHit=true;explicitClock=attackClock;}
                }
                AnimationData painted=null;Sprite[] existing=null;
                if(binding.paint&&(!carrierLib.clips.TryGetValue(binding.paintClip,out painted)||
                                 !carrierLib.frames.TryGetValue(binding.paintClip,out existing)))return;
                GiantWilliamSelectionV39 result;
                bool picked=explicitHit?bank.SelectOriginalHitV45(binding,explicitClock,painted,existing,out result):
                    bank.Select(binding,originalClip,originalFrames,renderer.sprite,clock,painted,existing,out result);
                if(!picked)return;
                if(!giantWilliamWitnessV39.TryGetValue(renderer,out var w)){w=new GiantWilliamFrameV39();giantWilliamWitnessV39.Add(renderer,w);}
                w.enemy=enemy;w.renderer=renderer;w.carrier=renderer.sprite;w.originalScale=renderer.transform.localScale;
                w.originalPosition=renderer.transform.localPosition;w.originalFlip=renderer.flipX;w.facing=enemy.facing;
                w.unityFrame=Time.frameCount;w.clock=clock;w.selected=result;w.foot=RenderEnemyPosition(enemy);
                renderer.sprite=result.sprite;renderer.flipX=enemy.facing!=bank.Data.authoredFacing;
                // The original NPC scale and LK ratio have already been baked or registered in V39.
                // The secondary Tree white giant has a legacy 1.8 display multiplier in its carrier.
                float xScale=result.scaleX,yScale=result.scaleY;
                renderer.transform.localScale=new Vector3(xScale,yScale,1);
                var offset=SpriteOffset(renderer);
                renderer.transform.localPosition=new Vector3(w.foot.x+offset.x*xScale,w.foot.y+offset.y*yScale,w.originalPosition.z);
            }catch(Exception ex){
                giantWilliamBlockedV39.Add(enemy.kind);Debug.LogWarning("GIANT_WILLIAM_SKIN_FALLBACK_V39 "+enemy.kind+" "+ex.Message);
            }
        }
        public GiantWilliamFrameV39 GiantWilliamFrameV39For(SpriteRenderer renderer){
            return renderer&&giantWilliamWitnessV39.TryGetValue(renderer,out var w)&&w.unityFrame==Time.frameCount&&
                   ReferenceEquals(renderer.sprite,w.selected.sprite)?w:null;
        }
        bool GiantWilliamWorldTopV39(SpriteRenderer renderer,out float top){
            top=0;if(!renderer||!giantWilliamWitnessV39.TryGetValue(renderer,out var w)||!ReferenceEquals(renderer.sprite,w.selected.sprite))return false;
            top=renderer.transform.TransformPoint(new Vector3(0,w.selected.top,0)).y;return true;
        }
        float GiantWilliamLocalTopV39(Enemy enemy,SpriteRenderer renderer,float previous){
            var w=GiantWilliamFrameV39For(renderer);return w==null?previous:
                renderer.transform.localPosition.y+w.selected.top*Mathf.Abs(renderer.transform.localScale.y)-enemy.y;
        }
        void DisposeGiantWilliamV39(){
            foreach(var w in giantWilliamWitnessV39.Values){
                var r=w.renderer;if(!r||!ReferenceEquals(r.sprite,w.selected.sprite))continue;
                r.sprite=w.carrier;r.flipX=w.originalFlip;r.transform.localPosition=w.originalPosition;r.transform.localScale=w.originalScale;
            }
            foreach(var bank in giantWilliamBanksV39.Values)bank.Dispose();
            giantWilliamWitnessV39.Clear();giantWilliamBanksV39.Clear();giantWilliamBlockedV39.Clear();
        }
    }
}
