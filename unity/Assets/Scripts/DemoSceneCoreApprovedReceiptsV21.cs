#if UNITY_EDITOR
namespace ElswordPixel {public sealed partial class DemoScene {
 static void ValidateCoreHardApprovedWholeReceiptsV21(CoreEntryScopeV21 b){
  PreviewRequire(b.kingWhole!=null&&b.kingWhole.sha256=="bb425852876c069322b8e9ef1b1c71ff3254cae607b22b99c01f85ac107774b3"&&b.kingReviewReceipts!=null&&b.kingReviewReceipts.Length==5,"Core latest King3 exact external receipt set required");
  string[] expected={"234b9580521313bbfb7fb6edde9da2a77701abcee794dc780992fe494f200c2b","789413514fedd9296bb4642bffee3719f68e73b4891616e24d0cfcfb9a26e3a1","a6d1f7f5068ddc790adf020225ff8873f78ffcd3ad1eac0aca75466cfe768080","4d523dbba3bb8009d914cbabe7d84db6e870c0a29bed2c381a2fd83fde2d0be9","730df3139ba6444766aa46b1f59cd32110efbe163157c476e06196eb8f016605"};
  CoreEntryReadV21(b.kingWhole);for(int i=0;i<expected.Length;i++){PreviewRequire(b.kingReviewReceipts[i]!=null&&b.kingReviewReceipts[i].sha256==expected[i],"Core King approved scope receipt changed");CoreEntryReadV21(b.kingReviewReceipts[i]);}
  PreviewRequire(b.alarmHandoff!=null&&b.alarmHandoff.sha256=="ff2c23f55f3f976379f751f8b27c4f6a643ddc3358ba78e034f3a160d268e14d"&&b.alarmIndex!=null&&b.alarmIndex.sha256=="4ab8434ef526080a2848af619778bf115724cb37f1f32c3fc5089d7fa7dedfac","Core four Alarm static/body library exact receipt required");CoreEntryReadV21(b.alarmHandoff);CoreEntryReadV21(b.alarmIndex);ValidateCoreExtractorHandoffV21(b.extractorHandoff,b.extractorIndex);
 }
}}
#endif
