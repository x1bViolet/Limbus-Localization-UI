using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using System.IO;
using System.Windows;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;
using static System.Windows.Visibility;


namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    public interface Mode_Keywords
    {
        public static string CurrentKeywordID = "";

        public static KeywordsFile DeserializedInfo;

        public static TripleDescriptionType CurrentDescriptionType = TripleDescriptionType.Main;

        public static SwitchedInterfaceProperties SwitchedInterfaceProperties = new SwitchedInterfaceProperties()
        {
            Key = EditorMode.Keywords,
            WindowSizesInfo = new WindowSizesConfig()
            {
                Height = 550,
                Width = 1000,
                MinHeight = 464,
                MinWidth = 709.8,
                MaxWidth = 1000,
            },
        };

        public ref struct @Current
        {
            public static Keyword Keyword => DelegateKeywords[CurrentKeywordID];
        }

        public static void TriggerSwitch(bool IsBufsMenu = false)
        {
            if (IsBufsMenu)
            {
                MainControl.PreviewLayoutGrid_Keywords_Sub_Bufs.Visibility = Visible;
                MainControl.PreviewLayoutGrid_Keywords_Sub_BattleKeywords.Visibility = Collapsed;
                SwitchedInterfaceProperties.WindowSizesInfo.MinHeight = 460;
                SwitchedInterfaceProperties.WindowSizesInfo.Height = 645;
                MainControl.PreviewLayouts.Height = 424;

                TargetPreviewLayout = MainControl.PreviewLayout_Keywords_Bufs_Desc;
            }
            else
            {
                MainControl.PreviewLayoutGrid_Keywords_Sub_Bufs.Visibility = Collapsed;
                MainControl.PreviewLayoutGrid_Keywords_Sub_BattleKeywords.Visibility = Visible;
                SwitchedInterfaceProperties.WindowSizesInfo.MinHeight = 460;
                SwitchedInterfaceProperties.WindowSizesInfo.Height = 645;
                MainControl.PreviewLayouts.Height = 424;

                TargetPreviewLayout = MainControl.Special_PreviewLayout_Keywords_BattleKeywords_Desc;
            }

            MainControl.EditorWidthControl.Width = new GridLength(706.6);

            MainControl.UptieLevelSelectionButtons.Visibility = Collapsed;
            MainControl.NavigationPanel_Skills_EGOAbnormalityName.Visibility = Collapsed;
            MainControl.NavigationPanel_SwitchButtons.Margin = new Thickness(2, 111, 4, 4);


            ActiveProperties = SwitchedInterfaceProperties;

            AdjustUI(ActiveProperties.WindowSizesInfo);

            HideNavigationPanelButtons(
                  ExceptButtonsPanel: MainControl.SwitchButtons_Keywords,
                ExceptPreviewLayout: MainControl.PreviewLayoutGrid_Keywords
            );
        }

        public static void ValidateAndLoadStructure(FileInfo JsonFile, string KeywordsType)
        {
            var TemplateDeserialized = JsonFile.Deserealize<KeywordsFile>();

            if (TemplateDeserialized != null && TemplateDeserialized.dataList != null && TemplateDeserialized.dataList.Count > 0)
            {
                if (TemplateDeserialized.dataList.Any(Keyword => !Keyword.ID.Trim().EqualsOneOf("NOTHING THERE \0 \0", "")))
                {
                    Mode_Keywords.DeserializedInfo = JsonFile.Deserealize<KeywordsFile>();

                    MainWindow.FocusOnFile(JsonFile);

                    InitializeKeywordsDelegateFromDeserialized();
                    Mode_Keywords.TriggerSwitch(IsBufsMenu: KeywordsType.StartsWith("Bufs"));

                    TransformToKeyword(DelegateKeywords_IDList[0]);
                }
            }
        }

        public static void TransformToKeyword(string KeywordID)
        {
            {
                ManualTextLoadEvent = true;
            }

            if (KeywordsInterrogation.KeywordImages.ContainsKey(KeywordID))
            {
                MainControl.CurrentBufsKeywordImage.Source = KeywordsInterrogation.KeywordImages[KeywordID];
                MainControl.CurrentBattleKeywordsKeywordImage.Source = KeywordsInterrogation.KeywordImages[KeywordID];
            }
            else
            {
                MainControl.CurrentBufsKeywordImage.Source = KeywordsInterrogation.KeywordImages["Unknown"];
                MainControl.CurrentBattleKeywordsKeywordImage.Source = KeywordsInterrogation.KeywordImages["Unknown"];
            }

            CurrentKeywordID = KeywordID;

            MainControl.STE_NavigationPanel_ObjectID_Display
                .RichText = GetLocalizationTextFor("[Main UI] * ID Copy Button")
                .Extern(CurrentKeywordID);
            
            MainWindow.NavigationPanel_IDSwitch_CheckAvalibles();

            MainControl.NavigationPanel_ObjectName_Display.Text = @Current.Keyword.Name;
            MainControl.SWBT_Keywords_KeywordName.Text = @Current.Keyword.Name.Replace("\n", "\\n");
            MainControl.SWBT_Keywords_KeywordColor.Text = @Current.Keyword.Color;
            MainControl.PreviewLayout_Keywords_Bufs_Name.RichText = @Current.Keyword.Name;
            MainControl.PreviewLayout_Keywords_BattleKeywords_Name.RichText = @Current.Keyword.Name;

            ReCheckKeywordSummaryAndFlavorButtons();

            SwitchToMainDesc();
            
            {
                ManualTextLoadEvent = false;
            }
        }

        public static void ReCheckKeywordSummaryAndFlavorButtons()
        {
            MainControl.KeywordSummarySwitchButton.IsEnabled = false;
            MainControl.KeywordFlavorSwitchButton.IsEnabled = false;


            if (@Current.Keyword.PresentSummaryDescription != null)
            {
                if (@Current.Keyword.PresentSummaryDescription != @Current.Keyword.EditorSummaryDescription)
                {
                    PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"].MarkWithUnsaved();
                }
                else
                {
                    PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"].SetDefaultText();
                }
                MainControl.KeywordSummarySwitchButton.IsEnabled = true;
            }
            else
            {
                PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"].SetDefaultText();
            }


            if (@Current.Keyword.PresentFlavorDescription != null)
            {
                if (@Current.Keyword.PresentFlavorDescription != @Current.Keyword.EditorFlavorDescription)
                {
                    PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword flavor"].MarkWithUnsaved();
                }
                else
                {
                    PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword flavor"].SetDefaultText();
                }
                MainControl.KeywordFlavorSwitchButton.IsEnabled = true;
            }
            else
            {
                PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword flavor"].SetDefaultText();
            }
        }

        public static void SwitchToMainDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentDescriptionType = TripleDescriptionType.Main;

            MainControl.TextEditor.Document = @Current.Keyword.DedicatedDocument_MainDesc;

            {
                ManualTextLoadEvent = true;
            }
        }

        public static void SwitchToSummaryDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentDescriptionType = TripleDescriptionType.Summary;

            MainControl.TextEditor.Document = @Current.Keyword.DedicatedDocument_SummaryDesc;

            {
                ManualTextLoadEvent = true;
            }
        }

        public static void SwitchToFlavorDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentDescriptionType = TripleDescriptionType.Flavor;

            MainControl.TextEditor.Document = @Current.Keyword.DedicatedDocument_FlavorDesc;

            {
                ManualTextLoadEvent = true;
            }
        }
    }
}

