using System;
using System.Collections.Generic;

namespace ElswordPixel {
    [Serializable] public sealed class NpcEffectSample {public float time,scaleX=1,scaleY=1,scaleZ=1,alpha=1,moveX;}
    [Serializable] public sealed class NpcEffectGeometrySample {public float time;public float[] spheres=new float[0],attackLines=new float[0];}
    [Serializable] public sealed class NpcProjectileVelocitySample {public float time,x,y;public bool reset;}
    [Serializable] public sealed class NpcEffectChildDefinition {public string effect;public float offsetX,offsetY,offsetZ,rotationZ;public bool applyParentRotation,rotateOffset;}
    [Serializable] public sealed class NpcEffectDefinition {
        public AlteraCannonBigBallisticRuleV18 alteraCannonBigBallistic;public AlteraRavenMaximumEffectCombatV18 alteraRavenMaximumCombat;public AlteraRavenMissileRainEffectV18 alteraRavenMissileRain;public AlteraRavenCannonBladeRuleV18 alteraRavenCannonBlade;
        public string id,visualId,contactChild,deathChild,hitChild;
        internal NpcEffectDefinition CopyDragonEffect(){return (NpcEffectDefinition)MemberwiseClone();}
        public PlainOriginalFxSourceR1304 plainOriginalSourceR1304;
        public bool runtimeAdmitted,sourceMechanismsComplete,reflectable,repeat,randomYaw,visualFromMesh;
        public float lifetime,physical,magic,launch,push,stopAttacker,stopDefender,repeatGap=.1f,yawMin,yawMax,lockOnUntil,lockCurveSpeed;
        public HitReaction reaction=HitReaction.Hurt;public float[] baseSpheres=new float[0];public NpcEffectSample[] samples=new NpcEffectSample[0];
        public NpcEffectGeometrySample[] geometrySamples=new NpcEffectGeometrySample[0];
        // Optional source projectile branch. Empty fields preserve every older
        // document, including Unity's null/empty-array deserialization.
        public bool projectilePhysics;
        public float projectileGravityY,projectileBounceX,projectileBounceY,damageFrom,damageUntil;
        public float onHitBurnDuration,onHitBurnPerSecond;
        public float onHitManaDrainDuration,onHitManaDrainPerSecond;
        public NpcProjectileVelocitySample[] projectileVelocities=new NpcProjectileVelocitySample[0];
        public NpcEffectChildDefinition[] deathChildren=new NpcEffectChildDefinition[0];
    }
    [Serializable] public sealed class NpcAttachmentPoint {public float x,y,z;}
    [Serializable] public sealed class NpcAttachmentTrack {public string bone;public NpcAttachmentPoint[] samples=new NpcAttachmentPoint[0];}
    [Serializable] public sealed class NpcEffectEmission {public string effect,bone,ownerState;public string sourcePlainCallbackStateR1315;public float start,offsetX,offsetY,offsetZ,rotationZ,scaleX=1,scaleY=1,scaleZ=1;public bool follow,sourceLandPositionYArgument;public int facingMultiplier=1,lockOnMode;}
    [Serializable] public sealed class NpcEffectSetDefinition {public string id;public float attachAt;public NpcEffectEmission[] emissions=new NpcEffectEmission[0];}
    [Serializable] public sealed class NpcEffectBinding {public string npc,state;public float unitScale=1;public NpcAttachmentTrack[] tracks=new NpcAttachmentTrack[0];public NpcEffectSetDefinition[] sets=new NpcEffectSetDefinition[0];}
    [Serializable] public sealed class NpcReactionEffectBinding {
        public string npc,sourceState,sourceAnimation;public CombatState combatState;
        public float duration,sourceAnimSpeed=1;public NpcAttachmentTrack[] tracks=new NpcAttachmentTrack[0];
    }
    [Serializable] public sealed class NpcEffectDocument {public bool runtimeAdmitted,sourceMechanismsComplete;public NpcEffectDefinition[] definitions=new NpcEffectDefinition[0];public NpcEffectBinding[] bindings=new NpcEffectBinding[0];public NpcReactionEffectBinding[] reactionBindings=new NpcReactionEffectBinding[0];}
    public sealed class NpcEffectInstance {
        public int serial,facing=1;public string id;public float x,y,z,age,lifetime,yaw,dirX=1,dirY,alpha=1;
        public bool reflected;public Enemy owner;
        public bool hasSourceLandY;public float sourceLandY;
        internal NpcEffectDefinition definition;internal NpcEffectEmission emission;
        internal float scaleX=1,scaleY=1,scaleZ=1,ownerScale=1,physicalAttack,magicAttack,bornAt,updatedAt;
        internal float nextPlayerHit=float.NegativeInfinity;internal bool ended;
        internal bool belowCannonPortGuidanceR863;internal float belowCannonPortDirectionZR863;
        internal float projectileVx,projectileVy,projectileHorizontalRetention=1,plainWorldGravityVelocityXR1304;
        internal readonly Dictionary<Enemy,float> nextEnemyHit=new Dictionary<Enemy,float>();
    }
    [Serializable] public sealed class NpcEffectEvent {public string type,effect,npc;public int serial,damage;public float time,x,y,z,age;public bool reflected;
        // damage stays legacy-int; these explicitly carry real fractional HP loss.
        public bool fractionalDamageAvailableR901;public string fractionalOutcomeR901;
        public float rawDamageFractionalR901,damageAfterDefenseFractionalR901,hpBeforeFractionalR901,hpAfterFractionalR901,appliedHpLossFractionalR901;
    }

