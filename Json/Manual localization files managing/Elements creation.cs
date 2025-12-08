using LC_Localization_Task_Absolute.Json.Manual_localization_files_managing;
using LC_Localization_Task_Absolute.Mode_Handlers;
using System.Windows;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Mode_Handlers.Upstairs;

#pragma warning disable CS1633

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    private void AddAdditionalUptie(object RequestSender, RoutedEventArgs EventArgs)
    {
        string TargetUptieString = ((RequestSender as MenuItem).Parent as ContextMenu).PlacementTarget.Uid;

        int NewUptie = int.Parse(TargetUptieString);
        Type_Skills.Skill TargetCurrentSkill = Mode_Skills.DeserializedInfo.dataList.Where(x => x.ID == Mode_Skills.CurrentSkillID).First();

        Type_Skills.UptieLevel AppendUptie = new Type_Skills.UptieLevel()
        {
            Uptie = NewUptie,
            EGOAbnormalityName = PreviewLayout_EGOSkills_Background.Visibility == Visibility.Visible ? "" : null,
            Coins = new List<Type_Skills.Coin>()
            {
                #pragma Absolutely brand new
                new() { CoinDescriptions = new() { new() } },
                new() { CoinDescriptions = new() { new() } },
                new() { CoinDescriptions = new() { new() } },
                new() { CoinDescriptions = new() { new() } },
                new() { CoinDescriptions = new() { new() } },
                new() { CoinDescriptions = new() { new() } },
                new() { CoinDescriptions = new() { new() } },
                new() { CoinDescriptions = new() { new() } },
                new() { CoinDescriptions = new() { new() } },
                new() { CoinDescriptions = new() { new() } },
            }
        };

        TargetCurrentSkill.UptieLevels.Add(AppendUptie);
        TargetCurrentSkill.UptieLevels = [.. TargetCurrentSkill.UptieLevels.OrderBy(UptieLevel => UptieLevel.Uptie)];

        Mode_Skills.@Current.Skill[NewUptie] = AppendUptie;
        Mode_Skills.TransformToSkill(Mode_Skills.CurrentSkillID, NewUptie);
    }

    private void AppendAdditionalObject(object RequestSender, RoutedEventArgs EventArgs)
    {
        switch (ActiveProperties.Key)
        {
            case EditorMode.Skills:
                ObjectIDInputDialog SkillIDInput = new ObjectIDInputDialog(Mode: ObjectIDInputDialog.StringCheckMode.Skill);
                if (SkillIDInput.ShowDialog() == true)
                {
                    int NewSkillItem_ID = int.Parse(SkillIDInput.ResponseText);
                    Mode_Skills.DeserializedInfo.dataList.Add(new Type_Skills.Skill
                    (
                        AutoAddUptie: true,
                        AddAbNameToThisUptie: PreviewLayout_EGOSkills_Background.Visibility == Visibility.Visible
                    ) {
                        ID = NewSkillItem_ID
                    });
                    DelegateSkills[NewSkillItem_ID] = new Dictionary<int, Type_Skills.UptieLevel>()
                    {
                        [1] = Mode_Skills.DeserializedInfo.dataList[^1].UptieLevels[0]
                    };
                    Mode_Skills.TransformToSkill(NewSkillItem_ID);
                }
                break;

            case EditorMode.Passives:
                ObjectIDInputDialog PassiveIDInput = new ObjectIDInputDialog(Mode: ObjectIDInputDialog.StringCheckMode.Passive);
                if (PassiveIDInput.ShowDialog() == true)
                {
                    int NewPassiveItem_ID = int.Parse(PassiveIDInput.ResponseText);
                    Mode_Passives.DeserializedInfo.dataList.Add(new Type_Passives.Passive()
                    {
                        ID = NewPassiveItem_ID,
                    });
                    DelegatePassives[NewPassiveItem_ID] = Mode_Passives.DeserializedInfo.dataList[^1];
                    Mode_Passives.TransformToPassive(NewPassiveItem_ID);
                } 
                break;

            case EditorMode.Keywords:
                ObjectIDInputDialog KeywordIDInput = new ObjectIDInputDialog(Mode: ObjectIDInputDialog.StringCheckMode.Keyword);
                if (KeywordIDInput.ShowDialog() == true)
                {
                    string NewKeywordItem_ID = KeywordIDInput.ResponseText;
                    Mode_Keywords.DeserializedInfo.dataList.Add(new Type_Keywords.Keyword()
                    {
                        ID = NewKeywordItem_ID,
                        Name = NewKeywordItem_ID,
                    });
                    DelegateKeywords[NewKeywordItem_ID] = Mode_Keywords.DeserializedInfo.dataList[^1];
                    Mode_Keywords.TransformToKeyword(NewKeywordItem_ID);
                }
                break;

            default: break;
        }
    }
}
