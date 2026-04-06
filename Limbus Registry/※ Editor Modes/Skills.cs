using ICSharpCode.AvalonEdit.Document;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using static LCLocalizationInterface.LimbusRegistry.@EditorModesShelf.Types;
using static LCLocalizationInterface.LimbusRegistry.InputRichTextFormatter;

namespace LCLocalizationInterface.LimbusRegistry
{
    public ref partial struct @EditorModesShelf
    {
        public static readonly SkillsEditorMode Skills = new();

        public ref partial struct Types
        {
            public class SkillsEditorMode : EditorModeAbstraction<Skill>
            {
                public override EditorModeKey Identifier => EditorModeKey.Skills;




                protected override Func<Skill, bool> DataListValidator { get; } =

                    ListObject => ListObject.ID is not null & ListObject.UptieLevels.All(UptieLevel => UptieLevel.UptieNumber is not null);


                protected override MainWindowDimensing Dimensing { get; } = new()
                {
                    MaxWidth = 1000, MinWidth = 713, MinHeight = 425,
                    Width    = 1000, Height   = 594.4,

                    RichTextViewsHeight             = new GridLength(387),
                    TextEditorAndRichTextViewsWidth = new GridLength(706),

                    RichTextViewsTabIndex     = 1,
                    RightMenuSectionsTabIndex = 1,
                };


                public override List<IntenseStareType3> PresentedRightMenuSytaxedTextInputs => [
                    @Languages.PresentedTextFields["[Skills / Right menu] * Skill main name"],
                    @Languages.PresentedTextFields["[Skills / Right menu] * E.G.O Skill abnormality name"]
                ];








                private object SreenshotArea => MainWindowInstance.RichTextViews__Skills_COMPOSITION_SurfaceScrollViewer.Content;
                public override void ScreenshotRichText()
                {
                    using (new ScreenshotBackgroundSetter(this.SreenshotArea))
                    {
                        (this.SreenshotArea as FrameworkElement)!.RenderImage(ScanPathTemplate.Exform(CurrentFile!.Name, this.CurrentSkillID), ScreenshotsUpscale);
                    }
                }


                public override void RefreshRichText()
                {
                    MainWindowInstance.RichTextViews__Skills_COMPOSITION_SkillNameReplica.RefreshNameRichText();
                    MainWindowInstance.RichTextViews__Skills_MainDescription.RefreshRichText();
                    MainWindowInstance.RichTextViews__Skills_FlavorDescription.RefreshRichText();
                    foreach (TMProEmitter CoinDesc in MainWindow.GeneratedCoinDescriptions.Values.Where(x => x.Visibility == Visibility.Visible))
                    {
                        CoinDesc.RefreshRichText();
                    }
                }




                public override (bool IsAnyUnsavedChanges, string UnsavedChangesText) CollectUnsavedChanges()
                {
                    string UnsavedChangesString = "";
                    foreach (KeyValuePair<BigInteger, Skill> InspectingSkill in this.RouteDictionary_SkillObjects)
                    {
                        string UnsavedChangesString_SingleSkill = "";
                        foreach (Skill.Uptie InspectingUptie in InspectingSkill.Value.UptieLevels)
                        {
                            string UnsavedChangesString_SingleUptie = "";
                            if (InspectingUptie.IsMainNameUnsaved)
                                UnsavedChangesString_SingleUptie += @Languages.VariableData.UnsavedChangesInfo.Skills.MainName;

                            if (InspectingUptie.IsEGOAbnormalityNameUnsaved)
                                UnsavedChangesString_SingleUptie += @Languages.VariableData.UnsavedChangesInfo.Skills.EGOAbnormalityName;
                            
                            if (InspectingUptie.IsMainDescUnsaved)
                                UnsavedChangesString_SingleUptie += @Languages.VariableData.UnsavedChangesInfo.Skills.MainDesc;
                            
                            if (InspectingUptie.IsFlavorDescUnsaved)
                                UnsavedChangesString_SingleUptie += @Languages.VariableData.UnsavedChangesInfo.Skills.FlavorDesc;

                            if (InspectingUptie.Coins is not null)
                            {
                                int CoinCounter = 1;
                                foreach (Skill.Uptie.Coin Coin in InspectingUptie.Coins)
                                {
                                    if (Coin.CoinDescriptions is not null && Coin.CoinDescriptions.Any(x => x.IsMainDescUnsaved | x.IsSummaryDescUnsaved))
                                    {
                                        UnsavedChangesString_SingleUptie += @Languages.VariableData.UnsavedChangesInfo.Skills.Coin.Extern(CoinCounter);
                                    }
                                    CoinCounter++;
                                }
                            }

                            if (UnsavedChangesString_SingleUptie != "")
                            {
                                UnsavedChangesString_SingleSkill +=
                                    $"{@Languages.VariableData.UnsavedChangesInfo.Skills.UptieLevel.Extern(InspectingUptie.UptieNumber!)}" +
                                    $"{UnsavedChangesString_SingleUptie}";
                            }
                        }

                        if (UnsavedChangesString_SingleSkill != "")
                        {
                            UnsavedChangesString +=
                                $"{@Languages.VariableData.UnsavedChangesInfo.Skills.IDHeader.Exform(InspectingSkill.Key, InspectingSkill.Value.UptieLevels.First().Name)}" +
                                $"{UnsavedChangesString_SingleSkill}";
                        }
                    }

                    return (UnsavedChangesString != "", UnsavedChangesString.Trim());
                }



                
                public override void SwitchIDButtonClick(IDSwitchDirection Direction)
                {
                    int NewIndex = this.AvailableIDsList.IndexOf(this.CurrentSkillID) + (Direction == IDSwitchDirection.Forward ? 1 : -1);
                    SwitchToSkill(this.AvailableIDsList[NewIndex]);
                }
                
                
                public override void SwitchIDButtonClick_ToVeryFirstOrLast(IDSwitchDirection Direction)
                {
                    SwitchToSkill(this.AvailableIDsList[Direction == IDSwitchDirection.Back ? 0 : this.AvailableIDsList.Count - 1]);
                }
                
                
                public override void SwitchToObjectByInput(string Input)
                {
                    if (BigInteger.TryParse(Input, out BigInteger TargetSkillID) && this.RouteDictionary_UptieLevels.ContainsKey(TargetSkillID))
                    {
                        SwitchToSkill(TargetSkillID);
                    }
                    else if (this.MainNameAndIDMatches.TryGetValueCaseInsensitive(Input, out BigInteger FoundMatchingSkillIDByMainName))
                    {
                        SwitchToSkill(FoundMatchingSkillIDByMainName);
                    }
                    else if (this.AbnormalityNameAndIDMatches.TryGetValueCaseInsensitive(Input, out BigInteger FoundMatchingSkillIDByEGOAbnormalityName))
                    {
                        SwitchToSkill(FoundMatchingSkillIDByEGOAbnormalityName);
                    }
                }








                public override void ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(TextDocument CurrentDocument)
                {
                    if (CurrentDocument == this.CurrentUptie.DedicatedDocument_MainDescription)
                    {
                        @Languages.PresentedTextElements["[Skills / Right menu] * Skill main desc"].MarkWithUnsavedByCondition(this.CurrentUptie.IsMainDescUnsaved);
                    }
                    else if (CurrentDocument == this.CurrentUptie.DedicatedDocument_FlavorDescription)
                    {
                        @Languages.PresentedTextElements["[Skills / Right menu] * Skill flavor desc"].MarkWithUnsavedByCondition(this.CurrentUptie.IsFlavorDescUnsaved);
                    }
                    else
                    {
                        @Languages.PresentedTextElements[$"[Skills / Right menu] * Skill Coin {this.CurrentCoinNumber}"].MarkWithUnsavedByCondition(this.CurrentCoin.CoinDescriptions!.Any(CoinDesc => CoinDesc.IsMainDescUnsaved | CoinDesc.IsSummaryDescUnsaved));
                        @Languages.PresentedTextElements[ "[Skills / Right menu] * Skill Coin desc number"].MarkWithUnsavedByCondition(this.CurrentCoinDescription.IsMainDescUnsaved, ExtraExtern: this.CurrentCoinDescriptionNumber);
                        @Languages.PresentedTextElements[ "[Skills / Right menu] * Skill Coin summary desc"].MarkWithUnsavedByCondition(this.CurrentCoinDescription.IsSummaryDescUnsaved);
                    }
                }


                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnButtons()
                {
                    @Languages.PresentedTextElements["[Skills / Right menu] * Skill main desc"].MarkWithUnsavedByCondition(this.CurrentUptie.IsMainDescUnsaved);
                    @Languages.PresentedTextElements["[Skills / Right menu] * Skill flavor desc"].MarkWithUnsavedByCondition(this.CurrentUptie.IsFlavorDescUnsaved);

                    for (int CoinNumber = 1; CoinNumber <= 10; CoinNumber++)
                    {
                        if (this.CurrentUptie.Coins?.ElementAtOrDefault(CoinNumber - 1) is not null)
                        {
                            @Languages.PresentedTextElements[$"[Skills / Right menu] * Skill Coin {CoinNumber}"]
                                .MarkWithUnsavedByCondition(this.CurrentUptie.Coins[CoinNumber - 1].CoinDescriptions?.Any(x => x.IsMainDescUnsaved | x.IsSummaryDescUnsaved));
                        }
                        else
                        {
                            @Languages.PresentedTextElements[$"[Skills / Right menu] * Skill Coin {CoinNumber}"].SetDefaultText();
                        }
                    }
                }


                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs()
                {
                    if (this.DeserializedLocalizationData is not null) // Can be called on startup by TextChanged
                    {
                        @Languages.PresentedTextFields["[Skills / Right menu] * Skill main name"].MarkWithUnsavedByCondition(this.CurrentUptie.IsMainNameUnsaved);
                        @Languages.PresentedTextFields["[Skills / Right menu] * E.G.O Skill abnormality name"].MarkWithUnsavedByCondition(this.CurrentUptie.IsEGOAbnormalityNameUnsaved);
                    }
                }








                public override void WindowPreviewMouseDown(MouseButtonEventArgs Args, ref bool CancelIDSwitchByMouseXButtons)
                {
                    // Uptie switch on Left Ctrl + Forward/Back mouse buttons
                    // XButton1 = Back, XButton2 = Forward
                    if (Args.ChangedButton.EqualsToOneOf(MouseButton.XButton1, MouseButton.XButton2) &&
                        Keyboard.IsKeyDown(Key.LeftShift)
                    ) {
                        CancelIDSwitchByMouseXButtons = true;

                        List<int> AvailableUpties = [.. this.RouteDictionary_UptieLevels[this.CurrentSkillID].Keys];
                        int TargetUptieIndex = AvailableUpties.IndexOf(this.CurrentUptieNumber) + (Args.ChangedButton == MouseButton.XButton1 ? -1 : +1);

                        if (TargetUptieIndex != -1 & TargetUptieIndex <= AvailableUpties.Count - 1)
                        {
                            SwitchToSkill(this.CurrentSkillID, AvailableUpties[TargetUptieIndex]);
                        }
                    }
                }
                
                
                public override void WindowPreviewKeyDown(KeyEventArgs Args, ref bool CancelIDSwitchByArrowButtons)
                {
                    // Uptie switch on Left Shift + Left/Right keyboard buttons
                    if (Args.Key.EqualsToOneOf(Key.Left, Key.Right) && Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        if (Keyboard.FocusedElement is not ICSharpCode.AvalonEdit.Editing.TextArea) // Prevent left shift + left/right button text selection breaking
                        {
                            MouseButton Direction = MouseButton.Middle;
                            if      (Args.Key == Key.Left ) Direction = MouseButton.XButton1;
                            else if (Args.Key == Key.Right) Direction = MouseButton.XButton2;

                            if (Direction != MouseButton.Middle)
                            {
                                this.WindowPreviewMouseDown(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, Direction), ref CancelIDSwitchByArrowButtons);
                            }
                        }
                    }
                }








                public override void SaveCurrentDescription(TextDocument CurrentDocument, ref bool CancelDefaultSave)
                {
                    if (@Languages.PresentedTextFields["[Skills / Right menu] * Skill main name"].IsFocused)
                    {
                        SaveName(); CancelDefaultSave = true;
                    }
                    else if (@Languages.PresentedTextFields["[Skills / Right menu] * E.G.O Skill abnormality name"].IsFocused)
                    {
                        SaveEGOAbnormalityName(); CancelDefaultSave = true;
                    }
                    else
                    {
                        if (CurrentDocument == this.CurrentUptie.DedicatedDocument_MainDescription)
                        {
                            this.CurrentUptie.SyncMainDesc();
                        }
                        else if (CurrentDocument == this.CurrentUptie.DedicatedDocument_FlavorDescription)
                        {
                            this.CurrentUptie.SyncFlavorDesc();
                        }
                        else
                        {
                            if (CurrentDocument == this.CurrentCoinDescription.DedicatedDocument_MainDescription)
                            {
                                this.CurrentCoinDescription.SyncMainDesc();
                            }
                            else
                            {
                                this.CurrentCoinDescription.SyncSummaryDesc();
                            }
                        }
                    }

                    // -> base.SaveCurrentFile_Action() if CancelDefaultSave is still false
                }
                
                
                [LayeredComponent]
                public void SaveName()
                {
                    // Change name Skill name in all other Upties when Left Shift is pressed
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        foreach (Skill.Uptie Uptie in this.CurrentSkill.UptieLevels)
                        {
                            Uptie.KeylessDedicatedDocument_MainName.Text = @Languages.PresentedTextFields["[Skills / Right menu] * Skill main name"].Text;
                            Uptie.SyncMainName();
                        }
                    }
                    else { this.CurrentUptie.Name = @Languages.PresentedTextFields["[Skills / Right menu] * Skill main name"].Text; }

                    this.ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
                    base.SaveCurrentFile_Action();
                }
                
                
                [LayeredComponent]
                public void SaveEGOAbnormalityName()
                {
                    // Change E.G.O Skill abnormality name in all other Upties when Left Shift is pressed
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        foreach (Skill.Uptie Uptie in this.CurrentSkill.UptieLevels)
                        {
                            Uptie.KeylessDedicatedDocument_EGOAbnormalityName!.Text = @Languages.PresentedTextFields["[Skills / Right menu] * E.G.O Skill abnormality name"].Text;
                            Uptie.SyncEGOAbnormalityName();
                        }
                    }
                    else { this.CurrentUptie.EGOAbnormalityName = @Languages.PresentedTextFields["[Skills / Right menu] * E.G.O Skill abnormality name"].Text; }

                    this.ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
                    base.SaveCurrentFile_Action();
                }




