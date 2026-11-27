#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    [Serializable] internal sealed class MonsterPixelRampV53 {public int id;public string name;public string[] colors;}
    [Serializable] internal sealed class MonsterPixelBindingV53 {
        public string texture;public float xMin,yMin,xMax=1,yMax=1;
        public int material;public bool detail,forcePaint;
    }
    [Serializable] internal sealed class MonsterPixelRecipeV53 {
        public string id,ink="#1b0c1a",groundPolicy="preserve-authored-relative-y";
        public float height,units=1.25f,occlusionDepthWorldUnits=7;
        public int outerInkPixels=1,shadowClusterPixels=2,uvClusterPixels=3;
        public string[] groundedClips;
        // Null retains the face/eye naming default. An explicit empty array
        // lets a reviewed mixed atlas protect only its UV detail bindings.
        public string[] detailTextures;
        public MonsterPixelRampV53[] ramps;
        public MonsterPixelBindingV53[] bindings;
    }
    // Color reduction is stable in source UV space. The default ramps are fitted
    // to this character's original textures; reviewed UV regions can override
    // them without changing its model, pose, proportions or animation clock.
    internal sealed class MonsterPixelPaletteV53 {
        readonly Dictionary<int,Color32[]> ramps=new Dictionary<int,Color32[]>();
        readonly MonsterPixelRecipeV53 recipe;
        internal readonly Color32 ink;
        internal MonsterPixelPaletteV53(MonsterPixelRecipeV53 recipe,IEnumerable<Texture2D> textures) {
            this.recipe=recipe;ink=Parse(recipe.ink);
            var groups=new Dictionary<int,Dictionary<int,int>>();
            foreach(var texture in textures)foreach(var c in texture.GetPixels32()) {
                if(c.a<128||Max(c)<28)continue;
                int id=AutomaticMaterial(c),rgb=c.r<<16|c.g<<8|c.b;
                if(!groups.TryGetValue(id,out var histogram))groups[id]=histogram=new Dictionary<int,int>();
                histogram.TryGetValue(rgb,out int count);histogram[rgb]=count+1;
            }
            foreach(var pair in groups)ramps[pair.Key]=Fit(pair.Value);
            if(recipe.ramps!=null)foreach(var ramp in recipe.ramps){
                if(ramp.id<64||ramp.id>254||ramp.colors==null||ramp.colors.Length!=3)throw new ArgumentException("Reviewed ramp IDs must be 64..254 and contain three colors");
                ramps[ramp.id]=new[]{Parse(ramp.colors[0]),Parse(ramp.colors[1]),Parse(ramp.colors[2])};
            }
        }
        static Color32 Parse(string text){if(!ColorUtility.TryParseHtmlString(text,out var color))throw new ArgumentException("Invalid palette color "+text);return color;}
        internal static int Luma(Color32 c){return (c.r*2+c.g*3+c.b)/6;}
        static int Max(Color32 c){return Math.Max(c.r,Math.Max(c.g,c.b));}
        static int Distance(Color32 a,Color32 b){int r=a.r-b.r,g=a.g-b.g,bl=a.b-b.b;return r*r*2+g*g*3+bl*bl;}
        static int AutomaticMaterial(Color32 c){
            Color.RGBToHSV(c,out float h,out float s,out float v);
            // Neutrals remain separate from saturated costume accents. Both
            // warm and cool grays keep their source hue instead of turning tan.
            if(s<.18f)return c.b>c.r+5?2:1;
            return 3+Math.Min(11,Mathf.FloorToInt((h+.0416667f)%1*12));
        }
        static Color32[] Fit(Dictionary<int,int> histogram){
            var colors=new List<KeyValuePair<int,int>>(histogram);
            Color32 Decode(int rgb){return new Color32((byte)(rgb>>16),(byte)(rgb>>8),(byte)rgb,255);}
            colors.Sort((a,b)=>Luma(Decode(a.Key)).CompareTo(Luma(Decode(b.Key))));
            long total=0;foreach(var p in colors)total+=p.Value;
            var result=new Color32[3];
            for(int k=0;k<3;k++){long current=0,target=(long)(total*(.15+.35*k));foreach(var p in colors){current+=p.Value;if(current>=target){result[k]=Decode(p.Key);break;}}}
            // Weighted RGB centroids avoid allocating one object per texel.
            for(int iteration=0;iteration<8;iteration++){
                var r=new long[3];var g=new long[3];var b=new long[3];var n=new long[3];
                foreach(var p in colors){var c=Decode(p.Key);int best=0;for(int k=1;k<3;k++)if(Distance(c,result[k])<Distance(c,result[best]))best=k;r[best]+=(long)c.r*p.Value;g[best]+=(long)c.g*p.Value;b[best]+=(long)c.b*p.Value;n[best]+=p.Value;}
                for(int k=0;k<3;k++)if(n[k]>0)result[k]=new Color32((byte)(r[k]/n[k]),(byte)(g[k]/n[k]),(byte)(b[k]/n[k]),255);
            }
            Array.Sort(result,(a,b)=>Luma(a).CompareTo(Luma(b)));return result;
        }
        MonsterPixelBindingV53 Binding(string texture,Vector2 uv){
            if(recipe.bindings==null)return null;
            // These are top-down coordinates on the source image. The native
            // shader flips V once, so GetPixels' bottom-up row is converted here.
            float x=uv.x-Mathf.Floor(uv.x),y=1-(uv.y-Mathf.Floor(uv.y));
            for(int i=recipe.bindings.Length-1;i>=0;i--){var b=recipe.bindings[i];if(texture.IndexOf(b.texture??"",StringComparison.OrdinalIgnoreCase)>=0&&x>=b.xMin&&x<=b.xMax&&y>=b.yMin&&y<=b.yMax)return b;}
            return null;
        }
        internal void Paint(Color32 original,string texture,Vector2 uv,out Color32 color,out byte material,out bool detail){
            var binding=Binding(texture,uv);detail=binding!=null&&binding.detail;
            if(original.a<128){color=default;material=0;return;}
            if(Max(original)<28&&(binding==null||!binding.forcePaint)){color=ink;material=0;return;}
            int id=binding!=null?binding.material:AutomaticMaterial(original);
            if(!ramps.TryGetValue(id,out var ramp))throw new ArgumentException("Missing material ramp "+id+" for "+texture);
            int best=0;for(int k=1;k<ramp.Length;k++)if(Distance(original,ramp[k])<Distance(original,ramp[best]))best=k;
            color=ramp[best];material=(byte)id;
        }
        internal Color32 Shade(Color32 original,int material,int band){
            if(material==0||band>=2||!ramps.TryGetValue(material,out var ramp))return original;
            var color=ramp[Mathf.Clamp(band,0,1)];return Luma(original)>Luma(color)?color:original;
        }
        internal MonsterPixelRampV53[] ExportRamps(){
            var result=new List<MonsterPixelRampV53>();foreach(var pair in ramps){var colors=new string[3];for(int i=0;i<3;i++)colors[i]="#"+ColorUtility.ToHtmlStringRGB(pair.Value[i]);result.Add(new MonsterPixelRampV53{id=pair.Key,name=pair.Key<64?"source-fit-"+pair.Key:"reviewed-"+pair.Key,colors=colors});}return result.ToArray();
        }
    }
}
#endif
