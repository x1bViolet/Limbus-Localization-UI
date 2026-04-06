using System.Windows.Markup;

namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    /// <summary>Special version with modified layout exclusively for Preview Creator</summary>
    [ContentProperty(nameof(SkillLocalizationTextView))]
    public partial class SkillNameReplicaUIElement_PCE : UserControl
    {
        public UIElement SkillLocalizationTextView { get => (UIElement)GetValue(SkillLocalizationTextViewProperty); set => SetValue(SkillLocalizationTextViewProperty, value); }
        public static readonly DependencyProperty SkillLocalizationTextViewProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, UIElement>();


        public string SkillName { get => (string)GetValue(SkillNameProperty); set => SetValue(SkillNameProperty, value); }
        public static readonly DependencyProperty SkillNameProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, string>(DefaultValue: "");
        
        public double NameMaximumWidth { get => (double)GetValue(NameMaximumWidthProperty); set => SetValue(NameMaximumWidthProperty, value); }
        public static readonly DependencyProperty NameMaximumWidthProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, double>(DefaultValue: 450.0);
        
        public bool ShowAffinityIcon { get => (bool)GetValue(ShowAffinityIconProperty); set => SetValue(ShowAffinityIconProperty, value); }
        public static readonly DependencyProperty ShowAffinityIconProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, bool>(DefaultValue: true);

        public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }
        public static readonly DependencyProperty IconProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, ImageSource>();

        public int Rank { get => (int)GetValue(RankProperty); set => SetValue(RankProperty, value); }
        public static readonly DependencyProperty RankProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, int>(DefaultValue: 1, PropertyChangedEvent: AffinityAndRankUpdater);
        
        public AffinityName Affinity { get => (AffinityName)GetValue(AffinityProperty); set => SetValue(AffinityProperty, value); }
        public static readonly DependencyProperty AffinityProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, AffinityName>(DefaultValue: AffinityName.None, PropertyChangedEvent: AffinityAndRankUpdater);
        
        public SkillType SkillType { get => (SkillType)GetValue(SkillTypeProperty); set => SetValue(SkillTypeProperty, value); }
        public static readonly DependencyProperty SkillTypeProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, SkillType>(DefaultValue: SkillType.Guard);
        
        public DamageType DamageType { get => (DamageType)GetValue(DamageTypeProperty); set => SetValue(DamageTypeProperty, value); }
        public static readonly DependencyProperty DamageTypeProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, DamageType>(DefaultValue: DamageType.None);

        public string Coins { get => (string)GetValue(CoinsProperty); set => SetValue(CoinsProperty, value); }
        public static readonly DependencyProperty CoinsProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, string>(DefaultValue: "Regular, \0");


        #region Technical DependencyProperties used in ControlTemplate Bindings
        public string AffinityAndRank { get => (string)GetValue(AffinityAndRankProperty); set => SetValue(AffinityAndRankProperty, value); }
        public static readonly DependencyProperty AffinityAndRankProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, string>(DefaultValue: "None|1");

        public SolidColorBrush InternalAffinityColor { get => (SolidColorBrush)GetValue(InternalAffinityColorProperty); set => SetValue(InternalAffinityColorProperty, value); }
        public static readonly DependencyProperty InternalAffinityColorProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, SolidColorBrush>(DefaultValue: ToSolidColorBrush("#9f6a3a"));
        
        public ImageSource InternalAffinityIcon { get => (ImageSource)GetValue(InternalAffinityIconProperty); set => SetValue(InternalAffinityIconProperty, value); }
        public static readonly DependencyProperty InternalAffinityIconProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, ImageSource>();
        #endregion




        /// <summary>
        /// Sets <see cref="AffinityAndRank"/> value as "Affinity|Rank" to trigger {TemplateBindings AffinityAndRank} inside ControlTemplate
        /// </summary>
        private static void AffinityAndRankUpdater(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
        {
            SkillNameReplicaUIElement_PCE ActualSender = (Sender as SkillNameReplicaUIElement_PCE)!;
            if (ActualSender.Rank < 1) ActualSender.Rank = 1;
            if (ActualSender.Rank > 3) ActualSender.Rank = 3;

            ActualSender.AffinityAndRank = $"{ActualSender.Affinity}|{ActualSender.Rank}";
            ActualSender.InternalAffinityColor = ToSolidColorBrush(GetAffinityColor($"{ActualSender.Affinity}"));

            ActualSender.InternalAffinityIcon = ActualSender.Affinity != AffinityName.None
                ? BitmapFromResource($"UI/Limbus/Skills/Affinity Icons/{ActualSender.Affinity}.png")
                : new BitmapImage();
        }
    }
}