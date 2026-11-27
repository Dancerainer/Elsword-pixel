using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class SwampCueV39 { public string key; public float at; }
        [Serializable] sealed class SwampDirectorV39 {
            public int frames, eye, look;
            public string sourceMotion;
            public AirshipDirectorMeshV20[] meshes;
            public SwampCueV39[] cues;
        }
        const string SwampResourceV39 = "Pixel/feedback-v39/swamp-director/";
        readonly List<AirshipDirectorViewV20> swampMeshesV39 = new List<AirshipDirectorViewV20>();
        readonly Dictionary<string, Texture2D> swampSkinsV39 = new Dictionary<string, Texture2D>();
        readonly HashSet<string> swampAudioV39 = new HashSet<string>();
        Enemy swampDisplayActorV39;
        SwampDirectorV39 swampMovieV39;
        float[] swampMotionV39;
        Matrix4x4[] swampBonesV39;
        AirshipDirectorCameraSnapshotV20 swampCameraV39;
        int swampNextCueV39,swampLastFrameV39 = -1;
        public int SwampMovieMeshCountV39 { get { return swampMeshesV39.Count; } }
        public bool SwampMovieCameraV39 { get { return swampCameraV39 != null; } }
        public List<MeshRenderer> SwampVisibleMovieMeshesV39() {
            var result = new List<MeshRenderer>();
            foreach (var item in swampMeshesV39) if (item.renderer && item.renderer.enabled) result.Add(item.renderer);
            return result;
        }
        // The verified NA StringTable is empty. Preserve caption identities and timing.
        public int SwampMovieCaptionIdV39 {
            get {
                float age = game.SwampIntroAgeV39;
                if (age >= 5 && age < 6.6f) return 30635;
                if (age >= 2 && age < 4) return 30633;
                return age >= .2f && age < 2 ? 30632 : 0;
            }
        }

        static T SwampResource<T>(string name) where T : UnityEngine.Object {
            var asset = Resources.Load<T>(SwampResourceV39 + name);
            if (!asset) throw new InvalidOperationException("Missing swamp director asset " + name);
            return asset;
        }
        Texture2D SwampTextureV39(string key) {
            if (string.IsNullOrEmpty(key)) return Texture2D.whiteTexture;
            if (swampSkinsV39.TryGetValue(key, out var previous)) return previous;
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            swampSkinsV39.Add(key, texture);
            if (!texture.LoadImage(SwampResource<TextAsset>(key).bytes, true))
                throw new InvalidOperationException("Invalid swamp director texture " + key);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;
            return texture;
        }
        static bool SwampAncientMaterialV42(string key) {
            return key == "Ancient_Pporu_Body" || key == "Ancient_Pporu_Face" ||
                   key == "Ancient_Pporu_Hand" || key == "Ancient_Pporu_Foot";
        }
        void LoadSwampMovieV39(Enemy actor) {
            swampDisplayActorV39 = actor;
            swampMovieV39 = JsonUtility.FromJson<SwampDirectorV39>(SwampResource<TextAsset>("director").text);
            var bytes = SwampResource<TextAsset>("motion").bytes;
            if (swampMovieV39.sourceMotion != "25e1892719e0ee5ef9950ffdded6380bbda6eda59bc879cffc9a9059ab3e9006" ||
                swampMovieV39.frames != 114 || bytes.Length != 751 * 114 * 16 * 4 || swampMovieV39.meshes.Length != 8)
                throw new InvalidOperationException("Swamp director motion/geometry mismatch");
            swampMotionV39 = new float[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, swampMotionV39, 0, bytes.Length);
            swampBonesV39 = new Matrix4x4[swampMovieV39.frames];
            var shader = Resources.Load<Shader>("Pixel/Shaders/NativeWorld");
            var ancientInk = Resources.Load<Shader>("Pixel/Shaders/SwampAncientInkV42");
            var depth = nativeTarget ? nativeTarget.depthStencilFormat : GraphicsFormat.None;
            bool canInk = ancientInk && ancientInk.isSupported &&
                (depth == GraphicsFormat.D24_UNorm_S8_UInt || depth == GraphicsFormat.D32_SFloat_S8_UInt ||
                 depth == GraphicsFormat.D16_UNorm_S8_UInt);
            if (!canInk) Debug.LogWarning("SWAMP_ANCIENT_INK_FALLBACK_V42 using painted NativeWorld skin; depth=" + depth);
            foreach (var m in swampMovieV39.meshes) {
                var v = new AirshipDirectorViewV20 { source = m, mesh = new Mesh { name = "Swamp movie " + m.name },
                    original = new Vector3[m.vertexCount], current = new Vector3[m.vertexCount],
                    offsets = new Matrix4x4[m.skins.Length], materials = new Material[m.materials.Length] };
                swampMeshesV39.Add(v);
                var uv = new Vector2[m.vertexCount];
                for (int i = 0; i < m.vertexCount; i++) {
                    v.original[i] = new Vector3(m.positions[i*3],m.positions[i*3+1],m.positions[i*3+2]);
                    uv[i] = new Vector2(m.uv[i*2],m.uv[i*2+1]);
                }
                v.mesh.MarkDynamic(); v.mesh.vertices = v.original; v.mesh.uv = uv;
                v.mesh.subMeshCount = m.submeshes.Length;
                for (int i = 0; i < m.submeshes.Length; i++) v.mesh.SetTriangles(m.submeshes[i].indices,i,false);
                for (int i = 0; i < m.skins.Length; i++) v.offsets[i] = AirshipDirectorMatrixV20(m.skins[i].offsetRowMajorColumnMatrix,0);
                for (int i = 0; i < m.materials.Length; i++) {
                    string key = m.materials[i].textureKey;
                    bool useInk = canInk && SwampAncientMaterialV42(key);
                    var material = new Material(useInk ? ancientInk : shader);
                    v.materials[i] = material;
                    material.mainTexture = SwampTextureV39(key);
                    material.color = NativeColor(m.materials[i].diffuse);
                    if (!useInk) material.SetFloat("_DetailReduction",0);
                }
                var obj = new GameObject("Swamp entrance / " + m.name);
                obj.layer = 28; obj.transform.SetParent(nativeRoot,false);
                obj.AddComponent<MeshFilter>().sharedMesh = v.mesh;
                v.renderer = obj.AddComponent<MeshRenderer>(); v.renderer.sharedMaterials = v.materials;
            }
        }
        void SkinSwampMovieV39(float age) {
            int frame = Mathf.Clamp(Mathf.RoundToInt(age*60),0,750);
            if (frame == swampLastFrameV39) return;
            swampLastFrameV39 = frame;
            for (int i = 0; i < swampBonesV39.Length; i++)
                swampBonesV39[i] = AirshipDirectorMatrixV20(swampMotionV39,(frame*swampBonesV39.Length+i)*16);
            foreach (var v in swampMeshesV39) {
                if (v.source.skins.Length == 0) {
                    var matrix = swampBonesV39[v.source.frame];
                    for (int i = 0; i < v.current.Length; i++) v.current[i] = matrix.MultiplyPoint3x4(v.original[i]);
                } else {
                    Array.Clear(v.current,0,v.current.Length);
                    for (int j = 0; j < v.source.skins.Length; j++) {
                        var group = v.source.skins[j];
                        var matrix = swampBonesV39[group.frame] * v.offsets[j];
                        for (int k = 0; k < group.vertexIndices.Length; k++) {
                            int vertex = group.vertexIndices[k];
                            v.current[vertex] += matrix.MultiplyPoint3x4(v.original[vertex])*group.weights[k];
                        }
                    }
                }
                v.mesh.vertices = v.current; v.mesh.RecalculateBounds();
            }
        }
        void SwampSoundCuesV39(float age) {
            while (swampNextCueV39 < swampMovieV39.cues.Length && swampMovieV39.cues[swampNextCueV39].at <= age) {
                var cue = swampMovieV39.cues[swampNextCueV39++];
                if (cue.at > GameModel.SwampDirectorDurationV39 && swampDisplayActorV39.npcState != "ANCIENT_PPORU_START") continue;
                string id = "swamp-director/" + cue.key;
                if (swampAudioV39.Add(id)) audioClips[id] = SwampResource<AudioClip>(cue.key);
                PlayNativeSound(new TimedSound {sound=id, volume=1},65);
            }
        }
        void EndSwampCameraV39() {
            var s = swampCameraV39;
            if (s != null && s.native) {
                s.native.transform.SetPositionAndRotation(s.nativePosition,s.nativeRotation);
                s.native.orthographic = s.orthographic; s.native.orthographicSize = s.size;
                s.native.enabled = s.enabled; s.native.fieldOfView = s.fov; s.native.aspect = s.aspect;
                s.native.nearClipPlane = s.near; s.native.farClipPlane = s.far;
            }
            swampCameraV39 = null;
            foreach (var v in swampMeshesV39) if (v.renderer) v.renderer.enabled = false;
        }
        void DisposeSwampMovieV39() {
            EndSwampCameraV39();
            foreach (var v in swampMeshesV39) {
                if (v.renderer) DestroyNativeWorldObject(v.renderer.gameObject);
                foreach (var mat in v.materials) DestroyNativeWorldObject(mat);
                DestroyNativeWorldObject(v.mesh);
            }
            swampMeshesV39.Clear();
            foreach (var t in swampSkinsV39.Values) DestroyNativeWorldObject(t);
            swampSkinsV39.Clear();
            foreach (var key in swampAudioV39) audioClips.Remove(key);
            swampAudioV39.Clear();
            swampLastFrameV39=-1; swampNextCueV39=0;
            swampDisplayActorV39=null; swampBonesV39=null; swampMotionV39=null; swampMovieV39=null;
        }
        bool UpdateSwampMovieV39() {
            if (game.SimpleDangerIntroductions) { if (swampDisplayActorV39 != null) DisposeSwampMovieV39(); return false; }
            var actor = game.SwampIntroActorV39;
            float age = game.SwampIntroAgeV39;
            if (swampDisplayActorV39 != null && (actor != swampDisplayActorV39 || !NativeWorldActive ||
                openMenu == "dungeons" || age >= GameModel.SwampDirectorDurationV39+GameModel.SwampStartDurationV39))
                DisposeSwampMovieV39();
            if (actor == null || !NativeWorldActive || openMenu == "dungeons" || !worldCamera || !nativeRoot) return false;
            if (swampMovieV39 == null) {
                if (age >= GameModel.SwampDirectorDurationV39) return false;
                try { LoadSwampMovieV39(actor); } catch { DisposeSwampMovieV39(); throw; }
            }
            SwampSoundCuesV39(age);
            if (age >= GameModel.SwampDirectorDurationV39) { if (swampCameraV39 != null) EndSwampCameraV39(); return false; }
            SkinSwampMovieV39(age);
            if (swampCameraV39 == null) swampCameraV39 = new AirshipDirectorCameraSnapshotV20 {
                native=nativeCamera,nativePosition=nativeCamera.transform.position,nativeRotation=nativeCamera.transform.rotation,
                orthographic=nativeCamera.orthographic,enabled=nativeCamera.enabled,size=nativeCamera.orthographicSize,
                fov=nativeCamera.fieldOfView,aspect=nativeCamera.aspect,near=nativeCamera.nearClipPlane,far=nativeCamera.farClipPlane };
            Vector3 eye=swampBonesV39[swampMovieV39.eye].GetColumn(3),look=swampBonesV39[swampMovieV39.look].GetColumn(3);
            nativeCamera.transform.SetPositionAndRotation(eye,Quaternion.LookRotation(look-eye,Vector3.up));
            nativeCamera.enabled=false; nativeCamera.orthographic=false; nativeCamera.fieldOfView=70;
            nativeCamera.nearClipPlane=1; nativeCamera.farClipPlane=100000;
            var center = worldCamera.transform.position;
            nativeScreen.transform.localPosition=new Vector3(center.x,center.y,10);
            return true;
        }
    }
}
