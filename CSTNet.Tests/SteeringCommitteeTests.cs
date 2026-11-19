namespace CSTNet.Tests;

/// <summary>
/// Reads a local Sims Steering Committee install, when <c>SSC_GAME_DIR</c> is set. SSC keeps its
/// tables in <c>.dir</c> folders beside its <c>.iff</c> files, named <c>#&lt;id&gt;#&lt;name&gt;.cst</c>.
/// </summary>
public class SteeringCommitteeTests
{
	[Fact]
	public void ReadsEveryTableOfALocalInstall()
	{
		var install = Install();
		var folders = Directory.GetDirectories(install, "*.dir", SearchOption.AllDirectories);
		Assert.NotEmpty(folders);

		var tableCount = 0;
		foreach (var folder in folders)
		{
			var directory = CSTDirectory.Read(folder);
			Assert.Empty(directory.Problems);

			foreach (var (id, table) in directory.Tables)
			{
				Assert.True(table.Problems.Count == 0, $"{folder} table {id} has problems.");
				Assert.All(table.Entries, entry => Assert.NotNull(entry.Value));
				tableCount++;
			}
		}

		var files = Directory.GetFiles(install, "*.cst", new EnumerationOptions { RecurseSubdirectories = true, MatchCasing = MatchCasing.CaseInsensitive });
		Assert.Equal(files.Length, tableCount);
	}

	[Fact]
	public void LaysAPersonOverTheGlobalTablesOfALocalInstall()
	{
		var scenario = Path.Combine(Install(), "DATA", "TDSScen");
		var global = CSTDirectory.Read(Path.Combine(scenario, "Global", "Global.dir"));
		var ross = CSTDirectory.Read(Path.Combine(scenario, "Objects", "ross.dir"));
		var layers = new CSTLayers(ross, global);

		// Ross's own animation replaces the global placeholder in the same slot.
		Assert.Equal("duchess-standing-loop", ValueAt(new CSTLayers(global), 130, 66));
		Assert.Equal("ross-standing-loop", ValueAt(layers, 130, 66));

		// Ross's blank slot is filled in from the global table only when asked.
		Assert.Equal("", ValueAt(layers, 130, 20));
		Assert.Equal("adult-haveaccident", ValueAt(layers, 130, 20, treatEmptyAsMissing: true));

		// Ross has no table 128, so the global one answers.
		Assert.Equal("adult-haveaccident", ValueAt(layers, 128, 1));
	}

	private static string Install()
	{
		var install = Environment.GetEnvironmentVariable("SSC_GAME_DIR");
		if (string.IsNullOrWhiteSpace(install))
			Assert.Skip("SSC_GAME_DIR isn't set.");

		return install;
	}

	private static string? ValueAt(CSTLayers layers, int tableId, int position, bool treatEmptyAsMissing = false) =>
		layers.TryGetValueAt(tableId, position, out var value, treatEmptyAsMissing) ? value : null;
}
