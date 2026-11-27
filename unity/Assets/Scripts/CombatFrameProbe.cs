using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace ElswordPixel {
    // Explicit opt-in for profiling a signed player with the ordinary combat entry.
    // adb shell am start ... --ez elsword.frameProbe true
    internal sealed class CombatFrameProbe {
        internal enum Phase { Simulation, Room, Actors, Effects, Hud, AttackGeometry, Presentation, Projection, Audio }
        static readonly string[] names=Enum.GetNames(typeof(Phase));
        readonly long[] ticks=new long[names.Length], maximum=new long[names.Length];
        long last,window;
        int frames;

        internal static CombatFrameProbe FromLaunchIntent() {
#if UNITY_ANDROID && !UNITY_EDITOR
            try {
                using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))
                using(var intent=activity?.Call<AndroidJavaObject>("getIntent")){
                    if(intent!=null && intent.Call<bool>("getBooleanExtra","elsword.frameProbe",false))return new CombatFrameProbe();
                }
            }catch(AndroidJavaException){return null;}
#endif
            return null;
        }
        internal void Begin() {
            last=Stopwatch.GetTimestamp();
            if(window==0)window=last;
        }
        internal void Mark(Phase phase) {
            long now=Stopwatch.GetTimestamp();
            int index=(int)phase;
            long elapsed=now-last;
            ticks[index]+=elapsed;
            maximum[index]=Math.Max(maximum[index],elapsed);
            last=now;
        }
        internal void End(string room) {
            Mark(Phase.Audio);
            frames++;
            if(last-window<Stopwatch.Frequency*2)return;
            var message=new StringBuilder("EL_FRAME_CPU room=").Append(room).Append(" frames=").Append(frames);
            double toMs=1000.0/Stopwatch.Frequency;
            for(int i=0;i<ticks.Length;i++) {
                message.Append(' ').Append(names[i]).Append('=')
                    .Append((ticks[i]*toMs/frames).ToString("F2",System.Globalization.CultureInfo.InvariantCulture))
                    .Append('/').Append((maximum[i]*toMs).ToString("F2",System.Globalization.CultureInfo.InvariantCulture));
            }
            UnityEngine.Debug.Log(message.ToString());
            Array.Clear(ticks,0,ticks.Length);Array.Clear(maximum,0,maximum.Length);
            frames=0;window=0;
        }
    }
}
