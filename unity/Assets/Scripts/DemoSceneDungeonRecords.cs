using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        const string BestKeyPrefix = "dungeon.best.v1.";
        readonly Dictionary<string,string> bestLabels = new Dictionary<string,string>();
        DungeonClearResult recordedClear;
        bool NewDungeonBest { get; set; }

        static bool ReadDungeonBest(string id, out int time, out int score) {
            time = score = 0;
            string saved = PlayerPrefs.GetString(BestKeyPrefix + id, "");
            int divider = saved.IndexOf('|');
            return divider > 0
                && int.TryParse(saved.Substring(0, divider), NumberStyles.None, CultureInfo.InvariantCulture, out time)
                && int.TryParse(saved.Substring(divider + 1), NumberStyles.None, CultureInfo.InvariantCulture, out score)
                && score <= 100;
        }

        void CaptureDungeonBest() {
            if (!adventureStarted || !game.victory) return;
            var result = game.ClearResult;
            if (result == null || ReferenceEquals(result, recordedClear)) return;
            recordedClear = result;
            NewDungeonBest = false;
            if (runAutomatedPlaythrough || calibrationRun || rubenReview) return;
            string id = Dungeon.id;
            if (string.IsNullOrEmpty(id)) return;
            NewDungeonBest = SaveDungeonBest(id, result.TimeCentiseconds, result.TotalScore);
            if (NewDungeonBest) bestLabels.Remove(id);
        }

        static bool SaveDungeonBest(string id, int newTime, int newScore) {
            int time, score;
            if (ReadDungeonBest(id, out time, out score) && (time < newTime
                || time == newTime && score >= newScore)) return false;
            string entry = newTime.ToString(CultureInfo.InvariantCulture) + "|"
                + newScore.ToString(CultureInfo.InvariantCulture);
            PlayerPrefs.SetString(BestKeyPrefix + id, entry);
            PlayerPrefs.Save();
            return true;
        }

        string DungeonBestLabel(CandidateDungeonArt route) {
            var baseRoutes = mainDungeons ?? catalog.dungeons;
            string id = route.runtimeIndex >= 0 && baseRoutes != null && route.runtimeIndex < baseRoutes.Length
                ? baseRoutes[route.runtimeIndex].id : route.id;
            if (string.IsNullOrEmpty(id)) return "";
            string label;
            if (bestLabels.TryGetValue(id, out label)) return label;
            int time, score;
            label = ReadDungeonBest(id, out time, out score)
                ? "BEST  " + DungeonClearResult.FormatTime(time) + "   " + DungeonClearResult.GradeForScore(score) + " / " + score.ToString(CultureInfo.InvariantCulture)
                : "BEST  --";
            bestLabels[id] = label;
            return label;
        }
    }
}
