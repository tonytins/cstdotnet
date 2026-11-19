using System.Text;

namespace CSTNet;

/// <summary>
/// Fills a value's placeholders in order, following C's <c>printf</c> for the ones real tables use.
/// </summary>
/// <remarks>
/// <c>%s</c>, <c>%d</c>, <c>%i</c>, <c>%u</c>, and <c>%ld</c>, <c>%li</c>, <c>%lu</c> each take a
/// variable. A width pads a number: <c>%3d</c> with spaces, and <c>%03d</c> and <c>%.3d</c> with
/// zeros after any minus sign. <c>%%</c> becomes one percent sign. A placeholder with no variable
/// left, and any other use of <c>%</c>, stays as written. A width over <see cref="MaxWidth"/> pads
/// nothing.
/// </remarks>
internal static class CSTPlaceholders
{
	/// <summary>The widest padding filled in. Real tables pad to 2 or 3 characters.</summary>
	internal const int MaxWidth = 1000;

	private enum Padding
	{
		None,
		Spaces, // %3d: at least this many characters, filled with spaces.
		Zeros, // %03d: at least this many characters including any sign, zeros after the sign.
		Digits, // %.3d: at least this many digits not including any sign.
	}

	/// <summary>
	/// Writes each variable as text with the invariant culture, so a number reads the same on every
	/// system.
	/// </summary>
	/// <exception cref="ArgumentNullException"><paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">One of <paramref name="variables"/> is <see langword="null"/>.</exception>
	internal static string[] ToTexts(object?[] variables, string parameterName)
	{
		ArgumentNullException.ThrowIfNull(variables, parameterName);

		var texts = new string[variables.Length];
		for (var index = 0; index < variables.Length; index++)
		{
			texts[index] = variables[index] is { } variable
				? Convert.ToString(variable, System.Globalization.CultureInfo.InvariantCulture) ?? ""
				: throw new ArgumentException($"Variable {index} is null.", parameterName);
		}

		return texts;
	}

	internal static string Fill(string template, string[] variables)
	{
		var result = new StringBuilder(template.Length);
		var nextVariable = 0;
		var index = 0;

		while (index < template.Length)
		{
			var character = template[index];
			if (character != '%')
			{
				result.Append(character);
				index++;
				continue;
			}

			if (index + 1 < template.Length && template[index + 1] == '%')
			{
				result.Append('%');
				index += 2;
				continue;
			}

			if (!TryReadPlaceholder(template, index + 1, out var end, out var padding, out var width))
			{
				result.Append('%');
				index++;
				continue;
			}

			if (nextVariable < variables.Length)
				result.Append(Pad(variables[nextVariable++], padding, width));
			else
				result.Append(template, index, end - index);

			index = end;
		}

		return result.ToString();
	}

	/// <summary>
	/// Reads an optional <c>.</c>, a width, an optional <c>l</c>, and a conversion character, starting
	/// right after the <c>%</c>.
	/// </summary>
	private static bool TryReadPlaceholder(string template, int start, out int end, out Padding padding, out int width)
	{
		end = start;
		padding = Padding.None;
		width = 0;

		var index = start;
		var hasPrecision = index < template.Length && template[index] == '.';
		if (hasPrecision)
			index++;

		var digitsStart = index;
		while (index < template.Length && template[index] is >= '0' and <= '9')
			index++;
		var digitCount = index - digitsStart;

		var hasLongModifier = index < template.Length && template[index] == 'l';
		if (hasLongModifier)
			index++;

		if (index >= template.Length)
			return false;

		var conversion = template[index];
		var isNumber = conversion is 'd' or 'i' or 'u';
		if (!(isNumber || (conversion == 's' && !hasLongModifier)) || (hasPrecision && digitCount == 0))
			return false;

		end = index + 1;
		if (!isNumber)
			return true;

		// A width past MaxWidth can't be real padding, so it pads nothing rather than running out of memory.
		if (!int.TryParse(template.AsSpan(digitsStart, digitCount), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out width) || width > MaxWidth)
			width = 0;

		padding = hasPrecision
			? Padding.Digits
			: digitCount > 0 && template[digitsStart] == '0' ? Padding.Zeros : Padding.Spaces;
		return true;
	}

	private static string Pad(string value, Padding padding, int width)
	{
		var sign = value.StartsWith('-') ? "-" : "";
		var digits = value[sign.Length..];

		return padding switch
		{
			Padding.Spaces => new string(' ', Math.Max(0, width - value.Length)) + value,
			Padding.Zeros => sign + new string('0', Math.Max(0, width - value.Length)) + digits,
			Padding.Digits => sign + new string('0', Math.Max(0, width - digits.Length)) + digits,
			_ => value,
		};
	}
}
