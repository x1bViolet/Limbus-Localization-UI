using LC_Localization_Task_Absolute.Limbus_Integration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Size = System.Windows.Size;

/*/
 * 
 * File with all utility garbage
 * 
/*/

namespace LC_Localization_Task_Absolute
{
    public static class SecondaryUtilities
    {
        // To apply custom color to white images (Tint), used for creating custom color for sinner name in custom identity preview
        // Sixlabors because default options of 'foreach(pixel in somewpfbitmapimage)' slow asf

        // ^ BitmapImage --ToSixlaborsImage()-> Image<Rgba32> -> TintWhiteMaskImage(Image<Rgba32>) -> colored Image<Rgba32> --ToBitmapImage()-> colored BitmapImage

        // Some speed shenanigans from SixLabors docs https://docs.sixlabors.com/articles/imagesharp/pixelbuffers.html
        public static Image<Rgba32> TintWhiteMaskImage(Image<Rgba32> LoadedImage, Rgba32 TintColor)
        {
            // Or 'LoadedImage = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(@"C:\,some,where,\Import.png");'

            Image<Rgba32> ResultImage = LoadedImage.Clone();
            ResultImage.ProcessPixelRows(Accessor =>
            {
                for (int Y_HeightPixel = 0; Y_HeightPixel < Accessor.Height; Y_HeightPixel++)
                {
                    Span<Rgba32> PixelSpan = Accessor.GetRowSpan(Y_HeightPixel);

                    for (int X_WidthPixel = 0; X_WidthPixel < PixelSpan.Length; X_WidthPixel++)
                    {
                        ref Rgba32 CurrentPixel = ref PixelSpan[X_WidthPixel];

                        if (CurrentPixel.A > 0)
                        {
                            CurrentPixel = new Rgba32
                            (
                                (byte)(CurrentPixel.R * TintColor.R / 255),
                                (byte)(CurrentPixel.G * TintColor.G / 255),
                                (byte)(CurrentPixel.B * TintColor.B / 255),
                                CurrentPixel.A
                            );
                        }
                    }
                }
            });
            
            return ResultImage; // Or 'ResultImage.SaveAsPng(@"C:\,some,where,\Export.png");'
        }

        public static BitmapImage TintWhiteMaskBitmap(BitmapImage Selected, string HexColor)
        {
            SixLabors.ImageSharp.Image<Rgba32> Colored = SecondaryUtilities.TintWhiteMaskImage(Selected.ToSixlaborsImage(), Rgba32.ParseHex(HexColor.Replace("#", "")));

            return Colored.ToBitmapImage();
        }

        public static Image<Rgba32> ToSixlaborsImage(this BitmapImage WPFImageSource)
        {
            int Width = WPFImageSource.PixelWidth;
            int Height = WPFImageSource.PixelHeight;
            int ArrayStride = Width * 4; // Four bytes per pixel in BGRA32
            byte[] PixelData = new byte[Height * ArrayStride];

            WPFImageSource.CopyPixels(PixelData, ArrayStride, 0);

            return SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(PixelData, Width, Height);
        }

        public static BitmapImage ToBitmapImage(this Image<Rgba32> SixlaborsImageSource)
        {
            using (MemoryStream ImageConvertStream = new MemoryStream())
            {
                SixlaborsImageSource.SaveAsPng(ImageConvertStream);
                ImageConvertStream.Position = 0;

                BitmapImage ResultBitmapImage = new BitmapImage();
                ResultBitmapImage.BeginInit();
                ResultBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                ResultBitmapImage.StreamSource = ImageConvertStream;
                ResultBitmapImage.EndInit();
                ResultBitmapImage.Freeze();

                return ResultBitmapImage;
            }
        }
    }

    public static class Requirements
    {
        #region WPF
        public static ResourceType Resource<ResourceType>(string Name) where ResourceType : class => MainWindow.MainControl.FindResource(Name) as ResourceType;
        public static ObjectType InterfaceObject<ObjectType>(string xName) where ObjectType : class => MainWindow.MainControl.FindName(xName) as ObjectType;

