# Project Guidelines

## Project Structure

CST.NET is a general-purpose .NET library for Caret-Separated Text (`.cst`), a
string-text format Maxis used in Sims Steering Committee (SSC) and later in The Sims
Online (TSO) for its interface text. It has two layers: wrappers that load files, then the
CST core they read through.

- **CST core** (`CSTTable`, `CSTEntry`, `CSTProblem`, and the older `CST.Parse`) reads a
  document from a `string`. It is public so anyone can write their own wrapper, and it
  must not assume TSO's file layout. It owns comments, keys, names, values, and malformed
  entries.
- **Wrappers** find tables on disk and answer lookups. They use only the core's public API,
  apart from `CSTFiles`, a shared internal helper that reads table file names and decodes
  files as UTF-8 or Windows-1252.
  - **`UIText`** mimics how TSO loads its text, and it may be TSO-specific. It owns TSO's
    file names, language directories, positions in key lookups, and references.

`CSTNet` holds the library, and `CSTNet.Tests` covers its behavior with xUnit v3.
[doc/README.md](doc/README.md) documents usage, and [changelog.md](changelog.md)
records each release. `notebooks/` holds the original prototype and is kept as history;
don't update it to match the library.

Never commit build output (`bin/`, `obj/`, `*.nupkg`).

## Build and Test

```sh
dotnet build   # no new warnings
dotnet test    # all tests must pass
```

- The library and tests target .NET 10, pinned in `global.json`.
- For a change to the public API or to lookup results, also call the library from a
  small scratch project outside the repository.
- CI builds and tests on Linux, Windows, and macOS. Report what you could not run
  locally, such as Windows or macOS from Linux.

## C# and CST

- Follow the [.NET guidelines skill](.agents/skills/dotnet-guidelines/SKILL.md) and
  [.editorconfig](.editorconfig) for general conventions, including SDK choices,
  language versions, and platform-specific APIs. Where they disagree with this file,
  this file wins.
- Match the project's existing naming and formatting. The C# version follows the target
  framework; don't set `LangVersion`.
- Keep public APIs clear at the call site, and document all public behavior with XML
  comments.
- Keep the core and `CSTFiles` free of LINQ, reflection, and code page providers, so they
  run where those haven't been shown to work, such as VentureOS's Cosmos kernel. The
  library builds with `IsAotCompatible`, so trimming and AOT problems show up as warnings.

## Golden Rules

### Design Process

- **Iterate**. A first design or name is a draft. Rebuild clumsy parts as understanding
  improves.
- **Ask when a decision is open**, and explain the reasoning behind what you suggest.
- **Keep it simple**. The simplest approach that matches what real tables do wins.
- **Say what you're unsure of**: assumptions, unverified behavior, and what needs testing.

### Never Return Wrong Text (Outranks Completeness)

A program shows whatever CST.NET hands it, so a plausible wrong value is worse than a
missing one. Accuracy outranks reading every last value.

- **Never return another entry's text.** A key matches whole, and an unclosed value must
  not swallow the entries after it.
- **Reading a table never fails.** Report a malformed entry in `Problems` and keep the rest
  of the table. Only an unreadable file or directory is an error.
- **Report missing text as missing.** Return `false` or `null`, never a marker string a
  program could mistake for real text. `***MISSING***` survives only in the older
  string-returning APIs, for compatibility.
- **Hand values back untouched** unless asked to fill their placeholders, because some
  tables hold data rather than text for players.
- **Validate arguments** at the public API, and throw `ArgumentNullException` for `null`
  rather than failing deeper in.
- **Add a focused regression test** for every misread, wrong-value, or malformed-input fix.

### Format Rules

- **The [CST specification](https://github.com/tonytins/swift-cst/blob/main/doc/CST.md)
  is the reference** for how real files behave. It lives in swift-cst, and it labels how
  sure each rule is. Follow its stated choice for an **Unsettled** rule, and say so in the
  change.
- **Change the specification along with any change to how files are read.** Propose the
  change in swift-cst first, so CST.NET and SwiftCST keep reading tables the same way.
  SwiftCST's scanner is the reference implementation to port from.
- **Keep the specification's example in the tests**, with every entry it lists, and update
  it when the specification's example changes.
- **Check parser changes against real tables.** Hand-written samples hold only the shapes
  their author already knows about. `TSO_GAME_DIR`, `TSO_CLASSIC_DIR`, and `TSO_BETA_DIR`
  point to local installs when they are set, and `dotnet test` then reads every table in
  them. Comparing every entry before and after a change is the strongest check. Do not
  commit files from a game install.

### Releases and Changelog

- **The version is `<Version>` in [CSTNet.csproj](CSTNet/CSTNet.csproj)**, set by hand and
  loosely following the .NET SDK's `major.minor.patch`: the hundreds digit of the patch
  is the feature band. Start a new band for a new feature; a new major or minor version
  needs the owner's word.
- **Pushing a `releases/**` branch publishes to NuGet.** Leave that to the owner.
- **Record changes callers would notice** in the changelog, under the next version, and
  list breaking changes against the last version published on NuGet.

## Keeping Docs Current

Describe only what works today.

- [doc/README.md](doc/README.md) when usage or behavior changes. Run every example
  against the library before committing it.
- [README.md](README.md) when features, prerequisites, or setup change, and the package
  description in `CSTNet.csproj` when what the library does changes.
- This file when the layout or rules change.

State breaking changes, dependency bumps, and anything you couldn't verify in the summary
or commit message. If you notice a risk outside your task, report it rather than fixing it
quietly.

See [REVIEW.md](REVIEW.md) for the project's review checklist.

## Personal Configuration

Keep personal preferences and machine-specific guidance in git-ignored `*.local` files or
directories (such as `AGENTS.local.md`), rather than in shared project instructions. Check
for relevant local guidance before environment-specific work. You can create or update
these files for details such as:

- Coding preferences
- Local build and development environment
- Personal shortcuts or workflows
