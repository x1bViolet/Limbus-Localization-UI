using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Microsoft.Win32;
using Newtonsoft.Json;
using RichText;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.InterfaceLocalizationModifiers.Frames.Static_UI_Text;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Json.FilesIntegration;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Globalization.NumberStyles;
using static System.Windows.Visibility;


namespace LC_Localization_Task_Absolute
{
    #region Custom UI Elements
    /// <summary>
    /// Parent interface for <c>&lt;UILocalization_Grocerius&gt;</c> and <c>&lt;UILocalization_Roseum&gt;</c> to unify their type in dictionary
    /// </summary>
    interface InterfaceTranslationEntry
    {
        public string Text { set; }

        public double FontSize { get; set; }
        public FontFamily FontFamily { get; set; }
        public FontWeight FontWeight { get; set; }
        public Brush Foreground { get; set; }

        public TextAlignment TextAlignment { get; set; }

        public Thickness Margin { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }

        public Visibility Visibility { get; set; }
    };

    public class UILocalization_Grocerius : RichTextBox, InterfaceTranslationEntry
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(UILocalization_Grocerius),
                new PropertyMetadata(string.Empty, SetRichTextFromProperty));

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(
                "TextAlignment",
                typeof(TextAlignment),
                typeof(UILocalization_Grocerius),
                new PropertyMetadata(System.Windows.TextAlignment.Left, SetTextAlignmentProperty));

        /// <summary>
        /// Set rich text
        /// </summary>
        public string Text
        {
            set {
                this.SetRichText(value);
            }
        }

        /// <summary>
        /// Rich text string that have been set for this RichTextBox last
        /// </summary>
        public string CurrentRichText { get; set; }

        /// <summary>
        /// Raw text extracted by the TextRange (Use <c>CurrentRichText</c> property to get actual content)
        /// </summary>
        public string RawText { get => this.GetText(); }

        private static void SetRichTextFromProperty(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            (CurrentElement as UILocalization_Grocerius).SetRichText(ChangeArgs.NewValue as string);
            (CurrentElement as UILocalization_Grocerius).CurrentRichText = ChangeArgs.NewValue as string;
        }

        public static void SetTextAlignmentProperty(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            foreach (Block TextBlock in (CurrentElement as UILocalization_Grocerius).Document.Blocks) TextBlock.TextAlignment = ChangeArgs.NewValue as dynamic;
        }

        /// <summary>
        /// For context menu in identity preview creator columns (Unable to normally get parent of ContextMenu with this RichTextBox as MenuItem Header)
        /// </summary>
        internal ItemRepresenter SpecProperty_ContextMenuParent { get; set; }

        public TextAlignment TextAlignment { get; set; }

        public string PlainText // Simple text without processing rich text (E.g. sliders with high frequency on value changes = maybe high cpu load)
        {
            set
            {
                this.Document.Blocks.Clear();
                this.Document.Blocks.Add(new Paragraph(new Run(value)));
            }
        }

        public static readonly DependencyProperty UIDProperty =
            DependencyProperty.Register(
                "UID",
                typeof(string),
                typeof(UILocalization_Grocerius),
                new PropertyMetadata(null, OnUIDChanged));

        public string UID
        {
            get => (string)GetValue(UIDProperty);
            set => SetValue(UIDProperty, value);
        }

        private static void OnUIDChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            string DefinedUID = ChangeArgs.NewValue as string;

            if (!ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries.ContainsKey(DefinedUID))
            {
                if (!DefinedUID.IsNullOrEmpty())
                {
                    ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[DefinedUID] = CurrentElement as UILocalization_Grocerius;
                    //Console.WriteLine($"Known {DefinedUID}");
                }
            }
            else
            {
                //throw new Exception("! Same UID already defined in this XAML document !");
            }
        }

        // For something
        public Dictionary<string, dynamic> UniversalDataBindings = new Dictionary<string, dynamic>();

        /// <summary>
        /// Test RichTextBox element for ui translation, for now used only in custom identity preview creator
        /// <br/>
        /// RichTextBox with Text property that leads to the RichTextBoxApplicator.SetRichText() method for applying tags (+UID for dictionary)
        /// </summary>
        public UILocalization_Grocerius()
        {
            IsReadOnly = true;
            Focusable = false;
            BorderBrush = Background = Brushes.Transparent;
            BorderThickness = new Thickness(0);
            Foreground = Brushes.Beige;
            FontSize = 13;
        }
    }

    public class UILocalization_Roseum : TextBlock, InterfaceTranslationEntry
    {
        public static readonly DependencyProperty UIDProperty =
            DependencyProperty.Register(
                "UID",
                typeof(string),
                typeof(UILocalization_Roseum),
                new PropertyMetadata(null, OnUIDChanged));

        public string UID
        {
            get => (string)GetValue(UIDProperty);
            set => SetValue(UIDProperty, value);
        }

        private static void OnUIDChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            var DefinedUID = ChangeArgs.NewValue as string;

            if (!ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries.ContainsKey(DefinedUID))
            {
                if (!DefinedUID.IsNullOrEmpty())
                {
                    ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[DefinedUID] = CurrentElement as UILocalization_Roseum;
                    //Console.WriteLine($"Known {DefinedUID}");
                }
            }
            else
            {
                //throw new Exception("! Same UID already defined in this XAML document !");
            }
        }

        // For something
        public Dictionary<string, dynamic> UniversalDataBindings = new Dictionary<string, dynamic>();

        /// <summary>
        /// Same with TextBlock (RichTextBox sometimes weird)
        /// </summary>
        public UILocalization_Roseum()
        {
            IsHitTestVisible = Focusable = false;
            Foreground = Brushes.Beige;
            FontSize = 13;
        }
    }
    #endregion

    internal abstract class InterfaceLocalizationModifiers
    {
        internal protected static Dictionary<string, string> @Font_References = new Dictionary<string, string>();
        internal protected static Dictionary<string, FontFamily> @Font_References_Loaded = new Dictionary<string, FontFamily>();

        internal protected record Static_UI_Text
        {
            [JsonProperty("Font Defaults")]
            public Frames.Static_UI_Text.Font_Defaults Font_Defaults { get; set; } = new Frames.Static_UI_Text.Font_Defaults();
            
            [JsonProperty("Readme")]
            public List<string> Readme { get; set; } = new List<string>();

            [JsonProperty("List<Translation>")]
            public List<List<Frames.Static_UI_Text.InterfaceTranslationParameter>> List { get; set; } = new List<List<Frames.Static_UI_Text.InterfaceTranslationParameter>>();
        }

        internal abstract class Frames
        {
            internal abstract class Static_UI_Text
            {
                internal protected record Font_Defaults
                {
                    [JsonProperty("Font")]
                    public string Font { get; set; } = "";

                    [JsonProperty("Font Weight")]
                    public string Font_Weight { get; set; } = "";
                }

                internal protected record InterfaceTranslationParameter
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
                    public List<double> Margin { get; set; }


                    [JsonProperty("Width")]
                    public double? Width { get; set; }

                    [JsonProperty("Height")]
                    public double? Height { get; set; }


                    [JsonProperty("Visible")]
                    public bool? Visible { get; set; }


                    [JsonIgnore]
                    public Thickness? Margin_Loaded { get; set; }

                    [JsonIgnore]
                    public Visibility? Visible_Loaded { get; set; }

                    [JsonIgnore]
                    public FontFamily? Font_Loaded { get; set; }

                    [JsonIgnore]
                    public FontWeight? Font_Weight_Loaded { get; set; }

                    [OnDeserialized]
                    void OnInit(StreamingContext context)
                    {
                        if (Margin != null && Margin.Count > 0)
                        {
                            if (Margin.Count == 1) Margin_Loaded = new Thickness(Margin[0]);
                            else if (Margin.Count == 4) Margin_Loaded = new Thickness(Margin[0], Margin[1], Margin[2], Margin[3]);
                        }

                        if (Visible != null && Visible == false)
                        {
                            Visible_Loaded = Visibility.Collapsed;
                        }

                        // Font_Loaded Font_Weight_Loaded by situation at SpecializedDefs.Transform()
                    }
                }
            }
        }
    }

    internal abstract class ᐁ_Interface_Localization_Loader
    {
        internal protected static Dictionary<string, InterfaceTranslationEntry> PresentedStaticTextEntries = new Dictionary<string, InterfaceTranslationEntry>();
        internal protected static Dictionary<string, InterfaceTranslationEntry> PresentedDynamicTextEntries = new Dictionary<string, InterfaceTranslationEntry>();

        internal protected static Dictionary<string, InterfaceTranslationParameter> LoadedModifiers = new Dictionary<string, InterfaceTranslationParameter>();

        internal protected static List<InterfaceLocalizationModifiers.Frames.Static_UI_Text.InterfaceTranslationParameter> ExportInfo = new();


        internal protected static Thickness ZeroThickness = new Thickness(0);
        internal protected static void XAMLConvertExport(UILocalization_Grocerius Item)
        {
            if (Item.UID != null)
            {
                InterfaceLocalizationModifiers.Frames.Static_UI_Text.InterfaceTranslationParameter ConvertedInfo = new()
                {
                    UID = new Dictionary<string, dynamic>() { [Item.UID] = "null" },
                    Text = Item.CurrentRichText,
                    Text_Alignment = Item.TextAlignment != TextAlignment.Left ? $"{Item.TextAlignment}" : null,
                    Font_Size = Item.FontSize != 13 ? Item.FontSize : null,
                    Font_Weight = Item.FontWeight != FontWeights.Normal ? $"{Item.FontWeight}" : null,
                    Margin = Item.Margin != ZeroThickness ? new List<double>() { Item.Margin.Left, Item.Margin.Top, Item.Margin.Right, Item.Margin.Bottom } : null,
                    Width = !double.IsNaN(Item.Width) ? Item.Width : null,
                    Height = !double.IsNaN(Item.Height) ? Item.Height : null
                };

                ExportInfo.Add(ConvertedInfo);
            }
        }

        internal protected static string ExternTextFor(string UID, string VariableKey = null)
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
       
        internal abstract class SpecializedDefs
        {
            internal protected static InterfaceTranslationParameter ColumnItemContextMenu_MoveUp = new InterfaceTranslationParameter();
            internal protected static InterfaceTranslationParameter ColumnItemContextMenu_MoveDown = new InterfaceTranslationParameter();
            internal protected static InterfaceTranslationParameter ColumnItemContextMenu_Refresh = new InterfaceTranslationParameter();
            internal protected static InterfaceTranslationParameter ColumnItemContextMenu_Remove = new InterfaceTranslationParameter();
            internal protected static InterfaceTranslationParameter ColumnItemContextMenu_Title = new InterfaceTranslationParameter();

            internal protected static InterfaceTranslationParameter AddedTextItemPlaceholder = new InterfaceTranslationParameter();
            internal protected static FontFamily DefaultFontFamilyForPlaceholder = MainWindow.MainControl.FindResource("BebasKaiUniversal") as FontFamily;
            internal protected static FontWeight DefaultFontWightForPlaceholder = FontWeights.Normal;
            internal protected static double DefaultFontSizeForPlaceholder = 25;
            internal protected static string DefaultTextForPlaceholder_Skill = "TEXT CONTROL\n(Skill, [Item #[$]])";
            internal protected static string DefaultTextForPlaceholder_Passive = "TEXT CONTROL\n(Passive, [Item #[$]])";
            internal protected static string DefaultTextForPlaceholder_Keyword = "TEXT CONTROL\n(Keyword, [Item #[$]])";

            internal protected static InterfaceTranslationParameter Transform(InterfaceTranslationParameter Parent, FontFamily DefaultFont, FontWeight DefaultWeight) 
            {
                if (Parent.Text == null)
                {
                    Parent.Text = $"<size=78%><b>[No \"{Parent.UID}\" entry in translation..]</b></size>";
                }

                if (Parent.Font_Size == null) Parent.Font_Size = 14;

                Parent.Font_Loaded = DefaultFont;
                Parent.Font_Weight_Loaded = DefaultWeight;

                if (Parent.Font != null)
                {
                    if (InterfaceLocalizationModifiers.@Font_References_Loaded.ContainsKey(Parent.Font))
                    {
                        Parent.Font_Loaded = InterfaceLocalizationModifiers.@Font_References_Loaded[Parent.Font];
                    }
                    else if (File.Exists(Parent.Font))
                    {
                        Parent.Font_Loaded = FileToFontFamily(Parent.Font);
                    }
                    else
                    {
                        Parent.Font_Loaded = new FontFamily(Parent.Font);
                    }
                }
                if (Parent.Font_Weight != null) Parent.Font_Weight_Loaded = WeightFrom(Parent.Font_Weight);
                else Parent.Font_Weight_Loaded = FontWeights.Normal;

                if (Parent.Margin != null && Parent.Margin.Count > 0)
                {
                    if (Parent.Margin.Count == 1) Parent.Margin_Loaded = new Thickness(Parent.Margin[0]);
                    else if (Parent.Margin.Count == 4) Parent.Margin_Loaded = new Thickness(Parent.Margin[0], Parent.Margin[1], Parent.Margin[2], Parent.Margin[3]);
                }
                else Parent.Margin_Loaded = new Thickness();

                if (Parent.Width == null) Parent.Width = 145;

                return Parent;
            }
        }


        internal protected static void ModifySingleObject(InterfaceTranslationEntry Target, InterfaceTranslationParameter Param, FontFamily DefaultFontFamily, FontWeight DefaultFontWeight)
        {
            Target.FontWeight = DefaultFontWeight;
            Target.FontFamily = DefaultFontFamily;

            if (Param.Font_Size != null) Target.FontSize = (double)Param.Font_Size;
            if (Param.Foreground != null)
            {
                Target.Foreground = ToSolidColorBrush(Param.Foreground);
            }

            if (Param.Font_Weight != null) Target.FontWeight = WeightFrom(Param.Font_Weight);
            if (Param.Font != null)
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

            if (Param.Margin_Loaded != null) Target.Margin = (Thickness)Param.Margin_Loaded;
            if (Param.Visible_Loaded != null) Target.Visibility = (Visibility)Param.Visible_Loaded;

            if (Param.Width != null) Target.Width = (double)Param.Width;
            if (Param.Height != null) Target.Width = (double)Param.Height;


            if (Param.Variable != null && Param.Variable.Keys.Count > 0)
            {
                Target.Text = Param.Variable.First().Value;
            }
            else if (!Param.Text.IsNullOrEmpty())
            {
                Target.Text = Param.Text;
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

                if (Target is UILocalization_Grocerius Grocerius)
                {
                    foreach (Block TextBlock in (Target as UILocalization_Grocerius).Document.Blocks) // RichTextBox moment
                    {
                        TextBlock.TextAlignment = TextAlignment.Center;
                    }
                }
                else Target.TextAlignment = TargetTextAlignment;
            }
        }

        internal protected static void ModifyUI(string LocalizationInfoPath)
        {
            if (Directory.Exists(LocalizationInfoPath))
            {
                List<FileInfo> InfoPreset = new List<string>()
                {   @$"{LocalizationInfoPath}\@ Font References.json",
                    @$"{LocalizationInfoPath}\Static UI Text.json",
                    @$"{LocalizationInfoPath}\Dynamic UI Text.json",
                    @$"{LocalizationInfoPath}\Secondary.json",
                    @$"{LocalizationInfoPath}\Textfield Parameters.json",
                    @$"{LocalizationInfoPath}\Unsaved Changes Info.json",
                }.GetFileInfos();

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

                if (InfoPreset[1].Exists) // Static UI Text
                {
                    InterfaceLocalizationModifiers.Static_UI_Text TranslationData = InfoPreset[1].Deserealize<InterfaceLocalizationModifiers.Static_UI_Text>();

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
                    
                        foreach (var LocalizationParameterSection in TranslationData.List)
                        {
                            foreach (var Param in LocalizationParameterSection)
                            {
                                if (Param.UID.Keys.Count > 0) // "UID": {"[C] * Title bar button tooltip":null}
                                {
                                    string UID = Param.UID.Keys.First();

                                    if (ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries.ContainsKey(UID))
                                    {
                                        LoadedModifiers[UID] = Param;

                                        InterfaceTranslationEntry Target = ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[UID];

                                        ModifySingleObject(Target, Param, DefaultFont, DefaultWeight);
                                    }
                                }
                            }
                        }
                    }
                }

                if (InfoPreset[2].Exists) // Dynamic UI Text
                {
                    InterfaceLocalizationModifiers.Static_UI_Text TranslationData = InfoPreset[2].Deserealize<InterfaceLocalizationModifiers.Static_UI_Text>();

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

                        foreach (var LocalizationParameterSection in TranslationData.List)
                        {
                            foreach (var Param in LocalizationParameterSection)
                            {
                                if (Param.UID.Keys.Count > 0) // "UID": {"[C] * Title bar button tooltip":null}
                                {
                                    string UID = Param.UID.Keys.First();

                                    LoadedModifiers[UID] = Param;

                                    switch (UID) // Unique cases
                                    {
                                        case "[C] [!] [^] * Sinner icon selection (Sinner name)":
                                            int Counter = 1;
                                            foreach (var Item in MainWindow.MainControl.IdentityPreviewCreator_SinnerIconSelector.Items)
                                            {
                                                if (Counter <= 12) // №13 = '[Custom]'
                                                {
                                                    UILocalization_Roseum Text = (Item as StackPanel).Children[1] as UILocalization_Roseum;

                                                    ModifySingleObject(Text, Param, DefaultFont, DefaultWeight);
                                                }
                                                Counter++;
                                            }
                                            break;

                                        case "[C] * Added item context menu — Title": SpecializedDefs.ColumnItemContextMenu_Title = SpecializedDefs.Transform(Param, DefaultFont, DefaultWeight);
                                            break;

                                        case "[C] * Added item context menu — Move up": SpecializedDefs.ColumnItemContextMenu_MoveUp = SpecializedDefs.Transform(Param, DefaultFont, DefaultWeight);
                                            break;

                                        case "[C] * Added item context menu — Refresh text": SpecializedDefs.ColumnItemContextMenu_Refresh = SpecializedDefs.Transform(Param, DefaultFont, DefaultWeight);
                                            break;

                                        case "[C] * Added item context menu — Move down": SpecializedDefs.ColumnItemContextMenu_MoveDown = SpecializedDefs.Transform(Param, DefaultFont, DefaultWeight);
                                            break;

                                        case "[C] * Added item context menu — Remove": SpecializedDefs.ColumnItemContextMenu_Remove = SpecializedDefs.Transform(Param, DefaultFont, DefaultWeight);
                                            break;
                                        
                                        case "[C] [!] * Added item placeholder text":
                                            if (Param.Variable != null && Param.Variable.Count == 3 && Param.Variable.ContainsKey("Skill") && Param.Variable.ContainsKey("Passive") && Param.Variable.ContainsKey("Keyword"))
                                            {
                                                SpecializedDefs.DefaultTextForPlaceholder_Skill = Param.Variable["Skill"];
                                                SpecializedDefs.DefaultTextForPlaceholder_Passive = Param.Variable["Passive"];
                                                SpecializedDefs.DefaultTextForPlaceholder_Keyword = Param.Variable["Keyword"];

                                                if (Param.Font_Size != null) SpecializedDefs.DefaultFontSizeForPlaceholder = (double)Param.Font_Size;
                                                if (Param.Font_Weight != null) SpecializedDefs.DefaultFontWightForPlaceholder = WeightFrom(Param.Font_Weight);
                                                if (Param.Font != null)
                                                {
                                                    if (InterfaceLocalizationModifiers.@Font_References_Loaded.ContainsKey(Param.Font))
                                                    {
                                                        SpecializedDefs.DefaultFontFamilyForPlaceholder = InterfaceLocalizationModifiers.@Font_References_Loaded[Param.Font];
                                                    }
                                                    else if (File.Exists(Param.Font)) SpecializedDefs.DefaultFontFamilyForPlaceholder = FileToFontFamily(Param.Font);
                                                    else SpecializedDefs.DefaultFontFamilyForPlaceholder = new FontFamily(Param.Font);
                                                }
                                            }
                                            break;

                                        default: break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal protected static void ExportConverted()
        {
            string Output = JsonConvert.SerializeObject(
                value: ExportInfo,
                formatting: Formatting.Indented,
                settings: new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
            ).Replace("\r", "");

            Output = Regex.Replace(Output, @"""UID"": {\n      ""(.*?)"": ""null""\n    },", Match =>
            {   return $"\"UID\": {{\"{Match.Groups[1].Value}\":null}},";
            }).Replace(".0", "");

            Output = Regex.Replace(Output, @"""Margin"": \[(.*?)\]", Match =>
            {   string Values = Match.Groups[1].Value.Replace("\n", "").Replace(" ", "").Replace(",", ", ");
                return $"\"Margin\": [{Values}]";
            }, RegexOptions.Singleline);

            File.WriteAllText(@"C:\Users\javas\OneDrive\Документы\LC Localization Interface (Code)\bin\Debug\net8.0-windows\⇲ Assets Directory\[+] Languages\1.json", Output);
        }
    }
}
