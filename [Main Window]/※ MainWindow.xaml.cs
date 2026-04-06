using LCLocalizationInterface.Instruments;
using LCLocalizationInterface.Internal.Abstractions;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using LCLocalizationInterface.LimbusRegistry.PreviewCreator;
using System.Diagnostics;
using System.Dynamic;

namespace LCLocalizationInterface
{
    public partial class MainWindow : FadeableWindow
    {
        #pragma warning disable CS8618
        public static MainWindow MainWindowInstance;
        #pragma warning restore CS8618

        public MainWindow()
        {
            InitializeComponent();
            MainWindowInstance = this;

            DoStartupActions();

            this.Loaded += (_, _) => App.CheckLatestVersion();
        }


        private void DoStartupActions()
        {
            Skills_SetupCoinsView();
            EGOGifts_SetupSimpleDescsView();

            SetupIDCopyAnimation();

            // E.G.O Gift icons to use in ui translation
            ImageDictionaries.ReadEGOGiftsDisplayInfo();
            ImageDictionaries.ReadKeywordIconsRedirectionJsonFiles();
        }



        #region FadeableWindow things
        protected override (double In, double Out) FadeDurations => AsPair(ThemeTimings.Duration.MainWindow);
        protected override (double In, double Out) FadeSpeedRatios => AsPair(ThemeTimings.SpeedRatio.MainWindow);
        protected override ((double Acceleration, double Deceleation) In, (double Acceleration, double Deceleation) Out) FadeKinematics
            => AsPairPair(ThemeTimings.AccelerationDecelerationRatios.MainWindow);


        #region Closing logic
        public override List<Action> AdditionalFadeOutCompleteActions => [ Application.Current.Shutdown ]; /// <-- <see cref="FadeableWindow.AdditionalFadeOutCompleteActions"/>

        public override bool PreventClosingByDefaultLogic => false; /// <-- <see cref="FadeableWindow.PreventClosingByDefaultLogic"/>, skip default OnClosing cancel
        protected override void OnClosing(CancelEventArgs Args) { Args.Cancel = true; this.ExplicitClose(); }
        protected override void FadeClose(object Sender, RoutedEventArgs Args) => this.ExplicitClose();
        #endregion

        #endregion



        private void DisableWindowChromeResizeBorder(object Sender, MouseEventArgs Args)
        {
            if (@CurrentPreviewCreator.ActiveState == false)
            {
                CurrentWindowChrome.ResizeBorderThickness = new Thickness(0);
            }
        }
        private void EnableWindowChromeResizeBorder(object Sender, MouseEventArgs Args)
        {
            if (@CurrentPreviewCreator.ActiveState == false)
            {
                CurrentWindowChrome.ResizeBorderThickness = new Thickness(10);
            }
        }



        #region Current json file
        private void OpenJsonFileButton_Click(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Limbus localization files (Skills/Passives/Keywords/E.G.O Gifts)", ["json"]);

            if (Select.ShowDialog() == true) OpenJsonFile_Action(Select.FileName);
        }

        /// <summary>Checks for unsaved changes in target editor mode before file opening</summary>
        private void OpenJsonFile_Action(string FilePath)
        {
            FileInfo SelectedFile = new(fileName: FilePath);

            if (SelectedFile.TryDeserealizeJsonAs(out LimbusLocalizationFile<object> TestStructure, out Exception Occurred))
            {
                if (TestStructure.DataList.Count > 0)
                {
                    string TypeCheckName = SelectedFile.Name.RemovePrefix("JP_", "KR_", "EN_");
                    if (TestStructure.TryMatchManualFileType(out string AcquiredManualFileType)) TypeCheckName = AcquiredManualFileType;

                    @EditorModesShelf.Types.EditorModeIntermediator? AssociatedEditor = @EditorModesShelf.ModesMapping
                        .FirstOrDefault(Mode => TypeCheckName.StartsWithOneOf(Mode.PossibleFileNameStarts))?.AssociatedMode;


                    if (AssociatedEditor is not null)
                    {
                        void SwitchToAssociatedModeAndLoadFile() => AssociatedEditor.TryValidateJsonAndSwitchMode(SelectedFile, TypeCheckName);

                        // Check for unsaved changes in target editor mode if it has file opened
                        if (AssociatedEditor.CurrentFile is not null)
                        {
                            (bool IsAnyUnsavedChanges, string UnsavedChangesText) = AssociatedEditor.CollectUnsavedChanges();
                            if (IsAnyUnsavedChanges)
                            {
                                if (@EditorModesShelf.CurrentEditorMode != AssociatedEditor)
                                {
                                    ModeSwitchContextMenu_Click(
                                        Sender: MainWindowInstance.FindName($"ModeSwitchButton_{AssociatedEditor.Identifier}"),
                                        Args: null!
                                    );
                                }

                                UnsavedChangesDialog_ShowMenu(Text: UnsavedChangesText, ProceedAction: SwitchToAssociatedModeAndLoadFile);
                                // -> Info and "Proceed" / "Cancel" buttons 
                            }
                            else
                            {
                                SwitchToAssociatedModeAndLoadFile();
                            }
                        }
                        else
                        {
                            SwitchToAssociatedModeAndLoadFile();
                        }
                    }
                }
            }
            else
            {
                ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to open localization file <b>\"{SelectedFile.Name}\"</b>");
            }
        }


