using System;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  PlainMineMajorViewR1302 plainMineMajorViewR1302;GameModel plainMineMajorModelR1302;
  // Future Plain Host calls after the existing native world/spatial bridge.
  // Texture bindings must come from the Root-reviewed original resource map.
  void BindPlainMineMajorViewR1302(PlainMineTextureR1302[] originalTextures,PlainMineAudioR1302[] originalSounds,TextAsset originalMeshData,Shader originalViewShader){
   if(plainMineMajorViewR1302!=null)throw new InvalidOperationException("Plain Mine Major view already owned");
   if(game==null||!game.PlainMineOriginalSpatialBoundR1269||!NativeWorldActive||!nativeRoot)throw new InvalidOperationException("Original Plain Mine world/spatial owner required");
   if(!originalMeshData)throw new InvalidOperationException("Original static Mine mesh resource is missing");var meshBytes=originalMeshData.bytes;
   using(var hash=System.Security.Cryptography.SHA256.Create())if(BitConverter.ToString(hash.ComputeHash(meshBytes)).Replace("-","").ToLowerInvariant()!=PlainMineMajorViewR1302.OriginalMeshDataResourceShaR1302)throw new InvalidOperationException("Original Mine mesh data resource bytes changed");
   var meshDocument=FactoryTypedJsonR581.ReadPlainMineStateMeshR1302(meshBytes);
   if(meshDocument.schema!="plain-mine-original-static-state-mesh-r1302"||meshDocument.meshes==null||meshDocument.meshes.Length!=2)throw new InvalidOperationException("Exact original Mine mesh data schema required");
   var model=game;var scene=nativeScene;var root=nativeRoot;int room=model.room,generation=model.NpcPresentationGeneration;
   var view=new PlainMineMajorViewR1302(nativeCamera,root,originalViewShader,originalTextures,originalSounds,meshDocument.meshes,e=>ReferenceEquals(game,model)&&model.room==room&&model.NpcPresentationGeneration==generation&&model.PlainOriginalSourceBoundR1258&&model.PlainMineOriginalSpatialBoundR1269&&ReferenceEquals(nativeScene,scene)&&ReferenceEquals(nativeRoot,root)&&NativeWorldActive&&model.enemies.Contains(e)&&e.sourceNPC=="NUI_ALTERA_MINE",()=>audioMuted);
   try{view.SetEffectGain(effectsVolume);model.SetPlainMineMajorConsumerR1302(view);plainMineMajorModelR1302=model;plainMineMajorViewR1302=view;}catch(Exception trigger){try{view.Dispose();}catch(Exception cleanup){throw new AggregateException("Mine bind and cleanup failed",trigger,cleanup);}throw;}
  }
  void SyncPlainMineAudioMutedR1302(){if(plainMineMajorViewR1302!=null)plainMineMajorViewR1302.SetMutedR1302(audioMuted);}
  void RenderPlainMineMajorViewR1302(){if(plainMineMajorViewR1302!=null){plainMineMajorViewR1302.SetPausedR1302(plainMineMajorModelR1302.paused);plainMineMajorViewR1302.RenderFrame(plainMineMajorModelR1302.elapsed);}}
  void DisposePlainMineMajorViewR1302(){var view=plainMineMajorViewR1302;var model=plainMineMajorModelR1302;plainMineMajorViewR1302=null;plainMineMajorModelR1302=null;try{if(model!=null&&view!=null)model.UnbindPlainMineMajorConsumerR1302(view);}finally{if(view!=null)view.Dispose();}}
 }
}
