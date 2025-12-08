using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;

namespace LC_Localization_Task_Absolute
{
    public static class ᐁ_Interface_Themes_Loader
    {
        public static ResourceDictionary ThemeKeysDictionary = Application.Current.Resources.MergedDictionaries[0]; // ᐁ UI Theme Keys.xaml

        public static InterfaceThemeModifiers.Visual LoadedTheme = new();

        public static void ModifyUI(string ThemeInfoPath)
        {
            if (Directory.Exists(ThemeInfoPath) && File.Exists($"{ThemeInfoPath}/Visual.json"))
            {
                LoadedTheme = new FileInfo($"{ThemeInfoPath}/Visual.json").Deserealize<InterfaceThemeModifiers.Visual>();

                InterfaceThemeModifiers.SetResourceKeysFromAttribute(LoadedTheme);
            }
        }

        public enum ThemeKeyType
        {
            Thickness,
            CornerRadius,
            SolidColorBrush,
            FontFamily,
            FontWeight,
            DoesntMatter,
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class ThemeSectionAttribute : Attribute;

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class ThemeKeyAttribute(string[] Key, ThemeKeyType Type) : Attribute
        {
            public string ResourceKey { get; } = Key[0]; // :p
            public ThemeKeyType ResourceType { get; } = Type;
        }
        public static class InterfaceThemeModifiers
        {
            public static void SetResourceKeysFromAttribute(object JsonSection)
            {
                foreach (PropertyInfo JsonProperty in JsonSection.GetType().GetProperties())
                {
                    if (JsonProperty.HasAttribute(out ThemeKeyAttribute ThemeKey))
                    {
                        if (!ThemeKeysDictionary.Contains(ThemeKey.ResourceKey)) throw new Exception($"Unknown theme key \"{ThemeKey.ResourceKey}\"");
                        object ThemeKeyValue = JsonProperty.GetValue(JsonSection);  //rin ($"'{ThemeKey.ResourceKey}' -> {ThemeKeyValue}");
                        ThemeKeysDictionary[ThemeKey.ResourceKey] = ThemeKey.ResourceType switch
                        {
                            ThemeKeyType.Thickness => ThicknessFrom((double[])ThemeKeyValue),
                            ThemeKeyType.CornerRadius => CornerRadiusFrom((double[])ThemeKeyValue),
                            ThemeKeyType.SolidColorBrush => ToSolidColorBrush((string)ThemeKeyValue),
                            ThemeKeyType.FontFamily => FileToFontFamily((string)ThemeKeyValue),
                            ThemeKeyType.FontWeight => WeightFrom((string)ThemeKeyValue),
                            ThemeKeyType.DoesntMatter => ThemeKeyValue,
                        };
                    }
                    else if (JsonProperty.HasAttribute<ThemeSectionAttribute>())
                    {
                        SetResourceKeysFromAttribute(JsonProperty.GetValue(JsonSection)); // Go to subsections
                    }
                }
            }

