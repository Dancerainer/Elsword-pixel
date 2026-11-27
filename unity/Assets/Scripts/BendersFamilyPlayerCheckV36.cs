#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class GuardAndTreePlayerCheckV35 {
        [Serializable] sealed class BendersDashProofV36 { public string motionSha;public int fps,count;public float duration; public float[] values; }
        [Serializable] sealed class BendersFootFrameV36 {
            public int facing,side,footPixels;public float clock,x,y,originalX,originalY;public string image;
        }
        [Serializable] sealed class BendersFootReportV36 {
            public List<BendersFootFrameV36> frames=new List<BendersFootFrameV36>();
        }
        readonly BendersFootReportV36 bendersFeetV36=new BendersFootReportV36();
        BendersDashProofV36 bendersProofV36;
        static bool BendersModeV36(string mode){return mode=="benders-tree"||mode=="benders-swamp"||mode=="benders-cave"||mode=="benders-waterway";}
        string BendersNpcV36(){switch(report.mode){case "benders-tree":return "NUI_BENDERS_NORMAL";case "benders-swamp":return "NUI_BENDERS_THIEF";
            case "benders-cave":return "NUI_BENDERS_UPGRADE_NORMAL";default:return "NUI_BENDERS_NASOD";}}
        string BendersRouteV36(){switch(report.mode){case "benders-tree":return "tree";case "benders-swamp":return "swamp";case "benders-cave":return "banthus-cave";default:return "underground-waterway";}}
        static string BendersFirstRoomV36(string route){switch(route){case "tree":return "tree-1";case "swamp":return "swamp-1";case "banthus-cave":return "cave-1";case "underground-waterway":return "waterway-1";default:return "ruins-1";}}
        object BendersOwnerV36(){
            var d=(IDictionary)Field(scene,"bendersOwnersV36");return d.Contains(enemy)?d[enemy]:null;
        }
        List<SpriteRenderer> BendersPartsV36(List<SpriteRenderer> group,out int count){
            var view=BendersOwnerV36();Need(view!=null,"Registered ordinary Benders owner");group.AddRange((SpriteRenderer[])Field(view,"members"));
            count=group.Count-1;Need(count==(report.mode=="benders-swamp"?14:15)&&group[0].forceRenderingOff,"Family registered count and hidden whole");
            return group;
        }
        void BendersRowV36(Observation row){
            var view=BendersOwnerV36();Need(view!=null,"Registered selected Benders");var parts=(FairyAuthoredPartsV33)Field(view,"parts");
            row.sourceMotion=parts.LastFrame.alias;row.sourceTime=parts.LastFrame.sourceClock;row.sourceKey=(string)Field(view,"alias");row.phase=enemy.clip;
        }
        void BendersRoomV36(int swampIndex=1){
            bool swamp=report.mode=="benders-swamp";Call(game,"EnterRoom",swamp?0:2);
            int index=swamp?swampIndex:report.mode=="benders-tree"?0:2;enemy=game.enemies[index];
            if(game.EncounterIndex!=enemy.encounterIndex)Call(game,"BeginEncounter",enemy.encounterIndex);
            Call(scene,"RebuildRoom");
            Need(enemy.sourceNPC==BendersNpcV36()&&enemy.active&&!enemy.dormant&&enemy.visualScale==1,"Specific ordinary staged family birth");
            report.npc=enemy.sourceNPC;report.room=game.CurrentRoom.id;
        }
        Observation BendersShotV36(string label){
            var row=Row(label);Capture(row,true);
            Need(row.eligible==14&&row.callbacks==14&&row.pixels>0,"Fourteen visible actual family GPU draws "+label);
            var members=(SpriteRenderer[])Field(BendersOwnerV36(),"members");
            Need(members[2].name=="far-boot"&&members[9].name=="near-boot","Independent left and right boots");
            string weapon=members.Length==14?members[13].name:members[members[13].enabled?13:14].name;
            if(members.Length==15)Need(members[13].enabled!=members[14].enabled,"One real cleaver projection");
            report.events.Add(label+" original="+row.sourceKey+" source="+row.sourceMotion+" t="+row.sourceTime.ToString("R")+" weapon="+weapon);
            return row;
        }
        IEnumerable AwaitBendersV36(Predicate<string> predicate,float distance,double seconds=28){
            double end=Time.realtimeSinceStartupAsDouble+seconds;
            for(;;){
                Protect(distance);yield return null;Budget();
                var view=BendersOwnerV36();string alias=view==null?"":(string)Field(view,"alias");
                Need(Time.realtimeSinceStartupAsDouble<end,"Requested real family state; actual alias="+alias+" npc="+enemy.npcState+" clip="+enemy.clip+
                    " actor/player="+enemy.x+"/"+game.player.x+" line="+enemy.groundLine+" sourceTime="+enemy.npcTime);
                if(view!=null&&predicate(alias))yield break;
            }
        }
        float BendersChaseDistanceV36(){
            var profile=(NpcDefinition)Call(game,"NpcProfile",enemy);
            return profile==null?150:Mathf.Max(150,profile.approachDistance+64);
        }
        float BendersAvailableV36(int facing){
            var area=game.CurrentEncounter;
            return facing>0?(area!=null?area.rightX:game.CurrentRoom.width)-enemy.x:enemy.x-(area!=null?area.leftX:0);
        }
        float BendersOpenChaseV36(){
            int side=BendersAvailableV36(1)>BendersAvailableV36(-1)?1:-1;
            return side*BendersChaseDistanceV36();
        }
        IEnumerable WalkToWaterwayOpenLineV36(float limit){
            if(report.mode!="benders-waterway")yield break;
            // The original foreground wall covers this boss near its birth.
            // Walk from that birth onto the central, elevated line 2 before
            // requiring on/off pixels. The stage, depth mask and enemy stay live.
            double end=Time.realtimeSinceStartupAsDouble+42;
            while(enemy.x>limit){
                Protect(-BendersChaseDistanceV36());yield return null;Budget();
                Need(Time.realtimeSinceStartupAsDouble<end,"Natural waterway walk to the open line x="+enemy.x+" line="+enemy.groundLine);
            }
            Need(enemy.groundLine==2,"Original central Waterway line for evidence "+enemy.groundLine);
            report.events.Add("Waterway open-line witness x="+enemy.x+" y="+enemy.y+" originalLine="+enemy.groundLine);
        }
        IEnumerable MakeBendersWalkingSpaceV36(int facing,float distance){
            // The Waterway boss starts on line seven with only 110.6 units
            // to the right boundary. Let the real AI pursue into the room
            // before reversing the player; the actor is never relocated.
            float before=enemy.x;double end=Time.realtimeSinceStartupAsDouble+30;
            while(BendersAvailableV36(facing)<distance+82){
                Protect(-facing*distance);yield return null;Budget();
                Need(enemy.hp>0&&Time.realtimeSinceStartupAsDouble<end,"Natural reposition for Benders gait "+facing+
                    " actor/player="+enemy.x+"/"+game.player.x+" source="+enemy.npcState+" line="+enemy.groundLine);
            }
            if(Math.Abs(before-enemy.x)>.01f)report.events.Add("Original AI pursuit before facing "+facing+" from "+before+" to "+enemy.x+
                " line="+enemy.groundLine+" chaseOffset="+distance);
        }
        BendersFootFrameV36 BendersFootV36(Observation shot,SpriteRenderer boot,FairyPartLayoutV33 part,float unit,int side){
            var root=boot.transform.parent.parent;var h=part.visibleHull;
            var xy=new Vector2[h.Length/2];float low=float.PositiveInfinity;
            for(int i=0;i<xy.Length;i++){
                var world=boot.transform.TransformPoint(new Vector3((boot.flipX?-1:1)*h[2*i],h[2*i+1],0)*unit);
                var point=root.InverseTransformPoint(world);xy[i]=new Vector2(point.x*enemy.facing,point.y);low=Math.Min(low,point.y);
            }
            double mass=0,sum=0;
            foreach(var point in xy){double pressure=Math.Exp(-(point.y-low)/.22);mass+=pressure;sum+=pressure*point.x;}
            float at=Mathf.Clamp(shot.sourceTime*bendersProofV36.fps,0,bendersProofV36.count-1);
            int a=Mathf.FloorToInt(at),b=Math.Min(a+1,bendersProofV36.count-1),k=side*2;
            var f=new BendersFootFrameV36{facing=enemy.facing,side=side,clock=shot.sourceTime,x=(float)(sum/mass),y=low,image=shot.image,
                originalX=enemy.facing*Mathf.Lerp(bendersProofV36.values[4*a+k],bendersProofV36.values[4*b+k],at-a),
                originalY=Mathf.Lerp(bendersProofV36.values[4*a+k+1],bendersProofV36.values[4*b+k+1],at-a),
                footPixels=BootPixelsV36(boot)};
            bendersFeetV36.frames.Add(f);
            File.WriteAllText(Path.Combine(output,"benders-independent-feet.json"),JsonUtility.ToJson(bendersFeetV36,true));
            Need(Math.Abs(f.y-f.originalY)<.5f,"Independent original foot height "+f.clock+" side "+side+" actual "+f.y+" expected "+f.originalY);
            Need(Math.Abs(f.x-f.originalX)<.5f,"Independent original foot contact X "+f.clock+" side "+side+" actual "+f.x+" expected "+f.originalX);
            return f;
        }
        IEnumerable BendersWalkingV36(int side,int index){
            BendersRoomV36(index);bool[] phases=new bool[4],pixels=new bool[2];int bins=0;
            foreach(var x in WalkToWaterwayOpenLineV36(side>0?550:640))yield return x;
            float distance=BendersChaseDistanceV36();
            foreach(var x in MakeBendersWalkingSpaceV36(side,distance))yield return x;
            double end=Time.realtimeSinceStartupAsDouble+35;float last=-10;
            while(bins<4||!pixels[0]||!pixels[1]){
                Protect(side*distance);yield return null;Budget();
                Need(Time.realtimeSinceStartupAsDouble<end,"Original Dash, each foot visible, facing "+side+" bins="+bins+
                    " foot draws="+pixels[0]+"/"+pixels[1]+" source="+enemy.npcState+" hp="+enemy.hp);
                var view=BendersOwnerV36();if(view==null||enemy.facing!=side)continue;
                var part=(FairyAuthoredPartsV33)Field(view,"parts");var f=part.LastFrame;
                if(!f.visible||f.alias!="Dash")continue;
                int bin=Math.Min(3,(int)(4*f.sourceClock/bendersProofV36.duration));
                if(phases[bin]&&(pixels[0]&&pixels[1]||Math.Abs(last-f.sourceClock)<.11f))continue;
                var shot=BendersShotV36("dash-"+index+"-"+side+"-"+bin+"-"+Time.frameCount);
                var bank=((Dictionary<string,BendersFamilyRigV36>)Field(scene,"bendersBanksV36"))[enemy.sourceNPC];
                var members=(SpriteRenderer[])Field(view,"members");
                for(int s=0;s<2;s++){int n=s==0?2:9;var r=BendersFootV36(shot,members[n],bank.lib.parts[n],bank.lib.UnitsPerPixel,s);pixels[s]|=r.footPixels>0;}
                if(!phases[bin]){phases[bin]=true;bins++;}last=f.sourceClock;
            }
        }
        IEnumerable BendersFlowV36(){
            foreach(var x in Enter(report.mode=="benders-tree"||report.mode=="benders-swamp"?"ruben":"elder",BendersRouteV36()))yield return x;
            var file=Resources.Load<TextAsset>(BendersFamilyPinsV36.Root(BendersNpcV36())+"dash-proof");
            Need(file,"Independent native model foot witness");bendersProofV36=JsonUtility.FromJson<BendersDashProofV36>(file.text);
            Need(bendersProofV36.fps==480&&bendersProofV36.values.Length==4*bendersProofV36.count,"480 Hz per-foot source witness");
            foreach(var x in BendersWalkingV36(1,1))yield return x;
            foreach(var x in BendersWalkingV36(-1,report.mode=="benders-swamp"?7:1))yield return x;
            BendersRoomV36();
            foreach(var x in WalkToWaterwayOpenLineV36(605))yield return x;
            // Generic masked Swamp thieves acquire melee below 43; the Tree
            // boss uses 85. Real profiled Cave/Waterway A acquires at 168.
            float attackDistance=report.mode=="benders-swamp"?28:50;
            foreach(var x in AwaitBendersV36(a=>a.StartsWith("source-attack",StringComparison.Ordinal)&&a.EndsWith("ready",StringComparison.Ordinal),attackDistance))yield return x;
            BendersShotV36("attack-ready");
            foreach(var x in AwaitBendersV36(a=>a.StartsWith("source-attack",StringComparison.Ordinal)&&!a.EndsWith("ready",StringComparison.Ordinal),attackDistance))yield return x;
            BendersShotV36("attack-fire");
            foreach(var x in AwaitBendersV36(a=>a=="source-walk",BendersOpenChaseV36()))yield return x;
            Need(game.ApplyHitToEnemy(enemy,1,HitReaction.Hurt)>0,"Original public hurt HP");
            foreach(var x in AwaitBendersV36(a=>a=="source-hurt",50))yield return x;BendersShotV36("hurt");
            Need(game.ApplyHitToEnemy(enemy,1,HitReaction.Launch,280)>0,"Original public launch");
            foreach(var x in AwaitBendersV36(a=>a=="source-air-up"||a=="source-launch",50))yield return x;BendersShotV36("launch");
            foreach(var x in AwaitBendersV36(a=>a=="source-air-fall"||a=="source-fall",50))yield return x;BendersShotV36("fall");
            foreach(var x in AwaitBendersV36(a=>a=="source-down",50))yield return x;BendersShotV36("down");
            foreach(var x in AwaitBendersV36(a=>a=="source-get-up",50))yield return x;BendersShotV36("get-up");
            foreach(var x in AwaitBendersV36(a=>a=="source-walk",BendersOpenChaseV36()))yield return x;
            Need(game.ApplyHitToEnemy(enemy,100000,HitReaction.None)>0&&enemy.hp==0,"Public fatal packet");
            foreach(var x in AwaitBendersV36(a=>a=="source-death",50))yield return x;BendersShotV36("death");
        }
    }
}
#endif
