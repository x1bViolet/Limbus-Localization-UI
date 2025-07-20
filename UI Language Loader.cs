using Newtonsoft.Json;
using RichText;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;

namespace LC_Localization_Task_Absolute
{
    internal abstract class UILanguageLoader
    {
        internal protected static Dictionary<string, String> UILanguageElementsTextData = [];
        internal protected static Dictionary<string, FontFamily> LoadedFontFamilies = [];
        internal protected static Dictionary<string, String> DynamicTypeElements = [];
        internal protected static UnsavedChangesInfo LangUnsavedChangesInfo = new();

        internal protected static Dictionary<string, RichTextBox> AbleToSetForegroundByTheme = [];

        internal protected static Language LoadedLanguage;

        internal protected class Language
        {
            [JsonProperty("Default Font")]
            public string DefaultUIFont { get; set; } = "";

            [JsonProperty("Custom Logo (⇲ Assets Directory/[+] Languages/Custom Logos)")]
            public string CustomMainMenuLogo { get; set; } = "";

            [JsonProperty("UI Static")]
            public List<UIStaticTextItem> StaticUIElements { get; set; }

            [JsonProperty("Default [$] Insertion")]
            public string DefaultInsertionText { get; set; } = "…";

            [JsonProperty("Make Scan Button tooltip")]
            public string MakeScanTooltip { get; set; } = "Export preview to PNG (To '⇲ Assets Directory\\[⇲] Scans' directory)";

            [JsonProperty("Make Scan Button tooltip width")]
            public double MakeScanTooltip_Width { get; set; } = 296;

            [JsonProperty("Program Version at main menu")]
            public string ProgramVersionTitle { get; set; } = "Version";

            [JsonProperty("Startup Object name")]
            public string StartupObjectName { get; set; } = "Ramus Aureus Resonantus Est";

            [JsonProperty("Unsaved Changes Marker")]
            public string UnsavedChangesMarker { get; set; } = "";

            [JsonProperty("UI Textfields")]
            public List<UITextfieldItem> TextfieldUIElements { get; set; }

            [JsonProperty("Unsaved Changes Info")]
            public UnsavedChangesInfo UnsavedChangesInfo { get; set; }

            [JsonProperty("Custom Localization Load Warnings")]
            public CustomLangLoadingWarnings CustomLangLoadingWarnings { get; set; }

            [JsonProperty("Fonts")]
            public FontsInfo Fonts { get; set; }

            [OnDeserialized]
            internal void OnInit(StreamingContext context)
            {
                if (!UnsavedChangesInfo.IsNull())
                {
                    LangUnsavedChangesInfo = UnsavedChangesInfo;
                }
            }
        }

        internal protected class UnsavedChangesInfo
        {
            [JsonProperty("Passives")]
            public UnsavedChangesInfo_Passives Passives { get; set; }

            [JsonProperty("Keywords")]
            public UnsavedChangesInfo_Keywords Keywords { get; set; }

            [JsonProperty("E.G.O Gifts")]
            public UnsavedChangesInfo_EGOGifts EGOGifts { get; set; }

            [JsonProperty("Skills")]
            public UnsavedChangesInfo_Skills   Skills   { get; set; }
        }
        internal protected class CustomLangLoadingWarnings
        {
            [JsonProperty("Fallback Keywords Directory not found")]
            public string FallbackKeywordsNotFound { get; set; } = "";

            [JsonProperty("Selected Custom Language not found in list")]
            public string CustomLanguagePropertyNotFound { get; set; } = "";

            [JsonProperty("Custom Language Keywords Directory not found")]
            public string KeywordsDirNotFound { get; set; } = "";

            [JsonProperty("Custom Language Context Font not found")]
            public string ContextFontMissing { get; set; } = "";

            [JsonProperty("Custom Language Title Font not found")]
            public string TitleFontMissing { get; set; } = "";

            [JsonProperty("Custom Language Keywords Multiple Meanings Dictionary not found")]
            public string MultipleKeywordsDictionaryMissing { get; set; } = "";

            [JsonProperty("Warnings disabling notice")]
            public string WarningsDisablingNotice { get; set; } = "";

            [JsonProperty("Warnings window title")]
            public string WarningsWindowTitle { get; set; } = "";
        }
        internal protected class UnsavedChangesInfo_Passives
        {
            [JsonProperty("ID Header")]
            public string IDHeader { get; set; }

