using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UInput=UnityEngine.Input;

namespace ElswordPixel {
    /// <summary>Editable Unity 2D scene, uGUI HUD, and native desktop/mobile input.</summary>
    public sealed partial class DemoScene : MonoBehaviour {
        [Header("Scene objects (created by Tools / Elsword / Rebuild Demo Scene)")]
        [SerializeField] Camera worldCamera;
        [SerializeField] Transform worldRoot, actorsRoot, stagesRoot, effectsRoot;
        [SerializeField] SpriteRenderer heroView;
        [SerializeField] Canvas hudCanvas;
        [SerializeField] RectTransform safeRoot;
        [Header("Display")]
        public bool showTouchControls;
        public Color clearColor=new Color(.045f,.09f,.12f,1);
        public bool runAutomatedPlaythrough;
        GameModel game=new GameModel();
        readonly Dictionary<string,Text> labels=new Dictionary<string,Text>();
        readonly Dictionary<string,Image> images=new Dictionary<string,Image>();
        readonly Dictionary<string,Library> libraries=new Dictionary<string,Library>();
        readonly Dictionary<string,Sprite> staticSprites=new Dictionary<string,Sprite>();
        readonly Dictionary<string,Texture2D> atlasTextures=new Dictionary<string,Texture2D>();
        readonly Dictionary<Sprite,Vector2> spriteOffsets=new Dictionary<Sprite,Vector2>();
        readonly Dictionary<int,string> touches=new Dictionary<int,string>();
        readonly List<SpriteRenderer> enemyViews=new List<SpriteRenderer>();
        readonly List<SpriteRenderer> enemyBars=new List<SpriteRenderer>();
        readonly List<SpriteRenderer> warnings=new List<SpriteRenderer>();
        readonly List<TextMesh> enemyDebuffViews=new List<TextMesh>();
        readonly List<EffectView> fxViews=new List<EffectView>();
        int visibleEffectViewCount;
        ArtCatalog catalog;
        StageArt CurrentStageArt {get{
            if(BattleProductSessionActiveR1325)return battleProductStagesR1325[game.room];
            if(CargoProductSessionActiveR1288)return cargoProductStagesR1288[game.room];
            if(PlainProductSessionActiveR1320)return plainProductStagesR1320[game.room];if(Mine2ProductSessionActiveR1233)return mine2ProductStagesR1233[game.room];
            if(FactoryProductSessionActiveR1179)return factoryProductStagesR1179[game.room];
            if(coreProductSessionR1147&&forestPreviewActive)return forestPreviewStages[game.room];
#if UNITY_EDITOR
            if(forestPreviewActive)return forestPreviewStages[game.room];
#endif
            return catalog.stages[StageIndex];
        }}
        Font font;
        Sprite white,hit,dust,slash,ring;
        SpriteRenderer exitMarker;
        Input pending=new Input{skill=-1};
        readonly Queue<KeyCode> verticalPresses=new Queue<KeyCode>();
        float accumulator,wallClock;
        int shownRoom=-1,frame;
        bool adventureStarted;
        bool smokeBoss,smokeEnd,smokeEntry,smokeCombat;
        string captureDir;
        Vector2 lastSize,lastCanvasSize;
        float lastCanvasScale;
        Rect lastSafeArea;
        class Library { public SpriteLibraryData data;public AnimationFrames frames=new AnimationFrames();public Dictionary<string,AnimationData> clips=new Dictionary<string,AnimationData>(); }
        class EffectView { public GameObject root;public SpriteRenderer sprite;public CombatNumberView number;public List<SpriteRenderer> activeSkillParts; }
        [Serializable] class CaptureState {
            public bool bossIntroActive;public string bossIntroKind;public float bossIntroRemaining,gateTransitionRemaining;
            public float modelTime;public bool gateOpen;public int encounter;public CaptureEnemy[] remainingEnemies;
            public int recoveryDropsSpawned,recoveryDropsPickedUp,recoveryPickupSounds;public CaptureDrop[] recoveryItems;
            public string engine="Unity";public int room,kills,hits;public float hp;public bool victory;
            public float playerX,playerY,cameraX,cameraY,groundY,roomWidth;public bool grounded;public int terrainNodes;public string state,command,menu;public string[] loadout;
            public int alertedEnemies,downedEnemies;public bool allEnemiesPresent;
            public Vector2 canvasSize,hudBottomLeft,hudTopRight;public float canvasScale,hudLocalScale;
            public CameraFramingCaptureV25 cameraFraming;
        }
        [Serializable] class CaptureEnemy {public string npc,kind,state;public float x,y,hp;public int groundLine;public bool dormant,passive;}
        [Serializable] class CaptureDrop {public string resource,sourceNPC;public float x,y,amount;public bool grounded;}

