namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    public record PlainSkillTag : IHasIdentifier<string?>
    {
        [JsonProperty("id")]
        public string? ID { get; init; }


        [JsonProperty("name")]
        public string? Tag { get; set; }


        /// <inheritdoc cref="PlainKeyword.Color"/>
        [JsonProperty("Color")]
        public string? Color { get; set; }
    }
}