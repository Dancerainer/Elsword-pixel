using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
    // Pixel shapes are in safe-canvas units. No texture atlases are allocated on victory.
    public sealed class DungeonResultInk : MaskableGraphic {
        public enum Shape { Card, Crest, Clock, Chain, Guard }
        Shape shape;
        string grade = "S";
        Color accent = new Color32(255,196,83,255);
        public static Color RankColor(string rank) {
            return rank.StartsWith("S") ? (Color)new Color32(255,193,74,255) :
                rank=="A" ? (Color)new Color32(133,223,255,255) :
                rank=="B" ? (Color)new Color32(146,235,196,255) : new Color32(210,205,228,255);
        }
        public void Draw(Shape kind, string rank = "S") {
            shape = kind;
            grade = rank;
            accent = RankColor(rank);
            raycastTarget = false;
            SetVerticesDirty();
        }
        static Color32 Ink(int rgb) { return new Color32((byte)(rgb>>16),(byte)(rgb>>8),(byte)rgb,255); }
        static void Quad(VertexHelper vh, Color color, float x1,float y1,float x2,float y2,float x3,float y3,float x4,float y4) {
            int offset=vh.currentVertCount;
            vh.AddVert(new Vector3(x1,y1),color,Vector2.zero);vh.AddVert(new Vector3(x2,y2),color,Vector2.zero);
            vh.AddVert(new Vector3(x3,y3),color,Vector2.zero);vh.AddVert(new Vector3(x4,y4),color,Vector2.zero);
            vh.AddTriangle(offset,offset+1,offset+2);vh.AddTriangle(offset,offset+2,offset+3);
        }
        static void Box(VertexHelper h,Color c,float x,float y,float w,float height) {
            Quad(h,c,x,y,x,y+height,x+w,y+height,x+w,y);
        }
        static void Gem(VertexHelper h,Color c,int x,int y,int w,int height) {
            Quad(h,c,x-w,y,x,y+height,x+w,y,x,y-height);
        }
        static void Card(VertexHelper h) {
            Color shadow=Ink(0x171521), edge=Ink(0x695263), face=Ink(0x171e31);
            // Right-hand cutaway terminates the gilt rule at the lower corner.
            Box(h,shadow,3,-3,462,210);
            Box(h,edge,0,8,466,206);Box(h,edge,8,0,450,8);
            Box(h,face,2,9,462,202);Box(h,face,9,2,448,8);
            Box(h,Ink(0x222a40),2,160,462,51);
            Quad(h,Ink(0x322d41),275,161,327,211,464,211,464,161);
            for(int i=0;i<4;i++){
                float x=287+i*10;Quad(h,Ink(0x423447),x,161,x+45,207,x+49,207,x+4,161);
            }
            Box(h,Ink(0x9b6643),4,211,458,1);Box(h,Ink(0xffcf72),4,212,273,2);
            Box(h,Ink(0xe38b48),2,162,3,46);
            Box(h,Ink(0x544551),3,158,460,2);
            Box(h,Ink(0x242336),301,10,162,147);
            Box(h,Ink(0x453745),300,11,1,143);
            // Engraved diagonal marks are confined to the rank plate.
            for(int i=0;i<5;i++){
                int x=305+i*10;Quad(h,Ink(0x2e283b),x,10,x+54,64,x+56,64,x+2,10);
            }
            Box(h,Ink(0xe68b50),9,2,41,2);Box(h,Ink(0xffdc88),9,4,20,2);
            Box(h,Ink(0x735346),416,3,38,2);
        }
        static readonly string[] S={"01111","11000","11000","01110","00011","00011","11110"};
        static readonly string[] A={"01110","11011","11011","11111","11011","11011","11011"};
        static readonly string[] B={"11110","11011","11011","11110","11011","11011","11110"};
        static readonly string[] C={"01111","11000","11000","11000","11000","11000","01111"};
        static readonly string[] D={"11110","11011","11011","11011","11011","11011","11110"};
        static readonly string[] E={"11111","11000","11000","11110","11000","11000","11111"};
        static readonly string[] F={"11111","11000","11000","11110","11000","11000","11000"};
        static string[] Glyph(char ch) {
            switch(ch){case 'S':return S; case 'A':return A; case 'B':return B; case 'C':return C;
                case 'D':return D; case 'E':return E; default:return F;}
        }
        void Letters(VertexHelper h,int dx,int dy,bool shadow) {
            int size=grade.Length>2?5:7;
            int left=78-((grade.Length*6-1)*size+6)/2;
            for(int i=0;i<grade.Length;i++){
                var glyph=Glyph(grade[i]);
                for(int y=0;y<7;y++){
                    int x0=left+i*size*6+6-y;
                    Color shade=shadow?Ink(0x5c3641):Color.Lerp(Color.white,accent,Mathf.Min(1f,y*.24f));
                    for(int x=0;x<5;x++)if(glyph[y][x]=='1')Box(h,shade,x0+x*size+dx,55+(3-y)*size+dy,size,size);
                }
            }
        }
        void Crest(VertexHelper h) {
            Color dark=Ink(0x332c43),mid=Color.Lerp(dark,accent,.4f), line=Color.Lerp(dark,accent,.65f);
            for(int i=0;i<3;i++){
                int x=7+i*5,y=47+i*7;
                Quad(h,line,x,y+10,x+24,y+4,x+34,y-4,x+13,y-1);
                Quad(h,line,156-x-34,y-4,156-x-24,y+4,156-x,y+10,156-x-13,y-1);
            }
            Gem(h,mid,78,53,49,47);Gem(h,dark,78,53,46,44);Gem(h,line,78,53,41,39);
            Gem(h,Ink(0x392d42),78,53,39,37);
            Gem(h,accent,78,102,3,3);Gem(h,accent,78,4,3,3);
            Letters(h,1,-4,true);Letters(h,0,0,false);
        }
        static readonly string[] Clock={"0011100","0100010","1001001","1001101","1000001","0100010","0011100"};
        static readonly string[] Chain={"0000111","0001001","0011011","0110110","1101100","1001000","1110000"};
        static readonly string[] Guard={"0111110","1111111","1100011","0100010","0110110","0011100","0001000"};
        protected override void OnPopulateMesh(VertexHelper h) {
            h.Clear();
            if(shape==Shape.Card){Card(h);return;}
            if(shape==Shape.Crest){Crest(h);return;}
            var glyph=shape==Shape.Clock?Clock:shape==Shape.Chain?Chain:Guard;
            for(int y=0;y<7;y++)for(int x=0;x<7;x++)if(glyph[y][x]=='1')Box(h,Ink(0xeac486),x*2,12-y*2,2,2);
        }
    }
}