        public static FontFamily FontFromResource(string FontResourceLocation, string FontFamilyName)
        {
            return new FontFamily(new Uri($"pack://application:,,,/{FontResourceLocation}"), $"./#{FontFamilyName}");
        }
        public static BitmapImage BitmapFromResource(string ResourecImageName)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/{ResourecImageName}"));
        }
        public static BitmapImage BitmapFromFile(string ImageFilepath)
        {
            if (File.Exists(ImageFilepath))
            {
                // Uri method instead of MemoryStream locks image file by the program
                //return new BitmapImage(new Uri(new FileInfo(ImageFilepath).FullName, UriKind.Absolute));

                using (MemoryStream Stream = new MemoryStream(File.ReadAllBytes(ImageFilepath)))
                {
                    BitmapImage LoadImage = new BitmapImage();
                    LoadImage.BeginInit();
                    LoadImage.StreamSource = Stream;
                    LoadImage.CacheOption = BitmapCacheOption.OnLoad;
                    LoadImage.EndInit();
                    LoadImage.Freeze();

                    return LoadImage;
                }
            }
            else return new BitmapImage();
        }

        public static byte[] ConvertWebpToPng(byte[] WebpImageData)
        {
            using (MemoryStream InputStream = new MemoryStream(WebpImageData))
            using (SixLabors.ImageSharp.Image OutputImage = SixLabors.ImageSharp.Image.Load(InputStream))
            {
                using (MemoryStream OutputStream = new MemoryStream())
                {
                    OutputImage.SaveAsPng(OutputStream);
                    return OutputStream.ToArray();
                }
            }
        }

        public static FontWeight WeightFrom(string StringVariant)
        {
            return StringVariant switch
            {
                "Black" => FontWeights.Black,
                "Bold" => FontWeights.Bold,
                "Demi Bold" => FontWeights.DemiBold,
                "Extra Black" => FontWeights.ExtraBlack,
                "Extra Bold" => FontWeights.ExtraBold,
                "Extra Light" => FontWeights.ExtraLight,
                "Heavy" => FontWeights.Heavy,
                "Light" => FontWeights.Light,
                "Medium" => FontWeights.Medium,
                "Normal" => FontWeights.Normal,
                "Regular" => FontWeights.Regular,
                "Semibold" => FontWeights.SemiBold,
                "Thin" => FontWeights.Thin,
                "Ultra Black" => FontWeights.UltraBlack,
                "Ultra Bold" => FontWeights.UltraBold,
                "Ultra Light" => FontWeights.UltraLight,
                _ => FontWeights.Normal,
            };
        }

        public static void ScanScrollviewer(ScrollViewer Target, string NameHint, string ManualPath = "", double DpiX = 96d, double DpiY = 96d)
        {
            string OutputPath = !ManualPath.Equals("") ? ManualPath : @$"[⇲] Assets Directory\[⇲] Scans\{NameHint} @ {DateTime.Now.ToString("HHːmmːss (dd.MM.yyyy)")}.png";

            ////////////////////////////////////////////////////
            double OriginalVerticalScrollOffset = Target.VerticalOffset;
            double OriginalHorizontalScrollOffset = Target.HorizontalOffset;
            Target.ScrollToVerticalOffset(0);
            Target.ScrollToHorizontalOffset(0);
            // Because text on image somehow will slide up if preview was scrolled

            double Upscale = Configurazione.DeltaConfig.ScanParameters.ScaleFactor;

            FrameworkElement PreviewContent = Target.Content as FrameworkElement;

            try
            {
                (PreviewContent as dynamic).Background = ToSolidColorBrush(Configurazione.DeltaConfig.ScanParameters.BackgroundColor);
            }
            catch { }

            PreviewContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            PreviewContent.Arrange(new Rect(PreviewContent.DesiredSize));
            PreviewContent.UpdateLayout();

            int RenderWidth = (int)(PreviewContent.ActualWidth * Upscale);
            int RenderHeight = (int)(PreviewContent.ActualHeight * Upscale);

            RenderTargetBitmap PreviewLayoutRender = new RenderTargetBitmap(RenderWidth, RenderHeight, DpiX * Upscale, DpiY * Upscale, PixelFormats.Pbgra32);

            PreviewLayoutRender.Render(PreviewContent);

            PngBitmapEncoder ExportBitmapEncoder = new PngBitmapEncoder();
            ExportBitmapEncoder.Frames.Add(BitmapFrame.Create(PreviewLayoutRender));

            using (FileStream ExportStream = new FileStream(path: OutputPath, mode: FileMode.Create))
            {
                ExportBitmapEncoder.Save(ExportStream);
            }

            try
            {
                (PreviewContent as dynamic).Background = Brushes.Transparent;
            }
            catch { }

            ////////////////////////////////////////////////////
            Target.ScrollToVerticalOffset(OriginalVerticalScrollOffset);
            Target.ScrollToHorizontalOffset(OriginalHorizontalScrollOffset);
        }

