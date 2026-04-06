using LCLocalizationInterface.LimbusRegistry.JsonTypes.Specific;
using static LCLocalizationInterface.LimbusRegistry.SkillsDisplayInfoManagerWindow;

namespace LCLocalizationInterface.LimbusRegistry
{
    /// <summary>Inner content of TabItems that represents current Skill Constructors</summary>
    public class SkillConstructorSelect : StackPanel
    {
        public AffinityName AffinityImage
        {
            get => field; set {
                field = value;
                (this.Children[0] as Image)!.Source = value == AffinityName.None
                    ? new BitmapImage()
                    : BitmapFromResource($"UI/Limbus/Skills/Affinity Icons/{value}.png");
            }
        } = AffinityName.None;

        public string SkillName
        {
            get => field; set {
                field = value; (this.Children[1] as IntenseStareType1)!.RichText = value;
            }
        } = "";

        public SkillConstructorSelect()
        {
            this.Orientation = Orientation.Horizontal;

            Image AffinityIcon = new() { Width = 25 };
            this.Children.Add(AffinityIcon);

            IntenseStareType1 Header = new()
            {
                Width = 148,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
                PerfectVerticalAlign = true,
            };
            Header.InherintPropertiesFrom(@Languages.PresentedTextElements["[Skills DI Manager] * Skill name inside button"]);
            this.Children.Add(Header);
        }
    }

    /// <summary>Skill Constructor's added coin adjustment panel</summary>
    public class CoinAdjustInteract : StackPanel
    {
        public string SelectedCoinType { get; private set; }

        public CoinAdjustInteract(string PresetType = "Regular")
        {
            this.Orientation = Orientation.Horizontal;
            this.SelectedCoinType = PresetType;
            this.Margin = new Thickness(0, 3, 0, 0);

            Image CreatedCoinImage = new()
            {
                Source = BitmapFromResource($"UI/Limbus/Skills/{PresetType} Coin.png"),
                Height = 25, Width = 30,
            };
            this.Children.Add(CreatedCoinImage);

            IntenseStareType1 RegularCoinOption = new() { Uid = "Regular" };
            RegularCoinOption.InherintPropertiesFrom(@Languages.PresentedTextElements["[Skills DI Manager] * Regular coin option"], IncludeRichText: true);

            IntenseStareType1 UnbreakableCoinOption = new() { Uid = "Unbreakable" };
            UnbreakableCoinOption.InherintPropertiesFrom(@Languages.PresentedTextElements["[Skills DI Manager] * Unbreakable coin option"], IncludeRichText: true);

            IntenseStareType1 ExcisionCoinOption = new() { Uid = "Excision" };
            ExcisionCoinOption.InherintPropertiesFrom(@Languages.PresentedTextElements["[Skills DI Manager] * Excision coin option"], IncludeRichText: true);

            IntenseStareType1 PurpleCoinOption = new() { Uid = "Purple" };
            PurpleCoinOption.InherintPropertiesFrom(@Languages.PresentedTextElements["[Skills DI Manager] * Purple coin option"], IncludeRichText: true);

            ComboBox CreatedCoinTypeSelector = new()
            {
                Padding = new Thickness(6, 3, 0, 3),
                Height = 25, Width = 207,
                SelectedIndex = PresetType switch { "Regular" => 0, "Unbreakable" => 1, "Excision" => 2, "Purple" => 3, _ => 0 },
                Items =
                {
                    RegularCoinOption,
                    UnbreakableCoinOption,
                    ExcisionCoinOption,
                    PurpleCoinOption
                }
            };
            this.Children.Add(CreatedCoinTypeSelector);



            Button CreatedMoveUpButton = new()
            {
                FontFamily = FontFromResource("UI/Fonts/#Segoe Fluent Icons"),
                Content = "", Height = 25, Width = 25,
                Margin = new Thickness(3, 0, 0, 0)
            };
            this.Children.Add(CreatedMoveUpButton);

            Button CreatedMoveDownButton = new()
            {
                FontFamily = FontFromResource("UI/Fonts/#Segoe Fluent Icons"),
                Content = "", Height = 25, Width = 25,
                Margin = new Thickness(3, 0, 0, 0)
            };
            this.Children.Add(CreatedMoveDownButton);

            Button CreatedDeleteButton = new()
            {
                FontFamily = FontFromResource("UI/Fonts/#Segoe Fluent Icons"),
                Content = "", Height = 25, Width = 25,
                Margin = new Thickness(3, 0, 0, 0)
            };
            this.Children.Add(CreatedDeleteButton);



            #region Interactions
            CreatedCoinTypeSelector.SelectionChanged += delegate (object Sender, SelectionChangedEventArgs Args)
            {
                string NewSelection = (Args.AddedItems[0] as IntenseStareType1)!.Uid;
                this.SelectedCoinType = (Args.AddedItems[0] as IntenseStareType1)!.Uid;
                CreatedCoinImage.Source = BitmapFromResource($"UI/Limbus/Skills/{NewSelection} Coin.png");

                SkillsDisplayInfoManagerWindowInstance.SelectedConstructor.Characteristics.CoinsList[(this.Parent as StackPanel)!.Children.IndexOf(this)] = NewSelection;
            };


            void SyncConstructorsOrder()
            {
                SkillsDisplayInfoManagerWindowInstance.SelectedConstructor.Characteristics.CoinsList =
                    [.. (this.Parent as StackPanel)!.Children.OfType<CoinAdjustInteract>().Select(x => x.SelectedCoinType)];
            }

            CreatedMoveUpButton.Click += (_, _) =>
            {
                (this.Parent as StackPanel)!.Children.MoveItemUp(this);
                SyncConstructorsOrder();
            };
            CreatedMoveDownButton.Click += (_, _) =>
            {
                (this.Parent as StackPanel)!.Children.MoveItemDown(this);
                SyncConstructorsOrder();
            };

            CreatedDeleteButton.Click += (_, _) =>
            {
                StackPanel Parent = (this.Parent as StackPanel)!;
                int RemoveTargetIndex = Parent.Children.IndexOf(this);

                Parent.Children.RemoveAt(RemoveTargetIndex);
                SkillsDisplayInfoManagerWindowInstance.SelectedConstructor.Characteristics.CoinsList.RemoveAt(RemoveTargetIndex);
            };
            #endregion
        }
    }




#warning No MVVM and all that stuff, although it still works perfectly fine event without it (Except for the style being separated from the main window)
    public partial class SkillsDisplayInfoManagerWindow : Window
    {
        #pragma warning disable CS8618
        public static SkillsDisplayInfoManagerWindow SkillsDisplayInfoManagerWindowInstance;
        #pragma warning restore CS8618


