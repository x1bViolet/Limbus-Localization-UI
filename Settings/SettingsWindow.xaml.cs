using ICSharpCode.AvalonEdit.Highlighting;
using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using LC_Localization_Task_Absolute.UITranslationHandlers;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Configurazione.ConfigDelta.PreviewSettings_PROP.CustomLanguageProperties_PROP.CustomLanguageAssociativeSettings_PROP;
using static LC_Localization_Task_Absolute.Configurazione.ConfigDelta.PreviewSettings_PROP.CustomLanguageProperties_PROP.CustomLanguageAssociativeSettings_PROP.CustomLanguageAssociativePropertyMain_PROP;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.@SyntaxedTextEditor;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Themes_Loader;


namespace LC_Localization_Task_Absolute
{
    public partial class ConfigurationWindow : Window
    {
        public static ConfigurationWindow ConfigControl;

        private static CustomLanguageAssociativePropertyValues_PROP CurrentCustomLang => @CurrentConfess.SelectedCustomLang.Properties;

        public ConfigurationWindow()
        {
            InitializeComponent();
            ConfigControl = this;


            this.Loaded += (Sender, Args) =>
            {
                ScrollTextBoxesToLeftest();

                if (TryParseColor(SV_ScansBackgroundColor.Text, out Color Color))
                {
                    ScansBackgroundColor_Display.Background = new SolidColorBrush(Color);
                }

                UpdateSpecializedMintSyntaxes();
            };
        }


        public void UpdateSpecializedMintSyntaxes()
        {
            var Theme = LoadedTheme.UITextfields.SpecialTextfields;
            static void SetHighlight(UITranslation_Hyacinth Target, string Pattern, string Foreground)
            {
                Target.SyntaxHighlighting.MainRuleSet.Rules.Add(new HighlightingRule()
                {
                    Regex = new Regex(Pattern),
                    Color = new HighlightingColor() { Foreground = new HighlightionBrush(Foreground), Underline = true }
                });
            }


            SetHighlight(SV_KeywordsAutoDetectionRegex, @"KeywordNameWillBeHere", Theme.SyntaxHighlight1);

            SetHighlight(SV_ShorthandsRegex, @"\(\?\<ID\>\\w\+\)", Theme.SyntaxHighlight2);
            SetHighlight(SV_ShorthandsRegex, @"\?<Name>", Theme.SyntaxHighlight3);
            SetHighlight(SV_ShorthandsRegex, @"\?<Color>", Theme.SyntaxHighlight4);

            SetHighlight(SV_ShorthandsCMInsertionShape, @"<KeywordID>", Theme.SyntaxHighlight2);
            SetHighlight(SV_ShorthandsCMInsertionShape, @"<KeywordName>", Theme.SyntaxHighlight3);
            SetHighlight(SV_ShorthandsCMInsertionShape, @"<KeywordColor>", Theme.SyntaxHighlight4);

            SetHighlight(SV_ShorthandsCMInsertionShape_KeywordColor, new(@"<HexColor>"), Theme.SyntaxHighlight4);
        }

        private void ScrollTextBoxesToLeftest()
        {
            ConfigControl.SV_TitleFont.ScrollToHorizontalOffset(double.PositiveInfinity);
            ConfigControl.SV_ContextFont.ScrollToHorizontalOffset(double.PositiveInfinity);
            ConfigControl.SV_KeywordsDirectory.ScrollToHorizontalOffset(double.PositiveInfinity);
            ConfigControl.SV_FallbackKeywordDirectory.ScrollToHorizontalOffset(double.PositiveInfinity);
            ConfigControl.SV_AdditionalKeywordsDirectory.ScrollToHorizontalOffset(double.PositiveInfinity);
            ConfigControl.SV_ContextMenuExtraReplacements.ScrollToHorizontalOffset(double.PositiveInfinity);
            ConfigControl.SV_KeywordsMultipleMeaningsDictionary.ScrollToHorizontalOffset(double.PositiveInfinity);
        }

        #region Base window logic
        private void Window_Closing(object RequestSender, CancelEventArgs EventArgs)
        {
            EventArgs.Cancel = true; this.Hide();
        }
        private void Window_DragMove(object RequestSender, RoutedEventArgs EventArgs) => this.DragMove();
        private void Settings_Minimize(object RequestSender, RoutedEventArgs EventArgs) => this.WindowState = WindowState.Minimized;
        private void Settings_ReloadConfig(object RequestSender, RoutedEventArgs EventArgs) => Configurazione.ReadConfigurazioneFile();
        private void Settings_Close(object RequestSender, RoutedEventArgs EventArgs) => this.Hide();
        #endregion


