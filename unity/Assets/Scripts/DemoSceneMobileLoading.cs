using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        bool startupStarted, mobileStartupPending;
        int mobileLoadingWindowCount;
        sealed class MobileLoadingWindow {
            internal GameObject screen;
            internal Text status;
        }

        IEnumerator StartMobileScene(){
            mobileStartupPending=true;
            var loading=ShowMobileLoadingWindow("LOADING GAME RESOURCES");
            // A coroutine started in Awake can resume before its first render.
            // Two player-loop yields let a complete frame draw and present this
            // overlay before the synchronous catalog import starts.
            yield return null;
            yield return null;
            Debug.Log("Android startup: loading game resources.");
            try{
                InitializeScene();
            }catch(Exception error){
                SetMobileLoadingMessage(loading,"RESOURCE LOAD FAILED\nPLEASE RESTART THE GAME");
                Debug.LogException(error,this);
                yield break;
            }
            mobileStartupPending=false;
            CloseMobileLoadingWindow(loading);
        }

        MobileLoadingWindow ShowMobileLoadingWindow(string message){
            var screen=new GameObject("Loading Screen",typeof(RectTransform),typeof(Canvas),
                typeof(CanvasScaler),typeof(GraphicRaycaster));
            screen.transform.SetParent(transform,false);
            var canvas=screen.GetComponent<Canvas>();
            canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder=short.MaxValue;
            var scaler=screen.GetComponent<CanvasScaler>();
            scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution=new Vector2(960,540);
            scaler.matchWidthOrHeight=.5f;

            // The shipped scene already references Silkscreen. Drawing the first
            // window must not synchronously open the gameplay Resources catalog.
            var nameLabel=safeRoot?safeRoot.Find("Name")?.GetComponent<Text>():null;
            if(nameLabel)pixelFont=nameLabel.font;
            if(!pixelFont)pixelFont=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var shade=Panel("Loading Shade",new Rect(),C(0x142436),screen.transform);
            shade.raycastTarget=true;
            var full=shade.rectTransform;
            full.anchorMin=Vector2.zero;full.anchorMax=Vector2.one;
            full.offsetMin=full.offsetMax=Vector2.zero;
            var window=Panel("Loading Window",new Rect(0,0,410,132),C(0x263d54),full);
            window.rectTransform.anchorMin=window.rectTransform.anchorMax=Vector2.one*.5f;
            window.rectTransform.anchoredPosition=new Vector2(-205,-66);
            Panel("Loading Header",new Rect(5,85,400,42),C(0xc6dced),window.transform);
            HeaderInk(Label("Loading Title","ELSWORD PIXEL",new Rect(18,87,370,38),21,
                Color.white,TextAnchor.MiddleLeft,window.transform));
            DrawLoadingCrystal(window.transform);
            var status=Label("Loading Message","",new Rect(70,12,328,64),15,
                C(0xe7edf3),TextAnchor.MiddleLeft,window.transform);
            status.horizontalOverflow=HorizontalWrapMode.Wrap;
            status.verticalOverflow=VerticalWrapMode.Truncate;
            var loading=new MobileLoadingWindow{screen=screen,status=status};
            SetMobileLoadingMessage(loading,message);
            mobileLoadingWindowCount++;
            Canvas.ForceUpdateCanvases();
            return loading;
        }

        void SetMobileLoadingMessage(MobileLoadingWindow loading,string message){
            loading.status.font=pixelFont;
            loading.status.fontSize=15;
            loading.status.text=message;
        }

        void CloseMobileLoadingWindow(MobileLoadingWindow loading){
            if(loading==null||!loading.screen)return;
            RemoveSceneObject(loading.screen);
            loading.screen=null;
            mobileLoadingWindowCount--;
        }

        void DrawLoadingCrystal(Transform window){
            int[] widths={4,10,16,22,26,22,16,10,4};
            for(int row=0;row<widths.Length;row++){
                int width=widths[row],x=38-width/2,y=21+row*5;
                Panel("Crystal Shadow",new Rect(x,y,width,5),C(0xb53552),window);
                Panel("Crystal Light",new Rect(x,y,width/2,5),C(row<4?0xe65861u:0xff9b82u),window);
            }
        }
    }
}
