using ICSharpCode.AvalonEdit.Document;
using static LCLocalizationInterface.LimbusRegistry.InputRichTextFormatter;

namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    public record Skill : LimbusEditorJsonObject, IHasIdentifier<BigInteger?>
    {
        [JsonProperty("id")]
        public BigInteger? ID { get; init; }


        public void ReOrderUptiesByNumber() => UptieLevels = [.. UptieLevels.OrderBy(Uptie => Uptie.UptieNumber)];


        [JsonProperty("levelList")]
        public ObservableCollection<Uptie> UptieLevels { get; set; } = [];

        public record Uptie : LimbusEditorJsonObject
        {
            [JsonProperty("Affinity")]
            public string? OptionalAffinity { get; set; }


            [JsonProperty("level")]
            public int? UptieNumber { get; init; }


            [JsonProperty("name")]
            public string Name { get; set; } = "";

            [JsonProperty("abName")]
            public string? EGOAbnormalityName { get; set; }


            [JsonProperty("keywords")]
            public List<string>? Keywords { get; set; }


            [JsonProperty("desc")]
            public string MainDescription { get; set; } = "";

            [JsonProperty("flavor")]
            public string? FlavorDescription { get; set; }



            [JsonIgnore] public bool IsMainNameUnsaved           => IsNotEquals(EscapeLineBreaks(Name), KeylessDedicatedDocument_MainName.Text);
            [JsonIgnore] public bool IsEGOAbnormalityNameUnsaved => IsNotEquals(EscapeLineBreaks(EGOAbnormalityName), KeylessDedicatedDocument_EGOAbnormalityName?.Text);

            [JsonIgnore] public bool IsMainDescUnsaved   => IsNotEquals(MainDescription, DedicatedDocument_MainDescription.Text);
            [JsonIgnore] public bool IsFlavorDescUnsaved => IsNotEquals(FlavorDescription, DedicatedDocument_FlavorDescription?.Text);


            public void SyncMainName()           => Name               = UnescapeLineBreaks(AsLF(KeylessDedicatedDocument_MainName.Text)!)!;
            public void SyncEGOAbnormalityName() => EGOAbnormalityName = UnescapeLineBreaks(AsLF(KeylessDedicatedDocument_EGOAbnormalityName?.Text));

            public void SyncMainDesc()   => MainDescription   = AsLF(DedicatedDocument_MainDescription.Text)!;
            public void SyncFlavorDesc() => FlavorDescription = AsLF(DedicatedDocument_FlavorDescription?.Text);


            [JsonIgnore] public required TextDocument KeylessDedicatedDocument_MainName { get; set; }
            [JsonIgnore] public TextDocument? KeylessDedicatedDocument_EGOAbnormalityName { get; set; }

            [JsonIgnore] public required TextDocument DedicatedDocument_MainDescription { get; set; }
            [JsonIgnore] public TextDocument? DedicatedDocument_FlavorDescription  { get; set; }


            [OnDeserialized]
            private void OnDeserialized(StreamingContext Context)
            {
                Keywords = null; // Remove, unused

                KeylessDedicatedDocument_MainName = new TextDocument(EscapeLineBreaks(Name));
                if (EGOAbnormalityName is not null) KeylessDedicatedDocument_EGOAbnormalityName = new TextDocument(EscapeLineBreaks(EGOAbnormalityName));

                DedicatedDocument_MainDescription = NewDedicatedDocument(Text: MainDescription, CarriedSyntaxKey: RichTextFormat.Skills);
                if (FlavorDescription is not null) DedicatedDocument_FlavorDescription = NewDedicatedDocument(Text: FlavorDescription, CarriedSyntaxKey: RichTextFormat.None);
            }



            [JsonProperty("coinlist")]
            public ObservableCollection<Coin>? Coins { get; set; }

            public record Coin : LimbusEditorJsonObject
            {
                [JsonProperty("coindescs")]
                public ObservableCollection<CoinDesc>? CoinDescriptions { get; set; }

                public record CoinDesc : LimbusEditorJsonObject
                {
                    [JsonProperty("desc")]
                    public string MainDescription { get; set; } = "";
                    
                    [JsonProperty("summary")]
                    public string? SummaryDescription { get; set; }



                    [JsonIgnore] public bool IsMainDescUnsaved    => IsNotEquals(MainDescription, DedicatedDocument_MainDescription.Text);
                    [JsonIgnore] public bool IsSummaryDescUnsaved => IsNotEquals(SummaryDescription, DedicatedDocument_SummaryDescription?.Text);

                    public void SyncMainDesc()    => MainDescription    = AsLF(DedicatedDocument_MainDescription.Text)!;
                    public void SyncSummaryDesc() => SummaryDescription = AsLF(DedicatedDocument_SummaryDescription!.Text);


                    [JsonIgnore] public required TextDocument DedicatedDocument_MainDescription { get; set; }
                    [JsonIgnore] public TextDocument? DedicatedDocument_SummaryDescription { get; set; }


                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        DedicatedDocument_MainDescription = NewDedicatedDocument(Text: MainDescription, CarriedSyntaxKey: RichTextFormat.Skills);
                        if (SummaryDescription is not null) DedicatedDocument_SummaryDescription = NewDedicatedDocument(Text: SummaryDescription, CarriedSyntaxKey: RichTextFormat.Skills);
                    }
                }
            }



            public static Uptie CreateBlank(int InitialUptieNumber)
            {
                return new()
                {
                    UptieNumber = InitialUptieNumber,
                    KeylessDedicatedDocument_MainName = new TextDocument(""),
                    DedicatedDocument_MainDescription = NewDedicatedDocument("", RichTextFormat.Skills)
                };
            }
        }
        
        

        public static Skill CreateBlank(BigInteger InitialID)
        {
            return new()
            {
                ID = InitialID,
                UptieLevels = [Uptie.CreateBlank(InitialUptieNumber: 1)]
            };
        }
    }




    public record PlainSkill : IHasIdentifier<BigInteger?>
    {
        [JsonProperty("id")]
        public BigInteger? ID { get; init; }

        [JsonProperty("levelList")]
        public List<UptieLevel> UptieLevels { get; set; } = [];

        public record UptieLevel
        {
            [JsonProperty("Affinity")]
            public string? OptionalAffinity { get; set; }


            [JsonProperty("level")]
            public int? UptieNumber { get; set; }


            [JsonProperty("name")]
            public string Name { get; set; } = "";

            [JsonProperty("abName")]
            public string? EGOAbnormalityName { get; set; }


            [JsonProperty("desc")]
            public string MainDescription { get; set; } = "";

            [JsonProperty("flavor")]
            public string? FlavorDescription { get; set; }



            [JsonProperty("coinlist")]
            public List<Coin>? Coins { get; set; }

            public record Coin
            {
                [JsonProperty("coindescs")]
                public List<CoinDesc>? CoinDescriptions { get; set; }

                public record CoinDesc
                {
                    [JsonProperty("desc")]
                    public string MainDescription { get; set; } = "";

                    [JsonProperty("summary")]
                    public string? SummaryDescription { get; set; }
                }
            }
        }
    }
}