                public override void UISwitchPrecedingActions()
                {
                    this.RouteDictionary_SkillObjects.Clear();

                    foreach (Skill Skill in this.DeserializedLocalizationData!.DataList.Where(this.DataListValidator))
                    {
                        this.RouteDictionary_SkillObjects[(BigInteger)Skill.ID!] = Skill;
                    }

                    SwitchToSkill(this.AvailableIDsList.First());
                }











                #region Integral part of this Editor


                #region Navigation nodes

                public List<BigInteger> AvailableIDsList => [.. this.RouteDictionary_SkillObjects.Keys];

                public Dictionary<string, BigInteger> MainNameAndIDMatches =>
                    this.RouteDictionary_SkillObjects
                    .ToDictionarySafe(SkillKVPair => SkillKVPair.Value.UptieLevels.First().Name, SkillKVPair => SkillKVPair.Key);

                public Dictionary<string, BigInteger> AbnormalityNameAndIDMatches =>
                    this.RouteDictionary_SkillObjects
                    .Where(SkillKVPair => SkillKVPair.Value.UptieLevels.First().EGOAbnormalityName is not null)
                    .ToDictionarySafe(SkillKVPair => SkillKVPair.Value.UptieLevels.First().EGOAbnormalityName!, SkillKVPair => SkillKVPair.Key);


