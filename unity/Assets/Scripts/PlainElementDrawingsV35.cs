using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    // The three variants in each element family share the same original vertex and
    // UV data for all 165 common poses. Plain's own state selection supplies the clock.
    sealed class PlainElementDrawingsV35 {
        const string SourceRoot = "Pixel/dungeons/altera-core-hard/data/";
        internal const string FireDoc = "b3db35a4f93dce05cfc7b16e9de17635bc1e2fcefac104faf6512eb44e10e1c1";
        internal const string IceDoc = "71d4f55e3b9a11d0bf6812120e09679ef2a8c61353b40a9e302debdcd817ee95";
        readonly Dictionary<string,SourceWholeMotionV21> motions = new Dictionary<string,SourceWholeMotionV21>();
        readonly Dictionary<string,SourceWholeKeyV21> keys = new Dictionary<string,SourceWholeKeyV21>();
        readonly Dictionary<string,Sprite> sprites = new Dictionary<string,Sprite>();
        readonly List<UnityEngine.Object> owned = new List<UnityEngine.Object>();
        readonly Action<UnityEngine.Object> release;
        internal readonly float scale;
        internal readonly string originalMotionSha;
        internal PlainElementDrawingsV35(bool fire,Action<UnityEngine.Object> releaseOwned) {
            release=releaseOwned;
            string pin=fire?FireDoc:IceDoc;
            var bytes=Read(pin);
            var doc=FiniteWholeJsonR268.Read<SourceWholeDocumentV21>(bytes);
            var bank=new SourcePhaseWholeLookupV21(doc);
            Need(bank.SourceNpc==(fire?"NUI_NASOD_ELEMENT_FIRE_B":"NUI_NASOD_ELEMENT_ICE_B") &&
                bank.KeyCount==(fire?98:153) && bank.MotionCount==28 && bank.StateCount==33,"original drawing bank");
            foreach(var motion in doc.motions) motions.Add(motion.id,motion);
            foreach(var key in doc.keys) keys.Add(key.key,key);
            // 116 and 114 are the existing neutral drawings' actual alpha64 heights.
            scale=fire?1.3232137333f:1.3213557927f;
            originalMotionSha=fire?"2b0af5b00e2e14f1819e9e751a20fbdb72d21ca58a3c0fd893642289eeb5ee7b":
                "83ede79e31640f05fa2a30502249c3289b181abf24811d3ca5a27c3c8f36f08f";
        }
        static void Need(bool ok,string why) {
            if(!ok)throw new InvalidOperationException("Plain element drawings V35: "+why);
        }
        static byte[] Read(string sha) {
            var asset=Resources.Load<TextAsset>(SourceRoot+sha);
            Need(asset,"shipped resource "+sha);
            var raw=asset.bytes;
            using(var hash=System.Security.Cryptography.SHA256.Create())
                Need(BitConverter.ToString(hash.ComputeHash(raw)).Replace("-","").Equals(sha,StringComparison.OrdinalIgnoreCase),"resource digest "+sha);
            return raw;
        }
        Sprite SpriteFor(string id) {
            if(sprites.TryGetValue(id,out var sprite))return sprite;
            var key=keys[id];
            var texture=new Texture2D(2,2,TextureFormat.RGBA32,false,false){
                name="Plain element "+id,filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
            try {
                Need(ImageConversion.LoadImage(texture,Read(key.png.sha256),true) &&
                    texture.width==key.width && texture.height==key.height,"whole dimensions "+id);
                sprite=Sprite.Create(texture,new Rect(0,0,key.width,key.height),new Vector2(.5f,.5f),1,0,SpriteMeshType.FullRect);
                Need(sprite,"create "+id);
                sprite.name="Plain element V35/"+id;
                owned.Add(texture);owned.Add(sprite);sprites.Add(id,sprite);return sprite;
            } catch {if(sprite)release(sprite);release(texture);throw;}
        }
        internal void Apply(SpriteRenderer renderer,PlainVariablePhaseBankR1330 source,PlainVariablePhaseMotionR1330 sourceMotion,
            double clock,Vector2 foot,int facing,out string selectedKey) {
            Need(source.sourceMotion.sha256==originalMotionSha && motions.TryGetValue(sourceMotion.animation,out _) &&
                !double.IsNaN(clock) && !double.IsInfinity(clock) && clock>=0,"known own motion and clock");
            var motion=motions[sourceMotion.animation];
            Need(Math.Abs(motion.sourceMotionDurationOriginal-sourceMotion.sourceMotionDuration)<.00002,"source duration "+motion.id);
            int lo=0,hi=motion.phases.Length;
            while(lo+1<hi){
                int mid=(lo+hi)/2;
                if(motion.phases[mid].sourceTimeOriginal<=clock+.000001)lo=mid;else hi=mid;
            }
            var phase=motion.phases[lo];
            var key=keys[phase.paintedWholeKey];
            renderer.sprite=SpriteFor(key.key);renderer.flipX=facing<0;
            renderer.transform.localScale=Vector3.one*scale;
            float sign=renderer.flipX?-1:1;
            renderer.transform.localPosition=new Vector3(foot.x+(float)(key.width*.5-phase.anchor[0])*scale*sign,
                foot.y+(float)(phase.anchor[1]-key.height*.5)*scale,0);
            selectedKey=key.key;
        }
        internal void Dispose() {
            for(int i=owned.Count-1;i>=0;i--)if(owned[i])release(owned[i]);
            owned.Clear();sprites.Clear();
        }
    }

    public sealed partial class DemoScene {
        PlainElementDrawingsV35 plainFireDrawingsV35,plainIceDrawingsV35;
        readonly HashSet<string> plainElementUnavailableV35=new HashSet<string>();
        partial void RecordPlainGuardianSmallSelectionV35(Enemy actor,SpriteRenderer renderer,PlainVariablePhaseBankR1330 bank,
            PlainVariablePhaseStateR1330 state,PlainVariablePhaseMotionR1330 motion,double clock,string poseId);
        partial void RecordPlainNasodMouseSelectionV35(Enemy actor,SpriteRenderer renderer,PlainVariablePhaseBankR1330 bank,
            PlainVariablePhaseStateR1330 state,PlainVariablePhaseMotionR1330 motion,double clock,string poseId);
        partial void RecordPlainSpraySelectionV35(Enemy actor,SpriteRenderer renderer,PlainVariablePhaseBankR1330 bank,
            PlainVariablePhaseStateR1330 state,PlainVariablePhaseMotionR1330 motion,double clock,string poseId);
        partial void RecordPlainIceRecoverySelectionV35(Enemy actor,SpriteRenderer renderer,PlainVariablePhaseBankR1330 bank,
            PlainVariablePhaseStateR1330 state,PlainVariablePhaseMotionR1330 motion,double clock,string poseId);

        void RecordPlainAuthoredSelectionV35(Enemy actor,SpriteRenderer renderer,PlainVariablePhaseBankR1330 bank,
            PlainVariablePhaseStateR1330 state,PlainVariablePhaseMotionR1330 motion,double clock,string poseId) {
            RecordPlainGuardianSmallSelectionV35(actor,renderer,bank,state,motion,clock,poseId);
            RecordPlainNasodMouseSelectionV35(actor,renderer,bank,state,motion,clock,poseId);
            RecordPlainSpraySelectionV35(actor,renderer,bank,state,motion,clock,poseId);
            RecordPlainIceRecoverySelectionV35(actor,renderer,bank,state,motion,clock,poseId);
            ApplyPlainElementDrawingV35(actor,renderer,bank,motion,clock,RenderEnemyPosition(actor));
        }
        void ApplyPlainElementDrawingV35(Enemy actor,SpriteRenderer renderer,PlainVariablePhaseBankR1330 bank,
            PlainVariablePhaseMotionR1330 motion,double sourceClock,Vector2 foot) {
            if(plainProductRouteR1320!=PlainProductResourceIndexR1320.Normal)return;
            bool fire=bank.npc=="NUI_NASOD_ELEMENT_FIRE"||bank.npc=="NUI_NASOD_ELEMENT_FIRE_SOLO";
            bool ice=bank.npc=="NUI_NASOD_ELEMENT_ICE"||bank.npc=="NUI_NASOD_ELEMENT_ICE_SOLO";
            if(!fire&&!ice)return;
            if(ice && bank.npc=="NUI_NASOD_ELEMENT_ICE_SOLO" &&
                (motion.animation=="StandUpAttackFront"||motion.animation=="StandUpAttackBack")){
                return;
            }
            if(plainElementUnavailableV35.Contains(bank.npc))return;
            try {
                var drawings=fire?plainFireDrawingsV35:plainIceDrawingsV35;
                if(drawings==null){
                    drawings=new PlainElementDrawingsV35(fire,DestroyNativeWorldObject);
                    if(fire)plainFireDrawingsV35=drawings;else plainIceDrawingsV35=drawings;
                }
                drawings.Apply(renderer,bank,motion,sourceClock,foot,actor.facing,out _);
            } catch(Exception e){
                plainElementUnavailableV35.Add(bank.npc);
                Debug.LogWarning("PLAIN_ELEMENT_DRAWINGS_FALLBACK_V35 "+bank.npc+" "+e.Message);
            }
        }
        void DisposePlainElementDrawingsV35() {
            if(plainFireDrawingsV35!=null)plainFireDrawingsV35.Dispose();
            if(plainIceDrawingsV35!=null)plainIceDrawingsV35.Dispose();
            plainFireDrawingsV35=plainIceDrawingsV35=null;plainElementUnavailableV35.Clear();
        }
    }
}

