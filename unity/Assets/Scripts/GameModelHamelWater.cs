using System;

namespace ElswordPixel {
    [Serializable] public sealed class HamelWaterLine {
        public float surfaceY, buoyancy, resistance, animation, oxygenPerSecond;
    }

    public sealed partial class GameModel {
        HamelWaterLine hamelWater;
        bool hamelWaterRoom;
        float suffocationClock;
        public float Oxygen { get; private set; } = 100;
        public bool IsUnderWater { get { return hamelWater != null; } }
        public int WaterEntrySerial { get; private set; }
        float HamelWaterResistance { get { return hamelWater?.resistance ?? 1; } }
        float HamelWaterAnimation { get { return hamelWater?.animation ?? 1; } }
        float HamelGravityScale { get { return hamelWater?.buoyancy ?? 1; } }

        static bool CanHamelWaterJump(GroundSurface origin, float gap, float rise) {
            if (origin.water == null || origin.water.buoyancy <= 0 || origin.water.surfaceY <= Math.Max(origin.y1, origin.y2) + rise) return false;
            // Keep the waterway's climber on the small, high-lift air vents.
            // Under the native gravity multipliers .23/.22 a walking jump
            // clears the corresponding 312/336-unit steps.
            float gravity = 1120 * origin.water.buoyancy;
            float residual = 420 * 420 - 2 * gravity * (rise + 4);
            if (residual < 0) return false;
            float flight = (420 + (float)Math.Sqrt(residual)) / gravity;
            return Math.Max(0, gap) + 26 <= 130 * origin.water.resistance * flight;
        }

        // Nonzero SPEED rows in the original Hamel line maps. Signs are relative to
        // the authored start -> end. The frozen temple's first camera lane is reversed.
        static void BindHamelSourceGroundSpeeds(RoomDefinition room) {
            switch(room.id) {
                case "hamel-outskirts-resiam-2":
                    SetHamelLineSpeed(room,1,1500);SetHamelLineSpeed(room,2,1500);break;
                case "hamel-outskirts-resiam-3":
                    SetHamelLineSpeed(room,18,800);SetHamelLineSpeed(room,27,-800);break;
                case "hamel-break-resiam-1":
                    SetHamelLineSpeed(room,1,1500);SetHamelLineSpeed(room,2,1500);SetHamelLineSpeed(room,3,1500);break;
                case "hamel-waterway-core-4":
                    SetHamelLineSpeed(room,8,250,npcOnly:true);SetHamelLineSpeed(room,9,-250,npcOnly:true);break;
                case "hamel-frozen-water-temple-3":
                    SetHamelLineSpeed(room,6,-2000,projectedDirection:-1);break;
            }
        }

        static void SetHamelLineSpeed(RoomDefinition room,int id,float nativeSpeed,int projectedDirection=1,bool npcOnly=false) {
            var floor=Array.Find(room.surfaces,s=>s.id==id);
            if(floor==null)return;
            float dx=floor.x2-floor.x1,dy=floor.y2-floor.y1;
            float length=(float)Math.Sqrt(dx*dx+dy*dy);
            if(length<=0)return;
            floor.conveyorSpeed=nativeSpeed*.28f*projectedDirection*dx/length;
            if(npcOnly)floor.npcOnly=true;
        }

        void ResetHamelWater() {
            hamelWaterRoom = CurrentRoom.surfaces != null && Array.Exists(CurrentRoom.surfaces, s => s.water != null && s.water.buoyancy > 0);
            hamelWater = null;
            Oxygen = 100;
            suffocationClock = 0;
        }

        void TickHamelWater(float dt) {
            if (!hamelWaterRoom || player.hp <= 0) return;
            // A diver retains the properties of the line below the current lane
            // during a jump. Each native line has its own waterline and air source.
            var surface = SurfaceBelow(player.x, player.y + .1f);
            var next = surface != null ? surface.water : hamelWater;
            if (next != null && (next.buoyancy <= 0 || player.y > next.surfaceY)) next = null;
            if (next != null && hamelWater == null) WaterEntrySerial++;
            hamelWater = next;
            if (next == null) { Oxygen = 100; suffocationClock = 0; return; }
            Oxygen = Clamp(Oxygen + dt * next.oxygenPerSecond, 0, 100);
            if (Oxygen > 0) { suffocationClock = 0; return; }
            suffocationClock += dt;
            if (suffocationClock >= 1) {
                suffocationClock -= 1;
                // The 2016 line data gives oxygen gain/loss. Its native damage
                // interval and amount are represented here in demo health units.
                ApplyHitToPlayer(Math.Max(1, (int)(player.maxHp * .05f)), player.x, HitReaction.None);
            }
        }
    }
}