        public static Color ToColorBrush(string HexColor)
        {
            if (HexColor == null) return Colors.White;

            HexColor = HexColor.Replace("#", "");

            try
            {
                if (HexColor.Length == 6)
                {
                    return new Color()
                    {
                        A = 255,
                        R = Convert.ToByte(HexColor.Substring(0, 2), 16),
                        G = Convert.ToByte(HexColor.Substring(2, 2), 16),
                        B = Convert.ToByte(HexColor.Substring(4, 2), 16)
                    };
                }
                else if (HexColor.Length == 8)
                {
                    return new Color()
                    {
                        A = Convert.ToByte(HexColor.Substring(0, 2), 16),
                        R = Convert.ToByte(HexColor.Substring(2, 2), 16),
                        G = Convert.ToByte(HexColor.Substring(4, 2), 16),
                        B = Convert.ToByte(HexColor.Substring(6, 2), 16)
                    };
                }
                else return Colors.White;
            }
            catch
            {
                return Colors.White;
            }
        }

        /// <summary>
        /// Accepts RGB or ARGB hex sequence (#rrggbb / #AArrggbb)<br/>
        /// Returns transperent if HexString is null/"" or if HexString is not color
        /// </summary>
        public static SolidColorBrush ToSolidColorBrush(string HexColor)
        {
            if (HexColor == null) return Brushes.White;

            HexColor = HexColor.Replace("#", "");

            try
            {
                if (HexColor.Length == 6)
                {
                    return new SolidColorBrush(new Color()
                    {
                        A = 255,
                        R = Convert.ToByte(HexColor.Substring(0, 2), 16),
                        G = Convert.ToByte(HexColor.Substring(2, 2), 16),
                        B = Convert.ToByte(HexColor.Substring(4, 2), 16)
                    });
                }
                else if (HexColor.Length == 8)
                {
                    return new SolidColorBrush(new Color()
                    {
                        A = Convert.ToByte(HexColor.Substring(0, 2), 16),
                        R = Convert.ToByte(HexColor.Substring(2, 2), 16),
                        G = Convert.ToByte(HexColor.Substring(4, 2), 16),
                        B = Convert.ToByte(HexColor.Substring(6, 2), 16)
                    });
                }
                else return Brushes.White;
            }
            catch
            {
                return Brushes.White;
            }
        }


        public static FontFamily FileToFontFamily(string FontPath, string OverrideFontInternalName = "", bool WriteInfo = false)
        {
            if (File.Exists(FontPath))
            {
                string FontFullPath = new FileInfo(FontPath).FullName;
                Uri FontUri = new Uri(FontFullPath, UriKind.Absolute);
                string FontInternalName = OverrideFontInternalName.Equals("") ? GetFontName(FontFullPath) : OverrideFontInternalName;
                if (WriteInfo) rin($"      Successful font loading from file as `{FontInternalName}`");
                return new FontFamily(FontUri, $"./#{FontInternalName}");
            }
            else
            {
                if (WriteInfo) rin($"      Font file \"{FontPath}\" not found, returning \"Arial\"");
                return new FontFamily("Arial");
            }
        }