        public SkillsDisplayInfoJson LoadedInfo = new();
        public SkillConstructor SelectedConstructor;

        private bool ManualViewUpdateEvent = false;


        private readonly MenuItem TabItemDeleteButton; // To manually disable if there are only one left

        public SkillsDisplayInfoManagerWindow()
        {
            InitializeComponent();
            SkillsDisplayInfoManagerWindowInstance = this;

            #region Init of language elements inside resource dictionaries
            // Tab items
            {
                ItemCollection TabItemsContextMenu = (MainGrid.Resources["TabItemContextMenu"] as ContextMenu)!.Items;
                TabItemDeleteButton = (TabItemsContextMenu[1] as MenuItem)!;

                List<IntenseStareType1> Headers = [.. TabItemsContextMenu.OfType<MenuItem>().Select(MenuItem => (IntenseStareType1)MenuItem.Header)];
                @Languages.PresentedTextElements["[Skills DI Manager] * Skill switch button (Context menu — Move Up)"] = Headers[0];
                @Languages.PresentedTextElements["[Skills DI Manager] * Skill switch button (Context menu — Delete)"] = Headers[1];
                @Languages.PresentedTextElements["[Skills DI Manager] * Skill switch button (Context menu — Move down)"] = Headers[2];
            }

            // Cut/Copy/Paste from textboxes
            {
                ItemCollection TextfieldsContextMenu = (MainGrid.Resources["TextfieldContextMenu"] as ContextMenu)!.Items;

                List<IntenseStareType1> Headers = [.. TextfieldsContextMenu.OfType<MenuItem>().Select(MenuItem => (IntenseStareType1)MenuItem.Header)];
                @Languages.PresentedTextElements["[Skills DI Manager] * Textfields context menu — Copy"] = Headers[0];
                @Languages.PresentedTextElements["[Skills DI Manager] * Textfields context menu — Cut"] = Headers[1];
                @Languages.PresentedTextElements["[Skills DI Manager] * Textfields context menu — Paste"] = Headers[2];
            }
            #endregion

            this.Closing += delegate (object? Sender, CancelEventArgs Args) { Args.Cancel = true; this.Hide(); };

            SelectedConstructor = SkillConstructor.CreateBlank(1);
            AddSkillConstructorSelectable(SelectedConstructor);
        }

















