using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace LCLocalizationInterface
{
    /// <summary>
    /// Rich text generator for <see cref="TextBlock.Inlines"/> using <see cref="Run"/> and <see cref="InlineUIContainer"/> elements
    /// </summary>
    public static class TextMeshLarp
    {
        /// <summary>
        /// Configurable open and close regex pattern parts to wrap start/end expressions of <see cref="TagDefinition"/> : <c>Regex.Match(..., Open + Start/End Expression + Close)</c>
        /// </summary>
        public record TagDividers([StringSyntax(StringSyntaxAttribute.Regex)] string Open, [StringSyntax(StringSyntaxAttribute.Regex)] string Close);
        public readonly struct RichTextGenerationContext
        {
            public required string OrigianlRichText { get; init; }
            public required TextBlock TargetTextBlock { get; init; }
            public required RichTextTagsRegistry TagsRegistry { get; init; }

            public required IReadOnlyList<TagDividers> TagDividers { get; init; }
            public required IReadOnlyDictionary<TagID, TagDefinition> AllowedTags { get; init; }

            public required SplittedTextSegmentsSequence SplittedTextSegmentsSequence { get; init; }
        }



        /// <summary>
        /// Has 2 <see langword="implicit operators"/> for conversion to/from <see cref="string"/>, so it is technically just a virtual <see cref="string"/> value to explicitly specify that value represents Tag ID and not just some <see cref="string"/>
        /// </summary>
        public readonly record struct TagID(string Value)
        {
            public TagID() : this("") => throw new ArgumentException("TagID must have an Identifier, use `TagID(string Value)` constructor", nameof(Value));
            
            private readonly string Value = Value;

            public override string ToString() => Value;

            public static implicit operator TagID  (string Identifier) => new TagID(Identifier);
            public static implicit operator string (TagID  Identifier) => Identifier.Value;
        }

        /// <summary>
        /// Defines behavior of the tag
        /// </summary>
        /// <param name="StartExpression">Regex pattern for opening tag content (Without separators), <see cref="Match"/> value provided in <see cref="TagProjections"/></param>
        /// <param name="EndExpression">Regex pattern for ending tag content (Without separators)</param>
        /// <param name="ID">Unique identifier for this tag</param>
        /// <param name="TagProjectionsList">Actions on a natively created <see cref="Run"/> from text, or another <see cref="Inline"/> from <see cref="StartExpressionToInlineTransformations"/>/<see cref="EndExpressionToInlineTransformations"/></param>
        public record TagDefinition([StringSyntax(StringSyntaxAttribute.Regex)] string StartExpression, [StringSyntax(StringSyntaxAttribute.Regex)] string? EndExpression, TagID ID, params TagDefinition.TagProjectionDelegate[] TagProjectionsList)
        {
            /// <summary>Unique identifier for this tag</summary>
            public TagID ID { get; set; } = ID;

            /// <summary>Regex pattern for opening tag content (Without separators), <see cref="Match"/> value provided in <see cref="TagProjections"/></summary>
            [StringSyntax(StringSyntaxAttribute.Regex)]
            public string StartExpression { get; set; } = StartExpression;

            /// <summary>Regex pattern for ending tag content (Without separators)</summary>
            [StringSyntax(StringSyntaxAttribute.Regex)]
            public string? EndExpression { get; set; } = EndExpression;

            /// <returns>ID of this tag</returns>
            public override string ToString() => this.ID;

            /// <summary>User-defineable properties if tag requires additional behavior</summary>
            public Dictionary<string, object?> AdditionalProperties { get; set; } = [];




            #region Delegates

            #region Assignability
            /// <summary>
            /// Determines whether the matched tag data (<see cref="DifferentiatedTagMatch"/> from <see cref="SplittedTextSegment"/> with this tag) can be assigned to subsequent text segments (Added to <see cref="SplittedTextSegment.AssignedTags"/>) before reaching <see cref="SplittedTextSegment"/> with <see cref="EndExpression"/> of this tag (Occurs in the <see cref="RichTextGenerationInstrumentary.PerformTagsAssignation"/>)
            /// </summary>
            public bool CanBeAssignedToTextSegments { get; set; } = true; // Global for following two delegates

            /// <summary>
            /// Defines an expression of condition which determines the possibility of further assignation of this tag to following set of <see cref="SplittedTextSegment"/>s (<c>TagAssignationCandidates</c> param); this set of text segments is created by <see cref="RichTextGenerationInstrumentary.DetermineTagAssignationCandidates"/>
            /// <br/><br/>
            /// Candidates for tag assignation can also be changed, because they are passed as <see langword="ref"/> <see cref="List{T}"/> param
            /// </summary>
            public AssignabilityToCandidatesListExpressionDelegate AssignabilityToCandidatesListExpression { get; set; } = (_, ref _, _) => true; // <- True by default
            public delegate bool AssignabilityToCandidatesListExpressionDelegate(SplittedTextSegment CurrentTextSegmentWithTag, ref List<SplittedTextSegment> TagAssignationCandidates, RichTextGenerationContext RichTextGenerationContext);


            public readonly record struct AssignationContext(SplittedTextSegment CurrentTextSegmentWithTag, SplittedTextSegment TargetTextSegment, IReadOnlyList<SplittedTextSegment> AllTagAssignationCandidates, RichTextGenerationContext RichTextGenerationContext);

            /// <summary>
            /// Defines an expression of condition which determines the possibility of matched tag data assigning to single text segment (Adding <see cref="DifferentiatedTagMatch"/> to <see cref="SplittedTextSegment.AssignedTags"/>), goes after <see cref="AssignabilityToCandidatesListExpression"/>
            /// </summary>
            public AssignabilityToSingleCandidateExpressionDelegate AssignabilityToSingleCandidateExpression { get; set; } = (_) => true; // <- True by default
            public delegate bool AssignabilityToSingleCandidateExpressionDelegate(AssignationContext Context);
            #endregion



            #region Order expression
            /// <summary>
            /// Expression that determines the order in which tags will be processed through <see cref="RichTextGenerationInstrumentary.PerformTagsAssignation"/>, returns 0 by default (Example: it can be made relative to another tag by asking its current <see cref="AssignationOrderExpression"/> and adding or subtracting 1 from it)
            /// <br/><br/>
            /// Example of relativity from above: situation where tag <c>A</c> should not be assigned for <see cref="SplittedTextSegment"/> that already has tag <c>B</c> in its <see cref="SplittedTextSegment.AssignedTags"/>. Therefore, the tag <c>A</c> must be placed in the order after that tag <c>B</c> to be sure that tag <c>B</c> has fully passed its assignment stage.
            /// <br/><br/>
            /// But beware of <see cref="StackOverflowException"/> when looping <see cref="AssignationOrderExpression"/> polls (If tag <c>A</c> asks order from tag <c>B</c>, and then tag <c>B</c> asks order from tag <c>A</c>)
            /// </summary>
            public AssignationOrderExpressionDelegate AssignationOrderExpression { get; set; } = (_) => 0; // Zero by default
            public delegate double AssignationOrderExpressionDelegate(RichTextGenerationContext RichTextGenerationContext);
            #endregion



            #region Text pre-processing
            /// <summary>
            /// List of actions to perform on input text and some other variables if this tag will be processed in <see cref="SetRichText"/> (Means located in given <see cref="RichTextTagsRegistry"/> and its ID is not added to IgnoredTagIDs param of SetRichText)
            /// </summary>
            public List<TextPreProcessingActionDelegate>? TextPreProcessingActions { get; set; }
            public delegate void TextPreProcessingActionDelegate(ref string? InputRichText, TextBlock TargetTextBlock, RichTextTagsRegistry TagsRegistry, ref List<TagDividers> TagDividers, ref Dictionary<TagID, TagDefinition> AllowedTags);

            /// <summary>
            /// Determines the order in which <see cref="TextPreProcessingActions"/> will be called from tags, see the <see cref="AssignationOrderExpression"/> annotation for a possible usage examples and general principle of order expressions
            /// <br/><br/>
            /// Returns 0 by default
            /// </summary>
            public TextPreProcessingActionsOrderExpressionDelegate TextPreProcessingActionsOrderExpression { get; set; } = (_, _, _, _, _) => 0;
            public delegate double TextPreProcessingActionsOrderExpressionDelegate(string? InputRichText, TextBlock TargetTextBlock, RichTextTagsRegistry TagsRegistry, IReadOnlyList<TagDividers> TagDividers, IReadOnlyDictionary<TagID, TagDefinition> AllowedTags);
            #endregion



            #region Tag to inline transformations
            public readonly record struct TagToInlineTransformationContext(Match TagExpressionMatch, SplittedTextSegment CurrentTextSegmentWithTag, RichTextGenerationContext RichTextGenerationContext);


            /// <summary>
            /// Converting <see cref="SplittedTextSegment"/> with matched <see cref="StartExpression"/> to a set of <see cref="Inline"/>s
            /// </summary>
            public List<TagExpressionToInlineTransformationDelegate>? StartExpressionToInlineTransformations { get; set; }

            /// <summary>
            /// Converting <see cref="SplittedTextSegment"/> with matched <see cref="EndExpression"/> to a set of <see cref="Inline"/>s
            /// </summary>
            public List<TagExpressionToInlineTransformationDelegate>? EndExpressionToInlineTransformations   { get; set; }

            /// <param name="AssignedTagsProjectionPredicate">Condition for <see cref="RichTextGenerationInstrumentary.ProjectAssignedTagsToInline"/> to filter out tags that will be projected onto the created <see cref="Inline"/></param>
            public delegate Inline TagExpressionToInlineTransformationDelegate(TagToInlineTransformationContext Context, ref Func<DifferentiatedTagMatch, bool> AssignedTagsProjectionPredicate);
            #endregion



            #region Tag value projection
            public readonly record struct TagProjectionContext(Inline CreatedInline, Match StartExpressionMatch, SplittedTextSegment TextSegmentWithTextOfInline, RichTextGenerationContext RichTextGenerationContext);

            /// <summary>
            /// Actions on a natively created <see cref="Run"/> from text, or another <see cref="Inline"/> from <see cref="StartExpressionToInlineTransformations"/>/<see cref="EndExpressionToInlineTransformations"/>
            /// <br/><br/>
            /// Acts as a primary representation of the tag (e.g. set font family or foreground + use <see cref="Match"/> values of <see cref="StartExpression"/> if the tag must have a specified value)
            /// </summary>
            public List<TagProjectionDelegate> TagProjections { get; set; } = [.. TagProjectionsList]; // <- From base constructor
            public delegate void TagProjectionDelegate(TagProjectionContext Context);
            #endregion

            #endregion
        }


        public static class TagsPreset
        {
            public static readonly TagDefinition Bold          = new("b", "/b", new TagID(nameof(Bold)),          [(Context) => Context.CreatedInline.FontWeight = FontWeights.Bold ]);
            public static readonly TagDefinition Italic        = new("i", "/i", new TagID(nameof(Italic)),        [(Context) => Context.CreatedInline.FontStyle  = FontStyles.Italic]);
            public static readonly TagDefinition Underline     = new("u", "/u", new TagID(nameof(Underline)),     [(Context) => Context.CreatedInline.TextDecorations.Add(new TextDecoration() { Location = TextDecorationLocation.Underline     })]);
            public static readonly TagDefinition Strikethrough = new("s", "/s", new TagID(nameof(Strikethrough)), [(Context) => Context.CreatedInline.TextDecorations.Add(new TextDecoration() { Location = TextDecorationLocation.Strikethrough })]);

            public static readonly TagDefinition Color      = new("color=#(?<ColorValue>[a-fA-F0-9]{8}|[a-fA-F0-9]{6})", "/color", new TagID(nameof(Color)), [(Context) => {
                Context.CreatedInline.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"{Context.StartExpressionMatch.Groups["ColorValue"]}"));
            }]);
            public static readonly TagDefinition Background = new("background=#(?<ColorValue>[a-fA-F0-9]{8}|[a-fA-F0-9]{6})", "/background", new TagID(nameof(Background)), [(Context) => {
                Context.CreatedInline.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString($"{Context.StartExpressionMatch.Groups["ColorValue"]}"));
            }]);

            public static readonly TagDefinition FontFamily  = new(@"font=""(?<FontExpression>.*?)""", "/font", new TagID(nameof(FontFamily)), [(Context) => {
                Context.CreatedInline.FontFamily = FontFamilyFromFileOrName(Context.StartExpressionMatch.Groups["FontExpression"].Value);
            }]);
            public static readonly TagDefinition FontWeight  = new(@"font-weight=""(?<WeightValue>Thin|ExtraLight|Light|Regular|Medium|SemiBold|Bold|Heavy|Black)""", "/font-weight", new TagID(nameof(FontWeight)), [(Context) => {
                Context.CreatedInline.FontWeight = (FontWeight) typeof(FontWeights).GetProperty($"{Context.StartExpressionMatch.Groups["WeightValue"]}")!.GetValue(null)!;
            }]);
            public static readonly TagDefinition FontStretch = new(@"font-stretch=""(?<StretchValue>Condensed|Expanded|ExtraCondensed|ExtraExpanded|Medium|Normal|SemiCondensed|SemiExpanded|UltraCondensed|UltraExpanded)""", "/font-stretch", new TagID(nameof(FontStretch)), [(Context) => {
                Context.CreatedInline.FontStretch = (FontStretch) typeof(FontStretches).GetProperty($"{Context.StartExpressionMatch.Groups["StretchValue"]}")!.GetValue(null)!;
            }]);

            public static readonly TagDefinition Hyperlink = new(@"hyperlink=""(?<Url>.*?)""", "/hyperlink", new TagID(nameof(Hyperlink)), [(Context) => {
                string Url = Context.StartExpressionMatch.Groups["Url"].Value;
                Context.CreatedInline.MouseLeftButtonUp += (_, _) => Process.Start(new ProcessStartInfo(Url) { UseShellExecute = true });
                Context.CreatedInline.Cursor = Cursors.Help;
                Context.CreatedInline.ToolTip = Url;
                ToolTipService.SetInitialShowDelay(Context.CreatedInline, 100);
            }]);

            public static readonly TagDefinition SizePercentage = new(@"size=(?<SizePercentageValue>(\d+)((\.|\,)\d+)?)%", @"/size", new TagID(nameof(SizePercentage)), [(Context) => {
                double SizePercentageValue = double.Parse(Context.StartExpressionMatch.Groups["SizePercentageValue"].Value.Replace(".", ",")) / 100;
                Context.CreatedInline.FontSize = Context.RichTextGenerationContext.TargetTextBlock.FontSize * SizePercentageValue;
            }]);

            public static readonly TagDefinition VOffset = new(@"voffset=(?<VerticalOffsetValue>(\+|\-)?\d+((\.|\,)\d+)?)",   "/voffset", new TagID(nameof(VOffset)), [(Context) => {
                double OffsetValue = double.Parse(Context.StartExpressionMatch.Groups["VerticalOffsetValue"].Value.Replace(".", ","));
                Context.CreatedInline.TextEffects.Add(new TextEffect() {
                    PositionStart = 0, PositionCount = int.MaxValue,
                    Transform = new TranslateTransform(0, OffsetValue)
                });
            }]);
            public static readonly TagDefinition HOffset = new(@"hoffset=(?<HorizontalOffsetValue>(\+|\-)?\d+((\.|\,)\d+)?)", "/hoffset", new TagID(nameof(HOffset)), [(Context) => {
                double OffsetValue = double.Parse(Context.StartExpressionMatch.Groups["HorizontalOffsetValue"].Value.Replace(".", ","));
                Context.CreatedInline.TextEffects.Add(new TextEffect() {
                    PositionStart = 0, PositionCount = int.MaxValue,
                    Transform = new TranslateTransform(OffsetValue, 0)
                });
            }]);

            public static readonly TagDefinition NoParse = new("noparse", "/noparse", new TagID(nameof(NoParse)))
            {
                CanBeAssignedToTextSegments = false,
                AdditionalProperties = { ["Char to insert after open dividers"] = '\u200B' },
                TextPreProcessingActions = [delegate (ref string? InputRichText, TextBlock TargetTextBlock, RichTextTagsRegistry TagsRegistry, ref List<TagDividers> TagDividers, ref Dictionary<TagID, TagDefinition> AllowedTags)
                {
                    if (AllowedTags.ContainsValue(TagsPreset.NoParse!))
                    {
                        if (InputRichText is not null && InputRichText.Contains("noparse"))
                        {
                            foreach (TagDividers TagDividersMode in TagDividers)
                            {
                                InputRichText = Regex.Replace(InputRichText, @$"{TagDividersMode.Open}noparse{TagDividersMode.Close}(?<NoParseRegionAsdAsdInternal>.*?){TagDividersMode.Open}/noparse{TagDividersMode.Close}", Match =>
                                {
                                    return Match.Groups["NoParseRegionAsdAsdInternal"].Value.Replace(TagDividersMode.Open, $"{TagDividersMode.Open}{TagsPreset.NoParse!.AdditionalProperties["Char to insert after open dividers"]}"); // Add zwsp char to opening tags inside noparse region to prevent their matching
                                },
                                RegexOptions.Singleline);
                            }
                        }
                    }
                }]
            };

            /// <summary>
            /// This tag wraps following text in <see cref="InlineUIContainer"/> with another <see cref="TextBlock"/> inside, which is created via <see cref="RichTextGenerationInstrumentary.Specific.CreateContextualContainer"/>
            /// </summary>
            public static readonly TagDefinition NoBreak = new("nobr", "/nobr", new TagID(nameof(NoBreak)))
            {
                CanBeAssignedToTextSegments = false,
                StartExpressionToInlineTransformations = [delegate (TagDefinition.TagToInlineTransformationContext Context, ref Func<DifferentiatedTagMatch, bool> AssignedTagsProjectionPredicate)
                {
                    List<SplittedTextSegment> NoBreakRegion = [];

                    int InnerNoBreaksSkipStack = 0;
                    foreach (SplittedTextSegment NoBreakTextSegment in Context.RichTextGenerationContext.SplittedTextSegmentsSequence.AfterThe(Context.CurrentTextSegmentWithTag))
                    {
                        if (NoBreakTextSegment.IsTag && NoBreakTextSegment.MatchedTagExpression!.Tag.ID == nameof(NoBreak))
                        {
                            if (NoBreakTextSegment.MatchedTagExpression!.IsEndExpression)
                            {
                                if (InnerNoBreaksSkipStack == 0)
                                {
                                    break; // Encountered current tag end expression
                                }
                                else
                                {
                                    InnerNoBreaksSkipStack--; // Encountered end expression of embedded tag
                                }
                            }
                            else
                            {
                                InnerNoBreaksSkipStack++; // If embedded tag opened, increase its skipping stack
                            }
                        }


                        NoBreakRegion.Add(NoBreakTextSegment with { }); // Create copy of text segment
                        NoBreakTextSegment.Ignored = true;              // And make original ignored to prevent text duplicating
                                                                        // i.e. deprecate from original SplittedTextSegmentsSequence and move to NoBreak region
                    }

                    TextBlock SolidLinePart = RichTextGenerationInstrumentary.Specific.CreateContextualContainer(Context.CurrentTextSegmentWithTag, Context.RichTextGenerationContext);

                    RichTextGenerationInstrumentary.ProcessSplittedTextSegmentsSequenceWithAssignedTags(Context.RichTextGenerationContext with { TargetTextBlock = SolidLinePart, SplittedTextSegmentsSequence = new SplittedTextSegmentsSequence(NoBreakRegion) });

                    return new InlineUIContainer()
                    {
                        BaselineAlignment = BaselineAlignment.Bottom,
                        Child = SolidLinePart,
                    };
                }]
            };



            /// <summary><c>'&lt;'</c> and <c>'&gt;'</c></summary>
            public static TagDividers DefaultDividers = new("<", ">");

            /// <summary><see cref="RichTextTagsRegistry"/> with all <see langword="static readonly"/> <see cref="TagDefinition"/>s from <see cref="TagsPreset"/></summary>
            public static RichTextTagsRegistry DefaultRegistry = new()
            {
                Bold,
                Italic,
                Underline,
                Strikethrough,

                Color,
                Background,

                FontFamily,
                FontWeight,
                FontStretch,

                Hyperlink,

                SizePercentage,

                VOffset,
                HOffset,

                NoParse,
                NoBreak,
            };
        }


        /// <summary>
        /// Contains list of tags to import + delegates for additional actions and conditions
        /// <br/><br/>
        /// Usages:<br/>
        /// • <see cref="RichTextTagsRegistry.ImportTagsFrom"/> by created instance<br/>
        /// • <see cref="RichTextTagsRegistry.ImportTagsFromNewInstanceOf{TImportable}"/> by specifying class derived from <see cref="ImportableRichTextTags"/>, which contains <see langword="override"/> properties with default values (All of them is <see langword="virtual"/>)
        /// </summary>
        public class ImportableRichTextTags
        {
            public virtual List<TagDefinition> TagDefinitions { get; set; } = [];

            public delegate void TagsExportingActionDelegate(RichTextTagsRegistry Destination);
            public delegate bool SingleTagExporingPredicate(TagDefinition TagToExport, RichTextTagsRegistry Destination);
            public virtual List<TagsExportingActionDelegate> PrecedingTagsExportingActions { get; set; } = [];
            public virtual List<TagsExportingActionDelegate> SubsequentTagsExportingActions { get; set; } = [];
            public virtual SingleTagExporingPredicate SingleTagExportingPredicate { get; set; } = (TagToExport, Destination) => true;
        }

        /// <summary>
        /// List for <see cref="TagDefinition"/>s:<br/>
        /// • Indexer by <see cref="TagID"/><br/>
        /// • <see cref="ContainsTag"/> / <see cref="TryGetTag"/> / <see cref="RemoveTag"/><br/>
        /// • <see cref="ImportTagsFrom"/> / <see cref="ImportTagsFromNewInstanceOf"/><br/>
        /// • <see langword="explicit operator"/> for conversion to <see cref="ImportableRichTextTags"/>
        /// </summary>
        public class RichTextTagsRegistry : List<TagDefinition>
        {
            public TagDefinition this[TagID RichTextTagID] => this.FirstOrDefault(Tag => Tag.ID == RichTextTagID)!;
            public bool ContainsTag(TagID RichTextTagID) => this[RichTextTagID] is not null;
            public bool TryGetTag(TagID RichTextTagID, out TagDefinition FoundTag) => (FoundTag = this[RichTextTagID]) is not null;
            public bool RemoveTag(TagID RichTextTagID) => this.TryGetTag(RichTextTagID, out TagDefinition FoundTag) && this.Remove(FoundTag);


            public void ImportTagsFrom(ImportableRichTextTags Importable)
            {
                Importable.PrecedingTagsExportingActions?.ForEach(PrecedingTagsExportingAction => PrecedingTagsExportingAction?.Invoke(this));
                
                foreach (TagDefinition Tag in Importable.TagDefinitions)
                {
                    if (Importable.SingleTagExportingPredicate(Tag, this))
                    {
                        this.Add(Tag);
                    }
                }

                Importable.SubsequentTagsExportingActions?.ForEach(SubsequentTagsExportingAction => SubsequentTagsExportingAction?.Invoke(this));
            }
            public void ImportTagsFromNewInstanceOf<TImportable>(params object?[] ConstructorArgs) where TImportable : ImportableRichTextTags, new()
            {
                TImportable NewInstanceOfImportableTags = ConstructorArgs.Length > 0
                    ? (TImportable)Activator.CreateInstance(typeof(TImportable), ConstructorArgs)!
                    : new TImportable();

                this.ImportTagsFrom(NewInstanceOfImportableTags);
            }

            public static explicit operator ImportableRichTextTags(RichTextTagsRegistry Registry) => new() { TagDefinitions = [.. Registry] };
        }



        /// <summary>
        /// Represents a sequence of text segments, initially obtained by splitting differentiated text into <see cref="SplittedTextSegment"/>s, where each <see cref="SplittedTextSegment"/> represents either a part of regular text or a tag (<see cref="RichTextGenerationInstrumentary.CreateSplittedTextSegmentsSequence"/>)
        /// <br/><br/>
        /// Has relative accessors for <see cref="SplittedTextSegment"/>s (Methods not from <c>System.Linq</c> for <see cref="IEnumerable{T}"/> <see langword="interface"/>) and indexers for them by index or range
        /// </summary>
        public class SplittedTextSegmentsSequence(IEnumerable<SplittedTextSegment> Source) : IReadOnlyList<SplittedTextSegment>
        {
            private List<SplittedTextSegment> SourceList { get; } = [.. Source];

            public SplittedTextSegment this[int SegmentIndex] => SourceList[SegmentIndex];
            public SplittedTextSegmentsSequence this[Range SegmentsRange] => new(Source: SourceList[SegmentsRange]);


            #region Relative accessors
            public SplittedTextSegment? NextFrom(SplittedTextSegment TextSegment) => SourceList.ElementAtOrDefault(IndexOf(TextSegment) + 1);
            public SplittedTextSegment? PreviousFrom(SplittedTextSegment TextSegment) => SourceList.ElementAtOrDefault(IndexOf(TextSegment) - 1);
            
            public SplittedTextSegmentsSequence AfterThe(SplittedTextSegment TextSegment) => this[(IndexOf(TextSegment) + 1)..];
            public SplittedTextSegmentsSequence BeforeThe(SplittedTextSegment TextSegment) => this[..IndexOf(TextSegment)];

            public SplittedTextSegment? FindNearest(
                SplittedTextSegment PointerTextSegment,
                Func<SplittedTextSegment, bool> Predicate,
                Func<SplittedTextSegment, bool>? EmergencyBreakPredicate = null,
                bool SearchForward = true
            ) {
                IEnumerable<SplittedTextSegment> SegmentsSearchRegion = SearchForward
                    ? this.AfterThe(PointerTextSegment)
                    : this.BeforeThe(PointerTextSegment).Reverse();

                foreach (SplittedTextSegment Segment in SegmentsSearchRegion)
                {
                    if (EmergencyBreakPredicate is not null && EmergencyBreakPredicate(Segment)) break;
                    if (Predicate(Segment)) return Segment;
                }

                return null;
            }

            public SplittedTextSegment? NearestTextForward(SplittedTextSegment PointerTextSegment, Func<SplittedTextSegment, bool>? EmergencyBreak = null)
                => FindNearest(PointerTextSegment, Segment => Segment.IsTag == false, EmergencyBreak, true);

            public SplittedTextSegment? NearestTextBackward(SplittedTextSegment PointerTextSegment, Func<SplittedTextSegment, bool>? EmergencyBreak = null)
                => FindNearest(PointerTextSegment, Segment => Segment.IsTag == false, EmergencyBreak, false);

            public SplittedTextSegment? NearestTagForward(SplittedTextSegment PointerTextSegment, Func<SplittedTextSegment, bool>? EmergencyBreak = null)
                => FindNearest(PointerTextSegment, Segment => Segment.IsTag == true, EmergencyBreak, true);

            public SplittedTextSegment? NearestTagBackward(SplittedTextSegment PointerTextSegment, Func<SplittedTextSegment, bool>? EmergencyBreak = null)
                => FindNearest(PointerTextSegment, Segment => Segment.IsTag == true, EmergencyBreak, false);
            #endregion



            public int Count => SourceList.Count;
            public int IndexOf(SplittedTextSegment TextSegment) => SourceList.IndexOf(TextSegment);

            public IEnumerator<SplittedTextSegment> GetEnumerator() => SourceList.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }


        /// <summary>
        /// Represents fragment of text splitted by differentiated tags that can be a regular part of the text or a tag(<see cref="SplittedTextSegment.IsTag"/>)
        /// </summary>
        public record SplittedTextSegment
        {
            /// <summary>
            /// Indicates that this <see cref="SplittedTextSegment"/> will be ignored at <see cref="RichTextGenerationInstrumentary.PerformTagsAssignation"/> or <see cref="RichTextGenerationInstrumentary.ProcessSplittedTextSegmentsSequenceWithAssignedTags"/>
            /// </summary>
            public bool Ignored { get; set; } = false;


            public string SegmentString { get; set; }

            /// <summary>If <see cref="MatchedTagExpression"/> is not <see langword="null"/></summary>
            public bool IsTag => MatchedTagExpression is not null;

            /// <summary>Initially created from <see cref="SplittedTextSegment(string, RichTextTagsRegistry)"/> constructor if given SegmentString is differentiated tag expression</summary>
            public DifferentiatedTagMatch? MatchedTagExpression { get; set; }

            public Dictionary<TagID, DifferentiatedTagMatch> AssignedTags { get; set; } = [];

            /// <summary>
            /// <paramref name="UsedTagsRegistry"/> needed for creating <see cref="MatchedTagExpression"/> if <paramref name="SegmentString"/> is differentiated tag expression (Contins <see langword="TagIDAndExpressionMatchDivider"/> from <see cref="DifferentiatedTagMatch.ExpressionMarkup"/>)
            /// </summary>
            public SplittedTextSegment(string SegmentString, RichTextTagsRegistry UsedTagsRegistry)
            {
                this.SegmentString = SegmentString;
                if (SegmentString.Contains(DifferentiatedTagMatch.ExpressionMarkup.TagIDAndExpressionMatchDivider))
                {
                    MatchedTagExpression = new DifferentiatedTagMatch(SegmentString, UsedTagsRegistry);
                }
            }

            public override string ToString() => $"({(IsTag ? "Tag" : "Text")}) {MatchedTagExpression?.ToString() ?? SegmentString}";
        }

        /// <summary>
        /// Represents tag match that embedded as <see cref="SplittedTextSegment.MatchedTagExpression"/> from <see cref="SplittedTextSegment(string, RichTextTagsRegistry)"/> constructor if <see cref="SplittedTextSegment.SegmentString"/> is differentiated tag expression
        /// </summary>
        public record DifferentiatedTagMatch
        {
            public TagDefinition Tag { get; }
            public bool IsEndExpression { get; }
            public Match ExpressionMatch { get; }

            public Dictionary<string, object?> AdditionalProperties { get; set; } = [];

            public DifferentiatedTagMatch(string MatchedDifferentiatedTagString, RichTextTagsRegistry ParentTagsRegistry)
            {
                (this.Tag, this.ExpressionMatch, this.IsEndExpression) = DeconstructDifferentiatedTagExpression(MatchedDifferentiatedTagString, ParentTagsRegistry);
            }

            public override string ToString() => $"{this.Tag.ID} :: `{this.ExpressionMatch}`";


            #region Static
            public static (char Open, char TagIDAndExpressionMatchDivider, string EndExpressionMatchPrefix, char Close) ExpressionMarkup = ('\uFFFE', /*ID*/ '\uF8FE', ("EndExpression:\uF8FF:"), /*Match value*/ '\uFFFF');

            public static string ConstructDifferentiatedTagExpression(TagID ID, string MatchValue, bool IsEndExpression = false)
            {
                return // {\uFFFE}TagID{\uF8FE}{EndExpressionPrefix?}expression regex match{\uFFFF}

                    ((ExpressionMarkup.Open)) +

                    ID +

                    ((ExpressionMarkup.TagIDAndExpressionMatchDivider)) +

                    (IsEndExpression ? ExpressionMarkup.EndExpressionMatchPrefix : "") + MatchValue +

                    ((ExpressionMarkup.Close));
            }

            public static (TagDefinition Tag, Match TagExpressionMatch, bool IsEndExpression) DeconstructDifferentiatedTagExpression(string DifferentiatedTagString, RichTextTagsRegistry ParentTagsRegistry)
            {
                string[] TagExpressionSplit = DifferentiatedTagString.Split(ExpressionMarkup.TagIDAndExpressionMatchDivider);

                TagID ID = TagExpressionSplit[0];
                string TagExpressionString = TagExpressionSplit[1];

                bool IsEndExpression = TagExpressionString.StartsWith(ExpressionMarkup.EndExpressionMatchPrefix);
                TagDefinition Tag = ParentTagsRegistry[ID];

                if (Tag is not null)
                {
                    Match TagExpressionMatch;

                    TagExpressionMatch = IsEndExpression == false
                        ? Regex.Match(TagExpressionString, $"^{Tag.StartExpression}$")
                        : Regex.Match(TagExpressionString[ExpressionMarkup.EndExpressionMatchPrefix.Length..], $"^{Tag.EndExpression}$");
                    
                    return (Tag, TagExpressionMatch, IsEndExpression);
                }
                else
                {
                    throw new NullReferenceException($"The tag \"{ID}\" from this expression was not found in the {nameof(ParentTagsRegistry)} by ID");
                }
            }
            #endregion
        }



        public static class RichTextGenerationInstrumentary
        {
            /// <summary>
            /// Replaces tag matches in text with special expressions containing unicode characters — <u>Differentiated Tags</u> (Without curly braces, they are used for emphasis):<br/>
            /// • <see langword="Start expression"/> : <c>{\uFFFE}Tag ID{\uF8FE}Start expression match value{\uFFFF}</c><br/>
            /// • <see langword="End expression"/> : <c>{\uFFFE}Tag ID{\uF8FE}{EndExpression:\uF8FF:}End expression match value{\uFFFF}</c>
            /// <br/><br/>
            /// It is necessary to separate the text into segments of tags and pieces of real text using the unique characters <c>\uFFFE</c> + <c>\uFFFF</c>, so as not to use simply tag separators, which can still occur as ordinary characters in the text
            /// <br/><br/>
            /// Unicode characters end expression match value prefix can be configured in <see cref="DifferentiatedTagMatch.ExpressionMarkup"/>
            /// </summary>
            public static string PerformTagsDifferentiation(string RichText, IReadOnlyList<TagDividers> ExpectedTagDividers, IReadOnlyDictionary<TagID, TagDefinition> AllowedTags)
            {
                ArgumentNullException.ThrowIfNull(RichText);
                ArgumentNullException.ThrowIfNull(ExpectedTagDividers);
                ArgumentNullException.ThrowIfNull(AllowedTags);

                foreach (TagDefinition Tag in AllowedTags.Values)
                {
                    if (Tag is not null)
                    {
                        foreach (TagDividers TagDividersMode in ExpectedTagDividers)
                        {
                            string OpenDivider = TagDividersMode.Open;
                            string CloseDivider = TagDividersMode.Close;

                            #region Replace actual tags in text with specific expressions to split text further
                            if (!string.IsNullOrEmpty(Tag.StartExpression))
                            {
                                RichText = Regex.Replace(RichText, OpenDivider + $"(?<TagStartExpressionValueAsdAsdInternal>{Tag.StartExpression})" + CloseDivider, Match =>
                                {
                                    string TagStartExpressionValue = Match.Groups["TagStartExpressionValueAsdAsdInternal"].Value;
                                    return DifferentiatedTagMatch.ConstructDifferentiatedTagExpression(ID: Tag.ID, MatchValue: TagStartExpressionValue, IsEndExpression: false);
                                });
                            }
                            if (!string.IsNullOrEmpty(Tag.EndExpression))
                            {
                                RichText = Regex.Replace(RichText, (OpenDivider + $"(?<TagEndExpressionValueAsdAsdInternal>{Tag.EndExpression})" + CloseDivider), Match =>
                                {
                                    string TagEndExpressionValue = Match.Groups["TagEndExpressionValueAsdAsdInternal"].Value;
                                    return TagEndExpressionValue == "" ? (OpenDivider + CloseDivider) : DifferentiatedTagMatch.ConstructDifferentiatedTagExpression(ID: Tag.ID, MatchValue: TagEndExpressionValue, IsEndExpression: true); ;
                                });
                            }
                            #endregion
                        }
                    }
                }

                return RichText;
            }

            /// <summary>
            /// Splits text to string segments by <see cref="DifferentiatedTagMatch.ExpressionMarkup"/>'s <see langword="Open"/> and <see langword="Close"/> chars
            /// </summary>
            /// <param name="RichTextWithDifferentiatedTags">Text obtained from <see cref="PerformTagsDifferentiation"/></param>
            public static List<string> SplitRichTextByDifferentiatedTags(string RichTextWithDifferentiatedTags)
            {
                return [.. RichTextWithDifferentiatedTags.Split([DifferentiatedTagMatch.ExpressionMarkup.Open, DifferentiatedTagMatch.ExpressionMarkup.Close], StringSplitOptions.RemoveEmptyEntries)];
            }

            /// <summary>
            /// Forms <see cref="SplittedTextSegmentsSequence"/> based on <see cref="SplittedTextSegment"/>s list created from <paramref name="RichTextSplittedByDifferentiatedTags"/> list( RichTextSplittedByDifferentiatedTags.Select(x => new SplittedTextSegment(x, TagsRegistry)) )
            /// </summary>
            public static SplittedTextSegmentsSequence CreateSplittedTextSegmentsSequence(List<string> RichTextSplittedByDifferentiatedTags, RichTextTagsRegistry TagsRegistry)
            {
                return new SplittedTextSegmentsSequence(Source: RichTextSplittedByDifferentiatedTags.Select(SplittedTextFragment => new SplittedTextSegment(SplittedTextFragment, TagsRegistry)));
            }

            /// <summary>
            /// |<br/>
            /// Creates <see cref="RichTextGenerationContext"/> with <see cref="SplittedTextSegmentsSequence"/> created from <see cref="SplitRichTextByDifferentiatedTags"/> List + all other specified information
            /// </summary>
            public static RichTextGenerationContext FormSplittedTextSegmentsSequenceAndContext(string OriginalRichText, string DifferentiatedRichText, TextBlock TargetTextBlock, RichTextTagsRegistry TagsRegistry, IReadOnlyList<TagDividers> TagDividers, IReadOnlyDictionary<TagID, TagDefinition> AllowedTags)
            {
                SplittedTextSegmentsSequence CreatedSplittedTextSegmentsSequence = CreateSplittedTextSegmentsSequence(SplitRichTextByDifferentiatedTags(DifferentiatedRichText), TagsRegistry);
                RichTextGenerationContext CreatedRichTextGenerationContext = new()
                {
                    OrigianlRichText = OriginalRichText, TargetTextBlock = TargetTextBlock, TagsRegistry = TagsRegistry, TagDividers = TagDividers, AllowedTags = AllowedTags, SplittedTextSegmentsSequence = CreatedSplittedTextSegmentsSequence
                };
                return CreatedRichTextGenerationContext;
            }

            /// <summary>
            /// Fill <see cref="SplittedTextSegment.AssignedTags"/> dictionaries for <see cref="SplittedTextSegment"/>s from <see cref="RichTextGenerationContext.SplittedTextSegmentsSequence"/> based on all <see cref="SplittedTextSegment"/>s with presented <see cref="SplittedTextSegment.MatchedTagExpression"/>
            /// <br/><br/>
            /// Uses <see cref="TagDefinition.AssignationOrderExpression"/> to group tags by returned <see cref="double"/> value
            /// </summary>
            public static void PerformTagsAssignation(RichTextGenerationContext RichTextGenerationContext)
            {
                IReadOnlyList<IReadOnlyList<TagDefinition>> GroupedTagPriorities = [.. RichTextGenerationContext.TagsRegistry.GroupBy(Tag => Tag.AssignationOrderExpression(RichTextGenerationContext)).OrderBy(Group => Group.Key).Select(Group => Group.ToList())];
                foreach (IReadOnlyList<TagDefinition> TagsPriorityGroup in GroupedTagPriorities)
                {
                    IReadOnlyList<TagID> CurrentTagsPriorityGroup = [.. TagsPriorityGroup.Select(Tag => Tag.ID)];

                    foreach (SplittedTextSegment TagTextSegment in RichTextGenerationContext.SplittedTextSegmentsSequence.Where(TextSegment => TextSegment.IsTag && CurrentTagsPriorityGroup.Contains(TextSegment.MatchedTagExpression!.Tag.ID)))
                    {
                        if (TagTextSegment.Ignored == false & TagTextSegment.MatchedTagExpression!.IsEndExpression == false)
                        {
                            List<SplittedTextSegment> TagAssignationCandidates = RichTextGenerationInstrumentary.DetermineTagAssignationCandidates(TagTextSegment, RichTextGenerationContext.SplittedTextSegmentsSequence);

                            if (TagTextSegment.MatchedTagExpression.Tag.CanBeAssignedToTextSegments)
                            {
                                if (TagTextSegment.MatchedTagExpression.Tag.AssignabilityToCandidatesListExpression(TagTextSegment, ref TagAssignationCandidates, RichTextGenerationContext))
                                {
                                    foreach (SplittedTextSegment TagAssignationCandidate in TagAssignationCandidates)
                                    {
                                        if (TagTextSegment.MatchedTagExpression.Tag.AssignabilityToSingleCandidateExpression(new TagDefinition.AssignationContext(TagTextSegment, TagAssignationCandidate, TagAssignationCandidates, RichTextGenerationContext)))
                                        {
                                            TagAssignationCandidate.AssignedTags.Add(TagTextSegment.MatchedTagExpression.Tag.ID, TagTextSegment.MatchedTagExpression);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Creates <see cref="Inline"/>s for <see cref="RichTextGenerationContext.TargetTextBlock"/> based on <see cref="SplittedTextSegment"/>s with assigned tags (Normal <see cref="Run"/>s) or uses <see cref="TagDefinition.StartExpressionToInlineTransformations"/>/<see cref="TagDefinition.EndExpressionToInlineTransformations"/> (Custom <see cref="Inline"/>s)
            /// <br/><br/>
            /// Enumerates <see cref="RichTextGenerationContext.SplittedTextSegmentsSequence"/> and adds <see cref="Inline"/> objects to <see cref="RichTextGenerationContext.TargetTextBlock"/> based on each <see cref="SplittedTextSegment"/> properties<br/>
            /// Related methods:<br/>
            /// • <see cref="CreaetRunWithProjectedTags"/> (If <see cref="SplittedTextSegment.IsTag"/> == <see langword="false"/>, simply create <see cref="Run"/> and call this method on it)<br/>
            /// • <see cref="ProjectAssignedTagsToInline"/> (If <see cref="SplittedTextSegment.IsTag"/> == <see langword="true"/> and <see cref="TagDefinition.StartExpressionToInlineTransformations"/> or <see cref="TagDefinition.EndExpressionToInlineTransformations"/> is not <see langword="null"/>, then sequentially create <see cref="Inline"/>s by them and use ProjectAssignedTagsToInline on each)
            /// </summary>
            public static void ProcessSplittedTextSegmentsSequenceWithAssignedTags(RichTextGenerationContext RichTextGenerationContext)
            {
                foreach (SplittedTextSegment TextSegment in RichTextGenerationContext.SplittedTextSegmentsSequence)
                {
                    if (TextSegment.Ignored == false) /// Can be dynamically changed by <see cref="TagDefinition.TagProjectionDelegate"/> or <see cref="TagDefinition.TagExpressionToInlineTransformationDelegate"/>, do not use RichTextGenerationContext.SplittedTextSegmentsSequence.Where(x => x.Ignored == false)
                    {
                        if (TextSegment.IsTag == false)
                        {
                            RichTextGenerationContext.TargetTextBlock.Inlines.Add(CreaetRunWithProjectedTags(TextSegment, RichTextGenerationContext));
                        }
                        else if (TextSegment.MatchedTagExpression!.Tag.StartExpressionToInlineTransformations is not null | TextSegment.MatchedTagExpression!.Tag.EndExpressionToInlineTransformations is not null)
                        {
                            DifferentiatedTagMatch TagInfo = TextSegment.MatchedTagExpression;

                            List<TagDefinition.TagExpressionToInlineTransformationDelegate>? TagExpressionToInlineTransformations = TagInfo.IsEndExpression == false
                                ? TagInfo.Tag.StartExpressionToInlineTransformations
                                : TagInfo.Tag.EndExpressionToInlineTransformations;

                            if (TagExpressionToInlineTransformations is not null)
                            {
                                foreach (TagDefinition.TagExpressionToInlineTransformationDelegate TagExpressionToInlineTransformation in TagExpressionToInlineTransformations)
                                {
                                    Func<DifferentiatedTagMatch, bool> AssignedTagsProjectionPredicate = (TagMatch) => true;
                                    Inline TagTransformedToInline = TagExpressionToInlineTransformation(new TagDefinition.TagToInlineTransformationContext(TextSegment.MatchedTagExpression.ExpressionMatch, TextSegment, RichTextGenerationContext), ref AssignedTagsProjectionPredicate);

                                    ProjectAssignedTagsToInline(TagTransformedToInline, TextSegment, RichTextGenerationContext, AssignedTagsProjectionPredicate);
                                    
                                    RichTextGenerationContext.TargetTextBlock.Inlines.Add(TagTransformedToInline);
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Enumerate <see cref="SplittedTextSegment.AssignedTags"/> of <paramref name="TextSegmentWithAssignedTags"/> and call <see cref="TagDefinition.TagProjections"/> from each of them on <paramref name="TargetInline"/>
            /// </summary>
            public static void ProjectAssignedTagsToInline(Inline TargetInline, SplittedTextSegment TextSegmentWithAssignedTags, RichTextGenerationContext RichTextGenerationContext, Func<DifferentiatedTagMatch, bool>? TagProjectionPredicate = null)
            {
                TagProjectionPredicate ??= (TagMatch) => true;

                foreach (DifferentiatedTagMatch AssignedTag in TextSegmentWithAssignedTags.AssignedTags.Values)
                {
                    TagDefinition.TagProjectionContext TagProjectionContext = new(TargetInline, AssignedTag.ExpressionMatch, TextSegmentWithAssignedTags, RichTextGenerationContext);

                    if (AssignedTag.Tag.TagProjections is not null && TagProjectionPredicate(AssignedTag))
                    {
                        foreach (TagDefinition.TagProjectionDelegate TagProjection in AssignedTag.Tag.TagProjections)
                        {
                            TagProjection.Invoke(Context: TagProjectionContext);
                        }
                    }
                }
            }

            /// <summary>
            /// Creates <see cref="Run"/> and uses <see cref="ProjectAssignedTagsToInline"/> on it
            /// </summary>
            public static Run CreaetRunWithProjectedTags(SplittedTextSegment TextSegment, RichTextGenerationContext Context, Func<DifferentiatedTagMatch, bool>? TagProjectionPredicate = null)
            {
                Run CreatedRun = new() { Text = TextSegment.SegmentString };
                ProjectAssignedTagsToInline(CreatedRun, TextSegment, Context, TagProjectionPredicate);
                return CreatedRun;
            }


            /// <returns>
            /// All <see cref="SplittedTextSegment"/>s after <paramref name="TextSegmentWithTag"/> until end expression hit and excluding <see cref="SplittedTextSegment"/>s inside inner regions with the same tag opened (e.g. <c>&lt;u&gt;</c>Main underline and then <c>&lt;u&gt;</c>Inner one<c>&lt;/u&gt;</c> there<c>&lt;/u&gt;</c>)
            /// </returns>
            public static List<SplittedTextSegment> DetermineTagAssignationCandidates(SplittedTextSegment TextSegmentWithTag, SplittedTextSegmentsSequence ParentSplittedTextSegmentsSequence)
            {
                if (TextSegmentWithTag.MatchedTagExpression is null)
                    throw new ArgumentException($"Given {nameof(SplittedTextSegment)} does not contain a tag ({nameof(SplittedTextSegment)}.{nameof(SplittedTextSegment.MatchedTagExpression)} is null)", nameof(TextSegmentWithTag));


                List<SplittedTextSegment> CollectedRegion = [];

                int SkipWhileThereAreSameEmbeddedTagOpened_Stack = 0;
                foreach (SplittedTextSegment TextSegment in ParentSplittedTextSegmentsSequence.AfterThe(TextSegmentWithTag))
                {
                    if (TextSegment.IsTag && TextSegment.MatchedTagExpression!.Tag == TextSegmentWithTag.MatchedTagExpression.Tag)
                    {
                        if (TextSegment.MatchedTagExpression.IsEndExpression)
                        {
                            if (SkipWhileThereAreSameEmbeddedTagOpened_Stack == 0)
                            {
                                break; // Encountered end expression of current tag
                            }
                            else
                            {
                                SkipWhileThereAreSameEmbeddedTagOpened_Stack--; // Encountered end expression of embedded tag
                            }
                        }
                        else
                        {
                            SkipWhileThereAreSameEmbeddedTagOpened_Stack++; // If embedded tag opened, increase its skipping stack
                        }
                    }

                    if (SkipWhileThereAreSameEmbeddedTagOpened_Stack == 0)
                    {
                        CollectedRegion.Add(TextSegment);
                    }
                }

                return CollectedRegion;
            }



            public static class Specific
            {
                /// <param name="TagProjectionPredicate">Can be used to exclude tag from which projection this method is called to prevent <see cref="StackOverflowException"/></param>
                /// <returns>
                /// <see cref="TextBlock"/> that inherints style-related properties from artificial <see cref="Run"/>, which was created created via <see cref="CreaetRunWithProjectedTags"/> and added to <see cref="TextBlock.Inlines"/> of copied <see cref="RichTextGenerationContext.TargetTextBlock"/>
                /// <br/><br/>
                /// Use if you need to insert text that should behave like <see cref="Run"/> into an <see cref="InlineUIContainer"/> (Probably <see cref="PreConstructedInlineUIContainer"/>)
                /// </returns>
                public static TextBlock CreateContextualContainer(SplittedTextSegment TextSegmentWithAssignedTags, RichTextGenerationContext CurrentRichTextGenerationContext, List<Inline>? InitialContent = null, Func<DifferentiatedTagMatch, bool>? TagProjectionPredicate = null)
                {
                    TextBlock CurrentTargetTextBlockTemplate = CurrentRichTextGenerationContext.TargetTextBlock.CreateBindedCopy();
                    Run TextElementTemplateInCurrentContext = CreaetRunWithProjectedTags(TextSegmentWithAssignedTags, CurrentRichTextGenerationContext, TagProjectionPredicate);
                    CurrentTargetTextBlockTemplate.Inlines.Add(TextElementTemplateInCurrentContext); // Now it considered as text of a CurrentTargetTextBlockTemplate and inherints its FontSize with other base properties..

                    TextBlock CreatedTextBlock = new();
                    List<DependencyProperty> PropertiesToBind = [
                        TextBlock.FontSizeProperty, TextBlock.FontFamilyProperty, TextBlock.FontWeightProperty, TextBlock.FontStyleProperty, TextBlock.FontStretchProperty,
                        TextBlock.ForegroundProperty, TextBlock.TextDecorationsProperty, TextBlock.TextEffectsProperty,
                    ];
                    PropertiesToBind.ForEach(PropertyToBind => {
                        CreatedTextBlock.SetValue(PropertyToBind, CurrentTargetTextBlockTemplate.GetPropertyValue<object?>(PropertyToBind.Name));
                    });

                    InitialContent?.ForEach(CreatedTextBlock.Inlines.Add);

                    return CreatedTextBlock;
                }

                /// <summary>
                /// <see cref="InlineUIContainer"/> with horizontal <see cref="StackPanel"/> as child and <see cref="BaselineAlignment"/> set to <see cref="BaselineAlignment.Bottom"/>
                /// <br/><br/>
                /// This <see cref="StackPanel"/> can be accessed through <see cref="InnerHorizontalStackPanel"/>
                /// </summary>
                public class PreConstructedInlineUIContainer : InlineUIContainer
                {
                    public StackPanel InnerHorizontalStackPanel { get; } = new() { Orientation = Orientation.Horizontal };
                    public PreConstructedInlineUIContainer(List<UIElement>? InitialHorizontalStackPanelContent = null)
                    {
                        InitialHorizontalStackPanelContent?.ForEach(InitialElement => this.InnerHorizontalStackPanel.Children.Add(InitialElement));
                        this.BaselineAlignment = BaselineAlignment.Bottom;
                        this.Child = InnerHorizontalStackPanel;
                    }
                }
            }
        }






        public static void SetRichText(TextBlock TargetTextBlock, string? InputRichText, List<TagDividers>? ExpectedTagDividers = null, RichTextTagsRegistry? TagsRegistry = null, List<TagID>? IgnoredTagIDs = null)
        {
            TagsRegistry ??= TagsPreset.DefaultRegistry;
            ExpectedTagDividers ??= [TagsPreset.DefaultDividers];
            IgnoredTagIDs ??= [];

            

            IEnumerable<TagID> PossibleTagDuplicates = TagsRegistry.Select(Tag => Tag.ID).GroupBy(Tag => Tag).Where(TagIDsGroup => TagIDsGroup.Count() > 1).Select(TagIDsGroup => TagIDsGroup.Key);
            if (PossibleTagDuplicates.Any())
                throw new ArgumentException($"Given {nameof(RichTextTagsRegistry)} contains multiple tags with the same ID: [{string.Join(", ", PossibleTagDuplicates)}]", nameof(TagsRegistry));


            TargetTextBlock.Inlines.Clear();


            Dictionary<TagID, TagDefinition> AllowedTags = TagsRegistry.Where(Tag => IgnoredTagIDs.Contains(Tag.ID) == false).ToDictionary(Tag => Tag.ID, Tag => Tag);
            AllowedTags
                .Where(Tag => Tag.Value.TextPreProcessingActions is not null)
                .OrderBy(Tag => Tag.Value.TextPreProcessingActionsOrderExpression(InputRichText, TargetTextBlock, TagsRegistry, ExpectedTagDividers, AllowedTags))
                .SelectMany(Tag => Tag.Value.TextPreProcessingActions!)
                .ToList()
                .ForEach(PreProcessing => PreProcessing?.Invoke(ref InputRichText, TargetTextBlock, TagsRegistry, ref ExpectedTagDividers, ref AllowedTags));


            if (!string.IsNullOrEmpty(InputRichText))
            {
                if (ExpectedTagDividers.Any(Divider => InputRichText.MatchesOneOf(Divider.Open, Divider.Close)) == false)
                {
                    TargetTextBlock.Text = InputRichText; // Set plain text if there are no expected tag dividers in text
                }
                else
                {
                    #region Explicitly separate actual tags in text using ExpressionMarkup chars
                    string DifferentiatedRichText = RichTextGenerationInstrumentary.PerformTagsDifferentiation(InputRichText, ExpectedTagDividers, AllowedTags);
                    #endregion


                    #region Create context
                    RichTextGenerationContext CurrentRichTextGenerationContext = RichTextGenerationInstrumentary.FormSplittedTextSegmentsSequenceAndContext(InputRichText, DifferentiatedRichText, TargetTextBlock, TagsRegistry, ExpectedTagDividers, AllowedTags);
                    #endregion


                    #region Assign tags
                    RichTextGenerationInstrumentary.PerformTagsAssignation(CurrentRichTextGenerationContext);
                    #endregion


                    #region Create inlines
                    RichTextGenerationInstrumentary.ProcessSplittedTextSegmentsSequenceWithAssignedTags(CurrentRichTextGenerationContext);
                    #endregion
                }
            }
        }
    }
}