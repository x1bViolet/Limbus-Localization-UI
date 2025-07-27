using LC_Localization_Task_Absolute;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;

namespace RichText 
{
    internal static class RichTextBoxApplicator
    {
        internal static RichTextBox LastUpdateTarget = new RichTextBox();
        internal static string LastUpdateText = "";

        internal static bool IsProcessingLimbusText = false;

        internal static void UpdateLast()
        {
            if (Upstairs.ActiveProperties.Key == "Skills")
            {
                foreach(var SkillDescItem in Mode_Skills.LastPreviewUpdatesBank)
                {
                    RichTextBoxApplicator.SetLimbusRichText(SkillDescItem.Key, SkillDescItem.Value);
                }
            }
            else
            {
                RichTextBoxApplicator.SetLimbusRichText(RichText.RichTextBoxApplicator.LastUpdateTarget, RichText.RichTextBoxApplicator.LastUpdateText);
            }
        }

        internal static void SetLimbusRichText(this RichTextBox Target, string RichTextString)
        {
            RichTextBoxApplicator.LastUpdateTarget = Target;
            RichTextBoxApplicator.LastUpdateText = RichTextString;
            RichTextString = LimbusPreviewFormatter.Apply(RichTextString);
            IsProcessingLimbusText = true;
            Target.SetRichText(RichTextString, ImagesLineBreak: true);
            IsProcessingLimbusText = false;
        }

        internal static void SetRichText(this RichTextBox Target, string RichTextString, bool ImagesLineBreak = false)
        {
            if (Target.Document.Blocks.Count > 0) Target.Document.Blocks.Clear();
            InternalModel.CreateOn(Target, RichTextString, ImagesLineBreak);
        }

        internal static string R(this string Target)
        {
            return InternalModel.ApplySyntax(Target).Replace("\ufff0", "⟦").Replace("\ufff1", "⟧").Replace("\ufff3", "«").Replace("\ufff4", "»");
        }
    }
    internal abstract class InternalModel
    {
        internal protected static bool InitializingEvent = true;

        internal protected static string PunctuationMarks = "«»\"'—·!?:.–,;¿\\„…›‹“¡”᭟‚᭞᭝‟’;‐‒·․‛‘؟，。⁇⁃،︙۔⁈﹣‽։－।՞՝᥄꘎؛܁⸘፥、࠽𐤟꘏᠃⳾⳹።᙮፡⳺꓿꛳꓾᠈߹܆࠰֊᠆߸፣꛷𒑰፧፤꘍꛶⳻꛴𑅃꛵꫱𐎟𐏐";
        internal protected static List<char> PunctuationMarksLS = PunctuationMarks.ToList<char>();

        internal protected static string ApplySyntax(string s)
        {
            s = s.Replace("\uFFF0", "\x1b[38;5;62m\uFFF0\x1b[0m");
            s = s.Replace("\uFFF1", "\x1b[38;5;62m\uFFF1\x1b[0m");
            s = s.Replace("@", "\x1b[38;5;62m@\x1b[0m");
            s = s.Replace("\uFFF3", "\x1b[38;5;72m\uFFF3");
            s = s.Replace("\uFFF4", "\x1b[38;5;72m\uFFF4\x1b[0m");
            s = s.Replace("InnerTag", "\x1b[38;5;203mInnerTag\x1b[0m");
            s = s.Replace("LevelTag", "\x1b[38;5;204mLevelTag\x1b[0m");
            s = s.Replace("FontFamily", "\x1b[38;5;202mFontFamily\x1b[0m");
            s = s.Replace("TextColor", "\x1b[38;5;202mTextColor\x1b[0m");
            s = s.Replace("TextStyle", "\x1b[38;5;202mTextStyle\x1b[0m");
            s = s.Replace("SpriteLink", "\x1b[38;5;202mSpriteLink\x1b[0m");
            return s;
        }

