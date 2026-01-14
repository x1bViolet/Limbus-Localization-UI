using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace LC_Localization_Task_Absolute.Json
{
    #pragma warning disable SYSLIB0050 // Idk why not
    public static class JsonSerialization
    {
        public static string SerializeToFormattedText(this object Target, string Context = "", int IndentationSize = 2, LineBreakMode LineBreakMode = LineBreakMode.LF)
        {
            using (StringWriter StringWriter = new StringWriter())
            {
                using (JsonTextWriter JsonWriter = new JsonTextWriter(StringWriter))
                {
                    JsonWriter.Formatting = Formatting.Indented;
                    JsonWriter.Indentation = IndentationSize;

                    JsonSerializer Serializer = new()
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Context = new StreamingContext(StreamingContextStates.Other, Context),
                        Converters = { new StringEnumConverter() }
                    };

                    Serializer.Serialize(JsonWriter, Target);
                }

                string Serialized = StringWriter.ToString();
                if (LineBreakMode == LineBreakMode.LF) Serialized = Serialized.Replace("\r\n", "\n");
                if (LineBreakMode == LineBreakMode.CR) Serialized = Serialized.Replace("\r\n", "\r");

                return Serialized;
            }
        }

        public static void SerializeToFormattedFile_Regular(this object Source, string Filename, string Context = "", int IndentationSize = 2, LineBreakMode LineBreakMode = LineBreakMode.CRLF)
        {
            string Serialized = Source.SerializeToFormattedText(Context: Context,  IndentationSize: IndentationSize,  LineBreakMode: LineBreakMode);
            File.WriteAllText(Filename, Serialized);
        }
        public static void SerializeToFormattedFile_CurrentLimbusJson(this object Source, string Filename)
        {
            string Serialized = Source.SerializeToFormattedText(IndentationSize: MainWindow.CurrentIndentationSize,  LineBreakMode: MainWindow.CurrentLineBreakMode);
            File.WriteAllText(Filename, Serialized, MainWindow.CurrentFileEncoding);
        }

        public static TargetType? Deserealize<TargetType>(this FileInfo Target, string Context = "")
        {
            return JsonConvert.DeserializeObject<TargetType>(Target.GetText(), settings: new JsonSerializerSettings()
            { 
               Context = new StreamingContext(StreamingContextStates.Other, Context),
               Converters = { new StringEnumConverter() },
               NullValueHandling = NullValueHandling.Ignore, // Can be suspecious
            });
        }


        public static OutputType? TranzitConvert<OutputType>(this object Target) => JsonConvert.DeserializeObject<OutputType>(JsonConvert.SerializeObject(Target));

        public enum LineBreakMode { LF, CR, CRLF, KeepOriginal /* Needed locally by configuration profiles */ }
        public static string ToActualString(this LineBreakMode LineBreakMode)
        {
            return LineBreakMode switch { LineBreakMode.LF => "\n", LineBreakMode.CR => "\r", LineBreakMode.CRLF => "\r\n" };
        }

        public static LineBreakMode DetermineLineBreakType(this string Text, LineBreakMode Fallback = LineBreakMode.CRLF)
        {
            if (Text.Contains("\r\n"))
            {
                return LineBreakMode.CRLF;
            }
            else if (Text.Contains('\n'))
            {
                return LineBreakMode.LF;
            }
            else if (Text.Contains('\r'))
            {
                return LineBreakMode.CR;
            }
            else
            {
                return Fallback;
            }
        }

        public static int GetJsonIndentationSize(this string JsonText, int FailedMatchFallback = 2)
        {
            Match IndentationMatch = Regex.Match(JsonText.Trim(), @"^{(\r)?\n(?<Indentation> +)""");
            return IndentationMatch.Success ? IndentationMatch.Groups["Indentation"].Length : FailedMatchFallback;
        }
    }
}