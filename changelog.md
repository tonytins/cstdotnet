# Change Log

## 1.0.400

This version supports both .NET Standard 2.1 and .NET 6 and brings with it (much needed) quality of life changes on the project side of things. Apart from that, nothing has changed to the API itself apart from much needed documentation.

From this version onward, CSTNet will only target LTS releases. This is why .NET 5 was skipped, despite the initial platform unification.

### Project Changes

With the move to .NET 6.0, this version brings with it a lot of quality of life changes to the project. This includes file-scoped namespaces, implicit and global usings. For a full list of language changes see [Welcome to C# 10](https://devblogs.microsoft.com/dotnet/welcome-to-csharp-10/).

### Nullable Reference Types

The only significant architectural change that was finally enabled with this release is [nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/nullable-reference-types). This feature was introduced in .NET Core 3.0 and .NET Standard 2.1, respectfully, but not the .NET Framework. Nullable values are already taken care of by the library but this ensures that it is always enforced.

Due to the Sims community historically using the .NET Framework, this feature was not enabled by default. Starting with this version, it is enabled as modern tools now use latest .NET SDK.

### UIText class

The UIText class allows for travseing in ``/<directory>/<language>.dir`` directories and searching for CST files by their Id number. (e.g. _*154*_miscstrings.cst). By defualt, the base path is ``/<program directory>/uitext/<language>.dir``. For more info, see [usage.md](./usage.md).

## 1.0.300

- Minor patch.

## 1.0.3

- Backport switch to CSTNet namespace
- Internal improvements.

## 1.0.2

- Fixed the multiple line parsing in the v2 format.
- Replaced "``[ENTRY NOT FOUND]``" message with "``***MISSING***``".

## 1.0.1

Despite only being a point release, this includes a major refinement to the normalizing algorithm.

### Rewrote normalizing algorithm

The normalizing algorithm has been rewritten to be more efficient and hopefully more reliable. The new algorithm de-constructs each line after converting it to the system's native line ending. Then it searches for the key and returns value. The rewrite also normalizes line endings to match the system's within the entry itself before returning the final output. This should make things more stable and predictable.

### Known issues

- Skipping comments is still a little buggy.
- Multiline parsing with the v2 format is still a little unpredictable.

## 1.0.0

- Initial release.
