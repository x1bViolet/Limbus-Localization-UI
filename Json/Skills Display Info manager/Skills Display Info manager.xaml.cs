using LC_Localization_Task_Absolute.UITranslationHandlers;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static LC_Localization_Task_Absolute.Json.SkillsDisplayInfo;
using static LC_Localization_Task_Absolute.Json.SkillsDisplayInfoManagerWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;

namespace LC_Localization_Task_Absolute.Json
{
    public class SkillConstructorSelect : StackPanel
    {
        private string _AffinityImage { get; set; }
        public string AffinityImage
        {
            get => _AffinityImage;
            set
            {
                _AffinityImage = value;
                (this.Children[0] as Image).Source = BitmapFromResource($"UI/Limbus/Skills/Affinity Icons/{value}.png");
            }
        }

        private string _SkillName { get; set; }
        public string SkillName
        {
            get => _SkillName;
            set
            {
                _SkillName = value;
                (this.Children[1] as UITranslation_Rose).RichText = value;
            }
        }

        public SkillConstructorSelect()
        {
            this.Orientation = Orientation.Horizontal;

            Image AffinityIcon = new() { Width = 25 };
            this.Children.Add(AffinityIcon);

            UITranslation_Rose Header = new()
            {
                Width = 148,
                VerticalAlignment = VerticalAlignment.Center,
                PerfectVerticalAlign = true,
            };
            Header.InterhintPropertiesFrom(SkillsDisplayInfoManagerControl.TabItemHeaderTextBinding);
            this.Children.Add(Header);
        }
    }

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

            UITranslation_Rose RegularCoinOption = new() { Uid = "Regular" };
            RegularCoinOption.InterhintPropertiesFrom(SkillsDisplayInfoManagerControl.RegularCoinTextBinding);

            UITranslation_Rose UnbreakableCoinOption = new() { Uid = "Unbreakable" };
            UnbreakableCoinOption.InterhintPropertiesFrom(SkillsDisplayInfoManagerControl.UnbreakableCoinTextBinding);

            ComboBox CreatedCoinTypeSelector = new()
            {
                Padding = new Thickness(6, 3, 0, 3),
                Height = 25, Width = 207,
                SelectedIndex = PresetType == "Unbreakable" ? 1 : 0,
                Items =
                {
                    RegularCoinOption,
                    UnbreakableCoinOption
                }
            };
            this.Children.Add(CreatedCoinTypeSelector);



            Button CreatedMoveUpButton = new()
            {
                FontFamily = FontFromResource("UI/Fonts/", "Segoe Fluent Icons"),
                Content = "", Height = 25, Width = 25,
                Margin = new Thickness(3, 0, 0, 0)
            };
            this.Children.Add(CreatedMoveUpButton);

            Button CreatedMoveDownButton = new()
            {
                FontFamily = FontFromResource("UI/Fonts/", "Segoe Fluent Icons"),
                Content = "", Height = 25, Width = 25,
                Margin = new Thickness(3, 0, 0, 0)
            };
            this.Children.Add(CreatedMoveDownButton);

            Button CreatedDeleteButton = new()
            {
                FontFamily = FontFromResource("UI/Fonts/", "Segoe Fluent Icons"),
                Content = "", Height = 25, Width = 25,
                Margin = new Thickness(3, 0, 0, 0)
            };
            this.Children.Add(CreatedDeleteButton);



