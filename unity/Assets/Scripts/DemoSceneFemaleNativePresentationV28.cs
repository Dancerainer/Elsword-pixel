using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Unity.Collections;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class DemoScene {
  const string FemaleNativePrefixV28="Pixel/feedback-v25/started-native-context-v28/chicken-female/";
  const string FemaleNativeManifestV28=FemaleNativePrefixV28+"native-display";
  const string FemaleNativeRawV28=FemaleNativePrefixV28+"waitstart-geometry";
  const string FemaleNativeUvV28=FemaleNativePrefixV28+"expanded-uv";
  const string FemaleNativeShaderV28="Pixel/Shaders/FemaleNativeReferenceV28";
  const string FemaleNativeShaderNameV28="ElswordPixel/FemaleNativeReferenceV28";
  const string FemaleNativeManifestHashV28="cf5a5bf4a689b5969c66bc97cc86b430bbf29aebfe1b3bd85d02725fa43faa38";
  const string FemaleNativeRawHashV28="33ca81f6662a3b6276226ab30b7876e043b9c87c541b9a98b23bf58a26803201";
  const string FemaleNativeUvHashV28="68c2209a77a9bd30a105cf8aaa9b18276e0d1f2b25db909fa6d8c605ece8d002";
  const string FemaleNativeShaderSourceHashV28="6156ed14f45d8f6e4a813be92f8ee2522952f43bbdbcbe28997cbd16a7a2a4a9";
  const string FemaleNativeDurationBitsV28="abaaaaaaaaaa2640";
  const string FemaleNativeUnitBitsV28="ec51b81e85ebd13f";
  const int FemaleNativeVerticesV28=7320,FemaleNativeFrameBytesV28=87840,FemaleNativeFramesV28=273;
  const double FemaleNativeUnitV28=.28;
  static readonly string[] FemaleNativePlaceBitsV28={"85eb51b89ee4a140","0000000000d08540","f0a7c64b378d6240"};
  static readonly string[] FemaleNativeTexturePathsV28={FemaleNativePrefixV28+"uv-640db8c99c30",FemaleNativePrefixV28+"uv-33f03041a163"};
  static readonly string[] FemaleNativePartNamesV28={"BODY/Sphere02","BODY/Object20","BODY/Object27","BODY/Object56","BODY/Object61","BODY/Sphere03","BODY/Object65","BODY/Object68","BODY/KSM_UnnamedFrame_30","BODY/KSM_UnnamedFrame_31","BODY/KSM_UnnamedFrame_40","BODY/KSM_UnnamedFrame_41","BODY/KSM_UnnamedFrame_42","BODY/KSM_UnnamedFrame_47","BODY/KSM_UnnamedFrame_48","BODY/KSM_UnnamedFrame_55","BODY/KSM_UnnamedFrame_56","BODY/KSM_UnnamedFrame_64","BODY/KSM_UnnamedFrame_65","BODY/KSM_UnnamedFrame_66","BODY/KSM_UnnamedFrame_81","BODY/KSM_UnnamedFrame_82"};
  static readonly int[] FemaleNativePartStartsV28={0,2376,2532,2754,2910,3132,5508,5730,5952,6078,6204,6228,6408,6432,6612,6636,6816,6840,7020,7044,7068,7194},FemaleNativePartCountsV28={2376,156,222,156,222,2376,222,222,126,126,24,180,24,180,24,180,24,180,24,24,126,126};
  static readonly int[] FemaleNativePartTexturesV28={0,0,1,0,1,0,1,1,0,0,1,1,1,1,1,1,1,1,1,1,0,0},FemaleNativePartRootsV28={12,12,13,17,16,17,15,14,17,17,16,16,16,15,15,14,14,13,13,13,12,12};

  [Serializable] sealed class FemaleNativeFrameV28 {
   public string id="",timeLE64="",rawF32Sha256="";public int index=0,offset=0;
  }
  [Serializable] sealed class FemaleNativeTextureV28 {
   public string resource="",pngSha256="",rgbaSha256="";public int width=0,height=0;public bool allAlpha255=false;
  }
  [Serializable] sealed class FemaleNativePartV28 {
   public string name="";public int vertexStart=0,count=0,uvOffset=0,textureIndex=0,componentRootIndex=0;
   public int[] colorRgba255=new int[0];
  }
  [Serializable] sealed class FemaleNativeDataV28 {
   public string schema="",rawResource="",uvResource="",rawSha256="",uvSha256="",shaderResource="",shaderName="",shaderSourceSha256="",durationLE64="",unitLE64="",nativeSceneResource="";
   public int frameCount=0,vertexCount=0,rawByteCount=0,uvByteCount=0,sourceHz=0;
   public string[] sourcePlaceLE64=new string[0];
   public FemaleNativeFrameV28[] frames=new FemaleNativeFrameV28[0];
   public FemaleNativePartV28[] parts=new FemaleNativePartV28[0];
   public FemaleNativeTextureV28[] textures=new FemaleNativeTextureV28[0];
  }
  sealed class FemaleNativeDisplayV28 {
   internal GameModel model;internal RoomDefinition room;internal Enemy enemy;internal SpriteRenderer legacy;
   internal Transform nativeRoot;internal GameObject group;internal Mesh[] meshes;internal Material[] materials;
   internal FlockSkinV37 flockShapeV37;internal Texture2D[] flockMaterialsV37;internal Shader flockShaderV37;internal Vector3[][] positions;internal TextAsset rawOwner;internal NativeArray<Vector3> raw;internal FemaleNativeDataV28 data;
   internal bool originalLegacyEnabled,ownsHiddenSprite;internal int frame=-1;internal double duration;
   internal FemaleRoot17A1DrawnPartsV29 a1Root17;internal FemaleRoot17A1DrawnPartsV29.Binding a1Root17BlockedBinding;
  }
  FemaleNativeDisplayV28 femaleNativeDisplayV28;
  RoomDefinition femaleNativeBlockedRoomV28;
  Enemy femaleNativeBlockedEnemyV28;

  static void FemaleNativeNeedV28(bool valid,string message){if(!valid)throw new InvalidOperationException("Female native display: "+message);}
  static bool FemaleNativeFiniteV28(float value){return !float.IsNaN(value)&&!float.IsInfinity(value);}
  static string FemaleNativeHashV28(byte[] bytes){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(bytes)).Replace("-","").ToLowerInvariant();}
  static string FemaleNativeAssetHashV28(TextAsset asset){
   var bytes=asset.GetData<byte>();
   using(var hash=SHA256.Create()){
    var chunk=new byte[Math.Min(65536,bytes.Length)];
    for(int at=0;at<bytes.Length;at+=chunk.Length){
     int count=Math.Min(chunk.Length,bytes.Length-at);NativeArray<byte>.Copy(bytes,at,chunk,0,count);
     hash.TransformBlock(chunk,0,count,chunk,0);
    }
    hash.TransformFinalBlock(Array.Empty<byte>(),0,0);
    return BitConverter.ToString(hash.Hash).Replace("-","").ToLowerInvariant();
   }
  }
  static double FemaleNativeDoubleV28(string hex){
   FemaleNativeNeedV28(BitConverter.IsLittleEndian&&hex!=null&&hex.Length==16,"Exact LE64 input required");
   var bytes=new byte[8];for(int i=0;i<8;i++)bytes[i]=Convert.ToByte(hex.Substring(i*2,2),16);
   double value=BitConverter.ToDouble(bytes,0);FemaleNativeNeedV28(!double.IsNaN(value)&&!double.IsInfinity(value),"Nonfinite source double");return value;
  }
  static string FemaleNativeTextureHashV28(Texture2D texture){
   int stride=texture.width,height=texture.height;
   var rgba=texture.format==TextureFormat.RGBA32?texture.GetPixelData<Color32>(0):default;
   var converted=rgba.IsCreated?null:texture.GetPixels32();
   FemaleNativeNeedV28((converted==null?rgba.Length:converted.Length)==checked(stride*height),"Exact texture pixel layout");
   var bytes=new byte[checked(stride*height*4)];int at=0;
   for(int y=height-1;y>=0;y--)for(int x=0;x<stride;x++){
    int pos=y*stride+x;var p=converted==null?rgba[pos]:converted[pos];FemaleNativeNeedV28(p.a==255,"Original opaque UV alpha changed");
    bytes[at++]=p.r;bytes[at++]=p.g;bytes[at++]=p.b;bytes[at++]=p.a;
   }
   return FemaleNativeHashV28(bytes);
  }
  RoomDefinition FemaleNativeCurrentRoomV28(){return game!=null&&game.room>=0&&game.room<game.RoomCount?game.CurrentRoom:null;}

  bool FemaleNativeScopeV28(out Enemy enemy,out SpriteRenderer legacy,out RoomDefinition room){
   enemy=null;legacy=null;room=FemaleNativeCurrentRoomV28();
   if(game==null||room==null||catalog==null||catalog.dungeons==null||catalog.dungeons.Length==0||catalog.stages==null||
      selectedDungeon!=0||game.room!=1||room.id!="tree-2"||game.HasEncounters||CurrentNpcContentSet!=null||!adventureStarted)return false;
   var route=catalog.dungeons[0];if(route==null)return false;int stageIndex=route.start+1;
   if(route.id!="tree"||stageIndex<0||stageIndex>=catalog.stages.Length)return false;
   var stage=catalog.stages[stageIndex];
   if(stage==null||stage.id!="tree-2"||stage.nativeScene!="Pixel/native-world/tree-2/scene"||stage.spawns==null||stage.spawns.Length==0||
      game.enemies==null||game.enemies.Count==0||enemyViews==null||enemyViews.Count==0)return false;
   var source=stage.spawns[0];enemy=game.enemies[0];legacy=enemyViews[0];
   if(source==null||enemy==null||!legacy||!legacy.sprite||!legacy.gameObject.activeInHierarchy||
      source.kind!="chicken-female"||source.sourceNPC!="NUI_CHICKEN_FEMALE"||source.sourceSpawnIndex!=3||
      !source.passive||source.aiEnabled||source.dormant||source.visualScale!=1||source.facing!=1||source.groundLine!=5||
      !FemaleNativeFiniteV28(source.x)||Math.Abs((double)source.x-330.37250501393805)>.002||source.y!=54||
      enemy.kind!=source.kind||enemy.sourceNPC!=source.sourceNPC||!enemy.active||enemy.dormant||!enemy.passive||enemy.aiEnabled||
      !FemaleNativeFiniteV28(enemy.hp)||enemy.hp<=0||enemy.visualScale!=1||enemy.facing!=1||enemy.spawnX!=source.x||
      !FemaleNativeFiniteV28(enemy.x)||Math.Abs((double)enemy.x-source.x)>.002||enemy.y!=54||enemy.groundLine!=5||enemy.encounterIndex!=source.encounterIndex)return false;
   int matching=0;foreach(var e in game.enemies)if(e!=null&&e.sourceNPC=="NUI_CHICKEN_FEMALE")matching++;
   if(matching!=1)return false;
   if(catalog.npcRules!=null)foreach(var profile in catalog.npcRules)if(profile!=null&&profile.id=="NUI_CHICKEN_FEMALE")return false;
   if(!NativeWorldActive||calibrationRun||cameraFramingReviewOriginalV25||CameraDirectorActiveV25||!worldRoot||!worldCamera||!worldCamera.orthographic||
      worldCamera.clearFlags!=CameraClearFlags.SolidColor||!nativeCamera||nativeScene.unit!=.28f||
      !nativeCamera.orthographic||nativeCamera.enabled||nativeCamera.clearFlags!=CameraClearFlags.SolidColor||
      nativeCamera.cullingMask!=(1<<28)||(worldCamera.cullingMask&(1<<28))!=0||!nativeRoot||
      nativeRoot.position!=Vector3.zero||nativeRoot.rotation!=Quaternion.identity||nativeRoot.lossyScale!=Vector3.one||
      nativeRoot.IsChildOf(worldRoot)||!nativeTarget||!nativeTarget.IsCreated()||nativeCamera.targetTexture!=nativeTarget||
      nativeTarget.width!=CameraNativePixelWidthV25||nativeTarget.height!=CameraNativePixelHeightV25||nativeTarget.depth<24||nativeTarget.format!=RenderTextureFormat.ARGB32||
      nativeTarget.filterMode!=FilterMode.Point||nativeTarget.antiAliasing!=1)return false;
   return true;
  }

  static FemaleNativeDataV28 FemaleNativeLoadDataV28(bool verifyContent,bool forBuild,out TextAsset raw,out TextAsset uv,out Texture2D[] textures,out Shader shader){
   var manifest=Resources.Load<TextAsset>(FemaleNativeManifestV28);FemaleNativeNeedV28(manifest,"Missing exact display manifest");
   byte[] json=manifest.bytes;FemaleNativeNeedV28(json.Length<=262144&&FemaleNativeHashV28(json)==FemaleNativeManifestHashV28,"Manifest readback hash/size");
   var d=JsonUtility.FromJson<FemaleNativeDataV28>(new UTF8Encoding(false,true).GetString(json));
   FemaleNativeNeedV28(d!=null&&d.schema=="female-native-whole273-display-manifest-v28-r1"&&d.frameCount==FemaleNativeFramesV28&&d.vertexCount==7320&&d.sourceHz==24&&
    d.rawByteCount==23980320&&d.uvByteCount==58560&&d.rawResource==FemaleNativeRawV28&&d.uvResource==FemaleNativeUvV28&&
    d.rawSha256==FemaleNativeRawHashV28&&d.uvSha256==FemaleNativeUvHashV28&&d.durationLE64==FemaleNativeDurationBitsV28&&d.unitLE64==FemaleNativeUnitBitsV28&&
    d.shaderResource==FemaleNativeShaderV28&&d.shaderName==FemaleNativeShaderNameV28&&d.shaderSourceSha256==FemaleNativeShaderSourceHashV28&&
    d.nativeSceneResource=="Pixel/native-world/tree-2/scene","Manifest fixed source contract");
   FemaleNativeNeedV28(d.sourcePlaceLE64!=null&&d.sourcePlaceLE64.Length==3&&d.frames!=null&&d.frames.Length==FemaleNativeFramesV28&&
    d.parts!=null&&d.parts.Length==22&&d.textures!=null&&d.textures.Length==2,"Complete manifest inventory");
   for(int i=0;i<3;i++)FemaleNativeNeedV28(d.sourcePlaceLE64[i]==FemaleNativePlaceBitsV28[i],"Original sourcePlace LE64");
   FemaleNativeNeedV28(FemaleNativeDoubleV28(d.unitLE64)==FemaleNativeUnitV28,"Original source unit once");
   raw=Resources.Load<TextAsset>(d.rawResource);uv=Resources.Load<TextAsset>(d.uvResource);
   FemaleNativeNeedV28(raw&&uv&&BitConverter.IsLittleEndian,"Missing raw/UV TextAsset or unsupported source endian");
   int rawBytes=raw.GetData<byte>().Length,uvBytes=uv.GetData<byte>().Length;
   FemaleNativeNeedV28(rawBytes==d.rawByteCount&&uvBytes==d.uvByteCount,"Raw/UV TextAsset lengths");
   if(verifyContent)FemaleNativeNeedV28(FemaleNativeAssetHashV28(raw)==d.rawSha256&&FemaleNativeAssetHashV28(uv)==d.uvSha256,"Raw/UV TextAsset readback hashes");
   double prior=-1,duration=FemaleNativeDoubleV28(d.durationLE64);
   for(int i=0;i<FemaleNativeFramesV28;i++){
    var f=d.frames[i];FemaleNativeNeedV28(f!=null&&f.index==i&&f.id=="female-WaitStart-"+i.ToString("D4")&&
     f.offset>=0&&(long)f.offset==(long)i*FemaleNativeFrameBytesV28&&(long)f.offset+FemaleNativeFrameBytesV28<=rawBytes,"Frame/offset inventory or bounds");
    double t=FemaleNativeDoubleV28(f.timeLE64);FemaleNativeNeedV28(t>=0&&t<=duration&&t>prior&&(i!=0||t==0)&&(i!=272||t==duration),"Exact source time order/endpoints");prior=t;
    FemaleNativeNeedV28(f.rawF32Sha256!=null&&f.rawF32Sha256.Length==64,"Pinned source frame hash metadata");
   }
   int cursor=0;var roots=new HashSet<int>();
   for(int i=0;i<22;i++){
    var p=d.parts[i];FemaleNativeNeedV28(p!=null&&p.name==FemaleNativePartNamesV28[i]&&p.vertexStart==cursor&&p.vertexStart==FemaleNativePartStartsV28[i]&&
     p.count==FemaleNativePartCountsV28[i]&&p.count>0&&p.count%3==0&&(long)p.vertexStart+p.count<=FemaleNativeVerticesV28&&
     p.uvOffset==(long)p.vertexStart*8&&(long)p.uvOffset+(long)p.count*8<=uvBytes&&p.textureIndex==FemaleNativePartTexturesV28[i]&&
     p.textureIndex>=0&&p.textureIndex<2&&p.componentRootIndex==FemaleNativePartRootsV28[i]&&
     p.colorRgba255!=null&&p.colorRgba255.Length==4,"Original part/UV/material ownership");
    foreach(int color in p.colorRgba255)FemaleNativeNeedV28(color==255,"Original neutral diffuse tint");
    roots.Add(p.componentRootIndex);cursor+=p.count;
   }
   FemaleNativeNeedV28(cursor==7320&&roots.SetEquals(new[]{12,13,14,15,16,17}),"All22 parts/six component roots");
   textures=new Texture2D[2];
   for(int i=0;i<2;i++){
    var p=d.textures[i];FemaleNativeNeedV28(p!=null&&p.resource==FemaleNativeTexturePathsV28[i]&&p.allAlpha255&&p.pngSha256.Length==64,"Exact two existing candidate UV identities");
    var t=Resources.Load<Texture2D>(p.resource);FemaleNativeNeedV28(t&&t.width==p.width&&t.height==p.height&&(t.format==TextureFormat.RGBA32||t.format==TextureFormat.RGB24)&&t.isReadable&&
     t.filterMode==FilterMode.Point&&t.wrapMode==TextureWrapMode.Repeat&&t.anisoLevel==0&&t.mipmapCount==1,"Original point/fullRGBA/readable/noMip UV");
    if(verifyContent)FemaleNativeNeedV28(FemaleNativeTextureHashV28(t)==p.rgbaSha256,"Existing candidate fullRGBA readback");textures[i]=t;
   }
   shader=Resources.Load<Shader>(d.shaderResource);FemaleNativeNeedV28(shader&&shader.name==d.shaderName&&(forBuild||shader.isSupported),"Exact Reference shader entry");
   // Shader source bytes are verified by the source/built-artifact gates; Shader
   // does not expose original source text here. Do not pretend to hash it.
   return d;
  }