        public static void StartupSet()
        {
            ConfigControl.SV_SelectingCustomLanguage.Items.Clear();
            foreach (var CustomLanguageProperty in @CurrentConfess.AssociativePropertiesList)
            {
                if (CustomLanguageProperty.Value.HideInList) continue;

                ConfigControl.SV_SelectingCustomLanguage.Items.Add(
                    new UITranslation_Rose()
                    {
                        FontSize = 13,
                        Text = CustomLanguageProperty.Key,
                        TextDecorations = CustomLanguageProperty.Key.EqualsOneOf("English (Original)", "Korean (Original)", "Japanese (Original)")
                            ? TextDecorations.Underline
                            : null,
                        DataContext = CustomLanguageProperty.Value,
                    }
                );
            }

            ConfigControl.SV_SelectingInterfaceTheme.Items.Clear();
            foreach (DirectoryInfo ThemeDirectory in new DirectoryInfo(@"[⇲] Assets Directory\※ Internal\Themes").GetDirectories())
            {
                ConfigControl.SV_SelectingInterfaceTheme.Items.Add(new UITranslation_Rose() { FontSize = 13, Text = ThemeDirectory.Name });
            }

            ConfigControl.SV_SelectingInterfaceLanguage.Items.Clear();
            foreach (DirectoryInfo TranslationDirectory in new DirectoryInfo(@"[⇲] Assets Directory\※ Internal\Translation").GetDirectories())
            {
                ConfigControl.SV_SelectingInterfaceLanguage.Items.Add(new UITranslation_Rose() { FontSize = 13, Text = TranslationDirectory.Name });
            }

            StartupSetJsonValues(LoadedProgramConfig);

            var Info = @CurrentConfess.SelectedCustomLang.Properties;
            ConfigControl.SV_TitleFont.Text = Info.TitleFont;
            ConfigControl.SV_ContextFont.Text = Info.ContextFont;
            ConfigControl.SV_KeywordsDirectory.Text = Info.KeywordsDirectory;
        }
        public static void StartupSetJsonValues(object ConfigurazioneSection)
        {
            foreach (PropertyInfo ConfigProperty in ConfigurazioneSection.GetType().GetProperties())
            {
                try
                {
                    object PropertyValue = ConfigProperty.GetValue(ConfigurazioneSection);
                    if (ConfigProperty.HasAttribute(out AssignedCheckBoxAttribute AssignedCheckBox))
                    {
                        ConfigControl.InterfaceObject<CheckBox>(AssignedCheckBox.CheckBoxName).IsChecked = (bool)PropertyValue!;
                    }
                    else if (ConfigProperty.HasAttribute(out AssignedTextBoxAttribute AssignedTextBox))
                    {
                        string StringToSet = PropertyValue.ToString();
                        if (AssignedTextBox.RemoveHashAsColor) StringToSet = StringToSet.Del("#");

                        ConfigControl.InterfaceObject<UITranslation_Mint>(AssignedTextBox.TextBoxName).Text = StringToSet;
                    }
                    else if (ConfigProperty.HasAttribute(out AssignedComboBoxAttribute AssignedComboBox))
                    {
                        ComboBox Target = ConfigControl.InterfaceObject<ComboBox>(AssignedComboBox.ComboBoxName);

                        string CompareWith = (string)PropertyValue;
                        if (AssignedComboBox.ConsiderAsDirectory) CompareWith = CompareWith.Split("\\")[^1].Split("/")[^1];

                        foreach (UITranslation_Rose CurrentSelectableItem in Target.Items)
                        {
                            if (CurrentSelectableItem.Text == CompareWith)
                            {
                                Target.SelectedItem = CurrentSelectableItem;
                                var NewCustomLanguageSelection = CurrentSelectableItem.DataContext as CustomLanguageAssociativePropertyMain_PROP;
                                break;
                            }
                        }
                        
                        if (Target.SelectedIndex == -1) Target.SelectedIndex = 0; // If invalid name or path..
                    }
                    else if (ConfigProperty.HasAttribute<ConfigSectionAttribute>()) // Go to subsections
                    {
                        StartupSetJsonValues(ConfigProperty.GetValue(ConfigurazioneSection));
                    }
                }
                catch (Exception ex)
                {
                    rin(FormattedStackTrace(ex, $"(Settings window parameter loading error {{{ConfigProperty.Name} at the {ConfigurazioneSection.GetType().Name}}})"));
                }
            }
        }




