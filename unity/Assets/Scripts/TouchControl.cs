using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ElswordPixel {
    public sealed class TouchControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
        public string action;
        public DemoScene owner;
        internal bool pulseOnPress;
        [System.NonSerialized] public bool tapOnRelease;
        Image pulseImage;
        Color idleColor;
        readonly HashSet<int> activePointers=new HashSet<int>();
        public void OnPointerDown(PointerEventData e){
            if(!owner||!activePointers.Add(e.pointerId))return;
            if(tapOnRelease)return;
            if(pulseOnPress&&activePointers.Count==1){pulseImage=GetComponent<Image>();idleColor=pulseImage.color;pulseImage.color=idleColor*new Color(.75f,.79f,.91f,1f);}
            owner.TouchDown(e.pointerId,action);
        }
        void Release(int id){
            if(!activePointers.Remove(id))return;
            if(pulseImage&&activePointers.Count==0)pulseImage.color=idleColor;
            if(owner)owner.TouchUp(id,action);
        }
        public void OnPointerUp(PointerEventData e){
            if(tapOnRelease){
                if(activePointers.Remove(e.pointerId)&&owner&&!e.dragging){owner.TouchDown(e.pointerId,action);owner.TouchUp(e.pointerId,action);}
            }else Release(e.pointerId);
        }
        public void OnPointerExit(PointerEventData e){if(tapOnRelease)activePointers.Remove(e.pointerId);else Release(e.pointerId);}
        void OnApplicationFocus(bool focused){if(!focused)ReleasePointers();}
        void OnApplicationPause(bool paused){if(paused)ReleasePointers();}
        void OnDisable(){ReleasePointers();}
        void ReleasePointers(){
            if(owner&&!tapOnRelease)foreach(int id in activePointers)owner.TouchUp(id,action);
            activePointers.Clear();if(pulseImage)pulseImage.color=idleColor;
        }
    }
}
