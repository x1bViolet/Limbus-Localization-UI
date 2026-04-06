using LCLocalizationInterface.LimbusRegistry.JsonTypes;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        #region Summary description
        private void JsonManaging_Passives_AddSummaryDesctiption(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.Passives.CurrentPassive.SummaryDescription = "";
            @EditorModesShelf.Passives.CurrentPassive.DedicatedDocument_SummaryDescription = LimbusEditorJsonObject.NewDedicatedDocument("", InputRichTextFormatter.RichTextFormat.Passives);

            @EditorModesShelf.Passives.SwitchToCurrentPassiveSummaryDescription();

            @EditorModesShelf.Passives.SaveCurrentFile_Action();
        }
        private void JsonManaging_Passives_RemoveSummaryDesctiption(object Sender, RoutedEventArgs Args)
        {
            string DialogUID = "[Main UI / Manual json files managing] * Passive summary — Remove <Confirm Dialog>";
            string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
            string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text");
            ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
            {
                @EditorModesShelf.Passives.SwitchToCurrentPassiveMainDescription();

                @EditorModesShelf.Passives.CurrentPassive.SummaryDescription = null;
                @EditorModesShelf.Passives.CurrentPassive.DedicatedDocument_SummaryDescription = null;

                @Languages.PresentedTextElements["[Passives / Right menu] * Passive summary"].SetDefaultText();

                @EditorModesShelf.Passives.SaveCurrentFile_Action();
            });
        }
        #endregion


        #region Flavor description
        private void JsonManaging_Passives_AddFlavorDesctiption(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.Passives.CurrentPassive.FlavorDescription = "";
            @EditorModesShelf.Passives.CurrentPassive.DedicatedDocument_FlavorDescription = LimbusEditorJsonObject.NewDedicatedDocument("", InputRichTextFormatter.RichTextFormat.None);

            @EditorModesShelf.Passives.SwitchToCurrentPassiveFlavorDescription();

            @EditorModesShelf.Passives.SaveCurrentFile_Action();
        }
        private void JsonManaging_Passives_RemoveFlavorDesctiption(object Sender, RoutedEventArgs Args)
        {
            string DialogUID = "[Main UI / Manual json files managing] * Passive flavor — Remove <Confirm Dialog>";
            string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
            string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text");
            ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
            {
                @EditorModesShelf.Passives.SwitchToCurrentPassiveMainDescription();

                @EditorModesShelf.Passives.CurrentPassive.FlavorDescription = null;
                @EditorModesShelf.Passives.CurrentPassive.DedicatedDocument_FlavorDescription = null;

                @Languages.PresentedTextElements["[Passives / Right menu] * Passive flavor"].SetDefaultText();

                @EditorModesShelf.Passives.SaveCurrentFile_Action();
            });
        }
        #endregion
    }
}