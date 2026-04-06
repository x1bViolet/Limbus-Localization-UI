using LCLocalizationInterface.LimbusRegistry.JsonTypes;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        private void JsonManaging_CreateFile(object Sender, RoutedEventArgs Args)
        {
            bool OfferToSaveNewLimbusLocalizationFile<LocalizationDataType>(LimbusLocalizationFile<LocalizationDataType> JsonToSave, string InitialName)
            {
                SaveFileDialog Saver = NewSaveFileDialog("Json files", ["json"], InitialName);
                if (Saver.ShowDialog() == true)
                {
                    JsonToSave.SerializeToFormattedJsonFile(Saver.FileName);
                    OpenJsonFile_Action(Saver.FileName);
                    return true;
                }
                else
                {
                    return false;
                }
            }


            Func<string, bool> ConfirmButtonAvailabilityCondition = delegate (string InputText) { return true; };
            Func<string, bool> ConfirmAction = delegate (string InputText) { return true; };
            string Tranz_KeywordsManualFileTypeForView = "";

            string SenderUID = (Sender as MenuItem_T1)!.HeaderText.UID;

            switch (SenderUID)
            {
                case "[Main UI / Create file context menu] * Skills":
                    ConfirmButtonAvailabilityCondition = delegate (string InputText)
                    {
                        return BigInteger.TryParse(InputText, out BigInteger TestID);
                    };
                    ConfirmAction = delegate (string InputText)
                    {
                        LimbusLocalizationFile<Skill> CreatedFile = new() { ManualFileType = "Skills", DataList = [Skill.CreateBlank(BigInteger.Parse(InputText))] };
                        return OfferToSaveNewLimbusLocalizationFile(JsonToSave: CreatedFile, InitialName: "Skills-….json");
                    };
                    break;


                case "[Main UI / Create file context menu] * Passives":
                    ConfirmButtonAvailabilityCondition = delegate (string InputText)
                    {
                        return BigInteger.TryParse(InputText, out BigInteger TestID);
                    };
                    ConfirmAction = delegate (string InputText)
                    {
                        LimbusLocalizationFile<Passive> CreatedFile = new() { ManualFileType = "Passives", DataList = [Passive.CreateBlank(BigInteger.Parse(InputText))] };
                        return OfferToSaveNewLimbusLocalizationFile(JsonToSave: CreatedFile, InitialName: "Passives-….json");
                    };
                    break;


                case "[Main UI / Create file context menu] * Keywords (Bufs)":
                    Tranz_KeywordsManualFileTypeForView = "Keywords (Bufs)";
                    goto case "Create Keywords file (Generalized)";

                case "[Main UI / Create file context menu] * Keywords (BattleKeywords)":
                    Tranz_KeywordsManualFileTypeForView = "Keywords (BattleKeywords)";
                    goto case "Create Keywords file (Generalized)";

                case "Create Keywords file (Generalized)":
                    ConfirmButtonAvailabilityCondition = delegate (string InputText)
                    {
                        return InputText.Matches(@"^\w+$");
                    };
                    ConfirmAction = delegate (string InputText)
                    {
                        LimbusLocalizationFile<Keyword> CreatedFile = new() { ManualFileType = Tranz_KeywordsManualFileTypeForView, DataList = [Keyword.CreateBlank(InputText)] };
                        return OfferToSaveNewLimbusLocalizationFile(JsonToSave: CreatedFile, InitialName: "Keywords-….json");
                    };
                    break;


                case "[Main UI / Create file context menu] * E.G.O Gifts":
                    ConfirmButtonAvailabilityCondition = delegate (string InputText)
                    {
                        return BigInteger.TryParse(InputText, out BigInteger TestID);
                    };
                    ConfirmAction = delegate (string InputText)
                    {
                        LimbusLocalizationFile<EGOGift> CreatedFile = new() { ManualFileType = "E.G.O Gifts", DataList = [EGOGift.CreateBlank(BigInteger.Parse(InputText))] };
                        return OfferToSaveNewLimbusLocalizationFile(JsonToSave: CreatedFile, InitialName: "E.G.O Gifts-….json");
                    };
                    break;


                case "[Main UI / Create file context menu] * Observation Logs":
                    ConfirmButtonAvailabilityCondition = delegate (string InputText)
                    {
                        return BigInteger.TryParse(InputText, out BigInteger TestID);
                    };
                    ConfirmAction = delegate (string InputText)
                    {
                        LimbusLocalizationFile<ObservationLog> CreatedFile = new() { ManualFileType = "Observation Logs", DataList = [ObservationLog.CreateBlank(BigInteger.Parse(InputText))] };
                        return OfferToSaveNewLimbusLocalizationFile(JsonToSave: CreatedFile, InitialName: "Observation Logs-….json");
                    };
                    break;
            }

            if (SenderUID != "[Main UI / Create file context menu] * Keywords") // Redirected from .. (Bufs) / .. (BattleKeywords)
            {
                string DialogTitle = @Languages.GetLocalizationTextFor("[Main UI / Manual json files managing] * New object ID <Input Dialog> — Title");
                InputDialog.InputDialogInstance.ShowInputDialog(DialogTitle, ConfirmButtonAvailabilityCondition, ConfirmAction);
            }
        }
    }
}