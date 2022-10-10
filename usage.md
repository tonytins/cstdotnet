# Usage

## Basic Parsing

If you want to create your own internal CST parsing framework, you can use the `CST` or `CaretSeparatedText` class directly.

```text
1 ^The quick brown fox jumps over the lazy dog.^
```

```csharp
#r "nuget:CSTNet,1.0.400-beta1" // If using notebooks
using CSTNet;

var file = File.ReadAllText("example.cst");
var example = CST.Parse(file, 1);

// "The quick brown fox jumps over the lazy dog."
Console.WriteLine(example);
```

## In Production

The Sims Online, and by extension FreeSO, are the only known example of CST files ever being used in production. CST.NET's APIs is based on FreeSO's and is meant to function both as drop-in replacement and general purpose API.

```csharp
#r "nuget:CSTNet,1.0.300"
using CSTNet;

var english = new UIText(); // UIText assumes English
var swedish = new UIText("swedish");
var engExample = english.GetText(152, 1); // english.dir/_154_miscstrings.cst
var sweExample = swedish.GetText(152, 1); // swedish.dir/_154_miscstrings.cst

Console.WriteLine(engExample);
Console.WriteLine(sweExample);
```

In The Sims Online, it was required translation were prefixed with numbers enclosed in underscores, known as the ID. The IDs were used to locate the right file without having to remember it's name. Meanwhile, each translation was split into their respective ``uitext/<languae>.dir`` directories:

- ``uitext/english.dir/_154_miscstrings.cst``
- ``uitext/swedish.dir/_154_miscstrings.cst``

Note that the ``UIText`` class uses the above mentioned ``CST.Parse()`` method internally. Any changes made to the CST class.

### Changing base directories

If you want to change the base directory, you can. Though, this is still a work in progress and not recommended.

```csharp
#r "nuget:CSTNet,1.0.300"
using CSTNet;

var example = new UIText()
{
    BasePath = new[] { "gamedata", "uitext" },
};
```
