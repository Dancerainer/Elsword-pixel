using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        const float BattleNavigationLeft=698;
        RectTransform touchHUD,keyboardHUD,potionDock;
        BattleJoystick battleStick;
        readonly List<Sprite> controlSprites=new List<Sprite>();
        readonly Image[] wheelIcons=new Image[GameModel.SkillSlotCount],wheelCooldown=new Image[GameModel.SkillSlotCount];
        readonly Text[] wheelTimers=new Text[GameModel.SkillSlotCount],potionTimers=new Text[GameModel.PotionSlotCount];
        readonly Image[] potionShades=new Image[GameModel.PotionSlotCount];
        bool? drawnKeyboardMode;
        readonly int[] drawnPotionSeconds={-1,-1,-1};

        static readonly string[] legacyTouchButtons={"Touch Left","Touch Right","Touch Dash","Touch Jump","Touch Attack","Touch Attack X"};
        static readonly Vector2[] lowerSkillArc={
            new Vector2(707,143),new Vector2(774,182),new Vector2(841,200),new Vector2(908,183)
        };
        static readonly Vector2 upperSkillArcOffset=new Vector2(-13,102);
        // Non-shipping recording fixtures replay the old independent directional buttons by name.
#if (ROOT_REDRAW_CHECK_V32 || ROOT_FACTORY_RAVEN_PLAYER_R1464 || ROOT_CARGO_BOSS_PLAYER_R1467 || ROOT_RECYCLE_NATURAL_PLAYER_R1471 || ROOT_OCCLUSION_CHECK_R1462) && !UNITY_EDITOR && !UNITY_ANDROID
        static readonly bool FixtureTouchLayout=System.Array.Exists(System.Environment.GetCommandLineArgs(),arg=>
            (arg.StartsWith("--root-",System.StringComparison.Ordinal)&&arg.Contains("-check-v"))
            ||arg=="--factory-raven-check-r1464"||arg=="--cargo-boss-check-r1467"
            ||arg=="--recycle-natural-check-r1471"||arg=="--below-lying-contact-check-r1483"
            ||arg=="--native-occlusion-check-r1462"||arg=="--below-core-readability-check-r1482"
            ||arg=="--check-dungeon-list");
#else
        bool FixtureTouchLayout {get{return false;}}