                public Dictionary<BigInteger, Skill> RouteDictionary_SkillObjects = [];

                /// <summary>
                /// <u>Created at the moment of asking</u>, means not suitable for things like RouteDictionary_UptiesDictionary[ID][Uptie] = ..., can be used only for condition checks
                /// </summary>
                public Dictionary<BigInteger, Dictionary<int, Skill.Uptie>> RouteDictionary_UptieLevels
                {
                    get
                    {
                        Dictionary<BigInteger, Dictionary<int, Skill.Uptie>> Collected = [];
                        foreach (Skill Skill in this.RouteDictionary_SkillObjects.Values)
                        {
                            Collected[(BigInteger)Skill.ID!] = Skill.UptieLevels.ToDictionarySafe(KeySelector: UptieLevel => (int)UptieLevel.UptieNumber!, ValueSelector: UptieLevel => UptieLevel);
                        }
                        return Collected;
                    }
                }


                public BigInteger CurrentSkillID;
                public Skill CurrentSkill => this.RouteDictionary_SkillObjects[this.CurrentSkillID];

                public int CurrentUptieNumber;
                public Skill.Uptie CurrentUptie => this.RouteDictionary_UptieLevels[this.CurrentSkillID][this.CurrentUptieNumber];

                public int CurrentCoinNumber;
                public Skill.Uptie.Coin CurrentCoin => this.CurrentUptie.Coins![this.CurrentCoinNumber - 1];

