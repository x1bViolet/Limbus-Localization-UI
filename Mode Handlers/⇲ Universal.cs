using LC_Localization_Task_Absolute.Json;
using System.IO;
using System.Windows;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_ContentBasedUniversal_UNUSEDPROBABLYUSELESS;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;


namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    // idk
    public abstract class Mode_ContentUniversal
    {
        public static dynamic FormalTaskCompleted = null;

        public static dynamic CurrentItemID = null;

        public static ContentBasedUniversal DeserializedInfo;

        public static SwitchedInterfaceProperties SwitchedInterfaceProperties = new SwitchedInterfaceProperties()
        {
            Key = "Universal",
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

            PreviewUpdate_TargetSite = MainControl.PreviewLayout_Passives;//////////////

            Upstairs.ActiveProperties = SwitchedInterfaceProperties;

            AdjustUI(ActiveProperties.DefaultValues);

            HideNavigationPanelButtons(
                  ExceptButtonsGrid: MainControl.SwitchButtons_Passives,///////////////
                ExceptPreviewLayout: MainControl.PreviewLayoutGrid_Passives////////////
            );
        }

        public static bool IsMatchesStructure(object Class)
        {


            return false;
        }

        public static Task LoadStructure(FileInfo JsonFile)
        {
            DeserializedInfo = JsonFile.Deserealize<ContentBasedUniversal>();

            if (DeserializedInfo != null && DeserializedInfo.dataList != null && DeserializedInfo.dataList.Count > 0)
            {
                InitializeContentBasedUniversalDelegateFrom(DeserializedInfo);
                Mode_Handlers.Mode_ContentUniversal.TriggerSwitch();
            }

            return FormalTaskCompleted;
        }
    }
}