    public sealed partial class GameModel {
#if UNITY_EDITOR
        // R786 sidecar observations only: first two real missiles in this model.
        // No source UID allocation, callback, collision replay or consumed damage.
        [Serializable] sealed class BelowCannonObservationR786 {
            public string reason,npc;public int generation,serial,room,sphereOffset=-1,roundedDamageDiagnostic,actualDealt;
            public float at,bornAt,age,x,y,z,ownerScale,scaleX,scaleY,scaleZ,ownerDepth,ownerVisualScale,physicalAttack,magicAttack,physicalMultiplier,magicMultiplier,rawDamageDiagnostic;
            public float sphereX,sphereY,sphereZ,radius,playerX,playerY,clampedPointX,clampedPointY,clampedPointZ,playerDepthMin=-9.8f,playerDepthMax=9.8f;
            public string fractionalObservationSchemaR901,fractionalOutcomeR901;
            public bool fractionalResultAvailableR901,fractionalCounteredR901;
            public float rawDamageFractionalR901,damageAfterDefenseFractionalR901,hpBeforeFractionalR901,hpAfterFractionalR901,appliedHpLossFractionalR901;
            public bool ended,reflected,hasSphere,hasComputedPlayerClamp,actualDealtAvailable,finite,playerNativeZAvailable=false,diagnosticDamageNotConsumed=true;
        }
        [Serializable] sealed class BelowCannonGeometrySampleR989 {
            public bool available,actualDistanceAvailable,inDamageWindow,intersectionPassed;
            public int sphereOffset=-1;public float at,age,x,y,z,playerX,playerY,sphereX,sphereY,sphereZ,radius,clampX,clampY,clampZ,distanceSquared,radiusSquared;
        }
        [Serializable] sealed class BelowCannonWindowR989 {
            public string schema="below-cannon-actual-window-readonly-r989",failure="";
            // Fixed source order: no-definition,before-window,after-window,repeat-wait,radius,sphere-reject,contact-child,int-result,fractional-result.
            public int[] reasonCounts=new int[9],inWindowReasonCounts=new int[9];
            public bool failed,countersSaturated;public int actualGeometrySamples;
            public BelowCannonGeometrySampleR989 first,last,minimum;
        }
        sealed class BelowCannonSamplesR786 {
            internal int generation,samples,applyCalls,positiveDealt;internal bool retired;
            internal BelowCannonObservationR786 spawn,first,last;internal BelowCannonWindowR989 window=new BelowCannonWindowR989();
        }
        [Serializable] sealed class BelowCannonLogR786 {
            public string schema="below-cannon-existing-path-readonly-r786",phase;public int contactSamples,actualApplyCalls,positiveDealt;
            public BelowCannonObservationR786 spawn,firstContact,lastContact,retire;public BelowCannonWindowR989 window;
        }
        readonly Dictionary<int,BelowCannonSamplesR786> belowCannonSamplesR786=new Dictionary<int,BelowCannonSamplesR786>();
        static readonly bool BelowCannonDiagnosticArmedR786=Array.IndexOf(Environment.GetCommandLineArgs(),"--below-ordinary-input-pilot")>=0;
        int belowCannonRecordsR786,belowCannonFailuresR786;long belowCannonLogBytesR786;
        bool BelowCannonScopeR786(NpcEffectInstance e){return BelowCannonDiagnosticArmedR786&&e!=null&&e.id=="CANNONFORT_PARASITE_CANNON_MISSILE"&&NativeTerrain&&room==2&&CurrentRoom.id=="Dungeon_Altera_Below_Tunnel_Normal-2";}
        static int BelowCannonIncrementR786(int n){return n<1000000?n+1:n;}
        BelowCannonObservationR786 BelowCannonCopyR786(NpcEffectInstance e,string reason,float at,float physical,float magic){
            float raw=e.physicalAttack*physical+e.magicAttack*magic;
            return new BelowCannonObservationR786{reason=reason,npc=e.owner==null?"":e.owner.sourceNPC,generation=NpcPresentationGeneration,serial=e.serial,room=room,at=at,bornAt=e.bornAt,age=e.age,x=e.x,y=e.y,z=e.z,ownerScale=e.ownerScale,scaleX=e.scaleX,scaleY=e.scaleY,scaleZ=e.scaleZ,ownerDepth=e.owner==null?0:e.owner.nativeDepth,ownerVisualScale=e.owner==null?0:e.owner.visualScale,physicalAttack=e.physicalAttack,magicAttack=e.magicAttack,physicalMultiplier=physical,magicMultiplier=magic,rawDamageDiagnostic=raw,roundedDamageDiagnostic=(int)Math.Round(raw),playerX=player.x,playerY=player.y,ended=e.ended,reflected=e.reflected,finite=Finite(at)&&Finite(e.bornAt)&&Finite(e.age)&&Finite(e.x)&&Finite(e.y)&&Finite(e.z)&&Finite(e.ownerScale)&&Finite(e.scaleX)&&Finite(e.scaleY)&&Finite(e.scaleZ)&&Finite(e.physicalAttack)&&Finite(e.magicAttack)&&Finite(physical)&&Finite(magic)&&Finite(raw)&&Finite(player.x)&&Finite(player.y)&&(e.owner==null||Finite(e.owner.nativeDepth)&&Finite(e.owner.visualScale))};
        }
        void BelowCannonDiagnosticFailureR786(){
            if(belowCannonFailuresR786>=3)return;belowCannonFailuresR786++;
            const string line="BELOW_CANNON_READONLY_R786_DIAGNOSTIC_FAILURE";
            int bytes=System.Text.Encoding.UTF8.GetByteCount(line)+1;if(belowCannonLogBytesR786+bytes>65536)return;belowCannonLogBytesR786+=bytes;UnityEngine.Debug.Log(line);
        }
        void BelowCannonWriteR786(BelowCannonLogR786 row){
            if(belowCannonRecordsR786>=4){BelowCannonDiagnosticFailureR786();return;}
            foreach(var snapshot in new[]{row.spawn,row.firstContact,row.lastContact,row.retire})if(snapshot!=null&&!snapshot.finite)throw new InvalidOperationException("R786 nonfinite diagnostic copy");
            string line="BELOW_CANNON_READONLY_R786 "+UnityEngine.JsonUtility.ToJson(row);int bytes=System.Text.Encoding.UTF8.GetByteCount(line)+1;
            if(bytes>8192||belowCannonLogBytesR786+bytes>65536){BelowCannonDiagnosticFailureR786();return;}belowCannonRecordsR786++;belowCannonLogBytesR786+=bytes;UnityEngine.Debug.Log(line);
        }
        void BelowCannonSpawnDiagnosticR786(NpcEffectInstance e,float at){try{
            if(!BelowCannonScopeR786(e)||belowCannonSamplesR786.Count>=2||belowCannonFailuresR786>=3||belowCannonSamplesR786.ContainsKey(e.serial))return;
            var sample=new BelowCannonSamplesR786{generation=NpcPresentationGeneration,spawn=BelowCannonCopyR786(e,"real-spawn-committed",at,e.definition.physical,e.definition.magic)};belowCannonSamplesR786.Add(e.serial,sample);
            BelowCannonWriteR786(new BelowCannonLogR786{phase="spawn-head",spawn=sample.spawn});
        }catch{BelowCannonDiagnosticFailureR786();}}
        void BelowCannonContactDiagnosticR786(NpcEffectInstance e,string reason,float at,float physical,float magic,int sphereOffset=-1,float x=0,float y=0,float z=0,float radius=0,bool clamp=false,float px=0,float py=0,float pz=0,bool dealtAvailable=false,int dealt=0){try{
            BelowCannonSamplesR786 samples;if(!BelowCannonScopeR786(e)||belowCannonFailuresR786>=3||!belowCannonSamplesR786.TryGetValue(e.serial,out samples)||samples.generation!=NpcPresentationGeneration||samples.retired)return;
            BelowCannonWindowReasonR989(samples,reason);
            var row=BelowCannonCopyR786(e,reason,at,physical,magic);row.sphereOffset=sphereOffset;row.hasSphere=sphereOffset>=0;row.sphereX=x;row.sphereY=y;row.sphereZ=z;row.radius=radius;row.hasComputedPlayerClamp=clamp;row.clampedPointX=px;row.clampedPointY=py;row.clampedPointZ=pz;row.actualDealtAvailable=dealtAvailable;row.actualDealt=dealt;
            // px/py are existing closest-point results, not a second hit test.
            row.finite&=Finite(x)&&Finite(y)&&Finite(z)&&Finite(radius)&&Finite(px)&&Finite(py)&&Finite(pz);
            samples.samples=BelowCannonIncrementR786(samples.samples);if(dealtAvailable){samples.applyCalls=BelowCannonIncrementR786(samples.applyCalls);if(dealt>0)samples.positiveDealt=BelowCannonIncrementR786(samples.positiveDealt);}if(samples.first==null)samples.first=row;samples.last=row;
        }catch{BelowCannonDiagnosticFailureR786();}}
        void BelowCannonFractionalContactDiagnosticR901(NpcEffectInstance e,float at,float physical,float magic,int sphereOffset,float x,float y,float z,float radius,float px,float py,float pz,BelowCannonFractionalHitResultR901 result){try{
            BelowCannonSamplesR786 samples;if(!BelowCannonScopeR786(e)||belowCannonFailuresR786>=3||!belowCannonSamplesR786.TryGetValue(e.serial,out samples)||samples.generation!=NpcPresentationGeneration||samples.retired)return;
            BelowCannonWindowReasonR989(samples,"actual-fractional-Apply-result-r901");
            var row=BelowCannonCopyR786(e,"actual-fractional-Apply-result-r901",at,physical,magic);row.sphereOffset=sphereOffset;row.hasSphere=sphereOffset>=0;row.sphereX=x;row.sphereY=y;row.sphereZ=z;row.radius=radius;row.hasComputedPlayerClamp=true;row.clampedPointX=px;row.clampedPointY=py;row.clampedPointZ=pz;
            row.actualDealtAvailable=false; // no integer return was obtained on this path
            row.fractionalObservationSchemaR901="below-cannon-real-fractional-hp-r901";row.fractionalResultAvailableR901=true;row.fractionalCounteredR901=result.countered;row.fractionalOutcomeR901=result.outcome;
            row.rawDamageFractionalR901=result.rawDamage;row.damageAfterDefenseFractionalR901=result.afterDefense;row.hpBeforeFractionalR901=result.hpBefore;row.hpAfterFractionalR901=result.hpAfter;row.appliedHpLossFractionalR901=result.appliedHpLoss;
            row.finite&=Finite(x)&&Finite(y)&&Finite(z)&&Finite(radius)&&Finite(px)&&Finite(py)&&Finite(pz)&&Finite(result.rawDamage)&&Finite(result.afterDefense)&&Finite(result.hpBefore)&&Finite(result.hpAfter)&&Finite(result.appliedHpLoss);
            samples.samples=BelowCannonIncrementR786(samples.samples);samples.applyCalls=BelowCannonIncrementR786(samples.applyCalls);if(result.appliedHpLoss>0)samples.positiveDealt=BelowCannonIncrementR786(samples.positiveDealt);if(samples.first==null)samples.first=row;samples.last=row;
        }catch{BelowCannonDiagnosticFailureR786();}}
        void BelowCannonWindowFailureR989(BelowCannonSamplesR786 samples,string reason){
            if(samples!=null){samples.window.failed=true;if(samples.window.failure.Length==0)samples.window.failure=reason;}BelowCannonDiagnosticFailureR786();
        }
        void BelowCannonWindowReasonR989(BelowCannonSamplesR786 samples,string reason){try{
            int index;switch(reason){case "no-damage-definition":index=0;break;case "before-damage-window":index=1;break;case "after-damage-window":index=2;break;case "existing-repeat-hit-wait":index=3;break;case "existing-sphere-radius-nonpositive":index=4;break;case "existing-sphere-distance-rejected":index=5;break;case "existing-contact-child-branch":index=6;break;case "actual-ApplyHitToPlayer-return":index=7;break;case "actual-fractional-Apply-result-r901":index=8;break;default:BelowCannonWindowFailureR989(samples,"unknown-reason");return;}
            var w=samples.window;if(samples.samples>=1000000){w.countersSaturated=true;BelowCannonWindowFailureR989(samples,"total-contact-capacity");return;}if(w.reasonCounts[index]>=1000000||(index>=3&&w.inWindowReasonCounts[index]>=1000000)){w.countersSaturated=true;BelowCannonWindowFailureR989(samples,"reason-count-capacity");return;}
            w.reasonCounts[index]++;if(index>=3)w.inWindowReasonCounts[index]++;
        }catch{BelowCannonWindowFailureR989(samples,"reason-copy-exception");}}
        void BelowCannonWindowGeometryR989(NpcEffectInstance e,float at,int offset,float x,float y,float z,float radius,float px,float py,float pz,float distanceSquared,float radiusSquared,bool rejected){
            BelowCannonSamplesR786 samples=null;try{
                if(!BelowCannonScopeR786(e)||belowCannonFailuresR786>=3||!belowCannonSamplesR786.TryGetValue(e.serial,out samples)||samples.generation!=NpcPresentationGeneration||samples.retired)return;
                var w=samples.window;if(w.actualGeometrySamples>=1000000){w.countersSaturated=true;BelowCannonWindowFailureR989(samples,"geometry-count-capacity");return;}
                if(!Finite(at)||!Finite(e.age)||!Finite(e.x)||!Finite(e.y)||!Finite(e.z)||!Finite(player.x)||!Finite(player.y)||!Finite(x)||!Finite(y)||!Finite(z)||!Finite(radius)||!Finite(px)||!Finite(py)||!Finite(pz)||!Finite(distanceSquared)||!Finite(radiusSquared)){BelowCannonWindowFailureR989(samples,"nonfinite-actual-geometry");return;}
                // Called only after the original damage-window gates and the original one-shot distance/radius expressions and comparison.
                var row=new BelowCannonGeometrySampleR989{available=true,actualDistanceAvailable=true,inDamageWindow=true,intersectionPassed=!rejected,sphereOffset=offset,at=at,age=e.age,x=e.x,y=e.y,z=e.z,playerX=player.x,playerY=player.y,sphereX=x,sphereY=y,sphereZ=z,radius=radius,clampX=px,clampY=py,clampZ=pz,distanceSquared=distanceSquared,radiusSquared=radiusSquared};
                w.actualGeometrySamples++;if(w.first==null)w.first=row;w.last=row;if(w.minimum==null||distanceSquared<w.minimum.distanceSquared)w.minimum=row;
            }catch{BelowCannonWindowFailureR989(samples,"geometry-copy-exception");}
        }
        void BelowCannonRetireDiagnosticR786(NpcEffectInstance e,string reason,float at){try{
            BelowCannonSamplesR786 samples;if(!BelowCannonScopeR786(e)||belowCannonFailuresR786>=3||!belowCannonSamplesR786.TryGetValue(e.serial,out samples)||samples.generation!=NpcPresentationGeneration||samples.retired)return;samples.retired=true;
            BelowCannonWriteR786(new BelowCannonLogR786{phase="retire-tail",spawn=samples.spawn,firstContact=samples.first,lastContact=samples.last,window=samples.window,retire=BelowCannonCopyR786(e,reason,at,e.definition.physical,e.definition.magic),contactSamples=samples.samples,actualApplyCalls=samples.applyCalls,positiveDealt=samples.positiveDealt});
        }catch{BelowCannonDiagnosticFailureR786();}}
#endif


