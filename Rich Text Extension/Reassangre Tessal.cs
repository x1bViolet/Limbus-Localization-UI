//#define DebugPrintInfo

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.Reassangre_Tessal.Actions;

namespace LC_Localization_Task_Absolute
{
    internal interface Reassangre_Tessal
    {
        internal abstract class @PostInfo
        {
            public static Dictionary<string, BitmapImage> InlineImages = new Dictionary<string, BitmapImage>()
            {
                ["Unknown"] = ImageFromResource(@"Default/Images/Unknown.png")
            };
            public static Dictionary<string, FontFamily> KnownFonts = new Dictionary<string, FontFamily>()
            {
                //["Pretendard"] = FontFromResource(@"Reassange Tessal/Fonts/", "Pretendard"),
                //["SCDream5"] = FontFromResource(@"Reassange Tessal/Fonts/", "S-Core Dream 5 Medium")
            };

            public static double SpritesVerticalOffset = 0;
            public static double SpritesHorizontalOffset = 0;

            public static TagType[] IgnoreTags_Default = [];
            public static TagType[] IgnoreTags_UnityTMProExclude =
            [
                TagType.Background,
                TagType.FontStretch,
                TagType.InlineImage,
                TagType.InlineImages_Size,
                TagType.InlineImages_XOffset,
                TagType.InlineImages_YOffset,
            ];

            public static readonly Dictionary<string, FontWeight> UnityTMProFontWeightValues = new Dictionary<string, FontWeight>()
            {
                ["100"] = FontWeights.Thin,
                ["200"] = FontWeights.ExtraLight,
                ["300"] = FontWeights.Light,
                ["400"] = FontWeights.Regular,
                ["500"] = FontWeights.Medium,
                ["600"] = FontWeights.SemiBold,
                ["700"] = FontWeights.Bold,
                ["800"] = FontWeights.Heavy,
                ["900"] = FontWeights.Black,
            };

            public static readonly Dictionary<string, FontStretch> FontStretchValues = new Dictionary<string, FontStretch>()
            {
                ["Condensed"     ] = FontStretches.Condensed,
                ["Expanded"      ] = FontStretches.Expanded,
                ["ExtraCondensed"] = FontStretches.ExtraCondensed,
                ["ExtraExpanded" ] = FontStretches.ExtraExpanded,
                ["Medium"        ] = FontStretches.Medium,
                ["Normal"        ] = FontStretches.Normal,
                ["SemiCondensed" ] = FontStretches.SemiCondensed,
                ["SemiExpanded"  ] = FontStretches.SemiExpanded,
                ["UltraCondensed"] = FontStretches.UltraCondensed,
                ["UltraExpanded" ] = FontStretches.UltraExpanded,
            };

            /* lang=regex */
            public static readonly string DefaultString1 =
@"Наносит на <style=""highlight"">15%</style> больше урона за каждый <sprite name=""DianxueDonQuixote""><color=#e30000><u><link=""DianxueDonQuixote"">Удар по меридианам - Дон Кихот</link></u></color> на цели (До <style=""highlight"">45%</style>)

<style=""highlight"">Если быстрее цели</style>, +1 к Силе монет <style=""highlight"">за каждые 2 единицы разницы в скорости (До 2)</style>

<color=#f95e00>[Победа в столкновении]</color> Получает 5 <sprite name=""Breath""><color=#f8c200><u><link=""Breath"">Дыхания</link></u></color>

<color=#f95e00>[Победа в столкновении]</color> Если цели нанесён <sprite name=""DianxueDonQuixote""><color=#e30000><u><link=""DianxueDonQuixote"">Удар по меридианам - Дон Кихот</link></u></color>, получает ещё 3 <sprite name=""Breath""><color=#f8c200><u><link=""Breath"">Дыхания</link></u></color>,";

            /* lang=regex */
            public static readonly string DefaultString2 =
@"Наносит на <style=""highlight"">15%</style> больше урона за каждый <nobr><sprite name=""DianxueDonQuixote""><color=#e30000><u><link=""DianxueDonQuixote"">Удар по меридианам - Дон Кихот</link></u></color></nobr> на цели

<sprite name=""Breath""><color=#f8c200><u><link=""Breath"">Дыхание</link></u></color>, было";
        }


