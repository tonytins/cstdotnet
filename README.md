# 🔠 CST.NET

CST.NET is a .NET library for parsing Maxis' key-value pair format. It can be used with custom frameworks or the original `UIText` APIs.

## What is CST?

Caret-Separated Text (or CST) is a string-text format: a plain-text resource format with digits or words as keys and each value enclosed between carets. (e.g. `<key> ^<value>^`) Maxis used it as early as Sims Steering Committee, a prototype of The Sims, for animation, file, and texture names, and later in The Sims Online for its interface text. It's handy on its own for any project that wants a simple table of strings or a clear path to translation.

## Features

- [x] TSO compatible, checked against the beta, base game, and New & Improved
    - [x] Values with no key, and names above values
    - [x] Keys with spaces, and indented keys
    - [x] Variables
    - [x] UTF-8 and Windows-1252 files
    - [x] References between tables
- [ ] Sims Steering Committee compatible
    - [x] Numbered keys such as `1.label` and `1:`
    - [ ] `.dir` folders of `#<id>#<name>.cst` tables, laid over one another
- [x] Multiline values
- [x] C-style comments
- [x] Malformed entries reported rather than stopping the read
- [x] UIText

The format is described in [the CST specification](https://github.com/tonytins/swift-cst/blob/main/doc/CST.md).

## Installation

```sh
dotnet add package CSTNet
```

## Usage

```csharp
using CSTNet;

var table = CSTTable.Parse("1 ^Hello, %s!^\nok button ^Accept^");

if (table.TryGetValue("1", out var hello))
    Console.WriteLine(CST.Format(hello, "World")); // Hello, World!

Console.WriteLine(table.Entries.Count); // Every value, in file order
Console.WriteLine(table.Problems.Count); // Anything malformed
```

`TryGetValue` returns `false` for a missing key, so it can't be mistaken for real text. A key must match whole, so `ok` doesn't find `ok button`. Values come back as written, and `CST.Format` fills placeholders such as `%s` and `%03d` in order.

For a one-off lookup, `CST.Parse` returns the value, or `***MISSING***` for a missing key:

```csharp
var accept = CST.Parse(content, "ok button");
```

### UIText

`UIText` looks up a key in a table named `_<id>_<name>.cst`, the way The Sims Online does. Pass an install's `gamedata` directory, and it finds the layout itself, whether New & Improved's `uitext/<language>.dir` or the earlier `uitext.dir`. It reads UTF-8 and Windows-1252 files, finds values with no key by their position from 0, and follows references between tables.

```csharp
var text = new UIText("english", "/path/to/TSOClient/gamedata");

if (text.GetTextIfPresent(154, 1) is { } greeting)
    Console.WriteLine(greeting);
```

`GetTextIfPresent` returns `null` for a missing table or key, while `GetText` returns `***MISSING***`. Filesystem errors are thrown to the caller. Each lookup reads the table again, so for many lookups in one table, use `CSTTable`.

See the [usage guide](doc/README.md) for how keys are found, problems, and `UIText`'s limits, and the [changelog](changelog.md) for what changed in each release.

## Supported Versions

| CSTNet | Minimum .NET Version |
| ------ | -------------------- |
| `2.1.200 ...` | .NET 10 |
| `2.0.100 ... 2.0.103` | .NET 6 or .NET Standard 2.1 |

## Background

This all began as a "what if?" What if I could made my own CST parser? It was an attempt to create my first algorithm. One that was a little better than came before. So, I did. First from a Jupyter notebook and then a full-fledged project that's taken on a life of it's own!

## License

I license this project under BSD-3-Clause license — see the [LICENSE](LICENSE) file for full text.
