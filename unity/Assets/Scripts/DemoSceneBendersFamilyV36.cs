using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        sealed class BendersWitnessV36 {
            internal SpriteRenderer carrier;
            internal Sprite sprite;
            internal string alias;
            internal float clock;
            internal int frame;
        }
        sealed class BendersOwnerV36 {
            internal SpriteRenderer carrier;
            internal FairyAuthoredPartsV33 parts;
            internal SpriteRenderer[] members;
            internal Transform[] axes;
            internal ArmedActionContinuity transitions;
            internal ThiefTravelView travel;
            internal TreeHumanoidPose humanoid;
            internal TreeBendersFrames frames;
            internal float[] depth;
            internal string alias;
        }
        readonly Dictionary<Enemy,BendersWitnessV36> bendersSelectionsV36=new Dictionary<Enemy,BendersWitnessV36>();
        readonly Dictionary<Enemy,BendersOwnerV36> bendersOwnersV36=new Dictionary<Enemy,BendersOwnerV36>();
        readonly Dictionary<string,BendersFamilyRigV36> bendersBanksV36=new Dictionary<string,BendersFamilyRigV36>(StringComparer.Ordinal);
        readonly List<Enemy> bendersStaleV36=new List<Enemy>();
        readonly HashSet<Enemy> bendersFailedV36=new HashSet<Enemy>();
        readonly HashSet<Renderer> bendersMemberSetV36=new HashSet<Renderer>();
        readonly Plane[] bendersViewPlanesV36=new Plane[6];
        string bendersRoomV36;
        bool smokeBendersVisible,smokeBendersArmored;
        void CaptureTreeBendersSmoke(){
            if(selectedDungeon!=0||!worldCamera||smokeBendersVisible&&smokeBendersArmored)return;
            foreach(var pair in monsterPixelViewsV53){
                var renderer=pair.Value.renderer;if(pair.Key.sourceNPC!="NUI_BENDERS_NORMAL"||!renderer||!renderer.enabled||pair.Key.hp<=0)continue;
                var point=worldCamera.WorldToViewportPoint(renderer.bounds.center);if(point.z<=0||point.x<.15f||point.x>.85f||point.y<.1f||point.y>.9f)continue;
                if(!smokeBendersVisible){smokeBendersVisible=true;Capture("14-benders-visible");}
                if(!smokeBendersArmored&&game.EnemyHasSuperArmor(pair.Key)){smokeBendersArmored=true;Capture("15-benders-armored");}
            }
            foreach(var pair in bendersOwnersV36){
                var view=pair.Value;if(view.frames==null||!view.frames.Renderer.enabled||pair.Key.hp<=0)continue;
                var p=worldCamera.WorldToViewportPoint(view.frames.Renderer.bounds.center);
                if(p.z<=0||p.x<.15f||p.x>.85f||p.y<.1f||p.y>.9f)continue;
                if(!smokeBendersVisible){smokeBendersVisible=true;Capture("14-benders-visible");}
                if(!smokeBendersArmored&&game.EnemyHasSuperArmor(pair.Key)){
                    smokeBendersArmored=true;Capture("15-benders-armored");
                }
            }
        }
        bool BendersBirthV36(Enemy e,int index){
            if(!BendersFamilyPinsV36.Eligible(e.sourceNPC,e.kind,game.CurrentRoom.id,index,selectedDungeon)||
                catalog==null||catalog.stages==null||catalog.dungeons==null||selectedDungeon<0||selectedDungeon>=catalog.dungeons.Length)return false;
            int si=catalog.dungeons[selectedDungeon].start+game.room;
            if(si<0||si>=catalog.stages.Length)return false;
            var stage=catalog.stages[si];
            if(stage.id!=game.CurrentRoom.id||stage.spawns==null||index>=stage.spawns.Length)return false;
            var birth=stage.spawns[index];
            return birth.sourceNPC==e.sourceNPC&&birth.kind==e.kind&&birth.x==e.spawnX&&birth.encounterIndex==e.encounterIndex&&
                birth.sourceSpawnIndex==BendersFamilyPinsV36.NativeSpawn(e.sourceNPC,index,game.CurrentRoom.id)&&!birth.passive&&!birth.dormant;
        }
        void RecordBendersSelectionV36(Enemy e,SpriteRenderer r,string alias,float clock){
            if(!BendersFamilyPinsV36.Known(e.sourceNPC)||!r||!r.sprite)return;
            if(!bendersSelectionsV36.TryGetValue(e,out var w)){w=new BendersWitnessV36();bendersSelectionsV36.Add(e,w);}
            w.carrier=r;w.sprite=r.sprite;w.alias=alias;w.clock=clock;w.frame=Time.frameCount;
        }
        bool BendersOwnsRendererV36(Renderer r){return bendersMemberSetV36.Contains(r);}
        void DropBendersV36(Enemy e){
            if(bendersOwnersV36.TryGetValue(e,out var view)){
                if(view.members!=null)foreach(var member in view.members)bendersMemberSetV36.Remove(member);
                view.parts?.Dispose();view.frames?.Dispose();bendersOwnersV36.Remove(e);
            }
        }
        void DisposeBendersViewsV36(){ReleaseBendersViewsV36(false);}
        void ChangeBendersRoomV36(){ReleaseBendersViewsV36(StreamMobileArt&&adventureStarted&&game!=null&&game.CurrentRoom!=null);}
        void ReleaseBendersViewsV36(bool keepIncoming){
            foreach(var view in bendersOwnersV36.Values){view.parts?.Dispose();view.frames?.Dispose();}
            bendersMemberSetV36.Clear();
            bendersOwnersV36.Clear();bendersSelectionsV36.Clear();bendersFailedV36.Clear();bendersStaleV36.Clear();bendersRoomV36=null;
            if(bendersBanksV36.Count==0)return;
            // The pinned immutable rig belongs to the source NPC, not to a particular enemy.
            // Keep only rigs already required by original births in the incoming room.
            foreach(string npc in new List<string>(bendersBanksV36.Keys)){
                bool needed=false;
                if(keepIncoming)for(int i=0;i<game.enemies.Count;i++){
                    var enemy=game.enemies[i];if(enemy.sourceNPC==npc&&BendersBirthV36(enemy,i)){needed=true;break;}
                }
                if(needed)continue;
                bendersBanksV36[npc].lib.ReleaseOwned(DestroyNativeWorldObject);bendersBanksV36.Remove(npc);
            }
        }
        bool BendersFrameV36(Enemy e,SpriteRenderer carrier,BendersFamilyRigV36 art,out FairyAuthoredPartsV33.Frame f){
            f=default;
            if(!bendersSelectionsV36.TryGetValue(e,out var w)||w.frame!=Time.frameCount||w.carrier!=carrier||w.sprite!=carrier.sprite||
                string.IsNullOrEmpty(w.alias)||!libraries.TryGetValue(e.kind,out var lib)||!lib.frames.TryFrameCount(w.alias,out var count)||
                !lib.clips.TryGetValue(w.alias,out var clip)||count==0)return false;
            float t=clip.loop&&clip.duration>0?Mathf.Repeat(w.clock,clip.duration):w.clock;
            int idx=Mathf.Clamp(Mathf.FloorToInt(t*clip.fps),0,count-1);
            return lib.frames.At(w.alias,idx,out _)==carrier.sprite&&art.Frame(e,w.alias,w.clock,clip.duration,clip.loop,out f);
        }
        bool BendersOutsideViewV36(BendersFamilyRigV36 bank,SpriteRenderer carrier,Vector3 foot,int index){
            if(bank.cullRadius<=0||!string.IsNullOrEmpty(enemyDebuffViews[index].text))return false;
            var transform=carrier.transform.localToWorldMatrix;
            // Bound the world X of every possible original pose under the current carrier scale.
            float radius=bank.cullRadius*Mathf.Sqrt(transform.m00*transform.m00+transform.m01*transform.m01);
            var bar=enemyBars[index];
            for(int i=0;i<2;i++){
                var plane=bendersViewPlanesV36[i];
                // General/custom tilted cameras take the complete display path.
                if(plane.normal.y!=0||plane.normal.z!=0||plane.GetDistanceToPoint(foot)>=-radius)continue;
                if(bar.enabled){var bounds=bar.bounds;if(plane.GetDistanceToPoint(bounds.center)>=-bounds.extents.x)continue;}
                return true;
            }
            return false;
        }
        void UpdateBendersViewsV36(){
            if(game==null||!worldRoot||!actorsRoot||game.paused&&openMenu=="dungeons"||!BendersFamilyPinsV36.HasRoom(game.CurrentRoom.id)){
                if(bendersRoomV36!=null||bendersSelectionsV36.Count>0)DisposeBendersViewsV36();return;
            }
            if(bendersRoomV36!=null&&bendersRoomV36!=game.CurrentRoom.id)DisposeBendersViewsV36();
            bendersRoomV36=game.CurrentRoom.id;
            bool mobileCull=StreamMobileArt&&worldCamera&&worldCamera.orthographic;
            if(mobileCull)GeometryUtility.CalculateFrustumPlanes(worldCamera,bendersViewPlanesV36);
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var e=game.enemies[i];var r=enemyViews[i];
                if(MonsterPixelOwnsActorV53(e)){DropBendersV36(e);continue;}
                if(!BendersBirthV36(e,i)||bendersFailedV36.Contains(e))continue;
                try{
                    if(!bendersBanksV36.TryGetValue(e.sourceNPC,out var bank)){
                        bank=BendersFamilyRigV36.Load(e.sourceNPC,DestroyNativeWorldObject);bendersBanksV36.Add(e.sourceNPC,bank);
                    }
                    if(!r||!r.sprite||!BendersFrameV36(e,r,bank,out var frame)){DropBendersV36(e);continue;}
                    if(bendersOwnersV36.TryGetValue(e,out var view)&&view.carrier!=r){DropBendersV36(e);view=null;}
                    if(view==null){
                        view=new BendersOwnerV36{carrier=r};
                        if(bank.npc=="NUI_BENDERS_NORMAL"){
                            view.frames=new TreeBendersFrames(r,DestroyNativeWorldObject);view.members=new[]{view.frames.Renderer};
                        }else{
                            view.parts=new FairyAuthoredPartsV33(r,bank.lib,DestroyNativeWorldObject);view.members=view.parts.Members;
                            view.depth=new float[bank.count];view.axes=bank.CreateMemberAxes(view.members);
                        }
                        bendersOwnersV36.Add(e,view);
                        foreach(var member in view.members)bendersMemberSetV36.Add(member);
                    }
                    var point=RenderEnemyPosition(e);
                    if(!TreeHumanoidPose.Uses(bank.npc)&&(ThiefLegPose.UsesRig(bank.npc)||bank.npc=="NUI_THIEF_THIN_TOTO")) {
                        if(view.travel==null)view.travel=new ThiefTravelView();
                        point+=view.travel.Prepare(bank,game,point,game.Grounded(e),game.elapsed,ref frame);
                    }
                    var world=actorsRoot.TransformPoint(new Vector3(point.x,point.y,0));
                    view.alias=bendersSelectionsV36[e].alias;
                    var projected=ProjectNativePointWithEye(world,e.nativeDepth,out float eye);
                    if(mobileCull&&BendersOutsideViewV36(bank,r,projected,i)){
                        if(view.frames!=null)view.frames.Suspend();else view.parts.SuspendPresentation(frame);
                        continue;
                    }
                    if(view.frames!=null)view.frames.Apply(frame,projected);
                    else if(TreeHumanoidPose.Uses(bank.npc)) {
                        var standing=new FairyAuthoredPartsV33.Frame{alias="Wait",sourceClock=0,facing=frame.facing,visible=frame.visible};
                        view.parts.Apply(standing,projected);bank.FinishMemberAxes(view.members,view.axes);
                        bank.Depth(standing,view.members,view.depth,view.axes);
                        if(view.humanoid==null)view.humanoid=new TreeHumanoidPose(bank,view.members,view.axes);
                        view.humanoid.Apply(frame,game.elapsed);
                    } else {
                        view.parts.Apply(frame,projected);bank.FinishMemberAxes(view.members,view.axes);
                        bank.Depth(frame,view.members,view.depth,view.axes);
                        view.travel?.AfterPose(bank,view.members,view.axes,point,game.elapsed,frame);
                        if(view.transitions==null)view.transitions=new ArmedActionContinuity(view.members,view.axes);
                        view.transitions.Apply(bank,frame,game.elapsed);
                    }
                    if(NativeWorldActive)foreach(var member in view.members)BindProjectedSceneOcclusion(member,world,e.nativeDepth,eye);
                    if(e.hp>0&&enemyBars[i]&&enemyBars[i].enabled){
                        float top=view.frames!=null?view.frames.Renderer.bounds.max.y:bank.VisibleTop(view.members);
                        if(!float.IsInfinity(top)){
                            var p=enemyBars[i].transform.position;p.y=top+8;enemyBars[i].transform.position=p;
                            p=enemyDebuffViews[i].transform.position;p.y=top+20;enemyDebuffViews[i].transform.position=p;
                        }
                    }
                }catch(Exception ex){DropBendersV36(e);bendersFailedV36.Add(e);Debug.LogWarning("BENDERS_AUTHORED_FALLBACK_V36 "+e.sourceNPC+" "+ex.Message);}
            }
            bendersStaleV36.Clear();
            foreach(var e in bendersOwnersV36.Keys)if(!game.enemies.Contains(e))bendersStaleV36.Add(e);
            foreach(var e in bendersStaleV36)DropBendersV36(e);
            bendersStaleV36.Clear();foreach(var e in bendersFailedV36)if(!game.enemies.Contains(e))bendersStaleV36.Add(e);
            foreach(var e in bendersStaleV36)bendersFailedV36.Remove(e);
            bendersSelectionsV36.Clear();
        }
    }
}
