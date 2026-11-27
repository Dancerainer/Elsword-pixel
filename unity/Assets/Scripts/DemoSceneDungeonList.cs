using System;
using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  const float DungeonRowHeight=85, DungeonRowPitch=92, DungeonListWidth=466;
  int revealDungeonRow=-1;
  ScrollRect dungeonList;
  RectTransform dungeonRail,dungeonThumb,dungeonRailBefore,dungeonRailAfter;

  void BuildDungeonList(Transform parent) {
   var host=UIRect("Dungeon List",parent,new Rect(24,65,481,348));
   var viewport=UIRect("Viewport",host,new Rect(0,0,DungeonListWidth,348));
   viewport.gameObject.AddComponent<RectMask2D>();
   var background=viewport.gameObject.AddComponent<Image>();background.color=Color.clear;
   var content=UIRect("Rows",viewport,new Rect(0,0,DungeonListWidth,348));
   content.anchorMin=content.anchorMax=content.pivot=new Vector2(0,1);
   var scroll=host.gameObject.AddComponent<ScrollRect>();
   scroll.horizontal=false;scroll.vertical=true;scroll.movementType=ScrollRect.MovementType.Clamped;
   scroll.viewport=viewport;scroll.content=content;scroll.scrollSensitivity=39;scroll.decelerationRate=.09f;
   var rail=Panel("Dungeon Scroll Track",new Rect(471,0,7,348),C(0x25394d),host);
   Panel("Dungeon Scroll Thumb",new Rect(0,0,7,40),C(0x7794a9),rail.transform);
   BuildDungeonRailButton(rail.transform,"Dungeon Prev","dungeon-page:-1");
   BuildDungeonRailButton(rail.transform,"Dungeon Next","dungeon-page:1");
  }
  void BuildDungeonRailButton(Transform rail,string name,string action) {
   var area=UIRect(name,rail,new Rect(-3,0,13,0));
   var image=area.gameObject.AddComponent<Image>();image.color=Color.clear;
   var touch=area.gameObject.AddComponent<TouchControl>();touch.action=action;touch.owner=this;touch.tapOnRelease=true;
  }
  void AddDungeonListRow(int slot) {
   var content=dungeonList.content;
   string id="Dungeon Card "+slot;
   Button(id,"",new Rect(0,0,DungeonListWidth,DungeonRowHeight),"dungeon-row:"+slot,12,content);
   var row=(RectTransform)content.Find(id);
   row.anchorMin=row.anchorMax=new Vector2(0,1);
   row.anchoredPosition=new Vector2(0,-slot*DungeonRowPitch-DungeonRowHeight);
   row.GetComponent<TouchControl>().tapOnRelease=true;
   var thumbnail=Panel("Dungeon Preview "+slot,new Rect(6,6,162,73),Color.white,row);thumbnail.preserveAspect=true;
   var title=WrappedLabel("Dungeon Name "+slot,"",new Rect(183,39,271,40),17,C(0xf6f6ec),row,TextAnchor.MiddleLeft);
   title.resizeTextForBestFit=true;title.resizeTextMinSize=12;title.resizeTextMaxSize=17;
   var boss=WrappedLabel("Dungeon List Boss "+slot,"",new Rect(184,23,269,20),11,C(0xbbccdc),row,TextAnchor.MiddleLeft);
   boss.resizeTextForBestFit=true;boss.resizeTextMinSize=9;boss.resizeTextMaxSize=11;
   Label("Dungeon Best "+slot,"",new Rect(184,4,269,18),11,C(0xffd384),TextAnchor.MiddleLeft,row);
   Panel("Card Selection Accent "+slot,new Rect(0,3,4,DungeonRowHeight-6),C(0xffd079),row);
   foreach(var image in row.GetComponentsInChildren<Image>(true))images[image.name]=image;
   foreach(var label in row.GetComponentsInChildren<Text>(true))labels[label.name]=label;
  }
  void RenderDungeonList(Transform root) {
   if(!dungeonList) {
    dungeonList=root.Find("Dungeon List").GetComponent<ScrollRect>();
    dungeonRail=(RectTransform)dungeonList.transform.Find("Dungeon Scroll Track");
    dungeonThumb=(RectTransform)dungeonRail.Find("Dungeon Scroll Thumb");
    dungeonRailBefore=(RectTransform)dungeonRail.Find("Dungeon Prev");
    dungeonRailAfter=(RectTransform)dungeonRail.Find("Dungeon Next");
    dungeonList.onValueChanged.AddListener(_=>UpdateDungeonScrollRail());
   }
   int count=mapVisible.Count;
   while(dungeonList.content.childCount<count)AddDungeonListRow(dungeonList.content.childCount);
   float view=dungeonList.viewport.rect.height;
   dungeonList.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,Math.Max(view,count*DungeonRowPitch-(DungeonRowPitch-DungeonRowHeight)));
   for(int slot=0;slot<dungeonList.content.childCount;slot++){
    string id="Dungeon Card "+slot;Visible(id,slot<count);if(slot>=count)continue;
    var row=images[id];var route=mapDungeons[mapVisible[slot]];var card=GetDungeonPostcard(route);
    bool selected=mapSelected==mapVisible[slot];
    row.color=selected?C(0x395671):C(0x253c52);
    // The existing product and external QA actions refer to six-slot groups.
    var control=row.GetComponent<TouchControl>();control.tapOnRelease=true;
    control.action=slot/MapPageSize==mapPage?"dungeon-card:"+(slot%MapPageSize):"dungeon-row:"+slot;
    var thumb=images["Dungeon Preview "+slot];thumb.sprite=card?.sprite;thumb.enabled=thumb.sprite!=null;
    Visible("Card Selection Accent "+slot,selected);
    TextValue("Dungeon Name "+slot,route.title);TextValue("Dungeon List Boss "+slot,card?.boss??"");
    TextValue("Dungeon Best "+slot,DungeonBestLabel(route));
   }
   if(revealDungeonRow>=0) {
    float top=revealDungeonRow*DungeonRowPitch,offset=dungeonList.content.anchoredPosition.y;
    ScrollDungeonListTo(Mathf.Clamp(offset,top+DungeonRowHeight-view,top));
    revealDungeonRow=-1;
   }
   UpdateDungeonScrollRail();
  }
  void ScrollDungeonListTo(float offset) {
   dungeonList.StopMovement();
   float limit=Math.Max(0,dungeonList.content.rect.height-dungeonList.viewport.rect.height);
   dungeonList.content.anchoredPosition=new Vector2(0,Mathf.Clamp(offset,0,limit));
   UpdateDungeonScrollRail();
  }
  void UpdateDungeonScrollRail() {
   float view=dungeonList.viewport.rect.height,extent=dungeonList.content.rect.height;
   bool scrolls=extent>view+.5f;
   if(dungeonRail.gameObject.activeSelf!=scrolls)dungeonRail.gameObject.SetActive(scrolls);
   if(!scrolls)return;
   float h=view*view/extent;
   float y=(view-h)*(1-Mathf.Clamp01(dungeonList.content.anchoredPosition.y/(extent-view)));
   SetDungeonRailSpan(dungeonThumb,y,h);
   SetDungeonRailSpan(dungeonRailBefore,y+h,view-y-h);
   SetDungeonRailSpan(dungeonRailAfter,0,y);
  }
  static void SetDungeonRailSpan(RectTransform rect,float y,float height) {
   rect.anchoredPosition=new Vector2(rect.anchoredPosition.x,y);
   rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,Math.Max(0,height));
  }
  void SelectDungeonListRow(int slot) {
   if(slot<0||slot>=mapVisible.Count)return;
   StopMapMusicPreview();
   mapSelected=mapVisible[slot];mapPage=slot/MapPageSize;revealDungeonRow=slot;
   int route=mapDungeons[mapSelected].runtimeIndex;
   if(route>=0&&route<catalog.dungeons.Length)previewDungeon=route;
   RenderMenus();
  }
  bool DungeonListNavigation(KeyCode key) {
   if(openMenu!="dungeons")return false;
   if(key==KeyCode.Return||key==KeyCode.KeypadEnter){Action("dungeon-enter");return true;}
   if(key==KeyCode.LeftArrow||key==KeyCode.RightArrow) {
    int region=Array.IndexOf(MapRegions,mapRegion)+(key==KeyCode.LeftArrow?-1:1);
    Action("region:"+MapRegions[(region+MapRegions.Length)%MapRegions.Length]);return true;
   }
   RefreshMapVisible();int slot=mapVisible.IndexOf(mapSelected);
   int page=dungeonList?Math.Max(1,(int)(dungeonList.viewport.rect.height/DungeonRowPitch)):1;
   switch(key) {
    case KeyCode.UpArrow:slot--;break;
    case KeyCode.DownArrow:slot++;break;
    case KeyCode.PageUp:slot-=page;break;
    case KeyCode.PageDown:slot+=page;break;
    case KeyCode.Home:slot=0;break;
    case KeyCode.End:slot=mapVisible.Count-1;break;
    default:return false;
   }
   if(mapVisible.Count>0){
    int nextSlot=Mathf.Clamp(slot,0,mapVisible.Count-1);
    if(mapVisible[nextSlot]!=mapSelected)PlayMenuButtonSound();
    SelectDungeonListRow(nextSlot);
   }
   return true;
  }
 }
}
