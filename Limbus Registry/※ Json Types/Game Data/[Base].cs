namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    /// <summary>
    /// Parent class for limbus data files
    /// </summary>
    /// <typeparam name="DataFileType"></typeparam>
    public record LimbusDataFile<DataFileType>
    {
        /// <summary>
        /// List with game data objects of specified type
        /// </summary>
        [JsonProperty("list")]
        public List<DataFileType> DataList { get; init; } = [];
    }
}
