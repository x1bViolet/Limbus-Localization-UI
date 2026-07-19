namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public partial class PreviewCreatorPage : Page
    {
        private async void SelectionChanged_Shared(object Sender, SelectionChangedEventArgs Args)
        {
            if (IsLoaded & Args.AddedItems.Count > 0)
            {
                string NewSelection = (Args.AddedItems[0] as UIElement)!.Uid;

                switch ((Sender as ComboBox)!.Name)
                {
                    case nameof(VC_ImageType):

                        MakeAvailableConditional(NewSelection == "Text", RegularTextColumnsSettingsPanel);
                        MakeAvailableConditional(NewSelection == "Summary", PassiveSignaturesOffsetOnSummaryViewPnel);

                        if (NewSelection == "Summary")
                        {
                            ImageNoticeAndLabel_Low.Visibility = Visibility.Visible;
                            ImageNoticeAndLabel_Top.Visibility = Visibility.Collapsed;

                            IdentityPortraitShadowing.Visibility = __TextColumnsCanvas__.Visibility = Visibility.Collapsed;
                            __SummaryInfoCanvas__.Visibility = Visibility.Visible;

                            if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false)
                            {
                                @DataContextDomain.PreviewCreator.ImageInfo.ImageLabelText.Text = "IDENTITY INFO"; // Switch from/to Summary is disabled in E.G.O Info mode
                            }
                        }
                        else if (NewSelection == "Text")
                        {
                            ImageNoticeAndLabel_Low.Visibility = Visibility.Collapsed;
                            ImageNoticeAndLabel_Top.Visibility = Visibility.Visible;

                            IdentityPortraitShadowing.Visibility = __TextColumnsCanvas__.Visibility = Visibility.Visible;
                            __SummaryInfoCanvas__.Visibility = Visibility.Collapsed;

                            if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false)
                            {
                                @DataContextDomain.PreviewCreator.ImageInfo.ImageLabelText.Text = "IDENTITY SKILL/PASSIVE"; // Switch from/to Summary is disabled in E.G.O Info mode
                            }
                        }

                        break;




                    case nameof(VC_PortraitType):
                        MakeAvailableConditional(NewSelection != "E.G.O", IdentityPortraitImageParametersPanel, ImageTypeSelectionPanel, LeftVignetteAddLengthAdjustPanel, RightVignetteAddLengthAdjustPanel, Vignette_Right, Vignette_Left);
                        MakeAvailableConditional(NewSelection == "E.G.O", EGOPortraitImageParametersPanel);

                        if (NewSelection == "E.G.O")
                        {
                            VC_ImageType.SelectedIndex = 0;

                            ImageNoticeAndLabel_Low.Visibility = Visibility.Visible;
                            ImageNoticeAndLabel_Top.Visibility = Visibility.Collapsed;

                            if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false)
                            {
                                @DataContextDomain.PreviewCreator.ImageInfo.ImageLabelText.Text = "E.G.O INFO";
                            }
                            __TextColumnsCanvas__.SetTopMargin(572);
                        }
                        else
                        {
                            ImageNoticeAndLabel_Low.Visibility = Visibility.Collapsed;
                            ImageNoticeAndLabel_Top.Visibility = Visibility.Visible;

                            if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false)
                            {
                                @DataContextDomain.PreviewCreator.ImageInfo.ImageLabelText.Text = "IDENTITY INFO";
                            }
                            __TextColumnsCanvas__.SetTopMargin(167);
                        }

                        break;




                    case nameof(VC_SinnerIcon):
                        string SinnerName = NewSelection;

                        if (SinnerName.EqualsToOneOf("Yi Sang", "Faust", "Don Quixote", "Ryōshū", "Meursault", "Hong Lu", "Heathcliff", "Ishmael", "Rodion", "Sinclair", "Outis", "Gregor"))
                        {
                            CustomSinnerIcon_ComboBoxItemIconDisplay.Visibility = Visibility.Collapsed;

                            // When switching to [Custom] from this one, keep the old Sinner icon showing instead of disappearing when file selection dialog appears (Icon image source is binded to selected item(as StackPanel) 1st Child(as Image)'s Source)
                            CustomSinnerIcon_ComboBoxItemIconDisplay.Source = ((Args.AddedItems[0] as StackPanel)!.Children[0] as Image)!.Source;


                            if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false)
                            {
                                @DataContextDomain.PreviewCreator.ImageInfo.Header.SinnerName.Text = NewSelection;
                            }

                            if (VC_PortraitType.SelectedIndex == 0) // If Identity portrait mode (E.G.O Skills have Affinity as color instead of Sinner color)
                            {
                                if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false) UnsealCautions();

                                _ = @DataContextDomain.PreviewCreator.ImageInfo.Header.Color
                                  = @DataContextDomain.PreviewCreator.ImageInfo.Cautions.Color
                                  = GetSinnerColor(SinnerName).Replace("#", "");

                                if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false)
                                {
                                    await Task.Delay(50);
                                    SealCautions();
                                }
                            }
                        }
                        else // If option [Custom] was selected
                        {
                            if (VC_PortraitType.SelectedIndex == 0) // If Identity portrait mode (E.G.O Skills have Affinity as color instead of Sinner color)
                            {
                                if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false) UnsealCautions();

                                _ = @DataContextDomain.PreviewCreator.ImageInfo.Header.Color
                                  = @DataContextDomain.PreviewCreator.ImageInfo.Cautions.Color
                                  = "ffffff";

                                if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false)
                                {
                                    await Task.Delay(50);
                                    SealCautions();
                                }
                            }

                            if (!@CurrentPreviewCreator.IsImageInfoLoadingEvent)
                            {
                                @DataContextDomain.PreviewCreator.ImageInfo.Header.SinnerName.Text = "";
                            }


                            if (@CurrentPreviewCreator.IsImageInfoLoadingEvent)
                            {
                                /// Custom Sinner icon is handled at the <see cref="ExchangeRemainingNonMvvmOptions"/>
                            }
                            else
                            {
                                OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
                                if (Select.ShowDialog() == true)
                                {
                                    SelectCustomSinnerIcon(Select.FileName);
                                }
                                else if (Args.RemovedItems.Count > 0)
                                {
                                    VC_SinnerIcon.SelectedItem = Args.RemovedItems[0]; // Select previous if cancel
                                }
                            }
                        }

                        break;




                    case nameof(VC_TextBackgroundEffectsClipMode):
                        if (IsLoaded && Args.AddedItems.Count > 0)
                        {
                            BindingOperations.SetBinding(TextBackgroundEffectsClip, VisualBrush.VisualProperty, new Binding()
                            {
                                ElementName = (Args.AddedItems[0] as IntenseStareType1)!.Uid switch
                                {
                                    "Bottom Vignette" => nameof(Vignette_Bottom_ParentGrid),
                                    "All Vignettes" => nameof(Vignettes),
                                }
                            });
                        }
                        break;

                    default: break;
                }
            }
        }

        private void SelectCustomSinnerIcon(string ImagePath)
        {
            CustomSinnerIconSelectableOption.Uid = ImagePath;
            CustomSinnerIcon_ComboBoxItemIconDisplay.Visibility = Visibility.Visible;
            CustomSinnerIcon_ComboBoxItemIconDisplay.Source = BitmapFromFile(ImagePath);
        }

        // Left click on a [Custom] option when its already selected
        private void SelectCustomSinnerIcon_AgainClickWhileSelected(object Sender, MouseButtonEventArgs Args)
        {
            if (VC_SinnerIcon.SelectedIndex == 12) // When selecting from another option, the index is not equal to 12 at this moment of click
            {
                OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
                if (Select.ShowDialog() == true)
                {
                    SelectCustomSinnerIcon(Select.FileName);
                }
            }
        }



        private void WalpurgisNightToggler_CheckUncheck(object Sender, RoutedEventArgs Args)
        {
            bool WalpurgisNightLogoMode = (bool)WalpurgisNightModeToggler.IsChecked!;

            @DataContextDomain.PreviewCreator.ImageInfo.OtherEffects.WalpurgisNightLogo = WalpurgisNightLogoMode;

            WalpurgisNightDecorationsGrid.Visibility = WalpurgisNightLogoMode ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}