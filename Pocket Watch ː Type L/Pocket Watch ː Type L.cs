//#define DebugPrintInfo

using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using LC_Localization_Task_Absolute.PreviewCreator;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.Pocket_Watch_ː_Type_L.@PostInfo.FullStopDividers;
using static LC_Localization_Task_Absolute.Pocket_Watch_ː_Type_L.Tags;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;

namespace LC_Localization_Task_Absolute
{
    public interface Pocket_Watch_ː_Type_L
    {
        public ref struct @Generic
        {
            public static Dictionary<string, BitmapImage> SpriteImages => KeywordsInterrogation.KeywordImages;
            public static double SpritesVerticalOffset => (double)@CurrentConfess.SelectedCustomLang.Properties.KeywordsSpriteVerticalOffset;
            public static double SpritesHorizontalOffset => (double)@CurrentConfess.SelectedCustomLang.Properties.KeywordsSpriteHorizontalOffset;
        }
        public ref struct @PostInfo
        {
            public static readonly Dictionary<string, FontFamily> LoadedKnownFonts = new()
            {
                ["BebasKai SDF"     ] = FileToFontFamily(@"[⇲] Assets Directory\Limbus Embedded Fonts\BebasKai.otf"),
                ["ExcelsiorSans SDF"] = FileToFontFamily(@"[⇲] Assets Directory\Limbus Embedded Fonts\ExcelsiorSans.ttf"),
               
                ["KR/p)SCDream(light)/SCDream5 SDF"  ] = FileToFontFamily(@"[⇲] Assets Directory\Limbus Embedded Fonts\SCDream5.otf"),
                ["KR/title)KOTRA_BOLD/KOTRA_BOLD SDF"] = FileToFontFamily(@"[⇲] Assets Directory\Limbus Embedded Fonts\KOTRA_BOLD.ttf", "KOTRA BOLD"),
                
                ["EN/title)mikodacs/Mikodacs SDF"      ] = FileToFontFamily(@"[⇲] Assets Directory\Limbus Embedded Fonts\Mikodacs.otf"),
                ["EN/Pretendard/Pretendard-Regular SDF"] = FileToFontFamily(@"[⇲] Assets Directory\Limbus Embedded Fonts\Pretendard-Regular.ttf"),
                ["EN/cur)Caveat-SemiBold/Caveat-SemiBold SDF"] = FileToFontFamily(@"[⇲] Assets Directory\Limbus Embedded Fonts\Caveat SemiBold.ttf"),

                ["JP/HigashiOme/HigashiOme-Gothic-C-1"                       ] = FileToFontFamily(@"[⇲] Assets Directory\Limbus Embedded Fonts\HigashiOme-Gothic-C-1.3.ttf"),
                ["JP/title)corporate logo(bold)/Corporate-Logo-Bold-ver2 SDF"] = FileToFontFamily(@"[⇲] Assets Directory\Limbus Embedded Fonts\Corporate-Logo-Bold-ver2.otf"),
            };

            public static readonly bool DoLineBreakWithSprites = true;

            public static readonly TagType[] IgnoreTags_Default = [];
            public static readonly TagType[] IgnoreTags_UnityTMProExclude =
            [
                TagType.Hyperlink,
                TagType.Background, // <mark> but behind the text, not valid
                TagType.FontStretch,
                TagType.InlineImage,
                TagType.InlineImages_Size,
                TagType.InlineImages_XOffset,
                TagType.InlineImages_YOffset,
            ];

            // FontStretch and FontWeight is not Enum :'
            // https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/RichTextFontWeight.html
            public static readonly Dictionary<string, FontWeight> UnityTMProFontWeightValues = new()
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
            public static readonly Dictionary<string, FontStretch> FontStretchValues = new()
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

            /*lang=regex*/
            public ref struct FullStopDividers
            {
                public readonly record struct FullStopDivider(string Open, string Close);


                public static readonly List<FullStopDivider> FullStopDividers_TMPro = [new FullStopDivider(Open:@"<", Close:@">")];
                public static readonly List<FullStopDivider> FullStopDividers_Regular =
                [
                    new FullStopDivider(Open:@"\[", Close:@"\]"),
                    new FullStopDivider(Open:@"<" , Close:@">"),
                ];
            }
        }
        public static class @Tags
        {
            public enum TagType
            {
                CloseSequence, // BreakPoints automatically applied type
                NaN, // Because of ContainsKey(null) for notnull keys at dictionaries

