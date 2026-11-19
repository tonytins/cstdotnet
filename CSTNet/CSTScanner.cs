namespace CSTNet;

internal sealed class CSTScanner
{
	private const char CARET = '^';

	private readonly HashSet<string> _seenKeys = new(StringComparer.Ordinal);
	private string? _lineAbove;
	private OpenValue? _openValue;

	internal CSTScanner(string content)
	{
		Entries = [];
		Problems = [];

		foreach (var (line, text) in ContentLines(content))
			ScanLine(text, line);

		if (_openValue is not null)
			AddUnclosedValue(_openValue);
	}

	internal List<CSTEntry> Entries { get; }

	internal List<CSTProblem> Problems { get; }

	private static IEnumerable<(int Line, string Text)> ContentLines(string content)
	{
		if (content.StartsWith('\uFEFF'))
			content = content[1..];

		content = content
			.Replace("\r\n", "\n", StringComparison.Ordinal)
			.Replace('\r', '\n')
			.Replace('\u2028', '\n');

		var lines = content.Split('\n');
		var isInBlockComment = false;
		for (var index = 0; index < lines.Length; index++)
		{
			var line = lines[index];
			var trimmed = line.TrimStart();

			if (isInBlockComment)
			{
				if (line.Contains("*/", StringComparison.Ordinal))
					isInBlockComment = false;

				continue;
			}

			if (trimmed.StartsWith("/*", StringComparison.Ordinal))
			{
				isInBlockComment = !line.Contains("*/", StringComparison.Ordinal);
				continue;
			}

			if (trimmed.StartsWith("//", StringComparison.Ordinal) || trimmed.StartsWith('#'))
				continue;

			yield return (index + 1, line);
		}
	}

	private void ScanLine(string text, int line)
	{
		var rest = text;
		var hasValue = false;

		if (_openValue is not null)
		{
			var caret = rest.IndexOf(CARET);
			if (caret < 0)
			{
				var trimmed = rest.Trim();
				_openValue = _openValue with
				{
					Text = string.Concat(_openValue.Text, "\n", rest),
					LastTextLine = trimmed.Length == 0 ? _openValue.LastTextLine : trimmed
				};
				return;
			}

			var openValue = _openValue;
			_openValue = null;

			if (HoldsWholeEntry(rest))
			{
				AddUnclosedValue(openValue);
				_lineAbove = openValue.LastTextLine;
			}
			else
			{
				AddValue(
					string.Concat(openValue.Text, "\n", rest[..caret]),
					openValue.Key,
					openValue.Name,
					openValue.Line);
				rest = rest[(caret + 1)..];
				hasValue = true;
			}
		}

		while (true)
		{
			var caret = rest.IndexOf(CARET);
			if (caret < 0)
				break;

			hasValue = true;
			var (key, name) = TakeKeyAndName(rest[..caret]);
			rest = rest[(caret + 1)..];

			var end = rest.IndexOf(CARET);
			if (end < 0)
			{
				_openValue = new OpenValue(key, name, line, rest, null);
				return;
			}

			AddValue(rest[..end], key, name, line);
			rest = rest[(end + 1)..];
		}

		var remainingText = rest.Trim();
		if (!hasValue && remainingText.Length > 0)
			_lineAbove = remainingText;
	}

	private static bool HoldsWholeEntry(string line)
	{
		var caretCount = 0;
		var lastCaret = -1;
		for (var index = 0; index < line.Length; index++)
		{
			if (line[index] != CARET)
				continue;

			caretCount++;
			lastCaret = index;
		}

		if (caretCount != 2)
			return false;

		for (var index = lastCaret + 1; index < line.Length; index++)
		{
			if (!char.IsWhiteSpace(line[index]))
				return false;
		}

		return true;
	}

	private (string? Key, string? Name) TakeKeyAndName(string text)
	{
		var lineAbove = _lineAbove;
		_lineAbove = null;

		var key = text.Trim();
		if (key.Length > 0)
			return TryTakeNumberedKey(key, out var numbered) ? numbered : (key, null);

		if (lineAbove is null)
			return (null, null);

		if (IsAllDigits(lineAbove))
			return (lineAbove, null);

		return TryTakeNumberedKey(lineAbove, out var numberedAbove) ? numberedAbove : (null, lineAbove);
	}

	/// <summary>
	/// Splits a number followed by a period or colon, such as <c>1.new house window name</c>,
	/// <c>5.</c>, or <c>1:</c>, into the number as a key and the rest, if any, as a name.
	/// </summary>
	/// <remarks>
	/// A digit right after the separator means the text is a number such as <c>1.5</c> or a time
	/// such as <c>10:30</c>, not a numbered key.
	/// </remarks>
	private static bool TryTakeNumberedKey(string text, out (string? Key, string? Name) result)
	{
		result = default;

		var digitCount = 0;
		while (digitCount < text.Length && IsDigit(text[digitCount]))
			digitCount++;

		if (digitCount == 0 || digitCount == text.Length || text[digitCount] is not ('.' or ':'))
			return false;

		var rest = text[(digitCount + 1)..];
		if (rest.Length > 0 && IsDigit(rest[0]))
			return false;

		var name = rest.Trim();
		result = (text[..digitCount], name.Length == 0 ? null : name);
		return true;
	}

	private static bool IsDigit(char character) => character is >= '0' and <= '9';

	// A loop rather than LINQ, which a Cosmos kernel hasn't been shown to run.
	private static bool IsAllDigits(string text)
	{
		foreach (var character in text)
		{
			if (!IsDigit(character))
				return false;
		}

		return text.Length > 0;
	}

	private void AddValue(string value, string? key, string? name, int line)
	{
		Entries.Add(new CSTEntry(key, name, value));

		if (key is not null && !_seenKeys.Add(key))
			Problems.Add(new CSTProblem(CSTProblemKind.RepeatedKey, line, key));
	}

	private void AddUnclosedValue(OpenValue openValue)
	{
		Entries.Add(new CSTEntry(openValue.Key, openValue.Name, null));
		Problems.Add(new CSTProblem(CSTProblemKind.UnclosedValue, openValue.Line, null));

		if (openValue.Key is not null && !_seenKeys.Add(openValue.Key))
			Problems.Add(new CSTProblem(CSTProblemKind.RepeatedKey, openValue.Line, openValue.Key));
	}

	private sealed record OpenValue(string? Key, string? Name, int Line, string Text, string? LastTextLine);
}