        internal abstract class Actions
        {
            private abstract class TagProcessors
            {
                public static void OnFormatting(InlineTagData Tag, TagType TranzitType)
                {
                    switch (TranzitType) // Simplify tag info based on type
                    {
                        case TagType.Link:
                            Tag.Info = Tag.Info.Del("link=\"")[0..^1];
                            break;

                        case TagType.Font:
                            Tag.Info = Tag.Info.Del("font=\"")[0..^1];
                            break;

                        case TagType.FontWeight:
                            Tag.Info = Tag.Info.Del("font-weight=\"")[0..^1];
                            break;

                        case TagType.FontStretch:
                            Tag.Info = Tag.Info.Del("font-stretch=\"")[0..^1];
                            break;

                        case TagType.SizeMultiplier:
                            Tag.Info = Tag.Info.Del("size=")[0..^1].Replace('.', ',');
                            break;

                        case TagType.Color:
                            Tag.Info = Tag.Info.Del("color=");
                            break;

                        case TagType.Background:
                            Tag.Info = Tag.Info.Del("background=");
                            break;

                        case TagType.Sprite:
                            Tag.SpriteOrNoBreakData_SubItems = new List<StableTextConstruction>();
                            Tag.Info = Tag.Info.Del("sprite name=\"")[0..^1]; // Sprite id
                            break;

                        case TagType.InlineImage:
                            Tag.Info = Tag.Info.Del("image id=\"")[0..^1]; // Image id
                            break;

                        case TagType.InlineImages_Size:
                            Tag.Info = Tag.Info.Del("images-size=").Replace('.', ',');
                            break;

                        case TagType.InlineImages_XOffset:
                            Tag.Info = Tag.Info.Del("images-xoffset=").Replace('.', ',');
                            break;

                        case TagType.InlineImages_YOffset:
                            Tag.Info = Tag.Info.Del("images-yoffset=").Replace('.', ',');
                            break;

                        case TagType.NoBreak:
                            Tag.SpriteOrNoBreakData_SubItems = new List<StableTextConstruction>();
                            break;

                        default: break;
                    }
                }

                public static void OnApplying(Run Target, KeyValuePair<TagType, InlineTagData> Tag)
                {
                    string TagInfo = Tag.Value.Info;
                    switch (Tag.Key) // Tag actions
                    {
                        case TagType.Color:
                            Target.Foreground = ToSolidColorBrush(TagInfo);
                            break;

                        case TagType.Background:
                            Target.Background = ToSolidColorBrush(TagInfo);
                            break;

                        case TagType.Link:
                            Target.Name = TagInfo; // Run.Name = id to text
                            //Target.MouseLeftButtonDown += Target_MouseLeftButtonDown; // Add trigger to display keyword info window
                            break;

                        case TagType.Font:
                            if (@PostInfo.KnownFonts.ContainsKey(TagInfo))
                            {
                                Target.FontFamily = @PostInfo.KnownFonts[TagInfo];
                            }
                            else
                            {
                                Target.FontFamily = new FontFamily(TagInfo);
                            }
                            break;

                        case TagType.FontWeight:
                            if (@PostInfo.UnityTMProFontWeightValues.ContainsKey(TagInfo))
                            {
                                Target.FontWeight = @PostInfo.UnityTMProFontWeightValues[TagInfo];
                            }
                            break;

                        case TagType.FontStretch:
                            if (@PostInfo.UnityTMProFontWeightValues.ContainsKey(TagInfo))
                            {
                                Target.FontStretch = @PostInfo.FontStretchValues[TagInfo];
                            }
                            break;

                        case TagType.SizeMultiplier:
                            if (double.TryParse(TagInfo, out double FontSizeMultiplyValue))
                            {
                                double ApplyValue = FontSizeMultiplyValue / (double)100;
                                if (ApplyValue == 0) ApplyValue = 0.01;
                                Target.FontSize = RecenetTextBlockTarget.FontSize * ApplyValue;
                            }
                            break;

                        case TagType.Subscript:
                            Target.FontSize = RecenetTextBlockTarget.FontSize * 0.7;
                            Target.BaselineAlignment = BaselineAlignment.Subscript;
                            break;

                        case TagType.Superscript:
                            Target.FontSize = RecenetTextBlockTarget.FontSize * 0.7;
                            Target.BaselineAlignment = BaselineAlignment.Superscript;
                            break;
                    }
                }
            }

            private static void SetRunFormatting(ref Run Target, Dictionary<TagType, InlineTagData> AssignedTags, params TagType[] IgnoreTags)
            {
                foreach (KeyValuePair<TagType, InlineTagData> Tag in AssignedTags)
                {
                    if (!IgnoreTags.Contains(Tag.Key))
                    {
                        if (RegisteredTags[Tag.Key].UnivocalPropertyKey != null)
                        {
                            Target.SetValue(RegisteredTags[Tag.Key].UnivocalPropertyKey, RegisteredTags[Tag.Key].UnivocalPropertyValue);
                        }
                        else
                        {
                            TagProcessors.OnApplying(Target, Tag);
                        }
                    }
                }
            }

            private static TagType EnumTransform(string From)
            {
                if (Enum.TryParse<TagType>(From, out TagType result))
                {
                    return result;
                }
                else
                {
                    throw new Exception($"Unknown tag type '{From}' not found in TagType enum params");
                }
            }

