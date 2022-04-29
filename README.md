# CSTNet

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-v2.0%20adopted-ff69b4.svg)](code_of_conduct.md)

Caret-Separated Text (or CST) is a key-value pair format represented by digits or words as keys and the value as text enclosed between carets. (e.g. ``<key> ^<text>^``) Any text which is not enclosed with carets is considered a comment and ignored. Neither strings nor comments may use the caret character. CST.NET is a library for parsing the CST format.

CST.NET uses .NET's built-in indexing extension function to accomplish locating of each respective key. As a consequence, it does not matter what you use for keys. I added an additional normalization to the pipeline that converts the document's line endings to the system's, in order to prevent crashes.

## Usage

See [usage.md](./usage.md).

## To-do

- [ ] Support for arguments (e.g. ``%1``)

## Known issues

- Skipping comments is a little unpredictable.

## Requirements

- [.NET](https://dotnet.microsoft.com/download) 6 or later.
- [.NET Interactive](https://github.com/dotnet/interactive/blob/main/README.md) for notebooks (optional).
    - [VSCode Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode) or [nteract](https://nteract.io/).

## License

I license this project under the MIT license - see [LICENSE](LICENSE) for details.