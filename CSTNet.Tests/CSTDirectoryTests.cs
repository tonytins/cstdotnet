using System.Text;

namespace CSTNet.Tests;

public class CSTDirectoryTests
{
	[Fact]
	public void ReadsEveryTableInADirectory()
	{
		using var directory = new TemporaryDirectory(new()
		{
			["#128#Animations.cst"] = Encoding.UTF8.GetBytes("^adult-tantrum^\tAnimMotiveTantrum"),
			["_101_strings.cst"] = Encoding.UTF8.GetBytes("1 ^one^"),
			["notes.txt"] = Encoding.UTF8.GetBytes("not a table"),
			["credits.cst"] = Encoding.UTF8.GetBytes("1 ^ignored^"),
		});

		var tables = CSTDirectory.Read(directory.Path);

		Assert.Equal([101, 128], tables.Tables.Keys.Order());
		Assert.True(tables.TryGetTable(128, out var animations));
		Assert.Equal("adult-tantrum", animations.Entries[0].Value);
		Assert.True(tables.TryGetTable(101, out var strings));
		Assert.True(strings.TryGetValue("1", out var one));
		Assert.Equal("one", one);
		Assert.False(tables.TryGetTable(999, out _));
		Assert.Empty(tables.Problems);
	}

	[Fact]
	public void KeepsTheFirstFileForARepeatedId()
	{
		using var directory = new TemporaryDirectory(new()
		{
			["#5#first.cst"] = Encoding.UTF8.GetBytes("^first^"),
			["#5#second.cst"] = Encoding.UTF8.GetBytes("^second^"),
		});

		var tables = CSTDirectory.Read(directory.Path);

		Assert.True(tables.TryGetTable(5, out var table));
		Assert.Equal("first", table.Entries[0].Value);
		var problem = Assert.Single(tables.Problems);
		Assert.Equal((CSTDirectoryProblemKind.RepeatedTable, 5, "#5#second.cst"), (problem.Kind, problem.TableId, problem.FileName));
	}

	[Fact]
	public void DecodesWindows1252()
	{
		using var directory = new TemporaryDirectory(new() { ["_1_credits.cst"] = [0x31, 0x20, 0x5E, 0xE9, 0x5E] });

		Assert.True(CSTDirectory.Read(directory.Path).TryGetTable(1, out var table));
		Assert.True(table.TryGetValue("1", out var value));
		Assert.Equal("é", value);
	}

	[Fact]
	public void IgnoresSubdirectories()
	{
		using var directory = new TemporaryDirectory(new() { ["_1_top.cst"] = Encoding.UTF8.GetBytes("1 ^top^") });
		var nested = Path.Combine(directory.Path, "_2_nested.cst");
		Directory.CreateDirectory(nested);

		Assert.Equal([1], CSTDirectory.Read(directory.Path).Tables.Keys);
	}

	[Fact]
	public void ThrowsForAMissingDirectory()
	{
		var missing = Path.Combine(Path.GetTempPath(), $"CSTNet.Tests-{Guid.NewGuid():N}");

		Assert.Throws<DirectoryNotFoundException>(() => CSTDirectory.Read(missing));
		Assert.Throws<ArgumentNullException>(() => CSTDirectory.Read(null!));
	}

	[Fact]
	public void CopiesTablesBuiltInMemory()
	{
		var source = new Dictionary<int, CSTTable> { [1] = CSTTable.Parse("1 ^one^") };
		var tables = new CSTDirectory(source);
		source[2] = CSTTable.Parse("1 ^two^");

		Assert.Equal([1], tables.Tables.Keys);
		Assert.Throws<ArgumentException>(() => new CSTDirectory(new Dictionary<int, CSTTable> { [1] = null! }));
	}

	internal sealed class TemporaryDirectory : IDisposable
	{
		public TemporaryDirectory(Dictionary<string, byte[]> files)
		{
			Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"CSTNet.Tests-{Guid.NewGuid():N}");
			Directory.CreateDirectory(Path);
			foreach (var (name, bytes) in files)
				File.WriteAllBytes(System.IO.Path.Combine(Path, name), bytes);
		}

		public string Path { get; }

		public void Dispose() => Directory.Delete(Path, recursive: true);
	}
}

public class CSTLayersTests
{
	private readonly CSTDirectory _person = new(new Dictionary<int, CSTTable>
	{
		[130] = CSTTable.Parse("""
			^^
			^ross-standing-loop^
			name ^ross^
			blank ^^
			"""),
	});

