using LC_Localization_Task_Absolute.Json;
using System.IO;
using System.Windows;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_ContentBasedUniversal;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;


namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    // idk
    internal abstract class Mode_ContentUniversal
    {
        internal protected static dynamic FormalTaskCompleted = null;

        internal protected static dynamic CurrentItemID = null;

        internal protected static ContentBasedUniversal DeserializedInfo;

        internal protected static SwitchedInterfaceProperties SwitchedInterfaceProperties = new()
        {
            Key = "Universal",
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

            PreviewUpdate_TargetSite = MainControl.PreviewLayout_Passives;//////////////

            Upstairs.ActiveProperties = SwitchedInterfaceProperties;

            AdjustUI(ActiveProperties.DefaultValues);

            HideNavigationPanelButtons(
                  ExceptButtonsGrid: MainControl.SwitchButtons_Passives,///////////////
                ExceptPreviewLayout: MainControl.PreviewLayoutGrid_Passives////////////
            );
        }

        internal protected static bool IsMatchesStructure(object Class)
        {


            return false;
        }

        internal protected static Task LoadStructure(FileInfo JsonFile)
        {
            DeserializedInfo = JsonFile.Deserealize<ContentBasedUniversal>();
            InitializeContentBasedUniversalDelegateFrom(DeserializedInfo);

            if (DelegatePassives_IDList.Count > 0)
            {
                Mode_Handlers.Mode_ContentUniversal.TriggerSwitch();
            }

            return FormalTaskCompleted;
        }
    }
}
