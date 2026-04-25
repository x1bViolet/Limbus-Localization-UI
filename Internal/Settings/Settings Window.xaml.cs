using ICSharpCode.AvalonEdit.Highlighting;
using LCLocalizationInterface.Internal.UIStyle;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using LCLocalizationInterface.LimbusRegistry.PreviewCreator;
using System.Diagnostics.CodeAnalysis;
using static RijnadelClassLibrary.SyntaxedTextEditorBase;

namespace LCLocalizationInterface.Internal.Configuration
{
    public partial class SettingsWindow : Abstractions.FadeableWindow
    {
        #pragma warning disable CS8618
        public static SettingsWindow SettingsWindowInstance;
        #pragma warning restore CS8618

        public SettingsWindow()
        {
            InitializeComponent();
            SettingsWindowInstance = this;

            CreateDecorativeCautions();
        }

        private const int SpecificSettingsCautionSegmentsCount = 10;
        private void CreateDecorativeCautions()
        {
            for (int i = 0; i <= SpecificSettingsCautionSegmentsCount; i++)
            {
                SpecificSettingsCautions_TopStackPanel.Children.Add(CreateCautionElement(AddRightMargin: i < SpecificSettingsCautionSegmentsCount));
                SpecificSettingsCautions_BottomStackPanel.Children.Add(CreateCautionElement(AddRightMargin: i < SpecificSettingsCautionSegmentsCount));
            }

            static StackPanel CreateCautionElement(bool AddRightMargin = true)
            {
                
                TextBlock PART_SlashChars = new()
                {
                    FontFamily = FontFromResource("UI/Fonts/#Sappy"),
                    Margin = new Thickness(0, 1, 8.3, 0), FontSize = 12.5,
                    Text = "////"
                };
                TextBlock PART_Text = new()
                {
                    FontFamily = FontFromResource("UI/Fonts/#Bebas Neue Bold"),
                    Margin = new Thickness(0, 0, 0, 0), FontSize = 13,
                    Text = "CAUTION"
                };
                PART_SlashChars.SetResourceReference(TextBlock.ForegroundProperty, "CautionsForeground");
                PART_Text.SetResourceReference(TextBlock.ForegroundProperty, "CautionsForeground");
                return new()
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, AddRightMargin ? 8.3 : 0, 0),
                    Children = { PART_SlashChars, PART_Text }
                };
            }
        }
        private void StartSpecificSettingsCautionAnimation()
        {
            SpecificSettingsCautions_TopStackPanel.BeginAnimation(Canvas.RightProperty, new DoubleAnimation()
            {
                From = 0, To = -(SpecificSettingsCautions_TopStackPanel.ActualWidth - 377 + 47 - 84.5), //uuuuggghghh
                Duration = TimeSpan.FromSeconds(SpecificSettingsCautionSegmentsCount * 2), RepeatBehavior = RepeatBehavior.Forever
            });
            SpecificSettingsCautions_BottomStackPanel.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation()
            {
                From = 0, To = -SpecificSettingsCautions_BottomStackPanel.ActualWidth + 377 - 47 + 84.5,
                Duration = TimeSpan.FromSeconds(SpecificSettingsCautionSegmentsCount * 2), RepeatBehavior = RepeatBehavior.Forever
            });
        }
        private void StopSpecificSettingsCautionAnimation()
        {
            SpecificSettingsCautions_TopStackPanel.BeginAnimation(Canvas.RightProperty, null);
            SpecificSettingsCautions_BottomStackPanel.BeginAnimation(Canvas.LeftProperty, null);
        }



        #region FadeableWindow members
        public override void BeginFadeShowing()
        {
            base.BeginFadeShowing();
            StartSpecificSettingsCautionAnimation();
        }

        public override List<Action> AdditionalFadeOutCompleteActions => [this.StopSpecificSettingsCautionAnimation];

        protected override (double In, double Out) FadeDurations => AsPair(ThemeTimings.Duration.SettingsWindow);
        protected override (double In, double Out) FadeSpeedRatios => AsPair(ThemeTimings.SpeedRatio.SettingsWindow);
        protected override ((double Acceleration, double Deceleation) In, (double Acceleration, double Deceleation) Out) FadeKinematics
            => AsPairPair(ThemeTimings.AccelerationDecelerationRatios.SettingsWindow);
        #endregion



        public void UpdateSyntaxHighlightColors()
        {
            var SyntaxColors = @Themes.CurrentTheme.UITextfields.Syntax;
            static void SetHighlight(IntenseStareType3 Target, [StringSyntax(StringSyntaxAttribute.Regex)] string Pattern, string Foreground)
            {
                Target.SyntaxHighlighting.MainRuleSet.Rules.Add(new HighlightingRule()
                {
                    Regex = new Regex(Pattern),
                    Color = new HighlightingColor() { Foreground = new HighlightionBrush(Foreground), Underline = true }
                });
            }
            @Languages.PresentedTextFields["[Settings / Custom Language] * Keywords autodetection Regex pattern"].SyntaxHighlighting = new SyntaxedTextEditorBase.SyntaxHighlightDefinition();
            @Languages.PresentedTextFields["[Settings / Custom Language] * Shorthands Regex pattern"].SyntaxHighlighting = new SyntaxedTextEditorBase.SyntaxHighlightDefinition();
            @Languages.PresentedTextFields["[Settings / Custom Language] * Shorthands Context Menu insertion shape"].SyntaxHighlighting = new SyntaxedTextEditorBase.SyntaxHighlightDefinition();

            SetHighlight(@Languages.PresentedTextFields["[Settings / Custom Language] * Keywords autodetection Regex pattern"], @"KeywordNameWillBeHere", SyntaxColors.Highlight1);

            SetHighlight(@Languages.PresentedTextFields["[Settings / Custom Language] * Shorthands Regex pattern"], @"\(\?\<ID\>\\w\+\)", SyntaxColors.Highlight1);
            SetHighlight(@Languages.PresentedTextFields["[Settings / Custom Language] * Shorthands Regex pattern"], @"\?<Name>", SyntaxColors.Highlight2);
            SetHighlight(@Languages.PresentedTextFields["[Settings / Custom Language] * Shorthands Regex pattern"], @"\?<Color>", SyntaxColors.Highlight3);
            SetHighlight(@Languages.PresentedTextFields["[Settings / Custom Language] * Shorthands Regex pattern"], @"\?<SpriteID>", SyntaxColors.Highlight4);

            SetHighlight(@Languages.PresentedTextFields["[Settings / Custom Language] * Shorthands Context Menu insertion shape"], @"<KeywordID>", SyntaxColors.Highlight1);
            SetHighlight(@Languages.PresentedTextFields["[Settings / Custom Language] * Shorthands Context Menu insertion shape"], @"<KeywordName>", SyntaxColors.Highlight2);
        }


        #region Title bar and interactions
        private void ReloadConfiguration(object Sender, RoutedEventArgs Args)
        {
            @Configurazione.Load();
        }
        #endregion

        /// <summary>Sections collapse/expand</summary>
        private void ToggleSecondChildVisibility(object Sender, MouseButtonEventArgs Args)
        {
            UIElement Target = ((Sender as IntenseStareType1)!.Parent as StackPanel)!.Children[1];
            Target.Visibility = Target.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private string SelectNumberFormat(string Input) => new([.. Input.Replace(".", ",").Where(c => char.IsDigit(c) | c.EqualsToOneOf('-', '+', ','))]);

        // Default click event for "Theme:ControlStyles.Spec.ConfirmValueButton"
        private void Buttons_AutoSaveConfig(object Sender, RoutedEventArgs Args)
        {
            if (Sender != ShowCustomLangExtendedButtons)
            {
                @Configurazione.Save();
            }
        }

        private void SaveAndDeploy_Common_UpdateLimbusCustomLang(object Sender, RoutedEventArgs Args)
        {
            @PartialStateUpdater.Limbus.UpdateFull();
        }




        #region ⦁ Limbus Preview (Checkboxes)
        private void Section_LimbusPreview_CheckBoxesCheckedUnchecked(object Sender, RoutedEventArgs Args)
        {
            CheckBox ActualSender = (Sender as CheckBox)!;
            bool IsSenderChecked = ActualSender.IsChecked ?? false;

            IntenseStareType1 Descriptor = ((ActualSender.Parent as TwoColumned)!.Children[0] as IntenseStareType1)!;
            switch (Descriptor.UID)
            {
                case "[Settings / Editor Parameters] * Toggle Syntax Highlight":
                    if (SelectedLimbusCustomLanguage is not null & ProgramFullyLoaded)
                    {
                        JsonTextEditor.@LimbusTextSyntaxesPreset.GenerateSyntaxes();
                    }
                    break;


                case "[Settings / Editor Parameters] * Hide Limbus Text Preview":
                    MainWindowInstance.RichTextViews__PARENT_Height.MaxHeight = (bool)ActualSender.IsChecked! ? 0 : double.MaxValue;
                    break;


                case "[Settings / Editor Parameters] * Highlight style":
                    if (SelectedLimbusCustomLanguage is not null & ProgramFullyLoaded)
                    {
                        JsonTextEditor.@LimbusTextSyntaxesPreset.GenerateSyntaxes();
                        using (TMProEmitter.DisabledRichTextDelay)
                        {
                            @EditorModesShelf.CurrentEditorMode.RefreshRichText();
                        }
                    }
                    break;


                case "[Settings / Editor Parameters] * Enable Skill Names Replication":
                    if (ProgramFullyLoaded)
                    {
                        if (IsSenderChecked && @EditorModesShelf.Skills.CurrentFile is not null)
                        {
                            @EditorModesShelf.Skills.ChangeSkillNameReplicaAppearance();
                        }
                    }
                    break;


                case "[Settings / Editor Parameters] * Enable Keyword tooltips":
                    if (ProgramFullyLoaded)
                    {
                        using (TMProEmitter.DisabledRichTextDelay)
                        {
                            @EditorModesShelf.CurrentEditorMode.RefreshRichText();
                        }
                    }
                    break;

                default: break;
            }

            @Configurazione.Save();
        }
        
        public void SaveAndDeploy_LimbusPreviewDelay(object Sender, RoutedEventArgs Args)
        {
            string NumberFormat = SelectNumberFormat(@Languages.PresentedTextFields["[Settings / Editor Parameters] * Preview update delay"].Text);
            LoadedConfiguration.PreviewSettings.Base.PreviewUpdateDelay = double.TryParse(NumberFormat, out double NewValue) ? NewValue : 0;
            @Languages.PresentedTextFields["[Settings / Editor Parameters] * Preview update delay"].Document.Text = $"{LoadedConfiguration.PreviewSettings.Base.PreviewUpdateDelay}";
        }
        #endregion






        #region ⦁ Limbus Custom Language

        #region Main selection
        private bool LimbusCustomLanguageSelectionEvent { get; set; } = false;
        private void LimbusCustomLanguageSelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
        {
            LimbusCustomLanguageSelectionEvent = true;
            {
                if (Args.AddedItems.Count > 0)
                {
                    var NewLanguageSelection = ((Args.AddedItems[0] as IntenseStareType1)!.DataContext as @Configurazione.JsonConfigurationFile.LimbusCustomLangDefinition)!;
                    var Properties = NewLanguageSelection.Properties;
                    @DataContextDomain.SelectedLimbusCustomLanguage = Properties;

                    LoadedConfiguration.PreviewSettings.CustomLang.AssociativeSettings.Selected = NewLanguageSelection.Name;

                    // Due to strange interaction between text binding to double
                    @Languages.PresentedTextFields["[Settings / Custom Language] * Sprites vertical offset"].Document.Text = $"{Properties.KeywordsSpriteVerticalOffset}";
                    @Languages.PresentedTextFields["[Settings / Custom Language] * Sprites horizontal offset"].Document.Text = $"{Properties.KeywordsSpriteHorizontalOffset}";

                    @PartialStateUpdater.Limbus.UpdateFull();

                    @Configurazione.Save();

                    foreach (IntenseStareType3 LimbusCustomLangTextSetting in SettingsSectionsStackPanel.FindVisualChildren<IntenseStareType3>())
                    {
                        LimbusCustomLangTextSetting.Document.UndoStack.ClearAll();
                        LimbusCustomLangTextSetting.ScrollToHorizontalOffset(double.PositiveInfinity);
                    }
                }
            }
            LimbusCustomLanguageSelectionEvent = false;
        }
        private void LimbusCustomLanguage_ReloadLocalizationFiles(object Sender, RoutedEventArgs Args) => @PartialStateUpdater.Limbus.UpdateFull();
        #endregion



        #region Keyword directories
        /// -> <see cref="SaveAndDeploy_Common_UpdateLimbusCustomLang"/>
        #endregion



        #region Fonts

        #region Files
        private void SaveAndDeploy_TitleFont(object Sender, RoutedEventArgs Args)
        {
            @PartialStateUpdater.Limbus.Fonts.FontsUpdate_FontFamilies();
        }
        private void SaveAndDeploy_ContextFont(object Sender, RoutedEventArgs Args)
        {
            @PartialStateUpdater.Limbus.Fonts.FontsUpdate_FontFamilies();
        }
        #endregion

        #region Weights
        private void TitleFontWeightSelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
        {
            if (LimbusCustomLanguageSelectionEvent == false)
            {
                @PartialStateUpdater.Limbus.Fonts.FontsUpdate_FontWeights();
                @Configurazione.Save();
            }
        }
        private void ContextFontWeightSelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
        {
            if (LimbusCustomLanguageSelectionEvent == false)
            {
                @PartialStateUpdater.Limbus.Fonts.FontsUpdate_FontWeights();
                @Configurazione.Save();
            }
        }
        #endregion

        #endregion



        #region Sprites offset
        private void ChangeSpritesHorizontalOffset(object Sender, RoutedEventArgs Args)
        {
            string NumberFormat = SelectNumberFormat(@Languages.PresentedTextFields["[Settings / Custom Language] * Sprites horizontal offset"].Text);
            DataContextDomain.SelectedLimbusCustomLanguage!.KeywordsSpriteHorizontalOffset = double.TryParse(NumberFormat, out double NewValue) ? NewValue : 0;
            @Languages.PresentedTextFields["[Settings / Custom Language] * Sprites horizontal offset"].Document.Text = $"{DataContextDomain.SelectedLimbusCustomLanguage.KeywordsSpriteHorizontalOffset}";
            @PartialStateUpdater.Limbus.FullyRefreshShownRichText();
        }
        private void ChangeSpritesVerticalOffset(object Sender, RoutedEventArgs Args)
        {
            string NumberFormat = SelectNumberFormat(@Languages.PresentedTextFields["[Settings / Custom Language] * Sprites vertical offset"].Text);
            DataContextDomain.SelectedLimbusCustomLanguage!.KeywordsSpriteVerticalOffset = double.TryParse(NumberFormat, out double NewValue) ? NewValue : 0;
            @Languages.PresentedTextFields["[Settings / Custom Language] * Sprites vertical offset"].Document.Text = $"{DataContextDomain.SelectedLimbusCustomLanguage.KeywordsSpriteVerticalOffset}";
            @PartialStateUpdater.Limbus.FullyRefreshShownRichText();
        }
        #endregion



        #region etc.

        private void SaveAndDeploy_KeywordsAutodetectionRegex(object Sender, RoutedEventArgs Args)
        {
            @PartialStateUpdater.Limbus.FullyRefreshShownRichText();
        }


        private void SaveAndDeploy_KeywordsMultipleMeaningsDictionary(object Sender, RoutedEventArgs Args)
        {
            @PartialStateUpdater.Limbus.Additions.UpdateMultipleMeaningsDictionary(UpdateKeywordsRightAfter: true);
        }
        private void SaveAndDeploy_ExtraReplacements(object Sender, RoutedEventArgs Args)
        {
            @PartialStateUpdater.Limbus.Additions.UpdateExtraReplacements();
        }


        private void SaveAndDeploy_ShorthandsPattern(object Sender, RoutedEventArgs Args)
        {
            @PartialStateUpdater.Limbus.FullyRefreshShownRichText();
        }
        private void SaveAndDeploy_ShorthandsPatternInsertionShape(object Sender, RoutedEventArgs Args)
        {
            /// -> <see cref="Buttons_AutoSaveConfig"/>
        }

        #endregion

        #endregion






        #region ⦁ Internal
        private void InterfaceLanguageSelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
        {
            if (Args.AddedItems.Count > 0 && Args.AddedItems[0] is string NewLanguageSelectionPath && Directory.Exists(@$"[⇲] Assets Directory\※ Internal\Translation\{NewLanguageSelectionPath}"))
            {
                @Languages.ModifyUI(new DirectoryInfo($"[⇲] Assets Directory/※ Internal/Translation/{NewLanguageSelectionPath}"));

                LoadedConfiguration.Internal.UILanguage = $"[⇲] Assets Directory/※ Internal/Translation/{NewLanguageSelectionPath}";
                @Configurazione.Save();
            }
        }
        private void InterfaceThemeSelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
        {
            if (Args.AddedItems.Count > 0 && Args.AddedItems[0] is string NewThemeSelection && Directory.Exists(@$"[⇲] Assets Directory\※ Internal\Themes\{NewThemeSelection}"))
            {
                @Themes.LoadFromFile($"[⇲] Assets Directory/※ Internal/Themes/{NewThemeSelection}/Visual.json");

                LoadedConfiguration.Internal.UITheme = $"[⇲] Assets Directory/※ Internal/Themes/{NewThemeSelection}";
                @Configurazione.Save();
            }
        }
        
        private void TompostWindow_CheckedUnchecked(object Sender, RoutedEventArgs Args)
        {
            if (@CurrentPreviewCreator.ActiveState == false)
            {
                MainWindowInstance.Topmost = (bool)(Sender as CheckBox)!.IsChecked!;

                if (MainWindowInstance.WindowState is not WindowState.Minimized & @CurrentPreviewCreator.ActiveState == false)
                {
                    SettingsWindowInstance.WindowState = WindowState.Minimized;
                    SettingsWindowInstance.WindowState = WindowState.Normal;
                }
            }
        }

        private void SkipSkillsDataReadig_Unchecked(object Sender, RoutedEventArgs Args)
        {
            if (ProgramFullyLoaded)
            {
                if (@SkillsData.ReadedSkillsData == ReadOnlyDictionary<BigInteger, @SkillsData.SkillsDataFileJson.SkillDataItem>.Empty)
                {
                    // Let tooltip disappear
                    Await(0.3, CompleteAction: delegate ()
                    {
                        @SkillsData.ReadSkillsDataFiles();
                        if (@EditorModesShelf.Skills.CurrentFile is not null)
                        {
                            @EditorModesShelf.Skills.ChangeSkillNameReplicaAppearance();
                        }
                    });
                }
            }
        }

        private void DisableUnknownForUnidentifiedKeywords_CheckedUnchecked(object Sender, RoutedEventArgs Args)
        {
            if (ProgramFullyLoaded)
            {
                @PartialStateUpdater.Limbus.FullyRefreshShownRichText();
                JsonTextEditor.@LimbusTextSyntaxesPreset.GenerateSyntaxes();
            }
        }

        private void DisableTextElementsSealingInPreviewCreator_CheckedUnchecked(object Sender, RoutedEventArgs Args)
        {
            if (PreviewCreatorPage.PreviewCreatorPageInstance.IsLoaded)
            {
                if ((Sender as CheckBox)!.IsChecked == true)
                {
                    //PreviewCreatorPage.PreviewCreatorPageInstance.UnsealAllTextElementsInBothColumns();

                    PreviewCreatorPage.PreviewCreatorPageInstance.RebuildTextElements();
                    PreviewCreatorPage.PreviewCreatorPageInstance.UnsealCautions();
                }
                else
                {
                    PreviewCreatorPage.PreviewCreatorPageInstance.RebuildTextElements();
                    PreviewCreatorPage.PreviewCreatorPageInstance.SealCautions();
                }
            }
        }


        private void EnableAutoSave_Checked(object Sender, RoutedEventArgs Args)
        {
            if (ProgramFullyLoaded)
            {
                @EditorModesShelf.CurrentEditorMode.SaveCurrentFile_Entry();
            }
        }

        public void SaveAndDeploy_AutoSaveDelay(object Sender, RoutedEventArgs Args)
        {
            string NumberFormat = SelectNumberFormat(@Languages.PresentedTextFields["[Settings / Editor Parameters] * Autosave delay"].Text);
            LoadedConfiguration.Internal.AutosaveDelay = double.TryParse(NumberFormat, out double NewValue) ? NewValue : 0.15;
            LoadedConfiguration.Internal.AssertAutoSaveDelay();
            @Languages.PresentedTextFields["[Settings / Editor Parameters] * Autosave delay"].Document.Text = $"{LoadedConfiguration.Internal.AutosaveDelay}";
        }
        #endregion






        #region ⦁ Image Exports
        private void SaveAndDeploy_ImageExportsScale(object Sender, RoutedEventArgs Args)
        {
            string NumberFormat = SelectNumberFormat(@Languages.PresentedTextFields["[Settings / Image Exports] * Scale Factor"].Text);
            LoadedConfiguration.ScanParameters.ScaleFactor = double.TryParse(NumberFormat, out double NewValue) ? NewValue : 4.5;
            LoadedConfiguration.ScanParameters.AssertScaleFactor();
            @Languages.PresentedTextFields["[Settings / Image Exports] * Scale Factor"].Document.Text = $"{LoadedConfiguration.ScanParameters.ScaleFactor}";
            /// -> <see cref="Buttons_AutoSaveConfig"/>
        }
        private void SaveAndDeploy_BackgroundColor(object Sender, RoutedEventArgs Args)
        {
            /// -> <see cref="Buttons_AutoSaveConfig"/>
        }
        private void SaveAndDeploy_PreloadImageInfoForPreviewCreator(object Sender, RoutedEventArgs Args)
        {
            string NewPath = LoadedConfiguration.ScanParameters.PreloadedImageInfoForPreviewCreator;
            if (!string.IsNullOrWhiteSpace(NewPath) && File.Exists(NewPath) == false)
            {
                ErrorMessageWindow.ShowException(new FileNotFoundException("File not found"), $"Specified preload Image Info file for Identity/E.G.O Preview Creator does not exists.");
            }
            /// -> <see cref="Buttons_AutoSaveConfig"/>
        }

        private void ToggleKeywordsUnderlineOrSprite_CheckedUnchecked(object Sender, RoutedEventArgs Args)
        {
            @PartialStateUpdater.Limbus.FullyRefreshShownRichText();
            /// -> <see cref="Buttons_AutoSaveConfig"/>
        }
        #endregion
    }
}