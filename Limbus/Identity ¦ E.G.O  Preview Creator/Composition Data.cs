using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.PreviewCreator;
using LC_Localization_Task_Absolute.UITranslationHandlers;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.PreviewCreator.AssignedComboBoxAttribute;
using static LC_Localization_Task_Absolute.PreviewCreator.CompositionData_PROP.TextColumns_PROP;
using static LC_Localization_Task_Absolute.Requirements;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute
{
    public partial class MainWindow
    {
        #region Save/Load image info
        private void OpenImageInfoButton(object RequestSender, RoutedEventArgs EventArgs)
        {
            OpenFileDialog Selector = NewOpenFileDialog("Json files", ["json"]);

            if (Selector.ShowDialog() == true)
            {
                @CurrentPreviewCreator.ImageInfoLoadingEvent = true;

                @CurrentPreviewCreator.LoadedImageInfo = new FileInfo(Selector.FileName).Deserealize<CompositionData_PROP>(Context: Path.GetDirectoryName(Selector.FileName).Replace("\\", "/"));
                ExchangeJsonValues(@CurrentPreviewCreator.LoadedImageInfo, ExchangingType.Load);

                // Add all text items again
                TextColumn_1.Children.Clear();
                TextColumn_2.Children.Clear();

                foreach (var I in @CurrentPreviewCreator.LoadedImageInfo.TextColumns.First.Items)
                {
                    @CurrentPreviewCreator.LoadedImageInfo.TextColumns.AddItemToColumn("1", I.Type, I, false);
                }
                foreach (var I in @CurrentPreviewCreator.LoadedImageInfo.TextColumns.Second.Items)
                {
                    @CurrentPreviewCreator.LoadedImageInfo.TextColumns.AddItemToColumn("2", I.Type, I, false);
                }

                @CurrentPreviewCreator.ImageInfoLoadingEvent = false;
            }
        }
        private void SaveImageInfoButton(object RequestSender, RoutedEventArgs EventArgs)
        {
            SaveFileDialog SaveLocation = NewSaveFileDialog("Json files", ["json"], "Image info.json");

            if (SaveLocation.ShowDialog() == true)
            {
                ExchangeJsonValues(@CurrentPreviewCreator.LoadedImageInfo, ExchangingType.Save);

                string Serialized = @CurrentPreviewCreator.LoadedImageInfo.SerializeToFormattedString(Context: Path.GetDirectoryName(SaveLocation.FileName).Replace("\\", "/"));
                Serialized = Serialized.RegexRemove(new(@"""__Separator\d+__"": 0\.0(,)?"));

                File.WriteAllText(SaveLocation.FileName, Serialized);
            }
        }
        

        public enum ExchangingType
        {
            Load,
            Save
        }
        
        /// <summary>
        /// Save/Load json info with <see cref="Type"/> shenanigans and property <see cref="Attribute"/> options with name links to corresponding ui elements
        /// </summary>
        public static void ExchangeJsonValues(object JsonSection, ExchangingType ProcessingMode, string OverrideDefaultColors = "")
        {
            foreach (PropertyInfo JsonProperty in JsonSection.GetType().GetProperties())
            {
                try
                {
                    if (JsonProperty.PropertyType == typeof(double))
                    {
                        var SliderTargets = JsonProperty.GetCustomAttributes<AssignedSliderAttribute>().Select(x => x.SliderName);
                        if (SliderTargets.Any())
                        {
                            foreach (string AffectedSlider in SliderTargets)
                            {
                                if (ProcessingMode == ExchangingType.Load)
                                {
                                    InterfaceObject<Slider>(AffectedSlider).Value = (double)JsonProperty.GetValue(JsonSection)!;
                                }
                                else if (ProcessingMode == ExchangingType.Save)
                                {
                                    JsonProperty.SetValue(JsonSection, InterfaceObject<Slider>(AffectedSlider).Value);
                                }
                            }
                        }
                    }

                    else if (JsonProperty.PropertyType == typeof(string))
                    {
                        var TextBoxTargets = JsonProperty.GetCustomAttributes<AssignedTextBoxAttribute>().Select(x => x.TextBoxName);
                        if (TextBoxTargets.Any())
                        {
                            foreach (string AffectedTextBox in TextBoxTargets)
                            {
                                if (ProcessingMode == ExchangingType.Load)
                                {
                                    InterfaceObject<UITranslation_Mint>(AffectedTextBox).Text = (string)JsonProperty.GetValue(JsonSection);
                                }
                                else if (ProcessingMode == ExchangingType.Save)
                                {
                                    JsonProperty.SetValue(JsonSection, InterfaceObject<UITranslation_Mint>(AffectedTextBox).Text);
                                }
                            }
                        }


                        var ComboBoxTargets = JsonProperty.GetCustomAttributes<AssignedComboBoxAttribute>();
                        if (ComboBoxTargets.Any())
                        {
                            foreach (AssignedComboBoxAttribute AffectedComboBoxProp in ComboBoxTargets)
                            {
                                string AffectedComboBox = AffectedComboBoxProp.ComboBoxName;
                                Dictionary<string, int> SelectedIndexMatcher = AffectedComboBoxProp.SelectedIndexMatcher;

                                if (ProcessingMode == ExchangingType.Load)
                                {
                                    string ComboBoxJsonValue = (string)JsonProperty.GetValue(JsonSection)!;
                                    if (SelectedIndexMatcher.ContainsKey(ComboBoxJsonValue))
                                    {
                                        InterfaceObject<ComboBox>(AffectedComboBox).SelectedIndex = SelectedIndexMatcher[ComboBoxJsonValue];
                                    }
                                    else
                                    {
                                        if (AffectedComboBox.Equals(nameof(VC_SinnerIcon)) && File.Exists(ComboBoxJsonValue)) // Path to image instead of sinner name
                                        {
                                            InterfaceObject<ComboBox>(AffectedComboBox).SelectedIndex = 12;
                                            MainControl.SinnerIcon_SelectCustom(ComboBoxJsonValue);
                                        }
                                        else InterfaceObject<ComboBox>(AffectedComboBox).SelectedIndex = -1; // If invalid
                                    }
                                }
                                else if (ProcessingMode == ExchangingType.Save)
                                {
                                    if (InterfaceObject<ComboBox>(AffectedComboBox).SelectedItem == null) JsonProperty.SetValue(JsonSection, "");
                                    // Uid = sinner name or path to image, or other type
                                    else JsonProperty.SetValue(JsonSection, (InterfaceObject<ComboBox>(AffectedComboBox).SelectedItem as UIElement).Uid.Replace("\\", "/"));
                                }
                            }
                        }


                        var FileSelectionInvokeTargets = JsonProperty.GetCustomAttributes<AssignedFileSelectionAttribute>().Select(x => x.MethodName);
                        if (FileSelectionInvokeTargets.Any())
                        {
                            string FilePathJsonValue = (string)JsonProperty.GetValue(JsonSection)!;
                            foreach (string TargetMethodToInvoke in FileSelectionInvokeTargets)
                            {
                                if (ProcessingMode == ExchangingType.Load)
                                {
                                    MainControl.GetType().GetMethod(TargetMethodToInvoke).Invoke(MainControl, [FilePathJsonValue]);
                                }
                                // "Save" is at TargetMethodToInvoke itself ('Files Selection.cs')
                            }
                        }
                    }

                    else if (JsonProperty.PropertyType == typeof(bool))
                    {
                        var OptionToggleInvokeTargets = JsonProperty.GetCustomAttributes<AssignedOptionToggleAttribute>().Select(x => x.MethodName);
                        if (OptionToggleInvokeTargets.Any())
                        {
                            bool OptionJsonValue = (bool)JsonProperty.GetValue(JsonSection)!;
                            foreach (string TargetMethodToInvoke in OptionToggleInvokeTargets)
                            {
                                if (ProcessingMode == ExchangingType.Load)
                                {
                                    MainControl.GetType().GetMethod(TargetMethodToInvoke).Invoke(MainControl, [OptionJsonValue]);
                                }
                                // "Save" is at TargetMethodToInvoke itself ('Files Selection.cs')
                            }
                        }
                    }

                    // Go to image info json subsections
                    else if (JsonProperty.HasAttribute<ImageInfoSectionAttribute>())
                    {
                        ExchangeJsonValues(JsonProperty.GetValue(JsonSection), ProcessingMode, OverrideDefaultColors);
                    }
                    
                }
                catch (Exception ex)
                {
                    rin(FormattedStackTrace(ex, $"(Identity/E.G.O Preview creator parameter {(ProcessingMode == ExchangingType.Load ? "loading" : "saving")} error {{{JsonProperty.Name} at the {JsonSection.GetType().Name}}})"));
                }
            }
        }
        #endregion


        // 'Text Control' placeholder
        private void AddItemBaseLabelToColumn_Button(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            string[] Context = (RequestSender as ComboBoxItem).Uid.Split(", ");
            (string ColumnNumber, string Type) = (Context[0], Context[1]);

            @CurrentPreviewCreator.LoadedImageInfo.TextColumns.AddItemToColumn(ColumnNumber, Type);
        }
    }
}

namespace LC_Localization_Task_Absolute.PreviewCreator
{
    public ref struct @CurrentPreviewCreator
    {
        public static bool IsActive = false;

        public static Grid FocusedColumnElement;
        public static TextItem_PROP FocusedColumnElementContext => FocusedColumnElement.DataContext as TextItem_PROP;

        public static bool ImageInfoLoadingEvent = false;
        public static bool ComboBoxItemAddEvent = false;
        public static bool FocusingOnElementEvent = false;
        public static CompositionData_PROP LoadedImageInfo = new();

        public class LoadedFiles
        {
            public static List<Type_Skills.Skill> Skills { get; set; }
            public static List<SkillsDisplayInfo.SkillConstructor> Skills_DisplayInfo { get; set; }
            public static List<Type_Passives.Passive> Passives { get; set; }
            public static List<Type_Keywords.Keyword> Keywords { get; set; }
        }
    }



    [AttributeUsage(AttributeTargets.Property)]
    public class AssignedSliderAttribute(string TargetSliderName) : Attribute { public string SliderName { get; } = TargetSliderName; }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class AssignedTextBoxAttribute(string TargetTextBox) : Attribute { public string TextBoxName { get; } = TargetTextBox; }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class AssignedFileSelectionAttribute(string ActorMethodName) : Attribute { public string MethodName { get; } = ActorMethodName; }

    [AttributeUsage(AttributeTargets.Property)]
    public class AssignedOptionToggleAttribute(string ActorMethodName) : Attribute { public string MethodName { get; } = ActorMethodName; }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class AssignedComboBoxAttribute(string TargetComboBox, RemoteIndexMatchingDictionary RemoteDictionaryKey) : Attribute
    {
        public string ComboBoxName { get; } = TargetComboBox;
        public Dictionary<string, int> SelectedIndexMatcher { get; } = RemoteDictionaries[RemoteDictionaryKey];

        // Selected item x:Uid matches for ComboBoxes when loading/saving Image Info
        public enum RemoteIndexMatchingDictionary
        {
            PortraitType,
            RarityOrRiskLevel,
            SinnerIcon,
            TextBackgroundEffectsClip
        }
        public static Dictionary<RemoteIndexMatchingDictionary, Dictionary<string, int>> RemoteDictionaries = new()
        {
            [RemoteIndexMatchingDictionary.PortraitType] = new()
            {
                ["Identity"] = 0, ["E.G.O"] = 1,
            },

            [RemoteIndexMatchingDictionary.RarityOrRiskLevel] = new()
            {
                ["ZAYIN"] = 0, ["TETH"] = 1, ["HE"] = 2, ["WAW"] = 3, ["ALEPH"] = 4,
                ["000"] = 5,   ["00"] = 6,   ["0"] = 7
            },

            [RemoteIndexMatchingDictionary.SinnerIcon] = new()
            {
                ["Yi Sang"] = 0, ["Faust"] = 1,      ["Don Quixote"] = 2, ["Ryōshū"] = 3, ["Ryoshu"] = 3,    ["Meursault"] = 4, ["Meur"] = 4,
                ["Hong Lu"] = 5, ["Heathcliff"] = 6, ["Ishmael"] = 7,     ["Rodion"] = 8, ["Sinclair"] = 9,  ["Outis"] = 10,    ["Gregor"] = 11
            },

            [RemoteIndexMatchingDictionary.TextBackgroundEffectsClip] = new()
            {
                ["Right Vignette"] = 0, ["All Vignettes"] = 1
            },
        };
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColorTextEntryAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Property)]
    public class ImageInfoSectionAttribute : Attribute;


    /// <summary>
    /// Image info json file, __Separator(\d+)__ things is used to add whitespace after serialization
    /// </summary>
    public record CompositionData_PROP
    {
        [JsonIgnore] private static MainWindow UI = MainControl;
        [JsonIgnore] private const string RelativeMarker = ":Current-Directory:";


        [JsonProperty("Credits")]
        [AssignedTextBox(nameof(UI.VC_CreditsInput))]
        public string Credits { get; set; } = $"©{DateTime.Now:yyyy}. Project Moon. All rights Reserved.";

        /* ------------------------------- */ public Double __Separator0__ { get; set; } = 0;


        [JsonProperty("Width (First step)")]
        [AssignedSlider(nameof(UI.VC_ImageWidth_FirstStep))]
        public double Width_FirstStep { get; set; } = 1084;

        [JsonProperty("Width (Second step)")]
        [AssignedSlider(nameof(UI.VC_ImageWidth_SecondStep))]
        public double Width_SecondStep { get; set; } = 1084;


        [JsonProperty("Height")]
        [AssignedSlider(nameof(UI.VC_ImageHeight))]
        public double Height { get; set; } = 742;

        /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


        [ImageInfoSection]
        [JsonProperty("Portrait")]
        public Portrait_PROP Portrait { get; set; } = new();
        public record Portrait_PROP
        {
            [OnSerializing]
            private void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
            {
                ImagePath = ImagePath.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
            }
            [OnDeserialized]
            private void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
            {
                ImagePath = ImagePath.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
            }


            [JsonProperty("Type")]
            [AssignedComboBox(nameof(UI.VC_PortraitType), RemoteIndexMatchingDictionary.PortraitType)]
            public string Type { get; set; } = "Identity";

            /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


            [JsonProperty("Image Path")]
            [AssignedFileSelection(nameof(UI.SelectPortraitImage_Action))]
            public string ImagePath { get; set; } = "";

            /* ------------------------------- */ public Double __Separator2__ { get; set; } = 0;


            [JsonProperty("Vertical Offset")]
            [AssignedSlider(nameof(UI.VC_PortraitImage_VerticalOffset))]
            public double VerticalOffset { get; set; } = 0;

            [JsonProperty("Horizontal Offset")]
            [AssignedSlider(nameof(UI.VC_PortraitImage_HorizontalOffset))]
            public double HorizontalOffset { get; set; } = 0;

            /* ------------------------------- */ public Double __Separator3__ { get; set; } = 0;


            [ImageInfoSection]
            [JsonProperty("E.G.O")]
            public EGO_PROP EGO { get; set; } = new();
            public record EGO_PROP
            {
                [JsonProperty("Inner Image Scale")]
                [AssignedSlider(nameof(UI.VC_EGOInnerPortraiScale))]
                public double InnerImageScale { get; set; } = 1;

                [JsonProperty("Whole Image Scale")]
                [AssignedSlider(nameof(UI.VC_EGOWholePortraiScale))]
                public double WholeImageScale { get; set; } = 1;

                /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


                [ColorTextEntry]
                [JsonProperty("Frame Color")]
                [AssignedTextBox(nameof(UI.VC_EGOPortrait_FrameColorInput))]
                public string FrameColor { get; set; } = "FFFFFF";
            }

            /* ------------------------------- */ public Double __Separator4__ { get; set; } = 0;


            [ImageInfoSection]
            [JsonProperty("Identity")]
            public Identity_PROP Identity { get; set; } = new();
            public record Identity_PROP
            {
                [JsonProperty("Scale")]
                [AssignedSlider(nameof(UI.VC_IdentityPortraitScale))]
                public double Scale { get; set; } = 1;
            }
        }


        [ImageInfoSection]
        [JsonProperty("Vignette")]
        public Vignette_PROP Vignette { get; set; } = new();
        public record Vignette_PROP
        {
            [ImageInfoSection]
            [JsonProperty("Softness")]
            public Softness_PROP Softness { get; set; } = new();
            public record Softness_PROP
            {
                [JsonProperty("Left, Top, Bottom")]
                [AssignedSlider(nameof(UI.VC_LeftTopBottomVignetteSoftness))]
                public double LeftTopBottom { get; set; } = 40;

                [JsonProperty("Right (Text background)")]
                [AssignedSlider(nameof(UI.VC_RightVignetteSoftness))]
                public double Right { get; set; } = 140;

                /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


                [JsonProperty("Left (Behind E.G.O Portrait)")]
                [AssignedSlider(nameof(UI.VC_LeftBehindEGOVignetteSoftness))]
                public double Left_BehindEGOPortrait { get; set; } = 0;
            }


            [ImageInfoSection]
            [JsonProperty("Plus Length")]
            public PlusLength_PROP PlusLength { get; set; } = new();
            public record PlusLength_PROP
            {
                [JsonProperty("Left")]
                [AssignedSlider(nameof(UI.VC_LeftVignettePlusLength))]
                public double Left { get; set; } = 0;

                [JsonProperty("Top")]
                [AssignedSlider(nameof(UI.VC_TopVignettePlusLength))]
                public double Top { get; set; } = 0;

                [JsonProperty("Right")]
                [AssignedSlider(nameof(UI.VC_RightVignettePlusLength))]
                public double Right { get; set; } = 600;

                [JsonProperty("Bottom")]
                [AssignedSlider(nameof(UI.VC_BottomVignettePlusLength))]
                public double Bottom { get; set; } = 0;

                /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


                [JsonProperty("Left (Behind E.G.O Portrait)")]
                [AssignedSlider(nameof(UI.VC_LeftBehindEGOVignettePlusLength))]
                public double Left_BehindEGOPortrait { get; set; } = 0;
            }
        }


        [ImageInfoSection]
        [JsonProperty("Header")]
        public Header_PROP Header { get; set; } = new();
        public record Header_PROP
        {
            [JsonProperty("Horizontal Offset")]
            [AssignedSlider(nameof(UI.VC_Header_HorizontalOffset))]
            public double HorizontalOffset { get; set; } = 0;

            [JsonProperty("Vertical Offset")]
            [AssignedSlider(nameof(UI.VC_Header_VerticalOffset))]
            public double VerticalOffset { get; set; } = 0;

            /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


            [ColorTextEntry]
            [JsonProperty("Color")]
            [AssignedTextBox(nameof(UI.VC_Header_ColorInput))]
            public string Color { get; set; } = "FFFFFF";

            /* ------------------------------- */ public Double __Separator2__ { get; set; } = 0;


            [ImageInfoSection]
            [JsonProperty("Sinner Name")]
            public SinnerName_PROP SinnerName { get; set; } = new();
            public record SinnerName_PROP
            {
                [JsonProperty("Text")]
                [AssignedTextBox(nameof(UI.VC_Header_SinnerNameInput))]
                public string Text { get; set; } = "Sinner";

                [JsonProperty("Horizontal Offset")]
                [AssignedSlider(nameof(UI.VC_Header_SinnerNameHorizontalOffset))]
                public double HorizontalOffset { get; set; } = 0;
            }

            /* ------------------------------- */ public Double __Separator3__ { get; set; } = 0;


            [ImageInfoSection]
            [JsonProperty("Identity or E.G.O Name")]
            public IdentityOrEGOName_PROP IdentityOrEGOName { get; set; } = new();
            public record IdentityOrEGOName_PROP
            {
                [JsonProperty("Text")]
                [AssignedTextBox(nameof(UI.VC_Header_IdentityOrEGONameInput))]
                public string Text { get; set; } = "Identity/E.G.O";

                [JsonProperty("Horizontal Offset")]
                [AssignedSlider(nameof(UI.VC_Header_IdentityOrEGONameHorizontalOffset))]
                public double HorizontalOffset { get; set; } = 0;
            }

            /* ------------------------------- */ public Double __Separator4__ { get; set; } = 0;


            [ImageInfoSection]
            [JsonProperty("Identity Rarity or E.G.O Risk Level")]
            public RarityOrRiskLevel_PROP RarityOrRiskLevel { get; set; } = new();
            public record RarityOrRiskLevel_PROP
            {
                [JsonProperty("Selected")]
                [AssignedComboBox(nameof(UI.VC_Header_RarityOrRiskLevel), RemoteIndexMatchingDictionary.RarityOrRiskLevel)]
                public string Selected { get; set; } = "000";

                /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


                [JsonProperty("Horizontal Offset")]
                [AssignedSlider(nameof(UI.VC_Header_RarityOrRiskLevelHorizontalOffset))]
                public double HorizontalOffset { get; set; } = 0;

                [JsonProperty("Vertical Offset")]
                [AssignedSlider(nameof(UI.VC_Header_RarityOrRiskLevelVerticalOffset))]
                public double VerticalOffset { get; set; } = 0;
            }
        }


        [ImageInfoSection]
        [JsonProperty("Sinner Icon")]
        public SinnerIcon_PROP SinnerIcon { get; set; } = new();
        public record SinnerIcon_PROP
        {
            [OnSerializing]
            private void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
            {
                SelectedImage = SelectedImage.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
            }
            [OnDeserialized]
            private void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
            {
                SelectedImage = SelectedImage.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
            }


            [JsonProperty("Selected")]
            [AssignedComboBox(nameof(UI.VC_SinnerIcon), RemoteIndexMatchingDictionary.SinnerIcon)]
            public string SelectedImage { get; set; } = "";

            [JsonProperty("Opacity")]
            [AssignedSlider(nameof(UI.VC_SinnerIconOpacity))]
            public double Brightness { get; set; } = 0.15;

            [JsonProperty("Size")]
            [AssignedSlider(nameof(UI.VC_SinnerIconSize))]
            public double Size { get; set; } = 124;
        }


        [ImageInfoSection]
        [JsonProperty("Image Type Text")]
        public ImageLabelText_PROP ImageLabelText { get; set; } = new();
        public record ImageLabelText_PROP
        {
            [OnSerializing]
            private void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
            {
                Font = Font.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
            }
            [OnDeserialized]
            private void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
            {
                Font = Font.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
            }


            [JsonProperty("Text")]
            [AssignedTextBox(nameof(UI.VC_RightCornerLabelInput))]
            public string Text { get; set; } = "IDENTITY INFO";

            [JsonProperty("Font")]
            [AssignedFileSelection(nameof(UI.SelectImageLabelFont_Action))]
            public string Font { get; set; } = "#Bebas Neue Bold";

            [JsonProperty("Size")]
            [AssignedSlider(nameof(UI.VC_LabelTextSize))]
            public double Size { get; set; } = 80;

            [JsonProperty("VerticalOffset")]
            [AssignedSlider(nameof(UI.VC_LabelVertialOffset))]
            public double VerticalOffset { get; set; } = 0;
        }


        [ImageInfoSection]
        [JsonProperty("Cautions")]
        public Cautions_PROP Cautions { get; set; } = new();
        public record Cautions_PROP
        {
            [OnSerializing]
            private void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
            {
                Font = Font.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
            }
            [OnDeserialized]
            private void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
            {
                Font = Font.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
            }


            [JsonProperty("Text")]
            [AssignedTextBox(nameof(UI.VC_CautionsText))]
            public string Text { get; set; } = "SEASON";

            [ColorTextEntry]
            [JsonProperty("Color")]
            [AssignedTextBox(nameof(UI.VC_CautionsColorInput))]
            public string Color { get; set; } = "FFFFFF";

            [JsonProperty("Bloom Radius")]
            [AssignedSlider(nameof(UI.VC_CautionsBloomRadius))]
            public double BloomRadius { get; set; } = 5;

            [JsonProperty("Opacity")]
            [AssignedSlider(nameof(UI.VC_CautionsOpacity))]
            public double Opacity { get; set; } = 0.30;

            /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


            [JsonProperty("Font")]
            [AssignedFileSelection(nameof(UI.SelectCautionsFont_Action))]
            public string Font { get; set; } = "#Bebas Neue Bold";

            [JsonProperty("Vertical Offset")]
            [AssignedSlider(nameof(UI.VC_CautionsVerticalOffset))]
            public double VerticalOffset { get; set; } = 0;

            [JsonProperty("Size")]
            [AssignedSlider(nameof(UI.VC_CautionsTextSize))]
            public double Size { get; set; } = 11;
        }


        [ImageInfoSection]
        [JsonProperty("Text Columns")]
        public TextColumns_PROP TextColumns { get; set; } = new();
        public record TextColumns_PROP
        {
            [OnSerializing]
            private void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
            {
                ItemSignaturesFont = ItemSignaturesFont.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
            }
            [OnDeserialized]
            private void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
            {
                ItemSignaturesFont = ItemSignaturesFont.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
            }


            [ImageInfoSection]
            [JsonProperty("Selected Files")]
            public SelectedFiles_PROP SelectedFiles { get; set; } = new();
            public record SelectedFiles_PROP
            {
                [OnSerializing]
                private void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
                {
                    SkillsLocalization = SkillsLocalization.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
                    SkillsDisplayInfo = SkillsDisplayInfo.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
                    PassivesLocalization = PassivesLocalization.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
                    KeywordsLocalization = KeywordsLocalization.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
                }
                [OnDeserialized]
                private void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
                {
                    SkillsLocalization = SkillsLocalization.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
                    SkillsDisplayInfo = SkillsDisplayInfo.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
                    PassivesLocalization = PassivesLocalization.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
                    KeywordsLocalization = KeywordsLocalization.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
                }


                [JsonProperty("Skills Localization")]
                [AssignedFileSelection(nameof(UI.SelectSkillsLocalization_Actor))]
                public string SkillsLocalization { get; set; } = "";


                [JsonProperty("Skills Display Info")]
                [AssignedFileSelection(nameof(UI.SelectSkillsDisplayInfo_Actor))]
                public string SkillsDisplayInfo { get; set; } = "";


                [JsonProperty("Passives Localization")]
                [AssignedFileSelection(nameof(UI.SelectPassivesLocalization_Actor))]
                public string PassivesLocalization { get; set; } = "";


                [JsonProperty("Keywords Localization")]
                [AssignedFileSelection(nameof(UI.SelectKeywordsLocalization_Actor))]
                public string KeywordsLocalization { get; set; } = "";
            }

            /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


            [JsonProperty("Keyword Containers Width")]
            [AssignedSlider(nameof(UI.VC_KeywordContainersWidth))]
            public double KeywordContainersWidth { get; set; } = 375;

            /* ------------------------------- */ public Double __Separator2__ { get; set; } = 0;


            [JsonProperty("Signatures Font")]
            [AssignedFileSelection(nameof(UI.SelectItemSignaturesFont_Action))]
            public string ItemSignaturesFont { get; set; } = "#Bebas Neue Bold";

            /* ------------------------------- */ public Double __Separator3__ { get; set; } = 0;


            [ImageInfoSection]
            [JsonProperty("First")]
            public FirstColumn_PROP First { get; set; } = new();
            public record FirstColumn_PROP
            {
                [JsonProperty("Horizontal Offset")]
                [AssignedSlider(nameof(UI.VC_FirstColumnHorizontalOffset))]
                public double HorizontalOffset { get; set; } = 0;

                [JsonProperty("Signatures Horizontal Offset")]
                [AssignedSlider(nameof(UI.VC_FirstColumnSignaturesOffset))]
                public double ItemSignatures_HorizontalOffset { get; set; } = 0;

                /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


                [JsonProperty("Items")]
                public List<TextItem_PROP> Items { get; set; } = [];
            }

            [ImageInfoSection]
            [JsonProperty("Second")]
            public SecondColumn_PROP Second { get; set; } = new();
            public record SecondColumn_PROP
            {
                [JsonProperty("Horizontal Offset")]
                [AssignedSlider(nameof(UI.VC_SecondColumnHorizontalOffset))]
                public double HorizontalOffset { get; set; } = 0;

                [JsonProperty("Signatures Horizontal Offset")]
                [AssignedSlider(nameof(UI.VC_SecondColumnSignaturesOffset))]
                public double ItemSignatures_HorizontalOffset { get; set; } = 0;

                /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


                [JsonProperty("Items")]
                public List<TextItem_PROP> Items { get; set; } = [];
            }




            // List item
            public record TextItem_PROP
            {
                [JsonIgnore] public TMProEmitter Link_Name { get; set; }
                [JsonIgnore] public TMProEmitter Link_PassiveDescription { get; set; }
                [JsonIgnore] public TMProEmitter Link_SkillMainDescription { get; set; }
                [JsonIgnore] public StackPanel Link_SkillCoinDescriptions { get; set; }
                [JsonIgnore] public TextBlock Link_ItemSignature { get; set; }
                [JsonIgnore] public Image Link_KeywordIcon { get; set; }
                [JsonIgnore] public string ColumnNumber { get; set; }


                [OnSerializing]
                private void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
                {
                    KeywordIcon_Path = KeywordIcon_Path.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
                }
                [OnDeserialized]
                private void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
                {
                    KeywordIcon_Path = KeywordIcon_Path.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
                }


                [JsonProperty("UID")]
                public string UID { get; set; }

                [JsonProperty("Type")]
                public string Type { get; set; }

                [JsonProperty("Signature")]
                public string Signature { get; set; } = "";

                /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


                [JsonProperty("Horizontal Offset")]
                public double HorizontalOffset { get; set; } = 0;

                [JsonProperty("Vertical Offset")]
                public double VerticalOffset { get; set; } = 0;

                /* ------------------------------- */ public Double __Separator2__ { get; set; } = 0;


                [JsonProperty("Selected Localization ID")]
                public string? SelectedLocalizationID { get; set; } = null;

                [JsonProperty("Selected Skill Constructor ID")]
                public string? SelectedSkillConstructorID { get; set; } = null;

                /* ------------------------------- */ public Double __Separator3__ { get; set; } = 0;


                [JsonProperty("Keyword Icon Path")]
                public string KeywordIcon_Path { get; set; } = "";

                /* ------------------------------- */ public Double __Separator4__ { get; set; } = 0;


                [JsonProperty("Max Width (Name)")]
                public double MaxWidth_Name { get; set; } = 678;

                [JsonProperty("Max Width (Passive Description)")]
                public double MaxWidth_PassiveDescription { get; set; } = 870;

                [JsonProperty("Max Width (Skill Main Description)")]
                public double MaxWidth_SkillMainDescription { get; set; } = 840;

                [JsonProperty("Max Width (Skill Coins Description)")]
                public double MaxWidth_SkillCoinsDescription { get; set; } = 790;
            }

            public void AddItemToColumn(string ColumnNumber, string Type, TextItem_PROP ItemInfo = null, bool ReEnumerateJsonColumns = true)
            {
                StackPanel Column = ColumnNumber == "1" ? MainControl.TextColumn_1 : MainControl.TextColumn_2;

                if (ItemInfo == null) ItemInfo = new TextItem_PROP() { UID = RandomUID(), Type = Type };
                ItemInfo.ColumnNumber = ColumnNumber;

                TextBlock LabelText = new TextBlock()
                {
                    Text = $"Text control\n({Type}, #{ItemInfo.UID})",
                    FontFamily = FontFromResource("UI/Fonts/", "Bebas Neue Bold"),
                    FontSize = 30,
                    Foreground = Brushes.White,
                    Opacity = 0.35,
                    TextAlignment = TextAlignment.Center,
                    Effect = new DropShadowEffect() { BlurRadius = 0, ShadowDepth = 3 },
                };
                LabelText.SetResourceReference(TextBlock.StyleProperty, "ColumnItem_PlainLabel");

                Grid ColumnItem = new Grid()
                {
                    Uid = ItemInfo.UID,
                    Margin = new Thickness(0, ItemInfo.VerticalOffset, 0, 13),
                    Children = { LabelText },
                    DataContext = ItemInfo
                };
                ColumnItem.SetResourceReference(Grid.ContextMenuProperty, "ColumnItemContextMenu");
                ColumnItem.MouseLeftButtonDown += (Sender, Args) => MainControl.FocusOnColumnElement(ColumnItem);

                Column.Children.Add(ColumnItem);

                MainControl.FocusOnColumnElement(ColumnItem);
                if (ReEnumerateJsonColumns) ReEnumerateColumnItems();
            }

            public void ReEnumerateColumnItems()
            {
                List<TextItem_PROP> CollectedFirst = [];
                foreach (Grid I in MainControl.TextColumn_1.Children) CollectedFirst.Add(I.DataContext as TextItem_PROP);
                First.Items = CollectedFirst;

                List<TextItem_PROP> CollectedSecond = [];
                foreach (Grid I in MainControl.TextColumn_2.Children) CollectedSecond.Add(I.DataContext as TextItem_PROP);
                Second.Items = CollectedSecond;
            }
        }


        [ImageInfoSection]
        [JsonProperty("Text Background Effects")]
        public TextBackgroundEffects_PROP TextBackgroundEffects { get; set; } = new();
        public record TextBackgroundEffects_PROP
        {
            [OnSerializing]
            private void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
            {
                ImagePath = ImagePath.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
            }
            [OnDeserialized]
            private void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
            {
                ImagePath = ImagePath.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
            }


            [JsonProperty("Image Path")]
            [AssignedFileSelection(nameof(UI.SelectTextBackgroundEffectsImage_Action))]
            public string ImagePath { get; set; } = "";

            [JsonProperty("Clip Mode")]
            [AssignedComboBox(nameof(UI.VC_TextBackgroundEffectsClipMode), RemoteIndexMatchingDictionary.TextBackgroundEffectsClip)]
            public string ClipMode { get; set; } = "Right Vignette";

            [JsonProperty("Opacity")]
            [AssignedSlider(nameof(UI.VC_TextBackgroundEffectsOpacity))]
            public double Opacity { get; set; } = 0.17;

            /* ------------------------------- */ public Double __Separator1__ { get; set; } = 0;


            [JsonProperty("Horizontal Offset")]
            [AssignedSlider(nameof(UI.VC_TextBackgroundEffectsHorizontalOffset))]
            public double HorizontalOffset { get; set; } = 0;

            [JsonProperty("Vertical Offset")]
            [AssignedSlider(nameof(UI.VC_TextBackgroundEffectsVerticalOffset))]
            public double VerticalOffset { get; set; } = 0;

            /* ------------------------------- */ public Double __Separator2__ { get; set; } = 0;


            [ImageInfoSection]
            [JsonProperty("Transform")]
            public Transform_PROP Transform { get; set; } = new();
            public record Transform_PROP
            {
                [JsonProperty("Scale (X)")]
                [AssignedSlider(nameof(UI.VC_TextBackgroundEffectsScaleX))]
                public double ScaleX { get; set; } = 1;

                [JsonProperty("Scale (Y)")]
                [AssignedSlider(nameof(UI.VC_TextBackgroundEffectsScaleY))]
                public double ScaleY { get; set; } = 1;

                [JsonProperty("Rotation")]
                [AssignedSlider(nameof(UI.VC_TextBackgroundEffectsAngle))]
                public double Angle { get; set; } = 0;
            }
        }


        [ImageInfoSection]
        [JsonProperty("Overlay Sketch Image")]
        public OverlaySketch_PROP OverlaySketch { get; set; } = new();
        public record OverlaySketch_PROP
        {
            [OnSerializing]
            private void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
            {
                ImagePath = ImagePath.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
            }
            [OnDeserialized]
            private void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
            {
                ImagePath = ImagePath.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
            }


            [JsonProperty("Image Path")]
            [AssignedFileSelection(nameof(UI.SelectOverlaySketchImage_Action))]
            public string ImagePath { get; set; } = "";

            [JsonProperty("Opacity")]
            [AssignedSlider(nameof(UI.VC_OverlaySketchOpacity))]
            public double Opacity { get; set; } = 0.50;

            [JsonProperty("Scale")]
            [AssignedSlider(nameof(UI.VC_OverlaySketchSize))]
            public double Scale { get; set; } = 742;
        }

        [ImageInfoSection]
        [JsonProperty("Other Effects")]
        public OtherEffects_PROP OtherEffects { get; set; } = new();
        public record OtherEffects_PROP
        {
            [OnSerializing]
            private void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
            {
                UpperLeftLogoImage = UpperLeftLogoImage.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
                BottomRightLogoImage = BottomRightLogoImage.Replace($"{FileDirectoryContext.Context}", RelativeMarker);
            }
            [OnDeserialized]
            private void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
            {
                UpperLeftLogoImage = UpperLeftLogoImage.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
                BottomRightLogoImage = BottomRightLogoImage.Replace(RelativeMarker, $"{FileDirectoryContext.Context}");
            }


            [JsonProperty("Walpurgis Night Logo Mode")]
            [AssignedOptionToggle(nameof(UI.SetWalpurgisNightMode))]
            public bool WalpurgisNightLogoMode { get; set; } = false;

            [JsonProperty("Walpurgis Night Logo Image")]
            [AssignedFileSelection(nameof(UI.SelectWalpurgisNighLogoImage_Action))]
            public string WalpurgisNightLogoImage { get; set; } = "";

            [JsonProperty("Walpurgis Night Logo Scale")]
            [AssignedSlider(nameof(UI.VC_WalpurgisLogoSize))]
            public double WalpurgisNightLogoScale { get; set; } = 1;

            /* ------------------------------- */
            public Double __Separator1__ { get; set; } = 0;


            [JsonProperty("Upper Left Logo Image")]
            [AssignedFileSelection(nameof(UI.SelectUpperLeftLogoImage_Action))]
            public string UpperLeftLogoImage { get; set; } = "";

            [JsonProperty("Upper Left Logo Scale")]
            [AssignedSlider(nameof(UI.VC_UpperLeftLogoSize))]
            public double UpperLeftLogoScale { get; set; } = 1;

            /* ------------------------------- */ public Double __Separator2__ { get; set; } = 0;


            [JsonProperty("Bottom Right Logo Image")]
            [AssignedFileSelection(nameof(UI.SelectBottomRightLogoImage_Action))]
            public string BottomRightLogoImage { get; set; } = "";

            [JsonProperty("Bottom Right Logo Scale")]
            [AssignedSlider(nameof(UI.VC_BottomRightLogoSize))]
            public double BottomRightLogoScale { get; set; } = 1;

            [JsonProperty("Bottom Right Logo Opacity")]
            [AssignedSlider(nameof(UI.VC_BottomRightLogoOpacity))]
            public double BottomRightLogoOpacity { get; set; } = 0.37;
        }
    }


    /// <summary>
    /// Several options with links to <see langword="DynamicResource"/> values ​​via properties <see langword="set"/>
    /// </summary>
    public ref struct @CompositionResources
    {
        private static ResourceDictionary T = MainControl.CompositionGrid.Resources;

        public ref struct TextColumns
        {
            public static FontFamily ItemSignaturesFont { set => T["TextColumns_ItemSignaturesFont"] = value; }
            public static double KeywordContainersWidth { set => T["TextColumns_KeywordCointainersWidth"] = value; }

            public ref struct First
            {
                public static double ItemSignatures_HorizontalOffset { set => T["TextColumns_FirstColumn_ItemSignatures_HorizontalOffset"] = value; }
            }
            public ref struct Second
            {
                public static double ItemSignatures_HorizontalOffset { set => T["TextColumns_SecondColumn_ItemSignatures_HorizontalOffset"] = value; }
            }
        }
    }
}
