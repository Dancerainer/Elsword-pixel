using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ElswordPixel {
 [Serializable] public sealed class MonsterTextureRedrawEntryV22 {
  public string id,sourceRgbaSha256,targetResource,targetPngSha256,targetRgbaSha256;
  public int width,height;
  public bool approved;
 }
 [Serializable] public sealed class MonsterTextureRedrawManifestV22 {
  public string schema;
  public MonsterTextureRedrawEntryV22[] entries;
 }

 // A presentation-only adapter. Original textures, materials, sprites, meshes,
 // animation data and source validation remain available and are never edited.
 // Canonical hashes use top-to-bottom RGBA bytes, matching PNG/Pillow order.
 public sealed class MonsterTextureRedrawV22:MonoBehaviour {
  public const string ManifestResource="Pixel/monster-texture-redraw-v22/manifest";
  sealed class Target {
   internal MonsterTextureRedrawEntryV22 entry;
   internal Texture2D texture;
   internal uint updateCount;
   internal string alpha;
   internal bool valid=true;
  }
  sealed class TextureMatch {
   internal Texture2D source;
   internal uint updateCount;
   internal int width,height;
   internal Target target;
  }
  sealed class SpriteCopy {internal Sprite source,copy;internal Target target;internal int copyId;}
  sealed class MaterialCopy {internal Material source,copy;internal Target target;internal int copyId;}
  static MonsterTextureRedrawV22 runtime;
  readonly Dictionary<string,Target> targets=new Dictionary<string,Target>(StringComparer.Ordinal);
  readonly HashSet<Vector2Int> approvedSizes=new HashSet<Vector2Int>();
  readonly Dictionary<int,TextureMatch> textures=new Dictionary<int,TextureMatch>();
  readonly Dictionary<string,SpriteCopy> sprites=new Dictionary<string,SpriteCopy>();
  readonly Dictionary<int,SpriteCopy> reverseSprites=new Dictionary<int,SpriteCopy>();
  readonly Dictionary<string,MaterialCopy> materials=new Dictionary<string,MaterialCopy>();
  readonly Dictionary<int,MaterialCopy> reverseMaterials=new Dictionary<int,MaterialCopy>();
  readonly HashSet<SpriteRenderer> touchedSprites=new HashSet<SpriteRenderer>();
  readonly HashSet<Renderer> touchedMaterials=new HashSet<Renderer>();
  readonly HashSet<GameObject> explicitRoots=new HashSet<GameObject>();
  readonly List<string> diagnostics=new List<string>();
  bool subscribed,applying,configured;
  int passes;
  public int SourceTextureHashes {get;private set;}
  public int TargetTextureHashes {get;private set;}
  public int SpriteCopies {get{return sprites.Count;}}
  public int MaterialCopies {get{return materials.Count;}}
  public int ApprovedEntries {get{return targets.Count;}}
  public bool IsConfigured {get{return configured;}}
  public bool CameraHookEnabled {get{return subscribed;}}
  public int CameraApplications {get;private set;}
  public string[] Diagnostics {get{return diagnostics.ToArray();}}

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
  static void ResetRuntime(){if(runtime){runtime.EnableCameraHook(false);Destroy(runtime.gameObject);}runtime=null;}
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  static void InstallRuntime(){
   if(runtime)return;
   var manifest=Resources.Load<TextAsset>(ManifestResource);
   if(!manifest){Debug.LogWarning("Monster texture redraw v22: manifest missing; original presentation retained.");return;}
   var host=new GameObject("Monster texture redraw v22");
   DontDestroyOnLoad(host);runtime=host.AddComponent<MonsterTextureRedrawV22>();
   if(!runtime.Configure(manifest.text,key=>Resources.Load<Texture2D>(key))){Destroy(host);runtime=null;}
  }
  void OnEnable(){if(Application.isPlaying)EnableCameraHook(true);}
  void OnDisable(){EnableCameraHook(false);RestoreOriginals();}
  void OnDestroy(){EnableCameraHook(false);ReleaseCopies();if(runtime==this)runtime=null;}
  void OnSceneUnloaded(Scene scene){PruneDestroyed();}
  public void EnableCameraHook(bool enabled){
   if(enabled==subscribed)return;
   subscribed=enabled;
   if(enabled){Camera.onPreCull+=BeforeCamera;SceneManager.sceneUnloaded+=OnSceneUnloaded;}
   else {Camera.onPreCull-=BeforeCamera;SceneManager.sceneUnloaded-=OnSceneUnloaded;}
  }
  void BeforeCamera(Camera camera){
   if(!this||!isActiveAndEnabled||!configured||targets.Count==0)return;
   CameraApplications++;
   // Enumerating renderers catches same-frame pooled/native actor creation.
   // Reading/hash of a large texture happens once per instance/updateCount.
   ApplyRenderers(FindObjectsOfType<Renderer>());
   // Preview scenes are intentionally excluded by Unity's ordinary scene
   // discovery. Controlled editor tools may register their own isolated root.
   foreach(var root in explicitRoots)if(root)ApplyHierarchy(root);
  }
  public void RegisterHierarchy(GameObject root){if(root)explicitRoots.Add(root);}
  public void UnregisterHierarchy(GameObject root){explicitRoots.Remove(root);}
  void Note(string message){
   string full="Monster texture redraw v22: "+message;
   if(diagnostics.Contains(full))return;
   diagnostics.Add(full);Debug.LogWarning(full,this);
  }
  static bool Sha(string value){
   if(value==null||value.Length!=64)return false;
   foreach(char c in value)if(!((c>='0'&&c<='9')||(c>='a'&&c<='f')))return false;
   return true;
  }
  static string Hex(byte[] bytes){return BitConverter.ToString(bytes).Replace("-","").ToLowerInvariant();}
  static void Fingerprints(Texture2D texture,out string rgba,out string alpha){
   if(!texture||!texture.isReadable)throw new InvalidOperationException("Texture must be readable.");
   var pixels=texture.GetPixels32();
   var row=new byte[texture.width*4];var arow=new byte[texture.width];
   using(var color=SHA256.Create())using(var coverage=SHA256.Create()){
    for(int y=texture.height-1;y>=0;y--){
     for(int x=0;x<texture.width;x++){
      var p=pixels[y*texture.width+x];int at=x*4;
      row[at]=p.r;row[at+1]=p.g;row[at+2]=p.b;row[at+3]=p.a;arow[x]=p.a;
     }
     color.TransformBlock(row,0,row.Length,row,0);
     coverage.TransformBlock(arow,0,arow.Length,arow,0);
    }
    color.TransformFinalBlock(new byte[0],0,0);coverage.TransformFinalBlock(new byte[0],0,0);
    rgba=Hex(color.Hash);alpha=Hex(coverage.Hash);
   }
  }
  public static string RgbaSha256(Texture2D texture){string rgba,alpha;Fingerprints(texture,out rgba,out alpha);return rgba;}
  public static string AlphaSha256(Texture2D texture){string rgba,alpha;Fingerprints(texture,out rgba,out alpha);return alpha;}
  public bool ConfigureFromResources(string resource=ManifestResource){
   var manifest=Resources.Load<TextAsset>(resource);
   if(!manifest){Note("manifest missing: "+resource+"; current presentation retained.");return false;}
   return Configure(manifest.text,key=>Resources.Load<Texture2D>(key));
  }
  // Public injection is for controlled EditMode tests and explicit project
  // consumers. Production uses Resources and the same validation path.
  public bool Configure(string json,Func<string,Texture2D> load){
   var prepared=new Dictionary<string,Target>(StringComparer.Ordinal);
   var ids=new HashSet<string>(StringComparer.Ordinal);
   var loaded=new Dictionary<string,Target>(StringComparer.Ordinal);
   try{
    if(load==null)throw new InvalidOperationException("Target loader missing.");
    var manifest=JsonUtility.FromJson<MonsterTextureRedrawManifestV22>(json);
    if(manifest==null||manifest.schema!="monster-texture-redraw-v22"||manifest.entries==null)
     throw new InvalidOperationException("Invalid manifest schema/entries.");
    foreach(var e in manifest.entries){
     if(e==null)throw new InvalidOperationException("Null manifest entry.");
     if(!e.approved)continue;
     if(string.IsNullOrEmpty(e.id)||!ids.Add(e.id)||!Sha(e.sourceRgbaSha256)||!Sha(e.targetRgbaSha256)||!Sha(e.targetPngSha256)||e.width<=0||e.height<=0||string.IsNullOrEmpty(e.targetResource)||e.targetResource.Contains("..")||e.targetResource.StartsWith("/")||e.targetResource.Contains("\\")||e.targetResource.Contains(":"))
      throw new InvalidOperationException("Invalid approved entry: "+e.id);
     Target t;
     if(!loaded.TryGetValue(e.targetResource,out t)){
      var texture=load(e.targetResource);
      if(!texture||!texture.isReadable||texture.width!=e.width||texture.height!=e.height)
       throw new InvalidOperationException("Missing/unreadable/wrong-size target: "+e.id);
      string rgba,alpha;Fingerprints(texture,out rgba,out alpha);TargetTextureHashes++;
      if(rgba!=e.targetRgbaSha256)throw new InvalidOperationException("Target RGBA hash differs: "+e.id);
      // targetPngSha256 is retained as source-artifact provenance. Unity's PNG
      // importer exposes decoded pixels, so targetRgbaSha256 is the exact
      // authoritative runtime integrity check, including alpha and dimensions.
      t=new Target{entry=e,texture=texture,updateCount=texture.updateCount,alpha=alpha};loaded.Add(e.targetResource,t);
     }else if(t.entry.targetRgbaSha256!=e.targetRgbaSha256||t.texture.width!=e.width||t.texture.height!=e.height)
      throw new InvalidOperationException("Conflicting target resource: "+e.id);
     Target previous;
     if(prepared.TryGetValue(e.sourceRgbaSha256,out previous)){
      if(previous.entry.targetRgbaSha256!=e.targetRgbaSha256||previous.texture.width!=e.width||previous.texture.height!=e.height)
       throw new InvalidOperationException("Conflicting source mapping: "+e.id);
     }else prepared.Add(e.sourceRgbaSha256,t);
    }
   }catch(Exception ex){Note("configuration rejected; current presentation retained. "+ex.Message);return false;}
   // Commit only after every approved entry has been verified.
   ReleaseCopies();targets.Clear();approvedSizes.Clear();
   foreach(var pair in prepared){var t=pair.Value;targets.Add(pair.Key,t);t.texture.filterMode=FilterMode.Point;approvedSizes.Add(new Vector2Int(t.entry.width,t.entry.height));}
   configured=true;return true;
  }
  bool TargetStillValid(Target t){
   if(!t.valid)return false;
   if(!t.texture){t.valid=false;Note("approved target destroyed: "+t.entry.id);return false;}
   if(t.texture.updateCount==t.updateCount)return true;
   t.updateCount=t.texture.updateCount;
   try{
    string rgba,alpha;Fingerprints(t.texture,out rgba,out alpha);TargetTextureHashes++;
    if(t.texture.width!=t.entry.width||t.texture.height!=t.entry.height||rgba!=t.entry.targetRgbaSha256)
     throw new InvalidOperationException("RGBA/size changed after configuration.");
   }catch(Exception e){t.valid=false;Note("approved target invalidated; originals restored: "+t.entry.id+". "+e.Message);}
   return t.valid;
  }
  Target Replacement(Texture texture){
   var source=texture as Texture2D;if(!source)return null;
   // A redraw must preserve the exact UV dimensions. Reject unrelated hero,
   // effect and scenery atlases before reading back any pixels or retaining them.
   if(!approvedSizes.Contains(new Vector2Int(source.width,source.height)))return null;
   int key=source.GetInstanceID();TextureMatch match;
   if(textures.TryGetValue(key,out match)&&ReferenceEquals(match.source,source)&&match.updateCount==source.updateCount&&match.width==source.width&&match.height==source.height)
    return match.target!=null&&TargetStillValid(match.target)?match.target:null;
   match=new TextureMatch{source=source,updateCount=source.updateCount,width=source.width,height=source.height};textures[key]=match;
   if(!source.isReadable)return null;
   // Targets can be seen by nested cameras; never hash/rewrite them again.
   foreach(var pair in targets)if(ReferenceEquals(source,pair.Value.texture))return null;
   try{
    string rgba,alpha;Fingerprints(source,out rgba,out alpha);SourceTextureHashes++;
    Target target;
    if(targets.TryGetValue(rgba,out target)&&TargetStillValid(target)){
     if(source.width!=target.entry.width||source.height!=target.entry.height){Note("source size mismatch: "+target.entry.id);return null;}
     if(alpha!=target.alpha){Note("alpha differs; replacement rejected: "+target.entry.id);return null;}
     match.target=target;return target;
    }
   }catch(Exception e){Note("source texture skipped: "+source.name+". "+e.Message);}
   return null;
  }
  internal static bool SameSourceFrameV35(Sprite actual,Sprite selected){
   if(!actual||!selected)return false;
   if(actual==selected)return true;
   // Camera.onPreCull can publish a texture-repainted copy of this source
   // frame before actor projection. Only accept a copy registered by this owner.
   return runtime && runtime.SourceSprite(actual)==runtime.SourceSprite(selected);
  }
  Sprite SourceSprite(Sprite sprite){SpriteCopy record;return sprite&&reverseSprites.TryGetValue(sprite.GetInstanceID(),out record)&&ReferenceEquals(record.copy,sprite)?record.source:sprite;}
  Material SourceMaterial(Material material){MaterialCopy record;return material&&reverseMaterials.TryGetValue(material.GetInstanceID(),out record)&&ReferenceEquals(record.copy,material)?record.source:material;}
  static bool Equal<T>(T[] a,T[] b){if(a.Length!=b.Length)return false;for(int n=0;n<a.Length;n++)if(!EqualityComparer<T>.Default.Equals(a[n],b[n]))return false;return true;}
  static bool SameGeometry(Sprite a,Sprite b){return Equal(a.vertices,b.vertices)&&Equal(a.triangles,b.triangles)&&Equal(a.uv,b.uv);}
  static bool SamePhysics(Sprite a,Sprite b){
   int count=a.GetPhysicsShapeCount();if(count!=b.GetPhysicsShapeCount())return false;
   var aa=new List<Vector2>();var bb=new List<Vector2>();
   for(int n=0;n<count;n++){aa.Clear();bb.Clear();a.GetPhysicsShape(n,aa);b.GetPhysicsShape(n,bb);if(!Equal(aa.ToArray(),bb.ToArray()))return false;}
   return true;
  }
  Sprite CopySprite(Sprite source,Target target){
   SpriteCopy record;string key=source.GetInstanceID()+":"+target.entry.targetRgbaSha256;
   if(sprites.TryGetValue(key,out record)&&ReferenceEquals(record.source,source)&&record.target==target&&record.copy)return record.copy;
   if(record!=null){reverseSprites.Remove(record.copyId);if(record.copy)DestroyOwned(record.copy);sprites.Remove(key);}
   if(source.packed){Note("packed sprite requires an explicit unpacked adapter; original retained: "+source.name);return source;}
   Sprite copy=null;
   try{
    var rect=source.rect;var pivot=new Vector2(source.pivot.x/rect.width,source.pivot.y/rect.height);
    bool hasPhysics=source.GetPhysicsShapeCount()>0;
    copy=Sprite.Create(target.texture,rect,pivot,source.pixelsPerUnit,0,SpriteMeshType.FullRect,source.border,hasPhysics);
    copy.name=source.name;
    if(!SameGeometry(source,copy)){
     DestroyOwned(copy);copy=Sprite.Create(target.texture,rect,pivot,source.pixelsPerUnit,0,SpriteMeshType.Tight,source.border,hasPhysics);copy.name=source.name;
     if(!SameGeometry(source,copy)){
      // Unity rejects OverrideGeometry outside its player loop without throwing.
      // EditMode therefore fails closed rather than silently changing geometry.
      if(!Application.isPlaying)throw new InvalidOperationException("Custom geometry needs the player loop; generated geometry did not match.");
      copy.OverrideGeometry(source.vertices,source.triangles);
      if(!SameGeometry(source,copy))throw new InvalidOperationException("Unity refused exact source geometry.");
     }
    }
    int count=source.GetPhysicsShapeCount();
    if(!SamePhysics(source,copy)){
     if(!Application.isPlaying)throw new InvalidOperationException("Custom physics shape needs the player loop; generated shape did not match.");
     var shapes=new List<Vector2[]>();var points=new List<Vector2>();for(int n=0;n<count;n++){points.Clear();source.GetPhysicsShape(n,points);shapes.Add(points.ToArray());}copy.OverridePhysicsShape(shapes);
     if(!SamePhysics(source,copy))throw new InvalidOperationException("Unity refused exact source physics shape.");
    }
    copy.hideFlags=HideFlags.HideAndDontSave;record=new SpriteCopy{source=source,copy=copy,target=target,copyId=copy.GetInstanceID()};sprites.Add(key,record);reverseSprites.Add(record.copyId,record);return copy;
   }catch(Exception e){if(copy)DestroyOwned(copy);Note("sprite clone rejected; original retained: "+source.name+". "+e.Message);return source;}
  }
  Material CopyMaterial(Material source,Target target){
   MaterialCopy record;string key=source.GetInstanceID()+":"+target.entry.targetRgbaSha256;
   if(!materials.TryGetValue(key,out record)||!ReferenceEquals(record.source,source)||record.target!=target||!record.copy){
    if(record!=null){reverseMaterials.Remove(record.copyId);if(record.copy)DestroyOwned(record.copy);materials.Remove(key);}
    record=new MaterialCopy{source=source,target=target,copy=new Material(source){name=source.name,hideFlags=HideFlags.HideAndDontSave}};record.copyId=record.copy.GetInstanceID();
    materials.Add(key,record);reverseMaterials.Add(record.copyId,record);
   }
   // Preserve live color/UV/render-state changes authored by the source owner.
   record.copy.shader=source.shader;record.copy.CopyPropertiesFromMaterial(source);record.copy.mainTexture=target.texture;
   return record.copy;
  }
  public void ApplyHierarchy(GameObject root){if(root)ApplyRenderers(root.GetComponentsInChildren<Renderer>(true));}
  public void ApplyRenderers(Renderer[] renderers){
   if(!configured||applying||renderers==null)return;applying=true;
   try{
    foreach(var renderer in renderers){
     if(!renderer)continue;
     var spriteRenderer=renderer as SpriteRenderer;
     if(spriteRenderer){
      var original=SourceSprite(spriteRenderer.sprite);if(!original)continue;
      var target=Replacement(original.texture);var selected=target==null?original:CopySprite(original,target);
      if(spriteRenderer.sprite!=selected){spriteRenderer.sprite=selected;touchedSprites.Add(spriteRenderer);}continue;
     }
     if(!(renderer is MeshRenderer)&&!(renderer is SkinnedMeshRenderer))continue;
     var list=renderer.sharedMaterials;bool changed=false;
     for(int n=0;n<list.Length;n++){
      var original=SourceMaterial(list[n]);if(!original)continue;
      var target=Replacement(original.mainTexture);var selected=target==null?original:CopyMaterial(original,target);
      if(list[n]!=selected){list[n]=selected;changed=true;}
     }
     if(changed){renderer.sharedMaterials=list;touchedMaterials.Add(renderer);}
    }
    if(++passes%120==0)PruneDestroyed();
   }finally{applying=false;}
  }
  public void RestoreOriginals(){
   foreach(var renderer in touchedSprites)if(renderer){var original=SourceSprite(renderer.sprite);if(renderer.sprite!=original)renderer.sprite=original;}
   foreach(var renderer in touchedMaterials)if(renderer){var list=renderer.sharedMaterials;bool changed=false;for(int n=0;n<list.Length;n++){var original=SourceMaterial(list[n]);if(list[n]!=original){list[n]=original;changed=true;}}if(changed)renderer.sharedMaterials=list;}
   touchedSprites.Clear();touchedMaterials.Clear();
  }
  static void DestroyOwned(UnityEngine.Object obj){if(!obj)return;if(Application.isPlaying)Destroy(obj);else DestroyImmediate(obj);}
  void ReleaseCopies(){
   RestoreOriginals();foreach(var pair in sprites)DestroyOwned(pair.Value.copy);foreach(var pair in materials)DestroyOwned(pair.Value.copy);
   sprites.Clear();reverseSprites.Clear();materials.Clear();reverseMaterials.Clear();textures.Clear();
  }
  public void PruneDestroyed(){
   explicitRoots.RemoveWhere(r=>!r);
   touchedSprites.RemoveWhere(r=>!r);touchedMaterials.RemoveWhere(r=>!r);
   var dead=new List<int>();foreach(var p in textures)if(!p.Value.source)dead.Add(p.Key);foreach(int key in dead)textures.Remove(key);
   var deadCopies=new List<string>();foreach(var p in sprites)if(!p.Value.source||!p.Value.copy)deadCopies.Add(p.Key);
   foreach(string key in deadCopies){var r=sprites[key];reverseSprites.Remove(r.copyId);if(r.copy)DestroyOwned(r.copy);sprites.Remove(key);}
   deadCopies.Clear();foreach(var p in materials)if(!p.Value.source||!p.Value.copy)deadCopies.Add(p.Key);
   foreach(string key in deadCopies){var r=materials[key];reverseMaterials.Remove(r.copyId);if(r.copy)DestroyOwned(r.copy);materials.Remove(key);}
  }
 }
}
