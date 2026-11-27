using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ElswordPixel {
    public sealed partial class DemoScene {
        string menuOrigin="";
        string openMenu="",skillFilter="All",selectedSkillId="mega-slash";
        int selectedSlot,skillPage;
        const int SkillPageSize=20;
        readonly List<SkillDefinition> visibleSkills=new List<SkillDefinition>();
        bool pausedBeforeMenu;
        float lastLeftTap=-10,lastRightTap=-10;
        int tappedDashDirection;
        void ConfigureRooms(){
            if(selectedDungeon<0||selectedDungeon>=catalog.dungeons.Length||
                !IsReleaseDungeon(selectedDungeon,catalog.dungeons[selectedDungeon].id))
                throw new InvalidOperationException("Dungeon is not available in this edition.");
            smokeNextAttack=0;
            var content=CurrentNpcContentSet;
            ConfigureLakeNightAdmissionNoticeV19(content);
            var npcRules=content==null?catalog.npcRules:content.npcRules;
            var effectRules=content==null?catalog.npcEffectRules:content.npcEffectRules;
            var mechanisms=content==null?catalog.npcMechanismRules:content.npcMechanismRules;
            ConfigureRubenAttacksV45(ref npcRules,ref effectRules,ref mechanisms);
            game.SetNpcDefinitions(npcRules);
            game.SetNpcEffectDefinitions(effectRules);
            game.SetNpcSoundDefinitions(content==null?catalog.npcSoundRules:content.npcSoundRules);
            game.SetNpcMechanismDefinitions(mechanisms);
            if(content!=null&&content.sourceDungeon==DragonNestSourceDungeonV20)ConfigurePackagedDragonNestVisualV20(content,content.dragonNestVisual);else if(content!=null&&content.sourceDungeon==LakeNightSourceDungeonV19)ConfigurePackagedLakeNightVisualV19(content);else if(content!=null&&content.sourceDungeon==LakeSourceDungeonV18)ConfigurePackagedLakeVisualV18(content);else if(content!=null&&content.sourceDungeon==DragonRoadSource)ConfigurePackagedDragonRoadVisual(content);else if(content!=null&&content.sourceDungeon==CastleCenterSource)ConfigurePackagedCastleCenterVisual(content);else ConfigurePackagedOutskirtsVisual(content==null?null:content.outskirtsVisual);
            var rooms=new List<RoomDefinition>();
            for(int i=Dungeon.start;i<Dungeon.start+Dungeon.count;i++){var art=catalog.stages[i];
                var room=new RoomDefinition{id=art.id,title=art.title,width=art.width,spawnX=art.spawnX,exitX=art.exitX,groundNodes=art.groundNodes,surfaces=calibrationRun?null:art.surfaces,hasSpawnY=art.hasSpawnY,spawnY=art.spawnY,entryEncounter=art.entryEncounter,encounters=art.encounters};
                if(art.spawns!=null)foreach(var spawn in art.spawns) {
                    if(!ChooseRegionalSpawn(spawn,out var kind,out var sourceNpc))continue;
                    var pos=spawn.positions!=null && spawn.positions.Length>0 ? spawn.positions[placementRandom.Next(spawn.positions.Length)] : null;
                    room.enemies.Add(new EnemySpawn{kind=kind,x=pos?.x??spawn.x,y=pos?.y??spawn.y,aggroRange=spawn.aggroRange,passive=spawn.passive,dormant=spawn.dormant,groundLine=pos?.groundLine??spawn.groundLine,facing=pos?.facing??spawn.facing,nativeDepth=pos?.nativeDepth??spawn.nativeDepth,visualScale=spawn.visualScale,aiEnabled=spawn.aiEnabled,spawnAfterDeath=spawn.spawnAfterDeath,spawnDelay=spawn.spawnDelay,hp=spawn.hp,sourceNPC=sourceNpc,sourceKeyCode=spawn.sourceKeyCode,encounterIndex=spawn.encounterIndex,countsForClear=spawn.countsForClear,isGate=spawn.isGate,siegeMode=spawn.siegeMode});
                }
                room.teleports=calibrationRun?null:art.teleports;
                room.bungeeLineIds=calibrationRun?null:art.bungeeLineIds;
                // 1-1 ends with Benders. Keep the historical source catalog intact,
                // but do not instantiate its obsolete post-Benders boss in gameplay.
                if(Dungeon.id=="tree"&&room.id=="tree-3")
                    room.enemies.RemoveAll(e=>e.kind=="white-giant"&&e.spawnAfterDeath=="benders");
                // Legacy route gates were passive decorations. The door leaf is
                // an attackable exit objective; its posts remain scenery.
                if(room.encounters==null||room.encounters.Length==0)
                    foreach(var birth in room.enemies)if(birth.kind.StartsWith("prop-",StringComparison.Ordinal)&&birth.kind.EndsWith("gate",StringComparison.Ordinal)) {
                        birth.isGate=true;birth.passive=false;birth.countsForClear=false;birth.aiEnabled=false;
                        birth.hp=40;
                    }
                rooms.Add(room);
            }
            game.SetRooms(rooms);
            game.ConfigureAlteraDeferredCannonGates(content==null?null:content.alteraDeferredCannonGates);
        }
        Sprite SkillIcon(string id){return Picture("Pixel/ui/skills/"+id);}
        Color SkillColor(SkillDefinition s){return C(s.category=="Hyper Active"?0xdc8dffu:s.category=="Special Active"?0xffba65u:0x77d5d9u);}
        SkillDefinition SelectedSkill(){foreach(var s in game.Skills)if(s.id==selectedSkillId)return s;return game.Skills[0];}
        Text WrappedLabel(string id,string text,Rect rect,int size,Color color,Transform parent,TextAnchor anchor=TextAnchor.UpperLeft){
            var label=Label(id,text,rect,size,color,anchor,parent);label.horizontalOverflow=HorizontalWrapMode.Wrap;label.verticalOverflow=VerticalWrapMode.Truncate;return label;
        }
        void BuildMenus(){
            var panel=Panel("Menu Root",new Rect(0,0,960,540),C(0x06111c,.97f));panel.raycastTarget=true;
            Panel("Menu Header",new Rect(14,456,932,74),C(0xc6dced),panel.transform);
            HeaderInk(Label("Menu Title","SKILL LOADOUT",new Rect(30,489,500,35),23,Color.white,TextAnchor.MiddleLeft,panel.transform));
            HeaderInk(Label("Menu Subtitle","",new Rect(30,463,516,22),13,Color.white,TextAnchor.MiddleLeft,panel.transform));
            var loadout=UIRect("Loadout Content",panel.transform,new Rect(0,0,960,450));
            string[] filters={"All","Base","Sword Knight","Lord Knight"};
            for(int i=0;i<4;i++)Button("Skill Filter "+i,filters[i]=="All"?"ALL SKILLS":filters[i].ToUpperInvariant(),new Rect(28+i*130,408,124,42),"filter:"+filters[i],12,loadout);
            for(int i=0;i<SkillPageSize;i++){
                float x=28+(i%5)*104,y=332-(i/5)*76;
                Button("Skill Choice "+i,"",new Rect(x,y,100,70),"choose:"+i,11,loadout);
                var card=loadout.Find("Skill Choice "+i);
                Panel("Skill Card Accent "+i,new Rect(0,67,100,3),C(0x527284),card);
                var icon=Panel("Skill Card Icon "+i,new Rect(30,28,39,39),Color.white,card);icon.preserveAspect=true;
                WrappedLabel("Skill Card Name "+i,"",new Rect(2,1,96,28),11,C(0xe4edf0),card,TextAnchor.MiddleCenter);
                Label("Skill Card Equipped "+i,"",new Rect(73,45,18,21),12,C(0xffdf8f),TextAnchor.MiddleCenter,card);
            }
            Panel("Skill Detail",new Rect(566,156,358,248),C(0x162c3a),loadout);
            Panel("Skill Detail Accent",new Rect(566,401,358,3),C(0xeab46c),loadout);
            var detailIcon=Panel("Skill Detail Icon",new Rect(582,312,78,78),Color.white,loadout);detailIcon.preserveAspect=true;
            var name=WrappedLabel("Skill Detail Name","",new Rect(674,350,232,43),21,C(0xffe3a3),loadout);name.resizeTextForBestFit=true;name.resizeTextMinSize=15;name.resizeTextMaxSize=21;
            Label("Skill Detail Job","",new Rect(674,321,232,23),11,C(0x91afb9),TextAnchor.MiddleLeft,loadout);
            Panel("Skill Detail Type Chip",new Rect(582,279,158,24),C(0x283e4b),loadout);
            Label("Skill Detail Type","",new Rect(587,279,148,24),11,C(0xffcb86),TextAnchor.MiddleLeft,loadout);
            Label("Skill Detail Cost","",new Rect(750,280,158,23),12,C(0x88daf3),TextAnchor.MiddleRight,loadout);
            WrappedLabel("Skill Detail Description","",new Rect(582,197,326,69),13,C(0xe0e9e7),loadout);
            WrappedLabel("Skill Detail Note","",new Rect(582,162,326,31),10,C(0x9bb7c2),loadout);
            Button("Equip Selected","EQUIP TO Q",new Rect(566,102,358,48),"equip-selected",15,loadout);
            Label("Loadout Info","EQUIPPED SKILLS",new Rect(36,76,888,20),10,C(0x99b9c1),TextAnchor.MiddleLeft,loadout);
            for(int i=0;i<GameModel.SkillSlotCount;i++){
                Button("Equip Slot "+i,"",new Rect(36+i*111,15,105,52),"slot:"+i,12,loadout);
                var slot=loadout.Find("Equip Slot "+i);var icon=Panel("Equip Slot Icon "+i,new Rect(5,10,32,32),Color.white,slot);icon.preserveAspect=true;
                Label("Equip Slot Key "+i,SkillKeyLabels[i],new Rect(43,29,57,20),14,C(0xffdf8f),TextAnchor.MiddleLeft,slot);
                WrappedLabel("Equip Slot Name "+i,"",new Rect(43,3,58,26),8,C(0xd8e7e8),slot,TextAnchor.MiddleLeft);
                Panel("Equip Slot Accent "+i,new Rect(0,49,105,3),C(0x3c6573),slot);
            }
            Button("Skill Prev","<",new Rect(566,414,24,27),"skill-prev",14,loadout);
            Label("Skill Page","",new Rect(596,414,298,27),10,C(0xa8c5cc),TextAnchor.MiddleCenter,loadout);
            Button("Skill Next",">",new Rect(900,414,24,27),"skill-next",14,loadout);
            Button("Menu Map","MAP",new Rect(553,469,100,48),"dungeons",12,panel.transform);
            Button("Menu Switch","COMMANDS",new Rect(661,469,130,48),"menu-switch",12,panel.transform);
            Button("Menu Close","BACK",new Rect(799,469,130,48),"menu-close",12,panel.transform);
            var guide=UIRect("Guide Content",panel.transform,new Rect(0,0,960,450));
            WrappedLabel("Guide Note","",new Rect(32,391,896,59),14,C(0xaddbd7),guide,TextAnchor.MiddleLeft);
            BuildCommandGroups(guide);
            panel.gameObject.SetActive(false);BuildDungeonMenu();
        }
        void SetMenu(string menu){
            CaptureDungeonBest();
            if(menu=="dungeons"&&(game.victory||game.player.hp<=0)){
                clearTimerScore=null;
                adventureStarted=false;
            }
            paintedPausedMenu=null;
            if(menu=="dungeons" && regionalSession)EndRegionalDungeon();
            if(menu=="dungeons"){ResetMonsterStream();CancelMobileRoomWarmup();DisposeBendersViewsV36();}
            if(menu=="dungeons"&&batBLoadedV41)DisposeBatBPaintV41();
            if(menu=="dungeons" && (giantWilliamBanksV39.Count!=0||giantWilliamBlockedV39.Count!=0))DisposeGiantWilliamV39();
            if(menu=="dungeons" && (mageBanksV38.Count!=0||mageBlockedV38.Count!=0))DisposeMagicianSkinsV38();
            if(menu=="dungeons"&&ancientSkinV37!=null)DisposeAncientViewsV35();
   if(menu=="dungeons"&&battleProductSessionR1325){EndBattleProductR1325();return;}
   if(menu=="dungeons"&&cargoProductSessionR1288){EndCargoProductR1288();return;}
   if(menu=="dungeons"&&plainProductSessionR1320){EndPlainProductR1320();return;}if(menu=="dungeons"&&mine2ProductSessionR1233){EndMine2ProductR1233();return;}
   if(menu=="dungeons"&&factoryProductSessionR1179){EndFactoryProductR1179();return;}
   if(menu=="dungeons"&&coreProductSessionR1147){EndCoreHardProductR1147();return;}
            if(menu=="dungeons"&&belowPlayerSessionV21){EndBelowPlayerCandidateV21();return;}
#if UNITY_EDITOR
            if(menu=="dungeons"&&(forestPreviewActive||forestPreviewLoading)){EndForestPreview();return;}
#endif
            if((menu=="loadout"||menu=="guide")&&(openMenu=="dungeons"||string.IsNullOrEmpty(openMenu)))menuOrigin=openMenu;
            ClearInput();if(string.IsNullOrEmpty(openMenu)&&!string.IsNullOrEmpty(menu))pausedBeforeMenu=game.paused;
            openMenu=menu;game.paused=string.IsNullOrEmpty(menu)?pausedBeforeMenu:true;RenderMenus();
        }
        void EquipSelectedSkill(){game.EquipSkill(selectedSlot,SelectedSkill().id);SaveLoadout();RenderMenus();}
        bool MenuAction(string action){
            if(DungeonMapAction(action))return true;
            if(action=="dungeons"){SelectRuntimeDungeonOnMap(selectedDungeon);SetMenu("dungeons");return true;}
            if(action.StartsWith("dungeon-preview:")){int index;if(int.TryParse(action.Substring(16),out index)&&index>=0&&index<ReleaseDungeonIds.Length)SelectRuntimeDungeonOnMap(index);RenderMenus();return true;}
            if(action=="dungeon-enter"){EnterSelectedDungeon();return true;}
            if(action=="loadout"||action=="guide"){SetMenu(openMenu==action?(menuOrigin=="dungeons"||!adventureStarted?"dungeons":""):action);return true;}
            if(action=="menu-close"){SetMenu(menuOrigin=="dungeons"||!adventureStarted?"dungeons":"");return true;}
            if(action=="menu-switch"){SetMenu(openMenu=="loadout"?"guide":"loadout");return true;}
            if(action.StartsWith("slot:")){selectedSlot=Mathf.Clamp(int.Parse(action.Substring(5)),0,GameModel.SkillSlotCount-1);RenderMenus();return true;}
            if(action.StartsWith("filter:")){skillFilter=action.Substring(7);skillPage=0;foreach(var s in game.Skills)if(skillFilter=="All"||s.job==skillFilter){selectedSkillId=s.id;break;}RenderMenus();return true;}
            if(action.StartsWith("select-id:")){selectedSkillId=action.Substring(10);RenderMenus();return true;}
            if(action=="equip-selected"){EquipSelectedSkill();return true;}
            if(action=="skill-prev"||action=="skill-next"){
                int pages=Math.Max(1,(visibleSkills.Count+SkillPageSize-1)/SkillPageSize);skillPage=(skillPage+(action=="skill-next"?1:pages-1))%pages;RenderMenus();return true;
            }
            if(action.StartsWith("choose:")){
                int index=skillPage*SkillPageSize+int.Parse(action.Substring(7));if(index<visibleSkills.Count)selectedSkillId=visibleSkills[index].id;RenderMenus();return true;
            }
            return false;
        }
        void SaveLoadout(){
            if(factoryProductSessionR1179)return;
#if UNITY_EDITOR
            if(forestPreviewActive||forestPreviewLoading)return;
#endif
            if(runAutomatedPlaythrough||calibrationRun)return;for(int i=0;i<GameModel.SkillSlotCount;i++)PlayerPrefs.SetString("lk.skill."+i,game.GetEquippedSkill(i).id);PlayerPrefs.Save();}
        void LoadLoadout(){for(int i=0;i<GameModel.SkillSlotCount;i++){string id=PlayerPrefs.GetString("lk.skill."+i,"");if(!string.IsNullOrEmpty(id))game.EquipSkill(i,id);}}
        void RenderMenus(){
            if(!string.IsNullOrEmpty(openMenu))RenderResultSheet(false);
            RenderDungeonMenu();if(!images.TryGetValue("Menu Root",out var root))return;
            if(openMenu=="dungeons"){root.gameObject.SetActive(false);return;}
            root.gameObject.SetActive(!string.IsNullOrEmpty(openMenu));if(string.IsNullOrEmpty(openMenu))return;
            bool loadout=openMenu=="loadout";root.transform.Find("Loadout Content").gameObject.SetActive(loadout);root.transform.Find("Guide Content").gameObject.SetActive(!loadout);
            TextValue("Menu Title",loadout?"SKILL LOADOUT":"COMMANDS");
            TextValue("Menu Subtitle",loadout?"Choose a skill and a slot, then equip.":">> DASH     ^ JUMP     Z LIGHT     X HEAVY");
            if(factoryProductSessionR1179&&loadout)TextValue("Menu Subtitle","Choose skills for this run.");
            TextValue("Menu Map Text",KeyboardMode?"MAP [TAB]":"MAP");
            TextValue("Menu Switch Text",loadout?(KeyboardMode?"COMMANDS [F1]":"COMMANDS"):(KeyboardMode?"SKILLS [K]":"SKILLS"));
            TextValue("Menu Close Text",menuOrigin=="dungeons"||!adventureStarted?"BACK TO MAP":"CONTINUE");
            Visible("Menu Map",menuOrigin!="dungeons"&&adventureStarted);
            TextValue("Guide Note",KeyboardMode?"Follow the green LINK cue. >>^ZZZ spins until landing. Release direction to stop after landing.\n>>XXX: briefly hold the second X. ^X: strike on descent. CTRL: awaken. F3: collision bounds.":"Drag the joystick to its outer ring to sprint. Z is light attack, X is heavy attack and UP is jump.\nFollow the green LINK cue. For >>XXX, briefly hold the second heavy attack.");
            visibleSkills.Clear();foreach(var s in game.Skills)if(skillFilter=="All"||s.job==skillFilter)visibleSkills.Add(s);
            int pages=Math.Max(1,(visibleSkills.Count+SkillPageSize-1)/SkillPageSize);skillPage=Math.Min(skillPage,pages-1);
            Visible("Skill Prev",loadout&&pages>1);Visible("Skill Next",loadout&&pages>1);TextValue("Skill Page",visibleSkills.Count+" SKILLS"+(pages>1?"  /  "+(skillPage+1)+" OF "+pages:""));
            string[] keys=KeyboardMode?SkillKeyLabels:TouchSlotLabels,filters={"All","Base","Sword Knight","Lord Knight"};
            for(int i=0;i<GameModel.SkillSlotCount;i++){
                TextValue("Equip Slot Key "+i,keys[i]);
                if(i<filters.Length)images["Skill Filter "+i].color=C(skillFilter==filters[i]?0x35646cu:0x152d3cu);
                var s=game.GetEquippedSkill(i);TextValue("Equip Slot Name "+i,s.name.ToUpperInvariant());images["Equip Slot Icon "+i].sprite=SkillIcon(s.id);
                images["Equip Slot "+i].color=C(i==selectedSlot?0x34505au:0x142b3au);images["Equip Slot Accent "+i].color=i==selectedSlot?C(0xffd07d):C(0x34596b);
            }
            for(int i=0;i<SkillPageSize;i++){
                int index=skillPage*SkillPageSize+i;Visible("Skill Choice "+i,index<visibleSkills.Count);if(index>=visibleSkills.Count)continue;
                var s=visibleSkills[index];string equipped="";for(int slot=0;slot<GameModel.SkillSlotCount;slot++)if(game.GetEquippedSkill(slot).id==s.id)equipped=keys[slot];
                bool chosen=s.id==selectedSkillId;images["Skill Choice "+i].color=C(chosen?0x47616au:equipped.Length>0?0x233e4bu:0x122735u);
                images["Skill Card Accent "+i].color=chosen?C(0xffe1a2):SkillColor(s);images["Skill Card Icon "+i].sprite=SkillIcon(s.id);
                TextValue("Skill Card Name "+i,s.name.ToUpperInvariant());TextValue("Skill Card Equipped "+i,equipped);
            }
            var selected=SelectedSkill();images["Skill Detail Icon"].sprite=SkillIcon(selected.id);images["Skill Detail Accent"].color=SkillColor(selected);
            TextValue("Skill Detail Name",selected.name.ToUpperInvariant());TextValue("Skill Detail Job",selected.job.ToUpperInvariant());
            TextValue("Skill Detail Type",selected.category.ToUpperInvariant());labels["Skill Detail Type"].color=SkillColor(selected);
            TextValue("Skill Detail Cost",selected.mpCost+" MP  /  "+selected.cooldown+"s CD");TextValue("Skill Detail Description",selected.description);
            TextValue("Skill Detail Note",selected.category=="Hyper Active"?"Consumes 1 El Essence. Training stock: "+game.HyperEssence:selected.id=="mega-slash"?"Hold the skill to charge, then release to strike.":selected.id=="windmill"?"Move / jump during the spin. Hold for extra duration.":selected.id=="unlimited-blade"?"Awakening changes its final attack sequence.":"Select a slot below to assign this skill.");
            TextValue("Equip Selected Text","EQUIP TO "+keys[selectedSlot]+"   >");TextValue("Loadout Info","EQUIPPED SKILLS  /  SELECT SLOT "+keys[selectedSlot]);
        }
        sealed class HudSlot {
            public Image icon,shade;
            public Text timer;
            public string skill;
            public int seconds=-1;
            public bool insufficientMana;
        }
        readonly HudSlot[] hudSlots=new HudSlot[GameModel.SkillSlotCount];
        void BindHudSlots(){
            for(int i=0;i<hudSlots.Length;i++){
                hudSlots[i]=new HudSlot{icon=images["Skill Icon "+i],shade=images["Cooldown Shade "+i],timer=labels["Cooldown "+i]};
                TextValue("Skill Name "+i,"");TextValue("Skill Glyph "+i,"");
            }
        }
        void RenderSkillSlots(){
            for(int i=0;i<hudSlots.Length;i++){
                var view=hudSlots[i];
                var skill=game.GetEquippedSkill(i);string id=skill.id;
                bool changed=view.skill!=id;
                if(changed){
                    view.skill=id;view.icon.sprite=SkillIcon(id);view.icon.color=Color.white;
                    if(wheelIcons[i])wheelIcons[i].sprite=view.icon.sprite;
                }
                int seconds=Mathf.CeilToInt(Mathf.Max(0,game.cooldowns[i]));
                bool insufficientMana=game.mana<skill.mpCost;
                if(!changed&&view.seconds==seconds&&view.insufficientMana==insufficientMana)continue;
                view.seconds=seconds;view.insufficientMana=insufficientMana;
                bool active=seconds>0||insufficientMana;
                string value=seconds>0?seconds.ToString():"";
                view.icon.color=insufficientMana?C(0x777777):Color.white;
                view.shade.color=insufficientMana?C(0x22252a,.74f):C(0x11212d,.78f);
                view.shade.gameObject.SetActive(active);view.timer.text=value;
                if(wheelCooldown[i]){
                    wheelIcons[i].color=insufficientMana?C(0x777777,.88f):C(0xffffff,.88f);
                    wheelCooldown[i].color=insufficientMana?C(0x22252a,.74f):C(0x091126,.76f);
                    wheelCooldown[i].enabled=active;wheelTimers[i].text=value;
                }
            }
        }
    }
}

