using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Passives;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.Custom_Skills_Constructor;
using static LC_Localization_Task_Absolute.Json.FilesIntegration;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    public bool ProjectLoadingEvent = false;

    public bool ColumnItemFocusingEvent = false;


    void SwitchUIQuestion(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        string Tilte = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] [!] [&] * UI switch dialog", "Message window title");
        string Forward = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] [!] [&] * UI switch dialog", "Switch to");
        string Back = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] [!] [&] * UI switch dialog", "Return back");

        if (CustomIdentityPreviewCreator.IsActive)
        {
            MessageBoxResult UISwitchQuestionResult = MessageBox.Show(Back, Tilte, MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            if (UISwitchQuestionResult == MessageBoxResult.Yes)
            {
                SwitchUI_Deactivate();
            }
        }
        else
        {
            MessageBoxResult UISwitchQuestion = MessageBox.Show(Forward, Tilte, MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            if (UISwitchQuestion == MessageBoxResult.Yes)
            {
                SwitchUI_Activate();
            }
        }
    }

    private double PreviousLocation_Left;
    private double PreviousLocation_Top;

    void SwitchUI_Activate(bool DisableTopmost = true)
    {
        if (DisableTopmost)
        {
            if (Configurazione.DeltaConfig.Internal.IsAlwaysOnTop)  // Disable topmost state for session
            {
                SettingsWindow.SettingsControl.ChangeConfigOnOptionToggle = false;
                SettingsWindow.SettingsControl.OptionToggle(SettingsWindow.SettingsControl.ToggleTopmostState, null);
                SettingsWindow.SettingsControl.ChangeConfigOnOptionToggle = true;
            }
        }

        LimbusEditorsGrid.Visibility = Collapsed;

        PreviousLocation_Left = MainControl.Left;
        PreviousLocation_Top = MainControl.Top;

        this.MaxWidth = 100000;
        this.MaxHeight = 100000;

        Rect ScreenSpace = SystemParameters.WorkArea; // *fullscreen*

        this.Left = ScreenSpace.Left;
        this.Top = ScreenSpace.Top;
        this.Width = ScreenSpace.Width;
        this.Height = ScreenSpace.Height;

        this.ResizeMode = ResizeMode.NoResize;

        WindowChrome.SetWindowChrome(this, new WindowChrome()
        {
            ResizeBorderThickness = new Thickness(0),
            CaptionHeight = 1
        });

        MainWindowContentControl.BorderThickness = new Thickness(0);
        CanDragMove = false;

        IdentityPreviewTemplateCreatorGrid.Visibility = Visible;

        CustomIdentityPreviewCreator.IsActive = true;
    }

    void SwitchUI_Deactivate()
    {
        IdentityPreviewTemplateCreatorGrid.Visibility = Collapsed;

        Mode_Handlers.Upstairs.TriggerSwitchToRecent();

        this.ResizeMode = ResizeMode.CanResizeWithGrip;

        WindowChrome.SetWindowChrome(this, new WindowChrome()
        {
            ResizeBorderThickness = new Thickness(10),
            CaptionHeight = 1
        });

        MainWindowContentControl.BorderThickness = new Thickness(1);
        CanDragMove = true;

        MainControl.Top = PreviousLocation_Top;
        MainControl.Left = PreviousLocation_Left;

        LimbusEditorsGrid.Visibility = Visible;

        CustomIdentityPreviewCreator.IsActive = false;
    }



    void InitializeIdentityPreviewCreatorProperties()
    {
        VignetteControllers = new List<UIElement>() // From 'Slider Triggers.cs'
        {
            IdentityPreviewCreator_VignetteSoftnessController,
            IdentityPreviewCreator_TopVignetteOffsetController,
            IdentityPreviewCreator_LeftVignetteOffsetController,
            IdentityPreviewCreator_BottomVignetteOffsetController,
            IdentityPreviewCreator_TextBackgroundFadeoutSoftnessController
        };
    }


    /// <summary>
    /// Unified method for <c>if (this.IsLoaded)</c> condition to prevent null exception from TextChanged/ValueChanged triggers on startup
    /// </summary>
    void Flow(Action Action)
    {        
        if (this.IsLoaded)
        {
            Action();
        }
    }


    #region Item text displayers generation based on focused item info
    void GenerateSkillDisplayerForFocusedItem(UptieLevel TextInfo, SkillContstructor DisplayInfo)
    {
        SkillInfoFormationParameters FormationParameters = new SkillInfoFormationParameters()
        {
            MainDescWidth = FocusedColumnItem.ItemInfo.SkillMainDescriptionWidth,
            CoinDescsWidth = FocusedColumnItem.ItemInfo.SkillCoinsDescriptionWidth,
        };

        bool HighlightStyleStoredCurrentSetting = Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle;
        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle = false; // Do not highlight <style>
        Grid BuildedSkill = CreateSkillGrid
        (
            TextInfo,
            DisplayInfo,
            FormationParameters,
            TargetColumn: FocusedColumnItem.ColumnNumber
        );
        Configurazione.DeltaConfig.PreviewSettings.PreviewSettingsBaseSettings.HighlightStyle = HighlightStyleStoredCurrentSetting;

        FocusedColumnItem.Children[0].Visibility = Collapsed; // Hide 'Text Control' sign

        if (FocusedColumnItem.Children.Count == 2)
        {
            FocusedColumnItem.Children.RemoveAt(1); // If some skill already added to a grid -> delete and add new
        }
        FocusedColumnItem.Children.Add(BuildedSkill);
    }

    void GeneratePassiveDisplayerForFocusedItem(Passive TextInfo)
    {
        Grid Name = GetNameWithBackground(TextInfo.Name, ReversedDropdownShadow: true, FontSizeModifier: ProjectFile.LoadedProject.Text.UnifiedTextSize - 22);

        Name.LayoutTransform = new ScaleTransform()
        {
            ScaleX = 0.48,
            ScaleY = 0.48
        };

        TMProEmitter PassiveDescription = new TMProEmitter()
        {
            LimbusPreviewFormattingMode = "Passives",
            
            LineHeight = CustomIdentityPreviewCreator.SharedParagraphLineHeigh + (ProjectFile.LoadedProject.Text.UnifiedTextSize - 22),
            LayoutTransform = new ScaleTransform(0.48, 0.48),
            FontSize = ProjectFile.LoadedProject.Text.UnifiedTextSize,
            Foreground = ToSolidColorBrush("#ebcaa2"),
            HorizontalAlignment = HorizontalAlignment.Left,
            Width = FocusedColumnItem.ItemInfo.PassiveDescriptionWidth,
            Margin = new Thickness(0, 5, 0, 0),
        }
        .SetBindingWithReturn(TMProEmitter.FontFamilyProperty, "FontFamily", PreviewLayout_Passives)
        as TMProEmitter;

        PassiveDescription.RichText = TextInfo.Description;
        ////////////////////////////////////////////////////////////////////////////////////
        //PassiveDescription.SetValue(Paragraph.TextAlignmentProperty, TextAlignment.Justify);

        FocusedColumnItem.PassiveDescriptionLink = PassiveDescription;

        //TextBlock ItemSignature = GenerateItemSignature(FocusedColumnItem.ColumnNumber);

        Grid BuildedPassiveBox = new Grid()
        {
            Margin = new Thickness(42, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Children =
            {
                new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 400,
                    Margin = new Thickness(-71, -20, 0, 0),
                    Children =
                    {
                        GenerateItemSignature(FocusedColumnItem.ColumnNumber),
                    }
                },
                new StackPanel()
                {
                    Margin = new Thickness(0, 40, 0, 0),
                    RenderTransform = new TranslateTransform(FocusedColumnItem.ItemInfo.HorizontalOffset, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Children =
                    {
                        Name,
                        PassiveDescription
                    }
                }
            }
        };

        FocusedColumnItem.Children[0].Visibility = Collapsed; // Hide 'Text Control' sign

        if (FocusedColumnItem.Children.Count == 2)
        {
            FocusedColumnItem.Children.RemoveAt(1); // If some keyword already added to a grid -> delete and add new
        }
        FocusedColumnItem.Children.Add(BuildedPassiveBox);
    }

    void GenerateKeywordDisplayerForFocusedItem(Keyword TextInfo)
    {
        Image KeywordIcon = new Image()
        {
            Source = FocusedColumnItem.KeywordIcon.Source,
            VerticalAlignment = VerticalAlignment.Top,
            Height = 25,
        };

        TextBlock KeywordName = (new TextBlock() // Keyword Name
        {
            Text = TextInfo.Name,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(4, 0, 0, 0),
            FontSize = 13.5,
            Foreground = ToSolidColorBrush(TextInfo.Color != null ? TextInfo.Color : "#9f6a3a")
        }.SetColumn(1) as TextBlock).SetBindingWithReturn(TextBlock.FontFamilyProperty, "FontFamily", NavigationPanel_ObjectName_Display) as TextBlock;
        

        TMProEmitter KeywordDescription = new TMProEmitter()
        {
            LimbusPreviewFormattingMode = "Keywords",

            LineHeight = CustomIdentityPreviewCreator.SharedParagraphLineHeigh + (ProjectFile.LoadedProject.Text.UnifiedTextSize - 22),
            LayoutTransform = new ScaleTransform(0.48, 0.48),
            FontSize = ProjectFile.LoadedProject.Text.UnifiedTextSize,
            Foreground = ToSolidColorBrush("#ebcaa2"),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(4),

        }.SetBindingWithReturn(TMProEmitter.FontFamilyProperty, "FontFamily", Special_PreviewLayout_Keywords_BattleKeywords_Desc) as TMProEmitter;

        //////////////////////////////////////////////////////////////////////////////////////
        //KeywordDescription.SetValue(Paragraph.TextAlignmentProperty, TextAlignment.Justify);

        KeywordDescription.RichText = TextInfo.Description;

        FocusedColumnItem.KeywordIcon = KeywordIcon;
        FocusedColumnItem.PassiveDescriptionLink = KeywordDescription;
        FocusedColumnItem.ItemNameLink = KeywordName;

        Border BuildedKeywordBox = new Border()
        {
            Margin = new Thickness(42, -5, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            BorderBrush = ToSolidColorBrush("#786753"),
            Background = ToSolidColorBrush("#7F000000"),
            BorderThickness = new Thickness(1),

            Child = new StackPanel()
            {
                Children =
                {
                    new Grid()
                    {
                        Margin = new Thickness(5, 5, 5, 0),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition() { Width = new GridLength(25) },
                            new ColumnDefinition(),
                        },
                        Children =
                        {
                            KeywordIcon, // Keyword Icon
                            KeywordName  // Keyword Name
                        }
                    },
                    KeywordDescription   // Keyword Desc
                }
            }
        }.SetBindingWithReturn(Border.WidthProperty, "Width", KeywordBoxesWidthBinder) as Border;

        FocusedColumnItem.Children[0].Visibility = Collapsed; // Hide 'Text Control' sign

        if (FocusedColumnItem.Children.Count == 2)
        {
            FocusedColumnItem.Children.RemoveAt(1); // If some keyword already added to a grid -> delete and add new
        }
        FocusedColumnItem.Children.Add(BuildedKeywordBox);
    }
    #endregion


    #region Custom cautions
    private void IdentityPreviewCreator_RefreshCustomCautions()
    {
        BitmapImage ColoredCautionPart = SecondaryUtilities.TintWhiteMaskBitmap
        (
            Selected: Caution_PART,
            HexColor: IdentityPreviewCreator_TextEntries_ElementsColor.Text
        );
        DecorativeCaution_Top.Children.Clear();
        DecorativeCaution_Bottom.Children.Clear();

        for (int i = 0; i <= 35; i++)
        {
            DecorativeCaution_Top.Children.Add(new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 8.2, 0),
                SnapsToDevicePixels = true,
                Children =
                {
                    new Image() { Source = ColoredCautionPart },
                    GenerateCautionTextBlock()
                }
            });

            DecorativeCaution_Bottom.Children.Add(new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 8.2, 0),
                SnapsToDevicePixels = true,
                Children =
                {
                    new Image() { Source = ColoredCautionPart },
                    GenerateCautionTextBlock()
                }
            });
        }
    }

    private TextBlock GenerateCautionTextBlock()
    {
        TextBlock OutputTextblock = new TextBlock()
        {
            Foreground = ToSolidColorBrush(IdentityPreviewCreator_TextEntries_ElementsColor.Text),
            SnapsToDevicePixels = true,
        };

        OutputTextblock.SetBinding(FrameworkElement.MarginProperty, new Binding("Margin")
        {
            Source = CustomCautionsParamBinder
        });
        OutputTextblock.SetBinding(TextBlock.FontSizeProperty, new Binding("FontSize")
        {
            Source = CustomCautionsParamBinder
        });
        OutputTextblock.SetBinding(TextBlock.FontFamilyProperty, new Binding("FontFamily")
        {
            Source = CustomCautionsParamBinder
        });
        OutputTextblock.SetBinding(TextBlock.RenderTransformProperty, new Binding("RenderTransform")
        {
            Source = CustomCautionsParamBinder
        });
        OutputTextblock.SetBinding(TextBlock.TextProperty, new Binding("Text")
        {
            Source = IdentityPreviewCreator_TextEntries_CustomCautionString
        });

        return OutputTextblock;
    }
    #endregion


    #region Project loading (Reconstruction)
    FileInfo LoadedProjectFile = null;
    void LoadProject()
    {
        OpenFileDialog ProjectFileSelector = NewOpenFileDialog("Json files", [".json"]);

        if (ProjectFileSelector.ShowDialog() == true)
        {
            FileInfo Target = LoadedProjectFile = new FileInfo(ProjectFileSelector.FileName);

            // All UI changing is inside json [OnDeserialized] events
            ProjectFile.CustomIdentityPreviewProject LoadedProject = Target.Deserealize<ProjectFile.CustomIdentityPreviewProject>(Context: Target.Directory.FullName.Replace("\\", "/"));

            
            
        }
    }
    #endregion


    /// <summary>
    /// Simplified <c>new OpenFileDialog()</c> with parameters
    /// </summary>
    /// <param name="Extensions">["png", "jpg", "bmp", ..]</param>
    /// <returns><c>new OpenFileDialog()</c> with <c>DefaultExt</c> and <c>Filter</c> parameters by <c>Extensions</c></returns>
    public static OpenFileDialog NewOpenFileDialog(string FilesHint, IEnumerable<string> Extensions)
    {
        List<string> FileFilters_DefaultExt = new List<string>();
        List<string> FileFilters_Filter = new List<string>();

        foreach(string Filter in Extensions)
        {
            FileFilters_DefaultExt.Add($".{Filter}");
            FileFilters_Filter.Add($"*.{Filter}");
        }

        OpenFileDialog FileSelection = new OpenFileDialog();
        FileSelection.DefaultExt = string.Join("|", FileFilters_DefaultExt); // .png|.jpg
        FileSelection.Filter = $"{FilesHint}|{string.Join(";", FileFilters_Filter)}";  // *.png;*.jpg

        return FileSelection;
    }

    public static SaveFileDialog NewSaveFileDialog(string FilesHint, IEnumerable<string> Extensions, string FileDefaultName = "")
    {
        List<string> FileFilters_DefaultExt = new List<string>();
        List<string> FileFilters_Filter = new List<string>();

        foreach (string Filter in Extensions)
        {
            FileFilters_DefaultExt.Add($".{Filter}");
            FileFilters_Filter.Add($"*.{Filter}");
        }

        SaveFileDialog FileSaving = new SaveFileDialog();
        FileSaving.DefaultExt = string.Join("|", FileFilters_DefaultExt); // .png|.jpg
        FileSaving.Filter = $"{FilesHint}|{string.Join(";", FileFilters_Filter)}";  // *.png;*.jpg
        FileSaving.FileName = FileDefaultName;

        return FileSaving;
    }



    /// <summary>
    /// Set Opacity to 1 and IsHitTestVisible to True
    /// </summary>
    void MakeAvailable(params UIElement[] Targets)
    {
        foreach (UIElement Target in Targets)
        {
            Target.Opacity = 1;
            Target.IsHitTestVisible = true;
        }
    }

    /// <summary>
    /// Set Opacity to 0.35 and IsHitTestVisible to False
    /// </summary>
    void MakeUnavailable(params UIElement[] Targets)
    {
        foreach (UIElement Target in Targets)
        {
            Target.Opacity = 0.35;
            Target.IsHitTestVisible = false;
        }
    }


    /// <summary>
    /// Set Opacity to 1 and IsHitTestVisible to True
    /// </summary>
    void LMakeAvailable(List<UIElement> Targets)
    {
        foreach (UIElement Target in Targets)
        {
            Target.Opacity = 1;
            Target.IsHitTestVisible = true;
        }
    }

    /// <summary>
    /// Set Opacity to 0.35 and IsHitTestVisible to False
    /// </summary>
    void LMakeUnavailable(List<UIElement> Targets)
    {
        foreach (UIElement Target in Targets)
        {
            Target.Opacity = 0.35;
            Target.IsHitTestVisible = false;
        }
    }
}