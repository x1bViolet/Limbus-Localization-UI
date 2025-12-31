using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using LC_Localization_Task_Absolute.UITranslationHandlers;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.Configurazione.ConfigDelta.PreviewSettings_PROP.CustomLanguageProperties_PROP.CustomLanguageAssociativeSettings_PROP;
using static LC_Localization_Task_Absolute.Configurazione.ConfigDelta.PreviewSettings_PROP.CustomLanguageProperties_PROP.CustomLanguageAssociativeSettings_PROP.CustomLanguageAssociativePropertyMain_PROP;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;

namespace LC_Localization_Task_Absolute
{
    public static class Configurazione
    {
        public static ConfigDelta LoadedProgramConfig = new();

        /// <summary>
        /// Loaded config variables
        /// </summary>
        public ref struct @CurrentConfess
        {
            public static string SelectedCustomLangName => SelectedCustomLang.PropertyName;
            public static CustomLanguageAssociativePropertyMain_PROP SelectedCustomLang = null;
            public static readonly Dictionary<string, CustomLanguageAssociativePropertyMain_PROP> AssociativePropertiesList = [];

            public static Regex ShorthandsPattern = new Regex(@"\0 NOTHING THERE");

            public static class ShorthandsInsertionParams
            {
                public static string InsertionShape = "[<KeywordID>]";
                public static string InsertionShape_Color = "";
            };
        }


        /// <summary>
        /// Remove default values from limbus custom languages to not create mess (DefaultValueHandling from JsonSerializerSettings is not suitable because it affects all values of the config)
        /// </summary>
        public static void SaveConfig()
        {
            ConfigDelta SettingsCopy = LoadedProgramConfig.TranzitConvert<ConfigDelta>();
            foreach (var CustomLang in SettingsCopy.PreviewSettings.CustomLanguageProperties.AssociativeSettings.List)
            {
                foreach (PropertyInfo Property in CustomLang.Properties.GetType().GetProperties())
                {
                    if (Property.HasAttribute(out DefaultValueAttribute DefaultValueAttribute))
                    {
                        object? CurrentValue = Property.GetValue(CustomLang.Properties);

                        if (CurrentValue != null && CurrentValue.Equals(DefaultValueAttribute.Value))
                        {
                            Property.SetValue(CustomLang.Properties, null); // Set null if value is default (-> ignore at json serialization)
                        }
                    }
                }
            }

            SettingsCopy.SerializeToFormattedFile(@"[⇲] Assets Directory\Configurazione^.json");
        }

        public ref struct @ExtraReplacements
        {
            public static ContextMenu ContextMenu => MainControl.Editor_Background.Resources["ExtraRegexContextMenu"] as ContextMenu;

            public readonly record struct RegexReplaceOption(Regex RegularExpression, string Replacement);

            /// <summary>
            /// Key is regex option name ("{Regex Option} - ..."), List is all "* Pattern: ..." and "  Replace: ..." after the option
            /// </summary>
            public static void ReadFile(string Path)
            {
                try
                {

                    ContextMenu.Items.Clear();

                    Dictionary<string, List<RegexReplaceOption>> Readed = [];
                    string[] Lines = File.ReadAllLines(Path);

                    int LineIndex = 0;
                    string? LatestOptionName = null;

                    foreach (string Line in Lines)
                    {
                        if (Line.StartsWith("{Regex Option} - "))
                        {
                            LatestOptionName = Line[17..];
                            Readed[LatestOptionName] = new List<RegexReplaceOption>();
                        }
                        else if (LatestOptionName != null & Line.StartsWith("* Pattern: ") & (LineIndex < Lines.Length - 1 && Lines[LineIndex + 1].StartsWith("  Replace: ")))
                        {
                            Readed[LatestOptionName].Add(new RegexReplaceOption(RegularExpression: new Regex(pattern: Line[11..]), Replacement: Lines[LineIndex + 1][11..]));
                        }

                        LineIndex++;
                    }

                    foreach (KeyValuePair<string, List<RegexReplaceOption>> ExtraRegexItem in Readed)
                    {
                        UITranslation_Rose CreatedHeader = new();
                        CreatedHeader.InterhintPropertiesFrom(MainControl.ExtraReplacementsContextMenu_ItemHeaderView);
                        CreatedHeader.RichText = ExtraRegexItem.Key;

                        MenuItem CreatedMenuItem = new() { Header = CreatedHeader, DataContext = ExtraRegexItem.Value };
                        CreatedMenuItem.Click += MainControl.TextEditor_SharedContextMenuClick;

                        ContextMenu.Items.Add(CreatedMenuItem);
                    }
                }
                catch (Exception ex) { rin(FormattedStackTrace(ex, "Context menu extra replacements file reading")); }
            }
        }


