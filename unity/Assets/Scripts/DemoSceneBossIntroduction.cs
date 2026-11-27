using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  const float BossIntroductionDuration=2.2f;
  RectTransform bossIntroductionRoot,bossIntroductionBand,bossIntroductionPortraitRoot;
  CanvasGroup bossIntroductionFade;
  Text bossIntroductionName,bossIntroductionRole;
  Image bossIntroductionPortrait;
  float bossIntroductionRemaining;
  int bossIntroductionRoom=-1;
  bool bossIntroductionCapture,bossIntroductionCaptured,bossIntroductionReadyCapture;
  readonly HashSet<Enemy> dangerShownActors=new HashSet<Enemy>();
  GameModel dangerModel;
  string bossIntroductionKind="";
  public bool BossIntroductionActive {get{return bossIntroductionRemaining>0;}}

  void EnsureBossIntroduction(){
   if(bossIntroductionRoot)return;
   bossIntroductionRoot=UIRect("Boss Introduction",safeRoot,new Rect(0,0,960,540));
   bossIntroductionRoot.gameObject.AddComponent<RectMask2D>();
   var shade=bossIntroductionRoot.gameObject.AddComponent<Image>();shade.color=C(0x050911,.78f);shade.raycastTarget=true;
   bossIntroductionFade=shade.gameObject.AddComponent<CanvasGroup>();
   bossIntroductionBand=UIRect("Danger horizontal band",bossIntroductionRoot,new Rect(0,0,960,540));
   Panel("Danger backdrop",new Rect(68,141,824,253),C(0x111b28,.98f),bossIntroductionBand);
   Panel("Danger upper Accent",new Rect(0,408,960,5),C(0xe85c4a),bossIntroductionBand);
   Panel("Danger lower Accent",new Rect(0,115,960,5),C(0xe85c4a),bossIntroductionBand);
   for(int row=0;row<2;row++)for(int i=0;i<24;i++){
    float y=row==0?421:92;Panel("Danger stripe "+row+" "+i,new Rect(i*42+(row==0?0:18),y,26,8),i%2==0?C(0xe6b76b):C(0x843d37),bossIntroductionBand);
   }
   Label("Danger repeating top","DANGER  /  DANGER  /  DANGER  /  DANGER",new Rect(0,443,960,25),16,C(0xf2ba85),TextAnchor.MiddleCenter,bossIntroductionBand);
   bossIntroductionRole=Label("Danger repeating bottom","BOSS ENCOUNTER",new Rect(104,149,430,26),12,C(0xc4cbc5),TextAnchor.MiddleLeft,bossIntroductionBand);
   var danger=Label("Danger title","DANGER",new Rect(103,282,440,75),44,C(0xffbc70),TextAnchor.MiddleLeft,bossIntroductionBand);
   if(pixelFont)danger.font=pixelFont;
   Label("Danger small title","A POWERFUL ENEMY APPROACHES",new Rect(105,258,440,25),12,C(0xd8d4c6),TextAnchor.MiddleLeft,bossIntroductionBand);
   bossIntroductionName=Label("Danger boss name","",new Rect(104,193,446,52),25,C(0xf8ebc9),TextAnchor.MiddleLeft,bossIntroductionBand);
   bossIntroductionName.resizeTextForBestFit=true;bossIntroductionName.resizeTextMinSize=15;bossIntroductionName.resizeTextMaxSize=25;
   bossIntroductionName.horizontalOverflow=HorizontalWrapMode.Wrap;bossIntroductionName.verticalOverflow=VerticalWrapMode.Truncate;
   bossIntroductionPortraitRoot=UIRect("Boss close-up mask",bossIntroductionBand,new Rect(560,120,300,304));
   bossIntroductionPortraitRoot.gameObject.AddComponent<RectMask2D>();
   bossIntroductionPortrait=Panel("Boss close-up Portrait",new Rect(12,12,276,280),Color.white,bossIntroductionPortraitRoot);
   bossIntroductionPortrait.preserveAspect=true;
   Panel("Danger portrait Accent",new Rect(550,155,3,218),C(0xbf7550),bossIntroductionBand);
   bossIntroductionRoot.gameObject.SetActive(false);
  }
  Enemy BossIntroductionSubject(){
   if(!ReferenceEquals(dangerModel,game)){dangerShownActors.Clear();dangerModel=game;}
   dangerShownActors.RemoveWhere(e=>!game.enemies.Contains(e));
   Enemy selected=null;
   foreach(var enemy in game.enemies){
    if(!enemy.active||enemy.dormant||enemy.encounterIndex!=game.EncounterIndex||enemy.hp<=0||dangerShownActors.Contains(enemy))continue;
    if(!game.IsBossIntroductionActor(enemy))continue;
    if(!game.IsDungeonBoss(enemy)&&Math.Abs(enemy.x-game.player.x)>360)continue;
    if(selected==null||enemy.maxHp>selected.maxHp)selected=enemy;
   }
   return selected;
  }
  void BeginBossIntroduction(){
   CancelBossIntroduction();
   if(!adventureStarted||calibrationRun||rubenReview||monsterArtReview||game.victory)return;
   var boss=BossIntroductionSubject();if(boss==null)return;
   foreach(var actor in game.enemies)if(actor.active&&!actor.dormant&&actor.encounterIndex==game.EncounterIndex&&actor.hp>0&&game.IsBossIntroductionActor(actor)&&(game.IsDungeonBoss(actor)||Math.Abs(actor.x-game.player.x)<=360))dangerShownActors.Add(actor);
   dangerShownActors.Add(boss);
   foreach(var successor in game.enemies)
    if(successor.dormant && successor.encounterIndex==boss.encounterIndex && successor.spawnAfterDeath==boss.kind && game.IsBossIntroductionActor(successor))
     dangerShownActors.Add(successor);
   EnsureBossIntroduction();bossIntroductionKind=boss.kind;bossIntroductionRoom=game.room;
   bossIntroductionName.text=BossIntroductionDisplayName(boss);
   bossIntroductionRole.text=game.IsDungeonBoss(boss)?"BOSS ENCOUNTER":"ELITE ENCOUNTER";
   bool ancient=game.CurrentRoom.id=="swamp-3"&&boss.sourceNPC==AncientRigV35.Npc&&boss.kind==AncientRigV35.Kind;
   Sprite portrait=null;bool wholePortrait=TryMonsterPixelPortraitV53(boss,out portrait);
   if(!wholePortrait&&!ancient){
    if(boss.sourceNPC=="NUI_BENDERS_NORMAL")portrait=TreeBendersFrames.StandingSprite;
    else if(libraries.TryGetValue(boss.kind,out var library)&&library.frames.TryGetValue("idle",out var idle)&&idle.Length>0)portrait=GiantWilliamIntroductionV39(boss,idle[0]);
   }
   bossIntroductionPortrait.sprite=portrait;bossIntroductionPortrait.enabled=portrait;bossIntroductionPortrait.color=Color.white;
   bool left=ancient?boss.facing>0:boss.facing<0;bossIntroductionPortrait.rectTransform.localScale=new Vector3(left?-1:1,1,1);
   // Every actor has its own tightly cropped idle rectangle. Fit that actual
   // rectangle, rather than applying the old Benders-specific negative offset.
   var portraitSize=portrait?portrait.rect.size:Vector2.one;float portraitScale=Mathf.Min(276/Mathf.Max(1,portraitSize.x),280/Mathf.Max(1,portraitSize.y));
   portraitSize*=portraitScale;bossIntroductionPortrait.rectTransform.sizeDelta=portraitSize;
   bossIntroductionPortrait.rectTransform.anchoredPosition=new Vector2((300-portraitSize.x)*.5f+(left?portraitSize.x:0),(304-portraitSize.y)*.5f);
   bossIntroductionRemaining=BossIntroductionDuration;bossIntroductionCaptured=false;bossIntroductionReadyCapture=false;
   bossIntroductionCapture=runAutomatedPlaythrough;
#if UNITY_EDITOR
   bossIntroductionCapture|=forestAutoArmed||forestAutoRunning;
#endif
   game.roomBanner=0;accumulator=0;ClearInput();bossIntroductionRoot.SetAsLastSibling();RenderBossIntroduction();
   Debug.Log("BOSS_INTRO_BEGIN room="+game.room+" actor="+boss.kind+" seconds="+BossIntroductionDuration+" modelTime="+game.elapsed);
  }
  static string DungeonBossLabel(string name){
   string title=(name??"").ToUpperInvariant();
   if(title.StartsWith("BOSS / ",StringComparison.Ordinal))title=title.Substring(7);
   return title=="LIZARDMAN OLDER"?"LIZARDMAN ELDER":title;
  }
  string BossIntroductionDisplayName(Enemy boss){
   switch(boss.sourceNPC){
    case "NUI_PPORU_WILLIAM_B":return "WILLIAM PHORU";
    case "NUI_WALLY_9TH_ARMOR":return "WALLY NO. 9 / ARMOR";
    case "NUI_WALLY_9TH":return "WALLY NO. 9";
    case "NUI_LIZARDMAN_HIGH":return "LIZARDMAN WARRIOR";
    case "NUI_NASOD_ELEMENT_FIRE":return "IGNIS";
    case "NUI_NASOD_ELEMENT_ICE":return "LEVIATHAN";
   }
   if(game.IsDungeonBoss(boss)){
    if(!string.IsNullOrEmpty(Dungeon.id)&&dungeonPostcards.TryGetValue(Dungeon.id,out var card)&&!string.IsNullOrEmpty(card.boss))
     return DungeonBossLabel(card.boss);
    if(boss.kind=="benders")return "BENDERS";
    if(!string.IsNullOrEmpty(Dungeon.boss))return DungeonBossLabel(Dungeon.boss);
   }
   return boss.kind.Replace('-', ' ').Replace('_',' ').ToUpperInvariant();
  }
  void CancelBossIntroduction(){
   bossIntroductionRemaining=0;bossIntroductionKind="";bossIntroductionRoom=-1;bossIntroductionReadyCapture=false;
   if(bossIntroductionRoot)bossIntroductionRoot.gameObject.SetActive(false);
  }
  bool TickBossIntroduction(){
   if(!game.SimpleDangerIntroductions)game.EnableSimpleDangerIntroductions();
   if(!BossIntroductionActive&&adventureStarted&&!game.paused&&string.IsNullOrEmpty(openMenu))BeginBossIntroduction();
   if(!BossIntroductionActive)return false;
   if(game.room!=bossIntroductionRoom||!adventureStarted||game.victory||game.player.hp<=0||!string.IsNullOrEmpty(openMenu)){CancelBossIntroduction();return false;}
   if(!game.paused){
    bossIntroductionRemaining=Mathf.Max(0,bossIntroductionRemaining-Time.unscaledDeltaTime);
    if(!BossIntroductionActive){bossIntroductionReadyCapture=bossIntroductionCapture;ClearInput();Debug.Log("BOSS_INTRO_COMPLETE room="+game.room+" actor="+bossIntroductionKind+" modelTime="+game.elapsed);}
   }
   return BossIntroductionActive;
  }
  // The 2016 DLG_BossState gauge uses an eye-strip, bevelled colour layers and oblique ticks.
  // Each actor owns its current layer; the lower layer is already visible behind the damage edge.
  sealed class BossGauge {
   internal RectTransform root,portraitClip,fill;
   internal Image portrait,under,baseColor,shine,rim;
   internal Text name,layers,portraitFallback;
   internal readonly RectTransform[] ticks=new RectTransform[3];
   internal Enemy owner;
   internal MonsterPixelFramesV53 portraitLibrary;
   internal int portraitPage;
   internal float width,lastHealth=-1,lastMax=-1;
   internal int layoutCount,layoutIndex;
  }
  readonly List<Enemy> bossHudSubjects=new List<Enemy>();
  readonly List<BossGauge> bossHudRows=new List<BossGauge>();
  static readonly uint[] bossGaugeColors={0xdb342e,0xe47920,0xe3b52b,0x82bc31,0x31aa62,0x27a6c4,0x4877d9,0x9560cb};
  bool bossPauseMoved;

  BossGauge CreateBossGauge(){
   var row=new BossGauge();
   row.root=UIRect("Boss HP",safeRoot,new Rect());
   var bevel=Panel("Boss Gauge frame",new Rect(0,0,1,1),C(0x191823),row.root);
   var edge=bevel.rectTransform;edge.anchorMax=Vector2.one;edge.offsetMin=edge.offsetMax=Vector2.zero;
   var upper=Panel("Boss Gauge edge",new Rect(),C(0xb6a5a6),row.root).rectTransform;
   upper.anchorMin=Vector2.up;upper.anchorMax=Vector2.one;upper.offsetMin=new Vector2(1,-1);upper.offsetMax=new Vector2(-1,0);
   row.portraitClip=UIRect("Boss portrait crop",row.root,new Rect(2,2,38,20));
   row.portraitClip.gameObject.AddComponent<RectMask2D>();
   Panel("Boss Portrait backing",new Rect(0,0,38,20),C(0x4c3442),row.portraitClip);
   row.portraitFallback=Label("Boss portrait fallback","BOSS",new Rect(0,0,38,20),9,C(0xffb697),TextAnchor.MiddleCenter,row.portraitClip);
   row.portrait=Panel("Boss Portrait",new Rect(0,0,38,20),Color.white,row.portraitClip);
   row.name=Label("Boss name","",new Rect(),11,C(0xffead1),TextAnchor.MiddleLeft,row.root);
   row.name.resizeTextForBestFit=true;row.name.resizeTextMinSize=8;row.name.resizeTextMaxSize=11;
   row.name.horizontalOverflow=HorizontalWrapMode.Wrap;row.name.verticalOverflow=VerticalWrapMode.Truncate;
   row.under=Panel("Boss Gauge underlying layer",new Rect(),Color.white,row.root);
   row.fill=UIRect("Current health layer",row.root,new Rect());
   row.baseColor=Panel("Boss Gauge midtone",new Rect(),Color.white,row.fill);
   row.shine=Panel("Boss Gauge shine",new Rect(),Color.white,row.fill);
   row.rim=Panel("Boss Gauge bottom",new Rect(),Color.white,row.fill);
   foreach(var image in new[]{row.baseColor,row.shine,row.rim}){
    var bar=image.rectTransform;bar.anchorMax=Vector2.one;bar.offsetMin=bar.offsetMax=Vector2.zero;
   }
   row.shine.rectTransform.anchorMin=new Vector2(0,1);row.shine.rectTransform.offsetMin=new Vector2(0,-2);
   row.rim.rectTransform.anchorMax=new Vector2(1,0);row.rim.rectTransform.offsetMax=new Vector2(0,2);
   for(int i=0;i<row.ticks.Length;i++){
    var tick=Panel("Boss Gauge slash",new Rect(0,1,1,7),C(0x261b2f,.6f),row.root).rectTransform;
    tick.localRotation=Quaternion.Euler(0,0,-26);row.ticks[i]=tick;
   }
   row.layers=Label("Boss HP layers","",new Rect(),11,C(0xffe8ac),TextAnchor.MiddleRight,row.root);
   bossHudRows.Add(row);
   return row;
  }
  string BossGaugeName(Enemy boss){
   int copies=0,index=0;
   foreach(var actor in game.enemies){
    if(actor.encounterIndex!=boss.encounterIndex||actor.kind!=boss.kind||!game.IsBossIntroductionActor(actor))continue;
    copies++;if(actor==boss)index=copies;
   }
   string title=(game.IsDungeonBoss(boss)?"":"ELITE / ")+BossIntroductionDisplayName(boss);
   return copies>1?title+" "+index:title;
  }
  void BindBossGauge(BossGauge row,Enemy actor){
   row.owner=actor;row.lastHealth=-1;row.name.text=BossGaugeName(actor);
   Sprite portrait=null;row.portraitLibrary=null;
   bool ancient=actor.sourceNPC==AncientRigV35.Npc&&actor.kind==AncientRigV35.Kind;
   TryMonsterPixelPortraitV53(actor,out portrait,out row.portraitLibrary,out row.portraitPage);
   if(!portrait&&!ancient&&libraries.TryGetValue(actor.kind,out var available)&&available.frames.TryGetValue("idle",out var idle)&&idle.Length>0)portrait=idle[0];
   if(!portrait&&!ancient){
    int index=game.enemies.IndexOf(actor);
    if(index>=0&&index<enemyViews.Count)portrait=enemyViews[index].sprite;
   }
   row.portrait.enabled=portrait;row.portrait.sprite=portrait;
   row.portraitFallback.text=game.IsDungeonBoss(actor)?"BOSS":"ELITE";
   row.portraitFallback.gameObject.SetActive(!portrait);
   if(portrait){
    float scale=Mathf.Max(54/portrait.rect.width,42/portrait.rect.height);
    var size=portrait.rect.size*scale;row.portrait.rectTransform.sizeDelta=size;
    row.portrait.rectTransform.anchoredPosition=new Vector2((38-size.x)*.5f,Mathf.Clamp(10-size.y*.64f,20-size.y,0));
   }
  }
  void LayoutBossGauge(BossGauge row,int index,int count){
   if(row.layoutCount==count&&row.layoutIndex==index&&row.lastMax==row.owner.maxHp)return;
   row.layoutCount=count;row.layoutIndex=index;
   int lines=Mathf.Min(2,count),columns=(count+lines-1)/lines;
   float slotWidth=Mathf.Floor(322f/columns);
   int height=count==1?35:23, barHeight=count==1?11:7;
   int nameSize=count==1?15:11;
   row.root.anchoredPosition=new Vector2(410+(index/lines)*slotWidth, count==1?490:512-(index%lines)*25);
   row.root.sizeDelta=new Vector2(slotWidth-2,height);
   row.portraitClip.anchoredPosition=new Vector2(2,(height-20)*.5f);
   row.name.fontSize=nameSize;row.name.resizeTextMaxSize=nameSize;row.layers.fontSize=nameSize;
   string maximumLayers="x"+Mathf.Max(1,Mathf.CeilToInt(row.owner.maxHp/1000));
   var settings=row.layers.GetGenerationSettings(Vector2.zero);
   float textWidth=row.layers.cachedTextGeneratorForLayout.GetPreferredWidth(maximumLayers,settings)/row.layers.pixelsPerUnit;
   float countWidth=Mathf.Max(maximumLayers.Length>=4?52:34,Mathf.Ceil(textWidth)+6);
   float countX=slotWidth-6-countWidth;
   float width=countX-43-3;
   row.name.rectTransform.anchoredPosition=new Vector2(43,barHeight+2);row.name.rectTransform.sizeDelta=new Vector2(slotWidth-47,height-barHeight-2);
   row.layers.rectTransform.anchoredPosition=new Vector2(countX,0);row.layers.rectTransform.sizeDelta=new Vector2(countWidth,height==35?21:13);
   row.under.rectTransform.anchoredPosition=new Vector2(43,2);row.under.rectTransform.sizeDelta=new Vector2(width,barHeight);
   row.fill.anchoredPosition=new Vector2(43,2);
   for(int i=0;i<row.ticks.Length;i++){
    row.ticks[i].anchoredPosition=new Vector2(43+Mathf.Floor(width*(i+1)/4),2);
    row.ticks[i].sizeDelta=new Vector2(1,barHeight);
   }
   row.width=width;row.lastHealth=-1;
  }
  void RenderBossGauges(){
   bossHudSubjects.Clear();
   if(adventureStarted&&!settingsOpen&&!game.paused&&!game.victory&&game.player.hp>0&&string.IsNullOrEmpty(openMenu)&&!BossIntroductionActive){
    foreach(var actor in game.enemies)
     if(actor.active&&!actor.dormant&&actor.encounterIndex==game.EncounterIndex&&actor.hp>0&&
        (game.IsDungeonBoss(actor)||dangerShownActors.Contains(actor)&&game.IsBossIntroductionActor(actor)))bossHudSubjects.Add(actor);
   }
   bool showing=bossHudSubjects.Count>0;
   if(showing||bossPauseMoved){
    float x=showing?304:407;
    float pauseWidth=showing?101:146;
    if(images["Pause Button"].rectTransform.sizeDelta.x!=pauseWidth)
     PlaceMenuItem(images["Pause Button"].transform,new Rect(x,483,pauseWidth,48));
    var track=images["Progress Track"].rectTransform;
    track.anchoredPosition=images["Progress Fill"].rectTransform.anchoredPosition=new Vector2(x+12+(showing?0:4),480);
    track.sizeDelta=new Vector2(showing?77:114,3);
    Width("Progress Fill",track.sizeDelta.x*game.player.x/game.RoomWidth);
    bossPauseMoved=showing;
   }
   for(int i=0;i<bossHudSubjects.Count;i++){
    var actor=bossHudSubjects[i];var row=i==bossHudRows.Count?CreateBossGauge():bossHudRows[i];
    if(!row.root.gameObject.activeSelf)row.root.gameObject.SetActive(true);
    if(row.owner!=actor)BindBossGauge(row,actor);
    LayoutBossGauge(row,i,bossHudSubjects.Count);
    if(row.lastHealth==actor.hp&&row.lastMax==actor.maxHp)continue;
    row.lastHealth=actor.hp;row.lastMax=actor.maxHp;
    int total=Mathf.Max(1,Mathf.CeilToInt(actor.maxHp/1000));
    float value=Mathf.Clamp01(actor.hp/Mathf.Max(1,actor.maxHp))*total;
    int layer=Mathf.Clamp(Mathf.CeilToInt(value),1,total);
    Color color=C(bossGaugeColors[(layer-1)%bossGaugeColors.Length]);
    row.baseColor.color=color;row.shine.color=Color.Lerp(color,Color.white,.62f);row.rim.color=Color.Lerp(color,Color.black,.32f);
    row.under.color=layer>1?C(bossGaugeColors[(layer-2)%bossGaugeColors.Length]):C(0x463b45);
    row.fill.sizeDelta=new Vector2(Mathf.Clamp(Mathf.CeilToInt(row.width*(value-layer+1)),1,row.width),row.under.rectTransform.sizeDelta.y);
    row.layers.text="x"+layer;
   }
   for(int i=bossHudSubjects.Count;i<bossHudRows.Count;i++){
    var row=bossHudRows[i];if(row.owner==null)continue;
    row.owner=null;row.portraitLibrary=null;row.portrait.sprite=null;row.root.gameObject.SetActive(false);
   }
  }

  void RenderBossIntroduction(){
   RenderBossGauges();
   if(!bossIntroductionRoot)return;
   bool visible=BossIntroductionActive&&!game.paused&&string.IsNullOrEmpty(openMenu);bossIntroductionRoot.gameObject.SetActive(visible);
   if(visible){
    float age=BossIntroductionDuration-bossIntroductionRemaining;
    bossIntroductionFade.alpha=Mathf.Min(Mathf.Clamp01(age/.12f),Mathf.Clamp01(bossIntroductionRemaining/.2f));
    float enter=1-Mathf.Pow(1-Mathf.Clamp01(age/.25f),3);
    bossIntroductionBand.anchoredPosition=new Vector2(Mathf.Round((1-enter)*-150),0);
    bossIntroductionPortraitRoot.anchoredPosition=new Vector2(560+Mathf.Round((1-enter)*80),120);
    // Capture only after applying the visible banner pose. A cold room load
    // can cross the capture threshold before its first rendered fade-in frame.
    if(bossIntroductionCapture&&!bossIntroductionCaptured&&age>.7f){bossIntroductionCaptured=true;Capture("12-boss-danger");}
   }
  }
  void CaptureBossIntroductionReady(){
   if(!bossIntroductionReadyCapture)return;bossIntroductionReadyCapture=false;Capture("13-boss-ready");
  }
 }
}
