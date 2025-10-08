using System.Text.RegularExpressions;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Limbus_Integration.KeywordsInterrogate;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    public abstract class LimbusPreviewFormatter
    {
        public static string LastAppliedUnformattedRichText = "";
        public static TMProEmitter LastApplyTarget = null;

        public static void UpdateLast()
        {
            if (LastApplyTarget != null)
            {
                if (Mode_Handlers.Upstairs.ActiveProperties.Key == "Skills")
                {
                    foreach (KeyValuePair<TMProEmitter, string> SkillDescItem in Mode_Handlers.Mode_Skills.LastPreviewUpdatesBank)
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

        public static Dictionary<string, string> FormatInsertionsReplaceValues = new Dictionary<string, string>()
        {
            ["0"] = "{0}",
            ["1"] = "{1}",
            ["2"] = "{2}",
            ["3"] = "{3}",
            ["4"] = "{4}",
            ["5"] = "{5}",
            ["6"] = "{6}",
        };

        public abstract class RemoteRegexPatterns
        {
            //                                                   Template until settings load
            public static string AutoKeywordsDetection = new Regex(@"(KeywordNameWillBeHere)(?![\p{L}\[\]\-_<'"":\+])").ToString();
            public static readonly Regex StyleMarker = new Regex(@"<style=""\w+"">|</style>", RegexOptions.Compiled);
            public static readonly Regex HexColor = new Regex(@"(#[a-fA-F0-9]{6})", RegexOptions.Compiled);
            public static readonly Regex TMProKeyword = new Regex(@"<sprite name=""(?<ID>\w+)""><color=(?<Color>#[a-fA-F0-9]{6})><u><link=""\w+"">(?<Name>.*?)(</color></link></u>|</color></u></link>|</link></color></u>|</link></u></color>|</u></color></link>)", RegexOptions.Compiled);
            public static readonly Regex SquareBracketLike = new Regex(@"\[(?<ID>.*?)\](?<Color>\(#[a-fA-F0-9]{6}\))?", RegexOptions.Compiled);
            public static readonly Regex TMProLinks = new Regex(@"(<link=""\w+"">)|(</link>)", RegexOptions.Compiled);
        }

        /// <summary>
        /// Format input limbus description text based on current editor mode to final version for displaying text for Reassangre Tessal rich text applicator
        /// </summary>
        /// <param name="SpecifiedTextProcessingMode">Process text with specified editor mode, by defaultvalue is taken from <see cref="Mode_Handlers.Upstairs.ActiveProperties"/> 'Key' if parameter is null</param>
        /// <returns></returns>
        public static string Apply(string PreviewText, string SpecifiedTextProcessingMode = null)
        {
            if (SpecifiedTextProcessingMode == null) SpecifiedTextProcessingMode = Mode_Handlers.Upstairs.ActiveProperties.Key;
            else if (!SpecifiedTextProcessingMode.EqualsOneOf("E.G.O Gifts", "Keywords", "Passives", "Skills")) // If invalid
            {
                SpecifiedTextProcessingMode = Mode_Handlers.Upstairs.ActiveProperties.Key;
            }

            // Format Insertions
            foreach (KeyValuePair<string, string> Insert in FormatInsertionsReplaceValues)
            {
                PreviewText = PreviewText.Replace($"{{{Insert.Key}}}", Insert.Value);
            }

            // Shorthands
            PreviewText = ShorthandsPattern.Replace(PreviewText, Match =>
            {
                string KeywordID = Match.Groups["ID"].Value;
                if (!KeywordID.Equals(""))
                {
                    string KeywordName = Match.Groups["Name"].Value;
                    string KeywordColor = Regex.Match(Match.Groups["Color"].Value, @"#[a-fA-F0-9]{6}").Value;
                    if (KeywordColor.Equals("") & KeywordsGlossary.ContainsKey(KeywordID))
                    {
                        KeywordColor = KeywordsGlossary[KeywordID].StringColor;
                    }
                    else if (KeywordColor.Equals("")) KeywordColor = "#9f6a3a";

                    return
                    (Configurazione.Spec_EnableKeywordIDSprite ? $"<sprite name=\"{KeywordID}\">" : "") +
                    $"<color={KeywordColor}>" +
                    (Configurazione.Spec_EnableKeywordIDUnderline ? $"<u>" : "") +
                    $"<link=\"{KeywordID}\">{KeywordName}</color>" +
                    $"</link>" +
                    (Configurazione.Spec_EnableKeywordIDUnderline ? $"</u>" : "");
                }
                else
                {
                    return Match.Groups[0].Value;
                }
            });


            if (SpecifiedTextProcessingMode.EqualsOneOf("Skills", "Passives", "E.G.O Gifts"))
            {
                PreviewText = RemoteRegexPatterns.SquareBracketLike.Replace(PreviewText, Match =>
                {
                    return $"[{Match.Groups["ID"].Value}]\0{Match.Groups["Color"].Value}"; // To avoid color error [abcdeID](#color), get out, that must be converted after shorthands (Jia Qui skill jumpscare in release with 'Dialogues(#c8e7d9)' in desc)
                });

                // Collapse all TMPro evident keywords with default names into links for safe unevident keywords conversion
                if (PreviewText.Contains("<sprite name=\""))
                {
                    PreviewText = RemoteRegexPatterns.TMProKeyword.Replace(PreviewText, Match =>
                    {
                        string ID = Match.Groups["ID"].Value;
                        if (!ID.Equals("") & KeywordsGlossary.ContainsKey(ID))
                        {
                            if (Match.Groups["Name"].Value.Equals(KeywordsGlossary[ID].Name))
                            {
                                return $"[{ID}]({Match.Groups["Color"].Value})";
                            }
                        }
                        return Match.Groups[0].Value;
                    });
                }
                // Unevident keywords conversion
                foreach (KeyValuePair<string, string> UnevidentKeyword in Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter)
                {
                    if (PreviewText.Contains(UnevidentKeyword.Key))
                    {
                        // https://regex101.com/r/CcrEVU/3 .NET 7.0 (C#) section
                        //                                                 (Wrath Fragility)(?![\p{L}\[\]\-_<'"":\+]) as example
                        // But broken somehow if there are square brackets in name (Inner Strength [底力] hello how th)
                        PreviewText = Regex.Replace(PreviewText, RemoteRegexPatterns.AutoKeywordsDetection.Replace("KeywordNameWillBeHere", UnevidentKeyword.Key.ToEscapeRegexString()), Match =>
                        {
                            return $"[{UnevidentKeyword.Value}]";
                        });
                    }
                }

                // [KeywordID] deconversion
                PreviewText = RemoteRegexPatterns.SquareBracketLike.Replace(PreviewText, Match =>
                {
                    string KeywordID = Match.Groups["ID"].Value;
                    if (KeywordsGlossary.ContainsKey(KeywordID))
                    {
                        string KeywordName = KeywordsGlossary[KeywordID].Name;
                        string KeywordColor = RemoteRegexPatterns.HexColor.Match(Match.Groups["Color"].Value).Groups[1].Value;
                        if (KeywordColor.Equals("")) KeywordColor = KeywordsGlossary[KeywordID].StringColor;

                        return
                        (Configurazione.Spec_EnableKeywordIDSprite ? $"<sprite name=\"{KeywordID}\">" : "") +
                        $"<color={KeywordColor}>" +
                        (Configurazione.Spec_EnableKeywordIDUnderline ? $"<u>" : "") +
                        $"<link=\"{KeywordID}\">{KeywordName}</color>" +
                        $"</link>" +
                        (Configurazione.Spec_EnableKeywordIDUnderline ? $"</u>" : "");
                    }
                    else if ( // Return skill tag or empty TabExplain if skills, or return default if ego gifts (keywords already excluded)
                           (SpecifiedTextProcessingMode.EqualsOneOf("Skills", "Passives") & SkillTags.ContainsKey(Match.Groups[0].Value))
                         | (SpecifiedTextProcessingMode.EqualsOneOf("Skills", "Passives") & Match.Groups[0].Value.Equals("[TabExplain]"))
                         |  SpecifiedTextProcessingMode.Equals("E.G.O Gifts")
                    ) {
                        return Match.Groups[0].Value;
                    }
                    else
                    {
                        return "<color=#93f03f>Unknown</color>";
                    }
                });

                if (!DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle)
                {
                    PreviewText = PreviewText.RegexRemove(RemoteRegexPatterns.StyleMarker);
                }

                // Skill tags and <style> fixation
                if (SpecifiedTextProcessingMode.EqualsOneOf("Skills", "Passives"))
                {
                    // Style only for E.G.O Gifts
                    PreviewText = PreviewText.Replace("<style=\"upgradeHighlight\">", "<style=\u0001\"upgradeHighlight\">");

                    PreviewText = PreviewText.Replace("[TabExplain]", "");
                    foreach (KeyValuePair<string, string> SkillTag in SkillTags)
                    {
                        PreviewText = PreviewText.Replace(SkillTag.Key, SkillTag.Value);
                    }
                }
                else if (SpecifiedTextProcessingMode.Equals("E.G.O Gifts"))
                {
                    // Style only for Passives/Skills
                    PreviewText = PreviewText.Replace("<style=\"highlight\">", "<style=\u0001\"highlight\">");
                }
            }

            PreviewText = PreviewText.Replace("<style>", "<\u0001style>");

            // <strikethrough>Preview does not support any keyword tooltips</strikethrough>    now it supports
            //PreviewText = RegexRemove(PreviewText, LimbusPreviewFormatter.RemoteRegexPatterns.TMProLinks);
            

            return PreviewText.Replace("\0", "");
        }
    }
}