        private void CheckBox_SharedCheckedUnchecked(object RequestSender, RoutedEventArgs EventArgs)
        {
            CheckBox Sender = RequestSender as CheckBox;

            bool IsChecked = (bool)Sender.IsChecked;
            switch (Sender.Name)
            {
                #region ⦁ Limbus Preview
                case nameof(SV_EnableSyntaxHighlight):
                    LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSyntaxHighlight = IsChecked;
                    JsonTextEditor.RecompileEditorSyntax();
                    break;


                case nameof(SV_HideLimbusTextPreview):
                    LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HidePreview = IsChecked;
                    
                    MainControl.LimbusPreviewRow.MaxHeight = IsChecked
                        ? 0
                        : double.PositiveInfinity;

                    if (!IsChecked) LimbusPreviewFormatter.UpdateLast();
                    break;


                case nameof(SV_HighlightStyle):
                    LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle = IsChecked;
                    LimbusPreviewFormatter.UpdateLast();
                    break;


                case nameof(SV_HighlightSkillDescsOnRightClick):
                    LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnRightClick = IsChecked;
                    break;


                case nameof(SV_HighlightSkillDescsOnManualSwitch):
                    LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch = IsChecked;
                    break;


                case nameof(SV_EnableSkillNamesRepliaction):
                    LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSkillNamesReplication = IsChecked;
                    MainControl.SkillNameReplicaParentGrid.Visibility = IsChecked ? Visibility.Visible : Visibility.Collapsed;
                    if (IsChecked) Mode_Skills.ChangeSkillNameReplicaAppearance();
                    break;


                case nameof(SV_Enable7to10coins):
                    LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.Enable7to10CoinButtons = IsChecked;

                    Mode_Skills.Toggle7to10CoinsVisibility(IsChecked);

                    break;


                case nameof(SV_EnableKeywordTooltips):
                    LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableKeywordTooltips = IsChecked;
                    LimbusPreviewFormatter.UpdateLast();
                    break;
                #endregion



                #region ⦁ Custom Language
                case nameof(ShowAllCustomLanguageOptions):
                    _ = CustomLangExtra_TitleFontOptions.Visibility
                      = CustomLangExtra_ContextFontOptions.Visibility
                      = CustomLangExtra_SpritesOffset.Visibility
                      = CustomLangExtra_KeywordsOther.Visibility
                      = CustomLangExtra_AdditionalKeywordsDirectory.Visibility
                      = IsChecked ? Visibility.Visible : Visibility.Collapsed;

                    _ = CustomLangMain_ContextFont.Margin
                      = CustomLangMain_TitleFont.Margin
                      = new Thickness(0, IsChecked ? 18 : 5, 0, 0);
                    break;
                #endregion



                #region ⦁ Internal
                case nameof(SV_TopmostWindow):
                    _ = LoadedProgramConfig.Internal.IsAlwaysOnTop
                      = MainControl.Topmost
                      = IsChecked;
                    break;


                case nameof(SV_EnableLoadWarnings):
                    LoadedProgramConfig.Internal.ShowLoadWarnings = IsChecked;
                    break;


                case nameof(SV_EnableManualJsonFilesManaging):
                    LoadedProgramConfig.Internal.EnableManualJsonFilesManaging = IsChecked;

                    _ = MainControl.AppendAdditionalObjectContextMenu.IsOpen
                      = MainControl.FilesCreationContextMenu.IsOpen
                      = MainControl.AddUptieContextMenu.IsOpen = false;

                    _ = MainControl.FilesCreationContextMenu.Visibility
                      = MainControl.AppendAdditionalObjectContextMenu.Visibility
                      = MainControl.AddUptieContextMenu.Visibility
                      = MainControl.KeywordColorInput.Visibility
                      = MainControl.NavigationPanel_Skills_AffinitySelection.Visibility
                      = IsChecked ? Visibility.Visible : Visibility.Collapsed;

                    break;
                #endregion



                #region ⦁ Preview Scans
                case nameof(SV_EnableSkillsScanAreaView):
                    if (ActiveProperties.Key != EditorMode.Skills)
                    {
                        SV_EnableSkillsScanAreaView.IsChecked = false;
                        return;
                    }
                    else
                    {
                        ScansManager.ToggleSkillScanAreaView();
                    }

                    break;



                case nameof(SV_DisplayKeywordIDSprites):
                    Configurazione.Spec_EnableKeywordIDSprite = IsChecked;
                    LimbusPreviewFormatter.UpdateLast();
                    break;



                case nameof(SV_DisplayKeywordIDUnderline):
                    Configurazione.Spec_EnableKeywordIDUnderline = IsChecked;
                    LimbusPreviewFormatter.UpdateLast();
                    break;
                #endregion



                default: break;
            }

            if (!Configurazione.ConfigLoadingEvent)
            {
                Configurazione.SaveConfig();
            }
        }

