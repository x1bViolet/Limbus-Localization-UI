using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using static LCLocalizationInterface.LimbusRegistry.EditorModesShelf.Types;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        private void JsonManaging_AppendAdditionalObject(object Sender, RoutedEventArgs Args)
        {
            Func<string, bool> ConfirmButtonAvailabilityCondition = delegate (string InputText) { return true; };
            Func<string, bool> ConfirmAction = delegate (string InputText) { return true; };

            switch (@EditorModesShelf.CurrentEditorMode.Identifier)
            {
                case EditorModeKey.Skills:
                    ConfirmButtonAvailabilityCondition = delegate (string InputText)
                    {
                        return BigInteger.TryParse(InputText, out BigInteger TestID) && @EditorModesShelf.Skills.AvailableIDsList.Contains(TestID) == false;
                    };
                    ConfirmAction = delegate (string InputText)
                    {
                        BigInteger NewSkillID = BigInteger.Parse(InputText);
                        Skill CreatedSkill = Skill.CreateBlank(NewSkillID);
                        @EditorModesShelf.Skills.DeserializedLocalizationData!.DataList.Add(CreatedSkill);
                        @EditorModesShelf.Skills.RouteDictionary_SkillObjects[NewSkillID] = CreatedSkill;
                        @EditorModesShelf.Skills.SaveCurrentFile_Action();

                        @EditorModesShelf.Skills.SwitchToSkill(NewSkillID);
                        return true;
                    };
                    break;


                case EditorModeKey.Passives:
                    ConfirmButtonAvailabilityCondition = delegate (string InputText)
                    {
                        return BigInteger.TryParse(InputText, out BigInteger TestID) && @EditorModesShelf.Passives.AvailableIDsList.Contains(TestID) == false;
                    };
                    ConfirmAction = delegate (string InputText)
                    {
                        BigInteger NewPassiveID = BigInteger.Parse(InputText);
                        Passive CreatedPassive = Passive.CreateBlank(NewPassiveID);
                        @EditorModesShelf.Passives.DeserializedLocalizationData!.DataList.Add(CreatedPassive);
                        @EditorModesShelf.Passives.RouteDictionary[NewPassiveID] = CreatedPassive;
                        @EditorModesShelf.Passives.SaveCurrentFile_Action();

                        @EditorModesShelf.Passives.SwitchToPassive(NewPassiveID);
                        return true;
                    };
                    break;


                case EditorModeKey.Keywords:
                    ConfirmButtonAvailabilityCondition = delegate (string InputText)
                    {
                        return InputText.Matches(@"^\w+$") && @EditorModesShelf.Keywords.AvailableIDsList.Contains(InputText) == false;
                    };
                    ConfirmAction = delegate (string InputText)
                    {
                        string NewKeywordID = InputText;
                        Keyword CreatedKeyword = Keyword.CreateBlank(NewKeywordID);
                        @EditorModesShelf.Keywords.DeserializedLocalizationData!.DataList.Add(CreatedKeyword);
                        @EditorModesShelf.Keywords.RouteDictionary[NewKeywordID] = CreatedKeyword;
                        @EditorModesShelf.Keywords.SaveCurrentFile_Action();

                        @EditorModesShelf.Keywords.SwitchToKeyword(NewKeywordID);
                        return true;
                    };
                    break;


                case EditorModeKey.EGOGifts:
                    ConfirmButtonAvailabilityCondition = delegate (string InputText)
                    {
                        return BigInteger.TryParse(InputText, out BigInteger TestID) && @EditorModesShelf.EGOGifts.AvailableIDsList.Contains(TestID) == false;
                    };
                    ConfirmAction = delegate (string InputText)
                    {
                        BigInteger NewEGOGiftID = BigInteger.Parse(InputText);
                        EGOGift CreatedEGOGift = EGOGift.CreateBlank(NewEGOGiftID);
                        @EditorModesShelf.EGOGifts.DeserializedLocalizationData!.DataList.Add(CreatedEGOGift);
                        @EditorModesShelf.EGOGifts.RouteDictionary[NewEGOGiftID] = CreatedEGOGift;
                        @EditorModesShelf.EGOGifts.UpdateInternalNavigationInfo(IsOverwriting: true);
                        @EditorModesShelf.EGOGifts.SaveCurrentFile_Action();

                        @EditorModesShelf.EGOGifts.SwitchToEGOGift(NewEGOGiftID);
                        return true;
                    };
                    break;


                case EditorModeKey.ObservationLogs:
                    ConfirmButtonAvailabilityCondition = delegate (string InputText)
                    {
                        return BigInteger.TryParse(InputText, out BigInteger TestID) && @EditorModesShelf.ObservationLogs.AvailableIDsList.Contains(TestID) == false;
                    };
                    ConfirmAction = delegate (string InputText)
                    {
                        BigInteger NewObservationLogID = BigInteger.Parse(InputText);
                        ObservationLog CreatedObservationLog = ObservationLog.CreateBlank(NewObservationLogID);
                        @EditorModesShelf.ObservationLogs.DeserializedLocalizationData!.DataList.Add(CreatedObservationLog);
                        @EditorModesShelf.ObservationLogs.RouteDictionary_ObservationLogs[NewObservationLogID] = CreatedObservationLog;
                        @EditorModesShelf.ObservationLogs.SaveCurrentFile_Action();

                        @EditorModesShelf.ObservationLogs.SwitchToObservationLog(NewObservationLogID);
                        return true;
                    };
                    break;
            }

            string DialogTitle = @Languages.GetLocalizationTextFor("[Main UI / Manual json files managing] * New object ID <Input Dialog> — Title");
            InputDialog.InputDialogInstance.ShowInputDialog(DialogTitle, ConfirmButtonAvailabilityCondition, ConfirmAction);
        }
    }
}