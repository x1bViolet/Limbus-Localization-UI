using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Limbus_Integration.KeywordsInterrogate;
using static LC_Localization_Task_Absolute.Requirements;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    /// <summary>
    /// TextBlock where <paramref name="RichText"/> property leads to the <see cref="Pocket_Watch_ː_Type_L.Actions.Apply"/> method for generating rich text
    /// </summary>
    public partial class TMProEmitter : TextBlock
    {
        public TMProEmitter()
        {
            Foreground = ToSolidColorBrush("#EBCAA2");
            TextWrapping = TextWrapping.Wrap;
            LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
        }

        public static readonly DependencyProperty RichTextProperty =
            DependencyProperty.Register(
                "RichText",
                typeof(string),
                typeof(TMProEmitter),
                new PropertyMetadata("", OnRichTextChanged));

        private static void OnRichTextChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs) // From XAML Designer by RichText=""
        {
            Pocket_Watch_ː_Type_L.Actions.Apply(
                Target: CurrentElement as TMProEmitter,
                RichText: LimbusPreviewFormatter.Apply(ChangeArgs.NewValue as string),
                DividersMode: Pocket_Watch_ː_Type_L.@PostInfo.FullStopDividers.FullStopDividers_Regular, // Use square breakets in XAML instead of &lt; &gt;
                IgnoreTags: Pocket_Watch_ː_Type_L.@PostInfo.IgnoreTags_UnityTMProExclude
            );

            (CurrentElement as TMProEmitter).CurrentRichText = ChangeArgs.NewValue as string;
        }

        public string CurrentRichText { get; private set; }

        public string LimbusPreviewFormattingMode { get; set; }
        public bool DisableKeyworLinksCreation { get; set; } = false; // Prevent endless keyword tooltips creation for keywords inside keywords tooltips inside tooltips for keywords inside tooltips

        public string RichText
        {
            get => this.CurrentRichText;
            set {
                if (DisableKeyworLinksCreation == false)
                {
                    // Do not let keyword tooltips grab last target place
                    LimbusPreviewFormatter.LastAppliedUnformattedRichText = value;
                    LimbusPreviewFormatter.LastApplyTarget = this;
                }
                
                Pocket_Watch_ː_Type_L.Actions.Apply(
                    Target:       this,
                    RichText:     LimbusPreviewFormatter.Apply(PreviewText: value, SpecifiedTextProcessingMode: this.LimbusPreviewFormattingMode),
                    DividersMode: Pocket_Watch_ː_Type_L.@PostInfo.FullStopDividers.FullStopDividers_TMPro,
                    IgnoreTags:   Pocket_Watch_ː_Type_L.@PostInfo.IgnoreTags_UnityTMProExclude,
                    DisableKeyworLinksCreation: this.DisableKeyworLinksCreation // ୧((#Φ益Φ#))୨
                );

                CurrentRichText = value;
            }
        }
    }

    /// <summary>
    /// With Ctrl + Mouse wheel font size adjustment and word wrap by default (Links also fine)
    /// </summary>
    sealed public partial class JsonTextEditor : ICSharpCode.AvalonEdit.TextEditor
    {
        // For XAML {DynamicResource} theme links
        public Brush CaretBrush { get => this.TextArea.Caret.CaretBrush; set { this.TextArea.Caret.CaretBrush = value; } }
        public static readonly DependencyProperty CaretBrushProperty =
            Register<JsonTextEditor, Brush>("CaretBrush", Brushes.White, (DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs) =>
            { (CurrentElement as JsonTextEditor).CaretBrush = ChangeArgs.NewValue as Brush; });

        public Brush SelectionBackground { get => this.TextArea.SelectionBrush; set { this.TextArea.SelectionBrush = value; } }
        public static readonly DependencyProperty SelectionBackgroundProperty =
            Register<JsonTextEditor, Brush>("SelectionBackground", Brushes.White, (DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs) =>
            { (CurrentElement as JsonTextEditor).SelectionBackground = ChangeArgs.NewValue as Brush; });

        public Brush SelectionForeground { get => this.TextArea.SelectionForeground; set { this.TextArea.SelectionForeground = value; } }
        public static readonly DependencyProperty SelectionForegroundProperty =
            Register<JsonTextEditor, Brush>("SelectionForeground", Brushes.White, (DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs) =>
            { (CurrentElement as JsonTextEditor).SelectionForeground = ChangeArgs.NewValue as Brush; });

        public Brush SelectionBorderBrush { get => this.TextArea.SelectionBorder.Brush; set { this.TextArea.SelectionBorder.Brush = value; } }
        public static readonly DependencyProperty SelectionBorderBrushProperty =
            Register<JsonTextEditor, Brush>("SelectionBorderBrush", Brushes.White, (DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs) =>
            { (CurrentElement as JsonTextEditor).SelectionBorderBrush = ChangeArgs.NewValue as Brush; });

        public double SelectionBorderThickness { get => this.TextArea.SelectionBorder.Thickness; set { this.TextArea.SelectionBorder.Thickness = value; } }
        public static readonly DependencyProperty SelectionBorderThicknessProperty =
            Register<JsonTextEditor, double>("SelectionBorderThickness", 1.0, (DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs) =>
            { (CurrentElement as JsonTextEditor).SelectionBorderThickness = (double)ChangeArgs.NewValue; });

        public double SelectionBorderCornerRadius { get => this.TextArea.SelectionCornerRadius; set { this.TextArea.SelectionCornerRadius = value; } }
        public static readonly DependencyProperty SelectionBorderCornerRadiusProperty =
            Register<JsonTextEditor, double>("SelectionBorderCornerRadius", 1.0, (DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs) =>
            { (CurrentElement as JsonTextEditor).SelectionBorderCornerRadius = (double)ChangeArgs.NewValue; });


        private bool IsCtrlPressed = false;
        public JsonTextEditor()
        {
            WordWrap = true;

            TextArea.SelectionBorder = new Pen(); // No this.TextArea.SelectionBorder null exception at DependencyProperty

            TextArea.TextView.LinkTextForegroundBrush = Brushes.LightBlue;
            TextArea.TextView.LinkTextUnderline = true;

            // Ctrl + Mouse wheel font size adjustment
            PreviewMouseWheel += (Sender, Args) =>
            {
                if (this.IsCtrlPressed)
                {
                    Args.Handled = true; // Prevent text scroll
                    this.FontSize += (Args.Delta > 0) ? 1.05 : (this.FontSize >= 5 ? -1.05 : 0);
                }
            };
            PreviewKeyDown += (Sender, Args) => { if (Args.Key == Key.LeftCtrl) this.IsCtrlPressed = true ; };
            PreviewKeyUp   += (Sender, Args) => { if (Args.Key == Key.LeftCtrl) this.IsCtrlPressed = false; };
        }
    };

    public abstract class SyntaxedTextEditor
    {
        public static void RecompileEditorSyntax()
        {
            if (false) rin(
@$"  Compiling text editor syntax with parameters (For `{Mode_Handlers.Upstairs.ActiveProperties.Key}` mode):
   - Disable Skill Tags: {Mode_Handlers.Upstairs.ActiveProperties.Key.EqualsOneOf("Keywords", "E.G.O Gifts")}
   - Disable Keyword Links: {Mode_Handlers.Upstairs.ActiveProperties.Key.EqualsOneOf("Keywords")}
   - Shorthands Included: {!Configurazione.SelectedAssociativePropery_Shared.Properties.Keywords_ShorthandsRegex.Equals(@"NOTHING THERE")}");

            if (!Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableSyntaxHighlight) MainWindow.MainControl.TextEditor.SyntaxHighlighting = null;
            else MainWindow.MainControl.TextEditor.SyntaxHighlighting = new LimbusJsonTextSyntax(
                     SkillTagColors: KeywordsInterrogate.CollectedSkillTagColors,
                      KeywordColors: KeywordsInterrogate.CollectedKeywordColors,
                   DisableSkillTags: Mode_Handlers.Upstairs.ActiveProperties.Key.EqualsOneOf("Keywords", "E.G.O Gifts"),
                DisableKeywordLinks: Mode_Handlers.Upstairs.ActiveProperties.Key.EqualsOneOf("Keywords")
            );
        }

        public static SolidColorBrush TagsBody_Color = ToSolidColorBrush("#CCCCCC");
        public static SolidColorBrush TagsValue_Color = ToSolidColorBrush("#F54927");
        public static Color TagsBody_Color_NotSolidColorBrush = ToColorBrush("#CCCCCC");

        public static SolidColorBrush TabExplainColor = ToSolidColorBrush("#2779F5");


        sealed public partial class LimbusJsonTextSyntax : IHighlightingDefinition
        {
            public string Name { get; set; }
            public IDictionary<string, string> Properties { get; }
            public HighlightingRuleSet MainRuleSet { get; set; } = new HighlightingRuleSet();
            public IEnumerable<HighlightingColor> NamedHighlightingColors { get; set; }
            public HighlightingColor GetNamedColor(string name) => null;
            public HighlightingRuleSet GetNamedRuleSet(string name) => null;

            /// <summary>
            /// Create new syntax highlightion based on loaded keyword/skilltag colors 
            /// </summary>
            public LimbusJsonTextSyntax(Dictionary<string, string> SkillTagColors, Dictionary<string, string> KeywordColors, bool DisableSkillTags = false, bool DisableKeywordLinks = false)
            {
                #region Tags with specific value type
                MainRuleSet.Spans.Add(new SyntaxHighlightingSpan_SingleContentRule(
                    StartAndEndPattern: [new(@"<(sprite name|link)="""), new(@""">")],
                    StartAndEndStyle: new HighlightingColor() { Foreground = new DefHighlightionBrush(TagsBody_Color), },
                    ContentPattern: new(@"^\w+$"),
                    ContentStyle: new HighlightingColor()
                    {
                        Foreground = new DefHighlightionBrush(TagsValue_Color)
                    })
                );

                MainRuleSet.Spans.Add(new SyntaxHighlightingSpan_SingleContentRule(
                    StartAndEndPattern: [new(@"<(font|font-weight)="""), new(@""">")],
                    StartAndEndStyle: new HighlightingColor() { Foreground = new DefHighlightionBrush(TagsBody_Color), },
                    ContentPattern: new(@"^(.+)$"),
                    ContentStyle: new HighlightingColor()
                    {
                        Foreground = new DefHighlightionBrush(TagsValue_Color)
                    })
                );

                MainRuleSet.Spans.Add(new SyntaxHighlightingSpan_SingleContentRule(
                    StartAndEndPattern: [new(@"<color="), new(@">")],
                    StartAndEndStyle: new HighlightingColor() { Foreground = new DefHighlightionBrush(TagsBody_Color), },
                    ContentPattern: new(@"^#([a-fA-F0-9]{8}|[a-fA-F0-9]{6})$"),
                    ContentStyle: new HighlightingColor()
                    {
                        Foreground = new DefHighlightionBrush(TagsValue_Color)
                    })
                );
                MainRuleSet.Spans.Add(new SyntaxHighlightingSpan_SingleContentRule(
                    StartAndEndPattern: [new(@"<size="), new(@"%>")],
                    StartAndEndStyle: new HighlightingColor() { Foreground = new DefHighlightionBrush(TagsBody_Color), },
                    ContentPattern: new(@"^(\d+)(\.\d+)?$"),
                    ContentStyle: new HighlightingColor()
                    {
                        Foreground = new DefHighlightionBrush(TagsValue_Color)
                    })
                );
                #endregion


                #region Base tags
                MainRuleSet.Rules.Add(new HighlightingRule()
                {
                    Regex = new Regex(@"(<(b|i|u|s|nobr|sub|sup|noparse)>|</(b|i|u|s|nobr|sub|sup|noparse|link|font|font-weight|size|color)>)"),
                    Color = new HighlightingColor()
                    {
                        Foreground = new DefHighlightionBrush(TagsBody_Color)
                    }
                });
                #endregion


                #region Keywords and skilltags with specified colors
                if (!DisableSkillTags) // Add skilltags highlight
                {
                    MainRuleSet.Rules.Add(new HighlightingRule()
                    {
                        Regex = new Regex(@$"\[TabExplain\]"),
                        Color = new HighlightingColor() { Underline = true, Foreground = new DefHighlightionBrush(TabExplainColor) }
                    });
                    foreach (KeyValuePair<string, string> SkillTagColor in SkillTagColors)
                    {
                        MainRuleSet.Rules.Add(new HighlightingRule()
                        {
                            Regex = new Regex(@$"\[{SkillTagColor.Key}\]"),
                            Color = new HighlightingColor() { Foreground = new DefHighlightionBrush(ToSolidColorBrush(SkillTagColor.Value)) }
                        });
                    }
                }

                if (!DisableKeywordLinks | Configurazione.SelectedAssociativePropery_Shared != null)
                {
                    foreach (KeyValuePair<string, string> KeywordColor in KeywordColors)
                    {
                        if (!DisableKeywordLinks)
                        {
                            MainRuleSet.Rules.Add(new HighlightingRule()
                            {
                                Regex = new Regex(@$"\[{KeywordColor.Key}\]"),
                                Color = new HighlightingColor() { Foreground = new DefHighlightionBrush(ToSolidColorBrush(KeywordColor.Value)) }
                            });
                        }

                        if (!Configurazione.SelectedAssociativePropery_Shared.Properties.Keywords_ShorthandsRegex.Equals(@"NOTHING THERE"))
                        {
                            MainRuleSet.Rules.Add(new HighlightingRule()
                            {
                                Regex = new Regex(Configurazione.SelectedAssociativePropery_Shared.Properties.Keywords_ShorthandsRegex.Replace(@"(?<ID>\w+)", KeywordColor.Key)),
                                Color = new HighlightingColor() { Foreground = new DefHighlightionBrush(ToSolidColorBrush(KeywordColor.Value)) }
                            });
                        }
                    }

                    if (!DisableKeywordLinks)
                    {
                        foreach (KeyValuePair<string, string> UnevidentKeyword in KeywordsInterrogate.Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter)
                        {
                            string Color = KeywordsInterrogate.CollectedKeywordColors.ContainsKey(UnevidentKeyword.Value)
                                ? KeywordsInterrogate.CollectedKeywordColors[UnevidentKeyword.Value]
                                : "#93f03f";

                            MainRuleSet.Rules.Add(new HighlightingRule()
                            {
                                Regex = new Regex(LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection.Replace("KeywordNameWillBeHere", UnevidentKeyword.Key.ToEscapeRegexString())),
                                Color = new HighlightingColor() { Foreground = new DefHighlightionBrush(ToSolidColorBrush(Color)) }
                            });
                        }
                    }
                }

                if (!DisableSkillTags) // Finnaly, in passives or skills, if square brackets content still not highlighted as skilltag or keyword, put strikethrough ('Unknown')
                {
                    MainRuleSet.Rules.Add(new HighlightingRule()
                    {
                        Regex = new Regex(@"(\[.*?\]|\[\])"),
                        Color = new HighlightingColor() { Strikethrough = true }
                    });
                }
                #endregion


                #region <style> tag
                //MainRuleSet.Rules.Add(new HighlightingRule()
                //{
                //    Regex = new Regex(@"(<style=""(upgradeHighlight|highlight)"">|</style>)"),
                //    Color = new HighlightingColor()
                //    {
                //        Foreground = new DefHighlightionBrush(ToSolidColorBrush("#f8c200"))
                //    }
                //});
                if (!Mode_Handlers.Upstairs.ActiveProperties.Key.Equals("Keywords")) MainRuleSet.Spans.Add(new HighlightingSpan()
                {
                    RuleSet = MainRuleSet, // Copy all highlight rules

                    //StartExpression = new Regex(@"<style=""(upgradeHighlight|highlight)"">"),
                    StartExpression = Mode_Handlers.Upstairs.ActiveProperties.Key.EqualsOneOf("Passives", "Skills") ? new Regex(@"<style=""highlight"">") : new Regex(@"<style=""upgradeHighlight"">"),
                    EndExpression = new Regex(@"</style>"),

                    StartColor = new HighlightingColor() { Foreground = new DefHighlightionBrush(ToSolidColorBrush("#f8c200")) },
                    EndColor = new HighlightingColor() { Foreground = new DefHighlightionBrush(ToSolidColorBrush("#f8c200")) },

                    //StartColor = new HighlightingColor() { Foreground = new DefHighlightionBrush(TagsBody_Color) },
                    //EndColor = new HighlightingColor() { Foreground = new DefHighlightionBrush(TagsBody_Color) },
                    
                    SpanColor = new HighlightingColor() { Foreground = new DefHighlightionBrush(ToSolidColorBrush("#f8c200")) }

                    //// Yellow color fadeout gradient option
                    //StartColor = new HighlightingColor()
                    //{
                    //    Foreground = new DefHighlightionBrush(new LinearGradientBrush()
                    //    {
                    //        StartPoint = new Point(0, 0),
                    //        EndPoint = new Point(1, 0),
                    //        GradientStops =
                    //        {
                    //            new GradientStop(TagsBody_Color_NotSolidColorBrush, 0),
                    //            new GradientStop(ToColorBrush("#f8c200"), 1)
                    //        }
                    //    })
                    //},
                    //EndColor = new HighlightingColor()
                    //{
                    //    Foreground = new DefHighlightionBrush(new LinearGradientBrush()
                    //    {
                    //        StartPoint = new Point(0, 0),
                    //        EndPoint = new Point(1, 0),
                    //        GradientStops =
                    //        {
                    //            new GradientStop(ToColorBrush("#f8c200"), 0),
                    //            new GradientStop(TagsBody_Color_NotSolidColorBrush, 1)
                    //        }
                    //    })
                    //},

                });
                #endregion
            }
        }

        private class DefHighlightionBrush : HighlightingBrush
        {
            private readonly Brush ActualBrush;
            public override Brush GetBrush(ITextRunConstructionContext context) => ActualBrush;
            public DefHighlightionBrush(Brush From) => ActualBrush = From;
        }
        private class SyntaxHighlightingSpan_SingleContentRule : HighlightingSpan
        {
            public SyntaxHighlightingSpan_SingleContentRule(Regex[] StartAndEndPattern, Regex ContentPattern, HighlightingColor StartAndEndStyle, HighlightingColor ContentStyle)
            {
                StartExpression = StartAndEndPattern[0]; EndExpression = StartAndEndPattern[1];
                SpanColorIncludesStart = true; SpanColorIncludesEnd = true;
                SpanColor = StartAndEndStyle;
                RuleSet = new HighlightingRuleSet();
                RuleSet.Rules.Add(new HighlightingRule()
                {
                    Regex = ContentPattern,
                    Color = ContentStyle,
                });
            }
        }
    }













    
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
        /// <param name="SpecifiedTextProcessingMode">Process text with specified editor mode, by defaultvalue is taken from <br/><c>Mode_Handlers.Upstairs.ActiveProperties.Key</c> if parameter is null</param>
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
