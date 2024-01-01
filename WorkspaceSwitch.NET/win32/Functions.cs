using System;
using System.Runtime.InteropServices;

namespace WorkspaceSwitcher.win32 {
    internal static class Functions {

        /// <summary>
        /// Unmanaged function to get the foreground window.
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
    }
}
