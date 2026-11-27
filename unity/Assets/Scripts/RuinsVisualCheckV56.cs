#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ElswordPixel {
    public sealed class RuinsVisualCheckV56:MonoBehaviour {
        [Serializable] internal sealed class GateRow {public string room,npc,kind,sprite,image;public float playerX,gateX,clip;public int visiblePixels,unclippedPixels;public bool clear,required,lockedProtected,lockedRevealed,unlockedDamageable;}
        [Serializable] internal sealed class WilliamRow {public string motion,sprite,image;public int facing;public float clock;}
        [Serializable] internal sealed class Report {public string error="",schema="ruins-gate-william-actual-scene-v56";public bool passed,requireReveal;public List<GateRow> gates=new List<GateRow>();public List<WilliamRow> william=new List<WilliamRow>();}
        internal Report report=new Report();internal string output;DemoScene scene;RenderTexture target;Texture2D readback;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        static void Attach(){var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--ruins-visual-review-v56");if(i<0)return;var r=new GameObject("Ruins visual check V56").AddComponent<RuinsVisualCheckV56>();r.output=Path.GetFullPath(args[i+1]);Directory.CreateDirectory(r.output);r.report.requireReveal=Array.IndexOf(args,"--ruins-require-reveal-v56")>=0;Application.runInBackground=true;}
        IEnumerator Start(){
            while(!(scene=UnityEngine.Object.FindFirstObjectByType<DemoScene>()))yield return null;
            var work=scene.ReviewRuinsVisualV56(this);Exception error=null;while(true){bool more=false;object value=null;try{more=work.MoveNext();if(more)value=work.Current;}catch(Exception e){error=e;}if(error!=null||!more)break;yield return value;}
            report.error=error==null?"":error.ToString();report.passed=error==null&&report.gates.Count==2&&report.william.Count==2;
            File.WriteAllText(Path.Combine(output,"visual-check.json"),JsonUtility.ToJson(report,true));if(target){target.Release();Destroy(target);}if(readback)Destroy(readback);UnityEditor.EditorApplication.Exit(report.passed?0:1);
        }
        internal Color32[] Render(Camera camera,string name){
            if(!target){target=new RenderTexture(960,540,24){filterMode=FilterMode.Point};target.Create();readback=new Texture2D(960,540,TextureFormat.RGBA32,false);}
            var saved=camera.targetTexture;camera.targetTexture=target;try{camera.Render();}finally{camera.targetTexture=saved;}
            var prior=RenderTexture.active;RenderTexture.active=target;readback.ReadPixels(new Rect(0,0,960,540),0,0);readback.Apply();RenderTexture.active=prior;if(name!=null)File.WriteAllBytes(Path.Combine(output,name+".png"),readback.EncodeToPNG());return readback.GetPixels32();
        }
        internal static int Difference(Color32[] a,Color32[] b){int n=0;for(int i=0;i<a.Length;i++)if(!a[i].Equals(b[i]))n++;return n;}
    }
    public sealed partial class DemoScene {
        public static void StartRuinsVisualReviewV56(){if(Array.IndexOf(Environment.GetCommandLineArgs(),"--ruins-visual-review-v56")<0)throw new ArgumentException("Explicit output required");UnityEditor.SceneManagement.EditorSceneManager.OpenScene(UnityEditor.EditorBuildSettings.scenes[0].path);UnityEditor.EditorApplication.EnterPlaymode();}
        internal IEnumerator ReviewRuinsVisualV56(RuinsVisualCheckV56 check){
            yield return null;Action("dungeons");yield return null;int map=mapDungeons.FindIndex(d=>d.id=="ruins");MenuAction("region:"+mapDungeons[map].region);RefreshMapVisible();int slot=mapVisible.IndexOf(map);mapPage=slot/MapPageSize;MenuAction("dungeon-card:"+(slot%MapPageSize));MenuAction("dungeon-enter");
            double deadline=Time.realtimeSinceStartupAsDouble+120;while(!adventureStarted||openMenu!=""||Dungeon.id!="ruins"||forestPreviewLoading){if(Time.realtimeSinceStartupAsDouble>deadline)throw new TimeoutException("Ruins entry");yield return null;}
            var enter=typeof(GameModel).GetMethod("EnterRoom",BindingFlags.NonPublic|BindingFlags.Instance);
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"--guardian-wake-v58")>=0)
                foreach(var step in ReviewGuardianWakeV58(check, enter)) yield return step;
            for(int room=0;room<2;room++){
                enter.Invoke(game,new object[]{room});RebuildRoom();yield return null;yield return null;
                Enemy gate=game.enemies.Find(e=>e.isGate&&e.active&&!e.dormant);if(gate==null)throw new InvalidOperationException("Missing original exit gate "+room);
                PlaceMonsterPixelReviewPlayerV53(gate,-60);double approach=Time.realtimeSinceStartupAsDouble+.8;while(Time.realtimeSinceStartupAsDouble<approach){game.player.invincible=120;yield return null;}
                float lockedHp=gate.hp;bool protectedGate=game.ExitGateProtected(gate)&&game.ApplyHitToEnemy(gate,1)==0&&gate.hp==lockedHp;
                var lockedProperties=new MaterialPropertyBlock();enemyViews[game.enemies.IndexOf(gate)].GetPropertyBlock(lockedProperties);bool lockedRevealed=lockedProperties.GetFloat("_NativeSceneClipR1461")==0;
                double end=Time.realtimeSinceStartupAsDouble+45;
                while(!game.RoomCombatClear){
                    game.player.invincible=120;Enemy focus=null;
                    foreach(var e in new List<Enemy>(game.enemies))if(!e.isGate&&!e.passive&&e.hp>0&&e.countsForClear){focus=e;break;}
                    if(focus!=null){PlaceMonsterPixelReviewPlayerV53(focus,-45);game.ApplyHitToEnemy(focus,100000);}
                    if(Time.realtimeSinceStartupAsDouble>end){var pending=new List<string>();foreach(var e in game.enemies)if(e.hp>0&&!e.isGate&&!e.passive&&e.countsForClear)pending.Add(e.sourceNPC+"/"+e.npcState+"/active="+e.active+"/dormant="+e.dormant);throw new TimeoutException("Public hits could not clear Ruins room "+room+": "+string.Join(",",pending));}yield return null;
                }
                PlaceMonsterPixelReviewPlayerV53(gate,-60);end=Time.realtimeSinceStartupAsDouble+1;while(Time.realtimeSinceStartupAsDouble<end){game.player.invincible=120;yield return null;}
                var body=enemyViews[game.enemies.IndexOf(gate)];var original=new MaterialPropertyBlock();body.GetPropertyBlock(original);var normal=check.Render(worldCamera,"gate-room"+room+"-normal");body.enabled=false;var absent=check.Render(worldCamera,null);body.enabled=true;var reveal=new MaterialPropertyBlock();body.GetPropertyBlock(reveal);reveal.SetFloat("_NativeSceneClipR1461",0);body.SetPropertyBlock(reveal);var unclipped=check.Render(worldCamera,"gate-room"+room+"-unclipped-reference");body.SetPropertyBlock(original);
                float openHp=gate.hp;bool damageable=game.ApplyHitToEnemy(gate,1)>0&&gate.hp<openHp;
                var row=new RuinsVisualCheckV56.GateRow{room=game.CurrentRoom.id,npc=gate.sourceNPC,kind=gate.kind,sprite=body.sprite.name,image="gate-room"+room+"-normal.png",playerX=game.player.x,gateX=gate.x,clip=original.GetFloat("_NativeSceneClipR1461"),visiblePixels=RuinsVisualCheckV56.Difference(normal,absent),unclippedPixels=RuinsVisualCheckV56.Difference(unclipped,absent),clear=game.RoomCombatClear,required=RequiredExitGate()==gate,lockedProtected=protectedGate,lockedRevealed=lockedRevealed,unlockedDamageable=damageable};check.report.gates.Add(row);
                if(check.report.requireReveal&&(!row.required||row.visiblePixels!=row.unclippedPixels||row.visiblePixels==0))throw new InvalidOperationException("Required nearby gate remains occluded: "+room);
                if(check.report.requireReveal&&(!protectedGate||!lockedRevealed||!damageable))throw new InvalidOperationException("Gate lock/reveal contract changed: "+room);
            }
            enter.Invoke(game,new object[]{2});RebuildRoom();yield return null;yield return null;Enemy william=game.enemies.Find(e=>e.kind=="william");if(william==null)throw new InvalidOperationException("Original William absent");
            for(int side=-1;side<=1;side+=2){
                PlaceMonsterPixelReviewPlayerV53(william,side*130);double end=Time.realtimeSinceStartupAsDouble+20;bool captured=false;
                while(Time.realtimeSinceStartupAsDouble<end){game.player.invincible=120;yield return null;if(BossIntroductionActive)continue;PlaceMonsterPixelReviewPlayerV53(william,side*130);if(william.facing==side&&monsterPixelSelectionsV53.TryGetValue(william,out var selected)&&selected.motion=="Wait"&&monsterPixelViewsV53.TryGetValue(william,out var view)){
                    check.Render(worldCamera,"william-side"+side);check.report.william.Add(new RuinsVisualCheckV56.WilliamRow{motion=selected.motion,sprite=view.renderer.sprite.name,clock=selected.clock,facing=william.facing,image="william-side"+side+".png"});captured=true;break;
                }}if(!captured)throw new TimeoutException("William standing view side "+side);
            }
        }
    }
}
#endif