        void Awake(){
            if(startupStarted)return;
            startupStarted=true;
            if(Application.platform==RuntimePlatform.Android){
                UInput.backButtonLeavesApp=false;
                StartCoroutine(StartMobileScene());
                return;
            }
            InitializeScene();
        }
        void InitializeScene(){
            LoadArt();
            if(!worldRoot||!hudCanvas)BuildForEditor();
            RestoreWindowSkin();
            PrepareMenuLayout();
            BuildBattleControls();BindViews();RestoreSceneArt();
            InitializePlayerSettings();
            Application.targetFrameRate=Application.isMobilePlatform?60:120;
            QualitySettings.vSyncCount=1;Screen.sleepTimeout=SleepTimeout.NeverSleep;
            SelectStartupDungeon();
            ConfigureRooms();game.ConfigureAwakening(1);LoadLoadout();ApplyTimings();
            if(Application.isMobilePlatform){stagesRoot.gameObject.SetActive(false);waitingForFirstMobileRoom=true;}
            else RebuildRoom();
            InitAudio();
            OpenStartupDungeonMap();
        }
        void LoadArt(){
            if(catalog!=null&&white&&catalog.hero!=null&&libraries.ContainsKey(catalog.hero.id))return;
            libraries.Clear();staticSprites.Clear();
            var json=Resources.Load<TextAsset>("Pixel/runtime-catalog");
            if(!json)throw new InvalidOperationException("Missing Pixel/runtime-catalog. Run node tools/art/stage_unity_assets.mjs.");
            // Validate the release catalog before adding the approved sprite and rules delta.
            byte[] baseBytes=json.bytes;
            installedCatalogDigest=new CatalogDigest(baseBytes);
            string baseText=System.Text.Encoding.UTF8.GetString(baseBytes);
            catalog=JsonUtility.FromJson<ArtCatalog>(baseText);
            ValidateReleaseCatalog();
            catalog=ApplyCompletedRedrawCatalogV31(installedCatalogDigest,baseText,catalog);
            foreach(var route in catalog.dungeons)if(route.id=="tree")route.boss="BOSS / BENDERS";
            // Unity materializes absent inline serializable classes as empty objects.
            // An empty optional effect document has no runner; populated documents
            // still have to pass the complete admission checks below.
            if(catalog!=null&&catalog.npcEffectRules!=null){var optional=catalog.npcEffectRules;
                if(!optional.runtimeAdmitted&&!optional.sourceMechanismsComplete&&(optional.definitions==null||optional.definitions.Length==0)&&(optional.bindings==null||optional.bindings.Length==0))catalog.npcEffectRules=null;
            }
            if(catalog==null||catalog.hero==null||catalog.stages==null||catalog.stages.Length<1||catalog.monsters==null)
                throw new InvalidOperationException("Incomplete Pixel/runtime-catalog. Stage the dungeon and monster atlases before running.");
            ValidateForestIterationAdmission();
            if(catalog.npcRules!=null)foreach(var npc in catalog.npcRules){
                if(npc==null||(!npc.runtimeAdmitted||!npc.sourceMechanismsComplete)&&!IterationNpc(npc.id))throw new InvalidOperationException("An unadmitted or incomplete source NPC mechanism was staged into the playable catalog.");
                var art=Array.Find(catalog.monsters,m=>m.id==npc.kind);if(art==null)throw new InvalidOperationException("Source NPC has no approved sprite library: "+npc.id);
                foreach(var state in npc.states)if(!Array.Exists(art.clips,c=>c.id==state.clip))throw new InvalidOperationException("Source NPC state would fall back to idle: "+npc.id+"/"+state.clip);
            }
            if(catalog.npcEffectRules!=null){
                if((!catalog.npcEffectRules.runtimeAdmitted||!catalog.npcEffectRules.sourceMechanismsComplete)&&!forestIterationApproved)throw new InvalidOperationException("Unadmitted source NPC effect runner in playable catalog");
                foreach(var fx in catalog.npcEffectRules.definitions){if((!fx.runtimeAdmitted||!fx.sourceMechanismsComplete)&&!IterationEffect(fx.id))throw new InvalidOperationException("Incomplete source NPC effect "+fx.id);if(!string.IsNullOrEmpty(fx.visualId)&&(catalog.effects==null||!Array.Exists(catalog.effects,e=>e.id==fx.visualId&&Array.Exists(e.clips,c=>c.id=="play"))))throw new InvalidOperationException("Missing approved source NPC effect pixels "+fx.visualId);}
            }
            pixelFont=Resources.Load<Font>("Pixel/ui/Silkscreen-Regular");font=Resources.Load<Font>("Pixel/ui/tahoma");if(!font)font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            bool deferCatalogArt=StreamMobileArt;
            MakeLibrary(catalog.hero,deferCatalogArt);
            foreach(var m in catalog.monsters)MakeLibrary(m,deferCatalogArt);
            if(catalog.effects!=null)foreach(var effect in catalog.effects)MakeLibrary(effect,deferCatalogArt);
            RegisterGroundSkillInk(deferCatalogArt);
            white=MakeShape("white",(x,y)=>true,2,2);
            hit=MakeShape("hit",(x,y)=>Math.Abs(x-15)<2||Math.Abs(y-15)<2||Math.Abs(x-y)<1||Math.Abs(x+y-30)<1,31,31);
            dust=MakeShape("dust",(x,y)=>((x/3+y/3)%3==0)&&((x-16)*(x-16)/4+(y-8)*(y-8)<65),32,16);
            slash=MakeShape("slash",(x,y)=>{float dx=x-31,dy=y-31;float r=dx*dx+dy*dy;return x>20&&r>710&&r<920;},64,64);
            ring=MakeShape("ring",(x,y)=>{float dx=x-31,dy=y-31;float r=dx*dx+dy*dy;return r>800&&r<920;},64,64);
        }
        Sprite MakeShape(string name,Func<int,int,bool> include,int w,int h){
            var t=new Texture2D(w,h,TextureFormat.RGBA32,false){name=name,filterMode=FilterMode.Point};var pixels=new Color32[w*h];
            for(int y=0;y<h;y++)for(int x=0;x<w;x++)pixels[y*w+x]=include(x,y)?new Color32(255,255,255,255):new Color32(255,255,255,0);
            t.SetPixels32(pixels);t.Apply();return Sprite.Create(t,new Rect(0,0,w,h),new Vector2(.5f,.5f),1);
        }
        void MakeLibrary(SpriteLibraryData data,bool deferFrames=false){
            if(data==null)return;
            if(string.IsNullOrEmpty(data.id)||data.clips==null)throw new InvalidOperationException("Invalid sprite library in runtime-catalog.");
            if(libraries.ContainsKey(data.id))return;
            deferFrames|=DeferMonsterPixelCarrierV53(data.id);
            var lib=new Library{data=data};
            if(deferFrames)lib.frames.open=id=>new MobileClip(this,lib.frames,data,lib.clips[id]);
            else AtlasTexture(BluePoruInkV45.Resource(data.id,data.texture));
            var sourceClips=data.clips;
            if(data==catalog.hero && !Array.Exists(sourceClips,c=>c.id=="walk")){
                var asset=Resources.Load<TextAsset>("Pixel/character/walk/clip");
                if(!asset)throw new InvalidOperationException("Missing player walk frame definitions.");
                var walk=JsonUtility.FromJson<AnimationData>(asset.text);
                Array.Resize(ref sourceClips,sourceClips.Length+1);
                sourceClips[sourceClips.Length-1]=walk;
            }
            foreach(var clip in sourceClips){
                if(clip.frames==null||clip.frames.Length==0||clip.fps<=0)throw new InvalidOperationException("Empty or invalid animation "+data.id+"/"+clip.id);
                lib.clips[clip.id]=clip;
                lib.frames[clip.id]=deferFrames?null:BuildAnimationFrames(data,clip);
            }
            libraries[data.id]=lib;
            if(deferFrames)mobileCatalogFrames.Add(lib.frames);
        }
        static AnimationData SelectFrameDrawing(SpriteLibraryData data,AnimationData clip){
            var signed=OutskirtsPaintExposureV40.Paint(data,clip);
            return SourceMotionInkV45.For(data,signed??clip)??signed;
        }
        static string FrameFallback(SpriteLibraryData data,AnimationData picture){
            return BluePoruInkV45.Resource(data.id,string.IsNullOrEmpty(picture.texture)?data.texture:picture.texture);
        }
        Sprite[] BuildAnimationFrames(SpriteLibraryData data,AnimationData clip){
            var drawing=SelectFrameDrawing(data,clip);
            var fallback=FrameFallback(data,drawing??clip);
            if(!AtlasTexture(fallback))throw new InvalidOperationException("Missing atlas for "+data.id+"/"+clip.id);
            var frames=new Sprite[clip.frames.Length];
            try{
                for(int i=0;i<frames.Length;i++)frames[i]=CreateAnimationSprite(data,clip,drawing,fallback,i,null);
                return frames;
            }catch{ReleaseCatalogSprites(frames);throw;}
        }
        Sprite CreateAnimationSprite(SpriteLibraryData data,AnimationData clip,AnimationData drawing,string fallback,int index,MobileClip mobile){
            var f=OutskirtsPaintExposureV40.Frame(clip,drawing,index);
            string path=string.IsNullOrEmpty(f.texture)?fallback:BluePoruInkV45.Resource(data.id,f.texture);
            var page=AtlasTexture(path);
            if(!page||f.w<=0||f.h<=0||f.x<0||f.y<0||f.x+f.w>page.width||f.y+f.h>page.height)
                throw new InvalidOperationException("Atlas rect is outside imported texture: "+data.id+"/"+clip.id+"/"+index+" path="+path+" rect="+f.x+","+f.y+","+f.w+","+f.h+" page="+(page?page.width+"x"+page.height:"missing")+". Check texture lifetime, max size and NPOT import settings.");
            var sprite=Sprite.Create(page,new Rect(f.x,page.height-f.y-f.h,f.w,f.h),f.hasAnchor?new Vector2(.5f,.5f):new Vector2(data.anchorX/f.w,1-data.anchorY/f.h),1,0,SpriteMeshType.FullRect);
            if(mobile==null)TrackBelowPlayerSpriteV21(sprite);
            else{mobile.pages.Add(path);mobileCatalogPages.Add(path);}
            if(f.hasAnchor)spriteOffsets[sprite]=new Vector2(f.w*.5f-f.anchorX,f.anchorY-f.h*.5f);
            if(path.StartsWith("Pixel/motion-v45/",StringComparison.Ordinal))RegisterTightMotionBoundsV45(sprite);
            sprite.name=data.id+"/"+clip.id+"/"+index;
            return sprite;
        }
        Texture2D AtlasTexture(string path){if(!atlasTextures.TryGetValue(path,out var t)){if(TryPrivateAtlasV53(path,out t))return t;t=LoadCastleBunkerV41(path)??CombatMachineArtV40.LoadBase(path)??TakeMonsterStreamPage(path)??Resources.Load<Texture2D>(path);if(t)t.filterMode=FilterMode.Point;atlasTextures[path]=t;}return t;}
        Vector2 SpriteOffset(SpriteRenderer renderer){return renderer.sprite&&spriteOffsets.TryGetValue(renderer.sprite,out var offset)?new Vector2(offset.x*(renderer.flipX?-1:1),offset.y):Vector2.zero;}
        Sprite Picture(string path){
            return CachedPicture(staticSprites,path);
        }
        Sprite CachedPicture(Dictionary<string,Sprite> cache,string path){
            if(string.IsNullOrEmpty(path))return null;
            if(cache.TryGetValue(path,out var old))return old;
            var t=Resources.Load<Texture2D>(path);if(!t)return null;t.filterMode=FilterMode.Point;
            var s=Sprite.Create(t,new Rect(0,0,t.width,t.height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);cache[path]=s;return s;
        }
        SpriteRenderer WorldSprite(string name,Transform parent,Sprite sprite,int order,Vector3 position){
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.localPosition=position;var r=go.AddComponent<SpriteRenderer>();r.sprite=sprite;r.sortingOrder=order;return r;
        }
        static Color C(uint rgb,float a=1){return new Color(((rgb>>16)&255)/255f,((rgb>>8)&255)/255f,(rgb&255)/255f,a);}
        static Transform Group(string name,Transform parent){var go=new GameObject(name);go.transform.SetParent(parent,false);return go.transform;}
        static void RemoveSceneObject(GameObject target){
            if(!target)return;
            if(Application.isPlaying){target.SetActive(false);Destroy(target);}else DestroyImmediate(target);
        }
        // The editor builder saves these objects and references in a real .unity scene.
        public void BuildForEditor(){DisposeBatBPaintV41();
            DisposeTreeKnightA1ViewsV30();DisposeChickenA1ViewsV31();DisposeBatViewsV34();DisposeCannonViewsV35();DisposeAncientViewsV35();DisposeBendersViewsV36();DisposeReusablePaintV37();DisposeFairyAuthoredViewsV33();DisposeHealerBladerViewsV35();DisposeFactoryRavenViewsV35();DisposeMachineViewsV35();
            LoadArt();
            if(worldRoot)RemoveSceneObject(worldRoot.gameObject);if(hudCanvas)RemoveSceneObject(hudCanvas.gameObject);
            worldRoot=Group("World",transform);stagesRoot=Group("Dungeon Stages",worldRoot);actorsRoot=Group("Actors",worldRoot);effectsRoot=Group("Effects",worldRoot);
            var cameraObject=new GameObject("Pixel Camera");cameraObject.transform.SetParent(worldRoot,false);cameraObject.transform.localPosition=new Vector3(320,180,-100);
            worldCamera=cameraObject.AddComponent<Camera>();worldCamera.orthographic=true;worldCamera.orthographicSize=180;worldCamera.clearFlags=CameraClearFlags.SolidColor;worldCamera.backgroundColor=clearColor;
            for(int i=0;i<catalog.stages.Length;i++) CreateStageLayers(i,!StreamMobileArt);
            heroView=WorldSprite("Elsword - animated sprite",actorsRoot,libraries[catalog.hero.id].frames["idle"][0],20,new Vector3(60,54,0));
            var canvasObject=new GameObject("HUD - editable uGUI");canvasObject.transform.SetParent(transform,false);hudCanvas=canvasObject.AddComponent<Canvas>();hudCanvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=canvasObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(960,540);scaler.matchWidthOrHeight=.5f;canvasObject.AddComponent<GraphicRaycaster>();
            safeRoot=UIRect("Safe Area",hudCanvas.transform,new Rect(0,0,960,540));safeRoot.anchorMin=Vector2.zero;safeRoot.anchorMax=Vector2.one;safeRoot.offsetMin=safeRoot.offsetMax=Vector2.zero;
            BuildPixelHud();BuildCombatHud();BuildMenus();
            if(!FindFirstObjectByType<EventSystem>()){var e=new GameObject("Event System");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();}
            BindViews();
        }
        void CreateStageLayers(int index,bool loadPictures){
            var stage=catalog.stages[index];
            var room=Group("Stage "+(index+1)+" - "+stage.title,stagesRoot);
            var center=new Vector3(stage.width/2,stage.height/2,0);
            var bg=WorldSprite("Background",room,loadPictures?Picture(stage.background):null,0,center);
            FitStageSprite(bg,stage.width,stage.height);
            if(!string.IsNullOrEmpty(stage.foreground)){
                var fg=WorldSprite("Ground Foreground",room,loadPictures?Picture(stage.foreground):null,35,center);
                FitStageSprite(fg,stage.width,stage.height);
            }
            room.gameObject.SetActive(index==0);
        }
        void FitStageSprite(SpriteRenderer s,float width,float height=270){if(s.sprite){s.transform.localPosition=new Vector3(width/2,height/2,0);s.transform.localScale=new Vector3(width/s.sprite.rect.width,height/s.sprite.rect.height,1);}}
        RectTransform UIRect(string name,Transform parent,Rect rect){
            var go=new GameObject(name,typeof(RectTransform));var t=go.GetComponent<RectTransform>();t.SetParent(parent,false);t.anchorMin=t.anchorMax=Vector2.zero;t.pivot=Vector2.zero;t.anchoredPosition=rect.position;t.sizeDelta=rect.size;return t;
        }
        Image Panel(string name,Rect rect,Color color,Transform parent=null){var t=UIRect(name,parent?parent:safeRoot,rect);var im=t.gameObject.AddComponent<Image>();im.color=color;im.raycastTarget=false;if(!name.Contains("Icon")&&!name.Contains("Fill")&&!name.Contains("Track")&&!name.Contains("Shade")&&!name.Contains("Shine")&&!name.Contains("Portrait")&&!name.Contains("Accent")&&!name.Contains("Divider")&&!name.Contains("Gauge")&&!name.Contains("Orb")&&!name.Contains("Preview")&&!name.Contains("Region Art")&&rect.height>=22)PixelBorder(im);return im;}
        Text Label(string name,string text,Rect rect,int size,Color color,TextAnchor alignment=TextAnchor.MiddleCenter,Transform parent=null){
            var t=UIRect(name,parent?parent:safeRoot,rect);var l=t.gameObject.AddComponent<Text>();l.font=pixelFont&&size>=8&&size<=28&&text.Length<100?pixelFont:font;l.text=text;l.fontSize=size;l.color=color;l.alignment=alignment;l.raycastTarget=false;l.horizontalOverflow=HorizontalWrapMode.Overflow;l.verticalOverflow=VerticalWrapMode.Overflow;
            var shadow=t.gameObject.AddComponent<Shadow>();shadow.effectColor=C(0x000000,.7f);shadow.effectDistance=new Vector2(1,-1);return l;
        }
        void Button(string name,string text,Rect r,string action,int size=17,Transform parent=null){
            var im=Panel(name,r,C(0x304963,.94f),parent);im.raycastTarget=true;var touch=im.gameObject.AddComponent<TouchControl>();touch.owner=this;touch.action=action;
            touch.tapOnRelease=IsMenuButtonAction(action);
            im.gameObject.AddComponent<PixelButtonFeedback>();
            Label(name+" Text",text,new Rect(0,0,r.width,r.height),size,C(0xece6c6),TextAnchor.MiddleCenter,im.transform);
        }
        void BuildHud(){
            Panel("Dungeon Panel",new Rect(700,452,245,77),C(0x101f29,.87f));Panel("Dungeon Accent",new Rect(700,452,2,77),C(0xc9ad69));
            Label("Zone","RUBEN / STAGE 01",new Rect(708,502,230,25),12,C(0x82d8c2));Label("Title","TREE OF EL",new Rect(708,477,230,28),23,C(0xf6eabc));Label("Mission","DEFEAT ALL ENEMIES",new Rect(708,454,230,24),11,C(0xe0eae3));
            Button("Pause Button","STAGE 1 / "+catalog.stages.Length,new Rect(400,494,160,33),"pause",14);
            Panel("Progress Track",new Rect(402,484,156,3),C(0x28464e));Panel("Progress Fill",new Rect(402,484,1,3),C(0x9ef1cb));
            Label("Command Input","",new Rect(370,448,220,26),17,C(0xffe1a2));
            Label("Link Window","",new Rect(365,416,330,23),11,C(0x9bd8d0));
            Label("Skill Phase","",new Rect(365,394,330,22),11,C(0xffd991));
            Panel("Buff Panel",new Rect(87,419,260,24),C(0x153e3b,.9f));var buff=Panel("Buff Icon",new Rect(91,421,20,20),Color.white);buff.sprite=SkillIcon("iron-body");Label("Buff","",new Rect(115,420,228,22),11,C(0x82f1d1),TextAnchor.MiddleLeft);Panel("Buff Timer",new Rect(87,419,260,2),C(0x8cecc8));
            Label("Combat State","",new Rect(87,395,260,22),12,C(0xffe49b),TextAnchor.MiddleLeft);
            Panel("Charge Track",new Rect(410,441,140,4),C(0x293f4c));Panel("Charge Fill",new Rect(410,441,1,4),C(0xffc671));
            Label("Combo","",new Rect(754,355,174,42),25,C(0xffe37d),TextAnchor.MiddleRight);Label("Gate Prompt","",new Rect(245,150,470,30),18,C(0xffe18f));
            Label("Feita Boss Warning","",new Rect(180,390,600,34),22,C(0xffa449));
            Panel("Footer",new Rect(0,0,960,27),C(0x0b1922,.96f));Label("Keyboard Hint","ARROWS move / UP jump   DOUBLE-TAP dash   Z / X   QWER ASDC skills",new Rect(10,2,570,23),11,C(0xaec3c4),TextAnchor.MiddleLeft);Label("System Hint","K skills  F1 combos  F3 hitboxes  T potion  ESC pause",new Rect(575,2,375,23),11,C(0xaec3c4),TextAnchor.MiddleRight);
            Panel("Skill Bar",new Rect(400,28,160,76),C(0x0b1922,.9f));string[] keys=SkillKeyLabels;
            for(int i=0;i<GameModel.SkillSlotCount;i++){
                float x=426+(i%4)*36,y=i<4?72:36;Button("Skill "+i,"",new Rect(x-15,y,30,30),"skill"+i);
                var im=Panel("Skill Icon "+i,new Rect(x-13,y+2,26,26),Color.white);im.preserveAspect=true;
                Label("Skill Glyph "+i,"",new Rect(x-13,y+2,26,26),24,C(0x82e9e3));
                Label("Skill Key "+i,keys[i],new Rect(x-16,y+24,13,13),10,C(0xffdd95));Label("Skill Name "+i,"",new Rect(x-17,y-8,34,9),6,C(0xccdddc));
                Panel("Cooldown Shade "+i,new Rect(x-14,y+1,28,28),C(0x11212d,.75f));Label("Cooldown "+i,"",new Rect(x-14,y+1,28,28),14,Color.white);
            }
            Button("Touch Left","<",new Rect(15,42,60,60),"left",28);Button("Touch Right",">",new Rect(82,42,60,60),"right",28);Button("Touch Dash","DASH",new Rect(150,42,64,60),"dash",12);
            Button("Touch Jump","JUMP",new Rect(719,37,65,60),"jump",13);Button("Touch Attack","Z",new Rect(797,37,65,60),"attack",28);Button("Touch Attack X","X",new Rect(875,37,65,60),"attackX",28);Button("Touch Toggle","TOUCH",new Rect(900,407,55,32),"touch",10);
            Button("Loadout Button","SKILLS [K]",new Rect(655,108,105,26),"loadout",11);Button("Guide Button","COMBOS [F1]",new Rect(768,108,116,26),"guide",11);Button("Potion Button","T +",new Rect(895,108,45,26),"potion",12);
            Panel("Banner Panel",new Rect(260,338,440,65),C(0x122a2d,.75f));Label("Banner","THE EL TREE",new Rect(260,365,440,38),28,C(0xfff2c5));Label("Banner Sub","01 - 01 / RUBEN",new Rect(260,341,440,25),13,C(0xcfead5));
            Button("Result Panel","",new Rect(240,211,480,127),"result");Label("Result Title","",new Rect(245,270,470,45),28,C(0xffe9b1));Label("Result Detail","",new Rect(245,229,470,40),14,C(0xe6ecdc));
        }
        void BindViews(){labels.Clear();images.Clear();foreach(var t in hudCanvas.GetComponentsInChildren<Text>(true))labels[t.name]=t;foreach(var i in hudCanvas.GetComponentsInChildren<Image>(true))images[i.name]=i;foreach(var c in hudCanvas.GetComponentsInChildren<TouchControl>(true))c.owner=this;if(labels.TryGetValue("Title",out var title)){title.horizontalOverflow=HorizontalWrapMode.Wrap;title.verticalOverflow=VerticalWrapMode.Truncate;title.resizeTextForBestFit=true;title.resizeTextMinSize=9;title.resizeTextMaxSize=15;}BindHudSlots();RefreshAwakeningHudStyle();ConfigureCombatHud();lastHudTitle=null;lastHudHealth=lastHudMana=-1;}
        void RestoreSceneArt(){
            // An editor-built scene retains its editable hierarchy. Rebind all
            // catalog pictures so a saved preview sprite is never required for
            // runtime correctness (also supports attaching this script directly).
            for(int i=0;i<Math.Min(stagesRoot.childCount,catalog.stages.Length);i++){
                var room=stagesRoot.GetChild(i);var bg=room.Find("Background");
                if(bg){var renderer=bg.GetComponent<SpriteRenderer>();renderer.sprite=StreamMobileArt?null:Picture(catalog.stages[i].background);FitStageSprite(renderer,catalog.stages[i].width,catalog.stages[i].height);}
                var fg=room.Find("Ground Foreground");
                if(fg){var renderer=fg.GetComponent<SpriteRenderer>();renderer.sprite=StreamMobileArt?null:Picture(catalog.stages[i].foreground);FitStageSprite(renderer,catalog.stages[i].width,catalog.stages[i].height);}
            }
            if(images.TryGetValue("Portrait",out var portrait))portrait.sprite=Picture("Pixel/ui/portrait");
            if(images.TryGetValue("Buff Icon",out var buff))buff.sprite=SkillIcon("iron-body");
            for(int i=0;i<GameModel.SkillSlotCount;i++)if(images.TryGetValue("Skill Icon "+i,out var icon))icon.sprite=SkillIcon(game.GetEquippedSkill(i).id);
            foreach(var label in labels.Values)if(!label.font)label.font=font;
        }
        void ApplyTimings(){
            foreach(var c in libraries[catalog.hero.id].clips.Values){
                var t=new ClipTiming();t.duration=c.duration;t.travel=Math.Abs(c.rootDistance);t.fps=c.fps;t.loop=c.loop;t.command=c.command;foreach(var f in c.frames){t.rootCurve.Add(f.rootX);var geometry=f.attackGeometry;if(geometry!=null&&geometry.weaponAttackStart!=null&&geometry.weaponAttackEnd!=null)t.geometry.Add(new AttackGeometry{weaponBaseX=geometry.weaponBase[0],weaponBaseY=geometry.weaponBase[1],weaponStartX=geometry.weaponAttackStart[0],weaponStartY=geometry.weaponAttackStart[1],weaponEndX=geometry.weaponAttackEnd[0],weaponEndY=geometry.weaponAttackEnd[1],radius=3,rightFootStartX=geometry.rightFootStart[0],rightFootStartY=geometry.rightFootStart[1],rightFootEndX=geometry.rightFootEnd[0],rightFootEndY=geometry.rightFootEnd[1],leftFootStartX=geometry.leftFootStart[0],leftFootStartY=geometry.leftFootStart[1],leftFootEndX=geometry.leftFootEnd[0],leftFootEndY=geometry.leftFootEnd[1]});}
                if(c.hits!=null&&c.hits.Length>0){t.hitStart=c.hits[0].start;t.hitEnd=c.hits[c.hits.Length-1].end;foreach(var h in c.hits)t.hitWindows.Add(new DamageWindow {start=h.start,end=h.end,rawReaction=h.rawReaction,repeatInterval=h.repeatInterval,damageScale=h.damageScale,damageDefined=h.damageDefined,enabledAttackBoxes=h.enabledAttackBoxes});}else{t.hitStart=100;t.hitEnd=100;}
                game.SetClipTiming(c.id,t);
            }
        }
        void Pose(SpriteRenderer renderer,string id,string clip,float clock,Enemy cannonSource=null){
            if(!libraries.TryGetValue(id,out var lib))return;
            string requestedClip=clip;bool idleFallback=false;
            if(id!=catalog.hero.id&&!lib.frames.ContainsKey(clip)){if(clip=="launched"||clip=="fall")clip="launch";if(clip=="knockdown")clip="down";if(clip=="getup")clip="get-up";}
            if(!lib.frames.ContainsKey(clip)){idleFallback=true;clip="idle";if(!lib.frames.ContainsKey(clip))return;}
            var info=lib.clips[clip];if(info.loop&&info.duration>0)clock=Mathf.Repeat(clock,info.duration);
            int requestedFrame=Mathf.FloorToInt(clock*info.fps);if(renderer==heroView&&id==catalog.hero.id)requestedFrame=HeroPoseFrame(info,requestedFrame,RenderPlayerClock);
            renderer.sprite=lib.frames.At(clip,requestedFrame,out int index);RecordMonsterPixelAliasV53(cannonSource,renderer,clip,clock,info,requestedClip,idleFallback);TraceLegacyPaintSelectionV40(cannonSource,renderer,clip,clock,index);RecordMineMachinePoseV35(renderer,id,clip,clock);if(cannonSource!=null)RecordCannonPoseV35(cannonSource,renderer,clip,clock);
        }
        void RebuildRoom(){
            ReleaseMonsterPixelRoom(StreamMobileArt);
            game.EnableSimpleDangerIntroductions();
            DisposeAirshipSoldiersV40();
            DisposeBeastsV45();
            DisposeBatBPaintV41();DisposeRubenForestPaintV40();DisposeGiantWilliamV39();
            DisposeMagicianSkinsV38();
            DisposeTreeKnightA1ViewsV30();DisposeChickenA1ViewsV31();DisposeBatViewsV34();DisposeCannonViewsV35();DisposeAncientViewsV35();ChangeBendersRoomV36();DisposeReusablePaintV37();DisposeFairyAuthoredViewsV33();DisposeHealerBladerViewsV35();DisposeFactoryRavenViewsV35();DisposeMachineViewsV35();
            if(CargoProductSessionActiveR1288){SyncCargoProductRoomR1288();if(cargoProductSkinsR1288!=null)cargoProductSkinsR1288.ReleaseRoom();}
            #if !UNITY_EDITOR
            if(coreProductSessionR1147)ReleaseCoreHardEntryRoomV21();
#endif
#if UNITY_EDITOR
            ReleaseCoreHardEntryRoomV21();
            if(cargoEntrySkinsV21!=null)cargoEntrySkinsV21.ReleaseRoom();
#endif
            ClearRecoveryDropViews();
            AdvanceMobileArtRoom();
            stagesRoot.gameObject.SetActive(!PlainProductSessionActiveR1320&&!BattleProductSessionActiveR1325&&!FactoryProductSessionActiveR1179&&!Mine2ProductSessionActiveR1233&&!CargoProductSessionActiveR1288);
            if(StreamMobileArt)BindMobileStagePictures(stagesRoot.gameObject.activeSelf?StageIndex:-1);
            if(!PlainProductSessionActiveR1320&&!BattleProductSessionActiveR1325&&!FactoryProductSessionActiveR1179&&!Mine2ProductSessionActiveR1233&&!CargoProductSessionActiveR1288){
#if UNITY_EDITOR
            if(!forestPreviewActive){
#endif
            shownRoom=game.room;for(int i=0;i<stagesRoot.childCount;i++)stagesRoot.GetChild(i).gameObject.SetActive(i==StageIndex);
            if(regionalSession&&!StreamMobileArt){
                var art=CurrentStageArt;
                var bg=stagesRoot.GetChild(StageIndex).Find("Background").GetComponent<SpriteRenderer>();
                bg.sprite=Picture(art.background);FitStageSprite(bg,art.width,art.height);
            }
            if(!StreamMobileArt&&StageIndex<catalog.stages.Length&&StageIndex<stagesRoot.childCount){var art=catalog.stages[StageIndex];var fg=stagesRoot.GetChild(StageIndex).Find("Ground Foreground");if(fg){var view=fg.GetComponent<SpriteRenderer>();view.sprite=Picture(calibrationRun&&!string.IsNullOrEmpty(art.flatForeground)?art.flatForeground:art.foreground);FitStageSprite(view,art.width,art.height);}}
#if UNITY_EDITOR
            }else shownRoom=game.room;
#endif
            }else shownRoom=game.room;
            foreach(var e in enemyViews)Destroy(e.gameObject);foreach(var b in enemyBars)Destroy(b.gameObject);foreach(var w in warnings)Destroy(w.gameObject);foreach(var d in enemyDebuffViews)Destroy(d.gameObject);enemyViews.Clear();enemyBars.Clear();warnings.Clear();enemyDebuffViews.Clear();
            armorBreakIcons.Clear();
            EnsureEnemyViews();
            curtainRoom = null;
            RebuildNativeWorld();
            var content=CurrentNpcContentSet;ConfigurePackagedNpcSteamVisual(content==null?null:content.steamVisual,content==null?null:content.npcRules);
            BeginBossIntroduction();
        }
        public void TouchDown(int id,string action){MarkMobileTouchActivity();if(mobileLoadingWindowCount>0||!keyboardFocus.Active)return;ClearKeySelection();touches[id]=action;Action(action);}
        public void TouchUp(int id,string action=null){MarkMobileTouchActivity();if(action==null||(touches.TryGetValue(id,out var held)&&held==action)){touches.TryGetValue(id,out var released);touches.Remove(id);if(released==null||touches.ContainsValue(released))return;if(released=="attackX")pending.attackXReleased=true;if(released!=null&&released.StartsWith("skill")&&int.TryParse(released.Substring(5),out var slot))pending.skillReleasedMask|=1<<slot;}}
        void ClearInput(){keyboardFocus.ClearCombat();ClearKeySelection();touches.Clear();verticalPresses.Clear();pending=new Input{skill=-1};tappedDashDirection=0;ResetBattleGesture();game.ClearBufferedInput();}
        void Action(string action){
            if(string.IsNullOrEmpty(action))return;
            bool menuButton=IsMenuButtonAction(action);
            if(HandleSettingsAction(action)){if(menuButton)PlayMenuButtonSound();return;}
            if(BossIntroductionActive&&action!="pause"&&action!="sound"&&action!="touch")return;
            if(menuButton&&action!="sound")PlayMenuButtonSound();
            if(MenuAction(action))return;
            if(action=="sound"){ToggleAudio();PlayMenuButtonSound();return;}
            if(action=="pause"){if(!string.IsNullOrEmpty(openMenu)){if(openMenu=="dungeons")SetMenu(adventureStarted?"":"dungeons");else MenuAction("menu-close");return;}if(!game.victory&&game.player.hp>0)game.paused=!game.paused;ClearInput();return;}
            if(action=="result"&&(game.victory||game.player.hp<=0)){ReturnFromDungeonResult();return;}
            if(action=="result"){ClearInput();if(game.paused)game.paused=false;else if(game.victory||game.player.hp<=0)ResetGame();return;}
            if(game.paused||game.player.hp<=0||string.IsNullOrEmpty(action))return;
            if(game.victory&&action!="jump"&&action!="drop"&&action!="dash")return;
            if(action=="drop")verticalPresses.Enqueue(KeyCode.DownArrow);
            if(action=="jump")verticalPresses.Enqueue(KeyCode.UpArrow);
            if(action=="attack"){
                verticalPresses.Enqueue(KeyCode.Z);
            }if(action=="attackX"){
                verticalPresses.Enqueue(KeyCode.X);
            }if(action=="potion")pending.potion=true;
            if(action.StartsWith("potion")&&int.TryParse(action.Substring(6),out var item)&&item>=1&&item<=GameModel.PotionSlotCount)pending.potionPressedMask|=1<<(item-1);
            if(action=="dash")pending.dashPressed=true;
            if(action=="awaken")pending.awaken=true;
            if(action.StartsWith("skill")&&int.TryParse(action.Substring(5),out var skill)&&skill>=0&&skill<GameModel.SkillSlotCount)pending.skill=skill;
        }
        void ResetGame(){CancelBossIntroduction();smokeNextAttack=0;openMenu="";game.Reset();ApplyTimings();shownRoom=-1;previousRoom=-1;accumulator=0;ClearInput();RenderMenus();}
        void OnApplicationFocus(bool focus){if(AndroidKeyboard)keyboardFocus.SetFocus(focus);MarkMobileSurface(false,!focus);if(mobileStartupPending)return;if(!focus){ClearInput();SuspendSettings();
#if UNITY_EDITOR
            if(forestAutoArmed||forestAutoRunning)return;
#endif
#if ROOT_PRODUCT_SMOKE_R1374 && !UNITY_EDITOR
            if(MainBattleFullNaturalDriverR1374.OwnedFocusPolicyActiveR1404(this)){
                MainBattleFullNaturalDriverR1374.ObserveOwnedInputClearR1404(this,"OnApplicationFocus",false);return;
            }
#endif
#if ROOT_FACTORY_RAVEN_PLAYER_R1464 && !UNITY_EDITOR
            if(FactoryRavenPlayerCheckR1464.OwnedFocusPolicyActiveR1468(this)){
                FactoryRavenPlayerCheckR1464.ObserveOwnedInputClearR1468(this);return;
            }
#endif
#if ROOT_CARGO_BOSS_PLAYER_R1467 && !UNITY_EDITOR
            if(CargoBossPlayerCheckR1467.OwnedFocusPolicyActiveR1468(this)){
                CargoBossPlayerCheckR1467.ObserveOwnedInputClearR1468(this);return;
            }
#endif
#if ROOT_RECYCLE_NATURAL_PLAYER_R1471 && !UNITY_EDITOR
            if(RecycleNaturalPlayerCheckR1471.OwnedFocusPolicyActiveR1471(this)){
                RecycleNaturalPlayerCheckR1471.ObserveOwnedInputClearR1471(this);return;
            }
#endif
            if(!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&!musicReview)game.paused=true;}}
        void OnApplicationPause(bool paused){if(AndroidKeyboard)keyboardFocus.SetPaused(paused);MarkMobileSurface(true,paused);if(mobileStartupPending)return;if(paused){ClearInput();SuspendSettings();
#if UNITY_EDITOR
            if(forestAutoArmed||forestAutoRunning)return;
#endif
#if ROOT_PRODUCT_SMOKE_R1374 && !UNITY_EDITOR
            if(MainBattleFullNaturalDriverR1374.OwnedFocusPolicyActiveR1404(this)){
                MainBattleFullNaturalDriverR1374.ObserveOwnedInputClearR1404(this,"OnApplicationPause",true);return;
            }
#endif
#if ROOT_FACTORY_RAVEN_PLAYER_R1464 && !UNITY_EDITOR
            if(FactoryRavenPlayerCheckR1464.OwnedFocusPolicyActiveR1468(this)){
                FactoryRavenPlayerCheckR1464.ObserveOwnedInputClearR1468(this);return;
            }
#endif
#if ROOT_CARGO_BOSS_PLAYER_R1467 && !UNITY_EDITOR
            if(CargoBossPlayerCheckR1467.OwnedFocusPolicyActiveR1468(this)){
                CargoBossPlayerCheckR1467.ObserveOwnedInputClearR1468(this);return;
            }
#endif
#if ROOT_RECYCLE_NATURAL_PLAYER_R1471 && !UNITY_EDITOR
            if(RecycleNaturalPlayerCheckR1471.OwnedFocusPolicyActiveR1471(this)){
                RecycleNaturalPlayerCheckR1471.ObserveOwnedInputClearR1471(this);return;
            }
#endif
            if(!runAutomatedPlaythrough&&!calibrationRun&&!rubenReview&&!musicReview)game.paused=true;}}
        static readonly KeyCode[] SkillKeys={KeyCode.Q,KeyCode.W,KeyCode.E,KeyCode.R,KeyCode.A,KeyCode.S,KeyCode.D,KeyCode.C};
        static readonly string[] SkillKeyLabels={"Q","W","E","R","A","S","D","C"};
        void ReadVerticalPress(Input input){
            if(verticalPresses.Count==0)return;
            var key=verticalPresses.Dequeue();
            input.jump=key==KeyCode.UpArrow;
            input.drop=key==KeyCode.DownArrow;
            if(key==KeyCode.X)input.attackX=true;
            else if(key==KeyCode.Z)input.attack=true;
            else if(verticalPresses.Count>0){
                if(verticalPresses.Peek()==KeyCode.X){verticalPresses.Dequeue();input.attackX=true;}
                else if(verticalPresses.Peek()==KeyCode.Z){verticalPresses.Dequeue();input.attack=true;}
            }
        }
        Input ReadInput(){var i=pending;pending=new Input{skill=-1};
            if(!keyboardFocus.Active||settingsOpen||!string.IsNullOrEmpty(openMenu)||game.paused||game.player.hp<=0||BossIntroductionActive){verticalPresses.Clear();return new Input{skill=-1};}
            ReadVerticalPress(i);
            if(HardwareKeyboardEnabled){
                i.move=(CombatHeld(KeyCode.RightArrow)?1:0)-(CombatHeld(KeyCode.LeftArrow)?1:0);i.dash=CombatHeld(KeyCode.LeftShift)||CombatHeld(KeyCode.RightShift);
                i.attackXHeld=CombatHeld(KeyCode.X);i.attackHeld=CombatHeld(KeyCode.Z);
                for(int slot=0;slot<GameModel.SkillSlotCount;slot++)if(CombatHeld(SkillKeys[slot]))i.skillHeldMask|=1<<slot;
            }if(!KeyboardMode&&battleStick&&i.move==0)i.move=battleStick.Horizontal;
            bool left=false,right=false;foreach(var t in touches.Values){if(t=="left")left=true;if(t=="right")right=true;if(t=="dash")i.dash=true;if(t=="attack")i.attackHeld=true;if(t=="attackX")i.attackXHeld=true;if(t.StartsWith("skill")&&int.TryParse(t.Substring(5),out var slot))i.skillHeldMask|=1<<slot;}
            i.skillReleasedMask&=~i.skillHeldMask;if(i.attackXHeld)i.attackXReleased=false;
            i.move+=(right?1:0)-(left?1:0);i.move=Mathf.Clamp(i.move,-1,1);if(i.move==0)tappedDashDirection=0;if(Math.Sign(i.move)==tappedDashDirection&&tappedDashDirection!=0)i.dash=true;return i;
        }
        float smokeNextAttack;
        bool AutoNavigateCombat(Input input,Enemy target,float distance,ref float nextAttack){
            if(!game.NativeApproachEnemy(input,target))return false;
            // Keep the chosen jump/landing steering while allowing an ordinary
            // air attack against a nearby body blocking the upper ledge.
            bool takingLedge=regionalSession&&(input.jump||target.y>game.player.y+8);
            if(!takingLedge&&distance<100&&Math.Abs(target.y-game.player.y)<45&&game.elapsed>=nextAttack){input.attack=true;nextAttack=game.elapsed+.16f;}
            return true;
        }
        Input Autopilot(){if(!rubenReview&&SmokeFeatures(out var featureInput))return featureInput;return DungeonCombatInput();}
        Input DungeonCombatInput(){var i=new Input{skill=-1};if(game.gateOpen){float exit=game.HasEncounters?game.CurrentEncounter.exitX:game.ExitX;int direction=game.HasEncounters?game.CurrentEncounter.direction:1;i.move=Math.Abs(exit-game.player.x)>1?Math.Sign(exit-game.player.x):direction;i.dash=true;game.NativeNavigate(i,exit,game.GroundAt(game.CurrentRoom,exit));return i;}Enemy target=null;float best=10000;
            if(game.player.hp<330)i.potion=true;
            foreach(var e in game.enemies){if(!e.active||e.dormant||e.passive||e.hp<=0||game.HasEncounters&&e.encounterIndex!=game.EncounterIndex)continue;if(e.isGate&&(!game.RoomCombatClear||e.invincible>0))continue;if(!e.isGate&&!e.countsForClear)continue;float distance=Vector2.Distance(new Vector2(e.x,e.y),new Vector2(game.player.x,game.player.y));if(distance<best){target=e;best=distance;}}
            SelectCastleCenterPathObstruction(ref target,ref best);
            if(target!=null){if(AutoNavigateCombat(i,target,best,ref smokeNextAttack))return i;float dx=target.x-game.player.x;i.move=Math.Abs(dx)>34?Math.Sign(dx):0;i.dash=best>200;if(dx*game.player.facing<0)i.move=Math.Sign(dx);
                // Button presses use the simulation clock, independent of the
                // renderer's refresh rate and whether this Update owns a tick.
                if(best<100&&Math.Abs(target.y-game.player.y)<45&&game.elapsed>=smokeNextAttack){i.attack=true;smokeNextAttack=game.elapsed+.16f;}
                if(game.player.hp<330)i.potion=true;
                if(best<110&&game.CanAwaken){i.awaken=true;i.attack=false;return i;}
                if(best<110&&!game.Busy()){for(int slot=0;slot<GameModel.SkillSlotCount;slot++){var s=game.GetEquippedSkill(slot);if(game.cooldowns[slot]>0||game.mana<s.mpCost||game.HyperEssence<s.costItem)continue;if(s.id=="counter-attack"||s.id=="roll")continue;if(s.id=="iron-body"&&(game.armor>0||game.player.hp>340))continue;if((s.id=="armor-break"||s.id=="kick")&&Math.Abs(dx)>28)continue;i.skill=slot;i.attack=false;break;}}}
            return i;
        }
        string paintedPausedMenu;
        bool waitingForFirstMobileRoom;
        CombatFrameProbe combatFrameProbe;

        void DrawCombatFrame(){
            RenderActors();combatFrameProbe?.Mark(CombatFrameProbe.Phase.Actors);
            RenderEffects();combatFrameProbe?.Mark(CombatFrameProbe.Phase.Effects);
            RenderHud();combatFrameProbe?.Mark(CombatFrameProbe.Phase.Hud);
            RenderCombatGeometry();combatFrameProbe?.Mark(CombatFrameProbe.Phase.AttackGeometry);
            RenderCombatPresentation();combatFrameProbe?.Mark(CombatFrameProbe.Phase.Presentation);
            FinishNativeProjection();RenderWaterwayBossLaser();RenderCastleBossBeam();RenderCargoProductDirectorR1288();
            combatFrameProbe?.Mark(CombatFrameProbe.Phase.Projection);
        }
        void Update(){
            if(Application.isMobilePlatform){if(mobileSurfacePaused||mobileSurfaceUnfocused)return;RestoreMobileSurface();}
            if(AndroidKeyboard)keyboardFocus.BeginFrame(SceneKeys,PhysicalHeld,PhysicalDown);
            if(mobileLoadingWindowCount>0){HandleLoadingKeyboard();return;}
            try{
            if(TryReturnAfterClear()){TickAudio();return;}
            if(settingsOpen){Keyboard();TickAudio();return;}
            if(PlainProductSessionLoadingR1320){if(CancelLoadingWithKeyboard())EndPlainProductR1320();return;}if(BattleProductSessionLoadingR1325){if(CancelLoadingWithKeyboard())EndBattleProductR1325();return;}if(CargoProductSessionLoadingR1288){if(CancelLoadingWithKeyboard())EndCargoProductR1288();return;}
            if(Mine2ProductSessionLoadingR1233){if(CancelLoadingWithKeyboard())EndMine2ProductR1233();return;}
            if(FactoryProductSessionLoadingR1179){if(CancelLoadingWithKeyboard())EndFactoryProductR1179();return;}
            if(coreProductSessionR1147&&forestPreviewLoading){if(CancelLoadingWithKeyboard())EndCoreHardProductR1147();return;}
            if(waitingForFirstMobileRoom){
                if(!adventureStarted&&!string.IsNullOrEmpty(openMenu)){Keyboard();TickAudio();return;}
                waitingForFirstMobileRoom=false;
            }
            // Keep the last projected world behind a paused overlay. Menu
            // widgets update on their own events; directions still use the
            // live event system. Never restore/step/repaint the hidden world.
            if(Application.isMobilePlatform&&game.paused&&!string.IsNullOrEmpty(openMenu)&&openMenu==paintedPausedMenu&&shownRoom==game.room&&!runAutomatedPlaythrough&&!calibrationRun){
                accumulator=0;Keyboard();TickAudio();return;
            }
            paintedPausedMenu=null;
            combatFrameProbe?.Begin();
            RestoreNativeProjection();
#if UNITY_EDITOR
            if(forestPreviewLoading){RenderForestPreviewStatus();TickForestAutoRun();return;}
#endif
            if(feedbackReview){UpdateFeedbackReview();return;}if(rubenReview){UpdateRubenReview();return;}if(calibrationRun){UpdateCalibration();return;}
            frame++;wallClock+=Time.unscaledDeltaTime;Keyboard();
#if UNITY_ANDROID && !UNITY_EDITOR
            if(battleStick)battleStick.PollAndroid();
#endif
            if(TickBossIntroduction()){
                accumulator=0;ClearInput();combatFrameProbe?.Mark(CombatFrameProbe.Phase.Simulation);
                DrawCombatFrame();
#if UNITY_EDITOR
                RenderCargoEntryDirectorV21();
#endif
                TickAudio();SamplePerformance();
                combatFrameProbe?.End(game.CurrentRoom.id);
#if UNITY_EDITOR
                TickForestAutoRun();
#endif
                return;
            }
            
#if UNITY_EDITOR || ROOT_REDRAW_CHECK_V32
            if(routeCheckOwnsInput)accumulator=0;else
#endif
            accumulator+=Math.Min(Time.unscaledDeltaTime,.1f);
            var input=accumulator>=1f/60?(runAutomatedPlaythrough?Autopilot():ReadInput()):null;int steps=0;
#if UNITY_EDITOR
            if(accumulator>=1f/60)input=ForestAutoInput(input);
#endif
            while(accumulator>=1f/60&&steps++<6){
                // Rebuild binds the new Recycle room's original mine spatial/particle owners before another Step.
                if(PlainProductSessionActiveR1320&&plainProductRouteR1320==PlainProductResourceIndexR1320.Recycle&&shownRoom!=game.room)break;
                if(steps>1&&!runAutomatedPlaythrough)ReadVerticalPress(input);
                RememberSimulationState();
#if UNITY_EDITOR
                RecordSourceInputTraceV17(input,1f/60);
#endif
                if(!StepCargoProductDirectorR1288(1f/60)){
                #if UNITY_EDITOR
                BeforeCoreStopOrdinaryStepDraftV21();
                if(!StepCargoEntryDirectorV21(1f/60)){if(factoryProductSessionR1179)StepFactoryProductR1179(1f/60,input);else game.Step(1f/60,input);AfterCoreIntroOrdinaryStepV25();TickFactoryHostAfterStepR657();}
#else
                if(coreProductSessionR1147)BeforeCoreStopOrdinaryStepDraftV21();if(factoryProductSessionR1179)StepFactoryProductR1179(1f/60,input);else game.Step(1f/60,input);
#endif
                }
                accumulator-=1f/60;input.drop=input.jump=input.attack=input.attackX=input.attackXReleased=input.potion=input.dashPressed=input.awaken=false;input.potionPressedMask=0;input.skill=-1;input.skillReleasedMask=0;}
            combatFrameProbe?.Mark(CombatFrameProbe.Phase.Simulation);
            if(shownRoom!=game.room)RebuildRoom();
            combatFrameProbe?.Mark(CombatFrameProbe.Phase.Room);
            DrawCombatFrame();
#if UNITY_EDITOR
                RenderCargoEntryDirectorV21();
#endif
                TickAudio();SamplePerformance();
            combatFrameProbe?.End(game.CurrentRoom.id);
            CaptureBossIntroductionReady();
            if(game.paused&&!string.IsNullOrEmpty(openMenu))paintedPausedMenu=openMenu;
#if UNITY_EDITOR
            TickForestAutoRun();
#endif
            if(runAutomatedPlaythrough&&!smokeEnd){
                if(smokeFeature==4&&!smokeEntry){smokeEntry=true;Capture("01-entry");}if(game.hits>2&&game.player.x>300&&!smokeCombat){smokeCombat=true;Capture("02-combat");}if(game.room==game.RoomCount-1&&!smokeBoss){smokeBoss=true;Capture("03-boss");}
                CaptureTreeBendersSmoke();
                if(game.victory||game.player.hp<=0||wallClock>240){smokeEnd=true;Capture(game.victory?"04-clear":"04-end");StartCoroutine(QuitAfterCapture());}
            }
                    }catch(Exception trigger){if(plainProductSessionR1320)ClosePlainProductAfterFailureR1320(trigger);if(battleProductSessionR1325)CloseBattleProductAfterFailureR1325(trigger);if(cargoProductSessionR1288)CloseCargoProductAfterFailureR1288(trigger);if(mine2ProductSessionR1233)CloseMine2ProductAfterFailureR1233(trigger);throw;}
        }
        void EnsureEnemyViews(){
            // Source summons append model actors during Step. Preserve the view
            // indices of existing actors; rebuilding the room would reset them.
            while(enemyViews.Count<game.enemies.Count){
                var enemy=game.enemies[enemyViews.Count];
                enemyViews.Add(WorldSprite(enemy.kind,actorsRoot,null,15,Vector3.zero));
                enemyBars.Add(WorldSprite("Enemy HP",actorsRoot,white,40,Vector3.zero));
                warnings.Add(WorldSprite("Enemy Telegraph",actorsRoot,ring,38,Vector3.zero));
                var label=new GameObject("Enemy status");label.transform.SetParent(actorsRoot,false);
                var text=label.AddComponent<TextMesh>();text.font=font;text.fontSize=48;text.characterSize=1.2f;text.anchor=TextAnchor.MiddleCenter;
                text.GetComponent<MeshRenderer>().sharedMaterial=font.material;text.GetComponent<MeshRenderer>().sortingOrder=51;enemyDebuffViews.Add(text);
                armorBreakIcons.Add(null);
            }
        }
        void RenderActors(){BeginMonsterPixelFrameV53();RestoreBatBPaintV41();RestoreRubenForestPaintV40();BeginFirst3NativeRenderFrameV25();PrepareFactoryProductPresentationR1179();
            #if UNITY_EDITOR
            PrepareFactoryHostPresentationR657();
#endif
            EnsureEnemyViews();
            float clock=game.player.clock;string clip=game.player.clip;
            Pose(heroView,catalog.hero.id,clip,RenderPlayerClock);heroView.flipX=game.player.facing<0;var heroPosition=RenderPlayerPosition+SpriteOffset(heroView);heroView.transform.localPosition=new Vector3(heroPosition.x,heroPosition.y,0);heroView.color=Color.white;
            PrepareCameraFramingV25();UpdateWorldCamera();ApplyCameraFramingV25();
            DrawGlassCurtain();
            nearestGuideEnemy=null;
            bossObjectiveTarget=game.AlteraBossObjective(out bossObjectiveText);
            for(int i=0;i<game.enemies.Count;i++){
                var e=game.enemies[i];bool sourceVisible,sourceGauge;game.TryAlteraAirshipRavenPresentationV18(e,out sourceVisible,out sourceGauge);if(game.SwampActorHiddenV39(e)){sourceVisible=false;sourceGauge=false;}ApplyDragonNestSourceVisibilityV20(e,ref sourceVisible,ref sourceGauge);ApplyBelowTunnelSourceVisibilityV21(e,ref sourceVisible,ref sourceGauge);ApplyFactoryProductVisibilityR1179(e,ref sourceVisible,ref sourceGauge);game.ApplyPlainMineSourceVisibilityR1302(e,ref sourceVisible,ref sourceGauge);
#if UNITY_EDITOR
                ApplyCoreHardEntryVisibilityV21(e,ref sourceVisible,ref sourceGauge);ApplyFactoryHostVisibilityR657(e,ref sourceVisible,ref sourceGauge);
#else
                if(coreProductSessionR1147)ApplyCoreHardEntryVisibilityV21(e,ref sourceVisible,ref sourceGauge);
#endif
                var s=enemyViews[i];if(HideCargoProductDirectorBirthR1288(e,i))continue;
                if(peitaSession&&(e.kind=="pd_monster_b"||e.kind=="event_npc_berthe"||e.kind=="event_npc_dullahan")){
                    s.gameObject.SetActive(false);enemyBars[i].enabled=false;warnings[i].enabled=false;
                    enemyDebuffViews[i].text="";
                    continue;
                }
#if UNITY_EDITOR
                if(HideCargoEntryDirectorBirthV21(e,i))continue;
#endif
                game.RegionSourceVisibility(e,ref sourceVisible,ref sourceGauge);
                bool showBody=e.active&&!e.dormant&&sourceVisible;
                if(s.gameObject.activeSelf!=showBody)s.gameObject.SetActive(showBody);
                if(StreamMobileArt&&(!e.active||e.dormant)&&
                    !(sourceVisible&&e.hp>0&&(e.active&&e.telegraph>0||game.GetEnemyDebuff(e)!=null))){
                    // The simulation still advances these births and their source clocks.
                    // A live telegraph or independent debuff label still takes the full path.
                    enemyBars[i].enabled=false;warnings[i].enabled=false;
                    if(!string.IsNullOrEmpty(enemyDebuffViews[i].text))enemyDebuffViews[i].text="";
                    continue;
                }
                float enemyClock=e.clock;
                string enemyClip=InitialNpcRenderClipV25(e,BelowTunnelEntryRenderClipV21(e,ref enemyClock));ApplyFactoryProductPressPoseR1179(e,ref enemyClip,ref enemyClock);enemyClip=CargoProductRenderClipR1288(e,enemyClip,ref enemyClock);
#if UNITY_EDITOR
                enemyClip=CargoEntryRenderClipV21(e,enemyClip,ref enemyClock);ApplyFactoryHostPressPoseR657(e,ref enemyClip,ref enemyClock);
#endif
                if(e.isGate&&e.hp<=0){enemyClip="death";enemyClock=e.deathTime;}
                if(e.state==CombatState.GettingUp&&libraries.TryGetValue(e.kind,out var enemyLibrary)&&enemyLibrary.clips.TryGetValue("get-up",out var rise))enemyClock=(1-e.recoveryTime/game.EnemyRecoveryDuration(e))*rise.duration;
                if(e.clip=="attack"&&e.telegraph>0&&libraries.TryGetValue(e.kind,out var attackArt)&&Mathf.Approximately(attackArt.data.logicalScale,2)&&attackArt.clips.TryGetValue("attack",out var attackClip)){float impact=e.maxHp>400?.73f:.4f;float normalized=e.clock<=impact?.52f*e.clock/impact:.52f+.48f*(e.clock-impact)/.12f;enemyClock=Mathf.Clamp01(normalized)*attackClip.duration;}
                enemyClip=NativeCombatRenderClipV25(e,enemyClip,ref enemyClock);enemyClip=WaterwayNativeRenderClipV26(e,enemyClip,ref enemyClock);enemyClip=OutskirtsNativeRenderClipV27(e,enemyClip,ref enemyClock);bool sourceDiagnostic=CargoWallyPoseV58(e,s)||TryRenderMine2RailOriginalV35(e,s,i);if(!sourceDiagnostic)sourceDiagnostic=TryRenderPlainProductVariablePhaseR1330(e,s);if(!sourceDiagnostic)sourceDiagnostic=TryRenderPlainProductWatchWholeR1330(e,s);if(!sourceDiagnostic)sourceDiagnostic=TryRenderFactoryProductWholeR1179(e,s);
#if UNITY_EDITOR
                if(!sourceDiagnostic){sourceDiagnostic=TryRenderFactoryWholeBankR684(e,s);if(!sourceDiagnostic){ValidateFactoryHostPoseR657(e,enemyClip);sourceDiagnostic=TryRenderCoreHardOriginalSourceActorV21(e,s);}}
#else
                if(!sourceDiagnostic&&coreProductSessionR1147)sourceDiagnostic=TryRenderCoreHardOriginalSourceActorV21(e,s);
#endif
                bool kingWhole=!sourceDiagnostic&&RenderCoreKingWholeCandidateV21(s,e,RenderEnemyPosition(e));float scale=kingWhole?1:(libraries.TryGetValue(e.kind,out var enemyArt)?enemyArt.data.logicalScale:1)*e.visualScale;
                if(!kingWhole&&!sourceDiagnostic){if(!OwnedSourceFramingR1111){enemyClip=NasodReactionRenderClipV28(e,enemyClip,ref enemyClock);enemyClip=CaveNormalRenderClipV28(e,enemyClip,ref enemyClock);enemyClip=Alpha4NormalRenderClipV28(e,sourceVisible,enemyClip,ref enemyClock);enemyClip=Next3NormalRenderClipV28(e,sourceVisible,enemyClip,ref enemyClock);enemyClip=NasodNormalRenderClipV28(e,enemyClip,ref enemyClock);enemyClip=First3CombatRenderClipV28(e,sourceVisible,enemyClip,ref enemyClock);enemyClip=Generic7NormalRenderClipV28(e,i,sourceVisible,enemyClip,ref enemyClock);RecordAncientSelectionV35(e,enemyClip,enemyClock);scale=Next3NormalEffectiveScaleV28(e,enemyClip,scale);}SelectMineMachineOriginalPoseV35(e,ref enemyClip,ref enemyClock);SelectBattleRavenLethalPoseV36(e,ref enemyClip,ref enemyClock);ResolveAirshipSoldierAliasV40(e,ref enemyClip);SelectOriginalHurtClipV45(e,ref enemyClip,ref enemyClock);SelectChickenSourceCarrierV45(e,i,ref enemyClip,ref enemyClock);SelectCannonDeathV35(e,ref enemyClip,ref enemyClock);Pose(s,e.kind,enemyClip,enemyClock,e);RecordMouseSelectionV35(e,s,enemyClip,enemyClock);RecordBendersSelectionV36(e,s,enemyClip,enemyClock);RecordReusablePaintV37(e,s,enemyClip,enemyClock);float legacyBodyRootYV39=RegisterLegacyBodyV39(e,ref scale);s.transform.localScale=Vector3.one*scale;s.flipX=OwnedSourceFramingR1111?e.facing<0:Generic7NormalFlipXV28(e,i,enemyClip,e.facing<0);var enemyPosition=RenderEnemyPosition(e)+SpriteOffset(s)*scale+BelowThinGroundedRegistrationR1481(e,s,scale)+Vector2.up*legacyBodyRootYV39;s.transform.localPosition=new Vector3(enemyPosition.x,enemyPosition.y,0);}
if(!kingWhole&&!sourceDiagnostic)ApplyMagicianSkinV38(e,s,enemyClip,enemyClock);
                if(!kingWhole&&!sourceDiagnostic)ApplyGiantWilliamV39(e,s,enemyClip,enemyClock);
                if(!kingWhole&&!sourceDiagnostic)ApplyAirshipSoldierV40(e,s,enemyClip,enemyClock);
                if(!kingWhole&&!sourceDiagnostic)ApplyBeastPixelV45(e,s,enemyClip,enemyClock);
                float gardenTint=e.peitaAttack!=null && e.peitaAttack.gardenRage ? .2f : 1;
                s.color=new Color(1,gardenTint,gardenTint,CargoDeathAlphaV58(e));ApplyDragonNestSourceColorV20(e,s);ApplyCargoProductAirMaterialR1288(e,s);
#if UNITY_EDITOR
                ApplyCargoEntryAirMaterialV21(e,s);
#endif
                float visibleTop=e.maxHp>400?83:54;
                if(libraries.TryGetValue(e.kind,out var bodyArt)){
                    if(bodyArt.data.standingHeight>0)visibleTop=bodyArt.data.standingHeight*scale;
                    else if(bodyArt.frames.TryFrameCount("idle",out int idleFrames)&&idleFrames>0){
                        var firstIdle=bodyArt.frames.At("idle",0,out _);
                        if(firstIdle&&spriteOffsets.TryGetValue(firstIdle,out var standingOffset))
                            visibleTop=(standingOffset.y+firstIdle.bounds.extents.y)*scale;
                    }
                }
                visibleTop=MagicianVisibleTopV38(e,s,visibleTop);
                visibleTop=GiantWilliamLocalTopV39(e,s,visibleTop);
                visibleTop=AirshipLocalTopV40(e,s,visibleTop);
                visibleTop=BeastLocalTopV45(e,s,visibleTop);
                if(CoreKingWholeActorV21(e))visibleTop=coreKingWholeLookupV21.document.standingHeight;float w=e.maxHp>400?64:30;var b=enemyBars[i];b.enabled=game.EnemyShowsHealthBar(e)&&sourceGauge&&!game.AlteraRepairmanGaugeHiddenV18(e)&&AlteraAirshipGaugeVisibleV20(e,true);b.color=C(0xe46260);b.transform.localPosition=new Vector3(e.x-w*(1-e.hp/e.maxHp)/2,e.y+visibleTop+8,0);b.transform.localScale=new Vector3(w*e.hp/e.maxHp/2,1.5f,1);
                if(showBody)ConsiderEnemyGuideTarget(e,visibleTop);
                var warning=warnings[i];warning.enabled=e.active&&e.hp>0&&e.telegraph>0&&sourceVisible;warning.color=C(0xff6f56,.85f);warning.transform.localPosition=new Vector3(e.x+e.facing*25,game.GroundAt(e.x)+1,0);warning.transform.localScale=new Vector3(e.maxHp>400?1.1f:.6f,.16f,1);
                var debuff=game.GetEnemyDebuff(e);var status=enemyDebuffViews[i];status.text=e.hp>0&&sourceVisible&&debuff!=null?debuff.Label:"";status.color=C(0xffd492);status.transform.localPosition=new Vector3(e.x,e.y+visibleTop+20,0);
            }
            ApplyEnemyInitialPresentationV25();ApplyWaterwayNativeUiV26();ApplyOutskirtsNativeUiV27();if(!OwnedSourceFramingR1111){ApplyNasodReactionUiV28();ApplyCaveNormalUiV28();ApplyAlpha4NormalUiV28();ApplyNext3NormalUiV28();ApplyNasodNormalUiV28();ApplyFirst3CombatUiV28();ApplyGeneric7NormalUiV28();}ApplyRubenForestPaintV40();ApplyBatBPaintV41();
#if UNITY_EDITOR
            LayoutCoreFiniteWholeAnnotationsV703();FinalizeCoreArenaCameraV21();
#else
            if(coreProductSessionR1147){LayoutCoreFiniteWholeAnnotationsV703();FinalizeCoreArenaCameraV21();}
#endif
        }
        EffectView CreateEffect(){var root=new GameObject("Pooled combat effect");root.transform.SetParent(effectsRoot,false);var s=root.AddComponent<SpriteRenderer>();s.sortingOrder=45;
            return new EffectView{root=root,sprite=s};}
        void RenderEffects(){
            npcEffectDepths.Clear();
            WarmCurrentHeroSkill();
            PrepareCombatNumberLanes();
            int effectCount=game.effects.Count;
            // Only the changing tail of the contiguous pool changes visibility.
            // After a large crowd hit, empty slots can stay asleep for the room.
            for(int i=effectCount;i<visibleEffectViewCount;i++)fxViews[i].root.SetActive(false);
            for(int i=visibleEffectViewCount;i<effectCount&&i<fxViews.Count;i++)fxViews[i].root.SetActive(true);
            while(fxViews.Count<effectCount)fxViews.Add(CreateEffect());
            visibleEffectViewCount=effectCount;
            for(int i=0;i<effectCount;i++){
                var v=fxViews[i];var e=game.effects[i];float t=RenderedEffectAge(e)/e.lifetime;
                PrepareSkillEffectView(v);
                v.root.transform.localPosition=e.followPlayer?new Vector3(RenderPlayerPosition.x+game.player.facing*e.offsetX,RenderPlayerPosition.y+e.offsetY,0):new Vector3(e.x,e.y,0);v.root.transform.localScale=Vector3.one;v.root.transform.localRotation=Quaternion.identity;v.sprite.enabled=e.type!="number";v.sprite.flipX=(e.followPlayer?game.player.facing:e.facing)<0;v.sprite.color=Color.white;v.sprite.sortingOrder=45;
                if(e.type=="number"){
                    if(v.number==null)v.number=new CombatNumberView(v.root.transform,FeedbackArt.digits);
                    npcEffectDepths[v.number.root.transform]=e.numberDepth;
                    v.number.Draw(e,t,nearbyCombatNumbers[i],newerCombatNumbers[i]);continue;
                }
                if(v.number!=null)v.number.root.SetActive(false);
                if(e.type=="slash"){v.sprite.enabled=false;continue;}
                if(e.type=="peita-dark-cloud" || e.type=="peita-sonic-wave" || e.type=="peita-ghost" || e.type=="peita-ghost-ring"){
                    v.sprite.sprite=e.type=="peita-sonic-wave"?slash:hit;
                    var bounds=v.sprite.sprite.bounds.size;
                    float fade=Mathf.Min(1,e.age*12)*Mathf.Clamp01((e.lifetime-e.age)*8);
                    v.sprite.color=e.type=="peita-sonic-wave"?C(0x9ae7ff,fade*.72f):C(0xb583ef,fade*(e.type=="peita-dark-cloud"?.44f:.8f));
                    v.root.transform.localScale=new Vector3(e.peitaWidth/Mathf.Max(1,bounds.x),e.peitaHeight/Mathf.Max(1,bounds.y),1);
                    continue;
                }
                if(e.type=="enemy-bolt"||e.type=="peita-poison"){v.sprite.sprite=hit;v.sprite.color=C(e.type=="peita-poison"?0x96cf65u:0x9fecdcu);v.root.transform.localScale=Vector3.one*.4f;v.root.transform.localRotation=Quaternion.Euler(0,0,wallClock*360);continue;}
                if(RenderSkillEffect(v,e))continue;
                string fxId=e.type=="hit"||e.type=="enemy-hit"||e.type=="kick"?"fx-hit":(e.type=="slash"||e.type=="armor-break"||e.type=="sonic-blade"||e.type=="double-slash"||e.type=="rolling-smash"||e.type=="windmill"||e.type=="spiral-blast"?"fx-slash":(e.type=="slam"?"fx-ground-impact":e.type=="guard"||e.type=="counter-attack"?"fx-guard":null));
                if(e.type=="hit"&&game.armor>0&&Math.Abs(e.x-game.player.x)<1)fxId="fx-guard";
                if(fxId!=null&&libraries.TryGetValue(fxId,out var effectLibrary)){
                    Pose(v.sprite,fxId,"play",t*effectLibrary.clips["play"].duration);
                    float scale=fxId=="fx-ground-impact"?.8f:(fxId=="fx-slash"?.6f:.45f);v.root.transform.localScale=Vector3.one*scale;
                    if(fxId=="fx-ground-impact")v.root.transform.localPosition=new Vector3(e.x,game.GroundAt(e.x),0);
                    if(e.type=="sonic-blade"){v.root.transform.localPosition+=new Vector3(e.facing*t*100,0,0);v.root.transform.localScale=Vector3.one*.95f;v.sprite.color=C(0x8ee8ff,1-t*.5f);}
                    if(e.type=="double-slash"){v.root.transform.localScale=new Vector3(1.1f,.85f,1);v.root.transform.localRotation=Quaternion.Euler(0,0,e.facing*(35-80*t));}
                    if(e.type=="rolling-smash"||e.type=="windmill"||e.type=="spiral-blast"){
                        v.root.transform.localRotation=Quaternion.Euler(0,0,e.facing*t*720);v.root.transform.localScale=Vector3.one*(e.type=="windmill"?1.25f:.9f);v.sprite.color=e.type=="spiral-blast"?C(0xa4d6ff):Color.white;
                    }
                    if(e.type=="armor-break")v.sprite.color=C(0xffae6b);
                    continue;
                }
                if(e.type=="mega-slash"||e.type=="flame-geyser"||e.type=="iron-body"){
                    v.sprite.sprite=Picture("Pixel/fx/"+e.type+"/frame-0"+Math.Min(5,(int)(t*6)));float scale=e.type=="flame-geyser"?.72f:.6f;v.root.transform.localScale=Vector3.one*scale;
                    if(e.type=="flame-geyser")v.root.transform.localPosition=new Vector3(e.x,game.GroundAt(e.x)+84*scale,0);
                }else if(e.type=="slash"){v.sprite.sprite=slash;v.root.transform.localScale=new Vector3(1.5f,1.2f,1);v.sprite.color=C(0xdfffae,1-t);v.root.transform.localRotation=Quaternion.Euler(0,0,-e.facing*t*25);}
                else{v.sprite.sprite=e.type=="dust"?dust:hit;v.sprite.color=C(e.type=="heal"?0x89ffceu:0xffd779u,1-t);float scale=(e.type=="slam"?2:1)*(.4f+t);v.root.transform.localScale=Vector3.one*scale;}
            }
            RenderSkillAttachments();
            RenderNpcEffects();
            RenderRecoveryDrops();
        }
        void TextValue(string id,string text){if(labels.TryGetValue(id,out var t)&&t.text!=text)t.text=text;}
        void Visible(string id,bool visible){if(images.TryGetValue(id,out var i)&&i.gameObject.activeSelf!=visible)i.gameObject.SetActive(visible);if(labels.TryGetValue(id,out var t)&&t.gameObject.activeSelf!=visible)t.gameObject.SetActive(visible);}
        void Width(string id,float width){if(images.TryGetValue(id,out var i)){var rt=i.rectTransform;if(Mathf.Abs(rt.rect.width-width)>.001f)rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,width);}}
        static string Number(float n){return Mathf.CeilToInt(Mathf.Max(0,n)).ToString();}
        static readonly string[] BuffWidgets={"Buff Panel","Buff Icon","Buff","Buff Timer"};
        static readonly string[] BannerWidgets={"Banner Panel","Banner","Banner Sub"};
        int lastHudHealth=-1,lastHudMana=-1,lastHudRoom=-1,lastHudCount=-1;
        string lastHudTitle,lastHudRegion,lastHudStage;
        string lastCancelWindow,lastCancelCue;
        string CancelCue(string window){
            if(lastCancelWindow==window)return lastCancelCue;
            lastCancelWindow=window;lastCancelCue="";
            if(!string.IsNullOrEmpty(window))foreach(var part in window.Split('/')){
                string key=part.Trim();int bracket=key.IndexOf('[');
                if(bracket>=0)key=key.Substring(0,bracket).Trim();
                key=key.Replace("dash edge","dash");
                if(key.Length>0)lastCancelCue+=(lastCancelCue.Length>0?" / ":"")+key;
            }
            return lastCancelCue;
        }
        void RenderHud(){
            CaptureDungeonBest();
            RenderHamelAir();
            RenderPlayerVitals();
            string title=DisplayDungeonTitleR1111(),region=DungeonRegion(Dungeon),stage=CurrentStageArt.title;
            if(!string.IsNullOrEmpty(stage)&&stage.StartsWith("WI_",StringComparison.Ordinal))stage=title;
            if(lastHudRoom!=game.room||lastHudCount!=game.RoomCount||lastHudTitle!=title||lastHudRegion!=region||lastHudStage!=stage){
                TextValue("Zone",region.ToUpperInvariant()+" / STAGE "+(game.room+1).ToString("D2"));
                TextValue("Pause Button Text","PAUSE   "+(game.room+1)+" / "+game.RoomCount);
                TextValue("Banner",stage);TextValue("Banner Sub",title+" / AREA "+(game.room+1)+" OF "+game.RoomCount);
                lastHudRoom=game.room;lastHudCount=game.RoomCount;lastHudTitle=title;lastHudRegion=region;lastHudStage=stage;
            }
            TextValue("Title",title);TextValue("Mission",EnemyGuideMission());Width("Progress Fill",114*game.player.x/game.RoomWidth);
            string command=game.CurrentCommand;TextValue("Command Input",command);RenderSkillSlots();
            bool showLink=!string.IsNullOrEmpty(command)&&string.IsNullOrEmpty(game.ActiveSkillId);
            string cue=showLink?CancelCue(game.CurrentCancelWindow):"";
            TextValue("Link Window",showLink?game.CurrentPhase.ToUpperInvariant()+(cue.Length>0?"  |  LINK: "+cue:""):"");if(labels.TryGetValue("Link Window",out var linkLabel))linkLabel.color=cue.Length>0?C(0x90f7b9):C(0xb9cfce);
            TextValue("Skill Phase",string.IsNullOrEmpty(game.ActiveSkillId)?"":game.SkillPhaseLabel);
            foreach(string name in BuffWidgets)Visible(name,game.armor>0);if(game.armor>0){Width("Buff Timer",204*game.armor/GameModel.ArmorDuration);TextValue("Buff","IRON BODY  "+Number(game.armor)+"s  /  DMG -38.6%");}
            TextValue("Combat State",game.player.state==CombatState.Knockdown?"KNOCKED DOWN / INVULNERABLE":game.player.state==CombatState.GettingUp?"GETTING UP / INVULNERABLE":game.player.invincible>0?"INVULNERABLE":!game.Grounded(game.player)?"AIRBORNE":"");
            Visible("Charge Track",game.IsCharging||game.IsSkillCharging);Visible("Charge Fill",game.IsCharging||game.IsSkillCharging);Width("Charge Fill",140*(game.IsSkillCharging?game.SkillChargeAmount:game.ChargeAmount));
            RenderComboReadout();TextValue("Gate Prompt",ExitGatePrompt());
            RenderRegionalWarning(peitaSession);
            RefreshBattleControls();
            bool banner=game.roomBanner>0&&!game.victory;
#if UNITY_EDITOR
            banner=CoreRoomBannerVisibleV31(banner,coreStopIntroBridgeDraftV21!=null&&coreStopIntroBridgeDraftV21.ActiveV31);
#else
            if(coreProductSessionR1147)banner=CoreRoomBannerVisibleV31(banner,coreStopIntroBridgeDraftV21!=null&&coreStopIntroBridgeDraftV21.ActiveV31);
#endif
            foreach(string name in BannerWidgets)Visible(name,banner);
            bool result=!monsterArtReview&&!settingsOpen&&string.IsNullOrEmpty(openMenu)&&(game.paused||game.victory||game.player.hp<=0);
            RenderResultSheet(result);
            RenderStageClear();
#if UNITY_EDITOR
            RenderForestPreviewStatus();
#endif
            RenderLakeNightAdmissionNoticeV19();
            RenderDragonNestAdmissionNoticeV20();
            RenderBossIntroduction();
        }
        // CanvasScaler applies its factor in Update. Running after every Update
        // prevents the unscaled Awake canvas from supplying a second HUD scale.
        void LateUpdate(){if(mobileStartupPending)return;FitScreen();RenderEnemyGuide();MaybeCapturePackagedDragonNestV20();}
        internal int HudGeometryVersion {get;private set;}
        void FitScreen(){
            var size=new Vector2(Screen.width,Screen.height);Rect safe=Screen.safeArea;
            if(size.x<=0||size.y<=0||!hudCanvas||!safeRoot||!worldCamera)return;
            var canvasTransform=hudCanvas.GetComponent<RectTransform>();
            var canvasSize=canvasTransform.rect.size;float canvasScale=hudCanvas.scaleFactor;
            if(size==lastSize&&safe==lastSafeArea&&canvasSize==lastCanvasSize&&Mathf.Approximately(canvasScale,lastCanvasScale))return;
            float aspect=CameraFramingPolicyV25.Aspect,actual=size.x/size.y;Rect viewport=Application.isMobilePlatform?new Rect(0,0,1,1):actual>aspect?new Rect((1-aspect/actual)*.5f,0,aspect/actual,1):new Rect(0,(1-actual/aspect)*.5f,1,actual/aspect);
            // Keep display and HUD safe-area calculations on whole screen pixels.
            int pixelWidth=Mathf.Clamp(Mathf.RoundToInt(viewport.width*size.x),1,(int)size.x),pixelHeight=Mathf.Clamp(Mathf.RoundToInt(viewport.height*size.y),1,(int)size.y);
            int pixelX=Mathf.RoundToInt((size.x-pixelWidth)*.5f),pixelY=Mathf.RoundToInt((size.y-pixelHeight)*.5f);
            SetWorldSurface(new Rect(pixelX,pixelY,pixelWidth,pixelHeight),Application.isMobilePlatform);
            viewport=worldCamera.rect; // The existing HUD safe-area fit follows the actual camera viewport.
            if(safe.width<=0||safe.height<=0)safe=new Rect(0,0,size.x,size.y);
            Vector2 min=new Vector2(Mathf.Max(viewport.x,safe.x/size.x),Mathf.Max(viewport.y,safe.y/size.y));
            Vector2 max=new Vector2(Mathf.Min(viewport.xMax,safe.xMax/size.x),Mathf.Min(viewport.yMax,safe.yMax/size.y));
            if(max.x<=min.x||max.y<=min.y){min=viewport.min;max=viewport.max;}
            Canvas.ForceUpdateCanvases();var canvasRect=canvasTransform.rect;
            Vector2 available=new Vector2(canvasRect.width*(max.x-min.x),canvasRect.height*(max.y-min.y));
            float scale=Mathf.Min(available.x/960,available.y/540);if(scale<=0)return;
            // Keep all authored HUD coordinates in one 960x540 logical space.
            // Stretching only the safe-area parent would leave fixed-position
            // right/top controls outside its bounds on 20:9 and notched phones.
            safeRoot.anchorMin=safeRoot.anchorMax=min;safeRoot.pivot=Vector2.zero;safeRoot.sizeDelta=new Vector2(960,540);
            safeRoot.localScale=new Vector3(scale,scale,1);
            safeRoot.anchoredPosition=new Vector2((available.x-960*scale)*.5f,(available.y-540*scale)*.5f);
            lastSize=size;lastSafeArea=Screen.safeArea;lastCanvasSize=canvasRect.size;lastCanvasScale=hudCanvas.scaleFactor;
            HudGeometryVersion++;
            FitMenuBackdrops();
        }
        void Capture(string name){
            lastCaptureRealtime=Time.realtimeSinceStartup;
            Directory.CreateDirectory(captureDir);if(UseOffscreenCaptures)CaptureOffscreenFrame(Path.Combine(captureDir,name+".png"));else ScreenCapture.CaptureScreenshot(Path.Combine(captureDir,name+".png"));
            var corners=new Vector3[4];safeRoot.GetWorldCorners(corners);
            int alerted=0,downed=0;bool all=true;foreach(var enemy in game.enemies){if(enemy.alerted)alerted++;if(enemy.state==CombatState.Knockdown||enemy.state==CombatState.GettingUp)downed++;all&=enemy.active;}
            var skills=new string[GameModel.SkillSlotCount];for(int i=0;i<GameModel.SkillSlotCount;i++)skills[i]=game.GetEquippedSkill(i).id;
            var remaining=new List<CaptureEnemy>();foreach(var e in game.enemies)if(e.active&&e.hp>0)remaining.Add(new CaptureEnemy{npc=e.sourceNPC,kind=e.kind,state=e.npcState,x=e.x,y=e.y,hp=e.hp,groundLine=e.groundLine,dormant=e.dormant,passive=e.passive});
            var drops=new List<CaptureDrop>();foreach(var d in game.recoveryDrops)drops.Add(new CaptureDrop{resource=d.resource,sourceNPC=d.sourceNPC,x=d.x,y=d.y,amount=d.amount,grounded=d.grounded});
            var state=new CaptureState{bossIntroActive=BossIntroductionActive,bossIntroKind=bossIntroductionKind,bossIntroRemaining=bossIntroductionRemaining,gateTransitionRemaining=game.GateTransitionDelayRemaining,room=game.room,kills=game.kills,hits=game.hits,hp=game.player.hp,victory=game.victory,
                modelTime=game.elapsed,gateOpen=game.gateOpen,encounter=game.EncounterIndex,remainingEnemies=remaining.ToArray(),
                recoveryDropsSpawned=game.RecoveryDropsSpawned,recoveryDropsPickedUp=game.RecoveryDropsPickedUp,recoveryPickupSounds=RecoveryPickupSoundPlays,recoveryItems=drops.ToArray(),
                playerX=game.player.x,playerY=game.player.y,cameraX=worldCamera.transform.localPosition.x,cameraY=worldCamera.transform.localPosition.y,groundY=game.PlayerGroundY,grounded=game.Grounded(game.player),terrainNodes=calibrationRun?0:CurrentStageArt.groundNodes.Length,roomWidth=game.RoomWidth,state=game.player.state.ToString(),command=game.CurrentCommand,menu=openMenu,loadout=skills,alertedEnemies=alerted,downedEnemies=downed,allEnemiesPresent=all,
                canvasSize=hudCanvas.GetComponent<RectTransform>().rect.size,canvasScale=hudCanvas.scaleFactor,hudLocalScale=safeRoot.localScale.x,hudBottomLeft=corners[0],hudTopRight=corners[2],cameraFraming=CameraFramingObservationV25()};
            File.WriteAllText(Path.Combine(captureDir,name+".json"),JsonUtility.ToJson(state,true));
#if UNITY_EDITOR
            if(forestPreviewActive)SaveForestPreviewCapture(name);
#endif
        }
        IEnumerator QuitAfterCapture(){WritePerformanceReport();yield return new WaitForSecondsRealtime(1);Application.Quit();}
    