                public int CurrentCoinDescriptionNumber;
                public Skill.Uptie.Coin.CoinDesc CurrentCoinDescription => this.CurrentCoin.CoinDescriptions![this.CurrentCoinDescriptionNumber - 1];

                protected override void ClearSpecificData()
                {
                    @DataContextDomain.Editor.CurrentSkill = null!;
                    @DataContextDomain.Editor.CurrentUptie = null!;

                        this.RouteDictionary_SkillObjects.Clear();
                    _ = this.CurrentSkillID
                      = this.CurrentUptieNumber
                      = this.CurrentCoinNumber
                      = this.CurrentCoinDescriptionNumber
                      = 0;
                }

                #endregion


                #region Right menu
                
                [LayeredComponent]
                public void SwitchToSkill(BigInteger TargetSkillID, int UptieLevelNumber = -1)
                {
                    if (UptieLevelNumber == -1) UptieLevelNumber = this.RouteDictionary_UptieLevels[TargetSkillID].Keys.First();

                    this.CurrentSkillID = TargetSkillID;
                    this.CurrentUptieNumber = UptieLevelNumber;

                    #region DataContextDomain values set
                    @DataContextDomain.Editor.CurrentSkill = this.RouteDictionary_SkillObjects[this.CurrentSkillID];
                    @DataContextDomain.Editor.CurrentUptie = this.RouteDictionary_UptieLevels[this.CurrentSkillID][this.CurrentUptieNumber];
                    #endregion

                    /// <see cref="MainWindow.JsonManaging_SkillOptionalAffinitySelector_SelectionChanged"/>
                    MainWindow.ManuallyChangingSelectedOptionalAffinity = true;
                    {
                        MainWindowInstance.SkillAffinitySelector.SelectedIndex = this.CurrentUptie.OptionalAffinity switch
                        {
                            "Wrath" => 0, "Lust" => 1, "Sloth" => 2, "Gluttony" => 3, "Gloom" => 4, "Pride" => 5, "Envy" => 6, _ => 7
                        };
                    }
                    MainWindow.ManuallyChangingSelectedOptionalAffinity = false;

                    if (LoadedConfiguration.PreviewSettings.Base.EnableSkillNamesReplication)
                    {
                        ChangeSkillNameReplicaAppearance();
                    }

                    base.CheckSwitchIDButtonsAvailability(this.AvailableIDsList.IndexOf(this.CurrentSkillID), this.AvailableIDsList.Count - 1);

                    SwitchToCurrentSkillMainDescription();


                    ReCheckRightMenuUptieButtonsAppearance();
                    ChangeAllRightMenuUnsavedChangesMarkers_OnButtons();
                }

                [LayeredComponent]
                public void SwitchToCurrentSkillMainDescription()
                {
                    ResetCoinInfoView();

                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentUptie.DedicatedDocument_MainDescription;

                    this.CurrentCoinNumber = -1;
                    this.CurrentCoinDescriptionNumber = -1;
                }

                [LayeredComponent]
                public void SwitchToCurrentSkillFlavorDescription()
                {
                    ResetCoinInfoView();
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentUptie.DedicatedDocument_FlavorDescription!;

                    this.CurrentCoinNumber = -1;
                    this.CurrentCoinDescriptionNumber = -1;
                }

                [LayeredComponent]
                public void SwitchToCoinDescriptionMainDesc(int TargetCoinNumber, int TargetCoinDescriptionNumber = 1)
                {
                    this.CurrentCoinNumber = TargetCoinNumber;
                    this.CurrentCoinDescriptionNumber = TargetCoinDescriptionNumber;

                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentCoinDescription.DedicatedDocument_MainDescription;

                    CheckCurrentCoinDescriptionsButtonsAvailability();

                    @Languages.ExternElement(UID: "[Skills / Right menu] * Skill Coin descs title", ExternObject: this.CurrentCoinNumber);
                    @Languages.PresentedTextElements["[Skills / Right menu] * Skill Coin desc number"].MarkWithUnsavedByCondition(this.CurrentCoinDescription.IsMainDescUnsaved, ExtraExtern: this.CurrentCoinDescriptionNumber);
                    @Languages.PresentedTextElements["[Skills / Right menu] * Skill Coin summary desc"].MarkWithUnsavedByCondition(this.CurrentCoinDescription.IsSummaryDescUnsaved);
                }

