using ICSharpCode.AvalonEdit.Document;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using LCLocalizationInterface.Instruments.Classes;
using static LCLocalizationInterface.LimbusRegistry.@EditorModesShelf.Types;

namespace LCLocalizationInterface.LimbusRegistry
{
    public ref partial struct @EditorModesShelf
    {
        public static readonly ObservationLogsEditorMode ObservationLogs = new();

        public ref partial struct Types
        {
            public class ObservationLogsEditorMode : EditorModeAbstraction<ObservationLog>
            {
                public override EditorModeKey Identifier => EditorModeKey.ObservationLogs;




                protected override Func<ObservationLog, bool> DataListValidator { get; } =

                     ListObject => ListObject.ID is not null && ListObject.ObservationStoriesDictionary is not null && ListObject.ObservationStoriesDictionary.ContainsOneOfTheseKeys(0, 1, 2, 3);


                protected override MainWindowDimensing Dimensing { get; } = new()
                {
                    MaxWidth = 894, MinWidth = 606, MinHeight = 504,
                    Width    = 894, Height   = 677,

                    RichTextViewsHeight             = new GridLength(466),
                    TextEditorAndRichTextViewsWidth = new GridLength(600),

                    RichTextViewsTabIndex     = 5,
                    RightMenuSectionsTabIndex = 5,
                };


                public override List<IntenseStareType3> PresentedRightMenuSytaxedTextInputs => [
                    @Languages.PresentedTextFields["[Observation Logs / Right Menu] * Code Name"],
                    @Languages.PresentedTextFields["[Observation Logs / Right Menu] * Name"],
                ];








                private object SreenshotArea => MainWindowInstance.RichTextViews__ObservationLogs_COMPOSITION_SurfaceScrollViewer.Content;
                public override void ScreenshotRichText()
                {
                    using (new ScreenshotBackgroundSetter(this.SreenshotArea))
                    {
                        (this.SreenshotArea as FrameworkElement)!.RenderImage(ScanPathTemplate.Exform(CurrentFile!.Name, this.CurrentObservationLogID), ScreenshotsUpscale);
                    }
                }


                public override void RefreshRichText()
                {
                    MainWindowInstance.RichTextViews__ObservationLogs_COMPOSITION_LackingDataSign.RefreshRichText();
                    MainWindowInstance.RichTextViews__ObservationLogs_Level1.RefreshRichText();
                    MainWindowInstance.RichTextViews__ObservationLogs_Level2.RefreshRichText();
                    MainWindowInstance.RichTextViews__ObservationLogs_Level3.RefreshRichText();
                }




                public override (bool IsAnyUnsavedChanges, string UnsavedChangesText) CollectUnsavedChanges()
                {
                    string UnsavedChangesString = "";
                    foreach (ObservationLog InspectingObservationLog in this.RouteDictionary_ObservationLogs.Values)
                    {
                        string UnsavedChangesString_SingleObservationLog = "";

                        if (InspectingObservationLog.IsCodeNameUnsaved)
                            UnsavedChangesString_SingleObservationLog += @Languages.VariableData.UnsavedChangesInfo.ObservationLogs.CodeName;

                        if (InspectingObservationLog.IsNameUnsaved)
                            UnsavedChangesString_SingleObservationLog += @Languages.VariableData.UnsavedChangesInfo.ObservationLogs.Name;

                        if (InspectingObservationLog.ObservationStoriesDictionary!.TryGetValue(0, out ObservationLog.ObservationStory? FoundDataLackingLog) && FoundDataLackingLog.IsStoryUnsaved)
                            UnsavedChangesString_SingleObservationLog += @Languages.VariableData.UnsavedChangesInfo.ObservationLogs.LackingData;

                        foreach (ObservationLog.ObservationStory InspectingStory in InspectingObservationLog.ObservationStoriesDictionary!.Values.Where(x => x.Level!.EqualsToOneOf(1, 2, 3)))
                        {
                            if (InspectingStory.IsStoryUnsaved)
                                UnsavedChangesString_SingleObservationLog += @Languages.VariableData.UnsavedChangesInfo.ObservationLogs.ObservationLevel.Extern(InspectingStory.Level);
                        }

                        if (UnsavedChangesString_SingleObservationLog != "")
                        {
                            UnsavedChangesString +=
                                $"{@Languages.VariableData.UnsavedChangesInfo.ObservationLogs.IDHeader.Exform(InspectingObservationLog.ID, InspectingObservationLog.Name, InspectingObservationLog.CodeName)}" +
                                $"{UnsavedChangesString_SingleObservationLog}";
                        }
                    }

                    return (UnsavedChangesString != "", UnsavedChangesString.Trim());
                }




                public override void SwitchIDButtonClick(IDSwitchDirection Direction)
                {
                    int NewIndex = this.AvailableIDsList.IndexOf(this.CurrentObservationLogID) + (Direction == IDSwitchDirection.Forward ? 1 : -1);
                    this.SwitchToObservationLog(this.AvailableIDsList[NewIndex]);
                }


                public override void SwitchIDButtonClick_ToVeryFirstOrLast(IDSwitchDirection Direction)
                {
                    this.SwitchToObservationLog(this.AvailableIDsList[Direction == IDSwitchDirection.Back ? 0 : this.AvailableIDsList.Count - 1]);
                }


                public override void SwitchToObjectByInput(string Input)
                {
                    if (BigInteger.TryParse(Input, out BigInteger TargetID) && this.RouteDictionary_ObservationLogs.ContainsKey(TargetID))
                    {
                        SwitchToObservationLog(TargetID);
                    }
                    else if (this.NameAndIDMatches.TryGetValueCaseInsensitive(Input, out BigInteger FoundMatchingObservationLogIDByName))
                    {
                        SwitchToObservationLog(FoundMatchingObservationLogIDByName!);
                    }
                    else if (this.CodeNameAndIDMatches.TryGetValueCaseInsensitive(Input, out BigInteger FoundMatchingObservationLogIDByCodeName))
                    {
                        SwitchToObservationLog(FoundMatchingObservationLogIDByCodeName!);
                    }
                }








                public override void ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(TextDocument CurrentDocument)
                {
                    string TargetLocalizationID = this.CurrentObservationStoryNumber == 0
                        ?  "[Observation Logs / Right Menu] * Lacking Data"
                        : $"[Observation Logs / Right Menu] * Observation Level {this.CurrentObservationStoryNumber}";

                    @Languages.PresentedTextElements[TargetLocalizationID].MarkWithUnsavedByCondition(this.CurrentObservationStory.IsStoryUnsaved);
                }


                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnButtons()
                {
                    // 0 "level", means "Lacking Data" story
                    @Languages.PresentedTextElements["[Observation Logs / Right Menu] * Lacking Data"].MarkWithUnsavedByCondition(
                        this.CurrentObservationLog.ObservationStoriesDictionary!.TryGetValue(0, out ObservationLog.ObservationStory? FoundLackingDataStory) &&
                        FoundLackingDataStory.IsStoryUnsaved
                    );

                    // 1~3 "level"s, means I~III observation level stories
                    for (int ObservationLevelNumber = 1; ObservationLevelNumber <= 3; ObservationLevelNumber++)
                    {
                        if (this.CurrentObservationLog.ObservationStoriesDictionary!.TryGetValue(ObservationLevelNumber, out ObservationLog.ObservationStory? FoundObservationStory))
                        {
                            @Languages.PresentedTextElements[$"[Observation Logs / Right Menu] * Observation Level {ObservationLevelNumber}"]
                                .MarkWithUnsavedByCondition(FoundObservationStory.IsStoryUnsaved);
                        }
                        else
                        {
                            @Languages.PresentedTextElements[$"[Observation Logs / Right Menu] * Observation Level {ObservationLevelNumber}"].SetDefaultText();
                        }
                    }
                }


                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs()
                {
                    if (CurrentFile is not null) // Can be called on startup by TextChanged
                    {
                        @Languages.PresentedTextFields["[Observation Logs / Right Menu] * Code Name"].MarkWithUnsavedByCondition(this.CurrentObservationLog.IsCodeNameUnsaved);
                        @Languages.PresentedTextFields["[Observation Logs / Right Menu] * Name"].MarkWithUnsavedByCondition(this.CurrentObservationLog.IsNameUnsaved);
                    }
                }


                

                public override void SaveCurrentDescription(TextDocument CurrentDocument, ref bool CancelDefaultSave)
                {
                    if (@Languages.PresentedTextFields["[Observation Logs / Right Menu] * Code Name"].IsFocused)
                    {
                        CancelDefaultSave = true; SaveCodeName();
                    }
                    else if (@Languages.PresentedTextFields["[Observation Logs / Right Menu] * Name"].IsFocused)
                    {
                        CancelDefaultSave = true; SaveLogsName();
                    }
                    else
                    {
                        this.CurrentObservationStory.SyncStoryDescription();
                    }
                }

                [LayeredComponent]
                public void SaveCodeName()
                {
                    this.CurrentObservationLog.SyncCodeName();
                    ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
                    base.SaveCurrentFile_Action();
                }

                [LayeredComponent]
                public void SaveLogsName()
                {
                    this.CurrentObservationLog.SyncName();
                    ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
                    base.SaveCurrentFile_Action();
                }




                public override void UISwitchPrecedingActions()
                {
                    this.RouteDictionary_ObservationLogs.Clear();

                    foreach (ObservationLog ObservationLog in DeserializedLocalizationData!.DataList.Where(DataListValidator))
                    {
                        this.RouteDictionary_ObservationLogs[(BigInteger)ObservationLog.ID!] = ObservationLog;
                    }

                    this.SwitchToObservationLog(this.AvailableIDsList.First());
                }








                #region Integral part of this Editor


                #region Navigation nodes

                public List<BigInteger> AvailableIDsList => [.. this.RouteDictionary_ObservationLogs.Keys];

                public Dictionary<string, BigInteger> NameAndIDMatches =>
                    this.RouteDictionary_ObservationLogs.ToDictionarySafe(KeySelector: KeywordKVPair => KeywordKVPair.Value.Name, KeywordKVPair => KeywordKVPair.Key);

                public Dictionary<string, BigInteger> CodeNameAndIDMatches =>
                    this.RouteDictionary_ObservationLogs.ToDictionarySafe(KeySelector: KeywordKVPair => KeywordKVPair.Value.CodeName, KeywordKVPair => KeywordKVPair.Key);


                public Dictionary<BigInteger, ObservationLog> RouteDictionary_ObservationLogs = [];

                public Dictionary<BigInteger, ObservableDictionary<int, ObservationLog.ObservationStory>> RouteDictionary_ObservationStories
                {
                    get
                    {
                        Dictionary<BigInteger, ObservableDictionary<int, ObservationLog.ObservationStory>> Collected = [];
                        foreach (ObservationLog ObservationLog in this.RouteDictionary_ObservationLogs.Values)
                        {
                            Collected[(BigInteger)ObservationLog.ID!] = ObservationLog.ObservationStoriesDictionary!;
                        }
                        return Collected;
                    }
                }

                public BigInteger CurrentObservationLogID;
                public int CurrentObservationStoryNumber;
                public ObservationLog CurrentObservationLog => this.RouteDictionary_ObservationLogs[this.CurrentObservationLogID];
                public ObservationLog.ObservationStory CurrentObservationStory => this.RouteDictionary_ObservationStories[this.CurrentObservationLogID][this.CurrentObservationStoryNumber];

                protected override void ClearSpecificData()
                {
                    @DataContextDomain.Editor.CurrentObservationLog = null!;
                    @DataContextDomain.Editor.CurrentObservationStory = null!;

                        this.RouteDictionary_ObservationLogs.Clear();
                    _ = this.CurrentObservationLogID
                      = this.CurrentObservationStoryNumber
                      = 0;
                }

                #endregion


                #region Right menu

                [LayeredComponent]
                public void SwitchToObservationLog(BigInteger TargetObservationLogID)
                {
                    this.CurrentObservationLogID = TargetObservationLogID;

                    base.CheckSwitchIDButtonsAvailability(this.AvailableIDsList.IndexOf(this.CurrentObservationLogID), this.AvailableIDsList.Count - 1);

                    #region DataContextDomain values set
                    @DataContextDomain.Editor.CurrentObservationLog = this.CurrentObservationLog;
                    #endregion

                    ChangeAllRightMenuUnsavedChangesMarkers_OnButtons();
                    SetSeparatorsVisibility();

                    SwitchToObservationLogStory(this.RouteDictionary_ObservationStories[this.CurrentObservationLogID].Keys.First());
                }
                
                [LayeredComponent]
                public void SwitchToObservationLogStory(int ObservationLevelNumber_ButActuallyIndex)
                {
                    this.CurrentObservationStoryNumber = ObservationLevelNumber_ButActuallyIndex;

                    #region DataContextDomain values set
                    @DataContextDomain.Editor.CurrentObservationStory = this.CurrentObservationStory;
                    #endregion

                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentObservationStory.DedicatedDocument_Description;
                }

                [LayeredComponent]
                public void SetSeparatorsVisibility()
                {
                    Dictionary<string, StackPanel> TextViews = new()
                    {
                        ["Lacking Data"] = MainWindowInstance.RichTextViews__ObservationLogs_COMPOSITION_TextPanel_LackingData,
                        ["Obs. Level 1"] = MainWindowInstance.RichTextViews__ObservationLogs_COMPOSITION_TextPanel_Level1,
                        ["Obs. Level 2"] = MainWindowInstance.RichTextViews__ObservationLogs_COMPOSITION_TextPanel_Level2,
                        ["Obs. Level 3"] = MainWindowInstance.RichTextViews__ObservationLogs_COMPOSITION_TextPanel_Level3,
                    };

                    #warning Its fine if UIElement extension property 'Visible' marked as unknown (CS1061) ("Preview" C# language version, whatever)
                    /// <see cref="Instruments.WPFTools.set_Visible(UIElement, bool)"/>
                    
                    MainWindowInstance.RichTextViews__ObservationLogs_COMPOSITION_SeparatorBetweenLackingDataAnd1.Visible =
                        TextViews["Lacking Data"].Visible & (TextViews["Obs. Level 1"].Visible | TextViews["Obs. Level 2"].Visible | TextViews["Obs. Level 3"].Visible);

                    MainWindowInstance.RichTextViews__ObservationLogs_COMPOSITION_SeparatorBetween1And2.Visible =
                        TextViews["Obs. Level 1"].Visible & (TextViews["Obs. Level 2"].Visible | TextViews["Obs. Level 3"].Visible);

                    MainWindowInstance.RichTextViews__ObservationLogs_COMPOSITION_SeparatorBetween2And3.Visible =
                        TextViews["Obs. Level 2"].Visible & (TextViews["Obs. Level 3"].Visible);
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

        private void RightMenuButtons__ObservationLogs_SaveCodeName(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.ObservationLogs.SaveCodeName();
        }

        private void RightMenuButtons__ObservationLogs_SaveLogsName(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.ObservationLogs.SaveLogsName();
        }

        private void RightMenuTextBoxes__ObservationLogs_CommonTextChanged(object Sender, EventArgs Args)
        {
            @EditorModesShelf.ObservationLogs.ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
        }


        private void RightMenuButtons__ObservationLogs_SwitchToObservationStory(object Sender, MouseButtonEventArgs Args)
        {
            Button ActualSender = (Sender as Button)!;

            TMProEmitter RelatedDescElement;

            if (ActualSender.Uid == "Lacking Data")
            {
                RelatedDescElement = MainWindowInstance.RichTextViews__ObservationLogs_LackingData;

                @EditorModesShelf.ObservationLogs.SwitchToObservationLogStory(0);
            }
            else
            {
                RelatedDescElement = MainWindowInstance.FindTypeName<TMProEmitter>($"RichTextViews__ObservationLogs_Level{ActualSender.Uid}")!;

                int TargetObservationStoryLevel = int.Parse(ActualSender.Uid);
                @EditorModesShelf.ObservationLogs.SwitchToObservationLogStory(TargetObservationStoryLevel);
            }



            RelatedDescElement.BringIntoView();
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(RelatedDescElement);
        }

        #endregion




        #region Fast switch by right click on desc at the rich text view
        
        private void RichTextViews__ObservationLogs_FastSwitch_ToObservationStory(object Sender, MouseButtonEventArgs Args)
        {
            TMProEmitter ActualSender = (Sender as TMProEmitter)!;

            if (ActualSender.Uid == "Lacking Data")
            {
                @EditorModesShelf.ObservationLogs.SwitchToObservationLogStory(0);
            }
            else
            {
                int TargetObservationStoryNumber = int.Parse(ActualSender.Uid);
                @EditorModesShelf.ObservationLogs.SwitchToObservationLogStory(TargetObservationStoryNumber);
            }


            ActualSender.BringIntoView();
            RichTextViews__SHARED_DescriptionFastSwitchHighlight(ActualSender);
        }
        
        #endregion
    }
}