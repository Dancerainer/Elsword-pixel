namespace ElswordPixel {
 public sealed partial class GameModel {
  // Presentation only, after the existing source-line test and positive HP loss.
  // Clip against that same model volume; no new hit, damage or enemy mechanism.
  void RecordFactorySheriffLineImpactV32(NpcEffectInstance e,float at,float[] line,int i,float targetX,float targetY){
   if(e==null||e.reflected||e.id!="DamageEffect_NASOD_SHERIFF_ATTACK_A"||e.owner==null||
       (e.owner.sourceNPC!="NUI_NASOD_SHERIFF"&&!CoreEliteSheriffBirthV35(e.owner)))return;
   float a=0,b=1,ax=line[i],ay=line[i+1],az=line[i+2],bx=line[i+3],by=line[i+4],bz=line[i+5];
   if(!ClipNpcLineAxis(ax,bx,targetX-9.8f,targetX+9.8f,ref a,ref b)||!ClipNpcLineAxis(ay,by,targetY,targetY+42,ref a,ref b)||!ClipNpcLineAxis(az,bz,-9.8f,9.8f,ref a,ref b))return;
   RecordNpcPresentationHit(e,at,ax+(bx-ax)*a,ay+(by-ay)*a,az+(bz-az)*a);
  }
 }
}
