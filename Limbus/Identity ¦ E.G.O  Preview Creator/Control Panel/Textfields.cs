using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LC_Localization_Task_Absolute.UITranslationHandlers;
using static LC_Localization_Task_Absolute.Requirements;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    // Colors applying
    private void HeaderColorInput(object RequestSender, TextChangedEventArgs EventArgs)
    {
        if (IsLoaded && TryParseColor((RequestSender as UITranslation_Mint).Text, out Color Color))
        {
            HeaderColorInput_Display.Background = new SolidColorBrush(Color);

            Header_SinnerName.Source = Tint(BitmapFromResource("Limbus/Identity ¦ E.G.O  Preview Creator/Images/Header/Sinner Name.png"), Color);
            Header_IdentityOrEGOName.Source = Tint(BitmapFromResource("Limbus/Identity ¦ E.G.O  Preview Creator/Images/Header/Identity or E.G.O Name.png"), Color);
        }
    }
    private void CautionsColorInput(object RequestSender, TextChangedEventArgs EventArgs)
    {
        if (IsLoaded && TryParseColor((RequestSender as UITranslation_Mint).Text, out Color Color))
        {
          _ = CautionsColorInput_Dispaly.Background
            = DecorativeCautions_PropertyBindingSource.Foreground
            = new SolidColorBrush(Color);
        }
    }
    private void EGOPortrait_FrameColorInput(object RequestSender, TextChangedEventArgs EventArgs)
    {
        if (IsLoaded && TryParseColor((RequestSender as UITranslation_Mint).Text, out Color Color))
        {
            EGOPortrait_FrameColorInput_Display.Background = new SolidColorBrush(Color);
            EGOPortrait_Frame.Source = Tint(BitmapFromResource("Limbus/Identity ¦ E.G.O  Preview Creator/Images/E.G.O Frame.png"), Color);
        }
    }


    readonly Regex HexColorCharPattern = new(@"[A-F0-9]", RegexOptions.IgnoreCase);
    private void ValidateInputText(object RequestSender, TextCompositionEventArgs EventArgs)
    {
        switch ((RequestSender as UITranslation_Mint).Name)
        {
            case nameof(VC_EGOPortrait_FrameColorInput):
                EventArgs.HandleIfNotMatches(HexColorCharPattern);
                break;

            case nameof(VC_Header_ColorInput):
                EventArgs.HandleIfNotMatches(HexColorCharPattern);
                break;

            case nameof(VC_CautionsColorInput):
                EventArgs.HandleIfNotMatches(HexColorCharPattern);
                break;

            default: break;
        }
    }
}
