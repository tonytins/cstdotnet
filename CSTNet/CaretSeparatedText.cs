// This project is licensed under the MIT license.
using System;
using System.Collections.Generic;

namespace CSTNet
{
    public static class CaretSeparatedText
    {
        /// <summary>
        /// Gets the value from the integer-based key.
        /// </summary>
        /// <returns>Returns the entry</returns>
        public static string Parse(string content, int key)
        {
            var entries = NormalizeEntries(content);
            return GetEntry(entries, $"{key}");
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
            var lineBreaks = new string[]
            {
                "^\u000A", // LF
                "^\u000D", // CR
                "^\u000D\u000A", // CR+LF
                "^\u2028" // LS
            };

            foreach (var line in lineBreaks)
            {
                var eol = Environment.NewLine; // System's line break

                // If the new line matches the system's, do nothing
                if (line.Contains(eol))
                    continue;

                content.Replace(line, eol);
            }

            return content.Split(lineBreaks, StringSplitOptions.RemoveEmptyEntries);

        }

        // TODO: support argument parameters
        static string GetEntry(IEnumerable<string> entries, string key)
        {
            var translation = "[ENTRY NOT FOUND]";

            // Search through array
            foreach (var entry in entries)
            {
                // Locate index, trim carets and return translation
                if (!entry.StartsWith(key))
                    continue;

                const char caret = '^';

                var startIndex = entry.IndexOf(caret.ToString(),
                    StringComparison.OrdinalIgnoreCase);

                var line = entry.Substring(startIndex);

                translation = line.Trim(caret);
            }

            return translation;
        }
    }
}
