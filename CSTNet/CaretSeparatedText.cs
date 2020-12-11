// This project is licensed under the MIT license.
using System;
using System.Collections.Generic;

namespace CSTNet
{
    public static class CaretSeparatedText
    {
        const char CARET = '^';
        static readonly string _lf = "\u000A";
        static readonly string _cr = "\u000D";
        static readonly string _crlf = "\u000D\u000A";
        static readonly string _ls = "\u2028";

        /// <summary>
        /// Gets the value from the integer-based key.
        /// </summary>
        /// <returns>Returns the entry</returns>
        public static string Parse(string content, int key)
        {
            var entries = NormalizeEntries(content);
            return GetEntry(entries, key.ToString());
        }

        /// <summary>
        /// Gets the value from the string-based key.
        /// </summary>
        /// <returns>Returns the entry</returns>
        public static string Parse(string content, string key)
        {
            var entries = NormalizeEntries(content);
            return GetEntry(entries, key);
        }

        /// <summary>
        /// Replaces the document's line endings with the native system line endings.
        /// </summary>
        /// <remarks>This stage ensures there are no crashes during parsing.</remarks>
        static IEnumerable<string> NormalizeEntries(string content)
        {

            /* 
            I tried putting the end carets with the different
            line endings in with the split function but it didn't work 
            */
            if (!content.Contains($"{CARET}{Environment.NewLine}"))
            {
                if (content.Contains($"{CARET}{_lf}"))
                    content = content.Replace($"{CARET}{_lf}",
                    $"{CARET}{Environment.NewLine}");

                if (content.Contains($"{CARET}{_cr}"))
                    content = content.Replace($"{CARET}{_cr}",
                    $"{CARET}{Environment.NewLine}");

                if (content.Contains($"{CARET}{_crlf}"))
                    content = content.Replace($"{CARET}{_crlf}",
                    $"{CARET}{Environment.NewLine}");

                if (content.Contains($"{CARET}{_ls}"))
                    content = content.Replace($"{CARET}{_ls}",
                    $"{CARET}{Environment.NewLine}");
            }


            var entries = content.Split(new[] { $"{CARET}{Environment.NewLine}" },
                StringSplitOptions.RemoveEmptyEntries);
            var newContent = new List<string>();

            foreach (var entry in entries)
            {
                // Skip comments
                if (entry.StartsWith(@"//") || entry.StartsWith("#") ||
                    entry.StartsWith("/*") || entry.EndsWith("*/"))
                    continue;

                newContent.Add(entry);
            }

            return newContent;
        }

        static string GetEntry(IEnumerable<string> entries, string key)
        {
            // Search through list
            foreach (var entry in entries)
            {
                // Locate index, trim carets and return translation
                if (!entry.StartsWith(key))
                    continue;

                var startIndex = entry.IndexOf(CARET);
                var line = entry.Substring(startIndex);

                if (!line.Contains(Environment.NewLine))
                {
                    if (line.Contains(_lf))
                        line = line.Replace(_lf, Environment.NewLine);

                    if (line.Contains(_cr))
                        line = line.Replace(_cr, Environment.NewLine);

                    if (line.Contains(_crlf))
                        line = line.Replace(_crlf, Environment.NewLine);

                    if (line.Contains(_ls))
                        line = line.Replace(_ls, Environment.NewLine);
                }

                return line.TrimStart(CARET).TrimEnd(CARET);
            }

            return "[ENTRY NOT FOUND]";
        }
    }
}
