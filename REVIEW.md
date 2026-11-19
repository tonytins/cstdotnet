# Review Guide

Use this checklist when reviewing a change to CST.NET. It is adapted from the
contributor and review practices in [NVIDIA FLARE's contribution guide](https://github.com/NVIDIA/NVFlare/blob/main/CONTRIBUTING.md),
by way of [Hudini](https://github.com/tonytins/Hudini), with checks tailored to
this .NET library and the CST format. The rules it checks against are in
[AGENTS.md](AGENTS.md).

## Before Reviewing

- Read the change description and the relevant code, tests, and docs.
- Identify the lookup results or other caller-visible behavior the change is meant to
  address. If it refers to a rule in the CST specification, an issue, or a design
  discussion, check that the change addresses that context.
- Check the available build, test, and CI results. Run `dotnet build` and `dotnet test`
  when the change or its verification warrants it. For changes to how tables are read,
  run the tests with a local TSO install set, and compare every entry before and after
  the change. Report checks that couldn't be run, such as Windows or macOS from Linux.

## Review Checklist

- Does the behavior match the stated intent, including boundary cases such as empty
  values, keys with spaces, indented keys, numbered keys, values with no key, comments
  inside values, a byte order mark, and every line ending?
- **Wrong text:** Could a lookup now return another entry's text? A key must match whole,
  and an unclosed value must not swallow the entries after it.
- **Malformed tables:** Does a malformed entry get reported in `Problems` while the rest of
  the table still reads? Is missing text reported as missing rather than as a marker
  string?
- **Specification:** Does the change follow the
  [CST specification](https://github.com/tonytins/swift-cst/blob/main/doc/CST.md),
  including its stated choice for an Unsettled rule? If it changes how files are read, is
  the matching specification change proposed in swift-cst, and does the specification
  example test still cover every entry?
- **Real tables:** If the change affects how files are read, was it checked against
  every table of a real install? Did any entry change that the change didn't mean to?
- **Layers:** Does the CST core stay free of TSO's file layout, encodings, positions, and
  references? Does `UIText` reach the core only through its public API?
- **Values:** Are values handed back untouched, with placeholders, references, and data
  such as the credits script left as written?
- **Public API:** Are names, defaults, and behavior clear and consistent with the existing
  APIs? Could the change break existing callers or change what a lookup returns? Is every
  public member documented with XML comments, and are `null` arguments rejected?
- **Platforms:** Does the change need a newer runtime than .NET 10, or behave differently
  on Windows, such as with line endings or path separators?
- **Dependencies:** Are new packages or version bumps approved, called out, and in a
  change of their own?
- **Tests:** Do focused tests cover the changed behavior, and would a new test fail
  without the fix? Is a run from a scratch project needed beyond the tests?
- **Docs:** Are the changelog (caller-visible changes only, with breaking changes called
  out), the usage guide, the README, and `AGENTS.md` accurate after the change? Do the
  usage guide's examples still produce what they show?

## Writing Review Comments

- Report concrete correctness, compatibility, reliability, or missing-test concerns. Don't
  block changes on personal style preferences.
- Make each comment specific, constructive, and actionable. Point to the smallest relevant
  location and explain the impact and a useful next step.
- Prioritize findings by impact:
  - **Blocking**: prevents the package from building, makes reading a table throw, or
    returns another entry's text in normal use.
  - **High**: breaks an existing public behavior, changes what a lookup returns without
    saying so, or misreads the entries after a common input.
  - **Medium**: causes a limited but meaningful behavior or reliability issue, or leaves
    docs describing something that isn't true.
  - **Low**: a small edge case or non-blocking improvement.
- If no actionable issues remain, say so and note any important checks that were not run.
