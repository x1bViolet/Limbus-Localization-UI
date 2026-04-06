using ICSharpCode.AvalonEdit.Document;
using static LCLocalizationInterface.LimbusRegistry.InputRichTextFormatter;

namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    public record Passive : LimbusEditorJsonObject, IHasIdentifier<BigInteger?>
    {
        [JsonProperty("id")]
        public BigInteger? ID { get; init; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";


        [JsonProperty("desc")]
        public string MainDescription { get; set; } = "";

        [JsonProperty("summary")]
        public string? SummaryDescription { get; set; }

        [JsonProperty("undefined")]
        public string? Undefined { get; set; } // ?

        [JsonProperty("flavor")]
        public string? FlavorDescription { get; set; }



        [JsonIgnore] public bool IsNameUnsaved => IsNotEquals(EscapeLineBreaks(Name), KeylessDedicatedDocument_Name.Text);

        [JsonIgnore] public bool IsMainDescUnsaved    => IsNotEquals(MainDescription,    DedicatedDocument_MainDescription.Text);
        [JsonIgnore] public bool IsSummaryDescUnsaved => IsNotEquals(SummaryDescription, DedicatedDocument_SummaryDescription?.Text);
        [JsonIgnore] public bool IsFlavorDescUnsaved  => IsNotEquals(FlavorDescription,  DedicatedDocument_FlavorDescription?.Text);


        public void SyncName() => Name = UnescapeLineBreaks(KeylessDedicatedDocument_Name.Text)!;

        public void SyncMainDesc()    => MainDescription    = AsLF(DedicatedDocument_MainDescription.Text)!;
        public void SyncSummaryDesc() => SummaryDescription = AsLF(DedicatedDocument_SummaryDescription?.Text);
        public void SyncFlavorDesc()  => FlavorDescription  = AsLF(DedicatedDocument_FlavorDescription?.Text);


        [JsonIgnore] public required TextDocument KeylessDedicatedDocument_Name { get; set; }

        [JsonIgnore] public required TextDocument DedicatedDocument_MainDescription { get; set; }
        [JsonIgnore] public TextDocument? DedicatedDocument_SummaryDescription { get; set; }
        [JsonIgnore] public TextDocument? DedicatedDocument_FlavorDescription  { get; set; }


        [OnDeserialized]
        private void OnDeserialized(StreamingContext Context)
        {
            KeylessDedicatedDocument_Name = new TextDocument(EscapeLineBreaks(Name));
            
            DedicatedDocument_MainDescription = NewDedicatedDocument(Text: MainDescription, CarriedSyntaxKey: RichTextFormat.Passives);
            if (SummaryDescription is not null) DedicatedDocument_SummaryDescription = NewDedicatedDocument(Text: SummaryDescription, CarriedSyntaxKey: RichTextFormat.Passives);
            if (FlavorDescription is not null) DedicatedDocument_FlavorDescription  = NewDedicatedDocument(Text: FlavorDescription,  CarriedSyntaxKey: RichTextFormat.None);
        }
        


        public static Passive CreateBlank(BigInteger InitialID)
        {
            return new()
            {
                ID = InitialID,
                KeylessDedicatedDocument_Name = new TextDocument(""),
                DedicatedDocument_MainDescription = NewDedicatedDocument("", RichTextFormat.Passives),
            };
        }
    }




    public record PlainPassive : IHasIdentifier<BigInteger?>
    {
        [JsonProperty("id")]
        public BigInteger? ID { get; init; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("desc")]
        public string MainDescription { get; set; } = "";

        [JsonProperty("summary")]
        public string? SummaryDescription { get; set; }

        [JsonProperty("flavor")]
        public string? FlavorDescription { get; set; }
    }
}