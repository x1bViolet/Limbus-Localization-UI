using System.Text.RegularExpressions;
using LC_Localization_Task_Absolute.Mode_Handlers;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Limbus_Integration.KeywordsInterrogation;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    public static class LimbusPreviewFormatter
    {
        public static string LastAppliedUnformattedRichText = "";
        public static TMProEmitter LastApplyTarget = null;

        public static void UpdateLast()
        {
            if (LastApplyTarget != null)
            {
                if (ActiveProperties.Key == EditorMode.Skills)
                {
                    foreach (KeyValuePair<TMProEmitter, string> SkillDescItem in Mode_Skills.LastPreviewUpdatesBank)
                    {
                        SkillDescItem.Key.RichText = SkillDescItem.Value;
                    }
                }
                else
                {
                    LastApplyTarget.RichText = LastAppliedUnformattedRichText;
                }
            }
        }

        public static Dictionary<string, string> FormatInsertionsReplaceValues = new()
        {
            ["0"] = "{0}",
            ["1"] = "{1}",
            ["2"] = "{2}",
            ["3"] = "{3}",
            ["4"] = "{4}",
            ["5"] = "{5}",
            ["6"] = "{6}",
        };

        public static class RemoteRegexPatterns
        {
            // @"(KeywordNameWillBeHere)(?![\p{L}\[\]\)\-_<'"":\+])"
            public static string AutoKeywordsDetection => @CurrentConfess.SelectedCustomLang.Properties.Keywords_AutodetectionRegex;
            public static readonly Regex StyleMarker = new Regex(@"<style=""\w+"">|</style>", RegexOptions.Compiled);
            public static readonly Regex HexColor = new Regex(@"(#[a-fA-F0-9]{6})", RegexOptions.Compiled);
            public static readonly Regex TMProKeyword = new Regex( // With any order of <color> <u> <link> tags . . .
@"<sprite name=""(?<ID>\w+)"">(<color=(?<Color>#[a-fA-F0-9]{6})><link=""\w+""><u>|<color=(?<Color>#[a-fA-F0-9]{6})><u><link=""\w+"">|<link=""\w+""><color=(?<Color>#[a-fA-F0-9]{6})><u>|<u><color=(?<Color>#[a-fA-F0-9]{6})><link=""\w+"">)(?<Name>.*?)(</color></link></u>|</color></u></link>|</link></color></u>|</link></u></color>|</u></color></link>)", RegexOptions.Compiled
            ); // ୧((#Φ益Φ#))୨
            public static readonly Regex SquareBracketLike = new Regex(@"\[(?<ID>.*?)\](?<Color>\(#[a-fA-F0-9]{6}\))?", RegexOptions.Compiled);
            public static readonly Regex Sprite = new Regex(@"<sprite name=""\w+"">", RegexOptions.Compiled);
            public static readonly Regex Underline = new Regex(@"<u>|</u>", RegexOptions.Compiled);
        }

        /// <summary>
        /// Format input limbus description text based on current editor mode to final version for displaying text for Reassangre Tessal rich text applicator
        /// </summary>
        /// <param name="SpecifiedTextProcessingMode">Process text with specified editor mode, by defaultvalue is taken from <see cref="ActiveProperties"/> 'Key' if parameter is null</param>
        /// <returns></returns>
        public static string Apply(string LimbusText, EditorMode SpecifiedTextProcessingMode = EditorMode.UseCurrentActiveProperties)
        {
            if (SpecifiedTextProcessingMode == EditorMode.UseCurrentActiveProperties) SpecifiedTextProcessingMode = ActiveProperties.Key;

            
            try   { LimbusText = string.Format(LimbusText, FormatInsertionsReplaceValues.Values.ToArray()); }
            catch { }

            // Invalid fonts check
            //if (LimbusText.Contains("<font=\""))
            //{
            //    LimbusText = Regex.Replace(LimbusText, @"<font=""(?<FontName>.*?)"">", Match =>
            //    {
            //        if (!Pocket_Watch_ː_Type_L.@PostInfo.LoadedKnownFonts.ContainsKey(Match.Groups["FontName"].Value))
            //        {
            //            return Match.Value.Replace("<font=\"", "<\u0001font=\"");
            //        }
            //        else
            //        {
            //            return Match.Value;
            //        }
            //    });
            //}

            // Shorthands
            if (@CurrentConfess.SelectedCustomLang.Properties.Keywords_ShorthandsRegex != "")
            {
                LimbusText = @CurrentConfess.ShorthandsPattern.Replace(LimbusText, Match =>
                {
                    string KeywordID = Match.Groups["ID"].Value;
                    if (KeywordID != "")
                    {
                        string KeywordName = Match.Groups["Name"].Value;
                        string KeywordColor = Match.Groups["Color"].Value;
                        
                        if (KeywordColor == "")
                        {
                            KeywordColor = Keywords_Bufs.ContainsKey(KeywordID)
                                ? Keywords_Bufs[KeywordID].StringColor
                                : "#9f6a3a";
                        }

                        return
                        $"<sprite name=\"{KeywordID}\">" +
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
                        return Match.Groups[0].Value;
                    }
                });
            }


            if (SpecifiedTextProcessingMode.EqualsOneOf(EditorMode.Skills, EditorMode.Passives, EditorMode.EGOGifts))
            {
                LimbusText = RemoteRegexPatterns.SquareBracketLike.Replace(LimbusText, Match =>
                {
                    return $"[{Match.Groups["ID"].Value}]\0{Match.Groups["Color"].Value}"; // To avoid premature color conversion error [abcdeID](#color), that must be converted after shorthands (Jia Qui skill jumpscare in release with 'Dialogues(#c8e7d9)' in desc)
                });

                // Collapse all TMPro evident keywords with default names into links for safe unevident keywords conversion
                if (LimbusText.Contains("<sprite name=\""))
                {
                    LimbusText = RemoteRegexPatterns.TMProKeyword.Replace(LimbusText, Match =>
                    {
                        string ID = Match.Groups["ID"].Value;
                        if (Keywords_Bufs.ContainsKey(ID) && Match.Groups["Name"].Value == Keywords_Bufs[ID].Name)  // IF NAME IS DEFAULT (Equals to readed from Bufs file)
                        {
                           return $"[{ID}]({Match.Groups["Color"].Value})"; // With special <color> bypass for `[KeywordID] and skill tags deconversion` to save original color
                        }
                        else return Match.Groups[0].Value;
                    });
                }

                // Unevident keywords conversion to [KeywordID]
                foreach (KeyValuePair<string, string> UnevidentKeyword in Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter)
                {
                    if (LimbusText.Contains(UnevidentKeyword.Key))
                    {
                        // https://regex101.com/r/CcrEVU/3 .NET 7.0 (C#) section
                        //                                                 (Wrath Fragility)(?![\p{L}\[\])\-_<'"":\+]) as example
                        // But broken somehow if there are square brackets in name ('Inner Strength [底力]' hello how th)
                        LimbusText = Regex.Replace(LimbusText, RemoteRegexPatterns.AutoKeywordsDetection.Replace("KeywordNameWillBeHere", UnevidentKeyword.Key.ToEscapeRegexString()), Match =>
                        {
                            return $"[{UnevidentKeyword.Value}]";
                        });
                    }
                }

                // [KeywordID] and skill tags deconversion
                LimbusText = RemoteRegexPatterns.SquareBracketLike.Replace(LimbusText, Match =>
                {
                    string KeywordID = Match.Groups["ID"].Value;

                    if (Keywords_Bufs.ContainsKey(KeywordID))
                    {
                        string KeywordName = Keywords_Bufs[KeywordID].Name;
                        string KeywordColor = RemoteRegexPatterns.HexColor.Match(Match.Groups["Color"].Value).Groups[1].Value;
                        if (KeywordColor == "") KeywordColor = Keywords_Bufs[KeywordID].StringColor;

                        return
                        $"<sprite name=\"{KeywordID}\">" +
                        $"<color={KeywordColor}>" +
                        $"<u>" +
                        $"<link=\"{KeywordID}\">" +
                        $"{KeywordName}" +
                        $"</link>" +
                        $"</u>" +
                        $"</color>";
                    }
                    else if ( // Return skill tag or empty TabExplain if skills, or return default if ego gifts (keywords already excluded)
                           (SpecifiedTextProcessingMode.EqualsOneOf(EditorMode.Skills, EditorMode.Passives) & SkillTags.ContainsKey(Match.Groups[0].Value))
                         | (SpecifiedTextProcessingMode.EqualsOneOf(EditorMode.Skills, EditorMode.Passives) & Match.Groups[0].Value == "[TabExplain]")
                         |  SpecifiedTextProcessingMode == EditorMode.EGOGifts
                    ) {
                        return Match.Groups[0].Value;
                    }
                    else
                    {
                        return "<color=#93f03f>Unknown</color>";
                    }
                });




                if (Configurazione.Spec_EnableKeywordIDSprite == false | Configurazione.Spec_EnableKeywordIDUnderline == false)
                {
                    LimbusText = RemoteRegexPatterns.TMProKeyword.Replace(LimbusText, Match =>
                    {
                        string Result = Match.Groups[0].Value;

                        if (Configurazione.Spec_EnableKeywordIDSprite    == false) Result = Result.RegexRemove(RemoteRegexPatterns.Sprite   );
                        if (Configurazione.Spec_EnableKeywordIDUnderline == false) Result = Result.RegexRemove(RemoteRegexPatterns.Underline);

                        return Result;
                    });
                }




                if (!LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle)
                {
                    LimbusText = LimbusText.RegexRemove(RemoteRegexPatterns.StyleMarker);
                }

                // Skill tags and <style> fixation
                if (SpecifiedTextProcessingMode.EqualsOneOf(EditorMode.Skills, EditorMode.Passives))
                {
                    // 'upgradeHighlight' style only for E.G.O Gifts
                    LimbusText = LimbusText.Replace("<style=\"upgradeHighlight\">", "<style=\u0001\"upgradeHighlight\">");

                    // SkillTag.json
                    foreach (KeyValuePair<string, string> SkillTag in SkillTags)
                    {
                        LimbusText = LimbusText.Replace(SkillTag.Key, SkillTag.Value);
                    }
                }
                else if (SpecifiedTextProcessingMode == EditorMode.EGOGifts)
                {
                    // 'highlight' style only for Passives/Skills
                    LimbusText = LimbusText.Replace("<style=\"highlight\">", "<style=\u0001\"highlight\">");
                }
            }

            LimbusText = LimbusText.Replace("<style>", "<\u0001style>");

            return LimbusText.Replace("\0", "");
        }
    }
}
