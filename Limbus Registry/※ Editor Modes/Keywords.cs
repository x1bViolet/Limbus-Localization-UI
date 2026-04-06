using ICSharpCode.AvalonEdit.Document;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using static LCLocalizationInterface.LimbusRegistry.@EditorModesShelf.Types;

namespace LCLocalizationInterface.LimbusRegistry
{
    public ref partial struct @EditorModesShelf
    {
        public static readonly KeywordsEditorMode Keywords = new();

        public ref partial struct Types
        {
            public class KeywordsEditorMode : EditorModeAbstraction<Keyword>
            {
                public override EditorModeKey Identifier => EditorModeKey.Keywords;




                protected override Func<Keyword, bool> DataListValidator { get; } =

                    ListObject => !string.IsNullOrWhiteSpace(ListObject.ID);


                protected override MainWindowDimensing Dimensing { get; } = new()
                {
                    MaxWidth = 1000, MinWidth = 713, MinHeight = 409,
                    Width    = 1000, Height   = 600,

                    RichTextViewsHeight             = new GridLength(424),
                    TextEditorAndRichTextViewsWidth = new GridLength(706),

                    RichTextViewsTabIndex     = 3,
                    RightMenuSectionsTabIndex = 3,
                };


                public override List<IntenseStareType3> PresentedRightMenuSytaxedTextInputs => [
                    @Languages.PresentedTextFields["[Keywords / Right menu] * Keyword name"]
                ];








                private object SreenshotArea_Bufs => MainWindowInstance.RichTextViews__Keywords_COMPOSITION_Bufs_SurfaceScrollViewer.Content;
                private object SreenshotArea_BattleKeywords => MainWindowInstance.RichTextViews__Keywords_COMPOSITION_BattleKeywords_GeneralParentScrollViewer.Content;
                public override void ScreenshotRichText()
                {
                    object TargetSreenshotArea = CheckFileName!.StartsWith("Bufs")
                        ? SreenshotArea_Bufs
                        : SreenshotArea_BattleKeywords;

                    MainWindowInstance.RichTextViews__Keywords_COMPOSITION_BattleKeywords_GeneralParentBorder.MaxHeight = double.MaxValue;

                    using (new ScreenshotBackgroundSetter(TargetSreenshotArea))
                    {
                        (TargetSreenshotArea as FrameworkElement)!.RenderImage(ScanPathTemplate.Exform(CurrentFile!.Name, this.CurrentKeywordID), ScreenshotsUpscale);
                    }

                    MainWindowInstance.RichTextViews__Keywords_COMPOSITION_BattleKeywords_GeneralParentBorder.MaxHeight = 277;
                }
                
                
                public override void RefreshRichText()
                {
                    MainWindowInstance.RichTextViews__Keywords_Bufs_Name.RefreshRichText();
                    MainWindowInstance.RichTextViews__Keywords_BattleKeywords_Name.RefreshRichText();

                    MainWindowInstance.RichTextViews__Keywords_Bufs_MainDescription.RefreshRichText();
                    MainWindowInstance.RichTextViews__Keywords_Bufs_SummaryDescription.RefreshRichText();
                    MainWindowInstance.RichTextViews__Keywords_Bufs_FlavorDescription.RefreshRichText();
                    MainWindowInstance.RichTextViews__Keywords_BattleKeywords_MainDescription.RefreshRichText();
                    MainWindowInstance.RichTextViews__Keywords_BattleKeywords_SummaryDescription.RefreshRichText();
                    MainWindowInstance.RichTextViews__Keywords_BattleKeywords_FlavorDescription.RefreshRichText();
                }
                



                public override (bool IsAnyUnsavedChanges, string UnsavedChangesText) CollectUnsavedChanges()
                {
                    string UnsavedChangesString = "";
                    foreach (Keyword InspectingKeyword in this.RouteDictionary.Values)
                    {
                        string UnsavedChangesString_SingleKeyword = "";

                        if (InspectingKeyword.IsNameUnsaved)
                            UnsavedChangesString_SingleKeyword += @Languages.VariableData.UnsavedChangesInfo.Keywords.Name;
                            
                        if (InspectingKeyword.IsColorUnsaved)
                            UnsavedChangesString_SingleKeyword += @Languages.VariableData.UnsavedChangesInfo.Keywords.Color;
                            
                        if (InspectingKeyword.IsMainDescUnsaved)
                            UnsavedChangesString_SingleKeyword += @Languages.VariableData.UnsavedChangesInfo.Keywords.MainDesc;
                            
                        if (InspectingKeyword.IsSummaryDescUnsaved)
                            UnsavedChangesString_SingleKeyword += @Languages.VariableData.UnsavedChangesInfo.Keywords.SummaryDesc;
                            
                        if (InspectingKeyword.IsFlavorDescUnsaved)
                            UnsavedChangesString_SingleKeyword += @Languages.VariableData.UnsavedChangesInfo.Keywords.FlavorDesc;

                        if (UnsavedChangesString_SingleKeyword != "")
                        {
                            UnsavedChangesString +=
                                $"{@Languages.VariableData.UnsavedChangesInfo.Keywords.IDHeader.Exform(InspectingKeyword.ID, InspectingKeyword.Name)}" +
                                $"{UnsavedChangesString_SingleKeyword}";
                        }
                    }

                    return (UnsavedChangesString != "", UnsavedChangesString.Trim());
                }




                public override void SwitchIDButtonClick(IDSwitchDirection Direction)
                {
                    int NewIndex = this.AvailableIDsList.IndexOf(this.CurrentKeywordID) + (Direction == IDSwitchDirection.Forward ? 1 : -1);
                    this.SwitchToKeyword(this.AvailableIDsList[NewIndex]);
                }
                

                public override void SwitchIDButtonClick_ToVeryFirstOrLast(IDSwitchDirection Direction)
                {
                    this.SwitchToKeyword(this.AvailableIDsList[Direction == IDSwitchDirection.Back ? 0 : this.AvailableIDsList.Count - 1]);
                }
                

                public override void SwitchToObjectByInput(string Input)
                {
                    if (this.RouteDictionary.ContainsKey(Input))
                    {
                        SwitchToKeyword(Input);
                    }
                    else if (this.NameAndIDMatches.TryGetValueCaseInsensitive(Input, out string? FoundMatchingSkillIDByName))
                    {
                        SwitchToKeyword(FoundMatchingSkillIDByName!);
                    }
                }








                public override void ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(TextDocument CurrentDocument)
                {
                    if (CurrentDocument == this.CurrentKeyword.DedicatedDocument_MainDescription)
                        @Languages.PresentedTextElements["[Keywords / Right Menu] * Keyword desc"].MarkWithUnsavedByCondition(this.CurrentKeyword.IsMainDescUnsaved);

                    else if (CurrentDocument == this.CurrentKeyword.DedicatedDocument_SummaryDescription)
                        @Languages.PresentedTextElements["[Keywords / Right Menu] * Keyword summary"].MarkWithUnsavedByCondition(this.CurrentKeyword.IsSummaryDescUnsaved);

                    else if (CurrentDocument == this.CurrentKeyword.DedicatedDocument_FlavorDescription)
                        @Languages.PresentedTextElements["[Keywords / Right Menu] * Keyword flavor"].MarkWithUnsavedByCondition(this.CurrentKeyword.IsFlavorDescUnsaved);
                }
                
                
                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnButtons()
                {
                    @Languages.PresentedTextElements["[Keywords / Right Menu] * Keyword desc"].MarkWithUnsavedByCondition(this.CurrentKeyword.IsMainDescUnsaved);
                    @Languages.PresentedTextElements["[Keywords / Right Menu] * Keyword summary"].MarkWithUnsavedByCondition(this.CurrentKeyword.IsSummaryDescUnsaved);
                    @Languages.PresentedTextElements["[Keywords / Right Menu] * Keyword flavor"].MarkWithUnsavedByCondition(this.CurrentKeyword.IsFlavorDescUnsaved);
                }
                
                
                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs()
                {
                    if (CurrentFile is not null) // Can be called on startup by TextChanged
                    {
                        @Languages.PresentedTextFields["[Keywords / Right menu] * Keyword name"].MarkWithUnsavedByCondition(this.CurrentKeyword.IsNameUnsaved);
                        @Languages.PresentedTextFields["[Keywords / Right menu] * Keyword color"].MarkWithUnsavedByCondition(this.CurrentKeyword.IsColorUnsaved);
                    }
                }








                public override void SaveCurrentDescription(TextDocument CurrentDocument, ref bool CancelDefaultSave)
                {
                    if (@Languages.PresentedTextFields["[Keywords / Right menu] * Keyword name"].IsFocused)
                    {
                        SaveName(); CancelDefaultSave = true;
                    }
                    else if (@Languages.PresentedTextFields["[Keywords / Right menu] * Keyword color"].IsFocused)
                    {
                        SaveColor(); CancelDefaultSave = true;
                    }
                    else if (CurrentDocument == this.CurrentKeyword.DedicatedDocument_MainDescription) this.CurrentKeyword.SyncMainDesc();
                    else if (CurrentDocument == this.CurrentKeyword.DedicatedDocument_FlavorDescription) this.CurrentKeyword.SyncFlavorDesc();
                    else if (CurrentDocument == this.CurrentKeyword.DedicatedDocument_SummaryDescription) this.CurrentKeyword.SyncSummaryDesc();
                }

                [LayeredComponent]
                public void SaveName()
                {
                    this.CurrentKeyword.SyncName();
                    ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
                    base.SaveCurrentFile_Action();
                }

                [LayeredComponent]
                public void SaveColor()
                {
                    this.CurrentKeyword.SyncColor();
                    ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
                    base.SaveCurrentFile_Action();
                }




                public override void UISwitchPrecedingActions()
                {
                    this.RouteDictionary.Clear();

                    foreach (Keyword Keyword in DeserializedLocalizationData!.DataList.Where(DataListValidator))
                    {
                        this.RouteDictionary[Keyword.ID!] = Keyword;
                    }

                    DetermineKeywordsViewType();

                    this.SwitchToKeyword(this.AvailableIDsList.First());
                }

                [LayeredComponent]
                private void DetermineKeywordsViewType()
                {
                    Dimensing.RichTextViewsHeight = new GridLength(CheckFileName!.StartsWith("Bufs") ? 424 : 370);
                    Dimensing.MinHeight = CheckFileName!.StartsWith("Bufs") ? 464 : 409;

                    /// <see cref="Instruments.WPFTools.set_Visible(UIElement, bool)"/> extension from "Preview" C# language version
                    MainWindowInstance.FormatInsertionsInputParentStackPanel.Visibility = CheckFileName!.StartsWith("Bufs") ? Visibility.Visible : Visibility.Collapsed;

                    MainWindowInstance.RichTextViews__Keywords_COMPOSITION_CurrentKeywordsViewVersionTab.SelectedIndex
                        = CheckFileName!.StartsWith("Bufs") ? 1 : 0; // Bufs / BattleKeywords view version select
                }








                #region Integral part of this Editor


                #region Navigation nodes

                public List<string> AvailableIDsList => [.. this.RouteDictionary.Keys];

                public Dictionary<string, string> NameAndIDMatches =>
                    this.RouteDictionary.ToDictionarySafe(KeySelector: KeywordKVPair => KeywordKVPair.Value.Name, KeywordKVPair => KeywordKVPair.Key);


                public Dictionary<string, Keyword> RouteDictionary = [];

                public string CurrentKeywordID = "";
                public Keyword CurrentKeyword => this.RouteDictionary[this.CurrentKeywordID];

                protected override void ClearSpecificData()
                {
                    @DataContextDomain.Editor.CurrentKeyword = null!;

                    this.RouteDictionary.Clear();
                    this.CurrentKeywordID = "";
                }

                #endregion


                #region Right menu

                [LayeredComponent]
                public void SwitchToKeyword(string TargetKeywordID)
                {
                    this.CurrentKeywordID = TargetKeywordID;

                    base.CheckSwitchIDButtonsAvailability(this.AvailableIDsList.IndexOf(this.CurrentKeywordID), this.AvailableIDsList.Count - 1);

                    #region DataContextDomain values set
                    @DataContextDomain.Editor.CurrentKeyword = this.CurrentKeyword;
                    #endregion

                    ChangeKeywordIconView();

                    SwitchToCurrentKeywordMainDescription();

                    ChangeAllRightMenuUnsavedChangesMarkers_OnButtons();
                }

                [LayeredComponent]
                public void ChangeKeywordIconView()
                {
                    BitmapImage KeywordIcon = ImageDictionaries.KeywordImages[this.CurrentKeywordID];
                    
                    if (KeywordIcon == ImageDictionaries.UnknownSpriteImage &&
                        ImageDictionaries.NotSuitableForSpriteTagRedirections.TryGetValue(this.CurrentKeywordID, out string? FoundAnotherSpriteID)
                    ) {
                        // Try get from NotSuitableForSpriteTag redirections then
                        KeywordIcon = ImageDictionaries.KeywordImages[FoundAnotherSpriteID];
                    }

                    MainWindowInstance.RichTextViews__Keywords_COMPOSITION_CurrentKeywordIcon.Source = KeywordIcon;
                }
                
                [LayeredComponent]
                public void SwitchToCurrentKeywordMainDescription()
                {
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentKeyword.DedicatedDocument_MainDescription;
                    MainWindowInstance.RichTextViews__Keywords_COMPOSITION_MainAndSummaryDescsTab.SelectedIndex = 0;
                }

                [LayeredComponent]
                public void SwitchToCurrentKeywordFlavorDescription()
                {
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentKeyword.DedicatedDocument_FlavorDescription!;
                    MainWindowInstance.RichTextViews__Keywords_COMPOSITION_MainAndSummaryDescsTab.SelectedIndex = 0;
                }

                [LayeredComponent]
                public void SwitchToCurrentKeywordSummaryDescription()
                {
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentKeyword.DedicatedDocument_SummaryDescription!;
                    MainWindowInstance.RichTextViews__Keywords_COMPOSITION_MainAndSummaryDescsTab.SelectedIndex = 1;
                }

                #endregion


                #endregion
            }
        }
    }
}

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        #region Right menu
        
        private void RightMenuButtons__Keywords_SaveName(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Keywords.SaveName();
        }
        
        private void RightMenuButtons__Keywords_SaveColor(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Keywords.SaveColor();
        }

        private void RightMenuTextBoxes__Keywords_CommonTextChanged(object Sender, EventArgs Args)
        {
            @EditorModesShelf.Keywords.ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
        }

        
        private void RightMenuButtons__Keywords_SwitchToMainDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Keywords.SwitchToCurrentKeywordMainDescription();

            if (@EditorModesShelf.Keywords.CurrentKeyword.FlavorDescription is not null)
            {
                RichTextViews__Keywords_Bufs_MainDescription.BringIntoView();
                RichTextViews__Keywords_BattleKeywords_MainDescription.BringIntoView();
                RichTextViews__SHARED_DescriptionManualSwitchHighlight(RichTextViews__Keywords_Bufs_MainDescription);
                RichTextViews__SHARED_DescriptionManualSwitchHighlight(RichTextViews__Keywords_BattleKeywords_MainDescription);
            }
        }
        
        private void RightMenuButtons__Keywords_SwitchToFlavorDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Keywords.SwitchToCurrentKeywordFlavorDescription();
            RichTextViews__Keywords_Bufs_FlavorDescription.BringIntoView();
            RichTextViews__Keywords_BattleKeywords_FlavorDescription.BringIntoView();
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(RichTextViews__Keywords_Bufs_FlavorDescription);
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(RichTextViews__Keywords_BattleKeywords_FlavorDescription);
        }
        
        private void RightMenuButtons__Keywords_SwitchToSummaryDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Keywords.SwitchToCurrentKeywordSummaryDescription();
        }
        
        #endregion




        #region Interactive things from limbus ui visualization

        private void RichTextViews__Keywords_COMPOSITION_Bufs_OKButton_Click(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.Keywords.CurrentKeyword.SyncName();
            @EditorModesShelf.Keywords.CurrentKeyword.SyncColor();
            @EditorModesShelf.Keywords.CurrentKeyword.SyncMainDesc();
            @EditorModesShelf.Keywords.CurrentKeyword.SyncFlavorDesc();
            @EditorModesShelf.Keywords.CurrentKeyword.SyncSummaryDesc();
            @EditorModesShelf.Keywords.ChangeAllRightMenuUnsavedChangesMarkers_OnButtons();
            @EditorModesShelf.Keywords.ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
            @EditorModesShelf.Keywords.SaveCurrentFile_Action();
        }

        #endregion




        #region Fast switch by right click on desc at the rich text view
        
        private void RichTextViews__Keywords_FastSwitch_ToKeywordMainDescription(object Sender, MouseButtonEventArgs Args)
        {
            if (@EditorModesShelf.Keywords.CurrentKeyword.FlavorDescription is not null)
            {
                @EditorModesShelf.Keywords.SwitchToCurrentKeywordMainDescription();
                RichTextViews__SHARED_DescriptionFastSwitchHighlight(Sender);
            }
        }
        
        private void RichTextViews__Keywords_FastSwitch_ToKeywordFlavorDescription(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Keywords.SwitchToCurrentKeywordFlavorDescription();
            RichTextViews__SHARED_DescriptionFastSwitchHighlight(Sender);
        }
        
        #endregion
    }
}