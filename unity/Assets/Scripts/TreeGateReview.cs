#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        static void ReviewTreeGates(DemoScene scene,Action<bool,string> need) {
            scene.ConfigureRooms();var g=scene.game;
            var enter=typeof(GameModel).GetMethod("EnterRoom",BindingFlags.NonPublic|BindingFlags.Instance);
            for(int room=0;room<2;room++) {
                enter.Invoke(g,new object[]{room});g.player.invincible=999;
                var gate=g.UnbrokenExitGate;
                need(gate!=null,"Room lacks a damageable exit gate "+room);
                need(g.ExitGateProtected(gate),"Gate vulnerable before clear");
                float hp=gate.hp;need(g.ApplyHitToEnemy(gate,9999)==0&&gate.hp==hp,"Protected gate took damage");
                foreach(var enemy in g.enemies)if(!enemy.passive&&!enemy.isGate&&!GameModel.IsRecoveryContainer(enemy))enemy.hp=0;
                // Optional live objects must not lock an otherwise cleared room.
                var optional=g.enemies.Find(e=>!e.passive&&!e.isGate&&!GameModel.IsRecoveryContainer(e));
                optional.hp=10;optional.countsForClear=false;optional.aiEnabled=false;
                g.Step(1f/60,new Input());
                need(g.RoomCombatClear&&!g.gateOpen&&gate.hp==hp,"Clear auto-broke gate or optional enemy blocked clear");
                need(!g.ExitGateProtected(gate)&&g.Status()=="BREAK THE GATE","Cleared gate is still protected");
                for(int i=0;i<120;i++)g.Step(1f/60,new Input());
                need(g.room==room&&gate.hp==hp&&g.GateTransitionDelayRemaining==0,"Waiting auto-broke gate or armed early delay");
                g.player.x=g.ExitX;g.player.y=g.GroundAt(g.player.x);g.player.vy=0;
                need(g.ApplyHitToEnemy(gate,9999)>0&&gate.hp==0,"Cleared gate resisted attack");
                g.Step(1f/60,new Input());
                need(g.gateOpen&&g.GateTransitionDelayRemaining>.85f,"Gate break did not start delay");
                for(int i=0;i<35;i++)g.Step(1f/60,new Input());
                need(g.room==room,"Changed room before gate break delay");
                for(int i=0;i<85&&g.room==room;i++)g.Step(1f/60,new Input());
                need(g.room==room+1,"Broken gate did not lead to next room");
            }
            scene.ConfigureRooms();
        }
    }
}
#endif
