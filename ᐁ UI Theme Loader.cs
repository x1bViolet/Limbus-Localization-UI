using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Microsoft.Win32;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Json.BaseTypes;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Passives;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.Custom_Skills_Constructor;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Json.FilesIntegration;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator.ProjectFile.Sections;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;
using static System.Globalization.NumberStyles;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute
{
    public abstract class ᐁ_Interface_Themes_Loader
    {
       // private protected static ResourceType Resource<ResourceType>(string Name) where ResourceType : class => MainWindow.MainControl.FindResource(Name) as ResourceType;
        private static ResourceDictionary Resource = MainWindow.MainControl.Resources;

        //private protected static Style Common = Resource<>

        public static InterfaceThemeModifiers.Visual LoadedTheme = new InterfaceThemeModifiers.Visual();

        public static void ModifyUI(string ThemeInfoPath)
        {
            if (Directory.Exists(ThemeInfoPath))
            {
                if (File.Exists($"{ThemeInfoPath}/Visual.json")) LoadedTheme = new FileInfo($"{ThemeInfoPath}/Visual.json").Deserealize<InterfaceThemeModifiers.Visual>();
            }
        }

        public abstract class InterfaceThemeModifiers
        {
            public record Visual
            {
                [JsonProperty("Common")]
                public Frames.Common Common { get; set; }

                [JsonProperty("UI Text")]
                public Frames.UIText UIText { get; set; }

                [JsonProperty("UI Textfields")]
                public Frames.UITextfields UITextfields { get; set; }

                [JsonProperty("Json Text Editor")]
                public Frames.JsonTextEditor JsonTextEditor { get; set; }

                [JsonProperty("Square Buttons")]
                public Frames.SquareButtons SquareButtons { get; set; }

                [JsonProperty("Elongated Buttons")]
                public Frames.ElongatedButtons ElongatedButtons { get; set; }

                [JsonProperty("Other Border-like things")]
                public Frames.OtherBorderLikeThings OtherBorderLikeThings { get; set; }

                [JsonProperty("Json Editor Syntax")]
                public Frames.JsonEditorSyntax JsonEditorSyntax { get; set; }
            }

            public abstract class Frames
            {
                public record Common
                {
                    [JsonProperty("Background Image")]
                    public string BackgroundImagePath { get; set; } = "x:\\wafldps[dmpqo";

                    [JsonProperty("Background Image Shadow")]
                    public string BackgroundImageShadow { get; set; } = "#0F0F0F";

                    [JsonProperty("Hide Background Image with minimum window width")]
                    public bool HideBackgroundImageWithMinimumWindowWidth { get; set; } = true;

                    [JsonProperty("Right Menu Navigation Panel Background")]
                    public string RightMenuNavigationPanelBackground { get; set; } = "#00000000";

                    [JsonProperty("Right Menu Object name Foreground")]
                    public string RightMenuObjectNameForeground { get; set; } = "#EBCAA2";

                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        if (File.Exists(BackgroundImagePath)) MainControl.BackgroundImage.Source = BitmapFromFile(BackgroundImagePath);
                        else MainControl.BackgroundImage.Source = new BitmapImage();

                        if (BackgroundImageShadow != null) MainControl.BackgroundImageShadowingColor.Background = ToSolidColorBrush(BackgroundImageShadow);

                        Resource["Theme_Common__RightMenuNavigationPanelBackground"] = ToSolidColorBrush(RightMenuNavigationPanelBackground);
                        Resource["Theme_Common__RightMenuObjectNameForeground"] = ToSolidColorBrush(RightMenuObjectNameForeground);
                    }
                }

                public record UIText
                {
                    [JsonProperty("Foreground")]
                    public string Foreground { get; set; } = "#A69885";

                    [JsonProperty("Foreground (Identity/E.G.O Preview creator)")]
                    public string Foreground_IdentityOrEGOPreviewCreator { get; set; } = "#F5F5DC";

                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        Resource["Theme_UIText__Foreground"] = ToSolidColorBrush(Foreground);
                        Resource["Theme_UIText__Foreground_IdentityOrEGOPreviewCreator"] = ToSolidColorBrush(Foreground_IdentityOrEGOPreviewCreator);
                    }
                }

                public record UITextfields
                {
                    [JsonProperty("Foreground")]
                    public string Foreground { get; set; }

                    [JsonProperty("Foreground (Shadow Text)")]
                    public string Foreground_ShadowText { get; set; }


                    [JsonProperty("Background")]
                    public string Background { get; set; } = "#55B4B4B4";


                    [JsonProperty("Border")]
                    public string Border { get; set; } = "#66D4D4D4";

                    [JsonProperty("Border Thickness")]
                    public double[] BorderThickness { get; set; } = [1.7];

                    [JsonProperty("Border Corner Radius")]
                    public double[] BorderCornerRadius { get; set; } = [0];


                    [JsonProperty("Selection")]
                    public string Selection { get; set; }

                    [JsonProperty("Caret")]
                    public string Caret { get; set; }

                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        Resource["Theme_UITextfields__Foreground"] = ToSolidColorBrush(Foreground);
                        Resource["Theme_UITextfields__Foreground_ShadowText"] = ToSolidColorBrush(Foreground_ShadowText);

                        Resource["Theme_UITextfields__Background"] = ToSolidColorBrush(Background);

                        Resource["Theme_UITextfields__Border"] = ToSolidColorBrush(Border);
                        Resource["Theme_UITextfields__Border_Thickness"] = ThicknessFrom(BorderThickness);
                        Resource["Theme_UITextfields__Border_CornerRadius"] = CornerRadiusFrom(BorderCornerRadius);

                        Resource["Theme_UITextfields__Selection"] = ToSolidColorBrush(Selection);
                        Resource["Theme_UITextfields__Caret"] = ToSolidColorBrush(Caret);
                    }
                }

                public record JsonTextEditor
                {
                    [JsonProperty("Show Line Numbers")]
                    public bool ShowLineNumbers { get; set; } = true;

                    [JsonProperty("Foreground")]
                    public string Foreground { get; set; } = "#ABA193";

                    [JsonProperty("Background")]
                    public string Background { get; set; } = "#191919";

                    [JsonProperty("Font")]
                    public string Font { get; set; } = "x:\\mf0239dnup2";

                    [JsonProperty("Font Weight")]
                    public string FontWeight { get; set; } = "Regular";

                    [JsonProperty("FontSize")]
                    public double FontSize { get; set; } = 16.8;

                    [JsonProperty("Caret")]
                    public string Caret { get; set; } = "#E9DBC6";

                    [JsonProperty("Border")]
                    public string Border { get; set; } = "#333333";

                    [JsonProperty("Border Thickness")]
                    public double[] BorderThickness { get; set; } = [0];

                    [JsonProperty("Border Corner Radius")]
                    public double[] BorderCornerRadius { get; set; } = [1];

                    [JsonProperty("Selection")]
                    public JsonTextEditor_Selection Selection { get; set; } = new JsonTextEditor_Selection();

                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        Resource["Theme_JsonTextEditor__ShowLineNumbers"] = ShowLineNumbers;

                        Resource["Theme_JsonTextEditor__Foreground"] = ToSolidColorBrush(Foreground);
                        Resource["Theme_JsonTextEditor__Background"] = ToSolidColorBrush(Background);

                        Resource["Theme_JsonTextEditor__Font"] = FontFamilyFrom(Font);
                        Resource["Theme_JsonTextEditor__FontWeight"] = WeightFrom(FontWeight);
                        Resource["Theme_JsonTextEditor__FontSize"] = FontSize;

                        Resource["Theme_JsonTextEditor__Caret"] = ToSolidColorBrush(Caret);

                        Resource["Theme_JsonTextEditor__Border"] = ToSolidColorBrush(Border);
                        Resource["Theme_JsonTextEditor__Border_Thickness"] = ThicknessFrom(BorderThickness);
                        Resource["Theme_JsonTextEditor__Border_CornerRadius"] = CornerRadiusFrom(BorderCornerRadius);
                    }
                }
                public record JsonTextEditor_Selection
                {
                    [JsonProperty("Foreground")]
                    public string Foreground { get; set; } = "#ABA193";

                    [JsonProperty("Background")]
                    public string Background { get; set; } = "#55B4B4B4";

                    [JsonProperty("Border")]
                    public string Border { get; set; } = "#66D4D4D4";

                    [JsonProperty("Border Thickness")]
                    public double BorderThickness { get; set; } = 1;

                    [JsonProperty("Border Corner Radius")]
                    public double BorderCornerRadius { get; set; } = 0;

                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        Resource["Theme_JsonTextEditor__Selection__Foreground"] = ToSolidColorBrush(Foreground);
                        Resource["Theme_JsonTextEditor__Selection__Background"] = ToSolidColorBrush(Background);
                        Resource["Theme_JsonTextEditor__Selection__Border"] = ToSolidColorBrush(Border);
                        Resource["Theme_JsonTextEditor__Selection__Border_Thickness"] = BorderThickness;
                        Resource["Theme_JsonTextEditor__Selection__Border_CornerRadius"] = BorderCornerRadius;
                    }
                }

                public record SquareButtons
                {
                    [JsonProperty("Disabled Buttons Shadowing")]
                    public string DisabledButtonsShadowing { get; set; } = "#77000000";

                    [JsonProperty("Icons")]
                    public string Icons { get; set; } = "#C4B0B6";

                    [JsonProperty("Background")]
                    public string Background { get; set; } = "#181818";

                    [JsonProperty("Background (Highlighted)")]
                    public string Background_Highlighted { get; set; } = "#282828";

                    [JsonProperty("Border")]
                    public string Border { get; set; } = "#333333";

                    [JsonProperty("Border Thickness")]
                    public double[] BorderThickness { get; set; } = [1.7];

                    [JsonProperty("Border Corner Radius")]
                    public double[] BorderCornerRadius { get; set; } = [0];

                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        Resource["Theme_SquareButtons__DisabledButtonsShadowing"] = ToSolidColorBrush(DisabledButtonsShadowing);
                        
                        Resource["Theme_SquareButtons__Icons"] = ToSolidColorBrush(Icons);
                       
                        Resource["Theme_SquareButtons__Background"] = ToSolidColorBrush(Background);
                        Resource["Theme_SquareButtons__Background_Highlighted"] = ToSolidColorBrush(Background_Highlighted);
                        
                        Resource["Theme_SquareButtons__Border"] = ToSolidColorBrush(Border);
                        Resource["Theme_SquareButtons__Border_Thickness"] = ThicknessFrom(BorderThickness);
                        Resource["Theme_SquareButtons__Border_CornerRadius"] = CornerRadiusFrom(BorderCornerRadius);
                    }
                }

                public record ElongatedButtons
                {
                    [JsonProperty("Disabled Buttons Shadowing")]
                    public string DisabledButtonsShadowing { get; set; } = "#77000000";

                    [JsonProperty("Text")]
                    public string Text { get; set; } = "#C4B0B6";

                    [JsonProperty("Background")]
                    public string Background { get; set; } = "#181818";

                    [JsonProperty("Background (Highlighted)")]
                    public string Background_Highlighted { get; set; } = "#282828";

                    [JsonProperty("Border")]
                    public string Border { get; set; } = "#333333";

                    [JsonProperty("Border Thickness")]
                    public double[] BorderThickness { get; set; } = [1.7];

                    [JsonProperty("Border Corner Radius")]
                    public double[] BorderCornerRadius { get; set; } = [0];

                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        Resource["Theme_ElongatedButtons__DisabledButtonsShadowing"] = ToSolidColorBrush(DisabledButtonsShadowing);

                        Resource["Theme_ElongatedButtons__Text"] = ToSolidColorBrush(Text);
                                                
                        Resource["Theme_ElongatedButtons__Background"] = ToSolidColorBrush(Background);
                        Resource["Theme_ElongatedButtons__Background_Highlighted"] = ToSolidColorBrush(Background_Highlighted);
                        
                        Resource["Theme_ElongatedButtons__Border"] = ToSolidColorBrush(Border);
                        Resource["Theme_ElongatedButtons__Border_Thickness"] = ThicknessFrom(BorderThickness);
                        Resource["Theme_ElongatedButtons__Border_CornerRadius"] = CornerRadiusFrom(BorderCornerRadius);
                    }
                }
            
                public record OtherBorderLikeThings
                {
                    [JsonProperty("Background")]
                    public string Background { get; set; } = "#181818";

                    [JsonProperty("Border")]
                    public string Border { get; set; } = "#333333";

                    [JsonProperty("Border Thickness")]
                    public double[] BorderThickness { get; set; } = [1.7];

                    [JsonProperty("Border Corner Radius")]
                    public double[] BorderCornerRadius { get; set; } = [0];

                    [OnDeserialized]
                    private void OnDeserialized(StreamingContext Context)
                    {
                        Resource["Theme_OtherBorderLikeThings__Background"] = ToSolidColorBrush(Background);

                        Resource["Theme_OtherBorderLikeThings__Border"] = ToSolidColorBrush(Border);
                        Resource["Theme_OtherBorderLikeThings__Border_Thickness"] = ThicknessFrom(BorderThickness);
                        Resource["Theme_OtherBorderLikeThings__Border_CornerRadius"] = CornerRadiusFrom(BorderCornerRadius);
                    }
                }

                public record JsonEditorSyntax
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
                        SyntaxedTextEditor.TagsBody_Color = ToSolidColorBrush(Tags);
                        SyntaxedTextEditor.TagsValue_Color = ToSolidColorBrush(TagsValue);
                        SyntaxedTextEditor.TagsBody_Color_NotSolidColorBrush = ToColorBrush(Tags);
                        SyntaxedTextEditor.TabExplainColor = ToSolidColorBrush(TabExplainColor);

                        SyntaxedTextEditor.RecompileEditorSyntax();
                    }
                }
            }
        }
    }
}
