# Change Log

## 1.1.100

- Switched to Sixam.CST namespace and marked CSTNet namespace as obsolete.
- Performance improvements.
- Patch numbers are now in the triple digits.

### UIText class

The UIText class allows for travseing in ``/<directory>/<language>.dir`` directories and searching for CST files by their Id number. (e.g. _*154*_miscstrings.cst). By defualt, the base path is ``/<program directory>/uitext/<language>.dir``. For more info, see [usage.md](./usage.md).

## 1.0.3

- Backport switch to Sixam.CST namespace
- Internal improvements.

## 1.0.2

- Fixed the multiple line parsing in the v2 format.
- Replaced "``[ENTRY NOT FOUND]``" message with "``***MISSING***``".

## 1.0.1

Despite only being a point release, this includes a major refinement to the normalizing algorithm.

### Rewrote normalizing algorithm

The normalizing algorithm has been rewritten to be more efficient and hopefully more reliable. The new algorithm de-constructs each line after converting it to the system's native line ending. Then it searches for the key and returns value. The rewrite also normalizes line endings to match the system's within the entry itself before returning the final output. This should make things more stable and predictable.

### CSTNet compatibility

For point releases (such as this), Sixam.CST will remain under the CSTNet namespace for compatibility reasons. CSTNet will be moved to Sixam.CST namespace starting with 1.1.

### Known issues

- Skipping comments is still a little buggy.
- Multiline parsing with the v2 format is still a little unpredictable.

## 1.0.0

- Initial release.