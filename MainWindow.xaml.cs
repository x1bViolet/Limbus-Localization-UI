using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Microsoft.Win32;
using RichText;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Json.FilesIntegration;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Globalization.NumberStyles;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow : Window
{
    #region Initials
    /// <summary>
    /// Damn
    /// </summary>
    internal protected static MainWindow MainControl;
    internal protected static SettingsWindow SettingsControl = new SettingsWindow();


    internal protected static Dictionary<string, RichTextBox> UILanguage;
    internal protected static Dictionary<string, TextBox> UITextfieldElements;
    internal protected static Dictionary<string, RichTextBox> PreviewLayoutControls;

    internal protected static List<dynamic?> SwitchButtons_TextFieldsControl = [];

    internal protected static List<RichTextBox?> PreviewLayoutsList = [];

    internal protected static List<TextBox> FocusableTextBoxes = [];
    
    internal protected static bool ManualTextLoadEvent = false;
    internal protected static RichTextBox PreviewUpdate_TargetSite;

    internal protected static string DefaultTheme = "";
    internal protected static string DefaultLanguage = "";

    internal protected static FileInfo CurrentFile;
    internal protected static Encoding CurrentFileEncoding = new UTF8Encoding();

    public void InitializeUIInteractionLogic()
    {
        UILanguage = new()
        {
            ["Title"] = STE_Title,

            ["Json Path [Shadow Text]"] = STE_JsonFile_Path_ShadowText,

            ["Right Menu — Current ID Copy Button"] = STE_NavigationPanel_ObjectID_Display,
            ["Right Menu — Current ID Copy Button (Copied)"] = STE_NavigationPanel_ObjectID_Display_IDCopied,

            ["Skills Main Name [Shadow Text]"] = STE_Skills_MainSkillName_ShadowText,
            ["Skills EGO Abnormality Name [Shadow Text]"] = STE_Skills_EGOAbnormalitySkillName_ShadowText,
            ["Right Menu — Skill Desc"] = STE_Skills_MainDescription,
            ["Right Menu — Skill Coin 1"] = STE_Skills_Coin_1,
            ["Right Menu — Skill Coin 2"] = STE_Skills_Coin_2,
            ["Right Menu — Skill Coin 3"] = STE_Skills_Coin_3,
            ["Right Menu — Skill Coin 4"] = STE_Skills_Coin_4,
            ["Right Menu — Skill Coin 5"] = STE_Skills_Coin_5,
            ["Right Menu — Skill Coin 6"] = STE_Skills_Coin_6,
            ["Right Menu — Skill Coin Descs Title"] = STE_CoinDescriptionsTitle,
            ["Right Menu — Skill Coin Desc Number"] = STE_Skills_Coin_DescNumberDisplay,

            ["Right Menu — Passive Name [Shadow Text]"] = STE_Passives_MainPassiveName_ShadowText,
            ["Right Menu — Passive Desc"] = STE_Passives_MainDescription,
            ["Right Menu — Passive Summary"] = STE_Passives_SummaryDescription,
            ["Right Menu — Add Passive Summary Description Tooltip"] = STE_Passives_AddSummaryDescriptionButton,

            ["Right Menu — Keyword Name [Shadow Text]"] = STE_Keywords_KeywordName_ShadowText,
            ["Right Menu — Keyword Desc"] = STE_Keyword_MainDescription,
            ["Right Menu — Keyword Summary"] = STE_Keyword_SummaryDescription,
            ["Right Menu — Keywords Format Insertions Button"] = STE_Keywords_ToggleFormatInsertionsDropdown,
            ["Right Menu — Keywords Format Insertion {0} [Shadow Text]"] = STE_Keywords_FormatInsertions_0_ShadowText,
            ["Right Menu — Keywords Format Insertion {1} [Shadow Text]"] = STE_Keywords_FormatInsertions_1_ShadowText,
            ["Right Menu — Keywords Format Insertion {2} [Shadow Text]"] = STE_Keywords_FormatInsertions_2_ShadowText,
            ["Right Menu — Keywords Format Insertion {3} [Shadow Text]"] = STE_Keywords_FormatInsertions_3_ShadowText,
            ["Right Menu — Keywords Format Insertion {4} [Shadow Text]"] = STE_Keywords_FormatInsertions_4_ShadowText,
            ["Right Menu — Keywords Format Insertion {5} [Shadow Text]"] = STE_Keywords_FormatInsertions_5_ShadowText,
            ["Right Menu — Keywords Format Insertion {6} [Shadow Text]"] = STE_Keywords_FormatInsertions_6_ShadowText,

            ["Right Menu — E.G.O Gift Name [Shadow Text]"] = STE_EGOGifts_EGOGiftName_ShadowText,
            ["Right Menu — E.G.O Gift Desc"] = STE_EGOGift_MainDescription,
            ["Right Menu — E.G.O Gift Simple Desc 1"] = STE_EGOGift_SimpleDescription1,
            ["Right Menu — E.G.O Gift Simple Desc 2"] = STE_EGOGift_SimpleDescription2,
            ["Right Menu — E.G.O Gift Simple Desc 3"] = STE_EGOGift_SimpleDescription3,
            ["Right Menu — E.G.O Gift Simple Desc 4"] = STE_EGOGift_SimpleDescription4,
            ["Right Menu — E.G.O Gift Simple Desc 5"] = STE_EGOGift_SimpleDescription5,
            ["Right Menu — E.G.O Gift Simple Desc 6"] = STE_EGOGift_SimpleDescription6,
            ["E.G.O Gifts Preview 'View Desc.' Text"] = STE_EGOGifts_LivePreview_ViewDescButtons,

            ["Editor Context Menu — Insert Style"] = STE_ContextMenu_InsertStyle,
            ["Editor Context Menu — TextMeshPro to [KeywordID]"] = STE_ContextMenu_TMProToKeywordLinks,
            ["Editor Context Menu — TextMeshPro to Shorthands"] = STE_ContextMenu_TMProToShorthands,
            ["Editor Context Menu — Unevident to [KeywordID]"] = STE_ContextMenu_UnevidentKeywordsToKeywordLinks,
            ["Editor Context Menu — Unevident to Shorthands"] = STE_ContextMenu_UnevidentKeywordsToShorthands,
            ["Editor Context Menu — [KeywordID] to Shorthands"] = STE_ContextMenu_KeywordLinksToShorthand,
            ["Editor Context Menu — [KeywordID] to TextMeshPro"] = STE_ContextMenu_KeywordLinksToTMPro,

            ["Unsaved Changes Window — Title"] = STE_UnsavedChangesWindow_Title,
            ["Unsaved Changes Window — Cancel"] = STE_UnsavedChangesWindow_Cancel,
            ["Unsaved Changes Window — Confirm Exit"] = STE_UnsavedChangesWindow_ConfirmExit,
            ["Unsaved Changes Window — Information Text"] = DTE_UnsavedChangesInfo,


            ["[Settings] Limbus Preview — Section Name"] = SettingsControl.STE_Settings_LimbusPreview_SectionName,
            ["[Settings] Limbus Preview — Highlight <style>"] = SettingsControl.STE_Settings_LimbusPreview_HighlightStyle,
            ["[Settings] Limbus Preview — Highlight Coin Desc on click"] = SettingsControl.STE_Settings_LimbusPreview_HighlightCoinDescOnClick,
            ["[Settings] Limbus Preview — Highlight Coin Desc on manual switch"] = SettingsControl.STE_Settings_LimbusPreview_HighlightCoinDescOnSwitch,
            ["[Settings] Limbus Preview — Preview update delay"] = SettingsControl.STE_Settings_LimbusPreview_UpdateDelay,
           
            ["[Settings] Custom Lanugage — Section Name"] = SettingsControl.STE_Settings_CustomLanguage_SectionName,
            ["[Settings] Custom Lanugage — Selected Properties"] = SettingsControl.STE_Settings_CustomLanguage_Selected,
            ["[Settings] Custom Lanugage — Keywords Directory"] = SettingsControl.STE_Settings_CustomLanguage_KeywordsDirectory,
            ["[Settings] Custom Lanugage — Title Font"] = SettingsControl.STE_Settings_CustomLanguage_TitleFont,
            ["[Settings] Custom Lanugage — Context Font"] = SettingsControl.STE_Settings_CustomLanguage_ContextFont,


            ["[Settings] Internal — Section Name"] = SettingsControl.STE_Settings_Internal_SectionName,
            ["[Settings] Internal — Selected UI Lanugage"] = SettingsControl.STE_Settings_Internal_UILanguage,
            ["[Settings] Internal — Selected UI Theme"] = SettingsControl.STE_Settings_Internal_UITheme,
            ["[Settings] Internal — Toggle Topmost state"] = SettingsControl.STE_Settings_Internal_TopmostWindowState,
            ["[Settings] Internal — Enable Load Warnings"] = SettingsControl.STE_Settings_Internal_EnableLoadWarnings,
            ["[Settings] Internal — Dropdown lists Readme"] = SettingsControl.STE_Settings_Internal_Readme,


            ["[Settings] Preview Scans — Section Name"] = SettingsControl.STE_Settings_PreviewScans_SectionName,
            ["[Settings] Preview Scans — Toggle Scan Area view"] = SettingsControl.STE_Settings_PreviewScans_ToggleAreaView,
            ["[Settings] Preview Scans — Width of Skills Preview"] = SettingsControl.STE_Settings_PreviewScans_SkillsAreaWidth,
            ["[Settings] Preview Scans — Display Keyword Sprites"] = SettingsControl.STE_Settings_PreviewScans_DisplayKeywordSpritesToggle,
            ["[Settings] Preview Scans — Display Keyword Underline"] = SettingsControl.STE_Settings_PreviewScans_DisplayKeywordUnderlineToggle,
            ["[Settings] Preview Scans — Readme Title"] = SettingsControl.STE_Settings_PreviewScans_Readme_0,
            ["[Settings] Preview Scans — Readme 1"] = SettingsControl.STE_Settings_PreviewScans_Readme_1,
            ["[Settings] Preview Scans — Readme 2"] = SettingsControl.STE_Settings_PreviewScans_Readme_2,
            ["[Settings] Preview Scans — Readme 3"] = SettingsControl.STE_Settings_PreviewScans_Readme_3,
        };
        UITextfieldElements = new()
        {
            ["Json File Path"] = JsonFilePath,
            ["Right Menu — Manual ID Input"] = NavigationPanel_IDSwitch_ManualInput_Textfield,
            ["Right Menu — Skills Main Name"] = SWBT_Skills_MainSkillName,
            ["Right Menu — EGO Abnormality Name"] = SWBT_Skills_EGOAbnormalitySkillName,
            ["Right Menu — Passive Name"] = SWBT_Passives_MainPassiveName,
            ["Right Menu — Keyword Name"] = SWBT_Keywords_KeywordName,
            ["Right Menu — E.G.O Gift Name"] = SWBT_EGOGifts_EGOGiftName,
            ["Right Menu — Keywords Format Insertion {0}"] = SWBT_Keywords_FormatInsertion_0,
            ["Right Menu — Keywords Format Insertion {1}"] = SWBT_Keywords_FormatInsertion_1,
            ["Right Menu — Keywords Format Insertion {2}"] = SWBT_Keywords_FormatInsertion_2,
            ["Right Menu — Keywords Format Insertion {3}"] = SWBT_Keywords_FormatInsertion_3,
            ["Right Menu — Keywords Format Insertion {4}"] = SWBT_Keywords_FormatInsertion_4,
            ["Right Menu — Keywords Format Insertion {5}"] = SWBT_Keywords_FormatInsertion_5,
            ["Right Menu — Keywords Format Insertion {6}"] = SWBT_Keywords_FormatInsertion_6,

            ["[Settings] Limbus Preview — Preview update delay"] = SettingsControl.InputPreviewUpdateDelay,
            ["[Settings] Custom Lanugage — Keywords Directory"] = SettingsControl.CustomLang_KeywordsDir,
            ["[Settings] Custom Lanugage — Title Font"] = SettingsControl.CustomLang_TitleFont,
            ["[Settings] Custom Lanugage — Context Font"] = SettingsControl.CustomLang_ContextFont,
            ["[Settings] Custom Lanugage — Width of Skills Preview"] = SettingsControl.InputSkillsPanelWidth,
        };
    }
    #endregion


    public MainWindow()
    {
        InitializeComponent();
        MainControl = this;

        try { Console.OutputEncoding = Encoding.UTF8; }
        catch { }
        
        InitializeUIInteractionLogic();
        InitializeDefaultResource();

        InitSurfaceScroll(NavigationPanel_ObjectName_DisplayScrollViewer);

        InitSurfaceScroll(SurfaceScrollPreview_Skills);
        InitSurfaceScroll(SurfaceScrollPreview_Keywords__Bufs);
        InitSurfaceScroll(SurfaceScrollPreview_Keywords__BattleKeywords);
        InitSurfaceScroll(SurfaceScrollPreview_Passives);
        InitSurfaceScroll(SurfaceScrollPreview_EGOGifts);
        InitSurfaceScroll(SurfaceScrollPreview_Default);
        InitSurfaceScroll(SurfaceScroll_UnsavedChangesInfo);

        PreviewMouseDown += MouseDownEvent;

        ProcessLogicalTree(this);

        InitMain();
    }

    internal protected void InitMain()
    {
        PreviewUpdate_TargetSite = PreviewLayout_Default;

        File.WriteAllText(@"⇲ Assets Directory\Latest loading.txt", "");

        Configurazione.PullLoad();

        if (File.Exists(@"⇲ Assets Directory\Default Text.txt"))
        {
            Editor.Text = File.ReadAllText(@"⇲ Assets Directory\Default Text.txt");
        }
        else
        {
            Editor.Text = "               <spritessize=+30><font=\"BebasKai SDF\"><size=140%><sprite name=\"9202\"> <u>Limbus Company Localization Interface</u> <color=#f8c200>'1.0:0</color></size></font></spritessize>";
        }

        RichText.InternalModel.InitializingEvent = false;



        ////Default file load on startup
        //FileInfo SomeFile = new FileInfo(@"Skills_personality-01.json");

        //FocusOnFile(SomeFile);
        //Mode_Skills.LoadStructure(SomeFile);
        //Mode_Skills.TransformToSkill(1011302);
    }



    #region Limbus live preview
    internal protected static bool IsPendingPreviewUpdate = false;
    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!ManualTextLoadEvent)
        {
            if (!IsPendingPreviewUpdate)
            {
                IsPendingPreviewUpdate = true;
                try
                {
                    DispatcherTimer Timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.PreviewUpdateDelay) };
                    Timer.Start();

                    Timer.Tick += (sender, args) =>
                    {
                        Timer.Stop();
                        PullUpdatePreview(Editor.Text);

                        IsPendingPreviewUpdate = false;
                    };
                }
                catch { IsPendingPreviewUpdate = false; }
            }
        }
        else
        {
            PullUpdatePreview(Editor.Text);
        }

        // Window_PreviewKeyDown() -> if ctrlv and \" -> Editor_TextChanged() -> here
        // After pasting formatted clipboard return normal text to clipboard that was before
        if (IsQuotesConvertedInClipboard)
        {
            Clipboard.SetText(QuotesClipboardOldText);
            IsQuotesConvertedInClipboard = false;
            QuotesClipboardOldText = "";
        }
    }

    internal protected static void PullUpdatePreview(string EditorText)
    {
        switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
        {
            case "Skills":

                if (Mode_Skills.CurrentSkillID != -1)
                {
                    if (!EditorText.Equals(""))
                    {
                        PreviewUpdate_TargetSite.Visibility = Visible;
                    }
                    else
                    {
                        PreviewUpdate_TargetSite.Visibility = Collapsed;
                    }

                    Mode_Skills.LastPreviewUpdatesBank[PreviewUpdate_TargetSite] = EditorText.Replace("\r", "");


                    if (PreviewUpdate_TargetSite.Equals(MainControl.PreviewLayout_Skills_MainDesc))
                    {
                        /////////////////////////////////////////////////////////////////////////////////////////////
                        var FullLink = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel];
                        /////////////////////////////////////////////////////////////////////////////////////////////

                        FullLink.EditorDescription = EditorText.Replace("\r", "");

                        if (!FullLink.Description
                            .Equals(FullLink.EditorDescription))
                        {
                            UILanguage["Right Menu — Skill Desc"]
                                .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker.Extern(UILanguageLoader.UILanguageElementsTextData["Right Menu — Skill Desc"]));
                        }
                        else
                        {
                            UILanguage["Right Menu — Skill Desc"]
                                .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Skill Desc"]);
                        }
                    }
                    else
                    {
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        var FullLink = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Coins[Mode_Skills.CurrentSkillCoinIndex].CoinDescriptions[Mode_Skills.CurrentSkillCoinDescIndex];
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        FullLink.EditorDescription = EditorText.Replace("\r", "");

                        if (!FullLink.Description
                            .Equals(FullLink.EditorDescription))
                        {
                            MainControl.STE_Skills_Coin_DescNumberDisplay
                                .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                                    .Extern(UILanguageLoader.UILanguageElementsTextData["Right Menu — Skill Coin Desc Number"].Extern(Mode_Skills.CurrentSkillCoinDescIndex + 1)));
                        }
                        else
                        {
                            if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Skill Coin Desc Number"))
                            {
                                MainControl.STE_Skills_Coin_DescNumberDisplay
                                    .SetRichText(UILanguageLoader.DynamicTypeElements["Right Menu — Skill Coin Desc Number"]
                                        .Extern(Mode_Skills.CurrentSkillCoinDescIndex + 1));
                            }
                        }

                        if (DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Coins[Mode_Skills.CurrentSkillCoinIndex].CoinDescriptions
                            .Where(x => x.Description != x.EditorDescription).Any())
                        {
                            (MainControl.FindName($"STE_Skills_Coin_{Mode_Skills.CurrentSkillCoinIndex + 1}") as RichTextBox)
                                .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                                    .Extern(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"]));
                        }
                        else
                        {
                            (MainControl.FindName($"STE_Skills_Coin_{Mode_Skills.CurrentSkillCoinIndex + 1}") as RichTextBox)
                                .SetRichText(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"]);
                        }


                        // Auto hide coin if its empty
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        var CoinInfoFullLink = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Coins[Mode_Skills.CurrentSkillCoinIndex];
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        
                        if (CoinInfoFullLink.CoinDescriptions.Where(x => x.EditorDescription.EqualsOneOf(["", "<style=\"highlight\"></style>"])).Count() == CoinInfoFullLink.CoinDescriptions.Count)
                        {
                            (MainControl.FindName($"PreviewLayout_Skills_Coin{Mode_Skills.CurrentSkillCoinIndex + 1}") as Grid).Visibility = Collapsed;
                        }
                        else
                        {
                            (MainControl.FindName($"PreviewLayout_Skills_Coin{Mode_Skills.CurrentSkillCoinIndex + 1}") as Grid).Visibility = Visible;
                        }
                    }
                }
                break;





            case "Passives":
                if (Mode_Passives.CurrentPassiveID != -1)
                {
                    ///////////////////////////////////////////////////////////////
                    var FullLink = DelegatePassives[Mode_Passives.CurrentPassiveID];
                    ///////////////////////////////////////////////////////////////

                    if (Mode_Passives.TargetSite_StringLine.Equals("Main Description")) FullLink.EditorDescription = EditorText.Replace("\r", "");
                    else
                    {
                        FullLink.EditorSummaryDescription = EditorText.Replace("\r", "");
                    }

                    switch (Mode_Passives.TargetSite_StringLine)
                    {
                        case "Main Description":
                            if (!FullLink.Description.Equals(FullLink.EditorDescription))
                            {
                                MainControl.STE_Passives_MainDescription
                                    .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                                    .Extern(UILanguageLoader.UILanguageElementsTextData["Right Menu — Passive Desc"]));
                            }
                            else
                            {
                                MainControl.STE_Passives_MainDescription
                                    .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Passive Desc"]);
                            }
                            break;

                        case "Summary Description":
                            if (!FullLink.SummaryDescription.Equals(FullLink.EditorSummaryDescription))
                            {
                                MainControl.STE_Passives_SummaryDescription
                                    .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                                    .Extern(UILanguageLoader.UILanguageElementsTextData["Right Menu — Passive Summary"]));
                            }
                            else
                            {
                                MainControl.STE_Passives_SummaryDescription
                                    .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Passive Summary"]);
                            }
                            break;
                    }
                }

                break;





            case "Keywords":
                if (!Mode_Keywords.CurrentKeywordID.Equals(""))
                {
                    ///////////////////////////////////////////////////////////////
                    var FullLink = DelegateKeywords[Mode_Keywords.CurrentKeywordID];
                    ///////////////////////////////////////////////////////////////

                    if (Mode_Keywords.TargetSite_StringLine.Equals("Main Description")) FullLink.EditorDescription = EditorText.Replace("\r", "");
                    else
                    {
                        FullLink.EditorSummaryDescription = EditorText.Replace("\r", "");
                    }

                    switch (Mode_Keywords.TargetSite_StringLine)
                    {
                        case "Main Description":
                            if (!FullLink.Description.Equals(FullLink.EditorDescription))
                            {
                                MainControl.STE_Keyword_MainDescription
                                    .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                                    .Extern(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Desc"]));
                            }
                            else
                            {
                                MainControl.STE_Keyword_MainDescription
                                    .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Desc"]);
                            }
                            break;

                        case "Summary Description":
                            if (!FullLink.SummaryDescription.Equals(FullLink.EditorSummaryDescription))
                            {
                                MainControl.STE_Keyword_SummaryDescription
                                    .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                                    .Extern(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Summary"]));
                            }
                            else
                            {
                                MainControl.STE_Keyword_SummaryDescription
                                    .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Summary"]);
                            }
                            break;
                    }
                }

                break;





            case "E.G.O Gifts":
                if (!Mode_EGOGifts.CurrentEGOGiftID.Equals(-1))
                {
                    ///////////////////////////////////////////////////////////////
                    var FullLink = DelegateEGOGifts[Mode_EGOGifts.CurrentEGOGiftID];
                    ///////////////////////////////////////////////////////////////
                    switch (Mode_EGOGifts.TargetSite_StringLine)
                    {
                        case "Main Description":

                            FullLink.EditorDescription = EditorText.Replace("\r", "");

                            if (!FullLink.Description.Equals(FullLink.EditorDescription))
                            {
                                MainControl.STE_EGOGift_MainDescription
                                    .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                                    .Extern(UILanguageLoader.UILanguageElementsTextData["Right Menu — E.G.O Gift Desc"]));
                            }
                            else
                            {
                                MainControl.STE_EGOGift_MainDescription
                                    .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — E.G.O Gift Desc"]);
                            }
                            break;

                        default:

                            string SimpleDescNumber = $"{Mode_EGOGifts.TargetSite_StringLine[^1]}";
                            
                            int TargetSimpleDescIndex = int.Parse(SimpleDescNumber) - 1;

                            FullLink.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription = EditorText.Replace("\r", "");


                            if (!FullLink.SimpleDescriptions[TargetSimpleDescIndex].Description.Equals(FullLink.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription))
                            {
                                (MainControl.FindName($"STE_EGOGift_SimpleDescription{SimpleDescNumber}") as RichTextBox)
                                    .SetRichText(UILanguageLoader.LoadedLanguage.UnsavedChangesMarker
                                    .Extern(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — E.G.O Gift Simple Desc {SimpleDescNumber}"]));
                            }
                            else
                            {
                                (MainControl.FindName($"STE_EGOGift_SimpleDescription{SimpleDescNumber}") as RichTextBox)
                                    .SetRichText(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — E.G.O Gift Simple Desc {SimpleDescNumber}"]);
                            }

                            break;
                    }
                }

                break;

            default: break;
        }

        if (!PreviewUpdate_TargetSite.IsNull()) PreviewUpdate_TargetSite.SetLimbusRichText(EditorText);
    }
    #endregion



    #region Surfacescroll
    internal protected static void InitSurfaceScroll(ScrollViewer Target)
    {
        try
        {
            Target.Resources.Add(SystemParameters.VerticalScrollBarWidthKey, 0.0);
        }
        catch { }
        
        Target.PreviewMouseLeftButtonDown += SurfaceScroll_MouseLeftButtonDown;
        Target.PreviewMouseMove += SurfaceScroll_MouseMove;
        Target.PreviewMouseLeftButtonUp += SurfaceScroll_MouseLeftButtonUp;
    }
    internal protected static bool SurfaceScroll_isDragging = false;
    internal protected static Point SurfaceScroll_lastMousePosition;
    internal protected static void SurfaceScroll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!MainControl.PreviewLayout_Keywords_Bufs_Name.IsMouseOver)
        {
            SurfaceScroll_isDragging = true;
            SurfaceScroll_lastMousePosition = e.GetPosition(sender as ScrollViewer);
            (sender as ScrollViewer).CaptureMouse();
        }
    }
    internal protected static void SurfaceScroll_MouseMove(object sender, MouseEventArgs e)
    {
        if (SurfaceScroll_isDragging)
        {
            Point currentPosition = e.GetPosition(sender as ScrollViewer);
            Vector diff = SurfaceScroll_lastMousePosition - currentPosition;
            (sender as ScrollViewer).ScrollToVerticalOffset((sender as ScrollViewer).VerticalOffset + diff.Y);
            (sender as ScrollViewer).ScrollToHorizontalOffset((sender as ScrollViewer).HorizontalOffset + diff.X);
            SurfaceScroll_lastMousePosition = currentPosition;
        }
    }
    internal protected static void SurfaceScroll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        SurfaceScroll_isDragging = false;
        (sender as ScrollViewer).ReleaseMouseCapture();
    }
    #endregion

    

    #region Skills switches
    private void Skills_HighlightUptieLevel(object sender, MouseEventArgs e)
    {
        Border Sender = sender as Border;
        int SenderNumber = int.Parse($"{Sender.Name[^1]}");

        if (Mode_Skills.CurrentSkillUptieLevel != SenderNumber)
        {
            (FindName($"NavigationPanel_Skills_UptieLevelSwitch_HighlightImage_{SenderNumber}") as Image).Visibility = Visible;
        }
    }
    private void Skills_DeHighlightUptieLevel(object sender, MouseEventArgs e)
    {
        Border Sender = sender as Border;
        int SenderNumber = int.Parse($"{Sender.Name[^1]}");

        if (Mode_Skills.CurrentSkillUptieLevel != SenderNumber)
        {
            (FindName($"NavigationPanel_Skills_UptieLevelSwitch_HighlightImage_{SenderNumber}") as Image).Visibility = Collapsed;
        }
    }


    private void ChangeSkillEGOAbnormalityName(object sender, MouseButtonEventArgs e)
    {
        /////////////////////////////////////////////////////////////////////////////////////////////
        var FullLink = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel];
        /////////////////////////////////////////////////////////////////////////////////////////////

        if (!SWBT_Skills_EGOAbnormalitySkillName.Text.Equals(FullLink.EGOAbnormalityName))
        {
            FullLink.EGOAbnormalityName = SWBT_Skills_EGOAbnormalitySkillName.Text;

            Mode_Skills.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
        }
    }


    private void NavigationPanel_Skills_SwitchToUptieLevel(object sender, MouseButtonEventArgs e)
    {
        Border Sender = sender as Border;
        string UptieLevelNumber = $"{Sender.Name[^1]}";

        Mode_Skills.TransformToSkill(Mode_Skills.CurrentSkillID, int.Parse(UptieLevelNumber));
    }

    private void Actions_Skills_SwitchToDesc(object sender, MouseButtonEventArgs e) => Mode_Skills.SwitchToDesc();

    private void Actions_Skills_SetCoinFocus(object sender, MouseButtonEventArgs e)
    {
        Border Sender = sender as Border;
        string CoinNumber = $"{Sender.Name[^1]}";
        Mode_Handlers.Mode_Skills.SetCoinFocus(int.Parse(CoinNumber));

        if (DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnManualSwitch)
        {
            RichTextBox HighlightTarget = FindName($"PreviewLayout_Skills_Coin{Mode_Skills.CurrentSkillCoinIndex + 1}_Desc{Mode_Skills.CurrentSkillCoinDescIndex + 1}") as RichTextBox;
            NavigationPanel_Skills_SwitchToCoinDesc_FastSwitch_CoinDescFocusHighlightEvent(HighlightTarget);
            HighlightTarget.Focus();
        }
    }

    private void NavigationPanel_Skills_SwitchToCoinDesc(object sender, MouseButtonEventArgs e)
    {
        Border Sender = sender as Border;

        string Direction = Sender.Name.Split("NavigationPanel_Skills_CoinDesc_")[^1];
        int IndexOfCurrentCoinDesc = Mode_Skills.CurrentCoinDescs_Avalible.IndexOf(Mode_Skills.CurrentSkillCoinDescIndex);
        int TargetSwitchIndex = Direction.Equals("Next") ? IndexOfCurrentCoinDesc + 1 : IndexOfCurrentCoinDesc - 1;

        Mode_Skills.SwitchToCoinDesc(TargetSwitchIndex);

        if (DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnManualSwitch)
        {
            RichTextBox HighlightTarget = FindName($"PreviewLayout_Skills_Coin{Mode_Skills.CurrentSkillCoinIndex + 1}_Desc{Mode_Skills.CurrentSkillCoinDescIndex + 1}") as RichTextBox;
            NavigationPanel_Skills_SwitchToCoinDesc_FastSwitch_CoinDescFocusHighlightEvent(HighlightTarget);
            HighlightTarget.Focus();
        }
    }

    internal protected static void NavigationPanel_Skills_SwitchToCoinDesc_CheckAvalibles()
    {
        int IndexOfCurrentCoinDesc = Mode_Skills.CurrentCoinDescs_Avalible.IndexOf(Mode_Skills.CurrentSkillCoinDescIndex);

        // If first item -> Hide 'Previous'
        if (IndexOfCurrentCoinDesc == 0)
        {
            MainControl.NavigationPanel_Skills_CoinDesc_Previous_DisableCover.Visibility = Visible;
        }
        else
        {
            MainControl.NavigationPanel_Skills_CoinDesc_Previous_DisableCover.Visibility = Collapsed;
        }

        // If last item -> Hide 'Next'
        if ((IndexOfCurrentCoinDesc + 1) == Mode_Skills.CurrentCoinDescs_Avalible.Count)
        {
            MainControl.NavigationPanel_Skills_CoinDesc_Next_DisableCover.Visibility = Visible;
        }
        else
        {
            MainControl.NavigationPanel_Skills_CoinDesc_Next_DisableCover.Visibility = Collapsed;
        }
    }

    private void NavigationPanel_Skills_SwitchToCoinDesc_FastSwitch(object sender, MouseButtonEventArgs e)
    {
        RichTextBox Sender = sender as RichTextBox;

        string CoinNumber = $"{Sender.Name.Split("PreviewLayout_Skills_Coin")[1][0]}";
        string CoinDescNumber = $"{Sender.Name.Split("_Desc")[1]}";

        Mode_Skills.SetCoinFocus(int.Parse(CoinNumber));
        Mode_Skills.SwitchToCoinDesc(int.Parse(CoinDescNumber) - 1);

        if (DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightCoinDescsOnRightClick)
        {
            NavigationPanel_Skills_SwitchToCoinDesc_FastSwitch_CoinDescFocusHighlightEvent(Sender);
        }
    }

    internal protected static async void NavigationPanel_Skills_SwitchToCoinDesc_FastSwitch_CoinDescFocusHighlightEvent(RichTextBox TargetDesc)
    {
        TargetDesc.Background = ToColor("#01303030");

        for (int i = 255; i >= 0; i--)
        {
            string TransperacyChange = i.ToString("X");
            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

            TargetDesc.Background = ToColor($"#{TransperacyChange}{TargetDesc.Background.ToString()[3..]}");
            if(i%3 == 0) await Task.Delay(1);
        }
    }
    #endregion



    #region Passive switches
    private void Actions_Passives_SwitchToMainDesc(object sender, MouseButtonEventArgs e) => Mode_Passives.SwitchToMainDesc();
    private void Actions_Passives_SwitchToSummaryDesc(object sender, MouseButtonEventArgs e) => Mode_Passives.SwitchToSummaryDesc();
    private async void Passives_CreateSummaryDescription(object sender, MouseButtonEventArgs e)
    {
        CanSwitchID = false;

        DelegatePassives[Mode_Passives.CurrentPassiveID].SummaryDescription = "\0";
        DelegatePassives[Mode_Passives.CurrentPassiveID].EditorSummaryDescription = "";

        Mode_Passives.TargetSite_StringLine = "Summary Description";

        Editor.Text = DelegatePassives[Mode_Passives.CurrentPassiveID].EditorSummaryDescription;

        int TransperacyStartPoint = int.Parse(Resources["UITheme_ButtonsDisableCover"].ToString().Substring(1, 2), HexNumber);

        for (int i = TransperacyStartPoint; i >= 0; i--)
        {
            string TransperacyChange = i.ToString("X");
            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

            STE_DisableCover_Passives_SummaryDescription.Background = ToColor($"#{TransperacyChange}{STE_DisableCover_Passives_SummaryDescription.Background.ToString()[3..]}");

            if (i % 2 == 0) await Task.Delay(1);
        }

        STE_DisableCover_Passives_SummaryDescription.Visibility = Collapsed;
        STE_DisableCover_Passives_SummaryDescription.Background = Resources["UITheme_ButtonsDisableCover"] as SolidColorBrush;

        CanSwitchID = true;
    }
    #endregion



    #region Keywords switches
    private void Actions_Keywords_SwitchToMainDesc(object sender, MouseButtonEventArgs e) => Mode_Keywords.SwitchToMainDesc();
    private void Actions_Keywords_SwitchToSummaryDesc(object sender, MouseButtonEventArgs e) => Mode_Keywords.SwitchToSummaryDesc();
    private void Actions_Keywords_ToggleFormatInsertions(object sender, MouseButtonEventArgs e)
    {
        switch (Keywords_FormatInsertions_StackPanel.Visibility)
        {
            case Visibility.Visible:
                Keywords_FormatInsertions_StackPanel.Visibility = Visibility.Collapsed;
                Keyword_DropdownFormatInsertions_IndicatorAngle.Angle = 0;
                break;

            case Visibility.Collapsed:
                Keywords_FormatInsertions_StackPanel.Visibility = Visibility.Visible;
                Keyword_DropdownFormatInsertions_IndicatorAngle.Angle = 180;
                break;
        }
    }
    private async void Actions_Keywords_CreateSummaryDescription(object sender, MouseButtonEventArgs e)
    {
        CanSwitchID = false;

        DelegateKeywords[Mode_Keywords.CurrentKeywordID].SummaryDescription = "\0";
        DelegateKeywords[Mode_Keywords.CurrentKeywordID].EditorSummaryDescription = "";

        Mode_Keywords.TargetSite_StringLine = "Summary Description";

        Editor.Text = DelegateKeywords[Mode_Keywords.CurrentKeywordID].EditorSummaryDescription;

        int TransperacyStartPoint = int.Parse(Resources["UITheme_ButtonsDisableCover"].ToString().Substring(1, 2), HexNumber);

        for (int i = TransperacyStartPoint; i >= 0; i--)
        {
            string TransperacyChange = i.ToString("X");
            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

            STE_DisableCover_Keyword_SummaryDescription.Background = ToColor($"#{TransperacyChange}{STE_DisableCover_Keyword_SummaryDescription.Background.ToString()[3..]}");

            if (i % 2 == 0) await Task.Delay(1);
        }

        STE_DisableCover_Keyword_SummaryDescription.Visibility = Collapsed;
        STE_DisableCover_Keyword_SummaryDescription.Background = Resources["UITheme_ButtonsDisableCover"] as SolidColorBrush;

        CanSwitchID = true;
    }

    #region Bufs interactive button
    bool AllowOpacityChange = true;
    private async void Actions_Keywords_InteractiveBufsButton_Highlight(object sender, MouseEventArgs e)
    {
        AllowOpacityChange = true;
        for (double i = 0; i <= 1; i += 0.114)
        {
            if (AllowOpacityChange)
            {
                Actions_Keywords_InteractiveBufsButton_Image.Opacity = i;
                await Task.Delay(20);
            }
        }
    }

    private void Actions_Keywords_InteractiveBufsButton_StopHighlight(object sender, MouseEventArgs e)
    {
        Actions_Keywords_InteractiveBufsButton_Image.Opacity = 0;
        Keywords_InteractiveBufsButton_Scale.ScaleX = 1;
        Keywords_InteractiveBufsButton_Scale.ScaleY = 1;
        AllowOpacityChange = false;
    }

    private async void Actions_Keywords_InteractiveBufsButtonDown(object sender, MouseButtonEventArgs e)
    {
        for (double i = 1; i >= 0.98; i -= 0.01)
        {
            Keywords_InteractiveBufsButton_Scale.ScaleX = i;
            Keywords_InteractiveBufsButton_Scale.ScaleY = i;
            await Task.Delay(40);
        }
    }
    private async void Actions_Keywords_InteractiveBufsButtonUp(object sender, MouseButtonEventArgs e)
    {
        for (double i = 0.98; i <= 1; i += 0.01)
        {
            Keywords_InteractiveBufsButton_Scale.ScaleX = i;
            Keywords_InteractiveBufsButton_Scale.ScaleY = i;
            await Task.Delay(40);
        }

        //////////////////////////////////////////////////////////////////////
        var FullLinkKeyword = DelegateKeywords[Mode_Keywords.CurrentKeywordID];
        //////////////////////////////////////////////////////////////////////
        bool AnyChanges = false;
        if (!FullLinkKeyword.Description.Equals(FullLinkKeyword.EditorDescription))
        {
            FullLinkKeyword.Description = FullLinkKeyword.EditorDescription;
            MainControl.STE_Keyword_MainDescription
                .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Desc"]);
            AnyChanges = true;
        }

        if (!FullLinkKeyword.SummaryDescription.IsNull())
        {
            if (!FullLinkKeyword.SummaryDescription.Equals(FullLinkKeyword.EditorSummaryDescription))
            {
                FullLinkKeyword.SummaryDescription = FullLinkKeyword.EditorSummaryDescription;
                STE_Keyword_SummaryDescription
                        .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Summary"]);
                AnyChanges = true;
            }
        }

        if (!SWBT_Keywords_KeywordName.Text.Equals(FullLinkKeyword.Name))
        {
            FullLinkKeyword.Name = SWBT_Keywords_KeywordName.Text.Trim();
            NavigationPanel_ObjectName_Display.Text = FullLinkKeyword.Name;

            AnyChanges = true;
        }

        if (AnyChanges) Mode_Keywords.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
    }
    private void Actions_Keywords_BufsSpecialNameButton(object sender, MouseButtonEventArgs e)
    {
        
    }
    #endregion

    #endregion



    #region E.G.O Gifts switches
    private void PreviewLayout_Keywords_Bufs_Name_TextChanged(object sender, TextChangedEventArgs e)
    {
        SWBT_Keywords_KeywordName.Text = PreviewLayout_Keywords_Bufs_Name.Text;
    }

    private void EGOGiftDisplay_HotSwitchToUpgradeLevel_MouseEnter(object sender, MouseEventArgs e)
    {
        (sender as Grid).Children[2].Opacity = 1;
    }

    private void EGOGiftDisplay_HotSwitchToUpgradeLevel_MouseLeave(object sender, MouseEventArgs e)
    {
        (sender as Grid).Children[2].Opacity = 0;
    }

    private void EGOGiftDisplay_HotSwitchToUpgradeLevel_SwitchButton(object sender, MouseButtonEventArgs e)
    {
        int TargetUpgradeLevel = int.Parse($"{(sender as Grid).Name[^1]}") - 1;

        if (DelegateEGOGifts[Mode_EGOGifts.CurrentEGOGiftID].UpgradeLevelsAssociativeIDs.Count > 0)
        {
            Mode_EGOGifts.TransformToEGOGift(DelegateEGOGifts[Mode_EGOGifts.CurrentEGOGiftID].UpgradeLevelsAssociativeIDs[TargetUpgradeLevel]);
        }
    }

    private void Actions_EGOGifts_SwitchToMainDesc(object sender, MouseButtonEventArgs e) => Mode_EGOGifts.SwitchToMainDesc();
    private void Actions_EGOGifts_SwitchToSimpleDesc(object sender, MouseButtonEventArgs e) => Mode_EGOGifts.SwitchToSimpleDesc($"{(sender as Border).Name[^1]}");
    #endregion



    #region ID Switch system
    internal protected static bool SwitchToFirstItem = false;
    internal protected static bool SwitchToLastItem = false;
    internal protected static bool CanSwitchID = true;
    private void NavigationPanel_IDSwitch(object sender, MouseButtonEventArgs e)
    {
        if (CanSwitchID)
        {
            Border Sender = sender as Border;
            string Direction = Sender.Name.Split("NavigationPanel_IDSwitch_")[^1];

            bool SuccessSwitch = false;

            switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
            {
                case "Skills":
                    {
                        int IndexOfCurrentID = DelegateSkills_IDList.IndexOf(Mode_Handlers.Mode_Skills.CurrentSkillID);
                        int TargetSwitchIDIndex = Direction.Equals("Next") ? IndexOfCurrentID + 1 : IndexOfCurrentID - 1;

                        if (TargetSwitchIDIndex <= (DelegateSkills_IDList.Count() - 1) & TargetSwitchIDIndex >= 0)
                        {
                            if (!(SwitchToFirstItem | SwitchToLastItem))
                            {
                                Mode_Handlers.Mode_Skills.TransformToSkill(DelegateSkills_IDList[TargetSwitchIDIndex]);
                            }
                            else
                            {
                                Mode_Handlers.Mode_Skills.TransformToSkill(DelegateSkills_IDList[SwitchToFirstItem ? 0 : (DelegateSkills_IDList.Count - 1)]);
                            }
                            SuccessSwitch = true;
                        }

                    }
                    break;

                case "Passives":
                    {
                        int IndexOfCurrentID = DelegatePassives_IDList.IndexOf(Mode_Handlers.Mode_Passives.CurrentPassiveID);
                        int TargetSwitchIDIndex = Direction.Equals("Next") ? IndexOfCurrentID + 1 : IndexOfCurrentID - 1;

                        if (TargetSwitchIDIndex <= (DelegatePassives_IDList.Count() - 1) & TargetSwitchIDIndex >= 0)
                        {
                            if (!(SwitchToFirstItem | SwitchToLastItem))
                            {
                                Mode_Handlers.Mode_Passives.TransformToPassive(DelegatePassives_IDList[TargetSwitchIDIndex]);
                            }
                            else
                            {
                                Mode_Handlers.Mode_Passives.TransformToPassive(DelegatePassives_IDList[SwitchToFirstItem ? 0 : (DelegatePassives_IDList.Count - 1)]);
                            }
                            SuccessSwitch = true;
                        }
                    }
                    break;

                case "Keywords":
                    {
                        int IndexOfCurrentID = DelegateKeywords_IDList.IndexOf(Mode_Handlers.Mode_Keywords.CurrentKeywordID);
                        int TargetSwitchIDIndex = Direction.Equals("Next") ? IndexOfCurrentID + 1 : IndexOfCurrentID - 1;

                        if (TargetSwitchIDIndex <= (DelegateKeywords_IDList.Count() - 1) & TargetSwitchIDIndex >= 0)
                        {
                            if (!(SwitchToFirstItem | SwitchToLastItem))
                            {
                                Mode_Handlers.Mode_Keywords.TransformToKeyword(DelegateKeywords_IDList[TargetSwitchIDIndex]);
                            }
                            else
                            {
                                Mode_Handlers.Mode_Keywords.TransformToKeyword(DelegateKeywords_IDList[SwitchToFirstItem ? 0 : (DelegateKeywords_IDList.Count - 1)]);
                            }
                            SuccessSwitch = true;
                        }
                    }
                    break;

                case "E.G.O Gifts":
                    {
                        int IndexOfCurrentID = DelegateEGOGifts_IDList.IndexOf(Mode_Handlers.Mode_EGOGifts.CurrentEGOGiftID);
                        int TargetSwitchIDIndex = Direction.Equals("Next") ? IndexOfCurrentID + 1 : IndexOfCurrentID - 1;

                        if (TargetSwitchIDIndex <= (DelegateEGOGifts_IDList.Count() - 1) & TargetSwitchIDIndex >= 0)
                        {
                            if (!(SwitchToFirstItem | SwitchToLastItem))
                            {
                                Mode_Handlers.Mode_EGOGifts.TransformToEGOGift(DelegateEGOGifts_IDList[TargetSwitchIDIndex]);
                            }
                            else
                            {
                                Mode_Handlers.Mode_EGOGifts.TransformToEGOGift(DelegateEGOGifts_IDList[SwitchToFirstItem ? 0 : (DelegateEGOGifts_IDList.Count - 1)]);
                            }
                            SuccessSwitch = true;
                        }
                    }
                    break;
            }
            if (SuccessSwitch) NavigationPanel_IDSwitch_ManualInput_Stop();
        }
    }

    internal protected static void NavigationPanel_IDSwitch_CheckAvalibles()
    {
        void Check(int IndexOfCurrentID, int MaxID)
        {
            // If first item -> Hide 'Previous'
            if (IndexOfCurrentID == 0)
            {
                MainControl.NavigationPanel_IDSwitch_Previous_DisableCover.Visibility = Visible;
            }
            else MainControl.NavigationPanel_IDSwitch_Previous_DisableCover.Visibility = Collapsed;

            // If last item -> Hide 'Next'
            if ((IndexOfCurrentID + 1) == MaxID)
            {
                MainControl.NavigationPanel_IDSwitch_Next_DisableCover.Visibility = Visible;
            }
            else MainControl.NavigationPanel_IDSwitch_Next_DisableCover.Visibility = Collapsed;
        }


        switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
        {
            case "Skills":
                {
                    int IndexOfCurrentID = DelegateSkills_IDList.IndexOf(Mode_Handlers.Mode_Skills.CurrentSkillID);
                    int MaximumID = DelegateSkills_IDList.Count;

                    Check(IndexOfCurrentID, MaximumID);
                }
                break;

            case "Passives":
                {
                    int IndexOfCurrentID = DelegatePassives_IDList.IndexOf(Mode_Handlers.Mode_Passives.CurrentPassiveID);
                    int MaximumID = DelegatePassives_IDList.Count;

                    Check(IndexOfCurrentID, MaximumID);
                }
                break;

            case "Keywords":
                {
                    int IndexOfCurrentID = DelegateKeywords_IDList.IndexOf(Mode_Handlers.Mode_Keywords.CurrentKeywordID);
                    int MaximumID = DelegateKeywords_IDList.Count;

                    Check(IndexOfCurrentID, MaximumID);
                }
                break;

            case "E.G.O Gifts":
                {
                    int IndexOfCurrentID = DelegateEGOGifts_IDList.IndexOf(Mode_Handlers.Mode_EGOGifts.CurrentEGOGiftID);
                    int MaximumID = DelegateEGOGifts_IDList.Count;

                    Check(IndexOfCurrentID, MaximumID);
                }
                break;

            default: break;
        }
    }

    private void NavigationPanel_IDSwitch_ToLast(object sender, MouseButtonEventArgs e)
    {
        SwitchToLastItem = true;
        NavigationPanel_IDSwitch(sender, e);
        SwitchToLastItem = false;
    }

    private void NavigationPanel_IDSwitch_ToFirst(object sender, MouseButtonEventArgs e)
    {
        SwitchToFirstItem = true;
        NavigationPanel_IDSwitch(sender, e);
        SwitchToFirstItem = false;
    }

    /// <summary>
    /// -> action in 'Manual ID Switch' region
    /// </summary>
    private void NavigationPanel_IDSwitch_ManualInput_Start(object sender, MouseButtonEventArgs e)
    {
        string IDText = STE_NavigationPanel_ObjectID_Display.GetText();
        if (!IDText.Equals(UILanguageLoader.LoadedLanguage.DefaultInsertionText))
        {
            STE_NavigationPanel_ObjectID_Display.Visibility = Collapsed;
            STE_NavigationPanel_ObjectID_Display_IDCopied.Visibility = Collapsed;

            NavigationPanel_IDSwitch_ManualInput_Textfield.CaretIndex = NavigationPanel_IDSwitch_ManualInput_Textfield.Text.Length;
            NavigationPanel_IDSwitch_ManualInput_Textfield.Visibility = Visible;

            NavigationPanel_IDSwitch_ManualInput_Textfield.Focus();
        }
    }

    internal protected static void NavigationPanel_IDSwitch_ManualInput_Stop()
    {
        MainControl.STE_NavigationPanel_ObjectID_Display.Visibility = Visible;
        MainControl.STE_NavigationPanel_ObjectID_Display_IDCopied.Visibility = Visible;
        MainControl.NavigationPanel_IDSwitch_ManualInput_Textfield.Visibility = Collapsed;

        UnfocusTextBox(MainControl.NavigationPanel_IDSwitch_ManualInput_Textfield);

        MainControl.NavigationPanel_IDSwitch_ManualInput_Textfield.Text = "";
    }

    bool AlreadyAnimatingCopiedInfo = false;
    private async void CopyID(object sender, MouseButtonEventArgs e)
    {
        if (!NavigationPanel_IDSwitch_ManualInput_Textfield.IsFocused)
        {
            try
            {
                string IDText = STE_NavigationPanel_ObjectID_Display.GetText();
                if (!IDText.Equals(UILanguageLoader.LoadedLanguage.DefaultInsertionText))
                {
                    Clipboard.SetText(STE_NavigationPanel_ObjectID_Display.GetText());

                    if (!AlreadyAnimatingCopiedInfo)
                    {
                        AlreadyAnimatingCopiedInfo = true;
                        for (int i = 255; i >= 0; i--)
                        {
                            string TransperacyChange = i.ToString("X");
                            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

                            STE_NavigationPanel_ObjectID_Display.Foreground = ToColor($"#{TransperacyChange}{STE_NavigationPanel_ObjectID_Display.Foreground.ToString()[3..]}");

                            if (i % 3 == 0) await Task.Delay(1);
                        }

                        for (int i = 0; i <= 255; i++)
                        {
                            string TransperacyChange = i.ToString("X");
                            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

                            STE_NavigationPanel_ObjectID_Display_IDCopied.Foreground = ToColor($"#{TransperacyChange}{STE_NavigationPanel_ObjectID_Display_IDCopied.Foreground.ToString()[3..]}");

                            if (i % 3 == 0) await Task.Delay(1);
                        }

                        await Task.Delay(460);

                        for (int i = 255; i >= 0; i--)
                        {
                            string TransperacyChange = i.ToString("X");
                            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

                            STE_NavigationPanel_ObjectID_Display_IDCopied.Foreground = ToColor($"#{TransperacyChange}{STE_NavigationPanel_ObjectID_Display_IDCopied.Foreground.ToString()[3..]}");

                            if (i % 3 == 0) await Task.Delay(1);
                        }

                        for (int i = 0; i <= 255; i++)
                        {
                            string TransperacyChange = i.ToString("X");
                            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

                            STE_NavigationPanel_ObjectID_Display.Foreground = ToColor($"#{TransperacyChange}{STE_NavigationPanel_ObjectID_Display.Foreground.ToString()[3..]}");

                            if (i % 3 == 0) await Task.Delay(1);
                        }

                        AlreadyAnimatingCopiedInfo = false;
                    }
                }
            }
            catch { }
        }
    }
    #endregion



    #region Shared
    private void ChangeObjectName(object sender, MouseButtonEventArgs e)
    {
        switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
        {
            case "Skills":

                /////////////////////////////////////////////////////////////////////////////////////////////
                var FullLinkSkills = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel];
                /////////////////////////////////////////////////////////////////////////////////////////////

                if (!SWBT_Skills_MainSkillName.Text.Equals(FullLinkSkills.Name))
                {
                    FullLinkSkills.Name = SWBT_Skills_MainSkillName.Text.Trim();
                    NavigationPanel_ObjectName_Display.Text = FullLinkSkills.Name;

                    Mode_Skills.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                }

                break;


            case "Passives":

                ///////////////////////////////////////////////////////////////////////
                var FullLinkPassives = DelegatePassives[Mode_Passives.CurrentPassiveID];
                ///////////////////////////////////////////////////////////////////////
                
                if (!SWBT_Passives_MainPassiveName.Text.Equals(FullLinkPassives.Name))
                {
                    FullLinkPassives.Name = SWBT_Passives_MainPassiveName.Text.Trim();
                    NavigationPanel_ObjectName_Display.Text = FullLinkPassives.Name;

                    Mode_Passives.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                }

                break;

            case "Keywords":

                ///////////////////////////////////////////////////////////////////////
                var FullLinkKeywords = DelegateKeywords[Mode_Keywords.CurrentKeywordID];
                ///////////////////////////////////////////////////////////////////////

                if (!SWBT_Keywords_KeywordName.Text.Equals(FullLinkKeywords.Name))
                {
                    FullLinkKeywords.Name = SWBT_Keywords_KeywordName.Text.Trim();
                    NavigationPanel_ObjectName_Display.Text = FullLinkKeywords.Name;

                    Mode_Keywords.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                }

                break;

            case "E.G.O Gifts":

                ///////////////////////////////////////////////////////////////////////
                var FullLinkEGOGifts = DelegateEGOGifts[Mode_EGOGifts.CurrentEGOGiftID];
                ///////////////////////////////////////////////////////////////////////

                if (!SWBT_EGOGifts_EGOGiftName.Text.Equals(FullLinkEGOGifts.Name))
                {
                    FullLinkEGOGifts.Name = SWBT_EGOGifts_EGOGiftName.Text.Trim();
                    NavigationPanel_ObjectName_Display.Text = FullLinkEGOGifts.Name;

                    Mode_EGOGifts.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                }

                break;
        }
    }

    internal protected static void FocusOnFile(FileInfo Target)
    {
        CurrentFile = Target;
        CurrentFileEncoding = Target.GetFileEncoding();
        MainControl.JsonFilePath.Text = CurrentFile.FullName;
        MainControl.JsonFilePath.CaretIndex = MainControl.JsonFilePath.Text.Length;
    }
    internal protected static void FocusOnFile(string TargetPath)
    {
        CurrentFile = new FileInfo(TargetPath);
        MainControl.JsonFilePath.Text = CurrentFile.FullName;
    }

    internal protected void Actions_FILE_SelectFile_Acutal()
    {
        OpenFileDialog JsonFileSelector = new Microsoft.Win32.OpenFileDialog();
        JsonFileSelector.DefaultExt = ".json";
        JsonFileSelector.Filter = "Text documents (.json)|*.json";

        bool? Result = JsonFileSelector.ShowDialog();

        if (Result == true)
        {
            string Filename = JsonFileSelector.FileName;

            FileInfo TemplateTarget = new FileInfo(Filename);

            string CheckName = TemplateTarget.Name.RemovePrefix(["JP_", "KR_", "EN_"]);

            if (File.ReadAllText(TemplateTarget.FullName).Contains(@"""Template Marker"": ""(Don't remove)"""))
            {
                CheckName = "Skills";
            }

            if (CheckName.StartsWith("Skills"))
            {
                FocusOnFile(TemplateTarget);

                Mode_Skills.LoadStructure(TemplateTarget);
            }
            else if (CheckName.StartsWith("Passive"))
            {
                FocusOnFile(TemplateTarget);

                Mode_Passives.LoadStructure(CurrentFile);
            }
            else if (CheckName.StartsWithOneOf(["BattleKeywords", "Bufs"]))
            {
                FocusOnFile(TemplateTarget);

                Mode_Keywords.LoadStructure(CurrentFile);
            }
            else if (CheckName.StartsWith("EGOgift"))
            {
                FocusOnFile(TemplateTarget);

                Mode_EGOGifts.LoadStructure(CurrentFile);
            }
        }
    }

    private void Actions_FILE_SelectFile(object sender, MouseButtonEventArgs e)
    {
        CheckUnsavedChanges(SelectFileOnEnd: true);
    }
    #endregion



    #region Technical
    internal protected static Assembly LCLocalizationTaskAbsolute = Assembly.GetExecutingAssembly();
    internal protected static string LoadFromEmbeddedResources(string FullName)
    {
        using (Stream stream = LCLocalizationTaskAbsolute.GetManifestResourceStream(FullName))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    internal protected static void InitializeDefaultResource()
    {
        DefaultTheme = LoadFromEmbeddedResources("LC_Localization_Task_Absolute.Default.Theme.Theme.json");
        DefaultLanguage = LoadFromEmbeddedResources("LC_Localization_Task_Absolute.Default.Language.English.json");

        UILanguageLoader.InitializeUILanguage("хуй]");
    }

    

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        //rin($"Autohide:{UIThemesLoader.LoadedTheme.AutoHideBackgroundOnMinWidth}; Width:{Width} (>{Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinWidth}), Height:{Height} (>{Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinHeight})");
        if ((Width <= Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinWidth + 2 & Height <= Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinHeight + 2)
            | (((UIThemesLoader.LoadedTheme != null ? UIThemesLoader.LoadedTheme.AutoHideBackgroundOnMinWidth : false) & Width <= Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinWidth + 2)))
        {
            BackgroundImage.Visibility = Visibility.Collapsed;
        }
        else
        {
            BackgroundImage.Visibility = Visibility.Visible;
        }
        NewWindowSizes.Rect = new Rect(0, 0, Width, Height);
    }


    internal protected void CheckUnsavedChanges(bool ExitOnEnd = false, bool SelectFileOnEnd = false)
    {
        int UnsavedChangesCount = 0;
        string UnsavedChangesInfo = "";

        switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
        {
            case "Passives":
                foreach (KeyValuePair<int, BaseTypes.Type_Passives.Passive> CheckPassive in DelegatePassives)
                {
                    bool UnsavedChangesInPassiveDesc = false;
                    bool UnsavedChangesInPassiveSummary = false;

                    if (!CheckPassive.Value.Description.Equals(CheckPassive.Value.EditorDescription))
                    {
                        UnsavedChangesInPassiveDesc = true;
                        UnsavedChangesCount++;
                    }
                    if (!CheckPassive.Value.SummaryDescription.IsNull())
                    {
                        if (!CheckPassive.Value.SummaryDescription.Equals(CheckPassive.Value.EditorSummaryDescription))
                        {
                            UnsavedChangesInPassiveSummary = true;
                            UnsavedChangesCount++;
                        }
                    }

                    if (UnsavedChangesInPassiveDesc | UnsavedChangesInPassiveSummary)
                    {
                        //UnsavedChangesInfo += $"\n\n<b>ID</b> <color=#f8c200>{CheckPassive.Key}</color> 「<color=#afbff9>{CheckPassive.Value.Name}</color>」";
                        UnsavedChangesInfo += UILanguageLoader.LangUnsavedChangesInfo.Passives.IDHeader.Exform(CheckPassive.Key, CheckPassive.Value.Name);
                        if (UnsavedChangesInPassiveDesc)
                        {
                            //UnsavedChangesInfo += "\n  > Главное описание";
                            UnsavedChangesInfo += UILanguageLoader.LangUnsavedChangesInfo.Passives.MainDesc;
                        }
                        if (UnsavedChangesInPassiveSummary)
                        {
                            //UnsavedChangesInfo += "\n  > Суммарное описание";
                            UnsavedChangesInfo += UILanguageLoader.LangUnsavedChangesInfo.Passives.SummaryDesc;
                        }
                    }
                }

                break;


            case "Skills":
                foreach (KeyValuePair<int, Dictionary<int, BaseTypes.Type_Skills.UptieLevel>> CheckSkill in DelegateSkills)
                {
                    bool AlreadyAddedThisID = false;
                    string SkillName = "";
                    foreach (var UptieLevel in CheckSkill.Value.Values)
                    {
                        SkillName = UptieLevel.Name;
                        bool AnythingChanged = false;
                        
                        if (!UptieLevel.Description.Equals(UptieLevel.EditorDescription))
                        {
                            AnythingChanged = true;
                        }
                        
                        if (!UptieLevel.Coins.IsNull())
                        {
                            foreach (var Coin in UptieLevel.Coins)
                            {
                                if (!Coin.IsNull())
                                {
                                    if (!Coin.CoinDescriptions.IsNull())
                                    {
                                        foreach (var CoinDesc in Coin.CoinDescriptions)
                                        {
                                            if (!CoinDesc.IsNull())
                                            {
                                                if (!CoinDesc.Description.IsNull())
                                                {
                                                    if (!CoinDesc.Description.Equals(CoinDesc.EditorDescription))
                                                    {
                                                        AnythingChanged = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        if (AnythingChanged)
                        {
                            if (!AlreadyAddedThisID)
                            {
                                //UnsavedChangesInfo += $"\n\n<b>ID</b> <color=#f8c200>{CheckSkill.Key}</color> 「<color=#afbff9>{SkillName}</color>」";
                                UnsavedChangesInfo += UILanguageLoader.LangUnsavedChangesInfo.Skills.IDHeader.Exform(CheckSkill.Key, SkillName);
                                AlreadyAddedThisID = true;
                            }
                            //UnsavedChangesInfo += $"\n  > Уровень связи {UptieLevel.Uptie}";
                            UnsavedChangesInfo += UILanguageLoader.LangUnsavedChangesInfo.Skills.UptieLevel.Extern($"{UptieLevel.Uptie}");
                        }
                    }
                }
                break;


            case "Keywords":
                foreach (KeyValuePair<string, BaseTypes.Type_Keywords.Keyword> CheckKeyword in DelegateKeywords)
                {
                    bool UnsavedChangesInKeywordDesc = false;
                    bool UnsavedChangesInKeywordSummary = false;

                    if (!CheckKeyword.Value.Description.Equals(CheckKeyword.Value.EditorDescription))
                    {
                        UnsavedChangesInKeywordDesc = true;
                        UnsavedChangesCount++;
                    }
                    if (!CheckKeyword.Value.SummaryDescription.IsNull())
                    {
                        if (!CheckKeyword.Value.SummaryDescription.Equals(CheckKeyword.Value.EditorSummaryDescription))
                        {
                            UnsavedChangesInKeywordSummary = true;
                            UnsavedChangesCount++;
                        }
                    }

                    if (UnsavedChangesInKeywordDesc | UnsavedChangesInKeywordSummary)
                    {
                        //UnsavedChangesInfo += $"\n\n<b>ID</b> <color=#f8c200>{CheckKeyword.Key}</color> 「<color=#afbff9>{CheckKeyword.Value.Name}</color>」";
                        UnsavedChangesInfo += UILanguageLoader.LangUnsavedChangesInfo.Keywords.IDHeader.Exform(CheckKeyword.Key, CheckKeyword.Value.Name);
                        if (UnsavedChangesInKeywordDesc)
                        {
                            //UnsavedChangesInfo += "\n  > Главное описание";
                            UnsavedChangesInfo += UILanguageLoader.LangUnsavedChangesInfo.Keywords.MainDesc;
                        }
                        if (UnsavedChangesInKeywordSummary)
                        {
                            //UnsavedChangesInfo += "\n  > Суммарное описание";
                            UnsavedChangesInfo += UILanguageLoader.LangUnsavedChangesInfo.Passives.MainDesc;
                        }
                    }
                }
                break;


            case "E.G.O Gifts":
                if (DelegateEGOGifts.Keys.Count != 0)
                {
                    foreach (KeyValuePair<int, BaseTypes.Type_EGOGifts.EGOGift> CheckEGOGift in DelegateEGOGifts)
                    {
                        string ChangedSimpleDescs = "";
                        bool ChangedDesc = false;
                        if (!CheckEGOGift.Value.Description.Equals(CheckEGOGift.Value.EditorDescription))
                        {
                            ChangedDesc = true;
                        }
                        if (!CheckEGOGift.Value.SimpleDescriptions.IsNull())
                        {
                            int SimpleDescIndexer = 1;
                            foreach (var SimpleDesc in CheckEGOGift.Value.SimpleDescriptions)
                            {
                                if (!SimpleDesc.Description.Equals(SimpleDesc.EditorDescription))
                                {
                                    //ChangedSimpleDescs += $"\n  > Простое описание №{SimpleDescIndexer}\n";
                                    ChangedSimpleDescs += UILanguageLoader.LangUnsavedChangesInfo.EGOGifts.SimpleDesc.Extern(SimpleDescIndexer);
                                }

                                SimpleDescIndexer++;
                            }
                        }

                        if (ChangedDesc | !ChangedSimpleDescs.Equals(""))
                        {
                            //UnsavedChangesInfo += $"\n<b>ID</b> <color=#f8c200>{CheckEGOGift.Key}</color> 「<color=#afbff9>{CheckEGOGift.Value.Name}</color>」";
                            UnsavedChangesInfo += UILanguageLoader.LangUnsavedChangesInfo.EGOGifts.IDHeader.Exform(CheckEGOGift.Key, CheckEGOGift.Value.Name);
                            if (ChangedDesc)
                            {
                                //UnsavedChangesInfo += "\n  > Описание";
                                UnsavedChangesInfo += UILanguageLoader.LangUnsavedChangesInfo.EGOGifts.MainDesc;
                            }
                            if (!ChangedSimpleDescs.Equals(""))
                            {
                                UnsavedChangesInfo += ChangedSimpleDescs;
                            }
                        }
                    }
                }
                break;
        }
        if (!UnsavedChangesInfo.Equals(""))
        {
            DTE_UnsavedChangesInfo.SetRichText(UnsavedChangesInfo.Trim());
            BackgroundCover_UnsavedChanges.Visibility = Visible;
            if (SelectFileOnEnd) SelectFileInstead = true;
        }
        else
        {
            if (ExitOnEnd) Application.Current.Shutdown();
            if (SelectFileOnEnd) Actions_FILE_SelectFile_Acutal();
        }
    }
    internal protected bool SelectFileInstead = false;
    private void Window_DragMove(object sender, MouseButtonEventArgs e) => this.DragMove();
    private void Minimize(object sender, MouseButtonEventArgs e) => WindowState = WindowState.Minimized;
    private void Shutdown(object sender, MouseButtonEventArgs e)
    {
        CheckUnsavedChanges(ExitOnEnd: true);
    }
    private void UnsavedChangesDialog_ConfirmProceed(object sender, MouseButtonEventArgs e)
    {
        if (SelectFileInstead)
        {
            Actions_FILE_SelectFile_Acutal();
            BackgroundCover_UnsavedChanges.Visibility = Collapsed;
            SelectFileInstead = false;
        }
        else Application.Current.Shutdown();
    }

    private void UnsavedChangesDialog_Cancel(object sender, MouseButtonEventArgs e)
    {
        BackgroundCover_UnsavedChanges.Visibility = Collapsed;
    }



    private void ProcessLogicalTree(object current)
    {
        string elementInfo = current.GetType().Name;

        if (current is FrameworkElement)
        {
            string ElementName = (current as FrameworkElement).Name;
            if (ElementName.StartsWithOneOf(["SWBT"]))
            {
                SwitchButtons_TextFieldsControl.Add(current);
            }
            else if (ElementName.StartsWith("PreviewLayout_") & current is RichTextBox)
            {
                PreviewLayoutsList.Add(current as RichTextBox);
                //(current as RichTextBox).Style = FindResource("LimbusType") as Style;

                if (ElementName.Equals("PreviewLayout_EGOGifts"))
                {
                    (current as RichTextBox).SetValue(Paragraph.LineHeightProperty, 24.9);
                }
                else
                {
                    (current as RichTextBox).SetValue(Paragraph.LineHeightProperty, 27.0);
                }
            }
            else if (ElementName.StartsWith("Special_PreviewLayout_") & current is RichTextBox)
            {
                PreviewLayoutsList.Add(current as RichTextBox);
                //(current as RichTextBox).Style = FindResource("LimbusType") as Style;
                if (ElementName.Equals("Special_PreviewLayout_Keywords_BattleKeywords_Desc"))
                {
                    (current as RichTextBox).SetValue(Paragraph.LineHeightProperty, 25.0);
                }
                else
                {
                    (current as RichTextBox).SetValue(Paragraph.LineHeightProperty, 20.0);
                }
            }
            else if (ElementName.StartsWith("STE_ContextMenu_"))
            {

            }

            if (current is TextBox)
            {
                //rin($"  Defined TextBox: {(current as TextBox).Name}");
                FocusableTextBoxes.Add(current as TextBox);
            }
        }


        DependencyObject dependencyObject = current as DependencyObject;

        if (dependencyObject != null)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(dependencyObject))
            {
                ProcessLogicalTree(child);
            }
        }
    }

    // Auto hide shadow text for textfields
    private void SWBT_TextChanged_Shared(object sender, TextChangedEventArgs e)
    {
        TextBox Sender = sender as TextBox;
        Grid TagetSite = Sender.Parent as Grid;
        RichTextBox ShadowText = TagetSite.Children[0] as RichTextBox;

        ShadowText.Visibility = Sender.Text switch
        {
            "" => Visible,
            _  => Collapsed,
        };

        if (Sender.Name.Contains("Keywords_FormatInsertion"))
        {
            string FormatInsertionNumber = Sender.Name.Split("Keywords_FormatInsertion_")[^1];

            LimbusPreviewFormatter.FormatInsertions[FormatInsertionNumber] = Sender.Text.Equals("") ? $"{{{FormatInsertionNumber}}}" : Sender.Text;
            PullUpdatePreview(Editor.Text);
        }

        if (Sender.Name.Equals("SWBT_Keywords_KeywordName") & !PreviewLayout_Keywords_Bufs_Name.IsFocused)
        {
            PreviewLayout_Keywords_Bufs_Name.Text = SWBT_Keywords_KeywordName.Text;
            PreviewLayout_Keywords_BattleKeywords_Name.Text = SWBT_Keywords_KeywordName.Text;
        }

        if (Sender.Name.Equals("SWBT_EGOGifts_EGOGiftName")) EGOGiftName_PreviewLayout.Text = SWBT_EGOGifts_EGOGiftName.Text;
    }

    #region Mouse and Keyboard shortcuts
    internal protected static bool IsAnyTextBoxFocused()
    {
        if (FocusableTextBoxes.Where(textbox => textbox.IsFocused == true).Any()) return true;
        else return false;
    }
    internal protected static void UnfocusAllTextBoxes()
    {
        IEnumerable<TextBox> FocusedTextBoxes = FocusableTextBoxes.Where(textbox => textbox.IsFocused == true);

        if (FocusedTextBoxes.Any())
        {
            foreach (TextBox FocusedTextBox in FocusedTextBoxes) UnfocusTextBox(FocusedTextBox);
        }
    }


    internal protected static bool IsQuotesConvertedInClipboard = false;
    internal protected static string QuotesClipboardOldText = "";
    internal protected static bool IsCtrlSPressed = false;
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Automatically remove backslash from quotes when insert
        if (Keyboard.IsKeyDown(Key.LeftCtrl) & Keyboard.IsKeyDown(Key.V) & Editor.IsFocused)
        {
            string ClipboardText = Clipboard.GetText();

            rin($"\n\n\nInput: {ClipboardText}");

            if (ClipboardText.Contains(@"\""") | ClipboardText.Contains(@"\n"))
            {
                QuotesClipboardOldText = ClipboardText;
                string Replaced = ClipboardText.Replace(@"\""", @"""").Replace(@"\n", "\n");
                rin($"Replace: {Replaced}");
                Clipboard.SetText(Replaced);
                IsQuotesConvertedInClipboard = true;
            }

            // ... -> Editor_TextChanged()
        }

        if (Keyboard.IsKeyDown(Key.LeftCtrl) & Keyboard.IsKeyDown(Key.P))
        {
            if (MakeLimbusPreviewScan.IsHitTestVisible) SavePreviewlayoutScan();
        }

        #region Files saving
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
            if (e.Key == Key.S && !IsCtrlSPressed)
            {
                IsCtrlSPressed = true;

                switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
                {
                    case "Skills":

                        if (PreviewUpdate_TargetSite.Equals(PreviewLayout_Skills_MainDesc))
                        {
                            DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Description = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].EditorDescription;

                            UILanguage["Right Menu — Skill Desc"].SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Skill Desc"]);

                            Mode_Skills.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                        }
                        else
                        {
                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            var FullLinkSkill = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Coins[Mode_Skills.CurrentSkillCoinIndex].CoinDescriptions;
                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                            FullLinkSkill[Mode_Skills.CurrentSkillCoinDescIndex].Description = FullLinkSkill[Mode_Skills.CurrentSkillCoinDescIndex].EditorDescription;

                            if (!FullLinkSkill.Where(x => !x.Description.Equals(x.EditorDescription)).Any())
                            {
                                (MainControl.FindName($"STE_Skills_Coin_{Mode_Skills.CurrentSkillCoinIndex + 1}") as RichTextBox)
                                    .SetRichText(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"]);
                            }

                            if (UILanguageLoader.DynamicTypeElements.ContainsKey("Right Menu — Skill Coin Desc Number"))
                            {
                                MainControl.STE_Skills_Coin_DescNumberDisplay.SetRichText(UILanguageLoader.DynamicTypeElements["Right Menu — Skill Coin Desc Number"].Extern(Mode_Skills.CurrentSkillCoinDescIndex + 1));
                            }

                            Mode_Skills.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                        }

                        break;


                    case "Passives":

                        //////////////////////////////////////////////////////////////////////
                        var FullLinkPassive = DelegatePassives[Mode_Passives.CurrentPassiveID];
                        //////////////////////////////////////////////////////////////////////

                        if (Mode_Passives.TargetSite_StringLine.Equals("Main Description"))
                        {
                            FullLinkPassive.Description = FullLinkPassive.EditorDescription;

                            MainControl.STE_Passives_MainDescription
                                .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Passive Desc"]);

                            Mode_Passives.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                        }
                        else if (Mode_Passives.TargetSite_StringLine.Equals("Summary Description"))
                        {
                            FullLinkPassive.SummaryDescription = FullLinkPassive.EditorSummaryDescription;

                            STE_Passives_SummaryDescription
                                .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Passive Summary"]);

                            Mode_Passives.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                        }

                        break;


                    case "Keywords":
                        //////////////////////////////////////////////////////////////////////
                        var FullLinkKeyword = DelegateKeywords[Mode_Keywords.CurrentKeywordID];
                        //////////////////////////////////////////////////////////////////////

                        if (Mode_Keywords.TargetSite_StringLine.Equals("Main Description"))
                        {
                            FullLinkKeyword.Description = FullLinkKeyword.EditorDescription;

                            MainControl.STE_Keyword_MainDescription
                                .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Desc"]);

                            Mode_Keywords.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                        }
                        else if (Mode_Keywords.TargetSite_StringLine.Equals("Summary Description"))
                        {
                            FullLinkKeyword.SummaryDescription = FullLinkKeyword.EditorSummaryDescription;

                            STE_Keyword_SummaryDescription
                                .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Summary"]);

                            Mode_Keywords.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                        }

                        break;


                    case "E.G.O Gifts":
                        //////////////////////////////////////////////////////////////////////
                        var FullLinkEGOGift = DelegateEGOGifts[Mode_EGOGifts.CurrentEGOGiftID];
                        //////////////////////////////////////////////////////////////////////

                        if (Mode_EGOGifts.TargetSite_StringLine.Equals("Main Description"))
                        {
                            FullLinkEGOGift.Description = FullLinkEGOGift.EditorDescription;

                            MainControl.STE_EGOGift_MainDescription
                                .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — E.G.O Gift Desc"]);

                            Mode_EGOGifts.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                        }
                        else
                        {
                            string SimpleDescNumber = $"{Mode_EGOGifts.TargetSite_StringLine[^1]}";

                            int TargetSimpleDescIndex = int.Parse(SimpleDescNumber) - 1;

                            FullLinkEGOGift.SimpleDescriptions[TargetSimpleDescIndex].Description = FullLinkEGOGift.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription;

                            (MainControl.FindName($"STE_EGOGift_SimpleDescription{SimpleDescNumber}") as RichTextBox)
                                .SetRichText(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — E.G.O Gift Simple Desc {SimpleDescNumber}"]);


                            Mode_EGOGifts.DeserializedInfo.MarkSerialize(CurrentFile.FullName);
                        }

                        break;
                }
            }
        }
        #endregion

        else if (e.Key == Key.Left | e.Key == Key.Right)
        {
            if (e.Key == Key.Right)
            {
                if (!IsAnyTextBoxFocused()) NavigationPanel_IDSwitch(NavigationPanel_IDSwitch_Next, null);
            }
            else if (e.Key == Key.Left)
            {
                if (!IsAnyTextBoxFocused()) NavigationPanel_IDSwitch(NavigationPanel_IDSwitch_Previous, null);
            }
        }
        else if (e.Key == Key.Escape)
        {
            if (NavigationPanel_IDSwitch_ManualInput_Textfield.IsFocused)
            {
                NavigationPanel_IDSwitch_ManualInput_Stop();
            }
            UnfocusAllTextBoxes();
        }

        #region Manual ID Switch
        else if (e.Key == Key.Enter)
        {
            if (NavigationPanel_IDSwitch_ManualInput_Textfield.IsFocused)
            {
                string IDString = NavigationPanel_IDSwitch_ManualInput_Textfield.Text.Trim();

                Dictionary<string, int> TargetSite_NameIDs = Mode_Handlers.Upstairs.ActiveProperties.Key switch
                {
                    "Skills"   => Mode_Skills  .Skills_NameIDs,
                    "Passives" => Mode_Passives.Passives_NameIDs,
                    "E.G.O Gifts" => Mode_EGOGifts.EGOGifts_NameIDs,
                    _ => new Dictionary<string, int> { }
                };

                int RegularObjectTargetID = -1;
                string KeywordObjectTargetID = "";

                if (Mode_Handlers.Upstairs.ActiveProperties.Key.Equals("Keywords"))
                {
                    KeywordObjectTargetID = IDString;

                    if (Mode_Keywords.Keywords_NameIDs.ContainsKey(KeywordObjectTargetID))
                    {
                        KeywordObjectTargetID = Mode_Keywords.Keywords_NameIDs[KeywordObjectTargetID];
                    }
                    else
                    {
                        if (!DelegateKeywords_IDList.Contains(KeywordObjectTargetID))
                        {
                            RegularObjectTargetID = -1;
                        }
                    }
                }
                else
                {
                    try
                    {
                        RegularObjectTargetID = int.Parse(IDString);
                    }
                    catch
                    {
                        if (TargetSite_NameIDs.ContainsKey(IDString))
                        {
                            RegularObjectTargetID = TargetSite_NameIDs[IDString];
                        }
                    }
                }

                if (!RegularObjectTargetID.Equals(-1))
                {
                    switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
                    {
                        case "Skills":
                            Mode_Handlers.Mode_Skills.TransformToSkill(RegularObjectTargetID);
                            break;

                        case "Passives":
                            Mode_Handlers.Mode_Passives.TransformToPassive(RegularObjectTargetID);
                            break;
                        
                        case "E.G.O Gifts":
                            Mode_Handlers.Mode_EGOGifts.TransformToEGOGift(RegularObjectTargetID);
                            break;
                    }
                }
                // Else if Keywords
                else if (!KeywordObjectTargetID.Equals("") & DelegateKeywords_IDList.Contains(KeywordObjectTargetID))
                {
                    Mode_Handlers.Mode_Keywords.TransformToKeyword(KeywordObjectTargetID);
                }

                NavigationPanel_IDSwitch_ManualInput_Stop();
            }
        }
        #endregion
    }

    private void MakeLimbusPreviewScan_Do(object sender, MouseButtonEventArgs e)
    {
        SurfaceScrollPreview_Skills.ReconnectAsChildTo(SkillsPreviewFreeBordersCanvas);
        if (Configurazione.DeltaConfig.ScanParameters.AreaWidth == 0)
        {
            PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;
        }
        else
        {
            PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Configurazione.DeltaConfig.ScanParameters.AreaWidth;
        }

        
        SavePreviewlayoutScan();


        SurfaceScrollPreview_Skills.ReconnectAsChildTo(PreviewLayoutGrid_Skills);
        PreviewLayoutGrid_Skills_ContentControlStackPanel.Width = Mode_Skills.LastRegisteredWidth;
    }

    private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.S) IsCtrlSPressed = false;
    }

    private new async void MouseDownEvent(object sender, MouseButtonEventArgs e)
    {
        switch (e.ChangedButton)
        {
            case MouseButton.XButton1: //Back
                PreviewLayout_Keywords_Bufs_Name.Focusable = false;

                if (Mode_Handlers.Upstairs.ActiveProperties.Key.Equals("Skills") & Keyboard.IsKeyDown(Key.LeftShift))
                {
                    int currentuptie = Mode_Skills.CurrentSkillUptieLevel;
                    List<int> avalible = new();
                    foreach (var i in DelegateSkills[Mode_Skills.CurrentSkillID])
                    {
                        avalible.Add(i.Value.Uptie);
                    }

                    int indexof = avalible.IndexOf(currentuptie);
                    int previndex = indexof - 1;
                    if (previndex != -1)
                    {
                        if (avalible[previndex] != Mode_Skills.CurrentSkillUptieLevel)
                        {
                            Mode_Skills.TransformToSkill(Mode_Skills.CurrentSkillID, avalible[previndex]);
                        }
                    }
                }
                else
                {
                    NavigationPanel_IDSwitch(NavigationPanel_IDSwitch_Previous, null);
                }

                await Task.Delay(100); PreviewLayout_Keywords_Bufs_Name.Focusable = true;
                break;

            case MouseButton.XButton2: //Forward
                PreviewLayout_Keywords_Bufs_Name.Focusable = false;
                if (Mode_Handlers.Upstairs.ActiveProperties.Key.Equals("Skills") & Keyboard.IsKeyDown(Key.LeftShift))
                {
                    int currentuptie = Mode_Skills.CurrentSkillUptieLevel;
                    List<int> avalible = new();
                    foreach (var i in DelegateSkills[Mode_Skills.CurrentSkillID])
                    {
                        avalible.Add(i.Value.Uptie);
                    }

                    int indexof = avalible.IndexOf(currentuptie);
                    int nextindex = indexof + 1;
                    if (avalible.Count >= nextindex + 1)
                    {
                        if (avalible[nextindex] != Mode_Skills.CurrentSkillUptieLevel)
                        {
                            Mode_Skills.TransformToSkill(Mode_Skills.CurrentSkillID, avalible[nextindex]);
                        }
                    }
                }
                else
                {
                    NavigationPanel_IDSwitch(NavigationPanel_IDSwitch_Next, null);
                }

                await Task.Delay(100); PreviewLayout_Keywords_Bufs_Name.Focusable = true;
                break;

            case MouseButton.Left:
                if (NavigationPanel_IDSwitch_ManualInput_Textfield.IsFocused)
                {
                    if (!IDButton_Container.IsMouseOver)
                    {
                        NavigationPanel_IDSwitch_ManualInput_Stop();
                    }
                }
                break;

            default: break;
        }
    }
    #endregion

    internal protected static void UnfocusTextBox(TextBox Target)
    {
        FocusManager.SetFocusedElement(FocusManager.GetFocusScope(Target), null);
        Keyboard.ClearFocus();
        MainControl.Focus();
    }

    internal protected static void LockEditorUndo()
    {
        MainControl.Editor.IsUndoEnabled = false;
        MainControl.Editor.IsUndoEnabled = true;
    }
    #endregion



    #region Context Menu
    private void Actions_ContextMenu_Shared(object sender, RoutedEventArgs e)
    {
        string Editor_SelectedTextTemplate = Editor.SelectedText;
        
        switch ((sender as MenuItem).Name.Split("ContextMenuItem_")[^1])
        {
            case "InsertStyle":
                Editor_SelectedTextTemplate = $"<style=\"{(Mode_Handlers.Upstairs.ActiveProperties.Key.Equals("Skills") ? "highlight" : "upgradeHighlight")}\">{Editor_SelectedTextTemplate}</style>";
                break;

            case "TMProToKeywordLinks":
                Editor_SelectedTextTemplate = LimbusPreviewFormatter.RemoteRegexPatterns.TMProKeyword.Replace(Editor_SelectedTextTemplate, Match =>
                {
                    string ID = Match.Groups["ID"].Value;
                    if (KeywordsInterrogate.KeywordsGlossary.ContainsKey(ID))
                    {
                        return $"[{ID}]";
                    }
                    else return Match.Groups[0].Value;
                });

                break;

            case "TMProToShorthands":
                Editor_SelectedTextTemplate = LimbusPreviewFormatter.RemoteRegexPatterns.TMProKeyword.Replace(Editor_SelectedTextTemplate, Match =>
                {
                    string ID = Match.Groups["ID"].Value;
                    string Color = Match.Groups["Color"].Value;
                    string Name = Match.Groups["Name"].Value;
                    if (KeywordsInterrogate.KeywordsGlossary.ContainsKey(ID))
                    {
                        string CustomColorAttach = "";
                        if (!Color.Equals(KeywordsInterrogate.KeywordsGlossary[ID].StringColor))
                        {
                            CustomColorAttach = Configurazione.ShorthandsInsertionShape.InsertionShape_Color.Replace("<HexColor>", Color);
                        }

                        string OutputShorthand = Configurazione.ShorthandsInsertionShape.InsertionShape.Replace("<KeywordID>", ID).Replace("<KeywordName>", Name).Replace("<KeywordColor>", CustomColorAttach);

                        return OutputShorthand;
                    }
                    else return Match.Groups[0].Value;
                });
                break;

            case "UnevidentKeywordsToKeywordLinks":
                foreach(KeyValuePair<string, string> UnevidentKeyword in KeywordsInterrogate.Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter)
                {
                    if (Editor.Text.Contains(UnevidentKeyword.Key))
                    {
                        Editor_SelectedTextTemplate = Regex.Replace(Editor_SelectedTextTemplate, LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection.Replace("KeywordNameWillBeHere", UnevidentKeyword.Key.ToEscapeRegexString()), Match =>
                        {
                            return $"[{UnevidentKeyword.Value}]";
                        });
                    }
                }
                break;

            case "UnevidentKeywordsToShorthands":
                foreach (KeyValuePair<string, string> UnevidentKeyword in KeywordsInterrogate.Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter)
                {
                    if (Editor_SelectedTextTemplate.Contains(UnevidentKeyword.Key))
                    {
                        Editor_SelectedTextTemplate = Regex.Replace(Editor_SelectedTextTemplate, LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection.Replace("KeywordNameWillBeHere", UnevidentKeyword.Key.ToEscapeRegexString()), Match =>
                        {
                            return Configurazione.ShorthandsInsertionShape.InsertionShape.Replace("<KeywordID>", UnevidentKeyword.Value).Replace("<KeywordName>", UnevidentKeyword.Key.Replace(" ", "<\0TMPSPACE>")).Replace("<KeywordColor>", "");
                        });
                    }
                }
                break;

            case "KeywordLinksToShorthand":
                Editor_SelectedTextTemplate = LimbusPreviewFormatter.RemoteRegexPatterns.KeywordLink.Replace(Editor_SelectedTextTemplate, Match =>
                {
                    string ID = Match.Groups["ID"].Value;
                    if (KeywordsInterrogate.KeywordsGlossary.ContainsKey(ID))
                    {
                        return Configurazione.ShorthandsInsertionShape.InsertionShape.Replace("<KeywordID>", ID).Replace("<KeywordName>", KeywordsInterrogate.KeywordsGlossary[ID].Name).Replace("<KeywordColor>", "");
                    }
                    else
                    {
                        return Match.Groups[0].Value;
                    }
                });
                break;

            case "KeywordLinksToTMPro":
                Editor_SelectedTextTemplate = LimbusPreviewFormatter.RemoteRegexPatterns.KeywordLink.Replace(Editor_SelectedTextTemplate, Match =>
                {
                    string ID = Match.Groups["ID"].Value;
                    if (KeywordsInterrogate.KeywordsGlossary.ContainsKey(ID))
                    {
                        string Name = KeywordsInterrogate.KeywordsGlossary[ID].Name;
                        string Color = KeywordsInterrogate.KeywordsGlossary[ID].StringColor;

                        return $"<sprite name=\"{ID}\"><color={Color}><u><link=\"{ID}\">{Name}</link></u></color>";
                    }
                    else return Match.Groups[0].Value;
                });
                break;
        }
        // tmpspace to avoid conversion of same keywords within other, as example 'Attack Power Up' with 'Power Up' inside that being converted too without tmpspace
        if (!Editor_SelectedTextTemplate.Equals(Editor.SelectedText)) Editor.SelectedText = Editor_SelectedTextTemplate.Replace("<\0TMPSPACE>", " ");
    }

    #endregion

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (!Configurazione.LoadErrors.Equals("") & Configurazione.DeltaConfig.Internal.ShowLoadWarnings)
        {
            Configurazione.ShowLoadWarningsWindow();
        }

        if (DeltaConfig.TechnicalActions.KeywordsDictionary.Generate & !File.Exists("Keywords Multiple Meanings.json"))
        {
            if (Directory.Exists(DeltaConfig.TechnicalActions.KeywordsDictionary.Path))
            {
                KeywordsInterrogate.ExportKeywordsMultipleMeaningsDictionary(
                    DeltaConfig.TechnicalActions.KeywordsDictionary.Path
                );
            }
            else
            {
                MessageBox.Show($"Keywords multiple meanings dir \"{DeltaConfig.TechnicalActions.KeywordsDictionary.Path}\" not found");
            }
        }
    }

    private void ReloadConfig(object sender, MouseButtonEventArgs e) => ReloadConfig_Direct();
    async private void OpenSettings(object sender, MouseButtonEventArgs e)
    {
        if (!SettingsControl.IsActive)
        {
            await Task.Delay(50);
            SettingsControl.Show();
            SettingsControl.Focus();
        }
    }

    internal protected static void ReloadConfig_Direct()
    {
        Configurazione.PullLoad();
    }

    private void SavePreviewlayoutScan()
    {
        string NameHint = "";

        ScrollViewer CurrentTarget = null;

        switch (Upstairs.ActiveProperties.Key)
        {
            case "Skills":
                CurrentTarget = SurfaceScrollPreview_Skills;
                NameHint = $"{CurrentFile.Name.Replace(".json", "")}, " +
                           $"ID {Mode_Skills.CurrentSkillID}" +
                           (CurrentFile.Name.ToLower().Contains("personality") ? $", Uptie {Mode_Skills.CurrentSkillUptieLevel}" : "");
                break;

            case "Passives":
                CurrentTarget = SurfaceScrollPreview_Passives;
                NameHint = $"{CurrentFile.Name.Replace(".json", "")}, " +
                           $"ID {Mode_Passives.CurrentPassiveID}";
                break;

            case "Keywords":
                CurrentTarget = 
                    PreviewLayoutGrid_Keywords_Sub_Bufs.Visibility == Visibility.Visible ?
                        Scanable__PreviewLayout_Keywords_Bufs_Desc
                        :
                        SurfaceScrollPreview_Keywords__BattleKeywords;
                NameHint = $"{CurrentFile.Name.Replace(".json", "")}, " +
                           $"ID {Mode_Keywords.CurrentKeywordID}";
                break;

            case "E.G.O Gifts":
                CurrentTarget = SurfaceScrollPreview_EGOGifts;
                NameHint = $"{CurrentFile.Name.Replace(".json", "")}, " +
                           $"ID {Mode_EGOGifts.CurrentEGOGiftID}";
                break;

            default: break;
        }

        if (!Directory.Exists(@"⇲ Assets Directory\[⇲] Scans"))
        {
            Directory.CreateDirectory(@"⇲ Assets Directory\[⇲] Scans");
        }

        if (CurrentTarget != null)
        {
            ScanScrollviewer(CurrentTarget, NameHint);
        }
    }
}
public static class RemoveChildHelper
{
    public static void ReconnectAsChildTo(this UIElement TargetElement, dynamic NewParent)
    {
        DependencyObject TargetElement_Parent = (TargetElement as FrameworkElement).Parent;
        TargetElement_Parent.RemoveChild(TargetElement);

        NewParent.Children.Add(TargetElement);
    } 

    public static void RemoveChild(this DependencyObject parent, UIElement child)
    {
        var panel = parent as Panel;
        if (panel != null)
        {
            panel.Children.Remove(child);
            return;
        }

        var decorator = parent as Decorator;
        if (decorator != null)
        {
            if (decorator.Child == child)
            {
                decorator.Child = null;
            }
            return;
        }

        var contentPresenter = parent as ContentPresenter;
        if (contentPresenter != null)
        {
            if (contentPresenter.Content == child)
            {
                contentPresenter.Content = null;
            }
            return;
        }

        var contentControl = parent as ContentControl;
        if (contentControl != null)
        {
            if (contentControl.Content == child)
            {
                contentControl.Content = null;
            }
            return;
        }

        // maybe more
    }
}