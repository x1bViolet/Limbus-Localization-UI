namespace LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing
{
    namespace Modules
    {
        public static class StylePlaceholders
        {
            public static string AddStyleHighlightPlaceholders(string SkillsJsonText)
            {
                int Replacements = 0;
                JToken JParser = JToken.Parse(SkillsJsonText);
                foreach (JToken StringItem in JParser.SelectTokens("$.dataList[*].levelList[*].coinlist[*].coindescs[*].desc"))
                {
                    if ($"{StringItem}".Contains("<style=\"highlight\">") == false & $"{StringItem}" != "")
                    {
                        StringItem.Replace($"{StringItem}<style=\"highlight\"></style>");
                        Replacements++;
                    }
                }
                foreach (JToken StringItem in JParser.SelectTokens("$.dataList[*].levelList[*].desc"))
                {
                    if ($"{StringItem}".Contains("<style=\"highlight\">") == false & $"{StringItem}" != "")
                    {
                        StringItem.Replace($"{StringItem}<style=\"highlight\"></style>");
                        Replacements++;
                    }
                }

                if (Replacements > 0)
                {
                    return JParser.ToString(Formatting.Indented);
                }
                else
                {
                    return SkillsJsonText;
                }
            }
        }
    }
}