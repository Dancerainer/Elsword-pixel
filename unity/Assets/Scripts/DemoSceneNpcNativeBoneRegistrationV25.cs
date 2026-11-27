using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        [Serializable] sealed class First3BoneFrameV25 {public float x,y,sourceBipX,sourceBipY,sourceTime;}
        [Serializable] sealed class First3BoneClipV25 {public string kind,clip;public float fps,duration,waitBipX,waitBipY;public First3BoneFrameV25[] frames;}
        [Serializable] sealed class First3BoneCatalogV25 {public string schema;public First3BoneClipV25[] entries;}
        [Serializable] sealed class First3BoneObservationV25 {
            public string clip,sprite;public int frame;public float sourceTime,registeredBipErrorLogical;
            public Vector3 positionBefore,positionAfter,expectedShift,actualShift,actualBipAnchor,expectedBipAnchor;
        }
        Dictionary<string,First3BoneClipV25> first3BoneClipsV25;
        readonly Dictionary<Enemy,First3BoneObservationV25> first3BoneObservationsV25=new Dictionary<Enemy,First3BoneObservationV25>();

        First3BoneObservationV25 First3ActualBoneObservationV25(Enemy enemy) {
            First3BoneObservationV25 observation;
            return first3BoneObservationsV25.TryGetValue(enemy,out observation)?observation:null;
        }

        void ApplyFirst3NativeBoneRegistrationV25(Enemy enemy,SpriteRenderer view) {
            if (!First3NativeNormalSpriteV25(enemy,view)) return;
            if (first3BoneClipsV25==null) {
                var text=Resources.Load<TextAsset>("Pixel/feedback-v25/native-bip-registration");
                var data=text?JsonUtility.FromJson<First3BoneCatalogV25>(text.text):null;
                if(data==null||data.schema!="first3-native-bip-registration-v25-r1"||data.entries==null||data.entries.Length!=35)
                    throw new InvalidOperationException("Missing exact first3 native Bip registration");
                first3BoneClipsV25=new Dictionary<string,First3BoneClipV25>();
                foreach(var entry in data.entries) {
                    var info=libraries[entry.kind].clips[entry.clip];
                    if(entry.frames==null||entry.frames.Length!=info.frames.Length||Mathf.Abs(entry.fps-info.fps)>.00001f||Mathf.Abs(entry.duration-info.duration)>.00001f)
                        throw new InvalidOperationException("Native Bip registration frame inventory differs: "+entry.kind+"/"+entry.clip);
                    first3BoneClipsV25.Add(entry.kind+"/"+entry.clip,entry);
                }
            }
            string sprite=view.sprite.name;int slash=sprite.LastIndexOf('/');int index;
            if(slash<0||!int.TryParse(sprite.Substring(slash+1),out index))throw new InvalidOperationException("Native sprite frame identity missing");
            string key=sprite.Substring(0,slash);First3BoneClipV25 clip;
            if(!first3BoneClipsV25.TryGetValue(key,out clip)||index<0||index>=clip.frames.Length)
                throw new InvalidOperationException("Actual native sprite has no same-frame Bip registration: "+sprite);
            var frame=clip.frames[index];float scale=libraries[enemy.kind].data.logicalScale*enemy.visualScale;
            float facing=view.flipX?-1:1;var before=view.transform.localPosition;
            // This is the original bone translation channel of this exact
            // sampled sprite, relative to original Wait frame0. It is not a
            // bounds fit, old-art match, physics move or interpolated pose.
            var shift=new Vector3(frame.x*facing,frame.y,0)*scale;
            view.transform.localPosition=before+shift;var after=view.transform.localPosition;
            var anchorOffset=SpriteOffset(view);
            var body=after-new Vector3(anchorOffset.x,anchorOffset.y,0)*scale+new Vector3(frame.sourceBipX*facing,frame.sourceBipY,0)*scale;
            var root=RenderEnemyPosition(enemy);
            float floor=InitialBatV25(enemy)?InitialBatFlightFloorV25(enemy):0;
            var expected=new Vector3(root.x+clip.waitBipX*facing*scale,root.y+clip.waitBipY*scale-floor,0);
            first3BoneObservationsV25[enemy]=new First3BoneObservationV25{clip=clip.clip,sprite=sprite,frame=index,sourceTime=frame.sourceTime,positionBefore=before,positionAfter=after,expectedShift=shift,actualShift=after-before,actualBipAnchor=body,expectedBipAnchor=expected,registeredBipErrorLogical=Vector2.Distance(new Vector2(body.x,body.y),new Vector2(expected.x,expected.y))};
        }
    }
}