        public static ResourceDictionary LimbusFonts => Application.Current.Resources.MergedDictionaries[1];


        #region loads
        // Latch
        public static bool ConfigLoadingEvent { get; set; } = false;

        // Per-session, no config
        public static bool Spec_EnableKeywordIDSprite = true;
        public static bool Spec_EnableKeywordIDUnderline = true;

        // To show warnings about missing paths and etc
        public static string LoadErrors = "";

        public static void ReadConfigurazioneFile()
        {
            Mode_EGOGifts.OrganizedData.UpdateDisplayInfo();

            Mode_Skills.LoadDisplayInfo();

            KeywordsInterrogation.LoadInlineImages();
            

            if (File.Exists(@"[⇲] Assets Directory\Configurazione^.json"))
            {
                try
                {
                    ConfigLoadingEvent = true;
                    LoadErrors = "";

                    rin($"\n[ Configurazione^.json loading ]\n");

                    LoadedProgramConfig = new FileInfo(@"[⇲] Assets Directory\Configurazione^.json").Deserealize<ConfigDelta>();


                    // Preset custom localizations list
                    @CurrentConfess.AssociativePropertiesList.Clear();
                    foreach (var CustomLangProperty in LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.List)
                    {
                        @CurrentConfess.AssociativePropertiesList[CustomLangProperty.PropertyName] = CustomLangProperty;

                        if (CustomLangProperty.PropertyName == LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected)
                        {
                            @CurrentConfess.SelectedCustomLang = CustomLangProperty;
                        }
                    }

                    rin(new string('-', "Custom language properties: ".Length + @CurrentConfess.SelectedCustomLangName.Length));

                    // Settings window and 99% of parameters auto set from value changed events
                    ConfigurationWindow.StartupSet();

                    // BattleKeywords background image
                    if (File.Exists(@"[⇲] Assets Directory\Limbus Images\UI\BattleKeywords Background.png"))
                    {
                        MainControl.PreviewLayoutGrid_Keywords_Sub_BattleKeywords_BackgroundImage.Source = BitmapFromFile(@"[⇲] Assets Directory\Limbus Images\UI\BattleKeywords Background.png");
                    }

                    rin("\n\n");
                }
                catch (Exception ex)
                {
                    rin(FormattedStackTrace(ex, "Configurazione^.json reading"));
                }
                finally
                {
                    ConfigLoadingEvent = false;
                }
            }
        }

