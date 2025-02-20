// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
// Begin Fumen Modification
using System.Runtime.InteropServices;
// End Fumen Modification
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework.Input
{
    public partial class MouseCursor
    {
        private MouseCursor(Sdl.Mouse.SystemCursor cursor)
        {
            // Begin Fumen Modification
            // Handle = Sdl.Mouse.CreateSystemCursor(cursor);
            if (ShouldUseX11Cursor())
            {
                if (X11Display == IntPtr.Zero)
                {
                    X11Display = X11.OpenDisplay(IntPtr.Zero);
                    X11DisplayCount++;
                }
                Handle = X11.CreateFontCursor(X11Display, GetX11Cursor(cursor));
            }
            else
            {
                Handle = Sdl.Mouse.CreateSystemCursor(cursor);
            }
            // End Fumen Modification
        }

        private static void PlatformInitalize()
        {
            // Begin Fumen Modification
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    X11Available = X11.Initialize();
                }
                catch (DllNotFoundException)
                {
                    X11Available = false;
                }
            }
            // End Fumen Modification

            Arrow = new MouseCursor(Sdl.Mouse.SystemCursor.Arrow);
            IBeam = new MouseCursor(Sdl.Mouse.SystemCursor.IBeam);
            Wait = new MouseCursor(Sdl.Mouse.SystemCursor.Wait);
            Crosshair = new MouseCursor(Sdl.Mouse.SystemCursor.Crosshair);
            WaitArrow = new MouseCursor(Sdl.Mouse.SystemCursor.WaitArrow);
            SizeNWSE = new MouseCursor(Sdl.Mouse.SystemCursor.SizeNWSE);
            SizeNESW = new MouseCursor(Sdl.Mouse.SystemCursor.SizeNESW);
            SizeWE = new MouseCursor(Sdl.Mouse.SystemCursor.SizeWE);
            SizeNS = new MouseCursor(Sdl.Mouse.SystemCursor.SizeNS);
            SizeAll = new MouseCursor(Sdl.Mouse.SystemCursor.SizeAll);
            No = new MouseCursor(Sdl.Mouse.SystemCursor.No);
            Hand = new MouseCursor(Sdl.Mouse.SystemCursor.Hand);
        }

        private static MouseCursor PlatformFromTexture2D(Texture2D texture, int originx, int originy)
        {
            IntPtr surface = IntPtr.Zero;
            IntPtr handle = IntPtr.Zero;
            try
            {
                var bytes = new byte[texture.Width * texture.Height * 4];
                texture.GetData(bytes);
                surface = Sdl.CreateRGBSurfaceFrom(bytes, texture.Width, texture.Height, 32, texture.Width * 4, 0x000000ff, 0x0000FF00, 0x00FF0000, 0xFF000000);
                if (surface == IntPtr.Zero)
                    throw new InvalidOperationException("Failed to create surface for mouse cursor: " + Sdl.GetError());

                handle = Sdl.Mouse.CreateColorCursor(surface, originx, originy);
                if (handle == IntPtr.Zero)
                    throw new InvalidOperationException("Failed to set surface for mouse cursor: " + Sdl.GetError());
            }
            finally
            {
                if (surface != IntPtr.Zero)
                    Sdl.FreeSurface(surface);
            }

            return new MouseCursor(handle);
        }

        private void PlatformDispose()
        {
            if (Handle == IntPtr.Zero)
                return;

            // Begin Fumen Modification
            //Sdl.Mouse.FreeCursor(Handle);
            if (ShouldUseX11Cursor())
            {
                X11.FreeCursor(X11Display, Handle);
                X11DisplayCount--;
                if (X11DisplayCount == 0 && X11Display != IntPtr.Zero)
                {
                    X11.CloseDisplay(X11Display);
                    X11Display = IntPtr.Zero;
                }
            }
            else
            {
                Sdl.Mouse.FreeCursor(Handle);
            }
            // End Fumen Modification
            Handle = IntPtr.Zero;
        }

        // Begin Fumen Modification
        private static bool X11Available;
        private static IntPtr X11Display;
        private static int X11DisplayCount = 0;

        private static bool ShouldUseX11Cursor()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && X11Available;
        }

        private static uint GetX11Cursor(Sdl.Mouse.SystemCursor cursorType)
        {
            switch (cursorType)
            {
                case Sdl.Mouse.SystemCursor.Arrow:
                    return 2; // XC_left_ptr
                case Sdl.Mouse.SystemCursor.IBeam:
                    return 152; // XC_xterm
                case Sdl.Mouse.SystemCursor.Wait:
                   return 150; // XC_watch
                case Sdl.Mouse.SystemCursor.Crosshair:
                    return 34; // XC_crosshair
                case Sdl.Mouse.SystemCursor.WaitArrow:
                    return 150; // XC_watch
                case Sdl.Mouse.SystemCursor.SizeNWSE:
                    return 14; // XC_bottom_right_corner
                case Sdl.Mouse.SystemCursor.SizeNESW:
                    return 12; // XC_bottom_left_corner
                case Sdl.Mouse.SystemCursor.SizeWE:
                    return 108; // XC_sb_h_double_arrow
                case Sdl.Mouse.SystemCursor.SizeNS:
                    return 116; // XC_sb_v_double_arrow
                case Sdl.Mouse.SystemCursor.SizeAll:
                    return 52; // XC_fleur;
                case Sdl.Mouse.SystemCursor.No:
                    return 0; // XC_X_cursor
                case Sdl.Mouse.SystemCursor.Hand:
                    return 2; // XC_hand2
            }
            return 2; // XC_left_ptr
        }

        public static void SetCursor(MouseCursor cursor, IntPtr windowHandle)
        {
            if (ShouldUseX11Cursor())
            {
                IntPtr display = X11.OpenDisplay(IntPtr.Zero);
                X11.DefineCursor(display, windowHandle, cursor.Handle);
            }
            else
            {
                Sdl.Mouse.SetCursor(cursor.Handle);
            }
        }
        // End Fumen Modification
    }
}
