#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class MonsterStylePinV54 {public string path,sha256;public long bytes;}
        [Serializable] sealed class MonsterStylePageV54 {
            public string motion,image;public int frame,page,totalPages,visiblePixels,residentBeforeTrim,residentAfterTrim,residentAfterRelease;public float sourceClock;
            public bool passed,pointFiltered,noMipMaps;
        }
        [Serializable] sealed class MonsterStyleIdentityV54 {
            public string npc,kind,standingMotion;public bool passed,portraitIsStandingZero,materialChecked;
            public int portraitFrame,portraitPage;public MonsterStylePinV54 library;
            public MonsterPixelIdentityCheckV53 material;public MonsterStylePageV54 highestPage;
        }
        [Serializable] sealed class MonsterStyleCheckV54 {
            public string schema="monster-pixel-v54-installed-style-smoke-1",scope="Installed style/material, preserved Benders parity, selected last-page imports and six-root projection; no route or source-selector coverage claim",error,rosterSha256;
            public bool passed,allSourceClocksRechecked,allRoutesRechecked;
            public int checks,identitiesReleased,portraitCases,materialCases,highestPageCases;
            public MonsterPixelCheckReportV53 preservedBenders;
            public MonsterStyleIdentityV54[] identities;
            public FemalePixelCheckV53 femaleProjection;
            public MonsterStylePinV54[] implementation;
        }
        static MonsterStylePinV54 PinMonsterStyleFileV54(string path){
            path=Path.GetFullPath(path);var bytes=File.ReadAllBytes(path);
            using(var sha=System.Security.Cryptography.SHA256.Create())return new MonsterStylePinV54{path=path,bytes=bytes.LongLength,sha256=BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant()};
        }

        // A deliberately small installed-pixel check. Complete animation contracts
        // and every atlas rectangle are verified by the external v54 full audit.
        public static void ReviewMonsterPixelStyleV54(){
            var args=Environment.GetCommandLineArgs();int flag=Array.IndexOf(args,"--monster-pixel-style-review-v54");
            if(flag<0||flag+1>=args.Length||args[flag+1].StartsWith("--",StringComparison.Ordinal)){
                Debug.LogError("Required: --monster-pixel-style-review-v54 <output-directory>");EditorApplication.Exit(1);return;
            }
            string output=Path.GetFullPath(args[flag+1]);Directory.CreateDirectory(output);
            var report=new MonsterStyleCheckV54();var rows=new List<MonsterStyleIdentityV54>();GameObject fixture=null;DemoScene scene=null;int exit=1;
            void Need(bool value,string reason){report.checks++;if(!value)throw new InvalidOperationException("V54 installed style smoke: "+reason);}
            void Write(){report.identities=rows.ToArray();File.WriteAllText(Path.Combine(output,"style-runtime-check.json"),JsonUtility.ToJson(report,true));}
            try{
                var pins=new List<MonsterStylePinV54>();
                foreach(string path in new[]{"Assets/Scripts/MonsterPixelStyleCheckV54.cs","Assets/Scripts/MonsterPixelRuntimeCheckV53.cs","Assets/Scripts/MonsterPixelFemaleCheckV53.cs","Assets/Scripts/MonsterPixelFramesV53.cs","Assets/Scripts/DemoSceneMonsterPixelV53.cs","Assets/Resources/Pixel/monster-v53/roster.json"})pins.Add(PinMonsterStyleFileV54(path));
                report.implementation=pins.ToArray();
                fixture=new GameObject("Installed v54 style smoke");fixture.SetActive(false);scene=fixture.AddComponent<DemoScene>();scene.ReadMonsterPixelRosterV53();report.rosterSha256=scene.MonsterPixelRosterShaV53;
                var cases=new[]{
                    "NUI_BENDERS_NORMAL/benders","NUI_CHICKEN/chicken","NUI_THIEF_THIN_ZERO/thief","NUI_RAVEN_CLONE/raven-clone",
                    "NUI_ANGER_FOREST_ENT/anger-forest-ent","NUI_NASOD_KING/nasod-king","NUI_BAT_A/bat-a",
                    "NUI_BENDERS_NASOD/benders-nasod","NUI_CANNON_BIG/cannon-big",
                    "NUI_AIRSHIP_SOLDIER_SWORDMAN/airship-soldier-swordman","NUI_BENDERS_UPGRADE_NORMAL/benders-upgrade-normal"
                };
                var lastPageKinds=new HashSet<string>{"airship-soldier-swordman","benders-nasod","benders-upgrade-normal","cannon-big"};
                using(var gpu=new MonsterPixelGpuCheckV53(output))foreach(string identity in cases){
                    Need(scene.monsterPixelActorsV53.TryGetValue(identity,out var registration),"Required exact registration missing: "+identity);
                    bool benders=identity=="NUI_BENDERS_NORMAL/benders";
                    if(benders)Need(registration.root=="Pixel/tree-v52/benders/"&&registration.scale==1&&registration.rootOffsetX==0&&registration.rootOffsetY==0&&registration.facingPolicy=="actor"&&registration.authoredFacing==1,"Approved v52 Benders registration changed");
                    var row=new MonsterStyleIdentityV54{npc=registration.npc,kind=registration.kind,standingMotion=registration.standingMotion};rows.Add(row);
                    MonsterPixelFramesV53 library=null;SpriteRenderer carrier=null;
                    try{
                        row.library=PinMonsterStyleFileV54("Assets/Resources/"+registration.root.TrimEnd('/')+"/library.json");
                        library=scene.MonsterPixelLibraryV53(registration);carrier=gpu.Carrier();
                        var actor=new Enemy{sourceNPC=registration.npc,kind=registration.kind,active=true,hp=100,maxHp=100,visualScale=1,facing=1,state=CombatState.Normal};
                        Need(scene.TryMonsterPixelPortraitV53(actor,out var portrait),"Danger portrait missing: "+row.kind);
                        var standing=library.At(registration.standingMotion,0,out int frame,out int page);
                        row.portraitFrame=frame;row.portraitPage=page;row.portraitIsStandingZero=portrait==standing&&frame==0;
                        Need(row.portraitIsStandingZero,"Danger portrait is not installed standing frame 0: "+row.kind);report.portraitCases++;
                        if(benders){
                            report.preservedBenders=new MonsterPixelCheckReportV53();gpu.VerifyAcceptedParity(library,carrier,report.preservedBenders,Need);
                            Need(report.preservedBenders.gpuPairs==48,"Accepted Benders must retain all 48 GPU motion/facing pairs");report.preservedBenders.passed=true;
                        }else if(row.kind!="airship-soldier-swordman"&&row.kind!="benders-upgrade-normal"){
                            row.material=new MonsterPixelIdentityCheckV53{npc=row.npc,kind=row.kind,clockReference="standing source frame 0; style-only GPU fixture"};
                            gpu.VerifyInstalledMaterial(library,carrier,registration.standingMotion,row.material,Need);row.material.passed=row.materialChecked=true;report.materialCases++;
                        }
                        if(lastPageKinds.Contains(row.kind)){
                            row.highestPage=CheckMonsterStyleHighestPageV54(gpu,library,carrier,output,Need);report.highestPageCases++;
                        }
                        row.passed=true;
                    }finally{
                        scene.monsterPixelPortraitLibraryV53=null;scene.monsterPixelPortraitPageV53=-1;
                        if(carrier)Object.DestroyImmediate(carrier.gameObject);
                        if(library!=null){library.Trim(new HashSet<int>(),0);library.Dispose();scene.monsterPixelLibrariesV53.Remove(identity);}
                        EditorUtility.UnloadUnusedAssetsImmediate();report.identitiesReleased++;Write();
                    }
                }
                report.femaleProjection=CheckFemalePixelProjectionV53(Path.Combine(output,"female-six-root"));
                Need(report.femaleProjection.passed,"Existing six-root projection fixture failed: "+report.femaleProjection.error);
                Need(report.femaleProjection.roots==6,"Female assembly lost a source root");
                Need(report.materialCases==8&&report.highestPageCases==4&&report.portraitCases==11,"Style smoke denominator changed");
                report.passed=true;exit=0;
            }catch(Exception error){report.error=error.ToString();Debug.LogException(error);}
            finally{
                try{if(scene)scene.DisposeMonsterPixelViewsV53();if(fixture)Object.DestroyImmediate(fixture);EditorUtility.UnloadUnusedAssetsImmediate();}
                catch(Exception cleanup){report.passed=false;exit=1;report.error=(report.error??"")+"\nCleanup: "+cleanup;Debug.LogException(cleanup);}
                try{Write();}finally{EditorApplication.Exit(exit);}
            }
        }

        static MonsterStylePageV54 CheckMonsterStyleHighestPageV54(MonsterPixelGpuCheckV53 gpu,MonsterPixelFramesV53 library,SpriteRenderer carrier,string output,Action<bool,string> need){
            int wanted=library.data.pages.Length-1;TreeBendersRasterClip selected=null;int requested=-1;
            foreach(var clip in library.data.clips){for(int i=0;i<clip.frames.Length;i++)if(clip.frames[i].page==wanted){selected=clip;requested=i;break;}if(selected!=null)break;}
            need(selected!=null,"Final atlas page has no referenced exposure: "+library.actor.kind);
            float clock=Mathf.Min(selected.duration,requested/60f);var sprite=library.At(selected.name,clock,out int frame,out int page);
            var result=new MonsterStylePageV54{motion=selected.name,frame=frame,page=page,totalPages=library.data.pages.Length,sourceClock=clock};
            need(sprite&&frame==requested&&page==wanted,"Highest-page source exposure selected incorrectly: "+library.actor.kind);
            result.pointFiltered=sprite.texture.filterMode==FilterMode.Point;result.noMipMaps=sprite.texture.mipmapCount==1;
            need(result.pointFiltered&&result.noMipMaps,"Highest page importer changed point/no-mipmap policy: "+library.actor.kind);
            var camera=gpu.Camera;var position=camera.transform.position;float size=camera.orthographicSize;
            using(var whole=new MonsterPixelViewV53(carrier,Object.DestroyImmediate))try{
                var properties=new MaterialPropertyBlock();properties.SetFloat("_MonsterInk",1);carrier.SetPropertyBlock(properties);
                camera.orthographicSize=Mathf.Max(2,sprite.bounds.extents.y*1.2f,sprite.bounds.extents.x*.6f);camera.transform.position=new Vector3(sprite.bounds.center.x,sprite.bounds.center.y,-100);
                whole.Apply(sprite,Vector3.zero,1,1,true);result.image=Path.Combine(output,library.actor.kind+"-highest-page.png");
                foreach(var pixel in gpu.CapturePixels(library.actor.kind+"-highest-page"))if(pixel.a>0)result.visiblePixels++;
                need(result.visiblePixels>0,"Highest imported page was not drawn: "+library.actor.kind);
                result.residentBeforeTrim=library.ResidentPages;library.Trim(new HashSet<int>{page},0);result.residentAfterTrim=library.ResidentPages;
                need(result.residentAfterTrim==1&&result.residentAfterTrim<=2,"Unreferenced pages were not reclaimed: "+library.actor.kind);
                need(library.At(selected.name,clock,out _,out _) == sprite,"Trim reclaimed protected highest-page sprite: "+library.actor.kind);
            }finally{camera.transform.position=position;camera.orthographicSize=size;}
            library.Trim(new HashSet<int>(),0);result.residentAfterRelease=library.ResidentPages;
            need(result.residentAfterRelease==0,"Highest page was not released: "+library.actor.kind);result.passed=true;return result;
        }
    }
}
#endif
