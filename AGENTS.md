# Project Guidelines

## Project structure

CST.NET is a general-purpose .NET library for Caret-Separated Text (`.cst`), the
format The Sims Online (TSO) used for its interface text. It has two layers, and text
lookups run through them in this order: `UIText`, then the CST core.

- **CST core** (`CST`) reads a document from a `string`. It is public so
  anyone can write their own wrapper, and it must not assume TSO's file layout. It owns
  comments, keys, values, and malformed entries.
- **`UIText`** mimics how TSO loads its text, and it may be TSO-specific. It owns file
  names, language directories, and text encodings. It should use only the core's public
  API.

`CSTNet` holds the library, `CSTNet.Tests` covers its behavior, and
[doc/README.md](doc/README.md) documents usage, [doc/changelog.md](doc/changelog.md)
records each release.

## Build and test

- Build with `dotnet build`.
- Run the test suite with `dotnet test`.
- For a change to the public API or to lookup results, also call the library from a
  small scratch project outside the repository.
- CI builds and tests on all supported platforms. Report what you could not run.

## C# and CST

- See [the .NET guidelines](.agents/skills/dotnet-guidelines/SKILL.md) for general conventions,
  including SDK choices, language versions, and platform-specific APIs.
- Match the project's C# language version and existing naming and formatting.
- Keep public APIs clear at the call site, and document all public behavior with XML comments.
- Reading a table never fails. Report a malformed entry and keep the rest of the table.
- Never return another entry's text. A wrong value is worse than a missing one.
- Hand values back untouched unless asked to fill their placeholders, because some
  tables hold data rather than text for players.
- Add focused tests when behavior changes, and record changes callers would notice in the
  changelog.

## Format rules

The CST specification is the reference for how real files behave. Change it along with any
change to how files are read. Tests should verify behavior against real tables when available.
`TSO_GAME_DIR`, `TSO_CLASSIC_DIR`, and `TSO_BETA_DIR` point to local installs when they are set,
and the test suite then reads every table in them. Do not commit files from a game install.

See [REVIEW.md](REVIEW.md) for the project's review checklist.

## Personal Configuration

Keep personal preferences and machine-specific guidance in git-ignored `*.local` files or
directories (such as `AGENTS.local.md`), rather than in shared project instructions. Check
for relevant local guidance before environment-specific work. You can create or update
these files for details such as:

- Coding preferences
- Local build and development environment
- Personal shortcuts or workflows
