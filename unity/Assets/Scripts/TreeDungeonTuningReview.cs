#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Object=UnityEngine.Object;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        public static void ReviewTreeDangerRegression() {
            int exit=1;
            try {
                foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                    var probe=assembly.GetType("ElswordPixel.Editor.SimpleDangerProbeV46");if(probe==null)continue;
                    probe.GetMethod("Execute").Invoke(null,new object[]{Path.GetFullPath("Reports/tree-tuning-v47/danger-regression")});exit=0;break;
                }
            }catch(Exception e){Debug.LogException(e);}
            finally{EditorApplication.Exit(exit);}
        }
        [Serializable] sealed class TreeReviewReport {public bool passed,built;public int checks,frames;public string error;}
        public static void ReviewTreeDungeonTuning() {
            var args=Environment.GetCommandLineArgs();string output=args[Array.IndexOf(args,"--tree-review")+1];
            Directory.CreateDirectory(output);var report=new TreeReviewReport();int exit=1;
            void Need(bool ok,string why){report.checks++;if(!ok)throw new Exception(why);}
            GameObject fixture=null;
            try {
                fixture=new GameObject("Tree review");fixture.SetActive(false);var scene=fixture.AddComponent<DemoScene>();
                scene.LoadArt();scene.selectedDungeon=0;scene.ConfigureRooms();ReviewTreeGates(scene,Need);var g=scene.game;g.EnableSimpleDangerIntroductions();
                var enter=typeof(GameModel).GetMethod("EnterRoom",BindingFlags.NonPublic|BindingFlags.Instance);
                for(int retry=0;retry<2;retry++) {
                    scene.ConfigureRooms();g.EnableSimpleDangerIntroductions();enter.Invoke(g,new object[]{2});
                    var boss=g.enemies.Find(e=>e.sourceNPC=="NUI_BENDERS_NORMAL");
                    Need(boss!=null,"Benders birth missing");Need(!g.enemies.Exists(e=>e.kind=="white-giant"),"Obsolete second boss instantiated");
                    Need(Mathf.Abs(boss.x-g.player.x)>360,"Entry fixture must exercise an offscreen boss");
                    Need(scene.BossIntroductionSubject()==boss,"DANGER subject is not Benders at room entry");
                    scene.dangerShownActors.Add(boss);Need(scene.BossIntroductionSubject()==null,"DANGER repeated");
                    string savedState=boss.npcState;
                    foreach(string state in new[]{"BENDERS_NORMAL_ATTACK_A_READY","BENDERS_NORMAL_ATTACK_A","BENDERS_NORMAL_ATTACK_B_READY","BENDERS_NORMAL_ATTACK_B","BENDERS_NORMAL_SPECIAL_ATTACK_A_READY","BENDERS_NORMAL_SPECIAL_ATTACK_A"}) {
                        boss.npcState=state;boss.npcTime=.1f;Need(g.EnemyHasSuperArmor(boss),"Benders lost source super armor "+state);
                    }
                    boss.npcState=savedState;boss.npcTime=0;
                    g.ApplyHitToEnemy(boss,100000);g.player.invincible=999;
                    for(int i=0;i<360;i++)g.Step(1f/60,new Input());
                    Need(!g.enemies.Exists(e=>e.kind=="white-giant"),"Post-death boss spawned");
                    Need(scene.BossIntroductionSubject()==null,"DANGER replayed after boss death");
                    g.player.x=g.ExitX;for(int i=0;i<180&&!g.victory;i++)g.Step(1f/60,new Input{move=1});
                    Need(g.victory,"Benders kill did not allow normal exit completion");
                }
                Need(scene.Dungeon.boss=="BOSS / BENDERS","Menu boss name mismatch");
                scene.LoadDungeonPostcards();Need(scene.dungeonPostcards["tree"].boss=="Benders","Postcard boss mismatch");
                var poses=new ChickenAuthoredPoseV31.PartPose[6];
                for(int hit=0;hit<6;hit++) {
                    ChickenAuthoredPoseV31.Sample(ChickenAuthoredPoseV31.State.AttackFire,(.185f+.1f*hit)/1.8f,poses);
                    Need(poses[4+hit%2].x< -60&&poses[4+hit%2].rotation>45&&poses[4+hit%2].scaleX>1.3f,"Claw contact missing "+hit);
                    Need(Math.Abs(poses[0].rotation)<15,"Attack reverted to beak peck");
                }
                using(var gpu=new TreeGpuFixture(output))report.frames=gpu.Render();
                report.passed=true;
                int build=Array.IndexOf(args,"--tree-build");
                if(build>=0) {
                    string destination=args[build+1];Directory.CreateDirectory(destination);
                    var built=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/TreeOfEl.unity"},
                        target=BuildTarget.StandaloneWindows64,locationPathName=Path.Combine(destination,"ElswordDungeonPlayer.exe"),options=BuildOptions.CompressWithLz4|(Array.IndexOf(args,"--tree-scripts-only")>=0?BuildOptions.BuildScriptsOnly:BuildOptions.None)});
                    Need(built.summary.result==BuildResult.Succeeded,"Player build failed");report.built=true;
                }
                exit=0;
            }catch(Exception ex){report.passed=false;report.error=ex.ToString();Debug.LogException(ex);}
            finally {if(fixture)Object.DestroyImmediate(fixture);File.WriteAllText(Path.Combine(output,"validation.json"),JsonUtility.ToJson(report,true));EditorApplication.Exit(exit);}
        }
        sealed class TreeGpuFixture:IDisposable {
            readonly string output;readonly GameObject root;readonly Camera camera;readonly RenderTexture target;
            readonly Texture2D readback;readonly Material material;readonly Sprite sprite;int frames;
            internal TreeGpuFixture(string output) {
                this.output=output;root=new GameObject("Tree GPU review");
                camera=new GameObject("Tree GPU camera").AddComponent<Camera>();camera.transform.SetParent(root.transform);
                camera.enabled=false;camera.orthographic=true;camera.orthographicSize=128;camera.aspect=2;
                camera.transform.position=new Vector3(0,123,-100);camera.nearClipPlane=.1f;camera.farClipPlane=250;
                camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.10f,.14f,.20f,1);camera.cullingMask=1<<31;
                camera.allowHDR=camera.allowMSAA=false;
                target=new RenderTexture(512,256,24){antiAliasing=1,filterMode=FilterMode.Point};target.Create();camera.targetTexture=target;
                readback=new Texture2D(512,256,TextureFormat.RGBA32,false);
                material=new Material(Resources.Load<Shader>("Pixel/Shaders/ArmoredSprite"));material.SetColor("_Color",Color.white);
                sprite=Sprite.Create(Texture2D.whiteTexture,new Rect(0,0,1,1),Vector2.zero,1);
            }
            SpriteRenderer Carrier(){var go=new GameObject("Carrier"){layer=31};go.transform.SetParent(root.transform);var sr=go.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sharedMaterial=material;var properties=new MaterialPropertyBlock();properties.SetFloat("_MonsterInk",1);sr.SetPropertyBlock(properties);return sr;}
            void Capture(string name) {
                camera.Render();var old=RenderTexture.active;RenderTexture.active=target;
                readback.ReadPixels(new Rect(0,0,512,256),0,0);readback.Apply();RenderTexture.active=old;
                File.WriteAllBytes(Path.Combine(output,name+".png"),readback.EncodeToPNG());frames++;
            }
            internal int Render() {
                var carrier=Carrier();var atlas=Resources.Load<Texture2D>(ChickenA1AtlasResourceV31);
                using(var chicken=new ChickenAuthoredPartsV31(carrier,new ChickenAuthoredPartsV31.Binding(atlas,ChickenAuthoredPartsV31.InitialUnitsPerArtPixel),Object.DestroyImmediate)) {
                    chicken.Apply(new ChickenAuthoredPartsV31.Frame{state=ChickenAuthoredPoseV31.State.Idle,visible=true},Vector3.zero);
                    Capture("chicken-idle");
                    for(int i=0;i<18;i++){
                        float time=i*.05f;
                        chicken.Apply(new ChickenAuthoredPartsV31.Frame{state=ChickenAuthoredPoseV31.State.AttackFire,normalizedPhase=time/1.8f,visible=true},Vector3.zero);
                        Capture("chicken-"+i.ToString("D2"));
                    }
                }
                Object.DestroyImmediate(carrier.gameObject);
                foreach(string npc in new[]{"NUI_THIEF_THIN_ZERO","NUI_BENDERS_NORMAL"}) {
                    if(npc=="NUI_BENDERS_NORMAL"){RenderWholeBenders();continue;}
                    carrier=Carrier();var rig=BendersFamilyRigV36.Load(npc,Object.DestroyImmediate);
                    using(var parts=new FairyAuthoredPartsV33(carrier,rig.lib,Object.DestroyImmediate)) {
                        var members=parts.Members;var axes=rig.CreateMemberAxes(members);var depths=new float[members.Length];
                        TreeHumanoidPose anatomy=null;float elapsed=0;
                        foreach(string clip in new[]{"Wait",string.IsNullOrEmpty(rig.gaitMotion)?"Dash":rig.gaitMotion,"AttackA_Ready","AttackA_Fire","DamageSmall","DamageDownFront"}) {
                            var track=rig.lib.Find(clip);if(track==null)continue;
                            for(int i=0;i<8;i++) {
                                var f=new FairyAuthoredPartsV33.Frame{alias=clip,sourceClock=track.duration*i/8,facing=1,visible=true};
                                var standing=new FairyAuthoredPartsV33.Frame{alias="Wait",sourceClock=0,facing=1,visible=true};
                                parts.Apply(standing,Vector3.zero);rig.FinishMemberAxes(members,axes);rig.Depth(standing,members,depths,axes);
                                if(anatomy==null)anatomy=new TreeHumanoidPose(rig,members,axes);
                                anatomy.Apply(f,elapsed);elapsed+=.15f;Capture(npc+"-"+clip+"-"+i);
                                if(npc=="NUI_BENDERS_NORMAL"&&clip=="Wait"&&i==0) {
                                    camera.backgroundColor=Color.clear;Capture("benders-cover");camera.backgroundColor=new Color(.10f,.14f,.20f,1);
                                }
                            }
                        }
                        ReviewContinuous(rig,parts,members,axes,depths,carrier);
                    }
                    rig.lib.ReleaseOwned(Object.DestroyImmediate);Object.DestroyImmediate(carrier.gameObject);
                }
                return frames;
            }
            void RenderWholeBenders() {
                var carrier=Carrier();var data=TreeBendersFrames.Data;
                foreach(var clip in data.clips)if(clip.fps!=60||clip.frames.Length<Mathf.CeilToInt(clip.duration*60))throw new Exception("Sparse whole-body clip "+clip.name);
                using(var whole=new TreeBendersFrames(carrier,Object.DestroyImmediate)) {
                    foreach(string name in new[]{"Wait","Dash","AttackA_Ready","AttackA_Fire","DamageSmall","DamageDownFront"}) {
                        var clip=Array.Find(data.clips,c=>c.name==name);if(clip==null)throw new Exception("Missing original whole-body action "+name);
                        for(int i=0;i<8;i++) {
                            whole.Apply(new FairyAuthoredPartsV33.Frame{alias=name,sourceClock=clip.duration*i/8,facing=1,visible=true},Vector3.zero);
                            Capture("NUI_BENDERS_NORMAL-"+name+"-"+i);
                            if(name=="Wait"&&i==0){camera.backgroundColor=Color.clear;Capture("benders-cover");camera.backgroundColor=new Color(.10f,.14f,.20f,1);}
                        }
                    }
                    int count=0,redNormal=0,redArmor=0;var property=new MaterialPropertyBlock();
                    foreach(string name in new[]{"Wait","Dash","AttackA_Ready","AttackA_Fire","Wait","DamageDownFront","DamageStandUpFront"}) {
                        var clip=Array.Find(data.clips,c=>c.name==name);
                        for(int i=0;i<clip.frames.Length;i++) {
                            bool armor=name.Contains("Ready")||name.Contains("Fire");property.SetFloat("_Outline",armor?1:0);property.SetColor("_OutlineColor",new Color(1,.14f,.26f,1));carrier.SetPropertyBlock(property);
                            whole.Apply(new FairyAuthoredPartsV33.Frame{alias=name,sourceClock=Mathf.Min(clip.duration,i/60f),facing=1,visible=true},Vector3.zero);
                            if(!whole.Renderer.enabled||!whole.Renderer.sprite)throw new Exception("Missing whole-frame presentation "+name);
                            if(i%3==0){Capture("NUI_BENDERS_NORMAL-continuous-"+count.ToString("D4"));int red=0;foreach(var p in readback.GetPixels32())if(p.r>210&&p.g<80&&p.b<100)red++;if(armor)redArmor=Math.Max(redArmor,red);else redNormal=Math.Max(redNormal,red);}
                            count++;
                        }
                    }
                    if(redArmor<redNormal+35)throw new Exception("Missing whole-frame super armor contour");
                    File.WriteAllText(Path.Combine(output,"NUI_BENDERS_NORMAL-continuity.json"),"{\"frames\":"+count+",\"fps\":60,\"wholeBodyRaster\":true,\"runtimeBones\":0,\"sourceClips\":"+data.clips.Length+",\"normalRedPixels\":"+redNormal+",\"armorRedPixels\":"+redArmor+"}");
                }
                Object.DestroyImmediate(carrier.gameObject);
            }
            void ReviewContinuous(BendersFamilyRigV36 rig,FairyAuthoredPartsV33 parts,SpriteRenderer[] members,Transform[] axes,float[] depths,SpriteRenderer carrier) {
                TreeHumanoidPose pose=null;float clock=0;int index=0;
                var standing=new FairyAuthoredPartsV33.Frame{alias="Wait",sourceClock=0,facing=1,visible=true};
                var baseline=new float[7];var pairs=new[]{0,1,7,8,11,12,3};var scales=new Vector3[members.Length];
                var property=new MaterialPropertyBlock();property.SetFloat("_MonsterInk",1);int redNormal=0,redArmor=0;
                foreach(string clip in new[]{"Wait",string.IsNullOrEmpty(rig.gaitMotion)?"Dash":rig.gaitMotion,"AttackA_Ready","AttackA_Fire","Wait","DamageDownFront","DamageStandUpFront"}) {
                    var track=rig.lib.Find(clip);if(track==null)continue;
                    int ticks=Mathf.CeilToInt(track.duration*60);
                    for(int tick=0;tick<=ticks;tick++) {
                        bool armored=clip.Contains("Ready")||clip.Contains("Fire");
                        property.SetFloat("_Outline",armored?1:0);property.SetColor("_OutlineColor",new Color(1,.14f,.26f,1));carrier.SetPropertyBlock(property);
                        parts.Apply(standing,Vector3.zero);rig.FinishMemberAxes(members,axes);rig.Depth(standing,members,depths,axes);
                        if(pose==null)pose=new TreeHumanoidPose(rig,members,axes);
                        pose.Apply(new FairyAuthoredPartsV33.Frame{alias=clip,sourceClock=Mathf.Min(track.duration,tick/60f),facing=1,visible=true},clock);
                        for(int i=0;i<pairs.Length;i++) {
                            int a=pairs[i];var first=axes[a]?axes[a]:members[a].transform;var second=axes[a+1]?axes[a+1]:members[a+1].transform;
                            float length=Vector3.Distance(first.localPosition,second.localPosition);
                            if(index==0)baseline[i]=length;
                            else if(Mathf.Abs(length-baseline[i])>.002f)throw new Exception(rig.npc+" limb changed length "+a+" "+clip);
                        }
                        for(int i=0;i<members.Length;i++){
                            var transform=axes[i]?axes[i]:members[i].transform;
                            if(index==0)scales[i]=transform.localScale;
                            else if((scales[i]-transform.localScale).sqrMagnitude>.000001f)throw new Exception("Animated limb scaling "+rig.npc+" "+i);
                        }
                        if(tick%3==0){
                            Capture(rig.npc+"-continuous-"+index.ToString("D4"));
                            int red=0;foreach(var pixel in readback.GetPixels32())if(pixel.r>210&&pixel.g<80&&pixel.b<100)red++;
                            if(clip=="Wait")redNormal=Math.Max(redNormal,red);
                            if(armored)redArmor=Math.Max(redArmor,red);
                        }
                        clock+=1f/60;index++;
                    }
                }
                if(redArmor<redNormal+35)throw new Exception("Super armor outline is not visible: "+rig.npc+" "+redNormal+" / "+redArmor);
                File.WriteAllText(Path.Combine(output,rig.npc+"-continuity.json"),"{\"frames\":"+index+",\"fixedLengthChains\":7,\"constantScales\":true,\"normalRedPixels\":"+redNormal+",\"armorRedPixels\":"+redArmor+"}");
                carrier.SetPropertyBlock(null);
            }
            public void Dispose(){camera.targetTexture=null;Object.DestroyImmediate(root);Object.DestroyImmediate(target);Object.DestroyImmediate(readback);Object.DestroyImmediate(material);Object.DestroyImmediate(sprite);}
        }
    }
}
#endif
