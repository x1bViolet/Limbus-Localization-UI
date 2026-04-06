using System.Globalization;

namespace LCLocalizationInterface.Internal.IValueConverters
{
    public class BoolInversionConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture) => Value is bool Boolean ? !(Boolean) : Value;
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
    }
    public class TextToColorConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture) => ToColor($"{Value}");
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
    }
    public class TextToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture) => ToSolidColorBrush($"{Value}");
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
    }

    public class DoubleToTopMarginConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture) => new Thickness(0, (double)Value, 0, 0);
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
    }
    public class DoubleToRightMarginConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture) => new Thickness((double)Value, 0, 0, 0);
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
    }

    public class DoubleToNegativeTopMarginConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture) => new Thickness(0, -(double)Value, 0, 0);
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
    }
}