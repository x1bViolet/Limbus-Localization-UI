namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public partial class BattleKeywordContainer_PCE : UserControl
    {
        public BattleKeywordContainer_PCE() => InitializeComponent();


        public ImageSource KeywordIcon { get => (ImageSource)GetValue(KeywordIconProperty); set => SetValue(KeywordIconProperty, value); }
        public static readonly DependencyProperty KeywordIconProperty = RegisterProperty<BattleKeywordContainer_PCE, ImageSource>(DefaultValue: ImageDictionaries.UnknownSpriteImage);

        public string KeywordName { get => (string)GetValue(KeywordNameProperty); set => SetValue(KeywordNameProperty, value); }
        public static readonly DependencyProperty KeywordNameProperty = RegisterProperty<BattleKeywordContainer_PCE, string>(DefaultValue: "Unknown");

        public string KeywordDesc { get => (string)GetValue(KeywordDescProperty); set => SetValue(KeywordDescProperty, value); }
        public static readonly DependencyProperty KeywordDescProperty = RegisterProperty<BattleKeywordContainer_PCE, string>(DefaultValue: "Unknown");

        public string? KeywordFlavor { get => (string?)GetValue(KeywordFlavorProperty); set => SetValue(KeywordFlavorProperty, value); }
        public static readonly DependencyProperty KeywordFlavorProperty = RegisterProperty<BattleKeywordContainer_PCE, string?>();


        public double KeywordName_MaxWidth { get => (double)GetValue(KeywordName_MaxWidthProperty); set => SetValue(KeywordName_MaxWidthProperty, value); }
        public static readonly DependencyProperty KeywordName_MaxWidthProperty = RegisterProperty<BattleKeywordContainer_PCE, double>(DefaultValue: double.PositiveInfinity);
    }
}
