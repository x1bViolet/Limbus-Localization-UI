using RichText;
using SixLabors.ImageSharp.PixelFormats;

using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Numerics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static LC_Localization_Task_Absolute.Json.BaseTypes;
using static LC_Localization_Task_Absolute.Json.Custom_Skills_Constructor;

using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator;
using static LC_Localization_Task_Absolute.Mode_Handlers.CustomIdentityPreviewCreator.ProjectFile.Sections;
using System.Runtime.Serialization;
using LC_Localization_Task_Absolute.Mode_Handlers;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    void ReconstructColumnItems()
    {
        List<Dictionary<string, AddedTextItems_Single>> Columns = new List<Dictionary<string, AddedTextItems_Single>>()
        {
            ProjectFile.LoadedProject.Text.FirstColumnItems,
            ProjectFile.LoadedProject.Text.SecondColumnItems,
        };

        int ColumnNumber = 1;
        foreach (Dictionary<string, AddedTextItems_Single> ColumnItemsData in Columns)
        {
            StackPanel TargetColumn = ColumnNumber == 1 ? IdentityPreviewItems_FirstColumn : IdentityPreviewItems_SecondColumn;

             rin($"Column {ColumnNumber}:");
            foreach (KeyValuePair<string, AddedTextItems_Single> ColumnItemData in ColumnItemsData)
            {
                string ItemUID = ColumnItemData.Key;
                AddedTextItems_Single ItemData = ColumnItemData.Value;

                if (ItemData.Type != null && ItemData.Type is string ItemType)
                {
                     rin($"- Item {ItemUID} ({ItemType}), ID {ItemData.SelectedTextIDFromLocalizationFile}");

                    ItemRepresenter CreatedColumnItem = CreatePlaceholder(ItemType,   ManualUID: ItemUID,   ManualInfoInsert: ItemData);
                    CreatedColumnItem.ColumnNumber = TargetColumn.Uid;

                    TargetColumn.Children.Add(CreatedColumnItem);

                    SetFocusOnColumnItem(CreatedColumnItem, UpdateSelectorsAndSliders: false); // Manually do all stuff there..

                    if (ItemData.SelectedTextIDFromLocalizationFile != null)
                    {
                        // Change selection in ComboBoxes again

                        switch (ItemType)
                        {
                            case "Skill":

                                int SelectedSkillTextID = (int)ItemData.SelectedTextIDFromLocalizationFile;
                                if (ID_And_Index__Links_Skills.ContainsKey(SelectedSkillTextID))
                                {
                                    CreatedColumnItem.SelectedLocalizationItemIndex = ID_And_Index__Links_Skills[SelectedSkillTextID];
                                    SkillsLocalizationIDSelector.SelectedIndex = CreatedColumnItem.SelectedLocalizationItemIndex;
                                }

                                if (ItemData.SelectedSkillConstructorFromDisplayInfoFile != null && ItemData.SelectedSkillConstructorFromDisplayInfoFile is BigInteger SelectedSkillDisplayInfoID)
                                {
                                    if (ID_And_Index__Links_SkillsDisplayInfo.ContainsKey(SelectedSkillDisplayInfoID))
                                    {
                                        CreatedColumnItem.SelectedSkillDisplayInfoConstructorIndex = ID_And_Index__Links_SkillsDisplayInfo[SelectedSkillDisplayInfoID];
                                        SkillsDisplayInfoIDSelector.SelectedIndex = CreatedColumnItem.SelectedSkillDisplayInfoConstructorIndex;
                                    }
                                }

                                SkillMainDescriptionWidthController.Value = ItemData.SkillMainDescriptionWidth;
                                SkillCoinDescriptionsWidthController.Value = ItemData.SkillCoinsDescriptionWidth;

                                break;


                            case "Passive":

                                int SelectedPassiveTextID = (int)ItemData.SelectedTextIDFromLocalizationFile;
                                if (ID_And_Index__Links_Passives.ContainsKey(SelectedPassiveTextID))
                                {
                                    CreatedColumnItem.SelectedLocalizationItemIndex = ID_And_Index__Links_Passives[SelectedPassiveTextID];
                                    PassivesLocalizationIDSelector.SelectedIndex = CreatedColumnItem.SelectedLocalizationItemIndex;
                                }

                                KeywordOrPassiveDescriptionWidthController.Value = ItemData.PassiveDescriptionWidth;

                                break;


                            case "Keyword":

                                string SelectedKeywordTextID = (string)ItemData.SelectedTextIDFromLocalizationFile;
                                if (ID_And_Index__Links_Keywords.ContainsKey(SelectedKeywordTextID))
                                {
                                    CreatedColumnItem.SelectedLocalizationItemIndex = ID_And_Index__Links_Keywords[SelectedKeywordTextID];
                                    KeywordsLocalizationIDSelector.SelectedIndex = CreatedColumnItem.SelectedLocalizationItemIndex;
                                }

                                if (File.Exists(ItemData.KeywordIconImage))
                                {
                                    SelectKeywordIconImage_Action(ItemData.KeywordIconImage);
                                }

                                break;
                        }
                        rin($" СВЕРХУ:{ItemData.VerticalOffset}");
                        ColumnItemVerticalOffsetControllder.Value = ItemData.VerticalOffset;
                        ColumnItemHorizontalOffsetControllder.Value = ItemData.HorizontalOffset;

                        NameMaxWidthController.Value = ItemData.NameMaxWidth;

                        SelectedItemSignature.Text = ItemData.TextItemSignature;
                    }



                    //#warning Slider values
                    //#warning Text signature last after displaying add
                }
            }

            rin();

            ColumnNumber++;
        }
    }
}