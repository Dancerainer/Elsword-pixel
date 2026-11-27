using System;using System.Collections.Generic;using UnityEngine;
namespace ElswordPixel { public sealed partial class DemoScene {
 static readonly string[] FactoryProductNPCsR1179={"NUI_BOX_ALTERA","NUI_FACTORY_PRESS","NUI_FACTORY_PRESS3","NUI_NASOD_GUARD","NUI_NASOD_HEALER_BLADER","NUI_NASOD_MAKER","NUI_NASOD_SHERIFF","NUI_RAVEN_CLONE","NUI_SMALLGATE_BESMA_AIR_SHIP","NUI_SMALLGATE_POST_BESMA_AIR_SHIP","NUI_WALLY_9TH_TYPE2"};
 static readonly string[] FactoryProductKindsR1179={"box-altera","factory-press","factory-press3","nasod-guard","nasod-healer-blader","nasod-maker","nasod-sheriff","raven-clone","smallgate-besma-air-ship","smallgate-post-besma-air-ship","wally-9th-type2"};
 WallyFiniteWholeLookupR198 factoryProductWallyR1179;SourcePhaseWholeLookupV21 factoryProductHealerR1179;
 // Nine source-phase lookups: seven original diagnostics plus finite Maker/Guard pixel candidates.
 int factoryProductOriginalReferenceBanksR1223,factoryProductMakerPixelBanksR1223,factoryProductGuardPixelBanksR1264;
 readonly Dictionary<string,SourcePhaseWholeLookupV21> factoryProductReferenceR1179=new Dictionary<string,SourcePhaseWholeLookupV21>(StringComparer.Ordinal);
 readonly DeferredWholeSpritesV53 factoryProductSpritesR1179=new DeferredWholeSpritesV53();
 int factoryProductMissingStateR1179,factoryProductOutsideR1179,factoryProductMissingReferenceR1179;string factoryProductLastNpcR1179="",factoryProductLastStateR1179="",factoryProductLastKeyR1179="";float factoryProductLastTimeR1179,factoryProductLastSourceTimeR1179;
 static bool FactoryProductWholeNpcR1179(string npc){return npc=="NUI_WALLY_9TH_TYPE2"||npc=="NUI_NASOD_HEALER_BLADER";}
 static string FactoryProductKeyR1179(string npc,string key){return npc+"/"+key;}
 static int FactoryProductReferenceIndexR1179(string npc){int i=Array.IndexOf(FactoryProductNPCsR1179,npc);return i>=0&&!FactoryProductWholeNpcR1179(npc)?i:-1;}
 static string FactoryProductFloatBitsR1179(float v){return BitConverter.ToInt32(BitConverter.GetBytes(v),0).ToString("X8",System.Globalization.CultureInfo.InvariantCulture);}
 void ConfigureFactoryProductBanksR1179(){
  FactoryProductNeedR1179(factoryProductWallyR1179==null&&factoryProductHealerR1179==null&&factoryProductSpritesR1179.Count==0&&factoryProductReferenceR1179.Count==0&&factoryProductOriginalReferenceBanksR1223==0&&factoryProductMakerPixelBanksR1223==0&&factoryProductGuardPixelBanksR1264==0,"fresh normal bank owner required");var map=factoryProductIndexR1179.Map;int keys=0,states=0;
  foreach(var bank in map.banks){
   var raw=factoryProductIndexR1179.Read(bank.document);int at=Array.IndexOf(FactoryProductNPCsR1179,bank.npc);FactoryProductNeedR1179(at>=0&&bank.kind==FactoryProductKindsR1179[at],"exact original bank/NPC/kind identity");
   if(bank.npc=="NUI_WALLY_9TH_TYPE2"){var d=FiniteWholeJsonR268.Read<WallyWholeDocumentR198>(raw);factoryProductWallyR1179=new WallyFiniteWholeLookupR198(d);FactoryProductNeedR1179(!bank.originalReference&&bank.document.sha256=="993de8969191996247a1be596100a34de11387dd0b8305f277e60baf36d505d9","original Wally document changed");foreach(var k in factoryProductWallyR1179.Keys()){var b=FactoryProductBoundKeyR1179(bank,k.Key,k.Path,k.Sha256,k.RgbaSha256,k.Width,k.Height);LoadFactoryProductKeyR1179(bank.npc,b);keys++;}}
   else{
    var d=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(raw);var lookup=bank.originalReference?SourcePhaseWholeLookupV21.CreateOriginalReferenceDiagnosticR718(d):new SourcePhaseWholeLookupV21(d);
    FactoryProductNeedR1179(lookup.SourceNpc==bank.npc&&lookup.Id==bank.id&&lookup.StateCount==bank.stateCount&&lookup.KeyCount==bank.keys.Length,"source-phase lookup identity/counts changed");
    if(bank.npc=="NUI_RAVEN_CLONE")PrepareFactoryRavenEnvelopeV21(d,lookup);
    if(bank.originalReference){FactoryProductNeedR1179(bank.npc!="NUI_NASOD_MAKER"&&bank.npc!="NUI_NASOD_GUARD"&&lookup.OriginalSourceReferenceDiagnosticR718&&lookup.SourceRulesSha256=="087c43182d0eb4afc82eff88fcd41162e42c58aede057bbf6b635f54d80ea159"&&lookup.MotionCount==bank.motionCount&&d.sourcePoseSamples==bank.sourcePoseSamples,"exact seven-original-reference source clock scope");factoryProductReferenceR1179.Add(bank.npc,lookup);factoryProductOriginalReferenceBanksR1223++;}
    else if(bank.npc=="NUI_NASOD_MAKER"){FactoryProductNeedR1179(!bank.originalReference&&!lookup.OriginalSourceReferenceDiagnosticR718&&bank.document.sha256==FactoryProductResourceIndexR1168.MakerPixelDocumentSha256R1223&&lookup.Id==FactoryProductResourceIndexR1168.MakerPixelIdR1223&&lookup.SourceNpc=="NUI_NASOD_MAKER"&&lookup.KeyCount==5&&lookup.MotionCount==2&&lookup.StateCount==6&&d.sourcePoseSamples==8&&lookup.FixedSourceFacing=="reference-model-yaw0-fixed-no-spawn-mirror"&&!lookup.AutomaticSpawnFacingMirror&&lookup.SourceRulesSha256=="087c43182d0eb4afc82eff88fcd41162e42c58aede057bbf6b635f54d80ea159","exact statically reviewed finite Maker pixel candidate");factoryProductReferenceR1179.Add(bank.npc,lookup);factoryProductMakerPixelBanksR1223++;}
    else if(bank.npc=="NUI_NASOD_GUARD"){FactoryProductNeedR1179(!bank.originalReference&&!lookup.OriginalSourceReferenceDiagnosticR718&&bank.document.sha256==FactoryProductResourceIndexR1168.GuardPixelDocumentSha256R1264&&lookup.Id==FactoryProductResourceIndexR1168.GuardPixelIdR1264&&lookup.SourceNpc=="NUI_NASOD_GUARD"&&lookup.KeyCount==40&&lookup.MotionCount==20&&lookup.StateCount==27&&d.sourcePoseSamples==77&&lookup.FixedSourceFacing=="reference-model-yaw0-fixed-no-spawn-mirror"&&!lookup.AutomaticSpawnFacingMirror&&lookup.SourceRulesSha256=="087c43182d0eb4afc82eff88fcd41162e42c58aede057bbf6b635f54d80ea159","exact reviewed finite Guard pixel candidate");factoryProductReferenceR1179.Add(bank.npc,lookup);factoryProductGuardPixelBanksR1264++;}
    else{FactoryProductNeedR1179(bank.npc=="NUI_NASOD_HEALER_BLADER"&&bank.document.sha256=="4ca179765a408bcc0cc3fec32fa682711b9da4e62b4c26ec2e7bf301ddc9f252"&&lookup.FixedSourceFacing=="+X"&&!lookup.AutomaticSpawnFacingMirror&&lookup.KeyCount==52&&lookup.MotionCount==14&&lookup.StateCount==26&&d.sourcePoseSamples==85&&lookup.SourceRulesSha256=="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72","exact original Healer finite family");factoryProductHealerR1179=lookup;}
    foreach(var k in lookup.Keys()){var b=FactoryProductBoundKeyR1179(bank,k.Key,k.PngPath,k.PngSha256,k.RgbaSha256,k.Width,k.Height);LoadFactoryProductKeyR1179(bank.npc,b);keys++;}
   }
   states+=bank.stateCount;
  }
  FactoryProductNeedR1179(factoryProductWallyR1179!=null&&factoryProductHealerR1179!=null&&factoryProductReferenceR1179.Count==9&&factoryProductOriginalReferenceBanksR1223==7&&factoryProductMakerPixelBanksR1223==1&&factoryProductGuardPixelBanksR1264==1&&keys==418&&states==185&&factoryProductSpritesR1179.Count==418,"eleven banks: existing Wally/Healer, seven original references and finite Maker/Guard pixel candidates");ValidateFactoryProductVisualStatesR1179(map,factoryProductWallyR1179,factoryProductHealerR1179);
 }
 FactoryProductKeyR1168 FactoryProductBoundKeyR1179(FactoryProductBankR1168 bank,string key,string path,string sha,string rgba,int width,int height){var b=Array.Find(bank.keys,k=>k.key==key);FactoryProductNeedR1179(b!=null&&FactoryProductResourceIndexR1168.Identity(b.originalPath)==FactoryProductResourceIndexR1168.Identity(path)&&b.file.sha256==sha&&b.rgbaSha256==rgba&&b.width==width&&b.height==height,"original finite key/anchor identity changed");return b;}
 void LoadFactoryProductKeyR1179(string npc,FactoryProductKeyR1168 key){string id=FactoryProductKeyR1179(npc,key.key);string kind=FactoryProductKindsR1179[Array.IndexOf(FactoryProductNPCsR1179,npc)];factoryProductSpritesR1179.Register(id,DeferMonsterPixelCarrierV53(kind),()=>{var t=LoadFactoryProductTextureR1179(key.file,key.width,key.height,"Factory whole / "+id);var sprite=PaintedPressV41(npc,key)??Sprite.Create(t,new Rect(0,0,key.width,key.height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);FactoryProductNeedR1179(sprite,"original FullRect Sprite creation failed");factoryProductOwnedR1179.Add(sprite);return sprite;});}
 void ReleaseFactoryProductBanksR1179(){DisposeFactoryRavenViewsV35();DisposeHealerBladerViewsV35();factoryProductSpritesR1179.Clear();factoryProductWallyR1179=null;factoryProductHealerR1179=null;factoryProductReferenceR1179.Clear();factoryProductOriginalReferenceBanksR1223=factoryProductMakerPixelBanksR1223=factoryProductGuardPixelBanksR1264=0;factoryProductMissingStateR1179=factoryProductOutsideR1179=factoryProductMissingReferenceR1179=0;factoryProductLastNpcR1179=factoryProductLastStateR1179=factoryProductLastKeyR1179="";factoryProductLastTimeR1179=factoryProductLastSourceTimeR1179=0;}
  void ValidateFactoryProductVisualStatesR1179(FactoryProductMapR1168 b,WallyFiniteWholeLookupR198 wally,SourcePhaseWholeLookupV21 healer){
   var seen=new HashSet<string>(StringComparer.Ordinal);int w=0,h=0;
   foreach(var row in b.twoBankVisualStates){FactoryProductNeedR1179(row!=null&&FactoryProductWholeNpcR1179(row.npc)&&seen.Add(FactoryProductKeyR1179(row.npc,row.state)),"Factory exact visual state identity");
    if(row.npc=="NUI_WALLY_9TH_TYPE2"){var s=wally.State(row.state);FactoryProductNeedR1179(s.Clip==row.clip&&s.Motion==row.motion&&FactoryProductFloatBitsR1179(s.Duration)==row.durationBits&&FactoryProductFloatBitsR1179(s.MotionDuration)==row.motionDurationBits&&FactoryProductFloatBitsR1179(s.Speed)==row.speedBits&&s.Loop==row.loop,"Factory Wally visual fields differ");w++;}
    else{var s=healer.State(row.state);FactoryProductNeedR1179(s.ModelClip==row.clip&&s.Motion==row.motion&&FactoryProductFloatBitsR1179(s.StateDuration)==row.durationBits&&FactoryProductFloatBitsR1179(s.MotionDuration)==row.motionDurationBits&&FactoryProductFloatBitsR1179(s.Speed)==row.speedBits&&s.Loop==row.loop&&!s.HasShow,"Factory Healer visual fields/no authored SHOW differ");h++;}
   }FactoryProductNeedR1179(w==27&&h==26,"Factory two exact visual-state sets");
   // NextState/holdForSourceEvent/sourceEventTransitions are deliberately never
   // imported from the Core document. Factory DAMAGE_AIR_FALL keeps its own event rule.
  }
  bool TryRenderFactoryProductWholeR1179(Enemy e,SpriteRenderer renderer){
   try{
    if(TryRenderFactoryProductProtoR1466(e,renderer))return true;if(TryRenderFactoryProductReferenceR1179(e,renderer))return true;if(!FactoryProductSessionActiveR1179||factoryProductWallyR1179==null||!FactoryProductWholeNpcR1179(e.sourceNPC))return false;
    FactoryProductNeedR1179(e.visualScale==1&&((e.sourceNPC=="NUI_WALLY_9TH_TYPE2"&&e.kind=="wally-9th-type2")||(e.sourceNPC=="NUI_NASOD_HEALER_BLADER"&&e.kind=="nasod-healer-blader")),"Original Factory whole actor identity/scale");
    string displayState;float displayTime;if(!TryFactoryReactionDisplayR1467(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;RecordMonsterPixelSourceHiddenV53(e,renderer,"Factory/Whole-original-hidden-init");factoryProductMissingStateR1179++;return true;}
    SourceWholeSelectionV21 healerSelection=null;
    string key;int width,height;double x,y;float diagnosticClock=0;string diagnosticMotion="",diagnosticPose="";bool diagnosticShow=true,diagnosticOutside=false;
    if(e.sourceNPC=="NUI_WALLY_9TH_TYPE2"){var selected=factoryProductWallyR1179.Select(displayState,displayTime);key=selected.Whole.Key;width=selected.Whole.Width;height=selected.Whole.Height;x=selected.Whole.RootX;y=selected.Whole.RootY;if(selected.OutsideFiniteRange)factoryProductOutsideR1179++;diagnosticClock=selected.SourceMotionClock;diagnosticMotion=selected.State.Motion;diagnosticPose=selected.SourcePose;diagnosticOutside=selected.OutsideFiniteRange;}
    else{FactoryProductNeedR1179(factoryProductHealerR1179!=null,"Factory Healer whole owner missing");var selected=factoryProductHealerR1179.Select(displayState,displayTime);healerSelection=selected;key=selected.Whole.Key;width=selected.Whole.Width;height=selected.Whole.Height;x=selected.AnchorX;y=selected.AnchorY;diagnosticClock=selected.SourceMotionClock;diagnosticMotion=selected.State.Motion;diagnosticPose=selected.SourcePose;diagnosticShow=!selected.State.HasShow||selected.State.Show;}
    renderer.enabled=true;renderer.sprite=factoryProductSpritesR1179[FactoryProductKeyR1179(e.sourceNPC,key)];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
    // Same centered FullRect + signed anchor placement as the real Core renderers.
    // No clamping, rounding, SpriteOffset, scale rebake, spawn-facing mirror or rerender.
    var offset=new Vector2((float)(width*.5-x),(float)(y-height*.5));var p=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(p.x,p.y,0);
    RecordMonsterPixelSourceV53(e,renderer,"Factory/Whole",displayState,diagnosticMotion,diagnosticClock,e.sourceNPC=="NUI_WALLY_9TH_TYPE2"?(e.facing<0?-1:1):1);
    if(e.sourceNPC=="NUI_WALLY_9TH_TYPE2")FitCoreEliteWholeV35(e,renderer,diagnosticMotion,diagnosticClock);
    if(healerSelection!=null)RecordHealerBladerSelectionV35(e,renderer,healerSelection);
    factoryProductLastNpcR1179=e.sourceNPC;factoryProductLastStateR1179=displayState;factoryProductLastKeyR1179=key;factoryProductLastTimeR1179=displayTime;return true;
   }catch(Exception trigger){CloseFactoryProductAfterFailureR1179(trigger);throw;}
  }
  bool TryRenderFactoryProductReferenceR1179(Enemy e,SpriteRenderer renderer){
   if(!FactoryProductSessionActiveR1179)return false;SourcePhaseWholeLookupV21 lookup;if(!factoryProductReferenceR1179.TryGetValue(e.sourceNPC,out lookup))return false;
   int at=FactoryProductReferenceIndexR1179(e.sourceNPC);FactoryProductNeedR1179(at>=0&&game.enemies.Contains(e)&&e.kind==FactoryProductKindsR1179[at]&&e.visualScale==1,"Factory source-phase actor identity/scale");
   string displayState;float displayTime;if(!TryFactoryReactionDisplayR1467(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;RecordMonsterPixelSourceHiddenV53(e,renderer,"Factory/Reference-original-hidden-init");factoryProductMissingReferenceR1179++;return true;}
   SourceWholeSelectionV21 selected;
   if(e.sourceNPC=="NUI_FACTORY_PRESS"||e.sourceNPC=="NUI_FACTORY_PRESS3"){
    FactoryPressFrameV569 f;FactoryProductNeedR1179(factoryProductPressFramesR1179.TryGetValue(e,out f)&&f!=null&&ReferenceEquals(f.actualOwner,e)&&f.npc==e.sourceNPC&&f.state==e.npcState,"Same actual Press owner/state frame required; no reconstructed clock");selected=lookup.SelectOriginalReferenceSourceClockR718(f.state,e.npcTime,f.sourcePoseTime);
   }else selected=lookup.Select(displayState,displayTime);
   RecordMonsterPixelSourceV53(e,renderer,"Factory/Reference",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1);
   var key=selected.Whole;renderer.enabled=true;renderer.sprite=factoryProductSpritesR1179[FactoryProductKeyR1179(e.sourceNPC,key.Key)];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
   // Keep model-owned GameObject visibility. Only the signed source anchor
   // offsets the already baked whole; no model SHOW/FSM/clock/facing is written.
   var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var p=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(p.x,p.y,0);
   factoryProductLastNpcR1179=e.sourceNPC;factoryProductLastStateR1179=displayState;factoryProductLastKeyR1179=key.Key;factoryProductLastTimeR1179=displayTime;factoryProductLastSourceTimeR1179=selected.SourceMotionClock;if(!MonsterPixelOwnsActorV53(e))RecordFactoryRavenIntroSelectionV21(e,renderer);RecordFactoryRavenSelectionV35(e,renderer,selected);RecordSheriffAuthoredSelectionV32(e,renderer,selected);RecordGuardSelectionV35(e,renderer,selected);return true;
  }
}}
