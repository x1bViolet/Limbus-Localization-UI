using LC_Localization_Task_Absolute.Mode_Handlers;
using Microsoft.Win32;
using System.Windows.Input;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    /*/—------------------------------------------------------------------------------—/*/
    /*/ File for PreviewMouseLeftButtonUp triggers of borders(Buttons) with            /*/
    /*/ ButtonDefaultDeskHighlightable Style from Custom Identity Preview Creator mode /*/
    /*/—------------------------------------------------------------------------------—/*/

    #region Identity or E.G.O portrait
    void SelectIdentityOrEGOPortrait(object RequestSender /* Border {ButtonDefaultDeskHighlightable} */, MouseButtonEventArgs EventArgs)
    {
        var Select = NewOpenFileDialog("Image files", ["jpg", "png"]);

        if (Select.ShowDialog() == true) // If file was selected
        {
            SelectIdentityOrEGOPortrait_Action(Select.FileName);

            ProjectFile.LoadedProject.ImageParameters.PortraitImage = Select.FileName.Replace("\\", "/");
        }
    }
    private void SelectIdentityOrEGOPortrait_Action(string Filepath)
    {
        IdentityPreviewCreator_IdentityPortrait.Source = GenerateBitmapFromFile(Filepath);

        IdentityPreviewCreator_IdentityCustomPortraitSelector__DisplayingSign.Text = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] * [Section:Image parameters] Portrait image").Extern(Filepath.GetName());
    }
    #endregion



    #region Image parameters
    void SelectImageTypeSignFont(object RequestSender /* Border {ButtonDefaultDeskHighlightable} */, MouseButtonEventArgs EventArgs)
    {
        var Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);

        if (Select.ShowDialog() == true)
        {
            SelectImageTypeSignFont_Action(Select.FileName);

            ProjectFile.LoadedProject.ImageParameters.ImageTypeSign_AnotherFont = Select.FileName.Replace("\\", "/");
        }
    }
    private void SelectImageTypeSignFont_Action(string Filepath)
    {
        string FontName;
        CustomIdentityPreviewCreator_ImageTypeText.FontFamily = FileToFontFamily_WithNameReturn(Filepath, out FontName);

        SelectImageTypeSignFont_Label.Text = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] * [Section:Image parameters] Another image type font", "Selected").Extern(FontName);
    }
    #endregion



    #region Custom sinner icon
    /// <summary>
    /// Trigger activation of <c>SelectCustomSinnerIcon()</c> on re-selection '[Custom]' sinner icon while it is already selected
    /// <br/>
    /// (Selecting the same item in ComboBox does not trigger <c>SelectionChanged</c>)
    /// </summary>
    void SelectCustomSinnerIcon_ManualTrigger(object RequestSender /* Border {ButtonDefaultDeskHighlightable} */, MouseButtonEventArgs EventArgs)
    {
        if (IdentityPreviewCreator_SinnerIconSelector.SelectedIndex == 12)
        {
            SelectCustomSinnerIcon();
        }
    }
    
    /// <summary>
    /// Being called from <c>SelectionChanged</c> event from 
    /// </summary>
    private void SelectCustomSinnerIcon()
    {
        var Select = NewOpenFileDialog("Image files", ["jpg", "png"]);

        if (Select.ShowDialog() == true)
        {
            SelectCustomSinnerIcon_Action(Select.FileName);

            ProjectFile.LoadedProject.Specific.SinnerIcon = Select.FileName.Replace("\\", "/");
        }
    }
    private void SelectCustomSinnerIcon_Action(string Filepath)
    {
        IdentityPreviewCreator_SinnerIcon.Source
            = IdentityPreviewCreator_SinnerIconSelector_CustomSinnerIcon_Image.Source
            = GenerateBitmapFromFile(Filepath);

        IdentityPreviewCreator_SinnerIconSelector_CustomSinnerIcon_Image.Visibility = Visible;
    }
    #endregion



    #region Decorative cautions custom font
    void SelectDecorativeCautionsCustomFont(object RequestSender /* Border {ButtonDefaultDeskHighlightable} */, MouseButtonEventArgs EventArgs)
    {
        var Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);

        if (Select.ShowDialog() == true)
        {
            SelectDecorativeCautionsCustomFont_Action(Select.FileName);

            ProjectFile.LoadedProject.DecorativeCautions.CustomText.AnotherFont = Select.FileName.Replace("\\", "/");
        }
    }
    private void SelectDecorativeCautionsCustomFont_Action(string Filepath)
    {
        string FontName;
        CustomCautionsParamBinder.FontFamily = FileToFontFamily_WithNameReturn(Filepath, out FontName);

        IdentityPreviewCreator_CautionType_CustomCautionFontTitle.Text = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] * [Section:Decorative cautions] Another font", "Selected").Extern(FontName);
    }
    #endregion



    #region Localization files and skills display info selection
    void SelectAnotherItemSignsFont(object RequestSender /* Border {ButtonDefaultDeskHighlightable} */, MouseButtonEventArgs EventArgs)
    {
        var Select = NewOpenFileDialog("Font files", ["ttf", "otf"]);

        if (Select.ShowDialog() == true)
        {
            ProjectFile.LoadedProject.Text.ItemSignaturesAnotherFont = Select.FileName.Replace("\\", "/");

            SelectAnotherItemSignsFont_Action(Select.FileName);
        }
    }
    private void SelectAnotherItemSignsFont_Action(string Filepath)
    {
        string FontName;
        ItemSignaturesFontBinder.FontFamily = FileToFontFamily_WithNameReturn(Filepath, out FontName);

        IdentityPreviewCreator_AnotherItemSignsFont_Label.Text = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] * [Section:Text info/Column settings] Another font for item signatures", "Selected").Extern(FontName);
    }

    void SelectSkillsLocalizationFile(object RequestSender /* Border {ButtonDefaultDeskHighlightable} */, MouseButtonEventArgs EventArgs)
    {
        var Select = NewOpenFileDialog("Skills-type Limbus localization file", ["json"]);

        if (Select.ShowDialog() == true)
        {
            ProjectFile.LoadedProject.Text.SkillsLocalizationFile = Select.FileName.Replace("\\", "/");

            UpdateSelector__SkillLocalizationIDSelector();
        }
    }
    #region Display info
    void SelectSkillsDisplayInfoFile(object RequestSender /* Border {ButtonDefaultDeskHighlightable} */, MouseButtonEventArgs EventArgs)
    {
        var Select = NewOpenFileDialog("Skills Display Info constructor-type file", ["json"]);

        if (Select.ShowDialog() == true)
        {
            ProjectFile.LoadedProject.Text.SkillsDisplayInfoConstructorFile = Select.FileName.Replace("\\", "/");

            UpdateSelector__SkillsDisplayInfoIDSelector();
        }
    }
    #endregion

    void SelectPassivesLocalizationFile(object RequestSender /* Border {ButtonDefaultDeskHighlightable} */, MouseButtonEventArgs EventArgs)
    {
        var Select = NewOpenFileDialog("Passives-type Limbus localization file", ["json"]);

        if (Select.ShowDialog() == true)
        {
            ProjectFile.LoadedProject.Text.PassivesLocalizationFile = Select.FileName.Replace("\\", "/");

            UpdateSelector__PassivesLocalizationIDSelector();
        }
    }

    void SelectKeywordsLocalizationFile(object RequestSender /* Border {ButtonDefaultDeskHighlightable} */, MouseButtonEventArgs EventArgs)
    {
        var Select = NewOpenFileDialog("Keywords-type(Bufs/BattleKeywords) Limbus localization file", ["json"]);

        if (Select.ShowDialog() == true)
        {
            ProjectFile.LoadedProject.Text.KeywordsLocalizationFile = Select.FileName.Replace("\\", "/");

            UpdateSelector__KeywordsLocalizationIDSelector();
        }
    }
    
    void SelectKeywordIconImage(object RequestSender /* Border {ButtonDefaultDeskHighlightable} */, MouseButtonEventArgs EventArgs)
    {
        var Select = NewOpenFileDialog("Image Files", ["png", "jpg"]);

        if (Select.ShowDialog() == true)
        {
            SelectKeywordIconImage_Action(Select.FileName);
        }
    }
    private void SelectKeywordIconImage_Action(string Filepath)
    {
        FocusedColumnItem.ItemInfo.KeywordIconImage = Filepath.Replace("\\", "/");
        FocusedColumnItem.KeywordIcon.Source = GenerateBitmapFromFile(Filepath);

        KeywordIconSelectionLabel.Text = ᐁ_Interface_Localization_Loader.ExternTextFor("[C] * [Section:Text info/Selected item settings] Keyword icon image", "Selected").Extern(Filepath.GetName());
    }
    #endregion
}