            /* lang=regex */                                        // Alternate divider options (Keep Regex syntax)
            public static List<Tuple<string, string>> FullStopDividers = new List<Tuple<string, string>>()
            {
              //new Tuple<string, string>(@"\[", @"\]"),
                new Tuple<string, string>(@"<", @">"),
            };

            public enum TagType
            {
                CloseSequence, // BreakPoints automatically applied type
                TechnicalNull, // Don't touch

                NoBreak,
                Link,
                Bold,
                Undeline,
                Italic,
                Strikethrough,
                Subscript,
                Superscript,
                StyleHighlighter,
                Color,
                Background,
                Font,
                FontWeight,
                FontStretch,
                SizeMultiplier,
                Sprite,
                InlineImage,
                InlineImages_Size,
                InlineImages_XOffset,
                InlineImages_YOffset,
            };

            private record PatternAndTypeMatch
            {
                public TagType? MainTag { get; set; }

                public TagType? CloseSequenceParentTag { get; set; }

                public PatternAndTypeMatch(TagType Main) { MainTag = Main; }
            }
            private record FullStopTag
            {
                // Regex patterns for tag endings (Modified to @"^String$")
                public string[] BreakPoints { get; set; }

                public DependencyProperty UnivocalPropertyKey { get; set; }

                public dynamic UnivocalPropertyValue { get; set; }

                public FullStopTag(string PatternToMatch, string[] BreakPoints, TagType Type)
                {
                    if (BreakPoints != null)
                    {
                        this.BreakPoints = BreakPoints.Select(x => $"^{x}$").ToArray();
                        foreach (string BreakPoint in BreakPoints) RegisteredTags_PatternAndTypeMatch[BreakPoint] = new PatternAndTypeMatch(TagType.CloseSequence)
                        {
                            CloseSequenceParentTag = Type
                        };
                    }

                    RegisteredTags_PatternAndTypeMatch[PatternToMatch] = new PatternAndTypeMatch(Type);

                    if (!RegisteredTags.ContainsKey(Type)) RegisteredTags[Type] = this;
                    else throw new Exception($"Tag definition with same type already defined ({Type})");
                }
            }

            private static Dictionary<TagType, FullStopTag> RegisteredTags = new Dictionary<TagType, FullStopTag>();

            /// <summary>
            /// To replace true tags in text with unique text construction 
            /// </summary>
            private static Dictionary<string, PatternAndTypeMatch> RegisteredTags_PatternAndTypeMatch = new Dictionary<string, PatternAndTypeMatch>();

            /* lang=regex  defines new instances of FullStopTag records that executes all further sequence */
            private static List<FullStopTag> TagDefinitions = new List<FullStopTag>()
            {
                                                        // Evident tag types, regardless of values like in <font> or <color>
                new("b", ["/b"], TagType.Bold)          { UnivocalPropertyKey = Run.FontWeightProperty, UnivocalPropertyValue = FontWeights.Bold  },
                new("i", ["/i"], TagType.Italic)        { UnivocalPropertyKey = Run.FontStyleProperty , UnivocalPropertyValue = FontStyles.Italic },
                new("u", ["/u"], TagType.Undeline)      { UnivocalPropertyKey = Run.TextDecorationsProperty, UnivocalPropertyValue = TextDecorations.Underline     },
                new("s", ["/s"], TagType.Strikethrough) { UnivocalPropertyKey = Run.TextDecorationsProperty, UnivocalPropertyValue = TextDecorations.Strikethrough },

                new("nobr", ["/nobr"],  TagType.NoBreak),

                new("sub",   ["/sub"],   TagType.Subscript),
                new("sup",   ["/sup"],   TagType.Superscript),
                new("style", ["/style"], TagType.StyleHighlighter) { UnivocalPropertyKey = Run.ForegroundProperty, UnivocalPropertyValue = ToSolidColorBrush("#f8c200") }, // Highlight color
                new(@"link=""\w+""", ["/link"], TagType.Link),
                new(@"font="".*?""", ["/font"], TagType.Font),
                new(@"font-weight=""(100|200|300|400|500|600|700|800|900)""", ["/font-weight"], TagType.FontWeight), // https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/RichTextFontWeight.html
                new(@"font-stretch=""(Condensed|Expanded|ExtraCondensed|ExtraExpanded|Medium|Normal|SemiCondensed|SemiExpanded|UltraCondensed|UltraExpanded)""", ["/font-stretch"], TagType.FontStretch),
                new(@"size=(\d+)(\.\d+)?%", ["/size"], TagType.SizeMultiplier),
                new(@"color=#([a-fA-F0-9]{8}|[a-fA-F0-9]{6})", ["/color"], TagType.Color),
                new(@"background=#([a-fA-F0-9]{8}|[a-fA-F0-9]{6})", ["/background"], TagType.Background),
                new(@"sprite name=""\w+""", null, TagType.Sprite),

                new(@"image id=""\w+""", null, TagType.InlineImage),
                new(@"images-size=(\d+)(\.\d+)?",    ["/images-size"],    TagType.InlineImages_Size),
                new(@"images-xoffset=(\-|\+)?(\d+)(\.\d+)?", ["/images-xoffset"], TagType.InlineImages_XOffset),
                new(@"images-yoffset=(\-|\+)?(\d+)(\.\d+)?", ["/images-yoffset"], TagType.InlineImages_YOffset),
            };

