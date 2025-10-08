using LC_Localization_Task_Absolute.Limbus_Integration; // <- struct CompositionReference
using LC_Localization_Task_Absolute.PreviewCreator;
using Microsoft.Win32;
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
using System.Windows.Media.Imaging;
using System.Windows.Data;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    private void PortraitType(object RequestSender, SelectionChangedEventArgs EventArgs)
    {
        if (IsLoaded)
        {
            if ((EventArgs.AddedItems[0] as FrameworkElement).Uid == "E.G.O")
            {
                MainControl.PortraitsAndVignettes.Children[0].Visibility = Visibility.Collapsed;
                MainControl.PortraitsAndVignettes.Children[1].Visibility = Visibility.Visible;
                MakeUnavailable(IdentityPortraitImageParametersPanel);
                MakeAvailable(EGOPortraitImageParametersPanel, LeftBehindEGOPortraitVignetteSoftnessPanel, LeftBehindEGOPortraitVignettePlusLengthPanel);
            }
            else
            {
                MainControl.PortraitsAndVignettes.Children[0].Visibility = Visibility.Visible;
                MainControl.PortraitsAndVignettes.Children[1].Visibility = Visibility.Collapsed;
                MakeAvailable(IdentityPortraitImageParametersPanel);
                MakeUnavailable(EGOPortraitImageParametersPanel, LeftBehindEGOPortraitVignetteSoftnessPanel, LeftBehindEGOPortraitVignettePlusLengthPanel);
            }
        }
    }


    // To apply sinner color and set name
    private void SinnerIcon(object RequestSender, SelectionChangedEventArgs EventArgs)
    {
        if (IsLoaded)
        {
            if (EventArgs.AddedItems.Count > 0)
            {
                string SinnerName = (EventArgs.AddedItems[0] as StackPanel).Uid;
            
                if (SinnerName.EqualsOneOf("Yi Sang", "Faust", "Don Quixote", "Ryōshū", "Meursault", "Hong Lu", "Heathcliff", "Ishmael", "Rodion", "Sinclair", "Outis", "Gregor"))
                {
                    CustomSinnerIconComboBoxDisplay.Visibility = Visibility.Collapsed;
                    CustomSinnerIconComboBoxDisplay.Source = ((EventArgs.AddedItems[0] as StackPanel).Children[0] as Image).Source; // Display previously selected if clicking on [Custom]
                    if (!PreviewCreator.CurrentInfo.ImageInfoLoadingEvent)
                    {
                        // Change header color if identity portrait type
                        if (VC_PortraitType.SelectedIndex == 0)
                        {
                          _ = VC_Header_ColorInput.Text
                            = VC_CautionsColorInput.Text
                            = @ColorInfo.GetSinnerColor(SinnerName).Replace("#", "");
                        }

                        VC_Header_SinnerNameInput.Text = SinnerName;
                    }
                }
                else if (!PreviewCreator.CurrentInfo.ImageInfoLoadingEvent) // Custom (Uid = path to image)
                {
                    OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
                    if (Select.ShowDialog() == true)
                    {
                        SinnerIcon_SelectCustom(Select.FileName);
                    }
                }
            }
        }
    }

    public void SinnerIcon_SelectCustom(string ImagePath)
    {
        CustomSinnerIconSelectableOption.Uid = ImagePath;
        CustomSinnerIconComboBoxDisplay.Visibility = Visibility.Visible;
        CustomSinnerIconComboBoxDisplay.Source = BitmapFromFile(ImagePath);
    }

    // If this option is already selected (Same item selection does not trigger SelectionChanged event)
    private void SinnerIcon_SelectCustom_Again(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        if (VC_SinnerIcon.SelectedIndex == 12)
        {
            OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
            if (Select.ShowDialog() == true)
            {
                SinnerIcon_SelectCustom(Select.FileName);
            }
        }
    }

    private void TextBackgroundEffectsClipMode(object RequestSender, SelectionChangedEventArgs EventArgs)
    {
        if (IsLoaded)
        {
            BindingOperations.SetBinding(TextBackgroundEffectsClipIdentity, VisualBrush.VisualProperty, new Binding()
            {
                ElementName = (EventArgs.AddedItems[0] as UITranslation_Rose).Uid switch
                {
                    "Right vignette" => "RightIdentityVignette",
                    "All vignettes" => "Vignette_IDENTITY",
                }
            });
        }
    }

    private void SetWalpurgisNightMode_L(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        SetWalpurgisNightMode(!CurrentInfo.LoadedImageInfo.OtherEffects.WalpurgisNightLogoMode);
    }
    public void SetWalpurgisNightMode(bool Is)
    {
        CurrentInfo.LoadedImageInfo.OtherEffects.WalpurgisNightLogoMode = Is;
        if (Is)
        {
            ToggleWalpirgisVisible.Visibility = Visibility.Visible;
            WalpurgisFrames.Visibility = Visibility.Visible;

            CautionsGrid.Visibility = Visibility.Collapsed;
            Logo_RightBottomCorner.Visibility = Visibility.Collapsed;

            MakeUnavailable(DecorativeCautionsParametersPanel, BottomRightLogoParametersPanel);
        }
        else
        {
            ToggleWalpirgisVisible.Visibility = Visibility.Collapsed;
            WalpurgisFrames.Visibility = Visibility.Collapsed;
            
            CautionsGrid.Visibility = Visibility.Visible;
            Logo_RightBottomCorner.Visibility = Visibility.Visible;

            MakeAvailable(DecorativeCautionsParametersPanel, BottomRightLogoParametersPanel);
        }
    }
}
