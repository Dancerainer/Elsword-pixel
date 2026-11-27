using System;
namespace ElswordPixel {
    [Serializable] public sealed class NpcSteamVisualData {
        public string textureResource;
        public NpcSteamEmitter[] emitters=new NpcSteamEmitter[0];
        public float unit,offsetY;
    }
    [Serializable] public sealed class NpcSteamEmitter {
        public string id;public float at,rate,lifetime,triggerDuration;public int capacity,layer;
        public float[] radius,gravity,velocityY,initialSize,initialColor,rotationZ;
        public NpcSteamFade[] sizeFades=new NpcSteamFade[0],colorFades=new NpcSteamFade[0],rotationFades=new NpcSteamFade[0];
    }
    [Serializable] public sealed class NpcSteamFade {public float start,end;public float[] value;}
}
