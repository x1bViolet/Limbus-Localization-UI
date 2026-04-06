using LCLocalizationInterface.LimbusRegistry.JsonTypes;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        #region Flavor description
        private void JsonManaging_EGOGifts_AddFlavorDesctiption(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.EGOGifts.CurrentEGOGift.FlavorDescription = "";
            @EditorModesShelf.EGOGifts.CurrentEGOGift.DedicatedDocument_FlavorDescription = LimbusEditorJsonObject.NewDedicatedDocument("", InputRichTextFormatter.RichTextFormat.None);

            @EditorModesShelf.EGOGifts.SwitchToCurrentEGOGiftFlavorDescription();

            @EditorModesShelf.EGOGifts.SaveCurrentFile_Action();
        }
        private void JsonManaging_EGOGifts_RemoveFlavorDesctiption(object Sender, RoutedEventArgs Args)
        {
            string DialogUID = "[Main UI / Manual json files managing] * E.G.O Gift flavor — Remove <Confirm Dialog>";
            string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
            string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text");
            ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
            {
                @EditorModesShelf.EGOGifts.SwitchToCurrentEGOGiftMainDescription();

                @EditorModesShelf.EGOGifts.CurrentEGOGift.FlavorDescription = null;
                @EditorModesShelf.EGOGifts.CurrentEGOGift.DedicatedDocument_FlavorDescription = null;

                @Languages.PresentedTextElements["[E.G.O Gifts / Right Menu] * E.G.O Gift Flavor"].SetDefaultText();

                @EditorModesShelf.EGOGifts.SaveCurrentFile_Action();
            });
        }
        #endregion


        #region Simple desc
        private void JsonManaging_EGOGifts_SimpleDescContextMenuOpening(object Sender, ContextMenuEventArgs Args)
        {
            // Available only when button number is count of simple descs + 1 (Add) or current count (Remove latest), not in the middle of
            int TargetSimpleDescNumber = int.Parse((Sender as Button)!.Uid);

            if (@EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions is null) // if no simple descs
            {
                Args.Handled = TargetSimpleDescNumber != 1; // Allow to add only first one if no simple descs
            }
            else // If simple descs are presented
            {
                int CurrentCountOfSimpleDescs = @EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions.Count;

                
                if (TargetSimpleDescNumber - 1 == CurrentCountOfSimpleDescs)
                {
                    // If button is disabled and right after latest enabled
                }
                else if (TargetSimpleDescNumber == CurrentCountOfSimpleDescs)
                {
                    // If button is enabled and latest
                }
                else
                {
                    Args.Handled = true;
                }
            }
        }

        private void JsonManaging_EGOGifts_AddSimpleDesctiption(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions ??= new ObservableCollection<EGOGift.SimpleDescription>();

            string AbilityIDPostfix = $"{@EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions!.Count}";
            if (AbilityIDPostfix == "0") AbilityIDPostfix = "";

            @EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions.Add(new EGOGift.SimpleDescription()
            {
                ID = BigInteger.Parse($"{@EditorModesShelf.EGOGifts.CurrentEGOGift.ID}{AbilityIDPostfix}"),
                DedicatedDocument_Description = LimbusEditorJsonObject.NewDedicatedDocument("", InputRichTextFormatter.RichTextFormat.EGOGifts)
            });

            @EditorModesShelf.EGOGifts.SwitchToCurrentEGOGiftSimpleDescription(@EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions.Count);

            @EditorModesShelf.EGOGifts.SaveCurrentFile_Action();
        }
        private void JsonManaging_EGOGifts_RemoveSimpleDesctiption(object Sender, RoutedEventArgs Args)
        {
            string DialogUID = "[Main UI / Manual json files managing] * E.G.O Gift simple desc — Remove <Confirm Dialog>";
            string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
            string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text").Extern(@EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions!.Count);
            ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
            {
                @EditorModesShelf.EGOGifts.SwitchToCurrentEGOGiftMainDescription();
                @EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions!.RemoveAt(@EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions!.Count - 1);

                if (@EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions!.Count == 0)
                {
                    @EditorModesShelf.EGOGifts.CurrentEGOGift.SimpleDescriptions = null;
                }

                @EditorModesShelf.EGOGifts.SaveCurrentFile_Action();
            });
        }
        #endregion
    }
}