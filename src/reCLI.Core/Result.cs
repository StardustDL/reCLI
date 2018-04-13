using System;
using System.Collections.Generic;
using System.Text;

namespace reCLI.Core
{
    public class Result
    {
        public Result(bool autoHide, object contents=null)
        {
            AutoHide = autoHide;
            Contents = contents;
        }

        public bool IsRealCall { get; set; }

        public bool AutoHide { get; private set; }

        public object Contents { get; set; }

        public static Result NotAutoHide { get; private set; } = new Result(false) { IsRealCall = true };
        public static Result NotRealCall { get; private set; } = new Result(false) { IsRealCall = false };
    }
}
