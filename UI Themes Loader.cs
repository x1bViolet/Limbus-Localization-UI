using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.UILanguageLoader;

namespace LC_Localization_Task_Absolute
{
    internal abstract class UIThemesLoader
    {
        internal protected static Theme LoadedTheme;

        internal protected static dynamic FormalTaskCompleted = null;
        
        internal protected static Task ChangeBackgroundImageShadowTransperacy(string BackgroundImageTransperacy)
        {
            MainControl.BackgroundImageShadowingColor.Background = ToSolidColorBrush(BackgroundImageTransperacy);

            return FormalTaskCompleted;
        }

        /// <summary>
        /// Get FontFamily from file or just as <c>new FontFamily(Name)</c>
        /// </summary>
        internal protected static FontFamily FindFontFamilyThere(string SomePath)
        {
            System.Windows.Media.FontFamily Finded = new FontFamily();

            if (File.Exists(SomePath))
            {
                Finded = FileToFontFamily(SomePath);
            }
            else
            {
                Finded = new System.Windows.Media.FontFamily(SomePath);
            }

            return Finded;
        }

        internal protected class Theme
        {
            [JsonProperty("Hide Background while Minimal Width")]
            public bool AutoHideBackgroundOnMinWidth { get; set; }

            [JsonProperty("Background Image Shadow")]
            public string BackgroundImageShadow { get; set; }

            [JsonProperty("Object Name Font Color")]
            public string ObjectNameFontColor { get; set; }

            [JsonProperty("Navigation Panel Background")]
            public string NavigationPanelBackground { get; set; }
            
            [JsonProperty("Textfields Shadow Font Color")]
            public string TextfieldsShadowFontColor { get; set; }

            [JsonProperty("UI Text Font Color")]
            public string UITextFontColor { get; set; }
            
            [JsonProperty("UI Text Font Color Individuals")]
            public List<IndividualFontColor> UITextFontColorIndividuals { get; set; }

            [JsonProperty("Buttons")]
            public ButtonsThemeControl ButtonsThemeControl { get; set; }

            [JsonProperty("Borders")]
            public BordersThemeControl BordersThemeControl { get; set; }

            [JsonProperty("Right Menu Textfields")]
            public RightMenuTextfieldsControl RightMenuTextfieldsThemeControl { get; set; }

            [JsonProperty("Editor")]
            public EditorSettings Editor { get; set; }

            [JsonProperty("Editor Context Menu")]
            public EditorContextMenuSettings EditorContextMenu { get; set; }

            [OnDeserialized]
            internal void OnInit(StreamingContext context)
            {
                ChangeBackgroundImageShadowTransperacy(BackgroundImageShadow);
                MainControl.Resources["UITheme_ObjectNameColor"] = ToSolidColorBrush(ObjectNameFontColor);
            }
        }
        internal protected class IndividualFontColor
        {
            [JsonProperty("Element ID")]
            public string ElementID { get; set; }

            [JsonProperty("Font Color")]
            public string FontColor { get; set; }
        }

        internal protected class RightMenuTextfieldsControl
        {
            [JsonProperty("Font Color")]
            public string FontColor { get; set; }

            [JsonProperty("Selection Color")]
            public string SelectionColor { get; set; }

            [JsonProperty("Caret Color")]
            public string CaretColor { get; set; }

            [OnDeserialized]
            internal void OnInit(StreamingContext context)
            {
                SolidColorBrush ApplyFontColor = ToSolidColorBrush(FontColor);
                SolidColorBrush ApplySelectionColor = ToSolidColorBrush(SelectionColor);
                SolidColorBrush ApplyCaretColor = ToSolidColorBrush(CaretColor);
                foreach(KeyValuePair<string, TextBox> UITextfield in MainWindow.UITextfieldElements)
                {
                    UITextfield.Value.Foreground = ApplyFontColor;
                    UITextfield.Value.SelectionBrush = ApplySelectionColor;
                    UITextfield.Value.CaretBrush = ApplyCaretColor;
                }
            }
        }

        internal protected class ButtonsThemeControl
        {
            public string Background { get; set; }

            [JsonProperty("Background (Highlighted)")]
            public string BackgroundHighlighted { get; set; } = "";
            public string Border { get; set; } = "";

            [JsonProperty("Disable Cover Color")]
            public string DisableCover { get; set; } = "";

