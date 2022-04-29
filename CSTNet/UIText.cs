// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.

namespace CSTNet;

public class UIText
{
    string Language { get; set; } = "english";
    public string[] BasePath { get; set; } = { AppContext.BaseDirectory, "uitext" };

    public UIText() { }

    public UIText(string language)
    {
        Language = language;
    }

    public UIText(string language, params string[] baseBath)
    {
        Language = language;
        BasePath = baseBath;
    }

    public string GetText(int id, int key) => GetText(id, key.ToString());

    public string GetText(int id, string key)
    {
        var basePath = Path.Combine(BasePath);
        var langPath = Path.Combine(basePath, $"{Language}.dir");
        var files = Directory.GetFiles(langPath);

        foreach (var file in files)
        {
            if (!file.Contains(".cst"))
                continue;

            var ids = Path.GetFileName(file);
            var second = ids.IndexOf("_", 1, StringComparison.InvariantCultureIgnoreCase);

            if (second == -1)
                continue;

            ids = ids.Substring(1, second - 1);

            if (ids != id.ToString())
                continue;

            var content = File.ReadAllText(file);
            return CaretSeparatedText.Parse(content, key);
        }

        return "***MISSING***";
    }
}
