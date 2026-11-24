using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Windows.Graphics;
using Windows.Graphics.Effects;
using Windows.UI.Composition;
using Windows.UI.Composition.Desktop;
using WinRT;

using Cccc.Platform.Windows;

namespace Cccc.Adapters.Outbound.Toast.Wpf
{
    // Public Windows composition APIs only. No screen capture, no downloaded
    // graphics runtime. A pure composition owner stays permanently behind WPF.
    internal sealed class AcrylicHost : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)] private struct QueueOptions { internal int Size, Thread, Apartment; }
        [DllImport("CoreMessaging.dll")] private static extern int CreateDispatcherQueueController(QueueOptions options, out IntPtr controller);
        [ComImport, Guid("29E691FA-4567-4DCA-B319-D0F207EB6807"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDesktopInterop
        {
            void CreateDesktopWindowTarget(IntPtr windowHandle, [MarshalAs(UnmanagedType.Bool)] bool topmost, out IntPtr target);
            void EnsureOnThread(uint thread);
        }
        private static IntPtr queue;
        private readonly CompositionWindow source;
        private readonly IntPtr overlay;
        private readonly Compositor compositor;
        private readonly DesktopWindowTarget target;
        private readonly Windows.UI.Composition.ContainerVisual root;
        private readonly SpriteVisual visual, backgroundVisual;
        private readonly CompositionBitmap background;
        private readonly CompositionBackdropBrush backdrop;
        private CompositionEffectBrush blurBrush;
        private CompositionEffectFactory blurFactory;
        private CompositionPathGeometry clipPath;
        private CompositionGeometricClip clip;
        private GeometrySource geometry;
        private int width, height, dpi;
        private int left = Int32.MinValue, top = Int32.MinValue;
        private double lastScale = Double.NaN, lastOpacity = 0;
        private bool disposed;
        internal IntPtr Handle { get { return source.Handle; } }
        internal bool Visible { get; private set; }

        internal AcrylicHost(IntPtr overlay)
        {
            this.overlay = overlay;
            if (queue == IntPtr.Zero && Windows.System.DispatcherQueue.GetForCurrentThread() == null)
            {
                int result = CreateDispatcherQueueController(new QueueOptions { Size = 12, Thread = 2, Apartment = 2 }, out queue);
                Marshal.ThrowExceptionForHR(result);
            }
            CompositionWindow created = null;
            try
            {
                created = new CompositionWindow();
                int enabled = 1;
                Marshal.ThrowExceptionForHR(WindowsNativeMethods.DwmSetWindowAttribute(created.Handle, 17, ref enabled, 4));
                compositor = new Compositor();
                IntPtr targetPtr;
                compositor.As<IDesktopInterop>().CreateDesktopWindowTarget(created.Handle, true, out targetPtr);
                target = MarshalInspectable<DesktopWindowTarget>.FromAbi(targetPtr);
                root = compositor.CreateContainerVisual();
                root.Opacity = 0;
                visual = compositor.CreateSpriteVisual();
                backgroundVisual = compositor.CreateSpriteVisual();
                background = new CompositionBitmap(compositor);
                backgroundVisual.Brush = background.Brush;
                root.Children.InsertAtBottom(visual);
                root.Children.InsertAtTop(backgroundVisual);
                backdrop = compositor.CreateHostBackdropBrush();
                target.Root = root;
                source = created;
                // Owned windows are always above their owner. This prevents the
                // material from ever sampling or covering the tinted text HWND.
                if (overlay != IntPtr.Zero) WindowsNativeMethods.SetWindowOwner(overlay, created.Handle);
            }
            catch
            {
                if (created != null && overlay != IntPtr.Zero && WindowsNativeMethods.IsWindow(overlay) && WindowsNativeMethods.GetWindow(overlay, 4) == created.Handle)
                    WindowsNativeMethods.SetWindowOwner(overlay, IntPtr.Zero);
                if (target != null) target.Root = null;
                if (backgroundVisual != null) { backgroundVisual.Brush = null; backgroundVisual.Dispose(); }
                if (background != null) background.Dispose();
                if (visual != null) visual.Dispose(); if (root != null) root.Dispose();
                if (target != null) target.Dispose();
                if (backdrop != null) backdrop.Dispose();
                if (compositor != null) compositor.Dispose(); if (created != null) created.Dispose(); throw;
            }
        }

        internal void Position(int x, int y, int pixelWidth, int pixelHeight, int targetDpi)
        {
            if (disposed) return;
            bool move = left != x || top != y, resize = width != pixelWidth || height != pixelHeight;
            if (width != pixelWidth || height != pixelHeight || dpi != targetDpi)
            {
                double scale = targetDpi / 96.0;
                GeometrySource next = null;
                CompositionPathGeometry nextPath = null;
                CompositionGeometricClip nextClip = null;
                CompositionEffectFactory nextFactory = null;
                CompositionEffectBrush nextBrush = null;
                try
                {
                    next = GeometrySource.Create(CardOutline.Create(new Rect(32 * scale + .5, 32 * scale + .5,
                        pixelWidth - 64 * scale - 1, pixelHeight - 64 * scale - 1), scale));
                    nextPath = compositor.CreatePathGeometry(new CompositionPath(next));
                    nextClip = compositor.CreateGeometricClip(nextPath);
                    nextFactory = compositor.CreateEffectFactory(new GaussianBlurEffect {
                        Radius = (float)(6 * scale), Source = new CompositionEffectSourceParameter("backdrop") });
                    nextBrush = nextFactory.CreateBrush();
                    nextBrush.SetSourceParameter("backdrop", backdrop);
                    visual.Clip = nextClip; visual.Brush = nextBrush;
                }
                catch
                {
                    visual.Clip = clip; visual.Brush = blurBrush;
                    if (nextBrush != null) nextBrush.Dispose(); if (nextFactory != null) nextFactory.Dispose();
                    if (nextClip != null) nextClip.Dispose(); if (nextPath != null) nextPath.Dispose();
                    if (next != null) next.Dispose(); throw;
                }
                // The old clip/path/native geometry used to await finalization on
                // every DPI replacement. Detach and dispose them together now.
                CompositionGeometricClip oldClip = clip; CompositionPathGeometry oldPath = clipPath;
                GeometrySource oldGeometry = geometry;
                CompositionEffectBrush oldBrush = blurBrush; CompositionEffectFactory oldFactory = blurFactory;
                clip = nextClip; clipPath = nextPath; geometry = next;
                blurBrush = nextBrush; blurFactory = nextFactory;
                if (oldBrush != null) oldBrush.Dispose(); if (oldFactory != null) oldFactory.Dispose();
                if (oldClip != null) oldClip.Dispose(); if (oldPath != null) oldPath.Dispose();
                if (oldGeometry != null) oldGeometry.Dispose();
                width = pixelWidth; height = pixelHeight; dpi = targetDpi;
                root.Size = visual.Size = backgroundVisual.Size = new Vector2(width, height);
                root.CenterPoint = new Vector3(width / 2f, height / 2f, 0);
            }
            if (move || resize)
            {
                WindowsNativeMethods.SetWindowPos(Handle, IntPtr.Zero, x, y, width, height,
                    0x0010 | 0x0200 | 0x0004 | (move ? 0u : 0x0002u) | (resize ? 0u : 0x0001u));
                left = x; top = y;
            }
        }
        internal void SetBackground(BackgroundFrame frame)
        {
            if (disposed) throw new ObjectDisposedException("AcrylicHost");
            background.SetFrame(frame);
        }
        internal void SetMotion(double scale, double opacity)
        {
            if (disposed) return;
            if (lastScale != scale) { root.Scale = new Vector3((float)scale, (float)scale, 1); lastScale = scale; }
            if (lastOpacity != opacity) { root.Opacity = (float)opacity; lastOpacity = opacity; }
        }
        internal void Show()
        {
            if (!disposed && !Visible)
            {
                WindowsNativeMethods.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, 0x0010 | 0x0200 | 0x0004 | 0x0001 | 0x0002 | 0x0040);
                Visible = true;
            }
        }
        internal void Hide()
        {
            if (!disposed && Visible)
            {
                WindowsNativeMethods.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, 0x0010 | 0x0200 | 0x0004 | 0x0001 | 0x0002 | 0x0080);
                Visible = false;
            }
        }
        public void Dispose()
        {
            if (disposed) return; Hide(); disposed = true;
            if (overlay != IntPtr.Zero && WindowsNativeMethods.IsWindow(overlay) && WindowsNativeMethods.GetWindow(overlay, 4) == Handle)
                WindowsNativeMethods.SetWindowOwner(overlay, IntPtr.Zero);
            try
            {
                target.Root = null; visual.Clip = null; visual.Brush = null; backgroundVisual.Brush = null;
                root.Children.RemoveAll();
            }
            finally
            {
                try { background.Dispose(); }
                finally
                {
                    backgroundVisual.Dispose(); visual.Dispose(); root.Dispose(); target.Dispose();
                    if (blurBrush != null) blurBrush.Dispose(); if (blurFactory != null) blurFactory.Dispose();
                    if (clip != null) clip.Dispose(); if (clipPath != null) clipPath.Dispose();
                    if (geometry != null) geometry.Dispose();
                    backdrop.Dispose(); compositor.Dispose(); source.Dispose();
                }
            }
        }
    }

    [ComVisible(true), Guid("2FC57384-A068-44D7-A331-30982FCF7177"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        WindowsRuntimeType("Cccc.GaussianBlurEffect"), WindowsRuntimeHelperType(typeof(EffectInteroperabilityAbi))]
    public interface IEffectInteroperability
    {
        [PreserveSig] int GetEffectId(out Guid id);
        [PreserveSig] int GetNamedPropertyMapping([MarshalAs(UnmanagedType.LPWStr)] string name, out uint index, out uint mapping);
        [PreserveSig] int GetPropertyCount(out uint count);
        [PreserveSig] int GetProperty(uint index, out IntPtr value);
        [PreserveSig] int GetSource(uint index, out IGraphicsEffectSource value);
        [PreserveSig] int GetSourceCount(out uint count);
    }
    // The compositor also queries the IUnknown-based effect interop interface
    // when building an effect factory. Expose it explicitly for the CCW.
    [Guid("2FC57384-A068-44D7-A331-30982FCF7177")]
    public static class EffectInteroperabilityAbi
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int GetEffectIdDelegate(IntPtr self, out Guid id);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int GetNamedPropertyMappingDelegate(IntPtr self, IntPtr name, out uint index, out uint mapping);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int GetPropertyCountDelegate(IntPtr self, out uint count);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int GetPropertyDelegate(IntPtr self, uint index, out IntPtr value);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int GetSourceDelegate(IntPtr self, uint index, out IntPtr value);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int GetSourceCountDelegate(IntPtr self, out uint count);
        private static readonly GetEffectIdDelegate GetEffectIdHandler = GetEffectId;
        private static readonly GetNamedPropertyMappingDelegate GetNamedPropertyMappingHandler = GetNamedPropertyMapping;
        private static readonly GetPropertyCountDelegate GetPropertyCountHandler = GetPropertyCount;
        private static readonly GetPropertyDelegate GetPropertyHandler = GetProperty;
        private static readonly GetSourceDelegate GetSourceHandler = GetSource;
        private static readonly GetSourceCountDelegate GetSourceCountHandler = GetSourceCount;
        public static readonly IntPtr AbiToProjectionVftablePtr;
        static EffectInteroperabilityAbi()
        {
            IntPtr unknown = ComWrappersSupport.IUnknownVftblPtr;
            IntPtr vtable = Marshal.AllocHGlobal(9 * IntPtr.Size);
            for (int index = 0; index < 3; index++)
                Marshal.WriteIntPtr(vtable, index * IntPtr.Size, Marshal.ReadIntPtr(unknown, index * IntPtr.Size));
            Marshal.WriteIntPtr(vtable, 3 * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(GetEffectIdHandler));
            Marshal.WriteIntPtr(vtable, 4 * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(GetNamedPropertyMappingHandler));
            Marshal.WriteIntPtr(vtable, 5 * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(GetPropertyCountHandler));
            Marshal.WriteIntPtr(vtable, 6 * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(GetPropertyHandler));
            Marshal.WriteIntPtr(vtable, 7 * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(GetSourceHandler));
            Marshal.WriteIntPtr(vtable, 8 * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(GetSourceCountHandler));
            AbiToProjectionVftablePtr = vtable;
        }
        private static int GetEffectId(IntPtr self, out Guid id)
        {
            id = Guid.Empty;
            try { return ComWrappersSupport.FindObject<GaussianBlurEffect>(self).GetEffectId(out id); }
            catch (Exception error) { return error.HResult; }
        }
        private static int GetNamedPropertyMapping(IntPtr self, IntPtr name, out uint index, out uint mapping)
        {
            index = 0; mapping = 0;
            try { return ComWrappersSupport.FindObject<GaussianBlurEffect>(self).GetNamedPropertyMapping(Marshal.PtrToStringUni(name), out index, out mapping); }
            catch (Exception error) { return error.HResult; }
        }
        private static int GetPropertyCount(IntPtr self, out uint count)
        {
            count = 0;
            try { return ComWrappersSupport.FindObject<GaussianBlurEffect>(self).GetPropertyCount(out count); }
            catch (Exception error) { return error.HResult; }
        }
        private static int GetProperty(IntPtr self, uint index, out IntPtr value)
        {
            value = IntPtr.Zero;
            try { return ComWrappersSupport.FindObject<GaussianBlurEffect>(self).GetProperty(index, out value); }
            catch (Exception error) { return error.HResult; }
        }
        private static int GetSource(IntPtr self, uint index, out IntPtr value)
        {
            value = IntPtr.Zero;
            try
            {
                IGraphicsEffectSource source;
                int status = ComWrappersSupport.FindObject<GaussianBlurEffect>(self).GetSource(index, out source);
                if (status < 0 || source == null) return status;
                value = MarshalInspectable<IGraphicsEffectSource>.FromManaged(source);
                return 0;
            }
            catch (Exception error) { return error.HResult; }
        }
        private static int GetSourceCount(IntPtr self, out uint count)
        {
            count = 0;
            try { return ComWrappersSupport.FindObject<GaussianBlurEffect>(self).GetSourceCount(out count); }
            catch (Exception error) { return error.HResult; }
        }
    }
    // The system's Gaussian effect through its documented D2D effect interop,
    // avoiding a Win2D/Windows App SDK runtime dependency.
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public sealed class GaussianBlurEffect : IGraphicsEffect, IEffectInteroperability
    {
        public string Name { get; set; }
        internal float Radius;
        internal IGraphicsEffectSource Source;
        public GaussianBlurEffect() { Name = "CcccGaussianBlurEffect"; }
        public int GetEffectId(out Guid id) { id = new Guid("1FEB6D69-2FE6-4AC9-8C58-1D7F93E7A6A5"); return 0; }
        public int GetNamedPropertyMapping(string name, out uint index, out uint mapping)
        { index = 0; mapping = 1; return name == "BlurAmount" ? 0 : unchecked((int)0x80070057); }
        public int GetPropertyCount(out uint count) { count = 3; return 0; }
        public int GetProperty(uint index, out IntPtr value)
        {
            value = IntPtr.Zero; if (index > 2) return unchecked((int)0x80070057);
            value = EffectValue.Box(index == 0, Radius, index == 1 ? 1u : 0u); return 0;
        }
        public int GetSource(uint index, out IGraphicsEffectSource value) { value = index == 0 ? Source : null; return value == null ? unchecked((int)0x80070057) : 0; }
        public int GetSourceCount(out uint count) { count = 1; return 0; }
    }

    internal static class EffectValue
    {
        [DllImport("combase.dll", CharSet = CharSet.Unicode)] private static extern int WindowsCreateString(string text, int length, out IntPtr value);
        [DllImport("combase.dll")] private static extern int WindowsDeleteString(IntPtr value);
        [DllImport("combase.dll")] private static extern int RoGetActivationFactory(IntPtr name, ref Guid iid, out IntPtr factory);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int BoxSingle(IntPtr self, float value, out IntPtr result);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int BoxUInt32(IntPtr self, uint value, out IntPtr result);
        // The CLR projects IPropertyValue as a boxed managed value. The effect
        // ABI needs the native IPropertyValue interface, so keep boxing native.
        internal static IntPtr Box(bool single, float number, uint integer)
        {
            IntPtr name = IntPtr.Zero, factory = IntPtr.Zero, boxed = IntPtr.Zero, result = IntPtr.Zero;
            try
            {
                string type = "Windows.Foundation.PropertyValue";
                Marshal.ThrowExceptionForHR(WindowsCreateString(type, type.Length, out name));
                Guid iid = new Guid("629BDBC8-D932-4FF4-96B9-8D96C5C1E858");
                Marshal.ThrowExceptionForHR(RoGetActivationFactory(name, ref iid, out factory));
                IntPtr method = Marshal.ReadIntPtr(Marshal.ReadIntPtr(factory), (single ? 14 : 11) * IntPtr.Size);
                int status = single ? ((BoxSingle)Marshal.GetDelegateForFunctionPointer(method, typeof(BoxSingle)))(factory, number, out boxed) :
                    ((BoxUInt32)Marshal.GetDelegateForFunctionPointer(method, typeof(BoxUInt32)))(factory, integer, out boxed);
                Marshal.ThrowExceptionForHR(status);
                iid = new Guid("4BD682DD-7554-40E9-9A9B-82654EDE7E62");
                Marshal.ThrowExceptionForHR(Marshal.QueryInterface(boxed, in iid, out result)); return result;
            }
            finally { if (boxed != IntPtr.Zero) Marshal.Release(boxed); if (factory != IntPtr.Zero) Marshal.Release(factory); if (name != IntPtr.Zero) WindowsDeleteString(name); }
        }
    }

    [ComVisible(true), Guid("0657AF73-53FD-47CF-84FF-C8492D2A80A3"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        WindowsRuntimeType("Cccc.GeometrySource2DInterop"), WindowsRuntimeHelperType(typeof(GeometrySourceInteroperabilityAbi))]
    public interface IGeometryInteroperability
    {
        [PreserveSig] int GetGeometry(out IntPtr value);
        [PreserveSig] int TryGetGeometryUsingFactory(IntPtr factory, out IntPtr value);
    }
    // The composition path factory also queries the IUnknown-based
    // IGeometrySource2DInterop interface. CsWinRT only exposes Windows
    // Runtime interfaces on a managed CCW, so publish this vtable explicitly.
    [Guid("0657AF73-53FD-47CF-84FF-C8492D2A80A3")]
    public static class GeometrySourceInteroperabilityAbi
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int GetGeometryDelegate(IntPtr self, out IntPtr value);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int TryGetGeometryDelegate(IntPtr self, IntPtr factory, out IntPtr value);
        private static readonly GetGeometryDelegate GetGeometryHandler = GetGeometry;
        private static readonly TryGetGeometryDelegate TryGetGeometryFactoryHandler = TryGetGeometryFactory;
        public static readonly IntPtr AbiToProjectionVftablePtr;
        static GeometrySourceInteroperabilityAbi()
        {
            IntPtr unknown = ComWrappersSupport.IUnknownVftblPtr;
            IntPtr vtable = Marshal.AllocHGlobal(5 * IntPtr.Size);
            for (int index = 0; index < 3; index++)
                Marshal.WriteIntPtr(vtable, index * IntPtr.Size, Marshal.ReadIntPtr(unknown, index * IntPtr.Size));
            Marshal.WriteIntPtr(vtable, 3 * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(GetGeometryHandler));
            Marshal.WriteIntPtr(vtable, 4 * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(TryGetGeometryFactoryHandler));
            AbiToProjectionVftablePtr = vtable;
        }
        private static int GetGeometry(IntPtr self, out IntPtr value)
        {
            value = IntPtr.Zero;
            try { return ComWrappersSupport.FindObject<GeometrySource>(self).GetGeometry(out value); }
            catch (Exception error) { return error.HResult; }
        }
        private static int TryGetGeometryFactory(IntPtr self, IntPtr factory, out IntPtr value)
        {
            value = IntPtr.Zero;
            try { return ComWrappersSupport.FindObject<GeometrySource>(self).TryGetGeometryUsingFactory(factory, out value); }
            catch (Exception error) { return error.HResult; }
        }
    }
    [ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public sealed class GeometrySource : IGeometrySource2D, IGeometryInteroperability, IDisposable
    {
        [StructLayout(LayoutKind.Sequential)] private struct FloatPoint { internal float X, Y; internal FloatPoint(Point p) { X = (float)p.X; Y = (float)p.Y; } }
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int CreatePath(IntPtr self, out IntPtr path);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int OpenPath(IntPtr self, out IntPtr sink);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void SetFill(IntPtr self, int mode);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void BeginFigure(IntPtr self, FloatPoint start, int begin);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void AddLines(IntPtr self, [In] FloatPoint[] points, uint count);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate void EndFigure(IntPtr self, int end);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int CloseSink(IntPtr self);
        [DllImport("d2d1.dll")] private static extern int D2D1CreateFactory(int type, ref Guid iid, IntPtr options, out IntPtr factory);
        private IntPtr geometry;
        private GeometrySource(IntPtr geometry) { this.geometry = geometry; }
        private static T Method<T>(IntPtr value, int index) where T : class
        { return (T)(object)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(Marshal.ReadIntPtr(value), index * IntPtr.Size), typeof(T)); }
        internal static GeometrySource Create(Geometry outline)
        {
            Guid iid = new Guid("06152247-6F50-465A-9245-118BFD3B6007");
            IntPtr factory = IntPtr.Zero, path = IntPtr.Zero, sink = IntPtr.Zero;
            try
            {
                Marshal.ThrowExceptionForHR(D2D1CreateFactory(0, ref iid, IntPtr.Zero, out factory));
                Marshal.ThrowExceptionForHR(Method<CreatePath>(factory, 10)(factory, out path));
                Marshal.ThrowExceptionForHR(Method<OpenPath>(path, 17)(path, out sink));
                Method<SetFill>(sink, 3)(sink, 1);
                foreach (PathFigure figure in outline.GetFlattenedPathGeometry(.01, ToleranceType.Absolute).Figures)
                {
                    Method<BeginFigure>(sink, 5)(sink, new FloatPoint(figure.StartPoint), 0);
                    List<FloatPoint> points = new List<FloatPoint>();
                    foreach (PathSegment segment in figure.Segments)
                    {
                        PolyLineSegment poly = segment as PolyLineSegment;
                        if (poly != null) foreach (Point p in poly.Points) points.Add(new FloatPoint(p));
                        LineSegment line = segment as LineSegment; if (line != null) points.Add(new FloatPoint(line.Point));
                    }
                    Method<AddLines>(sink, 6)(sink, points.ToArray(), (uint)points.Count);
                    Method<EndFigure>(sink, 8)(sink, 1);
                }
                Marshal.ThrowExceptionForHR(Method<CloseSink>(sink, 9)(sink));
                GeometrySource result = new GeometrySource(path); path = IntPtr.Zero; return result;
            }
            finally { if (sink != IntPtr.Zero) Marshal.Release(sink); if (path != IntPtr.Zero) Marshal.Release(path); if (factory != IntPtr.Zero) Marshal.Release(factory); }
        }
        public int GetGeometry(out IntPtr value) { value = geometry; if (value == IntPtr.Zero) return unchecked((int)0x80004005); Marshal.AddRef(value); return 0; }
        public int TryGetGeometryUsingFactory(IntPtr factory, out IntPtr value) { value = IntPtr.Zero; return 0; }
        public void Dispose() { if (geometry != IntPtr.Zero) { Marshal.Release(geometry); geometry = IntPtr.Zero; } GC.SuppressFinalize(this); }
        ~GeometrySource() { if (geometry != IntPtr.Zero) Marshal.Release(geometry); }
    }
}
