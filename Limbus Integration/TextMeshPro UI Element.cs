using System.Windows;
using System.Windows.Controls;
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
            set
            {
                if (DisableKeyworLinksCreation == false)
                {
                    // Do not let keyword tooltips grab last target place
                    LimbusPreviewFormatter.LastAppliedUnformattedRichText = value;
                    LimbusPreviewFormatter.LastApplyTarget = this;
                }

                Pocket_Watch_ː_Type_L.Actions.Apply(
                    Target: this,
                    RichText: LimbusPreviewFormatter.Apply(PreviewText: value, SpecifiedTextProcessingMode: this.LimbusPreviewFormattingMode),
                    DividersMode: Pocket_Watch_ː_Type_L.@PostInfo.FullStopDividers.FullStopDividers_TMPro,
                    IgnoreTags: Pocket_Watch_ː_Type_L.@PostInfo.IgnoreTags_UnityTMProExclude,
                    DisableKeyworLinksCreation: this.DisableKeyworLinksCreation // ୧((#Φ益Φ#))୨
                );

                CurrentRichText = value;
            }
        }
    }
}
