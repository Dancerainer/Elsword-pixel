using System;
using UnityEngine;
namespace ElswordPixel.Editor {
 // Runtime assembly observation only. No Editor type, emitter or gameplay logic.
 public sealed class BelowTailDrawWitnessR304 : MonoBehaviour {
  public SpriteRenderer target;
  public Action<BelowTailDrawWitnessR304,SpriteRenderer,Camera,int> observer;
  void OnWillRenderObject(){var r=target;var camera=Camera.current;var callback=observer;if(r&&camera&&callback!=null)callback(this,r,camera,Time.frameCount);}
  void OnDestroy(){observer=null;target=null;}
 }
}
