using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;

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
                int hotkey = message.wParam.ToInt32();
                switch (hotkey & 0x700) {
                    case 0x100:
                        VirtualDesktop.Desktop.FromIndex(hotkey & ~0x700 -1).MakeVisible();
                        break;
                    case 0x200:
                        break;
                    case 0x400:
                        break;
                }

            }

        }

    }

}

