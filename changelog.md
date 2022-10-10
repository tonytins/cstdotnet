# Change Log

## 2.0.100

This version supports both .NET Standard 2.1 and .NET 6 and brings with it (much needed) quality of life changes internally. Apart from that, nothing has changed to the API itself apart from much needed documentation.

### UIText class and interface

Based on FreeSO's API, the ``UIText`` class allows for traversing in ``/<directory>/<language>.dir`` directories and searching CST files by their Id number. (e.g. _*154*_miscstrings.cst). By defualt, the base path is ``/<program directory>/uitext/<language>.dir``. You may also create your own implementation based on these APIs using the ``IUIText`` interface which mine also uses.

For more info, see [usage.md](./usage.md).

### New Release Cycle

Because this library has no third-party dependencies, CST.NET's release cycle will generally follow .NET's LTS cycle. This is why .NET 5 was skipped. Minor releases will consist of targeting latest LTS.

For example, 2.5 adds .NET 8.0 next to 6.0 until the latter reaches end of life and finally 2.6 targets only 8.0. Keep in mind, this is all just hypothetical. Realistically, this could happen as soon as 2.2 because parameters will land as earlier as 2.1.

## 1.0.300

- Minor patch.

## 1.0.3

- Backport switch to CSTNet namespace
- Internal improvements.

## 1.0.2

- Fixed the multiple line parsing in the v2 format.
- Replaced "``[ENTRY NOT FOUND]``" message with "``***MISSING***``".

## 1.0.1

Despite only being a patch release, this includes a major refinement to the normalizing algorithm.

### Rewrote normalizing algorithm

The normalizing algorithm has been rewritten to be more efficient and hopefully more reliable. The new algorithm de-constructs each line after converting it to the system's native line ending. Then it searches for the key and returns value. The rewrite also normalizes line endings to match the system's within the entry itself before returning the final output. This should make things more stable and predictable.

### Known issues

- Skipping comments is still a little buggy.
- Multiline parsing with the v2 format is still a little unpredictable.

## 1.0.0

- Initial release.
