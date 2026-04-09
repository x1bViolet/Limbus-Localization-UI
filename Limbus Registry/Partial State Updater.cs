using LCLocalizationInterface.Instruments.Classes;
using LCLocalizationInterface.LimbusRegistry.PreviewCreator;

namespace LCLocalizationInterface.LimbusRegistry
{
    public ref struct @PartialStateUpdater
    {
        /// <summary>Everything is happening based on <see cref="SelectedLimbusCustomLanguage"/></summary>
        public ref struct Limbus
        {
            /// <summary>
            /// Call both <see cref="@EditorModesShelf.Types.EditorModeIntermediator.RefreshRichText"/> and <see cref="@CurrentPreviewCreator.RebuildTextElements"/><br/><br/>
            /// Does <see langword="using"/>(<see cref="TMProEmitter.DisabledRichTextDelay"/>) thing by default
            /// </summary>
            public static void FullyRefreshShownRichText()
            {
                if (ProgramFullyLoaded)
                {
                    using (TMProEmitter.DisabledRichTextDelay)
                    {
                        @EditorModesShelf.CurrentEditorMode.RefreshRichText();

                        @CurrentPreviewCreator.RebuildTextElements();
                    }
                }
            }
            public static void UpdateFull()
            {
                Fonts.FontsUpdate_FontFamilies();
                Fonts.FontsUpdate_FontWeights();

                Additions.UpdateExtraReplacements();
                Additions.UpdateMultipleMeaningsDictionary(); /// Do not set UpdateKeywordsAfter to true, it will call <see cref="LimbusCustomLang.UpdateKeywords"/> onсe more before needed

                LimbusCustomLang.UpdateKeywords();

                Limbus.FullyRefreshShownRichText();
            }
            public ref struct LimbusCustomLang
            {
                /// <summary>
                /// 1. Fallback<br/>
                /// 2. Limbus Custom Lang<br/>
                /// 3. Additional<br/>
                /// 4. Keywords Multiple Meanings Dictionary applying on <see cref="KeywordsLoader.LimbusKeywords_ImplicitConversionOrder"/> (<see cref="KeywordsMultipleMeaningsDictionary.ApplyOnImplicitConversionOrderDictionary"/>)<br/>
                /// 5. Generate syntaxes based on loaded keywords (<see cref="JsonTextEditor.@LimbusTextSyntaxesPreset.GenerateSyntaxes"/>)
                /// </summary>
                public static void UpdateKeywords()
                {
                    KeywordsLoader.LoadKeywordsFrom(LoadedConfiguration.PreviewSettings.CustomLang.KeywordsFallbackDirectory, LocalizationDirectoryType.Fallback);
                    KeywordsLoader.LoadKeywordsFrom(SelectedLimbusCustomLanguage.KeywordsDirectory, LocalizationDirectoryType.Custom);
                    KeywordsLoader.LoadKeywordsFrom(LoadedConfiguration.PreviewSettings.CustomLang.AdditionalKeywordsDirectory, LocalizationDirectoryType.Additional);

                    KeywordsMultipleMeaningsDictionary.ApplyOnImplicitConversionOrderDictionary();

                    /**/SplashScreenWindow.ProgressSubObject = @Languages.VariableData.ReadedStartupSteps.SubStages.TextEditorSyntaxes;
                    JsonTextEditor.@LimbusTextSyntaxesPreset.GenerateSyntaxes();
                }
            }
            public ref struct Additions
            {
                /// <summary>
                /// <paramref name="UpdateKeywordsRightAfter"/> should be <see langword="true"/> only when called from <see cref="Internal.Configuration.SettingsWindow.SaveAndDeploy_KeywordsMultipleMeaningsDictionary"/> button click<br/>
                /// </summary>
                public static void UpdateMultipleMeaningsDictionary(bool UpdateKeywordsRightAfter = false)
                {
                    KeywordsMultipleMeaningsDictionary.SetSourceFile(SelectedLimbusCustomLanguage.KeywordsMultipleMeaningsDictionary);
                    
                    if (UpdateKeywordsRightAfter)
                    {
                        LimbusCustomLang.UpdateKeywords();
                    }
                }
                public static void UpdateExtraReplacements()
                {
                    ExtraReplacementsContextMenu.SetSourceFile(SelectedLimbusCustomLanguage.ContextMenuExtraReplacements);
                }
            }
            public ref struct Fonts
            {
                private static ResourceDictionary FontsDictionary => @EntanglementDictionary.MergedDictionaries.ByUriSource("/Limbus Registry/Context & Title fonts.xaml")!;

                private static Dictionary<string, List<FallbackFontMapping>> CompositeFontDefinitions = [];

                private record FallbackFontMapping
                {
                    [JsonProperty("Font")] public string Font { get; set; } = "";
                    [JsonProperty("Unicode")] public List<string> Unicode { get; set; } = [];
                }


                private static FileEventsNotifier CompositeFontDefinitionsWatcher { get; } = new(TargetFile: @"[⇲] Assets Directory\Limbus Embedded Fonts\CompositeFont Definitions.json")
                {
                    GeneralHandler = (_, _, _) => ReadCompositeFontDefinitionsFile()
                };


                public static void ReadCompositeFontDefinitionsFile()
                {
                    if (new FileInfo(@"[⇲] Assets Directory\Limbus Embedded Fonts\CompositeFont Definitions.json").TryDeserealizeJsonAs(out Dictionary<string, List<FallbackFontMapping>> Deserialized, out Exception Occurred))
                    {
                        CompositeFontDefinitions = Deserialized;
                        if (ProgramFullyLoaded) FontsUpdate_FontFamilies();
                    }
                    else
                    {
                        ErrorMessageWindow.ShowException(Occurred, @$"This exception occurred while trying to read composite font definitions file ""<b>[⇲] Assets Directory\Limbus Embedded Fonts\CompositeFont Definitions.json</b>""");
                    }
                }

                public static FontFamily CreateCompositeFont(string OriginalFontPath, string MapsType)
                {
                    try
                    {
                        if (File.Exists(OriginalFontPath) == false)
                        {
                            ErrorMessageWindow.ShowException(new FileNotFoundException("File not found"), $"Could not find font file \"<b>{OriginalFontPath}</b>\" ({MapsType})");
                            // -> load as Segoe UI further
                        }

                        FontFamily CompositeFont = new();

                        if (CompositeFontDefinitions.TryGetValue(MapsType, out List<FallbackFontMapping>? FallbackFontMappings))
                        {
                            foreach (FallbackFontMapping FallbackFontMapping in FallbackFontMappings)
                            {
                                try
                                {
                                    FontFamilyMap CreatedMap = FontFamilyMapFromFileOrName(
                                        FontPathOrName: FallbackFontMapping.Font,
                                        UnicodeRanges: string.Join(", ", FallbackFontMapping.Unicode.Select(x => x.Replace('—', '-')))
                                    );

                                    CompositeFont.FamilyMaps.Add(CreatedMap);
                                }
                                catch (Exception Occurred)
                                {
                                    ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to create fallback range <u>{string.Join(", ", FallbackFontMapping.Unicode)}</u> from font file <b>\"{FallbackFontMapping.Font}\"</b> to apply it on the {MapsType} font.");
                                }
                            }
                        }
                    
                        // Take all remaining unicode characters by specified font
                        FontFamilyMap FinalMap = FontFamilyMapFromFileOrName(OriginalFontPath, "0000-10FFFF");
                        CompositeFont.FamilyMaps.Add(FinalMap);

                        return CompositeFont;
                    }
                    catch (Exception Occurred)
                    {
                        ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to create composite font (With fallbacks) from <b>\"{OriginalFontPath}\"</b> font file (Type: {MapsType}).");
                        return new FontFamily();
                    }
                }

                public static void FontsUpdate_FontFamilies()
                {
                    FontsDictionary["Limbus:TitleFont"] = CreateCompositeFont(SelectedLimbusCustomLanguage.TitleFont, "Title");
                    FontsDictionary["Limbus:ContextFont"] = CreateCompositeFont(SelectedLimbusCustomLanguage.ContextFont, "Context");
                }
                public static void FontsUpdate_FontWeights()
                {
                    FontsDictionary["Limbus:TitleFont_Weight"] = WeightFrom(SelectedLimbusCustomLanguage.TitleFont_FontWeight);
                    FontsDictionary["Limbus:ContextFont_Weight"] = WeightFrom(SelectedLimbusCustomLanguage.ContextFont_FontWeight);
                }
            }
        }
    }
}