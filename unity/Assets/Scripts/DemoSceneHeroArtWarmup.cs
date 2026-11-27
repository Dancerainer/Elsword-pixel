using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        // PPU, source frame selection, filtering and clip retirement are owned
        // by the regular mobile catalog. Warm only the atlas pages it will use.
        sealed class HeroArtPage {
            internal string path;
            internal Library library;
            internal AnimationData clip;
            internal int frame;
            internal ResourceRequest request;
        }
        static readonly string[] heroCommandOpeners={
            "idle","run","combo-z1","combo-z2","combo-z3","combo-z4","combo-x1","dash-z1","dash-z2","dash-z3","air-x","air-z"
        };
        // Ordered by first appearance on the original timeline. Remaining
        // pages are requested only once the player actually casts this skill.
        static readonly string[] doubleLayers={
            "double-sword","double-first","double-first-red","double-first-core","double-first-trail",
            "double-flash-cross","double-impact-spark","double-spark","double-streak","double-flash",
            "double-charge-ring","double-charge-rays","double-charge-light","double-charge-final",
            "double-slash","double-second-red","double-second-core","double-flash-second",
            "double-fire-0","double-fire-1","double-fire-2","double-fire-3","double-fire-4",
            "double-fire-5","double-fire-end","double-fire-spark"
        };
        static readonly string[] sandstormLayers={"sandstorm-ring","sandstorm","sandstorm-body","sandstorm-body-second"};
        static readonly string[] flameLayers={"flame-geyser","flame-column","flame-body","flame-burst","flame-core"};
        static readonly string[] unlimitedLayers={"unlimited-blade","unlimited-second","unlimited-third","unlimited-finish","unlimited-fire"};
        static readonly string[] giganticLayers={"gigantic-cast","gigantic-cast-rise","gigantic-sword","gigantic-wave","gigantic-wave-final"};
        static readonly string[] windmillLayers={"windmill","windmill-spin"};
        int heroWarmGeneration;
        string heroWarmSkill;

        static string[] HeroSkillLayers(string skill){
            switch(skill){
                case "double-slash":return doubleLayers;
                case "sandstorm":return sandstormLayers;
                case "flame-geyser":return flameLayers;
                case "unlimited-blade":case "unlimited-blade-awakened":return unlimitedLayers;
                case "gigantic-slash":return giganticLayers;
                case "windmill":return windmillLayers;
                default:return null;
            }
        }
        void AddHeroArtPages(List<HeroArtPage> pages,HashSet<string> seen,string libraryId,string clipId,bool firstOnly){
            if(string.IsNullOrEmpty(clipId)||!libraries.TryGetValue(libraryId,out var lib)||lib.frames.open==null||
                !lib.clips.TryGetValue(clipId,out var clip)||clip.frames==null||clip.frames.Length==0)return;
            var drawing=SelectFrameDrawing(lib.data,clip);
            string fallback=FrameFallback(lib.data,drawing??clip);
            int count=firstOnly?1:clip.frames.Length;
            for(int i=0;i<count;i++){
                var frame=OutskirtsPaintExposureV40.Frame(clip,drawing,i);
                string path=string.IsNullOrEmpty(frame.texture)?fallback:BluePoruInkV45.Resource(lib.data.id,frame.texture);
                if(string.IsNullOrEmpty(path)||!seen.Add(path)||atlasTextures.ContainsKey(path))continue;
                pages.Add(new HeroArtPage{path=path,library=lib,clip=clip,frame=i});
            }
        }
        void AddSkillArtPages(List<HeroArtPage> pages,HashSet<string> seen,string skill,bool firstOnly){
            var layers=HeroSkillLayers(skill);
            if(layers==null)AddHeroArtPages(pages,seen,"nativefx-"+skill,"play",firstOnly);
            else for(int i=0;i<(firstOnly?1:layers.Length);i++)AddHeroArtPages(pages,seen,"nativefx-"+layers[i],"play",firstOnly);
            AddHeroArtPages(pages,seen,"skillfx-"+skill,"play",firstOnly);
        }
        IEnumerator WarmHeroOpeners(){
            if(!StreamMobileArt||game==null||catalog?.hero==null)yield break;
            var pages=new List<HeroArtPage>();var seen=new HashSet<string>(StringComparer.Ordinal);
            foreach(var clip in heroCommandOpeners)AddHeroArtPages(pages,seen,catalog.hero.id,clip,true);
            for(int i=0;i<GameModel.SkillSlotCount;i++){
                var skill=game.GetEquippedSkill(i);
                if(skill==null)continue;
                AddHeroArtPages(pages,seen,catalog.hero.id,skill.clip,true);
                AddSkillArtPages(pages,seen,skill.id,true);
            }
            yield return WarmHeroPages(pages,heroWarmGeneration,true);
        }
        void KeepEntryLocomotionResident(){
            if(!mobileProductEntryRunning||!libraries.TryGetValue(catalog.hero.id,out var hero))return;
            // A new room can retire an older preview's idle/run pages before
            // the first controllable frame even when the other openers fit.
            hero.frames.KeepResident("idle",mobileArtRoom);
            hero.frames.KeepResident("run",mobileArtRoom);
        }
        void WarmCurrentHeroSkill(){
            if(!StreamMobileArt)return;
            var skill=game.ActiveSkillId;
            if(skill==heroWarmSkill)return;
            heroWarmSkill=skill;
            if(string.IsNullOrEmpty(skill))return;
            var pages=new List<HeroArtPage>();var seen=new HashSet<string>(StringComparer.Ordinal);
            AddHeroArtPages(pages,seen,catalog.hero.id,game.player.clip,false);
            AddSkillArtPages(pages,seen,skill,false);
            if(pages.Count>0)StartCoroutine(WarmHeroPages(pages,heroWarmGeneration,false));
        }
        IEnumerator WarmHeroPages(List<HeroArtPage> pages,int generation,bool entering){
            var room=game.CurrentRoom;
            string skill=heroWarmSkill;
            const int parallel=4;
            for(int i=0;i<pages.Count;i+=parallel){
                if(generation!=heroWarmGeneration||(entering?mobileCancelEntry:game.CurrentRoom!=room||heroWarmSkill!=skill))yield break;
                int end=Math.Min(i+parallel,pages.Count);
                for(int j=i;j<end;j++){
                    var item=pages[j];
                    if(atlasTextures.ContainsKey(item.path))continue;
                    item.request=Resources.LoadAsync<Texture2D>(item.path);
                }
                for(int j=i;j<end;j++){
                    var item=pages[j];
                    if(item.request==null)continue;
                    if(!item.request.isDone)yield return item.request;
                    if(generation!=heroWarmGeneration||(entering?mobileCancelEntry:game.CurrentRoom!=room||heroWarmSkill!=skill))yield break;
                    if(atlasTextures.ContainsKey(item.path))continue;
                    var texture=item.request.asset as Texture2D;
                    if(!texture)continue;
                    texture.filterMode=FilterMode.Point;atlasTextures[item.path]=texture;
                    // Building this one exposure registers the page with the
                    // normal clip cache and leaves all other sprites lazy.
                    item.library.frames.At(item.clip.id,item.frame,out _);
                }
            }
        }
    }
}
