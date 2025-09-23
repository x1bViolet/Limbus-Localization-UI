using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Microsoft.Win32;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Json.BaseTypes;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Passives;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_EGOGifts;
using static LC_Localization_Task_Absolute.Json.Custom_Skills_Constructor;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Json.FilesIntegration;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator.ProjectFile.Sections;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader.InterfaceLocalizationModifiers.Frames.StaticOrDynamic_UI_Text;
using static System.Globalization.NumberStyles;
using static System.Windows.Visibility;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute;

public partial class MainWindow : Window
{
    #region Initials
    public static MainWindow MainControl;
    public static bool ManualTextLoadEvent = false;

    public static Dictionary<string, TMProEmitter> PreviewLayoutControls;

    public static List<TMProEmitter> PreviewLayoutsList = [];
    public static Dictionary<TMProEmitter, double> PrimaryRegisteredFontSizes = [];
    public static List<TextBox> FocusableTextBoxes = [];
    
    public static TMProEmitter PreviewUpdate_TargetSite; // = PreviewLayout_Default by default from InitMain()

    public static FileInfo CurrentFile;
    public static Encoding CurrentFileEncoding = new UTF8Encoding();
    #endregion


    public MainWindow()
    {
        InitializeComponent();

        MainControl = this;
        SettingsWindow.SettingsControl = new SettingsWindow(); // Init settings window

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

    private void InitMain()
    { 
        if (File.Exists(@"[⇲] Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Raw Json\Raw Json $Unpack.zip"))
        {
            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(@"[⇲] Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Raw Json\Raw Json $Unpack.zip", @"[⇲] Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Raw Json");

                //File.Delete(@"[⇲] Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Raw Json\Raw Json $Unpack.zip");
            }
            catch { }
        }

        PreviewUpdate_TargetSite = PreviewLayoutDef_Default;

        File.WriteAllText(@"[⇲] Assets Directory\Latest loading.txt", "");

        Mode_Skills.LoadDefaultResources();

        Configurazione.PullLoad();

        if (File.Exists(@"[⇲] Assets Directory\Default Text.txt"))
        {
            TextEditor.Text = File.ReadAllText(@"[⇲] Assets Directory\Default Text.txt");
        }
        else
        {
            TextEditor.Text = "                      <font=\"BebasKai SDF\"><size=140%><u>Limbus Company Localization Interface</u> <color=#f8c200>'1.2:1</color></size></font>\n\nЧерти вышли из омута";
        }

        {
            InitializeIdentityPreviewCreatorProperties();

            // Switch ui back to regular on startup (i dont want to change xaml back)
            IdentityPreviewCreator_TextEntries_ElementsColor.Text = "#abcdef";

            IdentityPreviewCreator_CautionTypeSelector.SelectedIndex = 0;
            IdentityPreviewCreator_LoadBlankProject();

            SwitchUI_Activate(DisableTopmost: false);

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
    public static bool IsPendingPreviewUpdate = false;
    private void Editor_TextChanged(object RequestSender, EventArgs EventArgs)
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

                    Timer.Tick += (Sender, Args) =>
                    {
                        Timer.Stop();
                        PullUpdatePreview(TextEditor.Text);

                        IsPendingPreviewUpdate = false;
                    };
                }
                catch { IsPendingPreviewUpdate = false; }
            }
        }
        else
        {
            PullUpdatePreview(TextEditor.Text);
        }

        //if (Mode_Handlers.Upstairs.ActiveProperties.Key.Equals("Skills"))
        //{
        //    //////////////////////////////////////////////////////////////////////////////////////////////
        //    UptieLevel FullLink = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel];
        //    //////////////////////////////////////////////////////////////////////////////////////////////
        //    if (FullLink.Coins != null)
        //    {
        //        if (FullLink.Coins.Count > 0)
        //        {
        //            Mode_Skills.CheckSkillNameReplicaCoins_FromLocalizationFile();
        //        }
        //    }
        //}
    }

    public static void PullUpdatePreview(string EditorText)
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
                        UptieLevel FullLink = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel];
                        /////////////////////////////////////////////////////////////////////////////////////////////

                        FullLink.EditorDescription = EditorText.Replace("\r", "");

                        if (!FullLink.Description
                            .Equals(FullLink.EditorDescription))
                        {
                            ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[$"[Skills / Right menu] * Skill main desc"]
                                .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker.Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers[$"[Skills / Right menu] * Skill main desc"].Text);
                        }
                        else
                        {
                            ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[$"[Skills / Right menu] * Skill main desc"]
                                .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers[$"[Skills / Right menu] * Skill main desc"].Text;
                        }
                    }
                    else
                    {
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        CoinDesc FullLink = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Coins[Mode_Skills.CurrentSkillCoinIndex].CoinDescriptions[Mode_Skills.CurrentSkillCoinDescIndex];
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        FullLink.EditorDescription = EditorText.Replace("\r", "");

                        if (!FullLink.Description
                            .Equals(FullLink.EditorDescription))
                        {
                            ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Skills / Right menu] * Skill Coin desc number"]
                                .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker
                                    .Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers["[Skills / Right menu] * Skill Coin desc number"].Text.Extern(Mode_Skills.CurrentSkillCoinDescIndex + 1));
                        }
                        else
                        {
                            ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Skills / Right menu] * Skill Coin desc number"]
                                .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Skills / Right menu] * Skill Coin desc number"].Text
                                    .Extern(Mode_Skills.CurrentSkillCoinDescIndex + 1);
                        }

                        if (DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Coins[Mode_Skills.CurrentSkillCoinIndex].CoinDescriptions
                            .Where(x => x.Description != x.EditorDescription).Any())
                        {
                            ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"]
                                .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker
                                    .Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers[$"[Skills / Right menu] * Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"].Text);
                        }
                        else
                        {
                            ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"]
                                .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers[$"[Skills / Right menu] * Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"].Text;
                        }


                        // Auto hide coin if its empty
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        Coin CoinInfoFullLink = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Coins[Mode_Skills.CurrentSkillCoinIndex];
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        
                        if (CoinInfoFullLink.CoinDescriptions.Where(x => x.EditorDescription.EqualsOneOf("", "<style=\"highlight\"></style>")).Count() == CoinInfoFullLink.CoinDescriptions.Count)
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





            case "Passives":
                if (Mode_Passives.CurrentPassiveID != -1)
                {
                    ///////////////////////////////////////////////////////////////
                    Passive FullLink = DelegatePassives[Mode_Passives.CurrentPassiveID];
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
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Passives / Right menu] * Passive desc"]
                                    .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker
                                        .Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers["[Passives / Right menu] * Passive desc"].Text);
                            }
                            else
                            {
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Passives / Right menu] * Passive desc"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Passives / Right menu] * Passive desc"].Text;
                            }
                            break;

                        case "Summary Description":
                            if (!FullLink.SummaryDescription.Equals(FullLink.EditorSummaryDescription))
                            {
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"]
                                    .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker
                                        .Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers["[Passives / Right menu] * Passive summary"].Text);
                            }
                            else
                            {
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Passives / Right menu] * Passive summary"].Text;
                            }
                            break;
                    }
                }

                break;





            case "Keywords":
                if (!Mode_Keywords.CurrentKeywordID.Equals(""))
                {
                    ///////////////////////////////////////////////////////////////
                    Keyword FullLink = DelegateKeywords[Mode_Keywords.CurrentKeywordID];
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
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword desc"]
                                    .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker
                                        .Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword desc"].Text);
                            }
                            else
                            {
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword desc"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword desc"].Text;
                            }
                            break;

                        case "Summary Description":
                            if (!FullLink.SummaryDescription.Equals(FullLink.EditorSummaryDescription))
                            {
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"]
                                    .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker
                                        .Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword summary"].Text);
                            }
                            else
                            {
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword summary"].Text;
                            }
                            break;
                    }
                }

                break;





            case "E.G.O Gifts":
                if (!Mode_EGOGifts.CurrentEGOGiftID.Equals(-1))
                {
                    ///////////////////////////////////////////////////////////////
                    Type_EGOGifts.EGOGift FullLink = DelegateEGOGifts[Mode_EGOGifts.CurrentEGOGiftID];
                    ///////////////////////////////////////////////////////////////
                    switch (Mode_EGOGifts.TargetSite_StringLine)
                    {
                        case "Main Description":

                            FullLink.EditorDescription = EditorText.Replace("\r", "");

                            if (!FullLink.Description.Equals(FullLink.EditorDescription))
                            {
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"]
                                    .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker
                                        .Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"].Text);
                            }
                            else
                            {
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"].Text;
                            }
                            break;

                        default:

                            string SimpleDescNumber = $"{Mode_EGOGifts.TargetSite_StringLine[^1]}";
                            
                            int TargetSimpleDescIndex = int.Parse(SimpleDescNumber) - 1;

                            FullLink.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription = EditorText.Replace("\r", "");


                            if (!FullLink.SimpleDescriptions[TargetSimpleDescIndex].Description.Equals(FullLink.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription))
                            {
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"]
                                    .RichText = ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesMarker
                                        .Extern(ᐁ_Interface_Localization_Loader.LoadedModifiers[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"].Text);;
                            }
                            else
                            {
                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"].Text;
                            }

                            break;
                    }
                }

                break;

            default: break;
        }

        if (PreviewUpdate_TargetSite != null)
        {
            PreviewUpdate_TargetSite.RichText = EditorText;
        }
    }
    #endregion



    #region Surfacescroll
    public static void InitSurfaceScroll(ScrollViewer Target)
    {
        Target.PreviewMouseLeftButtonDown += SurfaceScroll_MouseLeftButtonDown;
        Target.PreviewMouseMove += SurfaceScroll_MouseMove;
        Target.PreviewMouseLeftButtonUp += SurfaceScroll_MouseLeftButtonUp;
    }
    public static bool SurfaceScroll_isDragging = false;
    public static Point SurfaceScroll_lastMousePosition;
    public static ScrollViewer LastCapturedScrollViewer = null;
    public static void SurfaceScroll_MouseLeftButtonDown(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        if (!MainControl.PreviewLayout_Keywords_Bufs_Name.IsMouseOver)
        {
            LastCapturedScrollViewer = RequestSender as ScrollViewer;
            SurfaceScroll_isDragging = true;
            SurfaceScroll_lastMousePosition = EventArgs.GetPosition(LastCapturedScrollViewer);
            LastCapturedScrollViewer.CaptureMouse();
        }
    }
    public static void SurfaceScroll_MouseMove(object RequestSender, MouseEventArgs EventArgs)
    {
        if (SurfaceScroll_isDragging)
        {
            System.Windows.Point currentPosition = EventArgs.GetPosition(RequestSender as ScrollViewer);
            System.Windows.Vector diff = SurfaceScroll_lastMousePosition - currentPosition;
            (RequestSender as ScrollViewer).ScrollToVerticalOffset((RequestSender as ScrollViewer).VerticalOffset + diff.Y);
            (RequestSender as ScrollViewer).ScrollToHorizontalOffset((RequestSender as ScrollViewer).HorizontalOffset + diff.X);
            SurfaceScroll_lastMousePosition = currentPosition;
        }
    }
    public static void SurfaceScroll_MouseLeftButtonUp(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        SurfaceScroll_isDragging = false;
        (RequestSender as ScrollViewer).ReleaseMouseCapture();
    }
    #endregion

    

    #region Skills switches
    private void Skills_HighlightUptieLevel(object RequestSender, MouseEventArgs EventArgs)
    {
        Border Sender = RequestSender as Border;
        int SenderNumber = int.Parse($"{Sender.Name[^1]}");

        if (Mode_Skills.CurrentSkillUptieLevel != SenderNumber)
        {
            InterfaceObject<Image>($"NavigationPanel_Skills_UptieLevelSwitch_HighlightImage_{SenderNumber}").Visibility = Visible;
        }
    }
    private void Skills_DeHighlightUptieLevel(object RequestSender, MouseEventArgs EventArgs)
    {
        Border Sender = RequestSender as Border;
        int SenderNumber = int.Parse($"{Sender.Name[^1]}");

        if (Mode_Skills.CurrentSkillUptieLevel != SenderNumber)
        {
            InterfaceObject<Image>($"NavigationPanel_Skills_UptieLevelSwitch_HighlightImage_{SenderNumber}").Visibility = Collapsed;
        }
    }


    private void ChangeSkillEGOAbnormalityName(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        /////////////////////////////////////////////////////////////////////////////////////////////
        UptieLevel FullLink = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel];
        /////////////////////////////////////////////////////////////////////////////////////////////

        if (!SWBT_Skills_EGOAbnormalitySkillName.Text.Equals(FullLink.EGOAbnormalityName))
        {
            FullLink.EGOAbnormalityName = SWBT_Skills_EGOAbnormalitySkillName.Text;

            Mode_Skills.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
        }
    }


    private void NavigationPanel_Skills_SwitchToUptieLevel(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        Border Sender = RequestSender as Border;
        string UptieLevelNumber = $"{Sender.Name[^1]}";

        Mode_Skills.TransformToSkill(Mode_Skills.CurrentSkillID, int.Parse(UptieLevelNumber));
    }

    private void SwitchToFifthUptie(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        Mode_Skills.TransformToSkill(Mode_Skills.CurrentSkillID, 5);
    }

    private void Actions_Skills_SwitchToMainDesc(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        Mode_Skills.SwitchToDesc();

        if (DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch)
        {
            FastSwitch_ToSkillMainDesc__Highlight();
        }
    }

    private void Actions_Skills_SetCoinFocus(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        Border Sender = RequestSender as Border;
        string CoinNumber = $"{Sender.Name[^1]}";
        Mode_Handlers.Mode_Skills.SetCoinFocus(int.Parse(CoinNumber));

        if (DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch)
        {
            TMProEmitter HighlightTarget = InterfaceObject<TMProEmitter>($"PreviewLayout_Skills_Coin{Mode_Skills.CurrentSkillCoinIndex + 1}_Desc{Mode_Skills.CurrentSkillCoinDescIndex + 1}");
            FastSwitch_ToSkillCoinDesc__Highlight(HighlightTarget);
            HighlightTarget.Focus();
        }
    }

    private void NavigationPanel_Skills_SwitchToCoinDesc(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        Border Sender = RequestSender as Border;

        string Direction = Sender.Name.Split("NavigationPanel_Skills_CoinDesc_")[^1];
        int IndexOfCurrentCoinDesc = Mode_Skills.CurrentCoinDescs_Avalible.IndexOf(Mode_Skills.CurrentSkillCoinDescIndex);
        int TargetSwitchIndex = Direction.Equals("Next") ? IndexOfCurrentCoinDesc + 1 : IndexOfCurrentCoinDesc - 1;

        Mode_Skills.SwitchToCoinDesc(TargetSwitchIndex);

        if (DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnManualSwitch)
        {
            TMProEmitter HighlightTarget = InterfaceObject<TMProEmitter>($"PreviewLayout_Skills_Coin{Mode_Skills.CurrentSkillCoinIndex + 1}_Desc{Mode_Skills.CurrentSkillCoinDescIndex + 1}");
            FastSwitch_ToSkillCoinDesc__Highlight(HighlightTarget);
            HighlightTarget.Focus();
        }
    }

    public static void NavigationPanel_Skills_SwitchToCoinDesc_CheckAvalibles()
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



    private void FastSwitch_ToSkillCoinDesc(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        if (!CustomIdentityPreviewCreator.IsActive)
        {
            TMProEmitter Sender = RequestSender as TMProEmitter;

            string CoinNumber = $"{Sender.Name.Split("PreviewLayout_Skills_Coin")[1][0]}";
            string CoinDescNumber = $"{Sender.Name.Split("_Desc")[1]}";

            Mode_Skills.SetCoinFocus(int.Parse(CoinNumber));
            Mode_Skills.SwitchToCoinDesc(int.Parse(CoinDescNumber) - 1);

            if (DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnRightClick)
            {
                FastSwitch_ToSkillCoinDesc__Highlight(Sender);
            }
        }
    }

    private void FastSwitch_ToSkillCoinDesc__Highlight(TMProEmitter TargetDesc)
    {
        TargetDesc.Background = ToSolidColorBrush("#FF262626");

        TargetDesc.Background.BeginAnimation(SolidColorBrush.OpacityProperty, new DoubleAnimation()
        {
            From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(0.4))
        });
    }

    private void FastSwitch_ToSkillMainDesc(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        Actions_Skills_SwitchToMainDesc(STE_Action_Skills_MainDescription, null);

        if (DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightSkillDescsOnRightClick)
        {
            FastSwitch_ToSkillMainDesc__Highlight();
        }
    }

    private void FastSwitch_ToSkillMainDesc__Highlight()
    {
        PreviewLayout_Skills_MainDesc.Background = ToSolidColorBrush("#FF262626");

        PreviewLayout_Skills_MainDesc.Background.BeginAnimation(SolidColorBrush.OpacityProperty, new DoubleAnimation()
        {
            From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(0.4))
        });
    }
    #endregion



    #region Passive switches
    private void Actions_Passives_SwitchToMainDesc(object RequestSender, MouseButtonEventArgs EventArgs) => Mode_Passives.SwitchToMainDesc();
    private void Actions_Passives_SwitchToSummaryDesc(object RequestSender, MouseButtonEventArgs EventArgs) => Mode_Passives.SwitchToSummaryDesc();
    private void Passives_CreateSummaryDescription(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        CanSwitchID = false;

        DelegatePassives[Mode_Passives.CurrentPassiveID].SummaryDescription = "\0";
        DelegatePassives[Mode_Passives.CurrentPassiveID].EditorSummaryDescription = "";

        Mode_Passives.TargetSite_StringLine = "Summary Description";

        TextEditor.Text = DelegatePassives[Mode_Passives.CurrentPassiveID].EditorSummaryDescription;

        STE_DisableCover_Passives_SummaryDescription.Background = ToSolidColorBrush("#00000000");
        ColorAnimation DisableCoverFadeout = new ColorAnimation()
        {
            From = ToColorBrush(Resource<SolidColorBrush>("Theme_ElongatedButtons__DisabledButtonsShadowing").ToString()),
            To = ToColorBrush("#00000000"),
            Duration = new Duration(TimeSpan.FromSeconds(0.38))
        };
        DisableCoverFadeout.Completed += (s, e) =>
        {
            STE_DisableCover_Passives_SummaryDescription.Visibility = Collapsed;
            STE_DisableCover_Passives_SummaryDescription.Background = Resource<SolidColorBrush>("Theme_ElongatedButtons__DisabledButtonsShadowing");

            CanSwitchID = true;
        };
        STE_DisableCover_Passives_SummaryDescription.Background.BeginAnimation(SolidColorBrush.ColorProperty, DisableCoverFadeout);
    }
    #endregion



    #region Keywords switches
    private void Actions_Keywords_SwitchToMainDesc(object RequestSender, MouseButtonEventArgs EventArgs) => Mode_Keywords.SwitchToMainDesc();
    private void Actions_Keywords_SwitchToSummaryDesc(object RequestSender, MouseButtonEventArgs EventArgs) => Mode_Keywords.SwitchToSummaryDesc();
    private void Actions_Keywords_ToggleFormatInsertions(object RequestSender, MouseButtonEventArgs EventArgs)
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

    #region Bufs interactive button
    private void Actions_Keywords_InteractiveBufsButton_Highlight(object RequestSender, MouseEventArgs EventArgs)
    {
        Actions_Keywords_InteractiveBufsButton_Image.BeginAnimation(Image.OpacityProperty, new DoubleAnimation()
        {
            From = 0, To = 1, Duration = TimeSpan.FromSeconds(0.265)
        });
    }

    private void Actions_Keywords_InteractiveBufsButton_StopHighlight(object RequestSender, MouseEventArgs EventArgs)
    {
        Actions_Keywords_InteractiveBufsButton_Image.BeginAnimation(Image.OpacityProperty, new DoubleAnimation()
        {
            From = 1, To = 0, Duration = TimeSpan.FromSeconds(0.120)
        });
        Keywords_InteractiveBufsButton_Scale.ScaleX = 1;
        Keywords_InteractiveBufsButton_Scale.ScaleY = 1;
    }

    private void Actions_Keywords_InteractiveBufsButtonDown(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        Keywords_InteractiveBufsButton_Scale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation()
        {
            From = 1, To = 0.98, Duration = TimeSpan.FromSeconds(0.1)
        });
        Keywords_InteractiveBufsButton_Scale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation()
        {
            From = 1, To = 0.98, Duration = TimeSpan.FromSeconds(0.1)
        });
    }
    private void Actions_Keywords_InteractiveBufsButtonUp(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        Keywords_InteractiveBufsButton_Scale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation()
        {
            From = 0.98, To = 1, Duration = TimeSpan.FromSeconds(0.1)
        });
        Keywords_InteractiveBufsButton_Scale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation()
        {
            From = 0.98, To = 1, Duration = TimeSpan.FromSeconds(0.1)
        });

        //////////////////////////////////////////////////////////////////////
        Keyword FullLinkKeyword = DelegateKeywords[Mode_Keywords.CurrentKeywordID];
        //////////////////////////////////////////////////////////////////////
        bool AnyChanges = false;
        if (!FullLinkKeyword.Description.Equals(FullLinkKeyword.EditorDescription))
        {
            FullLinkKeyword.Description = FullLinkKeyword.EditorDescription;

            ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword desc"]
                .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword desc"].Text;

            AnyChanges = true;
        }

        if (FullLinkKeyword.SummaryDescription != null)
        {
            if (!FullLinkKeyword.SummaryDescription.Equals(FullLinkKeyword.EditorSummaryDescription))
            {
                FullLinkKeyword.SummaryDescription = FullLinkKeyword.EditorSummaryDescription;

                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"]
                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword summary"].Text;

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
    #endregion

    #endregion



    #region E.G.O Gifts switches
    private void PreviewLayout_Keywords_Bufs_Name_TextChanged(object RequestSender, TextChangedEventArgs EventArgs)
    {
        SWBT_Keywords_KeywordName.Text = PreviewLayout_Keywords_Bufs_Name.Text;
    }

    private void EGOGiftDisplay_HotSwitchToUpgradeLevel_MouseEnter(object RequestSender, MouseEventArgs EventArgs)
    {
        (RequestSender as Grid).Children[2].Opacity = 1;
    }

    private void EGOGiftDisplay_HotSwitchToUpgradeLevel_MouseLeave(object RequestSender, MouseEventArgs EventArgs)
    {
        (RequestSender as Grid).Children[2].Opacity = 0;
    }

    private void EGOGiftDisplay_HotSwitchToUpgradeLevel_SwitchButton(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        int TargetUpgradeLevel = int.Parse($"{(RequestSender as Grid).Name[^1]}") - 1;

        if (DelegateEGOGifts[Mode_EGOGifts.CurrentEGOGiftID].UpgradeLevelsAssociativeIDs.Count > 0)
        {
            Mode_EGOGifts.TransformToEGOGift(DelegateEGOGifts[Mode_EGOGifts.CurrentEGOGiftID].UpgradeLevelsAssociativeIDs[TargetUpgradeLevel]);
        }
    }

    private void Actions_EGOGifts_SwitchToMainDesc(object RequestSender, MouseButtonEventArgs EventArgs) => Mode_EGOGifts.SwitchToMainDesc();
    private void Actions_EGOGifts_SwitchToSimpleDesc(object RequestSender, MouseButtonEventArgs EventArgs) => Mode_EGOGifts.SwitchToSimpleDesc($"{(RequestSender as Border).Name[^1]}");
    #endregion



    #region ID Switch system
    public static bool SwitchToFirstItem = false;
    public static bool SwitchToLastItem = false;
    public static bool CanSwitchID = true;
    private void NavigationPanel_IDSwitch(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        if (CanSwitchID)
        {
            Border Sender = RequestSender as Border;
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

    public static void NavigationPanel_IDSwitch_CheckAvalibles()
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

    private void NavigationPanel_IDSwitch_ToLast(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        SwitchToLastItem = true;
        NavigationPanel_IDSwitch(RequestSender, EventArgs);
        SwitchToLastItem = false;
    }

    private void NavigationPanel_IDSwitch_ToFirst(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        SwitchToFirstItem = true;
        NavigationPanel_IDSwitch(RequestSender, EventArgs);
        SwitchToFirstItem = false;
    }

    /// <summary>
    /// -> action in 'Manual ID Switch' region
    /// </summary>
    private void NavigationPanel_IDSwitch_ManualInput_Start(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        string IDText = STE_NavigationPanel_ObjectID_Display.CurrentRichText;
        if (!IDText.Equals(ᐁ_Interface_Localization_Loader.SpecializedDefs.InsertionsDefaultValue))
        {
            STE_NavigationPanel_ObjectID_Display.Visibility = Collapsed;
            STE_NavigationPanel_ObjectID_Display_IDCopied.Visibility = Collapsed;

            NavigationPanel_IDSwitch_ManualInput_Textfield.CaretIndex = NavigationPanel_IDSwitch_ManualInput_Textfield.Text.Length;
            NavigationPanel_IDSwitch_ManualInput_Textfield.Visibility = Visible;

            NavigationPanel_IDSwitch_ManualInput_Textfield.Focus();
        }
    }

    public static void NavigationPanel_IDSwitch_ManualInput_Stop()
    {
        MainControl.STE_NavigationPanel_ObjectID_Display.Visibility = Visible;
        MainControl.STE_NavigationPanel_ObjectID_Display_IDCopied.Visibility = Visible;
        MainControl.NavigationPanel_IDSwitch_ManualInput_Textfield.Visibility = Collapsed;

        UnfocusElement(MainControl.NavigationPanel_IDSwitch_ManualInput_Textfield);

        MainControl.NavigationPanel_IDSwitch_ManualInput_Textfield.Text = "";
    }

    bool AlreadyAnimatingCopiedInfo = false;
    private void CopyID(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        if (!NavigationPanel_IDSwitch_ManualInput_Textfield.IsFocused)
        {
            try
            {
                string IDText = STE_NavigationPanel_ObjectID_Display.CurrentRichText;
                if (!IDText.Equals(ᐁ_Interface_Localization_Loader.SpecializedDefs.InsertionsDefaultValue))
                {
                    Clipboard.SetDataObject(STE_NavigationPanel_ObjectID_Display.CurrentRichText as string);

                    double FadeInOutTime = 0.118;
                    double PauseTime = 0.35;

                    if (!AlreadyAnimatingCopiedInfo)
                    {
                        AlreadyAnimatingCopiedInfo = true;
                        CanSwitchID = false;

                        DoubleAnimation FadeIn_ID = new DoubleAnimation()
                        {
                            From = 0, To = 1, Duration = new Duration(TimeSpan.FromSeconds(FadeInOutTime))
                        };
                        // Finish
                        FadeIn_ID.Completed += (s, e) =>
                        {
                            AlreadyAnimatingCopiedInfo = false;
                        };

                        DoubleAnimation FadeOut_InfoText = new DoubleAnimation()
                        {
                            From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(FadeInOutTime))
                        };
                        FadeOut_InfoText.Completed += (s, e) =>
                        {
                            STE_NavigationPanel_ObjectID_Display.BeginAnimation(FrameworkElement.OpacityProperty, FadeIn_ID);
                        };

                        DoubleAnimation FadeIn_InfoText = new DoubleAnimation()
                        {
                            From = 0, To = 1, Duration = new Duration(TimeSpan.FromSeconds(FadeInOutTime))
                        };
                        FadeIn_InfoText.Completed += (s, e) =>
                        {
                            Await(PauseTime, () =>
                            {
                                CanSwitchID = true;
                                STE_NavigationPanel_ObjectID_Display_IDCopied.BeginAnimation(FrameworkElement.OpacityProperty, FadeOut_InfoText);
                            });
                        };

                        DoubleAnimation FadeOut_ID = new DoubleAnimation()
                        {
                            From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(FadeInOutTime))
                        };
                        FadeOut_ID.Completed += (s, e) =>
                        {
                            STE_NavigationPanel_ObjectID_Display_IDCopied.BeginAnimation(FrameworkElement.OpacityProperty, FadeIn_InfoText);
                        };
                        // Timeline goes backwards

                        STE_NavigationPanel_ObjectID_Display.BeginAnimation(FrameworkElement.OpacityProperty, FadeOut_ID);
                    }
                }
            }
            catch { }
        }
    }
    #endregion



    #region Shared
    private void ChangeObjectName(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
        {
            case "Skills":

                /////////////////////////////////////////////////////////////////////////////////////////////
                UptieLevel FullLinkSkills = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel];
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
                Passive FullLinkPassives = DelegatePassives[Mode_Passives.CurrentPassiveID];
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
                Keyword FullLinkKeywords = DelegateKeywords[Mode_Keywords.CurrentKeywordID];
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
                EGOGift FullLinkEGOGifts = DelegateEGOGifts[Mode_EGOGifts.CurrentEGOGiftID];
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

    public static void FocusOnFile(FileInfo Target)
    {
        CurrentFile = Target;
        CurrentFileEncoding = Target.GetFileEncoding();
        MainControl.JsonFilePath.Text = CurrentFile.FullName;
        MainControl.JsonFilePath.ScrollToHorizontalOffset(10000) ;
    }

    public void Actions_FILE_SelectFile_Acutal()
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

            string? PredefinedFileTypeNameChanger = TryAcquireManualFileType(TemplateTarget.FullName);
            if (PredefinedFileTypeNameChanger != null)
            {
                CheckName = PredefinedFileTypeNameChanger;
            }

            if (CheckName.StartsWith("Skills"))
            {
                FocusOnFile(TemplateTarget);

                Mode_Skills.LoadStructure(
                    JsonFile: TemplateTarget,
                    EnableUptieLevels: CheckName.ContainsOneOf("Skills_Ego_Personality-", "Skills_personality-", "Skills.json", "Skills_Ego.json", "Skills_Assist.json"),
                    EnableEGOAbnormalityName: CheckName.ContainsOneOf("Skills_Ego_Personality-", "Skills_Ego.json")
                );
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

            SyntaxedTextEditor.RecompileEditorSyntax();
        }
    }

    private void Actions_FILE_SelectFile(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        CheckUnsavedChanges(SelectFileOnEnd: true);
    }
    #endregion



    #region Technical
    public static Assembly LCLocalizationTaskAbsolute = Assembly.GetExecutingAssembly();
    public static string LoadFromEmbeddedResources(string FullName)
    {
        using (Stream stream = LCLocalizationTaskAbsolute.GetManifestResourceStream(FullName))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    

    private void Window_SizeChanged(object RequestSender, SizeChangedEventArgs EventArgs)
    {
        if ((Width <= Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinWidth + 2 & Height <= Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinHeight + 2)
            | (((ᐁ_Interface_Themes_Loader.LoadedTheme != null ? ᐁ_Interface_Themes_Loader.LoadedTheme.Common.HideBackgroundImageWithMinimumWindowWidth : false) & Width <= Mode_Handlers.Upstairs.ActiveProperties.DefaultValues.MinWidth + 2)))
        {
            BackgroundImage.Visibility = Visibility.Collapsed;
        }
        else
        {
            BackgroundImage.Visibility = Visibility.Visible;
        }
    }


    public void CheckUnsavedChanges(bool ExitOnEnd = false, bool SelectFileOnEnd = false)
    {
        int UnsavedChangesCount = 0;
        string UnsavedChangesInfo = "";

        switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
        {
            case "Passives":
                foreach (KeyValuePair<int, Type_Passives.Passive> CheckPassive in DelegatePassives)
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
                        UnsavedChangesInfo += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.Passives.IDHeader.Exform(CheckPassive.Key, CheckPassive.Value.Name);
                        if (UnsavedChangesInPassiveDesc)
                        {
                            UnsavedChangesInfo += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.Passives.MainDesc;
                        }
                        if (UnsavedChangesInPassiveSummary)
                        {
                            UnsavedChangesInfo += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.Passives.SummaryDesc;
                        }
                    }
                }

                break;


            case "Skills":
                foreach (KeyValuePair<int, Dictionary<int, Type_Skills.UptieLevel>> CheckSkill in DelegateSkills)
                {
                    bool AlreadyAddedThisID = false;
                    string SkillName = "";
                    foreach (Type_Skills.UptieLevel UptieLevel in CheckSkill.Value.Values)
                    {
                        SkillName = UptieLevel.Name;
                        bool AnythingChanged = false;
                        
                        if (!UptieLevel.Description.Equals(UptieLevel.EditorDescription))
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
                                        if (CoinDesc != null && CoinDesc.Description != null)
                                        {
                                            if (!CoinDesc.Description.Equals(CoinDesc.EditorDescription)) AnythingChanged = true;
                                        }
                                    }
                                }
                            }
                        }


                        if (AnythingChanged)
                        {
                            if (!AlreadyAddedThisID)
                            {
                                UnsavedChangesInfo += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.Skills.IDHeader.Exform(CheckSkill.Key, SkillName);
                                AlreadyAddedThisID = true;
                            }
                            UnsavedChangesInfo += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.Skills.UptieLevel.Extern($"{UptieLevel.Uptie}");
                        }
                    }
                }
                break;


            case "Keywords":
                foreach (KeyValuePair<string, Type_Keywords.Keyword> CheckKeyword in DelegateKeywords)
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
                        UnsavedChangesInfo += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.Keywords.IDHeader.Exform(CheckKeyword.Key, CheckKeyword.Value.Name);
                        if (UnsavedChangesInKeywordDesc)
                        {
                            UnsavedChangesInfo += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.Keywords.MainDesc;
                        }
                        if (UnsavedChangesInKeywordSummary)
                        {
                            UnsavedChangesInfo += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.Passives.MainDesc;
                        }
                    }
                }
                break;


            case "E.G.O Gifts":
                if (DelegateEGOGifts.Keys.Count != 0)
                {
                    foreach (KeyValuePair<int, Type_EGOGifts.EGOGift> CheckEGOGift in DelegateEGOGifts)
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
                            foreach (Type_EGOGifts.SimpleDescription SimpleDesc in CheckEGOGift.Value.SimpleDescriptions)
                            {
                                if (!SimpleDesc.Description.Equals(SimpleDesc.EditorDescription))
                                {
                                    ChangedSimpleDescs += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.EGOGifts.SimpleDesc.Extern(SimpleDescIndexer);
                                }

                                SimpleDescIndexer++;
                            }
                        }

                        if (ChangedDesc | !ChangedSimpleDescs.Equals(""))
                        {
                            UnsavedChangesInfo += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.EGOGifts.IDHeader.Exform(CheckEGOGift.Key, CheckEGOGift.Value.Name);
                            if (ChangedDesc)
                            {
                                UnsavedChangesInfo += ᐁ_Interface_Localization_Loader.SpecializedDefs.UnsavedChangesInfo.EGOGifts.MainDesc;
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
            DTE_UnsavedChangesInfo.RichText = UnsavedChangesInfo.Trim();
            BackgroundCover_UnsavedChanges.Visibility = Visible;
            if (SelectFileOnEnd) SelectFileInstead = true;
        }
        else
        {
            if (ExitOnEnd) Application.Current.Shutdown();
            if (SelectFileOnEnd) Actions_FILE_SelectFile_Acutal();
        }
    }
    public bool SelectFileInstead = false;

    private bool CanDragMove = true;
    private void Window_DragMove(object RequestSender, MouseButtonEventArgs EventArgs)
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
                    Rect WorkArea = SystemParameters.WorkArea;
                    this.Left = WorkArea.Left;
                    this.Top = WorkArea.Top;
                    this.Width = WorkArea.Width;
                    this.Height = WorkArea.Height;
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
    private void Minimize(object RequestSender, MouseButtonEventArgs EventArgs) => WindowState = WindowState.Minimized;
    private void Shutdown(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        CheckUnsavedChanges(ExitOnEnd: true);
    }
    private void UnsavedChangesDialog_ConfirmProceed(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        if (SelectFileInstead)
        {
            Actions_FILE_SelectFile_Acutal();
            BackgroundCover_UnsavedChanges.Visibility = Collapsed;
            SelectFileInstead = false;
        }
        else Application.Current.Shutdown();
    }

    private void UnsavedChangesDialog_Cancel(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        BackgroundCover_UnsavedChanges.Visibility = Collapsed;
    }



    private void ProcessLogicalTree(object Current)
    {
        if (Current is FrameworkElement)
        {
            // Setup line height on all tmproemitters based on type

            string ElementName = (Current as FrameworkElement).Name;
            if (ElementName.StartsWith("PreviewLayout_") && Current is TMProEmitter LimbusPreviewLayout)
            {
                PreviewLayoutsList.Add(LimbusPreviewLayout);
                PrimaryRegisteredFontSizes[LimbusPreviewLayout] = LimbusPreviewLayout.FontSize;

                if (ElementName.Equals("PreviewLayout_EGOGifts"))
                {
                    LimbusPreviewLayout.LineHeight = 24.9;
                }
                else
                {
                    LimbusPreviewLayout.LineHeight = 27.0;
                }
            }
            else if (ElementName.StartsWith("Special_PreviewLayout_") && Current is TMProEmitter LimbusPreviewLayout2)
            {
                PreviewLayoutsList.Add(LimbusPreviewLayout2);
                PrimaryRegisteredFontSizes[LimbusPreviewLayout2] = LimbusPreviewLayout2.FontSize;

                if (ElementName.Equals("Special_PreviewLayout_Keywords_BattleKeywords_Desc"))
                {
                    LimbusPreviewLayout2.LineHeight = 25.0;
                }
                else
                {
                    LimbusPreviewLayout2.LineHeight = 20.0;
                }
            }
            else if (!ElementName.StartsWith("SeriousScrollViewer") & Current is ScrollViewer)
            {
                try
                {
                    (Current as ScrollViewer).Resources.Add(SystemParameters.VerticalScrollBarWidthKey, 0.0);
                }
                catch { } // DISABLE SCROLLBAR
            }
            else if (Current is TextBox | Current is UITranslation_Mint)
            {
                FocusableTextBoxes.Add(Current as TextBox);
            }
        }

        DependencyObject DependencyObjectParent = Current as DependencyObject;

        if (DependencyObjectParent != null)
        {
            foreach (object Child in LogicalTreeHelper.GetChildren(DependencyObjectParent))
            {
                ProcessLogicalTree(Child);
            }
        }
    }

    // Auto hide shadow text for textfields
    private void SWBT_TextChanged_Shared(object RequestSender, TextChangedEventArgs EventArgs)
    {
        TextBox Sender = RequestSender as TextBox;
        Grid TagetSite = Sender.Parent as Grid;
        FrameworkElement ShadowText = TagetSite.Children[0] as FrameworkElement;

        ShadowText.Visibility = Sender.Text switch
        {
            "" => Visible,
            _  => Collapsed,
        };

        if (Sender.Name.Equals("SWBT_Skills_MainSkillName"))
        {
            //SkillNameReplica.Text = SWBT_Skills_MainSkillName.Text;
            SkillNamesReplication_SkillName_Text.Text = SWBT_Skills_MainSkillName.Text;
        }

        if (Sender.Name.Contains("Keywords_FormatInsertion"))
        {
            string FormatInsertionNumber = Sender.Name.Split("Keywords_FormatInsertion_")[^1];

            LimbusPreviewFormatter.FormatInsertionsReplaceValues[FormatInsertionNumber] = Sender.Text.Equals("") ? $"{{{FormatInsertionNumber}}}" : Sender.Text;
            PullUpdatePreview(TextEditor.Text);
        }

        if (Sender.Name.Equals("SWBT_Keywords_KeywordName") & !PreviewLayout_Keywords_Bufs_Name.IsFocused)
        {
            PreviewLayout_Keywords_Bufs_Name.Text = SWBT_Keywords_KeywordName.Text;
            PreviewLayout_Keywords_BattleKeywords_Name.Text = SWBT_Keywords_KeywordName.Text;
        }

        if (Sender.Name.Equals("SWBT_EGOGifts_EGOGiftName")) EGOGiftName_PreviewLayout.Text = SWBT_EGOGifts_EGOGiftName.Text;
    }

    #region Mouse and Keyboard shortcuts
    public static bool IsAnyTextBoxFocused()
    {
        if (FocusableTextBoxes.Where(textbox => textbox.IsFocused == true).Any()) return true;
        else return false;
    }
    public static void UnfocusAllTextBoxes()
    {
        IEnumerable<TextBox> FocusedTextBoxes = FocusableTextBoxes.Where(textbox => textbox.IsFocused == true);
        UnfocusElement(MainControl.TextEditor.TextArea);
        if (FocusedTextBoxes.Any())
        {
            foreach (TextBox FocusedTextBox in FocusedTextBoxes) UnfocusElement(FocusedTextBox);
        }
    }

    private static bool IsCtrlSPressed = false;
    private void Window_PreviewKeyDown(object RequestSender, KeyEventArgs EventArgs)
    {
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
            if (EventArgs.Key == Key.S && !IsCtrlSPressed)
            {
                IsCtrlSPressed = true;

                if (PreviewUpdate_TargetSite != PreviewLayoutDef_Default)
                {
                    switch (Mode_Handlers.Upstairs.ActiveProperties.Key)
                    {
                        case "Skills":

                            if (PreviewUpdate_TargetSite.Equals(PreviewLayout_Skills_MainDesc))
                            {
                                DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Description = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].EditorDescription;

                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Skills / Right menu] * Skill main desc"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Skills / Right menu] * Skill main desc"].Text;

                                Mode_Skills.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }
                            else
                            {
                                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                List<CoinDesc> FullLinkSkill = DelegateSkills[Mode_Skills.CurrentSkillID][Mode_Skills.CurrentSkillUptieLevel].Coins[Mode_Skills.CurrentSkillCoinIndex].CoinDescriptions;
                                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                                FullLinkSkill[Mode_Skills.CurrentSkillCoinDescIndex].Description = FullLinkSkill[Mode_Skills.CurrentSkillCoinDescIndex].EditorDescription;

                                if (!FullLinkSkill.Where(x => !x.Description.Equals(x.EditorDescription)).Any())
                                {
                                    ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[$"[Skills / Right menu] * Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"]
                                        .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers[$"[Skills / Right menu] * Skill Coin {Mode_Skills.CurrentSkillCoinIndex + 1}"].Text;
                                }

                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Skills / Right menu] * Skill Coin desc number"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Skills / Right menu] * Skill Coin desc number"].Text
                                        .Extern(Mode_Skills.CurrentSkillCoinDescIndex + 1);

                                Mode_Skills.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }

                            break;


                        case "Passives":

                            //////////////////////////////////////////////////////////////////////
                            Passive FullLinkPassive = DelegatePassives[Mode_Passives.CurrentPassiveID];
                            //////////////////////////////////////////////////////////////////////

                            if (Mode_Passives.TargetSite_StringLine.Equals("Main Description"))
                            {
                                FullLinkPassive.Description = FullLinkPassive.EditorDescription;

                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Passives / Right menu] * Passive desc"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Passives / Right menu] * Passive desc"].Text;

                                Mode_Passives.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }
                            else if (Mode_Passives.TargetSite_StringLine.Equals("Summary Description"))
                            {
                                FullLinkPassive.SummaryDescription = FullLinkPassive.EditorSummaryDescription;

                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Passives / Right menu] * Passive summary"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Passives / Right menu] * Passive summary"].Text;

                                Mode_Passives.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }

                            break;


                        case "Keywords":
                            //////////////////////////////////////////////////////////////////////
                            Keyword FullLinkKeyword = DelegateKeywords[Mode_Keywords.CurrentKeywordID];
                            //////////////////////////////////////////////////////////////////////

                            if (Mode_Keywords.TargetSite_StringLine.Equals("Main Description"))
                            {
                                FullLinkKeyword.Description = FullLinkKeyword.EditorDescription;

                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword desc"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword desc"].Text;


                                Mode_Keywords.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }
                            else if (Mode_Keywords.TargetSite_StringLine.Equals("Summary Description"))
                            {
                                FullLinkKeyword.SummaryDescription = FullLinkKeyword.EditorSummaryDescription;

                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[Keywords / Right Menu] * Keyword summary"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[Keywords / Right Menu] * Keyword summary"].Text;

                                Mode_Keywords.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }

                            break;


                        case "E.G.O Gifts":
                            //////////////////////////////////////////////////////////////////////
                            EGOGift FullLinkEGOGift = DelegateEGOGifts[Mode_EGOGifts.CurrentEGOGiftID];
                            //////////////////////////////////////////////////////////////////////

                            if (Mode_EGOGifts.TargetSite_StringLine.Equals("Main Description"))
                            {
                                FullLinkEGOGift.Description = FullLinkEGOGift.EditorDescription;

                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"].Text;

                                Mode_EGOGifts.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }
                            else
                            {
                                string SimpleDescNumber = $"{Mode_EGOGifts.TargetSite_StringLine[^1]}";

                                int TargetSimpleDescIndex = int.Parse(SimpleDescNumber) - 1;

                                FullLinkEGOGift.SimpleDescriptions[TargetSimpleDescIndex].Description = FullLinkEGOGift.SimpleDescriptions[TargetSimpleDescIndex].EditorDescription;

                                ᐁ_Interface_Localization_Loader.PresentedStaticTextEntries[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"]
                                    .RichText = ᐁ_Interface_Localization_Loader.LoadedModifiers[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"].Text;


                                Mode_EGOGifts.DeserializedInfo.SerializeFormatted(CurrentFile.FullName);
                            }

                            break;
                    }
                }
            }
        }
        #endregion

        else if (EventArgs.Key == Key.Left | EventArgs.Key == Key.Right)
        {
            if (EventArgs.Key == Key.Right)
            {
                if (!IsAnyTextBoxFocused() & !TextEditor.TextArea.IsFocused) NavigationPanel_IDSwitch(NavigationPanel_IDSwitch_Next, null);
            }
            else if (EventArgs.Key == Key.Left)
            {
                if (!IsAnyTextBoxFocused() & !TextEditor.TextArea.IsFocused) NavigationPanel_IDSwitch(NavigationPanel_IDSwitch_Previous, null);
            }
        }
        else if (EventArgs.Key == Key.Escape)
        {
            if (NavigationPanel_IDSwitch_ManualInput_Textfield.IsFocused)
            {
                NavigationPanel_IDSwitch_ManualInput_Stop();
            }
            UnfocusAllTextBoxes();
        }

        #region Manual ID Switch
        else if (EventArgs.Key == Key.Enter)
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

    private void MakeLimbusPreviewScan_Do(object RequestSender, MouseButtonEventArgs EventArgs)
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

    private void Window_PreviewKeyUp(object RequestSender, KeyEventArgs EventArgs)
    {
        if (EventArgs.Key == Key.S) IsCtrlSPressed = false;
    }

    private new async void MouseDownEvent(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        switch (EventArgs.ChangedButton)
        {
            case MouseButton.XButton1: //Back
                PreviewLayout_Keywords_Bufs_Name.Focusable = false;

                if (Mode_Handlers.Upstairs.ActiveProperties.Key.Equals("Skills") & Keyboard.IsKeyDown(Key.LeftShift))
                {
                    int currentuptie = Mode_Skills.CurrentSkillUptieLevel;
                    List<int> avalible = new List<int>();
                    foreach (KeyValuePair<int, Type_Skills.UptieLevel> Skill in DelegateSkills[Mode_Skills.CurrentSkillID])
                    {
                        avalible.Add(Skill.Value.Uptie);
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
                    List<int> avalible = new List<int>();
                    foreach (KeyValuePair<int, Type_Skills.UptieLevel> Skill in DelegateSkills[Mode_Skills.CurrentSkillID])
                    {
                        avalible.Add(Skill.Value.Uptie);
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

    public static void UnfocusElement(DependencyObject Target)
    {
        FocusManager.SetFocusedElement(FocusManager.GetFocusScope(Target), null);
        Keyboard.ClearFocus();
        MainControl.Focus();
    }

    public static void LockEditorUndo()
    {
        MainControl.TextEditor.Document.UndoStack.ClearAll();
    }
    #endregion



    #region Editor context Menu
    private void Actions_ContextMenu_Shared(object RequestSender, RoutedEventArgs EventArgs)
    {
        string Editor_SelectedTextTemplate = TextEditor.SelectedText;
        
        switch ((RequestSender as MenuItem).Name.Split("ContextMenuItem_")[^1])
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
                foreach (KeyValuePair<string, string> UnevidentKeyword in KeywordsInterrogate.Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter)
                {
                    if (TextEditor.Text.Contains(UnevidentKeyword.Key))
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
                Editor_SelectedTextTemplate = LimbusPreviewFormatter.RemoteRegexPatterns.SquareBracketLike.Replace(Editor_SelectedTextTemplate, Match =>
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
                Editor_SelectedTextTemplate = LimbusPreviewFormatter.RemoteRegexPatterns.SquareBracketLike.Replace(Editor_SelectedTextTemplate, Match =>
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
        if (!Editor_SelectedTextTemplate.Equals(TextEditor.SelectedText)) TextEditor.SelectedText = Editor_SelectedTextTemplate.Replace("<\0TMPSPACE>", " ");
    }

    #endregion

    private void Window_Loaded(object RequestSender, RoutedEventArgs EventArgs)
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

    public static void ReloadConfig_Direct()
    {
        Configurazione.PullLoad();
    }
    
    async private void OpenSettings(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        if (!SettingsWindow.SettingsControl.IsActive)
        {
            await Task.Delay(50);
            SettingsWindow.SettingsControl.Show();
            SettingsWindow.SettingsControl.Focus();
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
            else CurrentTarget = null;
        }
        else
        {
            switch (Upstairs.ActiveProperties.Key)
            {
                case "Skills":
                    CurrentTarget = SurfaceScrollPreview_Skills;
                    NameHint = $"{CurrentFile.Name.Replace(".json", "")}, " +
                               $"ID {Mode_Skills.CurrentSkillID}" +
                               (CurrentFile.Name.Contains("personality", comparisonType: StringComparison.OrdinalIgnoreCase) ? $", Uptie {Mode_Skills.CurrentSkillUptieLevel}" : "");
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
            if (!Directory.Exists(@"[⇲] Assets Directory\[⇲] Scans"))
            {
                Directory.CreateDirectory(@"[⇲] Assets Directory\[⇲] Scans");
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
    private void IdentityPreviewCreator_ToggleSectionVisibilityMaster(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        Grid Target = ((RequestSender as UITranslation_Rose).Parent as StackPanel).Children[1] as Grid;
        Target.Visibility = Target.Visibility == Visible ? Collapsed : Visible;
    }




    private static Random random = new Random();

    private static string RandomString(int length)
    {
        return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private List<string> DefinedUIDsForColumnItems = new List<string>();
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

                    FontSize = SpecializedDefs.DefaultFontSizeForPlaceholder,
                    FontFamily = SpecializedDefs.DefaultFontFamilyForPlaceholder,
                    FontWeight = SpecializedDefs.DefaultFontWightForPlaceholder,
                    Text = (NewItemType switch
                    {
                        "Skill" => SpecializedDefs.DefaultTextForPlaceholder_Skill,
                        "Passive" => SpecializedDefs.DefaultTextForPlaceholder_Passive,
                        "Keyword" => SpecializedDefs.DefaultTextForPlaceholder_Keyword,
                        _ => SpecializedDefs.DefaultTextForPlaceholder_Keyword,
                    }).Extern(UID),

                    TextAlignment = TextAlignment.Center,
                    Effect = new DropShadowEffect() { BlurRadius = 0, ShadowDepth = 3 },
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                }
            },
        };

        InterfaceTranslationParameter MenuItem_Title   = ᐁ_Interface_Localization_Loader.SpecializedDefs.ColumnItemContextMenu_Title;
        InterfaceTranslationParameter MenuItem_Up      = ᐁ_Interface_Localization_Loader.SpecializedDefs.ColumnItemContextMenu_MoveUp;
        InterfaceTranslationParameter MenuItem_Down    = ᐁ_Interface_Localization_Loader.SpecializedDefs.ColumnItemContextMenu_MoveDown;
        InterfaceTranslationParameter MenuItem_Remove  = ᐁ_Interface_Localization_Loader.SpecializedDefs.ColumnItemContextMenu_Remove;
        InterfaceTranslationParameter MenuItem_Refresh = ᐁ_Interface_Localization_Loader.SpecializedDefs.ColumnItemContextMenu_Refresh;

        ColumnItemAdd.ContextMenu = new ContextMenu()
        {
            Items =
            {
                new MenuItem() // 0
                {
                    Header = new UITranslation_Rose()
                    {
                        RichText = MenuItem_Title.Text.Extern(UID),
                        FontSize = (double)MenuItem_Title.Font_Size,
                        Width =  (double)MenuItem_Title.Width,
                        Margin = (Thickness)MenuItem_Title.Margin_Loaded,
                        FontFamily = (FontFamily)MenuItem_Title.Font_Loaded,
                        FontWeight = (FontWeight)MenuItem_Title.Font_Weight_Loaded,
                        Foreground = ToSolidColorBrush(ᐁ_Interface_Themes_Loader.LoadedTheme.UIText.Foreground),
                        IsHitTestVisible = false
                    }
                },
                new Separator() { Margin = new Thickness(-30, 2, 0, 2) }, // 1
                new MenuItem() // 2
                {
                    Header = new UITranslation_Rose()
                    {
                        SpecProperty_ContextMenuParent = ColumnItemAdd, // ContextMenu parent link because i somehow cant access it through MenuItem.parent.parent.parent
                        
                        RichText = MenuItem_Up.Text.Extern(UID),
                        FontSize = (double)MenuItem_Up.Font_Size,
                        Width =  (double)MenuItem_Up.Width,
                        Margin = (Thickness)MenuItem_Up.Margin_Loaded,
                        FontFamily = (FontFamily)MenuItem_Up.Font_Loaded,
                        FontWeight = (FontWeight)MenuItem_Title.Font_Weight_Loaded,
                        Foreground = ToSolidColorBrush(ᐁ_Interface_Themes_Loader.LoadedTheme.UIText.Foreground),
                    }
                },
                new MenuItem() // 3
                {
                    Header = new UITranslation_Rose()
                    {
                        SpecProperty_ContextMenuParent = ColumnItemAdd,

                        RichText = MenuItem_Refresh.Text.Extern(UID),
                        FontSize = (double)MenuItem_Refresh.Font_Size,
                        Width =  (double)MenuItem_Refresh.Width,
                        Margin = (Thickness)MenuItem_Refresh.Margin_Loaded,
                        FontFamily = (FontFamily)MenuItem_Refresh.Font_Loaded,
                        FontWeight = (FontWeight)MenuItem_Title.Font_Weight_Loaded,
                        Foreground = ToSolidColorBrush(ᐁ_Interface_Themes_Loader.LoadedTheme.UIText.Foreground),
                    }
                },
                new MenuItem() // 4
                {
                    Header = new UITranslation_Rose()
                    {
                        SpecProperty_ContextMenuParent = ColumnItemAdd,

                        RichText = MenuItem_Down.Text.Extern(UID),
                        FontSize = (double)MenuItem_Down.Font_Size,
                        Width =  (double)MenuItem_Down.Width,
                        Margin = (Thickness)MenuItem_Down.Margin_Loaded,
                        FontFamily = (FontFamily)MenuItem_Down.Font_Loaded,
                        FontWeight = (FontWeight)MenuItem_Title.Font_Weight_Loaded,
                        Foreground = ToSolidColorBrush(ᐁ_Interface_Themes_Loader.LoadedTheme.UIText.Foreground),
                    }
                },
                new Separator() { Margin = new Thickness(-30, 2, 0, 2) }, // 5
                new MenuItem() // 6
                {
                    Header = new UITranslation_Rose()
                    {
                        SpecProperty_ContextMenuParent = ColumnItemAdd,

                        RichText = MenuItem_Remove.Text.Extern(UID),
                        FontSize = (double)MenuItem_Remove.Font_Size,
                        Width =  (double)MenuItem_Remove.Width,
                        Margin = (Thickness)MenuItem_Remove.Margin_Loaded,
                        FontFamily = (FontFamily)MenuItem_Remove.Font_Loaded,
                        FontWeight = (FontWeight)MenuItem_Title.Font_Weight_Loaded,
                        Foreground = ToSolidColorBrush(ᐁ_Interface_Themes_Loader.LoadedTheme.UIText.Foreground),
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

    private void SetFocusOnColumnElement_Link(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        SeriousScrollViewer_0.ScrollToBottom();
        SetFocusOnColumnItem(RequestSender as ItemRepresenter);
    }

    private void MoveColumnItemUp(object RequestSender, RoutedEventArgs EventArgs)
    {
        MoveColumnItemMaster(RequestSender, "Up");
    }

    private void MoveColumnItemDown(object RequestSender, RoutedEventArgs EventArgs)
    {
        MoveColumnItemMaster(RequestSender, "Down");
    }

    private void RefreshTextSize(object RequestSender, RoutedEventArgs EventArgs)
    {
        SetFocusOnColumnItem(((RequestSender as MenuItem).Header as UITranslation_Rose).SpecProperty_ContextMenuParent as ItemRepresenter);

        CheckKeywordSelectorAndGenerateKeywordDisplayer();
        CheckPassiveSelectorAndGeneratePassiveDisplayer();
        CheckSkillSelectorsAndGenerateSkillDisplayer();
    }

    private void MoveColumnItemMaster(object ContextMenuItemSender, string Direction)
    {
        ItemRepresenter Target = ((ContextMenuItemSender as MenuItem).Header as UITranslation_Rose).SpecProperty_ContextMenuParent as ItemRepresenter;

        StackPanel TargetColumn = Target.Parent as StackPanel;

        if (Direction.Equals("Up")) TargetColumn.MoveItemUp(Target);
        else TargetColumn.MoveItemDown(Target);

        ReEnumerateColumnItemsInProject();
    }

    private async void RemoveColumnItem(object RequestSender, RoutedEventArgs EventArgs)
    {
        ItemRepresenter Target = ((RequestSender as MenuItem).Header as UITranslation_Rose).SpecProperty_ContextMenuParent as ItemRepresenter;

        StackPanel TargetColumn = Target.Parent as StackPanel;

        await Task.Delay(150); // some white thing appears for 0.1 second if without delay

        DefinedUIDsForColumnItems.Remove(Target.Uid);

        TargetColumn.Children.Remove(Target);

        ReEnumerateColumnItemsInProject();
    }


    // Check column items and update all text info in project file data
    private void ReEnumerateColumnItemsInProject()
    {
        Dictionary<string, AddedTextItems_Single> FirstColumnProjectData = CustomIdentityPreviewCreator.ProjectFile.LoadedProject.Text.FirstColumnItems;
        Dictionary<string, AddedTextItems_Single> SecondColumnProjectData = CustomIdentityPreviewCreator.ProjectFile.LoadedProject.Text.SecondColumnItems;

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



    private StackPanel GetTargetColumn(object Sender)
    {
        return ((((Sender as ComboBoxItem).Parent as ComboBox).Parent as Grid).Parent as StackPanel).Children[0] as StackPanel;
    }

    private void IdentityPreviewCreator_AddSkillToColumn(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        IdentityPreviewCreator_AddItemToColumnFromUIMaster(GetTargetColumn(RequestSender), "Skill");
    }
    private void IdentityPreviewCreator_AddPassiveToColumn(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        IdentityPreviewCreator_AddItemToColumnFromUIMaster(GetTargetColumn(RequestSender), "Passive");
    }
    private void IdentityPreviewCreator_AddKeywordToColumn(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        IdentityPreviewCreator_AddItemToColumnFromUIMaster(GetTargetColumn(RequestSender), "Keyword");
    }








    private void IdentityPreviewCreator_AddItemToColumnFromUIMaster(StackPanel TargetColumn, string Type)
    {
        ItemRepresenter CreatedColumnItem = CreatePlaceholder(Type);
        CreatedColumnItem.ColumnNumber = TargetColumn.Uid;

        TargetColumn.Children.Add(CreatedColumnItem);
        ReEnumerateColumnItemsInProject();

        SetFocusOnColumnItem(CreatedColumnItem);
    }

    public static ItemRepresenter FocusedColumnItem = null;
    private void SetFocusOnColumnItem(ItemRepresenter Target, bool UpdateSelectorsAndSliders = true)
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

        if (UpdateSelectorsAndSliders) KeywordIconSelectionLabel.RichText = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] * [Section:Text info/Selected item settings] Keyword icon image", "Default");

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
                        KeywordIconSelectionLabel.RichText = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] * [Section:Text info/Selected item settings] Keyword icon image", "Selected").Extern(FocusedColumnItem.ItemInfo.KeywordIconImage.GetName());
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

        IdentityPreviewCreator_EditingItemHeader.RichText = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] * [Section:Text info/Selected item settings] Editing item header").Extern(FocusedColumnItem.Uid);

        ColumnItemFocusingEvent = false;
    }
    private void ChangeSliderValuesOnFocus()
    {
        ColumnItemVerticalOffsetControllder.Value = FocusedColumnItem.ItemInfo.VerticalOffset;
        ColumnItemHorizontalOffsetControllder.Value = FocusedColumnItem.ItemInfo.HorizontalOffset;

        NameMaxWidthController.Value = FocusedColumnItem.ItemInfo.NameMaxWidth;

        KeywordOrPassiveDescriptionWidthController.Value = FocusedColumnItem.ItemInfo.PassiveDescriptionWidth;

        SkillMainDescriptionWidthController.Value = FocusedColumnItem.ItemInfo.SkillMainDescriptionWidth;
        SkillCoinDescriptionsWidthController.Value = FocusedColumnItem.ItemInfo.SkillCoinsDescriptionWidth;
    }









    private void IdentityPreviewCreator_LoadProjectFile(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        OpenFileDialog ProjectSelectorDialog = NewOpenFileDialog("Saved project file", ["json"]);

        if (ProjectSelectorDialog.ShowDialog() == true)
        {
            FileInfo Target = LoadedProjectFile = new FileInfo(ProjectSelectorDialog.FileName);
            CustomIdentityPreviewCreator.ProjectFile.CustomIdentityPreviewProject LoadedProject = Target.Deserealize<CustomIdentityPreviewCreator.ProjectFile.CustomIdentityPreviewProject>(Context: Target.Directory.FullName.Replace("\\", "/"));

            IdentityPreviewCreator_LoadBlankProject();
            IdentityPreviewCreator_LoadProjectRecord(LoadedProject);
        }
    }

    private bool CanSelectDecorativeCautions = true;
    private void IdentityPreviewCreator_LoadProjectRecord(CustomIdentityPreviewCreator.ProjectFile.CustomIdentityPreviewProject LoadedProject)
    {
        if (LoadedProject.ActualProject != null)
        {
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

                if (File.Exists(ImageParameters.PortraitImage)) SelectIdentityOrEGOPortrait_Action(ImageParameters.PortraitImage);
                else IdentityPreviewCreator_IdentityPortrait.Source = new BitmapImage();

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
                var @Specific = LoadedProject.Specific;

                string StoredSinnerName = Specific.SinnerName; // Changing icon also changes default sinner name
                string StoredAmbienceColor = Specific.AmbienceColor; // Same
                if (Specific.SinnerIcon != null)
                {
                    if (File.Exists(Specific.SinnerIcon))
                    {
                        // Custom
                        SelectCustomSinnerIcon_Action(Specific.SinnerIcon);
                    }
                    else
                    {
                        CanSelectDecorativeCautions = false;
                        IdentityPreviewCreator_SinnerIconSelector.SelectedIndex = Specific.SinnerIcon switch
                        {
                            "Yi Sang" => 0,
                            "Faust" => 1,
                            "Don Quixote" => 2,
                            "Ryōshū" => 3,
                            "Meursault" => 4,
                            "Hong Lu" => 5,
                            "Heathcliff" => 6,
                            "Ishmael" => 7,
                            "Rodion" => 8,
                            "Sinclair" => 9,
                            "Outis" => 10,
                            "Gregor" => 11,
                            _ => 0
                        };
                        CanSelectDecorativeCautions = true;
                    }
                }
                

                IdentityPreviewCreator_SinnerIconBrightnessController.Value = Specific.IconBrightness;
                IdentityPreviewCreator_SinnerIconSizeController.Value = Specific.IconSize;

                IdentityPreviewCreator_TextEntries_ElementsColor.Text = StoredAmbienceColor;
                if (StoredSinnerName != null) IdentityPreviewCreator_TextEntries_SinnerName.Text = StoredSinnerName;
                if (Specific.IdentityOrEGOName != null) IdentityPreviewCreator_TextEntries_IdentityOrEGOName.Text = Specific.IdentityOrEGOName;

                CanOverwriteProjectCautionsType = false;
                IdentityPreviewCreator_TextEntries_ElementsColor.Text = Specific.AmbienceColor;
                CanOverwriteProjectCautionsType = true;

                if (Specific.RarityOrEGORiskLevel != null)
                {
                    IdentityPreviewCreator_IdentityHeader_RarityOrEGORiskLevelSelector.SelectedIndex = Specific.RarityOrEGORiskLevel switch
                    {
                        "000" => 0,
                        "00" => 1,
                        "0" => 2,

                        "ZAYIN" => 3,
                        "HE" => 4,
                        "TETH" => 5,
                        "WAW" => 6,
                        "ALEPH" => 7,

                        _ => -1
                    };
                }
            }
            #endregion
            
            #region Decorative cautions
            {
                var @Cautions = LoadedProject.DecorativeCautions;

                ToggleArknightsMode.IsChecked = Cautions.EnableArknightsMode;

                IdentityPreviewCreator_Cautions_BoomRadiusController.Value = Cautions.CautionBloomRadius;
                IdentityPreviewCreator_Cautions_OpacityController.Value = Cautions.CautionOpacity;

                IdentityPreviewCreator_TextEntries_CustomCautionString.Text = Cautions.CustomText.CustomCautionString;
                if (File.Exists(Cautions.CustomText.AnotherFont)) SelectDecorativeCautionsCustomFont_Action(Cautions.CustomText.AnotherFont);

                CautionsCustomTextVerticalOffsetController.Value = Cautions.CustomText.TextVerticalOffset * 100;
                CautionsCustomTextSizeController.Value = Cautions.CustomText.TextSize * 100;

                IdentityPreviewCreator_CautionTypeSelector.SelectedIndex = Cautions.CautionType switch
                {
                    "SEASON" => 0,
                    "CAUTION" => 1,
                    "None" => 2,
                    "Custom text" => 3,
                    _ => -1
                };
            }
            #endregion

            #region Text info
            {
                var @TextInfo = LoadedProject.Text;

                UnifiedTextSizeController.Value = TextInfo.UnifiedTextSize;

                IdentityPreviewCreator_TextInfo_FirstColumnOffsetController.Value = TextInfo.FirstColumnOffset;
                IdentityPreviewCreator_TextInfo_SecondColumnOffsetController.Value = TextInfo.SecondColumnOffset;

                FirstColumnItemSignaturesOffsetController.Value = TextInfo.FirstColumnItemSignaturesOffset;
                SecondColumnItemSignaturesOffsetController.Value = TextInfo.SecondColumnItemSignaturesOffset;
                if (File.Exists(TextInfo.ItemSignaturesAnotherFont)) SelectAnotherItemSignsFont_Action(TextInfo.ItemSignaturesAnotherFont);

                KeywordBoxesWidthController.Value = TextInfo.KeywordBoxesWidth;

                // Update ID selectors
                if (File.Exists(TextInfo.SkillsLocalizationFile)) UpdateSelector__SkillLocalizationIDSelector();
                if (File.Exists(TextInfo.SkillsDisplayInfoConstructorFile)) UpdateSelector__SkillsDisplayInfoIDSelector();
                if (File.Exists(TextInfo.PassivesLocalizationFile)) UpdateSelector__PassivesLocalizationIDSelector();
                if (File.Exists(TextInfo.KeywordsLocalizationFile)) UpdateSelector__KeywordsLocalizationIDSelector();

                ReconstructColumnItems();
            }
            #endregion
        }
    }


    private void IdentityPreviewCreator_LoadBlankProject()
    {
        IdentityPreviewCreator_LoadProjectRecord(new CustomIdentityPreviewCreator.ProjectFile.CustomIdentityPreviewProject() with { ActualProject = true});

        IdentityPreviewCreator_IdentityPortrait.Source = new BitmapImage();

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

    private void IdentityPreviewCreator_SaveProjectToFile(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        SaveFileDialog SaveLocation = NewSaveFileDialog("Json files", ["json"], "Project.json");

        if (SaveLocation.ShowDialog() == true)
        {
            CustomIdentityPreviewCreator.ProjectFile.LoadedProject.SerializeFormatted(SaveLocation.FileName, Context: new FileInfo(SaveLocation.FileName).Directory.FullName.Replace("\\", "/"));
        }
    }

    





    private Dictionary<int, int> ID_And_Index__Links_Skills = new Dictionary<int, int>();
    private void UpdateSelector__SkillLocalizationIDSelector()
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

                    string ItemName = $"№{ItemEnumerator} — ID {SkillItem.ID} {SkillName}";

                    AddSpecItemToSelector(SkillsLocalizationIDSelector, ItemName, TargetUptieWithDesc, SkillItem.ID);
                    ID_And_Index__Links_Skills[(int)SkillItem.ID] = ItemEnumerator - 1;
                }

                ItemEnumerator++;
            }
        }

        if (SkillsLocalizationIDSelector.Items.Count > 0)
        {
            SelectSkillsLocalizationFile_Label.RichText = ExternTextFor("[C] * [Section:Text info/Text info sources] Skills localization", "Selected").Extern(TargetFile.GetName());
        }
    }

    private Dictionary<int, int> ID_And_Index__Links_Passives = new Dictionary<int, int>();
    private void UpdateSelector__PassivesLocalizationIDSelector()
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
                    string ItemName = $"№{ItemEnumerator} — ID {PassiveItem.ID} {PassiveItem.Name.nullHandle(NullText: "<i>No name</i>")}";

                    AddSpecItemToSelector(PassivesLocalizationIDSelector, ItemName, PassiveItem, PassiveItem.ID);

                    ID_And_Index__Links_Passives[(int)PassiveItem.ID] = ItemEnumerator - 1;
                }

                ItemEnumerator++;
            }
        }

        if (PassivesLocalizationIDSelector.Items.Count > 0)
        {
            SelectPassivesLocalizationFile_Label.RichText = ExternTextFor("[C] * [Section:Text info/Text info sources] Passives localization", "Selected").Extern(TargetFile.GetName());
        }
    }

    private Dictionary<string, int> ID_And_Index__Links_Keywords = new Dictionary<string, int>();
    private void UpdateSelector__KeywordsLocalizationIDSelector()
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
                    string ItemName = $"№{ItemEnumerator} — ID {KeywordItem.ID} {KeywordItem.Name.nullHandle(NullText: "<i>No name</i>")}";

                    AddSpecItemToSelector(KeywordsLocalizationIDSelector, ItemName, KeywordItem, KeywordItem.ID);

                    ID_And_Index__Links_Keywords[KeywordItem.ID] = ItemEnumerator - 1;
                }

                ItemEnumerator++;
            }
        }

        if (KeywordsLocalizationIDSelector.Items.Count > 0)
        {
            SelectKeywordsLocalizationFile_Label.RichText = ExternTextFor("[C] * [Section:Text info/Text info sources] Keywords localization", "Selected").Extern(TargetFile.GetName());
        }
    }

    private Dictionary<BigInteger, int> ID_And_Index__Links_SkillsDisplayInfo = new Dictionary<BigInteger, int>();
    private void UpdateSelector__SkillsDisplayInfoIDSelector()
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

                    string TargetName = $"№{ItemEnumerator} — ID {Constructor.ID} <color={AffinityColor}>{Constructor.SkillName.nullHandle(NullText: "<i>No name</i>")}</color>";

                    AddSpecItemToSelector(SkillsDisplayInfoIDSelector, TargetName, Constructor, Constructor.ID);

                    ID_And_Index__Links_SkillsDisplayInfo[(BigInteger)Constructor.ID] = ItemEnumerator - 1;
                }

                ItemEnumerator++;
            }
        }

        if (SkillsDisplayInfoIDSelector.Items.Count > 0)
        {
            SelectSkillsDisplayInfoFile_Label.RichText = ExternTextFor("[C] * [Section:Text info/Text info sources] Skills Display Info", "Selected").Extern(TargetFile.Name);
        }
    }


    private void AddSpecItemToSelector(ComboBox Target, string ItemName, dynamic AttachedItem, dynamic AttachedItemID)
    {
        Target.Items.Add(new UITranslation_Rose()
        {
            RichText = ItemName,
            FontSize = 14,
            Padding = new Thickness(5, 5, 0, 5),
            Width = 150,
            UniversalDataBindings = new Dictionary<string, dynamic>() { ["Attached item"] = AttachedItem, ["Attached item ID"] = AttachedItemID }
        }.SetBindingWithReturn(UITranslation_Rose.FontFamilyProperty, "FontFamily", Target));
    }

    private void ToggleArknightsMode_SidedLink(object RequestSender, RoutedEventArgs EventArgs)
    {
        CustomIdentityPreviewCreator.ProjectFile.LoadedProject.DecorativeCautions.EnableArknightsMode = (bool)ToggleArknightsMode.IsChecked;
        if ((bool)ToggleArknightsMode.IsChecked)
        {
            ArknightsXLimbusLogo.Visibility = Visible;
            LeftCornerRegularLogo.Visibility = Collapsed;
            RightCornerDarkLogo.Source = BitmapFromResource("UI/Limbus/Custom Identity Preview/Spec/Another Icon.png");
        }
        else
        {
            ArknightsXLimbusLogo.Visibility = Collapsed;
            LeftCornerRegularLogo.Visibility = Visible;
            RightCornerDarkLogo.Source = BitmapFromResource("UI/Limbus/Custom Identity Preview/Right Bottom Corner Logo.png");
        }
    }

    private void ToggleWalpurgisFrame_SidedLink(object RequestSender, RoutedEventArgs EventArgs)
    {
        if ((bool)ToggleWalpurgisFrame.IsChecked)
        {
            WalpurgisNightFrames.Visibility = Visible;
            IdentityPreviewCreator_DecorativeCautions_2.Visibility = Collapsed;
        }
        else
        {
            WalpurgisNightFrames.Visibility = Collapsed;
            IdentityPreviewCreator_DecorativeCautions_2.Visibility = Visible;
        }
    }
}