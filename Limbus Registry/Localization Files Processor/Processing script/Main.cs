using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using static LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing.LocalizationFilesProcessorWindow;
using static LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing.Modules.JsonPathRegexConversions;
using static LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing.Modules.MergedFontShenanigans;
using static LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing.Modules.MissingJsonObjects;
using static LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing.Modules.Shorthands;
using static LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing.Modules.StylePlaceholders;

namespace LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing
{
    namespace Modules
    {
        public static class Main
        {
            public static bool IsProcessing { get; private set; } = false;


            public static CancellationTokenSource CurrentLocalizationFilesProcessingCancelTokenSource { get; private set; } = new();


            public static async Task DoDirectExport()
            {
                CurrentLocalizationFilesProcessingCancelTokenSource = new();

                LocalizationFilesProcessorWindowInstance.TaskBarProgress.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;

                LocalizationFilesProcessorWindowInstance.FilesCounter.Visibility = Visibility.Visible;

                LocalizationFilesProcessorWindowInstance.StartButton.Visibility = Visibility.Collapsed;
                LocalizationFilesProcessorWindowInstance.CancelButton.Visibility = Visibility.Visible;
                LocalizationFilesProcessorWindowInstance.ExportedFilesLog.Children.Clear();

                LoadedMergedFontReplacementMap.Clear();
                LoadedMergedFontMultipleApplyConfig.Clear();

                var @Profile = DataContextDomain.LocalizationFilesProcessor.Profile; // Short

                bool CanProcecss = true;
                bool ProcessingWasCancelled = false;

                IsProcessing = true;

                try
                {
                    #region Load selected config files
                    if (@Profile.MergedFonts.ConvertFontsByMultipleApplyConfig | @Profile.MergedFonts.ConvertFontsByMarkers)
                    {
                        if (File.Exists(@Profile.MergedFonts.MergedFontCharactersReplacementMap))
                        {
                            LoadedMergedFontReplacementMap = new FileInfo(@Profile.MergedFonts.MergedFontCharactersReplacementMap).DeserealizeJsonAs<Dictionary<string, Dictionary<string, string>>>()!;
                        }
                    }


                    if (@Profile.MergedFonts.ConvertFontsByMultipleApplyConfig)
                    {
                        if (File.Exists(@Profile.MergedFonts.MergedFontMultipleApplyConfig))
                        {
                            LoadedMergedFontMultipleApplyConfig = new FileInfo(@Profile.MergedFonts.MergedFontMultipleApplyConfig).DeserealizeJsonAs<Dictionary<string, List<MergedFontRule>>>()!;

                            if (CheckForInvalidMergedFontFontRules()) CanProcecss = false;
                        }
                    }


                    if (@Profile.Misc.DoJsonPathMultipleRegexConversions)
                    {
                        if (File.Exists(@Profile.Misc.JsonPathMultipleRegexConversionsConfigFile))
                        {
                            LoadJsonPathMultipleRegexConversions(@Profile.Misc.JsonPathMultipleRegexConversionsConfigFile);
                        }
                    }
                    #endregion

                    if (CanProcecss)
                    {
                        #region Local methods
                        static void CreateDirectoryTree(string HeaderDirectory, List<string> SubDirectories)
                        {
                            if (Directory.Exists(HeaderDirectory) == false)
                            {
                                Directory.CreateDirectory(HeaderDirectory);
                            }

                            foreach (string RelativeSubDir in SubDirectories)
                            {
                                if (Directory.Exists(@$"{HeaderDirectory}\{RelativeSubDir}") == false & RelativeSubDir != "") Directory.CreateDirectory(@$"{HeaderDirectory}\{RelativeSubDir}");
                            }
                        }


                        #region Wildcards
                        static List<string> SplitWildcardsString(string WildcardsDelimitedByCommas)
                        {
                            List<string> PatternsList = [.. WildcardsDelimitedByCommas.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(Pattern => Pattern.Trim())
                                .Where(Pattern => !string.IsNullOrWhiteSpace(Pattern))];

                            return PatternsList;
                        }

                        static bool FindMatchedWildcardPatterns(string CheckString, List<string> GivenWildcardPatterns, out List<string> MatchedWildcardPatterns)
                        {
                            MatchedWildcardPatterns = [];
                            foreach (string WildcardPattern in GivenWildcardPatterns)
                            {
                                const string StarMarker = "\0WILDCARD_STAR\0";
                                const string QuestionMarker = "\0WILDCARD_QUESTION\0";

                                string WildcardTransformedToRegex = Regex.Escape( WildcardPattern.Replace("*", StarMarker).Replace("?", QuestionMarker) ).Replace(StarMarker, ".*?").Replace(QuestionMarker, ".");

                                if (Regex.Match(CheckString, WildcardTransformedToRegex).Success)
                                {
                                    MatchedWildcardPatterns.Add(WildcardPattern);
                                }
                            }

                            return MatchedWildcardPatterns.Count > 0;
                        }
                        static bool MatchesWildcardPatterns(string CheckString, List<string> GivenWildcardPatterns)
                        {
                            return FindMatchedWildcardPatterns(CheckString, GivenWildcardPatterns, out _);
                        }
                        #endregion


                        bool CanOverwriteExistingFile(string Filepath, string CurrentText)
                        {
                            if (File.Exists(Filepath)) // Check for skip if file is already exists and equals to final version
                            {
                                switch (@Profile.JsonFilesFormatting.ExistingFilesOverwritingRule)
                                {
                                    case "If Json content itself is not same":
                                        try
                                        {
                                            JToken ExistingFileJToken = JToken.Parse(StreamReadText(Filepath /* Already existing file by this path */));

                                            if (JToken.DeepEquals(JToken.Parse(CurrentText), ExistingFileJToken))  /**/ return false /**/;
                                        }
                                        catch { return true; /* Json syntax or file reading error = wrong syntax = can */ }

                                        break;


                                    case "If just text of both files is not same":
                                        string ExistingFileText = StreamReadText(Filepath);

                                        if (CurrentText == ExistingFileText)  /**/ return false /**/;

                                        break;


                                    case "Always overwrite existing files": /** :P **/ break;


                                    default: goto case "If Json content itself is not same";
                                }
                            }
                            
                            return true;
                        }
                        #endregion



                        #region Json format
                        LineBreakMode? SelectedOutputLineBreakMode = Enum.TryParse(@Profile.JsonFilesFormatting.LineBreakMode, out LineBreakMode ResultLineBreakMode)
                            ? ResultLineBreakMode
                            : null;

                        Formatting SelectedOutputFormatting = Enum.TryParse(@Profile.JsonFilesFormatting.OutputJsonFormatting, out Formatting ResultFormatting)
                            ? ResultFormatting
                            : Formatting.Indented;

                        int? SelectedOutputIndentationSize = int.TryParse(@Profile.JsonFilesFormatting.JsonIndentationSize, out int ResultIndentationSize)
                            ? ResultIndentationSize
                            : null;
                        #endregion


                        #region Full paths
                        string SourceDirectory = Path.GetFullPath(@Profile.Paths.SourceDirectory.Trim());
                        string DestinationDirectory = Path.GetFullPath(@Profile.Paths.DestinationDirectory.Trim());
                        string ReferenceDirectory = Directory.Exists(@Profile.ReferenceLocalization.Directory.Trim()) ? Path.GetFullPath(@Profile.ReferenceLocalization.Directory.Trim()) : "asdasdasd\0";
                        #endregion


                        #region Directories creation
                        CreateDirectoryTree(
                            HeaderDirectory: DestinationDirectory,
                            SubDirectories: [
                                @"BattleAnnouncerDlg",
                                @"BgmLyrics",
                                @"EGOVoiceDig",
                                @"PersonalityVoiceDlg",
                                @"StoryData",
                                @Profile.FontFiles.AlsoCopyFontFiles ? @"Font"         : @"",
                                @Profile.FontFiles.AlsoCopyFontFiles ? @"Font\Context" : @"",
                                @Profile.FontFiles.AlsoCopyFontFiles ? @"Font\Title"   : @""
                            ]
                        );
                        #endregion


                        #region Files whitelist, order and targets
                        List<string> GeneralFilesWhitelist = SplitWildcardsString(@Profile.GeneralLocalizationFilesWhitelist);

                        IEnumerable<FileInfo> TargetFiles = new DirectoryInfo(SourceDirectory)
                            .GetFiles("*.json", SearchOption.AllDirectories)
                            .Where(LocalizationFile => LocalizationFile.FullName.Cut(SourceDirectory).Contains(@".vs\") == false) // Ignore ".vs" folder of Visual Studio (Not of VS Code)
                            .Where(LocalizationFile => MatchesWildcardPatterns(LocalizationFile.FullName.Cut(SourceDirectory + "\\").Replace("\\", "/"), GeneralFilesWhitelist)); // General whitelist


                        if (@Profile.FilesProcessingOrder == "Recently changed first")
                        {
                            TargetFiles = TargetFiles.OrderBy(LocalizationFile => LocalizationFile.LastWriteTime).Reverse();
                        }
                        #endregion


                        #region Files counter reset
                        int ProcessedFilesCounter = 0;
                        int TotalFilesCounter = 0;
                        int TotalFilesCount = TargetFiles.Count();

                        LocalizationFilesProcessorWindowInstance.FilesCounter.Text = $"0 / {TotalFilesCount}";
                        #endregion


                        #region General conditions
                        List<string> WhiteListOfMissingFilesToAppend
                            = SplitWildcardsString(@Profile.ReferenceLocalization.MissingContentAppending.WhitelistOfFilesToAppend);

                        List<string> WhiteListOfFilesToAddMissingIDsTo
                            = SplitWildcardsString(@Profile.ReferenceLocalization.MissingContentAppending.WhitelistOfFilesToAddIDsTo);


                        List<string> KeywordsShorthandsApplyingRange
                            = SplitWildcardsString(@Profile.Misc.KeywordShorthandsFilesWhiteList);
                        #endregion


                        #region Font files copying
                        if (@Profile.FontFiles.AlsoCopyFontFiles)
                        {
                            if (File.Exists(@Profile.FontFiles.ContextFontFile))
                            {
                                string Destination = @$"{DestinationDirectory}\Font\Context\{Path.GetFileName(@Profile.FontFiles.ContextFontFile)}";
                                if (File.Exists(Destination)) File.Delete(Destination);
                                File.Copy(@Profile.FontFiles.ContextFontFile, Destination);
                            }

                            if (File.Exists(@Profile.FontFiles.TitleFontFile))
                            {
                                string Destination = @$"{DestinationDirectory}\Font\Title\{Path.GetFileName(@Profile.FontFiles.TitleFontFile)}";
                                if (File.Exists(Destination)) File.Delete(Destination);
                                File.Copy(@Profile.FontFiles.TitleFontFile, @$"{DestinationDirectory}\Font\Title\{Path.GetFileName(@Profile.FontFiles.TitleFontFile)}");
                            }
                        }
                        #endregion



                        List<string> PrimaryExportedFiles = [];
                        List<string> PrimaryCheckedFiles = [];


                        void ProcessFile(FileInfo LocalizationFile, bool IsFromReference)
                        {
                            // Cancel
                            if (CurrentLocalizationFilesProcessingCancelTokenSource.Token.IsCancellationRequested)
                            {
                                ProcessingWasCancelled = true;
                                CurrentLocalizationFilesProcessingCancelTokenSource.Token.ThrowIfCancellationRequested();
                            }



                            LocalizationFilesProcessorWindowInstance.Dispatcher.Invoke(delegate ()
                            {
                                LocalizationFilesProcessorWindowInstance.TaskBarProgress.ProgressValue = (double)TotalFilesCounter / (double)TotalFilesCount;
                                LocalizationFilesProcessorWindowInstance.FilesCounter.Text = $"{TotalFilesCounter} / {TotalFilesCount}";
                            });

                            PrimaryCheckedFiles.Add(LocalizationFile.Name);


                            string TargetFile_RelativePath = LocalizationFile.FullName.Cut((IsFromReference ? ReferenceDirectory : SourceDirectory) + "\\");
                            if (IsFromReference)
                            {
                                TargetFile_RelativePath = TargetFile_RelativePath.RegexReplace(LocalizationFile.Name + '$', LocalizationFile.Name.RemovePrefix(@Profile.ReferenceLocalization.Prefix));
                            }

                            string OutputFile_DestinationFullPath = DestinationDirectory + "\\" + TargetFile_RelativePath;
                            
                            // AbDlg_DonQuixote.json, PersonalityVoiceDlg/Voice_DonQuixote_Bloodfiend_10310.json, ... (With regular slash instead of backslash)
                            string RelativePathWithoutBackslashes = TargetFile_RelativePath.Replace("\\", "/");


                            try
                            {
                                // Currently not processed at this point
                                string OriginalFileText = StreamReadText(LocalizationFile.FullName);
                                string ProcessedFileText = OriginalFileText;

                                {
                                    JToken.Parse(ProcessedFileText); // Primary check for json synatax errors (Throws exception if something is wrong -> `catch`)
                                }

                                UTF8Encoding OuputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: Profile.JsonFilesFormatting.UTF8ByteOrderMarks switch
                                {
                                    "Keep BOM if its inside original file"   => LocalizationFile.EncodingHasBOM(),
                                    "Save files with UTF-8 BOM encoding"     => true,
                                    "Save files with regular UTF-8 encoding" => false,
                                    _                                        => LocalizationFile.EncodingHasBOM(),
                                });
                                int OriginalJsonIndentationSize = ProcessedFileText.GetJsonIndentationSize();
                                LineBreakMode OriginalLineBreakMode = ProcessedFileText.DetermineLineBreakType();

                                {
                                    ProcessedFileText = ProcessedFileText.DeserealizeJsonAs<LimbusLocalizationFile<dynamic>>()!.SerializeToFormattedJsonText(); // Format normalizing for following conversions
                                }



                                #region Process

                                // Append missing IDs
                                if (IsFromReference == false)
                                {
                                    #region Reference file path
                                    // EN_ KR_ JP_ Prefix insertion to the right place
                                    string ReferenceFile_RelativePath = TargetFile_RelativePath.Contains('\\')
                                        ? TargetFile_RelativePath.Insert(TargetFile_RelativePath.LastIndexOf('\\') + 1, Profile.ReferenceLocalization.Prefix)
                                        : $"{@Profile.ReferenceLocalization.Prefix}{TargetFile_RelativePath}";

                                    string ReferenceFile_FullPath = ReferenceDirectory + "\\" + ReferenceFile_RelativePath;
                                    #endregion

                                    
                                    if (@Profile.ReferenceLocalization.MissingContentAppending.AppendMissingIDs &&
                                        MatchesWildcardPatterns(TargetFile_RelativePath, WhiteListOfFilesToAddMissingIDsTo)
                                    ) {
                                        if (File.Exists(ReferenceFile_FullPath))
                                        {
                                            ProcessedFileText = CompareAppendDataList(
                                                TargetJson: ProcessedFileText,
                                                ReferenceJson: StreamReadText(ReferenceFile_FullPath)
                                            );
                                        }
                                    }
                                }



                                // Add <style> placeholders
                                if (@Profile.Misc.AddStylePlaceholders &&
                                    LocalizationFile.Name.ToLower().ContainsOneOf("skills.json", "skills_ego.json", "_personality-")
                                )
                                {
                                    ProcessedFileText = AddStyleHighlightPlaceholders(SkillsJsonText: ProcessedFileText);
                                }



                                // Do Regex Multiple Conversions
                                if (@Profile.Misc.DoJsonPathMultipleRegexConversions &&
                                    FindMatchedWildcardPatterns(TargetFile_RelativePath, SplitWildcardsString(string.Join(", ", LoadedJsonPathMultipleRegexConversions.Keys)), out List<string> MatchedFileNamePatternsForRegexConversions)
                                ) {
                                    foreach (string MatchedFileNamePattern in MatchedFileNamePatternsForRegexConversions)
                                    {
                                        ProcessedFileText = DoMultipleRegexConversions(
                                            JsonText: ProcessedFileText,
                                            ReplacementRules: LoadedJsonPathMultipleRegexConversions[MatchedFileNamePattern]
                                        );
                                    }
                                }



                                // Convert Shorthands
                                if (IsFromReference == false)
                                {
                                    if (@Profile.Misc.ConvertKeywordShorthands &&
                                        @Profile.Misc.KeywordShorthandsRegexPattern != @"" &&
                                        MatchesWildcardPatterns(TargetFile_RelativePath, KeywordsShorthandsApplyingRange)
                                    ) {
                                        ProcessedFileText = ConvertShorthands(
                                            JsonText: ProcessedFileText,
                                            ShorthandsPattern: @Profile.Misc.KeywordShorthandsRegexPattern
                                        );
                                    }
                                }



                                // Apply merged fonts by markers
                                if (IsFromReference == false)
                                {
                                    if (@Profile.MergedFonts.ConvertFontsByMarkers &&
                                        ProcessedFileText.Contains("[font=")
                                    ) {
                                        ProcessedFileText = PlaceMergedFontByMarkers(
                                            JsonText: ProcessedFileText,
                                            LoggingFileName: RelativePathWithoutBackslashes
                                        );
                                    }
                                }



                                // Apply merged fonts by multiple apply config
                                if (@Profile.MergedFonts.ConvertFontsByMultipleApplyConfig &&
                                    FindMatchedWildcardPatterns(TargetFile_RelativePath, SplitWildcardsString(string.Join(", ", LoadedMergedFontMultipleApplyConfig.Keys)), out List<string> MatchedFileNamePatternsForMergedFont)
                                ) {
                                    foreach (string MatchedFileNamePattern in MatchedFileNamePatternsForMergedFont)
                                    {
                                        ProcessedFileText = PlaceMergedFontsByMultipleApplyConfig(
                                            JsonText: ProcessedFileText,
                                            FontRules: LoadedMergedFontMultipleApplyConfig[MatchedFileNamePattern]
                                        );
                                    }
                                }

                                string FinalJsonFile = ProcessedFileText.DeserealizeJsonAs<LimbusLocalizationFile<dynamic>>()!.SerializeToFormattedJsonText
                                (
                                    Formatting: SelectedOutputFormatting,

                                    IndentationSize: SelectedOutputIndentationSize != null
                                        ? (int)SelectedOutputIndentationSize
                                        : OriginalJsonIndentationSize,

                                    LineBreakMode: SelectedOutputLineBreakMode != null
                                        ? (LineBreakMode)SelectedOutputLineBreakMode
                                        : OriginalLineBreakMode
                                );

                                #endregion


                                // Cancel (Also before saving file that maybe caused cancelling due to ConfirmDialog)
                                if (CurrentLocalizationFilesProcessingCancelTokenSource.Token.IsCancellationRequested)
                                {
                                    ProcessingWasCancelled = true;
                                    CurrentLocalizationFilesProcessingCancelTokenSource.Token.ThrowIfCancellationRequested();
                                }

                                bool CanSaveFile = false;

                                if (File.Exists(OutputFile_DestinationFullPath) == false)
                                {
                                    CanSaveFile = true;
                                }
                                else if (CanOverwriteExistingFile(OutputFile_DestinationFullPath, FinalJsonFile))
                                {
                                    CanSaveFile = true;
                                }



                                if (CanSaveFile)
                                {
                                    File.WriteAllText(path: OutputFile_DestinationFullPath, contents: FinalJsonFile, encoding: OuputEncoding);

                                    ProcessedFilesCounter++;

                                    if (IsFromReference == false)
                                    {
                                        PrimaryExportedFiles.Add(LocalizationFile.Name);
                                    }

                                    LocalizationFilesProcessorWindowInstance.Dispatcher.Invoke(delegate ()
                                    {
                                        LocalizationFilesProcessorWindowInstance.ExportedFilesLog.Children.Add(new IntenseStareType1
                                        {
                                            Style = LocalizationFilesProcessorWindowInstance.ExportedFilesLog_ParentScrollViewer.Resources["LogFileName"] as Style,
                                            Text = (IsFromReference ? "[Reference] " : "") + RelativePathWithoutBackslashes,
                                            Tag = new
                                            {
                                                FileName = Path.GetFileName(OutputFile_DestinationFullPath),
                                                FullPath = OutputFile_DestinationFullPath
                                            }
                                        });
                                        LocalizationFilesProcessorWindowInstance.ExportedFilesLog_ParentScrollViewer.ScrollToBottom();
                                    });
                                }
                            }
                            catch (Exception Occurred)
                            {
                                if (Occurred is not OperationCanceledException)
                                {
                                    ErrorMessageWindow.ShowException(
                                        Occurred,
                                        $"This exception occurred while trying to process file <b>\"{RelativePathWithoutBackslashes}\"</b>{(IsFromReference ? " (From reference)" : "")}" +
                                        (Occurred is JsonReaderException ? "\n(This type of exception means that Json file contains syntax errors)" : ""),
                                        EnableLocalizationFilesProcessorCancelButton: true
                                    );
                                }
                            }
                        }


                        try
                        {
                            await Task.Run(delegate()
                            {
                                foreach (FileInfo LocalizationFile in TargetFiles)
                                {
                                    TotalFilesCounter++;

                                    ProcessFile(LocalizationFile, IsFromReference: false);
                                }

                                #region Missing files appending
                                if (Profile.ReferenceLocalization.MissingContentAppending.AppendMissingFiles)
                                {
                                    IEnumerable<FileInfo> TargetFiles_Missing = new DirectoryInfo(ReferenceDirectory)
                                        .GetFiles("*.json", SearchOption.AllDirectories)
                                        .Where(ReferenceLocalizationFile => ReferenceLocalizationFile.FullName.Cut(ReferenceDirectory).Contains(@".vs\") == false) // .vs\ folder ignore
                                        .Where(ReferenceLocalizationFile => MatchesWildcardPatterns(ReferenceLocalizationFile.FullName.Cut(ReferenceDirectory + "\\").Replace("\\", "/"), WhiteListOfMissingFilesToAppend)); // General whitelist

                                    foreach (FileInfo ReferenceLocalizationFile in TargetFiles_Missing)
                                    {
                                        string UnprefixedName = ReferenceLocalizationFile.Name.RemovePrefix(Profile.ReferenceLocalization.Prefix);

                                        if (PrimaryCheckedFiles.Contains(UnprefixedName) == false)
                                        {
                                            if (PrimaryExportedFiles.Contains(UnprefixedName) == false)
                                            {
                                                ProcessFile(ReferenceLocalizationFile, IsFromReference: true);
                                            }
                                        }
                                    }
                                }
                                #endregion
                            });
                        }
                        catch (OperationCanceledException)
                        {
                            IntenseStareType1 CancelledProcessingMessage = new();
                            CancelledProcessingMessage.InherintPropertiesFrom(@Languages.PresentedTextElements["[Localization Processor] * Cancelled processing message"], IncludeRichText: true);
                            
                            LocalizationFilesProcessorWindowInstance.ExportedFilesLog.Children.Add(CancelledProcessingMessage);
                            LocalizationFilesProcessorWindowInstance.ExportedFilesLog_ParentScrollViewer.ScrollToBottom();
                        }

                        if (ProcessingWasCancelled == false)
                        {
                            IntenseStareType1 FinishedProcessingMessage = new();
                            FinishedProcessingMessage.InherintPropertiesFrom(@Languages.PresentedTextElements["[Localization Processor] * Finished processing message"], IncludeRichText: true);

                            LocalizationFilesProcessorWindowInstance.ExportedFilesLog.Children.Add(FinishedProcessingMessage);
                            LocalizationFilesProcessorWindowInstance.ExportedFilesLog_ParentScrollViewer.ScrollToBottom();
                        }
                    }
                }
                catch (Exception Occurred)
                {
                    ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to process localizaion files (Fatal)");
                }

                LocalizationFilesProcessorWindowInstance.TaskBarProgress.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;

                LocalizationFilesProcessorWindowInstance.StartButton.Visibility = Visibility.Visible;
                LocalizationFilesProcessorWindowInstance.CancelButton.Visibility = Visibility.Collapsed;

                IsProcessing = false;

                await Task.Delay(2100);
                if (IsProcessing == false) LocalizationFilesProcessorWindowInstance.FilesCounter.Visibility = Visibility.Collapsed;
            }
        }
    }
}