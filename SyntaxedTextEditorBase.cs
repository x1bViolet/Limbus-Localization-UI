using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.Requirements;

namespace LC_Localization_Task_Absolute
{
    public ref struct @SyntaxedTextEditor
    {
        public class SyntaxedTextEditorBase : ICSharpCode.AvalonEdit.TextEditor
        {
            #region DependencyProperty
            // For XAML {DynamicResource} theme links
            // get and set accessors are neccessary
            public Brush CaretBrush { get => this.TextArea.Caret.CaretBrush; set { this.TextArea.Caret.CaretBrush = value; } }
            public static readonly DependencyProperty CaretBrushProperty =
                Register<SyntaxedTextEditorBase, Brush>(nameof(CaretBrush), Brushes.White, (CurrentElement, ChangeArgs) =>
                { (CurrentElement as SyntaxedTextEditorBase).CaretBrush = ChangeArgs.NewValue as Brush; });

            public Brush SelectionBackground { get => this.TextArea.SelectionBrush; set { this.TextArea.SelectionBrush = value; } }
            public static readonly DependencyProperty SelectionBackgroundProperty =
                Register<SyntaxedTextEditorBase, Brush>(nameof(SelectionBackground), Brushes.White, (CurrentElement, ChangeArgs) =>
                { (CurrentElement as SyntaxedTextEditorBase).SelectionBackground = ChangeArgs.NewValue as Brush; });

            public Brush SelectionForeground { get => this.TextArea.SelectionForeground; set { this.TextArea.SelectionForeground = value; } }
            public static readonly DependencyProperty SelectionForegroundProperty =
                Register<SyntaxedTextEditorBase, Brush>(nameof(SelectionForeground), Brushes.White, (CurrentElement, ChangeArgs) =>
                { (CurrentElement as SyntaxedTextEditorBase).SelectionForeground = ChangeArgs.NewValue as Brush; });

            public Brush SelectionBorderBrush { get => this.TextArea.SelectionBorder.Brush; set { this.TextArea.SelectionBorder.Brush = value; } }
            public static readonly DependencyProperty SelectionBorderBrushProperty =
                Register<SyntaxedTextEditorBase, Brush>(nameof(SelectionBorderBrush), Brushes.White, (CurrentElement, ChangeArgs) =>
                { (CurrentElement as SyntaxedTextEditorBase).SelectionBorderBrush = ChangeArgs.NewValue as Brush; });

            public double SelectionBorderThickness { get => this.TextArea.SelectionBorder.Thickness; set { this.TextArea.SelectionBorder.Thickness = value; } }
            public static readonly DependencyProperty SelectionBorderThicknessProperty =
                Register<SyntaxedTextEditorBase, double>(nameof(SelectionBorderThickness), 1.0, (CurrentElement, ChangeArgs) =>
                { (CurrentElement as SyntaxedTextEditorBase).SelectionBorderThickness = (double)ChangeArgs.NewValue; });

            public double SelectionBorderCornerRadius { get => this.TextArea.SelectionCornerRadius; set { this.TextArea.SelectionCornerRadius = value; } }
            public static readonly DependencyProperty SelectionBorderCornerRadiusProperty =
                Register<SyntaxedTextEditorBase, double>(nameof(SelectionBorderCornerRadius), 1.0, (CurrentElement, ChangeArgs) =>
                { (CurrentElement as SyntaxedTextEditorBase).SelectionBorderCornerRadius = (double)ChangeArgs.NewValue; });
            #endregion

            public SyntaxedTextEditorBase()
            {
                TextArea.SelectionBorder = new Pen(); // No this.TextArea.SelectionBorder null exception at DependencyProperty
                TextArea.TextView.LinkTextForegroundBrush = Brushes.LightBlue;
            }
        }

        public class SyntaxHighlighting : IHighlightingDefinition
        {
            public string Name { get; set; }
            public IDictionary<string, string> Properties { get; }
            public HighlightingRuleSet MainRuleSet { get; set; } = new HighlightingRuleSet();
            public IEnumerable<HighlightingColor> NamedHighlightingColors { get; set; }
            public HighlightingColor GetNamedColor(string Name) => null;
            public HighlightingRuleSet GetNamedRuleSet(string Name) => null;
        }

        public class HighlightionBrush(string BaseColor) : HighlightingBrush
        {
            private readonly Brush ActualBrush = ToSolidColorBrush(BaseColor);
            public override Brush GetBrush(ITextRunConstructionContext Context) => ActualBrush;
        }
        public class SingleContentRuleSpan : HighlightingSpan
        {
            public SingleContentRuleSpan(Regex[] StartAndEndPattern, HighlightingColor StartAndEndStyle, Regex ContentPattern, HighlightingColor ContentStyle)
            {
                SpanColorIncludesStart = true; SpanColorIncludesEnd = true;
                StartExpression = StartAndEndPattern[0]; EndExpression = StartAndEndPattern[1];
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
