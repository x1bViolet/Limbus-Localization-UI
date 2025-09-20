using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using System.IO;
using System.Windows;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static System.Windows.Visibility;


namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    public abstract class Mode_Keywords
    {
        public static dynamic FormalTaskCompleted = null;

        public static string CurrentKeywordID = "";

        public static Keywords DeserializedInfo;
        public static Dictionary<string, string> Keywords_NameIDs = [];

        public static string TargetSite_StringLine = "Main Description";

        public static SwitchedInterfaceProperties SwitchedInterfaceProperties = new()
        {
            Key = "Keywords",
            DefaultValues = new()
            {
                Height = 550,
                Width = 1000,
                MinHeight = 464,
                MinWidth = 709.8,
                MaxHeight = 10000,
                MaxWidth = 1000,
            },
        };

        public static void TriggerSwitch(bool IsBufsMenu = false)
        {
            if (IsBufsMenu)
            {
                MainControl.PreviewLayoutGrid_Keywords_Sub_Bufs.Visibility = Visibility.Visible;
                MainControl.PreviewLayoutGrid_Keywords_Sub_BattleKeywords.Visibility = Visibility.Collapsed;
                SwitchedInterfaceProperties.DefaultValues.MinHeight = 460;
                SwitchedInterfaceProperties.DefaultValues.Height = 645;
                MainControl.PreviewLayouts.Height = 424;

                PreviewUpdate_TargetSite = MainControl.Special_PreviewLayout_Keywords_Bufs_Desc;
            }
            else
            {
                MainControl.PreviewLayoutGrid_Keywords_Sub_Bufs.Visibility = Visibility.Collapsed;
                MainControl.PreviewLayoutGrid_Keywords_Sub_BattleKeywords.Visibility = Visibility.Visible;
                SwitchedInterfaceProperties.DefaultValues.MinHeight = 460;
                SwitchedInterfaceProperties.DefaultValues.Height = 645;
                MainControl.PreviewLayouts.Height = 424;

                PreviewUpdate_TargetSite = MainControl.Special_PreviewLayout_Keywords_BattleKeywords_Desc;
            }

            MainControl.NavigationPanel_HeightControlScrollViewer.MaxHeight = 391;
            MainControl.EditorWidthControl.Width = new GridLength(706.6);

            MainControl.NavigationPanel_Skills_UptieLevelSelectorGrid.Visibility = Visibility.Collapsed;
            MainControl.NavigationPanel_Skills_EGOAbnormalityName.Visibility = Visibility.Collapsed;
            MainControl.NavigationPanel_SwitchButtons.Margin = new Thickness(2, 114, 4, 4);


            Upstairs.ActiveProperties = SwitchedInterfaceProperties;

            AdjustUI(ActiveProperties.DefaultValues);

            HideNavigationPanelButtons(
                  ExceptButtonsGrid: MainControl.SwitchButtons_Keywords,
                ExceptPreviewLayout: MainControl.PreviewLayoutGrid_Keywords
            );
        }

        public static Task LoadStructure(FileInfo JsonFile)
        {
            DeserializedInfo = JsonFile.Deserealize<Keywords>();
            InitializeKeywordsDelegateFrom(DeserializedInfo);

            if (DelegateKeywords_IDList.Count > 0)
            {
                Mode_Handlers.Mode_Keywords.TriggerSwitch(IsBufsMenu: JsonFile.Name.RemovePrefix(["JP_", "KR_", "EN_"]).StartsWith("Bufs"));
                TransformToKeyword(DelegateKeywords_IDList[0]);
            }

            return FormalTaskCompleted;
        }

        public static Task TransformToKeyword(string KeywordID)
        {
            {
                ManualTextLoadEvent = true;
            }

            if (KeywordsInterrogate.KeywordImages.ContainsKey(KeywordID))
            {
                MainControl.CurrentBufsKeywordImage.Source = KeywordsInterrogate.KeywordImages[KeywordID];
                MainControl.CurrentBattleKeywordsKeywordImage.Source = KeywordsInterrogate.KeywordImages[KeywordID];
            }
            else
            {
                MainControl.CurrentBufsKeywordImage.Source = KeywordsInterrogate.KeywordImages["Unknown"];
                MainControl.CurrentBattleKeywordsKeywordImage.Source = KeywordsInterrogate.KeywordImages["Unknown"];
            }

            CurrentKeywordID = KeywordID;

            MainControl.STE_NavigationPanel_ObjectID_Display
                .RichText = ᐁ_Interface_Localization_Loader.ExternTextFor("[Main UI] * ID Copy Button")
                .Extern(CurrentKeywordID);
            
            MainWindow.NavigationPanel_IDSwitch_CheckAvalibles();

            MainControl.NavigationPanel_ObjectName_Display.Text = DelegateKeywords[CurrentKeywordID].Name;
            MainControl.SWBT_Keywords_KeywordName.Text = DelegateKeywords[CurrentKeywordID].Name.Replace("\n", "\\n");
            MainControl.PreviewLayout_Keywords_Bufs_Name.Text = DelegateKeywords[CurrentKeywordID].Name;
            MainControl.PreviewLayout_Keywords_BattleKeywords_Name.Text = DelegateKeywords[CurrentKeywordID].Name;

            ReCheckKeywordInfo();

            SwitchToMainDesc();
            
            {
                ManualTextLoadEvent = false;
            }

            return FormalTaskCompleted;
        }

        public static void ReCheckKeywordInfo()
        {
            MainControl.STE_DisableCover_Keyword_SummaryDescription.Visibility = Visible;

            /////////////////////////////////////////////////
            var FullLink = DelegateKeywords[CurrentKeywordID];
            /////////////////////////////////////////////////
            
            if (FullLink.SummaryDescription != null)
            {
                if (!FullLink.SummaryDescription.Equals(FullLink.EditorSummaryDescription))
                {
                    ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"]
                        .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker
                            .Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword summary"].Text);

                    //MainControl.STE_Keyword_SummaryDescription
                    //    .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                    //    .Extern(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Summary"]));
                }
                else
                {
                    ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"]
                        .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword summary"].Text;

                    //MainControl.STE_Keyword_SummaryDescription
                    //    .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Summary"]);
                }
                MainControl.STE_DisableCover_Keyword_SummaryDescription.Visibility = Collapsed;
            }
            else
            {
                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"]
                        .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword summary"].Text;

                //MainControl.STE_Keyword_SummaryDescription
                //    .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Summary"]);
            }

            SwitchToMainDesc();
        }

        public static void SwitchToMainDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            TargetSite_StringLine = "Main Description";

            /////////////////////////////////////////////////
            var FullLink = DelegateKeywords[CurrentKeywordID];
            /////////////////////////////////////////////////

            if (!FullLink.Description.Equals(FullLink.EditorDescription))
            {
                MainControl.Editor.Text = FullLink.EditorDescription;
            }
            else
            {
                MainControl.Editor.Text = FullLink.Description;
            }

            LockEditorUndo();

            {
                ManualTextLoadEvent = true;
            }
        }

        public static void SwitchToSummaryDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            TargetSite_StringLine = "Summary Description";

            /////////////////////////////////////////////////
            var FullLink = DelegateKeywords[CurrentKeywordID];
            /////////////////////////////////////////////////

            if (!FullLink.SummaryDescription.Equals(FullLink.EditorSummaryDescription))
            {
                MainControl.Editor.Text = FullLink.EditorSummaryDescription;
            }
            else
            {
                MainControl.Editor.Text = FullLink.SummaryDescription;
            }

            LockEditorUndo();

            {
                ManualTextLoadEvent = true;
            }
        }
    }
}
