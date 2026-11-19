// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.
using System.Globalization;

namespace CSTNet;

public class UIText : IUIText
{
	const string MISSING = "***MISSING***";

	/// <summary>
	/// The language of the text.
	/// </summary>
	string Language { get; set; } = "english";

	/// <summary>
	/// The base path used to locate the language files.
	/// </summary>
	public string BasePath { get; set; } = Path.Combine(AppContext.BaseDirectory, "uitext");

	/// <summary>
	/// Constructor for the UIText class.
	/// </summary>
	public UIText() { }

	/// <summary>
	/// Constructor for the UIText class.
	/// Loads the language file for the specified language.
	/// </summary>
	/// <param name="language">Language to load</param>
	public UIText(string language)
	{
		ArgumentNullException.ThrowIfNull(language);
		Language = language;
	}

	/// <summary>
	/// Constructor for the UIText class.
	/// Loads the language file for the specified language and base directory.
	/// </summary>
	/// <param name="language">Language to load</param>
	/// <param name="baseBath">Base directory used to locate the language files.</param>
	public UIText(string language, string baseBath)
	{
		ArgumentNullException.ThrowIfNull(language);
		ArgumentNullException.ThrowIfNull(baseBath);
		Language = language;
		BasePath = baseBath;
	}

	/// <summary>
	/// Gets the text for the given table id and key or zero-based position.
	/// </summary>
	/// <param name="id">The id of the text.</param>
	/// <param name="key">The key or zero-based position of the text.</param>
	/// <returns>The text, or the missing marker if the table or value is missing.</returns>
	/// <exception cref="IOException">A table file or directory cannot be read.</exception>
	/// <exception cref="UnauthorizedAccessException">A table file or directory is inaccessible.</exception>
	public string GetText(int id, int key) => GetText(id, key.ToString(CultureInfo.InvariantCulture));

	/// <summary>
	/// Retrieves the text associated with the given table id and key or zero-based position.
	/// </summary>
	/// <param name="id">The id of the text table.</param>
	/// <param name="key">The key, name, or zero-based position to find.</param>
	/// <returns>The text, or the missing marker if the table or value is missing.</returns>
	/// <exception cref="IOException">A table file or directory cannot be read.</exception>
	/// <exception cref="UnauthorizedAccessException">A table file or directory is inaccessible.</exception>
	public string GetText(int id, string key)
	{
		return GetTextIfPresent(id, key) ?? MISSING;
	}

	/// <summary>
	/// Retrieves the text associated with the given table id and key or zero-based position, or <see langword="null"/> if it is missing.
	/// </summary>
	/// <param name="id">The id of the text table.</param>
	/// <param name="key">The key, name, or zero-based position to find.</param>
	/// <returns>The text for the given key, or <see langword="null"/> if the table or value is missing.</returns>
	/// <exception cref="IOException">A table file or directory cannot be read.</exception>
	/// <exception cref="UnauthorizedAccessException">A table file or directory is inaccessible.</exception>
	public string? GetTextIfPresent(int id, int key) =>
		GetTextIfPresent(id, key.ToString(CultureInfo.InvariantCulture));

	/// <summary>
	/// Retrieves the text associated with the given table id and key, or <see langword="null"/> if it is missing.
	/// </summary>
	/// <param name="id">The id of the text table.</param>
	/// <param name="key">The key, name, or zero-based position to find.</param>
	/// <returns>The text for the given key, or <see langword="null"/> if the table or value is missing.</returns>
	/// <exception cref="IOException">A table file or directory cannot be read.</exception>
	/// <exception cref="UnauthorizedAccessException">A table file or directory is inaccessible.</exception>
	public string? GetTextIfPresent(int id, string key)
	{
		ArgumentNullException.ThrowIfNull(key);

		var table = GetTable(id);
		if (table is null)
			return null;

		var value = GetValue(table, key);
		if (value is null || !TryGetReference(value, out var reference))
			return value;

		var targetTable = GetTable(reference.TableId);
		if (targetTable is null || !targetTable.TryGetValue(reference.Key, out var targetValue))
			return null;

		return TryGetReference(targetValue, out _) ? null : targetValue;
	}

	/// <summary>
	/// Gets the text for the given table id and key or zero-based position, and fills its placeholders.
	/// </summary>
	/// <param name="id">The id of the text table.</param>
	/// <param name="key">The key or zero-based position of the text.</param>
	/// <param name="variables">The values for the placeholders, in order, as <see cref="CST.Format"/> takes them.</param>
	/// <returns>The filled text, or the missing marker if the table or value is missing.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">One of <paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="IOException">A table file or directory cannot be read.</exception>
	/// <exception cref="UnauthorizedAccessException">A table file or directory is inaccessible.</exception>
	public string GetText(int id, int key, params object?[] variables) =>
		GetText(id, key.ToString(CultureInfo.InvariantCulture), variables);

	/// <summary>
	/// Gets the text for the given table id and key, name, or zero-based position, and fills its placeholders.
	/// </summary>
	/// <param name="id">The id of the text table.</param>
	/// <param name="key">The key, name, or zero-based position to find.</param>
	/// <param name="variables">The values for the placeholders, in order, as <see cref="CST.Format"/> takes them.</param>
	/// <returns>The filled text, or the missing marker if the table or value is missing.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">One of <paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="IOException">A table file or directory cannot be read.</exception>
	/// <exception cref="UnauthorizedAccessException">A table file or directory is inaccessible.</exception>
	public string GetText(int id, string key, params object?[] variables) =>
		GetTextIfPresent(id, key, variables) ?? MISSING;

