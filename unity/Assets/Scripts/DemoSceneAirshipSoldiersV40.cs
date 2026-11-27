using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed class AirshipSoldierFrameV40 {
        public string route;public Enemy actor;public SpriteRenderer renderer;public Sprite carrier;
        public AirshipSelectionV40 selected;public int facing,unityFrame;public float clock;public Vector2 foot;
        public Vector3 originalScale,originalPosition;public bool originalFlip;
    }
    public sealed partial class DemoScene {
        readonly Dictionary<string,AirshipSoldierBankV40> airshipSoldierBanksV40=new Dictionary<string,AirshipSoldierBankV40>();
        readonly Dictionary<SpriteRenderer,AirshipSoldierFrameV40> airshipSoldierWitnessV40=new Dictionary<SpriteRenderer,AirshipSoldierFrameV40>();
        readonly HashSet<string> airshipSoldierBlockedV40=new HashSet<string>();
        void ResolveAirshipSoldierAliasV40(Enemy actor,ref string alias){
            if(!AirshipSoldierBankV40.Known(actor.kind,actor.sourceNPC)||!libraries.TryGetValue(actor.kind,out var library)||library.frames.ContainsKey(alias))return;
            bool battle=BattleProductSessionActiveR1325&&library.data.texture=="battle-product-owned-r1325/"+actor.kind;
            bool recycle=PlainProductSessionActiveR1320&&plainProductRouteR1320==PlainProductResourceIndexR1320.Recycle&&
                library.data.texture=="plain-product-owned-r1320/"+actor.kind;
            if(!battle&&!recycle)return;
            // The product state table owns the whole drawings. Resolve generic combat labels only when the original library lacks them.
            string source;
            switch(alias){
                case "idle": source="source-airship-soldier-high-wait";break;
                case "launched":source=actor.vy>0?"air-up":"air-down";break;
                case "knockdown":source="down";break;
                case "getup":source="get-up";break;
                case "death":source=actor.groundLine<0?"air-landing":"down";break;
                default:return;
            }
            if(library.frames.ContainsKey(source))alias=source;
        }
        void ApplyAirshipSoldierV40(Enemy e,SpriteRenderer r,string alias,float clock){
            if(MonsterPixelOwnsActorV53(e))return;
            if(game.paused&&openMenu=="dungeons"||!r||!r.sprite||!AirshipSoldierBankV40.Known(e.kind,e.sourceNPC)||
               airshipSoldierBlockedV40.Contains(e.kind)||!libraries.TryGetValue(e.kind,out var lib))return;
            string route;
            if(BattleProductSessionActiveR1325&&lib.data.texture=="battle-product-owned-r1325/"+e.kind)route="Battle";
            else if(PlainProductSessionActiveR1320&&plainProductRouteR1320==PlainProductResourceIndexR1320.Recycle&&
                    (e.kind=="airship-soldier-gunner"||e.kind=="airship-soldier-swordman")&&
                    lib.data.texture=="plain-product-owned-r1320/"+e.kind)route="Recycle";
            else return;
            if(!lib.clips.TryGetValue(alias,out var clip)||!lib.frames.TryGetValue(alias,out var frames))return;
            try{
                if(!airshipSoldierBanksV40.TryGetValue(e.kind,out var bank)){
                    bank=new AirshipSoldierBankV40(e.kind,e.sourceNPC,spriteOffsets,DestroyNativeWorldObject);airshipSoldierBanksV40.Add(e.kind,bank);
                }
                if(Array.IndexOf(bank.Data.routes,route)<0||Mathf.Abs(e.visualScale-1)>.0001f)
                    throw new InvalidOperationException("Airship source route or duplicate model scaling "+e.kind);
                if(!bank.Select(alias,clock,clip,frames,r.sprite,out var selected))return;
                if(!airshipSoldierWitnessV40.TryGetValue(r,out var w)){w=new AirshipSoldierFrameV40();airshipSoldierWitnessV40.Add(r,w);}
                w.actor=e;w.renderer=r;w.route=route;w.carrier=r.sprite;w.originalFlip=r.flipX;
                w.originalScale=r.transform.localScale;w.originalPosition=r.transform.localPosition;w.unityFrame=Time.frameCount;
                w.clock=clock;w.facing=e.facing;w.foot=RenderEnemyPosition(e);w.selected=selected;
                r.sprite=selected.sprite;r.flipX=e.facing!=bank.Data.authoredFacing;
                r.transform.localScale=Vector3.one*selected.scale;
                var offset=SpriteOffset(r)*selected.scale;
                r.transform.localPosition=new Vector3(w.foot.x+offset.x,w.foot.y+offset.y,w.originalPosition.z);
            }catch(Exception ex){
                airshipSoldierBlockedV40.Add(e.kind);Debug.LogWarning("AIRSHIP_SOLDIER_SKIN_FALLBACK_V40 "+e.kind+" "+ex.Message);
            }
        }
        public AirshipSoldierFrameV40 AirshipSoldierFrameForV40(SpriteRenderer r){return r&&airshipSoldierWitnessV40.TryGetValue(r,out var w)&&
            w.unityFrame==Time.frameCount&&ReferenceEquals(r.sprite,w.selected.sprite)?w:null;}
        bool AirshipWorldTopV40(SpriteRenderer r,out float top){
            top=0;if(!r||!airshipSoldierWitnessV40.TryGetValue(r,out var w)||!ReferenceEquals(r.sprite,w.selected.sprite))return false;
            top=r.transform.TransformPoint(new Vector3(0,w.selected.top,0)).y;return true;
        }
        float AirshipLocalTopV40(Enemy e,SpriteRenderer r,float fallback){var w=AirshipSoldierFrameForV40(r);return w==null?fallback:
            r.transform.localPosition.y+w.selected.top*Mathf.Abs(r.transform.localScale.y)-e.y;}
        void DisposeAirshipSoldiersV40(){
            foreach(var w in airshipSoldierWitnessV40.Values){
                var r=w.renderer;if(!r||!ReferenceEquals(r.sprite,w.selected.sprite))continue;
                r.sprite=w.carrier;r.flipX=w.originalFlip;r.transform.localPosition=w.originalPosition;r.transform.localScale=w.originalScale;
            }
            foreach(var bank in airshipSoldierBanksV40.Values)bank.Dispose();
            airshipSoldierBanksV40.Clear();airshipSoldierWitnessV40.Clear();airshipSoldierBlockedV40.Clear();
        }
    }
}
