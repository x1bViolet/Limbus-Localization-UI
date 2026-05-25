namespace LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing
{
    namespace Modules
    {
        public static class JsonPathRegexConversions
        {
            public record RegexConversion(string Pattern, string Replace);

            /// <summary>
            /// Key is files pattern regex, Value is Dictionary{ Key is JsonPath, Value is list of regex conversions }
            /// </summary>
            public static Dictionary<string, Dictionary<string, List<RegexConversion>>> LoadedJsonPathMultipleRegexConversions = [];

            public static void LoadJsonPathMultipleRegexConversions(string FilePath)
            {
                LoadedJsonPathMultipleRegexConversions.Clear();

                List<string> Lines = TryReadAllLines(FilePath, []);
                int TotalIndex = Lines.Count - 1;
                int LineIndex = 0;
                List<string>? LatestFilePatterns = null;
                string? LatestJsonPath = null;

                foreach (string Line in Lines)
                {
                    try
                    {
                        if (Line == "{Regex Option}")
                        {
                            if (LineIndex + 1 <= TotalIndex & LineIndex + 2 <= TotalIndex)
                            {
                                string Files = Lines[LineIndex + 1][9..];
                                string JsonPath = Lines[LineIndex + 2][12..];

                                List<string> FilePatterns = [.. Files.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(FilePattern => FilePattern.Trim()).Where(FilePattern => FilePattern != "")];
                                LatestFilePatterns = FilePatterns;
                                LatestJsonPath = JsonPath;
                                foreach (string Pattern in FilePatterns)
                                {
                                    if (!LoadedJsonPathMultipleRegexConversions.ContainsKey(Pattern)) LoadedJsonPathMultipleRegexConversions[Pattern] = new Dictionary<string, List<RegexConversion>>();
                                    LoadedJsonPathMultipleRegexConversions[Pattern][JsonPath] = new List<RegexConversion>();
                                }
                            }
                        }
                        else if (LatestFilePatterns != null & LatestJsonPath != null & Line.StartsWith("* Pattern: ") & (LineIndex < TotalIndex && Lines[LineIndex + 1].StartsWith("  Replace: ")))
                        {
                            foreach (string Pattern in LatestFilePatterns!)
                            {
                                string RepalceForPattern = Lines[LineIndex + 1][11..];

                                // Unicode escapes
                                RepalceForPattern = Regex.Replace(RepalceForPattern, @"\\u(?<UnicodeCharacterCode>[a-fA-F0-9]{4})", Match =>
                                {
                                    int UnicodeCharacterCode = int.Parse(Match.Groups["UnicodeCharacterCode"].Value, System.Globalization.NumberStyles.HexNumber);
                                    return $"{(char)UnicodeCharacterCode}";
                                });

                                LoadedJsonPathMultipleRegexConversions[Pattern][LatestJsonPath!].Add(new RegexConversion(Pattern: Line[11..], Replace: RepalceForPattern));
                            }
                        }
                    }
                    catch (Exception ex) { rin(ex.ToString()); }

                    LineIndex++;
                }
            }
            public static string DoMultipleRegexConversions(string JsonText, Dictionary<string, List<RegexConversion>> ReplacementRules)
            {
                JToken JParser = JToken.Parse(JsonText);
                bool SomethingWasReplaced = false;
                
                foreach (KeyValuePair<string, List<RegexConversion>> JsonPathAndConversions in ReplacementRules)
                {
                    foreach (JToken StringItem in JParser.SelectTokens(JsonPathAndConversions.Key))
                    {
                        string StringItem_OriginalValue = $"{StringItem}";
                        string StringItem_WithReplacements = StringItem_OriginalValue;

                        foreach (RegexConversion Conversion in JsonPathAndConversions.Value)
                        {
                            StringItem_WithReplacements = Regex.Replace(StringItem_WithReplacements, Conversion.Pattern, Conversion.Replace, RegexOptions.Singleline);
                        }

                        if (StringItem_OriginalValue != StringItem_WithReplacements)
                        {
                            StringItem.Replace(StringItem_WithReplacements);
                            SomethingWasReplaced = true;
                        }
                    }
                }

                return SomethingWasReplaced ? JParser.ToString(Formatting.Indented) : JsonText;
            }
        }
    }
}