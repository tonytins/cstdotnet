namespace CSTNet;

/// <summary>
/// Describes a problem found while reading a CST table.
/// </summary>
public enum CSTProblemKind
{
	/// <summary>
	/// A value was opened but never closed.
	/// </summary>
	UnclosedValue,

	/// <summary>
	/// A key was already used by an earlier value.
	/// </summary>
	RepeatedKey
}