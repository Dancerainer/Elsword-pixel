using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class RegionArtFXIndex { public RegionArtFX[] rows; }
        [Serializable] sealed class RegionArtFX {
            public string id;
            public bool projectile;
            public RegionArtLayer[] layers;
            [NonSerialized] public float duration;
            [NonSerialized] public bool explosion;
        }
        [Serializable] sealed class RegionArtLayer {
            public string texture;
            public float[] offset, size, color;
            public float at, life;
            public bool follow,continuous;
        }
        sealed class RegionalCastView {
            public RegionArtFX art;
            public Enemy owner;
            public RoomDefinition room;
            public float started;
            public float depth;
            public Vector2 origin;
            public int facing;
        }

        readonly List<SpriteRenderer> regionFxPool = new List<SpriteRenderer>();
        readonly List<RegionalCastView> regionCasts = new List<RegionalCastView>();
        readonly Dictionary<string,RegionArtFX> regionalFx = new Dictionary<string,RegionArtFX>(StringComparer.OrdinalIgnoreCase);
        int regionSpritesUsed;

        void LoadRegionalEffects(string region) {
            regionCasts.Clear();
            regionalFx.Clear();
            game.RegionalVisualCue=null;
            game.RegionSpellContactArt=null;
            if(region=="peita")return;
            var index=Resources.Load<TextAsset>("Pixel/"+region+"/combat-fx/index");
            if(!index)throw new InvalidOperationException("Missing original combat effect artwork for "+region);
            foreach(var cue in JsonUtility.FromJson<RegionArtFXIndex>(index.text).rows) {
                cue.explosion=PixelExplosionVisuals.RegionalBurst(cue.id);
                foreach(var layer in cue.layers)cue.duration=Math.Max(cue.duration,layer.at+layer.life);
                regionalFx[cue.id]=cue;
            }
            game.RegionalVisualCue=QueueRegionalCast;
            game.RegionSpellContactArt=QueueRegionalContact;
        }

        bool QueueRegionalCast(Enemy owner,string id) {
            if(!regionalFx.TryGetValue(id,out var art))return false;
            // Projectile positions come from the damage model, including delayed bone attachments.
            if(art.projectile)return true;
            regionCasts.Add(new RegionalCastView{art=art,owner=owner,room=game.CurrentRoom,started=game.elapsed,
                origin=new Vector2(owner.x,owner.y),facing=owner.facing,depth=owner.nativeDepth});
            return true;
        }

        void QueueRegionalContact(string id,float x,float y,int facing,float depth) {
            if(regionalFx.TryGetValue(id+"#hit",out var art))
                regionCasts.Add(new RegionalCastView{art=art,room=game.CurrentRoom,started=game.elapsed,
                    origin=new Vector2(x,y),facing=facing,depth=depth});
        }

        void DrawRegionSprite(Sprite sprite,Vector2 at,Vector2 size,int facing,Color color,float angle,float depth) {
            if(!sprite)return;
            if(regionSpritesUsed==regionFxPool.Count)
                regionFxPool.Add(WorldSprite("Regional monster effect",effectsRoot,null,46,Vector3.zero));
            var view=regionFxPool[regionSpritesUsed++];
            npcEffectDepths[view.transform]=depth;
            view.enabled=true;view.sprite=sprite;view.flipX=facing<0;view.color=color;
            view.transform.localPosition=at;view.transform.localRotation=Quaternion.Euler(0,0,angle);
            var bounds=sprite.bounds.size;
            view.transform.localScale=new Vector3(size.x/Mathf.Max(.01f,bounds.x),size.y/Mathf.Max(.01f,bounds.y),1);
        }

        void DrawRegionLayers(RegionArtFX art,float age,Vector2 origin,int facing,Enemy owner,float angle,float depth) {
            foreach(var l in art.layers) {
                float t=age-l.at;
                if(t<0||(!l.continuous&&t>=l.life))continue;
                Vector2 basePoint=l.follow&&owner!=null?new Vector2(owner.x,owner.y):origin;
                var position=basePoint+new Vector2(facing*l.offset[0],l.offset[1]);
                Color tint=new Color(l.color[0],l.color[1],l.color[2],l.color[3]);
                if(!l.continuous)tint.a*=Mathf.Clamp01(t*40+.24f)*Mathf.Clamp01((l.life-t)*16);
                tint.a*=Mathf.Clamp(170/Mathf.Max(1,Mathf.Max(l.size[0],l.size[1])),.18f,.92f);
                var sprite=Picture(l.texture);
                if(art.explosion)sprite=PixelExplosionVisuals.Regional(l.texture,sprite);
                DrawRegionSprite(sprite,position,new Vector2(l.size[0],l.size[1]),facing,tint,angle,
                                 l.follow && owner!=null ? owner.nativeDepth : depth);
            }
        }

        void RenderRegionalEffects() {
            int previous=regionSpritesUsed;
            regionSpritesUsed=0;
            if(game.RegionalVisualCue!=null) {
                for(int i=regionCasts.Count-1;i>=0;i--) {
                    var cast=regionCasts[i];float age=game.elapsed-cast.started;
                    if(age<0||age>=cast.art.duration||cast.room!=game.CurrentRoom) {
                        regionCasts.RemoveAt(i);continue;
                    }
                    DrawRegionLayers(cast.art,age,cast.origin,cast.facing,cast.owner,0,cast.depth);
                }
                foreach(var spell in game.RegionSpellInstances) {
                    if(!spell.launched||spell.stopped)continue;
                    var point=new Vector2(spell.x,spell.y);var d=spell.spell;
                    if(regionalFx.TryGetValue(spell.id,out var art)) {
                        var velocity=RotateSkillOffset(new Vector2(spell.vx+d.gx*spell.age,spell.vy+d.gy*spell.age),spell.angle);
                        if(d.dirSpeed!=0 && spell.age>d.dirAt)
                            velocity+=d.dirSpeed*new Vector2(Mathf.Cos(spell.heading),Mathf.Sin(spell.heading));
                        float angle=spell.angle;
                        if((Math.Abs(spell.vx)>5 || d.dirSpeed!=0) && velocity.sqrMagnitude>25)
                            angle=Mathf.Atan2(velocity.y,velocity.x)*Mathf.Rad2Deg;
                        DrawRegionLayers(art,spell.age,point,spell.facing,null,angle*spell.facing,spell.owner.nativeDepth);
                    }
                }
            }
            for(int i=regionSpritesUsed;i<previous;i++)regionFxPool[i].enabled=false;
        }
    }
}
