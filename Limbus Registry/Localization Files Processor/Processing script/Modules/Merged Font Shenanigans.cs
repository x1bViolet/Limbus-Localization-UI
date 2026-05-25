using static LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing.Modules.Main;

namespace LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing
{
    namespace Modules
    {
        public static class MergedFontShenanigans
        {
            /// <summary>
            /// Key is font name, Value is characters replacement dictionary
            /// </summary>
            public static Dictionary<string, Dictionary<string, string>> LoadedMergedFontReplacementMap = [];

            /// <summary>
            /// Key is file pattern, Value is list of applied font rules (JsonPath of affected properties and Font Name from <see cref="LoadedMergedFontReplacementMap"/>)
            /// </summary>
            public static Dictionary<string, List<MergedFontRule>> LoadedMergedFontMultipleApplyConfig = [];

            public record MergedFontRule
            {
                [JsonProperty("Font")]
                public string FontName { get; set; } = "";

                [JsonProperty("JsonPath")]
                public string JsonPath { get; set; } = "";

                [JsonIgnore] // For report only
                public string FilesPattern { get; set; } = "";
            }








            public static string PlaceMergedFontCharacters(string Text, Dictionary<string, string> FontFromReplacementMap)
            {
                #region Local methods
                //              original pos, string part itself
                static Dictionary<int, string> GetMergedFontIgnoredSequences(string Text)
                {
                    return Regex.Matches(Text, DataContextDomain.LocalizationFilesProcessor.Profile.MergedFonts.MergedFontIgnoreSequencesRegexPattern)
                        .Select(Match => new KeyValuePair<int, string>(key: Match.Index, value: Match.Value)).ToDictionary();
                }

                static string InsertIgnoredSequencesBack(string Text, Dictionary<int, string> RecordedIgnoredSequences)
                {
                    static string InsertIgnoredSequenceBack(string Text, KeyValuePair<int, string> IgnoredSequenceInfo)
                    {
                        return new StringBuilder(value: Text)
                            .Remove(startIndex: IgnoredSequenceInfo.Key, length: IgnoredSequenceInfo.Value.Length)
                            .Insert(index: IgnoredSequenceInfo.Key, value: IgnoredSequenceInfo.Value).ToString();
                    }

                    foreach (KeyValuePair<int, string> EscapeArea in RecordedIgnoredSequences)
                    {
                        Text = InsertIgnoredSequenceBack(Text, EscapeArea);
                    }

                    return Text;
                }
                #endregion

                Dictionary<int, string> RecordedIgnoreSequences = GetMergedFontIgnoredSequences(Text);

                foreach (KeyValuePair<string, string> CharPair in FontFromReplacementMap)
                {
                    Text = Text.Replace(CharPair.Key, CharPair.Value);
                }

                Text = InsertIgnoredSequencesBack(Text, RecordedIgnoreSequences);

                return Text;
            }

            public static string PlaceMergedFontCharactersForTextWithMarkers(string Text, string LoggingFileName)
            {
                if (Text.Contains("[/font]") == false) Text += "[/font]"; // Add close tag

                Text = Regex.Replace(Text, @"\[font=(?<FontName>\w+)\](?<TextForConvesion>.*?)\[/font\]", Match =>
                {
                    string TextForConvesion = Match.Groups["TextForConvesion"].Value;
                    string FontNameFromReplacementMap = Match.Groups["FontName"].Value;

                    if (LoadedMergedFontReplacementMap.TryGetValue(FontNameFromReplacementMap, out Dictionary<string, string>? FoundReplacementMap))
                    {
                        TextForConvesion = PlaceMergedFontCharacters(TextForConvesion, FoundReplacementMap);
                    }
                    else
                    {
                        ErrorMessageWindow.ShowException(
                            new ArgumentException("Invalid Merged Font name in marker"),
                            $"The line <b>\"<noparse>{Text}</noparse>\"</b> in the <b>\"{LoggingFileName}\"</b> file, for which Merged Font conversion via a marker should occur, contains \"[font={FontNameFromReplacementMap}]\" marker with reference to the unknown font (Available fonts in current replacement map: <b>[{string.Join(", ", LoadedMergedFontReplacementMap.Keys)}]</b>)",
                            EnableLocalizationFilesProcessorCancelButton: true
                        );
                    }


                    return TextForConvesion;
                });

                return Text;
            }




