using System.Diagnostics.CodeAnalysis;

namespace CSTNet;

/// <summary>
/// Directories of tables laid over one another, looked through in order.
/// </summary>
/// <remarks>
/// <para>Sims Steering Committee lays each person's tables over the global ones: a person's table
/// 130 holds their own animations, and its empty values mean "use the global table's value". A
/// lookup here tries each layer from the first, so put the top layer, such as the person's,
/// first.</para>
/// <para>The layers don't change once created. To add or remove one, such as a mod or a language,
/// create a new <see cref="CSTLayers"/>; the directories themselves are shared, not copied.</para>
/// <code>
/// var layers = new CSTLayers(CSTDirectory.Read(rossPath), CSTDirectory.Read(globalPath));
/// layers.TryGetValueAt(130, 19, out var animation, treatEmptyAsMissing: true);
/// </code>
/// </remarks>
public sealed class CSTLayers
{
	private readonly IReadOnlyList<CSTDirectory> _layers;

	/// <summary>
	/// Creates a stack of directories.
	/// </summary>
	/// <param name="layers">The directories to look through, from the top layer down.</param>
	/// <exception cref="ArgumentNullException"><paramref name="layers"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">One of <paramref name="layers"/> is <see langword="null"/>.</exception>
	public CSTLayers(params CSTDirectory[] layers)
	{
		ArgumentNullException.ThrowIfNull(layers);

		var copy = new CSTDirectory[layers.Length];
		for (var index = 0; index < layers.Length; index++)
			copy[index] = layers[index] ?? throw new ArgumentException($"Layer {index} is null.", nameof(layers));

		_layers = Array.AsReadOnly(copy);
	}

	/// <summary>
	/// Gets the directories to look through, from the top layer down.
	/// </summary>
	public IReadOnlyList<CSTDirectory> Layers => _layers;

	/// <summary>
	/// Tries to find the value for a key or name in the first layer whose table has it.
	/// </summary>
	/// <param name="tableId">The table's ID.</param>
	/// <param name="key">The key or name to find, matched as <see cref="CSTTable.TryGetValue"/> does.</param>
	/// <param name="value">The value when found; otherwise, <see langword="null"/>.</param>
	/// <param name="treatEmptyAsMissing">Whether an empty value means "look in the next layer". By
	/// default an empty value is returned, because most empty values are real empty text.</param>
	/// <returns><see langword="true"/> if a layer has the value; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
	public bool TryGetValue(int tableId, string key, [NotNullWhen(true)] out string? value, bool treatEmptyAsMissing = false)
	{
		ArgumentNullException.ThrowIfNull(key);

		foreach (var layer in _layers)
		{
			if (layer.TryGetTable(tableId, out var table)
				&& table.TryGetValue(key, out value)
				&& !(treatEmptyAsMissing && value.Length == 0))
			{
				return true;
			}
		}

		value = null;
		return false;
	}

	/// <summary>
	/// Tries to find the value at a position in the first layer whose table has a value there.
	/// </summary>
	/// <remarks>
	/// Positions count every entry in a table from 0, as <see cref="CSTTable.Entries"/> does,
	/// including entries with keys and entries never closed. A value never closed counts as missing.
	/// </remarks>
	/// <param name="tableId">The table's ID.</param>
	/// <param name="position">The entry's index among the table's entries, from 0.</param>
	/// <param name="value">The value when found; otherwise, <see langword="null"/>.</param>
	/// <param name="treatEmptyAsMissing">Whether an empty value means "look in the next layer". By
	/// default an empty value is returned, because most empty values are real empty text.</param>
	/// <returns><see langword="true"/> if a layer has a value there; otherwise, <see langword="false"/>.</returns>
	public bool TryGetValueAt(int tableId, int position, [NotNullWhen(true)] out string? value, bool treatEmptyAsMissing = false)
	{
		foreach (var layer in _layers)
		{
			if (layer.TryGetTable(tableId, out var table)
				&& (uint)position < (uint)table.Entries.Count
				&& table.Entries[position].Value is { } entryValue
				&& !(treatEmptyAsMissing && entryValue.Length == 0))
			{
				value = entryValue;
				return true;
			}
		}

		value = null;
		return false;
	}
}
