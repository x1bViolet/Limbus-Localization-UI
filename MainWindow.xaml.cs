using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Microsoft.Win32;
using RichText;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Json.BaseTypes;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Passives;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.Custom_Skills_Constructor;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Json.FilesIntegration;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator.ProjectFile.Sections;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Globalization.NumberStyles;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow : Window
{
    #region Initials
    internal protected static MainWindow MainControl;
    internal protected static SettingsWindow SettingsControl = new SettingsWindow();
    internal protected static bool ManualTextLoadEvent = false;


    internal protected static Dictionary<string, RichTextBox> UILanguage;
    internal protected static Dictionary<string, TextBox> UITextfieldElements;
    internal protected static Dictionary<string, RichTextBox> PreviewLayoutControls;

    internal protected static List<dynamic?> SwitchButtons_TextFieldsControl = [];
    internal protected static List<RichTextBox?> PreviewLayoutsList = [];
    internal protected static List<TextBox> FocusableTextBoxes = [];
    
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
            ["Skills Name Replication 'Atk Weight'"] = STE_Skills_Replication_AtkWeightText,
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
            ["[Settings] Limbus Preview — Enable Skill Names Replication"] = SettingsControl.STE_Settings_LimbusPreview_EnableSkillNamesReplica,
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


            ["[Settings] Resourecs Reload — Secton Name"] = SettingsControl.STE_Settings_Resources_Reload_SectionName,
            ["[Settings] Resourecs Reload — Custom Language Keywords"] = SettingsControl.STE_Settings_Resources_Reload__CustomLangKeywords,
            ["[Settings] Resourecs Reload — Keyword Icons"] = SettingsControl.STE_Settings_Resources_Reload__KeywordImages,
            ["[Settings] Resourecs Reload — Skills Icons and Display Info"] = SettingsControl.STE_Settings_Resources_Reload__ReloadSkillsDisplayInfo,
            ["[Settings] Resourecs Reload — E.G.O Gifts Icons and Display Info"] = SettingsControl.STE_Settings_Resources_Reload__ResetEGOGiftsDisplayInfo,


            ["[Settings] Preview Scans — Section Name"] = SettingsControl.STE_Settings_PreviewScans_SectionName,
            ["[Settings] Preview Scans — Toggle Scan Area view"] = SettingsControl.STE_Settings_PreviewScans_ToggleAreaView,
            ["[Settings] Preview Scans — Scale Factor"] = SettingsControl.STE_Settings_PreviewScans_ScansScaleFactor,
            ["[Settings] Preview Scans — Background Color (Skills only)"] = SettingsControl.STE_Settings_PreviewScans_SkillsBackgroundColor,
            ["[Settings] Preview Scans — Toggle Background Color View"] = SettingsControl.STE_Settings_PreviewScans_ToggleSkillsBackgroundColorView,
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
            ["[Settings] Preview Scans — Width of Skills Preview"] = SettingsControl.InputSkillsPanelWidth,
            ["[Settings] Preview Scans — Scale Factor"] = SettingsControl.InputScansScaleFactor,
            ["[Settings] Preview Scans — Background Color (Skills only)"] = SettingsControl.InputSkillsScanBackgroundColor,
        };
    }
    #endregion


    public MainWindow()
    {
        InitializeComponent();
        MainControl = this;

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
        if (File.Exists(@"⇲ Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Raw Json\Raw Json $Unpack.zip"))
        {
            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(@"⇲ Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Raw Json\Raw Json $Unpack.zip", @"⇲ Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Raw Json");

                File.Delete(@"⇲ Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Raw Json\Raw Json $Unpack.zip");
            }
            catch { }
        }

        PreviewUpdate_TargetSite = PreviewLayout_Default;

        File.WriteAllText(@"⇲ Assets Directory\Latest loading.txt", "");

        Mode_Skills.LoadDefaultResources();

        Configurazione.PullLoad();

        if (File.Exists(@"⇲ Assets Directory\Default Text.txt"))
        {
            Editor.Text = File.ReadAllText(@"⇲ Assets Directory\Default Text.txt");
        }
        else
        {
            Editor.Text = "               <spritessize=+30><font=\"BebasKai SDF\"><size=140%><sprite name=\"9202\"> <u>Limbus Company Localization Interface</u> <color=#f8c200>'1.2:0</color></size></font></spritessize>\n\nЧерти вышли из омута";
        }

        RichText.InternalModel.InitializingEvent = false;

        {
            InitializeIdentityPreviewCreatorProperties();



            // Switch ui back to regular on startup (i dont want to change xaml back)
            IdentityPreviewCreator_TextEntries_ElementsColor.Text = "#abcdef";

            IdentityPreviewCreator_CautionTypeSelector.SelectedIndex = 0;
            IdentityPreviewCreator_CreateNewProject();

            SwitchUI_Activate();

            SwitchUI_Deactivate();

            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;
        }

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

        //if (Mode_Handlers.Upstairs.ActiveProperties.Key.Equals("Skills"))
        //{
        //    //////////////////////////////////////////////////////////////////////////////////////////////
        //    var FullLink = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel];
        //    //////////////////////////////////////////////////////////////////////////////////////////////
        //    if (FullLink.Coins != null)
        //    {
        //        if (FullLink.Coins.Count > 0)
        //        {
        //            Mode_Skills.CheckSkillNameReplicaCoins_FromLocalizationFile();
        //        }
        //    }
        //}

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
                        
                        if (CoinInfoFullLink.CoinDescriptions.Where(x => x.EditorDescription.EqualsOneOf("", "<style=\"highlight\"></style>")).Count() == CoinInfoFullLink.CoinDescriptions.Count)
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

        if (PreviewUpdate_TargetSite != null) PreviewUpdate_TargetSite.SetLimbusRichText(EditorText);
    }
    #endregion



    #region Surfacescroll
    internal protected static void InitSurfaceScroll(ScrollViewer Target)
    {
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
            System.Windows.Vector diff = SurfaceScroll_lastMousePosition - currentPosition;
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

            Mode_Skills.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
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
        if (!CustomIdentityPreviewCreator.IsActive)
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
    }

    internal protected static async void NavigationPanel_Skills_SwitchToCoinDesc_FastSwitch_CoinDescFocusHighlightEvent(RichTextBox TargetDesc)
    {
        TargetDesc.Background = ToSolidColorBrush("#01303030");

        for (int i = 255; i >= 0; i--)
        {
            string TransperacyChange = i.ToString("X");
            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

            TargetDesc.Background = ToSolidColorBrush($"#{TransperacyChange}{TargetDesc.Background.ToString()[3..]}");
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

            STE_DisableCover_Passives_SummaryDescription.Background = ToSolidColorBrush($"#{TransperacyChange}{STE_DisableCover_Passives_SummaryDescription.Background.ToString()[3..]}");

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

            STE_DisableCover_Keyword_SummaryDescription.Background = ToSolidColorBrush($"#{TransperacyChange}{STE_DisableCover_Keyword_SummaryDescription.Background.ToString()[3..]}");

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

        if (FullLinkKeyword.SummaryDescription != null)
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

        if (AnyChanges) Mode_Keywords.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
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

                            STE_NavigationPanel_ObjectID_Display.Foreground = ToSolidColorBrush($"#{TransperacyChange}{STE_NavigationPanel_ObjectID_Display.Foreground.ToString()[3..]}");

                            if (i % 3 == 0) await Task.Delay(1);
                        }

                        for (int i = 0; i <= 255; i++)
                        {
                            string TransperacyChange = i.ToString("X");
                            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

                            STE_NavigationPanel_ObjectID_Display_IDCopied.Foreground = ToSolidColorBrush($"#{TransperacyChange}{STE_NavigationPanel_ObjectID_Display_IDCopied.Foreground.ToString()[3..]}");

                            if (i % 3 == 0) await Task.Delay(1);
                        }

                        await Task.Delay(460);

                        for (int i = 255; i >= 0; i--)
                        {
                            string TransperacyChange = i.ToString("X");
                            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

                            STE_NavigationPanel_ObjectID_Display_IDCopied.Foreground = ToSolidColorBrush($"#{TransperacyChange}{STE_NavigationPanel_ObjectID_Display_IDCopied.Foreground.ToString()[3..]}");

                            if (i % 3 == 0) await Task.Delay(1);
                        }

                        for (int i = 0; i <= 255; i++)
                        {
                            string TransperacyChange = i.ToString("X");
                            if (TransperacyChange.Length == 1) TransperacyChange = $"0{TransperacyChange}";

                            STE_NavigationPanel_ObjectID_Display.Foreground = ToSolidColorBrush($"#{TransperacyChange}{STE_NavigationPanel_ObjectID_Display.Foreground.ToString()[3..]}");

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

                    Mode_Skills.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
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

                    Mode_Passives.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
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

                    Mode_Keywords.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
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

                    Mode_EGOGifts.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
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

    internal protected void Actions_FILE_SelectFile_Acutal()
    {
        OpenFileDialog JsonFileSelector = new OpenFileDialog();
        JsonFileSelector.DefaultExt = ".json";
        JsonFileSelector.Filter = "Limbus localization files |*.json";

        bool? Result = JsonFileSelector.ShowDialog();

        if (Result == true)
        {
            string Filename = JsonFileSelector.FileName;

            FileInfo TemplateTarget = new FileInfo(Filename);

            string CheckName = TemplateTarget.Name.RemovePrefix(["JP_", "KR_", "EN_"]);

            string? PredefinedFileType = TryAcquireManualFileType(TemplateTarget.FullName);
            if (PredefinedFileType != null)
            {
                CheckName = PredefinedFileType;
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
    }

    

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        //rin($"Autohide:{UIThemesLoader.LoadedTheme.AutoHideBackgroundOnMinWidth}; Width:{Width} (>{Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinWidth}), Height:{Height} (>{Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinHeight})");

        //if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;

        if ((Width <= Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinWidth + 2 & Height <= Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinHeight + 2)
            | (((UIThemesLoader.LoadedTheme != null ? UIThemesLoader.LoadedTheme.AutoHideBackgroundOnMinWidth : false) & Width <= Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinWidth + 2)))
        {
            BackgroundImage.Visibility = Visibility.Collapsed;
        }
        else
        {
            BackgroundImage.Visibility = Visibility.Visible;
        }
        NewWindowSizes.Rect = new Rect(0, 0, ActualWidth, ActualHeight);
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
                    if (CheckPassive.Value.SummaryDescription != null)
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
                        
                        if (UptieLevel.Coins != null)
                        {
                            foreach (var Coin in UptieLevel.Coins)
                            {
                                if (Coin != null)
                                {
                                    if (Coin.CoinDescriptions != null)
                                    {
                                        foreach (var CoinDesc in Coin.CoinDescriptions)
                                        {
                                            if (CoinDesc != null)
                                            {
                                                if (CoinDesc.Description != null)
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
                    if (CheckKeyword.Value.SummaryDescription != null)
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
                        if (CheckEGOGift.Value.SimpleDescriptions != null)
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

    private bool CanDragMove = true;
    private void Window_DragMove(object sender, MouseButtonEventArgs e)
    {
        if (CanDragMove)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                this.Left = 0;
                this.Top = 0;
            }
            this.DragMove();
            if (WindowState == WindowState.Maximized)
            {
                if (CustomIdentityPreviewCreator.IsActive)
                {
                    var workArea = SystemParameters.WorkArea;
                    this.Left = workArea.Left;
                    this.Top = workArea.Top;
                    this.Width = workArea.Width;
                    this.Height = workArea.Height;
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    MainWindowContentControl.Margin = new Thickness(6, 6, 0, 6);
                }
            }
            else
            {
                MainWindowContentControl.Margin = new Thickness(0);
            }
        }
    }
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

                if (ElementName.Equals("Special_PreviewLayout_Keywords_BattleKeywords_Desc"))
                {
                    (current as RichTextBox).SetValue(Paragraph.LineHeightProperty, 25.0);
                }
                else
                {
                    (current as RichTextBox).SetValue(Paragraph.LineHeightProperty, 20.0);
                }
            }
            else if (!ElementName.StartsWith("SeriousScrollViewer") & current is ScrollViewer)
            {
                try
                {
                    (current as ScrollViewer).Resources.Add(SystemParameters.VerticalScrollBarWidthKey, 0.0);
                }
                catch { } // DISABLE SCROLLBAR
            }
            else if (current is TextBox)
            {
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

        if (Sender.Name.Equals("SWBT_Skills_MainSkillName"))
        {
            SkillNameReplica.Text = SWBT_Skills_MainSkillName.Text;
        }

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

            //rin($"\n\n\nInput: {ClipboardText}");

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

        if (Keyboard.IsKeyDown(Key.LeftCtrl) & Keyboard.IsKeyDown(Key.F) & CustomIdentityPreviewCreator.IsActive)
        {
            if (FirstColumnItemsSelector.Visibility == Visible)
            {
                FirstColumnItemsSelector.Visibility = Collapsed;
                SecondColumnItemsSelector.Visibility = Collapsed;
            }
            else
            {
                FirstColumnItemsSelector.Visibility = Visible;
                SecondColumnItemsSelector.Visibility = Visible;
            }
        }

        #region Files saving
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
            if (e.Key == Key.S && !IsCtrlSPressed)
            {
                IsCtrlSPressed = true;

                if (PreviewUpdate_TargetSite != null)
                {
                    switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
                    {
                        case "Skills":

                            if (PreviewUpdate_TargetSite.Equals(PreviewLayout_Skills_MainDesc))
                            {
                                DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Description = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].EditorDescription;

                                UILanguage["Right Menu — Skill Desc"].SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Skill Desc"]);

                                Mode_Skills.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
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

                                Mode_Skills.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
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

                                Mode_Passives.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }
                            else if (Mode_Passives.TargetSite_StringLine.Equals("Summary Description"))
                            {
                                FullLinkPassive.SummaryDescription = FullLinkPassive.EditorSummaryDescription;

                                STE_Passives_SummaryDescription
                                    .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Passive Summary"]);

                                Mode_Passives.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
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

                                Mode_Keywords.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }
                            else if (Mode_Keywords.TargetSite_StringLine.Equals("Summary Description"))
                            {
                                FullLinkKeyword.SummaryDescription = FullLinkKeyword.EditorSummaryDescription;

                                STE_Keyword_SummaryDescription
                                    .SetRichText(UILanguageLoader.UILanguageElementsTextData["Right Menu — Keyword Summary"]);

                                Mode_Keywords.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
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

                                Mode_EGOGifts.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }
                            else
                            {
                                string SimpleDescNumber = $"{Mode_EGOGifts.TargetSite_StringLine[^1]}";

                                int TargetSimpleDescIndex = int.Parse(SimpleDescNumber) - 1;

                                FullLinkEGOGift.SimpleDescriptions[TargetSimpleDescIndex].Description = FullLinkEGOGift.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription;

                                (MainControl.FindName($"STE_EGOGift_SimpleDescription{SimpleDescNumber}") as RichTextBox)
                                    .SetRichText(UILanguageLoader.UILanguageElementsTextData[$"Right Menu — E.G.O Gift Simple Desc {SimpleDescNumber}"]);


                                Mode_EGOGifts.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }

                            break;
                    }
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
                            if (DelegateSkills_IDList.Contains(RegularObjectTargetID)) Mode_Handlers.Mode_Skills.TransformToSkill(RegularObjectTargetID);
                            break;

                        case "Passives":
                            if (DelegatePassives_IDList.Contains(RegularObjectTargetID)) Mode_Handlers.Mode_Passives.TransformToPassive(RegularObjectTargetID);
                            break;
                        
                        case "E.G.O Gifts":
                            if (DelegateEGOGifts_IDList.Contains(RegularObjectTargetID)) Mode_Handlers.Mode_EGOGifts.TransformToEGOGift(RegularObjectTargetID);
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



    #region Editor context Menu
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

    internal protected static void ReloadConfig_Direct()
    {
        Configurazione.PullLoad();
    }
    
    async private void OpenSettings(object sender, MouseButtonEventArgs e)
    {
        if (!SettingsControl.IsActive)
        {
            await Task.Delay(50);
            SettingsControl.Show();
            SettingsControl.Focus();
        }
    }

    private void SavePreviewlayoutScan()
    {
        string NameHint = "";
        string ManualPath = "";

        ScrollViewer CurrentTarget = null;

        if (CustomIdentityPreviewCreator.IsActive)
        {
            CurrentTarget = SeriousScrollViewer_1;

            SaveFileDialog OutputPathSelector = NewSaveFileDialog("Image files", ["png"]);
            OutputPathSelector.FileName = $"{DateTime.Now.ToString("HHːmmːss (dd.MM.yyyy)")}.png";
            if (OutputPathSelector.ShowDialog() == true)
            {
                ManualPath = OutputPathSelector.FileName;
            }
        }
        else
        {
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
                    if (CurrentFile != null)
                    {
                        CurrentTarget = SurfaceScrollPreview_EGOGifts;
                        NameHint = $"{CurrentFile.Name.Replace(".json", "")}, " +
                                   $"ID {Mode_EGOGifts.CurrentEGOGiftID}";
                    }
                    break;

                default: break;
            }
            if (!Directory.Exists(@"⇲ Assets Directory\[⇲] Scans"))
            {
                Directory.CreateDirectory(@"⇲ Assets Directory\[⇲] Scans");
            }
        }


        if (CurrentTarget != null)
        {
            bool ManuallyHiddenSelectors = false;
            if (FirstColumnItemsSelector.Visibility == Visible)
            {
                FirstColumnItemsSelector.Visibility = Collapsed;
                SecondColumnItemsSelector.Visibility = Collapsed;
            }
            else ManuallyHiddenSelectors = true;

            ScanScrollviewer(CurrentTarget, NameHint, ManualPath);

            if (!ManuallyHiddenSelectors)
            {
                FirstColumnItemsSelector.Visibility = Visible;
                SecondColumnItemsSelector.Visibility = Visible;
            }
        }
    }




































































    /// <summary>
    /// THERE ARE LINKS CODELENS STOP LYING
    /// </summary>
    private void IdentityPreviewCreator_ToggleSectionVisibilityMaster(object sender, MouseButtonEventArgs e)
    {
        Grid Target = ((sender as UILocalization_Grocerius).Parent as StackPanel).Children[1] as Grid;

        Target.Visibility = Target.Visibility == Visible ? Collapsed : Visible;
    }




    private static Random random = new Random();

    public static string RandomString(int length)
    {
        return new string(Enumerable.Repeat("ЙУABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private protected List<string> DefinedUIDsForColumnItems = new List<string>();
    private ItemRepresenter CreatePlaceholder(string NewItemType, string ManualUID = "", AddedTextItems_Single ManualInfoInsert = null)
    {
        string UID = ManualUID;
        if (ManualUID.Equals("")) // If not set (Item creation in ui) -> generate
        {
            for (int UIDGeneratorRound = 1; UIDGeneratorRound <= 100; UIDGeneratorRound++)
            {
                UID = RandomString(6);
                if (!DefinedUIDsForColumnItems.Contains(UID))
                {
                    DefinedUIDsForColumnItems.Add(UID);
                    break;
                } // idk they can be same in 0.00000001% of cases just for safety
            }
        }
        // else being set by loading project

        AddedTextItems_Single InfoToInsert = ManualInfoInsert != null ? ManualInfoInsert : new CustomIdentityPreviewCreator.ProjectFile.Sections.AddedTextItems_Single() { Type = NewItemType };

        ItemRepresenter ColumnItemAdd = new ItemRepresenter()
        {
            // Linked with project file data by ReEnumerateColumnItemsInProject() executing after each interactions with column items
            ItemInfo = InfoToInsert,
            HorizontalAlignment = HorizontalAlignment.Left,
            MinWidth = 300,
            Background = Brushes.Transparent,
            Uid = UID,
            Margin = new Thickness(0, 0, 0, 10),
            Children =
            {
                new TextBlock()
                {
                    Foreground = Brushes.White,
                    Opacity = 0.4,
                    FontSize = 25,
                    TextAlignment = TextAlignment.Center,
                    Effect = new DropShadowEffect() { BlurRadius = 0, ShadowDepth = 3 },
                    FontFamily = MainControl.FindResource("BebasKaiUniversal") as FontFamily,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = $"TEXT CONTROL\n({NewItemType}, [Item #{UID}]) "
                }
            },
        };

        ColumnItemAdd.ContextMenu = new ContextMenu()
        {
            Items =
            {
                new MenuItem() // 0
                {
                    Header = new UILocalization_Grocerius()
                    {
                        Text = $"Item #{UID}",
                        FontSize = 14,
                        Width = 145,
                        Margin = new Thickness(-7.2, 2, 0, 2),
                        FontFamily = new FontFamily("GOST Type BU"),
                        IsHitTestVisible = false
                        
                    }
                },
                new Separator() { Margin = new Thickness(-30, 2, 0, 2) }, // 1
                new MenuItem() // 2
                {
                    Header = new UILocalization_Grocerius()
                    {
                        SpecProperty_ContextMenuParent = ColumnItemAdd, // ContextMenu parent link because i somehow cant access it through MenuItem.parent.parent.parent
                        FontSize = 14,
                        Width = 145,
                        Margin = new Thickness(-7.2, 2, 0, 2),
                        Text = "▲ Move up",
                        FontFamily = new FontFamily("GOST Type BU")
                    }
                },
                new MenuItem() // 3
                {
                    Header = new UILocalization_Grocerius()
                    {
                        SpecProperty_ContextMenuParent = ColumnItemAdd,
                        FontSize = 14,
                        Width = 145,
                        Margin = new Thickness(-7.2, 2, 0, 2),
                        Text = "↪ Refresh text size",
                        FontFamily = new FontFamily("GOST Type BU")
                    }
                },
                new MenuItem() // 4
                {
                    Header = new UILocalization_Grocerius()
                    {
                        SpecProperty_ContextMenuParent = ColumnItemAdd,
                        FontSize = 14,
                        Width = 145,
                        Margin = new Thickness(-7.2, 2, 0, 2),
                        Text = "▼ Move down",
                        FontFamily = new FontFamily("GOST Type BU")
                    }
                },
                new Separator() { Margin = new Thickness(-30, 2, 0, 2) }, // 5
                new MenuItem() // 6
                {
                    Header = new UILocalization_Grocerius()
                    {
                        SpecProperty_ContextMenuParent = ColumnItemAdd,
                        FontSize = 14,
                        Width = 145,
                        Margin = new Thickness(-7.2, 2, 0, 2),
                        Text = "✕ Remove",
                        FontFamily = new FontFamily("GOST Type BU")
                    }
                }
            }
        };

        ColumnItemAdd.PreviewMouseLeftButtonDown += SetFocusOnColumnElement_Link;

        (ColumnItemAdd.ContextMenu.Items[2] as MenuItem).Click += MoveColumnItemUp;
        (ColumnItemAdd.ContextMenu.Items[3] as MenuItem).Click += RefreshTextSize;
        (ColumnItemAdd.ContextMenu.Items[4] as MenuItem).Click += MoveColumnItemDown;
        (ColumnItemAdd.ContextMenu.Items[6] as MenuItem).Click += RemoveColumnItem;

        return ColumnItemAdd;
    }

    private void SetFocusOnColumnElement_Link(object sender, MouseButtonEventArgs e)
    {
        SeriousScrollViewer_0.ScrollToBottom();
        SetFocusOnColumnItem(sender as ItemRepresenter);
    }

    private void MoveColumnItemUp(object sender, RoutedEventArgs e)
    {
        MoveColumnItemMaster(sender, "Up");
    }

    private void MoveColumnItemDown(object sender, RoutedEventArgs e)
    {
        MoveColumnItemMaster(sender, "Down");
    }

    private void RefreshTextSize(object sender, RoutedEventArgs e)
    {
        SetFocusOnColumnItem(((sender as MenuItem).Header as UILocalization_Grocerius).SpecProperty_ContextMenuParent as ItemRepresenter);

        CheckKeywordSelectorAndGenerateKeywordDisplayer();
        CheckPassiveSelectorAndGeneratePassiveDisplayer();
        CheckSkillSelectorsAndGenerateSkillDisplayer();
    }

    private protected void MoveColumnItemMaster(object ContextMenuItemSender, string Direction)
    {
        ItemRepresenter Target = ((ContextMenuItemSender as MenuItem).Header as UILocalization_Grocerius).SpecProperty_ContextMenuParent as ItemRepresenter;

        StackPanel TargetColumn = Target.Parent as StackPanel;

        if (Direction.Equals("Up")) TargetColumn.MoveItemUp(Target);
        else TargetColumn.MoveItemDown(Target);

        ReEnumerateColumnItemsInProject();
    }

    private async void RemoveColumnItem(object sender, RoutedEventArgs e)
    {
        ItemRepresenter Target = ((sender as MenuItem).Header as UILocalization_Grocerius).SpecProperty_ContextMenuParent as ItemRepresenter;

        StackPanel TargetColumn = Target.Parent as StackPanel;

        await Task.Delay(150); // some white thing appears for 0.1 second if without delay

        DefinedUIDsForColumnItems.Remove(Target.Uid);

        TargetColumn.Children.Remove(Target);

        ReEnumerateColumnItemsInProject();
    }


    // Check column items and update all text info in project file data
    private void ReEnumerateColumnItemsInProject()
    {
        var FirstColumnProjectData = CustomIdentityPreviewCreator.ProjectFile.LoadedProject.Text.FirstColumnItems;
        var SecondColumnProjectData = CustomIdentityPreviewCreator.ProjectFile.LoadedProject.Text.SecondColumnItems;

        FirstColumnProjectData.Clear();
        SecondColumnProjectData.Clear();

        foreach (ItemRepresenter TextItemGrid in IdentityPreviewItems_FirstColumn.Children)
        {
            FirstColumnProjectData[TextItemGrid.Uid] = TextItemGrid.ItemInfo;
        }

        foreach (ItemRepresenter TextItemGrid in IdentityPreviewItems_SecondColumn.Children)
        {
            SecondColumnProjectData[TextItemGrid.Uid] = TextItemGrid.ItemInfo;
        }
    }



    private protected StackPanel GetTargetColumn(object Sender)
    {
        return ((((Sender as ComboBoxItem).Parent as ComboBox).Parent as Grid).Parent as StackPanel).Children[0] as StackPanel;
    }

    private void IdentityPreviewCreator_AddSkillToColumn(object sender, MouseButtonEventArgs e)
    {
        IdentityPreviewCreator_AddItemToColumnFromUIMaster(GetTargetColumn(sender), "Skill");
    }
    private void IdentityPreviewCreator_AddPassiveToColumn(object sender, MouseButtonEventArgs e)
    {
        IdentityPreviewCreator_AddItemToColumnFromUIMaster(GetTargetColumn(sender), "Passive");
    }
    private void IdentityPreviewCreator_AddKeywordToColumn(object sender, MouseButtonEventArgs e)
    {
        IdentityPreviewCreator_AddItemToColumnFromUIMaster(GetTargetColumn(sender), "Keyword");
    }








    private protected void IdentityPreviewCreator_AddItemToColumnFromUIMaster(StackPanel TargetColumn, string Type)
    {
        ItemRepresenter CreatedColumnItem = CreatePlaceholder(Type);
        CreatedColumnItem.ColumnNumber = TargetColumn.Uid;

        TargetColumn.Children.Add(CreatedColumnItem);
        ReEnumerateColumnItemsInProject();

        SetFocusOnColumnItem(CreatedColumnItem);
    }

    internal static ItemRepresenter FocusedColumnItem = null;
    private protected void SetFocusOnColumnItem(ItemRepresenter Target, bool UpdateSelectorsAndSliders = true)
    {
        MakeAvailable(ItemEditor_ParentGrid); // Unlock from startup state

        ColumnItemFocusingEvent = true;

        #region Static value change (UI elements lock/unlock)
        FocusedColumnItem = Target;

        SkillsTextIDSelector.Visibility = Collapsed;
        PassivesTextIDSelector.Visibility = Collapsed;
        KeywordTextIDSelector.Visibility = Collapsed;

        MakeUnavailable
        (
            SkillMainAndCoinDescriptionsWidthController__ParentGrid,
            PassiveDescriptionWidthController__ParentGrid,
            KeywordIconFileSelectButton_ParentGrid,
            ItemSignatureInput_ParentGrid,

            SkillsDisplayInfoIDSelector_ParentGrid
        );

        SelectedItemSignature.Text = FocusedColumnItem.ItemInfo.TextItemSignature;

        if (UpdateSelectorsAndSliders) KeywordIconSelectionLabel.Text = "Keyword icon image";

        if (UpdateSelectorsAndSliders)
        {
            SkillsLocalizationIDSelector.SelectedIndex = -1;
            PassivesLocalizationIDSelector.SelectedIndex = -1;
            KeywordsLocalizationIDSelector.SelectedIndex = -1;
            SkillsDisplayInfoIDSelector.SelectedIndex = -1;
        }


        if (FocusedColumnItem.ItemInfo.Type.Equals("Skill"))
        {
            if (FocusedColumnItem.Children.Count == 2) MakeAvailable(ItemSignatureInput_ParentGrid);

            MakeAvailable(SkillMainAndCoinDescriptionsWidthController__ParentGrid, SkillsDisplayInfoIDSelector_ParentGrid);

            SkillsTextIDSelector.Visibility = Visible;
            if (UpdateSelectorsAndSliders)
            {
                SkillsLocalizationIDSelector.SelectedIndex = Target.SelectedLocalizationItemIndex;
                SkillsDisplayInfoIDSelector.SelectedIndex = Target.SelectedSkillDisplayInfoConstructorIndex;
            }
        }
        else
        {
            if (FocusedColumnItem.ItemInfo.Type.Equals("Passive"))
            {
                if (FocusedColumnItem.Children.Count == 2) MakeAvailable(ItemSignatureInput_ParentGrid);

                MakeAvailable(PassiveDescriptionWidthController__ParentGrid);

                PassivesTextIDSelector.Visibility = Visible;
                if (UpdateSelectorsAndSliders)
                {
                    PassivesLocalizationIDSelector.SelectedIndex = Target.SelectedLocalizationItemIndex;
                }
            }
            else if (FocusedColumnItem.ItemInfo.Type.Equals("Keyword"))
            {
                if (FocusedColumnItem.KeywordIcon != null)
                {
                    MakeAvailable(KeywordIconFileSelectButton_ParentGrid);

                    if (File.Exists(FocusedColumnItem.ItemInfo.KeywordIconImage))
                    {
                        KeywordIconSelectionLabel.Text = $"Keyword icon image\n<size=78%><color=#fc5a03>{FocusedColumnItem.ItemInfo.KeywordIconImage.GetName()}</color></size>";
                    }
                }

                KeywordTextIDSelector.Visibility = Visible;
                if (UpdateSelectorsAndSliders)
                {
                    KeywordsLocalizationIDSelector.SelectedIndex = Target.SelectedLocalizationItemIndex;
                }
            }
        }
        #endregion

        if (UpdateSelectorsAndSliders) ChangeSliderValuesOnFocus();

        IdentityPreviewCreator_EditingItemHeader.Text = $"> Editing item: <b>#{Target.Uid}</b> ({Target.ItemInfo.Type})";

        ColumnItemFocusingEvent = false;
    }
    private protected void ChangeSliderValuesOnFocus()
    {
        ColumnItemVerticalOffsetControllder.Value = FocusedColumnItem.ItemInfo.VerticalOffset;
        ColumnItemHorizontalOffsetControllder.Value = FocusedColumnItem.ItemInfo.HorizontalOffset;

        NameMaxWidthController.Value = FocusedColumnItem.ItemInfo.NameMaxWidth;

        KeywordOrPassiveDescriptionWidthController.Value = FocusedColumnItem.ItemInfo.PassiveDescriptionWidth;

        SkillMainDescriptionWidthController.Value = FocusedColumnItem.ItemInfo.SkillMainDescriptionWidth;
        SkillCoinDescriptionsWidthController.Value = FocusedColumnItem.ItemInfo.SkillCoinsDescriptionWidth;
    }









    // LOAD EVERYTHING
    private void IdentityPreviewCreator_LoadProjectFile(object sender, MouseButtonEventArgs e)
    {
        OpenFileDialog ProjectSelectorDialog = new OpenFileDialog();
        ProjectSelectorDialog.DefaultExt = ".json";
        ProjectSelectorDialog.Filter = "Saved project file |*.json";

        bool? Result = ProjectSelectorDialog.ShowDialog();

        if (Result == true)
        {
            FileInfo Target = LoadedProjectFile = new FileInfo(ProjectSelectorDialog.FileName);
            CustomIdentityPreviewCreator.ProjectFile.CustomIdentityPreviewProject LoadedProject = Target.Deserealize<CustomIdentityPreviewCreator.ProjectFile.CustomIdentityPreviewProject>(Context: Target.Directory.FullName.Replace("\\", "/"));

            if (LoadedProject.ActualProject != null)
            {
                IdentityPreviewCreator_CreateNewProject();

                CustomIdentityPreviewCreator.ProjectFile.LoadedProject = LoadedProject;

                #region Image parameters
                {
                    var @ImageParameters = LoadedProject.ImageParameters;

                    IdentityPreviewCreator_WidthController_FirstStep.Value = ImageParameters.WidthAdjustment_FirstStep;
                    IdentityPreviewCreator_WidthController_SecondStep.Value = ImageParameters.WidthAdjustment_SecondStep;

                    IdentityPreviewHeight_IsAuto.IsChecked = ImageParameters.HeightAdjustment_IsAuto;
                    if (!ImageParameters.HeightAdjustment_IsAuto)
                    {
                        IdentityPreviewCreator_HeightController.Value = ImageParameters.HeightAdjustment;
                    }
                
                    if (File.Exists(ImageParameters.PortraitImage))
                    {
                        SelectIdentityOrEGOPortrait_Action(ImageParameters.PortraitImage);
                    }
                    IdentityPreviewCreator_AllocatedWidthForPortraitController.Value = ImageParameters.AllocatedWidthForPortrait;

                    if (ImageParameters.Type.Equals("E.G.O"))
                    {
                        PortraitTypeSelector.SelectedIndex = 1;
                    }
                    else
                    {
                        PortraitTypeSelector.SelectedIndex = 0;
                    }

                    ImageTypeText_TextEntry.Text = ImageParameters.ImageTypeSign;
                    if (File.Exists(ImageParameters.ImageTypeSign_AnotherFont)) SelectImageTypeSignFont_Action(ImageParameters.ImageTypeSign_AnotherFont);

                    IdentityPreviewCreator_IdentityPortraitScaleController.Value = ImageParameters.IdentityPortraitScale;
                    IdentityPreviewCreator_IdentityPortraitHorizontalOffsetController.Value = ImageParameters.PortraitHorizontalOffset;
                    IdentityPreviewCreator_IdentityPortraitVerticalOffsetController.Value = ImageParameters.PortraitVerticalOffset;

                    IdentityPreviewCreator_IdentityHeader_ParentGridMarginController.Value = ImageParameters.HeaderOffset;
                    IdentityPreviewCreator_IdentityHeader_IdentityOrEGONameOffsetController.Value = ImageParameters.IdentityOrEGONameOffset;
                    IdentityPreviewCreator_IdentityHeader_SinnerNameOffsetController.Value = ImageParameters.SinnerNameOffset;
                    IdentityPreviewCreator_IdentityHeader_IdentityRarityHorizontalOffsetController.Value = ImageParameters.RarityOrEGORiskLevelHorizontalOffset;
                    IdentityPreviewCreator_IdentityHeader_IdentityRarityVerticalOffsetController.Value = ImageParameters.RarityOrEGORiskLevelVerticalOffset;

                    IdentityPreviewCreator_TextBackgroundFadeoutSoftnessController.Value = ImageParameters.TextBackgroundFadeoutSoftness;
                    IdentityPreviewCreator_VignetteSoftnessController.Value = ImageParameters.VignetteStrength;
                    IdentityPreviewCreator_TopVignetteOffsetController.Value = ImageParameters.TopVignetteOffset;
                    IdentityPreviewCreator_LeftVignetteOffsetController.Value = ImageParameters.LeftVignetteOffset;
                    IdentityPreviewCreator_BottomVignetteOffsetController.Value = ImageParameters.BottomVignetteOffset;
                }
                #endregion

                #region Specific of the sinner and Identity/E.G.O
                {
                    var q = LoadedProject.Specific;

                    string StoredSinnerName = q.SinnerName; // Changing icon also changes default sinner name
                    string StoredAmbienceColor = q.AmbienceColor; // Same

                    if (q.SinnerIcon != null)
                    {
                        if (File.Exists(q.SinnerIcon))
                        {
                            // Custom
                            SelectCustomSinnerIcon_Action(q.SinnerIcon);
                        }
                        else
                        {
                            IdentityPreviewCreator_SinnerIconSelector.SelectedIndex = q.SinnerIcon switch
                            {
                                "Yi Sang"     => 0,
                                "Faust"       => 1,
                                "Don Quixote" => 2,
                                "Ryōshū"      => 3,
                                "Meursault"   => 4,
                                "Hong Lu"     => 5,
                                "Heathcliff"  => 6,
                                "Ishmael"     => 7,
                                "Rodion"      => 8,
                                "Sinclair"    => 9,
                                "Outis"       => 10,
                                "Gregor"      => 11,
                                _             => 0
                            };
                        }
                    }

                    IdentityPreviewCreator_SinnerIconBrightnessController.Value = q.IconBrightness;
                    IdentityPreviewCreator_SinnerIconSizeController.Value = q.IconSize;

                    IdentityPreviewCreator_TextEntries_ElementsColor.Text = StoredAmbienceColor;
                    if (StoredSinnerName != null) IdentityPreviewCreator_TextEntries_SinnerName.Text = StoredSinnerName;
                    if (q.IdentityOrEGOName != null) IdentityPreviewCreator_TextEntries_IdentityOrEGOName.Text = q.IdentityOrEGOName;

                    CanOverwriteProjectCautionsType = false;
                    IdentityPreviewCreator_TextEntries_ElementsColor.Text = q.AmbienceColor;
                    CanOverwriteProjectCautionsType = true;

                    if (q.RarityOrEGORiskLevel != null)
                    {
                        IdentityPreviewCreator_IdentityHeader_RarityOrEGORiskLevelSelector.SelectedIndex = q.RarityOrEGORiskLevel switch
                        {
                            "000" => 0,
                            "00"  => 1,
                            "0"   => 2,

                            "ZAYIN" => 3,
                            "HE"    => 4,
                            "TETH"  => 5,
                            "WAW"   => 6,
                            "ALEPH" => 7,
                        };
                    }
                }
                #endregion

                #region Decorative cautions
                {
                    var q = LoadedProject.DecorativeCautions;

                    IdentityPreviewCreator_Cautions_BoomRadiusController.Value = q.CautionBloomRadius;
                    IdentityPreviewCreator_Cautions_OpacityController.Value = q.CautionOpacity;

                    IdentityPreviewCreator_TextEntries_CustomCautionString.Text = q.CustomText.CustomCautionString;
                    if (File.Exists(q.CustomText.AnotherFont)) SelectDecorativeCautionsCustomFont_Action(q.CustomText.AnotherFont);
                    //IdentityPreviewCreator_CautionSettings_CustomCautionsParamBinder.Margin = new Thickness(0, q.CustomText.TextVerticalOffset, 0, 0);
                    //IdentityPreviewCreator_CautionSettings_CustomCautionsParamBinder.FontSize = q.CustomText.TextSize;

                    CautionsCustomTextVerticalOffsetController.Value = q.CustomText.TextVerticalOffset * 100;
                    CautionsCustomTextSizeController.Value = q.CustomText.TextSize * 100;


                    IdentityPreviewCreator_CautionTypeSelector.SelectedIndex = q.CautionType switch
                    {
                        "SEASON" => 0,
                        "CAUTION" => 1,
                        "None" => 2,
                        "Custom text" => 3
                    };
                }
                #endregion

                #region Text info
                {
                    var q = LoadedProject.Text;

                    UnifiedTextSizeController.Value = q.UnifiedTextSize;

                    IdentityPreviewCreator_TextInfo_FirstColumnOffsetController.Value = q.FirstColumnOffset;
                    IdentityPreviewCreator_TextInfo_SecondColumnOffsetController.Value = q.SecondColumnOffset;

                    FirstColumnItemSignaturesOffsetController.Value = q.FirstColumnItemSignaturesOffset;
                    SecondColumnItemSignaturesOffsetController.Value = q.SecondColumnItemSignaturesOffset;
                    if (File.Exists(q.ItemSignaturesAnotherFont)) SelectAnotherItemSignsFont_Action(q.ItemSignaturesAnotherFont);
                    
                    KeywordBoxesWidthController.Value = q.KeywordBoxesWidth;

                    // Update ID selectors
                    if (File.Exists(q.SkillsLocalizationFile)) UpdateSelector__SkillLocalizationIDSelector();
                    if (File.Exists(q.SkillsDisplayInfoConstructorFile)) UpdateSelector__SkillsDisplayInfoIDSelector();
                    if (File.Exists(q.PassivesLocalizationFile)) UpdateSelector__PassivesLocalizationIDSelector();
                    if (File.Exists(q.KeywordsLocalizationFile)) UpdateSelector__KeywordsLocalizationIDSelector();


                    ReconstructColumnItems();
                }
                #endregion
            }
        }
    }


    private void IdentityPreviewCreator_CreateNewProject()
    {
        CustomIdentityPreviewCreator.ProjectFile.LoadedProject = new CustomIdentityPreviewCreator.ProjectFile.CustomIdentityPreviewProject();
        CustomIdentityPreviewCreator.ProjectFile.LoadedProject.ActualProject = true;

        IdentityPreviewItems_FirstColumn.Children.Clear();
        IdentityPreviewItems_SecondColumn.Children.Clear();

        MakeAvailable
        (
            IdentityPreviewCreator_SectionHosts_ImageParameters,
            IdentityPreviewCreator_SectionHosts_SinnerAndIdentityOrEGOSpecific,
            IdentityPreviewCreator_SectionHosts_DecorativeCaution,
            IdentityPreviewCreator_SectionHosts_TextInfo,
            IdentityPreviewCreator_SaveProjectToFileButton,
            IdentityTextColumns
        );
    }

    private void IdentityPreviewCreator_SaveProjectToFile(object sender, MouseButtonEventArgs e)
    {
        SaveFileDialog SaveLocation = NewSaveFileDialog("Json files", ["json"], "Project.json");

        if (SaveLocation.ShowDialog() == true)
        {
            CustomIdentityPreviewCreator.ProjectFile.LoadedProject.SerializeFormatted(SaveLocation.FileName, Context: new FileInfo(SaveLocation.FileName).Directory.FullName.Replace("\\", "/"));
        }
    }

    





    private protected Dictionary<int, int> ID_And_Index__Links_Skills = new Dictionary<int, int>();
    private protected void UpdateSelector__SkillLocalizationIDSelector()
    {
        string TargetFile = CustomIdentityPreviewCreator.ProjectFile.LoadedProject.Text.SkillsLocalizationFile;

        Skills LoadedSkillsLocalizationData = new FileInfo(TargetFile).Deserealize<Skills>();

        if (LoadedSkillsLocalizationData.dataList != null && LoadedSkillsLocalizationData.dataList.Count > 0)
        {
            ID_And_Index__Links_Skills.Clear();
            SkillsLocalizationIDSelector.Items.Clear();

            int ItemEnumerator = 1;
            foreach (Skill SkillItem in LoadedSkillsLocalizationData.dataList)
            {
                if (SkillItem.ID != null && SkillItem.UptieLevels != null && SkillItem.UptieLevels.Count > 0)
                {
                    UptieLevel TargetUptieWithDesc = SkillItem.UptieLevels[^1]; // Last uptie

                    string SkillName = TargetUptieWithDesc.Name.nullHandle(NullText: "<i>No name</i>");

                    
                    if (TargetUptieWithDesc.OptionalAffinity != null)
                    {
                        string AffinityColor = CustomIdentityPreviewCreator.GetAffinityColor(TargetUptieWithDesc.OptionalAffinity).ToString().Replace("#FF", "#");
                        SkillName = $"<color={AffinityColor}>{SkillName}</color>";
                    }

                    string ItemName = $"{ItemEnumerator} — {SkillItem.ID} {SkillName}";

                    if (!TargetUptieWithDesc.Description.IsNullOrEmpty())
                    {
                        AddSpecItemToSelector(SkillsLocalizationIDSelector, ItemName, TargetUptieWithDesc, SkillItem.ID);

                        ID_And_Index__Links_Skills[(int)SkillItem.ID] = ItemEnumerator - 1;
                    }
                }

                ItemEnumerator++;
            }
        }

        if (SkillsLocalizationIDSelector.Items.Count > 0)
        {
            SelectSkillsLocalizationFile_Label.Text = $"Skills localization\n<size=78%><color=#fc5a03>{TargetFile.GetName()}</color></size>";
        }
    }

    private protected Dictionary<int, int> ID_And_Index__Links_Passives = new Dictionary<int, int>();
    private protected void UpdateSelector__PassivesLocalizationIDSelector()
    {
        string TargetFile = CustomIdentityPreviewCreator.ProjectFile.LoadedProject.Text.PassivesLocalizationFile;

        Passives LoadedPassivesLocalizationData = new FileInfo(TargetFile).Deserealize<Passives>();

        if (LoadedPassivesLocalizationData.dataList != null && LoadedPassivesLocalizationData.dataList.Count > 0)
        {
            ID_And_Index__Links_Passives.Clear();
            PassivesLocalizationIDSelector.Items.Clear();

            int ItemEnumerator = 1;
            foreach (Passive PassiveItem in LoadedPassivesLocalizationData.dataList)
            {
                if (PassiveItem.ID != null & PassiveItem.Description != null)
                {
                    string ItemName = $"{ItemEnumerator} — {PassiveItem.ID} {PassiveItem.Name.nullHandle(NullText: "<i>No name</i>")}";

                    AddSpecItemToSelector(PassivesLocalizationIDSelector, ItemName, PassiveItem, PassiveItem.ID);

                    ID_And_Index__Links_Passives[(int)PassiveItem.ID] = ItemEnumerator - 1;
                }

                ItemEnumerator++;
            }
        }

        if (PassivesLocalizationIDSelector.Items.Count > 0)
        {
            SelectPassivesLocalizationFile_Label.Text = $"Passives localization\n<size=78%><color=#fc5a03>{TargetFile.GetName()}</color></size>";
        }
    }

    private protected Dictionary<string, int> ID_And_Index__Links_Keywords = new Dictionary<string, int>();
    private protected void UpdateSelector__KeywordsLocalizationIDSelector()
    {
        string TargetFile = CustomIdentityPreviewCreator.ProjectFile.LoadedProject.Text.KeywordsLocalizationFile;

        Keywords LoadedKeywordsLocalizationData = new FileInfo(TargetFile).Deserealize<Keywords>();

        if (LoadedKeywordsLocalizationData.dataList != null && LoadedKeywordsLocalizationData.dataList.Count > 0)
        {
            ID_And_Index__Links_Keywords.Clear();
            KeywordsLocalizationIDSelector.Items.Clear();

            int ItemEnumerator = 1;
            foreach (Keyword KeywordItem in LoadedKeywordsLocalizationData.dataList)
            {
                if (KeywordItem.ID != null & KeywordItem.Description != null)
                {
                    string ItemName = $"{ItemEnumerator} — {KeywordItem.ID} {KeywordItem.Name.nullHandle(NullText: "<i>No name</i>")}";

                    AddSpecItemToSelector(KeywordsLocalizationIDSelector, ItemName, KeywordItem, KeywordItem.ID);

                    ID_And_Index__Links_Keywords[KeywordItem.ID] = ItemEnumerator - 1;
                }

                ItemEnumerator++;
            }
        }

        if (KeywordsLocalizationIDSelector.Items.Count > 0)
        {
            SelectKeywordsLocalizationFile_Label.Text = $"Keywords localization\n<size=78%><color=#fc5a03>{TargetFile.GetName()}</color></size>";
        }
    }

    private protected Dictionary<BigInteger, int> ID_And_Index__Links_SkillsDisplayInfo = new Dictionary<BigInteger, int>();
    private protected void UpdateSelector__SkillsDisplayInfoIDSelector()
    {
        FileInfo TargetFile = new FileInfo(CustomIdentityPreviewCreator.ProjectFile.LoadedProject.Text.SkillsDisplayInfoConstructorFile);

        SkillsConstructorFile LoadedSkillsLocalizationData = new FileInfo(TargetFile.FullName).Deserealize<SkillsConstructorFile>(Context: TargetFile.Directory.FullName.Replace("\\", "/"));

        if (LoadedSkillsLocalizationData.List != null && LoadedSkillsLocalizationData.List.Count > 0)
        {
            ID_And_Index__Links_SkillsDisplayInfo.Clear();
            SkillsDisplayInfoIDSelector.Items.Clear();

            int ItemEnumerator = 1;
            foreach (SkillContstructor Constructor in LoadedSkillsLocalizationData.List)
            {
                if (Constructor.ID != null)
                {
                    string AffinityColor = CustomIdentityPreviewCreator.GetAffinityColor(Constructor.Specific.Affinity).ToString().Replace("#FF", "#");

                    string TargetName = $"{ItemEnumerator} — {Constructor.ID} <color={AffinityColor}>{Constructor.SkillName.nullHandle(NullText: "<i>No name</i>")}</color>";

                    AddSpecItemToSelector(SkillsDisplayInfoIDSelector, TargetName, Constructor, Constructor.ID);

                    ID_And_Index__Links_SkillsDisplayInfo[(BigInteger)Constructor.ID] = ItemEnumerator - 1;
                }

                ItemEnumerator++;
            }
        }

        if (SkillsDisplayInfoIDSelector.Items.Count > 0)
        {
            SelectSkillsDisplayInfoFile_Label.Text = $"Skills Display Info\n<size=78%><color=#fc5a03>{TargetFile.Name}</color></size>";
        }
    }


    private protected void AddSpecItemToSelector(ComboBox Target, string ItemName, dynamic AttachedItem, dynamic AttachedItemID)
    {
        Target.Items.Add(new UILocalization_Grocerius()
        {
            FontFamily = new FontFamily("GOST Type BU"),
            Text = ItemName,
            FontSize = 14,
            Padding = new Thickness(0, 5, 0, 5),
            Width = 150,
            UniversalDataBindings = new Dictionary<string, dynamic>() { ["Attached item"] = AttachedItem, ["Attached item ID"] = AttachedItemID }
        });
    }

}


// Used in preview exports to png as reconnection items to Canvas for displaying them over other elements
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