            /// <summary>
            /// Contains text segment and tags to apply
            /// </summary>
            public record StableTextConstruction
            {
                public string TextSentence { get; set; }
                public Dictionary<TagType, InlineTagData> AssignedTags { get; set; } = new Dictionary<TagType, InlineTagData>();

                public bool IsTagItself { get; set; } = false;
                public InlineTagData InnerTagData { get; set; }

                public StableTextConstruction(string BaseText)
                {
                    TextSentence = BaseText;
                    if (BaseText.Contains('\xF8FE'))
                    {
                        IsTagItself = true;
                        InnerTagData = new InlineTagData(TextSentence, out string PlainTagReturn);
                        TextSentence = PlainTagReturn;
                    }
                }
            }

            /// <summary>
            /// Tag data, set when creating EstablishedSequence and TextSentence contains \xF8FE (10 lines upper)
            /// </summary>
            public record InlineTagData
            {
                public TagType Type { get; set; }

                public string Info { get; set; }

                public string[] BreakPoints { get; set; }

                public List<StableTextConstruction> SpriteOrNoBreakData_SubItems { get; set; } // (Sprite: first word from next segment + quote mark after) (NoBreak: all segments inside)

                public InlineTagData(string BaseTextString, out string PlainTagReturn)
                {
                    string[] TagDefinition = BaseTextString.Split('\xF8FE');

                    TagType TranzitType = this.Type = EnumTransform(TagDefinition[0]);
                    Info = PlainTagReturn = TagDefinition[^1];

                    if (TranzitType != TagType.CloseSequence) BreakPoints = RegisteredTags[TranzitType].BreakPoints;

                    TagProcessors.OnFormatting(this, TranzitType);
                }
            }

            private static void ApplyCascadeSequence(List<StableTextConstruction> @Sequence, int StartIndex, InlineTagData TagDataToAdd, TagType ButRemoveThisTag = TagType.TechnicalNull, TagType ButIgnoreIfThisTagOccurs = TagType.TechnicalNull)
            {
                bool SkipWhileThereAreSameTagOpened = false;
                foreach (StableTextConstruction MidTextSegment in @Sequence[(StartIndex + 1)..])
                {
                    if (MidTextSegment.IsTagItself && MidTextSegment.InnerTagData.Type == TagDataToAdd.Type)
                    {
                        // Just as in Unity TextMeshPro, at text fragment "<color=#e30000><color=#f8c200>Yellow</color> and there are red</color>" 'and there are red' part still must me colored red
                        SkipWhileThereAreSameTagOpened = true;
                    }

                    if (!SkipWhileThereAreSameTagOpened)
                    {
                        if (MidTextSegment.TextSentence.MatchesWidthOneOf(TagDataToAdd.BreakPoints)) break;

                        if (!MidTextSegment.AssignedTags.ContainsKey(ButIgnoreIfThisTagOccurs))
                        {
                            if (!MidTextSegment.IsTagItself)
                            {
                                MidTextSegment.AssignedTags[TagDataToAdd.Type] = TagDataToAdd;

                                if (ButRemoveThisTag != TagType.TechnicalNull)
                                {
                                    MidTextSegment.AssignedTags.Remove(ButRemoveThisTag);
                                }
                            }
                            else if (MidTextSegment.IsTagItself && (MidTextSegment.InnerTagData.Type == TagType.InlineImage || MidTextSegment.InnerTagData.Type == TagType.Sprite))
                            {
                                MidTextSegment.AssignedTags[TagDataToAdd.Type] = TagDataToAdd;
                            }
                        }
                    }
                    else if (MidTextSegment.TextSentence.MatchesWidthOneOf(TagDataToAdd.BreakPoints))
                    {
                        SkipWhileThereAreSameTagOpened = false;
                    }
                }
            }

