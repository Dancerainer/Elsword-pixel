using System;using System.Collections.Generic;using UnityEngine;
namespace ElswordPixel {
 [Serializable] public sealed class BattleProductPcmRowR1325 {public string id,originalOggSha256,resource,wavSha256,pcmSha256;public int frames,channels,sampleRate;}
 [Serializable] public sealed class BattleProductPcmDocumentR1325 {public string schema;public BattleProductPcmRowR1325[] rows;}
 public sealed partial class DemoScene {
  const string BattleProductPcmResourceR1325="Pixel/dungeons/altera-battle-airship/pcm-index-r1325";
  static bool BattleProductHexR1325(string value){if(value==null||value.Length!=64)return false;foreach(char c in value)if(!(c>='0'&&c<='9'||c>='a'&&c<='f'))return false;return true;}
  void RequireBattleProductBorrowedSoundR1325(BattleProductAudioRowR1282 row,AudioClip clip){
   AudioClip prior;bool source=false;if(catalog.npcSoundRules!=null&&catalog.npcSoundRules.assets!=null)source=Array.Exists(catalog.npcSoundRules.assets,a=>a.id==row.id&&a.sha256==row.original.sha256);if(!source&&catalog.npcContentSets!=null)foreach(var set in catalog.npcContentSets)if(set!=null&&set.npcSoundRules!=null&&set.npcSoundRules.assets!=null&&Array.Exists(set.npcSoundRules.assets,a=>a.id==row.id&&a.sha256==row.original.sha256)){source=true;break;}
   BattleProductNeedR1325(row.reusePublishedClip&&source&&catalog.npcSounds!=null&&Array.Exists(catalog.npcSounds,a=>a.id==row.id&&a.resource==row.publishedResource)&&audioClips.TryGetValue(row.id,out prior)&&ReferenceEquals(prior,clip),"borrowed Main source sound/resource/object identity changed "+row.id);
  }
  static BattleProductPcmDocumentR1325 LoadBattleProductPcmMapR1325(BattleProductDataPlanR1282 p){
   var asset=Resources.Load<TextAsset>(BattleProductPcmResourceR1325);BattleProductNeedR1325(asset,"actual original Battle PCM index absent");var doc=BattleProductTypedJsonR1282.Read<BattleProductPcmDocumentR1325>(asset.bytes);BattleProductNeedR1325(doc!=null&&doc.schema=="battle-airship-original-pcm-resources-r1325"&&doc.rows!=null&&doc.rows.Length==61,"actual61 original PCM resources required");var seen=new HashSet<string>(StringComparer.Ordinal);
   foreach(var row in doc.rows){var original=Array.Find(p.Audio.rows,x=>x.id==row.id);BattleProductNeedR1325(original!=null&&!original.reusePublishedClip&&seen.Add(row.id)&&original.original.sha256==row.originalOggSha256&&row.resource=="Pixel/dungeons/altera-battle-airship/audio/"+row.originalOggSha256&&row.frames==original.sourcePcmFrames&&row.channels==original.channels&&row.sampleRate==original.sampleRate&&BattleProductHexR1325(row.wavSha256)&&BattleProductHexR1325(row.pcmSha256),"actual source PCM identity/metadata/index changed "+row.id);}
   // Main's two published OGG clips remain borrowed. Full PCM hash claim applies to the61 real WAV resources only.
   foreach(var row in p.Audio.rows)if(!row.reusePublishedClip)BattleProductNeedR1325(seen.Contains(row.id),"source PCM row missing "+row.id);return doc;
  }
 }
}
