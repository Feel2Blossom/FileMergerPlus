using MaterialDesignThemes.Wpf;
using FileMergerPlus.Localization;
using FileMergerPlus.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinForms = System.Windows.Forms;

namespace FileMergerPlus
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<int, string> _templateExtensions = new Dictionary<int, string>
        {
            { 0, string.Empty },
            { 1, ".cs, .vb, .py, .c, .cpp, .h, .java, .js, .ts, .php, .go, .rs" },
            { 2, ".html, .css, .js, .ts, .jsx, .tsx, .vue, .php" },
            { 3, ".txt, .md, .rst, .rtf" },
            { 4, ".json, .xml, .yaml, .yml, .ini, .cfg" },
            { 5, ".csv, .tsv, .sql, .log, .toml, .properties" },
            { 6, string.Empty }
        };

        private readonly Brush _defaultBorderBrush;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isMerging;
        private bool _settingsLoaded;
        private UiStrings _ui = UiStringsCatalog.Get(0);

        public MainWindow()
        {
            InitializeComponent();
            _defaultBorderBrush = FolderPathTextBox.BorderBrush;
            MainSnackbar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(4));
            InitLocalizationCombo();
        }

        private sealed class FileEntry
        {
            public string RelativePath { get; set; }
            public string FileName { get; set; }
            public long SizeBytes { get; set; }
            public int LineCount { get; set; }
            public string Content { get; set; }
        }

        private sealed class MergeResult
        {
            public string OutputFilePath { get; set; }
            public int TotalFiles { get; set; }
            public long TotalSizeBytes { get; set; }
            public int TotalLines { get; set; }
        }

        private sealed class ProcessingProgress
        {
            public int Processed { get; set; }
            public int Total { get; set; }
            public string Phase { get; set; }
        }

        private async void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isMerging)
            {
                return;
            }

            string rootFolder = GetSelectedRootFolder();
            if (!Directory.Exists(rootFolder))
            {
                UpdateStatus(_ui.FolderNotFound);
                FolderPathTextBox.BorderBrush = Brushes.IndianRed;
                return;
            }

            HashSet<string> allowedExtensions;
            try
            {
                allowedExtensions = ParseExtensions(ExtensionsTextBox.Text);
            }
            catch (Exception ex)
            {
                UpdateStatus(ex.Message);
                EnqueueSnackbar(ex.Message);
                return;
            }

            int separatorLength = 60;

            _cancellationTokenSource = new CancellationTokenSource();
            SetBusyState(true);
            MergeProgressBar.IsIndeterminate = true;
            MergeProgressBar.Value = 0;
            UpdateStatus(_ui.StatusScanning);

            var progress = new Progress<ProcessingProgress>(p =>
            {
                if (p == null)
                {
                    return;
                }

                if (string.Equals(p.Phase, "Scanning", StringComparison.OrdinalIgnoreCase))
                {
                    MergeProgressBar.IsIndeterminate = true;
                    UpdateStatus(_ui.StatusScanning);
                }
                else if (string.Equals(p.Phase, "Writing", StringComparison.OrdinalIgnoreCase))
                {
                    MergeProgressBar.IsIndeterminate = false;
                    MergeProgressBar.Maximum = p.Total <= 0 ? 1 : p.Total;
                    MergeProgressBar.Value = Math.Min(p.Processed, MergeProgressBar.Maximum);
                    UpdateStatus(string.Format(CultureInfo.InvariantCulture, _ui.StatusProcessedFormat, p.Processed, p.Total));
                }
            });

            try
            {
                MergeResult result = await MergeFilesAsync(
                    rootFolder,
                    IncludeSubfoldersCheckBox.IsChecked == true,
                    allowedExtensions,
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                    0,
                    IncludeHiddenFilesCheckBox.IsChecked == true,
                    IncludeEmptyFilesCheckBox.IsChecked == true,
                    FolderTreeOnlyCheckBox.IsChecked == true,
                    IncludeTreeCheckBox.IsChecked == true,
                    ShowFullPathCheckBox.IsChecked == true,
                    IncludeWarningsSectionCheckBox.IsChecked == true,
                    separatorLength,
                    _ui,
                    _cancellationTokenSource.Token,
                    progress);

                UpdateStatus(string.Format(CultureInfo.InvariantCulture, _ui.StatusDoneCreatedFormat, result.OutputFilePath));
                ShowMergeCompleteDialog(result.OutputFilePath);

                if (CopyToClipboardCheckBox.IsChecked == true)
                {
                    await CopyToClipboardAsync(result.OutputFilePath);
                }
            }
            catch (OperationCanceledException)
            {
                UpdateStatus(_ui.StatusCancelled);
                EnqueueSnackbar(_ui.SnackbarMergeCancelled);
            }
            catch (Exception ex)
            {
                UpdateStatus(string.Format(CultureInfo.InvariantCulture, _ui.ErrorFormat, ex.Message));
                EnqueueSnackbar(string.Format(CultureInfo.InvariantCulture, _ui.ErrorFormat, ex.Message));
            }
            finally
            {
                SetBusyState(false);
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                SaveSettings();
            }
        }

        private async Task<MergeResult> MergeFilesAsync(
            string rootFolder,
            bool includeSubfolders,
            HashSet<string> allowedExtensions,
            HashSet<string> excludedFolderNames,
            long maxSizeBytes,
            bool includeHiddenFiles,
            bool includeEmptyFiles,
            bool folderTreeOnly,
            bool includeTree,
            bool showFullPath,
            bool includeWarningsSection,
            int separatorLength,
            UiStrings ui,
            CancellationToken token,
            IProgress<ProcessingProgress> progress)
        {
            progress.Report(new ProcessingProgress { Phase = "Scanning" });

            var includedEntries = new List<FileEntry>();
            var skippedMessages = new List<string>();
            IEnumerable<string> allFiles = EnumerateFilesSafe(rootFolder, includeSubfolders, skippedMessages);

            foreach (string filePath in allFiles)
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    if (!MatchesExtension(filePath, allowedExtensions))
                    {
                        continue;
                    }

                    if (!includeHiddenFiles && IsHiddenOrSystemFile(filePath))
                    {
                        continue;
                    }

                    if (IsInExcludedFolder(filePath, rootFolder, excludedFolderNames))
                    {
                        continue;
                    }

                    var fileInfo = new FileInfo(filePath);
                    if (maxSizeBytes > 0 && fileInfo.Length > maxSizeBytes)
                    {
                        continue;
                    }

                    if (!includeEmptyFiles && fileInfo.Length == 0)
                    {
                        continue;
                    }

                    FileEntry entry = folderTreeOnly
                        ? CreateFileEntryMetadataOnly(filePath, rootFolder)
                        : await ReadFileEntryAsync(filePath, rootFolder);
                    includedEntries.Add(entry);
                }
                catch (UnauthorizedAccessException)
                {
                    skippedMessages.Add("Skipped (access denied): " + filePath);
                }
                catch (PathTooLongException)
                {
                    skippedMessages.Add("Skipped (path too long): " + filePath);
                }
                catch (IOException)
                {
                    skippedMessages.Add("Skipped (I/O error): " + filePath);
                }
                catch (Exception)
                {
                    skippedMessages.Add("Skipped (read error): " + filePath);
                }
            }

            if (!includedEntries.Any())
            {
                throw new InvalidOperationException(ui.NoFilesMatched);
            }

            includedEntries = includedEntries
                .OrderBy(x => x.RelativePath, StringComparer.OrdinalIgnoreCase)
                .ToList();

            string outputPath = BuildOutputFilePath(rootFolder);
            int total = includedEntries.Count;
            int totalLines = 0;
            long totalBytes = 0;
            bool outputCreated = false;
            bool emitTree = includeTree || folderTreeOnly;
            var diskDirectoriesRelative = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            AddDirectoryAncestorsFromEntries(includedEntries, diskDirectoriesRelative);

            try
            {
                using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fileStream, new UTF8Encoding(false)))
                {
                    outputCreated = true;

                    if (emitTree)
                    {
                        await writer.WriteLineAsync(BuildStructuredFolderTree(rootFolder, includedEntries, diskDirectoriesRelative));
                        await writer.WriteLineAsync();
                    }

                    string separator = new string('=', separatorLength);
                    if (!folderTreeOnly)
                    {
                        int processed = 0;
                        foreach (FileEntry entry in includedEntries)
                        {
                            token.ThrowIfCancellationRequested();

                            await writer.WriteLineAsync(separator);
                            int fileIndex = processed + 1;
                            string displayName = showFullPath ? entry.RelativePath : entry.FileName;
                            await writer.WriteLineAsync("File " + fileIndex.ToString(CultureInfo.InvariantCulture) + "/" + total.ToString(CultureInfo.InvariantCulture) + ": " + displayName);
                            await writer.WriteLineAsync("Path: " + entry.RelativePath);

                            await writer.WriteLineAsync("Size: " + FormatSize(entry.SizeBytes) + " | Lines: " + entry.LineCount.ToString(CultureInfo.InvariantCulture));
                            await writer.WriteLineAsync(separator);
                            await writer.WriteAsync(entry.Content);
                            if (!entry.Content.EndsWith("\n", StringComparison.Ordinal) &&
                                !entry.Content.EndsWith("\r", StringComparison.Ordinal))
                            {
                                await writer.WriteLineAsync();
                            }

                            await writer.WriteLineAsync(separator);
                            await writer.WriteLineAsync("End of file: " + entry.FileName);
                            await writer.WriteLineAsync(separator);
                            await writer.WriteLineAsync();

                            totalLines += entry.LineCount;
                            totalBytes += entry.SizeBytes;
                            processed++;
                            progress.Report(new ProcessingProgress
                            {
                                Phase = "Writing",
                                Processed = processed,
                                Total = total
                            });
                        }
                    }
                    else
                    {
                        foreach (FileEntry entry in includedEntries)
                        {
                            totalBytes += entry.SizeBytes;
                        }

                        progress.Report(new ProcessingProgress
                        {
                            Phase = "Writing",
                            Processed = total,
                            Total = total
                        });
                    }

                    if (includeWarningsSection && skippedMessages.Count > 0)
                    {
                        await writer.WriteLineAsync("========== WARNINGS ==========");
                        foreach (string warning in skippedMessages)
                        {
                            await writer.WriteLineAsync(warning);
                        }

                        await writer.WriteLineAsync();
                    }

                    await writer.WriteLineAsync("========== STATISTICS ==========");
                    await writer.WriteLineAsync("Total files: " + total.ToString(CultureInfo.InvariantCulture));
                    await writer.WriteLineAsync("Total lines: " + totalLines.ToString(CultureInfo.InvariantCulture));
                    await writer.WriteLineAsync("Total size: " + FormatSize(totalBytes) + " (" + totalBytes.ToString("N0", CultureInfo.InvariantCulture) + " bytes)");
                }
            }
            catch
            {
                if (outputCreated && File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }

                throw;
            }

            return new MergeResult
            {
                OutputFilePath = outputPath,
                TotalFiles = total,
                TotalLines = totalLines,
                TotalSizeBytes = totalBytes
            };
        }

        private static IEnumerable<string> EnumerateDirectoriesSafe(string rootFolder, bool recursive, List<string> skippedMessages)
        {
            var pending = new Queue<string>();
            pending.Enqueue(rootFolder);

            while (pending.Count > 0)
            {
                string current = pending.Dequeue();
                string[] directories;
                try
                {
                    directories = Directory.GetDirectories(current);
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is DirectoryNotFoundException || ex is PathTooLongException || ex is IOException)
                {
                    skippedMessages.Add("Skipped folder (" + ex.GetType().Name + "): " + current);
                    continue;
                }

                foreach (string directory in directories)
                {
                    yield return directory;
                    if (recursive)
                    {
                        pending.Enqueue(directory);
                    }
                }
            }
        }

        private static HashSet<string> CollectDiskDirectoriesRelative(string rootFolder, bool includeSubfolders, List<string> skippedMessages)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string dirAbs in EnumerateDirectoriesSafe(rootFolder, includeSubfolders, skippedMessages))
            {
                string rel = NormalizeRelativeSlashes(GetRelativePath(rootFolder, dirAbs));
                if (!string.IsNullOrWhiteSpace(rel))
                {
                    set.Add(rel);
                }
            }

            return set;
        }

        private static void AddDirectoryAncestorsFromEntries(IEnumerable<FileEntry> entries, HashSet<string> directoriesRelative)
        {
            foreach (FileEntry entry in entries)
            {
                string dir = GetRelativeDirectoryNormalized(entry.RelativePath);
                while (!string.IsNullOrWhiteSpace(dir))
                {
                    directoriesRelative.Add(dir);
                    dir = GetParentDirectoryNormalized(dir);
                }
            }
        }

        private static IEnumerable<string> EnumerateFilesSafe(string rootFolder, bool recursive, List<string> skippedMessages)
        {
            var pending = new Queue<string>();
            pending.Enqueue(rootFolder);

            while (pending.Count > 0)
            {
                string current = pending.Dequeue();
                string[] files;
                try
                {
                    files = Directory.GetFiles(current);
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is DirectoryNotFoundException || ex is PathTooLongException || ex is IOException)
                {
                    skippedMessages.Add("Skipped folder (" + ex.GetType().Name + "): " + current);
                    continue;
                }

                foreach (string file in files)
                {
                    yield return file;
                }

                if (!recursive)
                {
                    continue;
                }

                string[] directories;
                try
                {
                    directories = Directory.GetDirectories(current);
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is DirectoryNotFoundException || ex is PathTooLongException || ex is IOException)
                {
                    skippedMessages.Add("Skipped subfolders (" + ex.GetType().Name + "): " + current);
                    continue;
                }

                foreach (string directory in directories)
                {
                    pending.Enqueue(directory);
                }
            }
        }

        private static FileEntry CreateFileEntryMetadataOnly(string filePath, string rootFolder)
        {
            var fileInfo = new FileInfo(filePath);
            return new FileEntry
            {
                RelativePath = GetRelativePath(rootFolder, filePath),
                FileName = Path.GetFileName(filePath),
                SizeBytes = fileInfo.Length,
                LineCount = 0,
                Content = string.Empty
            };
        }

        private static async Task<FileEntry> ReadFileEntryAsync(string filePath, string rootFolder)
        {
            byte[] bytes;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                if (stream.Length > int.MaxValue)
                {
                    throw new IOException("File is too large to read into memory: " + filePath);
                }

                bytes = new byte[(int)stream.Length];
                int offset = 0;
                while (offset < bytes.Length)
                {
                    int read = await stream.ReadAsync(bytes, offset, bytes.Length - offset);
                    if (read == 0)
                    {
                        break;
                    }

                    offset += read;
                }
            }

            string content = DecodeWithFallback(bytes);

            int lineCount = CountLines(content);

            return new FileEntry
            {
                RelativePath = GetRelativePath(rootFolder, filePath),
                FileName = Path.GetFileName(filePath),
                SizeBytes = new FileInfo(filePath).Length,
                LineCount = lineCount,
                Content = content
            };
        }

        private static int CountLines(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return 0;
            }

            int lines = content.Count(ch => ch == '\n');
            if (!content.EndsWith("\n", StringComparison.Ordinal))
            {
                lines++;
            }

            return lines;
        }

        private static bool IsHiddenOrSystemFile(string filePath)
        {
            FileAttributes attrs = File.GetAttributes(filePath);
            return attrs.HasFlag(FileAttributes.Hidden) || attrs.HasFlag(FileAttributes.System);
        }

        private static string DecodeWithFallback(byte[] bytes)
        {
            Encoding bomEncoding = DetectBomEncoding(bytes);
            if (bomEncoding != null)
            {
                return bomEncoding.GetString(bytes);
            }

            var encodings = new[]
            {
                new UTF8Encoding(false, true),
                Encoding.GetEncoding(1251),
                Encoding.GetEncoding(1252),
                Encoding.Default
            };

            foreach (Encoding encoding in encodings)
            {
                try
                {
                    return encoding.GetString(bytes);
                }
                catch (DecoderFallbackException)
                {
                    // Try next fallback.
                }
            }

            return Encoding.Default.GetString(bytes);
        }

        private static Encoding DetectBomEncoding(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 2)
            {
                return null;
            }

            if (bytes.Length >= 3 &&
                bytes[0] == 0xEF &&
                bytes[1] == 0xBB &&
                bytes[2] == 0xBF)
            {
                return Encoding.UTF8;
            }

            if (bytes[0] == 0xFF && bytes[1] == 0xFE)
            {
                return Encoding.Unicode;
            }

            if (bytes[0] == 0xFE && bytes[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode;
            }

            return null;
        }

        private static bool MatchesExtension(string filePath, HashSet<string> allowedExtensions)
        {
            if (allowedExtensions.Count == 0)
            {
                return true;
            }

            string extension = Path.GetExtension(filePath);
            return allowedExtensions.Contains(extension ?? string.Empty);
        }

        private static bool IsInExcludedFolder(string filePath, string rootFolder, HashSet<string> excludedFolderNames)
        {
            if (excludedFolderNames.Count == 0)
            {
                return false;
            }

            string relativePath = GetRelativePath(rootFolder, filePath);
            string relativeDirectory = Path.GetDirectoryName(relativePath);

            if (string.IsNullOrWhiteSpace(relativeDirectory))
            {
                return false;
            }

            string[] segments = relativeDirectory.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            return segments.Any(s => excludedFolderNames.Contains(s));
        }

        private static string BuildOutputFilePath(string rootFolder)
        {
            string rootName = new DirectoryInfo(rootFolder).Name;
            string safeRootName = Regex.Replace(rootName, @"[^\p{L}\p{N}]", "_");
            safeRootName = Regex.Replace(safeRootName, @"_+", "_").Trim('_');
            if (string.IsNullOrWhiteSpace(safeRootName))
            {
                safeRootName = "Root";
            }

            int maxNumber = 0;
            var namePattern = new Regex(
                "^Merged_(\\d+)_" + Regex.Escape(safeRootName) + "\\.txt$",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            try
            {
                foreach (string path in Directory.EnumerateFiles(rootFolder, "*.txt", SearchOption.TopDirectoryOnly))
                {
                    string fileNameOnly = Path.GetFileName(path);
                    Match m = namePattern.Match(fileNameOnly);
                    if (m.Success &&
                        int.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n) &&
                        n > maxNumber)
                    {
                        maxNumber = n;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Use Merged_1 if listing is not allowed.
            }

            int nextNumber = maxNumber + 1;
            string fileName = string.Format(
                CultureInfo.InvariantCulture,
                "Merged_{0}_{1}.txt",
                nextNumber,
                safeRootName);
            return Path.Combine(rootFolder, fileName);
        }

        private static string BuildStructuredFolderTree(string rootFolder, List<FileEntry> entries, HashSet<string> diskDirectoriesRelative)
        {
            var sb = new StringBuilder();
            string rootName = new DirectoryInfo(rootFolder).Name;
            sb.AppendLine("-------------------------");
            sb.AppendLine("Structure: /" + rootName);
            sb.AppendLine("-");
            EmitStructuredRoot(sb, entries, diskDirectoriesRelative);
            sb.AppendLine("-------------------------");
            return sb.ToString().TrimEnd('\r', '\n');
        }

        private static void EmitStructuredRoot(StringBuilder sb, List<FileEntry> entries, HashSet<string> diskDirectoriesRelative)
        {
            List<string> rootFolders = diskDirectoriesRelative
                .Where(d => GetPathSegmentCount(d) == 1)
                .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
                .ToList();

            List<FileEntry> rootFiles = entries
                .Where(e => string.IsNullOrEmpty(GetRelativeDirectoryNormalized(e.RelativePath)))
                .OrderBy(e => e.FileName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            bool first = true;
            foreach (string folder in rootFolders)
            {
                if (!first)
                {
                    sb.AppendLine("-");
                }

                first = false;
                EmitFolderBlock(sb, folder, entries, diskDirectoriesRelative);
            }

            foreach (FileEntry file in rootFiles)
            {
                if (!first)
                {
                    sb.AppendLine("-");
                }

                first = false;
                EmitStructuredFileLine(sb, file);
            }
        }

        private static void EmitFolderBlock(StringBuilder sb, string folderRelNormalized, List<FileEntry> entries, HashSet<string> diskDirectoriesRelative)
        {
            int folderDepth = GetPathSegmentCount(folderRelNormalized);
            sb.AppendLine(new string('#', folderDepth) + " Folder: " + folderRelNormalized + "/");

            List<FileEntry> directFiles = entries
                .Where(e => string.Equals(GetRelativeDirectoryNormalized(e.RelativePath), folderRelNormalized, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.FileName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            List<string> childFolders = diskDirectoriesRelative
                .Where(d => string.Equals(GetParentDirectoryNormalized(d), folderRelNormalized, StringComparison.OrdinalIgnoreCase))
                .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (FileEntry file in directFiles)
            {
                EmitStructuredFileLine(sb, file);
            }

            bool hadDirectFiles = directFiles.Count > 0;
            for (int i = 0; i < childFolders.Count; i++)
            {
                if (hadDirectFiles || i > 0)
                {
                    sb.AppendLine("-");
                }

                EmitFolderBlock(sb, childFolders[i], entries, diskDirectoriesRelative);
            }

        }

        private static void EmitStructuredFileLine(StringBuilder sb, FileEntry file)
        {
            string normalizedPath = NormalizeRelativeSlashes(file.RelativePath);
            int depth = GetPathSegmentCount(normalizedPath);
            sb.AppendLine(
                new string('#', depth)
                + " File: "
                + file.FileName
                + " ("
                + FormatSize(file.SizeBytes)
                + ")");
        }

        private static string NormalizeRelativeSlashes(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return string.Empty;
            }

            return relativePath.Replace('\\', '/');
        }

        private static string GetRelativeDirectoryNormalized(string relativePath)
        {
            string normalized = NormalizeRelativeSlashes(relativePath);
            int idx = normalized.LastIndexOf('/');
            if (idx < 0)
            {
                return string.Empty;
            }

            return normalized.Substring(0, idx);
        }

        private static string GetParentDirectoryNormalized(string relativeNormalizedPath)
        {
            if (string.IsNullOrEmpty(relativeNormalizedPath))
            {
                return string.Empty;
            }

            int idx = relativeNormalizedPath.LastIndexOf('/');
            if (idx < 0)
            {
                return string.Empty;
            }

            return relativeNormalizedPath.Substring(0, idx);
        }

        private static int GetPathSegmentCount(string relativeNormalizedPath)
        {
            if (string.IsNullOrWhiteSpace(relativeNormalizedPath))
            {
                return 0;
            }

            int count = 1;
            foreach (char ch in relativeNormalizedPath)
            {
                if (ch == '/')
                {
                    count++;
                }
            }

            return count;
        }

        private static string GetRelativePath(string rootFolder, string filePath)
        {
            Uri rootUri = new Uri(rootFolder.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            Uri fileUri = new Uri(filePath);
            string relative = Uri.UnescapeDataString(rootUri.MakeRelativeUri(fileUri).ToString());
            return relative.Replace('/', Path.DirectorySeparatorChar);
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024)
            {
                return bytes.ToString(CultureInfo.InvariantCulture) + " B";
            }

            string[] units = { "KB", "MB", "GB", "TB" };
            double size = bytes / 1024d;
            int unitIndex = 0;
            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024d;
                unitIndex++;
            }

            return size.ToString("0.0", CultureInfo.InvariantCulture) + " " + units[unitIndex];
        }

        private HashSet<string> ParseExtensions(string rawInput)
        {
            string input = rawInput ?? string.Empty;
            if (string.IsNullOrWhiteSpace(input))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string normalized = input.Replace(";", ",").Replace(" ", ",");
            string[] parts = normalized.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                string ext = part.Trim();
                if (string.IsNullOrWhiteSpace(ext))
                {
                    continue;
                }

                if (ext == "*" || ext == ".*")
                {
                    return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                if (!ext.StartsWith(".", StringComparison.Ordinal))
                {
                    ext = "." + ext;
                }

                if (!Regex.IsMatch(ext, @"^\.[A-Za-z0-9]+$"))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, _ui.InvalidExtensionFormat, part.Trim()));
                }

                extensions.Add(ext);
            }

            return extensions;
        }

        private async Task CopyToClipboardAsync(string outputPath)
        {
            try
            {
                string text = await Task.Run(() => File.ReadAllText(outputPath, Encoding.UTF8));
                System.Windows.Clipboard.SetText(text);
                EnqueueSnackbar(_ui.SnackbarCopied);
            }
            catch (Exception ex)
            {
                EnqueueSnackbar(string.Format(CultureInfo.InvariantCulture, _ui.SnackbarClipboardWarningFormat, ex.Message));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                UpdateStatus(_ui.StatusCancelling);
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new WinForms.FolderBrowserDialog())
            {
                dialog.Description = _ui.FolderBrowseDescription;
                dialog.SelectedPath = Directory.Exists(FolderPathTextBox.Text)
                    ? FolderPathTextBox.Text
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (dialog.ShowDialog() == WinForms.DialogResult.OK)
                {
                    SelectFolderRadio.IsChecked = true;
                    FolderPathTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
            HookSettingsPersistenceEvents();
            _settingsLoaded = true;
            ValidateFolderPath();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }

        private void SourceModeChanged(object sender, RoutedEventArgs e)
        {
            bool isSelectMode = SelectFolderRadio.IsChecked == true;
            FolderPathTextBox.IsEnabled = isSelectMode;
            BrowseButton.IsEnabled = isSelectMode;
            ValidateFolderPath();
            SaveSettingsIfReady();
        }

        private void FolderPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateFolderPath();
            SaveSettingsIfReady();
        }

        private void TemplateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = TemplateComboBox.SelectedIndex;
            if (selectedIndex < 0)
            {
                return;
            }

            string mapped;
            if (_templateExtensions.TryGetValue(selectedIndex, out mapped))
            {
                ExtensionsTextBox.Text = mapped;
            }

            SaveSettingsIfReady();
        }

        private void ThemeToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ThemeToggleButton == null)
            {
                return;
            }

            bool darkMode = ThemeToggleButton.IsChecked == true;
            ApplyTheme(darkMode ? 1 : 0);
            ThemeToggleLabel.Text = darkMode ? _ui.DarkMode : _ui.LightMode;
            SaveSettingsIfReady();
        }

        private void Window_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            HandleDragState(e, true);
        }

        private void Window_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            HandleDragState(e, false);
        }

        private void Window_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            BorderBrush = _defaultBorderBrush;
        }

        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            BorderBrush = _defaultBorderBrush;
            if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                return;
            }

            string[] droppedItems = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
            if (droppedItems == null || droppedItems.Length == 0)
            {
                return;
            }

            string folder = droppedItems.FirstOrDefault(Directory.Exists);
            if (string.IsNullOrWhiteSpace(folder))
            {
                return;
            }

            SelectFolderRadio.IsChecked = true;
            FolderPathTextBox.Text = folder;
            UpdateStatus(_ui.StatusFolderDragDrop);
            SaveSettingsIfReady();
        }

        private void HandleDragState(System.Windows.DragEventArgs e, bool isEnter)
        {
            bool hasFolder = false;
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] entries = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
                if (entries != null && entries.Any(Directory.Exists))
                {
                    hasFolder = true;
                }
            }

            e.Effects = hasFolder ? System.Windows.DragDropEffects.Copy : System.Windows.DragDropEffects.None;
            e.Handled = true;

            if (hasFolder || isEnter)
            {
                BorderBrush = hasFolder ? Brushes.DarkGreen : _defaultBorderBrush;
            }
        }

        private string GetSelectedRootFolder()
        {
            if (CurrentFolderRadio.IsChecked == true)
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }

            return (FolderPathTextBox.Text ?? string.Empty).Trim();
        }

        private void ValidateFolderPath()
        {
            if (_isMerging)
            {
                return;
            }

            bool isCurrentFolderMode = CurrentFolderRadio.IsChecked == true;
            if (isCurrentFolderMode)
            {
                FolderPathTextBox.BorderBrush = _defaultBorderBrush;
                MergeButton.IsEnabled = true;
                UpdateStatus(_ui.StatusReady);
                return;
            }

            string path = (FolderPathTextBox.Text ?? string.Empty).Trim();
            bool exists = Directory.Exists(path);
            MergeButton.IsEnabled = exists;
            FolderPathTextBox.BorderBrush = exists ? _defaultBorderBrush : Brushes.IndianRed;
            UpdateStatus(exists ? _ui.StatusReady : _ui.SelectedFolderMissing);
        }

        private void SetBusyState(bool isBusy)
        {
            _isMerging = isBusy;
            MergeButton.IsEnabled = !isBusy;
            CancelButton.IsEnabled = isBusy;
            CurrentFolderRadio.IsEnabled = !isBusy;
            SelectFolderRadio.IsEnabled = !isBusy;
            FolderPathTextBox.IsEnabled = !isBusy && SelectFolderRadio.IsChecked == true;
            BrowseButton.IsEnabled = !isBusy && SelectFolderRadio.IsChecked == true;
            TemplateComboBox.IsEnabled = !isBusy;
            ExtensionsTextBox.IsEnabled = !isBusy;
            IncludeSubfoldersCheckBox.IsEnabled = !isBusy;
            IncludeHiddenFilesCheckBox.IsEnabled = !isBusy;
            IncludeEmptyFilesCheckBox.IsEnabled = !isBusy;
            FolderTreeOnlyCheckBox.IsEnabled = !isBusy;
            bool folderTreeOnlyLocked = FolderTreeOnlyCheckBox.IsChecked == true;
            IncludeTreeCheckBox.IsEnabled = !isBusy && !folderTreeOnlyLocked;
            ShowFullPathCheckBox.IsEnabled = !isBusy && !folderTreeOnlyLocked;
            CopyToClipboardCheckBox.IsEnabled = !isBusy;
            IncludeWarningsSectionCheckBox.IsEnabled = !isBusy;
            ThemeToggleButton.IsEnabled = !isBusy;
            LocalizationComboBox.IsEnabled = !isBusy;

            if (!isBusy)
            {
                MergeProgressBar.IsIndeterminate = false;
                MergeProgressBar.Value = 0;
                ValidateFolderPath();
            }
        }

        private void LoadSettings()
        {
            CurrentFolderRadio.IsChecked = Settings.Default.SourceMode == 0;
            SelectFolderRadio.IsChecked = Settings.Default.SourceMode == 1;
            FolderPathTextBox.Text = Settings.Default.LastFolderPath ?? string.Empty;
            IncludeSubfoldersCheckBox.IsChecked = Settings.Default.IncludeSubfolders;
            IncludeHiddenFilesCheckBox.IsChecked = Settings.Default.IncludeHiddenFiles;
            IncludeEmptyFilesCheckBox.IsChecked = Settings.Default.IncludeEmptyFiles;
            ExtensionsTextBox.Text = Settings.Default.ExtensionPattern ?? string.Empty;
            IncludeTreeCheckBox.IsChecked = Settings.Default.IncludeTree;
            FolderTreeOnlyCheckBox.IsChecked = Settings.Default.FolderTreeOnly;
            ShowFullPathCheckBox.IsChecked = Settings.Default.ShowFullPath;
            CopyToClipboardCheckBox.IsChecked = Settings.Default.CopyToClipboard;
            IncludeWarningsSectionCheckBox.IsChecked = Settings.Default.IncludeWarningsSection;
            ThemeToggleButton.IsChecked = Settings.Default.ThemeMode != 0;
            ApplyTheme(ThemeToggleButton.IsChecked == true ? 1 : 0);

            int uiLanguage = Settings.Default.UiLanguage;
            if (uiLanguage < 0 || uiLanguage >= UiStringsCatalog.NativeLanguageNames.Length)
            {
                uiLanguage = 0;
            }

            LocalizationComboBox.SelectedIndex = uiLanguage;

            int templateIndex = Settings.Default.LastTemplateIndex;
            if (templateIndex < 0 || templateIndex > 6)
            {
                templateIndex = 0;
            }

            TemplateComboBox.SelectedIndex = templateIndex;
            if (string.IsNullOrWhiteSpace(ExtensionsTextBox.Text) && _templateExtensions.ContainsKey(templateIndex))
            {
                ExtensionsTextBox.Text = _templateExtensions[templateIndex];
            }

            ApplyUiStrings();
        }

        private void HookSettingsPersistenceEvents()
        {
            IncludeSubfoldersCheckBox.Checked += PersistentSettingChanged;
            IncludeSubfoldersCheckBox.Unchecked += PersistentSettingChanged;
            IncludeHiddenFilesCheckBox.Checked += PersistentSettingChanged;
            IncludeHiddenFilesCheckBox.Unchecked += PersistentSettingChanged;
            IncludeEmptyFilesCheckBox.Checked += PersistentSettingChanged;
            IncludeEmptyFilesCheckBox.Unchecked += PersistentSettingChanged;
            FolderTreeOnlyCheckBox.Checked += FolderTreeOnlyCheckBox_Changed;
            FolderTreeOnlyCheckBox.Unchecked += FolderTreeOnlyCheckBox_Changed;
            IncludeTreeCheckBox.Checked += PersistentSettingChanged;
            IncludeTreeCheckBox.Unchecked += PersistentSettingChanged;
            ShowFullPathCheckBox.Checked += PersistentSettingChanged;
            ShowFullPathCheckBox.Unchecked += PersistentSettingChanged;
            CopyToClipboardCheckBox.Checked += PersistentSettingChanged;
            CopyToClipboardCheckBox.Unchecked += PersistentSettingChanged;
            IncludeWarningsSectionCheckBox.Checked += PersistentSettingChanged;
            IncludeWarningsSectionCheckBox.Unchecked += PersistentSettingChanged;
            ExtensionsTextBox.TextChanged += PersistentSettingChanged;
        }

        private void FolderTreeOnlyCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            ApplyFolderTreeOnlyDependentUi();
            SaveSettingsIfReady();
        }

        private void ApplyFolderTreeOnlyDependentUi()
        {
            if (FolderTreeOnlyCheckBox == null)
            {
                return;
            }

            bool treeOnly = FolderTreeOnlyCheckBox.IsChecked == true;
            if (treeOnly)
            {
                IncludeTreeCheckBox.IsChecked = true;
            }

            if (!_isMerging)
            {
                IncludeTreeCheckBox.IsEnabled = !treeOnly;
                ShowFullPathCheckBox.IsEnabled = !treeOnly;
            }
        }

        private void PersistentSettingChanged(object sender, RoutedEventArgs e)
        {
            SaveSettingsIfReady();
        }

        private void PersistentSettingChanged(object sender, TextChangedEventArgs e)
        {
            SaveSettingsIfReady();
        }

        private void SaveSettingsIfReady()
        {
            if (_settingsLoaded && !_isMerging)
            {
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            Settings.Default.SourceMode = CurrentFolderRadio.IsChecked == true ? 0 : 1;
            Settings.Default.LastFolderPath = (FolderPathTextBox.Text ?? string.Empty).Trim();
            Settings.Default.IncludeSubfolders = IncludeSubfoldersCheckBox.IsChecked == true;
            Settings.Default.IncludeHiddenFiles = IncludeHiddenFilesCheckBox.IsChecked == true;
            Settings.Default.IncludeEmptyFiles = IncludeEmptyFilesCheckBox.IsChecked == true;
            Settings.Default.FolderTreeOnly = FolderTreeOnlyCheckBox.IsChecked == true;
            Settings.Default.ExtensionPattern = ExtensionsTextBox.Text ?? string.Empty;
            Settings.Default.IncludeTree = IncludeTreeCheckBox.IsChecked == true;
            Settings.Default.ShowFullPath = ShowFullPathCheckBox.IsChecked == true;
            Settings.Default.CopyToClipboard = CopyToClipboardCheckBox.IsChecked == true;
            Settings.Default.IncludeWarningsSection = IncludeWarningsSectionCheckBox.IsChecked == true;
            Settings.Default.ThemeMode = ThemeToggleButton.IsChecked == true ? 1 : 0;
            Settings.Default.LastTemplateIndex = Math.Max(0, TemplateComboBox.SelectedIndex);
            int langIndex = LocalizationComboBox.SelectedIndex;
            Settings.Default.UiLanguage = langIndex < 0 ? 0 : Math.Min(langIndex, UiStringsCatalog.NativeLanguageNames.Length - 1);
            Settings.Default.Save();
        }

        private void ApplyTheme(int mode)
        {
            var helper = new PaletteHelper();
            ITheme theme = helper.GetTheme();
            theme.SetBaseTheme(mode == 1 ? Theme.Dark : Theme.Light);
            helper.SetTheme(theme);
        }

        private void ShowMergeCompleteDialog(string filePath)
        {
            string folderPath = Path.GetDirectoryName(filePath);
            MessageBoxResult choice = System.Windows.MessageBox.Show(
                _ui.SnackbarMergeComplete + Environment.NewLine + _ui.SnackbarOpenFolder + "?",
                "File Merger",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information,
                MessageBoxResult.Yes);

            if (choice != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
                {
                    Process.Start("explorer.exe", folderPath);
                }
            }
            catch (Exception ex)
            {
                EnqueueSnackbar(string.Format(CultureInfo.InvariantCulture, _ui.SnackbarCannotOpenFolderFormat, ex.Message));
            }
        }

        private void EnqueueSnackbar(string message)
        {
            MainSnackbar.MessageQueue.Enqueue(message);
        }

        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = message;
        }

        private void InitLocalizationCombo()
        {
            LocalizationComboBox.Items.Clear();
            foreach (string nativeName in UiStringsCatalog.NativeLanguageNames)
            {
                LocalizationComboBox.Items.Add(new ComboBoxItem { Content = nativeName });
            }
        }

        private void LocalizationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LocalizationComboBox == null || LocalizationComboBox.SelectedIndex < 0)
            {
                return;
            }

            ApplyUiStrings();
            SaveSettingsIfReady();
        }

        private void ApplyUiStrings()
        {
            if (LocalizationComboBox == null)
            {
                return;
            }

            int idx = LocalizationComboBox.SelectedIndex;
            if (idx < 0)
            {
                idx = 0;
            }

            _ui = UiStringsCatalog.Get(idx);
            LocalizationSectionTitle.Text = _ui.LocalizationSection;
            SourceSectionTitle.Text = _ui.Source;
            CurrentFolderRadio.Content = _ui.CurrentFolder;
            SelectFolderRadio.Content = _ui.SelectFolder;
            HintAssist.SetHint(FolderPathTextBox, _ui.FolderPathHint);
            BrowseButton.Content = _ui.Browse;
            IncludeSubfoldersCheckBox.Content = _ui.IncludeSubfolders;
            IncludeHiddenFilesCheckBox.Content = _ui.IncludeHiddenFiles;
            IncludeEmptyFilesCheckBox.Content = _ui.IncludeEmptyFiles;
            FolderTreeOnlyCheckBox.Content = _ui.FolderTreeOnly;
            FileExtensionsSectionTitle.Text = _ui.FileExtensions;
            SetTemplateComboItemContent(0, _ui.TemplateCustom);
            SetTemplateComboItemContent(1, _ui.TemplateProgramming);
            SetTemplateComboItemContent(2, _ui.TemplateWeb);
            SetTemplateComboItemContent(3, _ui.TemplateText);
            SetTemplateComboItemContent(4, _ui.TemplateConfiguration);
            SetTemplateComboItemContent(5, _ui.TemplateData);
            SetTemplateComboItemContent(6, _ui.TemplateAllFiles);
            HintAssist.SetHint(ExtensionsTextBox, _ui.ExtensionsHint);
            ExtensionsEmptyHintTextBlock.Text = _ui.ExtensionsEmptyHint;
            ThemeSectionTitle.Text = _ui.Theme;
            ThemeToggleButton.ToolTip = _ui.ThemeToggleTooltip;
            ThemeToggleLabel.Text = ThemeToggleButton.IsChecked == true ? _ui.DarkMode : _ui.LightMode;
            OutputOptionsSectionTitle.Text = _ui.OutputOptions;
            IncludeTreeCheckBox.Content = _ui.IncludeTree;
            ShowFullPathCheckBox.Content = _ui.ShowFullPath;
            CopyToClipboardCheckBox.Content = _ui.CopyToClipboard;
            IncludeWarningsSectionCheckBox.Content = _ui.IncludeWarnings;
            MergeButton.Content = _ui.Merge;
            CancelButton.Content = _ui.Cancel;

            if (!_isMerging)
            {
                ApplyFolderTreeOnlyDependentUi();
                ValidateFolderPath();
            }
        }

        private void SetTemplateComboItemContent(int index, string content)
        {
            if (index < 0 || index >= TemplateComboBox.Items.Count)
            {
                return;
            }

            var item = TemplateComboBox.Items[index] as ComboBoxItem;
            if (item != null)
            {
                item.Content = content;
            }
        }
    }
}
