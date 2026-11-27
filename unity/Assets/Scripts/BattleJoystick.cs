using UnityEngine;
using UnityEngine.EventSystems;

namespace ElswordPixel {
    // The pointer which started on the pad owns it until lift, even outside its bounds.
    public sealed partial class BattleJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
        IDragHandler, IInitializePotentialDragHandler, ICanvasRaycastFilter {
        internal DemoScene owner;
        internal RectTransform thumb;
        int? pointer;
        readonly BattleStickGesture gesture=new BattleStickGesture();
        internal UnityEngine.UI.Image sprintRing;
        internal int Horizontal {get{return gesture.Horizontal;}}

        public void OnInitializePotentialDrag(PointerEventData data){data.useDragThreshold=false;}
        public void OnPointerDown(PointerEventData data){
            if(nativeInput||pointer.HasValue||!owner.CanMoveStick)return;
            pointer=data.pointerId;Read(data);
        }
        public void OnDrag(PointerEventData data){if(!nativeInput&&pointer==data.pointerId)Read(data);}
        public void OnPointerUp(PointerEventData data){if(!nativeInput&&pointer==data.pointerId)Release();}
        void OnDisable(){ResetGesture();}
        internal void ResetGesture(){
#if UNITY_ANDROID && !UNITY_EDITOR
            if(nativeInput&&nativeStick!=null)nativeStick.CallStatic("disarm");
            nativeInput=false;
#endif
            Release();
        }
        void Release(){
            if(gesture.Horizontal!=0&&owner)owner.StickDirectionChanged(0,false);
            pointer=null;gesture.Reset();
            if(thumb)thumb.anchoredPosition=Vector2.zero;
            RefreshSprintRing();
        }
        bool LocalPoint(Vector2 screen,Camera camera,out Vector2 normalized){
            var rect=(RectTransform)transform;
            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,screen,camera,out normalized))return false;
            normalized=(normalized-rect.rect.center)/(rect.rect.width*.5f);return true;
        }
        public bool IsRaycastLocationValid(Vector2 screen,Camera camera){
            return LocalPoint(screen,camera,out var point)&&point.sqrMagnitude<=1f;
        }
        void Read(PointerEventData data){
            if(!owner.CanMoveStick){ResetGesture();return;}
            if(LocalPoint(data.position,data.pressEventCamera,out var point))
                ApplyPoint(point);
        }
        internal void ReplayStickSamples(long[] samples){
            if(samples.Length>0&&owner)owner.MarkMobileTouchActivity();
            for(int i=0;i<samples.Length;i+=4){
                long kind=samples[i+1];
                if(kind==0)ApplyPoint(new Vector2(samples[i+2]/8192f,samples[i+3]/8192f));
                else Release();
            }
        }
        void ApplyPoint(Vector2 point){
            point=Vector2.ClampMagnitude(point,1f);
            thumb.anchoredPosition=point*((RectTransform)transform).rect.width*.40f;
            if(gesture.Apply(point.x,point.y,out int verticalPress)){
                owner.StickDirectionChanged(gesture.Horizontal,gesture.Dashing);RefreshSprintRing();
            }
            if(verticalPress!=0)owner.StickVerticalPressed(verticalPress);
        }
        void RefreshSprintRing(){
            if(sprintRing)sprintRing.color=gesture.Dashing?new Color(1,.63f,.29f,.95f):new Color(.65f,.78f,1,.70f);
        }
    }
}
