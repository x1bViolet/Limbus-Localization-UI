using LC_Localization_Task_Absolute.Json;
using Newtonsoft.Json;
using RichText;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.Json.Custom_Skills_Constructor;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.Json.BaseTypes;

using static System.Windows.Visibility;
using static System.Windows.HorizontalAlignment;
using static System.Windows.VerticalAlignment;

using System.Windows.Media;
using System.Xml.Linq;
using System.Windows.Media.Effects;
using System.Windows.Documents;

namespace LC_Localization_Task_Absolute.Mode_Handlers
{
    internal abstract class __Identity_Preview_Sample__
    {
        /// <summary>
        /// Return insertable grid with skill name replication by constructor and imported desc from regular json localization file
        /// </summary>
        /// <param name="DisplayInformation">Deserialized Skill Constructor</param>
        /// <param name="DescriptionData">Skill Localization Info</param>
        /// <returns></returns>
        internal protected static Grid CreateSkillGrid(SkillContstructor DisplayInformation, Type_Skills.UptieLevel DescriptionData)
        {
            SolidColorBrush AffinityColor = DisplayInformation.Specific.Affinity switch
            {
                "Wrath"    => ToColor("#fe0101"),
                "Lust"     => ToColor("#fe6f01"),
                "Sloth"    => ToColor("#edc427"),
                "Gluttony" => ToColor("#a7fe01"),
                "Gloom"    => ToColor("#1cc7f1"),
                "Pride"    => ToColor("#014fd6"),
                "Envy"     => ToColor("#9800df"),
                _ => ToColor("#9f6a3a")
            };

            string SelectedIconID = DisplayInformation.IconID != null ? DisplayInformation.IconID : $"{DisplayInformation.ID}";
            string SelectedSkillFrameSource =
                !DisplayInformation.Specific.Affinity.Equals("None") // Add Affinity frame if not 'None', else default frame
                  ? $"UI/Limbus/Skills/Frames/{DisplayInformation.Specific.Affinity}/{DisplayInformation.Specific.Rank}.png"
                  : $"UI/Limbus/Skills/Frames/Skill Default Frame.png";

            Grid SkillIconSite = new Grid() // Skill icon, frame, affinity icon, name
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment   = VerticalAlignment.Top,
                Height = 137,
                Width  = 135,
                Margin = new Thickness(-10, 0, 0, 0),
                Children =
                {
                    // Skill icon
                    new Image()
                    {
                        Margin = new Thickness(54.8, 56, 0, 0),
                        Height = 65.2,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment   = VerticalAlignment.Top,
                        Source = Mode_Skills.AcquireSkillIcon(SelectedIconID),
                        OpacityMask = new ImageBrush() { ImageSource = FromResource("UI/Limbus/Skills/Icon Opacity Mask.png") }
                    },

                    // Frame
                    new Image()
                    {
                        Source = FromResource(SelectedSkillFrameSource),
                        Margin = new Thickness(22.8, 24, 0, 0),
                        Height = 127,
                        Width  = 127,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment   = VerticalAlignment.Top
                    },

                    // Damage type
                    DisplayInformation.Specific.DamageType.EqualsOneOf(["Pierce", "Blunt", "Slash"]) ? new Grid()
                    {
                        Margin = new Thickness(70, 111, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment   = VerticalAlignment.Top,
                        Children =
                        {
                            new Grid() // Damage icon frame, 3 layers ui element
                            {
                                Height = 26,
                                Width  = 26,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Children =
                                {
                                    new Border()
                                    {
                                        Background = AffinityColor,
                                        OpacityMask = new ImageBrush() { ImageSource = FromResource("UI/Limbus/Skills/Damage Type Opacity Mask.png") }
                                    },
                                    new Border()
                                    {
                                        Background = AffinityColor,
                                        OpacityMask = new ImageBrush() { ImageSource = FromResource("UI/Limbus/Skills/Damage Type Opacity Mask.png") },
                                        Height = 22.3,
                                        Width  = 22.3,
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        Child = new Border() { Background = ToColor("#7A000000") }
                                    },
                                    new Border()
                                    {
                                        Background = ToColor("#FF232323"),
                                        OpacityMask = new ImageBrush() { ImageSource = FromResource("UI/Limbus/Skills/Damage Type Opacity Mask.png") },
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment   = VerticalAlignment.Center,
                                        Height = 17.3,
                                        Width  = 17.3,
                                        Child = new Border() { Background = ToColor("#7F000000") }
                                    }
                                }
                            },
                            new Grid() // Damege icon
                            {
                                Margin = new Thickness(2),
                                Width = 25,
                                Children =
                                {
                                    new Image() { Source = FromResource($"UI/Limbus/Skills/Damage Types/{DisplayInformation.Specific.DamageType}.png") }
                                }
                            }
                        }
                    } : new Grid(), // Or do not insert damage type Grid if damage type info is invalid
                }
            };

            // Affinity-colored name background image
            Image SkillNameBackgroundImage = new Image()
            {
                Source  = FromResource($"UI/Limbus/Skills/Name Background/{DisplayInformation.Specific.Affinity}.png"),
                Stretch = System.Windows.Media.Stretch.Fill,
                Clip    = new RectangleGeometry(new Rect(77, 0, 1111, 1111)),
                Margin  = new Thickness(-77, 0, 0,0)
            };

            // Grid with skill name and this background above
            Grid SkillName_MainGrid = new Grid()
            {
                Effect = new DropShadowEffect() { ShadowDepth = 4, BlurRadius = 0, Direction = 230 },
                Children =
                {
                    SkillNameBackgroundImage, // Background image with auto scaling that being set below
                    new TextBlock() // Name of the skill
                    {
                        Text     = DisplayInformation.OptionalName,
                        Margin   = new Thickness(7, 3, 30, 0),
                        Padding  = new Thickness(0, 2, 0, 2),
                        MaxWidth = 270,

                        HorizontalAlignment = Left,

                        FontFamily = MainControl.NavigationPanel_ObjectName_Display.FontFamily,
                        Foreground = ToColor("#EBCAA2"),
                        FontSize = 23,

                        LineStackingStrategy = LineStackingStrategy.BlockLineHeight,
                        LineHeight = 25,

                        TextWrapping = TextWrapping.Wrap,
                        Effect = new DropShadowEffect() { ShadowDepth = 2, BlurRadius = 0, Direction = -50 }
                    }
                }
            };

            // Set height auto binding after inserting to main skill name grid
            SkillNameBackgroundImage.SetBinding(FrameworkElement.HeightProperty, new Binding("ActualHeight")
            {
                Source = SkillName_MainGrid
            });

            // Coin icons above skill name
            StackPanel CoinsTab = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment   = VerticalAlignment.Top,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(3.1, 0, 0, 0),
                Height = 28.2,
            };
            foreach (string CoinType in DisplayInformation.Characteristics.CoinsList)
            {
                CoinsTab.Children.Add(CoinType switch
                {
                    "Regular"     => new Image() { Source = Mode_Skills.RegularCoinIcon     },
                    "Unbreakable" => new Image() { Source = Mode_Skills.UnbreakableCoinIcon }
                });
            }

