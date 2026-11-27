using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed class CombatAudioProbe:MonoBehaviour {
  readonly List<float> samples=new List<float>();readonly object gate=new object();bool recording;int rate,channels=2;
  public void Begin(){rate=AudioSettings.outputSampleRate;recording=true;}
  void OnAudioFilterRead(float[] data,int ch){if(!recording)return;lock(gate){channels=ch;if(samples.Count<rate*ch*22)samples.AddRange(data);}}
  public void Save(string file){recording=false;float[] data;lock(gate)data=samples.ToArray();using(var w=new BinaryWriter(File.Create(file))){int bytes=data.Length*2;w.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));w.Write(bytes+36);w.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));w.Write(16);w.Write((short)1);w.Write((short)channels);w.Write(rate);w.Write(rate*channels*2);w.Write((short)(channels*2));w.Write((short)16);w.Write(System.Text.Encoding.ASCII.GetBytes("data"));w.Write(bytes);foreach(float s in data)w.Write((short)(Mathf.Clamp(s,-1,1)*32767));}}
 }
}
