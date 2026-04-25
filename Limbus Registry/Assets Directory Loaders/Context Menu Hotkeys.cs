namespace LCLocalizationInterface
{
    namespace LimbusRegistry.AssetsDirectoryLoaders
    {
        public static class ContextMenuHotkeys
        {
            private static FileEventsNotifier HotkeysFileWatcher { get; } = new(TargetFile: @"[⇲] Assets Directory\Context Menu Hotkeys.json")
            {
                GeneralHandler = (_, _, _) => ContextMenuHotkeys.ReadFile()
            };
            public static void ReadFile()
            {
                try
                {
                    if (new FileInfo(@"[⇲] Assets Directory\Context Menu Hotkeys.json").TryDeserealizeJsonAs(out HotkeysConfig Deserialized, out Exception Occurred))
                    {
                        LoadedHotkeys = Deserialized;
                    }
                    else
                    {
                        HandleError(Occurred);
                    }
                }
                catch (Exception Occurred)
                {
                    HandleError(Occurred);
                }


                static void HandleError(Exception Occurred)
                {
                    ErrorMessageWindow.ShowException(Occurred, @"Exception occurred while trying to read a hotkeys file (<b>[⇲] Assets Directory\Context Menu Hotkeys.json</b>)");

                    LoadedHotkeys.HotkeysList.Clear();
                }
            }


            public static HotkeysConfig LoadedHotkeys = new();

            public record HotkeysConfig
            {
                [JsonProperty("Hotkeys")]
                public List<Hotkey> HotkeysList { get; set; } = [];

                //[JsonProperty("Keyboard keys")]
                //public List<Key> KeybordKeysList => [.. Enum.GetValues(typeof(Key)).Cast<Key>()];

                public record Hotkey
                {
                    [JsonProperty("Command")]
                    public string? Command { get; set; }

                    [JsonProperty("Keys")]
                    public List<Key> Keys { get; set; } = [];
                }
            }
        }
    }
}