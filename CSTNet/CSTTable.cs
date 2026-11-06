using System.Diagnostics.CodeAnalysis;

namespace CSTNet;

/// <summary>
/// The values of a CST document in file order, and any problems found reading them.
/// </summary>
public sealed class CSTTable
{
	private readonly IReadOnlyDictionary<string, int> _entryIndexes;
	private readonly IReadOnlyList<CSTEntry> _entries;
	private readonly IReadOnlyList<CSTProblem> _problems;

	private CSTTable(List<CSTEntry> entries, List<CSTProblem> problems)
	{
		_entries = Array.AsReadOnly(entries.ToArray());
		_problems = Array.AsReadOnly(problems.ToArray());

		var entryIndexes = new Dictionary<string, int>(StringComparer.Ordinal);
		var keys = new HashSet<string>(StringComparer.Ordinal);
		for (var index = 0; index < _entries.Count; index++)
		{
			var entry = _entries[index];
			if (entry.Key is null || !keys.Add(entry.Key))
				continue;

			if (entry.Value is not null)
				entryIndexes.Add(entry.Key, index);
		}

		for (var index = 0; index < _entries.Count; index++)
		{
			var entry = _entries[index];
			if (entry.Value is not null && entry.Name is not null && !keys.Contains(entry.Name))
				entryIndexes.TryAdd(entry.Name, index);
		}

		_entryIndexes = entryIndexes;
	}

	/// <summary>
	/// Gets every value in file order, including values that were never closed.
	/// </summary>
	public IReadOnlyList<CSTEntry> Entries => _entries;

	/// <summary>
	/// Gets the problems found while reading the document.
	/// </summary>
	public IReadOnlyList<CSTProblem> Problems => _problems;

	/// <summary>
	/// Reads a CST document into a table.
	/// </summary>
	/// <param name="content">The text of the CST document.</param>
	/// <returns>The parsed table, including any malformed entries and diagnostics.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="content"/> is <see langword="null"/>.</exception>
	public static CSTTable Parse(string content)
	{
		ArgumentNullException.ThrowIfNull(content);

		var scanner = new CSTScanner(content);
		return new CSTTable(scanner.Entries, scanner.Problems);
	}

	/// <summary>
	/// Tries to find a closed value by its key or name.
	/// </summary>
	/// <param name="key">The key or name to find.</param>
	/// <param name="value">The value when found; otherwise, <see langword="null"/>.</param>
	/// <returns><see langword="true"/> if a closed value was found; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
	public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
	{
		ArgumentNullException.ThrowIfNull(key);

		if (_entryIndexes.TryGetValue(key, out var index))
		{
			if (_entries[index].Value is { } entryValue)
			{
				value = entryValue;
				return true;
			}
		}

		value = null;
		return false;
	}
}