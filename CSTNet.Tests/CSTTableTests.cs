namespace CSTNet.Tests;

public class CSTTableTests
{
	[Fact]
	public void ParseSpecificationExample()
	{
		const string content = """
			// A comment. So is a line starting with #, and a /* */ block.
			1 ^A value after its key^
			    2 ^An indented key^
			3
			^A key above its value^
			4 ^A value that runs
			across lines^
			ok button ^A key with spaces^
			kErrNoFood
			^A name above its value^
			A note for translators.

			^A value with a note above it^
			^A value on its own^ with a note after it
			^Another value on its own^
			""";

		var table = CSTTable.Parse(content);

		Assert.Collection(
			table.Entries,
			entry => Assert.Equal(("1", null, "A value after its key"), (entry.Key, entry.Name, entry.Value)),
			entry => Assert.Equal(("2", null, "An indented key"), (entry.Key, entry.Name, entry.Value)),
			entry => Assert.Equal(("3", null, "A key above its value"), (entry.Key, entry.Name, entry.Value)),
			entry => Assert.Equal(("4", null, "A value that runs\nacross lines"), (entry.Key, entry.Name, entry.Value)),
			entry => Assert.Equal(("ok button", null, "A key with spaces"), (entry.Key, entry.Name, entry.Value)),
			entry => Assert.Equal((null, "kErrNoFood", "A name above its value"), (entry.Key, entry.Name, entry.Value)),
			entry => Assert.Equal((null, "A note for translators.", "A value with a note above it"), (entry.Key, entry.Name, entry.Value)),
			entry => Assert.Equal((null, null, "A value on its own"), (entry.Key, entry.Name, entry.Value)),
			entry => Assert.Equal((null, null, "Another value on its own"), (entry.Key, entry.Name, entry.Value)));

		Assert.True(table.TryGetValue("ok button", out var keyedValue));
		Assert.Equal("A key with spaces", keyedValue);
		Assert.True(table.TryGetValue("kErrNoFood", out var namedValue));
		Assert.Equal("A name above its value", namedValue);
		Assert.Empty(table.Problems);
	}

	[Theory]
	[InlineData("\n")]
	[InlineData("\r")]
	[InlineData("\r\n")]
	[InlineData("\u2028")]
	public void NormalizesLineEndingsAndLeadingBom(string lineEnding)
	{
		var content = $"\uFEFF1 ^first{lineEnding}second^";

		var table = CSTTable.Parse(content);

		Assert.True(table.TryGetValue("1", out var value));
		Assert.Equal("first\nsecond", value);
	}

	[Fact]
	public void RemovesIndentedCommentsIncludingCommentsInsideValues()
	{
		const string content = """
			// line comment
			  # another comment
			1 ^first
			// not part of the value
			/* block comment
			still a comment */
			last^
			""";

		var table = CSTTable.Parse(content);

		Assert.True(table.TryGetValue("1", out var value));
		Assert.Equal("first\nlast", value);
		Assert.Empty(table.Problems);
	}

	[Fact]
	public void RetainsMultipleAndEmptyValues()
	{
		const string content = "one ^alpha^ two ^beta^\n^^";

		var table = CSTTable.Parse(content);

		Assert.Equal(3, table.Entries.Count);
		Assert.Equal("alpha", table.Entries[0].Value);
		Assert.Equal("beta", table.Entries[1].Value);
		Assert.Null(table.Entries[2].Key);
		Assert.Equal(string.Empty, table.Entries[2].Value);
		Assert.True(table.TryGetValue("two", out var value));
		Assert.Equal("beta", value);
	}

	[Fact]
	public void MatchesWholeKeysAndPreservesValueText()
	{
		const string content = """
			10 ^  leading
			trailing  ^
			ok button ^A keyed value^
			""";

		var table = CSTTable.Parse(content);

		Assert.True(table.TryGetValue("10", out var value));
		Assert.Equal("  leading\ntrailing  ", value);
		Assert.False(table.TryGetValue("1", out _));
		Assert.True(table.TryGetValue("ok button", out var keyedValue));
		Assert.Equal("A keyed value", keyedValue);
		Assert.False(table.TryGetValue("ok", out _));
	}

	[Fact]
	public void KeysWinOverNamesAndFirstRepeatedKeyWins()
	{
		const string content = """
			code
			^value found by name^
			code ^first keyed value^
			code ^repeated keyed value^
			""";

		var table = CSTTable.Parse(content);

		Assert.True(table.TryGetValue("code", out var value));
		Assert.Equal("first keyed value", value);
		Assert.Collection(
			table.Problems,
			problem =>
			{
				Assert.Equal(CSTProblemKind.RepeatedKey, problem.Kind);
				Assert.Equal(4, problem.Line);
				Assert.Equal("code", problem.Key);
			});
	}

	[Fact]
	public void DistinguishesMissingValuesFromEmptyValues()
	{
		var table = CSTTable.Parse("empty ^^\n");

		Assert.True(table.TryGetValue("empty", out var emptyValue));
		Assert.Equal(string.Empty, emptyValue);
		Assert.False(table.TryGetValue("missing", out var missingValue));
		Assert.Null(missingValue);
	}

	[Fact]
	public void RecoversAfterAnUnclosedValueInTheMiddle()
	{
		const string content = """
			broken ^unfinished
			next
			^recovered^
			last ^usable^
			""";

		var table = CSTTable.Parse(content);

		Assert.Collection(
			table.Entries,
			entry =>
			{
				Assert.Equal("broken", entry.Key);
				Assert.Null(entry.Value);
			},
			entry =>
			{
				Assert.Null(entry.Key);
				Assert.Equal("next", entry.Name);
				Assert.Equal("recovered", entry.Value);
			},
			entry =>
			{
				Assert.Equal("last", entry.Key);
				Assert.Equal("usable", entry.Value);
			});
		Assert.Collection(
			table.Problems,
			problem =>
			{
				Assert.Equal(CSTProblemKind.UnclosedValue, problem.Kind);
				Assert.Equal(1, problem.Line);
			});
	}

	[Fact]
	public void ReportsUnclosedValueAtEndOfDocument()
	{
		var table = CSTTable.Parse("bad ^unfinished\n");

		Assert.Single(table.Entries);
		Assert.Null(table.Entries[0].Value);
		Assert.Collection(
			table.Problems,
			problem =>
			{
				Assert.Equal(CSTProblemKind.UnclosedValue, problem.Kind);
				Assert.Equal(1, problem.Line);
			});
		Assert.False(table.TryGetValue("bad", out _));
	}

	[Fact]
	public void LeavesPlaceholdersAndReferencesUntouched()
	{
		const string reference = "\u0002<cst><cst_id>276</cst_id><string_id>1</string_id></cst>";
		var table = CSTTable.Parse($"1 ^Hello %s %% {reference}^");

		Assert.True(table.TryGetValue("1", out var value));
		Assert.Equal($"Hello %s %% {reference}", value);
	}

	[Fact]
	public void UsesOriginalLineNumbersAfterRemovingComments()
	{
		const string content = """
			// comment
			1 ^first^
			/* block
			comment */
			1 ^second^
			""";

		var table = CSTTable.Parse(content);

		Assert.Collection(
			table.Problems,
			problem =>
			{
				Assert.Equal(CSTProblemKind.RepeatedKey, problem.Kind);
				Assert.Equal(5, problem.Line);
			});
	}
}