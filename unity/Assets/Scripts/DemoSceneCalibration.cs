using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        // Shipping Input/model/renderer path; targets are configured at the start.
        // No hitbox enlargement, damage edits or in-flight target teleporting.
        bool calibrationRun,calibrationDone,calibrationPendingNext,calibrationQueued,calibrationSkillPressed,calibrationFollowupPressed,calibrationOriginalMute;
        int calibrationIndex,calibrationTick,calibrationStep,calibrationSeenTick,calibrationChargeTick,calibrationCastTick,calibrationIntroFrames;
        int calibrationCaptureCount,calibrationCaptureTick,calibrationHitsBeforeFollowup,calibrationAudioBefore,calibrationLoadedMenuIcons;
        float calibrationBusyEnd,calibrationAccumulator;
        readonly List<CalibrationCase> calibrationCases=new List<CalibrationCase>();
        readonly List<CalibrationResult> calibrationResults=new List<CalibrationResult>();
        readonly List<CalibrationReference> calibrationReferences=new List<CalibrationReference>();
        readonly List<CalibrationSlotBinding> calibrationSlotBindings=new List<CalibrationSlotBinding>();
        readonly HashSet<string> calibrationExtraCaptures=new HashSet<string>();
        CalibrationResult calibrationResult;
        [Serializable] sealed class CalibrationCase {public string id,sequence,skill;public string[] clips;public bool dash,air,charge,awakenedSkill;public int awakeningOrbs,slot;}
        [Serializable] sealed class CalibrationSlotBinding {public int slot;public string key,hudKey,skill,actualTexture,expectedTexture;public bool menuMatches,hudMatches;public float hudX,hudY,hudWidth,hudHeight;}
        [Serializable] sealed class CalibrationFrame {
            public int tick,hits,sourceFrame,inputSkill,inputHeldMask,inputReleasedMask;public string clip,targetState,phase;public float clock,x,y,vy,targetX,targetY,targetHp,worldStop,screenDim,awakeningRemaining;
            public bool superArmor,invulnerable,awakened;public float targetImpactFlash,targetStun,presentationRootX;
        }
        [Serializable] sealed class CalibrationReference {
            public string capture,clip,actualSprite,expectedSprite,texture,targetShader;public int sourceFrame,sourceFps,castVisibleLayers,facing;public float sourceClock,actorX,actorY,renderX,renderY,physicalRenderX,presentationRootX,spriteBottom,spriteTop,presentationDimAlpha,targetImpactFlash,targetShaderFlash,bodyScreenX,bodyScreenY,bodyScreenWidth,bodyScreenHeight;public bool matchesSourceFrame;
        }
        [Serializable] sealed class CalibrationResult {
            public string id;public bool reachedFinal,timedOut,armageddonFollowupHit,sawWorldStop,sawScreenDim,sawSuperArmor,sawAwakening;
            public int hits,audioEvents,slot;public float seconds,displacement,maxY,spinMaxClock,awakeningDuration;
            public List<string> visited=new List<string>();public List<CalibrationFrame> frames=new List<CalibrationFrame>();public List<CombatEvent> events=new List<CombatEvent>();
        }
        [Serializable] sealed class CalibrationReport {
            public string scope="Actual Unity Player inputs, 60 FPS source sprite selection, movement, effects and damage. Configured high-HP practice targets; not original-client runtime measurements.";
            public int captures,skillCount,skillSlotCount,commandCount,loadedMenuIcons,audioLoaded,audioPlays,audioPeakVoices,audioDropped;public bool audioReady,audioWasMuted;
            public List<CalibrationResult> cases;public List<CalibrationReference> sourceReferences;public List<CalibrationSlotBinding> slotBindings;
        }
        void BeginCalibration(){
            Directory.CreateDirectory(captureDir);calibrationCases.Clear();calibrationResults.Clear();calibrationReferences.Clear();calibrationSlotBindings.Clear();
            AddCalibration("ground-zzzz","ZZZZ",new[]{"combo-z1","combo-z2","combo-z3","combo-z4"});
            AddCalibration("ground-zzzx","ZZZX",new[]{"combo-z1","combo-z2","combo-z3","combo-z-launch"});
            AddCalibration("ground-forward","ZZZZ",new[]{"combo-z1","combo-z2","combo-z3","combo-z-front"});
            AddCalibration("ground-xxz","XXZ",new[]{"combo-x1","combo-x2","combo-x-launch"});
            AddCalibration("ground-xxxxx","XXXXX",new[]{"combo-x1","combo-x2","combo-x3","combo-x4","combo-x5"});
            AddCalibration("dash-zzz","ZZZ",new[]{"dash-z1","dash-z2","dash-z3"},true);
            AddCalibration("dash-zzx","ZZX",new[]{"dash-z1","dash-z2","dash-z-launch"},true);
            AddCalibration("dash-zxx","ZXX",new[]{"dash-z1","dash-zx","dash-zxx"},true);
            AddCalibration("dash-xxx","XXX",new[]{"dash-x1","dash-xx","dash-xxx"},true);
            AddCalibration("air-z","Z",new[]{"air-z"},false,true);
            AddCalibration("air-x","X",new[]{"air-x"},false,true);
            AddCalibration("dash-air-zzz","ZZZ",new[]{"dash-air-z1","dash-air-z2","dash-air-z3"},true,true);
            AddCalibration("dash-air-xxx","XXX",new[]{"dash-air-x1","dash-air-x2","dash-air-x3"},true,true);
            AddCalibration("dash-air-xxz","XXZ",new[]{"dash-air-x1","dash-air-x2","combo-z1"},true,true);
            AddCalibration("charge-zxx","ZXX",new[]{"dash-z1","dash-zx","dash-zxx"},true,false,true);
            AddCalibration("charge-xxx","XXX",new[]{"dash-x1","dash-xx","dash-xxx"},true,false,true);
            foreach(var skill in game.Skills){string final=skill.id=="mega-slash"?"mega-fire":skill.id=="rolling-smash"?"rolling-land":skill.id=="windmill"?"windmill-end":skill.id=="roll"?"roll-end":skill.clip;calibrationCases.Add(new CalibrationCase{id="skill-"+skill.id,skill=skill.id,clips=new[]{final}});}
            calibrationCases.Add(new CalibrationCase{id="awakening-one-orb",awakeningOrbs=1,clips=new[]{"awakening"}});
            calibrationCases.Add(new CalibrationCase{id="awakening-three-orbs",awakeningOrbs=3,clips=new[]{"awakening"}});
            calibrationCases.Add(new CalibrationCase{id="skill-unlimited-blade-awakened",skill="unlimited-blade",awakeningOrbs=3,awakenedSkill=true,clips=new[]{"unlimited-blade-awakened"}});
            calibrationCases.Add(new CalibrationCase{id="skill-eighth-slot-mega-slash",skill="mega-slash",slot=GameModel.SkillSlotCount-1,clips=new[]{"mega-fire"}});
            string only=Array.Find(Environment.GetCommandLineArgs(),arg=>arg.StartsWith("--calibration-only="));
            if(only!=null){string filter=only.Substring("--calibration-only=".Length);calibrationCases.RemoveAll(item=>item.id!=filter);if(calibrationCases.Count==0)throw new InvalidOperationException("Unknown calibration case: "+filter);}
            InitAudio();calibrationOriginalMute=audioMuted;audioMuted=false; // Temporary test override; saved user preference stays intact.
            StartCalibrationCase();skillFilter="All";skillPage=0;SetMenu("loadout");
        }
        void AddCalibration(string id,string sequence,string[] clips,bool dash=false,bool air=false,bool charge=false){calibrationCases.Add(new CalibrationCase{id=id,sequence=sequence,clips=clips,dash=dash,air=air,charge=charge});}
        void StartCalibrationCase(){
            var c=calibrationCases[calibrationIndex];float distance=c.skill=="armor-break"||c.skill=="kick"?24:c.skill=="armageddon-blade"?110:32;
            var spawns=new List<EnemySpawn>{new EnemySpawn{kind="thief",x=180+distance,y=GameModel.GroundY,hp=50000,aggroRange=0}};
            if(c.id=="dash-air-zzz")spawns=new List<EnemySpawn>{new EnemySpawn{kind="thief",x=240,hp=50000,aggroRange=0},new EnemySpawn{kind="thief",x=300,hp=50000,aggroRange=0},new EnemySpawn{kind="thief",x=370,hp=50000,aggroRange=0}};
            else if(c.skill!=null&&c.skill!="counter-attack"&&c.skill!="armageddon-blade")foreach(float x in new[]{350f,510f})spawns.Add(new EnemySpawn{kind="thief",x=x,hp=50000,aggroRange=0});
            game.SetRooms(new[]{new RoomDefinition{id="calibration",title="LK / 2016 NA CALIBRATION",width=1440,spawnX=180,exitX=1380,enemies=spawns}});
            game.ConfigureAwakening(c.awakeningOrbs);game.player.invincible=0;foreach(var e in game.enemies)e.cooldown=999;
            if(c.skill=="counter-attack"){game.enemies[0].cooldown=0;game.enemies[0].aggroRange=100;}
            game.roomBanner=0;if(c.skill!=null)game.EquipSkill(c.slot,c.skill);
            calibrationTick=calibrationStep=calibrationSeenTick=calibrationChargeTick=calibrationCastTick=0;calibrationCaptureTick=-1;calibrationQueued=calibrationSkillPressed=calibrationFollowupPressed=false;calibrationBusyEnd=-1;calibrationAccumulator=0;calibrationHitsBeforeFollowup=0;calibrationAudioBefore=AudioPlayCount;
            calibrationExtraCaptures.Clear();calibrationResult=new CalibrationResult{id=c.id,slot=c.slot,maxY=GameModel.GroundY};calibrationResults.Add(calibrationResult);RebuildRoom();RememberSimulationState();
        }
        Input CalibrationInput(){
            var c=calibrationCases[calibrationIndex];var input=new Input{skill=-1};
            if(c.awakeningOrbs>0){if(calibrationTick==1)input.awaken=true;if(!c.awakenedSkill||!game.IsAwakened||game.AwakeningCasting)return input;}
            if(c.skill!=null){
                if(!calibrationSkillPressed){input.skill=c.slot;calibrationSkillPressed=true;calibrationCastTick=calibrationTick;}
                int age=calibrationTick-calibrationCastTick;
                if(c.skill=="mega-slash"&&age<110)input.skillHeldMask=1<<c.slot;
                if(c.skill=="windmill"&&age<210){input.skillHeldMask=1<<c.slot;input.move=age<105?1:-1;}
                if(age==110&&c.skill=="mega-slash"||age==210&&c.skill=="windmill")input.skillReleasedMask=1<<c.slot;
                if(c.skill=="armageddon-blade"&&game.ArmageddonActive&&!game.Busy()&&!calibrationFollowupPressed){
                    float dx=game.enemies[0].x-game.player.x;
                    if(Math.Abs(dx)>95)input.move=Math.Sign(dx);else if(Math.Abs(dx)<75)input.move=-Math.Sign(dx==0?1:dx);
                    else{input.move=Math.Sign(dx);input.attack=true;calibrationFollowupPressed=true;calibrationHitsBeforeFollowup=game.hits;}
                }
                return input;
            }
            input.dash=c.dash;
            if(c.air&&calibrationTick==1){input.move=c.dash?1:0;input.jump=true;return input;}
            if(c.id=="dash-air-zzz"&&calibrationTick<=8){input.move=1;return input;}
            if(c.id=="air-x"&&calibrationTick<19)return input;
            if(calibrationStep>=c.clips.Length)return input;
            if(game.player.clip==c.clips[calibrationStep]){if(calibrationSeenTick==0)calibrationSeenTick=calibrationTick;if(calibrationTick-calibrationSeenTick>=2){calibrationStep++;calibrationQueued=false;calibrationSeenTick=0;}}
            if(calibrationStep<c.clips.Length&&!calibrationQueued&&(!c.air||calibrationTick>2)){
                char button=c.sequence[calibrationStep];bool forward=c.id=="ground-forward"&&calibrationStep==3;float at=game.NextCommandInputTime(button,forward);
                if(!game.Busy()||game.player.clock>=at+1f/60){input.attack=button=='Z';input.attackX=button=='X';input.move=c.dash||forward?1:0;calibrationQueued=true;}
            }
            if(c.charge&&calibrationStep==1){input.attackXHeld=calibrationTick<90;if(calibrationTick==90)input.attackXReleased=true;}
            if(c.id=="dash-xxx"&&calibrationStep==1){if(game.IsCharging&&calibrationChargeTick==0)calibrationChargeTick=calibrationTick;input.attackXHeld=calibrationChargeTick==0||calibrationTick-calibrationChargeTick<12;input.attackXReleased=calibrationChargeTick>0&&calibrationTick-calibrationChargeTick==12;}
            return input;
        }
        void RenderCalibration(){RenderActors();RenderEffects();RenderHud();RenderCombatGeometry();RenderCombatPresentation();if(calibrationResult!=null)calibrationResult.sawScreenDim|=PresentationDimAlpha>.01f;TickAudio();SamplePerformance();}
        void UpdateCalibration(){
            if(calibrationDone)return;frame++;wallClock+=Time.unscaledDeltaTime;
            if(calibrationIntroFrames<14){calibrationIntroFrames++;RenderCalibration();RenderMenus();if(calibrationIntroFrames==8){for(int i=0;i<20;i++){UnityEngine.UI.Image icon;if(images.TryGetValue("Skill Card Icon "+i,out icon)&&icon.gameObject.activeInHierarchy&&icon.sprite)calibrationLoadedMenuIcons++;}RecordCalibrationSlotBindings();Capture("00-skills-20-icons");calibrationCaptureCount++;RecordCalibrationReference("00-skills-20-icons");}if(calibrationIntroFrames==14)SetMenu("");return;}
            if(calibrationPendingNext){calibrationPendingNext=false;calibrationIndex++;if(calibrationIndex>=calibrationCases.Count){FinishCalibration();return;}StartCalibrationCase();}
            calibrationAccumulator+=Mathf.Min(Time.unscaledDeltaTime,.1f);int steps=0;
            while(calibrationAccumulator>=1f/60&&steps++<6){calibrationTick++;RememberSimulationState();var input=CalibrationInput();game.Step(1f/60,input);calibrationAccumulator-=1f/60;RecordCalibrationFrame(input);}
            RenderCalibration();var c=calibrationCases[calibrationIndex];
            TextValue("Zone","CALIBRATION "+(calibrationIndex+1)+" / "+calibrationCases.Count);TextValue("Title",c.id.ToUpperInvariant());labels["Title"].fontSize=c.id.Length>18?17:23;
            TextValue("Mission",game.player.clip+" / "+game.hits+" HITS");TextValue("Keyboard Hint","AUTOMATED / source timing, 60 FPS poses, real geometry");TextValue("System Hint","Step "+calibrationTick+" / "+game.player.clock.ToString("0.000")+" s / "+AudioPlayCount+" SFX");
            bool final=game.player.clip==c.clips[c.clips.Length-1]||c.skill=="iron-body"&&game.armor>0;if(final)calibrationResult.reachedFinal=true;
            if(game.hits>0||c.skill=="iron-body"&&game.armor>0&&calibrationTick>15)CaptureCalibration("impact");
            if(game.enemies[0].impactFlash>0)CaptureCalibration("hit-flash");
            if(final&&game.player.clock>.23f)CaptureCalibration("last-step");
            if(game.SkillWorldStopRemaining>0&&PresentationDimAlpha>.01f)CaptureCalibration("cast-stop-dim");
            if(game.AwakeningCasting&&game.player.clock>.12f)CaptureCalibration("awakening-caster");
            if(game.IsAwakened&&!game.AwakeningCasting)CaptureCalibration("awakening-active");
            if(c.id=="dash-air-zzz"){
                if(game.player.clip=="dash-air-z3"&&game.player.clock>.05f)CaptureCalibration("spin-first");if(game.player.clip=="dash-air-z3"&&game.player.clock>.25f)CaptureCalibration("spin-second");if(game.player.clip=="dash-air-z-land"&&game.player.clock>.02f)CaptureCalibration("landing");
            }
            if(c.skill=="mega-slash"&&game.player.clip=="mega-charge"&&game.player.clock>.8f)CaptureCalibration("charge");
            if(c.skill=="counter-attack"&&game.CounterWaiting&&game.player.clock>.15f)CaptureCalibration("guard");
            if(c.skill=="counter-attack"&&game.CounterWaiting&&game.player.clock>.08f)CaptureCalibration("guard-start");
            if(c.skill=="armor-break"&&game.effects.Exists(e=>e.type=="armor-break"&&e.age>=.15f))CaptureCalibration("shards");
            if(c.skill=="impact-smash"&&game.effects.Exists(e=>e.type=="impact-smash"&&e.age>=.1f))CaptureCalibration("floor-impact");
            if(c.skill=="mortal-blow"&&game.effects.Exists(e=>e.type=="mortal-blow"&&e.phase==1&&e.age>=.35f))CaptureCalibration("burn-trail");
            if(c.skill=="roll"&&game.effects.Exists(e=>e.type=="roll"&&e.phase==1&&e.age>=.07f))CaptureCalibration("landing-dust");
            if(c.skill=="iron-body"&&game.PlayerHasSuperArmor&&calibrationTick>30)CaptureCalibration("red-outline");
            if(c.skill=="armageddon-blade"){
                if(game.ArmageddonActive&&game.player.clock>.9f)CaptureCalibration("long-sword");if(calibrationFollowupPressed&&game.player.clip=="combo-z1"&&game.player.clock>.09f)CaptureCalibration("buff-z");
                if(calibrationFollowupPressed&&game.hits>calibrationHitsBeforeFollowup){calibrationResult.armageddonFollowupHit=true;CaptureCalibration("buff-z-impact");}
            }
            if(c.skill=="double-slash")CaptureCalibrationAt("second-impact",2.3f);if(c.skill=="spiral-blast")CaptureCalibrationAt("blast",1.15f);if(c.skill=="windmill")CaptureCalibrationAt("moving-spin",1.3f);if(c.skill=="rolling-smash")CaptureCalibrationAt("vault",.43f);if(c.skill=="sonic-blade")CaptureCalibrationAt("shockwave",.8f);
            if(c.skill=="windmill"&&game.player.clip=="windmill-loop"){
                float sway=game.SkillPresentationRootX(RenderPlayerClock);
                if(sway>13)CaptureCalibration("sway-peak-right");if(sway< -13)CaptureCalibration("sway-peak-left");
            }
            if(c.skill=="gigantic-slash"){if(game.player.clock>2.95f)CaptureCalibration("phase-2");if(game.player.clock>4.85f)CaptureCalibration("phase-3");if(game.player.clock>6.42f)CaptureCalibration("finisher");}
            if(c.skill=="flame-geyser")CaptureCalibrationAt("column-peak",.92f);
            if(c.skill=="triple-geyser")CaptureCalibrationAt("three-columns",1.22f);
            if(c.skill=="double-slash"){
                if(game.player.clock>=.400f)CaptureCalibration("first-arc");
                if(game.player.clock>=.504f)CaptureCalibration("first-expanded");
                if(game.player.clock>=1.950f)CaptureCalibration("second-arc");
                if(game.player.clock>=2.054f)CaptureCalibration("second-expanded");
                if(game.player.clock>=2.254f)CaptureCalibration("fire-front");
                if(game.player.clock>=2.454f)CaptureCalibration("fire-sweep");
            }
            bool objective=calibrationResult.reachedFinal&&(c.awakeningOrbs==0||game.IsAwakened)&&(c.skill!="armageddon-blade"||calibrationResult.armageddonFollowupHit);
            if(objective&&!game.Busy()&&calibrationBusyEnd<0)calibrationBusyEnd=calibrationTick/60f;
            float minimum=c.awakenedSkill?7:c.skill=="gigantic-slash"?11:c.skill=="windmill"||c.skill=="double-slash"?6:c.skill!=null?3.8f:c.awakeningOrbs>0?2.2f:1.8f;
            bool finished=calibrationBusyEnd>=0&&calibrationTick/60f>Math.Max(minimum,calibrationBusyEnd+.8f);
            if(finished||calibrationTick>1800){calibrationResult.timedOut=!finished;calibrationResult.hits=game.hits;calibrationResult.seconds=calibrationTick/60f;calibrationResult.displacement=game.player.x-180;calibrationResult.audioEvents=AudioPlayCount-calibrationAudioBefore;calibrationResult.awakeningDuration=game.AwakeningDuration;calibrationResult.events.AddRange(game.combatEvents);File.WriteAllText(Path.Combine(captureDir,c.id+"-trace.json"),JsonUtility.ToJson(calibrationResult,true));calibrationPendingNext=true;}
        }
        void RecordCalibrationSlotBindings(){
            for(int i=0;i<GameModel.SkillSlotCount;i++){
                var s=game.GetEquippedSkill(i);var expected=SkillIcon(s.id);UnityEngine.UI.Image menuIcon,hudIcon;UnityEngine.UI.Text menuKey,hudKey;
                bool menuBound=images.TryGetValue("Equip Slot Icon "+i,out menuIcon)&&menuIcon.gameObject.activeInHierarchy&&expected&&menuIcon.sprite==expected;
                bool hudBound=images.TryGetValue("Skill Icon "+i,out hudIcon)&&hudIcon.gameObject.activeInHierarchy&&expected&&hudIcon.sprite==expected;
                calibrationSlotBindings.Add(new CalibrationSlotBinding{slot=i,skill=s.id,key=labels.TryGetValue("Equip Slot Key "+i,out menuKey)?menuKey.text:"",hudKey=labels.TryGetValue("Skill Key "+i,out hudKey)?hudKey.text:"",actualTexture=menuIcon&&menuIcon.sprite?menuIcon.sprite.texture.name:"",expectedTexture=expected?expected.texture.name:"",menuMatches=menuBound,hudMatches=hudBound,hudX=hudIcon?hudIcon.rectTransform.anchoredPosition.x:0,hudY=hudIcon?hudIcon.rectTransform.anchoredPosition.y:0,hudWidth=hudIcon?hudIcon.rectTransform.rect.width:0,hudHeight=hudIcon?hudIcon.rectTransform.rect.height:0});
            }
        }
        void RecordCalibrationFrame(Input input){
            var e=game.enemies[0];if(!calibrationResult.visited.Contains(game.player.clip))calibrationResult.visited.Add(game.player.clip);
            calibrationResult.maxY=Math.Max(calibrationResult.maxY,game.player.y);if(game.player.clip=="dash-air-z3")calibrationResult.spinMaxClock=Math.Max(calibrationResult.spinMaxClock,game.player.clock);
            calibrationResult.sawWorldStop|=game.SkillWorldStopRemaining>0;calibrationResult.sawSuperArmor|=game.PlayerHasSuperArmor;calibrationResult.sawAwakening|=game.IsAwakened;
            calibrationResult.frames.Add(new CalibrationFrame{tick=calibrationTick,inputSkill=input.skill,inputHeldMask=input.skillHeldMask,inputReleasedMask=input.skillReleasedMask,clip=game.player.clip,clock=game.player.clock,x=game.player.x,y=game.player.y,vy=game.player.vy,hits=game.hits,targetX=e.x,targetY=e.y,targetHp=e.hp,targetState=e.state.ToString(),phase=game.SkillPhaseLabel,sourceFrame=SourceFrameIndex(game.player.clip,game.player.clock),worldStop=game.SkillWorldStopRemaining,screenDim=game.CastScreenDim,superArmor=game.PlayerHasSuperArmor,invulnerable=game.PlayerIsInvulnerable,awakened=game.IsAwakened,awakeningRemaining=game.AwakeningRemaining,targetImpactFlash=e.impactFlash,targetStun=e.stun,presentationRootX=game.SkillPresentationRootX(game.player.clock)});
        }
        int SourceFrameIndex(string clip,float clock){Library lib;AnimationData data;if(!libraries.TryGetValue(catalog.hero.id,out lib)||!lib.clips.TryGetValue(clip,out data))return -1;if(data.loop&&data.duration>0)clock%=data.duration;return Mathf.Clamp(Mathf.FloorToInt(clock*data.fps),0,data.frames.Length-1);}
        void RecordCalibrationReference(string name){
            Library lib;AnimationData data;int index=SourceFrameIndex(game.player.clip,RenderPlayerClock);if(index<0||!libraries.TryGetValue(catalog.hero.id,out lib)||!lib.clips.TryGetValue(game.player.clip,out data))return;
            var expected=lib.frames[game.player.clip][index];var bounds=heroView.bounds;var position=RenderPlayerPosition;
            int visibleLayers=0;foreach(var renderer in castComponents)if(renderer.enabled&&renderer.gameObject.activeInHierarchy)visibleLayers++;
            var targetView=enemyViews[0];var targetProperties=new MaterialPropertyBlock();targetView.GetPropertyBlock(targetProperties);
            var bodyBottomLeft=worldCamera.WorldToScreenPoint(new Vector3(game.player.x-9.8f,game.player.y,0));var bodyTopRight=worldCamera.WorldToScreenPoint(new Vector3(game.player.x+9.8f,game.player.y+42,0));
            calibrationReferences.Add(new CalibrationReference{capture=name,clip=game.player.clip,sourceFrame=index,sourceFps=(int)data.fps,castVisibleLayers=visibleLayers,sourceClock=RenderPlayerClock,actualSprite=heroView.sprite?heroView.sprite.name:"",expectedSprite=expected.name,texture=data.frames[index].texture,actorX=game.player.x,actorY=game.player.y,renderX=position.x,renderY=position.y,physicalRenderX=RenderPlayerWorldPosition.x,presentationRootX=game.SkillPresentationRootX(RenderPlayerClock),spriteBottom=bounds.min.y,spriteTop=bounds.max.y,presentationDimAlpha=PresentationDimAlpha,matchesSourceFrame=heroView.sprite==expected,targetImpactFlash=game.enemies[0].impactFlash,targetShaderFlash=targetProperties.GetFloat("_HitFlash"),targetShader=targetView.sharedMaterial?targetView.sharedMaterial.shader.name:"",facing=game.player.facing,bodyScreenX=bodyBottomLeft.x,bodyScreenY=Screen.height-bodyTopRight.y,bodyScreenWidth=bodyTopRight.x-bodyBottomLeft.x,bodyScreenHeight=bodyTopRight.y-bodyBottomLeft.y});
        }
        bool CaptureCalibration(string phase){if(calibrationExtraCaptures.Contains(phase)||calibrationCaptureTick==calibrationTick)return false;string name=calibrationCases[calibrationIndex].id+"-"+phase;Capture(name);RecordCalibrationReference(name);calibrationExtraCaptures.Add(phase);calibrationCaptureCount++;calibrationCaptureTick=calibrationTick;return true;}
        void CaptureCalibrationAt(string name,float seconds){if(calibrationTick/60f>=seconds)CaptureCalibration(name);}
        void FinishCalibration(){
            File.WriteAllText(Path.Combine(captureDir,"calibration-report.json"),JsonUtility.ToJson(new CalibrationReport{captures=calibrationCaptureCount,skillCount=game.Skills.Count,skillSlotCount=GameModel.SkillSlotCount,commandCount=16,loadedMenuIcons=calibrationLoadedMenuIcons,slotBindings=calibrationSlotBindings,cases=calibrationResults,sourceReferences=calibrationReferences,audioReady=AudioManifestReady,audioLoaded=AudioLoadedClips,audioPlays=AudioPlayCount,audioPeakVoices=AudioPeakVoices,audioDropped=AudioDroppedVoices,audioWasMuted=calibrationOriginalMute},true));
            audioMuted=calibrationOriginalMute;calibrationDone=true;StartCoroutine(QuitAfterCapture());
        }
    }
}
