using System.Windows;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.MainWindow;

namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    public abstract class Shared
    {
        public record DefaultValues
        {
            public double Height    { get; set; }
            public double Width     { get; set; }
            public double MinHeight { get; set; }
            public double MinWidth  { get; set; }
            public double MaxHeight { get; set; }
            public double MaxWidth  { get; set; }
        }

        public static void AdjustUI(DefaultValues From)
        {
            MainControl.Width     = From.Width;
            MainControl.Height    = From.Height;
            MainControl.MinWidth  = From.MinWidth;
            MainControl.MaxWidth  = From.MaxWidth;
            MainControl.MaxHeight = From.MaxHeight;
            MainControl.MinHeight = From.MinHeight;
        }

        public static void HideAllPreviewLayouts()
        {
            foreach(Grid PreviewLayoutChild in MainControl.PreviewLayouts.Children)
            {
                PreviewLayoutChild.Visibility = Visibility.Collapsed;
            }
        }
    }
}
