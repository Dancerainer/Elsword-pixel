#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class SourceInputTraceV17 {public float time,dt;public int room;public Input input;}
  readonly List<string> sourceInputTraceV17=new List<string>();
  readonly List<string> sourceMovementTraceV17=new List<string>();
  void BeginSourceInputTraceV17(){
   sourceInputTraceV17.Clear();sourceMovementTraceV17.Clear();
   if(!dragonRoadPreviewSession)return;
   game.NativeMovementTraceV17=sample=>{if(sample.isPlayer&&sample.room==1&&Math.Max(sample.oldX,sample.x)>2550&&Math.Min(sample.oldX,sample.x)<2700&&sourceMovementTraceV17.Count<20000)sourceMovementTraceV17.Add(JsonUtility.ToJson(sample));};
  }
  void RecordSourceInputTraceV17(Input input,float dt){
   if(!forestAutoRunning||!(dragonRoadPreviewSession||lakeNightPreviewSession||dragonNestPreviewSession)||sourceInputTraceV17.Count>=24000)return;
   sourceInputTraceV17.Add(JsonUtility.ToJson(new SourceInputTraceV17{time=game.elapsed,dt=dt,room=game.room,input=input}));
  }
  void SaveSourceInputTraceV17(){
   if(!(dragonRoadPreviewSession||lakeNightPreviewSession||dragonNestPreviewSession))return;
   game.NativeMovementTraceV17=null;
   File.WriteAllLines(Path.Combine(forestAutoOutput,"actual-inputs.jsonl"),sourceInputTraceV17);
   File.WriteAllLines(Path.Combine(forestAutoOutput,"actual-movement.jsonl"),sourceMovementTraceV17);
  }
 }
}
#endif
