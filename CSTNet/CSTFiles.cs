using System.Globalization;
using System.Text;

namespace CSTNet;

/// <summary>
/// Reads table files for the wrappers: their names and their text encodings.
/// </summary>
/// <remarks>
/// This avoids LINQ and code page providers, so the wrappers can run where neither has been
/// shown to work, such as a Cosmos kernel.
/// </remarks>
internal static class CSTFiles
{
	/// <summary>The separator in The Sims Online's file names, as in <c>_101_strings.cst</c>.</summary>
	internal const string TSOSeparators = "_";

	/// <summary>The separators in both games' file names, adding SSC's <c>#128#Animations.cst</c>.</summary>
	internal const string AllSeparators = "#_";

	private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

	/// <summary>
	/// The characters Windows-1252 gives bytes 0x80 to 0x9F, where it differs from Latin-1. The five
	/// bytes Windows-1252 leaves undefined map to the control character with the same number, as
	/// .NET's own Windows-1252 encoding does.
	/// </summary>
	private static readonly char[] Windows1252High =
	[
		'€', '\u0081', '‚', 'ƒ', '„', '…', '†', '‡',
		'ˆ', '‰', 'Š', '‹', 'Œ', '\u008D', 'Ž', '\u008F',
		'\u0090', '‘', '’', '“', '”', '•', '–', '—',
		'˜', '™', 'š', '›', 'œ', '\u009D', 'ž', 'Ÿ',
	];

	/// <summary>
	/// Decodes a table file as UTF-8, or as Windows-1252 when it isn't valid UTF-8. These are the two
	/// encodings real tables use. Every byte is valid Windows-1252, so decoding never fails.
	/// </summary>
	internal static string Decode(byte[] bytes)
	{
		try
		{
			return StrictUtf8.GetString(bytes);
		}
		catch (DecoderFallbackException)
		{
			return DecodeWindows1252(bytes);
		}
	}

	internal static string DecodeWindows1252(byte[] bytes)
	{
		var characters = new char[bytes.Length];
		for (var index = 0; index < bytes.Length; index++)
		{
			var value = bytes[index];
			characters[index] = value is >= 0x80 and <= 0x9F ? Windows1252High[value - 0x80] : (char)value;
		}

		return new string(characters);
	}

	/// <summary>
	/// Reads the table ID from a file name such as <c>_101_strings.cst</c> or <c>#128#Animations.cst</c>.
	/// </summary>
	/// <param name="fileName">The file name, without a directory.</param>
	/// <param name="separators">The characters a name may use around its ID.</param>
	/// <param name="id">The table ID when the name follows a pattern; otherwise, 0.</param>
	/// <returns>
	/// <see langword="true"/> for a <c>.cst</c> file, in any case, whose name is a separator, a decimal
	/// ID, and the same separator, followed by any name, including none.
	/// </returns>
	internal static bool TryGetTableId(string fileName, string separators, out int id)
	{
		id = 0;
		if (!string.Equals(Path.GetExtension(fileName), ".cst", StringComparison.OrdinalIgnoreCase))
			return false;

		var stem = Path.GetFileNameWithoutExtension(fileName);
		if (stem.Length == 0 || separators.IndexOf(stem[0]) < 0)
			return false;

		var end = stem.IndexOf(stem[0], 1);
		return end > 1
			&& int.TryParse(stem.AsSpan(1, end - 1), NumberStyles.None, CultureInfo.InvariantCulture, out id);
	}
}
