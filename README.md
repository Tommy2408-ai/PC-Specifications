# 🖥️ PC Specs Inspector (C#)

A simple, ultra-lightweight C# console application for Windows that detects and clearly displays the computer's key hardware specifications.

Designed for command-line use to copy or verify the hardware configuration of any PC on the fly.

---

## 🚀 Information Collected

- **Operating System:** Version, build, and architecture (x64/x86).
- **CPU:** Processor name, number of cores and threads, and maximum frequency.
- **RAM:** Total capacity, type (DDR3/DDR4/DDR5), frequency in MHz, and number of installed modules.
- **GPU:** Support for multiple video cards (dedicated and integrated) with VRAM indication.
- **Storage:** Fixed drives showing free and total space for each partition.
- **Motherboard:** Manufacturer and model.

---

## 🛠️ Requirements

- **OS:** Windows 10 / 11
- **Runtime:** .NET 6.0 / 7.0 / 8.0 or higher
- **NuGet dependencies:** `System.Management`
