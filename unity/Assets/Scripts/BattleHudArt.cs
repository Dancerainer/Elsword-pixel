using System;
using UnityEngine;

namespace ElswordPixel {
    // All battle emblems share a small, point-sampled palette and a stepped outline.
    sealed class BattleHudArt {
        const int AtlasSize=256;
        internal readonly Texture2D texture=new Texture2D(AtlasSize,AtlasSize,TextureFormat.RGBA32,false){
            name="Battle controls",filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp
        };
        Color32[] pixels=new Color32[AtlasSize*AtlasSize];
        int nextX,nextY,rowHeight;
        internal Sprite Disc(Color accent,bool thumb=false){
            return Paint(64,64,(x,y)=>{
                float dx=x-31.5f,dy=y-31.5f,r=Mathf.Sqrt(dx*dx+dy*dy);
                if(r>31.5f)return Color.clear;
                if(r>30.25f)return new Color(.04f,.05f,.12f,.96f);
                if(r>28.95f)return Color.Lerp(accent,Color.white,(dy-dx>0)?.70f:.20f);
                if(r>27.8f)return Color.Lerp(accent,Color.black,.40f);
                if(r>27f)return Color.Lerp(accent,Color.white,.18f);
                if(thumb)return Color.Lerp(new Color(.21f,.25f,.43f,.94f),new Color(.70f,.75f,.91f,.95f),Mathf.Clamp01((dy-dx+28)/74));
                var body=Color.Lerp(new Color(.046f,.060f,.12f,.70f),new Color(.11f,.12f,.23f,.82f),Mathf.Clamp01((dy-dx+36)/85));
                if(r>24f)return Color.Lerp(body,new Color(accent.r,accent.g,accent.b,.68f),.20f);
                return body;
            });
        }
        internal Sprite Stamp(string[] pixels,Color accent){
            return Paint(pixels[0].Length,pixels.Length,(x,y)=>{
                switch(pixels[pixels.Length-1-y][x]){
                    case '+':return new Color(.97f,.96f,.87f,1);
                    case 'o':return Color.Lerp(accent,Color.white,.48f);
                    case '#':return accent;
                    case 'x':return Color.Lerp(accent,new Color(.05f,.07f,.15f),.61f);
                    default:return Color.clear;
                }
            });
        }
        internal Sprite SprintRing(){
            return Paint(64,64,(x,y)=>{
                float dx=x-31.5f,dy=y-31.5f,r=Mathf.Sqrt(dx*dx+dy*dy);
                if(r>31.5f||r<28.8f)return Color.clear;
                return r>30.4f?new Color(.16f,.25f,.40f,.80f):Color.white;
            });
        }
        internal static readonly string[] Blade={
            "...........++","..........+o+",".........+o#x","........+o#x.",".......+o#x..","......+o#x...",
            ".....+o#x....","..o.+o#x.....","..oo##x......","...oo#.......","..oo#oo......",".#ox..o......","x#x.........."};
        internal static readonly string[] GreatBlade={
            ".........++++","........+ooo+",".......+ooo#x","......+ooo#x.",".....+ooo#x..","....+ooo#x...",
            ".oo.+oo#x....","..oo+o#x.....","...oo#xo.....","..oo##ooo....",".oo#x..oo....","ox#x.........","x#x.........."};
        internal static readonly string[] Crystal={
            "......+......",".....+o+.....","..#..+o+..#..","..o#.+o+.#o..","..#o+o#o+o#..",".#.#++o++#.#.",
            ".o#.#+++#.#o.","..oo.#o#.oo..","...#o#o#o#...","....#o#o#....",".....#o#.....","......#......"};
        internal static readonly string[] Bottle={
            "....oooo....","...x++++x...","....xoox....","....+..x....","....+..x....","...o+..ox...",
            "..o+....ox..",".o+ooooooox.",".o+#######x.",".o+o######x.",".o+#######x.",".xoo#####xx.",
            "..xoooooox..","...xxxxxx..."};
        internal static readonly string[] Arrow={"...+...","..+o+..",".+ooo+.","+ooooo+"};
        Sprite Paint(int width,int height,Func<int,int,Color> color){
            if(nextX+width>AtlasSize){nextX=0;nextY+=rowHeight+1;rowHeight=0;}
            if(nextY+height>AtlasSize)throw new InvalidOperationException("Battle control atlas is full");
            for(int y=0;y<height;y++)for(int x=0;x<width;x++)
                pixels[(nextY+y)*AtlasSize+nextX+x]=color(x,y);
            var rect=new Rect(nextX,nextY,width,height);
            nextX+=width+1;rowHeight=Math.Max(rowHeight,height);
            return Sprite.Create(texture,rect,new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect);
        }
        internal void Upload(){
            texture.SetPixels32(pixels);texture.Apply(false,true);pixels=null;
        }
    }
}
