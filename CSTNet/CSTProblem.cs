namespace CSTNet;

/// <summary>
/// A problem found while reading a CST table.
/// </summary>
public sealed class CSTProblem
{
	internal CSTProblem(CSTProblemKind kind, int line, string? key)
	{
		Kind = kind;
		Line = line;
		Key = key;
	}

	/// <summary>
	/// Gets the kind of problem.
	/// </summary>
	public CSTProblemKind Kind { get; }

	/// <summary>
	/// Gets the 1-based source line where the problem was found.
	/// </summary>
	public int Line { get; }

	/// <summary>
	/// Gets the repeated key, or <see langword="null"/> when the value was unclosed.
	/// </summary>
	public string? Key { get; }
}