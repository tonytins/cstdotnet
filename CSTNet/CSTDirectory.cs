using System.Diagnostics.CodeAnalysis;

namespace CSTNet;

/// <summary>
/// The tables kept as loose files in one directory, read all at once.
/// </summary>
/// <remarks>
/// <para>Each table is a file named <c>#&lt;id&gt;#&lt;name&gt;.cst</c>, as Sims Steering Committee
/// writes them in folders such as <c>Global.dir</c>, or <c>_&lt;id&gt;_&lt;name&gt;.cst</c>, as The
/// Sims Online does. Other files, and subdirectories, are ignored. Every table is read when the
/// directory is, so a <see cref="CSTDirectory"/> never touches the disk again.</para>
/// <para>To look through several directories in order, such as a person's tables over the global
/// ones, use <see cref="CSTLayers"/>.</para>
/// </remarks>
public sealed class CSTDirectory
{
	private readonly Dictionary<int, CSTTable> _tables;
	private readonly IReadOnlyList<CSTDirectoryProblem> _problems;

	/// <summary>
	/// Creates a directory from tables already in memory, such as tables a program builds itself.
	/// </summary>
	/// <param name="tables">The tables by their IDs. They're copied, so later changes to the
	/// dictionary don't reach the directory.</param>
	/// <exception cref="ArgumentNullException"><paramref name="tables"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">One of <paramref name="tables"/> is <see langword="null"/>.</exception>
	public CSTDirectory(IReadOnlyDictionary<int, CSTTable> tables)
	{
		ArgumentNullException.ThrowIfNull(tables);

		_tables = new Dictionary<int, CSTTable>(tables.Count);
		foreach (var (id, table) in tables)
			_tables.Add(id, table ?? throw new ArgumentException($"Table {id} is null.", nameof(tables)));

		_problems = [];
	}

	private CSTDirectory(Dictionary<int, CSTTable> tables, List<CSTDirectoryProblem> problems)
	{
		_tables = tables;
		_problems = problems.AsReadOnly();
	}

	/// <summary>
	/// Gets the tables by their IDs.
	/// </summary>
	public IReadOnlyDictionary<int, CSTTable> Tables => _tables;

	/// <summary>
	/// Gets the problems found while reading the directory.
	/// </summary>
	public IReadOnlyList<CSTDirectoryProblem> Problems => _problems;

	/// <summary>
	/// Reads every table in a directory.
	/// </summary>
	/// <remarks>
	/// A file is decoded as UTF-8, or as Windows-1252 when it isn't valid UTF-8, the two encodings
	/// real tables use. Reading a table never fails, so the only errors are a directory or file that
	/// can't be read.
	/// </remarks>
	/// <param name="path">The directory holding the tables.</param>
	/// <returns>The directory's tables, and any problems found reading them.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
	/// <exception cref="DirectoryNotFoundException">The directory doesn't exist.</exception>
	/// <exception cref="IOException">The directory or one of its tables can't be read.</exception>
	/// <exception cref="UnauthorizedAccessException">The directory or one of its tables is inaccessible.</exception>
	public static CSTDirectory Read(string path)
	{
		ArgumentNullException.ThrowIfNull(path);

		// GetFiles and Array.Sort rather than LINQ, which a Cosmos kernel hasn't been shown to run.
		var files = Directory.GetFiles(path);
		Array.Sort(files, StringComparer.Ordinal);

		var tables = new Dictionary<int, CSTTable>();
		var problems = new List<CSTDirectoryProblem>();
		foreach (var file in files)
		{
			var fileName = Path.GetFileName(file);
			if (!CSTFiles.TryGetTableId(fileName, CSTFiles.AllSeparators, out var id))
				continue;

			if (tables.ContainsKey(id))
			{
				problems.Add(new CSTDirectoryProblem(CSTDirectoryProblemKind.RepeatedTable, id, fileName));
				continue;
			}

			tables.Add(id, CSTTable.Parse(CSTFiles.Decode(File.ReadAllBytes(file))));
		}

		return new CSTDirectory(tables, problems);
	}

	/// <summary>
	/// Tries to find the table with an ID.
	/// </summary>
	/// <param name="id">The table's ID.</param>
	/// <param name="table">The table when found; otherwise, <see langword="null"/>.</param>
	/// <returns><see langword="true"/> if the directory has the table; otherwise, <see langword="false"/>.</returns>
	public bool TryGetTable(int id, [NotNullWhen(true)] out CSTTable? table) =>
		_tables.TryGetValue(id, out table);
}
