namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    public abstract class @ColorInfo
    {
        public static readonly Dictionary<string, string> Affinities_GameColors = new Dictionary<string, string>()
        {
            ["Wrath"   ] = "#fe0101",
            ["Lust"    ] = "#fe6f01",
            ["Sloth"   ] = "#edc427",
            ["Gluttony"] = "#85ce04",
            ["Gloom"   ] = "#1cc7f1",
            ["Pride"   ] = "#014fd6",
            ["Envy"    ] = "#9800df",
        };
        public static string GetAffinityColor_GameColors(string AffinityName) => GetColorWithFallback(AffinityName, Affinities_GameColors);
        
        
        public static readonly Dictionary<string, string> Affinities_InfoPreviewColors = new Dictionary<string, string>()
        {
            ["Wrath"   ] = "#ff0000",
            ["Lust"    ] = "#cc5200",
            ["Sloth"   ] = "#c9a022",
            ["Gluttony"] = "#85ce04",
            ["Gloom"   ] = "#119cbe",
            ["Pride"   ] = "#004bd5",
            ["Envy"    ] = "#9500de",
        };
        public static string GetAffinityColor_InfoPreviewColors(string AffinityName) => GetColorWithFallback(AffinityName, Affinities_InfoPreviewColors);

        
        
        public static readonly Dictionary<string, string> SinnerColors = new Dictionary<string, string>()
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
        public static string GetSinnerColor(string SinnerName) => GetColorWithFallback(SinnerName, SinnerColors);




        /// <summary>
        /// Return <see cref="DefaultBrownColor"/> if invalid
        /// </summary>
        private static string GetColorWithFallback(string? Input, Dictionary<string, string> Source)
        {
            return Input != null && Source.ContainsKey(Input)
                ? Source[Input]
                : "#9f6a3a";
        }
    }
}
