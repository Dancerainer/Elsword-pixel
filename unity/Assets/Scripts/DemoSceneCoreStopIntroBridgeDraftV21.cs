using System;using System.Collections.Generic;using UnityEngine;
namespace ElswordPixel {public sealed partial class DemoScene {
 CoreStopIntroViewBridgeDraftV21 coreStopIntroBridgeDraftV21;
 readonly Dictionary<string,CoreStopWholeEnvelopeV22> coreFourWholeStartEnvelopesV22=new Dictionary<string,CoreStopWholeEnvelopeV22>(StringComparer.Ordinal);
 readonly Dictionary<Enemy,CoreStopSourceActorFrameV21> coreFourWholeFramesV21=new Dictionary<Enemy,CoreStopSourceActorFrameV21>();
 void RecordCoreFourWholeFrameV21(Enemy e,SpriteRenderer sprite){
  // This records the actual ordinary renderer of the same source Enemy. It does
  // not fabricate an actor, source START, acknowledgement, camera or clock.
  if(e.sourceNPC!="NUI_RAVEN_CLONE_B"&&e.sourceNPC!="NUI_NASOD_ELEMENT_FIRE_B")return;
  if(string.IsNullOrEmpty(e.npcState)||!sprite||!sprite.sprite){coreFourWholeFramesV21.Remove(e);return;}
  var lookup=e.sourceNPC=="NUI_RAVEN_CLONE_B"?coreRavenBWholeLookupV21:coreElementFireBWholeLookupV21;var selected=lookup.Select(e.npcState,e.npcTime);
  var sprites=e.sourceNPC=="NUI_RAVEN_CLONE_B"?coreRavenBWholeSpritesV21:coreElementFireBWholeSpritesV21;PreviewRequire(MatchesCoreWholeInkV45(e.kind,lookup.State(e.npcState).Motion,(float)selected.SourceMotionClock,selected.Whole.Key,sprites[selected.Whole.Key],sprite.sprite,out var projected),"Original STOP whole-frame renderer must be exact approved selection");
  CoreStopWholeEnvelopeV22 envelope=null;
  if(e.npcState=="RAVEN_CLONE_B_START"||e.npcState=="NASOD_ELEMENT_FIRE_SOLO_B_START"){
   if(!coreFourWholeStartEnvelopesV22.TryGetValue(e.sourceNPC,out envelope)){var document=e.sourceNPC=="NUI_RAVEN_CLONE_B"?coreEntryScopeV21.ravenBWhole:coreEntryScopeV21.elementFireBWhole;envelope=CoreStopWholeFramingV22.Envelope(CoreEntryJsonV21<SourceWholeDocumentV21>(document),e.npcState);PreviewRequire(envelope.npc==e.sourceNPC,"Exact approved source START envelope identity");coreFourWholeStartEnvelopesV22.Add(e.sourceNPC,envelope);}
  }
  coreFourWholeFramesV21[e]=new CoreStopSourceActorFrameV21{actor=e,root=sprite.gameObject,renderers=new Renderer[]{sprite},state=e.npcState,pose=selected.SourcePose,renderedFrame=Time.frameCount,wholeStartEnvelopeV22=envelope,logicalActorRootV22=RenderEnemyPosition(e),wholeKeyV22=projected==null?selected.Whole.Key:projected.name,signedAnchorXV22=projected==null?selected.AnchorX:projected.anchorX,signedAnchorYV22=projected==null?selected.AnchorY:projected.anchorY};
 }
 void RecordCoreIceWholeStopFrameR390(Enemy e,SpriteRenderer sprite){
  // Read only the same ordinary whole renderer selected immediately above.
  // The existing bridge still owns camera, actual draw and ordinary-Step ack.
  PreviewRequire(e!=null&&e.sourceNPC=="NUI_NASOD_ELEMENT_ICE_B","IceB STOP frame requires the same original source actor");
  if(string.IsNullOrEmpty(e.npcState)||!sprite||!sprite.sprite){coreFourWholeFramesV21.Remove(e);return;}
  PreviewRequire(coreElementIceBWholeLookupV21!=null,"IceB STOP frame requires the configured exact whole lookup");
  var selected=coreElementIceBWholeLookupV21.Select(e.npcState,e.npcTime);Sprite expected;
  PreviewRequire(selected.Whole!=null&&coreElementIceBWholeSpritesV21.TryGetValue(selected.Whole.Key,out expected)&&sprite.sprite==expected,"IceB STOP whole-frame renderer must be the exact current approved selection");
  CoreStopWholeEnvelopeV22 envelope=null;
  if(e.npcState=="NASOD_ELEMENT_ICE_SOLO_B_START"){
   if(!coreFourWholeStartEnvelopesV22.TryGetValue(e.sourceNPC,out envelope)){
    var document=coreEntryScopeV21.elementIceBWhole;PreviewRequire(document!=null&&document.sha256==CoreElementIceBWholeDocumentShaV21,"Exact IceB complete START document pin");
    envelope=CoreStopWholeFramingV22.Envelope(FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(CoreEntryReadV21(document)),e.npcState);
    PreviewRequire(envelope.npc==e.sourceNPC&&envelope.state==e.npcState,"Exact IceB complete START envelope identity");coreFourWholeStartEnvelopesV22.Add(e.sourceNPC,envelope);
   }
  }
  coreFourWholeFramesV21[e]=new CoreStopSourceActorFrameV21{actor=e,root=sprite.gameObject,renderers=new Renderer[]{sprite},state=e.npcState,pose=selected.SourcePose,renderedFrame=Time.frameCount,wholeStartEnvelopeV22=envelope,logicalActorRootV22=RenderEnemyPosition(e),wholeKeyV22=selected.Whole.Key,signedAnchorXV22=selected.AnchorX,signedAnchorYV22=selected.AnchorY};
 }
 // Proposed explicit loader hook after SetRooms and before the first ordinary
 // Step. Current Host3/4 never calls this draft method and remains guard-only.
 void PrepareCoreStopIntroBridgeDraftV21(CoreStopOwnerDocumentV21 ownerDocument){if(!coreEntrySessionV21||!forestPreviewActive||coreEntrySourceViewV21==null)throw new InvalidOperationException("Core controlled source entry/view must own the bridge");DisposeCoreStopIntroBridgeDraftV21();game.SetCoreStopIntroOwnerV21(ownerDocument);coreStopIntroBridgeDraftV21=new CoreStopIntroViewBridgeDraftV21(game,ReadCoreStopSourceActorFrameDraftV21,ReadCoreStopCameraContextDraftV21);ConnectCoreIntroProbeScopeV25();}
 CoreStopSourceActorFrameV21 ReadCoreStopSourceActorFrameDraftV21(CoreStopIntroTicketV21 t){if(!coreEntrySessionV21||!forestPreviewActive||coreEntrySourceViewV21==null||game.CurrentRoom.id!="Dungeon_Altera_Core_Hard-"+t.Room||game.EncounterIndex!=t.Encounter)return null;CoreStopSourceActorFrameV21 whole;if(t.Npc=="NUI_RAVEN_CLONE_B"||t.Npc=="NUI_NASOD_ELEMENT_FIRE_B"||t.Npc=="NUI_NASOD_ELEMENT_ICE_B")return coreFourWholeFramesV21.TryGetValue(t.Actor,out whole)?whole:null;return coreEntrySourceViewV21.ReadStopActorFrameDraftV21(t.Actor);}
 CoreStopCameraContextV21 ReadCoreStopCameraContextDraftV21(){bool exactContext=coreEntrySessionV21&&forestPreviewActive&&NativeWorldActive&&worldCamera&&nativeCamera&&nativeScreen&&nativeTarget&&coreEntryScopeV21!=null&&game.room>=0&&game.room<coreEntryScopeV21.worlds.Length&&nativeScene.id==coreEntryScopeV21.worlds[game.room].id;float follow=cameraFollowY,velocity=cameraFollowVelocity;int followRoom=cameraFollowRoom;var context=new CoreStopCameraContextV21{coreSessionV26=coreEntrySessionV21,previewActiveV26=forestPreviewActive,nativeWorldActiveV26=NativeWorldActive,actualWorldV26=nativeScene==null?null:nativeScene.id,expectedWorldV26=coreEntryScopeV21!=null&&game.room>=0&&game.room<coreEntryScopeV21.worlds.Length?coreEntryScopeV21.worlds[game.room].id:null,roomV26=game.room,encounterV26=game.EncounterIndex,world=worldCamera,native=nativeCamera,screen=nativeScreen?nativeScreen.transform:null,target=nativeTarget,sourceUnit=nativeScene==null?0:nativeScene.unit,updateNativeCamera=()=>UpdateNativeCamera(),restoreFollow=()=>{cameraFollowY=follow;cameraFollowVelocity=velocity;cameraFollowRoom=followRoom;},readFollowState=()=>new CoreIntroFollowStateV25{y=cameraFollowY,velocity=cameraFollowVelocity,room=cameraFollowRoom},safeViewport=CoreStopMeasuredSafeViewportDraftV21};if(!exactContext)throw new CoreStopCameraReadExceptionV26("Exact current Core source native world/camera missing",context);return context;}
 Rect CoreStopMeasuredSafeViewportDraftV21(){var pixel=worldCamera.pixelRect;if(pixel.height<=0||pixel.width<=0)throw new InvalidOperationException("Core focus viewport absent");float top=pixel.yMin;var corners=new Vector3[4];Camera ui=hudCanvas&&hudCanvas.renderMode!=RenderMode.ScreenSpaceOverlay?hudCanvas.worldCamera:null;foreach(string id in BelowLowerHudImagesV21){if(!images.TryGetValue(id,out var image)||!image||!image.isActiveAndEnabled||image.color.a<=0)continue;image.rectTransform.GetWorldCorners(corners);for(int i=0;i<4;i++)top=Math.Max(top,RectTransformUtility.WorldToScreenPoint(ui,corners[i]).y);}for(int index=0;index<8;index++){if(!labels.TryGetValue("Skill Key "+index,out var label)||!label||!label.isActiveAndEnabled||label.color.a<=0)continue;label.rectTransform.GetWorldCorners(corners);for(int i=0;i<4;i++)top=Math.Max(top,RectTransformUtility.WorldToScreenPoint(ui,corners[i]).y);}float bottom=Math.Max(BelowCameraViewportV21.BottomHudMargin/270f,(top-pixel.yMin)/pixel.height+BelowCameraViewportV21.HudGeometryBuffer/270f);float upper=BelowCameraViewportV21.TopHudMargin/270f;return new Rect(24f/480,bottom,1-48f/480,1-bottom-upper);}
 void PrepareCoreStopWholeCurrentFrameV22(){if(coreStopIntroBridgeDraftV21!=null)coreStopIntroBridgeDraftV21.PrepareWholeCurrentFrameBeforeProjectionV22();}
 void AdjustCoreStopWholeProjectionV22(ref float width,ref float height){if(coreStopIntroBridgeDraftV21!=null)coreStopIntroBridgeDraftV21.AdjustWholeProjectionV22(ref width,ref height);}
 bool TryRenderCoreStopIntroCameraDraftV21(){return coreStopIntroBridgeDraftV21!=null&&coreStopIntroBridgeDraftV21.RenderTryApplyPairedFocusV21();}
 void ObserveCoreStopAfterNativeProjectionDraftV21(){if(coreStopIntroBridgeDraftV21!=null)coreStopIntroBridgeDraftV21.RenderObserveAfterProjectionV21();}
 void BeforeCoreStopOrdinaryStepDraftV21(){if(coreStopIntroBridgeDraftV21!=null&&coreIntroProbeAllowSubmissionV25)coreStopIntroBridgeDraftV21.BeforeOrdinaryStepV21();}
 void ReleaseCoreStopCameraForWorldChangeDraftV21(){if(coreStopIntroBridgeDraftV21!=null)coreStopIntroBridgeDraftV21.ReleaseCameraForWorldChangeV21();}
 void DisposeCoreStopIntroBridgeDraftV21(){try{if(coreStopIntroBridgeDraftV21!=null)coreStopIntroBridgeDraftV21.Dispose();}finally{RetireCoreIntroProbeScopeV25();coreStopIntroBridgeDraftV21=null;game.SetCoreStopIntroOwnerV21(null);}}
}}

