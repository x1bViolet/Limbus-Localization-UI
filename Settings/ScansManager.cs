using LC_Localization_Task_Absolute.Mode_Handlers;
using LC_Localization_Task_Absolute.PreviewCreator;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.Configurazione;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute
{
    public static class ScansManager
    {
        public static bool IsSkillsAreaViewEnabled = false;
        public static void ToggleSkillScanAreaView()
        {
            if (ActiveProperties.Key == EditorMode.Skills && MainControl.SkillsPreviewFreeBordersCanvas.ActualHeight > 0)
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
                    
                    if (LoadedProgramConfig.ScanParameters.AreaWidth != 0)
                    {
                        MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = LoadedProgramConfig.ScanParameters.AreaWidth;
                    }
                    else
                    {
                        MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;
                    }
                    
                    MainControl.ScanAreaView_Skills.BorderThickness = new Thickness(2);
                    MainControl.ScanAreaView_Skills.Background = ToSolidColorBrush("#E1121212");

                    MainControl.MakeLimbusPreviewScan.Opacity = 0.33;
                    MainControl.MakeLimbusPreviewScan.IsHitTestVisible = false;
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

                    MainControl.MakeLimbusPreviewScan.Opacity = 1;
                    MainControl.MakeLimbusPreviewScan.IsHitTestVisible = true;
                    IsSkillsAreaViewEnabled = false;
                }
            }
        }
    }

    public partial class MainWindow
    {
        #region Scans
        private void SavePreviewlayoutScan_TitlebarButtonClick(object RequestSender, RoutedEventArgs EventArgs)
        {
            SurfaceScrollPreview_Skills.ReconnectAsChildTo(SkillsPreviewFreeBordersCanvas);
            if (LoadedProgramConfig.ScanParameters.AreaWidth == 0)
            {
                PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;
            }
            else
            {
                PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = LoadedProgramConfig.ScanParameters.AreaWidth;
            }


            SavePreviewlayoutScan_Action();


            SurfaceScrollPreview_Skills.ReconnectAsChildTo(PreviewLayoutGrid_Skills);
            PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;
        }

        private void SavePreviewlayoutScan_Action()
        {
            string NameHint = "";
            string ManualPath = "";

            ScrollViewer CurrentTarget = null;

            if (@CurrentPreviewCreator.IsActive)
            {
                CurrentTarget = SeriousScrollViewer_1;

                SaveFileDialog OutputPathSelector = NewSaveFileDialog("Image files", ["jpg"]);
                OutputPathSelector.FileName = $"{DateTime.Now:HHːmmːss (dd.MM.yyyy)}.jpg";
                if (OutputPathSelector.ShowDialog() == true)
                {
                    ManualPath = OutputPathSelector.FileName;
                }
                else CurrentTarget = null;
            }
            else
            {
                switch (ActiveProperties.Key)
                {
                    case EditorMode.Skills:
                        CurrentTarget = SurfaceScrollPreview_Skills;
                        NameHint = $"{CurrentFile.Name.Replace(".json", "")}, " +
                                   $"ID {Mode_Skills.CurrentSkillID}" +
                                   (CurrentFile.Name.Contains("personality", comparisonType: StringComparison.OrdinalIgnoreCase) ? $", Uptie {Mode_Skills.CurrentSkillUptieLevel}" : "");
                        break;

                    case EditorMode.Passives:
                        CurrentTarget = SurfaceScrollPreview_Passives;
                        NameHint = $"{CurrentFile.Name.Replace(".json", "")}, " +
                                   $"ID {Mode_Passives.CurrentPassiveID}";
                        break;

                    case EditorMode.Keywords:
                        CurrentTarget =
                            PreviewLayoutGrid_Keywords_Sub_Bufs.Visibility == Visible
                                ? Scanable__PreviewLayout_Keywords_Bufs_Desc
                                : SurfaceScrollPreview_Keywords__BattleKeywords;
                        NameHint = $"{CurrentFile.Name.Replace(".json", "")}, " +
                                   $"ID {Mode_Keywords.CurrentKeywordID}";
                        break;

                    case EditorMode.EGOGifts:
                        if (CurrentFile != null)
                        {
                            CurrentTarget = SurfaceScrollPreview_EGOGifts;
                            NameHint = $"{CurrentFile.Name.Replace(".json", "")}, " +
                                       $"ID {Mode_EGOGifts.CurrentEGOGiftID}";
                        }
                        break;

                    default: break;
                }
                if (!Directory.Exists(@"[⇲] Assets Directory\Scans"))
                {
                    Directory.CreateDirectory(@"[⇲] Assets Directory\Scans");
                }
            }


            if (CurrentTarget != null)
            {
                bool ManuallyHiddenSelectors = false;
                if (FirstColumnItemsSelector.Visibility == Visible)
                {
                    FirstColumnItemsSelector.Visibility = Collapsed;
                    SecondColumnItemsSelector.Visibility = Collapsed;
                }
                else ManuallyHiddenSelectors = true;

                if (CurrentTarget == Scanable__PreviewLayout_Keywords_Bufs_Desc)
                {
                    BufsPreviewStackPanel.Children.Remove(PreviewLayout_Keywords_Bufs_Desc);
                    Scanable__PreviewLayout_Keywords_Bufs_Desc.Content = PreviewLayout_Keywords_Bufs_Desc;
                }

                ScanScrollviewer(CurrentTarget, NameHint, ManualPath);

                if (CurrentTarget == Scanable__PreviewLayout_Keywords_Bufs_Desc)
                {
                    Scanable__PreviewLayout_Keywords_Bufs_Desc.RemoveChild(PreviewLayout_Keywords_Bufs_Desc);
                    BufsPreviewStackPanel.Children.Add(PreviewLayout_Keywords_Bufs_Desc);
                }

                if (!ManuallyHiddenSelectors)
                {
                    FirstColumnItemsSelector.Visibility = Visible;
                    SecondColumnItemsSelector.Visibility = Visible;
                }
            }
        }
        #endregion
    }
}
