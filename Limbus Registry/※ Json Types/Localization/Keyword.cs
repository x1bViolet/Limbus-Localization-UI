using ICSharpCode.AvalonEdit.Document;
using static LCLocalizationInterface.LimbusRegistry.InputRichTextFormatter;

namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    public record Keyword : LimbusEditorJsonObject, IHasIdentifier<string?>
    {
        [JsonProperty("id")]
        public string? ID { get; init; }


        [JsonProperty("iconId")]
        public object? IconID_1 { get; init; } // ?

        [JsonProperty("iconID")]
        public object? IconID_2 { get; init; } // ?


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


        /// <inheritdoc cref="PlainKeyword.Color"/>
        [JsonProperty("Color")]
        public string? Color { get; set; }



        [JsonIgnore] public bool IsNameUnsaved  => IsNotEquals(EscapeLineBreaks(Name),   KeylessDedicatedDocument_Name.Text);
        [JsonIgnore] public bool IsColorUnsaved => IsNotEquals(Color?.RemovePrefix("#"), KeylessDedicatedDocument_Color?.Text);

        [JsonIgnore] public bool IsMainDescUnsaved    => IsNotEquals(MainDescription,    DedicatedDocument_MainDescription.Text);
        [JsonIgnore] public bool IsSummaryDescUnsaved => IsNotEquals(SummaryDescription, DedicatedDocument_SummaryDescription?.Text);
        [JsonIgnore] public bool IsFlavorDescUnsaved  => IsNotEquals(FlavorDescription,  DedicatedDocument_FlavorDescription?.Text);


        public void SyncName()  => Name  = UnescapeLineBreaks(KeylessDedicatedDocument_Name.Text)!;
        public void SyncColor() => Color = KeylessDedicatedDocument_Color is not null ? $"#{KeylessDedicatedDocument_Color?.Text}" : null;

        public void SyncMainDesc()    => MainDescription    = AsLF(DedicatedDocument_MainDescription.Text)!;
        public void SyncFlavorDesc()  => FlavorDescription  = AsLF(DedicatedDocument_FlavorDescription?.Text);
        public void SyncSummaryDesc() => SummaryDescription = AsLF(DedicatedDocument_SummaryDescription?.Text);


        [JsonIgnore] public required TextDocument KeylessDedicatedDocument_Name { get; set; }
        [JsonIgnore] public TextDocument? KeylessDedicatedDocument_Color { get; set; }

        [JsonIgnore] public required TextDocument DedicatedDocument_MainDescription { get; set; }
        [JsonIgnore] public TextDocument? DedicatedDocument_SummaryDescription { get; set; }
        [JsonIgnore] public TextDocument? DedicatedDocument_FlavorDescription  { get; set; }


        [OnDeserialized]
        private void OnDeserialized(StreamingContext Context)
        {
            KeylessDedicatedDocument_Name = new TextDocument(EscapeLineBreaks(Name));

            DedicatedDocument_MainDescription = NewDedicatedDocument(Text: MainDescription, CarriedSyntaxKey: RichTextFormat.None);
            if (SummaryDescription is not null) DedicatedDocument_SummaryDescription = NewDedicatedDocument(Text: SummaryDescription, CarriedSyntaxKey: RichTextFormat.None);
            if (FlavorDescription is not null) DedicatedDocument_FlavorDescription  = NewDedicatedDocument(Text: FlavorDescription,  CarriedSyntaxKey: RichTextFormat.None);
            if (Color is not null)
            {
                Color = Color.Cut("\r", "\n");
                KeylessDedicatedDocument_Color = new TextDocument(Color.RemovePrefix("#"));
            }
        }
        


        public static Keyword CreateBlank(string InitialID)
        {
            return new()
            {
                ID = InitialID,
                KeylessDedicatedDocument_Name = new TextDocument(""),
                DedicatedDocument_MainDescription = NewDedicatedDocument("", RichTextFormat.None),
            };
        }
    }




    public record PlainKeyword : IHasIdentifier<string?>
    {
        [JsonProperty("id")]
        public string? ID { get; init; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("desc")]
        public string MainDescription { get; set; } = "";

        [JsonProperty("summary")]
        public string? SummaryDescription { get; set; }

        [JsonProperty("flavor")]
        public string? FlavorDescription { get; set; }


        /// <summary>Special option within the boundaries of this program, can be used instead of files from <c>"[⇲] Assets Directory\Color Dictionaries"</c></summary>
        [JsonProperty("Color")]
        public string? Color { get; set; }
    }
}