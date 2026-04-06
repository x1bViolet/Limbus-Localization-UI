using LCLocalizationInterface.Instruments.Classes;

namespace LCLocalizationInterface.LimbusRegistry.AssetsDirectoryLoaders
{
    public static class KeywordsMultipleMeaningsDictionary
    {
        private static DictionaryJson CurrentlyDeserializedData { get; set; } = new();
        private record DictionaryJson
        {
            public List<KeywordMultipleMeanings> Info { get; set; } = [];

            public record KeywordMultipleMeanings([JsonProperty("Keyword ID")] string KeywordID, [JsonProperty("Meanings")] List<string> Meanings);
        }



        private static FileEventsNotifier KeywordsMultipleMeaningsJsonFileWatcher { get; } = new()
        {
            GeneralHandler = (_, _, _) =>
            {
                ReadKeywordsMultipleMeanings();
                @PartialStateUpdater.Limbus.LimbusCustomLang.UpdateKeywords();
            }
        };
        private static string CurrentFilePath { get; set; } = @"";
        public static void SetSourceFile(string FilePath)
        {
            CurrentFilePath = FilePath;

            if (File.Exists(FilePath))
            {
                KeywordsMultipleMeaningsJsonFileWatcher.WatchFile(CurrentFilePath);
            }
            else
            {
                KeywordsMultipleMeaningsJsonFileWatcher.Reset();
            }

            ReadKeywordsMultipleMeanings();
        }



        public static void ReadKeywordsMultipleMeanings()
        {
            CurrentlyDeserializedData = new();

            if (File.Exists(CurrentFilePath))
            {
                if (new FileInfo(CurrentFilePath).TryDeserealizeJsonAs(out DictionaryJson Deserialized, out Exception Occurred))
                {
                    CurrentlyDeserializedData = Deserialized;
                }
                else
                {
                    HandleError(Occurred);
                }
            }
            else if (!string.IsNullOrWhiteSpace(CurrentFilePath))
            {
                HandleError(new FileNotFoundException("File not found"));
            }


            static void HandleError(Exception Occurred)
            {
                ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to read Keywords Multiple Meanings dictionary \"<b>{KeywordsMultipleMeaningsDictionary.CurrentFilePath}</b>\"");
            }
        }



        public static void ApplyOnImplicitConversionOrderDictionary()
        {
            foreach (DictionaryJson.KeywordMultipleMeanings KeywordMultipleMeanings in CurrentlyDeserializedData.Info.Where(x => !string.IsNullOrWhiteSpace(x.KeywordID) & x.Meanings.Count > 0))
            {
                foreach (string Meaning in KeywordMultipleMeanings.Meanings.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    KeywordsLoader.LimbusKeywords_ImplicitConversionOrder[Meaning] = KeywordMultipleMeanings.KeywordID;
                }
            }

            KeywordsLoader.LimbusKeywords_ImplicitConversionOrder = ReorderDictionaryByStringKeysLength(KeywordsLoader.LimbusKeywords_ImplicitConversionOrder);
        }
    }
}