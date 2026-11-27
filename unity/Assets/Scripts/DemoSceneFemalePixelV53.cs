using System;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class FemalePixelAssemblyV53 {
            public string schema,sourceNPC;public float[] sourcePlace;
            public float sourceLogicalUnit,fps,duration;public int frameCount;public FemalePixelRootV53[] roots;
        }
        [Serializable] sealed class FemalePixelRootV53 {
            public int rootIndex;public string kind,rootName,root;public float nativeUnitScale,neutralBodyHeight;public float[] rootNativeXYZ;
        }
        sealed class FemalePixelOwnerV53 {
            public Enemy actor;public SpriteRenderer carrier;public RoomDefinition room;public Transform nativeRoot;
            public FemalePixelAssemblyV53 assembly;public MonsterPixelFramesV53[] libraries;public MonsterPixelViewV53[] views;
            public Sprite[] sprites;public int[] pages,indices;public Vector3[] nativePoints;
            public float sourceClock;public int selectedFrame=-1;public bool originalCarrierEnabled;
        }
        FemalePixelOwnerV53 femalePixelOwnerV53;

        bool FemalePixelOwnsV53(Enemy actor){return femalePixelOwnerV53!=null&&ReferenceEquals(femalePixelOwnerV53.actor,actor)&&femalePixelOwnerV53.selectedFrame==Time.frameCount;}
        void DisposeFemalePixelV53(){
            var owner=femalePixelOwnerV53;femalePixelOwnerV53=null;if(owner==null)return;
            if(owner.views!=null)foreach(var view in owner.views)if(view!=null){monsterPixelRenderersV53.Remove(view.renderer);view.Dispose();}
            if(owner.carrier)owner.carrier.enabled=owner.originalCarrierEnabled;
        }
        FemalePixelOwnerV53 LoadFemalePixelV53(Enemy actor,SpriteRenderer carrier,RoomDefinition room,MonsterPixelActorV53 definition){
            var text=Resources.Load<TextAsset>(definition.root.TrimEnd('/')+"/assembly");
            if(!text)throw new InvalidOperationException("Missing six-root female assembly");
            var data=JsonUtility.FromJson<FemalePixelAssemblyV53>(text.text);
            if(data==null||data.sourceNPC!=actor.sourceNPC||data.sourcePlace==null||data.sourcePlace.Length!=3||data.roots==null||data.roots.Length!=6||data.fps!=60||data.frameCount!=681||Math.Abs(data.duration-FemaleNativeDoubleV28(FemaleNativeDurationBitsV28))>.00002||Math.Abs(data.sourceLogicalUnit-.28f)>.000001f)throw new InvalidOperationException("Female original six-root timing/placement contract changed");
            for(int k=0;k<3;k++)if(Math.Abs(data.sourcePlace[k]-FemaleNativeDoubleV28(FemaleNativePlaceBitsV28[k]))>.001)throw new InvalidOperationException("Female sourcePlace changed");
            var owner=new FemalePixelOwnerV53{actor=actor,carrier=carrier,room=room,nativeRoot=nativeRoot,assembly=data,libraries=new MonsterPixelFramesV53[6],views=new MonsterPixelViewV53[6],sprites=new Sprite[6],pages=new int[6],indices=new int[6],nativePoints=new Vector3[6],originalCarrierEnabled=femaleNativeDisplayV28!=null?femaleNativeDisplayV28.originalLegacyEnabled:carrier.enabled};
            var roots=new System.Collections.Generic.HashSet<int>();
            for(int i=0;i<6;i++){
                var part=data.roots[i];if(part==null||part.rootIndex<12||part.rootIndex>17||!roots.Add(part.rootIndex)||part.rootNativeXYZ==null||part.rootNativeXYZ.Length!=data.frameCount*3||part.nativeUnitScale!=1||string.IsNullOrEmpty(part.kind))throw new InvalidOperationException("Female incomplete root/sample inventory");
                foreach(float value in part.rootNativeXYZ)if(!MonsterPixelFramesV53.Finite(value))throw new InvalidOperationException("Female nonfinite native root track");
                string path=string.IsNullOrEmpty(part.root)?"Pixel/monster-v53/"+part.kind+"/":part.root;
                var registration=new MonsterPixelActorV53{npc=actor.sourceNPC,kind=part.kind,root=path,standingMotion="WaitStart",standingHeight=part.neutralBodyHeight,scale=1,authoredFacing=1,facingPolicy="fixed"};
                var library=owner.libraries[i]=MonsterPixelLibraryV53(registration);
                if(!library.TryClip("WaitStart",out var clip)||clip.frames.Length!=data.frameCount||Math.Abs(clip.duration-data.duration)>.00002f)throw new InvalidOperationException("Female root source clock differs from assembly");
            }
            return owner;
        }
        bool TrySelectFemalePixelV53(Enemy actor,SpriteRenderer carrier,RoomDefinition room){
            if(!MonsterPixelRegisteredV53(actor,out var definition)||definition.assembly!="female-six-root"){DisposeFemalePixelV53();return false;}
            try{
                var owner=femalePixelOwnerV53;
                if(owner!=null&&(!ReferenceEquals(owner.actor,actor)||owner.carrier!=carrier||!ReferenceEquals(owner.room,room)||owner.nativeRoot!=nativeRoot)){DisposeFemalePixelV53();owner=null;}
                if(owner==null)owner=LoadFemalePixelV53(actor,carrier,room,definition);
                float clock=(float)Math.Min(actor.clock,FemaleNativeDoubleV28(FemaleNativeDurationBitsV28));
                // Resolve every body before taking ownership, so one missing
                // page never removes only part of the original six-root group.
                for(int i=0;i<6;i++){
                    owner.sprites[i]=owner.libraries[i].At("WaitStart",clock,out owner.indices[i],out owner.pages[i]);
                    var part=owner.assembly.roots[i];int n=owner.indices[i]*3;
                    owner.nativePoints[i]=new Vector3(owner.assembly.sourcePlace[0]+part.rootNativeXYZ[n],owner.assembly.sourcePlace[1]+part.rootNativeXYZ[n+1],owner.assembly.sourcePlace[2]+part.rootNativeXYZ[n+2]);
                }
                if(femalePixelOwnerV53==null){
                    DisposeFemaleNativeDisplayV28();femalePixelOwnerV53=owner;
                    for(int i=0;i<6;i++){var view=owner.views[i]=new MonsterPixelViewV53(carrier,DestroyNativeWorldObject);view.renderer.name="Female complete pixel root "+owner.assembly.roots[i].rootIndex;monsterPixelRenderersV53.Add(view.renderer);}
                }
                owner.sourceClock=clock;owner.selectedFrame=Time.frameCount;carrier.enabled=owner.originalCarrierEnabled;carrier.forceRenderingOff=true;
                ObserveMonsterPixelV53(actor,"Female/NativeSixRoots","WaitStart","WaitStart",clock,"selected","six-complete-bodies-original-native-root-tracks",owner.indices[0],owner.pages[0]);return true;
            }catch(Exception error){DisposeFemalePixelV53();FailMonsterPixelV53(actor,"Female/NativeSixRoots","WaitStart","WaitStart",actor.clock,error);return false;}
        }
        // A complete body retains the shared side-view pixel art. Its root is
        // projected through the exact native camera, including scene depth, then
        // placed on the same viewport rectangle as the native scenery quad.
        // This proves root/spacing/scale correspondence, not a rotating 3D mesh's
        // per-vertex silhouette under every camera yaw.
        void ProjectFemalePixelV53(){
            var owner=femalePixelOwnerV53;if(owner==null)return;
            if(!NativeWorldActive||!nativeCamera||!nativeScreen||!worldCamera||owner.selectedFrame!=Time.frameCount){DisposeFemalePixelV53();return;}
            var quad=nativeScreen.transform;float scale=quad.lossyScale.y/(2*nativeCamera.orthographicSize*owner.assembly.sourceLogicalUnit);
            var parentScale=owner.carrier.transform.parent.lossyScale;
            if(Math.Abs(parentScale.x-parentScale.y)>.0001f||parentScale.y<=0)throw new InvalidOperationException("Female pixel parent needs uniform positive projection scale");
            scale/=parentScale.y;
            for(int i=0;i<6;i++){
                var uv=nativeCamera.WorldToViewportPoint(owner.nativeRoot.TransformPoint(owner.nativePoints[i]));
                var point=quad.TransformPoint(new Vector3(uv.x-.5f,uv.y-.5f,0));point.z=owner.carrier.transform.position.z;
                var view=owner.views[i];view.Apply(owner.sprites[i],point,scale,1,owner.actor.active&&!owner.actor.dormant&&uv.z>nativeCamera.nearClipPlane);
                // Native meshes used depth testing. The 2D complete bodies use
                // the same captured native scene depth and stable near-over-far
                // order among the six group members.
                int order=owner.carrier.sortingOrder;
                for(int j=0;j<6;j++)if(j!=i){var other=nativeCamera.WorldToViewportPoint(owner.nativeRoot.TransformPoint(owner.nativePoints[j]));if(other.z>uv.z||other.z==uv.z&&j<i)order++;}
                view.renderer.sortingOrder=order;
                BindProjectedSceneOcclusion(view.renderer,Vector3.zero,0,uv.z);
                ObserveMonsterPixelV53(owner.actor,"Female/NativeRoot"+owner.assembly.roots[i].rootIndex,"WaitStart","WaitStart",owner.sourceClock,"rendered","native-root-projection-fixed-side-view-body",owner.indices[i],owner.pages[i]);
            }
        }
    }
}
