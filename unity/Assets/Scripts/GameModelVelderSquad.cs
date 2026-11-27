using System;

namespace ElswordPixel {
    internal sealed partial class RegionActorState {
        internal Enemy rider, dismountedFrom;
        internal bool cannonBack;
        internal bool formationActive;
        internal float formationX,formationY;
    }

    public sealed partial class GameModel {
        static bool IsRegionCannon(Enemy e) {return e!=null&&e.kind=="cannon_demon";}
        static bool IsRegionHitFixture(Enemy e) {return IsRegionMirror(e)||IsRegionCannon(e)||e!=null&&e.kind=="ice_pillar_trap";}

        static bool ActiveSquadMember(Enemy member,Enemy relative) {
            return member!=null&&member.active&&!member.dormant&&member.hp>0&&
                member.encounterIndex==relative.encounterIndex&&member.regionalAlly==relative.regionalAlly;
        }

        Enemy SquadMember(Enemy e,string kind,RegionPoint center,float range,bool previouslyConsulted) {
            Enemy found=null;float best=range*range;
            foreach(var other in enemies){
                if(other==e||other.sourceNPC!=kind||!ActiveSquadMember(other,e))continue;
                var a=other.regionActor;
                if(previouslyConsulted&&(a==null||a.commander!=e))continue;
                if(!previouslyConsulted&&a!=null&&a.commander!=e&&ActiveSquadMember(a.commander,e))continue;
                float dx=other.x/.28f-center.x,dy=other.y/.28f-center.y,d2=dx*dx+dy*dy;
                if(d2<=best){best=d2;found=other;}
            }
            return found;
        }

        static string VelderMountKind(string rider) {
            if(rider=="gliter_lance_great_mount")return "coackatrigle";
            if(rider=="gliter_lance_great_boss")return "coackatrigle_boss";
            return null;
        }

        void UnseatVelderRider(Enemy member) {
            if(member?.regionActor==null)return;
            var rider=member.regionActor.rider??member;
            var a=rider.regionActor;
            var mount=a?.mount;
            if(mount==null||VelderMountKind(rider.kind)!=mount.kind)return;
            a.mount=null;a.dismountedFrom=mount;
            if(mount.regionActor!=null)mount.regionActor.rider=null;
            if(rider.hp>0)a.request=a.spec.wait;
        }

        void TickVelderRider(Enemy e,RegionActorState a,float dt) {
            string targetKind=VelderMountKind(e.kind);
            if(targetKind==null||a.mount!=null||!e.aiEnabled||e.passive||!Grounded(e))return;
            if(a.dismountedFrom!=null){
                if(ActiveSquadMember(a.dismountedFrom,e)&&Math.Abs(e.x-a.dismountedFrom.x)<64)return;
                a.dismountedFrom=null;
            }
            bool canApproach=a.node.id==a.spec.wait||a.node.id.EndsWith("_WALK",StringComparison.Ordinal);
            if(!canApproach)return;
            Enemy nearest=null;float distance=Single.PositiveInfinity;
            foreach(var candidate in enemies){
                if(candidate.kind!=targetKind||!ActiveSquadMember(candidate,e)||candidate.regionActor==null||
                    candidate.regionActor.rider!=null||!Grounded(candidate))continue;
                var body=EnemyBounds(candidate);
                if(e.y<body.bottom-24||e.y>body.top)continue;
                float dx=Math.Abs(e.x-candidate.x);
                if(dx<distance){nearest=candidate;distance=dx;}
            }
            if(nearest==null)return;
            var target=EnemyBounds(nearest);var self=EnemyBounds(e);
            if(self.right<target.left||self.left>target.right){
                e.facing=nearest.x>=e.x?1:-1;
                MoveAlongGround(e,e.x+e.facing*Math.Min(distance,a.spec.walk*dt));
                return;
            }
            a.mount=nearest;nearest.regionActor.rider=e;
            a.request=e.kind=="gliter_lance_great_boss"?"GLITER_LANCE_GREAT_BOSS_RIDE_MOUNT":"GLITER_LANCE_GREAT_RIDE_MOUNT";
        }