        public static FontFamily FileToFontFamily_WithNameReturn(string FontPath, out string AcquiredFontName)
        {
            if (File.Exists(FontPath))
            {
                string FontFullPath = new FileInfo(FontPath).FullName;
                Uri FontUri = new Uri(FontFullPath, UriKind.Absolute);

                AcquiredFontName = GetFontName(FontFullPath);

                return new FontFamily(FontUri, $"./#{AcquiredFontName}");
            }
            else
            {
                AcquiredFontName = "?";

                return new FontFamily("Arial");
            }
        }

        public static string GetFontName(string FontPath)
        {
            using (System.Drawing.Text.PrivateFontCollection PrivateFonts = new())
            {
                PrivateFonts.AddFontFile(FontPath);
                string FontName = PrivateFonts.Families[0].Name;

                return FontName;
            }
        }

        public static FrameworkElement SetBindingWithReturn(this FrameworkElement Target, DependencyProperty Property, string BindingPropertyName, DependencyObject BindingSource)
        {
            Target.SetBinding(Property, new Binding(BindingPropertyName)
            {
                Source = BindingSource
            });

            return Target;
        }

        public static UIElement SetColumn(this UIElement Target, int ColumnNumber)
        {
            Grid.SetColumn(Target, ColumnNumber);
            return Target;
        }

        public static TextBlock SetLineHeight(this TextBlock Target, double LineHeight)
        {
            Target.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            Target.LineHeight = LineHeight;
            return Target;
        }

        public static TextBlock ImposedClone(this TextBlock TargetTextBlock, Inline Content = null)
        {
            TextBlock Output = new TextBlock()
            {
                FontSize = TargetTextBlock.FontSize,
                FontFamily = TargetTextBlock.FontFamily,
                FontWeight = TargetTextBlock.FontWeight,
                FontStyle = TargetTextBlock.FontStyle,
                Foreground = TargetTextBlock.Foreground,
                Background = TargetTextBlock.Background,
                TextAlignment = TargetTextBlock.TextAlignment,
                TextWrapping = TargetTextBlock.TextWrapping,
                LineHeight = TargetTextBlock.LineHeight,
                LineStackingStrategy = TargetTextBlock.LineStackingStrategy,
                TextTrimming = TargetTextBlock.TextTrimming,
            };
            if (Content != null)
            {
                Output.Inlines.Add(Content);
            }

            return Output;
        }
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

        public static void MoveItemUp(this StackPanel ParentStackPanel, UIElement TargetElement)
        {
            int CurrentIndex = ParentStackPanel.Children.IndexOf(TargetElement);
            if (CurrentIndex > 0)
            {
                ParentStackPanel.Children.RemoveAt(CurrentIndex);
                ParentStackPanel.Children.Insert(CurrentIndex - 1, TargetElement);
            }
        }
        public static void MoveItemDown(this StackPanel ParentStackPanel, UIElement TargetElement)
        {
            int CurrentIndex = ParentStackPanel.Children.IndexOf(TargetElement);
            if (CurrentIndex >= 0 && CurrentIndex < ParentStackPanel.Children.Count - 1)
            {
                ParentStackPanel.Children.RemoveAt(CurrentIndex);
                ParentStackPanel.Children.Insert(CurrentIndex + 1, TargetElement);
            }
        }

        public static Thickness ThicknessFrom(double[] Values)
        {
            if (Values.Length == 1) return new Thickness(Values[0]);
            else if (Values.Length == 4) return new Thickness(Values[0], Values[1], Values[2], Values[3]);
            else return new Thickness(0);
        }
        public static CornerRadius CornerRadiusFrom(double[] Values)
        {
            if (Values.Length == 1) return new CornerRadius(Values[0]);
            else if (Values.Length == 4) return new CornerRadius(Values[0], Values[1], Values[2], Values[3]);
            else return new CornerRadius(0);
        }

        public static FontFamily FontFamilyFrom(string FilePathOrFamilyName)
        {
            if (File.Exists(FilePathOrFamilyName)) return FileToFontFamily(FilePathOrFamilyName);
            else return new FontFamily(FilePathOrFamilyName);
        }

