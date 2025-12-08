using Newtonsoft.Json;
using System.Runtime.Serialization;

#pragma warning disable IDE0079
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute.Json
{
    public static class SkillsDisplayInfo
    {
        public static readonly Dictionary<int, SkillConstructor> LoadedSkillConstructors = [];

        public static SkillConstructor CreateBlankConstructor()
        {
            return new SkillConstructor()
            {
                SkillName = "",
                ID = 1, IconID = "",
                Specific = new SkillContstructor_Specific()
                {
                    Rank = 1,
                    Action = "Attack",
                    Affinity = "None",
                    DamageType = "Blunt",
                },
                Characteristics = new SkillContstructor_Characteristics()
                {
                    CoinsList = new List<string>() { "Regular" },
                },
                Attributes = new SkillContstructor_Attributes()
                {
                    ShowAffinityIcon = true,
                },
            };
        }

        public record SkillsDisplayInfoFile
        {
            [JsonProperty("Skills Info")]
            public List<SkillConstructor> List { get; set; } = [];
        }
        public record SkillConstructor
        {
            [JsonIgnore] private const string RelativeMarker = ":Current-Directory:";


            [JsonProperty("(Name)")]
            public string? SkillName { get; set; }

            public int? ID { get; set; }

            [JsonProperty("Icon ID")]
            public string? IconID { get; set; }

            [JsonProperty("Specific")]
            public SkillContstructor_Specific Specific { get; set; } = new();

            [JsonProperty("Characteristics")]
            public SkillContstructor_Characteristics Characteristics { get; set; } = new();

            [JsonProperty("Attributes")]
            public SkillContstructor_Attributes Attributes { get; set; } = new();

            [OnDeserialized]
            private void TechnicalProcessing(StreamingContext ThisFilePathContext)
            {
                if (IconID != null) IconID = IconID.Replace(RelativeMarker, $"{ThisFilePathContext.Context}");
            }

            [OnSerializing]
            private void HandleRelativePaths_OnSave(StreamingContext ThisFilePathContext)
            {
                IconID = IconID.Replace((string)ThisFilePathContext.Context, RelativeMarker);
            }
        }

        public record SkillContstructor_Specific
        {
            [JsonProperty("Damage Type")] public string DamageType { get; set; } = "None";
            [JsonProperty("Affinity"   )] public string Affinity   { get; set; } = "None";
            [JsonProperty("Action"     )] public string Action     { get; set; } = "Attack";
            [JsonProperty("Rank"       )] public int    Rank       { get; set; } = 1;

            [OnDeserialized]
            private void TechnicalProcessing(StreamingContext Context)
            {
                if (!Affinity.EqualsOneOf("Wrath", "Lust", "Sloth", "Gluttony", "Gloom", "Pride", "Envy")) Affinity = "None";

                if (Rank > 3) Rank = 3;
                if (Rank < 1) Rank = 1;
                if (!DamageType.EqualsOneOf("Pierce", "Blunt", "Slash")) DamageType = "None";
            }
        }

        public record SkillContstructor_Characteristics
        {
            [JsonProperty("Coins List")] public List<string> CoinsList { get; set; } = [];

            [OnDeserialized]
            private void OnDeserialized(StreamingContext Context)
            {
                if (CoinsList.Count == 0) CoinsList.Add("Regular");

                int Indexer = 0;
                foreach (string Coin in CoinsList)
                {
                    if (!Coin.EqualsOneOf("Regular", "Unbreakable")) CoinsList[Indexer] = "Regular";
                    Indexer++;
                }
            }
        }

        public record SkillContstructor_Attributes
        {
            [JsonProperty("Show Affinity Icon")] public bool? ShowAffinityIcon { get; set; } = false;
        }
    }
}
