using ICSharpCode.AvalonEdit.Document;
using static LCLocalizationInterface.LimbusRegistry.InputRichTextFormatter;

namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    public record ObservationLog : LimbusEditorJsonObject, IHasIdentifier<BigInteger?>
    {
        [JsonProperty("id")]
        public BigInteger? ID { get; init; }

        [JsonProperty("codeName")]
        public string CodeName { get; set; } = "";

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("desc")]
        public string? Description { get; set; } // ?

        [JsonProperty("clue")]
        public string? Clue { get; set; } // ?



        [JsonIgnore] public bool IsCodeNameUnsaved => IsNotEquals(EscapeLineBreaks(CodeName), KeylessDedicatedDocument_CodeName.Text);
        [JsonIgnore] public bool IsNameUnsaved     => IsNotEquals(EscapeLineBreaks(Name),     KeylessDedicatedDocument_Name.Text);


        public void SyncCodeName() => CodeName = UnescapeLineBreaks(KeylessDedicatedDocument_CodeName.Text)!;
        public void SyncName()     => Name     = UnescapeLineBreaks(KeylessDedicatedDocument_Name.Text)!;


        [JsonIgnore] public required TextDocument KeylessDedicatedDocument_CodeName { get; set; }
        [JsonIgnore] public required TextDocument KeylessDedicatedDocument_Name     { get; set; }
        

        [JsonIgnore] public ObservableDictionary<int, ObservationStory>? ObservationStoriesDictionary { get; set; }


        public void ReOrderListByLevels() => StoryList = [.. StoryList!.OrderBy(x => x.Level)];


        [OnDeserialized]
        private void OnDeserialized(StreamingContext Context)
        {
            if (StoryList is not null && StoryList.Count > 0)
            {
                KeylessDedicatedDocument_CodeName = new TextDocument(EscapeLineBreaks(CodeName));
                KeylessDedicatedDocument_Name = new TextDocument(EscapeLineBreaks(Name));

                InitObservableNavigationDictionary();
            }
        }
        private void InitObservableNavigationDictionary()
        {
            ObservationStoriesDictionary = [];
            foreach (ObservationStory Story in StoryList!.Where(x => x.Level is not null))
            {
                ObservationStoriesDictionary[(int)Story.Level!] = Story;
            }
        }



        [JsonProperty("storyList")]
        public ObservableCollection<ObservationStory>? StoryList { get; set; }

        public record ObservationStory : LimbusEditorJsonObject
        {
            /// <summary>0 = "Lacking Data" description; 1, 2, and 3 is actual observation levels</summary>
            [JsonProperty("level")]
            public int? Level { get; init; }

            [JsonProperty("story")]
            public string Description { get; set; } = "";



            [JsonIgnore] public bool IsStoryUnsaved => IsNotEquals(Description, DedicatedDocument_Description.Text);


            public void SyncStoryDescription() => Description = AsLF(DedicatedDocument_Description.Text)!;


            [JsonIgnore] public required TextDocument DedicatedDocument_Description { get; set; }


            [OnDeserialized]
            private void OnDeserialized(StreamingContext Context)
            {
                DedicatedDocument_Description = NewDedicatedDocument(Text: Description, CarriedSyntaxKey: RichTextFormat.None);
            }




            public static ObservationStory CreateBlank(int InitialNumber)
            {
                return new()
                {
                    Level = InitialNumber,
                    Description = "",
                    DedicatedDocument_Description = NewDedicatedDocument("", RichTextFormat.None)
                };
            }
        }
        



        public static ObservationLog CreateBlank(BigInteger InitialID)
        {
            ObservationLog Created = new()
            {
                ID = InitialID,
                CodeName = "??-??-??-??",
                Name = "",
                KeylessDedicatedDocument_CodeName = new TextDocument(""),
                KeylessDedicatedDocument_Name = new TextDocument(""),
                StoryList =
                [
                    new ObservationLog.ObservationStory()
                    {
                        Level = 0,
                        DedicatedDocument_Description = NewDedicatedDocument(Text: "", CarriedSyntaxKey: RichTextFormat.None)
                    }
                ],
            };
            Created.InitObservableNavigationDictionary(); // Trigger ObservationStoriesDictionary creation

            return Created;
        }
    }




    public record PlainObservationLog : IHasIdentifier<BigInteger?>
    {
        [JsonProperty("id")]
        public BigInteger? ID { get; init; }

        [JsonProperty("codeName")]
        public string CodeName { get; set; } = "";

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("desc")]
        public string Description { get; set; } = "";

        [JsonProperty("clue")]
        public string? Clue { get; set; } = "Clue";



        [JsonProperty("storyList")]
        public List<ObservationStory>? StoryList { get; set; }

        public record ObservationStory
        {
            /// <inheritdoc cref="ObservationLog.ObservationStory.Level" />
            [JsonProperty("level")]
            public int? Level { get; set; }

            [JsonProperty("story")]
            public string Description { get; set; } = "";
        }
    }
}