        readonly Dictionary<string,NpcEffectDefinition> npcEffectDefinitions=new Dictionary<string,NpcEffectDefinition>(StringComparer.Ordinal);
        readonly Dictionary<string,NpcEffectBinding> npcEffectBindings=new Dictionary<string,NpcEffectBinding>(StringComparer.Ordinal);
        readonly Dictionary<string,NpcReactionEffectBinding> npcReactionEffectBindings=new Dictionary<string,NpcReactionEffectBinding>(StringComparer.Ordinal);
        struct NpcReactionAttachmentSnapshot {internal CombatState state;internal float clock,hitPause;}
        readonly Dictionary<Enemy,NpcReactionAttachmentSnapshot> npcReactionAttachmentSnapshots=new Dictionary<Enemy,NpcReactionAttachmentSnapshot>();
        sealed class NpcAttachedEffectSet {internal Enemy owner;internal NpcEffectSetDefinition definition;internal float bornAt;internal int next;}
        readonly List<NpcAttachedEffectSet> npcAttachedSets=new List<NpcAttachedEffectSet>();
        public readonly List<NpcEffectInstance> npcEffects=new List<NpcEffectInstance>();
        public readonly List<NpcEffectEvent> npcEffectEvents=new List<NpcEffectEvent>();
        public bool traceNpcEffects;int npcEffectSerial,npcEffectLiveCount;uint npcEffectRandom=0xa765239u;
        public void SetNpcEffectDefinitions(NpcEffectDocument document){
            var definitions=new Dictionary<string,NpcEffectDefinition>(StringComparer.Ordinal);var bindings=new Dictionary<string,NpcEffectBinding>(StringComparer.Ordinal);var reactions=new Dictionary<string,NpcReactionEffectBinding>(StringComparer.Ordinal);
            if(document!=null){
                if(document.definitions==null||document.bindings==null)throw new ArgumentException("Missing source NPC effect arrays");
                foreach(var d in document.definitions){
                    if(d==null||string.IsNullOrEmpty(d.id)||definitions.ContainsKey(d.id)||!Finite(d.lifetime)||d.lifetime<=0||d.lifetime>60||d.baseSpheres==null||d.baseSpheres.Length%4!=0||d.samples==null||d.samples.Length<2)throw new ArgumentException("Invalid original NPC effect");
                    for(int i=0;i<d.baseSpheres.Length;i++)if(!Finite(d.baseSpheres[i])||i%4==3&&d.baseSpheres[i]<=0)throw new ArgumentException("Invalid NPC effect helper");
                    float previous=-1;foreach(var s in d.samples){if(s==null||!Finite(s.time)||s.time<previous||s.time<0||!Finite(s.scaleX)||!Finite(s.scaleY)||!Finite(s.scaleZ)||!Finite(s.moveX)||!Finite(s.alpha)||s.alpha<0||s.alpha>1)throw new ArgumentException("Invalid NPC effect curve");previous=s.time;}
                    if(Math.Abs(d.samples[0].time)>.00001f||Math.Abs(previous-d.lifetime)>.00001f||!Finite(d.physical)||d.physical<0||!Finite(d.magic)||d.magic<0||!Finite(d.repeatGap)||d.repeatGap<=0||!Finite(d.lockOnUntil)||d.lockOnUntil<0||!Finite(d.lockCurveSpeed)||d.lockCurveSpeed<0||!Finite(d.yawMin)||!Finite(d.yawMax)||d.yawMax<d.yawMin||!Finite(d.launch)||!Finite(d.push)||!Finite(d.stopAttacker)||d.stopAttacker<0||!Finite(d.stopDefender)||d.stopDefender<0)throw new ArgumentException("Invalid effect timing or damage");
                    ValidateNpcEffectGeometry(d);ValidateNpcProjectile(d);ValidateAlteraCannonBigBallisticV18(d);ValidateAlteraRavenMaximumCombatV18(d);ValidateAlteraRavenMissileRainEffectV18(d);ValidateAlteraRavenCannonBladeV18(d);ValidateNpcPlayerStatus(d);ValidatePlainOriginalFxSourceR1304(d);definitions.Add(d.id,ConfigureCastleMissileContact(d));
                }
                BindDragonKingImpactEffects(definitions);
                foreach(var d in definitions.Values){
                    if(d.deathChildren!=null&&d.deathChildren.Length>0){
                        if(!string.IsNullOrEmpty(d.deathChild))throw new ArgumentException("NPC effect declares both legacy and source death children");
                        foreach(var child in d.deathChildren)if(child==null||string.IsNullOrEmpty(child.effect)||!Finite(child.offsetX)||!Finite(child.offsetY)||!Finite(child.offsetZ)||!Finite(child.rotationZ))throw new ArgumentException("Invalid source NPC effect child transform");
                    }
                    foreach(string child in NpcEffectChildIdsV18(d))if(!string.IsNullOrEmpty(child)&&!definitions.ContainsKey(child))throw new ArgumentException("Missing exact child effect "+child);
                    if(d.deathChildren!=null)foreach(var child in d.deathChildren)if(definitions[child.effect].projectilePhysics&&(child.applyParentRotation||Math.Abs(child.rotationZ)>.000001f))throw new ArgumentException("Rotated ballistic source child has no verified velocity-reset consumer");
                }
                foreach(var d in definitions.Values)ValidateEffectChildren(d,definitions,new HashSet<string>(),0);
                foreach(var b in document.bindings){
                    if(b==null||string.IsNullOrEmpty(b.npc)||string.IsNullOrEmpty(b.state)||!npcStates.ContainsKey(b.npc+"/"+b.state)||bindings.ContainsKey(b.npc+"/"+b.state)||!Finite(b.unitScale)||b.unitScale<=0||b.tracks==null||b.sets==null)throw new ArgumentException("Invalid original NPC effect state binding");
                    var bones=new HashSet<string>();foreach(var track in b.tracks){if(track==null||string.IsNullOrEmpty(track.bone)||!bones.Add(track.bone)||track.samples==null||track.samples.Length==0)throw new ArgumentException("Invalid source attachment track");foreach(var p in track.samples)if(p==null||!Finite(p.x)||!Finite(p.y)||!Finite(p.z))throw new ArgumentException("Invalid source attachment point");}
                    foreach(var set in b.sets){if(set==null||!Finite(set.attachAt)||set.attachAt<0||set.attachAt>npcStates[b.npc+"/"+b.state].duration||set.emissions==null)throw new ArgumentException("Invalid source EffectSet attachment");float at=-1;foreach(var e in set.emissions){if(e==null||!definitions.ContainsKey(e.effect)||!Finite(e.start)||e.start<at||e.start<0||!Finite(e.offsetX)||!Finite(e.offsetY)||!Finite(e.offsetZ)||!Finite(e.scaleX)||!Finite(e.scaleY)||!Finite(e.scaleZ)||e.scaleX<=0||e.scaleY<=0||e.scaleZ<=0||!string.IsNullOrEmpty(e.bone)&&!bones.Contains(e.bone))throw new ArgumentException("Invalid source EffectSet emission");at=e.start;}}
                    foreach(var set in b.sets)foreach(var e in set.emissions)if(!string.IsNullOrEmpty(e.ownerState)&&e.ownerState!=b.state)throw new ArgumentException("Owner-bound effect must name its source state");
                    foreach(var set in b.sets)foreach(var e in set.emissions){
                        if(!Finite(e.rotationZ))throw new ArgumentException("Invalid source emission rotation");
                        if(definitions[e.effect].projectilePhysics&&Math.Abs(e.rotationZ)>.000001f&&!SupportsAlteraCannonBigRotationV18(definitions[e.effect],e.rotationZ))throw new ArgumentException("Rotated ballistic emission has no verified velocity-reset consumer");
                        ValidateAlteraCannonBigEmissionV18(definitions[e.effect],b,e);ValidateAlteraRavenCannonBladeEmissionV18(definitions[e.effect],b,e);
                        if(Math.Abs(e.rotationZ)>.000001f&&(e.follow||definitions[e.effect].lockOnUntil>0))throw new ArgumentException("Rotated following or guided emission needs a verified transform consumer");
                    }
                    bindings.Add(b.npc+"/"+b.state,b);
                }
                if(document.reactionBindings!=null)foreach(var b in document.reactionBindings){
                    NpcDefinition profile;
                    if(b==null||string.IsNullOrEmpty(b.npc)||!npcDefinitions.TryGetValue(b.npc,out profile)||
                       (b.combatState!=CombatState.Hurt&&b.combatState!=CombatState.Knockdown&&b.combatState!=CombatState.GettingUp)||
                       reactions.ContainsKey(b.npc+"/"+(int)b.combatState)||string.IsNullOrEmpty(b.sourceState)||string.IsNullOrEmpty(b.sourceAnimation)||
                       !Finite(b.duration)||b.duration<=0||!Finite(b.sourceAnimSpeed)||b.sourceAnimSpeed<=0||b.tracks==null)
                        throw new ArgumentException("Invalid source reaction effect binding");
                    var body=Array.Find(profile.reactions,r=>r.combatState==b.combatState);
                    if(body==null||body.sourceState!=b.sourceState||body.sourceAnimation!=b.sourceAnimation||Math.Abs(body.duration-b.duration)>.00001f||Math.Abs(body.sourceAnimSpeed-b.sourceAnimSpeed)>.00001f)
                        throw new ArgumentException("Reaction effect binding differs from source reaction motion");
                    var requiredBones=new HashSet<string>();foreach(var normal in bindings.Values)if(normal.npc==b.npc)foreach(var track in normal.tracks)requiredBones.Add(track.bone);
                    var bones=new HashSet<string>();foreach(var track in b.tracks){
                        if(track==null||string.IsNullOrEmpty(track.bone)||!bones.Add(track.bone)||track.samples==null||track.samples.Length!=(int)Math.Ceiling(b.duration*60-.001f)+1)
                            throw new ArgumentException("Invalid source reaction attachment track");
                        foreach(var p in track.samples)if(p==null||!Finite(p.x)||!Finite(p.y)||!Finite(p.z))throw new ArgumentException("Invalid source reaction attachment point");
                    }
                    if(requiredBones.Count==0||!requiredBones.SetEquals(bones))throw new ArgumentException("Incomplete source reaction attachment bones");
                    reactions.Add(b.npc+"/"+(int)b.combatState,b);
                }
                // An old document with no reaction bindings remains supported.
                // Once supplied, every emitting NPC's three selected ground
                // reactions must be present; partial data must not silently
                // send a live attachment back to standing bones.
                if(reactions.Count>0){var emitters=new HashSet<string>();foreach(var b in bindings.Values)if(b.sets.Length>0)emitters.Add(b.npc);
                    foreach(string npc in emitters)foreach(var body in npcDefinitions[npc].reactions)
                        if(!reactions.ContainsKey(npc+"/"+(int)body.combatState))throw new ArgumentException("Missing source reaction attachment state");}
            }
            ClearPlainOriginalFxBindingsR1304();npcEffectDefinitions.Clear();npcEffectBindings.Clear();npcReactionEffectBindings.Clear();foreach(var d in definitions)npcEffectDefinitions.Add(d.Key,d.Value);foreach(var b in bindings)npcEffectBindings.Add(b.Key,b.Value);foreach(var b in reactions)npcReactionEffectBindings.Add(b.Key,b.Value);ResetNpcEffects();
            foreach(var enemy in enemies)enemy.npcAttachedEffectSets=new bool[0];
        }
        static IEnumerable<string> NpcEffectChildIdsV18(NpcEffectDefinition d){yield return d.contactChild;yield return d.deathChild;yield return d.hitChild;if(d.deathChildren!=null)foreach(var child in d.deathChildren)yield return child.effect;}
        static void ValidateEffectChildren(NpcEffectDefinition d,Dictionary<string,NpcEffectDefinition> all,HashSet<string> path,int depth){if(depth>16||!path.Add(d.id))throw new ArgumentException("Cyclic or excessive NPC effect child chain");foreach(string child in NpcEffectChildIdsV18(d))if(!string.IsNullOrEmpty(child))ValidateEffectChildren(all[child],all,path,depth+1);path.Remove(d.id);}
        void ClearNpcEffectWorld(){ClearPlainOriginalElementCallbackWorldR1315();ClearPlainOriginalFxStatusesR1304();ClearCoreHardCasterWorldV21();ClearCoreHardClosureStatusesV21();ClearCoreHardLocalBirthsV21();ClearAlteraAirshipStatePresentationV20();ClearAlteraRavenMissileRainV18();ClearAlteraRavenCannonBladeV18();ClearAlteraRavenMaximumReleaseV18();ClearNpcLakeVisualV18();ClearNpcSourceDeathChildrenV18();ClearNpcSourceLandingStampsV17();ClearNpcPresentationEvents();npcEffects.Clear();npcAttachedSets.Clear();npcReactionAttachmentSnapshots.Clear();npcEffectLiveCount=0;ClearNpcSoundWorld();ClearNpcSummons();ClearNpcOutskirtsActors();ClearNpcPlayerStatuses();}
        void ResetNpcEffects(){ClearNpcEffectWorld();npcEffectEvents.Clear();npcEffectSerial=0;npcEffectRandom=0xa765239u;}
        void SnapshotNpcEffectOwners(){npcReactionAttachmentSnapshots.Clear();foreach(var owner in enemies){owner.npcEffectPreviousState=owner.npcState;owner.npcEffectPreviousTime=owner.npcTime;owner.npcEffectPreviousX=owner.x;owner.npcEffectPreviousY=owner.y;owner.npcEffectSnapshotAt=elapsed;npcReactionAttachmentSnapshots[owner]=new NpcReactionAttachmentSnapshot{state=owner.state,clock=owner.clock,hitPause=owner.hitPause};}}
        void TraceNpcEffect(string type,NpcEffectInstance effect,int damage=0,float at=-1){RecordNpcPresentation(type,effect,at);if(!traceNpcEffects)return;if(npcEffectEvents.Count>=8192)npcEffectEvents.RemoveAt(0);npcEffectEvents.Add(new NpcEffectEvent{type=type,effect=effect.id,npc=effect.owner==null?"":effect.owner.sourceNPC,serial=effect.serial,damage=damage,time=at<0?elapsed:at,x=effect.x,y=effect.y,z=effect.z,age=effect.age,reflected=effect.reflected});}
        bool BelowCannonFractionalScopeR901(NpcEffectInstance effect){
            // R863's birth-only flag includes parent==null and exact two-owner provenance.
            return belowCandidateV21!=null&&NativeTerrain&&room==2&&CurrentRoom.id=="Dungeon_Altera_Below_Tunnel_Normal-2"&&effect.belowCannonPortGuidanceR863&&!effect.reflected&&effect.id=="CANNONFORT_PARASITE_CANNON_MISSILE"&&effect.owner!=null&&(effect.owner.sourceNPC=="NUI_CANNON_FORT_PARASITE_TOP_CANNON"||effect.owner.sourceNPC=="NUI_CANNON_FORT_PARASITE_BOTTOM_CANNON");
        }
        void TraceNpcFractionalHitR901(NpcEffectInstance effect,BelowCannonFractionalHitResultR901 result,float at){
            if(!Finite(result.appliedHpLoss)||result.appliedHpLoss<=0)throw new InvalidOperationException("R901 fractional HIT without real HP loss");
            RecordNpcPresentation("hit-player",effect,at);if(!traceNpcEffects)return;if(npcEffectEvents.Count>=8192)npcEffectEvents.RemoveAt(0);
            npcEffectEvents.Add(new NpcEffectEvent{type="hit-player",effect=effect.id,npc=effect.owner==null?"":effect.owner.sourceNPC,serial=effect.serial,damage=0,time=at<0?elapsed:at,x=effect.x,y=effect.y,z=effect.z,age=effect.age,reflected=effect.reflected,fractionalDamageAvailableR901=true,fractionalOutcomeR901=result.outcome,rawDamageFractionalR901=result.rawDamage,damageAfterDefenseFractionalR901=result.afterDefense,hpBeforeFractionalR901=result.hpBefore,hpAfterFractionalR901=result.hpAfter,appliedHpLossFractionalR901=result.appliedHpLoss});
        }
        NpcEffectBinding EffectBinding(Enemy enemy,string state){NpcEffectBinding b;return enemy!=null&&!string.IsNullOrEmpty(enemy.sourceNPC)&&!string.IsNullOrEmpty(state)&&npcEffectBindings.TryGetValue(enemy.sourceNPC+"/"+state,out b)?b:null;}
        void AttachNpcEffectSets(Enemy enemy,NpcStateDefinition state,float from,float to,float startAt){
            var binding=EffectBinding(enemy,state.id);if(binding==null)return;
            if(enemy.npcAttachedEffectSets.Length!=binding.sets.Length)enemy.npcAttachedEffectSets=new bool[binding.sets.Length];
            for(int i=0;i<binding.sets.Length;i++){var set=binding.sets[i];if(enemy.npcAttachedEffectSets[i]||set.attachAt<from-1e-6f||set.attachAt>to+1e-6f)continue;enemy.npcAttachedEffectSets[i]=true;npcAttachedSets.Add(new NpcAttachedEffectSet{owner=enemy,definition=set,bornAt=startAt+Math.Max(0,set.attachAt-from)});}
        }
        internal const float BombMonkeyDisplayScale = .92f;
        // The whole-pose atlas was baked at 48.15780693 / 36.63375451 logical
        // units. Its right-hand bone track is stored in original model units.
        const float BombMonkeyHandScale = (48.15780693f / 36.63375451f) * BombMonkeyDisplayScale;