            public static bool CheckForInvalidMergedFontFontRules()
            {
                bool FoundInvalidRules = false;

                if (LoadedMergedFontReplacementMap.Keys.Count > 0)
                {
                    foreach (KeyValuePair<string, List<MergedFontRule>> AppliedFontRules in LoadedMergedFontMultipleApplyConfig)
                    {
                        foreach (MergedFontRule FontRule in AppliedFontRules.Value)
                        {
                            FontRule.FilesPattern = AppliedFontRules.Key; // Add AppliedFontRules FilesPattern to value

                            if (LoadedMergedFontReplacementMap.ContainsKey(FontRule.FontName) == false)
                            {
                                ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(
                                    "Invalid font name in Merged Font Multiple Apply Config",
                                    $"Font rule '{AppliedFontRules.Key} -> {FontRule.JsonPath}' from Merged Font Multiple Apply Config uses unknown font name \"{FontRule.FontName}\" that is not defined at the current Merged Font Characters Replacement map.\nIgnore this issue?\n\n<size=88%>Click \"Yes\" to ignore, \"No\" to cancel the processing.</size>",
                                    CancelAction: () => FoundInvalidRules = true,
                                    UseShowDialog: true
                                );
                            }
                        }
                    }
                }

                return FoundInvalidRules;
            }

            public static string PlaceMergedFontsByMultipleApplyConfig(string JsonText, List<MergedFontRule> FontRules)
            {
                JToken JParser = JToken.Parse(JsonText);
                bool SomethingWasReplaced = false;

                foreach (MergedFontRule FontRule in FontRules)
                {
                    if (LoadedMergedFontReplacementMap.ContainsKey(FontRule.FontName))
                    {
                        foreach (JToken StringItem in JParser.SelectTokens(FontRule.JsonPath))
                        {
                            string StringItem_OriginalValue = $"{StringItem}";
                            if (StringItem_OriginalValue != "")
                            {
                                string StringItem_WithConvertedFont = PlaceMergedFontCharacters(StringItem_OriginalValue, LoadedMergedFontReplacementMap[FontRule.FontName]);

                                if (StringItem_WithConvertedFont != StringItem_OriginalValue)
                                {
                                    StringItem.Replace(StringItem_WithConvertedFont);
                                    SomethingWasReplaced = true;
                                }
                            }
                        }
                    }
                }

                return SomethingWasReplaced ? JParser.ToString(Formatting.Indented) : JsonText;
            }


            private static readonly Regex PropertiesWithFontMarkersPattern = new(@"""(?<PropertyName>\w+)"":(?<SpacingBeforePropertyValue> +)?""(?<PropertyValueWithFontMarker>.*?\[font=\w+].*?)""(?<Afterward>(,)?(\r)?(\n))", RegexOptions.Compiled);
            public static string PlaceMergedFontByMarkers(string JsonText, string LoggingFileName)
            {
                JsonText = PropertiesWithFontMarkersPattern.Replace(JsonText, Match =>
                {
                    string TextForConversion = Match.Groups["PropertyValueWithFontMarker"].Value;

                    return @$"""{Match.Groups["PropertyName"].Value}"":{Match.Groups["SpacingBeforePropertyValue"].Value}""{PlaceMergedFontCharactersForTextWithMarkers(TextForConversion, LoggingFileName)}""{Match.Groups["Afterward"].Value}";
                });

                return JsonText;
            }   
        }
    }
}