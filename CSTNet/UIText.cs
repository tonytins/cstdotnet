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
    public string[] BasePath { get; set; } = { AppContext.BaseDirectory, "uitext" };

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
    /// <param name="basePath">Base directory for the language files.</param>
    public UIText(string language, params string[] baseBath)
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
    /// Get the text for the given id and key.
    /// </summary>
    /// <param name="id">The id of the text.</param>
    /// <param name="key">The key of the text.</param>
    /// <returns>The text for the given id and key.</returns>
    public string GetText(int id, string key)
    {
        // Combine the base path and language path to get the full path of the language file.
        var basePath = Path.Combine(BasePath);
        var langPath = Path.Combine(basePath, $"{Language}.dir");

        // Get all the files in the language directory.
        var files = Directory.GetFiles(langPath);

        // Iterate through the files in the language directory.
        foreach (var file in files)
        {
            // Skip files that do not have the ".cst" extension.
            if (!file.Contains(".cst"))
                continue;

            // Get the id of the current file.
            var ids = Path.GetFileName(file);
            var second = ids.IndexOf("_", 1, StringComparison.InvariantCultureIgnoreCase);

            if (second == -1)
                continue;

            ids = ids.Substring(1, second - 1);

            // If the id of the current file does not match the id passed to the function,
            // skip to the next file.
            if (ids != id.ToString())
                continue;

            // Read the content of the current file.
            var content = File.ReadAllText(file);

            // Return the text for the specified key.
            return CST.Parse(content, key);
        }

        // If no text is found, return a default string.
        return "***MISSING***";
    }

}

