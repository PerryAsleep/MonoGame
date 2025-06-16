// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Input
{
    // Begin Fumen Modification
    // internal static class KeyboardUtil
    public static class KeyboardUtil
    // End Fumen Modification
    {
        static Dictionary<int, Keys> _map;

        // Begin Fumen Modification
        private static readonly Dictionary<ushort, Keys> NSEventToKeys = new()
        {
            // Alphabet
            [0x00] = Keys.A,
            [0x0B] = Keys.B,
            [0x08] = Keys.C,
            [0x02] = Keys.D,
            [0x0E] = Keys.E,
            [0x03] = Keys.F,
            [0x05] = Keys.G,
            [0x04] = Keys.H,
            [0x22] = Keys.I,
            [0x26] = Keys.J,
            [0x28] = Keys.K,
            [0x25] = Keys.L,
            [0x2E] = Keys.M,
            [0x2D] = Keys.N,
            [0x1F] = Keys.O,
            [0x23] = Keys.P,
            [0x0C] = Keys.Q,
            [0x0F] = Keys.R,
            [0x01] = Keys.S,
            [0x11] = Keys.T,
            [0x20] = Keys.U,
            [0x09] = Keys.V,
            [0x0D] = Keys.W,
            [0x07] = Keys.X,
            [0x10] = Keys.Y,
            [0x06] = Keys.Z,

            // Numbers (top row)
            [0x12] = Keys.D1,
            [0x13] = Keys.D2,
            [0x14] = Keys.D3,
            [0x15] = Keys.D4,
            [0x17] = Keys.D5,
            [0x16] = Keys.D6,
            [0x1A] = Keys.D7,
            [0x1C] = Keys.D8,
            [0x19] = Keys.D9,
            [0x1D] = Keys.D0,

            // Symbols/punctuation
            [0x18] = Keys.OemPlus,
            [0x1B] = Keys.OemMinus,
            [0x1E] = Keys.OemCloseBrackets,
            [0x21] = Keys.OemOpenBrackets,
            [0x29] = Keys.OemSemicolon,
            [0x27] = Keys.OemQuotes,
            [0x2A] = Keys.OemPipe,
            [0x2B] = Keys.OemComma,
            [0x2C] = Keys.OemQuestion,
            [0x2F] = Keys.OemPeriod,
            [0x32] = Keys.OemTilde,

            // Whitespace/control
            [0x31] = Keys.Space,
            [0x30] = Keys.Tab,
            [0x33] = Keys.Back,
            [0x35] = Keys.Escape,
            [0x24] = Keys.Enter,          // Return
            [0x4C] = Keys.Enter,          // Keypad Enter

            // Modifier keys
            [0x38] = Keys.LeftShift,
            [0x3C] = Keys.RightShift,
            [0x3B] = Keys.LeftControl,
            [0x3E] = Keys.RightControl,
            [0x3A] = Keys.LeftAlt,
            [0x3D] = Keys.RightAlt,
            [0x39] = Keys.CapsLock,
            [0x37] = Keys.LeftWindows,
            [0x36] = Keys.RightWindows,

            // Navigation/edit
            [0x7B] = Keys.Left,
            [0x7C] = Keys.Right,
            [0x7D] = Keys.Down,
            [0x7E] = Keys.Up,
            [0x73] = Keys.Home,
            [0x77] = Keys.End,
            [0x74] = Keys.PageUp,
            [0x79] = Keys.PageDown,
            [0x47] = Keys.NumLock,
            [0x75] = Keys.Delete,
            [0x72] = Keys.Insert,

            // Function keys
            [0x7A] = Keys.F1,
            [0x78] = Keys.F2,
            [0x63] = Keys.F3,
            [0x76] = Keys.F4,
            [0x60] = Keys.F5,
            [0x61] = Keys.F6,
            [0x62] = Keys.F7,
            [0x64] = Keys.F8,
            [0x65] = Keys.F9,
            [0x6D] = Keys.F10,
            [0x67] = Keys.F11,
            [0x6F] = Keys.F12,
            [0x69] = Keys.F13,
            [0x6B] = Keys.F14,
            [0x71] = Keys.F15,
            [0x6A] = Keys.F16,
            [0x40] = Keys.F17,
            [0x4F] = Keys.F18,
            [0x50] = Keys.F19,
            [0x5A] = Keys.F20,

            // Numpad
            [0x41] = Keys.Decimal,
            [0x43] = Keys.Multiply,
            [0x45] = Keys.Add,
            [0x4E] = Keys.Subtract,
            [0x4B] = Keys.Divide,
            [0x52] = Keys.NumPad0,
            [0x53] = Keys.NumPad1,
            [0x54] = Keys.NumPad2,
            [0x55] = Keys.NumPad3,
            [0x56] = Keys.NumPad4,
            [0x57] = Keys.NumPad5,
            [0x58] = Keys.NumPad6,
            [0x59] = Keys.NumPad7,
            [0x5B] = Keys.NumPad8,
            [0x5C] = Keys.NumPad9,
        };
        // End Fumen Modification

        static KeyboardUtil()
        {
            _map = new Dictionary<int, Keys>();
            _map.Add(8, Keys.Back);
            _map.Add(9, Keys.Tab);
            _map.Add(13, Keys.Enter);
            _map.Add(27, Keys.Escape);
            _map.Add(32, Keys.Space);
            _map.Add(39, Keys.OemQuotes);
            _map.Add(43, Keys.Add);
            _map.Add(44, Keys.OemComma);
            _map.Add(45, Keys.OemMinus);
            _map.Add(46, Keys.OemPeriod);
            _map.Add(47, Keys.OemQuestion);
            _map.Add(48, Keys.D0);
            _map.Add(49, Keys.D1);
            _map.Add(50, Keys.D2);
            _map.Add(51, Keys.D3);
            _map.Add(52, Keys.D4);
            _map.Add(53, Keys.D5);
            _map.Add(54, Keys.D6);
            _map.Add(55, Keys.D7);
            _map.Add(56, Keys.D8);
            _map.Add(57, Keys.D9);
            _map.Add(59, Keys.OemSemicolon);
            _map.Add(60, Keys.OemBackslash);
            _map.Add(61, Keys.OemPlus);
            _map.Add(91, Keys.OemOpenBrackets);
            _map.Add(92, Keys.OemPipe);
            _map.Add(93, Keys.OemCloseBrackets);
            _map.Add(96, Keys.OemTilde);
            _map.Add(97, Keys.A);
            _map.Add(98, Keys.B);
            _map.Add(99, Keys.C);
            _map.Add(100, Keys.D);
            _map.Add(101, Keys.E);
            _map.Add(102, Keys.F);
            _map.Add(103, Keys.G);
            _map.Add(104, Keys.H);
            _map.Add(105, Keys.I);
            _map.Add(106, Keys.J);
            _map.Add(107, Keys.K);
            _map.Add(108, Keys.L);
            _map.Add(109, Keys.M);
            _map.Add(110, Keys.N);
            _map.Add(111, Keys.O);
            _map.Add(112, Keys.P);
            _map.Add(113, Keys.Q);
            _map.Add(114, Keys.R);
            _map.Add(115, Keys.S);
            _map.Add(116, Keys.T);
            _map.Add(117, Keys.U);
            _map.Add(118, Keys.V);
            _map.Add(119, Keys.W);
            _map.Add(120, Keys.X);
            _map.Add(121, Keys.Y);
            _map.Add(122, Keys.Z);
            _map.Add(127, Keys.Delete);
            _map.Add(1073741881, Keys.CapsLock);
            _map.Add(1073741882, Keys.F1);
            _map.Add(1073741883, Keys.F2);
            _map.Add(1073741884, Keys.F3);
            _map.Add(1073741885, Keys.F4);
            _map.Add(1073741886, Keys.F5);
            _map.Add(1073741887, Keys.F6);
            _map.Add(1073741888, Keys.F7);
            _map.Add(1073741889, Keys.F8);
            _map.Add(1073741890, Keys.F9);
            _map.Add(1073741891, Keys.F10);
            _map.Add(1073741892, Keys.F11);
            _map.Add(1073741893, Keys.F12);
            _map.Add(1073741894, Keys.PrintScreen);
            _map.Add(1073741895, Keys.Scroll);
            _map.Add(1073741896, Keys.Pause);
            _map.Add(1073741897, Keys.Insert);
            _map.Add(1073741898, Keys.Home);
            _map.Add(1073741899, Keys.PageUp);
            _map.Add(1073741901, Keys.End);
            _map.Add(1073741902, Keys.PageDown);
            _map.Add(1073741903, Keys.Right);
            _map.Add(1073741904, Keys.Left);
            _map.Add(1073741905, Keys.Down);
            _map.Add(1073741906, Keys.Up);
            _map.Add(1073741907, Keys.NumLock);
            _map.Add(1073741908, Keys.Divide);
            _map.Add(1073741909, Keys.Multiply);
            _map.Add(1073741910, Keys.Subtract);
            _map.Add(1073741911, Keys.Add);
            _map.Add(1073741912, Keys.Enter);
            _map.Add(1073741913, Keys.NumPad1);
            _map.Add(1073741914, Keys.NumPad2);
            _map.Add(1073741915, Keys.NumPad3);
            _map.Add(1073741916, Keys.NumPad4);
            _map.Add(1073741917, Keys.NumPad5);
            _map.Add(1073741918, Keys.NumPad6);
            _map.Add(1073741919, Keys.NumPad7);
            _map.Add(1073741920, Keys.NumPad8);
            _map.Add(1073741921, Keys.NumPad9);
            _map.Add(1073741922, Keys.NumPad0);
            _map.Add(1073741923, Keys.Decimal);
            _map.Add(1073741925, Keys.Apps);
            _map.Add(1073741928, Keys.F13);
            _map.Add(1073741929, Keys.F14);
            _map.Add(1073741930, Keys.F15);
            _map.Add(1073741931, Keys.F16);
            _map.Add(1073741932, Keys.F17);
            _map.Add(1073741933, Keys.F18);
            _map.Add(1073741934, Keys.F19);
            _map.Add(1073741935, Keys.F20);
            _map.Add(1073741936, Keys.F21);
            _map.Add(1073741937, Keys.F22);
            _map.Add(1073741938, Keys.F23);
            _map.Add(1073741939, Keys.F24);
            _map.Add(1073741951, Keys.VolumeMute);
            _map.Add(1073741952, Keys.VolumeUp);
            _map.Add(1073741953, Keys.VolumeDown);
            _map.Add(1073742040, Keys.OemClear);
            _map.Add(1073742044, Keys.Decimal);
            _map.Add(1073742048, Keys.LeftControl);
            _map.Add(1073742049, Keys.LeftShift);
            _map.Add(1073742050, Keys.LeftAlt);
            _map.Add(1073742051, Keys.LeftWindows);
            _map.Add(1073742052, Keys.RightControl);
            _map.Add(1073742053, Keys.RightShift);
            _map.Add(1073742054, Keys.RightAlt);
            _map.Add(1073742055, Keys.RightWindows);
            _map.Add(1073742082, Keys.MediaNextTrack);
            _map.Add(1073742083, Keys.MediaPreviousTrack);
            _map.Add(1073742084, Keys.MediaStop);
            _map.Add(1073742085, Keys.MediaPlayPause);
            _map.Add(1073742086, Keys.VolumeMute);
            _map.Add(1073742087, Keys.SelectMedia);
            _map.Add(1073742089, Keys.LaunchMail);
            _map.Add(1073742092, Keys.BrowserSearch);
            _map.Add(1073742093, Keys.BrowserHome);
            _map.Add(1073742094, Keys.BrowserBack);
            _map.Add(1073742095, Keys.BrowserForward);
            _map.Add(1073742096, Keys.BrowserStop);
            _map.Add(1073742097, Keys.BrowserRefresh);
            _map.Add(1073742098, Keys.BrowserFavorites);
            _map.Add(1073742106, Keys.Sleep);
        }

        public static Keys ToXna(int key)
        {
            Keys xnaKey;
            if (_map.TryGetValue(key, out xnaKey))
                return xnaKey;

            return Keys.None;
        }

        // Begin Fumen Modification
        public static bool TryGetKeyFromNSEventKey(ushort nsEventKey, out Keys key)
        {
            return NSEventToKeys.TryGetValue(nsEventKey, out key);
        }
        // End Fumen Modification
    }
}

