using LC_Localization_Task_Absolute.Mode_Handlers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Windows.Visibility;
using SixLabors.ImageSharp.PixelFormats;
using System.Windows;
using System.Reflection;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    /*/—------------------------------------------------------------------------------------—/*/
    /*/ File for TextChanged triggers of TextBoxes from Custom Identity Preview Creator mode /*/
    /*/—------------------------------------------------------------------------------------—/*/
    #region Image parameters
    void ChangeImageTypeText(object RequestSender /* Textfield */, TextChangedEventArgs EventArgs)
    {
        Flow(() =>
        {
            CustomIdentityPreviewCreator_ImageTypeText.Text
                = ProjectFile.LoadedProject.ImageParameters.ImageTypeSign
                = (RequestSender as TextBox).Text;
        });
    }
    #endregion



    #region Specific of the sinner and Identity/E.G.O
    void ChangeSinnerName(object RequestSender /* Textfield */, TextChangedEventArgs EventArgs)
    {
        Flow(() =>
        {
            IdentityPreviewCreator_IdentityHeader_SinnerName_Text.Text
                = ProjectFile.LoadedProject.Specific.SinnerName
                = IdentityPreviewCreator_TextEntries_SinnerName.Text;
        });
    }

    void ChangeIdentityOrEGOName(object RequestSender /* Textfield */, TextChangedEventArgs EventArgs)
    {
        IdentityPreviewCreator_IdentityHeader_IdentityName_Text.Text
                = ProjectFile.LoadedProject.Specific.IdentityOrEGOName
                = IdentityPreviewCreator_TextEntries_IdentityOrEGOName.Text;
    }

    void ChangeAmbienceColor(object RequestSender /* Textfield */, TextChangedEventArgs EventArgs)
    {
        Flow(() =>
        {
            string NewColor = IdentityPreviewCreator_TextEntries_ElementsColor.Text.Replace("#", "");

            if (NewColor.Length == 6)
            {
                try
                {
                    var СorrectnessCheck = Rgba32.ParseHex(IdentityPreviewCreator_TextEntries_ElementsColor.Text); // -> catch { } if fail

                    Color AlternateElementsColor = Color.FromRgb(NewColor[0..2].ToByte(), NewColor[2..4].ToByte(), NewColor[4..6].ToByte());

                    SolidColorBrush SolidColor = ToSolidColorBrush($"#{NewColor}");


                    // Fading color indicator near textfield
                    IdentityPreviewCreator_TextEntries_ElementsColor_SubIndicator.Color = AlternateElementsColor;

                    // Custom cautions
                    CustomCautionsParamBinder.Foreground = SolidColor;
                    DecorativeCautions_BlurColor.Color = AlternateElementsColor;

                    // Sinner and Identity/E.G.O name background color
                    SinnerName_Background.Source = CustomIdentityPreviewCreator.CreateColoredHeader(NewColor, "Sinner");
                    IdentityName_Background.Source = CustomIdentityPreviewCreator.CreateColoredHeader(NewColor, "Identity or E.G.O");

                    // E.G.O frame color
                    EGOFrameImage.Source = CustomIdentityPreviewCreator.EGOPlainFrame;
                    EGOFrameImage.Source = SecondaryUtilities.TintWhiteMaskBitmap(CustomIdentityPreviewCreator.EGOPlainFrame, NewColor);
                    EGOPortraitImage_ParentGrid_ColorFill.Background = SolidColor;
                    // Update decorative cautions (But not at project loading while changing ambience color first and calling this method)
                    if (CanSelectDecorativeCautions) SelectCautionsType(IdentityPreviewCreator_CautionTypeSelector, null);

                    CustomIdentityPreviewCreator.ProjectFile.LoadedProject.Specific.AmbienceColor = $"#{NewColor}";
                }
                catch { }
            }
        });
    }
    #endregion



    #region Decorative cautions
    void SetCustomCautionsText(object RequestSender /* Textfield */, TextChangedEventArgs EventArgs)
    {
        Flow(() =>
        {
            CustomCautionsParamBinder.Text
                = ProjectFile.LoadedProject.DecorativeCautions.CustomText.CustomCautionString
                = (RequestSender as TextBox).Text;
        });
    }
    #endregion



    #region Selected text item
    void ChangeTextItemSignature(object RequestSender /* Textfield */, TextChangedEventArgs EventArgs)
    {
        Flow(() =>
        {
            if (FocusedColumnItem.ItemSignaruteTextBlockLink != null & !ColumnItemFocusingEvent)
            {
                FocusedColumnItem.ItemSignaruteTextBlockLink.Text
                    = FocusedColumnItem.ItemInfo.TextItemSignature
                    = (RequestSender as TextBox).Text;
            }
        });
    }
    #endregion
}