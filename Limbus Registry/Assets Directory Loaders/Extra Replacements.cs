namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        // Replace context menu with specific one when Left Shift is pressed
        private void LimbusJsonTextEditor_ContextMenuOpening(object Sender, ContextMenuEventArgs Args)
        {
            LimbusJsonTextEditor.ContextMenu = Keyboard.IsKeyDown(Key.LeftShift) && ExtraReplacementsContextMenu.ContextMenu_ExtraReplacements.Items.Count > 0
                ? ExtraReplacementsContextMenu.ContextMenu_ExtraReplacements
                : ExtraReplacementsContextMenu.TextEditorContextMenu_Default;
        }
    }

    namespace LimbusRegistry.AssetsDirectoryLoaders
    {
        public static class ExtraReplacementsContextMenu
        {
            public static ContextMenu ContextMenu_ExtraReplacements => (MainWindowInstance.JsonTextEditor_And_RichTextViews.Resources["LimbusJsonTextEditor_ExtraReplacements"] as ContextMenu)!;
            public static ContextMenu TextEditorContextMenu_Default => (MainWindowInstance.JsonTextEditor_And_RichTextViews.Resources["LimbusJsonTextEditor_Conversions"] as ContextMenu)!;



            private static FileEventsNotifier ExtraReplacementsRgTxtFileWatcher { get; } = new()
            {
                GeneralHandler = (_, _, _) => ReadExtraReplacementsFile()
            };
            private static string CurrentFilePath = @"";
            public static void SetSourceFile(string FilePath)
            {
                CurrentFilePath = FilePath;

                if (File.Exists(FilePath))
                {
                    ExtraReplacementsRgTxtFileWatcher.WatchFile(CurrentFilePath);
                }
                else
                {
                    ExtraReplacementsRgTxtFileWatcher.Reset();
                }

                ReadExtraReplacementsFile();
            }


            public readonly record struct RegexReplaceOption(Regex? RegularExpression = null, string? Replacement = null, bool ParentIsUnclickableHeader = false, bool ParentIsSeparator = false);
            private static void ReadExtraReplacementsFile()
            {
                ContextMenu_ExtraReplacements.Items.Clear();

                if (File.Exists(CurrentFilePath))
                {
                    try
                    {
                        Dictionary<string, List<RegexReplaceOption>> Readed = [];

                        List<string> Lines = StreamReadLines(CurrentFilePath);

                        int LineIndex = 0; string? LatestOptionName = null;
                        int SeparatorCounter = 0;

                        foreach (string Line in Lines)
                        {
                            if (Line.StartsWith("{Regex Option} - "))
                            {
                                LatestOptionName = Line[17..];

                                if (Readed.ContainsKey(LatestOptionName) == false)
                                {
                                    Readed[LatestOptionName] = new List<RegexReplaceOption>();
                                }
                                else
                                {
                                    LatestOptionName = null;
                                }
                            }
                            else if (Line.StartsWith("{Unclickable Text} - "))
                            {
                                LatestOptionName = Line[21..];
                                Readed[LatestOptionName] = new List<RegexReplaceOption>() { new RegexReplaceOption(ParentIsUnclickableHeader: true) };
                            }
                            else if (Line.StartsWith("{Separator}"))
                            {
                                LatestOptionName = $"<Separator {SeparatorCounter}>";
                                SeparatorCounter++;
                                Readed[LatestOptionName] = new List<RegexReplaceOption>() { new RegexReplaceOption(ParentIsSeparator: true) };
                            }
                            else if (LatestOptionName is not null)
                            {
                                if (Line.StartsWith("* Pattern: ") && LineIndex < Lines.Count - 1 && Lines[LineIndex + 1].StartsWith("  Replace: "))
                                {
                                    string RepalceForPattern = Lines[LineIndex + 1][11..];

                                    // Unicode escapes
                                    RepalceForPattern = Regex.Replace(RepalceForPattern, @"\\u(?<UnicodeCharacterCode>[a-fA-F0-9]{4})", Match =>
                                    {
                                        int UnicodeCharacterCode = int.Parse(Match.Groups["UnicodeCharacterCode"].Value, System.Globalization.NumberStyles.HexNumber);
                                        return $"{(char)UnicodeCharacterCode}";
                                    });

                                    // Line breaks
                                    RepalceForPattern = RepalceForPattern.Replace("\\n", "\n");

                                    Readed[LatestOptionName].Add(new RegexReplaceOption(RegularExpression: new Regex(pattern: Line[11..]), Replacement: RepalceForPattern));
                                }
                            }

                            LineIndex++;
                        }

                        foreach (KeyValuePair<string, List<RegexReplaceOption>> ExtraRegexItem in Readed)
                        {
                            if (ExtraRegexItem.Value.Any(x => x.ParentIsSeparator))
                            {
                                ContextMenu_ExtraReplacements.Items.Add(new Separator() { UseLayoutRounding = true });
                            }
                            else
                            {
                                IntenseStareType1 CreatedHeader = new();
                                CreatedHeader.InherintPropertiesFrom(@Languages.PresentedTextElements["[Context Menu] [-] * Extra replacements item header"]);
                                CreatedHeader.RichText = ExtraRegexItem.Key;

                                if (ExtraRegexItem.Value.Any(x => x.ParentIsUnclickableHeader))
                                {
                                    ContextMenu_ExtraReplacements.Items.Add(new MenuItem() { Header = CreatedHeader, IsHitTestVisible = false });
                                }
                                else
                                {
                                    MenuItem_T1 CreatedMenuItem = new() { Header = CreatedHeader, DataContext = ExtraRegexItem.Value, Tag = ExtraRegexItem.Key };
                                    CreatedMenuItem.Click += MainWindowInstance.TextEditor_ContextMenuClick;

                                    ContextMenu_ExtraReplacements.Items.Add(CreatedMenuItem);
                                }
                            }
                        }
                    }
                    catch (Exception Occurred)
                    {
                        HandleError(Occurred);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(CurrentFilePath))
                {
                    HandleError(new FileNotFoundException("File not found"));
                }


                static void HandleError(Exception Occurred)
                {
                    ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to read Extra Replacements file \"<b>{CurrentFilePath}</b>\"");
                }
            }
        }
    }
}