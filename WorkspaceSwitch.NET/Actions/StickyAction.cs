using System;
using System.Runtime.InteropServices;
using VirtualDesktop;
using Win32HotkeyListener;

using static WorkspaceSwitcher.win32.Functions;

namespace WorkspaceSwitcher.Actions {

    /// <summary>
    /// Action for Sticky Windows (PinWindow/UnpinWindow)
    /// </summary>
    internal class StickyAction : BaseAction {
        /// <summary>
        /// implements BaseAction.Execute()
        /// </summary>
        public override bool Execute() {
            IntPtr hwnd = GetForegroundWindow();
            
            if (hwnd == IntPtr.Zero) {
                return false;
            }
            try {
                if (Desktop.IsWindowPinned(hwnd)) {
                    Desktop.UnpinWindow(hwnd);
                }
                else {
                    Desktop.PinWindow(hwnd);
                }
            } catch (COMException e) {
                Logger.GetInstance().Log("Error: " + e.Message);
                return false;
            }
            return true;
        }
    }
}