        //internal protected static Size AbstractedTextSize(string TargetString, RichTextBox FontParametersSource)
        //{
        //    FormattedText AbstractedWidth = new FormattedText(
        //        textToFormat: TargetString,
        //        culture: CultureInfo.CurrentCulture,
        //        flowDirection: FlowDirection.LeftToRight,
        //        typeface: new Typeface(FontParametersSource.FontFamily, FontParametersSource.FontStyle, FontParametersSource.FontWeight, FontParametersSource.FontStretch),
        //        emSize: FontParametersSource.FontSize,
        //        foreground: Brushes.Black,
        //        numberSubstitution: new NumberSubstitution(),
        //        pixelsPerDip: VisualTreeHelper.GetDpi(FontParametersSource).PixelsPerDip);

        //    return new Size(AbstractedWidth.Width, AbstractedWidth.Height);
        //}

        #region Методы добавления текста и спрайтов на предпросмотр
        private protected static void AppendText(InlineTextConstructor TextData, RichTextBox Target)
        {
            try
            {
                FlowDocument TargetedFlowDocument = Target.Document;
                if (TargetedFlowDocument.Blocks.LastBlock is not Paragraph LastFlowDocumentParagraph)
                {
                    LastFlowDocumentParagraph = new Paragraph();
                    TargetedFlowDocument.Blocks.Add(LastFlowDocumentParagraph);
                }

                Run PreviewLayout_AppendRun = new Run(TextData.Text)
                {
                    FontSize = Target.FontSize,
                };

                TagManager.ApplyTags(ref PreviewLayout_AppendRun, TextData.InnerTags);

                #region Sub/Sup
                if (TextData.InnerTags.Contains("TextStyle@Subscript") | TextData.InnerTags.Contains("TextStyle@Superscript"))
                {
                    PreviewLayout_AppendRun.FontSize = Target.FontSize / 1.4;

                    if      (TextData.InnerTags.Contains("TextStyle@Subscript"  )) PreviewLayout_AppendRun.BaselineAlignment = BaselineAlignment.Subscript;
                    else if (TextData.InnerTags.Contains("TextStyle@Superscript")) PreviewLayout_AppendRun.BaselineAlignment = BaselineAlignment.Superscript;

                    LastFlowDocumentParagraph.Inlines.Add(PreviewLayout_AppendRun);
                }
                #endregion

                else
                {
                    LastFlowDocumentParagraph.Inlines.Add(PreviewLayout_AppendRun);
                }
            }
            catch (Exception ex) { rin(ex.ToString()); }
        }

