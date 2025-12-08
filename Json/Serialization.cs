using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Runtime.Serialization;

namespace LC_Localization_Task_Absolute.Json
{
    #pragma warning disable SYSLIB0050 // Idk why not
    public static class Serialization
    {
        public static string SerializeToFormattedString(this object Source, string Context = "")
        {
            string Output = JsonConvert.SerializeObject(
                value: Source,
                formatting: Formatting.Indented,
                settings: new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Context = new StreamingContext(StreamingContextStates.Other, Context),
                    Converters = { new StringEnumConverter() }
                }
            );
            Output = Output.Replace("\r\n", "\n");

            return Output;
        }

        public static void SerializeToFormattedFile(this object Source, string Filename, string Context = "")
        {
            string Serialized = Source.SerializeToFormattedString(Context);

            File.WriteAllText(Filename, Serialized, MainWindow.CurrentFileEncoding);
        }

        public static OutputType? TranzitConvert<OutputType>(this object Target) => JsonConvert.DeserializeObject<OutputType>(JsonConvert.SerializeObject(Target));

        public static TargetType? Deserealize<TargetType>(this FileInfo Target, string Context = "")
        {
            return JsonConvert.DeserializeObject<TargetType>(Target.GetText(), settings: new JsonSerializerSettings()
            { 
               Context = new StreamingContext(StreamingContextStates.Other, Context),
               Converters = { new StringEnumConverter() },
               NullValueHandling = NullValueHandling.Ignore, // Can be suspecious
            });
        }
    }
}