        public ref struct @PartialStateUpdater
        {
            public ref struct CustomLanguage
            {
                public static void UpdateLimbusKeywords(string LimbusCustomLanguagePath = "*")
                {
                    if (LimbusCustomLanguagePath == "*")
                    {
                        LimbusCustomLanguagePath = @CurrentConfess.SelectedCustomLang.Properties.KeywordsDirectory;
                    }


                    string FallbackKeywords = LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.KeywordsFallbackDirectory;
                    string AdditionalKeywords = LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.AdditionalKeywordsDirectory;


                    // Fallback first
                    if (Directory.Exists(FallbackKeywords))
                    {
                        KeywordsInterrogation.InitializeGlossaryFrom
                        (
                            KeywordsDirectory: FallbackKeywords,
                            WriteOverFallback: false
                        );
                    }
                    else
                    {
                        LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.FallbackKeywordsNotFound
                            .Extern(FallbackKeywords);
                    }

                    // Custom language
                    if (Directory.Exists(LimbusCustomLanguagePath))
                    {
                        KeywordsInterrogation.InitializeGlossaryFrom
                        (
                            KeywordsDirectory: LimbusCustomLanguagePath,
                            WriteOverFallback: true
                        );
                    }
                    else
                    {
                        LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.KeywordsDirNotFound
                            .Extern(LimbusCustomLanguagePath);
                    }

                    // Additional keywords directory
                    if (Directory.Exists(AdditionalKeywords))
                    {
                        KeywordsInterrogation.InitializeGlossaryFrom
                        (
                            KeywordsDirectory: AdditionalKeywords,
                            WriteOverFallback: true,
                            IgnoreUILabels: true
                        );
                    }

                    Limbus_Integration.LimbusPreviewFormatter.UpdateLast();
                }

                public static void UpdateKeywordsMultipleMeaningsDictionary(string DictionaryFilePath)
                {
                    if (File.Exists(DictionaryFilePath))
                    {
                        KeywordsInterrogation.ReadKeywordsMultipleMeanings(DictionaryFilePath);
                    }
                    else if (DictionaryFilePath.Trim() != "")
                    {
                        LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.MultipleKeywordsDictionaryMissing.Extern(DictionaryFilePath);
                    }
                }

                public static void UpdateExtraReplacementsDictionary(string DictionaryFilePath)
                {
                    if (File.Exists(DictionaryFilePath))
                    {
                        @ExtraReplacements.ReadFile(DictionaryFilePath);
                    }
                    else if (DictionaryFilePath.Trim() != "")
                    {
                        LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.ContextMenuExtraReplacementsMissing.Extern(DictionaryFilePath);
                    }
                    else @ExtraReplacements.ContextMenu.Items.Clear();
                }
            }


            public ref struct Fonts
            {
                public static void UpdateFontSizes()
                {
                    #region Context font
                    double ContextFont_Multiplier = (double)@CurrentConfess.SelectedCustomLang.Properties.ContextFont_FontSizeMultipler;
                    foreach (TMProEmitter PreviewLayoutItem in PreviewLayoutsList)
                    {
                        if (MainWindow.PrimaryRegisteredFontSizes.ContainsKey(PreviewLayoutItem))
                        {
                            PreviewLayoutItem.FontSize = MainWindow.PrimaryRegisteredFontSizes[PreviewLayoutItem] * ContextFont_Multiplier;
                        }
                    }
                    #endregion


                    #region Title font
                    double TitleFont_Multiplier = (double)@CurrentConfess.SelectedCustomLang.Properties.TitleFont_FontSizeMultipler;
                    MainControl.SkillNamesReplication_SkillName_Text.FontSize = 23 * TitleFont_Multiplier;
                    MainControl.PassiveNamesReplication_PassiveName_Text.FontSize = 21 * TitleFont_Multiplier;
                    MainControl.NavigationPanel_ObjectName_Display.FontSize = 25 * TitleFont_Multiplier;
                    MainControl.EGOGiftName_PreviewLayout.FontSize = 29 * TitleFont_Multiplier;
                    MainControl.EGOGiftViewDescSign.FontSize = 20 * TitleFont_Multiplier;
                    MainControl.PreviewLayout_Keywords_Bufs_Name.FontSize = 23 * TitleFont_Multiplier;
                    MainControl.PreviewLayout_Keywords_BattleKeywords_Name.FontSize = 20 * TitleFont_Multiplier;
                    #endregion
                }
                public static void UpdateContextFont(string ContextFontFilePath = "*", string Weight = "*", string OverrideReadName = "*")
                {
                    if (ContextFontFilePath == "*") ContextFontFilePath = @CurrentConfess.SelectedCustomLang.Properties.ContextFont;
                    if (Weight == "*") Weight = @CurrentConfess.SelectedCustomLang.Properties.ContextFont_FontWeight;
                    if (OverrideReadName == "*") OverrideReadName = @CurrentConfess.SelectedCustomLang.Properties.ContextFont_OverrideReadName;

                    rin($"- Context font file : \"{ContextFontFilePath}\"");
                    if (File.Exists(ContextFontFilePath))
                    {
                        FontFamily ContextFontFamily = FileToFontFamily(ContextFontFilePath, OverrideReadName, WriteInfo: true);

                        LimbusFonts["Limbus:ContextFont"] = ContextFontFamily;
                        LimbusFonts["Limbus:ContextFont_Weight"] = WeightFrom(Weight);
                    }
                    else
                    {
                        LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.ContextFontMissing
                            .Extern(ContextFontFilePath);
                    }
                }
                public static void UpdateTitleFont(string TitleFontFilePath = "*", string Weight = "*", string OverrideReadName = "*")
                {
                    if (TitleFontFilePath == "*") TitleFontFilePath = @CurrentConfess.SelectedCustomLang.Properties.TitleFont;
                    if (Weight == "*") Weight = @CurrentConfess.SelectedCustomLang.Properties.TitleFont_FontWeight;
                    if (OverrideReadName == "*") OverrideReadName = @CurrentConfess.SelectedCustomLang.Properties.TitleFont_OverrideReadName;

                    rin($"- Title font file : \"{TitleFontFilePath}\"");
                    if (File.Exists(TitleFontFilePath))
                    {
                        FontFamily TitleFontFamily = FileToFontFamily(TitleFontFilePath, OverrideReadName, WriteInfo: true);
                        LimbusFonts["Limbus:TitleFont"] = TitleFontFamily;
                        LimbusFonts["Limbus:TitleFont_Weight"] = WeightFrom(Weight);
                    }
                    else
                    {
                        LoadErrors += ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.TitleFontMissing
                            .Extern(TitleFontFilePath);
                    }
                }
            }
        }