                NoBreak,
                Hyperlink,
                Link,
                Bold,
                Underline,
                Italic,
                Strikethrough,
                Subscript,
                Superscript,
                StyleHighlighter,
                Color,
                Background,
                Mark,
                Font,
                FontWeight,
                FontStretch,
                SizeMultiplier,
                Sprite,
                InlineImage,
                InlineImages_Size,
                InlineImages_XOffset,
                InlineImages_YOffset,

                FlavorTextOverrideFormatting,
                SizeMultiplier85_Override,
            };



            public readonly record struct PatternAndTypeMatcher(TagType MainTag, TagType? CloseSequenceParentTag = null);

            public static readonly Dictionary<string, PatternAndTypeMatcher> @PatternAndTypeMatch = [];
            public static readonly Dictionary<TagType, FullStopTag> @RegisteredTags = [];

            public readonly record struct FullStopTag
            {
                // Regex patterns for tag endings (Modified to @"^String$")
                public string[] BreakPoints { get; }

                public Dictionary<DependencyProperty, object> UnivocalProperties { get; } = null;

                public Regex TagRegex { get; }

                public dynamic ValueGroupNameOrNumber { get; }

                /// <param name="PatternToMatch">Pattern that will be used to determine actual tags in the text by regular expression, tag value is supposted to be inside the first match group</param>
                /// <param name="BreakPointsInput">Breakpoints that stops tag from applying</param>
                /// <param name="Type"><see cref="TagType"/> enum that indicates identifier</param>
                /// <param name="Univocal">
                ///  Dictionary with explicit properties and values to set for <see cref="Run"/> inline objects instead of processing them manually
                ///  <br/>
                ///  inside <see cref="Actions.SetRunFormatting(Run, Dictionary{TagType, Actions.InlineTagData}, TagType[])"/>
                /// </param>
                /// <param name="ValueGroupNameOrNumber">Used for tag <see cref="Match.Groups"/> to get value of tag, 1 by default (First group),<br/>you can also put string there as name of the group because <see cref="Match.Groups"/> accepts both <see cref="int"/> and <see cref="string"/> as key (Group ordinal number or its name)</param>
                public FullStopTag(string PatternToMatch, string[] BreakPointsInput, TagType Type, Dictionary<DependencyProperty, object> Univocal = null, dynamic ValueGroupNameOrNumber = null)
                {
                    ValueGroupNameOrNumber ??= 1;

                    this.ValueGroupNameOrNumber = ValueGroupNameOrNumber;

                    if (Univocal != null) this.UnivocalProperties = Univocal;
                    if (BreakPointsInput != null)
                    {
                        this.BreakPoints = [.. BreakPointsInput.Select(BreakPoinString => $"^{BreakPoinString}$")];
                        foreach (string BreakPoint in BreakPointsInput)
                        {
                            // Assing close tags
                            PatternAndTypeMatch[BreakPoint] = new PatternAndTypeMatcher(TagType.CloseSequence, CloseSequenceParentTag: Type);
                        }
                    }

                    TagRegex = new Regex(PatternToMatch, RegexOptions.Compiled);

                    PatternAndTypeMatch[PatternToMatch] = new PatternAndTypeMatcher(Type);

                    if (!@RegisteredTags.ContainsKey(Type)) @RegisteredTags[Type] = this;
                    else throw new Exception($"Tag definition with same type already defined ({Type})");
                }
            }

