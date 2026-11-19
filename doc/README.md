# Using CST.NET

CST.NET reads Caret-Separated Text (`.cst`) tables, the plain-text format The Sims Online
(TSO) uses for its interface text. Each value sits between two carets, and you find it by
its key:

```text
// Buttons for the purchase dialog
1 ^Buy^
2 ^Cancel^
ok button ^Accept^
kErrNoFunds
^You don't have enough money for that.^
3 ^Welcome back, %s!^
```

CST.NET follows the [CST specification](https://github.com/tonytins/swift-cst/blob/main/doc/CST.md),
which describes how real tables behave in detail.

## Install

CST.NET needs .NET 10 or later.

```sh
dotnet add package CSTNet
```

## Look up a value

Read the file, parse it with `CSTTable.Parse`, and look values up with `TryGetValue`:

```csharp
using CSTNet;

var table = CSTTable.Parse(File.ReadAllText("_101_purchase.cst"));

if (table.TryGetValue("2", out var cancel))
    Console.WriteLine(cancel); // Cancel

if (!table.TryGetValue("99", out _))
    Console.WriteLine("No value has the key 99.");
```

`TryGetValue` returns `false` when nothing matches, so a missing value can't be mistaken
for real text. An empty value, `^^`, is found and returns an empty string.

Keys match whole and are case-sensitive: `ok button` finds `Accept`, but `ok` and
`OK button` find nothing.

To load the files of a TSO install, see [Read text from a TSO install](#read-text-from-a-tso-install).
It handles TSO's file names, folders, and text encodings for you.

## How values get their keys

A value's key comes from the text before its opening caret:

| In the file | Found by |
| --- | --- |
| `1 ^Buy^` | `1` |
| `ok button ^Accept^` | `ok button`, the whole text |
| `3` on one line, then `^Done^` on the next | `3` |
| `kErrNoFunds` on one line, then `^...^` on the next | `kErrNoFunds`, as the value's name |
| `5.Purchase title` on one line, then `^...^` on the next | `5`, or the name `Purchase title` |
| `6: ^Sell^` | `6` |
| `^Buy^` with no text before it or on the line above | Only by position, through `Entries` or `UIText` |

Text on the line above a value only counts if it's the nearest non-blank line, and blank
lines and comments in between don't matter. If several values use the same key, the first
one wins. A key always wins over a name, wherever each appears in the file.

Lines starting with `//` or `#` are comments, and so is anything from a line starting with
`/*` to the line holding `*/`. Text outside carets that isn't a key or name, such as a note
after a closing caret, is ignored.

## Read every value in order

`Entries` lists every value in file order, including values with no key:

```csharp
foreach (var entry in table.Entries)
    Console.WriteLine($"{entry.Key ?? "-"} | {entry.Name ?? "-"} | {entry.Value}");
```

Each entry has:

- `Key`: the key written for the value, or `null`.
- `Name`: the text on the line above when it isn't a key, such as `kErrNoFunds`, or the
  label of a numbered key such as `5.Purchase title`. Otherwise `null`.
- `Value`: the text between the carets, or `null` if the value was never closed.

Some tables hold data rather than text for players, such as the credits script in table 242.
Read those through `Entries`.

## Check a table for mistakes

Parsing never fails on a badly formed table. CST.NET reads what it can and lists the
mistakes in `Problems`:

```csharp
foreach (var problem in table.Problems)
    Console.WriteLine($"Line {problem.Line}: {problem.Kind} {problem.Key}");
```

| `Kind` | Cause | What you get |
| --- | --- | --- |
| `UnclosedValue` | A value has no closing caret | The value is left out of lookups, and its entry's `Value` is `null`. Reading picks up again at the next line holding a whole value, such as `3 ^Done^`. |
| `RepeatedKey` | A key is used a second time | Lookups return the first value. `Key` holds the repeated key. |

`Line` is the line the value starts on, counting from 1.

## What CST.NET leaves alone

Values come back exactly as written, except for line breaks:

- **Line breaks** inside a value are always `\n`, whatever the file uses and whatever system
  you run on.
- **Placeholders** such as `%s`, `%d`, and `%%` stay as written until you
  [fill them in](#fill-in-placeholders).
- **References** to another table, values starting with the character U+0002, stay as written
  in a `CSTTable`. `UIText` follows them for you.

## Fill in placeholders

Pass variables to `CST.Format`, and they fill a value's placeholders in order:

```csharp
if (table.TryGetValue("3", out var welcome))
    Console.WriteLine(CST.Format(welcome, "Maya")); // Welcome back, Maya!
```

| Placeholder | Fills in |
| --- | --- |
| `%s` | The next variable |
| `%d`, `%i`, `%u`, `%ld`, `%li`, `%lu` | The next variable, usually a number |
| `%3d` | The next variable, padded with spaces to at least 3 characters |
| `%03d` | The next variable, padded with zeros to at least 3 characters, after any minus sign |
| `%.3d` | The next variable, padded with zeros to at least 3 digits, after any minus sign |
| `%%` | One percent sign |

Variables are written the same way on every system, so `1.5` never becomes `1,5`. Padding
never shortens a number, and a width over 1,000 pads nothing. Text a variable puts in isn't
read for placeholders again.

If you run out of variables, the remaining placeholders stay as written, so the mistake shows.
Any other `%`, such as in `to sell % for $`, is left alone. A `null` variable throws an
`ArgumentException`.

`CST.Parse` and `UIText`'s lookups also take variables after the key:

```csharp
var welcome = text.GetTextIfPresent(101, 3, "Maya"); // Welcome back, Maya!
```

## Read text from a TSO install

`UIText` finds a table by its ID, reads it, and looks a value up, the way TSO loads its own
text. Point it at the game's `gamedata` folder:

```csharp
using CSTNet;

var text = new UIText("english", "/path/to/TSOClient/gamedata");

var accept = text.GetTextIfPresent(101, "ok button");
if (accept is null)
    Console.WriteLine("That table or value doesn't exist.");
```

`UIText` looks for tables named `_<id>_<name>.cst` in these folders, in this order:

1. `<base>/<language>.dir`
2. `<base>/uitext/<language>.dir`, used by TSO New & Improved
3. `<base>/uitext.dir`, used by earlier TSO versions

Without a base path, `UIText` looks in a `uitext` folder next to your program, and without
a language it uses `english`.

It also:

- **Decodes files** as UTF-8 and falls back to Windows-1252 for the few TSO tables that
  aren't UTF-8.
- **Finds values by position** when you pass a number that no value uses as its key. Positions
  count every value in the table from 0, and only reach values with no key of their own.
- **Follows references** to another table and returns that table's text. It follows only one
  reference, so a reference to another reference returns `null`.

### Missing text

`GetTextIfPresent` returns `null` when the table, the value, or a reference's target is
missing. `GetText` returns the text `***MISSING***` instead, which is kept for code written
against older versions. Prefer `GetTextIfPresent`, because a real value could hold that text.

If a table file or folder can't be read, both methods throw an `IOException` or
`UnauthorizedAccessException`.

### Limits

- Installs with `uitext.dir` hold one language. `UIText` uses it for any language you ask for.
- `UIText` reads and parses the table file on every lookup. For many lookups in one table,
  parse it once with `CSTTable` instead, and decode the file yourself if it isn't UTF-8.
- A reference's own arguments, such as `<string arg="1">%s</string>`, aren't passed on. The
  variables you pass fill the target's text.

## Quick lookups

`CST.Parse` looks up one value in a string, and returns `***MISSING***` if there's no match.
Variables after the key fill its placeholders:

```csharp
var content = File.ReadAllText("_101_purchase.cst");
var cancel = CST.Parse(content, 2); // Cancel
var welcome = CST.Parse(content, 3, "Maya"); // Welcome back, Maya!
```

It parses the whole table on every call and can't tell a missing value from real text.
Use `CSTTable` for anything more than a one-off lookup.
