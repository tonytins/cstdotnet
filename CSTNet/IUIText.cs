// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.
namespace CSTNet;

public interface IUIText
{
	/// <summary>
	/// The base directory for the language files.
	/// </summary>
	string BasePath { get; set; }

	/// <summary>
	/// Gets the text for the given table id and key or zero-based position.
	/// </summary>
	/// <param name="id">The id of the text.</param>
	/// <param name="key">The key or zero-based position of the text.</param>
	/// <returns>The text, or the missing marker if the table or value is missing.</returns>
	string GetText(int id, int key);
}