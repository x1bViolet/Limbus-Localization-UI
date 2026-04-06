using Newtonsoft.Json.Converters;
using System.Diagnostics.CodeAnalysis;

namespace LCLocalizationInterface.Instruments
{
    public static partial class Json
    {
        #region Serialization

        #pragma warning disable SYSLIB0050 // Idk why not
        public static string SerializeToFormattedJsonText(this object Target, string Context = "", int IndentationSize = 2, LineBreakMode LineBreakMode = LineBreakMode.LF)
        {
            using (StringWriter StringWriter = new())
            {
                using (JsonTextWriter JsonWriter = new(textWriter: StringWriter))
                {
                    JsonWriter.Formatting = Formatting.Indented;
                    JsonWriter.Indentation = IndentationSize;

                    JsonSerializer Serializer = new()
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Context = new StreamingContext(StreamingContextStates.Other, Context),
                        Converters = { new StringEnumConverter() },
                    };

                    Serializer.Serialize(JsonWriter, Target);
                }

                string Serialized = StringWriter.ToString();
                if (LineBreakMode == LineBreakMode.LF) Serialized = Serialized.Replace("\r\n", "\n");
                if (LineBreakMode == LineBreakMode.CR) Serialized = Serialized.Replace("\r\n", "\r");

                return Serialized;
            }
        }


        public static void SerializeToFormattedJsonFile(this object Source, string Path, string Context = "", int IndentationSize = 2, LineBreakMode LineBreakMode = LineBreakMode.CRLF, bool IsBOM = false)
        {
            UTF8Encoding Encoding = new(encoderShouldEmitUTF8Identifier: IsBOM);

            string Serialized = Source.SerializeToFormattedJsonText(Context: Context, IndentationSize: IndentationSize, LineBreakMode: LineBreakMode);
            File.WriteAllText(Path, Serialized, Encoding);
        }










        #region (FileInfo|String).(Deserialize|TryDeserialize)
        public static TargetType? DeserealizeJsonAs<TargetType>(this string Text, string Context = "")
        {
            return JsonConvert.DeserializeObject<TargetType>(Text, settings: new JsonSerializerSettings()
            {
                Context = new StreamingContext(StreamingContextStates.Other, Context),
                Converters = { new StringEnumConverter() },
                NullValueHandling = NullValueHandling.Ignore,
            });
        }
        public static bool TryDeserealizeJsonAs<TargetType>(this string Target, [NotNullWhen(true)] out TargetType Deserialized, [NotNullWhen(false)] out Exception OccuredException, string Context = "")
        {
            try
            {
                Deserialized = Target.DeserealizeJsonAs<TargetType>(Context)!;
                OccuredException = Deserialized is null ? new Exception("Deserialized json is null") : null!;
                return Deserialized is not null;
            }
            catch (Exception Occurred)
            {
                Deserialized = default!;
                OccuredException = Occurred;
                return false;
            }
        }


        public static TargetType? DeserealizeJsonAs<TargetType>(this FileInfo Target, string Context = "")
        {
            return StreamReadText(Target.FullName).DeserealizeJsonAs<TargetType>(Context);
        }

        /// <summary>
        /// <paramref name="OccuredException"/> contains only message, not stacktrace, because it anyway leads to the path of TryDeserialize methods
        /// </summary>
        public static bool TryDeserealizeJsonAs<TargetType>(this FileInfo Target, [NotNullWhen(true)] out TargetType Deserialized, [NotNullWhen(false)] out Exception OccuredException, string Context = "")
        {
            try
            {
                Deserialized = Target.DeserealizeJsonAs<TargetType>(Context)!;
                OccuredException = Deserialized is null ? new Exception("Deserialized json is null") : null!;
                return Deserialized is not null;
            }
            catch (Exception ActuallyOccurredException)
            {
                Deserialized = default!;
                OccuredException = (Exception)Activator.CreateInstance(type: ActuallyOccurredException.GetType(), args: [ActuallyOccurredException.Message])!;
                return false;
            }
        }
        #endregion

        public static TObject JsonClone<TObject>(this TObject Target) => JsonConvert.DeserializeObject<TObject>(JsonConvert.SerializeObject(Target))!;

        #endregion





        #region Specific json things
        public enum LineBreakMode { LF, CR, CRLF }
        public static string ToActualString(this LineBreakMode LineBreakMode, string Fallback = "\r\n")
        {
            return LineBreakMode switch { LineBreakMode.LF => "\n", LineBreakMode.CR => "\r", LineBreakMode.CRLF => "\r\n", _ => Fallback };
        }

        public static LineBreakMode DetermineLineBreakType(this string Text, LineBreakMode Fallback = LineBreakMode.CRLF)
        {
            if (Text.Contains("\r\n")) return LineBreakMode.CRLF;
            else if (Text.Contains('\n')) return LineBreakMode.LF;
            else if (Text.Contains('\r')) return LineBreakMode.CR;
            else return Fallback;
        }

        public static int GetJsonIndentationSize(this string JsonText, int FailedMatchFallback = 2)
        {
            Match IndentationMatch = Regex.Match(JsonText.Trim(), @"^{(\r)?\n(?<Indentation> +)""");
            return IndentationMatch.Success ? IndentationMatch.Groups["Indentation"].Length : FailedMatchFallback;
        }
        #endregion
    }
}