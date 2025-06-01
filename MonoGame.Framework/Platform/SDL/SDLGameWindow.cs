// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.Utilities;

namespace Microsoft.Xna.Framework
{
    internal class SdlGameWindow : GameWindow, IDisposable
    {
        public override bool AllowUserResizing
        {
            get { return !IsBorderless && _resizable; }
            set
            {
                var nonResizeableVersion = new Sdl.Version() { Major = 2, Minor = 0, Patch = 4 };

                if (Sdl.version > nonResizeableVersion)
                    Sdl.Window.SetResizable(_handle, value);
                else
                    throw new Exception("SDL " + nonResizeableVersion + " does not support changing resizable parameter of the window after it's already been created, please use a newer version of it.");

                _resizable = value;
            }
        }

        public override Rectangle ClientBounds
        {
            get
            {
                int x = 0, y = 0;
                Sdl.Window.GetPosition(Handle, out x, out y);
                return new Rectangle(x, y, _width, _height);
            }
        }

        public override Point Position
        {
            get
            {
                int x = 0, y = 0;

                if (!IsFullScreen)
                    Sdl.Window.GetPosition(Handle, out x, out y);

                return new Point(x, y);
            }
            set
            {
                Sdl.Window.SetPosition(Handle, value.X, value.Y);
                _wasMoved = true;
            }
        }

        public override DisplayOrientation CurrentOrientation
        {
            get { return DisplayOrientation.Default; }
        }

        public override IntPtr Handle
        {
            get { return _handle; }
        }

        public override string ScreenDeviceName
        {
            get { return _screenDeviceName; }
        }

        public override bool IsBorderless
        {
            get { return _borderless; }
            set
            {
                Sdl.Window.SetBordered(_handle, value ? 0 : 1);
                _borderless = value;
            }
        }

        public static GameWindow Instance;
        public uint? Id;
        public bool IsFullScreen;

        internal readonly Game _game;
        private IntPtr _handle, _icon;
        private bool _disposed;
        private bool _resizable, _borderless, _willBeFullScreen, _mouseVisible, _hardwareSwitch;
        private string _screenDeviceName;
        private int _width, _height;
        private bool _wasMoved, _supressMoved;
        // Begin Fumen Modification
        private double _platformDpiScale = 1.0;
        // End Fumen Modification

        public SdlGameWindow(Game game)
        {
            _game = game;
            _screenDeviceName = "";

            Instance = this;

            _width = GraphicsDeviceManager.DefaultBackBufferWidth;
            _height = GraphicsDeviceManager.DefaultBackBufferHeight;

            Sdl.SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", "0");
            Sdl.SetHint("SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS", "1");

            // Begin Fumen Modification
            if (CurrentPlatform.OS == OS.MacOSX)
                Sdl.SetHint("SDL_HINT_VIDEO_HIGHDPI_DISABLED", "0");
            // End Fumen Modification

            // when running NUnit tests entry assembly can be null
            if (Assembly.GetEntryAssembly() != null)
            {
                using (
                    var stream =
                        Assembly.GetEntryAssembly().GetManifestResourceStream(Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace + ".Icon.bmp") ??
                        Assembly.GetEntryAssembly().GetManifestResourceStream("Icon.bmp") ??
                        Assembly.GetExecutingAssembly().GetManifestResourceStream("MonoGame.bmp"))
                {
                    if (stream != null)
                        using (var br = new BinaryReader(stream))
                        {
                            try
                            {
                                var src = Sdl.RwFromMem(br.ReadBytes((int)stream.Length), (int)stream.Length);
                                _icon = Sdl.LoadBMP_RW(src, 1);
                            }
                            catch { }
                        }
                }
            }

            // Begin Fumen Modification
            // Get the DPI scale by comparing the window size to the drawable size.
            // For MacOS with HiDPI scaling SDL will report half sizes for the window.
            // Getting the actual scale here lets us compensate.
            var initFlags = Sdl.Window.State.Hidden | Sdl.Window.State.FullscreenDesktop;
            if (CurrentPlatform.OS == OS.MacOSX)
                initFlags |= Sdl.Window.State.AllowHighDPI;
            _handle = Sdl.Window.Create("", 0, 0,
                GraphicsDeviceManager.DefaultBackBufferWidth, GraphicsDeviceManager.DefaultBackBufferHeight,
                initFlags);
            RefreshCachedPlatformDpiScale();
            // End Fumen Modification
        }

        // Begin Fumen Modification
        private void RefreshCachedPlatformDpiScale()
        {
            if (CurrentPlatform.OS == OS.MacOSX)
            {
                Sdl.Window.GetSize(_handle, out var windowWidth, out _);
                Sdl.GL.GetDrawableSize(_handle, out var drawableWidth, out _);
                _platformDpiScale = (double)drawableWidth / windowWidth;
            }
        }
        // End Fumen Modification

