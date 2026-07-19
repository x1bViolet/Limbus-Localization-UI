using System.Windows.Markup;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage.ImageInfoJsonFile.TextColumns_PROP;

namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public class TextElementsColumn : VirtualizingStackPanel;

    /// <summary>
    /// <see cref="PlaceSkillAt"/>, <see cref="GetSkillAt"/>, <see cref="SkillsCountInColumn"/>
    /// </summary>
    public class SummarySkillTextElementsGrid : Grid
    {
        public SummarySkillTextElementsGrid()
        {
            for (int i = 0; i <= 4; i++) this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(145) });
            for (int i = 0; i <= 50; i++) this.RowDefinitions.Add(new RowDefinition() { MinHeight = 145 });
            this.Width = 580;
        }

        public void PlaceSkillAt(int RowIndex, int ColumnIndex, UIElement Element)
        {
            this.Children.Remove(this.GetSkillAt(RowIndex, ColumnIndex));

            Grid.SetColumn(Element, ColumnIndex);
            Grid.SetRow(Element, RowIndex);
            this.Children.Add(Element);
        }
        public ColumnTextElementContainer? GetSkillAt(int RowIndex, int ColumnIndex)
        {
            return this.Children.OfType<ColumnTextElementContainer>().FirstOrDefault(x => Grid.GetRow(x) == RowIndex & Grid.GetColumn(x) == ColumnIndex);
        }
        public int SkillsCountInColumn(int ColumnIndex)
        {
            return this.Children.OfType<ColumnTextElementContainer>().Count(x => Grid.GetColumn(x) == ColumnIndex);
        }
    }
    

    [ContentProperty(nameof(LocalizationTextView))]
    public partial class ColumnTextElementContainer : UserControl
    {
        /// <summary>
        /// <paramref name="CancelDefaultOnLoadSeal"/> needed to cancel <see cref="SealLocalizationTextView"/> call before <see cref="OnIsSummaryChanged"/> if it was
        /// </summary>
        public ColumnTextElementContainer()
        {
            this.Loaded += SealOnLoad;
            this.IsVisibleChanged += ColumnTextElementContainer_IsVisibleChanged;
        }

        private static void SealOnLoad(object Sender, RoutedEventArgs Args) => (Sender as ColumnTextElementContainer)!.SealLocalizationTextView();


        public TextElementsColumn ParentColumn => (this.Parent as TextElementsColumn)!;


        #region Sealing
        ~ColumnTextElementContainer() => App.HandledInvoke(this.UnsealLocalizationTextView);

        private Image TemplateSealedView => this.FindTypeNameFromTemplate<Image>("TemplateSealedView")!;
        private Grid TemplateContentGrid => this.FindTypeNameFromTemplate<Grid>("PART_TemplateContentGrid")!;
        public bool IsViewSealed => this.TemplateSealedView?.Visibility is Visibility.Visible;


        /// <summary>
        /// Convert UI element view to static image and then hide UI element view, huge performance benefit in case with Skills and Passives + their Signature with drop shadow within <see cref="CautionsTextElement"/> template + Cautions (<see cref="DropShadowEffect"/> has a particularly strong impact on scrolling performance, I think it's really its fault)<br/>
        /// • <see cref="BattleKeywordContainer_PCE"/>s are unaffected because they have no impact on performance (i.e. small text in general + no drop shadows within <see cref="BattleKeywordContainer_PCE"/> template)<br/>
        /// • Template must be already applied at the moment of execution of this method (i.e. <c>`IsLoaded == <see langword="true"/>`</c> or <c>`Template != <see langword="null"/>`</c> or <c>`CautionsTextElement.Loaded += (_, _) => CautionsTextElement.SealLocalizationTextView()`</c>)
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

                    //if (this.IsSummaryView == false) await Task.Delay(1000);

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


        /// <summary>
        /// Affects only Passives (Reduces size of <see cref="SignatureText"/>) and Skills (Enables alternate version of <see cref="SignatureText"/>)
        /// </summary>
        public bool IsSummaryView { get => (bool)GetValue(IsSummaryViewProperty); set => SetValue(IsSummaryViewProperty, value); }
        public static readonly DependencyProperty IsSummaryViewProperty = RegisterProperty<ColumnTextElementContainer, bool>(DefaultValue: false, PropertyChangedEvent: OnIsSummaryChanged);

        /// <summary>
        /// <see cref="FrameworkElement.Loaded"/> goes wrong somehow and fires even if this element in <see cref="Visibility.Collapsed"/> area
        /// </summary>
        public List<Action> FirstTimeShowingActions = [];
        private bool WasShownForTheFirstTime = false;
        private void ColumnTextElementContainer_IsVisibleChanged(object Sender, DependencyPropertyChangedEventArgs Args)
        {
            if (WasShownForTheFirstTime == false && (bool)Args.NewValue == true)
            {
                WasShownForTheFirstTime = true;
                FirstTimeShowingActions.ForEach(FirstTimeShowingAction => FirstTimeShowingAction?.Invoke());
            }
        }
        /// <summary>
        /// I fuckin hate ControlTemplate MultiDataTriggers headache it just doesnt work even if all Condition values from Bindings is correct
        /// </summary>
        private static void OnIsSummaryChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
        {
            ColumnTextElementContainer ActualSender = (Sender as ColumnTextElementContainer)!;

            ActualSender.Loaded -= SealOnLoad; // Prevent sealing on load before CheckAndSet call

            async void CheckAndSet()
            {
                if (ActualSender.TemplateContentGrid is null)
                {
                    await Task.Delay(50); // Idk template is empty when ui element was shown
                }

                Grid PART_SignatureGrid = ActualSender.FindTypeNameFromTemplate<Grid>("PART_SignatureGrid")!;
                TextBlock PART_SignatureText = ActualSender.FindTypeNameFromTemplate<TextBlock>("PART_SignatureText")!;
                TextBlock PART_AlternativeSignatureForSkillSummaryView = ActualSender.FindTypeNameFromTemplate<TextBlock>("PART_AlternativeSignatureForSkillSummaryView")!;

                // Reset
                PART_SignatureGrid.Visibility = Visibility.Visible;
                PART_SignatureText.FontSize = 31;
                PART_SignatureText.SetRightMargin(32);
                PART_AlternativeSignatureForSkillSummaryView.Visibility = Visibility.Collapsed;

                if ((bool)Args.NewValue == true)
                {
                    if (ActualSender.RelatedJsonData.Type == ColumnTextElementType.Skill)
                    {
                        PART_SignatureGrid.Visibility = Visibility.Collapsed;
                        PART_AlternativeSignatureForSkillSummaryView.Visibility = Visibility.Visible;
                    }
                    else if (ActualSender.RelatedJsonData.Type == ColumnTextElementType.Passive)
                    {
                        PART_SignatureText.FontSize = 26;
                        PART_SignatureText.SetRightMargin(135);
                    }
                }

                await Task.Delay(1000);

                ActualSender.SealLocalizationTextView();
            }

            if (ActualSender.IsLoaded == false)
            {
                ActualSender.FirstTimeShowingActions.Add(CheckAndSet);
            }
            else
            {
                CheckAndSet();
            }
        }


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