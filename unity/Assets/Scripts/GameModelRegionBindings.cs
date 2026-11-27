using System;
using System.Collections.Generic;

namespace ElswordPixel
{
    public sealed partial class GameModel
    {
        readonly Dictionary<Enemy,int> regionUnitIds=new Dictionary<Enemy,int>();
        public bool RegionShakeRequired {get{return enemies.Exists(e=>e.hp>0&&e.active&&!e.dormant&&e.regionActor!=null&&e.regionActor.stick>0);}}
        int regionRawDirection;
        static float RegionalAcceleration(Fighter fighter,float fallback,bool gravity)
        {
            var enemy=fighter as Enemy;var a=enemy==null?null:enemy.regionActor;
            if(a==null)return fallback;
            float value=gravity?a.gravityOverride:a.reloadOverride;
            return Finite(value)?value:fallback;
        }
        RoomDefinition regionLineRoom;
        GroundSurface[] regionOriginalLines;

        float RegionUnitId(Enemy e)
        {
            int id;
            if(!regionUnitIds.TryGetValue(e,out id))regionUnitIds[e]=id=regionUnitIds.Count+1;
            return id;
        }
        void ResetRegionalBindings()
        {
            if(regionLineRoom!=null)regionLineRoom.surfaces=regionOriginalLines;
            regionLineRoom=null;regionOriginalLines=null;regionUnitIds.Clear();
        }
        RegionPoint RegionLand(RegionPoint point)
        {
            if(!Finite(point.x)||!Finite(point.y))return point;
            float x=Clamp(point.x*.28f,8,RoomWidth-8);
            var line=NativeTerrain?SurfaceBelow(x,point.y*.28f+1):null;
            return new RegionPoint(x/.28f,(line!=null?line.Y(x):NativeTerrain?point.y*.28f:GroundAt(CurrentRoom,x))/.28f,point.z);
        }
        RegionPoint RegionLineEnd(Fighter fighter,bool right)
        {
            var line=Surface(fighter.groundLine)??SurfaceBelow(fighter.x,fighter.y+.1f);
            if(line==null)return RegionLand(new RegionPoint((right?RoomWidth-8:8)/.28f,fighter.y/.28f));
            // Continue through connected endpoints, without crossing between stacked floors.
            for(int i=0;i<CurrentRoom.surfaces.Length;i++){
                float x=right?line.x2:line.x1,y=right?line.y2:line.y1;
                GroundSurface next=null;
                foreach(var s in CurrentRoom.surfaces){
                    if(s==line||s.hamelDisabled||s.npcOnly!=line.npcOnly||s.platform!=line.platform)continue;
                    float join=right?s.x1:s.x2, height=right?s.y1:s.y2;
                    if(Math.Abs(join-x)<.1f&&Math.Abs(height-y)<.4f&&(right?s.x2>x+.1f:s.x1<x-.1f)){next=s;break;}
                }
                if(next==null)break;
                line=next;
            }
            return new RegionPoint((right?line.x2:line.x1)/.28f,(right?line.y2:line.y1)/.28f);
        }
        void RegionalLineEndpoints(int index,RegionPoint p,RegionPoint q)
        {
            var lines=CurrentRoom.surfaces;
            if(index<0||index>=lines.Length||regionLineRoom!=CurrentRoom||index<regionOriginalLines.Length)return;
            if(q.x<p.x){var temp=p;p=q;q=temp;}
            var s=lines[index];
            if(s.hamelDisabled||s.x1!=p.x*.28f||s.x2!=q.x*.28f||s.y1!=p.y*.28f||s.y2!=q.y*.28f)nativeNavigationDestination=-1;
            s.x1=p.x*.28f;s.y1=p.y*.28f;s.x2=q.x*.28f;s.y2=q.y*.28f;
            s.hamelDisabled=s.x2-s.x1<.01f;
        }
        bool TryRegionalBindings(string name,Enemy e,Fighter subject,RegionActorState a,object p,object q,object third,object fourth,out object value)
        {
            value=null;
            int key=RegionId(p);
            switch(name){
                case "GetRandomPositionDir":
                    if(p is RegionPoint&&Finite(RegionNumber(q))){
                        var from=(RegionPoint)p;
                        float desired=from.x+(RegionTruth(fourth)?1:-1)*SkillRandom()*Math.Abs(RegionNumber(q));
                        float x=Clamp(desired*.28f,8,RoomWidth-8);
                        // If this choice crosses a gap, stop on the nearest eligible landing in that direction.
                        if(NativeTerrain&&SurfaceBelow(x,from.y*.28f+1)==null){
                            float nearest=Single.PositiveInfinity;
                            foreach(var line in CurrentRoom.surfaces){
                                if(line.hamelDisabled||line.npcOnly)continue;
                                float at=Clamp(x,line.x1,line.x2);
                                if((at-from.x*.28f)*(desired-from.x)<0||line.Y(at)>from.y*.28f+1)continue;
                                float d=Math.Abs(x-at);if(d<nearest){nearest=d;desired=at/.28f;}
                            }
                            if(Single.IsPositiveInfinity(nearest))desired=from.x;
                        }
                        value=RegionLand(new RegionPoint(desired,from.y,from.z));
                    }
                    return true;
                case "GetLineGroupEndPos":case "GetLineGroupStartPos":value=RegionLineEnd(subject,name=="GetLineGroupEndPos");return true;
                case "GetNPCUnitListSize":value=(float)enemies.Count;return true;
                case "GetNPCUnit":value=key>=0&&key<enemies.Count?enemies[key]:null;return true;
                case "IsUnitID":value=subject is Enemy&&((Enemy)subject).sourceNPC==RegionSymbol(p);return true;
                case "SetUserHold":a.holdBone=RegionTruth(p)?RegionSymbol(q):null;a.holdingPlayer=false;return true;
                case "SetUserHoldReset":a.holdBone=null;a.holdingPlayer=false;return true;
                case "SetRideToUnit_LUA":a.mount=p as Enemy;return true;
                case "SetAItoMainAIData_LUA":a.alternateAi=false;return true;
                case "SetAItoSubAIData_LUA":a.alternateAi=a.spec.subTargetRange>0;a.mount=null;return true;
                case "SetSuperArmor":a.scriptedArmor=RegionTruth(p);return true;
                case "SetDefaultAnimSpeed_LUA":
                    if(RegionNumber(p)>0){a.defaultSpeed=RegionNumber(p);a.animSpeed=a.node.speed*a.defaultSpeed;}return true;
                case "SetEnableAllKeyProcess":a.capturesInput=!RegionTruth(p);return true;
                case "SetLockOnTarget":case "SetLockOnNearTarget":
                    if(p is RegionSpellInstance)((RegionSpellInstance)p).tracking=RegionTarget(e);return true;
                case "SetLockOnPos":
                    if(p is RegionSpellInstance&&q is RegionPoint){
                        var shot=(RegionSpellInstance)p;var point=(RegionPoint)q;
                        shot.trackingPoint=true;shot.targetX=point.x*.28f;shot.targetY=point.y*.28f;
                    }
                    return true;
                case "SetDrainHPSeq":case "SetDieSeq":
                    // These setters own the emitted particle's handle. DrainHP itself runs separately on the incoming hit.
                    a.visuals[name]=p;return true;
                case "SetExtraDamageDataTime":
                    if(RegionNumber(p)>=0){a.extraDamageUntil=a.age+RegionNumber(p);a.extraDamageContact=0;}
                    return true;
                case "GetNumLineData_LUA":value=(float)CurrentRoom.surfaces.Length;return true;
                case "AddLine_LUA":{
                    var previous=CurrentRoom.surfaces;
                    if(previous.Length==0)return true;
                    if(regionLineRoom!=CurrentRoom){regionLineRoom=CurrentRoom;regionOriginalLines=previous;}
                    var next=new GroundSurface[previous.Length+1];Array.Copy(previous,next,previous.Length);
                    int id=0;foreach(var s in previous)id=Math.Max(id,s.id+1);
                    next[previous.Length]=new GroundSurface{id=id,platform=true,hamelDisabled=true};
                    CurrentRoom.surfaces=next;return true;
                }
                case "InsertMonsterLineMapIndex_LUA":if(key>=0)a.lines.Add(key);return true;
                case "GetNumOfMonsterLineMap_LUA":value=(float)a.lines.Count;return true;
                case "GetMonsterLineMapIndex_LUA":value=key>=0&&key<a.lines.Count?(float)a.lines[key]:-1f;return true;
                case "SetStartEndPosAtThisLineMap_LUA":if(q is RegionPoint&&third is RegionPoint)RegionalLineEndpoints(key,(RegionPoint)q,(RegionPoint)third);return true;
                case "EnableLineData_LUA":
                    if(regionLineRoom==CurrentRoom&&key>=regionOriginalLines.Length&&key<CurrentRoom.surfaces.Length&&CurrentRoom.surfaces[key].hamelDisabled==RegionTruth(q)){
                        CurrentRoom.surfaces[key].hamelDisabled=!RegionTruth(q);nativeNavigationDestination=-1;
                    }
                    return true;
                default:return false;
            }
        }
        void TrackRegionalBindings(Enemy e,RegionActorState a,float dt)
        {
            if(a.stick>0){
                if(player.hp<=0)a.stick=0;
                else{
                    if(regionRawDirection!=0&&a.stickDirection!=regionRawDirection){a.stick--;a.stickDirection=regionRawDirection;}
                    e.x=player.x;e.y=player.y;e.facing=player.facing;e.groundLine=player.groundLine;e.vx=e.vy=0;
                }
            }
            if(a.capturesInput){
                // A hit or direct transition can interrupt a scripted intro before its
                // release frame. The combat states never take ownership of the key lock.
                a.capturesInput=a.node.id.StartsWith("VICTOR_WAITSTART",StringComparison.Ordinal)||
                    a.node.id.StartsWith("VICTOR_LAST_WAITSTART",StringComparison.Ordinal)||a.node.id=="RAN_WAIT_START";
                if(a.capturesInput)regionInputUntil=Math.Max(regionInputUntil,elapsed+2*dt);
            }
            if(a.mount!=null){
                if(!a.mount.active||a.mount.hp<=0)a.mount=null;
                else{e.x=a.mount.x;e.y=a.mount.y;e.vx=e.vy=0;e.groundLine=a.mount.groundLine;}
            }
            if(!a.holdingPlayer||a.holdBone==null)return;
            var grip=RegionBonePoint(e,a.holdBone);
            player.x=ConstrainEncounterX(player,grip.x);player.y=grip.y;
            player.vx=player.vy=0;player.groundLine=-1;
            regionInputUntil=Math.Max(regionInputUntil,elapsed+2*dt);
        }
    }
}
