# CST.NET

Caret-Separated Text (or CST) is a key-value pair format represented by numbers or words as keys and the value is the string enclosed between carets (^) that contains the contents. CST.NET is a library for prasing the CST format.

## Example

```csharp
using System;
using System.IO;
using CSTNet;

var file = File.ReadAllText("example.cst");
var example = CaretSeparatedText.Parse(file, 1);

Console.WriteLine(example);
```

## To-do

- [ ] Support for arguments

## License

I license this project under the MIT license - see [LICENSE](LICENSE) for details.