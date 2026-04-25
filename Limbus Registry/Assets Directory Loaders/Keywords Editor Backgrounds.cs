namespace LCLocalizationInterface.LimbusRegistry.AssetsDirectoryLoaders
{
    public static class KeywordsEditorBackgrounds
    {
        private static FileEventsNotifier BackgroundsImagesWatcher { get; } = new(TargetDirectory: @"[⇲] Assets Directory\Limbus Images\UI\Keywords Editor", FileFilters: ["BattleKeywords Background.png", "Bufs Background.png"])
        {
            EventsRaisingDelay = 250,
            GeneralHandler = (_, _, _) => KeywordsEditorBackgrounds.ReadAndSetImages()
        };
        public static void ReadAndSetImages()
        {
            #warning Idk its static ass somehow won't initialize and watch files until I refer to it directly (Although there are a lot of the same static FileEventsNotifiers with 0 references to them but they are initialized normally)
            _ = BackgroundsImagesWatcher;

            MainWindowInstance.RichTextViews__Keywords_COMPOSITION_Bufs_BackgroundImage.Source
                = BitmapFromFile(@"[⇲] Assets Directory\Limbus Images\UI\Keywords Editor\Bufs Background.png");

            MainWindowInstance.RichTextViews__Keywords_COMPOSITION_BattleKeywords_BackgroundImage.Source
                = BitmapFromFile(@"[⇲] Assets Directory\Limbus Images\UI\Keywords Editor\BattleKeywords Background.png");
        }
    }
}