	/// <summary>
	/// Gets the text for the given table id and key or zero-based position and fills its placeholders,
	/// or returns <see langword="null"/> if it is missing.
	/// </summary>
	/// <param name="id">The id of the text table.</param>
	/// <param name="key">The key or zero-based position to find.</param>
	/// <param name="variables">The values for the placeholders, in order, as <see cref="CST.Format"/> takes them.</param>
	/// <returns>The filled text, or <see langword="null"/> if the table or value is missing.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">One of <paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="IOException">A table file or directory cannot be read.</exception>
	/// <exception cref="UnauthorizedAccessException">A table file or directory is inaccessible.</exception>
	public string? GetTextIfPresent(int id, int key, params object?[] variables) =>
		GetTextIfPresent(id, key.ToString(CultureInfo.InvariantCulture), variables);

	/// <summary>
	/// Gets the text for the given table id and key, name, or zero-based position and fills its
	/// placeholders, or returns <see langword="null"/> if it is missing.
	/// </summary>
	/// <remarks>
	/// For a reference to another table, the variables fill the target's text.
	/// </remarks>
	/// <param name="id">The id of the text table.</param>
	/// <param name="key">The key, name, or zero-based position to find.</param>
	/// <param name="variables">The values for the placeholders, in order, as <see cref="CST.Format"/> takes them.</param>
	/// <returns>The filled text, or <see langword="null"/> if the table or value is missing.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">One of <paramref name="variables"/> is <see langword="null"/>.</exception>
	/// <exception cref="IOException">A table file or directory cannot be read.</exception>
	/// <exception cref="UnauthorizedAccessException">A table file or directory is inaccessible.</exception>
	public string? GetTextIfPresent(int id, string key, params object?[] variables)
	{
		var texts = CSTPlaceholders.ToTexts(variables, nameof(variables));

		return GetTextIfPresent(id, key) is { } text ? CSTPlaceholders.Fill(text, texts) : null;
	}

	CSTTable? GetTable(int id)
	{
		var filePath = FindTablePath(id);
		if (filePath is null)
			return null;

		var bytes = File.ReadAllBytes(filePath);
		return CSTTable.Parse(CSTFiles.Decode(bytes));
	}

	string? FindTablePath(int id)
	{
		foreach (var directory in GetSearchDirectories())
		{
			if (!Directory.Exists(directory))
				continue;

			foreach (var filePath in Directory.EnumerateFiles(directory)
				.OrderBy(path => Path.GetFileName(path), StringComparer.Ordinal))
			{
				if (CSTFiles.TryGetTableId(Path.GetFileName(filePath), CSTFiles.TSOSeparators, out var tableId) && tableId == id)
					return filePath;
			}
		}

		return null;
	}

	IEnumerable<string> GetSearchDirectories()
	{
		var basePath = Path.TrimEndingDirectorySeparator(BasePath);
		var directories = new List<string>
		{
			Path.Combine(basePath, $"{Language}.dir"),
			Path.Combine(basePath, "uitext", $"{Language}.dir"),
			Path.Combine(basePath, "uitext.dir")
		};

		if (string.Equals(Path.GetFileName(basePath), "uitext", StringComparison.OrdinalIgnoreCase)
			&& Path.GetDirectoryName(basePath) is { } parentPath)
		{
			directories.Add(Path.Combine(parentPath, "uitext.dir"));
		}

		return directories.Distinct(StringComparer.Ordinal);
	}

	static string? GetValue(CSTTable table, string key)
	{
		if (table.TryGetValue(key, out var value))
			return value;

		if (table.Entries.Any(entry => string.Equals(entry.Key, key, StringComparison.Ordinal)))
			return null;

		if (!int.TryParse(key, NumberStyles.None, CultureInfo.InvariantCulture, out var position)
			|| !string.Equals(key, position.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
			|| (uint)position >= (uint)table.Entries.Count)
		{
			return null;
		}

		var entry = table.Entries[position];
		return entry.Key is null ? entry.Value : null;
	}

	static bool TryGetReference(string value, out Reference reference)
	{
		reference = default;
		const string prefix = "\u0002<cst>";
		if (!value.StartsWith(prefix, StringComparison.Ordinal)
			|| !value.EndsWith("</cst>", StringComparison.Ordinal))
			return false;

		var tableIdText = GetEnclosedText(value, "cst_id");
		var key = GetEnclosedText(value, "string_id");
		if (tableIdText is null
			|| !int.TryParse(tableIdText, NumberStyles.None, CultureInfo.InvariantCulture, out var tableId)
			|| key is null)
		{
			return false;
		}

		reference = new Reference(tableId, key);
		return true;
	}

	static string? GetEnclosedText(string value, string tag)
	{
		var openingTag = $"<{tag}>";
		var closingTag = $"</{tag}>";
		var start = value.IndexOf(openingTag, StringComparison.Ordinal);
		if (start < 0)
			return null;

		start += openingTag.Length;
		var end = value.IndexOf(closingTag, start, StringComparison.Ordinal);
		return end < 0 ? null : value[start..end];
	}

	readonly record struct Reference(int TableId, string Key);
}