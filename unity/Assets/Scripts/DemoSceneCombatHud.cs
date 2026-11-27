using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        const float VitalsBarLeft=116,VitalsBarWidth=180;
        const int ManaSegment=100;
        static readonly Color32[] ComboColors={
            new Color32(137,235,141,255),new Color32(255,169,73,255),new Color32(255,102,92,255)
        };
        static readonly string[] ComboRanks={"GOOD","GREAT","AMAZING"};
        const string ComboAlphabet="AGIMNODRTZE";
        // Five-pixel rows ordered from the top of each custom letter.
        static readonly ulong[] ComboGlyphs={
            0b01110_11011_11011_11111_11011_11011_11011UL, // A
            0b01111_11000_11000_11011_11011_11011_01111UL, // G
            0b11111_00100_00100_00100_00100_00100_11111UL, // I
            0b10001_11011_11111_10101_10001_10001_10001UL, // M
            0b10001_11001_11001_10101_10011_10011_10001UL, // N
            0b01110_11011_11011_11011_11011_11011_01110UL, // O
            0b11110_11011_11011_11011_11011_11011_11110UL, // D
            0b11110_11011_11011_11110_11100_11010_11011UL, // R
            0b11111_00100_00100_00100_00100_00100_00100UL, // T
            0b11111_00011_00110_01100_11000_11000_11111UL, // Z
            0b11111_11000_11000_11110_11000_11000_11111UL  // E
        };
        const int DigitCellWidth=16,DigitCellHeight=14;
        const float DigitWidth=136,DigitHeight=112,DigitAdvance=82,ComboDigitBudget=360;
        const float ComboDigitPulseWidth=.14f;
        const float AwakeningKeyClearance=60;
        static readonly ulong[] ComboDigitGlyphs={
            0b0111110_1111111_1100011_1100011_1100011_1100011_1100011_1111111_0111110UL, // 0
            0b0011100_0111100_1111100_0011100_0011100_0011100_0011100_0111110_1111111UL, // 1
            0b0111110_1111111_1100011_0000111_0011110_0111100_1110000_1111111_1111111UL, // 2
            0b1111110_1111111_0000011_0000111_0011110_0000111_0000011_1111111_1111110UL, // 3
            0b0001110_0011110_0110110_1100110_1100110_1111111_1111111_0000110_0000110UL, // 4
            0b1111111_1111111_1100000_1111110_1111111_0000011_0000011_1111111_1111110UL, // 5
            0b0011110_0111110_1110000_1111110_1111111_1100011_1100011_1111111_0111110UL, // 6
            0b1111111_1111111_0000111_0001110_0011100_0011100_0111000_0111000_0111000UL, // 7
            0b0111110_1111111_1100011_1111111_0111110_1111111_1100011_1111111_0111110UL, // 8
            0b0111110_1111111_1100011_1100011_1111111_0111111_0000011_0111110_0111100UL  // 9
        };
        readonly List<Sprite> vitalsArt=new List<Sprite>();
        readonly List<Texture2D> vitalsTextures=new List<Texture2D>();
        Sprite healthEnamel,manaEnamel,healthTrack,manaTrack,manaDivider,jewel;
        readonly Sprite[] comboRankArt=new Sprite[3],comboDigitArt=new Sprite[10];
        readonly Image[] comboDigitViews=new Image[10];
        Sprite comboSlashArt,comboSparksArt;
        RectTransform comboReadout,comboDigits;
        Image comboRank,comboSlash,comboSparks;
        int shownCombo=-1,shownMaxHealth=-1;
        int shownRank=-1;
        float comboPulseAt,comboRankAt,comboDigitScale=1;
        float comboDigitSpan,comboStatusEdge;

        void ConfigureCombatHud() {
            if(!healthEnamel){
                healthEnamel=MakeVitalsSprite("Amber health enamel",54,9,(x,y)=>
                    BarEnamel(x/53f,y,C(0xffbc45),C(0xe14f45)));
                manaEnamel=MakeVitalsSprite("Cyan mana enamel",54,9,(x,y)=>
                    BarEnamel(x/53f,y,C(0x67dded),C(0x3f82e1)));
                healthTrack=MakeVitalsSprite("Health trough",54,9,(x,y)=>
                    BarTrack(x/53f,y,C(0x542c39),C(0x3b212e)));
                manaTrack=MakeVitalsSprite("Mana trough",54,9,(x,y)=>
                    BarTrack(x/53f,y,C(0x233754),C(0x17243a)));
                manaDivider=MakeVitalsSprite("100 MP tick",2,9,(x,y)=>
                    y==0||y==8?Color.clear:x==0?C(0x101e37,.92f):C(0xc1f1ff,.82f));
                jewel=MakeVitalsSprite("Awakening setting",12,12,(x,y)=>{
                    int distance=Mathf.Abs(x*2-11)+Mathf.Abs(y*2-11);
                    if(distance>14)return Color.clear;
                    if(distance>11)return C(0x291e30);
                    if(distance>8)return C(0xe2a7ae);
                    if(x<5&&y>6)return C(0xffecea);
                    return y>=5?C(0xf0bac8):C(0xbd687f);
                });
            }

            // Older serialized scenes already contain these widgets, including the retired frame.
            foreach(string id in new[]{"Player Panel","Portrait Panel","Level","HP Shine"})Visible(id,false);
            var vitals=safeRoot.Find("Player Vitals");
            if(!vitals){
                vitals=UIRect("Player Vitals",safeRoot,new Rect(0,0,300,540));
                vitals.SetAsFirstSibling();
            }
            var hpRect=new Rect(VitalsBarLeft,484,VitalsBarWidth,17);
            var mpRect=new Rect(VitalsBarLeft,462,VitalsBarWidth,16);
            StyleHudImage(vitals,"HP Track",hpRect,Color.white,healthTrack);
            StyleHudImage(vitals,"HP Fill",hpRect,Color.white,healthEnamel);
            StyleHudImage(vitals,"MP Track",mpRect,Color.white,manaTrack);
            StyleHudImage(vitals,"MP Fill",mpRect,Color.white,manaEnamel);
            for(int amount=ManaSegment;amount<GameModel.MaxMana;amount+=ManaSegment){
                float x=VitalsBarLeft+VitalsBarWidth*amount/GameModel.MaxMana;
                StyleHudImage(vitals,"MP Divider "+(amount/ManaSegment),new Rect(x-1,mpRect.y,2,mpRect.height),Color.white,manaDivider);
            }
            var portrait=StyleHudImage(vitals,"Portrait",new Rect(13,445,102,84),Color.white,Picture("Pixel/ui/portrait"));
            portrait.preserveAspect=true;
            StyleHudText(vitals,"Name",new Rect(112,502,200,27),"LORD KNIGHT",16,C(0xffe3a3));
            StyleHudText(vitals,"HP",new Rect(183,483,107,19),null,11,C(0xffefd8),TextAnchor.MiddleRight);
            StyleHudText(vitals,"MP",new Rect(183,461,107,18),null,11,C(0xd8f5ff),TextAnchor.MiddleRight);
            for(int i=0;i<3;i++)StyleHudImage(vitals,"Awakening Orb "+i,new Rect(23+i*22,436,17,17),C(0x775e72),jewel);
            StyleHudImage(vitals,"Awakening Gauge Track",new Rect(VitalsBarLeft,450,VitalsBarWidth,4),C(0x4d2b40));
            StyleHudImage(vitals,"Awakening Gauge",new Rect(VitalsBarLeft,450,0,4),C(0xe15478));
            if(labels.TryGetValue("Combo",out var oldCombo))oldCombo.gameObject.SetActive(false);
            comboStatusEdge=float.NaN;
            FitStatusBesideCombo(false);
            shownMaxHealth=-1;
            if(!comboReadout||comboReadout.parent!=safeRoot)BuildComboReadout();
        }

        static Color BarEnamel(float length,int y,Color left,Color right) {
            var color=Color.Lerp(left,right,length);
            if(y==0||y==8)return Color.Lerp(color,C(0x201b26),.68f);
            if(y==7)return Color.Lerp(color,Color.white,.66f);
            if(y==6)return Color.Lerp(color,Color.white,.28f);
            return y<=2?Color.Lerp(color,C(0x21152e),.22f):color;
        }

        static Color BarTrack(float length,int y,Color left,Color right) {
            if(y==0||y==8)return C(0x171522);
            return Color.Lerp(left,right,length);
        }

        Sprite MakeVitalsSprite(string id,int width,int height,System.Func<int,int,Color> sample) {
            var texture=PaintVitalsTexture(id,width,height,sample);
            return MakeVitalsSprite(id,texture,new Rect(0,0,width,height));
        }

        Texture2D PaintVitalsTexture(string id,int width,int height,System.Func<int,int,Color> sample) {
            var texture=new Texture2D(width,height,TextureFormat.RGBA32,false){
                name=id,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp,
                hideFlags=HideFlags.DontSave
            };
            var pixels=new Color32[width*height];
            for(int y=0;y<height;y++)for(int x=0;x<width;x++)pixels[y*width+x]=sample(x,y);
            texture.SetPixels32(pixels);
            texture.Apply(false,true);
            vitalsTextures.Add(texture);
            return texture;
        }

        Sprite MakeVitalsSprite(string id,Texture2D texture,Rect cell) {
            var sprite=Sprite.Create(texture,cell,new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);
            sprite.name=id;
            sprite.hideFlags=HideFlags.DontSave;
            vitalsArt.Add(sprite);
            return sprite;
        }

        Image StyleHudImage(Transform parent,string name,Rect rect,Color color,Sprite custom=null) {
            if(!images.TryGetValue(name,out var image))images[name]=image=Panel(name,rect,color,parent);
            if(image.transform.parent!=parent)image.transform.SetParent(parent,false);
            image.transform.SetAsLastSibling();
            PlaceMenuItem(image.transform,rect);
            image.color=color;
            if(custom){
                image.sprite=custom;
                image.type=Image.Type.Simple;
                image.preserveAspect=false;
            }
            var inset=image.transform.Find("Pixel Inlay");
            if(inset)inset.gameObject.SetActive(false);
            return image;
        }

        void StyleHudText(Transform parent,string name,Rect rect,string title,int size,Color ink,TextAnchor align=TextAnchor.MiddleLeft) {
            if(!labels.TryGetValue(name,out var label))
                labels[name]=label=Label(name,title??"",rect,size,ink,align,parent);
            if(label.transform.parent!=parent)label.transform.SetParent(parent,false);
            label.transform.SetAsLastSibling();
            PlaceMenuItem(label.transform,rect);
            label.fontSize=size;
            label.alignment=align;
            label.color=ink;
            if(title!=null)label.text=title;
        }

        void BuildComboReadout() {
            if(!comboSlashArt){
                for(int i=0;i<comboRankArt.Length;i++)
                    comboRankArt[i]=PaintComboRank(ComboRanks[i],ComboColors[i]);
                var atlas=PaintVitalsTexture("Combo numeral atlas",DigitCellWidth*comboDigitArt.Length,DigitCellHeight,
                    (x,y)=>PaintComboDigit(ComboDigitGlyphs[x/DigitCellWidth],x%DigitCellWidth,y));
                for(int i=0;i<comboDigitArt.Length;i++)
                    comboDigitArt[i]=MakeVitalsSprite(i.ToString(),atlas,new Rect(i*DigitCellWidth,0,DigitCellWidth,DigitCellHeight));
                comboSlashArt=MakeVitalsSprite("Combo ink slash",100,24,(x,y)=>{
                    int left=y/2,right=82+y*3/4;
                    if(x<left||x>=right)return Color.clear;
                    if(y==3&&x>6&&x<right-9)return C(0xf8dcc6,.72f);
                    if(y<4||y>=20||x<left+2||x>=right-2)return C(0x2b2032,.94f);
                    return C(y>15?0x37263bu:0x1c172bu,.91f);
                });
                comboSparksArt=MakeVitalsSprite("Combo impact marks",80,33,(x,y)=>{
                    if(y<4&&x>8&&x<49+y*4)return Color.white;
                    if(y==25&&x>=15&&x<=47||y==26&&x>=20&&x<=41)return Color.white;
                    if(x+2*y>=105&&x+2*y<=108&&y>10&&y<28)return Color.white;
                    if(x+2*y>=115&&x+2*y<=118&&y>13&&y<31)return Color.white;
                    if(x+2*y>=125&&x+2*y<=127&&y>17&&y<31)return Color.white;
                    return Color.clear;
                });
            }
            comboReadout=UIRect("Combo readout",safeRoot,new Rect(18,273,420,152));
            var banner=safeRoot.Find("Banner Panel");
            if(banner)comboReadout.SetSiblingIndex(banner.GetSiblingIndex());
            comboSlash=ControlImage("Combo ink Accent",new Rect(0,0,200,39),comboSlashArt,comboReadout);
            comboSlash.preserveAspect=false;
            comboRank=ControlImage("Combo Rank",new Rect(6,-3,160,56),comboRankArt[0],comboReadout);
            comboRank.preserveAspect=false;
            comboSparks=ControlImage("Combo impact Accent",new Rect(0,0,218,53),comboSparksArt,comboReadout);
            comboSparks.preserveAspect=false;
            comboDigits=UIRect("Combo Count",comboReadout,new Rect(0,44,ComboDigitBudget,DigitHeight));
            for(int i=0;i<comboDigitViews.Length;i++){
                var digit=ControlImage("Combo Digit "+i,new Rect(i*DigitAdvance,0,DigitWidth,DigitHeight),comboDigitArt[0],comboDigits);
                digit.preserveAspect=false;
                digit.enabled=false;
                comboDigitViews[i]=digit;
            }
            comboReadout.gameObject.SetActive(false);
        }

        static Color PaintComboDigit(ulong glyph,int x,int y) {
            bool Face(int px,int py){
                int row=py-2;
                if(row<0||row>=9)return false;
                int column=px-2-row/3;
                return column>=0&&column<7&&(glyph&(1UL<<(row*7+6-column)))!=0;
            }
            if(Face(x,y)){
                if(y>=8&&!Face(x,y+1))return Color.white;
                if(y>=7)return C(0xfff2d4);
                return y>=4?C(0xffd58b):C(0xf2a760);
            }
            if(Face(x-1,y)||Face(x+1,y)||Face(x,y+1)||Face(x,y-1))return C(0x221c31);
            if(Face(x-2,y+2)||Face(x-3,y+2)||Face(x-1,y+2))return C(0x50344c);
            return Color.clear;
        }

        Sprite PaintComboRank(string title,Color ink) {
            int width=title.Length*7+12,height=16;
            var letter=new bool[width,height];
            for(int i=0;i<title.Length;i++){
                ulong glyph=ComboGlyphs[ComboAlphabet.IndexOf(title[i])];
                for(int y=0;y<7;y++)for(int x=0;x<5;x++)
                    if((glyph&(1UL<<(y*5+4-x)))!=0)letter[4+i*7+x+y/2,4+y]=true;
            }
            bool At(int x,int y){return x>=0&&y>=0&&x<width&&y<height&&letter[x,y];}
            return MakeVitalsSprite(title,width,height,(x,y)=>{
                if(At(x,y)){
                    if(y>=8&&!At(x,y+1))return Color.Lerp(ink,Color.white,.82f);
                    return y>7?Color.Lerp(ink,Color.white,.30f):Color.Lerp(ink,C(0xb04c5e),.20f);
                }
                if(At(x-1,y)||At(x+1,y)||At(x,y-1)||At(x,y+1))return C(0x24172c);
                if(At(x-2,y+2)||At(x-3,y+2)||At(x-2,y+3)||At(x-1,y+2))return C(0x462144,.95f);
                return Color.clear;
            });
        }

        void SetComboDigits(int count,int rank) {
            string value=count.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Color ink=Color.Lerp(ComboColors[rank],Color.white,.78f);
            for(int i=0;i<comboDigitViews.Length;i++){
                var digit=comboDigitViews[i];
                digit.enabled=i<value.Length;
                if(!digit.enabled)continue;
                digit.sprite=comboDigitArt[value[i]-'0'];
                digit.color=ink;
            }
            float width=DigitWidth+DigitAdvance*(value.Length-1);
            comboDigitScale=Mathf.Min(1,ComboDigitBudget/width);
            comboDigitSpan=width*comboDigitScale;
        }

        void FitStatusBesideCombo(bool showCombo) {
            // Reserve space for the entire pulse. The nearby buffs and control
            // hint then move only when the count gains or loses a digit.
            float edge=showCombo?Mathf.Ceil(comboReadout.anchoredPosition.x+
                comboDigitSpan*(1+ComboDigitPulseWidth)+6):0;
            if(edge==comboStatusEdge)return;
            comboStatusEdge=edge;
            float badgeLeft=Mathf.Max(87,edge);
            PlaceComboNeighbor("Awakened Buff Panel",badgeLeft);
            PlaceComboNeighbor("Awakened Buff Icon",badgeLeft+4);
            PlaceComboNeighbor("Awakened Buff",badgeLeft+28);
            float bladeLeft=Mathf.Max(87,edge+AwakeningKeyClearance);
            PlaceComboNeighbor("Blade Buff Panel",bladeLeft);
            PlaceComboNeighbor("Blade Buff Icon",bladeLeft+4);
            PlaceComboNeighbor("Blade Buff",bladeLeft+28);
            Width("Blade Buff Panel",Mathf.Min(260,BattleNavigationLeft-bladeLeft-8));
            PlaceComboNeighbor("Awakening Button",Mathf.Max(22,edge));
        }

        void PlaceComboNeighbor(string name,float left) {
            if(images.TryGetValue(name,out var image)){
                var rect=image.rectTransform;
                rect.anchoredPosition=new Vector2(left,rect.anchoredPosition.y);
            }else if(labels.TryGetValue(name,out var label)){
                var rect=label.rectTransform;
                rect.anchoredPosition=new Vector2(left,rect.anchoredPosition.y);
            }
        }

        void RenderPlayerVitals() {
            float maxHealth=Mathf.Max(1,game.player.maxHp);
            Width("HP Fill",VitalsBarWidth*Mathf.Clamp01(game.player.hp/maxHealth));
            Width("MP Fill",VitalsBarWidth*Mathf.Clamp01(game.mana/GameModel.MaxMana));
            int hp=Mathf.CeilToInt(Mathf.Max(0,game.player.hp));
            int maximum=Mathf.CeilToInt(maxHealth);
            int mp=Mathf.CeilToInt(Mathf.Max(0,game.mana));
            if(hp!=lastHudHealth||maximum!=shownMaxHealth){TextValue("HP",hp+" / "+maximum);lastHudHealth=hp;shownMaxHealth=maximum;}
            if(mp!=lastHudMana){TextValue("MP",mp+" / "+GameModel.MaxMana);lastHudMana=mp;}
        }

        void RenderComboReadout() {
            if(!comboReadout)return;
            int count=game.combo;
            bool visible=count>0&&adventureStarted&&!game.victory&&!game.paused&&game.player.hp>0&&
                string.IsNullOrEmpty(openMenu)&&!settingsOpen&&!BossIntroductionActive;
            if(comboReadout.gameObject.activeSelf!=visible)comboReadout.gameObject.SetActive(visible);
            if(!visible){shownCombo=-1;shownRank=-1;FitStatusBesideCombo(false);return;}
            if(count!=shownCombo){
                int rank=count<5?0:count<=20?1:2;
                SetComboDigits(count,rank);
                if(rank!=shownRank){
                    shownRank=rank;
                    comboRankAt=Time.unscaledTime;
                    comboRank.sprite=comboRankArt[rank];
                    float width=comboRank.sprite.rect.width*3.1f;
                    comboRank.rectTransform.sizeDelta=new Vector2(width,56);
                    comboSlash.rectTransform.sizeDelta=new Vector2(width+1,36);
                    comboSparks.rectTransform.sizeDelta=new Vector2(width+12,53);
                }
                comboPulseAt=Time.unscaledTime;
                shownCombo=count;
            }
            float pulse=Mathf.Clamp01(1-(Time.unscaledTime-comboPulseAt)/.19f);
            comboDigits.localScale=new Vector3(comboDigitScale*(1+pulse*ComboDigitPulseWidth),comboDigitScale*(1+pulse*.09f),1);
            float impact=Mathf.Clamp01(1-(Time.unscaledTime-comboRankAt)/.38f);
            comboRank.rectTransform.localScale=new Vector3(1+.19f*impact,1+.09f*impact,1);
            Color sparks=ComboColors[shownRank];
            sparks.a=(shownRank==2?.74f:.54f)*impact;
            comboSparks.color=sparks;
            FitStatusBesideCombo(true);
        }

        void DisposeCombatHudArt() {
            foreach(var sprite in vitalsArt)if(sprite)Destroy(sprite);
            foreach(var texture in vitalsTextures)if(texture)Destroy(texture);
            vitalsArt.Clear();
            vitalsTextures.Clear();
        }
    }
}
