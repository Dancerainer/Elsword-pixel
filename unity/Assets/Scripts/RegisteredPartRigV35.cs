using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    // Loads a registered rig into the same isolated sprite library used by Fairy.
    // The source identity supplies its own pinned document and validates its own aliases.
    public static class RegisteredPartRigV35 {
        [Serializable] sealed class Page {
            public string resource,pngSha,rgbaSha,canonicalResource;
            public int w,h,selectedAlpha64,allAlpha64;
        }
        [Serializable] sealed class PageCatalog {
            public string schema,id,poseSha;
            public Page[] pages;
        }
        static string Digest(byte[] bytes) {
            using (var sha=SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();
        }
        static void Require(bool ok,string why) { if (!ok) throw new InvalidOperationException("Registered parts V35: "+why); }
        static void ReleasePage(UnityEngine.Object page) {
            if(Application.isPlaying)UnityEngine.Object.Destroy(page);
            else UnityEngine.Object.DestroyImmediate(page);
        }
        static Texture2D LoadCanonical(Page p,string root) {
            Require(p.canonicalResource==p.resource+"-source" && p.canonicalResource.StartsWith(root,StringComparison.Ordinal),"canonical page path "+p.resource);
            var asset=Resources.Load<TextAsset>(p.canonicalResource);
            Require(asset,"original PNG and texels "+p.canonicalResource);
            var blob=asset.bytes;
            Require(blob.Length>12 && System.Text.Encoding.ASCII.GetString(blob,0,8)=="ELPXRGB1","canonical page header "+p.resource);
            int pngSize=BitConverter.ToInt32(blob,8);
            Require(pngSize>8 && pngSize<blob.Length-12,"canonical original PNG size "+p.resource);
            using(var sha=SHA256.Create())
                Require(BitConverter.ToString(sha.ComputeHash(blob,12,pngSize)).Replace("-","").ToLowerInvariant()==p.pngSha,
                    "encoded original PNG hash "+p.resource);
            var raw=new byte[checked(p.w*p.h*4)];int written=0;
            using(var container=new MemoryStream(blob,12+pngSize,blob.Length-12-pngSize,false))
            using(var gz=new GZipStream(container,CompressionMode.Decompress)){
                while(written<raw.Length){int got=gz.Read(raw,written,raw.Length-written);if(got==0)break;written+=got;}
                Require(written==raw.Length && gz.ReadByte()==-1,"complete canonical texels "+p.resource);
            }
            Require(Digest(raw)==p.rgbaSha,"source RGBA bytes "+p.resource);
            int alpha=0;for(int i=3;i<raw.Length;i+=4)if(raw[i]>=64)alpha++;
            Require(alpha==p.allAlpha64,"source alpha coverage "+p.resource);
            var texture=new Texture2D(p.w,p.h,TextureFormat.RGBA32,false,false){name=p.resource,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
            try {texture.LoadRawTextureData(raw);texture.Apply(false,false);return texture;}
            catch{ReleasePage(texture);throw;}
        }
        static Texture2D LoadPage(Page p,string root,string[] sharedPages) {
            Require(p!=null && !string.IsNullOrEmpty(p.resource) && (p.resource.StartsWith(root,StringComparison.Ordinal) || sharedPages!=null && Array.IndexOf(sharedPages,p.resource)>=0) && p.w>0 && p.h>0,"source page path");
            if(!string.IsNullOrEmpty(p.canonicalResource)){
                var decoded=LoadCanonical(p,root);
                try{ValidatePagePixels(p,decoded);return decoded;}
                catch{ReleasePage(decoded);throw;}
            }
            var atlas=Resources.Load<Texture2D>(p.resource);
            ValidatePagePixels(p,atlas);
            return atlas;
        }
        static void ValidatePagePixels(Page p,Texture2D atlas) {
            Require(atlas && atlas.width==p.w && atlas.height==p.h && atlas.isReadable && atlas.isDataSRGB &&
                atlas.mipmapCount==1 && atlas.filterMode==FilterMode.Point && atlas.wrapMode==TextureWrapMode.Clamp &&
                (atlas.format==TextureFormat.RGBA32 || atlas.format==TextureFormat.ARGB32),"source page format "+p.resource);
            var colors=atlas.GetPixels32(); var bytes=new byte[4*colors.Length]; int alpha=0;
            for (int i=0;i<colors.Length;i++) {
                var c=colors[i]; bytes[i*4]=c.r;bytes[i*4+1]=c.g;bytes[i*4+2]=c.b;bytes[i*4+3]=c.a;
                if(c.a>=64)alpha++;
            }
            Require(Digest(bytes)==p.rgbaSha && alpha==p.allAlpha64,"original RGBA "+p.resource);
        }
        // An instance supplies its own name, order, pose, mirror and scale. The
        // source part solely owns texture coverage, pivot, UVs and lifetime.
        internal static FairyPartLayoutV33 ResolveLayout(FairyPartLayoutV33[] parts, int i) {
            var part = parts[i];
            Require(part != null && !string.IsNullOrEmpty(part.name) && part.scale > 0, "registered part " + i);
            if (part.instanceOf == 0) return part;
            int owner = part.instanceOf - 1;
            Require(owner >= 0 && owner < i && parts[owner] != null && parts[owner].instanceOf == 0 &&
                part.alpha64 == 0 && (part.geometry == null || part.geometry.Length == 0), "invalid instance owner " + part.name);
            var original = parts[owner];
            part.page = original.page;
            part.rect = original.rect;
            part.pivot = original.pivot;
            part.geometry = original.geometry;
            part.indices = original.indices;
            part.visibleHull = original.visibleHull;
            return part;
        }

        public static FairyPoseLibraryV33 Load(string resourceRoot,string id,string pagesHash,
            Action<FairyPoseLibraryV33> validateSource,string[] sharedPages=null) {
            var manifest=Resources.Load<TextAsset>(resourceRoot+"pages");
            Require(manifest && Digest(manifest.bytes)==pagesHash,"pinned source-page catalog");
            var pages=JsonUtility.FromJson<PageCatalog>(manifest.text);
            Require(pages!=null && pages.schema=="registered-part-pages-v35" && pages.id==id && pages.pages!=null && pages.pages.Length>0,"registered source identity");
            var source=Resources.Load<TextAsset>(resourceRoot+"poses");
            Require(source && Digest(source.bytes)==pages.poseSha,"pinned art and bone registration");
            var lib=JsonUtility.FromJson<FairyPoseLibraryV33>(source.text);
            Require(lib!=null && lib.schema=="original-bone-part-rig-v35" && lib.rigName==id && lib.parts!=null &&
                lib.parts.Length>0 && lib.parts.Length<=64 && lib.foot.Length==2 && lib.UnitsPerPixel>0,"source rig layout");
            validateSource(lib);
            var textures=new Texture2D[pages.pages.Length];
            try {
                for(int i=0;i<textures.Length;i++){
                    textures[i]=LoadPage(pages.pages[i],resourceRoot,sharedPages);
                    if(!string.IsNullOrEmpty(pages.pages[i].canonicalResource)){
                        if(lib.ownedDecodedPages==null)lib.ownedDecodedPages=new Texture2D[textures.Length];
                        lib.ownedDecodedPages[i]=textures[i];
                    }
                }
                var totals=new int[textures.Length];
                for (int i = 0; i < lib.parts.Length; ++i) {
                    var part = ResolveLayout(lib.parts, i);
                    Require(part.page>=0 && part.page<textures.Length && part.geometry!=null && part.geometry.Length%8==0 &&
                        part.rect.Length==4 && part.pivot.Length==2 && part.visibleHull.Length>=6 && part.visibleHull.Length%2==0 && part.scale>0,"part geometry");
                    var page=textures[part.page];
                    Require(part.rect[0]>=0 && part.rect[1]>=0 && part.rect[2]>0 && part.rect[3]>0 &&
                        part.rect[0]+part.rect[2]<=page.width && part.rect[1]+part.rect[3]<=page.height,"page bounds "+part.name);
                    if (part.instanceOf == 0) totals[part.page] += part.alpha64;
                }
                for(int i=0;i<totals.Length;i++)Require(totals[i]==pages.pages[i].selectedAlpha64,"selected foreground coverage "+i);
                lib.PrepareRegisteredParts(textures);return lib;
            }
            catch {
                lib.ReleaseOwned(ReleasePage);
                throw;
            }
        }
    }
}
