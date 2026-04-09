using LCLocalizationInterface.Instruments;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using LCLocalizationInterface.LimbusRegistry.JsonTypes.Specific;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage.AssignedComboBoxAttribute;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage.ImageInfoJsonFile.TextColumns_PROP;

namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public partial class PreviewCreatorPage : Page
    {
        #region Save/Load image info
        private void OpenImageInfo_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            bool SelectAndOpenFile()
            {
                OpenFileDialog Selector = NewOpenFileDialog("Json files", ["json"]);
                if (Selector.ShowDialog() == true)
                {
                    OpenImageInfo_Action(Selector.FileName);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (CurrentImageInfoJson != RecentlySerializedImageInfoJson)
            {
                UnsavedChangesDialog_ShowMenu(ProceedAction: SelectAndOpenFile);
            }
            else
            {
                SelectAndOpenFile();
            }
        }
        public void OpenImageInfo_Action(string ImageInfoPath)
        {
            @CurrentPreviewCreator.IsImageInfoLoadingEvent = true;
            try
            {
                if (File.Exists(ImageInfoPath))
                {
                    string LocationContext = Path.GetDirectoryName(ImageInfoPath)!.Replace("\\", "/");
                    if (new FileInfo(ImageInfoPath).TryDeserealizeJsonAs(out ImageInfoJsonFile Deserialized, out Exception Occurred, Context: LocationContext))
                    {
                        UnsealCautions();

                        @DataContextDomain.PreviewCreator.ImageInfo = Deserialized;
                        ExchangeRemainingNonMvvmOptions(@DataContextDomain.PreviewCreator.ImageInfo, ExchangingMode.Load);
                        RecentlySerializedImageInfoJson = CurrentImageInfoJson;

                        ClearUndos();

                        ReconstructColumnItems(@DataContextDomain.PreviewCreator.ImageInfo.TextColumns.First.Items, TextColumn_1);
                        ReconstructColumnItems(@DataContextDomain.PreviewCreator.ImageInfo.TextColumns.Second.Items, TextColumn_2);

                        //CompositionScrollViewer.UpdateLayout();
                        SealCautions();
                    }
                    else
                    {
                        ErrorMessageWindow.ShowException(Occurred);

                        @DataContextDomain.PreviewCreator.ImageInfo = new();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(ImageInfoPath))
                {
                    ErrorMessageWindow.ShowException(new FileNotFoundException("File not found"), $"Specified preload Image Info file <b>\"{ImageInfoPath}\"</b> for Identity/E.G.O Preview Creator does not exists.");
                }
            }
            catch (Exception Occurred)
            {
                ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to load Image Info <b>\"{ImageInfoPath}\"</b> to Identity/E.G.O Preview Creator");
            }
            finally
            {
                @CurrentPreviewCreator.IsImageInfoLoadingEvent = false;
            }
        }
        public void ClearUndos()
        {
            foreach (IntenseStareType3 TextInput in ImageControlPanel.FindVisualChildren<IntenseStareType3>())
            {
                TextInput.Document.UndoStack.ClearAll();
            }
        }

        #region Column items reconstruction
        private void ReconstructColumnItems(ObservableCollection<ColumnTextElementData> JsonDataItems, TextElementsColumn TargetColumn)
        {
            foreach (ColumnTextElementContainer ColumnTextElementJsonData in TargetColumn.Children)
            {
                ColumnTextElementJsonData.UnsealLocalizationTextView();
            }

            TargetColumn.Children.Clear();
            foreach (ColumnTextElementData ColumnTextElementJsonData in JsonDataItems)
            {
                if (ColumnTextElementJsonData.SelectedLocalizationID is not null)
                {
                    switch (ColumnTextElementJsonData.Type)
                    {
                        case ColumnTextElementType.Skill:

                            if (BigInteger.TryParse(ColumnTextElementJsonData.SelectedLocalizationID, out BigInteger SkillID) &&
                                BigInteger.TryParse(ColumnTextElementJsonData.SelectedSkillConstructorID, out BigInteger SkillConstructorID)
                            ) {
                                if (PreviewCreatorPage.LoadedSkills.TryGetValue(SkillID, out PlainSkill.UptieLevel? Uptie) &&
                                    PreviewCreatorPage.LoadedSkillsDisplayInfo.TryGetValue(SkillConstructorID, out SkillConstructor? Constructor)
                                ) {
                                    AddTextElementToColumn(
                                        TargetColumn: TargetColumn,
                                        CreatedColumnElement: this.CreateSkill(
                                            GivenSkillText: Uptie,
                                            Displaying: Constructor,
                                            GivenJsonData: ColumnTextElementJsonData
                                        ),
                                        DoColumnsJsonDataReEnumeration: false // !!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                    );
                                }
                            }

                            break;


                        case ColumnTextElementType.Passive:

                            if (BigInteger.TryParse(ColumnTextElementJsonData.SelectedLocalizationID, out BigInteger PassiveID))
                            {
                                if (PreviewCreatorPage.LoadedPassives.TryGetValue(PassiveID, out PlainPassive? Passive))
                                {
                                    AddTextElementToColumn(
                                        TargetColumn: TargetColumn,
                                        CreatedColumnElement: this.CreatePassive(
                                            GivenPassiveText: Passive,
                                            GivenJsonData: ColumnTextElementJsonData
                                        ),
                                        DoColumnsJsonDataReEnumeration: false // !!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                    );
                                }
                            }

                            break;


                        case ColumnTextElementType.Keyword:

                            if (PreviewCreatorPage.LoadedKeywords.TryGetValue(ColumnTextElementJsonData.SelectedLocalizationID, out PlainKeyword? Keyword))
                            {
                                AddTextElementToColumn(
                                    TargetColumn: TargetColumn,
                                    CreatedColumnElement: this.CreateKeyword(
                                        GivenKeywordText: Keyword,
                                        GivenJsonData: ColumnTextElementJsonData,
                                        Icon: File.Exists(ColumnTextElementJsonData.KeywordIcon_Path)
                                            ? BitmapFromFile(ColumnTextElementJsonData.KeywordIcon_Path)
                                            : ImageDictionaries.KeywordImages[Keyword.ID!],
                                        TargetColumn: TargetColumn
                                    ),
                                    DoColumnsJsonDataReEnumeration: false // !!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                );
                            }

                            break;
                    }
                }
            }
        }
        #endregion




        private string SerializeImageInfo(string LocationContext)
        {
            ExchangeRemainingNonMvvmOptions(@DataContextDomain.PreviewCreator.ImageInfo, ExchangingMode.Save);

            return @DataContextDomain.PreviewCreator.ImageInfo
                .SerializeToFormattedJsonText(Context: LocationContext)
                .RegexRemove(@"""__Separator\d+__"": 0\.0(,)?");
        }

        public string RecentlySerializedImageInfo_Directory = "No path";
        public string CurrentImageInfoJson => SerializeImageInfo(LocationContext: RecentlySerializedImageInfo_Directory);
        public string RecentlySerializedImageInfoJson; // = CurrentImageInfoJson; from PreviewCreatorPage class constructor


        private void SaveImageInfo_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            SaveFileDialog SaveLocation = NewSaveFileDialog("Json files", ["json"], "Image info.json");

            if (SaveLocation.ShowDialog() == true)
            {
                RecentlySerializedImageInfo_Directory = Path.GetDirectoryName(SaveLocation.FileName)!.Replace("\\", "/");
                string Serialized = SerializeImageInfo(LocationContext: RecentlySerializedImageInfo_Directory);

                RecentlySerializedImageInfoJson = Serialized;

                File.WriteAllText(SaveLocation.FileName, Serialized);
            }
        }
        #endregion



        public Func<bool> UnsavedChangesDialog_ProceedAction = null!;
        public void UnsavedChangesDialog_ShowMenu(Func<bool> ProceedAction)
        {
            ImageInfoUnsavedChangesViewer.OldText = RecentlySerializedImageInfoJson;
            ImageInfoUnsavedChangesViewer.NewText = CurrentImageInfoJson;

            UnsavedChangesDialog_ProceedAction = ProceedAction;
            Panel.SetZIndex(UnsavedChangesGrid, 1);
            UnsavedChangesGrid.Opacity = 1;
            PreviewCreatorPageContentView.Effect = new BlurEffect() { Radius = 10 };
        }
        private void UnsavedChangesDialog_HideMenu()
        {
            Panel.SetZIndex(UnsavedChangesGrid, -1);
            UnsavedChangesGrid.Opacity = 0;
            PreviewCreatorPageContentView.Effect = null; // BlurEffect with Radius=0 still adds some annoying micro blur
        }

        /// <summary>Unsaved Changes Dialog - 'Confirm' click (Or click on blurred background)</summary>
        private void UnsavedChangesDialog_ButtonsClick_Proceed(object Sender, RoutedEventArgs Args)
        {
            bool ProceedResult = UnsavedChangesDialog_ProceedAction.Invoke();
            if (ProceedResult)
            {
                UnsavedChangesDialog_HideMenu();
            }
        }
        /// <summary>Unsaved Changes Dialog - 'Cancel' click (Or click on blurred background)</summary>
        private void UnsavedChangesDialog_ButtonsClick_Cancel(object Sender, RoutedEventArgs Args)
        {
            UnsavedChangesDialog_HideMenu();
        }



        [AttributeUsage(AttributeTargets.Property)]
        public class AssignedFileSelectionAttribute(string TargetMethodName) : Attribute { public string MethodName { get; } = TargetMethodName; }


        [AttributeUsage(AttributeTargets.Property)]
        public class AssignedSliderAttribute(string TargetSliderName) : Attribute { public string SliderName { get; } = TargetSliderName; }


        [AttributeUsage(AttributeTargets.Property)]
        public class AssignedComboBoxAttribute(string TargetComboBoxName, AssignedComboBoxAttribute.IndexMatchingRemoteDictionary RemoteDictionaryKey) : Attribute
        {
            public string ComboBoxName { get; } = TargetComboBoxName;
            public Dictionary<string, int> SelectedIndexMatcher { get; } = RemoteDictionaries[RemoteDictionaryKey];

            // Selected item x:Uid matches for ComboBoxes when loading/saving Image Info
            public enum IndexMatchingRemoteDictionary { PortraitType, RarityOrRiskLevel, SinnerIcon, TextBackgroundEffectsClip }
            public static Dictionary<IndexMatchingRemoteDictionary, Dictionary<string, int>> RemoteDictionaries = new()
            {
                [IndexMatchingRemoteDictionary.PortraitType] = new()
                {
                    ["Identity"] = 0, ["E.G.O"] = 1,
                },

                [IndexMatchingRemoteDictionary.RarityOrRiskLevel] = new()
                {
                    ["ZAYIN"] = 0, ["TETH"] = 1, ["HE"] = 2, ["WAW"] = 3, ["ALEPH"] = 4,
                    ["000"] = 5,   ["00"] = 6,   ["0"] = 7
                },

                [IndexMatchingRemoteDictionary.SinnerIcon] = new()
                {
                    ["Yi Sang"] = 0, ["Faust"] = 1,      ["Don Quixote"] = 2, ["Ryōshū"] = 3, ["Ryoshu"] = 3,    ["Meursault"] = 4, ["Meur"] = 4,
                    ["Hong Lu"] = 5, ["Heathcliff"] = 6, ["Ishmael"] = 7,     ["Rodion"] = 8, ["Sinclair"] = 9,  ["Outis"] = 10,    ["Gregor"] = 11
                },

                [IndexMatchingRemoteDictionary.TextBackgroundEffectsClip] = new()
                {
                    ["Right Vignette"] = 0, ["All Vignettes"] = 1
                },
            };
        }


        [AttributeUsage(AttributeTargets.Property)]
        public class ImageInfoSectionAttribute : Attribute;




        private enum ExchangingMode { Load, Save }
        private void ExchangeRemainingNonMvvmOptions(object ImageInfoJsonSection, ExchangingMode Mode)
        {
            Type SectionType = ImageInfoJsonSection.GetType();
            foreach (PropertyInfo JsonProperty in SectionType.GetProperties())
            {
                try
                {
                    // Go to subsections marked with the [ImageInfoSection] attribute
                    if (JsonProperty.HasAttribute<ImageInfoSectionAttribute>())
                    {
                        ExchangeRemainingNonMvvmOptions(JsonProperty.GetValue(ImageInfoJsonSection)!, Mode);
                    }
                    else if (JsonProperty.HasAttribute(out AssignedFileSelectionAttribute AssignedFileSelection))
                    {
                        if (Mode == ExchangingMode.Load)
                        {
                            string FilePath = (string)JsonProperty.GetValue(ImageInfoJsonSection)!;
                            string MethodName = AssignedFileSelection.MethodName;

                            typeof(PreviewCreatorPage).GetMethod(MethodName)!.Invoke(obj: PreviewCreatorPageInstance,  parameters: [FilePath]);
                        }
                    }
                    else if (JsonProperty.HasAttribute(out AssignedSliderAttribute AssignedSlider))
                    {
                        Slider SliderUIElement = PreviewCreatorPageInstance.FindTypeName<Slider>(AssignedSlider.SliderName)!;

                        if (Mode == ExchangingMode.Load)
                        {
                            SliderUIElement.Value = (double)JsonProperty.GetValue(obj: ImageInfoJsonSection)!;
                        }
                        else if (Mode == ExchangingMode.Save)
                        {
                            JsonProperty.SetValue(obj: ImageInfoJsonSection,  value: SliderUIElement.Value);
                        }
                    }
                    else if (JsonProperty.HasAttribute(out AssignedComboBoxAttribute AssignedComboBox))
                    {
                        ComboBox ComboBoxUIElement = PreviewCreatorPageInstance.FindTypeName<ComboBox>(AssignedComboBox.ComboBoxName)!;
                        Dictionary<string, int> SelectedIndexMatcher = AssignedComboBox.SelectedIndexMatcher;

                        if (Mode == ExchangingMode.Load)
                        {
                            string ComboBoxJsonValue = (string)JsonProperty.GetValue(obj: ImageInfoJsonSection)!;
                            if (SelectedIndexMatcher.TryGetValue(ComboBoxJsonValue, out int MatchedIndex))
                            {
                                ComboBoxUIElement.SelectedIndex = MatchedIndex;
                            }
                            else
                            {
                                if (ComboBoxUIElement == VC_SinnerIcon && File.Exists(ComboBoxJsonValue)) // Path to image instead of sinner name
                                {
                                    ComboBoxUIElement.SelectedIndex = 12;
                                    SelectCustomSinnerIcon(ComboBoxJsonValue);
                                }
                                else ComboBoxUIElement.SelectedIndex = -1; // If invalid
                            }
                        }
                        else if (Mode == ExchangingMode.Save)
                        {
                            string SaveValue = ComboBoxUIElement.SelectedItem is not null
                                ? (ComboBoxUIElement.SelectedItem as UIElement)!.Uid
                                : "";

                            JsonProperty.SetValue(obj: ImageInfoJsonSection,  value: SaveValue);
                        }
                    }
                }
                catch (Exception Occurred)
                {
                    ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while loading Image Info in Identity/E.G.O Preview Creator (Property \"{JsonProperty.Name}\", section \"{SectionType.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? SectionType.Name}\")");
                }
            }
        }








        public enum ColumnTextElementType { Skill, Passive, Keyword }

        /// <summary>"Image Info" json, __Separator(\d+)__ things is used to add whitespace after serialization</summary>
        public record ImageInfoJsonFile : Explicit
        {
            [JsonIgnore] public const string RelativeMarker = ":Current-Directory:";

            public static string PathStringFormat(object? Value)
            {
                string PathString = (Value as string)!;
                return Path.Exists(PathString) ? PathString.Replace("\\", "/") : PathString;
            }




            /* ------------------------------- */ public Double __Separator0__ { get; set; }


            [JsonProperty("Width (First step)"), AssignedSlider(nameof(VC_ImageWidth_FirstStep))]
            public double Width_FirstStep { get; set; } = 1084;

            [JsonProperty("Width (Second step)"), AssignedSlider(nameof(VC_ImageWidth_SecondStep))]
            public double Width_SecondStep { get; set; } = 1084;


            [JsonProperty("Height")]
            public double Height { get; set; } = 742.0;

            /* ------------------------------- */ public Double __Separator1__ { get; set; }


            [ImageInfoSection, JsonProperty("Portrait")]
            public Portrait_PROP Portrait { get; set; } = new();
            public record Portrait_PROP : Explicit
            {
                [OnSerializing]
                private void HandleRelativePaths_OnSave(StreamingContext FileLocationContext)
                {
                    ImagePath = ImagePath.Replace($"{FileLocationContext.Context}", RelativeMarker);
                }
                [OnDeserialized]
                private void HandleRelativePaths_OnRead(StreamingContext FileLocationContext)
                {
                    ImagePath = ImagePath.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                }


                [JsonProperty("Type"), AssignedComboBox(nameof(VC_PortraitType), IndexMatchingRemoteDictionary.PortraitType)]
                public string Type { get; set; } = "Identity";

                /* ------------------------------- */ public Double __Separator1__ { get; set; }


                [JsonProperty("Image Path"), AssignedFileSelection(nameof(SelectPortraitImage_Action))]
                public string ImagePath { get; set => field = PathStringFormat(value); } = "";

                /* ------------------------------- */ public Double __Separator2__ { get; set; }


                [JsonProperty("Vertical Offset")]
                public double VerticalOffset { get; set; } = 0.0;

                [JsonProperty("Horizontal Offset")]
                public double HorizontalOffset { get; set; } = 0.0;

                /* ------------------------------- */ public Double __Separator3__ { get; set; }


                [ImageInfoSection, JsonProperty("E.G.O")]
                public EGO_PROP EGO { get; set; } = new();
                public record EGO_PROP : Explicit
                {
                    [JsonProperty("Inner Image Scale")]
                    public double InnerImageScale { get; set; } = 1.0;

                    [JsonProperty("Whole Image Scale")]
                    public double WholeImageScale { get; set; } = 1.0;

                    /* ------------------------------- */ public Double __Separator1__ { get; set; }


                    [JsonProperty("Frame Color")]
                    public string FrameColor { get; set; } = "ffffff";
                }

                /* ------------------------------- */ public Double __Separator4__ { get; set; }


                [ImageInfoSection, JsonProperty("Identity")]
                public Identity_PROP Identity { get; set; } = new();
                public record Identity_PROP : Explicit
                {
                    [JsonProperty("Scale")]
                    public double Scale { get; set; } = 1.0;
                }
            }


            [ImageInfoSection, JsonProperty("Vignette")]
            public Vignette_PROP Vignette { get; set; } = new();
            public record Vignette_PROP : Explicit
            {
                [ImageInfoSection, JsonProperty("Softness")]
                public Softness_PROP Softness { get; set; } = new();
                public record Softness_PROP : Explicit
                {
                    [JsonProperty("Left, Top, Bottom")]
                    public double LeftTopBottom { get; set; } = 40.0;

                    [JsonProperty("Right (Text background)")]
                    public double Right { get; set; } = 255.0;

                    /* ------------------------------- */ public Double __Separator1__ { get; set; }


                    [JsonProperty("Left (Behind E.G.O Portrait)")]
                    public double Left_BehindEGOPortrait { get; set; } = 0.0;
                }


                [ImageInfoSection, JsonProperty("Plus Length")]
                public PlusLength_PROP PlusLength { get; set; } = new();
                public record PlusLength_PROP : Explicit
                {
                    [JsonProperty("Left")]
                    public double Left { get; set; } = 0.0;

                    [JsonProperty("Top")]
                    public double Top { get; set; } = 0.0;

                    [JsonProperty("Right"),]
                    public double Right { get; set; } = 600.0;

                    [JsonProperty("Bottom")]
                    public double Bottom { get; set; } = 0.0;

                    /* ------------------------------- */ public Double __Separator1__ { get; set; }


                    [JsonProperty("Left (Behind E.G.O Portrait)")]
                    public double Left_BehindEGOPortrait { get; set; } = 0.0;
                }
            }


            [ImageInfoSection, JsonProperty("Header")]
            public Header_PROP Header { get; set; } = new();
            public record Header_PROP : Explicit
            {
                [JsonProperty("Horizontal Offset")]
                public double HorizontalOffset { get; set; } = 0.0;

                [JsonProperty("Vertical Offset")]
                public double VerticalOffset { get; set; } = 0.0;

                /* ------------------------------- */ public Double __Separator1__ { get; set; }


                [JsonProperty("Color")]
                public string Color { get; set; } = "ffffff";

                /* ------------------------------- */ public Double __Separator2__ { get; set; }


                [ImageInfoSection, JsonProperty("Sinner Name")]
                public SinnerName_PROP SinnerName { get; set; } = new();
                public record SinnerName_PROP : Explicit
                {
                    [JsonProperty("Text")]
                    public string Text { get; set; } = "Sinner";

                    [JsonProperty("Horizontal Offset")]
                    public double HorizontalOffset { get; set; } = 0.0;
                }

                /* ------------------------------- */ public Double __Separator3__ { get; set; }


                [ImageInfoSection, JsonProperty("Identity or E.G.O Name")]
                public IdentityOrEGOName_PROP IdentityOrEGOName { get; set; } = new();
                public record IdentityOrEGOName_PROP : Explicit
                {
                    [JsonProperty("Text")]
                    public string Text { get; set; } = "Identity/E.G.O name";

                    [JsonProperty("Horizontal Offset")]
                    public double HorizontalOffset { get; set; } = 0.0;
                }

                /* ------------------------------- */ public Double __Separator4__ { get; set; }


                [ImageInfoSection, JsonProperty("Identity Rarity or E.G.O Risk Level")]
                public RarityOrRiskLevel_PROP RarityOrRiskLevel { get; set; } = new();
                public record RarityOrRiskLevel_PROP : Explicit
                {
                    [JsonProperty("Selected"), AssignedComboBox(nameof(VC_Header_RarityOrRiskLevel), IndexMatchingRemoteDictionary.RarityOrRiskLevel)]
                    public string Selected { get; set; } = "000";

                    /* ------------------------------- */ public Double __Separator1__ { get; set; }


                    [JsonProperty("Horizontal Offset")]
                    public double HorizontalOffset { get; set; } = 0.0;

                    [JsonProperty("Vertical Offset")]
                    public double VerticalOffset { get; set; } = 0.0;
                }
            }


            [ImageInfoSection, JsonProperty("Sinner Icon")]
            public SinnerIcon_PROP SinnerIcon { get; set; } = new();
            public record SinnerIcon_PROP : Explicit
            {
                [OnSerializing]
                private void HandleRelativePaths_OnSave(StreamingContext FileLocationContext)
                {
                    SelectedImage = SelectedImage.Replace($"{FileLocationContext.Context}", RelativeMarker);
                }
                [OnDeserialized]
                private void HandleRelativePaths_OnRead(StreamingContext FileLocationContext)
                {
                    SelectedImage = SelectedImage.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                }


                [JsonProperty("Selected"), AssignedComboBox(nameof(VC_SinnerIcon), IndexMatchingRemoteDictionary.SinnerIcon)]
                public string SelectedImage { get; set => field = PathStringFormat(value); } = "";

                [JsonProperty("Opacity")]
                public double Opacity { get; set; } = 0.15;

                [JsonProperty("Size")]
                public double Size { get; set; } = 124.0;
            }


            [ImageInfoSection, JsonProperty("Image Type Text")]
            public ImageLabelText_PROP ImageLabelText { get; set; } = new();
            public record ImageLabelText_PROP : Explicit
            {
                [OnSerializing]
                private void HandleRelativePaths_OnSave(StreamingContext FileLocationContext)
                {
                    Font = Font.Replace($"{FileLocationContext.Context}", RelativeMarker);
                }
                [OnDeserialized]
                private void HandleRelativePaths_OnRead(StreamingContext FileLocationContext)
                {
                    Font = Font.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                }


                [JsonProperty("Text")]
                public string Text { get; set; } = "IDENTITY INFO";

                [JsonProperty("Font"), AssignedFileSelection(nameof(SelectImageLabelFont_Action))]
                public string Font { get; set => field = PathStringFormat(value); } = "#Bebas Neue Bold";

                [JsonProperty("Size")]
                public double Size { get; set; } = 80.0;

                [JsonProperty("VerticalOffset")]
                public double VerticalOffset { get; set; } = 0.0;
            }


            [ImageInfoSection, JsonProperty("Cautions")]
            public Cautions_PROP Cautions { get; set; } = new();
            public record Cautions_PROP : Explicit
            {
                [OnSerializing]
                private void HandleRelativePaths_OnSave(StreamingContext FileLocationContext)
                {
                    Font = Font.Replace($"{FileLocationContext.Context}", RelativeMarker);
                }
                [OnDeserialized]
                private void HandleRelativePaths_OnRead(StreamingContext FileLocationContext)
                {
                    Font = Font.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                }


                [JsonProperty("Text")]
                public string Text { get; set; } = "SEASON";

                [JsonProperty("Color")]
                public string Color { get; set; } = "ffffff";

                [JsonProperty("Bloom Radius")]
                public double BloomRadius { get; set; } = 6;

                [JsonProperty("Opacity")]
                public double Opacity { get; set; } = 0.148;

                /* ------------------------------- */ public Double __Separator1__ { get; set; }


                [JsonProperty("Font"), AssignedFileSelection(nameof(SelectCautionsFont_Action))]
                public string Font { get; set => field = PathStringFormat(value); } = "#Bebas Neue Bold";

                [JsonProperty("Vertical Offset")]
                public double VerticalOffset { get; set; } = 0.0;

                [JsonProperty("Size")]
                public double Size { get; set; } = 11.0;
            }


            [ImageInfoSection, JsonProperty("Text Columns")]
            public TextColumns_PROP TextColumns { get; set; } = new();
            public record TextColumns_PROP : Explicit
            {
                [OnSerializing]
                private void HandleRelativePaths_OnSave(StreamingContext FileLocationContext)
                {
                    TextElementsSignaturesFont = TextElementsSignaturesFont.Replace($"{FileLocationContext.Context}", RelativeMarker);
                }
                [OnDeserialized]
                private void HandleRelativePaths_OnRead(StreamingContext FileLocationContext)
                {
                    TextElementsSignaturesFont = TextElementsSignaturesFont.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                }




                [ImageInfoSection, JsonProperty("Selected Files")]
                public SelectedFiles_PROP SelectedFiles { get; set; } = new();
                public record SelectedFiles_PROP : Explicit
                {
                    [OnSerializing]
                    public void HandleRelativePaths_OnSave(StreamingContext FileLocationContext)
                    {
                        SkillsLocalization = SkillsLocalization.Replace($"{FileLocationContext.Context}", RelativeMarker);
                        SkillsDisplayInfo = SkillsDisplayInfo.Replace($"{FileLocationContext.Context}", RelativeMarker);
                        PassivesLocalization = PassivesLocalization.Replace($"{FileLocationContext.Context}", RelativeMarker);
                        KeywordsLocalization = KeywordsLocalization.Replace($"{FileLocationContext.Context}", RelativeMarker);
                    }
                    [OnDeserialized]
                    public void HandleRelativePaths_OnRead(StreamingContext FileLocationContext)
                    {
                        SkillsLocalization = SkillsLocalization.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                        SkillsDisplayInfo = SkillsDisplayInfo.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                        PassivesLocalization = PassivesLocalization.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                        KeywordsLocalization = KeywordsLocalization.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                    }


                    [JsonProperty("Skills Localization"), AssignedFileSelection(nameof(SelectSkillsLocalization_Action))]
                    public string SkillsLocalization { get; set => field = PathStringFormat(value); } = "";


                    [JsonProperty("Skills Display Info"), AssignedFileSelection(nameof(SelectSkillsDisplayInfo_Action))]
                    public string SkillsDisplayInfo { get; set => field = PathStringFormat(value); } = "";


                    [JsonProperty("Passives Localization"), AssignedFileSelection(nameof(SelectPassivesLocalization_Action))]
                    public string PassivesLocalization { get; set => field = PathStringFormat(value); } = "";


                    [JsonProperty("Keywords Localization"), AssignedFileSelection(nameof(SelectKeywordsLocalization_Action))]
                    public string KeywordsLocalization { get; set => field = PathStringFormat(value); } = "";
                }

                /* ------------------------------- */ public Double __Separator1__ { get; set; }


                [JsonProperty("Keyword Containers Width")]
                public double KeywordContainersWidth { get; set; } = 375.0;

                /* ------------------------------- */ public Double __Separator2__ { get; set; }


                [JsonProperty("Signatures Font"), AssignedFileSelection(nameof(SelectTextElementsSignatureFont_Action))]
                public string TextElementsSignaturesFont { get; set => field = PathStringFormat(value); } = "#Bebas Neue Bold";

                /* ------------------------------- */ public Double __Separator3__ { get; set; }


                [ImageInfoSection, JsonProperty("First")]
                public FirstColumn_PROP First { get; set; } = new();
                public record FirstColumn_PROP : Explicit
                {
                    [JsonProperty("Horizontal Offset")]
                    public double HorizontalOffset { get; set; } = 0.0;

                    [JsonProperty("Signatures Horizontal Offset")]
                    public double ItemSignatures_HorizontalOffset { get; set; } = 0.0;

                    [JsonProperty("Keyword containers width")]
                    public double KeywordContainersWidth { get; set; } = 370.0;

                    /* ------------------------------- */ public Double __Separator1__ { get; set; }


                    [JsonProperty("Items")]
                    public ObservableCollection<ColumnTextElementData> Items { get; set; } = [];
                }

                [ImageInfoSection, JsonProperty("Second")]
                public SecondColumn_PROP Second { get; set; } = new();
                public record SecondColumn_PROP : Explicit
                {
                    [JsonProperty("Horizontal Offset")]
                    public double HorizontalOffset { get; set; } = 0.0;

                    [JsonProperty("Signatures Horizontal Offset")]
                    public double ItemSignatures_HorizontalOffset { get; set; } = 0.0;

                    [JsonProperty("Keyword containers width")]
                    public double KeywordContainersWidth { get; set; } = 370.0;

                    /* ------------------------------- */  public Double __Separator1__ { get; set; }


                    [JsonProperty("Items")]
                    public ObservableCollection<ColumnTextElementData> Items { get; set; } = [];
                }



                public record ColumnTextElementData : Explicit
                {
                    [OnSerializing]
                    private void HandleRelativePaths_OnSave(StreamingContext FileLocationContext)
                    {
                        KeywordIcon_Path = KeywordIcon_Path.Replace($"{FileLocationContext.Context}", ImageInfoJsonFile.RelativeMarker);
                    }

                    [OnDeserialized]
                    private void HandleRelativePaths_OnRead(StreamingContext FileLocationContext)
                    {
                        KeywordIcon_Path = KeywordIcon_Path.Replace(ImageInfoJsonFile.RelativeMarker, $"{FileLocationContext.Context}");
                    }


                    [JsonProperty("UID")]
                    public string UID { get; set; } = "";

                    [JsonProperty("Type")]
                    public ColumnTextElementType? Type { get; set; }

                    [JsonProperty("Signature")]
                    public string Signature { get; set; } = "";

                    /* ------------------------------- */
                    public Double __Separator1__ { get; set; }


                    [JsonProperty("Horizontal Offset")]
                    public double HorizontalOffset { get; set; } = 0.0;

                    [JsonProperty("Vertical Offset")]
                    public double VerticalOffset { get; set; } = 0.0;

                    /* ------------------------------- */
                    public Double __Separator2__ { get; set; }


                    [JsonProperty("Selected Localization ID")]
                    public string? SelectedLocalizationID { get; set; } = null;

                    [JsonProperty("Selected Skill Constructor ID")]
                    public string? SelectedSkillConstructorID { get; set; } = null;

                    /* ------------------------------- */
                    public Double __Separator3__ { get; set; }


                    [JsonProperty("Keyword Icon Path")]
                    public string KeywordIcon_Path { get; set => field = ImageInfoJsonFile.PathStringFormat(value); } = "";

                    /* ------------------------------- */
                    public Double __Separator4__ { get; set; }


                    [JsonProperty("Max Width (Name)")]
                    public double MaxWidth_Name { get; set; } = 450.0;

                    [JsonProperty("Max Width (Passive Description)")]
                    public double MaxWidth_PassiveDescription { get; set; } = 540.0;

                    [JsonProperty("Max Width (Skill Main Description)")]
                    public double MaxWidth_SkillMainDescription { get; set; } = 540.0;

                    [JsonProperty("Max Width (Skill Coins Description)")]
                    public double MaxWidth_SkillCoinsDescription { get; set; } = 490.0;
                }
            }


            [ImageInfoSection, JsonProperty("Text Background Effects")]
            public TextBackgroundEffects_PROP TextBackgroundEffects { get; set; } = new();
            public record TextBackgroundEffects_PROP : Explicit
            {
                [OnSerializing]
                private void HandleRelativePaths_OnSave(StreamingContext FileLocationContext)
                {
                    ImagePath = ImagePath.Replace($"{FileLocationContext.Context}", RelativeMarker);
                }
                [OnDeserialized]
                private void HandleRelativePaths_OnRead(StreamingContext FileLocationContext)
                {
                    ImagePath = ImagePath.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                }


                [JsonProperty("Image Path"), AssignedFileSelection(nameof(SelectTextBackgroundEffectsImage_Action))]
                public string ImagePath { get; set => field = PathStringFormat(value); } = "";

                [JsonProperty("Clip Mode"), AssignedComboBox(nameof(VC_TextBackgroundEffectsClipMode), IndexMatchingRemoteDictionary.TextBackgroundEffectsClip)]
                public string ClipMode { get; set; } = "Right Vignette";

                [JsonProperty("Opacity")]
                public double Opacity { get; set; } = 0.17;

                /* ------------------------------- */ public Double __Separator1__ { get; set; }


                [JsonProperty("Horizontal Offset")]
                public double HorizontalOffset { get; set; } = 0.0;

                [JsonProperty("Vertical Offset")]
                public double VerticalOffset { get; set; } = 0.0;

                /* ------------------------------- */ public Double __Separator2__ { get; set; }


                [ImageInfoSection, JsonProperty("Transform")]
                public Transform_PROP Transform { get; set; } = new();
                public record Transform_PROP : Explicit
                {
                    [JsonProperty("Scale (X)")]
                    public double ScaleX { get; set; } = 1.0;

                    [JsonProperty("Scale (Y)")]
                    public double ScaleY { get; set; } = 1.0;

                    [JsonProperty("Rotation")]
                    public double Angle { get; set; } = 0.0;
                }
            }


            [ImageInfoSection, JsonProperty("Overlay Sketch Image")]
            public OverlaySketch_PROP OverlaySketch { get; set; } = new();
            public record OverlaySketch_PROP : Explicit
            {
                [OnSerializing]
                private void HandleRelativePaths_OnSave(StreamingContext FileLocationContext)
                {
                    ImagePath = ImagePath.Replace($"{FileLocationContext.Context}", RelativeMarker);
                }
                [OnDeserialized]
                private void HandleRelativePaths_OnRead(StreamingContext FileLocationContext)
                {
                    ImagePath = ImagePath.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                }


                [JsonProperty("Image Path"), AssignedFileSelection(nameof(SelectOverlaySketchImage_Action))]
                public string ImagePath { get; set => field = PathStringFormat(value); } = "";

                [JsonProperty("Opacity")]
                public double Opacity { get; set; } = 0.50;

                [JsonProperty("Scale")]
                public double Scale { get; set; } = 742.0;
            }

            [ImageInfoSection, JsonProperty("Other Effects")]
            public OtherEffects_PROP OtherEffects { get; set; } = new();
            public record OtherEffects_PROP : Explicit
            {
                [OnSerializing]
                private void HandleRelativePaths_OnSave(StreamingContext FileLocationContext)
                {
                    UpperLeftLogoImage = UpperLeftLogoImage.Replace($"{FileLocationContext.Context}", RelativeMarker);
                    BottomRightLogoImage = BottomRightLogoImage.Replace($"{FileLocationContext.Context}", RelativeMarker);
                    WalpurgisNightLogoImage = WalpurgisNightLogoImage.Replace($"{FileLocationContext.Context}", RelativeMarker);
                }
                [OnDeserialized]
                private void HandleRelativePaths_OnRead(StreamingContext FileLocationContext)
                {
                    UpperLeftLogoImage = UpperLeftLogoImage.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                    BottomRightLogoImage = BottomRightLogoImage.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                    WalpurgisNightLogoImage = WalpurgisNightLogoImage.Replace(RelativeMarker, $"{FileLocationContext.Context}");
                }


                [JsonProperty("Walpurgis Night Logo Mode")]
                public bool WalpurgisNightLogoMode { get; set; } = false;

                [JsonProperty("Walpurgis Night Logo Image"), AssignedFileSelection(nameof(SelectWalpurgisNighLogoImage_Action))]
                public string WalpurgisNightLogoImage { get; set => field = PathStringFormat(value); } = "";

                [JsonProperty("Walpurgis Night Logo Scale")]
                public double WalpurgisNightLogoScale { get; set; } = 1.0;

                /* ------------------------------- */ public Double __Separator1__ { get; set; }


                [JsonProperty("Upper Left Logo Image"), AssignedFileSelection(nameof(SelectUpperLeftLogoImage_Action))]
                public string UpperLeftLogoImage { get; set => field = PathStringFormat(value); } = "";

                [JsonProperty("Upper Left Logo Scale")]
                public double UpperLeftLogoScale { get; set; } = 1.0;

                /* ------------------------------- */ public Double __Separator2__ { get; set; }


                [JsonProperty("Bottom Right Logo Image"), AssignedFileSelection(nameof(SelectBottomRightLogoImage_Action))]
                public string BottomRightLogoImage { get; set => field = PathStringFormat(value); } = "";

                [JsonProperty("Bottom Right Logo Scale")]
                public double BottomRightLogoScale { get; set; } = 1.0;

                [JsonProperty("Bottom Right Logo Opacity")]
                public double BottomRightLogoOpacity { get; set; } = 0.37;
            }
        }
    }
}