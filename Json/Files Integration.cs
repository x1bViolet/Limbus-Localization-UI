using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;

namespace LC_Localization_Task_Absolute.Json
{
    #pragma warning disable SYSLIB0050 // Idk why not
    public static partial class FilesIntegration
    {
        public static string SerializeFormatted(this object Source, bool EnableNull = false, string Context = "", bool RemoveCR = false)
        {
            string Output = JsonConvert.SerializeObject(
                value: Source,
                formatting: Formatting.Indented,
                settings: new JsonSerializerSettings { NullValueHandling = EnableNull ? NullValueHandling.Include : NullValueHandling.Ignore, Context = new StreamingContext(StreamingContextStates.Other, Context) }
            );
            if (RemoveCR) Output = Output.Replace("\r\n", "\n");
            return Output;
        }

        public static void SerializeFormattedFile(this object Target, string Filename, bool EnableNull = false, string Context = "")
        {
            string Output = JsonConvert.SerializeObject(
                value: Target,
                formatting: Formatting.Indented,
                settings: new JsonSerializerSettings { NullValueHandling = EnableNull ? NullValueHandling.Include : NullValueHandling.Ignore, Context = new StreamingContext(StreamingContextStates.Other, Context) }
            );

            File.WriteAllText(Filename, Output.Replace("\r\n", "\n"), MainWindow.CurrentFileEncoding);
        }

        public static OutputType? TranzitConvert<OutputType>(this object Target) => JsonConvert.DeserializeObject<OutputType>(JsonConvert.SerializeObject(Target));

        public static TargetType? Deserealize<TargetType>(this FileInfo Target, string Context = "") where TargetType : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<TargetType>(Target.GetText(), settings: new JsonSerializerSettings() { Context = new StreamingContext(StreamingContextStates.Other, Context) });
            }
            catch { return new TargetType(); }
        }
    }
}