                [LayeredComponent]
                public void SwitchToCurrentCoinDescriptionSummaryDesc()
                {
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentCoinDescription.DedicatedDocument_SummaryDescription!;
                }


                #region Servo things
                
                [LayeredComponent]
                private void ResetCoinInfoView()
                {
                    _ = MainWindowInstance.RightMenu_Skills_SwitchToCoinDesc_Back.IsEnabled
                      = MainWindowInstance.RightMenu_Skills_SwitchToCoinDesc_Forward.IsEnabled
                      = MainWindowInstance.RightMenu_Skills_SwitchToCoinSummaryDesc.IsEnabled
                      = MainWindowInstance.RightMenu_Skills_CurrentCoinDesc_Display.IsEnabled
                      = false;

                    @Languages.PresentedTextElements["[Skills / Right menu] * Skill Coin descs title"].SetDefaultText(ExtraExtern: @Languages.VariableData.InsertionsDefaultValue);
                    @Languages.PresentedTextElements["[Skills / Right menu] * Skill Coin desc number"].SetDefaultText(ExtraExtern: @Languages.VariableData.InsertionsDefaultValue);
                    @Languages.PresentedTextElements["[Skills / Right menu] * Skill Coin summary desc"].SetDefaultText();
                }
                
                [LayeredComponent]
                private void CheckCurrentCoinDescriptionsButtonsAvailability()
                {
                    int CurrentDescIndex = this.CurrentCoinDescriptionNumber - 1;

                    MainWindowInstance.RightMenu_Skills_CurrentCoinDesc_Display.IsEnabled = true;
                    MainWindowInstance.RightMenu_Skills_SwitchToCoinDesc_Back.IsEnabled = this.CurrentCoin.CoinDescriptions!.ElementAtOrDefault(CurrentDescIndex - 1) is not null;
                    MainWindowInstance.RightMenu_Skills_SwitchToCoinDesc_Forward.IsEnabled = this.CurrentCoin.CoinDescriptions!.ElementAtOrDefault(CurrentDescIndex + 1) is not null;
                    MainWindowInstance.RightMenu_Skills_SwitchToCoinSummaryDesc.IsEnabled = this.CurrentCoin.CoinDescriptions![CurrentDescIndex].SummaryDescription is not null;
                }

                /// <summary>Hide/Show "Uptie Tier" text because there isn't enough space for it with 6 Uptie switch buttons</summary>
                [LayeredComponent]
                public void ReCheckRightMenuUptieButtonsAppearance()
                {
                    MainWindowInstance.UptieSwitch_1.IsEnabled = RouteDictionary_UptieLevels[this.CurrentSkillID].ContainsKey(1);
                    MainWindowInstance.UptieSwitch_2.IsEnabled = RouteDictionary_UptieLevels[this.CurrentSkillID].ContainsKey(2);
                    MainWindowInstance.UptieSwitch_3.IsEnabled = RouteDictionary_UptieLevels[this.CurrentSkillID].ContainsKey(3);
                    MainWindowInstance.UptieSwitch_4.IsEnabled = RouteDictionary_UptieLevels[this.CurrentSkillID].ContainsKey(4);
                    MainWindowInstance.UptieSwitch_5.IsEnabled = RouteDictionary_UptieLevels[this.CurrentSkillID].ContainsKey(5);
                    MainWindowInstance.UptieSwitch_5.Visibility = RouteDictionary_UptieLevels[this.CurrentSkillID].ContainsOneOfTheseKeys(5, 6) ? Visibility.Visible : Visibility.Collapsed;
                    MainWindowInstance.UptieSwitch_6.Visibility = RouteDictionary_UptieLevels[this.CurrentSkillID].ContainsKey(6) ? Visibility.Visible : Visibility.Collapsed;

                    if (MainWindowInstance.RightMenuUIElements__Skills_UptieSwitchButtons.Children.Cast<UptieLevelButton>().Where(x => x.Visibility == Visibility.Visible).Count() == 6)
                    {
                        MainWindowInstance.RightMenuUIElements__Skills_UptieSwitchButtons_ParentGrid.Width = 280.6;
                        MainWindowInstance.RightMenuUIElements__Skills_UptieSwitchButtons.Margin = new Thickness(0, 2, 0, 0);
                    }
                    else
                    {
                        MainWindowInstance.RightMenuUIElements__Skills_UptieSwitchButtons_ParentGrid.Width = double.NaN;
                        MainWindowInstance.RightMenuUIElements__Skills_UptieSwitchButtons.Margin = new Thickness(0, 2, 5, 0);
                    }
                }

