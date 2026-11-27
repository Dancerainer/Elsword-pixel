using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        readonly List<RectTransform> menuBackdrops = new List<RectTransform>();
        RectTransform resultSheet;
        RectTransform resultWindow, pauseActions;

        static bool IsMenuButtonAction(string action) {
            switch(action) {
                case "pause": case "result": case "touch": case "sound": case "loadout":
                case "guide": case "settings": case "dungeons": case "equip-selected":
                case "skill-prev": case "skill-next": return true;
            }
            return action.StartsWith("settings-",System.StringComparison.Ordinal) || action.StartsWith("dungeon-",System.StringComparison.Ordinal) ||
                action.StartsWith("menu-",System.StringComparison.Ordinal) || action.StartsWith("region:",System.StringComparison.Ordinal) ||
                action.StartsWith("filter:",System.StringComparison.Ordinal) || action.StartsWith("slot:",System.StringComparison.Ordinal) ||
                action.StartsWith("choose:",System.StringComparison.Ordinal);
        }

        // Content stays inside the phone's safe area. Its background and pointer
        // barrier reach the physical edges even if a cutout offsets that content.
        void AddMenuBackdrop(Transform sheet, Color color) {
            var shade = Panel(sheet.name + " Backdrop", new Rect(), color, sheet);
            shade.raycastTarget = true;
            shade.transform.SetAsFirstSibling();
            menuBackdrops.Add(shade.rectTransform);
            FitMenuBackdrops();
        }

        void FitMenuBackdrops() {
            var canvas = (RectTransform)hudCanvas.transform;
            var bounds = canvas.rect;
            Vector3 bottom = canvas.TransformPoint(bounds.min), top = canvas.TransformPoint(bounds.max);
            foreach (var shade in menuBackdrops) {
                var parent = (RectTransform)shade.parent;
                Vector2 min = parent.InverseTransformPoint(bottom), max = parent.InverseTransformPoint(top);
                shade.anchoredPosition = min - parent.rect.min;
                shade.sizeDelta = max - min;
            }
        }

        static void PlaceMenuItem(Transform item, Rect rect) {
            var t = (RectTransform)item;
            t.anchoredPosition = rect.position;
            t.sizeDelta = rect.size;
            var caption = t.Find(t.name + " Text") as RectTransform;
            if (caption) caption.sizeDelta = t.sizeDelta;
        }

        void PrepareMenuLayout() {
            foreach (string name in new[]{"Menu Root", "Dungeon Root"}) {
                var face = safeRoot.Find(name).GetComponent<Image>();
                var background = face.color;
                background.a = 1;
                AddMenuBackdrop(face.transform, background);
                face.color = Color.clear;
                var rim = face.transform.Find("Pixel Inlay");
                if (rim) rim.gameObject.SetActive(false);
            }

            resultSheet = UIRect("Result sheet", safeRoot, new Rect(0,0,960,540));
            resultWindow = Panel("Result window", new Rect(256,157,448,226), C(0x21374b), resultSheet).rectTransform;
            var card = resultWindow;
            var title = safeRoot.Find("Result Title");
            var detail = safeRoot.Find("Result Detail");
            var primary = safeRoot.Find("Result Panel");
            title.SetParent(card,false); detail.SetParent(card,false); primary.SetParent(card,false);
            title.gameObject.SetActive(true);detail.gameObject.SetActive(true);primary.gameObject.SetActive(true);
            PlaceMenuItem(title, new Rect(20,147,408,55));
            PlaceMenuItem(detail, new Rect(20,90,408,55));
            var detailText = detail.GetComponent<Text>();
            detailText.fontSize = 16;
            detailText.horizontalOverflow = HorizontalWrapMode.Wrap;
            Panel("Result Divider",new Rect(24,86,400,2),C(0x567586),card);
            primary.GetComponent<Image>().color = C(0x39746e);
            primary.GetComponent<TouchControl>().tapOnRelease = true;
            pauseActions = UIRect("Pause actions",card,new Rect(24,80,400,112));
            Button("Result Skills","SKILLS",new Rect(0,60,195,50),"loadout",15,pauseActions);
            Button("Result Commands","COMMANDS",new Rect(205,60,195,50),"guide",15,pauseActions);
            Button("Result Settings","SETTINGS",new Rect(0,0,195,50),"settings",15,pauseActions);
            Button("Result Map","MAP",new Rect(205,0,195,50),"dungeons",15,pauseActions);
            primary.SetAsLastSibling();
            AddMenuBackdrop(resultSheet,C(0x07131e,.77f));
            resultSheet.gameObject.SetActive(false);

            // The menu buttons in the authored scene predate the runtime layouts.
            foreach (var control in safeRoot.GetComponentsInChildren<TouchControl>(true))
                if (!string.IsNullOrEmpty(control.action) && IsMenuButtonAction(control.action))
                    control.tapOnRelease=true;
        }

        bool? resultWasPaused;
        void RenderResultSheet(bool visible) {
            if (!resultSheet) return;
            bool cleared = visible && game.victory && game.ClearResult != null;
            RenderClearCard(cleared ? game.ClearResult : null);
            visible &= !cleared;
            if (resultSheet.gameObject.activeSelf != visible) resultSheet.gameObject.SetActive(visible);
            if (!visible) { resultWasPaused = null; return; }
            bool paused = !game.victory && game.player.hp>0;
            if (resultWasPaused != paused) {
                pauseActions.gameObject.SetActive(paused);
                PlaceMenuItem(resultWindow,paused?new Rect(256,108,448,324):new Rect(256,157,448,226));
                PlaceMenuItem(labels["Result Title"].transform,new Rect(20,paused?254:147,408,50));
                PlaceMenuItem(labels["Result Detail"].transform,new Rect(20,paused?208:90,408,45));
                PlaceMenuItem(resultWindow.Find("Result Divider"),new Rect(24,paused?198:86,400,2));
                PlaceMenuItem(images["Result Panel"].transform,new Rect(24,20,400,50));
                resultWasPaused = paused;
            }
            TextValue("Result Title", paused ? "PAUSED" : game.victory ? "DUNGEON CLEAR" : "DEFEATED");
            TextValue("Result Detail", paused ? DisplayDungeonTitleR1111() : game.kills+" ENEMIES   /   "+(int)game.elapsed+" SEC");
            TextValue("Result Panel Text", paused ? "RESUME" : "BACK TO MAP");
        }

        sealed class ResultLine {
            public CanvasGroup group;
            public RectTransform rect, meter;
            public Text value, points, grade;
            public int score, metric, lastPoints, lastMetric;
            public float y;
        }
        readonly ResultLine[] clearLines=new ResultLine[3];
        static readonly string[] coveredDuringClear={
            "Pause Button","Progress Track","Progress Fill","Dungeon Panel","Dungeon Accent","Zone","Title","Mission",
            "Touch Toggle","Sound Button","Loadout Button","Guide Button","Dungeon Button","Command Input","Link Window","Skill Phase",
            "Keyboard battle controls","Potion rack"
        };
        readonly List<GameObject> restoreAfterClear=new List<GameObject>();
        RectTransform clearCard,rankStamp;
        CanvasGroup clearFade,rankFade;
        DungeonResultInk rankEmblem;
        Text clearDungeon,clearOverall,clearBest;
        Text clearReturnLabel,clearCountdown;
        DungeonClearResult shownClear,clearTimerScore;
        float clearStart;
        const float ClearReturnSeconds=30f;
        int clearSecondsLeft;
        bool clearSettled,clearRankCuePlayed;

        void ReturnFromDungeonResult(){
            if(settingsOpen)CloseSettings();
            SetMenu("dungeons");
            adventureStarted=false;
        }

        bool TryReturnAfterClear(){
            if(clearTimerScore==null||!game.victory||clearTimerScore!=game.ClearResult||
                Time.unscaledTime-clearStart<ClearReturnSeconds)return false;
            ReturnFromDungeonResult();
            return true;
        }

        DungeonResultInk ResultInk(string name,Rect rect,Transform parent,DungeonResultInk.Shape shape) {
            var ink=UIRect(name,parent,rect).gameObject.AddComponent<DungeonResultInk>();
            ink.Draw(shape);return ink;
        }

        void BuildClearCard() {
            clearCard=UIRect("Dungeon clear scorecard",safeRoot,new Rect(488,329,456,200));
            clearFade=clearCard.gameObject.AddComponent<CanvasGroup>();
            var frame=ResultInk("Results frame",new Rect(0,0,466,214),clearCard,DungeonResultInk.Shape.Card);
            frame.rectTransform.localScale=new Vector3(456f/466,200f/214,1);
            Label("Clear heading","DUNGEON CLEAR",new Rect(15,167,300,29),25,C(0xffdf8c),TextAnchor.MiddleLeft,clearCard);
            clearDungeon=Label("Clear dungeon","",new Rect(17,148,299,21),14,C(0xe3dfeb),TextAnchor.MiddleLeft,clearCard);
            clearDungeon.horizontalOverflow=HorizontalWrapMode.Wrap;
            clearDungeon.verticalOverflow=VerticalWrapMode.Truncate;
            clearDungeon.resizeTextForBestFit=true;clearDungeon.resizeTextMinSize=9;clearDungeon.resizeTextMaxSize=14;
            Button("Clear return","MAP  >",new Rect(356,151,94,44),"result",12,clearCard);
            var returnButton=clearCard.Find("Clear return");
            returnButton.GetComponent<Image>().color=C(0x334259);
            clearReturnLabel=returnButton.GetComponentInChildren<Text>();
            clearCountdown=Label("Clear return countdown","",new Rect(333,131,110,18),9,
                C(0xe3dfeb),TextAnchor.MiddleRight,clearCard);
            string[] captions={"CLEAR TIME","MAX COMBO","HITS TAKEN"};
            var icons=new[]{DungeonResultInk.Shape.Clock,DungeonResultInk.Shape.Chain,DungeonResultInk.Shape.Guard};
            for(int i=0;i<clearLines.Length;i++){
                var line=clearLines[i]=new ResultLine();
                line.y=110-i*34;
                line.rect=UIRect(captions[i],clearCard,new Rect(12,line.y,273,33));
                line.group=line.rect.gameObject.AddComponent<CanvasGroup>();
                ResultInk(captions[i]+" icon",new Rect(2,12,14,14),line.rect,icons[i]);
                Label(captions[i]+" label",captions[i],new Rect(23,9,87,21),12,C(0xf2ebdf),TextAnchor.MiddleLeft,line.rect);
                line.value=Label(captions[i]+" value","",new Rect(108,9,62,21),13,Color.white,TextAnchor.MiddleRight,line.rect);
                line.value.resizeTextForBestFit=true;line.value.resizeTextMinSize=9;line.value.resizeTextMaxSize=13;
                line.points=Label(captions[i]+" points","",new Rect(180,9,54,21),11,C(0xded3be),TextAnchor.MiddleRight,line.rect);
                Panel(captions[i]+" grade plate",new Rect(240,8,34,21),C(0x343248),line.rect);
                line.grade=Label(captions[i]+" grade","",new Rect(240,8,34,21),14,Color.white,TextAnchor.MiddleCenter,line.rect);
                Panel(captions[i]+" Track",new Rect(23,4,251,3),C(0x374054),line.rect);
                line.meter=Panel(captions[i]+" Fill",new Rect(23,4,0,3),C(0x89c9ca),line.rect).rectTransform;
            }
            clearBest=Label("Clear record","NEW PERSONAL BEST",new Rect(15,8,277,27),14,C(0xffcf72),TextAnchor.MiddleLeft,clearCard);
            rankStamp=UIRect("Rank reveal",clearCard,new Rect(374,77,0,0));
            rankFade=rankStamp.gameObject.AddComponent<CanvasGroup>();
            rankEmblem=ResultInk("Rank crest",new Rect(-78,-52,156,106),rankStamp,DungeonResultInk.Shape.Crest);
            Label("Clear rank label","FINAL RANK",new Rect(-76,52,152,18),13,C(0xfde9c8),TextAnchor.MiddleCenter,rankStamp);
            clearOverall=Label("Overall points","",new Rect(-77,-72,154,21),16,C(0xffdd92),TextAnchor.MiddleCenter,rankStamp);
        }
        void HideDuringClear(Transform view) {
            if(!view||!view.gameObject.activeSelf)return;
            restoreAfterClear.Add(view.gameObject);
            view.gameObject.SetActive(false);
        }
        void RenderClearCard(DungeonClearResult score) {
            if(score==null){
                if(shownClear!=null){
                    foreach(var view in restoreAfterClear)if(view)view.SetActive(true);
                    restoreAfterClear.Clear();
                }
                shownClear=null;
                if(clearCard && clearCard.gameObject.activeSelf)clearCard.gameObject.SetActive(false);
                return;
            }
            if(!clearCard)BuildClearCard();
            if(clearTimerScore!=score){
                clearTimerScore=score;
                clearStart=Time.unscaledTime;
                clearSecondsLeft=-1;
                clearRankCuePlayed=false;
            }
            float age=Time.unscaledTime-clearStart;
            if(shownClear!=score){
                foreach(string name in coveredDuringClear)HideDuringClear(safeRoot.Find(name));
                foreach(Transform control in touchHUD)if(control!=battleStick.transform)HideDuringClear(control);
                shownClear=score;clearSettled=false;
                clearReturnLabel.text=KeyboardMode?"MAP [ENTER]":"MAP  >";
                clearReturnLabel.fontSize=KeyboardMode?10:12;
                clearDungeon.text=DisplayDungeonTitleR1111();
                clearLines[0].metric=score.TimeCentiseconds;
                clearLines[1].metric=score.MaxCombo;
                clearLines[2].metric=score.HitsTaken;
                clearLines[0].score=score.TimeScore;clearLines[1].score=score.ComboScore;clearLines[2].score=score.DefenseScore;
                foreach(var line in clearLines){
                    line.lastPoints=-1;
                    line.lastMetric=-1;
                    line.grade.text=DungeonClearResult.GradeForScore(line.score);
                    line.grade.color=DungeonResultInk.RankColor(line.grade.text);
                }
                rankEmblem.Draw(DungeonResultInk.Shape.Crest,score.Grade);
                clearOverall.text=score.TotalScore+" / 100";
                clearCard.SetAsLastSibling();clearCard.gameObject.SetActive(true);
            }
            int secondsLeft=Mathf.Max(0,Mathf.CeilToInt(ClearReturnSeconds-age));
            if(secondsLeft!=clearSecondsLeft){
                clearSecondsLeft=secondsLeft;
                clearCountdown.text="AUTO RETURN "+secondsLeft+"s";
            }
            if(clearSettled)return;
            float arrival=Mathf.Clamp01(age/.28f);
            clearCard.anchoredPosition=new Vector2(488+Mathf.Round(40*(1-arrival)*(1-arrival)),329);
            clearFade.alpha=arrival;
            for(int i=0;i<clearLines.Length;i++){
                var line=clearLines[i];
                float t=Mathf.Clamp01((age-.24f-i*.40f)/.35f),ease=1-(1-t)*(1-t);
                line.group.alpha=t;line.rect.anchoredPosition=new Vector2(12-Mathf.Round(14*(1-ease)),line.y);
                float count=Mathf.Ceil(t*7)/7;
                line.meter.sizeDelta=new Vector2(Mathf.Round(2.51f*line.score*count),3);
                int points=Mathf.RoundToInt(line.score*count),metric=Mathf.RoundToInt(line.metric*count);
                if(t>0&&(points!=line.lastPoints||metric!=line.lastMetric)){
                    bool hadValue=line.lastPoints>=0;
                    line.points.text=points+"/100";line.lastPoints=points;
                    line.value.text=i==0?DungeonClearResult.FormatTime(metric):metric.ToString();
                    line.lastMetric=metric;
                    if(hadValue||t<1||age<.24f+i*.40f+.35f+.10f)PlayResultCountSound();
                }
                line.grade.enabled=t>=1;
            }
            float stamp=Mathf.Clamp01((age-1.58f)/.26f);
            rankFade.alpha=Mathf.Min(1,stamp*2);
            // An oversize first impression hits the plate then settles back into place.
            float scale=stamp<.65f?Mathf.Lerp(1.40f,.93f,stamp/.65f):Mathf.Lerp(.93f,1,(stamp-.65f)/.35f);
            rankStamp.localScale=new Vector3(scale,scale,1);
            clearBest.enabled=stamp>=.65f&&NewDungeonBest;
            if(stamp>0&&!clearRankCuePlayed){clearRankCuePlayed=true;if(stamp<1)PlayResultCountSound();}
            clearSettled=stamp>=1;
        }

        void BuildCommandGroups(Transform guide) {
            string[] commands = game.CommandGuide;
            var groups = new[]{
                new {title="GROUND", x=28, y=265, start=0, count=5},
                new {title="DASH", x=492, y=265, start=5, count=4},
                new {title="AIR", x=28, y=76, start=9, count=5},
                new {title="HOLD & RELEASE", x=492, y=76, start=14, count=2}
            };
            foreach (var group in groups) {
                var card = Panel("Commands "+group.title, new Rect(group.x,group.y-56,440,177), C(0x172e3e), guide).transform;
                Label(group.title+" heading",group.title,new Rect(14,143,410,24),14,C(0xffd88b),TextAnchor.MiddleLeft,card);
                for(int i=0;i<group.count;i++) {
                    var parts=commands[group.start+i].Split(new[]{" / "},System.StringSplitOptions.None);
                    float y=115-i*26;
                    Label(group.title+" input "+i,parts[0].Trim(),new Rect(14,y,176,24),14,C(0xeef5f5),TextAnchor.MiddleLeft,card);
                    if(parts.Length>1)
                        Label(group.title+" move "+i,parts[1].Trim(),new Rect(192,y,234,24),13,C(0xaec8d0),TextAnchor.MiddleLeft,card);
                }
            }
        }
    }
}