            /* lang=regex */
            /// <summary>
            /// All tags is being defined there
            /// </summary>
            public static readonly List<FullStopTag> TagDefinitions = new()
            {
                                                        // Evident tag types, regardless of values like in <font> or <color>
                new("b", ["/b"], TagType.Bold,          Univocal: new() { [Run.FontWeightProperty] = FontWeights.Bold }),
                new("i", ["/i"], TagType.Italic,        Univocal: new() { [Run.FontStyleProperty] = FontStyles.Italic }),
                new("u", ["/u"], TagType.Underline,      Univocal: new() { [Run.TextDecorationsProperty] = TextDecorations.Underline }),
                new("s", ["/s"], TagType.Strikethrough, Univocal: new() { [Run.TextDecorationsProperty] = TextDecorations.Strikethrough }),

                new("nobr", ["/nobr"], TagType.NoBreak),

                new("sub", ["/sub"], TagType.Subscript),
                new("sup", ["/sup"], TagType.Superscript),

                // Unique tag only for Limbus texts to forcibility set specific formatting without ability to remove it with </color></i>
                new(@"flavor\uAAFF", [@"/flavor\uAAFF"], TagType.FlavorTextOverrideFormatting, Univocal: new() { [Run.ForegroundProperty] = ToSolidColorBrush("#9f6a3a"), [Run.FontStyleProperty] = FontStyles.Italic }),
                new(@"size\uAAFF", [@"/size\uAAFF"], TagType.SizeMultiplier85_Override),
                
                // new("noparse", ["/noparse"]), Being applied on formatting stage as \0 after tag dividers in <noparse></noparse> range, see at Apply()

                new("style", ["/style"], TagType.StyleHighlighter, Univocal:  new() { [Run.ForegroundProperty] = ToSolidColorBrush("#f8c200") }), // Highlight color
                
                new(@"hyperlink=""(.*?)""", ["/hyperlink"], TagType.Hyperlink),

                new(@"link=""(\w+)""", ["/link"], TagType.Link),
                new(@"font=""(.*?)""", ["/font"], TagType.Font),
                new(@"font-weight=""(100|200|300|400|500|600|700|800|900)""", ["/font-weight"], TagType.FontWeight), // https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/RichTextFontWeight.html
                new(@"font-stretch=""(Condensed|Expanded|ExtraCondensed|ExtraExpanded|Medium|Normal|SemiCondensed|SemiExpanded|UltraCondensed|UltraExpanded)""", ["/font-stretch"], TagType.FontStretch),
                new(@"size=((\d+)((\.|\,)\d+)?)%", ["/size"], TagType.SizeMultiplier),
                new(@"color=#([a-fA-F0-9]{8}|[a-fA-F0-9]{6})", ["/color"], TagType.Color),
                new(@"background=#([a-fA-F0-9]{8}|[a-fA-F0-9]{6})", ["/background"], TagType.Background),
                new(@"mark( color)?=#([a-fA-F0-9]{8}|[a-fA-F0-9]{6})", ["/mark"], TagType.Mark, ValueGroupNameOrNumber: 2),
                new(@"sprite name=""(\w+)""", null, TagType.Sprite),

                new(@"image id=""(\w+)""", null, TagType.InlineImage),
                new(@"images-size=(\d+)((\.|\,)\d+)?", ["/images-size"], TagType.InlineImages_Size),
                new(@"images-xoffset=((\-|\+)?(\d+)((\.|\,)\d+)?)", ["/images-xoffset"], TagType.InlineImages_XOffset),
                new(@"images-yoffset=((\-|\+)?(\d+)((\.|\,)\d+)?)", ["/images-yoffset"], TagType.InlineImages_YOffset),
            };
        }

