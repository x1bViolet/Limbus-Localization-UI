namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public partial class PassiveView : UserControl
    {
        public PassiveView() => InitializeComponent();


        public string PassiveName { get => (string)GetValue(PassiveNameProperty); set => SetValue(PassiveNameProperty, value); }
        public static readonly DependencyProperty PassiveNameProperty = RegisterProperty<PassiveView, string>(DefaultValue: "Unknown");

        public string PassiveDesc { get => (string)GetValue(PassiveDescProperty); set => SetValue(PassiveDescProperty, value); }
        public static readonly DependencyProperty PassiveDescProperty = RegisterProperty<PassiveView, string>(DefaultValue: "Unknown");

        public string? PassiveFlavor { get => (string?)GetValue(PassiveFlavorProperty); set => SetValue(PassiveFlavorProperty, value); }
        public static readonly DependencyProperty PassiveFlavorProperty = RegisterProperty<PassiveView, string?>();


        public double PassiveName_MaxWidth { get => (double)GetValue(PassiveName_MaxWidthProperty); set => SetValue(PassiveName_MaxWidthProperty, value); }
        public static readonly DependencyProperty PassiveName_MaxWidthProperty = RegisterProperty<PassiveView, double>(DefaultValue: double.PositiveInfinity);

        public double PassiveDesc_MaxWidth { get => (double)GetValue(PassiveDesc_MaxWidthProperty); set => SetValue(PassiveDesc_MaxWidthProperty, value); }
        public static readonly DependencyProperty PassiveDesc_MaxWidthProperty = RegisterProperty<PassiveView, double>(DefaultValue: double.PositiveInfinity);
    }
}
