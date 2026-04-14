using LCLocalizationInterface.Internal.Configuration;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using System.Diagnostics.CodeAnalysis;

namespace LCLocalizationInterface.Internal
{
    public ref struct @Configurazione
    {
        public static bool FileLoadingEvent { get; private set; } = false;
        public static bool FileSavingEvent { get; private set; } = false;

        public static void Load()
        {
            FileLoadingEvent = true;
            if (new FileInfo(@"[⇲] Assets Directory\Configurazione^.json").TryDeserealizeJsonAs(out JsonConfigurationFile Loaded, out Exception Occurred))
            {
                @DataContextDomain.Configuration = Loaded;

                @Languages.PresentedTextFields["[Settings / Editor Parameters] * Preview update delay"].Document.Text = $"{LoadedConfiguration.PreviewSettings.Base.PreviewUpdateDelay}";
                @Languages.PresentedTextFields["[Settings / Editor Parameters] * Autosave delay"].Document.Text = $"{LoadedConfiguration.Internal.AutosaveDelay}";
                @Languages.PresentedTextFields["[Settings / Image Exports] * Scale Factor"].Text = $"{LoadedConfiguration.ScanParameters.ScaleFactor}";



                SplashScreenWindow.ProgressSubObject = @Languages.VariableData.ReadedStartupSteps.SubStages.Theme;
                if (Directory.Exists(@"[⇲] Assets Directory\※ Internal\Themes"))
                {
                    SettingsWindow.SettingsWindowInstance.InterfaceThemeSelector.Items.Clear();
                    foreach (DirectoryInfo ThemeDirectory in new DirectoryInfo(@"[⇲] Assets Directory\※ Internal\Themes").GetDirectories())
                    {
                        if (ThemeDirectory.GetFiles().Any(x => x.Name == "Visual.json"))
                        {
                            SettingsWindow.SettingsWindowInstance.InterfaceThemeSelector.Items.Add(ThemeDirectory.Name);
                            string PredictedValue = $"[⇲] Assets Directory/※ Internal/Themes/{ThemeDirectory.Name}";
                            if (PredictedValue == LoadedConfiguration.Internal.UITheme)
                            {
                                SettingsWindow.SettingsWindowInstance.InterfaceThemeSelector.SelectedItem = ThemeDirectory.Name;
                                /// --->  <see cref="@Themes.LoadFromFile"/>
                            }
                        }
                    }
                    if (SettingsWindow.SettingsWindowInstance.InterfaceThemeSelector.SelectedItem is null)
                    {
                        SettingsWindow.SettingsWindowInstance.InterfaceThemeSelector.SelectedIndex = 0;
                    }
                }

                SplashScreenWindow.ProgressSubObject = @Languages.VariableData.ReadedStartupSteps.SubStages.Language;
                if (Directory.Exists(@"[⇲] Assets Directory\※ Internal\Translation"))
                {
                    SettingsWindow.SettingsWindowInstance.InterfaceLanguageSelector.Items.Clear();
                    foreach (DirectoryInfo TranslationDirectory in new DirectoryInfo(@"[⇲] Assets Directory\※ Internal\Translation").GetDirectories())
                    {
                        if (TranslationDirectory
                            .GetFiles()
                            .Select(x => x.Name)
                            .ContainsOneOf("@ Font References.json", "Dynamic UI Text.json", "Static UI Text.json", "Textfield Parameters.json", "Unsaved Changes.json", "Logo.png")
                        ) {
                            SettingsWindow.SettingsWindowInstance.InterfaceLanguageSelector.Items.Add(TranslationDirectory.Name);
                            string PredictedValue = $"[⇲] Assets Directory/※ Internal/Translation/{TranslationDirectory.Name}";
                            if (PredictedValue == LoadedConfiguration.Internal.UILanguage)
                            {
                                SettingsWindow.SettingsWindowInstance.InterfaceLanguageSelector.SelectedItem = TranslationDirectory.Name;
                                /// --->  <see cref="@Languages.ModifyUI"/>
                            }
                        }
                    }
                    if (SettingsWindow.SettingsWindowInstance.InterfaceLanguageSelector.SelectedItem is null)
                    {
                        SettingsWindow.SettingsWindowInstance.InterfaceLanguageSelector.SelectedIndex = 0;
                    }
                }


                /**/SplashScreenWindow.ProgressObject = @Languages.VariableData.ReadedStartupSteps.MainStages.LimbusCompanyPart;

                if (LoadedConfiguration.Internal.SkipSkillsDataReading == false)
                {
                    /**/SplashScreenWindow.ProgressSubObject = @Languages.VariableData.ReadedStartupSteps.SubStages.SkillsData;
                    @SkillsData.ReadSkillsDataFiles();
                }

                /**/SplashScreenWindow.ProgressSubObject = @Languages.VariableData.ReadedStartupSteps.SubStages.CompositeFonts;
                @PartialStateUpdater.Limbus.Fonts.ReadCompositeFontDefinitionsFile();

                // Select limbus custom lang
                SettingsWindow.SettingsWindowInstance.SV_SelectingCustomLanguage.Items.Clear();
                foreach (var LimbusCustomLang in Loaded.PreviewSettings.CustomLang.AssociativeSettings.List)
                {
                    if (LimbusCustomLang.HideInList != true)
                    {
                        IntenseStareType1 LimbusCustomLangSelectionLabel = new()
                        {
                            FontSize = 18, RichText = LimbusCustomLang.Name, DataContext = LimbusCustomLang,
                            TextDecorations = LimbusCustomLang.Name.EndsWith("(Original)") ? TextDecorations.Underline : null
                        };
                        LimbusCustomLangSelectionLabel.InherintPropertiesFrom(@Languages.PresentedTextElements["[Settings / Custom Language] * Selected Properties"]);
                        SettingsWindow.SettingsWindowInstance.SV_SelectingCustomLanguage.Items.Add(LimbusCustomLangSelectionLabel);
                        if (LimbusCustomLang.Name == Loaded.PreviewSettings.CustomLang.AssociativeSettings.Selected)
                        {
                            SettingsWindow.SettingsWindowInstance.SV_SelectingCustomLanguage.SelectedItem = LimbusCustomLangSelectionLabel;
                            /// --->  <see cref="@PartialStateUpdater.Limbus.UpdateFull"/>
                        }
                    }
                }

                if (SettingsWindow.SettingsWindowInstance.SV_SelectingCustomLanguage.SelectedItem is null & SettingsWindow.SettingsWindowInstance.SV_SelectingCustomLanguage.Items.Count > 0)
                {
                    SettingsWindow.SettingsWindowInstance.SV_SelectingCustomLanguage.SelectedIndex = 0;
                }
            }
            else
            {
                SplashScreenWindow.DiscardIfNotStarted();

                ErrorMessageWindow.ShowException(Occurred, "This exception occurred while trying to read main configuration file <b>Configurazione^.json</b>");

                if (ProgramFullyLoaded == false) Application.Current.Shutdown();
            }
            FileLoadingEvent = false;
        }
        
        /// <summary>
        /// Remove default values ​​from Limbus Custom Languages ​​to avoid mess in the json file (<see cref="JsonSerializerSettings.DefaultValueHandling"/> is not suitable because it affects ALL values of the config)
        /// </summary>
        public static void Save()
        {
            try
            {
                if (FileLoadingEvent == false)
                {
                    LoadedConfiguration.SerializeToFormattedJsonFile(@"[⇲] Assets Directory\Configurazione^.json");
                }
            }
            catch (Exception Occurred)
            {
                ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to save <b>Configurazione^.json<b> file");
            }
        }




        





















        public record JsonConfigurationFile : Explicit
        {
            private static string PathStringFormat(string PathString)
            {
                PathString = Path.Exists(PathString) ? PathString.Replace("\\", "/") : string.IsNullOrWhiteSpace(PathString) ? PathString.Trim() : PathString;
                return PathString;
            }

            public record LimbusCustomLangDefinition : Explicit
            {
                [JsonProperty("Name")]
                public string Name { get; set; } = "";

                [JsonProperty("Hide in list")]
                public bool HideInList { get; set; } = false;


                [JsonProperty("Properties")]
                public LangProperties Properties { get; set; } = new();
                public record LangProperties : Explicit
                {
                    [JsonProperty("Keywords Directory"), DefaultValue("")]
                    public string KeywordsDirectory { get; set => field = PathStringFormat(value); } = "";


                    /*lang=regex*/
                    [JsonProperty("Keywords Autodetection Regex Pattern", DefaultValueHandling = DefaultValueHandling.Ignore), StringSyntax(StringSyntaxAttribute.Regex), DefaultValue(@"(KeywordNameWillBeHere)(?![\p{L}\[\]()【】\-_<'"":+])")]
                    public string Keywords_AutodetectionRegex { get; set; } = @"(KeywordNameWillBeHere)(?![\p{L}\[\]()【】\-_<'"":+])";


                    [JsonProperty("Keywords Shorthands Regex Pattern", DefaultValueHandling = DefaultValueHandling.Ignore), StringSyntax(StringSyntaxAttribute.Regex), DefaultValue(@"")]
                    public string Keywords_ShorthandsRegex { get; set; } = @"";


                    [JsonProperty("Keywords Shorthands Contextmenu Insertion Shape", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("")]
                    public string Shorthands_InsertionShape { get; set; } = "";



                    [JsonProperty("Keywords Multiple Meanings Dictionary", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("")]
                    public string KeywordsMultipleMeaningsDictionary { get; set => field = PathStringFormat(value); } = "";


                    [JsonProperty("Context Menu Extra Replacements", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("")]
                    public string ContextMenuExtraReplacements { get; set => field = PathStringFormat(value); } = "";



                    [JsonProperty("Keywords Sprite Horizontal Offset", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(0.0)]
                    public double KeywordsSpriteHorizontalOffset { get; set; } = 0.0;


                    [JsonProperty("Keywords Sprite Vertical Offset", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(0.0)]
                    public double KeywordsSpriteVerticalOffset { get; set; } = 0.0;



                    [JsonProperty("Title Font"), DefaultValue("")]
                    public string TitleFont { get; set => field = PathStringFormat(value); } = "";


                    [JsonProperty("Title Font (Font Weight)", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(nameof(FontWeights.Regular))]
                    public string TitleFont_FontWeight { get; set; } = nameof(FontWeights.Regular);



                    [JsonProperty("Context Font"), DefaultValue("")]
                    public string ContextFont { get; set => field = PathStringFormat(value); } = "";


                    [JsonProperty("Context Font (Font Weight)", DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(nameof(FontWeights.Regular))]
                    public string ContextFont_FontWeight { get; set; } = nameof(FontWeights.Regular);
                }
            }












            [JsonProperty("Internal")]
            public Internal_PROP Internal { get; set; } = new();
            public record Internal_PROP : Explicit
            {
                [JsonProperty("Check for updates")]
                public bool CheckForUpdates { get; set; } = true;


                [JsonProperty("Show startup progress")]
                public bool ShowStartupProgress { get; set; } = true;


                [JsonProperty("UI Language")]
                public string UILanguage { get; set => field = PathStringFormat(value); } = "";


                [JsonProperty("UI Theme")]
                public string UITheme { get; set => field = PathStringFormat(value); } = "";


                [JsonProperty("Topmost Window")]
                public bool IsAlwaysOnTop { get; set; } = true;




                [JsonProperty("Enable manual Json files managing")]
                public bool EnableManualJsonFilesManaging { get; set; } = false;


                [JsonProperty("Skip Skills Data reading")]
                public bool SkipSkillsDataReading { get; set; } = false;


                [JsonProperty("Disable 'Unknown' for unidentified keywords")]
                public bool DisableUnknownForUnidentifiedKeywords { get; set; } = false;


                [JsonProperty("Disable Sealing of Text Elements in Identity/E.G.O Preview Creator")]
                public bool DisableTextElementsSealingInPreviewCreator { get; set; } = false;


                [JsonProperty("Enable autosave")]
                public bool EnableAutoSave { get; set; } = false;


                [JsonProperty("Autosave delay")]
                public double AutosaveDelay { get; set; } = 0.0;


                public void AssertAutoSaveDelay()
                {
                    if (AutosaveDelay < 0.0) AutosaveDelay = 0.0;
                }


                [JsonProperty("Specific title bar buttons On/Off")]
                public SpecificTitleBarButtonsOnOff_PROP SpecificTitleBarButtonsOnOff { get; set; } = new();
                public record SpecificTitleBarButtonsOnOff_PROP : Explicit
                {
                    [JsonProperty("Editor mode switch")]
                    public bool EditorModeSwitch { get; set; } = true;


                    [JsonProperty("Open Skills Display Info manager")]
                    public bool OpenSkillsDisplayInfoManager { get; set; } = true;


                    [JsonProperty("Switch UI to Identity/E.G.O Preview Creator")]
                    public bool SwitchUIToPreviewCreator { get; set; } = true;


                    [JsonProperty("Export text to image")]
                    public bool ExportTextToImage { get; set; } = true;
                }
            }



            [JsonProperty("Preview Settings")]
            public PreviewSettings_PROP PreviewSettings { get; set; } = new();
            public record PreviewSettings_PROP : Explicit
            {

                [JsonProperty("Base")]
                public Base_PROP Base { get; set; } = new();
                public record Base_PROP : Explicit
                {
                    [JsonProperty("Enable Syntax Highlight")]
                    public bool EnableSyntaxHighlight { get; set; } = true;



                    [JsonProperty("Hide Limbus Text Preview")]
                    public bool HidePreview { get; set; } = false;



                    [JsonProperty("Highlight <style>")]
                    public bool HighlightStyle { get; set; } = true;



                    [JsonProperty("Highlight descs on right click")]
                    public bool HighlightDescsOnRightClick { get; set; } = true;


                    [JsonProperty("Highlight descs on manual switch")]
                    public bool HighlightDescsOnManualSwitch { get; set; } = false;



                    [JsonProperty("Enable Skill Names Replication")]
                    public bool EnableSkillNamesReplication { get; set; } = true;


                    [JsonProperty("Enable Keyword Tooltips")]
                    public bool EnableKeywordTooltips { get; set; } = true;



                    [JsonProperty("Enable 7~10 Coin buttons of Skills")]
                    public bool Enable7to10CoinButtons { get; set; } = false;



                    [JsonProperty("Enable 7~10 Simple Description buttons of EGO Gifts")]
                    public bool Enable7to10EGOGiftSimpleDescButtons { get; set; } = false;



                    [JsonProperty("Preview Update Delay (Seconds)")]
                    public double PreviewUpdateDelay { get; set; } = 0.0;
                }


                [JsonProperty("Custom Language Properties")]
                public CustomLang_PROP CustomLang { get; set; } = new();
                public record CustomLang_PROP : Explicit
                {
                    // lang=regex
                    [JsonProperty("Implicit keywords conversion ignores")]
                    public string[] ImplicitKeywordsConversionIgnores { get; set; } = [];

                    [JsonProperty("Keywords Fallback")]
                    public string KeywordsFallbackDirectory { get; set => field = PathStringFormat(value); } = "";

                    [JsonProperty("Additional Keywords Directory")]
                    public string AdditionalKeywordsDirectory { get; set => field = PathStringFormat(value); } = "";


                    [JsonProperty("Custom Language Associative Settings")]
                    public AssociativeSettings_PROP AssociativeSettings { get; set; } = new();
                    public record AssociativeSettings_PROP : Explicit
                    {
                        [JsonProperty("Associative Properties Selected")]
                        public string Selected { get; set; } = "";


                        [JsonProperty("Associative Properties List")]
                        public List<LimbusCustomLangDefinition> List { get; set; } = [];
                    
                    }
                }
            }


            [JsonProperty("Scan Parameters")]
            public ScanParameters_PROP ScanParameters { get; set; } = new();
            public record ScanParameters_PROP : Explicit
            {
                [JsonProperty("Scale Factor")]
                public double ScaleFactor { get; set; } = 3.3;


                [JsonProperty("Background Color")]
                public string BackgroundColor { get; set; } = "#00000000";


                [JsonProperty("Enable Keywords Sprite")]
                public bool EnableKeywordsSprite { get; set; } = true;

                [JsonProperty("Enable Keywords Underline")]
                public bool EnableKeywordsUnderline { get; set; } = true;


                [JsonProperty("Preloaded Image Info for Identity/E.G.O Preview Creator")]
                public string PreloadedImageInfoForPreviewCreator { get; set => field = PathStringFormat(value); } = "";


                [JsonProperty("Comments")]
                public List<string> Comments { get; set; } = [];


                public void AssertScaleFactor()
                {
                    if (ScaleFactor > 8) ScaleFactor = 8;
                    if (ScaleFactor <= 0) ScaleFactor = 1;
                }

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    AssertScaleFactor();
                    BackgroundColor = BackgroundColor.Cut("#");
                }
            }
        }
    }
}