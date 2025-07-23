using LC_Localization_Task_Absolute.Json;
using RichText;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
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

        internal protected static void TriggerSwitch(bool EnableUptieLevels, bool EnableEGOAbnormalityName)
        {
            MainControl.PreviewLayouts.Height = 383;
            MainControl.EditorWidthControl.Width = new GridLength(706.6);

            if (EnableEGOAbnormalityName)
            {
                MainControl.PreviewLayout_EGOSkills_Background.Visibility = Visibility.Visible;
                MainControl.NavigationPanel_HeightControlScrollViewer.MaxHeight = 403.5;
                SwitchedInterfaceProperties.DefaultValues.Height = 603.5;
            }
            else
            {
                MainControl.PreviewLayout_EGOSkills_Background.Visibility = Visibility.Collapsed;
                MainControl.NavigationPanel_HeightControlScrollViewer.MaxHeight = 370;
                SwitchedInterfaceProperties.DefaultValues.Height = 570;
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
            DeserializedInfo = JsonFile.Deserealize<Skills>() as Skills;
            InitializeSkillsDelegateFrom(DeserializedInfo);

            if (DelegateSkills_IDList.Count > 0)
            {
                Mode_Skills.TriggerSwitch(
                           EnableUptieLevels: JsonFile.Name.ContainsOneOf(["Skills_Ego_Personality-", "Skills_personality-", "Skills.json", "Skills_Ego.json"]),
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
                (MainControl.FindName($"NavigationPanel_Skills_UptieLevelSwitch_DisableCover_{UptieLevelNumber}") as Border).Visibility = Collapsed;
            }

            (MainControl.FindName($"NavigationPanel_Skills_UptieLevelSwitch_HighlightImage_{CurrentSkillUptieLevel}") as Image).Visibility = Visible;

            //////////////////////////////////////////////////
            var FullLink = DelegateSkills[CurrentSkillID][CurrentSkillUptieLevel];
            //////////////////////////////////////////////////
            MainControl.NavigationPanel_ObjectName_Display.Text = FullLink.Name;
            MainControl.SWBT_Skills_MainSkillName.Text = FullLink.Name.Replace("\n", "\\n");
            if (!FullLink.EGOAbnormalityName.IsNull())
            {
                MainControl.SWBT_Skills_EGOAbnormalitySkillName.Text = FullLink.EGOAbnormalityName.Replace("\n", "\\n");
            }

            Dictionary<dynamic, Visibility> VisibilityChangeQuery = [];

            if (!FullLink.Coins.IsNull())
            {
                int CoinNumber = 1;
                foreach(Coin CurrentCoin in FullLink.Coins)
                {
                    if (!CurrentCoin.CoinDescriptions.IsNull())
                    {
                        if (CurrentCoin.CoinDescriptions.Count() != 0)
                        {
                            if (CurrentCoin.CoinDescriptions.Where(x => x.Description.IsNull()).Count() != CurrentCoin.CoinDescriptions.Count())
                            {
                                int CoinDescNumber = 1;
                                Grid MainCoinPanel = MainControl.FindName($"PreviewLayout_Skills_Coin{CoinNumber}") as Grid;

                                if (!MainCoinPanel.IsNull())
                                {
                                    foreach(CoinDesc CoinDescription in CurrentCoin.CoinDescriptions)
                                    {
                                        if (!CoinDescription.Description.IsNull())
                                        {
                                            RichTextBox ThisCoinDescPanel = MainControl.FindName($"PreviewLayout_Skills_Coin{CoinNumber}_Desc{CoinDescNumber}") as RichTextBox;
                                            if (!ThisCoinDescPanel.IsNull())
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
                                        if (!MainControl.FindName($"STE_Skills_Coin_{CoinNumber}").IsNull())
                                        {
                                            (MainControl.FindName($"STE_Skills_Coin_{CoinNumber}") as RichTextBox)
                                                .SetRichText(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — Skill Coin {CoinNumber}"]);
                                        }
                                    }


                                    if (CurrentCoin.CoinDescriptions.Where(x => x.EditorDescription.EqualsOneOf(["", "<style=\"highlight\"></style>"])).Count() == CurrentCoin.CoinDescriptions.Count)
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
                if (!CoinDescription.Description.IsNull())
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
