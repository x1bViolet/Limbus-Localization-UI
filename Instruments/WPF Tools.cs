using System.Globalization;

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
    

    namespace Instruments
    {
        public static partial class WPFTools
        {
            public static string ThisAssembly { get; } = Assembly.GetExecutingAssembly().GetName().Name!;

            extension(UIElement UIElement)
            {
                public bool Visible
                {
                    get => UIElement.Visibility == Visibility.Visible;
                    set => UIElement.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                }
            }





            #region Collections Move Up/Down

            #region ItemCollection
            public static void MoveItemUp(this ItemCollection ParentItemCollection, UIElement TargetElement)
            {
                int CurrentIndex = ParentItemCollection.IndexOf(TargetElement);
                if (CurrentIndex > 0)
                {
                    ParentItemCollection.RemoveAt(CurrentIndex);
                    ParentItemCollection.Insert(CurrentIndex - 1, TargetElement);
                }
            }
            public static void MoveItemDown(this ItemCollection ParentItemCollection, UIElement TargetElement)
            {
                int CurrentIndex = ParentItemCollection.IndexOf(TargetElement);
                if (CurrentIndex >= 0 && CurrentIndex < ParentItemCollection.Count - 1)
                {
                    ParentItemCollection.RemoveAt(CurrentIndex);
                    ParentItemCollection.Insert(CurrentIndex + 1, TargetElement);
                }
            }
            #endregion

            #region UIElementCollection
            public static void MoveItemUp(this UIElementCollection ParentUIElementCollection, UIElement TargetElement)
            {
                int CurrentIndex = ParentUIElementCollection.IndexOf(TargetElement);
                if (CurrentIndex > 0)
                {
                    ParentUIElementCollection.RemoveAt(CurrentIndex);
                    ParentUIElementCollection.Insert(CurrentIndex - 1, TargetElement);
                }
            }
            public static void MoveItemDown(this UIElementCollection ParentUIElementCollection, UIElement TargetElement)
            {
                int CurrentIndex = ParentUIElementCollection.IndexOf(TargetElement);
                if (CurrentIndex >= 0 && CurrentIndex < ParentUIElementCollection.Count - 1)
                {
                    ParentUIElementCollection.RemoveAt(CurrentIndex);
                    ParentUIElementCollection.Insert(CurrentIndex + 1, TargetElement);
                }
            }
            #endregion

            #endregion





            #region Assets loading

            #region Fonts
            /// <returns>
            /// <see cref="FontFamily"/> created from file using <see cref="GetFontFamilyFromFile"/>, otherwise just <see langword="new"/> <see cref="FontFamily"/>(<paramref name="FontPathOrName"/>) if <paramref name="FontPathOrName"/> is not a file path
            /// </returns>
            public static FontFamily FontFamilyFromFileOrName(string FontPathOrName)
            {
                return File.Exists(FontPathOrName) ? GetFontFamilyFromFile(FontPathOrName) : new FontFamily(FontPathOrName);
            }

            /// <returns>
            /// <see cref="FontFamily"/> created from file using <see cref="System.Windows.Media.Fonts.GetFontFamilies(string)"/>'s first value
            /// </returns>
            public static FontFamily GetFontFamilyFromFile(string FontFilePath)
            {
                return System.Windows.Media.Fonts.GetFontFamilies(Path.GetFullPath(FontFilePath))?.FirstOrDefault() ?? new FontFamily();
            }

            /// <returns>
            /// First value from <see cref="FontFamily.FamilyNames"/> of <see cref="FontFamily"/> acquired through <see cref="GetFontFamilyFromFile"/>
            /// </returns>
            public static string GetFontFriendlyNameFromFile(string FontFilePath)
            {
                return GetFontFamilyFromFile(FontFilePath).FamilyNames?.Values.FirstOrDefault() ?? "Unknown";
            }

            /// <returns>
            /// <see cref="FontFamily"/> from <see cref="GetFontFamilyFromFile"/> converted to string (e.g. <c>"file:///C:/Some/Fonts/KOTRA_BOLD.ttf#KOTRA"</c>)
            /// </returns>
            public static string GetFontFamilyUriPointerPath(string FontFilePath)
            {
                return GetFontFamilyFromFile(FontFilePath).ToString();
            }

            /// <param name="UnicodeRanges">A string in the format "XXXX-YYYY, XXXX-YYYY, ..." where X and Y are the start and end points of the Unicode ranges (Like "4E00-9FCB, F900-FA6A, 2E80-2FD5")</param>
            /// <returns>
            /// <see cref="FontFamilyMap"/> created from file using <see cref="GetFontFamilyUriPointerPath"/> as <see cref="FontFamilyMap.Target"/>, otherwise target is just <paramref name="FontPathOrName"/> if not a file path
            /// </returns>
            public static FontFamilyMap FontFamilyMapFromFileOrName(string FontPathOrName, string UnicodeRanges)
            {
                if (File.Exists(FontPathOrName))
                {
                    return new FontFamilyMap() { Target = GetFontFamilyUriPointerPath(FontPathOrName), Unicode = UnicodeRanges };
                }
                else
                {
                    return new FontFamilyMap() { Target = FontPathOrName, Unicode = UnicodeRanges };
                }
            }

            public static FontFamily FontFromResource(string FontFamilyPath)
            {
                return new FontFamily(new Uri($"pack://application:,,,/{ThisAssembly};component"), $"./{FontFamilyPath}");
            }
            #endregion

            #region Images

            public static BitmapImage BitmapFromResource(string ResourcePath)
            {
                return new BitmapImage(new Uri($"pack://application:,,,/{ThisAssembly};component/{ResourcePath}"));
            }

            public static BitmapImage BitmapFromFile(string ImagePath)
            {
                if (!string.IsNullOrWhiteSpace(ImagePath) && File.Exists(ImagePath))
                {
                    try
                    {
                        // Uri method instead of MemoryStream locks image file by the program
                        //return new BitmapImage(new Uri(new FileInfo(ImageFilepath).FullName, UriKind.Absolute));

                        using (MemoryStream Stream = new(buffer: File.ReadAllBytes(ImagePath)))
                        {
                            BitmapImage LoadedImage = new();
                            LoadedImage.BeginInit();
                            LoadedImage.StreamSource = Stream;
                            LoadedImage.CacheOption = BitmapCacheOption.OnLoad;
                            LoadedImage.EndInit();
                            LoadedImage.Freeze();

                            return LoadedImage;
                        }
                    }
                    catch (Exception Occurred)
                    {
                        ErrorMessageWindow.ShowException(Occurred);
                        return BitmapFromResource("UI/Empty image.png");
                    }
                }
                else return BitmapFromResource("UI/Empty image.png");
            }
            
            #endregion

            #endregion





            #region Finders

            public static ResourceDictionary? ByUriSource(this Collection<ResourceDictionary> MergedDictionaries, string MergedDictionaryUriSource)
            {
                foreach (ResourceDictionary MergedResourceDictionary in MergedDictionaries)
                {
                    if (MergedResourceDictionary.Source.ToString() == MergedDictionaryUriSource)
                    {
                        return MergedResourceDictionary;
                    }
                }

                return null;
            }

            public static ObjectType? FindTypeName<ObjectType>(this FrameworkElement ParentElement, string xName) where ObjectType : class
                => ParentElement?.FindName(xName) as ObjectType;

            public static ObjectType? FindTypeName<ObjectType>(this FrameworkTemplate TargetTemplate, FrameworkElement TemplateParent, string xName) where ObjectType : class
                => TargetTemplate?.FindName(xName, TemplateParent) as ObjectType;

            public static ObjectType? FindTypeNameFromTemplate<ObjectType>(this Control ParentControl, string xName) where ObjectType : class
                => ParentControl?.Template?.FindName(xName, ParentControl) as ObjectType;


            public static List<ChildrenType> FindVisualChildren<ChildrenType>(this DependencyObject Parent) where ChildrenType : DependencyObject
            {
                List<ChildrenType> Result = [];
                int Count = VisualTreeHelper.GetChildrenCount(Parent);

                for (int ChildIndex = 0; ChildIndex < Count; ChildIndex++)
                {
                    DependencyObject Child = VisualTreeHelper.GetChild(Parent, ChildIndex);

                    if (Child is ChildrenType TargetTypeChild)
                    {
                        Result.Add(TargetTypeChild);
                    }

                    Result.AddRange(Child.FindVisualChildren<ChildrenType>());
                }

                return Result;
            }
            public static T? FindVisualParent<T>(this DependencyObject Child) where T : DependencyObject
            {
                DependencyObject ParentObject = VisualTreeHelper.GetParent(Child);

                while (ParentObject is not null)
                {
                    if (ParentObject is T FoundParent)
                    {
                        return FoundParent;
                    }

                    ParentObject = VisualTreeHelper.GetParent(ParentObject);
                }

                return null;
            }

            #endregion





            #region DependencyProperty
            public static RoutedEvent RegisterEvent<OwnerType, HandlerType>(RoutingStrategy RoutingStrategy = RoutingStrategy.Bubble, [CallerMemberName] string RoutedEventName = "")
            {
                return EventManager.RegisterRoutedEvent(
                    name: RoutedEventName.RegexRemove("Event$"),
                    routingStrategy: RoutingStrategy,
                    handlerType: typeof(HandlerType),
                    ownerType: typeof(OwnerType));
            }

            public static DependencyProperty RegisterProperty<OwnerType, PropertyType>(PropertyType? DefaultValue = default, PropertyChangedCallback? PropertyChangedEvent = null, bool BindsTwoWayByDefault = false, [CallerMemberName] string DependencyPropertyName = "")
            {
                return DependencyProperty.Register(
                   name: DependencyPropertyName.RegexRemove("Property$"), ownerType: typeof(OwnerType), propertyType: typeof(PropertyType),
                   typeMetadata: new FrameworkPropertyMetadata(DefaultValue, PropertyChangedEvent) { BindsTwoWayByDefault = BindsTwoWayByDefault }
                );
            }

            public static DependencyProperty RegisterAttachedProperty(Type PropertyType, Type OwnerType, object? DefaultValue, PropertyChangedCallback? PropertyChangedEvent = null, [CallerMemberName] string DependencyPropertyName = "")
            {
                return DependencyProperty.RegisterAttached(
                   name: DependencyPropertyName.RegexRemove("Property$"), ownerType: OwnerType, propertyType: PropertyType,
                   defaultMetadata: new FrameworkPropertyMetadata(DefaultValue, PropertyChangedEvent)
                );
            }
            #endregion





            #region Quick margin
            public static void SetLeftMargin(this FrameworkElement Target, double LeftMargin)
            {
                Target.Margin = new Thickness(LeftMargin, Target.Margin.Top, Target.Margin.Right, Target.Margin.Bottom);
            }
            public static void SetTopMargin(this FrameworkElement Target, double TopMargin)
            {
                Target.Margin = new Thickness(Target.Margin.Left, TopMargin, Target.Margin.Right, Target.Margin.Bottom);
            }
            public static void SetBottomMargin(this FrameworkElement Target, double BottomMargin)
            {
                Target.Margin = new Thickness(Target.Margin.Left, Target.Margin.Top, Target.Margin.Right, BottomMargin);
            }
            public static void SetRightMargin(this FrameworkElement Target, double RightMargin)
            {
                Target.Margin = new Thickness(Target.Margin.Left, Target.Margin.Top, RightMargin, Target.Margin.Bottom);
            }
            #endregion





            #region Structs value transformers

            public static Thickness ThicknessFrom(double[]? Values)
            {
                if (Values is null)
                {
                    return new Thickness(0);
                }
                else
                {
                    if (Values.Length == 1) return new Thickness(Values[0]);
                    else if (Values.Length == 4) return new Thickness(Values[0], Values[1], Values[2], Values[3]);
                    else return new Thickness(0);
                }
            }

            public static CornerRadius CornerRadiusFrom(double[]? Values)
            {
                if (Values is null)
                {
                    return new CornerRadius(0);
                }
                else
                {
                    if (Values.Length == 1) return new CornerRadius(Values[0]);
                    else if (Values.Length == 4) return new CornerRadius(Values[0], Values[1], Values[2], Values[3]);
                    else return new CornerRadius(0);
                }
            }

            public static FontWeight WeightFrom(string? StringVariant)
            {
                return StringVariant switch
                {
                    "Black"       => FontWeights.Black,
                    "Bold"        => FontWeights.Bold,
                    "Demi Bold"   => FontWeights.DemiBold,
                    "Extra Black" => FontWeights.ExtraBlack,
                    "Extra Bold"  => FontWeights.ExtraBold,
                    "Extra Light" => FontWeights.ExtraLight,
                    "Heavy"       => FontWeights.Heavy,
                    "Light"       => FontWeights.Light,
                    "Medium"      => FontWeights.Medium,
                    "Normal"      => FontWeights.Normal,
                    "Regular"     => FontWeights.Regular,
                    "Semibold"    => FontWeights.SemiBold,
                    "Thin"        => FontWeights.Thin,
                    "Ultra Black" => FontWeights.UltraBlack,
                    "Ultra Bold"  => FontWeights.UltraBold,
                    "Ultra Light" => FontWeights.UltraLight,
                    _             => FontWeights.Normal,
                };
            }
            public static FontStretch FontStretchFrom(string? StringVariant)
            {
                return StringVariant switch
                {
                    "Condensed"       => FontStretches.Condensed,
                    "Expanded"        => FontStretches.Expanded,
                    "Extra Condensed" => FontStretches.ExtraCondensed,
                    "Extra Expanded"  => FontStretches.ExtraExpanded,
                    "Medium"          => FontStretches.Medium,
                    "Normal"          => FontStretches.Normal,
                    "Semi Condensed"  => FontStretches.SemiCondensed,
                    "Semi Expanded"   => FontStretches.SemiExpanded,
                    "Ultra Condensed" => FontStretches.UltraCondensed,
                    "Ultra Expanded"  => FontStretches.UltraExpanded,
                    _                 => FontStretches.Normal
                };
            }

            #endregion





            #region Color from string

            private static byte AsByte(string Hex) => Convert.ToByte(Hex, 16);

            /// <summary>
            /// Accepts RGB or ARGB hex sequence (#rrggbb / #AArrggbb), use <paramref name="AlphaAtTheEnd"/> = <see langword="true"/> for #rrggbbAA hex sequence<br/>
            /// </summary>
            /// <returns><see cref="Colors.White"/> if input is invalid (<see langword="null"/> string, invalid bytes (Not A~F / 0~9), invalid hex length (Not 6 or 8 without '#'))</returns>
            public static Color ToColor(string? HexColor, bool AlphaAtTheEnd = false)
            {
                if (HexColor is null) return Colors.White;

                HexColor = HexColor.Del("#");
                try
                {
                    if (HexColor.Length != 8 & HexColor.Length != 6) return Colors.White;

                    string RGB = HexColor.Length == 8 ? (AlphaAtTheEnd ? HexColor[0..6] : HexColor[2..8]) : HexColor;
                    string Alpha = HexColor.Length == 8 ? (AlphaAtTheEnd ? HexColor[^2..] : HexColor[0..2]) : "FF";

                    return new Color()
                    {
                        A = AsByte(Alpha),
                        R = AsByte(RGB[0..2]),
                        G = AsByte(RGB[2..4]),
                        B = AsByte(RGB[4..6]),
                    };
                }
                catch
                {
                    return Colors.White;
                }
            }

            /// <summary> <inheritdoc cref="ToColor"/> </summary>
            /// <returns> <inheritdoc cref="ToColor"/> </returns>
            public static bool TryParseColor(string? HexColor, out Color OutColor, bool AlphaAtTheEnd = false)
            {
                if (HexColor is null) return false;

                HexColor = HexColor.Del("#");
                try
                {
                    if (HexColor.Length != 8 & HexColor.Length != 6) return false;

                    string RGB = HexColor.Length == 8 ? (AlphaAtTheEnd ? HexColor[0..6] : HexColor[2..8]) : HexColor;
                    string Alpha = HexColor.Length == 8 ? (AlphaAtTheEnd ? HexColor[^2..] : HexColor[0..2]) : "FF";

                    OutColor = new Color()
                    {
                        A = AsByte(Alpha),
                        R = AsByte(RGB[0..2]),
                        G = AsByte(RGB[2..4]),
                        B = AsByte(RGB[4..6]),
                    };

                    return true;
                }
                catch { return false; }
            }

            /// <summary> <inheritdoc cref="ToColor"/> </summary>
            /// <returns><see cref="Brushes.White"/> if input is invalid (<see langword="null"/> string, invalid bytes (Not A~F / 0~9), invalid hex length (Not 6 or 8 without '#'))</returns>
            public static SolidColorBrush ToSolidColorBrush(string? HexColor, bool AlphaAtTheEnd = false) => new(ToColor(HexColor, AlphaAtTheEnd));

            #endregion





            #region Bindings

            public static void BindSameProperties(this FrameworkElement BindingTarget, FrameworkElement BindingSource, params DependencyProperty?[] Properties)
            {
                foreach (DependencyProperty? BindProperty in Properties.Where(x => x is not null))
                {
                    BindingTarget.SetBinding(BindProperty, new Binding(BindProperty!.ToString())
                    {
                        Source = BindingSource
                    });
                }
            }

            public static void BindSame(this FrameworkElement BindingTarget, DependencyProperty BindingSameProperty, FrameworkElement BindingSource)
            {
                BindingTarget.SetBinding(BindingSameProperty, new Binding(BindingSameProperty.ToString())
                {
                    Source = BindingSource
                });
            }

            public static FrameworkElement BindSamePropertiesWithReturn(this FrameworkElement BindingTarget, FrameworkElement BindingSource, params DependencyProperty[] Properties)
            {
                foreach (DependencyProperty BindProperty in Properties)
                {
                    BindingTarget.SetBinding(BindProperty, new Binding(BindProperty.ToString())
                    {
                        Source = BindingSource
                    });
                }

                return BindingTarget;
            }

            #endregion





            #region Specific

            public static void StopAnimation(this UIElement Target, DependencyProperty AnimatedProperty)
            {
                Target.BeginAnimation(AnimatedProperty, null);
            }

            public static void CenterOnScreen(this Window Target)
            {
                Target.Left = (SystemParameters.PrimaryScreenWidth / 2) - Target.ActualWidth / 2;
                Target.Top = (SystemParameters.PrimaryScreenHeight / 2) - Target.ActualHeight / 2;
            }

            public static Task<TWindow> CreateAnotherThreadWindow<TWindow>(Action<TWindow>? SubAction = null) where TWindow : Window, new()
            {
                TaskCompletionSource<TWindow> WindowReturn = new();

                System.Threading.Thread AnotherThread = new(delegate ()
                {
                    TWindow NewWindow = new();
                    NewWindow.Closed += (_, _) => NewWindow.Dispatcher.InvokeShutdown();

                    SubAction?.Invoke(NewWindow);

                    WindowReturn.SetResult(NewWindow);
                    System.Windows.Threading.Dispatcher.Run();
                });
                AnotherThread.SetApartmentState(ApartmentState.STA);
                AnotherThread.Start();

                return WindowReturn.Task;
            }

            public static BitmapSource Tint(this BitmapImage BitmapImage, Color TintColor)
            {
                WriteableBitmap OutputImage = new(BitmapImage);

                int ImageWidth = OutputImage.PixelWidth;
                int ImageHeight = OutputImage.PixelHeight;
                int Stride = ImageWidth * (OutputImage.Format.BitsPerPixel / 8);
                byte[] Pixels = new byte[ImageHeight * Stride];

                OutputImage.CopyPixels(Pixels, Stride, 0);

                for (int PixelIndex = 0; PixelIndex < Pixels.Length; PixelIndex += 4)
                {
                    /* R */ Pixels[PixelIndex + 2] = (byte)(Pixels[PixelIndex + 2] * TintColor.R / 255);
                    /* G */ Pixels[PixelIndex + 1] = (byte)(Pixels[PixelIndex + 1] * TintColor.G / 255);
                    /* B */ Pixels[PixelIndex    ] = (byte)(Pixels[PixelIndex    ] * TintColor.B / 255);
                }

                OutputImage.WritePixels(new Int32Rect(0, 0, ImageWidth, ImageHeight), Pixels, Stride, 0);

                return OutputImage;
            }


            public static double GetInlineTextHeight(this TextBlock Source)
            {
                return new FormattedText(
                    "Text",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(Source.FontFamily, Source.FontStyle, Source.FontWeight, FontStretches.Normal),
                    Source.FontSize,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip
                ).Height;
            }


            public static TextBlock CreateBindedClone(this TextBlock TargetTextBlock, Inline? Content = null)
            {
                TextBlock Copy = new();
                Copy.BindSameProperties(TargetTextBlock,
                [
                    TextBlock.FontSizeProperty, TextBlock.FontFamilyProperty, TextBlock.FontWeightProperty, TextBlock.FontStyleProperty, TextBlock.FontStretchProperty,
                    TextBlock.ForegroundProperty, TextBlock.BackgroundProperty, TextBlock.TextAlignmentProperty, TextBlock.TextWrappingProperty,
                    TextBlock.LineHeightProperty, TextBlock.LineStackingStrategyProperty, TextBlock.TextTrimmingProperty
                ]);
                if (Content is not null)
                {
                    Copy.Inlines.Add(Content);
                }

                return Copy;
            }





            #region Image exports
            public static BitmapImage CaptureElement(FrameworkElement Target, double Upscale = 3.4)
            {
                RenderTargetBitmap CapturedImage = new((int)(Target.ActualWidth * Upscale), (int)(Target.ActualHeight * Upscale), 96d * Upscale, 96d * Upscale, PixelFormats.Default);

                CapturedImage.Render(Target);

                var BitmapEncoder = new PngBitmapEncoder();
                BitmapEncoder.Frames.Add(BitmapFrame.Create(CapturedImage));

                using (MemoryStream ImageSavingStream = new())
                {
                    BitmapEncoder.Save(ImageSavingStream);
                    ImageSavingStream.Seek(0, SeekOrigin.Begin);

                    BitmapImage OutputBitmapImage = new();
                    OutputBitmapImage.BeginInit();
                    OutputBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    OutputBitmapImage.StreamSource = ImageSavingStream;
                    OutputBitmapImage.EndInit();
                    return OutputBitmapImage;
                }
            }



            public static void RenderImage(this FrameworkElement Target, string SavePath, double Upscale = 3.6, bool DoLayoutUpdate = true, bool UseJpegEncoder = false)
            {
                Target.RenderImage(Upscale, DoLayoutUpdate, UseJpegEncoder).SaveToImage(SavePath);
            }


            public static BitmapEncoder RenderImage(this FrameworkElement Target, double Upscale = 3.6, bool DoLayoutUpdate = true, bool UseJpegEncoder = false)
            {
                BitmapEncoder Encoder = UseJpegEncoder ? new JpegBitmapEncoder() : new PngBitmapEncoder(); 

                if (DoLayoutUpdate)
                {
                    Target.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Target.Arrange(new Rect(Target.DesiredSize));
                    Target.UpdateLayout();
                }

                RenderTargetBitmap PreviewLayoutRender = new((int)(Target.ActualWidth * Upscale), (int)(Target.ActualHeight * Upscale), 96d * Upscale, 96d * Upscale, PixelFormats.Default);
                PreviewLayoutRender.Render(Target);

                Encoder.Frames.Add(BitmapFrame.Create(PreviewLayoutRender));

                return Encoder;
            }
            public static void SaveToImage(this BitmapEncoder ImageEncoder, string SavePath)
            {
                using (FileStream ExportStream = new(path: SavePath, mode: FileMode.Create))
                {
                    ImageEncoder.Save(ExportStream);
                    ImageEncoder.Frames.Clear();
                }
            }
            #endregion

            #endregion
        }
    }
}