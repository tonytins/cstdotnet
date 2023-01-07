# CST.NET

<p align="center">
  <a href="https://github.com/tonytins/cstdotnet/blob/main/LICENSE"><img src="https://img.shields.io/github/license/tonytins/cstdotnet" alt="GitHub license"></a>
  <a href="https://github.com/tonytins/cstdotnet/actions?query=workflow%3Adotnet.yml"><img src="https://img.shields.io/github/actions/workflow/status/tonytins/cstdotnet/dotnet.yml" alt="GitHub Workflow Status"></a>
  <img src="https://img.shields.io/github/commit-activity/w/tonytins/cstdotnet" alt="GitHub commit activity">
  <a href="code_of_conduct.md"><img src="https://img.shields.io/badge/Contributor%20Covenant-v2.0%20adopted-ff69b4.svg" alt="Contributor Covenant"></a>
  <img src="https://img.shields.io/codeclimate/maintainability-percentage/tonytins/cstdotnet" alt="Code Climate maintainability">
  <img src="https://img.shields.io/codeclimate/tech-debt/tonytins/cstdotnet" alt="Code Climate technical debt">
</p>


Caret-Separated Text (or CST) is a key-value pair format represented by digits or words as keys and the value as text enclosed between carets. (e.g. ``<key> ^<text>^``) Any text which is not enclosed with carets is considered a comment and ignored. Neither strings nor comments may use the caret character. CST.NET is a library for parsing the CST format.

## Changelog

See [changelog.md](./changelog.md)

## Usage

See [usage.md](./usage.md).

## To-do

- [ ] Support for parameters (e.g. ``%1``)

## Known issues

- Skipping comments is a little unpredictable.

## Requirements

- [.NET](https://dotnet.microsoft.com/download) 6 or later.
- IDEs or Editors
  - [Visual Studio Code](https://code.visualstudio.com/)
  - [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [.NET Interactive](https://github.com/dotnet/interactive/blob/main/README.md) for notebooks (optional).
  - [VSCode Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode) or [nteract](https://nteract.io/).

## License

I license this project under the BSD-3-Clause license - see [LICENSE](LICENSE) for details.
