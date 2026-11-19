namespace CSTNet;

/// <summary>
/// Something wrong with a directory of tables that didn't stop it being read.
/// </summary>
/// <remarks>
/// Problems inside a table are in that table's <see cref="CSTTable.Problems"/>.
/// </remarks>
public sealed class CSTDirectoryProblem
{
	internal CSTDirectoryProblem(CSTDirectoryProblemKind kind, int tableId, string fileName)
	{
		Kind = kind;
		TableId = tableId;
		FileName = fileName;
	}

	/// <summary>
	/// Gets the kind of problem.
	/// </summary>
	public CSTDirectoryProblemKind Kind { get; }

	/// <summary>
	/// Gets the ID of the table the problem is about.
	/// </summary>
	public int TableId { get; }

	/// <summary>
	/// Gets the name of the file the problem is about, without its directory.
	/// </summary>
	public string FileName { get; }
}
