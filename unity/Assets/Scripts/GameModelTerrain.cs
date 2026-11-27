using System;

namespace ElswordPixel
{
    // World-pixel sole heights. Equal X values describe a walkable low step;
    // all other pairs describe a linear slope. Rooms without nodes stay flat.
    [Serializable] public sealed class GroundNode
    {
        public float x,y;
        public GroundNode() { }
        public GroundNode(float x,float y) { this.x=x;this.y=y; }
    }

    public sealed partial class GameModel
    {
        public float PlayerGroundY { get { return GroundAt(player.x); } }
        public float GroundAt(float x) { if(NativeTerrain){var surface=SurfaceBelow(x,player.y+1);return surface==null?54:surface.Y(x);}return GroundAt(CurrentRoom,x); }
        public float GroundAt(RoomDefinition definition,float x)
        {
            var nodes=definition==null?null:definition.groundNodes;
            if(nodes==null||nodes.Length==0)return GroundY;
            if(x<nodes[0].x)return nodes[0].y;
            for(int i=1;i<nodes.Length;i++)
            {
                var left=nodes[i-1];var right=nodes[i];
                // Advancing past equal-X nodes makes a vertical step right
                // continuous and avoids dividing by zero at its top edge.
                if(x<right.x)return left.y+(right.y-left.y)*(x-left.x)/(right.x-left.x);
            }
            return nodes[nodes.Length-1].y;
        }
        public bool Grounded(Fighter fighter)
        { return fighter!=null&&!EnemyHangingV25(fighter as Enemy)&&(NativeTerrain?NativeGrounded(fighter):fighter.y<=GroundAt(fighter.x)+.01f&&fighter.vy<=0); }

        private static GroundNode[] CopyGroundNodes(RoomDefinition source)
        {
            if(source.groundNodes==null||source.groundNodes.Length==0)return new GroundNode[0];
            var result=new GroundNode[source.groundNodes.Length];float previous=-1;
            for(int i=0;i<result.Length;i++)
            {
                var node=source.groundNodes[i];
                if(node==null||!Finite(node.x)||!Finite(node.y)||node.x<0||node.x>source.width||node.x<previous)
                    throw new ArgumentException("Ground nodes need finite heights and ascending X coordinates within the room.");
                result[i]=new GroundNode(node.x,node.y);previous=node.x;
            }
            return result;
        }
        private void MoveAlongGround(Fighter fighter,float x)
        {
            var fixedGate=fighter as Enemy;if(fixedGate!=null&&EnemyPositionFixedV25(fixedGate))return;
            if(NativeTerrain){MoveNativeGround(fighter,x);return;}
            bool onGround=Grounded(fighter);
            fighter.x=x;
            // Staying in contact here also covers animation root motion and
            // enemy pulls, which can move before the regular physics update.
            if(onGround)fighter.y=GroundAt(x);
        }
    }
}
