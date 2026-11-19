using System.Globalization;
using System.Text;

namespace CSTNet.Tests;

public class PlaceholderTests
{
	[Fact]
	public void PreservesUnmatchedPlaceholders()
	{
		Assert.Equal("Messages 7 004 %1", CST.Parse("mail ^%s %d %03d %1^", "mail", "Messages", 7, 4));
	}

	[Fact]
	public void PreservesUnknownAndIncompletePlaceholders()
	{
		Assert.Equal("%q %s %", CST.Parse("note ^%q %s %^", "note", Array.Empty<object>()));
	}

	[Fact]
	public void DoesNotRescanInsertedVariables()
	{
		Assert.Equal("%d", CST.Parse("note ^%s^", "note", "%d"));
	}

	[Theory]
	[InlineData("%s")]
	[InlineData("%d")]
	[InlineData("%i")]
	[InlineData("%u")]
	[InlineData("%ld")]
	[InlineData("%li")]
	[InlineData("%lu")]
	public void PlaceholdersTakeAVariable(string placeholder)
	{
		Assert.Equal("[7]", CST.Parse($"1 ^[{placeholder}]^", 1, 7));
	}

	[Theory]
	[InlineData("%03d", "007")]
	[InlineData("%.2d", "07")]
	[InlineData("%03i", "007")]
	[InlineData("%.2u", "07")]
	[InlineData("%02ld", "07")]
	[InlineData("%3d", "  7")]
	[InlineData("%3i", "  7")]
	[InlineData("%3ld", "  7")]
	public void NumberPlaceholdersPadLikePrintf(string placeholder, string expected)
	{
		Assert.Equal(expected, CST.Parse($"1 ^{placeholder}^", 1, 7));
	}

	[Theory]
	[InlineData("%3d", " -5")]
	[InlineData("%03d", "-05")]
	[InlineData("%.3d", "-005")]
	[InlineData("%2d", "-5")]
	[InlineData("%02d", "-5")]
	[InlineData("%.1d", "-5")]
	[InlineData("%3s", "-5")]
	public void PaddingKeepsANegativeSignInFront(string placeholder, string expected)
	{
		Assert.Equal(expected, CST.Parse($"1 ^{placeholder}^", 1, -5));
	}

	[Fact]
	public void PaddingNeverShortensANumber()
	{
		Assert.Equal("1234 1234 1234", CST.Parse("1 ^%3d %03d %.3d^", 1, 1234, 1234, 1234));
	}

	[Fact]
	public void DoubledPercentBecomesOnePercentWithoutUsingAVariable()
	{
		Assert.Equal("%d 7%", CST.Parse("1 ^%%d %d%%^", 1, 7));
		Assert.Equal("100%", CST.Format("100%%"));
	}

	[Theory]
	[InlineData("%.d")]
	[InlineData("%ls")]
	[InlineData("%l")]
	[InlineData("%.")]
	[InlineData("%5")]
	[InlineData("to sell % for $")]
	[InlineData("<%>")]
	[InlineData("%x")]
	public void OtherUsesOfPercentStayAsWritten(string text)
	{
		Assert.Equal(text, CST.Parse($"1 ^{text}^", 1, 7));
	}

	[Fact]
	public void FormatFillsAValueFromATable()
	{
		Assert.True(CSTTable.Parse("1 ^%s has %d^").TryGetValue("1", out var value));

		Assert.Equal("Bob has 2", CST.Format(value, "Bob", 2));
		Assert.Equal("only one %s", CST.Format("%s %s", "only one"));
	}

	[Fact]
	public void LookupsWithoutVariablesLeavePlaceholdersUntouched()
	{
		Assert.Equal("100%% of %s", CST.Parse("1 ^100%% of %s^", 1));
		Assert.Equal("***MISSING***", CST.Parse("1 ^%s^", 2, "unused"));
	}

	[Fact]
	public void WritesVariablesWithTheInvariantCulture()
	{
		var culture = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
			Assert.Equal("1.5 -3", CST.Format("%s %d", 1.5, -3));
		}
		finally
		{
			CultureInfo.CurrentCulture = culture;
		}
	}

	[Fact]
	public void WidthsPastTheLimitPadNothing()
	{
		Assert.Equal(new string(' ', 999) + "7", CST.Format("%1000d", 7));
		Assert.Equal("7", CST.Format("%1001d", 7));
		Assert.Equal("7", CST.Format("%2000000000d", 7));
		Assert.Equal("7", CST.Format("%99999999999999999999d", 7));
	}

	[Fact]
	public void RejectsNullVariablesBeforeLookingUp()
	{
		Assert.Throws<ArgumentNullException>(() => CST.Format("%s", null!));
		Assert.Throws<ArgumentException>(() => CST.Format("%s", "one", null));
		Assert.Throws<ArgumentException>(() => CST.Parse("1 ^%s^", 2, "one", null));
	}

	[Fact]
	public void UITextFillsTextAndReferenceTargets()
	{
		var directory = Path.Combine(Path.GetTempPath(), $"CSTNet.Tests-{Guid.NewGuid():N}", "english.dir");
		Directory.CreateDirectory(directory);
		try
		{
			const string reference = "\u0002<cst><cst_id>2</cst_id><string_id>1</string_id><string arg=\"1\">%s</string></cst>";
			File.WriteAllText(Path.Combine(directory, "_1_refs.cst"), $"1 ^You have %d new %s.^\n2 ^{reference}^\n", new UTF8Encoding(false));
			File.WriteAllText(Path.Combine(directory, "_2_targets.cst"), "1 ^Hello %s^\n", new UTF8Encoding(false));
			var text = new UIText("english", Path.GetDirectoryName(directory)!);

			Assert.Equal("You have 5 new messages.", text.GetText(1, 1, 5, "messages"));
			Assert.Equal("You have 5 new messages.", text.GetTextIfPresent(1, "1", 5, "messages"));
			Assert.Equal("Hello World", text.GetTextIfPresent(1, 2, "World"));
			Assert.Equal("You have %d new %s.", text.GetTextIfPresent(1, 1));
			Assert.Null(text.GetTextIfPresent(1, 3, "unused"));
			Assert.Equal("***MISSING***", text.GetText(1, 3, "unused"));
			Assert.Throws<ArgumentException>(() => text.GetTextIfPresent(1, 3, (object?)null));
		}
		finally
		{
			Directory.Delete(Path.GetDirectoryName(directory)!, recursive: true);
		}
	}
}
