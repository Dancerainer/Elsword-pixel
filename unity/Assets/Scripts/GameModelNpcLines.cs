using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        static void BindNpcWeaponAttackLine(NpcDefinition profile,NpcStateDefinition state){
            // These original weapons are separate models attached to Dummy1_Rhand.
            float[] samples;
            if(profile.id=="NUI_LIZARDMAN_SPEAR" && state.id=="LIZARDMAN_SPEAR_ATTACK_A" && state.sourceAnimation=="AttackA_Fire")
                samples=BesmaSpearLineSamples;
            else if(profile.id=="NUI_LIZARDMAN_HIGH" && state.id=="LIZARDMAN_HIGH_ATTACK_A" && state.sourceAnimation=="DashAttack")
                samples=BesmaHighDashLineSamples;
            else if(profile.id=="NUI_LIZARDMAN_HIGH" && state.id=="LIZARDMAN_HIGH_SPECIAL_ATTACK_A" && state.sourceAnimation=="SpecialAttackA_Fire")
                samples=BesmaHighSpecialLineSamples;
            else if(state.sourceAnimation=="AttackA_Fire"){
                samples=DungeonMeleeLineSamples(profile.id,state.id);
                if(samples==null)return;
            }else return;
            if(state.sourceAnimSpeed!=1 || state.samples.Length*6!=samples.Length ||
                Math.Abs(state.duration-(state.samples.Length-1)/60f)>.00001f)return;
            // Leave an already populated or independently supplied motion alone.
            foreach(var frame in state.samples)if(frame.attackLines.Length!=0||frame.spheres.Length!=0)return;
            for(int i=0;i<state.samples.Length;i++){
                var line=new float[6];
                Array.Copy(samples,i*6,line,0,6);
                state.samples[i].attackLines=line;
            }
        }

        static void ValidateNpcLines(float[] lines){
            if(lines==null||lines.Length%6!=0)throw new ArgumentException("Invalid original NPC line helpers");
            foreach(float value in lines)if(!Finite(value))throw new ArgumentException("Non-finite original NPC line helper");
        }
        // Original line helpers are zero-width segments. Clip the segment to
        // the target's actual volume instead of inventing a box around steam.
        static bool ClipNpcLineAxis(float a,float b,float lo,float hi,ref float start,ref float end){
            float delta=b-a;if(Math.Abs(delta)<.000001f)return a>=lo&&a<=hi;
            float p=(lo-a)/delta,q=(hi-a)/delta;if(p>q){float swap=p;p=q;q=swap;}
            start=Math.Max(start,p);end=Math.Min(end,q);return start<=end;
        }
        static bool NpcLineHitsPlayer(float ax,float ay,float az,float bx,float by,float bz,Fighter target,float verticalRadius=0){
            float start=0,end=1;
            return ClipNpcLineAxis(ax,bx,target.x-9.8f,target.x+9.8f,ref start,ref end)&&
                ClipNpcLineAxis(ay,by,target.y-verticalRadius,target.y+42+verticalRadius,ref start,ref end)&&
                ClipNpcLineAxis(az,bz,-9.8f,9.8f,ref start,ref end);
        }
        bool NpcLineHitsEnemy(float ax,float ay,float az,float bx,float by,float bz,Enemy enemy){
            float start=0,end=1;if(!ClipNpcLineAxis(az,bz,-9.8f,9.8f,ref start,ref end))return false;
            return EnemySegmentIntersects(enemy,ax+(bx-ax)*start,ay+(by-ay)*start,ax+(bx-ax)*end,ay+(by-ay)*end,0);
        }
        bool NpcStateLinesTouchPlayer(Enemy enemy,NpcStateDefinition state,float at){
            float sample=Clamp(NpcLakeNightMotionFrameV19(state,at),0,state.samples.Length-1);int i=(int)sample;float u=sample-i;
            var a=state.samples[i].attackLines;var b=state.samples[Math.Min(i+1,state.samples.Length-1)].attackLines;
            if(a==null||a.Length==0)return false;
            float scale=enemy.visualScale,rootY=enemy.y+NativeHoverLiftV58(enemy);
            for(int n=0;n<a.Length;n+=6){
                float ax=enemy.x+enemy.facing*(a[n]+(b[n]-a[n])*u)*scale,ay=rootY+(a[n+1]+(b[n+1]-a[n+1])*u)*scale,az=(a[n+2]+(b[n+2]-a[n+2])*u)*scale;
                float bx=enemy.x+enemy.facing*(a[n+3]+(b[n+3]-a[n+3])*u)*scale,by=rootY+(a[n+4]+(b[n+4]-a[n+4])*u)*scale,bz=(a[n+5]+(b[n+5]-a[n+5])*u)*scale;
                if(NpcLineHitsPlayer(ax,ay,az,bx,by,bz,player))return true;
            }return false;
        }
    }
}
