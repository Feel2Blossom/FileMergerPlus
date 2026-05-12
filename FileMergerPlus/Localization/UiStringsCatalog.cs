using System;

namespace FileMergerPlus.Localization
{
    internal sealed class UiStrings
    {
        public string LocalizationSection { get; }
        public string Source { get; }
        public string CurrentFolder { get; }
        public string SelectFolder { get; }
        public string FolderPathHint { get; }
        public string Browse { get; }
        public string IncludeSubfolders { get; }
        public string IncludeHiddenFiles { get; }
        public string IncludeEmptyFiles { get; }
        public string FolderTreeOnly { get; }
        public string FileExtensions { get; }
        public string TemplateCustom { get; }
        public string TemplateProgramming { get; }
        public string TemplateWeb { get; }
        public string TemplateText { get; }
        public string TemplateConfiguration { get; }
        public string TemplateData { get; }
        public string TemplateAllFiles { get; }
        public string ExtensionsHint { get; }
        public string ExtensionsEmptyHint { get; }
        public string Theme { get; }
        public string ThemeToggleTooltip { get; }
        public string DarkMode { get; }
        public string LightMode { get; }
        public string OutputOptions { get; }
        public string IncludeTree { get; }
        public string ShowFullPath { get; }
        public string CopyToClipboard { get; }
        public string IncludeWarnings { get; }
        public string Merge { get; }
        public string Cancel { get; }
        public string StatusReady { get; }
        public string FolderNotFound { get; }
        public string StatusScanning { get; }
        public string StatusProcessedFormat { get; }
        public string StatusDoneCreatedFormat { get; }
        public string StatusCancelled { get; }
        public string SnackbarMergeCancelled { get; }
        public string ErrorFormat { get; }
        public string NoFilesMatched { get; }
        public string SnackbarCopied { get; }
        public string SnackbarClipboardWarningFormat { get; }
        public string StatusCancelling { get; }
        public string FolderBrowseDescription { get; }
        public string StatusFolderDragDrop { get; }
        public string SelectedFolderMissing { get; }
        public string InvalidExtensionFormat { get; }
        public string SnackbarCannotOpenFolderFormat { get; }
        public string SnackbarMergeComplete { get; }
        public string SnackbarOpenFolder { get; }

        public UiStrings(
            string localizationSection,
            string source,
            string currentFolder,
            string selectFolder,
            string folderPathHint,
            string browse,
            string includeSubfolders,
            string includeHiddenFiles,
            string includeEmptyFiles,
            string folderTreeOnly,
            string fileExtensions,
            string templateCustom,
            string templateProgramming,
            string templateWeb,
            string templateText,
            string templateConfiguration,
            string templateData,
            string templateAllFiles,
            string extensionsHint,
            string extensionsEmptyHint,
            string theme,
            string themeToggleTooltip,
            string darkMode,
            string lightMode,
            string outputOptions,
            string includeTree,
            string showFullPath,
            string copyToClipboard,
            string includeWarnings,
            string merge,
            string cancel,
            string statusReady,
            string folderNotFound,
            string statusScanning,
            string statusProcessedFormat,
            string statusDoneCreatedFormat,
            string statusCancelled,
            string snackbarMergeCancelled,
            string errorFormat,
            string noFilesMatched,
            string snackbarCopied,
            string snackbarClipboardWarningFormat,
            string statusCancelling,
            string folderBrowseDescription,
            string statusFolderDragDrop,
            string selectedFolderMissing,
            string invalidExtensionFormat,
            string snackbarCannotOpenFolderFormat,
            string snackbarMergeComplete,
            string snackbarOpenFolder)
        {
            LocalizationSection = localizationSection;
            Source = source;
            CurrentFolder = currentFolder;
            SelectFolder = selectFolder;
            FolderPathHint = folderPathHint;
            Browse = browse;
            IncludeSubfolders = includeSubfolders;
            IncludeHiddenFiles = includeHiddenFiles;
            IncludeEmptyFiles = includeEmptyFiles;
            FolderTreeOnly = folderTreeOnly;
            FileExtensions = fileExtensions;
            TemplateCustom = templateCustom;
            TemplateProgramming = templateProgramming;
            TemplateWeb = templateWeb;
            TemplateText = templateText;
            TemplateConfiguration = templateConfiguration;
            TemplateData = templateData;
            TemplateAllFiles = templateAllFiles;
            ExtensionsHint = extensionsHint;
            ExtensionsEmptyHint = extensionsEmptyHint;
            Theme = theme;
            ThemeToggleTooltip = themeToggleTooltip;
            DarkMode = darkMode;
            LightMode = lightMode;
            OutputOptions = outputOptions;
            IncludeTree = includeTree;
            ShowFullPath = showFullPath;
            CopyToClipboard = copyToClipboard;
            IncludeWarnings = includeWarnings;
            Merge = merge;
            Cancel = cancel;
            StatusReady = statusReady;
            FolderNotFound = folderNotFound;
            StatusScanning = statusScanning;
            StatusProcessedFormat = statusProcessedFormat;
            StatusDoneCreatedFormat = statusDoneCreatedFormat;
            StatusCancelled = statusCancelled;
            SnackbarMergeCancelled = snackbarMergeCancelled;
            ErrorFormat = errorFormat;
            NoFilesMatched = noFilesMatched;
            SnackbarCopied = snackbarCopied;
            SnackbarClipboardWarningFormat = snackbarClipboardWarningFormat;
            StatusCancelling = statusCancelling;
            FolderBrowseDescription = folderBrowseDescription;
            StatusFolderDragDrop = statusFolderDragDrop;
            SelectedFolderMissing = selectedFolderMissing;
            InvalidExtensionFormat = invalidExtensionFormat;
            SnackbarCannotOpenFolderFormat = snackbarCannotOpenFolderFormat;
            SnackbarMergeComplete = snackbarMergeComplete;
            SnackbarOpenFolder = snackbarOpenFolder;
        }
    }