// UI interactions
namespace LC_Localization_Task_Absolute
{
    public partial class MainWindow
    {
        private void Keywords_SwitchToMainDesc(object RequestSender, RoutedEventArgs EventArgs) => Mode_Keywords.SwitchToMainDesc();
        private void Keywords_SwitchToSummaryDesc(object RequestSender, RoutedEventArgs EventArgs) => Mode_Keywords.SwitchToSummaryDesc();
        private void Keywords_SwitchToFlavorDesc(object RequestSender, RoutedEventArgs EventArgs) => Mode_Keywords.SwitchToFlavorDesc();
        private void Keywords_ToggleFormatInsertions(object RequestSender, RoutedEventArgs EventArgs)
        {
            switch (Keywords_FormatInsertions_StackPanel.Visibility)
            {
                case Visible:
                    Keywords_FormatInsertions_StackPanel.Visibility = Collapsed;
                    Keyword_DropdownFormatInsertions_IndicatorAngle.Angle = 0;
                    break;

                case Collapsed:
                    Keywords_FormatInsertions_StackPanel.Visibility = Visible;
                    Keyword_DropdownFormatInsertions_IndicatorAngle.Angle = 180;
                    break;
            }
        }

        private void Keywords_ChangeKeywordColor(object RequestSender, RoutedEventArgs EventArgs)
        {
            Mode_Keywords.@Current.Keyword.Color = SWBT_Keywords_KeywordColor.Text.Trim() == "" ? null : SWBT_Keywords_KeywordColor.Text;
            Mode_Keywords.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
        }

        private void Keywords_InteractiveBufsOKButton_Click(object RequestSender, RoutedEventArgs EventArgs)
        {
            bool AnyChanges = false;
            if (Mode_Keywords.@Current.Keyword.PresentMainDescription != Mode_Keywords.@Current.Keyword.EditorMainDescription)
            {
                Mode_Keywords.@Current.Keyword.PresentMainDescription = Mode_Keywords.@Current.Keyword.EditorMainDescription;

                PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword desc"].SetDefaultText();

                AnyChanges = true;
            }

            if (Mode_Keywords.@Current.Keyword.PresentSummaryDescription != null)
            {
                if (Mode_Keywords.@Current.Keyword.PresentSummaryDescription != Mode_Keywords.@Current.Keyword.EditorSummaryDescription)
                {
                    Mode_Keywords.@Current.Keyword.PresentSummaryDescription = Mode_Keywords.@Current.Keyword.EditorSummaryDescription;

                    PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"].SetDefaultText();

                    AnyChanges = true;
                }
            }

            if (SWBT_Keywords_KeywordName.Text != Mode_Keywords.@Current.Keyword.Name)
            {
                Mode_Keywords.@Current.Keyword.Name = SWBT_Keywords_KeywordName.Text.Trim();
                NavigationPanel_ObjectName_Display.Text = Mode_Keywords.@Current.Keyword.Name;

                AnyChanges = true;
            }

            if (AnyChanges) Mode_Keywords.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
        }
    }
}