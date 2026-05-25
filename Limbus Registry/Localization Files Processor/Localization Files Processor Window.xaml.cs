using LCLocalizationInterface.Internal.Abstractions;
using System.Diagnostics;

namespace LCLocalizationInterface.LimbusRegistry
{
    namespace LocalizationFilesProcessing
    {
        public partial class LocalizationFilesProcessorWindow : FadeableWindow
        {
            #pragma warning disable CS8618
            public static LocalizationFilesProcessorWindow LocalizationFilesProcessorWindowInstance;
            #pragma warning restore CS8618

            public LocalizationFilesProcessorWindow()
            {
                InitializeComponent();
                foreach (FileInfo Profile in new DirectoryInfo(@"[⇲] Assets Directory\Localization Files Processor\Profiles").GetFiles("*.json"))
                {
                    DataContextDomain.LocalizationFilesProcessor.ProfilesList.Add(Profile.Name);
                }


                {
                    bool StyleAlreadyForced = false;
                    this.Loaded += (_, _) =>
                    {
                        if (!StyleAlreadyForced)
                        {
                            Internal.UIStyle.DefaultStyleDictionaryClass.OverrideDiffPlexScrollBarStyle(ProfileUnsavedChangesViewer);

                            StyleAlreadyForced = true; 
                        }
                    };
                }
            }

            #region FadeableWindow members
            protected override (double In, double Out) FadeDurations => AsPair(ThemeTimings.Duration.LocalizationFilesProcessorWindow);
            protected override (double In, double Out) FadeSpeedRatios => AsPair(ThemeTimings.SpeedRatio.LocalizationFilesProcessorWindow);
            protected override ((double Acceleration, double Deceleation) In, (double Acceleration, double Deceleation) Out) FadeKinematics
                => AsPairPair(ThemeTimings.AccelerationDecelerationRatios.LocalizationFilesProcessorWindow);
            #endregion

            public bool HasLoadedProfile => SelectedProfileLock != null;
            private static FileStream? SelectedProfileLock;
            private static FileEventsNotifier ProfilesWatcher = new(@"[⇲] Assets Directory\Localization Files Processor\Profiles", "*.json")
            {
                GeneralHandler = (_, FileSystemArgs, RenameArgs) =>
                {
                    ObservableCollection<string> ProfilesObservableList = DataContextDomain.LocalizationFilesProcessor.ProfilesList;
                    if (RenameArgs != null)
                    {
                        string OldName = Path.GetFileName(RenameArgs.OldFullPath);
                        string NewName = Path.GetFileName(RenameArgs.FullPath);

                        if (ProfilesObservableList.Contains(OldName))
                        {
                            ProfilesObservableList[ProfilesObservableList.IndexOf(OldName)] = NewName;
                        }
                    }
                    else
                    {
                        if (FileSystemArgs!.ChangeType == WatcherChangeTypes.Created)
                        {
                            ProfilesObservableList.Add(FileSystemArgs.Name!);
                        }
                        else if (FileSystemArgs.ChangeType == WatcherChangeTypes.Deleted)
                        {
                            ProfilesObservableList.Remove(FileSystemArgs.Name!);
                        }
                    }
                },
            };

            private void ToggleMaximizedStage(object Sender, RoutedEventArgs Args)
            {
                if (WindowState == WindowState.Maximized)
                {
                    ExitPseudoMaximizedState();
                    this.CenterOnScreen();
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                }
                CheckPseudoMaximizedStateParameters();
            }
            private void WindowOverrideDragMove(object Sender, MouseButtonEventArgs Args)
            {
                // WHEN WINDOW ALREADY WAS MAXIMIZED TO THE LEFT TOP CORNER and now dragging again -> exit maximized state
                if (WindowState == WindowState.Maximized)
                {
                    ExitPseudoMaximizedState();
                }

                try { this.DragMove(); }
                catch (InvalidOperationException) { /* Idk */ }

                // If window was maximized after dragging
                CheckPseudoMaximizedStateParameters();
            }
            public void ExitPseudoMaximizedState()
            {
                this.WindowState = WindowState.Normal; this.Top = 0;
            }
            public void CheckPseudoMaximizedStateParameters()
            {
                VisualBorder.BorderThickness = new Thickness(this.WindowState is WindowState.Maximized ? 0 : 1);
                CurrentWindowChrome.ResizeBorderThickness = this.WindowState is WindowState.Maximized ? new(0) : new(4, 10, 10, 10);
                VisualBorder.Padding = new Thickness(this.WindowState is WindowState.Maximized ? 4 : 0);
                VisualBorder.Margin = new Thickness(0, 0, 0, this.WindowState is WindowState.Maximized ? SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height : 0);
            }