        #region Save/Load file
        private void LoadDisplayInfoFile(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("LCLI Skills Display Info files", ["json"]);
            if (Select.ShowDialog() == true)
            {
                if (new FileInfo(Select.FileName).TryDeserealizeJsonAs(out SkillsDisplayInfoJson ReadedFile, out Exception Occurred, Context: Path.GetDirectoryName(Select.FileName)!.Replace("\\", "/")))
                {
                    LoadedInfo = ReadedFile;
                    if (LoadedInfo.List.Count > 0)
                    {
                        CurrentConstructorsList.Items.Clear();
                        SelectedFileLabel.Text = Path.GetFileName(Select.FileName);
                        SelectedFileLabel_Tooltip.Text = Select.FileName;

                        int Indexer = 0;
                        foreach (SkillConstructor Constructor in LoadedInfo.List)
                        {
                            ManualViewUpdateEvent = true;

                            AddSkillConstructorSelectable(Constructor);
                            Indexer++;

                            ManualViewUpdateEvent = false;
                        }
                    }
                }
                else
                {
                    ErrorMessageWindow.ShowException(Occurred);
                }
            }
        }

        private void SaveDisplayInfoFile(object Sender, RoutedEventArgs Args)
        {
            SaveFileDialog SaveLocation = NewSaveFileDialog("LCLI Skills Display Info files", ["json"], "Skills Display Info.json");
            if (SaveLocation.ShowDialog() == true)
            {
                string Json = LoadedInfo.SerializeToFormattedJsonText(Context: Path.GetDirectoryName(SaveLocation.FileName)!.Replace("\\", "/"));
                SelectedFileLabel.Text = Path.GetFileName(SaveLocation.FileName);
                SelectedFileLabel_Tooltip.Text = SaveLocation.FileName;

                #region Json formatting
                Json = Regex.Replace(Json, @"""ID"": (?<ID>(\-)?(\d+)),\n( +)""Icon ID"": ", Match =>
                {
                    return @$"""ID"": {Match.Groups["ID"].Value}, ""Icon ID"": ";
                });
                Json = Regex.Replace(Json, @"""Attributes"": {\n( +)""Show Affinity Icon"": true\n( +)}", Match =>
                {
                    return @"""Attributes"": { ""Show Affinity Icon"": true }";
                });
                Json = Regex.Replace(Json, @"},\n(?<Spaces> +){", Match =>
                {
                    return $"}},\n\n\n\n{Match.Groups["Spaces"].Value}{{";
                });
                Json = Regex.Replace(Json, @"""Characteristics"": {\n( +)""Coins List"": \[\n( +)(?<CoinsList>.*?)\]\n( +)}", Match =>
                {
                    return @$"""Characteristics"": {{ ""Coins List"": [{Match.Groups["CoinsList"].Value.Replace("\n", "").Replace(" ", "").Replace(",", ", ")}] }}";
                }, RegexOptions.Singleline);
                Json = Json.RegexRemove(new Regex(@",\n( +)""Attributes"": {\n( +)""Show Affinity Icon"": false\n( +)}"));
                #endregion