            private static TextBlock RecenetTextBlockTarget = null;
            public static void Apply(TextBlock Target, string RichText, params TagType[] IgnoreTags)
            {
                IgnoreTags = IgnoreTags.Union(@PostInfo.IgnoreTags_Default).ToArray();

                RecenetTextBlockTarget = Target;
                RichText = RichText.RegexRemove(@"=(""highlight""|""upgradeHighlight"")"); // Unify to just <style></style>, doesn't matter -> yellow color

                foreach (Tuple<string, string> FullStopTagDivider in FullStopDividers)
                {
                    string TagKey_Open = FullStopTagDivider.Item1;
                    string TagKey_Close = FullStopTagDivider.Item2;

                    foreach (KeyValuePair<string, PatternAndTypeMatch> TagInfo in RegisteredTags_PatternAndTypeMatch)
                    {
                        string TagBodyPattern = TagInfo.Key;
                        TagType TagEnumSign = (TagType)TagInfo.Value.MainTag;

                        TagType TagEnumSignToCheckIgnore = (TagType)(TagInfo.Value.CloseSequenceParentTag != null ? TagInfo.Value.CloseSequenceParentTag : TagInfo.Value.MainTag);

                        // <color=#f8c200> -> '{\xFFFE}TagType.Color{\xF8FE}color=#f8c200{\xFFFF}' where {\x} is special unicode character
                        if (!IgnoreTags.Contains(TagEnumSignToCheckIgnore))
                        {
                            RichText = Regex.Replace(RichText, TagKey_Open + $"({TagBodyPattern})" + TagKey_Close, Match =>
                            {
                                return $"\xFFFE{(TagEnumSign)}\xF8FE{Match.Groups[1].Value}\xFFFF";
                            });
                        }
                    }
                }

                List<StableTextConstruction> @Segments = new List<StableTextConstruction>(RichText.Replace("\r", "").Split(['\xFFFE', '\xFFFF'], StringSplitOptions.RemoveEmptyEntries).Select(RegularText => new StableTextConstruction(RegularText)));


                int CurrentIndex = 0;
                int MaxTotalIndex = @Segments.Count - 1;

                /* FIRST ORDER TAGS [Without <style> being applied, because it checks for <link> to decide 'ignore keyword color or not' (And also must remove <color>), but there are no <link> in further segments because the foreach loop hasn't reached its opening tag yet and hasn't applied it to the text segments] */
                foreach (StableTextConstruction TextSegment in @Segments)
                {
                    // Need to process actual tags only
                    if (TextSegment.IsTagItself &&
                        TextSegment.InnerTagData.Type != TagType.StyleHighlighter && // IN THE SECOND ORDER
                        TextSegment.InnerTagData.Type != TagType.Subscript &&        //
                        TextSegment.InnerTagData.Type != TagType.Superscript         //
                    ) {
                        // Regular simple tags
                        if (TextSegment.InnerTagData.Type != TagType.NoBreak &
                            TextSegment.InnerTagData.Type != TagType.CloseSequence &
                            TextSegment.InnerTagData.BreakPoints != null
                        ) {
                            ApplyCascadeSequence(@Segments, CurrentIndex, TextSegment.InnerTagData); // REGULAR TAGS
                        }
                        else if (TextSegment.InnerTagData.Type == TagType.Sprite | TextSegment.InnerTagData.Type == TagType.InlineImage)
                        {
                            TextSegment.IsTagItself = false; // Do not remove sprite/image tags at List<StableTextConstruction> EstablishedSequence by this condition
                            if (CurrentIndex != MaxTotalIndex & TextSegment.InnerTagData.Type == TagType.Sprite)
                            {
                                // And also connect image with next word in text and maybe punctuation mark after after to prevent keyword from wrapping to the next line without a sprite
                                int SubSpriteSegmentIndex = CurrentIndex + 1;
                                foreach (StableTextConstruction SubSpriteSegment in Segments[SubSpriteSegmentIndex..])
                                {
                                    if (!SubSpriteSegment.IsTagItself)
                                    {
                                        Match FirstWordFound = Regex.Match(SubSpriteSegment.TextSentence, @"^\w+");
                                        if (FirstWordFound.Success) // If next text segment is starts with word
                                        {
                                            string GrabbedWord = FirstWordFound.Value;

                                            SubSpriteSegment.TextSentence = SubSpriteSegment.TextSentence[GrabbedWord.Length..]; // Cut grabbed word from original
                                            TextSegment.InnerTagData.SpriteOrNoBreakData_SubItems.Add(SubSpriteSegment with { TextSentence = GrabbedWord }); // Copy next text segment with grabbed word only

                                            // Check if next text sentence is starts with punctuation marks and grab it too
                                            foreach (StableTextConstruction SubSpriteSegment_PunctuationSearch in Segments[SubSpriteSegmentIndex..])
                                            {
                                                if (!SubSpriteSegment_PunctuationSearch.IsTagItself && SubSpriteSegment_PunctuationSearch.TextSentence != "")
                                                {
                                                    Match FirstPunctuationFound = Regex.Match(SubSpriteSegment_PunctuationSearch.TextSentence, @"^\p{P}"); // ^\p{P} = any punctuation mark at start of text
                                                    if (FirstPunctuationFound.Success)
                                                    {
                                                        string GrabbedPunctuation = FirstPunctuationFound.Value;

                                                        SubSpriteSegment_PunctuationSearch.TextSentence = SubSpriteSegment_PunctuationSearch.TextSentence[GrabbedPunctuation.Length..];
                                                        TextSegment.InnerTagData.SpriteOrNoBreakData_SubItems.Add(SubSpriteSegment_PunctuationSearch with { TextSentence = GrabbedPunctuation });
                                                    }

                                                    break;
                                                }
                                            }
                                        }

                                        break; // Check only next text segment
                                    }
                                    // else encountered with tag

                                    SubSpriteSegmentIndex++;
                                }
                            }
                        }
                    }

                    CurrentIndex++;
                }
                CurrentIndex = 0;

                /* SECOND ORDER TAGS [Apply <style> highlight tag to every segment without <link> attached and also replace any <color> with self (So only <link></link> text regions are protected (Keywords)), sub/sup weird on keywords] */
                if (RichText.ContainsOneOf("\xFFFEStyleHighlighter\xF8FEstyle\xFFFF", "\xFFFESubscript\xF8FEsub\xFFFF", "\xFFFESuperscript\xF8FEsup\xFFFF"))
                {
                    foreach (StableTextConstruction TextSegment in @Segments)
                    {
                        if (TextSegment.IsTagItself &&
                            (TextSegment.InnerTagData.Type == TagType.StyleHighlighter | TextSegment.InnerTagData.Type == TagType.Subscript | TextSegment.InnerTagData.Type == TagType.Superscript))
                        {
                            ApplyCascadeSequence(@Segments, CurrentIndex, TextSegment.InnerTagData,
                                ButRemoveThisTag: TagType.Color,
                                ButIgnoreIfThisTagOccurs: TagType.Link);
                        }
                        CurrentIndex++;
                    }
                    CurrentIndex = 0;
                }

                /* THIRD ORDER TAGS [<nobr>, apply only when everything is processed and text segments is at their final form] */
                if (RichText.Contains("\xFFFENoBreak\xF8FEnobr\xFFFF"))
                {
                    foreach (StableTextConstruction TextSegment in @Segments)
                    {
                        if (TextSegment.IsTagItself && TextSegment.InnerTagData.Type == TagType.NoBreak)
                        {
                            TextSegment.IsTagItself = false;
                            if (CurrentIndex != MaxTotalIndex)
                            {
                                int SubNoBreakSegmentIndex = CurrentIndex + 1;
                                foreach (StableTextConstruction SubNoBreakSegmentSegment in Segments[SubNoBreakSegmentIndex..])
                                {
                                    if (SubNoBreakSegmentSegment.IsTagItself &&
                                        SubNoBreakSegmentSegment.TextSentence.MatchesWidthOneOf(RegisteredTags[TagType.NoBreak].BreakPoints)
                                    ) break;
                                    else
                                    {
                                        string StoredTextSentence = SubNoBreakSegmentSegment.TextSentence;
                                        SubNoBreakSegmentSegment.TextSentence = ""; // To remove at EstablishedSequence
                                        TextSegment.InnerTagData.SpriteOrNoBreakData_SubItems.Add(SubNoBreakSegmentSegment with { TextSentence = StoredTextSentence });
                                    }

                                    SubNoBreakSegmentIndex++;
                                }

                                // Clear empty just as EstablishedSequence
                                TextSegment.InnerTagData.SpriteOrNoBreakData_SubItems = TextSegment.InnerTagData.SpriteOrNoBreakData_SubItems.Where(x => x.TextSentence != "").ToList();
                            }
                        }
                        CurrentIndex++;
                    }
                    CurrentIndex = 0;
                }

                List<StableTextConstruction> EstablishedSequence = new List<StableTextConstruction>(@Segments.Where(x => (!x.IsTagItself & x.TextSentence != "")));

                ProcessEstablishedSequence(EstablishedSequence, Target);
            }


