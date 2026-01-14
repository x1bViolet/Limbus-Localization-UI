using System.Windows;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.MainWindow;

namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    public static class Upstairs
    {
        /// <summary>
        /// Used by <see cref="Limbus_Integration.LimbusPreviewFormatter.Apply(string, EditorMode)"/> to determine the necessary text conversions
        /// </summary>
        public enum EditorMode
        {
            /// <summary>Convert [KeywordID] only</summary>
            EGOGifts,

            /// <summary>Convert [KeywordID], [SkillTagID], and replace [SomethingElse] inside square brackets with 'Unknown'</summary>
            Skills,

            /// <summary>Same as <see cref="Skills"/></summary>
            Passives,

            /// <summary>Technically equals to the 'None', no conversions</summary>
            Keywords,

            /// <summary>Set by default for each <see cref="Limbus_Integration.TMProEmitter"/>, uses <see cref="ActiveProperties"/> 'Key' that reflects current editor mode</summary>
            UseCurrentActiveProperties
        }

        public enum TripleDescriptionType
        {
            Main,
            Summary,
            Flavor
        }

        public static SwitchedInterfaceProperties ActiveProperties = new()
        {
            Key = EditorMode.EGOGifts,
            WindowSizesInfo = new WindowSizesConfig()
            {
                Height = 550,
                Width = 1000,

                MinHeight = 464,
                MinWidth = 709.8,

                MaxWidth = 1000,
            }
        };
        public static void AdjustUIToRecent()
        {
            // If main menu (Default mode is EditorMode.EGOGifts and main menu is `null` CurrentFile)
            if (ActiveProperties.Key == EditorMode.EGOGifts & MainWindow.CurrentFile == null)
            {
                MainControl.MaxWidth = 1000;
                MainControl.MaxHeight = 10000;

                MainControl.Height = 550;
                MainControl.Width = 1000;
            }
            else
            {
                AdjustUI(ActiveProperties.WindowSizesInfo);
            }
        }


        public record SwitchedInterfaceProperties
        {
            public EditorMode Key { get; set; }
            public WindowSizesConfig WindowSizesInfo { get; set; }
        }

        public record WindowSizesConfig
        {
            public double Height    { get; set; }
            public double Width     { get; set; }
            public double MinHeight { get; set; }
            public double MinWidth  { get; set; }
            public double MaxWidth  { get; set; }
        }

        public static void AdjustUI(WindowSizesConfig From)
        {
             MainControl.MinWidth = From.MinWidth;
             MainControl.MaxWidth = From.MaxWidth;
            MainControl.MinHeight = From.MinHeight;
                MainControl.Width = From.Width;
               MainControl.Height = From.Height;
        }


        public static void HideNavigationPanelButtons(StackPanel ExceptButtonsPanel, Grid ExceptPreviewLayout)
        {
            foreach (UIElement PreviewLayoutChild in MainControl.PreviewLayouts.Children)
            {
                if (!PreviewLayoutChild.Equals(ExceptPreviewLayout)) PreviewLayoutChild.Visibility = Visibility.Collapsed;
                else PreviewLayoutChild.Visibility = Visibility.Visible;
            }

            foreach (UIElement SwitchButtonsChild in MainControl.SwitchButtons_MainGrid.Children)
            {
                if (!SwitchButtonsChild.Equals(ExceptButtonsPanel)) SwitchButtonsChild.Visibility = Visibility.Collapsed;
                else SwitchButtonsChild.Visibility = Visibility.Visible;
            }
        }
    }
}