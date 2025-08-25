using RichText;
using SixLabors.ImageSharp.PixelFormats;

using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Numerics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static LC_Localization_Task_Absolute.Json.BaseTypes;
using static LC_Localization_Task_Absolute.Json.Custom_Skills_Constructor;

using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator;
using System.Runtime.Serialization;



namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    #region Extensions
    /// <summary>
    /// <c>Grid</c> with <c>&lt;AddedTextItems_Single&gt;</c> property for navigation within custom identity preview creator columns (+extra links for easier actions)
    /// </summary>
    internal class ItemRepresenter : Grid
    {
        /// <summary>
        /// Direct link to item info from project file json data
        /// </summary>
        internal CustomIdentityPreviewCreator.ProjectFile.Sections.AddedTextItems_Single ItemInfo { get; set; }

        internal string ColumnNumber = "1";

        internal int SelectedLocalizationItemIndex { get; set; } = -1;
        internal int SelectedSkillDisplayInfoConstructorIndex { get; set; } = -1;
        internal Image KeywordIcon { get; set; }

        // Store links to generated text items
        internal TextBlock ItemSignaruteTextBlockLink { get; set; } = null;
        internal TextBlock ItemNameLink { get; set; } = null;
        internal RichTextBox PassiveDescriptionLink { get; set; } = null;
        internal RichTextBox SkillMainDescriptionLink { get; set; } = null;
        internal StackPanel SkillCoinDescriptionsLink { get; set; } = null;
    }
    #endregion


    internal abstract class CustomIdentityPreviewCreator
    {
        internal protected static double SharedParagraphLineHeigh = 27;

        internal abstract class ProjectFile
        {
            // Used to specify a relative path to files located in the same(or sub) folder as the project file through StreamingContext string in OnSerializing or OnDeserialized events
            internal protected static string ProjectRelativeMarker = ":Project:";

            internal protected static CustomIdentityPreviewProject LoadedProject;

            internal protected class CustomIdentityPreviewProject
            {
                public bool? ActualProject { get; set; }

                public Sections.ImageParameters ImageParameters { get; set; } = new Sections.ImageParameters();

                public Sections.Specific Specific { get; set; } = new Sections.Specific();

                public Sections.DecorativeCautions DecorativeCautions { get; set; } = new Sections.DecorativeCautions();

                public Sections.AddedTextItems Text { get; set; } = new Sections.AddedTextItems();
            }

            internal abstract class Sections
            {
                internal protected class ImageParameters
                {
                    public string Type { get; set; } = "Identity"; // or E.G.O
                    public string ImageTypeSign { get; set; } = "IDENTITY INFO";
                    public double ImageTypeVerticalOffset { get; set; } = 0;
                    public double ImageTypeTextSize { get; set; } = 77;
                    public string ImageTypeSign_AnotherFont { get; set; } = ""; // PATH

                    public double WidthAdjustment_FirstStep { get; set; } = 1084;
                    public double WidthAdjustment_SecondStep { get; set; } = 1084;

                    public double HeightAdjustment { get; set; } = 500;
                    public bool HeightAdjustment_IsAuto { get; set; } = true;

                    public string PortraitImage { get; set; } = ""; // PATH
                    public double AllocatedWidthForPortrait { get; set; } = 450;

                    public double EGOPortraitScale { get; set; } = 100;
                      public double EGOPortraitHorizontalOffset { get; set; } = 0;
                      public double EGOPortraitVerticalOffset { get; set; } = 0;

                    public double IdentityPortraitScale { get; set; } = 742;
                      public double PortraitHorizontalOffset { get; set; } = 0;
                      public double PortraitVerticalOffset { get; set; } = 0;

                    public double HeaderOffset { get; set; } = 0;
                      public double IdentityOrEGONameOffset { get; set; } = 0;
                      public double SinnerNameOffset { get; set; } = 0;
                      public double RarityOrEGORiskLevelHorizontalOffset { get; set; } = 0;
                      public double RarityOrEGORiskLevelVerticalOffset { get; set; } = 0;

                    public double TextBackgroundFadeoutSoftness { get; set; } = 200;
                    public double VignetteStrength { get; set; } = 35;
                      public double TopVignetteOffset { get; set; } = 0;
                      public double LeftVignetteOffset { get; set; } = 0;
                      public double BottomVignetteOffset { get; set; } = 0;

                    [OnSerializing]
                    void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
                    {
                        ImageTypeSign_AnotherFont = ImageTypeSign_AnotherFont.Replace($"{FileDirectoryContext.Context}", ProjectRelativeMarker);
                        PortraitImage = PortraitImage.Replace($"{FileDirectoryContext.Context}", ProjectRelativeMarker);
                    }

                    [OnDeserialized]
                    void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
                    {
                        ImageTypeSign_AnotherFont = ImageTypeSign_AnotherFont.Replace(ProjectRelativeMarker, $"{FileDirectoryContext.Context}");
                        PortraitImage = PortraitImage.Replace(ProjectRelativeMarker, $"{FileDirectoryContext.Context}");
                    }
                }

                internal protected class Specific
                {
                    public string? SinnerIcon { get; set; } // PATH (?)

                    public double IconBrightness { get; set; } = 15;
                    public double IconSize { get; set; } = 130;

                    public string? SinnerName { get; set; }
                    public string? IdentityOrEGOName { get; set; }

                    public string AmbienceColor { get; set; } = "#abcdef";

                    public string? RarityOrEGORiskLevel { get; set; }

                    [OnSerializing]
                    void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
                    {
                        if (SinnerIcon != null && File.Exists(SinnerIcon))
                        {
                            SinnerIcon = SinnerIcon.Replace($"{FileDirectoryContext.Context}", ProjectRelativeMarker);
                        }
                    }

                    [OnDeserialized]
                    void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
                    {
                        if (SinnerIcon != null)
                        {
                            SinnerIcon = SinnerIcon.Replace(ProjectRelativeMarker, $"{FileDirectoryContext.Context}");
                        }
                    }
                }

                internal protected class DecorativeCautions
                {
                    public string CautionType { get; set; } = "SEASON";
                    public double CautionBloomRadius { get; set; } = 5;
                    public double CautionOpacity { get; set; } = 35;
                    public DecorativeCautions_CustomText CustomText { get; set; } = new DecorativeCautions_CustomText();
                }
                internal protected class DecorativeCautions_CustomText
                {
                    public string CustomCautionString { get; set; } = "";
                    public string AnotherFont { get; set; } = ""; // PATH
                    public double TextVerticalOffset { get; set; } = -2.55;
                    public double TextSize { get; set; } = 10.6;

                    [OnSerializing]
                    void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
                    {
                        AnotherFont = AnotherFont.Replace($"{FileDirectoryContext.Context}", ProjectRelativeMarker);
                    }

                    [OnDeserialized]
                    void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
                    {
                        AnotherFont = AnotherFont.Replace(ProjectRelativeMarker, $"{FileDirectoryContext.Context}");
                    }
                }

                internal protected class AddedTextItems
                {
                    public string SkillsLocalizationFile { get; set; } = ""; // PATH
                    public string SkillsDisplayInfoConstructorFile { get; set; } = ""; // PATH
                    public string PassivesLocalizationFile { get; set; } = ""; // PATH
                    public string KeywordsLocalizationFile { get; set; } = ""; // PATH

                    public string ItemSignaturesAnotherFont { get; set; } = ""; // PATH
                    public double KeywordBoxesWidth { get; set; } = 315;
                    public double UnifiedTextSize { get; set; } = 22;

                    public double FirstColumnOffset { get; set; } = 0;
                    public double FirstColumnItemSignaturesOffset { get; set; } = 0;
                    public Dictionary<string, AddedTextItems_Single> FirstColumnItems { get; set; } = new Dictionary<string, AddedTextItems_Single>(); // UID as key

                    public double SecondColumnOffset { get; set; } = 0;
                    public double SecondColumnItemSignaturesOffset { get; set; } = 0;
                    public Dictionary<string, AddedTextItems_Single> SecondColumnItems { get; set; } = new Dictionary<string, AddedTextItems_Single>();

                    [OnSerializing]
                    void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
                    {
                        SkillsLocalizationFile = SkillsLocalizationFile.Replace($"{FileDirectoryContext.Context}", ProjectRelativeMarker);
                        SkillsDisplayInfoConstructorFile = SkillsDisplayInfoConstructorFile.Replace($"{FileDirectoryContext.Context}", ProjectRelativeMarker);
                        PassivesLocalizationFile = PassivesLocalizationFile.Replace($"{FileDirectoryContext.Context}", ProjectRelativeMarker);
                        KeywordsLocalizationFile = KeywordsLocalizationFile.Replace($"{FileDirectoryContext.Context}", ProjectRelativeMarker);
                        ItemSignaturesAnotherFont = ItemSignaturesAnotherFont.Replace($"{FileDirectoryContext.Context}", ProjectRelativeMarker);
                    }

                    [OnDeserialized]
                    void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
                    {
                        SkillsLocalizationFile = SkillsLocalizationFile.Replace(ProjectRelativeMarker, $"{FileDirectoryContext.Context}");
                        SkillsDisplayInfoConstructorFile = SkillsDisplayInfoConstructorFile.Replace(ProjectRelativeMarker, $"{FileDirectoryContext.Context}");
                        PassivesLocalizationFile = PassivesLocalizationFile.Replace(ProjectRelativeMarker, $"{FileDirectoryContext.Context}");
                        KeywordsLocalizationFile = KeywordsLocalizationFile.Replace(ProjectRelativeMarker, $"{FileDirectoryContext.Context}");
                        ItemSignaturesAnotherFont = ItemSignaturesAnotherFont.Replace(ProjectRelativeMarker, $"{FileDirectoryContext.Context}");
                    }
                }
                internal protected class AddedTextItems_Single
                {
                    public string? Type { get; set; }
                    public string TextItemSignature { get; set; } = "";

                    public double VerticalOffset { get; set; } = 0;
                    public double HorizontalOffset { get; set; } = 0;

                    public dynamic? SelectedTextIDFromLocalizationFile { get; set; } // int(skill/passive) or string(keyword)
                    public BigInteger? SelectedSkillConstructorFromDisplayInfoFile { get; set; }

                    public string KeywordIconImage { get; set; } = ""; // PATH

                    public double NameMaxWidth { get; set; } = 640;
                    public double PassiveDescriptionWidth { get; set; } = 739;

                    public double SkillMainDescriptionWidth { get; set; } = 790;
                    public double SkillCoinsDescriptionWidth { get; set; } = 750;

                    [OnSerializing]
                    void HandleRelativePaths_OnSave(StreamingContext FileDirectoryContext)
                    {
                        KeywordIconImage = KeywordIconImage.Replace($"{FileDirectoryContext.Context}", ProjectRelativeMarker);
                    }

                    [OnDeserialized]
                    void HandleRelativePaths_OnLoad(StreamingContext FileDirectoryContext)
                    {
                        KeywordIconImage = KeywordIconImage.Replace(ProjectRelativeMarker, $"{FileDirectoryContext.Context}");
                    }
                }
            }
        }





        internal protected static bool IsActive = true;

        internal protected static BitmapImage SinnerNameBackground = ImageFromResource($"UI/Limbus/Custom Identity Preview/Name Background (Sinner).png");
        internal protected static BitmapImage IdentityNameBackground = ImageFromResource($"UI/Limbus/Custom Identity Preview/Name Background (Identity).png");

        internal protected static BitmapImage EGOPlainFrame = ImageFromResource($"UI/Limbus/Custom Identity Preview/E.G.O Frame.png");

        internal static protected BitmapImage CreateColoredHeader(string HexColor, string ImageType = "Sinner")
        {
            BitmapImage Selected = ImageType.Equals("Sinner") ? SinnerNameBackground : IdentityNameBackground;

            SixLabors.ImageSharp.Image<Rgba32> Colored = SecondaryUtilities.TintWhiteMaskImage(Selected.ToSixlaborsImage(), Rgba32.ParseHex(HexColor.Replace("#", "")));

            return Colored.ToBitmapImage();
        }

        internal protected static SolidColorBrush GetAffinityColor(string Affinity)
        {
            return ToSolidColorBrush(Affinity switch
            {
                   "Wrath" => "#fe0101",
                    "Lust" => "#fe6f01",
                   "Sloth" => "#edc427",
                "Gluttony" => "#85ce04",
                   "Gloom" => "#1cc7f1",
                   "Pride" => "#014fd6",
                    "Envy" => "#9800df",
                _ => "#9f6a3a"
            });
        }
        internal protected static SolidColorBrush GetAffinityColor_DarkerVersion(string Affinity)
        {
            return ToSolidColorBrush(Affinity switch
            {
                   "Wrath" => "#ff0000",
                    "Lust" => "#cc5200",
                   "Sloth" => "#c9a022",
                "Gluttony" => "#85ce04",
                   "Gloom" => "#119cbe",
                   "Pride" => "#004bd5",
                    "Envy" => "#9500de",
                _ => "#9f6a3a"
            });
        }

        internal protected static Dictionary<string, string> SinnerColors = new Dictionary<string, string>()
        {
                ["Yi Sang"] = "#d4e1e8",
                  ["Faust"] = "#ffb1b4",
            ["Don Quixote"] = "#ffef23",
                 ["Ryōshū"] = "#cf0000",
                 ["Ryoshu"] = "#cf0000",
              ["Meursault"] = "#293b95",
                ["Hong Lu"] = "#5bffde",
             ["Heathcliff"] = "#4e3076",
                ["Ishmael"] = "#ff9500",
                 ["Rodion"] = "#820000",
               ["Sinclair"] = "#8b9c15",
                  ["Outis"] = "#325339",
                 ["Gregor"] = "#69350b",
        };

        internal protected static SolidColorBrush GetSinnerColor(string SinnerName)
        {
            return ToSolidColorBrush(SinnerColors[SinnerName]);
        }

        internal protected class SkillInfoFormationParameters
        {
            public double PanelWidth { get; set; } = 663;
            public double MainDescWidth { get; set; } = 663;
            public double CoinDescsWidth { get; set; } = double.NaN;
        }



        /// <summary>
        /// Generates skill or passive name with affinity-colored background
        /// </summary>
        internal protected static Grid GetNameWithBackground(string Name, string AffinityName = "None", bool ReversedDropdownShadow = false)
        {
            /*/
             * 
             * Grid with TextBlock for the skill name + horizontal StackPanel with actual background color and image on the right side
             * First child of StackPanel (Affinity color filler) is being resized by actual width binding to the TextBlock
             * Second child (Image with pattern) stays fixed horizontally, resizes only vertically when line break appears
             * 
            /*/

            TextBlock NameTextBlock = new TextBlock()
            {
                Text = Name,
                Margin = new Thickness(10, 11, 0, 4.4),
                MaxWidth = FocusedColumnItem.ItemInfo.NameMaxWidth,
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = ToSolidColorBrush("#EBCAA2"),
                FontSize = 27,
                TextWrapping = TextWrapping.Wrap,
                LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                LineHeight = 25,
                Effect = new DropShadowEffect() { ShadowDepth = 3, BlurRadius = 0, Direction = -50 },
            }.SetBindingWithReturn(TextBlock.FontFamilyProperty, "FontFamily", MainControl.NavigationPanel_ObjectName_Display) as TextBlock;

            /* Special */
            if (CustomIdentityPreviewCreator.IsActive)
            {
                MainWindow.FocusedColumnItem.ItemNameLink = NameTextBlock;
            }

            Grid Name_MainGrid = new Grid()
            {
                Effect = new DropShadowEffect() { ShadowDepth = 4, BlurRadius = 0, Direction = ReversedDropdownShadow ? -40 : 230 },
                Children =
                {
                    new StackPanel()
                    {
                        Orientation = Orientation.Horizontal, //--//
                        Margin = new Thickness(0, 0, 110, 0),
                        ClipToBounds = true,
                        Children =
                        {
                            new Border() // Affinity color filler
                            {
                                MinWidth = 110,
                                Background = GetAffinityColor_DarkerVersion(AffinityName),
                                Margin = new Thickness(-50, 0, 0 ,0)
                            }.SetBindingWithReturn(FrameworkElement.WidthProperty, "ActualWidth", NameTextBlock), // Dynamic resize by name
                            new Image() // Image on the right side
                            {
                                Source = ImageFromResource($"UI/Limbus/Skills/Name Background/Cut/{AffinityName}.png"),
                                Margin = new Thickness(-16, 0, 0, 0),
                                Clip = new RectangleGeometry(new Rect(15, 0, 1000, 2000)),

                                HorizontalAlignment = HorizontalAlignment.Left,
                                Stretch = Stretch.Fill,

                                // OR Right HorizontalAlignment and UniformToFill Stretch -> no vertical resize and fixed scale,
                                // but image left side cutout at minimal width (Can be fixed by dynamic margin based in scale (idc previous option better))
                                //HorizontalAlignment = HorizontalAlignment.Right,
                                //Stretch = Stretch.UniformToFill,
                            },
                        }
                    },
                    NameTextBlock
                }
            };

            return Name_MainGrid;
        }
        internal protected static TextBlock GenerateItemSignature(string TargetColumn)
        {
            TextBlock Output = new TextBlock()
            {
                Foreground = ToSolidColorBrush("#47311e"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 35,
                Margin = new Thickness(0, 40, 0, 0),
                TextAlignment = TextAlignment.Right,
                Text = FocusedColumnItem.ItemInfo.TextItemSignature,
                Effect = new DropShadowEffect() { ShadowDepth = 0, BlurRadius = 7 }
            }.SetBindingWithReturn(TextBlock.FontFamilyProperty, "FontFamily", MainControl.ItemSignaturesFontBinder).SetBindingWithReturn(FrameworkElement.RenderTransformProperty, "RenderTransform", TargetColumn.Equals("1") ? MainControl.FirstColumn_ItemsSignaturesOffsetBinder : MainControl.SecondColumn_ItemsSignaturesOffsetBinder) as TextBlock;

            FocusedColumnItem.ItemSignaruteTextBlockLink = Output;

            //.SetBindingWithReturn(FrameworkElement.MarginProperty, "Margin", TargetColumn.Equals("1") ? MainControl.FirstColumn_ItemsSignaturesOffsetBinder : MainControl.SecondColumn_ItemsSignaturesOffsetBinder)
            return Output;
        }





        /// <summary>
        /// Return insertable grid with skill name replication by constructor and imported desc from regular json localization file
        /// </summary>
        /// <param name="DisplayInformation">Deserialized Skill Constructor</param>
        /// <param name="TextInfo">Skill Localization Info</param>
        internal protected static Grid CreateSkillGrid(Type_Skills.UptieLevel TextInfo, SkillContstructor DisplayInformation, SkillInfoFormationParameters FormationParameters = null, string TargetColumn = "1")
        {
            if (FormationParameters == null) FormationParameters = new SkillInfoFormationParameters();

            SolidColorBrush AffinityColor = GetAffinityColor_DarkerVersion(DisplayInformation.Specific.Affinity);

            string SelectedIconID = DisplayInformation.IconID != null ? DisplayInformation.IconID : $"{DisplayInformation.ID}";


            string SelectedSkillFrameSource =
                !DisplayInformation.Specific.Affinity.Equals("None") // Add Affinity frame if not 'None', else default frame
                  ? $"UI/Limbus/Skills/Frames/{DisplayInformation.Specific.Affinity}/{DisplayInformation.Specific.Rank}.png"
                  : $"UI/Limbus/Skills/Frames/Skill Default Frame.png";

            // Skill icon, frame
            Grid SkillIconSite = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 137,
                Width = 135,
                Margin = new Thickness(-10, 0, 0, 0),
                LayoutTransform = new ScaleTransform(1.04, 1.04),
                Children =
                {
                    // Skill icon
                    new Image()
                    {
                        Margin = new Thickness(52.8, 56, 0, 0),
                        Height = 65.2,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment   = VerticalAlignment.Top,
                        Source = Mode_Skills.AcquireSkillIcon(SelectedIconID),
                        OpacityMask = new ImageBrush() { ImageSource = ImageFromResource("UI/Limbus/Skills/Icon Opacity Mask.png") }
                    },

                    // Frame
                    new Image()
                    {
                        Source = ImageFromResource(SelectedSkillFrameSource),
                        Margin = new Thickness(22.8, 24, 0, 0),
                        Height = 127,
                        Width  = 127,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment   = VerticalAlignment.Top
                    },

                    // Damage type
                    DisplayInformation.Specific.DamageType.EqualsOneOf(["Pierce", "Blunt", "Slash"]) ? new Grid()
                    {
                        Margin = new Thickness(74, 111, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment   = VerticalAlignment.Top,
                        RenderTransform = new ScaleTransform(1.1, 1.1),
                        RenderTransformOrigin = new Point(0.5, 1),
                        Children =
                        {
                            new Grid() // Damage icon frame, 3 layers ui element
                            {
                                Height = 26,
                                Width  = 26,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Children =
                                {
                                    new Border()
                                    {
                                        Background = AffinityColor,
                                        OpacityMask = new ImageBrush() { ImageSource = ImageFromResource("UI/Limbus/Skills/Damage Type Opacity Mask.png") }
                                    },
                                    new Border()
                                    {
                                        Background = AffinityColor,
                                        OpacityMask = new ImageBrush() { ImageSource = ImageFromResource("UI/Limbus/Skills/Damage Type Opacity Mask.png") },
                                        Height = 22.3,
                                        Width  = 22.3,
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        Child = new Border() { Background = ToSolidColorBrush("#7A000000") }
                                    },
                                    new Border()
                                    {
                                        Background = ToSolidColorBrush("#FF232323"),
                                        OpacityMask = new ImageBrush() { ImageSource = ImageFromResource("UI/Limbus/Skills/Damage Type Opacity Mask.png") },
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment   = VerticalAlignment.Center,
                                        Height = 17.3,
                                        Width  = 17.3,
                                        Child = new Border() { Background = ToSolidColorBrush("#7F000000") }
                                    }
                                }
                            },
                            new Grid() // Damege icon
                            {
                                Margin =  DisplayInformation.Specific.DamageType switch {
                                    "Pierce" => new Thickness(3.66, 2, 2, 2),
                                    "Blunt" => new Thickness(4, -0.7, 2, 2),
                                    "Slash" => new Thickness(0.7, 2, 2, 2),
                                    _ => new Thickness(2, 2, 2, 2),
                                },
                                Width = 20,
                                Children =
                                {
                                    new Image() { Source = ImageFromResource($"UI/Limbus/Skills/Damage Types/{DisplayInformation.Specific.DamageType}.png") }
                                }
                            }
                        }
                    } : new Grid(), // Or do not insert damage type Grid if damage type info is invalid
                }
            };

            // Skill name with background
            Grid SkillName_MainGrid = GetNameWithBackground(TextInfo.Name, DisplayInformation.Specific.Affinity);

            // Coin icons above skill name
            StackPanel CoinsTab = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment   = VerticalAlignment.Top,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(3.1, -5, 0, 0),
                Height = 33,
            };
            foreach (string CoinType in DisplayInformation.Characteristics.CoinsList)
            {
                CoinsTab.Children.Add(CoinType switch
                {
                    "Regular"     => new Image() { Source = Mode_Skills.RegularCoinIcon     },
                    "Unbreakable" => new Image() { Source = Mode_Skills.UnbreakableCoinIcon }
                });
            }

            // Affinity icon and skill name with background
            StackPanel SkillName = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                
                Children =
                {
                    // Affinity icon of skill
                    !DisplayInformation.Specific.Affinity.Equals("None") & DisplayInformation.Attributes.ShowAffinityIcon ? new Image()
                    {
                        Source = ImageFromResource($"UI/Limbus/Skills/Affinity Icons/{DisplayInformation.Specific.Affinity}.png"),
                        Height = 68,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(-5, 11, -4, 0)
                    } : new Image(),
                    new Grid() // Coin icons and skill name
                    {
                        Children =
                        {
                            CoinsTab, // Coin icons
                            new Border() // Grid for skill name (Created above — SkillName_MainGrid)
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Margin = new Thickness(4, 35.8, 0, 0),
                                Child = SkillName_MainGrid
                            }
                        }
                    }
                }
            };


            // Skill desc view
            StackPanel SkillDescription = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(-4, 8.7, 0, 0),
                Width = double.NaN // Main width ('Skills Panel Width' from scan settings)
            };

            // RichTextBoxes creation with applied desc
            {
                // Main desc
                if (!TextInfo.Description.IsNullOrEmpty())
                {
                    RichTextBox MainSkillDescription = new RichTextBox()
                    {
                        FontSize = ProjectFile.LoadedProject.Text.UnifiedTextSize,
                        Foreground = ToSolidColorBrush("#ebcaa2"),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Width = FormationParameters.MainDescWidth,
                        Margin = new Thickness(4, 0, 0, 7),
                    }.ApplyLimbusRichText(
                        RichTextString: TextInfo.Description,
                        SpecifiedTextProcessingMode: "Skills"
                    )
                    .SetBindingWithReturn(RichTextBox.FontFamilyProperty, "FontFamily", MainControl.PreviewLayout_Skills_MainDesc)
                    as RichTextBox;

                    MainSkillDescription.SetValue(Paragraph.LineHeightProperty, CustomIdentityPreviewCreator.SharedParagraphLineHeigh);


                    MainWindow.FocusedColumnItem.SkillMainDescriptionLink = MainSkillDescription;
                    SkillDescription.Children.Add(MainSkillDescription);
                }


                if (TextInfo.Coins != null)
                {
                    // Coins
                    StackPanel CoinDescs = new StackPanel()
                    {
                        Width = FormationParameters.CoinDescsWidth,
                        Margin = new Thickness(5, 0, 40.2, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };
                    MainWindow.FocusedColumnItem.SkillCoinDescriptionsLink = CoinDescs;
                    SkillDescription.Children.Add(CoinDescs);

                    int CoinEnumerator = 1;
                    foreach (var Coin in TextInfo.Coins)
                    {
                        string CollectedCoinDescriptions = "";
                        if (Coin.CoinDescriptions != null)
                        {
                            foreach (var Desc in Coin.CoinDescriptions)
                            {
                                if (Desc.Description != null)
                                {
                                    CollectedCoinDescriptions += $"{Desc.Description}\n";
                                }
                            }
                        }
                        if (!CollectedCoinDescriptions.Equals(""))
                        {
                            RichTextBox SingleCoinDescription = new RichTextBox() { // Coin desc
                                Margin = new Thickness(50, 15, 0, 0),
                                Focusable = false,
                                FontSize = ProjectFile.LoadedProject.Text.UnifiedTextSize,
                                Style = MainControl.FindResource("CoinDesc") as Style
                            }.ApplyLimbusRichText(
                                RichTextString: CollectedCoinDescriptions.TrimEnd('\n'),
                                SpecifiedTextProcessingMode: "Skills"
                            )
                            .SetBindingWithReturn(RichTextBox.FontFamilyProperty, "FontFamily", MainControl.PreviewLayout_Skills_MainDesc)
                            as RichTextBox;

                            SingleCoinDescription.SetValue(Paragraph.LineHeightProperty, 27.0);

                            CoinDescs.Children.Add(new Grid()
                            {
                                Margin = new Thickness(0, 3, 0, 0),
                                Children =
                                {
                                    new Image() // Coin Icon
                                    {
                                        Source = ImageFromResource($"UI/Limbus/Coins/Coin {CoinEnumerator}.png"),
                                        Width = 43.6 + (ProjectFile.LoadedProject.Text.UnifiedTextSize - 20),
                                        HorizontalAlignment = HorizontalAlignment.Left,
                                        VerticalAlignment = VerticalAlignment.Top,
                                    },
                                    SingleCoinDescription,
                                }
                            });
                        }

                        CoinEnumerator++;
                        if (CoinEnumerator == 6) break; // Max 6 coins, idk i don't have 7+ coin icons
                    }
                }
            }



            return new Grid()
            {
                Margin = new Thickness(-30, -34, 0, 0),
                Children =
                {
                    new Grid()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Width = 400,
                        Children =
                        {
                            GenerateItemSignature(TargetColumn),
                        }
                    },
                    new Grid()
                    {
                        Children =
                        {
                            SkillIconSite, // Skill icon, frame, affinity icon, name
                            new StackPanel() // Skill name and description with coins
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Margin = new Thickness(128, 43, 0, 0),
                                Children =
                                {
                                    SkillName,
                                    SkillDescription
                                },
                                LayoutTransform = new ScaleTransform(scaleX: 0.48, scaleY: 0.48) // Reduce text size
                            }
                        }
                    }
                }
            };
        }
    }
}
