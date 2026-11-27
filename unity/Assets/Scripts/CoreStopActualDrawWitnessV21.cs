using UnityEngine;
namespace ElswordPixel {
 // Unity's per-renderer callback records view evidence only. It never calls a
 // model API or modifies transforms during culling/rendering.
 public sealed class CoreStopActualDrawWitnessV21:MonoBehaviour {
#if UNITY_EDITOR
  CoreStopIntroViewBridgeDraftV21 owner;CoreStopIntroTicketV21 ticket;Renderer body;DemoScene factoryOwnerR977;Enemy factoryActorR977;
#else
  CoreStopIntroViewBridgeDraftV21 owner;CoreStopIntroTicketV21 ticket;Renderer body;
#endif
#if UNITY_EDITOR
  internal void Bind(CoreStopIntroViewBridgeDraftV21 viewOwner,CoreStopIntroTicketV21 t,Renderer r){owner=viewOwner;ticket=t;body=r;factoryOwnerR977=null;factoryActorR977=null;}
#else
  internal void Bind(CoreStopIntroViewBridgeDraftV21 viewOwner,CoreStopIntroTicketV21 t,Renderer r){owner=viewOwner;ticket=t;body=r;}
#endif
#if UNITY_EDITOR
   internal void BindFactoryR977(DemoScene viewOwner,Enemy actor,SpriteRenderer r){owner=null;ticket=null;factoryOwnerR977=viewOwner;factoryActorR977=actor;body=r;}
#endif
#if UNITY_EDITOR
  void OnWillRenderObject(){if(owner!=null&&body)owner.ObserveWorldDrawOnlyV21(ticket,body,Camera.current,Time.frameCount);if(factoryOwnerR977&&body)factoryOwnerR977.ObserveFactoryNpcWorldDrawR977(factoryActorR977,(SpriteRenderer)body,Camera.current,Time.frameCount);}
#else
  void OnWillRenderObject(){if(owner!=null&&body)owner.ObserveWorldDrawOnlyV21(ticket,body,Camera.current,Time.frameCount);}
#endif
 }
}

