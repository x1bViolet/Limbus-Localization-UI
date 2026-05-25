using static LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing.Modules.Main;

namespace LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing
{
    namespace Modules
    {
        public static class Shorthands
        {
            public static string ConvertShorthands(string JsonText, string ShorthandsPattern)
            {
                JsonText = Regex.Replace(JsonText, ShorthandsPattern, Match =>
                {
                    if (Match.Groups["ID"].Value != "")
                    {
                        string ID = Match.Groups["ID"].Value;
                        string Name = Match.Groups["Name"].Value;
                        string Color = Match.Groups["Color"].Value;
                        string SpriteID = Match.Groups["SpriteID"].Value;

                        string FullKeywordSpelling =
                        @$"<sprite name=\""{(SpriteID != "" ? SpriteID : ID)}\"">" +
                        @$"<color={(Color != "" ? Color : ColorDictionaries.KeywordColors[ID])}>" +
                        @$"<u>" +
                        @$"<link=\""{ID}\"">" +
                        Name +
                        @$"</link>" +
                        @$"</u>" +
                        @$"</color>";

                        return FullKeywordSpelling;
                    }
                    else
                    {
                        return Match.Value;
                    }
                }, RegexOptions.Singleline);

                return JsonText;
            }
        }
    }
}