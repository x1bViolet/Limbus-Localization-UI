using ICSharpCode.AvalonEdit.Document;
using static LCLocalizationInterface.LimbusRegistry.InputRichTextFormatter;

namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    public record EGOGift : LimbusEditorJsonObject, IHasIdentifier<BigInteger?>
    {
        [JsonProperty("id")]
        public BigInteger? ID { get; init; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";


        [JsonProperty("desc")]
        public string MainDescription { get; set; } = "";

        [JsonProperty("flavor")]
        public string? FlavorDescription { get; set; }



        [JsonIgnore] public bool IsNameUnsaved => IsNotEquals(EscapeLineBreaks(Name), KeylessDedicatedDocument_Name.Text);
        
        [JsonIgnore] public bool IsMainDescUnsaved   => IsNotEquals(MainDescription,   DedicatedDocument_MainDescription.Text);
        [JsonIgnore] public bool IsFlavorDescUnsaved => IsNotEquals(FlavorDescription, DedicatedDocument_FlavorDescription?.Text);


        public void SyncName() => Name = UnescapeLineBreaks(KeylessDedicatedDocument_Name.Text)!;

        public void SyncMainDesc()    => MainDescription    = AsLF(DedicatedDocument_MainDescription.Text)!;
        public void SyncFlavorDesc()  => FlavorDescription  = AsLF(DedicatedDocument_FlavorDescription?.Text);


        [JsonIgnore] public required TextDocument KeylessDedicatedDocument_Name { get; set; }

        [JsonIgnore] public required TextDocument DedicatedDocument_MainDescription { get; set; }
        [JsonIgnore] public TextDocument? DedicatedDocument_FlavorDescription { get; set; }


        /// <summary>For navigation between upgrade levels</summary>
        [JsonIgnore] public int UpgradeLevel { get; set; } = 1;

        /// <summary>For navigation between upgrade levels</summary>
        [JsonIgnore] public List<BigInteger> UpgradeLevelsAssociativeIDs { get; set; } = [];


        [OnDeserialized]
        private void OnDeserialized(StreamingContext Context)
        {
            KeylessDedicatedDocument_Name = new TextDocument(EscapeLineBreaks(Name));

            DedicatedDocument_MainDescription = NewDedicatedDocument(Text: MainDescription, CarriedSyntaxKey: RichTextFormat.EGOGifts);
            if (FlavorDescription is not null) DedicatedDocument_FlavorDescription = NewDedicatedDocument(Text: FlavorDescription, CarriedSyntaxKey: RichTextFormat.None);
        }



        [JsonProperty("simpleDesc")]
        public ObservableCollection<SimpleDescription>? SimpleDescriptions { get; set; }

        public record SimpleDescription : LimbusEditorJsonObject, IHasIdentifier<BigInteger?>
        {
            /// <summary>This is "abilityID"</summary>
            [JsonProperty("abilityID")]
            public BigInteger? ID { get; init; }


            [JsonProperty("simpleDesc")]
            public string Description { get; set; } = "";



            [JsonIgnore] public bool IsDescUnsaved => IsNotEquals(Description, DedicatedDocument_Description.Text);

            public void SyncDesc() => Description = AsLF(DedicatedDocument_Description.Text)!;


            [JsonIgnore] public required TextDocument DedicatedDocument_Description { get; set; }


            [OnDeserialized]
            private void OnDeserialized(StreamingContext Context)
            {
                DedicatedDocument_Description = NewDedicatedDocument(Text: Description, CarriedSyntaxKey: RichTextFormat.EGOGifts);
            }
        }



        public static EGOGift CreateBlank(BigInteger InitialID)
        {
            return new()
            {
                ID = InitialID,
                KeylessDedicatedDocument_Name = new TextDocument(""),
                DedicatedDocument_MainDescription = NewDedicatedDocument(Text: "", CarriedSyntaxKey: RichTextFormat.EGOGifts)
            };
        }
    }




    public record PlainEGOGift : IHasIdentifier<BigInteger?>
    {
        [JsonProperty("id")]
        public BigInteger? ID { get; init; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";


        [JsonProperty("desc")]
        public string MainDescription { get; set; } = "";

        [JsonProperty("flavor")]
        public string? FlavorDescription { get; set; }



        [JsonProperty("simpleDesc")]
        public List<SimpleDescription>? SimpleDescriptions { get; set; }

        public record SimpleDescription : IHasIdentifier<BigInteger?>
        {
            /// <summary>This is "abilityID"</summary>
            [JsonProperty("abilityID")]
            public BigInteger? ID { get; init; }


            [JsonProperty("simpleDesc")]
            public string Description { get; set; } = "";
        }
    }
}