using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        const string MusicVolumeKey = "ElswordPixel.MusicVolume";
        const string EffectsVolumeKey = "ElswordPixel.EffectsVolume";
        const string KeyboardModeKey = "ElswordPixel.KeyboardMode";
        const string ResolutionKey="ElswordPixel.RenderResolution";
        const string FrameRateDisplayKey="ElswordPixel.ShowFrameRate";
        enum MobileResolution { Auto, Saver, Native }
        MobileResolution resolutionMode;
        readonly Image[] resolutionChoices=new Image[3];
        bool mobileDisplayReady;
        bool smallerMobileScenery, mobileSceneryTargetDirty;
        bool ShowResolutionSettings {get{return Application.platform==RuntimePlatform.Android;}}
        static readonly string[] TouchSlotLabels = { "1", "2", "3", "4", "5", "6", "7", "8" };

        float musicVolume = 1, effectsVolume = 1;
        public bool KeyboardMode { get; private set; }
        bool settingsOpen, pausedBeforeSettings;
        Image settingsShade, touchChoice, keyboardChoice;
        Slider musicSlider, effectsSlider;
        Text musicPercent, effectsPercent, masterLabel, frameRateChoice;

        void InitializePlayerSettings() {
            musicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicVolumeKey, 1));
            effectsVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(EffectsVolumeKey, 1));
            KeyboardMode = PlayerPrefs.GetInt(KeyboardModeKey, 0) == 1;
            showPerformance=PlayerPrefs.GetInt(FrameRateDisplayKey,0)!=0;
            if(ShowResolutionSettings){
                resolutionMode=(MobileResolution)Mathf.Clamp(PlayerPrefs.GetInt(ResolutionKey,0),0,2);
                mobileDisplayReady=true;ApplyMobileResolution();
            }
            ApplyInputMode();

            // Authored scenes include the former TOUCH toggle. Rebind it as well as runtime-built HUDs.
            if (images.TryGetValue("Touch Toggle", out var entry)) {
                entry.GetComponent<TouchControl>().action = "settings";
                entry.rectTransform.anchoredPosition = new Vector2(862, 416);
                entry.rectTransform.sizeDelta = new Vector2(86, 40);
                var title = labels["Touch Toggle Text"];
                title.rectTransform.sizeDelta = entry.rectTransform.sizeDelta;
                title.fontSize = 10;
                title.text = "SETTINGS";
            }
            if (images.TryGetValue("Sound Button", out var sound)) {
                sound.rectTransform.anchoredPosition = new Vector2(762, 416);
                sound.rectTransform.sizeDelta = new Vector2(94, 40);
                labels["Sound Button Text"].rectTransform.sizeDelta = sound.rectTransform.sizeDelta;
            }
        }

        void ApplyMobileResolution(){
            if(!mobileDisplayReady)return;
            // Read native dimensions so switching back from Saver restores the full display.
            int width=Display.main.systemWidth,height=Display.main.systemHeight;
            if(width<=0||height<=0)return;
            if((Screen.width>=Screen.height)!=(width>=height)){int old=width;width=height;height=old;}
            int shortEdge=Mathf.Min(width,height);
            bool lowPower=SystemInfo.processorCount<=4||(SystemInfo.systemMemorySize>0&&SystemInfo.systemMemorySize<=3072);
            int topCap=lowPower?540:720;
            autoFrameBudget.Configure(topCap);
            int cap=resolutionMode==MobileResolution.Native?shortEdge:
                resolutionMode==MobileResolution.Saver?432:autoFrameBudget.ShortEdge;
            bool smaller=resolutionMode==MobileResolution.Saver||(resolutionMode==MobileResolution.Auto&&cap<=540);
            if(smallerMobileScenery!=smaller){smallerMobileScenery=smaller;mobileSceneryTargetDirty=true;}
            float scale=Mathf.Min(1f,(float)cap/shortEdge);
            int targetWidth=Mathf.RoundToInt(width*scale),targetHeight=Mathf.RoundToInt(height*scale);
            if(Screen.width!=targetWidth||Screen.height!=targetHeight)
                Screen.SetResolution(targetWidth,targetHeight,Screen.fullScreenMode);
        }

        void ApplyInputMode() {
            showTouchControls = !KeyboardMode;
            var events = EventSystem.current ? EventSystem.current : FindFirstObjectByType<EventSystem>();
            if (events) {
                events.SetSelectedGameObject(null);
                // Android sends keys to the active game window, including the settings sliders.
                events.sendNavigationEvents = KeyboardMode && !AndroidKeyboard;
            }
        }

        bool CancelLoadingWithKeyboard() {
            return HardwareKeyboardEnabled && (HardwareDown(KeyCode.Escape) || HardwareDown(KeyCode.Tab));
        }

        bool HandleSettingsAction(string action) {
            if (!settingsOpen) {
                if (action != "settings" && action != "touch") return false;
                OpenSettings();
            } else if (action == "settings-close") {
                CloseSettings();
            } else if (action == "settings-touch" || action == "settings-keyboard") {
                ClearInput();
                KeyboardMode = action == "settings-keyboard";
                PlayerPrefs.SetInt(KeyboardModeKey, KeyboardMode ? 1 : 0);
                ApplyInputMode();
                RefreshBattleControls();
                RefreshSettings();
            } else if (action=="settings-fps") {
                SetFrameRateDisplay(!showPerformance);
                RefreshSettings();
            } else if (action.StartsWith("settings-resolution-") && ShowResolutionSettings) {
                int mode;
                if(int.TryParse(action.Substring("settings-resolution-".Length),out mode)&&mode>=0&&mode<resolutionChoices.Length){
                    ClearInput();resolutionMode=(MobileResolution)mode;
                    autoFrameBudget.Reset();
                    PlayerPrefs.SetInt(ResolutionKey,mode);
                    ApplyMobileResolution();RefreshSettings();
                }
            } else if (action == "settings-sound") {
                ToggleAudio();
                RefreshSettings();
            }
            return true;
        }

        void OpenSettings() {
            if (!settingsShade) BuildSettingsWindow();
            settingsOpen = true;
            pausedBeforeSettings = game.paused;
            game.paused = true;
            ClearInput();
            settingsShade.transform.SetAsLastSibling();
            settingsShade.gameObject.SetActive(true);
            RenderResultSheet(false);
            RefreshSettings();
            if (CargoProductSessionActiveR1288) TickCargoProductDirectorAudioR1296();
            if (plainMineMajorViewR1302 != null) plainMineMajorViewR1302.SetPausedR1302(true);
        }

        void CloseSettings() {
            if (!settingsOpen) return;
            settingsOpen = false;
            settingsShade.gameObject.SetActive(false);
            ClearInput();
            game.paused = pausedBeforeSettings;
            accumulator = 0;
            PlayerPrefs.Save();
            if (!string.IsNullOrEmpty(openMenu)) RenderMenus();
        }

        void SuspendSettings() {
            if (!settingsOpen) return;
            pausedBeforeSettings = true;
            PlayerPrefs.Save();
        }

        void BuildSettingsWindow() {
            settingsShade = Panel("Settings Shade", new Rect(0, 0, 960, 540), Color.clear);
            settingsShade.raycastTarget = true;
            AddMenuBackdrop(settingsShade.transform,C(0x09141e,.82f));
            int extra=ShowResolutionSettings?58:0;
            var card = Panel("Settings Window", new Rect(156, 84-extra/2, 648, 372+extra), C(0x20354a), settingsShade.transform).transform;
            Label("Settings title", "SETTINGS", new Rect(30, 305+extra, 316, 50), 25, C(0xffe1a3), TextAnchor.MiddleLeft, card);
            Button("Settings master sound", "", new Rect(444, 304+extra, 174, 48), "settings-sound", 14, card);
            masterLabel = card.Find("Settings master sound/Settings master sound Text").GetComponent<Text>();
            Panel("Settings Divider", new Rect(30, 289+extra, 588, 2), C(0x53718D), card);
            musicSlider = BuildVolumeSlider(card, "Music", "MUSIC", 225+extra, C(0x62CEEF), out musicPercent);
            effectsSlider = BuildVolumeSlider(card, "Effects", "EFFECTS", 165+extra, C(0x78D5B2), out effectsPercent);
            musicSlider.onValueChanged.AddListener(value => {
                musicVolume = value / 100f;
                PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
                musicPercent.text = value.ToString("0") + "%";
            });
            effectsSlider.onValueChanged.AddListener(value => {
                effectsVolume = value / 100f;
                PlayerPrefs.SetFloat(EffectsVolumeKey, effectsVolume);
                effectsPercent.text = value.ToString("0") + "%";
                ApplyEffectsMix();
                if (CargoProductSessionActiveR1288) TickCargoProductDirectorAudioR1296();
                if (plainMineMajorViewR1302 != null) plainMineMajorViewR1302.SetEffectGain(effectsVolume);
            });
            Label("Settings input", "CONTROLS", new Rect(30, 95+extra, 160, 48), 15, C(0xEDF4FF), TextAnchor.MiddleLeft, card);
            Button("Settings touch choice", "TOUCH", new Rect(215, 95+extra, 193, 48), "settings-touch", 15, card);
            Button("Settings keyboard choice", "KEYBOARD", new Rect(417, 95+extra, 201, 48), "settings-keyboard", 15, card);
            if(ShowResolutionSettings){
                Label("Settings resolution","RESOLUTION",new Rect(30,95,170,48),15,C(0xEDF4FF),TextAnchor.MiddleLeft,card);
                string[] names={"AUTO","SAVER","NATIVE"};
                for(int i=0;i<names.Length;i++){
                    string id="Resolution "+names[i];
                    Button(id,names[i],new Rect(215+i*137,95,129,48),"settings-resolution-"+i,14,card);
                    resolutionChoices[i]=card.Find(id).GetComponent<Image>();
                }
            }
            touchChoice = card.Find("Settings touch choice").GetComponent<Image>();
            keyboardChoice = card.Find("Settings keyboard choice").GetComponent<Image>();
            Button("Settings frame rate", "", new Rect(30,24,170,48), "settings-fps", 14, card);
            frameRateChoice=card.Find("Settings frame rate/Settings frame rate Text").GetComponent<Text>();
            Button("Settings done", "DONE", new Rect(417,24,201,48), "settings-close", 16, card);
            card.Find("Settings done").GetComponent<Image>().color=C(0x39746e);
        }

        Slider BuildVolumeSlider(Transform card, string id, string caption, int y, Color accent, out Text percent) {
            Label(id + " label", caption, new Rect(30, y, 158, 48), 15, C(0xEDF4FF), TextAnchor.MiddleLeft, card);
            percent = Label(id + " value", "", new Rect(537, y, 81, 48), 15, accent, TextAnchor.MiddleRight, card);
            var area = Panel(id + " Slider Track", new Rect(202, y, 329, 48), Color.clear, card);
            area.raycastTarget = true;
            // Fill and handle share one inset so tapping a position selects the displayed value.
            var track = Panel(id + " Track", new Rect(12, 19, 305, 10), C(0x101e31), area.transform);
            var fill = Panel(id + " Fill", new Rect(0, 0, 0, 0), accent, track.transform);
            var handle = Panel(id + " Handle", new Rect(0, -7, 20, 24), C(0xFFF1CF), track.transform);
            handle.rectTransform.pivot = new Vector2(.5f, .5f);
            handle.rectTransform.offsetMin = new Vector2(-10, -7);
            handle.rectTransform.offsetMax = new Vector2(10, 7);
            var slider = area.gameObject.AddComponent<Slider>();
            slider.maxValue = 100;
            slider.wholeNumbers = true;
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            return slider;
        }

        void RefreshSettings() {
            musicSlider.SetValueWithoutNotify(Mathf.RoundToInt(musicVolume * 100));
            effectsSlider.SetValueWithoutNotify(Mathf.RoundToInt(effectsVolume * 100));
            musicPercent.text = musicSlider.value.ToString("0") + "%";
            effectsPercent.text = effectsSlider.value.ToString("0") + "%";
            masterLabel.text = audioMuted ? "SOUND OFF" : "SOUND ON";
            frameRateChoice.text=showPerformance?"FPS ON":"FPS OFF";
            var selected = C(0x407D8C);
            var idle = C(0x293E56);
            touchChoice.color = KeyboardMode ? idle : selected;
            keyboardChoice.color = KeyboardMode ? selected : idle;
            for(int i=0;i<resolutionChoices.Length;i++)
                if(resolutionChoices[i])resolutionChoices[i].color=(int)resolutionMode==i?selected:idle;
        }
    }
}
