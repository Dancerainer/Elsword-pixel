using System;
namespace ElswordPixel {
 public sealed partial class GameModel {
  // Only channels omitted by the old moveX compiler. Existing moveX/lane
  // rotation remains sole X/XY owner. Called before ContactNpcEffect.
  void AdvanceCargoEffectMissingChannelsV21(NpcEffectInstance effect,float fromAge,float toAge){if(cargoSourceV21==null||effect==null||effect.owner==null||effect.reflected)return;if(effect.id=="DamageEffect_BEE_NASOD_ATTACK_A"&&effect.owner.sourceNPC=="NUI_BEE_NASOD"){double from=Math.Min(.2,fromAge),to=Math.Min(.2,toAge);effect.y+=(float)(-330*.28*(to-from)*effect.ownerScale);}else if(effect.id=="DamageEffect_WALLY_9TH_ARMOR_ATTACK_C"&&effect.owner.sourceNPC=="NUI_WALLY_9TH_ARMOR"){double a=Math.Max(0,Math.Min(.2,toAge)-Math.Min(.2,fromAge)),b=Math.Max(0,Math.Min(.4,toAge)-Math.Max(.2,fromAge));effect.z+=(float)((-200*a+400*b)*.28*effect.ownerScale);}}
 }
}