        private void JsonFilePath_ContextMenuClick(object Sender, RoutedEventArgs Args)
        {
            if (@EditorModesShelf.CurrentEditorMode.CurrentFile is not null)
            {
                switch ((Sender as MenuItem_T1)!.HeaderText.UID)
                {
                    case "[Main UI] * Json Path — 'Open with external editor' context menu option":
                        using (Process FileOpener = new() { StartInfo = new() { FileName = "explorer" } })
                        {
                            FileOpener.StartInfo.Arguments = $"\"{@EditorModesShelf.CurrentEditorMode.CurrentFile.FullName}\"";
                            FileOpener.Start();
                        }
                        break;


                    case "[Main UI] * Json Path — 'Copy file text' context menu option":
                        string Text = @EditorModesShelf.CurrentEditorMode.RecentlySerializedJsonText ?? File.ReadAllText(@EditorModesShelf.CurrentEditorMode.CurrentFile.FullName);
                        Clipboard.SetDataObject(Text);
                        break;


                    case "[Main UI] * Json Path — 'Copy file itself' context menu option":
                        Clipboard.SetFileDropList([@EditorModesShelf.CurrentEditorMode.CurrentFile.FullName]);
                        break;


                    case "[Main UI] * Json Path — 'Copy full path' context menu option":
                        Clipboard.SetDataObject(@EditorModesShelf.CurrentEditorMode.CurrentFile.FullName);
                        break;


                    case "[Main UI] * Json Path — 'Copy name' context menu option":
                        Clipboard.SetDataObject(@EditorModesShelf.CurrentEditorMode.CurrentFile.Name);
                        break;
                }
            }
        }

        private void JsonPathText_TextChanged(object Sender, EventArgs Args)
        {
            @Languages.PresentedTextFields["[Main UI] * Json Path"].ScrollToHorizontalOffset(double.PositiveInfinity);
        }
        private void JsonPathText_ContextMenuOpening(object Sender, ContextMenuEventArgs Args)
        {
            Args.Handled = @EditorModesShelf.CurrentEditorMode == @EditorModesShelf.MainMenu;
        }
        #endregion



        /// <summary>Check for unsaved changes before calling <see cref="FadeableWindow.BeginFadeHiding"/></summary>
        private void ExplicitClose()
        {
            if (PreviewCreatorPage.PreviewCreatorPageInstance.CurrentImageInfoJson != PreviewCreatorPage.PreviewCreatorPageInstance.RecentlySerializedImageInfoJson)
            {
                if (@CurrentPreviewCreator.ActiveState == false)
                {
                    PreviewCreatorPage.SwitchUI_Activate();
                }

                PreviewCreatorPage.PreviewCreatorPageInstance.UnsavedChangesDialog_ShowMenu(ProceedAction: delegate ()
                {
                    UnsavedChangesCheckingInEditors();
                    return true; // Hide preview creator unsaved changes view
                });
            }
            else
            {
                UnsavedChangesCheckingInEditors();
            }

            void UnsavedChangesCheckingInEditors()
            {
                List<@EditorModesShelf.Types.EditorModeIntermediator> EditorsWithOpenedFiles =
                    [.. @EditorModesShelf.ModesMapping.Select(x => x.AssociatedMode).Where(x => x.CurrentFile is not null)];


                if (EditorsWithOpenedFiles.Count > 0)
                {
                                                                        // Place current editor mode first in a sequence of checks
                    EditorsWithOpenedFiles = [.. EditorsWithOpenedFiles.OrderBy(x => x == @EditorModesShelf.CurrentEditorMode ? 0 : 1)];

                    bool FoundSomeUnsavedChanges = false;
                    foreach (@EditorModesShelf.Types.EditorModeIntermediator CheckingEditor in EditorsWithOpenedFiles)
                    {
                        (bool IsAnyUnsavedChanges, string UnsavedChangesText) = CheckingEditor.CollectUnsavedChanges();
                        if (IsAnyUnsavedChanges)
                        {
                            if (@EditorModesShelf.CurrentEditorMode != CheckingEditor)
                            {
                                ModeSwitchContextMenu_Click(
                                    Sender: MainWindowInstance.FindName($"ModeSwitchButton_{CheckingEditor.Identifier}"),
                                    Args: null!
                                );
                            }

                            // If switching still does not occur according to the above condition
                            if (@CurrentPreviewCreator.ActiveState == true)
                            {
                                PreviewCreatorPage.SwitchUI_Return();
                            }


                            UnsavedChangesDialog_ShowMenu(Text: UnsavedChangesText, ProceedAction: base.BeginFadeHiding);
                            FoundSomeUnsavedChanges = true;

                            break;
                        }
                    }


                    if (FoundSomeUnsavedChanges == false)
                    {
                        base.BeginFadeHiding();
                    }
                }
                else
                {
                    base.BeginFadeHiding();
                }
            }
        }
    }
}