using LCLocalizationInterface.Internal.Configuration;
using LCLocalizationInterface.Internal.UIStyle;
using LCLocalizationInterface.LimbusRegistry.PreviewCreator;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        public bool CanDragMove = true;
        private void MainWindowOverrideDragMove(object Sender, MouseButtonEventArgs Args)
        {
            if (CanDragMove)
            {
                // WHEN WINDOW ALREADY WAS MAXIMIZED TO THE LEFT TOP CORNER and now dragging again -> exit maximized state
                if (WindowState == WindowState.Maximized)
                {
                    ExitPseudoMaximizedState();
                }

                try { this.DragMove(); }
                catch (InvalidOperationException) { /* Idk */ }

                // If window was maximized after dragging
                CheckPseudoMaximizedStatePadding();
            }
        }
        public void ExitPseudoMaximizedState()
        {
            this.WindowState = WindowState.Normal; this.Left = 0; this.Top = 0;
        }
        public void CheckPseudoMaximizedStatePadding()
        {
            VisualBorder.Padding = new Thickness(0, this.WindowState is WindowState.Maximized ? 4 : 0, 0, 0);
        }


        private void ShowSettings(object Sender, RoutedEventArgs Args)
        {
            SettingsWindow.SettingsWindowInstance.BeginFadeShowing();

            // Show over this window
            SettingsWindow.SettingsWindowInstance.WindowState = WindowState.Minimized;
            SettingsWindow.SettingsWindowInstance.WindowState = WindowState.Normal;
        }


        private void MakeLimbusTextPreviewScanButton_Click(object Sender, RoutedEventArgs Args)
        {
            if (@CurrentPreviewCreator.ActiveState == false)
            {
                if (Directory.Exists(@"[⇲] Assets Directory\Scans") == false) Directory.CreateDirectory(@"[⇲] Assets Directory\Scans");

                @EditorModesShelf.CurrentEditorMode.ScreenshotRichText();
            }
            else
            {
                ZoomableScrollViewer Target = PreviewCreatorPageInstance.CompositionScrollViewer;

                _ = PreviewCreatorPageInstance.FirstColumnItemsSelector.Visibility
                  = PreviewCreatorPageInstance.SecondColumnItemsSelector.Visibility
                  = Visibility.Collapsed;

                double OriginalZoom = Target.CurrentElementTransform.ScaleX;
                _ = Target.CurrentElementTransform.ScaleX
                  = Target.CurrentElementTransform.ScaleY
                  = 1.0;

                (double OriginalHOffset, double OriginalVOffset) = (Target.HorizontalOffset, Target.VerticalOffset);
                Target.ScrollToHome();

                PreviewCreatorPageInstance.UnsealAllTextElementsInBothColumns();
                PreviewCreatorPageInstance.UnsealCautions();

                BitmapEncoder Screenshot = (Target.Content as FrameworkElement)!.RenderImage(LoadedConfiguration.ScanParameters.ScaleFactor, UseJpegEncoder: true);

                _ = Target.CurrentElementTransform.ScaleX
                  = Target.CurrentElementTransform.ScaleY
                  = OriginalZoom;

                Target.ScrollToHorizontalOffset(OriginalHOffset);
                Target.ScrollToVerticalOffset(OriginalVOffset);
                
                PreviewCreatorPageInstance.SealAllTextElementsInBothColumns();
                PreviewCreatorPageInstance.SealCautions();


                if (PreviewCreator_ColumnButtonsWasManuallyHidden == false)
                {
                    _ = PreviewCreatorPageInstance.FirstColumnItemsSelector.Visibility
                      = PreviewCreatorPageInstance.SecondColumnItemsSelector.Visibility
                      = Visibility.Visible;
                }

                SaveFileDialog SaveLocation = NewSaveFileDialog("Image files", ["jpg"], $"{DateTime.Now:HHːmmːss (dd.MM.yyyy)}.jpg");
                if (SaveLocation.ShowDialog() == true)
                {
                    Screenshot.SaveToImage(SaveLocation.FileName);
                }
            }
        }
        private void MakeLimbusTextPreviewScanButton_ToolTipOpening(object Sender, ToolTipEventArgs Args)
        {
            Args.Handled = @CurrentPreviewCreator.ActiveState == true;
        }


        private void PreviewCreatorUISwitchButton_Click(object Sender, RoutedEventArgs Args)
        {
            if (@CurrentPreviewCreator.ActiveState == true)
            {
                PreviewCreatorPage.SwitchUI_Return();
            }
            else
            {
                if (WindowState == WindowState.Maximized)
                {
                    ExitPseudoMaximizedState();
                    CheckPseudoMaximizedStatePadding();
                }

                PreviewCreatorPage.SwitchUI_Activate();
            }
        }


        private void ShowSkillDisplayInfoManagerWindow(object Sender, RoutedEventArgs Args)
        {
            SkillsDisplayInfoManagerWindow.SkillsDisplayInfoManagerWindowInstance.Show();
            SkillsDisplayInfoManagerWindow.SkillsDisplayInfoManagerWindowInstance.WindowState = WindowState.Normal;
        }
    }
}