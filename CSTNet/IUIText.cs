// This project is licensed under the BSD 3-Clause license.
// See the LICENSE file in the project root for more information.
namespace CSTNet;

public interface IUIText
{
    string[] BasePath { get; set; }
    string GetText(int id, int key);
}