            [JsonProperty("Corner Radius")]
            public double CornerRadius { get; set; }

            [JsonProperty("Border Thickness")]
            public double BorderThickness { get; set; }

            [JsonProperty("Icons Color")]
            public string IconsColor { get; set; } = "";
        }
        internal protected class BordersThemeControl
        {
            [JsonProperty("Background")]
            public string Background { get; set; } = "";

            [JsonProperty("Border")]
            public string Border { get; set; } = "";

            [JsonProperty("Corner Radius")]
            public double CornerRadius { get; set; }

            [JsonProperty("Border Thickness")]
            public double BorderThickness { get; set; }
        }

        internal protected class EditorSettings
        {
            [JsonProperty("Font Size")]
            public double FontSize { get; set; }

            [JsonProperty("Font")]
            public string FontFamily { get; set; } = "";

            [JsonProperty("Font Color")]
            public string FontColor { get; set; } = "";

            [JsonProperty("Font Weight")]
            public string FontWeight { get; set; } = "";

            [JsonProperty("Background")]
            public string Background { get; set; } = "";

            [JsonProperty("Border")]
            public string Border { get; set; } = "";

            [JsonProperty("Corner Radius")]
            public double CornerRadius { get; set; }

            [JsonProperty("Caret Color")]
            public string CaretColor { get; set; } = "";

            [JsonProperty("Selection Color")]
            public string SelectionColor { get; set; } = "";

            [OnDeserialized]
            internal void ChangeEditorStyle(StreamingContext context)
            {
                if (UILanguageLoader.LoadedFontFamilies.ContainsKey(FontFamily))
                {
                    MainControl.Editor.FontFamily = LoadedFontFamilies[FontFamily];
                }
                else
                {
                    MainControl.Editor.FontFamily = FindFontFamilyThere(FontFamily);
                }

                MainControl.Editor.FontSize = FontSize;
                MainControl.Editor.FontWeight = WeightFrom(FontWeight);
                MainControl.Editor.Foreground = ToSolidColorBrush(FontColor);
                MainControl.Editor.CaretBrush = ToSolidColorBrush(CaretColor);
                MainControl.Editor.SelectionBrush = ToSolidColorBrush(SelectionColor);

                MainControl.Editor_Background.Background = ToSolidColorBrush(Background);
                MainControl.Editor_Background.BorderBrush = ToSolidColorBrush(Border);
                MainControl.Editor_Background.CornerRadius = new CornerRadius(CornerRadius);
            }
        }

        internal protected class EditorContextMenuSettings
        {
            [JsonProperty("Font")]
            public string Font { get; set; } = "";

            [JsonProperty("Font Weight")]
            public string FontWeight { get; set; } = "";

            [JsonProperty("Background")]
            public string Background { get; set; } = "";

            [JsonProperty("Background (Highlighted)")]
            public string BackgroundHighlighted { get; set; } = "";

            [JsonProperty("Border")]
            public string Border { get; set; } = "";

            [JsonProperty("Font Color")]
            public string FontColor { get; set; } = "";

            [JsonProperty("Corner Radius")]
            public double CornerRadius { get; set; }
        }

