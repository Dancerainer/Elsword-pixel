#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class CoreReactionPlayerCheckV32 {
        [Serializable] sealed class OriginalHitSampleV45 {
            public string npc,sourceState,sourceMotion,actualMotion,caseName;
            public int code,damage,visiblePixels;
            public bool rear;
        }
        [Serializable] sealed class OriginalHitReportV45 {
            public bool returnedMain;
            public List<OriginalHitSampleV45> samples=new List<OriginalHitSampleV45>();
        }
        IEnumerator CheckOriginalHitsV45(){
            var proof=new OriginalHitReportV45();
            int[] codes={1,2,5,5,4,3,3};
            bool[] rear={false,false,false,true,false,false,true};
            string[] wanted={"DamageSmall","DamageBig","DamageAirFlyFront","DamageAirFlyBack","DamageAirUp","DamageDownFront","DamageDownBack"};
            for(int i=0;i<codes.Length;i++){
                int code=codes[i];string tag=(i+1).ToString("D2")+"-"+wanted[i];
                current=new CaseRow{npc=Npcs[2]};report.cases.Add(current);Stage(2);
                double until=Time.realtimeSinceStartupAsDouble+25;
                do{yield return null;Check();Need(Time.realtimeSinceStartupAsDouble<until,"Original elite must be hittable "+tag);}while(!CanHit()||!ReadyOnscreen());
                float x=target.x+target.facing*(rear[i]?-45:45);
                var reaction=code==5||code==4?HitReaction.Launch:code==3?HitReaction.Knockdown:HitReaction.Hurt;
                int damage=(int)Call(game,"ApplyHitToEnemyFromXR1417",target,2,reaction,280f,x,code);
                Need(damage>0&&target.originalHitCodeV45==code&&target.originalHitBackV45==rear[i],"Original hit accepted "+tag);
                RedrawCurrentFrameV44();
                var views=(System.Collections.IDictionary)Field(scene,"eliteViewsV36");
                Need(views.Contains(target),"Elite physical body rendered "+tag);
                string actual=(string)Field(views[target],"motion");
                string sourceState=target.npcState;
                if(string.IsNullOrEmpty(sourceState)&&OriginalReactionV45.TryState(target,out var pose))sourceState=pose.state;
                Need(!string.IsNullOrEmpty(sourceState)&&actual==wanted[i],"Selected real body "+actual+" / expected "+wanted[i]);
                var frame=Observe(tag,true);
                proof.samples.Add(new OriginalHitSampleV45{npc=target.sourceNPC,caseName=tag,code=code,damage=damage,rear=rear[i],
                    sourceState=sourceState,sourceMotion=wanted[i],actualMotion=actual,visiblePixels=frame.changedBodyPixels});
                current.completed=frame.changedBodyPixels>0;
                File.WriteAllText(Path.Combine(output,"original-hits.json"),JsonUtility.ToJson(proof,true));
            }
            target=null;Press("dungeons");for(int i=0;i<4;i++)yield return null;
            proof.returnedMain=!scene.CoreHardProductSessionActiveR1147&&(string)Field(scene,"openMenu")=="dungeons";
            Need(proof.returnedMain,"Ordinary Core menu after directed reactions");report.returnedMain=true;
            File.WriteAllText(Path.Combine(output,"original-hits.json"),JsonUtility.ToJson(proof,true));
        }
    }
}
#endif
