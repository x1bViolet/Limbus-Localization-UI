namespace LCLocalizationInterface.LimbusRegistry.AssetsDirectoryLoaders
{
    public static class ColorDictionaries
    {
        public static LimbusColorsDictionary LoadedKeywordColors { get; } = new
        (
            SourceColorsDictionaryFile: @"[⇲] Assets Directory\Color Dictionaries\Keyword Colors.cd.txt",
            FallbackColor: "#9f6a3a" // Brown
        );

        public static LimbusColorsDictionary LoadedSkillTagColors { get; } = new
        (
            SourceColorsDictionaryFile: @"[⇲] Assets Directory\Color Dictionaries\SkillTag Colors.cd.txt",
            FallbackColor: "#93f03f" // Green
        );


        public class LimbusColorsDictionary
        {
            /// <summary>
            /// Intended for colors loaded from specified '.cd.txt' file
            /// </summary>
            public ReadOnlyDictionary<string, string> GeneralColors { get; private set; } = ReadOnlyDictionary<string, string>.Empty;

            /// <summary>
            /// Intended for colors loaded externally from <see cref="JsonTypes.PlainKeyword.Color"/> or <see cref="JsonTypes.PlainSkillTag.Color"/> properties
            /// <br/><br/>
            /// Has higher priority when returning colors
            /// </summary>
            public Dictionary<string, string> ExternalColors { get; } = [];



            public string FallbackColor { get; }
            public string this[string Key]
            {
                get => ExternalColors.TryGetValue(Key, out string? Color_FromLocalizeFile)
                    ? Color_FromLocalizeFile
                    : GeneralColors.TryGetValue(Key, out string? Color_FromDictopnaryFile)
                        ? Color_FromDictopnaryFile
                        : FallbackColor;
            }


            public FileEventsNotifier ColorsDictionaryFileWatcher { get; }
            public LimbusColorsDictionary(string SourceColorsDictionaryFile, string FallbackColor = "#9f6a3a")
            {
                this.FallbackColor = FallbackColor;
                
                ColorsDictionaryFileWatcher = new FileEventsNotifier(TargetFile: SourceColorsDictionaryFile)
                {
                    GeneralHandler = (_, _, _) =>
                    {
                        ReadColorsDictionaryFile(SourceColorsDictionaryFile);
                        @PartialStateUpdater.Limbus.FullyRefreshShownRichText();
                        JsonTextEditor.@LimbusTextSyntaxesPreset.GenerateSyntaxes();
                    }
                };

                ReadColorsDictionaryFile(SourceColorsDictionaryFile);
            }



            private void ReadColorsDictionaryFile(string FilePath)
            {
                if (File.Exists(FilePath))
                {
                    try
                    {
                        List<string> Lines = StreamReadLines(FilePath);

                        Dictionary<string, string> ReadedColorsPairs = [];

                        foreach (string Line in Lines.Where(Line => Line.Contains(" ¤ ")))
                        {
                            string[] ColorPair = Line.Split(" ¤ ");
                            if (ColorPair.Length == 2)
                            {
                                string ID = ColorPair[0].Trim();
                                string Color = ColorPair[1].Trim();

                                ReadedColorsPairs[ID] = Color;
                            }
                        }

                        this.GeneralColors = new ReadOnlyDictionary<string, string>(ReadedColorsPairs);
                    }
                    catch (Exception Occurred)
                    {
                        HandleError(Occurred);
                    }
                }
                else
                {
                    HandleError(new FileNotFoundException("File not found"));
                }


                void HandleError(Exception Occurred)
                {
                    this.GeneralColors = ReadOnlyDictionary<string, string>.Empty;
                    ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to read the colors dictionary \"{FilePath}\"");
                }
            }
        }
    }
}