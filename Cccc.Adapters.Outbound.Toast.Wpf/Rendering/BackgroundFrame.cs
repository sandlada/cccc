using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    internal sealed class BackgroundFrame
    {
        internal BitmapSource Bitmap;
        internal BitmapSource LeftSlice, MiddleSlice, RightSlice;
        internal int LeftSliceWidth, RightSliceWidth;
        internal Geometry Outline;
        internal int Dpi, Width, Height;
        internal int LogicalWidth, LogicalHeight;
        internal double CornerExtent;
        internal bool ShowBorder, ShowShadow;
        internal bool Dark, Acrylic, Compact, TaskbarBlend;
        internal double BuildMilliseconds;
        internal int HighBufferBytes;
    }
}
