using System;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  // Single future Plain Host hook: call after BuildNativeWorld for each real
  // Recycle room. No automatic entry, new Host, camera or render call is added.
  void BindPlainMineOriginalWorldR1269(){
   if(game==null||!game.PlainOriginalSourceBoundR1258||!NativeWorldActive||nativeScene.path==null||nativeScene.path.Length==0||nativeScene.unit<=0)
    throw new InvalidOperationException("Original Plain Mine native world is unavailable");
   var model=game;var scene=nativeScene;int room=model.room;
   Action requireCurrent=()=>{if(!ReferenceEquals(game,model)||model.room!=room||!ReferenceEquals(nativeScene,scene)||!NativeWorldActive)
    throw new InvalidOperationException("Original Plain Mine world/room identity changed before spatial rebind");};
   model.SetPlainMineOriginalSpatialR1269(f=>{
    requireCurrent();if(f==null)throw new ArgumentNullException("f");
    var enemy=f as Enemy;if(!ReferenceEquals(f,model.player)&&(enemy==null||!model.enemies.Contains(enemy)))throw new InvalidOperationException("Foreign actor in original Plain Mine spatial sample");
    // Reuse the existing source route station and its real source XZ normal.
    // Enemy depth is source-space depth; player is the original route plane.
    var at=NativePathPoint(f.x)+NativeNormal(f.x)*(enemy==null?0:enemy.nativeDepth);
    at.y=(f.y-scene.offsetY)/scene.unit;
    return new PlainMineSourcePositionR1269(at.x,at.y,at.z);
   },e=>{
    requireCurrent();if(e==null||!model.enemies.Contains(e))throw new InvalidOperationException("Foreign original Mine land sample");
    var surface=model.NativeTerrain?model.Surface(e.groundLine):null;
    if(surface==null&&model.NativeTerrain)surface=model.SurfaceBelow(e.x,e.y+.05f);
    if(model.NativeTerrain&&surface==null)throw new InvalidOperationException("Original Mine has no supporting source surface");
    float y=surface==null?model.GroundAt(e.x):surface.Y(e.x);
    return(y-scene.offsetY)/scene.unit;
   });
  }
 }
}
