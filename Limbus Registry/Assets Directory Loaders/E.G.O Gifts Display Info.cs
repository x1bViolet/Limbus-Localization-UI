using LCLocalizationInterface.Instruments.Classes;

namespace LCLocalizationInterface.LimbusRegistry.AssetsDirectoryLoaders
{
    public static partial class ImageDictionaries
    {
        /// <summary>
        /// The structure of the E.G.O Gifts folder is different from the Keywords/Skill icons<br/>
        /// <c>[⇲] Assets Directory\Limbus Images\E.G.O Gifts\E.G.O Gifts Display Info.json</c> is responsible for ids
        /// </summary>
        public static ReadOnlyDictionary<BigInteger, EGOGiftsDisplayInfoFile.EGOGiftDetails> LoadedEGOGiftsDisplayInfo { get; private set; }
            = ReadOnlyDictionary<BigInteger, EGOGiftsDisplayInfoFile.EGOGiftDetails>.Empty;

        private static FileEventsNotifier EGOGiftsInfoWatcher { get; } = new(@"[⇲] Assets Directory\Limbus Images\E.G.O Gifts", ["*.png", "*.json"])
        {
            GeneralHandler = (_, _, _) => ReadEGOGiftsDisplayInfo()
        };

        public static void ReadEGOGiftsDisplayInfo()
        {
            static void UpdateDictionaryAndVisualView(Dictionary<BigInteger, EGOGiftsDisplayInfoFile.EGOGiftDetails> Source)
            {
                LoadedEGOGiftsDisplayInfo = new ReadOnlyDictionary<BigInteger, EGOGiftsDisplayInfoFile.EGOGiftDetails>(Source);

                if (@EditorModesShelf.EGOGifts.CurrentFile is not null)
                {
                    @EditorModesShelf.EGOGifts.UpdateEGOGiftVisualView();
                }
            }

            Dictionary<BigInteger, EGOGiftsDisplayInfoFile.EGOGiftDetails> NewlyReadedDisplayInfo = [];


            if (new FileInfo(@"[⇲] Assets Directory\Limbus Images\E.G.O Gifts\E.G.O Gifts Display Info.json").TryDeserealizeJsonAs(out EGOGiftsDisplayInfoFile DisplayInfo, out Exception Occurred))
            {
                foreach (EGOGiftsDisplayInfoFile.EGOGiftDetails EGOGift in DisplayInfo.List.Where(EGOGift => EGOGift.BaseID is not null))
                {
                    BigInteger BaseID     = (BigInteger)EGOGift.BaseID!;
                    BigInteger Upgrade2ID =  BigInteger.Parse($"1{BaseID}");
                    BigInteger Upgrade3ID =  BigInteger.Parse($"2{BaseID}");

                    NewlyReadedDisplayInfo[BaseID    ] = EGOGift;
                    NewlyReadedDisplayInfo[Upgrade2ID] = EGOGift;
                    NewlyReadedDisplayInfo[Upgrade3ID] = EGOGift;
                }

            }
            else
            {
                ErrorMessageWindow.ShowException(Occurred, @"Exception occurred while trying to read the E.G.O Gifts display info (<b>[⇲] Assets Directory\Limbus Images\E.G.O Gifts\E.G.O Gifts Display Info.json</b>)");
            }

            UpdateDictionaryAndVisualView(NewlyReadedDisplayInfo);
        }

        public record EGOGiftsDisplayInfoFile
        {
            [JsonProperty("E.G.O Gifts Info")]
            public List<EGOGiftDetails> List { get; set; } = [];
            public record EGOGiftDetails
            {
                [JsonProperty("ID")]
                public BigInteger? BaseID { get; init; }

                [JsonProperty("Image")]
                public string ImageName { get; set; } = "";

                [JsonProperty("Affinity")]
                public string Affinity { get; set; } = "None";

                [JsonProperty("Keyword")]
                public string Keyword { get; set; } = "-";

                [JsonProperty("Tier")]
                public string Tier { get; set; } = "-";

                public BitmapImage TryGetImage()
                {
                    string ExpectedPath = @$"[⇲] Assets Directory\Limbus Images\E.G.O Gifts\{ImageName}.png";

                    return File.Exists(ExpectedPath) ? BitmapFromFile(ExpectedPath) : ImageDictionaries.UnknownSpriteImage;
                }

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    if (!Affinity.EqualsToOneOf("Wrath", "Lust", "Sloth", "Gluttony", "Gloom", "Pride", "Envy", "None")) Affinity = "None";
                }
            }
        }
    }
}