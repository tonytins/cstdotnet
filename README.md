# 🔠 CST.NET

CST.NET is a library for parsing Maxis' key-value pair format. It can be used in conjunction with your own custom frameworks, or the original `UIText` APIs.

Caret-Separated Text (or CST) is a key-value pair format represented by digits or words as keys and the value as text enclosed between carets. (e.g. `<key> ^<text>^`) Any text which is not enclosed with carets is considered a comment and ignored. Neither strings nor comments may use the caret character.

## 🛠 Features

* [x] Native support for Sims Online's `.cst `files.
* [x] UIText Support
* [ ] Variable support

## 📅 Support & Update Cadence

| Type         | Frequency            | Notes                                    |
| ------------ | -------------------- | ---------------------------------------- |
| Minor Update | Every 3–6 months     | Small enhancements, non-breaking changes |
| Patch Update | Monthly or as needed | Bug fixes, security updates              |
| Major Update | As needed            | Framework upgrades, major refactors      |

* Reserve months: June (Mid-Year Chill) & December (End-Year Freeze)

## 🧘 Sustainability Practices

* 20% creative/recovery space built into development
* Mandatory cooldowns after major launches (minimum 1 week)
* Crisis Mode Activates if:
  * Critical vulnerabilities
  * Framework-breaking issues

## 🛡️ Support Levels

* [ ] Active Support
* [x] Limited Support (Security patches only)
* [x] Maintenance Mode (Dependency-only updates)
* [ ] Archived (No active work planned)

## ⏰ Project Timeline

| Milestone          | Target Date | Status  |
| ------------------ | ----------- | ------- |
| First Stable Build | 12/13/2020  | Done    |
| Feature Complete   | N/A         | Planned |
| Maintenance Phase  | N/A         | Planned |

## 📓 Project Notes

* This all began as a "what if?" What if I could make my own? So, I did. First from a Jupyter notebook and then a full-fledged project that technically has a support cycle.
* While this project is technically feature complete, it is not a full drop-in replacement as it lacks variable (`%[digit]`) parsing.

## 📄 License

I license this project under BSD-3-Clause license — see the [LICENSE](LICENSE) file for full text.