	private readonly CSTDirectory _global = new(new Dictionary<int, CSTTable>
	{
		[130] = CSTTable.Parse("""
			^adult-haveaccident^
			^duchess-standing-loop^
			name ^global^
			blank ^global blank^
			only ^only global^
			"""),
		[128] = CSTTable.Parse("^global only table^"),
	});

	[Fact]
	public void TheTopLayerWins()
	{
		var layers = new CSTLayers(_person, _global);

		Assert.Equal("ross-standing-loop", ValueAt(layers, 130, 1));
		Assert.Equal("ross", Value(layers, 130, "name"));
	}

	[Fact]
	public void EmptyValuesAreReturnedUnlessTheCallerAsks()
	{
		var layers = new CSTLayers(_person, _global);

		Assert.Equal("", ValueAt(layers, 130, 0));
		Assert.Equal("", Value(layers, 130, "blank"));
		Assert.Equal("adult-haveaccident", ValueAt(layers, 130, 0, treatEmptyAsMissing: true));
		Assert.Equal("global blank", Value(layers, 130, "blank", treatEmptyAsMissing: true));
	}

	[Fact]
	public void MissingValuesAndTablesFallThrough()
	{
		var layers = new CSTLayers(_person, _global);

		Assert.Equal("only global", Value(layers, 130, "only"));
		Assert.Equal("only global", ValueAt(layers, 130, 4));
		Assert.Equal("global only table", ValueAt(layers, 128, 0));
	}

	[Fact]
	public void MissesReturnFalse()
	{
		var layers = new CSTLayers(_person, _global);

		Assert.Null(Value(layers, 130, "absent"));
		Assert.Null(ValueAt(layers, 130, 5));
		Assert.Null(ValueAt(layers, 130, -1));
		Assert.Null(ValueAt(layers, 999, 0));
		Assert.Null(ValueAt(new CSTLayers(), 130, 0));
		Assert.Null(Value(layers, 130, "", treatEmptyAsMissing: true));
	}

	[Fact]
	public void AnUnclosedValueFallsThrough()
	{
		var broken = new CSTDirectory(new Dictionary<int, CSTTable> { [130] = CSTTable.Parse("^never closed") });

		Assert.Equal("adult-haveaccident", ValueAt(new CSTLayers(broken, _global), 130, 0));
	}

	[Fact]
	public void ANewStackLeavesTheOthersAsTheyWere()
	{
		var both = new CSTLayers(_person, _global);
		var globalOnly = new CSTLayers(both.Layers[1]);

		Assert.Equal("duchess-standing-loop", ValueAt(globalOnly, 130, 1));
		Assert.Equal("ross-standing-loop", ValueAt(both, 130, 1));
	}

	[Fact]
	public void CopiesItsLayersAndRejectsNull()
	{
		CSTDirectory[] directories = [_person, _global];
		var layers = new CSTLayers(directories);
		directories[0] = _global;

		Assert.Same(_person, layers.Layers[0]);
		Assert.Throws<ArgumentNullException>(() => new CSTLayers(null!));
		Assert.Throws<ArgumentException>(() => new CSTLayers(_person, null!));
		Assert.Throws<ArgumentNullException>(() => layers.TryGetValue(130, null!, out _));
	}

	[Fact]
	public void LaysTablesReadFromDisk()
	{
		using var person = new CSTDirectoryTests.TemporaryDirectory(new() { ["#130#ross.cst"] = Encoding.UTF8.GetBytes("^^\n^ross-standing-loop^") });
		using var global = new CSTDirectoryTests.TemporaryDirectory(new() { ["#130#Global.cst"] = Encoding.UTF8.GetBytes("^adult-haveaccident^\n^duchess-standing-loop^") });

		var layers = new CSTLayers(CSTDirectory.Read(person.Path), CSTDirectory.Read(global.Path));

		Assert.Equal("ross-standing-loop", ValueAt(layers, 130, 1));
		Assert.Equal("adult-haveaccident", ValueAt(layers, 130, 0, treatEmptyAsMissing: true));
	}

	private static string? Value(CSTLayers layers, int tableId, string key, bool treatEmptyAsMissing = false) =>
		layers.TryGetValue(tableId, key, out var value, treatEmptyAsMissing) ? value : null;

	private static string? ValueAt(CSTLayers layers, int tableId, int position, bool treatEmptyAsMissing = false) =>
		layers.TryGetValueAt(tableId, position, out var value, treatEmptyAsMissing) ? value : null;
}
