using System;
namespace ElswordPixel {
 public sealed partial class GameModel {
  // Presentation-only: called after dealt>0 from the committed source line
  // collision. The same clip intervals select the first contact point of this
  // candidate's existing 2D player volume. No native 3D impact ABI is claimed.
  void RecordBelowLineImpactPresentationV21(NpcEffectInstance e,float at,float[] lines,int i,float targetX,float targetY){
   if(belowCandidateV21==null||e==null||e.reflected)return;
   if(e.id!="CANNONFORT_PARASITE_LASER"&&e.id!="CANNONFORT_PARASITE_LASER_BIG")return;
   float start=0,end=1,ax=lines[i],ay=lines[i+1],az=lines[i+2],bx=lines[i+3],by=lines[i+4],bz=lines[i+5];
   if(!ClipNpcLineAxis(ax,bx,targetX-9.8f,targetX+9.8f,ref start,ref end)||!ClipNpcLineAxis(ay,by,targetY,targetY+42,ref start,ref end)||!ClipNpcLineAxis(az,bz,-9.8f,9.8f,ref start,ref end))return;
   RecordNpcPresentationHit(e,at,ax+(bx-ax)*start,ay+(by-ay)*start,az+(bz-az)*start);
  }
 }
}
