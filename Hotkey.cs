using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;


public class Hotkey : IDisposable {

    // Registers a hot key with Windows.
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    // Unregisters the hot key with Windows.
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public uint id { get; private set; }
    public bool initialised { get; private set; }

    public Hotkey(uint id, uint modifiers, uint keycode) {
        this.id = id;
        Console.WriteLine("Registered id {0:X}|modifier {1:X}|keycode {2:X}", id, modifiers, keycode);
        RegisterHotKey(IntPtr.Zero, checked((int)id), modifiers, keycode);
        initialised = true;
    }

    public void Dispose() {
        if (initialised) {
            UnregisterHotKey(IntPtr.Zero, checked((int)id));
        }
    }

}