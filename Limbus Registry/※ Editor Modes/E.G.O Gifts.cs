using ICSharpCode.AvalonEdit.Document;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using static LCLocalizationInterface.LimbusRegistry.@EditorModesShelf.Types;

namespace LCLocalizationInterface.LimbusRegistry
{
    public ref partial struct @EditorModesShelf
    {
        public static readonly EGOGiftsEditorMode EGOGifts = new();

        public ref partial struct Types
        {
            public class EGOGiftsEditorMode : EditorModeAbstraction<EGOGift>
            {
                public override EditorModeKey Identifier => EditorModeKey.EGOGifts;




                protected override Func<EGOGift, bool> DataListValidator { get; } =

                    ListObject => ListObject.ID is not null;


                protected override MainWindowDimensing Dimensing { get; } = new()
                {
                    MaxWidth = 804, MinWidth = 516.3, MinHeight = 461,
                    Width    = 804, Height   = 631,

                    RichTextViewsHeight             = new GridLength(421),
                    TextEditorAndRichTextViewsWidth = new GridLength(510),

                    RichTextViewsTabIndex     = 4,
                    RightMenuSectionsTabIndex = 4,
                };


                public override List<IntenseStareType3> PresentedRightMenuSytaxedTextInputs => [
                    @Languages.PresentedTextFields["[E.G.O Gifts / Right Menu] * E.G.O Gift Name"]
                ];








                private object SreenshotArea => MainWindowInstance.RichTextViews__EGOGifts_COMPOSITION_SurfaceScrollViewer.Content;
                public override void ScreenshotRichText()
                {
                    using (new ScreenshotBackgroundSetter(this.SreenshotArea))
                    {
                        _ = ScanPathTemplate.Exform(CurrentFile!.Name, this.CurrentEGOGiftID);
                        (this.SreenshotArea as FrameworkElement)!.RenderImage(ScanPathTemplate.Exform(CurrentFile!.Name, this.CurrentEGOGiftID), ScreenshotsUpscale);
                    }
                }


                public override void RefreshRichText()
                {
                    MainWindowInstance.EGOGiftVisualView_Name.RefreshRichText();
                    MainWindowInstance.RichTextViews__EGOGifts_MainDescription.RefreshRichText();
                    MainWindowInstance.RichTextViews__EGOGifts_FlavorDescription.RefreshRichText();
                    for (int SimpleDescNumber = 1; SimpleDescNumber <= 10; SimpleDescNumber++)
                    {
                        ((MainWindowInstance.RichTextViews__EGOGifts_COMPOSITION_SimpleDescriptionsTab.Children[SimpleDescNumber - 1] as Grid)!.Children[0] as TMProEmitter)!.RefreshRichText();
                    }
                }




                public override (bool IsAnyUnsavedChanges, string UnsavedChangesText) CollectUnsavedChanges()
                {
                    string UnsavedChangesString = "";
                    foreach (EGOGift InspectingEGOGift in this.RouteDictionary.Values)
                    {
                        string UnsavedChangesString_SingleEGOGift = "";

                        if (InspectingEGOGift.IsNameUnsaved)
                            UnsavedChangesString_SingleEGOGift += @Languages.VariableData.UnsavedChangesInfo.EGOGifts.Name;
                            
                        if (InspectingEGOGift.IsMainDescUnsaved)
                            UnsavedChangesString_SingleEGOGift += @Languages.VariableData.UnsavedChangesInfo.EGOGifts.MainDesc;
                            
                        if (InspectingEGOGift.IsFlavorDescUnsaved)
                            UnsavedChangesString_SingleEGOGift += @Languages.VariableData.UnsavedChangesInfo.EGOGifts.FlavorDesc;

                        if (InspectingEGOGift.SimpleDescriptions is not null)
                        {
                            int SimpleDescCounter = 1;
                            foreach (EGOGift.SimpleDescription SimpleDesc in InspectingEGOGift.SimpleDescriptions)
                            {
                                if (SimpleDesc.IsDescUnsaved)
                                    UnsavedChangesString_SingleEGOGift += @Languages.VariableData.UnsavedChangesInfo.EGOGifts.SimpleDesc.Extern(SimpleDescCounter);

                                SimpleDescCounter++;
                            }
                        }

                        if (UnsavedChangesString_SingleEGOGift != "")
                        {
                            UnsavedChangesString +=
                                $"{@Languages.VariableData.UnsavedChangesInfo.EGOGifts.IDHeader.Exform(InspectingEGOGift.ID, InspectingEGOGift.Name)}" +
                                $"{UnsavedChangesString_SingleEGOGift}";
                        }
                    }

                    return (UnsavedChangesString != "", UnsavedChangesString.Trim());
                }




                public override void SwitchIDButtonClick(IDSwitchDirection Direction)
                {
                    int NewIndex = this.AvailableIDsList.IndexOf(this.CurrentEGOGiftID) + (Direction == IDSwitchDirection.Forward ? 1 : -1);
                    this.SwitchToEGOGift(this.AvailableIDsList[NewIndex]);
                }
                
                
                public override void SwitchIDButtonClick_ToVeryFirstOrLast(IDSwitchDirection Direction)
                {
                    this.SwitchToEGOGift(this.AvailableIDsList[Direction == IDSwitchDirection.Back ? 0 : this.AvailableIDsList.Count - 1]);
                }
                
                
                public override void SwitchToObjectByInput(string Input)
                {
                    if (BigInteger.TryParse(Input, out BigInteger TargetID) && this.RouteDictionary.ContainsKey(TargetID))
                    {
                        SwitchToEGOGift(TargetID);
                    }
                    else if (this.NameAndIDMatches.TryGetValueCaseInsensitive(Input, out BigInteger FoundMatchingEGOGiftIDByName))
                    {
                        SwitchToEGOGift(FoundMatchingEGOGiftIDByName!);
                    }
                }








                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnButtons()
                {
                    @Languages.PresentedTextElements["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"].MarkWithUnsavedByCondition(this.CurrentEGOGift.IsMainDescUnsaved);
                    @Languages.PresentedTextElements["[E.G.O Gifts / Right Menu] * E.G.O Gift Flavor"].MarkWithUnsavedByCondition(this.CurrentEGOGift.IsFlavorDescUnsaved);
                    for (int SimpleDescNumber = 1; SimpleDescNumber <= 10; SimpleDescNumber++)
                    {
                        if (this.CurrentEGOGift.SimpleDescriptions?.ElementAtOrDefault(SimpleDescNumber - 1) is not null)
                        {
                            @Languages.PresentedTextElements[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"]
                                .MarkWithUnsavedByCondition(this.CurrentEGOGift.SimpleDescriptions[SimpleDescNumber - 1].IsDescUnsaved);
                        }
                        else
                        {
                            @Languages.PresentedTextElements[$"[E.G.O Gifts / Right Menu] * Simple Desc {SimpleDescNumber}"].SetDefaultText();
                        }
                    }
                }
                
                
                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs()
                {
                    if (CurrentFile is not null) // Can be called on startup by TextChanged
                    {
                        @Languages.PresentedTextFields["[E.G.O Gifts / Right Menu] * E.G.O Gift Name"].MarkWithUnsavedByCondition(this.CurrentEGOGift.IsNameUnsaved);
                    }
                }


                public override void ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(TextDocument CurrentDocument)
                {
                    if (CurrentDocument == this.CurrentEGOGift.DedicatedDocument_MainDescription)
                    {
                        @Languages.PresentedTextElements["[E.G.O Gifts / Right Menu] * E.G.O Gift Desc"].MarkWithUnsavedByCondition(this.CurrentEGOGift.IsMainDescUnsaved);
                    }
                    else if (CurrentDocument == this.CurrentEGOGift.DedicatedDocument_FlavorDescription)
                    {
                        @Languages.PresentedTextElements["[E.G.O Gifts / Right Menu] * E.G.O Gift Flavor"].MarkWithUnsavedByCondition(this.CurrentEGOGift.IsFlavorDescUnsaved);
                    }
                    else
                    {
                        @Languages.PresentedTextElements[$"[E.G.O Gifts / Right Menu] * Simple Desc {this.CurrentSimpleDescNumber}"]
                            .MarkWithUnsavedByCondition(this.CurrentSimpleDesc.IsDescUnsaved);
                    }
                }
                
                
                
                




                public override void SaveCurrentDescription(TextDocument CurrentDocument, ref bool CancelDefaultSave)
                {
                    if (@Languages.PresentedTextFields["[E.G.O Gifts / Right Menu] * E.G.O Gift Name"].IsFocused)
                    {
                        SaveName(); CancelDefaultSave = true;
                    }
                    else if (CurrentDocument == this.CurrentEGOGift.DedicatedDocument_MainDescription) this.CurrentEGOGift.SyncMainDesc();
                    else if (CurrentDocument == this.CurrentEGOGift.DedicatedDocument_FlavorDescription) this.CurrentEGOGift.SyncFlavorDesc();
                    else this.CurrentSimpleDesc.SyncDesc();
                }
                
                [LayeredComponent]
                public void SaveName()
                {
                    this.CurrentEGOGift.SyncName();
                    ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
                    base.SaveCurrentFile_Action();
                }




                public override void UISwitchPrecedingActions()
                {
                    this.RouteDictionary.Clear();

                    foreach (EGOGift EGOGift in DeserializedLocalizationData!.DataList.Where(DataListValidator))
                    {
                        this.RouteDictionary[(BigInteger)EGOGift.ID!] = EGOGift;
                    }

                    UpdateInternalNavigationInfo();

                    SwitchToEGOGift(this.AvailableIDsList.First());
                }
                
                [LayeredComponent]
                public void UpdateInternalNavigationInfo(bool IsOverwriting = false)
                {
                    if (IsOverwriting)
                    {
                        foreach (EGOGift EGOGift in this.RouteDictionary.Values)
                        {
                            EGOGift.UpgradeLevel = 1;
                            EGOGift.UpgradeLevelsAssociativeIDs.Clear();
                        }
                    }

                    foreach (BigInteger EGOGiftID in this.AvailableIDsList)
                    {
                        //                                     1                          2
                        // Base ID = 9001,   Upgrade level 2 = 19001,   Upgrade level 3 = 29001
                        BigInteger ExpectedID_UpgradeLevel2 = BigInteger.Parse($"1{EGOGiftID}");
                        BigInteger ExpectedID_UpgradeLevel3 = BigInteger.Parse($"2{EGOGiftID}");


                        if (this.RouteDictionary.TryGetValue(ExpectedID_UpgradeLevel2, out EGOGift? FoundUpgradeLevel2EGOGift))
                        {
                            this.RouteDictionary[EGOGiftID].UpgradeLevelsAssociativeIDs.Add(EGOGiftID);
                            this.RouteDictionary[EGOGiftID].UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel2);

                            FoundUpgradeLevel2EGOGift.UpgradeLevelsAssociativeIDs.Add(EGOGiftID);
                            FoundUpgradeLevel2EGOGift.UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel2);

                            FoundUpgradeLevel2EGOGift.UpgradeLevel = 2;

                            if (this.RouteDictionary.TryGetValue(ExpectedID_UpgradeLevel3, out EGOGift? FoundUpgradeLevel3EGOGift))
                            {
                                this.RouteDictionary[EGOGiftID].UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel3);

                                this.RouteDictionary[ExpectedID_UpgradeLevel2].UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel3);

                                FoundUpgradeLevel3EGOGift.UpgradeLevelsAssociativeIDs.Add(EGOGiftID);
                                FoundUpgradeLevel3EGOGift.UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel2);
                                FoundUpgradeLevel3EGOGift.UpgradeLevelsAssociativeIDs.Add(ExpectedID_UpgradeLevel3);

                                FoundUpgradeLevel3EGOGift.UpgradeLevel = 3;
                            }
                        }
                    }
                }








                #region Integral part of this Editor


                #region Navigation nodes

                public List<BigInteger> AvailableIDsList => [.. this.RouteDictionary.Keys];

                public Dictionary<string, BigInteger> NameAndIDMatches =>
                    this.RouteDictionary.ToDictionarySafe(KeySelector: EGOGiftKVPair => EGOGiftKVPair.Value.Name, EGOGiftKVPair => EGOGiftKVPair.Key);


                public Dictionary<BigInteger, EGOGift> RouteDictionary = [];

                public BigInteger CurrentEGOGiftID;
                public EGOGift CurrentEGOGift => this.RouteDictionary[this.CurrentEGOGiftID];

                public int CurrentSimpleDescNumber;
                public EGOGift.SimpleDescription CurrentSimpleDesc => this.CurrentEGOGift.SimpleDescriptions![this.CurrentSimpleDescNumber - 1];

                protected override void ClearSpecificData()
                {
                    @DataContextDomain.Editor.CurrentEGOGift = null!;

                        this.RouteDictionary.Clear();
                    _ = this.CurrentEGOGiftID
                      = this.CurrentSimpleDescNumber
                      = 0;
                }

                #endregion


                #region Right menu

                [LayeredComponent]
                public void SwitchToEGOGift(BigInteger TargetEGOGiftID)
                {
                    this.CurrentEGOGiftID = TargetEGOGiftID;

                    base.CheckSwitchIDButtonsAvailability(this.AvailableIDsList.IndexOf(this.CurrentEGOGiftID), this.AvailableIDsList.Count - 1);

                    #region DataContextDomain values set
                    @DataContextDomain.Editor.CurrentEGOGift = this.CurrentEGOGift;
                    #endregion

                    UpdateEGOGiftVisualView();

                    SwitchToCurrentEGOGiftMainDescription();

                    ChangeAllRightMenuUnsavedChangesMarkers_OnButtons();
                }

                [LayeredComponent]
                public void SwitchToCurrentEGOGiftMainDescription()
                {
                    this.CurrentSimpleDescNumber = -1;
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentEGOGift.DedicatedDocument_MainDescription;
                    MainWindowInstance.RichTextViews__EGOGifts_COMPOSITION_MainAndSimpleDesriptionsTab.SelectedIndex = 0;
                }

                [LayeredComponent]
                public void SwitchToCurrentEGOGiftFlavorDescription()
                {
                    this.CurrentSimpleDescNumber = -1;
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentEGOGift.DedicatedDocument_FlavorDescription!;
                    MainWindowInstance.RichTextViews__EGOGifts_COMPOSITION_MainAndSimpleDesriptionsTab.SelectedIndex = 0;
                }

                [LayeredComponent]
                public void SwitchToCurrentEGOGiftSimpleDescription(int TargetSimpleDescriptionNumber)
                {
                    this.CurrentSimpleDescNumber = TargetSimpleDescriptionNumber;
                    MainWindowInstance.LimbusJsonTextEditor.Document = this.CurrentEGOGift.SimpleDescriptions![TargetSimpleDescriptionNumber - 1].DedicatedDocument_Description;
                    MainWindowInstance.RichTextViews__EGOGifts_COMPOSITION_MainAndSimpleDesriptionsTab.SelectedIndex = 1;
                    MainWindowInstance.RichTextViews__EGOGifts_COMPOSITION_SimpleDescriptionsTab.SelectedIndex = TargetSimpleDescriptionNumber - 1;
                }

                #endregion


                [LayeredComponent]
                public void UpdateEGOGiftVisualView()
                {
                    if (ImageDictionaries.LoadedEGOGiftsDisplayInfo.TryGetValue(this.CurrentEGOGiftID, out var EGOGiftInfo))
                    {
                        MainWindowInstance.EGOGiftVisualView_MainIcon.Source = EGOGiftInfo.TryGetImage();

                        MainWindowInstance.EGOGiftVisualView_Tier.Text = EGOGiftInfo.Tier switch
                        {
                            "1" => "I",
                            "2" => "II",
                            "3" => "III",
                            "4" => "IV",
                            "5" => "V",
                            _ => ""
                        };

                        if (EGOGiftInfo.Tier == "EX")
                        {
                            MainWindowInstance.EGOGiftVisualView_Tier.Visibility = Visibility.Collapsed;
                            MainWindowInstance.EGOGiftVisualView_EXTier.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MainWindowInstance.EGOGiftVisualView_Tier.Visibility = Visibility.Visible;
                            MainWindowInstance.EGOGiftVisualView_EXTier.Visibility = Visibility.Collapsed;
                        }

                        if (EGOGiftInfo.Keyword != "-")
                        {
                            BitmapImage Icon = EGOGiftInfo.Keyword.EqualsToOneOf("Blunt", "Pierce", "Slash")
                                ? BitmapFromResource($"UI/Limbus/E.G.O Gifts/Damage Type Icons/{EGOGiftInfo.Keyword}.png")
                                : ImageDictionaries.KeywordImages[EGOGiftInfo.Keyword switch
                                  {
                                      "Burn"    => "Combustion",
                                      "Bleed"   => "Laceration",
                                      "Tremor"  => "Vibration",
                                      "Poise"   => "Breath",
                                      "Charge"  => "Charge",
                                      "Rupture" => "Burst",
                                      "Sinking" => "Sinking",
                                      _ => EGOGiftInfo.Keyword
                                  }];
                            MainWindowInstance.EGOGiftVisualView_Keyword.Source = Icon;
                            MainWindowInstance.EGOGiftVisualView_Keyword.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MainWindowInstance.EGOGiftVisualView_Keyword.Visibility = Visibility.Collapsed;
                        }

                        MainWindowInstance.EGOGiftVisualView_NameBackground.Source = BitmapFromResource($"UI/Limbus/E.G.O Gifts/Affinity-Colored E.G.O Gifts Name Backgrounds/{EGOGiftInfo.Affinity}.png");

                        MainWindowInstance.EGOGiftVisualView_UpgradeLevel2_OnIcon.Visibility
                            = this.CurrentEGOGift.UpgradeLevel == 2 ? Visibility.Visible : Visibility.Collapsed;
                        MainWindowInstance.EGOGiftVisualView_UpgradeLevel3_OnIcon.Visibility
                            = this.CurrentEGOGift.UpgradeLevel == 3 ? Visibility.Visible : Visibility.Collapsed;

                        MainWindowInstance.EGOGiftVisualView_UpgradeLevel2Border.Visibility
                          = this.CurrentEGOGift.UpgradeLevel >= 2 ? Visibility.Visible : Visibility.Collapsed;
                        MainWindowInstance.EGOGiftVisualView_UpgradeLevel3Border.Visibility
                          = this.CurrentEGOGift.UpgradeLevel == 3 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        MainWindowInstance.EGOGiftVisualView_MainIcon.Source = ImageDictionaries.UnknownSpriteImage;

                        _ = MainWindowInstance.EGOGiftVisualView_UpgradeLevel2_OnIcon.Visibility
                          = MainWindowInstance.EGOGiftVisualView_UpgradeLevel2Border.Visibility
                          = MainWindowInstance.EGOGiftVisualView_UpgradeLevel3_OnIcon.Visibility
                          = MainWindowInstance.EGOGiftVisualView_UpgradeLevel3Border.Visibility
                          = MainWindowInstance.EGOGiftVisualView_Tier.Visibility
                          = MainWindowInstance.EGOGiftVisualView_EXTier.Visibility
                          = MainWindowInstance.EGOGiftVisualView_Keyword.Visibility
                          = Visibility.Collapsed;

                        MainWindowInstance.EGOGiftVisualView_NameBackground.Source = BitmapFromResource($"UI/Limbus/E.G.O Gifts/Affinity-Colored E.G.O Gifts Name Backgrounds/None.png");
                    }
                }


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
        
        private void RightMenuButtons__EGOGifts_SaveName(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.EGOGifts.SaveName();
        }

        private void RightMenuTextBoxes__EGOGifts_NameTextChanged(object Sender, EventArgs Args)
        {
            @EditorModesShelf.EGOGifts.ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
        }

        
        private void RightMenuButtons__EGOGifts_SwitchToMainDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.EGOGifts.SwitchToCurrentEGOGiftMainDescription();

            if (@EditorModesShelf.EGOGifts.CurrentEGOGift.FlavorDescription is not null)
            {
                RichTextViews__EGOGifts_MainDescription.BringIntoView();
                RichTextViews__SHARED_DescriptionManualSwitchHighlight(RichTextViews__EGOGifts_MainDescription);
            }
        }
        
        private void RightMenuButtons__EGOGifts_SwitchToFlavorDesc(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.EGOGifts.SwitchToCurrentEGOGiftFlavorDescription();
            RichTextViews__EGOGifts_FlavorDescription.BringIntoView();
            RichTextViews__SHARED_DescriptionManualSwitchHighlight(RichTextViews__EGOGifts_FlavorDescription);
        }
        
        private void RightMenuButtons__EGOGifts_SwitchToSimpleDesc(object Sender, MouseButtonEventArgs Args)
        {
            int TargetSimpleDescNumber = int.Parse((Sender as Button)!.Uid);
            @EditorModesShelf.EGOGifts.SwitchToCurrentEGOGiftSimpleDescription(TargetSimpleDescNumber);
        }
        
        #endregion




        #region Fast switch by right click on desc at the rich text view
        
        private void RichTextViews__EGOGifts_FastSwitch_ToEGOGiftMainDescription(object Sender, MouseButtonEventArgs Args)
        {
            if (@EditorModesShelf.EGOGifts.CurrentEGOGift.FlavorDescription is not null)
            {
                @EditorModesShelf.EGOGifts.SwitchToCurrentEGOGiftMainDescription();
                RichTextViews__SHARED_DescriptionFastSwitchHighlight(Sender);
            }
        }

        private void RichTextViews__EGOGifts_FastSwitch_ToEGOGiftFlavorDescription(object Sender, MouseButtonEventArgs Args)
        {
            @EditorModesShelf.EGOGifts.SwitchToCurrentEGOGiftFlavorDescription();
            RichTextViews__SHARED_DescriptionFastSwitchHighlight(Sender);
        }

        #endregion




        #region Interactive things from limbus ui visualization
        
        private void RichTextViews__EGOGifts_COMPOSITION_SwitchToAssociatedUpgradeLevel(object Sender, MouseButtonEventArgs Args)
        {
            int TargetUpgradeLevelNumber = int.Parse((Sender as EGOGiftUpgradeLevelSwitch)!.Uid);
            if (@EditorModesShelf.EGOGifts.CurrentEGOGift.UpgradeLevelsAssociativeIDs.Count > 0)
            {
                @EditorModesShelf.EGOGifts.SwitchToEGOGift(@EditorModesShelf.EGOGifts.CurrentEGOGift.UpgradeLevelsAssociativeIDs[TargetUpgradeLevelNumber - 1]);
            }
        }
        
        #endregion




        #region Manual simple descs construction (For MainWindow.RichTextViews__EGOGifts_COMPOSITION_SimpleDescriptionsTab)
        
        private void EGOGifts_SetupSimpleDescsView()
        {
            for (int CoinDescNumber = 1; CoinDescNumber <= 10; CoinDescNumber++)
            {
                TMProEmitter SimpleDesc = new()
                {
                    FontType = LimbusFontTypes.Context,
                    TextProcessingMode = InputRichTextFormatter.RichTextFormat.EGOGifts,
                    FontSize = 18.5, LineHeight = 24.9,
                };
                SimpleDesc.SetBinding(TMProEmitter.RichTextProperty, new Binding()
                {
                    /// <see cref="Entanglement.EntanglementModel.DataContextDomain"/>
                    Path = new PropertyPath($"DataContextDomain.Editor.CurrentEGOGift.SimpleDescriptions[{CoinDescNumber - 1}].DedicatedDocument_Description.Text"),
                    Mode = BindingMode.OneWay, FallbackValue = null
                });
                RichTextViews__EGOGifts_COMPOSITION_SimpleDescriptionsTab.Children.Add(new Grid() { Children = { SimpleDesc } });
            }
        }

        #endregion
    }
}

namespace LCLocalizationInterface.LimbusRegistry
{
    public class EGOGiftUpgradeLevelSwitch : Button
    {
        public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
        public static readonly DependencyProperty IsSelectedProperty = RegisterProperty<EGOGiftUpgradeLevelSwitch, bool>(DefaultValue: false);
    }
}