            private void SwitchMenu(object Sender, MouseButtonEventArgs Args)
            {
                @Languages.PresentedTextElements["[Localization Processor] * Menu switch — Parameters"].Tag = "";
                @Languages.PresentedTextElements["[Localization Processor] * Menu switch — Files processing"].Tag = "";

                (Sender as IntenseStareType1)!.Tag = "This menu is selected";
            }


            public Action UnsavedChangesDialog_ProceedAction = null!;
            public Action? UnsavedChangesDialog_CancelAction = null;
            public void UnsavedChangesDialog_ShowMenu(Action ProceedAction, Action? CancelAction = null)
            {
                ProfileUnsavedChangesViewer.OldText = OriginallyLoadedProfile.Trim();
                ProfileUnsavedChangesViewer.NewText = CurrentProfile.Trim();

                UnsavedChangesDialog_ProceedAction = ProceedAction;
                UnsavedChangesDialog_CancelAction = CancelAction;
                Panel.SetZIndex(UnsavedChangesGrid, 1);
                UnsavedChangesGrid.Opacity = 1;
                MainGrid.Effect = new BlurEffect() { Radius = 10 };
            }
            private void UnsavedChangesDialog_HideMenu()
            {
                Panel.SetZIndex(UnsavedChangesGrid, -1);
                UnsavedChangesGrid.Opacity = 0;
                MainGrid.Effect = null;
            }

            /// <summary>Unsaved Changes Dialog - 'Confirm' click (Or click on blurred background)</summary>
            private void UnsavedChangesDialog_ButtonsClick_Proceed(object Sender, RoutedEventArgs Args)
            {
                UnsavedChangesDialog_HideMenu();
                UnsavedChangesDialog_ProceedAction.Invoke();
            }
            /// <summary>Unsaved Changes Dialog - 'Cancel' click (Or click on blurred background)</summary>
            private void UnsavedChangesDialog_ButtonsClick_Cancel(object Sender, RoutedEventArgs Args)
            {
                UnsavedChangesDialog_HideMenu();
                UnsavedChangesDialog_CancelAction?.Invoke();
            }



            private void TopmostStateCheckChanged(object Sender, RoutedEventArgs Args)
            {
                if (ProgramFullyLoaded) @Configurazione.Save();
            }

