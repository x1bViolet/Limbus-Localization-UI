using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Mode_Handlers;
using System.IO;
using System.Windows;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Passives;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static System.Windows.Visibility;


namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    public interface Mode_Passives
    {
        public static int CurrentPassiveID = -1;

        public static PassivesFile DeserializedInfo;
        public static DualDescriptionType CurrentDescriptionType = DualDescriptionType.Main;

        public static SwitchedInterfaceProperties SwitchedInterfaceProperties = new SwitchedInterfaceProperties()
        {
            Key = EditorMode.Passives,
            WindowSizesInfo = new WindowSizesConfig()
            {
                Height = 550,
                Width = 1000,
                MinHeight = 420,
                MinWidth = 708.8,
                MaxWidth = 1000,
            },
        };

        public ref struct @Current
        {
            public static Passive Passive => DelegatePassives[CurrentPassiveID];
        }

        public static void TriggerSwitch()
        {
            MainControl.UptieLevelSelectionButtons.Visibility = Collapsed;
            MainControl.NavigationPanel_Skills_EGOAbnormalityName.Visibility = Collapsed;
            MainControl.NavigationPanel_SwitchButtons.Margin = new Thickness(2, 111, 4, 4);

            MainControl.PreviewLayouts.Height = 383;
            MainControl.EditorWidthControl.Width = new GridLength(706.6);

            TargetPreviewLayout = MainControl.PreviewLayout_Passives;

            ActiveProperties = SwitchedInterfaceProperties;

            AdjustUI(ActiveProperties.WindowSizesInfo);

            HideNavigationPanelButtons(
                  ExceptButtonsPanel: MainControl.SwitchButtons_Passives,
                ExceptPreviewLayout: MainControl.PreviewLayoutGrid_Passives
            );
        }

        public static void ValidateAndLoadStructure(FileInfo JsonFile)
        {
            var TemplateDeserialized = JsonFile.Deserealize<PassivesFile>();

            if (TemplateDeserialized != null && TemplateDeserialized.dataList != null && TemplateDeserialized.dataList.Count > 0)
            {
                if (TemplateDeserialized.dataList.Any(Passive => Passive.ID != null))
                {
                    Mode_Passives.DeserializedInfo = JsonFile.Deserealize<PassivesFile>();

                    MainWindow.FocusOnFile(JsonFile);

                    InitializePassivesDelegateFromDeserialized();
                    Mode_Passives.TriggerSwitch();

                    TransformToPassive(DelegatePassives_IDList[0]);
                }
            }
        }

        public static void TransformToPassive(int PassiveID)
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentPassiveID = PassiveID;

            MainControl.STE_NavigationPanel_ObjectID_Display
                .RichText = GetLocalizationTextFor("[Main UI] * ID Copy Button")
                .Extern(CurrentPassiveID);
            
            MainWindow.NavigationPanel_IDSwitch_CheckAvalibles();

            MainControl.NavigationPanel_ObjectName_Display.Text = @Current.Passive.Name;
            MainControl.SWBT_Passives_MainPassiveName.Text = @Current.Passive.Name.Replace("\n", "\\n");

            ReCheckPassiveSummaryButton();

            SwitchToMainDesc();
            
            {
                ManualTextLoadEvent = false;
            }
        }

        public static void ReCheckPassiveSummaryButton()
        {
            MainControl.PassiveSummarySwitchButton.IsEnabled = false;

            if (@Current.Passive.PresentSummaryDescription != null)
            {
                if (@Current.Passive.PresentSummaryDescription != @Current.Passive.EditorSummaryDescription)
                {
                    PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"].MarkWithUnsaved();
                }
                else
                {
                    PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"].SetDefaultText();
                }
                MainControl.PassiveSummarySwitchButton.IsEnabled = true;
            }
            else
            {
                PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"].SetDefaultText();
            }
        }

        public static void SwitchToMainDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentDescriptionType = DualDescriptionType.Main;

            MainControl.TextEditor.Document = @Current.Passive.DedicatedDocument_MainDesc;

            {
                ManualTextLoadEvent = true;
            }
        }

        public static void SwitchToSummaryDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentDescriptionType = DualDescriptionType.Summary;

            MainControl.TextEditor.Document = @Current.Passive.DedicatedDocument_SummaryDesc;

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
        private void Passives_SwitchToMainDesc(object RequestSender, RoutedEventArgs EventArgs) => Mode_Passives.SwitchToMainDesc();
        private void Passives_SwitchToSummaryDesc(object RequestSender, RoutedEventArgs EventArgs) => Mode_Passives.SwitchToSummaryDesc();
        private void Passives_CreateSummaryDescription(object RequestSender, RoutedEventArgs EventArgs)
        {
            Mode_Passives.@Current.Passive.PresentSummaryDescription = " ";
            Mode_Passives.@Current.Passive.EditorSummaryDescription = "";

            Mode_Passives.@Current.Passive.DedicatedDocument_SummaryDesc.Text = Mode_Passives.@Current.Passive.EditorSummaryDescription;
            Mode_Passives.@Current.Passive.DedicatedDocument_SummaryDesc.UndoStack.ClearAll();

            Mode_Passives.CurrentDescriptionType = DualDescriptionType.Summary;

            TextEditor.Document = Mode_Passives.@Current.Passive.DedicatedDocument_SummaryDesc;
            PassiveSummarySwitchButton.IsEnabled = true;
        }
    }
}