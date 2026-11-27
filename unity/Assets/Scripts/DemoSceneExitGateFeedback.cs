using System;
using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        const int CurtainFrameWidth = 80, CurtainFrameHeight = 120, CurtainOpenFrame = 6;
        readonly Sprite[] curtainFrames = new Sprite[CurtainOpenFrame + 1];
        RoomDefinition curtainRoom;
        EncounterDefinition curtainEncounter;
        bool curtainHasExit;
        float curtainX;
        GroundSurface curtainFloor;
        SpriteRenderer backCurtain;
        float backCurtainX;
        GroundSurface backCurtainFloor;
        bool hasBackCurtain;
        Image stageClearImage;

        bool HasGlassPassage(EncounterDefinition source, int destination = -1) {
            foreach (var clause in source.clauses) {
                if (!clause.requiresExit) continue;
                foreach (var edge in clause.transitions)
                    if (!edge.completeDungeon && edge.targetRoom == game.room &&
                        !ReferenceEquals(curtainRoom.encounters[edge.targetEncounter],source) &&
                        (destination < 0 || edge.targetEncounter == destination)) return true;
            }
            return false;
        }

        void RenderStageClear() {
            float age=game.StageClearAge;
            bool visible=adventureStarted && age>=0 && age<GameModel.StageClearDuration && game.player.hp>0 &&
                !game.victory && !game.paused && !game.BossCompletionPendingV57 && !BossIntroductionActive &&
                !settingsOpen && string.IsNullOrEmpty(openMenu);
            if (visible && !stageClearImage) {
                stageClearImage=ControlImage("Stage CLEAR",new Rect(0,0,244,78),Picture("Pixel/ui/stage-clear"),safeRoot);
                var rect=stageClearImage.rectTransform;
                rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);
                rect.pivot=new Vector2(.5f,.5f);
            }
            if (!stageClearImage) return;
            if (stageClearImage.gameObject.activeSelf!=visible) stageClearImage.gameObject.SetActive(visible);
            if (!visible) return;
            float enter=Mathf.Clamp01(age/.12f), leave=Mathf.Clamp01((GameModel.StageClearDuration-age)/.15f);
            stageClearImage.rectTransform.anchoredPosition=new Vector2(Mathf.Round(-18*(1-enter)),68);
            stageClearImage.color=new Color(1,1,1,leave);
        }

        void DrawGlassCurtain() {
            var encounter = game.CurrentEncounter;
            if (!ReferenceEquals(curtainRoom, game.CurrentRoom) || !ReferenceEquals(curtainEncounter, encounter)) {
                curtainRoom = game.CurrentRoom;
                curtainEncounter = encounter;
                curtainHasExit = encounter != null && HasGlassPassage(encounter);
                curtainFloor = backCurtainFloor = null;
                if (curtainHasExit) curtainX = game.CurtainX;
                hasBackCurtain = false;
                if (encounter != null) {
                    backCurtainX = encounter.direction > 0 ? encounter.leftX : encounter.rightX;
                    foreach (var other in curtainRoom.encounters) {
                        if ((encounter.direction > 0 ? other.leftX < backCurtainX-1 : other.rightX > backCurtainX+1) &&
                            HasGlassPassage(other,game.EncounterIndex)) {
                            hasBackCurtain = true;
                            break;
                        }
                    }
                }
            }
            if ((!curtainHasExit && !hasBackCurtain) || !adventureStarted || game.victory) {
                if (exitMarker) exitMarker.enabled = false;
                if (backCurtain) backCurtain.enabled = false;
                return;
            }
            if (!exitMarker) {
                var sheet = Resources.Load<Texture2D>("Pixel/ui/glass-curtain");
                if (!sheet || sheet.width != CurtainFrameWidth * curtainFrames.Length || sheet.height != CurtainFrameHeight)
                    throw new InvalidOperationException("Missing dungeon curtain sprite sheet.");
                for (int i = 0; i < curtainFrames.Length; i++) {
                    var rect = new Rect(i * CurtainFrameWidth, 0, CurtainFrameWidth, CurtainFrameHeight);
                    curtainFrames[i] = Sprite.Create(sheet, rect, new Vector2(.5f, 8f/CurtainFrameHeight), 1, 0, SpriteMeshType.FullRect);
                }
                exitMarker = WorldSprite("Dungeon glass curtain", worldRoot, curtainFrames[0], 12, Vector3.zero);
            }
            if (hasBackCurtain) {
                if (!backCurtain) backCurtain = WorldSprite("Previous combat area curtain",worldRoot,curtainFrames[0],12,Vector3.zero);
                backCurtain.flipX = encounter.direction > 0;
                backCurtain.enabled = true;
                backCurtain.transform.localPosition = CurtainPosition(backCurtainX,ref backCurtainFloor);
            } else if (backCurtain) backCurtain.enabled = false;
            exitMarker.enabled = curtainHasExit;
            if (!curtainHasExit) return;
            float age=game.CurtainBreakAge;
            float progress=Mathf.Clamp01(age/GameModel.CurtainBreakDuration);
            int frame=!game.gateOpen || age<0 ? 0 : 1+Mathf.FloorToInt(progress*(CurtainOpenFrame-1));
            exitMarker.sprite = curtainFrames[frame];
            exitMarker.flipX = encounter != null && encounter.direction < 0;
            exitMarker.transform.localPosition = CurtainPosition(curtainX,ref curtainFloor);
        }

        Vector3 CurtainPosition(float x, ref GroundSurface floor) {
            if (game.NativeTerrain && (floor == null || floor.hamelDisabled)) floor = FindCurtainFloor(x);
            float y = floor == null ? game.GroundAt(curtainRoom,x) : floor.Y(Mathf.Clamp(x,floor.x1,floor.x2));
            return new Vector3(x,y,0);
        }

        GroundSurface FindCurtainFloor(float boundary) {
            // Use the authored walking profile, independent of a player's jumping height.
            float baseY = game.GroundAt(curtainRoom, boundary);
            GroundSurface best = null;
            float bestDx = float.PositiveInfinity, bestDy = float.PositiveInfinity;
            foreach (var s in curtainRoom.surfaces) {
                if (s.hamelDisabled || s.npcOnly) continue;
                float x = Mathf.Clamp(boundary,s.x1,s.x2);
                float dx = Mathf.Abs(x-boundary), dy = Mathf.Abs(s.Y(x)-baseY);
                if (dx < bestDx || (dx == bestDx && dy < bestDy)) { best=s; bestDx=dx; bestDy=dy; }
            }
            return best;
        }

        void DisposeGlassCurtain() {
            foreach (var frame in curtainFrames) if (frame) Destroy(frame);
        }

        Enemy RequiredExitGate() {
            return game.RequiredExitGate();
        }

        string ExitGatePrompt() {
            if (game.victory || game.player.hp<=0) return "";
            if (game.StageClearAge>=0 && game.StageClearAge<GameModel.StageClearDuration) return "";
            if (game.gateOpen) {
                if (game.GateTransitionDelayRemaining>0) return "GATE OPENING...";
                float exit=game.HasEncounters ? game.CurrentEncounter.exitX : game.ExitX;
                return ExitDirection("AREA CLEAR  /  EXIT",exit-game.player.x);
            }
            var gate=RequiredExitGate();
            if(gate!=null)return ExitDirection("BREAK THE EXIT GATE",gate.x-game.player.x);
            if(Mine2LowerPathInputV21.TryGetDescent(game,nearestGuideEnemy,out float ledgeX,out _,out bool drop) && game.Grounded(game.player))
                return drop ? "DROP THROUGH TO THE LOWER TRACK" : ledgeX<game.player.x ? "LOWER TRACK  /  GO LEFT" : "LOWER TRACK  /  GO RIGHT";
            if(game.CurrentRoom.id=="Dungeon_Besma_Dragon_Nest_Normal-2" && !game.BossCompletionPendingV57){
                const string warning="HOMING FLAMES  /  KEEP MOVING";
                foreach(var enemy in game.enemies)
                    if(enemy.hp>0 && enemy.active && !enemy.dormant && (enemy.npcState=="LIZARDMAN_SHAMAN_KING_ATTACK_B_READY" || enemy.npcState=="LIZARDMAN_SHAMAN_KING_ATTACK_B"))return warning;
                foreach(var effect in game.npcEffects)
                    if(!effect.ended && effect.id=="DamageEffect_LIZARDMAN_SHAMAN_KING_ATTACK_B")return warning;
            }
            return "";
        }

        string ExitDirection(string action,float delta) {
            if (Mathf.Abs(delta)<12) return action;
            return action+(delta<0?"  /  LEFT":"  /  RIGHT");
        }

        void ShowNearbyForestExitGate() {
            if (!NativeWorldActive) return;
            string room=game.CurrentRoom.id;
            bool forest=room=="forest-1"||room=="forest-2";
            bool ruins=room=="ruins-1"||room=="ruins-2";
            if (!forest&&!ruins) return;
            // Ruins' original foreground slabs cover the exit prop itself.
            // Reveal this exact nearby gate while locked as well as unlocked;
            // damage protection and clear requirements remain model-owned.
            var gate=ruins?game.UnbrokenExitGate:RequiredExitGate();
            if (gate==null || (forest?gate.sourceNPC!="NUI_ELDER_DRUID_FOREST_GATE":
                gate.sourceNPC!="NUI_RUBEN_RUIN_GATE"||gate.kind!="prop-ruins-gate") ||
                Mathf.Abs(game.player.x-gate.x)>120 || Mathf.Abs(game.player.y-gate.y)>70) return;
            int index=game.enemies.IndexOf(gate);
            if (index<0 || index>=enemyViews.Count) return;
            var body=enemyViews[index];
            if (!body || !body.enabled || !body.gameObject.activeInHierarchy) return;
            // Keep registered feet and pose; only this nearby exit prop
            // bypasses the foreground depth clip.
            if (initialNpcMaterialV25==null) initialNpcMaterialV25=new MaterialPropertyBlock();
            body.GetPropertyBlock(initialNpcMaterialV25);
            initialNpcMaterialV25.SetFloat("_NativeSceneClipR1461",0);
            body.SetPropertyBlock(initialNpcMaterialV25);
            initialNpcMaterialV25.Clear();
        }
    }
}