            #region Interactions
            CreatedCoinTypeSelector.SelectionChanged += (Sender, Args) =>
            {
                string NewSelection = (Args.AddedItems[0] as UITranslation_Rose).Uid;
                this.SelectedCoinType = (Args.AddedItems[0] as UITranslation_Rose).Uid;
                CreatedCoinImage.Source = BitmapFromResource($"UI/Limbus/Skills/{NewSelection} Coin.png");

                SkillsDisplayInfoManagerControl.SelectedConstructor.Characteristics.CoinsList[(this.Parent as StackPanel).Children.IndexOf(this)] = NewSelection;
            };
            CreatedMoveUpButton.Click += (Sender, Args) =>
            {
                (this.Parent as StackPanel).MoveItemUp(this);

                SkillsDisplayInfoManagerControl.SelectedConstructor.Characteristics.CoinsList =
                    [.. (this.Parent as StackPanel).Children.OfType<CoinAdjustInteract>().Select(x => x.SelectedCoinType)];
            };
            CreatedMoveDownButton.Click += (Sender, Args) =>
            {
                (this.Parent as StackPanel).MoveItemDown(this);

                SkillsDisplayInfoManagerControl.SelectedConstructor.Characteristics.CoinsList =
                    [.. (this.Parent as StackPanel).Children.OfType<CoinAdjustInteract>().Select(x => x.SelectedCoinType)];
            };
            CreatedDeleteButton.Click += (Sender, Args) =>
            {
                StackPanel Parent = this.Parent as StackPanel;
                int RemoveTargetIndex = Parent.Children.IndexOf(this);

                Parent.Children.RemoveAt(RemoveTargetIndex);
                SkillsDisplayInfoManagerControl.SelectedConstructor.Characteristics.CoinsList.RemoveAt(RemoveTargetIndex);
            };
            #endregion
        }
    }



    public partial class SkillsDisplayInfoManagerWindow : Window
    {
        public static SkillsDisplayInfoManagerWindow SkillsDisplayInfoManagerControl;


        public SkillsDisplayInfoFile LoadedInfo = new() { List = new List<SkillConstructor>() };
        public SkillConstructor SelectedConstructor = CreateBlankConstructor();

        private bool ManualViewUpdateEvent = false;

        private MenuItem TabItemDeleteButton; // To manually disable if there are only one left


        public SkillsDisplayInfoManagerWindow()
        {
            InitializeComponent();

            {
                ItemCollection TabItemsContextMenu = (MainGrid.Resources["TabItemContextMenu"] as ContextMenu).Items;
                TabItemDeleteButton = TabItemsContextMenu[1] as MenuItem;

                List<UITranslation_Rose> Headers = [.. TabItemsContextMenu.OfType<MenuItem>().Select(MenuItem => (UITranslation_Rose)MenuItem.Header)];
                PresentedStaticTextEntries["[Skills DI Manager] * Skill switch button (Context menu — Move Up)"] = Headers[0];
                PresentedStaticTextEntries["[Skills DI Manager] * Skill switch button (Context menu — Delete)"] = Headers[1];
                PresentedStaticTextEntries["[Skills DI Manager] * Skill switch button (Context menu — Move down)"] = Headers[2];
            }

            {
                ItemCollection TextfieldsContextMenu = (MainGrid.Resources["TextfieldContextMenu"] as ContextMenu).Items;
                
                List<UITranslation_Rose> Headers = [.. TextfieldsContextMenu.OfType<MenuItem>().Select(MenuItem => (UITranslation_Rose)MenuItem.Header)];
                PresentedStaticTextEntries["[Skills DI Manager] * Textfields context menu — Copy"] = Headers[0];
                PresentedStaticTextEntries["[Skills DI Manager] * Textfields context menu — Cut"] = Headers[1];
                PresentedStaticTextEntries["[Skills DI Manager] * Textfields context menu — Paste"] = Headers[2];
            }

            this.Closing += (Sender, Args) => { Args.Cancel = true; this.Hide(); };
            this.Loaded += (Sender, Args) => AddSkilLConstructorSelectable(CreateBlankConstructor());
        }

        #region Sqare corners
        private void Window_Loaded(object RequestSender, RoutedEventArgs EventArgs)
        {
            int Preference = 1;
            DwmSetWindowAttribute(new WindowInteropHelper(this).Handle, 33, ref Preference, sizeof(int));
        }
        [LibraryImport("dwmapi.dll")]
        static partial void DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);
        #endregion



        private void AddCoinDefinitionDisplay_Button(object RequestSender, RoutedEventArgs EventArgs) => AddCoinDefinitionDisplay();

        private void AddCoinDefinitionDisplay(string PresetType = "Regular")
        {
            CoinsListPanel.Children.Add(new CoinAdjustInteract(PresetType));
            if (!ManualViewUpdateEvent) SelectedConstructor.Characteristics.CoinsList.Add(PresetType); //operationmaynotexecutecollectionwasmodified
        }


        private void LoadDisplayInfoFile(object RequestSender, RoutedEventArgs EventArgs)
        {
            OpenFileDialog Select = NewOpenFileDialog("LCLI Skills Display Info files", ["json"]);
            if (Select.ShowDialog() == true)
            {
                LoadedInfo = new FileInfo(Select.FileName).Deserealize<SkillsDisplayInfoFile>(Context: Path.GetDirectoryName(Select.FileName).Replace("\\", "/"));

                if (LoadedInfo.List.Count > 0)
                {
                    CurrentConstructorsList.Items.Clear();
                    SelectedFileLabel.Text = Path.GetFileName(Select.FileName);
                    SelectedFileLabel_Tooltip.Text = Select.FileName;

                    int Indexer = 0;
                    foreach (SkillConstructor Constructor in LoadedInfo.List)
                    {
                        ManualViewUpdateEvent = true;

                        AddSkilLConstructorSelectable(Constructor);
                        Indexer++;

                        ManualViewUpdateEvent = false;
                    }
                }
            }
        }

        private void SaveDisplayInfoFile(object RequestSender, RoutedEventArgs EventArgs)
        {
            SaveFileDialog SaveLocation = NewSaveFileDialog("LCLI Skills Display Info files", ["json"], "Skills Display Info.json");
            if (SaveLocation.ShowDialog() == true)
            {
                string Json = LoadedInfo.SerializeToFormattedText(Context: Path.GetDirectoryName(SaveLocation.FileName).Replace("\\", "/"));
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

        private void AddSkilLConstructorSelectable(SkillConstructor Contstructor)
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
            (CurrentConstructorsList.Items[^1] as TabItem).IsSelected = true;
            CheckTabItemDeleteButtonAvailable();
        }

        private void UpdateCurrentConstructorView(object RequestSender, SelectionChangedEventArgs EventArgs)
        {
            if (EventArgs.AddedItems.Count > 0)
            {
                ManualViewUpdateEvent = true;
                try
                {
                    SelectedConstructor = (EventArgs.AddedItems[0] as TabItem).DataContext as SkillConstructor;
                    if (SelectedConstructor != null)
                    {
                        SkillNameTextInput.Text = SelectedConstructor.SkillName;
                        SkillIDTextInput.Text = $"{SelectedConstructor.ID}";
                        ShowAffinityIconCheck.IsChecked = SelectedConstructor.Attributes.ShowAffinityIcon;
                        UpdateSkillIconLabel();

                        SkillTypeSelector.SelectedIndex = SelectedConstructor.Specific.Action switch
                        {
                            "Attack" => 0,
                            "Counter" => 1,
                            "Guard" => 2,
                            "Evade" => 3,
                            _ => 0
                        };
                        DamageTypeSelectSelector.SelectedIndex = SelectedConstructor.Specific.DamageType switch
                        {
                            "Blunt" => 0,
                            "Pierce" => 1,
                            "Slash" => 2,
                            _ => 3,
                        };
                        AffinitySelector.SelectedIndex = SelectedConstructor.Specific.Affinity switch
                        {
                            "Wrath" => 0,
                            "Lust" => 1,
                            "Sloth" => 2,
                            "Gluttony" => 3,
                            "Gloom" => 4,
                            "Pride" => 5,
                            "Envy" => 6,
                            "None" => 7,
                            _ => 7
                        };
                        SkillRankSelector.SelectedIndex = SelectedConstructor.Specific.Rank switch
                        {
                            1 => 0,
                            2 => 1,
                            3 => 2,
                        };

                        CoinsListPanel.Children.Clear();
                        foreach (string CoinType in SelectedConstructor.Characteristics.CoinsList) AddCoinDefinitionDisplay(CoinType);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(FormattedStackTrace(ex, "Stupid errors handler (Current skill constructor view update i mean)").Trim());
                }
                ManualViewUpdateEvent = false;
            }
        }

        private void SkillNameTextInput_TextChanged(object RequestSender, TextChangedEventArgs EventArgs)
        {
            if (IsLoaded && !ManualViewUpdateEvent)
            {
                _ = SelectedConstructor.SkillName
                  = ((CurrentConstructorsList.SelectedItem as TabItem).Header as SkillConstructorSelect).SkillName
                  = SkillNameTextInput.Text;
            }
        }



        private void SkillIDTextInput_TextChanged(object RequestSender, TextChangedEventArgs EventArgs)
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

        private void SelectSkillIcon(object RequestSender, RoutedEventArgs EventArgs)
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
                SelectSkillIconLabel.RichText = GetLocalizationTextFor("[Skills DI Manager] * Icon image (Label)", "Selected");

                SelectSkillIconLabel_Tooltip.Visibility = Visibility.Visible;
                SelectSkillIconLabel_Tooltip_Text.Text = SelectedConstructor.IconID;
            }
            else
            {
                SelectSkillIconLabel.RichText = GetLocalizationTextFor("[Skills DI Manager] * Icon image (Label)", "Default");
                SelectSkillIconLabel_Tooltip.Visibility = Visibility.Collapsed;
            }
        }
        private void ResetSelectedSkillIcon(object RequestSender, MouseButtonEventArgs EventArgs)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                SelectedConstructor.IconID = "";
                UpdateSkillIconLabel();
            }
        }

        private void SkillTypeSelector_SelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
        {
            if (IsLoaded && EventArgs.AddedItems.Count > 0)
            {
                string SkillType = (EventArgs.AddedItems[0] as StackPanel).Uid;

                // Auto disable affinity showing checkbox
                if (!ManualViewUpdateEvent) ShowAffinityIconCheck.IsChecked = !SkillType.EqualsOneOf("Evade", "Guard", "Counter");

                if (SkillType.EqualsOneOf("Evade", "Guard"))
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

                if (!ManualViewUpdateEvent) SelectedConstructor.Specific.Action = (SkillTypeSelector.SelectedItem as UIElement).Uid;
            }
        }

        private void DamageTypeSelectSelector_SelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
        {
            if (IsLoaded && !ManualViewUpdateEvent)
            {
                SelectedConstructor.Specific.DamageType = (DamageTypeSelectSelector.SelectedItem as UIElement).Uid;
            }
        }

        private void AffinitySelector_SelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
        {
            if (IsLoaded && !ManualViewUpdateEvent)
            {
                _ = SelectedConstructor.Specific.Affinity
                  = ((CurrentConstructorsList.SelectedItem as TabItem).Header as SkillConstructorSelect).AffinityImage
                  = (AffinitySelector.SelectedItem as UIElement).Uid;
            }
        }

        private void SkillRankSelector_SelectionChanged(object RequestSender, SelectionChangedEventArgs EventArgs)
        {
            if (IsLoaded && !ManualViewUpdateEvent)
            {
                SelectedConstructor.Specific.Rank = int.Parse((SkillRankSelector.SelectedItem as UITranslation_Rose).Uid);
            }
        }

        private void ShowAffinityIconCheck_CheckedUnchecked(object RequestSender, RoutedEventArgs EventArgs)
        {
            if (IsLoaded && !ManualViewUpdateEvent)
            {
                SelectedConstructor.Attributes.ShowAffinityIcon = (bool)ShowAffinityIconCheck.IsChecked;
            }
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
                LoadedInfo.List.Add(ConstructorSelectable.DataContext as SkillConstructor);
            }
        }
        private void MoveTabItemUp(object RequestSender, RoutedEventArgs EventArgs)
        {
            TabItem PlacementTarget = ((RequestSender as MenuItem).Parent as ContextMenu).PlacementTarget as TabItem;
            CurrentConstructorsList.MoveItemUp(PlacementTarget); PlacementTarget.IsSelected = true;
            ReEnumerateConstructorsList();
        }
        private void MoveTabItemDown(object RequestSender, RoutedEventArgs EventArgs)
        {
            TabItem PlacementTarget = ((RequestSender as MenuItem).Parent as ContextMenu).PlacementTarget as TabItem;
            CurrentConstructorsList.MoveItemDown(PlacementTarget); PlacementTarget.IsSelected = true;
            ReEnumerateConstructorsList();
        }
        private void DeleteTabItem(object RequestSender, RoutedEventArgs EventArgs)
        {
            if (CurrentConstructorsList.Items.Count != 1)
            {
                TabItem PlacementTarget = ((RequestSender as MenuItem).Parent as ContextMenu).PlacementTarget as TabItem;
                CurrentConstructorsList.Items.Remove(((RequestSender as MenuItem).Parent as ContextMenu).PlacementTarget);
                ReEnumerateConstructorsList();
                CheckTabItemDeleteButtonAvailable();
            }
        }

        private void AddConstructorButton_Click(object RequestSender, RoutedEventArgs EventArgs)
        {
            AddSkilLConstructorSelectable(CreateBlankConstructor());
        }

        private void UnfocusMint(UITranslation_Mint Target)
        {
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(Target), null);
            Keyboard.ClearFocus();
        }

        private void EscapeOrEnterUnfocus(object RequestSender, KeyEventArgs EventArgs)
        {
            if (EventArgs.Key == Key.Enter | EventArgs.Key == Key.Escape) UnfocusMint(SkillNameTextInput);
        }
    }
}
