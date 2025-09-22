using LC_Localization_Task_Absolute.Json;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_SkillTag;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    public abstract class KeywordsInterrogate
    {
        public record KeywordDescriptor
        {
            public string Name { get; set; }
            public string StringColor { get; set; }
        }

        #region Keywords Multiple Meanings create/read
        public static Dictionary<string, List<string>> KeywordsMultipleMeaningsDictionary = new Dictionary<string, List<string>>();
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
        public static void ExportKeywordsMultipleMeaningsDictionary(string LocalizationWithKeywordsPath, bool IgnoreRailAndMirrorKeywords = true)
        {
            KeywordsMultipleMeaningsDictionaryJson Export = new KeywordsMultipleMeaningsDictionaryJson();
            Export.Info = new List<KeywordsMultipleMeanings>();
            int FoundCounter = 0;
            foreach (FileInfo LocalizeFile in new DirectoryInfo(LocalizationWithKeywordsPath)
                .GetFiles("*.json", SearchOption.AllDirectories)
                    .Where(file => file.Name.StartsWithOneOf([
                        "Passive",
                        "EGOgift",
                        "Bufs",
                        "BattleKeywords",
                        "Skills",
                        "PanicInfo"
                    ])
                )
            ) {
                string TextToAnalyze = File.ReadAllText(LocalizeFile.FullName);
                foreach (Match keywordMatch in Regex.Matches(TextToAnalyze, @"<sprite name=\\""(?<ID>\w+)\\""><color=(?<Color>#[a-fA-F0-9]{6})><u><link=\\""\w+\\"">(?<Name>.*?)</link></u></color>"))
                {
                    string ID = keywordMatch.Groups["ID"].Value;
                    string VariativeName = keywordMatch.Groups["Name"].Value;

                    if (!KeywordsMultipleMeaningsDictionary.ContainsKey(ID)) KeywordsMultipleMeaningsDictionary[ID] = new List<string>();

                    if (KeywordsGlossary.ContainsKey(ID))
                    {
                        if (!KeywordsMultipleMeaningsDictionary[ID].Contains(KeywordsGlossary[ID].Name))
                        {
                            KeywordsMultipleMeaningsDictionary[ID].Add(KeywordsGlossary[ID].Name);
                        }
                    }

                    if (!KeywordsMultipleMeaningsDictionary[ID].Contains(VariativeName))
                    {
                        KeywordsMultipleMeaningsDictionary[ID].Add(VariativeName);
                    }

                    FoundCounter++;
                }
            }

            foreach (KeyValuePair<string, List<string>> Item in KeywordsMultipleMeaningsDictionary)
            {
                Export.Info.Add(new KeywordsMultipleMeanings()
                {
                    KeywordID = Item.Key,
                    Meanings = Item.Value
                });
            }

            if (FoundCounter > 0)
            {
                Export.SerializeFormatted("Keywords Multiple Meanings.json");
                MessageBox.Show($"Keywords multiple meanings from \"{LocalizationWithKeywordsPath}\" dir exported as \"Keywords Multiple Meanings.json\" at program folder");
            }
        }
        public static void ReadKeywordsMultipleMeanings(string Filepath)
        {
            if (File.Exists(Filepath))
            {
                KeywordsMultipleMeaningsDictionaryJson Readed = new FileInfo(Filepath).Deserealize<KeywordsMultipleMeaningsDictionaryJson>();
                if (Readed != null && Readed.Info != null)
                {
                    KeywordsMultipleMeaningsDictionary = new Dictionary<string, List<string>>();
                    foreach (KeywordsMultipleMeanings KeywordMeaningsInfo in Readed.Info)
                    {
                        if (!KeywordsMultipleMeaningsDictionary.ContainsKey(KeywordMeaningsInfo.KeywordID)) KeywordsMultipleMeaningsDictionary[KeywordMeaningsInfo.KeywordID] = new List<string>();
                        foreach (string KeywordMeaning in KeywordMeaningsInfo.Meanings)
                        {
                            KeywordsMultipleMeaningsDictionary[KeywordMeaningsInfo.KeywordID].Add(KeywordMeaning);

                            Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter[KeywordMeaning] = KeywordMeaningsInfo.KeywordID;
                        }
                    }
                }
            }
        }
        #endregion

        sealed public partial class KeywordDescriptionInfoPopup : Grid
        {
            public static void AttachToInline(Inline Target)
            {
                string Name = "Unknown";
                string Description = "Unknown";
                BitmapImage KeywordIcon = KeywordImages["Unknown"];

                string KeywordID = Target.Name;
                if (BattleKeywordsDescriptions.ContainsKey(KeywordID))
                {
                    Name = BattleKeywordsDescriptions[KeywordID].Item1;
                    Description = BattleKeywordsDescriptions[KeywordID].Item2;
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
                Width = 418;
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Top;
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
                            new GradientStop() { Color = ToColorBrush("#FF4B3F31") },
                            new GradientStop() { Color = ToColorBrush("#00000000"), Offset = 0.6 }
                        }
                    },
                    Child = new Border()
                    {
                        Margin = new Thickness(1.5),
                        BorderThickness = new Thickness(0.8),
                        BorderBrush = ToSolidColorBrush("#FF4B3F31"),
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
                                            Child = new TMProEmitter()
                                            {
                                                DisableKeyworLinksCreation = true,
                                                LimbusPreviewFormattingMode = "Keywords",

                                                MinWidth = 305,
                                                MaxWidth = 345,
                                                TextWrapping = TextWrapping.Wrap,
                                                VerticalAlignment = VerticalAlignment.Center,
                                                FontSize = 22,
                                                Foreground = ToSolidColorBrush("#EBCAA2"),
                                                LineHeight = 24,
                                            }
                                            .SetRichTextWithReturn(Name)
                                            .SetBindingWithReturn(
                                                TMProEmitter.FontFamilyProperty,
                                                "FontFamily",
                                                MainControl.NavigationPanel_ObjectName_Display
                                            ).SetBindingWithReturn(
                                                TMProEmitter.FontWeightProperty,
                                                "FontWeight",
                                                MainControl.NavigationPanel_ObjectName_Display
                                            )
                                        }
                                    }
                                },
                                new TMProEmitter()
                                {
                                    DisableKeyworLinksCreation = true,

                                    Margin = new Thickness(18, 48, 25, 18),
                                    LimbusPreviewFormattingMode = "Keywords",
                                    Foreground = ToSolidColorBrush("#EBCAA2"),
                                    FontSize = 20,
                                    LineHeight = 25,
                                }.SetRichTextWithReturn(
                                    Description
                                ).SetBindingWithReturn(
                                    TMProEmitter.FontFamilyProperty,
                                    "FontFamily",
                                    MainControl.Special_PreviewLayout_Keywords_BattleKeywords_Desc
                                ).SetBindingWithReturn(
                                    TMProEmitter.FontWeightProperty,
                                    "FontWeight",
                                    MainControl.Special_PreviewLayout_Keywords_BattleKeywords_Desc
                                )
                            }
                        }
                    }
                });
            }
        }
       
        /// <summary>
        /// Contains matches of keyword names and their IDs in descending order of name length (For limbus preview formatter, only base keyword names)
        /// </summary>
        public static Dictionary<string, string> Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter = [];
        
        /// <summary>
        /// Extended version with other keyword meanings (E.g. "Charge", "Charges" for same Charge keyword id)
        /// </summary>
        public static Dictionary<string, string> Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter = [];


        public static Dictionary<string, string> Keywords_IDName = [];
        public static List<string> KnownID = [];

        public static Dictionary<string, Tuple<string, string>> BattleKeywordsDescriptions = []; // Tuple<Name, Desc>
        public static Dictionary<string, string> SkillTags = [];
        
        public static Dictionary<string, string> CollectedKeywordColors = [];
        public static Dictionary<string, string> CollectedSkillTagColors = [];
        
        public static Dictionary<string, KeywordDescriptor> KeywordsGlossary = [];
        public static Dictionary<string, BitmapImage> KeywordImages = [];
        public static Dictionary<string, BitmapImage> EGOGiftInlineImages = [];

        public static void LoadInlineImages()
        {
            KeywordImages["Unknown"] = BitmapFromResource("Default/Images/Unknown.png");

            FileInfo[] LoadSite = new DirectoryInfo(@"[⇲] Assets Directory\[⇲] Limbus Images\Keywords").GetFiles("*.png", SearchOption.AllDirectories);

            foreach (FileInfo KeywordImage in LoadSite)
            {
                string TargetID = KeywordImage.Name.Replace(KeywordImage.Extension, "");
                KeywordImages[TargetID] = BitmapFromFile(KeywordImage.FullName);
            }
        }

        public static void InitializeGlossaryFrom(string KeywordsDirectory, bool WriteOverFallback = false, string FilesPrefix = "")
        {
            if (!WriteOverFallback) // if (Fallback keywords loading)
            {
                ClearMany(
                    KeywordsGlossary,

                    SkillTags,
                    BattleKeywordsDescriptions,

                    KnownID,
                    Keywords_IDName,

                    Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter,
                    Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter,

                    CollectedKeywordColors,
                    CollectedSkillTagColors
                );
            }


            if (Directory.Exists(KeywordsDirectory))
            {
                DirectoryInfo MainSite = new DirectoryInfo(KeywordsDirectory);

                rin($"\n {(WriteOverFallback ? " " : "[Fallback] ")}Loading Keywords from \"{KeywordsDirectory}\"{(!FilesPrefix.Equals("") ? $" with files prefix \"{FilesPrefix}\"" : "")}");
                Dictionary<string, string> SkillTagColors = new Dictionary<string, string>();
                if (File.Exists(@"[⇲] Assets Directory\[+] Keywords\SkillTag Colors.T[-]"))
                {
                    foreach (string Line in File.ReadAllLines(@"[⇲] Assets Directory\[+] Keywords\SkillTag Colors.T[-]").Where(Line => Line.Contains(" ¤ ")))
                    {
                        string[] ColorPair = Line.Split(" ¤ ");
                        if (ColorPair.Count() == 2)
                        {
                            string SkillTagID = ColorPair[0].Trim();
                            string SkillTagColor = ColorPair[1].Trim();

                            SkillTagColors[SkillTagID] = SkillTagColor;
                            CollectedSkillTagColors[SkillTagID] = SkillTagColor;
                        }
                    }
                }

                // File with skills 'Atk Weight' label
                FileInfo[] AtkWeightLabelFile = MainSite.GetFiles("*MainUIText_UPDATE_ON_0720.json", SearchOption.AllDirectories);
                if (AtkWeightLabelFile.Length > 0)
                {
                    string JsonText = File.ReadAllText(AtkWeightLabelFile[0].FullName);
                    string FoundText = Regex.Match(JsonText, @"""id"": ""mainui_target_num_label"",(\r)?\n.*?""content"": ""(?<AtkWeightText>.*?)""", RegexOptions.Singleline).Groups["AtkWeightText"].Value;
                    if (!FoundText.Equals("")) MainControl.AtkWeightText_sub.Text = FoundText.Replace("\\n", "\n").Replace("\\r", "\r").Trim();
                }

                // File with 'View Desc.' label
                FileInfo[] ViewDescLabelFile = MainSite.GetFiles("*MirrorDungeonUI_3.json", SearchOption.AllDirectories);
                if (ViewDescLabelFile.Length > 0)
                {
                    string JsonText = File.ReadAllText(ViewDescLabelFile[0].FullName);
                    string FoundText = Regex.Match(JsonText, @"""id"": ""mirror_dungoen_ego_gift_history_view_desc"",(\r)?\n.*?""content"": ""(?<ViewDescText>.*?)""", RegexOptions.Singleline).Groups["ViewDescText"].Value;
                    if (!FoundText.Equals("")) MainControl.STE_EGOGifts_LivePreview_ViewDescButtons.Text = FoundText.Replace("\\n", "\n").Replace("\\r", "\r").Trim();
                }

                FileInfo[] SkillTag_Found = MainSite.GetFiles("*SkillTag.Json", SearchOption.AllDirectories);
                if (SkillTag_Found.Length > 0)
                {
                    BaseTypes.Type_SkillTag.SkillTags SkillTagsJson = SkillTag_Found[0].Deserealize<SkillTags>();

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
                            }
                        }
                    }
                }

                Dictionary<string, string> KeywordColors = [];
                if (File.Exists(@"[⇲] Assets Directory\[+] Keywords\Keyword Colors.T[-]"))
                {
                    foreach (string Line in File.ReadAllLines(@"[⇲] Assets Directory\[+] Keywords\Keyword Colors.T[-]"))
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

                FileInfo[] LoadSite_Bufs = MainSite.GetFiles(
                    "*Bufs*.json",
                    SearchOption.AllDirectories
                ).OrderBy(x => x.Name.Length).ToArray(); // Order by length starting from Bufs.json

                foreach (FileInfo KeywordFileInfo in LoadSite_Bufs)
                {
                    Keywords TargetSite = KeywordFileInfo.Deserealize<Keywords>();

                    if (TargetSite.dataList != null && TargetSite.dataList.Count > 0)
                    {
                        foreach (Keyword KeywordItem in TargetSite.dataList)
                        {
                            if (!string.IsNullOrEmpty(KeywordItem.ID))
                            {
                                if (!KeywordItem.ID.ContainsOneOf(DeltaConfig.PreviewSettings.CustomLanguageProperties.KeywordsIgnore))
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

                                    KeywordsGlossary[KeywordItem.ID] = new KeywordDescriptor
                                    {
                                        Name = KeywordItem.Name,
                                        StringColor = DefinedColor
                                    };

                                    CollectedKeywordColors[KeywordItem.ID] = DefinedColor;

                                    Keywords_IDName[KeywordItem.ID] = KeywordItem.Name;
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

                                    KnownID.Add(KeywordItem.ID);
                                }
                            }
                        }
                    }
                    else
                    {
                        rin($"  [!] Null dataList or 0 items at the {KeywordFileInfo.Name} file");
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
                    Keywords TargetSite = KeywordFileInfo.Deserealize<Keywords>();

                    if (TargetSite.dataList != null && TargetSite.dataList.Count > 0)
                    {
                        foreach (Keyword KeywordItem in TargetSite.dataList)
                        {
                            if (!string.IsNullOrEmpty(KeywordItem.ID) && KeywordItem.Description != null) // Empty desc allowed
                            {
                                BattleKeywordsDescriptions[KeywordItem.ID] = new Tuple<string, string>(KeywordItem.Name, KeywordItem.Description);
                            }
                        }
                    }
                    else
                    {
                        rin($"  [!] Null dataList or 0 items at the {KeywordFileInfo.Name} file");
                    }
                }

                if (WriteOverFallback) SyntaxedTextEditor.RecompileEditorSyntax();
            }
            else
            {
                //rin($"\n[!] Keywords Directory \"{KeywordsDirectory}\" not found");
            }
        }
    }
}
