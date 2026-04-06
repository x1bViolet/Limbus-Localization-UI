using LCLocalizationInterface.LimbusRegistry.JsonTypes;

namespace LCLocalizationInterface.LimbusRegistry
{
    public partial class BattleKeywordContainer : UserControl
    {
        public BattleKeywordContainer() => InitializeComponent();


        public static ToolTip CreateTooltip(PlainKeyword Keyword)
        {
            string Name = Keyword.Name;
            string Description = Keyword.MainDescription;
            string Flavor = Keyword.FlavorDescription ?? "";

            BitmapImage Icon = ImageDictionaries.UnknownSpriteImage;

            if (Keyword.ID is not null)
            {
                Icon = ImageDictionaries.KeywordImages[Keyword.ID];

                if (Icon == ImageDictionaries.UnknownSpriteImage && ImageDictionaries.NotSuitableForSpriteTagRedirections.TryGetValue(Keyword.ID, out string? FoundAnotherSpriteID))
                {
                    // Try get from NotSuitableForSpriteTag redirections then
                    Icon = ImageDictionaries.KeywordImages[FoundAnotherSpriteID];
                }
            }

            ToolTip KeywordTooltip = new()
            {
                Margin = new Thickness(0, 13, 0, 0),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                LayoutTransform = new ScaleTransform(0.89, 0.89),
                Content = new BattleKeywordContainer()
                {
                    KeywordIcon = Icon,
                    KeywordName = Name,
                    KeywordDesc = Description,
                    KeywordFlavor = Flavor,
                }
            };

            return KeywordTooltip;
        }


        public ImageSource KeywordIcon { get => (ImageSource)GetValue(KeywordIconProperty); set => SetValue(KeywordIconProperty, value); }
        public static readonly DependencyProperty KeywordIconProperty = RegisterProperty<BattleKeywordContainer, ImageSource>(DefaultValue: ImageDictionaries.UnknownSpriteImage);

        public string KeywordName { get => (string)GetValue(KeywordNameProperty); set => SetValue(KeywordNameProperty, value); }
        public static readonly DependencyProperty KeywordNameProperty = RegisterProperty<BattleKeywordContainer, string>(DefaultValue: "Unknown");

        public string KeywordDesc { get => (string)GetValue(KeywordDescProperty); set => SetValue(KeywordDescProperty, value); }
        public static readonly DependencyProperty KeywordDescProperty = RegisterProperty<BattleKeywordContainer, string>(DefaultValue: "Unknown");

        public string? KeywordFlavor { get => (string?)GetValue(KeywordFlavorProperty); set => SetValue(KeywordFlavorProperty, value); }
        public static readonly DependencyProperty KeywordFlavorProperty = RegisterProperty<BattleKeywordContainer, string?>();
    }
}