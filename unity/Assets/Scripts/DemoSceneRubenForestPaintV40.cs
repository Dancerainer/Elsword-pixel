using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        partial void TraceLegacyPaintSelectionV40(Enemy actor,SpriteRenderer carrier,string alias,float clock,int index);
        sealed class RubenForestFrameV40 {
            internal Sprite source,painted;internal string resource;internal int paintedIndex;
        }
        readonly Dictionary<Sprite,RubenForestFrameV40> rubenForestBySourceV40=new Dictionary<Sprite,RubenForestFrameV40>();
        readonly Dictionary<Sprite,RubenForestFrameV40> rubenForestByPaintV40=new Dictionary<Sprite,RubenForestFrameV40>();
        readonly List<SpriteRenderer> rubenForestVisibleV40=new List<SpriteRenderer>();
        internal static bool RubenForestKindV40(string kind){return kind=="william-cute"||kind=="william-thief"||kind=="curse-wisp"||kind=="anger-forest-knight";}
        static bool RubenForestIdentityV40(Enemy e,string room){
            if(e==null||string.IsNullOrEmpty(room))return false;
            switch(e.kind){
                case "william-cute":return e.sourceNPC=="NUI_PPORU_WILLIAM_CUTE"&&room.StartsWith("ruins-",StringComparison.Ordinal);
                case "william-thief":return e.sourceNPC=="NUI_PPORU_WILLIAM_CUTE_THIEF"&&room.StartsWith("ruins-",StringComparison.Ordinal);
                case "curse-wisp":return e.sourceNPC=="NUI_CURSE_WISP"&&room.StartsWith("forest-",StringComparison.Ordinal);
                case "anger-forest-knight":return e.sourceNPC=="NUI_ANGER_FOREST_KNIGHT"&&room.StartsWith("forest-",StringComparison.Ordinal);
                default:return false;
            }
        }
        internal static string RubenForestPaintKeyV40(string kind,string alias){
            switch(alias){
                case "source-idle":return "idle";case "source-walk":return "walk";case "source-hurt":return "hurt";
                case "source-down":return "down";case "source-get-up":return "get-up";case "source-death":return "death";
                case "source-attack-ready":case "source-attack-fire":return "attack";
                case "source-attack-a-ready":return "attack-a-ready";case "source-attack-a":return "attack-a";
                case "source-air-up":return "launch";
                case "source-air-fall":return kind.StartsWith("william-",StringComparison.Ordinal)?"launch":"fall";
                default:return null;
            }
        }
        struct SpellPoseV40 {
            internal readonly float at;internal readonly string action;internal readonly int index;
            internal SpellPoseV40(float at,string action,int index){this.at=at;this.action=action;this.index=index;}
        }
        // These are source-motion fractions and indices in existing V07/V13
        // exposure arrays. The masked thief actually rolls during MagicAttackA
        // while the pale unmasked William lunges and comes upright.
        static readonly SpellPoseV40[] cuteReadyV40={
            new SpellPoseV40(0,"idle",0),new SpellPoseV40(.13f,"attack",0),new SpellPoseV40(.32f,"attack",8),
            new SpellPoseV40(.44f,"attack",15),new SpellPoseV40(.65f,"attack",24),new SpellPoseV40(.79f,"roar",10),new SpellPoseV40(.90f,"roar",40)};
        static readonly SpellPoseV40[] cuteFireV40={
            new SpellPoseV40(0,"roar",40),new SpellPoseV40(.10f,"jump",8),new SpellPoseV40(.25f,"attack",15),
            new SpellPoseV40(.43f,"attack",24),new SpellPoseV40(.60f,"attack",27),new SpellPoseV40(.77f,"attack",29),new SpellPoseV40(.91f,"idle",0)};
        static readonly SpellPoseV40[] thiefReadyV40={
            new SpellPoseV40(0,"idle",0),new SpellPoseV40(.12f,"roar",10),new SpellPoseV40(.26f,"roar",40),
            new SpellPoseV40(.42f,"launch",0),new SpellPoseV40(.60f,"roar",10),new SpellPoseV40(.78f,"jump",8),new SpellPoseV40(.93f,"attack",8)};
        static readonly SpellPoseV40[] thiefFireV40={
            new SpellPoseV40(0,"jump",8),new SpellPoseV40(.13f,"attack",24),new SpellPoseV40(.24f,"launch",10),
            new SpellPoseV40(.37f,"launch",29),new SpellPoseV40(.57f,"get-up",8),new SpellPoseV40(.74f,"get-up",22),new SpellPoseV40(.89f,"idle",0)};
        internal static bool RubenSpellPoseV40(string kind,string alias,AnimationData selected,int sourceIndex,out string key,out int index){
            key=null;index=0;bool ready=alias=="source-attack-ready";
            if(!ready&&alias!="source-attack-fire")return false;
            bool cute=kind=="william-cute";if(!cute&&kind!="william-thief")return false;
            var poses=cute?(ready?cuteReadyV40:cuteFireV40):(ready?thiefReadyV40:thiefFireV40);
            float time=sourceIndex/(selected.fps*selected.duration);
            int at=0;while(at+1<poses.Length&&poses[at+1].at<=time)at++;
            key=poses[at].action;index=poses[at].index;return true;
        }
        internal static int RubenForestPaintIndexV40(string kind,string alias,AnimationData selected,AnimationData drawn,int sourceIndex){
            int first=0,last=drawn.frames.Length-1;
            if(kind=="william-cute"||kind=="william-thief"){
                if(alias=="source-air-up"||alias=="source-air-fall"){
                    // Split at the second of the three painted airborne poses.
                    // Use the existing exposure lengths, not invented clocks.
                    var f=drawn.frames[0];int second=1;
                    while(second<drawn.frames.Length&&drawn.frames[second].x==f.x&&drawn.frames[second].y==f.y)second++;
                    if(second<drawn.frames.Length){if(alias=="source-air-up")last=second;else first=second;}
                }
            }
            if(first==0&&last==drawn.frames.Length-1)return OutskirtsPaintExposureV40.Map(selected,drawn,sourceIndex);
            double phase=Math.Min(1,(double)sourceIndex/selected.fps/selected.duration);
            return Math.Min(last,first+(int)Math.Floor(phase*(last-first+1)+.00001));
        }
        void RestoreRubenForestPaintV40(){
            foreach(var carrier in rubenForestVisibleV40)if(carrier&&carrier.sprite&&rubenForestByPaintV40.TryGetValue(carrier.sprite,out var view))carrier.sprite=view.source;
            rubenForestVisibleV40.Clear();
        }
        void DisposeRubenForestPaintV40(){
            RestoreRubenForestPaintV40();
            foreach(var f in rubenForestBySourceV40.Values){
                if(f.painted){spriteOffsets.Remove(f.painted);nativeUiVisibleLocalRectsV28.Remove(f.painted);DestroyNativeWorldObject(f.painted);}
            }
            rubenForestBySourceV40.Clear();rubenForestByPaintV40.Clear();
        }
        Sprite OriginalRubenForestSpriteV40(Sprite actual){
            return actual&&rubenForestByPaintV40.TryGetValue(actual,out var f)?f.source:actual;
        }
        RubenForestFrameV40 BuildRubenForestFrameV40(Library lib,Sprite source,string alias,int index,string key){
            var selected=lib.clips[alias];string spellKey;int spellIndex;
            bool spell=RubenSpellPoseV40(lib.data.id,alias,selected,index,out spellKey,out spellIndex);
            if(spell)key=spellKey;
            if(!lib.clips.TryGetValue(key,out var drawn)||!lib.frames.TryGetValue(key,out var pictures)||pictures.Length!=drawn.frames.Length)
                throw new InvalidOperationException("Missing approved painted clip "+lib.data.id+"/"+key);
            int paintIndex=spell?spellIndex:RubenForestPaintIndexV40(lib.data.id,alias,selected,drawn,index);
            var art=pictures[paintIndex];var rect=art.rect;var pivot=new Vector2(art.pivot.x/rect.width,art.pivot.y/rect.height);
            var proxy=Sprite.Create(art.texture,rect,pivot,art.pixelsPerUnit,0,SpriteMeshType.FullRect);
            proxy.name=source.name;
            if(nativeUiVisibleLocalRectsV28.TryGetValue(art,out var visible))nativeUiVisibleLocalRectsV28[proxy]=visible;
            if(spriteOffsets.TryGetValue(art,out var offset))spriteOffsets[proxy]=offset;
            var frame=drawn.frames[paintIndex];
            string resource=SourceMotionInkV45.ResourceFor(lib.data.id,key,paintIndex)??
                (!string.IsNullOrEmpty(frame.texture)?frame.texture:!string.IsNullOrEmpty(drawn.texture)?drawn.texture:lib.data.texture);
            var result=new RubenForestFrameV40{source=source,painted=proxy,resource=resource,paintedIndex=paintIndex};
            rubenForestBySourceV40.Add(source,result);rubenForestByPaintV40.Add(proxy,result);return result;
        }
        void ApplyRubenForestPaintV40(){
            if(game==null||game.CurrentRoom==null)return;
            for(int i=0;i<game.enemies.Count&&i<enemyViews.Count;i++){
                var actor=game.enemies[i];var view=enemyViews[i];
                if(MonsterPixelOwnsActorV53(actor))continue;
                if(!RubenForestIdentityV40(actor,game.CurrentRoom.id)||actor.visualScale!=1||!view||!view.enabled||view.forceRenderingOff||!view.sprite)continue;
                if(!libraries.TryGetValue(actor.kind,out var lib)||LegacyBodyRegistrationV39.Find(actor,Dungeon.id,lib.data.artVersion)==null)continue;
                var source=view.sprite;
                if(!rubenForestBySourceV40.TryGetValue(source,out var frame)){
                    var name=source.name;int slash=name.LastIndexOf('/'),prefix=actor.kind.Length+1,index;
                    if(slash<=prefix||!name.StartsWith(actor.kind+"/source-",StringComparison.Ordinal)||!int.TryParse(name.Substring(slash+1),out index))continue;
                    string alias=name.Substring(prefix,slash-prefix),key=RubenForestPaintKeyV40(actor.kind,alias);
                    if(key==null||!lib.frames.TryGetValue(alias,out var actual)||index<0||index>=actual.Length||actual[index]!=source)continue;
                    frame=BuildRubenForestFrameV40(lib,source,alias,index,key);
                }
                // Selection and its original texture/anchor validation finish
                // first. Only the visible picture is exchanged; the same live
                // carrier keeps all hit-flash, sorting and visibility properties.
                view.sprite=frame.painted;view.flipX=actor.facing<0;rubenForestVisibleV40.Add(view);
                float scale=lib.data.logicalScale*actor.visualScale;float baseline=RegisterLegacyBodyV39(actor,ref scale);
                view.transform.localScale=Vector3.one*scale;
                var place=RenderEnemyPosition(actor)+SpriteOffset(view)*scale+Vector2.up*baseline;
                view.transform.localPosition=new Vector3(place.x,place.y,0);
                if(actor.hp>0&&enemyBars[i]&&enemyBars[i].enabled){
                    float top=NativeVisibleTopYV28(view),x=view.bounds.center.x;var pos=enemyBars[i].transform.position;
                    pos.x=x-(actor.maxHp>400?64:30)*(1-actor.hp/actor.maxHp)*.5f;pos.y=top+8;enemyBars[i].transform.position=pos;
                    pos=enemyDebuffViews[i].transform.position;pos.x=x;pos.y=top+20;enemyDebuffViews[i].transform.position=pos;
                }
            }
        }
    }
}
