// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.
#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace CSTNet;

/// <summary>
/// One-off lookups in a CST document, and filling a value's placeholders.
/// </summary>
/// <remarks>
/// For more than one lookup, or to tell a missing value from real text, read the document once
/// with <see cref="CSTTable.Parse(string)"/>.
/// </remarks>
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

	/// <summary>
	/// Gets the value for an integer key and fills its placeholders.
	/// </summary>
	/// <param name="content">The CST document.</param>
	/// <param name="key">The integer key to find.</param>
	/// <param name="variables">The values for the placeholders, in order, as <see cref="Format"/> takes them.</param>
	/// <returns>The filled value, or <c>***MISSING***</c> if the key is not found.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="content"/> or <paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">One of <paramref name="variables"/> is <see langword="null"/>.</exception>
	public static string Parse(string content, int key, params object?[] variables) =>
		Parse(content, key.ToString(System.Globalization.CultureInfo.InvariantCulture), variables);

	/// <summary>
	/// Gets the value for a string key and fills its placeholders.
	/// </summary>
	/// <param name="content">The CST document.</param>
	/// <param name="key">The key or name to find.</param>
	/// <param name="variables">The values for the placeholders, in order, as <see cref="Format"/> takes them.</param>
	/// <returns>The filled value, or <c>***MISSING***</c> if the key is not found.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="content"/>, <paramref name="key"/>, or <paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">One of <paramref name="variables"/> is <see langword="null"/>.</exception>
	public static string Parse(string content, string key, params object?[] variables)
	{
		var texts = CSTPlaceholders.ToTexts(variables, nameof(variables));

		var table = CSTTable.Parse(content);
		return table.TryGetValue(key, out var value) ? CSTPlaceholders.Fill(value, texts) : MISSING;
	}

	/// <summary>
	/// Fills a value's placeholders, for code that reads values from a <see cref="CSTTable"/>.
	/// </summary>
	/// <remarks>
	/// <para><c>%s</c>, <c>%d</c>, <c>%i</c>, <c>%u</c>, <c>%ld</c>, <c>%li</c>, and <c>%lu</c> each
	/// take the next variable, written with the invariant culture. A width pads a number as C's
	/// <c>printf</c> does, and never shortens it: <c>%3d</c> with spaces, and <c>%03d</c> and
	/// <c>%.3d</c> with zeros after any minus sign. <c>%%</c> becomes one percent sign.</para>
	/// <para>A placeholder with no variable left stays as written, so the mistake is visible, and any
	/// other use of <c>%</c>, such as <c>to sell % for $</c>, is left alone. Text a variable puts in
	/// isn't read for placeholders again.</para>
	/// </remarks>
	/// <param name="value">The text to fill.</param>
	/// <param name="variables">The values for the placeholders, in order.</param>
	/// <returns>The filled text.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">One of <paramref name="variables"/> is <see langword="null"/>.</exception>
	public static string Format(string value, params object?[] variables)
	{
		ArgumentNullException.ThrowIfNull(value);
		return CSTPlaceholders.Fill(value, CSTPlaceholders.ToTexts(variables, nameof(variables)));
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