using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.UITranslationHandlers;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.Json.Serialization;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.@SyntaxedTextEditor;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader.InterfaceLocalizationModifiers.Frames.StaticOrDynamic_UI_Text;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute
{
    #region Specialized UI Elements
    namespace UITranslationHandlers
    {
        /// <summary>
        /// Unifier of <see cref="UITranslation_Mint"/>, <see cref="UITranslation_Hyacinth"/> and <see cref="UITranslation_Rose"/> for method <see cref="ModifySingleObject"/>
        /// </summary>
        public interface UITranslationEntry
        {
            public string UID { get; set; }
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
        /// Unifier of <see cref="UITranslation_Mint"/> and <see cref="UITranslation_Hyacinth"/> for <see cref="PresentedTextInputEntries"/>
        /// </summary>
        public interface UITranslationTextfield : UITranslationEntry;


        /// <summary>
        /// TextBox with <see cref="UID"/> property for language loading navigation
        /// </summary>
        public partial class UITranslation_Mint : TextBox, UITranslationTextfield
        {
            /// <summary>
            /// Create binding of all properties
            /// </summary>
            public void InterhintPropertiesFrom(UITranslation_Mint AnotherTextElement)
            {
                this.BindSameProperties(
                    AnotherTextElement,
                    [
                        FontFamilyProperty, FontWeightProperty, FontSizeProperty,
                        ForegroundProperty, TextAlignmentProperty,
                        MarginProperty,     PaddingProperty,
                        WidthProperty,      HeightProperty
                    ]
                );
            }

            public static readonly DependencyProperty UIDProperty = Register<UITranslation_Mint, string>(nameof(UID), "", OnUIDChanged);

            private static void OnUIDChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
            {
                string DefinedUID = ChangeArgs.NewValue as string;

                if (!PresentedTextInputEntries.ContainsKey(DefinedUID) && !DefinedUID.IsNullOrEmpty())
                {
                    PresentedTextInputEntries[DefinedUID] = CurrentElement as UITranslation_Mint;
                }
            }

            public string UID
            {
                get => (string)GetValue(UIDProperty);
                set => SetValue(UIDProperty, value);
            }
        }

        /// <summary>
        /// <see cref="ICSharpCode.AvalonEdit.TextEditor"/> with <see cref="UID"/> property for language loading navigation
        /// </summary>
        public partial class UITranslation_Hyacinth : @SyntaxedTextEditor.SyntaxedTextEditorBase, UITranslationTextfield
        {
            #region Properties
            public static readonly DependencyProperty UIDProperty = Register<UITranslation_Hyacinth, string>(nameof(UID), "", OnUIDChanged);

            private static void OnUIDChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
            {
                string DefinedUID = ChangeArgs.NewValue as string;

                if (!PresentedTextInputEntries.ContainsKey(DefinedUID) && !DefinedUID.IsNullOrEmpty())
                {
                    PresentedTextInputEntries[DefinedUID] = CurrentElement as UITranslation_Hyacinth;
                }
            }

            public string UID
            {
                get => (string)GetValue(UIDProperty);
                set => SetValue(UIDProperty, value);
            }

            public TextAlignment TextAlignment { get; set; } // No effect, no actual TextAlignment property in AvalonEdit, satisfy UITranslationEntry
            #endregion


            public UITranslation_Hyacinth()
            {
                TextArea.SelectionBorder = new Pen();

                TextArea.TextView.LinkTextForegroundBrush = Brushes.LightBlue;
                TextArea.TextView.LinkTextUnderline = true;

                Background = Brushes.Transparent;
                BorderThickness = new Thickness(0);

                SelectionBorderThickness = SelectionBorderCornerRadius = 0;
                VerticalScrollBarVisibility = HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

                SyntaxHighlighting = new SyntaxHighlighting();

                SetResourceReference(UITranslation_Hyacinth.ForegroundProperty, "Theme:UITextfields.Foreground");
                SetResourceReference(UITranslation_Hyacinth.SelectionForegroundProperty, "Theme:UITextfields.Foreground");

                SetResourceReference(UITranslation_Hyacinth.CaretBrushProperty, "Theme:UITextfields.Spec.Caret");
                SetResourceReference(UITranslation_Hyacinth.SelectionBackgroundProperty, "Theme:UITextfields.Spec.Selection");
            }
        }

        /// <summary>
        /// TextBlock with <see cref="UID"/> property for language loading navigation (And <see cref="RichText"/>)
        /// </summary>
        public partial class UITranslation_Rose : TextBlock, UITranslationEntry
        {
            /// <summary>
            /// Create binding of all properties (Except <see cref="RichText"/>, regular TextProperty only)
            /// </summary>
            public void InterhintPropertiesFrom(UITranslation_Rose AnotherTextElement)
            {
                this.BindSameProperties(
                    AnotherTextElement,
                    [
                        FontFamilyProperty, FontWeightProperty, FontSizeProperty,
                        TextProperty,       ForegroundProperty, TextAlignmentProperty,
                        MarginProperty,     PaddingProperty,
                        WidthProperty,      HeightProperty
                    ]
                );
            }

            #region UI Translation helpers
            public void MarkWithUnsaved(object ExtraExtern = null)
            {
                if (LoadedModifiers.ContainsKey(this.UID))
                {
                    if (ExtraExtern == null) this.RichText = SpecializedDefs.UnsavedChangesMarker.Extern(LoadedModifiers[this.UID].Text);
                    else this.RichText = SpecializedDefs.UnsavedChangesMarker.Extern(LoadedModifiers[this.UID].Text.Extern(ExtraExtern));
                }
                else
                {
                    this.RichText = $"<size=78%><b>[No '{this.UID}' key in translation..]</b></size>";
                }
            }
            public void SetDefaultText(object ExtraExtern = null)
            {
                if (LoadedModifiers.ContainsKey(this.UID))
                {
                    if (ExtraExtern == null) this.RichText = LoadedModifiers[this.UID].Text;
                    else this.RichText = LoadedModifiers[this.UID].Text.Extern(ExtraExtern);
                }
                else
                {
                    this.RichText = $"<size=78%><b>[No '{this.UID}' key in translation..]</b></size>";
                }
            }
            #endregion



            #region Dependency properties
            public static readonly DependencyProperty UIDProperty = Register<UITranslation_Rose, string>(nameof(UID), "", OnUIDChanged);

            private static void OnUIDChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
            {
                string DefinedUID = ChangeArgs.NewValue as string;

                if (!PresentedStaticTextEntries.ContainsKey(DefinedUID) && !DefinedUID.IsNullOrEmpty())
                {
                    PresentedStaticTextEntries[DefinedUID] = CurrentElement as UITranslation_Rose;
                }
            }

            public string UID
            {
                get => (string)GetValue(UIDProperty);
                set => SetValue(UIDProperty, value);
            }




            public static readonly DependencyProperty PerfectVerticalAlignProperty = Register<UITranslation_Rose, bool>(nameof(PerfectVerticalAlign), false, OnPerfectVerticalAlignChanged);

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

            /// <summary>
            /// Sets LineHeight same sa FontSize, looks better for buttons
            /// </summary>
            public bool PerfectVerticalAlign
            {
                get => (bool)GetValue(PerfectVerticalAlignProperty);
                set => SetValue(PerfectVerticalAlignProperty, value);
            }




            public static readonly DependencyProperty RichTextProperty = Register<UITranslation_Rose, string>(nameof(RichText), "", OnRichTextChanged);

            private static void OnRichTextChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
            {
                UITranslation_Rose TargetElement = (UITranslation_Rose)CurrentElement;
                string InputRichText = (string)ChangeArgs.NewValue;

                Pocket_Watch_ː_Type_L.Actions.Apply(
                    Target: TargetElement,
                    RichText: InputRichText,
                    DividersMode: Pocket_Watch_ː_Type_L.@PostInfo.FullStopDividers.FullStopDividers_Regular,
                    DisableKeyworLinksCreation: true
                );
            }

            public string RichText
            {
                get => (string)GetValue(RichTextProperty);
                set => SetValue(RichTextProperty, value);
            }
            #endregion


            public UITranslation_Rose()
            {
                TextWrapping = TextWrapping.Wrap;
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            }
        }
    }
    #endregion

    
    
    public static class ᐁ_Interface_Localization_Loader
    {
        public static class InterfaceLocalizationModifiers
        {
            public static Dictionary<string, string> @Font_References = [];
            public static Dictionary<string, FontFamily> @Font_References_Loaded = [];

            public record StaticOrDynamic_UI_Text
            {
                [JsonProperty("Font Defaults")]
                public Font_Defaults Font_Defaults { get; set; } = new Frames.StaticOrDynamic_UI_Text.Font_Defaults();

                [JsonProperty("Custom Logo")]
                public string CustomLogo { get; set; } = "";

                [JsonProperty("Readme")]
                public List<string> Readme { get; set; } = [];

                [JsonProperty("List<Translation>")]
                public List<List<InterfaceTranslationParameter>> List { get; set; } = [];

                [JsonProperty("List<Parametre>")] // For `UI Textfields.json`
                private List<List<Frames.StaticOrDynamic_UI_Text.InterfaceTranslationParameter>> List_SecondName { set { List = value; } }
            }

            public static class Frames
            {
                public static class StaticOrDynamic_UI_Text
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
                        public Dictionary<string, dynamic> UID { get; set; } = []; // "UID": {"[C] * [Section Title] Decorative cautions":null},


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

                    [JsonProperty("Custom Language Context Menu Extra Replacements not found")]
                    public string ContextMenuExtraReplacementsMissing { get; set; } = "¤ Cannot find Context Menu Extra replacements \"[$]\"\n\n";

                    [JsonProperty("Warnings disabling notice")]
                    public string WarningsDisablingNotice { get; set; } = "\n(You can disable this warning in Settings at 'Internal' section)";

                    [JsonProperty("Warnings window title")]
                    public string WarningsWindowTitle { get; set; } = "Loading exceptions @ [$]";
                }

                public static class UnsavedChangesInfo
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


        public static Dictionary<string, UITranslation_Rose> PresentedStaticTextEntries = [];
        public static Dictionary<string, UITranslationTextfield> PresentedTextInputEntries = [];

        public static Dictionary<string, InterfaceTranslationParameter> LoadedModifiers = [];

        public static List<InterfaceTranslationParameter> ExportInfo = [];

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
        

        public static void ExternElement(string UID, string? VariableKey, string ExternString = "")
        {
            PresentedStaticTextEntries[UID].RichText = GetLocalizationTextFor(UID, VariableKey).Extern(ExternString);
        }

        public static class SpecializedDefs
        {
            public static string UnsavedChangesMarker { get; set; } = "[$] <size=66%><color=#fc5a03><b>(Changed)</b></color></size>";
            public static string InsertionsDefaultValue = "…";

            public static InterfaceLocalizationModifiers.Frames.UnsavedChangesInfo.UnsavedChangesInfo_Main UnsavedChangesInfo = new();
            public static InterfaceLocalizationModifiers.Frames.CustomLangLoadingWarnings CustomLangLoadingWarnings = new();
        }

        private static void ModifySingleObject(UITranslationEntry Target, InterfaceTranslationParameter Param, FontFamily DefaultFontFamily, FontWeight DefaultFontWeight)
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

            if ($"{Param.Font_Weight}" != "$Ignore-default") Target.FontWeight = DefaultFontWeight;
            if ($"{Param.Font}" != "$Ignore-default") Target.FontFamily = DefaultFontFamily;
            
            if (Param.Font_Size != null) Target.FontSize = (double)Param.Font_Size;
            if (Param.Foreground != null)
            {
                Target.Foreground = ToSolidColorBrush(Param.Foreground);
            }

            if (Param.Font_Weight != null) Target.FontWeight = WeightFrom(Param.Font_Weight);
            if (Param.Font != null && Param.Font != "$Ignore-default")
            {
                if (InterfaceLocalizationModifiers.@Font_References_Loaded.ContainsKey(Param.Font))
                {
                    // Get from loaded
                    Target.FontFamily = InterfaceLocalizationModifiers.@Font_References_Loaded[Param.Font];
                }
                else
                {
                    // Get from file or by name
                    Target.FontFamily = FileToFontFamily(Param.Font);
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
                List<FileInfo> InfoPreset = new string[] {
                    @$"{LocalizationInfoPath}\@ Font References.json",
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
                        else
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
                                    try
                                    {
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
                                    catch (Exception ex) { rin(FormattedStackTrace(ex, $"Translation key \"{UID}\" from \"Dynamic UI Text.json\" applying")); }
                                }
                            }
                        }
                    }
                }

                if (InfoPreset[1].Exists) // Static UI Text
                {
                    InterfaceLocalizationModifiers.StaticOrDynamic_UI_Text TranslationData = InfoPreset[1].Deserealize<InterfaceLocalizationModifiers.StaticOrDynamic_UI_Text>();

                    if (File.Exists(TranslationData.CustomLogo)) MainWindow.MainControl.MainMenuLogo.Source = BitmapFromFile(TranslationData.CustomLogo);
                    else
                    {
                        MainWindow.MainControl.MainMenuLogo.Source = BitmapFromResource("UI/Logo.png");
                    }

                    if (TranslationData.List.Count > 0)
                    {
                        FontFamily DefaultFont = new FontFamily();
                        FontWeight DefaultWeight = WeightFrom(TranslationData.Font_Defaults.Font_Weight);

                        if (InterfaceLocalizationModifiers.@Font_References.ContainsKey(TranslationData.Font_Defaults.Font))
                        {
                            DefaultFont = InterfaceLocalizationModifiers.@Font_References_Loaded[TranslationData.Font_Defaults.Font];
                        }
                        else
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
                                    try
                                    {
                                        if (PresentedStaticTextEntries.ContainsKey(UID))
                                        {
                                            LoadedModifiers[UID] = Param;

                                            UITranslation_Rose Target = PresentedStaticTextEntries[UID];

                                            ModifySingleObject(Target, Param, DefaultFont, DefaultWeight);
                                        }
                                        else if (UID.EndsWith("(Window Caption)"))
                                        {
                                            Application.Current.Resources[UID switch
                                            {
                                                "[Main UI] [!] * Program Title (Window Caption)" => @"Caption_MainWindow",
                                                "[Settings] [!] * Settings Title (Window Caption)" => @"Caption_SettingsWindow",
                                                "[New Object ID Input] [!] * New Object ID Input Title (Window Caption)" => @"Caption_ObjectIDInputDialog",
                                                "[Skills DI Manager] [!] * Skills Display Info manager Title (Window Caption)" => @"Caption_SkillsDisplayInfoManagerWindow",
                                                _ => "Черти в омуте"
                                            }] = Param.Text;
                                        }
                                    }
                                    catch (Exception ex) { rin(FormattedStackTrace(ex, $"Translation key \"{UID}\" from \"Static UI Text.json\" applying")); }
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
                        else
                        {
                            DefaultFont = FileToFontFamily(TranslationData.Font_Defaults.Font);
                        }


                        foreach (List<InterfaceTranslationParameter> LocalizationParameterSection in TranslationData.List)
                        {
                            foreach (InterfaceTranslationParameter Param in LocalizationParameterSection)
                            {
                                if (Param.UID.Keys.Count > 0)
                                {
                                    string UID = Param.UID.Keys.First();
                                    try
                                    {
                                        if (PresentedTextInputEntries.ContainsKey(UID))
                                        {
                                            ModifySingleObject(PresentedTextInputEntries[UID], Param, DefaultFont, DefaultWeight);
                                        }
                                    }
                                    catch (Exception ex) { rin(FormattedStackTrace(ex, $"Translation key \"{UID}\" from \"Textfield Parameters.json\" applying")); }
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
