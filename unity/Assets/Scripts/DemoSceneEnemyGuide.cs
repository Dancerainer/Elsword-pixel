using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        RectTransform enemyGuide;
        Enemy nearestGuideEnemy;
        Enemy bossObjectiveTarget;
        string bossObjectiveText;
        readonly Input tunnelGuideAction=new Input();
        float nearestGuideDistanceSquared, nearestGuideHeight;

        void BuildEnemyGuide() {
            // Downward arrow, drawn on the same point-filtered atlas as the battle buttons.
            string[] pixels = {
                "....xxxxx....", "....x+++x....", "....xooox....", "....x###x....",
                "xxxxx###xxxxx", "x+ooooo####xx", ".x+ooo####xx.", "..x+o####xx..",
                "...x+###xx...", "....x##xx....", ".....xxx.....", "......x......"
            };
            var sprite = KeepControlSprite(battleArt.Stamp(pixels, C(0xff7947)));
            var image = ControlImage("Nearest enemy", new Rect(0,0,26,24), sprite, safeRoot);
            enemyGuide = image.rectTransform;
            enemyGuide.pivot = new Vector2(.5f,.5f);
            enemyGuide.SetAsFirstSibling();
            enemyGuide.gameObject.SetActive(false);
        }

        void ConsiderEnemyGuideTarget(Enemy enemy, float height) {
            if (bossObjectiveText!=null && enemy!=bossObjectiveTarget) return;
            if (enemy.hp<=0 || enemy.regionalAlly || (enemy!=bossObjectiveTarget && !GameModel.IsCombatMonster(enemy)) ||
                (game.HasEncounters && enemy.encounterIndex!=game.EncounterIndex)) return;
            float dx=enemy.x-game.player.x, dy=enemy.y-game.player.y;
            float distance=dx*dx+dy*dy;
            if (nearestGuideEnemy!=null && distance>=nearestGuideDistanceSquared) return;
            nearestGuideEnemy=enemy;
            nearestGuideDistanceSquared=distance;
            nearestGuideHeight=height;
        }

        string EnemyGuideMission() {
            string mission=game.Status();
            if(mission!="DEFEAT ALL ENEMIES")return mission;
            if(bossObjectiveText!=null)return bossObjectiveText;
            if(!belowPlayerSessionV21 || nearestGuideEnemy==null)return mission;
            tunnelGuideAction.drop=false;
            if(!BelowTunnelLowerPathInputV21.TryDrive(true,game,tunnelGuideAction,nearestGuideEnemy))return mission;
            return tunnelGuideAction.drop?"PRESS DOWN TO DROP":"MOVE TO THE DROP";
        }

        void RenderEnemyGuide() {
            if (!enemyGuide) return;
            bool visible=nearestGuideEnemy!=null && adventureStarted && !game.gateOpen &&
                !game.victory && game.player.hp>0 && !game.paused && !BossIntroductionActive &&
                !settingsOpen && string.IsNullOrEmpty(openMenu);
            if (visible!=enemyGuide.gameObject.activeSelf) enemyGuide.gameObject.SetActive(visible);
            if (!visible) return;

            Camera ui=hudCanvas.renderMode==RenderMode.ScreenSpaceOverlay ? null : hudCanvas.worldCamera;
            Rect frame=worldCamera.pixelRect, safe=Screen.safeArea;
            if (safe.width<=0 || safe.height<=0) safe=frame;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(safeRoot,Vector2.Max(frame.min,safe.min),ui,out var lo);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(safeRoot,Vector2.Min(frame.max,safe.max),ui,out var hi);
            // Reserve the top HUD and the bottom controls while keeping the edges of wide phones usable.
            lo+=new Vector2(19,KeyboardMode ? 83 : 116);
            hi-=new Vector2(19,93);
            Vector2 center=(lo+hi)*.5f, half=(hi-lo)*.5f;
            var status=game.GetEnemyDebuff(nearestGuideEnemy);
            float clearance=status!=null && status.Label.Length>0 ? 34 : 12;
            Vector3 head=RenderEnemyPosition(nearestGuideEnemy);
            if(game.TryNephilimGuideBounds(nearestGuideEnemy,out var bounds)){
                // Its movement pivot is behind the head and the offset flips with its facing.
                head.x+=(bounds.left+bounds.right)*.5f-nearestGuideEnemy.x;
                head.y+=bounds.top-nearestGuideEnemy.y+clearance;
            }else head.y+=nearestGuideHeight+clearance;
            float depth=nearestGuideEnemy.nativeDepth;
            if(Mine2LowerPathInputV21.TryGetDescent(game,nearestGuideEnemy,out float ledgeX,out float ledgeY,out _)){
                head=new Vector3(ledgeX,ledgeY,0);
                depth=0; // Playable path depth, independent of the building's native depth.
            }
            var projected=ProjectNativePointWithEye(head,depth,out float eye);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(safeRoot,worldCamera.WorldToScreenPoint(projected),ui,out var point);
            point.y+=12;
            Vector2 delta=point-center;
            if (NativeWorldActive && eye<=0) {
                // A road can bend behind the native camera. Follow the fight's
                // world direction instead of using the mirrored perspective point.
                delta=new Vector2(head.x-game.player.x,head.y-game.player.y).normalized*(half.magnitude+1);
            }
            float extent=Mathf.Max(Mathf.Abs(delta.x)/Mathf.Max(1,half.x),Mathf.Abs(delta.y)/Mathf.Max(1,half.y));
            bool atEdge=extent>1;
            enemyGuide.anchoredPosition=atEdge ? center+delta/extent : point;
            enemyGuide.localRotation=atEdge ? Quaternion.Euler(0,0,Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg+90) : Quaternion.identity;
        }
    }

    public sealed partial class GameModel {
        // The Altera bosses have separate, independently targetable bodies. Show
        // the objective that advances the current phase through the real health/state rules.
        internal Enemy AlteraBossObjective(out string instruction) {
            instruction=null;
            if(victory || BossCompletionPendingV57 || player.hp<=0 || room!=RoomCount-1) return null;
            switch(CurrentRoom.id) {
                case "Dungeon_Altera_Core_Hard-4":
                    if(!CoreHardClosureScopeV21())return null;
                    var plant=CoreHardNearV21("NUI_EL_POWER_PLANT",false);
                    if(plant==null)return null;
                    if(plant.hp<=0){instruction="CORE DESTROYED";return null;}
                    if(CoreHardActorRuntimeV21(plant).up && CoreHardActorShownV21(plant)){
                        instruction="STRIKE THE EXPOSED CORE";return plant;
                    }
                    var extractor=NearestAlteraTarget(AlteraTarget.Extractor);
                    instruction=extractor==null?"WAIT FOR THE CORE":"DESTROY THE EXTRACTORS";
                    return extractor;
                case "Dungeon_Altera_Below_Tunnel_Normal-2":
                    if(belowFortV20==null)return null;
                    var armor=FortKeyV20(belowFortV20.armorKey);
                    if(armor!=null){instruction="BREAK THE FORT ARMOR";return armor;}
                    return AlteraBossById(belowFortV20.npc,"ATTACK THE FORT",out instruction);
                case "Dungeon_Altera_Plain_Normal-2":
                    if(!PlainPairRoomScopeR1258())return null;
                    var machine=NearestAlteraTarget(AlteraTarget.Generator);
                    if(machine!=null){instruction="DISABLE THE GENERATORS";return machine;}
                    instruction="DEFEAT THE TWO GUARDIANS";
                    return NearestAlteraTarget(AlteraTarget.Boss);
                case "Dungeon_Besma_Air_Ship_Normal-2":
                    var shell=AlteraBossById("NUI_WALLY_9TH_ARMOR","BREAK WALLY'S ARMOR",out instruction);
                    return shell??AlteraBossById("NUI_WALLY_9TH","DEFEAT WALLY",out instruction);
                case "Dungeon_Altera_Battle_Air_Ship_Normal-3":return AlteraBossById("NUI_RAVEN","DEFEAT RAVEN",out instruction);
                case "Dungeon_Altera_Nasod_Factory_Normal-2":return AlteraBossById("NUI_RAVEN_CLONE","DEFEAT THE RAVEN CLONE",out instruction);
                case "Dungeon_Altera_Plain_Recycle_Normal-2":return AlteraBossById("NUI_NASOD_DRILLER_CANNON","STOP THE DRILLER",out instruction);
                default:return null;
            }
        }

        Enemy AlteraBossById(string npc,string label,out string instruction) {
            foreach(var e in enemies)if(e.sourceNPC==npc&&e.hp>0&&e.active&&!e.dormant&&e.encounterIndex==EncounterIndex){
                instruction=label;return e;
            }
            instruction=null;return null;
        }

        enum AlteraTarget { Extractor, Generator, Boss }

        Enemy NearestAlteraTarget(AlteraTarget type) {
            Enemy best=null;float minimum=float.PositiveInfinity;
            foreach(var e in enemies) {
                if(!e.active||e.dormant||e.hp<=0||e.encounterIndex!=EncounterIndex)continue;
                if(type==AlteraTarget.Boss && !IsDungeonBoss(e))continue;
                if(type==AlteraTarget.Extractor && (!CoreHardExtractorV21(e.sourceNPC)||!CoreHardActorShownV21(e)))continue;
                if(type==AlteraTarget.Generator && (!PlainPairMachineIDR1258(e.sourceNPC)||e.hp<=e.maxHp*.31f))continue;
                float dx=e.x-player.x,dy=e.y-player.y,d=dx*dx+dy*dy;
                if(d<minimum){minimum=d;best=e;}
            }
            return best;
        }

    }
}
