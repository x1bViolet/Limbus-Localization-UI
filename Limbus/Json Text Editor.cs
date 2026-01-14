using ICSharpCode.AvalonEdit.Highlighting;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.@SyntaxedTextEditor;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    /// <summary>
    /// With Ctrl + Mouse wheel font size adjustment and word wrap by default (Links also fine)
    /// </summary>
    public partial class JsonTextEditor : @SyntaxedTextEditor.SyntaxedTextEditorBase
    {
        private bool IsCtrlPressed = false;
        public JsonTextEditor()
        {
            WordWrap = true;

            // Ctrl + Mouse wheel font size adjustment
            PreviewKeyDown += (Sender, Args) => { if (Args.Key == Key.LeftCtrl) this.IsCtrlPressed = true ; };
            PreviewKeyUp   += (Sender, Args) => { if (Args.Key == Key.LeftCtrl) this.IsCtrlPressed = false; };
            PreviewMouseWheel += (Sender, Args) =>
            {
                if (this.IsCtrlPressed)
                {
                    Args.Handled = true; // Prevent text scroll
                    this.FontSize += (Args.Delta > 0) ? 1.05 : (this.FontSize >= 5 ? -1.05 : 0);
                }
            };
        }

        public static void RecompileEditorSyntax()
        {
            if (!LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSyntaxHighlight)
            {
                MainWindow.MainControl.TextEditor.SyntaxHighlighting = null;
            }
            else
            {
                MainWindow.MainControl.TextEditor.SyntaxHighlighting = new LimbusJsonTextSyntax(
                         SkillTagColors: KeywordsInterrogation.CollectedSkillTagColors,
                          KeywordColors: KeywordsInterrogation.CollectedKeywordColors,
                       DisableSkillTags: ActiveProperties.Key == EditorMode.Keywords | ActiveProperties.Key == EditorMode.EGOGifts,
                    DisableKeywordLinks: ActiveProperties.Key == EditorMode.Keywords
                );
            }
        }
    }

    public class LimbusJsonTextSyntax : @SyntaxedTextEditor.SyntaxHighlighting
    {
        public static string TagsBody_Color;
        public static string TagsValue_Color;
        public static string TabExplainColor;

        #pragma Do not use (.*?) as Span Content's pattern, endless exception about something will appear, (.+) is better
        /// <summary>
        /// Create new syntax highlightion based on current editor mode and loaded keywords
        /// </summary>
        public LimbusJsonTextSyntax(Dictionary<string, string> SkillTagColors, Dictionary<string, string> KeywordColors, bool DisableSkillTags = false, bool DisableKeywordLinks = false)
        {
            #region Tags with specific value type
            MainRuleSet.Spans.Add(new SingleContentRuleSpan(
                StartAndEndPattern: [new(@"<(sprite name|link)="""), new(@""">")],
                StartAndEndStyle: new HighlightingColor() { Foreground = new HighlightionBrush(TagsBody_Color), },
                ContentPattern: new(@"^\w+$"),
                ContentStyle: new HighlightingColor()
                {
                    Foreground = new HighlightionBrush(TagsValue_Color)
                })
            );

            MainRuleSet.Spans.Add(new SingleContentRuleSpan(
                StartAndEndPattern: [new(@"<(font|font-weight)="""), new(@""">")],
                StartAndEndStyle: new HighlightingColor() { Foreground = new HighlightionBrush(TagsBody_Color), },
                ContentPattern: new(@"^.+$"),
                ContentStyle: new HighlightingColor()
                {
                    Foreground = new HighlightionBrush(TagsValue_Color)
                })
            );

            MainRuleSet.Spans.Add(new SingleContentRuleSpan(
                StartAndEndPattern: [new(@"<(color|mark color|mark)="), new(@">")],
                StartAndEndStyle: new HighlightingColor() { Foreground = new HighlightionBrush(TagsBody_Color), },
                ContentPattern: new(@"^#([a-fA-F0-9]{8}|[a-fA-F0-9]{6})$"),
                ContentStyle: new HighlightingColor()
                {
                    Foreground = new HighlightionBrush(TagsValue_Color)
                })
            );
            MainRuleSet.Spans.Add(new SingleContentRuleSpan(
                StartAndEndPattern: [new(@"<size="), new(@"%>")],
                StartAndEndStyle: new HighlightingColor() { Foreground = new HighlightionBrush(TagsBody_Color), },
                ContentPattern: new(@"^\d+((\.|\,)\d+)?$"),
                ContentStyle: new HighlightingColor()
                {
                    Foreground = new HighlightionBrush(TagsValue_Color)
                })
            );
            #endregion


            #region Base tags
            MainRuleSet.Rules.Add(new HighlightingRule()
            {
                Regex = new Regex(@"<(b|i|u|s|nobr|sub|sup|noparse)>|</(b|i|u|s|nobr|sub|sup|noparse|link|font|font-weight|size|color|mark)>"),
                Color = new HighlightingColor()
                {
                    Foreground = new HighlightionBrush(TagsBody_Color)
                }
            });
            #endregion


            #region Keywords and skilltags with specified colors
            // Key = ID
            // Value = Color

            if (!DisableSkillTags) // Add skilltags highlight
            {
                MainRuleSet.Rules.Add(new HighlightingRule()
                {
                    Regex = new Regex(@$"\[TabExplain\]"),
                    Color = new HighlightingColor() { Underline = true, Foreground = new HighlightionBrush(TabExplainColor) }
                });
                foreach (KeyValuePair<string, string> SkillTagColor in SkillTagColors)
                {
                    MainRuleSet.Rules.Add(new HighlightingRule()
                    {
                        Regex = new Regex(@$"\[{SkillTagColor.Key}\]"),
                        Color = new HighlightingColor() { Foreground = new HighlightionBrush(SkillTagColor.Value) }
                    });
                }
            }

            if (!DisableKeywordLinks | @CurrentConfess.SelectedCustomLang != null)
            {
                foreach (KeyValuePair<string, string> KeywordColor in KeywordColors)
                {
                    if (!DisableKeywordLinks)
                    {
                        MainRuleSet.Rules.Add(new HighlightingRule()
                        {
                            Regex = new Regex(@$"\[{KeywordColor.Key}\]"),
                            Color = new HighlightingColor() { Foreground = new HighlightionBrush(KeywordColor.Value) }
                        });
                    }

                    if (@CurrentConfess.SelectedCustomLang.Properties.Keywords_ShorthandsRegex != @"")
                    {
                        MainRuleSet.Rules.Add(new HighlightingRule()
                        {
                            Regex = new Regex(@CurrentConfess.SelectedCustomLang.Properties.Keywords_ShorthandsRegex.Replace(@"(?<ID>\w+)", KeywordColor.Key)),
                            Color = new HighlightingColor() { Foreground = new HighlightionBrush(KeywordColor.Value) }
                        });
                    }
                }

                if (!DisableKeywordLinks)
                {
                    foreach (KeyValuePair<string, string> UnevidentKeyword in KeywordsInterrogation.Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter)
                    {
                        string Color = KeywordsInterrogation.CollectedKeywordColors.ContainsKey(UnevidentKeyword.Value)
                            ? KeywordsInterrogation.CollectedKeywordColors[UnevidentKeyword.Value]
                            : "#93f03f";

                        MainRuleSet.Rules.Add(new HighlightingRule()
                        {
                            Regex = new Regex(LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection.Replace("KeywordNameWillBeHere", UnevidentKeyword.Key.ToEscapeRegexString())),
                            Color = new HighlightingColor() { Foreground = new HighlightionBrush(Color) }
                        });
                    }
                }
            }

            if (!DisableSkillTags) // At the end, in passives or skills, if square brackets content still not highlighted as skilltag or keyword, put strikethrough ('Unknown')
            {
                MainRuleSet.Rules.Add(new HighlightingRule()
                {
                    Regex = new Regex(@"\[.*?\]|\[\]"),
                    Color = new HighlightingColor() { Strikethrough = true }
                });
            }
            #endregion


            MainRuleSet.Rules.Add(new HighlightingRule()
            {
                Regex = new Regex(@"\\n|\\r|\\t"), // Make no mistake
                Color = new HighlightingColor() { Strikethrough = true, Foreground = new HighlightionBrush("#e30000") }
            });


            #region <style> tag
            if (ActiveProperties.Key != EditorMode.Keywords)
            {
                if (LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle)
                {
                    MainRuleSet.Spans.Add(new HighlightingSpan()
                    {
                        RuleSet = MainRuleSet, // Copy all highlight rules

                        StartExpression = new Regex(ActiveProperties.Key.EqualsOneOf(EditorMode.Passives, EditorMode.Skills) ? @"<style=""highlight"">" : @"<style=""upgradeHighlight"">"),
                        EndExpression = new Regex(@"</style>"),

                        StartColor = new HighlightingColor() { Foreground = new HighlightionBrush("#f8c200") },
                        EndColor = new HighlightingColor() { Foreground = new HighlightionBrush("#f8c200") },

                        SpanColor = new HighlightingColor() { Foreground = new HighlightionBrush("#f8c200") }
                    });
                }
                else
                {
                    MainRuleSet.Rules.Add(new HighlightingRule()
                    {
                        Regex = new Regex(@"</style>"),
                        Color = new HighlightingColor() { Foreground = new HighlightionBrush("#f8c200") }
                    });
                    MainRuleSet.Rules.Add(new HighlightingRule()
                    {
                        Regex = new Regex(ActiveProperties.Key.EqualsOneOf(EditorMode.Passives, EditorMode.Skills) ? @"<style=""highlight"">" : @"<style=""upgradeHighlight"">"),
                        Color = new HighlightingColor() { Foreground = new HighlightionBrush("#f8c200") }
                    });
                }
            }
            #endregion
        }
    }
}