        void ApplyVelderSquadImpact(Enemy e,HitReaction reaction,int raw) {
            var a=e.regionActor;
            if(a==null)return;
            if(e.hp<=0||raw==3||raw==4||raw==5||raw==16||reaction==HitReaction.Knockdown||reaction==HitReaction.Launch)
                UnseatVelderRider(e);
            if(e.kind!="cannon_demon"||e.hp<=0)return;
            bool flip=raw==4||raw==5||raw==0&&reaction==HitReaction.Launch;
            string state=flip?(a.cannonBack?"CANNON_DEMON_TURN_FROM_BACK_TO_FRONT":"CANNON_DEMON_TURN_FROM_FRONT_TO_BACK"):
                (a.cannonBack?"CANNON_DEMON_DAMAGE_BACK_SIDE":"CANNON_DEMON_DAMAGE_FRONT_SIDE");
            RegionMove next;if(!a.spec.nodes.TryGetValue(state,out next))return;
            e.stun=e.vx=e.vy=e.downTime=e.recoveryTime=0;e.state=CombatState.Normal;e.knockdownOnLanding=false;
            a.suspended=false;
            EnterRegionNode(e,a,next);
        }

        bool TryVelderSquad(Enemy e,RegionActorState a,string name,object owner,object p,object q,object third,out object result) {
            result=null;
            switch(name){
                case "GetFixedDistancePosition_LUA":
                    if(!(p is RegionPoint))return false;
                    result=RegionGroundPoint((RegionPoint)p,RegionNumber(q),RegionTruth(third));return true;
                case "COMMAND_FORCED_MOVE_GROUND_LUA":
                    if(!(p is RegionPoint))return false;
                    var goal=(RegionPoint)p;
                    if(!Finite(goal.x)||!Finite(goal.y))return true;
                    a.formationActive=true;a.formationX=goal.x*.28f;a.formationY=goal.y*.28f;
                    a.command="GAI_CMD_FORCED_MOVE";a.selfCommand=false;return true;
                case "COMMAND_ATTACK_MOVE_LUA":case "COMMAND_SKILL_LUA":case "COMMAND_FORCED_MOVE_TARGET_LUA":
                    a.formationActive=false;return false;
                case "SetGroupAICommand_LUA":
                    if(RegionSymbol(p)!="GAI_CMD_FORCED_MOVE")a.formationActive=false;
                    return false;
                case "IsNearNPCUnitUID_LUA":
                case "GetConsultNearNPC_LUA":
                    if(!(p is RegionPoint))return false;
                    var other=SquadMember(e,RegionSymbol(q),(RegionPoint)p,RegionNumber(third),false);
                    result=name=="IsNearNPCUnitUID_LUA"?(object)(other!=null):other;
                    return true;
                case "GetConsultNpc":
                    if(e.kind!="gliter_commander")return false;
                    goto case "GetOnlyConsultNPC_LUA";
                case "GetOnlyConsultNPC_LUA":
                    result=SquadMember(e,RegionSymbol(p),RegionPosition(e),Single.PositiveInfinity,true);
                    return true;
                case "GetOnlyConsultedNPC_LUA":
                    result=ActiveSquadMember(a.commander,e)&&a.commander.sourceNPC==RegionSymbol(p)?a.commander:null;
                    return true;
                case "GetRideUnit_LUA":
                    var member=owner as Enemy??e;
                    result=member.regionActor==null?null:member.regionActor.mount??member.regionActor.rider;
                    return true;
                case "RideOff_LUA":UnseatVelderRider(owner as Enemy??e);return true;
                case "ChangeDamageStateID_LUA":case "ChangeDyingStateID_LUA":
                    if(e.kind=="cannon_demon"){
                        string next=RegionSymbol(q);
                        a.cannonBack=next.EndsWith("_BACK_SIDE",StringComparison.Ordinal)||
                            next.EndsWith("_BACK_TO_FRONT",StringComparison.Ordinal);
                    }
                    return true;
                default:return false;
            }
        }
    }
}
