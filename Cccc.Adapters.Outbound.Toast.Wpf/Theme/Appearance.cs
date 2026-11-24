using System;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal sealed class Appearance
    {
        internal bool Dark, Transparent;
        internal Color Fill { get { return Dark ? Color.FromRgb(40, 40, 43) : Color.FromRgb(255, 255, 255); } }
        internal double Tint { get { return Dark ? .77 : .80; } }
        // User-selected taskbar surface; the ordinary card keeps Fill/Tint.
        internal Color TaskbarFill { get { return Dark ? Colors.White : Color.FromRgb(240, 240, 240); } }
        internal double TaskbarTint { get { return Dark ? .06 : .80; } }
        internal double TaskbarShadow { get { return Dark ? 0 : .0235; } }
        internal string Text { get { return Dark ? "#F5F5F7" : "#1D1D1F"; } }
        internal string Detail { get { return Dark ? "#AEAEB2" : "#6E6E73"; } }
        internal string Rim { get { return Dark ? "#636366" : "#C7C7CC"; } }
        internal string Badge { get { return Dark ? "#86DDBA" : "#15976B"; } }
        internal string Green { get { return Dark ? "#244A3B" : "#DDF6EB"; } }
        internal string Counter { get { return Dark ? "#3A3A3C" : "#E9E9ED"; } }
        internal string CountText { get { return Text; } }
    }
}
