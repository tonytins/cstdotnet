# Change Log

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