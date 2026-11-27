#if UNITY_EDITOR
using System;using System.Collections;using System.Collections.Generic;using System.Reflection;using UnityEngine;
namespace ElswordPixel {public sealed partial class DemoScene {
 // One Editor-only read-only observation/fault scope for the private lifecycle probe.
 Action<string> coreLifecycleObserveV24;
 sealed class CoreLifecycleAudioLeaseRecordV24:IDisposable {internal UnityEngine.Networking.UnityWebRequest request;internal bool disposed;internal CoreLifecycleAudioLeaseRecordV24(UnityEngine.Networking.UnityWebRequest value){request=value;}public void Dispose(){if(disposed)return;try{request.Dispose();}finally{disposed=true;}}}
 readonly List<CoreLifecycleAudioLeaseRecordV24> coreLifecycleAudioLeasesV24=new List<CoreLifecycleAudioLeaseRecordV24>();
 CoreLifecycleAudioLeaseRecordV24 CoreLifecycleAudioLeaseV24(UnityEngine.Networking.UnityWebRequest request){var lease=new CoreLifecycleAudioLeaseRecordV24(request);coreLifecycleAudioLeasesV24.Add(lease);coreLifecycleObserveV24?.Invoke("audio-request-open");return lease;}
 public int CoreLifecycleOpenAudioRequestsV24 {get{int n=0;foreach(var lease in coreLifecycleAudioLeasesV24)if(!lease.disposed)n++;return n;}}
 public int CoreLifecycleTotalAudioRequestsV24 {get{return coreLifecycleAudioLeasesV24.Count;}}
 public sealed class CoreLifecycleSnapshotV24 {
  internal readonly Dictionary<string,Dictionary<object,object>> dictionaries=new Dictionary<string,Dictionary<object,object>>();
  internal Enemy[] actors;internal string actorState;internal object room;internal int selected,preview;internal string[] loadout;internal Rect viewport;internal Texture targetTexture;internal Texture heroTexture;
  public int libraries,sounds;public string stage;public bool pendingActorsIdentical,dictionariesIdentical,selectionLoadoutViewportIdentical;
 }
 const BindingFlags CoreLifecycleFlagsV24=BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public;
 static object CoreLifecycleFieldV24(object o,string name){var f=o.GetType().GetField(name,CoreLifecycleFlagsV24);if(f==null)throw new InvalidOperationException("Lifecycle scope missing exact field "+name);return f.GetValue(o);}
 static Dictionary<object,object> CoreLifecycleDictionaryV24(IDictionary d){var result=new Dictionary<object,object>();foreach(DictionaryEntry e in d)result.Add(e.Key,e.Value);return result;}
 static bool CoreLifecycleSameDictionaryV24(IDictionary actual,Dictionary<object,object> expected){if(actual.Count!=expected.Count)return false;foreach(var p in expected){if(!actual.Contains(p.Key))return false;var value=actual[p.Key];if(p.Value!=null&&p.Value.GetType().IsValueType){if(!Equals(value,p.Value))return false;}else if(!ReferenceEquals(value,p.Value))return false;}return true;}
 string CoreLifecycleActorFingerprintV24(){var s=new System.Text.StringBuilder();foreach(var e in game.enemies)s.Append(e.sourceNPC).Append('|').Append(e.hp.ToString("R")).Append('|').Append(e.x.ToString("R")).Append('|').Append(e.y.ToString("R")).Append('|').Append(e.npcState).Append('|').Append(e.npcTime.ToString("R")).Append('|').Append(e.aiEnabled).Append(';');return s.ToString();}
 public CoreLifecycleSnapshotV24 CoreLifecycleCaptureAdmittedV24(){
  var s=new CoreLifecycleSnapshotV24{actors=game.enemies.ToArray(),actorState=CoreLifecycleActorFingerprintV24(),room=game.CurrentRoom,selected=selectedDungeon,preview=previewDungeon,loadout=new string[GameModel.SkillSlotCount],viewport=worldCamera.rect,targetTexture=worldCamera.targetTexture,heroTexture=heroView.sprite?heroView.sprite.texture:null,libraries=libraries.Count,sounds=audioClips.Count,stage=CoreHardEntryOwnershipStageV21};
  foreach(string n in new[]{"libraries","audioClips","atlasTextures","npcEffectDepths"})s.dictionaries.Add("scene/"+n,CoreLifecycleDictionaryV24((IDictionary)CoreLifecycleFieldV24(this,n)));
  foreach(string n in new[]{"npcDefinitions","npcStates","npcEffectDefinitions","npcEffectBindings","npcStateSounds","npcEffectSounds","npcMechanismRules","npcRetaliations"})s.dictionaries.Add("model/"+n,CoreLifecycleDictionaryV24((IDictionary)CoreLifecycleFieldV24(game,n)));
  for(int i=0;i<s.loadout.Length;i++)s.loadout[i]=game.GetEquippedSkill(i).id;return s;
 }
 public bool CoreLifecycleSnapshotMatchesV24(CoreLifecycleSnapshotV24 s,bool pending,bool destroying){
  bool same=true;foreach(var p in s.dictionaries){if(destroying&&p.Key.StartsWith("model/",StringComparison.Ordinal))continue;var a=p.Key.Split('/');same&=CoreLifecycleSameDictionaryV24((IDictionary)CoreLifecycleFieldV24(a[0]=="scene"?(object)this:game,a[1]),p.Value);}s.dictionariesIdentical=same;
  bool fields=selectedDungeon==s.selected&&previewDungeon==s.preview;for(int i=0;i<s.loadout.Length;i++)fields&=game.GetEquippedSkill(i).id==s.loadout[i];
  if(!destroying)fields&=worldCamera.rect==s.viewport&&ReferenceEquals(worldCamera.targetTexture,s.targetTexture)&&ReferenceEquals(heroView.sprite?heroView.sprite.texture:null,s.heroTexture);s.selectionLoadoutViewportIdentical=fields;
  bool actors=true;if(pending){actors=ReferenceEquals(game.CurrentRoom,s.room)&&game.enemies.Count==s.actors.Length&&CoreLifecycleActorFingerprintV24()==s.actorState;for(int i=0;actors&&i<s.actors.Length;i++)actors&=ReferenceEquals(game.enemies[i],s.actors[i]);}s.pendingActorsIdentical=actors;
  return same&&fields&&actors;
 }
 static void CoreLifecycleAddOwnedV24(object value,List<UnityEngine.Object> found,int depth=0){
  if(ReferenceEquals(value,null)||depth>4)return;var u=value as UnityEngine.Object;if(!ReferenceEquals(u,null)){if(!found.Contains(u))found.Add(u);return;}
  var dict=value as IDictionary;if(dict!=null){foreach(DictionaryEntry e in dict)CoreLifecycleAddOwnedV24(e.Value,found,depth+1);return;}
  var list=value as IEnumerable;if(list!=null&&!(value is string)){foreach(var v in list)CoreLifecycleAddOwnedV24(v,found,depth+1);return;}
  // Selected resource records only; never traverse GameModel/source DTO graphs.
  foreach(var f in value.GetType().GetFields(CoreLifecycleFlagsV24)){if(f.Name=="data"||f.Name=="track"||f.Name=="view"||f.Name=="textures"||f.Name=="states")continue;var t=f.FieldType;if(typeof(UnityEngine.Object).IsAssignableFrom(t)||typeof(IEnumerable).IsAssignableFrom(t)||t.Name=="OutskirtsMeshView")CoreLifecycleAddOwnedV24(f.GetValue(value),found,depth+1);}
 }
 public UnityEngine.Object[] CoreLifecycleCaptureOwnedV24(){
  var result=new List<UnityEngine.Object>();CoreLifecycleAddOwnedV24(forestPreviewOwned,result);CoreLifecycleAddOwnedV24(coreKingWholeCacheV21,result);
  if(coreEntrySourceViewV21!=null)CoreLifecycleAddOwnedV24(CoreLifecycleFieldV24(coreEntrySourceViewV21,"actors"),result);
  CoreLifecycleAddOwnedV24(coreHardClosureVisualViewsV21,result);CoreLifecycleAddOwnedV24(coreHardClosureVisualMeshesV21,result);CoreLifecycleAddOwnedV24(coreHardClosureVisualMaterialsV21,result);CoreLifecycleAddOwnedV24(coreHardClosureVisualLoadedTexturesV21,result);CoreLifecycleAddOwnedV24(coreHardLocalAddVisualLoadedTexturesV21,result);CoreLifecycleAddOwnedV24(coreHardClosureVisualSpritesV21,result);
  if(coreEntrySessionV21&&forestPreviewActive){CoreLifecycleAddOwnedV24(nativeObjects,result);CoreLifecycleAddOwnedV24(nativeRoot,result);CoreLifecycleAddOwnedV24(nativeCamera,result);CoreLifecycleAddOwnedV24(nativeScreen,result);CoreLifecycleAddOwnedV24(nativeTarget,result);}return result.ToArray();
 }
 public static int CoreLifecycleAliveOwnedV24(UnityEngine.Object[] owned){int n=0;foreach(var o in owned)if(o)n++;return n;}
 public bool CoreLifecycleCoreReleasedV24 {get{return CoreHardEntryBindingsReleasedV21&&CoreIntroProbeBridgeReleasedV25&&!game.CoreHardConfiguredV21&&!game.CoreHardEntryActivationConfiguredV21&&!game.CoreHardLocalBalanceConfiguredV21&&!game.CoreHardClosureConfiguredV21&&!game.CoreHardLocalAddsConfiguredV21&&!game.CoreHardProtectionConfiguredV21&&!game.CoreHardBattlePlaneConfiguredV21&&!game.CoreHardCallbacksConfiguredV21&&!CoreKingWholeConfiguredV21&&CoreKingWholeResidentKeyCountV21==0&&coreExtractorStateRowsV21.Count==0;}}
 public string CoreLifecycleFaultPointV24 {get{return coreEntryFaultPointV21??"";}}
 public bool CoreLifecycleLoadingInAudioRequestV24 {get{return coreEntryOwnedLoadWorkV21!=null&&coreEntryOwnedLoadWorkV21.Current is UnityEngine.Networking.UnityWebRequestAsyncOperation;}}
 public void CoreLifecycleSetFaultV24(string point,Action<string> observe){
  string[] exact={"","inline-complete","after-snapshot","before-common-begin","after-load-owned","before-source-model","after-source-model","after-source-actors","during-audio","after-audio-register","before-fx","after-fx"};if(Array.IndexOf(exact,point)<0)throw new ArgumentException("Unknown exact lifecycle fault point");if(CoreLifecycleOpenAudioRequestsV24!=0)throw new InvalidOperationException("Cannot replace lifecycle scope with open audio request");coreLifecycleAudioLeasesV24.Clear();coreEntryFaultPointV21=point;coreLifecycleObserveV24=observe;
 }
 public void CoreLifecycleSetPolicyFlagV24(string name,bool value){if(name=="AutoTest")runAutomatedPlaythrough=value;else if(name=="Calibration")calibrationRun=value;else if(name=="RubenReview")rubenReview=value;else if(name=="FeedbackReview")feedbackReview=value;else throw new ArgumentException("Unknown lifecycle policy flag");}
 public bool CoreLifecycleNoPendingInputV24 {get{return pending!=null&&verticalPresses.Count==0&&!pending.attack&&!pending.attackX&&!pending.jump&&!pending.potion&&pending.potionPressedMask==0&&(!battleStick||battleStick.Horizontal==0)&&!pending.drop&&pending.skill<0&&!pending.dashPressed&&touches.Count==0;}}
 public bool CoreLifecycleOwnedSpritesDetachedV24 {get{return coreKingWholeCacheV21.Count==0&&coreAlarmWholeSpritesV21.Count==0&&coreExtractorWholeSpritesV21.Count==0&&coreEntrySourceViewV21==null&&coreEntryScopeV21==null&&coreEntryOwnedLoadWorkV21==null;}}
 public bool CoreLifecycleRoutineReleasedV24 {get{return forestPreviewRoutine==null&&coreEntryOwnedLoadWorkV21==null&&coreEntryScopeV21==null&&coreEntryBeginOwnerV21.Phase==CoreEntryBeginOwnershipV21.Stage.None;}}
 sealed class CoreLifecycleInlineCompleteV24:IEnumerator,IDisposable {IEnumerator inner;internal CoreLifecycleInlineCompleteV24(IEnumerator work){inner=work;}public object Current{get{return null;}}public bool MoveNext(){return false;}public void Reset(){throw new NotSupportedException();}public void Dispose(){var work=inner;inner=null;(work as IDisposable)?.Dispose();}}
 IEnumerator CoreLifecycleWrapOwnedWorkV24(IEnumerator work){if(coreEntryFaultPointV21!="inline-complete")return work;coreLifecycleObserveV24?.Invoke("inline-complete");return new CoreLifecycleInlineCompleteV24(work);}
 void CompleteCoreLifecycleOwnedGuardV24(){if(coreEntrySessionV21&&forestPreviewLoading&&!forestPreviewActive){forestPreviewError="Core controlled premature owned loader completion before Active";Debug.LogError(forestPreviewError);EndForestPreview();}}
 void StartCoreAwareForestPreviewRoutineV24(){bool wasCore=coreEntrySessionV21;var started=StartCoroutine(ForestPreviewLoadGuard());if(!wasCore||coreEntrySessionV21&&(forestPreviewLoading||forestPreviewActive))forestPreviewRoutine=started;else{forestPreviewRoutine=null;if(started!=null)StopCoroutine(started);}}
 [Serializable]public sealed class CoreLifecycleBodyRowV24 {public string npc,state,displayClip;public float originalStateClock,motionPlaybackClock,stateDuration,motionDuration;public bool sourceClockUnchanged,hasSourceShow,sourceShow,visibleTruePreserved,visibleFalsePreserved,fixedFrontal,componentFixture=true,actualSelectorCalled=true,actualGPUForEachState=false;}
 public CoreLifecycleBodyRowV24[] CoreLifecycleBodySelectorFixturesV24(){
  string original=CoreLifecycleActorFingerprintV24();var rows=new List<CoreLifecycleBodyRowV24>();var host=new GameObject("Core private lifecycle selector fixture");forestPreviewOwned.Add(host);var renderer=host.AddComponent<SpriteRenderer>();renderer.enabled=false;
  foreach(var p in coreAlarmStateRowsV21.Values){var e=new Enemy{sourceNPC=p.sourceNPC,kind=p.kind,npcState=p.state,npcTime=.25f,visualScale=1,facing=-1};RenderCoreAlarmWholeV21(e,renderer);rows.Add(new CoreLifecycleBodyRowV24{npc=e.sourceNPC,state=e.npcState,displayClip=p.displayClip,originalStateClock=e.npcTime,stateDuration=p.sourceStateDuration,motionDuration=p.sourceMotionDuration,sourceClockUnchanged=e.npcTime==.25f,fixedFrontal=!renderer.flipX&&!renderer.flipY&&renderer.transform.localScale==Vector3.one});}
  foreach(var p in coreExtractorStateRowsV21.Values){float clock=p.state=="EL_EXTRACTOR_EARTH_START"?1.5f:.25f;var e=new Enemy{sourceNPC=p.sourceNPC,kind=p.kind,npcState=p.state,npcTime=clock,visualScale=1,facing=-1};bool v=true,f=false;ApplyCoreExtractorSourceVisibilityV21(e,ref v);ApplyCoreExtractorSourceVisibilityV21(e,ref f);RenderCoreExtractorWholeV21(e,renderer);rows.Add(new CoreLifecycleBodyRowV24{npc=e.sourceNPC,state=e.npcState,displayClip=p.displayClip,originalStateClock=e.npcTime,motionPlaybackClock=CoreExtractorWholeLastMotionPlaybackClockV21,stateDuration=p.sourceStateDuration,motionDuration=p.sourceMotionDuration,sourceClockUnchanged=e.npcTime==clock,hasSourceShow=p.hasSourceShow,sourceShow=p.sourceShow,visibleTruePreserved=v,visibleFalsePreserved=!f,fixedFrontal=!renderer.flipX&&!renderer.flipY&&renderer.transform.localScale==Vector3.one});}
  host.SetActive(false);Destroy(host);if(original!=CoreLifecycleActorFingerprintV24())throw new InvalidOperationException("Private selector fixture mutated original roster");return rows.ToArray();
 }
 public int CoreLifecycleSourceIdentityCountV24 {get{return coreEntrySourceViewV21==null?0:((IDictionary)CoreLifecycleFieldV24(coreEntrySourceViewV21,"identities")).Count;}}
}}
#endif
