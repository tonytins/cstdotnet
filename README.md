# Sixam.CST

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-v2.0%20adopted-ff69b4.svg)](code_of_conduct.md)

Caret-Separated Text (or CST) is a key-value pair format represented by digits or words as keys and the value as text enclosed between carets. (e.g. ``<key> ^<text>^``) Any text which is not enclosed with carets is considered a comment and ignored. Neither strings nor comments may use the caret character. Sixam.CST is a library for parsing the CST format.

## Usage

```text
1 ^The quick brown fox jumps over the lazy dog.^
```

```csharp
#r "nuget:CSTNet,1.0.2"
using System;
using System.IO;
using CSTNet;

var file = File.ReadAllText("example.cst");
var example = CaretSeparatedText.Parse(file, 1);

Console.WriteLine(example);
```

See working example on [.NET Fiddle](https://dotnetfiddle.net/ecKb2h).

In production, CST files were used in The Sims Online (TSO) to provide translations. Each translation was split into their respective directories:

- ``uitext/english.dir/_154_miscstrings.cst``
- ``uitext/swedish.dir/_154_miscstrings.cst``

Sixam.CST only provides the basic parsing functionality.

## To-do

- [ ] Support for arguments (e.g. ``%1``)

## Known issues

- Skipping comments is a little buggy.

## Requirements
### Prerequisites

- [.NET](https://dotnet.microsoft.com/download) 5+ or Core 3.1
- [.NET Interactive](https://github.com/dotnet/interactive/blob/main/README.md) for notebooks
    - [VSCode Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode) (does not require Jupyter)
    - [nteract](https://nteract.io/) (requires Jupyter)

## License

I license this project under the MIT license - see [LICENSE](LICENSE) for details.