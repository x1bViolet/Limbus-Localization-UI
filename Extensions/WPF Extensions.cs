using System.Globalization;

namespace LCLocalizationInterface
{
    namespace MarkupExtensions
    {
        public static partial class MarkupShrinking
        {
            #region ComboBoxDropDownWidth
            /// <summary>
            /// Used by ComboBox ControlTempalte in this program to set Auto width for Popup instead of fixed Width value 
            /// </summary>
            public static readonly DependencyProperty FreeComboBoxDropDownWidthProperty = RegisterAttachedProperty(PropertyType: typeof(bool), OwnerType: typeof(MarkupShrinking), DefaultValue: false);
            public static bool GetFreeComboBoxDropDownWidth(DependencyObject Sender) => (bool)Sender.GetValue(FreeComboBoxDropDownWidthProperty);
            public static void SetFreeComboBoxDropDownWidth(DependencyObject Sender, bool Value) => Sender.SetValue(FreeComboBoxDropDownWidthProperty, Value);
            #endregion


            #region ScrollViewerVerticalOffset
            public static readonly DependencyProperty ScrollViewerVerticalOffsetProperty = RegisterAttachedProperty(
                PropertyType: typeof(double), OwnerType: typeof(MarkupShrinking), DefaultValue: 0.0,
                PropertyChangedEvent: delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
                {
                    if (Sender is ScrollViewer ScrollViewer)
                    {
                        double Value = (double)Args.NewValue;
                        ScrollViewer.ScrollToVerticalOffset(Value);
                    }
                }
            );
            public static double GetScrollViewerVerticalOffset(DependencyObject Sender) => (double)Sender.GetValue(ScrollViewerVerticalOffsetProperty);
            public static void SetScrollViewerVerticalOffset(DependencyObject Sender, double Value) => Sender.SetValue(ScrollViewerVerticalOffsetProperty, Value);
            #endregion



            #region ScrollViewerHorizontalOffset
            public static readonly DependencyProperty ScrollViewerHorizontalOffsetProperty = RegisterAttachedProperty(
                PropertyType: typeof(double), OwnerType: typeof(MarkupShrinking), DefaultValue: 0.0,
                PropertyChangedEvent: delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
                {
                    if (Sender is ScrollViewer ScrollViewer)
                    {
                        double Value = (double)Args.NewValue;
                        ScrollViewer.ScrollToHorizontalOffset(Value);
                    }
                }
            );
            public static double GetScrollViewerHorizontalOffset(DependencyObject Sender) => (double)Sender.GetValue(ScrollViewerHorizontalOffsetProperty);
            public static void SetScrollViewerHorizontalOffset(DependencyObject Sender, double Value) => Sender.SetValue(ScrollViewerHorizontalOffsetProperty, Value);
            #endregion
        }



        public static class NullStateHandler
        {
            /// <summary>
            /// Set <see cref="UIElement.IsEnabled"/> to <see langword="false"/> if value of this property is <see langword="null"/>, can be used with {Binding}
            /// </summary>
            public static readonly DependencyProperty IsEnabledProperty = RegisterAttachedProperty(
                PropertyType: typeof(object), OwnerType: typeof(NullStateHandler), DefaultValue: "\0 asd \0 (Dummy to trigger PropertyChangedEvent)",
                PropertyChangedEvent: delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
                {
                    if (Sender is UIElement UIElement)
                    {
                        UIElement.IsEnabled = Args.NewValue is not null;
                    }
                }
            );
            public static object? GetIsEnabled(DependencyObject Sender) => (object?)Sender.GetValue(IsEnabledProperty);
            public static void SetIsEnabled(DependencyObject Sender, object? Value) => Sender.SetValue(IsEnabledProperty, Value);


            /// <summary>
            /// Set <see cref="UIElement.Visibility"/> to <see cref="Visibility.Collapsed"/> if value of this property is <see langword="null"/>, can be used with {Binding}
            /// </summary>
            public static readonly DependencyProperty IsVisibleProperty = RegisterAttachedProperty(
                PropertyType: typeof(object), OwnerType: typeof(NullStateHandler), DefaultValue: "\0 asd \0 (Dummy to trigger PropertyChangedEvent)",
                PropertyChangedEvent: delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
                {
                    if (Sender is UIElement UIElement)
                    {
                        UIElement.Visibility = Args.NewValue is not null ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            );
            public static object? GetIsVisible(DependencyObject Sender) => (object?)Sender.GetValue(IsVisibleProperty);
            public static void SetIsVisible(DependencyObject Sender, object? Value) => Sender.SetValue(IsVisibleProperty, Value);
        }

        /// <summary><see cref="Binding"/> with '<see langword="null"/>' as FallbackValue</summary>
        public class NullStateBinding : Binding
        {
            public NullStateBinding(string path) : base(path)
            {
                this.Path = new PropertyPath(path);
                this.FallbackValue = (object)null!;
            }
        }



        public static class BooleanStateHandler
        {
            /// <summary>
            /// Set <see cref="UIElement.Visibility"/> to <see cref="Visibility.Collapsed"/> if value of this property is <see langword="false"/>, can be used with {Binding}
            /// </summary>
            public static readonly DependencyProperty IsVisibleProperty = RegisterAttachedProperty(typeof(bool?), typeof(BooleanStateHandler), null, delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                if (Sender is UIElement UIElement)
                {
                    UIElement.Visibility = ((bool?)Args.NewValue ?? false) ? Visibility.Visible : Visibility.Collapsed;
                }
            });
            public static bool? GetIsVisible(DependencyObject Sender) => (bool?)Sender.GetValue(IsVisibleProperty);
            public static void SetIsVisible(DependencyObject Sender, bool? Value) => Sender.SetValue(IsVisibleProperty, Value);
        }

        /// <summary><see cref="Binding"/> with '<see langword="false"/>' as FallbackValue</summary>
        public class BooleanStateBinding : Binding
        {
            public BooleanStateBinding(string path) : base(path)
            {
                this.Path = new PropertyPath(path);
                this.FallbackValue = (bool) false!;
            }
        }



        public class EqualityStateBinding : Binding
        {
            private class EqualityConverter() : IValueConverter
            {
                public bool Inverted { get; set; } = false;
                public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture) => object.Equals(Value, Parameter) == (Inverted ? false : true);
                public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
            }

            public object? EqualityObject { get; set => field = this.ConverterParameter = value; }
            public bool Inverted { get; set => field = (this.Converter as EqualityConverter)!.Inverted = value; } = false;

            public EqualityStateBinding() : base()
            {
                this.Converter = new EqualityConverter();
            }
        }


        public class PathExistanceBinding : Binding
        {
            private class PathExistanceConverter() : IValueConverter
            {
                public IntenseStareType3.PathType CheckType { get; set; } = IntenseStareType3.PathType.None;
                public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
                {
                    Func<string, bool> ExistanceCheckProvider = this.CheckType switch
                    {
                        IntenseStareType3.PathType.File => File.Exists,
                        IntenseStareType3.PathType.Directory => Directory.Exists,
                        _ => delegate (string Path) { return false; }
                    };

                    return ExistanceCheckProvider((string)Value);
                }
                public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
            }

            public IntenseStareType3.PathType CheckType { get; set => field = (this.Converter as PathExistanceConverter)!.CheckType = value; } = IntenseStareType3.PathType.None;

            public PathExistanceBinding() : base()
            {
                this.Converter = new PathExistanceConverter();
            }
        }
    }


    public static class WPFExtensions
    {
        extension(UIElement UIElement)
        {
            public bool Visible
            {
                get => UIElement.Visibility == Visibility.Visible;
                set => UIElement.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}