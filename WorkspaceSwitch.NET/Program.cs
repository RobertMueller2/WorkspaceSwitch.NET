using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Win32HotkeyListener;
using WorkspaceSwitcher.Actions;
using static WorkspaceSwitcher.Actions.VirtualDesktopAction;
using WorkspaceSwitcher.Gui;

namespace WorkspaceSwitcher {
    public static class Program {

        // allow command line output for GUI application
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        public static void Main(string[] args) {

            AttachConsole(ATTACH_PARENT_PROCESS);

            var MoveActiveWindowAndSwitchModifiersString = "CTRL,WIN,SHIFT";
            var MoveActiveWindowModifiersString = "WIN,SHIFT";
            var MakeVisibleModifiersString = "CTRL,WIN";
            var stickyKeyCombinationString = "CTRL,WIN,SHIFT+T";

            for (int i = 0; i < args.Length; i++) {

                switch (args[i]) {
                    case "-h":
                    case "--help":
                        ShowUsage();
                        return;

                    case "-a":
                    case "--switch-and-move-modifiers":
                        MoveActiveWindowAndSwitchModifiersString = GetNextArg(args, ref i);
                        break;
                    case "-m":
                    case "--move-window-modifiers":
                        MoveActiveWindowModifiersString = GetNextArg(args, ref i);
                        break;

                    case "-s":
                    case "--switch-desktop-modifiers":
                        MakeVisibleModifiersString = GetNextArg(args, ref i);
                        break;

                    case "-t":
                    case "--sticky-key-combination":
                    case "--sticky-key-combo":
                        stickyKeyCombinationString = GetNextArg(args, ref i);
                        break;

                    default:
                        ShowUsage();
                        return;
                }
            }

            if (stickyKeyCombinationString.Split('+').Length != 2) {
                Console.WriteLine($"Invalid sticky key combination: {stickyKeyCombinationString}");
                ShowUsage();
                return;
            }

            // is 6 modifiers enough? who knows...
            // safe according to devina.io
            // 2,10 is arbitrary, but should be a close enough guess for a modifier string
            Regex ModifierRegex = new Regex(@"^[a-zA-Z0-9]{2,10}((,[a-zA-Z0-9]{2,10}){0,4})$", RegexOptions.Compiled);

            foreach (var modifierString in new string[] { MoveActiveWindowAndSwitchModifiersString, MoveActiveWindowModifiersString, MakeVisibleModifiersString, stickyKeyCombinationString.Split('+')[0] }) {
                if (!ModifierRegex.IsMatch(modifierString)) {
                    Console.WriteLine($"Invalid modifier string: {modifierString}");
                    ShowUsage();
                    return;
                }
            }

            // all other checks, i.e. whether the individual tokens are valid, are done in KeyCombination and Hotkey

            List<Hotkey> hotkeys = new List<Hotkey>();

            var actions = Enum.GetValues(typeof(DesktopActionType)).Cast<DesktopActionType>().ToList();

            for (int i = 0; i < 10; i++) {
                hotkeys.Add(new Hotkey(MakeVisibleModifiersString.Split(',').ToList(), $"D{i}") { Target = i.AdjustOverflow().ToTarget(), Action = new VirtualDesktopAction() { ActionType = (DesktopActionType)Enum.Parse(typeof(DesktopActionType), "MakeVisible") } });
                hotkeys.Add(new Hotkey(MoveActiveWindowModifiersString.Split(',').ToList(), $"D{i}") { Target = i.AdjustOverflow().ToTarget(), Action = new VirtualDesktopAction() { ActionType = (DesktopActionType)Enum.Parse(typeof(DesktopActionType), "MoveActiveWindow") } });
                hotkeys.Add(new Hotkey(MoveActiveWindowAndSwitchModifiersString.Split(',').ToList(), $"D{i}") { Target = i.AdjustOverflow().ToTarget(), Action = new VirtualDesktopAction() { ActionType = (DesktopActionType)Enum.Parse(typeof(DesktopActionType), "MoveActiveWindowAndSwitch") } });
            }

            var stickyKeyCombo = stickyKeyCombinationString.Split('+'); // this should be ok due to the checks above
            hotkeys.Add(new Hotkey(stickyKeyCombo[0].Split(',').ToList(), stickyKeyCombo[1]) { Action = new StickyAction() }) ;

            // we don't expect any errors beyond here, so may as well remove the console output
            Console.SetOut(Logger.GetInstance());

            var hkl = new HotkeyListener(hotkeys);
            hkl.Run();
            var g = new MainWindow(hkl);
            
            Application.Run(g);
        }

        // TODO: Candidate for library
        private static string GetNextArg(string[] args, ref int i) {
            if (i + 1 >= args.Length) {
                throw new System.Exception("Missing argument");
            }
            return args[++i];
        }

        //TODO: Proper visualisation of modifiers and keys
        private static void ShowUsage() {
            Console.WriteLine("Usage: WorkspaceSwitcher [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("  -h, --help\t\t\tShow this help message and exit");
            Console.WriteLine("  -a, --switch-and-move-modifiers\tModifier keys for switching desktop and moving active window");
            Console.WriteLine("  -m, --move-window-modifiers\t\tModifier keys for moving active window");
            Console.WriteLine("  -s, --switch-desktop-modifiers\tModifier keys for switching desktop");
            Console.WriteLine("  -t, --sticky-key-combination\t\tKey combination for sticky mode");            
        }
    }

}