        readonly Regex HexColorCharPattern = new(@"[a-fA-F0-9]", RegexOptions.Compiled);
        readonly Regex FloatNumberCharPattern = new(@"[\d,.]",  RegexOptions.Compiled);
        readonly Regex FloatAlsoNegativeNumberCharPattern = new(@"[\d\-,.]", RegexOptions.Compiled);
        private void Mint_SharedPreviewTextInput(object RequestSender, TextCompositionEventArgs EventArgs)
        {
            switch ((RequestSender as UITranslation_Mint).Name)
            {
                case "HandleIfNotNumber":
                    EventArgs.HandleIfNotMatches(FloatNumberCharPattern);
                    break;


                #region ⦁ Limbus Preview
                case nameof(SV_PreviewUpdateDelay): goto case "HandleIfNotNumber";
                #endregion



                #region ⦁ Custom Language
                case nameof(SV_TitleFont_SizeMultiplier): goto case "HandleIfNotNumber";
                case nameof(SV_ContextFont_SizeMultiplier): goto case "HandleIfNotNumber";

                case nameof(SV_SpritesHorizontalOffset): EventArgs.HandleIfNotMatches(FloatAlsoNegativeNumberCharPattern); break;
                case nameof(SV_SpritesVerticalOffset): EventArgs.HandleIfNotMatches(FloatAlsoNegativeNumberCharPattern); break;
                #endregion



                #region ⦁ Preview Scans
                case nameof(SV_SkillsPanelWidth): goto case "HandleIfNotNumber";
                case nameof(SV_ScansScaleFactor): goto case "HandleIfNotNumber";
                case nameof(SV_ScansBackgroundColor): EventArgs.HandleIfNotMatches(HexColorCharPattern); break;
                #endregion

                default: break;
            }
        }

        private void SV_ScansBackgroundColor_TextChanged(object RequestSender, TextChangedEventArgs EventArgs)
        {
            if (IsLoaded)
            {
                if (TryParseColor(SV_ScansBackgroundColor.Text, out Color Color))
                {
                    ScansBackgroundColor_Display.Background = new SolidColorBrush(Color);
                }
            }
        }

