using LCLocalizationInterface.LimbusRegistry.JsonTypes;

namespace LCLocalizationInterface.LimbusRegistry
{
    public static class KeywordsLoader
    {
        #region Dictionaries
        /// <summary>Key is ID</summary>
        public static Dictionary<string, PlainKeyword> LoadedKeywords_Bufs { get; set; } = [];

        /// <summary>Key is ID</summary>
        public static Dictionary<string, PlainKeyword> LoadedKeywords_BattleKeywords { get; set; } = [];


        /// <summary>
        /// Key is Name, value is ID. Used by "Implicit to ..." options in <see cref="MainWindow.TextEditor_ContextMenuClick"/> method, contains keyword names from both fallback and loaded custom lang.
        /// </summary>
        public static Dictionary<string, string> LimbusKeywords_ImplicitConversionOrder { get; set; } = [];


        /// <summary>Key is ID</summary>
        public static Dictionary<string, PlainSkillTag> LoadedSkillTags { get; set; } = [];
        #endregion




        public static void LoadKeywordsFrom(string LocalizationPath, LocalizationDirectoryType LoadingType)
        {
            string LoadingTarget = LoadingType is LocalizationDirectoryType.Fallback
                ? @Languages.VariableData.ReadedStartupSteps.SubStages.LimbusFallbackKeywords
                : LoadingType is LocalizationDirectoryType.Additional
                    ? @Languages.VariableData.ReadedStartupSteps.SubStages.LimbusAdditionalKeywords
                    : @Languages.VariableData.ReadedStartupSteps.SubStages.LimbusCustomLangKeywords;

            SplashScreenWindow.ProgressSubObject = LoadingTarget;

            if (LoadingType is LocalizationDirectoryType.Fallback)
            {
                LoadedSkillTags.Clear();
                LoadedKeywords_Bufs.Clear();
                LoadedKeywords_BattleKeywords.Clear();

                ColorDictionaries.LoadedKeywordColors.ExternalColors.Clear();
                ColorDictionaries.LoadedSkillTagColors.ExternalColors.Clear();

                LimbusKeywords_ImplicitConversionOrder.Clear();
            }


            if (Directory.Exists(LocalizationPath))
            {
                try
                {
                    DirectoryInfo LocalizationFilesDirectory = new(path: LocalizationPath);

                    if (LoadingType is not LocalizationDirectoryType.Additional)
                    {
                        MainWindowInstance.RichTextViews__Skills_COMPOSITION_SkillNameReplica.AttackWeightLabel
                            = LocalizationFilesDirectory.TryAcquireString("*MainUIText_UPDATE_ON_0720.json", "mainui_target_num_label", Fallback: "Atk Weight");
                    
                        MainWindowInstance.RightMenuUIElements__Skills_SkillUptieLevelSign.RichText
                            = LocalizationFilesDirectory.TryAcquireString("*Filter.json", "filter_awaken_state", Fallback: "Uptie Tier");
                    
                        MainWindowInstance.RichTextViews__EGOGifts_COMPOSITION_ViewDescSign.RichText
                            = LocalizationFilesDirectory.TryAcquireString("*MirrorDungeonUI_3.json", "mirror_dungoen_ego_gift_history_view_desc", Fallback: "View Desc.");

                    
                        MainWindowInstance.RichTextViews__ObservationLogs_COMPOSITION_LackingDataSign.RichText
                            = LocalizationFilesDirectory.TryAcquireString("*BattleUIText.json", "abnormality_no_research_level", Fallback: "Lacking Data");

                        string ObservationLevelMainSign
                            = LocalizationFilesDirectory.TryAcquireString("*BattleUIText.json", "abnormality_research_level", Fallback: "Obs. Level");
                    
                        for (int ObservationLevel = 1; ObservationLevel <= 3; ObservationLevel++)
                        {
                            MainWindowInstance.FindTypeName<TMProEmitter>($"RichTextViews__ObservationLogs_COMPOSITION_ObsLevelSign_{ObservationLevel}")!.RichText
                                = $"{ObservationLevelMainSign} " + ObservationLevel switch { 1 => "I", 2 => "II", 3 => "III" };
                        }
                    }

                    FileInfo[] SkillTagFileSearcher = LocalizationFilesDirectory.GetFiles("*SkillTag.json", SearchOption.AllDirectories);
                    if (SkillTagFileSearcher.Length > 0)
                    {
                        if (SkillTagFileSearcher[0].TryDeserealizeJsonAs(out LimbusLocalizationFile<PlainSkillTag> Deserialized, out Exception Occurred))
                        {
                            List<PlainSkillTag> Targets = [.. Deserialized.DataList.Where(SkillTag => !string.IsNullOrWhiteSpace(SkillTag.ID))];
                            foreach (PlainSkillTag SkillTag in Targets)
                            {
                                LoadedSkillTags[SkillTag.ID!] = SkillTag;

                                if (!string.IsNullOrEmpty(SkillTag.Color))
                                {
                                    ColorDictionaries.LoadedSkillTagColors.ExternalColors[SkillTag.ID!] = SkillTag.Color;
                                }
                            }
                        }
                        else
                        {
                            ErrorMessageWindow.ShowException(Occurred, $"This exception occured at the moment of <u>{LoadingType}</u> keywords loading (SkillTag.json reading (<b>\"{SkillTagFileSearcher[0].Name}\"</b> file))");
                        }
                    }


                    LocalizationFilesDirectory.DoKeywordsInspection(
                        FilesWildcardPattern: "*Bufs*.json",
                        RelatedDictionary: LoadedKeywords_Bufs,
                        SubActors:
                        [
                            DirectoryInspection_FoundExternalColorsSetter,
                            delegate (PlainKeyword Keyword)
                            {
                                if (!string.IsNullOrWhiteSpace(Keyword.Name))
                                {
                                    if (Keyword.ID!.MatchesOneOf(LoadedConfiguration.PreviewSettings.CustomLang.ImplicitKeywordsConversionIgnores) == false)
                                    {
                                        if (LimbusKeywords_ImplicitConversionOrder.ContainsKey(Keyword.Name) == false)
                                        {
                                            LimbusKeywords_ImplicitConversionOrder[Keyword.Name] = Keyword.ID!;
                                        }
                                    }
                                }
                            }
                        ]
                    );

                    LocalizationFilesDirectory.DoKeywordsInspection(
                        FilesWildcardPattern: "*BattleKeywords*.json",
                        RelatedDictionary: LoadedKeywords_BattleKeywords,
                        SubActors: DirectoryInspection_FoundExternalColorsSetter
                    );

                    LimbusKeywords_ImplicitConversionOrder = ReorderDictionaryByStringKeysLength(LimbusKeywords_ImplicitConversionOrder);
                }
                catch (Exception Occurred)
                {
                    ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to load files from <u>{LoadingType}</u> keywords directory (Kinda fatal)");
                }
            }
            else if (!string.IsNullOrWhiteSpace(LocalizationPath))
            {
                ErrorMessageWindow.ShowException(new DirectoryNotFoundException("Directory not found"), $"Could not find keywords directory \"<b>{LocalizationPath}</b>\" ({LoadingTarget})");
            }
        }




