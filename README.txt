# FileMergerPlus

A Windows desktop application for collecting text-based files from a folder and merging them into a single `.txt` report. It is built for **.NET Framework 4.6.2** with **WPF** and **MaterialDesignThemes**, so it runs on **Windows 7, Windows 8, Windows 10, and Windows 11**.

The app was designed to be practical, visually polished, and safe for long-running merges: it supports cancellation, persistent settings, folder drag-and-drop, localization, dark/light themes, and structured output with file statistics.

## Features

- Merge files from a selected folder or from the folder where the `.exe` is located
- Optional recursive scan of subfolders
- File extension presets with editable extension lists
- Exclude hidden/system files
- Include empty files
- Folder tree only mode for fast structure reports
- Beautiful Material Design WPF interface
- Dark theme by default, with a light/dark toggle
- Localization support for 10 languages
- Progress reporting and cancellation
- Persistent settings between sessions
- Automatic output file naming with numbering
- Safe skipping of unreadable/problematic files
- The app saves its settings automatically between sessions.

## Requirements

- Windows 7 / 8 / 10 / 11
- .NET Framework 4.6.2 Developer Pack
- Visual Studio with WPF support
- NuGet packages:
  - `MaterialDesignThemes` **4.9.0**
  - `MaterialDesignColors` **2.1.4**

## What the app does

FileMergerPlus scans a folder, filters files by extension and other rules, then writes a single `.txt` file containing:

- a folder tree section
- per-file metadata
- file contents
- a final statistics section

The output is organized and readable, making it useful for AI training data, sharing code bases, text collections, logs, notes, and other plain-text projects.

## Main workflow

1. Choose the source folder, or use the current application folder.
2. Pick whether subfolders should be included.
3. Select a file extension preset or enter a custom extension list.
4. Configure optional behavior such as empty files, hidden/system files, folder tree only mode, theme, and language.
5. Click **Merge**.
6. The app creates a numbered file like:

```text
Merged_1_MyProject.txt
Merged_2_MyProject.txt
Merged_3_MyProject.txt
```

The merged file name follows this format:

```text
Merged_<Number>_<RootFolderName>.txt
```

## Folder tree format

The folder tree at the beginning of the merged file uses a depth-based `#` layout:

```text
Structure: /Project
# Folder: src/
## File: main.py (12 KB)
## File: utils.py (5 KB)
# Folder: tests/
## File: test_main.py (3 KB)
# File: readme.md (2 KB)
```

## File entry format

Each merged file is written with metadata and content in a clean block. The current format uses:

- file index and total file count, file name, relative path, size, line count, file content, a clear end-of-file marker

## Localization

The interface can be switched to one of these languages:

- English
- Deutsch
- Русский
- 中文
- Español
- Français
- 日本語
- Português
- 한국어
- Italiano

## Compatibility notes

This project intentionally targets older Windows environments. To keep compatibility with Windows 7 and .NET Framework 4.6.2:

- `FolderBrowserDialog` is used for folder selection
- the UI is implemented in WPF
- NuGet package versions are chosen for .NET Framework compatibility
- asynchronous file work is handled without requiring newer runtime features

## Building the project

1. Install the **.NET Framework 4.6.2 Developer Pack**.
2. Open the solution in Visual Studio.
3. Restore NuGet packages.
4. Build the solution.

# License

[MIT](./LICENSE.txt)

## Notes

Special thanks to Cursor for the help with the development