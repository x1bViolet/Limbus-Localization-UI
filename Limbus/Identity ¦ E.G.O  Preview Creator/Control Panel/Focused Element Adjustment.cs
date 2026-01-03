using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using LC_Localization_Task_Absolute.PreviewCreator;
using LC_Localization_Task_Absolute.UITranslationHandlers;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Passives;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.SkillsDisplayInfo;
using static LC_Localization_Task_Absolute.Limbus_Integration.TMProEmitter;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.PreviewCreator.CompositionData_PROP.TextColumns_PROP;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    #region Focus on element
    public void FocusOnColumnElement(Grid Target)
    {
        @CurrentPreviewCreator.FocusingOnElementEvent = true;

        @CurrentPreviewCreator.FocusedColumnElement = Target;
        MakeAvailable(AdjustingElementPanel); // Just unlock

        TextItem_PROP FocusedElementInfo = Target.DataContext as TextItem_PROP;

        ExternElement("[C] * [Section:Textual info/Selected item settings] Adjusting element header", null, FocusedElementInfo.UID);



        #region Restore IDs selection, sliders, and adjusters availability
        MakeUnavailable(
            SkillConstructorSelection_Panel,
            ItemSignatureInputPanel,

            PassiveDescriptionWidth_Panel,
            SkillWidths_Panel,
            KeywordIconSelector_Panel
        );

        FocusedItem_SignatureInput.Text = FocusedElementInfo.Signature;

        _ = SkillLocalizationIDSelector.SelectedIndex
          = SkillConstructorIDSelector.SelectedIndex
          = PassiveLocalizationIDSelector.SelectedIndex
          = KeywordLocalizationIDSelector.SelectedIndex
          = -1;

        _ = SkillLocalizationIDSelector.Visibility
          = PassiveLocalizationIDSelector.Visibility
          = KeywordLocalizationIDSelector.Visibility
          = Visibility.Collapsed;

        switch (FocusedElementInfo.Type)
        {
            case "Skill":
                SkillLocalizationIDSelector.Visibility = Visibility.Visible;
                MakeAvailable(
                    SkillConstructorSelection_Panel,
                    ItemSignatureInputPanel,
                    SkillWidths_Panel
                );

                if (FocusedElementInfo.SelectedLocalizationID != null) ReSelectIDSelectorItem(FocusedElementInfo.SelectedLocalizationID, SkillLocalizationIDSelector);
                if (FocusedElementInfo.SelectedSkillConstructorID != null) ReSelectIDSelectorItem(FocusedElementInfo.SelectedSkillConstructorID, SkillConstructorIDSelector);

                // Manually add displaying if FocusOnColumnElement called while loading image info
                if (@CurrentPreviewCreator.ImageInfoLoadingEvent) CheckBothSkillsIDSelectorsAndAddDisplaying();

                break;

            case "Passive":
                PassiveLocalizationIDSelector.Visibility = Visibility.Visible;
                MakeAvailable(
                    ItemSignatureInputPanel,
                    PassiveDescriptionWidth_Panel
                );

                if (FocusedElementInfo.SelectedLocalizationID != null) ReSelectIDSelectorItem(FocusedElementInfo.SelectedLocalizationID, PassiveLocalizationIDSelector);

                // Manually add displaying if FocusOnColumnElement called while loading image info
                if (@CurrentPreviewCreator.ImageInfoLoadingEvent) CheckPassiveIDSelectorAndAddDisplaying();

                break;

            case "Keyword":
                KeywordLocalizationIDSelector.Visibility = Visibility.Visible;
                MakeAvailable(KeywordIconSelector_Panel);

                if (FocusedElementInfo.SelectedLocalizationID != null) ReSelectIDSelectorItem(FocusedElementInfo.SelectedLocalizationID, KeywordLocalizationIDSelector);

                // Manually add displaying if FocusOnColumnElement called while loading image info
                if (@CurrentPreviewCreator.ImageInfoLoadingEvent) CheckKeywordIDSelectorAndAddDisplaying();

                break;
        }
        VC_FocusedColumnElement_VerticalOffset.Value = FocusedElementInfo.VerticalOffset;
        VC_FocusedColumnElement_HorizontalOffset.Value = FocusedElementInfo.HorizontalOffset;
        VC_FocusedColumnElement_NameMaximumWidth.Value = FocusedElementInfo.MaxWidth_Name;
        VC_FocusedColumnElement_PassiveDescriptionWidth.Value = FocusedElementInfo.MaxWidth_PassiveDescription;
        VC_FocusedColumnElement_SkillMainDescriptionWidth.Value = FocusedElementInfo.MaxWidth_SkillMainDescription;
        VC_FocusedColumnElement_SkillCoinDescriptionsWidth.Value = FocusedElementInfo.MaxWidth_SkillCoinsDescription;
        FocusedItem_SignatureInput.Text = FocusedElementInfo.Signature;
        if (FocusedElementInfo.KeywordIcon_Path != "")
        {
            ExternElement("[C] * [Section:Textual info/Selected item settings] Keyword icon (Label)", "Selected", Path.GetFileName(FocusedElementInfo.KeywordIcon_Path));
        }
        else
        {
            ExternElement("[C] * [Section:Textual info/Selected item settings] Keyword icon (Label)", "Default");
        }

        #region Load again
        if (@CurrentPreviewCreator.ImageInfoLoadingEvent)
        {
            if (File.Exists(FocusedElementInfo.KeywordIcon_Path)) FocusedColumnElement_SelectKeywordIcon_Action(FocusedElementInfo.KeywordIcon_Path);
        }
        #endregion

        #endregion



        @CurrentPreviewCreator.FocusingOnElementEvent = false;
    }
    private void ReSelectIDSelectorItem(string ElementID, ComboBox TargetComboBox)
    {
        foreach (UITranslation_Rose MenuItem in TargetComboBox.Items)
        {
            string ElementUid = MenuItem.Uid;
            if (ElementUid != null && ElementUid == ElementID) TargetComboBox.SelectedItem = MenuItem;
        }
    }
    #endregion

    #region Textual info / Selected item settings

    #region ID Selectors
    private void SkillLocalizationIDSelector_SelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
    {
        if (!@CurrentPreviewCreator.ComboBoxItemAddEvent & !@CurrentPreviewCreator.FocusingOnElementEvent & EventArgs.AddedItems.Count > 0)
        {
            Type_Skills.UptieLevel SelectedSkill = (EventArgs.AddedItems[0] as UITranslation_Rose).DataContext as Type_Skills.UptieLevel;
            TextItem_PROP TargetElementInfo = @CurrentPreviewCreator.FocusedColumnElement.DataContext as TextItem_PROP;
            TargetElementInfo.SelectedLocalizationID = (EventArgs.AddedItems[0] as UITranslation_Rose).Uid;

            CheckBothSkillsIDSelectorsAndAddDisplaying();
        }
    }
    private void SkillConstructorIDSelector_SelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
    {
        if (!@CurrentPreviewCreator.ComboBoxItemAddEvent & !@CurrentPreviewCreator.FocusingOnElementEvent & EventArgs.AddedItems.Count > 0)
        {
            SkillsDisplayInfo.SkillConstructor SelectedSkillConstructor = (EventArgs.AddedItems[0] as UITranslation_Rose).DataContext as SkillsDisplayInfo.SkillConstructor;
            TextItem_PROP TargetElementInfo = @CurrentPreviewCreator.FocusedColumnElement.DataContext as TextItem_PROP;
            TargetElementInfo.SelectedSkillConstructorID = (EventArgs.AddedItems[0] as UITranslation_Rose).Uid;

            CheckBothSkillsIDSelectorsAndAddDisplaying();
        }
    }
    public void CheckBothSkillsIDSelectorsAndAddDisplaying()
    {
        if (SkillLocalizationIDSelector.SelectedIndex != -1 && SkillConstructorIDSelector.SelectedIndex != -1)
        {
            Type_Skills.UptieLevel SelectedSkill = (SkillLocalizationIDSelector.SelectedItem as UITranslation_Rose).DataContext as Type_Skills.UptieLevel;
            SkillsDisplayInfo.SkillConstructor SelectedSkillConstructor = (SkillConstructorIDSelector.SelectedItem as UITranslation_Rose).DataContext as SkillsDisplayInfo.SkillConstructor;

            AssertAndAddDisplaying(CreateSkill(SelectedSkill, SelectedSkillConstructor));
        } 
    }


    private void PassiveLocalizationIDSelector_SelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
    {
        if (!@CurrentPreviewCreator.ComboBoxItemAddEvent & !@CurrentPreviewCreator.FocusingOnElementEvent & EventArgs.AddedItems.Count > 0)
        {
            TextItem_PROP TargetElementInfo = @CurrentPreviewCreator.FocusedColumnElement.DataContext as TextItem_PROP;
            TargetElementInfo.SelectedLocalizationID = (EventArgs.AddedItems[0] as UITranslation_Rose).Uid;

            CheckPassiveIDSelectorAndAddDisplaying();
        }
    }
    public void CheckPassiveIDSelectorAndAddDisplaying()
    {
        if (PassiveLocalizationIDSelector.SelectedIndex != -1)
        {
            Type_Passives.Passive SelectedPassive = (PassiveLocalizationIDSelector.SelectedItem as UITranslation_Rose).DataContext as Type_Passives.Passive;

            AssertAndAddDisplaying(CreatePassive(SelectedPassive));
        }
    }


    private void KeywordLocalizationIDSelector_SelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
    {
        if (!@CurrentPreviewCreator.ComboBoxItemAddEvent & !@CurrentPreviewCreator.FocusingOnElementEvent & EventArgs.AddedItems.Count > 0)
        {
            TextItem_PROP TargetElementInfo = @CurrentPreviewCreator.FocusedColumnElement.DataContext as TextItem_PROP;
            TargetElementInfo.SelectedLocalizationID = (EventArgs.AddedItems[0] as UITranslation_Rose).Uid;

            CheckKeywordIDSelectorAndAddDisplaying();
        }
    }
    public void CheckKeywordIDSelectorAndAddDisplaying()
    {
        if (KeywordLocalizationIDSelector.SelectedIndex != -1)
        {
            Type_Keywords.Keyword SelectedKeyword = (KeywordLocalizationIDSelector.SelectedItem as UITranslation_Rose).DataContext as Type_Keywords.Keyword;

            AssertAndAddDisplaying(CreateKeyword(SelectedKeyword));
        }
    }
    
    
    
    private void AssertAndAddDisplaying(Grid ElementToAdd)
    {
        if (@CurrentPreviewCreator.FocusedColumnElement.Children.Count == 2) @CurrentPreviewCreator.FocusedColumnElement.Children.RemoveAt(1);
        else @CurrentPreviewCreator.FocusedColumnElement.Children[0].Visibility = Visibility.Collapsed;

        @CurrentPreviewCreator.FocusedColumnElement.Children.Add(ElementToAdd);
    }
    #endregion
    

    private void ChangeFocusedItemSignature(object RequestSender, TextChangedEventArgs EventArgs)
    {
        if (MainControl.IsLoaded & !@CurrentPreviewCreator.ComboBoxItemAddEvent & !@CurrentPreviewCreator.FocusingOnElementEvent)
        {
            TextItem_PROP Info = @CurrentPreviewCreator.FocusedColumnElement.DataContext as TextItem_PROP;
            if (Info.Link_ItemSignature != null) Info.Link_ItemSignature.Text = (RequestSender as UITranslation_Mint).Text;
            Info.Signature = (RequestSender as UITranslation_Mint).Text;
        }
    }


    private void FocusedColumnElement_VerticalOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            @CurrentPreviewCreator.FocusedColumnElement.SetTopMargin(EventArgs.NewValue);
            @CurrentPreviewCreator.FocusedColumnElementContext.VerticalOffset = EventArgs.NewValue;
        }
    }
    private void FocusedColumnElement_HorizontalOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            @CurrentPreviewCreator.FocusedColumnElementContext.HorizontalOffset = EventArgs.NewValue;

            if (@CurrentPreviewCreator.FocusedColumnElement.Children.Count == 2)
            {
                //                                 Main item grid       Actual content (Grid/StackPanel/Border), Children[0] is item signature
                (@CurrentPreviewCreator.FocusedColumnElement.Children[1] as Grid).Children[1].RenderTransform = new TranslateTransform(EventArgs.NewValue, 0);
            }
        }
    }


    private void FocusedColumnElement_NameMaximumWidth(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            TextItem_PROP Info = @CurrentPreviewCreator.FocusedColumnElementContext;
            if (Info.Link_Name != null) Info.Link_Name.MaxWidth = EventArgs.NewValue;
            Info.MaxWidth_Name = EventArgs.NewValue;
        }
    }

    private void FocusedColumnElement_PassiveDescriptionWidth(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            TextItem_PROP Info = @CurrentPreviewCreator.FocusedColumnElementContext;
            if (Info.Link_PassiveDescription != null) Info.Link_PassiveDescription.MaxWidth = EventArgs.NewValue;
            Info.MaxWidth_PassiveDescription = EventArgs.NewValue;
        }
    }


    private void FocusedColumnElement_SkillMainDescriptionWidth(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            TextItem_PROP Info = @CurrentPreviewCreator.FocusedColumnElementContext;
            if (Info.Link_SkillMainDescription != null) Info.Link_SkillMainDescription.MaxWidth = EventArgs.NewValue;
            Info.MaxWidth_SkillMainDescription = EventArgs.NewValue;
        }
    }
    private void FocusedColumnElement_SkillCoinDescriptionsWidth(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            TextItem_PROP Info = @CurrentPreviewCreator.FocusedColumnElementContext;
            if (Info.Link_SkillCoinDescriptions != null) Info.Link_SkillCoinDescriptions.MaxWidth = EventArgs.NewValue;
            Info.MaxWidth_SkillCoinsDescription = EventArgs.NewValue;
        }
    }

    private void FocusedColumnElement_SelectKeywordIcon(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) FocusedColumnElement_SelectKeywordIcon_Action(Select.FileName);
    }
    public static void FocusedColumnElement_SelectKeywordIcon_Action(string ImagePath)
    {
        TextItem_PROP Info = @CurrentPreviewCreator.FocusedColumnElement.DataContext as TextItem_PROP;
        Info.KeywordIcon_Path = ImagePath.Replace("\\", "/");
        Info.Link_KeywordIcon.Source = BitmapFromFile(ImagePath);

        if (File.Exists(ImagePath))
        {
            ExternElement("[C] * [Section:Textual info/Selected item settings] Keyword icon (Label)", "Selected", Path.GetFileName(ImagePath));
        }
        else
        {
            ExternElement("[C] * [Section:Textual info/Selected item settings] Keyword icon (Label)", "Default");
        }
    }
    #endregion

    #region Elements view creation

    #region Parts
    private static Grid CreateSkillOrPassiveName(string Name, string AffinityName = "None", bool ReversedDropdownShadow = false, double ManualFontSize = 29)
    {
        /*/
         * Grid with TextBlock for the skill name + horizontal StackPanel with actual background color and image on the right side
         * First child of StackPanel (Affinity color filler) is being resized by actual width binding to the TextBlock
         * Second child (Image with pattern) stays fixed horizontally, resizes only vertically when line break appears
        /*/
        TMProEmitter NameTextBlock = new()
        {
            TextProcessingMode = EditorMode.Keywords,
            FontType = LimbusFontTypes.Title,

            MaxWidth = @CurrentPreviewCreator.FocusedColumnElementContext.MaxWidth_Name,

            Margin = new Thickness(10, 11, 0, 4.4),
            HorizontalAlignment = HorizontalAlignment.Left,
            FontSize = ManualFontSize,
            LineHeight = ManualFontSize,
            Effect = new DropShadowEffect() { ShadowDepth = 3, BlurRadius = 0, Direction = -50 },
        };
        NameTextBlock.RichText = Name;

        @CurrentPreviewCreator.FocusedColumnElementContext.Link_Name = NameTextBlock;


        Grid Name_MainGrid = new()
        {
            Effect = new DropShadowEffect() { ShadowDepth = 4, BlurRadius = 0, Direction = ReversedDropdownShadow ? -40 : 230 },
            Children =
            {
                new StackPanel()
                {
                    Orientation = Orientation.Horizontal, //--//
                    Margin = new Thickness(0, 0, 110, 0),
                    ClipToBounds = true, // Needed
                    Children =
                    {
                        new Border() // Affinity color filler
                        {
                            MinWidth = 110,
                            Background = ToSolidColorBrush(@ColorInfo.GetAffinityColor_InfoPreviewColors(AffinityName)),
                            Margin = new Thickness(-50, 0, 0 ,0)
                        }.SetBindingWithReturn(FrameworkElement.WidthProperty, "ActualWidth", NameTextBlock), // Dynamic resize by name
                        new Image() // Image on the right side
                        {
                            Source = BitmapFromResource($"UI/Limbus/Skills/Name Background/Preview Colors/{AffinityName}.png"),
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
    private static TextBlock CreateItemSignature(string TargetColumn)
    {
        TranslateTransform OffsetTransform = new TranslateTransform();
        BindingOperations.SetBinding(OffsetTransform, TranslateTransform.XProperty, new Binding(path: "Value")
        {
            Source = TargetColumn == "1" ? MainControl.VC_FirstColumnSignaturesOffset : MainControl.VC_SecondColumnSignaturesOffset
        });
        TextBlock Output = new TextBlock()
        {
            Text = @CurrentPreviewCreator.FocusedColumnElementContext.Signature,
            RenderTransform = OffsetTransform,

            FontSize = 35,
            Foreground = ToSolidColorBrush("#47311e"),
            Margin = new Thickness(0, 40, 0, 0),

            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,

            TextAlignment = TextAlignment.Right,

            Effect = new DropShadowEffect() { ShadowDepth = 0, BlurRadius = 7 }
        };
        Output.SetResourceReference(TextBlock.FontFamilyProperty, "TextColumns_ItemSignaturesFont");
        @CurrentPreviewCreator.FocusedColumnElementContext.Link_ItemSignature = Output;

        return Output;
    }
    #endregion

    public static Grid CreateSkill(UptieLevel TextInfo, SkillConstructor DisplayInfo)
    {
        SolidColorBrush AffinityColor = ToSolidColorBrush(@ColorInfo.GetAffinityColor_InfoPreviewColors(DisplayInfo.Specific.Affinity));

        string SelectedIconID = DisplayInfo.IconID ?? $"{DisplayInfo.ID}";

        string SelectedSkillFrameSource =
            DisplayInfo.Specific.Affinity != "None" // Add Affinity frame if not 'None', else default frame
                ? $"UI/Limbus/Skills/Frames/{DisplayInfo.Specific.Affinity}/{DisplayInfo.Specific.Rank}.png"
                : $"UI/Limbus/Skills/Frames/Skill Default Frame.png";

        #region Skill icon with affinity frame and damage type
        Grid Part_SkillIcon = new()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment   = VerticalAlignment.Top,

            Margin = new Thickness(-10, 0, 0, 0),

            Height = 137,
            Width  = 135,

            LayoutTransform = new ScaleTransform(1.04, 1.04),
            
            Children =
            {
                #region Icon image
                new Image()
                {
                    Margin = new Thickness(52.8, 56, 0, 0),
                    Height = 65.2,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment   = VerticalAlignment.Top,
                    Source = Mode_Skills.AcquireSkillIcon(SelectedIconID),
                    OpacityMask = new ImageBrush() { ImageSource = BitmapFromResource("UI/Limbus/Skills/Icon Opacity Mask.png") }
                },
                #endregion

                #region Affinity frame
                new Image()
                {
                    Source = BitmapFromResource(SelectedSkillFrameSource),
                    Margin = new Thickness(22.8, 24, 0, 0),
                    Height = 127,
                    Width  = 127,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment   = VerticalAlignment.Top
                },
                #endregion

                #region Damage type
                DisplayInfo.Specific.DamageType.EqualsOneOf(["Pierce", "Blunt", "Slash"]) ? new Grid()
                {
                    HorizontalAlignment   = HorizontalAlignment.Left,
                    VerticalAlignment     = VerticalAlignment.Top,
                    RenderTransform       = new ScaleTransform(1.1, 1.1),
                    RenderTransformOrigin = new Point(0.5, 1),
                    Margin = new Thickness(74, 111, 0, 0),
                    Children =
                    {
                        new Grid() // Damage icon frame, 3 layers ui element
                        {
                            Height = 26,
                            Width  = 26,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment   = VerticalAlignment.Top,
                            Children =
                            {
                                new Border()
                                {
                                    Background  = AffinityColor,
                                    OpacityMask = new ImageBrush() { ImageSource = BitmapFromResource("UI/Limbus/Skills/Damage Type Opacity Mask.png") }
                                },
                                new Border()
                                {
                                    Background  = AffinityColor,
                                    OpacityMask = new ImageBrush() { ImageSource = BitmapFromResource("UI/Limbus/Skills/Damage Type Opacity Mask.png") },
                                    
                                    Height = 22.3,
                                    Width  = 22.3,

                                    VerticalAlignment   = VerticalAlignment.Center, // Suspecious

                                    Child = new Border() { Background = ToSolidColorBrush("#7A000000") }
                                },
                                new Border()
                                {
                                    Background  = ToSolidColorBrush("#FF232323"),
                                    OpacityMask = new ImageBrush() { ImageSource = BitmapFromResource("UI/Limbus/Skills/Damage Type Opacity Mask.png") },
                                    
                                    Height = 17.3,
                                    Width  = 17.3,

                                    VerticalAlignment   = VerticalAlignment.Center,

                                    Child = new Border() { Background = ToSolidColorBrush("#7F000000") }
                                }
                            }
                        },
                        new Grid() // Damege icon
                        {
                            Width = 20,
                            Margin =  DisplayInfo.Specific.DamageType switch
                            {
                                "Pierce" => new Thickness(+2.96, +2.00, +2.00, +2.00),
                                "Blunt"  => new Thickness(+4.00, -0.70, +2.00, +2.00),
                                "Slash"  => new Thickness(+1.40, +2.00, +2.00, +2.00),
                            },
                            Children =
                            {
                                new Image()
                                {
                                    Source = BitmapFromResource($"UI/Limbus/Skills/Damage Types/{DisplayInfo.Specific.DamageType}.png")
                                }
                            }
                        }
                    }
                } : new Grid(), // Or do not insert damage type Grid if damage type info is invalid
                #endregion
            }
        };
        #endregion
        
        #region Skill name with affinity icon and coins tab

        #region Skill name with background
        Grid Part_SkillName = CreateSkillOrPassiveName(TextInfo.Name, DisplayInfo.Specific.Affinity);
        #endregion

        #region Skill coin icons above the name
        StackPanel Part_CoinsTab = new StackPanel()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(3.1, -5, 0, 0),
            Orientation = Orientation.Horizontal,
            Height = 33,
        };

        foreach (string CoinType in DisplayInfo.Characteristics.CoinsList)
        {
            Part_CoinsTab.Children.Add(CoinType switch
            {
                "Regular"     => new Image() { Source = Mode_Skills.RegularCoinIcon     },
                "Unbreakable" => new Image() { Source = Mode_Skills.UnbreakableCoinIcon }
            });
        }
        #endregion

        StackPanel Part_SkillNameWithAffinityIconAndCoinsTab = new StackPanel()
        {
            Orientation = Orientation.Horizontal,

            Children =
            {
                #region Affinity icon
                (DisplayInfo.Specific.Affinity != "None" & (bool)DisplayInfo.Attributes.ShowAffinityIcon) ? new Image()
                {
                    Source = BitmapFromResource($"UI/Limbus/Skills/Affinity Icons/{DisplayInfo.Specific.Affinity}.png"),
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(-5, 11, -4, 0),
                    Height = 68,
                } : new Image(),
                #endregion

                new Grid()
                {
                    Children =
                    {
                        Part_CoinsTab,
                        new Border()
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(4, 35.8, 0, 0),
                            Child = Part_SkillName
                        }
                    }
                }
            }
        };
        #endregion

        #region Skill descriptions
        StackPanel Part_SkillDescriptionPanel = new StackPanel()
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(-4, 8.7, 0, 0),
        };

        #region Main skill description
        if (!string.IsNullOrEmpty(TextInfo.PresentDescription))
        {
            TMProEmitter SubPart_SkillMainDesc = new()
            {
                TextProcessingMode = EditorMode.Skills,
                FontType = LimbusFontTypes.Context,

                LineHeight = 27,
                FontSize = 22,
                MaxWidth = @CurrentPreviewCreator.FocusedColumnElementContext.MaxWidth_SkillMainDescription,
                Margin = new Thickness(9, 0, 0, 7),
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            SubPart_SkillMainDesc.RichText = TextInfo.PresentDescription;

            @CurrentPreviewCreator.FocusedColumnElementContext.Link_SkillMainDescription = SubPart_SkillMainDesc;


            Part_SkillDescriptionPanel.Children.Add(SubPart_SkillMainDesc);
        }
        #endregion

        #region Skill coin descriptions
        if (TextInfo.Coins != null)
        {
            StackPanel SubPart_SkillCoinDescs = new StackPanel()
            {
                Margin = new Thickness(12, 0, 0, 0),
                MaxWidth = @CurrentPreviewCreator.FocusedColumnElementContext.MaxWidth_SkillCoinsDescription,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            @CurrentPreviewCreator.FocusedColumnElementContext.Link_SkillCoinDescriptions = SubPart_SkillCoinDescs;


            int CurrentCoinNumber = 1;
            foreach (Coin Coin in TextInfo.Coins)
            {
                List<string> CollectedCoinDescriptions = [];
                if (Coin.CoinDescriptions != null)
                {
                    foreach (CoinDesc Desc in Coin.CoinDescriptions)
                    {
                        if (!string.IsNullOrEmpty(Desc.PresentDescription)) CollectedCoinDescriptions.Add(Desc.PresentDescription);
                    }
                }
                if (CollectedCoinDescriptions.Count > 0)
                {
                    TMProEmitter SingleCoinDescription = new()
                    {
                        TextProcessingMode = EditorMode.Skills,
                        FontType = LimbusFontTypes.Context,

                        LineHeight = 27,
                        FontSize = 22,
                        Margin = new Thickness(11, 15, -400, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };
                    SingleCoinDescription.SetResourceReference(TMProEmitter.StyleProperty, "CoinDesc");
                    SingleCoinDescription.BindSame(TMProEmitter.MaxWidthProperty, SubPart_SkillCoinDescs);
                    SingleCoinDescription.RichText = string.Join('\n', CollectedCoinDescriptions);

                    SubPart_SkillCoinDescs.Children.Add(new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 3, 0, 0),
                        Children =
                        {
                            new Image() // Coin icon
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Source = BitmapFromResource($"UI/Limbus/Skills/Coins/Coin {CurrentCoinNumber}.png"),
                                Width = 48,
                                Margin = new Thickness(-2, 0, 0, 0)
                            },
                            SingleCoinDescription // Coin description
                        }
                    });
                }

                if (CurrentCoinNumber == Mode_Skills.MaxCoinsAmount) break;

                CurrentCoinNumber++;
            }

            Part_SkillDescriptionPanel.Children.Add(SubPart_SkillCoinDescs);
        }
        #endregion
        #endregion

        return new Grid()
        {
            Background = Brushes.Transparent,
            Margin = new Thickness(-30, -34, 0, 0),
            Children =
            {
                new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 500,
                    Children = { CreateItemSignature(@CurrentPreviewCreator.FocusedColumnElementContext.ColumnNumber) }
                },
                new Grid()
                {
                    Uid = "SKILL GRID",
                    RenderTransform = new TranslateTransform(@CurrentPreviewCreator.FocusedColumnElementContext.HorizontalOffset, 0), // [SKILL] Set horizontal offset initially
                    
                    Children =
                    {
                        Part_SkillIcon,
                        new StackPanel()
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(128, 43, 0, 0),
                            Children =
                            {
                                Part_SkillNameWithAffinityIconAndCoinsTab,
                                Part_SkillDescriptionPanel
                            },
                            LayoutTransform = new ScaleTransform(scaleX: 0.48, scaleY: 0.48) // Reduce text size
                        }
                    }
                }
            }
        };
    }
    public static Grid CreatePassive(Passive TextInfo)
    {
        Grid PassiveName = CreateSkillOrPassiveName(TextInfo.Name, ReversedDropdownShadow: true);
        PassiveName.LayoutTransform = new ScaleTransform()
        {
            ScaleX = 0.48,
            ScaleY = 0.48
        };

        TMProEmitter PassiveDescription = new()
        {
            TextProcessingMode = EditorMode.Passives,
            FontType = LimbusFontTypes.Context,

            MaxWidth = @CurrentPreviewCreator.FocusedColumnElementContext.MaxWidth_PassiveDescription,

            LineHeight = 27,
            FontSize = 22,
            Margin = new Thickness(0, 5, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            LayoutTransform = new ScaleTransform(0.48, 0.48),
        };
        PassiveDescription.RichText = TextInfo.PresentMainDescription;

        @CurrentPreviewCreator.FocusedColumnElementContext.Link_PassiveDescription = PassiveDescription;

        return new Grid()
        {
            Background = ToSolidColorBrush("#00000000"),
            Margin = new Thickness(42, 15, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Children =
            {
                new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 500,
                    Margin = new Thickness(-71, -60, 0, 0), // Align with Skills
                    Children = { CreateItemSignature(@CurrentPreviewCreator.FocusedColumnElementContext.ColumnNumber) }
                },
                new StackPanel()
                {
                    Uid = "PASSIVE STACKPANEL",
                    RenderTransform = new TranslateTransform(@CurrentPreviewCreator.FocusedColumnElementContext.HorizontalOffset, 0), // [PASSIVE] Set horizontal offset initially

                    Margin = new Thickness(0, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Children =
                    {
                        PassiveName,
                        PassiveDescription
                    }
                }
            }
        };
    }
    public static Grid CreateKeyword(Keyword TextInfo)
    {
        Image KeywordIcon = new Image()
        {
            Height = 25,
            VerticalAlignment = VerticalAlignment.Top,
        };
        @CurrentPreviewCreator.FocusedColumnElementContext.Link_KeywordIcon = KeywordIcon;

        TMProEmitter KeywordName = new()
        {
            TextProcessingMode = EditorMode.Keywords,
            FontType = LimbusFontTypes.Title,

            MaxWidth = @CurrentPreviewCreator.FocusedColumnElementContext.MaxWidth_Name,

            FontSize = 13.5,
            Margin = new Thickness(4, 0, 0, 0),
            Foreground = ToSolidColorBrush(TextInfo.Color != null ? TextInfo.Color : "#9f6a3a"),
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
        };
        KeywordName.RichText = TextInfo.Name;
        Grid.SetColumn(KeywordName, 1);

        @CurrentPreviewCreator.FocusedColumnElementContext.Link_Name = KeywordName;

        TMProEmitter KeywordDescription = new()
        {
            TextProcessingMode = EditorMode.Keywords,
            FontType = LimbusFontTypes.Context,

            FontSize = 22,
            LineHeight = 27,
            Margin = new Thickness(4),
            HorizontalAlignment = HorizontalAlignment.Left,
            LayoutTransform = new ScaleTransform(0.48, 0.48),
        };
        KeywordDescription.RichText = TextInfo.PresentMainDescription;


        Border KeywordContainer = new()
        {
            Uid = "KEYWORD BORDER",
            RenderTransform = new TranslateTransform(@CurrentPreviewCreator.FocusedColumnElementContext.HorizontalOffset, 0), // [KEYWORD] Set horizontal offset initially

            Margin = new Thickness(42, -10, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            BorderBrush = ToSolidColorBrush("#786753"),
            Background = ToSolidColorBrush("#7F000000"),
            BorderThickness = new Thickness(1),

            Child = new StackPanel()
            {
                Children =
                {
                    new Grid()
                    {
                        Margin = new Thickness(5, 5, 5, 0),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition() { Width = new GridLength(25) },
                            new ColumnDefinition(),
                        },
                        Children =
                        {
                            KeywordIcon,
                            KeywordName
                        }
                    },
                    KeywordDescription
                }
            }
        };
        KeywordContainer.SetResourceReference(Border.WidthProperty, "TextColumns_KeywordCointainersWidth"); // 'Keyword containers width' binding

        return new Grid()
        {
            Background = ToSolidColorBrush("#00000000"),
            Children =
            {
                new Grid() { Visibility = Visibility.Collapsed }, // Placeholder instead of signature
                KeywordContainer
            }
        };
    }
    #endregion
}
