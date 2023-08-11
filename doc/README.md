# CST.NET

Caret-Separated Text (or CST) is a key-value pair format represented by digits or words as keys and the value as text enclosed between carets. (e.g. ``<key> ^<text>^``) Any text which is not enclosed with carets is considered a comment and ignored. Neither strings nor comments may use the caret character. CST.NET is a library for parsing the CST format.

## Usage

### Basic Parsing

If you want to create your own internal CST parsing framework, you can use the `CST` or `CaretSeparatedText` class directly.

```text
1 ^The quick brown fox jumps over the lazy dog.^
```

```csharp
#r "nuget:CSTNet,2.0.103"
using CSTNet;

var file = File.ReadAllText("example.cst");
var example = CST.Parse(file, 1);

// "The quick brown fox jumps over the lazy dog."
Console.WriteLine(example);
```

### In Production

```csharp
#r "nuget:CSTNet,2.0.103"
using CSTNet;

var english = new UIText(); // UIText assumes English
var swedish = new UIText("swedish");
var engExample = english.GetText(152, 1); // english.dir/_154_miscstrings.cst
var sweExample = swedish.GetText(152, 1); // swedish.dir/_154_miscstrings.cst

Console.WriteLine(engExample);
Console.WriteLine(sweExample);
```