        public static void UpdateCustomLanguagePart(CustomLanguageAssociativePropertyMain_PROP SelectedAssociativePropery)
        {
            rin($"\nCustom language properties: {SelectedAssociativePropery.PropertyName}");

            // Main set
            @CurrentConfess.SelectedCustomLang = SelectedAssociativePropery;
            var Params = SelectedAssociativePropery.Properties;
            rin($"- Keywords Directory: \"{Params.KeywordsDirectory}\"");

            #region General options

            rin($"- Keywords Autodetection Regex Pattern: {LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection}");
            

            @CurrentConfess.ShorthandsPattern = new Regex(Params.Keywords_ShorthandsRegex, RegexOptions.Singleline | RegexOptions.Compiled);
            rin($"- Keywords Shorthands Regex Pattern: {@CurrentConfess.ShorthandsPattern}\n");


            @CurrentConfess.ShorthandsInsertionParams.InsertionShape = Params.Keywords_ShorthandsContextMenuInsertionShape ?? "[<KeywordID>]";
            @CurrentConfess.ShorthandsInsertionParams.InsertionShape_Color = Params.Keywords_ShorthandsContextMenuInsertionShape_HexColor ?? "";
            #endregion


            // Update fonts first
            rin($"Fonts:");
            @PartialStateUpdater.Fonts.UpdateFontSizes();
            @PartialStateUpdater.Fonts.UpdateContextFont();
            @PartialStateUpdater.Fonts.UpdateTitleFont();


            // Keywords second ( -> Limbus_Integration.LimbusPreviewFormatter.UpdateLast(); )
            @PartialStateUpdater.CustomLanguage.UpdateLimbusKeywords(Params.KeywordsDirectory);

            if (Params.KeywordsMultipleMeaningsDictionary != "")
            {
                @PartialStateUpdater.CustomLanguage.UpdateKeywordsMultipleMeaningsDictionary(Params.KeywordsMultipleMeaningsDictionary);
            }

            if (Params.ContextMenuExtraReplacements != "")
            {
                @PartialStateUpdater.CustomLanguage.UpdateExtraReplacementsDictionary(Params.ContextMenuExtraReplacements);
            }





            // If not only starting (Somehow just cant show messagebox at startup fom public MainWindow())
            // Then MainWindow.Window_Loaded() will be triggered with this
            if (LoadErrors != "" & LoadedProgramConfig.Internal.ShowLoadWarnings & MainControl.IsLoaded)
            {
                ShowLoadWarningsWindow();
            }
        }

        public static void UpdatePreviewLayoutsFont(CustomLanguageAssociativePropertyValues_PROP Properties = null)
        {
            if (Properties != null)
            {
                @PartialStateUpdater.Fonts.UpdateFontSizes();

                @PartialStateUpdater.Fonts.UpdateContextFont(Properties.ContextFont, Properties.ContextFont_FontWeight, Properties.ContextFont_OverrideReadName);
                @PartialStateUpdater.Fonts.UpdateTitleFont(Properties.TitleFont, Properties.TitleFont_FontWeight, Properties.TitleFont_OverrideReadName);
            }
        }

