using LCLocalizationInterface.LimbusRegistry.JsonTypes;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        #region Summary description
        private void JsonManaging_Keywords_AddSummaryDesctiption(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.Keywords.CurrentKeyword.SummaryDescription = "";
            @EditorModesShelf.Keywords.CurrentKeyword.DedicatedDocument_SummaryDescription = LimbusEditorJsonObject.NewDedicatedDocument("", InputRichTextFormatter.RichTextFormat.None);

            @EditorModesShelf.Keywords.SwitchToCurrentKeywordSummaryDescription();

            @EditorModesShelf.Keywords.SaveCurrentFile_Action();
        }
        private void JsonManaging_Keywords_RemoveSummaryDesctiption(object Sender, RoutedEventArgs Args)
        {
            string DialogUID = "[Main UI / Manual json files managing] * Keyword summary — Remove <Confirm Dialog>";
            string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
            string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text");
            ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
            {
                @EditorModesShelf.Keywords.SwitchToCurrentKeywordMainDescription();

                @EditorModesShelf.Keywords.CurrentKeyword.SummaryDescription = null;
                @EditorModesShelf.Keywords.CurrentKeyword.DedicatedDocument_SummaryDescription = null;

                @Languages.PresentedTextElements["[Keywords / Right Menu] * Keyword summary"].SetDefaultText();

                @EditorModesShelf.Keywords.SaveCurrentFile_Action();
            });
        }
        #endregion


        #region Flavor description
        private void JsonManaging_Keywords_AddFlavorDesctiption(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.Keywords.CurrentKeyword.FlavorDescription = "";
            @EditorModesShelf.Keywords.CurrentKeyword.DedicatedDocument_FlavorDescription = LimbusEditorJsonObject.NewDedicatedDocument("", InputRichTextFormatter.RichTextFormat.None);

            @EditorModesShelf.Keywords.SwitchToCurrentKeywordFlavorDescription();

            @EditorModesShelf.Keywords.SaveCurrentFile_Action();
        }
        private void JsonManaging_Keywords_RemoveFlavorDesctiption(object Sender, RoutedEventArgs Args)
        {
            string DialogUID = "[Main UI / Manual json files managing] * Keyword flavor — Remove <Confirm Dialog>";
            string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
            string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text");
            ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
            {
                @EditorModesShelf.Keywords.SwitchToCurrentKeywordMainDescription();

                @EditorModesShelf.Keywords.CurrentKeyword.FlavorDescription = null;
                @EditorModesShelf.Keywords.CurrentKeyword.DedicatedDocument_FlavorDescription = null;

                @Languages.PresentedTextElements["[Keywords / Right Menu] * Keyword flavor"].SetDefaultText();

                @EditorModesShelf.Keywords.SaveCurrentFile_Action();
            });
        }
        #endregion


        #region Template color
        private void JsonManaging_Keywords_AddTemplateColor(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.Keywords.CurrentKeyword.Color = "";
            @EditorModesShelf.Keywords.CurrentKeyword.KeylessDedicatedDocument_Color = new ICSharpCode.AvalonEdit.Document.TextDocument("");

            @EditorModesShelf.Keywords.SaveCurrentFile_Action();
        }
        private void JsonManaging_Keywords_RemoveTemplateColor(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.Keywords.CurrentKeyword.Color = null;
            @EditorModesShelf.Keywords.CurrentKeyword.KeylessDedicatedDocument_Color = null;

            @EditorModesShelf.Keywords.SaveCurrentFile_Action();
        }
        #endregion
    }
}