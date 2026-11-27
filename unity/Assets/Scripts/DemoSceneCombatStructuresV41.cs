using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        const string StructureRootV41="Pixel/feedback-v41/structures/";
        [Serializable] sealed class PressPageV41 { public string npc,key,source; public int x,y,w,h; }
        [Serializable] sealed class PressMapV41 { public PressPageV41[] frames; }
        Dictionary<string,PressPageV41> pressRectsV41;
        Texture2D pressPageV41;
        static Texture2D LoadCastleBunkerV41(string path){
            if(path!="Pixel/monsters/castle-center/bunker/atlas")return null;
            var texture=Resources.Load<Texture2D>(StructureRootV41+"castle-bunker");
            if(!texture)throw new InvalidOperationException("Missing castle bunker art.");return texture;
        }
        Sprite PaintedPressV41(string npc,FactoryProductKeyR1168 native){
            if(npc!="NUI_FACTORY_PRESS"&&npc!="NUI_FACTORY_PRESS3")return null;
            if(pressRectsV41==null){
                var doc=Resources.Load<TextAsset>(StructureRootV41+"press-map");
                var book=doc?JsonUtility.FromJson<PressMapV41>(doc.text):null;
                pressPageV41=Resources.Load<Texture2D>(StructureRootV41+"press");
                if(!pressPageV41||book==null||book.frames==null||book.frames.Length!=17)throw new InvalidOperationException("Incomplete press art.");
                pressRectsV41=new Dictionary<string,PressPageV41>(StringComparer.Ordinal);
                foreach(var p in book.frames)pressRectsV41.Add(p.npc+"/"+p.key,p);
            }
            if(!pressRectsV41.TryGetValue(npc+"/"+native.key,out var rect)||rect.source!=native.file.sha256||rect.w!=native.width||rect.h!=native.height||
                rect.x<0||rect.y<0||rect.x+rect.w>pressPageV41.width||rect.y+rect.h>pressPageV41.height)
                throw new InvalidOperationException("Press art no longer matches its source pose: "+npc+"/"+native.key);
            var picture=Sprite.Create(pressPageV41,new Rect(rect.x,pressPageV41.height-rect.y-rect.h,rect.w,rect.h),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
            picture.name=npc+"/"+native.key;return picture;
        }
    }
}
