using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using LCLocalizationInterface.LimbusRegistry.JsonTypes.Specific;
using System.Threading.Tasks;

namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    #region File selection buttons
    public partial class PreviewCreatorPage : Page
    {
        private void ResetSelectedFile(object Sender, MouseButtonEventArgs Args)
        {
            switch ((Sender as Button)!.Name)
            {                                   /////// ugghhhhhhhhhhhhhhhhhhhhhhhhhhh
                case nameof(SelectPortraitImage_Button             ): SelectPortraitImage_Action             ("");                 break;
                case nameof(SelectImageLabelFont_Button            ): SelectImageLabelFont_Action            ("#Bebas Neue Bold"); break;
                case nameof(SelectCautionsFont_Button              ): SelectCautionsFont_Action              ("#Bebas Neue Bold"); break;
                case nameof(SelectItemSignaturesFont_Button        ): SelectTextElementsSignatureFont_Action ("#Bebas Neue Bold"); break;
                case nameof(SelectTextBackgroundEffectsImage_Button): SelectTextBackgroundEffectsImage_Action("")                ; break;
                case nameof(SelectOverlaySketchImage_Button        ): SelectOverlaySketchImage_Action        ("")                ; break;
                case nameof(SelectWalpurgisNightLogoImage_Button   ): SelectWalpurgisNighLogoImage_Action    ("")                ; break;
                case nameof(SelectUpperLeftLogoImage_Button        ): SelectUpperLeftLogoImage_Action        ("")                ; break;
                case nameof(SelectBottomRightLogoImage_Button      ): SelectBottomRightLogoImage_Action      ("")                ; break;
                case nameof(SelectSkillsLocalization_Button        ): SelectSkillsLocalization_Action        ("")                ; break;
                case nameof(SelectSkillsDisplayInfo_Button         ): SelectSkillsDisplayInfo_Action         ("")                ; break;
                case nameof(SelectPassivesLocalization_Button      ): SelectPassivesLocalization_Action      ("")                ; break;
                case nameof(SelectKeywordsLocalization_Button      ): SelectKeywordsLocalization_Action      ("")                ; break;


                default: break;
            }
        }





        private static readonly FontFamily BebasNeueBold = FontFromResource("UI/Fonts/#Bebas Neue Bold");
        private static BitmapImage ExchangeImageFile(string ImagePath, string FallbackAssetPath)
        {
            return File.Exists(ImagePath) ? BitmapFromFile(ImagePath) : BitmapFromResource(FallbackAssetPath);
        }
        private static void ExternFileLabelConditional(string UID, bool Condition, string SuccessExternString)
        {
            @Languages.ExternElementConditional(
                UID: UID,
                Condition: Condition,
                FailureVariableKey: "Default",
                SuccessVariableKey: "Selected",
                SuccessExternString: SuccessExternString
            );
        }

        #region Image parameters / Portrait image
        private void SelectPortraitImage_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
            if (Select.ShowDialog() == true) SelectPortraitImage_Action(Select.FileName);
        }
        public void SelectPortraitImage_Action(string ImagePath)
        {
            IdentityPortrait_Image.Source = EGOPortrait_Image.Source = BitmapFromFile(ImagePath);
            @DataContextDomain.PreviewCreator.ImageInfo.Portrait.ImagePath = ImagePath;

            ExternFileLabelConditional(
                UID: "[C] * [Section:Image parameters / Portrait image] Portrait image (Label)",
                Condition: File.Exists(ImagePath), SuccessExternString: Path.GetFileName(ImagePath)
            );
        }
        #endregion


        #region Image parameters / Image label
        private void SelectImageLabelFont_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);
            if (Select.ShowDialog() == true) SelectImageLabelFont_Action(Select.FileName);
        }
        public void SelectImageLabelFont_Action(string FontPath)
        {
            ImageLabel.FontFamily = FontPath == "#Bebas Neue Bold" ? BebasNeueBold : FontFamilyFromFileOrName(FontPath);
            @DataContextDomain.PreviewCreator.ImageInfo.ImageLabelText.Font = FontPath;

            ExternFileLabelConditional(
                UID: "[C] * [Section:Image parameters / Image label] Image label font (Label)",
                Condition: File.Exists(FontPath), SuccessExternString: GetFontFriendlyNameFromFile(FontPath)
            );
        }
        #endregion


        #region Decorative cautions
        private void SelectCautionsFont_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);
            if (Select.ShowDialog() == true) SelectCautionsFont_Action(Select.FileName);
        }
        public void SelectCautionsFont_Action(string FontPath)
        {
            DecorativeCautions_StyleBindingSource.FontFamily = FontPath == "#Bebas Neue Bold" ? BebasNeueBold : FontFamilyFromFileOrName(FontPath);

            if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false) UnsealCautions();

            @DataContextDomain.PreviewCreator.ImageInfo.Cautions.Font = FontPath;

            if (DecorativeCautionsParametersPanel.IsMouseOver == false)
            {
                if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false) SealCautions();
            }

            ExternFileLabelConditional(
                UID: "[C] * [Section:Decorative cautions] Custom font (Label)",
                Condition: File.Exists(FontPath), SuccessExternString: GetFontFriendlyNameFromFile(FontPath)
            );
        }
        #endregion


        #region Text / General options
        private void SelectItemSignaturesFont_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);
            if (Select.ShowDialog() == true) SelectTextElementsSignatureFont_Action(Select.FileName);
        }
        public async void SelectTextElementsSignatureFont_Action(string FontPath)
        {
            if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false) UnsealAllTextElementsInBothColumns();

            @CurrentPreviewCreator.TextColumnsDynamicResources.SignaturesFont = FontPath == "#Bebas Neue Bold" ? BebasNeueBold : FontFamilyFromFileOrName(FontPath);
            @DataContextDomain.PreviewCreator.ImageInfo.TextColumns.TextElementsSignaturesFont = FontPath;

            @DataContextDomain.PreviewCreator.ImageInfo.Cautions.Font = FontPath;

            ExternFileLabelConditional(
                UID: "[C] * [Section:Textual info/General options] Item signatures font (Label)",
                Condition: File.Exists(FontPath), SuccessExternString: GetFontFriendlyNameFromFile(FontPath)
            );

            if (@CurrentPreviewCreator.IsImageInfoLoadingEvent == false)
            {
                await Task.Delay(150);
                SealAllTextElementsInBothColumns();
            }
        }
        #endregion


        #region Text / Text sources

        private static readonly DropShadowEffect SelectionLabelsEffect = new() { ShadowDepth = 2, Color = ToColor("#191919"), BlurRadius = 1 };


        private static Dictionary<BigInteger, PlainSkill.UptieLevel> LoadedSkills { get; } = new();
        private void SelectSkillsLocalization_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Limbus Json localization files (Skills)", ["json"]);
            if (Select.ShowDialog() == true) SelectSkillsLocalization_Action(Select.FileName);
        }
        public void SelectSkillsLocalization_Action(string FilePath)
        {
            @DataContextDomain.PreviewCreator.ImageInfo.TextColumns.SelectedFiles.SkillsLocalization = FilePath;

            static bool Validator(PlainSkill Skill) => Skill.ID is not null & Skill.UptieLevels.Count > 0;

            LoadedSkills.Clear();
            ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance.SkillLocalizationIDSelector.Items.Clear();

            if (File.Exists(FilePath))
            {
                if (new FileInfo(FilePath).TryDeserealizeJsonAs(out LimbusLocalizationFile<PlainSkill> SkillsLocalization, out Exception Occurred) && SkillsLocalization.DataList.Any(Validator))
                {
                    @Languages.ExternElement(
                        UID: "[C] * [Section:Textual info/Text sources] Skills localization (Label)",
                        VariableKey: "Selected",
                        ExternObject: Path.GetFileName(FilePath)
                    );
                    
                    foreach (PlainSkill Skill in SkillsLocalization.DataList.Where(Validator))
                    {
                        BigInteger ID = (BigInteger)Skill.ID!;

                        if (!LoadedSkills.ContainsKey(ID))
                        {
                            PlainSkill.UptieLevel TargetUptie = Skill.UptieLevels.Last();
                            IntenseStareType1 SelectionLabel = new() { DataContext = TargetUptie, Effect = SelectionLabelsEffect };

                            LoadedSkills[ID] = TargetUptie;

                            SelectionLabel.InherintPropertiesFrom(@Languages.PresentedTextElements["[C] * [Column element creation dialog] Skill ID from localization file"]);
                            SelectionLabel.RichText = $"{Skill.ID}: <color={GetAffinityColor(TargetUptie.OptionalAffinity, Fallback: @Themes.CurrentTheme.UIText.Foreground_DialogWindows)}>{TargetUptie.Name}</color>";
                            SelectionLabel.Padding = new Thickness(1, 4, 0, 4);
                            SelectionLabel.Uid = $"{Skill.ID}";


                            ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance.SkillLocalizationIDSelector.Items.Add(SelectionLabel);
                        }
                    }
                }
                else
                {
                    ErrorMessageWindow.ShowException(Occurred);
                }

            }
            else
            {
                @Languages.ExternElement(UID: "[C] * [Section:Textual info/Text sources] Skills localization (Label)", VariableKey: "Default");
            }
        }



        private static Dictionary<BigInteger, SkillConstructor> LoadedSkillsDisplayInfo { get; } = new();
        private void SelectSkillsDisplayInfo_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("LCLI Skills Display Info files", ["json"]);
            if (Select.ShowDialog() == true) SelectSkillsDisplayInfo_Action(Select.FileName);
        }
        public void SelectSkillsDisplayInfo_Action(string FilePath)
        {
            @DataContextDomain.PreviewCreator.ImageInfo.TextColumns.SelectedFiles.SkillsDisplayInfo = FilePath;

            static bool Validator(SkillConstructor Constructor) => Constructor.ID is not null;

            LoadedSkillsDisplayInfo.Clear();
            ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance.SkillConstructorIDSelector.Items.Clear();

            if (File.Exists(FilePath))
            {
                if (new FileInfo(FilePath).TryDeserealizeJsonAs(out SkillsDisplayInfoJson SkillsDisplayInfo, out Exception Occurred, Context: Path.GetDirectoryName(FilePath)!.Replace("\\", "/")) && SkillsDisplayInfo.List.Count > 0)
                {
                    @Languages.ExternElement(
                        UID: "[C] * [Section:Textual info/Text sources] Skills Display Info (Label)",
                        VariableKey: "Selected",
                        ExternObject: Path.GetFileName(FilePath)
                    );

                    foreach (SkillConstructor Constructor in SkillsDisplayInfo.List.Where(Validator))
                    {
                        BigInteger ID = (BigInteger)Constructor.ID!;
                        if (!LoadedSkillsDisplayInfo.ContainsKey(ID))
                        {
                            LoadedSkillsDisplayInfo[ID] = Constructor;

                            IntenseStareType1 SelectionLabel = new() { DataContext = Constructor, Effect = SelectionLabelsEffect };

                            SelectionLabel.InherintPropertiesFrom(@Languages.PresentedTextElements["[C] * [Column element creation dialog] Skill Constructor ID from Display Info"]);
                            SelectionLabel.RichText = $"{Constructor.ID}: <color={GetAffinityColor($"{Constructor.Specific.Affinity}", Fallback: @Themes.CurrentTheme.UIText.Foreground_DialogWindows)}>{Constructor.SkillName}</color>";
                            SelectionLabel.Padding = new Thickness(1, 4, 0, 4);
                            SelectionLabel.Uid = $"{Constructor.ID}";


                            ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance.SkillConstructorIDSelector.Items.Add(SelectionLabel);
                        }
                    }
                }
                else
                {
                    ErrorMessageWindow.ShowException(Occurred);
                }
            }
            else
            {
                @Languages.ExternElement(UID: "[C] * [Section:Textual info/Text sources] Skills Display Info (Label)", VariableKey: "Default");
            }
        }



        private static Dictionary<BigInteger, PlainPassive> LoadedPassives { get; } = new();
        private void SelectPassivesLocalization_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Limbus Json localization files (Passives)", ["json"]);
            if (Select.ShowDialog() == true) SelectPassivesLocalization_Action(Select.FileName);
        }
        public void SelectPassivesLocalization_Action(string FilePath)
        {
            @DataContextDomain.PreviewCreator.ImageInfo.TextColumns.SelectedFiles.PassivesLocalization = FilePath;

            static bool Validator(PlainPassive Passive) => Passive.ID is not null;

            LoadedPassives.Clear();
            ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance.PassiveLocalizationIDSelector.Items.Clear();

            if (File.Exists(FilePath))
            {
                if (new FileInfo(FilePath).TryDeserealizeJsonAs(out LimbusLocalizationFile<PlainPassive> PassivesLocalization, out Exception Occurred) && PassivesLocalization.DataList.Any(Validator))
                {
                    @Languages.ExternElement(
                        UID: "[C] * [Section:Textual info/Text sources] Passives localization (Label)",
                        VariableKey: "Selected",
                        ExternObject: Path.GetFileName(FilePath)
                    );

                    foreach (PlainPassive Passive in PassivesLocalization.DataList.Where(Validator))
                    {
                        BigInteger ID = (BigInteger)Passive.ID!;
                        if (!LoadedPassives.ContainsKey(ID))
                        {
                            LoadedPassives[ID] = Passive;

                            IntenseStareType1 SelectionLabel = new() { DataContext = Passive, Effect = SelectionLabelsEffect };

                            SelectionLabel.InherintPropertiesFrom(@Languages.PresentedTextElements["[C] * [Column element creation dialog] Passive ID from localization file"]);
                            SelectionLabel.RichText = $"{Passive.ID}: {Passive.Name}";
                            SelectionLabel.Padding = new Thickness(1, 4, 0, 4);
                            SelectionLabel.Uid = $"{Passive.ID}";


                            ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance.PassiveLocalizationIDSelector.Items.Add(SelectionLabel);
                        }
                    }
                }
                else
                {
                    ErrorMessageWindow.ShowException(Occurred);
                }
            }
            else
            {
                @Languages.ExternElement(UID: "[C] * [Section:Textual info/Text sources] Passives localization (Label)", VariableKey: "Default");
            }
        }



        private static Dictionary<string, PlainKeyword> LoadedKeywords { get; } = new();
        private void SelectKeywordsLocalization_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Limbus Json localization files (Keywords)", ["json"]);
            if (Select.ShowDialog() == true) SelectKeywordsLocalization_Action(Select.FileName);
        }
        public void SelectKeywordsLocalization_Action(string FilePath)
        {
            @DataContextDomain.PreviewCreator.ImageInfo.TextColumns.SelectedFiles.KeywordsLocalization = FilePath;

            static bool Validator(PlainKeyword keyword) => keyword.ID is not null;

            LoadedKeywords.Clear();
            ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance.KeywordLocalizationIDSelector.Items.Clear();

            if (File.Exists(FilePath))
            {
                if (new FileInfo(FilePath).TryDeserealizeJsonAs(out LimbusLocalizationFile<PlainKeyword> KeywordsLocalization, out Exception Occurred) && KeywordsLocalization.DataList.Any(Validator))
                {
                    @Languages.ExternElement(
                        UID: "[C] * [Section:Textual info/Text sources] Keywords localization (Label)",
                        VariableKey: "Selected",
                        ExternObject: Path.GetFileName(FilePath)
                    );

                    foreach (PlainKeyword Keyword in KeywordsLocalization.DataList.Where(Validator))
                    {
                        if (!LoadedKeywords.ContainsKey(Keyword.ID!))
                        {
                            LoadedKeywords[Keyword.ID!] = Keyword;

                            IntenseStareType1 SelectionLabel = new() { DataContext = Keyword, Effect = SelectionLabelsEffect };

                            string DefinedColor = Keyword.Color ?? ColorDictionaries.KeywordColors[Keyword.ID!];

                            SelectionLabel.InherintPropertiesFrom(@Languages.PresentedTextElements["[C] * [Column element creation dialog] Keyword ID from localization file"]);
                            SelectionLabel.RichText = $"{Keyword.ID}: <color={DefinedColor}>{Keyword.Name}</color>";
                            SelectionLabel.Padding = new Thickness(1, 4, 0, 4);
                            SelectionLabel.Uid = $"{Keyword.ID}";


                            ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance.KeywordLocalizationIDSelector.Items.Add(SelectionLabel);
                        }
                    }
                }
                else
                {
                    ErrorMessageWindow.ShowException(Occurred);
                }
            }
            else
            {
                @Languages.ExternElement(UID: "[C] * [Section:Textual info/Text sources] Keywords localization (Label)", VariableKey: "Default");
            }
        }



        private void ReloadLocalizationFilesAndRebuildText(object Sender, RoutedEventArgs Args)
        {
#pragma warning disable SYSLIB0050
            @DataContextDomain.PreviewCreator.ImageInfo.TextColumns.SelectedFiles.HandleRelativePaths_OnRead(new StreamingContext(StreamingContextStates.Other, RecentlySerializedImageInfo_Directory));
            // Restore paths to these files from relatives if image info was saved before clicking this option
#pragma warning restore SYSLIB0050
            SelectSkillsLocalization_Action(@DataContextDomain.PreviewCreator.ImageInfo.TextColumns.SelectedFiles.SkillsLocalization);
            SelectSkillsDisplayInfo_Action(@DataContextDomain.PreviewCreator.ImageInfo.TextColumns.SelectedFiles.SkillsDisplayInfo);
            SelectPassivesLocalization_Action(@DataContextDomain.PreviewCreator.ImageInfo.TextColumns.SelectedFiles.PassivesLocalization);
            SelectKeywordsLocalization_Action(@DataContextDomain.PreviewCreator.ImageInfo.TextColumns.SelectedFiles.KeywordsLocalization);

            RebuildTextElements();
        }
        #endregion


        #region Text background effects
        private void SelectTextBackgroundEffectsImage_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
            if (Select.ShowDialog() == true) SelectTextBackgroundEffectsImage_Action(Select.FileName);
        }
        public void SelectTextBackgroundEffectsImage_Action(string ImagePath)
        {
            @DataContextDomain.PreviewCreator.ImageInfo.TextBackgroundEffects.ImagePath = ImagePath;

           _ = TextBackgroundEffects_Image_IDENTITY.Source
             = TextBackgroundEffects_Image_EGO.Source
             = BitmapFromFile(ImagePath);

            ExternFileLabelConditional(
                UID: "[C] * [Section:Text background effects] Effects image (Label)",
                Condition: File.Exists(ImagePath), SuccessExternString: Path.GetFileName(ImagePath)
            );
        }
        #endregion


        #region Overlay sketch
        private void SelectOverlaySketchImage_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
            if (Select.ShowDialog() == true) SelectOverlaySketchImage_Action(Select.FileName);
        }
        public void SelectOverlaySketchImage_Action(string ImagePath)
        {
            @DataContextDomain.PreviewCreator.ImageInfo.OverlaySketch.ImagePath = ImagePath;
            OverlaySketchImage.Source = BitmapFromFile(ImagePath);

            ExternFileLabelConditional(
                UID: "[C] * [Section:Overlay sketch] Sketch image (Label)",
                Condition: File.Exists(ImagePath), SuccessExternString: Path.GetFileName(ImagePath)
            );
        }
        #endregion


        #region Other effects

        private void SelectWalpurgisNighLogoImage_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
            if (Select.ShowDialog() == true) SelectWalpurgisNighLogoImage_Action(Select.FileName);
        }
        public void SelectWalpurgisNighLogoImage_Action(string ImagePath)
        {
            @DataContextDomain.PreviewCreator.ImageInfo.OtherEffects.WalpurgisNightLogoImage = ImagePath;
            WalpurgisNightLogo.Source = ExchangeImageFile(ImagePath, "Limbus Registry/※ Identity ¦ E.G.O  Preview Creator/Images/Walpurgis Night/Logo.png");

            ExternFileLabelConditional(
                UID: "[C] * [Section:Other effects] Walpurgis Night logo image (Label)",
                Condition: File.Exists(ImagePath), SuccessExternString: Path.GetFileName(ImagePath)
            );
        }


        private void SelectUpperLeftLogoImage_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
            if (Select.ShowDialog() == true) SelectUpperLeftLogoImage_Action(Select.FileName);
        }
        public void SelectUpperLeftLogoImage_Action(string ImagePath)
        {
            @DataContextDomain.PreviewCreator.ImageInfo.OtherEffects.UpperLeftLogoImage = ImagePath;
            Logo_LeftTopCorner.Source = ExchangeImageFile(ImagePath, "Limbus Registry/※ Identity ¦ E.G.O  Preview Creator/Images/Logo/Left Top Corner.png");

            ExternFileLabelConditional(
                UID: "[C] * [Section:Other effects] Upper left logo image (Label)",
                Condition: File.Exists(ImagePath), SuccessExternString: Path.GetFileName(ImagePath)
            );
        }


        private void SelectBottomRightLogoImage_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
            if (Select.ShowDialog() == true) SelectBottomRightLogoImage_Action(Select.FileName);
        }
        public void SelectBottomRightLogoImage_Action(string ImagePath)
        {
            @DataContextDomain.PreviewCreator.ImageInfo.OtherEffects.BottomRightLogoImage = ImagePath;
            Logo_RightBottomCorner.Source = ExchangeImageFile(ImagePath, "Limbus Registry/※ Identity ¦ E.G.O  Preview Creator/Images/Logo/Right Bottom Corner.png");

            ExternFileLabelConditional(
                UID: "[C] * [Section:Other effects] Bottom right logo image (Label)",
                Condition: File.Exists(ImagePath), SuccessExternString: Path.GetFileName(ImagePath)
            );
        }
        #endregion
    }
    #endregion
}