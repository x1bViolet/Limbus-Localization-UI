using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.ConfigRegexSaver;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Windows.Visibility;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute
{
    public partial class SettingsWindow : Window
    {
        public static SettingsWindow SettingsControl;

        public SettingsWindow()
        {
            InitializeComponent();

            SettingsControl = this;
        }

        public static void UpdateSettingsMenu_Regular()
        {
            SettingsControl.UpdateSettingsMenu_Inner();
        }

        public static void UpdateSettingsMenu_CustomLang()
        {
            SettingsControl.UpdateSelectedCustomLanguageSettingsView();
        }

        private void UpdateSettingsMenu_Inner()
        {
            Configurazione.ConfigDelta Settings = Configurazione.DeltaConfig;

            ToggleStyleHighlightion_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle ? Visible : Collapsed;
            ToggleSkillDescHighlightion_OnClick_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnRightClick ? Visible : Collapsed;
            ToggleSkillDescHighlightion_OnSwitch_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch ? Visible : Collapsed;
            InputPreviewUpdateDelay.Text = Settings.PreviewSettings.PreviewSettingsBaseSettings.PreviewUpdateDelay.ToString();
            ToggleTopmostState_I.Visibility = Settings.Internal.IsAlwaysOnTop ? Visible : Collapsed;
            ToggleLoadWarnings_I.Visibility = Settings.Internal.ShowLoadWarnings ? Visible : Collapsed;
            ToggleEnableSkillNamesReplica_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.EnableSkillNamesReplication ? Visible : Collapsed;
            ToggleKeywordTooltips_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.EnableKeywordTooltips ? Visible : Collapsed;
            ToggleSyntaxHighlight_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.EnableSyntaxHighlight ? Visible : Collapsed;
            HideLimbusPreview_I.Visibility = Settings.PreviewSettings.PreviewSettingsBaseSettings.HidePreview ? Visible : Collapsed;

            rin("\n\n----------------------------------------------------");

            Dictionary<string, int> ThemeIndexes = new Dictionary<string, int>();
            int Index_Themes = 0;
            ThemeSelector.Items.Clear();
            foreach (var ThemeDir in new DirectoryInfo(@"[⇲] Assets Directory\@ Internal\Themes").GetDirectories())
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
            foreach (var LanguageDirectory in new DirectoryInfo(@"[⇲] Assets Directory\@ Internal\Translation").GetDirectories())
            {
                LanguageSelector.Items.Add(new TextBlock { Text = LanguageDirectory.Name });
                LanguageIndexes[LanguageDirectory.Name] = Index_Languages;
                Index_Languages++;
            }
            if (Directory.Exists(Settings.Internal.UILanguage))
            {
                string RelativeLangFileName = Settings.Internal.UILanguage.Split("\\")[^1].Split("/")[^1];
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

        private void Settings_Minimize(object RequestSender, MouseButtonEventArgs EventArgs) => WindowState = WindowState.Minimized;
        private void Settings_Close(object RequestSender, MouseButtonEventArgs EventArgs) => DoClose();
        private void DoClose()
        {
            if (ScansManager.IsSkillsAreaViewEnabled)
            {
                MainControl.ScanAreaView_Skills.BorderThickness = new Thickness(2);
                ScansManager.ToggleSkillScanAreaView();

            }
            if (!MainControl.SurfaceScrollPreview_Skills_Inner.Background.Equals(Brushes.Transparent))
            {
                //ToggleSkillsScanBackgroundColorView_I.Visibility = Collapsed;
                MainControl.SurfaceScrollPreview_Skills_Inner.Background = Brushes.Transparent;
            }
            this.Hide();
        }

        private void Settings_ReloadConfig(object RequestSender, MouseButtonEventArgs EventArgs) => MainWindow.ReloadConfig_Direct();
        private void Window_DragMove(object RequestSender, MouseButtonEventArgs EventArgs) => this.DragMove();
        private void AntiComboBoxScroll(object RequestSender, MouseWheelEventArgs EventArgs)
        {
            if (!(RequestSender as ComboBox).IsDropDownOpen)
            {
                EventArgs.Handled = true;
            }
        }

        private void OptionPress(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            switch ((RequestSender as FrameworkElement).Name)
            {
                case "ReloadCustomLanguageKeywords":
                    if (Directory.Exists(Configurazione.SelectedAssociativePropery_Shared.Properties.KeywordsDirectory))
                    {
                        KeywordsInterrogate.InitializeGlossaryFrom
                        (
                            KeywordsDirectory: Configurazione.SelectedAssociativePropery_Shared.Properties.KeywordsDirectory,
                            WriteOverFallback: true
                        );
                        if (Directory.Exists(Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AdditionalKeywordsDirectory))
                        {
                            KeywordsInterrogate.InitializeGlossaryFrom
                            (
                                KeywordsDirectory: Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AdditionalKeywordsDirectory,
                                WriteOverFallback: true
                            );
                        }
                        LimbusPreviewFormatter.UpdateLast();
                    }
                    break;

                case "ReloadKeywordImages":
                    KeywordsInterrogate.KeywordImages.Clear();
                    KeywordsInterrogate.LoadInlineImages();
                    LimbusPreviewFormatter.UpdateLast();
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
        public void OptionToggle(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            if (!Configurazione.SettingsLoadingEvent)
            {
                string TempConfigFile = File.ReadAllText(@"[⇲] Assets Directory\Configurazione^.json");
                string SelectedPropertiesName = Configurazione.DeltaConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected;


                string Input_Simplified = "";
                string Current_Simplified = "";

                switch ((RequestSender as FrameworkElement).Name)
                {
                    case "ToggleScansPreview":
                        ScansManager.ToggleSkillScanAreaView();
                        break;



                    case "Recheck_SkillsBackgroundColor":
                        Configurazione.DeltaConfig.ScanParameters.BackgroundColor = InputSkillsScanBackgroundColor.Text;

                        ChangeJsonConfigViaRegex("Background Color", Configurazione.DeltaConfig.ScanParameters.BackgroundColor);

                        break;



                    case "ToggleSyntaxHighlight":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSyntaxHighlight = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSyntaxHighlight;

                        ChangeJsonConfigViaRegex("Enable Syntax Highlight", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSyntaxHighlight);
                        
                        ToggleSyntaxHighlight_I.Visibility = Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSyntaxHighlight ? Visibility.Visible : Visibility.Collapsed;

                        SyntaxedTextEditor.RecompileEditorSyntax();

                        break;



                    case "HideLimbusPreview":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HidePreview = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HidePreview;

                        ChangeJsonConfigViaRegex("Hide Limbus Text Preview", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HidePreview);

                        HideLimbusPreview_I.Visibility = Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HidePreview ? Visibility.Visible : Visibility.Collapsed;

                        Configurazione.ToggleLimbusPreviewVisibility();

                        break;


                    //case "ToggleSkillsScanBackgroundColorView":

                    //    switch (ToggleSkillsScanBackgroundColorView_I.Visibility)
                    //    {
                    //        case Collapsed:
                    //            ToggleSkillsScanBackgroundColorView_I.Visibility = Visible;
                    //            MainControl.SurfaceScrollPreview_Skills_Inner.Background = SettingsControl.InputSkillsBackgroundColor_Display.Background;
                    //            break;

                    //        case Visible:
                    //            ToggleSkillsScanBackgroundColorView_I.Visibility = Collapsed;
                    //            MainControl.SurfaceScrollPreview_Skills_Inner.Background = Brushes.Transparent;
                    //            break;
                    //    }

                    //    break;



                    case "ToggleStyleHighlightion":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle;
                        ToggleStyleHighlightion_I.Visibility = ToggleStyleHighlightion_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        LimbusPreviewFormatter.UpdateLast();

                        ChangeJsonConfigViaRegex("Highlight <style>", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle);

                        break;



                    case "ToggleSkillDescHighlightion_OnClick":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnRightClick = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnRightClick;
                        ToggleSkillDescHighlightion_OnClick_I.Visibility = ToggleSkillDescHighlightion_OnClick_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        ChangeJsonConfigViaRegex("Highlight Skill Descs on right click", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnRightClick);

                        break;



                    case "ToggleSkillDescHighlightion_OnSwitch":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch;
                        ToggleSkillDescHighlightion_OnSwitch_I.Visibility = ToggleSkillDescHighlightion_OnSwitch_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        ChangeJsonConfigViaRegex("Highlight Skill Descs on manual switch", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch);

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



                    case "ToggleKeywordTooltips":
                        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableKeywordTooltips = !Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableKeywordTooltips;
                        ToggleKeywordTooltips_I.Visibility = ToggleKeywordTooltips_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        LimbusPreviewFormatter.UpdateLast();

                        ChangeJsonConfigViaRegex("Enable Keyword Tooltips", Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableKeywordTooltips);

                        break;



                    case "ToggleTopmostState":
                        Configurazione.DeltaConfig.Internal.IsAlwaysOnTop = !Configurazione.DeltaConfig.Internal.IsAlwaysOnTop;
                        MainControl.Topmost = Configurazione.DeltaConfig.Internal.IsAlwaysOnTop;
                        ToggleTopmostState_I.Visibility = ToggleTopmostState_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        ChangeJsonConfigViaRegex("Topmost Window", Configurazione.DeltaConfig.Internal.IsAlwaysOnTop);

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

                        LimbusPreviewFormatter.UpdateLast();

                        break;



                    case "ToggleKeywordUnderline":
                        Configurazione.Spec_EnableKeywordIDUnderline = !Configurazione.Spec_EnableKeywordIDUnderline;
                        ToggleKeywordUnderline_I.Visibility = ToggleKeywordUnderline_I.Visibility switch
                        {
                            Visible => Collapsed,
                            _/*Collapsed*/ => Visible
                        };

                        LimbusPreviewFormatter.UpdateLast();

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
                                
                                LimbusPreviewFormatter.UpdateLast();

                                ChangeJsonConfigViaRegex("Keywords Directory", FormattedPath, IsInsideCurrentCustomLangProperties: true);
                            }
                        }
                        else
                        {
                            MessageBox.Show(ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.KeywordsDirNotFound.Extern(CustomLang_KeywordsDir.Text.Trim()));
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
                            MessageBox.Show(ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.TitleFontMissing.Extern(CustomLang_TitleFont.Text.Trim()));
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
                            MessageBox.Show(ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.ContextFontMissing.Extern(CustomLang_ContextFont.Text.Trim()));
                        }
                        break;
                }
            }
        }

        //""Custom Language Associative Settings"": {(?<Between1>.*?)""Name"": ""Russian \(MTL\)"",(.*?)""Keywords Directory"": ""(?<Between2>.*?)"",    flag single line gms
        private void SelectionToggle(object RequestSender, SelectionChangedEventArgs EventArgs)
        {
            if (!Configurazione.SettingsLoadingEvent)
            {
                string NewSelectionName = "";
                string TempConfigFile = File.ReadAllText(@"[⇲] Assets Directory\Configurazione^.json");
                switch ((RequestSender as FrameworkElement).Name)
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

                            LimbusPreviewFormatter.UpdateLast();

                            ChangeJsonConfigViaRegex("Associative Properties Selected", NewSelection.PropertyName);
                        }


                        break;


                    case "LanguageSelector":
                        NewSelectionName = (LanguageSelector.SelectedItem as TextBlock).Text;
                       // UILanguageLoader.InitializeUILanguage(@$"[⇲] Assets Directory/[+] Languages/{NewSelectionName}.json");
                        ᐁ_Interface_Localization_Loader.ModifyUI(@$"[⇲] Assets Directory\@ Internal\Translation\{NewSelectionName}");

                        ChangeJsonConfigViaRegex("UI Language", $"[⇲] Assets Directory/@ Internal/Translation/{NewSelectionName}");

                        break;


                    case "ThemeSelector":
                        NewSelectionName = (ThemeSelector.SelectedItem as TextBlock).Text;
                        //UIThemesLoader.InitializeUITheme(@$"[⇲] Assets Directory\[+] Themes\{NewSelectionName}");
                        ᐁ_Interface_Themes_Loader.ModifyUI(@$"[⇲] Assets Directory\@ Internal\Themes\{NewSelectionName}");

                        ChangeJsonConfigViaRegex("UI Theme", $"[⇲] Assets Directory/@ Internal/Themes/{NewSelectionName}");

                        break;
                }
            }
        }

        private void Window_Closing(object RequestSender, System.ComponentModel.CancelEventArgs EventArgs)
        {
            EventArgs.Cancel = true;
            DoClose();
        }

        private void InputSkillsBackgroundColor_TextChanged(object RequestSender, TextChangedEventArgs EventArgs)
        {
            try
            {
                if (InputSkillsBackgroundColor_Display != null)
                {
                    string NewSkillsBackground = InputSkillsScanBackgroundColor.Text;

                    SolidColorBrush NewColor = ToSolidColorBrush(NewSkillsBackground);

                    InputSkillsBackgroundColor_Display.Background = NewColor;

                    //if (ToggleSkillsScanBackgroundColorView_I.Visibility.Equals(Visible))
                    //{
                    //    MainControl.SurfaceScrollPreview_Skills_Inner.Background = NewColor;
                    //}
                }
            }
            catch { }
        }
    }
}
