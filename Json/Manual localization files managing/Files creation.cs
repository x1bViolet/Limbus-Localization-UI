using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Json.Manual_localization_files_managing;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.Json.BaseTypes;
using static LC_Localization_Task_Absolute.Requirements;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211
#pragma warning disable CS1696

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    private void CreatePassivesFile(object RequestSender, RoutedEventArgs EventArgs)
    {
        ObjectIDInputDialog PassiveIDInput = new ObjectIDInputDialog(Mode: ObjectIDInputDialog.StringCheckMode.Passive, CheckCurrentIDLists: false);
        if (PassiveIDInput.ShowDialog() == true)
        {
            Type_Passives.PassivesFile Created = new Type_Passives.PassivesFile()
            {
                ManualFileType = "Passives",
                dataList = new List<Type_Passives.Passive>()
                {
                    new Type_Passives.Passive()
                    {
                        ID = int.Parse(PassiveIDInput.ResponseText)
                    }
                }
            };
            SaveNewLimbusLocalizationFile(Created, "Passives-….json");
        }
    }
    private void CreateKeywordsFile(object RequestSender, RoutedEventArgs EventArgs)
    {
        ObjectIDInputDialog KeywordIDInput = new ObjectIDInputDialog(Mode: ObjectIDInputDialog.StringCheckMode.Keyword, CheckCurrentIDLists: false);
        if (KeywordIDInput.ShowDialog() == true)
        {
            Type_Keywords.KeywordsFile Created = new Type_Keywords.KeywordsFile()
            {
                ManualFileType = "Keywords (BattleKeywords)",
                dataList = new List<Type_Keywords.Keyword>()
                {
                    new Type_Keywords.Keyword()
                    {
                        ID = KeywordIDInput.ResponseText,
                        Name = KeywordIDInput.ResponseText
                    }
                }
            };
            SaveNewLimbusLocalizationFile(Created, "BattleKeywords-….json");
        }
    }
    private void CreateSkillsFile(object RequestSender, RoutedEventArgs EventArgs)
    {
        ObjectIDInputDialog SkillIDInput = new ObjectIDInputDialog(Mode: ObjectIDInputDialog.StringCheckMode.Skill, CheckCurrentIDLists: false);
        if (SkillIDInput.ShowDialog() == true)
        {
            string Type = (RequestSender as MenuItem).Uid;

            Type_Skills.SkillsFile Created = new Type_Skills.SkillsFile()
            {
                ManualFileType = Type switch
                {
                    "No upties" => "Skills",
                    "Identities" => "Skills (With upties)",
                    "E.G.O Skills" => "Skills (With upties; With abName)",
                },
                dataList = new()
                {
                    new Type_Skills.Skill(AutoAddUptie: true, AddAbNameToThisUptie: Type.Equals("E.G.O Skills"))
                    {
                        ID = int.Parse(SkillIDInput.ResponseText)
                    }
                }
            };

            SaveNewLimbusLocalizationFile(Created, Type switch
            {
                "No upties" => "Skills-….json",
                "Identities" => "Skills_personality-….json",
                "E.G.O Skills" => "Skills_Ego_Personality-….json",
            });
        }
    }

    public void SaveNewLimbusLocalizationFile(object JsonToSave, string InitialName)
    {
        SaveFileDialog SaveLocation = NewSaveFileDialog("Json files", ["json"], InitialName);
        if (SaveLocation.ShowDialog() == true)
        {
            JsonToSave.SerializeFormattedFile(SaveLocation.FileName);

            LoadFileAndSetFocus(SaveLocation.FileName);
        }
    }
}
