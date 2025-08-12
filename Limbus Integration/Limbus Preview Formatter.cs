using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static LC_Localization_Task_Absolute.Limbus_Integration.KeywordsInterrogate;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.MainWindow;
using System.Windows.Media;
using System.Windows;

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    internal abstract class LimbusPreviewFormatter
    {
        internal protected static Dictionary<string, string> FormatInsertions = new Dictionary<string, string>()
        {
            ["0"] = "{0}",
            ["1"] = "{1}",
            ["2"] = "{2}",
            ["3"] = "{3}",
            ["4"] = "{4}",
            ["5"] = "{5}",
            ["6"] = "{6}",
        };

        internal protected static Dictionary<string, FontFamily> LimbusEmbeddedFonts = new Dictionary<string, FontFamily> { };
        
        internal protected static void InitializeLimbusEmbeddedFonts()
        {
            //rin($""$ Loading embedded limbus fonts");

            LimbusEmbeddedFonts = new Dictionary<string, FontFamily>
            {
                                              ["BebasKai SDF"] = FileToFontFamily(@"⇲ Assets Directory\[⇲] Limbus Embedded Fonts\BebasKai.otf"),
                                         ["ExcelsiorSans SDF"] = FileToFontFamily(@"⇲ Assets Directory\[⇲] Limbus Embedded Fonts\ExcelsiorSans.ttf"),
                            ["EN/title)mikodacs/Mikodacs SDF"] = FileToFontFamily(@"⇲ Assets Directory\[⇲] Limbus Embedded Fonts\Mikodacs.otf"),
                          ["KR/p)SCDream(light)/SCDream5 SDF"] = FileToFontFamily(@"⇲ Assets Directory\[⇲] Limbus Embedded Fonts\SCDream5.otf"),
                        ["KR/title)KOTRA_BOLD/KOTRA_BOLD SDF"] = FileToFontFamily(@"⇲ Assets Directory\[⇲] Limbus Embedded Fonts\KOTRA_BOLD.ttf", "KOTRA BOLD"),
                       ["JP/HigashiOme/HigashiOme-Gothic-C-1"] = FileToFontFamily(@"⇲ Assets Directory\[⇲] Limbus Embedded Fonts\HigashiOme-Gothic-C-1.3.ttf"),
                      ["EN/Pretendard/Pretendard-Regular SDF"] = FileToFontFamily(@"⇲ Assets Directory\[⇲] Limbus Embedded Fonts\Pretendard-Regular.ttf"),
                ["EN/cur)Caveat-SemiBold/Caveat-SemiBold SDF"] = FileToFontFamily(@"⇲ Assets Directory\[⇲] Limbus Embedded Fonts\Caveat SemiBold.ttf"),
                ["JP/title)corporate logo(bold)/Corporate-Logo-Bold-ver2 SDF"] = FileToFontFamily(@"⇲ Assets Directory\[⇲] Limbus Embedded Fonts\Corporate-Logo-Bold-ver2.otf"),
            };
        }

        internal protected abstract class RemoteRegexPatterns
        {
            //                                                                Template until settings load
            internal protected static string AutoKeywordsDetection = new Regex(@"(KeywordNameWillBeHere)(?![\p{L}\[\]\-_<'"":\+])").ToString();
            internal protected static Regex StyleMarker = new Regex(@"<style=""\w+"">|</style>");
            internal protected static Regex LoadfontMark = new Regex(@"<loadfont=`.*?`>|</loadfont>");
            internal protected static Regex SpritesOverrideHorizontalOffset = new Regex(@"<spriteshoffset=((\+|\-)\d+)>|</spritesvoffset>");
            internal protected static Regex SpritesOverrideVerticalOffset = new Regex(@"<spritesvoffset=((\+|\-)\d+)>|</spritesvoffset>");
            internal protected static Regex SpritesOverrideSize = new Regex(@"<spritessize=((\+|\-)\d+)>|</spritessize>");
            internal protected static Regex HexColor = new Regex(@"(#[a-fA-F0-9]{6})");
            internal protected static Regex TMProKeyword = new Regex(@"<sprite name=""(?<ID>\w+)""><color=(?<Color>#[a-fA-F0-9]{6})><u><link=""\w+"">(?<Name>.*?)</link></u></color>");
            internal protected static Regex KeywordLink = new Regex(@"\[(?<ID>[^\]]+)?\](?<Color>\(#[a-fA-F0-9]{6}\))?");
            internal protected static Regex TMProLinks = new Regex(@"(<link=""\w+"">)|(</link>)");
        }

        internal protected static string Apply(string PreviewText)
        {
            // Format Insertions
            foreach(KeyValuePair<string, string> Insert in FormatInsertions)
            {
                PreviewText = PreviewText.Replace($"{{{Insert.Key}}}", Insert.Value);
            }

            if (!MainWindow.PreviewUpdate_TargetSite.Equals(MainControl.PreviewLayout_Default))
            {
                // TMPro does not support this
                PreviewText = RemoteRegexPatterns.LoadfontMark.Replace(PreviewText, Match =>
                {
                    return Match.Groups[0].Value.Replace("<", "<\0");
                });
                PreviewText = RemoteRegexPatterns.SpritesOverrideVerticalOffset.Replace(PreviewText, Match =>
                {
                    return Match.Groups[0].Value.Replace("<", "<\0");
                });
                PreviewText = RemoteRegexPatterns.SpritesOverrideHorizontalOffset.Replace(PreviewText, Match =>
                {
                    return Match.Groups[0].Value.Replace("<", "<\0");
                });
                PreviewText = RemoteRegexPatterns.SpritesOverrideSize.Replace(PreviewText, Match =>
                {
                    return Match.Groups[0].Value.Replace("<", "<\0");
                });
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
                    else
                    {
                        if (KeywordColor.Equals("")) KeywordColor = "#9f6a3a";
                    }

                    return 
                    (Configurazione.Spec_EnableKeywordIDSprite ? $"<sprite name=\"{KeywordID}\">" : "") +
                    $"<color={KeywordColor}>" +
                    (Configurazione.Spec_EnableKeywordIDUnderline ? $"<u>" : "") +
                    $"<link=\"{KeywordID}\">{KeywordName}</link>" +
                    (Configurazione.Spec_EnableKeywordIDUnderline? $"</u>" : "") +
                    $"</color>";
                }
                else
                {
                    return Match.Groups[0].Value;
                }
            });


            if (Mode_Handlers.Upstairs.ActiveProperties.Key.EqualsOneOf(["Skills", "Passives", "E.G.O Gifts"]))
            {
                PreviewText = RemoteRegexPatterns.KeywordLink.Replace(PreviewText, Match =>
                {
                    return $"[{Match.Groups["ID"].Value}]\0{Match.Groups["Color"].Value}"; // To avoid color error [abcdeID](#color), get out, that must be converted at line 150 after shorthands (Jia Qui skill jumpscare in release with 'Dialogues(#c8e7d9)' in desc)
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
                        // https://regex101.com/r/CcrEVU/2 .NET 7.0 (C#) section
                        //                                                 (Wrath Fragility)(?![\p{L}\[\]\-_<'"":\+]) as example
                        PreviewText = Regex.Replace(PreviewText, RemoteRegexPatterns.AutoKeywordsDetection.Replace("KeywordNameWillBeHere", UnevidentKeyword.Key), Match =>
                        {
                            return $"[{UnevidentKeyword.Value}]";
                        });
                    }
                }

                // [KeywordID] deconversion
                PreviewText = RemoteRegexPatterns.KeywordLink.Replace(PreviewText, Match =>
                {
                    string MaybeID = Match.Groups["ID"].Value;
                    if (KeywordsGlossary.ContainsKey(MaybeID))
                    {
                        string KeywordName = KeywordsGlossary[MaybeID].Name;
                        string KeywordColor = RemoteRegexPatterns.HexColor.Match(Match.Groups["Color"].Value).Groups[1].Value;
                        if (KeywordColor.Equals("")) KeywordColor = KeywordsGlossary[MaybeID].StringColor;

                        return 
                        (Configurazione.Spec_EnableKeywordIDSprite ? $"<sprite name=\"{MaybeID}\">" : "") +
                        $"<color={KeywordColor}>" +
                        (Configurazione.Spec_EnableKeywordIDUnderline ? $"<u>" : "") +
                        $"{KeywordName}" +
                        (Configurazione.Spec_EnableKeywordIDUnderline ? $"</u>" : "") +
                        $"</color>";
                    }
                    else if ( // Return skill tag or empty TabExplain if skills, or return default if ego gifts (keywords already excluded)
                           (Mode_Handlers.Upstairs.ActiveProperties.Key.EqualsOneOf(["Skills", "Passives"]) & SkillTags.ContainsKey(Match.Groups[0].Value))
                         | (Mode_Handlers.Upstairs.ActiveProperties.Key.EqualsOneOf(["Skills", "Passives"]) & Match.Groups[0].Value.Equals("[TabExplain]"))
                         |  Mode_Handlers.Upstairs.ActiveProperties.Key.Equals("E.G.O Gifts")
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
                    PreviewText = RegexRemove(PreviewText, RemoteRegexPatterns.StyleMarker);
                }

                // Skill tags
                if (Mode_Handlers.Upstairs.ActiveProperties.Key.EqualsOneOf(["Skills", "Passives"]))
                {
                    PreviewText = PreviewText.Replace("[TabExplain]", "");
                    foreach (KeyValuePair<string, string> SkillTag in SkillTags)
                    {
                        PreviewText = PreviewText.Replace(SkillTag.Key, SkillTag.Value);
                    }
                }
            }

            // Preview does not support any keyword tooltips
            PreviewText = RegexRemove(PreviewText, LimbusPreviewFormatter.RemoteRegexPatterns.TMProLinks);
            return PreviewText.Replace("\0", "");
        }
    }
}
