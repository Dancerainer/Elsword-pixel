#if ROOT_REDRAW_CHECK_V32 && !UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace ElswordPixel {
 public sealed partial class CoreReactionPlayerCheckV32 {
  readonly bool denseRequestedV44=Array.IndexOf(Environment.GetCommandLineArgs(),"--body-pixels-every-frame-v44")>=0;
  RenderTexture offTargetV44;
  Texture2D onReadV44,offReadV44;
  [Serializable] sealed class BodyPixelFrameV44 {
   public string npc,phase,clip,sourceState,headPart;public int frame,pixels,members,groundLine;
   public float clock,npcClock,actorX,actorY,facing,bodyWorldHeight,heroWorldHeight,headWidthScale,headHeightScale,headZ;
  }
  void CheckBodyContributionV44(SpriteRenderer[] body,Camera cam,FrameRow row){
   int w=observationTarget.width,h=observationTarget.height;
   int x0=Mathf.Clamp(Mathf.FloorToInt(row.viewportMin.x*w)-3,0,w-1),x1=Mathf.Clamp(Mathf.CeilToInt(row.viewportMax.x*w)+3,x0+1,w);
   int y0=Mathf.Clamp(Mathf.FloorToInt(row.viewportMin.y*h)-3,0,h-1),y1=Mathf.Clamp(Mathf.CeilToInt(row.viewportMax.y*h)+3,y0+1,h);
   int rw=x1-x0,rh=y1-y0;
   if(!offTargetV44){offTargetV44=new RenderTexture(w,h,24,RenderTextureFormat.ARGB32){antiAliasing=1,filterMode=FilterMode.Point};Need(offTargetV44.Create(),"Frame visibility target");}
   int tw=Mathf.NextPowerOfTwo(rw),th=Mathf.NextPowerOfTwo(rh);
   if(!onReadV44||onReadV44.width<tw||onReadV44.height<th){
    if(onReadV44){tw=Mathf.Max(tw,onReadV44.width);th=Mathf.Max(th,onReadV44.height);Destroy(onReadV44);Destroy(offReadV44);}
    onReadV44=new Texture2D(tw,th,TextureFormat.RGBA32,false);offReadV44=new Texture2D(tw,th,TextureFormat.RGBA32,false);
   }
   var rt=RenderTexture.active;var prev=cam.targetTexture;bool priorPair=pairRender;int lastPost=worldFrame,pixels=0;
   var enabled=new bool[body.Length];for(int i=0;i<body.Length;i++)enabled[i]=body[i].enabled;
   try{
    pairRender=true;
    RenderTexture.active=observationTarget;onReadV44.ReadPixels(new Rect(x0,y0,rw,rh),0,0,false);
    cam.targetTexture=offTargetV44;RenderTexture.active=offTargetV44;GL.Clear(true,true,Color.clear);
    foreach(var v in body)v.enabled=false;cam.Render();
    RenderTexture.active=offTargetV44;offReadV44.ReadPixels(new Rect(x0,y0,rw,rh),0,0,false);
    var a=onReadV44.GetRawTextureData<Color32>();var b=offReadV44.GetRawTextureData<Color32>();
    for(int y=0;y<rh;y++)for(int x=0;x<rw;x++){
     int n=y*onReadV44.width+x;var p=a[n];var q=b[n];
     if(Math.Abs(p.r-q.r)+Math.Abs(p.g-q.g)+Math.Abs(p.b-q.b)>6)pixels++;
    }
   }finally{for(int i=0;i<body.Length;i++)body[i].enabled=enabled[i];cam.targetTexture=prev;RenderTexture.active=rt;worldFrame=lastPost;pairRender=priorPair;}
   var f=new BodyPixelFrameV44{npc=row.npc,phase=row.phase,clip=row.clip,sourceState=row.state,npcClock=target.npcTime,frame=row.frame,pixels=pixels,clock=row.clock,members=body.Length,
     actorX=target.x,actorY=target.y,groundLine=target.groundLine,facing=target.facing,bodyWorldHeight=row.bounds.size.y,heroWorldHeight=((SpriteRenderer)Field(scene,"heroView")).bounds.size.y};
   foreach(var s in body)if(s.gameObject.name=="head"||s.gameObject.name=="helmet"){
    f.headPart=s.gameObject.name;f.headWidthScale=s.transform.lossyScale.x;f.headHeightScale=s.transform.lossyScale.y;f.headZ=s.transform.eulerAngles.z;break;
   }
   File.AppendAllText(Path.Combine(output,"body-pixels.jsonl"),JsonUtility.ToJson(f)+"\n");
   Need(pixels>0,"Actual body contributed zero pixels on "+row.npc+"/"+row.phase+" frame "+row.frame);
  }
  void ReleaseDenseEvidenceV44(){
   if(offTargetV44){offTargetV44.Release();Destroy(offTargetV44);}
   if(onReadV44)Destroy(onReadV44);if(offReadV44)Destroy(offReadV44);
  }
 }
}
#endif