            /// <summary>
            /// Visual.json theme file
            /// </summary>
            public record Visual
            {
                [ThemeSection]
                [JsonProperty("Title Bar")]
                public TitleBar_PROP TitleBar { get; set; } = new();
                public record TitleBar_PROP
                {
                    [JsonProperty("Background")]
                    [ThemeKey([@"Theme:TitleBar.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; }

                    [JsonProperty("Icons")]
                    [ThemeKey([@"Theme:TitleBar.Icons"], ThemeKeyType.SolidColorBrush)]
                    public string Icons { get; set; }

                    [JsonProperty("Titles Foreground")]
                    [ThemeKey([@"Theme:TitleBar.Titles"], ThemeKeyType.SolidColorBrush)]
                    public string TitlesForeground { get; set; }

                    [JsonProperty("Buttons Highlight")]
                    [ThemeKey([@"Theme:TitleBar.Buttons.Highlighted"], ThemeKeyType.SolidColorBrush)]
                    public string ButtonsHighlight { get; set; }

                    [JsonProperty("Buttons Highlight (Exit button)")]
                    [ThemeKey([@"Theme:TitleBar.Buttons.Highlighted.ExitButton"], ThemeKeyType.SolidColorBrush)]
                    public string ButtonsHighlight_ExitButton { get; set; }
                }

                [ThemeSection]
                [JsonProperty("Common")]
                public Common_PROP Common { get; set; }
                public record Common_PROP
                {
                    [JsonProperty("Background Image")]
                    public string BackgroundImagePath { get; set; } = "x:\\wafldps[dmpqo";


                    [JsonProperty("Background Color")]
                    public string BackgroundColor { get; set; } = "#0F0F0F";

                    
                    [JsonProperty("Background Image Shadow")]
                    public string BackgroundImageShadow { get; set; } = "#0F0F0F";

                    [JsonProperty("Background Image Shadow (Settings)")]
                    public string BackgroundImageShadow_Settings { get; set; } = "#191818";

                    [JsonProperty("Background Image Shadow (Identity/E.G.O Preview creator)")]
                    public string BackgroundImageShadow_PreviewCreator { get; set; } = "#00000000";


                    [JsonProperty("Hide Background Image with minimum window width")]
                    public bool HideBackgroundImageWithMinimumWindowWidth { get; set; } = true;


                    [JsonProperty("Right Menu Navigation Panel Background")]
                    [ThemeKey([@"Theme:Common.RightMenu.NavigationPanel.Background"], ThemeKeyType.SolidColorBrush)]
                    public string RightMenuNavigationPanelBackground { get; set; } = "#00000000";

                    [JsonProperty("Right Menu Object name Foreground")]
                    [ThemeKey([@"Theme:Common.RightMenuObjectName.Foreground"], ThemeKeyType.SolidColorBrush)]
                    public string RightMenuObjectNameForeground { get; set; } = "#EBCAA2";

                    [JsonProperty("Right Menu Uptie buttons background")]
                    [ThemeKey([@"Theme:Common.RightMenuUptieButtons.Background"], ThemeKeyType.SolidColorBrush)]
                    public string RightMenuUptieButtonsBackground { get; set; } = "#0E0907";

                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        if (File.Exists(BackgroundImagePath))
                        {
                            _ = MainControl.BackgroundImage.Source
                              = ConfigurationWindow.ConfigControl.BackgroundImage.Source
                              = BitmapFromFile(BackgroundImagePath);

                            MainControl.PreviewCreatorThemeImageShadow.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            _ = ConfigurationWindow.ConfigControl.BackgroundImage.Source
                              = MainControl.BackgroundImage.Source
                              = new BitmapImage();

                            MainControl.PreviewCreatorThemeImageShadow.Visibility = Visibility.Collapsed;
                        }

                        MainControl.MainBackgroundGrid.Background = ToSolidColorBrush(BackgroundColor);

                        MainControl.BackgroundImageShadowingColor.Background = ToSolidColorBrush(BackgroundImageShadow);
                        ConfigurationWindow.ConfigControl.BackgroundImageShadowing.Background = ToSolidColorBrush(BackgroundImageShadow_Settings);
                        MainControl.PreviewCreatorThemeImageShadow.Background = ToSolidColorBrush(BackgroundImageShadow_PreviewCreator);
                    }
                }

                [ThemeSection]
                [JsonProperty("UI Text")]
                public UIText_PROP UIText { get; set; }
                public record UIText_PROP
                {
                    [JsonProperty("Foreground")]
                    [ThemeKey([@"Theme:UIText.Foreground"], ThemeKeyType.SolidColorBrush)]
                    public string Foreground { get; set; } = "#A69885";

                    [JsonProperty("Foreground (Settings)")]
                    [ThemeKey([@"Theme:UIText.Foreground.Settings"], ThemeKeyType.SolidColorBrush)]
                    public string Foreground_Settings { get; set; } = "#FAFAFA";

                    [JsonProperty("Foreground (Settings (Comments))")]
                    [ThemeKey([@"Theme:UIText.Foreground.Settings.Comments"], ThemeKeyType.SolidColorBrush)]
                    public string Foreground_SettingsComments { get; set; } = "#C7BCBC";

                    [JsonProperty("Foreground (Identity/E.G.O Preview creator)")]
                    [ThemeKey([@"Theme:UIText.Foreground.IdentityOrEGOPreviewCreator"], ThemeKeyType.SolidColorBrush)]
                    public string Foreground_IdentityOrEGOPreviewCreator { get; set; } = "#F5F5DC";
                }

                [ThemeSection]
                [JsonProperty("UI Textfields")]
                public UITextfields_PROP UITextfields { get; set; }
                public record UITextfields_PROP
                {
                    [JsonProperty("Foreground")]
                    [ThemeKey([@"Theme:UITextfields.Foreground"], ThemeKeyType.SolidColorBrush)]
                    public string Foreground { get; set; } = "#B1B1B9";

                    [JsonProperty("Foreground (Shadow Text)")]
                    [ThemeKey([@"Theme:UITextfields.Foreground.ShadowText"], ThemeKeyType.SolidColorBrush)]
                    public string Foreground_ShadowText { get; set; } = "#514C46";


                    [JsonProperty("Background")]
                    [ThemeKey([@"Theme:UITextfields.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#55B4B4B4";


                    [JsonProperty("Border")]
                    [ThemeKey([@"Theme:UITextfields.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#66D4D4D4";

                    [JsonProperty("Border Thickness")]
                    [ThemeKey([@"Theme:UITextfields.Border.Thickness"], ThemeKeyType.Thickness)]
                    public double[] BorderThickness { get; set; } = [1.7];

                    [JsonProperty("Border Corner Radius")]
                    [ThemeKey([@"Theme:UITextfields.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] BorderCornerRadius { get; set; } = [0];


                    [JsonProperty("Selection")]
                    [ThemeKey([@"Theme:UITextfields.Selection"], ThemeKeyType.SolidColorBrush)]
                    public string Selection { get; set; } = "#C4B0B6";

                    [JsonProperty("Caret")]
                    [ThemeKey([@"Theme:UITextfields.Caret"], ThemeKeyType.SolidColorBrush)]
                    public string Caret { get; set; } = "#C4B0B6";


                    [ThemeSection]
                    [JsonProperty("Special textfields")]
                    public SpecialTextfields_PROP SpecialTextfields { get; set; } = new();
                    public record SpecialTextfields_PROP
                    {
                        [JsonProperty("Selection")]
                        [ThemeKey([@"Theme:UITextfields.Spec.Selection"], ThemeKeyType.SolidColorBrush)]
                        public string Selection { get; set; } = "#66C4B0B6";

                        [JsonProperty("Caret")]
                        [ThemeKey([@"Theme:UITextfields.Spec.Caret"], ThemeKeyType.SolidColorBrush)]
                        public string Caret { get; set; } = "#FFFFFF";


                        [JsonProperty("Syntax highlightion 1")]
                        public string SyntaxHighlight1 { get; set; } = "#CCCCCC";

                        [JsonProperty("Syntax highlightion 2")]
                        public string SyntaxHighlight2 { get; set; } = "#CCCCCC";

                        [JsonProperty("Syntax highlightion 3")]
                        public string SyntaxHighlight3 { get; set; } = "#CCCCCC";

                        [JsonProperty("Syntax highlightion 4")]
                        public string SyntaxHighlight4 { get; set; } = "#CCCCCC";

                    }
                }

                [ThemeSection]
                [JsonProperty("Json Text Editor")]
                public JsonTextEditor_PROP JsonTextEditorSettings { get; set; }
                public record JsonTextEditor_PROP
                {
                    [JsonProperty("Show Line Numbers")]
                    [ThemeKey([@"Theme:JsonTextEditor.ShowLineNumbers"], ThemeKeyType.DoesntMatter)]
                    public bool ShowLineNumbers { get; set; } = true;

                    [JsonProperty("Foreground")]
                    [ThemeKey([@"Theme:JsonTextEditor.Foreground"], ThemeKeyType.SolidColorBrush)]
                    public string Foreground { get; set; } = "#ABA193";

                    [JsonProperty("Background")]
                    [ThemeKey([@"Theme:JsonTextEditor.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#191919";

                    [JsonProperty("Font")]
                    [ThemeKey([@"Theme:JsonTextEditor.Font"], ThemeKeyType.FontFamily)]
                    public string Font { get; set; } = "x:\\mf0239dnup2";

                    [JsonProperty("Font Weight")]
                    [ThemeKey([@"Theme:JsonTextEditor.FontWeight"], ThemeKeyType.FontWeight)]
                    public string FontWeight { get; set; } = "Regular";

                    [JsonProperty("FontSize")]
                    [ThemeKey([@"Theme:JsonTextEditor.FontSize"], ThemeKeyType.DoesntMatter)]
                    public double FontSize { get; set; } = 16.8;

                    [JsonProperty("Caret")]
                    [ThemeKey([@"Theme:JsonTextEditor.Caret"], ThemeKeyType.SolidColorBrush)]
                    public string Caret { get; set; } = "#E9DBC6";

                    [JsonProperty("Border")]
                    [ThemeKey([@"Theme:JsonTextEditor.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#333333";

                    [JsonProperty("Border Thickness")]
                    [ThemeKey([@"Theme:JsonTextEditor.Border.Thickness"], ThemeKeyType.Thickness)]
                    public double[] BorderThickness { get; set; } = [0];

                    [JsonProperty("Border Corner Radius")]
                    [ThemeKey([@"Theme:JsonTextEditor.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] BorderCornerRadius { get; set; } = [1];

                    [ThemeSection]
                    [JsonProperty("Selection")]
                    public Selection_PROP Selection { get; set; } = new();
                    public record Selection_PROP
                    {
                        [JsonProperty("Foreground")]
                        [ThemeKey([@"Theme:JsonTextEditor.Selection.Foreground"], ThemeKeyType.SolidColorBrush)]
                        public string Foreground { get; set; } = "#ABA193";

                        [JsonProperty("Background")]
                        [ThemeKey([@"Theme:JsonTextEditor.Selection.Background"], ThemeKeyType.SolidColorBrush)]
                        public string Background { get; set; } = "#55B4B4B4";

                        [JsonProperty("Border")]
                        [ThemeKey([@"Theme:JsonTextEditor.Selection.Border"], ThemeKeyType.SolidColorBrush)]
                        public string Border { get; set; } = "#66D4D4D4";

                        [JsonProperty("Border Thickness")]
                        [ThemeKey([@"Theme:JsonTextEditor.Selection.Border.Thickness"], ThemeKeyType.DoesntMatter)]
                        public double BorderThickness { get; set; } = 1;

                        [JsonProperty("Border Corner Radius")]
                        [ThemeKey([@"Theme:JsonTextEditor.Selection.Border.CornerRadius"], ThemeKeyType.DoesntMatter)]
                        public double BorderCornerRadius { get; set; } = 0;
                    }
                }

                [ThemeSection]
                [JsonProperty("Square Buttons")]
                public SquareButtons_PROP SquareButtons { get; set; }
                public record SquareButtons_PROP
                {
                    [JsonProperty("Disabled Buttons Shadowing")]
                    [ThemeKey([@"Theme:SquareButtons.DisabledButtonsShadowing"], ThemeKeyType.SolidColorBrush)]
                    public string DisabledButtonsShadowing { get; set; } = "#77000000";

                    [JsonProperty("Icons")]
                    [ThemeKey([@"Theme:SquareButtons.Icons"], ThemeKeyType.SolidColorBrush)]
                    public string Icons { get; set; } = "#C4B0B6";

                    [JsonProperty("Background")]
                    [ThemeKey([@"Theme:SquareButtons.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#181818";

                    [JsonProperty("Background (Pressed)")]
                    [ThemeKey([@"Theme:SquareButtons.Background.Pressed"], ThemeKeyType.SolidColorBrush)]
                    public string Background_Pressed { get; set; } = "#212121";

                    [JsonProperty("Background (Highlighted)")]
                    [ThemeKey([@"Theme:SquareButtons.Background.Highlighted"], ThemeKeyType.SolidColorBrush)]
                    public string Background_Highlighted { get; set; } = "#282828";

                    [JsonProperty("Border")]
                    [ThemeKey([@"Theme:SquareButtons.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#333333";

                    [JsonProperty("Border Thickness")]
                    [ThemeKey([@"Theme:SquareButtons.Border.Thickness"], ThemeKeyType.Thickness)]
                    public double[] BorderThickness { get; set; } = [1.7];

                    [JsonProperty("Border Corner Radius")]
                    [ThemeKey([@"Theme:SquareButtons.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] BorderCornerRadius { get; set; } = [0];
                }


                [ThemeSection]
                [JsonProperty("Elongated Buttons")]
                public ElongatedButtons_PROP ElongatedButtons { get; set; }
                public record ElongatedButtons_PROP
                {
                    [JsonProperty("Disabled Buttons Shadowing")]
                    [ThemeKey([@"Theme:ElongatedButtons.DisabledButtonsShadowing"], ThemeKeyType.SolidColorBrush)]
                    public string DisabledButtonsShadowing { get; set; } = "#77000000";

                    [JsonProperty("Text")]
                    [ThemeKey([@"Theme:ElongatedButtons.Text"], ThemeKeyType.SolidColorBrush)]
                    public string Text { get; set; } = "#C4B0B6";

                    [JsonProperty("Background")]
                    [ThemeKey([@"Theme:ElongatedButtons.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#181818";

                    [JsonProperty("Background (Pressed)")]
                    [ThemeKey([@"Theme:ElongatedButtons.Background.Pressed"], ThemeKeyType.SolidColorBrush)]
                    public string Background_Pressed { get; set; } = "#212121";

                    [JsonProperty("Background (Highlighted)")]
                    [ThemeKey([@"Theme:ElongatedButtons.Background.Highlighted"], ThemeKeyType.SolidColorBrush)]
                    public string Background_Highlighted { get; set; } = "#282828";

                    [JsonProperty("Border")]
                    [ThemeKey([@"Theme:ElongatedButtons.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#333333";

                    [JsonProperty("Border Thickness")]
                    [ThemeKey([@"Theme:ElongatedButtons.Border.Thickness"], ThemeKeyType.Thickness)]
                    public double[] BorderThickness { get; set; } = [1.7];

                    [JsonProperty("Border Corner Radius")]
                    [ThemeKey([@"Theme:ElongatedButtons.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] BorderCornerRadius { get; set; } = [0];
                }

                [ThemeSection]
                [JsonProperty("Other Border-like things")]
                public OtherBorderLikeThings_PROP OtherBorderLikeThings { get; set; }
                public record OtherBorderLikeThings_PROP
                {
                    [JsonProperty("Background")]
                    [ThemeKey([@"Theme:OtherBorderLikeThings.Background"], ThemeKeyType.SolidColorBrush)]
                    public string Background { get; set; } = "#181818";

                    [JsonProperty("Border")]
                    [ThemeKey([@"Theme:OtherBorderLikeThings.Border"], ThemeKeyType.SolidColorBrush)]
                    public string Border { get; set; } = "#333333";

                    [JsonProperty("Border Thickness")]
                    [ThemeKey([@"Theme:OtherBorderLikeThings.Border.Thickness"], ThemeKeyType.Thickness)]
                    public double[] BorderThickness { get; set; } = [1.7];

                    [JsonProperty("Border Corner Radius")]
                    [ThemeKey([@"Theme:OtherBorderLikeThings.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                    public double[] BorderCornerRadius { get; set; } = [0];
                }

                [ThemeSection]
                [JsonProperty("Scrollbars")]
                public Scrollbars_PROP Scrollbars { get; set; } = new();
                public record Scrollbars_PROP
                {
                    [JsonProperty("Line")]
                    [ThemeKey([@"Theme:ScrollBars.Line"], ThemeKeyType.SolidColorBrush)]
                    public string Line { get; set; } = "#333333";

                    [JsonProperty("Line Thickness")]
                    [ThemeKey([@"Theme:ScrollBars.Line.Thickness"], ThemeKeyType.DoesntMatter)]
                    public double LineThickness { get; set; } = 1.2;

                    [ThemeSection]
                    [JsonProperty("Up/Down Buttons")]
                    public UpDownButtons_PROP UpDownButtons { get; set; } = new();
                    public record UpDownButtons_PROP
                    {
                        [JsonProperty("Icon")]
                        [ThemeKey([@"Theme:ScrollBars.UpDownButtons.Icon"], ThemeKeyType.SolidColorBrush)]
                        public string Icon { get; set; } = "#DEDEDE";

                        [JsonProperty("Background")]
                        [ThemeKey([@"Theme:ScrollBars.UpDownButtons.Background"], ThemeKeyType.SolidColorBrush)]
                        public string Background { get; set; } = "#181818";

                        [JsonProperty("Background (Pressed)")]
                        [ThemeKey([@"Theme:ScrollBars.UpDownButtons.Background.Pressed"], ThemeKeyType.SolidColorBrush)]
                        public string Background_Pressed { get; set; } = "#282828";

                        [JsonProperty("Background (Highlighted)")]
                        [ThemeKey([@"Theme:ScrollBars.UpDownButtons.Background.Highlighted"], ThemeKeyType.SolidColorBrush)]
                        public string Background_Highlighted { get; set; } = "#282828";

                        [JsonProperty("Border")]
                        [ThemeKey([@"Theme:ScrollBars.UpDownButtons.Border"], ThemeKeyType.SolidColorBrush)]
                        public string Border { get; set; } = "#181818";

                        [JsonProperty("Border Thickness")]
                        [ThemeKey([@"Theme:ScrollBars.UpDownButtons.Border.Thickness"], ThemeKeyType.Thickness)]
                        public double[] Border_Thickness { get; set; } = [1.7];

                        [JsonProperty("Border Corner Radius")]
                        [ThemeKey([@"Theme:ScrollBars.UpDownButtons.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                        public double[] Border_CornerRadius { get; set; } = [1.5];
                    }

                    [ThemeSection]
                    [JsonProperty("Thumb")]
                    public Thumb_PROP Thumb { get; set; } = new();
                    public record Thumb_PROP
                    {
                        [JsonProperty("Background")]
                        [ThemeKey([@"Theme:ScrollBars.Thumb.Background"], ThemeKeyType.SolidColorBrush)]
                        public string Background { get; set; } = "#181818";

                        [JsonProperty("Background (Pressed)")]
                        [ThemeKey([@"Theme:ScrollBars.Thumb.Background.Pressed"], ThemeKeyType.SolidColorBrush)]
                        public string Background_Pressed { get; set; } = "#282828";

                        [JsonProperty("Background (Highlighted)")]
                        [ThemeKey([@"Theme:ScrollBars.Thumb.Background.Highlighted"], ThemeKeyType.SolidColorBrush)]
                        public string Background_Highlighted { get; set; } = "#282828";

                        [JsonProperty("Border")]
                        [ThemeKey([@"Theme:ScrollBars.Thumb.Border"], ThemeKeyType.SolidColorBrush)]
                        public string Border { get; set; } = "#181818";

                        [JsonProperty("Border Thickness")]
                        [ThemeKey([@"Theme:ScrollBars.Thumb.Border.Thickness"], ThemeKeyType.Thickness)]
                        public double[] Border_Thickness { get; set; } = [1.7];

                        [JsonProperty("Border Corner Radius")]
                        [ThemeKey([@"Theme:ScrollBars.Thumb.Border.CornerRadius"], ThemeKeyType.CornerRadius)]
                        public double[] Border_CornerRadius { get; set; } = [1.5];
                    }
                }

                [JsonProperty("Json Editor Syntax")]
                public JsonEditorSyntax_PROP JsonEditorSyntax { get; set; }
                public record JsonEditorSyntax_PROP
                {
                    [JsonProperty("Tags")]
                    public string Tags { get; set; }

                    [JsonProperty("Tags Value")]
                    public string TagsValue { get; set; }

                    [JsonProperty("[TabExplain]")]
                    public string TabExplainColor { get; set; }

                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        LimbusJsonTextSyntax.TagsBody_Color = Tags;
                        LimbusJsonTextSyntax.TagsValue_Color = TagsValue;
                        LimbusJsonTextSyntax.TabExplainColor = TabExplainColor;
                        JsonTextEditor.RecompileEditorSyntax();
                    }
                }
            }
        }
    }
    partial class ThemeDictionaryClass
    {
        void CornerClickEvent(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            Window Ram = new()
            {Background = Brushes.Black, Width = 572.8, Height = 430,
             Content = new Image() { Source = BitmapFromResource("UI/Limbus/Ram") },
             ResizeMode = ResizeMode.NoResize, WindowStyle = WindowStyle.ToolWindow};
            Ram.Show(); Ram.Focus();
        }
    }
}
