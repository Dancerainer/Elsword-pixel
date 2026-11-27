namespace ElswordPixel {
 public sealed partial class DemoScene {
  string BelowPixelRenderClipV21(Enemy enemy) {
   string selected=BelowPixelStateClipV21.OverrideClip(enemy.sourceNPC,enemy.npcState);if(selected==null)return enemy.clip;
   Library art;bool loaded=libraries.TryGetValue(enemy.kind,out art)&&art.frames.ContainsKey(selected)&&art.clips.ContainsKey(selected)&&!art.clips[selected].loop;
   return BelowPixelStateClipV21.Resolve(enemy.sourceNPC,enemy.npcState,enemy.clip,loaded);
  }
 }
}
