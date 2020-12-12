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
            if (!content.Contains(Environment.NewLine))
            {
                if (content.Contains(_lf))
                    content = content.Replace(_lf, Environment.NewLine);

                if (content.Contains(_cr))
                    content = content.Replace(_cr, Environment.NewLine);

                if (content.Contains(_crlf))
                    content = content.Replace(_crlf, Environment.NewLine);

                if (content.Contains(_ls))
                    content = content.Replace(_ls, Environment.NewLine);
            }

            var lines = content.Split(new[] { $"{CARET}{Environment.NewLine}" },
                StringSplitOptions.RemoveEmptyEntries);
            var entries = new List<string>();

            foreach (var line in lines)
            {
                // Skip comments
                if (line.StartsWith("//") || line.StartsWith("#") ||
                    line.StartsWith("/*") || line.EndsWith("*/"))
                    continue;

                entries.Add(line);
            }

            return entries;
        }

        static string GetEntry(IEnumerable<string> entries, string key)
        {
            // Search through list
            foreach (var entry in entries)
            {
                // If the line doesn't start with the key, keep searching.
                if (!entry.StartsWith(key))
                    continue;

                // Locate index, trim carets and return translation.
                var startIndex = entry.IndexOf(CARET);
                var line = entry.Substring(startIndex);

                return line.TrimStart(CARET).TrimEnd(CARET);
            }

            return "***MISSING***";
        }
    }
}
