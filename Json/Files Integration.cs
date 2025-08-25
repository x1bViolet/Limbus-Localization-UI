using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;

namespace LC_Localization_Task_Absolute.Json
{
    internal static partial class FilesIntegration
    {
        internal static void SerializeFormatted(this object Target, string Filename, bool EnableNull = false, string Context = "")
        {
            string Output = JsonConvert.SerializeObject(
                value: Target,
                formatting: Formatting.Indented,
                settings: new JsonSerializerSettings { NullValueHandling = EnableNull ? NullValueHandling.Include : NullValueHandling.Ignore, Context = new StreamingContext(StreamingContextStates.Other, Context) }
            );

            File.WriteAllText(Filename, Output.Replace("\r\n", "\n"), MainWindow.CurrentFileEncoding);
        }

        internal static dynamic? TranzitConvert<OutputType>(this object Target) => JsonConvert.DeserializeObject<OutputType>(JsonConvert.SerializeObject(Target));

        internal static dynamic? Deserealize<TargetType>(this FileInfo Target, string Context = "") => JsonConvert.DeserializeObject<TargetType>(Target.GetText(), settings: new JsonSerializerSettings() { Context = new StreamingContext(StreamingContextStates.Other, Context) });
    }
}