        void NpcAttachment(Enemy owner,NpcEffectEmission emission,float at,out float x,out float y,out float z){
            x=owner.x;y=owner.y;z=0;var profile=NpcProfile(owner);var binding=EffectBinding(owner,owner.npcState);float clock=Math.Max(0,owner.npcTime-Math.Max(0,elapsed-at));
            if(elapsed>owner.npcEffectSnapshotAt&&at>=owner.npcEffectSnapshotAt-1e-6f){float u=Clamp((at-owner.npcEffectSnapshotAt)/(elapsed-owner.npcEffectSnapshotAt),0,1);x=owner.npcEffectPreviousX+(owner.x-owner.npcEffectPreviousX)*u;y=owner.npcEffectPreviousY+(owner.y-owner.npcEffectPreviousY)*u;if(owner.npcState==owner.npcEffectPreviousState)clock=owner.npcEffectPreviousTime+(owner.npcTime-owner.npcEffectPreviousTime)*u;}
            if(UsesWaterwayNasodEye(owner,emission.effect)){
                float eyeX,eyeY;
                WaterwayNasodEye.FireOffset(clock,out eyeX,out eyeY);
                x+=owner.facing*eyeX*owner.visualScale;
                y+=eyeY*owner.visualScale;
                return;
            }
            NpcReactionEffectBinding reaction;NpcAttachmentTrack[] tracks=null;float duration=0;
            if(!string.IsNullOrEmpty(owner.sourceNPC)&&npcReactionEffectBindings.TryGetValue(owner.sourceNPC+"/"+(int)owner.state,out reaction)){
                tracks=reaction.tracks;duration=reaction.duration;clock=ReactionAttachmentClock(owner,at);
            }else{
                if(binding==null&&profile!=null){binding=EffectBinding(owner,profile.waitState);clock=0;}
                if(binding!=null)tracks=binding.tracks;
            }
            if(tracks!=null&&!string.IsNullOrEmpty(emission.bone)){var track=Array.Find(tracks,t=>t.bone==emission.bone);if(track!=null){
                if(owner.npcState==OriginalRushTimerV45.WilliamRush&&binding!=null)clock%=NpcState(owner).duration;
                float f=Clamp(clock*60,0,track.samples.Length-1);int a=(int)f,b=Math.Min(a+1,track.samples.Length-1);float blend=f-a;
                if(duration>0){if(clock>=duration){a=b=track.samples.Length-1;blend=0;}else{float start=a/60f,end=Math.Min(duration,b/60f);blend=end>start?Clamp((clock-start)/(end-start),0,1):0;}}
                float boneScale=owner.visualScale;
                if(owner.sourceNPC=="NUI_MONKEY_BOMB" && emission.effect=="DamageEffect_MONKEY_BOMB_ATTACK_A" && emission.bone=="Dummy01_Rhand")
                    boneScale*=BombMonkeyHandScale;
                var p=track.samples[a];var q=track.samples[b];x+=owner.facing*(p.x+(q.x-p.x)*blend)*boneScale;y+=NativeHoverLiftV58(owner)+(p.y+(q.y-p.y)*blend)*boneScale;z+=(p.z+(q.z-p.z)*blend)*boneScale;
            }}
            x+=owner.facing*emission.offsetX*owner.visualScale;y+=emission.offsetY*owner.visualScale;z+=emission.offsetZ*owner.visualScale;
        }
        float ReactionAttachmentClock(Enemy owner,float at){
            NpcReactionAttachmentSnapshot previous;float span=elapsed-owner.npcEffectSnapshotAt;
            if(span>0&&at>=owner.npcEffectSnapshotAt-1e-6f&&npcReactionAttachmentSnapshots.TryGetValue(owner,out previous)&&previous.state==owner.state){
                // TickEnemy consumes its own hit pause before advancing the
                // pose. Preserve that frozen prefix when an emission falls
                // inside a long simulation packet; linear interpolation
                // would advance the pose during the frozen portion.
                if(Math.Abs(owner.clock-previous.clock)<.000001f)return owner.clock;
                float expectedEnd=previous.clock+Math.Max(0,span-Math.Min(previous.hitPause,span));
                if(owner.clock>=previous.clock&&Math.Abs(owner.clock-expectedEnd)<.0001f){
                    float local=Clamp(at-owner.npcEffectSnapshotAt,0,span),advance=Math.Max(0,local-Math.Min(previous.hitPause,span));
                    return Math.Min(owner.clock,previous.clock+advance);
                }
                // Another hit can restart Hurt without changing its enum.
                // A reset clock must not be blended with the earlier Hurt.
            }
            // State changes follow the existing model's packet semantics.
            // New reaction clocks reset at entry; no clock from the previous
            // attack/reaction is blended into this selected source motion.
            return Math.Max(0,owner.clock-Math.Max(0,elapsed-at));
        }
        float EffectRandom(){npcEffectRandom^=npcEffectRandom<<13;npcEffectRandom^=npcEffectRandom>>17;npcEffectRandom^=npcEffectRandom<<5;return(npcEffectRandom%1000000)/1000000f;}
        NpcEffectInstance SpawnNpcEffect(Enemy owner,NpcEffectDefinition definition,NpcEffectEmission emission,float bornAt,NpcEffectInstance parent=null){
            if(npcEffectLiveCount>=512)throw new InvalidOperationException("NPC effect budget exceeded");
            var profile=NpcProfile(owner);var binding=EffectBinding(owner,owner.npcState);if(binding==null&&profile!=null)binding=EffectBinding(owner,profile.waitState);
            var effect=new NpcEffectInstance{id=definition.id,definition=definition,owner=owner,emission=emission,serial=++npcEffectSerial,bornAt=bornAt,updatedAt=bornAt,lifetime=definition.lifetime,
                facing=parent==null?owner.facing:parent.facing,ownerScale=parent==null?(binding==null?1:binding.unitScale)*owner.visualScale:parent.ownerScale,
                physicalAttack=parent==null?(profile==null?0:profile.physicalAttack):parent.physicalAttack,magicAttack=parent==null?(profile==null?0:profile.magicAttack):parent.magicAttack,
                reflected=parent!=null&&parent.reflected};
            if(parent==null){NpcAttachment(owner,emission,bornAt,out effect.x,out effect.y,out effect.z);effect.scaleX=emission.scaleX;effect.scaleY=emission.scaleY;effect.scaleZ=emission.scaleZ;}
            else{effect.x=parent.x;effect.y=parent.y;effect.z=parent.z;}
            if(parent==null&&emission!=null&&emission.facingMultiplier==-1)effect.facing=-effect.facing;
            SetCastleBeamOrigin(effect);
            CaptureNpcSourceLandArgumentV18(effect,parent,emission);
            effect.dirX=effect.facing;effect.yaw=definition.randomYaw?(definition.yawMin+(definition.yawMax-definition.yawMin)*EffectRandom())*(float)Math.PI/180:0;
            if(definition.lockOnUntil>0&&(emission==null||emission.lockOnMode==0)){float tx,ty;if(NpcEffectTarget(effect,out tx,out ty)){float dx=tx-effect.x,dy=ty-effect.y,len=(float)Math.Sqrt(dx*dx+dy*dy);if(len>.001f){effect.dirX=dx/len;effect.dirY=dy/len;}}}
            if(emission!=null)ApplyNpcSourceEmissionRotationV18(effect,emission.rotationZ);
            InitializeBelowCannonPortGuidanceR863(effect,parent);
            InitializeNpcProjectile(effect);effect.alpha=EffectAt(definition,0).alpha;npcEffects.Add(effect);npcEffectLiveCount++;BeginNpcEffectSounds(effect);TraceNpcEffect("spawn",effect,0,bornAt);
#if UNITY_EDITOR
            BelowCannonSpawnDiagnosticR786(effect,bornAt);
#endif
            return effect;
        }
        struct EffectPose {internal float x,y,z,alpha,move;}
        static EffectPose EffectAt(NpcEffectDefinition definition,float age){
            var samples=definition.samples;int lo=0,hi=samples.Length;
            while(lo<hi){int mid=(lo+hi)/2;if(samples[mid].time<=age)lo=mid+1;else hi=mid;}
            int a=Math.Max(0,lo-1),b=Math.Min(a+1,samples.Length-1);float span=samples[b].time-samples[a].time,u=span<=0?0:Clamp((age-samples[a].time)/span,0,1);var p=samples[a];var q=samples[b];
            return new EffectPose{x=p.scaleX+(q.scaleX-p.scaleX)*u,y=p.scaleY+(q.scaleY-p.scaleY)*u,z=p.scaleZ+(q.scaleZ-p.scaleZ)*u,alpha=p.alpha+(q.alpha-p.alpha)*u,move=p.moveX+(q.moveX-p.moveX)*u};
        }
        bool NpcEffectTarget(NpcEffectInstance effect,out float x,out float y){
            if(TryMine2SourceFixedTargetV20(effect,out x,out y))return true;if(TryDragonFixedTargetV20(effect,out x,out y))return true;
            if(!effect.reflected){x=player.x;y=player.y+21;return player.hp>0;}
            x=effect.x;y=effect.y;float distance=float.PositiveInfinity;
            foreach(var enemy in enemies){
                if(!enemy.active||enemy.dormant||enemy.passive||enemy.hp<=0)continue;
                float targetY=enemy.y+24;
                if(NativeHoverFloorV58(enemy.sourceNPC)>0){
                    var body=EnemyBounds(enemy);
                    if(!Finite(body.top)||!Finite(body.bottom))continue;
                    targetY=(body.top+body.bottom)*.5f;
                }
                float dx=enemy.x-effect.x,dy=targetY-effect.y,d=dx*dx+dy*dy;
                if(d<distance){distance=d;x=enemy.x;y=targetY;}
            }
            return !float.IsPositiveInfinity(distance);
        }
        void TurnNpcEffect(NpcEffectInstance effect,float dt){
            if(effect.emission!=null&&effect.emission.lockOnMode==1&&!IsCastleGuidedMissile(effect))return;
            if(effect.definition.lockOnUntil<=effect.age||effect.definition.lockCurveSpeed<=0)return;float x,y;if(!NpcEffectTarget(effect,out x,out y))return;
            if(TurnBelowCannonPortGuidanceR863(effect,x,y,dt))return;
            float current=(float)Math.Atan2(effect.dirY,effect.dirX),wanted=(float)Math.Atan2(y-effect.y,x-effect.x),difference=wanted-current;
            while(difference>Math.PI)difference-=(float)Math.PI*2;while(difference< -Math.PI)difference+=(float)Math.PI*2;
            float turn=effect.definition.lockCurveSpeed*Math.Min(dt,effect.definition.lockOnUntil-effect.age),angle=current+Clamp(difference,-turn,turn);effect.dirX=(float)Math.Cos(angle);effect.dirY=(float)Math.Sin(angle);
        }
        // R863 explicit2.5D port policy: original native binding ABI remains unknown.
        // The original birthXYZ/XYheading and age0 contact are unchanged.
        void InitializeBelowCannonPortGuidanceR863(NpcEffectInstance effect,NpcEffectInstance parent){
            var e=effect.emission;var d=effect.definition;var owner=effect.owner;
            effect.belowCannonPortGuidanceR863=belowCandidateV21!=null&&parent==null&&owner!=null&&e!=null&&e.lockOnMode==0&&!e.follow&&!d.projectilePhysics&&!d.reflectable&&effect.id=="CANNONFORT_PARASITE_CANNON_MISSILE"&&(owner.sourceNPC=="NUI_CANNON_FORT_PARASITE_TOP_CANNON"||owner.sourceNPC=="NUI_CANNON_FORT_PARASITE_BOTTOM_CANNON");
            effect.belowCannonPortDirectionZR863=0;
            if(effect.belowCannonPortGuidanceR863)ValidateBelowCannonPortGuidanceR863(effect);
        }
        void ValidateBelowCannonPortGuidanceR863(NpcEffectInstance effect){
            if(!Finite(effect.x)||!Finite(effect.y)||!Finite(effect.z)||!Finite(effect.dirX)||!Finite(effect.dirY)||!Finite(effect.belowCannonPortDirectionZR863)||!Finite(effect.age)||!Finite(effect.definition.lockOnUntil)||!Finite(effect.definition.lockCurveSpeed)||effect.definition.lockOnUntil<=0||effect.definition.lockCurveSpeed<=0)
                throw new InvalidOperationException("R863 nonfinite or invalid Cannon port guidance");
            double x=effect.dirX,y=effect.dirY,z=effect.belowCannonPortDirectionZR863;
            if(x*x+y*y+z*z<=0)throw new InvalidOperationException("R863 zero Cannon port heading");
        }
        bool TurnBelowCannonPortGuidanceR863(NpcEffectInstance effect,float targetX,float targetY,float dt){
            if(!effect.belowCannonPortGuidanceR863)return false;
            ValidateBelowCannonPortGuidanceR863(effect);
            if(!Finite(targetX)||!Finite(targetY)||!Finite(dt)||dt<0)throw new InvalidOperationException("R863 invalid Cannon port target or step");
            // Preserve the original planar arithmetic, including its zero-target rule.
            if(effect.z==0&&effect.belowCannonPortDirectionZR863==0)return false;
            double hx=effect.dirX,hy=effect.dirY,hz=effect.belowCannonPortDirectionZR863;
            double length=Math.Sqrt(hx*hx+hy*hy+hz*hz);hx/=length;hy/=length;hz/=length;
            double wx=(double)targetX-effect.x,wy=(double)targetY-effect.y,wz=-(double)effect.z;
            double wantedLength=Math.Sqrt(wx*wx+wy*wy+wz*wz);
            if(wantedLength==0||dt==0)return true; // no invented direction at an attained target
            wx/=wantedLength;wy/=wantedLength;wz/=wantedLength;
            double dot=Math.Max(-1,Math.Min(1,hx*wx+hy*wy+hz*wz));
            double ax=hy*wz-hz*wy,ay=hz*wx-hx*wz,az=hx*wy-hy*wx;
            double crossLength=Math.Sqrt(ax*ax+ay*ay+az*az),angle=Math.Atan2(crossLength,dot);
            double limit=effect.definition.lockCurveSpeed*Math.Min(dt,effect.definition.lockOnUntil-effect.age);
            double turn=Math.Min(angle,limit),nx,ny,nz;
            if(angle<=limit){nx=wx;ny=wy;nz=wz;}
            else{
                if(crossLength==0){
                    // Here angle>limit: exact reverse. Preserve the declared tangent tie-break.
                    double tx=0,ty=0,tz=0;
                    if(Math.Abs(hx)<=Math.Abs(hy)&&Math.Abs(hx)<=Math.Abs(hz))tx=1;
                    else if(Math.Abs(hy)<=Math.Abs(hz))ty=1;else tz=1;
                    double projection=tx*hx+ty*hy+tz*hz;
                    tx-=hx*projection;ty-=hy*projection;tz-=hz*projection;
                    ax=hy*tz-hz*ty;ay=hz*tx-hx*tz;az=hx*ty-hy*tx;
                    crossLength=Math.Sqrt(ax*ax+ay*ay+az*az);
                }
                ax/=crossLength;ay/=crossLength;az/=crossLength;
                double c=Math.Cos(turn),s=Math.Sin(turn),along=ax*hx+ay*hy+az*hz;
                nx=hx*c+(ay*hz-az*hy)*s+ax*along*(1-c);
                ny=hy*c+(az*hx-ax*hz)*s+ay*along*(1-c);
                nz=hz*c+(ax*hy-ay*hx)*s+az*along*(1-c);
            }
            double norm=Math.Sqrt(nx*nx+ny*ny+nz*nz);
            effect.dirX=(float)(nx/norm);effect.dirY=(float)(ny/norm);effect.belowCannonPortDirectionZR863=(float)(nz/norm);
            return true;
        }
        bool MoveBelowCannonPortGuidanceR863(NpcEffectInstance effect,float distance){
            if(!effect.belowCannonPortGuidanceR863)return false;
            ValidateBelowCannonPortGuidanceR863(effect);
            if(!Finite(distance)||distance<0)throw new InvalidOperationException("R863 invalid Cannon source movement delta");
            if(effect.z==0&&effect.belowCannonPortDirectionZR863==0)return false;
            if(distance==0)return true;
            // Normalize the stored binary32 heading for the total3D arc budget.
            double x=effect.dirX,y=effect.dirY,z=effect.belowCannonPortDirectionZR863;
            double unit=distance/Math.Sqrt(x*x+y*y+z*z);
            effect.x=(float)((double)effect.x+x*unit);effect.y=(float)((double)effect.y+y*unit);effect.z=(float)((double)effect.z+z*unit);
            return true;
        }
        void NpcEffectSphere(NpcEffectInstance effect,EffectPose pose,int index,out float x,out float y,out float z,out float radius){
            float sx=pose.x*effect.scaleX,sy=pose.y*effect.scaleY,sz=pose.z*effect.scaleZ;
            float a=NpcEffectGeometryValue(effect,false,index)*sx,b=NpcEffectGeometryValue(effect,false,index+2)*sz,cs=(float)Math.Cos(effect.yaw),sn=(float)Math.Sin(effect.yaw);
            float relativeX=effect.facing*(a*cs+b*sn)*effect.ownerScale,relativeY=NpcEffectGeometryValue(effect,false,index+1)*sy*effect.ownerScale;
            if(IsCastleGuidedMissile(effect)){
                // The check follows the nose even when the missile has turned around.
                relativeX=a*effect.dirX*effect.ownerScale;
                relativeY=a*effect.dirY*effect.ownerScale;
            }
            ApplyNpcSourceEffectRotationV18(effect,ref relativeX,ref relativeY);
            x=effect.x+relativeX;y=effect.y+relativeY;z=effect.z+(-a*sn+b*cs)*effect.ownerScale;
            radius=NpcEffectGeometryValue(effect,false,index+3)*Math.Max(Math.Abs(sx),Math.Max(Math.Abs(sy),Math.Abs(sz)))*effect.ownerScale;
        }
        public float[] NpcEffectSpheres(NpcEffectInstance effect){var result=new float[effect.definition.baseSpheres.Length];var p=EffectAt(effect.definition,effect.age);for(int i=0;i<result.Length;i+=4)NpcEffectSphere(effect,p,i,out result[i],out result[i+1],out result[i+2],out result[i+3]);return result;}
        public bool NpcEffectCanDamage(NpcEffectInstance effect){var d=effect.definition;return effect.age>=d.damageFrom&&(d.damageUntil<=0||effect.age<=d.damageUntil)&&(d.physical>0||d.magic>0||!string.IsNullOrEmpty(d.contactChild));}
        public struct NpcEffectVisualState {public string id;public float scaleX,scaleY,alpha,angle;}
        public NpcEffectVisualState NpcEffectVisual(NpcEffectInstance effect){return NpcEffectVisual(effect,effect.age);}
        public NpcEffectVisualState NpcEffectVisual(NpcEffectInstance effect,float visualAge){var p=EffectAt(effect.definition,visualAge);bool mesh=effect.definition.visualFromMesh;return new NpcEffectVisualState{id=effect.definition.visualId,scaleX=effect.ownerScale*effect.scaleX*(mesh?p.x:1),scaleY=effect.ownerScale*effect.scaleY*(mesh?p.y:1),alpha=mesh?p.alpha:1,angle=(float)(Math.Atan2(effect.dirY,effect.dirX)*180/Math.PI)-(effect.facing<0?180:0)};}
    }
}
