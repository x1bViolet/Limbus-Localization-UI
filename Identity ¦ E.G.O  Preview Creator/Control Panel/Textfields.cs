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
using Microsoft.Win32;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.MainWindow;
using LC_Localization_Task_Absolute.PreviewCreator;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    // Colors applying

    private void HeaderColorInput(object RequestSender, TextChangedEventArgs EventArgs)
    {
        if (IsLoaded && TryParseColor((RequestSender as UITranslation_Mint).Text, out Color Color))
        {
            HeaderColorInput_Display.Background = new SolidColorBrush(Color);

            Header_SinnerName.Source = Tint(BitmapFromResource("Identity ¦ E.G.O  Preview Creator/Images/Header/Sinner Name.png"), Color);
            Header_IdentityOrEGOName.Source = Tint(BitmapFromResource("Identity ¦ E.G.O  Preview Creator/Images/Header/Identity or E.G.O Name.png"), Color);
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
            EGOPortrait_Frame.Source = Tint(BitmapFromResource("Identity ¦ E.G.O  Preview Creator/Images/E.G.O Frame.png"), Color);
        }
    }
}
