using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel { public sealed partial class DemoScene {
 const string CoreSheriffDocumentShaR227="4d02e9901ca91a4e60b2b3b4a02daa4be51d05fae854eea6faea1eb9c999ba1c";
 const string CoreSheriffRegistryAcceptanceShaR227="cd21bce081f4be2ddb82ef2a2d45d9fca87a7bd8080c384fc58dadaefbc4ef53";
 const string CoreSheriffRulesShaR227="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 SheriffFiniteWholeLookupR227 coreSheriffWholeLookupR227;
 readonly DeferredWholeSpritesV53 coreSheriffWholeSpritesR227=new DeferredWholeSpritesV53();
 readonly Dictionary<string,Texture2D> coreSheriffWholeTexturesR227=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CoreSheriffWholeConfiguredR227 {get{return coreSheriffWholeLookupR227!=null;}}
 public bool CoreSheriffWholeBindingsReleasedR227 {get{return coreSheriffWholeLookupR227==null&&coreSheriffWholeSpritesR227.Count==0&&coreSheriffWholeTexturesR227.Count==0;}}
 public int CoreSheriffWholeResidentKeysR227 {get{return coreSheriffWholeSpritesR227.ResidentCount;}}
 public long CoreSheriffWholeResidentRgbaBytesR227 {get{long n=0;foreach(var t in coreSheriffWholeTexturesR227.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CoreSheriffWholeDrawsR227 {get;private set;} public int CoreSheriffWholeMissingStateHoldsR227 {get;private set;}
 public int CoreSheriffWholeOutsideFiniteHoldsR227 {get;private set;}
 public SheriffWholeSelectionR227 CoreSheriffWholeLastSelectionR227 {get;private set;}
 public bool CoreSheriffWholeEmitsGameplayR227 {get{return false;}}
 static PreviewFile CoreSheriffPinR227(SheriffWholePinR227 p){return new PreviewFile{path=p.path,sha256=p.sha256};}
 static PreviewFile CoreSheriffPngR227(SheriffWholeKeyInfoR227 k){return new PreviewFile{path=k.Path,sha256=k.Sha256};}
 static SheriffFiniteWholeLookupR227 ValidateCoreSheriffWholeDocumentR227(PreviewFile documentPin,PreviewFile acceptancePin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
   if(CoreProductResourcesActiveR1147)return ValidateCoreSheriffWholeDocumentR227RuntimeR1147(documentPin,acceptancePin,sourceRulesPin,rules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CoreSheriffDocumentShaR227&&acceptancePin!=null&&acceptancePin.sha256==CoreSheriffRegistryAcceptanceShaR227,"Sheriff exact source registry acceptance/document required");CoreEntryReadV21(acceptancePin);
  var d=FiniteWholeJsonR268.Read<SheriffWholeDocumentR227>(CoreEntryReadV21(documentPin));var lookup=new SheriffFiniteWholeLookupR227(d);
  PreviewRequire(d.registryAcceptance.path==acceptancePin.path&&d.registryAcceptance.sha256==acceptancePin.sha256&&d.sourceRulesSha256==CoreSheriffRulesShaR227&&sourceRulesPin!=null&&sourceRulesPin.sha256==CoreSheriffRulesShaR227,"Sheriff original rules/acceptance binding; active gameplay rules are independently validated by the R215 dual pin gate");
  CoreEntryReadV21(CoreSheriffPinR227(d.registry));CoreEntryReadV21(CoreSheriffPinR227(d.sourceClock));CoreEntryReadV21(CoreSheriffPinR227(d.sourceCoverage));
  var profile=Array.Find(rules??new NpcDefinition[0],p=>p.id=="NUI_NASOD_ELITE_SHERIFF");PreviewRequire(profile!=null&&profile.kind=="nasod-elite-sheriff"&&profile.states!=null&&profile.states.Length==28,"Sheriff original Hard28 profile required");
  var seen=new HashSet<string>(StringComparer.Ordinal);
  foreach(var actual in profile.states){var s=lookup.State(actual.id);PreviewRequire(seen.Add(actual.id)&&actual.clip==s.Clip&&actual.sourceAnimation==s.Motion&&actual.sourceAnimSpeed==s.Speed&&actual.duration==s.Duration&&actual.loop==s.Loop&&actual.nextState==s.NextState&&actual.holdForSourceEvent==s.HoldForSourceEvent,"Sheriff original state, independent expiry and event-hold projection changed "+actual.id);}
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));
  foreach(var key in lookup.Keys()){string path=Path.GetFullPath(Path.Combine(directory,key.File.Replace('/',Path.DirectorySeparatorChar)));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.Path),StringComparison.OrdinalIgnoreCase),"Sheriff native PNG escaped/mismatched package");CoreEntryReadV21(CoreSheriffPngR227(key));}
  return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCoreSheriffWholeR227(PreviewFile documentPin,PreviewFile acceptancePin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
  PreviewRequire(CoreSheriffWholeBindingsReleasedR227,"Sheriff display owner must be empty before configure");var lookup=ValidateCoreSheriffWholeDocumentR227(documentPin,acceptancePin,sourceRulesPin,rules);
  foreach(var key in lookup.Keys()){
   coreSheriffWholeSpritesR227.Register(key.Key,DeferMonsterPixelCarrierV53("nasod-elite-sheriff"),()=>{
   byte[] bytes=CoreEntryReadV21(CoreSheriffPngR227(key));
   var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core Sheriff whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
   // Enroll the native object before fallible decode. Existing common cancel,
   // End and OnDestroy own destruction; display dictionaries never double-destroy.
   forestPreviewOwned.Add(texture);coreSheriffWholeTexturesR227.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Sheriff approved full native PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core Sheriff whole / "+key.Key;forestPreviewOwned.Add(sprite);return sprite;
   });
  }
  coreSheriffWholeLookupR227=lookup;CoreSheriffWholeDrawsR227=CoreSheriffWholeMissingStateHoldsR227=CoreSheriffWholeOutsideFiniteHoldsR227=0;CoreSheriffWholeLastSelectionR227=null;CoreSheriffWholeEndpointDiagnosticDrawsR227=0;CoreSheriffWholeLastEndpointDiagnosticR227=null;
 }
 bool RenderCoreSheriffWholeR227(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_NASOD_ELITE_SHERIFF")return false;
  PreviewRequire(coreSheriffWholeLookupR227!=null,"Sheriff exact display library not configured");
  string displayState;float displayTime;
  if(!TryCoreReactionDisplayV32(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;CoreSheriffWholeMissingStateHoldsR227++;CoreSheriffWholeLastSelectionR227=null;return true;}
  PreviewRequire(e.kind=="nasod-elite-sheriff"&&e.visualScale==1,"Sheriff1 source scale is already baked; extra scale must stay1");
  var selected=coreSheriffWholeLookupR227.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/Sheriff",displayState,selected.State.Motion,(float)selected.SourceMotionClock,e.facing<0?-1:1,(float)selected.State.OriginalAddRotateZ*(e.facing<0?-1:1));
  DrawCoreSheriffRegisteredWholeR227(key,selected.State.OriginalAddRotateZ,renderer,RenderEnemyPosition(e));
  FitCoreEliteWholeV35(e,renderer,selected.State.Motion,selected.SourceMotionClock);
  CoreSheriffWholeLastSelectionR227=selected;CoreSheriffWholeDrawsR227++;if(selected.OutsideFiniteRange)CoreSheriffWholeOutsideFiniteHoldsR227++;return true;
 }
 public int CoreSheriffWholeEndpointDiagnosticDrawsR227 {get;private set;}
 public string CoreSheriffWholeLastEndpointDiagnosticR227 {get;private set;}
 void DrawCoreSheriffRegisteredWholeR227(SheriffWholeKeyInfoR227 key,double originalZ,SpriteRenderer renderer,Vector2 actorRoot){
  double ox,oy;SheriffWholePlacementR227.CenterOffset(key,originalZ,out ox,out oy);
  renderer.enabled=true;renderer.sprite=coreSheriffWholeSpritesR227[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.Euler(0,0,(float)originalZ);
  renderer.transform.localPosition=new Vector3(actorRoot.x+(float)ox,actorRoot.y+(float)oy,0);
 }
 // Explicit zero-dwell fixture path through this same provider and native cache.
 // It is never called by normal state rendering, never mutates Enemy/FSM/clock,
 // and cannot fabricate Wait's unknown endpoint or Ready's unassigned endpoint art.
 // All142 wholes already have positive exposure; these21 endpoints add no dwell.
 public bool RenderCoreSheriffEndpointDiagnosticR227(string state,SpriteRenderer renderer,Vector2 actorRoot){
  PreviewRequire(coreSheriffWholeLookupR227!=null,"Sheriff endpoint diagnostic requires configured owner");
  var s=coreSheriffWholeLookupR227.State(state);var endpoint=coreSheriffWholeLookupR227.EndpointDiagnostic(s.Motion);
  CoreSheriffWholeLastEndpointDiagnosticR227=null;
  if(!endpoint.SourceKnown||endpoint.Whole==null){renderer.sprite=null;renderer.enabled=false;return false;}
  DrawCoreSheriffRegisteredWholeR227(endpoint.Whole,s.OriginalAddRotateZ,renderer,actorRoot);CoreSheriffWholeEndpointDiagnosticDrawsR227++;CoreSheriffWholeLastEndpointDiagnosticR227=endpoint.MetadataWholeKey;return true;
 }
 void DisposeCoreSheriffWholeR227(){coreSheriffWholeSpritesR227.Clear();coreSheriffWholeTexturesR227.Clear();coreSheriffWholeLookupR227=null;CoreSheriffWholeLastSelectionR227=null;CoreSheriffWholeLastEndpointDiagnosticR227=null;}
}}

