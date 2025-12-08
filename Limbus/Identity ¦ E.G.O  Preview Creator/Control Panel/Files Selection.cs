using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.PreviewCreator;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using LC_Localization_Task_Absolute.UITranslationHandlers;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Limbus_Integration.@ColorInfo;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    private void SelectPortraitImage(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectPortraitImage_Action(Select.FileName);
    }
    public void SelectPortraitImage_Action(string ImagePath)
    {
        @CurrentPreviewCreator.LoadedImageInfo.Portrait.ImagePath = ImagePath.Replace("\\", "/");

        _ = IdentityPortraitImage.Source
          = EGOPortrait_Image.Source
          = BitmapFromFile(ImagePath);

        if (File.Exists(ImagePath))
        {
            ExternElement("[C] * [Section:Image parameters / Portrait image] Portrait image (Label)", "Selected", Path.GetFileName(ImagePath));
        }
        else
        {
            ExternElement("[C] * [Section:Image parameters / Portrait image] Portrait image (Label)", "Default");
        }
    }

    private void SelectImageLabelFont(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);
        if (Select.ShowDialog() == true) SelectImageLabelFont_Action(Select.FileName);
    }
    public void SelectImageLabelFont_Action(string FontPath)
    {
        if (FontPath == "#Bebas Neue Bold") ImageLabel.FontFamily = Resource<FontFamily>("BebasKaiUniversal");
        else
        {
            @CurrentPreviewCreator.LoadedImageInfo.ImageLabelText.Font = FontPath.Replace("\\", "/");
            ImageLabel.FontFamily = FileToFontFamily(FontPath);
        }

        if (File.Exists(FontPath))
        {
            ExternElement("[C] * [Section:Image parameters / Image label] Image label font (Label)", "Selected", GetFontName(FontPath));
        }
        else
        {
            ExternElement("[C] * [Section:Image parameters / Image label] Image label font (Label)", "Default");
        }
    }

    private void SelectCautionsFont(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);
        if (Select.ShowDialog() == true) SelectCautionsFont_Action(Select.FileName);
    }
    public void SelectCautionsFont_Action(string FontPath)
    {
        if (FontPath == "#Bebas Neue Bold") DecorativeCautions_PropertyBindingSource.FontFamily = Resource<FontFamily>("BebasKaiUniversal");
        else
        {
            @CurrentPreviewCreator.LoadedImageInfo.Cautions.Font = FontPath.Replace("\\", "/");
            DecorativeCautions_PropertyBindingSource.FontFamily = FileToFontFamily(FontPath);
        }

        if (File.Exists(FontPath))
        {
            ExternElement("[C] * [Section:Decorative cautions] Custom font (Label)", "Selected", GetFontName(FontPath));
        }
        else
        {
            ExternElement("[C] * [Section:Decorative cautions] Custom font (Label)", "Default");
        }
    }

    private void SelectItemSignaturesFont(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);
        if (Select.ShowDialog() == true) SelectItemSignaturesFont_Action(Select.FileName);
    }
    public void SelectItemSignaturesFont_Action(string FontPath)
    {
        if (FontPath == "#Bebas Neue Bold") @CompositionResources.TextColumns.ItemSignaturesFont = Resource<FontFamily>("BebasKaiUniversal");
        else
        {
            @CurrentPreviewCreator.LoadedImageInfo.TextColumns.ItemSignaturesFont = FontPath.Replace("\\", "/");
            @CompositionResources.TextColumns.ItemSignaturesFont = FileToFontFamily(FontPath);
        }

        if (File.Exists(FontPath))
        {
            ExternElement("[C] * [Section:Textual info/General options] Item signatures font (Label)", "Selected", GetFontName(FontPath));
        }
        else
        {
            ExternElement("[C] * [Section:Textual info/General options] Item signatures font (Label)", "Default");
        }
    }

    private void SelectTextBackgroundEffectsImage(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectTextBackgroundEffectsImage_Action(Select.FileName);
    }
    public void SelectTextBackgroundEffectsImage_Action(string ImagePath)
    {
        @CurrentPreviewCreator.LoadedImageInfo.TextBackgroundEffects.ImagePath = ImagePath.Replace("\\", "/");
        TextBackgroundEffects_Image_IDENTITY.Source = TextBackgroundEffects_Image_EGO.Source  = BitmapFromFile(ImagePath);

        if (File.Exists(ImagePath))
        {
            ExternElement("[C] * [Section:Text background effects] Effects image (Label)", "Selected", Path.GetFileName(ImagePath));
        }
        else
        {
            ExternElement("[C] * [Section:Text background effects] Effects image (Label)", "Default");
        }
    }

    private void SelectOverlaySketchImage(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectOverlaySketchImage_Action(Select.FileName);
    }
    public void SelectOverlaySketchImage_Action(string ImagePath)
    {
        @CurrentPreviewCreator.LoadedImageInfo.OverlaySketch.ImagePath = ImagePath.Replace("\\", "/");
        OverlaySketchImage.Source = BitmapFromFile(ImagePath);

        if (File.Exists(ImagePath))
        {
            ExternElement("[C] * [Section:Overlay sketch] Sketch image (Label)", "Selected", Path.GetFileName(ImagePath));
        }
        else
        {
            ExternElement("[C] * [Section:Overlay sketch] Sketch image (Label)", "Default");
        }
    }




    private void SelectUpperLeftLogoImage(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectUpperLeftLogoImage_Action(Select.FileName);
    }
    public void SelectUpperLeftLogoImage_Action(string ImagePath)
    {
        if (File.Exists(ImagePath))
        {
            Logo_LeftTopCorner.Source = BitmapFromFile(ImagePath);
            @CurrentPreviewCreator.LoadedImageInfo.OtherEffects.UpperLeftLogoImage = ImagePath.Replace("\\", "/");
            ExternElement("[C] * [Section:Other effects] Upper left logo image (Label)", "Selected", Path.GetFileName(ImagePath));
        }
        else
        {
            Logo_LeftTopCorner.Source = BitmapFromResource("Limbus/Identity ¦ E.G.O  Preview Creator/Images/Logo/Left Top Corner.png");
            ExternElement("[C] * [Section:Other effects] Upper left logo image (Label)", "Default");
        }
    }

    private void SelectWalpurgisNightLogoImage(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectWalpurgisNighLogoImage_Action(Select.FileName);
    }
    public void SelectWalpurgisNighLogoImage_Action(string ImagePath)
    {
        if (File.Exists(ImagePath))
        {
            WalpurgisNightLogo.Source = BitmapFromFile(ImagePath);
            @CurrentPreviewCreator.LoadedImageInfo.OtherEffects.WalpurgisNightLogoImage = ImagePath.Replace("\\", "/");
            ExternElement("[C] * [Section:Other effects] Walpurgis Night logo image (Label)", "Selected", Path.GetFileName(ImagePath));
        }
        else
        {
            WalpurgisNightLogo.Source = BitmapFromResource("Limbus/Identity ¦ E.G.O  Preview Creator/Images/Walpurgis Night/Logo.png");
            ExternElement("[C] * [Section:Other effects] Walpurgis Night logo image (Label)", "Default");
        }
    }

    private void SelectBottomRightLogoImage(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectBottomRightLogoImage_Action(Select.FileName);
    }
    public void SelectBottomRightLogoImage_Action(string ImagePath)
    {
        if (File.Exists(ImagePath))
        {
            Logo_RightBottomCorner.Source = BitmapFromFile(ImagePath);
            @CurrentPreviewCreator.LoadedImageInfo.OtherEffects.UpperLeftLogoImage = ImagePath.Replace("\\", "/");
            ExternElement("[C] * [Section:Other effects] Bottom right logo image (Label)", "Selected", Path.GetFileName(ImagePath));
        }
        else
        {
            Logo_RightBottomCorner.Source = BitmapFromResource("Limbus/Identity ¦ E.G.O  Preview Creator/Images/Logo/Right Bottom Corner.png");
            ExternElement("[C] * [Section:Other effects] Bottom right logo image (Label)", "Default");
        }
    }
















    private void SelectSkillsLocalization(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Limbus Json localization files (Skills)", ["json"]);
        if (Select.ShowDialog() == true) SelectSkillsLocalization_Actor(Select.FileName);
    }
    public void SelectSkillsLocalization_Actor(string Path)
    {
        if (File.Exists(Path))
        {
            FileInfo Target = new FileInfo(Path);

            var LoadedInfo = Target.Deserealize<SkillsFile>().dataList;
            if (LoadedInfo != null && LoadedInfo.Count > 0)
            {
                @CurrentPreviewCreator.LoadedFiles.Skills = LoadedInfo;
                @CurrentPreviewCreator.LoadedImageInfo.TextColumns.SelectedFiles.SkillsLocalization = Path.Replace("\\", "/");
                SkillLocalizationIDSelector.Items.Clear();

                foreach (Skill Skill in LoadedInfo)
                {
                    if (Skill.UptieLevels.Count > 0)
                    {
                        UptieLevel TargetUptie = Skill.UptieLevels[0];

                        string Name = $"{Skill.ID}: <color={GetAffinityColor_GameColors(TargetUptie.OptionalAffinity)}>{TargetUptie.Name}</color>";
                        AddItemToSelectionMenu(SkillLocalizationIDSelector, $"{Skill.ID}", Name, TargetUptie, TextIDFromLocalizationFile_Label);
                    }
                }
                ExternElement("[C] * [Section:Textual info/Text sources] Skills localization (Label)", "Selected", Target.Name);
            }
        }
        else
        {
            SkillLocalizationIDSelector.Items.Clear();
            ExternElement("[C] * [Section:Textual info/Text sources] Skills localization (Label)", "Default");
        }
    }

    private void SelectSkillsDisplayInfo(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("LCLI Skills Display Info files", ["json"]);
        if (Select.ShowDialog() == true) SelectSkillsDisplayInfo_Actor(Select.FileName);
    }
    public void SelectSkillsDisplayInfo_Actor(string Path)
    {
        if (File.Exists(Path))
        {
            FileInfo Target = new FileInfo(Path);

            var LoadedInfo = Target.Deserealize<SkillsDisplayInfo.SkillsDisplayInfoFile>(Context: Target.Directory.FullName.Replace("\\", "/")).List;
            if (LoadedInfo != null && LoadedInfo.Count > 0)
            {
                @CurrentPreviewCreator.LoadedFiles.Skills_DisplayInfo = LoadedInfo;
                @CurrentPreviewCreator.LoadedImageInfo.TextColumns.SelectedFiles.SkillsDisplayInfo = Path.Replace("\\", "/");
                SkillConstructorIDSelector.Items.Clear();

                foreach (SkillsDisplayInfo.SkillConstructor Constructor in LoadedInfo)
                {
                    string Name = $"{Constructor.ID}: <color={GetAffinityColor_GameColors(Constructor.Specific.Affinity)}>{Constructor.SkillName}</color>";
                    AddItemToSelectionMenu(SkillConstructorIDSelector, $"{Constructor.ID}", Name, Constructor, ConstructorIDFromDisplayInfoFile_Label);
                }
                ExternElement("[C] * [Section:Textual info/Text sources] Skills Display Info (Label)", "Selected", Target.Name);
            }
        }
        else
        {
            SkillConstructorIDSelector.Items.Clear();
            ExternElement("[C] * [Section:Textual info/Text sources] Skills Display Info (Label)", "Default");
        }
    }

    private void SelectPassivesLocalization(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Limbus Json localization files (Passives)", ["json"]);
        if (Select.ShowDialog() == true) SelectPassivesLocalization_Actor(Select.FileName);
    }
    public void SelectPassivesLocalization_Actor(string Path)
    {
        if (File.Exists(Path))
        {
            FileInfo Target = new FileInfo(Path);

            var LoadedInfo = Target.Deserealize<Type_Passives.PassivesFile>().dataList;
            if (LoadedInfo != null && LoadedInfo.Count > 0)
            {
                @CurrentPreviewCreator.LoadedFiles.Passives = LoadedInfo;
                @CurrentPreviewCreator.LoadedImageInfo.TextColumns.SelectedFiles.PassivesLocalization = Path.Replace("\\", "/");
                PassiveLocalizationIDSelector.Items.Clear();

                foreach (Type_Passives.Passive Passive in LoadedInfo)
                {
                    AddItemToSelectionMenu(PassiveLocalizationIDSelector, $"{Passive.ID}", $"{Passive.ID}: {Passive.Name}", Passive, TextIDFromLocalizationFile_Label);
                }
                ExternElement("[C] * [Section:Textual info/Text sources] Passives localization (Label)", "Selected", Target.Name);
            }
        }
        else
        {
            PassiveLocalizationIDSelector.Items.Clear();
            ExternElement("[C] * [Section:Textual info/Text sources] Passives localization (Label)", "Default");
        }
    }

    private void SelectKeywordsLocalization(object RequestSender, RoutedEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Limbus Json localization files (Keywords)", ["json"]);
        if (Select.ShowDialog() == true) SelectKeywordsLocalization_Actor(Select.FileName);
    }
    public void SelectKeywordsLocalization_Actor(string Path)
    {
        if (File.Exists(Path))
        {
            FileInfo Target = new FileInfo(Path);

            var LoadedInfo = Target.Deserealize<Type_Keywords.KeywordsFile>().dataList;
            if (LoadedInfo != null && LoadedInfo.Count > 0)
            {
                @CurrentPreviewCreator.LoadedFiles.Keywords = LoadedInfo;
                @CurrentPreviewCreator.LoadedImageInfo.TextColumns.SelectedFiles.KeywordsLocalization = Path.Replace("\\", "/");
                KeywordLocalizationIDSelector.Items.Clear();

                foreach (Type_Keywords.Keyword Keyword in LoadedInfo)
                {
                    string KeywordColor = "#9f6a3a";
                    if (Keyword.Color != null) KeywordColor = Keyword.Color;
                    else if (KeywordsInterrogation.Keywords_Bufs.ContainsKey(Keyword.ID)) KeywordColor = KeywordsInterrogation.Keywords_Bufs[Keyword.ID].StringColor;

                    string Name = $"{Keyword.ID}: <color={KeywordColor}>{Keyword.Name}</color>";
                    AddItemToSelectionMenu(KeywordLocalizationIDSelector, $"{Keyword.ID}", Name, Keyword, TextIDFromLocalizationFile_Label);
                }
                ExternElement("[C] * [Section:Textual info/Text sources] Keywords localization (Label)", "Selected", Target.Name);
            }
        }
        else
        {
            KeywordLocalizationIDSelector.Items.Clear();
            ExternElement("[C] * [Section:Textual info/Text sources] Keywords localization (Label)", "Default");
        }
    }



    private void AddItemToSelectionMenu(ComboBox Target, string Uid, string Text, object DataContextObject, FrameworkElement FontBinding)
    {
        @CurrentPreviewCreator.ComboBoxItemAddEvent = true;
        UITranslation_Rose MenuItem = new UITranslation_Rose()
        {
            Uid = Uid, // Uid used at image info loading to select item from context menu by ContextMenu.Items.Where(item => item.Uid == $"{TargetID}")
            RichText = Text,
            DataContext = DataContextObject,
            FontSize = 17,
            Padding = new Thickness(0, 5, 0, 5),
            Width = Target.Width - 15,
            TextWrapping = TextWrapping.Wrap,
            Effect = new DropShadowEffect() { ShadowDepth = 2, Color = ToColor("#191919"), BlurRadius = 1 },
        };
        MenuItem.BindSame(UITranslation_Rose.FontFamilyProperty, FontBinding);
        MenuItem.BindSame(UITranslation_Rose.FontWeightProperty, FontBinding);
        MenuItem.SetResourceReference(UITranslation_Rose.ForegroundProperty, "Theme:UIText.Foreground.IdentityOrEGOPreviewCreator");

        Target.Items.Add(MenuItem);
        Target.SelectedIndex = -1; // Do not automatically select last added
        @CurrentPreviewCreator.ComboBoxItemAddEvent = false;
    }
}
