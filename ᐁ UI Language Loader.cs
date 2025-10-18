using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.Json.FilesIntegration;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader.InterfaceLocalizationModifiers.Frames.StaticOrDynamic_UI_Text;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute
{
    #region Specialized UI Elements
    /// <summary>
    /// Unifier for <see cref="UITranslation_Mint"/> and <see cref="UITranslation_Rose"/> for method <see cref="ᐁ_Interface_Localization_Loader.ModifySingleObject"/>
    /// </summary>
    public interface UILocalization_Entry
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double FontSize { get; set; }
        public FontFamily FontFamily { get; set; }
        public FontWeight FontWeight { get; set; }
        public DependencyObject Parent { get; }
        public Thickness Margin { get; set; }
        public Thickness Padding { get; set; }
        public Visibility Visibility { get; set; }
        public Brush Foreground { get; set; }
        public TextAlignment TextAlignment { get; set; }
    };


    /// <summary>
    /// TextBox with <see cref="UID"/> property for language loading navigation
    /// </summary>
    sealed public partial class UITranslation_Mint : TextBox, UILocalization_Entry
    {
        /// <summary>
        /// Create binding of all properties
        /// </summary>
        public void InterhintPropertiesFrom(UITranslation_Mint AnotherTextElement)
        {
            this.BindSameProperties(
                AnotherTextElement,
                [
                    UITranslation_Mint.FontFamilyProperty, UITranslation_Mint.FontWeightProperty, UITranslation_Mint.FontSizeProperty,
                    UITranslation_Mint.ForegroundProperty, UITranslation_Mint.TextAlignmentProperty,
                    UITranslation_Mint.MarginProperty,     UITranslation_Mint.PaddingProperty,
                    UITranslation_Mint.WidthProperty,      UITranslation_Mint.HeightProperty
                ]
            );
        }

        public static readonly DependencyProperty UIDProperty =
            DependencyProperty.Register(
                "UID",
                typeof(string),
                typeof(UITranslation_Mint),
                new PropertyMetadata("", OnUIDChanged));

        private static void OnUIDChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            string DefinedUID = ChangeArgs.NewValue as string;

            if (!ᐁ_Interface_Localization_Loader.PresentedTextInputEntries.ContainsKey(DefinedUID) && !DefinedUID.IsNullOrEmpty())
            {
                ᐁ_Interface_Localization_Loader.PresentedTextInputEntries[DefinedUID] = CurrentElement as UITranslation_Mint;
            }
        }

        public string UID
        {
            get => (string)GetValue(UIDProperty);
            set => SetValue(UIDProperty, value);
        }
    }

    /// <summary>
    /// TextBlock with <see cref="UID"/> property for language loading navigation (And <see cref="RichText"/>)
    /// </summary>
    sealed public partial class UITranslation_Rose : TextBlock, UILocalization_Entry
    {
        /// <summary>
        /// Create binding of all properties (Except <see cref="RichText"/>, regular TextProperty only)
        /// </summary>
        public void InterhintPropertiesFrom(UITranslation_Rose AnotherTextElement)
        {
            this.BindSameProperties(
                AnotherTextElement,
                [
                    UITranslation_Rose.FontFamilyProperty, UITranslation_Rose.FontWeightProperty, UITranslation_Rose.FontSizeProperty,
                    UITranslation_Rose.TextProperty,       UITranslation_Rose.ForegroundProperty, UITranslation_Rose.TextAlignmentProperty,
                    UITranslation_Rose.MarginProperty,     UITranslation_Rose.PaddingProperty,
                    UITranslation_Rose.WidthProperty,      UITranslation_Rose.HeightProperty
                ]
            );
        }

        public static readonly DependencyProperty UIDProperty =
            DependencyProperty.Register(
                "UID",
                typeof(string),
                typeof(UITranslation_Rose),
                new PropertyMetadata("", OnUIDChanged));

        public string UID
        {
            get => (string)GetValue(UIDProperty);
            set => SetValue(UIDProperty, value);
        }

        private static void OnUIDChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            string DefinedUID = ChangeArgs.NewValue as string;

            if (!ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries.ContainsKey(DefinedUID) && !DefinedUID.IsNullOrEmpty())
            {
                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[DefinedUID] = CurrentElement as UITranslation_Rose;
            }
        }


        public static readonly DependencyProperty PerfectVerticalAlignProperty =
            DependencyProperty.Register(
                "PerfectVerticalAlign",
                typeof(bool),
                typeof(UITranslation_Rose),
                new PropertyMetadata(false, OnPerfectVerticalAlignChanged));

        public bool PerfectVerticalAlign
        {
            get => (bool)GetValue(PerfectVerticalAlignProperty);
            set => SetValue(PerfectVerticalAlignProperty, value);
        }

        private static void OnPerfectVerticalAlignChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            if ((bool)ChangeArgs.NewValue == true)
            {
                (CurrentElement as UITranslation_Rose).SetBinding(TextBlock.LineHeightProperty, new Binding("FontSize")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.Self)
                });
            }
            else
            {
                (CurrentElement as UITranslation_Rose).LineHeight = double.NaN;
            }
        }



        public static readonly DependencyProperty RichTextProperty =
            DependencyProperty.Register(
                "RichText",
                typeof(string),
                typeof(UITranslation_Rose),
                new PropertyMetadata("", OnRichTextChanged));

        private static void OnRichTextChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            Pocket_Watch_ː_Type_L.Actions.Apply(
                Target: CurrentElement as UITranslation_Rose,
                RichText: ChangeArgs.NewValue as string,
                DividersMode: Pocket_Watch_ː_Type_L.@PostInfo.FullStopDividers.FullStopDividers_Regular,
                DoCustomIdentityPreviewCreatorIsActiveCheck: false,
                DisableKeyworLinksCreation: true
            );
        }

        public string RichText
        {
            set
            {
                Pocket_Watch_ː_Type_L.Actions.Apply(
                    Target: this,
                    RichText: value,
                    DividersMode: Pocket_Watch_ː_Type_L.@PostInfo.FullStopDividers.FullStopDividers_Regular,
                    DisableKeyworLinksCreation: true
                );
                CurrentRichText = value;
            }
        }

        public string CurrentRichText { get; private set; } = "";

        /// <summary>
        /// Disable regular Text property
        /// </summary>
        private new string Text;


        // For something
        public Dictionary<string, dynamic> UniversalDataBindings = new Dictionary<string, dynamic>();

        public UITranslation_Rose()
        {
            TextWrapping = TextWrapping.Wrap;
            LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            //Foreground = Brushes.Beige;

            FontSize = 13;

            // Not there
            //this.SetBinding(TextBlock.LineHeightProperty, new Binding("FontSize")
            //{
            //    RelativeSource = new RelativeSource(RelativeSourceMode.Self)
            //});
        }
    }
    #endregion
    
    public abstract class ᐁ_Interface_Localization_Loader
    {
        public abstract class InterfaceLocalizationModifiers
        {
            public static Dictionary<string, string> @Font_References = new Dictionary<string, string>();
            public static Dictionary<string, FontFamily> @Font_References_Loaded = new Dictionary<string, FontFamily>();

            public record StaticOrDynamic_UI_Text
            {
                [JsonProperty("Font Defaults")]
                public Frames.StaticOrDynamic_UI_Text.Font_Defaults Font_Defaults { get; set; } = new Frames.StaticOrDynamic_UI_Text.Font_Defaults();

                [JsonProperty("Custom Logo")]
                public string CustomLogo { get; set; } = "";

                [JsonProperty("Readme")]
                public List<string> Readme { get; set; } = new List<string>();

                [JsonProperty("List<Translation>")]
                public List<List<Frames.StaticOrDynamic_UI_Text.InterfaceTranslationParameter>> List { get; set; } = new List<List<Frames.StaticOrDynamic_UI_Text.InterfaceTranslationParameter>>();

                [JsonProperty("List<Parametre>")] // For `UI Textfields.json`
                private List<List<Frames.StaticOrDynamic_UI_Text.InterfaceTranslationParameter>> List_SecondName { set { List = value; } }

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    if (File.Exists(CustomLogo)) MainWindow.MainControl.MainMenuLogo.Source = BitmapFromFile(CustomLogo);
                }
            }

            public abstract class Frames
            {
                public abstract class StaticOrDynamic_UI_Text
                {
                    public record Font_Defaults
                    {
                        [JsonProperty("Font")]
                        public string Font { get; set; } = "";

                        [JsonProperty("Font Weight")]
                        public string Font_Weight { get; set; } = "";
                    }

                    public record InterfaceTranslationParameter
                    {
                        [JsonProperty("UID")]
                        public Dictionary<string, dynamic> UID { get; set; } = new Dictionary<string, dynamic>(); // "UID": {"[C] * [Section Title] Decorative cautions":null},


                        [JsonProperty("Text")]
                        public string Text { get; set; }

                        [JsonProperty("Variable")]
                        public Dictionary<string, string> Variable { get; set; }

                        [JsonProperty("Text Alignment")]
                        public string Text_Alignment { get; set; }

                        [JsonProperty("Font")]
                        public string Font { get; set; }


                        [JsonProperty("Font Size")]
                        public double? Font_Size { get; set; }

                        [JsonProperty("Font Weight")]
                        public string Font_Weight { get; set; }


                        [JsonProperty("Foreground")]
                        public string Foreground { get; set; }


                        [JsonProperty("Margin")]
                        public double[] Margin { get; set; }

                        [JsonProperty("Padding")]
                        public double[] Padding { get; set; }


                        [JsonProperty("Width")]
                        public double? Width { get; set; }

                        [JsonProperty("Height")]
                        public double? Height { get; set; }


                        [JsonProperty("Visible")]
                        public bool? Visible { get; set; }


                        [JsonIgnore]
                        public Thickness? Margin_Loaded { get; set; }

                        [JsonIgnore]
                        public Thickness? Padding_Loaded { get; set; }

                        [JsonIgnore]
                        public Visibility? Visible_Loaded { get; set; }

                        [JsonIgnore]
                        public FontFamily? Font_Loaded { get; set; }

                        [JsonIgnore]
                        public FontWeight? Font_Weight_Loaded { get; set; }

                        [OnDeserialized]
                        private void OnDeserialized(StreamingContext Context)
                        {
                            if (Margin != null && Margin.Length > 0)
                            {
                                if (Margin.Length == 1 | Margin.Length == 4) Margin_Loaded = ThicknessFrom(Margin);
                            }
                            if (Padding != null && Padding.Length > 0)
                            {
                                if (Padding.Length == 1 | Padding.Length == 4) Padding_Loaded = ThicknessFrom(Padding);
                            }

                            if (Visible != null && Visible == false)
                            {
                                Visible_Loaded = Visibility.Collapsed;
                            }

                            // Font_Loaded Font_Weight_Loaded by situation at SpecializedDefs.Transform()
                        }
                    }
                }

                public record CustomLangLoadingWarnings
                {
                    [JsonProperty("Fallback Keywords Directory not found")]
                    public string FallbackKeywordsNotFound { get; set; } = "¤ Cannot find fallback keywords directory \"[$]\" (Can it be on disk D:\\ or E:\\??)\n\n";

                    [JsonProperty("Selected Custom Language not found in list")]
                    public string CustomLanguagePropertyNotFound { get; set; } = "¤ Cannot find Custom Language property named \"[$]\"\n\n";

                    [JsonProperty("Custom Language Keywords Directory not found")]
                    public string KeywordsDirNotFound { get; set; } = "¤ Cannot find Custom Language keywords directory \"[$]\"\n\n";

                    [JsonProperty("Custom Language Context Font not found")]
                    public string ContextFontMissing { get; set; } = "¤ Cannot find Context Font file \"[$]\"\n\n";

                    [JsonProperty("Custom Language Title Font not found")]
                    public string TitleFontMissing { get; set; } = "¤ Cannot find Title Font file \"[$]\"\n\n";

                    [JsonProperty("Custom Language Keywords Multiple Meanings Dictionary not found")]
                    public string MultipleKeywordsDictionaryMissing { get; set; } = "¤ Cannot find Keywords Multiple Meanings Dictionary \"[$]\"\n\n";

                    [JsonProperty("Warnings disabling notice")]
                    public string WarningsDisablingNotice { get; set; } = "\n\n(You can disable this warning in Settings at 'Internal' section)";

                    [JsonProperty("Warnings window title")]
                    public string WarningsWindowTitle { get; set; } = "Loading exceptions @ [$]";
                }

                public abstract class UnsavedChangesInfo
                {
                    public record UnsavedChangesInfo_Main
                    {
                        [JsonProperty("Passives")]
                        public UnsavedChangesInfo_Passives Passives { get; set; } = new UnsavedChangesInfo_Passives();

                        [JsonProperty("Keywords")]
                        public UnsavedChangesInfo_Keywords Keywords { get; set; } = new UnsavedChangesInfo_Keywords();

                        [JsonProperty("E.G.O Gifts")]
                        public UnsavedChangesInfo_EGOGifts EGOGifts { get; set; } = new UnsavedChangesInfo_EGOGifts();

                        [JsonProperty("Skills")]
                        public UnsavedChangesInfo_Skills Skills { get; set; } = new UnsavedChangesInfo_Skills();
                    }
                    public record UnsavedChangesInfo_Passives
                    {
                        [JsonProperty("ID Header")]
                        public string IDHeader { get; set; } = "\n\n<b>ID</b> <color=#f8c200>[$1]</color> 「<color=#afbff9>[$2]</color>」";

                        [JsonProperty("Main Desc")]
                        public string MainDesc { get; set; } = "\n  > Main description";

                        [JsonProperty("Summary Desc")]
                        public string SummaryDesc { get; set; } = "\n  > Summary description";
                    }
                    public record UnsavedChangesInfo_Keywords
                    {
                        [JsonProperty("ID Header")]
                        public string IDHeader { get; set; } = "\n\n<b>ID</b> <color=#f8c200>[$1]</color> 「<color=#afbff9>[$2]</color>」";

                        [JsonProperty("Main Desc")]
                        public string MainDesc { get; set; } = "\n  > Main description";

                        [JsonProperty("Summary Desc")]
                        public string SummaryDesc { get; set; } = "\n  > Summary description";
                    }
                    public record UnsavedChangesInfo_EGOGifts
                    {
                        [JsonProperty("ID Header")]
                        public string IDHeader { get; set; } = "\n\n<b>ID</b> <color=#f8c200>[$1]</color> 「<color=#afbff9>[$2]</color>」";

                        [JsonProperty("Main Desc")]
                        public string MainDesc { get; set; } = "\n  > Main description";

                        [JsonProperty("Simple Desc")]
                        public string SimpleDesc { get; set; } = "\n  > Simple description №[$]";
                    }
                    public record UnsavedChangesInfo_Skills
                    {
                        [JsonProperty("ID Header")]
                        public string IDHeader { get; set; } = "\n\n<b>ID</b> <color=#f8c200>[$1]</color> 「<color=#afbff9>[$2]</color>」";

                        [JsonProperty("Uptie Level")]
                        public string UptieLevel { get; set; } = "\n  > Uptie level [$]";
                    }
                }
            }
        }


        public static Dictionary<string, UITranslation_Rose> PresentedStaticTextEntries = new Dictionary<string, UITranslation_Rose>();
        public static Dictionary<string, UITranslation_Mint> PresentedTextInputEntries = new Dictionary<string, UITranslation_Mint>();

        public static Dictionary<string, InterfaceTranslationParameter> LoadedModifiers = new Dictionary<string, InterfaceTranslationParameter>();

        public static List<InterfaceLocalizationModifiers.Frames.StaticOrDynamic_UI_Text.InterfaceTranslationParameter> ExportInfo = new();

        public static string GetLocalizationTextFor(string UID, string VariableKey = null)
        {
            if (VariableKey == null & LoadedModifiers.ContainsKey(UID))
            {
                return LoadedModifiers[UID].Text;
            }
            else
            {
                if (LoadedModifiers.ContainsKey(UID) && LoadedModifiers[UID].Variable != null && LoadedModifiers[UID].Variable.ContainsKey(VariableKey))
                {
                    return LoadedModifiers[UID].Variable[VariableKey];
                }
                else
                {
                    return $"<size=78%><b>[No Variable '{VariableKey}' with '{UID}' UID in translation..]</b></size>\n:: <size=78%><color=#fc5a03>[$]</color></size>";
                }
            }
        }
        
        public static void ExternElement(string UID, string VariableKey, string ExternString = "")
        {
            PresentedStaticTextEntries[UID].RichText = GetLocalizationTextFor(UID, VariableKey).Extern(ExternString);
        }

        public abstract class SpecializedDefs
        {
            public static string UnsavedChangesMarker = "[$] <size=66%><color=#fc5a03><b>(Changed)</b></color></size>";
            public static string InsertionsDefaultValue = "…";

            public static InterfaceLocalizationModifiers.Frames.UnsavedChangesInfo.UnsavedChangesInfo_Main UnsavedChangesInfo = new();
            public static InterfaceLocalizationModifiers.Frames.CustomLangLoadingWarnings CustomLangLoadingWarnings = new();
        }

        private static void ModifySingleObject(UILocalization_Entry Target, InterfaceTranslationParameter Param, FontFamily DefaultFontFamily, FontWeight DefaultFontWeight)
        {
            if (Target == null) return;

            if (Param.UID.Keys.First().StartsWith("[Context Menu] * "))
            {
                if (Param.Visible != null && Param.Visible == false) Target.Visibility = (Visibility)Param.Visible_Loaded;
                else Target.Visibility = Visibility.Visible;
            }

            if (Param.Foreground != null && Target is UITranslation_Mint ManualIDUnputMint && ManualIDUnputMint.UID == "[Main UI] * Manual ID Input")
            {
                ManualIDUnputMint.Foreground = ManualIDUnputMint.CaretBrush = ManualIDUnputMint.SelectionBrush = ToSolidColorBrush(Param.Foreground);
                ManualIDUnputMint.Visibility = Visibility.Collapsed;
            }

            if (!$"{Param.Font_Weight}".Equals("$Ignore-default")) Target.FontWeight = DefaultFontWeight;
            if (!$"{Param.Font}".Equals("$Ignore-default")) Target.FontFamily = DefaultFontFamily;
            
            if (Param.Font_Size != null) Target.FontSize = (double)Param.Font_Size;
            if (Param.Foreground != null)
            {
                Target.Foreground = ToSolidColorBrush(Param.Foreground);
            }

            if (Param.Font_Weight != null) Target.FontWeight = WeightFrom(Param.Font_Weight);
            if (Param.Font != null && !Param.Font.Equals("$Ignore-default"))
            {
                if (InterfaceLocalizationModifiers.@Font_References_Loaded.ContainsKey(Param.Font))
                {
                    // Get from loaded
                    Target.FontFamily = InterfaceLocalizationModifiers.@Font_References_Loaded[Param.Font];
                }
                else if (File.Exists(Param.Font))
                {
                    // Get from file
                    Target.FontFamily = FileToFontFamily(Param.Font);
                }
                else
                {
                    // Get by name from pc
                    Target.FontFamily = new FontFamily(Param.Font);
                }
            }

            if (Param.Margin_Loaded  != null) Target.Margin     = (Thickness)Param.Margin_Loaded;
            if (Param.Padding_Loaded != null) Target.Padding    = (Thickness)Param.Padding_Loaded;
            if (Param.Visible_Loaded != null) Target.Visibility = (Visibility)Param.Visible_Loaded;


            if (Param.Width != null)
            {
                if (Target.Parent is ToolTip tooltip)
                {
                    Target.Width = tooltip.Width = (double)Param.Width;
                }
                else
                {
                    Target.Width = (double)Param.Width;
                }
            }
            if (Param.Height != null)
            {
                if (Target.Parent is ToolTip tooltip)
                {
                    Target.Height = tooltip.Height = (double)Param.Height;
                }
                else
                {
                    Target.Height = (double)Param.Height;
                }
            }

            if (Target is UITranslation_Rose RoseumType) // Only one difference
            {
                string TextToSet = "";

                if (Param.Variable != null && Param.Variable.Keys.Count > 0)
                {
                    TextToSet = Param.Variable.First().Value;
                }
                else if (!Param.Text.IsNullOrEmpty())
                {
                    TextToSet = Param.Text;
                }

                if (TextToSet.Contains("[$]")) TextToSet = TextToSet.Extern(SpecializedDefs.InsertionsDefaultValue);

                if (Param.UID.Keys.First().Contains("[!]")) RoseumType.Text = TextToSet;
                else RoseumType.RichText = TextToSet;
            }

            if (Param.Text_Alignment != null)
            {
                TextAlignment TargetTextAlignment = Param.Text_Alignment switch
                {
                    "Left" => TextAlignment.Left,
                    "Right" => TextAlignment.Right,
                    "Center" => TextAlignment.Center,
                    "Justify" => TextAlignment.Justify,
                };

                Target.TextAlignment = TargetTextAlignment;
            }
        }

        public static bool LanguageTranslationModifyingEvent = false;
        public static void ModifyUI(string LocalizationInfoPath)
        {
            LanguageTranslationModifyingEvent = true;
            if (Directory.Exists(LocalizationInfoPath))
            {
                List<FileInfo> InfoPreset = new List<string>()
                {   @$"{LocalizationInfoPath}\@ Font References.json",
                    @$"{LocalizationInfoPath}\Static UI Text.json",
                    @$"{LocalizationInfoPath}\Dynamic UI Text.json",
                    @$"{LocalizationInfoPath}\Textfield Parameters.json",
                    @$"{LocalizationInfoPath}\Unsaved Changes.json",
                    @$"{LocalizationInfoPath}\Custom Localization Load Warnings.json",
                }.ToFileInfos();

                if (InfoPreset[0].Exists) // @ Font References.json
                {
                    InterfaceLocalizationModifiers.@Font_References = InfoPreset[0].Deserealize<Dictionary<string, string>>();
                    if (InterfaceLocalizationModifiers.@Font_References.Keys.Count > 0)
                    {
                        InterfaceLocalizationModifiers.@Font_References_Loaded.Clear();
                        foreach (KeyValuePair<string, string> FontReference in InterfaceLocalizationModifiers.@Font_References)
                        {
                            InterfaceLocalizationModifiers.@Font_References_Loaded[FontReference.Key] = FileToFontFamily(FontReference.Value);
                        }
                    }
                }

                if (InfoPreset[2].Exists) // Dynamic UI Text (Load [Main UI] Default Unsaved changes marker value first)
                {
                    InterfaceLocalizationModifiers.StaticOrDynamic_UI_Text TranslationData = InfoPreset[2].Deserealize<InterfaceLocalizationModifiers.StaticOrDynamic_UI_Text>();

                    if (TranslationData.List.Count > 0)
                    {
                        FontFamily DefaultFont = new FontFamily();
                        FontWeight DefaultWeight = WeightFrom(TranslationData.Font_Defaults.Font_Weight);

                        if (InterfaceLocalizationModifiers.@Font_References.ContainsKey(TranslationData.Font_Defaults.Font))
                        {
                            DefaultFont = InterfaceLocalizationModifiers.@Font_References_Loaded[TranslationData.Font_Defaults.Font];
                        }
                        else if (File.Exists(TranslationData.Font_Defaults.Font))
                        {
                            DefaultFont = FileToFontFamily(TranslationData.Font_Defaults.Font);
                        }
                        else
                        {
                            DefaultFont = new FontFamily(TranslationData.Font_Defaults.Font);
                        }

                        foreach (List<InterfaceTranslationParameter> LocalizationParameterSection in TranslationData.List)
                        {
                            foreach (InterfaceTranslationParameter Param in LocalizationParameterSection)
                            {
                                if (Param.UID.Keys.Count > 0) // "UID": {"[C] * Title bar button tooltip":null}
                                {
                                    string UID = Param.UID.Keys.First();

                                    LoadedModifiers[UID] = Param;

                                    switch (UID) // Unique cases
                                    {
                                        case "[Main UI] Unsaved changes marker":
                                            SpecializedDefs.UnsavedChangesMarker = Param.Text;
                                            break;

                                        case "[Main UI] Default insertion value":
                                            SpecializedDefs.InsertionsDefaultValue = Param.Text;
                                            break;

                                        case "[Main UI] [!] * Default Object name":
                                            MainWindow.MainControl.NavigationPanel_ObjectName_Display.Text = Param.Text;
                                            break;

                                        default: break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (InfoPreset[1].Exists) // Static UI Text
                {
                    InterfaceLocalizationModifiers.StaticOrDynamic_UI_Text TranslationData = InfoPreset[1].Deserealize<InterfaceLocalizationModifiers.StaticOrDynamic_UI_Text>();

                    if (TranslationData.List.Count > 0)
                    {
                        FontFamily DefaultFont = new FontFamily();
                        FontWeight DefaultWeight = WeightFrom(TranslationData.Font_Defaults.Font_Weight);

                        if (InterfaceLocalizationModifiers.@Font_References.ContainsKey(TranslationData.Font_Defaults.Font))
                        {
                            DefaultFont = InterfaceLocalizationModifiers.@Font_References_Loaded[TranslationData.Font_Defaults.Font];
                        }
                        else if (File.Exists(TranslationData.Font_Defaults.Font))
                        {
                            DefaultFont = FileToFontFamily(TranslationData.Font_Defaults.Font);
                        }

                        foreach (List<InterfaceTranslationParameter> LocalizationParameterSection in TranslationData.List)
                        {
                            foreach (InterfaceTranslationParameter Param in LocalizationParameterSection)
                            {
                                if (Param.UID.Keys.Count > 0) // "UID": {"[C] * Title bar button tooltip":null}
                                {
                                    string UID = Param.UID.Keys.First();

                                    if (ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries.ContainsKey(UID))
                                    {
                                        LoadedModifiers[UID] = Param;

                                        UITranslation_Rose Target = ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[UID];

                                        ModifySingleObject(Target, Param, DefaultFont, DefaultWeight);
                                    }
                                }
                            }
                        }
                    }
                }

                if (InfoPreset[3].Exists) // Textfield Parameters
                {
                    InterfaceLocalizationModifiers.StaticOrDynamic_UI_Text TranslationData = InfoPreset[3].Deserealize<InterfaceLocalizationModifiers.StaticOrDynamic_UI_Text>();

                    if (TranslationData.List.Count > 0)
                    {
                        FontFamily DefaultFont = new FontFamily();
                        FontWeight DefaultWeight = WeightFrom(TranslationData.Font_Defaults.Font_Weight);

                        if (InterfaceLocalizationModifiers.@Font_References.ContainsKey(TranslationData.Font_Defaults.Font))
                        {
                            DefaultFont = InterfaceLocalizationModifiers.@Font_References_Loaded[TranslationData.Font_Defaults.Font];
                        }
                        else if (File.Exists(TranslationData.Font_Defaults.Font)) DefaultFont = FileToFontFamily(TranslationData.Font_Defaults.Font);

                        else DefaultFont = new FontFamily(TranslationData.Font_Defaults.Font);


                        foreach (List<InterfaceTranslationParameter> LocalizationParameterSection in TranslationData.List)
                        {
                            foreach (InterfaceTranslationParameter Param in LocalizationParameterSection)
                            {
                                if (Param.UID.Keys.Count > 0)
                                {
                                    string UID = Param.UID.Keys.First();

                                    if (ᐁ_Interface_Localization_Loader.PresentedTextInputEntries.ContainsKey(UID))
                                    {
                                        ModifySingleObject(ᐁ_Interface_Localization_Loader.PresentedTextInputEntries[UID], Param, DefaultFont, DefaultWeight);
                                    }
                                }
                            }
                        }
                    }
                }

                if (InfoPreset[4].Exists) // Unsaved Changes
                {
                    SpecializedDefs.UnsavedChangesInfo = InfoPreset[4].Deserealize<InterfaceLocalizationModifiers.Frames.UnsavedChangesInfo.UnsavedChangesInfo_Main>();
                }

                if (InfoPreset[5].Exists) // Load warnings
                {
                    SpecializedDefs.CustomLangLoadingWarnings = InfoPreset[5].Deserealize<InterfaceLocalizationModifiers.Frames.CustomLangLoadingWarnings>();
                }
            }
            LanguageTranslationModifyingEvent = false;
        }
    }
}
