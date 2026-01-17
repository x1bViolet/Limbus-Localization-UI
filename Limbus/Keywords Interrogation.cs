using LC_Localization_Task_Absolute.Json;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Abstract;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_SkillTag;
using static LC_Localization_Task_Absolute.Limbus_Integration.TMProEmitter;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    public static class KeywordsInterrogation
    {
        public partial class KeywordDescriptionInfoPopup : Grid
        {
            public static void AttachToInline(Inline Target)
            {
                // Default
                string Name = "Unknown";
                string Description = "Unknown";
                BitmapImage KeywordIcon = KeywordImages["Unknown"];

                string KeywordID = Target.Name;
                if (Keywords_BattleKeywords.ContainsKey(KeywordID))
                {
                    Name = Keywords_BattleKeywords[KeywordID].Name;
                    Description = Keywords_BattleKeywords[KeywordID].Description;
                    if (Keywords_BattleKeywords[KeywordID].Flavor != null)
                    {
                        Description += $"\n\n<flavor\uAAFF><size\uAAFF>{Keywords_BattleKeywords[KeywordID].Flavor}</size\uAAFF></flavor\uAAFF>";
                    }
                }
                if (KeywordImages.ContainsKey(KeywordID)) KeywordIcon = KeywordImages[KeywordID];

                ToolTip KeywordInfoPopup = new ToolTip()
                {
                    Margin = new Thickness(0, 13, 0, 0),
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    LayoutTransform = new ScaleTransform(0.89, 0.89),
                    IsHitTestVisible = false,
                    Content = new KeywordDescriptionInfoPopup(Name, Description, KeywordIcon)
                };
                Target.ToolTip = KeywordInfoPopup;
                ToolTipService.SetInitialShowDelay(Target, 1000);
            }


            public KeywordDescriptionInfoPopup(string Name, string Description, BitmapImage Icon)
            {
                Width = 422;
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Top;

                TMProEmitter KeywordName = new()
                {
                    DisableKeyworLinksCreation = true,
                    TextProcessingMode = EditorMode.Keywords,
                    FontType = LimbusFontTypes.Title,

                    MinWidth = 305,
                    MaxWidth = 345,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 22,
                    LineHeight = 24,
                };
                TMProEmitter KeywordDesc = new()
                {
                    DisableKeyworLinksCreation = true,
                    TextProcessingMode = EditorMode.Keywords,
                    FontType = LimbusFontTypes.Context,

                    Margin = new Thickness(18, 48, 25, 18),
                    FontSize = 20,
                    LineHeight = 25,
                };

                KeywordName.RichText = Name;
                KeywordDesc.RichText = Description;

                Children.Add(new Border()
                {
                    BorderThickness = new Thickness(0.8),
                    CornerRadius = new CornerRadius(0.55),
                    BorderBrush = new LinearGradientBrush()
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops =
                        {
                            new GradientStop() { Color = ToColor("#4B3F31") },
                            new GradientStop() { Color = ToColor("#00000000"), Offset = 0.6 }
                        }
                    },
                    Child = new Border()
                    {
                        Margin = new Thickness(1.5),
                        BorderThickness = new Thickness(0.8),
                        BorderBrush = ToSolidColorBrush("#4B3F31"),
                        Background = ToSolidColorBrush("#C6000000"),
                        Child = new Grid()
                        {
                            Children =
                            {
                                new StackPanel()
                                {
                                    Orientation = Orientation.Horizontal,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    MaxHeight = 55,
                                    Children =
                                    {
                                        new Image()
                                        {
                                            Source = Icon,
                                            VerticalAlignment = VerticalAlignment.Top,
                                            Width = 35,
                                            Margin = new Thickness(6, 6, 0, 0)
                                        },
                                        new Viewbox()
                                        {
                                            MaxWidth = 345,
                                            Height = 40,
                                            Stretch = Stretch.Uniform,
                                            HorizontalAlignment = HorizontalAlignment.Left,
                                            Margin = new Thickness(5, 6.6, 0, 0),
                                            Child = KeywordName
                                        }
                                    }
                                },
                                KeywordDesc
                            }
                        }
                    }
                });
            }
        }
        

        #region Keywords Multiple Meanings create/read
        public static Dictionary<string, List<string>> KeywordsMultipleMeaningsDictionary_ReadOrSavePlaceholder = [];
        public record KeywordsMultipleMeaningsDictionaryJson
        {
            public List<KeywordsMultipleMeanings> Info { get; set; }
        }
        public record KeywordsMultipleMeanings
        {
            [JsonProperty("Keyword ID")]
            public string KeywordID { get; set; }

            public List<string> Meanings { get; set; }
        }
        public static void ExportKeywordsMultipleMeaningsDictionary(string LocalizationWithKeywordsPath)
        {
            KeywordsMultipleMeaningsDictionaryJson Export = new() { Info = new List<KeywordsMultipleMeanings>() };
            
            int FoundCounter = 0;
            foreach (FileInfo LocalizeFile in new DirectoryInfo(LocalizationWithKeywordsPath)
                .GetFiles("*.json", SearchOption.AllDirectories)
                    .Where(LocalizeFile => LocalizeFile.Name.StartsWithOneOf(
                        "Passive",
                        "EGOgift",
                        "Bufs",
                        "BattleKeywords",
                        "Skills",
                        "PanicInfo",
                        "BuffAbilities"
                    )
                )
            ) {
                string TextToAnalyze = File.ReadAllText(LocalizeFile.FullName).Replace("\\\"", "\"");
                foreach (Match keywordMatch in LimbusPreviewFormatter.RemoteRegexPatterns.TMProKeyword.Matches(TextToAnalyze))
                {
                    string ID = keywordMatch.Groups["ID"].Value;

                    if (!ID.ContainsOneOf(LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.KeywordsIgnore))
                    {
                        string VariableName = keywordMatch.Groups["Name"].Value;

                        if (!KeywordsMultipleMeaningsDictionary_ReadOrSavePlaceholder.ContainsKey(ID)) KeywordsMultipleMeaningsDictionary_ReadOrSavePlaceholder[ID] = new List<string>();

                        if (!KeywordsMultipleMeaningsDictionary_ReadOrSavePlaceholder[ID].Contains(VariableName))
                        {
                            KeywordsMultipleMeaningsDictionary_ReadOrSavePlaceholder[ID].Add(VariableName);
                        }

                        FoundCounter++;
                    }
                }
            }

            foreach (KeyValuePair<string, List<string>> Item in KeywordsMultipleMeaningsDictionary_ReadOrSavePlaceholder)
            {
                Export.Info.Add(new KeywordsMultipleMeanings()
                {
                    KeywordID = Item.Key,
                    Meanings = Item.Value
                });
            }

            if (FoundCounter > 0)
            {
                Export.SerializeToFormattedFile_Regular("Keywords Multiple Meanings.json");
                MessageBox.Show($"Keywords multiple meanings from \"{LocalizationWithKeywordsPath}\" folder exported as \"Keywords Multiple Meanings.json\" to the program folder");
            }
        }
        public static void ReadKeywordsMultipleMeanings(string Filepath)
        {
            try
            {
                KeywordsMultipleMeaningsDictionaryJson Readed = new FileInfo(Filepath).Deserealize<KeywordsMultipleMeaningsDictionaryJson>();
                if (Readed != null && Readed.Info != null)
                {
                    KeywordsInterrogation.KeywordsMultipleMeaningsDictionary_ReadOrSavePlaceholder = new Dictionary<string, List<string>>();
                    foreach (KeywordsMultipleMeanings KeywordMeaningsInfo in Readed.Info)
                    {
                        if (!KeywordsMultipleMeaningsDictionary_ReadOrSavePlaceholder.ContainsKey(KeywordMeaningsInfo.KeywordID)) KeywordsMultipleMeaningsDictionary_ReadOrSavePlaceholder[KeywordMeaningsInfo.KeywordID] = new List<string>();
                        foreach (string KeywordMeaning in KeywordMeaningsInfo.Meanings)
                        {
                            KeywordsInterrogation.KeywordsMultipleMeaningsDictionary_ReadOrSavePlaceholder[KeywordMeaningsInfo.KeywordID].Add(KeywordMeaning);
                            KeywordsInterrogation.Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter[KeywordMeaning] = KeywordMeaningsInfo.KeywordID;
                        }
                    }
                }
            }
            catch (Exception ex) { rin(FormattedStackTrace(ex, "Keywords multiple meanings dictionary reading")); }
        }
        #endregion
        
        /// <summary>
        /// Contains matches of keyword names and their IDs in descending order of name length (For limbus preview formatter, only base keyword names)
        /// </summary>
        public static Dictionary<string, string> Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter { get; set; } = [];

        /// <summary>
        /// Extended version with multiple keyword meanings from Keywords multiple meanings dictionary (E.g. "Charge", "Charges" for same Charge keyword id)
        /// and also mixed with fallback keywords
        /// </summary>
        public static Dictionary<string, string> Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter { get; set; } = [];



        public readonly record struct KeywordDescriptor(string Name, string Description, string StringColor, string Flavor);

        #pragma Key is ID
        public static readonly Dictionary<string, KeywordDescriptor> Keywords_BattleKeywords = [];
        public static readonly Dictionary<string, KeywordDescriptor> Keywords_Bufs = [];

        public static readonly Dictionary<string /*[SkillTagID]*/, string /*<color=#hexcolor>Text</color>*/> SkillTags = [];

        public static readonly Dictionary<string, string /*#hexcolor*/> CollectedKeywordColors = [];
        public static readonly Dictionary<string, string /*#hexcolor*/> CollectedSkillTagColors = [];

        public static readonly Dictionary<string, BitmapImage> KeywordImages = [];
        public static readonly Dictionary<string, BitmapImage> EGOGiftInlineImages = [];


        public static void LoadInlineImages()
        {
            KeywordImages["Unknown"] = BitmapFromResource("UI/Unknown.png");

            FileInfo[] LoadSite = new DirectoryInfo(@"[⇲] Assets Directory\Limbus Images\Keywords").GetFiles("*.png", SearchOption.AllDirectories);
            foreach (FileInfo KeywordImage in LoadSite)
            {
                string TargetID = KeywordImage.Name.Replace(KeywordImage.Extension, "");
                KeywordImages[TargetID] = BitmapFromFile(KeywordImage.FullName);
            }
        }

        private static string TryAcquireLocalizeLine(this DirectoryInfo Source, string FilePath, string ID, string Fallback)
        {
            FileInfo[] FoundFiles = Source.GetFiles(FilePath, SearchOption.AllDirectories);
            if (FoundFiles.Length > 0)
            {
                try
                {
                    Dictionary<string, string> LocalizeInfo = FoundFiles[0].Deserealize<AbstractDataListFile>().dataList
                        .Select(AbstractObject => new KeyValuePair<string, string>((string)AbstractObject.id, (string)AbstractObject.content))
                            .ToDictionary();

                    return LocalizeInfo.ContainsKey(ID) ? LocalizeInfo[ID] : Fallback;
                }
                catch (Exception ex) { rin(FormattedStackTrace(ex, $"\"{ID}\" localize text acquiring")); return Fallback; } // If json file read error or something (what if 'id' is suddenly int)
            }
            else return Fallback;
        }

        public static void InitializeGlossaryFrom(string KeywordsDirectory, bool WriteOverFallback = false, string FilesPrefix = "", bool IgnoreUILabels = false)
        {
            if (!WriteOverFallback) // if (Fallback keywords loading)
            {
                ClearMany(
                    Keywords_Bufs,

                    SkillTags,
                    Keywords_BattleKeywords

                    //Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter,
                    //Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter

                    //CollectedKeywordColors,
                    //CollectedSkillTagColors
                );
            }


            DirectoryInfo MainSite = new DirectoryInfo(KeywordsDirectory);

            rin($"{(WriteOverFallback ? "" : "\n[Fallback] ")}Loading Keywords from \"{KeywordsDirectory}\"{(FilesPrefix != "" ? $" with files prefix \"{FilesPrefix}\"" : "")}");
            Dictionary<string, string> SkillTagColors = [];
            if (File.Exists(@"[⇲] Assets Directory\Color Dictionaries\SkillTag Colors.cd.txt"))
            {
                foreach (string Line in File.ReadAllLines(@"[⇲] Assets Directory\Color Dictionaries\SkillTag Colors.cd.txt").Where(Line => Line.Contains(" ¤ ")))
                {
                    string[] ColorPair = Line.Split(" ¤ ");
                    if (ColorPair.Count() == 2)
                    {
                        string SkillTagID = ColorPair[0].Trim();
                        string SkillTagColor = ColorPair[1].Trim();

                        SkillTagColors[SkillTagID] = SkillTagColor;
                    }
                }
            }

            Dictionary<string, string> KeywordColors = [];
            if (File.Exists(@"[⇲] Assets Directory\Color Dictionaries\Keyword Colors.cd.txt"))
            {
                foreach (string Line in File.ReadAllLines(@"[⇲] Assets Directory\Color Dictionaries\Keyword Colors.cd.txt"))
                {
                    string[] ColorPair = Line.Split(" ¤ ");
                    if (ColorPair.Length == 2)
                    {
                        string KeywordID = ColorPair[0].Trim();
                        string KeywordColor = ColorPair[1].Trim();
                        KeywordColors[KeywordID] = KeywordColor;
                    }
                }
            }


            if (!IgnoreUILabels)
            {
                MainControl.SkillAtkWeightSign .RichText = MainSite.TryAcquireLocalizeLine("*MainUIText_UPDATE_ON_0720.json", "mainui_target_num_label", Fallback: "Atk Weight");
                MainControl.SkillUptieLevelSign.RichText = MainSite.TryAcquireLocalizeLine("*Filter.json", "filter_awaken_state", Fallback: "Uptie Tier");
                MainControl.EGOGiftViewDescSign.RichText = MainSite.TryAcquireLocalizeLine("*MirrorDungeonUI_3.json", "mirror_dungoen_ego_gift_history_view_desc", Fallback: "View Desc.");
            }




            FileInfo[] SkillTag_Found = MainSite.GetFiles("*SkillTag.Json", SearchOption.AllDirectories);
            if (SkillTag_Found.Length > 0)
            {
                SkillTagsFile? SkillTagsJson = SkillTag_Found[0].Deserealize<SkillTagsFile>();

                if (SkillTagsJson != null && SkillTagsJson.dataList != null)
                {
                    foreach (SkillTag SkillTag in SkillTagsJson.dataList)
                    {
                        if (!string.IsNullOrEmpty(SkillTag.ID))
                        {
                            string DefinedColor = "#93f03f";
                            if (SkillTag.Color != null)
                            {
                                DefinedColor = SkillTag.Color;
                            }
                            else if (SkillTagColors.ContainsKey(SkillTag.ID))
                            {
                                DefinedColor = SkillTagColors[SkillTag.ID];
                            }
                            SkillTags[$"[{SkillTag.ID}]"] = $"<color={DefinedColor}>{SkillTag.Tag}</color>";

                            CollectedSkillTagColors[SkillTag.ID] = DefinedColor;
                        }
                    }
                }
            }

            FileInfo[] LoadSite_Bufs = MainSite.GetFiles(
                "*Bufs*.json",
                SearchOption.AllDirectories
            ).OrderBy(x => x.Name.Length).ToArray(); // Order by length starting from Bufs.json

            foreach (FileInfo KeywordFileInfo in LoadSite_Bufs)
            {
                try
                {
                    KeywordsFile? TargetSite = KeywordFileInfo.Deserealize<KeywordsFile>();

                    if (TargetSite.dataList != null && TargetSite.dataList.Count > 0)
                    {
                        foreach (Keyword KeywordItem in TargetSite.dataList)
                        {
                            if (!string.IsNullOrEmpty(KeywordItem.ID))
                            {
                                if (!KeywordItem.ID.ContainsOneOf(LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.KeywordsIgnore))
                                {
                                    string DefinedColor = "#9f6a3a";

                                    if (KeywordItem.Color != null)
                                    {
                                        DefinedColor = KeywordItem.Color;
                                    }
                                    else if (KeywordColors.ContainsKey(KeywordItem.ID))
                                    {
                                        DefinedColor = KeywordColors[KeywordItem.ID];
                                    }

                                    CollectedKeywordColors[KeywordItem.ID] = DefinedColor;

                                    Keywords_Bufs[KeywordItem.ID] = new KeywordDescriptor(KeywordItem.Name, null, DefinedColor, KeywordItem.PresentFlavorDescription);

                                    if (!KeywordItem.ID.EndsWithOneOf(["_Re", "Re", "Mirror"]))
                                    {
                                        //Fallback overwrite
                                        if (Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter.ContainsValue(KeywordItem.ID) & WriteOverFallback)
                                        {
                                            Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter =
                                                Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter.RemoveItemWithValue(KeywordItem.ID);
                                        }

                                        // Take first encountered and no more
                                        if (!Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter.ContainsKey(KeywordItem.Name))
                                        {
                                            Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter[KeywordItem.Name] = KeywordItem.ID;
                                        }

                                    
                                        Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter[KeywordItem.Name] = KeywordItem.ID;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        rin($"  [!] Null dataList or 0 items at the {KeywordFileInfo.Name} file");
                    }
                }
                catch (Exception ex)
                {
                    string Error = FormattedStackTrace(ex, $"Limbus json file reading: {KeywordFileInfo.Name}");
                    if (MainControl.IsLoaded) MessageBox.Show(Error.Trim());
                    if (!MainControl.IsLoaded) LoadErrors += "\n\n" + Error.Trim();
                    rin(Error);
                }
            }


            Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter =
                Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter
                    .OrderBy(obj => obj.Key.Length)
                        .ToDictionary(obj => obj.Key, obj => obj.Value)
                            .Reverse().ToDictionary();

            Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter =
                Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter
                    .OrderBy(obj => obj.Key.Length)
                        .ToDictionary(obj => obj.Key, obj => obj.Value)
                            .Reverse().ToDictionary();


            FileInfo[] LoadSite_BattleKeywords = MainSite.GetFiles(
                "*BattleKeywords*.json",
                SearchOption.AllDirectories
            );
            foreach (FileInfo KeywordFileInfo in LoadSite_BattleKeywords)
            {
                try
                {
                    KeywordsFile? TargetSite = KeywordFileInfo.Deserealize<KeywordsFile>();

                    if (TargetSite != null && TargetSite.dataList != null)
                    {
                        foreach (Keyword KeywordItem in TargetSite.dataList)
                        {
                            if (!string.IsNullOrEmpty(KeywordItem.ID) && KeywordItem.PresentMainDescription != null) // Empty desc allowed
                            {
                                Keywords_BattleKeywords[KeywordItem.ID] = new KeywordDescriptor(KeywordItem.Name, KeywordItem.PresentMainDescription, null, KeywordItem.PresentFlavorDescription);
                            }
                        }
                    }
                    else
                    {
                        rin($"  [!] Null dataList or 0 items at the {KeywordFileInfo.Name} file");
                    }
                }
                catch (Exception ex)
                {
                    string Error = FormattedStackTrace(ex, $"Limbus json file reading: {KeywordFileInfo.Name}");
                    if (MainControl.IsLoaded) MessageBox.Show(Error.Trim());
                    if (!MainControl.IsLoaded) LoadErrors += "\n\n" + Error.Trim();
                    rin(Error);
                }
            }

            if (WriteOverFallback) JsonTextEditor.RecompileEditorSyntax();
        }
    }
}