        #region Local utils
        private static void DirectoryInspection_FoundExternalColorsSetter(PlainKeyword Keyword)
        {
            if (!string.IsNullOrEmpty(Keyword.Color))
            {
                ColorDictionaries.LoadedKeywordColors.ExternalColors[Keyword.ID!] = Keyword.Color;
            }
        }

        private static void DoKeywordsInspection(
            this DirectoryInfo LocalizationFilesDirectory,
            string FilesWildcardPattern,
            Dictionary<string, PlainKeyword> RelatedDictionary,
            params Action<PlainKeyword>[] SubActors
        ) {
            /*/
             * `.OrderBy(KeywordsFile => KeywordsFile.Name.Length)` creates approximately neat order of files
             * [Bufs.json, ..., Bufs-a1c5p1.json, Bufs-a1c5p2.json, Bufs-a1c5p3.json, ..., Bufs-a1c9p3.json, ...]
            /*/
            try
            {
                List<FileInfo> Targets = [.. LocalizationFilesDirectory
                    .GetFiles(FilesWildcardPattern, SearchOption.AllDirectories)
                    .OrderBy(KeywordsFile => KeywordsFile.Name.Length)];

                foreach (FileInfo LocalizationFile in Targets)
                {
                    try
                    {
                        if (LocalizationFile.TryDeserealizeJsonAs(out LimbusLocalizationFile<PlainKeyword> Deserialized, out Exception Occurred))
                        {
                            foreach (PlainKeyword Keyword in Deserialized.DataList.Where(x => !string.IsNullOrWhiteSpace(x.ID)))
                            {
                                RelatedDictionary[Keyword.ID!] = Keyword;

                                foreach (Action<PlainKeyword> Actor in SubActors) Actor?.Invoke(Keyword);
                            }
                        }
                        else
                        {
                            ErrorMessageWindow.ShowException(Occurred, $"This exception occured when trying to load keywords from file <b>\"{LocalizationFile.Name}\"</b> in directory <b>\"{LocalizationFilesDirectory.FullName}\"</b> using pattern \"{FilesWildcardPattern}\"");
                        }
                    }
                    catch (Exception Occurred)
                    {
                        ErrorMessageWindow.ShowException(Occurred, $"This exception occured when loading keywords from the <b>\"{LocalizationFilesDirectory.FullName}\"</b> directory using the \"{FilesWildcardPattern}\" files pattern, guilty file: </b>\"{LocalizationFile.Name}\"</b>");
                    }
                }
            }
            catch (Exception Occurred)
            {
                ErrorMessageWindow.ShowException(Occurred, $"This exception occured when trying to load keywords from the directory <b>\"{LocalizationFilesDirectory.FullName}\"</b> using the pattern \"{FilesWildcardPattern}\"");
            }
        }

        private static string TryAcquireString<IDType>(this DirectoryInfo Location, string FileName, IDType ID, string Fallback) where IDType : notnull
        {
            FileInfo[] FoundFiles = Location.GetFiles(FileName, SearchOption.AllDirectories);
            if (FoundFiles.Length > 0)
            {
                if (FoundFiles[0].TryDeserealizeJsonAs(out LimbusLocalizationFile<dynamic> Deserialized, out Exception Occurred))
                {
                    Dictionary<IDType, string> LocalizationDataDictionary = [];
                    foreach (dynamic LocalizationObject in Deserialized.DataList)
                    {
                        if ((string)LocalizationObject.content is not null)
                        {
                            LocalizationDataDictionary[(IDType)LocalizationObject.id] = (string)LocalizationObject.content;
                        }
                    }

                    return LocalizationDataDictionary.TryGetValue(ID, out string? FoundString) ? FoundString : Fallback;
                }
                else
                {
                    ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to read additional localization file <b>\"{FoundFiles[0].Name}\"</b> from <b>\"{Location.FullName}\"</b> directory for UI text by <u>{ID}</u> ID");
                    return Fallback;
                }
            }
            else
            {
                return Fallback;
            }
        }
        #endregion
    }
}