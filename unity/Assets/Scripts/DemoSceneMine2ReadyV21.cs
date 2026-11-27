#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
namespace ElswordPixel {
 public sealed partial class DemoScene {
  [Serializable] sealed class Mine2ReadyScopeV21 {public PreviewFile naturalExecution,independentImageReview;public bool naturalRunVerified;}
  [Serializable] sealed class Mine2NaturalProofV21 {public string schema;public bool passed,actualUnity,inputOnly,sourcesUnchanged,resourcesUnchanged,dependenciesUnchanged,naturalThreeRoomsPassed,runtimeAdmitted;public PreviewFile report,bundle,before,after;}
  [Serializable] sealed class Mine2NaturalReportV21 {public bool passed,victory,allRoomsReached;public int expectedRooms,room,npcSoundsMissing;public string bundleSha256;public string[] screenshots,errors;}
  [Serializable] sealed class Mine2NaturalImageReviewV21 {public string schema,reviewer;public bool passed,actualUnity,naturalRoute,runtimeAdmitted;public PreviewFile executionBinding;public PreviewFile[] captures;}
  static void ValidateMine2ReadyEvidenceV21(string path){
   var s=JsonUtility.FromJson<Mine2ReadyScopeV21>(File.ReadAllText(path));var p=Mine2JsonV21<Mine2NaturalProofV21>(s.naturalExecution);
   PreviewRequire(s.naturalRunVerified&&p.schema=="mine2-natural-actual-execution-v21"&&p.passed&&p.actualUnity&&p.inputOnly&&p.sourcesUnchanged&&p.resourcesUnchanged&&p.dependenciesUnchanged&&p.naturalThreeRoomsPassed&&!p.runtimeAdmitted,"Mine2 ready entry requires actual normal-input three-room execution");
   Mine2ReadV21(p.bundle);Mine2ReadV21(p.before);Mine2ReadV21(p.after);var r=Mine2JsonV21<Mine2NaturalReportV21>(p.report);PreviewRequire(r.passed&&r.victory&&r.allRoomsReached&&r.expectedRooms==3&&r.room==2&&r.npcSoundsMissing==0&&r.bundleSha256==p.bundle.sha256&&(r.errors==null||r.errors.Length==0)&&r.screenshots!=null&&r.screenshots.Length>=9,"Mine2 actual route evidence failed");
   var review=Mine2JsonV21<Mine2NaturalImageReviewV21>(s.independentImageReview);PreviewRequire(review.schema=="mine2-natural-image-review-root-v21"&&review.passed&&review.actualUnity&&review.naturalRoute&&!review.runtimeAdmitted&&review.reviewer=="root"&&Mine2SameV21(review.executionBinding,s.naturalExecution)&&review.captures!=null&&review.captures.Length==r.screenshots.Length,"Mine2 natural-route image review does not bind actual execution");
   for(int i=0;i<r.screenshots.Length;i++){PreviewRequire(review.captures[i].path==r.screenshots[i],"Mine2 reviewed capture order differs from route");Mine2ReadV21(review.captures[i]);}
  }
 }
}
#endif