            private static void ProcessEstablishedSequence(List<StableTextConstruction> TargetSequence, TextBlock TargetTextBlock, bool NoBreakMode = false)
            {
                #if DebugPrintInfo
                if (!NoBreakMode)
                {
                    Console.Clear();
                    Console.WriteLine("\x1b[3J");
                }
                #endif

                TargetTextBlock.Text = ""; // Reset text

                int Total = TargetSequence.Where(x => !x.IsTagItself).Count();
                int Counter = 1;
                foreach (StableTextConstruction SequenceItem in TargetSequence)
                {
                    if (SequenceItem.InnerTagData == null)
                    {
                        #if DebugPrintInfo
                        PrintDebugInfo_RegularText(SequenceItem, AddAsNoBreakSegment: NoBreakMode, IsLastFromNoBreak: Counter == Total);
                        #endif

                        Run TextFragment = new Run(SequenceItem.TextSentence);

                        SetRunFormatting(ref TextFragment, SequenceItem.AssignedTags);

                        TargetTextBlock.Inlines.Add(TextFragment);

                        Counter++;
                    }
                    else if (SequenceItem.InnerTagData.Type == TagType.Sprite)
                    {
                        #if DebugPrintInfo
                        PrintDebugInfo_Sprite(SequenceItem, AddAsNoBreakSegment: NoBreakMode, IsLastFromNoBreak: Counter == Total);
                        #endif

                        string SpriteID = SequenceItem.InnerTagData.Info;

                        double SpriteSizeMultiplier = 1;
                        if (SequenceItem.AssignedTags.ContainsKey(TagType.SizeMultiplier))
                        {
                            if (double.TryParse(SequenceItem.AssignedTags[TagType.SizeMultiplier].Info, out double Value))
                            {
                                SpriteSizeMultiplier *= Value / (double)100;
                            }
                        }

                        BitmapImage ImageSource = @PostInfo.InlineImages.ContainsKey(SpriteID) ? @PostInfo.InlineImages[SpriteID] : @PostInfo.InlineImages["Unknown"];

                        StackPanel GrabbedText = new StackPanel() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Bottom };
                        foreach (StableTextConstruction AttachedTextItem in SequenceItem.InnerTagData.SpriteOrNoBreakData_SubItems)
                        {
                            Run GeneratedTextItem = new Run(AttachedTextItem.TextSentence);
                            SetRunFormatting(ref GeneratedTextItem, AttachedTextItem.AssignedTags, IgnoreTags: SequenceItem.AssignedTags.ContainsKey(TagType.Background) ? TagType.Background : TagType.TechnicalNull);
                            GrabbedText.Children.Add(TargetTextBlock.ImposedClone(Content: GeneratedTextItem));
                        }

                        TargetTextBlock.Inlines.Add(new InlineUIContainer()
                        {
                            BaselineAlignment = BaselineAlignment.Bottom,
                            Child = new StackPanel()
                            {
                                //Background = ToSolidColorBrush("#f95e00"), // Debug color for image containers
                                Background = SequenceItem.AssignedTags.ContainsKey(TagType.Background) ? ToSolidColorBrush(SequenceItem.AssignedTags[TagType.Background].Info) : Brushes.Transparent,
                                Orientation = Orientation.Horizontal,
                                Children =
                                {
                                    new Grid()
                                    {
                                        VerticalAlignment = VerticalAlignment.Bottom,
                                        Margin = new Thickness(0, 0, 0, 2.7),
                                        RenderTransform = new TranslateTransform() { X = @PostInfo.SpritesHorizontalOffset, Y = @PostInfo.SpritesVerticalOffset },
                                        Children =
                                        {
                                            new Image()
                                            {
                                                Source = ImageSource, Height = TargetTextBlock.FontSize * SpriteSizeMultiplier,
                                            },
                                        }
                                    },
                                    TargetTextBlock.ImposedClone(), // Placeholder TextBlock to keep vertical offset of sprite in text same as regular text even if GrabbedText is empty
                                    GrabbedText
                                }
                            }
                        });

                        Counter++;
                    }
                    else if (SequenceItem.InnerTagData.Type == TagType.NoBreak & !NoBreakMode)
                    {
                        #if DebugPrintInfo
                        PrintDebugInfo_NoBreak(SequenceItem);
                        #endif

                        TextBlock NoBreakText = TargetTextBlock.ImposedClone();
                        //NoBreakText.Background = ToSolidColorBrush("#f95e00");

                        ProcessEstablishedSequence(SequenceItem.InnerTagData.SpriteOrNoBreakData_SubItems, NoBreakText, NoBreakMode: true);

                        TargetTextBlock.Inlines.Add(new InlineUIContainer(NoBreakText) { BaselineAlignment = BaselineAlignment.Bottom });
                    }
                    else if (SequenceItem.InnerTagData.Type == TagType.InlineImage)
                    {
                        #if DebugPrintInfo
                        PrintDebugInfo_Sprite(SequenceItem, AddAsNoBreakSegment: NoBreakMode, IsLastFromNoBreak: Counter == Total);
                        #endif

                        double ImageSize = TargetTextBlock.FontSize;
                        double OffsetX = 0;
                        double OffsetY = 0;

                        if (SequenceItem.AssignedTags.ContainsKey(TagType.InlineImages_Size) &&
                            double.TryParse(SequenceItem.AssignedTags[TagType.InlineImages_Size].Info, out double SizeValue))
                        {
                            ImageSize = SizeValue;
                        }
                        if (SequenceItem.AssignedTags.ContainsKey(TagType.InlineImages_XOffset) &&
                            double.TryParse(SequenceItem.AssignedTags[TagType.InlineImages_XOffset].Info, out double XOffsetValue))
                        {
                            OffsetX = XOffsetValue;
                        }
                        if (SequenceItem.AssignedTags.ContainsKey(TagType.InlineImages_YOffset) &&
                            double.TryParse(SequenceItem.AssignedTags[TagType.InlineImages_YOffset].Info, out double YOffsetValue))
                        {
                            OffsetY = YOffsetValue;
                        }

                        TargetTextBlock.Inlines.Add(new InlineUIContainer(new Image()
                        {
                            Source = @PostInfo.InlineImages.ContainsKey(SequenceItem.InnerTagData.Info)
                                ? @PostInfo.InlineImages[SequenceItem.InnerTagData.Info]
                                : @PostInfo.InlineImages["Unknown"],

                            RenderTransform = new TranslateTransform() { X = OffsetX, Y = OffsetY },
                            Width = ImageSize
                        }) { BaselineAlignment = BaselineAlignment.Center });
                    }
                }
            }

