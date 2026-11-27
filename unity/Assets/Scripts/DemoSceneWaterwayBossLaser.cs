using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        readonly LineRenderer[] waterwayLaserBands=new LineRenderer[3];
        readonly LineRenderer[] waterwayLaserMuzzles=new LineRenderer[3];
        readonly LineRenderer[] castleBossBeamBands=new LineRenderer[3];
        LineRenderer castleBossMuzzle;

        void RenderCastleBossBeam(){
            NpcEffectInstance beam=null;
            foreach(var effect in game.npcEffects){
                if(game.IsCastleBossBeam(effect)&&!effect.ended){beam=effect;break;}
            }
            if(beam==null){
                foreach(var band in castleBossBeamBands)if(band)band.enabled=false;
                if(castleBossMuzzle)castleBossMuzzle.enabled=false;
                return;
            }
            var line=game.NpcEffectLines(beam);
            float depth=beam.owner.nativeDepth;
            var from=ProjectNativePoint(new Vector3(line[0],line[1],0),depth);
            var to=ProjectNativePoint(new Vector3(line[3],line[4],0),depth);
            var top=ProjectNativePoint(new Vector3(line[0],line[1]+GameModel.CastleBossBeamHalfHeight,0),depth);
            float width=2f*(top.y-from.y);
            float fade=Mathf.Clamp01((beam.lifetime-beam.age)/.15f);
            float pulse=1f+.10f*Mathf.Sin(beam.age*72f);
            bool hasSpan=line[0]!=line[3];
            for(int i=0;i<castleBossBeamBands.Length;i++){
                var band=castleBossBeamBands[i];
                Color color=i==0?new Color32(233,80,39,215):i==1?new Color32(255,166,68,255):new Color32(255,244,200,255);
                if(!band){
                    band=castleBossBeamBands[i]=CombatLine("Castle boss beam",1,color);
                    band.positionCount=2;
                    band.sortingOrder=43+i;
                }
                color.a*=fade;
                band.enabled=hasSpan;
                band.startColor=band.endColor=color;
                band.startWidth=band.endWidth=width*(i==0?1f:i==1?.69f*pulse:.29f*pulse);
                band.SetPosition(0,from);
                band.SetPosition(1,to);
            }
            if(!castleBossMuzzle){
                castleBossMuzzle=CombatLine("Castle boss emitter flash",1,Color.white);
                castleBossMuzzle.positionCount=2;
                castleBossMuzzle.sortingOrder=46;
            }
            castleBossMuzzle.enabled=hasSpan;
            castleBossMuzzle.startColor=new Color(1,.96f,.73f,fade);
            castleBossMuzzle.endColor=new Color(1,.92f,.61f,0);
            castleBossMuzzle.startWidth=width*.98f;
            castleBossMuzzle.endWidth=width*.64f;
            castleBossMuzzle.SetPosition(0,from);
            float beamSpan=Vector3.Distance(from,to);
            castleBossMuzzle.SetPosition(1,beamSpan>34f?Vector3.Lerp(from,to,34f/beamSpan):to);
        }

        void HideWaterwayBossLaser(){
            foreach(var band in waterwayLaserBands)if(band)band.enabled=false;
            foreach(var muzzle in waterwayLaserMuzzles)if(muzzle)muzzle.enabled=false;
        }

        Vector3 WaterwayLaserPoint(NpcEffectInstance laser,float[] line,float t){
            float depth=laser.owner.nativeDepth+Mathf.Lerp(line[2],line[5],t)/(NativeWorldActive?nativeScene.unit:.28f);
            return ProjectNativePoint(new Vector3(Mathf.Lerp(line[0],line[3],t),Mathf.Lerp(line[1],line[4],t),0),depth);
        }

        void RenderWaterwayBossLaser(){
            NpcEffectInstance laser=null;
            if(selectedDungeon==5&&game!=null&&game.room==2){
                foreach(var effect in game.npcEffects){
                    if(effect.id=="BENDERS_NASWORD_SPECIAL_ATTACK_A"&&effect.owner!=null&&
                       effect.owner.sourceNPC=="NUI_BENDERS_NASOD"&&!effect.ended&&effect.age<effect.lifetime){laser=effect;break;}
                }
            }
            var line=laser==null?null:game.NpcEffectLines(laser);
            if(line==null||line.Length!=6||
               !monsterPixelSelectionsV53.TryGetValue(laser.owner,out var pose)||pose.motion!="SpecialAttackA_Fire"||
               !monsterPixelViewsV53.TryGetValue(laser.owner,out var view)||!view.renderer||!view.renderer.enabled){
                HideWaterwayBossLaser();
                return;
            }

            // The actor is a billboard, projected at the feet, then posed in screen space.
            // Its pupil must follow that exact sprite as the camera changes perspective.
            WaterwayNasodEye.FireOffset(pose.clock,out float irisX,out float irisY);
            var pupil=view.renderer.transform.TransformPoint(new Vector3(irisX,irisY,0));
            float dx=line[3]-line[0];
            float end=dx<0?Mathf.Clamp01(-line[0]/dx):dx>0?Mathf.Clamp01((game.RoomWidth-line[0])/dx):1;
            float length=Mathf.Sqrt(dx*dx+(line[4]-line[1])*(line[4]-line[1]));
            float near=Mathf.Min(end,12f/Mathf.Max(1f,length));
            var neck=WaterwayLaserPoint(laser,line,near);
            int segments=Mathf.Clamp(Mathf.CeilToInt(Mathf.Abs(dx)*(end-near)/32f),1,48);
            float pulse=Mathf.Sin(laser.age*53f);
            for(int layer=0;layer<waterwayLaserBands.Length;layer++){
                var band=waterwayLaserBands[layer];
                var muzzle=waterwayLaserMuzzles[layer];
                if(!band){
                    var color=layer==0?new Color32(87,52,192,230):layer==1?new Color32(73,209,255,255):new Color32(238,255,255,255);
                    band=waterwayLaserBands[layer]=CombatLine("Waterway eye beam "+layer,1,color);
                    muzzle=waterwayLaserMuzzles[layer]=CombatLine("Waterway iris "+layer,1,color);
                    band.sortingOrder=muzzle.sortingOrder=43+layer;
                }
                band.enabled=muzzle.enabled=true;
                band.positionCount=segments+1;
                muzzle.positionCount=2;
                float width=layer==0?7.2f+pulse:layer==1?4.3f+pulse*.4f:1.7f;
                band.startWidth=muzzle.endWidth=width;
                band.endWidth=width*.72f;
                muzzle.startWidth=layer==0?1.3f:layer==1?1f:.7f;
                muzzle.SetPosition(0,pupil);
                muzzle.SetPosition(1,neck);
            }
            for(int i=0;i<=segments;i++){
                var point=WaterwayLaserPoint(laser,line,Mathf.Lerp(near,end,(float)i/segments));
                foreach(var band in waterwayLaserBands)band.SetPosition(i,point);
            }
        }
    }
}
