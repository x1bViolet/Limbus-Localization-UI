using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using LC_Localization_Task_Absolute.PreviewCreator;
using LC_Localization_Task_Absolute.UITranslationHandlers;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_EGOGifts;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Passives;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.JsonSerialization;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;
using static System.Windows.Visibility;


namespace LC_Localization_Task_Absolute;

public partial class MainWindow : Window
{
    public static MainWindow MainControl;

    public static readonly List<TMProEmitter> PreviewLayoutsList = [];
    public static readonly Dictionary<TMProEmitter, double> PrimaryRegisteredFontSizes = [];
    public static readonly List<UIElement> FocusableTextBoxes = [];
    public static List<UITranslation_Mint> NameInputs = [];

    public static TMProEmitter TargetPreviewLayout; // = PreviewLayout_Default by default from InitMain()

    public static FileInfo CurrentFile;
    public static UTF8Encoding CurrentFileEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    public static LineBreakMode CurrentLineBreakMode = LineBreakMode.LF;
    public static int CurrentIndentationSize = 2;

    public ContextMenu AddUptieContextMenu => MainControl.UptieSwitchButtons.Resources["AddUptieContextMenu"] as ContextMenu; // Localization elements from ResourceDictionary
    

    public MainWindow()
    {
        InitializeComponent();

        MainControl = this;

        File.WriteAllText(@"[⇲] Assets Directory\Latest loading.txt", "");

        ConfigurationWindow.ConfigControl = new ConfigurationWindow();
        SkillsDisplayInfoManagerWindow.SkillsDisplayInfoManagerControl = new SkillsDisplayInfoManagerWindow();

        InitSurfaceScroll(NavigationPanel_ObjectName_DisplayScrollViewer);
        InitSurfaceScroll(SurfaceScrollPreview_Skills);
        InitSurfaceScroll(SurfaceScrollPreview_Keywords__Bufs);
        InitSurfaceScroll(SurfaceScrollPreview_Keywords__BattleKeywords);
        InitSurfaceScroll(SurfaceScrollPreview_Passives);
        InitSurfaceScroll(SurfaceScrollPreview_EGOGifts);
        InitSurfaceScroll(SurfaceScrollPreview_Default);
        InitSurfaceScroll(SurfaceScroll_UnsavedChangesInfo);

        ProcessLogicalTree(this);
        InitMain();
    }

    

    private void InitMain()
    {
        { // UITranslation_Rose constructors from ResourceDictionary doesn't work
            #region Identity/E.G.O Preview Creator column items context menu
            {
                ContextMenu ColumnItemsContextMenu = CompositionGrid.Resources["ColumnItemContextMenu"] as ContextMenu; // First ColumnItemContextMenu's MenuItem Header is StackPanel
                List<UITranslation_Rose> Headers = [.. ColumnItemsContextMenu.Items.OfType<MenuItem>().Select(MenuItem => MenuItem.Header is UITranslation_Rose Rose ? Rose : null)];

                PresentedStaticTextEntries["[C] * [Element context menu] Header part"] = ((ColumnItemsContextMenu.Items[0] as MenuItem).Header as StackPanel).Children[0] as UITranslation_Rose;
                PresentedStaticTextEntries["[C] * [Element context menu] Move up"] = Headers[1];
                PresentedStaticTextEntries["[C] * [Element context menu] Refresh text"] = Headers[2];
                PresentedStaticTextEntries["[C] * [Element context menu] Move down"] = Headers[3];
                PresentedStaticTextEntries["[C] * [Element context menu] Delete"] = Headers[4];
            }
            #endregion

            #region Json text editor context menu
            {
                ContextMenu JsonTextEditorContextMenu = Editor_Background.Resources["DefaultContextMenu"] as ContextMenu;
                List<UITranslation_Rose> Headers = [.. JsonTextEditorContextMenu.Items.OfType<MenuItem>().Select(MenuItem => (UITranslation_Rose)MenuItem.Header)];

                PresentedStaticTextEntries["[Context Menu] * Insert Style"] = Headers[0];
                PresentedStaticTextEntries["[Context Menu] * TextMeshPro to [KeywordID]"] = Headers[1];
                PresentedStaticTextEntries["[Context Menu] * TextMeshPro to Shorthands"] = Headers[2];
                PresentedStaticTextEntries["[Context Menu] * Unevident to [KeywordID]"] = Headers[3];
                PresentedStaticTextEntries["[Context Menu] * Unevident to Shorthands"] = Headers[4];
                PresentedStaticTextEntries["[Context Menu] * [KeywordID] to Shorthands"] = Headers[5];
                PresentedStaticTextEntries["[Context Menu] * [KeywordID] to TextMeshPro"] = Headers[6];
            }
            #endregion
        }

        TargetPreviewLayout = PreviewLayoutDef_Default;
        NameInputs =
        [
            MainControl.SWBT_Skills_MainSkillName,
            MainControl.SWBT_Skills_EGOAbnormalitySkillName,
            MainControl.SWBT_Keywords_KeywordName,
            MainControl.SWBT_Passives_MainPassiveName,
            MainControl.SWBT_EGOGifts_EGOGiftName,
        ];

        SetupIDCopyAnimation();
        SetupPreviewCreator();

        ReadConfigurazioneFile();
        ContextMenuHotkeys.ReadFile();

        if (File.Exists(@"[⇲] Assets Directory\Default Text.txt"))
        {
            TextEditor.Text = File.ReadAllText(@"[⇲] Assets Directory\Default Text.txt");
        }
        else
        {
            TextEditor.Text = "                    <font=\"BebasKai SDF\"><size=140%><u>Limbus Company Localization Interface</u> <color=#f8c200>'1.3:1</color></size></font>\n\nЧерти вышли из омута";
        }
    }



