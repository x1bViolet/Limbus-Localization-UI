using LC_Localization_Task_Absolute.PreviewCreator; // <- struct CompositionReference
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.PreviewCreator.CompositionData_PROP.TextColumns_PROP;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    // Toggle section visibility
    private void ToggleSecondChildVisibility(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        UIElement Target = ((RequestSender as UITranslation_Rose).Parent as StackPanel).Children[1] as UIElement;
        Target.Visibility = Target.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }

    #region Resets
    private void ResetSliderValue(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        if (Keyboard.IsKeyDown(Key.LeftShift))
        {
            Slider Target = RequestSender as Slider;
            Target.Value = Target.TickFrequency;
        }
    }
    private void ResetSelectedFile(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        if (Keyboard.IsKeyDown(Key.LeftShift))
        {
            switch ((RequestSender as Border).Name)
            {
                case nameof(SelectPortraitImage_Button): SelectPortraitImage_Action(""); break;
                case nameof(SelectImageLabelFont_Button): SelectImageLabelFont_Action("#Bebas Neue Bold"); break;
                case nameof(SelectCautionsFont_Button): SelectCautionsFont_Action("#Bebas Neue Bold"); break;
                case nameof(SelectItemSignaturesFont_Button): SelectItemSignaturesFont_Action("#Bebas Neue Bold"); break;
                case nameof(SelectTextBackgroundEffectsImage_Button): SelectTextBackgroundEffectsImage_Action(""); break;
                case nameof(SelectOverlaySketchImage_Button): SelectOverlaySketchImage_Action(""); break;
                case nameof(SelectUpperLeftLogoImage_Button): SelectUpperLeftLogoImage_Action(""); break;
                case nameof(SelectBottomRightLogoImage_Button): SelectBottomRightLogoImage_Action(""); break;
                case nameof(SelectSkillsLocalization_Button): SelectSkillsLocalization_Actor(""); break;
                case nameof(SelectSkillsDisplayInfo_Button): SelectSkillsDisplayInfo_Actor(""); break;
                case nameof(SelectPassivesLocalization_Button): SelectPassivesLocalization_Actor(""); break;
                case nameof(SelectKeywordsLocalization_Button): SelectKeywordsLocalization_Actor(""); break;
                case nameof(FocusedColumnElement_SelectKeywordIcon_Button): FocusedColumnElement_SelectKeywordIcon_Action(""); break;
            }
        }
    }
    #endregion
    
    #region Vignettes highlight animations
    private void OpacityAnimation_In(string LinearGradientResourceName)
    {
        if (IsLoaded)
        {
            (CompositionGrid.Resources[LinearGradientResourceName] as LinearGradientBrush).BeginAnimation(LinearGradientBrush.OpacityProperty, new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(0.09))
            });
        }
    }
    private void OpacityAnimation_Out(string LinearGradientResourceName)
    {
        if (IsLoaded)
        {
            (CompositionGrid.Resources[LinearGradientResourceName] as LinearGradientBrush).BeginAnimation(LinearGradientBrush.OpacityProperty, new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.09))
            });
        }
    }

    private void HighlightVignette_LeftTopBottom(object RequestSender, MouseEventArgs EventArgs)
    {
        OpacityAnimation_In("VignettesViewBorder_Left");
        OpacityAnimation_In("VignettesViewBorder_Top");
        OpacityAnimation_In("VignettesViewBorder_Bottom");
    }
    private void UnHighlightVignette_LeftTopBottom(object RequestSender, MouseEventArgs EventArgs)
    {
        OpacityAnimation_Out("VignettesViewBorder_Left");
        OpacityAnimation_Out("VignettesViewBorder_Top");
        OpacityAnimation_Out("VignettesViewBorder_Bottom");
    }

    private void HighlightVignette_Right(object RequestSender, MouseEventArgs EventArgs) => OpacityAnimation_In("VignettesViewBorder_Right");
    private void UnHighlightVignette_Right(object RequestSender, MouseEventArgs EventArgs) => OpacityAnimation_Out("VignettesViewBorder_Right");

    private void HighlightVignette_Top(object RequestSender, MouseEventArgs EventArgs) => OpacityAnimation_In("VignettesViewBorder_Top");
    private void UnHighlightVignette_Top(object RequestSender, MouseEventArgs EventArgs) => OpacityAnimation_Out("VignettesViewBorder_Top");

    private void HighlightVignette_Bottom(object RequestSender, MouseEventArgs EventArgs) => OpacityAnimation_In("VignettesViewBorder_Bottom");
    private void UnHighlightVignette_Bottom(object RequestSender, MouseEventArgs EventArgs) => OpacityAnimation_Out("VignettesViewBorder_Bottom");

    private void HighlightVignette_Left(object RequestSender, MouseEventArgs EventArgs) => OpacityAnimation_In("VignettesViewBorder_Left");
    private void UnHighlightVignette_Left(object RequestSender, MouseEventArgs EventArgs) => OpacityAnimation_Out("VignettesViewBorder_Left");

    private void HighlightVignette_LeftBehindEGOPortrait(object RequestSender, MouseEventArgs EventArgs) => OpacityAnimation_In("VignettesViewBorder_LeftBehindEGOPortrait");
    private void UnHighlightVignette_LeftBehindEGOPortrait(object RequestSender, MouseEventArgs EventArgs) => OpacityAnimation_Out("VignettesViewBorder_LeftBehindEGOPortrait");
    #endregion
}