                File.WriteAllText(SaveLocation.FileName, Json);
            }
        }
        #endregion

        #region Add coin
        private void AddCoinDefinitionDisplayButton_Click(object Sender, RoutedEventArgs Args) => AddCoinDefinitionDisplay();

        private void AddCoinDefinitionDisplay(string PresetType = "Regular")
        {
            CoinsListPanel.Children.Add(new CoinAdjustInteract(PresetType));
            if (!ManualViewUpdateEvent) SelectedConstructor.Characteristics.CoinsList.Add(PresetType); //operationmaynotexecutecollectionwasmodified
        }
        #endregion

        #region Add skill constructor
        private void AddSkillConstructorSelectable(SkillConstructor Contstructor)
        {
            if (!ManualViewUpdateEvent) LoadedInfo.List.Add(Contstructor);

            CurrentConstructorsList.Items.Add(new TabItem()
            {
                DataContext = Contstructor,
                Header = new SkillConstructorSelect()
                {
                    AffinityImage = Contstructor.Specific.Affinity,
                    SkillName = Contstructor.SkillName
                },
            });
            (CurrentConstructorsList.Items[^1] as TabItem)!.IsSelected = true;
            CheckTabItemDeleteButtonAvailable();
        }
        #endregion

        #region Update current skill constructor on switch
        private void UpdateCurrentConstructorView(object Sender, SelectionChangedEventArgs Args)
        {
            if (Args.AddedItems.Count > 0)
            {
                ManualViewUpdateEvent = true;
                try
                {
                    SelectedConstructor = ((Args.AddedItems[0] as TabItem)!.DataContext as SkillConstructor)!;
                    if (SelectedConstructor is not null)
                    {
                        SkillNameTextInput.Text = SelectedConstructor.SkillName;
                        SkillIDTextInput.Text = $"{SelectedConstructor.ID}";
                        ShowAffinityIconCheck.IsChecked = SelectedConstructor.Attributes.ShowAffinityIcon;
                        UpdateSkillIconLabel();

                        SkillTypeSelector.SelectedIndex = SelectedConstructor.Specific.Action switch
                        {
                            SkillType.Attack => 0, SkillType.Counter => 1, SkillType.Guard => 2, SkillType.Evade => 3, _ => 0
                        };
                        DamageTypeSelectSelector.SelectedIndex = SelectedConstructor.Specific.DamageType switch
                        {
                            DamageType.Blunt => 0, DamageType.Pierce => 1, DamageType.Slash => 2, _ => 3,
                        };
                        AffinitySelector.SelectedIndex = SelectedConstructor.Specific.Affinity switch
                        {
                            AffinityName.Wrath => 0, AffinityName.Lust => 1, AffinityName.Sloth => 2, AffinityName.Gluttony => 3, AffinityName.Gloom => 4, AffinityName.Pride => 5, AffinityName.Envy => 6, AffinityName.None => 7, _ => 7
                        };
                        SkillRankSelector.SelectedIndex = SelectedConstructor.Specific.Rank switch
                        {
                            1 => 0,   2 => 1,   3 => 2,   _ => 0
                        };

                        CoinsListPanel.Children.Clear();
                        foreach (string CoinType in SelectedConstructor.Characteristics.CoinsList) AddCoinDefinitionDisplay(CoinType);
                    }
                }
                finally
                {
                    ManualViewUpdateEvent = false;
                }
            }
        }
        #endregion



        private void AddConstructorButton_Click(object Sender, RoutedEventArgs Args) => AddSkillConstructorSelectable(SkillConstructor.CreateBlank(1));

        #region Selected skill constructor adjustment
        /// <summary>Sync skill name with the current Skill Constrcutor</summary>
        private void SkillNameTextInput_TextChanged(object Sender, EventArgs Args)
        {
            if (IsLoaded && !ManualViewUpdateEvent)
            {
                _ = SelectedConstructor.SkillName
                  = ((CurrentConstructorsList.SelectedItem as TabItem)!.Header as SkillConstructorSelect)!.SkillName
                  = SkillNameTextInput.Text;
            }
        }

        /// <summary>Change borders color to red if not a number &amp; sync id with the current Skill Constrcutor</summary>
        private void SkillIDTextInput_TextChanged(object Sender, EventArgs Args)
        {
            if (IsLoaded)
            {
                if (int.TryParse(SkillIDTextInput.Text, out int NewID))
                {
                    if (!ManualViewUpdateEvent) SelectedConstructor.ID = NewID;
                    SkillIDTextInput_InvalidIndicator.BorderBrush = Brushes.Transparent;
                }
                else
                {
                    SkillIDTextInput_InvalidIndicator.BorderBrush = ToSolidColorBrush("#973D3D"); // Red border if not number
                }
            }
        }

        private void SelectSkillIcon(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Image files", ["png", "jpg"]);
            if (Select.ShowDialog() == true)
            {
                SelectedConstructor.IconID = Select.FileName.Replace("\\", "/");
                UpdateSkillIconLabel();
            }
        }
        private void UpdateSkillIconLabel()
        {
            if (File.Exists(SelectedConstructor.IconID))
            {
                SelectSkillIconLabel.RichText = @Languages.GetLocalizationTextFor("[Skills DI Manager] * Icon image (Label)", "Selected");

                SelectSkillIconLabel_Tooltip.Visibility = Visibility.Visible;
                SelectSkillIconLabel_Tooltip_Text.Text = SelectedConstructor.IconID;
            }
            else
            {
                SelectSkillIconLabel.RichText = @Languages.GetLocalizationTextFor("[Skills DI Manager] * Icon image (Label)", "Default");
                SelectSkillIconLabel_Tooltip.Visibility = Visibility.Collapsed;
            }
        }
        private void ResetSelectedSkillIcon(object Sender, MouseButtonEventArgs Args)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                SelectedConstructor.IconID = "";
                UpdateSkillIconLabel();
            }
        }

        private void SkillTypeSelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
        {
            if (IsLoaded && Args.AddedItems.Count > 0)
            {
                string SkillType = (Args.AddedItems[0] as StackPanel)!.Uid;

                // Auto disable affinity showing checkbox
                if (!ManualViewUpdateEvent) ShowAffinityIconCheck.IsChecked = !SkillType.EqualsToOneOf("Evade", "Guard", "Counter");

                if (SkillType.EqualsToOneOf("Evade", "Guard"))
                {
                    DamageTypeSelectSelector.SelectedIndex = 3;
                    NoneDamageTypeOption.Visibility = Visibility.Visible;
                    DamageTypeSelectSelector.IsEnabled = false;
                }
                else
                {
                    DamageTypeSelectSelector.SelectedIndex = 0;
                    NoneDamageTypeOption.Visibility = Visibility.Collapsed;
                    DamageTypeSelectSelector.IsEnabled = true;
                }

                if (!ManualViewUpdateEvent) SelectedConstructor.Specific.Action = Enum.Parse<SkillType>((SkillTypeSelector.SelectedItem as UIElement)!.Uid);
            }
        }

        private void DamageTypeSelectSelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
        {
            if (IsLoaded && !ManualViewUpdateEvent)
            {
                SelectedConstructor.Specific.DamageType = Enum.Parse<DamageType>((DamageTypeSelectSelector.SelectedItem as UIElement)!.Uid);
            }
        }

        private void AffinitySelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
        {
            if (IsLoaded && !ManualViewUpdateEvent)
            {
                _ = SelectedConstructor.Specific.Affinity
                  = ((CurrentConstructorsList.SelectedItem as TabItem)!.Header as SkillConstructorSelect)!.AffinityImage
                  = Enum.Parse<AffinityName>((AffinitySelector.SelectedItem as UIElement)!.Uid);
            }
        }

        private void SkillRankSelector_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
        {
            if (IsLoaded && !ManualViewUpdateEvent)
            {
                SelectedConstructor.Specific.Rank = int.Parse((SkillRankSelector.SelectedItem as IntenseStareType1)!.Uid);
            }
        }

        private void ShowAffinityIconCheck_CheckedUnchecked(object Sender, RoutedEventArgs Args)
        {
            if (IsLoaded && !ManualViewUpdateEvent)
            {
                SelectedConstructor.Attributes.ShowAffinityIcon = (bool)ShowAffinityIconCheck.IsChecked!;
            }
        }
        #endregion

        #region Technical
        private void EscapeOrEnterUnfocus(object Sender, KeyEventArgs Args)
        {
            if (Args.Key.EqualsToOneOf(Key.Escape, Key.Enter)) Keyboard.ClearFocus();
        }

        private void CheckTabItemDeleteButtonAvailable()
        {
            TabItemDeleteButton.IsEnabled = CurrentConstructorsList.Items.Count > 1;
        }

        private void ReEnumerateConstructorsList()
        {
            LoadedInfo.List.Clear();
            foreach (TabItem ConstructorSelectable in CurrentConstructorsList.Items)
            {
                LoadedInfo.List.Add((ConstructorSelectable.DataContext as SkillConstructor)!);
            }
        }
        private void MoveTabItemUp(object Sender, RoutedEventArgs Args)
        {
            TabItem PlacementTarget = (((Sender as MenuItem)!.Parent as ContextMenu)!.PlacementTarget as TabItem)!;
            CurrentConstructorsList.Items.MoveItemUp(PlacementTarget); PlacementTarget.IsSelected = true;
            ReEnumerateConstructorsList();
        }
        private void MoveTabItemDown(object Sender, RoutedEventArgs Args)
        {
            TabItem PlacementTarget = (((Sender as MenuItem)!.Parent as ContextMenu)!.PlacementTarget as TabItem)!;
            CurrentConstructorsList.Items.MoveItemDown(PlacementTarget!); PlacementTarget.IsSelected = true;
            ReEnumerateConstructorsList();
        }
        private void DeleteTabItem(object Sender, RoutedEventArgs Args)
        {
            if (CurrentConstructorsList.Items.Count != 1)
            {
                CurrentConstructorsList.Items.Remove(((Sender as MenuItem)!.Parent as ContextMenu)!.PlacementTarget);
                ReEnumerateConstructorsList();
                CheckTabItemDeleteButtonAvailable();
            }
        }
        #endregion
    }
}