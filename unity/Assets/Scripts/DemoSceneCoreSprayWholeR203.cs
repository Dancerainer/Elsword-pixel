using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel { public sealed partial class DemoScene {
 const string CoreSprayDocumentShaR203="539ccfb33715beec9f1578b756724e3201526f5d9b8ee75ac6b2773ae738be13";
 const string CoreSprayRegistryAcceptanceShaR203="81123d495ced91357c01f9dfd32dcc35f62928e0e3f065b49524749199e07210";
 const string CoreSprayRulesShaR203="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 SprayFiniteWholeLookupR203 coreSprayWholeLookupR203;
 readonly DeferredWholeSpritesV53 coreSprayWholeSpritesR203=new DeferredWholeSpritesV53();
 readonly Dictionary<string,Texture2D> coreSprayWholeTexturesR203=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CoreSprayWholeConfiguredR203 {get{return coreSprayWholeLookupR203!=null;}}
 public bool CoreSprayWholeBindingsReleasedR203 {get{return coreSprayWholeLookupR203==null&&coreSprayWholeSpritesR203.Count==0&&coreSprayWholeTexturesR203.Count==0;}}
 public int CoreSprayWholeResidentKeysR203 {get{return coreSprayWholeSpritesR203.ResidentCount;}}
 public long CoreSprayWholeResidentRgbaBytesR203 {get{long n=0;foreach(var t in coreSprayWholeTexturesR203.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CoreSprayWholeDrawsR203 {get;private set;} public int CoreSprayWholeMissingStateHoldsR203 {get;private set;}
 public int CoreSprayWholeOutsideFiniteHoldsR203 {get;private set;}
 public SprayWholeSelectionR203 CoreSprayWholeLastSelectionR203 {get;private set;}
 public bool CoreSprayWholeEmitsGameplayR203 {get{return false;}}
 static PreviewFile CoreSprayPinR203(SprayWholePinR203 p){return new PreviewFile{path=p.path,sha256=p.sha256};}
 static PreviewFile CoreSprayPngR203(SprayWholeKeyInfoR203 k){return new PreviewFile{path=k.Path,sha256=k.Sha256};}
 static SprayFiniteWholeLookupR203 ValidateCoreSprayWholeDocumentR203(PreviewFile documentPin,PreviewFile acceptancePin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
   if(CoreProductResourcesActiveR1147)return ValidateCoreSprayWholeDocumentR203RuntimeR1147(documentPin,acceptancePin,sourceRulesPin,rules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CoreSprayDocumentShaR203&&acceptancePin!=null&&acceptancePin.sha256==CoreSprayRegistryAcceptanceShaR203,"Spray exact source registry acceptance/document required");CoreEntryReadV21(acceptancePin);
  var d=FiniteWholeJsonR268.Read<SprayWholeDocumentR203>(CoreEntryReadV21(documentPin));var lookup=new SprayFiniteWholeLookupR203(d);
  PreviewRequire(d.registryAcceptance.path==acceptancePin.path&&d.registryAcceptance.sha256==acceptancePin.sha256&&d.sourceRulesSha256==CoreSprayRulesShaR203&&sourceRulesPin!=null&&sourceRulesPin.sha256==CoreSprayRulesShaR203,"Spray original rules/acceptance binding; R105 is a separate future integration");
  CoreEntryReadV21(CoreSprayPinR203(d.registry));CoreEntryReadV21(CoreSprayPinR203(d.sourceClock));CoreEntryReadV21(CoreSprayPinR203(d.sourceCoverage));
  var profile=Array.Find(rules??new NpcDefinition[0],p=>p.id=="NUI_NASOD_SPRAY_ICE");PreviewRequire(profile!=null&&profile.kind=="nasod-spray-ice"&&profile.states!=null&&profile.states.Length==28,"Spray original Hard28 profile required");
  var seen=new HashSet<string>(StringComparer.Ordinal);
  foreach(var actual in profile.states){var s=lookup.State(actual.id);PreviewRequire(seen.Add(actual.id)&&actual.clip==s.Clip&&actual.sourceAnimation==s.Motion&&actual.sourceAnimSpeed==s.Speed&&actual.duration==s.Duration&&actual.loop==s.Loop&&actual.nextState==s.NextState&&actual.holdForSourceEvent==s.HoldForSourceEvent,"Spray original state, independent expiry and event-hold projection changed "+actual.id);}
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));
  foreach(var key in lookup.Keys()){string path=Path.GetFullPath(Path.Combine(directory,key.File.Replace('/',Path.DirectorySeparatorChar)));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.Path),StringComparison.OrdinalIgnoreCase),"Spray native PNG escaped/mismatched package");CoreEntryReadV21(CoreSprayPngR203(key));}
  return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCoreSprayWholeR203(PreviewFile documentPin,PreviewFile acceptancePin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
  PreviewRequire(CoreSprayWholeBindingsReleasedR203,"Spray display owner must be empty before configure");var lookup=ValidateCoreSprayWholeDocumentR203(documentPin,acceptancePin,sourceRulesPin,rules);
  foreach(var key in lookup.Keys()){
   coreSprayWholeSpritesR203.Register(key.Key,DeferMonsterPixelCarrierV53("nasod-spray-ice"),()=>{
   byte[] bytes=CoreEntryReadV21(CoreSprayPngR203(key));
   var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core Spray whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
   // Enroll the native object before fallible decode. Existing common cancel,
   // End and OnDestroy own destruction; display dictionaries never double-destroy.
   forestPreviewOwned.Add(texture);coreSprayWholeTexturesR203.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Spray approved full native PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core Spray whole / "+key.Key;forestPreviewOwned.Add(sprite);return sprite;
   });
  }
  coreSprayWholeLookupR203=lookup;CoreSprayWholeDrawsR203=CoreSprayWholeMissingStateHoldsR203=CoreSprayWholeOutsideFiniteHoldsR203=0;CoreSprayWholeLastSelectionR203=null;CoreSprayWholeEndpointDiagnosticDrawsR203=0;CoreSprayWholeLastEndpointDiagnosticR203=null;
 }
 bool RenderCoreSprayWholeR203(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_NASOD_SPRAY_ICE")return false;
  PreviewRequire(coreSprayWholeLookupR203!=null,"Spray exact display library not configured");
  string displayState;float displayTime;
  if(!TryCoreReactionDisplayV32(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;CoreSprayWholeMissingStateHoldsR203++;CoreSprayWholeLastSelectionR203=null;return true;}
  PreviewRequire(e.kind=="nasod-spray-ice"&&e.visualScale==1,"Spray1.2 source scale is already baked; extra scale must stay1");
  var selected=coreSprayWholeLookupR203.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/Spray",displayState,selected.State.Motion,(float)selected.SourceMotionClock,1,(float)selected.State.OriginalAddRotateZ);
  DrawCoreSprayRegisteredWholeR203(key,selected.State.OriginalAddRotateZ,renderer,RenderEnemyPosition(e));
  RecordCoreSpraySelectionV35(e,renderer,selected);
  CoreSprayWholeLastSelectionR203=selected;CoreSprayWholeDrawsR203++;if(selected.OutsideFiniteRange)CoreSprayWholeOutsideFiniteHoldsR203++;return true;
 }
 public int CoreSprayWholeEndpointDiagnosticDrawsR203 {get;private set;}
 public string CoreSprayWholeLastEndpointDiagnosticR203 {get;private set;}
 void DrawCoreSprayRegisteredWholeR203(SprayWholeKeyInfoR203 key,double originalZ,SpriteRenderer renderer,Vector2 actorRoot){
  double ox,oy;SprayWholePlacementR203.CenterOffset(key,originalZ,out ox,out oy);
  renderer.enabled=true;renderer.sprite=coreSprayWholeSpritesR203[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.Euler(0,0,(float)originalZ);
  renderer.transform.localPosition=new Vector3(actorRoot.x+(float)ox,actorRoot.y+(float)oy,0);
 }
 // Explicit zero-dwell fixture path through this same provider and native cache.
 // It is never called by normal state rendering, never mutates Enemy/FSM/clock,
 // and cannot fabricate Wait's unknown endpoint. Future GPU coverage must invoke
 // this path explicitly for the endpoint-only AttackA_Ready-05 whole.
 public bool RenderCoreSprayEndpointDiagnosticR203(string state,SpriteRenderer renderer,Vector2 actorRoot){
  PreviewRequire(coreSprayWholeLookupR203!=null,"Spray endpoint diagnostic requires configured owner");
  var s=coreSprayWholeLookupR203.State(state);var endpoint=coreSprayWholeLookupR203.EndpointDiagnostic(s.Motion);
  CoreSprayWholeLastEndpointDiagnosticR203=null;
  if(!endpoint.SourceKnown){renderer.sprite=null;renderer.enabled=false;return false;}
  DrawCoreSprayRegisteredWholeR203(endpoint.Whole,s.OriginalAddRotateZ,renderer,actorRoot);CoreSprayWholeEndpointDiagnosticDrawsR203++;CoreSprayWholeLastEndpointDiagnosticR203=endpoint.MetadataWholeKey;return true;
 }
 void DisposeCoreSprayWholeR203(){coreSprayWholeSpritesR203.Clear();coreSprayWholeTexturesR203.Clear();coreSprayWholeLookupR203=null;CoreSprayWholeLastSelectionR203=null;CoreSprayWholeLastEndpointDiagnosticR203=null;}
}}

