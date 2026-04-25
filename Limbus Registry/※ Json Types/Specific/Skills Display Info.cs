namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    namespace Specific
    {
        public record SkillsDisplayInfoJson : Explicit
        {
            [JsonProperty("Skills Info")]
            public ObservableCollection<SkillConstructor> List { get; set; } = [];
        }

        public record SkillConstructor : Explicit, IHasIdentifier<BigInteger?>
        {
            [JsonIgnore] private const string RelativeMarker = ":Current-Directory:";


            [JsonProperty("(Name)")]
            public string SkillName { get; set; } = "";


            [JsonProperty("ID")]
            public BigInteger? ID { get; set; }


            [JsonProperty("Icon ID")]
            public string IconID { get; set; } = "";


            [JsonProperty("Specific")]
            public Specific_PROP Specific { get; set; } = new();
            public record Specific_PROP : Explicit
            {
                [JsonProperty("Damage Type"), JsonConverter(typeof(InvalidEnumLiteralResolver<DamageType>), DamageType.None)]
                public DamageType DamageType { get; set; } = DamageType.Blunt;

                [JsonProperty("Affinity"), JsonConverter(typeof(InvalidEnumLiteralResolver<AffinityName>), AffinityName.None)]
                public AffinityName Affinity { get; set; } = AffinityName.None;

                [JsonProperty("Action"), JsonConverter(typeof(InvalidEnumLiteralResolver<SkillType>), SkillType.Attack)]
                public SkillType Action { get; set; } = SkillType.Attack;

                [JsonProperty("Rank")]
                public int Rank { get; set; } = 1;


                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    if (Rank > 3) Rank = 3;
                    if (Rank < 1) Rank = 1;
                }
            }

            [JsonProperty("Characteristics")]
            public Characteristics_PROP Characteristics { get; set; } = new();
            public record Characteristics_PROP : Explicit
            {
                [JsonProperty("Coins List")] public ObservableCollection<string> CoinsList { get; set; } = [];

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    if (CoinsList.Count == 0) CoinsList.Add("Regular");

                    int Indexer = 0;
                    foreach (string Coin in CoinsList)
                    {
                        if (!Coin.EqualsToOneOf("Regular", "Unbreakable", "Excision", "Purple")) CoinsList[Indexer] = "Regular";
                        Indexer++;
                    }
                }
            }

            [JsonProperty("Attributes")]
            public Attributes_PROP Attributes { get; set; } = new();
            public record Attributes_PROP : Explicit
            {
                [JsonProperty("Show Affinity Icon")]
                public bool? ShowAffinityIcon { get; set; } = false;
            }



            [OnDeserialized]
            private void HandleRelativePaths_OnRead(StreamingContext ThisFilePathContext)
            {
                if (IconID is not null) IconID = IconID.Replace(RelativeMarker, $"{ThisFilePathContext.Context}");
            }

            [OnSerializing]
            private void HandleRelativePaths_OnSave(StreamingContext ThisFilePathContext)
            {
                IconID = IconID.Replace((string)ThisFilePathContext.Context!, RelativeMarker);
            }



            public static SkillConstructor CreateBlank(BigInteger InitialID)
            {
                return new()
                {
                    SkillName = "", ID = InitialID, IconID = "",
                    Characteristics = new() { CoinsList = ["Regular"], },
                    Attributes = new() { ShowAffinityIcon = true, },
                };
            }

            private class InvalidEnumLiteralResolver<TEnum>(TEnum DefaultEnumValueInput) : JsonConverter<TEnum> where TEnum : struct, Enum
            {
                private readonly TEnum DefaultEnumValue = DefaultEnumValueInput;

                public override TEnum ReadJson(JsonReader Reader, Type ObjectType, TEnum ExistingValue, bool HasExistingValue, JsonSerializer Serializer)
                {
                    if (Reader.TokenType == JsonToken.String)
                    {
                        string? EnumText = Reader.Value?.ToString();
                        if (Enum.TryParse(EnumText, ignoreCase: true, out TEnum ParsedEnum))
                        {
                            return ParsedEnum;
                        }
                        else
                        {
                            return DefaultEnumValue;
                        }
                    }
                    // Maybe `if (Reader.TokenType == JsonToken.Integer)` too, unused there
                    else
                    {
                        return DefaultEnumValue;
                    }
                }

                public override void WriteJson(JsonWriter Writer, TEnum Value, JsonSerializer Serializer)
                {
                    Writer.WriteValue(Value.ToString());
                }
            }
        }
    }
}