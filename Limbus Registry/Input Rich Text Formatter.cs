using LCLocalizationInterface.LimbusRegistry.JsonTypes;

namespace LCLocalizationInterface.LimbusRegistry
{
    public static partial class InputRichTextFormatter
    {
        public enum RichTextFormat
        {
            /// <summary>Convert [KeywordID] and highlight Implicit kewords</summary>
            EGOGifts,

            /// <summary>Convert [KeywordID], [SkillTagID], replace [SomethingElse] inside square brackets with 'Unknown', highlight Implicit kewords</summary>
            Skills,

            /// <summary>Same as <see cref="Skills"/></summary>
            Passives,


            /// <summary>Default value for <see cref="TMProEmitter.TextProcessingMode"/>, no special insertions or replacements</summary>
            None,
        }

        /// <summary>Changed from <see cref="MainWindow.ChangeKeywordsFormatInsertions"/> and used only in Keywords editor mode when Bufs view is active</summary>
        public static List<string> FormatInsertions { get; set; } = ["{0}", "{1}", "{2}", "{3}", "{4}", "{5}", "{6}"];


        [GeneratedRegex(@"(?<TagsBegin><sprite name=""(?<SpriteID>\w+)"">(<color=(?<Color>#[a-fA-F0-9]{6})><link=""(?<DescID>\w+)""><u>|<color=(?<Color>#[a-fA-F0-9]{6})><u><link=""(?<DescID>\w+)"">|<link=""(?<DescID>\w+)""><color=(?<Color>#[a-fA-F0-9]{6})><u>|<u><color=(?<Color>#[a-fA-F0-9]{6})><link=""(?<DescID>\w+)"">))(?<Name>.*?)(?<TagsEnd>(</color></link></u>|</color></u></link>|</link></color></u>|</link></u></color>|</u></color></link>))", RegexOptions.Compiled)]
        private static partial Regex TMProKeyword();

        /// <summary>
        /// With any order of &lt;color&gt; &lt;u&gt; &lt;link&gt; tags . . . ୧((#Φ益Φ#))୨ 
        /// </summary>
        public static Regex TMProKeywordPattern => TMProKeyword();




