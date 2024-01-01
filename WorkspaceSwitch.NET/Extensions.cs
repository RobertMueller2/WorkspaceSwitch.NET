using System;
using VirtualDesktop;

static class Extensions {

    public static Int32 ToTarget(this Int32 value) => (value & ~0xF00) - 1;

    /// <summary>
    /// key 1 is Desktop 1 (index 0)
    /// ...
    /// key 0 has to be Desktop 10 (index 9)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Int32 AdjustOverflow(this Int32 value) => value + ((value % 10 == 0) ? 10 : 0);

    public static void MoveActiveWindowAndSwitch(this Desktop v) {
        v.MoveActiveWindow();
        v.MakeVisible();
    }
}