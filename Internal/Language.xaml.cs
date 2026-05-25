using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Markup;
using static LCLocalizationInterface.Internal.@Languages;
using static LCLocalizationInterface.Internal.@Languages.JsonClasses.UIModifyingParameters;
using static LCLocalizationInterface.TextMeshLarp;

namespace LCLocalizationInterface.Internal
{
    namespace Terminus
    {
        /// <summary>Basic <see langword="interface"/> for <see cref="IntenseStareType1"/>, and <see cref="IntenseStareType3"/></summary>
        public interface UITranslationEntry
        {
            public string UID { get; }
            public string Text { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public double FontSize { get; set; }
            public FontFamily FontFamily { get; set; }
            public FontWeight FontWeight { get; set; }
            public FontStretch FontStretch { get; set; }
            public FontStyle FontStyle { get; set; }
            public Thickness Margin { get; set; }
            public TextAlignment TextAlignment { get; set; }

            public static List<PropertyInfo> AccessibleProperties
                => [.. typeof(UITranslationEntry).GetProperties().Where(x => !x.Name.EqualsToOneOf(nameof(UID), nameof(Text), nameof(DefaultValues), nameof(AccessibleProperties)))];

            public Dictionary<string, object?> DefaultValues { get; }
            public void StashDefaultValues()
            {
                DefaultValues.Clear();
                foreach (PropertyInfo AccessibleProperty in UITranslationEntry.AccessibleProperties)
                {
                    DefaultValues[AccessibleProperty.Name] = this.GetPropertyValue<object?>(AccessibleProperty.Name);
                }

                if (this is IntenseStareType1 TextElement && !TextElement.UID.Contains("[-]")) // Default text
                {
                    DefaultValues[TextElement.DesiredTextPropertyName] = TextElement.DesiredTextPropertyValue;
                }
            }
            public void ApplyDefaultValues()
            {
                foreach (PropertyInfo AccessibleProperty in UITranslationEntry.AccessibleProperties)
                {
                    this.SetPropertyValue<object?>(AccessibleProperty.Name, DefaultValues[AccessibleProperty.Name]);
                }

                if (this is IntenseStareType1 TextElement && !TextElement.UID.Contains("[-]")) // Default text
                {
                    TextElement.DesiredTextPropertyValue = (string?) DefaultValues[TextElement.DesiredTextPropertyName];
                }
            }
        };








        #region Generic text element
        /// <summary><see cref="TextBlock"/></summary>
        [ContentProperty(nameof(RichText))]
        public class IntenseStareType1 : TextBlock, UITranslationEntry
        {
            #region Translation UID
            public string UID { get => (string)GetValue(UIDProperty); set => SetValue(UIDProperty, value); }
            public static readonly DependencyProperty UIDProperty = RegisterProperty<IntenseStareType1, string>(PropertyChangedEvent: OnUIDChanged);
            private static void OnUIDChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                string? DefinedUID = Args.NewValue as string;
                if (!string.IsNullOrWhiteSpace(DefinedUID) && !PresentedTextElements.ContainsKey(DefinedUID!))
                {
                    PresentedTextElements[DefinedUID] = (IntenseStareType1)Sender;
                    PresentedTextElements_Interface[DefinedUID] = (IntenseStareType1)Sender;
                }
            }
            #endregion



            #region Default values
            public Dictionary<string, object?> DefaultValues { get; } = [];
            #endregion




            #region Rich text
            public string DesiredTextPropertyName => this.UID.Contains("[!]") ? nameof(Text) : nameof(RichText);

            /// <summary>
            /// Gets/sets value of <see cref="IntenseStareType1.RichText"/> or regular <see cref="TextBlock.Text"/> properties based on presence of "<c>[!]</c>" symbol in <see cref="UID"/>, which should indicate that rich text is not available for this text element
            /// </summary>
            public string? DesiredTextPropertyValue
            {
                get => this.GetPropertyValue<string?>(this.DesiredTextPropertyName);
                set => this.SetPropertyValue<string?>(this.DesiredTextPropertyName, value);
            }

            public string? RichText { get => (string?)GetValue(RichTextProperty); set => SetValue(RichTextProperty, value); }
            public static readonly DependencyProperty RichTextProperty = RegisterProperty<IntenseStareType1, string?>(PropertyChangedEvent: OnRichTextChanged);
            private static void OnRichTextChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                string? InputRichText = (string?)Args.NewValue;
                IntenseStareType1 ActualSender = (IntenseStareType1)Sender;

                // Square brackets can be used in XAML instead of &lt; &gt;
                TextMeshLarp.SetRichText(ActualSender, ActualSender.ShouldTrimRichText ? InputRichText?.Trim() : InputRichText, [new(@"\[", @"\]"), new(@"<", @">")], IgnoredTagIDs: [ImportableLimbusTags.Mark.ID, ImportableLimbusTags.ChangesHighlight.ID]); // Except some specific for limbus text preview
            }

            public bool ShouldTrimRichText { get => (bool)GetValue(ShouldTrimRichTextProperty); set => SetValue(ShouldTrimRichTextProperty, value); }
            public static readonly DependencyProperty ShouldTrimRichTextProperty = RegisterProperty<IntenseStareType1, bool>(DefaultValue: false);
            #endregion




            #region Stored Extern/Exform values
            /// <summary>Value stored by <see cref="@Languages.ExternElement"/></summary>
            public object? CurrentExtern { get; set; } = null;

            /// <summary>Value stored by <see cref="@Languages.ExternElement"/> / <see cref="@Languages.ExformElement"/></summary>
            public string? CurrentVariableKey { get; set; } = null;

            /// <summary>Value stored by <see cref="@Languages.ExformElement"/></summary>
            public object?[]? CurrentExform { get; set; } = null;
            #endregion




            #region Shadow text based on related IntenseStareType3
            // Shadow text thing....  Hide this IntenseStareType1 when binded IntenseStareType1's Document.Text is empty (Mostly used in the Right Menu)
            private bool AlreadyAppliedAsShadowText = false;
            public UIElement? HideWhenThisDocumentTextIsNotEmpty { get => (UIElement?)GetValue(HideWhenThisDocumentTextIsNotEmptyProperty); set => SetValue(HideWhenThisDocumentTextIsNotEmptyProperty, value); }
            public static readonly DependencyProperty HideWhenThisDocumentTextIsNotEmptyProperty = RegisterProperty<IntenseStareType1, UIElement?>(PropertyChangedEvent: OnHideWhenThisDocumentTextIsNotEmptyChanged);
            private static void OnHideWhenThisDocumentTextIsNotEmptyChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                IntenseStareType1 ActualSender = (IntenseStareType1)Sender;
                if (ActualSender.AlreadyAppliedAsShadowText == false && Args.NewValue is IntenseStareType3 SourceElement)
                {
                    ActualSender.AlreadyAppliedAsShadowText = true;

                    ActualSender.Style = new()
                    {
                        TargetType = typeof(IntenseStareType1),
                        BasedOn = (Style)Application.Current.FindResource(typeof(IntenseStareType1)),
                        Setters =
                        {
                            // Default setters for Right Menu
                            new Setter(property: VisibilityProperty, value: Visibility.Collapsed),
                            new Setter(property: VerticalAlignmentProperty, value: VerticalAlignment.Center),
                            new Setter(property: IsHitTestVisibleProperty, value: false),
                            new Setter(property: PaddingProperty, value: new Thickness(5.5, 0, 8, 0)),
                            new Setter(property: TextWrappingProperty, value: TextWrapping.NoWrap),
                        },
                        Triggers =
                        {
                            new DataTrigger()
                            {
                                Binding = new Binding() { Source = SourceElement, Path = new PropertyPath("Document.Text"), FallbackValue = "" },
                                Value = "",
                                Setters = { new Setter(property: VisibilityProperty, value: Visibility.Visible) },
                            }
                        }
                    };
                    if (ActualSender.UID != "[Main UI] * Json Path (Shadow Text)") ActualSender.FontSize = 21; // ?????????????????????
                    ActualSender.SetResourceReference(ForegroundProperty, "Theme:UITextfields.ShadowText.Foreground");
                }
            }
            #endregion




