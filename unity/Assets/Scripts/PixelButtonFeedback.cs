using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace ElswordPixel {
 public sealed class PixelButtonFeedback:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler {
  Image view;
  void Awake(){var rim=transform.Find("Pixel Inlay");if(rim)view=rim.GetComponent<Image>();}
  public void OnPointerEnter(PointerEventData e){if(view)view.color=new Color(1,.94f,.78f);}
  public void OnPointerExit(PointerEventData e){if(view)view.color=Color.white;}
  public void OnPointerDown(PointerEventData e){if(view)view.color=new Color(.62f,.77f,.92f);}
  public void OnPointerUp(PointerEventData e){if(view)view.color=Color.white;}
 }
}
