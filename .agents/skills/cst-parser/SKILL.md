---
name: cst-parser
description: 'Use when writing, porting, reviewing, or debugging a Caret-Separated Text (CST or .cst) reader/parser in any language. Use the CST specification and swift-cst reference implementation to preserve parsing behavior.'
---

# Writing CST Parsers

Use this workflow to implement or evolve a CST reader in any language or
project. Caret-Separated Text (CST) was used for The Sims Online interface
text. Its specification labels each rule **Confirmed**, **Observed**,
**Unsettled**, or **Not seen**. Keep those levels of certainty visible in
design decisions instead of treating every rule as equally established.

## References

- The [CST format specification](https://github.com/tonytins/swift-cst/blob/main/doc/CST.md)
  is authoritative for format behavior and its evidence.
- The [reference scanner](https://github.com/tonytins/swift-cst/blob/main/Sources/SwiftCST/TableScanner.swift)
  and [table API](https://github.com/tonytins/swift-cst/blob/main/Sources/SwiftCST/CSTTable.swift)
  show one implementation.
- The [reference tests](https://github.com/tonytins/swift-cst/blob/main/Tests/SwiftCSTTests/CSTTableTests.swift)
  show how the specification example and edge cases are verified.

When the specification and reference implementation appear to differ, check the
relevant tests and specification history before choosing behavior. Do not copy
language-specific APIs or assume the implementation settles an explicitly
Unsettled rule.

## Procedure

1. **Set the boundary.** A reader consumes the text of one table and returns
   values in file order, with their keys/names and any malformed-entry
   diagnostics. Keep file discovery, table naming, language directories,
   encoding fallbacks, references between tables, and application-specific
   positional lookups in an optional wrapper. Do not make the reader assume a
   particular application's file layout.
2. **Model entries and problems.** Preserve each value's raw text, optional
   key, optional name, and order. Represent an unclosed value distinctly from
   an empty value. Reading should continue after a malformed entry; report it
   instead of failing the whole table or silently returning another entry's
   text.
3. **Normalize input before scanning.** Accept UTF-8 text with or without a
   leading BOM. Normalize CRLF before lone CR, as well as LF and U+2028, to LF.
   Keep source line numbers for diagnostics. If supporting original TSO files,
   a file-loading wrapper may need Windows-1252 fallback; byte decoding is
   separate from parsing an already-decoded string.
4. **Remove comment lines.** A line whose first non-whitespace characters are
   `//` or `#` is a comment. A line beginning with `/*` starts a block comment
   that continues through the line containing `*/`. Remove these lines before
   parsing, including when they occur inside a multiline value; preserve line
   positions for diagnostics.
5. **Scan carets in order.** A value begins at `^` and ends at the next `^`.
   There is no caret escape. Preserve all text and line breaks inside a value;
   `^^` is a valid empty value. Multiple values may occur on one line, and a
   value may span lines. Text outside values is ignored unless it supplies the
   next value's key or name.
6. **Determine each value's key/name at its opening caret.**
   - Non-whitespace text since the prior value boundary on that line, trimmed,
     is the whole key. Do not split multiword keys or match key prefixes.
   - If that text is empty, use the nearest preceding non-blank,
     non-comment line: ASCII digits alone are a key; any other text is a name.
   - Blank and comment lines do not break this relationship. Text after a
     closing caret, when no later value begins on that line, is ignored and
     does not become the next value's key or name.
7. **Recover from unclosed values.** While a value is open, a line containing
   exactly two carets with only whitespace after the second marks the start of
   a whole entry. Abandon and report the open value, but retain its entry
   position; then parse that line as the next entry. The last non-blank line
   read into the abandoned value may be the next value's key or name. Also
   report and retain an unclosed value at end of file. Never let one missing
   caret consume the rest of the table.
8. **Build lookups without changing entries.** If the reader offers key/name
   lookup, keys take priority over names, even if a matching name occurs
   earlier. The first repeated key wins and the repeat is reported. Return a
   missing result distinctly from any real value. Positional lookup is a
   wrapper concern: if the consuming application defines it, count all
   entries, including unclosed ones, and only use a position when no explicit
   key applies.
9. **Return data untouched.** Do not interpret placeholders, references,
   scripts, or other application-specific value contents in the reader.
   Placeholder substitution and reference resolution belong to the consumer or
   wrapper.
10. **Verify the behavior.** Use the specification's embedded example as a
    golden input and check the returned entries, not only successful lookups.
    Add focused cases for all line endings and BOM; comments and whitespace;
    single-, multi-, and empty-value lines; multiline values and exact spacing;
    names and whole multiword keys; duplicate keys and key/name collisions;
    missing lookups; unclosed entries in the middle and at EOF; and values
    containing literal placeholders or references. When representative source
    tables are available, compare successful and expected-missing lookups
    across them.
