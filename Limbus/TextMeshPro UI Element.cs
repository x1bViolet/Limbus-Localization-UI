using System.Windows;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    /// <summary>
    /// TextBlock where <paramref name="RichText"/> property leads to the <see cref="Pocket_Watch_ː_Type_L.Actions.Apply"/>
    /// </summary>
    public partial class TMProEmitter : TextBlock
    {
        public bool DisableKeyworLinksCreation { get; set; } = false; // Prevent endless keyword tooltips creation for keywords inside keywords tooltips inside tooltips for keywords inside tooltips
        
        public TMProEmitter()
        {
            Foreground = ToSolidColorBrush("#EBCAA2");
            TextWrapping = TextWrapping.Wrap;
            LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
        }



        #region Font
        public enum LimbusFontTypes
        {
            Context,
            Title,
            None,
        }

        #pragma AVOID any 'TargetType only' Styles for TMProEmitters becuase of possible keyword tooltips inside (KeywordsInterrogation.KeywordDescriptionInfoPopup), they will interhint it and weird things can happen ("CoinDesc" 'TargetType only' Style previously broke font in keyword tooltips and distorted the switching between skills)
        public static readonly DependencyProperty FontTypeProperty = Register<TMProEmitter, LimbusFontTypes>(nameof(FontType), LimbusFontTypes.None, OnFontTypeChanged);
        public static void OnFontTypeChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            // Limbus:FontType="" set from XAML Designer
            (CurrentElement as TMProEmitter).FontType = (LimbusFontTypes)ChangeArgs.NewValue;
        }
        private LimbusFontTypes CurrentFontType;
        public  LimbusFontTypes FontType
        {
            get => CurrentFontType;
            set
            {
                // Console.WriteLine($"Setting {{DynamicResource Limbus:{value}Font}} for TMProEmitter `{this.Name}`");

                // Working value = "Context" / "Title"
                // Resource name is from 'Limbus/Font References.xaml' ResourceDictionary with fonts (Included at the App.xaml), "None" ~= Segoe UI because of null font from ResourceReference
                this.SetResourceReference(TMProEmitter.FontFamilyProperty, $"Limbus:{value}Font");
                this.SetResourceReference(TMProEmitter.FontWeightProperty, $"Limbus:{value}Font_Weight");

                CurrentFontType = value;
            }
        }
        #endregion



        #region Text processing mode
        public static readonly DependencyProperty TextProcessingModeProperty = Register<TMProEmitter, EditorMode>(nameof(TextProcessingMode), EditorMode.UseCurrentActiveProperties);
        public required EditorMode TextProcessingMode
        {
            get => (EditorMode)GetValue(TextProcessingModeProperty);
            set => SetValue(TextProcessingModeProperty, value);
        }
        #endregion



        #region Rich text
        private string CurrentRichText;
        public string RichText
        {
            get => CurrentRichText;
            set
            {
                string InputRichText = value;

                if (this.DisableKeyworLinksCreation == false & this.Name.StartsWith("PreviewLayout_") /*If not name text or keyword tooltip or something else*/)
                {
                    // Do not let keyword tooltips or names with "DisableKeyworLinksCreation = true" grab the last target place
                    LimbusPreviewFormatter.LastAppliedUnformattedRichText = InputRichText;
                    LimbusPreviewFormatter.LastApplyTarget = this;
                }

                Pocket_Watch_ː_Type_L.Actions.Apply(
                    Target: this,
                    RichText: LimbusPreviewFormatter.Apply(LimbusText: InputRichText, SpecifiedTextProcessingMode: this.TextProcessingMode),
                    DividersMode: Pocket_Watch_ː_Type_L.@PostInfo.FullStopDividers.FullStopDividers_TMPro,
                    IgnoreTags: Pocket_Watch_ː_Type_L.@PostInfo.IgnoreTags_UnityTMProExclude,
                    DisableKeyworLinksCreation: this.DisableKeyworLinksCreation // ୧((#Φ益Φ#))୨
                );

                CurrentRichText = value;
            }
        }
        #endregion
    }
}
