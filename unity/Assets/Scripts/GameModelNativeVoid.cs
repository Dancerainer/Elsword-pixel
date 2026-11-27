using System;
using System.Collections.Generic;

namespace ElswordPixel {
    public sealed partial class GameModel {
        struct VoidEdge {
            internal float x, passHeight;
            internal int direction;
        }

        readonly List<VoidEdge> nativeVoidEdges=new List<VoidEdge>();
        GroundSurface[] voidSurfaces;
        GroundSurface lastFooting;
        int lastFootingChart;
        float lastFootingX;

        // Include the longest ordinary dash-jump (the mine uses normal gravity)
        // and the extra time gained when the next deck is lower. Deliberate
        // jumpable gaps and drops keep their original traversal rules.
        static float PossibleJumpSpan(float drop) {
            const float horizontal=332, vertical=644, gravity=1120;
            return horizontal*(vertical+(float)Math.Sqrt(vertical*vertical+2*gravity*drop))/gravity;
        }

        void FindVoidEdge(GroundSurface floor,int direction) {
            float edge=direction>0?floor.x2:floor.x1, height=direction>0?floor.y2:floor.y1;
            float outside=edge+direction*.06f;
            if(SurfaceBelow(outside,height+7)!=null)return;

            float passHeight=float.PositiveInfinity;
            foreach(var other in voidSurfaces){
                float far=direction>0?other.x2:other.x1;
                if(direction*(far-outside)<0)continue;
                float near=direction>0?Math.Max(outside,other.x1):Math.Min(outside,other.x2);
                float nearHeight=other.Y(near),farHeight=other.Y(far);
                float lowest=Math.Min(nearHeight,farHeight);
                if(lowest>height+7){
                    // A lower deck can end underneath another floor. A jump
                    // high enough for that floor may still cross this edge.
                    if(direction*(near-edge)<=PossibleJumpSpan(0))passHeight=Math.Min(passHeight,lowest);
                    continue;
                }
                if(nearHeight>height+7)near+=(far-near)*(height+7-nearHeight)/(farHeight-nearHeight);
                if(direction*(near-edge)<=PossibleJumpSpan(Math.Max(0,height-lowest)))return;
            }
            nativeVoidEdges.Add(new VoidEdge{x=edge,direction=direction,passHeight=passHeight});
        }

        float ConstrainVoidCrossing(float target) {
            if(target==player.x)return target;
            if(!ReferenceEquals(voidSurfaces,CurrentRoom.surfaces)){
                voidSurfaces=CurrentRoom.surfaces;nativeVoidEdges.Clear();
                foreach(var s in voidSurfaces){FindVoidEdge(s,-1);FindVoidEdge(s,1);}
            }
            int direction=Math.Sign(target-player.x);
            for(int i=0;i<nativeVoidEdges.Count;i++){
                var edge=nativeVoidEdges[i];
                if(direction!=edge.direction||direction*(edge.x-player.x)<0||direction*(target-edge.x)<=0)continue;
                if(player.y>=edge.passHeight-.02f)continue;
                // A lower parallel deck at the same X, if present, has its own
                // contact and remains usable. The Cargo chart handoff runs
                // before ordinary movement and uses its linked endpoint.
                if(SurfaceBelow(edge.x+direction*.06f,player.y+.05f,player.dropThrough>0)!=null)continue;
                target=edge.x-direction*.025f;
            }
            return target;
        }

        void RememberNativeFooting(GroundSurface surface) {
            lastFooting=surface;lastFootingX=player.x;lastFootingChart=CargoProjectionChartSerialR1479;
        }

        bool SafeXOnSurface(GroundSurface floor,float preferred,out float x) {
            float low=Math.Max(floor.x1,ConstrainEncounterX(player,NativeClampX(floor.x1)));
            float high=Math.Min(floor.x2,ConstrainEncounterX(player,NativeClampX(floor.x2)));
            if(low>high){x=0;return false;}
            float inset=Math.Min(9.8f,(high-low)*.5f);
            x=Clamp(preferred,low+inset,high-inset);return true;
        }

        GroundSurface FindVoidReturn(out float x) {
            x=player.x;
            if(lastFooting!=null&&lastFootingChart==CargoProjectionChartSerialR1479&&SafeXOnSurface(lastFooting,lastFootingX,out x))return lastFooting;
            // A stage transition or an externally applied launch may leave no
            // previous contact. Choose real footing inside the active bounds.
            GroundSurface nearest=null;float distance=float.PositiveInfinity,closestHeight=float.PositiveInfinity;
            foreach(var floor in CurrentRoom.surfaces){
                if(!SafeXOnSurface(floor,player.x,out float candidate))continue;
                float gap=Math.Abs(candidate-player.x),level=Math.Abs(floor.Y(candidate)-CurrentRoom.spawnY);
                if(gap>distance||(gap==distance&&level>=closestHeight))continue;
                nearest=floor;x=candidate;distance=gap;closestHeight=level;
            }
            return nearest;
        }

        void RecoverFromNativeFall(Fighter fighter,bool isPlayer) {
            var floor=SurfaceBelow(fighter.x,10000);
            if(floor==null&&isPlayer){
                floor=FindVoidReturn(out float x);
                if(floor!=null)fighter.x=x;
            }
            if(floor==null)return;
            fighter.y=floor.Y(fighter.x);fighter.groundLine=floor.id;fighter.vy=0;
            fighter.state=!isPlayer&&fighter.knockdownOnLanding?CombatState.Airborne:CombatState.Normal;
            if(!isPlayer)return;
            fighter.vx=0;fighter.dropThrough=0;fighter.knockdownOnLanding=false;
            float before=player.hp;ApplyCombatHealth(player,Math.Max(1,player.hp-40));SpawnResourceDamage(player,before-player.hp);LandPlayer();
        }
    }
}
