namespace CSTNet;

/// <summary>
/// A value in a CST table, in the order it appears in the document.
/// </summary>
public sealed class CSTEntry
{
	internal CSTEntry(string? key, string? name, string? value)
	{
		Key = key;
		Name = name;
		Value = value;
	}

	/// <summary>
	/// Gets the key written for this value, or <see langword="null"/> if it has no key.
	/// </summary>
	public string? Key { get; }

	/// <summary>
	/// Gets the other text associated with this value, or <see langword="null"/> if there is none.
	/// </summary>
	public string? Name { get; }

	/// <summary>
	/// Gets the text between the carets, or <see langword="null"/> if the value was never closed.
	/// </summary>
	public string? Value { get; }
}