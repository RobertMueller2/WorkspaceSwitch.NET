using System;
using VirtualDesktop;
using Win32HotkeyListener;

namespace WorkspaceSwitcher.Actions {
    internal class VirtualDesktopAction : BaseAction {

        internal enum DesktopActionType {
            MakeVisible,
            MoveActiveWindow,
            MoveActiveWindowAndSwitch,
        };

        internal DesktopActionType ActionType { get; set; }

        internal Hotkey Hotkey { get; set; }

        /// <summary>
        /// Implements BaseAction.Execute()
        /// </summary>
        /// <returns></returns>
        public override bool Execute() {

            Func<Desktop, Action> targetMethod;

            switch (ActionType) {
                case DesktopActionType.MakeVisible:
                    targetMethod = d => d.MakeVisible;
                    break;
                case DesktopActionType.MoveActiveWindow:
                    targetMethod = d => d.MoveActiveWindow;
                    break;
                case DesktopActionType.MoveActiveWindowAndSwitch:
                    targetMethod = d => d.MoveActiveWindowAndSwitch;
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (Hotkey?.Target is int target) {
                if (target+1 > Desktop.Count) {
                    Logger.GetInstance().Log($"Desktop {target+1} does not exist.");
                }
                var method = targetMethod(Desktop.FromIndex(target));
                method?.Invoke();                
                return true;
            }
            return false;
        }
    }
}