        public static class Actions
        {
            private static void ApplyInlineTagDataFormatting(Run TargetRun, KeyValuePair<TagType, InlineTagData> Tag)
            {
                TextBlock ParentTextBlock = (@RecentInfo.IsApplyingRichTextToTheKeywordPopup ? @RecentInfo.TextBlockTarget_KeywordPopup : @RecentInfo.TextBlockTarget);

                string TagInfo = Tag.Value.Info;
                switch (Tag.Key) // Tag actions
                {
                    case TagType.Color: TargetRun.Foreground = ToSolidColorBrush(TagInfo, AlphaAtTheEnd: @RecentInfo.IsLimbusText);
                        break;

                    case TagType.Background: TargetRun.Background = ToSolidColorBrush(TagInfo, AlphaAtTheEnd: @RecentInfo.IsLimbusText);
                        break;

                    case TagType.Mark: TargetRun.Background = ToSolidColorBrush(TagInfo, AlphaAtTheEnd: @RecentInfo.IsLimbusText);
                        break;

                    case TagType.Link:
                        TargetRun.Name = TagInfo; // Name = Keyword ID

                        KeywordsInterrogation.KeywordDescriptionInfoPopup.AttachToInline(TargetRun);

                        break;

                    case TagType.Hyperlink:
                        TargetRun.PreviewMouseLeftButtonUp += (Sender, Args) =>
                        {
                            Process.Start(new ProcessStartInfo()
                            {
                                FileName = TagInfo, /*URL*/
                                UseShellExecute = true
                            });
                        };

                        ToolTipService.SetInitialShowDelay(TargetRun, 100);
                        TargetRun.ToolTip = new ToolTip()
                        {
                            Background = ToSolidColorBrush("#E0676767"),
                            HorizontalOffset = 20,
                            Content = new TextBlock()
                            {
                                FontSize = 12,
                                Foreground = Brushes.White,
                                Text = TagInfo /*URL*/,
                            }.BindSamePropertiesWithReturn(ParentTextBlock, [
                                TextBlock.FontFamilyProperty, TextBlock.FontWeightProperty
                            ])
                        };
                        break;

                    case TagType.Font:
                        if (@PostInfo.LoadedKnownFonts.ContainsKey(TagInfo))
                        {
                            TargetRun.FontFamily = @PostInfo.LoadedKnownFonts[TagInfo];
                        }
                        else if (!ParentTextBlock.Name.StartsWith($"PreviewLayout_")) // If not TMProEmitter limbus textblock
                        {
                            if (InterfaceLocalizationModifiers.Font_References_Loaded.ContainsKey(TagInfo))
                            {
                                TargetRun.FontFamily = InterfaceLocalizationModifiers.Font_References_Loaded[TagInfo];
                            }
                            else TargetRun.FontFamily = new FontFamily(TagInfo);
                        }
                        break;

                    case TagType.FontWeight:
                        TargetRun.FontWeight = @PostInfo.UnityTMProFontWeightValues[TagInfo];
                        break;

                    case TagType.FontStretch:
                        TargetRun.FontStretch = @PostInfo.FontStretchValues[TagInfo];
                        break;

                    case TagType.SizeMultiplier:
                        if (double.TryParse(TagInfo.Replace(".", ","), out double FontSizeMultiplyValue))
                        {
                            double ApplyValue = FontSizeMultiplyValue / 100.0;
                            if (ApplyValue == 0) ApplyValue = 0.01;
                            TargetRun.FontSize = ParentTextBlock.FontSize * ApplyValue;
                        }
                        break;

                    case TagType.SizeMultiplier85_Override:
                        TargetRun.FontSize = ParentTextBlock.FontSize * 0.85;
                        break;

                    case TagType.Subscript:
                        TargetRun.FontSize = ParentTextBlock.FontSize * 0.7;
                        TargetRun.BaselineAlignment = BaselineAlignment.Subscript;
                        break;

                    case TagType.Superscript:
                        TargetRun.FontSize = ParentTextBlock.FontSize * 0.7;
                        TargetRun.BaselineAlignment = BaselineAlignment.Superscript;
                        break;
                }
            }


            private static void SetRunFormatting
            (
                Run Target,
                Dictionary<TagType, InlineTagData> AssignedTags,
                params TagType[] IgnoreTags
            )
            {
                foreach (KeyValuePair<TagType, InlineTagData> Tag in AssignedTags)
                {
                    if (!IgnoreTags.Contains(Tag.Key))
                    {
                        if (@RegisteredTags[Tag.Key].UnivocalProperties != null)
                        {
                            foreach (var Univocal in @RegisteredTags[Tag.Key].UnivocalProperties)
                            {
                                Target.SetValue(Univocal.Key, Univocal.Value);
                            }
                        }
                        else
                        {
                            ApplyInlineTagDataFormatting(Target, Tag);
                        }
                    }
                }
            }


            
            /// <summary>
            /// Part of the text splitted by tags, can be regular text part or tag value expression itself
            /// </summary>
            public record StableTextConstruction
            {
                /// <summary>
                /// Tag info or plain text
                /// </summary>
                public string TextSentence { get; set; }

                /// <summary>
                /// Tags and their info to apply to this <see cref="StableTextConstruction"/> if it is regular text part
                /// </summary>
                public Dictionary<TagType, InlineTagData> AssignedTags { get; } = [];

