using System.Windows;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.MainWindow;

namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    public abstract class Upstairs
    {
        public static SwitchedInterfaceProperties ActiveProperties = new SwitchedInterfaceProperties()
        {
            Key = "E.G.O Gifts",
            DefaultValues = new DefaultValues()
            {
                Height = 550,
                Width = 1000,

                MinHeight = 464,
                MinWidth = 709.8,

                MaxHeight = 10000,
                MaxWidth = 1000,
            },
        };
        public static void TriggerSwitchToRecent()
        {
            switch (ActiveProperties.Key)
            {
                case "E.G.O Gifts":

                    if (MainWindow.CurrentFile != null)
                    {
                        Mode_EGOGifts.TriggerSwitch();
                    }
                    else
                    {
                        MainControl.MaxWidth = 1000;
                        MainControl.MaxHeight = 10000;

                        MainControl.Height = 550;
                        MainControl.Width = 1000;
                    }
                    break;

                case "Keywords": Mode_Keywords.TriggerSwitch(); break;
                case "Passives": Mode_Passives.TriggerSwitch(); break;
                case "Skills": Mode_Skills.TriggerSwitch(
                    Mode_Skills.EnableUptieLevels_Recent, Mode_Skills.EnableEGOAbnormalityName_Recent
                ); break;
            }
        }

        public record SwitchedInterfaceProperties
        {
            public string Key { get; set; }
            public DefaultValues DefaultValues { get; set; }
        }

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
             MainControl.MinWidth = From.MinWidth;
             MainControl.MaxWidth = From.MaxWidth;
            MainControl.MaxHeight = From.MaxHeight;
            MainControl.MinHeight = From.MinHeight;
                MainControl.Width = From.Width;
               MainControl.Height = From.Height;

            LockEditorUndo();
        }

        public static void HideNavigationPanelButtons(Grid ExceptButtonsGrid, Grid ExceptPreviewLayout)
        {
            foreach (Grid PreviewLayoutChild in MainControl.PreviewLayouts.Children)
            {
                if (!PreviewLayoutChild.Equals(ExceptPreviewLayout)) PreviewLayoutChild.Visibility = Visibility.Collapsed;
                else PreviewLayoutChild.Visibility = Visibility.Visible;
            }

            foreach (Grid SwitchButtonsChild in MainControl.SwitchButtons_MainGrid.Children)
            {
                if (!SwitchButtonsChild.Equals(ExceptButtonsGrid)) SwitchButtonsChild.Visibility = Visibility.Collapsed;
                else SwitchButtonsChild.Visibility = Visibility.Visible;
            }
        }
    }
}
