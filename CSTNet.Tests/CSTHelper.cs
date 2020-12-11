using System;
using System.IO;

namespace CSTNet.Tests
{
    static class CSTHelper
    {
        public static string CSTFile(string cst, string key)
        {
            var path = Path.Combine(AppContext.BaseDirectory, cst);
            var file = File.ReadAllText(path);

            return CaretSeparatedText.Parse(file, key);
        }
    }
}
