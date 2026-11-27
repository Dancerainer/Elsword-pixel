using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    // The director poses, crop rects and mesh skins remain source-owned; this resolver only selects registered paint.
    internal static class DirectorPaintV41 {
        internal sealed class Pin {
            internal readonly string key,id,sourceRgba,targetRgba;
            internal Pin(string key,string id,string sourceRgba,string targetRgba){
                this.key=key;this.id=id;this.sourceRgba=sourceRgba;this.targetRgba=targetRgba;
            }
        }
        internal static readonly Pin[] Pins={
            new Pin("DragonNestShaman","texture-008","2652286f232e1af48bbd1caff8abf214b05acf47fb83ed59b0b8508a6a0ac53f","446a071b6c7c3b5be22b226d5ebe4491945219375ff671492398451ec3bb97ba"),
            new Pin("textures/Mesh_Raven_Boss_Foot.png","texture-084","39fa0d8ae701f7426128aa3adc0231d6955187704169e8b0d912fa306afb464a","b62c5e88a54b5f0661e212653fe5dd01c5e9c1fde60f2a90a2724f8308a79366"),
            new Pin("textures/Mesh_Raven_Boss_Face.png","texture-083","24fd6d08053655eb931f6e4509834e26f583d5c281d62f0ea3c9e7627a0aab09","f5449d647d2f6e487d4845a28cf928c4811962b1816b00ce40c6eeaffe0cd07e"),
            new Pin("textures/Mesh_Raven_Boss_Hair.png","texture-085","e7f058ab3be4c6fdc31afef3757a0e9020920b3ea63a5fd4aa8865d17278abff","f1ba98dfe3c0ea9063531a77237c8f58aa34c8403ae6a7a8468c011c1c60fd5c"),
            new Pin("textures/Mesh_Raven_Boss_Upbody.png","texture-089","e41ce7ca4114389bc660dcb9e18694955f59394d4d1612b7a1a1b890bdfbcfaf","8d3aabe5b6955f9f216f858e5e890139dcd22fcffaeacd044bd14332672823de"),
            new Pin("textures/Mesh_Raven_Boss_Lowbody.png","texture-087","5b82e7d9747f06d073ebcd9aea817ce6bf2226a473df85a53031d0acecc38d10","883955ec16d461eae926b8c33dd47745d3a47d7d6690554c237934f5cd32b94e"),
            new Pin("textures/Mesh_Raven_Cash_Weapon.png","texture-090","008493c9c0e8cde418ab8616ff6cf612d0e16c3d3c5c0e59c42e50c4ac8b2983","a4869ceb08fa8e67e9bd1a7fd9f02a60dd88c8dde8637a7258267975309228e1"),
            new Pin("textures/Mesh_Raven_Boss_Left_Hand.png","texture-086","488695c095b2bb1ef1619e2cfcaa46a2f33a70b8b7bc7b5ae878354e4e3b688c","97aa29d4e4c3497acaaba5bdf3affd1af4753c235b081d5469a625093454e6a3"),
            new Pin("textures/Mesh_Raven_Boss_Right_Hand.png","texture-088","0bd16df316bf7356a3c2de4c504de71ec39be8121b56ec4633a2cdac643bec9f","6674fd4fbb59e4a2740a9fb301aa656f4dc3fb04b48db8fbc94cf0552118896d"),
        };
        internal static string Rgba(Texture2D texture){
            Color32[] pixels=texture.GetPixels32();byte[] bytes=new byte[pixels.Length*4];
            for(int y=0;y<texture.height;y++)for(int x=0;x<texture.width;x++){
                var c=pixels[(texture.height-1-y)*texture.width+x];int i=(y*texture.width+x)*4;
                bytes[i]=c.r;bytes[i+1]=c.g;bytes[i+2]=c.b;bytes[i+3]=c.a;
            }
            using(var digest=SHA256.Create())return BitConverter.ToString(digest.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();
        }
        internal static Texture2D Target(string key,Texture2D original){
            var pin=Array.Find(Pins,p=>p.key==key);
            if(pin==null||!original||!original.isReadable||Rgba(original)!=pin.sourceRgba)
                throw new InvalidOperationException("Unregistered cinematic source "+key);
            var painted=Resources.Load<Texture2D>("Pixel/monster-texture-redraw-v22/"+pin.id);
            if(!painted||!painted.isReadable||original.width!=painted.width||original.height!=painted.height||
               painted.filterMode!=FilterMode.Point||Rgba(painted)!=pin.targetRgba)
                throw new InvalidOperationException("Cinematic paint unavailable or changed "+key);
            return painted;
        }
    }
    public sealed partial class DemoScene {
        Texture2D dragonDirectorOriginalV41,dragonDirectorPaintV41;
        readonly Dictionary<Rect,Sprite> dragonDirectorSpritesV41=new Dictionary<Rect,Sprite>();
        Sprite dragonDirectorSourceFrameV41;
        void ApplyDragonDirectorPaintV41(SpriteRenderer view){
            var source=view.sprite;if(!source)return;
            if(!dragonDirectorPaintV41){
                dragonDirectorOriginalV41=source.texture;
                dragonDirectorPaintV41=DirectorPaintV41.Target("DragonNestShaman",source.texture);
            }
            if(source.texture!=dragonDirectorOriginalV41||source.packed)throw new InvalidOperationException("Dragon director carrier changed");
            var rect=source.rect;
            if(!dragonDirectorSpritesV41.TryGetValue(rect,out var selected)){
                var pivot=new Vector2(source.pivot.x/rect.width,source.pivot.y/rect.height);
                selected=Sprite.Create(dragonDirectorPaintV41,rect,pivot,source.pixelsPerUnit,0,SpriteMeshType.FullRect,source.border);
                selected.name="Dragon director / registered paint / "+(int)rect.x+","+(int)rect.y;
                var originalVertices=source.vertices;var selectedVertices=selected.vertices;
                var originalTriangles=source.triangles;var selectedTriangles=selected.triangles;
                bool same=originalVertices.Length==selectedVertices.Length&&originalTriangles.Length==selectedTriangles.Length&&
                    source.bounds==selected.bounds&&source.pivot==selected.pivot&&source.border==selected.border;
                for(int i=0;same&&i<originalVertices.Length;i++)same=originalVertices[i]==selectedVertices[i];
                for(int i=0;same&&i<originalTriangles.Length;i++)same=originalTriangles[i]==selectedTriangles[i];
                if(!same){DragonCountdownDestroyObjectV20(selected);
                    throw new InvalidOperationException("Dragon director painted sprite no longer matches original full rectangle");}
                dragonDirectorSpritesV41.Add(rect,selected);
            }
            dragonDirectorSourceFrameV41=source;view.sprite=selected;
        }
        void ReleaseDragonDirectorPaintV41(){
            foreach(var frame in dragonDirectorSpritesV41.Values)if(frame)DragonCountdownDestroyObjectV20(frame);
            dragonDirectorSpritesV41.Clear();dragonDirectorPaintV41=null;dragonDirectorOriginalV41=null;dragonDirectorSourceFrameV41=null;
        }
    }
}

