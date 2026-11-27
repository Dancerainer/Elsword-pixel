using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        bool StreamMobileArt { get { return Application.platform == RuntimePlatform.Android; } }

        // Product routes own complete sprite arrays. Mobile catalog clips can draw
        // one exposure at a time; consumers explicitly requesting an array get every frame.
        sealed class AnimationFrames : Dictionary<string, Sprite[]> {
            internal Func<string, MobileClip> open;
            readonly Dictionary<string,MobileClip> active = new Dictionary<string,MobileClip>();

            MobileClip Open(string id) {
                if (!active.TryGetValue(id,out var clip)) {
                    clip=open(id);
                    active.Add(id,clip);
                    base[id]=clip.frames;
                }
                return clip;
            }
            MobileClip Deferred(string id,Sprite[] assigned) {
                if(open==null)return null;
                if(assigned==null)return Open(id);
                return active.TryGetValue(id,out var clip)&&ReferenceEquals(clip.frames,assigned)?clip:null;
            }
            public new Sprite[] this[string id] {
                get {
                    var values=base[id];
                    return Deferred(id,values)?.Complete()??values;
                }
                set { base[id]=value; }
            }
            public new bool TryGetValue(string id,out Sprite[] values) {
                if (!base.TryGetValue(id,out values)) return false;
                var clip=Deferred(id,values);if(clip!=null)values=clip.Complete();
                return true;
            }
            internal bool TryFrameCount(string id,out int count) {
                count=0;if(!base.TryGetValue(id,out var assigned))return false;
                var values=assigned??(open!=null?Open(id).frames:null);
                if(values==null)return false;
                count=values.Length;return true;
            }
            internal Sprite At(string id,int requested,out int index) {
                var values=base[id];
                var clip=Deferred(id,values);
                if (clip==null) {
                    index=Mathf.Clamp(requested,0,values.Length-1);
                    return values[index];
                }
                index=Mathf.Clamp(requested,0,clip.frames.Length-1);
                return clip.At(index);
            }
            void LoadAll() {
                if(open==null)return;
                foreach (string id in new List<string>(Keys)) _=this[id];
            }
            public new ValueCollection Values { get { LoadAll(); return base.Values; } }
            public new Enumerator GetEnumerator() { LoadAll(); return base.GetEnumerator(); }
            internal void Collect(List<MobileClip> result) { result.AddRange(active.Values); }
            internal void KeepResident(string id,int room) {
                if(active.TryGetValue(id,out var clip))clip.room=room;
            }
            internal void RemoveResident(string id) {
                if(ReferenceEquals(base[id],active[id].frames))base[id]=null;
                active.Remove(id);
            }
        }

        sealed class MobileClip {
            readonly DemoScene scene;
            readonly Func<int,Sprite> privateFrame;
            internal readonly AnimationFrames bank;
            internal readonly SpriteLibraryData library;
            internal readonly AnimationData clip, drawing;
            internal readonly string fallback;
            internal readonly Sprite[] frames;
            internal readonly HashSet<string> pages=new HashSet<string>();
            internal int room, lastFrame;
            int built;

            internal MobileClip(DemoScene scene,AnimationFrames bank,SpriteLibraryData library,AnimationData clip,Func<int,Sprite> privateFrame=null) {
                this.scene=scene; this.bank=bank; this.library=library;this.clip=clip;
                this.privateFrame=privateFrame;
                drawing=privateFrame==null?SelectFrameDrawing(library,clip):null;
                fallback=FrameFallback(library,drawing??clip);
                frames=new Sprite[clip.frames.Length];
            }
            public Sprite At(int index) {
                room=scene.mobileArtRoom;lastFrame=Time.frameCount;
                if (!frames[index]) {
                    frames[index]=privateFrame!=null?privateFrame(index):scene.CreateAnimationSprite(library,clip,drawing,fallback,index,this);
                    built++;
                }
                return frames[index];
            }
            internal Sprite[] Complete() {
                room=scene.mobileArtRoom;lastFrame=Time.frameCount;
                if (built!=frames.Length) for(int i=0;i<frames.Length;i++) At(i);
                return frames;
            }
        }

        const long MobileRetainedBytes=64L*1024*1024;
        const long MobileSweepBytes=24L*1024*1024;
        readonly List<AnimationFrames> mobileCatalogFrames=new List<AnimationFrames>();
        readonly HashSet<string> mobileCatalogPages=new HashSet<string>();
        readonly List<MobileClip> mobileRoomClips=new List<MobileClip>();
        readonly HashSet<string> mobileKeptPages=new HashSet<string>();
        readonly HashSet<Texture2D> mobileRetiredTextures=new HashSet<Texture2D>();
        int mobileArtRoom;
        long mobileRetiredBytes;
        bool mobileSweepPending, mobileSweepUrgent;
        const long MobileEmergencySweepBytes = 160L * 1024 * 1024;
        readonly Dictionary<string,Sprite> mobileStagePictures=new Dictionary<string,Sprite>();
        Transform mobileStageRoot;
        int mobileStageIndex=-1;

        void BindMobileStagePictures(int index){
            if(index>=catalog.stages.Length||index>=stagesRoot.childCount)index=-1;
            // Normal native rooms hide this entire 2D layer before the frame is displayed.
            // Calibration and rooms without native geometry still use their authored flat artwork.
            if(index>=0&&!calibrationRun&&!string.IsNullOrEmpty(catalog.stages[index].nativeScene))index=-1;
            if(mobileStageRoot==stagesRoot&&mobileStageIndex==index)return;
            if(mobileStageRoot&&mobileStageIndex>=0&&mobileStageIndex<mobileStageRoot.childCount){
                var oldRoom=mobileStageRoot.GetChild(mobileStageIndex);
                foreach(string layer in new[]{"Background","Ground Foreground"}){
                    var node=oldRoom.Find(layer);
                    if(node)node.GetComponent<SpriteRenderer>().sprite=null;
                }
            }
            mobileStageIndex=index;mobileStageRoot=stagesRoot;
            var art=index<0?null:catalog.stages[index];
            string foreground=art!=null&&calibrationRun&&!string.IsNullOrEmpty(art.flatForeground)?art.flatForeground:art?.foreground;
            foreach(var path in new List<string>(mobileStagePictures.Keys)){
                if(art!=null && (art.background==path||foreground==path))continue;
                var sprite=mobileStagePictures[path];
                if(sprite){mobileRetiredBytes+=(long)sprite.texture.width*sprite.texture.height*4;Destroy(sprite);}
                mobileStagePictures.Remove(path);
            }
            if(art==null)return;
            var room=stagesRoot.GetChild(index);
            BindMobileStageLayer(room,"Background",art.background,art);
            BindMobileStageLayer(room,"Ground Foreground",foreground,art);
        }
        void BindMobileStageLayer(Transform room,string name,string resource,StageArt art){
            var node=room.Find(name);if(!node)return;
            var renderer=node.GetComponent<SpriteRenderer>();
            renderer.sprite=CachedPicture(mobileStagePictures,resource);
            FitStageSprite(renderer,art.width,art.height);
        }

        void ReleaseCatalogSprites(Sprite[] sprites) {
            foreach(var sprite in sprites) {
                if(!sprite)continue;
                spriteOffsets.Remove(sprite);nativeUiVisibleLocalRectsV28.Remove(sprite);
                if(Application.isPlaying)Destroy(sprite);else DestroyImmediate(sprite);
            }
        }
        long MobilePageBytes(string path) {
            if(!atlasTextures.TryGetValue(path,out var page)||!page)return 0;
            // Conservative upper estimate for point-filtered, unmipped RGBA art.
            return (long)page.width*page.height*(page.isReadable?8:4);
        }
        void AdvanceMobileArtRoom() {
            if(!StreamMobileArt&&mobileCatalogFrames.Count==0)return;
            mobileArtRoom++;
            KeepEntryLocomotionResident();
            TrimMobileArt(false);
        }
        void OnEnable(){if(StreamMobileArt)Application.lowMemory+=OnMobileLowMemory;}
        void OnDisable(){if(StreamMobileArt){Application.lowMemory-=OnMobileLowMemory;ResetMonsterStream();}}
        void OnMobileLowMemory(){
            heroWarmGeneration++;
            ResetMonsterStream();
            TrimMonsterPixelMemoryV53(true);
            CancelMobileRoomWarmup();
            ReleaseMobileSceneryOnPressure();
            TrimMobileArt(true);
        }

        void TrimMobileArt(bool lowMemory) {
            mobileRoomClips.Clear();mobileKeptPages.Clear();
            foreach(var bank in mobileCatalogFrames)bank.Collect(mobileRoomClips);
            mobileRoomClips.Sort((a,b)=>b.lastFrame.CompareTo(a.lastFrame));
            long retained=0;
            foreach(var clip in mobileRoomClips){
                long added=0;
                foreach(var path in clip.pages)if(!mobileKeptPages.Contains(path))added+=MobilePageBytes(path);
                bool needed=clip.room==mobileArtRoom;
                bool recent=!lowMemory&&clip.room>=mobileArtRoom-1&&retained+added<=MobileRetainedBytes;
                if(needed||recent){
                    retained+=added;mobileKeptPages.UnionWith(clip.pages);
                }else{
                    ReleaseCatalogSprites(clip.frames);clip.bank.RemoveResident(clip.clip.id);
                }
            }
            mobileRoomClips.Clear();
            mobileRetiredTextures.Clear();
            foreach(var path in mobileCatalogPages){
                if(mobileKeptPages.Contains(path))continue;
                if(atlasTextures.TryGetValue(path,out var page)&&page)mobileRetiredTextures.Add(page);
                atlasTextures.Remove(path);
            }
            mobileCatalogPages.IntersectWith(mobileKeptPages);
            if(mobileRetiredTextures.Count!=0){
                // Different catalog paths may resolve to the same Texture2D.
                // The last path owns the converted alpha pixels used by native UI.
                foreach(var page in atlasTextures.Values)mobileRetiredTextures.Remove(page);
                foreach(var page in mobileRetiredTextures){
                    mobileRetiredBytes+=(long)page.width*page.height*(page.isReadable?8:4);
                    if(nativeUiTexturePixelsV28.TryGetValue(page,out var pixels)){
                        mobileRetiredBytes+=(long)pixels.Length*4;
                        nativeUiTexturePixelsV28.Remove(page);
                    }
                }
                mobileRetiredTextures.Clear();
            }
            RequestMobileArtSweep(lowMemory);
        }
        void RequestMobileArtSweep(bool lowMemory) {
            mobileSweepUrgent |= lowMemory;
            if (!mobileSweepPending && Application.isPlaying && isActiveAndEnabled &&
                (mobileSweepUrgent || mobileRetiredBytes >= MobileSweepBytes)) StartCoroutine(SweepMobileCatalogPages());
        }

        bool MobileSweepWindow {
            get {
                return !StreamMobileArt || !adventureStarted || mobileLoadingWindowCount>0 ||
                    !string.IsNullOrEmpty(openMenu) || game==null || game.paused || game.victory ||
                    BossIntroductionActive || game.GateTransitionDelayRemaining>0;
            }
        }

        IEnumerator SweepMobileCatalogPages() {
            mobileSweepPending=true;
            try {
                do {
                    yield return null; // Let Unity finish destroying retired sprite objects.
                    while (!mobileSweepUrgent && mobileRetiredBytes<MobileEmergencySweepBytes && !MobileSweepWindow)
                        yield return null;
                    mobileSweepUrgent=false;
                    mobileRetiredBytes=0;
                    yield return Resources.UnloadUnusedAssets();
                } while (mobileSweepUrgent || mobileRetiredBytes>=MobileSweepBytes);
            } finally { mobileSweepPending=false; }
        }
    }
}
