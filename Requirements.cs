using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.PreviewCreator;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static LC_Localization_Task_Absolute.Configurazione;

/*/
 * 
 * File with all utility garbage
 * 
/*/

namespace LC_Localization_Task_Absolute
{
    public static class Requirements
    {
        #region WPF
        public static ResourceType Resource<ResourceType>(string xKey) where ResourceType : class => MainWindow.MainControl.FindResource(xKey) as ResourceType;
        public static ObjectType InterfaceObject<ObjectType>(string xName) where ObjectType : class => MainWindow.MainControl.FindName(xName) as ObjectType;
        
        public static ObjectType InterfaceObject<ObjectType>(this FrameworkElement ParentElement, string xName) where ObjectType : class => ParentElement.FindName(xName) as ObjectType;

        public static FontFamily FontFromResource(string FontResourceLocation, string FontFamilyName)
        {
            return new FontFamily(new Uri($"pack://application:,,,/{FontResourceLocation}"), $"./#{FontFamilyName}");
        }
        public static BitmapImage BitmapFromResource(string ResourecImageName)
        {
            try
            {
                return new BitmapImage(new Uri($"pack://application:,,,/{ResourecImageName}"));
            }
            catch
            {
                return new BitmapImage();
            }
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

        public static BitmapSource Tint(BitmapImage BitmapImage, Color ChangeColor)
        {
            WriteableBitmap OutputImage = new WriteableBitmap(BitmapImage);

            int ImageWidth = OutputImage.PixelWidth;
            int ImageHeight = OutputImage.PixelHeight;
            int Stride = ImageWidth * (OutputImage.Format.BitsPerPixel / 8);
            byte[] Pixels = new byte[ImageHeight * Stride];

            OutputImage.CopyPixels(Pixels, Stride, 0);

            Span<byte> PixelSpan = Pixels.AsSpan();
            for (int PixelIndex = 0; PixelIndex < PixelSpan.Length; PixelIndex += 4)
            {
                /* R */ PixelSpan[PixelIndex + 2] = (byte)(PixelSpan[PixelIndex + 2] * ChangeColor.R / 255);
                /* G */ PixelSpan[PixelIndex + 1] = (byte)(PixelSpan[PixelIndex + 1] * ChangeColor.G / 255);
                /* B */ PixelSpan[PixelIndex    ] = (byte)(PixelSpan[PixelIndex    ] * ChangeColor.B / 255);
            }

            OutputImage.WritePixels(new Int32Rect(0, 0, ImageWidth, ImageHeight), Pixels, Stride, 0);

            return OutputImage;
        }

        public static FontWeight WeightFrom(string StringVariant)
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
                _ => FontWeights.Normal,
            };
        }

        public static void ScanScrollviewer(ScrollViewer Target, string NameHint, string ManualPath = "")
        {
            string OutputPath = ManualPath != "" ? ManualPath : @$"[⇲] Assets Directory\Scans\{NameHint} @ {DateTime.Now:HHːmmːss (dd.MM.yyyy)}.jpg";

            double OriginalVerticalScrollOffset = Target.VerticalOffset;
            double OriginalHorizontalScrollOffset = Target.HorizontalOffset;
            Target.ScrollToVerticalOffset(0);
            Target.ScrollToHorizontalOffset(0);
            // Because text on image somehow will slide up if preview was scrolled

            double Upscale = LoadedProgramConfig.ScanParameters.ScaleFactor;

            FrameworkElement PreviewContent = Target.Content as FrameworkElement;

            if (!@CurrentPreviewCreator.IsActive)
            {
                try
                {
                    (PreviewContent as dynamic).Background = ToSolidColorBrush(LoadedProgramConfig.ScanParameters.BackgroundColor);
                }
                catch { }
            }

            BitmapEncoder ExportBitmapEncoder = @CurrentPreviewCreator.IsActive
                ? ScanFrameworkElement<JpegBitmapEncoder>(PreviewContent, Upscale)
                : ScanFrameworkElement<PngBitmapEncoder>(PreviewContent, Upscale);

            using (FileStream ExportStream = new FileStream(path: OutputPath, mode: FileMode.Create))
            {
                ExportBitmapEncoder.Save(ExportStream);
            }

            if (!@CurrentPreviewCreator.IsActive)
            {
                try
                {
                    (PreviewContent as dynamic).Background = Brushes.Transparent;
                }
                catch { }
            }

            Target.ScrollToVerticalOffset(OriginalVerticalScrollOffset);
            Target.ScrollToHorizontalOffset(OriginalHorizontalScrollOffset);
        }

