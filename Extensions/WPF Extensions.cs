namespace LCLocalizationInterface
{
    namespace MarkupExtensions
    {
        public static partial class MarkupShrinking
        {
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


            /// <summary>
            /// Set <see cref="UIElement.IsEnabled"/> to <see langword="false"/> if value of this property is <see langword="false"/>, can be used with {Binding}
            /// </summary>
            public static readonly DependencyProperty IsEnabledProperty = RegisterAttachedProperty(typeof(bool?), typeof(BooleanStateHandler), null, delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                if (Sender is UIElement UIElement)
                {
                    UIElement.IsEnabled = (bool?)Args.NewValue ?? false;
                }
            });
            public static bool? GetIsEnabled(DependencyObject Sender) => (bool?)Sender.GetValue(IsEnabledProperty);
            public static void SetIsEnabled(DependencyObject Sender, bool? Value) => Sender.SetValue(IsEnabledProperty, Value);
        }






        /// <summary><see cref="Binding"/> with '<see langword="null"/>' as FallbackValue</summary>
        public class NullStateBinding : Binding
        {
            public NullStateBinding(string path) : base(path)
            {
                this.Path = new PropertyPath(path);
                this.FallbackValue = (object) null!;
            }
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