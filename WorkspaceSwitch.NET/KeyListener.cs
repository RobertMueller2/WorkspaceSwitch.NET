
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using VirtualDesktop;
using Win32HotkeyListener.Win32;

namespace WorkspaceSwitcher {
    static class KeyListener {
        [DllImport("user32.dll")]
        private static extern int GetMessageA(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static BackgroundWorker run() {

            var w = new BackgroundWorker();
            w.DoWork += new DoWorkEventHandler(Backgroundworker_DoWork);
            w.WorkerSupportsCancellation = true;
            w.RunWorkerAsync();
            return w;
        }

        public static void Backgroundworker_DoWork(object sender, DoWorkEventArgs e) {

            var hotkeys = RegisterHotkeys();

            MSG message;
            int rv;
            while ((rv = GetMessageA(out message, IntPtr.Zero, 0, 0)) != 0) {

                if (rv == -1) {
                    Console.WriteLine("Error returned");
                    continue;
                }

                //FIXME: WM_HOTKEY
                if (message.message != 786) {
                    continue;
                }

                System.Diagnostics.Debug.WriteLine("Message {0:X}, Hotkey: {1:X}", message.message, message.wParam.ToInt32());
                Int32 hotkey = message.wParam.ToInt32();

                try {
                    //FIXME: this could potentially be methods in Hotkey, but would require lookup
                    switch (hotkey.ToIdModifier()) {
                        case 0x100:
                            Desktop.FromIndex(hotkey.ToTarget().AdjustOverflow()).MakeVisible();
                            break;
                        case 0x200:
                            Desktop.FromIndex(hotkey.ToTarget().AdjustOverflow()).MoveActiveWindow();
                            break;
                        case 0x400:
                            Desktop.FromIndex(hotkey.ToTarget().AdjustOverflow()).MoveActiveWindowAndSwitch();
                            break;
                        case 0x800:
                            //FIXME: If there is only one hotkey, no further distinction is necessary (yet)
                            TogglePinnedStatus();
                            break;
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine("Exception occurred: {0}", ex.ToString());
                }

            }

        }

        public static List<User32Hotkey> RegisterHotkeys() {
            var hotkeys = new List<User32Hotkey>();
            var mod_switch = ((uint)(ModifierKeys.MOD_WIN | ModifierKeys.MOD_NOREPEAT));
            var mod_move = ((uint)(ModifierKeys.MOD_WIN | ModifierKeys.MOD_SHIFT |
                ModifierKeys.MOD_NOREPEAT));
            var mod_move_and_switch = ((uint)(ModifierKeys.MOD_WIN | ModifierKeys.MOD_SHIFT |
                ModifierKeys.MOD_CONTROL | ModifierKeys.MOD_NOREPEAT));

            for (uint i = 0; i < 10; i++) {
                uint keycode = 0x30 + i;
                hotkeys.Add(new User32Hotkey(0x100 + i, mod_switch, 0x30 + i));
                hotkeys.Add(new User32Hotkey(0x200 + i, mod_move, 0x30 + i));
                hotkeys.Add(new User32Hotkey(0x400 + i, mod_move_and_switch, 0x30 + i));
            }

            //bind mod+shift+t
            hotkeys.Add(new User32Hotkey(0x800, mod_move_and_switch, 0x54));

            return hotkeys;
        }

        public static void TogglePinnedStatus() {
            IntPtr hwnd = GetForegroundWindow();
            if (Desktop.IsWindowPinned(hwnd)) {
                Desktop.UnpinWindow(hwnd);
            }
            else {
                Desktop.PinWindow(hwnd);
            }
        }

    }
}