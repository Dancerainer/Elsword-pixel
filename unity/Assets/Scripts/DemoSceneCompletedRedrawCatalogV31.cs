using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  // The delta records its original authoring catalog. This edition pins a scoped copy.
  // inspect its original bytes and the Mine2 capture keeps its original DTO.
  const string CompletedRedrawCatalogResourceV31="Pixel/feedback-v29/completed-started7/catalog-delta";
  const string CompletedRedrawCatalogBaseHashV31="05a8b62aaa462f4e57de18c6bbeee89a62190dab7caf79e6d2f62203877b97b3";
  const string CompletedRedrawCatalogDeltaHashV31="5a29824d85b8dff23d629af511596f1d89ca41da3f747ac84e11806e9f0eef0c";
  const int CompletedRedrawCatalogBaseBytesV31=41191929;
  const string ReleaseCatalogHash="8be5db6ffc7de940412fda359ee7c9bb0b0fa32768d6210e037ff8850cdf839e";
  const int ReleaseCatalogBytes=22291357;
  const int CompletedRedrawCatalogDeltaBytesV31=6075678;
  bool completedRedrawCatalogAppliedV31;

  [Serializable] sealed class CompletedRedrawMonsterClipsV31 {
   public string kind;public AnimationData[] clips;public string[] replaceExistingIds;
  }
  [Serializable] sealed class CompletedRedrawNpcStatesV31 {
   public string npc,initialState;public bool setInitialState;public NpcStateDefinition[] states;
  }
  [Serializable] sealed class CompletedRedrawDeltaV31 {
   public string schema,baseCatalogSha256;public int baseCatalogBytes;
   public CompletedRedrawMonsterClipsV31[] monsterClips;
   public NpcDefinition[] npcDefinitionsToAdd;
   public CompletedRedrawNpcStatesV31[] npcStatesToAppend;
   public NpcEffectDefinition[] effectDefinitionsToAdd;
   public NpcEffectBinding[] effectBindingsToAdd;
   public NpcMechanismStateRule[] mechanismStatesToAdd;
  }
  static void CompletedRedrawNeedV31(bool condition,string message){
   if(!condition)throw new InvalidOperationException("Completed started-scene art data: "+message);
  }
  static string CompletedRedrawHashV31(byte[] raw){
   using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(raw)).Replace("-","").ToLowerInvariant();
  }
  static T[] CompletedRedrawAppendV31<T>(T[] current,T[] added,Func<T,string> key,int expectedAdded)where T:class {
   CompletedRedrawNeedV31(current!=null&&added!=null&&added.Length==expectedAdded,"exact append count");
   var result=new List<T>(current);var seen=new HashSet<string>(StringComparer.Ordinal);
   foreach(var row in current)CompletedRedrawNeedV31(row!=null&&!string.IsNullOrEmpty(key(row))&&seen.Add(key(row)),"unique existing entity key");
   foreach(var row in added){CompletedRedrawNeedV31(row!=null&&!string.IsNullOrEmpty(key(row))&&seen.Add(key(row)),"append cannot replace an existing entity");result.Add(row);}
   return result.ToArray();
  }
  static string CompletedRedrawPairV31(string npc,string state){
   CompletedRedrawNeedV31(!string.IsNullOrEmpty(npc)&&!string.IsNullOrEmpty(state),"complete NPC/state key");return npc+"\n"+state;
  }
  static void CompletedRedrawBaseShapeV31(ArtCatalog value){
   CompletedRedrawNeedV31(value!=null&&value.dungeons!=null&&value.dungeons.Length==ReleaseDungeonIds.Length&&value.stages!=null&&value.stages.Length==ReleaseRoomCount&&value.monsters!=null&&value.monsters.Length==60&&value.effects!=null&&value.effects.Length==75&&value.npcContentSets!=null&&value.npcContentSets.Length==ReleaseContentSetCount,"release route, library and content-set counts");
  }
  // Apply the original reviewed delta before creating frames or running the content validators.
  ArtCatalog ApplyCompletedRedrawCatalogV31(CatalogDigest digest,string baseText,ArtCatalog original){
   completedRedrawCatalogAppliedV31=false;fairySourceSoundAppliedV34=false;CompletedRedrawBaseShapeV31(original);
   CompletedRedrawNeedV31(baseText!=null,"release catalog resource");
   CompletedRedrawNeedV31(digest.Matches(ReleaseCatalogBytes,ReleaseCatalogHash),"Ruben and Elder catalog identity");
   var asset=Resources.Load<TextAsset>(CompletedRedrawCatalogResourceV31);CompletedRedrawNeedV31(asset,"completed art delta resource");var bytes=asset.bytes;
   CompletedRedrawNeedV31(bytes.Length==CompletedRedrawCatalogDeltaBytesV31&&CompletedRedrawHashV31(bytes)==CompletedRedrawCatalogDeltaHashV31,"completed art delta identity");
   var delta=JsonUtility.FromJson<CompletedRedrawDeltaV31>(Encoding.UTF8.GetString(bytes));
   CompletedRedrawNeedV31(delta!=null&&delta.schema=="completed-started7-catalog-delta-v31"&&delta.baseCatalogSha256==CompletedRedrawCatalogBaseHashV31&&delta.baseCatalogBytes==CompletedRedrawCatalogBaseBytesV31,"delta binds this Main base");
   // On Android the original digest and shape were captured before any patch.
   // No consumer retains this DTO; the immutable Resources bytes remain the baseline.
   var result=original;
   if(!StreamMobileArt){
    result=JsonUtility.FromJson<ArtCatalog>(baseText);CompletedRedrawBaseShapeV31(result);
    CompletedRedrawNeedV31(!ReferenceEquals(result,original)&&!ReferenceEquals(result.monsters,original.monsters)&&!ReferenceEquals(result.npcRules,original.npcRules)&&!ReferenceEquals(result.npcEffectRules,original.npcEffectRules)&&!ReferenceEquals(result.npcMechanismRules,original.npcMechanismRules),"separate DTO and mutable data owners");
   }
   var librariesById=new Dictionary<string,SpriteLibraryData>(StringComparer.Ordinal);
   foreach(var lib in result.monsters)CompletedRedrawNeedV31(lib!=null&&!string.IsNullOrEmpty(lib.id)&&!librariesById.ContainsKey(lib.id)&&AddCompletedRedrawLibraryV31(librariesById,lib),"unique original monster library");
   CompletedRedrawNeedV31(delta.monsterClips!=null&&delta.monsterClips.Length==25,"exact completed monster families");
   var patchedKinds=new HashSet<string>(StringComparer.Ordinal);int appended=0,replaced=0;
   foreach(var patch in delta.monsterClips){
    SpriteLibraryData lib=null;CompletedRedrawNeedV31(patch!=null&&patchedKinds.Add(patch.kind)&&librariesById.TryGetValue(patch.kind,out lib)&&patch.clips!=null&&patch.replaceExistingIds!=null,"known one-time monster delta");
    var list=new List<AnimationData>(lib.clips);var indices=new Dictionary<string,int>(StringComparer.Ordinal);
    for(int i=0;i<list.Count;i++){CompletedRedrawNeedV31(list[i]!=null&&!string.IsNullOrEmpty(list[i].id)&&!indices.ContainsKey(list[i].id),"unique original clip ID");indices.Add(list[i].id,i);}
    var replacementIds=new HashSet<string>(patch.replaceExistingIds,StringComparer.Ordinal);var appliedIds=new HashSet<string>(StringComparer.Ordinal);var replacedIds=new HashSet<string>(StringComparer.Ordinal);
    foreach(var clip in patch.clips){
     CompletedRedrawNeedV31(clip!=null&&!string.IsNullOrEmpty(clip.id)&&clip.id.StartsWith("source-",StringComparison.Ordinal)&&appliedIds.Add(clip.id)&&clip.frames!=null&&clip.frames.Length>0,"completed source clip delta");int index;
     if(indices.TryGetValue(clip.id,out index)){
      CompletedRedrawNeedV31((patch.kind=="tree-knight"||patch.kind=="bat-a"||patch.kind=="bat-b")&&replacementIds.Contains(clip.id),"only three approved 24Hz source banks replace existing aliases");
      list[index]=clip;replacedIds.Add(clip.id);replaced++;
     }else{CompletedRedrawNeedV31(!replacementIds.Contains(clip.id),"replacement alias must already exist");indices.Add(clip.id,list.Count);list.Add(clip);appended++;}
    }
    CompletedRedrawNeedV31(replacementIds.SetEquals(replacedIds),"all declared replacements consumed");lib.clips=list.ToArray();
   }
   CompletedRedrawNeedV31(appended==239&&replaced==37,"exact clip append/replacement scope");
   CompletedRedrawNeedV31(result.npcRules!=null&&result.npcRules.Length==5&&result.npcEffectRules!=null&&result.npcEffectRules.definitions.Length==8&&result.npcEffectRules.bindings.Length==18&&result.npcMechanismRules!=null&&result.npcMechanismRules.states.Length==26,"original Main rules/effects/mechanism scope");
   CompletedRedrawNeedV31(delta.npcStatesToAppend!=null&&delta.npcStatesToAppend.Length==1,"one existing NPC state append");
   var ent=delta.npcStatesToAppend[0];CompletedRedrawNeedV31(ent!=null&&ent.npc=="NUI_ANGER_FOREST_ENT"&&ent.setInitialState&&ent.initialState=="ANGER_FOREST_ENT_START"&&ent.states!=null&&ent.states.Length==1&&ent.states[0].id=="ANGER_FOREST_ENT_START","exact completed Ent emergence state");
   NpcDefinition existingEnt=null;foreach(var rule in result.npcRules)if(rule.id==ent.npc)existingEnt=rule;
   CompletedRedrawNeedV31(existingEnt!=null&&string.IsNullOrEmpty(existingEnt.initialState),"original Ent initial state unchanged");existingEnt.states=CompletedRedrawAppendV31(existingEnt.states,ent.states,r=>r.id,1);existingEnt.initialState=ent.initialState;
   result.npcRules=CompletedRedrawAppendV31(result.npcRules,delta.npcDefinitionsToAdd,r=>r.id,4);
   WhiteGiantRushV45.RestoreChoices(result.npcRules);
   result.npcEffectRules.definitions=CompletedRedrawAppendV31(result.npcEffectRules.definitions,delta.effectDefinitionsToAdd,r=>r.id,8);
   result.npcEffectRules.bindings=CompletedRedrawAppendV31(result.npcEffectRules.bindings,delta.effectBindingsToAdd,r=>CompletedRedrawPairV31(r.npc,r.state),25);
   result.npcMechanismRules.states=CompletedRedrawAppendV31(result.npcMechanismRules.states,delta.mechanismStatesToAdd,r=>CompletedRedrawPairV31(r.npc,r.state),25);
   CompletedRedrawBaseShapeV31(result);CompletedRedrawNeedV31(result.npcRules.Length==9&&result.npcEffectRules.definitions.Length==16&&result.npcEffectRules.bindings.Length==43&&result.npcMechanismRules.states.Length==51,"completed started-scene union counts");
   ApplyFairySourceSoundV34(result);completedRedrawCatalogAppliedV31=true;return result;
  }
  static bool AddCompletedRedrawLibraryV31(Dictionary<string,SpriteLibraryData> map,SpriteLibraryData lib){map.Add(lib.id,lib);return true;}
  bool CompletedRedrawCatalogReadyV31(){return completedRedrawCatalogAppliedV31;}
 }
}
