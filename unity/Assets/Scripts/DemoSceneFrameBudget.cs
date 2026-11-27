using System;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        readonly MobileFrameBudget autoFrameBudget = new MobileFrameBudget();
        readonly float[] displayedFrameTimes = new float[180];
        int displayedFrameCount;
        float displayedSeconds, lastTouchTime = -1;
        RoomDefinition observedBudgetRoom;
        internal void MarkMobileTouchActivity(){lastTouchTime=Time.unscaledTime;}

        void ObserveMobileFrameBudget(float seconds) {
            if (!mobileDisplayReady || resolutionMode != MobileResolution.Auto || game.paused) return;
            if (!ReferenceEquals(observedBudgetRoom,game.CurrentRoom)) {
                observedBudgetRoom=game.CurrentRoom;
                autoFrameBudget.BeginRoom();
            }
            float now=Time.unscaledTime;
            if (touches.Count>0 || UnityEngine.Input.touchCount>0) lastTouchTime=now;
            if (autoFrameBudget.Observe(seconds,now,now-lastTouchTime>.3f)) ApplyMobileResolution();
        }

        void SetFrameRateDisplay(bool enabled) {
            showPerformance=enabled;
            displayedFrameCount=0;
            displayedSeconds=0;
            PlayerPrefs.SetInt(FrameRateDisplayKey,enabled?1:0);
        }
        void RefreshFrameRateDisplay(float seconds) {
            if (!showPerformance || game.paused || !string.IsNullOrEmpty(openMenu)) {
                displayedFrameCount=0;
                displayedSeconds=0;
                return;
            }
            if (!(seconds>0)) return;
            displayedSeconds+=seconds;
            displayedFrameTimes[displayedFrameCount++]=seconds;
            if (displayedSeconds<1 && displayedFrameCount<displayedFrameTimes.Length) return;
            Array.Sort(displayedFrameTimes,0,displayedFrameCount);
            int index=Math.Max(0,(int)Math.Ceiling(displayedFrameCount*.95)-1);
            TextValue("Performance",(displayedFrameCount/displayedSeconds).ToString("F0")+" FPS  /  P95 "+(1000*displayedFrameTimes[index]).ToString("F0")+" MS");
            displayedFrameCount=0;
            displayedSeconds=0;
        }
    }

    // Two slow windows lower the output one step. A single load or unusually long
    // frame cannot lower quality. Recovery needs sustained headroom and a dwell time.
    internal sealed class MobileFrameBudget {
        static readonly int[] caps={720,540,432};
        int defaultTier, tier, requestedTier, samples, slowFrames, slowWindows;
        float windowTime, fastTime, settleTime, lastChange=-1000;
        bool configured;
        internal int ShortEdge {get {return caps[tier];}}
        internal void Configure(int maxHeight) {
            int best=maxHeight<=540?1:0;
            if(configured&&defaultTier==best)return;
            defaultTier=best;configured=true;Reset();
        }
        internal void Reset() {
            tier=requestedTier=defaultTier;
            lastChange=-1000;
            BeginRoom();
        }
        internal void BeginRoom() {
            ClearWindow();
            requestedTier=tier;
            fastTime=0;slowWindows=0;settleTime=1.5f;
        }
        void ClearWindow(){windowTime=0;samples=0;slowFrames=0;}
        internal bool Observe(float dt,float now,bool canResize) {
            if(!configured||!(dt>0))return false;
            if(settleTime>0){settleTime-=Math.Min(dt,.1f);return false;}
            if(requestedTier!=tier){
                if(!canResize)return false;
                tier=requestedTier;lastChange=now;BeginRoom();return true;
            }
            windowTime+=Math.Min(dt,.1f);samples++;
            if(dt>.020f)slowFrames++;
            if(windowTime<2)return false;
            float average=windowTime/samples;
            if(average>.019f&&slowFrames*4>=samples){
                fastTime=0;
                if(++slowWindows>=2&&tier<caps.Length-1)requestedTier=tier+1;
            }else{
                slowWindows=0;
                fastTime=average<.01725f&&slowFrames*20<=samples?fastTime+windowTime:0;
                if(fastTime>=30 && now-lastChange>=45 && tier>defaultTier)requestedTier=tier-1;
            }
            ClearWindow();return false;
        }
    }
}
