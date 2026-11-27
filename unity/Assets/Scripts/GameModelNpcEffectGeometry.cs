using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        bool UsesWaterwayNasodEye(Enemy owner,string effect){
            return owner!=null&&owner.sourceNPC=="NUI_BENDERS_NASOD"&&
                owner.npcState=="BENDERS_NASOD_SPECIAL_ATTACK_A"&&CurrentRoom.id=="waterway-3"&&
                (effect=="BENDERS_NASWORD_SPECIAL_ATTACK_A"||effect=="BENDERSNASOD_LASER_ATTACK_INVISIBLE");
        }

        static void ValidateNpcEffectGeometry(NpcEffectDefinition d){
            // Missing arrays can be null under JsonUtility. An authored array
            // containing an empty nested object is invalid, never skipped.
            if(d.geometrySamples==null)d.geometrySamples=new NpcEffectGeometrySample[0];
            if(d.geometrySamples.Length==0)return;
            if(d.geometrySamples.Length<2)throw new ArgumentException("Incomplete NPC effect geometry motion");
            float previous=-1;int lines=-1;
            foreach(var s in d.geometrySamples){
                if(s==null||!Finite(s.time)||s.time<0||s.time<=previous||s.spheres==null||s.spheres.Length!=d.baseSpheres.Length)throw new ArgumentException("Invalid NPC effect geometry sample");
                if(s.attackLines==null)s.attackLines=new float[0];ValidateNpcLines(s.attackLines);
                if(lines>=0&&lines!=s.attackLines.Length)throw new ArgumentException("Inconsistent NPC effect line geometry");lines=s.attackLines.Length;
                for(int i=0;i<s.spheres.Length;i++)if(!Finite(s.spheres[i])||i%4==3&&s.spheres[i]<=0)throw new ArgumentException("Invalid animated NPC effect helper");
                previous=s.time;
            }
            if(Math.Abs(d.geometrySamples[0].time)>.00001f||Math.Abs(previous-d.lifetime)>.00001f)throw new ArgumentException("NPC geometry motion must span the effect lifetime");
        }
        static float NpcEffectGeometryValue(NpcEffectInstance e,bool line,int index){
            var samples=e.definition.geometrySamples;if(samples==null||samples.Length==0)return e.definition.baseSpheres[index];
            int lo=0,hi=samples.Length;while(lo<hi){int mid=(lo+hi)/2;if(samples[mid].time<=e.age)lo=mid+1;else hi=mid;}
            int a=Math.Max(0,lo-1),b=Math.Min(a+1,samples.Length-1);float span=samples[b].time-samples[a].time,u=span<=0?0:Clamp((e.age-samples[a].time)/span,0,1);
            var p=line?samples[a].attackLines:samples[a].spheres;var q=line?samples[b].attackLines:samples[b].spheres;return p[index]+(q[index]-p[index])*u;
        }
        float[] NpcEffectLines(NpcEffectInstance e,EffectPose pose){
            if(IsCastleBossBeam(e))return CastleBossBeamLine(e);
            var samples=e.definition.geometrySamples;int count=samples==null||samples.Length==0?0:samples[0].attackLines.Length;
            var result=new float[count];float cs=(float)Math.Cos(e.yaw),sn=(float)Math.Sin(e.yaw);
            for(int i=0;i<count;i+=3){
                float x=NpcEffectGeometryValue(e,true,i)*pose.x*e.scaleX,z=NpcEffectGeometryValue(e,true,i+2)*pose.z*e.scaleZ;
                float relativeX=e.facing*(x*cs+z*sn)*e.ownerScale,relativeY=NpcEffectGeometryValue(e,true,i+1)*pose.y*e.scaleY*e.ownerScale;
                ApplyNpcSourceEffectRotationV18(e,ref relativeX,ref relativeY);
                result[i]=e.x+relativeX;
                result[i+1]=e.y+relativeY;
                result[i+2]=e.z+(-x*sn+z*cs)*e.ownerScale;
            }
            if(count==6&&UsesWaterwayNasodEye(e.owner,e.id)){
                // The source line has a moving local start of its own. Keep its full
                // animated vector, with the first damaging point exactly at the eye.
                result[3]=e.x+(result[3]-result[0]);
                result[4]=e.y+(result[4]-result[1]);
                result[5]=e.z+(result[5]-result[2]);
                result[0]=e.x;result[1]=e.y;result[2]=e.z;
            }
            return result;
        }
        public float[] NpcEffectLines(NpcEffectInstance e){return NpcEffectLines(e,EffectAt(e.definition,e.age));}
        void ContactNpcEffectLines(NpcEffectInstance e,EffectPose pose,float at){
            var damage=NpcEffectDamageViewBelowLaserV20(e);var lines=NpcEffectLines(e,pose);
            float verticalRadius=IsCastleBossBeam(e)?CastleBossBeamHalfHeight:0;
            for(int i=0;i<lines.Length;i+=6){
                if(!e.reflected){
                    if(verticalRadius>0&&lines[i]==lines[i+3])continue;
                    if(!NpcLineHitsPlayer(lines[i],lines[i+1],lines[i+2],lines[i+3],lines[i+4],lines[i+5],player,verticalRadius))continue;
                    if(!string.IsNullOrEmpty(damage.contactChild)){EndNpcEffect(e,"contact",at);return;}
                    float belowContactX=player.x,belowContactY=player.y;int dealt=ApplyEnemyAttackToPlayer((int)Math.Round(e.physicalAttack*damage.physical+e.magicAttack*damage.magic),e.x-e.facing,damage.reaction,damage.launch);
                    if(dealt>0){e.nextPlayerHit=damage.repeat?at+damage.repeatGap:float.PositiveInfinity;if(!PlayerHasSuperArmor&&player.state!=CombatState.Knockdown&&player.state!=CombatState.GettingUp)player.vx=e.facing*damage.push;hitstop=Math.Max(hitstop,damage.stopDefender);if(e.owner!=null&&e.owner.hp>0)e.owner.hitPause=Math.Max(e.owner.hitPause,damage.stopAttacker);TraceNpcEffect("hit-player",e,dealt,at);ApplyNpcPlayerStatus(e.definition,at);ApplyPlainOriginalFxPlayerStatusR1304(e,at);RecordBelowLineImpactPresentationV21(e,at,lines,i,belowContactX,belowContactY);RecordFactorySheriffLineImpactV32(e,at,lines,i,belowContactX,belowContactY);}return;
                }
                foreach(var enemy in enemies){
                    float next;if(!enemy.active||enemy.dormant||enemy.passive||enemy.hp<=0||e.nextEnemyHit.TryGetValue(enemy,out next)&&at<next||!NpcLineHitsEnemy(lines[i],lines[i+1],lines[i+2],lines[i+3],lines[i+4],lines[i+5],enemy))continue;
                    if(!string.IsNullOrEmpty(damage.contactChild)){EndNpcEffect(e,"contact",at);return;}
                    int dealt=ApplySourceHit(enemy,(int)Math.Round(e.physicalAttack*damage.physical+e.magicAttack*damage.magic),damage.reaction,damage.push,damage.launch,0,0,damage.stopDefender);
                    if(dealt>0){e.nextEnemyHit[enemy]=damage.repeat?at+damage.repeatGap:float.PositiveInfinity;TraceNpcEffect("hit-enemy",e,dealt,at);}
                }
            }
        }
    }
}