        public static DependencyProperty Register<OwnerType, PropertyType>(string Name, object DefaultValue, PropertyChangedCallback ValueSetEvent)
        {
            return DependencyProperty.Register(
               name: Name, ownerType: typeof(OwnerType), propertyType: typeof(PropertyType),
               typeMetadata: new PropertyMetadata(DefaultValue, ValueSetEvent)
            );
        }

        public static TMProEmitter SetRichTextWithReturn(this TMProEmitter Target, string RichText)
        {
            Target.RichText = RichText;
            return Target;
        }

        // Used in preview exports to png as reconnection items to Canvas for displaying them over other elements
        public static void ReconnectAsChildTo(this UIElement TargetElement, dynamic NewParent)
        {
            DependencyObject TargetElement_Parent = (TargetElement as FrameworkElement).Parent;
            TargetElement_Parent.RemoveChild(TargetElement);

            NewParent.Children.Add(TargetElement);
        }
        public static void RemoveChild(this DependencyObject ParentObject, UIElement TargetChild)
        {
            var Panel = ParentObject as Panel;
            if (Panel != null)
            {
                Panel.Children.Remove(TargetChild);
                return;
            }

            var Decorator = ParentObject as Decorator;
            if (Decorator != null)
            {
                if (Decorator.Child == TargetChild)
                {
                    Decorator.Child = null;
                }
                return;
            }

            var ContentPresenter = ParentObject as ContentPresenter;
            if (ContentPresenter != null)
            {
                if (ContentPresenter.Content == TargetChild)
                {
                    ContentPresenter.Content = null;
                }
                return;
            }

            var ContentControl = ParentObject as ContentControl;
            if (ContentControl != null)
            {
                if (ContentControl.Content == TargetChild)
                {
                    ContentControl.Content = null;
                }
                return;
            }
        }
        #endregion



        #region General
        /// <summary>
        /// Console.WriteLine()
        /// </summary>
        public static void rin(params object[] s)
        {
            File.AppendAllText(@"[⇲] Assets Directory\Latest loading.txt", String.Join(' ', s) + "\n");
            Console.WriteLine(String.Join(' ', s));
        }
        public static void rinx(params object[] s) { Console.WriteLine(String.Join(' ', s)); rinx(); }
        public static void rinx() => Console.ReadKey();


        // Readed file encoding
        public static Encoding GetFileEncoding(this FileInfo TargetFile)
        {
            using (StreamReader Reader = new StreamReader(TargetFile.FullName, Encoding.Default, true))
            {
                Reader.Peek();
                return Reader.CurrentEncoding;
            }
        }


        // Check filename without language prefix
        public static string RemovePrefix(this string Target, params string[] Prefixes)
        {
            if (Target.StartsWithOneOf(Prefixes))
            {
                foreach (string SinglePrefix in Prefixes)
                {
                    if (Target.StartsWith(SinglePrefix))
                    {
                        return Target[SinglePrefix.Length..];
                    }
                }
            }

            return Target;
        }


        // KeywordsInterrogate.Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter
        public static Dictionary<string, string> RemoveItemWithValue(this Dictionary<string, string> TargetDictionary, string RemoveValue)
        {
            foreach (KeyValuePair<string, string> StringItem in TargetDictionary.Where(KeyValuePair => KeyValuePair.Value == RemoveValue).ToList())
            {
                TargetDictionary.Remove(StringItem.Key);
            }

            return TargetDictionary;
        }


        /// <summary>
        /// Formatter for [$] insertions
        /// </summary>
        public static string Extern(this string TargetString, object Replacement)
        {
            return TargetString.Replace("[$]", $"{Replacement}");
        }
        /// <summary>
        /// Formatter for enumerated [$n] insertions
        /// </summary>
        public static string Exform(this string TargetString, params object[] Replacements)
        {
            Dictionary<string, string> IndexReplacements = new();
            int ReplacementsIndexer = 1;
            foreach (object Replacement in Replacements)
            {
                IndexReplacements[$"[${ReplacementsIndexer}]"] = $"{Replacement}";
                ReplacementsIndexer++;
            }

            foreach (KeyValuePair<string, string> Replacement in IndexReplacements)
            {
                TargetString = TargetString.Replace(Replacement.Key, Replacement.Value);
            }

            return TargetString;
        }


