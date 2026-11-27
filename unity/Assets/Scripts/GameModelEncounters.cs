using System;
using System.Collections.Generic;

namespace ElswordPixel {
    public enum EncounterClearMode { KillAllActive, KillNpcTypes, KillNpcKeyCodes, SurviveTime }
    [Serializable] public sealed class EncounterTransition {
        public int targetRoom,targetEncounter,weight=100;
        public bool completeDungeon;
    }
    [Serializable] public sealed class EncounterClearClause {
        public string id="";public EncounterClearMode mode;public bool requiresExit=true;public float seconds,delaySeconds;
        public string[] npcTypes=new string[0];
        public int[] npcKeyCodes=new int[0];
        public EncounterTransition[] transitions=new EncounterTransition[0];
    }
    [Serializable] public sealed class EncounterDefinition {
        public string id="",sourceWorldId="";
        public int mainLineSet,direction=1;
        public float leftX=22,rightX=480,exitX=454;
        public bool requiresExit=true;
        public EncounterClearMode clearMode;
        public string[] clearNpcTypes=new string[0];
        public EncounterTransition[] transitions=new EncounterTransition[0];
        public EncounterClearClause[] clauses=new EncounterClearClause[0];
    }
    public sealed partial class GameModel {
        internal RoomDefinition NextRoomForWarmup { get { return room+1<rooms.Count ? rooms[room+1] : null; } }
        int currentEncounter,pendingEncounterEntry=-1;
        float encounterStartedAt;
        uint encounterRandom=0x9173a5u;
        public int EncounterIndex {get{return currentEncounter;}}
        public int EncounterCount {get{return HasEncounters?CurrentRoom.encounters.Length:1;}}
        public bool HasEncounters {get{return CurrentRoom.encounters!=null&&CurrentRoom.encounters.Length>0;}}
        public EncounterDefinition CurrentEncounter {get{return HasEncounters?CurrentRoom.encounters[currentEncounter]:null;}}
        public bool EncounterCombatClear {
            get {
                if(!HasEncounters)return false;
                foreach(var enemy in enemies)if(enemy.active&&enemy.encounterIndex==currentEncounter&&enemy.countsForClear&&!enemy.isGate&&enemy.hp>0)return false;
                return true;
            }
        }
        public float EncounterTimeRemaining {
            get {
                if (HasEncounters)
                    foreach (var clause in CurrentEncounter.clauses)
                        if (clause.mode == EncounterClearMode.SurviveTime)
                            return Math.Max(0, clause.seconds - (elapsed - encounterStartedAt));
                return 0;
            }
        }
        public bool EncounterConditionMet {
            get {return HasEncounters&&ResolvedEncounterClause()!=null;}
        }
        EncounterClearClause ResolvedEncounterClause() {
            foreach(var clause in CurrentEncounter.clauses) {
                if(clause.mode==EncounterClearMode.SurviveTime){if(elapsed-encounterStartedAt>=clause.seconds)return clause;continue;}
                if(clause.mode==EncounterClearMode.KillAllActive){if(EncounterCombatClear)return clause;continue;}
                if(clause.mode==EncounterClearMode.KillNpcKeyCodes){if(SourceNpcKeyCodesSatisfied(clause.npcKeyCodes))return clause;continue;}
                bool satisfied=clause.npcTypes.Length>0;
                foreach(string kind in clause.npcTypes) {
                    bool found=false;
                    foreach(var enemy in enemies)if(enemy.encounterIndex==currentEncounter&&enemy.sourceNPC==kind) {
                        found=true;if(enemy.hp>0)satisfied=false;
                    }
                    // A not-yet-created target is not equivalent to a dead one.
                    if(!found)satisfied=false;
                }
                if(satisfied)return clause;
            }
            return null;
        }
        static EncounterDefinition[] CopyEncounters(RoomDefinition room) {
            if(room.encounters==null)return new EncounterDefinition[0];
            var result=new EncounterDefinition[room.encounters.Length];
            for(int i=0;i<result.Length;i++) {
                var e=room.encounters[i];
                if(e==null||!Finite(e.leftX)||!Finite(e.rightX)||!Finite(e.exitX)||e.leftX<0||e.rightX>room.width||e.rightX<=e.leftX||e.exitX<e.leftX||e.exitX>e.rightX||(e.direction!=1&&e.direction!=-1))throw new ArgumentException("Invalid authored encounter bounds.");
                var edges=e.transitions??new EncounterTransition[0];var copies=new EncounterTransition[edges.Length];
                for(int j=0;j<edges.Length;j++) {
                    var edge=edges[j];if(edge==null||edge.weight<=0)throw new ArgumentException("Encounter transitions need positive weights.");
                    copies[j]=new EncounterTransition{targetRoom=edge.targetRoom,targetEncounter=edge.targetEncounter,weight=edge.weight,completeDungeon=edge.completeDungeon};
                }
                result[i]=new EncounterDefinition{id=e.id,sourceWorldId=e.sourceWorldId,mainLineSet=e.mainLineSet,direction=e.direction,
                    leftX=e.leftX,rightX=e.rightX,exitX=e.exitX,requiresExit=e.requiresExit,clearMode=e.clearMode,
                    clearNpcTypes=e.clearNpcTypes==null?new string[0]:(string[])e.clearNpcTypes.Clone(),transitions=copies};
                var originals=e.clauses!=null&&e.clauses.Length>0?e.clauses:new[]{new EncounterClearClause{id="default",mode=e.clearMode,requiresExit=e.requiresExit,npcTypes=e.clearNpcTypes,transitions=edges}};
                result[i].clauses=new EncounterClearClause[originals.Length];
                for(int j=0;j<originals.Length;j++) {
                    var c=originals[j];if(c==null)throw new ArgumentException("Null source clear clause.");
                    var targets=c.npcTypes??new string[0];if(c.mode==EncounterClearMode.KillNpcTypes&&targets.Length==0)throw new ArgumentException("Kill-type encounter has no original NPC target.");
                    var keyCodes=c.npcKeyCodes??new int[0];if(c.mode==EncounterClearMode.KillNpcKeyCodes&&(keyCodes.Length==0||Array.Exists(keyCodes,k=>k<=0)))throw new ArgumentException("Kill-keycode encounter has no valid original instance key.");
                    if(c.mode==EncounterClearMode.SurviveTime&&(!Finite(c.seconds)||c.seconds<=0))throw new ArgumentException("Timed encounter needs a positive source duration.");
                    if(!Finite(c.delaySeconds)||c.delaySeconds<0)throw new ArgumentException("Invalid clear transition delay.");
                    if(!Enum.IsDefined(typeof(EncounterClearMode),c.mode))throw new ArgumentException("Unknown source clear clause mode.");
                    var originalEdges=c.transitions??new EncounterTransition[0];var clonedEdges=new EncounterTransition[originalEdges.Length];
                    for(int k=0;k<originalEdges.Length;k++){var edge=originalEdges[k];if(edge==null||edge.weight<=0)throw new ArgumentException("Clear-clause transition needs a positive weight.");clonedEdges[k]=new EncounterTransition{targetRoom=edge.targetRoom,targetEncounter=edge.targetEncounter,weight=edge.weight,completeDungeon=edge.completeDungeon};}
                    result[i].clauses[j]=new EncounterClearClause{id=c.id,mode=c.mode,seconds=c.seconds,delaySeconds=c.delaySeconds,requiresExit=c.requiresExit,npcTypes=(string[])targets.Clone(),npcKeyCodes=(int[])keyCodes.Clone(),transitions=clonedEdges};
                }
            }
            if(result.Length>0&&(room.entryEncounter<0||room.entryEncounter>=result.Length))throw new ArgumentException("Invalid entry encounter.");
            return result;
        }
        static void ValidateEncounterGraph(List<RoomDefinition> definitions) {
            for(int roomIndex=0;roomIndex<definitions.Count;roomIndex++) {
                var room=definitions[roomIndex];
                foreach(var spawn in room.enemies)if(room.encounters.Length>0&&(spawn.encounterIndex<0||spawn.encounterIndex>=room.encounters.Length))throw new ArgumentException("NPC has no original encounter.");
                foreach(var encounter in room.encounters)foreach(var clause in encounter.clauses)foreach(var edge in clause.transitions) {
                    if(edge.completeDungeon)continue;
                    if(edge.targetRoom<0||edge.targetRoom>=definitions.Count||edge.targetEncounter<0||edge.targetEncounter>=definitions[edge.targetRoom].encounters.Length)throw new ArgumentException("Encounter transition refers to a missing stage/substage.");
                }
            }
        }
        void BeginEncounter(int index) {
            lastFooting=null;
            BeginCoreStopIntroEncounterV21();
            ResetBossCompletionV57();
            ResetGateTransitionDelay();
            currentEncounter=index;
            encounterStartedAt=elapsed;
            if(!HasEncounters){currentEncounter=0;CountScoreEncounter();return;}
            if(index<0||index>=CurrentRoom.encounters.Length)throw new InvalidOperationException("Missing encounter entry.");
            CountScoreEncounter();
            foreach(var enemy in enemies) {
                if(enemy.encounterIndex>index)enemy.active=false;
                else if(enemy.encounterIndex==index)enemy.active=true;
                if(enemy.isGate&&enemy.encounterIndex==index&&enemy.hp>0)enemy.invincible=3600;
            }
            gateOpen=false;roomBanner=2.6f;
        }
        float ConstrainEncounterX(Fighter fighter,float x) {
            if(!HasEncounters)return x;
            float cargoProjected;if(TryCargoProjectionEncounterClampR1479(fighter,x,out cargoProjected))return cargoProjected;
            var enemy=fighter as Enemy;
            if(fighter!=player&&(enemy==null||!enemy.active||enemy.hp<=0||enemy.encounterIndex!=currentEncounter))return x;
            var e=CurrentEncounter;
            return Clamp(x,e.leftX+.025f,e.rightX-.025f);
        }
        partial void ReconcileBelowDefaultEnvelopeExitV21(EncounterDefinition definition,ref float exit);
        partial void ReconcileCoreHardDefaultEnvelopeExitV29(EncounterDefinition definition,ref float exit);
        partial void ReconcileFactoryDefaultEnvelopeExitR1460(EncounterDefinition definition,ref float exit);
        // The presentation and the transition must agree, including linked airship decks.
        public float CurtainX { get { return HasEncounters ? EncounterExitContactX(CurrentEncounter) : ExitX; } }
        float EncounterExitContactX(EncounterDefinition definition) {
            float exit=definition.exitX;ReconcileCargoOriginalCurtainExitR1479(definition,ref exit);
            if(!NativeTerrain)return exit;
            // The source exporter can round the world envelope outwards while
            // retaining fractional native support endpoints. Only reconcile a
            // default envelope-derived exit within a quarter display pixel;
            // authored interior exits and real missing ground stay unchanged.
            float edge=definition.direction>0?definition.rightX-.05f:definition.leftX+.05f;
            if(Math.Abs(exit-edge)>.0001f)return exit;
            float reachable=NativeClampX(exit);
            if(Math.Abs(reachable-exit)<=.25f)return reachable;
            ReconcileBelowDefaultEnvelopeExitV21(definition,ref exit);
            ReconcileCoreHardDefaultEnvelopeExitV29(definition,ref exit);
            ReconcileFactoryDefaultEnvelopeExitR1460(definition,ref exit);
            return exit;
        }
        bool TickEncounters(float dt) {
            if(!HasEncounters)return false;
            bool combatClear=EncounterCombatClear;
            foreach(var enemy in enemies)if(enemy.active&&enemy.isGate&&enemy.encounterIndex==currentEncounter&&enemy.hp>0)enemy.invincible=combatClear?0:3600;
            var clause=ResolvedEncounterClause();gateOpen=clause!=null;
            if (!gateOpen || player.hp<=0) {
                AdvanceGateTransitionDelay(false, combatClear && RequiredExitGate()!=null, null, dt);
                return true;
            }
            var definition=CurrentEncounter;
            if(clause.transitions.Length==0)throw new InvalidOperationException("Cleared encounter has no original transition.");
            int sum=0;foreach(var edge in clause.transitions)sum=checked(sum+edge.weight);
            // Preview the same next draw during the wait; consume it only on
            // actual transition. Final victory edges need no opening delay.
            uint nextRandom=encounterRandom*1664525u+1013904223u;
            int draw=(int)(nextRandom%(uint)sum);EncounterTransition selected=null;
            foreach(var edge in clause.transitions){draw-=edge.weight;if(draw<0){selected=edge;break;}}
            if(selected==null)throw new InvalidOperationException("No weighted original transition.");
            bool showClear=!selected.completeDungeon && (clause.requiresExit || selected.targetRoom!=room);
            AdvanceGateTransitionDelay(!selected.completeDungeon,showClear,clause,dt,!selected.completeDungeon && selected.targetRoom!=room);
            bool needsExitContact=clause.requiresExit && !autoEnterAfterGateBreak;
            if(needsExitContact&&!CargoOriginalCurtainContactR1479())return true;
            if(needsExitContact){
                if(!Grounded(player))return true;
                float exitContact=EncounterExitContactX(definition);
                bool regionalEdge=CurrentRoom.id!=null && (CurrentRoom.id.StartsWith("velder-",StringComparison.Ordinal) ||
                    CurrentRoom.id.StartsWith("hamel-",StringComparison.Ordinal));
                bool sameRoomCurtain=!selected.completeDungeon && selected.targetRoom==room;
                if(NativeTerrain && (regionalEdge || (sameRoomCurtain && TouchesNativeCurtainRiser(exitContact,definition.direction))))
                    exitContact-=definition.direction*NativePlayerHalfWidth;
                if(definition.direction*(player.x-exitContact)<-.05f)return true;
            }
            if(!selected.completeDungeon&&GateTransitionDelayRemaining>.00001f)return true;
            encounterRandom=nextRandom;
            if(clause.mode==EncounterClearMode.SurviveTime)routeSurvivalSeconds+=clause.seconds;
            if(selected.completeDungeon){ResetGateTransitionDelay();FinishDungeonScore();CancelAction(true);player.clip="idle";player.clock=0;return true;}
            if(selected.targetRoom==room) {
                int previousEncounter=currentEncounter;BeginEncounter(selected.targetEncounter);ReconcileCargoLinkedEncounterEntryR1479(previousEncounter,selected.targetEncounter);
                // Preserve physical position, HP, MP and actors inside a world.
                // The character crosses the opened curtain instead of respawning.
            } else {
                pendingEncounterEntry=selected.targetEncounter;
                RecoverBetweenRooms(selected.targetRoom);
            }
            return true;
        }
    }
}