            #region Tech info
            public static readonly string DMarker = "\x1b[48;5;196m[Debug]\x1b[0m";

            public static void PrintDebugInfo_RegularText(StableTextConstruction About, bool AddAsSpriteSegment = false, bool IsLastFromSprite = false, bool AddAsNoBreakSegment = false, bool IsLastFromNoBreak = false)
            {
                string Additional_header = "";
                string Additional_item = "";

                if (AddAsNoBreakSegment)
                {
                    if (AddAsSpriteSegment)
                    {
                        Additional_header += IsLastFromNoBreak ? "  " : "\x1b[5m│\x1b[0m ";
                    }
                    else
                    {
                        Additional_header += IsLastFromNoBreak ? "\x1b[5m└─\x1b[0m" : "\x1b[5m├─\x1b[0m";
                    }
                }
                if (AddAsNoBreakSegment)
                {
                    Additional_item += IsLastFromNoBreak ? "  " : "\x1b[5m│\x1b[0m ";
                }

                if (AddAsSpriteSegment) Additional_header += IsLastFromSprite ? "└─" : "├─";
                if (AddAsSpriteSegment) Additional_item += IsLastFromSprite ? "  " : "│ ";

                rin($"{DMarker} {Additional_header}Text \"\x1b[7m{About.TextSentence.Replace("\n", "\\n")}\x1b[0m\"");
                int TotalTags = About.AssignedTags.Count;
                int TagCounter = 1;
                foreach (KeyValuePair<TagType, InlineTagData> Tag in About.AssignedTags)
                {
                    rin($"{DMarker} {Additional_item}{(TagCounter != TotalTags ? "├" : "└")}─ ␌ [{Tag.Key}]: '{Tag.Value.Info}'");
                    TagCounter++;
                }
                if (!AddAsSpriteSegment) rin($"{DMarker}{(AddAsNoBreakSegment & !IsLastFromNoBreak ? " \x1b[5m│\x1b[0m" : "")}");
            }
            public static void PrintDebugInfo_Sprite(StableTextConstruction About, bool AddAsNoBreakSegment = false, bool IsLastFromNoBreak = false)
            {
                rin($"{DMarker} {(AddAsNoBreakSegment ? (IsLastFromNoBreak ? "\x1b[5m└─\x1b[0m" : "\x1b[5m├─\x1b[0m") : "")}Image \"@\x1b[4m{About.InnerTagData.Info}\x1b[0m\"");
                if (About.InnerTagData.SpriteOrNoBreakData_SubItems != null)
                {
                    int TotalSegments = About.InnerTagData.SpriteOrNoBreakData_SubItems.Count;
                    int SegmentCounter = 1;
                    foreach (StableTextConstruction SubTextSegment in About.InnerTagData.SpriteOrNoBreakData_SubItems)
                    {
                        PrintDebugInfo_RegularText(SubTextSegment, AddAsSpriteSegment: true, IsLastFromSprite: SegmentCounter == TotalSegments, AddAsNoBreakSegment: AddAsNoBreakSegment, IsLastFromNoBreak: IsLastFromNoBreak);
                        SegmentCounter++;
                    }
                    rin($"{DMarker}{(AddAsNoBreakSegment & !IsLastFromNoBreak ? " \x1b[5m│\x1b[0m" : "")}");
                }
            }

            public static void PrintDebugInfo_NoBreak(StableTextConstruction About)
            {
                rin($"{DMarker} \x1b[5m[No break region]\x1b[0m (:\x1b[4m{About.InnerTagData.SpriteOrNoBreakData_SubItems.Where(x => !x.IsTagItself).Count()}\x1b[0m)");

                int TotalSegments = About.InnerTagData.SpriteOrNoBreakData_SubItems.Count;
                int SegmentCounter = 1;
                foreach (StableTextConstruction NobrSegment in About.InnerTagData.SpriteOrNoBreakData_SubItems)
                {
                    SegmentCounter++;
                }
            }
            #endregion
        }
    }
}
