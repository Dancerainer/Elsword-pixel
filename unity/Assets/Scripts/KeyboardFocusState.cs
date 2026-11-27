using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElswordPixel {
    // Android can resume with a key still down, or with only its release event.
    // A combat release belongs to the view which received the original press.
    internal sealed class KeyboardFocusState {
        bool focused = true, paused, captureOnResume;
        readonly List<KeyCode> suppressed = new List<KeyCode>();
        readonly HashSet<KeyCode> combatPresses = new HashSet<KeyCode>();
        internal bool Active { get { return focused && !paused; } }

        internal void SetFocus(bool value) { SetLifecycle(value, paused); }
        internal void SetPaused(bool value) { SetLifecycle(focused, value); }

        void SetLifecycle(bool nextFocus, bool nextPause) {
            bool wasActive = Active;
            focused = nextFocus;
            paused = nextPause;
            if (wasActive && !Active) {
                combatPresses.Clear();
                captureOnResume = true;
            }
        }

        internal void BeginFrame(KeyCode[] keys, Func<KeyCode, bool> held, Func<KeyCode, bool> pressed) {
            if (!Active) return;
            if (captureOnResume) {
                suppressed.Clear();
                foreach (var key in keys)
                    if (held(key) || pressed(key)) suppressed.Add(key);
                captureOnResume = false;
            } else {
                for (int i = suppressed.Count - 1; i >= 0; --i)
                    if (!held(suppressed[i]) || pressed(suppressed[i])) suppressed.RemoveAt(i);
            }
        }

        internal bool Accept(KeyCode key) { return Active && !suppressed.Contains(key); }
        internal void ClearCombat() { combatPresses.Clear(); }
        internal void PressCombat(KeyCode key) { if (Accept(key)) combatPresses.Add(key); }
        internal bool OwnsCombat(KeyCode key) { return Active && combatPresses.Contains(key); }
        internal bool ReleaseCombat(KeyCode key) { return combatPresses.Remove(key) && Accept(key); }
    }
}
