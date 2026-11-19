using System.Text;

namespace CSTNet.Tests;

public class CSTFilesTests
{
	[Theory]
	[InlineData("#128#Animations.cst", 128)]
	[InlineData("_101_strings.cst", 101)]
	[InlineData("#7#.CST", 7)]
	[InlineData("_0_zero.cst", 0)]
	public void ReadsTableIdsFromBothNamingPatterns(string fileName, int id)
	{
		Assert.True(CSTFiles.TryGetTableId(fileName, CSTFiles.AllSeparators, out var actual));
		Assert.Equal(id, actual);
	}

	[Theory]
	[InlineData("gamecompdrawacard.cst")]
	[InlineData("#128.cst")]
	[InlineData("#12a#name.cst")]
	[InlineData("##name.cst")]
	[InlineData("_1#name.cst")]
	[InlineData("#128#Animations.txt")]
	[InlineData("_-1_negative.cst")]
	[InlineData("_99999999999_overflow.cst")]
	public void IgnoresOtherFileNames(string fileName)
	{
		Assert.False(CSTFiles.TryGetTableId(fileName, CSTFiles.AllSeparators, out _));
	}

	[Fact]
	public void TheSimsOnlineNamesIgnoreSteeringCommitteeNames()
	{
		Assert.True(CSTFiles.TryGetTableId("_101_strings.cst", CSTFiles.TSOSeparators, out _));
		Assert.False(CSTFiles.TryGetTableId("#128#Animations.cst", CSTFiles.TSOSeparators, out _));
	}

	[Fact]
	public void DecodesUtf8BeforeWindows1252()
	{
		Assert.Equal("café", CSTFiles.Decode(Encoding.UTF8.GetBytes("café")));
		Assert.Equal("caf€", CSTFiles.Decode([(byte)'c', (byte)'a', (byte)'f', 0x80]));
	}

	[Fact]
	public void DecodesEveryByteAsDotNetsWindows1252Does()
	{
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		var windows1252 = Encoding.GetEncoding(1252);
		var bytes = new byte[256];
		for (var index = 0; index < bytes.Length; index++)
			bytes[index] = (byte)index;

		Assert.Equal(windows1252.GetString(bytes), CSTFiles.DecodeWindows1252(bytes));
	}
}
