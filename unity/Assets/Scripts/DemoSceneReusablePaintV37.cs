using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class PaintSelectionV37 {internal SpriteRenderer carrier;internal Sprite sprite;internal string alias;internal float clock;internal int frame;}
        sealed class PaintOwnerV37 {internal SpriteRenderer carrier;internal ReusablePaintViewV37 view;internal string alias;}
        readonly Dictionary<Enemy,PaintSelectionV37> paintSelectionsV37=new Dictionary<Enemy,PaintSelectionV37>();
        readonly Dictionary<Enemy,PaintOwnerV37> paintOwnersV37=new Dictionary<Enemy,PaintOwnerV37>();
        readonly Dictionary<string,ReusablePaintRigV37> paintBanksV37=new Dictionary<string,ReusablePaintRigV37>(StringComparer.Ordinal);
        readonly HashSet<Enemy> paintFailedV37=new HashSet<Enemy>();
        readonly List<Enemy> paintStaleV37=new List<Enemy>();
        void RecordReusablePaintV37(Enemy e,SpriteRenderer r,string alias,float clock){
            if(!ReusablePaintPinsV37.InScope(e.kind,e.sourceNPC,game.CurrentRoom.id)||!r||!r.sprite)return;
            if(UseElderAttackInkV45(e,r,alias,clock))return;
            if(!paintSelectionsV37.TryGetValue(e,out var sel)){sel=new PaintSelectionV37();paintSelectionsV37.Add(e,sel);}
            sel.carrier=r;sel.sprite=r.sprite;sel.alias=alias;sel.clock=clock;sel.frame=Time.frameCount;
        }
        bool ReusablePaintOwnsV37(Renderer r){foreach(var item in paintOwnersV37.Values)if(item.view.Owns(r))return true;return false;}
        void DropReusablePaintV37(Enemy e){if(paintOwnersV37.TryGetValue(e,out var owner)){owner.view.Dispose();paintOwnersV37.Remove(e);}}
        void DisposeReusablePaintV37(){
            foreach(var owner in paintOwnersV37.Values)owner.view.Dispose();
            paintOwnersV37.Clear();paintBanksV37.Clear();paintSelectionsV37.Clear();paintFailedV37.Clear();paintStaleV37.Clear();
        }
        bool ReusableSourceV37(Enemy e,int i,ReusablePaintRigV37 bank){
            if(catalog==null||catalog.dungeons==null||catalog.stages==null||selectedDungeon<0||selectedDungeon>=catalog.dungeons.Length)return false;
            int s=catalog.dungeons[selectedDungeon].start+game.room;if(s<0||s>=catalog.stages.Length)return false;
            var stage=catalog.stages[s];if(stage.id!=game.CurrentRoom.id||stage.spawns==null||i>=stage.spawns.Length)return false;
            var b=stage.spawns[i];
            return b.kind==e.kind&&b.sourceNPC==e.sourceNPC&&b.x==e.spawnX&&b.encounterIndex==e.encounterIndex&&bank.ExactBirth(e,stage.id,i,b.sourceSpawnIndex);
        }
        bool RegisteredPaintFrameV37(Enemy e,SpriteRenderer r,ReusablePaintRigV37 bank,out ReusablePaintRigV37.Frame frame){
            frame=default;
            if(!paintSelectionsV37.TryGetValue(e,out var selection)||selection.frame!=Time.frameCount||selection.carrier!=r||selection.sprite!=r.sprite||
                !libraries.TryGetValue(e.kind,out var original)||string.IsNullOrEmpty(selection.alias)||!original.clips.TryGetValue(selection.alias,out var clip)||
                !original.frames.TryGetValue(selection.alias,out var sprites)||sprites.Length==0)return false;
            float clock=clip.loop&&clip.duration>0?Mathf.Repeat(selection.clock,clip.duration):selection.clock;
            int idx=Mathf.Clamp(Mathf.FloorToInt(clock*clip.fps),0,sprites.Length-1);
            return sprites[idx]==r.sprite&&bank.Sample(selection.alias,selection.clock,clip.duration,out frame,e);
        }
        void UpdateReusablePaintV37(){
            if(game==null||!actorsRoot||!worldRoot||game.paused&&openMenu=="dungeons"||!ReusablePaintPinsV37.HasRoom(game.CurrentRoom.id)){
                if(paintOwnersV37.Count>0||paintBanksV37.Count>0||paintSelectionsV37.Count>0)DisposeReusablePaintV37();
                return;
            }
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var e=game.enemies[i];var carrier=enemyViews[i];
                if(MonsterPixelOwnsActorV53(e)){DropReusablePaintV37(e);continue;}
                if(!ReusablePaintPinsV37.InScope(e.kind,e.sourceNPC,game.CurrentRoom.id)||paintFailedV37.Contains(e))continue;
                try{
                    if(!paintBanksV37.TryGetValue(e.kind,out var bank)){bank=ReusablePaintRigV37.Load(e.kind);paintBanksV37.Add(e.kind,bank);}
                    if(!ReusableSourceV37(e,i,bank)||!carrier||!RegisteredPaintFrameV37(e,carrier,bank,out var frame)){DropReusablePaintV37(e);continue;}
                    if(paintOwnersV37.TryGetValue(e,out var own)&&own.carrier!=carrier){DropReusablePaintV37(e);own=null;}
                    if(own==null){own=new PaintOwnerV37{carrier=carrier,view=new ReusablePaintViewV37(carrier,bank,DestroyNativeWorldObject)};paintOwnersV37.Add(e,own);}
                    var local=RenderEnemyPosition(e);var world=actorsRoot.TransformPoint(new Vector3(local.x,local.y,0));
                    own.alias=frame.alias;own.view.Apply(frame,NativeWorldActive?ProjectNativePoint(world,e.nativeDepth):world,e.facing);
                    if(NativeWorldActive)BindNativeSceneOcclusionR1461(own.view.painted,world,e.nativeDepth);
                    if(e.hp>0&&enemyBars[i]&&enemyBars[i].enabled){
                        var p=enemyBars[i].transform.position;p.y=own.view.visibleTop+8;enemyBars[i].transform.position=p;
                        p=enemyDebuffViews[i].transform.position;p.y=own.view.visibleTop+20;enemyDebuffViews[i].transform.position=p;
                    }
                }catch(Exception ex){DropReusablePaintV37(e);paintFailedV37.Add(e);Debug.LogWarning("PAINTED_SOURCE_FALLBACK_V37 "+e.sourceNPC+" "+ex.Message);}
            }
            paintStaleV37.Clear();foreach(var e in paintOwnersV37.Keys)if(!game.enemies.Contains(e))paintStaleV37.Add(e);
            foreach(var e in paintStaleV37)DropReusablePaintV37(e);paintStaleV37.Clear();
            foreach(var e in paintFailedV37)if(!game.enemies.Contains(e))paintStaleV37.Add(e);
            foreach(var e in paintStaleV37)paintFailedV37.Remove(e);paintSelectionsV37.Clear();
        }
    }
}