        private protected static void AppendImage(InlineImageConstructor ImageData, RichTextBox Target)
        {
            try
            {
                FlowDocument TargetedFlowDocument = Target.Document;
                if (TargetedFlowDocument.Blocks.LastBlock is not Paragraph LastFlowDocumentParagraph)
                {
                    LastFlowDocumentParagraph = new Paragraph();
                    TargetedFlowDocument.Blocks.Add(LastFlowDocumentParagraph);
                }

                BitmapImage KeywordImageSource = new BitmapImage();
                if (KeywordsInterrogate.KeywordImages.ContainsKey(ImageData.ImageID))
                {
                    KeywordImageSource = KeywordsInterrogate.KeywordImages[ImageData.ImageID];
                }
                else
                {
                    if (KeywordsInterrogate.EGOGiftInlineImages.ContainsKey(ImageData.ImageID) & (UILanguageLoader.UILanguageLoadingEvent || PreviewUpdate_TargetSite.Name.Equals("PreviewLayout_Default")))
                    {
                        KeywordImageSource = KeywordsInterrogate.EGOGiftInlineImages[ImageData.ImageID];
                    }
                    else if (RichTextBoxApplicator.IsProcessingLimbusText)
                    {
                        KeywordImageSource = KeywordsInterrogate.KeywordImages["Unknown"];
                    }
                }

                double OverrideSpriteVerticalOffset = 0;
                double OverrideSpriteHorizontalOffset = 0;
                double OverrideSpriteSize = 0;

                var OverrideSpriteVerticalOffsetValuesFound = ImageData.TextBase.InnerTags.ItemsThatContain("SpritesVerticalOffset@");
                var OverrideSpriteHorizontalOffsetValuesFound = ImageData.TextBase.InnerTags.ItemsThatContain("SpritesHorizontalOffset@");
                var OverrideSpriteSizeValuesFound = ImageData.TextBase.InnerTags.ItemsThatContain("SpritesSize@");

                if (OverrideSpriteVerticalOffsetValuesFound.Count > 0)
                {
                    string target = Regex.Match(OverrideSpriteVerticalOffsetValuesFound[0], @"SpritesVerticalOffset@((\+|\-)\d+)").Groups[1].Value;
                    OverrideSpriteVerticalOffset = double.Parse(target);
                }
                
                if (OverrideSpriteHorizontalOffsetValuesFound.Count > 0)
                {
                    string target = Regex.Match(OverrideSpriteHorizontalOffsetValuesFound[0], @"SpritesHorizontalOffset@((\+|\-)\d+)").Groups[1].Value;
                    OverrideSpriteHorizontalOffset = double.Parse(target);
                }
                
                if (OverrideSpriteSizeValuesFound.Count > 0)
                {
                    string target = Regex.Match(OverrideSpriteSizeValuesFound[0], @"SpritesSize@((\+|\-)\d+)").Groups[1].Value;
                    OverrideSpriteSize = double.Parse(target);
                }

                double DefinedWidth = Target.FontSize;
                double DefinedHeight = Target.FontSize;

                if (OverrideSpriteSize != 0)
                {
                    DefinedWidth = OverrideSpriteSize;
                    DefinedHeight = OverrideSpriteSize;
                }

                Image SpriteImage = new()
                {
                    Source = KeywordImageSource,
                    Width = DefinedWidth,
                    Height = DefinedHeight,
                    Margin = new Thickness(OverrideSpriteHorizontalOffset == 0 ? Configurazione.KeywordSpriteHorizontalOffset : OverrideSpriteHorizontalOffset,
                                           OverrideSpriteVerticalOffset == 0 ? Configurazione.KeywordSpriteVerticalOffset : OverrideSpriteVerticalOffset,
                                           0,
                                           0)
                };

                if (ImageData.TextBase.Text.Equals(""))
                {
                    LastFlowDocumentParagraph.Inlines.Add(new InlineUIContainer(SpriteImage));
                }
                else
                {
                    if (!RichTextBoxApplicator.IsProcessingLimbusText) ImageData.TextBase.Text = "";

                    Run KeywordRun = new Run(ImageData.TextBase.Text);
                    TagManager.ApplyTags(ref KeywordRun, ImageData.TextBase.InnerTags);

                    TextBlock KeywordTextblock = new TextBlock(KeywordRun);

                    StackPanel SpritePlusEffectNameCanvas = new()
                    {
                        Orientation = Orientation.Horizontal,
                        Children = {
                            SpriteImage,
                            KeywordTextblock
                        },
                    };

                    if (!ImageData.PunctuationMarksTextBase.Text.Equals(""))
                    {
                        Run PunctuationMarkRun = new Run(ImageData.PunctuationMarksTextBase.Text);
                        TagManager.ApplyTags(ref PunctuationMarkRun, ImageData.PunctuationMarksTextBase.InnerTags);
                        SpritePlusEffectNameCanvas.Children.Add(new TextBlock(PunctuationMarkRun));
                    }

                    //                                                                                                                              this mf
                    LastFlowDocumentParagraph.Inlines.Add(new InlineUIContainer(SpritePlusEffectNameCanvas) { BaselineAlignment = BaselineAlignment.Top});

                }
            }
            catch (Exception ex) { rin(ex.ToString()); }
        }
        #endregion

        

        

