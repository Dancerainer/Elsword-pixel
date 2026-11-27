using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel { public sealed partial class DemoScene {
 const string CoreWallyDocumentShaR198="993de8969191996247a1be596100a34de11387dd0b8305f277e60baf36d505d9";
 const string CoreWallyRegistryAcceptanceShaR198="52580bc74ef49a3c02bb65f11d24a971cb008e27d5f0edb858b98003a8cb1470";
 const string CoreWallyRulesShaR198="863c195c11bd11aef5c7a57bddbf0e05a27590369b7fbcf5e7b9d72675f5ae72";
 WallyFiniteWholeLookupR198 coreWallyWholeLookupR198;
 readonly DeferredWholeSpritesV53 coreWallyWholeSpritesR198=new DeferredWholeSpritesV53();
 readonly Dictionary<string,Texture2D> coreWallyWholeTexturesR198=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
 public bool CoreWallyWholeConfiguredR198 {get{return coreWallyWholeLookupR198!=null;}}
 public bool CoreWallyWholeBindingsReleasedR198 {get{return coreWallyWholeLookupR198==null&&coreWallyWholeSpritesR198.Count==0&&coreWallyWholeTexturesR198.Count==0;}}
 public int CoreWallyWholeResidentKeysR198 {get{return coreWallyWholeSpritesR198.ResidentCount;}}
 public long CoreWallyWholeResidentRgbaBytesR198 {get{long n=0;foreach(var t in coreWallyWholeTexturesR198.Values)n+=(long)t.width*t.height*4;return n;}}
 public int CoreWallyWholeDrawsR198 {get;private set;} public int CoreWallyWholeMissingStateHoldsR198 {get;private set;}
 public int CoreWallyWholeOutsideFiniteHoldsR198 {get;private set;}
 public WallyWholeSelectionR198 CoreWallyWholeLastSelectionR198 {get;private set;}
 public bool CoreWallyWholeEmitsGameplayR198 {get{return false;}}
 static PreviewFile CoreWallyPinR198(WallyWholePinR198 p){return new PreviewFile{path=p.path,sha256=p.sha256};}
 static PreviewFile CoreWallyPngR198(WallyWholeKeyInfoR198 k){return new PreviewFile{path=k.Path,sha256=k.Sha256};}
 static WallyFiniteWholeLookupR198 ValidateCoreWallyWholeDocumentR198(PreviewFile documentPin,PreviewFile acceptancePin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
   if(CoreProductResourcesActiveR1147)return ValidateCoreWallyWholeDocumentR198RuntimeR1147(documentPin,acceptancePin,sourceRulesPin,rules);
#if UNITY_EDITOR

  PreviewRequire(documentPin!=null&&documentPin.sha256==CoreWallyDocumentShaR198&&acceptancePin!=null&&acceptancePin.sha256==CoreWallyRegistryAcceptanceShaR198,"Wally exact source registry acceptance/document required");CoreEntryReadV21(acceptancePin);
  var d=FiniteWholeJsonR268.Read<WallyWholeDocumentR198>(CoreEntryReadV21(documentPin));var lookup=new WallyFiniteWholeLookupR198(d);
  PreviewRequire(d.registryAcceptance.path==acceptancePin.path&&d.registryAcceptance.sha256==acceptancePin.sha256&&d.sourceRulesSha256==CoreWallyRulesShaR198&&sourceRulesPin!=null&&sourceRulesPin.sha256==CoreWallyRulesShaR198,"Wally original rules/acceptance binding; R105 is a separate future integration");
  CoreEntryReadV21(CoreWallyPinR198(d.registry));CoreEntryReadV21(CoreWallyPinR198(d.sourceClock));
  var profile=Array.Find(rules??new NpcDefinition[0],p=>p.id=="NUI_WALLY_9TH_TYPE2");PreviewRequire(profile!=null&&profile.kind=="wally-9th-type2"&&profile.states!=null&&profile.states.Length==27,"Wally original Hard27 profile required");
  var seen=new HashSet<string>(StringComparer.Ordinal);
  foreach(var actual in profile.states){var s=lookup.State(actual.id);PreviewRequire(seen.Add(actual.id)&&actual.clip==s.Clip&&actual.sourceAnimation==s.Motion&&actual.sourceAnimSpeed==s.Speed&&actual.duration==s.Duration&&actual.loop==s.Loop&&actual.nextState==s.NextState&&actual.holdForSourceEvent==s.HoldForSourceEvent,"Wally original state, independent expiry and event-hold projection changed "+actual.id);}
  string directory=Path.GetDirectoryName(Path.GetFullPath(documentPin.path));
  foreach(var key in lookup.Keys()){string path=Path.GetFullPath(Path.Combine(directory,key.File.Replace('/',Path.DirectorySeparatorChar)));PreviewRequire(path.StartsWith(directory+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)&&string.Equals(path,Path.GetFullPath(key.Path),StringComparison.OrdinalIgnoreCase),"Wally native PNG escaped/mismatched package");CoreEntryReadV21(CoreWallyPngR198(key));}
  return lookup;
 
#else
   throw new InvalidOperationException("Core filesystem/review validator is Editor-only");
#endif
  }
 void ConfigureCoreWallyWholeR198(PreviewFile documentPin,PreviewFile acceptancePin,PreviewFile sourceRulesPin,NpcDefinition[] rules){
  PreviewRequire(CoreWallyWholeBindingsReleasedR198,"Wally display owner must be empty before configure");var lookup=ValidateCoreWallyWholeDocumentR198(documentPin,acceptancePin,sourceRulesPin,rules);
  foreach(var key in lookup.Keys()){
   coreWallyWholeSpritesR198.Register(key.Key,DeferMonsterPixelCarrierV53("wally-9th-type2"),()=>{
   byte[] bytes=CoreEntryReadV21(CoreWallyPngR198(key));
   var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){name="Core Wally whole / "+key.Key,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
   // Enroll the native object before fallible decode. Existing common cancel,
   // End and OnDestroy own destruction; display dictionaries never double-destroy.
   forestPreviewOwned.Add(texture);coreWallyWholeTexturesR198.Add(key.Key,texture);
   PreviewRequire(ImageConversion.LoadImage(texture,bytes,true)&&texture.width==key.Width&&texture.height==key.Height,"Wally approved full native PNG decode/dimensions");
   var sprite=Sprite.Create(texture,new Rect(0,0,key.Width,key.Height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);sprite.name="Core Wally whole / "+key.Key;forestPreviewOwned.Add(sprite);return sprite;
   });
  }
  coreWallyWholeLookupR198=lookup;CoreWallyWholeDrawsR198=CoreWallyWholeMissingStateHoldsR198=CoreWallyWholeOutsideFiniteHoldsR198=0;CoreWallyWholeLastSelectionR198=null;
 }
 bool RenderCoreWallyWholeR198(Enemy e,SpriteRenderer renderer){
  if(e==null||e.sourceNPC!="NUI_WALLY_9TH_TYPE2")return false;
  PreviewRequire(coreWallyWholeLookupR198!=null,"Wally exact display library not configured");
  string displayState;float displayTime;
  if(!TryCoreReactionDisplayV32(e,out displayState,out displayTime)){renderer.enabled=false;renderer.sprite=null;CoreWallyWholeMissingStateHoldsR198++;CoreWallyWholeLastSelectionR198=null;return true;}
  PreviewRequire(e.kind=="wally-9th-type2"&&e.visualScale==1,"Wally .9 source scale is already baked; extra scale must stay1");
  var selected=coreWallyWholeLookupR198.Select(displayState,displayTime);var key=selected.Whole;
  RecordMonsterPixelSourceV53(e,renderer,"Core/Wally",displayState,selected.State.Motion,(float)selected.SourceMotionClock,e.facing<0?-1:1);
  renderer.enabled=true;renderer.sprite=coreWallyWholeSpritesR198[key.Key];renderer.flipX=renderer.flipY=false;renderer.transform.localScale=Vector3.one;renderer.transform.localRotation=Quaternion.identity;
  // A centered full-rect sprite plus its signed root reproduces the registered
  // native placement, including roots outside the canvas. No clamp/resize/floor
  // or SpriteOffset is applied, and no actor/geometry/FSM field is written.
  var offset=new Vector2((float)(key.Width*.5-key.RootX),(float)(key.RootY-key.Height*.5));var position=RenderEnemyPosition(e)+offset;renderer.transform.localPosition=new Vector3(position.x,position.y,0);
  FitCoreEliteWholeV35(e,renderer,selected.State.Motion,selected.SourceMotionClock);
  CoreWallyWholeLastSelectionR198=selected;CoreWallyWholeDrawsR198++;if(selected.OutsideFiniteRange)CoreWallyWholeOutsideFiniteHoldsR198++;return true;
 }
 void DisposeCoreWallyWholeR198(){coreWallyWholeSpritesR198.Clear();coreWallyWholeTexturesR198.Clear();coreWallyWholeLookupR198=null;CoreWallyWholeLastSelectionR198=null;}
}}

