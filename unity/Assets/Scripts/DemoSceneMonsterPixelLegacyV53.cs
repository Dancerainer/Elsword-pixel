using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        readonly Dictionary<string,Func<Texture2D>> privateAtlasLoadersV53=new Dictionary<string,Func<Texture2D>>(StringComparer.Ordinal);
        void RegisterPrivateAtlasV53(string key,Func<Texture2D> load){privateAtlasLoadersV53.Add(key,load);}
        bool TryPrivateAtlasV53(string key,out Texture2D texture){
            texture=null;if(!privateAtlasLoadersV53.TryGetValue(key,out var load))return false;
            texture=load();if(!texture)throw new InvalidOperationException("Missing registered private atlas "+key);return true;
        }
        void ReleasePrivateAtlasV53(string key){privateAtlasLoadersV53.Remove(key);atlasTextures.Remove(key);}
        // Keep every authored key and selector available. Migrated actors only
        // decode an old whole image if an existing consumer actually requests it.
        // Native objects still belong to the original route loading transaction.
        sealed class DeferredWholeSpritesV53 : Dictionary<string,Sprite>, IDictionary<string,Sprite>, IDictionary {
            readonly Dictionary<string,Func<Sprite>> factories = new Dictionary<string,Func<Sprite>>(StringComparer.Ordinal);
            internal DeferredWholeSpritesV53() : base(StringComparer.Ordinal) { }
            internal int DeferredCount { get { return factories.Count; } }
            internal int ResidentCount { get { return Count-factories.Count; } }
            internal void Register(string key,bool defer,Func<Sprite> create) {
                if(create==null)throw new ArgumentNullException(nameof(create));
                if(defer){base.Add(key,null);factories.Add(key,create);}
                else base.Add(key,create());
            }
            Sprite Resolve(string key) {
                var result=base[key];
                if(factories.TryGetValue(key,out var create)) {
                    result=create();
                    if(!result)throw new InvalidOperationException("Missing original whole fallback: "+key);
                    base[key]=result;factories.Remove(key);
                }
                return result;
            }
            public new Sprite this[string key] {get{return Resolve(key);}set{base[key]=value;factories.Remove(key);}}
            Sprite IDictionary<string,Sprite>.this[string key] {get{return Resolve(key);}set{this[key]=value;}}
            object IDictionary.this[object key] {get{return key is string id&&ContainsKey(id)?Resolve(id):null;}set{this[(string)key]=(Sprite)value;}}
            public new bool TryGetValue(string key,out Sprite value) {
                if(!base.ContainsKey(key)){value=null;return false;}
                value=Resolve(key);return true;
            }
            bool IDictionary<string,Sprite>.TryGetValue(string key,out Sprite value){return TryGetValue(key,out value);}
            // Count, Keys and enumeration remain metadata-only; teardown must not
            // materialize images that were never requested by the original owner.
            public new void Clear(){factories.Clear();base.Clear();}
        }

        bool DeferPrivateFramesV53(Library library,Func<FrameData,Texture2D> originalPage,List<UnityEngine.Object> owner,bool sourceInk) {
            if(!DeferMonsterPixelCarrierV53(library.data.id))return false;
            foreach(var clip in library.data.clips){library.clips.Add(clip.id,clip);library.frames.Add(clip.id,null);}
            library.frames.open=id=>{
                var clip=library.clips[id];
                // These private owners use only this explicit V45 substitution.
                // Plain never used it, nor the ordinary catalog paint selector.
                var ink=sourceInk?SourceMotionInkV45.For(library.data,clip):null;
                var pictures=ink??clip;
                var unique=new Dictionary<FrameData,Sprite>();
                var pages=new Dictionary<string,Texture2D>(StringComparer.Ordinal);
                return new MobileClip(this,library.frames,library.data,clip,index=>{
                    var frame=pictures.frames[index];
                    if(unique.TryGetValue(frame,out var sprite))return sprite;
                    Texture2D texture;
                    if(ink!=null){
                        if(!pages.TryGetValue(frame.texture,out texture)){
                            texture=Resources.Load<Texture2D>(frame.texture);
                            if(!texture)throw new InvalidOperationException("Missing private source-motion page "+frame.texture);
                            pages.Add(frame.texture,texture);
                        }
                    }else texture=originalPage(frame);
                    if(!texture||frame.x<0||frame.y<0||frame.x+frame.w>texture.width||frame.y+frame.h>texture.height)
                        throw new InvalidOperationException("Private source pose outside "+library.data.id+"/"+id);
                    sprite=Sprite.Create(texture,new Rect(frame.x,texture.height-frame.y-frame.h,frame.w,frame.h),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
                    owner.Add(sprite);if(ink!=null)RegisterTightMotionBoundsV45(sprite);
                    sprite.name=library.data.id+"/"+id+"/"+index;
                    spriteOffsets[sprite]=new Vector2(frame.w*.5f-frame.anchorX,frame.anchorY-frame.h*.5f);
                    unique.Add(frame,sprite);return sprite;
                });
            };
            // The private route transaction owns these frames/pages and destroys
            // them when its library is removed. Do not enroll in catalog eviction.
            return true;
        }

        void ReadPrivateWholeMemoryV53(out int declared,out int deferred,out int resident) {
            declared=deferred=resident=0;
            foreach(var bank in new[]{factoryProductSpritesR1179,coreElementFireBWholeSpritesV21,coreElementIceBWholeSpritesV21,coreRavenBWholeSpritesV21,coreHealerWholeSpritesV21,coreWatchWholeSpritesV21,coreGuardWholeSpritesV21,corePlantWholeSpritesV21,coreWallyWholeSpritesR198,coreSprayWholeSpritesR203,coreSheriffWholeSpritesR227,plainWatchSpritesR1330,plainVariableSpritesR1330}){
                declared+=bank.Count;deferred+=bank.DeferredCount;resident+=bank.ResidentCount;
            }
        }
    }
}
