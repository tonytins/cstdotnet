// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.
#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace CSTNet;

public static class CST
{
	const char CARET = '^';
	const string LF = "\u000A";
	const string CR = "\u000D";
	const string CRLF = "\u000D\u000A";
	const string LS = "\u2028";
	const string MISSING = "***MISSING***";

	/// <summary>
	/// Gets the value for an integer key.
	/// </summary>
	/// <param name="content">The CST document.</param>
	/// <param name="key">The integer key to find.</param>
	/// <returns>The value, or <c>***MISSING***</c> if the key is not found.</returns>
	public static string Parse(string content, int key) => Parse(content, key.ToString(System.Globalization.CultureInfo.InvariantCulture));

	/// <summary>
	/// Gets the value for a string key.
	/// </summary>
	/// <param name="content">The CST document.</param>
	/// <param name="key">The key or name to find.</param>
	/// <returns>The value, or <c>***MISSING***</c> if the key is not found.</returns>
	public static string Parse(string content, string key)
	{
		var table = CSTTable.Parse(content);
		return table.TryGetValue(key, out var value) ? value : MISSING;
	}

#if DEBUG
	[UnmanagedCallersOnly(EntryPoint = "parse")]
	public static IntPtr Parse(IntPtr content, IntPtr key)
	{
		var tableContent = Marshal.PtrToStringAnsi(content) ?? throw new ArgumentNullException(nameof(content));
		var lookupKey = Marshal.PtrToStringAnsi(key) ?? throw new ArgumentNullException(nameof(key));
		return Marshal.StringToHGlobalAnsi(Parse(tableContent, lookupKey));
	}
#endif

	/// <summary>
	/// Normalizes line endings and separates text at caret/newline boundaries.
	/// </summary>
	/// <param name="content">The content to normalize.</param>
	/// <returns>The separated text segments.</returns>
	internal static IEnumerable<string> NormalizeEntries(string content)
	{
		ArgumentNullException.ThrowIfNull(content);

		content = content
			.Replace(CRLF, LF, StringComparison.Ordinal)
			.Replace(CR, LF, StringComparison.Ordinal)
			.Replace(LS, LF, StringComparison.Ordinal);

		if (Environment.NewLine != LF)
			content = content.Replace(LF, Environment.NewLine, StringComparison.Ordinal);

		return content.Split($"{CARET}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries)
			.Where(line =>
			{
				var trimmed = line.TrimStart();
				return !trimmed.StartsWith("//", StringComparison.Ordinal)
					&& !trimmed.StartsWith('#')
					&& !trimmed.StartsWith("/*", StringComparison.Ordinal)
					&& !trimmed.EndsWith("*/", StringComparison.Ordinal);
			});
	}
}