            [JsonProperty("Main Desc")]
            public string MainDesc { get; set; }

            [JsonProperty("Summary Desc")]
            public string SummaryDesc { get; set; }

            [OnDeserialized] internal void OnInit(StreamingContext context) => NullableControl.NullExterminate(this);
        }
        internal protected class UnsavedChangesInfo_Keywords
        {
            [JsonProperty("ID Header")]
            public string IDHeader { get; set; }

            [JsonProperty("Main Desc")]
            public string MainDesc { get; set; }

            [JsonProperty("Summary Desc")]
            public string SummaryDesc { get; set; }

            [OnDeserialized] internal void OnInit(StreamingContext context) => NullableControl.NullExterminate(this);
        }
        internal protected class UnsavedChangesInfo_EGOGifts
        {
            [JsonProperty("ID Header")]
            public string IDHeader { get; set; }

            [JsonProperty("Main Desc")]
            public string MainDesc { get; set; }

            [JsonProperty("Simple Desc")]
            public string SimpleDesc { get; set; }

            [OnDeserialized] internal void OnInit(StreamingContext context) => NullableControl.NullExterminate(this);
        }
        internal protected class UnsavedChangesInfo_Skills
        {
            [JsonProperty("ID Header")]
            public string IDHeader { get; set; }

            [JsonProperty("Uptie Level")]
            public string UptieLevel { get; set; }

            [OnDeserialized] internal void OnInit(StreamingContext context) => NullableControl.NullExterminate(this);
        }

        internal protected class UIStaticTextItem
        {
            [JsonProperty("Element ID")]
            public string ElementID { get; set; }

            [JsonProperty("Text")]
            public string Text { get; set; }

            [JsonProperty("Font")]
            public string Font { get; set; }

            [JsonProperty("Font Color")]
            public string FontColor { get; set; }

            [JsonProperty("Font Size")]
            public double? FontSize { get; set; }

            [JsonProperty("Font Weight")]
            public string FontWeight { get; set; }

            [JsonProperty("Vertical Offset")]
            public double? VericalOffset { get; set; }

            [JsonProperty("Horizontal Offset")]
            public double? HorizontalOffset { get; set; }

            [JsonProperty("Width")]
            public double? Width { get; set; }

            [JsonProperty("Height")]
            public double? Height { get; set; }

            [JsonProperty("Visible")]
            public bool? Visible { get; set; }

            [OnDeserialized] internal void OnInit(StreamingContext context) => NullableControl.NullExterminate(this);
        }

        internal protected class UITextfieldItem
        {
            [JsonProperty("Element ID")]
            public string ElementID { get; set; }

            public string Font { get; set; }

            [JsonProperty("Font Color")]
            public string FontColor { get; set; }

            [JsonProperty("Font Size")]
            public double FontSize { get; set; }

            [JsonProperty("Font Weight")]
            public string FontWeight { get; set; }

            [JsonProperty("Vertical Offset")]
            public double VericalOffset { get; set; }

            [JsonProperty("Horizontal Offset")]
            public double HorizontalOffset { get; set; }

            [OnDeserialized] internal void OnInit(StreamingContext context) => NullableControl.NullExterminate(this);
        }

        internal protected class FontsInfo
        {
            [JsonProperty("Font Files (⇲ Assets Directory/[+] Languages/Fonts)")]
            public List<FontFileInfo> FontFilesInfo { get; set; }
        }
        internal protected class FontFileInfo
        {
            [JsonProperty("File")]
            public string File { get; set; } = "";

            [JsonProperty("Name")]
            public string Name { get; set; } = "";

            [OnDeserialized]
            internal void FontFileInfoSet(StreamingContext context)
            {
                LoadedFontFamilies[Name] = FileToFontFamily(@$"⇲ Assets Directory\[+] Languages\Fonts\{File}");
            }
        }