        internal static string FormatBelowCannonFractionalNumberR901A2(float amount){
            if(float.IsNaN(amount)||float.IsInfinity(amount)||amount<=0)return "?";
            if(amount<.01f)return "<0.01";
            return System.Math.Round((double)amount,2,System.MidpointRounding.AwayFromZero).ToString("0.00",System.Globalization.CultureInfo.InvariantCulture);
        }

        internal static bool CoreRoomBannerVisibleV31(bool ordinaryVisible,bool introFocusActive){return ordinaryVisible&&!introFocusActive;}

        bool TryCoreFiniteWholeCanvasTopV703(Enemy e,SpriteRenderer body,out float top){
            top=0;if(!coreEntrySessionV21||!forestPreviewActive||!e.active||e.dormant||string.IsNullOrEmpty(e.npcState)||!body||!body.enabled||!body.gameObject.activeInHierarchy||!body.sprite)return false;
            SourcePhaseWholeLookupV21 lookup;Dictionary<string,Sprite> sprites;
            if(e.sourceNPC=="NUI_NASOD_ELEMENT_FIRE_B"){lookup=coreElementFireBWholeLookupV21;sprites=coreElementFireBWholeSpritesV21;}
            else if(e.sourceNPC=="NUI_NASOD_ELEMENT_ICE_B"){lookup=coreElementIceBWholeLookupV21;sprites=coreElementIceBWholeSpritesV21;}
            else return false;
            if(lookup==null||e.visualScale!=1||body.transform.localScale!=Vector3.one||body.transform.localRotation!=Quaternion.identity||body.flipX||body.flipY)return false;
            // The immutable existing lookup reads this actor's real state/time;
            // global LastAnchor fields would be wrong for multiple actors.
            var selected=lookup.Select(e.npcState,e.npcTime);var key=selected.Whole;Sprite expected;
            if(!sprites.TryGetValue(key.Key,out expected)||!ReferenceEquals(body.sprite,expected)||body.sprite.rect.width!=key.Width||body.sprite.rect.height!=key.Height)return false;
            var offset=new Vector2((float)(key.Width*.5-selected.AnchorX),(float)(selected.AnchorY-key.Height*.5));var root=RenderEnemyPosition(e)+offset;var actual=body.transform.localPosition;
            if(actual.x!=root.x||actual.y!=root.y||actual.z!=0)return false;
            // Centered FullRect bounds plus the already signed-anchor-registered
            // current sprite position. No crop, extra floor/offset or projection.
            top=actual.y+body.sprite.bounds.max.y;return !float.IsNaN(top)&&!float.IsInfinity(top);
        }

