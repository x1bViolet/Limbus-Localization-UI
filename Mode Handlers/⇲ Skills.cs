using LC_Localization_Task_Absolute.Json;
using Newtonsoft.Json;
using RichText;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.Custom_Skills_Constructor;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    internal abstract class Mode_Skills
    {
        internal protected static dynamic FormalTaskCompleted = null;

        internal protected static int CurrentSkillID = -1;
        internal protected static int CurrentSkillUptieLevel = -1;

        internal protected static int CurrentSkillCoinIndex = -1;

        internal protected static List<int> CurrentCoinDescs_Avalible = [];
        internal protected static int CurrentSkillCoinDescIndex = -1;

        internal protected static Skills DeserializedInfo;
        internal protected static Dictionary<string, int> Skills_NameIDs = [];


        internal protected static Dictionary<RichTextBox, string> LastPreviewUpdatesBank = [];


        internal protected static double LastRegisteredWidth = 0;

        internal protected static SwitchedInterfaceProperties SwitchedInterfaceProperties = new()
        {
            Key = "Skills",
            DefaultValues = new()
            {
                Height = 570,
                Width = 1000,
                MinHeight = 420,
                MinWidth = 708.8,
                MaxHeight = 10000,
                MaxWidth = 1000,
            },
        };


        internal protected static Dictionary<BigInteger, BaseTypes.Type_RawSkillsDisplayInfo.DetailedInfoItem> OrganizedDisplayInfo = new Dictionary<BigInteger, BaseTypes.Type_RawSkillsDisplayInfo.DetailedInfoItem>();
        
        internal protected static BitmapImage RegularCoinIcon = new BitmapImage(new Uri($"pack://application:,,,/UI/Limbus/Skills/Regular Coin.png"));
        internal protected static BitmapImage UnbreakableCoinIcon = new BitmapImage(new Uri($"pack://application:,,,/UI/Limbus/Skills/Unbreakable Coin.png"));
        internal protected static BitmapImage DefaultSkillFrameAlt = new BitmapImage(new Uri($"pack://application:,,,/UI/Limbus/Skills/Frames/Skill Default Frame alt.png"));

        internal protected static Dictionary<string, BitmapImage> AffinityIcons = new Dictionary<string, BitmapImage>();
        internal protected static Dictionary<string, BitmapImage> SkillFrames = new Dictionary<string, BitmapImage>();
        internal protected static Dictionary<string, BitmapImage> SkillIcons = new Dictionary<string, BitmapImage>();

        internal protected static Dictionary<string, BitmapImage> DefaultIcons = new Dictionary<string, BitmapImage>();

        // Read from file at moment or get one from list
        internal protected static BitmapImage AcquireSkillIcon(string ID)
        {
            if (SkillIcons.ContainsKey(ID)) return SkillIcons[ID];
            else
            {
                if (File.Exists(ID))
                {
                    // rin($"Returning from {ID}..");
                    return GenerateBitmapFromFile(ID);
                }
                else
                {
                    FileInfo IconFile = @"⇲ Assets Directory\[⇲] Limbus Images\Skills\Icons".GetFileWithName($"{ID}.png");
                    if (IconFile != null)
                    {
                        BitmapImage FoundedImage = GenerateBitmapFromFile(IconFile.FullName);
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

        internal protected static void LoadDefaultResources()
        {
            foreach (string Affinity in new List<string> { "Wrath", "Lust", "Sloth", "Gluttony", "Gloom", "Pride", "Envy" })
            {
                AffinityIcons[Affinity] = new BitmapImage(new Uri($"pack://application:,,,/UI/Limbus/Skills/Affinity Icons/{Affinity}.png"));

                for (int i = 1; i <= 3; i++)
                {
                    SkillFrames[$"{Affinity} {i}"] = new BitmapImage(new Uri($"pack://application:,,,/UI/Limbus/Skills/Frames/{Affinity}/{i}.png"));
                }
            }
            SkillFrames["None"] = new BitmapImage(new Uri($"pack://application:,,,/UI/Limbus/Skills/Frames/Skill Default Frame.png"));
        }

        internal protected static void LoadDisplayInfo()
        {
            foreach (string SkillType in new List<string> { "Attack", "Guard", "Evade", "Counter" })
            {
                DefaultIcons[$"{SkillType} None"] = GenerateBitmapFromFile(@$"⇲ Assets Directory\[⇲] Limbus Images\Skills\Icons\[⇲] Default\None\{SkillType}.png");
                
                foreach (string Affinity in new List<string> { "Wrath", "Lust", "Sloth", "Gluttony", "Gloom", "Pride", "Envy" })
                {
                    DefaultIcons[$"{SkillType} {Affinity}"] = GenerateBitmapFromFile(@$"⇲ Assets Directory\[⇲] Limbus Images\Skills\Icons\[⇲] Default\{Affinity}\{SkillType}.png");
                }
            }

            if (Directory.Exists(@"⇲ Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Raw Json"))
            {
                foreach(FileInfo SkillDataFile in new DirectoryInfo(@"⇲ Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Raw Json").GetFiles("*.json", SearchOption.AllDirectories))
                {
                    try
                    {
                        var Deserialized = SkillDataFile.Deserealize<BaseTypes.Type_RawSkillsDisplayInfo.SkillsDetailedInfo>();

                        if (Deserialized != null)
                        {
                            if (Deserialized.List != null)
                            {
                                foreach(BaseTypes.Type_RawSkillsDisplayInfo.DetailedInfoItem SkillData in Deserialized.List)
                                {
                                    if (SkillData.ID != null && SkillData.UptieLevelsDictionary != null)
                                    {
                                        try
                                        {
                                            OrganizedDisplayInfo[(BigInteger)SkillData.ID] = SkillData;
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
        }


        internal protected static void ChangeSkillHeaderReplicaAppearance()
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
                        var Info_Uptie = OrganizedDisplayInfo[CurrentSkillID].UptieLevelsDictionary[UptieInfoCheck];
                        var Info_Main = OrganizedDisplayInfo[CurrentSkillID];

                        // Affinity color and frame
                        if (Info_Uptie.Affinity_UPTIE != null)
                        {
                            MainControl.SkillReplicaAffinityColorHolder.Background = Info_Uptie.Affinity_UPTIE switch
                            {
                                "Wrath"    => ToSolidColorBrush("#fe0101"),
                                "Lust"     => ToSolidColorBrush("#fe6f01"),
                                "Sloth"    => ToSolidColorBrush("#edc427"),
                                "Gluttony" => ToSolidColorBrush("#a7fe01"),
                                "Gloom"    => ToSolidColorBrush("#1cc7f1"),
                                "Pride"    => ToSolidColorBrush("#014fd6"),
                                "Envy"     => ToSolidColorBrush("#9800df"),
                                _ => ToSolidColorBrush("#9f6a3a")
                            };

                            // Frame
                            string FrameName = $"{Info_Uptie.Affinity_UPTIE} {Info_Main.Rank}";
                            if (Info_Uptie.Affinity_UPTIE.Equals("None")) FrameName = "None";
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
                                Info_Main.SkillAction.Equals("Attack") &
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


                        // Manual coins insertion if they are null in localization files (Probably disabled currently)
                        bool ManualCoinInsertFromSkillData = true;
                        //ManualCoinInsertFromSkillData = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Coins == null;
                        //if (!ManualCoinInsertFromSkillData) // // If it was switched to FALSE due to coins list in localization isn't null
                        //{
                        //    // // Then check if coins count in localization is 0 to disagree
                        //    if (DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Coins.Count == 0)
                        //    {
                        //        ManualCoinInsertFromSkillData = true;
                        //    }
                        //}

                        if (ManualCoinInsertFromSkillData) // Final switch
                        {
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
                    }
                    catch (Exception ex) { rin(ex.ToString()); }
                }
                else
                {
                    if (OrganizedDisplayInfo[CurrentSkillID].UptieLevelsDictionary.Count == 0)
                    {
                        SwitchSkillViewReplicaToUnknown = true;
                    }
                }
            }


            else if (Custom_Skills_Constructor.LoadedSkillConstructors.ContainsKey(CurrentSkillID))
            {
                var Info_Main = LoadedSkillConstructors[CurrentSkillID];

                /// Main part


                // Resets
                MainControl.OffenseLevelIcon.Visibility = Collapsed;
                MainControl.DefenseLevelIcon.Visibility = Collapsed;
                MainControl.SkillCopiesGrid .Visibility = Collapsed;
                MainControl.DamageTypeGrid  .Visibility = Collapsed;

                MainControl.SkillDamageType_Blunt .Visibility = Collapsed;
                MainControl.SkillDamageType_Slash .Visibility = Collapsed;
                MainControl.SkillDamageType_Pierce.Visibility = Collapsed;

                // Attributes from Constructor, there
                MainControl.AttackWeightPanel.Visibility = Collapsed;
                MainControl.SkillAffinityIcon.Visibility = Collapsed;
                MainControl.SkillLevel.Visibility = Hidden;
                MainControl.SkillLevelTypeIcons.Visibility = Collapsed;
                MainControl.CoinPowerBackground.Visibility = Collapsed;

                // Attack Weight
                if (!Info_Main.Attributes.HideAttackWeight)
                {
                    MainControl.AttackWeightPanel.Visibility = Visible;
                    if (Info_Main.Characteristics.AttackWeight > 0)
                    {
                        MainControl.SkillAtkWeight.Text = new string('■', Info_Main.Characteristics.AttackWeight);
                    }
                    else
                    {
                        MainControl.SkillAtkWeight.Text = "■";
                    }
                }

                // Offense|Defense Level icons
                if (!Info_Main.Attributes.HideBaseLevel)
                {
                    MainControl.SkillLevel.Visibility = Visible;
                    MainControl.SkillLevelTypeIcons.Visibility = Visible;
                    if (Info_Main.Specific.Action.EqualsOneOf("Attack", "Counter"))
                    {
                        MainControl.OffenseLevelIcon.Visibility = Visible;
                    }
                    else
                    {
                        MainControl.DefenseLevelIcon.Visibility = Visible;
                    }
                }
                if (Info_Main.Specific.Action.EqualsOneOf("Attack", "Counter"))
                {
                    MainControl.DamageTypeGrid.Visibility = Visible;
                }

                // Copies icon
                if (!Info_Main.Attributes.HideSkillCopies)
                {
                    if (
                        Info_Main.Specific.Action.Equals("Attack") &
                        MainWindow.CurrentFile.Name.ContainsOneOf(["Skills.json", "Skills_personality-"])
                    )
                    {
                        MainControl.SkillCopiesGrid.Visibility = Visible;
                        if (Info_Main.Specific.Copies == null)
                        {
                            MainControl.SkillCopiesAmount.Text = Info_Main.Specific.Rank switch { 1 => "3", 2 => "2", 3 => "1", _ => "0" };
                        }
                        else
                        {
                            MainControl.SkillCopiesAmount.Text = $"{Info_Main.Specific.Copies}";
                        }
                    }
                }

                // Damage Type
                switch (Info_Main.Specific.DamageType)
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

                // Coins
                MainControl.SkillReplicaCoinsTab.Children.Clear();
                foreach (string CoinType in Info_Main.Characteristics.CoinsList)
                {
                    MainControl.SkillReplicaCoinsTab.Children.Add(CoinType switch
                    {
                        "Regular"     => new Image() { Source = RegularCoinIcon     },
                        "Unbreakable" => new Image() { Source = UnbreakableCoinIcon }
                    });
                }

                // Unobservable attribute
                if (Info_Main.Attributes.Unobservable)
                {
                    MainControl.SkillLevel.Visibility = Visible;
                    MainControl.SkillLevel.Text = "???";
                    MainControl.SkillLevel.TextDecorations = null;

                    MainControl.SkillCopiesGrid.Visibility = Collapsed;

                    MainControl.CoinPowerBackground.Visibility = Visible;
                    MainControl.CoinPowerValue.Text = $"{Info_Main.Characteristics.CoinsType}" + "?";

                    MainControl.BasePowerValue.Text = "?";
                }



















                /// Upties or Characteristics values select
                string Select_Affinity = "None";
                string Select_IconID = $"{CurrentSkillID}";
                string Select_CoinPower = "?";
                string Select_BasePower = "?";


                if (LoadedSkillConstructors[CurrentSkillID].Skill_Upties != null)
                {
                    if (LoadedSkillConstructors[CurrentSkillID].Skill_Upties.ContainsKey($"{CurrentSkillUptieLevel}"))
                    {
                        var Info_Uptie = LoadedSkillConstructors[CurrentSkillID].Skill_Upties[$"{CurrentSkillUptieLevel}"];

                        // Affinity color
                        Select_Affinity = Info_Uptie.Affinity;

                        // Another Icon
                        if (Info_Uptie.IconID != null) // If icon is alt from some other skill or _4 uptie
                        {
                            Select_IconID = Info_Uptie.IconID;
                        }

                        // No Unobservable attribute
                        if (!Info_Main.Attributes.Unobservable)
                        {
                            // Coin|Base Power values
                            if (Info_Uptie.CoinPower != null)
                            {
                                Select_CoinPower = $"{Info_Uptie.CoinPower}";
                            }
                            if (Info_Uptie.BasePower != null)
                            {
                                Select_BasePower = $"{Info_Uptie.BasePower}";
                            }
                        }
                    }
                    else
                    {
                        SwitchSkillViewReplicaToUnknown = true;
                    }
                }
                else // (If no upties attached)
                {
                    // Affinity color
                    Select_Affinity = Info_Main.Specific.Affinity;

                    // Icon
                    if (Info_Main.IconID != null) // If icon is alt from some other skill or _4 uptie
                    {
                        Select_IconID = Info_Main.IconID;
                    }

                    // No Unobservable attribute
                    if (!Info_Main.Attributes.Unobservable)
                    {
                        // Coin|Base Power values
                        Select_CoinPower = $"{Info_Main.Characteristics.CoinPower}";
                        Select_BasePower = $"{Info_Main.Characteristics.BasePower}";
                    }
                }







                // Affinity color
                MainControl.SkillReplicaAffinityColorHolder.Background = Select_Affinity switch
                {
                    "Wrath"    => ToSolidColorBrush("#fe0101"),
                    "Lust"     => ToSolidColorBrush("#fe6f01"),
                    "Sloth"    => ToSolidColorBrush("#edc427"),
                    "Gluttony" => ToSolidColorBrush("#a7fe01"),
                    "Gloom"    => ToSolidColorBrush("#1cc7f1"),
                    "Pride"    => ToSolidColorBrush("#014fd6"),
                    "Envy"     => ToSolidColorBrush("#9800df"),
                    _ => ToSolidColorBrush("#9f6a3a")
                };

                // Frame
                string FrameName = $"{Select_Affinity} {Info_Main.Specific.Rank}";
                if (Select_Affinity.Equals("None")) FrameName = "None";
                MainControl.SkilFrame.Source = SkillFrames[FrameName];

                // Skill Icon
                BitmapImage AcquiredImage = AcquireSkillIcon(Select_IconID);
                if (AcquiredImage == null & Select_Affinity != null)
                {
                    AcquiredImage = DefaultIcons[$"{Info_Main.Specific.Action} {Select_Affinity}"];
                }
                MainControl.SkilIcon.Source = AcquiredImage;

                // Affinity Icon
                if (Info_Main.Attributes.ShowAffinityIcon & !Select_Affinity.Equals("None"))
                {
                    MainControl.SkillAffinityIcon.Source = AffinityIcons[Select_Affinity];
                    MainControl.SkillAffinityIcon.Visibility = Visible;
                }

                // Unobservable attribute
                if (!Info_Main.Attributes.Unobservable)
                {
                    MainControl.SkillLevel.TextDecorations = TextDecorations.Underline;

                    // Level value
                    if (!Info_Main.Attributes.HideBaseLevel)
                    {
                        MainControl.SkillLevel.Visibility = Visible;

                        if (Info_Main.Attributes.OverrideBaseLevel == null)
                        {
                            MainControl.SkillLevel.Text = $"{Info_Main.Characteristics.BaseLevel + Info_Main.Characteristics.LevelCorrection}";
                        }
                        else
                        {
                            MainControl.SkillLevel.Text = Info_Main.Attributes.OverrideBaseLevel;
                        }
                    }

                    // Coin Power
                    if (!Info_Main.Attributes.HideCoinPower)
                    {
                        MainControl.CoinPowerBackground.Visibility = Visible;
                        if (Info_Main.Attributes.OverrideCoinPower == null)
                        {
                            MainControl.CoinPowerValue.Text =
                                $"{Info_Main.Characteristics.CoinsType}" +
                                $"{Select_CoinPower}";
                        }
                        else
                        {
                            MainControl.CoinPowerValue.Text = Info_Main.Attributes.OverrideCoinPower;
                        }
                    }
                    else
                    {
                        MainControl.CoinPowerValue.Text = "";
                    }

                    // Base Power
                    if (!Info_Main.Attributes.HideBasePower)
                    {
                        if (Info_Main.Attributes.OverrideBasePower == null)
                        {
                            MainControl.BasePowerValue.Text = $"{Select_BasePower}";
                        }
                        else
                        {
                            MainControl.BasePowerValue.Text = Info_Main.Attributes.OverrideBasePower;
                        }
                    }
                    else
                    {
                        MainControl.BasePowerValue.Text = "";
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

                MainControl.SkilIcon.Source = new BitmapImage();
                MainControl.SkilFrame.Source = DefaultSkillFrameAlt;

                MainControl.SkillReplicaCoinsTab.Children.Clear();
                MainControl.SkillReplicaCoinsTab.Children.Add(new Image() { Source = RegularCoinIcon }); // Add single coin
            }
        }


        internal protected static void CheckSkillNameReplicaCoins_FromLocalizationFile()
        {
            var FullLink = DelegateSkills[CurrentSkillID][CurrentSkillUptieLevel];
            if (FullLink.Coins != null)
            {
                MainControl.SkillReplicaCoinsTab.Children.Clear();

                int CoinsLimit = MainControl.CoinsStackPanel.Children.Count;
                for (int i = MainControl.CoinsStackPanel.Children.Count-1; i >= 0; i--)
                {
                    if (MainControl.CoinsStackPanel.Children[i].Visibility.Equals(Visible))
                    {
                        break;
                    }
                    else
                    {
                        CoinsLimit--;
                    }
                }

                int CoinAddCounter = 1;
                foreach (var CurrentCoin in FullLink.Coins)
                {
                    if (CoinAddCounter <= CoinsLimit)
                    {
                        bool AddedUnbreakable = false;
                        if (CurrentCoin.CoinDescriptions != null)
                        {
                            if (CurrentCoin.CoinDescriptions.Count > 0)
                            {
                                if (!CurrentCoin.CoinDescriptions.Where(x => x.Description == null).Equals(CurrentCoin.CoinDescriptions.Count))
                                {
                                    if (CurrentCoin.CoinDescriptions.Where(x => x.EditorDescription.StartsWith("[SuperCoin")).Any())
                                    {
                                        MainControl.SkillReplicaCoinsTab.Children.Add(new Image() { Source = UnbreakableCoinIcon });
                                        AddedUnbreakable = true;
                                    }
                                }
                            }
                        }
                        if (!AddedUnbreakable)
                        {
                            MainControl.SkillReplicaCoinsTab.Children.Add(new Image() { Source = RegularCoinIcon });
                        }
                        CoinAddCounter++;
                    }
                }
            }
            else
            {
                MainControl.SkillReplicaCoinsTab.Children.Add(new Image() { Source = RegularCoinIcon });
            }
        }




        internal protected static bool EnableUptieLevels_Recent = false;
        internal protected static bool EnableEGOAbnormalityName_Recent = false;
        internal protected static void TriggerSwitch(bool EnableUptieLevels, bool EnableEGOAbnormalityName)
        {
            EnableUptieLevels_Recent = EnableUptieLevels;
            EnableEGOAbnormalityName_Recent = EnableEGOAbnormalityName;

            MainControl.PreviewLayouts.Height = 383;
            MainControl.EditorWidthControl.Width = new GridLength(706.6);

            if (EnableEGOAbnormalityName)
            {
                MainControl.PreviewLayout_EGOSkills_Background.Visibility = Visibility.Visible;
                MainControl.NavigationPanel_HeightControlScrollViewer.MaxHeight = 403.5;
                SwitchedInterfaceProperties.DefaultValues.Height = 603.5;
                MainControl.SkillNameReplica_WidthRestrictor.MaxWidth = 390;
            }
            else
            {
                MainControl.PreviewLayout_EGOSkills_Background.Visibility = Visibility.Collapsed;
                MainControl.NavigationPanel_HeightControlScrollViewer.MaxHeight = 370;
                SwitchedInterfaceProperties.DefaultValues.Height = 570;
                MainControl.SkillNameReplica_WidthRestrictor.MaxWidth = 465;
            }

            LastRegisteredWidth = EnableEGOAbnormalityName ? 578 : 663;
            MainControl.PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = LastRegisteredWidth;

            if (EnableUptieLevels)
            {
                MainControl.NavigationPanel_Skills_UptieLevelSelectorGrid.Visibility = Visibility.Visible;
                MainControl.NavigationPanel_SwitchButtons.Margin = new Thickness(2, 168, 4, 4);
            }
            else
            {
                MainControl.NavigationPanel_Skills_UptieLevelSelectorGrid.Visibility = Visibility.Collapsed;
                MainControl.NavigationPanel_SwitchButtons.Margin = new Thickness(2, 114, 4, 4);
            }

            if (EnableEGOAbnormalityName)
            {
                MainControl.NavigationPanel_Skills_EGOAbnormalityName.Visibility = Visible;
            }
            else
            {
                MainControl.NavigationPanel_Skills_EGOAbnormalityName.Visibility = Collapsed;
            }


            MainWindow.PreviewUpdate_TargetSite = MainControl.PreviewLayout_Skills_MainDesc;

            Upstairs.ActiveProperties = SwitchedInterfaceProperties;

            AdjustUI(ActiveProperties.DefaultValues);

            HideNavigationPanelButtons(
                ExceptButtonsGrid  : MainControl.SwitchButtons_Skills,
                ExceptPreviewLayout: MainControl.PreviewLayoutGrid_Skills
            );
        }

        internal protected static Task LoadStructure(FileInfo JsonFile)
        {
            DeserializedInfo = JsonFile.Deserealize<Skills>();
            InitializeSkillsDelegateFrom(DeserializedInfo);

            if (DelegateSkills_IDList.Count > 0)
            {
                Mode_Skills.TriggerSwitch(
                           EnableUptieLevels: JsonFile.Name.ContainsOneOf(["Skills_Ego_Personality-", "Skills_personality-", "Skills.json", "Skills_Ego.json", "Skills_Assist.json"]),
                    EnableEGOAbnormalityName: JsonFile.Name.ContainsOneOf(["Skills_Ego_Personality-", "Skills_Ego.json"])
                );
                TransformToSkill(DelegateSkills_IDList[0]);
            }

            return FormalTaskCompleted;
        }

        internal protected static void TransformToSkill(int SkillID, int TranzUptieLevel = -1)
        {
            {
                ManualTextLoadEvent = true;
            }


            int SwitchingUptieLevel = TranzUptieLevel switch
            {
                -1 => DelegateSkills[SkillID].Keys.ToList()[0],
                _  => TranzUptieLevel
            };

            if (DelegateSkills[SkillID].Keys.Count == 5)
            {
                MainControl.SpecialFifthUptie.Visibility = Visible;
            }
            else
            {
                MainControl.SpecialFifthUptie.Visibility = Collapsed;
            }

            CurrentSkillID = SkillID;
            CurrentSkillUptieLevel = SwitchingUptieLevel;

            ResetSkillInfo();
            LastPreviewUpdatesBank.Clear();
            
            SwitchToDesc();

            if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Current ID Copy Button"))
            {
                MainControl.STE_NavigationPanel_ObjectID_Display
                    .SetRichText(UILanguageLoader.DynamicTypeElements["Right Menu — Current ID Copy Button"]
                    .Extern(CurrentSkillID));
            }

            MainWindow.NavigationPanel_IDSwitch_CheckAvalibles();


            if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Skill Coin Desc Number"))
            {
                MainControl.STE_Skills_Coin_DescNumberDisplay
                    .SetRichText(UILanguageLoader.DynamicTypeElements["Right Menu — Skill Coin Desc Number"]
                    .Extern(UILanguageLoader.LoadedLanguage.DefaultInsertionText));
            }


            foreach (int UptieLevelNumber in DelegateSkills[CurrentSkillID].Keys.ToList())
            {
                if (UptieLevelNumber <= 4)
                {
                    (MainControl.FindName($"NavigationPanel_Skills_UptieLevelSwitch_DisableCover_{UptieLevelNumber}") as Border).Visibility = Collapsed;
                }
            }

            if (CurrentSkillUptieLevel <= 4) (MainControl.FindName($"NavigationPanel_Skills_UptieLevelSwitch_HighlightImage_{CurrentSkillUptieLevel}") as Image).Visibility = Visible;

            //////////////////////////////////////////////////
            var FullLink = DelegateSkills[CurrentSkillID][CurrentSkillUptieLevel];
            //////////////////////////////////////////////////
            MainControl.NavigationPanel_ObjectName_Display.Text = FullLink.Name;
            MainControl.SWBT_Skills_MainSkillName.Text = FullLink.Name.Replace("\n", "\\n");
            if (FullLink.EGOAbnormalityName != null)
            {
                MainControl.SWBT_Skills_EGOAbnormalitySkillName.Text = FullLink.EGOAbnormalityName.Replace("\n", "\\n");
            }

            Dictionary<dynamic, Visibility> VisibilityChangeQuery = [];


            if (Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSkillNamesReplication)
            {
                try
                {
                    ChangeSkillHeaderReplicaAppearance();
                }
                catch (Exception ex) { rin(ex.ToString()); /*by thy way*/ }
            }


            if (FullLink.Coins != null)
            {
                int CoinNumber = 1;
                foreach(Coin CurrentCoin in FullLink.Coins)
                {
                    if (CurrentCoin.CoinDescriptions != null)
                    {
                        if (CurrentCoin.CoinDescriptions.Count() != 0)
                        {
                            if (CurrentCoin.CoinDescriptions.Where(x => x.Description == null).Count() != CurrentCoin.CoinDescriptions.Count())
                            {
                                int CoinDescNumber = 1;
                                Grid MainCoinPanel = MainControl.FindName($"PreviewLayout_Skills_Coin{CoinNumber}") as Grid;

                                if (MainCoinPanel != null)
                                {
                                    foreach(CoinDesc CoinDescription in CurrentCoin.CoinDescriptions)
                                    {
                                        if (CoinDescription.Description != null)
                                        {
                                            RichTextBox ThisCoinDescPanel = MainControl.FindName($"PreviewLayout_Skills_Coin{CoinNumber}_Desc{CoinDescNumber}") as RichTextBox;
                                            if (ThisCoinDescPanel != null)
                                            {
                                                Border RightMenuCoinButton_DisableCover = MainControl.FindName($"STE_DisableCover_Skills_Coin_{CoinNumber}") as Border;

                                                if (!CoinDescription.Description.Equals("")) VisibilityChangeQuery[ThisCoinDescPanel] = Visible;
                                                VisibilityChangeQuery[RightMenuCoinButton_DisableCover] = Collapsed;

                                                if (!CoinDescription.Description.Equals(CoinDescription.EditorDescription))
                                                {
                                                    ThisCoinDescPanel.SetLimbusRichText(CoinDescription.EditorDescription);
                                                    LastPreviewUpdatesBank[ThisCoinDescPanel] = CoinDescription.EditorDescription;

                                                    (MainControl.FindName($"STE_Skills_Coin_{CoinNumber}") as RichTextBox)
                                                        .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                                                        .Extern(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — Skill Coin {CoinNumber}"]));
                                                }
                                                else
                                                {
                                                    ThisCoinDescPanel.SetLimbusRichText(CoinDescription.Description);
                                                    LastPreviewUpdatesBank[ThisCoinDescPanel] = CoinDescription.Description;
                                                    (MainControl.FindName($"STE_Skills_Coin_{CoinNumber}") as RichTextBox)
                                                        .SetRichText(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — Skill Coin {CoinNumber}"]);
                                                }
                                            }
                                        }

                                        CoinDescNumber++;
                                    }
                                

                                    if (FullLink.Coins[CoinNumber - 1].CoinDescriptions
                                        .Where(x => x.Description != x.EditorDescription).Any())
                                    {
                                        (MainControl.FindName($"STE_Skills_Coin_{CoinNumber}") as RichTextBox)
                                            .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                                                .Extern(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — Skill Coin {CoinNumber}"]));
                                    }
                                    else
                                    {
                                        if (MainControl.FindName($"STE_Skills_Coin_{CoinNumber}") != null)
                                        {
                                            (MainControl.FindName($"STE_Skills_Coin_{CoinNumber}") as RichTextBox)
                                                .SetRichText(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — Skill Coin {CoinNumber}"]);
                                        }
                                    }


                                    if (CurrentCoin.CoinDescriptions.Where(x => x.EditorDescription.EqualsOneOf("", "<style=\"highlight\"></style>")).Count() == CurrentCoin.CoinDescriptions.Count)
                                    {
                                        MainCoinPanel.Visibility = Collapsed;
                                    }
                                    else
                                    {
                                        MainCoinPanel.Visibility = Visible;
                                    }
                                }
                            }
                        }
                    }

                    
                    CoinNumber++;
                }
            }

            foreach (KeyValuePair<dynamic, Visibility> VisibilityOrder in VisibilityChangeQuery)
            {
                VisibilityOrder.Key.Visibility = VisibilityOrder.Value;
            }


            {
                ManualTextLoadEvent = false;
            }
        }

        internal protected static void ResetSkillInfo()
        {
            for (int UptieLevelNumber = 1; UptieLevelNumber <= 4; UptieLevelNumber++)
            {
                (MainControl.FindName($"NavigationPanel_Skills_UptieLevelSwitch_HighlightImage_{UptieLevelNumber}") as Image).Visibility = Collapsed;
                (MainControl.FindName($"NavigationPanel_Skills_UptieLevelSwitch_DisableCover_{UptieLevelNumber}") as Border).Visibility = Visible;
            }

            for (int CoinNumber = 1; CoinNumber <= 6; CoinNumber++)
            {
                //Grid CoinPanel = MainControl.FindName($"PreviewLayout_Skills_Coin{CoinNumber}") as Grid;

                (MainControl.FindName($"STE_DisableCover_Skills_Coin_{CoinNumber}") as Border).Visibility = Visible;
                for (int CoinDescNumber = 1; CoinDescNumber <= 12; CoinDescNumber++)
                {
                    RichTextBox CoinDescPanel = MainControl.FindName($"PreviewLayout_Skills_Coin{CoinNumber}_Desc{CoinDescNumber}") as RichTextBox;
                    CoinDescPanel.Document.Blocks.Clear();
                    CoinDescPanel.Visibility = Collapsed;

                    RichTextBox CoinSwitchButtonText = (MainControl.FindName($"STE_Skills_Coin_{CoinNumber}") as RichTextBox);
                    CoinSwitchButtonText.SetRichText(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — Skill Coin {CoinNumber}"]);
                }

                (MainControl.FindName($"PreviewLayout_Skills_Coin{CoinNumber}") as Grid).Visibility = Collapsed;
            }
        }

        internal protected static void SetCoinFocus(int CoinNumber)
        {
            MainControl.NavigationPanel_Skills_CoinDesc_Previous_DisableCover.Visibility = Visible;
            MainControl.NavigationPanel_Skills_CoinDesc_Next_DisableCover.Visibility = Visible;
            MainControl.NavigationPanel_Skills_CoinDesc_Display_DisableCover.Visibility = Collapsed;

            if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Skill Coin Descs Title"))
            {
                MainControl.STE_CoinDescriptionsTitle
                    .SetRichText(UILanguageLoader.DynamicTypeElements["Right Menu — Skill Coin Descs Title"]
                    .Extern(CoinNumber));
            }

            CurrentCoinDescs_Avalible.Clear();
            CurrentSkillCoinIndex = CoinNumber - 1;

            int CoinDescIndexer = 0;
            foreach (CoinDesc CoinDescription in DelegateSkills[CurrentSkillID][CurrentSkillUptieLevel].Coins[CurrentSkillCoinIndex].CoinDescriptions)
            {
                if (CoinDescription.Description != null)
                {
                    CurrentCoinDescs_Avalible.Add(CoinDescIndexer);
                }
                CoinDescIndexer++;
            }

            CurrentSkillCoinDescIndex = CurrentCoinDescs_Avalible[0];

            SwitchToCoinDesc(CurrentSkillCoinDescIndex);
        }

        internal protected static void SwitchToCoinDesc(int CoinDescIndex, bool HighlightOnManualSwitch = false)
        {
            {
                ManualTextLoadEvent = true;
            }

            PreviewUpdate_TargetSite = MainControl.FindName($"PreviewLayout_Skills_Coin{CurrentSkillCoinIndex + 1}_Desc{CoinDescIndex + 1}") as RichTextBox;

            CurrentSkillCoinDescIndex = CoinDescIndex;

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var FullLink = DelegateSkills[CurrentSkillID][CurrentSkillUptieLevel].Coins[CurrentSkillCoinIndex].CoinDescriptions[CurrentSkillCoinDescIndex];
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (!FullLink.Description.Equals(FullLink.EditorDescription))
            {
                MainControl.Editor.Text = FullLink.EditorDescription;

                if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Skill Coin Desc Number"))
                {
                    MainControl.STE_Skills_Coin_DescNumberDisplay
                        .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                        .Extern(UILanguageLoader.DynamicTypeElements["Right Menu — Skill Coin Desc Number"].Extern($"{CoinDescIndex + 1}")));
                }
            }
            else
            {
                MainControl.Editor.Text = FullLink.Description;

                if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Skill Coin Desc Number"))
                {
                    MainControl.STE_Skills_Coin_DescNumberDisplay
                        .SetRichText(UILanguageLoader.DynamicTypeElements["Right Menu — Skill Coin Desc Number"]
                        .Extern($"{CoinDescIndex + 1}"));
                }
            }

            MainWindow.NavigationPanel_Skills_SwitchToCoinDesc_CheckAvalibles();

            LockEditorUndo();

            {
                ManualTextLoadEvent = true;
            }
        }

        internal protected static void SwitchToDesc()
        {
            {
                ManualTextLoadEvent = true;
            }

            MainControl.NavigationPanel_Skills_CoinDesc_Previous_DisableCover.Visibility = Visible;
            MainControl.NavigationPanel_Skills_CoinDesc_Next_DisableCover.Visibility = Visible;
            MainControl.NavigationPanel_Skills_CoinDesc_Display_DisableCover.Visibility = Visible;

            if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Skill Coin Descs Title"))
            {
                MainControl.STE_CoinDescriptionsTitle
                    .SetRichText(UILanguageLoader.DynamicTypeElements["Right Menu — Skill Coin Descs Title"]
                    .Extern(UILanguageLoader.LoadedLanguage.DefaultInsertionText));
            }

            PreviewUpdate_TargetSite = MainControl.PreviewLayout_Skills_MainDesc;

            /////////////////////////////////////////////////////////////////////
            var FullLink = DelegateSkills[CurrentSkillID][CurrentSkillUptieLevel];
            /////////////////////////////////////////////////////////////////////

            // ... -> MainWindow.Editor_TextChanged() -> update main desc
            if (!FullLink.Description.Equals(FullLink.EditorDescription))
            {
                MainControl.Editor.Text = FullLink.EditorDescription;

                LastPreviewUpdatesBank[MainControl.PreviewLayout_Skills_MainDesc] = FullLink.EditorDescription;
            }
            else
            {
                MainControl.Editor.Text = FullLink.Description;
                LastPreviewUpdatesBank[MainControl.PreviewLayout_Skills_MainDesc] = FullLink.Description;
            }

            if (MainControl.Editor.Text.Equals("")) PreviewUpdate_TargetSite.Visibility = Collapsed;

            if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Skill Coin Desc Number"))
            {
                MainControl.STE_Skills_Coin_DescNumberDisplay
                    .SetRichText(UILanguageLoader.DynamicTypeElements["Right Menu — Skill Coin Desc Number"]
                    .Extern(UILanguageLoader.LoadedLanguage.DefaultInsertionText));
            }

            LockEditorUndo();

            {
                ManualTextLoadEvent = false;
            }
        }
    }
}