        public static EncoderType ScanFrameworkElement<EncoderType>(FrameworkElement PreviewContent, double Upscale) where EncoderType : BitmapEncoder, new()
        {
            PreviewContent.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            PreviewContent.Arrange(new Rect(PreviewContent.DesiredSize));
            PreviewContent.UpdateLayout();

            int RenderWidth = (int)(PreviewContent.ActualWidth * Upscale);
            int RenderHeight = (int)(PreviewContent.ActualHeight * Upscale);

            RenderTargetBitmap PreviewLayoutRender = new RenderTargetBitmap(RenderWidth, RenderHeight, 96d * Upscale, 96d * Upscale, PixelFormats.Pbgra32);

            PreviewLayoutRender.Render(PreviewContent);

            EncoderType ExportBitmapEncoder = new EncoderType();
            ExportBitmapEncoder.Frames.Add(BitmapFrame.Create(PreviewLayoutRender));

            return ExportBitmapEncoder;
            // // -> Save
            //using (FileStream ExportStream = new FileStream(path: @"C:\Save.png", mode: FileMode.Create))
            //{
            //    ExportBitmapEncoder.Save(ExportStream);
            //}
        }

        public static byte AsByte(string Hex) => Convert.ToByte(Hex, 16);

        public static bool TryParseColor(string HexColor, out Color OutColor)
        {
            if (HexColor == null) return false;

            HexColor = HexColor.Replace("#", "");
            try
            {
                if (HexColor.Length != 8 & HexColor.Length != 6) return false;

                string RGB = HexColor.Length == 8 ? HexColor[2..8] : HexColor;
                string Alpha = HexColor.Length == 8 ? HexColor[0..2] : "FF";

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

        /// <summary>
        /// Accepts RGB or ARGB hex sequence (#rrggbb / #AArrggbb), use <paramref name="AlphaAtTheEnd"/> = <see langword="true"/> for #rrggbbAA hex sequence<br/>
        /// </summary>
        /// <returns><see cref="Colors.White"/> if input is invalid (<see langword="null"/> string, invalid bytes (Not A~F / 0~9), invalid hex length (Not 6 or 8 without '#'))</returns>
        public static Color ToColor(string HexColor, bool AlphaAtTheEnd = false)
        {
            if (HexColor == null) return Colors.White;

            HexColor = HexColor.Replace("#", "");
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

        /// <summary>
        /// Accepts RGB or ARGB hex sequence (#rrggbb / #AArrggbb), use <paramref name="AlphaAtTheEnd"/> = <see langword="true"/> for #rrggbbAA hex sequence<br/>
        /// </summary>
        /// <returns><see cref="Brushes.White"/> if input is invalid (<see langword="null"/> string, invalid bytes (Not A~F / 0~9), invalid hex length (Not 6 or 8 without '#'))</returns>
        public static SolidColorBrush ToSolidColorBrush(string HexColor, bool AlphaAtTheEnd = false) => new SolidColorBrush(ToColor(HexColor, AlphaAtTheEnd));


        public static FontFamily FileToFontFamily(string FontPathOrName, string OverrideFontInternalName = "", bool WriteInfo = false)
        {
            if (File.Exists(FontPathOrName))
            {
                string FontFullPath = new FileInfo(FontPathOrName).FullName;
                Uri FontUri = new Uri(FontFullPath, UriKind.Absolute);
                string FontInternalName = OverrideFontInternalName == "" ? GetFontName(FontFullPath) : OverrideFontInternalName;
                if (WriteInfo) rin($"  Loaded as `{FontInternalName}`");
                return new FontFamily(FontUri, $"./#{FontInternalName}");
            }
            else
            {
                if (WriteInfo) rin($"  Font file \"{FontPathOrName}\" not found, considering given path as font name");
                return new FontFamily(FontPathOrName);
            }
        }

        public static string GetFontName(string FontPath)
        {
            using (System.Drawing.Text.PrivateFontCollection PrivateFonts = new System.Drawing.Text.PrivateFontCollection())
            {
                PrivateFonts.AddFontFile(FontPath);
                string FontName = PrivateFonts.Families[0].Name;

                return FontName;
            }
        }

        public static void BindSame(this FrameworkElement BindingTarget, DependencyProperty BindingSameProperty, FrameworkElement BindingSource)
        {
            BindingTarget.SetBinding(BindingSameProperty, new Binding(BindingSameProperty.ToString())
            {
                Source = BindingSource
            });
        }

        public static void BindSameProperties(this FrameworkElement BindingTarget, FrameworkElement BindingSource, params DependencyProperty[] Properties)
        {
            foreach (DependencyProperty BindProperty in Properties)
            {
                BindingTarget.SetBinding(BindProperty, new Binding(BindProperty.ToString())
                {
                    Source = BindingSource
                });
            }
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

        public static FrameworkElement SetBindingWithReturn(this FrameworkElement Target, DependencyProperty Property, string BindingPropertyName, DependencyObject BindingSource)
        {
            Target.SetBinding(Property, new Binding(BindingPropertyName)
            {
                Source = BindingSource
            });

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
        public static void SetRightMargin(this FrameworkElement Target, double RightMargin)
        {
            Target.Margin = new Thickness(Target.Margin.Left, Target.Margin.Top, RightMargin, Target.Margin.Bottom);
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
        public static void MoveItemUp(this TabControl ParentTabControl, UIElement TargetElement)
        {
            int CurrentIndex = ParentTabControl.Items.IndexOf(TargetElement);
            if (CurrentIndex > 0)
            {
                ParentTabControl.Items.RemoveAt(CurrentIndex);
                ParentTabControl.Items.Insert(CurrentIndex - 1, TargetElement);
            }
        }
        public static void MoveItemDown(this TabControl ParentTabControl, UIElement TargetElement)
        {
            int CurrentIndex = ParentTabControl.Items.IndexOf(TargetElement);
            if (CurrentIndex >= 0 && CurrentIndex < ParentTabControl.Items.Count - 1)
            {
                ParentTabControl.Items.RemoveAt(CurrentIndex);
                ParentTabControl.Items.Insert(CurrentIndex + 1, TargetElement);
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


        public static DependencyProperty Register<OwnerType, PropertyType>(string Name, PropertyType DefaultValue, PropertyChangedCallback ValueSetEvent = null)
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
            Panel Panel = ParentObject as Panel;
            if (Panel != null)
            {
                Panel.Children.Remove(TargetChild);
                return;
            }

            Decorator Decorator = ParentObject as Decorator;
            if (Decorator != null)
            {
                if (Decorator.Child.Equals(TargetChild))
                {
                    Decorator.Child = null;
                }
                return;
            }

            ContentPresenter ContentPresenter = ParentObject as ContentPresenter;
            if (ContentPresenter != null)
            {
                if (ContentPresenter.Content.Equals(TargetChild))
                {
                    ContentPresenter.Content = null;
                }
                return;
            }

            ContentControl ContentControl = ParentObject as ContentControl;
            if (ContentControl != null)
            {
                if (ContentControl.Content.Equals(TargetChild))
                {
                    ContentControl.Content = null;
                }
                return;
            }
        }


        /// <summary>
        /// Set Opacity to 1 and IsHitTestVisible to True
        /// </summary>
        public static void MakeAvailable(params UIElement[] Targets)
        {
            foreach (UIElement Target in Targets)
            {
                Target.Opacity = 1;
                Target.IsHitTestVisible = true;
            }
        }

        /// <summary>
        /// Set Opacity to 0.55 and IsHitTestVisible to False
        /// </summary>
        public static void MakeUnavailable(params UIElement[] Targets)
        {
            foreach (UIElement Target in Targets)
            {
                Target.Opacity = 0.55;
                Target.IsHitTestVisible = false;
            }
        }


        public static void HandleIfNotMatches(this TextCompositionEventArgs PreviewTextInputArgs, Regex Pattern)
        {
            if (!Pattern.Match(PreviewTextInputArgs.Text).Success) PreviewTextInputArgs.Handled = true;
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
        public static bool IsUTF8BOM(this FileInfo TargetFile)
        {
            using (StreamReader Reader = new(TargetFile.FullName, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            {
                Reader.Read(); // Read the first character to trigger encoding detection
                return Reader.CurrentEncoding.GetPreamble().Length != 0;
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


        public static bool ContainsKeyCaseInsensitive<DictionaryValuesType>(this Dictionary<string, DictionaryValuesType> Source, string TargetKey, out string? FoundKey)
        {
            FoundKey = null;

            foreach (string Key in Source.Keys)
            {
                if (Key.Equals(TargetKey, StringComparison.OrdinalIgnoreCase))
                {
                    FoundKey = Key;
                    return true;
                }
            }

            return false;
        }

        public static Dictionary<string, string> RemoveItemWithValue(this Dictionary<string, string> TargetDictionary, string RemoveValue)
        {
            foreach (KeyValuePair<string, string> StringItem in TargetDictionary.Where(KeyValuePair => KeyValuePair.Value.Equals(RemoveValue)).ToList())
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
            Dictionary<string, string> IndexReplacements = [];
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
        #endregion












        public static bool HasAttribute<AttributeType>(this PropertyInfo Property, out AttributeType AcquiredAttribute) where AttributeType : Attribute
        {
            AcquiredAttribute = null;
            AttributeType? GettedAttribute = Property.GetCustomAttribute<AttributeType>();

            if (GettedAttribute != null)
            {
                AcquiredAttribute = GettedAttribute;
                return true;
            }
            else return false;
        }

        public static bool HasAttribute<AttributeType>(this PropertyInfo Property) where AttributeType : Attribute => Property.GetCustomAttribute<AttributeType>() != null;

        public static OpenFileDialog NewOpenFileDialog(string FilesHint, IEnumerable<string> Extensions)
        {
            List<string> FileFilters_DefaultExt = [];
            List<string> FileFilters_Filter = [];

            foreach (string Filter in Extensions)
            {
                FileFilters_DefaultExt.Add($".{Filter}");
                FileFilters_Filter.Add($"*.{Filter}");
            }

            OpenFileDialog FileSelection = new OpenFileDialog();
            FileSelection.DefaultExt = string.Join("|", FileFilters_DefaultExt); // .png|.jpg
            FileSelection.Filter = $"{FilesHint}|{string.Join(";", FileFilters_Filter)}";  // *.png;*.jpg

            return FileSelection;
        }

        public static SaveFileDialog NewSaveFileDialog(string FilesHint, IEnumerable<string> Extensions, string FileDefaultName = "")
        {
            List<string> FileFilters_DefaultExt = [];
            List<string> FileFilters_Filter = [];

            foreach (string Filter in Extensions)
            {
                FileFilters_DefaultExt.Add($".{Filter}");
                FileFilters_Filter.Add($"*.{Filter}");
            }

            SaveFileDialog FileSaving = new SaveFileDialog();
            FileSaving.DefaultExt = string.Join("|", FileFilters_DefaultExt); // .png|.jpg
            FileSaving.Filter = $"{FilesHint}|{string.Join(";", FileFilters_Filter)}";  // *.png;*.jpg
            FileSaving.FileName = FileDefaultName;

            return FileSaving;
        }

        public static string RandomUID(int Length = 5)
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", Length).Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
        public static string FormattedStackTrace(Exception Info, string SetupExceptionHandlingSource = "")
        {
            string AdditionalInfo = "";
            if (Info.Source.Contains("Newtonsoft.Json")) AdditionalInfo = "\n\n\nThis may mean that this json file has an incorrect syntax or a property value type.";

            return $"\n\nThe vile exception Abruptly appears!!!!:\n[{{{SetupExceptionHandlingSource}}} : {Info.Source}] {Info.Message}\nInner: {Info.InnerException}\n\n{Info.StackTrace.FormatStackTraceByNamespace("LC_Localization_Task_Absolute", @"C:\Users\javas\OneDrive\Документы\LC Localization Interface (Code)\")}{AdditionalInfo}\n\n";
        }

        public static string FormatStackTraceByNamespace(this string StackTrace, string Namepsace, string DeletePart = "")
        {
            return string.Join('\n', StackTrace.Split('\n').Where(x => x.Contains(Namepsace))).Del(DeletePart);
        }


        /// <summary>
        /// where <paramref name="IEnumerableTargets"/> : <see cref="IEnumerable{T}"/> with .Clear() method
        /// </summary>
        public static void ClearMany(params dynamic[] IEnumerableTargets)
        {
            foreach (dynamic IIEnumerable in IEnumerableTargets)
            {
                IIEnumerable.Clear();
            }
        }

        public static void Await(double Seconds, Action CompleteAction)
        {
            DispatcherTimer Timer = new() { Interval = TimeSpan.FromSeconds(Seconds) };
            Timer.Tick += (Sender, Args) =>
            {
                Timer.Stop();
                CompleteAction();
            };
            Timer.Start();
        }

        public static List<FileInfo> ToFileInfos(this IEnumerable<string> FilePaths)
        {
            List<FileInfo> Output = [];
            foreach (string TargetFile in FilePaths)
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
            foreach (string Check in CheckSource)
            {
                if (CheckString.Equals(Check)) return true;
            }

            return false;
        }
        public static bool EqualsOneOf(this Enum CheckEnum, params Enum[] CheckSource)
        {
            foreach (Enum Check in CheckSource)
            {
                if (CheckEnum.Equals(Check)) return true;
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

        public static bool IsNullOrEmpty(this string? CheckString)
        {
            if (CheckString == null)
            {
                return true;
            }
            else if (CheckString.Equals("") | CheckString.Equals(string.Empty))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool StartsWithOneOf(this string CheckString, params string[] CheckSource)
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
            List<string> Export = [];
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
            List<string> Export = [""];
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
            List<string> Export = [""];
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
            if (!string.IsNullOrEmpty(Target)) foreach (string? Fragment in FragmentsToRemove)
            {
                if (!string.IsNullOrEmpty(Fragment)) Target = Target.Replace(Fragment, "");
            }

            return Target;
        }

        /// <summary>
        /// Return "null" string if Source is null, else Source
        /// </summary>
        public static string nullHandle(this object? Source, string NullText = "null")
        {
            return $"{Source ?? NullText}";
        }

        public static bool HasProperty(this Type Target, string PropertyName)
        {
            return Target.GetProperties().Any(property => property.Name.Equals(PropertyName));
        }
    }
}
