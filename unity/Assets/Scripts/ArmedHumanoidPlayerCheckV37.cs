#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class GuardAndTreePlayerCheckV35 {
        [Serializable] sealed class WeaponTrackV37 {public string motion;public int count,fps;public float[] values;}
        [Serializable] sealed class WeaponSeriesV37 {public string id,motionSha;public float[] tipLocal,forearmTipLocal;public WeaponTrackV37[] tracks;}
        [Serializable] sealed class WeaponSampleV37 {public string image,action;public float clock,gripError,tipError,handGripError;public int facing,pixels;}
        [Serializable] sealed class WeaponLogV37 {public List<WeaponSampleV37> samples=new List<WeaponSampleV37>();}
        WeaponSeriesV37 weaponSourceV37;
        readonly WeaponLogV37 weaponsV37=new WeaponLogV37();
        static bool ArmedModeV37(string mode){return mode=="armed-tree"||mode=="armed-cave"||mode=="armed-toto"||mode=="armed-soldier";}
        string ArmedNpcV37(){switch(report.mode){case "armed-tree":return "NUI_THIEF_THIN_ZERO";case "armed-cave":return "NUI_THIEF_THIN";
            case "armed-toto":return "NUI_THIEF_THIN_TOTO";default:return "NUI_SOLDIER_THIN";}}
        string ArmedRouteV37(){switch(report.mode){case "armed-tree":return "tree";case "armed-cave":return "banthus-cave";
            case "armed-toto":return "swamp";default:return "underground-waterway";}}
        void ArmedRoomV37(){
            Call(game,"EnterRoom",0);int slot=report.mode=="armed-tree"||report.mode=="armed-cave"?3:report.mode=="armed-toto"?2:0;
            enemy=game.enemies[slot];if(enemy.encounterIndex!=game.EncounterIndex)Call(game,"BeginEncounter",enemy.encounterIndex);
            Call(scene,"RebuildRoom");Need(enemy.sourceNPC==ArmedNpcV37()&&enemy.active&&!enemy.dormant&&enemy.visualScale==1,"Real staged armed actor");
            report.npc=enemy.sourceNPC;report.room=game.CurrentRoom.id;
        }
        List<SpriteRenderer> ArmedMembersV37(List<SpriteRenderer> group,out int count){
            var view=BendersOwnerV36();Need(view!=null&&group[0].forceRenderingOff,"Armed 14-part presenter and whole carrier");
            var members=(SpriteRenderer[])Field(view,"members");Need(members.Length==14&&members[13].name=="weapon","Original registered weapon child");
            group.AddRange(members);count=members.Length;return group;
        }
        void LoadArmedEvidenceV37(){
            string npc=ArmedNpcV37(),root=ArmedHumanoidPinsV37.Root(npc);
            var feet=Resources.Load<TextAsset>(root+"dash-proof");var weapon=Resources.Load<TextAsset>(root+"weapon-proof");
            Need(feet&&weapon&&ReusablePaintRigV37.Digest(weapon)==ArmedHumanoidPinsV37.WeaponProof(npc),"Pinned native weapon witnesses");
            bendersProofV36=JsonUtility.FromJson<BendersDashProofV36>(feet.text);weaponSourceV37=JsonUtility.FromJson<WeaponSeriesV37>(weapon.text);
            Need(bendersProofV36.fps==480&&bendersProofV36.values.Length==bendersProofV36.count*4&&weaponSourceV37.motionSha==bendersProofV36.motionSha
                &&weaponSourceV37.id==npc&&weaponSourceV37.tipLocal.Length==2,"Native independent feet and weapon geometry");
        }
        WeaponSampleV37 ArmedShotV37(string label){
            var shot=Row(label);Capture(shot,true);Need(shot.eligible==14&&shot.callbacks==14&&shot.pixels>0,"Fourteen genuine armed draws "+label);
            var members=(SpriteRenderer[])Field(BendersOwnerV36(),"members");var weapon=members[13];
            // Boots always use a source-axis holder beneath the signed view root.
            // Weapon may or may not require a holder depending on its paint axes.
            var root=members[2].transform.parent.parent;
            var pa=root.InverseTransformPoint(weapon.transform.TransformPoint(Vector3.zero));
            var pb=root.InverseTransformPoint(weapon.transform.TransformPoint(new Vector3(weaponSourceV37.tipLocal[0],weaponSourceV37.tipLocal[1],0)));
            var hand=root.InverseTransformPoint(members[12].transform.TransformPoint(new Vector3(weaponSourceV37.forearmTipLocal[0],weaponSourceV37.forearmTipLocal[1],0)));
            var tr=Array.Find(weaponSourceV37.tracks,t=>t.motion==shot.sourceMotion);
            Need(tr!=null&&tr.values.Length==tr.count*4,"Source grip and tip track "+shot.sourceMotion);
            float at=Mathf.Clamp(shot.sourceTime*tr.fps,0,tr.count-1);int a=Mathf.FloorToInt(at),b=Math.Min(a+1,tr.count-1);
            float V(int k){return Mathf.Lerp(tr.values[4*a+k],tr.values[4*b+k],at-a);}
            var sample=new WeaponSampleV37{action=shot.sourceMotion,image=shot.image,clock=shot.sourceTime,facing=enemy.facing,pixels=BootPixelsV36(weapon),
                gripError=Vector2.Distance(pa,new Vector2(V(0),V(1))),tipError=Vector2.Distance(pb,new Vector2(V(2),V(3))),handGripError=Vector2.Distance(hand,pa)};
            weaponsV37.samples.Add(sample);File.WriteAllText(Path.Combine(output,"armed-source-grip-tip.json"),JsonUtility.ToJson(weaponsV37,true));
            Need(sample.gripError<.08f&&sample.tipError<.08f&&sample.handGripError<.08f,"Actual hand, grip and tip to real native weapon "+sample.action+" grip="+sample.gripError+" tip="+sample.tipError+" hand="+sample.handGripError);
            report.events.Add(label+" alias="+shot.sourceKey+" actual="+shot.sourceMotion+" t="+shot.sourceTime+" weaponPixels="+sample.pixels);
            return sample;
        }
        IEnumerable ArmedStrideV37(int face){
            ArmedRoomV37();float distance=BendersChaseDistanceV36();
            foreach(var x in MakeBendersWalkingSpaceV36(face,distance))yield return x;
            bool[] phases=new bool[4],pixels=new bool[2];bool weaponVisible=false;int bins=0;float last=-5;double end=Time.realtimeSinceStartupAsDouble+48;
            while(bins<4||!pixels[0]||!pixels[1]||!weaponVisible){
                Protect(face*distance);yield return null;Budget();var view=BendersOwnerV36();
                Need(Time.realtimeSinceStartupAsDouble<end,"Original armed stride "+face+" bins="+bins+" feet="+pixels[0]+"/"+pixels[1]+" weapon="+weaponVisible+" state="+enemy.npcState+" x="+enemy.x);
                if(view==null||enemy.facing!=face)continue;var frame=((FairyAuthoredPartsV33)Field(view,"parts")).LastFrame;
                if(!frame.visible||frame.alias!=(report.mode=="armed-toto"?"Walk":"Dash"))continue;
                int bin=Math.Min(3,(int)(4*frame.sourceClock/bendersProofV36.duration));
                if(phases[bin]&&(pixels[0]&&pixels[1]&&weaponVisible||Math.Abs(last-frame.sourceClock)<.11f))continue;
                var w=ArmedShotV37("stride-"+face+"-"+bin+"-"+Time.frameCount);weaponVisible|=w.pixels>0;
                var shot=report.observations[report.observations.Count-1];var members=(SpriteRenderer[])Field(view,"members");
                var bank=((Dictionary<string,BendersFamilyRigV36>)Field(scene,"bendersBanksV36"))[enemy.sourceNPC];
                for(int i=0;i<2;i++){int k=i==0?2:9;var foot=BendersFootV36(shot,members[k],bank.lib.parts[k],bank.lib.UnitsPerPixel,i);pixels[i]|=foot.footPixels>0;}
                if(!phases[bin]){phases[bin]=true;bins++;}last=frame.sourceClock;
            }
        }
        IEnumerable ArmedFlowV37(){
            string route=ArmedRouteV37();bool toto=report.mode=="armed-toto";
            foreach(var x in Enter(route=="tree"||route=="swamp"?"ruben":"elder",route))yield return x;
            LoadArmedEvidenceV37();
            foreach(var x in ArmedStrideV37(1))yield return x;
            foreach(var x in ArmedStrideV37(-1))yield return x;
            ArmedRoomV37();
            foreach(var x in AwaitBendersV36(a=>a.StartsWith("source-attack",StringComparison.Ordinal)&&a.EndsWith("ready",StringComparison.Ordinal),28))yield return x;
            ArmedShotV37("attack-ready");
            foreach(var x in AwaitBendersV36(a=>a.StartsWith("source-attack",StringComparison.Ordinal)&&!a.EndsWith("ready",StringComparison.Ordinal),28))yield return x;
            ArmedShotV37("attack-fire");
            if(toto){
                NpcPresentationEvent spell=null;double end=Time.realtimeSinceStartupAsDouble+18;
                while(spell==null){
                    Protect(45);yield return null;Budget();Need(Time.realtimeSinceStartupAsDouble<end,"Original Toto spell spawn");
                    foreach(var e in game.npcPresentationEvents)if(e.owner==enemy&&e.type=="spawn"&&e.effect=="DamageEffect_THIEF_THIN_TOTO_MAGIC_ATTACK_A")spell=e;
                }
                report.events.Add("Original Toto spell serial="+spell.serial+" time="+spell.time+" world="+spell.x+","+spell.y+" facing="+spell.facing);
            }
            ArmedRoomV37();
            foreach(var x in AwaitBendersV36(a=>a=="source-walk"&&game.Grounded(enemy)&&!game.EnemyHasSuperArmor(enemy),BendersOpenChaseV36()))yield return x;
            Need(game.ApplyHitToEnemy(enemy,1,HitReaction.Hurt)>0&&enemy.state==CombatState.Hurt,"Grounded public hurt");
            foreach(var x in AwaitBendersV36(a=>a=="source-hurt",40))yield return x;ArmedShotV37("hurt");
            Need(game.ApplyHitToEnemy(enemy,1,HitReaction.Launch,280)>0&&enemy.state==CombatState.Airborne,"Public launch");
            foreach(var x in AwaitBendersV36(a=>(a=="source-air-up"||a=="source-launch"||a=="source-air-hit")&&enemy.vy>0,40))yield return x;ArmedShotV37("launch");
            foreach(var x in AwaitBendersV36(a=>(a=="source-air-fall"||a=="source-fall"||toto&&a=="source-air-hit")&&enemy.vy<=0,40))yield return x;ArmedShotV37("descending");
            foreach(var x in AwaitBendersV36(a=>a=="source-down",40))yield return x;ArmedShotV37("down");
            foreach(var x in AwaitBendersV36(a=>a=="source-get-up",40))yield return x;ArmedShotV37("get-up");
            foreach(var x in AwaitBendersV36(a=>a=="source-walk",BendersOpenChaseV36()))yield return x;
            Need(game.ApplyHitToEnemy(enemy,100000,HitReaction.None)>0&&enemy.hp==0,"Public lethal HP");
            foreach(var x in AwaitBendersV36(a=>a=="source-death",40))yield return x;ArmedShotV37("death");
        }
    }
}
#endif
