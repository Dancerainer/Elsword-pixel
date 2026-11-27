using System;

namespace ElswordPixel {
    internal sealed partial class RegionActorState {
        internal string mirrorReaction;
        internal RegionMove chargerGrabState;
        internal float chargerGripX, chargerGripY;
        internal bool chargerHolding;
    }

    public sealed partial class RegionSpellDefinition {
        internal RegionSpellDefinition Parabolic(float gravityX,float gravityY,float duration) {
            var launch=(RegionSpellDefinition)MemberwiseClone();
            launch.gx=gravityX;
            launch.gy=gravityY;
            launch.dirSpeed=0;
            launch.life=duration;
            return launch;
        }
    }

    public sealed partial class GameModel {
        static bool IsRegionMirror(Enemy e) {
            return e!=null&&(e.kind=="crystal_trap_left"||e.kind=="crystal_trap_right");
        }

        bool RegionGroundSpan(GroundSurface s,out float left,out float right) {
            var bounds=CurrentEncounter;
            left=bounds==null?s.x1:Math.Max(s.x1,bounds.leftX);
            right=bounds==null?s.x2:Math.Min(s.x2,bounds.rightX);
            return right>left&&!s.hamelDisabled&&!s.npcOnly&&s.bungeeTargetPlusOne==0;
        }

        // GetNearest and GetFixedDistancePosition both address the active line map.
        RegionPoint RegionGroundPoint(RegionPoint point,float distance=0,bool rightward=true) {
            var missing=new RegionPoint(Single.NaN,Single.NaN);
            if(!NativeTerrain||!Finite(point.x)||!Finite(point.y)||!Finite(distance))return missing;
            float px=point.x*.28f,py=point.y*.28f,x=0,best=Single.PositiveInfinity;
            GroundSurface line=null;
            foreach(var candidate in CurrentRoom.surfaces){
                float left,right;if(!RegionGroundSpan(candidate,out left,out right))continue;
                float slope=(candidate.y2-candidate.y1)/(candidate.x2-candidate.x1);
                float at=Clamp(left+((px-left)+(py-candidate.Y(left))*slope)/(1+slope*slope),left,right);
                float dx=at-px,dy=candidate.Y(at)-py,d2=dx*dx+dy*dy;
                if(d2>=best)continue;
                best=d2;line=candidate;x=at;
            }
            if(line==null)return missing;
            float remaining=Math.Abs(distance)*.28f;
            int direction=rightward==(distance>=0)?1:-1;
            for(int i=0;remaining>0&&i<CurrentRoom.surfaces.Length;i++){
                float left,right;RegionGroundSpan(line,out left,out right);
                float edge=direction>0?right:left,slope=(line.y2-line.y1)/(line.x2-line.x1);
                float stride=(float)Math.Sqrt(1+slope*slope),available=Math.Abs(edge-x)*stride;
                if(remaining<=available){x+=direction*remaining/stride;break;}
                remaining-=available;x=edge;
                GroundSurface next=null;float closestSlope=Single.PositiveInfinity;
                foreach(var candidate in CurrentRoom.surfaces){
                    if(candidate==line||!RegionGroundSpan(candidate,out left,out right))continue;
                    float join=direction>0?left:right;
                    if(Math.Abs(join-x)>.025f||Math.Abs(candidate.Y(join)-line.Y(x))>.025f)continue;
                    float turn=Math.Abs((candidate.y2-candidate.y1)/(candidate.x2-candidate.x1)-slope);
                    if(turn<closestSlope){next=candidate;closestSlope=turn;}
                }
                if(next==null)break;
                line=next;
            }
            return new RegionPoint(x/.28f,line.Y(x)/.28f,point.z);
        }

        int StrikeHamelIcePillar(Enemy pillar,int damage) {
            if(damage<=0||!pillar.active||pillar.dormant||pillar.hp<=1||pillar.invincible>0||pillar.npcState!="ICE_PILLAR_TRAP_WAIT")return 0;
            if(!RequestRegionNpcState(pillar,"ICE_PILLAR_TRAP_DAMAGE"))return 0;
            // The source damage state checks remaining HP, then runs both collapsing hit windows before death.
            int dealt=Math.Min(damage,(int)Math.Ceiling(pillar.hp)-1);
            pillar.hp-=dealt;
            return dealt;
        }

