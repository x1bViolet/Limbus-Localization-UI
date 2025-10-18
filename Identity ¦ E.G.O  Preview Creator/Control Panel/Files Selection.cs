using LC_Localization_Task_Absolute.Json; // <- struct CompositionReference
using LC_Localization_Task_Absolute.PreviewCreator;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using static LC_Localization_Task_Absolute.Json.BaseTypes;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;
using static LC_Localization_Task_Absolute.Limbus_Integration.@ColorInfo;
using LC_Localization_Task_Absolute.Limbus_Integration;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    private void SelectPortraitImage(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectPortraitImage_Action(Select.FileName);
    }
    public void SelectPortraitImage_Action(string ImagePath)
    {
        CurrentInfo.LoadedImageInfo.Portrait.ImagePath = ImagePath.Replace("\\", "/");

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

    private void SelectImageLabelFont(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);
        if (Select.ShowDialog() == true) SelectImageLabelFont_Action(Select.FileName);
    }
    public void SelectImageLabelFont_Action(string FontPath)
    {
        if (FontPath.Equals("#Bebas Neue Bold")) ImageLabel.FontFamily = Resource<FontFamily>("BebasKaiUniversal");
        else
        {
            CurrentInfo.LoadedImageInfo.ImageLabelText.Font = FontPath.Replace("\\", "/");
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

    private void SelectCautionsFont(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);
        if (Select.ShowDialog() == true) SelectCautionsFont_Action(Select.FileName);
    }
    public void SelectCautionsFont_Action(string FontPath)
    {
        if (FontPath.Equals("#Bebas Neue Bold")) DecorativeCautions_PropertyBindingSource.FontFamily = Resource<FontFamily>("BebasKaiUniversal");
        else
        {
            CurrentInfo.LoadedImageInfo.Cautions.Font = FontPath.Replace("\\", "/");
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

    private void SelectItemSignaturesFont(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);
        if (Select.ShowDialog() == true) SelectItemSignaturesFont_Action(Select.FileName);
    }
    public void SelectItemSignaturesFont_Action(string FontPath)
    {
        if (FontPath.Equals("#Bebas Neue Bold")) CompositionResources.TextColumns.ItemSignaturesFont = Resource<FontFamily>("BebasKaiUniversal");
        else
        {
            CurrentInfo.LoadedImageInfo.TextColumns.ItemSignaturesFont = FontPath.Replace("\\", "/");
            CompositionResources.TextColumns.ItemSignaturesFont = FileToFontFamily(FontPath);
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

    private void SelectTextBackgroundEffectsImage(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectTextBackgroundEffectsImage_Action(Select.FileName);
    }
    public void SelectTextBackgroundEffectsImage_Action(string ImagePath)
    {
        CurrentInfo.LoadedImageInfo.TextBackgroundEffects.ImagePath = ImagePath.Replace("\\", "/");
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

    private void SelectOverlaySketchImage(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectOverlaySketchImage_Action(Select.FileName);
    }
    public void SelectOverlaySketchImage_Action(string ImagePath)
    {
        CurrentInfo.LoadedImageInfo.OverlaySketch.ImagePath = ImagePath.Replace("\\", "/");
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




    private void SelectUpperLeftLogoImage(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectUpperLeftLogoImage_Action(Select.FileName);
    }
    public void SelectUpperLeftLogoImage_Action(string ImagePath)
    {
        if (File.Exists(ImagePath))
        {
            Logo_LeftTopCorner.Source = BitmapFromFile(ImagePath);
            CurrentInfo.LoadedImageInfo.OtherEffects.UpperLeftLogoImage = ImagePath.Replace("\\", "/");
            ExternElement("[C] * [Section:Other effects] Upper left logo image (Label)", "Selected", Path.GetFileName(ImagePath));
        }
        else
        {
            Logo_LeftTopCorner.Source = BitmapFromResource("Identity ¦ E.G.O  Preview Creator/Images/Logo/Left Top Corner.png");
            ExternElement("[C] * [Section:Other effects] Upper left logo image (Label)", "Default");
        }
    }

    private void SelectBottomRightLogoImage(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
        if (Select.ShowDialog() == true) SelectBottomRightLogoImage_Action(Select.FileName);
    }
    public void SelectBottomRightLogoImage_Action(string ImagePath)
    {
        if (File.Exists(ImagePath))
        {
            Logo_RightBottomCorner.Source = BitmapFromFile(ImagePath);
            CurrentInfo.LoadedImageInfo.OtherEffects.UpperLeftLogoImage = ImagePath.Replace("\\", "/");
            ExternElement("[C] * [Section:Other effects] Bottom right logo image (Label)", "Selected", Path.GetFileName(ImagePath));
        }
        else
        {
            Logo_RightBottomCorner.Source = BitmapFromResource("Identity ¦ E.G.O  Preview Creator/Images/Logo/Right Bottom Corner.png");
            ExternElement("[C] * [Section:Other effects] Bottom right logo image (Label)", "Default");
        }
    }
















    /////////////////////////////////////////////////////////////////////////////////////////////////////
    private void SelectSkillsLocalization(object RequestSender, MouseButtonEventArgs EventArgs)
    {
        OpenFileDialog Select = NewOpenFileDialog("Limbus Json localization files (Skills)", ["json"]);
        if (Select.ShowDialog() == true) SelectSkillsLocalization_Actor(Select.FileName);
    }
    public void SelectSkillsLocalization_Actor(string Path)
    {
        if (File.Exists(Path))
        {
            FileInfo Target = new FileInfo(Path);

            var LoadedInfo = Target.Deserealize<Type_Skills.SkillsFile>().dataList;
            if (LoadedInfo != null && LoadedInfo.Count > 0)
            {
                CurrentInfo.LoadedFiles.Skills = LoadedInfo;
                CurrentInfo.LoadedImageInfo.TextColumns.SelectedFiles.SkillsLocalization = Path.Replace("\\", "/");
                SkillLocalizationIDSelector.Items.Clear();

                foreach (Type_Skills.Skill Skill in LoadedInfo)
                {
                    if (Skill.UptieLevels.Count > 0)
                    {
                        Type_Skills.UptieLevel TargetUptie = Skill.UptieLevels[0];

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

    private void SelectSkillsDisplayInfo(object RequestSender, MouseButtonEventArgs EventArgs)
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
                CurrentInfo.LoadedFiles.Skills_DisplayInfo = LoadedInfo;
                CurrentInfo.LoadedImageInfo.TextColumns.SelectedFiles.SkillsDisplayInfo = Path.Replace("\\", "/");
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

    private void SelectPassivesLocalization(object RequestSender, MouseButtonEventArgs EventArgs)
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
                CurrentInfo.LoadedFiles.Passives = LoadedInfo;
                CurrentInfo.LoadedImageInfo.TextColumns.SelectedFiles.PassivesLocalization = Path.Replace("\\", "/");
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

    private void SelectKeywordsLocalization(object RequestSender, MouseButtonEventArgs EventArgs)
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
                CurrentInfo.LoadedFiles.Keywords = LoadedInfo;
                CurrentInfo.LoadedImageInfo.TextColumns.SelectedFiles.KeywordsLocalization = Path.Replace("\\", "/");
                KeywordLocalizationIDSelector.Items.Clear();

                foreach (Type_Keywords.Keyword Keyword in LoadedInfo)
                {
                    string KeywordColor = "#9f6a3a";
                    if (Keyword.Color != null) KeywordColor = Keyword.Color;
                    else if (KeywordsInterrogate.KeywordsGlossary.ContainsKey(Keyword.ID)) KeywordColor = KeywordsInterrogate.KeywordsGlossary[Keyword.ID].StringColor;

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
        CurrentInfo.ComboBoxItemAddEvent = true;
        UITranslation_Rose MenuItem = new UITranslation_Rose()
        {
            Uid = Uid, // Uid used at image info loading to select item from context menu by ContextMenu.Items.Where(item => item.Uid.Equals($"{TargetID}"))
            RichText = Text,
            DataContext = DataContextObject,
            FontSize = 17,
            Padding = new Thickness(0, 5, 0, 5),
            Width = Target.Width - 15,
            TextWrapping = TextWrapping.Wrap,
            Effect = new DropShadowEffect() { ShadowDepth = 2, Color = ToColorBrush("#191919"), BlurRadius = 1 },
        };
        MenuItem.BindSame(UITranslation_Rose.FontFamilyProperty, FontBinding);
        MenuItem.BindSame(UITranslation_Rose.FontWeightProperty, FontBinding);
        MenuItem.SetResourceReference(UITranslation_Rose.ForegroundProperty, "Theme_UIText__Foreground_IdentityOrEGOPreviewCreator");

        Target.Items.Add(MenuItem);
        Target.SelectedIndex = -1; // Do not automatically select last added
        CurrentInfo.ComboBoxItemAddEvent = false;
    }
}
