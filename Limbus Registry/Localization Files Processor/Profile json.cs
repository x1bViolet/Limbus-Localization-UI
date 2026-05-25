using System.Diagnostics.CodeAnalysis;

namespace LCLocalizationInterface.LimbusRegistry
{
    public record LocalizationFilesProcessingProfile : Explicit
    {
        // Change backslashes to regular slashes to unify them for matching patterns during localization files processing
        private static string BackslashFormatInstructor(string Value) => Value.Replace("\\", "/");
        private static string PathStringFormatInstructor(string PathString) => Path.Exists(PathString) ? BackslashFormatInstructor(PathString) : PathString;



        [JsonProperty("Paths")]
        public Paths_PROP Paths { get; set; } = new();
        public record Paths_PROP : Explicit
        {
            [JsonProperty("Source path of localization files")]
            public string SourceDirectory { get; set => field = PathStringFormatInstructor(value); } = "";



            [JsonProperty("Destination path of processed files")]
            public string DestinationDirectory { get; set => field = PathStringFormatInstructor(value); } = "";
        }



        [JsonProperty("General localization files whitelist")]
        public string GeneralLocalizationFilesWhitelist { get; set => field = BackslashFormatInstructor(value); } = "*.json";



        [JsonProperty("Files processing order")]
        public string FilesProcessingOrder { get; set; } = "Recently changed first";


        [JsonProperty("Json files formatting")]
        public JsonFilesFormatting_PROP JsonFilesFormatting { get; set; } = new();
        public record JsonFilesFormatting_PROP : Explicit
        {
            [JsonProperty("Existing files overwriting rule")]
            public string ExistingFilesOverwritingRule { get; set; } = "If Json content itself is not the same";


            [JsonProperty("Output json formatting")]
            public string OutputJsonFormatting { get; set; } = "Indented";


            [JsonProperty("Json Indentation size")]
            public string JsonIndentationSize { get; set; } = "Keep original";


            [JsonProperty("Line break mode")]
            public string LineBreakMode { get; set; } = "Keep original";


            [JsonProperty("UTF-8 Byte Order Marks")]
            public string UTF8ByteOrderMarks { get; set; } = "Keep BOM if they are inside original file";
        }



        [JsonProperty("Reference localization")]
        public ReferenceLocalization_PROP ReferenceLocalization { get; set; } = new();
        public record ReferenceLocalization_PROP : Explicit
        {
            [JsonProperty("Directory of reference localization")]
            public string Directory { get; set => field = PathStringFormatInstructor(value); } = @"C:\Program Files (x86)\Steam\steamapps\common\Limbus Company\LimbusCompany_Data\Assets\Resources_moved\Localize\en";



            [JsonProperty("Prefix of reference localization")]
            public string Prefix { get; set; } = "EN_";


            [JsonProperty("Missing content appending")]
            public MissingContentAppending_PROP MissingContentAppending { get; set; } = new();
            public record MissingContentAppending_PROP : Explicit
            {
                [JsonProperty("Append missing files")]
                public bool AppendMissingFiles { get; set; } = false;


                [JsonProperty("Append missing IDs")]
                public bool AppendMissingIDs { get; set; } = false;


                [JsonProperty("Whitelist of missing files to append")]
                public string WhitelistOfFilesToAppend { get; set => field = BackslashFormatInstructor(value); } = "";



                //Skills_personality-*.json, Skills_Ego_Personality-*.json, Passives.json, Personalities.json, GachaTitle*.json, IntroductionPreset.json, Personality_Get_Condition.json
                [JsonProperty("Whitelist of existing files to add IDs to")]
                public string WhitelistOfFilesToAddIDsTo { get; set => field = BackslashFormatInstructor(value); } = "";



                [JsonProperty("Count IDs as missed starting from the last one from source file")]
                public bool CountIDsAsMissedStartingFromLastOneFromSourceFile { get; set; } = true;
            }
        }



        [JsonProperty("Font files")]
        public FontFiles_PROP FontFiles { get; set; } = new();
        public record FontFiles_PROP : Explicit
        {
            [JsonProperty("Also copy font files")]
            public bool AlsoCopyFontFiles { get; set; } = false;


            [JsonProperty("Title font file")]
            public string TitleFontFile { get; set => field = PathStringFormatInstructor(value); } = @"";



            [JsonProperty("Context font file")]
            public string ContextFontFile { get; set => field = PathStringFormatInstructor(value); } = @"";
        }



        [JsonProperty("Miscellaneous")]
        public Misc_PROP Misc { get; set; } = new();
        public record Misc_PROP : Explicit
        {
            [JsonProperty("Add <style> placeholders")]
            public bool AddStylePlaceholders { get; set; } = false;



            [JsonProperty("Do JsonPath Multiple Regex Conversions")]
            public bool DoJsonPathMultipleRegexConversions { get; set; } = false;


            [JsonProperty("JsonPath Multiple Regex Conversions config")]
            public string JsonPathMultipleRegexConversionsConfigFile { get; set => field = PathStringFormatInstructor(value); } = @"";



            [JsonProperty("Convert Keyword Shorthands")]
            public bool ConvertKeywordShorthands { get; set; } = false;


            [JsonProperty("Keyword Shorthands regex pattern"), StringSyntax(StringSyntaxAttribute.Regex)]
            public string KeywordShorthandsRegexPattern { get; set; } = @"\[(?<ID>\w+):`(?<Name>.*?)`\](\((?<Color>#[a-fA-F0-9]{6})?(;(?<SpriteID>\w+))?\))?";
            /* [ID:`Name`]  [ID:`Name`](#color)  [ID:`Name`](#color;Sprite)  [ID:`Name`](;Sprite) */


            [JsonProperty("Keyword Shorthands files whitelist"), StringSyntax(StringSyntaxAttribute.Regex) /*(Not really)*/]
            public string KeywordShorthandsFilesWhiteList { get; set => field = BackslashFormatInstructor(value); } = "Skills*.json, Passive*.json, EGOgift_*.json, PanicInfo*.json, BuffAbilities.json, MentalCondition*.json, BattleKeywords*.json, Bufs*.json";
        }



        [JsonProperty("Merged fonts")]
        public MergedFonts_PROP MergedFonts { get; set; } = new();
        public record MergedFonts_PROP : Explicit
        {
            [JsonProperty("Merged font Characters Replacement Map")]
            public string MergedFontCharactersReplacementMap { get; set => field = PathStringFormatInstructor(value); } = @"";



            [JsonProperty("Merged font Multiple Apply config")]
            public string MergedFontMultipleApplyConfig { get; set => field = PathStringFormatInstructor(value); } = @"";



            [JsonProperty("Regex pattern of text parts to ignore during conversion"), StringSyntax(StringSyntaxAttribute.Regex)]
            public string MergedFontIgnoreSequencesRegexPattern { get; set; } = @"{\d+}|<.*?>|\\n";


            [JsonProperty("Convert fonts by '[font=name]' markers")]
            public bool ConvertFontsByMarkers { get; set; } = false;


            [JsonProperty("Convert fonts by Multiple Apply config")]
            public bool ConvertFontsByMultipleApplyConfig { get; set; } = false;
        }
    }
}