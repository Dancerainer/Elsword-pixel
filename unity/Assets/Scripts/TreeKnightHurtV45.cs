using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    // Screen-space poses of the six actual painted pivots. Bone rolls are
    // interpolated along the shortest arc and can pass through +/-180 degrees.
    internal static class TreeKnightHurtV45 {
        [Serializable] internal sealed class Track {
            public string motion;
            public float duration;
            public int count;
            public float[] values;
            internal void Sample(float time,int part,out float x,out float y,out float turn){
                float t=Mathf.Clamp(time*24,0,count-1);
                int a=(int)t,b=Math.Min(a+1,count-1),p=(a*6+part)*3,q=(b*6+part)*3;
                x=Mathf.Lerp(values[p],values[q],t-a);y=Mathf.Lerp(values[p+1],values[q+1],t-a);
                turn=Mathf.LerpAngle(values[p+2],values[q+2],t-a);
            }
        }
        [Serializable] sealed class Document {public string npc;public int fps;public Track[] tracks;}
        static Dictionary<string,Track> tracks;
        static Dictionary<string,Track> attacks;
        internal static Track OriginalAttackV45(Enemy actor,out float seconds){
            seconds=0;
            if(actor.state!=CombatState.Normal)return null;
            string motion;
            if(actor.npcState=="TREE_KNIGHT_ATTACK_READY"){motion="Attack_Ready";seconds=actor.npcTime*1.25f;}
            else if(actor.npcState=="TREE_KNIGHT_ATTACK_A"){motion="Attack_Fire";seconds=actor.npcTime;}
            else return null;
            if(attacks==null)attacks=Load("Pixel/monster-fidelity-v45/tree-knight-attack",actor.sourceNPC);
            return attacks[motion];
        }
        internal static Track For(Enemy actor,out float seconds){
            seconds=0;
            if(actor==null||actor.sourceNPC!="NUI_TREE_KNIGHT"||
               !OriginalReactionV45.TryPhysicalOrCommittedV45(actor,out var reaction,out seconds))return null;
            if(tracks==null)tracks=Load("Pixel/feedback-v45/tree-knight-hurt",actor.sourceNPC);
            return tracks.TryGetValue(reaction.rendered,out var selected)?selected:null;
        }
        static Dictionary<string,Track> Load(string key,string npc){
                var data=Resources.Load<TextAsset>(key);
                if(!data)throw new InvalidOperationException("Missing TREE_KNIGHT registration");
                var doc=JsonUtility.FromJson<Document>(data.text);
                if(doc.npc!=npc||doc.fps!=24)throw new InvalidOperationException("Unexpected TREE_KNIGHT source bank");
                var loaded=new Dictionary<string,Track>(StringComparer.Ordinal);
                foreach(var t in doc.tracks){
                    if(t.count<2||t.values.Length!=t.count*6*3)throw new InvalidOperationException("Incomplete original TREE_KNIGHT motion "+t.motion);
                    loaded.Add(t.motion,t);
                }
                return loaded;
        }
    }
}
