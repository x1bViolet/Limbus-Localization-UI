using System.Globalization;

namespace LCLocalizationInterface.LimbusRegistry
{
    public partial class SkillNameReplicaUIElement : UserControl
    {
        public static readonly DependencyProperty SkillNameProperty = RegisterProperty<SkillNameReplicaUIElement, string>(DefaultValue: "");
        public static readonly DependencyProperty NameMaximumWidthProperty = RegisterProperty<SkillNameReplicaUIElement, double>(DefaultValue: 500.0);

        public static readonly DependencyProperty IconProperty = RegisterProperty<SkillNameReplicaUIElement, ImageSource>();

        public static readonly DependencyProperty RankProperty = RegisterProperty<SkillNameReplicaUIElement, int>(DefaultValue: 1, PropertyChangedEvent: AffinityAndRankUpdater);
        public static readonly DependencyProperty AffinityProperty = RegisterProperty<SkillNameReplicaUIElement, AffinityName>(DefaultValue: AffinityName.None, PropertyChangedEvent: AffinityAndRankUpdater);
        public static readonly DependencyProperty SkillTypeProperty = RegisterProperty<SkillNameReplicaUIElement, SkillType>(DefaultValue: SkillType.Guard);
        public static readonly DependencyProperty DamageTypeProperty = RegisterProperty<SkillNameReplicaUIElement, DamageType>(DefaultValue: DamageType.None);

        public static readonly DependencyProperty CoinsProperty = RegisterProperty<SkillNameReplicaUIElement, string>(DefaultValue: "Regular, \0"); // ", 0" to trigger dependency property changed event and update Binding

        public static readonly DependencyProperty LevelTextProperty = RegisterProperty<SkillNameReplicaUIElement, string>(DefaultValue: "??");
        public static readonly DependencyProperty AttackWeightLabelProperty = RegisterProperty<SkillNameReplicaUIElement, string>(DefaultValue: "Atk Weight");
        public static readonly DependencyProperty AttackWeightProperty = RegisterProperty<SkillNameReplicaUIElement, int>(DefaultValue: 1);

        public static readonly DependencyProperty BasePowerProperty = RegisterProperty<SkillNameReplicaUIElement, string>(DefaultValue: "?");
        public static readonly DependencyProperty CoinPowerProperty = RegisterProperty<SkillNameReplicaUIElement, string>(DefaultValue: "+?");

        #region Technical DependencyProperties
        public static readonly DependencyProperty AffinityAndRankProperty = RegisterProperty<SkillNameReplicaUIElement, string>(DefaultValue: "None|1");
        public static readonly DependencyProperty InternalAffinityColorProperty = RegisterProperty<SkillNameReplicaUIElement, SolidColorBrush>(DefaultValue: ToSolidColorBrush("#9f6a3a"));
        #endregion