#if UNITY_EDITOR
  public static void VerifyFemaleNativeAndroidBuildAssets(){
   var data=FemaleNativeLoadDataV28(true,true,out _,out _,out _,out _);
   string root=System.IO.Path.Combine(Application.dataPath,"Resources");
   foreach(var texture in data.textures){
    var source=System.IO.File.ReadAllBytes(System.IO.Path.Combine(root,texture.resource+".png"));
    FemaleNativeNeedV28(FemaleNativeHashV28(source)==texture.pngSha256,"Original texture source bytes: "+texture.resource);
   }
   var shader=System.IO.File.ReadAllBytes(System.IO.Path.Combine(root,data.shaderResource+".shader"));
   FemaleNativeNeedV28(FemaleNativeHashV28(shader)==data.shaderSourceSha256,"Original shader source bytes");
  }
#endif

  // The Android build hook checks the exact raw, UV and texture content before
  // sealing the Resources payload. Other players and Editor play verify at use.
#if UNITY_ANDROID && !UNITY_EDITOR
  const bool VerifyFemaleNativeAtUseV28=false;
#else
  const bool VerifyFemaleNativeAtUseV28=true;
#endif

  FemaleNativeDisplayV28 FemaleNativeBuildV28(Enemy enemy,SpriteRenderer legacy,RoomDefinition room){
   var s=new FemaleNativeDisplayV28{model=game,room=room,enemy=enemy,legacy=legacy,nativeRoot=nativeRoot,originalLegacyEnabled=legacy.enabled,
    meshes=new Mesh[22],materials=new Material[22],positions=new Vector3[22][]};
   try{
    s.data=FemaleNativeLoadDataV28(VerifyFemaleNativeAtUseV28,false,out s.rawOwner,out var uvOwner,out var textures,out var shader);
    s.raw=s.rawOwner.GetData<Vector3>();var uv=uvOwner.GetData<Vector2>();
    FemaleNativeNeedV28(s.raw.Length==s.data.rawByteCount/12&&uv.Length==s.data.uvByteCount/8,"Source float tuple layout");
    s.duration=FemaleNativeDoubleV28(s.data.durationLE64);LoadFemaleFlockV37(s);
    s.group=new GameObject("Female original whole22 native display V28");s.group.SetActive(false);s.group.layer=28;s.group.transform.SetParent(nativeRoot,false);
    s.group.transform.localPosition=new Vector3((float)FemaleNativeDoubleV28(s.data.sourcePlaceLE64[0]),(float)FemaleNativeDoubleV28(s.data.sourcePlaceLE64[1]),(float)FemaleNativeDoubleV28(s.data.sourcePlaceLE64[2]));
    s.group.transform.localRotation=Quaternion.identity;s.group.transform.localScale=Vector3.one;
    for(int i=0;i<22;i++){
     var p=s.data.parts[i];var vertices=new Vector3[p.count];s.positions[i]=vertices;var coords=new Vector2[p.count];var indices=new int[p.count];
     for(int j=0;j<p.count;j++){var point=uv[p.uvOffset/8+j];FemaleNativeNeedV28(FemaleNativeFiniteV28(point.x)&&FemaleNativeFiniteV28(point.y),"Nonfinite original UV");coords[j]=point;indices[j]=j;}
     var mesh=new Mesh{name="Female raw "+p.name,indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};s.meshes[i]=mesh;mesh.vertices=vertices;mesh.uv=coords;mesh.triangles=indices;
     var go=new GameObject(p.name);go.layer=28;go.transform.SetParent(s.group.transform,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;
     bool flockPart=s.flockShapeV37!=null&&p.componentRootIndex!=17;var mat=new Material(flockPart?s.flockShaderV37:shader);s.materials[i]=mat;mat.SetColor("_Color",Color.white);mat.SetFloat("_Cull",0);mat.SetFloat("_FogNear",1000000000);mat.SetFloat("_FogFar",1000000128);
     mat.SetFloat("_FogHeightStart",100000);mat.SetFloat("_FogHeightEnd",-100000);mat.SetFloat("_FogBias",0);mat.mainTexture=flockPart?s.flockMaterialsV37[p.textureIndex]:textures[p.textureIndex];if(flockPart)mat.SetColor("_Ink",p.textureIndex==0?new Color(.26f,.18f,.21f):new Color(.32f,.18f,.09f));go.AddComponent<MeshRenderer>().sharedMaterial=mat;
    }
    GC.KeepAlive(uvOwner);FemaleNativeNeedV28(s.group.transform.childCount==22,"Complete mesh group");return s;
   }catch{FemaleNativeReleaseV28(s,false);throw;}
  }
  void FemaleNativePoseV28(FemaleNativeDisplayV28 s,float clock){
   FemaleNativeNeedV28(FemaleNativeFiniteV28(clock)&&clock>=0,"Finite existing model clock required");
   double t=Math.Min((double)clock,s.duration);int frame=t>=s.duration?272:Math.Min(272,(int)Math.Floor(t*24d));
   if(frame==s.frame)return;var f=s.data.frames[frame];
   for(int i=0;i<22;i++){
    var p=s.data.parts[i];var positions=s.positions[i];
    for(int j=0;j<p.count;j++){
     var point=s.raw[f.offset/12+p.vertexStart+j];float x=(float)(point.x/FemaleNativeUnitV28),y=(float)(point.y/FemaleNativeUnitV28),z=(float)(point.z/FemaleNativeUnitV28);
     FemaleNativeNeedV28(FemaleNativeFiniteV28(x)&&FemaleNativeFiniteV28(y)&&FemaleNativeFiniteV28(z),"Nonfinite original native position");positions[j]=new Vector3(x,y,z);
    }
   }
   ShapeFemaleFlockHeadsV37(s);
   for(int i=0;i<22;i++){FemaleNativeNeedV28(s.meshes[i],"Owned mesh disappeared");s.meshes[i].vertices=s.positions[i];s.meshes[i].RecalculateBounds();}
   UpdateFlockInkOriginsV37(s);s.frame=frame;
  }
  void FemaleNativeReleaseV28(FemaleNativeDisplayV28 s,bool restore){
   if(s==null)return;if(s.a1Root17!=null){s.a1Root17.Dispose();s.a1Root17=null;}s.a1Root17BlockedBinding=null;if(s.group)s.group.SetActive(false);
   if(restore&&s.ownsHiddenSprite&&s.legacy&&ReferenceEquals(s.model,game)&&ReferenceEquals(s.room,FemaleNativeCurrentRoomV28())&&
      game.enemies!=null&&game.enemies.Count>0&&ReferenceEquals(game.enemies[0],s.enemy)&&enemyViews!=null&&enemyViews.Count>0&&ReferenceEquals(enemyViews[0],s.legacy))s.legacy.enabled=s.originalLegacyEnabled;
   if(s.group)RemoveSceneObject(s.group);
   if(s.meshes!=null)foreach(var mesh in s.meshes)if(mesh)DestroyNativeWorldObject(mesh);
   if(s.materials!=null)foreach(var material in s.materials)if(material)DestroyNativeWorldObject(material);
   // The array is borrowed from TextAsset. Releasing its owner never disposes Unity's buffer.
   s.ownsHiddenSprite=false;s.raw=default;s.rawOwner=null;s.data=null;s.positions=null;s.meshes=null;s.materials=null;
   s.legacy=null;s.enemy=null;s.room=null;s.model=null;s.nativeRoot=null;s.group=null;
   // Resources-owned Texture2D/TextAsset assets are never Destroyed by this owner.
  }
  void DisposeFemaleNativeDisplayV28(){
   var s=femaleNativeDisplayV28;femaleNativeDisplayV28=null;FemaleNativeReleaseV28(s,true);
   var room=FemaleNativeCurrentRoomV28();var enemy=game!=null&&game.enemies!=null&&game.enemies.Count>0?game.enemies[0]:null;
   if(!ReferenceEquals(femaleNativeBlockedRoomV28,room)||!ReferenceEquals(femaleNativeBlockedEnemyV28,enemy)){femaleNativeBlockedRoomV28=null;femaleNativeBlockedEnemyV28=null;}
  }
  void UpdateFemaleNativeDisplayV28(){
   var currentRoom=FemaleNativeCurrentRoomV28();var currentEnemy=game!=null&&game.enemies!=null&&game.enemies.Count>0?game.enemies[0]:null;
   if(!ReferenceEquals(femaleNativeBlockedRoomV28,currentRoom)||!ReferenceEquals(femaleNativeBlockedEnemyV28,currentEnemy)){femaleNativeBlockedRoomV28=null;femaleNativeBlockedEnemyV28=null;}
   if(currentRoom!=null&&currentEnemy!=null&&ReferenceEquals(femaleNativeBlockedRoomV28,currentRoom)&&ReferenceEquals(femaleNativeBlockedEnemyV28,currentEnemy)){DisposeFemalePixelV53();DisposeFemaleNativeDisplayV28();return;}
   Enemy enemy=currentEnemy;SpriteRenderer legacy=null;RoomDefinition room=currentRoom;
   try{
    if(!FemaleNativeScopeV28(out enemy,out legacy,out room)){DisposeFemalePixelV53();DisposeFemaleNativeDisplayV28();return;}
    if(TrySelectFemalePixelV53(enemy,legacy,room))return;
    // The six native roots occupy different depths. A single sprite projected
    // at the actor foot cannot reproduce their native camera placement. Keep
    // this owner until a root-aware pixel assembly passes the GPU comparison.
    ObserveMonsterPixelV53(enemy,"Female/NativeSixRoots","WaitStart","WaitStart",(float)Math.Min(enemy.clock,FemaleNativeDoubleV28(FemaleNativeDurationBitsV28)),"pending","six-root-native-projection-adapter-required");
    var s=femaleNativeDisplayV28;
    if(s!=null&&(!ReferenceEquals(s.model,game)||!ReferenceEquals(s.room,room)||!ReferenceEquals(s.enemy,enemy)||!ReferenceEquals(s.legacy,legacy)||!ReferenceEquals(s.nativeRoot,nativeRoot)||!s.group)){
     DisposeFemaleNativeDisplayV28();s=null;
    }
    if(!legacy.enabled&&(s==null||!s.ownsHiddenSprite)){DisposeFemaleNativeDisplayV28();return;}
    if(s==null){s=FemaleNativeBuildV28(enemy,legacy,room);femaleNativeDisplayV28=s;}
    FemaleNativePoseV28(s,enemy.clock);
    FemaleNativeApplyRoot17A1V29(s);
    FemaleNativeNeedV28(ReferenceEquals(s.model,game)&&ReferenceEquals(s.room,FemaleNativeCurrentRoomV28())&&ReferenceEquals(s.nativeRoot,nativeRoot),"Room changed during resource setup");
    s.group.SetActive(true);legacy.enabled=false;s.ownsHiddenSprite=true;
   }catch(Exception error){
    DisposeFemalePixelV53();DisposeFemaleNativeDisplayV28();femaleNativeBlockedRoomV28=room??currentRoom;femaleNativeBlockedEnemyV28=enemy??currentEnemy;
    Debug.LogWarning("FEMALE_NATIVE_DISPLAY_FALLBACK_V28 "+error.Message);
   }
  }
 }
}
