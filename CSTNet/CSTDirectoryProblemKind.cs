namespace CSTNet;

/// <summary>
/// Describes a problem found while reading a directory of tables.
/// </summary>
public enum CSTDirectoryProblemKind
{
	/// <summary>
	/// Another file already has this table ID, so this file was skipped. Files are read in order
	/// of their names, so the first name wins.
	/// </summary>
	RepeatedTable
}
