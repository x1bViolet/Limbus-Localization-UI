using LC_Localization_Task_Absolute.Json;
using System.IO;
using System.Windows;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Passives;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static System.Windows.Visibility;


namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    public abstract class Mode_Passives
    {
        public static dynamic FormalTaskCompleted = null;

        public static int CurrentPassiveID = -1;

        public static Passives DeserializedInfo;
        public static Dictionary<string, int> Passives_NameIDs = [];

        public static string TargetSite_StringLine = "Main Description";

        public static SwitchedInterfaceProperties SwitchedInterfaceProperties = new SwitchedInterfaceProperties()
        {
            Key = "Passives",
            DefaultValues = new DefaultValues()
            {
                Height = 550,
                Width = 1000,
                MinHeight = 420,
                MinWidth = 708.8,
                MaxHeight = 10000,
                MaxWidth = 1000,
            },
        };

        public static void TriggerSwitch()
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

        public static Task LoadStructure(FileInfo JsonFile)
        {
            DeserializedInfo = JsonFile.Deserealize<Passives>();

            if (DeserializedInfo != null && DeserializedInfo.dataList != null && DeserializedInfo.dataList.Count > 0)
            {
                InitializePassivesDelegateFrom(DeserializedInfo);
                Mode_Handlers.Mode_Passives.TriggerSwitch();
                TransformToPassive(DelegatePassives_IDList[0]);
            }

            return FormalTaskCompleted;
        }

        public static Task TransformToPassive(int PassiveID)
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentPassiveID = PassiveID;

            MainControl.STE_NavigationPanel_ObjectID_Display
                .RichText = ᐁ_Interface_Localization_Loader.ExternTextFor("[Main UI] * ID Copy Button")
                .Extern(CurrentPassiveID);
            
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

        public static void ReCheckPassiveInfo()
        {
            MainControl.STE_DisableCover_Passives_SummaryDescription.Visibility = Visible;

            /////////////////////////////////////////////////
            Passive FullLink = DelegatePassives[CurrentPassiveID];
            /////////////////////////////////////////////////
            
            if (FullLink.SummaryDescription != null)
            {
                if (!FullLink.SummaryDescription.Equals(FullLink.EditorSummaryDescription))
                {
                    ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"]
                        .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker
                            .Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers["[Passives / Right menu] * Passive summary"].Text);
                }
                else
                {
                    ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"]
                        .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Passives / Right menu] * Passive summary"].Text;
                }
                MainControl.STE_DisableCover_Passives_SummaryDescription.Visibility = Collapsed;
            }
            else
            {
                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"]
                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Passives / Right menu] * Passive summary"].Text;
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
            Passive FullLink = DelegatePassives[CurrentPassiveID];
            /////////////////////////////////////////////////

            if (!FullLink.Description.Equals(FullLink.EditorDescription))
            {
                MainControl.TextEditor.Text = FullLink.EditorDescription;
            }
            else
            {
                MainControl.TextEditor.Text = FullLink.Description;
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
            Passive FullLink = DelegatePassives[CurrentPassiveID];
            /////////////////////////////////////////////////

            if (!FullLink.SummaryDescription.Equals(FullLink.EditorSummaryDescription))
            {
                MainControl.TextEditor.Text = FullLink.EditorSummaryDescription;
            }
            else
            {
                MainControl.TextEditor.Text = FullLink.SummaryDescription;
            }

            LockEditorUndo();

            {
                ManualTextLoadEvent = true;
            }
        }
    }
}
