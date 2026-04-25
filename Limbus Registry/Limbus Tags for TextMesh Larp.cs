using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using System.Diagnostics.CodeAnalysis;
using static LCLocalizationInterface.TextMeshLarp;

namespace LCLocalizationInterface.LimbusRegistry
{
    public class ImportableLimbusTags : ImportableRichTextTags
    {
        private static FontFamily Font(string FileName) => FontFamilyFromFileOrName(@$"[⇲] Assets Directory\Limbus Embedded Fonts\{FileName}");
        private static Dictionary<string, FontFamily> UnityFontAssets = new()
        {
            ["BebasKai SDF"] = Font(@"BebasKai.otf"),
            ["ExcelsiorSans SDF"] = Font(@"ExcelsiorSans.ttf"),
            ["EN/cur)Caveat-SemiBold/Caveat-SemiBold SDF"] = Font(@"Caveat SemiBold.ttf"),

            ["EN/title)mikodacs/Mikodacs SDF"] = Font(@"Mikodacs.otf"),
            ["EN/Pretendard/Pretendard-Regular SDF"] = Font(@"Pretendard-Regular.ttf"),

            ["KR/title)KOTRA_BOLD/KOTRA_BOLD SDF"] = Font(@"KOTRA_BOLD.ttf"),
            ["KR/p)SCDream(light)/SCDream5 SDF"] = Font(@"SCDream5.otf"),

            ["JP/title)corporate logo(bold)/Corporate-Logo-Bold-ver2 SDF"] = Font(@"Corporate-Logo-Bold-ver2.otf"),
            ["JP/HigashiOme/HigashiOme-Gothic-C-1"] = Font(@"HigashiOme-Gothic-C-1.3.ttf"),
        };

        public override List<TagsExportingActionDelegate> PrecedingTagsExportingActions => [(TargetRegistry) => {
            // Rewrite TagProjections logic for some exiting tags

            TagsPreset.Color.TagProjections = [(Context) => {
                Context.CreatedInline.Foreground = ToSolidColorBrush($"{Context.StartExpressionMatch.Groups["ColorValue"]}", AlphaAtTheEnd: Context.RichTextGenerationContext.TargetTextBlock is TMProEmitter);
            }];
            TagsPreset.Background.TagProjections = [(Context) => {
                Context.CreatedInline.Background = ToSolidColorBrush($"{Context.StartExpressionMatch.Groups["ColorValue"]}", AlphaAtTheEnd: Context.RichTextGenerationContext.TargetTextBlock is TMProEmitter);
            }];

            TagsPreset.FontWeight.StartExpression = @"font-weight=""(?<WeightValue>100|200|300|400|500|600|700|800|900)""";
            TagsPreset.FontWeight.TagProjections = [(Context) => {
                // https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/RichTextFontWeight.html
                Dictionary<string, FontWeight> UnityTMProFontWeightValues = new()
                {
                    ["100"] = FontWeights.Thin, ["200"] = FontWeights.ExtraLight, ["300"] = FontWeights.Light, ["400"] = FontWeights.Regular, ["500"] = FontWeights.Medium, ["600"] = FontWeights.SemiBold, ["700"] = FontWeights.Bold, ["800"] = FontWeights.Heavy, ["900"] = FontWeights.Black,
                };
                Context.CreatedInline.FontWeight = UnityTMProFontWeightValues[Context.StartExpressionMatch.Groups["WeightValue"].Value];
            }];

            TagsPreset.FontFamily.TagProjections = [(Context) => {
                string FontExpression = $"{Context.StartExpressionMatch.Groups["FontExpression"]}";
                Context.CreatedInline.FontFamily = UnityFontAssets.TryGetValue(FontExpression, out FontFamily? LimbusFontAsset) ? LimbusFontAsset : FontFamilyFromFileOrName(FontExpression);
            }];
        }];

        public override List<TagDefinition> TagDefinitions =>
        [
            Mark,
            KeywordDescTooltip,
            ChangesHighlight,
            Sprite
        ];

