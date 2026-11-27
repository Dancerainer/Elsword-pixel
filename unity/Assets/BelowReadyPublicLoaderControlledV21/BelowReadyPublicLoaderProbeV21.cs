#if UNITY_EDITOR
using System;using System.IO;using UnityEngine;
namespace ElswordPixel {public sealed partial class DemoScene {
 [Serializable] public sealed class BelowReadyPublicLoaderResultV21 {
  public string schema="below-ready-actual-public-loader-and-exit-controlled-report-v21";
  public bool passed,actualEntryExecuted,actualPlayMode,naturalRoute=false,runtimeAdmitted=false,inputOverrides=false,sourceRoomInitializationOverride=false;
  public bool publicMode,sourceRoom0,originalTargetPoint,projectDarkApplied,effectsConfigured,externalOriginalMusicLoaded,previewExit,backgroundScopeDisposed,fortScopeDisposed,previewCameraClearRestoredOrDestroyed,oldNativeCameraDestroyed,selectionRestored,loadoutRestored,outsideNativeDefaultColor,baseViewportRestored;
  public int originalSelectedDungeon,restoredSelectedDungeon,backgroundRestoreCountBefore,backgroundRestoreCountAfter;public float modelElapsedAtCapture;public string nativeWorld;
 }
 BelowReadyPublicLoaderResultV21 belowReadyPublicLoaderResultV21;string belowReadyPublicProbeOutputV21;string[] belowReadyPublicOriginalLoadoutV21;Camera belowReadyPublicOldNativeCameraV21;
 public void BeginBelowReadyPublicLoaderProbeV21(string bundle,string output){
  PreviewRequire(Application.isBatchMode&&Application.isPlaying&&!forestPreviewActive&&!forestPreviewLoading,"Idle batch PlayMode public-loader probe required");belowReadyPublicProbeOutputV21=output;captureDir=output;forestAutoOutput=output;belowReadyPublicLoaderResultV21=new BelowReadyPublicLoaderResultV21{originalSelectedDungeon=selectedDungeon};belowReadyPublicOriginalLoadoutV21=new string[GameModel.SkillSlotCount];for(int i=0;i<belowReadyPublicOriginalLoadoutV21.Length;i++)belowReadyPublicOriginalLoadoutV21[i]=game.GetEquippedSkill(i).id;
  // Call the same public entry used by the interactive Editor launcher.
  // No direct EnterRoom, model mutation, invincibility, input or Step hooks.
  BeginBelowTunnelPreviewV21(bundle);
 }
 public float BelowReadyPublicLoaderProbeElapsedV21 {get{return game.elapsed;}}
 public void ObserveAndExitBelowReadyPublicLoaderProbeV21(){
  PreviewRequire(BelowTunnelEntryActiveV21&&!forestPreviewLoading&&game.elapsed>=1,"Actual public loader and ordinary simulation must complete before observing");var r=belowReadyPublicLoaderResultV21;r.actualPlayMode=Application.isPlaying;r.actualEntryExecuted=true;r.publicMode=!belowTunnelEntryControlledV21;r.sourceRoom0=game.room==0&&game.CurrentRoom.id=="Dungeon_Altera_Below_Tunnel_Normal-0";r.nativeWorld=nativeScene==null?"":nativeScene.id;r.sourceRoom0&=r.nativeWorld=="WI_ALTERA_BELOW_TUNNEL_STAGE1";r.originalTargetPoint=nativeTarget&&nativeTarget.width==480&&nativeTarget.height==270&&nativeTarget.filterMode==FilterMode.Point;r.projectDarkApplied=belowBackgroundCameraV21==nativeCamera&&nativeCamera.backgroundColor==worldCamera.backgroundColor;r.effectsConfigured=belowTunnelEntryEffectsConfiguredV21;r.externalOriginalMusicLoaded=belowEntryMusicClipsV21.Count>0;r.modelElapsedAtCapture=game.elapsed;
  RestoreNativeProjection();previousRoom=-1;accumulator=0;FitScreen();RenderActors();RenderEffects();RenderHud();RenderCombatGeometry();RenderCombatPresentation();FinishNativeProjection();Capture("public-loader-loaded");SaveBelowCameraViewportSnapshotV21("public-loader-loaded");
  belowReadyPublicOldNativeCameraV21=nativeCamera;var original=belowBackgroundOriginalV21;r.backgroundRestoreCountBefore=belowBackgroundRestoreCountV21;EndForestPreview();r.backgroundRestoreCountAfter=belowBackgroundRestoreCountV21;r.previewCameraClearRestoredOrDestroyed=!belowReadyPublicOldNativeCameraV21||belowReadyPublicOldNativeCameraV21.backgroundColor==original;r.previewExit=!forestPreviewActive&&!forestPreviewLoading&&!belowTunnelEntrySessionV21;r.backgroundScopeDisposed=belowBackgroundCameraV21==null&&r.backgroundRestoreCountAfter>r.backgroundRestoreCountBefore;r.fortScopeDisposed=belowFortCameraSavedV21==null;r.restoredSelectedDungeon=selectedDungeon;r.selectionRestored=selectedDungeon==r.originalSelectedDungeon;r.loadoutRestored=true;for(int i=0;i<belowReadyPublicOriginalLoadoutV21.Length;i++)r.loadoutRestored&=game.GetEquippedSkill(i).id==belowReadyPublicOriginalLoadoutV21[i];r.outsideNativeDefaultColor=!nativeCamera||nativeCamera.backgroundColor==new Color(.67f,.87f,.94f);r.baseViewportRestored=worldCamera.orthographicSize==135&&(!nativeScreen||nativeScreen.transform.localScale==new Vector3(480,270,1))&&(!nativeTarget||nativeTarget.width==480&&nativeTarget.height==270&&nativeTarget.filterMode==FilterMode.Point);
 }
 public BelowReadyPublicLoaderResultV21 FinishBelowReadyPublicLoaderProbeV21(){
  var r=belowReadyPublicLoaderResultV21;r.oldNativeCameraDestroyed=!belowReadyPublicOldNativeCameraV21;RestoreNativeProjection();previousRoom=-1;accumulator=0;FitScreen();RenderActors();RenderEffects();RenderHud();RenderCombatGeometry();RenderCombatPresentation();FinishNativeProjection();captureDir=belowReadyPublicProbeOutputV21;Capture("public-loader-exited");r.passed=r.actualEntryExecuted&&r.actualPlayMode&&r.publicMode&&r.sourceRoom0&&r.originalTargetPoint&&r.projectDarkApplied&&r.effectsConfigured&&r.externalOriginalMusicLoaded&&r.previewExit&&r.backgroundScopeDisposed&&r.fortScopeDisposed&&r.previewCameraClearRestoredOrDestroyed&&r.oldNativeCameraDestroyed&&r.selectionRestored&&r.loadoutRestored&&r.outsideNativeDefaultColor&&r.baseViewportRestored;return r;
 }
}}
#endif

