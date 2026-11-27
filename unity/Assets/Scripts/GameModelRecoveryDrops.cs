using System;
using System.Collections.Generic;

namespace ElswordPixel
{
    [Serializable] public sealed class RecoveryDrop
    {
        public int id,room,encounter;
        public string resource,sourceNPC;
        public float x,y,nativeDepth,vy,age,amount,percent;
        public bool grounded;
        internal int bounces;
    }
    [Serializable] public sealed class RecoveryPickupEvent
    {
        public int dropId,worldSerial;
        public string resource,sourceNPC;
        public float x,y,amount,time;
    }

    public sealed partial class GameModel
    {
        // Local 2016 DropItemManager declares HP/MP recovery drops. The box
        // loot table/rate and amounts are not recovered: guaranteed alternating
        // HP120/MP60, contact pickup and this small hop are explicit demo rules.
        public const float RecoveryDropHP=120,RecoveryDropMP=60;
        public readonly List<RecoveryDrop> recoveryDrops=new List<RecoveryDrop>();
        // A living box owns exactly one drop. Breaking it consumes that claim.
        readonly HashSet<Enemy> recoveryContainers=new HashSet<Enemy>();
        readonly Queue<RecoveryPickupEvent> recoveryPickups=new Queue<RecoveryPickupEvent>();
        int recoveryOrdinal,recoveryId;
        public int RecoveryDropWorldSerial {get;private set;}
        public int RecoveryDropsSpawned {get;private set;}
        public int RecoveryDropsPickedUp {get;private set;}

        public static bool IsRecoveryContainer(Enemy enemy)
        {
            if(enemy==null||enemy.isGate)return false;
            switch(enemy.sourceNPC){
                case "NUI_BOX_PEITA":case "NUI_BOX_VELDER":case "NUI_BOX_HAMEL":
                case "NUI_BOX":case "NUI_CHEST":case "NUI_BOX_BESMA1":case "NUI_BOX_BESMA2":
                case "NUI_BOX_ALTERA":case "NUI_CHEST_MONSTER":case "NUI_CHEST_MONSTER_BOSS":return true;
            }
            if(!string.IsNullOrEmpty(enemy.sourceNPC))return false;
            switch(enemy.kind){
                case "box":case "chest":case "prop-box":case "prop-chest":
                case "box-besma1":case "box-besma2":case "box-altera":return true;
                default:return false;
            }
        }
        void ResetRecoveryDropRun()
        {recoveryOrdinal=recoveryId=RecoveryDropsSpawned=RecoveryDropsPickedUp=0;}
        void ResetRecoveryDrops()
        {
            RecoveryDropWorldSerial++;recoveryDrops.Clear();recoveryPickups.Clear();recoveryContainers.Clear();
            foreach(var enemy in enemies)PrepareRecoveryContainer(enemy);
        }
        void PrepareRecoveryContainer(Enemy enemy)
        {
            if(!IsRecoveryContainer(enemy))return;
            // Scene/source clear flags remain intact. A breakable prop is a
            // weapon target, even if its old generic presentation was passive.
            enemy.passive=false;enemy.aiEnabled=false;
            enemy.maxHp=1;
            if(enemy.hp>0){enemy.hp=1;recoveryContainers.Add(enemy);}
        }
        void TrySpawnRecoveryDrop(Enemy enemy)
        {
            if(enemy.hp>0||!enemy.active||enemy.dormant||!recoveryContainers.Remove(enemy))return;
            AddRecoveryDrop(enemy,(recoveryOrdinal++&1)==0);
        }
        void AddRecoveryDrop(Enemy enemy,bool hp,float percent=0)
        {
            recoveryDrops.Add(new RecoveryDrop{id=++recoveryId,room=room,encounter=enemy.encounterIndex,
                sourceNPC=enemy.sourceNPC,resource=hp?"HP":"MP",percent=percent,amount=hp?RecoveryDropHP:RecoveryDropMP,
                x=enemy.x,y=enemy.y+16,nativeDepth=enemy.nativeDepth,vy=150});
            RecoveryDropsSpawned++;
        }
        public bool TryDequeueRecoveryPickup(out RecoveryPickupEvent pickup)
        {if(recoveryPickups.Count==0){pickup=null;return false;}pickup=recoveryPickups.Dequeue();return true;}
        public bool RecoveryResourceFull(RecoveryDrop drop)
        {return drop!=null&&(drop.resource=="HP"?player.hp>=player.maxHp-.0001f:mana>=MaxMana-.0001f);}
        void TickRecoveryDrops(float dt)
        {
            // This uses independent simulation time, like hostile effects:
            // pause stops it; attack hitstop does not hang a falling pickup.
            // Observe non-weapon death paths too, with the same per-actor guard.
            foreach(var enemy in enemies){
                PeitaEaterBonusDrop(enemy);
                if(!IsRecoveryContainer(enemy))continue;
                if(enemy.hp>0){
                    if(enemy.maxHp!=1||enemy.hp!=1||enemy.passive||enemy.aiEnabled||!recoveryContainers.Contains(enemy))
                        PrepareRecoveryContainer(enemy);
                }else TrySpawnRecoveryDrop(enemy);
            }
            for(int i=recoveryDrops.Count-1;i>=0;i--){
                var drop=recoveryDrops[i];drop.age+=dt;
                if(!drop.grounded){
                    float before=drop.y;drop.vy-=620*dt;drop.y+=drop.vy*dt;
                    var surface=NativeTerrain?SurfaceBelow(drop.x,before+.02f):null;
                    float floor=NativeTerrain?(surface==null?float.NegativeInfinity:surface.Y(drop.x)):GroundAt(CurrentRoom,drop.x);
                    if(drop.vy<=0&&drop.y<=floor){
                        drop.y=floor;
                        if(drop.bounces++==0&&-drop.vy>80)drop.vy=-drop.vy*.24f;
                        else{drop.vy=0;drop.grounded=true;}
                    }
                    if(drop.y< -2000){recoveryDrops.RemoveAt(i);continue;}
                }
                // Both y values are feet. Test the body against the visible
                // bottle (including its hop), so a jump can catch it in flight.
                if(drop.age<.10f||player.hp<=0||player.state==CombatState.Dead||
                    Math.Abs(player.x-drop.x)>20||player.y>drop.y+21||player.y+42<drop.y)continue;
                bool hp=drop.resource=="HP";
                float beforeResource=hp?player.hp:mana;
                float maximum=hp?player.maxHp:MaxMana;
                float amount=drop.percent>0?maximum*drop.percent/100:drop.amount;
                float restored=Math.Max(0,Math.Min(amount,maximum-beforeResource));
                if(hp)player.hp+=restored;else mana+=restored;
                if(restored>0)SpawnRecoveryNumber(drop.resource=="HP",restored,false);
                var pickup=new RecoveryPickupEvent{dropId=drop.id,worldSerial=RecoveryDropWorldSerial,resource=drop.resource,
                    sourceNPC=drop.sourceNPC,x=drop.x,y=drop.y,amount=restored,time=elapsed};
                if(recoveryPickups.Count>=32)recoveryPickups.Dequeue();recoveryPickups.Enqueue(pickup);
                RecoveryDropsPickedUp++;recoveryDrops.RemoveAt(i);
                if(restored>0)SpawnEffect("heal",player.x,player.y+25);
            }
        }
    }
}