    #region Limbus live preview
    public static bool IsPendingPreviewUpdate = false;
    public static bool ManualTextLoadEvent = false;
    private void TextEditor_TextChanged(object RequestSender, EventArgs EventArgs)
    {
        if (!ManualTextLoadEvent)
        {
            if (!IsPendingPreviewUpdate)
            {
                IsPendingPreviewUpdate = true;
                try
                {
                    DispatcherTimer Timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.PreviewUpdateDelay) };
                    Timer.Start();

                    Timer.Tick += (Sender, Args) =>
                    {
                        Timer.Stop();
                        PullUpdatePreview(TextEditor.Text.Replace("\r", ""));

                        IsPendingPreviewUpdate = false;
                    };
                }
                catch { IsPendingPreviewUpdate = false; }
            }
        }
        else
        {
            PullUpdatePreview(TextEditor.Text.Replace("\r", ""));
        }
    }

    /// <summary>
    /// Write info to deserialized local json and update UI text elements
    /// </summary>
    public static void PullUpdatePreview(string InputEditorText)
    {
        #region UI/json update
        switch (ActiveProperties.Key)
        {
            case EditorMode.Skills:
                if (Mode_Skills.CurrentSkillID != int.MinValue)
                {
                    TargetPreviewLayout.Visibility = InputEditorText != "" ? Visible : Collapsed;


                    Mode_Skills.LastPreviewUpdatesBank[TargetPreviewLayout] = InputEditorText;

                    if (TargetPreviewLayout.Equals(MainControl.PreviewLayout_Skills_MainDesc))
                    {
                        Mode_Skills.@Current.Uptie.EditorMainDescription = InputEditorText;

                        if (Mode_Skills.@Current.Uptie.PresentMainDescription != Mode_Skills.@Current.Uptie.EditorMainDescription)
                        {
                            PresentedStaticTextEntries[$"[Skills / Right menu] * Skill main desc"].MarkWithUnsaved();
                        }
                        else
                        {
                            PresentedStaticTextEntries[$"[Skills / Right menu] * Skill main desc"].SetDefaultText();
                        }
                    }
                    else if (TargetPreviewLayout.Equals(MainControl.PreviewLayout_Skills_FlavorDesc))
                    {
                        Mode_Skills.@Current.Uptie.EditorFlavorDescription = InputEditorText;

                        if (Mode_Skills.@Current.Uptie.PresentFlavorDescription != Mode_Skills.@Current.Uptie.EditorFlavorDescription)
                        {
                            PresentedStaticTextEntries[$"[Skills / Right menu] * Skill flavor desc"].MarkWithUnsaved();
                        }
                        else
                        {
                            PresentedStaticTextEntries[$"[Skills / Right menu] * Skill flavor desc"].SetDefaultText();
                        }
                    }
                    else
                    {
                        Mode_Skills.@Current.CoinDesc.EditorDescription = InputEditorText;

                        if (Mode_Skills.@Current.CoinDesc.PresentDescription != Mode_Skills.@Current.CoinDesc.EditorDescription)
                        {
                            PresentedStaticTextEntries["[Skills / Right menu] * Skill Coin desc number"]
                                .MarkWithUnsaved(ExtraExtern: Mode_Skills.CurrentSkillCoinDescIndex + 1);
                        }
                        else
                        {
                            PresentedStaticTextEntries["[Skills / Right menu] * Skill Coin desc number"]
                                .SetDefaultText(ExtraExtern: Mode_Skills.CurrentSkillCoinDescIndex + 1);
                        }

                        if (Mode_Skills.@Current.Coin.CoinDescriptions.Any(CoinDesc => CoinDesc.PresentDescription != CoinDesc.EditorDescription))
                        {
                            PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"]
                                .MarkWithUnsaved();
                        }
                        else
                        {
                            PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"]
                                .SetDefaultText();
                        }


                        // Auto hide coin if its empty
                        if (Mode_Skills.@Current.Coin.CoinDescriptions.Where(x => x.EditorDescription.EqualsOneOf("", "<style=\"highlight\"></style>")).Count() == Mode_Skills.@Current.Coin.CoinDescriptions.Count)
                        {
                            InterfaceObject<Grid>($"PreviewLayout_Skills_Coin{Mode_Skills.CurrentSkillCoinIndex + 1}").Visibility = Collapsed;
                        }
                        else
                        {
                            InterfaceObject<Grid>($"PreviewLayout_Skills_Coin{Mode_Skills.CurrentSkillCoinIndex + 1}").Visibility = Visible;
                        }
                    }
                }
                break;





            case EditorMode.Passives:
                if (Mode_Passives.CurrentPassiveID != int.MinValue)
                {
                    switch (Mode_Passives.CurrentDescriptionType)
                    {
                        case TripleDescriptionType.Main:
                            Mode_Passives.@Current.Passive.EditorMainDescription = InputEditorText;

                            if (!string.IsNullOrWhiteSpace(Mode_Passives.@Current.Passive.EditorFlavorDescription))
                            {
                                InputEditorText = $"{Mode_Passives.@Current.Passive.EditorMainDescription}\n\n" +
                                                  $"<flavor\uAAFF>{Mode_Passives.@Current.Passive.EditorFlavorDescription}</flavor\uAAFF>";
                            }

                            if (Mode_Passives.@Current.Passive.PresentMainDescription != Mode_Passives.@Current.Passive.EditorMainDescription)
                            {
                                PresentedStaticTextEntries["[Passives / Right menu] * Passive desc"].MarkWithUnsaved();
                            }
                            else
                            {
                                PresentedStaticTextEntries["[Passives / Right menu] * Passive desc"].SetDefaultText();
                            }
                            break;

                        case TripleDescriptionType.Summary:
                            Mode_Passives.@Current.Passive.EditorSummaryDescription = InputEditorText;
                            
                            if (Mode_Passives.@Current.Passive.PresentSummaryDescription != Mode_Passives.@Current.Passive.EditorSummaryDescription)
                            {
                                PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"].MarkWithUnsaved();
                            }
                            else
                            {
                                PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"].SetDefaultText();
                            }
                            break;

                        case TripleDescriptionType.Flavor:
                            Mode_Passives.@Current.Passive.EditorFlavorDescription = InputEditorText;

                            if (!string.IsNullOrWhiteSpace(Mode_Passives.@Current.Passive.EditorFlavorDescription))
                            {
                                InputEditorText = $"{Mode_Passives.@Current.Passive.EditorMainDescription}\n\n" +
                                                  $"<flavor\uAAFF>{Mode_Passives.@Current.Passive.EditorFlavorDescription}</flavor\uAAFF>";
                            }
                            else InputEditorText = Mode_Passives.@Current.Passive.EditorMainDescription;

                            if (Mode_Passives.@Current.Passive.PresentFlavorDescription != Mode_Passives.@Current.Passive.EditorFlavorDescription)
                            {
                                PresentedStaticTextEntries["[Passives / Right menu] * Passive flavor"].MarkWithUnsaved();
                            }
                            else
                            {
                                PresentedStaticTextEntries["[Passives / Right menu] * Passive flavor"].SetDefaultText();
                            }

                            break;
                    }
                }

                break;





            case EditorMode.Keywords:
                if (Mode_Keywords.CurrentKeywordID != "")
                {
                    switch (Mode_Keywords.CurrentDescriptionType)
                    {
                        case TripleDescriptionType.Main:
                            Mode_Keywords.@Current.Keyword.EditorMainDescription = InputEditorText;

                            if (!string.IsNullOrWhiteSpace(Mode_Keywords.@Current.Keyword.EditorFlavorDescription))
                            {
                                InputEditorText = $"{Mode_Keywords.@Current.Keyword.EditorMainDescription}\n\n" +
                                                  $"<flavor\uAAFF>{Mode_Keywords.@Current.Keyword.EditorFlavorDescription}</flavor\uAAFF>";
                            }

                            if (Mode_Keywords.@Current.Keyword.PresentMainDescription != Mode_Keywords.@Current.Keyword.EditorMainDescription)
                            {
                                PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword desc"].MarkWithUnsaved();
                            }
                            else
                            {
                                PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword desc"].SetDefaultText();
                            }
                            break;

                        case TripleDescriptionType.Summary:
                            Mode_Keywords.@Current.Keyword.EditorSummaryDescription = InputEditorText;

                            if (Mode_Keywords.@Current.Keyword.PresentSummaryDescription != Mode_Keywords.@Current.Keyword.EditorSummaryDescription)
                            {
                                PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"].MarkWithUnsaved();
                            }
                            else
                            {
                                PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"].SetDefaultText();
                            }
                            break;

                        case TripleDescriptionType.Flavor:
                            Mode_Keywords.@Current.Keyword.EditorFlavorDescription = InputEditorText;

                            if (!string.IsNullOrWhiteSpace(Mode_Keywords.@Current.Keyword.EditorFlavorDescription))
                            {
                                InputEditorText = $"{Mode_Keywords.@Current.Keyword.EditorMainDescription}\n\n" +
                                                  $"<flavor\uAAFF>{Mode_Keywords.@Current.Keyword.EditorFlavorDescription}</flavor\uAAFF>";
                            }
                            else InputEditorText = Mode_Keywords.@Current.Keyword.EditorMainDescription;

                            if (Mode_Keywords.@Current.Keyword.PresentSummaryDescription != Mode_Keywords.@Current.Keyword.EditorSummaryDescription)
                            {
                                PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword flavor"].MarkWithUnsaved();
                            }
                            else
                            {
                                PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword flavor"].SetDefaultText();
                            }
                            break;
                    }
                }

                break;





            case EditorMode.EGOGifts:
                if (Mode_EGOGifts.CurrentEGOGiftID != int.MinValue)
                {
                    switch (Mode_EGOGifts.CurrentDescriptionType_String)
                    {
                        case "Main Description":

                            Mode_EGOGifts.@Current.EGOGift.EditorDescription = InputEditorText;

                            if (Mode_EGOGifts.@Current.EGOGift.PresentDescription != Mode_EGOGifts.@Current.EGOGift.EditorDescription)
                            {
                                PresentedStaticTextEntries["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"].MarkWithUnsaved();
                            }
                            else
                            {
                                PresentedStaticTextEntries["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"].SetDefaultText();
                            }
                            break;

                        default:

                            string SimpleDescNumber = Regex.Match(Mode_EGOGifts.CurrentDescriptionType_String, @"Simple Description №(\d+)").Groups[1].Value;

                            int TargetSimpleDescIndex = int.Parse(SimpleDescNumber) - 1;

                            Mode_EGOGifts.@Current.EGOGift.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription = InputEditorText;

                            if (Mode_EGOGifts.@Current.EGOGift.SimpleDescriptions[TargetSimpleDescIndex].PresentDescription != Mode_EGOGifts.@Current.EGOGift.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription)
                            {
                                PresentedStaticTextEntries[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"].MarkWithUnsaved();
                            }
                            else
                            {
                                PresentedStaticTextEntries[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"].SetDefaultText();
                            }

                            break;
                    }
                }

                break;





            default: break;
        }
        #endregion

        // Update preview itself
        if (TargetPreviewLayout != null && !LoadedProgramConfig.PreviewSettings.PreviewSettingsBaseSettings.HidePreview)
        {
            TargetPreviewLayout.RichText = InputEditorText;
        }
    }
    #endregion



    private void ProcessLogicalTree(object Current)
    {
        if (Current is FrameworkElement)
        {
            // Fill special lists

            string ElementName = (Current as FrameworkElement).Name;
            if (ElementName.StartsWith("PreviewLayout_") && Current is TMProEmitter LimbusPreviewLayout)
            {
                PreviewLayoutsList.Add(LimbusPreviewLayout);
                PrimaryRegisteredFontSizes[LimbusPreviewLayout] = LimbusPreviewLayout.FontSize;
            }
            else if (Current is TextBox | Current is UITranslation_Mint | Current is UITranslation_Hyacinth)
            {
                FocusableTextBoxes.Add(Current as UIElement);
            }
        }

        DependencyObject AsDependencyObjectParent = Current as DependencyObject;

        if (AsDependencyObjectParent != null)
        {
            foreach (object Child in LogicalTreeHelper.GetChildren(AsDependencyObjectParent))
            {
                ProcessLogicalTree(Child);
            }
        }
    }
    private void Window_Loaded(object RequestSender, RoutedEventArgs EventArgs)
    {
        if (LoadErrors != "" & LoadedProgramConfig.Internal.ShowLoadWarnings)
        {
            ShowLoadWarningsWindow();
        }

        if (LoadedProgramConfig.TechnicalActions.KeywordsDictionary.GenerateOnStartup && !File.Exists("Keywords Multiple Meanings.json"))
        {
            if (Directory.Exists(LoadedProgramConfig.TechnicalActions.KeywordsDictionary.SourcePath))
            {
                KeywordsInterrogation.ExportKeywordsMultipleMeaningsDictionary(
                    LoadedProgramConfig.TechnicalActions.KeywordsDictionary.SourcePath
                );
            }
            else
            {
                MessageBox.Show($"Keywords multiple meanings dir \"{LoadedProgramConfig.TechnicalActions.KeywordsDictionary.SourcePath}\" not found");
            }
        }
    }



    private void OpenCurrentFileWithExternalEditor(object RequestSender, RoutedEventArgs EventArgs)
    {
        if (JsonFilePath.Text != "")
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + JsonFilePath.Text + "\"";
            fileopener.Start();
        }
    }



    #region ID Switch system
    private bool SwitchToFirstItem = false;
    private bool SwitchToLastItem = false;

    private void NavigationPanel_IDSwitch(object RequestSender, RoutedEventArgs EventArgs)
    {
        FrameworkElement Sender = RequestSender as FrameworkElement;
        string Direction = Sender.Name.Split("NavigationPanel_IDSwitch_")[^1];

        bool SuccessSwitch = false;

        switch (ActiveProperties.Key)
        {
            case EditorMode.Skills:
                {
                    int IndexOfCurrentID = DelegateSkills_IDList.IndexOf(Mode_Skills.CurrentSkillID);
                    int TargetSwitchIDIndex = Direction == "Next" ? IndexOfCurrentID + 1 : IndexOfCurrentID - 1;

                    if (TargetSwitchIDIndex <= (DelegateSkills_IDList.Count - 1) & TargetSwitchIDIndex >= 0)
                    {
                        if (!(SwitchToFirstItem | SwitchToLastItem))
                        {
                            Mode_Skills.TransformToSkill(DelegateSkills_IDList[TargetSwitchIDIndex]);
                        }
                        else
                        {
                            Mode_Skills.TransformToSkill(DelegateSkills_IDList[SwitchToFirstItem ? 0 : (DelegateSkills_IDList.Count - 1)]);
                        }
                        SuccessSwitch = true;
                    }
                }
                break;

            case EditorMode.Passives:
                {
                    int IndexOfCurrentID = DelegatePassives_IDList.IndexOf(Mode_Passives.CurrentPassiveID);
                    int TargetSwitchIDIndex = Direction == "Next" ? IndexOfCurrentID + 1 : IndexOfCurrentID - 1;

                    if (TargetSwitchIDIndex <= (DelegatePassives_IDList.Count - 1) & TargetSwitchIDIndex >= 0)
                    {
                        if (!(SwitchToFirstItem | SwitchToLastItem))
                        {
                            Mode_Passives.TransformToPassive(DelegatePassives_IDList[TargetSwitchIDIndex]);
                        }
                        else
                        {
                            Mode_Passives.TransformToPassive(DelegatePassives_IDList[SwitchToFirstItem ? 0 : (DelegatePassives_IDList.Count - 1)]);
                        }
                        SuccessSwitch = true;
                    }
                }
                break;

            case EditorMode.Keywords:
                {
                    int IndexOfCurrentID = DelegateKeywords_IDList.IndexOf(Mode_Keywords.CurrentKeywordID);
                    int TargetSwitchIDIndex = Direction == "Next" ? IndexOfCurrentID + 1 : IndexOfCurrentID - 1;

                    if (TargetSwitchIDIndex <= (DelegateKeywords_IDList.Count - 1) & TargetSwitchIDIndex >= 0)
                    {
                        if (!(SwitchToFirstItem | SwitchToLastItem))
                        {
                            Mode_Keywords.TransformToKeyword(DelegateKeywords_IDList[TargetSwitchIDIndex]);
                        }
                        else
                        {
                            Mode_Keywords.TransformToKeyword(DelegateKeywords_IDList[SwitchToFirstItem ? 0 : (DelegateKeywords_IDList.Count - 1)]);
                        }
                        SuccessSwitch = true;
                    }
                }
                break;

            case EditorMode.EGOGifts:
                {
                    int IndexOfCurrentID = DelegateEGOGifts_IDList.IndexOf(Mode_EGOGifts.CurrentEGOGiftID);
                    int TargetSwitchIDIndex = Direction == "Next" ? IndexOfCurrentID + 1 : IndexOfCurrentID - 1;

                    if (TargetSwitchIDIndex <= (DelegateEGOGifts_IDList.Count - 1) & TargetSwitchIDIndex >= 0)
                    {
                        if (!(SwitchToFirstItem | SwitchToLastItem))
                        {
                            Mode_EGOGifts.TransformToEGOGift(DelegateEGOGifts_IDList[TargetSwitchIDIndex]);
                        }
                        else
                        {
                            Mode_EGOGifts.TransformToEGOGift(DelegateEGOGifts_IDList[SwitchToFirstItem ? 0 : (DelegateEGOGifts_IDList.Count - 1)]);
                        }
                        SuccessSwitch = true;
                    }
                }
                break;
        }


        if (SuccessSwitch) NavigationPanel_IDSwitch_ManualInput_Stop();
    }

    #region To Last/First
    private void PreventAppendAdditionalObjectTooltip(object RequestSender, ContextMenuEventArgs EventArgs) => EventArgs.Handled = true;
    private void NavigationPanel_IDSwitch_ToLast(object RequestSender, RoutedEventArgs EventArgs)
    {
        // Temporary disable context menu open on RMB click
        AppendAdditionalObjectContextMenu_ParentBorder.ContextMenuOpening += PreventAppendAdditionalObjectTooltip;

        SwitchToLastItem = true;
        NavigationPanel_IDSwitch(RequestSender, EventArgs);
        SwitchToLastItem = false;

        Await(0.5, () => { AppendAdditionalObjectContextMenu_ParentBorder.ContextMenuOpening -= PreventAppendAdditionalObjectTooltip; });
    }
    private void NavigationPanel_IDSwitch_ToFirst(object RequestSender, RoutedEventArgs EventArgs)
    {
        SwitchToFirstItem = true;
        NavigationPanel_IDSwitch(RequestSender, EventArgs);
        SwitchToFirstItem = false;
    }
    #endregion
    
    public static void NavigationPanel_IDSwitch_CheckAvalibles()
    {
        static void Check(int IndexOfCurrentID, int MaxID)
        {
            // If first item -> Hide 'Previous'
            if (IndexOfCurrentID == 0)
            {
                MainControl.NavigationPanel_IDSwitch_Previous.IsEnabled = false;
            }
            else MainControl.NavigationPanel_IDSwitch_Previous.IsEnabled = true;

            // If last item -> Hide 'Next'
            if ((IndexOfCurrentID + 1) == MaxID)
            {
                MainControl.NavigationPanel_IDSwitch_Next.IsEnabled = false;
                MainControl.AppendAdditionalObjectContextMenu_ParentBorder.Visibility = Visibility.Visible;
            }
            else
            {
                MainControl.NavigationPanel_IDSwitch_Next.IsEnabled = true;
                MainControl.AppendAdditionalObjectContextMenu_ParentBorder.Visibility = Visibility.Collapsed;
            }
        }


        switch (ActiveProperties.Key)
        {
            case EditorMode.Skills:
                {
                    int IndexOfCurrentID = DelegateSkills_IDList.IndexOf(Mode_Skills.CurrentSkillID);
                    int MaximumID = DelegateSkills_IDList.Count;

                    Check(IndexOfCurrentID, MaximumID);
                }
                break;

            case EditorMode.Passives:
                {
                    int IndexOfCurrentID = DelegatePassives_IDList.IndexOf(Mode_Passives.CurrentPassiveID);
                    int MaximumID = DelegatePassives_IDList.Count;

                    Check(IndexOfCurrentID, MaximumID);
                }
                break;

            case EditorMode.Keywords:
                {
                    int IndexOfCurrentID = DelegateKeywords_IDList.IndexOf(Mode_Keywords.CurrentKeywordID);
                    int MaximumID = DelegateKeywords_IDList.Count;

                    Check(IndexOfCurrentID, MaximumID);
                }
                break;

            case EditorMode.EGOGifts:
                {
                    int IndexOfCurrentID = DelegateEGOGifts_IDList.IndexOf(Mode_EGOGifts.CurrentEGOGiftID);
                    int MaximumID = DelegateEGOGifts_IDList.Count;

                    Check(IndexOfCurrentID, MaximumID);
                }
                break;

            default: break;
        }
    }


    #region ID Manual input
    private void NavigationPanel_IDSwitch_ManualInput_Start(object RequestSender, RoutedEventArgs EventArgs)
    {
        string IDText = STE_NavigationPanel_ObjectID_Display.RichText;
        if (IDText != SpecializedDefs.InsertionsDefaultValue)
        {
            STE_NavigationPanel_ObjectID_Display.Visibility = Collapsed;
            STE_NavigationPanel_ObjectID_Display_IDCopied.Visibility = Collapsed;

            NavigationPanel_IDSwitch_ManualInput_Textfield.CaretIndex = NavigationPanel_IDSwitch_ManualInput_Textfield.Text.Length;
            NavigationPanel_IDSwitch_ManualInput_Textfield.Visibility = Visible;

            NavigationPanel_IDSwitch_ManualInput_Textfield.Focus();

            // 'Enter' press at the Window_PreviewKeyDown()
        }
    }
    public void NavigationPanel_IDSwitch_ManualInput_Stop()
    {
        STE_NavigationPanel_ObjectID_Display.Visibility = Visible;
        STE_NavigationPanel_ObjectID_Display_IDCopied.Visibility = Visible;
        NavigationPanel_IDSwitch_ManualInput_Textfield.Visibility = Collapsed;

        UnfocusElement(NavigationPanel_IDSwitch_ManualInput_Textfield);

        NavigationPanel_IDSwitch_ManualInput_Textfield.Text = "";
    }
    #endregion



    #region ID copying animation
    private static bool AlreadyAnimatingCopiedInfo = false;
    private static double FadeInOutTime = 0.118;
    private static double AnimPauseTime = 0.35;
    private DoubleAnimation FadeIn_IDSign        = new DoubleAnimation() { From = 0, To = 1, Duration = new(TimeSpan.FromSeconds(FadeInOutTime)) };
    private DoubleAnimation FadeOut_IDCopiedText = new DoubleAnimation() { From = 1, To = 0, Duration = new(TimeSpan.FromSeconds(FadeInOutTime)) };
    private DoubleAnimation FadeIn_IDCopiedText  = new DoubleAnimation() { From = 0, To = 1, Duration = new(TimeSpan.FromSeconds(FadeInOutTime)) };
    private DoubleAnimation FadeOut_IDSign       = new DoubleAnimation() { From = 1, To = 0, Duration = new(TimeSpan.FromSeconds(FadeInOutTime)) };
    private void SetupIDCopyAnimation()
    {
        // Hide ID, show 'ID Copied' text
        FadeOut_IDSign.Completed       += (s, e) => STE_NavigationPanel_ObjectID_Display_IDCopied.BeginAnimation(OpacityProperty, FadeIn_IDCopiedText);
        FadeIn_IDCopiedText.Completed  += (s, e) => Await(AnimPauseTime, () => { STE_NavigationPanel_ObjectID_Display_IDCopied.BeginAnimation(OpacityProperty, FadeOut_IDCopiedText); });

        // Hide 'ID Copied' text, show ID
        FadeOut_IDCopiedText.Completed += (s, e) => STE_NavigationPanel_ObjectID_Display.BeginAnimation(OpacityProperty, FadeIn_IDSign);
        FadeIn_IDSign.Completed        += (s, e) => AlreadyAnimatingCopiedInfo = false;
    }
    #endregion

    private void CopyID(object RequestSender, RoutedEventArgs EventArgs)
    {
        if (!NavigationPanel_IDSwitch_ManualInput_Textfield.IsFocused)
        {
            try
            {
                string IDText = STE_NavigationPanel_ObjectID_Display.RichText;
                if (IDText != SpecializedDefs.InsertionsDefaultValue)
                {
                    Clipboard.SetDataObject(STE_NavigationPanel_ObjectID_Display.RichText); // SetText() causes exceptions sometimes

                    if (!AlreadyAnimatingCopiedInfo)
                    {
                        AlreadyAnimatingCopiedInfo = true;
                        STE_NavigationPanel_ObjectID_Display.BeginAnimation(OpacityProperty, FadeOut_IDSign);
                    }
                }
            }
            catch { }
        }
    }
    #endregion



    #region Shared
    private void ChangeObjectName(object RequestSender, RoutedEventArgs EventArgs)
    {
        switch (ActiveProperties.Key)
        {
            case EditorMode.Skills:

                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    foreach (var Uptie in Mode_Skills.@Current.Skill) Uptie.Value.Name = SWBT_Skills_MainSkillName.Text.Replace("\\n", "\n");
                }
                else
                {
                    Mode_Skills.@Current.Uptie.Name = SWBT_Skills_MainSkillName.Text.Replace("\\n", "\n");
                }
                NavigationPanel_ObjectName_Display.Text = Mode_Skills.@Current.Uptie.Name;

                Mode_Skills.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                
                break;


            case EditorMode.Passives:

                Mode_Passives.@Current.Passive.Name = SWBT_Passives_MainPassiveName.Text.Replace("\\n", "\n");
                NavigationPanel_ObjectName_Display.Text = Mode_Passives.@Current.Passive.Name;

                Mode_Passives.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                
                break;


            case EditorMode.Keywords:

                Mode_Keywords.@Current.Keyword.Name = SWBT_Keywords_KeywordName.Text.Replace("\\n", "\n");
                NavigationPanel_ObjectName_Display.Text = Mode_Keywords.@Current.Keyword.Name;

                Mode_Keywords.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                
                break;


            case EditorMode.EGOGifts:

                Mode_EGOGifts.@Current.EGOGift.Name = SWBT_EGOGifts_EGOGiftName.Text.Replace("\\n", "\n");
                NavigationPanel_ObjectName_Display.Text = Mode_EGOGifts.@Current.EGOGift.Name;

                Mode_EGOGifts.DeserializedInfo.SerializeToFormattedFile_CurrentLimbusJson(CurrentFile.FullName);
                
                break;
        }
    }

    private void SelectFile_ButtonClick(object RequestSender, RoutedEventArgs EventArgs) => CheckUnsavedChanges(SelectFileOnEnd: true);
    public void SelectFile_Action()
    {
        OpenFileDialog JsonFileSelector = NewOpenFileDialog("Limbus localization files", ["json"]);

        if (JsonFileSelector.ShowDialog() == true)
        {
            TryLoadFileAndSetFocus(JsonFileSelector.FileName);
        }
    }

    private void TryLoadFileAndSetFocus(string FilePath)
    {
        FileInfo TemplateTarget = new FileInfo(FilePath);

        string CheckName = TemplateTarget.Name.RemovePrefix("JP_", "KR_", "EN_");

        if (TryAcquireManualFileType(TemplateTarget.FullName, out string AcquiredType))
        {
            CheckName = AcquiredType;
        }

        if (CheckName.StartsWith("Skills"))
        {
            Mode_Skills.ValidateAndLoadStructure(
                JsonFile: TemplateTarget,
                EnableUptieLevels: CheckName.ContainsOneOf(
                    "Skills_Ego_Personality-",
                    "Skills_personality-",
                    "Skills.json",
                    "Skills_Ego.json",
                    "Skills_Assist.json"
                ),
                EnableEGOAbnormalityName: CheckName.ContainsOneOf(
                    "Skills_Ego_Personality-",
                    "Skills_Ego.json"
                )
            );
        }
        else if (CheckName.StartsWith("Passive")) Mode_Passives.ValidateAndLoadStructure(TemplateTarget);
        else if (CheckName.StartsWith("EGOgift")) Mode_EGOGifts.ValidateAndLoadStructure(TemplateTarget);
        else if (CheckName.StartsWithOneOf("BattleKeywords", "Bufs"))
        {
            Mode_Keywords.ValidateAndLoadStructure(TemplateTarget, KeywordsType: CheckName);
        }

        JsonTextEditor.RecompileEditorSyntax();
    }
    public static void FocusOnFile(FileInfo Target) // Called after successful validation of dataList at the ValidateAnd method of each editor mode
    {
        CurrentFile = Target;

        string TempFileText = File.ReadAllText(CurrentFile.FullName);
        CurrentIndentationSize = TempFileText.GetJsonIndentationSize();
        CurrentLineBreakMode = TempFileText.DetermineLineBreakType();
        CurrentFileEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: CurrentFile.IsUTF8BOM());

        MainControl.JsonFilePath.Text = CurrentFile.FullName;
        MainControl.JsonFilePath.ScrollToHorizontalOffset(double.PositiveInfinity);
        MainControl.AppendAdditionalObjectContextMenu.IsOpen = false;
        if (LoadedProgramConfig.Internal.EnableManualJsonFilesManaging)
        {
            MainControl.AppendAdditionalObjectContextMenu_ParentBorder.Visibility = Visible;
        }
    }
    #endregion



    #region Technical

    #region Unsaved changes
    private bool GlobalSelectFileOnEnd = false;
    public void CheckUnsavedChanges(bool ExitOnEnd = false, bool SelectFileOnEnd = false)
    {
        int UnsavedChangesCount = 0;
        string UnsavedChangesInfo = "";

        switch (ActiveProperties.Key)
        {
            case EditorMode.Passives:
                foreach (KeyValuePair<int, Passive> CheckPassive in DelegatePassives)
                {
                    bool UnsavedChangesInPassiveDesc = false;
                    bool UnsavedChangesInPassiveSummary = false;

                    if (CheckPassive.Value.PresentMainDescription != CheckPassive.Value.EditorMainDescription)
                    {
                        UnsavedChangesInPassiveDesc = true;
                        UnsavedChangesCount++;
                    }
                    if (CheckPassive.Value.PresentSummaryDescription != null)
                    {
                        if (CheckPassive.Value.PresentSummaryDescription != CheckPassive.Value.EditorSummaryDescription)
                        {
                            UnsavedChangesInPassiveSummary = true;
                            UnsavedChangesCount++;
                        }
                    }

                    if (UnsavedChangesInPassiveDesc | UnsavedChangesInPassiveSummary)
                    {
                        UnsavedChangesInfo += SpecializedDefs.UnsavedChangesInfo.Passives.IDHeader.Exform(CheckPassive.Key, CheckPassive.Value.Name);
                        if (UnsavedChangesInPassiveDesc)
                        {
                            UnsavedChangesInfo += SpecializedDefs.UnsavedChangesInfo.Passives.MainDesc;
                        }
                        if (UnsavedChangesInPassiveSummary)
                        {
                            UnsavedChangesInfo += SpecializedDefs.UnsavedChangesInfo.Passives.SummaryDesc;
                        }
                    }
                }

                break;


            case EditorMode.Skills:
                foreach (KeyValuePair<int, Dictionary<int, UptieLevel>> CheckSkill in DelegateSkills)
                {
                    bool AlreadyAddedThisID = false;
                    string SkillName = "";
                    foreach (UptieLevel UptieLevel in CheckSkill.Value.Values)
                    {
                        SkillName = UptieLevel.Name;
                        bool AnythingChanged = false;
                        
                        if (UptieLevel.PresentMainDescription != UptieLevel.EditorMainDescription)
                        {
                            AnythingChanged = true;
                        }
                        
                        if (UptieLevel.Coins != null)
                        {
                            foreach (Coin Coin in UptieLevel.Coins)
                            {
                                if (Coin != null && Coin.CoinDescriptions != null)
                                {
                                    foreach (CoinDesc CoinDesc in Coin.CoinDescriptions)
                                    {
                                        if (CoinDesc != null && CoinDesc.PresentDescription != null)
                                        {
                                            if (CoinDesc.PresentDescription != CoinDesc.EditorDescription) AnythingChanged = true;
                                        }
                                    }
                                }
                            }
                        }


                        if (AnythingChanged)
                        {
                            if (!AlreadyAddedThisID)
                            {
                                UnsavedChangesInfo += SpecializedDefs.UnsavedChangesInfo.Skills.IDHeader.Exform(CheckSkill.Key, SkillName);
                                AlreadyAddedThisID = true;
                            }
                            UnsavedChangesInfo += SpecializedDefs.UnsavedChangesInfo.Skills.UptieLevel.Extern($"{UptieLevel.Uptie}");
                        }
                    }
                }
                break;


            case EditorMode.Keywords:
                foreach (KeyValuePair<string, Keyword> CheckKeyword in DelegateKeywords)
                {
                    bool UnsavedChangesInKeywordDesc = false;
                    bool UnsavedChangesInKeywordSummary = false;

                    if (CheckKeyword.Value.PresentMainDescription != CheckKeyword.Value.EditorMainDescription)
                    {
                        UnsavedChangesInKeywordDesc = true;
                        UnsavedChangesCount++;
                    }
                    if (CheckKeyword.Value.PresentSummaryDescription != null)
                    {
                        if (CheckKeyword.Value.PresentSummaryDescription != CheckKeyword.Value.EditorSummaryDescription)
                        {
                            UnsavedChangesInKeywordSummary = true;
                            UnsavedChangesCount++;
                        }
                    }

                    if (UnsavedChangesInKeywordDesc | UnsavedChangesInKeywordSummary)
                    {
                        UnsavedChangesInfo += SpecializedDefs.UnsavedChangesInfo.Keywords.IDHeader.Exform(CheckKeyword.Key, CheckKeyword.Value.Name);
                        if (UnsavedChangesInKeywordDesc)
                        {
                            UnsavedChangesInfo += SpecializedDefs.UnsavedChangesInfo.Keywords.MainDesc;
                        }
                        if (UnsavedChangesInKeywordSummary)
                        {
                            UnsavedChangesInfo += SpecializedDefs.UnsavedChangesInfo.Passives.MainDesc;
                        }
                    }
                }
                break;


            case EditorMode.EGOGifts:
                if (DelegateEGOGifts.Keys.Count != 0)
                {
                    foreach (KeyValuePair<int, EGOGift> CheckEGOGift in DelegateEGOGifts)
                    {
                        string ChangedSimpleDescs = "";
                        bool ChangedDesc = false;
                        if (CheckEGOGift.Value.PresentDescription != CheckEGOGift.Value.EditorDescription)
                        {
                            ChangedDesc = true;
                        }
                        if (CheckEGOGift.Value.SimpleDescriptions != null)
                        {
                            int SimpleDescIndexer = 1;
                            foreach (SimpleDescription SimpleDesc in CheckEGOGift.Value.SimpleDescriptions)
                            {
                                if (SimpleDesc.PresentDescription != SimpleDesc.EditorDescription)
                                {
                                    ChangedSimpleDescs += SpecializedDefs.UnsavedChangesInfo.EGOGifts.SimpleDesc.Extern(SimpleDescIndexer);
                                }

                                SimpleDescIndexer++;
                            }
                        }

                        if (ChangedDesc | ChangedSimpleDescs != "")
                        {
                            UnsavedChangesInfo += SpecializedDefs.UnsavedChangesInfo.EGOGifts.IDHeader.Exform(CheckEGOGift.Key, CheckEGOGift.Value.Name);
                            if (ChangedDesc)
                            {
                                UnsavedChangesInfo += SpecializedDefs.UnsavedChangesInfo.EGOGifts.MainDesc;
                            }
                            if (ChangedSimpleDescs != "")
                            {
                                UnsavedChangesInfo += ChangedSimpleDescs;
                            }
                        }
                    }
                }
                break;
        }
        
        if (UnsavedChangesInfo != "")
        {
            if (@CurrentPreviewCreator.IsActive) SwitchUI_Deactivate();

            UnsavedChangesInfoTextDisplayer.RichText = UnsavedChangesInfo.Trim();
            UnsavedChangesInfoGrid.Visibility = Visible;
            if (SelectFileOnEnd) GlobalSelectFileOnEnd = true;
        }
        else
        {
            if (ExitOnEnd) Application.Current.Shutdown();
            else if (SelectFileOnEnd) SelectFile_Action();
        }
    }
    private void UnsavedChangesDialog_ConfirmProceed(object RequestSender, RoutedEventArgs EventArgs)
    {
        if (GlobalSelectFileOnEnd)
        {
            SelectFile_Action();
            UnsavedChangesInfoGrid.Visibility = Collapsed;
            GlobalSelectFileOnEnd = false;
        }
        else Application.Current.Shutdown();
    }
    private void UnsavedChangesDialog_Cancel(object RequestSender, RoutedEventArgs EventArgs)
    {
        UnsavedChangesInfoGrid.Visibility = Collapsed;
        GlobalSelectFileOnEnd = false;
    }
    #endregion

    // Auto hide shadow text for textfields
    private void SWBT_TextChanged_Shared(object RequestSender, TextChangedEventArgs EventArgs)
    {
        UITranslation_Mint Textfield = RequestSender as UITranslation_Mint;
        UITranslation_Rose ShadowText = (Textfield.Parent as Grid).Children[0] as UITranslation_Rose;

        ShadowText.Visibility = Textfield.Text == "" ? Visible : Collapsed;

        switch (Textfield.Name)
        {
            case nameof(SWBT_Skills_MainSkillName):
                SkillNamesReplication_SkillName_Text.RichText = SWBT_Skills_MainSkillName.Text.Replace("\\n", "\n");
                break;

            case nameof(SWBT_Passives_MainPassiveName):
                PassiveNamesReplication_PassiveName_Text.RichText = SWBT_Passives_MainPassiveName.Text.Replace("\\n", "\n");
                break;

            case nameof(SWBT_Keywords_KeywordName):
                PreviewLayout_Keywords_Bufs_Name.RichText = SWBT_Keywords_KeywordName.Text.Replace("\\n", "\n");
                PreviewLayout_Keywords_BattleKeywords_Name.RichText = SWBT_Keywords_KeywordName.Text.Replace("\\n", "\n");
                break;

            case nameof(SWBT_EGOGifts_EGOGiftName):
                EGOGiftName_PreviewLayout.RichText = SWBT_EGOGifts_EGOGiftName.Text.Replace("\\n", "\n");
                break;

            default:

                if (Textfield.UID.StartsWith("[Keywords / Right Menu] * Format Insertion "))
                {
                    string FormatInsertionNumber = Textfield.Uid;

                    LimbusPreviewFormatter.FormatInsertionsReplaceValues[FormatInsertionNumber] = Textfield.Text == "" ? $"{{{FormatInsertionNumber}}}" : Textfield.Text.Replace("\\n", "\n");
                    TextEditor_TextChanged(null, null);
                }

                break;
        }
    }
    

    public static void UnfocusElement(DependencyObject Target)
    {
        FocusManager.SetFocusedElement(FocusManager.GetFocusScope(Target), null);
        Keyboard.ClearFocus();
        MainControl.Focus();
    }
    #endregion





    #region Editor context Menu

    // Switch to extra replacements when left shift is pressed
    private void TextEditor_ContextMenuOpening(object RequestSender, ContextMenuEventArgs EventArgs)
    {
        if (Keyboard.IsKeyDown(Key.LeftShift))
        {
            TextEditor.ContextMenu = @ExtraReplacements.ContextMenu;
        }
    }
    private void TextEditor_ContextMenuClosing(object RequestSender, ContextMenuEventArgs EventArgs)
    {
        TextEditor.ContextMenu = Editor_Background.Resources["DefaultContextMenu"] as ContextMenu;
    }


    public void TextEditor_SharedContextMenuClick(object RequestSender, RoutedEventArgs EventArgs)
    {
        string SelectedTextToEdit = TextEditor.SelectedText;
        MenuItem ActualSender = RequestSender as MenuItem;
        switch (ActualSender.Name)
        {
            case "InsertStyle":
                SelectedTextToEdit = $"<style=\"{(ActiveProperties.Key.EqualsOneOf(EditorMode.Skills, EditorMode.Passives) ? "highlight" : "upgradeHighlight")}\">{SelectedTextToEdit}</style>";
                break;

            case "TMProToKeywordID":
                SelectedTextToEdit = LimbusPreviewFormatter.RemoteRegexPatterns.TMProKeyword.Replace(SelectedTextToEdit, Match =>
                {
                    string ID = Match.Groups["ID"].Value;
                    if (KeywordsInterrogation.Keywords_Bufs.ContainsKey(ID))
                    {
                        return $"[{ID}]";
                    }
                    else return Match.Groups[0].Value;
                });

                break;

            case "TMProToShorthands":
                SelectedTextToEdit = LimbusPreviewFormatter.RemoteRegexPatterns.TMProKeyword.Replace(SelectedTextToEdit, Match =>
                {
                    string ID = Match.Groups["ID"].Value;
                    string Color = Match.Groups["Color"].Value;
                    string Name = Match.Groups["Name"].Value;
                    if (KeywordsInterrogation.Keywords_Bufs.ContainsKey(ID))
                    {
                        string CustomColorAttach = "";
                        if (Color != KeywordsInterrogation.Keywords_Bufs[ID].StringColor)
                        {
                            CustomColorAttach = @CurrentConfess.ShorthandsInsertionParams.InsertionShape_Color.Replace("<HexColor>", Color);
                            //CustomColorAttach = ShorthandsInsertionShape.InsertionShape_Color.Replace("<HexColor>", Color);
                        }

                        //string OutputShorthand = ShorthandsInsertionShape.InsertionShape.Replace("<KeywordID>", ID).Replace("<KeywordName>", Name).Replace("<KeywordColor>", CustomColorAttach);
                        string OutputShorthand = @CurrentConfess.ShorthandsInsertionParams.InsertionShape.Replace("<KeywordID>", ID).Replace("<KeywordName>", Name).Replace("<KeywordColor>", CustomColorAttach);

                        return OutputShorthand;
                    }
                    else return Match.Groups[0].Value;
                });
                break;

            case "UnevidentToKeywordID":
                foreach (KeyValuePair<string, string> UnevidentKeyword in KeywordsInterrogation.Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter)
                {
                    if (TextEditor.Text.Contains(UnevidentKeyword.Key))
                    {
                        SelectedTextToEdit = Regex.Replace(SelectedTextToEdit, LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection.Replace("KeywordNameWillBeHere", UnevidentKeyword.Key.ToEscapeRegexString()), Match =>
                        {
                            return $"[{UnevidentKeyword.Value}]";
                        });
                    }
                }
                break;

            case "UnevidentToShorthands":
                foreach (KeyValuePair<string, string> UnevidentKeyword in KeywordsInterrogation.Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter)
                {
                    if (SelectedTextToEdit.Contains(UnevidentKeyword.Key))
                    {
                        SelectedTextToEdit = Regex.Replace(SelectedTextToEdit, LimbusPreviewFormatter.RemoteRegexPatterns.AutoKeywordsDetection.Replace("KeywordNameWillBeHere", UnevidentKeyword.Key.ToEscapeRegexString()), Match =>
                        {
                            //return ShorthandsInsertionShape.InsertionShape.Replace("<KeywordID>", UnevidentKeyword.Value).Replace("<KeywordName>", UnevidentKeyword.Key.Replace(" ", "<\0TMPSPACE>")).Replace("<KeywordColor>", "");
                            return @CurrentConfess.ShorthandsInsertionParams.InsertionShape.Replace("<KeywordID>", UnevidentKeyword.Value).Replace("<KeywordName>", UnevidentKeyword.Key.Replace(" ", "<\0TMPSPACE>")).Replace("<KeywordColor>", "");
                        });
                    }
                }
                break;

            case "KeywordIDToShorthands":
                SelectedTextToEdit = LimbusPreviewFormatter.RemoteRegexPatterns.SquareBracketLike.Replace(SelectedTextToEdit, Match =>
                {
                    string ID = Match.Groups["ID"].Value;
                    if (KeywordsInterrogation.Keywords_Bufs.ContainsKey(ID))
                    {
                        
                        //return ShorthandsInsertionShape.InsertionShape.Replace("<KeywordID>", ID).Replace("<KeywordName>", KeywordsInterrogate.KeywordsGlossary[ID].Name).Replace("<KeywordColor>", "");
                        return @CurrentConfess.ShorthandsInsertionParams.InsertionShape.Replace("<KeywordID>", ID).Replace("<KeywordName>", KeywordsInterrogation.Keywords_Bufs[ID].Name).Replace("<KeywordColor>", "");
                    }
                    else
                    {
                        return Match.Groups[0].Value;
                    }
                });
                break;

            case "KeywordIDToTMPro":
                SelectedTextToEdit = LimbusPreviewFormatter.RemoteRegexPatterns.SquareBracketLike.Replace(SelectedTextToEdit, Match =>
                {
                    string ID = Match.Groups["ID"].Value;
                    if (KeywordsInterrogation.Keywords_Bufs.ContainsKey(ID))
                    {
                        string Name = KeywordsInterrogation.Keywords_Bufs[ID].Name;
                        string Color = KeywordsInterrogation.Keywords_Bufs[ID].StringColor;

                        return $"<sprite name=\"{ID}\"><color={Color}><u><link=\"{ID}\">{Name}</link></u></color>";
                    }
                    else return Match.Groups[0].Value;
                });
                break;

            default:

                if (ActualSender.DataContext != null) // @ExtraReplacements.ContextMenu item
                {
                    var RegexReplacements = ActualSender.DataContext as List<@ExtraReplacements.RegexReplaceOption>;

                    foreach (var RegexReplacement in RegexReplacements)
                    {
                        SelectedTextToEdit = RegexReplacement.RegularExpression.Replace(SelectedTextToEdit, RegexReplacement.Replacement);
                    }
                }
                else // Call from Window_PreviewKeyDown() Hotkeys but with invalid name
                {
                    MessageBox.Show($"Unknown context menu command from hotkeys: \"{ContextMenuHotkeys.LatestHotkeyCommandName}\"\n\n(You can only use those listed in THE \"[⇲] Assets Directory\\Context Menu Hotkeys.json\" file)", "Invalid hotkey command", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                break;
        }

        #pragma '<\0TMPSPACE>' to avoid conversion of same keywords within each other when converting Unevident to Shorthands
        // As example: "Повышение силы атаки" (Attack Power Up) with "Повышение силы" (Power Up) inside that being converted too without tmpspace ("[Enhancement:`Повышение силы атаки`]" (First 'Match =>' replace step by AutoKeywordsDetection) -> "[Enhancement:`[ResultEnhancement:`Повышение силы`] атаки`]" (Second and last 'Match =>' replace step by AutoKeywordsDetection))
        if (SelectedTextToEdit != TextEditor.SelectedText) TextEditor.SelectedText = SelectedTextToEdit.Replace("<\0TMPSPACE>", " ");
    }
    #endregion
}