                /// <summary>
                /// Change properties of <see cref="MainWindow.RichTextViews__Skills_COMPOSITION_SkillNameReplica"/> UI element based on info found at <see cref="@SkillsData.ReadedSkillsData"/> dictionary (<see cref="LimbusRegistry.SkillNameReplicaUIElement"/> UserControl)
                /// </summary>
                [LayeredComponent]
                public void ChangeSkillNameReplicaAppearance()
                {
                    var VisualElement = MainWindowInstance.RichTextViews__Skills_COMPOSITION_SkillNameReplica;

                    #region Set defaults
                    VisualElement.Icon = new BitmapImage();
                    VisualElement.Rank = 1;
                    VisualElement.AttackWeight = 1;

                    VisualElement.LevelText = "??";
                    VisualElement.BasePower = "?";
                    VisualElement.CoinPower = "+?";

                    VisualElement.Affinity = AffinityName.None;
                    VisualElement.DamageType = DamageType.None;
                    VisualElement.SkillType  = SkillType.Attack;

                    VisualElement.AffinityAndRank = "None|1";
                    VisualElement.Coins = "Regular";
                    #endregion


                    #region Set actual if can find data
                    if (@SkillsData.ReadedSkillsData   .TryGetValue(this.CurrentSkillID,     out var SkillData) &&
                        SkillData.UptieLevelsDictionary.TryGetValue(this.CurrentUptieNumber, out var CurrentUptieData)
                    ) {
                        VisualElement.Rank = SkillData.Rank;
                        VisualElement.AttackWeight = CurrentUptieData.AttackWeight ?? 1;

                        VisualElement.LevelText = $"{60 + CurrentUptieData.LevelCorrection}";
                        VisualElement.BasePower = $"{CurrentUptieData.BasePower}";
                        VisualElement.CoinPower = $"{CurrentUptieData.CoinMathOperator}{CurrentUptieData.CoinPower}";
                        
                        // Anyway
                        Try(delegate () { VisualElement.Affinity   = Enum.Parse<AffinityName>(CurrentUptieData.Affinity!  ); });
                        Try(delegate () { VisualElement.DamageType = Enum.Parse<DamageType>  (CurrentUptieData.DamageType!); });
                        Try(delegate () { VisualElement.SkillType  = Enum.Parse<SkillType>   (CurrentUptieData.SkillType! ); });

                        VisualElement.Coins = CurrentUptieData.CoinsSequence!;



                        #region Skill icon determination
                        string OriginallyPresumedSkillIconID = $"{CurrentUptieData.IconID ?? SkillData.ID}";
                        string PresumedSkillIconID = OriginallyPresumedSkillIconID;


                        // Try format E.G.O Skill ID from localization file to ID of the actual image from assets
                        // 2021011 -> 20210_awaken_profile, 2021021 -> 20210_erosion_profile  as example
                        if (this.CurrentUptie.EGOAbnormalityName is not null)
                        {
                            if (PresumedSkillIconID.EndsWith("11"))
                            {
                                PresumedSkillIconID = PresumedSkillIconID[..^2] + "_awaken_profile";
                            }
                            else if (PresumedSkillIconID.EndsWith("21"))
                            {
                                PresumedSkillIconID = PresumedSkillIconID[..^2] + "_erosion_profile";
                            }
                        }


                        // Try get icon
                        BitmapImage AcquiredSkillIcon = ImageDictionaries.SkillIcons[PresumedSkillIconID];


                        // If presumed E.G.O Skill ID formatted above is still leads to unknown image, then try get it by default ID just as regular Skill
                        if (AcquiredSkillIcon == ImageDictionaries.UnknownSpriteImage & this.CurrentUptie.EGOAbnormalityName is not null)
                        {
                            AcquiredSkillIcon = ImageDictionaries.SkillIcons[OriginallyPresumedSkillIconID];
                        }

                        // Then apply placeholder if icon is still unknown after all steps
                        if (AcquiredSkillIcon == ImageDictionaries.UnknownSpriteImage)
                        {
                            AcquiredSkillIcon = BitmapFromResource($"UI/Limbus/Skills/Default Icons/{VisualElement.Affinity}/{VisualElement.SkillType}.png");
                        }

                        // Finally set icon
                        VisualElement.Icon = AcquiredSkillIcon;
                        #endregion
                    }
                    else
                    {
                        VisualElement.AffinityAndRank = "None|-1"; // Alt icon for unknown Skills
                    }
                    #endregion
                }
                
                #endregion

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

        private void RightMenuTextBoxes__Skills_CommonTextChanged(object Sender, EventArgs Args) => @EditorModesShelf.Skills.ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();

        private void RightMenuButtons__Skills_SaveName(object Sender, MouseButtonEventArgs Args) => @EditorModesShelf.Skills.SaveName();
        
        private void RightMenuButtons__Skills_SaveEGOAbnormalityName(object Sender, MouseButtonEventArgs Args) => @EditorModesShelf.Skills.SaveEGOAbnormalityName();


        private void RightMenuButtons__Skills_SwitchToUptie(object Sender, MouseButtonEventArgs Args)
        {
            UptieLevelButton ActualSender = (Sender as UptieLevelButton)!;
            @EditorModesShelf.Skills.SwitchToSkill(@EditorModesShelf.Skills.CurrentSkillID, ActualSender.IndicatedNumber);
        }
        
