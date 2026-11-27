using System;
using System.Collections.Generic;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  int selectedDungeon,previewDungeon,mapSelected,mapPage;
  string mapRegion="ruben";
  readonly List<CandidateDungeonArt> mapDungeons=new List<CandidateDungeonArt>();
  readonly List<int> mapVisible=new List<int>();
  [Serializable] class DungeonPostcards {public int width,height;public DungeonPostcard[] entries;}
  [Serializable] class DungeonPostcard {public string id,boss;public int x,y;[NonSerialized] public Sprite sprite;}
  readonly Dictionary<string,DungeonPostcard> dungeonPostcards=new Dictionary<string,DungeonPostcard>(StringComparer.Ordinal);
  static readonly string[] MapRegions={"ruben","elder"};
  const int MapPageSize=6;
  int StageIndex {get{return Dungeon.start+game.room;}}
  DungeonArt Dungeon {get{return catalog.dungeons[selectedDungeon];}}
  string DungeonRegion(DungeonArt dungeon){return string.IsNullOrEmpty(dungeon.region)?"ruben":dungeon.region;}

  void OpenStartupDungeonMap(){
   SetMenu("dungeons");
   var args=Environment.GetCommandLineArgs();
   int option=Array.IndexOf(args,"--region");
   if(option>=0&&option+1<args.Length&&Array.IndexOf(MapRegions,args[option+1])>=0)
    DungeonMapAction("region:"+args[option+1]);
  }
  void InitializeDungeonMap(){
   mapDungeons.Clear();
   for(int i=0;i<catalog.dungeons.Length;i++){
    var d=catalog.dungeons[i];var rooms=new List<CandidateRoomArt>();int zones=0;
    for(int j=0;j<d.count;j++){var stage=catalog.stages[d.start+j];int count=stage.encounters==null?1:Math.Max(1,stage.encounters.Length);zones+=count;
     rooms.Add(new CandidateRoomArt{id=stage.sourceWorldId,width=stage.width,zones=count,spawnCount=stage.spawns==null?0:stage.spawns.Length,bgm=stage.bgm,bgmAvailable=!string.IsNullOrEmpty(stage.bgm)});}
    mapDungeons.Add(new CandidateDungeonArt{id=d.id,title=d.title,region=ReleaseDungeonRegion(i),description=d.description,count=d.count,combatZones=zones,runtimeIndex=i,rooms=rooms.ToArray()});
   }
   SelectRuntimeDungeonOnMap(previewDungeon);
  }
  void SelectRuntimeDungeonOnMap(int index){
   StopMapMusicPreview();
   previewDungeon=Mathf.Clamp(index,0,Math.Max(0,catalog.dungeons.Length-1));
   for(int i=0;i<mapDungeons.Count;i++)if(mapDungeons[i].runtimeIndex==previewDungeon){mapSelected=i;mapRegion=mapDungeons[i].region;break;}
   RefreshMapVisible();
   int visible=mapVisible.IndexOf(mapSelected);
   mapPage=Math.Max(0,visible)/MapPageSize;
   revealDungeonRow=Math.Max(0,visible);
  }
  void RefreshMapVisible(){
   mapVisible.Clear();
   for(int i=0;i<mapDungeons.Count;i++)if(mapDungeons[i].region==mapRegion)mapVisible.Add(i);
   mapPage=Mathf.Clamp(mapPage,0,Math.Max(0,(mapVisible.Count-1)/MapPageSize));
  }
  void LoadDungeonPostcards(){
   if(dungeonPostcards.Count!=0)return;
   const string prefix="Pixel/ui/dungeon-preview/postcards";
   var index=Resources.Load<TextAsset>(prefix+"-index");
   var atlas=Resources.Load<Texture2D>(prefix);
   if(!index||!atlas){Debug.LogError("Dungeon postcard atlas is missing.");return;}
   var doc=JsonUtility.FromJson<DungeonPostcards>(index.text);
   atlas.filterMode=FilterMode.Point;
   foreach(var entry in doc.entries){
    if(Array.IndexOf(ReleaseDungeonIds,entry.id)<0)continue;
    entry.sprite=Sprite.Create(atlas,new Rect(entry.x,entry.y,doc.width,doc.height),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);
    dungeonPostcards.Add(entry.id,entry);
   }
  }
  DungeonPostcard GetDungeonPostcard(CandidateDungeonArt row){
   if(row==null)return null;
   string id=row.id;
   dungeonPostcards.TryGetValue(id,out var card);return card;
  }
  void BuildDungeonMenu(){
   if(mapDungeons.Count==0)InitializeDungeonMap();
   var root=Panel("Dungeon Root",new Rect(0,0,960,540),C(0x17283c),null);root.raycastTarget=true;
   var parent=root.transform;
   Panel("Map Header",new Rect(14,468,932,60),C(0xc6dced),parent);
   HeaderInk(Label("Region Eyebrow","ADVENTURE MAP",new Rect(32,503,640,14),9,Color.white,TextAnchor.MiddleLeft,parent));
   HeaderInk(Label("Region Title","DUNGEON SELECT",new Rect(32,473,650,30),24,Color.white,TextAnchor.MiddleLeft,parent));
   Button("Dungeon Settings","SETTINGS",new Rect(805,476,122,40),"settings",11,parent);
   for(int i=0;i<MapRegions.Length;i++)
    Button("Region Tab "+i,MapRegions[i].ToUpperInvariant(),new Rect(24+i*100,423,94,36),"region:"+MapRegions[i],12,parent);
   Label("Region Note","",new Rect(723,425,208,32),10,C(0xb7ccdf),TextAnchor.MiddleRight,parent);
   BuildDungeonList(parent);
   Panel("Dungeon Detail",new Rect(521,65,415,348),C(0x23384f),parent);
   var illustration=Panel("Dungeon Illustration Preview",new Rect(531,225,395,177),Color.white,parent);
   illustration.preserveAspect=true;
   var heading=WrappedLabel("Dungeon Detail Title","",new Rect(536,184,385,36),22,C(0xffe8ae),parent,TextAnchor.MiddleLeft);
   heading.resizeTextForBestFit=true;heading.resizeTextMinSize=14;heading.resizeTextMaxSize=22;
   Label("Boss Identity","",new Rect(536,164,385,20),12,C(0xbddaf4),TextAnchor.MiddleLeft,parent);
   WrappedLabel("Dungeon Description","",new Rect(536,119,385,38),12,C(0xdce9f1),parent,TextAnchor.MiddleLeft);
   Label("Dungeon Route","",new Rect(536,102,385,17),9,C(0xabc6df),TextAnchor.MiddleLeft,parent);
   for(int i=0;i<5;i++)Button("Dungeon BGM "+i,"AREA "+(i+1),new Rect(536+i*77,72,71,27),"dungeon-bgm:"+i,10,parent);
   Button("Dungeon Skills","SKILL LOADOUT",new Rect(24,14,190,41),"loadout",12,parent);
   Label("Dungeon Status","",new Rect(521,14,166,41),10,C(0xb3e7da),TextAnchor.MiddleLeft,parent);
   Button("Dungeon Enter","ENTER DUNGEON",new Rect(686,14,250,41),"dungeon-enter",14,parent);
   root.gameObject.SetActive(false);
  }
  bool DungeonMapAction(string action){
   if(action.StartsWith("dungeon-bgm:")){int room;if(int.TryParse(action.Substring(12),out room)&&mapSelected>=0&&mapSelected<mapDungeons.Count){var rooms=mapDungeons[mapSelected].rooms;if(rooms!=null&&room>=0&&room<rooms.Length)StartMapMusicPreview(rooms[room]);}return true;}
   if(action.StartsWith("region:")){string region=action.Substring(7);if(Array.IndexOf(MapRegions,region)<0)return true;StopMapMusicPreview();mapRegion=region;mapPage=0;RefreshMapVisible();mapSelected=mapVisible.Count>0?mapVisible[0]:-1;revealDungeonRow=0;RenderMenus();return true;}
   if(action.StartsWith("dungeon-card:")||action.StartsWith("dungeon-row:")){
    RefreshMapVisible();bool legacy=action.StartsWith("dungeon-card:");int slot;
    if(int.TryParse(action.Substring(legacy?13:12),out slot)&&slot>=0&&(!legacy||slot<MapPageSize))
     SelectDungeonListRow(legacy?mapPage*MapPageSize+slot:slot);
    return true;
   }
   if(action.StartsWith("dungeon-page:")){RefreshMapVisible();int step;if(int.TryParse(action.Substring(13),out step)){mapPage=Mathf.Clamp(mapPage+step,0,Math.Max(0,(mapVisible.Count-1)/MapPageSize));if(dungeonList)ScrollDungeonListTo(dungeonList.content.anchoredPosition.y+step*dungeonList.viewport.rect.height);}RenderMenus();return true;}
   return false;
  }
  void RenderDungeonMenu(){
   // The shipped scene already contains baked UI objects. Its nonserialized
   // route list must also be initialized when BuildMenus is skipped in Awake.
   if(mapDungeons.Count==0)InitializeDungeonMap();
   if(!images.TryGetValue("Dungeon Root",out var root))return;root.gameObject.SetActive(openMenu=="dungeons");if(openMenu!="dungeons"){StopMapMusicPreview();return;}RefreshMapVisible();
   LoadDungeonPostcards();
   for(int i=0;i<MapRegions.Length;i++){
    bool active=mapRegion==MapRegions[i];
    images["Region Tab "+i].color=C(active?0x47798eu:0x2c425cu);
    labels["Region Tab "+i+" Text"].color=active?C(0xffe3a3):C(0xd0e2f2);
   }
   TextValue("Region Note",mapRegion.ToUpperInvariant()+"   /   "+mapVisible.Count+" DUNGEONS");
   RenderDungeonList(root.transform);
   bool selected=mapSelected>=0&&mapSelected<mapDungeons.Count&&mapDungeons[mapSelected].region==mapRegion;
   var chosen=selected?mapDungeons[mapSelected]:null;
   bool ready=chosen!=null&&IsReleaseDungeon(chosen.runtimeIndex,chosen.id);
   TextValue("Region Eyebrow","ADVENTURE MAP / NORMAL");
   var selectedArt=GetDungeonPostcard(chosen);
   var hero=images["Dungeon Illustration Preview"];hero.sprite=selectedArt?.sprite;hero.enabled=hero.sprite;
   TextValue("Boss Identity",selectedArt!=null?"BOSS   /   "+DungeonBossLabel(selectedArt.boss):"");
   TextValue("Dungeon Detail Title",chosen==null?"NO DUNGEONS AVAILABLE":chosen.title);
   TextValue("Dungeon Description",chosen==null?"Select another region to choose a dungeon.":chosen.description);
   TextValue("Dungeon Status",ready?"READY TO ENTER":"");
   TextValue("Dungeon Enter Text","ENTER DUNGEON");
   images["Dungeon Enter"].color=ready?C(0x476b61):C(0x293944);
   TextValue("Dungeon Route",chosen==null?"":chosen.count+" ROOMS   /   "+chosen.combatZones+" COMBAT ZONES");
   for(int i=0;i<5;i++){
    bool present=chosen!=null&&i<chosen.count;
    string key="Dungeon BGM "+i;Visible(key,present);if(!present)continue;
    bool music=chosen.rooms!=null&&i<chosen.rooms.Length&&chosen.rooms[i]!=null&&chosen.rooms[i].bgmAvailable;
    images[key].raycastTarget=music;
    images[key].color=music?C(0x395875):C(0x273e56);
    TextValue(key+" Text",(music?"> ":"")+"AREA "+(i+1));
   }
  }
  void EnterSelectedDungeon(){if(!TryStartMobileProductEntry())EnterSelectedDungeonNow();}
  void EnterSelectedDungeonNow(){
   if(mapSelected<0||mapSelected>=mapDungeons.Count)return;
   var route=mapDungeons[mapSelected];
   if(route.region!=mapRegion||!IsReleaseDungeon(route.runtimeIndex,route.id))return;
   StartSelectedCatalogDungeon();
  }
  void StartSelectedCatalogDungeon(){
   previewDungeon=mapDungeons[mapSelected].runtimeIndex;
   adventureStarted=true;selectedDungeon=previewDungeon;openMenu="";pausedBeforeMenu=false;ConfigureRooms();ApplyTimings();game.paused=false;shownRoom=-1;previousRoom=-1;cameraFollowRoom=-1;accumulator=0;ClearInput();RebuildRoom();ResetAudioTracking();RenderMenus();
  }

 }
}