#endif
        internal bool CanMoveStick {get{return !KeyboardMode&&!settingsOpen&&string.IsNullOrEmpty(openMenu)&&!game.paused&&game.player.hp>0&&!BossIntroductionActive;}}
        internal void StickVerticalPressed(int sign){Action(sign>0?"jump":"drop");}
        internal void StickDirectionChanged(int sign,bool dash){
            tappedDashDirection=dash?sign:0;
            pending.dashPressed=dash;
        }
        void DirectionPressed(int sign){
            float last=sign<0?lastLeftTap:lastRightTap;
            tappedDashDirection=0;
            if(wallClock-last<.26f){tappedDashDirection=sign;pending.dashPressed=true;}
            // Any opposite direction breaks a repeated-direction command.
            lastLeftTap=sign<0?wallClock:-10;lastRightTap=sign>0?wallClock:-10;
        }
        void ResetBattleGesture(){
            if(battleStick)battleStick.ResetGesture();
            lastLeftTap=lastRightTap=-10;tappedDashDirection=0;
        }
        BattleHudArt battleArt;
        Sprite KeepControlSprite(Sprite sprite){controlSprites.Add(sprite);return sprite;}
        void DisposeBattleControls(){
            foreach(var sprite in controlSprites)if(sprite)Destroy(sprite);
            controlSprites.Clear();
            if(battleArt!=null){Destroy(battleArt.texture);battleArt=null;}
        }
        static Rect CenterBox(float x,float y,float diameter){return new Rect(x-diameter/2,y-diameter/2,diameter,diameter);}
        Image ControlImage(string name,Rect rect,Sprite sprite,Transform parent,bool target=false){
            var image=UIRect(name,parent,rect).gameObject.AddComponent<Image>();image.sprite=sprite;image.raycastTarget=target;
            if(sprite)image.preserveAspect=true;
            return image;
        }
        void BindAction(Image image,string action){
            var button=image.gameObject.AddComponent<TouchControl>();button.owner=this;button.action=action;button.pulseOnPress=true;
        }
        void BuildBattleControls(){
            battleArt=new BattleHudArt();
            var full=new Rect(0,0,960,540);
            touchHUD=UIRect("Touch battle controls",safeRoot,full);
            keyboardHUD=UIRect("Keyboard battle controls",safeRoot,full);
            touchHUD.gameObject.AddComponent<CanvasGroup>().alpha=.60f;
            keyboardHUD.gameObject.AddComponent<CanvasGroup>().alpha=.70f;
            potionDock=UIRect("Potion rack",safeRoot,new Rect(0,0,178,58));
            Transform banner=safeRoot.Find("Banner Panel");
            foreach(Transform group in new[]{touchHUD,keyboardHUD,potionDock})group.SetSiblingIndex(banner?banner.GetSiblingIndex():0);
            foreach(string name in new[]{"Footer","Keyboard Hint","System Hint","Skill Bar","Awakening Button"})MoveToKeyboard(name);
            for(int i=0;i<GameModel.SkillSlotCount;i++)
                foreach(string part in new[]{"Skill ","Skill Icon ","Skill Glyph ","Skill Key ","Skill Name ","Cooldown Shade ","Cooldown "})MoveToKeyboard(part+i);
            foreach(string name in legacyTouchButtons)safeRoot.Find(name)?.gameObject.SetActive(FixtureTouchLayout);
            safeRoot.Find("Potion Button")?.gameObject.SetActive(false);

            var steel=KeepControlSprite(battleArt.Disc(C(0xc6deef)));
            var amber=KeepControlSprite(battleArt.Disc(C(0xff9952)));
            var crimson=KeepControlSprite(battleArt.Disc(C(0xff626b)));
            var blue=KeepControlSprite(battleArt.Disc(C(0x6bafff)));
            var thumb=KeepControlSprite(battleArt.Disc(C(0xdbdff4),true));
            BuildJoystick(blue,thumb);
            for(int i=0;i<GameModel.SkillSlotCount;i++)BuildWheelSlot(i,steel);
            BuildCommand("Touch Light","attack","LIGHT",893,70,86,amber,BattleHudArt.Blade,C(0xff7239));
            BuildCommand("Touch Heavy","attackX","HEAVY",796,70,86,amber,BattleHudArt.GreatBlade,C(0xffae6a));
            BuildCommand("Touch Awaken","awaken","AWAKEN",708,63,44,crimson,BattleHudArt.Crystal,C(0xff3f53));
            BuildPotions();BuildEnemyGuide();battleArt.Upload();
            touchHUD.gameObject.SetActive(false);keyboardHUD.gameObject.SetActive(false);potionDock.gameObject.SetActive(false);
        }
        void MoveToKeyboard(string name){
            var child=safeRoot.Find(name);
            if(child)child.SetParent(keyboardHUD,false);
        }
        void BuildJoystick(Sprite disc,Sprite thumbSprite){
            const float size=256;
            var pad=ControlImage("Movement pad",CenterBox(138,140,size),null,touchHUD,true);
            pad.color=Color.clear;
            ControlImage("Movement inner pad",new Rect((size-176)/2,(size-176)/2,176,176),disc,pad.transform);
            battleStick=pad.gameObject.AddComponent<BattleJoystick>();battleStick.owner=this;
            battleStick.sprintRing=ControlImage("Movement sprint ring",new Rect(0,0,size,size),KeepControlSprite(battleArt.SprintRing()),pad.transform);
            battleStick.sprintRing.color=new Color(.65f,.78f,1,.70f);
            Label("Movement sprint hint","SPRINT",new Rect((size-72)/2,9,72,14),8,C(0xb9d7ff),TextAnchor.MiddleCenter,pad.transform);
            var arrow=KeepControlSprite(battleArt.Stamp(BattleHudArt.Arrow,C(0x9cafe3)));
            for(int axis=0;axis<4;axis++){
                float angle=axis*90;Vector2 offset=Quaternion.Euler(0,0,angle)*(Vector2.up*65);
                var mark=ControlImage("Stick arrow "+axis,new Rect(0,0,14,9),arrow,pad.transform);
                var r=mark.rectTransform;r.pivot=new Vector2(.5f,.5f);r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.anchoredPosition=offset;r.localRotation=Quaternion.Euler(0,0,angle);
            }
            var thumb=ControlImage("Stick thumb",new Rect(0,0,58,58),thumbSprite,pad.transform);
            var tr=thumb.rectTransform;tr.pivot=new Vector2(.5f,.5f);tr.anchorMin=tr.anchorMax=tr.pivot;tr.anchoredPosition=Vector2.zero;
            battleStick.thumb=tr;
        }
        void BuildWheelSlot(int index,Sprite frame){
            const int size=56;
            var center=lowerSkillArc[index%lowerSkillArc.Length];
            if(index>=lowerSkillArc.Length)center+=upperSkillArcOffset;
            var button=ControlImage("Wheel slot "+(index+1),CenterBox(center.x,center.y,size),frame,touchHUD,true);
            BindAction(button,"skill"+index);
            // The rectangular source icons are clipped to the inner edge of their circular bezel.
            var mask=ControlImage("Wheel aperture "+index,new Rect(4,4,size-8,size-8),frame,button.transform);
            mask.gameObject.AddComponent<Mask>().showMaskGraphic=false;
            wheelIcons[index]=ControlImage("Wheel skill art "+index,new Rect(0,0,size-8,size-8),null,mask.transform);
            wheelIcons[index].color=C(0xffffff,.88f);
            wheelCooldown[index]=ControlImage("Wheel cooldown shade "+index,new Rect(0,0,size-8,size-8),null,mask.transform);
            wheelCooldown[index].color=C(0x091126,.76f);
            wheelTimers[index]=Label("Wheel cooldown "+index,"",new Rect(0,7,size,size-14),15,Color.white,TextAnchor.MiddleCenter,button.transform);
            var badge=ControlImage("Wheel number plate "+index,new Rect(18,-4,20,14),null,button.transform);
            badge.color=C(0x101b30,.97f);
            Label("Wheel number "+index,(index+1).ToString(),new Rect(0,0,20,14),9,C(0xffdfa7),TextAnchor.MiddleCenter,badge.transform);
        }
        void BuildCommand(string name,string action,string title,float x,float y,int size,Sprite frame,string[] pixels,Color color){
            var button=ControlImage(name,CenterBox(x,y,size),frame,touchHUD,true);BindAction(button,action);
            int art=Mathf.RoundToInt(size*.44f);
            ControlImage(name+" emblem",new Rect((size-art)/2,(size-art)/2+5,art,art),KeepControlSprite(battleArt.Stamp(pixels,color)),button.transform);
            Label(name+" caption",title,new Rect(2,5,size-4,Mathf.RoundToInt(size*.28f)),size>=75?10:8,C(0xffedcc),TextAnchor.MiddleCenter,button.transform);
        }
        void BuildPotions(){
            var rack=Panel("Medicine case",new Rect(0,0,178,58),C(0x19273d,.79f),potionDock);
            string[] labels={"50 MP","100 MP","FULL"};
            uint[] colors={0x53c9f7,0x6d83ff,0xf783a3};
            for(int i=0;i<GameModel.PotionSlotCount;i++){
                var button=Panel("Medicine "+i,new Rect(4+i*57,4,56,50),C(0x152335,.83f),rack.transform);
                button.raycastTarget=true;BindAction(button,"potion"+(i+1));
                var bottle=KeepControlSprite(battleArt.Stamp(BattleHudArt.Bottle,C(colors[i])));
                if(i==0)recoveryMpBottle=bottle;
                if(i==2)recoveryHpBottle=bottle;
                ControlImage("Medicine bottle "+i,new Rect(15,13,26,32),bottle,button.transform);
                Label("Medicine key "+i,(i+1).ToString(),new Rect(4,32,12,13),8,C(0xffdf98),TextAnchor.MiddleLeft,button.transform);
                Label("Medicine dose "+i,labels[i],new Rect(2,1,52,13),8,C(0xdce9f6),TextAnchor.MiddleCenter,button.transform);
                potionShades[i]=ControlImage("Medicine cooldown shade "+i,new Rect(3,3,50,44),null,button.transform);
                potionShades[i].color=C(0x0b1325,.73f);
                potionTimers[i]=Label("Medicine cooldown "+i,"",new Rect(0,0,56,50),14,Color.white,TextAnchor.MiddleCenter,button.transform);
            }
        }
        void RefreshBattleControls(){
            bool keyboard=KeyboardMode;
            if(drawnKeyboardMode!=keyboard){
                touchHUD.gameObject.SetActive(!keyboard&&!FixtureTouchLayout);
                keyboardHUD.gameObject.SetActive(keyboard||FixtureTouchLayout);
                potionDock.gameObject.SetActive(true);
                potionDock.anchoredPosition=keyboard?new Vector2(213,30):new Vector2(391,13);
                string[] names={"Dungeon Button","Loadout Button","Guide Button"};
                string[] hints=keyboard?new[]{"MAP TAB","SKILLS K","COMBOS F1"}:new[]{"MAP","SKILLS","COMBOS"};
                for(int i=0;i<names.Length;i++){
                    if(!images.TryGetValue(names[i],out var nav))continue;
                    nav.gameObject.SetActive(keyboard);
                      nav.rectTransform.anchoredPosition=new Vector2(BattleNavigationLeft+i*84,378);
                    nav.rectTransform.sizeDelta=new Vector2(80,32);labels[names[i]+" Text"].rectTransform.sizeDelta=nav.rectTransform.sizeDelta;
                    TextValue(names[i]+" Text",hints[i]);
                }
                Visible("Touch Toggle",keyboard);
                Visible("Sound Button",keyboard);
                PlaceMenuItem(images["Pause Button"].transform,new Rect(407,483,146,48));
                PlaceMenuItem(images["Progress Track"].transform,new Rect(423,480,114,3));
                images["Progress Fill"].rectTransform.anchoredPosition=new Vector2(423,480);
                drawnKeyboardMode=keyboard;
            }
            for(int i=0;i<GameModel.PotionSlotCount;i++){
                int seconds=Mathf.CeilToInt(Mathf.Max(0,game.potionCooldowns[i]));
                if(drawnPotionSeconds[i]==seconds)continue;
                drawnPotionSeconds[i]=seconds;potionShades[i].enabled=seconds>0;
                potionTimers[i].text=seconds>0?seconds.ToString():"";
            }
        }
    }
}
