using LCLocalizationInterface.Instruments.Classes;
using LCLocalizationInterface.Internal.Configuration;
using LCLocalizationInterface.LimbusRegistry.PreviewCreator;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        /// <summary>Check for Theme's <c>"Hide background image at minimum window width"</c> condition</summary>
        public void SizeChanged_CheckThemeBackgroundImageVisibility(object Sender, SizeChangedEventArgs Args)
        {
            BackgroundImageStackPanel.Visibility =
                double.Round(this.Width) <= double.Round(this.MinWidth) &
                @Themes.CurrentTheme.Common.MainWindow.HideBackgroundImageAtMinimalWidth
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }
    }
}

namespace LCLocalizationInterface.Internal
{
    public ref struct @Themes
    {
        private static FileEventsNotifier ThemeFileWatcher { get; } = new(
            TargetDirectory: AppDomain.CurrentDomain.BaseDirectory, // Dummy dir to not crash XAML Designer
            FileFilter: "Visual.json"
        ) {
            GeneralHandler = (_, _, _) =>
            {
                if (ProgramFullyLoaded) @Themes.LoadFromFile($"{LoadedConfiguration.Internal.UITheme}/Visual.json");
            }
        };

        private static ResourceDictionary ThemeKeysDictionary => @EntanglementDictionary.MergedDictionaries.ByUriSource("/Internal/Theme.xaml")!;
        public static ThemeDefinition CurrentTheme { get; private set; } = new();

        /// <summary>
        /// Change {DynamicResource} theme keys from <c>`UI Theme.xaml`</c> resource dictionary by the <see cref="ThemeDefinitionSection.TriggerKeysApplying"/> using reflection and <see cref="ThemeKeyAttribute"/>
        /// </summary>
        public static void LoadFromFile(string ThemeFilePath)
        {
            if (File.Exists(ThemeFilePath))
            {
                if (new FileInfo(ThemeFilePath).TryDeserealizeJsonAs(out ThemeDefinition Theme, out Exception Occurred))
                {
                    TriggerStagedThemeKeysApplying(Theme);

                    ThemeFileWatcher.Path = Path.GetDirectoryName(ThemeFilePath)!;
                    CurrentTheme = Theme;
                    SettingsWindow.SettingsWindowInstance.UpdateSyntaxHighlightColors();
                    PreviewCreatorPage.PreviewCreatorPageInstance.SetIdentityOrEGONameLineBreakSyntaxColor();
                    @Languages.PresentedTextElements["[Settings / Internal] [-] * Selected UI Theme (Comment)"].RichText = CurrentTheme.ThemeComment;

                    var TextEditorMarkers = CurrentTheme.JsonTextEditorSettings.VisualMarkersVisibility;
                    var TextEditorViewOptions = MainWindowInstance.LimbusJsonTextEditor.TextArea.TextView.Options;

                    MainWindowInstance.LimbusJsonTextEditor.ShowLineNumbers = TextEditorMarkers.LineNumbers;
                    TextEditorViewOptions.ShowTabs = TextEditorMarkers.Tabs;
                    TextEditorViewOptions.ShowSpaces = TextEditorMarkers.Spaces;
                    TextEditorViewOptions.ShowEndOfLine = TextEditorMarkers.EndLineSymbol;
                    TextEditorViewOptions.ShowBoxForControlCharacters = TextEditorMarkers.BoxForControlCharacters;

                    // Manual change from UI
                    if (ProgramFullyLoaded)
                    {
                        JsonTextEditor.@LimbusTextSyntaxesPreset.GenerateSyntaxes(); // Update tag colors

                        // Update unsaved changes marker color
                        @EditorModesShelf.CurrentEditorMode.ChangeAllRightMenuUnsavedChangesMarkers_OnButtons();
                        @EditorModesShelf.CurrentEditorMode.ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(MainWindowInstance.LimbusJsonTextEditor.Document);
                        @EditorModesShelf.CurrentEditorMode.ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();

                        Languages.ReModifyTextElementsWithThemeKeysApplied();

                        MainWindowInstance.SizeChanged_CheckThemeBackgroundImageVisibility(null!, null!);
                    }
                }
                else
                {
                    if (Occurred.InnerException is ThemeDefinitionSection.UnknownXAMLResourceKeyException UnknownThemeKey)
                    {
                        rin(UnknownThemeKey.Message); // Debug thing when key from ThemeKeyAttribute is not in the Theme.xaml ResourceDictionary
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        ErrorMessageWindow.ShowException(Occurred, $"This exception occured while trying to read theme file <b>\"{ThemeFilePath}\"</b>");
                    }
                }
            }
        }

        /// <summary>
        /// Recursive call of all <see cref="ThemeDefinitionSection.TriggerKeysApplying"/> within the properties tree of given <see cref="ThemeDefinitionSection"/>
        /// </summary>
        public static void TriggerStagedThemeKeysApplying(ThemeDefinitionSection ThemeSection)
        {
            ThemeSection.TriggerKeysApplying();

            foreach (ThemeDefinitionSection SubSecion in ThemeSection.GetBaseTypedProperties<ThemeDefinitionSection>())
            {
                TriggerStagedThemeKeysApplying(SubSecion);
            }
        }

        public static TResource GetDefaultStyleDictionaryResource<TResource>(string Key)
        {
            return (TResource)@EntanglementDictionary.MergedDictionaries.ByUriSource("/Internal/Resource Dictionaries/Default Style.xaml")![Key];
        }


        public enum ThemeKeyType
        {
            Thickness,
            CornerRadius,
            SolidColorBrush,
            SystemColor,
            FontFamily,
            FontWeight,
            FontStretch,
            BitmapImage,
            DoesntMatter,
        }


        [AttributeUsage(AttributeTargets.Property)]
        public class ThemeKeyAttribute(string[] Key, ThemeKeyType Type) : Attribute
        {
            public string ResourceKey { get; } = Key[0]; // :p
            public ThemeKeyType ResourceType { get; } = Type;
        }


        /// <summary>
        /// Parent record for all theme sections
        /// </summary>
        public abstract record ThemeDefinitionSection
        {
            public class UnknownXAMLResourceKeyException(string Message) : Exception(Message);

            public void TriggerKeysApplying()
            {
                foreach (PropertyInfo JsonProperty in this.GetType().GetProperties())
                {
                    if (JsonProperty.HasAttribute(out JsonPropertyAttribute JsonPropertyInfo) && JsonProperty.HasAttribute(out ThemeKeyAttribute ThemeKeyAttr))
                    {
                        try
                        {
                            if (!ThemeKeysDictionary.Contains(ThemeKeyAttr!.ResourceKey)) throw new UnknownXAMLResourceKeyException($"Unknown theme key \"{ThemeKeyAttr.ResourceKey}\" (Its presented in this ThemeDefinitionSection, but not in the `UI Theme.xaml` file)");

                            object? ThemeKeyValue = JsonProperty.GetValue(obj: this);

                            object? ConvertedValueToSet = ThemeKeyAttr.ResourceType switch
                            {
                                ThemeKeyType.Thickness => ThicknessFrom((double[])ThemeKeyValue!),
                                ThemeKeyType.CornerRadius => CornerRadiusFrom((double[])ThemeKeyValue!),
                                ThemeKeyType.SolidColorBrush => ToSolidColorBrush((string)ThemeKeyValue!),
                                ThemeKeyType.SystemColor => ToColor((string)ThemeKeyValue!),
                                ThemeKeyType.FontFamily => FontFamilyFromFileOrName((string)ThemeKeyValue!),
                                ThemeKeyType.FontWeight => WeightFrom((string)ThemeKeyValue!),
                                ThemeKeyType.FontStretch => FontStretchFrom((string)ThemeKeyValue!),
                                ThemeKeyType.BitmapImage => BitmapFromFile((string)ThemeKeyValue!),
                                _ => ThemeKeyValue
                            };

                            if (ThemeKeyAttr.ResourceType == ThemeKeyType.SolidColorBrush)
                            {
                                object? CurrentValue = ThemeKeysDictionary[ThemeKeyAttr.ResourceKey];

                                // Check SolidColorBrushes for equality in order not to waste a whole second of time updating hundreds of factially equal DynamicResources
                                // And check in the string version, because one SolidColorBrush will not be equal to another SolidColorBrush with the same color because instances references heaps something . . . . 
                                if ($"{CurrentValue}" != $"{ConvertedValueToSet}")
                                {
                                    ThemeKeysDictionary[ThemeKeyAttr.ResourceKey] = ConvertedValueToSet;
                                }
                            }
                            else
                            {
                                ThemeKeysDictionary[ThemeKeyAttr.ResourceKey] = ConvertedValueToSet;
                            }
                        }
                        catch (Exception Occurred)
                        {
                            ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to apply Theme property \"{JsonPropertyInfo.PropertyName}\" ({ThemeKeyAttr.ResourceKey})");
                        }
                    }
                }
            }
        }