        private static void AffinityAndRankUpdater(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
        {
            SkillNameReplicaUIElement ActualSender = (Sender as SkillNameReplicaUIElement)!;
            if (ActualSender.Rank < 1) ActualSender.Rank = 1;
            if (ActualSender.Rank > 3) ActualSender.Rank = 3;

            ActualSender.AffinityAndRank = $"{ActualSender.Affinity}|{((Args.NewValue as string)! == "None|-1" ? "-1" : ActualSender.Rank)}";
            ActualSender.InternalAffinityColor = ToSolidColorBrush(GetAffinityColor($"{ActualSender.Affinity}"));
        }


        public void RefreshNameRichText() => this.FindTypeNameFromTemplate<NameLineElement>("NameLine")!.RefreshNameRichText();


        // Values controlled by the Bindings in MainWindow
        public string SkillName { get => (string)GetValue(SkillNameProperty); set => SetValue(SkillNameProperty, value); }
        public double NameMaximumWidth { get => (double)GetValue(NameMaximumWidthProperty); set => SetValue(NameMaximumWidthProperty, value); }


        public ImageSource Icon { get => (ImageSource)GetValue(IconProperty); set => SetValue(IconProperty, value); }

        public int Rank { get => (int)GetValue(RankProperty); set => SetValue(RankProperty, value); }

        public AffinityName Affinity { get => (AffinityName)GetValue(AffinityProperty); set => SetValue(AffinityProperty, value); }
        public SkillType SkillType { get => (SkillType)GetValue(SkillTypeProperty); set => SetValue(SkillTypeProperty, value); }
        public DamageType DamageType { get => (DamageType)GetValue(DamageTypeProperty); set => SetValue(DamageTypeProperty, value); }

        public string Coins { get => (string)GetValue(CoinsProperty); set => SetValue(CoinsProperty, value); }

        public string LevelText { get => (string)GetValue(LevelTextProperty); set => SetValue(LevelTextProperty, value); }
        public int AttackWeight { get => (int)GetValue(AttackWeightProperty); set => SetValue(AttackWeightProperty, value); }
        public string AttackWeightLabel { get => (string)GetValue(AttackWeightLabelProperty); set => SetValue(AttackWeightLabelProperty, value); }

        public string CoinPower { get => (string)GetValue(CoinPowerProperty); set => SetValue(CoinPowerProperty, value); }
        public string BasePower { get => (string)GetValue(BasePowerProperty); set => SetValue(BasePowerProperty, value); }


        public string AffinityAndRank { get => (string)GetValue(AffinityAndRankProperty); set => SetValue(AffinityAndRankProperty, value); }

        public SolidColorBrush InternalAffinityColor { get => (SolidColorBrush)GetValue(InternalAffinityColorProperty); set => SetValue(InternalAffinityColorProperty, value); }
    }


    /// <summary>
    /// Indeterminator Border with bindable <see cref="CoinsSequence"/> <see cref="DependencyProperty"/> that creates new StackPanel Child with coin images based on given info, original idea was to use Stackpanel Children binding using converter
    /// </summary>
    public class SkillNameReplica_CoinsStackPanelParentBorder : Border
    {
        public static readonly DependencyProperty CoinsSequenceProperty = RegisterProperty<SkillNameReplica_CoinsStackPanelParentBorder, string>(DefaultValue: "Regular, \0", PropertyChangedEvent: OnCoinsSequenceChaged);
        public string CoinsSequence { get => (string)GetValue(CoinsSequenceProperty); set => SetValue(CoinsSequenceProperty, value); }
        private static void OnCoinsSequenceChaged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
        {
            SkillNameReplica_CoinsStackPanelParentBorder ActualSender = (Sender as SkillNameReplica_CoinsStackPanelParentBorder)!;

            List<string> CoinTypesSequence = Args.NewValue is not null ? [.. (Args.NewValue as string)!
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToLower())
                .Where(x => x.EqualsToOneOf("regular", "unbreakable", "excision", "purple"))] : ["Regular"];

            StackPanel CoinsPanel = new() { Orientation = Orientation.Horizontal };
            foreach (string CoinType in CoinTypesSequence)
            {
                CoinsPanel.Children.Add(new Image() { Source = BitmapFromResource($"UI/Limbus/Skills/{CoinType} Coin.png") });
            }

            ActualSender.Child = CoinsPanel;
        }
    }



    namespace LimbusIValueConverters
    {
        public class AffinityAndRankToSkillFrameConverter : IValueConverter
        {
            public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
            {
                string[] Parts = (Value as string)!.Split('|'); (string Affinity, string Rank) = (Parts[0], Parts[1]);

                /// "UI/Limbus/Skills/Frames/Skill Default Frame alt.png" used for unknown Skills (Manually set "None|-1" from <see cref="EditorModesShelf.Types.SkillsEditorMode.ChangeSkillNameReplicaAppearance()"/>)
            
                string ActualImage = Affinity == "None" ? $"Skill Default Frame{(Rank == "-1" ? " alt" : "")}" : $"{Affinity}/{Rank}";

                return BitmapFromResource($"UI/Limbus/Skills/Frames/{ActualImage}.png");
            }
            public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
        }

        public class AttackWeightToCharIconsConverter : IValueConverter
        {
            public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
            {
                int Count = (int)Value;
                return new string('■', Count > 0 ? Count : 1);
            }
            public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
        }
    }
}