            #region Other IntenseStareType1's style copying
            /// <summary>Create binding for all style properties</summary>
            public void InherintPropertiesFrom(IntenseStareType1 AnotherTextElement, bool IncludeRichText = false)
            {
                this.BindSameProperties(
                    BindingSource: AnotherTextElement,
                    Properties:
                    [
                        FontFamilyProperty, FontWeightProperty,    FontSizeProperty,
                        ForegroundProperty, TextAlignmentProperty, IncludeRichText ? RichTextProperty : null,
                        MarginProperty,     PaddingProperty,
                        WidthProperty,      HeightProperty
                    ]
                );
            }

            /// <summary>Create binding for specific style properties</summary>
            public void InherintPropertiesPartiallyFrom(IntenseStareType1 AnotherTextElement)
            {
                this.BindSameProperties(
                    BindingSource: AnotherTextElement,
                    Properties:
                    [
                        FontFamilyProperty, FontWeightProperty, TextAlignmentProperty,
                        MarginProperty,     PaddingProperty
                    ]
                );
            }

            /// <summary>
            /// Same as <see cref="InherintPropertiesFrom"/>, but can be set from XAML designer via <c>StyleInherintingSource="{Binding ElementName=SomeOtherIntenseStareType1}"</c><br/>
            /// (Does not include <see cref="RichText"/>)
            /// </summary>
            public IntenseStareType1? StyleInherintingSource { get => (IntenseStareType1?)GetValue(StyleInherintingSourceProperty); set => SetValue(StyleInherintingSourceProperty, value); }
            public static readonly DependencyProperty StyleInherintingSourceProperty = RegisterProperty<IntenseStareType1, IntenseStareType1?>(PropertyChangedEvent: OnStyleInherintingSourceChanged);
            private static void OnStyleInherintingSourceChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                if (Args.NewValue is not null)
                {
                    IntenseStareType1 ActualSender = (Sender as IntenseStareType1)!;
                    ActualSender.InherintPropertiesFrom((Args.NewValue as IntenseStareType1)!, IncludeRichText: false);
                }
            }


            public IntenseStareType1? PartialStyleInherintingSource { get => (IntenseStareType1?)GetValue(PartialStyleInherintingSourceProperty); set => SetValue(PartialStyleInherintingSourceProperty, value); }
            public static readonly DependencyProperty PartialStyleInherintingSourceProperty = RegisterProperty<IntenseStareType1, IntenseStareType1?>(PropertyChangedEvent: OnPartialStyleInherintingSourcePropertyChanged);
            private static void OnPartialStyleInherintingSourcePropertyChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                if (Args.NewValue is not null)
                {
                    IntenseStareType1 ActualSender = (Sender as IntenseStareType1)!;
                    ActualSender.InherintPropertiesPartiallyFrom((Args.NewValue as IntenseStareType1)!);
                }
            }
            #endregion




            #region Limbus editor
            public void MarkWithUnsavedByCondition(bool Condition, object? ExtraExtern = null)
            {
                if (Condition) this.MarkWithUnsaved(ExtraExtern);
                else this.SetDefaultText(ExtraExtern);
            }
            public void MarkWithUnsavedByCondition(bool? Condition, object? ExtraExtern = null) // Nullable<Boolean> version
            {
                MarkWithUnsavedByCondition(Condition is not null && (bool)Condition, ExtraExtern);
            }

            public void MarkWithUnsaved(object? ExtraExtern = null)
            {
                if (LoadedConfiguration.Internal.EnableAutoSave == false)
                {
                    if (LoadedStaticTextModifiers.TryGetValue(this.UID, out UIModifier? FoundModifier))
                    {
                        this.RichText = VariableData.UnsavedChangesMarkerContainer.Extern(FoundModifier.Text.Extern(ExtraExtern));
                    }
                    else
                    {
                        this.RichText = $"Unknown text {(this.UID.Contains("[!]") ? "" : "<size=85%>")}({this.UID})";
                    }
                }
            }

            /// <summary>Take string from <see cref="@LoadedStaticTextModifiers"/></summary>
            public void SetDefaultText(object? ExtraExtern = null)
            {
                CurrentExtern = ExtraExtern;
                CurrentExform = null;
                if (@LoadedStaticTextModifiers.TryGetValue(this.UID, out UIModifier? FoundModifier))
                {
                    string Text = FoundModifier.Variable is not null
                        ? FoundModifier.Variable.First().Value
                        : FoundModifier.Text;

                    Text = Text.Extern(ExtraExtern);

                    this.RichText = Text;
                }
                else
                {
                    this.RichText = $"Unknown text {(this.UID.Contains("[!]") ? "" : "<size=85%>")}({this.UID})";
                }
            }
            #endregion




            #region Additional things
            /// <summary>Binds <see cref="TextBlock.LineHeight"/> to <see cref="TextBlock.FontSize"/>, sometimes looks better inside buttons</summary>
            public bool PerfectVerticalAlign { get => (bool)GetValue(PerfectVerticalAlignProperty); set => SetValue(PerfectVerticalAlignProperty, value); }
            public static readonly DependencyProperty PerfectVerticalAlignProperty = RegisterProperty<IntenseStareType1, bool>(DefaultValue: false, PropertyChangedEvent: OnPerfectVerticalAlignChanged);
            private static void OnPerfectVerticalAlignChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                if ((bool)Args.NewValue == true)
                {
                    (Sender as IntenseStareType1)!.SetBinding(LineHeightProperty, new Binding(nameof(IntenseStareType1.FontSize)) { RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
                }
                else
                {
                    (Sender as IntenseStareType1)!.SetValue(IntenseStareType1.LineHeightProperty, DependencyProperty.UnsetValue);
                }
            }
            #endregion
        }
        #endregion








        #region Text input (With documents and syntax)
        /// <summary><see cref="SyntaxedTextEditorBase"/></summary>
        public class IntenseStareType3 : SyntaxedTextEditorBase, UITranslationEntry
        {
            public IntenseStareType3()
            {
                /// Neccessary for <see cref="BindableText"/>
                this.Document = new();

                // Clear selection when focus lost
                this.TextArea.LostKeyboardFocus += (_, _) => this.Select(0, 0);

                // Format backslashes on text paste
                this.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(delegate (object Sender, DataObjectPastingEventArgs Args)
                {
                    string TextToPaste = (string)Args.DataObject.GetData(DataFormats.UnicodeText);

                    DataObject TransformedText = new();
                    TransformedText.SetText(TextToPaste.Replace("\\\"", "\"").Replace("\n", "\\n").Cut("\r"), TextDataFormat.UnicodeText);

                    Args.DataObject = TransformedText;
                }));

                // RoutedEvent for default TextChanged (Which is not)
                this.TextChanged += (_, _) => RaiseEvent(new RoutedEventArgs(RoutedTextChangedEvent, this));
                this.TextChanged += (_, _) => CheckInputPathExistance();

                // IntenseStareType3 is always single line input (AcceptsReturn="False")
                this.PreviewKeyDown += delegate (object Sender, KeyEventArgs Args)
                {
                    if (Args.Key == Key.Return)
                    {
                        Args.Handled = true;
                    }
                };
            }


            #region Translation UID
            public string UID { get => (string)GetValue(UIDProperty); set => SetValue(UIDProperty, value); }
            public static readonly DependencyProperty UIDProperty = RegisterProperty<IntenseStareType3, string>(PropertyChangedEvent: OnUIDChanged);
            private static void OnUIDChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                string? DefinedUID = Args.NewValue as string;
                if (!string.IsNullOrEmpty(DefinedUID) && !PresentedTextFields_Interface.ContainsKey(DefinedUID!))
                {
                    PresentedTextFields_Interface[DefinedUID!] = (UITranslationEntry)Sender;
                    PresentedTextFields[DefinedUID] = (IntenseStareType3)Sender;
                }
            }
            #endregion




            #region Default values
            public Dictionary<string, object?> DefaultValues { get; } = [];
            #endregion




            #region Limbus editor
            public void MarkWithUnsavedByCondition(bool Condition)
            {
                if (LoadedConfiguration.Internal.EnableAutoSave == false)
                {
                    BorderBrush =
                        ToSolidColorBrush(
                            Condition
                                ? @Themes.CurrentTheme.UIText.UnsavedChangesMarkerColor
                                : @Themes.CurrentTheme.UITextfields.Border
                        );
                }
                else
                {
                    BorderBrush = ToSolidColorBrush(@Themes.CurrentTheme.UITextfields.Border);
                }
            }
            #endregion




            #region BindableText
            public string? BindableText { get => (string?)GetValue(BindableTextProperty); set => SetValue(BindableTextProperty, value);}
            public static readonly DependencyProperty BindableTextProperty = RegisterProperty<IntenseStareType3, string?>(PropertyChangedEvent: OnBindableTextChanged, BindsTwoWayByDefault: true);
            private static void OnBindableTextChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                IntenseStareType3 ActualSender = (Sender as IntenseStareType3)!;
                if (ActualSender.Document is not null && ActualSender.Document.Text != (string?)Args.NewValue)
                {
                    ActualSender.Document.Text = (string?)Args.NewValue ?? string.Empty;
                }
            }
            protected override void OnTextChanged(EventArgs Args)
            {
                SetCurrentValue(BindableTextProperty, Document?.Text);
                base.OnTextChanged(Args);
            }
            #endregion




            #region TextChanged but RoutedEvent
            public event RoutedEventHandler RoutedTextChanged { add => AddHandler(RoutedTextChangedEvent, value); remove => RemoveHandler(RoutedTextChangedEvent, value); }
            public static readonly RoutedEvent RoutedTextChangedEvent = RegisterEvent<IntenseStareType3, RoutedEventHandler>();
            #endregion




            #region Additional things
            /// <summary>Does nothing in <see cref="ICSharpCode.AvalonEdit.TextEditor"/>, needed to satisfy <see cref="UITranslationEntry"/> <see langword="interface"/></summary>
            [Obsolete] public TextAlignment TextAlignment { get; set; }

            /// <summary>Returns <see cref="UIElement.IsFocused"/> from <see cref="ICSharpCode.AvalonEdit.TextEditor.TextArea"/></summary>
            public new bool IsFocused => this.TextArea.IsFocused;

            #region Input color visualization
            public bool DisplayInputColor { get => (bool)GetValue(DisplayInputColorProperty); set => SetValue(DisplayInputColorProperty, value); }
            public static readonly DependencyProperty DisplayInputColorProperty = RegisterProperty<IntenseStareType3, bool>(DefaultValue: false);
            #endregion


            #region TextMaxLength
            public uint? TextMaxLength { get => (uint?)GetValue(TextMaxLengthProperty); set => SetValue(TextMaxLengthProperty, value); }
            public static readonly DependencyProperty TextMaxLengthProperty = RegisterProperty<IntenseStareType3, uint?>(PropertyChangedEvent: OnTextMaxLengthChanged);

            private static void OnTextMaxLengthChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                IntenseStareType3 ActualSender = (Sender as IntenseStareType3)!;

                if ((uint?)Args.NewValue is not null)
                {
                    ActualSender.TextArea.TextEntering += TextMaxLength_OnTextEntering;
                    ActualSender.AddHandler(DataObject.PastingEvent, TextMaxLength_PastingHandler);
                }
                else
                {
                    ActualSender.TextArea.TextEntering -= TextMaxLength_OnTextEntering;
                    ActualSender.RemoveHandler(DataObject.PastingEvent, TextMaxLength_PastingHandler);
                }
            }
            private static void TextMaxLength_OnTextEntering(object Sender, TextCompositionEventArgs Args)
            {
                IntenseStareType3 ActualSender = (Sender as TextArea)!.FindVisualParent<IntenseStareType3>()!;
                
                if (ActualSender.Document.TextLength + Args.Text.Length - ActualSender.SelectionLength > ActualSender.TextMaxLength) Args.Handled = true;
            }
            private static DataObjectPastingEventHandler TextMaxLength_PastingHandler = new(delegate (object Sender, DataObjectPastingEventArgs Args)
            {
                IntenseStareType3 ActualSender = (Sender as IntenseStareType3)!;
                string TextToPaste = (string)Args.DataObject.GetData(DataFormats.UnicodeText);

                if (TextToPaste.Length + ActualSender.Document.TextLength - ActualSender.SelectionLength > ActualSender.TextMaxLength)
                {
                    Args.CancelCommand();
                }
            });
            #endregion


            #region File/Directory selection buttons
            public enum PathType { File, Directory, None }

            public FileSelectAttributes FileSelectAttributes { get => (FileSelectAttributes)GetValue(FileSelectAttributesProperty); set => SetValue(FileSelectAttributesProperty, value); }
            public static readonly DependencyProperty FileSelectAttributesProperty = RegisterProperty<IntenseStareType3, FileSelectAttributes>(DefaultValue: new());

            public PathType PathSelectionButtonType { get => (PathType)GetValue(PathSelectionButtonTypeProperty); set => SetValue(PathSelectionButtonTypeProperty, value); }
            public static readonly DependencyProperty PathSelectionButtonTypeProperty = RegisterProperty<IntenseStareType3, PathType>(DefaultValue: PathType.None);

            public double PathSelectionButtonSpacing { get => (double)GetValue(PathSelectionButtonSpacingProperty); set => SetValue(PathSelectionButtonSpacingProperty, value); }
            public static readonly DependencyProperty PathSelectionButtonSpacingProperty = RegisterProperty<IntenseStareType3, double>(DefaultValue: 4);

            public event RoutedEventHandler PathSelectionAction { add => AddHandler(PathSelectionActionEvent, value); remove => RemoveHandler(PathSelectionActionEvent, value); }
            public static readonly RoutedEvent PathSelectionActionEvent = RegisterEvent<IntenseStareType3, RoutedEventHandler>();

            public void CheckInputPathExistance()
            {
                if (this.HighlightMissingPath is not PathType.None)
                {
                    Func<string?, bool> ExistanceCheckProvider = this.HighlightMissingPath switch
                    {
                        PathType.File => File.Exists,
                        PathType.Directory => Directory.Exists
                    };

                    IsInputPathExists = ExistanceCheckProvider(this.BindableText);
                }
                else
                {
                    IsInputPathExists = true;
                }
            }


            public PathType HighlightMissingPath { get => (PathType)GetValue(HighlightMissingPathProperty); set => SetValue(HighlightMissingPathProperty, value); }
            public static readonly DependencyProperty HighlightMissingPathProperty = RegisterProperty<IntenseStareType3, PathType>(DefaultValue: PathType.None, PropertyChangedEvent: OnHighlightMissingPathChanged);
            private static void OnHighlightMissingPathChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                (Sender as IntenseStareType3)!.CheckInputPathExistance();
            }

            // Set by TextChaged check
            public bool IsInputPathExists { get => (bool)GetValue(IsInputPathExistsProperty); set => SetValue(IsInputPathExistsProperty, value); }
            public static readonly DependencyProperty IsInputPathExistsProperty = RegisterProperty<IntenseStareType3, bool>(DefaultValue: true);
            #endregion



            #region Syntax shenanigans
            public void ResetHighlightDefinition() => this.SyntaxHighlighting = new SyntaxedTextEditorBase.SyntaxHighlightDefinition();

            public record RegexHighlightSimple([StringSyntax(StringSyntaxAttribute.Regex)] string Pattern, string Foreground, bool UseUnderline = true);
            public void AddHighlight(params RegexHighlightSimple[] Highlights)
            {
                foreach (RegexHighlightSimple Highlight in Highlights)
                {
                    SyntaxHighlighting.MainRuleSet.Rules.Add(new HighlightingRule()
                    {
                        Regex = new Regex(Highlight.Pattern),
                        Color = new HighlightingColor() { Foreground = new HighlightionBrush(Highlight.Foreground), Underline = Highlight.UseUnderline }
                    });
                }
            }
            #endregion

            #endregion
        }

        public class FileSelectAttributes
        {
            public string FilesHint { get; set; } = "";
            public string Extensions { get; set; } = "*";
        }
        #endregion


        public partial class LanguageUIElementsResourceDictionary : ResourceDictionary
        {
            private void PathSelection_File_Click(object Sender, RoutedEventArgs Args)
            {
                IntenseStareType3 ActualSender = (Sender as Button)!.FindVisualParent<IntenseStareType3>()!;

                OpenFileDialog FileSelect = NewOpenFileDialog(ActualSender.FileSelectAttributes.FilesHint, ActualSender.FileSelectAttributes.Extensions.Split(", "));
                if (FileSelect.ShowDialog() == true)
                {
                    ActualSender.Document.Text = FileSelect.FileName.Cut(Path.GetDirectoryName(Environment.ProcessPath) + "\\").Replace("\\", "/");
                    ActualSender.ScrollToHorizontalOffset(double.PositiveInfinity);
                    ActualSender.RaiseEvent(new RoutedEventArgs(IntenseStareType3.PathSelectionActionEvent, ActualSender));
                }
            }

            private void PathSelection_Directory_Click(object Sender, RoutedEventArgs Args)
            {
                IntenseStareType3 ActualSender = (Sender as Button)!.FindVisualParent<IntenseStareType3>()!;

                OpenFolderDialog DirectorySelect = new();
                if (DirectorySelect.ShowDialog() == true)
                {
                    ActualSender.Document.Text = DirectorySelect.FolderName.Cut(Path.GetDirectoryName(Environment.ProcessPath) + "\\").Replace("\\", "/");
                    ActualSender.ScrollToHorizontalOffset(double.PositiveInfinity);
                    ActualSender.RaiseEvent(new RoutedEventArgs(IntenseStareType3.PathSelectionActionEvent, ActualSender));
                }
            }
        }







        #region Simplified context menu
        /// <summary><see cref="MenuItem"/> with <see cref="IntenseStareType1"/> as header</summary>
        public class MenuItem_T1 : MenuItem
        {
            public MenuItem_T1()
            {
                this.Header = HeaderText = new IntenseStareType1();
            }

            public IntenseStareType1 HeaderText;

            public string UID { get => (string)GetValue(UIDProperty); set => SetValue(UIDProperty, value); }
            public static readonly DependencyProperty UIDProperty = RegisterProperty<MenuItem_T1, string>(PropertyChangedEvent: OnUIDChanged);
            private static void OnUIDChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                (Sender as MenuItem_T1)!.HeaderText.UID = (string)Args.NewValue;
            }

            public string RichText { get => (string)GetValue(RichTextProperty); set => SetValue(RichTextProperty, value); }
            public static readonly DependencyProperty RichTextProperty = RegisterProperty<MenuItem_T1, string>(PropertyChangedEvent: OnRichTextChanged);
            private static void OnRichTextChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                (Sender as MenuItem_T1)!.HeaderText.RichText = (string)Args.NewValue;
            }
        }

        /// <summary><see cref="ContextMenu"/> with <see cref="MenuItem_T1"/> inside, <see cref="MenuItem_T1"/>'s header is <see cref="IntenseStareType1"/></summary>
        public class PopupButton_T1 : ContextMenu
        {
            private readonly MenuItem_T1 ContainedMenuItem = new();
            public PopupButton_T1() => this.Items.Add(ContainedMenuItem);


            public string UID { set => ContainedMenuItem.UID = value; }
            public string RichText { set => ContainedMenuItem.RichText = value; }

            public RoutedEventHandler? Click
            {
                get => field;
                set
                {
                    if (field is not null) ContainedMenuItem.Click -= field;
                    field = value;
                    if (field is not null) ContainedMenuItem.Click += field;
                }
            }
        }
        #endregion






        public static class Assistant
        {
            #region PreviewInputRegexHandling
            public static readonly DependencyProperty PreviewInputRegexHandlingProperty = RegisterAttachedProperty(
                PropertyType: typeof(string), OwnerType: typeof(Assistant), DefaultValue: null,
                PropertyChangedEvent: delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
                {
                    if (Args.NewValue is string Pattern && !string.IsNullOrEmpty(Pattern) && Sender is UIElement TextInputElement)
                    {
                        TextInputElement.PreviewTextInput += delegate (object Sender, TextCompositionEventArgs Args)
                        {
                            Args.Handled = Args.Text.Matches(Pattern) == false;
                        };

                        // Also consider text pasting
                        TextInputElement.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(delegate (object Sender, DataObjectPastingEventArgs Args)
                        {
                            string TextToPaste = (string)Args.DataObject.GetData(DataFormats.UnicodeText);
                            if (TextToPaste.Matches(Pattern) == false) Args.CancelCommand();
                        }));
                    }
                }
            );
            public static string GetPreviewInputRegexHandling(DependencyObject Sender) => (string)Sender.GetValue(PreviewInputRegexHandlingProperty);
            public static void SetPreviewInputRegexHandling(DependencyObject Sender, [StringSyntax(StringSyntaxAttribute.Regex)] string Value) => Sender.SetValue(PreviewInputRegexHandlingProperty, Value);
            #endregion
        }
    }
















    public ref struct @Languages
    {
        #region Main navigation nodes
        public static Dictionary<string, Terminus.IntenseStareType1> PresentedTextElements { get; } = [];
        public static Dictionary<string, Terminus.IntenseStareType3> PresentedTextFields { get; } = [];

        public static Dictionary<string, Terminus.UITranslationEntry> PresentedTextElements_Interface { get; } = [];
        public static Dictionary<string, Terminus.UITranslationEntry> PresentedTextFields_Interface { get; } = [];
        #endregion



        #region Loaded data
        /// <summary><c>@ Font References.json</c> file</summary>
        public static Dictionary<string, FontFamily> @LoadedFontReferences { get; private set; } = [];

        /// <summary><c>Static UI Text.json</c> file</summary>
        public static Dictionary<string, @JsonClasses.UIModifyingParameters.UIModifier> @LoadedStaticTextModifiers { get; private set; } = [];

        public static @JsonClasses.UIModifyingParameters.FontDefaultSettings @LoadedStaticTextModifiers_FontDefaults { get; private set; } = new();

        /// <summary>
        /// UIDs of loaded <see cref="UIModifier"/>s that have color theme keys from <see cref="@Themes.ThemeDefinition.ColorKeysForTranslation"/> in their text
        /// </summary>
        public static List<string> @LoadedStaticTextModifiersWithThemeKeys { get; } = [];


        /// <summary><see cref="UnsavedChangesMarkerContainer"/>,   <see cref="InsertionsDefaultValue"/>,   <see cref="UnsavedChangesInfo"/>,   <see cref="ReadedStartupSteps"/></summary>
        public ref struct VariableData
        {
            public static string UnsavedChangesMarkerContainer {
                get => field.Replace("{Theme.UnsavedChangesMarker}", @Themes.CurrentTheme.UIText.UnsavedChangesMarkerColor);
                set => field = value;
            } = "[$] <size=66%><color={Theme.UnsavedChangesMarker}><b>(Changed)</b></color></size>"; // [$] is original button text

            public static string InsertionsDefaultValue { get; set; } = "…";

            public static @JsonClasses.UnsavedChangesInfoText UnsavedChangesInfo { get; set; } = new();
            public static @JsonClasses.StartupSteps ReadedStartupSteps { get; set; } = new();
        }
        #endregion



        #region UI updating methods
        /// <summary>
        /// Get text from <see cref="@LoadedStaticTextModifiers"/> by given <paramref name="UID"/>, maybe with <paramref name="VariableKey"/> from <see cref="UIModifier.Variable"/><br/>
        /// </summary>
        /// <returns>
        /// Acquired text or 'Unknown text ...'
        /// </returns>
        public static string GetLocalizationTextFor(string UID, string? VariableKey = null)
        {
            if (@LoadedStaticTextModifiers.TryGetValue(UID, out UIModifier? FoundModifier))
            {
                if (VariableKey is not null)
                {
                    if (FoundModifier.Variable is not null && FoundModifier.Variable.TryGetValue(VariableKey, out string? FoundVariableValue))
                    {
                        return FoundVariableValue;
                    }
                    else { return $"Unknown text {(UID.Contains("[!]") ? "" : "<size=85%>")}(\"{UID}\".Variable[\"{VariableKey}\"])"; }
                }
                else
                {
                    return FoundModifier.Text;
                }
            }
            else { return $"Unknown text {(UID.Contains("[!]") ? "" : "<size=85%>")}(\"{UID}\"{(VariableKey is not null ? $".Variable[\"{VariableKey}\"]" : "")})"; }
        }


        /// <summary>Set text from <see cref="UIModifier.Variable"/> || Replace <b>[$]</b> || Both in sequence</summary>
        public static void ExternElement(string UID, string? VariableKey = null, object? ExternObject = null)
        {
            IntenseStareType1 TextElement = PresentedTextElements[UID];

            TextElement.CurrentExtern = ExternObject;
            TextElement.CurrentExform = null;
            TextElement.CurrentVariableKey = VariableKey;

            string GettedText = GetLocalizationTextFor(UID, VariableKey).Extern(ExternObject);

            TextElement.DesiredTextPropertyValue = GettedText;
        }

        /// <summary>Set text from <see cref="UIModifier.Variable"/> || Replace <b>[$1]</b>, <b>[$2]</b>, <b>...</b> || Both in sequence</summary>
        public static void ExformElement(string UID, string? VariableKey = null, params object?[] ExformObjects)
        {
            IntenseStareType1 TextElement = PresentedTextElements[UID];

            TextElement.CurrentExtern = null;
            TextElement.CurrentExform = ExformObjects;
            TextElement.CurrentVariableKey = VariableKey;

            string GettedText = GetLocalizationTextFor(UID, VariableKey).Exform(ExformObjects);

            TextElement.DesiredTextPropertyValue = GettedText;
        }
        

        public static void ExternElementConditional(string UID, bool Condition, string FailureVariableKey, string SuccessVariableKey, string SuccessExternString)
        {
            if (Condition)
            {
                @Languages.ExternElement(UID, SuccessVariableKey, SuccessExternString);
            }
            else
            {
                @Languages.ExternElement(UID, FailureVariableKey);
            }
        }
        #endregion



        #region Inline image TagDefinition for TextMesh Larp
        public static readonly TagDefinition InlineImage = new(@"image source=""(?<ImageSourceExpression>.*?)""(\s+((size=(?<Size>-?\d+))|(xoffset=(?<X>-?\d+))|(yoffset=(?<Y>-?\d+))))*", null, new TagID(nameof(InlineImage)))
        {
            CanBeAssignedToTextSegments = false,
            StartExpressionToInlineTransformations = [delegate (TagDefinition.TagToInlineTransformationContext Context, ref Func<DifferentiatedTagMatch, bool> AssignedTagsProjectionPredicate)
            {
                string ImageSourceExpression = Context.TagExpressionMatch.Groups["ImageSourceExpression"].Value;

                double GetGroupValue(string GroupName, double Fallback)
                {
                    return double.TryParse(Context.TagExpressionMatch.Groups[GroupName].Value, out double DefinedValue) ? DefinedValue : Fallback;
                }
                double Size  = GetGroupValue("Size",  Context.RichTextGenerationContext.TargetTextBlock.FontSize);
                double XOffset = GetGroupValue("X", 0);
                double YOffset = GetGroupValue("Y", 0);

                Brush ImageBackground = Context.CurrentTextSegmentWithTag.AssignedTags.TryGetValue(nameof(TagsPreset.Background), out DifferentiatedTagMatch? BackgroundTag)
                    ? ToSolidColorBrush(BackgroundTag.ExpressionMatch.Groups["ColorValue"].Value)
                    : Context.CurrentTextSegmentWithTag.AssignedTags.TryGetValue("Mark", out DifferentiatedTagMatch? MarkTag)
                        ? ToSolidColorBrush(MarkTag.ExpressionMatch.Groups["ColorValue"].Value)
                        : Brushes.Transparent;

                Image CreatedImage = new Image()
                {
                    Source = File.Exists(ImageSourceExpression) ? BitmapFromFile(ImageSourceExpression) : ImageDictionaries.UnknownSpriteImage,
                    Width = Size, Height = Size,
                    RenderTransform = new TranslateTransform(XOffset, YOffset)
                };
                Canvas ImageContainer = new() { Children = { CreatedImage }, Width = Size };

                AssignedTagsProjectionPredicate = (TagMatch) => false;
                return new RichTextGenerationInstrumentary.Specific.PreConstructedInlineUIContainer([
                    Context.RichTextGenerationContext.TargetTextBlock.CreateBindedCopy(PropertyExceptions: [TextBlock.BackgroundProperty]),
                    new Grid()
                    {
                        Background = ImageBackground,
                        Children =
                        {
                            ImageContainer
                        }
                    },
                ]);
            }]
        };
        #endregion



        /// <summary>Must be called before language loading and when all <see cref="IntenseStareType1"/> and <see cref="IntenseStareType3"/> was initialized</summary>
        public static void StashDefaultValues()
        {
            foreach (UITranslationEntry TextElement in PresentedTextElements_Interface.Values.Union(PresentedTextFields_Interface.Values))
            {
                TextElement.StashDefaultValues();
            }
        }
        public static void ApplyDefaultValues()
        {
            foreach (UITranslationEntry TextElement in PresentedTextElements_Interface.Values.Union(PresentedTextFields_Interface.Values))
            {
                TextElement.ApplyDefaultValues();
            }
        }





        public ref struct @JsonClasses
        {
            public static List<PropertyInfo> ModifierProperties = 
                [.. typeof(@JsonClasses.UIModifyingParameters.UIModifier)
                    .GetProperties()
                    .Where(Property => !Property.Name.EqualsToOneOf(
                        nameof(UIModifier.Text), nameof(UIModifier.UID), nameof(UIModifier.Font)
                        // Text set is a bit more complicated, UID should not be overwritten, FontFamily is also more complicated
                        // See ModifySingleObject()
                    ) & Property.GetGetMethod() is not null)];

            public record UIModifyingParameters
            {
                [JsonProperty("Font Defaults")]
                public FontDefaultSettings FontDefaults { get; set; } = new();
                public record FontDefaultSettings
                {
                    [JsonProperty("Font")]
                    public string Font { get; set; } = "";


                    [JsonProperty("Font Weight")]
                    public string? FontWeight_StringValue { set => FontWeight = WeightFrom(value); }

                    [JsonIgnore]
                    public FontWeight FontWeight { get; set; }


                    [JsonProperty("Font Stretch")]
                    internal string? FontStretch_StringValue { set => FontStretch = FontStretchFrom(value); }

                    [JsonIgnore]
                    public FontStretch FontStretch { get; set; }
                }

                [JsonProperty("Readme")]
                public List<string> Readme { get; set; } = [];


                [JsonProperty("List<Translation>")] // Static UI Text.json | Dynamic UI Text.json
                public List<List<UIModifier>> List { get; set; } = [];

                [JsonProperty("List<Parametre>")]   // Textfield Parameters.json
                public List<List<UIModifier>> List_SecondName { set => List = value; }


                public record UIModifier
                {
                    [JsonProperty("UID")]
                    public Dictionary<string, dynamic> UID_Definer { set => UID = value.First().Key; } // "UID": {"[C] * [Section Title] Decorative cautions":null},

                    [JsonIgnore]
                    public string UID { get; set; } = "";


                    [JsonProperty("Text")]
                    public string Text { get; set; } = "";

                    [JsonIgnore]
                    public string Text_BeforeThemeKeys { get; set; } = "";

                    [JsonProperty("Variable")]
                    public Dictionary<string, string>? Variable { get; set; }

                    [JsonIgnore]
                    public Dictionary<string, string>? Variable_BeforeThemeKeys { get; set; }

                    [JsonProperty("Text Alignment")]
                    public TextAlignment? TextAlignment { get; set; }


                    [JsonProperty("Font")]
                    public string? Font { get; set; }


                    [JsonProperty("Font Size")]
                    public double? FontSize { get; set; }


                    [JsonProperty("Font Weight")]
                    public string? FontWeight_StringValue { set => FontWeight = WeightFrom(value); }

                    [JsonIgnore]
                    public FontWeight? FontWeight { get; set; }


                    [JsonProperty("Font Stretch")]
                    public string? FontStretch_StringValue { set => FontStretch = FontStretchFrom(value); }

                    [JsonIgnore]
                    public FontStretch? FontStretch { get; set; }


                    [JsonProperty("Margin")]
                    public double[]? Margin_DoubleValues { set => Margin = ThicknessFrom(value); }

                    [JsonIgnore]
                    public Thickness? Margin { get; set; }


                    [JsonProperty("Width")]
                    public double? Width { get; set; }

                    [JsonProperty("Height")]
                    public double? Height { get; set; }


                    [JsonProperty("Visible")]
                    public bool? Visible { get; set; }


                    [OnDeserialized]
                    public void OnDeserialized(StreamingContext Context) => ApplyThemeColorKeys();

                    public void ApplyThemeColorKeys()
                    {
                        try
                        {
                            Text_BeforeThemeKeys = Text;
                            Text = @Themes.CurrentTheme.ColorKeysForTranslation.Apply(Text, RelatedUITranslationEntryUID: this.UID);

                            if (Variable is not null)
                            {
                                Variable_BeforeThemeKeys = Variable.ToDictionarySafe(x => x.Key, x => x.Value); // Clone

                                Variable = Variable.ToDictionarySafe
                                (
                                    KeySelector: x => x.Key,
                                    ValueSelector: x => @Themes.CurrentTheme.ColorKeysForTranslation.Apply(x.Value, RelatedUITranslationEntryUID: this.UID)
                                );
                            }
                        }
                        catch (Exception Occurred)
                        {
                            ErrorMessageWindow.ShowException(Occurred);
                        }
                    }
                }
            }

            public record UnsavedChangesInfoText
            {
                [JsonProperty("Passives")]
                public UnsavedChangesInfo_Passives Passives { get; set; } = new();
                public record UnsavedChangesInfo_Passives
                {
                    [JsonProperty("ID Header")]
                    public string IDHeader { get; set; } = "\n\n<b>ID</b> <color=#f8c200>[$1]</color> 「<color=#afbff9>[$2]</color>」";

                    [JsonProperty("Name")]
                    public string Name { get; set; } = "\n  > Name";

                    [JsonProperty("Main Desc")]
                    public string MainDesc { get; set; } = "\n  > Main description";

                    [JsonProperty("Summary Desc")]
                    public string SummaryDesc { get; set; } = "\n  > Summary description";

                    [JsonProperty("Flavor Desc")]
                    public string FlavorDesc { get; set; } = "\n  > Flavor description";
                }


                [JsonProperty("Keywords")]
                public UnsavedChangesInfo_Keywords Keywords { get; set; } = new();
                public record UnsavedChangesInfo_Keywords
                {
                    [JsonProperty("ID Header")]
                    public string IDHeader { get; set; } = "\n\n<b>ID</b> <color=#f8c200>[$1]</color> 「<color=#afbff9>[$2]</color>」";

                    [JsonProperty("Name")]
                    public string Name { get; set; } = "\n  > Name";

                    [JsonProperty("Keyword Color")]
                    public string Color { get; set; } = "\n  > Color";

                    [JsonProperty("Main Desc")]
                    public string MainDesc { get; set; } = "\n  > Main description";

                    [JsonProperty("Summary Desc")]
                    public string SummaryDesc { get; set; } = "\n  > Summary description";

                    [JsonProperty("Flavor Desc")]
                    public string FlavorDesc { get; set; } = "\n  > Flavor description";
                }


                [JsonProperty("E.G.O Gifts")]
                public UnsavedChangesInfo_EGOGifts EGOGifts { get; set; } = new();
                public record UnsavedChangesInfo_EGOGifts
                {
                    [JsonProperty("ID Header")]
                    public string IDHeader { get; set; } = "\n\n<b>ID</b> <color=#f8c200>[$1]</color> 「<color=#afbff9>[$2]</color>」";

                    [JsonProperty("Name")]
                    public string Name { get; set; } = "\n  > Name";

                    [JsonProperty("Main Desc")]
                    public string MainDesc { get; set; } = "\n  > Main description";

                    [JsonProperty("Simple Desc")]
                    public string SimpleDesc { get; set; } = "\n  > Simple description №[$]";

                    [JsonProperty("Flavor Desc")]
                    public string FlavorDesc { get; set; } = "\n  > Flavor description";
                }


                [JsonProperty("Skills")]
                public UnsavedChangesInfo_Skills Skills { get; set; } = new();
                public record UnsavedChangesInfo_Skills
                {
                    [JsonProperty("ID Header")]
                    public string IDHeader { get; set; } = "\n\n<b>ID</b> <color=#f8c200>[$1]</color> 「<color=#afbff9>[$2]</color>」";

                    [JsonProperty("Uptie Level")]
                    public string UptieLevel { get; set; } = "\n  > Uptie level [$]";

                    [JsonProperty("Main Name")]
                    public string MainName { get; set; } = "\n    > Name";

                    [JsonProperty("E.G.O Abnormality Name")]
                    public string EGOAbnormalityName { get; set; } = "\n  > E.G.O Abnormality";

                    [JsonProperty("Main Desc")]
                    public string MainDesc { get; set; } = "\n  > Main description";

                    [JsonProperty("Flavor Desc")]
                    public string FlavorDesc { get; set; } = "\n  > Flavor description";

                    [JsonProperty("Coin")]
                    public string Coin { get; set; } = "\n  > Coin №[$]";
                }


                [JsonProperty("Observation Logs")]
                public UnsavedChangesInfo_ObservationLogs ObservationLogs { get; set; } = new();
                public record UnsavedChangesInfo_ObservationLogs
                {
                    [JsonProperty("ID Header")] // [$3] is abnormality Code Name (F-04-03-04, GU-08-03, ...)
                    public string IDHeader { get; set; } = "\n\n<b>ID</b> <color={Theme.UnsavedChanges.IDColor}>[$1]</color> 「<color={Theme.UnsavedChanges.NameColor}>[$2] ([$3])</color>」";

                    [JsonProperty("Code Name")]
                    public string CodeName { get; set; } = "\n  > Code name";

                    [JsonProperty("Name")]
                    public string Name { get; set; } = "\n  > Name";

                    [JsonProperty("Description")]
                    public string Description { get; set; } = "\n  > Description";

                    [JsonProperty("Lacking Data")]
                    public string LackingData { get; set; } = "\n  > Lacking Data";

                    [JsonProperty("Observation Level")]
                    public string ObservationLevel { get; set; } = "\n  > Observation Level №[$]";
                }
            }
            
            public record StartupSteps
            {
                [JsonProperty("Main Stages")]
                public MainStages_PROP MainStages { get; set; } = new();
                public record MainStages_PROP
                {
                    [JsonProperty("Windows Initializing")]
                    public string WindowsInitializing { get; set; } = "Initializing windows ([$1]/[$2])";

                    [JsonProperty("Configuration")]
                    public string Configuration { get; set; } = "Loading configuration";

                    [JsonProperty("Limbus Company part")]
                    public string LimbusCompanyPart { get; set; } = "Loading Limbus interior";

                    [JsonProperty("Final")]
                    public string Final { get; set; } = "Fade-in animation engage";
                }

                [JsonProperty("Sub Stages")]
                public SubStages_PROP SubStages { get; set; } = new();
                public record SubStages_PROP
                {
                    [JsonProperty("Theme")]
                    public string Theme { get; set; } = "Theme";

                    [JsonProperty("Language")]
                    public string Language { get; set; } = "Language";

                    [JsonProperty("Skills Data")]
                    public string SkillsData { get; set; } = "Skills Data";

                    [JsonProperty("Composite Fonts")]
                    public string CompositeFonts { get; set; } = "Composite Fonts";

                    [JsonProperty("Limbus Fallback keywords")]
                    public string LimbusFallbackKeywords { get; set; } = "Fallback keywords";

                    [JsonProperty("Limbus Custom Language")]
                    public string LimbusCustomLangKeywords { get; set; } = "Limbus Custom Language";

                    [JsonProperty("Limbus Additional keywords")]
                    public string LimbusAdditionalKeywords { get; set; } = "Additional keywords";

                    [JsonProperty("Text editor syntaxes")]
                    public string TextEditorSyntaxes { get; set; } = "Generating text editor syntaxes";
                }
            }
        }





        private static FileEventsNotifier LanguageDirectoryWatcher { get; } = new(
            TargetDirectory: AppDomain.CurrentDomain.BaseDirectory, // Dummy dir to not crash XAML Designer
            FileFilters: ["@ Font References.json", "Dynamic UI Text.json", "Static UI Text.json", "Textfield Parameters.json", "Unsaved Changes.json", "Logo.png"]
        ) {
            GeneralHandler = (_, _, _) =>
            {
                if (ProgramFullyLoaded) @Languages.ModifyUI(new DirectoryInfo(LoadedConfiguration.Internal.UILanguage));
            }
        };

        /// <summary>
        /// <paramref name="LocalizationInfoPath"/> is folder from <c>[⇲] Assets Directory\※ Internal\Translation</c>
        /// </summary>
        public static void ModifyUI(DirectoryInfo LocalizationInfoPath)
        {
            if (LocalizationInfoPath.Exists)
            {
                LanguageDirectoryWatcher.Path = LocalizationInfoPath.FullName;

                Dictionary<string, FileInfo> CurrentLocalizationFilesPreset = LocalizationInfoPath.GetFiles("*.json")
                    .ToDictionary(keySelector: JsonFile => JsonFile.Name, elementSelector: JsonFile => JsonFile);

                @LoadedStaticTextModifiers.Clear();
                @LoadedStaticTextModifiersWithThemeKeys.Clear();
                @LoadedFontReferences.Clear();

                ApplyDefaultValues();


                MainWindowInstance.MainMenuLogoImage.Source = File.Exists(@$"{LocalizationInfoPath}\Logo.png")
                    ?  BitmapFromFile(@$"{LocalizationInfoPath}\Logo.png")
                    : BitmapFromResource(@$"UI\Logo.png");


                if (IsFileValid("@ Font References.json", out Dictionary<string, string> FontReferencesDictionary))
                {
                    foreach (KeyValuePair<string, string> FontReference in FontReferencesDictionary)
                    {
                        string FontReferenceKey = FontReference.Key;
                        string FontReferencePath = FontReference.Value;

                        try
                        {
                            @LoadedFontReferences[FontReferenceKey] = FontFamilyFromFileOrName(FontReferencePath);
                        }
                        catch (Exception Occurred)
                        {
                            ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to read font references file <b>\"{LocalizationInfoPath.FullName}\\@ Font References.json\"</b>");
                        }
                    }
                }


                if (IsFileValid("Dynamic UI Text.json", out @JsonClasses.UIModifyingParameters DynamicUITextParameters))
                {
                    Dictionary<string, @JsonClasses.UIModifyingParameters.UIModifier> DynamicModifiers = DynamicUITextParameters.List
                        .SelectMany(section => section)
                        .ToDictionarySafe(Param => Param.UID, Param => Param);


                    VariableData.UnsavedChangesMarkerContainer = DynamicModifiers.TryGetValue("[Main UI] [#] Unsaved changes marker", out var UnsavedChangesMarkerInfo)
                        ? UnsavedChangesMarkerInfo.Text
                        : $"[$] <size=66%><color={@Themes.CurrentTheme.UIText.UnsavedChangesMarkerColor}><b>(Changed)</b></color></size>";

                    VariableData.InsertionsDefaultValue = DynamicModifiers.TryGetValue("[Main UI] [#] Default insertion value", out var DefaultInsertionValueInfo)
                        ? DefaultInsertionValueInfo.Text
                        : "…";


                    // #warning And if current mode is main menu !!!!!!!!!!!!!!!!!! !! !! !! ! !
                    if (@EditorModesShelf.CurrentEditorMode == @EditorModesShelf.MainMenu)
                    {
                        DataContextDomain.Editor.CurrentObjectID = VariableData.InsertionsDefaultValue;
                    }
                }

                if (IsFileValid("Static UI Text.json", out @JsonClasses.UIModifyingParameters StaticUITextParameters))
                {
                    @LoadedStaticTextModifiers_FontDefaults = StaticUITextParameters.FontDefaults;

                    ProcessModifierParametersList(StaticUITextParameters, PresentedTextElements_Interface);

                    @LoadedStaticTextModifiers = StaticUITextParameters.List
                        .SelectMany(Section => Section)
                        .ToDictionarySafe(Param => Param.UID, Param => Param);
                }
            
                if (IsFileValid("Textfield Parameters.json", out @JsonClasses.UIModifyingParameters TextfieldParameters))
                    ProcessModifierParametersList(TextfieldParameters, PresentedTextFields_Interface);


                VariableData.UnsavedChangesInfo = IsFileValid("Unsaved Changes.json", out @JsonClasses.UnsavedChangesInfoText UnsavedChangesTextInfo)
                    ? UnsavedChangesTextInfo
                    : new();



                bool IsFileValid<DeserializationTargetType>(string Name, out DeserializationTargetType Success)
                {
                    if (CurrentLocalizationFilesPreset.TryGetValue(Name, out FileInfo? Found))
                    {
                        if (Found.TryDeserealizeJsonAs(out DeserializationTargetType Deserialized, out Exception Occurred))
                        {
                            Success = Deserialized;
                            return true;
                        }
                        else
                        {
                            ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to read file <b>\"{Name}\"</b> from the selected interface translation");
                            Success = default!;
                            return false;
                        }
                    }
                    else
                    {
                        Success = default!;
                        return false;
                    }
                }
            }
        }





        private static void ProcessModifierParametersList(@JsonClasses.UIModifyingParameters UIModifyingParameters, Dictionary<string, UITranslationEntry> RelatedUIObjectsDictionary)
        {
            foreach (List<@JsonClasses.UIModifyingParameters.UIModifier> ParametersSection in UIModifyingParameters.List)
            {
                foreach (@JsonClasses.UIModifyingParameters.UIModifier Parameters in ParametersSection)
                {
                    if (RelatedUIObjectsDictionary.TryGetValue(Parameters.UID, out UITranslationEntry? TextElement))
                    {
                        ModifySingleObject(TextElement, Parameters, UIModifyingParameters.FontDefaults);
                    }
                }
            }
        }

        private static void ModifySingleObject(UITranslationEntry Target, @JsonClasses.UIModifyingParameters.UIModifier Parameters, @JsonClasses.UIModifyingParameters.FontDefaultSettings FontDefaults)
        {
            // Set font defaults first
            Target.FontFamily = @LoadedFontReferences.TryGetValue(FontDefaults.Font, out FontFamily? DefaultFont) ? DefaultFont : FontFamilyFromFileOrName(FontDefaults.Font);
            Target.FontWeight = FontDefaults.FontWeight;
            Target.FontStretch = FontDefaults.FontStretch;



            // Get dictionary with values of current UIModifier parameters (Keys is property names from `@JsonClasses.UIModifyingParameters.UIModifier`, values is current `Parameters` values)
            Dictionary<string, object?> DictionaryOfValuesToSet = @JsonClasses.ModifierProperties
                .ToDictionary(keySelector: Property => Property.Name, elementSelector: Property => Property.GetValue(obj: Parameters));

            // For each target's property (IntenseStareType1 / IntenseStareType3)
            foreach (PropertyInfo TargetProperty in Target.GetType().GetProperties())
            {
                if (DictionaryOfValuesToSet.TryGetValue(TargetProperty.Name, out object? ValueToSet) && ValueToSet is not null)
                {
                    TargetProperty.SetValue(obj: Target, value: ValueToSet);
                }

                /// Font set based on <see cref="@LoadedFontReferences"/> info, otherwise take from file or by font family name
                else if (Parameters.Font is not null && TargetProperty.Name == nameof(UITranslationEntry.FontFamily))
                {
                    Target.FontFamily = @LoadedFontReferences.TryGetValue(Parameters.Font, out FontFamily? FoundReferencedFont)
                        ? FoundReferencedFont
                        : FontFamilyFromFileOrName(Parameters.Font);
                }
            }


            // Visibility for context menu items
            if (Parameters.Visible is not null & Target.UID.StartsWith("[Context Menu] * "))
            {
                ((Target as IntenseStareType1)!.Parent as MenuItem_T1)!.Visibility = (bool)Parameters.Visible! ? Visibility.Visible : Visibility.Collapsed;
            }


            // Text set if Target is IntenseStareType1 (Not textfield)
            if (Target.UID.Contains("[-]") == false && Target is IntenseStareType1 Rose/* && .......................*/)
            {
                string TextToSet = "";
                
                if (Parameters.Variable?.Keys.Count > 0)
                {
                    // Keep current if set
                    TextToSet = Rose.CurrentVariableKey is not null
                        ? Parameters.Variable[Rose.CurrentVariableKey]
                        : Parameters.Variable.First().Value;
                }
                else if (Parameters.Text is not null)
                {
                    TextToSet = Parameters.Text;
                }


                // Insertions (Keep current if set)
                if (TextToSet.Contains("[$]"))
                {
                    TextToSet = TextToSet.Extern(Rose.CurrentExtern ?? VariableData.InsertionsDefaultValue);
                }
                else if (TextToSet.Contains("[$1]"))
                {
                    TextToSet = TextToSet.Exform(Rose.CurrentExform ?? [.. Enumerable.Repeat(VariableData.InsertionsDefaultValue, 10)]);
                }


                // Rich text or regular text
                if (Parameters.UID.Contains("[!]")) Rose.Text = TextToSet;
                else Rose.RichText = TextToSet;
            }
        }
        
        public static void ReModifyTextElementsWithThemeKeysApplied()
        {
            foreach (string AffectedElementUID in @LoadedStaticTextModifiersWithThemeKeys)
            {
                if (@PresentedTextElements.TryGetValue(AffectedElementUID, out IntenseStareType1? TextElement) &&
                    @LoadedStaticTextModifiers.TryGetValue(AffectedElementUID, out UIModifier? FoundModifier)
                ) {
                    FoundModifier.Text = FoundModifier.Text_BeforeThemeKeys;
                    if (FoundModifier.Variable is not null) FoundModifier.Variable = FoundModifier.Variable_BeforeThemeKeys;

                    FoundModifier.ApplyThemeColorKeys();

                    ModifySingleObject(TextElement, FoundModifier, @LoadedStaticTextModifiers_FontDefaults);
                }
            }
        }
    }
}