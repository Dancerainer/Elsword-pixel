using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  Sprite pixelFrame, pixelSurface; Font pixelFont;
  const int WindowTileSize=32, WindowTileInset=10;

  Sprite WindowTile(Texture2D texture,int x){
   return Sprite.Create(texture,new Rect(x,0,WindowTileSize,WindowTileSize),Vector2.one*.5f,
    100,0,SpriteMeshType.FullRect,Vector4.one*WindowTileInset);
  }
  void MakeWindowTiles(){
   var fill=new Color32[WindowTileSize*WindowTileSize];
   var rim=new Color32[fill.Length];
   for(int y=0;y<WindowTileSize;y++)for(int x=0;x<WindowTileSize;x++){
    // A shared nine-slice gives a two-step glass lip and a fixed pixel radius,
    // including on narrow HUD buttons and full-width dialogs.
    float dx=Mathf.Max(8.5f-x,x-(WindowTileSize-9.5f),0);
    float dy=Mathf.Max(8.5f-y,y-(WindowTileSize-9.5f),0);
    float inset=9-Mathf.Sqrt(dx*dx+dy*dy);
    inset=Mathf.Min(inset,x+.5f,y+.5f,WindowTileSize-x-.5f,WindowTileSize-y-.5f);
    if(inset<=0)continue;
    int at=y*WindowTileSize+x;
    fill[at]=new Color32(255,255,255,255);
    bool light=y>=WindowTileSize/2;
    if(inset<1)rim[at]=new Color32(13,24,42,255);
    else if(inset<2)rim[at]=light?new Color32(191,220,238,255):new Color32(91,119,151,255);
    else if(inset<3)rim[at]=light?new Color32(112,146,176,255):new Color32(49,68,95,255);
    else if(inset<4)rim[at]=new Color32(224,241,255,58);
    else if(y>=WindowTileSize-7)rim[at]=new Color32(232,246,255,23);
    else if(y<6)rim[at]=new Color32(8,16,34,38);
   }
   var texture=new Texture2D(WindowTileSize*2,WindowTileSize,TextureFormat.RGBA32,false){
    name="Window surface and edge",filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
   texture.SetPixels32(0,0,WindowTileSize,WindowTileSize,fill);
   texture.SetPixels32(WindowTileSize,0,WindowTileSize,WindowTileSize,rim);
   texture.Apply(false,true);
   pixelSurface=WindowTile(texture,0);pixelFrame=WindowTile(texture,WindowTileSize);
  }
  void PixelBorder(Image image) {
   if(!pixelSurface||!pixelFrame)MakeWindowTiles();
   image.sprite=pixelSurface;image.type=Image.Type.Sliced;
   var existing=image.transform.Find("Pixel Inlay");
   var border=existing?existing.GetComponent<Image>():null;
   if(!border){
    var g=new GameObject("Pixel Inlay",typeof(RectTransform),typeof(Image));
    var r=g.GetComponent<RectTransform>();r.SetParent(image.transform,false);
    r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
    border=g.GetComponent<Image>();
   }
   border.sprite=pixelFrame;border.type=Image.Type.Sliced;border.raycastTarget=false;
  }
  void RestoreWindowSkin(){
   // The shipping scene can contain an older serialized UI hierarchy. Rebuild
   // its menu before binding input, then restore runtime-created frame sprites.
   var oldMap=safeRoot.Find("Dungeon Root");
   var oldMenus=safeRoot.Find("Menu Root");
   if(!oldMap||!oldMap.Find("Dungeon List")||!oldMap.Find("Region Tab 6")||
      !oldMenus||!oldMenus.Find("Guide Content/Commands GROUND")){
    foreach(string name in new[]{"Dungeon Root","Menu Root"}){
     var old=safeRoot.Find(name);
     if(old){old.SetParent(null,false);RemoveSceneObject(old.gameObject);}
    }
    BuildMenus();
   }
   foreach(var im in safeRoot.GetComponentsInChildren<Image>(true))
    if(im.transform.Find("Pixel Inlay"))PixelBorder(im);
  }
  void HeaderInk(Text label){
   label.color=C(0x26435e);var shadow=label.GetComponent<Shadow>();
   if(shadow){shadow.effectColor=C(0xf7fcff,.85f);shadow.effectDistance=new Vector2(0,-1);}
  }
  void BuildPixelHud(){
   Panel("Dungeon Panel",new Rect(736,464,212,64),C(0x152330));Label("Zone","RUBEN",new Rect(748,509,186,13),9,C(0x9bd2bf));Label("Title","TREE OF EL",new Rect(748,483,186,25),15,C(0xffdfa0));Label("Mission","",new Rect(748,470,186,14),8,C(0xd3ded7));
   Button("Pause Button","AREA 1 / 3",new Rect(422,497,116,28),"pause",10);Panel("Progress Track",new Rect(423,491,114,3),C(0x273b45));Panel("Progress Fill",new Rect(423,491,1,3),C(0xb6dda0));
   Label("Command Input","",new Rect(340,459,280,25),13,C(0xffdc8c));Label("Link Window","",new Rect(320,440,320,17),9,C(0xa1d7be));Label("Skill Phase","",new Rect(320,422,320,16),9,C(0xe4d5af));
   Panel("Buff Panel",new Rect(84,426,204,23),C(0x203f3a));var buff=Panel("Buff Icon",new Rect(88,429,17,17),Color.white);buff.sprite=SkillIcon("iron-body");Label("Buff","",new Rect(110,429,174,17),9,C(0xa5f0bc),TextAnchor.MiddleLeft);Panel("Buff Timer",new Rect(84,426,204,2),C(0x8cecc8));
   Label("Combat State","",new Rect(84,405,220,18),9,C(0xffe49b),TextAnchor.MiddleLeft);
   Panel("Charge Track",new Rect(410,452,140,3),C(0x293f4c));Panel("Charge Fill",new Rect(410,452,1,3),C(0xffc671));
   Label("Combo","",new Rect(756,351,178,40),22,C(0xffe37d),TextAnchor.MiddleRight);Label("Gate Prompt","",new Rect(245,146,470,25),13,C(0xffe18f));
   Panel("Footer",new Rect(0,0,960,21),C(0x111c29));Label("Keyboard Hint","ARROWS MOVE  UP JUMP  Z/X ATTACK",new Rect(14,2,430,17),9,C(0xaec3c4),TextAnchor.MiddleLeft);Label("System Hint","K SKILLS  TAB MAP  CTRL AWAKEN  ESC PAUSE",new Rect(445,2,501,17),9,C(0xaec3c4),TextAnchor.MiddleRight);
   Panel("Skill Bar",new Rect(400,25,160,80),C(0x18232f));
   for(int i=0;i<8;i++){float x=426+i%4*36,y=i<4?72:36;Button("Skill "+i,"",new Rect(x-15,y,30,30),"skill"+i,9);var icon=Panel("Skill Icon "+i,new Rect(x-12,y+3,24,24),Color.white);icon.preserveAspect=true;Label("Skill Glyph "+i,"",new Rect(x-13,y+2,26,26),20,Color.white);Label("Skill Key "+i,SkillKeyLabels[i],new Rect(x-15,y+25,12,10),8,C(0xffdd95));Label("Skill Name "+i,"",new Rect(x-17,y-8,34,9),6,Color.white);Panel("Cooldown Shade "+i,new Rect(x-12,y+3,24,24),C(0x11212d,.78f));Label("Cooldown "+i,"",new Rect(x-12,y+3,24,24),12,Color.white);}
   string[] names={"Touch Left","Touch Right","Touch Dash","Touch Jump","Touch Attack","Touch Attack X"},texts={"<",">","DASH","JUMP","Z","X"},actions={"left","right","dash","jump","attack","attackX"};float[] xs={15,82,150,719,797,875};for(int i=0;i<6;i++)Button(names[i],texts[i],new Rect(xs[i],37,60,60),actions[i],14);
   Button("Touch Toggle","TOUCH",new Rect(889,427,59,24),"touch",8);Button("Loadout Button","SKILLS K",new Rect(562,29,82,24),"loadout",8);Button("Guide Button","COMBOS F1",new Rect(562,57,82,24),"guide",8);Button("Potion Button","POTION T",new Rect(316,29,80,24),"potion",8);Button("Dungeon Button","MAP TAB",new Rect(316,57,80,24),"dungeons",8);
   Panel("Banner Panel",new Rect(280,333,400,52),C(0x162b32,.92f));Label("Banner","",new Rect(288,355,384,25),17,C(0xffe5a0));Label("Banner Sub","",new Rect(288,339,384,16),9,C(0xb9d9ca));
   Button("Result Panel","",new Rect(280,211,400,119),"result");Label("Result Title","",new Rect(288,264,384,43),23,C(0xffe9b1));Label("Result Detail","",new Rect(288,222,384,37),11,C(0xd4e3d7));
  }
 }
}
