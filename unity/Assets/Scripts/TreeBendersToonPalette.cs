#if UNITY_EDITOR
using System;
using UnityEngine;

namespace ElswordPixel {
    // The five native textures have separate UV islands for the face, hair,
    // garments and cleaver. Keep those materials distinct before reducing
    // each to a three-value ramp; a global RGB nearest match loses that intent.
    internal static class TreeBendersToonPalette {
        internal const int Ink=0,Skin=1,Hair=2,Cloth=3,White=4,Red=5,Blue=6,Blade=7,Gold=8;
        static readonly Color32 Outline=Rgb(0x1b0c1a);
        // Ordered darkest to lightest. White follows the chicken's warm cream
        // and dusty pink feather shadows; dark cloth uses a related plum ramp.
        static readonly Color32[][] Ramps={
            new[]{Outline,Outline,Outline},
            new[]{Rgb(0xb97868),Rgb(0xf3b985),Rgb(0xffe0aa)},
            new[]{Rgb(0xa74634),Rgb(0xe87743),Rgb(0xffaf65)},
            new[]{Rgb(0x483145),Rgb(0x6b5667),Rgb(0x927487)},
            new[]{Rgb(0xb98488),Rgb(0xd5a7a5),Rgb(0xfdf0e3)},
            new[]{Rgb(0x943340),Rgb(0xd65348),Rgb(0xf9895e)},
            new[]{Rgb(0x34475c),Rgb(0x567d99),Rgb(0x94b5c6)},
            new[]{Rgb(0x6e4c4d),Rgb(0xa27559),Rgb(0xcba982)},
            new[]{Rgb(0x805337),Rgb(0xc58e49),Rgb(0xffdb83)}
        };
        static Color32 Rgb(uint rgb){return new Color32((byte)(rgb>>16),(byte)(rgb>>8),(byte)rgb,255);}
        static Color32 Tone(int material,int level){return Ramps[material][level];}
        static int Brightness(Color32 c){return 2*c.r+3*c.g+c.b;}
        static bool Is(string name,string ending){return name.EndsWith(ending,StringComparison.OrdinalIgnoreCase);}

        // The offline light mask supplies 0=shadow, 1=base, 2=unrestricted.
        // This is a ceiling on existing paint, not a new albedo assignment:
        // deep boots, source ink and already shaded texels can never brighten.
        // The cold sharpened edge remains a graphic identity accent.
        internal static Color32 Shade(Color32 color,int material,int band) {
            if(color.a==0||material<=Ink||material>Gold||band>=2)return color;
            if(color.r==Outline.r&&color.g==Outline.g&&color.b==Outline.b)return color;
            if(material==Blade&&color.r==228&&color.g==224&&color.b==213)return color;
            var target=Tone(material,Mathf.Clamp(band,0,1));
            if(Brightness(color)<=Brightness(target))return color;
            target.a=color.a;return target;
        }

