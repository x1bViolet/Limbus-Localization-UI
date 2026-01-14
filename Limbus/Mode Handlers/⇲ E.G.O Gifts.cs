using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_EGOGifts;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    public class EGOGiftUpgradeLevelSwitch : Button
    {
        public static readonly DependencyProperty IsSelectedProperty = Register<EGOGiftUpgradeLevelSwitch, bool>(Name: nameof(IsSelected), DefaultValue: false);

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
    }
}


namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    public interface Mode_EGOGifts
    {
        public static int CurrentEGOGiftID = int.MinValue;

        public static EGOGiftsFile DeserializedInfo;
        public static string CurrentDescriptionType_String { get; set; } = "Main Description"; // String instead of DualDescriptionType to insert current simple desc number at the end

        public static class OrganizedData
        {
            public static AttributesDisplayInfo_File DisplayInfo_Attributes_JSON = new();

            public static Dictionary<int, AttributesDisplayInfo_Item> DisplayInfo_Attributes = [];
            public static Dictionary<int, BitmapImage> DisplayInfo_Icons = [];

            public static Dictionary<int, List<int>> UpgradeLevelsAssociativeIDs = [];

            public record AttributesDisplayInfo_File
            {
                public string Readme { get; set; }

                [JsonProperty("E.G.O Gifts Info")]
                public List<AttributesDisplayInfo_Item> EGOGiftsInfo { get; set; }
            }
            public record AttributesDisplayInfo_Item
            {
                public int ID { get; set; }
                public string Image { get; set; }
                public string Affinity { get; set; }
                public string Keyword { get; set; }
                public string Tier { get; set; }

                [JsonIgnore]
                public BitmapImage LoadedImage { get; set; }

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    if (File.Exists(@$"[⇲] Assets Directory\Limbus Images\E.G.O Gifts\{Image}.png"))
                    {
                        LoadedImage = BitmapFromFile(@$"[⇲] Assets Directory\Limbus Images\E.G.O Gifts\{Image}.png");
                    }
                    else
                    {
                        rin($"Not found file called \"{Image}.png\" for E.G.O Gift with id '{ID}'");
                    }
                }
            }

            public static void UpdateDisplayInfo()
            {
                DisplayInfo_Attributes_JSON = new FileInfo(@"[⇲] Assets Directory\Limbus Images\E.G.O Gifts\E.G.O Gifts Display Info.json").Deserealize<AttributesDisplayInfo_File>();

                if (DisplayInfo_Attributes_JSON != null && DisplayInfo_Attributes_JSON.EGOGiftsInfo != null && DisplayInfo_Attributes_JSON.EGOGiftsInfo.Count > 0)
                {
                    foreach (AttributesDisplayInfo_Item EGOGiftViewData in DisplayInfo_Attributes_JSON.EGOGiftsInfo)
                    {
                        DisplayInfo_Icons[EGOGiftViewData.ID] = EGOGiftViewData.LoadedImage;
                        DisplayInfo_Icons[int.Parse($"1{EGOGiftViewData.ID}")] = EGOGiftViewData.LoadedImage;
                        DisplayInfo_Icons[int.Parse($"2{EGOGiftViewData.ID}")] = EGOGiftViewData.LoadedImage;

                        DisplayInfo_Attributes[EGOGiftViewData.ID] = EGOGiftViewData;
                        DisplayInfo_Attributes[int.Parse($"1{EGOGiftViewData.ID}")] = EGOGiftViewData;
                        DisplayInfo_Attributes[int.Parse($"2{EGOGiftViewData.ID}")] = EGOGiftViewData;

                        KeywordsInterrogation.EGOGiftInlineImages[$"{EGOGiftViewData.ID}"] = EGOGiftViewData.LoadedImage;
                    }
                }
            }

            public static void UpdateUpgradeLevelsAssociativeIDs()
            {
                foreach (int EGOGiftID in DelegateEGOGifts_IDList)
                {
                    if (int.TryParse($"1{EGOGiftID}", out int ExpectedID_UpgradeLevel2))
                    {
                        if (DelegateEGOGifts_IDList.Contains(ExpectedID_UpgradeLevel2))
                        {
                            DelegateEGOGifts[EGOGiftID].UpgradeLevelsAssociativeIDs.Add(EGOGiftID);
                            DelegateEGOGifts[EGOGiftID].UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel2);

                            DelegateEGOGifts[ExpectedID_UpgradeLevel2].UpgradeLevelsAssociativeIDs.Add(EGOGiftID);
                            DelegateEGOGifts[ExpectedID_UpgradeLevel2].UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel2);

                            DelegateEGOGifts[ExpectedID_UpgradeLevel2].UpgradeLevel = 2;
                        }
                    }

                    if (int.TryParse($"2{EGOGiftID}", out int ExpectedID_UpgradeLevel3))
                    {
                        if (DelegateEGOGifts_IDList.Contains(ExpectedID_UpgradeLevel3))
                        {
                            DelegateEGOGifts[EGOGiftID].UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel3);

                            DelegateEGOGifts[ExpectedID_UpgradeLevel2].UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel3);

                            DelegateEGOGifts[ExpectedID_UpgradeLevel3].UpgradeLevelsAssociativeIDs.Add(EGOGiftID);
                            DelegateEGOGifts[ExpectedID_UpgradeLevel3].UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel2);
                            DelegateEGOGifts[ExpectedID_UpgradeLevel3].UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel3);

                            DelegateEGOGifts[ExpectedID_UpgradeLevel3].UpgradeLevel = 3;
                        }
                    }
                }
            }
        }

        public static SwitchedInterfaceProperties SwitchedInterfaceProperties = new SwitchedInterfaceProperties()
        {
            Key = EditorMode.EGOGifts,
            WindowSizesInfo = new WindowSizesConfig()
            {
                Height = 635,
                Width = 812,
                MinHeight = 461,
                MinWidth = 520.3,
                MaxWidth = 812,
            },
        };

        public ref struct @Current
        {
            public static EGOGift EGOGift => DelegateEGOGifts[CurrentEGOGiftID];
        }

        public static void TriggerSwitch()
        {
            MainControl.UptieLevelSelectionButtons.Visibility = Collapsed;
            MainControl.NavigationPanel_Skills_EGOAbnormalityName.Visibility = Collapsed;
            MainControl.NavigationPanel_SwitchButtons.Margin = new Thickness(2, 111, 4, 4);

            MainControl.PreviewLayouts.Height = 420;
            MainControl.EditorWidthControl.Width = new GridLength(518);

            TargetPreviewLayout = MainControl.PreviewLayout_EGOGifts;

            ActiveProperties = SwitchedInterfaceProperties;

            AdjustUI(ActiveProperties.WindowSizesInfo);

            HideNavigationPanelButtons(
                  ExceptButtonsPanel: MainControl.SwitchButtons_EGOGifts,
                ExceptPreviewLayout: MainControl.PreviewLayoutGrid_EGOGifts
            );
        }

        public static void ValidateAndLoadStructure(FileInfo JsonFile)
        {
            var TemplateDeserialized = JsonFile.Deserealize<EGOGiftsFile>();

            if (TemplateDeserialized != null && TemplateDeserialized.dataList != null && TemplateDeserialized.dataList.Count > 0)
            {
                if (TemplateDeserialized.dataList.Any(EGOGift => EGOGift.ID != null))
                {
                    Mode_EGOGifts.DeserializedInfo = JsonFile.Deserealize<EGOGiftsFile>();

                    MainWindow.FocusOnFile(JsonFile);

                    InitializeEGOGiftsDelegateFromDeserialized();
                    Mode_EGOGifts.TriggerSwitch();
                    OrganizedData.UpdateUpgradeLevelsAssociativeIDs();

                    TransformToEGOGift(DelegateEGOGifts_IDList[0]);
                }
            }
        }

        public static void TransformToEGOGift(int EGOGiftID)
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentEGOGiftID = EGOGiftID;

            
            MainControl.STE_NavigationPanel_ObjectID_Display
                .RichText = GetLocalizationTextFor("[Main UI] * ID Copy Button")
                .Extern(CurrentEGOGiftID);
            

            MainWindow.NavigationPanel_IDSwitch_CheckAvalibles();

            MainControl.NavigationPanel_ObjectName_Display.Text = @Current.EGOGift.Name;
            MainControl.SWBT_EGOGifts_EGOGiftName.Text = @Current.EGOGift.Name.Replace("\n", "\\n");

            SwitchToMainDesc();

            ReCheckEGOGiftInfo();
            
            {
                ManualTextLoadEvent = false;
            }
        }

        public static void ReCheckEGOGiftInfo()
        {
            for (int i = 1; i <= 10; i++) InterfaceObject<Button>($"SimpleDescSwitchButton_{i}").IsEnabled = false;

            if (@Current.EGOGift.SimpleDescriptions != null && @Current.EGOGift.SimpleDescriptions.Count > 0)
            {
                for (int i = 1; i <= @Current.EGOGift.SimpleDescriptions.Count & i <= 10; i++)
                {
                    if (@Current.EGOGift.SimpleDescriptions[i - 1].PresentDescription != null)
                    {
                        InterfaceObject<Button>($"SimpleDescSwitchButton_{i}").IsEnabled = true;

                        if (@Current.EGOGift.SimpleDescriptions[i - 1].PresentDescription != @Current.EGOGift.SimpleDescriptions[i - 1].EditorDescription)
                        {
                            PresentedStaticTextEntries[$"[E.G.O Gifts / Right Menu] * Simple Desc {i}"].MarkWithUnsaved();
                        }
                        else
                        {
                            PresentedStaticTextEntries[$"[E.G.O Gifts / Right Menu] * Simple Desc {i}"].SetDefaultText();
                        }
                    }
                }
            }

            _ = MainControl.EGOGift_FastSwitch_2.Visibility
              = MainControl.EGOGift_FastSwitch_3.Visibility
              = Collapsed;

            _ = MainControl.EGOGift_FastSwitch_1.IsSelected
              = MainControl.EGOGift_FastSwitch_2.IsSelected
              = MainControl.EGOGift_FastSwitch_3.IsSelected
              = false;

            switch (@Current.EGOGift.UpgradeLevelsAssociativeIDs.Count)
            {
                case 2:
                    MainControl.EGOGift_FastSwitch_2.Visibility = Visible;
                    break;

                case 3:
                    MainControl.EGOGift_FastSwitch_2.Visibility = Visible;
                    MainControl.EGOGift_FastSwitch_3.Visibility = Visible;
                    break;

                default: break;
            }

            InterfaceObject<EGOGiftUpgradeLevelSwitch>($"EGOGift_FastSwitch_{@Current.EGOGift.UpgradeLevel}").IsSelected = true;


            _ = MainControl.EGOGiftDisplay_UpgradeLevel2Border.Visibility
              = MainControl.EGOGiftDisplay_UpgradeLevel2_OnIcon.Visibility
              = @Current.EGOGift.UpgradeLevel >= 2 ? Visible : Collapsed;

            _ = MainControl.EGOGiftDisplay_UpgradeLevel3Border.Visibility
              = MainControl.EGOGiftDisplay_UpgradeLevel3_OnIcon.Visibility
              = @Current.EGOGift.UpgradeLevel == 3 ? Visible : Collapsed; ;


            try
            {
                ReCheckEGOGiftDisplayInfo();
            }
            catch { }
        }

        public static void ReCheckEGOGiftDisplayInfo()
        {
            MainControl.EGOGiftDisplay_MainIcon.Source = OrganizedData.DisplayInfo_Icons.ContainsKey(CurrentEGOGiftID)
                ? OrganizedData.DisplayInfo_Icons[CurrentEGOGiftID]
                : KeywordsInterrogation.KeywordImages["Unknown"];

            if (OrganizedData.DisplayInfo_Attributes.ContainsKey(CurrentEGOGiftID))
            {
                MainControl.EGOGiftDisplay_Tier.Text = OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Tier switch
                {
                    "1" => "I",
                    "2" => "II",
                    "3" => "III",
                    "4" => "IV",
                    "5" => "V",
                    _ => ""
                };

                if (OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Tier == "EX")
                {
                    MainControl.EGOGiftDisplay_Tier.Visibility = Collapsed;
                    MainControl.EGOGiftDisplay_EXTier.Visibility = Visible;
                }
                else
                {
                    MainControl.EGOGiftDisplay_Tier.Visibility = Visible;
                    MainControl.EGOGiftDisplay_EXTier.Visibility = Collapsed;
                }

                if (OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Keyword != "-")
                {
                    MainControl.EGOGiftDisplay_Keyword.Visibility = Visible;

                    MainControl.EGOGiftDisplay_Keyword.Source = OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Keyword switch
                    {
                        "Burn" => KeywordsInterrogation.KeywordImages["Combustion"],
                        "Bleed" => KeywordsInterrogation.KeywordImages["Laceration"],
                        "Tremor" => KeywordsInterrogation.KeywordImages["Vibration"],
                        "Poise" => KeywordsInterrogation.KeywordImages["Breath"],
                        "Charge" => KeywordsInterrogation.KeywordImages["Charge"],
                        "Rupture" => KeywordsInterrogation.KeywordImages["Burst"],
                        "Sinking" => KeywordsInterrogation.KeywordImages["Sinking"],
                        "Blunt" => KeywordsInterrogation.KeywordImages["LCLocaliazationInterface_Blunt"],
                        "Pierce" => KeywordsInterrogation.KeywordImages["LCLocaliazationInterface_Pierce"],
                        "Slash" => KeywordsInterrogation.KeywordImages["LCLocaliazationInterface_Slash"],
                        _ => KeywordsInterrogation.KeywordImages.ContainsKey(OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Keyword)
                                ? KeywordsInterrogation.KeywordImages[OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Keyword]
                                : KeywordsInterrogation.KeywordImages["Unknown"],
                    };
                }
                else
                {
                    MainControl.EGOGiftDisplay_Keyword.Visibility = Collapsed;
                    MainControl.EGOGiftDisplay_Keyword.Source = new BitmapImage();
                }


                string SelectAffinityForNameBackground = OrganizedData.DisplayInfo_Attributes[CurrentEGOGiftID].Affinity switch
                {
                    "Wrath" => "Wrath",
                    "Lust" => "Lust",
                    "Sloth" => "Sloth",
                    "Gluttony" => "Gluttony",
                    "Gloom" => "Gloom",
                    "Pride" => "Pride",
                    "Envy" => "Envy",
                    _ => "None"
                };

                MainControl.EGOGiftNameBackground.Source = BitmapFromResource($"UI/Limbus/E.G.O Gifts/Affinity-Colored E.G.O Gifts Name Backgrounds/{SelectAffinityForNameBackground}.png");

            }
            else
            {
                MainControl.EGOGiftNameBackground.Source = BitmapFromResource($"UI/Limbus/E.G.O Gifts/Affinity-Colored E.G.O Gifts Name Backgrounds/None.png");
                MainControl.EGOGiftDisplay_Tier.Text = "";
            }
        }

        public static void SwitchToMainDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentDescriptionType_String = "Main Description";

            MainControl.TextEditor.Document = @Current.EGOGift.DedicatedDocument;

            {
                ManualTextLoadEvent = true;
            }
        }

        public static void SwitchToSimpleDesc(string SimpleDescNumber)
        {
            if (SimpleDescNumber == "") return;

            {
                ManualTextLoadEvent = true;
            }

            CurrentDescriptionType_String = $"Simple Description №{SimpleDescNumber}";

            MainControl.TextEditor.Document = @Current.EGOGift.SimpleDescriptions[int.Parse(SimpleDescNumber) - 1].DedicatedDocument;

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
        private void EGOGiftDisplay_HotSwitchToUpgradeLevel_SwitchButton(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            int TargetUpgradeLevel = int.Parse((RequestSender as Button).Uid) - 1;

            if (Mode_EGOGifts.@Current.EGOGift.UpgradeLevelsAssociativeIDs.Count > 0)
            {
                Mode_EGOGifts.TransformToEGOGift(Mode_EGOGifts.@Current.EGOGift.UpgradeLevelsAssociativeIDs[TargetUpgradeLevel]);
            }
        }

        private void EGOGifts_SwitchToMainDesc(object RequestSender, RoutedEventArgs EventArgs) => Mode_EGOGifts.SwitchToMainDesc();
        private void EGOGifts_SwitchToSimpleDesc(object RequestSender, RoutedEventArgs EventArgs) => Mode_EGOGifts.SwitchToSimpleDesc((RequestSender as Button).Uid);
    }
}