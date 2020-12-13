# Usage

## Basic Parsing

```text
1 ^The quick brown fox jumps over the lazy dog.^
```

```csharp
#r "nuget:Sixam.CST,1.1"
using System;
using System.IO;
using Sixam.CST;

var file = File.ReadAllText("example.cst");
var example = CaretSeparatedText.Parse(file, 1);

Console.WriteLine(example);
```

See working example on [.NET Fiddle](https://dotnetfiddle.net/ecKb2h).

## Complex Scenarios

In production, CST files were used in The Sims Online (TSO) to provide translations. It was required that they were prefixed with numbers enclosed in underscores, known as the ID. The IDs were used to locate the right file without knowing it's name. Meanwhile, each translation was split into their respective ``uitext/<languae>.dir`` directories:

- ``uitext/english.dir/_154_miscstrings.cst``
- ``uitext/swedish.dir/_154_miscstrings.cst``[^1]

Starting with 1.1, the UIText class provides methods that directorly map to these directories relative to the application's.

```csharp
#r "nuget:Sixam.CST,1.1"
using Sixam.CST;

var english = new UIText(); // UIText assumes English
var swedish = new UIText("swedish");
var engExample = english.GetText(101, 1); // english.dir/_101_example.cst
var sweExample = swedish.GetText(101, 1); // swedish.dir/_101_example.cst

Console.WriteLine(engExample);
Console.WriteLine(sweExample);
```

Note that GetText() remains unchanged because the translation is expected to be same id and key, despite being in a different directory.

### Changing base directories

If you want to change the base directory, you can. Though, this is still a work in progress and not recommended.

```csharp
#r "nuget:Sixam.CST,1.1"
using Sixam.CST;

var example = new UIText()
{
    BasePath = new[] { "gamedata", "uitext" },
};
```

[^1]: TSO only supported English.