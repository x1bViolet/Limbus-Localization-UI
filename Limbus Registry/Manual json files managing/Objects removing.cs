namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        private void JsonManaging_RemoveThisObject(object Sender, RoutedEventArgs Args)
        {
            static int PredictAlternativeIDIndex<IDType>(int IndexOfCurrentID, List<IDType> AvailableIDsList)
            {
                return IndexOfCurrentID == 0 /* If first */
                    ? 0 // Then switch to first after removal
                    : IndexOfCurrentID == AvailableIDsList.Count - 1 /* If last */
                        ? AvailableIDsList.Count - 2 // Then switch to last after removal when ids count was decreased by one
                        : IndexOfCurrentID - 1; // Othwewise to the previous object
            }

            (string DialogUID, object DialogTextExtern, bool ObjectRemovalCondition, Action ConfirmAction) = ("", new object(), false, delegate () { });



            switch ((Sender as MenuItem_T1)!.HeaderText.UID.RemovePrefix("[Main UI / Manual json files managing] * ").RemovePostfix(" removal"))
            {
                // Remove affected from Deserialized and RouteDictionary -> switch to another


                case "Skill":
                    ObjectRemovalCondition = @EditorModesShelf.Skills.RouteDictionary_SkillObjects.Count > 1;
                    (DialogUID, DialogTextExtern) = ("[Main UI / Manual json files managing] * Skill removal <Confirm Dialog>", @EditorModesShelf.Skills.CurrentSkillID);
                    ConfirmAction = delegate ()
                    {
                        BigInteger SkillIDToRemove = @EditorModesShelf.Skills.CurrentSkillID;
                        int IndexOfCurrentID = @EditorModesShelf.Skills.AvailableIDsList.IndexOf(@EditorModesShelf.Skills.CurrentSkillID);
                        int AnotherSkillIndexToSwitchTo = PredictAlternativeIDIndex(IndexOfCurrentID, @EditorModesShelf.Skills.AvailableIDsList);

                        @EditorModesShelf.Skills.DeserializedLocalizationData!.DataList.Remove(@EditorModesShelf.Skills.CurrentSkill);
                        @EditorModesShelf.Skills.RouteDictionary_SkillObjects.Remove(SkillIDToRemove);
                        @EditorModesShelf.Skills.SaveCurrentFile_Action();

                        @EditorModesShelf.Skills.SwitchToSkill(@EditorModesShelf.Skills.AvailableIDsList[AnotherSkillIndexToSwitchTo]);
                    };
                    break;


                case "Passive":
                    ObjectRemovalCondition = @EditorModesShelf.Passives.RouteDictionary.Count > 1;
                    (DialogUID, DialogTextExtern) = ("[Main UI / Manual json files managing] * Passive removal <Confirm Dialog>", @EditorModesShelf.Passives.CurrentPassiveID);
                    ConfirmAction = delegate ()
                    {
                        BigInteger PassiveIDToRemove = @EditorModesShelf.Passives.CurrentPassiveID;
                        int IndexOfCurrentID = @EditorModesShelf.Passives.AvailableIDsList.IndexOf(@EditorModesShelf.Passives.CurrentPassiveID);
                        int AnotherPassiveIndexToSwitchTo = PredictAlternativeIDIndex(IndexOfCurrentID, @EditorModesShelf.Passives.AvailableIDsList);

                        @EditorModesShelf.Passives.DeserializedLocalizationData!.DataList.Remove(@EditorModesShelf.Passives.CurrentPassive);
                        @EditorModesShelf.Passives.RouteDictionary.Remove(PassiveIDToRemove);
                        @EditorModesShelf.Passives.SaveCurrentFile_Action();

                        @EditorModesShelf.Passives.SwitchToPassive(@EditorModesShelf.Passives.AvailableIDsList[AnotherPassiveIndexToSwitchTo]);
                    };
                    break;


                case "Keyword":
                    ObjectRemovalCondition = @EditorModesShelf.Keywords.RouteDictionary.Count > 1;
                    (DialogUID, DialogTextExtern) = ("[Main UI / Manual json files managing] * Keyword removal <Confirm Dialog>", @EditorModesShelf.Keywords.CurrentKeywordID);
                    ConfirmAction = delegate ()
                    {
                        String KeywordIDToRemove = @EditorModesShelf.Keywords.CurrentKeywordID;
                        int IndexOfCurrentID = @EditorModesShelf.Keywords.AvailableIDsList.IndexOf(@EditorModesShelf.Keywords.CurrentKeywordID);
                        int AnotherKeywordIndexToSwitchTo = PredictAlternativeIDIndex(IndexOfCurrentID, @EditorModesShelf.Keywords.AvailableIDsList);

                        @EditorModesShelf.Keywords.DeserializedLocalizationData!.DataList.Remove(@EditorModesShelf.Keywords.CurrentKeyword);
                        @EditorModesShelf.Keywords.RouteDictionary.Remove(KeywordIDToRemove);
                        @EditorModesShelf.Keywords.SaveCurrentFile_Action();

                        @EditorModesShelf.Keywords.SwitchToKeyword(@EditorModesShelf.Keywords.AvailableIDsList[AnotherKeywordIndexToSwitchTo]);
                    };
                    break;


                case "E.G.O Gift":
                    ObjectRemovalCondition = @EditorModesShelf.EGOGifts.RouteDictionary.Count > 1;
                    (DialogUID, DialogTextExtern) = ("[Main UI / Manual json files managing] * E.G.O Gift removal <Confirm Dialog>", @EditorModesShelf.EGOGifts.CurrentEGOGiftID);
                    ConfirmAction = delegate ()
                    {
                        BigInteger EGOGiftIDToRemove = @EditorModesShelf.EGOGifts.CurrentEGOGiftID;
                        int IndexOfCurrentID = @EditorModesShelf.EGOGifts.AvailableIDsList.IndexOf(@EditorModesShelf.EGOGifts.CurrentEGOGiftID);
                        int AnotherEGOGiftIndexToSwitchTo = PredictAlternativeIDIndex(IndexOfCurrentID, @EditorModesShelf.EGOGifts.AvailableIDsList);

                        @EditorModesShelf.EGOGifts.DeserializedLocalizationData!.DataList.Remove(@EditorModesShelf.EGOGifts.CurrentEGOGift);
                        @EditorModesShelf.EGOGifts.RouteDictionary.Remove(EGOGiftIDToRemove);
                        @EditorModesShelf.EGOGifts.UpdateInternalNavigationInfo(IsOverwriting: true);
                        @EditorModesShelf.EGOGifts.SaveCurrentFile_Action();

                        @EditorModesShelf.EGOGifts.SwitchToEGOGift(@EditorModesShelf.EGOGifts.AvailableIDsList[AnotherEGOGiftIndexToSwitchTo]);
                    };
                    break;


                case "Observation Log":
                    ObjectRemovalCondition = @EditorModesShelf.ObservationLogs.RouteDictionary_ObservationLogs.Count > 1;
                    (DialogUID, DialogTextExtern) = ("[Main UI / Manual json files managing] * Observation Log removal <Confirm Dialog>", @EditorModesShelf.ObservationLogs.CurrentObservationLogID);
                    ConfirmAction = delegate ()
                    {
                        BigInteger ObservationLogIDToRemove = @EditorModesShelf.ObservationLogs.CurrentObservationLogID;
                        int IndexOfCurrentID = @EditorModesShelf.ObservationLogs.AvailableIDsList.IndexOf(@EditorModesShelf.ObservationLogs.CurrentObservationLogID);
                        int AnotherObservationLogIndexToSwitchTo = PredictAlternativeIDIndex(IndexOfCurrentID, @EditorModesShelf.ObservationLogs.AvailableIDsList);

                        @EditorModesShelf.ObservationLogs.DeserializedLocalizationData!.DataList.Remove(@EditorModesShelf.ObservationLogs.CurrentObservationLog);
                        @EditorModesShelf.ObservationLogs.RouteDictionary_ObservationLogs.Remove(ObservationLogIDToRemove);
                        @EditorModesShelf.ObservationLogs.SaveCurrentFile_Action();

                        @EditorModesShelf.ObservationLogs.SwitchToObservationLog(@EditorModesShelf.ObservationLogs.AvailableIDsList[AnotherObservationLogIndexToSwitchTo]);
                    };
                    break;
            }


            if (ObjectRemovalCondition is true)
            {
                string Title = @Languages.GetLocalizationTextFor(DialogUID, "Title");
                string Text = @Languages.GetLocalizationTextFor(DialogUID, "Text").Extern(DialogTextExtern);

                ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(Title, Text, ConfirmAction);
            }
        }
    }
}