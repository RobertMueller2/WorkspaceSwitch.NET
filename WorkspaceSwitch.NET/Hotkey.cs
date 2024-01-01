using System;
using System.Collections.Generic;
using Win32HotkeyListener;

namespace WorkspaceSwitcher {

    /// <summary>
    /// Extends the abstract BaseHotkey class with a concrete implementation.
    /// </summary>
    internal class Hotkey : BaseHotkey {

        internal int? Target { get; set; }

        internal Hotkey(List<string> modifiers, string keyString) {
            KeyCombo = new KeyCombination() { Key = new Key() { KeyStr = keyString } , Modifiers = modifiers };
        }

        public new BaseAction Action {
            get => base.Action;
            set {
                if (value is Actions.VirtualDesktopAction action) {
                    action.Hotkey = this;
                }
                base.Action = value;
            }
        }

    }
}