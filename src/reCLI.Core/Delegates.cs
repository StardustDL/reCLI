using System;
using System.Collections.Generic;
using System.Text;

namespace reCLI.Core
{
    /// <summary>
    /// Global keyboard events
    /// </summary>
    /// <param name="keyevent">WM_KEYDOWN = 256,WM_KEYUP = 257,WM_SYSKEYUP = 261,WM_SYSKEYDOWN = 260</param>
    /// <param name="vkcode"></param>
    /// <param name="state"></param>
    /// <returns>return true to continue handling, return false to intercept system handling</returns>
    public delegate bool GlobalKeyboardEventHandler(int keyevent, int vkcode, SpecialKeyState state);

    public class ActionContext
    {
        public InvokeKey InvokeKeys { get; set; }
    }

    public enum InvokeKey
    {
        Enter,
        CtrlEnter,
        ShiftEnter,
        AltEnter,
        WinEnter,
    }

    public class SpecialKeyState
    {
        public bool CtrlPressed { get; set; }
        public bool ShiftPressed { get; set; }
        public bool AltPressed { get; set; }
        public bool WinPressed { get; set; }
    }
}
