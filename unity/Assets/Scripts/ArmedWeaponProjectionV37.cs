using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    [Serializable] internal sealed class ArmedWeaponTrackV37 {
        public string motion;public int count,fps;public float[] values;
    }
    [Serializable] internal sealed class ArmedWeaponProjectionV37 {
        public string id,motionSha;public float[] tipLocal,forearmTipLocal;public ArmedWeaponTrackV37[] tracks;
        readonly Dictionary<string,ArmedWeaponTrackV37> lookup=new Dictionary<string,ArmedWeaponTrackV37>(StringComparer.Ordinal);
        float drawnGripTipLength,drawnElbowGripLength;
        internal static ArmedWeaponProjectionV37 Load(BendersFamilyRigV36 rig){
            var file=Resources.Load<TextAsset>(rig.root+"weapon-proof");
            ReusablePaintRigV37.Need(file&&ReusablePaintRigV37.Digest(file)==ArmedHumanoidPinsV37.WeaponProof(rig.npc),"pinned source weapon projection");
            var bank=JsonUtility.FromJson<ArmedWeaponProjectionV37>(file.text);
            ReusablePaintRigV37.Need(bank.id==rig.npc&&bank.motionSha==rig.motionSha&&bank.tipLocal.Length==2&&bank.forearmTipLocal.Length==2&&bank.tracks.Length==rig.lib.tracks.Length,"weapon registration");
            bank.drawnGripTipLength=new Vector2(bank.tipLocal[0],bank.tipLocal[1]).magnitude;
            bank.drawnElbowGripLength=new Vector2(bank.forearmTipLocal[0],bank.forearmTipLocal[1]).magnitude;
            ReusablePaintRigV37.Need(bank.drawnGripTipLength>.00001f&&bank.drawnElbowGripLength>.00001f,"drawn weapon and forearm lengths");
            foreach(var t in bank.tracks){
                var pose=rig.lib.Find(t.motion);
                ReusablePaintRigV37.Need(pose!=null&&t.fps==480&&t.count==Mathf.RoundToInt(pose.duration*t.fps)+1&&t.values.Length==4*t.count,"480Hz original hold and tip "+t.motion);
                bank.lookup.Add(t.motion,t);
            }
            return bank;
        }
        static void Aim(Transform axis,Vector2 delta,float length){
            if(delta.sqrMagnitude>1e-10f)axis.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(delta.y,delta.x)*Mathf.Rad2Deg);
            var scale=axis.localScale;scale.x=delta.magnitude/length;axis.localScale=scale;
        }
        internal void Apply(FairyAuthoredPartsV33.Frame frame,Transform weaponAxis,Transform forearmAxis){
            var t=lookup[frame.alias];float at=Mathf.Clamp(frame.sourceClock*t.fps,0,t.count-1);
            int a=Mathf.FloorToInt(at),b=Math.Min(t.count-1,a+1);float f=at-a;
            float V(int i){return Mathf.Lerp(t.values[4*a+i],t.values[4*b+i],f);}
            var grip=new Vector2(V(0),V(1));var delta=new Vector2(V(2),V(3))-grip;
            weaponAxis.localPosition=grip;
            // Set both projected endpoints directly. Interpolating an angle
            // across an almost edge-on blade can put the point on the wrong
            // side of the hand even when its two entire key poses are exact.
            Aim(weaponAxis,delta,drawnGripTipLength);
            Aim(forearmAxis,grip-(Vector2)forearmAxis.localPosition,drawnElbowGripLength);
        }
    }
}
