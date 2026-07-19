using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using LCLocalizationInterface.LimbusRegistry.JsonTypes.Specific;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage.ImageInfoJsonFile.TextColumns_PROP;

namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public partial class PreviewCreatorPage : Page
    {
        private void OpenContentSelectionDialog(object Sender, RoutedEventArgs Args)
        {
            ColumnElementContentSelectorWindow SelectionDialog = ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance;
            
            if (Args is SelectionChangedEventArgs ColumnTextElementsSelectionArgs && ColumnTextElementsSelectionArgs.AddedItems.Count > 0)
            {
                string[] Context = (ColumnTextElementsSelectionArgs.AddedItems[0] as ComboBoxItem)!.Uid.Split(", ");
                (string ColumnNumber, string Type) = (Context[0], Context[1]);

                FirstColumnItemsSelector_ComboBox.SelectedIndex = -1;
                SecondColumnItemsSelector_ComboBox.SelectedIndex = -1;

                SelectionDialog.SummaryInfoAdditionTarget = null;
                SelectionDialog.CurrentTargetColumn = ColumnNumber == "1" ? TextColumn_1 : TextColumn_2;
                SelectionDialog.OKButtonView.SelectedIndex = ColumnNumber == "1" ? 0 : 1;
                SelectionDialog.CurrentSelectorView.SelectedIndex = Type switch
                {
                    "Skill" => 0, "Passive" => 1, "Keyword" => 2
                };
            }
            else if (Sender is Button SummaryElementAddButton)
            {
                SelectionDialog.SummaryInfoAdditionTarget = SummaryElementAddButton.Uid;
                SelectionDialog.CurrentSelectorView.SelectedIndex = SummaryElementAddButton.Uid.Contains("Skill") ? 0 : 1;
                SelectionDialog.OKButtonView.SelectedIndex = SummaryElementAddButton.Uid switch
                {
                    "Skill 1" => 2,
                    "Skill 2" => 3,
                    "Skill 3" => 4,
                    "Defense Skill" => 5,
                    "Passive" => 6,
                };
                SelectionDialog.CurrentTargetColumn = null!;
            }

            SelectionDialog.BeginFadeShowing();
            SelectionDialog.WindowState = WindowState.Normal;
            SelectionDialog.Focus();
        }












        
        private string RandomUID(int Length = 5) => new([.. Enumerable.Repeat("ABCDEFGHIJKЙLMNOPQRSTUVWXYZ0123456789", Length).Select(s => s[Random.Shared.Next(s.Length)])]);

        private ColumnTextElementContainer SetupNewTextElementContainer(ColumnTextElementContainer CreatedColumnElement, TextElementsColumn TargetColumn, bool DoColumnsJsonDataReEnumeration = true, bool IsAddingSummaryPassive = false)
        {
            // When RelatedJsonData was created from 'Column Element content selector' window
            if (CreatedColumnElement.RelatedJsonData.UID == "") CreatedColumnElement.RelatedJsonData.UID = RandomUID();
            CreatedColumnElement.Uid = CreatedColumnElement.RelatedJsonData.UID;

            Slider SignaturesOffsetValueSource = IsAddingSummaryPassive == false
                ? TargetColumn == TextColumn_1
                    ? VC_FirstColumnSignaturesOffset
                    : VC_SecondColumnSignaturesOffset
                : VC_PassiveSignaturesOffsetOnSummaryView;

            CreatedColumnElement.SetBinding(ColumnTextElementContainer.SignatureText_HOffsetProperty, new Binding()
            {
                Source = SignaturesOffsetValueSource,
                Path = new PropertyPath(nameof(Slider.Value))
            });

            return CreatedColumnElement;
        }

        /// <summary>
        /// Give UID and create Bindings for <paramref name="CreatedColumnElement"/> properties based on its RelatedJsonData and <paramref name="TargetColumn"/>, then add it to the <paramref name="TargetColumn"/>.
        /// </summary>
        /// <param name="DoColumnsJsonDataReEnumeration">Must be <see langword="false"/> when this method is called at the moment of image info loading to not suddenly break everything</param>
        public void AddTextElementToColumn(TextElementsColumn TargetColumn, ColumnTextElementContainer CreatedColumnElement, bool DoColumnsJsonDataReEnumeration = true, bool IsAddingSummaryPassive = false)
        {
            SetupNewTextElementContainer(CreatedColumnElement, TargetColumn, DoColumnsJsonDataReEnumeration, IsAddingSummaryPassive);

            TargetColumn.Children.Add(CreatedColumnElement);

            if (DoColumnsJsonDataReEnumeration) ReEnumerateColumnItemsJsonData();
        }

        public void AddSkillToSummarySkillsTable(int ColumnIndex, ColumnTextElementContainer CreatedColumnElement, bool DoColumnsJsonDataReEnumeration = true)
        {
            SetupNewTextElementContainer(CreatedColumnElement, null!, DoColumnsJsonDataReEnumeration, false);

            SummarySkillsTable.PlaceSkillAt(SummarySkillsTable.SkillsCountInColumn(ColumnIndex), ColumnIndex, CreatedColumnElement);

            if (DoColumnsJsonDataReEnumeration) ReEnumerateSummarySkillsTableJsonData();
        }

        /// <summary>Synchronize json list order with the actual view by enumerating <see cref="ColumnTextElementContainer.RelatedJsonData"/> of each column text element</summary>
        private void ReEnumerateColumnItemsJsonData()
        {
            static ObservableCollection<ColumnTextElementData> From(TextElementsColumn Column) => [.. Column.Children.Cast<ColumnTextElementContainer>().Select(x => x.RelatedJsonData)];
            
            @DataContextDomain.PreviewCreator.ImageInfo.TextColumns. First  .Items = From(TextColumn_1);
            @DataContextDomain.PreviewCreator.ImageInfo.TextColumns. Second .Items = From(TextColumn_2);

            @DataContextDomain.PreviewCreator.ImageInfo.SummaryInfo.Passives    = From(SummaryPassivesView);
        }
        private void ReEnumerateSummarySkillsTableJsonData()
        {
            ObservableCollection<ColumnTextElementData> From(int Column) => [.. SummarySkillsTable.Children.OfType<ColumnTextElementContainer>().Where(x => Grid.GetColumn(x) == Column - 1).Select(x => x.RelatedJsonData)];

            @DataContextDomain.PreviewCreator.ImageInfo.SummaryInfo.Skll1 = From(1);
            @DataContextDomain.PreviewCreator.ImageInfo.SummaryInfo.Skll2 = From(2);
            @DataContextDomain.PreviewCreator.ImageInfo.SummaryInfo.Skll3 = From(3);
            @DataContextDomain.PreviewCreator.ImageInfo.SummaryInfo.DefenseSkll = From(4);
        }









        #region Skills / Passives / Keywords  visualization
        #pragma warning disable IDE0017
        private BitmapImage ResolveSkillIcon(SkillConstructor Displaying)
        {
            BitmapImage ResolvedSkillIcon = new();

            if (File.Exists(Displaying.IconID))
            {
                ResolvedSkillIcon = BitmapFromFile(Displaying.IconID);
            }
            else
            {
                try
                {
                    ResolvedSkillIcon = ImageDictionaries.SkillIcons[Displaying.IconID];
                    if (ResolvedSkillIcon == ImageDictionaries.UnknownSpriteImage)
                    {
                        ResolvedSkillIcon = BitmapFromResource($"UI/Limbus/Skills/Default Icons/{Displaying.Specific.Affinity}/{Displaying.Specific.Action}.png");
                    }
                }
                catch { }
            }

            return ResolvedSkillIcon;
        }
        public ColumnTextElementContainer CreateSkill(PlainSkill.UptieLevel GivenSkillText, SkillConstructor Displaying, ColumnTextElementData GivenJsonData)
        {
            #region Skill name replica creation
            BitmapImage ResolvedSkillIcon = ResolveSkillIcon(Displaying);



            SkillNameReplicaUIElement_PCE SkillNameReplicaElement = new()
            {
                ShowAffinityIcon = Displaying.Attributes.ShowAffinityIcon ?? false,
                SkillName = GivenSkillText.Name, Icon = ResolvedSkillIcon,
                Rank = Displaying.Specific.Rank, Coins = string.Join(", ", Displaying.Characteristics.CoinsList),
                SkillType = Displaying.Specific.Action, Affinity = Displaying.Specific.Affinity, DamageType = Displaying.Specific.DamageType,
            };
            SkillNameReplicaElement.SetBinding(SkillNameReplicaUIElement_PCE.NameMaximumWidthProperty, new Binding()
            {
                Source = GivenJsonData, Path = new PropertyPath(nameof(ColumnTextElementData.MaxWidth_Name))
            });
            #endregion



            #region Skill text view creation
            
            ///////////////////////////////////////////////////////////////////////////////////////////
            StackPanel CreatedSkillTextView = new() { HorizontalAlignment = HorizontalAlignment.Left };
            ///////////////////////////////////////////////////////////////////////////////////////////

            #region Main desc
            TMProEmitter Skill_MainDesc = new()
            {
                AcceptsRichTextDelay = false,
                FontType = LimbusFontTypes.Context, TextProcessingMode = InputRichTextFormatter.RichTextFormat.Skills,
                FontSize = 11, LineHeight = 13.7, DisableKeyworLinksCreation = true,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Skill_MainDesc.SetValue(TMProEmitter.RichTextProperty, GivenSkillText.MainDescription);
            Skill_MainDesc.SetBinding(TMProEmitter.MaxWidthProperty, new Binding()
            {
                Source = GivenJsonData, Path = new PropertyPath(nameof(ColumnTextElementData.MaxWidth_SkillMainDescription))
            });
            CreatedSkillTextView.Children.Add(Skill_MainDesc);
            #endregion


            #region Coins
            if (GivenSkillText.Coins is not null)
            {
                StackPanel SkillCoinsPanel = new() { Margin = new Thickness(0, 2, 0, 0), HorizontalAlignment = HorizontalAlignment.Left };
                SkillCoinsPanel.SetBinding(TMProEmitter.MaxWidthProperty, new Binding()
                {
                    Source = GivenJsonData, Path = new PropertyPath(nameof(ColumnTextElementData.MaxWidth_SkillCoinsDescription))
                });
                CreatedSkillTextView.Children.Add(SkillCoinsPanel);


                int CoinNumber = 1;
                foreach (PlainSkill.UptieLevel.Coin Coin in GivenSkillText.Coins)
                {
                    List<string> CollectedCoinDescriptions = [];
                    if (Coin.CoinDescriptions is not null)
                    {
                        CollectedCoinDescriptions = [.. Coin.CoinDescriptions
                            .Where(x => !string.IsNullOrWhiteSpace(x.MainDescription))
                            .Select(x => x.MainDescription)];

                        if (CollectedCoinDescriptions.Count > 0)
                        {
                            TMProEmitter Skill_DescOfSingleCoin = new()
                            {
                                AcceptsRichTextDelay = false,
                                FontType = LimbusFontTypes.Context, TextProcessingMode = InputRichTextFormatter.RichTextFormat.Skills,
                                FontSize = 11, LineHeight = 13.7, DisableKeyworLinksCreation = true,
                                Margin = new Thickness(25, 5, 0, 0), HorizontalAlignment = HorizontalAlignment.Left
                            };
                            Skill_DescOfSingleCoin.SetValue(TMProEmitter.RichTextProperty, string.Join('\n', CollectedCoinDescriptions));


                            Grid CoinGrid = new()
                            {
                                Margin = new Thickness(0, 3, 0, 0),
                                Children =
                                {
                                    new Image()
                                    {
                                        VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left,
                                        Width = 21.6, Source = BitmapFromResource($"UI/Limbus/Skills/Coins/Coin {CoinNumber}.png"),
                                    },
                                    Skill_DescOfSingleCoin
                                }
                            };

                            SkillCoinsPanel.Children.Add(CoinGrid);
                        }
                    }


                    CoinNumber++;

                    if (CoinNumber > 10) break;
                }
            }
            #endregion


            #region Flavor desc
            if (!string.IsNullOrEmpty(GivenSkillText.FlavorDescription))
            {
                TMProEmitter Skill_FlavorDesc = new()
                {
                    AcceptsRichTextDelay = false,
                    FontType = LimbusFontTypes.Context, TextProcessingMode = InputRichTextFormatter.RichTextFormat.Passives,
                    FontSize = 8.5, LineHeight = 11.7, DisableKeyworLinksCreation = true,
                    Foreground = ToSolidColorBrush("#8a592f"), FontStyle = FontStyles.Italic,
                    HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(-1, 10, 0, 0)
                };
                Skill_FlavorDesc.SetValue(TMProEmitter.RichTextProperty, GivenSkillText.FlavorDescription);
                Skill_FlavorDesc.SetBinding(TMProEmitter.MaxWidthProperty, new Binding()
                {
                    Source = GivenJsonData, Path = new PropertyPath(nameof(ColumnTextElementData.MaxWidth_SkillMainDescription))
                });
                CreatedSkillTextView.Children.Add(Skill_FlavorDesc);
            }
            #endregion


            ////////////////////////////////////////////////////////////////////
            SkillNameReplicaElement.SkillLocalizationTextView = CreatedSkillTextView;
            ////////////////////////////////////////////////////////////////////
            
            #endregion



            return new ColumnTextElementContainer() { RelatedJsonData = GivenJsonData, LocalizationTextView = SkillNameReplicaElement };
        }
        public ColumnTextElementContainer CreateSkill_Summary(PlainSkill.UptieLevel GivenSkillText, SkillConstructor Displaying, ColumnTextElementData GivenJsonData)
        {
            #region Skill name replica creation
            BitmapImage ResolvedSkillIcon = ResolveSkillIcon(Displaying);


            SkillNameReplicaUIElement_PCE_Summary SkillNameReplicaElement = new()
            {
                SkillName = GivenSkillText.Name, Icon = ResolvedSkillIcon,
                Rank = Displaying.Specific.Rank, Coins = string.Join(", ", Displaying.Characteristics.CoinsList),
                SkillType = Displaying.Specific.Action, Affinity = Displaying.Specific.Affinity, DamageType = Displaying.Specific.DamageType,
            };
            SkillNameReplicaElement.SetBinding(SkillNameReplicaUIElement_PCE.NameMaximumWidthProperty, new Binding()
            {
                Source = GivenJsonData, Path = new PropertyPath(nameof(ColumnTextElementData.MaxWidth_Name))
            });
            #endregion

            return new ColumnTextElementContainer() { RelatedJsonData = GivenJsonData, LocalizationTextView = SkillNameReplicaElement, IsSummaryView = true, VerticalAlignment = VerticalAlignment.Top };
        }



        public ColumnTextElementContainer CreatePassive(PlainPassive GivenPassiveText, ColumnTextElementData GivenJsonData)
        {
            #region Passive text view creation
            PreviewCreator.PassiveView Passive = new()
            {
                PassiveName = GivenPassiveText.Name,
                PassiveDesc = GivenPassiveText.MainDescription,
                PassiveFlavor = GivenPassiveText.FlavorDescription,
            };
            Passive.SetBinding(PassiveView.PassiveName_MaxWidthProperty, new Binding()
            {
                Source = GivenJsonData, Path = new PropertyPath(nameof(ColumnTextElementData.MaxWidth_Name))
            });
            Passive.SetBinding(PassiveView.PassiveDesc_MaxWidthProperty, new Binding()
            {
                Source = GivenJsonData, Path = new PropertyPath(nameof(ColumnTextElementData.MaxWidth_PassiveDescription))
            });
            #endregion

            return new ColumnTextElementContainer() { RelatedJsonData = GivenJsonData, LocalizationTextView = Passive };
        }
        public ColumnTextElementContainer CreatePassive_Summary(PlainPassive GivenPassiveText, ColumnTextElementData GivenJsonData)
        {
            // Take default, remove descs and change Margin
            ColumnTextElementContainer PassiveContainer = CreatePassive(GivenPassiveText, GivenJsonData);
            PassiveView InnerPassiveView = (PassiveContainer.LocalizationTextView as PassiveView)!;
            PassiveContainer.IsSummaryView = true;
            InnerPassiveView.PassiveDesc = InnerPassiveView.PassiveFlavor = "";
            InnerPassiveView.Margin = new Thickness(0, 30, 0, 30);

            return PassiveContainer;
        }


        public ColumnTextElementContainer CreateKeyword(PlainKeyword GivenKeywordText, ImageSource Icon, ColumnTextElementData GivenJsonData, TextElementsColumn TargetColumn)
        {
            #region Keyword text view creation
            string ResolvedKeywordColor = GivenKeywordText.Color ?? ColorDictionaries.KeywordColors[GivenKeywordText.ID!];
            PreviewCreator.BattleKeywordContainer_PCE KeywordContainer = new()
            {
                KeywordIcon = Icon,
                KeywordName = $"<color={ResolvedKeywordColor}>{GivenKeywordText.Name}</color>",
                KeywordDesc = GivenKeywordText.MainDescription!,
                KeywordFlavor = GivenKeywordText.FlavorDescription,
                Margin = new Thickness(20.4, 0, 0, -10) 
            };

            KeywordContainer.SetBinding(BattleKeywordContainer_PCE.WidthProperty, new Binding()
            {
                // For each column
                Source = TargetColumn == TextColumn_1
                    ? VC_FirstColumnKeywordContainersWidth
                    : VC_SecondColumnKeywordContainersWidth,

                Path = new PropertyPath(nameof(Slider.Value))
            });
            KeywordContainer.SetBinding(BattleKeywordContainer_PCE.KeywordName_MaxWidthProperty, new Binding()
            {
                Source = GivenJsonData, Path = new PropertyPath(nameof(ColumnTextElementData.MaxWidth_Name))
            });
            #endregion



            return new ColumnTextElementContainer() { RelatedJsonData = GivenJsonData, LocalizationTextView = KeywordContainer };
        }
        #pragma warning restore IDE0017
        #endregion
    }
}