        private void Button_SharedPressed(object RequestSender, RoutedEventArgs EventArgs)
        {
            Button ActualSender = RequestSender as Button;

            bool CancelConfigSave = false;

            string InputText = "";

            InputText = ActualSender.Name.StartsWith("SV_Reload")
                ? "" // Reload button does not have assigned textbox
                : ConfigControl.InterfaceObject<UITranslationTextfield>(ActualSender.Name.Del("_ConfirmButton")).Text.Replace("\\", "/");

            switch (ActualSender.Name)
            {
                #region ⦁ Limbus Preview
                case nameof(SV_PreviewUpdateDelay_ConfirmButton):
                    double InputDelay = double.Parse(SV_PreviewUpdateDelay.Text.Replace(".", ","));

                    LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.PreviewUpdateDelay = InputDelay;
                    SV_PreviewUpdateDelay.Text = $"{InputDelay}";

                    break;
                #endregion



                #region ⦁ Custom Language
                case nameof(SV_KeywordsDirectory_ConfirmButton):

                    if (Directory.Exists(InputText))
                    {
                        _ = SV_KeywordsDirectory.Text
                          = CurrentCustomLang.KeywordsDirectory
                          = InputText;

                        Configurazione.@PartialStateUpdater.CustomLanguage.UpdateLimbusKeywords(InputText);
                    }
                    else
                    {
                        MessageBox.Show(ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.KeywordsDirNotFound.Extern(InputText.Trim()));
                        SV_KeywordsDirectory.Text = CurrentCustomLang.KeywordsDirectory;
                        CancelConfigSave = true;
                    }

                    break;


                case nameof(SV_TitleFont_ConfirmButton):

                    if (File.Exists(InputText))
                    {
                        _ = SV_TitleFont.Text
                          = CurrentCustomLang.TitleFont
                          = InputText;

                        Configurazione.@PartialStateUpdater.Fonts.UpdateTitleFont();
                        LimbusPreviewFormatter.UpdateLast();
                    }
                    else
                    {
                        MessageBox.Show(ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.TitleFontMissing.Extern(InputText.Trim()));
                        SV_TitleFont.Text = CurrentCustomLang.TitleFont;
                        CancelConfigSave = true;
                    }

                    break;


                case nameof(SV_ContextFont_ConfirmButton):

                    if (File.Exists(InputText))
                    {
                        _ = SV_ContextFont.Text
                          = CurrentCustomLang.ContextFont
                          = InputText;

                        Configurazione.@PartialStateUpdater.Fonts.UpdateContextFont();
                        LimbusPreviewFormatter.UpdateLast();
                    }
                    else
                    {
                        MessageBox.Show(ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.TitleFontMissing.Extern(InputText.Trim()));
                        SV_ContextFont.Text = CurrentCustomLang.ContextFont;
                        CancelConfigSave = true;
                    }
                    break;



                case nameof(SV_TitleFont_SizeMultiplier_ConfirmButton):
                    double InputTitleFontSizeMultiplier = double.Parse(InputText.Replace(".", ","));

                    CurrentCustomLang.TitleFont_FontSizeMultipler = InputTitleFontSizeMultiplier;
                    SV_TitleFont_SizeMultiplier.Text = $"{InputTitleFontSizeMultiplier}";

                    Configurazione.@PartialStateUpdater.Fonts.UpdateFontSizes();
                    LimbusPreviewFormatter.UpdateLast();

                    break;

                case nameof(SV_ContextFont_SizeMultiplier_ConfirmButton):
                    double InputContextFontSizeMultiplier = double.Parse(InputText.Replace(".", ","));

                    CurrentCustomLang.ContextFont_FontSizeMultipler = InputContextFontSizeMultiplier;
                    SV_ContextFont_SizeMultiplier.Text = $"{InputContextFontSizeMultiplier}";

                    Configurazione.@PartialStateUpdater.Fonts.UpdateFontSizes();
                    LimbusPreviewFormatter.UpdateLast();

                    break;



                case nameof(SV_TitleFont_OverrideName_ConfirmButton):
                    CurrentCustomLang.TitleFont_OverrideReadName = InputText;
                    Configurazione.@PartialStateUpdater.Fonts.UpdateTitleFont();
                    break;

                case nameof(SV_ContextFont_OverrideName_ConfirmButton):
                    CurrentCustomLang.ContextFont_OverrideReadName = InputText;
                    Configurazione.@PartialStateUpdater.Fonts.UpdateContextFont();
                    break;



                case nameof(SV_SpritesVerticalOffset_ConfirmButton):
                    double InputSpritesVerticalOffset = double.Parse(InputText.Replace(".", ","));

                    CurrentCustomLang.KeywordsSpriteVerticalOffset = InputSpritesVerticalOffset;
                    SV_SpritesVerticalOffset.Text = $"{InputSpritesVerticalOffset}";

                    LimbusPreviewFormatter.UpdateLast();

                    break;

                case nameof(SV_SpritesHorizontalOffset_ConfirmButton):
                    double InputSpritesHorizontalOffset = double.Parse(InputText.Replace(".", ","));

                    CurrentCustomLang.KeywordsSpriteHorizontalOffset = InputSpritesHorizontalOffset;
                    SV_SpritesHorizontalOffset.Text = $"{InputSpritesHorizontalOffset}";
                    LimbusPreviewFormatter.UpdateLast();

                    break;



                case nameof(SV_KeywordsAutoDetectionRegex_ConfirmButton):
                    try
                    {
                        _ = new Regex(SV_KeywordsAutoDetectionRegex.Text);

                        CurrentCustomLang.Keywords_AutodetectionRegex = SV_KeywordsAutoDetectionRegex.Text.Trim();

                        JsonTextEditor.RecompileEditorSyntax();
                        LimbusPreviewFormatter.UpdateLast();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); CancelConfigSave = true; }

                    break;

                case nameof(SV_KeywordsMultipleMeaningsDictionary_ConfirmButton):

                    if (File.Exists(InputText) | InputText.Trim() == "")
                    {
                        _ = SV_KeywordsMultipleMeaningsDictionary.Text
                          = CurrentCustomLang.KeywordsMultipleMeaningsDictionary
                          = InputText;

                        Configurazione.@PartialStateUpdater.CustomLanguage.UpdateKeywordsMultipleMeaningsDictionary(InputText);
                    }
                    else if (InputText.Trim() != "")
                    {
                        MessageBox.Show(ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.MultipleKeywordsDictionaryMissing.Extern(InputText.Trim()));
                        SV_KeywordsMultipleMeaningsDictionary.Text = CurrentCustomLang.KeywordsMultipleMeaningsDictionary;
                        CancelConfigSave = true;
                    }

                    break;

                case nameof(SV_ContextMenuExtraReplacements_ConfirmButton):
                    
                    if (File.Exists(InputText) | InputText.Trim() == "")
                    {
                        _ = SV_ContextMenuExtraReplacements.Text
                          = CurrentCustomLang.ContextMenuExtraReplacements
                          = InputText;

                        Configurazione.@PartialStateUpdater.CustomLanguage.UpdateExtraReplacementsDictionary(CurrentCustomLang.ContextMenuExtraReplacements);
                    }
                    else if (InputText.Trim() != "")
                    {
                        MessageBox.Show(ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.ContextMenuExtraReplacementsMissing.Extern(InputText.Trim()));
                        SV_ContextMenuExtraReplacements.Text = CurrentCustomLang.ContextMenuExtraReplacements;
                        CancelConfigSave = true;
                    }

                    break;



                case nameof(SV_ShorthandsRegex_ConfirmButton):
                    try
                    {
                        _ = new Regex(SV_KeywordsAutoDetectionRegex.Text);

                        CurrentCustomLang.Keywords_ShorthandsRegex = SV_ShorthandsRegex.Text.Trim();

                        @CurrentConfess.ShorthandsPattern = new Regex(CurrentCustomLang.Keywords_ShorthandsRegex, RegexOptions.Singleline | RegexOptions.Compiled);
                        
                        JsonTextEditor.RecompileEditorSyntax();
                        LimbusPreviewFormatter.UpdateLast();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    break;

                case nameof(SV_ShorthandsCMInsertionShape_ConfirmButton):
                    _ = CurrentCustomLang.Keywords_ShorthandsContextMenuInsertionShape
                      = @CurrentConfess.ShorthandsInsertionParams.InsertionShape
                      = InputText;
                    break;

                case nameof(SV_ShorthandsCMInsertionShape_KeywordColor_ConfirmButton):
                    _ = CurrentCustomLang.Keywords_ShorthandsContextMenuInsertionShape_HexColor
                      = @CurrentConfess.ShorthandsInsertionParams.InsertionShape_Color
                      = InputText;
                    break;



                case nameof(SV_AdditionalKeywordsDirectory_ConfirmButton):

                    if (Directory.Exists(InputText) | InputText.Trim() == "")
                    {
                        _ = SV_AdditionalKeywordsDirectory.Text
                          = LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.AdditionalKeywordsDirectory
                          = InputText;

                        Configurazione.@PartialStateUpdater.CustomLanguage.UpdateLimbusKeywords();
                    }
                    else if (InputText.Trim() != "")
                    {
                        MessageBox.Show(ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.KeywordsDirNotFound.Extern(InputText.Trim()));
                        SV_AdditionalKeywordsDirectory.Text = LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.AdditionalKeywordsDirectory;
                        CancelConfigSave = true;
                    }
                    break;


                case nameof(SV_FallbackKeywordDirectory_ConfirmButton):
                    
                    if (Directory.Exists(InputText))
                    {
                        _ = SV_FallbackKeywordDirectory.Text
                          = LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.KeywordsFallbackDirectory
                          = InputText;

                        Configurazione.@PartialStateUpdater.CustomLanguage.UpdateLimbusKeywords();
                    }
                    else
                    {
                        MessageBox.Show(ᐁ_Interface_Localization_Loader.SpecializedDefs.CustomLangLoadingWarnings.FallbackKeywordsNotFound.Extern(InputText.Trim()));
                        SV_FallbackKeywordDirectory.Text = LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.KeywordsFallbackDirectory;
                        CancelConfigSave = true;
                    }
                    break;
                #endregion



                #region ⦁ Resources Reload
                case nameof(SV_ReloadCustomLanguage):
                    Configurazione.@PartialStateUpdater.CustomLanguage.UpdateLimbusKeywords();
                    break;


                case nameof(SV_ReloadKeywordIcons):
                    KeywordsInterrogation.KeywordImages.Clear();
                    KeywordsInterrogation.LoadInlineImages();

                    LimbusPreviewFormatter.UpdateLast();
                    break;


                case nameof(SV_ReloadSkillsDisplayInfo):
                    Mode_Skills.OrganizedDisplayInfo.Clear();
                    Mode_Skills.LoadDisplayInfo();

                    SkillsDisplayInfo.LoadedSkillConstructors.Clear();

                    if ((bool)SV_EnableSkillNamesRepliaction.IsChecked) Mode_Skills.ChangeSkillNameReplicaAppearance();
                    break;


                case nameof(SV_ReloadEGOGiftsDisplayInfo):
                    Mode_EGOGifts.OrganizedData.DisplayInfo_Icons.Clear();
                    Mode_EGOGifts.OrganizedData.UpdateDisplayInfo();

                    Mode_EGOGifts.ReCheckEGOGiftDisplayInfo();
                    break;


                case nameof(SV_ReloadHotkeys):
                    ContextMenuHotkeys.ReadFile();
                    break;
                #endregion



                #region ⦁ Preview Scans
                case nameof(SV_SkillsPanelWidth_ConfirmButton):
                    double InputSkillsPanelWidth = double.Parse(SV_SkillsPanelWidth.Text.Replace(".", ","));

                    if (MainControl.ScanAreaView_Skills.BorderThickness.Top != 0)
                    {
                        MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = InputSkillsPanelWidth != 0
                            ? InputSkillsPanelWidth
                            : Mode_Skills.LastRegisteredWidth;
                    }

                    LoadedProgramConfig.ScanParameters.AreaWidth = InputSkillsPanelWidth;
                    SV_SkillsPanelWidth.Text = $"{InputSkillsPanelWidth}";
                    break;


                case nameof(SV_ScansScaleFactor_ConfirmButton):
                    double InputScansScaleFactor = double.Parse(SV_ScansScaleFactor.Text.Replace(".", ","));
                    if (InputScansScaleFactor > 20) InputScansScaleFactor = 20;
                    if (InputScansScaleFactor < 1) InputScansScaleFactor = 1;

                    LoadedProgramConfig.ScanParameters.ScaleFactor = InputScansScaleFactor;
                    SV_ScansScaleFactor.Text = $"{InputScansScaleFactor}";
                    break;


                case nameof(SV_ScansBackgroundColor_ConfirmButton):
                    LoadedProgramConfig.ScanParameters.BackgroundColor = $"#{SV_ScansBackgroundColor.Text}";
                    break;
                #endregion

                default: break;
            }

            if (!CancelConfigSave & !Configurazione.ConfigLoadingEvent)
            {
                Configurazione.SaveConfig();
            }
        }
        private void ComboBox_SharedSelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
        {
            ComboBox ActualSender = RequestSender as ComboBox;

            UITranslation_Rose SelectedItem = ActualSender.SelectedItem as UITranslation_Rose;
            if (SelectedItem != null)
            {
                switch (ActualSender.Name)
                {
                    #region ⦁ Custom Language
                    case nameof(SV_SelectingCustomLanguage):
                        var NewCustomLanguageSelection = SelectedItem.DataContext as CustomLanguageAssociativePropertyMain_PROP;


                        Configurazione.LoadErrors = "";
                        Configurazione.UpdateCustomLanguagePart(NewCustomLanguageSelection);
                        LoadedProgramConfig.PreviewSettings.CustomLanguageProperties.AssociativeSettings.Selected = NewCustomLanguageSelection.PropertyName;

                        SV_KeywordsDirectory.Text = NewCustomLanguageSelection.Properties.KeywordsDirectory;
                        SV_TitleFont.Text = NewCustomLanguageSelection.Properties.TitleFont;
                        SV_ContextFont.Text = NewCustomLanguageSelection.Properties.ContextFont;


                        foreach (UITranslation_Rose WeightSelect in SV_TitleFont_FontWeight.Items)
                        {
                            if (WeightSelect.Text == NewCustomLanguageSelection.Properties.TitleFont_FontWeight) SV_TitleFont_FontWeight.SelectedItem = WeightSelect;
                        }
                        
                        foreach (UITranslation_Rose WeightSelect in SV_ContextFont_FontWeight.Items)
                            if (WeightSelect.Text == NewCustomLanguageSelection.Properties.ContextFont_FontWeight) SV_ContextFont_FontWeight.SelectedItem = WeightSelect;
                        

                        SV_ContextFont_SizeMultiplier.Text = $"{NewCustomLanguageSelection.Properties.ContextFont_FontSizeMultipler}";
                        SV_TitleFont_SizeMultiplier.Text = $"{NewCustomLanguageSelection.Properties.TitleFont_FontSizeMultipler}";

                        SV_ContextFont_OverrideName.Text = NewCustomLanguageSelection.Properties.ContextFont_OverrideReadName.Trim();
                        SV_TitleFont_OverrideName.Text = NewCustomLanguageSelection.Properties.TitleFont_OverrideReadName.Trim();


                        SV_SpritesHorizontalOffset.Text = $"{NewCustomLanguageSelection.Properties.KeywordsSpriteHorizontalOffset}";
                        SV_SpritesVerticalOffset.Text = $"{NewCustomLanguageSelection.Properties.KeywordsSpriteVerticalOffset}";


                        SV_KeywordsAutoDetectionRegex.Text = NewCustomLanguageSelection.Properties.Keywords_AutodetectionRegex.Trim();
                        SV_KeywordsMultipleMeaningsDictionary.Text = NewCustomLanguageSelection.Properties.KeywordsMultipleMeaningsDictionary.Trim();
                        SV_ContextMenuExtraReplacements.Text = NewCustomLanguageSelection.Properties.ContextMenuExtraReplacements.Trim();

                        SV_ShorthandsRegex.Text = NewCustomLanguageSelection.Properties.Keywords_ShorthandsRegex.Trim();
                        SV_ShorthandsCMInsertionShape.Text = NewCustomLanguageSelection.Properties.Keywords_ShorthandsContextMenuInsertionShape ?? "";
                        SV_ShorthandsCMInsertionShape_KeywordColor.Text = NewCustomLanguageSelection.Properties.Keywords_ShorthandsContextMenuInsertionShape_HexColor ?? "";

                        ScrollTextBoxesToLeftest();

                        break;


                    case nameof(SV_TitleFont_FontWeight):
                        CurrentCustomLang.TitleFont_FontWeight = (SV_TitleFont_FontWeight.SelectedItem as UITranslation_Rose).Text;
                        Configurazione.LimbusFonts["Limbus:TitleFont_Weight"] = WeightFrom(CurrentCustomLang.TitleFont_FontWeight);
                        break;

                    case nameof(SV_ContextFont_FontWeight):
                        CurrentCustomLang.ContextFont_FontWeight = (SV_ContextFont_FontWeight.SelectedItem as UITranslation_Rose).Text;
                        FontWeight Weight_Context = WeightFrom(CurrentCustomLang.ContextFont_FontWeight);
                        Configurazione.LimbusFonts["Limbus:ContextFont_Weight"] = WeightFrom(CurrentCustomLang.ContextFont_FontWeight);
                        break;
                    #endregion



                    #region ⦁ Internal
                    case nameof(SV_SelectingInterfaceLanguage):
                        string SelectedTranslation = @$"[⇲] Assets Directory\※ Internal\Translation\{(SV_SelectingInterfaceLanguage.SelectedItem as UITranslation_Rose).Text}";
                        ᐁ_Interface_Localization_Loader.ModifyUI(SelectedTranslation);
                        LoadedProgramConfig.Internal.UILanguage = SelectedTranslation.Replace("\\", "/");
                        break;


                    case nameof(SV_SelectingInterfaceTheme):
                        string SelectedTheme = @$"[⇲] Assets Directory\※ Internal\Themes\{(SV_SelectingInterfaceTheme.SelectedItem as UITranslation_Rose).Text}";
                        ᐁ_Interface_Themes_Loader.ModifyUI(SelectedTheme);
                        LoadedProgramConfig.Internal.UITheme = SelectedTheme.Replace("\\", "/");
                        JsonTextEditor.RecompileEditorSyntax();
                        UpdateSpecializedMintSyntaxes();
                        break;
                    #endregion

                    default: break;
                }
            }

            if (!Configurazione.ConfigLoadingEvent)
            {
                Configurazione.SaveConfig();
            }
        }

        private void IncreaseDecreaseSpritesHorOffset(object RequestSender, RoutedEventArgs EventArgs)
        {
            SV_SpritesHorizontalOffset.Text = (CurrentCustomLang.KeywordsSpriteHorizontalOffset + ((RequestSender as Button).Uid == "Up" ? 1 : -1)).ToString();
            Button_SharedPressed(SV_SpritesHorizontalOffset_ConfirmButton, EventArgs);
        }
        private void IncreaseDecreaseSpritesVerOffset(object RequestSender, RoutedEventArgs EventArgs)
        {
            SV_SpritesVerticalOffset.Text = (CurrentCustomLang.KeywordsSpriteVerticalOffset + ((RequestSender as Button).Uid == "Up" ? 1 : -1)).ToString();
            Button_SharedPressed(SV_SpritesVerticalOffset_ConfirmButton, EventArgs);
        }
    }
}