        public static void CreateOn(RichTextBox Target, string RichTextString, bool LinebreakWithSprites = false)
        {
            List<string> TagList = new()
            {
                "/color", 
                "sub",
                "sup",
                "/sub",
                "/sup",
                "i",
                "/i",
                "u",
                "/u",
                "b",
                "/b",
                "style=\"upgradeHighlight\"",
                "/style",
                "strikethrough",
                "/strikethrough",
                "/font",
                "/loadfont",
                "/size",
                "/spritesvoffset",
                "/spriteshoffset",
                "/spritessize",

                "\0",

                "EMPTY¤",
            };
            foreach (string Tag in TagList)
            {
                RichTextString = RichTextString.Replace($">{Tag}<", $">\0{Tag}<");
            }

            bool IsSafeToApplyTags(string Range_TextItem)
            {
                return !TagList.Contains(Range_TextItem) &
                       !Range_TextItem.StartsWith("color=#") &
                       !Range_TextItem.StartsWith("sprite name=\"") &
                       !Range_TextItem.StartsWith("font=\"") &
                       !Range_TextItem.StartsWith("loadfont=`") &
                       !Range_TextItem.StartsWith("size=") &
                       !Range_TextItem.StartsWith("spritesvoffset=") & 
                       !Range_TextItem.StartsWith("spriteshoffset=") & 
                       !Range_TextItem.StartsWith("spritessize=");
            }

            #region Базовое форматирование текста
            RichTextString = RichTextString.Replace("color=#None", "color=#ffffff")
                                           .Replace("<style=\"highlight\">", "<style=\"upgradeHighlight\">"); // Подсветка улучшения (Без разницы как в итоге)

            // Сепарированые обычных '<' '>' от тегов
            RichTextString = Regex.Replace(RichTextString, @"<color=#([0-9a-fA-F]{6})>", "\uFFF5color=#$1\uFFF6");
            RichTextString = Regex.Replace(RichTextString, @"<sprite name=""(\w+)"">", "\uFFF5sprite name=\"$1\"\uFFF6");
            RichTextString = Regex.Replace(RichTextString, @"<style=""(\w+)"">", "\uFFF5style=\"$1\"\uFFF6");
            RichTextString = Regex.Replace(RichTextString, @"<loadfont=`(.*?)`>", "\uFFF5loadfont=`$1`\uFFF6");
            RichTextString = Regex.Replace(RichTextString, @"<size=(\d+)%>", "\uFFF5size=$1%\uFFF6");
            RichTextString = Regex.Replace(RichTextString, @"<spritesvoffset=((\+|\-)\d+)>", "\uFFF5spritesvoffset=$1\uFFF6");
            RichTextString = Regex.Replace(RichTextString, @"<spriteshoffset=((\+|\-)\d+)>", "\uFFF5spriteshoffset=$1\uFFF6");
            RichTextString = Regex.Replace(RichTextString, @"<spritessize=((\+|\-)\d+)>", "\uFFF5spritessize=$1\uFFF6");
            RichTextString = Regex.Replace(RichTextString, @"<font=""(.*?)"">", Match =>
            {
                if ((LimbusPreviewFormatter.LimbusEmbeddedFonts.ContainsKey(Match.Groups[1].Value) & RichTextBoxApplicator.IsProcessingLimbusText) | UILanguageLoader.UILanguageLoadingEvent)
                {
                    return $"\uFFF5font=\"{Match.Groups[1].Value}\"\uFFF6";
                }
                else
                {
                    return Match.Groups[0].Value;
                }
            });
            foreach (string Tag in TagList) RichTextString = RichTextString.Replace($"<{Tag}>", $"\uFFF5{Tag}\uFFF6");
            #endregion


            // Главное разбивание текста на список с обычным текстом и тегами
            string[] TextSegmented = $"\uFFF5EMPTY¤\uFFF6\0\uFFF5EMPTY¤\uFFF6\0\uFFF5EMPTY¤\uFFF6\0\uFFF5EMPTY¤\uFFF6\0{RichTextString}\0\uFFF5EMPTY¤\uFFF6\0\uFFF5EMPTY¤\uFFF6\0\uFFF5EMPTY¤\uFFF6\0\uFFF5EMPTY¤\uFFF6\0".Split(['\uFFF5', '\uFFF6'], StringSplitOptions.RemoveEmptyEntries);

            List<string> __TextSegmented__ = TextSegmented.ToList();
            __TextSegmented__.RemoveAll(TextItem => TextItem.Equals("\0"));

            #region ¤ Форматирование тегов ¤
            int TextSegmented_Count = __TextSegmented__.Count();

            #region Обычный текст
            for (int TextItem_Index = 0; TextItem_Index < __TextSegmented__.Count; TextItem_Index++)
            {
                string TextItem = __TextSegmented__[TextItem_Index];

                #region \uFFF0InnerTag/UptieHighlight\uFFF1
                if (TextItem.Equals("style=\"upgradeHighlight\""))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/style")) break;
                        else if (!TagList.Contains(Range_TextItem))
                        {
                            if (Range_TextItem.Equals("/color"))
                            {
                                __TextSegmented__[RangeIndex] = "";
                            }
                            try
                            {
                                if (!__TextSegmented__[RangeIndex - 1].StartsWith("sprite name=\"") &
                                    !__TextSegmented__[RangeIndex - 2].StartsWith("sprite name=\"") &
                                    !__TextSegmented__[RangeIndex].StartsWith("size=") &
                                    !__TextSegmented__[RangeIndex - 1].StartsWith("size=") &
                                    !__TextSegmented__[RangeIndex].StartsWith("spritesvoffset=") &
                                    !__TextSegmented__[RangeIndex - 1].StartsWith("spritesvoffset=") &
                                    !__TextSegmented__[RangeIndex].StartsWith("spriteshoffset=") &
                                    !__TextSegmented__[RangeIndex - 1].StartsWith("spriteshoffset=") &
                                    !__TextSegmented__[RangeIndex].StartsWith("spritessize=") &
                                    !__TextSegmented__[RangeIndex - 1].StartsWith("spritessize=") &
                                    !__TextSegmented__[RangeIndex].StartsWith("sprite name=\"")) // Сохранять цвета для статусных эффектов

                                {
                                    if (__TextSegmented__[RangeIndex].StartsWith("color=#"))
                                    {
                                        __TextSegmented__[RangeIndex] = "color=#f8c200";
                                    }
                                    else if (!Range_TextItem.Contains("\uFFF0InnerTag/UptieHighlight\uFFF1"))
                                    {
                                        __TextSegmented__[RangeIndex] += "\uFFF0InnerTag/UptieHighlight\uFFF1";
                                    }
                                }
                            }
                            catch (Exception ex) { rin(ex.ToString()); }
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/FontFamily@FontFamilyName\uFFF1
                if (TextItem.StartsWith("font=\""))
                {
                    string FontFamily = Regex.Match(TextItem, @"font=""(.*?)""").Groups[1].ToString();

                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/font") | Range_TextItem.StartsWith("font=\"") | Range_TextItem.StartsWith("loadfont=`")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/FontFamily@") & !Range_TextItem.Contains("\uFFF0InnerTag/LoadedFontFamily@"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/FontFamily@{FontFamily.Trim().Replace(" ", "TEMPLATESPACE")}\uFFF1";
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/LoadedFontFamily@LoadedFontFamilyName\uFFF1
                // Needed for taking fontfamily from application resources
                if (TextItem.StartsWith("loadfont=`"))
                {
                    string FontFamily = Regex.Match(TextItem, @"loadfont=`(.*)`").Groups[1].ToString();
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/font") | Range_TextItem.Equals("/loadfont") | Range_TextItem.StartsWith("font=\"") | Range_TextItem.StartsWith("loadfont=`")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/FontFamily@") & !Range_TextItem.Contains("\uFFF0InnerTag/LoadedFontFamily@"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/LoadedFontFamily@{FontFamily}\uFFF1";
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/FontSize@Percent\uFFF1
                if (TextItem.StartsWith("size=") & TextItem.EndsWith('%'))
                {
                    string FontSizePercentage = Regex.Match(TextItem, @"size=(\d+)%").Groups[1].ToString();
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/size") | Range_TextItem.StartsWith("size=") & Range_TextItem.EndsWith('%')) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/FontSize@"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/FontSize@{FontSizePercentage}%\uFFF1";
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/TextColor@HexRGB\uFFF1
                if (TextItem.StartsWith("color=#") & TextItem.Length == 13)
                {
                    string ColorCode = Regex.Match(TextItem, @"([0-9a-fA-F]{6})").Groups[1].ToString();
                    
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/color") | Range_TextItem.StartsWith("color=#")) break;

                        else
                        {
                            if (IsSafeToApplyTags(Range_TextItem))
                            {
                                __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/TextColor@{ColorCode}\uFFF1";
                            }
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/SpritesVerticalOffset@Number\uFFF1
                if (TextItem.StartsWith("spritesvoffset"))
                {
                    string FontSizePercentage = Regex.Match(TextItem, @"spritesvoffset=((\+|\-)\d+)").Groups[1].ToString();
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/spritesvoffset")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/SpritesVerticalOffset@"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/SpritesVerticalOffset@{FontSizePercentage}\uFFF1";
                            
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/SpritesHorizontalOffset@Number\uFFF1
                if (TextItem.StartsWith("spriteshoffset"))
                {
                    string FontSizePercentage = Regex.Match(TextItem, @"spriteshoffset=((\+|\-)\d+)").Groups[1].ToString();
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/spriteshoffset")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/SpritesHorizontalOffset@"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/SpritesHorizontalOffset@{FontSizePercentage}\uFFF1";

                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/SpritesSize@Number\uFFF1
                if (TextItem.StartsWith("spritessize"))
                {
                    string FontSizePercentage = Regex.Match(TextItem, @"spritessize=((\+|\-)\d+)").Groups[1].ToString();
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/spritessize")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/SpritesSize@"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/SpritesSize@{FontSizePercentage}\uFFF1";
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/TextStyle@Underline\uFFF1
                if (TextItem.Equals("u"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/u")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/TextStyle@Underline\uFFF1") & !Range_TextItem.Contains("\uFFF0InnerTag/TextStyle@Strikethrough\uFFF1"))
                        {
                            __TextSegmented__[RangeIndex] += "\uFFF0InnerTag/TextStyle@Underline\uFFF1";
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/TextStyle@Italic\uFFF1
                if (TextItem.Equals("i"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/i")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/TextStyle@Italic\uFFF1"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/TextStyle@Italic\uFFF1";
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/TextStyle@Bold\uFFF1
                if (TextItem.Equals("b"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/b")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/TextStyle@Bold\uFFF1"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/TextStyle@Bold\uFFF1";
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/TextStyle@Strikethrough\uFFF1
                if (TextItem.Equals("strikethrough"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/strikethrough")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/TextStyle@Strikethrough\uFFF1") & !Range_TextItem.Contains("\uFFF0InnerTag/TextStyle@Underline\uFFF1"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/TextStyle@Strikethrough\uFFF1";
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/TextStyle@Subscript\uFFF1
                if (TextItem.Equals("sub"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/sub") | Range_TextItem.Equals("sup")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/TextStyle@Subscript\uFFF1"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/TextStyle@Subscript\uFFF1";
                        }
                    }
                }
                #endregion

                #region \uFFF0InnerTag/TextStyle@Superscript\uFFF1
                if (TextItem.Equals("sup"))
                {
                    for (int RangeIndex = TextItem_Index + 1; RangeIndex < TextSegmented_Count; RangeIndex++)
                    {
                        string Range_TextItem = __TextSegmented__[RangeIndex];

                        if (Range_TextItem.Equals("/sup") | Range_TextItem.Equals("sub")) break;
                        if (IsSafeToApplyTags(Range_TextItem) & !Range_TextItem.Contains("\uFFF0InnerTag/TextStyle@Superscript\uFFF1"))
                        {
                            __TextSegmented__[RangeIndex] += $"\uFFF0InnerTag/TextStyle@Superscript\uFFF1";
                        }
                    }
                }
                #endregion
            }
            #endregion

            // Очистить сегментированный список текстовых фрагметов от тегов
            __TextSegmented__.RemoveAll(
                TextItem => TagList.Contains(TextItem) |
                TextItem.StartsWith("color=#") |
                TextItem.StartsWith("font=\"") |
                TextItem.StartsWith("loadfont=`") |
                TextItem.StartsWith("size=") |
                TextItem.StartsWith("spritesvoffset=") |
                TextItem.StartsWith("spriteshoffset=") |
                TextItem.StartsWith("spritessize="));

            #region Спрайты
            for (int TextItem_Index = 0; TextItem_Index < __TextSegmented__.Count; TextItem_Index++)
            {
                string TextItem = __TextSegmented__[TextItem_Index];

                #region \uFFF0LevelTag/SpriteLink\uFFF1
                if (TextItem.StartsWith("sprite name=\""))
                {
                    string SpriteLink = TextItem.Split("sprite name=\"")[^1][0..^1];

                    string SpriteKeyword = ":\uFFF3\uFFF4";
                    if (LinebreakWithSprites)
                    {
                        try
                        {
                            if (TextItem_Index + 1 != __TextSegmented__.Count)
                            {
                                string SpriteKeywordAppend = __TextSegmented__[TextItem_Index + 1].Split(' ')[0];
                                if (!__TextSegmented__[TextItem_Index + 1].StartsWith("sprite name=\""))
                                {
                                    string NextTextItem_InnerTags = "";
                                    if (!__TextSegmented__[TextItem_Index + 1][0].Equals(" "))
                                    {
                                        bool SpaceAdd = false;
                                        if (__TextSegmented__[TextItem_Index + 1].Split(' ').Count() > 1) SpaceAdd = true;
                                        __TextSegmented__[TextItem_Index + 1] = (SpaceAdd ? " " : "") + string.Join(' ', __TextSegmented__[TextItem_Index + 1].Split(' ')[1..]);

                                        Regex InnerTags = new Regex(@"\uFFF0InnerTag/(.*?)\uFFF1");

                                        foreach (Match InnerTagMatch in InnerTags.Matches(__TextSegmented__[TextItem_Index + 1]))
                                        {
                                            NextTextItem_InnerTags += InnerTagMatch;
                                        }

                                        SpriteKeyword = $":\uFFF3{(!SpriteKeywordAppend.Contains('\n') ? SpriteKeywordAppend : "") + NextTextItem_InnerTags}\uFFF4";
                                    }
                                    int MaxIndex = __TextSegmented__.Count - 1;
                                    int CheckIndex = TextItem_Index + 2;
                                    if (NextTextItem_InnerTags.Equals("") & MaxIndex >= CheckIndex) // If keyword is only one word length, check for puctuation marks after it to prevent their line break at line end without keyword container
                                    {
                                        foreach (char PunctuationMarker in PunctuationMarksLS)
                                        {
                                            if (__TextSegmented__[TextItem_Index + 2].StartsWith(PunctuationMarker))
                                            {
                                                Regex InnerTags = new Regex(@"\uFFF0InnerTag/(.*?)\uFFF1");
                                                string PunctuationMarkInnerTags = "";
                                                foreach (Match InnerTagMatch in InnerTags.Matches(__TextSegmented__[TextItem_Index + 2]))
                                                {
                                                    PunctuationMarkInnerTags += InnerTagMatch;
                                                }

                                                SpriteKeyword += $"&PunctuationMarker=>\uFFF3{__TextSegmented__[TextItem_Index + 2][0]}{PunctuationMarkInnerTags}\uFFF4";
                                                __TextSegmented__[TextItem_Index + 2] = __TextSegmented__[TextItem_Index + 2][1..];
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex) { rin(ex.ToString()); }
                    }
                    __TextSegmented__[TextItem_Index] = $"\uFFF0LevelTag/SpriteLink@{SpriteLink}{SpriteKeyword}\uFFF1";
                }
                #endregion
            }
            #endregion

            #endregion


            #region Debug Preview
            //if (RichTextBoxApplicator.IsProcessingLimbusText)
            //foreach (var i in __TextSegmented__)
            //{
            //    rin($"  > {i.R()}");
            //}
            #endregion

            try
            {
                Target.Document.Blocks.Clear();
            }
            catch { }

            #region Вывод на предпросмотр
            int Indexer = 0;
            __TextSegmented__ = __TextSegmented__.Where(x => !x.Equals("")).ToList();
            foreach (string TextItem in __TextSegmented__.Where(x => !x.Equals("")))
            {
                #region Спрайты
                if (TextItem.StartsWith("\uFFF0LevelTag/SpriteLink@") & TextItem.EndsWith('\uFFF1'))
                {
                    string[] SpriteSet = TextItem.Split(":\uFFF3");
                    string This_SpriteKeyword = SpriteSet[0].Split("@")[^1].Replace("\uFFF1", "");

                    string ExtraMark = "";
                    List<string> ExtraMarkInnerTags = new();
                    if (SpriteSet[1].Contains("&PunctuationMarker=>\uFFF3"))
                    {
                        string ExtraMarkSector = SpriteSet[1].Split("&PunctuationMarker=>\uFFF3")[1][..^2];
                        ExtraMark = ExtraMarkSector[0].ToString();
                        ExtraMarkInnerTags = TagManager.InnerTags(ExtraMarkSector);
                        //rin($"{ExtraMarkSector} : {ExtraMarkInnerTags.Count}");
                    }
                    SpriteSet[1] = RegexRemove(SpriteSet[1], new Regex(@"&PunctuationMarker=>\uFFF3(.*?)\uFFF4"))[..^2];

                    string This_StickedWord = "";
                    if (SpriteSet.Count() == 2) This_StickedWord = SpriteSet[1].Split("\uFFF4\uFFF1")[0];

                    if (This_StickedWord.Equals("") & Indexer < __TextSegmented__.Count - 1 & !RichTextBoxApplicator.IsProcessingLimbusText)
                    {
                        This_StickedWord = __TextSegmented__[Indexer + 1];
                    }

                    InlineImageConstructor Current_SpriteConstructor = new InlineImageConstructor
                    {
                        ImageID = This_SpriteKeyword,
                        TextBase = new InlineTextConstructor
                        {
                            InnerTags = TagManager.InnerTags(This_StickedWord),
                            Text = TagManager.ClearText(This_StickedWord).Replace("\0", ""),
                        },
                        PunctuationMarksTextBase = new InlineTextConstructor
                        {
                            InnerTags = ExtraMarkInnerTags,
                            Text = ExtraMark,
                        }
                    };
                    AppendImage(Current_SpriteConstructor, Target);
                }
                #endregion
                #region Обычный текст
                else
                {
                    InlineTextConstructor Current_TextConstructor = new InlineTextConstructor
                    {
                        InnerTags = TagManager.InnerTags(TextItem),
                        Text = TagManager.ClearText(TextItem).Replace("\0", "")
                    };

                    AppendText(Current_TextConstructor, Target);
                }
                #endregion

                Indexer++;
            }
            #endregion
        }
    }
}
