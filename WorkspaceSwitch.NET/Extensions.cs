using System;
using VirtualDesktop;

static class Extensions {

    public static Int32 ToIdModifier(this Int32 value) => value & 0xF00;
    public static Int32 ToTarget(this Int32 value) => (value & ~0xF00) - 1;

    public static Int32 AdjustOverflow(this Int32 value) => value + ((value < 0) ? 10 : 0);

    public static void MoveActiveWindowAndSwitch(this Desktop v) {
        v.MoveActiveWindow();
        v.MakeVisible();
    }
}