using System.Text;

namespace CSTNet.Tests;

public class UITextTests
{
	[Fact]
	public void PreservesLegacyMissingMarkerAndOffersDistinctMissingResult()
	{
		using var temporaryDirectory = new TemporaryDirectory();
		WriteTable(temporaryDirectory.Path, "english.dir", "_100_text.cst", "1 ^found^\n2 ^***MISSING***^\n");
		var text = new UIText("english", temporaryDirectory.Path);

		Assert.Equal("found", text.GetText(100, 1));
		Assert.Equal("***MISSING***", text.GetText(100, "missing"));
		Assert.Null(text.GetTextIfPresent(100, "missing"));
		Assert.Equal("***MISSING***", text.GetTextIfPresent(100, 2));
	}

	[Fact]
	public void UsesZeroBasedPositionsOnlyWhenNoExplicitKeyExists()
	{
		using var temporaryDirectory = new TemporaryDirectory();
		WriteTable(
			temporaryDirectory.Path,
			"english.dir",
			"_101_positions.cst",
			"""
				0 ^explicit zero^
				name above
				^named position one^
				^position two^
				broken ^unfinished
				next
				^position four^
				""");
		var text = new UIText("english", temporaryDirectory.Path);

		Assert.Equal("explicit zero", text.GetTextIfPresent(101, 0));
		Assert.Equal("named position one", text.GetTextIfPresent(101, 1));
		Assert.Equal("position two", text.GetTextIfPresent(101, 2));
		Assert.Null(text.GetTextIfPresent(101, 3));
		Assert.Null(text.GetTextIfPresent(101, "broken"));
		Assert.Equal("position four", text.GetTextIfPresent(101, 4));
	}

	[Fact]
	public void FindsTablesInBothSupportedDirectoryLayouts()
	{
		using var temporaryDirectory = new TemporaryDirectory();
		WriteTable(temporaryDirectory.Path, Path.Combine("uitext", "english.dir"), "_102_new.CST", "key ^new layout^\n");
		WriteTable(temporaryDirectory.Path, "uitext.dir", "_103_old.cst", "key ^old layout^\n");
		WriteTable(temporaryDirectory.Path, "uitext.dir", "un-numbered.cst", "key ^wrong file^\n");
		WriteTable(temporaryDirectory.Path, "uitext.dir", "_999_other.cst", "key ^wrong table^\n");
		var text = new UIText("english", temporaryDirectory.Path);

		Assert.Equal("new layout", text.GetTextIfPresent(102, "key"));
		Assert.Equal("old layout", text.GetTextIfPresent(103, "key"));
		Assert.Null(text.GetTextIfPresent(9999, "key"));

		var legacyBasePath = Path.Combine(temporaryDirectory.Path, "uitext");
		var legacyText = new UIText("english", legacyBasePath);
		Assert.Equal("old layout", legacyText.GetTextIfPresent(103, "key"));
	}

	[Fact]
	public void FallsBackToWindows1252ForNonUtf8Files()
	{
		using var temporaryDirectory = new TemporaryDirectory();
		var languageDirectory = Path.Combine(temporaryDirectory.Path, "english.dir");
		Directory.CreateDirectory(languageDirectory);
		var path = Path.Combine(languageDirectory, "_104_encoding.cst");
		File.WriteAllBytes(path, [(byte)'1', (byte)' ', (byte)'^', (byte)'c', (byte)'a', (byte)'f', 0x80, (byte)'^']);
		var text = new UIText("english", temporaryDirectory.Path);

		Assert.Equal("caf€", text.GetTextIfPresent(104, 1));
	}

	[Fact]
	public void ResolvesOneReferenceAndDoesNotFollowASecond()
	{
		using var temporaryDirectory = new TemporaryDirectory();
		const string referenceToTable11 = "\u0002<cst><cst_id>11</cst_id><string_id>1</string_id></cst>";
		const string referenceToTable12 = "\u0002<cst><cst_id>12</cst_id><string_id>1</string_id></cst>";
		WriteTable(
			temporaryDirectory.Path,
			"english.dir",
			"_10_source.cst",
			$"direct ^{referenceToTable11}^\nnested ^{referenceToTable12}^\n");
		WriteTable(temporaryDirectory.Path, "english.dir", "_11_target.cst", "1 ^resolved %s^\n");
		WriteTable(temporaryDirectory.Path, "english.dir", "_12_reference.cst", $"1 ^{referenceToTable11}^\n");
		var text = new UIText("english", temporaryDirectory.Path);

		Assert.Equal("resolved %s", text.GetTextIfPresent(10, "direct"));
		Assert.Null(text.GetTextIfPresent(10, "nested"));
	}

	private static void WriteTable(string basePath, string relativeDirectory, string fileName, string content)
	{
		var directory = Path.Combine(basePath, relativeDirectory);
		Directory.CreateDirectory(directory);
		File.WriteAllText(Path.Combine(directory, fileName), content, new UTF8Encoding(false));
	}

	private sealed class TemporaryDirectory : IDisposable
	{
		public TemporaryDirectory()
		{
			Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"CSTNet.Tests-{Guid.NewGuid():N}");
			Directory.CreateDirectory(Path);
		}

		public string Path { get; }

		public void Dispose() => Directory.Delete(Path, recursive: true);
	}
}