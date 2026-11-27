using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UInput = UnityEngine.Input;

namespace ElswordPixel {
    public sealed partial class DemoScene {
        readonly KeyboardFocusState keyboardFocus = new KeyboardFocusState();
#if ROOT_ANDROID_KEYBOARD_PROBE
        static readonly Func<KeyCode, bool> PhysicalHeld = AndroidKeyboardProbe.Held;
        static readonly Func<KeyCode, bool> PhysicalDown = AndroidKeyboardProbe.Down;
        static readonly Func<KeyCode, bool> PhysicalUp = AndroidKeyboardProbe.Up;
#else
        static readonly Func<KeyCode, bool> PhysicalHeld = UInput.GetKey;
        static readonly Func<KeyCode, bool> PhysicalDown = UInput.GetKeyDown;
        static readonly Func<KeyCode, bool> PhysicalUp = UInput.GetKeyUp;
#endif
        static readonly KeyCode[] DungeonKeys = {
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow,
            KeyCode.Home, KeyCode.End, KeyCode.PageUp, KeyCode.PageDown, KeyCode.Return, KeyCode.KeypadEnter
        };
        static readonly KeyCode[] SceneKeys = {
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow,
            KeyCode.Home, KeyCode.End, KeyCode.PageUp, KeyCode.PageDown, KeyCode.Return, KeyCode.KeypadEnter,
            KeyCode.Escape, KeyCode.Tab, KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5,
            KeyCode.F12, KeyCode.M, KeyCode.K, KeyCode.LeftShift, KeyCode.RightShift, KeyCode.LeftControl,
            KeyCode.Z, KeyCode.X, KeyCode.T, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
            KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.C
        };
        bool AndroidKeyboard {
            get {
#if ROOT_ANDROID_KEYBOARD_PROBE
                return AndroidKeyboardProbe.SimulateAndroid;
#elif UNITY_ANDROID && !UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }
        bool HardwareKeyboardEnabled { get { return KeyboardMode || AndroidKeyboard; } }
        bool HardwareDown(KeyCode key) { return keyboardFocus.Accept(key) && PhysicalDown(key); }
        bool CombatDown(KeyCode key) {
            if (!HardwareDown(key)) return false;
            keyboardFocus.PressCombat(key);
            return true;
        }
        bool CombatHeld(KeyCode key) {
            return (!AndroidKeyboard || keyboardFocus.OwnsCombat(key)) && PhysicalHeld(key);
        }
        bool CombatUp(KeyCode key) {
            if (!PhysicalUp(key)) return false;
            bool owned = keyboardFocus.ReleaseCombat(key);
            return !AndroidKeyboard || owned;
        }
        bool ConfirmDown() { return HardwareDown(KeyCode.Return) || HardwareDown(KeyCode.KeypadEnter); }

        void HandleLoadingKeyboard() {
            if (!AndroidKeyboard || !CancelLoadingWithKeyboard()) return;
            if (mobileProductEntryRunning) mobileCancelEntry = true;
            else if (HardwareDown(KeyCode.Escape)) BackgroundAndroidTask();
        }

        void AndroidBack() {
            if (settingsOpen) { Action("settings-close"); return; }
            if (openMenu == "dungeons" && (!adventureStarted || game.victory || game.player.hp <= 0)) {
                BackgroundAndroidTask();
                return;
            }
            if (string.IsNullOrEmpty(openMenu) && (game.victory || game.player.hp <= 0)) {
                Action("result");
                return;
            }
            Action("pause");
        }

        static void BackgroundAndroidTask() {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity")) {
                activity.Call<bool>("moveTaskToBack", true);
            }
#endif
        }

        void Keyboard() {
            if (!HardwareKeyboardEnabled || !keyboardFocus.Active) return;
            if (HardwareDown(KeyCode.Escape)) {
                if (AndroidKeyboard) AndroidBack();
                else if (settingsOpen) Action("settings-close");
                else if (game.victory && string.IsNullOrEmpty(openMenu)) Action("result");
                else Action("pause");
                return;
            }
            if (settingsOpen) {
                if (AndroidKeyboard) {
                    if (HardwareDown(KeyCode.F2)) Action("settings-close");
                    else NavigateModal(settingsShade.rectTransform);
                }
                return;
            }
            if (HardwareDown(KeyCode.F2)) { Action("touch"); return; }
            if (HardwareDown(KeyCode.M)) Action("sound");
            if (HardwareDown(KeyCode.F3)) showCollisionReview = !showCollisionReview;
            if (HardwareDown(KeyCode.F4)) SetFrameRateDisplay(!showPerformance);
            if (HardwareDown(KeyCode.F12)) Capture("manual");
            if (HardwareDown(KeyCode.F5) && (!AndroidKeyboard || string.IsNullOrEmpty(openMenu))) { ResetGame(); return; }
            if (HardwareDown(KeyCode.K)) { Action("loadout"); return; }
            if (HardwareDown(KeyCode.F1)) { Action("guide"); return; }
            if (openMenu == "dungeons") {
                foreach (var key in DungeonKeys)
                    if (HardwareDown(key)) { DungeonListNavigation(key); return; }
                return;
            }
            if (!string.IsNullOrEmpty(openMenu)) {
                if (AndroidKeyboard && images.TryGetValue("Menu Root", out var menu)) NavigateModal(menu.rectTransform);
                else if (HardwareDown(KeyCode.Tab)) Action("dungeons");
                else if (ConfirmDown()) Action("menu-close");
                return;
            }
            if (HardwareDown(KeyCode.Tab) && (!AndroidKeyboard || !game.paused)) { Action("dungeons"); return; }
            if (game.victory && ConfirmDown()) { Action("result"); return; }
            if (game.player.hp <= 0) {
                if (ConfirmDown()) { if (AndroidKeyboard) Action("result"); else ResetGame(); }
                return;
            }
            if (game.paused) {
                if (AndroidKeyboard && resultSheet) NavigateModal(resultSheet, "result");
                return;
            }
            if (BossIntroductionActive) return;
            bool up = CombatDown(KeyCode.UpArrow), down = CombatDown(KeyCode.DownArrow);
            // Legacy hardware polling can report both presses in one render frame.
            // If only Up is still held, the roll ended on Up. Otherwise prefer the usual Up/Down tap.
            bool endsOnUp = up && down && PhysicalHeld(KeyCode.UpArrow) && !PhysicalHeld(KeyCode.DownArrow);
            if (endsOnUp) Action("drop");
            if (up) Action("jump");
            if (down && !endsOnUp) Action("drop");
            if (CombatDown(KeyCode.Z)) Action("attack");
            if (CombatDown(KeyCode.X)) Action("attackX");
            if (CombatUp(KeyCode.X)) pending.attackXReleased = true;
            if (CombatDown(KeyCode.LeftArrow)) DirectionPressed(-1);
            if (CombatDown(KeyCode.RightArrow)) DirectionPressed(1);
            bool leftShift = CombatDown(KeyCode.LeftShift), rightShift = CombatDown(KeyCode.RightShift);
            if (leftShift || rightShift) Action("dash");
            if (HardwareDown(KeyCode.T) || HardwareDown(KeyCode.Alpha3)) Action("potion3");
            if (HardwareDown(KeyCode.Alpha1)) Action("potion1");
            if (HardwareDown(KeyCode.Alpha2)) Action("potion2");
            if (HardwareDown(KeyCode.LeftControl)) Action("awaken");
            for (int slot = 0; slot < GameModel.SkillSlotCount; slot++) {
                if (CombatDown(SkillKeys[slot])) Action("skill" + slot);
                if (CombatUp(SkillKeys[slot])) pending.skillReleasedMask |= 1 << slot;
            }
        }

        RectTransform keySelection, keyOutline;
        readonly List<RectTransform> modalKeys = new List<RectTransform>();

        void ClearKeySelection() {
            keySelection = null;
            if (keyOutline) keyOutline.gameObject.SetActive(false);
        }

        void SelectKeyControl(RectTransform item) {
            keySelection = item;
            if (!keyOutline) {
                keyOutline = new GameObject("Keyboard focus", typeof(RectTransform)).GetComponent<RectTransform>();
                for (int i = 0; i < 4; i++) {
                    var image = new GameObject("Edge", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
                    image.raycastTarget = false;
                    image.color = C(0xffdf8f);
                    var edge = image.rectTransform;
                    edge.SetParent(keyOutline, false);
                    // Top, bottom, left, right. Draw inside the target and follow its size.
                    edge.anchorMin = new Vector2(i == 3 ? 1 : 0, i == 0 ? 1 : 0);
                    edge.anchorMax = new Vector2(i == 2 ? 0 : 1, i == 1 ? 0 : 1);
                    edge.offsetMin = new Vector2(i == 3 ? -2 : 0, i == 0 ? -2 : 0);
                    edge.offsetMax = new Vector2(i == 2 ? 2 : 0, i == 1 ? 2 : 0);
                }
            }
            keyOutline.SetParent(item, false);
            keyOutline.anchorMin = Vector2.zero; keyOutline.anchorMax = Vector2.one;
            keyOutline.offsetMin = keyOutline.offsetMax = Vector2.zero;
            keyOutline.gameObject.SetActive(true);
        }

        void NavigateModal(RectTransform root, string defaultAction = null) {
            if (keySelection && (!keySelection.gameObject.activeInHierarchy || !keySelection.IsChildOf(root))) ClearKeySelection();
            bool tab = HardwareDown(KeyCode.Tab);
            int x = (HardwareDown(KeyCode.RightArrow) ? 1 : 0) - (HardwareDown(KeyCode.LeftArrow) ? 1 : 0);
            int y = (HardwareDown(KeyCode.UpArrow) ? 1 : 0) - (HardwareDown(KeyCode.DownArrow) ? 1 : 0);
            if (!tab && x == 0 && y == 0) {
                if (!ConfirmDown()) return;
                var button = keySelection ? keySelection.GetComponent<TouchControl>() : null;
                if (button) Action(button.action);
                else if (!keySelection && defaultAction != null) Action(defaultAction);
                else if (!keySelection && !settingsOpen) Action("menu-close");
                return;
            }
            if (keySelection && x != 0 && keySelection.TryGetComponent<Slider>(out var slider)) {
                slider.value += x * 5;
                return;
            }
            modalKeys.Clear();
            foreach (var control in root.GetComponentsInChildren<RectTransform>()) {
                if (control.TryGetComponent<TouchControl>(out var button) && button.enabled ||
                    control.TryGetComponent<Slider>(out var volume) && volume.IsInteractable()) modalKeys.Add(control);
            }
            if (modalKeys.Count == 0) return;
            int current = modalKeys.IndexOf(keySelection);
            if (current < 0 || tab) {
                bool reverse = tab && (PhysicalHeld(KeyCode.LeftShift) || PhysicalHeld(KeyCode.RightShift));
                int next = current < 0 ? (reverse ? modalKeys.Count - 1 : 0) : (current + (reverse ? -1 : 1) + modalKeys.Count) % modalKeys.Count;
                SelectKeyControl(modalKeys[next]);
                return;
            }
            Vector2 dir = new Vector2(x, y).normalized;
            Vector2 origin = keySelection.TransformPoint(keySelection.rect.center);
            float best = float.MaxValue;
            RectTransform target = null;
            foreach (var item in modalKeys) {
                Vector2 center = item.TransformPoint(item.rect.center);
                Vector2 delta = center - origin;
                float forward = Vector2.Dot(delta, dir);
                if (forward <= .01f) continue;
                float cross = Mathf.Abs(delta.x * dir.y - delta.y * dir.x);
                float cost = forward + cross * 3;
                if (cost < best) { best = cost; target = item; }
            }
            if (target) SelectKeyControl(target);
        }
    }
}
