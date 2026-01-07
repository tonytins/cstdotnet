// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.
namespace CSTNet;

public class UIText : IUIText
{
	/// <summary>
	/// The language of the text.
	/// </summary>
	string Language { get; set; } = "english";

	/// <summary>
	/// The base directory for the language files.
	/// </summary>
	public string BasePath { get; set; } = Path.Combine(AppContext.BaseDirectory, "uitext");

	/// <summary>
	/// Constructor for the UIText class.
	/// </summary>
	public UIText() { }

	/// <summary>
	/// Constructor for the UIText class.
	/// Loads the language file for the specified language.
	/// </summary>
	/// <param name="language">Language to load</param>
	public UIText(string language)
	{
		Language = language;
	}

	/// <summary>
	/// Constructor for the UIText class.
	/// Loads the language file for the specified language and base directory.
	/// </summary>
	/// <param name="language">Language to load</param>
	/// <param name="baseBath">Base directory for the language files.</param>
	public UIText(string language, string baseBath)
	{
		Language = language;
		BasePath = baseBath;
	}

	/// <summary>
	/// Get the text for the given id and key.
	/// </summary>
	/// <param name="id">The id of the text.</param>
	/// <param name="key">The key of the text.</param>
	/// <returns>The text for the given id and key.</returns>
	public string GetText(int id, int key) => GetText(id, key.ToString());

	/// <summary>
	/// Retrieves the text associated with the given id and key from the language files.
	/// </summary>
	/// <param name="id">The id to search for.</param>
	/// <param name="key">The key to parse from the content.</param>
	/// <returns>The parsed text if found, otherwise "***MISSING***".</returns>
	public string GetText(int id, string key)
	{
		var langPath = Path.Combine(BasePath, $"{Language}.dir");

		var files = Directory.GetFiles(langPath).Where(file =>
		file.Contains(".cst") && Path.GetFileName(file).Split("_")[1] == id.ToString());

		return files.Any() ? CST.Parse(File.ReadAllText(files.First()), key) : "***MISSING***";
	}
}

