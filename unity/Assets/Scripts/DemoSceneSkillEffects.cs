using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        // Native frames already contain source color/size/rotation curves.
        // logicalScale converts raster density to .28 logical px/native unit.
        // Effect.scaleX/Y belong to old painted sheets and are not reused.
        readonly Dictionary<EffectView,List<SpriteRenderer>> skillComponentViews=new Dictionary<EffectView,List<SpriteRenderer>>();
        readonly List<SpriteRenderer> castComponents=new List<SpriteRenderer>();
        readonly List<SpriteRenderer> visibleCastComponents=new List<SpriteRenderer>();
        readonly Dictionary<string,string> nativeSkillKeys=new Dictionary<string,string>(StringComparer.Ordinal);
        static readonly string[] doubleFireParts={"double-fire-0","double-fire-1","double-fire-2","double-fire-3","double-fire-4","double-fire-5"};

        bool NativeSkillClip(string component,out Library library,out AnimationData clip){
            if(!nativeSkillKeys.TryGetValue(component,out var key)){
                key="nativefx-"+component;nativeSkillKeys.Add(component,key);
            }
            clip=null;
            return libraries.TryGetValue(key,out library)&&library.clips.TryGetValue("play",out clip);
        }
        static bool NativeSkillFrameVisible(AnimationData clip,float age,bool repeat){
            return age>=0&&(clip.loop||repeat||age<clip.duration);
        }
        static readonly float[] giganticWaveTimes={3.166f,3.633f,4.033f,4.199f,4.533f};
        static readonly float[] giganticFinalTimes={5.633f,5.866f,6.233f,6.733f,6.399f};
        static readonly float[] unlimitedVisualTimes={.001f,.401f,.651f,.951f,1.251f};
        static readonly string[] unlimitedVisualParts={"unlimited-blade","unlimited-second","unlimited-third","unlimited-third","unlimited-finish"};
        static readonly float[] stormTimes={.01f,.15f,.3f,.45f,.55f,.65f};
        static readonly float[] stormScaleX={.8f,.9f,1,1.1f,1.2f,1.1f},stormScaleY={.8f,.9f,1,1,1,1};
        string sourceTimelineSkill="";int sourceTimelineRoom=-1,sonicFacing;float sourceTimelineClock=-1;bool sonicOriginSet;Vector2 sonicOrigin;
        SpriteRenderer armageddonAttachment,giganticAttachment;
        readonly Dictionary<int,Material> doubleMaterials=new Dictionary<int,Material>();
        readonly Dictionary<SpriteRenderer,Material> doubleOriginalMaterials=new Dictionary<SpriteRenderer,Material>();
        readonly Vector2[] doubleSwordOrigins=new Vector2[28];readonly float[] doubleSwordAngles=new float[28];readonly bool[] doubleSwordBorn=new bool[28];
        MaterialPropertyBlock doubleSpriteProperties;bool doubleFireOriginSet;Vector2 doubleFireOrigin;int doubleFireFacing;
        // Source character projection uses .20 px/native unit. FX density
        // remains .28. Bip01 root X/Z are already consumed by actor motion.
        static readonly float[] doubleHipY={
12.125684f,12.125684f,12.125684f,12.125684f,12.125684f,11.933869f,11.742055f,11.604627f,11.467199f,11.480293f,11.493386f,11.528374f,11.563361f,11.613789f,11.664217f,
11.723744f,11.783272f,11.845621f,11.907970f,11.966806f,12.025642f,12.074476f,12.123310f,12.143396f,12.163483f,12.174975f,12.186467f,12.241166f,12.295865f,12.409743f,
12.523621f,12.668452f,12.813283f,12.974092f,13.134901f,13.295475f,13.456049f,13.651062f,13.846075f,14.089207f,14.332339f,14.576154f,14.819968f,15.017969f,15.215970f,
15.322069f,15.428168f,15.395972f,15.363777f,14.967229f,14.570682f,13.850817f,13.130952f,12.440545f,11.750138f,11.441646f,11.133154f,11.132764f,11.132375f,11.131358f,
11.130342f,11.128918f,11.127493f,11.125895f,11.124297f,11.122792f,11.121288f,11.120159f,11.119030f,11.119101f,11.119172f,11.119305f,11.119439f,11.119468f,11.119497f,
11.119582f,11.119667f,11.119802f,11.119937f,11.120117f,11.120296f,11.120516f,11.120735f,11.120987f,11.121239f,11.121519f,11.121799f,11.122099f,11.122400f,11.122716f,
11.123032f,11.123359f,11.123685f,11.124015f,11.124345f,11.124675f,11.125005f,11.125329f,11.125654f,11.125969f,11.126283f,11.126585f,11.126887f,11.127172f,11.127458f,
11.127725f,11.127991f,11.128238f,11.128484f,11.128708f,11.128931f,11.129132f,11.129333f,11.129510f,11.129687f,11.129840f,11.129993f,11.130122f,11.130252f,11.130358f,
11.130463f,11.130546f,11.130629f,11.130689f,11.130749f,11.130785f,11.130821f,11.130834f,11.130846f,11.243449f,11.356052f,11.650668f,11.945283f,12.356869f,12.768455f,
13.233274f,13.698094f,14.154700f,14.611307f,14.999732f,15.388158f,15.911668f,16.435178f,16.476831f,16.518484f,14.051722f,11.584959f,11.377727f,11.170495f,11.380698f,
11.590902f,11.604515f,11.618129f,11.630043f,11.641958f,11.652654f,11.663351f,11.673312f,11.683273f,11.692983f,11.702693f,11.712635f,11.722577f,11.733237f,11.743897f,
11.755761f,11.767624f,11.781175f,11.794726f,11.810451f,11.826176f,11.844559f,11.862943f,11.884468f,11.905993f,11.931141f,11.956290f,11.985542f,12.014794f,12.048627f,
12.082461f,12.143961f,12.205461f,12.303728f,12.401995f,12.516563f,12.631132f,12.741698f,12.852264f,12.938679f,13.025093f,13.067256f,13.109419f,13.114718f,13.120016f,
13.111094f,13.102171f,13.081786f,13.061400f,13.032300f,13.003200f,12.968126f,12.933052f,12.894740f,12.856427f,12.817607f,12.778788f,12.742199f,12.705609f,12.673986f,
12.642362f,12.618452f,12.594542f,12.581094f,12.567645f,12.567414f,12.567182f,12.576690f,12.586197f,12.599860f,12.613524f,12.630939f,12.648354f,12.669117f,12.689879f,
12.713584f,12.737289f,12.763530f,12.789771f,12.818143f,12.846515f,12.876612f,12.906709f,12.938124f,12.969539f,13.001866f,13.034193f,13.067026f,13.099858f,13.132790f,
13.165723f,13.198347f,13.230972f,13.262884f,13.294795f,13.325589f,13.356383f,13.385651f,13.414919f,13.442255f,13.469592f,13.494591f,13.519589f,13.541846f,13.564102f,
13.581797f,13.599492f,13.611887f,13.624281f,13.632594f,13.640906f,13.646353f,13.651801f,13.655605f,13.659410f,13.662797f,13.666183f,13.670382f,13.674580f,13.680817f,
13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,
13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f,13.687054f
        };
        void PrepareSkillEffectView(EffectView view){
            if(view.activeSkillParts==null)return;
            foreach(var child in view.activeSkillParts)child.enabled=false;
            view.activeSkillParts.Clear();
        }
        SpriteRenderer SkillComponent(EffectView view,int slot){
            if(!skillComponentViews.TryGetValue(view,out var list)){list=new List<SpriteRenderer>();skillComponentViews.Add(view,list);}
            while(list.Count<=slot)list.Add(WorldSprite("Source skill component",view.root.transform,null,44,Vector3.zero));
            if(view.activeSkillParts==null)view.activeSkillParts=new List<SpriteRenderer>();
            var renderer=list[slot];view.activeSkillParts.Add(renderer);return renderer;
        }
        void TintSkillPart(EffectView view,int slot,Color color){
            if(skillComponentViews.TryGetValue(view,out var list)&&slot<list.Count)list[slot].color=color;
        }
        static Vector2 RotateSkillOffset(Vector2 p,float degrees){float a=degrees*Mathf.Deg2Rad,c=Mathf.Cos(a),s=Mathf.Sin(a);return new Vector2(c*p.x-s*p.y,s*p.x+c*p.y);}
        bool NativeFrame(SpriteRenderer sprite,string component,float age,Vector2 origin,int facing,int order,bool repeat=false,float angle=0,float sourceScaleX=1,float sourceScaleY=1){
            if(!NativeSkillClip(component,out var lib,out var clip))return false;
            if(!NativeSkillFrameVisible(clip,age,repeat)){sprite.enabled=false;return true;}
            DrawNativeSkillFrame(sprite,lib,clip,age,origin,facing,order,repeat,angle,sourceScaleX,sourceScaleY);
            return true;
        }
        void DrawNativeSkillFrame(SpriteRenderer sprite,Library lib,AnimationData clip,float age,Vector2 origin,int facing,int order,bool repeat,float angle,float sourceScaleX,float sourceScaleY){
            if((repeat||clip.loop)&&clip.duration>0)age=Mathf.Repeat(age,clip.duration);
            sprite.sprite=lib.frames.At("play",Mathf.FloorToInt(age*clip.fps),out _);
            sprite.flipX=facing<0;sprite.color=Color.white;sprite.sortingOrder=order;sprite.enabled=true;
            float scale=lib.data.logicalScale>0?lib.data.logicalScale:1,sx=scale*sourceScaleX,sy=scale*sourceScaleY;Vector2 offset=SpriteOffset(sprite);offset=RotateSkillOffset(new Vector2(offset.x*sx,offset.y*sy),angle);
            sprite.transform.localScale=new Vector3(sx,sy,1);sprite.transform.localRotation=Quaternion.Euler(0,0,angle);sprite.transform.localPosition=new Vector3(origin.x+offset.x,origin.y+offset.y,0);
        }
        bool NativePart(EffectView view,int slot,string id,float age,Vector2 world,int facing,int order=44,bool repeat=false,float angle=0,float sx=1,float sy=1){
            if(!NativeSkillClip(id,out var lib,out var clip))return false;
            if(NativeSkillFrameVisible(clip,age,repeat))
                DrawNativeSkillFrame(SkillComponent(view,slot),lib,clip,age,world-(Vector2)view.root.transform.localPosition,facing,order,repeat,angle,sx,sy);
            return true;
        }
        bool NativeBurst(EffectView view,int first,string id,float age,int count,float spacing,Vector2 origin,int facing,int order=44){bool available=false;for(int i=0;i<count;i++)available|=NativePart(view,first+i,id,age-i*spacing,origin,facing,order);return available;}
        void MinorPart(EffectView view,int slot,Sprite sprite,Vector2 world,Vector2 scale,Color color,int facing=1,float angle=0,int order=44){
            var r=SkillComponent(view,slot);r.sprite=sprite;r.flipX=facing<0;r.color=color;r.sortingOrder=order;r.enabled=sprite;
            r.transform.localScale=new Vector3(scale.x,scale.y,1);r.transform.localRotation=Quaternion.Euler(0,0,angle);Vector2 p=world-(Vector2)view.root.transform.localPosition;r.transform.localPosition=new Vector3(p.x,p.y,0);
        }
        bool PaintedFallback(EffectView view,string id,Effect e,Vector2 origin,int first,int count,bool loop=false){
            if(!libraries.TryGetValue("skillfx-"+id,out var lib)||!lib.frames.ContainsKey("play"))return false;
            int i=first+(loop?(int)(RenderedEffectAge(e)*12)%count:Math.Min(count-1,(int)(Mathf.Clamp01(RenderedEffectAge(e)/Mathf.Max(.001f,e.lifetime))*count)));
            MinorPart(view,0,lib.frames.At("play",i,out _),origin,Vector2.one*.7f,Color.white,e.facing);return true;
        }
        Vector2 SkillEffectOrigin(Effect e){return e.followPlayer?RenderPlayerPosition+new Vector2(game.player.facing*e.offsetX,e.offsetY):new Vector2(e.x,e.y);}
        Vector2 SourceTorso(Effect e,float oldX,float oldY){int facing=e.followPlayer?game.player.facing:e.facing;return SkillEffectOrigin(e)+new Vector2(-facing*oldX,32-oldY);}
        bool RenderSkillEffect(EffectView v,Effect e){
            string id=e.type;int face=e.followPlayer?game.player.facing:e.facing;float age=RenderedEffectAge(e),t=Mathf.Clamp01(age/Mathf.Max(.001f,e.lifetime));Vector2 p=SkillEffectOrigin(e),ground=new Vector2(p.x,e.visualGroundY);
            // Recognized skills own all fallbacks, so they never fall through
            // to the former shared orange-slash branch in RenderEffects.
            switch(id){
                case "mega-slash":
                    v.sprite.enabled=false;
                    if(e.phase<2){
                        if(game.player.clip!="mega-charge")return true;
                        RenderedWeapon(out var grip,out var bladeAngle);
                        Vector2 charge=grip+RotateSkillOffset(new Vector2(face*15,0),bladeAngle*face);
                        float pulse=.65f+.3f*Mathf.Sin(age*(age>=1?30:14));
                        SkillInkPart(v,0,id,0,charge,face,Vector2.one*(age>=1?.34f:.24f),InkAlpha(pulse));return true;
                    }
                    if(!NativeBurst(v,0,id,age,3,1/80f,SourceTorso(e,28,36),face)&&!PaintedFallback(v,id,e,p,1,5))MinorPart(v,0,hit,p,new Vector2(.65f,.35f),C(0xffefbd,1-t),face);return true;
                case "flame-geyser":case "triple-geyser":
                    v.sprite.enabled=false;bool flame=NativePart(v,0,"flame-geyser",age,ground,face,18),column=NativeBurst(v,1,"flame-column",age,3,.01f,ground,face,43);
                    // P02 is the beam/glow, not the visible fire column. The
                    // source START_PARTICLE2 emits three P03 skinned flames;
                    // P05 supplies the bright ground burst beneath each column.
                    bool body=NativeBurst(v,4,"flame-body",age,3,1/80f,ground,face,44);
                    NativePart(v,7,"flame-burst",age,ground,face,42);
                    NativePart(v,8,"flame-core",age,ground+Vector2.up*1.4f,face,43);
                    if(!flame&&!column&&!body&&!PaintedFallback(v,"flame-geyser",e,ground+Vector2.up*62,0,6))MinorPart(v,0,hit,ground+Vector2.up*30,new Vector2(.5f,1.5f),C(0xff8b42,1-t),face);return true;
                case "iron-body":
                    v.sprite.enabled=false;
                    int aura=e.phase==0?Mathf.Min(2,(int)(age*8)):2+(int)(age*5)%3;
                    float auraOpacity=e.phase==0?.88f:(.38f+.07f*Mathf.Sin(age*5))*Mathf.Clamp01(game.armor/.3f);
                    SkillInkPart(v,0,id,aura,RenderPlayerPosition+Vector2.up*24,face,new Vector2(.39f,.38f),
                                 InkAlpha(auraOpacity),heroView.sortingOrder-1);return true;
                case "armor-break":case "kick":
                    v.sprite.enabled=false;
                    bool shatter=id=="armor-break";
                    int footFrame=shatter?Mathf.Min(4,(int)(t*5)):Mathf.Min(5,(int)(t*6));
                    SkillInkPart(v,0,id,footFrame,p,face,shatter?new Vector2(.34f,.32f):new Vector2(.27f,.22f),
                                 InkAlpha(Mathf.Clamp01((1-t)*4)));return true;
                case "counter-attack":
                    v.sprite.enabled=false;
                    if(e.phase==0){
                        if(game.CounterWaiting && RenderPlayerClock>=.06f && RenderPlayerClock<=.5f &&
                           libraries.TryGetValue("fx-guard",out var guardInk))
                            MinorPart(v,0,guardInk.frames.At("play",0,out _),p+Vector2.up*4,
                                      new Vector2(.28f,.34f),InkAlpha(.82f+.14f*Mathf.Sin(age*13)),face,0,42);
                    }else SkillInkPart(v,0,id,Mathf.Min(5,1+(int)(t*5)),p,face,new Vector2(.51f,.48f),InkAlpha(1-t*.6f));
                    return true;
                case "roll":
                    v.sprite.enabled=false;SkillInkPart(v,0,id,t<.25f?0:t<.6f?4:5,ground,face,new Vector2(.44f,.20f),
                                 InkAlpha(1-t*.65f),heroView.sortingOrder-1);return true;
                case "sonic-blade":
                    v.sprite.enabled=false;
                    if(libraries.ContainsKey("nativefx-sonic-blade"))return true; // Source visual starts before its damage event; see RenderCastComponents.
                    // World-fixed origin: expansion is baked; there is no
                    // age*facing displacement or player-following transform.
                    if(!NativeBurst(v,0,id,age,3,1/80f,p,face)&&!PaintedFallback(v,id,e,p,0,6))MinorPart(v,0,ring,p,new Vector2(.4f+t,.3f+t*.4f),C(0xffe8ba,1-t),face);return true;
                case "double-contact":
                    v.sprite.enabled=false;
                    RenderDoubleContact(v,e,p,age,t);
                    return true;
                case "double-slash":
                    v.sprite.enabled=false;
                    // The source EffectSet starts the first four mesh layers
                    // at .304 and the second three at 1.854. Their animation
                    // clock is independent of old painted-sheet effects.
                    if(libraries.ContainsKey("nativefx-double-first"))return true;
                    if(!PaintedFallback(v,id,e,p,e.phase==0?0:3,3))MinorPart(v,0,hit,p,new Vector2(.7f,.9f),C(0xffe6bf,1-t),face);return true;
                case "spiral-blast":
                    v.sprite.enabled=false;
                    if(e.phase==0)NativeBurst(v,0,id,age,3,1/80f,SourceTorso(e,40,30)+Vector2.up*8.4f,face);
                    else SkillInkPart(v,0,id,Mathf.Min(5,2+(int)(t*4)),p,face,new Vector2(.83f,.64f),
                                      InkAlpha(Mathf.Clamp01((1-t)*4)));
                    return true;
                case "rolling-smash":
                    v.sprite.enabled=false;if(e.phase==2){SkillInkPart(v,0,"impact-smash",Mathf.Min(5,3+(int)(t*3)),ground,face,new Vector2(.67f,.29f),InkAlpha(1-t*.7f),42);return true;}
                    if(!NativePart(v,0,id,age,SourceTorso(e,0,32),face,44,true)&&!PaintedFallback(v,id,e,p,0,6,true))MinorPart(v,0,ring,p,Vector2.one*.65f,C(0xffe6c4,.6f),face);return true;
                case "windmill":
                    v.sprite.enabled=false;NativePart(v,0,id,age,RenderPlayerPosition,face,18,e.phase==1);
                    if(e.phase==1){bool spin=false;int emitted=Mathf.FloorToInt(age*30);for(int i=0;i<24;i++){
                            float particleAge=Mathf.Repeat(age,1/30f)+i/30f;
                            // Source Emitradius Y=random[-60,80]. A stable
                            // deterministic distribution preserves that height
                            // envelope without frame-to-frame random jitter.
                            float sourceY=Mathf.Lerp(-60,80,Mathf.Repeat((emitted-i)*.61803399f,1));
                            spin|=NativePart(v,i+1,"windmill-spin",age<i/30f?-1:particleAge,RenderPlayerPosition+Vector2.up*(32+sourceY*.28f),face);}
                        if(!spin&&!PaintedFallback(v,id,e,RenderPlayerPosition+Vector2.up*30,2,3,true))MinorPart(v,1,ring,RenderPlayerPosition+Vector2.up*27,new Vector2(.85f,.3f),C(0xffdfa6,.65f),face);}
                    else MinorPart(v,1,dust,RenderPlayerPosition+Vector2.up*3,new Vector2(.8f,.2f),C(0xe2d8bf,1-t),face,0,18);return true;
                case "unlimited-blade":
                    v.sprite.enabled=false;if(e.phase==0){if(!libraries.ContainsKey("nativefx-unlimited-blade"))MinorPart(v,0,hit,p,new Vector2(.4f,.6f),C(0xffedc1,1-t),face);}
                    else if(!NativePart(v,0,"unlimited-fire",age,new Vector2(p.x-face*32,p.y-65),face,43)){bool finisher=NativePart(v,0,"unlimited-finish",age,p,face),fire=NativeBurst(v,1,"flame-column",age,3,.01f,new Vector2(p.x-face*32,p.y-65),face,43);if(!finisher&&!fire)MinorPart(v,0,hit,p,new Vector2(.8f,1.6f),C(0xffb96b,1-t),face);}return true;
                case "assault-slash":
                    v.sprite.enabled=false;if(!NativeBurst(v,0,id,age,3,.03f,SourceTorso(e,30,27),face,42))MinorPart(v,0,dust,p-new Vector2(face*30,0),new Vector2(1.3f,.12f),C(0xf7e9ce,1-t),face);return true;
                case "mortal-blow":
                    v.sprite.enabled=false;if(e.phase==0){if(!NativePart(v,0,id,age,ground,face,42))MinorPart(v,0,dust,ground+Vector2.up*8,new Vector2(.6f,.8f),C(0xb6a492,1-t),face);}
                    else SkillInkPart(v,0,id,t<.25f?0:t<.67f?1:0,ground,face,new Vector2(.28f,.18f),
                                      InkAlpha(.68f*Mathf.Clamp01((1-t)*3)),heroView.sortingOrder-1);return true;
                case "impact-smash":
                    v.sprite.enabled=false;SkillInkPart(v,0,id,Mathf.Min(5,2+(int)(t*4)),ground,face,new Vector2(.66f,.58f),
                                              InkAlpha(Mathf.Clamp01((1-t)*4)),43);return true;
                case "armageddon-blade":
                    v.sprite.enabled=false;if(e.phase==0)MinorPart(v,0,ring,RenderPlayerPosition+Vector2.up*30,Vector2.one*(.3f+t*.5f),C(0xffe6a2,1-t),face);return true;
                case "sandstorm":
                    v.sprite.enabled=false;
                    // p follows the independent main body's DirSpeed1000,
                    // plus Velocity200 after.8s, already integrated by model.
                    bool storm=false;Vector2 stormOrigin=p+Vector2.up*(80*.28f-38);
                    // StormFX1 emits six timed pairs. The original 3D yaw/pitch
                    // is a planar projection here; depth and helper sub-bones
                    // require a full particle renderer, not a borrowed fire arc.
                    for(int burst=0;burst<6;burst++)for(int particle=0;particle<4;particle++){
                        float particleAge=age-stormTimes[burst]-particle*.1f;
                        float yawProjection=Mathf.Cos(-60*burst*Mathf.Deg2Rad);
                        float pitchProjection=Mathf.Cos((burst==2||burst==3?20:15)*Mathf.Deg2Rad);
                        int slot=(burst*4+particle)*2;
                        storm|=NativePart(v,slot,"sandstorm-body",particleAge,stormOrigin,face,43,false,0,stormScaleX[burst]*yawProjection,stormScaleY[burst]*pitchProjection);
                        storm|=NativePart(v,slot+1,"sandstorm-body-second",particleAge,stormOrigin,face,42,false,0,stormScaleX[burst]*yawProjection,stormScaleY[burst]*pitchProjection);
                    }
                    if(!storm)MinorPart(v,0,dust,p,new Vector2(1.3f,.8f),C(0xd2b99b,.35f*(1-t)),face);return true;
                case "gigantic-slash":
                    v.sprite.enabled=false;
                    // Its six FX sets have their own animation-time events;
                    // damage phase pulses must not replay an unrelated slash.
                    if(!libraries.ContainsKey("nativefx-gigantic-cast"))MinorPart(v,0,e.phase==0?ring:hit,p,e.phase==0?new Vector2(1.5f,.2f):e.phase==1?new Vector2(1.5f,.45f):new Vector2(1.4f,1.8f),C(0xffecc7,1-t),face);return true;
                default:return false;
            }
        }
        void RenderDoubleContact(EffectView view,Effect effect,Vector2 center,float age,float t){
            bool finish=effect.phase==1;
            int face=effect.facing;
            float angle=(finish ? -32 : 26)*face;
            // Impact is fixed to the struck body at contact. The blade and
            // charge still follow their original caster/weapon attachments.
            if(!NativePart(view,0,finish ? "double-flash-second" : "double-flash-cross",
                age,center,face,50,false,angle,finish ? .26f : .22f,finish ? .18f : .15f))
                MinorPart(view,0,hit,center,new Vector2(.45f,.22f),C(0xffe6ae,1-t),face,angle,50);
            else TintSkillPart(view,0,finish ? C(0xffdb9c,.86f) : C(0xffefb6,.84f));
            for(int i=0;i<(finish ? 3 : 2);i++){
                Vector2 ray=new Vector2(face*(i==0 ? 1 : i==1 ? -.45f : .6f),i==0 ? .35f : i==1 ? .7f : -.5f);
                Vector2 origin=center+ray*(3+(finish ? 24 : 15)*t);
                NativePart(view,i+1,"double-impact-spark",.045f+age,origin,face,51,false,angle+i*36*face,.53f,.32f);
                TintSkillPart(view,i+1,C(0xffd390,1-t));
            }
        }
        bool RenderedWeapon(out Vector2 hand,out float localAngle){
            hand=RenderPlayerPosition+new Vector2(game.player.facing*12,32);localAngle=0;
            if(!libraries.TryGetValue(catalog.hero.id,out var lib)||!lib.clips.TryGetValue(game.player.clip,out var clip)||clip.frames==null||clip.frames.Length==0)return false;
            float clock=clip.loop&&clip.duration>0?Mathf.Repeat(RenderPlayerClock,clip.duration):RenderPlayerClock;
            int index=clip==heroDrawClip?heroDrawFrame:Mathf.Clamp(Mathf.FloorToInt(clock*clip.fps),0,clip.frames.Length-1);
            var g=clip.frames[index].attackGeometry;
            if(g==null||g.weaponBase==null||g.weaponBase.Length<2)return false;var tip=g.weaponTip??g.weaponAttackEnd;if(tip==null||tip.Length<2)return false;
            hand=RenderPlayerPosition+new Vector2(game.player.facing*g.weaponBase[0],g.weaponBase[1]);localAngle=Mathf.Atan2(tip[1]-g.weaponBase[1],tip[0]-g.weaponBase[0])*Mathf.Rad2Deg;return true;
        }
        int HeroWeaponOrder {get{return heroView.sortingOrder+(game.player.clip=="walk"?-1:1);}}
        bool CastNative(int slot,string component,float age,Vector2 origin,int face,int order=43,float sx=1,float sy=1,float angle=0,bool keepDoubleBlend=false){
            if(!NativeSkillClip(component,out var lib,out var clip))return false;
            if(!NativeSkillFrameVisible(clip,age,false))return true;
            DrawCastComponent(slot,lib,clip,age,origin,face,order,sx,sy,angle,keepDoubleBlend);
            return true;
        }
        void DrawCastComponent(int slot,Library lib,AnimationData clip,float age,Vector2 origin,int face,int order,float sx,float sy,float angle,bool keepDoubleBlend){
            while(castComponents.Count<=slot){
                var item=WorldSprite("Source cast timeline",effectsRoot,null,order,Vector3.zero);
                item.enabled=false;
                castComponents.Add(item);
            }
            var renderer=castComponents[slot];bool restored=doubleOriginalMaterials.TryGetValue(renderer,out var original)&&!keepDoubleBlend;if(restored&&renderer.sharedMaterial!=original)renderer.sharedMaterial=original;
            DrawNativeSkillFrame(renderer,lib,clip,age,origin,face,order,false,angle,sx,sy);
            visibleCastComponents.Add(renderer);
            // A pooled slot can next display another skill. Keep an explicit
            // per-renderer atlas binding synchronized after material restore.
            if(restored&&renderer.sprite){if(doubleSpriteProperties==null)doubleSpriteProperties=new MaterialPropertyBlock();renderer.GetPropertyBlock(doubleSpriteProperties);doubleSpriteProperties.SetTexture("_MainTex",renderer.sprite.texture);renderer.SetPropertyBlock(doubleSpriteProperties);}
        }
        bool CastDouble(int slot,string component,float age,Vector2 origin,int face,int order=46,float sx=1,float sy=1,float angle=0,int destination=7){
            if(age<0)return true;
            if(!NativeSkillClip(component,out var lib,out var clip))return false;
            if(!NativeSkillFrameVisible(clip,age,false))return true;
            // Original particle counts overlap in depth. A sprite projection
            // collapses them into one plane; preserve the authored blade
            // layers while giving auxiliary lights a bounded screen footprint.
            float projectionScale=1,opacity=1;
            switch(component){
                case "double-first":case "double-first-red":case "double-first-core":case "double-first-trail":
                case "double-slash":case "double-second-red":case "double-second-core":
                    // The source hip is only 11-16 px above the feet, while
                    // the native blade radius projects to 120-142 px. Frame
                    // this chibi presentation above the ground/HUD as the
                    // blade rotates downward; keep the native mesh clock.
                    bool firstBlade=component.StartsWith("double-first",StringComparison.Ordinal);
                    float sweep=Mathf.SmoothStep(0,1,Mathf.Clamp01((age-.1f)/.1f));
                    // The opener is a close rising cut. Leave more lateral
                    // room for the delayed finishing stroke and its fire front.
                    projectionScale=firstBlade ? .49f : .68f;
                    sx*=firstBlade ? .94f : 1.07f;
                    sy*=firstBlade ? 1.02f : .93f;
                    origin+=new Vector2(face*(firstBlade ? 3 : 10)*sweep,Mathf.Lerp(18,firstBlade ? 57 : 68,sweep));
                    if(component=="double-first-trail"){opacity=.48f;order=HeroWeaponOrder-2;}
                    else if(component=="double-first-red" || component=="double-second-red")opacity=.78f;
                    else if(component=="double-first")opacity=.84f;
                    break;
                case "double-sword":projectionScale=.48f;opacity=.38f;break;
                case "double-streak":projectionScale=.18f;opacity=.22f;break;
                case "double-flash-cross":case "double-flash-second":projectionScale=.22f;opacity=.26f;break;
                case "double-flash":projectionScale=.34f;opacity=.14f;break;
                case "double-charge-ring":projectionScale=.46f;opacity=.47f;break;
                case "double-charge-rays":projectionScale=.17f;opacity=.29f;break;
                case "double-charge-light":projectionScale=.3f;opacity=.2f;break;
                case "double-charge-final":projectionScale=.41f;opacity=.44f;break;
                case "double-spark":projectionScale=.35f;opacity=.35f;break;
                case "double-impact-spark":projectionScale=.5f;opacity=.5f;break;
                case "double-fire-spark":projectionScale=.4f;opacity=.4f;break;
                case "double-fire-0":projectionScale=.24f;opacity=.38f;break;
                case "double-fire-1":projectionScale=.3f;opacity=.5f;break;
                case "double-fire-2":case "double-fire-3":projectionScale=.28f;opacity=.35f;break;
                case "double-fire-4":case "double-fire-end":projectionScale=.3f;opacity=.3f;break;
                case "double-fire-5":projectionScale=.35f;opacity=.3f;break;
            }
            sx*=projectionScale;sy*=projectionScale;
            DrawCastComponent(slot,lib,clip,age,origin,face,order,sx,sy,angle,true);var renderer=castComponents[slot];
            renderer.color=new Color(1,1,1,opacity);
            if(!doubleMaterials.TryGetValue(destination,out var material)){var shader=Resources.Load<Shader>("Pixel/Shaders/NativeSkillSprite");if(!shader)return true;
                material=new Material(shader);material.name="Double Slash source blend "+destination;material.SetInt("_SrcBlend",5);material.SetInt("_DstBlend",destination);doubleMaterials.Add(destination,material);}
            if(!doubleOriginalMaterials.ContainsKey(renderer))doubleOriginalMaterials.Add(renderer,renderer.sharedMaterial);
            if(renderer.sharedMaterial!=material)renderer.sharedMaterial=material;
            if(doubleSpriteProperties==null)doubleSpriteProperties=new MaterialPropertyBlock();renderer.GetPropertyBlock(doubleSpriteProperties);doubleSpriteProperties.SetTexture("_MainTex",renderer.sprite.texture);renderer.SetPropertyBlock(doubleSpriteProperties);return true;
        }
        void DisposeDoubleEffects(){foreach(var material in doubleMaterials.Values)if(material)Destroy(material);doubleMaterials.Clear();doubleOriginalMaterials.Clear();}
        static float DoubleFireDistance(float seconds){float t=Mathf.Clamp(seconds,0,.35f);return t<=.01f?0:t<=.2f?1000*(t-.01f)*(t-.01f)/.19f:190+2000*(t-.2f);}
        static Vector2 DoubleFireTravel(float seconds,int face){float distance=DoubleFireDistance(seconds)*.28f;return new Vector2(face*distance*.7094065f,-distance*.4967318f);}
        static float DoubleSample(int seed,int salt){return Mathf.Repeat((seed+1)*.61803399f+salt*.41421356f,1);}
        static Vector2 DoubleSparkTravel(float seconds,int seed,int kind,int face){
            float t=Mathf.Max(0,seconds),x=DoubleSample(seed,1),y=DoubleSample(seed,2),u=DoubleSample(seed,3),v=DoubleSample(seed,4);
            Vector2 a,b,c;
            if(kind==1){a=new Vector2(Mathf.Lerp(-1000,1800,x),Mathf.Lerp(-600,1500,y));float span=Mathf.Clamp(t-.1f,0,.3f);Vector2 impact=a*(Mathf.Min(t,.1f)+span-span*span/.6f)+Vector2.down*(500*t*t);return new Vector2(impact.x*face,impact.y)*.28f;}
            if(kind==2){a=new Vector2(Mathf.Lerp(-2500,500,x),Mathf.Lerp(-500,500,y));b=new Vector2(Mathf.Lerp(-1500,500,u),Mathf.Lerp(-300,400,v));c=new Vector2(Mathf.Lerp(-600,30,y),Mathf.Lerp(-200,1000,u));}
            else{a=new Vector2(Mathf.Lerp(-1100,1100,x),Mathf.Lerp(-900,900,y));b=new Vector2(Mathf.Lerp(-400,400,u),Mathf.Lerp(-100,400,v));c=new Vector2(Mathf.Lerp(-30,30,y),Mathf.Lerp(-200,700,u));}
            float first=Mathf.Clamp(t-.06f,0,.14f),second=Mathf.Clamp(t-.2f,0,.6f);Vector2 distance=a*Mathf.Min(t,.06f)+a*first+(b-a)*(first*first/.28f)+b*second+(c-b)*(second*second/1.2f)+c*Mathf.Max(0,t-.8f);
            return new Vector2(distance.x*face,distance.y)*.28f;
        }
        void RenderDoubleCast(float clock,int face,Vector2 torso){
            int slot=0;
            for(int i=0;i<3;i++)CastDouble(slot++,"double-first",clock-.304f-i/100f,torso,face);
            for(int i=0;i<2;i++)CastDouble(slot++,"double-first-red",clock-.304f-i/70f,torso,face,44,1,1,0,10);
            for(int i=0;i<2;i++)CastDouble(slot++,"double-first-core",clock-.304f-i/100f,torso,face);
            for(int i=0;i<3;i++)CastDouble(slot++,"double-first-trail",clock-.304f-i/30f,torso,face);
            for(int i=0;i<3;i++)CastDouble(slot++,"double-slash",clock-1.854f-i/100f,torso,face);
            for(int i=0;i<2;i++)CastDouble(slot++,"double-second-red",clock-1.854f-i/70f,torso,face,44,1,1,0,10);
            for(int i=0;i<2;i++)CastDouble(slot++,"double-second-core",clock-1.854f-i/100f,torso,face);
            RenderedWeapon(out var hand,out var angle);
            // During the second swing the broad arc turns edge-on while the
            // blade is still raised. Keep its outward edge legible against a
            // horizontal platform, following the animated hand in both facings.
            float release=clock-2.07f;
            if(release>=0 && release<.30f){
                float weight=Mathf.SmoothStep(0,1,Mathf.Clamp01(release/.055f))
                    *Mathf.SmoothStep(0,1,Mathf.Clamp01((.30f-release)/.12f));
                float radians=angle*Mathf.Deg2Rad;
                Vector2 blade=new Vector2(face*Mathf.Cos(radians),Mathf.Sin(radians));
                Vector2 origin=hand+blade*5;
                CastNative(slot,"double-sword",.12f+release,origin,face,49,.76f,1.1f,angle*face);
                castComponents[slot].color=new Color(1,1,1,.76f*weight);
                // The streak's registration is at the bottom of its local
                // vertical axis. Rotate it towards the same sword tip.
                CastNative(slot+1,"double-streak",.13f+release*.28f,origin,face,50,.24f,.24f,(angle-90)*face);
                castComponents[slot+1].color=new Color(1,.94f,.70f,.93f*weight);
            }
            slot+=2;
            // TRACE_MATRIX on the sword records each emitted hand transform.
            // Its native Z length is projected along local +X by the baker.
            for(int i=0;i<28;i++){float age=clock-.014f-i/10f;if(age>=0&&!doubleSwordBorn[i]){doubleSwordBorn[i]=true;doubleSwordOrigins[i]=hand;doubleSwordAngles[i]=angle;}
                CastDouble(slot++,"double-sword",age,doubleSwordOrigins[i],face,47,1,1,doubleSwordAngles[i]*face);}
            float charge=clock-.904f;
            for(int i=0;i<2;i++)CastDouble(slot++,"double-charge-ring",charge-i/15f,hand,face);
            for(int i=0;i<4;i++)CastDouble(slot++,"double-charge-rays",charge-i/30f,hand,face,45,1,1,Mathf.Repeat(i*137.50776f,360)*face);
            CastDouble(slot++,"double-charge-light",charge,hand,face);
            for(int i=0;i<3;i++)CastDouble(slot++,"double-charge-final",charge-.6f-i/50f,hand,face,46,1,1,Mathf.Repeat(i*137.50776f,360)*face);
            for(int burst=0;burst<2;burst++){
                float age=charge-burst*.6f;
                for(int i=0;i<2;i++)CastDouble(slot++,"double-flash",age-i/60f,hand,face,47);
                for(int i=0;i<4;i++){float particleAge=age-i/60f;CastDouble(slot++,"double-spark",particleAge,hand+DoubleSparkTravel(particleAge,i,0,face),face,48);}
            }
            for(int stage=0;stage<2;stage++){float age=clock-(stage==0?.404f:1.954f);Vector2 center=torso+new Vector2(face*70,stage==0?11.2f:0);
                for(int i=0;i<2;i++)CastDouble(slot++,"double-flash",age-i/60f,center,face,47,2,2);
                CastDouble(slot++,"double-flash-cross",age,center,face,46,2,2,0,1);
                if(stage==1)CastDouble(slot++,"double-flash-second",age,center,face,46,2,2,0,1);
                for(int i=0;i<4;i++)CastDouble(slot++,"double-streak",age-i/60f,center,face,45,1,1,Mathf.Repeat(i*137.50776f,360)*face);
                if(stage==0)for(int i=0;i<6;i++){float particleAge=age-i/60f;CastDouble(slot++,"double-impact-spark",particleAge,center+DoubleSparkTravel(particleAge,i,1,face),face,46,2,2);}
            }
            float fireAge=clock-2.154f;
            if(fireAge>=0&&!doubleFireOriginSet){doubleFireOrigin=RenderPlayerPosition+new Vector2(face*28,84);doubleFireFacing=face;doubleFireOriginSet=true;}
            if(!doubleFireOriginSet)return;
            // FireFX is a moving helper, not another caster-centered arc.
            // Source yaw -30 and roll -35 are baked before XY projection;
            // DirSpeed ramps 0->2000 at .01-.2 and stops at .35 seconds.
            for(int part=0;part<6;part++){int count=part==0?20:part==1?16:part==2||part==3?15:part==4?5:3;float rate=part==5?50:30;
                int newest=Mathf.Min(count-1,Mathf.FloorToInt(fireAge*rate));
                for(int sample=0;sample<2;sample++){int i=newest-sample*3;float emitted=i/rate,age=i<0?-1:fireAge-emitted,radius=part==0?Mathf.Lerp(90,160,Mathf.Repeat(i*.618034f,1)):part==4?150:part==3?5.5f:part==5?0:5.5f;
                    Vector2 origin=doubleFireOrigin+DoubleFireTravel(emitted,doubleFireFacing)+new Vector2(doubleFireFacing*radius*.28f*.7094065f,-radius*.28f*.4967318f);
                    CastDouble(slot++,doubleFireParts[part],age,origin,doubleFireFacing,46+(part>=4?1:0),part==5?1:1.2f,part==5?1:1.2f);}}
            for(int i=0;i<3;i++)CastDouble(slot++,"double-fire-5",fireAge-.2f-i/50f,doubleFireOrigin+DoubleFireTravel(.2f+i/50f,doubleFireFacing),doubleFireFacing,47,1.2f,1.2f);
            CastDouble(slot++,"double-fire-end",fireAge-.6f,doubleFireOrigin+DoubleFireTravel(.6f,doubleFireFacing),doubleFireFacing,47,1.2f,1.2f);
            for(int i=0;i<8;i++){float emitted=i/15f,age=fireAge-emitted;Vector2 radius=new Vector2(Mathf.Lerp(-150,150,DoubleSample(i,5))*doubleFireFacing,Mathf.Lerp(-150,150,DoubleSample(i,6)))*.28f;
                CastDouble(slot++,"double-fire-spark",age,doubleFireOrigin+DoubleFireTravel(emitted,doubleFireFacing)+radius+DoubleSparkTravel(age,i,2,doubleFireFacing),doubleFireFacing,48,1.2f,1.2f);}
        }
        void RenderCastComponents(){
            foreach(var r in visibleCastComponents)r.enabled=false;
            visibleCastComponents.Clear();
            string id=game.ActiveSkillId;float clock=RenderPlayerClock;
            if(id!=sourceTimelineSkill||game.room!=sourceTimelineRoom||clock+.001f<sourceTimelineClock){sonicOriginSet=false;doubleFireOriginSet=false;Array.Clear(doubleSwordBorn,0,doubleSwordBorn.Length);sourceTimelineSkill=id;sourceTimelineRoom=game.room;}
            sourceTimelineClock=clock;if(game.player.hp<=0)return;
            int face=game.player.facing;Vector2 torso=RenderPlayerPosition+Vector2.up*32;
            if(id=="double-slash"){
                float frame=Mathf.Clamp(clock*60,0,doubleHipY.Length-1);int index=Mathf.FloorToInt(frame);
                float hip=Mathf.Lerp(doubleHipY[index],doubleHipY[Mathf.Min(index+1,doubleHipY.Length-1)],frame-index);
                RenderDoubleCast(clock,face,RenderPlayerPosition+Vector2.up*hip);
            }else if(id=="sonic-blade"){
                // EffectSet binds at .001 animation seconds, SonicBladeM1 at .3.
                float age=clock-.301f/.85f;
                if(age>=0&&!sonicOriginSet){sonicOrigin=torso+Vector2.up*(20*.28f);sonicFacing=face;sonicOriginSet=true;}
                if(sonicOriginSet)for(int i=0;i<3;i++)CastNative(i,"sonic-blade",age-i/80f,sonicOrigin,sonicFacing);
            }else if(id=="sandstorm"){
                // Startup meshes belong to the caster. Only StormFX follows
                // the independent damage helper's moving position.
                for(int i=0;i<2;i++)CastNative(i,"sandstorm-ring",clock-.4333f-i/70f,torso,face,42);
                for(int i=0;i<3;i++)CastNative(i+2,"sandstorm",clock-.8833f-i/100f,torso,face);
            }else if(id=="unlimited-blade"){
                // M01 / M02 / M03 / M03 / M04 are five different authored
                // visual events; the six contact windows do not spawn M01 six times.
                for(int part=0;part<unlimitedVisualTimes.Length;part++)for(int particle=0;particle<(part==4?3:2);particle++)
                    CastNative(part*3+particle,unlimitedVisualParts[part],clock-unlimitedVisualTimes[part]-particle/80f,torso,face);
            }else if(id=="gigantic-slash"){
                Vector2 origin=RenderPlayerPosition;float sourceClock=clock*1.15f;
                CastNative(0,"gigantic-cast",(sourceClock-1.066f)/1.15f,origin,face);
                CastNative(1,"gigantic-cast-rise",(sourceClock-1.9993f)/1.15f,origin,face);
                for(int i=0;i<giganticWaveTimes.Length;i++)CastNative(i+2,"gigantic-wave",(sourceClock-giganticWaveTimes[i])/1.15f,origin,face);
                for(int i=0;i<giganticFinalTimes.Length;i++)CastNative(i+7,"gigantic-wave-final",(sourceClock-giganticFinalTimes[i])/1.15f,origin,face);
                // FX6 explicitly scales these last two source meshes.
                CastNative(12,"gigantic-wave",(sourceClock-7.299f)/1.15f,origin,face,43,2,1);
                CastNative(13,"gigantic-wave-final",(sourceClock-7.466f)/1.15f,origin,face,43,1,2);
            }
        }
        void RenderSkillAttachments(){
            RenderCastComponents();
            RenderMotionSkillInk();
            RenderRegionalEffects();
            if(armageddonAttachment)armageddonAttachment.enabled=false;if(giganticAttachment)giganticAttachment.enabled=false;
            if(game.player.hp<=0||(!game.ArmageddonActive&&game.ActiveSkillId!="gigantic-slash"))return;
            RenderedWeapon(out var hand,out var angle);
            if(game.ArmageddonActive){if(!armageddonAttachment)armageddonAttachment=WorldSprite("Armageddon source sword",effectsRoot,null,44,Vector3.zero);
                // The mesh appears at 1.1, the 25s buff starts at 1.68.
                // Keep its phase continuous when control returns to the player.
                float age=game.ArmageddonRemaining>0?25-game.ArmageddonRemaining+(.58f/1.3f):Mathf.Max(0,RenderPlayerClock-1.1f/1.3f);
                if(!NativeFrame(armageddonAttachment,"armageddon-blade",age,hand,game.player.facing,HeroWeaponOrder,true,angle*game.player.facing)){
                    // Reduced packs still show the actual extended segment;
                    // this thin energy-line fallback is not a native texture.
                    float center=(GameModel.ArmageddonAttackStart+GameModel.ArmageddonAttackEnd)*.5f,length=GameModel.ArmageddonAttackEnd-GameModel.ArmageddonAttackStart;
                    Vector2 q=hand+RotateSkillOffset(new Vector2(game.player.facing*center,0),angle*game.player.facing);
                    armageddonAttachment.sprite=white;armageddonAttachment.enabled=true;armageddonAttachment.sortingOrder=HeroWeaponOrder;armageddonAttachment.color=C(0xffe7a6,.75f);armageddonAttachment.transform.localPosition=new Vector3(q.x,q.y,0);armageddonAttachment.transform.localScale=new Vector3(length/2,1.5f,1);armageddonAttachment.transform.localRotation=Quaternion.Euler(0,0,angle*game.player.facing);
                }}
            if(game.ActiveSkillId=="gigantic-slash"){
                if(!giganticAttachment)giganticAttachment=WorldSprite("Gigantic source animated sword",effectsRoot,null,44,Vector3.zero);
                // FX2 binds at1.066 animation seconds, P18 waits1 more.
                // TRACE follows the hand; no TRACE_MATRIX: its own decoded
                // animated mesh must not be rotated a second time by the hand.
                NativeFrame(giganticAttachment,"gigantic-sword",RenderPlayerClock-(1.066f+1)/1.15f,hand,game.player.facing,44);
            }
        }
    }
}