        public static void ChecksetLimbusPreviewVisibility()
        {
            MainControl.LimbusPreviewRow.MaxHeight = LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HidePreview
                ? 0
                : double.PositiveInfinity;
        }
        #endregion


        public static void ShowLoadWarningsWindow()
        {
            MessageBox.Show(
                (Configurazione.LoadErrors + ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.WarningsDisablingNotice).Trim(),

                ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.WarningsWindowTitle.Extern(@CurrentConfess.SelectedCustomLang.PropertyName),

                MessageBoxButton.OK,

                MessageBoxImage.Warning);
        }





        [AttributeUsage(AttributeTargets.Property)]
        public class AssignedCheckBoxAttribute(string CheckBoxName) : Attribute { public string CheckBoxName { get; } = CheckBoxName; }

        [AttributeUsage(AttributeTargets.Property)]
        public class AssignedTextBoxAttribute(string TextBoxName, bool RemoveHashAsColor = false) : Attribute
        {
            public string TextBoxName { get; } = TextBoxName;
            public bool RemoveHashAsColor { get; } = RemoveHashAsColor;
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class AssignedComboBoxAttribute(string ComboBoxName, bool ConsiderAsDirectory = false) : Attribute
        {
            public string ComboBoxName { get; } = ComboBoxName;
            public bool ConsiderAsDirectory { get; } = ConsiderAsDirectory;
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class ConfigSectionAttribute : Attribute;


        /// <summary>
        /// Configurazione^.json file
        /// </summary>
        public record ConfigDelta
        {
            [JsonIgnore] private static ConfigurationWindow UI = ConfigurationWindow.ConfigControl;

            [ConfigSection]
            [JsonProperty("Internal")]
            public Internal_PROP Internal { get; set; } = new();
            public record Internal_PROP
            {
                [JsonProperty("UI Language")]
                [AssignedComboBox(nameof(UI.SV_SelectingInterfaceLanguage), ConsiderAsDirectory: true)]
                public string UILanguage { get; set; } = "";

                [JsonProperty("UI Theme")]
                [AssignedComboBox(nameof(UI.SV_SelectingInterfaceTheme), ConsiderAsDirectory: true)]
                public string UITheme { get; set; } = "";

                [JsonProperty("Topmost Window")]
                [AssignedCheckBox(nameof(UI.SV_TopmostWindow))]
                public bool IsAlwaysOnTop { get; set; } = true;

                [JsonProperty("Show Load Warnings")]
                [AssignedCheckBox(nameof(UI.SV_EnableLoadWarnings))]
                public bool ShowLoadWarnings { get; set; } = true;

                [JsonProperty("Enable manual Json files managing")]
                [AssignedCheckBox(nameof(UI.SV_EnableManualJsonFilesManaging))]
                public bool EnableManualJsonFilesManaging { get; set; } = false;
            }

            [ConfigSection]
            [JsonProperty("Preview Settings")]
            public PreviewSettings_PROP PreviewSettings { get; set; } = new();
            public record PreviewSettings_PROP
            {
                [ConfigSection]
                [JsonProperty("Base")]
                public PreviewSettingsBaseSettings_PROP PreviewSettingsBaseSettings { get; set; } = new();
                public record PreviewSettingsBaseSettings_PROP
                {
                    [JsonProperty("Enable Syntax Highlight")]
                    [AssignedCheckBox(nameof(UI.SV_EnableSyntaxHighlight))]
                    public bool EnableSyntaxHighlight { get; set; } = true;

                    [JsonProperty("Hide Limbus Text Preview")]
                    [AssignedCheckBox(nameof(UI.SV_HideLimbusTextPreview))]
                    public bool HidePreview { get; set; } = false;


                    [JsonProperty("Highlight <style>")]
                    [AssignedCheckBox(nameof(UI.SV_HighlightStyle))]
                    public bool HighlightStyle { get; set; } = true;


                    [JsonProperty("Highlight Skill Descs on right click")]
                    [AssignedCheckBox(nameof(UI.SV_HighlightSkillDescsOnRightClick))]
                    public bool HighlightSkillDescsOnRightClick { get; set; } = true;

                    [JsonProperty("Highlight Skill Descs on manual switch")]
                    [AssignedCheckBox(nameof(UI.SV_HighlightSkillDescsOnManualSwitch))]
                    public bool HighlightSkillDescsOnManualSwitch { get; set; } = false;


                    [JsonProperty("Enable Skill Names Replication")]
                    [AssignedCheckBox(nameof(UI.SV_EnableSkillNamesRepliaction))]
                    public bool EnableSkillNamesReplication { get; set; } = true;

                    [JsonProperty("Enable Keyword Tooltips")]
                    [AssignedCheckBox(nameof(UI.SV_EnableKeywordTooltips))]
                    public bool EnableKeywordTooltips { get; set; } = false;


                    [JsonProperty("Enable 7~10 coin buttons")]
                    [AssignedCheckBox(nameof(UI.SV_Enable7to10coins))]
                    public bool Enable7to10CoinButtons { get; set; } = false;


                    [JsonProperty("Preview Update Delay (Seconds)")]
                    [AssignedTextBox(nameof(UI.SV_PreviewUpdateDelay))]
                    public double PreviewUpdateDelay { get; set; } = 0.00;
                }

                [ConfigSection]
                [JsonProperty("Custom Language Properties")]
                public CustomLanguageProperties_PROP CustomLanguageProperties { get; set; } = new();
                public record CustomLanguageProperties_PROP
                {
                    [JsonProperty("Keywords Ignore")]
                    public string[] KeywordsIgnore { get; set; } = [];

                    [JsonProperty("Keywords Fallback")]
                    [AssignedTextBox(nameof(UI.SV_FallbackKeywordDirectory))]
                    public string KeywordsFallbackDirectory { get; set; } = "";

                    [JsonProperty("Additional Keywords Directory")]
                    [AssignedTextBox(nameof(UI.SV_AdditionalKeywordsDirectory))]
                    public string AdditionalKeywordsDirectory { get; set; } = "";

                    [ConfigSection]
                    [JsonProperty("Custom Language Associative Settings")]
                    public CustomLanguageAssociativeSettings_PROP AssociativeSettings { get; set; }
                    public record CustomLanguageAssociativeSettings_PROP
                    {
                        [JsonProperty("Associative Properties Selected")]
                        [AssignedComboBox(nameof(UI.SV_SelectingCustomLanguage))]
                        public string Selected { get; set; } = "";

                        [JsonProperty("Associative Properties List")]
                        public List<CustomLanguageAssociativePropertyMain_PROP> List { get; set; } = [];
                        public record CustomLanguageAssociativePropertyMain_PROP
                        {
                            [JsonProperty("Name")]
                            public string PropertyName { get; set; } = "<none>";

                            [JsonProperty("Hide in list")]
                            public bool HideInList { get; set; } = false;

                            [JsonProperty("Properties")]
                            public CustomLanguageAssociativePropertyValues_PROP Properties { get; set; } = new();
                            public record CustomLanguageAssociativePropertyValues_PROP
                            {
                                [JsonProperty("Keywords Directory")]
                                public string KeywordsDirectory { get; set; } = "";


                                /* lang=regex */
                                [DefaultValue(@"(KeywordNameWillBeHere)(?![\p{L}\[\])\-_<'"":\+])")]
                                [JsonProperty("Keywords Autodetection Regex Pattern")]
                                public string Keywords_AutodetectionRegex { get; set; } = @"(KeywordNameWillBeHere)(?![\p{L}\[\])\-_<'"":\+])";

                                /* lang=regex */
                                [DefaultValue(@"")]
                                [JsonProperty("Keywords Shorthands Regex Pattern")]
                                public string Keywords_ShorthandsRegex { get; set; } = @"";

                                [DefaultValue(null)]
                                [JsonProperty("Keywords Shorthands Contextmenu Insertion Shape")]
                                public string Keywords_ShorthandsContextMenuInsertionShape { get; set; }

                                [DefaultValue(null)]
                                [JsonProperty("Keywords Shorthands Contextmenu Insertion Shape <KeywordColor>")]
                                public string Keywords_ShorthandsContextMenuInsertionShape_HexColor { get; set; }

                                [DefaultValue("")]
                                [JsonProperty("Keywords Multiple Meanings Dictionary")]
                                public string KeywordsMultipleMeaningsDictionary { get; set; } = "";

                                [DefaultValue("")]
                                [JsonProperty("Context Menu Extra Replacements")]
                                public string ContextMenuExtraReplacements { get; set; } = "";

                                [DefaultValue(0.0)]
                                [JsonProperty("Keywords Sprite Horizontal Offset")]
                                public double? KeywordsSpriteHorizontalOffset { get; set; } = 0.0;

                                [DefaultValue(0.0)]
                                [JsonProperty("Keywords Sprite Vertical Offset")]
                                public double? KeywordsSpriteVerticalOffset { get; set; } = 0.0;


                                [JsonProperty("Title Font")]
                                public string TitleFont { get; set; } = "";

                                [DefaultValue("")]
                                [JsonProperty("Title Font (Override Read Name)")]
                                public string TitleFont_OverrideReadName { get; set; } = "";

                                [DefaultValue(nameof(FontWeights.Regular))]
                                [JsonProperty("Title Font (Font Weight)")]
                                public string TitleFont_FontWeight { get; set; } = nameof(FontWeights.Regular);

                                [DefaultValue(1.0)]
                                [JsonProperty("Title Font (Font Size Multipler)")]
                                public double? TitleFont_FontSizeMultipler { get; set; } = 1.0;


                                [JsonProperty("Context Font")]
                                public string ContextFont { get; set; } = "";

                                [DefaultValue("")]
                                [JsonProperty("Context Font (Override Read Name)")]
                                public string ContextFont_OverrideReadName { get; set; } = "";

                                [DefaultValue(nameof(FontWeights.Regular))]
                                [JsonProperty("Context Font (Font Weight)")]
                                public string ContextFont_FontWeight { get; set; } = nameof(FontWeights.Regular);

                                [DefaultValue(1.0)]
                                [JsonProperty("Context Font (Font Size Multipler)")]
                                public double? ContextFont_FontSizeMultipler { get; set; } = 1.0;

                                [OnDeserialized]
                                public void OnDeserialized(StreamingContext Context)
                                {
                                    // They are nullable to remove as default value on config save

                                    KeywordsSpriteHorizontalOffset ??= 0.0;
                                    KeywordsSpriteVerticalOffset ??= 0.0;

                                    TitleFont_FontSizeMultipler ??= 1.0;
                                    ContextFont_FontSizeMultipler ??= 1.0;
                                }
                            }
                        }
                    }
                }
            }

            [ConfigSection]
            [JsonProperty("Scan Parameters")]
            public ScanParameters_PROP ScanParameters { get; set; } = new();
            public record ScanParameters_PROP
            {
                [JsonProperty("Skills Area Width")]
                [AssignedTextBox(nameof(UI.SV_SkillsPanelWidth))]
                public double AreaWidth { get; set; } = 0;

                [JsonProperty("Scale Factor")]
                [AssignedTextBox(nameof(UI.SV_ScansScaleFactor))]
                public double ScaleFactor { get; set; } = 4;

                [JsonProperty("Background Color")]
                [AssignedTextBox(nameof(UI.SV_ScansBackgroundColor), RemoveHashAsColor: true)]
                public string BackgroundColor { get; set; } = "#00000000";

                [JsonProperty("Comments")]
                public List<string> Comments { get; set; }

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    if (ScaleFactor > 15) ScaleFactor = 15;
                    if (ScaleFactor <= 0) ScaleFactor = 1;
                }
            }

            [JsonProperty("Technical Actions")]
            public TechnicalActions_PROP TechnicalActions { get; set; } = new TechnicalActions_PROP();
            public record TechnicalActions_PROP
            {
                [JsonProperty("Keywords Multiple Meanings Dictionary")]
                public TA_KeywordsDictionary_PROP KeywordsDictionary { get; set; } = new TA_KeywordsDictionary_PROP();
                public record TA_KeywordsDictionary_PROP
                {
                    [JsonProperty("Generate On Startup")]
                    public bool GenerateOnStartup { get; set; } = false;

                    [JsonProperty("Source Path")]
                    public string SourcePath { get; set; } = "";

                    [JsonProperty("Comment")]
                    public string Comment { get; set; } = "";
                }
            }
        }
    }
}
