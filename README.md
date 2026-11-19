# 🔠 CST.NET

CST.NET is a .NET library for reading Caret-Separated Text (`.cst`), the plain-text format The Sims Online uses for its interface text. Each value sits between two carets and is found by its key, as in `1 ^Buy^` or `ok button ^Accept^`.

It reads tables the way the [CST specification](https://github.com/tonytins/swift-cst/blob/main/doc/CST.md) describes, and comes with a `UIText` class that loads text from a TSO install the way the game does. See the [usage guide](doc/README.md) to get started.

## 🛠 Features

- [x] Reads every table in The Sims Online's beta, base game, and New & Improved installs
- [x] Keys, names, numbered keys, and multiline values, as the CST specification describes them
- [x] Reports malformed entries, such as unclosed values, without losing the rest of the table
- [x] `UIText`, for loading text from a TSO install by table ID, position, or reference
- [ ] Filling in placeholders such as `%s` and `%d`

## 🗓️ Update Cycle

| Type         | Frequency            |
| ------------ | -------------------- |
| Minor Update | Every 3–6 months     |
| Patch Update | Monthly or as needed |
| Major Update | As needed            |

## 🛡️ Support

- [ ] Active Support
- [x] Limited Support (Security patches only)
- [ ] Maintenance Mode (Dependency-only updates)
- [ ] Archived (No active work planned)

## 🧰 Prerequisites

Before you begin, ensure you have the latest versions of the following installed:

- [.NET](https://dotnet.microsoft.com/en-us/) 10.0 or later

## 📝 Project Notes

- CST.NET returns placeholders such as `%s` untouched, so code that shows the text fills them in.
- Changes are recorded in the [changelog](doc/changelog.md).

## 🔍 Background

This all began as a "what if?" What if I could made my own CST parser? It was an attempt to create my first algorithm. One that was a little better than came before. So, I did. First from a Jupyter notebook and then a full-fledged project that now has it's own support cycle.

## ⚖️ License

I license this project under BSD-3-Clause license — see the [LICENSE](LICENSE) file for full text.