        public static readonly TagDefinition Mark = new("mark( color)?=#(?<ColorValue>[a-fA-F0-9]{8}|[a-fA-F0-9]{6})", "/mark", new TagID(nameof(Mark)), [(Context) => {
            Context.CreatedInline.Background = ToSolidColorBrush($"{Context.StartExpressionMatch.Groups["ColorValue"]}", AlphaAtTheEnd: Context.RichTextGenerationContext.TargetTextBlock is TMProEmitter);
        }]);
        public static readonly TagDefinition KeywordDescTooltip = new(@"link=""(?<KeywordID>\w+)""", "/link", new TagID(nameof(KeywordDescTooltip)), [(Context) => {
            if (LoadedConfiguration is not null && LoadedConfiguration.PreviewSettings.Base.EnableKeywordTooltips & Context.RichTextGenerationContext.TargetTextBlock is TMProEmitter { DisableKeyworLinksCreation: false }) {
                string KeywordID = $"{Context.StartExpressionMatch.Groups["KeywordID"]}";
                Context.CreatedInline.Cursor = Cursors.Help;
                Context.CreatedInline.ToolTip = BattleKeywordContainer.CreateTooltip(KeywordsLoader.LoadedKeywords_BattleKeywords.TryGetValue(KeywordID, out PlainKeyword? FoundKeywordDesc) ? FoundKeywordDesc : new PlainKeyword() { Name = "Unknown", MainDescription = "Unknown" });
                ToolTipService.SetInitialShowDelay(Context.CreatedInline, 250);
            }
        }]);
        public static readonly TagDefinition ChangesHighlight = new(@"style=""(highlight|upgradeHighlight)""", "/style", new TagID(nameof(ChangesHighlight)), [(Context) => Context.CreatedInline.Foreground = ToSolidColorBrush("#f8c200")])
        {
            AssignationOrderExpression = (Context) => TagsPreset.Color.AssignationOrderExpression(Context) + ImportableLimbusTags.KeywordDescTooltip.AssignationOrderExpression(Context) + 1,
            AssignabilityToSingleCandidateExpression = (Context) => Context.TargetTextSegment.AssignedTags.ContainsKey(ImportableLimbusTags.KeywordDescTooltip.ID) == false,
        };
        public static readonly TagDefinition Sprite = new(@"sprite name=""(?<SpriteID>\w+)""", null, new TagID(nameof(Sprite)))
        {
            CanBeAssignedToTextSegments = false,
            StartExpressionToInlineTransformations = [delegate (TagDefinition.TagToInlineTransformationContext Context, ref Func<DifferentiatedTagMatch, bool> AssignedTagsProjectionPredicate)
            {
                SplittedTextSegmentsSequence FollowingTextSegments = Context.RichTextGenerationContext.SplittedTextSegmentsSequence.AfterThe(Context.CurrentTextSegmentWithTag);

                #region Borrow next logical word and punctuation from following text segments to create solid word wrap with them
                Run BorrowedNextWord = new();
                Run BorrowedPunctuationAfterNextWord = new();
                bool TryBorrowFromFollowing(ref Run AffectedRun, SplittedTextSegment? TextSegment, [StringSyntax(StringSyntaxAttribute.Regex)] string Pattern)
                {
                    if (TextSegment is null) return false;

                    Match FoundMatch = Regex.Match(TextSegment.SegmentString, Pattern);
                    if (FoundMatch.Success)
                    {
                        AffectedRun = RichTextGenerationInstrumentary.CreaetRunWithProjectedTags(TextSegment, Context.RichTextGenerationContext);
                        AffectedRun.Text = FoundMatch.Value;

                        TextSegment.SegmentString = TextSegment.SegmentString.Remove(FoundMatch.Index, FoundMatch.Length);
                    }

                    return FoundMatch.Success;
                }


                static bool SearchEmergencyBreak(SplittedTextSegment TextSegment) => TextSegment.IsTag && TextSegment.MatchedTagExpression!.Tag.EqualsToOneOf(ImportableLimbusTags.Sprite, @Languages.InlineImage);

                SplittedTextSegment? NextTextSegmentWithText = Context.RichTextGenerationContext.SplittedTextSegmentsSequence.NearestTextForward(Context.CurrentTextSegmentWithTag, EmergencyBreak: SearchEmergencyBreak);
                if (TryBorrowFromFollowing(ref BorrowedNextWord, NextTextSegmentWithText, @"^\w+(\p{P})?"))
                {
                    if (BorrowedNextWord.Text.Matches(@"\p{P}") == false && NextTextSegmentWithText!.SegmentString.Length == 0)
                    {
                        SplittedTextSegment? NextTextSegmentWithPunctuation = Context.RichTextGenerationContext.SplittedTextSegmentsSequence.NearestTextForward(NextTextSegmentWithText, EmergencyBreak: SearchEmergencyBreak);
                        TryBorrowFromFollowing(ref BorrowedPunctuationAfterNextWord, NextTextSegmentWithPunctuation, @"^\p{P}");
                    }
                }
                #endregion


                #region Icon
                Image KeywordImage = new()
                {
                    Source = ImageDictionaries.KeywordImages[Context.TagExpressionMatch.Groups["SpriteID"].Value],
                    RenderTransform = new TranslateTransform()
                    {
                        X = SelectedLimbusCustomLanguage?.KeywordsSpriteHorizontalOffset ?? 0,
                        Y = SelectedLimbusCustomLanguage?.KeywordsSpriteVerticalOffset ?? 0,
                    }
                };
                KeywordImage.SetBinding(Image.WidthProperty, new Binding(nameof(Run.FontSize)) { Source = BorrowedNextWord });
                #endregion


                #region Sticked text
                List<Inline> SpriteTextLines = [BorrowedNextWord];
                if (BorrowedPunctuationAfterNextWord.Text != "") SpriteTextLines.Add(BorrowedPunctuationAfterNextWord);
                TextBlock SpriteTextBlock = Context.RichTextGenerationContext.TargetTextBlock.CreateBindedCopy(SpriteTextLines, [TextBlock.BackgroundProperty]);
                /// Not <see cref="RichTextGenerationInstrumentary.Specific.CreateContextualContainer"/> bc huge performance downfall idk
                #endregion


                #region Image container
                string SpriteBackgroundColor = Context.CurrentTextSegmentWithTag.AssignedTags.TryGetValue(nameof(TagsPreset.Background), out DifferentiatedTagMatch? BackgroundTag)
                    ? BackgroundTag.ExpressionMatch.Groups["ColorValue"].Value
                    : Context.CurrentTextSegmentWithTag.AssignedTags.TryGetValue(ImportableLimbusTags.Mark.ID, out DifferentiatedTagMatch? MarkTag)
                        ? MarkTag.ExpressionMatch.Groups["ColorValue"].Value
                        : "#00000000";

                Grid ImageContainer = new Grid() { Children = { KeywordImage }, Background = ToSolidColorBrush(SpriteBackgroundColor, AlphaAtTheEnd: Context.RichTextGenerationContext.TargetTextBlock is TMProEmitter) };
                #endregion


                AssignedTagsProjectionPredicate = (TagMatch) => false; // Prevent underline or background tags from being applid on created InlineUIContainer
                return new RichTextGenerationInstrumentary.Specific.PreConstructedInlineUIContainer([
                    ImageContainer,
                    SpriteTextBlock,
                ]);
            }]
        };
    }
}