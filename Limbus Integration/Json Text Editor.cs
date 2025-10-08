using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.Requirements;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
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
            PreviewKeyDown += (Sender, Args) => { if (Args.Key == Key.LeftCtrl) this.IsCtrlPressed = true; };
            PreviewKeyUp += (Sender, Args) => { if (Args.Key == Key.LeftCtrl) this.IsCtrlPressed = false; };
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
}
