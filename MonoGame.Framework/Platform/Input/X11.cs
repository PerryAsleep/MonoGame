// Begin Fumen Modification
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Input;

internal sealed class X11
{
    private static IntPtr _libX11Handle;
    private static string[] X11LibraryNames = new[] { "libX11", "libX11.so", "libX11.so.6" };

    private delegate IntPtr XOpenDisplayDelegate(IntPtr display);
    private delegate IntPtr XCreateFontCursorDelegate(IntPtr display, uint shape);
    private delegate int XDefineCursorDelegate(IntPtr display, IntPtr window, IntPtr cursor);
    private delegate int XFreeCursorDelegate(IntPtr display, IntPtr cursor);
    private delegate int XCloseDisplayDelegate(IntPtr display);

    private static XOpenDisplayDelegate _xOpenDisplay;
    private static XCreateFontCursorDelegate _xCreateFontCursor;
    private static XDefineCursorDelegate _xDefineCursor;
    private static XFreeCursorDelegate _xFreeCursor;
    private static XCloseDisplayDelegate _xCloseDisplay;

    public static bool Initialize()
    {
        foreach (string libraryName in X11LibraryNames)
        {
            if (NativeLibrary.TryLoad(libraryName, out _libX11Handle))
            {
                _xOpenDisplay = Marshal.GetDelegateForFunctionPointer<XOpenDisplayDelegate>(
                    NativeLibrary.GetExport(_libX11Handle, "XOpenDisplay"));
                _xCreateFontCursor = Marshal.GetDelegateForFunctionPointer<XCreateFontCursorDelegate>(
                    NativeLibrary.GetExport(_libX11Handle, "XCreateFontCursor"));
                _xDefineCursor = Marshal.GetDelegateForFunctionPointer<XDefineCursorDelegate>(
                    NativeLibrary.GetExport(_libX11Handle, "XDefineCursor"));
                _xFreeCursor = Marshal.GetDelegateForFunctionPointer<XFreeCursorDelegate>(
                    NativeLibrary.GetExport(_libX11Handle, "XFreeCursor"));
                _xCloseDisplay = Marshal.GetDelegateForFunctionPointer<XCloseDisplayDelegate>(
                    NativeLibrary.GetExport(_libX11Handle, "XCloseDisplay"));
                return true;
            }
        }
        return false;
    }

    public static IntPtr OpenDisplay(IntPtr display) => _xOpenDisplay(display);
    public static IntPtr CreateFontCursor(IntPtr display, uint shape) => _xCreateFontCursor(display, shape);
    public static int DefineCursor(IntPtr display, IntPtr window, IntPtr cursor) => _xDefineCursor(display, window, cursor);
    public static int FreeCursor(IntPtr display, IntPtr cursor) => _xFreeCursor(display, cursor);
    public static int CloseDisplay(IntPtr display) => _xCloseDisplay(display);
}
// End Fumen Modification
