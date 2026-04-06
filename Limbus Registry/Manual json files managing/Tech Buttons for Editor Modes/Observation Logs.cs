using LCLocalizationInterface.LimbusRegistry.JsonTypes;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        #region Observation Stories
        private void JsonManaging_ObservationLogs_AddObservationStory(object Sender, RoutedEventArgs Args)
        {
            int NewObservationStoryNumber = int.TryParse((((Sender as MenuItem_T1)!.Parent as PopupButton_T1)!.PlacementTarget as Button)!.Uid, out int Num) ? Num : 0; // "Lacking Data" (<-- 0) / "1" / "2" / "3" Button Uids

            ObservationLog.ObservationStory CreatedBlankStory = ObservationLog.ObservationStory.CreateBlank(NewObservationStoryNumber);

            @EditorModesShelf.ObservationLogs.CurrentObservationLog.StoryList!.Add(CreatedBlankStory);
            @EditorModesShelf.ObservationLogs.CurrentObservationLog.ObservationStoriesDictionary![NewObservationStoryNumber] = CreatedBlankStory;
            @EditorModesShelf.ObservationLogs.CurrentObservationLog.ReOrderListByLevels();

            @EditorModesShelf.ObservationLogs.SetSeparatorsVisibility();

            @EditorModesShelf.ObservationLogs.SwitchToObservationLogStory(NewObservationStoryNumber);

            @EditorModesShelf.ObservationLogs.SaveCurrentFile_Action();
        }
        private void JsonManaging_ObservationLogs_RemoveObservationStory(object Sender, RoutedEventArgs Args)
        {
            if (@EditorModesShelf.ObservationLogs.CurrentObservationLog.ObservationStoriesDictionary!.Count > 1)
            {
                int TargetObservationStoryNumber = int.TryParse((((Sender as MenuItem_T1)!.Parent as PopupButton_T1)!.PlacementTarget as Button)!.Uid, out int Num) ? Num : 0; // "Lacking Data" (<-- 0) / "1" / "2" / "3" Button Uids

                string DialogUID = "[Main UI / Manual json files managing] * Observation Log Story — Remove <Confirm Dialog>";
                string DialogTitle = @Languages.GetLocalizationTextFor(DialogUID, "Title");
                string DialogText = @Languages.GetLocalizationTextFor(DialogUID, "Text")
                    .Extern(TargetObservationStoryNumber + 1);

                ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
                {
                    List<int> AvailableObservationStoriesLeft =
                    [.. @EditorModesShelf.ObservationLogs.CurrentObservationLog.ObservationStoriesDictionary.Keys.Where(x => x != TargetObservationStoryNumber)];

                    ObservationLog.ObservationStory StoryToRemove = @EditorModesShelf.ObservationLogs.CurrentObservationLog.ObservationStoriesDictionary[TargetObservationStoryNumber];

                    @EditorModesShelf.ObservationLogs.SwitchToObservationLogStory(AvailableObservationStoriesLeft.First());

                    @EditorModesShelf.ObservationLogs.CurrentObservationLog.StoryList!.Remove(StoryToRemove);
                    @EditorModesShelf.ObservationLogs.CurrentObservationLog.ObservationStoriesDictionary!.Remove((int)StoryToRemove.Level!);
                    @EditorModesShelf.ObservationLogs.CurrentObservationLog.ReOrderListByLevels();


                    @EditorModesShelf.ObservationLogs.SetSeparatorsVisibility();
                    @EditorModesShelf.ObservationLogs.ChangeAllRightMenuUnsavedChangesMarkers_OnButtons();

                    @EditorModesShelf.ObservationLogs.SaveCurrentFile_Action();
                });
            }
        }
        #endregion
    }
}