        public static void LogCustomSkillsConstructor(params object[] s)
        {
            string LogFile = @"[⇲] Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Constructor\Recognizing Log.txt";
            if (!File.Exists(LogFile))
            {
                File.WriteAllText(LogFile, "");
                File.SetAttributes(LogFile, File.GetAttributes(LogFile) | FileAttributes.Hidden);
            }

            File.AppendAllText(LogFile, String.Join(' ', s) + "\n");
        }
        #endregion






















        public static string FormatStackTraceByNamespace(this string StackTrace, string Namepsace, string DeletePart = "")
        {
            return string.Join('\n', StackTrace.Split('\n').Where(x => x.Contains(Namepsace))).Del(DeletePart);
        }

        public static void Await(double Seconds, Action CompleteAction)
        {
            Grid placeholder = new Grid() { Background = ToSolidColorBrush("#00000000") };
            DoubleAnimation doubleAnimation = new DoubleAnimation() { Duration = new Duration(TimeSpan.FromSeconds(Seconds)) };
            doubleAnimation.Completed += (s, e) => { CompleteAction(); };
            placeholder.Background.BeginAnimation(SolidColorBrush.OpacityProperty, doubleAnimation);
        }

        public class HighPrecisionTimer
        {
            public static void DoTimerAction(double DurationSeconds, double ActionFrequencySeconds, Action FrequencyAction, Action FinishAction = null)
            {
                var timer = new HighPrecisionTimer(DurationSeconds, ActionFrequencySeconds * (double)1000);

                if (FrequencyAction != null)
                {
                    timer.OnTick += (elapsed) =>
                    {
                        FrequencyAction();
                    };
                }
                if (FinishAction != null)
                {
                    timer.OnFinished += FinishAction;
                }

                timer.Start();
            }

            private System.Timers.Timer _timer;
            private double _durationSeconds;
            private DateTime _startTime;

            public event Action<double> OnTick; // Передает прошедшее время
            public event Action OnFinished;

            public HighPrecisionTimer(double DurationSeconds, double ActionFrequencyMilliseconds)
            {
                _durationSeconds = DurationSeconds;
                _timer = new System.Timers.Timer(ActionFrequencyMilliseconds); // интервал 10 мс
                _timer.Elapsed += Timer_Elapsed;
                _timer.AutoReset = true;
            }

            public void Start()
            {
                _startTime = DateTime.Now;
                _timer.Start();
            }

            private void Timer_Elapsed(object? RequestSender, ElapsedEventArgs EventArgs)
            {
                var elapsed = (DateTime.Now - _startTime).TotalSeconds;
                if (elapsed >= _durationSeconds)
                {
                    _timer.Stop();
                    OnFinished?.Invoke();
                }
                else
                {
                    OnTick?.Invoke(elapsed);
                }
            }
        }
        
        public static int LinesCount(this string check) => check.Count(f => f == '\n');

        public static byte ToByte(this string HexDigit) => byte.Parse(HexDigit, System.Globalization.NumberStyles.HexNumber);

        public static List<FileInfo> ToFileInfos(this IEnumerable<string> FilePaths)
        {
            List<FileInfo> Output = new List<FileInfo>();
            foreach(string TargetFile in FilePaths)
            {
                Output.Add(new FileInfo(TargetFile));
            }
            return Output;
        }

        public static string ToEscapeRegexString(this string TargetString)
        {
            return TargetString.Replace("(", @"\(").Replace(")", @"\)")
                               .Replace("[", @"\[").Replace("]", @"\]")
                               .Replace(".", @"\.")
                               .Replace("?", @"\?")
                               .Replace("$", @"\$")
                               .Replace("^", @"\^")
                               .Replace("+", @"\+");
        }

