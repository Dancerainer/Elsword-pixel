using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ElswordPixel {
    // One point-filtered atlas and one mesh draw per floating amount, regardless of digit count.
    internal sealed class CombatFeedbackArt : IDisposable {
        internal const string Alphabet="0123456789+%.<";
        internal const int CellWidth=16,CellHeight=20;
        const int AtlasWidth=256,AtlasHeight=128;
        static readonly string[] masks={
            "01110/11011/11011/11011/11011/11011/01110",
            "00110/01110/00110/00110/00110/00110/01111",
            "01110/11011/00011/00110/01100/11000/11111",
            "11110/00011/00011/01110/00011/00011/11110",
            "00011/00111/01111/11011/11111/00011/00011",
            "11111/11000/11000/11110/00011/00011/11110",
            "01110/11000/11000/11110/11011/11011/01110",
            "11111/00011/00110/00110/01100/01100/01100",
            "01110/11011/11011/01110/11011/11011/01110",
            "01110/11011/11011/01111/00011/00011/01110",
            "00000/00000/00100/11111/00100/00000/00000",
            "11001/11001/00010/00100/01000/10011/10011",
            "00/00/00/00/00/11/11",
            "0001/0010/0100/1100/0100/0010/0001"
        };
        static readonly uint[,] palette={
            {0xfffbd0,0xffe555,0xffbb26,0xe57c20,0x84402a,0x372039},
            {0xfff4b7,0xff9671,0xff463b,0xe11e38,0x92203c,0x361729},
            {0xf0ffd3,0xbaff82,0x64ed71,0x24ad60,0x167254,0x163546},
            {0xe4ffff,0x9ffaff,0x42cfff,0x3287ed,0x3253a5,0x162a4a}
        };
        internal readonly Material digits;
        readonly Texture2D atlas,auraAtlas;
        const int AuraFrames=12;
        internal readonly Sprite[] flame=new Sprite[AuraFrames];
        internal CombatFeedbackArt() {
            var colors=new Color32[AtlasWidth*AtlasHeight];
            for(int kind=0;kind<4;kind++)for(int i=0;i<masks.Length;i++)PaintDigit(colors,kind,i);
            atlas=Texture("Battle numeral atlas",AtlasWidth,AtlasHeight,colors);
            digits=new Material(Shader.Find("Sprites/Default")){name="Battle numerals",mainTexture=atlas};
            const int w=108,h=118;colors=new Color32[w*h*flame.Length];
            for(int f=0;f<flame.Length;f++)PaintFlame(colors,w,h,f);
            auraAtlas=Texture("Awakening fire",w*flame.Length,h,colors);
            for(int f=0;f<flame.Length;f++)
                flame[f]=Sprite.Create(auraAtlas,new Rect(f*w,0,w,h),new Vector2(.5f,6f/h),1,0,SpriteMeshType.FullRect);
        }
        static Color32 ColorOf(uint rgb,byte a=255){return new Color32((byte)(rgb>>16),(byte)(rgb>>8),(byte)rgb,a);}
        static Texture2D Texture(string name,int width,int height,Color32[] colors){
            var t=new Texture2D(width,height,TextureFormat.RGBA32,false){name=name,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
            t.SetPixels32(colors);t.Apply(false,true);return t;
        }
        static void PaintDigit(Color32[] atlas,int kind,int i){
            var shape=new bool[CellWidth*CellHeight];
            var rows=masks[i].Split('/');
            for(int y=0;y<rows.Length;y++)for(int x=0;x<rows[y].Length;x++){
                if(rows[y][x]!='1')continue;
                int left=2+x*2+(6-y)/3,baseY=3+(6-y)*2;
                for(int dy=0;dy<2;dy++)for(int dx=0;dx<2;dx++)shape[(baseY+dy)*CellWidth+left+dx]=true;
            }
            bool Ink(int x,int y){return x>=0&&x<CellWidth&&y>=0&&y<CellHeight&&shape[y*CellWidth+x];}
            for(int y=0;y<CellHeight;y++)for(int x=0;x<CellWidth;x++){
                int shade=-1;
                if(Ink(x,y)){
                    shade=y>=14?0:y>=10?1:y>=6?2:3;
                    if(!Ink(x-1,y)&&y>=8)shade=Math.Max(0,shade-1);
                }else{
                    for(int dy=-1;dy<=1;dy++)for(int dx=-1;dx<=1;dx++)
                        if(Ink(x+dx,y+dy))shade=4;
                    if(shade<0&&(Ink(x-1,y+2)||Ink(x-2,y+2)||Ink(x-2,y+1)))shade=5;
                }
                if(shade>=0)atlas[(kind*CellHeight+y)*AtlasWidth+i*CellWidth+x]=ColorOf(palette[kind,shade]);
            }
        }
        // Ordered silhouette of the pressure wave, in pixels relative to the feet.
        // The low outer flares roll upward into longer, inward-curving tips.
        static readonly Vector2[] auraOutline={
            new Vector2(-3,0), new Vector2(-18,2), new Vector2(-32,9),
            new Vector2(-43,23), new Vector2(-32,18), new Vector2(-46,46),
            new Vector2(-34,35), new Vector2(-42,68), new Vector2(-29,52),
            new Vector2(-28,87), new Vector2(-18,67), new Vector2(-9,97),
            new Vector2(-8,77), new Vector2(6,101), new Vector2(8,74),
            new Vector2(23,89), new Vector2(22,61), new Vector2(38,78),
            new Vector2(32,47), new Vector2(45,56), new Vector2(35,29),
            new Vector2(45,32), new Vector2(29,11), new Vector2(35,13),
            new Vector2(19,3), new Vector2(6,0)
        };
        static void PaintFlame(Color32[] dst,int w,int h,int frame){
            int stride=w*AuraFrames,origin=frame*w;
            float phase=2*Mathf.PI*frame/AuraFrames;
            Vector2[] contour=new Vector2[auraOutline.Length];
            float[] cuts=new float[contour.Length];

            // Back-to-front bands: a dark rim stays visible on light scenery.
            // The translucent center leaves gaps between the arms and sword readable.
            Layer(1.05f,1.02f,ColorOf(0x9c1238,125));
            Layer(1,1,ColorOf(0xf02543,194));
            Layer(.88f,.93f,ColorOf(0xff5859,222));
            Layer(.74f,.78f,ColorOf(0xffa087,185));
            Layer(.64f,.70f,ColorOf(0xff4250,65));

            for(int streak=0;streak<6;streak++){
                int sign=(streak&1)==0?-1:1;
                float t=Mathf.Repeat(frame/(float)AuraFrames+streak*.347f,1);
                int y=9+Mathf.RoundToInt(91*t);
                int radius=Mathf.RoundToInt(28+5*(streak%3)+13*Mathf.Sin(t*Mathf.PI));
                int length=Mathf.RoundToInt(8*Mathf.Sin(t*Mathf.PI));
                for(int i=0;i<length;i++){
                    int x=w/2+sign*(radius+i/3);
                    dst[(y+i)*stride+origin+x]=ColorOf(0xff515a,205);
                    if(i>1&&i<length-1)dst[(y+i)*stride+origin+x-sign]=ColorOf(0xffb5a2,223);
                }
            }

            void Layer(float width,float height,Color32 color){
                for(int i=0;i<contour.Length;i++){
                    Vector2 p=auraOutline[i];
                    float lift=Mathf.Clamp01(p.y/90f);
                    float wave=phase+p.x*.086f-p.y*.071f;
                    contour[i]=new Vector2(w*.5f+p.x*width+lift*3.5f*Mathf.Sin(wave),
                        6+height*(p.y+lift*5.5f*Mathf.Cos(wave)));
                }
                for(int y=0;y<h;y++){
                    float scan=y+.5f;
                    int count=0;
                    for(int i=0,j=contour.Length-1;i<contour.Length;j=i++){
                        Vector2 a=contour[j],b=contour[i];
                        if((a.y>scan)==(b.y>scan))continue;
                        cuts[count++]=a.x+(b.x-a.x)*(scan-a.y)/(b.y-a.y);
                    }
                    Array.Sort(cuts,0,count);
                    for(int n=0;n<count;n+=2){
                        int left=Mathf.Max(0,Mathf.CeilToInt(cuts[n]-.5f));
                        int right=Mathf.Min(w,Mathf.CeilToInt(cuts[n+1]-.5f));
                        for(int x=left;x<right;x++)dst[y*stride+origin+x]=color;
                    }
                }
            }
        }
        internal static Rect Uv(char c,CombatNumberKind kind){
            int i=Alphabet.IndexOf(c);
            return new Rect(i*CellWidth/(float)AtlasWidth,(int)kind*CellHeight/(float)AtlasHeight,
                CellWidth/(float)AtlasWidth,CellHeight/(float)AtlasHeight);
        }
        internal static int Advance(char c){return c=='.'?7:c=='<'?11:13;}
        public void Dispose(){
            foreach(var s in flame)UnityEngine.Object.Destroy(s);
            UnityEngine.Object.Destroy(digits);UnityEngine.Object.Destroy(atlas);UnityEngine.Object.Destroy(auraAtlas);
        }
    }

    internal sealed class CombatNumberView {
        internal readonly GameObject root;
        readonly Mesh mesh;
        readonly MeshRenderer renderer;
        readonly MaterialPropertyBlock properties=new MaterialPropertyBlock();
        readonly List<Vector3> vertices=new List<Vector3>(40);
        readonly List<Vector2> uvs=new List<Vector2>(40);
        readonly List<Color32> colors=new List<Color32>(40);
        readonly List<int> indices=new List<int>(60);
        Effect current;
        static readonly int Tint=Shader.PropertyToID("_Color");
        internal CombatNumberView(Transform parent,Material material){
            root=new GameObject("Pixel combat amount");root.transform.SetParent(parent,false);
            mesh=new Mesh{name="Pooled combat amount"};mesh.MarkDynamic();
            root.AddComponent<MeshFilter>().sharedMesh=mesh;
            renderer=root.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.sortingOrder=55;
        }
        internal void Draw(Effect e,float age,int nearby,int newer){
            root.SetActive(true);
            if(!ReferenceEquals(e,current)){current=e;SetGlyphs(Label(e),e.numberKind);}
            float lift=Mathf.Round(age*(24-8*age))+e.numberLane*10;
            float x=e.numberLane%2==0?0:6;
            float opacity=Mathf.Clamp01((1-age)*4);
            if(e.numberKind==CombatNumberKind.Damage||e.numberKind==CombatNumberKind.Critical){
                if(nearby>=4){
                    // The first four amounts fit their existing lanes. In a longer burst,
                    // spread the cluster around its target and retire superseded amounts first.
                    float crowd=Mathf.Clamp01((nearby-3)/4f);
                    float older=Mathf.Clamp01((newer-1)/5f);
                    float spread=Mathf.Round(crowd*(10+8*older));
                    x+=e.numberLane%2==0?-spread:spread;
                    lift+=Mathf.Round(crowd*(6+8*older));
                    float fadeEnd=Mathf.Lerp(1,.34f,crowd*older);
                    opacity=Mathf.Min(opacity,Mathf.Clamp01((fadeEnd-age)/.24f));
                }
            }
            root.transform.localPosition=new Vector3(x,lift,0);
            float size=e.numberKind==CombatNumberKind.Critical?1.24f:1;
            root.transform.localScale=Vector3.one*(size*(1+.16f*Mathf.Clamp01(1-age*6)));
            properties.SetColor(Tint,new Color(1,1,1,opacity));renderer.SetPropertyBlock(properties);
        }
        internal static string Label(Effect e){
            if(e.numberKind==CombatNumberKind.Health||e.numberKind==CombatNumberKind.Mana){
                string value=e.recoveryAmount<.01f?"<0.01":e.recoveryAmount.ToString("0.##",CultureInfo.InvariantCulture);
                return "+"+value+(e.numberPercent?"%":"");
            }
            return e.hasFractionalAmountR901?DemoScene.FormatBelowCannonFractionalNumberR901A2(e.fractionalAmountR901):e.amount.ToString(CultureInfo.InvariantCulture);
        }
        void SetGlyphs(string text,CombatNumberKind kind){
            vertices.Clear();uvs.Clear();colors.Clear();indices.Clear();
            int width=0;foreach(char c in text)width+=CombatFeedbackArt.Advance(c);
            float left=-Mathf.Round(width*.5f);
            foreach(char c in text){
                Rect uv=CombatFeedbackArt.Uv(c,kind);int j=vertices.Count;
                float right=left+CombatFeedbackArt.CellWidth;
                vertices.Add(new Vector3(left,-10));vertices.Add(new Vector3(left,10));
                vertices.Add(new Vector3(right,10));vertices.Add(new Vector3(right,-10));
                uvs.Add(new Vector2(uv.xMin,uv.yMin));uvs.Add(new Vector2(uv.xMin,uv.yMax));
                uvs.Add(new Vector2(uv.xMax,uv.yMax));uvs.Add(new Vector2(uv.xMax,uv.yMin));
                for(int k=0;k<4;k++)colors.Add(new Color32(255,255,255,255));
                indices.Add(j);indices.Add(j+1);indices.Add(j+2);indices.Add(j);indices.Add(j+2);indices.Add(j+3);
                left+=CombatFeedbackArt.Advance(c);
            }
            mesh.Clear();mesh.SetVertices(vertices);mesh.SetUVs(0,uvs);mesh.SetColors(colors);mesh.SetTriangles(indices,0);
        }
        internal void Dispose(){UnityEngine.Object.Destroy(mesh);}
    }

    public sealed partial class DemoScene {
        CombatFeedbackArt combatFeedbackArt;
        SpriteRenderer awakeningFlame;
        readonly List<int> combatDamageNumbers=new List<int>();
        int[] nearbyCombatNumbers=new int[16],newerCombatNumbers=new int[16];
        void PrepareCombatNumberLanes(){
            var effects=game.effects;
            if(effects.Count>nearbyCombatNumbers.Length){
                int capacity=Mathf.NextPowerOfTwo(effects.Count);
                nearbyCombatNumbers=new int[capacity];newerCombatNumbers=new int[capacity];
            }
            combatDamageNumbers.Clear();
            for(int i=0;i<effects.Count;i++){
                var e=effects[i];
                if(e.type!="number")continue;
                nearbyCombatNumbers[i]=newerCombatNumbers[i]=0;
                if(e.numberKind==CombatNumberKind.Damage||e.numberKind==CombatNumberKind.Critical)
                    combatDamageNumbers.Add(i);
            }
            // A cluster only changes its layout when four other amounts overlap.
            if(combatDamageNumbers.Count<5)return;
            for(int a=0;a<combatDamageNumbers.Count;a++){
                int i=combatDamageNumbers[a];var first=effects[i];
                for(int b=a+1;b<combatDamageNumbers.Count;b++){
                    int j=combatDamageNumbers[b];var second=effects[j];
                    if(first.facing!=second.facing||Mathf.Abs(first.x-second.x)>=18||Mathf.Abs(first.y-second.y)>=24)continue;
                    nearbyCombatNumbers[i]++;nearbyCombatNumbers[j]++;newerCombatNumbers[i]++;
                }
            }
        }
        CombatFeedbackArt FeedbackArt {get{return combatFeedbackArt??(combatFeedbackArt=new CombatFeedbackArt());}}
        void RenderAwakeningFlame(){
            bool active=(game.AwakeningActive||game.AwakeningCasting)&&game.player.hp>0&&heroView&&heroView.enabled;
            if(!active){if(awakeningFlame)awakeningFlame.enabled=false;return;}
            if(!awakeningFlame)awakeningFlame=WorldSprite("Awakening red flame",effectsRoot,null,heroView.sortingOrder-2,Vector3.zero);
            awakeningFlame.enabled=true;
            // Cast dimming changes the hero's order from20 to100. Follow it on
            // every frame so the flame cannot remain hidden under the dimmer.
            awakeningFlame.sortingOrder=heroView.sortingOrder-2;
            var frames=FeedbackArt.flame;
            float elapsed=game.AwakeningActive?game.AwakeningDuration-game.AwakeningRemaining:game.AwakeningCastTime;
            awakeningFlame.sprite=frames[Mathf.Max(0,Mathf.FloorToInt(elapsed*15))%frames.Length];
            float breath=.5f-.5f*Mathf.Cos(elapsed*4.2f);
            awakeningFlame.color=new Color(1,1,1,Mathf.Lerp(.65f,.78f,breath));
            var feet=RenderPlayerPosition;
            awakeningFlame.transform.localPosition=new Vector3(feet.x,feet.y,0);
            awakeningFlame.flipX=game.player.facing<0;
        }
        void DisposeCombatFeedback(){
            foreach(var view in fxViews)if(view.number!=null)view.number.Dispose();
            if(combatFeedbackArt!=null){combatFeedbackArt.Dispose();combatFeedbackArt=null;}
        }
    }
}
