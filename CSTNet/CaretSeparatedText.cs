// This project is licensed under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sixam.CST
{
    public class CaretSeparatedText
    {

        const char CARET = '^';
        const string LF = "\u000A";
        const string CR = "\u000D";
        const string CRLF = "\u000D\u000A";
        const string LS = "\u2028";

        /// <summary>
        /// Gets the value from the digit-based key.
        /// </summary>
        /// <returns>Returns the entry</returns>
        public static string Parse(string content, int key) => Parse(content, key.ToString());

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
                if (content.Contains(LF))
                    content = content.Replace(LF, Environment.NewLine);

                if (content.Contains(CR))
                    content = content.Replace(CR, Environment.NewLine);

                if (content.Contains(CRLF))
                    content = content.Replace(CRLF, Environment.NewLine);

                if (content.Contains(LS))
                    content = content.Replace(LS, Environment.NewLine);
            }

            var lines = content.Split(new[] { $"{CARET}{Environment.NewLine}" },
                StringSplitOptions.RemoveEmptyEntries);

            return lines.Where(line =>
                    !line.StartsWith("//") &&
                    !line.StartsWith("#") &&
                    !line.StartsWith("/*") &&
                    !line.EndsWith("*/"))
                .AsEnumerable();
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

namespace CSTNet
{
    [Obsolete("CaretSeparatedText has moved to the Sixam.CST namespace.")]
    public class CaretSeparatedText : Sixam.CST.CaretSeparatedText { }
}
