using System.Globalization;
using System.Text;

namespace CSTNet.Tests;

public class InstalledTableTests
{
	[Fact]
	public void ReadsEveryTableFromAvailableLocalInstalls()
	{
		var installs = new List<(string Name, string Root)>();
		foreach (var name in new[] { "TSO_GAME_DIR", "TSO_CLASSIC_DIR", "TSO_BETA_DIR" })
		{
			var root = Environment.GetEnvironmentVariable(name);
			if (!string.IsNullOrWhiteSpace(root))
				installs.Add((name, root));
		}

		if (installs.Count == 0)
			Assert.Skip("No local TSO installation directory is configured.");

		foreach (var (name, root) in installs)
		{
			var gameData = Path.Combine(root, "gamedata");
			var newLayout = Path.Combine(gameData, "uitext", "english.dir");
			var tableDirectory = Directory.Exists(newLayout) ? newLayout : Path.Combine(gameData, "uitext.dir");
			Assert.True(Directory.Exists(tableDirectory), $"No CST directory was found for {name}.");

			var text = new UIText("english", gameData);
			foreach (var path in Directory.EnumerateFiles(tableDirectory)
				.OrderBy(filePath => Path.GetFileName(filePath), StringComparer.Ordinal))
			{
				if (!TryGetTableId(Path.GetFileName(path), out var id))
					continue;

				var table = CSTTable.Parse(Decode(File.ReadAllBytes(path)));
				Assert.DoesNotContain(table.Problems, problem => problem.Kind == CSTProblemKind.UnclosedValue);

				var firstEntry = table.Entries.FirstOrDefault();
				if (firstEntry?.Value is not null && !firstEntry.Value.StartsWith("\u0002", StringComparison.Ordinal))
				{
					var expected = firstEntry.Value;
					if (firstEntry.Key is null && table.TryGetValue("0", out var explicitKey))
						expected = explicitKey;

					var actual = firstEntry.Key is null
						? text.GetTextIfPresent(id, 0)
						: text.GetTextIfPresent(id, firstEntry.Key);
					Assert.True(
						string.Equals(expected, actual, StringComparison.Ordinal),
						$"{name} table {id} did not return its first value.");
				}

				for (var index = 0; index < table.Entries.Count; index++)
				{
					var entry = table.Entries[index];
					if (entry.Value is null || !entry.Value.StartsWith("\u0002", StringComparison.Ordinal))
						continue;

					var key = entry.Key ?? index.ToString(CultureInfo.InvariantCulture);
					Assert.NotNull(text.GetTextIfPresent(id, key));
				}
			}
		}
	}

	[Fact]
	public void ReadsEveryTableDirectoryOfAvailableLocalInstalls()
	{
		var checkedInstalls = 0;
		foreach (var name in new[] { "TSO_GAME_DIR", "TSO_CLASSIC_DIR", "TSO_BETA_DIR" })
		{
			var root = Environment.GetEnvironmentVariable(name);
			if (string.IsNullOrWhiteSpace(root))
				continue;

			var gameData = Path.Combine(root, "gamedata");
			var newLayout = Path.Combine(gameData, "uitext", "english.dir");
			var tableDirectory = Directory.Exists(newLayout) ? newLayout : Path.Combine(gameData, "uitext.dir");

			var directory = CSTDirectory.Read(tableDirectory);
			Assert.Empty(directory.Problems);

			var fileCount = 0;
			foreach (var path in Directory.EnumerateFiles(tableDirectory))
			{
				if (!TryGetTableId(Path.GetFileName(path), out var id))
					continue;

				fileCount++;
				var expected = CSTTable.Parse(Decode(File.ReadAllBytes(path)));
				Assert.True(directory.TryGetTable(id, out var actual), $"{name} table {id} is missing.");
				Assert.Equal(
					expected.Entries.Select(entry => (entry.Key, entry.Name, entry.Value)),
					actual.Entries.Select(entry => (entry.Key, entry.Name, entry.Value)));
			}

			Assert.Equal(fileCount, directory.Tables.Count);
			checkedInstalls++;
		}

		if (checkedInstalls == 0)
			Assert.Skip("No local TSO installation directory is configured.");
	}

	private static string Decode(byte[] bytes)
	{
		try
		{
			return new UTF8Encoding(false, true).GetString(bytes);
		}
		catch (DecoderFallbackException)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			return Encoding.GetEncoding(1252).GetString(bytes);
		}
	}

	private static bool TryGetTableId(string fileName, out int id)
	{
		id = default;
		if (!string.Equals(Path.GetExtension(fileName), ".cst", StringComparison.OrdinalIgnoreCase))
			return false;

		var stem = Path.GetFileNameWithoutExtension(fileName);
		if (stem.Length == 0 || stem[0] != '_')
			return false;

		var separator = stem.IndexOf('_', 1);
		return separator > 1
			&& separator < stem.Length - 1
			&& int.TryParse(
				stem.AsSpan(1, separator - 1),
				NumberStyles.None,
				CultureInfo.InvariantCulture,
				out id);
	}
}