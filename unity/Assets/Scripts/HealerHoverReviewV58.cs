#if UNITY_EDITOR || ROOT_MONSTER_PIXEL_CHECK_V53
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class GameModel {
        internal bool CheckHealerContactV58(Enemy actor,float relativeCenter,out float low,out float high,out bool floorHit){
            var body=EnemyBounds(actor);
            low=body.bottom-actor.y;high=body.top-actor.y;
            floorHit=EnemyCircleIntersects(actor,actor.x,actor.y,1);
            return EnemyCircleIntersects(actor,actor.x,actor.y+relativeCenter,1);
        }
    }
    public sealed partial class DemoScene {
        [Serializable] sealed class HoverCaptureV58 {
            public string route,room,kind,motion;
            public int floorLine,inkPixels,firstInkPixel,lastInkPixel;
            public float floorPixel,clearancePixels,bodyCenter,hitBottom,hitTop;
            public bool centerHit,floorHit;
        }
        bool CaptureHealerHoverV58(MonsterPixelRouteCheckV53 observer,Enemy actor){
            if(!game.Grounded(actor)||!monsterPixelSelectionsV53.TryGetValue(actor,out var pick)||
               !(pick.motion=="Wait"||pick.motion=="Walk"||pick.motion=="Healing")||
               !TryMonsterPixelRendererV53(actor,out var body))return false;
            var line=game.Surface(actor.groundLine);
            if(line==null)return false;
            int width=MonsterPixelReviewWidthV53,height=MonsterPixelReviewHeightV53;
            var foot=actorsRoot.TransformPoint(new Vector3(actor.x,line.Y(actor.x),0));
            if(NativeWorldActive)foot=ProjectNativePoint(foot,actor.nativeDepth);
            var floorViewport=worldCamera.WorldToViewportPoint(foot);
            if(floorViewport.x<.08f||floorViewport.x>.92f||floorViewport.y<0||floorViewport.y>.88f)return false;
            observer.Watch(actor,body);
            var row=new HoverCaptureV58{route=Dungeon.id,room=game.CurrentRoom.id,kind=actor.kind,motion=pick.motion,
                floorLine=line.id,floorPixel=floorViewport.y*height,firstInkPixel=height};
            var low=worldCamera.WorldToViewportPoint(body.bounds.min);
            var high=worldCamera.WorldToViewportPoint(body.bounds.max);
            int left=Mathf.Clamp(Mathf.FloorToInt(low.x*width)-2,0,width);
            int right=Mathf.Clamp(Mathf.CeilToInt(high.x*width)+2,0,width);
            int bottom=Mathf.Clamp(Mathf.FloorToInt(low.y*height)-2,0,height);
            int top=Mathf.Clamp(Mathf.CeilToInt(high.y*height)+2,0,height);
            Color32[] visible=observer.ReadViewV58(),background;
            try{body.forceRenderingOff=true;background=observer.ReadViewV58();}
            finally{body.forceRenderingOff=false;}
            for(int y=bottom;y<top;y++)for(int x=left;x<right;x++){
                int i=y*width+x;var a=visible[i];var b=background[i];
                if(Math.Abs(a.r-b.r)+Math.Abs(a.g-b.g)+Math.Abs(a.b-b.b)<12)continue;
                row.inkPixels++;row.firstInkPixel=Math.Min(row.firstInkPixel,y);row.lastInkPixel=Math.Max(row.lastInkPixel,y);
            }
            if(row.inkPixels<20)return false;
            row.clearancePixels=row.firstInkPixel-row.floorPixel;
            row.bodyCenter=MonsterPixelLogicalRootV53(pick).y-RenderEnemyPosition(actor).y+
                pick.sprite.bounds.center.y*pick.definition.scale*actor.visualScale;
            row.centerHit=game.CheckHealerContactV58(actor,row.bodyCenter,out row.hitBottom,out row.hitTop,out row.floorHit);
            string stem="hover-"+actor.kind;
            File.WriteAllText(Path.Combine(observer.output,stem+".json"),JsonUtility.ToJson(row,true));
            observer.RenderView();observer.CaptureView(stem);
            if(row.clearancePixels<=2||!row.centerHit||row.floorHit)throw new InvalidOperationException("Hover/sphere registration failed: "+JsonUtility.ToJson(row));
            return true;
        }
        IEnumerator ReviewHealerHoverV58(MonsterPixelRouteCheckV53 observer,MethodInfo enter,MethodInfo begin){
            string kind=Dungeon.id=="bethma-richie-mine"?"nasod-healer":"nasod-healer-blader";
            for(int ri=0;ri<game.RoomCount;ri++){
                enter.Invoke(game,new object[]{ri});
                for(int ei=0;ei<game.EncounterCount;ei++){
                    if(game.EncounterIndex!=ei)begin.Invoke(game,new object[]{ei});
                    var actor=game.enemies.Find(e=>e.kind==kind&&e.active&&!e.dormant&&e.hp>0);
                    if(actor==null)continue;
                    RebuildRoom();observer.BeginRoom(game);
                    double until=Time.realtimeSinceStartupAsDouble+10;
                    while(Time.realtimeSinceStartupAsDouble<until){
                        observer.Budget();game.player.invincible=5;PlaceMonsterPixelReviewPlayerV53(actor,90);
                        yield return null;
                        if(!BossIntroductionActive&&CaptureHealerHoverV58(observer,actor))yield break;
                    }
                }
            }
            throw new InvalidOperationException("No visible grounded "+kind+" available on "+Dungeon.id);
        }
    }
}
#endif
