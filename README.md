# 🔠 CST.NET

CST.NET is a library for parsing Maxis' key-value pair format. It can be used in conjunction with your own custom frameworks, or the original `UIText` APIs.

Caret-Separated Text (or CST) is a key-value pair format represented by digits or words as keys and the value as text enclosed between carets. (e.g. `<key> ^<text>^`) Any text which is not enclosed with carets is considered a comment and ignored. Neither strings nor comments may use the caret character.

## 🛠 Features

- [x] Native support for Sims Online's `.cst` files.
- [x] UIText Support
- [ ] Variable support

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

- [.NET](https://dotnet.microsoft.com/en-us/) 8.0 or later

## 📝 Project Notes

- While this project is technically feature complete, it is not a full drop-in replacement as it lacks variable (`%[digit]`) parsing.
- I'm deprecating the `.sln` solution type in favour of `.slnx` going forward. It'll be removed sometime in the future.

## 🔍 Background

This all began as a "what if?" What if I could made my own CST parser? It was an attempt to create my first algorithm. One that was a little better than came before. So, I did. First from a Jupyter notebook and then a full-fledged project that now has it's own support cycle.

## ⚖️ License

I license this project under BSD-3-Clause license — see the [LICENSE](LICENSE) file for full text.