        internal protected static bool UILanguageLoadingEvent = false;
        internal protected static string LastSeenDefaultInsertion = "";
        internal protected static string LastSeenDefaultObjectName = "";
        internal protected static void InitializeUILanguage(string SourceFile)
        {
            UILanguageLoadingEvent = true;
            string LanugageFile;
            if (File.Exists(SourceFile))
            {
                LanugageFile = File.ReadAllText(SourceFile);
            }
            else
            {
                LanugageFile = MainWindow.DefaultLanguage;
            }

            LoadedFontFamilies.Clear();
            LoadedLanguage = JsonConvert.DeserializeObject<Language>(LanugageFile);

            LastSeenDefaultInsertion = LoadedLanguage.DefaultInsertionText;

            MainControl.MakePreviewScan_Tooltip.SetRichText(LoadedLanguage.MakeScanTooltip);
            MainControl.MakePreviewScan_Tooltip.Width = LoadedLanguage.MakeScanTooltip_Width;
            MainControl.PROGRAM_Version.Text = LoadedLanguage.ProgramVersionTitle;

            if (File.Exists(@$"⇲ Assets Directory\[+] Languages\Custom Logos\{LoadedLanguage.CustomMainMenuLogo}"))
            {
                MainControl.MainMenuLogo.Source = GenerateBitmapFromFile(@$"⇲ Assets Directory\[+] Languages\Custom Logos\{LoadedLanguage.CustomMainMenuLogo}");
            }

            if (PreviewUpdate_TargetSite != null)
            {
                if (PreviewUpdate_TargetSite.Equals(MainControl.PreviewLayout_Default))
                {
                    MainControl.NavigationPanel_ObjectName_Display.Text = LoadedLanguage.StartupObjectName;
                }
            }

            foreach (UIStaticTextItem UIStaticItemData in LoadedLanguage.StaticUIElements)
            {
                string TargetUIElementID = UIStaticItemData.ElementID;
                string TargetUIElementText = UIStaticItemData.Text;
                //rin(TargetUIElementID);
                if (UILanguage.ContainsKey(TargetUIElementID))
                {
                    string LoadFontAttach = "";

                    if (UIStaticItemData.ElementID.StartsWith("Editor Context Menu"))
                    {
                        if (!UIStaticItemData.Visible.IsNull())
                        {
                            if (UIStaticItemData.Visible == false)
                            {
                                UILanguage[TargetUIElementID].Visibility = (bool)UIStaticItemData.Visible ? Visibility.Visible : Visibility.Collapsed;
                                //rin($"Hide {TargetUIElementID}");
                            }
                        }
                        else
                        {
                            UILanguage[TargetUIElementID].Visibility = Visibility.Visible;
                            //rin($"Show {TargetUIElementID}");
                        }
                    }

                    if (!UIStaticItemData.VericalOffset.IsNull())
                    {
                        UILanguage[TargetUIElementID].Margin =
                            new System.Windows.Thickness(
                                UILanguage[TargetUIElementID].Margin.Left,
                                (double)UIStaticItemData.VericalOffset,
                                UILanguage[TargetUIElementID].Margin.Right,
                                UILanguage[TargetUIElementID].Margin.Bottom
                            );
                    }
                    if (!UIStaticItemData.HorizontalOffset.IsNull())
                    {
                        UILanguage[TargetUIElementID].Margin =
                        new System.Windows.Thickness(
                            (double)UIStaticItemData.HorizontalOffset, 
                            UILanguage[TargetUIElementID].Margin.Top,
                            UILanguage[TargetUIElementID].Margin.Right,
                            UILanguage[TargetUIElementID].Margin.Bottom
                        );
                    }

                    bool DefaultFontFromPCLoaded = false;
                    if (!UIStaticItemData.Font.IsNull())
                    {
                        if (LoadedFontFamilies.ContainsKey(UIStaticItemData.Font))
                        {
                            LoadFontAttach = UIStaticItemData.Font;
                            UILanguage[TargetUIElementID].FontFamily = LoadedFontFamilies[UIStaticItemData.Font];
                        }
                        else
                        {
                            DefaultFontFromPCLoaded = true;
                            UILanguage[TargetUIElementID].FontFamily = new FontFamily(UIStaticItemData.Font);
                            //rin($"Apply {UIStaticItemData.Font} for {TargetUIElementID}");
                        }
                    }
                        
                    if (LoadedFontFamilies.ContainsKey(LoadedLanguage.DefaultUIFont) & !DefaultFontFromPCLoaded)
                    {
                        if (LoadFontAttach.Equals(""))
                        {
                            if (!TargetUIElementID.Equals("E.G.O Gifts Preview 'View Desc.' Text"))
                            {
                                LoadFontAttach = LoadedLanguage.DefaultUIFont;
                                UILanguage[TargetUIElementID].FontFamily = LoadedFontFamilies[LoadedLanguage.DefaultUIFont];
                            }
                        }
                    }
                    else
                    {
                        UILanguage[TargetUIElementID].FontFamily = new FontFamily(UIStaticItemData.Font);
                    }
                        

                    if (!UIStaticItemData.FontSize.IsNull()) UILanguage[TargetUIElementID].FontSize = (double)UIStaticItemData.FontSize;

                    if (!UIStaticItemData.FontWeight.IsNull())
                    {
                        UILanguage[TargetUIElementID].FontWeight = WeightFrom(UIStaticItemData.FontWeight);
                    }
                    if (!UIStaticItemData.FontColor.IsNull())
                    {
                        UILanguage[TargetUIElementID].Foreground = ToColor(UIStaticItemData.FontColor);
                    }
                    else
                    {
                        AbleToSetForegroundByTheme[TargetUIElementID] = UILanguage[TargetUIElementID];
                    }




                    if (!UIStaticItemData.Width.IsNull())
                    {
                        if (UIStaticItemData.Width != 0) UILanguage[TargetUIElementID].Width = (double)UIStaticItemData.Width;
                    }
                    if (!UIStaticItemData.Height.IsNull())
                    {
                        if (UIStaticItemData.Height != 0) UILanguage[TargetUIElementID].Height = (double)UIStaticItemData.Height;
                    }

                    if (!LoadFontAttach.Equals(""))
                    {
                        //rin($"{UIStaticItemData.ElementID} : {LoadFontAttach}");
                        TargetUIElementText = $"<loadfont=`{LoadFontAttach}`>{TargetUIElementText}"
                            .Replace("</font>", $"</font><loadfont=`{LoadFontAttach}`>")
                            .Replace("</loadfont>", $"</loadfont><loadfont=`{LoadFontAttach}`>");
                    }

                    if (!UIStaticItemData.Text.IsNull())
                    {
                        UILanguageElementsTextData[TargetUIElementID] = UIStaticItemData.Text;

                        if (UIStaticItemData.Text.Contains("[$]"))
                        {
                            DynamicTypeElements[TargetUIElementID] = UIStaticItemData.Text;
                            //rin($"------------------------------");
                            if (UILanguage[TargetUIElementID].GetText().Contains(LastSeenDefaultInsertion))
                            {
                                UILanguage[TargetUIElementID].SetRichText(TargetUIElementText.Extern(LoadedLanguage.DefaultInsertionText));
                            }
                            //rin($"{TargetUIElementID}: \"{UILanguage[TargetUIElementID].GetText()}\"");
                        }
                        else
                        {
                            UILanguage[TargetUIElementID].SetRichText(TargetUIElementText);
                        }
                    }
                }
            }

            foreach (UITextfieldItem UITextfieldItemData in LoadedLanguage.TextfieldUIElements)
            {
                string TargetUIElementID = UITextfieldItemData.ElementID;
                if (UITextfieldElements.ContainsKey(TargetUIElementID))
                {                    
                    if (LoadedFontFamilies.ContainsKey(UITextfieldItemData.Font))
                    {
                        UITextfieldElements[TargetUIElementID].FontFamily = LoadedFontFamilies[UITextfieldItemData.Font];
                    }
                    else
                    {
                        UITextfieldElements[TargetUIElementID].FontFamily = new FontFamily(UITextfieldItemData.Font);
                    }
                    
                    if (UITextfieldItemData.FontSize != 0) UITextfieldElements[TargetUIElementID].FontSize = (double)UITextfieldItemData.FontSize;
                    if (UITextfieldItemData.VericalOffset != 0) UITextfieldElements[TargetUIElementID].Margin = new Thickness(
                        UITextfieldElements[TargetUIElementID].Margin.Left,
                        UITextfieldItemData.VericalOffset,
                        UITextfieldElements[TargetUIElementID].Margin.Right,
                        UITextfieldElements[TargetUIElementID].Margin.Bottom
                    );
                    if (UITextfieldItemData.HorizontalOffset != 0) UITextfieldElements[TargetUIElementID].Margin = new Thickness(
                        UITextfieldItemData.HorizontalOffset,
                        UITextfieldElements[TargetUIElementID].Margin.Top,
                        UITextfieldElements[TargetUIElementID].Margin.Right,
                        UITextfieldElements[TargetUIElementID].Margin.Bottom
                    );
                    UITextfieldElements[TargetUIElementID].FontWeight = WeightFrom(UITextfieldItemData.FontWeight);
                }
            }
            UILanguageLoadingEvent = false;
        }
    }
}
