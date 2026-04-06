using ICSharpCode.AvalonEdit.Document;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using System.Globalization;
using static LCLocalizationInterface.LimbusRegistry.InputRichTextFormatter;

namespace LCLocalizationInterface.LimbusRegistry.LimbusIValueConverters
{
    /// <summary>
    /// To use in a RichText Bindings for names with applied <see cref="LimbusEditorJsonObject.EscapeLineBreaks"/>
    /// </summary>
    public class EscapedLineBreaksConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture) => LimbusEditorJsonObject.UnescapeLineBreaks(Value as string)!;
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture) => DependencyProperty.UnsetValue;
    }
}

namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    /// <summary>
    /// Base class for all localization json objects that interact with the editor. Has <see cref="AdditionalData"/> (<see cref="JsonExtensionDataAttribute"/>) and a set of helper methods to deal with line breaks.
    /// <br/><br/>
    /// Derived from <see cref="Explicit"/> for UI interactions through {Bindings} to <see cref="DataContextDomain"/>.
    /// </summary>
    public abstract record LimbusEditorJsonObject : Explicit
    {
        [JsonExtensionData] // To avoid unknown properties accidental erasing (Yes, "flavor", I'm talking about you)
        public Dictionary<string, JToken> AdditionalData { get; } = [];



        /// <summary>
        /// Create <see cref="TextDocument"/> with <see cref="TextEditorSyntaxKeyCarrier"/> as <see cref="TextDocument.ServiceProvider"/> to automatically set specific syntax to <see cref="MainWindow.LimbusJsonTextEditor"/> via <see cref="JsonTextEditor.Document"/> property with custom <see langword="set"/> that tries to get given <see cref="TextDocument"/>'s ServiceProvider as <see cref="TextEditorSyntaxKeyCarrier"/> and then sets syntax from <see cref="JsonTextEditor.@LimbusTextSyntaxesPreset.GeneratedSyntaxes"/> Dictionary based on acquired <see cref="TextEditorSyntaxKeyCarrier.CarriedSyntaxKey"/><br/><br/>
        /// For names this is not required as they are entered via <see cref="IntenseStareType3"/>s and their synatax is set manually using <see cref="EditorModesShelf.Types.EditorModeAbstraction{LocalizationDataType}.PresentedRightMenuSytaxedTextInputs"/> list
        /// </summary>
        public static TextDocument NewDedicatedDocument(string Text, RichTextFormat CarriedSyntaxKey = RichTextFormat.None)
        {
            return new TextDocument(initialText: Text) { ServiceProvider = new TextEditorSyntaxKeyCarrier(CarriedSyntaxKey) };
        }


        #region Shenanigans with line break type unifying (Needed because AvalonEdit.TextEditor may suddenly start using \r\n line breaks instead of \n for no apparent reason)
        /// <summary>Change line breaks to LF (<see cref="string.ReplaceLineEndings"/>)</summary>
        public static string? AsLF(string? CRLFString) => CRLFString?.ReplaceLineEndings("\n");

        /// <summary>Сompare strings without taking into account the type of line breaks (Both considered as LF via <see cref="string.ReplaceLineEndings"/>)</summary>
        public static bool IsNotEquals(string? A, string? B) => A?.ReplaceLineEndings("\n") != B?.ReplaceLineEndings("\n");
        #endregion


        #region Shenanigans with line breaks input in textboxes at the right menu
        /// <summary>Replace \r and \n with \\r and \\n</summary>
        public static string? EscapeLineBreaks(string? String) => String?.Replace("\r", "\\r").Replace("\n", "\\n");

        /// <summary>Replace \\r and \\n with \r and \n</summary>
        public static string? UnescapeLineBreaks(string? String) => String?.Replace("\\r", "\r").Replace("\\n", "\n");
        #endregion
    }



    /// <summary>
    /// <see cref="IServiceProvider"/>-based class with <see cref="CarriedSyntaxKey"/> property. Needed to embed <see cref="RichTextFormat"/> Key into a <see cref="TextDocument"/> via <see cref="TextDocument.ServiceProvider"/> property to associate the desired type of syntax with it <i>(Key for <see cref="JsonTextEditor.@LimbusTextSyntaxesPreset.GeneratedSyntaxes"/> Dictionary)</i>
    /// <br/><br/>
    /// Can be recieved only by the <see cref="JsonTextEditor.Document"/> override property with custom <see langword="set"/> that tries to get given <see cref="TextDocument"/>'s ServiceProvider as <see cref="TextEditorSyntaxKeyCarrier"/> and then sets syntax from <see cref="JsonTextEditor.@LimbusTextSyntaxesPreset.GeneratedSyntaxes"/> Dictionary based on acquired <see cref="TextEditorSyntaxKeyCarrier.CarriedSyntaxKey"/>
    /// <br/><br/>
    /// (Because <see cref="TextDocument"/> is <see langword="sealed"/> and i can't add custom property for derived class ((Although there is another way via <see cref="TextDocument.FileName"/> with conversion via <see cref="Enum.TryParse{TEnum}(string?, out TEnum)"/> but whatever)))
    /// </summary>
    public class TextEditorSyntaxKeyCarrier(RichTextFormat CarriedSyntaxKey = RichTextFormat.None) : IServiceProvider
    {
        public RichTextFormat CarriedSyntaxKey { get; } = CarriedSyntaxKey;

        #region IServiceProvider interface
        public object? GetService(Type ServiceType) => null;
        #endregion
    }




    /// <summary>
    /// Parent class for limbus localization files:<br/>
    /// • <see cref="DataList"/> for localization objects of type <typeparamref name="LocalizationDataType"/>;<br/>
    /// • <see cref="ManualFileType"/>;<br/>
    /// • <see cref="TryMatchManualFileType"/> to return string that matches the original raw localization file names beginning based on the one defined in <see cref="ManualFileType"/>, which can be understood ("E.G.O Gifts" => "EGOgift", "Observation Logs" => "AbnormalityGuides", "Keywords" => "BattleKeywords", ...). This raw localization file name beginning string is used to match corresponding editor mode by <see cref="@EditorModesShelf.ModesMapping"/> list.
    /// </summary>
    public record LimbusLocalizationFile<LocalizationDataType>
    {
        /// <summary>
        /// Readable version of file type ("E.G.O Gifts", "Keywords (Bufs)", "Observation Logs") instead of original boilerplate file name ("EGOgift", "Bufs", "AbnormalityGuides")
        /// </summary>
        [JsonProperty("Manual File Type")]
        public string? ManualFileType { get; set; }


        [JsonExtensionData] // To avoid unknown properties accidental erasing (Yes, "flavor", I'm talking about you)
        public Dictionary<string, JToken> AdditionalData { get; } = [];


        /// <summary>
        /// List with localization objects of specified type
        /// </summary>
        [JsonProperty("dataList")]
        public List<LocalizationDataType> DataList { get; init; } = [];


        /// <summary>
        /// Convert readable <see cref="ManualFileType"/> to raw localization file name beginning ("E.G.O Gifts" => "EGOgift", "Observation Logs" => "AbnormalityGuides", "Keywords" => "BattleKeywords", ...). This raw localization file name beginning string is used to match corresponding editor mode by the <see cref="@EditorModesShelf.ModesMapping"/> list.
        /// </summary>
        public bool TryMatchManualFileType(out string AcquiredNameStart)
        {
            AcquiredNameStart = this.ManualFileType switch
            {
                "Skills" => "Skills",
                "Passives" => "Passive",
                "Keywords" => "BattleKeywords",
                "Keywords (Bufs)" => "Bufs",
                "Keywords (BattleKeywords)" => "BattleKeywords",
                "E.G.O Gifts" => "EGOgift",
                "Observation Logs" => "AbnormalityGuides",

                #region Older versions compatibility (Now skills editor appearance with the availability of buttons and visual elements is determined by the properties of current skill instead of fixed file name condition)
                "Skills (With upties)" => "Skills",
                "Skills (With upties; With abName)" => "Skills",
                #endregion

                _ => "",
            };

            return AcquiredNameStart != "";
        }
    }



    public interface IHasIdentifier<TIdentifier>
    {
        public TIdentifier ID { get; }
    }
}