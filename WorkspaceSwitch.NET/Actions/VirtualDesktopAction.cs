using System;
using System.Runtime.InteropServices;
using VirtualDesktop;
using Win32HotkeyListener;

namespace WorkspaceSwitcher.Actions {

    /// <summary>
    /// Action for Virtual Desktops (MakeVisible/MoveActiveWindow/MoveActiveWindowAndSwitch)
    /// </summary>
    internal class VirtualDesktopAction : BaseAction {

        /// <summary>
        /// Represents the type of action to be performed on a virtual desktop
        /// </summary>
        internal enum DesktopActionType {
            MakeVisible,
            MoveActiveWindow,
            MoveActiveWindowAndSwitch,
        };

        /// <summary>
        /// The type of action to be performed on a virtual desktop
        /// </summary>
        internal DesktopActionType ActionType { get; set; }

        /// <summary>
        /// The hotkey that triggered this action. This is required because the hotkey holds the desktop number.
        /// </summary>
        internal Hotkey? Hotkey { get; set; }


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
                    return false;
                }
                var method = targetMethod(Desktop.FromIndex(target));

                try {
                    method?.Invoke();
                }
                catch (COMException e) {
                    Logger.GetInstance().Log("Error: " + e.Message);
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
