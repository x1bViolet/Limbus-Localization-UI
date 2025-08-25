using LC_Localization_Task_Absolute;
using LC_Localization_Task_Absolute.Limbus_Integration;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using static LC_Localization_Task_Absolute.Requirements;

namespace RichText
{
    public class InlineTextConstructor
    {
        public string Text;
        public List<string> InnerTags;
    }

    public class InlineImageConstructor
    {
        public string ImageID;
        public InlineTextConstructor TextBase;
        public InlineTextConstructor PunctuationMarksTextBase;
    };

    public static class TagManager
    {
        public static List<string> InnerTags(string Source)
        {
            List<string> OutputTags = new();
            try
            {
                Regex InnerTags = new Regex(@"\uFFF0InnerTag/(.*?)\uFFF1");
                foreach (Match InnerTagMatch in InnerTags.Matches(Source))
                {
                    OutputTags.Add(InnerTagMatch.Groups[1].Value.Replace("InnerTag/", ""));
                }
            }
            catch { }
            return OutputTags;
        }

        public static string ClearText(string Source)
        {
            try
            {
                if (!Source.StartsWith("\uFFF0LevelTag/"))
                {
                    Source = Regex.Replace(Source, @"\uFFF0InnerTag/(.*?)\uFFF1", Match => { return ""; });
                }
                else
                {
                    Source = Regex.Replace(Source, @"\uFFF0LevelTag/SpriteLink@(\w+):\uFFF3(.*?)\uFFF4\uFFF1", Match => { return ClearText(Match.Groups[2].Value); });
                }
            }
            catch { }

            return Source;
        }



        public static void ApplyTags(ref Run TargetRun, List<string> Tags)
        {
            try
            {
                foreach (var Tag in Tags)
                {
                    string[] TagBody = Tag.Split('@');
                    switch (TagBody[0])
                    {
                        case "TextColor":
                            TargetRun.Foreground = ToSolidColorBrush($"#{TagBody[1]}");
                            break;

                        case "FontFamily":
                            try
                            {
                                if (LimbusPreviewFormatter.LimbusEmbeddedFonts.ContainsKey(TagBody[1].Replace("TEMPLATESPACE", " ")))
                                {
                                    TargetRun.FontFamily = LimbusPreviewFormatter.LimbusEmbeddedFonts[TagBody[1].Replace("TEMPLATESPACE", " ")];
                                }
                                else if (UILanguageLoader.UILanguageLoadingEvent)
                                {
                                    TargetRun.FontFamily = new System.Windows.Media.FontFamily(TagBody[1].Replace("TEMPLATESPACE", " "));
                                }
                            }
                            catch { }

                            break;

                        case "LoadedFontFamily":
                            try
                            {
                                if (UILanguageLoader.LoadedFontFamilies.ContainsKey(TagBody[1]))
                                {
                                    TargetRun.FontFamily = UILanguageLoader.LoadedFontFamilies[TagBody[1]];
                                }
                            }
                            catch { }
                            break;

                        case "FontSize":
                            try
                            {
                                int TargetFontSize = int.Parse(TagBody[1][..^1]);
                                
                                if (TargetFontSize == 0) TargetFontSize = 1;

                                TargetRun.FontSize *= 0.01 * TargetFontSize;
                            }
                            catch { }
                            break;

                        case "UptieHighlight":
                            TargetRun.Foreground = ToSolidColorBrush($"#fff8c200");
                            break;

                        case "TextStyle":
                            switch (TagBody[1])
                            {
                                case "Underline":
                                    TargetRun.TextDecorations = TextDecorations.Underline;
                                    break;

                                case "Strikethrough":
                                    TargetRun.TextDecorations = TextDecorations.Strikethrough;
                                    break;

                                case "Italic":
                                    TargetRun.FontStyle = FontStyles.Italic;
                                    break;

                                case "Bold":
                                    TargetRun.FontWeight = FontWeights.SemiBold;
                                    break;

                            }

                            break;

                        default: break;
                    }
                }
            }
            catch { }
        }
    }
}
