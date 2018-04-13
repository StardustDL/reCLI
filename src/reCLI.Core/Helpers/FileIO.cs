using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace reCLI.Core.Helpers
{
    public static class FileIO
    {
        public static string ReadText(string path, Encoding encoding = null)
        {
            return File.ReadAllText(path, encoding ?? Encoding.UTF8);
        }

        public static void WriteText(string path, string contents, Encoding encoding = null)
        {
            File.WriteAllText(path, contents, encoding ?? Encoding.UTF8);
        }
    }
}