        // Begin Fumen Modification
        //internal void CreateWindow()
        internal void CreateWindow(PresentationParameters presentationParameters)
        // End Fumen Modification
        {
            var initflags =
                Sdl.Window.State.OpenGL |
                Sdl.Window.State.Hidden |
                Sdl.Window.State.InputFocus |
                Sdl.Window.State.MouseFocus;

            // Begin Fumen Modification
            if (CurrentPlatform.OS == OS.MacOSX)
                initflags |= Sdl.Window.State.AllowHighDPI;
            if (presentationParameters.IsMaximized)
                initflags |= Sdl.Window.State.Maximized;
            // End Fumen Modification

            if (_handle != IntPtr.Zero)
                Sdl.Window.Destroy(_handle);

            var winx = Sdl.Window.PosCentered;
            var winy = Sdl.Window.PosCentered;

            // if we are on Linux, start on the current screen
            if (CurrentPlatform.OS == OS.Linux)
            {
                winx |= GetMouseDisplay();
                winy |= GetMouseDisplay();
            }

            // Begin Fumen Modification
            //_width = GraphicsDeviceManager.DefaultBackBufferWidth;
            //_height = GraphicsDeviceManager.DefaultBackBufferHeight;
            _width = presentationParameters.BackBufferWidth;
            _height = presentationParameters.BackBufferHeight;
            // End Fumen Modification

            _handle = Sdl.Window.Create(
                AssemblyHelper.GetDefaultWindowTitle(),
                winx, winy, _width, _height, initflags
            );

            Id = Sdl.Window.GetWindowId(_handle);

            if (_icon != IntPtr.Zero)
                Sdl.Window.SetIcon(_handle, _icon);

            Sdl.Window.SetBordered(_handle, _borderless ? 0 : 1);
            Sdl.Window.SetResizable(_handle, _resizable);

            SetCursorVisible(_mouseVisible);
        }

        ~SdlGameWindow()
        {
            Dispose(false);
        }

        private static int GetMouseDisplay()
        {
            var rect = new Sdl.Rectangle();

            int x, y;
            Sdl.Mouse.GetGlobalState(out x, out y);

            var displayCount = Sdl.Display.GetNumVideoDisplays();
            for (var i = 0; i < displayCount; i++)
            {
                Sdl.Display.GetBounds(i, out rect);

                if (x >= rect.X && x < rect.X + rect.Width &&
                    y >= rect.Y && y < rect.Y + rect.Height)
                {
                    return i;
                }
            }

            return 0;
        }

        public void SetCursorVisible(bool visible)
        {
            _mouseVisible = visible;
            Sdl.Mouse.ShowCursor(visible ? 1 : 0);
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            _willBeFullScreen = willBeFullScreen;
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            _screenDeviceName = screenDeviceName;

            var prevBounds = ClientBounds;
            var displayIndex = Sdl.Window.GetDisplayIndex(Handle);

            Sdl.Rectangle displayRect;
            Sdl.Display.GetBounds(displayIndex, out displayRect);

            if (_willBeFullScreen != IsFullScreen || _hardwareSwitch != _game.graphicsDeviceManager.HardwareModeSwitch)
            {
                var fullscreenFlag = _game.graphicsDeviceManager.HardwareModeSwitch ? Sdl.Window.State.Fullscreen : Sdl.Window.State.FullscreenDesktop;
                Sdl.Window.SetFullscreen(Handle, (_willBeFullScreen) ? fullscreenFlag : 0);
                _hardwareSwitch = _game.graphicsDeviceManager.HardwareModeSwitch;
            }
            // If going to exclusive full-screen mode, force the window to minimize on focus loss (Windows only)
            if (CurrentPlatform.OS == OS.Windows)
            {
                Sdl.SetHint("SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS", _willBeFullScreen && _hardwareSwitch ? "1" : "0");
            }

            if (!_willBeFullScreen || _game.graphicsDeviceManager.HardwareModeSwitch)
            {
                Sdl.Window.SetSize(Handle, clientWidth, clientHeight);
                _width = clientWidth;
                _height = clientHeight;
            }
            else
            {
                _width = displayRect.Width;
                _height = displayRect.Height;
            }

            int ignore, minx = 0, miny = 0;
            Sdl.Window.GetBorderSize(_handle, out miny, out minx, out ignore, out ignore);

            var centerX = Math.Max(prevBounds.X + ((prevBounds.Width - clientWidth) / 2), minx);
            var centerY = Math.Max(prevBounds.Y + ((prevBounds.Height - clientHeight) / 2), miny);

            if (IsFullScreen && !_willBeFullScreen)
            {
                // We need to get the display information again in case
                // the resolution of it was changed.
                Sdl.Display.GetBounds (displayIndex, out displayRect);

                // This centering only occurs when exiting fullscreen
                // so it should center the window on the current display.
                centerX = displayRect.X + displayRect.Width / 2 - clientWidth / 2;
                centerY = displayRect.Y + displayRect.Height / 2 - clientHeight / 2;
            }

            // If this window is resizable, there is a bug in SDL 2.0.4 where
            // after the window gets resized, window position information
            // becomes wrong (for me it always returned 10 8). Solution is
            // to not try and set the window position because it will be wrong.
            if ((Sdl.version > new Sdl.Version() { Major = 2, Minor = 0, Patch = 4 }  || !AllowUserResizing) && !_wasMoved)
                Sdl.Window.SetPosition(Handle, centerX, centerY);

            if (IsFullScreen != _willBeFullScreen)
                OnClientSizeChanged();

            IsFullScreen = _willBeFullScreen;

            _supressMoved = true;
        }

