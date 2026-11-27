using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        readonly Dictionary<Effect,float> previousEffectAges=new Dictionary<Effect,float>();
        float RenderedEffectAge(Effect e){return !calibrationRun&&previousEffectAges.TryGetValue(e,out var old)&&e.age>=old?Mathf.Lerp(old,e.age,RenderAlpha):e.age;}
        Vector2 previousPlayer;float previousClock;string previousClip;int previousRoom=-1,previousCargoProjectionChartSerialR1479;
        readonly Dictionary<Enemy,Vector2> previousEnemies=new Dictionary<Enemy,Vector2>();
        SpriteRenderer sceneDim;Material armoredMaterial;MaterialPropertyBlock armorProperties;
        int presentationCastSerial;float castBannerTime,lastPresentationTime=-1;bool wasAwakeningCasting;
        bool showPerformance;float lastCaptureRealtime=-100,performanceStart;readonly List<float> frameTimes=new List<float>(16384);
        readonly float[] mixedAudioSamples=new float[512];int audioMixedFrames;float audioMixPeak;
        public float PresentationDimAlpha {get{return sceneDim&&sceneDim.enabled?sceneDim.color.a:0;}}
        public bool HeroOutlineActive {get;private set;}
        float RenderAlpha {get{return calibrationRun?1:Mathf.Clamp01(accumulator*60);}}
        Vector2 RenderPlayerWorldPosition {get{return previousRoom==game.room&&previousCargoProjectionChartSerialR1479==game.CargoProjectionChartSerialR1479?Vector2.Lerp(previousPlayer,new Vector2(game.player.x,game.player.y),RenderAlpha):new Vector2(game.player.x,game.player.y);}}
        Vector2 RenderPlayerPosition {get{return RenderPlayerWorldPosition+Vector2.right*game.SkillPresentationRootX(RenderPlayerClock);}}
        float RenderPlayerClock {get{return previousRoom==game.room&&previousClip==game.player.clip&&game.player.clock>=previousClock?Mathf.Lerp(previousClock,game.player.clock,RenderAlpha):game.player.clock;}}
        AnimationData heroDrawClip;
        int heroDrawFrame, gaitStartFrame;
        RoomDefinition heroDrawRoom;
        float previousHeroVy;

        // The walk cycle uses a planted toe path (build_hero_walk.mjs); the other
        // clips already carry ankle positions in actor-local coordinates.
        static Vector2 FootSeparation(AnimationData clip,int frame){
            if(clip.id=="walk"){
                float phase=frame/(float)clip.frames.Length+.12f;
                return WalkToe(phase)-WalkToe(phase+.5f);
            }
            var g=clip.frames[frame].attackGeometry;
            if(g?.rightFoot==null||g.rightFoot.Length<2||g.leftFoot==null||g.leftFoot.Length<2)return Vector2.zero;
            return new Vector2(g.rightFoot[0]-g.leftFoot[0],g.rightFoot[1]-g.leftFoot[1]);
        }
        static Vector2 WalkToe(float phase){
            float p=Mathf.Repeat(phase,1),stride=GameModel.GroundWalkSpeed*(40f/60),lead=stride*.52f/2;
            if(p<=.52f)return new Vector2(lead-stride*p,0);
            float t=(p-.52f)/.48f,lift=Mathf.Sin(Mathf.PI*t);
            return new Vector2(Mathf.SmoothStep(-lead,lead,t)-stride*.48f*t*(1-t)*(1-2*t),5*lift*lift);
        }
        int HeroPoseFrame(AnimationData clip,int frame,float clock){
            int count=clip.frames.Length;
            if(clip.id=="walk"||clip.id=="run"){
                if(clip!=heroDrawClip||heroDrawRoom!=game.CurrentRoom){
                    gaitStartFrame=0;
                    if(heroDrawClip!=null&&heroDrawRoom==game.CurrentRoom){
                        var stance=FootSeparation(heroDrawClip,heroDrawFrame);float best=float.PositiveInfinity;
                        for(int i=0;i<count;i++){
                            float difference=(FootSeparation(clip,i)-stance).sqrMagnitude;
                            if(difference<best){best=difference;gaitStartFrame=i;}
                        }
                    }
                }
                frame=(frame+gaitStartFrame)%count;
            }else if(clip.id=="jump-rise"||clip.id=="jump-fall"||clip.id=="dash-jump"){
                float vy=previousRoom==game.room&&previousClip==clip.id?Mathf.Lerp(previousHeroVy,game.player.vy,RenderAlpha):game.player.vy;
                float phase=clip.id=="dash-jump"?Mathf.InverseLerp(644,-560,vy):
                    clip.id=="jump-rise"?Mathf.InverseLerp(320,0,vy):Mathf.Min(Mathf.InverseLerp(0,-420,vy),clock*9);
                frame=Mathf.RoundToInt(phase*(count-1));
            }
            heroDrawRoom=game.CurrentRoom;heroDrawClip=clip;heroDrawFrame=Mathf.Clamp(frame,0,count-1);
            return heroDrawFrame;
        }
        Vector2 RenderEnemyPosition(Enemy enemy){return previousRoom==game.room&&previousEnemies.TryGetValue(enemy,out var old)?Vector2.Lerp(old,new Vector2(enemy.x,enemy.y),RenderAlpha):new Vector2(enemy.x,enemy.y);}
        void RememberSimulationState(){
            previousHeroVy=game.player.vy;
            // Establish the audio baseline before the first input after reset.
            // Doing this only after Step would consume instant Iron Body's cue.
            if(audioInitialized&&(game.elapsed+.0001f<audioLastModelTime||game.room!=audioRoom))ResetAudioTracking();
            previousEffectAges.Clear();foreach(var e in game.effects)previousEffectAges[e]=e.age;
            RememberNpcEffectState();
            previousRoom=game.room;previousCargoProjectionChartSerialR1479=game.CargoProjectionChartSerialR1479;previousPlayer=new Vector2(game.player.x,game.player.y);previousClock=game.player.clock;previousClip=game.player.clip;previousEnemies.Clear();foreach(var e in game.enemies)previousEnemies[e]=new Vector2(e.x,e.y);
        }
        void BuildCombatHud(){
            var orbSprite=MakeShape("Awakening crystal",(x,y)=>Math.Abs(x-7)+Math.Abs(y-7)<=6,15,15);
            for(int i=0;i<3;i++){var orb=Panel("Awakening Orb "+i,new Rect(18+i*20,426,16,16),C(0x542a30,.72f));orb.sprite=orbSprite;}
            Panel("Awakening Gauge Track",new Rect(18,418,56,3),C(0x48242b,.72f));Panel("Awakening Gauge",new Rect(18,418,1,3),C(0xff4e61,.72f));
            Button("Awakening Button","CTRL",new Rect(22,390,48,21),"awaken",9);
            safeRoot.Find("Awakening Button").GetComponent<UnityEngine.UI.Image>().color=C(0x8f2938,.75f);
            safeRoot.Find("Awakening Button/Awakening Button Text").GetComponent<UnityEngine.UI.Text>().color=C(0xffd4d6);
            foreach(string id in new[]{"Blade","Awakened"}){float y=id=="Blade"?391:363;Panel(id+" Buff Panel",new Rect(87,y,260,24),C(id=="Awakened"?0x42252cu:0x252c42u,.9f));var icon=Panel(id+" Buff Icon",new Rect(91,y+2,20,20),Color.white);icon.sprite=id=="Blade"?SkillIcon("armageddon-blade"):orbSprite;if(id=="Awakened")icon.color=C(0xff5263);Label(id+" Buff","",new Rect(115,y+1,228,22),11,id=="Awakened"?C(0xffa0a5):C(0xe9b4ff),TextAnchor.MiddleLeft);}
            Label("Performance","",new Rect(707,333,237,22),11,C(0x95f2d4),TextAnchor.MiddleRight);
            Button("Sound Button","SOUND M",new Rect(802,427,82,24),"sound",9);
            Panel("Cast Banner",new Rect(397,345,285,56),C(0x111b31,.88f));var castIcon=Panel("Cast Icon",new Rect(405,351,44,44),Color.white);castIcon.preserveAspect=true;
            Label("Cast Name","",new Rect(457,359,218,30),19,C(0xffe5bd),TextAnchor.MiddleLeft);
        }
        // Serialized shipping scenes already contain these widgets, so their
        // current styling must be restored when views are bound as well.
        void RefreshAwakeningHudStyle(){
            if(images.TryGetValue("Awakening Gauge Track",out var track))track.color=C(0x48242b,.72f);
            if(images.TryGetValue("Awakening Gauge",out var gauge))gauge.color=C(0xff4e61,.72f);
            if(images.TryGetValue("Awakened Buff Panel",out var panel))panel.color=C(0x42252c,.9f);
            if(images.TryGetValue("Awakened Buff Icon",out var icon))icon.color=C(0xff5263);
            if(labels.TryGetValue("Awakened Buff",out var buff))buff.color=C(0xffa0a5);
            if(images.TryGetValue("Awakening Button",out var button)){
                button.color=C(0x8f2938,.75f);button.rectTransform.anchoredPosition=new Vector2(22,390);button.rectTransform.sizeDelta=new Vector2(48,21);
            }
            if(labels.TryGetValue("Awakening Button Text",out var caption)){
                caption.color=C(0xffd4d6);caption.fontSize=9;caption.rectTransform.sizeDelta=new Vector2(48,21);
            }
        }
        void EnsurePresentation(){
            if(!sceneDim){sceneDim=WorldSprite("Skill full-screen dim",effectsRoot,white,39,Vector3.zero);sceneDim.transform.localScale=new Vector3(240,135,1);}
            if(!armoredMaterial){var shader=Resources.Load<Shader>("Pixel/Shaders/ArmoredSprite");if(shader){armoredMaterial=new Material(shader);armorProperties=new MaterialPropertyBlock();}}
        }
        static readonly int monsterInkProperty=Shader.PropertyToID("_MonsterInk");
        static readonly int burnTintProperty=Shader.PropertyToID("_BurnTint");
        readonly List<SpriteRenderer> armorBreakIcons=new List<SpriteRenderer>();

        void RenderArmorBreakIcon(int index,Enemy actor,SkillDebuff debuff){
            var label=enemyDebuffViews[index];
            var icon=armorBreakIcons[index];
            bool visible=actor.active&&!actor.dormant&&actor.hp>0&&debuff!=null&&debuff.armorBreak>0&&!string.IsNullOrEmpty(label.text);
            if(visible){
                if(!icon){
                    icon=WorldSprite("Armor Break",label.transform,Picture("Pixel/ui/armor-break-debuff"),51,Vector3.zero);
                    icon.transform.localScale=Vector3.one*.72f;
                    armorBreakIcons[index]=icon;
                }
                label.text="";
            }
            if(icon&&icon.gameObject.activeSelf!=visible)icon.gameObject.SetActive(visible);
        }
        void SetArmorOutline(SpriteRenderer renderer,bool active,Fighter fighter=null,bool burning=false){
            if(!renderer||!armoredMaterial)return;if(renderer.sharedMaterial!=armoredMaterial)renderer.sharedMaterial=armoredMaterial;
            renderer.GetPropertyBlock(armorProperties);if(renderer.sprite)armorProperties.SetTexture("_MainTex",renderer.sprite.texture);
            bool monster=fighter is Enemy npc&&!npc.passive&&!npc.isGate&&!string.IsNullOrEmpty(npc.kind)&&!npc.kind.StartsWith("prop-",StringComparison.Ordinal)&&!GameModel.IsRecoveryContainer(npc);
            armorProperties.SetFloat(monsterInkProperty,monster?1:0);
            armorProperties.SetFloat(burnTintProperty,burning?.52f:0);
            armorProperties.SetFloat("_Outline",active?1:0);armorProperties.SetColor("_OutlineColor",C(0xff2444));
            float pulse=fighter!=null&&fighter.impactFlashDuration>0?Mathf.Clamp01(fighter.impactFlash/fighter.impactFlashDuration):0;
            armorProperties.SetFloat("_HitFlash",pulse>.45f?.65f:pulse/.45f*.65f);renderer.SetPropertyBlock(armorProperties);
        }
        void RenderCombatPresentation(){
            EnsurePresentation();if(game.elapsed<lastPresentationTime){presentationCastSerial=0;castBannerTime=0;wasAwakeningCasting=false;}
            lastPresentationTime=game.elapsed;
            if(game.SkillCastSerial!=presentationCastSerial){presentationCastSerial=game.SkillCastSerial;castBannerTime=.65f;foreach(var skill in game.Skills)if(skill.id==game.LastCastSkillId){TextValue("Cast Name",skill.name);if(images.TryGetValue("Cast Icon",out var icon)){icon.sprite=SkillIcon(skill.id);icon.color=Color.white;}}}
            if(game.AwakeningCasting&&!wasAwakeningCasting){castBannerTime=1.1f;TextValue("Cast Name","AWAKENING");if(images.TryGetValue("Cast Icon",out var icon)){icon.sprite=images["Awakening Orb 0"].sprite;icon.color=C(0xff5263);}}
            wasAwakeningCasting=game.AwakeningCasting;if(!game.paused)castBannerTime=Mathf.Max(0,castBannerTime-Time.unscaledDeltaTime);
            foreach(string id in new[]{"Cast Banner","Cast Icon","Cast Name"})Visible(id,castBannerTime>0&&string.IsNullOrEmpty(openMenu));
            // Windmill's source loop has no CHANGE_WORLD_COLOR. Only its startup
            // gets this requested demo presentation cue; the moving loop stays lit.
            float dim=game.AwakeningCasting?.72f:game.CastScreenDim;
            if(game.ActiveSkillId=="windmill"&&game.player.clip=="windmill-start"&&game.player.clock<.366f)dim=Mathf.Max(dim,.65f);
            sceneDim.color=new Color(0,0,0,dim);sceneDim.enabled=dim>0;sceneDim.transform.localPosition=new Vector3(worldCamera.transform.localPosition.x,worldCamera.transform.localPosition.y,0);
            heroView.sortingOrder=dim>0?100:20;HeroOutlineActive=game.PlayerHasSuperArmor;SetArmorOutline(heroView,HeroOutlineActive,game.player);
            for(int i=0;i<enemyViews.Count;i++){
                var enemy=game.enemies[i];
                var debuff=game.GetEnemyDebuff(enemy);
                SetArmorOutline(enemyViews[i],game.EnemyShowsArmorOutline(enemy),enemy,enemy.hp>0&&debuff!=null&&debuff.burn>0);
                RenderArmorBreakIcon(i,enemy,debuff);
            }
            RenderAwakeningFlame();
            if(armageddonAttachment)armageddonAttachment.sortingOrder=HeroWeaponOrder;
            for(int i=0;i<3;i++)if(images.TryGetValue("Awakening Orb "+i,out var orb))orb.color=C(i<game.AwakeningOrbs?0xff6280u:0x80758cu);
            Width("Awakening Gauge",VitalsBarWidth*Mathf.Clamp01(game.AwakeningGauge/GameModel.AwakeningChargePerOrb));
            TextValue("Awakening Button Text",game.AwakeningActive?Number(game.AwakeningRemaining)+"s":"CTRL");
            foreach(string id in new[]{"Blade Buff Panel","Blade Buff Icon","Blade Buff"})Visible(id,game.ArmageddonActive);
            TextValue("Blade Buff",game.ArmageddonRemaining>0?"ARMAGEDDON  "+Number(game.ArmageddonRemaining)+"s":"ARMAGEDDON / SUMMONING");
            foreach(string id in new[]{"Awakened Buff Panel","Awakened Buff Icon","Awakened Buff"})Visible(id,game.AwakeningActive);
            TextValue("Awakened Buff","AWAKENED  "+Number(game.AwakeningRemaining)+"s  /  DMG +20%");
            if(labels.TryGetValue("Combat State",out var status))status.rectTransform.anchoredPosition=
                new Vector2(Mathf.Max(84,comboStatusEdge+AwakeningKeyClearance),405);
            TextValue("Sound Button Text",AudioMuted?"SOUND OFF":"SOUND ON");
            TextValue("System Hint","CTRL awake  M sound  K skills  F1 combos  F4 FPS  ESC pause");
        }
        [Serializable] sealed class PerformanceReport {public int frames,animationFps=60,simulationFps=60,targetFps,vSync,audioImpactPlays,audioHurtPlays,audioLoaded,audioPlays,audioPeakVoices,audioMixedFrames,npcSoundsConsumed,npcSoundsPlayed,npcSoundsMuted,npcSoundsCulled,npcSoundsMissing;public int[] materialImpactCues;public string[] materialImpactNames={"flesh","wood","shell","metal","spirit"};public float averageMs,p95Ms,maxMs,audioMixPeak;public string sampling="Unscaled rendered frame deltas; 2s warm-up; excludes paused/menu frames and 250ms after screenshot IO. Audio peak samples the actual Unity AudioListener output, not device loopback. NPC/material counts are cumulative consumer requests, not device-specific audibility.";}
        void SamplePerformance(){
            bool capture=runAutomatedPlaythrough||calibrationRun||feedbackReview||rubenReview;
            if(capture&&frame%4==0&&AudioManifestReady&&!AudioMuted){AudioListener.GetOutputData(mixedAudioSamples,0);float peak=0;foreach(float sample in mixedAudioSamples)peak=Mathf.Max(peak,Mathf.Abs(sample));audioMixPeak=Mathf.Max(audioMixPeak,peak);if(peak>.00001f)audioMixedFrames++;}
            if(performanceStart==0)performanceStart=Time.realtimeSinceStartup;
            float ms=Time.unscaledDeltaTime*1000;if(capture&&Time.realtimeSinceStartup-performanceStart>2&&Time.realtimeSinceStartup-lastCaptureRealtime>.25f&&!game.paused&&string.IsNullOrEmpty(openMenu)&&frameTimes.Count<20000)frameTimes.Add(ms);
            ObserveMobileFrameBudget(Time.unscaledDeltaTime);
            Visible("Performance",showPerformance);RefreshFrameRateDisplay(Time.unscaledDeltaTime);
        }
        void WritePerformanceReport(){
            if(string.IsNullOrEmpty(captureDir)||frameTimes.Count==0)return;var samples=frameTimes.ToArray();Array.Sort(samples);float sum=0;foreach(var ms in samples)sum+=ms;
            var report=new PerformanceReport{frames=samples.Length,targetFps=Application.targetFrameRate,vSync=QualitySettings.vSyncCount,averageMs=sum/samples.Length,p95Ms=samples[Math.Min(samples.Length-1,(int)(samples.Length*.95f))],maxMs=samples[samples.Length-1],audioImpactPlays=AudioImpactPlays,audioHurtPlays=AudioHurtPlays,audioLoaded=AudioLoadedClips,audioPlays=AudioPlayCount,audioPeakVoices=AudioPeakVoices,audioMixedFrames=audioMixedFrames,audioMixPeak=audioMixPeak,materialImpactCues=(int[])AudioMaterialImpactPlays.Clone(),npcSoundsConsumed=NpcAudioConsumed,npcSoundsPlayed=NpcAudioPlayed,npcSoundsMuted=NpcAudioMuted,npcSoundsCulled=NpcAudioCulled,npcSoundsMissing=NpcAudioMissing};Directory.CreateDirectory(captureDir);File.WriteAllText(Path.Combine(captureDir,"performance.json"),JsonUtility.ToJson(report,true));
        }
        void DisposePresentation(){DisposeDoubleEffects();DisposeCombatHudArt();if(armoredMaterial)Destroy(armoredMaterial);}
    }
}
