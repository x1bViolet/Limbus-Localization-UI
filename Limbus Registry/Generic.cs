namespace LCLocalizationInterface.LimbusRegistry
{
    public static class Generic
    {
        public enum AffinityName { Wrath, Lust, Sloth, Gluttony, Gloom, Pride, Envy, None }
        public enum DamageType { Slash, Pierce, Blunt, None }
        public enum SkillType { Attack, Counter, Guard, Evade }

        public enum LimbusFontTypes { Context, Title, None, }
        public enum LocalizationDirectoryType { Fallback, Custom, Additional }



        public static string GetAffinityColor(string? AffinityName, string Fallback = "#9f6a3a") => GetColorWithFallback(AffinityName, AffinitiyColors, Fallback);
        private static Dictionary<string, string> AffinitiyColors = new()
        {
            ["Wrath"   ] = "#fe0101",
            ["Lust"    ] = "#fe6f01",
            ["Sloth"   ] = "#fed133",
            ["Gluttony"] = "#a5fe06",
            ["Gloom"   ] = "#1cc7f1",
            ["Pride"   ] = "#014fd6",
            ["Envy"    ] = "#9808de",
        };



        public static string GetSinnerColor(string? SinnerName, string Fallback = "#ffffff") => GetColorWithFallback(SinnerName, Sinners, Fallback);
        private static Dictionary<string, string> Sinners = new()
        {
            ["Yi Sang"    ] = "#d4e1e8",
            ["Faust"      ] = "#ffb1b4",
            ["Don Quixote"] = "#ffef23",
            ["Ryōshū"     ] = "#cf0000",
            ["Ryoshu"     ] = "#cf0000",
            ["Meursault"  ] = "#293b95",
            ["Hong Lu"    ] = "#5bffde",
            ["Heathcliff" ] = "#4e3076",
            ["Ishmael"    ] = "#ff9500",
            ["Rodion"     ] = "#820000",
            ["Sinclair"   ] = "#8b9c15",
            ["Outis"      ] = "#325339",
            ["Gregor"     ] = "#69350b",
        };




        private static string GetColorWithFallback(string? Input, Dictionary<string, string> Source, string Fallback = "#9f6a3a")
        {
            return Input is not null && Source.TryGetValue(Input, out string? GettedColor) ? GettedColor : Fallback;
        }
    }
}