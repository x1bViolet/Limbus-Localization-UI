using System.Windows.Markup;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage.ImageInfoJsonFile.TextColumns_PROP;

namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public class TextElementsColumn : VirtualizingStackPanel;
    

    [ContentProperty(nameof(LocalizationTextView))]
    public partial class ColumnTextElementContainer : UserControl
    {
        public ColumnTextElementContainer()
        {
            InitializeComponent();

            this.Loaded += (_, _) => this.SealLocalizationTextView();
        }


        public TextElementsColumn ParentColumn => (this.Parent as TextElementsColumn)!;


        #region Sealing
        ~ColumnTextElementContainer() => App.HandledInvoke(this.UnsealLocalizationTextView);

        private Image TemplateSealedView => this.FindTypeNameFromTemplate<Image>("TemplateSealedView")!;
        private Grid TemplateContentGrid => this.FindTypeNameFromTemplate<Grid>("PART_TemplateContentGrid")!;
        public bool IsViewSealed => this.TemplateSealedView?.Visibility is Visibility.Visible;


        /// <summary>
        /// Convert UI element view to static image and then hide UI element view, huge performance benefit in case with Skills and Passives + their Signature with drop shadow within <see cref="CautionsTextElement"/> template + Cautions (<see cref="DropShadowEffect"/> has a particularly strong impact on scrolling performance, I think it's really its fault)<br/>
        /// • <see cref="BattleKeywordContainer_PCE"/>s are unaffected because they have no impact on performance (i.e. small text in general + no drop shadows within <see cref="BattleKeywordContainer_PCE"/> template)<br/>
        /// • Template must be applied at the moment of execution of this method (i.e. <c>`IsLoaded == <see langword="true"/>`</c> or <c>`Template != <see langword="null"/>`</c> or <c>`CautionsTextElement.Loaded += (_, _) => CautionsTextElement.SealLocalizationTextView()`</c>)
        /// </summary>
        public async void SealLocalizationTextView()
        {
            if (LoadedConfiguration.Internal.DisableTextElementsSealingInPreviewCreator == false)
            {
                if (this.IsViewSealed | this.RelatedJsonData.Type is ColumnTextElementType.Keyword) return;

                if (this.Template is not null)
                {
                    if (this.TemplateContentGrid is null)
                    {
                        await Task.Delay(500); // Idk, some oddities with the Template elements creation timings when settings options "Enable Keywords Underline/Sprite" is clicked too frequently or even on Loaded/OnTemplateApplying
                    }

                    try
                    {
                        this.TemplateSealedView.Source = CaptureElement(this.TemplateContentGrid!);
                        this.TemplateSealedView.Visibility = Visibility.Visible;
                        this.TemplateContentGrid!.Visibility = Visibility.Collapsed;
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Revert <see cref="SealLocalizationTextView"/> effect, same execution conditions
        /// </summary>
        public void UnsealLocalizationTextView()
        {
            if (this.IsViewSealed == false | this.RelatedJsonData.Type is ColumnTextElementType.Keyword) return;

            if (this.Template is not null)
            {
                this.TemplateContentGrid.Visibility = Visibility.Visible;

                // Memory clean (Probably)  (https://github.com/dotnet/wpf/issues/2397#issuecomment-570837535)
                {
                    this.TemplateSealedView.Source = null;
                    this.TemplateSealedView.UpdateLayout();
                }

                this.TemplateSealedView.Visibility = Visibility.Collapsed;
            }
        }
        #endregion





        public required ColumnTextElementData RelatedJsonData { get => (ColumnTextElementData)GetValue(RelatedJsonDataProperty); set => SetValue(RelatedJsonDataProperty, value); }
        public static readonly DependencyProperty RelatedJsonDataProperty = RegisterProperty<ColumnTextElementContainer, ColumnTextElementData>();


        public required UIElement LocalizationTextView { get => (UIElement)GetValue(LocalizationTextViewProperty); set => SetValue(LocalizationTextViewProperty, value); }
        public static readonly DependencyProperty LocalizationTextViewProperty = RegisterProperty<ColumnTextElementContainer, UIElement>();


        public Thickness LocalizationTextViewMargin { get => (Thickness)GetValue(LocalizationTextViewMarginProperty); set => SetValue(LocalizationTextViewMarginProperty, value); }
        public static readonly DependencyProperty LocalizationTextViewMarginProperty = RegisterProperty<ColumnTextElementContainer, Thickness>();



        public double VerticalOffset { get => (double)GetValue(VerticalOffsetProperty); set => SetValue(VerticalOffsetProperty, value); }
        public static readonly DependencyProperty VerticalOffsetProperty = RegisterProperty<ColumnTextElementContainer, double>();


        public double ContentHorizontalOffset { get => (double)GetValue(ContentHorizontalOffsetProperty); set => SetValue(ContentHorizontalOffsetProperty, value); }
        public static readonly DependencyProperty ContentHorizontalOffsetProperty = RegisterProperty<ColumnTextElementContainer, double>();



        public string? SignatureText { get => (string?)GetValue(SignatureTextProperty); set => SetValue(SignatureTextProperty, value); }
        public static readonly DependencyProperty SignatureTextProperty = RegisterProperty<ColumnTextElementContainer, string?>();


        public double SignatureText_HOffset { get => (double)GetValue(SignatureText_HOffsetProperty); set => SetValue(SignatureText_HOffsetProperty, value); }
        public static readonly DependencyProperty SignatureText_HOffsetProperty = RegisterProperty<ColumnTextElementContainer, double>();
    }
}