                /// <summary>
                /// <see langword="true"/> if this <see cref="StableTextConstruction"/> is tag (Will be set to <see langword="false"/> for <see cref="TagType.Sprite"/> or <see cref="TagType.InlineImage"/> during <see cref="Actions.Apply(TextBlock, string, List{FullStopDivider}, bool, TagType[])"/>)
                /// </summary>
                public bool IsTagItself { get; set; } = false; // Can be changed during process

                /// <summary>
                /// Tag data if tag itself
                /// </summary>
                public InlineTagData InnerTagData { get; }

                public StableTextConstruction(string BaseTextSentence)
                {
                    this.TextSentence = BaseTextSentence;

                    if (BaseTextSentence.Contains('\xF8FE')) // If tag expression ( $"\xFFFE{TagType}\xF8FE{Tag Info}\xFFFF" where xF8FE is type and info divider)
                    {
                        this.IsTagItself = true;
                        this.InnerTagData = new InlineTagData(this.TextSentence, out string PlainTagInfoReturn);
                        this.TextSentence = PlainTagInfoReturn;
                    }
                }
            }

            /// <summary>
            /// Special info created for <see cref="StableTextConstruction"/> object if it is tag
            /// </summary>
            public record InlineTagData
            {
                public TagType Type { get; }

                public string Info { get; }

                public string[] BreakPoints { get; }

                /// <summary>
                /// Another '@EstablishedSequence' specially for &lt;sprite&gt; or &lt;nobr&gt; tags that supposed to contain their own text sequences
                /// <br/>
                /// <br/>
                /// (Sprite: first word from next StableTextConstruction + quote mark after) (NoBreak: all StableTextConstruction objects inside)
                /// </summary>
                public List<StableTextConstruction>? SpriteOrNoBreakData_SubItems { get; set; } = null;

                public InlineTagData(string BaseTextString, out string PlainTagInfoReturn)
                {
                    string[] TagDefinition = BaseTextString.Split('\xF8FE');

                    this.Type = Enum.Parse<TagType>(TagDefinition[0]);
                    this.Info = PlainTagInfoReturn = TagDefinition[^1];

                    if (this.Type != TagType.CloseSequence)
                    {
                        this.Info = @RegisteredTags[this.Type].TagRegex.Match(this.Info).Groups[@RegisteredTags[this.Type].ValueGroupNameOrNumber].Value;

                        this.BreakPoints = @RegisteredTags[this.Type].BreakPoints;
                        if (this.Type == TagType.Sprite | this.Type == TagType.NoBreak)
                        {
                            this.SpriteOrNoBreakData_SubItems = new List<StableTextConstruction>();
                        }
                    }
                }
            }


            private static void ApplyCascadeSequence
            (
                List<StableTextConstruction> @Sequence,
                int TextSegmentStartIndex,
                InlineTagData TagDataToAdd,
                TagType? ButRemoveThisTag = null,
                TagType ButIgnoreIfThisTagOccurs = TagType.NaN
            )
            {
                int SkipWhileThereAreSameTagOpened_Stack = 0;
                foreach (StableTextConstruction MidTextSegment in @Sequence[(TextSegmentStartIndex + 1)..])
                {
                    if (MidTextSegment.IsTagItself && MidTextSegment.InnerTagData.Type == TagDataToAdd.Type)
                    {
                        // Just as in Unity TextMeshPro, at text fragment "<color=#e30000><color=#f8c200>Yellow</color> and there are red</color>" 'and there are red' part still must me colored red
                        SkipWhileThereAreSameTagOpened_Stack++;
                    }

                    if (SkipWhileThereAreSameTagOpened_Stack == 0)
                    {
                        if (MidTextSegment.TextSentence.MatchesWithOneOf(TagDataToAdd.BreakPoints)) break;

                        if (!MidTextSegment.AssignedTags.ContainsKey(ButIgnoreIfThisTagOccurs))
                        {
                            if (!MidTextSegment.IsTagItself)
                            {
                                MidTextSegment.AssignedTags[TagDataToAdd.Type] = TagDataToAdd;

                                if (ButRemoveThisTag != null)
                                {
                                    MidTextSegment.AssignedTags.Remove((TagType)ButRemoveThisTag);
                                }
                            }
                            else if (MidTextSegment.IsTagItself && (MidTextSegment.InnerTagData.Type == TagType.InlineImage || MidTextSegment.InnerTagData.Type == TagType.Sprite))
                            {
                                MidTextSegment.AssignedTags[TagDataToAdd.Type] = TagDataToAdd;
                            }
                        }
                    }
                    else if (MidTextSegment.TextSentence.MatchesWithOneOf(TagDataToAdd.BreakPoints))
                    {
                        SkipWhileThereAreSameTagOpened_Stack--;
                    }
                }
            }

