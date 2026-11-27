#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class GuardAndTreePlayerCheckV35 {
        [Serializable] sealed class SizeSampleV39 {
            public string stage,npc,image,sourceSprite,sourceState;
            public int facing,groundLine,pixels;
            public bool flipX,actuallyShowingIdleZero;
            public float sourceBody,registeredIdleBody,sourceClock,gameY,groundedOffset,rootError;
        }
        [Serializable] sealed class SizeLogV39 {public List<SizeSampleV39> samples=new List<SizeSampleV39>();}
        readonly SizeLogV39 sizeLogV39=new SizeLogV39();
        static bool LegacySizeModeV39(string mode){return mode=="size-bethma-spear"||mode=="size-below-miner"||mode=="size-mine-fatb";}
        string SizeRouteV39(){return report.mode=="size-bethma-spear"?"dragon-road":report.mode=="size-below-miner"?"altera-below-tunnel":"bethma-richie-mine";}
        string SizeNpcV39(){return report.mode=="size-bethma-spear"?"NUI_LIZARDMAN_SPEAR":report.mode=="size-below-miner"?"NUI_NASOD_MINER_THIN_DEFECT":"NUI_NASOD_MINER_FAT_B";}
        string SizeFirstRoomV39(){return report.mode=="size-bethma-spear"?"dragon-road-1":report.mode=="size-below-miner"?"Dungeon_Altera_Below_Tunnel_Normal-0":"Dungeon_Besma_MINE2_Normal-0";}
        bool SizeRouteReadyV39(){return report.mode=="size-below-miner"?scene.BelowProductRouteActiveR1130:report.mode=="size-mine-fatb"?scene.Mine2ProductSessionActiveR1233:true;}
        SpriteRenderer SizeCarrierV39(){
            int slot=game.enemies.IndexOf(enemy);Need(slot>=0,"Original size owner is still in this room");
            return ((IList)Field(scene,"enemyViews"))[slot] as SpriteRenderer;
        }
        void SizeRowV39(Observation r){
            var carrier=SizeCarrierV39();Need(carrier&&carrier.sprite,"Actual legacy painted enemy sprite");
            r.sourceKey=carrier.sprite.name;r.sourceMotion=enemy.clip;r.sourceTime=enemy.clock;r.phase=enemy.npcState;
        }
        void SizeRoomV39(){
            int room=report.mode=="size-below-miner"?1:0,slot=report.mode=="size-bethma-spear"?2:report.mode=="size-below-miner"?7:0;
            Call(game,"EnterRoom",room);enemy=game.enemies[slot];
            if(game.EncounterIndex!=enemy.encounterIndex)Call(game,"BeginEncounter",enemy.encounterIndex);
            Call(scene,"RebuildRoom");Need(enemy.sourceNPC==SizeNpcV39()&&enemy.active&&!enemy.dormant&&enemy.visualScale==1,"Registered original stage birth "+slot);
            report.room=game.CurrentRoom.id;report.npc=enemy.sourceNPC;
        }
        void SizeShotV39(string label){
            var view=SizeCarrierV39();Need(view&&view.enabled&&!view.forceRenderingOff&&view.sprite,"Ordinary painted carrier");
            var banks=(IDictionary)Field(scene,"libraries");var bank=banks[enemy.kind];var data=(SpriteLibraryData)Field(bank,"data");
            var entry=LegacyBodyRegistrationV39.Find(enemy,SizeRouteV39(),data.artVersion);
            Need(entry!=null,"Scoped original size receipt");var idle=((Dictionary<string,Sprite[]>)Field(bank,"frames"))[entry.idle][0];
            Need(idle&&idle.rect.width==entry.width&&idle.rect.height==entry.height,"Same inspected native idle frame");
            var obs=Row(label);Capture(obs,true);Need(obs.callbacks==1&&obs.eligible==1&&obs.pixels>0,"Actual legacy actor D3D11 camera submission");
            // Transform two points on the registered neutral sprite through the
            // same active carrier that the world camera just submitted. Include
            // the actual selected sprite name when this is a moving or hurt pose.
            float centre=idle.rect.height*.5f;var top=view.transform.TransformPoint(new Vector3(0,centre-entry.top,0));
            var bottom=view.transform.TransformPoint(new Vector3(0,centre-entry.bottom,0));
            float observed=Math.Abs(top.y-bottom.y);
            float actualScale=view.transform.localScale.y,offset=entry.neutralFoot*(1-actualScale);
            var nativeAt=(Vector2)Call(scene,"RenderEnemyPosition",enemy);var paintOffset=(Vector2)Call(scene,"SpriteOffset",view);
            var actors=(Transform)Field(scene,"actorsRoot");var actorRoot=actors.TransformPoint(nativeAt+paintOffset*actualScale+Vector2.up*offset);
            var projected=(Vector3)Call(scene,"ProjectNativePoint",actorRoot,enemy.nativeDepth);var actual=view.transform.position;
            var sample=new SizeSampleV39{stage=game.CurrentRoom.id,npc=enemy.sourceNPC,image=obs.image,sourceSprite=view.sprite.name,sourceState=enemy.npcState,
                facing=enemy.facing,groundLine=enemy.groundLine,pixels=obs.pixels,flipX=view.flipX,actuallyShowingIdleZero=view.sprite==idle,
                sourceBody=entry.target,registeredIdleBody=observed,sourceClock=enemy.clock,gameY=enemy.y,groundedOffset=offset,rootError=Vector2.Distance(actual,projected)};
            sizeLogV39.samples.Add(sample);File.WriteAllText(Path.Combine(output,"legacy-body-source-registration.json"),JsonUtility.ToJson(sizeLogV39,true));
            Need(Math.Abs(entry.target-observed)<.15f&&Math.Abs(actualScale-entry.factor)<.0001f,"Native body height on the real carrier "+observed+" source "+entry.target);
            Need(sample.rootError<.15f,"Original projected root and standing sole registration "+sample.rootError);
            report.events.Add(label+" source="+view.sprite.name+" height="+observed+" native="+entry.target+" x/y="+enemy.x+"/"+enemy.y);
        }
        IEnumerable SizeWaitV39(Func<bool> test,float offset){
            var end=Time.realtimeSinceStartupAsDouble+38;
            for(;;){Protect(offset);yield return null;Budget();var v=SizeCarrierV39();
                Need(Time.realtimeSinceStartupAsDouble<end,"Actual size witness "+enemy.sourceNPC+" action="+enemy.npcState+" combat="+enemy.state+" x="+enemy.x+" facing="+enemy.facing);
                if(v&&v.enabled&&v.sprite&&!v.forceRenderingOff&&enemy.active&&test())yield break;
            }
        }
        IEnumerable LegacySizeFlowV39(){
            string route=SizeRouteV39();foreach(var x in Enter(report.mode=="size-below-miner"?"altera":"bethma",route))yield return x;
            foreach(int facing in new[]{1,-1}){
                SizeRoomV39();float distance=BendersChaseDistanceV36();foreach(var x in MakeBendersWalkingSpaceV36(facing,distance))yield return x;
                foreach(var x in SizeWaitV39(()=>enemy.facing==facing&&game.Grounded(enemy)&&enemy.state==CombatState.Normal&&!game.EnemyHasSuperArmor(enemy),facing*distance))yield return x;
                SizeShotV39("ground-"+facing);
            }
            float hp=enemy.hp;int dealt=game.ApplyHitToEnemy(enemy,1,HitReaction.Hurt);
            Need(dealt==1&&hp-enemy.hp==1&&enemy.state==CombatState.Hurt,"One original ground hurt packet and unchanged combat geometry");
            foreach(var x in SizeWaitV39(()=>enemy.state==CombatState.Hurt,35))yield return x;SizeShotV39("hurt");
            Need(game.ApplyHitToEnemy(enemy,1,HitReaction.Launch,280)>0&&enemy.state==CombatState.Airborne,"Original public launch");
            foreach(var x in SizeWaitV39(()=>enemy.state==CombatState.Airborne&&enemy.vy>0,35))yield return x;SizeShotV39("launch");
            foreach(var x in SizeWaitV39(()=>enemy.state==CombatState.Airborne&&enemy.vy<=0,35))yield return x;SizeShotV39("descent");
            foreach(var x in SizeWaitV39(()=>enemy.state==CombatState.Knockdown&&game.Grounded(enemy),35))yield return x;SizeShotV39("land");
        }
    }
}
#endif
