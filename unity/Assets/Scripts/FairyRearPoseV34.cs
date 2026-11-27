using System;
using System.Security.Cryptography;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class FairyPoseLibraryV33 {
        [Serializable] sealed class RearStandDocumentV34 {
            public string schema, originalMotionSha256, registeredBasePoseSha256;
            public FairyPoseTrackV33 track;
        }
        void AppendSourceRearPosesV34() {
            const string expected = "4ed676a0f52c602da1ccf69c268048b336750e89227ee6eb2ba0a93c7f7dd875";
            var data = Resources.Load<TextAsset>(ResourceRoot + "stand-up-back-v34");
            Require(data != null, "original rear stand source pose document");
            using (var sha = SHA256.Create())
                Require(BitConverter.ToString(sha.ComputeHash(data.bytes)).Replace("-", "").ToLowerInvariant() == expected,
                    "actual rear pose bytes");
            var doc = JsonUtility.FromJson<RearStandDocumentV34>(data.text);
            Require(doc.schema == "fairy-actual-back-stand-v34" && doc.originalMotionSha256 == motionSha256 &&
                doc.registeredBasePoseSha256 == PoseSha && doc.track.alias == "source-get-up-back-v34" &&
                doc.track.animation == "DamageStandUpBack" && doc.track.count == 45 && !doc.track.loop &&
                Math.Abs(doc.track.duration - GameModel.FairyBackStandSecondsV34) < .000001f &&
                doc.track.values.Length == 45 * 21, "original 45-sample rear stand motion");
            var down = byAlias["source-death"];
            Require(down.animation == "DamageDownBack" && down.count == 32 &&
                Math.Abs(down.duration - GameModel.FairyDownSecondsV34) < .000001f, "existing rear down material");
            byAlias.Add("source-down-back-v34", down);
            byAlias.Add(doc.track.alias, doc.track);
        }
    }
}