        private void RightMenuButtons__Skills_SwitchToMainDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Skills.SwitchToCurrentSkillMainDescription();
            MainWindowInstance.RichTextViews__Skills_MainDescription.BringIntoView();
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(MainWindowInstance.RichTextViews__Skills_MainDescription);
        }
        
        private void RightMenuButtons__Skills_SwitchToFlavorDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Skills.SwitchToCurrentSkillFlavorDescription();
            MainWindowInstance.RichTextViews__Skills_FlavorDescription.BringIntoView();
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(MainWindowInstance.RichTextViews__Skills_FlavorDescription);
        }


        private void RightMenuButtons__Skills_SwitchToCoinDesc(object Sender, MouseButtonEventArgs Args)
        {
            int TargetCoinNumber = int.Parse((Sender as Button)!.Uid);
            @EditorModesShelf.Skills.SwitchToCoinDescriptionMainDesc(TargetCoinNumber);

            TMProEmitter TargetDesc = GeneratedCoinDescriptions[(TargetCoinNumber, 1, false)];
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(TargetDesc);
            TargetDesc.BringIntoView();
        }
        
        private void RightMenuButtons__Skills_SwitchToPrevOrNextCoinDesc(object Sender, MouseButtonEventArgs Args)
        {
            var Direction = Enum.Parse<@EditorModesShelf.IDSwitchDirection>((Sender as Button)!.Uid);

            int TargetCoinDescNumber = @EditorModesShelf.Skills.CurrentCoinDescriptionNumber + (Direction == @EditorModesShelf.IDSwitchDirection.Back ? -1 : 1);
            @EditorModesShelf.Skills.SwitchToCoinDescriptionMainDesc(@EditorModesShelf.Skills.CurrentCoinNumber, TargetCoinDescNumber);

            TMProEmitter TargetDesc = GeneratedCoinDescriptions[(@EditorModesShelf.Skills.CurrentCoinNumber, TargetCoinDescNumber, false)];
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(TargetDesc);
            TargetDesc.BringIntoView();
        }


        private void RightMenuButtons__Skills_SwitchToCurrentCoinDescriptionSummaryDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Skills.SwitchToCurrentCoinDescriptionSummaryDesc();

            TMProEmitter TargetDesc = GeneratedCoinDescriptions[(@EditorModesShelf.Skills.CurrentCoinNumber, @EditorModesShelf.Skills.CurrentCoinDescriptionNumber, true)];
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(TargetDesc);
            TargetDesc.BringIntoView();
        }
        
        private void RightMenuButtons__Skills_SwitchToCurrentCoinDescriptionMainDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Skills.SwitchToCoinDescriptionMainDesc(@EditorModesShelf.Skills.CurrentCoinNumber, @EditorModesShelf.Skills.CurrentCoinDescriptionNumber);

            TMProEmitter TargetDesc = GeneratedCoinDescriptions[(@EditorModesShelf.Skills.CurrentCoinNumber, @EditorModesShelf.Skills.CurrentCoinDescriptionNumber, false)];
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(TargetDesc);
            TargetDesc.BringIntoView();
        }


        #region Manual json files managing

        private void RightMenu_Skills_SwitchToCoinDesc_Forward_ContextMenuOpening(object Sender, ContextMenuEventArgs Args)
        {
            Args.Handled = @EditorModesShelf.Skills.CurrentCoinNumber == -1;
        }
        
        private void RightMenuButtons__Skills_SwitchToCurrentCoinDescriptionSummaryDesc_ContextMenuOpening(object Sender, ContextMenuEventArgs Args)
        {
            Args.Handled = @EditorModesShelf.Skills.CurrentCoinNumber == -1;
        }
        
        #endregion


        #endregion




        #region Fast switch by right click on desc at the rich text view
        
        private static readonly Regex BindingPathCoinIdentifier = new(@"DataContextDomain.Editor.CurrentUptie.Coins\[(\d+)\].CoinDescriptions\[(\d+)\].DedicatedDocument_(Main|Summary)Description");
        
        private static (int CoinNumber, int CoinDescriptionNumber, bool IsSummaryDesc) IdentifyCoinPosition(TMProEmitter Sender)
        {
            Match BindingPath = BindingPathCoinIdentifier.Match(Sender.GetBindingExpression(TMProEmitter.RichTextProperty).ParentBinding.Path.Path);
            return (CoinNumber: int.Parse(BindingPath.Groups[1].Value) + 1, CoinDescriptionNumber: int.Parse(BindingPath.Groups[2].Value) + 1, IsSummaryDesc: BindingPath.Groups[3].Value == "Summary");
        }
        
        private void RichTextViews__Skills_FastSwitch_ToCoinDescriptionMainOrSummaryDesc(object Sender, MouseButtonEventArgs Args)
        {
            (int CoinNumber, int CoinDescriptionNumber, bool IsSummaryDesc) = IdentifyCoinPosition((Sender as TMProEmitter)!);
            @EditorModesShelf.Skills.SwitchToCoinDescriptionMainDesc(CoinNumber, CoinDescriptionNumber);
            if (IsSummaryDesc) @EditorModesShelf.Skills.SwitchToCurrentCoinDescriptionSummaryDesc();
            RichTextViews__SHARED_DescriptionFastSwitchHighlight(Sender);
        }


        private void RichTextViews__Skills_FastSwitch_ToSkillMainDescription(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Skills.SwitchToCurrentSkillMainDescription();
            RichTextViews__SHARED_DescriptionFastSwitchHighlight(Sender);
        }
        
        private void RichTextViews__Skills_FastSwitch_ToSkillFlavorDescription(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.Skills.SwitchToCurrentSkillFlavorDescription();
            RichTextViews__SHARED_DescriptionFastSwitchHighlight(Sender);
        }
        
        #endregion




        #region Manual skill coins construction (Because in its pure form it will take about 20% of the entire XAML document (10 Coins × 30 TMProEmitters with descs))
        
        /// <summary>
        /// <see cref="TMProEmitter"/> objects created by <see cref="Skills_SetupCoinsView"/> (Key is <b>(Coin number, Desc number, is Summary desc)</b>)
        /// </summary>
        public static ReadOnlyDictionary<(int CoinNumber, int DescNumber, bool IsSummaryDesc), TMProEmitter> GeneratedCoinDescriptions { get; private set; } = ReadOnlyDictionary<(int, int, bool), TMProEmitter>.Empty;

        private void Skills_SetupCoinsView()
        {
            Dictionary<(int CoinNumber, int DescNumber, bool IsSummaryDesc), TMProEmitter> GeneratedCoinDescriptions_There = [];

            for (int CoinNumber = 1; CoinNumber <= 10; CoinNumber++)
            {
                Grid CoinMainGrid = new() { Margin = new Thickness(0, 3, 0, 0) };

                Image CoinImage = new()
                {
                    Source = BitmapFromResource($"UI/Limbus/Skills/Coins/Coin {CoinNumber}.png"),
                    Width = 48,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                VirtualizingStackPanel CoinDescsStackPanel = new()
                {
                    Margin = new Thickness(55, 15, 0, 0)
                };


                CoinMainGrid.Children.Add(CoinImage);
                CoinMainGrid.Children.Add(CoinDescsStackPanel);


                for (int CoinDescNumber = 1; CoinDescNumber <= 30; CoinDescNumber++) // Max coin descs amount
                {
                    TMProEmitter MainCoinDesc = new()
                    {
                        Uid = $"Coin {CoinNumber}, Desc {CoinDescNumber}",
                        FontType = LimbusFontTypes.Context,
                        TextProcessingMode = RichTextFormat.Skills,
                        FontSize = 20, LineHeight = 27
                    };
                    MainCoinDesc.SetBinding(TMProEmitter.RichTextProperty, new Binding()
                    {
                        /// <see cref="Entanglement.EntanglementModel.DataContextDomain"/>
                        Path = new PropertyPath($"DataContextDomain.Editor.CurrentUptie.Coins[{CoinNumber - 1}].CoinDescriptions[{CoinDescNumber - 1}].DedicatedDocument_MainDescription.Text"),
                        Mode = BindingMode.OneWay, FallbackValue = null
                    });
                    MainCoinDesc.PreviewMouseRightButtonDown += RichTextViews__Skills_FastSwitch_ToCoinDescriptionMainOrSummaryDesc;

                    TMProEmitter SummaryCoinDesc = new()
                    {
                        Uid = $"Coin {CoinNumber}, Desc {CoinDescNumber} (Summary)",
                        FontType = LimbusFontTypes.Context,
                        TextProcessingMode = RichTextFormat.Skills,
                        FontSize = 16, LineHeight = 21,
                        Margin = new Thickness(4, 0, 4, 8)
                    };
                    SummaryCoinDesc.SetBinding(TMProEmitter.RichTextProperty, new Binding()
                    {
                        // ※ Entanglement Model.cs
                        Path = new PropertyPath($"DataContextDomain.Editor.CurrentUptie.Coins[{CoinNumber - 1}].CoinDescriptions[{CoinDescNumber - 1}].DedicatedDocument_SummaryDescription.Text"),
                        Mode = BindingMode.OneWay, FallbackValue = null
                    });
                    SummaryCoinDesc.PreviewMouseRightButtonDown += RichTextViews__Skills_FastSwitch_ToCoinDescriptionMainOrSummaryDesc;



                    static void HideWholeCoinIfAllDescsIsEmpty(TMProEmitter Sender, string? NewRichText)
                    {
                        VirtualizingStackPanel CoinDescsParentStackPanel = (Sender.Parent as VirtualizingStackPanel)!;
                        Grid CoinParentGrid = (CoinDescsParentStackPanel.Parent as Grid)!;

                        CoinParentGrid.Visibility = CoinDescsParentStackPanel.Children.Cast<TMProEmitter>().Select(TMProEmitter => TMProEmitter.RichText).All(string.IsNullOrEmpty)
                            ? Visibility.Collapsed
                            : Visibility.Visible;
                    }
                    MainCoinDesc.RichTextSetted    += HideWholeCoinIfAllDescsIsEmpty;
                    SummaryCoinDesc.RichTextSetted += HideWholeCoinIfAllDescsIsEmpty;



                    CoinDescsStackPanel.Children.Add(MainCoinDesc);
                    CoinDescsStackPanel.Children.Add(SummaryCoinDesc);

                    GeneratedCoinDescriptions_There[(CoinNumber, CoinDescNumber, false)] = MainCoinDesc;
                    GeneratedCoinDescriptions_There[(CoinNumber, CoinDescNumber, true )] = SummaryCoinDesc;



                    MainCoinDesc.RichText = null!; // Trigger HideWholeCoinIfAllDescsIsEmpty to hide each Coin by default
                }


                RichTextViews__Skills_COMPOSITION_Coins.Children.Add(CoinMainGrid);
            }

            GeneratedCoinDescriptions = new ReadOnlyDictionary<(int CoinNumber, int DescNumber, bool IsSummaryDesc), TMProEmitter>(GeneratedCoinDescriptions_There);
        }
        
        #endregion
    }
}

namespace LCLocalizationInterface.LimbusRegistry
{
    public class UptieLevelButton : Button
    {
        public int IndicatedNumber { get => (int)GetValue(IndicatedNumberProperty); set => SetValue(IndicatedNumberProperty, value); }
        public static readonly DependencyProperty IndicatedNumberProperty = RegisterProperty<UptieLevelButton, int>(DefaultValue: -1);

        public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
        public static readonly DependencyProperty IsSelectedProperty = RegisterProperty<UptieLevelButton, bool>(DefaultValue: false);
    }
}