            // Affinity icon and skill name with background
            StackPanel SkillName = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                
                Children =
                {
                    // Affinity icon of skill
                    !DisplayInformation.Specific.Affinity.Equals("None") & DisplayInformation.Attributes.ShowAffinityIcon ? new Image()
                    {
                        Source = FromResource($"UI/Limbus/Skills/Affinity Icons/{DisplayInformation.Specific.Affinity}.png"),
                        Height = 54,
                        VerticalAlignment = Top,
                        Margin = new Thickness(-5, 15, -4, 0)
                    } : new Image(),
                    new Grid() // Coin icons and skill name
                    {
                        Children =
                        {
                            CoinsTab, // Coin icons
                            new Border() // Grid for skill name (Created above — SkillName_MainGrid)
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Margin = new Thickness(4, 35.8, 0, 0),
                                Child = SkillName_MainGrid
                            }
                        }
                    }
                }
            };


            // Readed and recreated skill desc view
            StackPanel SkillDescription = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(-4, 8.7, 0, 0),
                Width = 663 // Main width ('Skills Panel Width' from scan settings)
            };

            { // RichTextBoxes creation with applied desc

                // Main desc
                RichTextBox MainSkillDescription = new RichTextBox()
                {
                    FontFamily = MainControl.PreviewLayout_Skills_MainDesc.FontFamily,
                    FontSize = MainControl.PreviewLayout_Skills_MainDesc.FontSize,
                    Foreground = ToColor("#ebcaa2"),
                    IsReadOnly = true,
                    Margin = new Thickness(0, 0, 0, 7),
                }.ApplyLimbusRichText(
                    RichTextString: DescriptionData.Description,
                    SpecifiedTextProcessingMode: "Skills"
                );
                MainSkillDescription.SetValue(Paragraph.LineHeightProperty, 27.0);

                // Coins
                StackPanel CoinDescs = new StackPanel()
                {
                    Margin = new Thickness(5, 0, 40.2, 0)
                };
                SkillDescription.Children.Add(MainSkillDescription);
                SkillDescription.Children.Add(CoinDescs);
                if (DescriptionData.Coins != null)
                {
                    int CoinEnumerator = 1;
                    foreach (var Coin in DescriptionData.Coins)
                    {
                        string CollectedCoinDescriptions = "";
                        if (Coin.CoinDescriptions != null)
                        {
                            foreach (var Desc in Coin.CoinDescriptions)
                            {
                                if (Desc.Description != null)
                                {
                                    CollectedCoinDescriptions += $"{Desc.Description}\n";
                                }
                            }
                        }
                        if (!CollectedCoinDescriptions.Equals(""))
                        {
                            RichTextBox SingleCoinDescription = new RichTextBox() { // Coin desc
                                Margin = new Thickness(50, 15, 0, 0),
                                Style = MainControl.FindResource("CoinDesc") as Style
                            }.ApplyLimbusRichText(
                                RichTextString: CollectedCoinDescriptions.TrimEnd('\n'),
                                SpecifiedTextProcessingMode: "Skills"
                            );
                            SingleCoinDescription.SetValue(Paragraph.LineHeightProperty, 27.0);

                            CoinDescs.Children.Add(new Grid()
                            {
                                Margin = new Thickness(0, 3, 0, 0),
                                Children =
                                {
                                    new Image() // Coin Icon
                                    {
                                        Source = FromResource($"UI/Limbus/Coins/Coin {CoinEnumerator}.png"),
                                        Width = 43.6,
                                        HorizontalAlignment = HorizontalAlignment.Left,
                                        VerticalAlignment = VerticalAlignment.Top,
                                    },
                                    SingleCoinDescription,
                                }
                            });
                        }

                        CoinEnumerator++;
                        if (CoinEnumerator == 6) break; // Max 6 coins, idk i don't have 7+ coin icons
                    }
                }
            }



            // Final Output
            return new Grid()
            {
                Margin = new Thickness(-30, -20, 0, 0),
                Children =
                {
                    SkillIconSite, // Skill icon, frame, affinity icon, name
                    new StackPanel() // Skill name and description with coins
                    {
                        Children =
                        {
                            new StackPanel()
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Margin = new Thickness(128, 23, 0, 0),
                                Children =
                                {
                                    SkillName,
                                    SkillDescription
                                }
                            }
                        }
                    }
                }
            };
        }
    
    }
}
