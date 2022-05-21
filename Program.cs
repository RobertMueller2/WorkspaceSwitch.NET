using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using VirtualDesktop;

namespace WorkspaceSwitcher
{
    public static class Program
    {


        [DllImport("user32.dll")]
        private static extern int GetMessageA(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        public static void Main(string[] args)
        {

            var hotkeys = new List<Hotkey>();
            var mod_switch = ((uint) (ModifierKeys.MOD_WIN | ModifierKeys.MOD_NOREPEAT));
            var mod_move = ((uint) (ModifierKeys.MOD_WIN | ModifierKeys.MOD_SHIFT | 
                ModifierKeys.MOD_NOREPEAT));
            var mod_move_and_switch = ((uint) (ModifierKeys.MOD_WIN | ModifierKeys.MOD_SHIFT |
                ModifierKeys.MOD_CONTROL | ModifierKeys.MOD_NOREPEAT));

            for (uint i=0; i<10; i++) {
                uint keycode = 0x30 + i;
                hotkeys.Add(new Hotkey(0x100+i, mod_switch, 0x30+i));
                hotkeys.Add(new Hotkey(0x200+i, mod_move, 0x30+i));
                hotkeys.Add(new Hotkey(0x400+i, mod_move_and_switch, 0x30+i));
            }

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

                Debug.WriteLine("Message {0:X}, Hotkey: {1:X}", message.message, message.wParam.ToInt32());
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
                    }
                } catch (Exception e) {
                    Console.WriteLine("Exception occurred: {0}", e.ToString());
                }

                //FIXME: intercept process end, Ctrl+C etc to unregister hotkeys

            }

        }

    }

}

