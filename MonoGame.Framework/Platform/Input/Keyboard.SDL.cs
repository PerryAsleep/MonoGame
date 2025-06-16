// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Input
{
    public static partial class Keyboard
    {
        // Begin Fumen Modification
        // This entire file is effectively rewritten.
        private static KeyboardState CachedState;
        private static bool Dirty = true;
        private static List<Keys> HeldKeys = new();
        private static readonly object Lock = new();

        private static KeyboardState PlatformGetState()
        {
            if (!Dirty)
            {
                return CachedState;
            }
            lock (Lock)
            {
                var modifiers = Sdl.Keyboard.GetModState();
                CachedState = new KeyboardState(HeldKeys,
                                             (modifiers & Sdl.Keyboard.Keymod.CapsLock) == Sdl.Keyboard.Keymod.CapsLock,
                                             (modifiers & Sdl.Keyboard.Keymod.NumLock) == Sdl.Keyboard.Keymod.NumLock);
            }
            Dirty = false;
            return CachedState;
        }

        public static void PlatformKeyDown(Keys key)
        {
            lock (Lock)
            {
                if (!HeldKeys.Contains(key))
                    HeldKeys.Add(key);
                Dirty = true;
            }
        }

        public static void PlatformKeyUp(Keys key)
        {
            lock (Lock)
            {
                HeldKeys.Remove(key);
                Dirty = true;
            }
        }

        public static void PlatformClearHeldKeys()
        {
            lock (Lock)
            {
                HeldKeys.Clear();
                Dirty = true;
            }
        }
        // End Fumen Modification
    }
}
