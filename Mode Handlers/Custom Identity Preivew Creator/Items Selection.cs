using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Passives;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.Custom_Skills_Constructor;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Windows.Visibility;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    /*/—-----------------------------------------------------------------------------------—/*/
    /*/ File for Selection triggers of ComboBoxes from Custom Identity Preview Creator mode /*/
    /*/—-----------------------------------------------------------------------------------—/*/

    #region Image parameters
    void ChangePortraitType(object RequestSender /* ComboBox */, SelectionChangedEventArgs EventArgs)
    {
        Flow(() =>
        {
            if ((PortraitTypeSelector.SelectedItem as UIElement).Uid.Equals("E.G.O"))
            {
                Mark_NotAnOfficial.Text = "※This is an unofficial E.G.O Preview image created by using Limbus Company Localization Interface";

                IdentityPortraitImage_ParentGrid.Visibility = Collapsed;
                EGOPortraitImage_ParentGrid.Visibility = Visible;

                MakeAvailable(EGOPortraitScaleController_ParentGrid);
                MakeUnavailable(IdentityPortraitScaleController_ParentGrid);

                ProjectFile.LoadedProject.ImageParameters.Type = "E.G.O";
            }
            else
            {
                Mark_NotAnOfficial.Text = "※This is an unofficial Identity Preview image created by using Limbus Company Localization Interface";

                IdentityPortraitImage_ParentGrid.Visibility = Visible;
                EGOPortraitImage_ParentGrid.Visibility = Collapsed;

                MakeAvailable(IdentityPortraitScaleController_ParentGrid);
                MakeUnavailable(EGOPortraitScaleController_ParentGrid);

                ProjectFile.LoadedProject.ImageParameters.Type = "Identity";
            }
        });
    }
    #endregion



    #region Specific of the sinner and Identity/E.G.O
    void SelectSinnerIcon(object RequestSender /* ComboBox */, SelectionChangedEventArgs EventArgs)
    {
        if (IdentityPreviewCreator_SinnerIconSelector.SelectedIndex != 12) // If not [Custom]
        {
            Image SelectedSinnerIcon = (IdentityPreviewCreator_SinnerIconSelector.SelectedItem as StackPanel).Children[0] as Image;
            IdentityPreviewCreator_SinnerIcon.Source = SelectedSinnerIcon.Source;

            IdentityPreviewCreator_TextEntries_SinnerName.Text
                = ProjectFile.LoadedProject.Specific.SinnerIcon
                = SelectedSinnerIcon.Uid; // Uid of Image is set as Sinner name

            IdentityPreviewCreator_TextEntries_ElementsColor.Text = SinnerColors[SelectedSinnerIcon.Uid];

            // Hide custom icon on item in selection list
            IdentityPreviewCreator_SinnerIconSelector_CustomSinnerIcon_Image.Visibility = Collapsed;
            IdentityPreviewCreator_SinnerIconSelector_CustomSinnerIcon_Image.Source = new BitmapImage();
        }
        else
        {
            SelectCustomSinnerIcon(); // -> Files Selection.cs
        }
    }
    
    void SelectRarityOrEGORiskLevel(object RequestSender /* ComboBox */, SelectionChangedEventArgs EventArgs)
    {
        if (EventArgs.AddedItems.Count > 0)
        {
            Image Selected = EventArgs.AddedItems[0] as Image;

            IdentityRarityOrEGORiskLevel.Source = Selected.Source;
            ProjectFile.LoadedProject.Specific.RarityOrEGORiskLevel = Selected.Uid;

            IdentityPreviewCreator_IdentityHeader_RarityOrEGORiskLevelSelector.Padding = new Thickness(
                (RequestSender as ComboBox).SelectedIndex switch { 
                  0 => -89,
                  1 => -102,
                  2 => -115,
                  _ => -68,
                },
                3, 0, 0
            );
        }
    }
    #endregion



    #region Decorative cautions type
    /// <summary>
    /// Condition not to suddenly change the type of Cautions in the project while updating Ambience color
    /// </summary>
    private bool CanOverwriteProjectCautionsType = false;

    private BitmapImage Caution_PART = ImageFromResource(@"UI/Limbus/Custom Identity Preview/Decorative Warning (Part).png");
    private BitmapImage Caution_SEASON = ImageFromResource(@"UI/Limbus/Custom Identity Preview/Decorative Warning (Season).png");
    private BitmapImage Caution_CAUTION = ImageFromResource(@"UI/Limbus/Custom Identity Preview/Decorative Warning (Caution).png");

    void SelectCautionsType(object RequestSender /* ComboBox */, SelectionChangedEventArgs EventArgs)
    {
        string Selected = ((RequestSender as ComboBox).SelectedItem as UIElement).Uid;
        
        IdentityPreviewCreator_DecorativeCautions.Visibility = Selected.Equals("None") ? Collapsed : Visible;
        IdentityPreviewCreator_CautionType_SubCategory_CustomCaution.Visibility = Selected.Equals("Custom text") ? Visible : Collapsed;

        if (Selected.EqualsOneOf("SEASON", "CAUTION"))
        {
            BitmapImage ColoredCaution = SecondaryUtilities.TintWhiteMaskBitmap
            (
                Selected: Selected.Equals("SEASON") ? Caution_SEASON : Caution_CAUTION,
                HexColor: IdentityPreviewCreator_TextEntries_ElementsColor.Text
            );

            DecorativeCaution_Top.Children.Clear();
            DecorativeCaution_Bottom.Children.Clear();

            for (int i = 0; i <= 25; i++)
            {
                DecorativeCaution_Top.Children.Add(new Image() { Source = ColoredCaution });
                DecorativeCaution_Bottom.Children.Add(new Image() { Source = ColoredCaution });
            }

            if (CanOverwriteProjectCautionsType) ProjectFile.LoadedProject.DecorativeCautions.CautionType = Selected;
        }
        else if (Selected.Equals("Custom text"))
        {
            IdentityPreviewCreator_RefreshCustomCautions();

            if (CanOverwriteProjectCautionsType) ProjectFile.LoadedProject.DecorativeCautions.CautionType = "Custom text";
        }
        else if (Selected.Equals("None"))
        {
            if (CanOverwriteProjectCautionsType) ProjectFile.LoadedProject.DecorativeCautions.CautionType = "None";
        }
    }
    #endregion



    #region Localization and Skills Display Info ID selection
    void SelectSkillsLocalization(object RequestSender /* ComboBox */, SelectionChangedEventArgs EventArgs)
    {
        if (!ColumnItemFocusingEvent & EventArgs.AddedItems.Count > 0)
        {
            FocusedColumnItem.SelectedLocalizationItemIndex = SkillsLocalizationIDSelector.SelectedIndex;

            FocusedColumnItem.ItemInfo.SelectedTextIDFromLocalizationFile = (EventArgs.AddedItems[0] as UILocalization_Grocerius).UniversalDataBindings["Attached item ID"];

            CheckSkillSelectorsAndGenerateSkillDisplayer();
        }
    }
    #region Skills Display info
    void SelectSkillsDisplayInfo(object RequestSender /* ComboBox */, SelectionChangedEventArgs EventArgs)
    {
        if (!ColumnItemFocusingEvent & EventArgs.AddedItems.Count > 0)
        {
            FocusedColumnItem.SelectedSkillDisplayInfoConstructorIndex = SkillsDisplayInfoIDSelector.SelectedIndex;

            FocusedColumnItem.ItemInfo.SelectedSkillConstructorFromDisplayInfoFile = (SkillsDisplayInfoIDSelector.SelectedItem as UILocalization_Grocerius).UniversalDataBindings["Attached item ID"];

            CheckSkillSelectorsAndGenerateSkillDisplayer();
        }
    }
    private void CheckSkillSelectorsAndGenerateSkillDisplayer() // Chack both values and create skill if possible
    {
        if (SkillsLocalizationIDSelector.SelectedIndex != -1 & SkillsDisplayInfoIDSelector.SelectedIndex != -1)
        {
            UptieLevel TextInfo = (SkillsLocalizationIDSelector.SelectedItem as UILocalization_Grocerius).UniversalDataBindings["Attached item"];

            SkillContstructor DisplayInfo = (SkillsDisplayInfoIDSelector.SelectedItem as UILocalization_Grocerius).UniversalDataBindings["Attached item"];

            GenerateSkillDisplayerForFocusedItem(TextInfo, DisplayInfo);

            if (FocusedColumnItem.Children.Count == 2) MakeAvailable(ItemSignatureInput_ParentGrid);
        }
    }
    #endregion

    void SelectPassiveLocalization(object RequestSender /* ComboBox */, SelectionChangedEventArgs EventArgs)
    {
        if (!ColumnItemFocusingEvent & EventArgs.AddedItems.Count > 0)
        {
            FocusedColumnItem.SelectedLocalizationItemIndex = PassivesLocalizationIDSelector.SelectedIndex;
            FocusedColumnItem.ItemInfo.SelectedTextIDFromLocalizationFile = (EventArgs.AddedItems[0] as UILocalization_Grocerius).UniversalDataBindings["Attached item ID"];

            CheckPassiveSelectorAndGeneratePassiveDisplayer();

            if (FocusedColumnItem.Children.Count == 2) MakeAvailable(ItemSignatureInput_ParentGrid);
        }
    }
    private void CheckPassiveSelectorAndGeneratePassiveDisplayer()
    {
        if (PassivesLocalizationIDSelector.SelectedIndex != -1)
        {
            Passive TextInfo = (PassivesLocalizationIDSelector.SelectedItem as UILocalization_Grocerius).UniversalDataBindings["Attached item"];

            GeneratePassiveDisplayerForFocusedItem(TextInfo);
        }
    }
    
    void SelectKeywordLocalization(object RequestSender /* ComboBox */, SelectionChangedEventArgs EventArgs)
    {
        if (!ColumnItemFocusingEvent & EventArgs.AddedItems.Count > 0)
        {
            FocusedColumnItem.SelectedLocalizationItemIndex = KeywordsLocalizationIDSelector.SelectedIndex;
            FocusedColumnItem.ItemInfo.SelectedTextIDFromLocalizationFile = (EventArgs.AddedItems[0] as UILocalization_Grocerius).UniversalDataBindings["Attached item ID"];

            CheckKeywordSelectorAndGenerateKeywordDisplayer();
        }
    }
    void CheckKeywordSelectorAndGenerateKeywordDisplayer()
    {
        if (KeywordsLocalizationIDSelector.SelectedIndex != -1)
        {
            MakeAvailable(KeywordIconFileSelectButton_ParentGrid);

            Keyword TextInfo = (KeywordsLocalizationIDSelector.SelectedItem as UILocalization_Grocerius).UniversalDataBindings["Attached item"];

            GenerateKeywordDisplayerForFocusedItem(TextInfo);
        }
    }
    #endregion
}
