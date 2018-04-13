using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace reCLI.Helpers
{
    class BlurWindowHelper
    {
        #region Blur Handling
        /*
        Found on https://github.com/riverar/sample-win10-aeroglass
        */
        private enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        private enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }
        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        /// <summary>
        /// Sets the blur for a window via SetWindowCompositionAttribute
        /// </summary>
        public static void SetBlurForWindow()
        {

            // Exception of FindResource can't be cathed if global exception handle is set
            if (Environment.OSVersion.Version >= new Version(6, 2))
            {
                var resource = Application.Current.TryFindResource("ThemeBlurEnabled");
                bool blur;
                if (resource is bool)
                {
                    blur = (bool)resource;
                }
                else
                {
                    blur = false;
                }

                if (blur)
                {
                    SetWindowAccent(Application.Current.MainWindow, AccentState.ACCENT_ENABLE_BLURBEHIND);
                }
                else
                {
                    SetWindowAccent(Application.Current.MainWindow, AccentState.ACCENT_DISABLED);
                }
            }
        }

        private static void SetWindowAccent(Window w, AccentState state)
        {
            var windowHelper = new System.Windows.Interop.WindowInteropHelper(w);
            var accent = new AccentPolicy { AccentState = state };
            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
        #endregion
    }
}
