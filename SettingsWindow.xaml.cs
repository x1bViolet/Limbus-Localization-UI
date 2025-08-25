using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using RichText;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.ConfigRegexSaver;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute
{
    public partial class SettingsWindow : Window
    {
        internal protected static SettingsWindow SettingsControl;


        private static Dictionary<string, int> FontWeights = new Dictionary<string, int>();
        public SettingsWindow()
        {
            InitializeComponent();

            SettingsControl = this;
        }

        internal protected static void UpdateSettingsMenu_Regular()
        {
            SettingsControl.UpdateSettingsMenu_Inner();
        }

        internal protected static void UpdateSettingsMenu_CustomLang()
        {
            SettingsControl.UpdateSelectedCustomLanguageSettingsView();
        }

        private void UpdateSettingsMenu_Inner()
        {
            Configurazione.ConfigDelta Settings = Configurazione.DeltaConfig;

            ToggleStyleHighlightion_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle ? Visible : Collapsed;
            ToggleCoinDescHighlightion_OnClick_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnRightClick ? Visible : Collapsed;
            ToggleCoinDescHighlightion_OnSwitch_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnManualSwitch ? Visible : Collapsed;
            InputPreviewUpdateDelay.Text = Settings.PreviewSettings.PreviewSettingsBaseSettings.PreviewUpdateDelay.ToString();
            ToggleTopmostState_I.Visibility = Settings.Internal.AlwaysOnTop ? Visible : Collapsed;
            ToggleLoadWarnings_I.Visibility = Settings.Internal.ShowLoadWarnings ? Visible : Collapsed;
            ToggleEnableSkillNamesReplica_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.EnableSkillNamesReplication ? Visible : Collapsed;

            rin("\n\n----------------------------------------------------");

            Dictionary<string, int> ThemeIndexes = new Dictionary<string, int>();
            int Index_Themes = 0;
            ThemeSelector.Items.Clear();
            foreach (var ThemeDir in new DirectoryInfo(@"⇲ Assets Directory\[+] Themes").GetDirectories())
            {
                ThemeSelector.Items.Add(new TextBlock { Text = ThemeDir.Name });
                ThemeIndexes[ThemeDir.Name] = Index_Themes;
                Index_Themes++;
            }
            if (Directory.Exists(Settings.Internal.UITheme))
            {
                string RelativeThemePathName = Settings.Internal.UITheme.Split("\\")[^1].Split("/")[^1];
                if (ThemeIndexes.ContainsKey(RelativeThemePathName))
                {
                    ThemeSelector.SelectedIndex = ThemeIndexes[RelativeThemePathName];
                }
            }




            int Index_Languages = 0;
            Dictionary<string, int> LanguageIndexes = new Dictionary<string, int>();
            LanguageSelector.Items.Clear();
            foreach (var LanguageFile in new DirectoryInfo(@"⇲ Assets Directory\[+] Languages").GetFiles("*.json"))
            {
                LanguageSelector.Items.Add(new TextBlock { Text = LanguageFile.Name.Replace(".json", "") });
                LanguageIndexes[LanguageFile.Name.Replace(".json", "")] = Index_Languages;
                Index_Languages++;
            }
            if (File.Exists(Settings.Internal.UILanguage))
            {
                string RelativeLangFileName = Settings.Internal.UILanguage.Split("\\")[^1].Split("/")[^1].Replace(".json", "");
                if (LanguageIndexes.ContainsKey(RelativeLangFileName))
                {
                    LanguageSelector.SelectedIndex = LanguageIndexes[RelativeLangFileName];
                }
            }

            int Index_CustomLanguageProperties = 0;
            Dictionary<string, int> CustomLanguagePropIndexes = new Dictionary<string, int>();
            CustomLanguagePropertiesSelector.Items.Clear();
            foreach (var CustomLanguageProperty in Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.List)
            {
                if (!CustomLanguageProperty.HideInList)
                {
                    CustomLanguagePropertiesSelector.Items.Add(new TextBlock { Text = CustomLanguageProperty.PropertyName });
                    CustomLanguagePropIndexes[CustomLanguageProperty.PropertyName] = Index_CustomLanguageProperties;
                    Index_CustomLanguageProperties++;

                    if (Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected.Equals(CustomLanguageProperty.PropertyName))
                    {
                        CustomLanguagePropertiesSelector.SelectedIndex = CustomLanguagePropIndexes[CustomLanguageProperty.PropertyName];
                    }
                }
            }

        }

        private void UpdateSelectedCustomLanguageSettingsView()
        {
            Configurazione.CustomLanguageAssociativePropertyValues Settings = Configurazione.SelectedAssociativePropery_Shared.Properties;
            CustomLang_KeywordsDir.Text = Settings.KeywordsDirectory;
            CustomLang_TitleFont.Text = Settings.TitleFont;
            CustomLang_ContextFont.Text = Settings.ContextFont;
        }

        private void Settings_Minimize(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
        private void Settings_Close(object sender, MouseButtonEventArgs e) => DoClose();
        private void DoClose()
        {
            if (ScansManager.IsAreaViewEnabled)
            {
                MainControl.ScanAreaView_Skills.BorderThickness = new Thickness(2);
                ScansManager.ToggleScanAreaView();

            }
            if (!MainControl.SurfaceScrollPreview_Skills_Inner.Background.Equals(Brushes.Transparent))
            {
                ToggleSkillsScanBackgroundColorView_I.Visibility = Collapsed;
                MainControl.SurfaceScrollPreview_Skills_Inner.Background = Brushes.Transparent;
            }
            this.Hide();
        }

        private void Settings_ReloadConfig(object sender, MouseButtonEventArgs e) => MainWindow.ReloadConfig_Direct();
        private void Window_DragMove(object sender, MouseButtonEventArgs e) => this.DragMove();
        private void AntiComboBoxScroll(object sender, MouseWheelEventArgs e)
        {
            if (!(sender as ComboBox).IsDropDownOpen)
            {
                e.Handled = true;
            }
        }

        private void OptionPress(object sender, MouseButtonEventArgs e)
        {
            string Sender = (sender as FrameworkElement).Name;
            switch (Sender)
            {
                case "ReloadCustomLanguageKeywords":
                    if (Directory.Exists(Configurazione.SelectedAssociativePropery_Shared.Properties.KeywordsDirectory))
                    {
                        KeywordsInterrogate.InitializeGlossaryFrom
                        (
                            KeywordsDirectory: Configurazione.SelectedAssociativePropery_Shared.Properties.KeywordsDirectory,
                            WriteOverFallback: true
                        );
                        RichTextBoxApplicator.UpdateLast();
                    }
                    break;

                case "ReloadKeywordImages":
                    KeywordsInterrogate.KeywordImages.Clear();
                    KeywordsInterrogate.LoadInlineImages();
                    RichTextBoxApplicator.UpdateLast();
                    break;

                case "ResetEGOGiftsDisplayInfo":
                    Mode_EGOGifts.OrganizedData.DisplayInfo_Icons.Clear();
                    Mode_EGOGifts.OrganizedData.UpdateDisplayInfo();
                    break;

                case "ReloadSkillsDisplayInfo":

                    Mode_Skills.OrganizedDisplayInfo.Clear();
                    Mode_Skills.LoadDisplayInfo();

                    Custom_Skills_Constructor.LoadedSkillConstructors.Clear();
                    Custom_Skills_Constructor.ReadSkillConstructors();

                    Mode_Skills.ChangeSkillHeaderReplicaAppearance();

                    break;
            }
        }

        public bool ChangeConfigOnOptionToggle = true;
        public void OptionToggle(object sender, MouseButtonEventArgs e)
        {
            if (!Configurazione.SettingsLoadingEvent)
            {
                string Sender = (sender as FrameworkElement).Name;
                string TempConfigFile = File.ReadAllText(@"⇲ Assets Directory\Configurazione^.json");
                string SelectedPropertiesName = Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected;


                string Input_Simplified = "";
                string Current_Simplified = "";

                switch (Sender)
                {
                    case "ToggleScansPreview":
                        ScansManager.ToggleScanAreaView();
                        break;



                    case "Recheck_SkillsBackgroundColor":
                        Configurazione.DeltaConfig.ScanParameters.BackgroundColor = InputSkillsScanBackgroundColor.Text;

                        ChangeJsonConfigViaRegex("Background Color", Configurazione.DeltaConfig.ScanParameters.BackgroundColor);

                        break;


                    case "ToggleSkillsScanBackgroundColorView":

                        switch (ToggleSkillsScanBackgroundColorView_I.Visibility)
                        {
                            case Collapsed:
                                ToggleSkillsScanBackgroundColorView_I.Visibility = Visible;
                                MainControl.SurfaceScrollPreview_Skills_Inner.Background = SettingsControl.InputSkillsBackgroundColor_Display.Background;
                                break;

                            case Visible:
                                ToggleSkillsScanBackgroundColorView_I.Visibility = Collapsed;
                                MainControl.SurfaceScrollPreview_Skills_Inner.Background = Brushes.Transparent;
                                break;
                        }

                        break;



                    case "ToggleStyleHighlightion":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle;
                        ToggleStyleHighlightion_I.Visibility = ToggleStyleHighlightion_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        RichTextBoxApplicator.UpdateLast();

                        ChangeJsonConfigViaRegex("Highlight <style>", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle);

                        break;



                    case "ToggleCoinDescHighlightion_OnClick":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnRightClick = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnRightClick;
                        ToggleCoinDescHighlightion_OnClick_I.Visibility = ToggleCoinDescHighlightion_OnClick_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        ChangeJsonConfigViaRegex("Highlight Coin Descs on right click", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnRightClick);

                        break;



                    case "ToggleCoinDescHighlightion_OnSwitch":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnManualSwitch = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnManualSwitch;
                        ToggleCoinDescHighlightion_OnSwitch_I.Visibility = ToggleCoinDescHighlightion_OnSwitch_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        ChangeJsonConfigViaRegex("Highlight Coin Descs on manual switch", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnManualSwitch);

                        break;



                    case "ToggleEnableSkillNamesReplica":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSkillNamesReplication = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSkillNamesReplication;
                        ToggleEnableSkillNamesReplica_I.Visibility = ToggleEnableSkillNamesReplica_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        MainControl.SkillReplica.Visibility = Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSkillNamesReplication switch
                        {
                            true  => Visible,
                            false => Collapsed
                        };

                        ChangeJsonConfigViaRegex("Enable Skill Names Replication", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSkillNamesReplication);
                        Mode_Skills.ChangeSkillHeaderReplicaAppearance();
                        break;



                    case "ToggleTopmostState":
                        Configurazione.DeltaConfig.Internal.AlwaysOnTop = !Configurazione.DeltaConfig.Internal.AlwaysOnTop;
                        MainControl.Topmost = Configurazione.DeltaConfig.Internal.AlwaysOnTop;
                        ToggleTopmostState_I.Visibility = ToggleTopmostState_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        ChangeJsonConfigViaRegex("Topmost Window", Configurazione.DeltaConfig.Internal.AlwaysOnTop);

                        break;



                    case "ToggleLoadWarnings":
                        Configurazione.DeltaConfig.Internal.ShowLoadWarnings = !Configurazione.DeltaConfig.Internal.ShowLoadWarnings;
                        ToggleLoadWarnings_I.Visibility = ToggleLoadWarnings_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        ChangeJsonConfigViaRegex("Show Load Warnings", Configurazione.DeltaConfig.Internal.ShowLoadWarnings);

                        break;



                    case "ToggleKeywordSprites":
                        Configurazione.Spec_EnableKeywordIDSprite = !Configurazione.Spec_EnableKeywordIDSprite;
                        ToggleKeywordSprites_I.Visibility = ToggleKeywordSprites_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        RichTextBoxApplicator.UpdateLast();

                        break;



                    case "ToggleKeywordUnderline":
                        Configurazione.Spec_EnableKeywordIDUnderline = !Configurazione.Spec_EnableKeywordIDUnderline;
                        ToggleKeywordUnderline_I.Visibility = ToggleKeywordUnderline_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        RichTextBoxApplicator.UpdateLast();

                        break;



                    case "Recheck_PreviewUpdateDelay":
                        try
                        {
                            double InputDelay = double.Parse(InputPreviewUpdateDelay.Text.Replace(".", ","));

                            Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.PreviewUpdateDelay = InputDelay;

                            string StringedDelay = InputDelay.ToString();
                            InputPreviewUpdateDelay.Text = StringedDelay;

                            ChangeJsonConfigViaRegex("Preview Update Delay (Seconds)", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.PreviewUpdateDelay);
                        }
                        catch
                        {
                            InputPreviewUpdateDelay.Text = $"{Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.PreviewUpdateDelay}";
                        }
                        break;

                        

                    case "Recheck_ScansScaleFactor":
                        try
                        {
                            double NewScansScaleFactor = double.Parse(InputScansScaleFactor.Text);
                            if (NewScansScaleFactor > 20)
                            {
                                NewScansScaleFactor = 20;
                            }
                            if (NewScansScaleFactor < 1)
                            {
                                NewScansScaleFactor = 1;
                            }

                            Configurazione.DeltaConfig.ScanParameters.ScaleFactor = NewScansScaleFactor;
                            ChangeJsonConfigViaRegex("Scale Factor", Configurazione.DeltaConfig.ScanParameters.ScaleFactor = NewScansScaleFactor);

                            InputScansScaleFactor.Text = $"{NewScansScaleFactor}";
                        }
                        catch
                        {

                        }
                        break;



                    case "Recheck_SkillsPanelWidth":
                        try
                        {
                            double NewSkillsWidth = double.Parse(SettingsControl.InputSkillsPanelWidth.Text.Replace(".", ","));

                            SettingsControl.InputSkillsPanelWidth.Text = NewSkillsWidth.ToString();

                            Configurazione.DeltaConfig.ScanParameters.AreaWidth = NewSkillsWidth;
                            if (MainControl.ScanAreaView_Skills.BorderThickness.Top != 0)
                            {
                                if (NewSkillsWidth != 0)
                                {
                                    MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = NewSkillsWidth;
                                }
                                else
                                {
                                    MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;
                                }
                            }

                            string StringWidthSkills = NewSkillsWidth.ToString();
                            InputSkillsPanelWidth.Text = StringWidthSkills;

                            ChangeJsonConfigViaRegex("Skills Area Width", Configurazione.DeltaConfig.ScanParameters.AreaWidth);
                        }
                        catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                        break;





                    case "Recheck_KeywordsDir":
                        if (Directory.Exists(CustomLang_KeywordsDir.Text))
                        {
                            Input_Simplified = CustomLang_KeywordsDir.Text.Replace("\\", "/").ToLower();
                            Current_Simplified = Configurazione.SelectedAssociativePropery_Shared.Properties.KeywordsDirectory.Replace("\\", "/").ToLower();

                            if (!Input_Simplified.Equals(Current_Simplified))
                            {
                                string FormattedPath = CustomLang_KeywordsDir.Text.Replace("\\", "/");

                                CustomLang_KeywordsDir.Text = FormattedPath;

                                Configurazione.SelectedAssociativePropery_Shared.Properties.KeywordsDirectory = FormattedPath;
                                Configurazione.LoadErrors = "";
                                Configurazione.UpdateCustomLanguagePart(Configurazione.SelectedAssociativePropery_Shared);
                                
                                RichTextBoxApplicator.UpdateLast();

                                ChangeJsonConfigViaRegex("Keywords Directory", FormattedPath, IsInsideCurrentCustomLangProperties: true);
                            }
                        }
                        else
                        {
                            MessageBox.Show(UILanguageLoader.LoadedLanguage.CustomLangLoadingWarnings.KeywordsDirNotFound.Extern(CustomLang_KeywordsDir.Text.Trim()));
                        }

                        break;


                    case "Recheck_TitleFont":
                        if (File.Exists(CustomLang_TitleFont.Text))
                        {
                            Input_Simplified = CustomLang_TitleFont.Text.Replace("\\", "/").ToLower();
                            Current_Simplified = Configurazione.SelectedAssociativePropery_Shared.Properties.TitleFont.Replace("\\", "/").ToLower();

                            if (!Input_Simplified.Equals(Current_Simplified))
                            {
                                string FormattedPath = CustomLang_TitleFont.Text.Replace("\\", "/");

                                CustomLang_TitleFont.Text = FormattedPath;

                                Configurazione.SelectedAssociativePropery_Shared.Properties.TitleFont = FormattedPath;
                                Configurazione.LoadErrors = "";
                                Configurazione.UpdatePreviewLayoutsFont(Configurazione.SelectedAssociativePropery_Shared.Properties);

                                ChangeJsonConfigViaRegex("Title Font", FormattedPath, IsInsideCurrentCustomLangProperties: true);
                            }
                        }
                        else
                        {
                            MessageBox.Show(UILanguageLoader.LoadedLanguage.CustomLangLoadingWarnings.TitleFontMissing.Extern(CustomLang_TitleFont.Text.Trim()));
                        }
                        break;


                    case "Recheck_ContextFont":
                        if (File.Exists(CustomLang_ContextFont.Text))
                        {
                            Input_Simplified = CustomLang_ContextFont.Text.Replace("\\", "/").ToLower();
                            Current_Simplified = Configurazione.SelectedAssociativePropery_Shared.Properties.ContextFont.Replace("\\", "/").ToLower();

                            if (!Input_Simplified.Equals(Current_Simplified))
                            {
                                string FormattedPath = CustomLang_ContextFont.Text.Replace("\\", "/");

                                CustomLang_ContextFont.Text = FormattedPath;

                                Configurazione.SelectedAssociativePropery_Shared.Properties.ContextFont = FormattedPath;
                                Configurazione.LoadErrors = "";
                                Configurazione.UpdatePreviewLayoutsFont(Configurazione.SelectedAssociativePropery_Shared.Properties);

                                ChangeJsonConfigViaRegex("Context Font", FormattedPath, IsInsideCurrentCustomLangProperties: true);
                            }
                        }
                        else
                        {
                            MessageBox.Show(UILanguageLoader.LoadedLanguage.CustomLangLoadingWarnings.ContextFontMissing.Extern(CustomLang_ContextFont.Text.Trim()));
                        }
                        break;
                }
            }
        }

        //""Custom Language Associative Settings"": {(?<Between1>.*?)""Name"": ""Russian \(MTL\)"",(.*?)""Keywords Directory"": ""(?<Between2>.*?)"",    flag single line gms
        private void SelectionToggle(object sender, SelectionChangedEventArgs e)
        {
            if (!Configurazione.SettingsLoadingEvent)
            {
                string Sender = (sender as FrameworkElement).Name;
                string NewSelectionName = "";
                string TempConfigFile = File.ReadAllText(@"⇲ Assets Directory\Configurazione^.json");
                switch (Sender)
                {
                    case "CustomLanguagePropertiesSelector":

                        NewSelectionName = (CustomLanguagePropertiesSelector.SelectedItem as TextBlock).Text;
                        var NewSelectionFound = Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.List.Where(x => x.PropertyName.Equals(NewSelectionName)).ToList();
                        if (NewSelectionFound.Count() > 0)
                        {
                            var NewSelection = NewSelectionFound[0];

                            Configurazione.SelectedAssociativePropery_Shared = NewSelection;
                            Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected = NewSelection.PropertyName;

                            Configurazione.LoadErrors = "";
                            Configurazione.UpdateCustomLanguagePart(NewSelection);
                            UpdateSelectedCustomLanguageSettingsView();

                            RichTextBoxApplicator.UpdateLast();

                            ChangeJsonConfigViaRegex("Associative Properties Selected", NewSelection.PropertyName);
                        }


                        break;


                    case "LanguageSelector":
                        NewSelectionName = (LanguageSelector.SelectedItem as TextBlock).Text;
                        UILanguageLoader.InitializeUILanguage(@$"⇲ Assets Directory/[+] Languages/{NewSelectionName}.json");

                        ChangeJsonConfigViaRegex("UI Language", $"⇲ Assets Directory/[+] Languages/{NewSelectionName}.json");

                        break;


                    case "ThemeSelector":
                        NewSelectionName = (ThemeSelector.SelectedItem as TextBlock).Text;
                        UIThemesLoader.InitializeUITheme(@$"⇲ Assets Directory\[+] Themes\{NewSelectionName}");

                        ChangeJsonConfigViaRegex("UI Theme", $"⇲ Assets Directory/[+] Themes/{NewSelectionName}");

                        break;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            DoClose();
        }

        private void InputSkillsBackgroundColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (InputSkillsBackgroundColor_Display != null)
                {
                    string NewSkillsBackground = InputSkillsScanBackgroundColor.Text;

                    SolidColorBrush NewColor = ToSolidColorBrush(NewSkillsBackground);

                    InputSkillsBackgroundColor_Display.Background = NewColor;

                    if (ToggleSkillsScanBackgroundColorView_I.Visibility.Equals(Visible))
                    {
                        MainControl.SurfaceScrollPreview_Skills_Inner.Background = NewColor;
                    }
                }
            }
            catch { }
        }
    }
}