    internal static class UiStringsCatalog
    {
        public static readonly string[] NativeLanguageNames =
        {
            "English",
            "Deutsch",
            "Русский",
            "中文",
            "Español",
            "Français",
            "日本語",
            "Português",
            "한국어",
            "Italiano"
        };

        private static readonly UiStrings[] All =
        {
            new UiStrings(
                "Localization",
                "Source",
                "Current folder",
                "Select folder",
                "Folder path",
                "Browse...",
                "Include subfolders",
                "Include hidden/system files",
                "Include empty files",
                "Folder tree only",
                "File extensions",
                "Custom (manual list)",
                "Programming (.cs, .py, .cpp, .h, ...)",
                "Web development (.html, .css, .js, .ts, ...)",
                "Text files (.txt, .md, .rst, .rtf, ...)",
                "Configuration (.json, .xml, .yaml, .ini, ...)",
                "Data files (.csv, .tsv, .sql, .log, .toml, ...)",
                "All files (*)",
                "Enter extensions separated by commas, e.g. .cs, .py",
                "Leave empty for all files",
                "Theme",
                "Switch between dark and light theme",
                "Dark mode",
                "Light mode",
                "Output options",
                "Add folder tree at the beginning",
                "Show full path instead of file name",
                "Copy result to clipboard after creation",
                "Include skipped-file warnings section",
                "Merge",
                "Cancel",
                "Ready",
                "The selected folder does not exist.",
                "Scanning...",
                "Processed {0} of {1}",
                "Done. File created: {0}",
                "Operation cancelled.",
                "Merge cancelled.",
                "Error: {0}",
                "No files matched your criteria.",
                "Result copied to clipboard.",
                "Clipboard warning: {0}",
                "Cancelling...",
                "Select source folder",
                "Folder selected from drag and drop.",
                "Selected folder does not exist.",
                "Invalid extension format: {0}",
                "Cannot open folder: {0}",
                "Merge complete.",
                "Open folder"),
            new UiStrings(
                "Lokalisierung",
                "Quelle",
                "Aktueller Ordner",
                "Ordner auswählen",
                "Ordnerpfad",
                "Durchsuchen...",
                "Unterordner einbeziehen",
                "Ausgeblendete/Systemdateien einbeziehen",
                "Leere Dateien einbeziehen",
                "Nur Ordnerbaum",
                "Dateierweiterungen",
                "Benutzerdefiniert (manuelle Liste)",
                "Programmierung (.cs, .py, .cpp, .h, ...)",
                "Webentwicklung (.html, .css, .js, .ts, ...)",
                "Textdateien (.txt, .md, .rst, .rtf, ...)",
                "Konfiguration (.json, .xml, .yaml, .ini, ...)",
                "Datendateien (.csv, .tsv, .sql, .log, .toml, ...)",
                "Alle Dateien (*)",
                "Erweiterungen durch Kommas trennen, z. B. .cs, .py",
                "Leer lassen für alle Dateien",
                "Design",
                "Zwischen dunklem und hellem Design wechseln",
                "Dunkelmodus",
                "Hellmodus",
                "Ausgabeoptionen",
                "Ordnerbaum am Anfang hinzufügen",
                "Vollständigen Pfad statt Dateinamen anzeigen",
                "Ergebnis nach Erstellung in die Zwischenablage kopieren",
                "Abschnitt mit Warnungen zu übersprungenen Dateien",
                "Zusammenführen",
                "Abbrechen",
                "Bereit",
                "Der ausgewählte Ordner existiert nicht.",
                "Scanne...",
                "{0} von {1} verarbeitet",
                "Fertig. Datei erstellt: {0}",
                "Vorgang abgebrochen.",
                "Zusammenführung abgebrochen.",
                "Fehler: {0}",
                "Keine Dateien entsprechen Ihren Kriterien.",
                "Ergebnis in die Zwischenablage kopiert.",
                "Zwischenablage-Warnung: {0}",
                "Wird abgebrochen...",
                "Quellordner auswählen",
                "Ordner per Drag & Drop ausgewählt.",
                "Ausgewählter Ordner existiert nicht.",
                "Ungültiges Erweiterungsformat: {0}",
                "Ordner kann nicht geöffnet werden: {0}",
                "Zusammenführung abgeschlossen.",
                "Ordner öffnen"),
            new UiStrings(
                "Локализация",
                "Источник",
                "Текущая папка",
                "Выбрать папку",
                "Путь к папке",
                "Обзор...",
                "Включать вложенные папки",
                "Включать скрытые файлы",
                "Включать пустые файлы",
                "Только дерево папок",
                "Расширения файлов",
                "Другое (список вручную)",
                "Программирование (.cs, .py, .cpp, .h, ...)",
                "Веб-разработка (.html, .css, .js, .ts, ...)",
                "Текстовые файлы (.txt, .md, .rst, .rtf, ...)",
                "Конфигурация (.json, .xml, .yaml, .ini, ...)",
                "Файлы данных (.csv, .tsv, .sql, .log, .toml, ...)",
                "Все файлы (*)",
                "Введите расширения через запятую, напр. .cs, .py",
                "Оставьте пустым для всех файлов",
                "Тема",
                "Переключение тёмной и светлой темы",
                "Тёмная тема",
                "Светлая тема",
                "Параметры вывода",
                "Добавить дерево папок в начале",
                "Показывать полный путь вместо имени файла",
                "Копировать результат в буфер после создания",
                "Включать раздел предупреждений о пропущенных файлах",
                "Объединить",
                "Отмена",
                "Готово",
                "Выбранная папка не существует.",
                "Сканирование...",
                "Обработано {0} из {1}",
                "Готово. Файл создан: {0}",
                "Операция отменена.",
                "Объединение отменено.",
                "Ошибка: {0}",
                "Нет файлов, соответствующих критериям.",
                "Результат скопирован в буфер обмена.",
                "Предупреждение буфера: {0}",
                "Отмена...",
                "Выберите исходную папку",
                "Папка выбрана перетаскиванием.",
                "Выбранная папка не существует.",
                "Неверный формат расширения: {0}",
                "Не удалось открыть папку: {0}",
                "Объединение завершено.",
                "Открыть папку"),
            new UiStrings(
                "本地化",
                "来源",
                "当前文件夹",
                "选择文件夹",
                "文件夹路径",
                "浏览...",
                "包含子文件夹",
                "包含隐藏/系统文件",
                "包含空文件",
                "仅文件夹树",
                "文件扩展名",
                "自定义（手动列表）",
                "编程 (.cs, .py, .cpp, .h, ...)",
                "Web 开发 (.html, .css, .js, .ts, ...)",
                "文本文件 (.txt, .md, .rst, .rtf, ...)",
                "配置 (.json, .xml, .yaml, .ini, ...)",
                "数据文件 (.csv, .tsv, .sql, .log, .toml, ...)",
                "所有文件 (*)",
                "输入扩展名，用逗号分隔，例如 .cs、.py",
                "留空表示所有文件",
                "主题",
                "在深色与浅色主题之间切换",
                "深色模式",
                "浅色模式",
                "输出选项",
                "在开头添加文件夹树",
                "显示完整路径而非文件名",
                "创建后将结果复制到剪贴板",
                "包含已跳过文件的警告部分",
                "合并",
                "取消",
                "就绪",
                "所选文件夹不存在。",
                "正在扫描...",
                "已处理 {0} / {1}",
                "完成。已创建文件：{0}",
                "操作已取消。",
                "合并已取消。",
                "错误：{0}",
                "没有符合条件的文件。",
                "结果已复制到剪贴板。",
                "剪贴板警告：{0}",
                "正在取消...",
                "选择源文件夹",
                "已通过拖放选择文件夹。",
                "所选文件夹不存在。",
                "无效的扩展名格式：{0}",
                "无法打开文件夹：{0}",
                "合并完成。",
                "打开文件夹"),
            new UiStrings(
                "Localización",
                "Origen",
                "Carpeta actual",
                "Seleccionar carpeta",
                "Ruta de la carpeta",
                "Examinar...",
                "Incluir subcarpetas",
                "Incluir archivos ocultos/del sistema",
                "Incluir archivos vacíos",
                "Solo árbol de carpetas",
                "Extensiones de archivo",
                "Personalizado (lista manual)",
                "Programación (.cs, .py, .cpp, .h, ...)",
                "Desarrollo web (.html, .css, .js, .ts, ...)",
                "Archivos de texto (.txt, .md, .rst, .rtf, ...)",
                "Configuración (.json, .xml, .yaml, .ini, ...)",
                "Archivos de datos (.csv, .tsv, .sql, .log, .toml, ...)",
                "Todos los archivos (*)",
                "Introduzca extensiones separadas por comas, p. ej. .cs, .py",
                "Dejar vacío para todos los archivos",
                "Tema",
                "Cambiar entre tema oscuro y claro",
                "Modo oscuro",
                "Modo claro",
                "Opciones de salida",
                "Añadir árbol de carpetas al inicio",
                "Mostrar ruta completa en lugar del nombre",
                "Copiar resultado al portapapeles al terminar",
                "Incluir sección de advertencias de archivos omitidos",
                "Combinar",
                "Cancelar",
                "Listo",
                "La carpeta seleccionada no existe.",
                "Escaneando...",
                "Procesados {0} de {1}",
                "Hecho. Archivo creado: {0}",
                "Operación cancelada.",
                "Combinación cancelada.",
                "Error: {0}",
                "Ningún archivo coincide con los criterios.",
                "Resultado copiado al portapapeles.",
                "Advertencia del portapapeles: {0}",
                "Cancelando...",
                "Seleccione la carpeta de origen",
                "Carpeta seleccionada por arrastrar y soltar.",
                "La carpeta seleccionada no existe.",
                "Formato de extensión no válido: {0}",
                "No se puede abrir la carpeta: {0}",
                "Combinación completada.",
                "Abrir carpeta"),
            new UiStrings(
                "Localisation",
                "Source",
                "Dossier actuel",
                "Choisir un dossier",
                "Chemin du dossier",
                "Parcourir...",
                "Inclure les sous-dossiers",
                "Inclure les fichiers cachés/système",
                "Inclure les fichiers vides",
                "Arborescence uniquement",
                "Extensions de fichier",
                "Personnalisé (liste manuelle)",
                "Programmation (.cs, .py, .cpp, .h, ...)",
                "Développement web (.html, .css, .js, .ts, ...)",
                "Fichiers texte (.txt, .md, .rst, .rtf, ...)",
                "Configuration (.json, .xml, .yaml, .ini, ...)",
                "Fichiers de données (.csv, .tsv, .sql, .log, .toml, ...)",
                "Tous les fichiers (*)",
                "Saisissez les extensions séparées par des virgules, ex. .cs, .py",
                "Laisser vide pour tous les fichiers",
                "Thème",
                "Basculer entre thème sombre et clair",
                "Mode sombre",
                "Mode clair",
                "Options de sortie",
                "Ajouter l'arborescence au début",
                "Afficher le chemin complet au lieu du nom",
                "Copier le résultat dans le presse-papiers après création",
                "Inclure la section d'avertissements sur les fichiers ignorés",
                "Fusionner",
                "Annuler",
                "Prêt",
                "Le dossier sélectionné n'existe pas.",
                "Analyse...",
                "Traité {0} sur {1}",
                "Terminé. Fichier créé : {0}",
                "Opération annulée.",
                "Fusion annulée.",
                "Erreur : {0}",
                "Aucun fichier ne correspond à vos critères.",
                "Résultat copié dans le presse-papiers.",
                "Avertissement presse-papiers : {0}",
                "Annulation...",
                "Sélectionnez le dossier source",
                "Dossier sélectionné par glisser-déposer.",
                "Le dossier sélectionné n'existe pas.",
                "Format d'extension invalide : {0}",
                "Impossible d'ouvrir le dossier : {0}",
                "Fusion terminée.",
                "Ouvrir le dossier"),
            new UiStrings(
                "ローカライズ",
                "ソース",
                "現在のフォルダー",
                "フォルダーを選択",
                "フォルダー パス",
                "参照...",
                "サブフォルダーを含める",
                "隠し/システム ファイルを含める",
                "空のファイルを含める",
                "フォルダー ツリーのみ",
                "ファイル拡張子",
                "カスタム（手動リスト）",
                "プログラミング (.cs, .py, .cpp, .h, ...)",
                "Web 開発 (.html, .css, .js, .ts, ...)",
                "テキスト ファイル (.txt, .md, .rst, .rtf, ...)",
                "設定 (.json, .xml, .yaml, .ini, ...)",
                "データ ファイル (.csv, .tsv, .sql, .log, .toml, ...)",
                "すべてのファイル (*)",
                "拡張子をカンマ区切りで入力（例: .cs, .py）",
                "空欄ですべてのファイル",
                "テーマ",
                "ダークとライトのテーマを切り替え",
                "ダーク モード",
                "ライト モード",
                "出力オプション",
                "先頭にフォルダー ツリーを追加",
                "ファイル名の代わりに完全パスを表示",
                "作成後に結果をクリップボードへコピー",
                "スキップしたファイルの警告セクションを含める",
                "マージ",
                "キャンセル",
                "準備完了",
                "選択したフォルダーが存在しません。",
                "スキャン中...",
                "{0} / {1} 件を処理",
                "完了。ファイルを作成しました: {0}",
                "操作はキャンセルされました。",
                "マージをキャンセルしました。",
                "エラー: {0}",
                "条件に一致するファイルがありません。",
                "結果をクリップボードにコピーしました。",
                "クリップボードの警告: {0}",
                "キャンセル中...",
                "ソース フォルダーを選択",
                "ドラッグ アンド ドロップでフォルダーを選択しました。",
                "選択したフォルダーが存在しません。",
                "拡張子の形式が無効です: {0}",
                "フォルダーを開けません: {0}",
                "マージが完了しました。",
                "フォルダーを開く"),
            new UiStrings(
                "Localização",
                "Origem",
                "Pasta atual",
                "Selecionar pasta",
                "Caminho da pasta",
                "Procurar...",
                "Incluir subpastas",
                "Incluir arquivos ocultos/do sistema",
                "Incluir arquivos vazios",
                "Somente árvore de pastas",
                "Extensões de arquivo",
                "Personalizado (lista manual)",
                "Programação (.cs, .py, .cpp, .h, ...)",
                "Desenvolvimento web (.html, .css, .js, .ts, ...)",
                "Arquivos de texto (.txt, .md, .rst, .rtf, ...)",
                "Configuração (.json, .xml, .yaml, .ini, ...)",
                "Arquivos de dados (.csv, .tsv, .sql, .log, .toml, ...)",
                "Todos os arquivos (*)",
                "Digite extensões separadas por vírgulas, ex. .cs, .py",
                "Deixe vazio para todos os arquivos",
                "Tema",
                "Alternar entre tema escuro e claro",
                "Modo escuro",
                "Modo claro",
                "Opções de saída",
                "Adicionar árvore de pastas no início",
                "Mostrar caminho completo em vez do nome",
                "Copiar resultado para a área de transferência após criar",
                "Incluir seção de avisos de arquivos ignorados",
                "Mesclar",
                "Cancelar",
                "Pronto",
                "A pasta selecionada não existe.",
                "Analisando...",
                "Processados {0} de {1}",
                "Concluído. Arquivo criado: {0}",
                "Operação cancelada.",
                "Mesclagem cancelada.",
                "Erro: {0}",
                "Nenhum arquivo corresponde aos critérios.",
                "Resultado copiado para a área de transferência.",
                "Aviso da área de transferência: {0}",
                "Cancelando...",
                "Selecione a pasta de origem",
                "Pasta selecionada por arrastar e soltar.",
                "A pasta selecionada não existe.",
                "Formato de extensão inválido: {0}",
                "Não é possível abrir a pasta: {0}",
                "Mesclagem concluída.",
                "Abrir pasta"),
            new UiStrings(
                "언어",
                "원본",
                "현재 폴더",
                "폴더 선택",
                "폴더 경로",
                "찾아보기...",
                "하위 폴더 포함",
                "숨김/시스템 파일 포함",
                "빈 파일 포함",
                "폴더 트리만",
                "파일 확장명",
                "사용자 지정(수동 목록)",
                "프로그래밍 (.cs, .py, .cpp, .h, ...)",
                "웹 개발 (.html, .css, .js, .ts, ...)",
                "텍스트 파일 (.txt, .md, .rst, .rtf, ...)",
                "구성 (.json, .xml, .yaml, .ini, ...)",
                "데이터 파일 (.csv, .tsv, .sql, .log, .toml, ...)",
                "모든 파일 (*)",
                "확장명을 쉼표로 구분하여 입력(예: .cs, .py)",
                "비워 두면 모든 파일",
                "테마",
                "어두운 테마와 밝은 테마 전환",
                "다크 모드",
                "라이트 모드",
                "출력 옵션",
                "맨 앞에 폴더 트리 추가",
                "파일 이름 대신 전체 경로 표시",
                "만든 후 결과를 클립보드에 복사",
                "건너뛴 파일 경고 섹션 포함",
                "병합",
                "취소",
                "준비됨",
                "선택한 폴더가 없습니다.",
                "검색 중...",
                "{0}/{1} 처리됨",
                "완료. 파일이 만들어졌습니다: {0}",
                "작업이 취소되었습니다.",
                "병합이 취소되었습니다.",
                "오류: {0}",
                "조건에 맞는 파일이 없습니다.",
                "결과가 클립보드에 복사되었습니다.",
                "클립보드 경고: {0}",
                "취소 중...",
                "원본 폴더 선택",
                "끌어서 놓기로 폴더를 선택했습니다.",
                "선택한 폴더가 없습니다.",
                "잘못된 확장명 형식: {0}",
                "폴더를 열 수 없습니다: {0}",
                "병합이 완료되었습니다.",
                "폴더 열기"),
            new UiStrings(
                "Localizzazione",
                "Origine",
                "Cartella corrente",
                "Seleziona cartella",
                "Percorso cartella",
                "Sfoglia...",
                "Includi sottocartelle",
                "Includi file nascosti/di sistema",
                "Includi file vuoti",
                "Solo albero cartelle",
                "Estensioni file",
                "Personalizzato (elenco manuale)",
                "Programmazione (.cs, .py, .cpp, .h, ...)",
                "Sviluppo web (.html, .css, .js, .ts, ...)",
                "File di testo (.txt, .md, .rst, .rtf, ...)",
                "Configurazione (.json, .xml, .yaml, .ini, ...)",
                "File di dati (.csv, .tsv, .sql, .log, .toml, ...)",
                "Tutti i file (*)",
                "Inserisci estensioni separate da virgola, es. .cs, .py",
                "Lascia vuoto per tutti i file",
                "Tema",
                "Passa dal tema scuro a quello chiaro",
                "Modalità scura",
                "Modalità chiara",
                "Opzioni di output",
                "Aggiungi albero cartelle all'inizio",
                "Mostra percorso completo invece del nome file",
                "Copia risultato negli appunti dopo la creazione",
                "Includi sezione avvisi file saltati",
                "Unisci",
                "Annulla",
                "Pronto",
                "La cartella selezionata non esiste.",
                "Scansione in corso...",
                "Elaborati {0} di {1}",
                "Fatto. File creato: {0}",
                "Operazione annullata.",
                "Unione annullata.",
                "Errore: {0}",
                "Nessun file corrisponde ai criteri.",
                "Risultato copiato negli appunti.",
                "Avviso appunti: {0}",
                "Annullamento...",
                "Seleziona cartella di origine",
                "Cartella selezionata con trascinamento.",
                "La cartella selezionata non esiste.",
                "Formato estensione non valido: {0}",
                "Impossibile aprire la cartella: {0}",
                "Unione completata.",
                "Apri cartella")
        };

        public static UiStrings Get(int languageIndex)
        {
            if (languageIndex < 0 || languageIndex >= All.Length)
            {
                return All[0];
            }

            return All[languageIndex];
        }
    }
}