        internal void Moved()
        {
            if (_supressMoved)
            {
                _supressMoved = false;
                return;
            }

            _wasMoved = true;
        }

        public void ClientResize(int width, int height)
        {
            // Begin Fumen Modification
            // When creating an SDL window that starts maximized it does not consider the taskbar area.
            // It will use bounds larger than the available screen real estate. We need to clamp the
            // dimensions to the usable bounds to correct this.
            if (IsMaximized())
            {
                var displayIndex = Sdl.Window.GetDisplayIndex(Handle);
                Sdl.Display.GetUsableBounds(displayIndex, out var bounds);
                if (width > bounds.Width)
                    width = bounds.Width;
                if (height > bounds.Height)
                    height = bounds.Height;
            }

            // Convert SDL points to pixels for platforms which report values in points.
            RefreshCachedPlatformDpiScale();
            width = (int)(width * _platformDpiScale);
            height = (int)(height * _platformDpiScale);
            // End Fumen Modification

            // SDL reports many resize events even if the Size didn't change.
            // Only call the code below if it actually changed.
            if (_game.GraphicsDevice.PresentationParameters.BackBufferWidth == width &&
                _game.GraphicsDevice.PresentationParameters.BackBufferHeight == height) {
                return;
            }

            _game.GraphicsDevice.PresentationParameters.BackBufferWidth = width;
            _game.GraphicsDevice.PresentationParameters.BackBufferHeight = height;
            _game.GraphicsDevice.Viewport = new Viewport(0, 0, width, height);

            // Begin Fumen Modification
            //Sdl.Window.GetSize(Handle, out _width, out _height);
            _width = width;
            _height = height;
            // End Fumen Modification

            OnClientSizeChanged();
        }

        protected internal override void SetSupportedOrientations(DisplayOrientation orientations)
        {
            // Nothing to do here
        }

        protected override void SetTitle(string title)
        {
            Sdl.Window.SetTitle(_handle, title);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            Sdl.Window.Destroy(_handle);
            _handle = IntPtr.Zero;

            if (_icon != IntPtr.Zero)
                Sdl.FreeSurface(_icon);

            _disposed = true;
        }

        // Begin Fumen Modification
        public override void SetResolution(int w, int h)
        {
            if (IsMaximized())
                Sdl.RestoreWindow(Handle);

            _game.graphicsDeviceManager.PreferredBackBufferWidth = w;
            _game.graphicsDeviceManager.PreferredBackBufferHeight = h;
            _game.graphicsDeviceManager.ApplyChanges();
        }

        public override bool IsMaximized()
        {
            var flags = Sdl.Window.GetWindowFlags(Handle);
            return (flags & Sdl.Window.State.Maximized) != 0;
        }

        public override void Maximize()
        {
            Sdl.MaximizeWindow(Handle);
        }

        public override void SetClipboardText(string text)
        {
            Sdl.SetClipboardText(text);
        }

        public override string GetClipboardText()
        {
            var textPtr = Sdl.GetClipboardText();
            var text = System.Runtime.InteropServices.Marshal.PtrToStringAuto(textPtr);
            Sdl.GameController.SDL_Free(textPtr);
            return text;
        }

        public override void AllowDragDrop(bool allow)
        {
            // TODO: Conditionally enable drag / drop support.
        }

        public override double GetMonitorDpiScale()
        {
            if (CurrentPlatform.OS == OS.MacOSX)
                return _platformDpiScale;
            var displayIndex = Sdl.Window.GetDisplayIndex(Handle);
            Sdl.Display.GetDisplayDPI(displayIndex, out _, out var hdpi, out _);
            return hdpi / 96.0;
        }

        public override double GetPlatformDpiScale()
        {
            return _platformDpiScale;
        }
        // End Fumen Modification
    }
}
