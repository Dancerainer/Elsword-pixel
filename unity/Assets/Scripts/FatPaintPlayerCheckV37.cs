#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class GuardAndTreePlayerCheckV35 {
        [Serializable] sealed class PaintFootSampleV37{
            public string source,texture,image;public int side,facing,drawnPixels;public float clock,x,y,actualX,actualY;
        }
        [Serializable] sealed class PaintedFeetV37{public List<PaintFootSampleV37> samples=new List<PaintFootSampleV37>();}
        readonly PaintedFeetV37 feetPaintV37=new PaintedFeetV37();
        static bool FatPaintModeV37(string mode){return mode=="painted-fat"||mode=="painted-fat-boss"||mode=="painted-soldier-fat";}
        string FatKindV37(){return report.mode=="painted-fat"?"thief-fat":report.mode=="painted-fat-boss"?"thief-fat-boss":"soldier-fat";}
        object FatOwnerV37(){
            var views=(IDictionary)Field(scene,"paintOwnersV37");return views.Contains(enemy)?views[enemy]:null;
        }
        ReusablePaintViewV37 FatViewV37(){var owner=FatOwnerV37();return owner==null?null:(ReusablePaintViewV37)Field(owner,"view");}
        List<SpriteRenderer> FatPaintMembersV37(List<SpriteRenderer> group,out int count){
            var view=FatViewV37();Need(view!=null&&view.painted&&view.painted.sprite&&group[0].forceRenderingOff,"Owned independently retargeted painted sprite");
            group.Add(view.painted);count=1;return group;
        }
        void FatPaintRowV37(Observation row){
            var view=FatViewV37();Need(view!=null,"Painted owner witness");var f=view.Last;
            row.sourceMotion=f.track.motion;row.sourceKey=f.alias;row.sourceTime=f.clock;row.phase=f.paint+"/"+f.key;
        }
        Observation FatShotV37(string name){
            var r=Row(name);Capture(r,true);Need(r.callbacks==1&&r.eligible==1&&r.actualParts==1&&r.pixels>0,"Actual owned painted GPU draw "+name);
            var f=FatViewV37().Last;report.events.Add(name+" source="+f.alias+"/"+f.track.motion+" time="+f.clock.ToString("R")+
                " existingPaintedKey="+f.key+" legacyAction="+f.paint+" screenPixels="+r.pixels);
            return r;
        }
        void FatRoomV37(){
            var bank=ReusablePaintRigV37.Load(FatKindV37());var birth=bank.stages[0];
            string route=report.mode=="painted-soldier-fat"?"waterway-":"cave-";
            foreach(var b in bank.stages)if(b.room.StartsWith(route,StringComparison.Ordinal)){birth=b;break;}
            int stage=int.Parse(birth.room.Substring(birth.room.LastIndexOf('-')+1))-1;
            Call(game,"EnterRoom",stage);enemy=game.enemies[birth.index];
            if(enemy.encounterIndex!=game.EncounterIndex)Call(game,"BeginEncounter",enemy.encounterIndex);
            Call(scene,"RebuildRoom");
            Need(enemy.sourceNPC==bank.npc&&enemy.kind==bank.kind&&enemy.active&&!enemy.dormant&&game.CurrentRoom.id==birth.room,"Original staged guarded identity");
            report.npc=bank.npc;report.room=birth.room;
        }
        int FatShoeDrawV37(ReusablePaintViewV37 view,int side){
            var c=World;var old=c.targetTexture;var prior=RenderTexture.active;var rect=c.rect;var aspect=c.aspect;
            Texture2D on=null,off=null;bool excluded=false;
            try{
                c.targetTexture=rt;c.rect=new Rect(0,0,1,1);c.aspect=(float)Width/Height;RenderTexture.active=rt;
                callbacks.Clear();post=-1;onPass=true;GL.Clear(true,true,Color.clear);c.Render();onPass=false;
                Need(callbacks.Contains(view.painted)&&post==Time.frameCount,"Genuine painted owner camera callback");on=Read();
                view.ShowShoeInCheck(side,false);excluded=true;
                GL.Clear(true,true,Color.clear);c.Render();off=Read();
                var a=on.GetPixels32();var b=off.GetPixels32();int n=0;
                for(int i=0;i<a.Length;i++)if(a[i].r!=b[i].r||a[i].g!=b[i].g||a[i].b!=b[i].b)n++;
                return n;
            }finally{
                if(excluded)view.ShowShoeInCheck(side,true);onPass=false;c.targetTexture=old;c.rect=rect;c.aspect=aspect;RenderTexture.active=prior;
                if(on)Destroy(on);if(off)Destroy(off);
            }
        }
        void VerifyFatShoeV37(ReusablePaintViewV37 view,Observation shot,int side,bool[] seen){
            var f=view.Last;var source=f.track.XY(f.clock,side==0?0:6);var actual=view.ActualSole(side);
            int px=FatShoeDrawV37(view,side);
            var row=new PaintFootSampleV37{source=f.alias,texture=view.painted.sprite.texture.name,image=shot.image,side=side,
                facing=enemy.facing,clock=f.clock,x=source.x,y=source.y,actualX=actual.x,actualY=actual.y,drawnPixels=px};
            feetPaintV37.samples.Add(row);File.WriteAllText(Path.Combine(output,"painted-independent-soles.json"),JsonUtility.ToJson(feetPaintV37,true));
            Need(Math.Abs(source.x-actual.x)<.12f&&Math.Abs(source.y-actual.y)<.12f,"Actual owned Sprite vertex buffer follows the original independent sole "+
                side+" t="+f.clock+" source="+source+" draw="+actual);
            seen[side]|=px>0;
        }
        IEnumerable FatWalkV37(int face){
            FatRoomV37();float dist=BendersChaseDistanceV36();foreach(var x in MakeBendersWalkingSpaceV36(face,dist))yield return x;
            bool[] pixels=new bool[2],phases=new bool[4];int bins=0;float last=-5;
            var end=Time.realtimeSinceStartupAsDouble+35;
            while(bins<4||!pixels[0]||!pixels[1]){
                Protect(dist*face);yield return null;Budget();
                Need(Time.realtimeSinceStartupAsDouble<end,"Actual painted stride and two visible feet "+face+" phase count="+bins+
                    " foot proof="+pixels[0]+"/"+pixels[1]+" actor/player="+enemy.x+"/"+game.player.x+" source="+enemy.npcState);
                var view=FatViewV37();if(view==null||enemy.facing!=face||!view.Last.move||view.Last.track.motion!="Dash")continue;
                var f=view.Last;int bin=Math.Min(3,(int)(4*f.clock/f.track.duration));
                if(phases[bin]&&(pixels[0]&&pixels[1]||Math.Abs(last-f.clock)<.11f))continue;
                var shot=FatShotV37("stride-"+face+"-"+bin+"-"+Time.frameCount);
                for(int side=0;side<2;side++)VerifyFatShoeV37(view,shot,side,pixels);
                last=f.clock;if(!phases[bin]){phases[bin]=true;bins++;}
            }
        }
        IEnumerable FatStateV37(Predicate<ReusablePaintRigV37.Frame> matches,float offset,double seconds=30){
            var end=Time.realtimeSinceStartupAsDouble+seconds;
            for(;;){
                Protect(offset);yield return null;Budget();var v=FatViewV37();
                Need(Time.realtimeSinceStartupAsDouble<end,"Painted real phase actual="+(v==null?"none":v.Last.alias)+" source="+enemy.npcState+" clip="+enemy.clip+" actor/player="+enemy.x+"/"+game.player.x);
                if(v!=null&&matches(v.Last))yield break;
            }
        }
        IEnumerable FatPaintFlowV37(){
            bool water=report.mode=="painted-soldier-fat";foreach(var x in Enter("elder",water?"underground-waterway":"banthus-cave"))yield return x;
            foreach(var x in FatWalkV37(1))yield return x;foreach(var x in FatWalkV37(-1))yield return x;
            FatRoomV37();
            foreach(var x in FatStateV37(f=>f.track.motion=="AttackA_Ready",32))yield return x;FatShotV37("attack-ready");
            foreach(var x in FatStateV37(f=>f.track.motion=="AttackA_Fire",32))yield return x;FatShotV37("attack-fire");
            // The Cave floor drops between lines 2 and 4. A Hurt packet
            // delivered off a ledge enters the model's airborne reaction.
            // Use a fresh original birth and an actually grounded stride.
            FatRoomV37();
            foreach(var x in FatStateV37(f=>f.track.motion=="Dash"&&game.Grounded(enemy)&&!game.EnemyHasSuperArmor(enemy),BendersOpenChaseV36()))yield return x;
            report.events.Add("Public grounded hurt origin x="+enemy.x+" y="+enemy.y+" line="+enemy.groundLine+" clip="+enemy.clip);
            float hp=enemy.hp;Need(game.ApplyHitToEnemy(enemy,1,HitReaction.Hurt)>0&&enemy.hp<hp,"Public unarmored damage");
            report.events.Add("Public grounded hit result="+enemy.state+" clip="+enemy.clip+" hp="+hp+"->"+enemy.hp);
            Need(enemy.state==CombatState.Hurt,"Actual grounded public Hurt result "+enemy.state+" "+enemy.clip);
            foreach(var x in FatStateV37(f=>f.paint=="hurt",40))yield return x;FatShotV37("hurt");
            Need(game.ApplyHitToEnemy(enemy,1,HitReaction.Launch,280)>0,"Public unarmored launch");
            foreach(var x in FatStateV37(f=>f.paint=="launch",40))yield return x;FatShotV37("launch");
            foreach(var x in FatStateV37(f=>f.paint=="fall",40))yield return x;FatShotV37("fall");
            foreach(var x in FatStateV37(f=>f.paint=="down",40))yield return x;FatShotV37("down");
            foreach(var x in FatStateV37(f=>f.paint=="get-up",40))yield return x;FatShotV37("get-up");
            foreach(var x in FatStateV37(f=>f.track.motion=="Dash",BendersOpenChaseV36()))yield return x;
            Need(game.ApplyHitToEnemy(enemy,100000,HitReaction.None)>0&&enemy.hp==0,"Original fatal HP"); 
            foreach(var x in FatStateV37(f=>f.paint=="death",40))yield return x;FatShotV37("death");
        }
    }
}
#endif
