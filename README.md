# CSTNet

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-v2.0%20adopted-ff69b4.svg)](code_of_conduct.md)

Caret-Separated Text (or CST) is a key-value pair format represented by numbers or words as keys and the value is the string enclosed between carets (^) that contains the contents. Any text which is not enclosed with carets is considered a comment and ignored. Neither strings nor comments may use the caret character.

CSTNet is a library for parsing the CST format.

## Usage

```csharp
#r "nuget:CSTNet,1.0.0"
using System;
using System.IO;
using CSTNet;

var file = File.ReadAllText("example.cst");
var example = CaretSeparatedText.Parse(file, 1);

Console.WriteLine(example);
```

In production, CST files were used in The Sims Online to provide translations. Each translation was split into their respective directories:

- ``uitext/english.dir/hints/_154_miscstrings.cst``
- ``uitext/swedish.dir/hints/_154_miscstrings.cst``

CST.NET only provides the basic parsing functionality.

## To-do

- [ ] Support for arguments (e.g. ``%1``)

## Known issues

- Reading multiple lines 

## Requirements
### Prerequisites

- [.NET](https://dotnet.microsoft.com/download) 5+ or Core 3.1
- [.NET Interactive](https://github.com/dotnet/interactive/blob/main/README.md) for notebooks
    - [VSCode Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode) (does not require Jupyter)
    - [nteract](https://nteract.io/) (requires Jupyter)

## License

I license this project under the MIT license - see [LICENSE](LICENSE) for details.