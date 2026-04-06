using ICSharpCode.AvalonEdit.Document;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using static LCLocalizationInterface.LimbusRegistry.@EditorModesShelf.Types;

namespace LCLocalizationInterface.LimbusRegistry
{
    public ref partial struct @EditorModesShelf
    {
        public static readonly PassivesEditorMode Passives = new();

        public ref partial struct Types
        {
            public class PassivesEditorMode : EditorModeAbstraction<Passive>
            {
                public override EditorModeKey Identifier => EditorModeKey.Passives;




                protected override Func<Passive, bool> DataListValidator { get; } =

                    ListObject => ListObject.ID is not null;


                protected override MainWindowDimensing Dimensing { get; } = new()
                {
                    MaxWidth = 1000, MinWidth = 713, MinHeight = 425,
                    Width    = 1000, Height   = 600,

                    RichTextViewsHeight             = new GridLength(387),
                    TextEditorAndRichTextViewsWidth = new GridLength(706),

                    RichTextViewsTabIndex     = 2,
                    RightMenuSectionsTabIndex = 2,
                };


                public override List<IntenseStareType3> PresentedRightMenuSytaxedTextInputs => [
                    @Languages.PresentedTextFields["[Passives / Right menu] * Passive name"]
                ];








                private object SreenshotArea => MainWindowInstance.RichTextViews__Passives_COMPOSITION_SurfaceScrollViewer.Content;
                public override void ScreenshotRichText()
                {
                    using (new ScreenshotBackgroundSetter(this.SreenshotArea))
                    {
                        (this.SreenshotArea as FrameworkElement)!.RenderImage(ScanPathTemplate.Exform(CurrentFile!.Name, this.CurrentPassiveID), ScreenshotsUpscale);
                    }
                }

                
                public override void RefreshRichText()
                {
                    MainWindowInstance.RichTextViews__Passives_Name.RefreshNameRichText();
                    MainWindowInstance.RichTextViews__Passives_MainDescription.RefreshRichText();
                    MainWindowInstance.RichTextViews__Passives_FlavorDescription.RefreshRichText();
                    MainWindowInstance.RichTextViews__Passives_SummaryDescription.RefreshRichText();
                }




                public override (bool IsAnyUnsavedChanges, string UnsavedChangesText) CollectUnsavedChanges()
                {
                    string UnsavedChangesString = "";
                    foreach (Passive InspectingPassive in this.RouteDictionary.Values)
                    {
                        string UnsavedChangesString_SinglePassive = "";

                        if (InspectingPassive.IsNameUnsaved)
                            UnsavedChangesString_SinglePassive += @Languages.VariableData.UnsavedChangesInfo.Passives.Name;
                            
                        if (InspectingPassive.IsMainDescUnsaved)
                            UnsavedChangesString_SinglePassive += @Languages.VariableData.UnsavedChangesInfo.Passives.MainDesc;
                            
                        if (InspectingPassive.IsSummaryDescUnsaved)
                            UnsavedChangesString_SinglePassive += @Languages.VariableData.UnsavedChangesInfo.Passives.SummaryDesc;
                            
                        if (InspectingPassive.IsFlavorDescUnsaved)
                            UnsavedChangesString_SinglePassive += @Languages.VariableData.UnsavedChangesInfo.Passives.FlavorDesc;

                        if (UnsavedChangesString_SinglePassive != "")
                        {
                            UnsavedChangesString +=
                                $"{@Languages.VariableData.UnsavedChangesInfo.Passives.IDHeader.Exform(InspectingPassive.ID, InspectingPassive.Name)}" +
                                $"{UnsavedChangesString_SinglePassive}";
                        }
                    }

                    return (UnsavedChangesString != "", UnsavedChangesString.Trim());
                }




                public override void SwitchIDButtonClick(IDSwitchDirection Direction)
                {
                    int NewIndex = this.AvailableIDsList.IndexOf(this.CurrentPassiveID) + (Direction == IDSwitchDirection.Forward ? 1 : -1);
                    this.SwitchToPassive(this.AvailableIDsList[NewIndex]);
                }
                
                
                public override void SwitchIDButtonClick_ToVeryFirstOrLast(IDSwitchDirection Direction)
                {
                    this.SwitchToPassive(this.AvailableIDsList[Direction == IDSwitchDirection.Back ? 0 : this.AvailableIDsList.Count - 1]);
                }
                
                
                public override void SwitchToObjectByInput(string Input)
                {
                    if (BigInteger.TryParse(Input, out BigInteger TargetSkillID) && this.RouteDictionary.ContainsKey(TargetSkillID))
                    {
                        SwitchToPassive(TargetSkillID);
                    }
                    else if (this.NameAndIDMatches.TryGetValueCaseInsensitive(Input, out BigInteger FoundMatchingSkillIDByMainName))
                    {
                        SwitchToPassive(FoundMatchingSkillIDByMainName);
                    }
                }








                public override void ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(TextDocument CurrentDocument)
                {
                    if (CurrentDocument == this.CurrentPassive.DedicatedDocument_MainDescription)
                        @Languages.PresentedTextElements["[Passives / Right menu] * Passive desc"].MarkWithUnsavedByCondition(this.CurrentPassive.IsMainDescUnsaved);
                    
                    else if (CurrentDocument == this.CurrentPassive.DedicatedDocument_SummaryDescription)
                        @Languages.PresentedTextElements["[Passives / Right menu] * Passive summary"].MarkWithUnsavedByCondition(this.CurrentPassive.IsSummaryDescUnsaved);
                    
                    else if (CurrentDocument == this.CurrentPassive.DedicatedDocument_FlavorDescription)
                        @Languages.PresentedTextElements["[Passives / Right menu] * Passive flavor"].MarkWithUnsavedByCondition(this.CurrentPassive.IsFlavorDescUnsaved);
                }
                
                
                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnButtons()
                {
                    @Languages.PresentedTextElements["[Passives / Right menu] * Passive desc"].MarkWithUnsavedByCondition(this.CurrentPassive.IsMainDescUnsaved);
                    @Languages.PresentedTextElements["[Passives / Right menu] * Passive summary"].MarkWithUnsavedByCondition(this.CurrentPassive.IsSummaryDescUnsaved);
                    @Languages.PresentedTextElements["[Passives / Right menu] * Passive flavor"].MarkWithUnsavedByCondition(this.CurrentPassive.IsFlavorDescUnsaved);
                }
                
                
                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs()
                {
                    if (CurrentFile is not null) // Can be called on startup by TextChanged
                    {
                        @Languages.PresentedTextFields["[Passives / Right menu] * Passive name"].MarkWithUnsavedByCondition(this.CurrentPassive.IsNameUnsaved);
                    }
                }








                public override void SaveCurrentDescription(TextDocument CurrentDocument, ref bool CancelDefaultSave)
                {
                    if (@Languages.PresentedTextFields["[Passives / Right menu] * Passive name"].IsFocused)
                    {
                        SaveName(); CancelDefaultSave = true;
                    }
                    else if (CurrentDocument == this.CurrentPassive.DedicatedDocument_MainDescription) this.CurrentPassive.SyncMainDesc();
                    else if (CurrentDocument == this.CurrentPassive.DedicatedDocument_FlavorDescription) this.CurrentPassive.SyncFlavorDesc();
                    else if (CurrentDocument == this.CurrentPassive.DedicatedDocument_SummaryDescription) this.CurrentPassive.SyncSummaryDesc();
                }
                
                
                [LayeredComponent]
                public void SaveName()
                {
                    this.CurrentPassive.SyncName();
                    ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
                    base.SaveCurrentFile_Action();
                }




                public override void UISwitchPrecedingActions()
                {
                    this.RouteDictionary.Clear();

                    foreach (Passive Passive in DeserializedLocalizationData!.DataList.Where(DataListValidator))
                    {
                        this.RouteDictionary[(BigInteger)Passive.ID!] = Passive;
                    }

                    this.SwitchToPassive(this.AvailableIDsList.First());
                }











                #region Integral part of this Editor


                #region Navigation nodes

                public List<BigInteger> AvailableIDsList => [.. this.RouteDictionary.Keys];

                public Dictionary<string, BigInteger> NameAndIDMatches =>
                    this.RouteDictionary.ToDictionarySafe(KeySelector: PassiveKVPair => PassiveKVPair.Value.Name, PassiveKVPair => PassiveKVPair.Key);


                public Dictionary<BigInteger, Passive> RouteDictionary = [];

                public BigInteger CurrentPassiveID;
                public Passive CurrentPassive => this.RouteDictionary[this.CurrentPassiveID];

                protected override void ClearSpecificData()
                {
                    @DataContextDomain.Editor.CurrentPassive = null!;
                    
                    this.RouteDictionary.Clear();
                    this.CurrentPassiveID = 0;
                }

                #endregion


                #region Right menu
                
                [LayeredComponent]
                public void SwitchToPassive(BigInteger TargetPassiveID)
                {
                    this.CurrentPassiveID = TargetPassiveID;

                    base.CheckSwitchIDButtonsAvailability(this.AvailableIDsList.IndexOf(this.CurrentPassiveID), this.AvailableIDsList.Count - 1);

                    #region DataContextDomain values set
                    @DataContextDomain.Editor.CurrentPassive = this.CurrentPassive;
                    #endregion

                    SwitchToCurrentPassiveMainDescription();

                    ChangeAllRightMenuUnsavedChangesMarkers_OnButtons();
                }

                [LayeredComponent]
                public void SwitchToCurrentPassiveMainDescription()
                {
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentPassive.DedicatedDocument_MainDescription;
                    MainWindowInstance.RichTextViews__Passives_COMPOSITION_MainAndSummaryDescsTab.SelectedIndex = 0;
                }

                [LayeredComponent]
                public void SwitchToCurrentPassiveFlavorDescription()
                {
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentPassive.DedicatedDocument_FlavorDescription!;
                    MainWindowInstance.RichTextViews__Passives_COMPOSITION_MainAndSummaryDescsTab.SelectedIndex = 0;
                }

                [LayeredComponent]
                public void SwitchToCurrentPassiveSummaryDescription()
                {
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentPassive.DedicatedDocument_SummaryDescription!;
                    MainWindowInstance.RichTextViews__Passives_COMPOSITION_MainAndSummaryDescsTab.SelectedIndex = 1;
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

        private void RightMenuButtons__Passives_SaveName(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Passives.SaveName();
        }

        private void RightMenuTextBoxes__Passives_NameTextChanged(object Sender, EventArgs Args)
        {
            @EditorModesShelf.Passives.ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
        }


        private void RightMenuButtons__Passives_SwitchToMainDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Passives.SwitchToCurrentPassiveMainDescription();

            if (@EditorModesShelf.Passives.CurrentPassive.FlavorDescription is not null)
            {
                RichTextViews__Passives_MainDescription.BringIntoView();
                RichTextViews__SHARED_DescriptionManualSwitchHighlight(RichTextViews__Passives_MainDescription);
            }
        }
        
        private void RightMenuButtons__Passives_SwitchToFlavorDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Passives.SwitchToCurrentPassiveFlavorDescription();
            RichTextViews__Passives_FlavorDescription.BringIntoView();
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(RichTextViews__Passives_FlavorDescription);
        }
        
        private void RightMenuButtons__Passives_SwitchToSummaryDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Passives.SwitchToCurrentPassiveSummaryDescription();
        }

        #endregion




        #region Fast switch by right click on desc at the rich text view
        
        private void RichTextViews__Passives_FastSwitch_ToPassiveMainDescription(object Sender, MouseButtonEventArgs Args)
        {
            if (@EditorModesShelf.Passives.CurrentPassive.FlavorDescription is not null)
            {
                @EditorModesShelf.Passives.SwitchToCurrentPassiveMainDescription();
                RichTextViews__SHARED_DescriptionFastSwitchHighlight(Sender);
            }
        }
        
        private void RichTextViews__Passives_FastSwitch_ToPassiveFlavorDescription(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Passives.SwitchToCurrentPassiveFlavorDescription();
            RichTextViews__SHARED_DescriptionFastSwitchHighlight(Sender);
        }
        
        #endregion
    }
}