using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    /*/—-----------------------------------------------------------------------------------—/*/
    /*/ File for ValueChanged triggers of Sliders from Custom Identity Preview Creator mode /*/
    /*/—-----------------------------------------------------------------------------------—/*/

    #region Image sizes
    void ImageWidth_FirstStep(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        
        Flow(() => {
            IdentityPreviewCreator_ParentGrid.Width
                = ProjectFile.LoadedProject.ImageParameters.WidthAdjustment_FirstStep
                = EventArgs.NewValue;

            ImageWidth_CheckEachStepAvailable();
        });
    }
    
    void ImageWidth_SecondStep(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_ParentGrid.Width
                = ProjectFile.LoadedProject.ImageParameters.WidthAdjustment_SecondStep
                = EventArgs.NewValue;

            ImageWidth_CheckEachStepAvailable();
        });
    }

    void ImageHeight(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_ParentGrid.Height
                = ProjectFile.LoadedProject.ImageParameters.HeightAdjustment
                = EventArgs.NewValue;
        });
    }
    void ImageAutoHeightToggle_SidedLink(object RequestSender /* CheckBox */, RoutedEventArgs EventArgs) => ToggleImageAutoHeight((bool)IdentityPreviewHeight_IsAuto.IsChecked);
    private void ToggleImageAutoHeight(bool Value) // CheckBox
    {
        Flow(() => {
            if (Value == true)
            {
                MakeUnavailable(IdentityPreviewCreator_HeightController);
                IdentityPreviewCreator_HeightController_TextIndicator.Text = "Height adjustment (Auto)";
                IdentityPreviewCreator_ParentGrid.Height = double.NaN;
            }
            else
            {
                MakeAvailable(IdentityPreviewCreator_HeightController);
                IdentityPreviewCreator_HeightController_TextIndicator.Text = "Height adjustment (Manual)";
                IdentityPreviewCreator_ParentGrid.Height = IdentityPreviewCreator_HeightController.Value;
            }

            ProjectFile.LoadedProject.ImageParameters.HeightAdjustment_IsAuto = Value;
        });
    }

    #region Sub methods
    private void ImageWidth_CheckEachStepAvailable()
    {
        Slider First = IdentityPreviewCreator_WidthController_FirstStep;
        Slider Second = IdentityPreviewCreator_WidthController_SecondStep;


        if (First.Value == First.Maximum) MakeAvailable(Second);
        else MakeUnavailable(Second);

        if (Second.Value == Second.Minimum) MakeAvailable(First);
        else MakeUnavailable(First);
    }
    #endregion

    #endregion



    #region Image type
    void ImageTypeVerticalOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() =>
        {
            CustomIdentityPreviewCreator_ImageTypeText.Margin = new Thickness(0, 38 + EventArgs.NewValue, 12.5, 0);

            ProjectFile.LoadedProject.ImageParameters.ImageTypeVerticalOffset = EventArgs.NewValue;
        });
    }
    void ImageTypeTextSize(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() =>
        {
            CustomIdentityPreviewCreator_ImageTypeText.FontSize = EventArgs.NewValue;

            ProjectFile.LoadedProject.ImageParameters.ImageTypeTextSize = EventArgs.NewValue;
        });
    }
    #endregion



    #region Portrait
    void AllocatedWidthForPortrait(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_AllocatedWidthForPortrait.Width = new GridLength(EventArgs.NewValue);

            if (EventArgs.NewValue == IdentityPreviewCreator_AllocatedWidthForPortraitController.Minimum)
            {
                LMakeUnavailable(VignetteControllers);
            }
            else
            {
                LMakeAvailable(VignetteControllers);
            }

            ProjectFile.LoadedProject.ImageParameters.AllocatedWidthForPortrait = EventArgs.NewValue;
        });
    }

    void PortraitOffset_Horizontal(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_IdentityPortrait.SetLeftMargin(EventArgs.NewValue);

            EGOPortraitImage_ParentGrid.SetLeftMargin(EventArgs.NewValue);

            ProjectFile.LoadedProject.ImageParameters.PortraitHorizontalOffset = EventArgs.NewValue;
        });
    }

    void PortraitOffset_Vertical(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_IdentityPortrait.SetTopMargin(EventArgs.NewValue);

            EGOPortraitImage_ParentGrid.SetTopMargin(EventArgs.NewValue);

            ProjectFile.LoadedProject.ImageParameters.PortraitVerticalOffset = EventArgs.NewValue;
        });
    }

    void IdentityPortraitScale(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_IdentityPortrait.Height
                = ProjectFile.LoadedProject.ImageParameters.IdentityPortraitScale
                = EventArgs.NewValue;
        });
    }

    void EGOPortraitScale(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            EGOPortraitImage_ScaleModifier.ScaleX = EventArgs.NewValue / 100;

            ProjectFile.LoadedProject.ImageParameters.EGOPortraitScale = EventArgs.NewValue;
        });
    }

    #region Portrait vignette
    List<UIElement> VignetteControllers; // Filled in InitializeIdentityPreviewCreatorProperties()

    void ChangeVignetteStrength(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_VignetteStrength_SharedBinder.Width
                = ProjectFile.LoadedProject.ImageParameters.VignetteStrength
                = EventArgs.NewValue;
        });
    }

    void VignetteOffset_Top(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_TopVignette.Margin = new Thickness(0, EventArgs.NewValue - 1, 0, 0);

            double FurtherBGSize = EventArgs.NewValue + 1; if (FurtherBGSize < 0) FurtherBGSize = 0;
            IdentityPreviewCreator_TopVignette_FurtherBg.Height = FurtherBGSize;

            ProjectFile.LoadedProject.ImageParameters.TopVignetteOffset = EventArgs.NewValue;
        });
    }

    void VignetteOffset_Left(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_LeftVignette.Margin = new Thickness(EventArgs.NewValue - 1, 0, 0, 0);

            double FurtherBGSize = EventArgs.NewValue + 1; if (FurtherBGSize < 0) FurtherBGSize = 0;
            IdentityPreviewCreator_LeftVignette_FurtherBg.Width = FurtherBGSize;

            ProjectFile.LoadedProject.ImageParameters.LeftVignetteOffset = EventArgs.NewValue;
        });
    }

    void VignetteOffset_Bottom(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_BottomVignette.Margin = new Thickness(0, 0, 0, EventArgs.NewValue - 1);

            double FurtherBGSize = EventArgs.NewValue + 1; if (FurtherBGSize < 0) FurtherBGSize = 0;
            IdentityPreviewCreator_BottomVignette_FurtherBg.Height = FurtherBGSize;

            ProjectFile.LoadedProject.ImageParameters.BottomVignetteOffset = EventArgs.NewValue;
        });
    }

    #endregion

    #endregion



    #region Identity/E.G.O header
    void HeaderGeneralOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_IdentityHeader_ParentGrid.Margin = new Thickness(EventArgs.NewValue, 0, 0, 0);

            ProjectFile.LoadedProject.ImageParameters.HeaderOffset = EventArgs.NewValue;
        });
    }

    void IdentityOrEGONameOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_IdentityHeader_IdentityOrEGONameOffset.X
                = ProjectFile.LoadedProject.ImageParameters.IdentityOrEGONameOffset
                = EventArgs.NewValue;
        });
    }

    void SinnerNameOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_IdentityHeader_SinnerNameOffset.X
                = ProjectFile.LoadedProject.ImageParameters.SinnerNameOffset
                = EventArgs.NewValue;
        });
    }

    void RarityOrEGORiskLevelOffset_Horizontal(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_IdentityHeader_IdentityRarityOffset.X
                = ProjectFile.LoadedProject.ImageParameters.RarityOrEGORiskLevelHorizontalOffset
                = EventArgs.NewValue;
        });
    }

    void RarityOrEGORiskLevelOffset_Vertical(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_IdentityHeader_IdentityRarityOffset.Y
                = ProjectFile.LoadedProject.ImageParameters.RarityOrEGORiskLevelVerticalOffset
                = EventArgs.NewValue;
        });
    }
    #endregion



    #region Sinner icon
    void SinnerIconBrightness(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_SinnerIcon.Opacity
                = ProjectFile.LoadedProject.Specific.IconBrightness
                = (double)(EventArgs.NewValue / 100);
        });
    }
    void SinnerIconSize(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_SinnerIcon.Width
                = ProjectFile.LoadedProject.Specific.IconSize
                = EventArgs.NewValue;
        });
    }
    #endregion



    #region Text area
    void TextBackgroundFadeoutSoftness(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_TextBackgroundFadeoutSoftness.Width
                = ProjectFile.LoadedProject.ImageParameters.TextBackgroundFadeoutSoftness
                = EventArgs.NewValue;
        });
    }

    void UnifiedTextSize(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() =>
        {
            UnifiedTextSizeBinder.FontSize
                = ProjectFile.LoadedProject.Text.UnifiedTextSize
                = EventArgs.NewValue;

            string ValueInsert = $"{EventArgs.NewValue}"; if (ValueInsert.Length == 2) ValueInsert += ",0";
            UnifiedTextSize_Label.Text = $"Unified text size [<color=#fc5a03>{EventArgs.NewValue}</color>]\n(Save and reload project or refresh text size via context menu for text item to apply)";
        });
    }

    void KeywordBoxesWidth(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() =>
        {
            KeywordBoxesWidthBinder.Width
                = ProjectFile.LoadedProject.Text.KeywordBoxesWidth
                = EventArgs.NewValue;
        });
    }
    #endregion



    #region Decorative cautions
    void CautionsBloomRadius(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            DecorativeCautions_BlurColor.BlurRadius
                = ProjectFile.LoadedProject.DecorativeCautions.CautionBloomRadius
                = EventArgs.NewValue;
        });
    }
    
    void CautionsOpacity(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewCreator_DecorativeCautions.Opacity
                = ProjectFile.LoadedProject.DecorativeCautions.CautionOpacity
                = (double)(EventArgs.NewValue / 100);
        });
    }
    
    void CautionsCustomTextVerticalOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            CustomCautionsParamBinder.Margin = new Thickness(0, EventArgs.NewValue / 100, 0, 0);

            ProjectFile.LoadedProject.DecorativeCautions.CustomText.TextVerticalOffset = EventArgs.NewValue / 100;
        });
    }

    void CautionsCustomTextSize(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            CustomCautionsParamBinder.FontSize = EventArgs.NewValue / 100;

            ProjectFile.LoadedProject.DecorativeCautions.CustomText.TextSize = EventArgs.NewValue / 100;
        });
    }
    #endregion



    #region Column settings
    void FirstColumn_SelfOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewItems_FirstColumn_ParentAndOffsetController.Margin = new Thickness(EventArgs.NewValue, 0, 0, 0);

            ProjectFile.LoadedProject.Text.FirstColumnOffset = EventArgs.NewValue;
        });
    }

    void SecondColumn_SelfOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            IdentityPreviewItems_SecondColumn_ParentAndOffsetController.Margin = new Thickness(EventArgs.NewValue, 0, 0, 0);

            ProjectFile.LoadedProject.Text.SecondColumnOffset = EventArgs.NewValue;
        });
    }

    void FirstColumn_ItemsSignaturesOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            //FirstColumn_ItemsSignaturesOffsetBinder.Margin = new Thickness(EventArgs.NewValue, 40, 0, 0);
            FirstColumn_ItemsSignaturesOffsetBinder.RenderTransform = new TranslateTransform(EventArgs.NewValue, 0);

            ProjectFile.LoadedProject.Text.FirstColumnItemSignaturesOffset = EventArgs.NewValue;
        });
    }

    void SecondColumn_ItemsSignaturesOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            //SecondColumn_ItemsSignaturesOffsetBinder.Margin = new Thickness(EventArgs.NewValue, 40, 0, 0);
            SecondColumn_ItemsSignaturesOffsetBinder.RenderTransform = new TranslateTransform(EventArgs.NewValue, 0);

            ProjectFile.LoadedProject.Text.SecondColumnItemSignaturesOffset = EventArgs.NewValue;
        });
    }

    #region Selected column item
    void ItemVerticalOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() =>
        {
            FocusedColumnItem.SetTopMargin(EventArgs.NewValue);
            FocusedColumnItem.ItemInfo.VerticalOffset = EventArgs.NewValue;
        });
    }

    void ItemHorizontalOffset(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() =>
        {
            if (FocusedColumnItem.Children.Count == 2)
            {
                if (FocusedColumnItem.ItemInfo.Type.EqualsOneOf("Passive", "Skill"))
                {
                    ((FocusedColumnItem.Children[1] as Grid).Children[1] as UIElement).RenderTransform = new TranslateTransform(EventArgs.NewValue, 0);
                                                                            // Skill   = Grid       = UIElement
                                                                            // Passive = StackPanel = UIElement
                }
                else
                {
                    FocusedColumnItem.SetLeftMargin(EventArgs.NewValue);
                }
            }
            FocusedColumnItem.ItemInfo.HorizontalOffset = EventArgs.NewValue;
        });
    }

    void NameMaximumWidth(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            if (!ColumnItemFocusingEvent & FocusedColumnItem.ItemNameLink != null)
            {                
                FocusedColumnItem.ItemNameLink.MaxWidth
                    = FocusedColumnItem.ItemInfo.NameMaxWidth
                    = EventArgs.NewValue;
            }
        });
    }

    void KeywordOrPassiveDescriptionWidth(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            if (!ColumnItemFocusingEvent & FocusedColumnItem.PassiveDescriptionLink != null)
            {
                FocusedColumnItem.PassiveDescriptionLink.Width
                    = FocusedColumnItem.ItemInfo.PassiveDescriptionWidth
                    = EventArgs.NewValue;
            }
        });
    }

    void Skill_MainDescriptionWidth(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            if (!ColumnItemFocusingEvent & FocusedColumnItem.SkillMainDescriptionLink != null)
            {
                FocusedColumnItem.SkillMainDescriptionLink.Width
                    = FocusedColumnItem.ItemInfo.SkillMainDescriptionWidth
                    = EventArgs.NewValue;
            }
        });
    }
    void Skill_CoinDescriptionsWidth(object RequestSender /* Slider */, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        Flow(() => {
            if (!ColumnItemFocusingEvent & FocusedColumnItem.SkillCoinDescriptionsLink != null)
            {
                FocusedColumnItem.SkillCoinDescriptionsLink.Width
                    = FocusedColumnItem.ItemInfo.SkillCoinsDescriptionWidth
                    = EventArgs.NewValue;
            }
        });
    }
    #endregion

    #endregion
}