        int StrikeRegionMirror(Enemy enemy,int damage,HitReaction reaction,int raw) {
            var actor=enemy.regionActor;
            if(damage<=0||!enemy.active||enemy.dormant||enemy.hp<=0||actor?.node==null||
                !actor.node.id.EndsWith("_WAIT",StringComparison.Ordinal)||actor.node.invTo>0)return 0;
            // Source REACT_TYPE values in Enum.lua. The three wait callbacks choose
            // their own next orientation using this packet and then clear it.
            string type=raw==3?"RT_DOWN":raw==4?"RT_UP":raw==5?"RT_FLY":raw==16?"RT_DRAG_UP":null;
            if(raw==0)type=reaction==HitReaction.Launch?"RT_UP":reaction==HitReaction.Knockdown?"RT_DOWN":null;
            if(type==null)return 0;
            actor.mirrorReaction=type;
            return 1;
        }

        void LaunchRegionParabola(Enemy e,object[] args,int offset) {
            string id=RegionArg(args,offset+1) as string;
            if(id==null||!(RegionArg(args,offset+2) is RegionPoint)||
                !(RegionArg(args,offset+3) is RegionPoint)||!(RegionArg(args,offset+4) is RegionPoint))return;
            var from=(RegionPoint)args[offset+2];var to=(RegionPoint)args[offset+3];
            var gravity=(RegionPoint)args[offset+4];float time=RegionNumber(RegionArg(args,offset+5));
            if(!Finite(time)||time<=0)return;
            var shot=AddRegionSpell(e,id,from.x*.28f,from.y*.28f);
            if(shot==null)return;
            // The emitter supplies a landing point, gravity and time of flight
            // separately for each arrow. Preserve its authored hit shape and impact children.
            shot.facing=to.x<from.x?-1:1;
            float gx=shot.facing*gravity.x*.28f,gy=gravity.y*.28f;
            shot.spell=shot.spell.Parabolic(gx,gy,time);
            shot.vx=shot.facing*(to.x-from.x)*.28f/time-.5f*gx*time;
            shot.vy=(to.y-from.y)*.28f/time-.5f*gy*time;
        }

        void TickRegionChargerGrip(Enemy e,RegionActorState actor,float dt) {
            if(actor.chargerGrabState==null)return;
            if(actor.node!=actor.chargerGrabState||player.hp<=0||e.hp<=0){
                actor.chargerGrabState=null;actor.chargerHolding=false;return;
            }
            if(!actor.hit)return;
            actor.chargerHolding=true;
            player.x=ConstrainEncounterX(player,e.x+actor.chargerGripX);
            player.y=e.y+actor.chargerGripY;
            player.vx=player.vy=0;player.groundLine=-1;player.knockdownOnLanding=false;
            regionInputUntil=Math.Max(regionInputUntil,elapsed+dt*2);
        }

        bool TryRegionInteraction(Enemy e,RegionActorState a,string name,object owner,object[] raw,int offset,out object result) {
            result=null;
            object p=RegionArg(raw,offset),q=RegionArg(raw,offset+1);
            if(TryVelderSquad(e,a,name,owner,p,q,RegionArg(raw,offset+2),out result))return true;
            switch(name) {
                case "GetNearest_LUA":
                    if(!(p is RegionPoint))return false;
                    result=RegionGroundPoint((RegionPoint)p);return true;
                case "GetDamageTypeThisFrame_LUA":result=a.mirrorReaction;return true;
                case "InitDamageTypeThisFrame_LUA":a.mirrorReaction=null;return true;
                case "AnyUserUnitInRange":
                    if(p is RegionPoint) {
                        var origin=(RegionPoint)p;float radius=RegionNumber(q);
                        float dx=player.x/.28f-origin.x,dy=player.y/.28f-origin.y;
                        result=player.hp>0&&radius>=0&&dx*dx+dy*dy<=radius*radius;
                    }else result=false;
                    return true;
                case "CreateInstanceParabolic_LUA":LaunchRegionParabola(e,raw,offset);return true;
                case "SetUserGrap":
                    if(RegionTruth(p)&&q is RegionPoint) {
                        var point=(RegionPoint)q;
                        a.chargerGrabState=a.node;
                        a.chargerGripX=point.x*.28f-e.x;a.chargerGripY=point.y*.28f-e.y;
                    }
                    a.chargerHolding=false;
                    return true;
                case "SetUserGrapReset":a.chargerGrabState=null;a.chargerHolding=false;return true;
                case "IsLostGrap":result=!a.chargerHolding;return true;
                default:return false;
            }
        }
    }
}
