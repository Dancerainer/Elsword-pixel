#if UNITY_EDITOR || ROOT_REDRAW_CHECK_V32
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  bool checkedDungeonListArguments;
  [Serializable] sealed class DungeonListCheckResult {
   public List<string> regions=new List<string>();
   public int routes,touchRoutes;public float dragStart,dragFirst,dragSecond,inertiaTravel,wheelOffset;
   public bool tapAfterRelease,dragKeptSelection,clippedRaycasts,keyboardFocus,legacyActions,entered,returned;
  }
  void StartDungeonListCheckIfRequested() {
   if(checkedDungeonListArguments)return;checkedDungeonListArguments=true;
   var args=Environment.GetCommandLineArgs();int flag=Array.IndexOf(args,"--check-dungeon-list");
   if(flag<0||flag+1>=args.Length||!Application.isPlaying)return;
   Application.runInBackground=true;
   StartCoroutine(GuardPeitaCheck(CheckDungeonList(Path.GetFullPath(args[flag+1]))));
  }
  static void NeedDungeonList(bool condition,string message) {if(!condition)throw new InvalidOperationException(message);}
  static Vector2 DungeonScreenPoint(RectTransform rect,Vector2 local) {
   return RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(local));
  }
  TouchControl RaycastDungeonControl(PointerEventData pointer) {
   var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
   return hits.Count>0?hits[0].gameObject.GetComponentInParent<TouchControl>():null;
  }
  IEnumerator CheckDungeonList(string directory) {
   Directory.CreateDirectory(directory);yield return null;
   var result=new DungeonListCheckResult();
   foreach(string region in MapRegions) {
    MenuAction("region:"+region);
    NeedDungeonList(mapVisible.Count>0&&Mathf.Abs(dungeonList.content.anchoredPosition.y)<.1f,"Region start "+region);
    for(int i=0;i<mapVisible.Count;i++){
     if(i>0)DungeonListNavigation(KeyCode.DownArrow);
     var row=(RectTransform)dungeonList.content.GetChild(i);
     float top=i*DungeonRowPitch-dungeonList.content.anchoredPosition.y;
     var c=row.GetComponent<TouchControl>();
     NeedDungeonList(mapSelected==mapVisible[i]&&top>=-.5f&&top+DungeonRowHeight<=dungeonList.viewport.rect.height+.5f&&c.isActiveAndEnabled,"Focused row "+region+"/"+i);
     Canvas.ForceUpdateCanvases();
     // Unity publishes clipping and raycast depth for newly visible rows on its render loop.
     yield return null;
     var touch=new PointerEventData(EventSystem.current){pointerId=43,button=PointerEventData.InputButton.Left,position=DungeonScreenPoint(row,row.rect.center)};
     NeedDungeonList(RaycastDungeonControl(touch)==c,"Touch hitbox "+region+"/"+i);
     ExecuteEvents.Execute(c.gameObject,touch,ExecuteEvents.pointerDownHandler);
     ExecuteEvents.Execute(c.gameObject,touch,ExecuteEvents.pointerUpHandler);
     NeedDungeonList(mapSelected==mapVisible[i],"Touch selected "+region+"/"+i);result.touchRoutes++;
     result.routes++;
    }
    result.regions.Add(region);
   }
   MenuAction("region:hamel");yield return null;
   NeedDungeonList(mapVisible.Count>=7,"Long region required");
   var view=dungeonList.viewport;
   CaptureOffscreenFrame(Path.Combine(directory,"list-top.png"));
   var pointer=new PointerEventData(EventSystem.current){pointerId=44,button=PointerEventData.InputButton.Left};
   pointer.position=DungeonScreenPoint(view,new Vector2(100,view.rect.height-DungeonRowPitch-34));
   var card=RaycastDungeonControl(pointer);NeedDungeonList(card&&card.gameObject==dungeonList.content.GetChild(1).gameObject,"Touch raycast on second row");
   ExecuteEvents.Execute(card.gameObject,pointer,ExecuteEvents.pointerDownHandler);
   NeedDungeonList(mapSelected==mapVisible[0],"Card must wait for release");
   ExecuteEvents.Execute(card.gameObject,pointer,ExecuteEvents.pointerUpHandler);
   result.tapAfterRelease=mapSelected==mapVisible[1];
   NeedDungeonList(result.tapAfterRelease,"Tap selection");
   int selected=mapSelected;result.dragStart=dungeonList.content.anchoredPosition.y;
   var scroll=dungeonList.gameObject;
   pointer.pressPosition=pointer.position;
   ExecuteEvents.Execute(card.gameObject,pointer,ExecuteEvents.pointerDownHandler);
   ExecuteEvents.Execute(scroll,pointer,ExecuteEvents.initializePotentialDrag);
   ExecuteEvents.Execute(scroll,pointer,ExecuteEvents.beginDragHandler);pointer.dragging=true;
   ExecuteEvents.Execute(card.gameObject,pointer,ExecuteEvents.pointerUpHandler);
   Vector2 start=pointer.position;
   float scale=(DungeonScreenPoint(view,new Vector2(0,100))-DungeonScreenPoint(view,Vector2.zero)).magnitude/100f;
   pointer.position=start+Vector2.up*42*scale;ExecuteEvents.Execute(scroll,pointer,ExecuteEvents.dragHandler);yield return null;
   result.dragFirst=dungeonList.content.anchoredPosition.y;
   pointer.position=start+Vector2.up*103*scale;ExecuteEvents.Execute(scroll,pointer,ExecuteEvents.dragHandler);yield return null;
   result.dragSecond=dungeonList.content.anchoredPosition.y;
   ExecuteEvents.Execute(scroll,pointer,ExecuteEvents.endDragHandler);pointer.dragging=false;
   yield return new WaitForSecondsRealtime(.12f);
   result.inertiaTravel=dungeonList.content.anchoredPosition.y-result.dragSecond;
   result.dragKeptSelection=mapSelected==selected&&Math.Abs(result.dragFirst-42)<2&&Math.Abs(result.dragSecond-103)<2;
   NeedDungeonList(result.dragKeptSelection&&result.inertiaTravel>=0,"Continuous drag / selection");
   CaptureOffscreenFrame(Path.Combine(directory,"list-scrolled.png"));
   DungeonListNavigation(KeyCode.Home);pointer.scrollDelta=new Vector2(0,-1);
   ExecuteEvents.Execute(scroll,pointer,ExecuteEvents.scrollHandler);
   result.wheelOffset=dungeonList.content.anchoredPosition.y;NeedDungeonList(result.wheelOffset>0,"Wheel scroll");
   DungeonListNavigation(KeyCode.End);yield return null;
   pointer.position=DungeonScreenPoint(view,new Vector2(100,-2));
   var outside=RaycastDungeonControl(pointer);
   result.clippedRaycasts=!outside||!outside.transform.IsChildOf(dungeonList.content);
   NeedDungeonList(result.clippedRaycasts,"Clipped rows must not take touches");
   result.keyboardFocus=mapSelected==mapVisible[mapVisible.Count-1]&&dungeonList.content.anchoredPosition.y>0;
   NeedDungeonList(result.keyboardFocus,"Last row reachable");
   MenuAction("region:hamel");MenuAction("dungeon-page:1");MenuAction("dungeon-card:0");
   result.legacyActions=mapSelected==mapVisible[6];NeedDungeonList(result.legacyActions,"Existing six-slot action");
   string id=mapDungeons[mapSelected].id;DungeonListNavigation(KeyCode.Return);yield return null;
   result.entered=openMenu==""&&Dungeon.id==id;NeedDungeonList(result.entered,"Enter last dungeon");
   MenuAction("dungeons");yield return null;
   result.returned=openMenu=="dungeons"&&mapDungeons[mapSelected].id==id;
   NeedDungeonList(result.returned,"Return focus");
   File.WriteAllText(Path.Combine(directory,"dungeon-list.json"),JsonUtility.ToJson(result,true));
   ExitPeitaCheck(0);
  }
 }
}
#endif
