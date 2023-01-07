// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.
namespace CSTNet;

public interface IUIText
{
    /// <summary>
    /// The base directory for the language files.
    /// </summary>
    string[] BasePath { get; set; }

    /// <summary>
    /// Get the text for the given id and key.
    /// </summary>
    /// <param name="id">The id of the text.</param>
    /// <param name="key">The key of the text.</param>
    /// <returns>The text for the given id and key.</returns>
    string GetText(int id, int key);
}
