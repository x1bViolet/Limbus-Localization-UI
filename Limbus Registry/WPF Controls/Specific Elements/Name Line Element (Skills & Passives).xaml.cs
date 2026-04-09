using System.Globalization;

namespace LCLocalizationInterface.LimbusRegistry
{
    /// <summary>
    /// Recreation the Skills/Passives name with an Affinity-colored background
    /// <br/><br/>
    /// (ControlTemplate at the <c>Limbus Registry/WPF Controls/Specific Elements/Name Line Element (Skills &amp; Passives).xaml</c> ResourceDictionary file)
    /// </summary>
    public partial class NameLineElement : UserControl
    {
        public string DisplayedName { get => (string)GetValue(DisplayedNameProperty); set => SetValue(DisplayedNameProperty, value); }
        public static readonly DependencyProperty DisplayedNameProperty = RegisterProperty<NameLineElement, string>(DefaultValue: "");
        
        public double NameMaximumWidth { get => (double)GetValue(NameMaximumWidthProperty); set => SetValue(NameMaximumWidthProperty, value); }
        public static readonly DependencyProperty NameMaximumWidthProperty = RegisterProperty<NameLineElement, double>(DefaultValue: 500.0);
        
        public AffinityName Affinity { get => (AffinityName)GetValue(AffinityProperty); set => SetValue(AffinityProperty, value); }
        public static readonly DependencyProperty AffinityProperty = RegisterProperty<NameLineElement, AffinityName>(DefaultValue: AffinityName.None, PropertyChangedEvent: FillColorSetter);
        private static void FillColorSetter(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
        {
            (Sender as NameLineElement)!.FillColor = ToSolidColorBrush(GetAffinityColor($"{Args.NewValue}"));
        }


        public double DropShadowEffect_Depth { get => (double)GetValue(DropShadowEffect_DepthProperty); set => SetValue(DropShadowEffect_DepthProperty, value); }
        public static readonly DependencyProperty DropShadowEffect_DepthProperty = RegisterProperty<NameLineElement, double>(DefaultValue: 4.0);
        
        public double DropShadowEffect_Direction { get => (double)GetValue(DropShadowEffect_DirectionProperty); set => SetValue(DropShadowEffect_DirectionProperty, value); }
        public static readonly DependencyProperty DropShadowEffect_DirectionProperty = RegisterProperty<NameLineElement, double>(DefaultValue: -40.0);


        #region Technical DependencyProperties
        public ImageSource CornerImageSource { get => (ImageSource)GetValue(CornerImageSourceProperty); set => SetValue(CornerImageSourceProperty, value); }
        public static readonly DependencyProperty CornerImageSourceProperty = RegisterProperty<NameLineElement, ImageSource>();
        
        public SolidColorBrush FillColor { get => (SolidColorBrush)GetValue(FillColorProperty); set => SetValue(FillColorProperty, value); }
        public static readonly DependencyProperty FillColorProperty = RegisterProperty<NameLineElement, SolidColorBrush>(DefaultValue: ToSolidColorBrush("#9f6a3a"));
        
        public double FillColorRightOffsetReduction { get => (double)GetValue(FillColorRightOffsetReductionProperty); set => SetValue(FillColorRightOffsetReductionProperty, value); }
        public static readonly DependencyProperty FillColorRightOffsetReductionProperty = RegisterProperty<NameLineElement, double>();
        #endregion


        public void RefreshNameRichText() => this.FindTypeNameFromTemplate<TMProEmitter>("NameDisplaying")?.RefreshRichText();
    }

    namespace LimbusIValueConverters
    {
        public class FillColorRightOffsetReductionConverter : IValueConverter
        {
            public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
            {
                double Offset = (double)Value; return new Thickness(Offset > 0 ? -Offset : 0, 0, 0, 0);
            }
            public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
        }

        public class AffinityToNamePartImageConverter : IValueConverter
        {
            public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
            {
                return BitmapFromResource($"UI/Limbus/Skills/Name Background/{Value}.png");
            }
            public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
        }
    }
}