        internal protected static void InitializeUITheme(string FolderName)
        {
            if (File.Exists(@$"{FolderName}\Theme.json"))
            {
                LoadedTheme = JsonConvert.DeserializeObject<Theme>(File.ReadAllText(@$"{FolderName}\Theme.json"));
            }
            else
            {
                LoadedTheme = JsonConvert.DeserializeObject<Theme>(MainWindow.DefaultTheme);
            }

            if (File.Exists(@$"{FolderName}\Background.png"))
            {
                MainControl.BackgroundImage.Source = GenerateBitmapFromFile(@$"{FolderName}\Background.png");
                MainControl.BackgroundImage.Visibility = Visibility.Visible;
            }
            else
            {
                MainControl.BackgroundImage.Source = new BitmapImage();
                MainControl.BackgroundImage.Visibility = Visibility.Collapsed;
            }

            MainControl.Resources["UITheme_NavigationPanelBackground"] = ToSolidColorBrush(LoadedTheme.NavigationPanelBackground);
            MainControl.Resources["UITheme_DefaultTextColor"]          = ToSolidColorBrush(LoadedTheme.UITextFontColor);

            if (LoadedTheme.ButtonsThemeControl != null)
            {
                MainControl.Resources["UITheme_ButtonsBackground"]            = ToSolidColorBrush(LoadedTheme.ButtonsThemeControl.Background);
                MainControl.Resources["UITheme_ButtonsBorder"]                = ToSolidColorBrush(LoadedTheme.ButtonsThemeControl.Border);
                MainControl.Resources["UITheme_ButtonsBackgroundHighlighted"] = ToSolidColorBrush(LoadedTheme.ButtonsThemeControl.BackgroundHighlighted);
                MainControl.Resources["UITheme_ButtonsDisableCover"]          = ToSolidColorBrush(LoadedTheme.ButtonsThemeControl.DisableCover);

                MainControl.Resources["UITheme_DefaultButtonIconsColor"] = ToSolidColorBrush(LoadedTheme.ButtonsThemeControl.IconsColor);
                MainControl.Resources["UITheme_DefaultShadowTextColor"]  = ToSolidColorBrush(LoadedTheme.TextfieldsShadowFontColor);

                MainControl.Resources["UITheme_ButtonsCornerRadius"]    = new CornerRadius(LoadedTheme.ButtonsThemeControl.CornerRadius);
                MainControl.Resources["UITheme_ButtonsBorderThickness"] = new Thickness(LoadedTheme.ButtonsThemeControl.BorderThickness);
            }

            if (LoadedTheme.BordersThemeControl != null)
            {
                MainControl.Resources["UITheme_BordersBackground"]  = ToSolidColorBrush(LoadedTheme.BordersThemeControl.Background);
                MainControl.Resources["UITheme_BordersBorderColor"] = ToSolidColorBrush(LoadedTheme.BordersThemeControl.Border);

                MainControl.Resources["UITheme_BordersCornerRadius"]    = new CornerRadius(LoadedTheme.BordersThemeControl.CornerRadius);
                MainControl.Resources["UITheme_BordersBorderThickness"] = new Thickness(LoadedTheme.BordersThemeControl.BorderThickness);
            }

            if (LoadedTheme.EditorContextMenu != null)
            {
                MainControl.Resources["UITheme_EditorContextMenu_Border"]                = ToSolidColorBrush(LoadedTheme.EditorContextMenu.Border);
                MainControl.Resources["UITheme_EditorContextMenu_Background"]            = ToSolidColorBrush(LoadedTheme.EditorContextMenu.Background);
                MainControl.Resources["UITheme_EditorContextMenu_BackgroundHighlighted"] = ToSolidColorBrush(LoadedTheme.EditorContextMenu.BackgroundHighlighted);
                MainControl.Resources["UITheme_EditorContextMenu_FontColor"]             = ToSolidColorBrush(LoadedTheme.EditorContextMenu.FontColor);
                MainControl.Resources["UITheme_EditorContextMenu_CornerRadius"]          = new CornerRadius(LoadedTheme.EditorContextMenu.CornerRadius);

                if (UILanguageLoader.LoadedFontFamilies.ContainsKey(LoadedTheme.EditorContextMenu.Font))
                {
                    MainControl.Resources["UITheme_EditorContextMenu_FontFamily"] = LoadedFontFamilies[LoadedTheme.EditorContextMenu.Font];
                }
                else
                {
                    MainControl.Resources["UITheme_EditorContextMenu_FontFamily"] = FindFontFamilyThere(LoadedTheme.EditorContextMenu.Font);
                }

                MainControl.Resources["UITheme_EditorContextMenu_Font"]       = ToSolidColorBrush(LoadedTheme.EditorContextMenu.Font);
                MainControl.Resources["UITheme_EditorContextMenu_FontWeight"] = WeightFrom(LoadedTheme.EditorContextMenu.FontWeight);
            }

            if (LoadedTheme.UITextFontColorIndividuals != null)
            {
                foreach(IndividualFontColor IndividualFontColor in LoadedTheme.UITextFontColorIndividuals)
                {
                    if (IndividualFontColor.ElementID != null)
                    {
                        if (AbleToSetForegroundByTheme.ContainsKey(IndividualFontColor.ElementID))
                        {
                            AbleToSetForegroundByTheme[IndividualFontColor.ElementID].Foreground = ToSolidColorBrush(IndividualFontColor.FontColor);
                        }
                    }
                }
            }
        }
    }
}
