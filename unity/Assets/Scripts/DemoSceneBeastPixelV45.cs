using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  readonly Dictionary<string,AirshipSoldierBankV40> beastsV45=new Dictionary<string,AirshipSoldierBankV40>();
  readonly Dictionary<SpriteRenderer,AirshipSoldierFrameV40> beastViewsV45=new Dictionary<SpriteRenderer,AirshipSoldierFrameV40>();
  readonly HashSet<string> blockedBeastsV45=new HashSet<string>();
  void ApplyBeastPixelV45(Enemy e,SpriteRenderer r,string alias,float clock){
   if(MonsterPixelOwnsActorV53(e))return;
   if(!r||!r.sprite||e==null||!(e.kind=="armadillo"||e.kind=="fire-komodo"||e.kind=="armadillo-earth"||e.kind=="bat-b-fire")||
      game.paused&&openMenu=="dungeons"||blockedBeastsV45.Contains(e.kind)||!libraries.TryGetValue(e.kind,out var lib))return;
   var receipt=LegacyBodyRegistrationV39.Find(e,Dungeon.id,lib.data.artVersion);
   if(receipt==null)return;
   if(!lib.frames.ContainsKey(alias)){
    switch(alias){case "launched":case "fall":alias=lib.frames.ContainsKey("launch")?"launch":"air-up";break;
     case "knockdown":alias="down";break;case "getup":alias="get-up";break;default:alias="idle";break;}
   }
   if(!lib.clips.TryGetValue(alias,out var clip)||!lib.frames.TryGetValue(alias,out var frames))return;
   try{
    if(!beastsV45.TryGetValue(e.kind,out var bank)){bank=new AirshipSoldierBankV40(e.kind,e.sourceNPC,spriteOffsets,DestroyNativeWorldObject,true);beastsV45.Add(e.kind,bank);}
    if(Array.IndexOf(bank.Data.routes,Dungeon.id)<0||Mathf.Abs(receipt.target-bank.Data.standingHeight)>.01f)throw new InvalidOperationException("Unregistered route or size");
    if(!bank.Select(alias,clock,clip,frames,r.sprite,out var selected))return;
    if(!beastViewsV45.TryGetValue(r,out var w)){w=new AirshipSoldierFrameV40();beastViewsV45.Add(r,w);}
    w.actor=e;w.renderer=r;w.route=Dungeon.id;w.carrier=r.sprite;w.originalFlip=r.flipX;w.originalScale=r.transform.localScale;w.originalPosition=r.transform.localPosition;
    w.foot=RenderEnemyPosition(e);w.selected=selected;w.unityFrame=Time.frameCount;w.facing=e.facing;w.clock=clock;
    r.sprite=selected.sprite;r.flipX=e.facing!=bank.Data.authoredFacing;r.transform.localScale=Vector3.one;
    var offset=SpriteOffset(r);r.transform.localPosition=new Vector3(w.foot.x+offset.x,w.foot.y+offset.y+receipt.neutralFoot,w.originalPosition.z);
   }catch(Exception ex){blockedBeastsV45.Add(e.kind);Debug.LogWarning("BEAST_PIXEL_FALLBACK_V45 "+e.kind+": "+ex.Message);}
  }
  public AirshipSoldierFrameV40 BeastPixelFrameV45(SpriteRenderer r){return r&&beastViewsV45.TryGetValue(r,out var w)&&w.unityFrame==Time.frameCount&&ReferenceEquals(r.sprite,w.selected.sprite)?w:null;}
  float BeastLocalTopV45(Enemy e,SpriteRenderer r,float fallback){var w=BeastPixelFrameV45(r);return w==null?fallback:r.transform.localPosition.y+w.selected.top-e.y;}
  void DisposeBeastsV45(){
   foreach(var w in beastViewsV45.Values){var r=w.renderer;if(!r||!ReferenceEquals(r.sprite,w.selected.sprite))continue;
    r.sprite=w.carrier;r.flipX=w.originalFlip;r.transform.localScale=w.originalScale;r.transform.localPosition=w.originalPosition;}
   foreach(var bank in beastsV45.Values)bank.Dispose();beastsV45.Clear();beastViewsV45.Clear();blockedBeastsV45.Clear();
  }
 }
}
