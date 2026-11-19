# Change Log

## 2.1

### 2.1.200

CST.NET now reads tables the way the
[CST specification](https://github.com/tonytins/swift-cst/blob/main/doc/CST.md) describes,
and has been checked against every table in The Sims Online's beta, base game, and New &
Improved installs. See the [usage guide](README.md) for the new APIs.

#### Breaking changes

- CST.NET now needs .NET 10. Support for .NET 8 has ended.
- Keys match whole. Before, a lookup returned the first value whose line started with the key,
  so `1` could return the value for `10`, and `ok` the value for `ok button`.
- Line breaks inside a value are `\n` on every platform, whatever the file uses. Before, they
  used the system's line ending, so on Windows a multiline value contained `\r\n`.
- Comments are recognized wherever they start a line, including indented lines and lines inside
  a multiline value, which are no longer part of the value.
- `UIText.GetText` returns `***MISSING***` when the language folder doesn't exist, instead of
  throwing `DirectoryNotFoundException`.
- `UIText`'s constructors throw `ArgumentNullException` for a `null` language or base path.

#### New

- `CSTTable` reads a table into its values in file order, with `TryGetValue` for lookups.
  Each `CSTEntry` has a `Key`, a `Name`, and a `Value`, and `Problems` lists unclosed values and
  repeated keys.
- A number alone on the line above a value is its key, and other text there is its name, such
  as `kErrNoFunds`.
- Numbered keys: a number followed by a period or colon, such as `1.new house window name`,
  `5.`, or `1:`, gives the key `1` (or `5`) and uses any text after the separator as the
  value's name. A digit right after the separator, as in `1.5` or `10:30`, keeps the whole text
  as the key.
- An unclosed value no longer swallows the values after it. It's reported, and reading picks up
  again at the next line holding a whole value.
- When a key repeats, the first value wins and the repeat is reported.
- A byte order mark and LF, CR, CRLF, and U+2028 line endings are all accepted.
- `UIText.GetTextIfPresent` returns `null` for missing text, so it can't be confused with a
  real value.
- `UIText` also finds tables in `uitext/<language>.dir` (New & Improved) and `uitext.dir`
  (earlier versions), decodes non-UTF-8 tables as Windows-1252, finds values with no key by
  their position from 0, and follows references to other tables.

### 2.1.100

- Removed `CaretSeparatedText` in favor of `CST` class
- Due to technical reasons, .NET Standard support was removed

#### Experimental

- Parser function can now be consumed from non-.NET programming languages.

## 2.0

### 2.0.104

- Support for .NET 8

### 2.0.102

- Properly commented and documented the rest of the code.

### 2.0.100

This version supports both .NET Standard 2.1 and .NET 6 and brings with it (much needed) quality of life changes internally. Apart from that, nothing has changed to the API itself apart from much needed documentation.

#### UIText class and interface

Based on FreeSO's API, the `UIText` class allows for traversing in `/<directory>/<language>.dir` directories and searching CST files by their Id number. (e.g. \_*154*\_miscstrings.cst). By default, the base path is `/<program directory>/uitext/<language>.dir`. You may also create your own implementation based on these APIs using the `IUIText` interface which mine also uses.

For more info, see the [usage guide](README.md).

## 1.0

### 1.0.300

- Minor patch.

### 1.0.3

- Backport switch to CSTNet namespace
- Internal improvements.

### 1.0.2

- Fixed the multiple line parsing in the v2 format.
- Replaced "`[ENTRY NOT FOUND]`" message with "`***MISSING***`".

### 1.0.1

Despite only being a patch release, this includes a major refinement to the normalizing algorithm.

#### Rewrote normalizing algorithm

The normalizing algorithm has been rewritten to be more efficient and hopefully more reliable. The new algorithm de-constructs each line after converting it to the system's native line ending. Then it searches for the key and returns value. The rewrite also normalizes line endings to match the system's within the entry itself before returning the final output. This should make things more stable and predictable.

#### Known issues

- Skipping comments is still a little buggy.
- Multiline parsing with the v2 format is still a little unpredictable.

### 1.0.0

- Initial release.