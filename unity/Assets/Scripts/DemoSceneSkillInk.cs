using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        SpriteRenderer motionSkillInk;

        void RegisterGroundSkillInk(bool deferFrames) {
            var data=Resources.Load<TextAsset>("Pixel/skill-fx/ground-sheets");
            var sheets=JsonUtility.FromJson<ArtCatalog>(data.text);
            foreach(var sheet in sheets.effects)MakeLibrary(sheet,deferFrames);
        }

        bool DrawSkillInk(SpriteRenderer renderer,string id,int frame,Vector2 position,int facing,
                          Vector2 scale,Color color,int order) {
            if (!libraries.TryGetValue("skillfx-"+id,out var library) || !library.frames.ContainsKey("play"))
                return false;
            renderer.sprite=library.frames.At("play",frame,out _);
            renderer.flipX=facing<0;
            renderer.color=color;
            renderer.sortingOrder=order;
            renderer.transform.localScale=new Vector3(scale.x,scale.y,1);
            renderer.transform.localRotation=Quaternion.identity;
            renderer.transform.localPosition=position+Vector2.Scale(SpriteOffset(renderer),scale);
            renderer.enabled=renderer.sprite;
            return renderer.enabled;
        }

        bool SkillInkPart(EffectView view,int slot,string id,int frame,Vector2 world,int facing,
                          Vector2 scale,Color color,int order=44) {
            return DrawSkillInk(SkillComponent(view,slot),id,frame,world-(Vector2)view.root.transform.localPosition,
                                facing,scale,color,order);
        }

        static Color InkAlpha(float opacity) {return new Color(1,1,1,Mathf.Clamp01(opacity));}

        // These short lead-ins belong to the moving actor's animation. The landing/shock
        // already has its own independent world-space Effect emitted by the combat model.
        void RenderMotionSkillInk() {
            if(motionSkillInk)motionSkillInk.enabled=false;
            if(game.player.hp<=0)return;
            string id=game.ActiveSkillId;
            float clock=RenderPlayerClock;
            int facing=game.player.facing;
            Vector2 position=RenderPlayerPosition,scale;
            int frame;
            if(id=="roll" && game.player.clip=="roll" && clock<.4f) {
                frame=clock<.11f?0:1;
                position.x-=facing*(6+clock*28);
                scale=new Vector2(.36f,.20f);
            } else if(id=="impact-smash" && clock>=.266f/1.1f && clock<.713f/1.1f) {
                frame=clock<.46f?0:1;
                position.x+=facing*25;
                scale=new Vector2(.31f,.28f);
            } else return;

            if(!motionSkillInk)motionSkillInk=WorldSprite("Skill movement ink",effectsRoot,null,43,Vector3.zero);
            DrawSkillInk(motionSkillInk,id,frame,position,facing,scale,InkAlpha(id=="roll"?.65f:.87f),
                         id=="roll"?heroView.sortingOrder-1:43);
        }
    }
}
