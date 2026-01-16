using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LC_Localization_Task_Absolute.PreviewCreator;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Json.JsonSerialization;
using static LC_Localization_Task_Absolute.MainWindow.ContextMenuHotkeys;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Themes_Loader;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute
{
    public partial class MainWindow
    {
        #region Titlebar
        private bool CanDragMove = true;
        private void Window_DragMove(object RequestSender, RoutedEventArgs EventArgs)
        {
            if (CanDragMove)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    this.Left = 0;
                    this.Top = 0;
                }

                this.DragMove();

                if (WindowState == WindowState.Maximized)
                {
                    if (@CurrentPreviewCreator.IsActive)
                    {
                        Rect WorkArea = SystemParameters.WorkArea;
                        this.Left = WorkArea.Left;
                        this.Top = WorkArea.Top;
                        this.Width = WorkArea.Width;
                        this.Height = WorkArea.Height;
                        this.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        MainWindowContentControl.Margin = new Thickness(6, 6, 0, 6);
                    }
                }
                else
                {
                    MainWindowContentControl.Margin = new Thickness(0);
                }
            }
        }
        private void Minimize(object RequestSender, RoutedEventArgs EventArgs) => WindowState = WindowState.Minimized;
        private void Shutdown(object RequestSender, RoutedEventArgs EventArgs) => CheckUnsavedChanges(ExitOnEnd: true);

        private void OpenSettings(object RequestSender, RoutedEventArgs EventArgs)
        {
            ConfigurationWindow.ConfigControl.Show();
            ConfigurationWindow.ConfigControl.WindowState = WindowState.Normal;
            ConfigurationWindow.ConfigControl.Focus();
        }

        private void OpenDisplayInfoManager(object RequestSender, RoutedEventArgs EventArgs)
        {
            SkillsDisplayInfoManagerWindow.SkillsDisplayInfoManagerControl.Show();
            SkillsDisplayInfoManagerWindow.SkillsDisplayInfoManagerControl.WindowState = WindowState.Normal;
            SkillsDisplayInfoManagerWindow.SkillsDisplayInfoManagerControl.Focus();
        }
        #endregion

        private void Window_SizeChanged(object RequestSender, SizeChangedEventArgs EventArgs)
        {
            if (IsLoaded)
            {
                double CapMinWidth = ActiveProperties.WindowSizesInfo.MinWidth + 2;
                double CapMinHeight = ActiveProperties.WindowSizesInfo.MinHeight + 2;

                if ((this.Width <= CapMinWidth & this.Height <= CapMinHeight) | (LoadedTheme.Common.HideBackgroundImageWithMinimumWindowWidth & this.Width <= CapMinWidth))
                {
                    BackgroundImage.Visibility = Collapsed;
                }
                else
                {
                    BackgroundImage.Visibility = Visible;
                }
            }
        }

        #region Surfacescroll
        private void InitSurfaceScroll(ScrollViewer ScrollViewerTarget)
        {
            (ScrollViewerTarget.Content as FrameworkElement).PreviewMouseLeftButtonDown += SurfaceScroll_MouseLeftButtonDown; // To not affect scrollbars
            ScrollViewerTarget.PreviewMouseMove += SurfaceScroll_MouseMove;
            ScrollViewerTarget.PreviewMouseLeftButtonUp += SurfaceScroll_MouseLeftButtonUp;
        }
        private bool SurfaceScroll_isDragging = false;
        private Point SurfaceScroll_lastMousePosition;
        private ScrollViewer LastCapturedScrollViewer = null;
        public void SurfaceScroll_MouseLeftButtonDown(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            LastCapturedScrollViewer = (RequestSender as FrameworkElement).Parent as ScrollViewer;
            SurfaceScroll_isDragging = true;
            SurfaceScroll_lastMousePosition = EventArgs.GetPosition(LastCapturedScrollViewer);
            LastCapturedScrollViewer.CaptureMouse();
        }
        public void SurfaceScroll_MouseMove(object RequestSender, MouseEventArgs EventArgs)
        {
            if (SurfaceScroll_isDragging)
            {
                ScrollViewer Target = RequestSender as ScrollViewer;

                Point currentPosition = EventArgs.GetPosition(Target);
                Vector diff = SurfaceScroll_lastMousePosition - currentPosition;
                Target.ScrollToVerticalOffset(Target.VerticalOffset + diff.Y);
                Target.ScrollToHorizontalOffset(Target.HorizontalOffset + diff.X);
                SurfaceScroll_lastMousePosition = currentPosition;
            }
        }
        public void SurfaceScroll_MouseLeftButtonUp(object RequestSender, RoutedEventArgs EventArgs)
        {
            SurfaceScroll_isDragging = false;
            (RequestSender as ScrollViewer).ReleaseMouseCapture();
        }
        #endregion








        #region Mouse and Keyboard shortcuts
        public static bool IsAnyTextBoxFocused => FocusableTextBoxes.Any(TextBox => TextBox.IsFocused);
        public static void UnfocusAllTextBoxes()
        {
            UnfocusElement(MainControl.TextEditor.TextArea);

            foreach (UIElement FocusedTextBox in FocusableTextBoxes.Where(textbox => textbox.IsFocused)) UnfocusElement(FocusedTextBox);
        }



        public void SaveCurrentDescription()
        {
            switch (ActiveProperties.Key)
            {
                case EditorMode.Skills:

                    if (TargetPreviewLayout == PreviewLayout_Skills_MainDesc)
                    {
                        Mode_Skills.@Current.Uptie.PresentMainDescription = Mode_Skills.@Current.Uptie.EditorMainDescription;

                        PresentedStaticTextEntries["[Skills / Right menu] * Skill main desc"].SetDefaultText();

                        Mode_Skills.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                    }
                    else
                    {
                        Mode_Skills.@Current.CoinDesc.PresentDescription = Mode_Skills.@Current.CoinDesc.EditorDescription;
                        if (!Mode_Skills.@Current.CoinDescs.Any(CoinDesc => CoinDesc.PresentDescription != CoinDesc.EditorDescription))
                        {
                            PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"].SetDefaultText();
                        }

                        PresentedStaticTextEntries["[Skills / Right menu] * Skill Coin desc number"].SetDefaultText(ExtraExtern: Mode_Skills.CurrentSkillCoinDescIndex + 1);

                        Mode_Skills.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                    }

                    break;



                case EditorMode.Passives:

                    switch (Mode_Passives.CurrentDescriptionType)
                    {
                        case TripleDescriptionType.Main:
                            Mode_Passives.@Current.Passive.PresentMainDescription = Mode_Passives.@Current.Passive.EditorMainDescription;

                            PresentedStaticTextEntries["[Passives / Right menu] * Passive desc"].SetDefaultText();

                            Mode_Passives.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                            break;


                        case TripleDescriptionType.Summary:
                            Mode_Passives.@Current.Passive.PresentSummaryDescription = Mode_Passives.@Current.Passive.EditorSummaryDescription;

                            PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"].SetDefaultText();

                            Mode_Passives.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                            break;


                        case TripleDescriptionType.Flavor:
                            Mode_Passives.@Current.Passive.PresentFlavorDescription = Mode_Passives.@Current.Passive.EditorFlavorDescription;

                            PresentedStaticTextEntries["[Passives / Right menu] * Passive flavor"].SetDefaultText();

                            Mode_Passives.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                            break;
                    }

                    break;



                case EditorMode.Keywords:

                    switch (Mode_Keywords.CurrentDescriptionType)
                    {
                        case TripleDescriptionType.Main:
                            Mode_Keywords.@Current.Keyword.PresentMainDescription = Mode_Keywords.@Current.Keyword.EditorMainDescription;

                            PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword desc"].SetDefaultText();

                            Mode_Keywords.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                            break;


                        case TripleDescriptionType.Summary:
                            Mode_Keywords.@Current.Keyword.PresentSummaryDescription = Mode_Keywords.@Current.Keyword.EditorSummaryDescription;

                            PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"].SetDefaultText();

                            Mode_Keywords.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                            break;


                        case TripleDescriptionType.Flavor:
                            Mode_Keywords.@Current.Keyword.PresentFlavorDescription = Mode_Keywords.@Current.Keyword.EditorFlavorDescription;

                            PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword flavor"].SetDefaultText();

                            Mode_Keywords.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                            break;
                    }

                    break;



                case EditorMode.EGOGifts:

                    if (Mode_EGOGifts.CurrentDescriptionType_String == "Main Description")
                    {
                        Mode_EGOGifts.@Current.EGOGift.PresentDescription = Mode_EGOGifts.@Current.EGOGift.EditorDescription;

                        PresentedStaticTextEntries["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"].SetDefaultText();

                        Mode_EGOGifts.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                    }
                    else
                    {
                        string SimpleDescNumber = Regex.Match(Mode_EGOGifts.CurrentDescriptionType_String, @"Simple Description №(\d+)").Groups[1].Value;

                        int TargetSimpleDescIndex = int.Parse(SimpleDescNumber) - 1;

                        Mode_EGOGifts.@Current.EGOGift.SimpleDescriptions[TargetSimpleDescIndex].PresentDescription = Mode_EGOGifts.@Current.EGOGift.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription;

                        PresentedStaticTextEntries[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"].SetDefaultText();


                        Mode_EGOGifts.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                    }

                    break;
            }

            if (NameInputs.Any(x => x.IsFocused))
            {
                ChangeObjectName(null, null);
            }
        }



        private static bool IsCtrlPressed = false;
        private void Window_PreviewKeyDown(object RequestSender, KeyEventArgs EventArgs)
        {
            #region Preview scan shortcut
            if (Keyboard.IsKeyDown(Key.LeftCtrl) & Keyboard.IsKeyDown(Key.P))
            {
                if (MakeLimbusPreviewScan.IsEnabled) SavePreviewlayoutScan_TitlebarButtonClick(null, null);
            }
            #endregion



            #region `[Column №N] Add item..` Buttons from Identity/E.G.O Preivew Creator hide/show
            if (Keyboard.IsKeyDown(Key.LeftCtrl) & Keyboard.IsKeyDown(Key.F) & @CurrentPreviewCreator.IsActive)
            {
                if (FirstColumnItemsSelector.Visibility == Visible)
                {
                    FirstColumnItemsSelector.Visibility = Collapsed;
                    SecondColumnItemsSelector.Visibility = Collapsed;
                }
                else
                {
                    FirstColumnItemsSelector.Visibility = Visible;
                    SecondColumnItemsSelector.Visibility = Visible;
                }
            }

            #region Other thing
            if (@CurrentPreviewCreator.IsActive && (Keyboard.IsKeyDown(Key.LeftShift) & Keyboard.IsKeyDown(Key.LeftCtrl) & Keyboard.IsKeyDown(Key.U)))
            {
                if (Marker.IsEnabled)
                {
                    Marker.Foreground = ToSolidColorBrush("#652a22");
                    Marker.IsEnabled = false;
                }
                else
                {
                    Marker.Foreground = ToSolidColorBrush("#654122");
                    Marker.IsEnabled = true;
                }
            }
            #endregion

            #endregion



            #region Ctrl + S save
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (EventArgs.Key == Key.S && !IsCtrlPressed)
                {
                    IsCtrlPressed = true;
                    if (!TargetPreviewLayout.Equals(PreviewLayoutDef_Default))
                    {
                        SaveCurrentDescription();
                    }
                }
            }
            #endregion



            #region ID switch by Left/Right keyboard buttons
            else if (EventArgs.Key == Key.Left | EventArgs.Key == Key.Right)
            {
                if (EventArgs.Key == Key.Right)
                {
                    if (!IsAnyTextBoxFocused & !TextEditor.TextArea.IsFocused) NavigationPanel_IDSwitch(NavigationPanel_IDSwitch_Next, null);
                }
                else if (EventArgs.Key == Key.Left)
                {
                    if (!IsAnyTextBoxFocused & !TextEditor.TextArea.IsFocused) NavigationPanel_IDSwitch(NavigationPanel_IDSwitch_Previous, null);
                }
            }
            #endregion



            // Stop manual ID input on Esc click
            else if (EventArgs.Key == Key.Escape)
            {
                if (NavigationPanel_IDSwitch_ManualInput_Textfield.IsFocused)
                {
                    NavigationPanel_IDSwitch_ManualInput_Stop();
                }

                UnfocusAllTextBoxes();
            }



            #region Manual ID switch confirm when MainWindow.NavigationPanel_IDSwitch_ManualInput_Start() is activated
            else if (EventArgs.Key == Key.Enter & NavigationPanel_IDSwitch_ManualInput_Textfield.IsFocused)
            {
                string IDInputString = NavigationPanel_IDSwitch_ManualInput_Textfield.Text.Trim();

                Dictionary<string, int> TargetSite_NameIDs = ActiveProperties.Key switch
                {
                    EditorMode.Skills => DelegateSkills_NameIDs,
                    EditorMode.Passives => DelegatePassives_NameIDs,
                    EditorMode.EGOGifts => DelegateEGOGifts_NameIDs,
                    _ => null // Keywords dictionary is <string, string>
                };

                int? RegularObjectTargetID = null;
                string? KeywordObjectTargetID = null;



                if (ActiveProperties.Key == EditorMode.Keywords)
                {
                    KeywordObjectTargetID = DelegateKeywords_NameIDs.ContainsKeyCaseInsensitive(IDInputString, out string FoundKey)
                        ? DelegateKeywords_NameIDs[FoundKey] // If input is keyword name, not id
                        : IDInputString; // Else keep input
                }
                else // Skills / Passives / E.G.O Gifts
                {
                    // If input is object name, not id
                    if (TargetSite_NameIDs.ContainsKeyCaseInsensitive(IDInputString, out string FoundKey))
                    {
                        RegularObjectTargetID = TargetSite_NameIDs[FoundKey];
                    }
                    // Else try get integer
                    else if (int.TryParse(IDInputString, out int IntID))
                    {
                        RegularObjectTargetID = IntID;
                    }
                }



                if (RegularObjectTargetID != null)
                {
                    // And then check if id list of current mode contains parsed integer id
                    switch (ActiveProperties.Key)
                    {
                        case EditorMode.Skills:
                            if (DelegateSkills_IDList.Contains((int)RegularObjectTargetID)) Mode_Skills.TransformToSkill((int)RegularObjectTargetID);
                            break;

                        case EditorMode.Passives:
                            if (DelegatePassives_IDList.Contains((int)RegularObjectTargetID)) Mode_Passives.TransformToPassive((int)RegularObjectTargetID);
                            break;

                        case EditorMode.EGOGifts:
                            if (DelegateEGOGifts_IDList.Contains((int)RegularObjectTargetID)) Mode_EGOGifts.TransformToEGOGift((int)RegularObjectTargetID);
                            break;
                    }
                }

                // Else if Keywords, same
                else if (KeywordObjectTargetID != null && DelegateKeywords_IDList.Contains(KeywordObjectTargetID))
                {
                    Mode_Keywords.TransformToKeyword(KeywordObjectTargetID);
                }

                NavigationPanel_IDSwitch_ManualInput_Stop();
            }
            #endregion



            #region Hotkeys
            if (TextEditor.TextArea.IsFocused & !HotkeyExecutedLock)
            {
                foreach (HotkeysConfig.Hotkey Shortcut in LoadedHotkeys.HotkeysList)
                {
                    if (Shortcut.Command != null && Shortcut.Keys.Count > 0 && Shortcut.Keys.All(Keyboard.IsKeyDown))
                    {
                        HotkeyExecutedLock = true; // Release on Window_PreviewKeyUp

                        LatestHotkeyCommandName = Shortcut.Command;

                        if (!Shortcut.Command.StartsWith("Extra ")) // Default context menu
                        {
                            TextEditor_SharedContextMenuClick(new MenuItem() { Name = Shortcut.Command.Del("<", ">", "[", "]", " ") }, null);
                        }
                        
                        else if (int.TryParse(Regex.Match(Shortcut.Command, @"Extra (\d+)").Groups[1].Value, out int ExtraReplacementsMenuItemNumber)) // Extra replacements
                        {
                            if (ExtraReplacementsMenuItemNumber <= @ExtraReplacements.ContextMenu.Items.Count)
                            {
                                TextEditor_SharedContextMenuClick(@ExtraReplacements.ContextMenu.Items[ExtraReplacementsMenuItemNumber - 1], null);
                            }
                        }
                    }
                }
            }
        }

        public static class ContextMenuHotkeys
        {
            public static bool HotkeyExecutedLock = false;

            public static string LatestHotkeyCommandName = "";

            public static HotkeysConfig LoadedHotkeys = new();

            public record HotkeysConfig
            {
                [JsonProperty("Hotkeys")]
                public List<Hotkey> HotkeysList { get; set; } = [];

                [JsonProperty("Keyboard keys")]
                public List<Key> KeybordKeysList => [.. Enum.GetValues(typeof(Key)).Cast<Key>()];

                public record Hotkey
                {
                    public string? Command { get; set; }
                    public List<Key> Keys { get; set; } = [];
                }
            }
            
            public static void ReadFile()
            {
                try { LoadedHotkeys = new FileInfo(@"[⇲] Assets Directory\Context Menu hotkeys.json").Deserealize<HotkeysConfig>(); }
                catch (Exception ex) { rin(FormattedStackTrace(ex, "Hotkeys file reading (Wrong key name?)")); }
            }
        }
        #endregion


        // Ctrl + S save ctrl unlock
        private void Window_PreviewKeyUp(object RequestSender, KeyEventArgs EventArgs)
        {
            HotkeyExecutedLock = false;
            if (EventArgs.Key == Key.S) IsCtrlPressed = false;
        }
        private void Window_PreviewMouseDown(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            if (EventArgs.ChangedButton == MouseButton.Left)
            {
                if (NavigationPanel_IDSwitch_ManualInput_Textfield.IsFocused & !IDButton_Container.IsMouseOver)
                {
                    NavigationPanel_IDSwitch_ManualInput_Stop();
                }
            }
            else
            {
                switch (EventArgs.ChangedButton)
                {
                    case MouseButton.XButton1: //Back (id/skill uptie switch)
                        if (ActiveProperties.Key == EditorMode.Skills && Keyboard.IsKeyDown(Key.LeftShift))
                        {
                            List<int> AvailableUpties = [.. Mode_Skills.@Current.Skill.Keys];
                            int PreviousIndex = AvailableUpties.IndexOf(Mode_Skills.CurrentSkillUptieLevel) - 1;
                            if (PreviousIndex != -1 && AvailableUpties[PreviousIndex] != Mode_Skills.CurrentSkillUptieLevel)
                            {
                                Mode_Skills.TransformToSkill(Mode_Skills.CurrentSkillID, AvailableUpties[PreviousIndex]);
                            }
                        }
                        else NavigationPanel_IDSwitch(NavigationPanel_IDSwitch_Previous, null);

                        break;


                    case MouseButton.XButton2: //Forward (id/skill uptie switch)
                        if (ActiveProperties.Key == EditorMode.Skills && Keyboard.IsKeyDown(Key.LeftShift))
                        {
                            List<int> AvailableUpties = [.. Mode_Skills.@Current.Skill.Keys];
                            int NextIndex = AvailableUpties.IndexOf(Mode_Skills.CurrentSkillUptieLevel) + 1;
                            if (AvailableUpties.Count >= NextIndex + 1 && AvailableUpties[NextIndex] != Mode_Skills.CurrentSkillUptieLevel)
                            {
                                Mode_Skills.TransformToSkill(Mode_Skills.CurrentSkillID, AvailableUpties[NextIndex]);
                            }
                        }
                        else NavigationPanel_IDSwitch(NavigationPanel_IDSwitch_Next, null);

                        break;


                    default: break;
                }
            }
        }
        #endregion
    }
}
