using LC_Localization_Task_Absolute.Json;
using RichText;
using System.IO;
using System.Windows;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Passives;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static System.Windows.Visibility;


namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    internal abstract class Mode_Passives
    {
        internal protected static dynamic FormalTaskCompleted = null;

        internal protected static int CurrentPassiveID = -1;

        internal protected static Passives DeserializedInfo;
        internal protected static Dictionary<string, int> Passives_NameIDs = [];

        internal protected static string TargetSite_StringLine = "Main Description";

        internal protected static SwitchedInterfaceProperties SwitchedInterfaceProperties = new()
        {
            Key = "Passives",
            DefaultValues = new()
            {
                Height = 550,
                Width = 1000,
                MinHeight = 420,
                MinWidth = 708.8,
                MaxHeight = 10000,
                MaxWidth = 1000,
            },
        };

        internal protected static void TriggerSwitch()
        {
            MainControl.NavigationPanel_Skills_UptieLevelSelectorGrid.Visibility = Visibility.Collapsed;
            MainControl.NavigationPanel_Skills_EGOAbnormalityName.Visibility = Visibility.Collapsed;
            MainControl.NavigationPanel_SwitchButtons.Margin = new Thickness(2, 114, 4, 4);

            MainControl.PreviewLayouts.Height = 383;
            MainControl.NavigationPanel_HeightControlScrollViewer.MaxHeight = 370;
            MainControl.EditorWidthControl.Width = new GridLength(706.6);

            PreviewUpdate_TargetSite = MainControl.PreviewLayout_Passives;

            Upstairs.ActiveProperties = SwitchedInterfaceProperties;

            AdjustUI(ActiveProperties.DefaultValues);

            HideNavigationPanelButtons(
                  ExceptButtonsGrid: MainControl.SwitchButtons_Passives,
                ExceptPreviewLayout: MainControl.PreviewLayoutGrid_Passives
            );
        }

        internal protected static Task LoadStructure(FileInfo JsonFile)
        {
            DeserializedInfo = JsonFile.Deserealize<Passives>();
            InitializePassivesDelegateFrom(DeserializedInfo);

            if (DelegatePassives_IDList.Count > 0)
            {
                Mode_Handlers.Mode_Passives.TriggerSwitch();
                TransformToPassive(DelegatePassives_IDList[0]);
            }

            return FormalTaskCompleted;
        }

        internal protected static Task TransformToPassive(int PassiveID)
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentPassiveID = PassiveID;

            if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Current ID Copy Button"))
            {
                MainControl.STE_NavigationPanel_ObjectID_Display
                    .SetRichText(UILanguageLoader.DynamicTypeElements["Right Menu — Current ID Copy Button"]
                    .Extern(CurrentPassiveID));
            }

            MainWindow.NavigationPanel_IDSwitch_CheckAvalibles();

            MainControl.NavigationPanel_ObjectName_Display.Text = DelegatePassives[CurrentPassiveID].Name;
            MainControl.SWBT_Passives_MainPassiveName.Text = DelegatePassives[CurrentPassiveID].Name.Replace("\n", "\\n");

            ReCheckPassiveInfo();

            SwitchToMainDesc();
            
            {
                ManualTextLoadEvent = false;
            }

            return FormalTaskCompleted;
        }

        internal protected static void ReCheckPassiveInfo()
        {
            MainControl.STE_DisableCover_Passives_SummaryDescription.Visibility = Visible;

            /////////////////////////////////////////////////
            var FullLink = DelegatePassives[CurrentPassiveID];
            /////////////////////////////////////////////////
            
            if (FullLink.SummaryDescription != null)
            {
                if (!FullLink.SummaryDescription.Equals(FullLink.EditorSummaryDescription))
                {
                    MainControl.STE_Passives_SummaryDescription
                        .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                        .Extern(UILanguageLoader.UILanguageElementsTextData["Right Menu — Passive Summary"]));
                }
                else
                {
                    MainControl.STE_Passives_SummaryDescription
                        .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Passive Summary"]);
                }
                MainControl.STE_DisableCover_Passives_SummaryDescription.Visibility = Collapsed;
            }
            else
            {
                MainControl.STE_Passives_SummaryDescription
                    .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Passive Summary"]);
            }

            SwitchToMainDesc();
        }

        internal protected static void SwitchToMainDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            TargetSite_StringLine = "Main Description";

            /////////////////////////////////////////////////
            var FullLink = DelegatePassives[CurrentPassiveID];
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

        internal protected static void SwitchToSummaryDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            TargetSite_StringLine = "Summary Description";

            /////////////////////////////////////////////////
            var FullLink = DelegatePassives[CurrentPassiveID];
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