            public string OriginallyLoadedProfile = "";
            public string CurrentProfile => DataContextDomain.LocalizationFilesProcessor.Profile.SerializeToFormattedJsonText();
            public bool CurrentProfileHasUnsavedChanges => LocalizationFilesProcessorWindowInstance.CurrentProfile.ReplaceLineEndings("\n") != LocalizationFilesProcessorWindowInstance.OriginallyLoadedProfile.ReplaceLineEndings("\n");
            private bool ManuallyRolledBackProfileLatch = false;
            private void ProfileSelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
            {
                if (Args.AddedItems.Count > 0 && Args.AddedItems[0] is string SelectedProfile)
                {
                    if (ProgramFullyLoaded && ManuallyRolledBackProfileLatch == false && HasLoadedProfile && CurrentProfileHasUnsavedChanges)
                    {
                        UnsavedChangesDialog_ShowMenu(ProceedAction: LoadNewProfile, CancelAction: delegate ()
                        {
                            if (Args.RemovedItems.Count > 0 && Args.RemovedItems[0] is string OldProfile)
                            {
                                ManuallyRolledBackProfileLatch = true;
                                ProfileSelector.SelectedItem = OldProfile;
                            }
                        });
                    }
                    else
                    {
                        if (ManuallyRolledBackProfileLatch == true)
                        {
                            ManuallyRolledBackProfileLatch = false;
                        }
                        else
                        {
                            LoadNewProfile();
                        }
                    }

                    void LoadNewProfile()
                    {
                        SelectedProfileLock?.Dispose();

                        string ProfilePath = @$"[⇲] Assets Directory\Localization Files Processor\Profiles\{SelectedProfile}";
                        OriginallyLoadedProfile = StreamReadText(ProfilePath);
                        DataContextDomain.LocalizationFilesProcessor.Profile = OriginallyLoadedProfile.DeserealizeJsonAs<LocalizationFilesProcessingProfile>()!;

                        foreach (IntenseStareType3 TextInputParameter in VisualBorder.FindVisualChildren<IntenseStareType3>())
                        {
                            TextInputParameter.Document.UndoStack.ClearAll();

                            if (TextInputParameter.PathSelectionButtonType != IntenseStareType3.PathType.None)
                            {
                                TextInputParameter.ScrollToHorizontalOffset(double.PositiveInfinity);
                            }
                        }

                        ExportedFilesLog.Children.Clear();

                        SelectedProfileLock = new FileStream(ProfilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        if (ProgramFullyLoaded) @Configurazione.Save();
                    }
                }
            }

            private async void SaveEditedProfile(object Sender, RoutedEventArgs Args)
            {
                SaveFileDialog FileSaver = NewSaveFileDialog(".json files", ["json"], "Edited profile", Path.GetFullPath(@"[⇲] Assets Directory\Localization Files Processor\Profiles"));
                if (FileSaver.ShowDialog() == true)
                {
                    SelectedProfileLock?.Dispose();

                    string SerializedProfile = DataContextDomain.LocalizationFilesProcessor.Profile.SerializeToFormattedJsonText();
                    OriginallyLoadedProfile = SerializedProfile; // To be sure if saved profile has same name and SelectionChanges wasn't triggered
                    File.WriteAllText(FileSaver.FileName, SerializedProfile, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

                    await Task.Delay(100);
                    ProfileSelector.SelectedItem = FileSaver.SafeFileName;
                }
            }


            public void UpdateSyntaxHighlightColors()
            {
                var SyntaxColors = @Themes.CurrentTheme.UITextfields.Syntax;
                static void NewSyntax(List<string> IDs)
                {
                    IDs.ForEach(ID => @Languages.PresentedTextFields[ID].ResetHighlightDefinition());
                }
                void WildcardPattern(List<string> IDs)
                {
                    IDs.ForEach(ID =>
                    {
                        @Languages.PresentedTextFields[ID].AddHighlight([
                            new(@"\*", SyntaxColors.Highlight1, false),
                            new(@"\,", SyntaxColors.Highlight4, false),
                            new(@"\?", SyntaxColors.Highlight3, false)
                        ]);
                    });
                }

                NewSyntax([
                    "[Localization Processor / Profile] * Shorthands pattern",
                    "[Localization Processor / Profile] * General localization files whitelist",
                    "[Localization Processor / Profile] * Whitelist of missing files to append",
                    "[Localization Processor / Profile] * Whitelist of existing files to add IDs to",
                    "[Localization Processor / Profile] * Keyword Shorthands files whitelist"
                ]);

                WildcardPattern([
                    "[Localization Processor / Profile] * General localization files whitelist",
                    "[Localization Processor / Profile] * Whitelist of missing files to append",
                    "[Localization Processor / Profile] * Whitelist of existing files to add IDs to",
                    "[Localization Processor / Profile] * Keyword Shorthands files whitelist"
                ]);

                @Languages.PresentedTextFields["[Localization Processor / Profile] * Shorthands pattern"].AddHighlight([
                    new(@"\(\?\<ID\>\\w\+\)", SyntaxColors.Highlight1),
                    new(@"\?<Name>",          SyntaxColors.Highlight2),
                    new(@"\?<Color>",         SyntaxColors.Highlight3),
                    new(@"\?<SpriteID>",      SyntaxColors.Highlight4)
                ]);
            }

            private async void StartButton_Click(object Sender, RoutedEventArgs Args)
            {
                await Modules.Main.DoDirectExport();
            }
            private void CancelButton_Click(object Sender, RoutedEventArgs Args)
            {
                LocalizationFilesProcessing.Modules.Main.CurrentLocalizationFilesProcessingCancelTokenSource.Cancel();
            }
            private void ViewReportButton_Click(object Sender, RoutedEventArgs Args)
            {
                using (Process FileOpener = new() { StartInfo = new() { FileName = "explorer" } })
                {
                    FileOpener.StartInfo.Arguments = "\"[⇲] Assets Directory\\Localization Files Processor\\Recent log.json\"";
                    FileOpener.Start();
                }
            }

            private void LogFileName_OpenWithExternalEditor(object Sender, RoutedEventArgs Args)
            {
                using (Process FileOpener = new() { StartInfo = new() { FileName = "explorer" } })
                {
                    FileOpener.StartInfo.Arguments = $"\"{((Sender as MenuItem_T1)!.Tag as dynamic).FullPath}\"";
                    FileOpener.Start();
                }
            }
            private void LogFileName_CopyFileText(object Sender, RoutedEventArgs Args)
            {
                Clipboard.SetDataObject(StreamReadText(((Sender as MenuItem_T1)!.Tag as dynamic).FullPath));
            }
            private void LogFileName_CopyFileItself(object Sender, RoutedEventArgs Args)
            {
                Clipboard.SetFileDropList([ ((Sender as MenuItem_T1)!.Tag as dynamic).FullPath ]);
            }
            private void LogFileName_CopyName(object Sender, RoutedEventArgs Args)
            {
                Clipboard.SetDataObject(((Sender as MenuItem_T1)!.Tag as dynamic).FileName);
            }
            private void LogFileName_CopyFullPath(object Sender, RoutedEventArgs Args)
            {
                Clipboard.SetDataObject(((Sender as MenuItem_T1)!.Tag as dynamic).FullPath);
            }
        }
    }
}