        internal static int MaterialId(Color32 source,string textureName,Vector2 uv) {
            if(source.a<128)return Ink;
            int r=source.r,g=source.g,b=source.b,high=Math.Max(r,Math.Max(g,b));
            // GetPixels uses bottom-up rows. The UV island bounds below are
            // measured on the source PNG viewed conventionally, from its top.
            float x=uv.x-Mathf.Floor(uv.x),y=1-(uv.y-Mathf.Floor(uv.y));
            bool face=Is(textureName,"face");
            bool repeatedPlate=Is(textureName,"bodyB")&&x<.675f&&y>.37f&&y<.665f;
            // Most dark clothing is 40..80, so never classify it all as ink.
            // Facial eyebrows, eye rims, mouth and scar use darker brown ink.
            // A small scratched plate is repeated many times over the waist.
            // Its texture border must be a cloth shade, not dozens of tiny ink
            // boxes. The baked material/depth contour defines the whole object.
            if(high<28&&!repeatedPlate||face&&r<111&&g<82&&b<65)return Ink;
            if(face) {
                if(r>142&&g>142&&b>125&&high-Math.Min(r,Math.Min(g,b))<46)return White;
                if(b>r*1.04f&&g>r*.95f&&b>85)return Blue;
                return Skin;
            }
            if(Is(textureName,"bodyA")) {
                if(x>.755f&&y<.25f)return r>g*1.15f&&g>b*1.08f?Skin:White;
                if(x>.425f&&x<.755f&&y>.12f&&y<.355f&&r>g*1.12f&&r>100)return Gold;
                if(y<.12f&&b>r*1.12f&&g>r*1.08f)return Blue;
                return Cloth;
            }
            if(Is(textureName,"bodyB")) {
                if(x<.435f&&y>.655f)return Skin;
                // The native Wait vertices below Y=15 identify a boot upper
                // at x=.19..49,y=.014..362 and its brown sole strip below.
                // That strip shares hair-like RGB, but is a distinct UV island.
                if(x>.18f&&x<.435f&&y<.375f||x>.18f&&x<.505f&&y>.31f&&y<.375f||
                   x>.435f&&x<.65f&&y>.915f)return Cloth;
                if(x<.675f&&y>.37f&&y<.665f)return Cloth;
                if(x<.165f&&y<.405f)return Gold;
                if(r<g*1.42f&&r-b<65)return Cloth;
                return Hair;
            }
            if(Is(textureName,"bodyC")) {
                if(y<.405f)return x<.418f?Blue:Red;
                if(x<.35f&&r<95&&g<90&&b>g)return Ink;
                // Trouser cloth and the wrapped shin share a warm-white ramp;
                // the original dark band separations remain protected ink.
                return White;
            }
            if(Is(textureName,"Weapon")) {
                // The top chain, middle cross-band and left grip are brass.
                // The cold pale strip at right is the sharpened blade edge.
                if(b>r*1.02f&&g>r*.95f)return Blade;
                if(y<.195f||y>.425f&&y<.59f||x<.17f&&y>.425f||x>.87f)return Gold;
                return Blade;
            }
            return Cloth;
        }

        internal static Color32 Colorize(Color32 source,string textureName,Vector2 uv) {
            if(source.a<128)return default;
            int material=MaterialId(source,textureName,uv),r=source.r,g=source.g,b=source.b;
            switch(material) {
                case Ink:return Outline;
                // Two substantial skin planes, with the brightest tone reserved
                // for small highlights instead of filling whole limbs yellow.
                case Skin:return Tone(Skin,r<185?0:r<236?1:2);
                case Hair:return Tone(Hair,r<129?0:r<176?1:2);
                case Cloth:{
                    float x=uv.x-Mathf.Floor(uv.x),y=1-(uv.y-Mathf.Floor(uv.y));
                    if(Is(textureName,"bodyB")&&x>.435f&&x<.65f&&y>.915f)
                        return r<130?Rgb(0x302935):r<170?Rgb(0x443845):Rgb(0x61505a);
                    int v=Math.Max(r,Math.Max(g,b));return Tone(Cloth,v<49?0:v<85?1:2);
                }
                // Keep most trousers cream. Broad pink shade coverage is
                // supplied by the stable offline light bands, not tiny folds.
                case White:{int light=(r+g+b)/3;return Tone(White,light<150?0:light<190?1:2);}
                case Red:return Tone(Red,r<125?0:r<194?1:2);
                case Blue:return Tone(Blue,b<83?0:b<132?1:2);
                case Blade:
                    if(b>r*1.02f&&g>r*.95f)return Rgb(0xe4e0d5);
                    // Rust mottling is not a structural feature. Keep a broad
                    // blade face and only the deepest continuous recess shade;
                    // the native dark ornaments and brass border stay separate.
                    return Tone(Blade,r<96?0:1);
                case Gold:return Tone(Gold,r<135?0:r<218?1:2);
                default:return Outline;
            }
        }
    }
}
#endif