        /// <summary>
        /// <c>Visual.json</c> theme file
        /// </summary>
        public sealed record ThemeDefinition : ThemeDefinitionSection
        {
            private static string RemoveAlphaValue(string Color) => Color.RegexReplace(@"^(#)?(?<ExpectedAlpha>[a-fA-F0-9]{2})(?<RemainingRGBValue>[a-fA-F0-9]{6})$", "$1$3");


            [JsonProperty("Readme")]
            public List<string> Readme { get; set; } = [];

            [JsonProperty("Theme Comment")]
            public string ThemeComment { get; set; } = "";

            [JsonProperty("Title Bar")]
            public TitleBar_PROP TitleBar { get; set; } = new();
            public record TitleBar_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Background"), ThemeKey([@"Theme:TitleBar.Background.Color"], ThemeKeyType.SolidColorBrush)]
                public string Background { get; set => field = RemoveAlphaValue(value); } = "#101117";


                [JsonProperty("Icons"), ThemeKey([@"Theme:TitleBar.Icons"], ThemeKeyType.SolidColorBrush)]
                public string Icons { get; set; } = "#ADADAD";


                [JsonProperty("Titles Foreground"), ThemeKey([@"Theme:TitleBar.TitlesForeground"], ThemeKeyType.SolidColorBrush)]
                public string TitlesForeground { get; set; } = "#473131";


                [JsonProperty("Buttons Corner Radius"), ThemeKey([@"Theme:TitleBar.Buttons.CornerRadius"], ThemeKeyType.CornerRadius)]
                public double[] ButtonsCornerRadius { get; set; } = [4];


                [JsonProperty("Buttons Highlight"), ThemeKey([@"Theme:TitleBar.Buttons.Highlighted"], ThemeKeyType.SolidColorBrush)]
                public string ButtonsHighlight { get; set; } = "#302829";


                [JsonProperty("Buttons Highlight (Exit button)"), ThemeKey([@"Theme:TitleBar.Buttons.Highlighted.ExitButton"], ThemeKeyType.SolidColorBrush)]
                public string ButtonsHighlight_ExitButton { get; set; } = "#A52C42";
            }



            [JsonProperty("Common")]
            public Common_PROP Common { get; set; } = new();
            public record Common_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Context Menu Separators Color"), ThemeKey([@"Theme:Common.ContextMenus.Separator.Color"], ThemeKeyType.SolidColorBrush)]
                public string ContextMenuSeparatorsColor { get; set; } = "#FFFFFF";



                [JsonProperty("Windows Drop Shadow")]
                public DropShadowForWindows_PROP DropShadowForWindows { get; set; } = new();
                public record DropShadowForWindows_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Enabled"), ThemeKey([@"Theme:Common.WindowsDropShadow.Enabled"], ThemeKeyType.DoesntMatter)]
                    public bool Enabled { get; set; } = true;