        void LayoutCoreFiniteWholeAnnotationsV703(){
            if(!coreEntrySessionV21||!forestPreviewActive||projectionRestore.Count!=0)return;
            for(int i=0;i<game.enemies.Count;i++){
                float top;if(!TryCoreFiniteWholeCanvasTopV703(game.enemies[i],enemyViews[i],out top))continue;
                // Keep existing X/Z, enabled/text, color, layer, sprite and scale.
                // HP/status retain their original8/20 logical-unit clearances.
                var hp=enemyBars[i].transform.localPosition;hp.y=top+8;enemyBars[i].transform.localPosition=hp;
                var status=enemyDebuffViews[i].transform.localPosition;status.y=top+20;enemyDebuffViews[i].transform.localPosition=status;
                // The existing small telegraph marker remains visible under its
                // original condition;4logical units place it below HP and above
                // the intact canvas, without hiding or rescaling any marker.
                var warning=warnings[i].transform.localPosition;warning.y=top+4;warnings[i].transform.localPosition=warning;
            }
        }
        string DisplayDungeonTitleR1111(){
#if UNITY_EDITOR
            if(factoryHostSessionR657)return "NASOD FACTORY";
            if(coreEntrySessionV21)return "ALTERA CORE / HARD";
            if(belowTunnelEntrySessionV21)return "BELOW TUNNEL";
#endif
            return Dungeon.title;
        }
}
}
