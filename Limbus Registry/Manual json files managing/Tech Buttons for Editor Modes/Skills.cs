using ICSharpCode.AvalonEdit.Document;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        #region Optional affinity
        public static bool ManuallyChangingSelectedOptionalAffinity = false;
        private void JsonManaging_SkillOptionalAffinitySelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
        {
            if (ManuallyChangingSelectedOptionalAffinity == false)
            {
                string Affinity = (SkillAffinitySelector.SelectedItem as StackPanel)!.Uid;
                @EditorModesShelf.Skills.CurrentUptie.OptionalAffinity = Affinity != "None" ? Affinity : null;
                @EditorModesShelf.Skills.SaveCurrentFile_Action();
            }
        }
        #endregion


        #region E.G.O Abnormality name
        private void JsonManaging_Skills_AddEGOAbnormalityName(object Sender, RoutedEventArgs Args)
        {
            if (@EditorModesShelf.Skills.CurrentUptie.EGOAbnormalityName is null)
            {
                @EditorModesShelf.Skills.CurrentUptie.EGOAbnormalityName = "";
                @EditorModesShelf.Skills.CurrentUptie.KeylessDedicatedDocument_EGOAbnormalityName = new TextDocument("");
                @EditorModesShelf.Skills.SaveCurrentFile_Action();
            }
        }
        private void JsonManaging_Skills_RemoveEGOAbnormalityName(object Sender, RoutedEventArgs Args)
        {
            if (@EditorModesShelf.Skills.CurrentUptie.EGOAbnormalityName is not null)
            {
                @EditorModesShelf.Skills.CurrentUptie.EGOAbnormalityName = null;
                @EditorModesShelf.Skills.CurrentUptie.KeylessDedicatedDocument_EGOAbnormalityName = null;
                @EditorModesShelf.Skills.SaveCurrentFile_Action();
            }
        }
        #endregion


        #region Add/Remove Uptie levels
        private void JsonManaging_Skills_AddUptie(object Sender, RoutedEventArgs Args)
        {
            int TargetUptieNumber = int.Parse((Sender as MenuItem_T1)!.Uid);
            if (@EditorModesShelf.Skills.RouteDictionary_UptieLevels[@EditorModesShelf.Skills.CurrentSkillID].ContainsKey(TargetUptieNumber) == false)
            {
                @EditorModesShelf.Skills.RouteDictionary_SkillObjects[@EditorModesShelf.Skills.CurrentSkillID].UptieLevels.Add(Skill.Uptie.CreateBlank(TargetUptieNumber));
                @EditorModesShelf.Skills.RouteDictionary_SkillObjects[@EditorModesShelf.Skills.CurrentSkillID].ReOrderUptiesByNumber();
                @EditorModesShelf.Skills.SwitchToSkill(@EditorModesShelf.Skills.CurrentSkillID, TargetUptieNumber);

                @EditorModesShelf.Skills.SaveCurrentFile_Action();
            }
        }
        private void JsonManaging_Skills_RemoveUptie(object Sender, RoutedEventArgs Args)
        {
            int TargetUptieNumber = int.Parse((Sender as MenuItem_T1)!.Uid);
            if (@EditorModesShelf.Skills.RouteDictionary_UptieLevels[@EditorModesShelf.Skills.CurrentSkillID].ContainsKey(TargetUptieNumber) == true)
            {
                if (@EditorModesShelf.Skills.CurrentSkill.UptieLevels.Count > 1)
                {
                    string DialogUID = "[Main UI / Manual json files managing] * Skill Uptie — Remove <Confirm Dialog>";
                    string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
                    string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text").Extern(TargetUptieNumber);
                    ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
                    {
                        @EditorModesShelf.Skills.CurrentSkill.UptieLevels.Remove(@EditorModesShelf.Skills.RouteDictionary_UptieLevels[@EditorModesShelf.Skills.CurrentSkillID][TargetUptieNumber]);
                        @EditorModesShelf.Skills.RouteDictionary_SkillObjects[@EditorModesShelf.Skills.CurrentSkillID].ReOrderUptiesByNumber();

                        if (TargetUptieNumber == @EditorModesShelf.Skills.CurrentUptieNumber)
                        {
                            @EditorModesShelf.Skills.SwitchToSkill(@EditorModesShelf.Skills.CurrentSkillID, (int)@EditorModesShelf.Skills.CurrentSkill.UptieLevels.First().UptieNumber!);
                        }
                        else
                        {
                            @EditorModesShelf.Skills.ReCheckRightMenuUptieButtonsAppearance();
                        }

                        @EditorModesShelf.Skills.SaveCurrentFile_Action();
                    });
                }
            }
        }
        #endregion


        #region Add/Remove Coins
        private void JsonManaging_Skills_AddCoin(object Sender, RoutedEventArgs Args)
        {
            Button CoinButton = (((Sender as MenuItem)!.Parent as ContextMenu)!.PlacementTarget as Button)!;

            int TargetCoinNumber = int.Parse(CoinButton.Uid);

            if (@EditorModesShelf.Skills.CurrentUptie.Coins is null) @EditorModesShelf.Skills.CurrentUptie.Coins = new();

            Skill.Uptie.Coin.CoinDesc CreatedCoinDesc = new()
            {
                DedicatedDocument_MainDescription = LimbusEditorJsonObject.NewDedicatedDocument("", InputRichTextFormatter.RichTextFormat.Skills)
            };

            if (TargetCoinNumber > @EditorModesShelf.Skills.CurrentUptie.Coins.Count) // Add empty Coins without descs before
            {
                for (int CoinNumber = @EditorModesShelf.Skills.CurrentUptie.Coins.Count; CoinNumber <= TargetCoinNumber - 1; CoinNumber++)
                {
                    @EditorModesShelf.Skills.CurrentUptie.Coins.Insert(CoinNumber, new Skill.Uptie.Coin() { CoinDescriptions = CoinNumber == TargetCoinNumber - 1 ? [CreatedCoinDesc] : null });
                }
            }
            else
            {
                @EditorModesShelf.Skills.CurrentUptie.Coins[TargetCoinNumber - 1].CoinDescriptions = [CreatedCoinDesc];
            }

            @EditorModesShelf.Skills.SwitchToCoinDescriptionMainDesc(TargetCoinNumber);
            @EditorModesShelf.Skills.SaveCurrentFile_Action();
        }
        private void JsonManaging_Skills_RemoveCoin(object Sender, RoutedEventArgs Args)
        {
            string NumberSource = Sender is MenuItem RemoveCoinOption ? ((RemoveCoinOption.Parent as ContextMenu)!.PlacementTarget as Button)!.Uid : (string)Sender;

            int TargetCoinNumber = int.Parse(NumberSource);

            string DialogUID = "[Main UI / Manual json files managing] * Skill coin — Remove <Confirm Dialog>";
            string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
            string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text").Extern(TargetCoinNumber);
            ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
            {
                if (TargetCoinNumber < @EditorModesShelf.Skills.CurrentUptie.Coins!.Count)
                {
                    @EditorModesShelf.Skills.CurrentUptie.Coins![TargetCoinNumber - 1].CoinDescriptions = null;
                }
                else
                {
                    @EditorModesShelf.Skills.CurrentUptie.Coins!.RemoveAt(TargetCoinNumber - 1);

                    @Languages.PresentedTextElements[$"[Skills / Right menu] * Skill Coin {TargetCoinNumber}"].SetDefaultText();

                    // Clear trailing Coins with empty descs
                    for (int CoinIndex = @EditorModesShelf.Skills.CurrentUptie.Coins!.Count - 1; CoinIndex >= 0; CoinIndex--)
                    {
                        if (@EditorModesShelf.Skills.CurrentUptie.Coins!.ElementAtOrDefault(CoinIndex)!.CoinDescriptions is null)
                        {
                            @EditorModesShelf.Skills.CurrentUptie.Coins!.RemoveAt(CoinIndex);
                        }
                        else
                        {
                            break; // If encountered Coin with desc
                        }
                    }
                }

                if (@EditorModesShelf.Skills.CurrentUptie.Coins!.Count == 0)
                {
                    @EditorModesShelf.Skills.CurrentUptie.Coins = null;
                }

                @EditorModesShelf.Skills.SwitchToCurrentSkillMainDescription();
                @EditorModesShelf.Skills.SaveCurrentFile_Action();
            });
        }
        #endregion


        #region Add/Remove Coin descriptions

        #region Add Coin main description
        private void JsonManaging_Skills_AddCoinDesctiption(object Sender, RoutedEventArgs Args)
        {
            Skill.Uptie.Coin.CoinDesc CreatedCoinDesc = new()
            {
                DedicatedDocument_MainDescription = LimbusEditorJsonObject.NewDedicatedDocument("", InputRichTextFormatter.RichTextFormat.Skills)
            };

            if (@EditorModesShelf.Skills.CurrentCoinDescriptionNumber + 1 <= 15)
            {
                @EditorModesShelf.Skills.CurrentCoin.CoinDescriptions!.Add(CreatedCoinDesc);
                @EditorModesShelf.Skills.SwitchToCoinDescriptionMainDesc(@EditorModesShelf.Skills.CurrentCoinNumber, @EditorModesShelf.Skills.CurrentCoinDescriptionNumber + 1);

                @EditorModesShelf.Skills.SaveCurrentFile_Action();

            }
        }
        private void JsonManaging_Skills_RemoveCoinDesctiption(object Sender, RoutedEventArgs Args)
        {
            if (@EditorModesShelf.Skills.CurrentCoin.CoinDescriptions!.Count == 1)
            {
                JsonManaging_Skills_RemoveCoin($"{@EditorModesShelf.Skills.CurrentCoinNumber}", null!);
            }
            else
            {
                string DialogUID = "[Main UI / Manual json files managing] * Skill coin description — Remove <Confirm Dialog>";
                string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
                string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text").Extern(@EditorModesShelf.Skills.CurrentCoinDescriptionNumber);
                ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
                {
                    int PreviousCoinDescriptionNumber = @EditorModesShelf.Skills.CurrentCoinDescriptionNumber - 1;

                    @EditorModesShelf.Skills.CurrentCoin.CoinDescriptions.RemoveAt(@EditorModesShelf.Skills.CurrentCoinDescriptionNumber - 1);
                    @EditorModesShelf.Skills.SwitchToCoinDescriptionMainDesc(@EditorModesShelf.Skills.CurrentCoinNumber, PreviousCoinDescriptionNumber);

                    @EditorModesShelf.Skills.SaveCurrentFile_Action();
                });
            }
        }
        #endregion

        #region Add summary for Coin description
        private void JsonManaging_Skills_AddCoinSummaryDesctiption(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.Skills.CurrentCoinDescription.SummaryDescription = "";
            @EditorModesShelf.Skills.CurrentCoinDescription.DedicatedDocument_SummaryDescription = LimbusEditorJsonObject.NewDedicatedDocument("", InputRichTextFormatter.RichTextFormat.Skills);
            MainWindowInstance.RightMenu_Skills_SwitchToCoinSummaryDesc.IsEnabled = true;

            @EditorModesShelf.Skills.SwitchToCurrentCoinDescriptionSummaryDesc();

            @EditorModesShelf.Skills.SaveCurrentFile_Action();
        }
        private void JsonManaging_Skills_RemoveCoinSummaryDesctiption(object Sender, RoutedEventArgs Args)
        {
            string DialogUID = "[Main UI / Manual json files managing] * Skill coin description summary — Remove <Confirm Dialog>";
            string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
            string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text");
            ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
            {
                @EditorModesShelf.Skills.SwitchToCoinDescriptionMainDesc(@EditorModesShelf.Skills.CurrentCoinNumber, @EditorModesShelf.Skills.CurrentCoinDescriptionNumber);

                @EditorModesShelf.Skills.CurrentCoinDescription.SummaryDescription = null;
                @EditorModesShelf.Skills.CurrentCoinDescription.DedicatedDocument_SummaryDescription = null;
                MainWindowInstance.RightMenu_Skills_SwitchToCoinSummaryDesc.IsEnabled = false;

                @EditorModesShelf.Skills.SaveCurrentFile_Action();
            });
        }
        #endregion

        #endregion


        #region Flavor description
        private void JsonManaging_Skills_AddFlavorDesctiption(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.Skills.CurrentUptie.FlavorDescription = "";
            @EditorModesShelf.Skills.CurrentUptie.DedicatedDocument_FlavorDescription = LimbusEditorJsonObject.NewDedicatedDocument("", InputRichTextFormatter.RichTextFormat.None);

            @EditorModesShelf.Skills.SwitchToCurrentSkillFlavorDescription();

            @EditorModesShelf.Skills.SaveCurrentFile_Action();
        }
        private void JsonManaging_Skills_RemoveFlavorDesctiption(object Sender, RoutedEventArgs Args)
        {
            string DialogUID = "[Main UI / Manual json files managing] * Skill flavor — Remove <Confirm Dialog>";
            string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
            string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text");
            ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
            {
                @EditorModesShelf.Skills.SwitchToCurrentSkillMainDescription();

                @EditorModesShelf.Skills.CurrentUptie.FlavorDescription = null;
                @EditorModesShelf.Skills.CurrentUptie.DedicatedDocument_FlavorDescription = null;

                @Languages.PresentedTextElements["[Skills / Right menu] * Skill flavor desc"].SetDefaultText();

                @EditorModesShelf.Skills.SaveCurrentFile_Action();
            });
        }
        #endregion
    }
}