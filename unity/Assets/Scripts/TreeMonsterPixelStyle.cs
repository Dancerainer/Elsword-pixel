using UnityEngine;

namespace ElswordPixel {
    // The chicken uses a few warm, flat paint values and large readable clusters.
    // Reduce the owned humanoid atlas to that vocabulary, after source validation.
    // A fixed palette also prevents each moving part from acquiring a different
    // set of blended browns when its high-resolution painting is reduced.
    internal static class TreeMonsterPixelStyle {
        const float LogicalCell=2f;
        static readonly Color32[] Palette={
            Ink(0x30232b),Ink(0x53303a),                       // warm outline / deep shade
            Ink(0x77343a),Ink(0xae463b),Ink(0xe86b48),         // red hair / sash
            Ink(0x884a37),Ink(0xbb724b),Ink(0xeca46a),Ink(0xffce91), // skin / leather
            Ink(0x514038),Ink(0x806044),Ink(0xae8651),Ink(0xdbb66a),Ink(0xffde91), // wood / brass
            Ink(0x353139),Ink(0x585058),Ink(0x8b7d80),         // vest / steel shadows
            Ink(0xb8a4a0),Ink(0xe1ccc0),Ink(0xffeedc),        // cloth, with the chicken's warm white
            Ink(0x283429),Ink(0x425334),Ink(0x6b7c44),Ink(0x9fa55a) // thief cloth
        };
        static Color32 Ink(uint rgb){return new Color32((byte)(rgb>>16),(byte)(rgb>>8),(byte)rgb,255);}
        internal static Color32 Quantize(Color32 color){return color.a<128?default:Palette[Nearest(color)];}
        internal static bool Applies(string npc) { return npc=="NUI_THIEF_THIN_ZERO"||npc=="NUI_BENDERS_NORMAL"; }
        internal static void Paint(FairyPoseLibraryV33 lib,int part,Color32[] pixels,Vector2Int origin,int stride) {
            if(!Applies(lib.rigName))return;
            var p=lib.parts[part];lib.Find("Wait").SampleAxes(0,part,out _,out _,out var scale);
            int w=p.rect[2],h=p.rect[3];
            // The thief's trousers are already small authored pixel tiles. Do
            // not sample these 15-pixel tiles as if they were large paintings.
            if(ThiefLegPose.UsesPart(lib.rigName,part)) {
                for(int y=0;y<h;y++)for(int x=0;x<w;x++) {
                    int i=(origin.y+y)*stride+origin.x+x;
                    if(pixels[i].a>=128)pixels[i]=Palette[Nearest(pixels[i])];
                }
                return;
            }
            // Use square source cells sized from the larger projected axis.
            // Separate X/Y scaling used to make rectangular stripes on limbs
            // when their paint axes were rotated by the assembled skeleton.
            float unit=lib.UnitsPerPixel*p.scale*Mathf.Max(.1f,Mathf.Max(scale.x,scale.y));
            int cell=Mathf.Clamp(Mathf.RoundToInt(LogicalCell/unit),1,48);
            var votes=new int[Palette.Length];
            int columns=(w+cell-1)/cell,rows=(h+cell-1)/cell;
            var clusters=new int[columns*rows];
            // Accessories inside one opaque sprite have no alpha boundary for
            // MonsterInk to find. Keep their source-material boundary before
            // reducing the colours, so a mask is distinct from the hair behind it.
            bool accessories=lib.rigName=="NUI_THIEF_THIN_ZERO"&&(part==10||part==6);
            var accessory=accessories?new bool[clusters.Length]:null;
            var receivingSurface=accessories?new bool[clusters.Length]:null;
            for(int i=0;i<clusters.Length;i++)clusters[i]=-1;
            for(int y=0;y<h;y+=cell)for(int x=0;x<w;x+=cell) {
                System.Array.Clear(votes,0,votes.Length);
                int right=Mathf.Min(w,x+cell),top=Mathf.Min(h,y+cell),count=0,objectPixels=0,surfacePixels=0;
                for(int yy=y;yy<top;yy++)for(int xx=x;xx<right;xx++) {
                    var c=pixels[(origin.y+yy)*stride+origin.x+xx];if(c.a<128)continue;
                    count++;votes[Nearest(c)]++;
                    if(accessories) {
                        if(AccessoryMaterial(c,part))objectPixels++;
                        if(AccessoryBacking(c,part))surfacePixels++;
                    }
                }
                if(count*2>=(right-x)*(top-y)) {
                    int dominant=0;
                    for(int i=1;i<votes.Length;i++)if(votes[i]>votes[dominant])dominant=i;
                    int index=y/cell*columns+x/cell;
                    clusters[index]=dominant;
                    if(accessories){accessory[index]=objectPixels*2>=count;receivingSurface[index]=surfacePixels*2>=count;}
                }
            }
            CleanIsolatedShades(clusters,columns,rows,part==10,votes);
            for(int y=0;y<h;y+=cell)for(int x=0;x<w;x+=cell) {
                int right=Mathf.Min(w,x+cell),top=Mathf.Min(h,y+cell);
                int ink=clusters[y/cell*columns+x/cell];
                Color32 color=ink<0?default:Palette[ink];
                for(int yy=y;yy<top;yy++)for(int xx=x;xx<right;xx++)pixels[(origin.y+yy)*stride+origin.x+xx]=color;
            }
            if(accessories)OutlineAccessories(pixels,origin,stride,w,h,cell,columns,rows,accessory,receivingSurface);
        }
        static bool AccessoryMaterial(Color32 c,int part) {
            int hi=Mathf.Max(c.r,Mathf.Max(c.g,c.b)),lo=Mathf.Min(c.r,Mathf.Min(c.g,c.b));
            bool grey=hi-lo<32&&hi>=100;
            // The head's white/grey mask is separate from red hair, skin and
            // the red painted smile. At the waist, include leather and its buckle.
            return part==10?grey:grey||c.r>55&&c.r>c.g*1.12f&&c.g>c.b*1.05f;
        }
        static bool AccessoryBacking(Color32 c,int part) {
            // Put the stroke on the backing surface, preserving the small mask
            // and buckle. The red smile and dark eye holes never receive ink.
            return part==10?c.r>45&&c.g>=20&&c.r>c.g*1.7f&&c.r<c.g*4.5f&&c.b<c.g*.8f:
                c.g>c.r*1.05f&&c.g>c.b*1.05f;
        }
        static void OutlineAccessories(Color32[] pixels,Vector2Int origin,int stride,int width,int height,int cell,
                int columns,int rows,bool[] accessory,bool[] backing) {
            int stroke=Mathf.Max(1,Mathf.RoundToInt(cell*.5f));
            for(int cy=0;cy<rows;cy++)for(int cx=0;cx<columns;cx++) {
                int index=cy*columns+cx;if(accessory[index]||!backing[index])continue;
                bool left=cx>0&&accessory[index-1],right=cx+1<columns&&accessory[index+1];
                bool down=cy>0&&accessory[index-columns],up=cy+1<rows&&accessory[index+columns];
                if(!left&&!right&&!up&&!down)continue;
                int x0=cx*cell,y0=cy*cell,x1=Mathf.Min(width,x0+cell),y1=Mathf.Min(height,y0+cell);
                for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++) {
                    if(!(left&&x<x0+stroke||right&&x>=x1-stroke||down&&y<y0+stroke||up&&y>=y1-stroke))continue;
                    int at=(origin.y+y)*stride+origin.x+x;if(pixels[at].a>=128)pixels[at]=Palette[0];
                }
            }
        }
        static int Material(int ink) {
            if(ink<=1)return 0;
            if(ink<=5)return 1;
            if(ink<=8)return 2;
            if(ink<=13)return 3;
            if(ink<=19)return 4;
            return 5;
        }
        static void CleanIsolatedShades(int[] clusters,int columns,int rows,bool head,int[] votes) {
            var original=(int[])clusters.Clone();
            for(int y=1;y<rows-1;y++)for(int x=1;x<columns-1;x++) {
                int index=y*columns+x,ink=original[index];
                if(ink<2)continue; // Never erase outline, eyes or mouth ink.
                int material=Material(ink);
                if(head&&material!=1)continue; // Only hair shades; keep facial structure.
                System.Array.Clear(votes,0,votes.Length);
                int opaque=0,related=0,same=0;
                for(int dy=-1;dy<=1;dy++)for(int dx=-1;dx<=1;dx++) {
                    if(dx==0&&dy==0)continue;
                    int neighbor=original[(y+dy)*columns+x+dx];
                    if(neighbor<0)continue;
                    opaque++;if(neighbor==ink)same++;
                    if(Material(neighbor)!=material)continue;
                    related++;votes[neighbor]++;
                }
                // One pass only, well inside a surface of the same material.
                // Keep connected highlights, silhouettes and costume boundaries.
                if(same!=0||opaque<6||related<5)continue;
                int dominant=ink;
                for(int i=0;i<votes.Length;i++)if(votes[i]>votes[dominant])dominant=i;
                if(votes[dominant]>=3)clusters[index]=dominant;
            }
        }
        static int Nearest(Color32 pixel) {
            int closest=0,best=int.MaxValue;
            for(int i=0;i<Palette.Length;i++) {
                int r=pixel.r-Palette[i].r,g=pixel.g-Palette[i].g,b=pixel.b-Palette[i].b;
                int distance=2*r*r+3*g*g+2*b*b;
                if(distance<best){best=distance;closest=i;}
            }
            return closest;
        }
    }
}
