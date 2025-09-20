using LC_Localization_Task_Absolute.Mode_Handlers;
using System.Windows;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute
{
    public abstract class ScansManager
    {
        public static bool IsSkillsAreaViewEnabled = false;
        public static void ToggleSkillScanAreaView()
        {
            if (Mode_Handlers.Upstairs.ActiveProperties.Key == "Skills")
            {
                if (MainControl.SkillsPreviewFreeBordersCanvas.ActualHeight > 0)
                {
                    if (MainControl.ScanAreaView_Skills.BorderThickness.Top == 0)
                    {
                        // ACTIVATE
                        MainControl.SurfaceScrollPreview_Skills.ReconnectAsChildTo(MainControl.SkillsPreviewFreeBordersCanvas);
                        try
                        {
                            MainControl.SurfaceScrollPreview_Skills.Height = MainControl.SkillsPreviewFreeBordersCanvas.ActualHeight - 60;
                        }
                        catch { }
                    
                        if (Configurazione.DeltaConfig.ScanParameters.AreaWidth != 0)
                        {
                            MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Configurazione.DeltaConfig.ScanParameters.AreaWidth;
                        }
                        else
                        {
                            MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;
                        }
                    
                        MainControl.ScanAreaView_Skills.BorderThickness = new Thickness(2);
                        MainControl.ScanAreaView_Skills.Background = ToSolidColorBrush("#E1121212");

                        MainControl.PreviewScanButtonIndicator.Foreground = ToSolidColorBrush("#FF383838");
                        MainControl.MakeLimbusPreviewScan.IsHitTestVisible = false;

                        SettingsWindow.SettingsControl.ToggleScansPreview_I.Visibility = Visible;
                        IsSkillsAreaViewEnabled = true;
                    }
                    else
                    {
                        // HIDE
                        MainControl.SurfaceScrollPreview_Skills.ReconnectAsChildTo(MainControl.PreviewLayoutGrid_Skills);
                        MainControl.SurfaceScrollPreview_Skills.Height = double.NaN;

                        MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;

                        MainControl.ScanAreaView_Skills.BorderThickness = new Thickness(0);
                        MainControl.ScanAreaView_Skills.Background = ToSolidColorBrush("#00121212");

                        MainControl.PreviewScanButtonIndicator.Foreground = ToSolidColorBrush("#FF9D9D9D");
                        MainControl.MakeLimbusPreviewScan.IsHitTestVisible = true;

                        SettingsWindow.SettingsControl.ToggleScansPreview_I.Visibility = Collapsed;
                        IsSkillsAreaViewEnabled = false;
                    }
                }
            }
        }
    }
}