                    [JsonProperty("Color"), ThemeKey([@"Theme:Common.WindowsDropShadow.Color"], ThemeKeyType.SystemColor)]
                    public string Color { get; set; } = "#000000";
                }



                [JsonProperty("Windows Fade Animation Timings")]
                public WindowsFadeAnimation_PROP WindowsFadeAnimationTimings { get; set; } = new();
                public record WindowsFadeAnimation_PROP : ThemeDefinitionSection
                {
                    private static double[] FormatTimings(double[] Original, double MinValue = 0.0)
                    {
                        if      (Original.Length == 1) { Original = [Original[0], 0.0]; }
                        else if (Original.Length == 0) { Original = [0.0, 0.0];         }

                        Original[0] = Math.Max(Original[0], MinValue);
                        Original[1] = Math.Max(Original[1], MinValue);

                        return Original;
                    }

                    [JsonProperty("Duration (Seconds)")]
                    public Duration_PROP Duration { get; set; } = new();
                    public record Duration_PROP
                    {
                        [JsonProperty("Main Window"    )] public double[] MainWindow     { get; set; } = [0.35, 0.11];
                        [JsonProperty("Settings Window")] public double[] SettingsWindow { get; set; } = [0.04, 0.1];
                        [JsonProperty("Dialog Windows" )] public double[] DialogWindows  { get; set; } = [0.11, 0.21];

                        [JsonProperty("Identity/E.G.O Preivew Creator Switch Transition")]
                        public double[] PreviewCreator { get; set; } = [0.2, 0.2];


                        [OnDeserialized]
                        public void OnDeserialized(StreamingContext Context) => EnsureRanges();
                        public void EnsureRanges()
                        {
                            try
                            {
                                MainWindow = FormatTimings(MainWindow);
                                SettingsWindow = FormatTimings(SettingsWindow);
                                DialogWindows = FormatTimings(DialogWindows);
                                PreviewCreator = FormatTimings(PreviewCreator);
                            }
                            catch (Exception Occurred)
                            {
                                ErrorMessageWindow.ShowException(Occurred);
                            }
                        }
                    }


                    [JsonProperty("Speed Ratio")]
                    public SpeedRatio_PROP SpeedRatio { get; set; } = new();
                    public record SpeedRatio_PROP
                    {
                        [JsonProperty("Main Window"    )] public double[] MainWindow     { get; set; } = [1.0, 1.0];
                        [JsonProperty("Settings Window")] public double[] SettingsWindow { get; set; } = [1.0, 1.0];
                        [JsonProperty("Dialog Windows" )] public double[] DialogWindows  { get; set; } = [1.0, 1.0];
                        
                        [JsonProperty("Identity/E.G.O Preivew Creator Switch Transition")]
                        public double[] PreviewCreator { get; set; } = [1.0, 1.0];


                        [OnDeserialized]
                        public void OnDeserialized(StreamingContext Context) => EnsureRanges();
                        public void EnsureRanges()
                        {
                            try
                            {
                                MainWindow = FormatTimings(MainWindow, 0.05);
                                SettingsWindow = FormatTimings(SettingsWindow, 0.05);
                                DialogWindows = FormatTimings(DialogWindows, 0.05);
                                PreviewCreator = FormatTimings(PreviewCreator, 0.05);
                            }
                            catch (Exception Occurred)
                            {
                                ErrorMessageWindow.ShowException(Occurred);
                            }
                        }
                    }


                    [JsonProperty("Acceleration/Deceleration Ratios")]
                    public AccelerationRatio_PROP AccelerationDecelerationRatios { get; set; } = new();
                    public record AccelerationRatio_PROP
                    {
                        [JsonProperty("Main Window"    )] public double[][] MainWindow     { get; set; } = [[0.0, 0.0], [0.0, 0.0]];
                        [JsonProperty("Settings Window")] public double[][] SettingsWindow { get; set; } = [[0.0, 0.0], [0.0, 0.0]];
                        [JsonProperty("Dialog Windows" )] public double[][] DialogWindows  { get; set; } = [[0.0, 0.0], [0.0, 0.0]];
                        
                        [JsonProperty("Identity/E.G.O Preivew Creator Switch Transition")]
                        public double[][] PreviewCreator { get; set; } = [[0.0, 0.0], [0.0, 0.0]];


                        [OnDeserialized]
                        public void OnDeserialized(StreamingContext Context) => EnsureRanges();
                        public void EnsureRanges()
                        {
                            try
                            {
                                static double[][] Ensure(double[][] ADRatios)
                                {
                                    for (int FadeInOrOutIndex = 0; FadeInOrOutIndex < ADRatios.Length; FadeInOrOutIndex++)
                                    {
                                        if (ADRatios[FadeInOrOutIndex].Length == 2)
                                        {
                                            if (ADRatios[FadeInOrOutIndex][0] + ADRatios[FadeInOrOutIndex][1] > 1.0)
                                            {
                                                ADRatios[FadeInOrOutIndex][0] = 0.0;
                                                ADRatios[FadeInOrOutIndex][1] = 0.0;
                                            }

                                            if (ADRatios[FadeInOrOutIndex][0] < 0) ADRatios[FadeInOrOutIndex][0] = 0.0;
                                            if (ADRatios[FadeInOrOutIndex][1] < 0) ADRatios[FadeInOrOutIndex][1] = 0.0;
                                        }
                                        else
                                        {
                                            ADRatios[FadeInOrOutIndex] = [0.0, 0.0];
                                        }
                                    }

                                    return ADRatios;
                                }

                                MainWindow = Ensure(MainWindow);
                                SettingsWindow = Ensure(SettingsWindow);
                                DialogWindows = Ensure(DialogWindows);
                                PreviewCreator = Ensure(PreviewCreator);
                            }
                            catch (Exception Occurred)
                            {
                                ErrorMessageWindow.ShowException(Occurred);
                            }
                        }
                    }
                }



                [JsonProperty("Main Window")]
                public MainWindow_PROP MainWindow { get; set; } = new();
                public record MainWindow_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Border Color"), ThemeKey([@"Theme:Common.MainWindow.Border.Color"], ThemeKeyType.SolidColorBrush)]
                    public string BorderColor { get; set; } = "#3A3A3A";


                    [JsonProperty("Border Corner radius"), ThemeKey([@"Theme:Common.MainWindow.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] BorderCornerRadius { get; set; } = [4];



                    [JsonProperty("Background Color"), ThemeKey([@"Theme:Common.MainWindow.Background.Color"], ThemeKeyType.SolidColorBrush)]
                    public string BackgroundColor { get; set; } = "#0F0F0F";


                    [JsonProperty("Background Image"), ThemeKey([@"Theme:Common.MainWindow.Background.Image"], ThemeKeyType.BitmapImage)]
                    public string BackgroundImage { get; set; } = "";


                    [JsonProperty("Background Image Shadow"), ThemeKey([@"Theme:Common.MainWindow.Background.Image.Shadow"], ThemeKeyType.SolidColorBrush)]
                    public string BackgroundImageShadow { get; set; } = "#00000000";



                    [JsonProperty("Hide background image at minimum window width")]
                    public bool HideBackgroundImageAtMinimalWidth { get; set; } = false;



                    [JsonProperty("Identity/E.G.O preview creator")]
                    public MainWindow_PreviewCreator_PROP MainWindow_PreviewCreator { get; set; } = new();
                    public record MainWindow_PreviewCreator_PROP : ThemeDefinitionSection
                    {
                        [JsonProperty("Background Color"), ThemeKey([@"Theme:Common.MainWindow.PreviewCreator.Background.Color"], ThemeKeyType.SolidColorBrush)]
                        public string BackgroundColor { get; set; } = "#0F0F0F";


                        [JsonProperty("Background Image"), ThemeKey([@"Theme:Common.MainWindow.PreviewCreator.Background.Image"], ThemeKeyType.BitmapImage)]
                        public string BackgroundImage { get; set; } = "";


                        [JsonProperty("Background Image Shadow"), ThemeKey([@"Theme:Common.MainWindow.PreviewCreator.Background.Image.Shadow"], ThemeKeyType.SolidColorBrush)]
                        public string BackgroundImageShadow { get; set; } = "#00000000";
                    }
                }


                [JsonProperty("Dialog Windows")]
                public DialogWindows_PROP DialogWindows { get; set; } = new();
                public record DialogWindows_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Border Color"), ThemeKey([@"Theme:Common.DialogWindows.Border.Color"], ThemeKeyType.SolidColorBrush)]
                    public string BorderColor { get; set; } = "#0F0F0F";

                    [JsonProperty("Border Corner radius"), ThemeKey([@"Theme:Common.DialogWindows.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] BorderCornerRadius { get; set; } = [4];



                    [JsonProperty("Background Color"), ThemeKey([@"Theme:Common.DialogWindows.Background.Color"], ThemeKeyType.SolidColorBrush)]
                    public string BackgroundColor { get; set; } = "#0F0F0F";


                    [JsonProperty("Background Image"), ThemeKey([@"Theme:Common.DialogWindows.Background.Image"], ThemeKeyType.BitmapImage)]
                    public string BackgroundImage { get; set; } = "";


                    [JsonProperty("Background Image Shadow"), ThemeKey([@"Theme:Common.DialogWindows.Background.Image.Shadow"], ThemeKeyType.SolidColorBrush)]
                    public string BackgroundImageShadow { get; set; } = "#00000000";
                }


                [JsonProperty("Settings Window")]
                public SettingsWindow_PROP SettingsWindow { get; set; } = new();
                public record SettingsWindow_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Border Color"), ThemeKey([@"Theme:Common.SettingsWindow.Border.Color"], ThemeKeyType.SolidColorBrush)]
                    public string BorderColor { get; set; } = "#3A3A3A";

                    [JsonProperty("Border Corner radius"), ThemeKey([@"Theme:Common.SettingsWindow.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] BorderCornerRadius { get; set; } = [4];



                    [JsonProperty("Background Color"), ThemeKey([@"Theme:Common.SettingsWindow.Background.Color"], ThemeKeyType.SolidColorBrush)]
                    public string BackgroundColor { get; set; } = "#191818";


                    [JsonProperty("Background Image"), ThemeKey([@"Theme:Common.SettingsWindow.Background.Image"], ThemeKeyType.BitmapImage)]
                    public string BackgroundImage { get; set; } = "";


                    [JsonProperty("Background Image Shadow"), ThemeKey([@"Theme:Common.SettingsWindow.Background.Image.Shadow"], ThemeKeyType.SolidColorBrush)]
                    public string BackgroundImageShadow { get; set; } = "#00000000";


                    [JsonProperty("Specific Settings Caution"), ThemeKey([@"Theme:Common.SettingsWindow.SpecificSettings.CautionsColor"], ThemeKeyType.SystemColor)]
                    public string SpecificSettingsCaution { get; set; } = "#FF4500";
                }


                //[JsonProperty("Skills Display Info Manager Window")]
                //public SkillsDisplayInfoManagerWindow_PROP SkillsDisplayInfoManagerWindow { get; set; } = new();
                //public record SkillsDisplayInfoManagerWindow_PROP : ThemeDefinitionSection
                //{
                //    [JsonProperty("Border Color"), ThemeKey([@"Theme:Common.SkillsDisplayInfoManagerWindow.Border.Color"], ThemeKeyType.SolidColorBrush)]
                //    public string BorderColor { get; set; } = "#0F0F0F";

                //    [JsonProperty("Border Corner radius"), ThemeKey([@"Theme:Common.SkillsDisplayInfoManagerWindow.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                //    public double[] BorderCornerRadius { get; set; } = [4];



                //    [JsonProperty("Background Color"), ThemeKey([@"Theme:Common.SkillsDisplayInfoManagerWindow.Background.Color"], ThemeKeyType.SolidColorBrush)]
                //    public string BackgroundColor { get; set; } = "#0F0F0F";


                //    [JsonProperty("Background Image"), ThemeKey([@"Theme:Common.SkillsDisplayInfoManagerWindow.Background.Image"], ThemeKeyType.BitmapImage)]
                //    public string BackgroundImage { get; set; } = "";


                //    [JsonProperty("Background Image Shadow"), ThemeKey([@"Theme:Common.SkillsDisplayInfoManagerWindow.Background.Image.Shadow"], ThemeKeyType.SolidColorBrush)]
                //    public string BackgroundImageShadow { get; set; } = "#00000000";
                //}


                [JsonProperty("Right Menu")]
                public RightMenu_PROP RightMenu { get; set; } = new();
                public record RightMenu_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Uptie Buttons Background"), ThemeKey([@"Theme:Common.RightMenu.UptieButtons.Background"], ThemeKeyType.SolidColorBrush)]
                    public string UptieButtonsBackground { get; set; } = "#0E0907";

                    [JsonProperty("Navigation Panel Background"), ThemeKey([@"Theme:Common.RightMenu.NavigationPanel.Background"], ThemeKeyType.SolidColorBrush)]
                    public string NavigationPanelBackground { get; set; } = "#00000000";

                    [JsonProperty("Current Object ID Foreground"), ThemeKey([@"Theme:Common.RightMenu.CurrentObjectID.Foreground"], ThemeKeyType.SolidColorBrush)]
                    public string CurrentObjectIDForeground { get; set; } = "#DDD9D9";

                    [JsonProperty("Uptie Tier Text Foreground"), ThemeKey([@"Theme:Common.RightMenu.UptieTierText.Foreground"], ThemeKeyType.SolidColorBrush)]
                    public string UptieTierTextForeground { get; set; } = "#E6C5C5";
                }
            }



            [JsonProperty("UI Text")]
            public UIText_PROP UIText { get; set; } = new();
            public record UIText_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Unsaved Changes Marker Color")]
                public string UnsavedChangesMarkerColor { get; set; } = "#FC5A03";


                
                
                [JsonProperty("Foreground"), ThemeKey([@"Theme:UIText.MainWindow.Foreground"], ThemeKeyType.SolidColorBrush)]
                public string Foreground { get; set; } = "#D5CDCD";


                [JsonProperty("Foreground (Dialog Windows)"), ThemeKey([@"Theme:UIText.DialogWindows.Foreground"], ThemeKeyType.SolidColorBrush)]
                public string Foreground_DialogWindows { get; set; } = "#EEEEEE";


                [JsonProperty("Foreground (Settings)"), ThemeKey([@"Theme:UIText.SettingsWindow.Foreground"], ThemeKeyType.SolidColorBrush)]
                public string Foreground_Settings { get; set; } = "#D5CDCD";


                [JsonProperty("Foreground (Settings (Comments))"), ThemeKey([@"Theme:UIText.SettingsWindow.Foreground.Comments"], ThemeKeyType.SolidColorBrush)]
                public string Foreground_SettingsComments { get; set; } = "#B2ABAB";


                [JsonProperty("Foreground (Identity/E.G.O Preview creator)"), ThemeKey([@"Theme:UIText.MainWindow.PreviewCreator.Foreground"], ThemeKeyType.SolidColorBrush)]
                public string Foreground_IdentityOrEGOPreviewCreator { get; set; } = "#D5CDCD";




                [JsonProperty("Tooltips Foreground"), ThemeKey([@"Theme:UIText.MainWindow.Foreground.Tooltips"], ThemeKeyType.SolidColorBrush)]
                public string Foreground_Tooltips { get; set; } = "#D5CDCD";


                [JsonProperty("Tooltips Foreground (Identity/E.G.O Preview creator)"), ThemeKey([@"Theme:UIText.DialogWindows.Foreground.Tooltips"], ThemeKeyType.SolidColorBrush)]
                public string Foreground_DialogWindows_Tooltips { get; set; } = "#D5CDCD";


                [JsonProperty("Tooltips Foreground (Settings)"), ThemeKey([@"Theme:UIText.SettingsWindow.Foreground.Tooltips"], ThemeKeyType.SolidColorBrush)]
                public string Foreground_SettingsWindow_Tooltips { get; set; } = "#D5CDCD";


                [JsonProperty("Tooltips Foreground (Dialog Windows)"), ThemeKey([@"Theme:UIText.MainWindow.PreviewCreator.Foreground.Tooltips"], ThemeKeyType.SolidColorBrush)]
                public string Foreground_IdentityOrEGOPreviewCreator_Tooltips { get; set; } = "#D5CDCD";
            }



            [JsonProperty("UI Textfields")]
            public UITexfields_PROP UITextfields { get; set; } = new();
            public record UITexfields_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Foreground"), ThemeKey([@"Theme:UITextfields.Foreground"], ThemeKeyType.SolidColorBrush)]
                public string Foreground { get; set; } = "#B1B1B9";


                [JsonProperty("Foreground (Shadow text)"), ThemeKey([@"Theme:UITextfields.ShadowText.Foreground"], ThemeKeyType.SolidColorBrush)]
                public string Foreground_ShadowText { get; set; } = "#514C46";



                [JsonProperty("Background"), ThemeKey([@"Theme:UITextfields.Background"], ThemeKeyType.SolidColorBrush)]
                public string Background { get; set; } = "#181818";


                [JsonProperty("Border"), ThemeKey([@"Theme:UITextfields.Border"], ThemeKeyType.SolidColorBrush)]
                public string Border { get; set; } = "#333333";


                [JsonProperty("Border Thickness"), ThemeKey([@"Theme:UITextfields.Border.Thickness"], ThemeKeyType.Thickness)]
                public double[] BorderThickness { get; set; } = [1.7];


                [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:UITextfields.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                public double[] BorderCornerRadius { get; set; } = [1.5];



                [JsonProperty("Caret"), ThemeKey([@"Theme:UITextfields.Caret"], ThemeKeyType.SolidColorBrush)]
                public string Caret { get; set; } = "#E9DBC6";



                [JsonProperty("Selection")]
                public Selection_PROP Selection { get; set; } = new();
                public record Selection_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Foreground"), ThemeKey([@"Theme:UITextfields.Selection.Foreground"], ThemeKeyType.SolidColorBrush)]
                    public string Foreground { get; set; } = "#ABA193";


                    [JsonProperty("Background"), ThemeKey([@"Theme:UITextfields.Selection.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#55B4B4B4";


                    [JsonProperty("Border"), ThemeKey([@"Theme:UITextfields.Selection.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#66D4D4D4";


                    [JsonProperty("Border Thickness"), ThemeKey([@"Theme:UITextfields.Selection.Border.Thickness"], ThemeKeyType.DoesntMatter)]
                    public double BorderThickness { get; set; } = 1;


                    [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:UITextfields.Selection.Border.CornerRadius"], ThemeKeyType.DoesntMatter)]
                    public double BorderCornerRadius { get; set; } = 0;
                }



                [JsonProperty("Syntax")]
                public Syntax_PROP Syntax { get; set; } = new();
                public record Syntax_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Highlight 1")]
                    public string Highlight1 { get; set; } = "#F54927";

                    [JsonProperty("Highlight 2")]
                    public string Highlight2 { get; set; } = "#F52727";

                    [JsonProperty("Highlight 3")]
                    public string Highlight3 { get; set; } = "#E24A4A";

                    [JsonProperty("Highlight 4")]
                    public string Highlight4 { get; set; } = "#E2734A";
                }
            }



            [JsonProperty("Json Text Editor")]
            public JsonTextEditor_PROP JsonTextEditorSettings { get; set; } = new();
            public record JsonTextEditor_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Visual Markers Visibility")]
                public VisualMarkersVisibility_PROP VisualMarkersVisibility { get; set; } = new();
                public record VisualMarkersVisibility_PROP : ThemeDefinitionSection
                {
                    // Easier to set manually rather than creating Boolean DependencyProperties for DynamicResources

                    [JsonProperty("Tabs")]
                    public bool Tabs { get; set; } = false;

                    [JsonProperty("Spaces")]
                    public bool Spaces { get; set; } = false;

                    [JsonProperty("Line Numbers")]
                    public bool LineNumbers { get; set; } = true;

                    [JsonProperty("Line End Symbol")]
                    public bool EndLineSymbol { get; set; } = false;

                    [JsonProperty("Box for Control Characters")]
                    public bool BoxForControlCharacters { get; set; } = false;
                }



                [JsonProperty("Foreground"), ThemeKey([@"Theme:JsonTextEditor.Foreground"], ThemeKeyType.SolidColorBrush)]
                public string Foreground { get; set; } = "#ABA193";


                [JsonProperty("Background"), ThemeKey([@"Theme:JsonTextEditor.Background"], ThemeKeyType.SolidColorBrush)]
                public string Background { get; set; } = "#191919";


                [JsonProperty("Font"), ThemeKey([@"Theme:JsonTextEditor.Font"], ThemeKeyType.FontFamily)]
                public string Font { get; set; } = "";


                [JsonProperty("Font Weight"), ThemeKey([@"Theme:JsonTextEditor.FontWeight"], ThemeKeyType.FontWeight)]
                public string FontWeight { get; set; } = "Regular";


                [JsonProperty("Font Stretch"), ThemeKey([@"Theme:JsonTextEditor.FontWeight"], ThemeKeyType.FontWeight)]
                public string FontStretch { get; set; } = "Normal";


                [JsonProperty("Font Size"), ThemeKey([@"Theme:JsonTextEditor.FontSize"], ThemeKeyType.DoesntMatter)]
                public double FontSize { get; set; } = 16.8;


                [JsonProperty("Caret"), ThemeKey([@"Theme:JsonTextEditor.Caret"], ThemeKeyType.SolidColorBrush)]
                public string Caret { get; set; } = "#E9DBC6";


                [JsonProperty("Border"), ThemeKey([@"Theme:JsonTextEditor.Border"], ThemeKeyType.SolidColorBrush)]
                public string Border { get; set; } = "#333333";


                [JsonProperty("Border Thickness"), ThemeKey([@"Theme:JsonTextEditor.Border.Thickness"], ThemeKeyType.Thickness)]
                public double[] BorderThickness { get; set; } = [0];


                [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:JsonTextEditor.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                public double[] BorderCornerRadius { get; set; } = [1];



                [JsonProperty("Selection")]
                public Selection_PROP Selection { get; set; } = new();
                public record Selection_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Foreground"), ThemeKey([@"Theme:JsonTextEditor.Selection.Foreground"], ThemeKeyType.SolidColorBrush)]
                    public string Foreground { get; set; } = "#ABA193";


                    [JsonProperty("Background"), ThemeKey([@"Theme:JsonTextEditor.Selection.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#55B4B4B4";


                    [JsonProperty("Border"), ThemeKey([@"Theme:JsonTextEditor.Selection.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#66D4D4D4";


                    [JsonProperty("Border Thickness"), ThemeKey([@"Theme:JsonTextEditor.Selection.Border.Thickness"], ThemeKeyType.DoesntMatter)]
                    public double BorderThickness { get; set; } = 1;


                    [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:JsonTextEditor.Selection.Border.CornerRadius"], ThemeKeyType.DoesntMatter)]
                    public double BorderCornerRadius { get; set; } = 0;
                }
            }



            [JsonProperty("Unsaved Changes Info")]
            public UnsavedChangesInfo_PROP UnsavedChangesInfo { get; set; } = new();
            public record UnsavedChangesInfo_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Main Editor")]
                public MainEditor_PROP MainEditor { get; set; } = new();
                public record MainEditor_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Blurred Background"), ThemeKey([@"Theme:UnsavedChanges.MainEditor.BlurredBackground"], ThemeKeyType.SolidColorBrush)]
                    public string BlurredBackground { get; set; } = "#8C090909";
                }


                [JsonProperty("Identity/E.G.O Preview Creator")]
                public PreviewCreator_PROP PreviewCreator { get; set; } = new();
                public record PreviewCreator_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Blurred Background"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.BlurredBackground"], ThemeKeyType.SolidColorBrush)]
                    public string BlurredBackground { get; set; } = "#8C090909";


                    [JsonProperty("Json Difference Viewer")]
                    public JsonDiffViewer_PROP JsonDiffViewer { get; set; } = new();
                    public record JsonDiffViewer_PROP : ThemeDefinitionSection
                    {
                        [JsonProperty("Font"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.Font"], ThemeKeyType.FontFamily)]
                        public string Font { get; set; } = "Cascadia Code";


                        [JsonProperty("Font Size"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.FontSize"], ThemeKeyType.DoesntMatter)]
                        public double FontSize { get; set; } = 14;


                        [JsonProperty("Font Weight"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.FontWeight"], ThemeKeyType.FontWeight)]
                        public string FontWeight { get; set; } = "Regular";


                        [JsonProperty("Font Stretch"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.FontStretch"], ThemeKeyType.FontStretch)]
                        public string FontStretch { get; set; } = "Normal";



                        [JsonProperty("Foreground"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.Foreground"], ThemeKeyType.SolidColorBrush)]
                        public string Foreground { get; set; } = "#D5CDCD";


                        [JsonProperty("Line Numbers"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.LineNumbers"], ThemeKeyType.SolidColorBrush)]
                        public string LineNumbers { get; set; } = "#999393";



                        [JsonProperty("Old Value (Background)"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.OldValue.Background"], ThemeKeyType.SolidColorBrush)]
                        public string OldValue_Background { get; set; } = "#40D82020";

                        [JsonProperty("Old Value (Foreground)"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.OldValue.Foreground"], ThemeKeyType.SolidColorBrush)]
                        public string OldValue_Foreground { get; set; } = "#D5CDCD";


                        [JsonProperty("New Value (Background)"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.NewValue.Background"], ThemeKeyType.SolidColorBrush)]
                        public string NewValue_Background { get; set; } = "#4060D820";

                        [JsonProperty("New Value (Foreground)"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.NewValue.Foreground"], ThemeKeyType.SolidColorBrush)]
                        public string NewValue_Foreground { get; set; } = "#D5CDCD";



                        [JsonProperty("Change Type Icon"), ThemeKey([@"Theme:UnsavedChanges.PreviewCreator.Diff.ChangeTypeIcon"], ThemeKeyType.SolidColorBrush)]
                        public string ChangeTypeIcon { get; set; } = "#808080";
                    }
                }
            }



            [JsonProperty("Square Buttons")]
            public SquareButtons_PROP SquareButtons { get; set; } = new();
            public record SquareButtons_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Disabled Buttons Shadowing"), ThemeKey([@"Theme:SquareButtons.DisabledButtonsShadowing"], ThemeKeyType.SolidColorBrush)]
                public string DisabledButtonsShadowing { get; set; } = "#77000000";


                [JsonProperty("Icons"), ThemeKey([@"Theme:SquareButtons.Icons"], ThemeKeyType.SolidColorBrush)]
                public string Icons { get; set; } = "#C4B0B6";


                [JsonProperty("ComboBox Drop-down Arrow"), ThemeKey([@"Theme:SquareButtons.ComboBox.DropDownArrow"], ThemeKeyType.SolidColorBrush)]
                public string ComboBoxDropDownArrow { get; set; } = "#9D9D9D";


                [JsonProperty("Background"), ThemeKey([@"Theme:SquareButtons.Background"], ThemeKeyType.SolidColorBrush)]
                public string Background { get; set; } = "#181818";


                [JsonProperty("Background (Pressed)"), ThemeKey([@"Theme:SquareButtons.Background.Pressed"], ThemeKeyType.SolidColorBrush)]
                public string Background_Pressed { get; set; } = "#212121";


                [JsonProperty("Background (Highlighted)"), ThemeKey([@"Theme:SquareButtons.Background.Highlighted"], ThemeKeyType.SolidColorBrush)]
                public string Background_Highlighted { get; set; } = "#282828";


                [JsonProperty("Border"), ThemeKey([@"Theme:SquareButtons.Border"], ThemeKeyType.SolidColorBrush)]
                public string Border { get; set; } = "#333333";


                [JsonProperty("Border Thickness"), ThemeKey([@"Theme:SquareButtons.Border.Thickness"], ThemeKeyType.Thickness)]
                public double[] BorderThickness { get; set; } = [1.7];


                [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:SquareButtons.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                public double[] BorderCornerRadius { get; set; } = [0];
            }



            [JsonProperty("Elongated Buttons")]
            public ElongatedButtons_PROP ElongatedButtons { get; set; } = new();
            public record ElongatedButtons_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Disabled Buttons Shadowing"), ThemeKey([@"Theme:ElongatedButtons.DisabledButtonsShadowing"], ThemeKeyType.SolidColorBrush)]
                public string DisabledButtonsShadowing { get; set; } = "#77000000";


                [JsonProperty("Background"), ThemeKey([@"Theme:ElongatedButtons.Background"], ThemeKeyType.SolidColorBrush)]
                public string Background { get; set; } = "#181818";


                [JsonProperty("Background (Pressed)"), ThemeKey([@"Theme:ElongatedButtons.Background.Pressed"], ThemeKeyType.SolidColorBrush)]
                public string Background_Pressed { get; set; } = "#212121";


                [JsonProperty("Background (Highlighted)"), ThemeKey([@"Theme:ElongatedButtons.Background.Highlighted"], ThemeKeyType.SolidColorBrush)]
                public string Background_Highlighted { get; set; } = "#282828";


                [JsonProperty("Border"), ThemeKey([@"Theme:ElongatedButtons.Border"], ThemeKeyType.SolidColorBrush)]
                public string Border { get; set; } = "#333333";


                [JsonProperty("Border Thickness"), ThemeKey([@"Theme:ElongatedButtons.Border.Thickness"], ThemeKeyType.Thickness)]
                public double[] BorderThickness { get; set; } = [1.7];


                [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:ElongatedButtons.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                public double[] BorderCornerRadius { get; set; } = [0];
            }



            [JsonProperty("Other Border-like things")]
            public OtherBorderLikeThings_PROP OtherBorderLikeThings { get; set; } = new();
            public record OtherBorderLikeThings_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Background"), ThemeKey([@"Theme:OtherBorderLikeThings.Background"], ThemeKeyType.SolidColorBrush)]
                public string Background { get; set; } = "#181818";


                [JsonProperty("Border"), ThemeKey([@"Theme:OtherBorderLikeThings.Border"], ThemeKeyType.SolidColorBrush)]
                public string Border { get; set; } = "#333333";


                [JsonProperty("Border Thickness"), ThemeKey([@"Theme:OtherBorderLikeThings.Border.Thickness"], ThemeKeyType.Thickness)]
                public double[] BorderThickness { get; set; } = [1.7];


                [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:OtherBorderLikeThings.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                public double[] BorderCornerRadius { get; set; } = [0];
            }



            [JsonProperty("Scrollbars")]
            public Scrollbars_PROP Scrollbars { get; set; } = new();
            public record Scrollbars_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Line"), ThemeKey([@"Theme:ScrollBars.Line"], ThemeKeyType.SolidColorBrush)]
                public string Line { get; set; } = "#333333";


                [JsonProperty("Line Thickness"), ThemeKey([@"Theme:ScrollBars.Line.Thickness"], ThemeKeyType.DoesntMatter)]
                public double LineThickness { get; set; } = 1.2;



                [JsonProperty("Up/Down Buttons")]
                public UpDownButtons_PROP UpDownButtons { get; set; } = new();
                public record UpDownButtons_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Icon"), ThemeKey([@"Theme:ScrollBars.UpDownButtons.Icon"], ThemeKeyType.SolidColorBrush)]
                    public string Icon { get; set; } = "#DEDEDE";


                    [JsonProperty("Background"), ThemeKey([@"Theme:ScrollBars.UpDownButtons.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#181818";


                    [JsonProperty("Background (Pressed)"), ThemeKey([@"Theme:ScrollBars.UpDownButtons.Background.Pressed"], ThemeKeyType.SolidColorBrush)]
                    public string Background_Pressed { get; set; } = "#282828";


                    [JsonProperty("Background (Highlighted)"), ThemeKey([@"Theme:ScrollBars.UpDownButtons.Background.Highlighted"], ThemeKeyType.SolidColorBrush)]
                    public string Background_Highlighted { get; set; } = "#282828";


                    [JsonProperty("Border"), ThemeKey([@"Theme:ScrollBars.UpDownButtons.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#333333";


                    [JsonProperty("Border Thickness"), ThemeKey([@"Theme:ScrollBars.UpDownButtons.Border.Thickness"], ThemeKeyType.Thickness)]
                    public double[] Border_Thickness { get; set; } = [1.7];


                    [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:ScrollBars.UpDownButtons.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] Border_CornerRadius { get; set; } = [1.5];
                }



                [JsonProperty("Thumb")]
                public Thumb_PROP Thumb { get; set; } = new();
                public record Thumb_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Background"), ThemeKey([@"Theme:ScrollBars.Thumb.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#181818";


                    [JsonProperty("Background (Pressed)"), ThemeKey([@"Theme:ScrollBars.Thumb.Background.Pressed"], ThemeKeyType.SolidColorBrush)]
                    public string Background_Pressed { get; set; } = "#282828";


                    [JsonProperty("Background (Highlighted)"), ThemeKey([@"Theme:ScrollBars.Thumb.Background.Highlighted"], ThemeKeyType.SolidColorBrush)]
                    public string Background_Highlighted { get; set; } = "#282828";


                    [JsonProperty("Border"), ThemeKey([@"Theme:ScrollBars.Thumb.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#333333";


                    [JsonProperty("Border Thickness"), ThemeKey([@"Theme:ScrollBars.Thumb.Border.Thickness"], ThemeKeyType.Thickness)]
                    public double[] Border_Thickness { get; set; } = [1.7];


                    [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:ScrollBars.Thumb.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] Border_CornerRadius { get; set; } = [1.5];
                }
            }



            [JsonProperty("Sliders")]
            public Sliders_PROP Sliders { get; set; } = new();
            public record Sliders_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Thumb")]
                public Thumb_PROP Thumb { get; set; } = new();
                public record Thumb_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Background"), ThemeKey([@"Theme:Slider.Thumb.Default.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#B8B3B1";

                    [JsonProperty("Background (Pressed)"), ThemeKey([@"Theme:Slider.Thumb.Pressed.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background_Pressed { get; set; } = "#B8B3B1";

                    [JsonProperty("Background (Highlighted)"), ThemeKey([@"Theme:Slider.Thumb.Highlighted.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background_Highlighted { get; set; } = "#B8B3B1";
                    

                    [JsonProperty("Border"), ThemeKey([@"Theme:Slider.Thumb.Default.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#888381";

                    [JsonProperty("Border (Pressed)"), ThemeKey([@"Theme:Slider.Thumb.Pressed.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border_Pressed { get; set; } = "#888381";
                    
                    [JsonProperty("Border (Highlighted)"), ThemeKey([@"Theme:Slider.Thumb.Highlighted.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border_Highlighted { get; set; } = "#888381";


                    [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:Slider.Thumb.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] CornerRadius { get; set; } = [1];

                    [JsonProperty("Border Thickness"), ThemeKey([@"Theme:Slider.Thumb.BorderThickness"], ThemeKeyType.Thickness)]
                    public double[] BorderThickness { get; set; } = [0.8];


                    [JsonProperty("Width"), ThemeKey([@"Theme:Slider.Thumb.Width"], ThemeKeyType.DoesntMatter)]
                    public double Width { get; set; } = 11;

                    [JsonProperty("Height"), ThemeKey([@"Theme:Slider.Thumb.Height"], ThemeKeyType.DoesntMatter)]
                    public double Height { get; set; } = 18;

                    [JsonProperty("Scale"), ThemeKey([@"Theme:Slider.Thumb.Scale"], ThemeKeyType.DoesntMatter)]
                    public double Scale { get; set; } = 1.0;
                }


                [JsonProperty("Line")]
                public Line_PROP Line { get; set; } = new();
                public record Line_PROP : ThemeDefinitionSection
                {
                    [JsonProperty("Height"), ThemeKey([@"Theme:Slider.Line.Height"], ThemeKeyType.DoesntMatter)]
                    public double Height { get; set; } = 3;


                    [JsonProperty("Border Thickness"), ThemeKey([@"Theme:Slider.Line.BorderThickness"], ThemeKeyType.Thickness)]
                    public double[] BorderThickness { get; set; } = [1];

                    [JsonProperty("Border Corner Radius"), ThemeKey([@"Theme:Slider.Line.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] CornerRadius { get; set; } = [0];


                    [JsonProperty("Background"), ThemeKey([@"Theme:Slider.Line.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#B0ADAC";

                    [JsonProperty("Border"), ThemeKey([@"Theme:Slider.Line.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#A6A09E";
                }
            }



            [JsonProperty("Json Editor Syntax")]
            public JsonEditorSyntax_PROP JsonEditorSyntax { get; set; } = new();
            public record JsonEditorSyntax_PROP : ThemeDefinitionSection
            {
                [JsonProperty("Tags")]
                public string Tags { get; set; } = "#CCCCCC";

                [JsonProperty("Tags Value")]
                public string TagsValue { get; set; } = "#F54927";

                [JsonProperty("[TabExplain]")]
                public string TabExplainColor { get; set; } = "#2779F5";
            }



            [JsonProperty("Color Keys for Translation")]
            public ColorKeysForTranslation_PROP ColorKeysForTranslation { get; set; } = new();
            public record ColorKeysForTranslation_PROP : ThemeDefinitionSection
            {
                [JsonProperty("{Theme.HyperlinkColor}")]
                public string HyperlinkColor { get; set; } = "#569CD6";

                [JsonProperty("{Theme.SelectedFileNameColor}")]
                public string SelectedFileNameColor { get; set; } = "#FC5A03";

                [JsonProperty("{Theme.UnsavedChanges.IDColor}")]
                public string UnsavedChanges_IDColor { get; set; } = "#F8C200";

                [JsonProperty("{Theme.UnsavedChanges.NameColor}")]
                public string UnsavedChanges_NameColor { get; set; } = "#AFBFF9";

                [JsonProperty("{Theme.TooltipHints}")]
                public string TooltipHints { get; set; } = "#919188";


                [JsonProperty("{Theme.IColor0}")] public string IColor0 { get; set; } = "#FFFFFF";
                [JsonProperty("{Theme.IColor1}")] public string IColor1 { get; set; } = "#FFFFFF";
                [JsonProperty("{Theme.IColor2}")] public string IColor2 { get; set; } = "#FFFFFF";
                [JsonProperty("{Theme.IColor3}")] public string IColor3 { get; set; } = "#FFFFFF";
                [JsonProperty("{Theme.IColor4}")] public string IColor4 { get; set; } = "#FFFFFF";
                [JsonProperty("{Theme.IColor5}")] public string IColor5 { get; set; } = "#FFFFFF";
                [JsonProperty("{Theme.IColor6}")] public string IColor6 { get; set; } = "#FFFFFF";
                [JsonProperty("{Theme.IColor7}")] public string IColor7 { get; set; } = "#FFFFFF";
                [JsonProperty("{Theme.IColor8}")] public string IColor8 { get; set; } = "#FFFFFF";
                [JsonProperty("{Theme.IColor9}")] public string IColor9 { get; set; } = "#FFFFFF";


                public string Apply(string OriginalString, string? RelatedUITranslationEntryUID = null)
                {
                    string TransformedString = OriginalString;

                    foreach (KeyValuePair<string, PropertyInfo> Keys in ColorProperties)
                    {
                        TransformedString = TransformedString.Replace(Keys.Key, (string)Keys.Value.GetValue(this)!);
                    }

                    if (RelatedUITranslationEntryUID is not null && @Languages.@LoadedStaticTextModifiersWithThemeKeys.Contains(RelatedUITranslationEntryUID) == false)
                    {
                        if (TransformedString != OriginalString)
                        {
                            @Languages.@LoadedStaticTextModifiersWithThemeKeys.Add(RelatedUITranslationEntryUID);
                        }
                    }

                    return TransformedString;
                }

                private Dictionary<string, PropertyInfo> ColorProperties = [];
                public ColorKeysForTranslation_PROP()
                {
                    foreach (PropertyInfo ColorKey in this.GetType().GetProperties())
                    {
                        if (ColorKey.HasAttribute(out JsonPropertyAttribute JsonProperty)) ColorProperties[JsonProperty.PropertyName!] = ColorKey;
                    }
                }
            }
        }
    }
}