        public static string RegexRemove(this string TargetString, Regex PartPattern)
        {
            TargetString = PartPattern.Replace(TargetString, Match =>
            {
                return "";
            });
            return TargetString;
        }

        public static bool EqualsOneOf(this string CheckString, params string[] CheckSource)
        {
            foreach (var Check in CheckSource)
            {
                if (CheckString.Equals(Check)) return true;
            }

            return false;
        }
        public static bool MatchesWithOneOf(this string CheckString, params string[] Patterns)
        {
            foreach (string Checkpattern in Patterns)
            {
                if (Regex.Match(CheckString, Checkpattern).Success) return true;
            }

            return false;
        }

        public static bool IsNullOrEmpty(this string CheckString)
        {
            if (CheckString == null)
            {
                return true;
            }
            else if (CheckString.Equals("") | CheckString == string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool StartsWithOneOf(this string CheckString, IEnumerable<string> CheckSource)
        {
            foreach (string Check in CheckSource)
            {
                if (CheckString.StartsWith(Check)) return true;
            }

            return false;
        }

        public static bool EndsWithOneOf(this string CheckString, IEnumerable<string> CheckSource)
        {
            foreach (string Check in CheckSource)
            {
                if (CheckString.EndsWith(Check)) return true;
            }

            return false;
        }

        public static bool ContainsOneOf(this string CheckString, params string[] CheckSource)
        {
            foreach (string Check in CheckSource)
            {
                if (CheckString.Contains(Check)) return true;
            }

            return false;
        }

        public static List<string> ItemsThatContain(this IEnumerable<string> CheckSource, string CheckString)
        {
            List<string> Export = new() { };
            foreach (string Check in CheckSource)
            {
                if (Check.Contains(CheckString))
                {
                    Export.Add(Check);
                }
            }

            return Export;
        }

        public static List<string> ItemsThatStartsWith(this IEnumerable<string> CheckSource, string CheckString)
        {
            List<string> Export = new() { "" };
            foreach (string Check in CheckSource)
            {
                if (Check.StartsWith(CheckString))
                {
                    if (Export[0].Equals("")) Export.RemoveAt(0);
                    Export.Add(Check);
                }
            }

            return Export;
        }

        public static List<string> ItemsThatEndsWith(this IEnumerable<string> CheckSource, string CheckString)
        {
            List<string> Export = new() { "" };
            foreach (string Check in CheckSource)
            {
                if (Check.EndsWith(CheckString))
                {
                    if (Export[0].Equals("")) Export.RemoveAt(0);
                    Export.Add(Check);
                }
            }

            return Export;
        }

        public static FileInfo? GetFileWithName(this string SearchDirectory, string SearchName)
        {
            FileInfo[] Files = new DirectoryInfo(SearchDirectory).GetFiles("*.*", SearchOption.AllDirectories);

            FileInfo[] Found = Files.Where(SearchFile => SearchFile.Name.Equals(SearchName)).ToArray();
            if (Found.Length > 0)
            {
                return Found[0];
            }
            else
            {
                return null;
            }
        }

        public static string GetText(this FileInfo file)
        {
            return File.ReadAllText(file.FullName);
        }

        public static string GetName(this string Filepath)
        {
            return Filepath.Split("\\")[^1].Split("/")[^1];
        }

        public static string GetEscapeSequence(char c)
        {
            return ((int)c).ToString("X4");
        }
        /// <summary>
        /// Remove string parts
        /// </summary>
        public static string Del(this string Target, params string[] FragmentsToRemove)
        {
            foreach (string Fragment in FragmentsToRemove) Target = Target.Replace(Fragment, "");
            return Target;
        }

        /// <summary>
        /// Return "null" string if Source is null, else Source
        /// </summary>
        public static string nullHandle(this object? Source, string NullText = "null")
        {
            return $"{(Source == null ? NullText : Source)}";
        }

        public static bool HasProperty(this Type Target, string PropertyName)
        {
            return Target.GetProperties().Where(property => property.Name.Equals(PropertyName)).Any();
        }
    }
}