        /// <summary>
        /// Modify the input rich text so that in the output all conditional constructs like <c>[KeywordID]</c> or <c>'Just keyword name'</c> are replaced with rich text tags that form the final text displayed in the preview.
        /// <br/><br/>
        /// However, this is still an attempt to recreate the original algorithm from the game code based on manual testing, so it may not be very accurate in very specific situations.
        /// </summary>
        public static string Apply(string LimbusText, RichTextFormat SpecifiedRichTextFormat)
        {
            // Replace {0}, {1}, {2}, ... with replacements in Bufs keywords editor mode
            if (@EditorModesShelf.CurrentEditorMode == @EditorModesShelf.Keywords && @EditorModesShelf.Keywords.CheckFileName!.StartsWith("Bufs"))
            {
                try   { LimbusText = string.Format(LimbusText, [.. FormatInsertions]); }
                catch {         /* Some FormatException from string.Format */          }
            }


            // Shorthands (Must be converted first and then left completely untouched since its just a [ID:`Name`] -> <sprite=...>...Name...</link> performed before localization files releasing)
            if (!string.IsNullOrWhiteSpace(SelectedLimbusCustomLanguage.Keywords_ShorthandsRegex))
            {
                LimbusText = Regex.Replace(LimbusText, SelectedLimbusCustomLanguage.Keywords_ShorthandsRegex, Match =>
                {
                    string KeywordID = Match.Groups["ID"].Value;
                    if (KeywordID != "") // && KeywordsLoader.LoadedKeywords_Bufs.ContainsKey(KeywordID)
                    {
                        string SpriteID = KeywordID;
                        if (Match.Groups["SpriteID"].Value != "")
                        {
                            SpriteID = Match.Groups["SpriteID"].Value;
                        }

                        string KeywordName = Match.Groups["Name"].Value;
                        string KeywordColor = ColorDictionaries.LoadedKeywordColors[KeywordID];

                        if (Match.Groups["Color"].Value != "") KeywordColor = Match.Groups["Color"].Value;

                        return
                        $"<sprite name=\"{SpriteID}\">" +
                        $"<color={KeywordColor}>" +
                        $"<u>" +
                        $"<link=\"{KeywordID}\">" +
                        $"{KeywordName}" +
                        $"</link>" +
                        $"</u>" +
                        $"</color>";
                    }
                    else
                    {
                        return Match.Value;
                    }
                });
            }



            // Conversion for Skills, Passives, and E.G.O Gifts
            if (SpecifiedRichTextFormat.EqualsToOneOf(RichTextFormat.Skills, RichTextFormat.Passives, RichTextFormat.EGOGifts))
            {
                ///
                /// Tricky point: Implicit keywords aren't converted if they're inside a full tag structure (<see cref="TMProKeywordPattern"/>).
                /// I tried converting them to [KeywordID] if it exactly matches <see cref="TMProKeywordPattern"/> pattern and has default name from Bufs files to protect from Implicit keywords conversion,
                /// but then I ran into the problem of custom colors and sprites in tags, which remained unchanged in-game when reset to defaults after that when converting [KeywordID] back.
                /// e.g. <sprite><...>Rupture Protection<...> is not converted to <sprite><...>[Burst] Protection<...>
                /// 
                /// So I came up with the idea of ​​simply replacing the spaces in their name with template strings so that the Implicit keywords conversion doesn't happen in them. Idk....
                ///
                const string InternalTMProAntiImplicitKeywordsConversionSpace = "<\xFFFFTMPSpace\xFFFF>";
                if (LimbusText.Contains("<sprite name=\""))
                {
                    LimbusText = InputRichTextFormatter.TMProKeywordPattern.Replace(LimbusText, Match =>
                    {
                        string ID = Match.Groups["DescID"].Value;
                        string MatchedName = Match.Groups["Name"].Value;
                        if (KeywordsLoader.LoadedKeywords_Bufs.TryGetValue(ID, out PlainKeyword? FoundKeyword) && FoundKeyword.Name == MatchedName)
                        {
                            return Match.Value.Replace($">{MatchedName}<", $">{MatchedName.Replace(" ", InternalTMProAntiImplicitKeywordsConversionSpace)}<");
                        }
                        else
                        {
                            return Match.Value;
                        }
                    });
                }



                // Implicit keywords conversion to [KeywordID] (e.g. just 'Burn' in the text to '[Combustion]')
                foreach (PlainKeyword MaybeImplicitKeyword in KeywordsLoader.LoadedKeywords_Bufs.Values)
                {
                    if (LimbusText.Contains(MaybeImplicitKeyword.Name))
                    {
                        /// Replace if matches <see cref="@Configurazione.JsonConfigurationFile.LimbusCustomLangDefinition.LangProperties.Keywords_AutodetectionRegex"/> Implicit pattern
                        LimbusText = Regex.Replace(LimbusText, SelectedLimbusCustomLanguage.Keywords_AutodetectionRegex.Replace("KeywordNameWillBeHere", Regex.Escape(MaybeImplicitKeyword.Name)), Match =>
                        {
                            return $"[{MaybeImplicitKeyword.ID}]";
                        });
                    }
                }



                /// Reset InternalTMProAntiImplicitKeywordsConversionSpace placed before Implicit keywords conversion
                LimbusText = LimbusText.Replace(InternalTMProAntiImplicitKeywordsConversionSpace, " ");



                // [KeywordID]
                LimbusText = Regex.Replace(LimbusText, @"\[(?<DescID>.*?)\]", Match =>
                {
                    string DescID = Match.Groups["DescID"].Value;

                    // If keyword is readed from Bufs….json files
                    if (KeywordsLoader.LoadedKeywords_Bufs.TryGetValue(DescID, out PlainKeyword? FoundKeyword))
                    {
                        string SpriteID = ImageDictionaries.NotSuitableForSpriteTagRedirections.TryGetValue(DescID, out string? AnotherSpriteID)
                            ? AnotherSpriteID
                            : DescID;

                        string KeywordName = FoundKeyword.Name;
                        string KeywordColor = ColorDictionaries.LoadedKeywordColors[DescID];

                        return
                        $"<sprite name=\"{SpriteID}\">" +
                        $"<color={KeywordColor}>" +
                        $"<u>" +
                        $"<link=\"{DescID}\">" +
                        $"{KeywordName}" +
                        $"</link>" +
                        $"</u>" +
                        $"</color>";
                    }

                    // Otherwise, if Skills/Passives and ID not found in loaded keywords list, then maybe Skill Tag
                    else if (SpecifiedRichTextFormat.EqualsToOneOf(RichTextFormat.Skills, RichTextFormat.Passives))
                    {
                        if (KeywordsLoader.LoadedSkillTags.TryGetValue(DescID, out PlainSkillTag? FoundSkillTag))
                        {
                            return $"<color={ColorDictionaries.LoadedSkillTagColors[FoundSkillTag.ID!]}>{FoundSkillTag.Tag}</color>";
                        }
                        else
                        {
                            return LoadedConfiguration.Internal.DisableUnknownForUnidentifiedKeywords
                                ? Match.Value
                                : "<color=#93f03f>Unknown</color>"; // Despair.
                        }
                    }

                    // Otherwise, if E.G.O Gifts, then its just a text in the square brackets
                    else
                    {
                        return Match.Value;
                    }
                });



                if (LoadedConfiguration.PreviewSettings.Base.HighlightStyle == false)
                {
                    LimbusText = LimbusText.RegexRemove(@"<style=""\w+"">|</style>");
                }



                // Changes highlight type restriction
                const char ZWSP = '\u200B';
                if (SpecifiedRichTextFormat is RichTextFormat.EGOGifts)
                {
                    LimbusText = LimbusText.Replace("<style=\"highlight\">", $"<style={ZWSP}\"highlight\">");
                }
                else if (SpecifiedRichTextFormat.EqualsToOneOf(RichTextFormat.Skills, RichTextFormat.Passives))
                {
                    LimbusText = LimbusText.Replace("<style=\"upgradeHighlight\">", $"<style={ZWSP}\"upgradeHighlight\">");
                }
            }



            bool EnableKeywordsSprite = LoadedConfiguration.ScanParameters.EnableKeywordsSprite;
            bool EnableKeywordsUnderline = LoadedConfiguration.ScanParameters.EnableKeywordsUnderline;

            if (EnableKeywordsSprite == false | EnableKeywordsUnderline == false)
            {
                LimbusText = InputRichTextFormatter.TMProKeywordPattern.Replace(LimbusText, Match =>
                {
                    string Result = Match.Value;

                    if (EnableKeywordsSprite == false)
                    {
                        Result = Result.RegexRemove(@"<sprite name=""\w+"">");
                    }
                    if (EnableKeywordsUnderline == false)
                    {
                        Result = Result.RegexRemove(@"<u>|</u>");
                    }

                    return Result;
                });
            }







            return LimbusText;
        }
    }
}