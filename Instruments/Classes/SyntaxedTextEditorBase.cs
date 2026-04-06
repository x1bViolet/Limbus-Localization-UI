using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

namespace LCLocalizationInterface
{
    /// <summary>
    /// General implementations of <see cref="ICSharpCode.AvalonEdit"/> stuff
    /// </summary>
    public class SyntaxedTextEditorBase : TextEditor
    {
        public SyntaxedTextEditorBase()
        {
            TextArea.SelectionBorder = new Pen();
            TextArea.TextView.Options.EnableHyperlinks = false;
            TextArea.TextView.Options.EnableEmailHyperlinks = false;
        }

        public class SyntaxHighlightDefinition : IHighlightingDefinition
        {
            public string? Name { get; set; }
            public IDictionary<string, string>? Properties { get; }
            public HighlightingRuleSet MainRuleSet { get; set; } = new();
            public IEnumerable<HighlightingColor>? NamedHighlightingColors { get; set; }
            public HighlightingColor? GetNamedColor(string Name) => null;
            public HighlightingRuleSet? GetNamedRuleSet(string Name) => null;
        }

        public class HighlightionBrush(string HexColor) : HighlightingBrush
        {
            private readonly Brush ActualBrush = ToSolidColorBrush(HexColor);
            public override Brush GetBrush(ITextRunConstructionContext Context) => ActualBrush;
        }

        public class SingleContentRuleSpan : HighlightingSpan
        {
            public SingleContentRuleSpan((Regex Start, Regex End) StartAndEndPattern, HighlightingColor StartAndEndStyle, Regex ContentPattern, HighlightingColor ContentStyle)
            {
                SpanColorIncludesStart = true; SpanColorIncludesEnd = true;
                StartExpression = StartAndEndPattern.Start; EndExpression = StartAndEndPattern.End;
                SpanColor = StartAndEndStyle;
                RuleSet = new HighlightingRuleSet();
                RuleSet.Rules.Add(new HighlightingRule()
                {
                    Regex = ContentPattern,
                    Color = ContentStyle,
                });
            }
        }



        // DependencyProperty registration for theme keys reference in the style
        private static DependencyProperty RegisterAlt<PropertyType>(object? DefaultValue = null, [CallerMemberName] string DependencyPropertyName = "")
        {
            DefaultValue ??= typeof(PropertyType) == typeof(double) ? 1.0 : Brushes.White;
                    
            string RegularPropertyName = DependencyPropertyName.RegexRemove("Property$");
            return DependencyProperty.Register(
                name: RegularPropertyName, ownerType: typeof(SyntaxedTextEditorBase), propertyType: typeof(PropertyType),
                typeMetadata: new PropertyMetadata(DefaultValue, delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
                {
                    SyntaxedTextEditorBase This = (SyntaxedTextEditorBase)Sender;
                    This.SetPropertyValue<object>(RegularPropertyName, Args.NewValue);
                })
            );
        }



        #region Style DependencyProperties
        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
        public static readonly DependencyProperty CornerRadiusProperty = RegisterProperty<SyntaxedTextEditorBase, CornerRadius>();


        public Brush CaretBrush { get => this.TextArea.Caret.CaretBrush; set => this.TextArea.Caret.CaretBrush = value; }
        public static readonly DependencyProperty CaretBrushProperty = RegisterAlt<Brush>();


        public Brush SelectionBackground { get => this.TextArea.SelectionBrush; set => this.TextArea.SelectionBrush = value; }
        public static readonly DependencyProperty SelectionBackgroundProperty = RegisterAlt<Brush>();


        public Brush SelectionForeground { get => this.TextArea.SelectionForeground; set => this.TextArea.SelectionForeground = value; }
        public static readonly DependencyProperty SelectionForegroundProperty = RegisterAlt<Brush>();


        public Brush SelectionBorderBrush { get => this.TextArea.SelectionBorder.Brush; set => this.TextArea.SelectionBorder.Brush = value; }
        public static readonly DependencyProperty SelectionBorderBrushProperty = RegisterAlt<Brush>();


        public double SelectionBorderThickness { get => this.TextArea.SelectionBorder.Thickness; set => this.TextArea.SelectionBorder.Thickness = value; }
        public static readonly DependencyProperty SelectionBorderThicknessProperty = RegisterAlt<double>();


        public double SelectionBorderCornerRadius { get => this.TextArea.SelectionCornerRadius; set => this.TextArea.SelectionCornerRadius = value; }
        public static readonly DependencyProperty SelectionBorderCornerRadiusProperty = RegisterAlt<double>();
        #endregion
    }
}