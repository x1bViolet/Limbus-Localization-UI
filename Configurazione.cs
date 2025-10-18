using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute
{
    public static class ConfigRegexSaver
    {
        // Because serealizing current config with all default values causes a lot of mess string values that hard to remove
        public static void ChangeJsonConfigViaRegex(string PropertyName, dynamic NewValue, bool IsInsideCurrentCustomLangProperties = false)
        {
            if (SettingsWindow.SettingsControl.ChangeConfigOnOptionToggle)
            {
                string CurrentConfigurationJsonContent = File.ReadAllText(@"[⇲] Assets Directory\Configurazione^.json");
                string OldConfig = CurrentConfigurationJsonContent;

                #region Change
                string MatchAppend = "";
                if (IsInsideCurrentCustomLangProperties)
                {
                    /*lang=regex*/
                    MatchAppend = @"(?<PatternInsideThisProperty>""Custom Language Associative Settings"": {(.*?)""Name"": ""<Current \0 Name>"",(.*?))"
                        .Replace(
                            @"<Current \0 Name>",
                            Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected.ToEscapeRegexString()
                        );
                }

                string ValueTypePattern = NewValue.GetType().ToString() switch
                {
                    /*lang=regex*/ "System.Double"  => @"(\d+)(\.(\d+))?",
                    /*lang=regex*/ "System.Boolean" => @"(true|false)",
                    /*lang=regex*/ "System.String"  => @"""(.*?)""",
                    /*lang=regex*/ _ => @"""(.*?)"""
                };

                CurrentConfigurationJsonContent = Regex.Replace(
                    input:     CurrentConfigurationJsonContent,
                    pattern:   @$"{MatchAppend}""{PropertyName.ToEscapeRegexString()}"": {ValueTypePattern}(?<Afterward>(,)?(\r)?\n)",
                    evaluator: Match => {

                        string ValueReplacementString = ValueTypePattern switch
                        {
                            /*lang=regex*/ @"(\d+)(\.(\d+))?" => $"{NewValue.ToString().Replace(",", ".")}",
                            /*lang=regex*/ @"(true|false)"    => $"{NewValue.ToString().ToLower()}",
                            /*lang=regex*/ @"""(.*?)"""       => $"\"{NewValue}\"",
                            /*lang=regex*/ _                  => $"\"{NewValue}\"",
                        };

                        return @$"{Match.Groups["PatternInsideThisProperty"].Value}""{PropertyName}"": {ValueReplacementString}{Match.Groups["Afterward"].Value}";
                    
                }, options: MatchAppend.Equals("") ? RegexOptions.None : RegexOptions.Singleline);
                #endregion

                if (!OldConfig.Equals(CurrentConfigurationJsonContent))
                {
                    File.WriteAllText(@"[⇲] Assets Directory\Configurazione^.json", CurrentConfigurationJsonContent);
                }
            }
        }
    }

    public abstract class Configurazione
    {
        #region loads
        public static ConfigDelta DeltaConfig = new ConfigDelta();

        public static Regex ShorthandsPattern = new Regex("NOTHING THERE");

        public static ShorthandInsertionProperty ShorthandsInsertionShape = new ShorthandInsertionProperty()
        {
            InsertionShape = "[<KeywordID>]",
            InsertionShape_Color = "",
        };

        public static CustomLanguageAssociativePropertyMain SelectedAssociativePropery_Shared = null;

        public static string LoadErrors = "";

        public static bool SettingsLoadingEvent = false;


        // Per-session, no config
        public static bool Spec_EnableKeywordIDSprite = true;
        public static bool Spec_EnableKeywordIDUnderline = true;

        public static void PullLoad()
        {
            Mode_EGOGifts.OrganizedData.UpdateDisplayInfo();

            Mode_Skills.LoadDisplayInfo();
            SkillsDisplayInfo.ReadSkillConstructors();

            KeywordsInterrogate.LoadInlineImages();

            if (File.Exists(@"[⇲] Assets Directory\Configurazione^.json"))
            {
                try
                {
                    LoadErrors = "";

                    SettingsLoadingEvent = true;
                    rin($"\n\n\n[ Settings load pull initialized ]\n");

                    Configurazione.DeltaConfig = JsonConvert.DeserializeObject<Configurazione.ConfigDelta>(File.ReadAllText(@"[⇲] Assets Directory\Configurazione^.json"));
                    rin($" Configuration file readed");

                    SettingsWindow.UpdateSettingsMenu_Regular();

                    ToggleLimbusPreviewVisibility();

                    if (Directory.Exists(DeltaConfig.PreviewSettings.CustomLanguageProperties.KeywordsFallback.FallbackKeywordsDirectory))
                    {
                        KeywordsInterrogate.InitializeGlossaryFrom(
                            KeywordsDirectory: DeltaConfig.PreviewSettings.CustomLanguageProperties.KeywordsFallback.FallbackKeywordsDirectory
                        );
                    }
                    else
                    {
                        LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.FallbackKeywordsNotFound.Extern(DeltaConfig.PreviewSettings.CustomLanguageProperties.KeywordsFallback.FallbackKeywordsDirectory);
                    }

                    string SelectedAssociativePropertyName = DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected;
                    rin($"\n Custom language properties: {SelectedAssociativePropertyName}");


                    List<CustomLanguageAssociativePropertyMain> SelectedAssociativePropery_Found = DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.List
                        .Where(x => x.PropertyName.Equals(SelectedAssociativePropertyName)).ToList();

                    if (SelectedAssociativePropery_Found.Count() > 0)
                    {
                        SelectedAssociativePropery_Shared = SelectedAssociativePropery_Found[0];

                        UpdateCustomLanguagePart(SelectedAssociativePropery_Shared);

                        SettingsWindow.UpdateSettingsMenu_CustomLang();
                    }
                    else
                    {
                        LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.CustomLanguagePropertyNotFound.Extern(SelectedAssociativePropertyName);
                    }

                    if (Directory.Exists(DeltaConfig.PreviewSettings.CustomLanguageProperties.AdditionalKeywordsDirectory))
                    {
                        KeywordsInterrogate.InitializeGlossaryFrom
                        (
                            KeywordsDirectory: DeltaConfig.PreviewSettings.CustomLanguageProperties.AdditionalKeywordsDirectory,
                            WriteOverFallback: true
                        );
                    }

                    try {
                        ᐁ_Interface_Localization_Loader.ModifyUI(DeltaConfig.Internal.UILanguage);
                        rin($" UI Language loaded from \"{DeltaConfig.Internal.UILanguage}\"");
                    } catch { }

                    try {
                        ᐁ_Interface_Themes_Loader.ModifyUI(DeltaConfig.Internal.UITheme);
                        rin($" UI Theme loaded from \"{DeltaConfig.Internal.UITheme}\"");
                    } catch { }

                    LimbusPreviewFormatter.UpdateLast();

                    SettingsLoadingEvent = false;
                }
                catch (Exception ex)
                {
                    rin(ex.ToString());
                    SettingsLoadingEvent = false;
                }
            }
        }

        public static void UpdateCustomLanguagePart(CustomLanguageAssociativePropertyMain SelectedAssociativePropery)
        {
            if (Directory.Exists(SelectedAssociativePropery.Properties.KeywordsDirectory))
            {
                KeywordsInterrogate.InitializeGlossaryFrom
                (
                    KeywordsDirectory: SelectedAssociativePropery.Properties.KeywordsDirectory,
                    WriteOverFallback: true
                );
            }
            else
            {
                LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.KeywordsDirNotFound.Extern(SelectedAssociativePropery.Properties.KeywordsDirectory);
            }

            if (!SelectedAssociativePropery.Properties.KeywordsMultipleMeaningsDictionary.Equals(""))
            {
                if (File.Exists(SelectedAssociativePropery.Properties.KeywordsMultipleMeaningsDictionary))
                {
                    KeywordsInterrogate.ReadKeywordsMultipleMeanings(SelectedAssociativePropery.Properties.KeywordsMultipleMeaningsDictionary);
                }
                else
                {
                    LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.MultipleKeywordsDictionaryMissing.Extern(SelectedAssociativePropery.Properties.KeywordsDirectory);
                }
            }

            Pocket_Watch_ː_Type_L.@Generic.SpritesVerticalOffset = SelectedAssociativePropery_Shared.Properties.KeywordsSpriteVerticalOffset;
            Pocket_Watch_ː_Type_L.@Generic.SpritesHorizontalOffset = SelectedAssociativePropery_Shared.Properties.KeywordsSpriteHorizontalOffset;

            MainControl.SkillNamesReplication_SkillName_ViewBox.SetTopMargin(SelectedAssociativePropery_Shared.Properties.SkillNamesVerticalOffset + 2);


            LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection = SelectedAssociativePropery.Properties.Keywords_AutodetectionRegex;
            rin($"  Keywords Autodetection Regex Pattern: {LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection}");
            Configurazione.ShorthandsPattern = new Regex(SelectedAssociativePropery.Properties.Keywords_ShorthandsRegex, RegexOptions.Singleline);

            rin($"  Keywords Shorthands Regex Pattern: {Configurazione.ShorthandsPattern}");


            if (SelectedAssociativePropery.Properties.Keywords_ShorthandsContextMenuInsertionShape != null)
            {
                ShorthandsInsertionShape.InsertionShape = SelectedAssociativePropery.Properties.Keywords_ShorthandsContextMenuInsertionShape;
            }
            if (SelectedAssociativePropery.Properties.Keywords_ShorthandsContextMenuInsertionShape_HexColor != null)
            {
                ShorthandsInsertionShape.InsertionShape_Color = SelectedAssociativePropery.Properties.Keywords_ShorthandsContextMenuInsertionShape_HexColor;
            }

            rin($"   Loading fonts:");
            UpdatePreviewLayoutsFont(SelectedAssociativePropery.Properties);



            //                                      if only starting (Somehow just cant show messagebox at startup fom public MainWindow() )
            // Then MainWindow.Window_Loaded() will be triggered with this
            if (!LoadErrors.Equals("") & Configurazione.DeltaConfig.Internal.ShowLoadWarnings & MainControl.IsLoaded)
            {
                ShowLoadWarningsWindow();
            }
        }

        public static void ToggleLimbusPreviewVisibility()
        {
            if (DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HidePreview)
            {
                MainControl.LimbusPreviewRow.MaxHeight = 0;
            }
            else
            {
                MainControl.LimbusPreviewRow.MaxHeight = double.PositiveInfinity;
            }
        }

        public static void UpdatePreviewLayoutsFont(CustomLanguageAssociativePropertyValues Properties = null)
        {
            if (Properties != null)
            {
                foreach (TMProEmitter PreviewLayoutItem in PreviewLayoutsList)
                {
                    if (MainWindow.PrimaryRegisteredFontSizes.ContainsKey(PreviewLayoutItem))
                    {
                        PreviewLayoutItem.FontSize = MainWindow.PrimaryRegisteredFontSizes[PreviewLayoutItem];
                    }
                }

                if (File.Exists(Properties.ContextFont))
                {
                    rin($"    - Context font file: \"{Properties.ContextFont}\"");

                    FontFamily ContextFontFamily = FileToFontFamily(Properties.ContextFont, Properties.ContextFont_OverrideReadName, WriteInfo: true);
                    
                    foreach (TMProEmitter PreviewLayoutItem in PreviewLayoutsList)
                    {
                        try
                        {
                            PreviewLayoutItem.FontFamily = ContextFontFamily;
                            PreviewLayoutItem.FontSize  *= Properties.ContextFont_FontSizeMultipler;
                            PreviewLayoutItem.FontWeight = WeightFrom(Properties.ContextFont_FontWeight);
                        }
                        catch { }
                    }
                    rin($"      Applied context font");
                }
                else
                {
                    LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.ContextFontMissing.Extern(Properties.ContextFont);
                    rin($"    - [!] Context font file NOT FOUND (\"{Properties.ContextFont}\")");
                }


                if (File.Exists(Properties.TitleFont))
                {
                    rin($"    - Title font file: \"{Properties.TitleFont}\"");
                    FontFamily TitleFontFamily = FileToFontFamily(Properties.TitleFont, Properties.TitleFont_OverrideReadName, WriteInfo: true);

                    MainControl.NavigationPanel_ObjectName_Display.FontFamily = TitleFontFamily;
                    rin($"      Applied title font");
                    MainControl.NavigationPanel_ObjectName_Display.FontWeight = WeightFrom(Properties.TitleFont_FontWeight);

                    // Reset
                    MainControl.SkillNamesReplication_SkillName_Text.FontSize = 23;
                    MainControl.NavigationPanel_ObjectName_Display.FontSize = 25;
                    MainControl.EGOGiftName_PreviewLayout.FontSize = 29;
                    MainControl.STE_EGOGifts_LivePreview_ViewDescButtons.FontSize = 20;
                    MainControl.PreviewLayout_Keywords_Bufs_Name.FontSize = 23;
                    MainControl.PreviewLayout_Keywords_BattleKeywords_Name.FontSize = 20;

                    // Set
                    MainControl.SkillNamesReplication_SkillName_Text.FontSize *= Properties.TitleFont_FontSizeMultipler;
                    MainControl.NavigationPanel_ObjectName_Display.FontSize *= Properties.TitleFont_FontSizeMultipler;
                    MainControl.EGOGiftName_PreviewLayout.FontSize *= Properties.TitleFont_FontSizeMultipler;
                    MainControl.STE_EGOGifts_LivePreview_ViewDescButtons.FontSize *= Properties.TitleFont_FontSizeMultipler;
                    MainControl.PreviewLayout_Keywords_Bufs_Name.FontSize *= Properties.TitleFont_FontSizeMultipler;
                    MainControl.PreviewLayout_Keywords_BattleKeywords_Name.FontSize *= Properties.TitleFont_FontSizeMultipler;

                }
                else
                {
                    LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.TitleFontMissing.Extern(Properties.TitleFont);
                    rin($"    - [!] Title font file NOT FOUND (\"{Properties.TitleFont}\")");
                }


                if (File.Exists(@"[⇲] Assets Directory\[⇲] Limbus Images\UI\BattleKeywords Background.png"))
                {
                    MainControl.PreviewLayoutGrid_Keywords_Sub_BattleKeywords_BackgroundImage.Source = BitmapFromFile(@"[⇲] Assets Directory\[⇲] Limbus Images\UI\BattleKeywords Background.png");
                }

                SettingsWindow.SettingsControl.InputSkillsPanelWidth.Text = $"{Configurazione.DeltaConfig.ScanParameters.AreaWidth}";
                SettingsWindow.SettingsControl.InputScansScaleFactor.Text = $"{Configurazione.DeltaConfig.ScanParameters.ScaleFactor}";
                SettingsWindow.SettingsControl.InputSkillsScanBackgroundColor.Text = $"{Configurazione.DeltaConfig.ScanParameters.BackgroundColor}";
                MainControl.SkillReplica.Visibility = Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSkillNamesReplication ? Visible : Collapsed;
            }
        }
        #endregion


        public static void ShowLoadWarningsWindow()
        {
            MessageBox.Show(
                Configurazione.LoadErrors + ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.WarningsDisablingNotice,

                ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.WarningsWindowTitle.Extern(Configurazione.SelectedAssociativePropery_Shared.PropertyName),

                MessageBoxButton.OK,

                MessageBoxImage.Information);
        }

        public record ShorthandInsertionProperty
        {
            [JsonProperty("(Context Menu) Insertion Shape")]
            public string InsertionShape { get; set; } = "[<KeywordID>:`<KeywordName>`]<KeywordColor>";

            [JsonProperty("(Context Menu) Insertion Shape (Color)")]
            public string InsertionShape_Color { get; set; } = "<HexColor>";
        }

        public record ConfigDelta
        {
            [JsonProperty("Internal")]
            public Internal Internal { get; set; } = new Internal();

            [JsonProperty("Preview Settings")]
            public PreviewSettings PreviewSettings { get; set; } = new PreviewSettings();

            [JsonProperty("Scan Parameters")]
            public ScanParameters ScanParameters { get; set; } = new ScanParameters();

            [JsonProperty("Technical Actions")]
            public TechnicalActions TechnicalActions { get; set; } = new TechnicalActions();
        }
        public record Internal
        {
            [JsonProperty("UI Language")]
            public string UILanguage { get; set; } = "";

            [JsonProperty("UI Theme")]
            public string UITheme { get; set; } = "";

            [JsonProperty("Topmost Window")]
            public bool IsAlwaysOnTop { get; set; } = true;

            [JsonProperty("Show Load Warnings")]
            public bool ShowLoadWarnings { get; set; } = true;

            [JsonProperty("Enable manual Json files managing")]
            public bool EnablemanualJsonFilesManaging { get; set; } = false;

            [OnDeserialized]
            private void OnDeserialized(StreamingContext context)
            {
                MainControl.Topmost = IsAlwaysOnTop;
            }
        }
        public record PreviewSettings
        {
            [JsonProperty("Base")]
            public PreviewSettingsBaseSettings PreviewSettingsBaseSettings { get; set; } = new PreviewSettingsBaseSettings();

            [JsonProperty("Custom Language Properties")]
            public CustomLanguageProperties CustomLanguageProperties { get; set; } = new CustomLanguageProperties();
        }
        public record PreviewSettingsBaseSettings
        {
            [JsonProperty("Enable Syntax Highlight")]
            public bool EnableSyntaxHighlight { get; set; } = true;

            [JsonProperty("Hide Limbus Text Preview")]
            public bool HidePreview { get; set; } = false;


            [JsonProperty("Preview Update Delay (Seconds)")]
            public double PreviewUpdateDelay { get; set; } = 0.00;


            [JsonProperty("Highlight <style>")]
            public bool HighlightStyle { get; set; } = true;


            [JsonProperty("Highlight Skill Descs on right click")]
            public bool HighlightSkillDescsOnRightClick { get; set; } = true;

            [JsonProperty("Highlight Skill Descs on manual switch")]
            public bool HighlightSkillDescsOnManualSwitch { get; set; } = false;


            [JsonProperty("Enable Skill Names Replication")]
            public bool EnableSkillNamesReplication { get; set; } = true;

            [JsonProperty("Enable Keyword Tooltips")]
            public bool EnableKeywordTooltips { get; set; } = false;
        }
        public record CustomLanguageProperties
        {
            [JsonProperty("Keywords Ignore")]
            public string[] KeywordsIgnore { get; set; } = [];

            [JsonProperty("Keywords Fallback")]
            public FallbackKeywords KeywordsFallback { get; set; } = new FallbackKeywords();

            [JsonProperty("Custom Language Associative Settings")]
            public CustomLanguageAssociativeSettings AssociativeSettings { get; set; }

            [JsonProperty("Additional Keywords Directory")]
            public string AdditionalKeywordsDirectory { get; set; } = "";
        }
        public record FallbackKeywords
        {
            [JsonProperty("Directory")]
            public string FallbackKeywordsDirectory { get; set; } = "";
        }
        public record CustomLanguageAssociativeSettings
        {
            [JsonProperty("Associative Properties Selected")]
            public string Selected { get; set; } = "";

            [JsonProperty("Associative Properties List")]
            public List<CustomLanguageAssociativePropertyMain> List { get; set; } = new List<CustomLanguageAssociativePropertyMain>();
        }
        public record CustomLanguageAssociativePropertyMain
        {
            [JsonProperty("Name")]
            public string PropertyName { get; set; } = "<none>";

            [JsonProperty("Hide in list")]
            public bool HideInList { get; set; } = false;

            [JsonProperty("Properties")]
            public CustomLanguageAssociativePropertyValues Properties { get; set; } = new CustomLanguageAssociativePropertyValues();
        }
        public record CustomLanguageAssociativePropertyValues
        {
            [JsonProperty("Keywords Directory")]
            public string KeywordsDirectory { get; set; } = "";


            
            [JsonProperty("Keywords Autodetection Regex Pattern")]
            public string Keywords_AutodetectionRegex { get; set; } = /* lang=regex */ @"(KeywordNameWillBeHere)(?![\p{L}\[\])\-_<'"":\+])";

            [JsonProperty("Keywords Shorthands Regex Pattern")]
            public string Keywords_ShorthandsRegex { get; set; } = new Regex(@"NOTHING THERE").ToString();

            [JsonProperty("Keywords Shorthands Contextmenu Insertion Shape")]
            public string Keywords_ShorthandsContextMenuInsertionShape { get; set; }

            [JsonProperty("Keywords Shorthands Contextmenu Insertion Shape <KeywordColor>")]
            public string Keywords_ShorthandsContextMenuInsertionShape_HexColor { get; set; }

            [JsonProperty("Keywords Multiple Meanings Dictionary")]
            public string KeywordsMultipleMeaningsDictionary { get; set; } = "";

            [JsonProperty("Keywords Sprite Horizontal Offset")]
            public double KeywordsSpriteHorizontalOffset { get; set; } = 0;

            [JsonProperty("Keywords Sprite Vertical Offset")]
            public double KeywordsSpriteVerticalOffset { get; set; } = 0;


            [JsonProperty("Skill Names Vertical Offset")]
            public double SkillNamesVerticalOffset { get; set; } = 0;


            [JsonProperty("Title Font")]
            public string TitleFont { get; set; } = "";

            [JsonProperty("Title Font (Override Read Name)")]
            public string TitleFont_OverrideReadName { get; set; } = "";

            [JsonProperty("Title Font (Font Weight)")]
            public string TitleFont_FontWeight { get; set; } = "";

            [JsonProperty("Title Font (Font Size Multipler)")]
            public double TitleFont_FontSizeMultipler { get; set; } = 1.0;

            
            [JsonProperty("Context Font")]
            public string ContextFont { get; set; } = "";
            [JsonProperty("Context Font (Override Read Name)")]
            public string ContextFont_OverrideReadName { get; set; } = "";
            [JsonProperty("Context Font (Font Weight)")]
            public string ContextFont_FontWeight { get; set; } = "";
            [JsonProperty("Context Font (Font Size Multipler)")]
            public double ContextFont_FontSizeMultipler { get; set; } = 1.0;
        }
        public record ScanParameters
        {
            [JsonProperty("Skills Area Width")]
            public double AreaWidth { get; set; } = 0;

            [JsonProperty("Scale Factor")]
            public double ScaleFactor { get; set; } = 4;

            [JsonProperty("Background Color")]
            public string BackgroundColor { get; set; } = "#00000000";

            [OnDeserialized]
            private void OnDeserialized(StreamingContext Context)
            {
                if (ScaleFactor > 20) ScaleFactor = 20;
                if (ScaleFactor < 0) ScaleFactor = 1;
            }
        }
        public record TechnicalActions
        {
            [JsonProperty("Keywords Multiple Meanings Dictionary")]
            public TA_KeywordsDictionary KeywordsDictionary { get; set; } = new TA_KeywordsDictionary();
        }
        public record TA_KeywordsDictionary
        {
            [JsonProperty("Generate On Startup")]
            public bool Generate { get; set; } = false;

            [JsonProperty("Source Path")]
            public string Path { get; set; } = "";

            [JsonProperty("Comment")]
            public string Comment { get; set; } = "";
        }
    }
}