            public ref struct @RecentInfo
            {
                public static double CurrentInlinesHeight => (IsApplyingRichTextToTheKeywordPopup ? TextBlockTarget_KeywordPopup : TextBlockTarget).GetInlineTextHeight();
                public static bool IsLimbusText => (IsApplyingRichTextToTheKeywordPopup ? TextBlockTarget_KeywordPopup : TextBlockTarget) is TMProEmitter;

                public static TextBlock TextBlockTarget { get; set; } = null;
                public static TextBlock TextBlockTarget_KeywordPopup = null;

                public static bool IsApplyingRichTextToTheKeywordPopup = false;
            }

            public static void Apply
            (
                TextBlock Target,
                string RichText,
                List<FullStopDivider> DividersMode = null,
                bool DisableKeyworLinksCreation = false,
                params TagType[] IgnoreTags
            ) {
                if (!LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.EnableKeywordTooltips) DisableKeyworLinksCreation = true;

                if (@CurrentPreviewCreator.IsActive) DisableKeyworLinksCreation = true;
                
                DividersMode ??= FullStopDividers_Regular;

                if (IgnoreTags != null) IgnoreTags = [.. IgnoreTags.Union(@PostInfo.IgnoreTags_Default)];
                else IgnoreTags = @PostInfo.IgnoreTags_Default;

                RichText = Regex.Replace(RichText, @"<style=(""highlight""|""upgradeHighlight"")>", "<style>"); // Unify to just <style></style>, doesn't matter -> yellow color

                if (!DisableKeyworLinksCreation)
                {
                    @RecentInfo.TextBlockTarget = Target;
                    @RecentInfo.IsApplyingRichTextToTheKeywordPopup = false;
                }
                else
                {
                    @RecentInfo.TextBlockTarget_KeywordPopup = Target;
                    @RecentInfo.IsApplyingRichTextToTheKeywordPopup = true;
                }

                foreach (FullStopDivider FullStopTagDivider in DividersMode)
                {
                    string TagOpen = FullStopTagDivider.Open;
                    string TagClose = FullStopTagDivider.Close;

                    if (RichText.Contains("noparse"))
                    {
                        RichText = Regex.Replace(RichText, @$"{TagOpen}noparse{TagClose}(.*?){TagOpen}/noparse{TagClose}", Match =>
                        {
                            return Match.Groups[1].Value.Replace(TagOpen, $"{TagOpen}\u0001");
                        }, RegexOptions.Singleline);
                    }

                    foreach (KeyValuePair<string, PatternAndTypeMatcher> TagInfo in PatternAndTypeMatch)
                    {
                        string TagBodyPattern = TagInfo.Key;
                        TagType TagEnumSign = TagInfo.Value.MainTag;

                        TagType TagEnumSignToCheckIgnore = (TagType)(TagInfo.Value.CloseSequenceParentTag != null ? TagInfo.Value.CloseSequenceParentTag : TagInfo.Value.MainTag);

                        // <color=#f8c200> -> '{\xFFFE}TagType.Color{\xF8FE}color=#f8c200{\xFFFF}' where {\x} is special unicode character
                        if (!IgnoreTags.Contains(TagEnumSignToCheckIgnore))
                        {
                            RichText = Regex.Replace(RichText, TagOpen+$"({TagBodyPattern})"+TagClose, Match =>
                            {
                                if (DisableKeyworLinksCreation && TagEnumSignToCheckIgnore == TagType.Link)
                                {
                                    return "";
                                }
                                else
                                {
                                    return $"\xFFFE{TagEnumSign}\xF8FE{Match.Groups[1].Value}\xFFFF"; // Where xF8FE is type and info divider
                                }
                            });
                        }
                    }
                }

                List<StableTextConstruction> @Segments = [.. RichText.Replace("\r", "").Split(['\xFFFE', '\xFFFF'], StringSplitOptions.RemoveEmptyEntries).Select(RegularText => new StableTextConstruction(RegularText))];


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
                            if (@PostInfo.DoLineBreakWithSprites & (CurrentIndex != MaxTotalIndex & TextSegment.InnerTagData.Type == TagType.Sprite))
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
                                    else if (SubSpriteSegment.InnerTagData.Type == TagType.Sprite) break; // Also break because no text encountered and new sprite instance started so there are new priority
                                    // else encountered with tag

                                    SubSpriteSegmentIndex++;
                                }
                            }
                        }
                    }

                    CurrentIndex++;
                }
                CurrentIndex = 0;

                /* SECOND ORDER TAGS [Apply <style> highlight tag to every segment without <link> attached and also replace any <color> with self (So only <link></link> text regions are (Keywords)), sub/sup weird on keywords] */
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
                                        SubNoBreakSegmentSegment.TextSentence.MatchesWithOneOf(@RegisteredTags[TagType.NoBreak].BreakPoints)
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
                                TextSegment.InnerTagData.SpriteOrNoBreakData_SubItems = [.. TextSegment.InnerTagData.SpriteOrNoBreakData_SubItems.Where(x => x.TextSentence != "")];
                            }
                        }
                        CurrentIndex++;
                    }
                    CurrentIndex = 0;
                }

                List<StableTextConstruction> EstablishedSequence = [.. @Segments.Where(x => !x.IsTagItself & x.TextSentence != "")];

                ProcessEstablishedSequence(EstablishedSequence, Target);

                if (@RecentInfo.IsApplyingRichTextToTheKeywordPopup)
                {
                    @RecentInfo.IsApplyingRichTextToTheKeywordPopup = false;
                }
            }

            private static void ProcessEstablishedSequence
            (
                List<StableTextConstruction> TargetSequence,
                TextBlock TargetTextBlock,
                bool NoBreakMode = false
            )
            {
                #if DebugPrintInfo
                if (!NoBreakMode)
                {
                    Console.Clear();
                    Console.WriteLine("\x1b[3J");
                }
                #endif

                TargetTextBlock.Inlines.Clear(); // Reset text

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

                        SetRunFormatting(TextFragment, SequenceItem.AssignedTags);

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
                            if (double.TryParse(SequenceItem.AssignedTags[TagType.SizeMultiplier].Info.Replace(".", ","), out double Value))
                            {
                                SpriteSizeMultiplier *= Value / 100.0;
                            }
                        }

                        BitmapImage ImageSource = @Generic.SpriteImages.ContainsKey(SpriteID) ? @Generic.SpriteImages[SpriteID] : @Generic.SpriteImages["Unknown"];

                        StackPanel GrabbedText = new() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Bottom };
                        foreach (StableTextConstruction AttachedTextItem in SequenceItem.InnerTagData.SpriteOrNoBreakData_SubItems)
                        {
                            Run GeneratedTextItem = new Run(AttachedTextItem.TextSentence);
                            SetRunFormatting(GeneratedTextItem, AttachedTextItem.AssignedTags, IgnoreTags: SequenceItem.AssignedTags.ContainsKey(TagType.Background) ? TagType.Background : TagType.NaN);
                            GrabbedText.Children.Add(TargetTextBlock.ImposedClone(Content: GeneratedTextItem));
                        }

                        double SpriteHeightWidth = TargetTextBlock.FontSize * SpriteSizeMultiplier;

                        TargetTextBlock.Inlines.Add(new InlineUIContainer()
                        {
                            BaselineAlignment = BaselineAlignment.Bottom,
                            Child = new StackPanel()
                            {
                                //Background = ToSolidColorBrush("#f95e00"), // Debug color for image containers

                                Orientation = Orientation.Horizontal,
                                Children =
                                {
                                    new Grid() // Sprite background
                                    {
                                        Background = SequenceItem.AssignedTags.ContainsKey(TagType.Background)
                                            ? ToSolidColorBrush(SequenceItem.AssignedTags[TagType.Background].Info, AlphaAtTheEnd: @RecentInfo.IsLimbusText)
                                            : SequenceItem.AssignedTags.ContainsKey(TagType.Mark)
                                                ? ToSolidColorBrush(SequenceItem.AssignedTags[TagType.Mark].Info, AlphaAtTheEnd: @RecentInfo.IsLimbusText)
                                                : Brushes.Transparent,
                                        Height = @RecentInfo.CurrentInlinesHeight,
                                        Width = SpriteHeightWidth,
                                        Margin = new Thickness(0, 0, -SpriteHeightWidth, 0)
                                    },
                                    new Grid()
                                    {
                                        VerticalAlignment = VerticalAlignment.Bottom,
                                        Margin = new Thickness(0, 0, 0, 2.7),
                                        RenderTransform = new TranslateTransform()
                                        {
                                            X = @Generic.SpritesHorizontalOffset,
                                            Y = @Generic.SpritesVerticalOffset
                                        },
                                        Children =
                                        {
                                            new Image()
                                            {
                                                Width = SpriteHeightWidth,
                                                Height = SpriteHeightWidth,
                                                Source = ImageSource,
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
                            double.TryParse(SequenceItem.AssignedTags[TagType.InlineImages_Size].Info.Replace(".", ","), out double SizeValue))
                        {
                            ImageSize = SizeValue;
                        }
                        if (SequenceItem.AssignedTags.ContainsKey(TagType.InlineImages_XOffset) &&
                            double.TryParse(SequenceItem.AssignedTags[TagType.InlineImages_XOffset].Info.Replace(".", ","), out double XOffsetValue))
                        {
                            OffsetX = XOffsetValue;
                        }
                        if (SequenceItem.AssignedTags.ContainsKey(TagType.InlineImages_YOffset) &&
                            double.TryParse(SequenceItem.AssignedTags[TagType.InlineImages_YOffset].Info.Replace(".", ","), out double YOffsetValue))
                        {
                            OffsetY = YOffsetValue;
                        }

                        BitmapImage Source = new();
                        if (KeywordsInterrogation.KeywordImages.ContainsKey(SequenceItem.InnerTagData.Info))
                        {
                            Source = KeywordsInterrogation.KeywordImages[SequenceItem.InnerTagData.Info];
                        }
                        // Try check E.G.O Gift integer ID
                        else if (int.TryParse(SequenceItem.InnerTagData.Info, out int IntID))
                        {
                            if (Mode_EGOGifts.OrganizedData.DisplayInfo_Icons.ContainsKey(IntID))
                            {
                                Source = Mode_EGOGifts.OrganizedData.DisplayInfo_Icons[IntID];
                            }
                        }
                        else
                        {
                            Source = KeywordsInterrogation.KeywordImages["Unknown"];
                        }

                        TargetTextBlock.Inlines.Add(new InlineUIContainer()
                        {
                            BaselineAlignment = BaselineAlignment.Top,
                            Child = new Canvas()
                            {
                                Children =
                                {
                                    new Image()
                                    {
                                        Source = Source,
                                        Margin = new Thickness(OffsetX, OffsetY, 0, 0),
                                        RenderTransformOrigin = new Point(0.5, 0.5),
                                        Width = ImageSize,
                                        Height = ImageSize,
                                    }
                                },
                                Width = ImageSize,
                                VerticalAlignment = VerticalAlignment.Top
                            },
                        });
                    }
                }
            }

            #region Tech info
            private static readonly string DMarker = "\x1b[48;5;196m[Debug]\x1b[0m";
            private static void PrintDebugInfo_RegularText
            (
                StableTextConstruction About,
                bool AddAsSpriteSegment = false,
                bool IsLastFromSprite = false,
                bool AddAsNoBreakSegment = false,
                bool IsLastFromNoBreak = false
            )
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
            private static void PrintDebugInfo_Sprite
            (
                StableTextConstruction About,
                bool AddAsNoBreakSegment = false,
                bool IsLastFromNoBreak = false
            )
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
            private static void PrintDebugInfo_NoBreak(StableTextConstruction About)
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
