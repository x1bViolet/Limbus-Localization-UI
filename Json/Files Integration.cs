using Newtonsoft.Json;
using System.IO;

namespace LC_Localization_Task_Absolute.Json
{
    internal static partial class FilesIntegration
    {
        internal static void SerializeFormatted(this object Target, string Filename)
        {
            string Output = JsonConvert.SerializeObject(
                value:      Target,
                formatting: Formatting.Indented,
                settings:   new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
            );
            File.WriteAllText(Filename, Output.Replace("\r\n", "\n"), MainWindow.CurrentFileEncoding);
        }

        internal static dynamic? Deserealize<TargetType>(this FileInfo Target) => JsonConvert.DeserializeObject<TargetType>(Target.GetText());
    }
}