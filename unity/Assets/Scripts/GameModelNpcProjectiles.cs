using System;

namespace ElswordPixel {
    public sealed partial class GameModel {
        static void ValidateNpcProjectile(NpcEffectDefinition d){
            if(d.projectileVelocities==null)d.projectileVelocities=new NpcProjectileVelocitySample[0];
            if(!Finite(d.damageUntil)||d.damageUntil<0||d.damageUntil>d.lifetime)throw new ArgumentException("Invalid NPC projectile damage lifetime");
            if(!Finite(d.damageFrom)||d.damageFrom<0||d.damageFrom>d.lifetime||d.damageUntil>0&&d.damageFrom>d.damageUntil)throw new ArgumentException("Invalid NPC effect damage start");
            if(!d.projectilePhysics){
                if(d.projectileVelocities.Length!=0||d.projectileGravityY!=0||d.projectileBounceX!=0||d.projectileBounceY!=0)throw new ArgumentException("Projectile payload has no enabled consumer");
                return;
            }
            if(d.projectileVelocities.Length<2||!Finite(d.projectileGravityY)||d.projectileGravityY>=0||!Finite(d.projectileBounceX)||d.projectileBounceX<0||d.projectileBounceX>1||!Finite(d.projectileBounceY)||d.projectileBounceY<0||d.projectileBounceY>1)throw new ArgumentException("Invalid original projectile physics");
            float previous=-1;
            foreach(var row in d.projectileVelocities){
                if(row==null||!Finite(row.time)||row.time<=previous||!Finite(row.x)||!Finite(row.y))throw new ArgumentException("Invalid original projectile velocity curve");
                previous=row.time;
            }
            if(Math.Abs(d.projectileVelocities[0].time)>.00001f||Math.Abs(previous-d.lifetime)>.00001f)throw new ArgumentException("Projectile velocity must span its lifetime");
        }
        static void ProjectileVelocityAt(NpcEffectDefinition d,float time,out float x,out float y){
            var rows=d.projectileVelocities;int lo=0,hi=rows.Length;
            while(lo<hi){int mid=(lo+hi)/2;if(rows[mid].time<=time)lo=mid+1;else hi=mid;}
            var a=rows[Math.Max(0,lo-1)];var b=rows[Math.Min(lo,rows.Length-1)];
            float span=b.time-a.time,u=span<=0||b.reset?0:Clamp((time-a.time)/span,0,1);
            x=a.x+(b.x-a.x)*u;y=a.y+(b.y-a.y)*u;
        }
        void InitializeNpcProjectile(NpcEffectInstance e){
            if(TryInitializeAlteraCannonBigBallisticV18(e))return;
            if(TryInitializePlainDrillerBallisticR1315(e))return;
            if(!e.definition.projectilePhysics)return;
            ProjectileVelocityAt(e.definition,0,out e.projectileVx,out e.projectileVy);e.projectileVx*=e.facing;
        }
        void MoveNpcProjectile(NpcEffectInstance e,float from,float to){
            if(TryMoveAlteraCannonBigBallisticV18(e,from,to))return;
            if(TryMovePlainDrillerBallisticR1315(e,from,to))return;
            float cursor=from;
            foreach(var key in e.definition.projectileVelocities){
                if(!key.reset||key.time<=from+1e-7f||key.time>to+1e-7f)continue;
                IntegrateNpcProjectile(e,cursor,key.time,true);
                e.projectileVx=e.facing*key.x;e.projectileVy=key.y;e.projectileHorizontalRetention=1;e.plainWorldGravityVelocityXR1304=0;
                TraceNpcEffect("projectile-velocity-reset",e,0,e.bornAt+key.time);cursor=key.time;
            }
            if(to>cursor+1e-7f)IntegrateNpcProjectile(e,cursor,to,false);
        }
        void IntegrateNpcProjectile(NpcEffectInstance e,float from,float to,bool beforeReset){
            float dt=to-from;if(dt<=0)return;float ax,ay,bx,by;
            ProjectileVelocityAt(e.definition,from,out ax,out ay);
            ProjectileVelocityAt(e.definition,beforeReset?Math.Max(from,to-.00001f):to,out bx,out by);
            float oldY=e.y,vx=e.projectileVx,vy=e.projectileVy;
            // Explicit reconstruction adapter: horizontal source velocity is
            // reduced by accumulated ground restitution, while vertical fades
            // add their delta to gravity/bounce velocity. An "at" key resets
            // both. Damping cannot turn a positive throw into backwards travel.
            float plainGravityX=PlainOriginalFxGravityWorldXR1304(e);
            if(plainGravityX==0){e.projectileVx=e.facing*bx*e.projectileHorizontalRetention;e.projectileVy+=by-ay+e.definition.projectileGravityY*dt;}
            else {e.plainWorldGravityVelocityXR1304+=plainGravityX*dt;e.projectileVx=e.facing*bx*e.projectileHorizontalRetention+e.plainWorldGravityVelocityXR1304;e.projectileVy+=by-ay+e.definition.projectileGravityY*dt;}
            e.x+=(vx+e.projectileVx)*.5f*dt;e.y+=(vy+e.projectileVy)*.5f*dt;
            var surface=NativeTerrain?SurfaceBelow(e.x,oldY+.05f):null;
            float floor=NativeTerrain?(surface==null?float.NegativeInfinity:surface.Y(e.x)):GroundAt(CurrentRoom,e.x);
            // UseLand is projected at the original effect origin, consistent
            // with the scene's source line system. We do not raise a global
            // floor, clamp to the player's elevation, or invent side walls.
            if(e.projectileVy<=0&&oldY>=floor-.05f&&e.y<=floor){
                e.y=floor;e.projectileVy=-e.projectileVy*e.definition.projectileBounceY;e.projectileHorizontalRetention*=e.definition.projectileBounceX;e.projectileVx*=e.definition.projectileBounceX;e.plainWorldGravityVelocityXR1304*=e.definition.projectileBounceX;
                if(e.projectileVy<.5f)e.projectileVy=0;
                TraceNpcEffect("projectile-bounce",e,0,e.bornAt+to);
            }
        }
        void EmitNpcHitChild(NpcEffectInstance parent,float at,float x,float y,float z){
            RecordNpcPresentationHit(parent,at,x,y,z);
            string child=parent.definition.hitChild;if(string.IsNullOrEmpty(child))return;
            var effect=SpawnNpcEffect(parent.owner,npcEffectDefinitions[child],null,at,parent);
            // HIT_DAMAGE_EFFECT/DEPT_IMPACT_POINT is separate from the source
            // DIE_DAMAGE_EFFECT/DEPT_POS. A successful hit does not implicitly
            // kill the parent or emit its expiry child a second time.
            effect.x=x;effect.y=y;effect.z=z;TraceNpcEffect("hit-child",effect,0,at);
        }
    }
}
