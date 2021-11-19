# Usage

## Basic Parsing

```text
1 ^The quick brown fox jumps over the lazy dog.^
```

```csharp
#r "nuget:Sixam.CST,1.1.100" // If using notebooks
using Sixam.CST;

var file = File.ReadAllText("example.cst");
var example = CaretSeparatedText.Parse(file, 1);

Console.WriteLine(example);
```

## Complex Scenarios

In production, CST files were used in The Sims Online to provide translations It was required that they were prefixed with numbers enclosed in underscores, known as the ID. The IDs were used to locate the right file without knowing it's name. Meanwhile, each translation was split into their respective ``uitext/<languae>.dir`` directories:

- ``uitext/english.dir/_154_miscstrings.cst``
- ``uitext/swedish.dir/_154_miscstrings.cst``

Starting with 1.1, the UIText class provides methods that directorly map to these directories relative to the application's.

```csharp
#r "nuget:Sixam.CST,1.1.100"
using Sixam.CST;

var english = new UIText(); // UIText assumes English
var swedish = new UIText("swedish");
var engExample = english.GetText(152, 1); // english.dir/_154_miscstrings.cst
var sweExample = swedish.GetText(152, 1); // swedish.dir/_154_miscstrings.cst

Console.WriteLine(engExample);
Console.WriteLine(sweExample);
```

Note that the IDs and keys in both ``GetText()`` examples remains the same. This is because it was historically assumed that the English and translations files were identical. However, the original never saw any new languages added.

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
