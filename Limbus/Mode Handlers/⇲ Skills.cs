using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using LC_Localization_Task_Absolute.PreviewCreator;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Json.SkillsDisplayInfo;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    public class UptieLevelButton : Button
    {
        public static readonly DependencyProperty IsSelectedProperty = Register<UptieLevelButton, bool>(Name: nameof(IsSelected), DefaultValue: false);

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }


        public static readonly DependencyProperty AvailableProperty = Register<UptieLevelButton, bool>(Name: nameof(Available), DefaultValue: true);

        /// <summary>
        /// IsEnabled alternative that leaves button accessible (ContextMenu with the "Add Uptie" (Manual json files managin) button should still be available), controlled by the style
        /// </summary>
        public bool Available
        {
            get => (bool)GetValue(AvailableProperty);
            set => SetValue(AvailableProperty, value);
        }
    }
}

namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    public interface Mode_Skills
    {
        public static int CurrentSkillID = -1;
        public static int CurrentSkillUptieLevel = -1;
        public static int CurrentSkillCoinIndex = -1;
        public static int CurrentSkillCoinDescIndex = -1;

        public static SkillsFile DeserializedInfo;

        public static double LastRegisteredWidth = 0;
        public static SwitchedInterfaceProperties SwitchedInterfaceProperties = new SwitchedInterfaceProperties()
        {
            Key = EditorMode.Skills,
            WindowSizesInfo = new WindowSizesConfig()
            {
                Height = 570,
                Width = 1000,
                MinHeight = 420,
                MinWidth = 708.8,
                MaxWidth = 1000,
            },
        };

        public static int MaxCoinsAmount => MainControl.CoinsStackPanel.Children.Count;
        public static int MaxCoinDescsAmount => MainControl.FirstCoinDescs.Children.Count;

        /// <summary>
        /// For <see cref="LimbusPreviewFormatter.UpdateLast"/> to fully update skill instead of only one of descs
        /// </summary>
        public static readonly Dictionary<TMProEmitter, string> LastPreviewUpdatesBank = [];
        public static readonly List<int> CurrentCoinDescs_Avalible = [];

        public static readonly BitmapImage RegularCoinIcon = BitmapFromResource($"UI/Limbus/Skills/Regular Coin.png");
        public static readonly BitmapImage UnbreakableCoinIcon = BitmapFromResource($"UI/Limbus/Skills/Unbreakable Coin.png");
        public static readonly BitmapImage DefaultSkillFrameAlt = BitmapFromResource($"UI/Limbus/Skills/Frames/Skill Default Frame alt.png");

        public static readonly Dictionary<string, BitmapImage> AffinityIcons = [];
        public static readonly Dictionary<string, BitmapImage> SkillFrames = [];
        public static readonly Dictionary<string, BitmapImage> SkillIcons = [];
        public static readonly Dictionary<int, Type_RawSkillsDisplayInfo.DetailedInfoItem> OrganizedDisplayInfo = [];

        public static readonly Dictionary<string, BitmapImage> DefaultIcons = [];

        static Mode_Skills()
        {
            foreach (string Affinity in new string[] { "Wrath", "Lust", "Sloth", "Gluttony", "Gloom", "Pride", "Envy" })
            {
                AffinityIcons[Affinity] = BitmapFromResource($"UI/Limbus/Skills/Affinity Icons/{Affinity}.png");

                for (int i = 1; i <= 3; i++)
                {
                    SkillFrames[$"{Affinity} {i}"] = BitmapFromResource($"UI/Limbus/Skills/Frames/{Affinity}/{i}.png");
                }
            }
            SkillFrames["None"] = BitmapFromResource($"UI/Limbus/Skills/Frames/Skill Default Frame.png");
        }

        public ref struct @Current
        {
            public static Dictionary<int, UptieLevel> Skill
                => DelegateSkills[CurrentSkillID];

            public static UptieLevel Uptie
                => DelegateSkills[CurrentSkillID][CurrentSkillUptieLevel];

            public static Coin Coin
                => DelegateSkills[CurrentSkillID][CurrentSkillUptieLevel]
                    .Coins[CurrentSkillCoinIndex];

            public static List<CoinDesc> CoinDescs
                => DelegateSkills[CurrentSkillID][CurrentSkillUptieLevel]
                    .Coins[CurrentSkillCoinIndex]
                        .CoinDescriptions;

            public static CoinDesc CoinDesc
                => DelegateSkills[CurrentSkillID][CurrentSkillUptieLevel]
                    .Coins[CurrentSkillCoinIndex]
                        .CoinDescriptions[CurrentSkillCoinDescIndex];
        }

        // Read from file at moment or get one from list
        public static BitmapImage AcquireSkillIcon(string ID)
        {
            if (SkillIcons.ContainsKey(ID)) return SkillIcons[ID];
            else
            {
                if (File.Exists(ID))
                {
                    return BitmapFromFile(ID);
                }
                else
                {
                    FileInfo IconFile = @"[⇲] Assets Directory\Limbus Images\Skills\Icons".GetFileWithName($"{ID}.png");
                    if (IconFile != null)
                    {
                        BitmapImage FoundedImage = BitmapFromFile(IconFile.FullName);
                        SkillIcons[ID] = FoundedImage;

                        return FoundedImage;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public static void LoadDisplayInfo()
        {
            foreach (string SkillType in new List<string> { "Attack", "Guard", "Evade", "Counter" })
            {
                DefaultIcons[$"{SkillType} None"] = BitmapFromFile(@$"[⇲] Assets Directory\Limbus Images\Skills\Icons\[⇲] Default\None\{SkillType}.png");
                
                foreach (string Affinity in new List<string> { "Wrath", "Lust", "Sloth", "Gluttony", "Gloom", "Pride", "Envy" })
                {
                    DefaultIcons[$"{SkillType} {Affinity}"] = BitmapFromFile(@$"[⇲] Assets Directory\Limbus Images\Skills\Icons\[⇲] Default\{Affinity}\{SkillType}.png");
                }
            }

            if (Directory.Exists(@"[⇲] Assets Directory\Limbus Images\Skills\Display Info"))
            {
                foreach (FileInfo SkillDataFile in new DirectoryInfo(@"[⇲] Assets Directory\Limbus Images\Skills\Display Info").GetFiles("*.json", SearchOption.AllDirectories))
                {
                    Type_RawSkillsDisplayInfo.SkillsDetailedInfo Deserialized = SkillDataFile.Deserealize<Type_RawSkillsDisplayInfo.SkillsDetailedInfo>();

                    if (Deserialized.List != null)
                    {
                        foreach (Type_RawSkillsDisplayInfo.DetailedInfoItem SkillData in Deserialized.List)
                        {
                            if (SkillData.ID != null && SkillData.UptieLevelsDictionary != null)
                            {
                                OrganizedDisplayInfo[(int)SkillData.ID] = SkillData;
                            }
                        }
                    }
                }
            }
        }


        public static void ChangeSkillNameReplicaAppearance()
        {
            bool SwitchSkillViewReplicaToUnknown = false;
            
            if (OrganizedDisplayInfo.ContainsKey(CurrentSkillID) & !LoadedSkillConstructors.ContainsKey(CurrentSkillID))
            {
                int UptieInfoCheck = CurrentSkillUptieLevel;

                bool FoundInfoToDisplay = OrganizedDisplayInfo[CurrentSkillID].UptieLevelsDictionary.ContainsKey(UptieInfoCheck);
                if (!FoundInfoToDisplay & UptieInfoCheck == 1)
                {
                    // As example LCE Faust third alt skill has default desc on uptie 1, but all display info in skill data inside uptie 3 insted, and there it asks for uptie 1
                    bool ContainsAsAltThirdLevel = OrganizedDisplayInfo[CurrentSkillID].UptieLevelsDictionary.ContainsKey(3);
                    if (ContainsAsAltThirdLevel)
                    {
                        UptieInfoCheck = 3;
                        FoundInfoToDisplay = true;
                    }
                }
                else if (!FoundInfoToDisplay & UptieInfoCheck == 3) // Directly opposite situation with Dieci Rodion defense skill: main skill data on uptie 1 in data files, in localization files first avalible desc on uptie 3
                {
                    bool ContainsAsAltFirstLevel = OrganizedDisplayInfo[CurrentSkillID].UptieLevelsDictionary.ContainsKey(1);
                    if (ContainsAsAltFirstLevel)
                    {
                        UptieInfoCheck = 1;
                        FoundInfoToDisplay = true;
                    }
                }

                if (FoundInfoToDisplay)
                {
                    try
                    {
                        Type_RawSkillsDisplayInfo.DetailedInfoItem_UptieLevel Info_Uptie = OrganizedDisplayInfo[CurrentSkillID].UptieLevelsDictionary[UptieInfoCheck];
                        Type_RawSkillsDisplayInfo.DetailedInfoItem Info_Main = OrganizedDisplayInfo[CurrentSkillID];

                        // Affinity color and frame
                        if (Info_Uptie.Affinity_UPTIE != null)
                        {
                            MainControl.SkillReplicaAffinityColorHolder.Background = Info_Uptie.Affinity_UPTIE switch
                            {
                                "Wrath"    => ToSolidColorBrush("#fe0302"),
                                "Lust"     => ToSolidColorBrush("#fe6e04"),
                                "Sloth"    => ToSolidColorBrush("#fed133"),
                                "Gluttony" => ToSolidColorBrush("#a5fe06"),
                                "Gloom"    => ToSolidColorBrush("#1ec6ef"),
                                "Pride"    => ToSolidColorBrush("#044ed6"),
                                "Envy"     => ToSolidColorBrush("#9808de"),
                                _ => ToSolidColorBrush("#9f6a3a")
                            };
                            MainControl.SkillNamesReplication_Background_RightSidePatternedImage.Source = BitmapFromResource($"UI/Limbus/Skills/Name Background/Game Colors/{Info_Uptie.Affinity_UPTIE}.png");

                            // Frame
                            string FrameName = $"{Info_Uptie.Affinity_UPTIE} {Info_Main.Rank}";
                            if (Info_Uptie.Affinity_UPTIE == "None") FrameName = "None";
                            MainControl.SkilFrame.Source = SkillFrames[FrameName];
                        }


                        // Icon
                        string TargetIconID = $"{CurrentSkillID}";
                        if (Info_Uptie.IconID != null) // If icon is alt from some other skill
                        {
                            TargetIconID = Info_Uptie.IconID;
                        }
                        BitmapImage AcquiredImage = AcquireSkillIcon(TargetIconID); // Search in list of icons or read from file
                        if (AcquiredImage == null & Info_Uptie.Affinity_UPTIE != null) // If file not found
                        {
                            AcquiredImage = DefaultIcons[$"{Info_Main.SkillAction} {Info_Uptie.Affinity_UPTIE}"];
                        }
                        MainControl.SkilIcon.Source = AcquiredImage;

                        // Attack Weight
                        if (Info_Main.AttackWeight != null)
                        {
                            if (Info_Main.AttackWeight > 0)
                            {
                                MainControl.SkillAtkWeight.Text = new string('■', (int)Info_Main.AttackWeight);
                            }
                            else
                            {
                                MainControl.SkillAtkWeight.Text = "■";
                            }
                        }



                        // Resets
                        MainControl.OffenseLevelIcon.Visibility = Collapsed;
                        MainControl.DefenseLevelIcon.Visibility = Collapsed;
                        MainControl.SkillCopiesGrid.Visibility = Collapsed;
                        MainControl.DamageTypeGrid.Visibility = Collapsed;

                        MainControl.SkillDamageType_Blunt.Visibility = Collapsed;
                        MainControl.SkillDamageType_Slash.Visibility = Collapsed;
                        MainControl.SkillDamageType_Pierce.Visibility = Collapsed;


                        // Attributes from constructor
                        MainControl.AttackWeightPanel.Visibility = Visible;
                        MainControl.SkillAffinityIcon.Visibility = Collapsed;
                        MainControl.SkillLevel.Visibility = Visible;
                        MainControl.SkillLevelTypeIcons.Visibility = Visible;
                        MainControl.CoinPowerBackground.Visibility = Visible;


                        // Offense|Defense Level icons
                        if (Info_Main.SkillAction.EqualsOneOf("Attack", "Counter"))
                        {
                            MainControl.OffenseLevelIcon.Visibility = Visible;
                            MainControl.DamageTypeGrid.Visibility = Visible;
                        }
                        else
                        {
                            MainControl.DefenseLevelIcon.Visibility = Visible;
                        }

                        if ((bool)Info_Main.Unobservable) // If skill marked as that
                        {
                            MainControl.SkillLevel.Text = "???";
                            MainControl.SkillLevel.TextDecorations = null;

                            MainControl.SkillCopiesGrid.Visibility = Collapsed;


                            MainControl.CoinPowerValue.Text = $"{Info_Main.CoinMathOperator}" + "?";
                            
                            MainControl.BasePowerValue.Text = "?";
                        }
                        else
                        {
                            MainControl.SkillLevel.TextDecorations = TextDecorations.Underline;

                            // Level value
                            if (Info_Main.LevelCorrection != null)
                            {
                                MainControl.SkillLevel.Text = $"{Info_Main.BaseLevel + Info_Main.LevelCorrection}";
                            }

                            // Coin|Base Power values
                            if (Info_Uptie.CoinPower_UPTIE != null)
                            {
                                MainControl.CoinPowerValue.Text =
                                    $"{Info_Main.CoinMathOperator}" +
                                    $"{Info_Uptie.CoinPower_UPTIE}";
                            }
                            if (Info_Uptie.BasePower != null)
                            {
                                MainControl.BasePowerValue.Text =
                                    $"{Info_Uptie.BasePower}";
                            }

                            // Copies icon
                            if (
                                Info_Main.SkillAction == "Attack" &
                                MainWindow.CurrentFile.Name.ContainsOneOf(["Skills.json", "Skills_personality-"])
                            ) {
                                MainControl.SkillCopiesGrid.Visibility = Visible;

                                MainControl.SkillCopiesAmount.Text = Info_Main.Rank switch { 1 => "3", 2 => "2", 3 => "1", _ => "0" };
                            }
                        }


                        // Damage Type
                        switch (Info_Main.DamageType)
                        {
                            case "Blunt":
                                MainControl.SkillDamageType_Blunt.Visibility = Visible;
                                break;

                            case "Slash":
                                MainControl.SkillDamageType_Slash.Visibility = Visible;
                                break;

                            case "Pierce":
                                MainControl.SkillDamageType_Pierce.Visibility = Visible;
                                break;

                            default: break;
                        }


                        if (Info_Main.CoinTypesSequence.Count > 0)
                        {
                            MainControl.SkillReplicaCoinsTab.Children.Clear();

                            foreach (string CoinType in Info_Main.CoinTypesSequence)
                            {
                                MainControl.SkillReplicaCoinsTab.Children.Add(CoinType switch
                                {
                                    "Regular"     => new Image() { Source = RegularCoinIcon     },
                                    "Unbreakable" => new Image() { Source = UnbreakableCoinIcon }
                                });
                            }
                        }
                        else
                        {
                            MainControl.SkillReplicaCoinsTab.Children.Add(new Image() { Source = RegularCoinIcon });
                        }
                    }
                    catch (Exception ex) { rin(FormattedStackTrace(ex, "Skill name construction")); }
                }
                else
                {
                    if (OrganizedDisplayInfo[CurrentSkillID].UptieLevelsDictionary.Count == 0)
                    {
                        SwitchSkillViewReplicaToUnknown = true;
                    }
                }
            }


            else SwitchSkillViewReplicaToUnknown = true;











            if (SwitchSkillViewReplicaToUnknown)
            {
                MainControl.SkillCopiesGrid.Visibility = Collapsed;
                MainControl.DamageTypeGrid.Visibility = Collapsed;

                MainControl.OffenseLevelIcon.Visibility = Visible;
                MainControl.DefenseLevelIcon.Visibility = Collapsed;

                MainControl.SkillAffinityIcon.Visibility = Collapsed;

                MainControl.CoinPowerValue.Text = "+?";
                MainControl.BasePowerValue.Text = "?";

                MainControl.SkillAtkWeight.Text = "■";

                MainControl.SkillLevel.Visibility = Visible;
                MainControl.SkillLevel.Text = "??";

                MainControl.SkillReplicaAffinityColorHolder.Background = ToSolidColorBrush("#9f6a3a");
                MainControl.SkillNamesReplication_Background_RightSidePatternedImage.Source = BitmapFromResource($"UI/Limbus/Skills/Name Background/Game Colors/None.png");

                MainControl.SkilIcon.Source = new BitmapImage();
                MainControl.SkilFrame.Source = DefaultSkillFrameAlt;

                MainControl.SkillReplicaCoinsTab.Children.Clear();
                MainControl.SkillReplicaCoinsTab.Children.Add(new Image() { Source = RegularCoinIcon }); // Add single coin
            }
        }

        public static bool EnableUptieLevels_Recent = false;
        public static bool EnableEGOAbnormalityName_Recent = false;
        
        public static void TriggerSwitch(bool EnableUptieLevels, bool EnableEGOAbnormalityName)
        {
            EnableUptieLevels_Recent = EnableUptieLevels;
            EnableEGOAbnormalityName_Recent = EnableEGOAbnormalityName;

            MainControl.PreviewLayouts.Height = 383;
            MainControl.EditorWidthControl.Width = new GridLength(706.6);

            if (EnableEGOAbnormalityName)
            {
                MainControl.PreviewLayout_EGOSkills_Background.Visibility = Visible;
                SwitchedInterfaceProperties.WindowSizesInfo.Height = 603.5;
                MainControl.SkillNamesReplication_SkillName_Text.MaxWidth = 390;
            }
            else
            {
                MainControl.PreviewLayout_EGOSkills_Background.Visibility = Collapsed;
                SwitchedInterfaceProperties.WindowSizesInfo.Height = 573;
                MainControl.SkillNamesReplication_SkillName_Text.MaxWidth = 465;
            }

            LastRegisteredWidth = EnableEGOAbnormalityName ? 578 : 663;
            MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = LastRegisteredWidth;

            if (EnableUptieLevels)
            {
                MainControl.UptieLevelSelectionButtons.Visibility = Visible;
                MainControl.NavigationPanel_SwitchButtons.Margin = new Thickness(2, 168, 4, 4);
            }
            else
            {
                MainControl.UptieLevelSelectionButtons.Visibility = Collapsed;
                MainControl.NavigationPanel_SwitchButtons.Margin = new Thickness(2, 111, 4, 4);
            }

            if (EnableEGOAbnormalityName)
            {
                MainControl.NavigationPanel_Skills_EGOAbnormalityName.Visibility = Visible;
            }
            else
            {
                MainControl.NavigationPanel_Skills_EGOAbnormalityName.Visibility = Collapsed;
            }


            MainWindow.TargetPreviewLayout = MainControl.PreviewLayout_Skills_MainDesc;

            ActiveProperties = SwitchedInterfaceProperties;

            AdjustUI(ActiveProperties.WindowSizesInfo);

            HideNavigationPanelButtons(
                ExceptButtonsPanel  : MainControl.SwitchButtons_Skills,
                ExceptPreviewLayout: MainControl.PreviewLayoutGrid_Skills
            );
        }

        public static void ValidateAndLoadStructure(FileInfo JsonFile, bool EnableUptieLevels, bool EnableEGOAbnormalityName)
        {
            var TemplateDeserialized = JsonFile.Deserealize<SkillsFile>();

            if (TemplateDeserialized != null && TemplateDeserialized.dataList != null && TemplateDeserialized.dataList.Count > 0)
            {
                if (TemplateDeserialized.dataList.Any(Skill => Skill.ID != null & (Skill.UptieLevels != null && Skill.UptieLevels.Count > 0)))
                {
                    Mode_Skills.DeserializedInfo = JsonFile.Deserealize<SkillsFile>();

                    MainWindow.FocusOnFile(JsonFile);

                    InitializeSkillsDelegateFromDeserialized();
                    Mode_Skills.TriggerSwitch(
                                EnableUptieLevels: EnableUptieLevels,
                        EnableEGOAbnormalityName: EnableEGOAbnormalityName
                    );

                    TransformToSkill(DelegateSkills_IDList[0]);
                }
            }
        }

        public static void TransformToSkill(int TargetSkillID, int TranzUptieLevel = -1)
        {
            {
                ManualTextLoadEvent = true;
            }

            CurrentSkillID = TargetSkillID;

            int SwitchingUptieLevel = TranzUptieLevel switch
            {
                -1 => @Current.Skill.Keys.ToList()[0],
                _  => TranzUptieLevel
            };

            CurrentSkillUptieLevel = SwitchingUptieLevel;



            // ヽ( `д´*)ノ
            if (@Current.Skill.Keys.Count == 6)
            {
                MainControl.SkillUptieLevelSign_ParentViewBox.Visibility = Collapsed;
                MainControl.UptieSwitchButtons.HorizontalAlignment = HorizontalAlignment.Center;
                MainControl.SwitchUptieLevelButton_6.Visibility = Visible;
            }
            else
            {
                MainControl.SkillUptieLevelSign_ParentViewBox.Visibility = Visible;
                MainControl.UptieSwitchButtons.HorizontalAlignment = HorizontalAlignment.Right;
                MainControl.SwitchUptieLevelButton_6.Visibility = Collapsed;
            }

            // Show fifth uptie button if exists i'll have nightmares about viewboxesi'll have nightmares about viewboxesi'll have nightmares about viewboxesi'll have nightmares about viewboxesi'll have nightmares about viewboxes
            if (@Current.Skill.Keys.Count == 5)
            {
                MainControl.SwitchUptieLevelButton_5.Visibility = Visible;
                MainControl.SkillUptieLevelSign.MinWidth = 47;
                MainControl.SkillUptieLevelSign.MaxWidth = 70;
                MainControl.SkillUptieLevelSign_ParentViewBox.Width = 75;
                MainControl.SkillUptieLevelSign_ParentViewBox.SetLeftMargin(5);
            }
            else
            {
                MainControl.SwitchUptieLevelButton_5.Visibility = Collapsed;
                MainControl.SkillUptieLevelSign.MinWidth = 77;
                MainControl.SkillUptieLevelSign.MaxWidth = 111;
                MainControl.SkillUptieLevelSign_ParentViewBox.Width = 110;
                MainControl.SkillUptieLevelSign_ParentViewBox.SetLeftMargin(6.7);
            }

            
            ResetSkillInfo();
            LastPreviewUpdatesBank.Clear();
            
            SwitchToMainDesc();
            ManualTextLoadEvent = true; // Undo from SwitchToMainDesc()

            MainControl.STE_NavigationPanel_ObjectID_Display
                .RichText = GetLocalizationTextFor("[Main UI] * ID Copy Button")
                .Extern(CurrentSkillID);

            
            MainControl.SkillAffinitySelector.SelectedIndex = @Current.Uptie.OptionalAffinity switch
            {
                   "Wrath" => 0,
                    "Lust" => 1,
                   "Sloth" => 2,
                "Gluttony" => 3,
                   "Gloom" => 4,
                   "Pride" => 5,
                    "Envy" => 6,
                    "None" => 7,
                _ => 7
            };

            MainWindow.NavigationPanel_IDSwitch_CheckAvalibles();

            PresentedStaticTextEntries["[Skills / Right menu] * Skill Coin desc number"].SetDefaultText(ExtraExtern: SpecializedDefs.InsertionsDefaultValue);

            foreach (int UptieLevelNumber in @Current.Skill.Keys.ToList())
            {
                if (UptieLevelNumber <= 6)
                {
                    InterfaceObject<UptieLevelButton>($"SwitchUptieLevelButton_{UptieLevelNumber}").Available = true;
                }
            }


            if (CurrentSkillUptieLevel <= 6) InterfaceObject<UptieLevelButton>($"SwitchUptieLevelButton_{CurrentSkillUptieLevel}").IsSelected = true;

            MainControl.NavigationPanel_ObjectName_Display.Text = @Current.Uptie.Name;
            MainControl.SWBT_Skills_MainSkillName.Text = @Current.Uptie.Name.Replace("\n", "\\n");
            if (@Current.Uptie.EGOAbnormalityName != null)
            {
                MainControl.SWBT_Skills_EGOAbnormalitySkillName.Text = @Current.Uptie.EGOAbnormalityName.Replace("\n", "\\n");
            }

            Dictionary<TMProEmitter, Visibility> VisibilityChangeQuery = [];


            if (LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSkillNamesReplication)
            {
                try { ChangeSkillNameReplicaAppearance(); }
                catch (Exception ex) { rin(FormattedStackTrace(ex, "Skill name construction")); }
            }


            if (@Current.Uptie.Coins != null)
            {
                int CoinNumber = 1;
                foreach (Coin CurrentCoin in @Current.Uptie.Coins)
                {
                    if (CurrentCoin.CoinDescriptions != null && CurrentCoin.CoinDescriptions.Count > 0)
                    {
                        if (CurrentCoin.CoinDescriptions.Where(x => x.PresentDescription == null).Count() != CurrentCoin.CoinDescriptions.Count)
                        {
                            Grid MainCoinPanel = InterfaceObject<Grid>($"PreviewLayout_Skills_Coin{CoinNumber}");

                            if (MainCoinPanel != null)
                            {

                                if (@Current.Uptie.Coins[CoinNumber - 1].CoinDescriptions.Any(x => x.PresentDescription != x.EditorDescription))
                                {
                                    PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {CoinNumber}"].MarkWithUnsaved();
                                }
                                else
                                {
                                    PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {CoinNumber}"].SetDefaultText();
                                }


                                if (CurrentCoin.CoinDescriptions.Where(x => x.EditorDescription.EqualsOneOf("", "<style=\"highlight\"></style>")).Count() != CurrentCoin.CoinDescriptions.Count)
                                {
                                    MainCoinPanel.Visibility = Visible;
                                }
                                else
                                {
                                    MainCoinPanel.Visibility = Collapsed;
                                }


                                int CoinDescNumber = 1;
                                foreach (CoinDesc CoinDescription in CurrentCoin.CoinDescriptions)
                                {
                                    if (CoinDescription.PresentDescription != null)
                                    {
                                        TMProEmitter ThisCoinDescPanel = InterfaceObject<TMProEmitter>($"PreviewLayout_Skills_Coin{CoinNumber}_Desc{CoinDescNumber}");
                                        
                                        InterfaceObject<Button>($"CoinSwitchButton_{CoinNumber}").IsEnabled = true; // Make coin switch button enabled

                                        #pragma EditorDescription is always actual recently edited text!!!!!!!!!!!!!!!!!!!
                                        if (CoinDescription.EditorDescription != "")
                                        {
                                            VisibilityChangeQuery[ThisCoinDescPanel] = Visible;
                                            ThisCoinDescPanel.RichText = CoinDescription.EditorDescription;
                                        }

                                        LastPreviewUpdatesBank[ThisCoinDescPanel] = CoinDescription.EditorDescription;

                                        if (CoinDescription.PresentDescription != CoinDescription.EditorDescription)
                                        {
                                            PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {CoinNumber}"].MarkWithUnsaved();
                                        }
                                        else
                                        {
                                            PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {CoinNumber}"].SetDefaultText();
                                        }
                                    }

                                    if (CoinDescNumber == MaxCoinDescsAmount) break;

                                    CoinDescNumber++;
                                }
                            }
                        }
                    }

                    if (CoinNumber == MaxCoinsAmount) break;
                    
                    CoinNumber++;
                }
            }

            foreach (KeyValuePair<TMProEmitter, Visibility> VisibilityOrder in VisibilityChangeQuery)
            {
                VisibilityOrder.Key.Visibility = VisibilityOrder.Value;
            }


            {
                ManualTextLoadEvent = false;
            }
        }

        public static void ResetSkillInfo()
        {
            for (int UptieLevelNumber = 1; UptieLevelNumber <= 6; UptieLevelNumber++)
            {
                InterfaceObject<UptieLevelButton>($"SwitchUptieLevelButton_{UptieLevelNumber}").Available = false;
                InterfaceObject<UptieLevelButton>($"SwitchUptieLevelButton_{UptieLevelNumber}").IsSelected = false;
            }

            for (int CoinNumber = 1; CoinNumber <= MaxCoinsAmount; CoinNumber++)
            {
                for (int CoinDescNumber = 1; CoinDescNumber <= MaxCoinDescsAmount; CoinDescNumber++)
                {
                    TMProEmitter CoinDescPanel = InterfaceObject<TMProEmitter>($"PreviewLayout_Skills_Coin{CoinNumber}_Desc{CoinDescNumber}");
                    CoinDescPanel.RichText = "";
                    CoinDescPanel.Visibility = Collapsed;
                    
                    PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {CoinNumber}"].SetDefaultText();
                }

                InterfaceObject<Button>($"CoinSwitchButton_{CoinNumber}").IsEnabled = false;
                InterfaceObject<Grid>($"PreviewLayout_Skills_Coin{CoinNumber}").Visibility = Collapsed;
            }
        }

        public static void SwitchToMainDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            MainControl.CoinDescSwitchButton_Next.IsEnabled = false;
            MainControl.CoinDescSwitchButton_Prev.IsEnabled = false;
            MainControl.CurrentCoinDesc_Display.IsEnabled = false;

            PresentedStaticTextEntries["[Skills / Right menu] * Skill Coin descs title"].SetDefaultText(ExtraExtern: SpecializedDefs.InsertionsDefaultValue);
            PresentedStaticTextEntries["[Skills / Right menu] * Skill Coin desc number"].SetDefaultText(ExtraExtern: SpecializedDefs.InsertionsDefaultValue);

            TargetPreviewLayout = MainControl.PreviewLayout_Skills_MainDesc;

            MainControl.TextEditor.Document = @Current.Uptie.DedicatedDocument;

            {
                ManualTextLoadEvent = false;
            }
        }

        public static void SetCoinFocus(int CoinNumber, bool AutoSwitchToFirstCoinDesc = true)
        {
            MainControl.CoinDescSwitchButton_Next.IsEnabled = false;
            MainControl.CoinDescSwitchButton_Prev.IsEnabled = false;
            MainControl.CurrentCoinDesc_Display.IsEnabled = true;

            PresentedStaticTextEntries["[Skills / Right menu] * Skill Coin descs title"].SetDefaultText(ExtraExtern: CoinNumber);

            CurrentCoinDescs_Avalible.Clear();
            CurrentSkillCoinIndex = CoinNumber - 1;

            int CoinDescIndexer = 0;
            foreach (CoinDesc CoinDescription in @Current.CoinDescs)
            {
                if (CoinDescription.PresentDescription != null)
                {
                    CurrentCoinDescs_Avalible.Add(CoinDescIndexer);
                }
                CoinDescIndexer++;
            }

            if (AutoSwitchToFirstCoinDesc) SwitchToCoinDesc(CurrentCoinDescs_Avalible[0]);
        }

        public static void SwitchToCoinDesc(int TargetCoinDescIndex)
        {
            {
                ManualTextLoadEvent = true;
            }

            TargetPreviewLayout = InterfaceObject<TMProEmitter>($"PreviewLayout_Skills_Coin{CurrentSkillCoinIndex + 1}_Desc{TargetCoinDescIndex + 1}");

            CurrentSkillCoinDescIndex = TargetCoinDescIndex;

            MainControl.TextEditor.Document = @Current.CoinDesc.DedicatedDocument;

            MainWindow.Skills_SwitchToCoinDesc_CheckAvalibles();

            {
                ManualTextLoadEvent = true;
            }
        }

        public static void Toggle7to10CoinsVisibility(bool Is)
        {
            for (int CoinButtonNumer = 7; CoinButtonNumer <= 10; CoinButtonNumer++)
            {
                InterfaceObject<Button>($"CoinSwitchButton_{CoinButtonNumer}").Visibility = Is ? Visible : Collapsed;
            }
        }
    }
}

// UI interactions
namespace LC_Localization_Task_Absolute
{
    public partial class MainWindow
    {
        private void SkillAffinitySelector_SelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
        {
            if (!ManualTextLoadEvent)
            {
                string Affinity = (SkillAffinitySelector.SelectedItem as StackPanel).Uid;
                Mode_Skills.@Current.Uptie.OptionalAffinity = Affinity != "None" ? Affinity : null;

                Mode_Skills.DeserializedInfo.SerializeToFormattedFile(CurrentFile.FullName);
            }
        }


        #region Skills switches
        private void Skills_ChangeSkillEGOAbnormalityName(object RequestSender, RoutedEventArgs EventArgs)
        {
            if (SWBT_Skills_EGOAbnormalitySkillName.Text != Mode_Skills.@Current.Uptie.EGOAbnormalityName)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    foreach (var Uptie in Mode_Skills.@Current.Skill) Uptie.Value.EGOAbnormalityName = SWBT_Skills_EGOAbnormalitySkillName.Text.Replace("\\n", "\n");
                }
                else
                {
                    Mode_Skills.@Current.Uptie.EGOAbnormalityName = SWBT_Skills_EGOAbnormalitySkillName.Text.Replace("\\n", "\n"); ;
                }

                Mode_Skills.DeserializedInfo.SerializeToFormattedFile(CurrentFile.FullName);
            }
        }


        private void Skills_SwitchToUptieLevel(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            int UptieLevelNumber = int.Parse((RequestSender as Button).Uid);
            if (!Mode_Skills.@Current.Skill.ContainsKey(UptieLevelNumber)) return;

            Mode_Skills.TransformToSkill(Mode_Skills.CurrentSkillID, UptieLevelNumber);
        }


        private void Skills_SwitchToMainDesc(object RequestSender, RoutedEventArgs EventArgs)
        {
            Mode_Skills.SwitchToMainDesc();

            if (LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch)
            {
                FastSwitch_ToSkillMainDesc__Highlight();
            }
        }

        private void Skills_SetCoinFocus(object RequestSender, RoutedEventArgs EventArgs)
        {
            string CoinNumber = (RequestSender as Button).Uid;
            if (CoinNumber == "") return;

            Mode_Skills.SetCoinFocus(int.Parse(CoinNumber));

            if (LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch)
            {
                TMProEmitter HighlightTarget = InterfaceObject<TMProEmitter>($"PreviewLayout_Skills_Coin{Mode_Skills.CurrentSkillCoinIndex + 1}_Desc{Mode_Skills.CurrentSkillCoinDescIndex + 1}");
                if (HighlightTarget != null)
                {
                    FastSwitch_ToSkillCoinDesc__Highlight(HighlightTarget);
                    HighlightTarget.Focus();
                }
            }
        }

        private void Skills_SwitchToPrevOrNextCoinDesc(object RequestSender, RoutedEventArgs EventArgs)
        {
            Button Sender = RequestSender as Button;

            string Direction = Sender.Name.Split("CoinDescSwitchButton_")[^1];
            int IndexOfCurrentCoinDesc = Mode_Skills.CurrentCoinDescs_Avalible.IndexOf(Mode_Skills.CurrentSkillCoinDescIndex);
            int TargetSwitchIndex = Direction == "Next" ? IndexOfCurrentCoinDesc + 1 : IndexOfCurrentCoinDesc - 1;

            Mode_Skills.SwitchToCoinDesc(TargetSwitchIndex);

            if (LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch)
            {
                TMProEmitter HighlightTarget = InterfaceObject<TMProEmitter>($"PreviewLayout_Skills_Coin{Mode_Skills.CurrentSkillCoinIndex + 1}_Desc{Mode_Skills.CurrentSkillCoinDescIndex + 1}");
                if (HighlightTarget != null)
                {
                    FastSwitch_ToSkillCoinDesc__Highlight(HighlightTarget);
                    HighlightTarget.Focus();
                }
            }
        }

        public static void Skills_SwitchToCoinDesc_CheckAvalibles()
        {
            int IndexOfCurrentCoinDesc = Mode_Skills.CurrentCoinDescs_Avalible.IndexOf(Mode_Skills.CurrentSkillCoinDescIndex);

            // If first item -> Hide 'Previous'
            MainControl.CoinDescSwitchButton_Prev.IsEnabled = IndexOfCurrentCoinDesc != 0;

            // If last item -> Hide 'Next'
            MainControl.CoinDescSwitchButton_Next.IsEnabled = (IndexOfCurrentCoinDesc + 1) < Mode_Skills.CurrentCoinDescs_Avalible.Count;
        }



        private void FastSwitch_ToSkillCoinDesc(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            if (!@CurrentPreviewCreator.IsActive)
            {
                TMProEmitter Sender = RequestSender as TMProEmitter;

                string CoinNumberText = Regex.Match(Sender.Name, @"PreviewLayout_Skills_Coin(\d+)_Desc\d+").Groups[1].Value;
                string CoinDescNumber = Regex.Match(Sender.Name, @"PreviewLayout_Skills_Coin\d+_Desc(\d+)").Groups[1].Value;

                Mode_Skills.SetCoinFocus(int.Parse(CoinNumberText), AutoSwitchToFirstCoinDesc: false);
                Mode_Skills.SwitchToCoinDesc(int.Parse(CoinDescNumber) - 1);

                if (LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnRightClick)
                {
                    FastSwitch_ToSkillCoinDesc__Highlight(Sender);
                }
            }
        }

        private void FastSwitch_ToSkillCoinDesc__Highlight(TMProEmitter TargetDesc)
        {
            TargetDesc.Background = ToSolidColorBrush("#FF262626");
            TargetDesc.Background.BeginAnimation(SolidColorBrush.OpacityProperty, new DoubleAnimation()
            {
                From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(0.4))
            });
        }

        private void FastSwitch_ToSkillMainDesc(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            Skills_SwitchToMainDesc(null, null);
            if (LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnRightClick)
            {
                FastSwitch_ToSkillMainDesc__Highlight();
            }
        }

        private void FastSwitch_ToSkillMainDesc__Highlight()
        {
            PreviewLayout_Skills_MainDesc.Background = ToSolidColorBrush("#FF262626");
            PreviewLayout_Skills_MainDesc.Background.BeginAnimation(SolidColorBrush.OpacityProperty, new DoubleAnimation()
            {
                From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(0.4))
            });
        }
        #endregion
    }
}