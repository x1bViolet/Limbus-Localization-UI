namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    public record KeywordData : IHasIdentifier<string>
    {
        [JsonProperty("id")]
        public string? ID { get; init; }

        [JsonProperty("buffType")]
        public string? BuffType { get; set; }

        /// <summary>
        /// Returns hex color based on <see cref="BuffType"/> value (Negative: #e30000, Positive: #f8c200, Other/null: #a16a3b)
        /// </summary>
        public string Color => this.BuffType == "Negative" ? "#e30000" : this.BuffType == "Positive" ? "#f8c200" : "#a16a3b";
    }
}
