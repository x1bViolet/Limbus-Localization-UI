using LC_Localization_Task_Absolute.Mode_Handlers;
using static LC_Localization_Task_Absolute.Json.BaseTypes;

namespace LC_Localization_Task_Absolute.Json
{
    /// <summary>
    /// Dictionaries with links to deserialized json classes for simplified id access
    /// </summary>
    internal abstract class DelegateDictionaries
    {
        internal protected static Dictionary<int, Dictionary<int, Type_Skills.UptieLevel>> DelegateSkills = [];
            internal protected static List<int> DelegateSkills_IDList = [];
        
        internal protected static Dictionary<int, Type_Passives.Passive> DelegatePassives = [];
            internal protected static List<int> DelegatePassives_IDList = [];
        
        internal protected static Dictionary<int, Type_EGOGifts.EGOGift> DelegateEGOGifts = [];
            internal protected static List<int> DelegateEGOGifts_IDList = [];
        
        internal protected static Dictionary<string, Type_Keywords.Keyword> DelegateKeywords = [];
            internal protected static List<string> DelegateKeywords_IDList = [];

        internal protected static Dictionary<dynamic, Type_ContentBasedUniversal.ContentBasedUniversal> DelegateUniversal = [];
            internal protected static List<dynamic> DelegateUniversal_IDList = [];

        internal protected static List<dynamic> Delegates = [
            DelegateSkills,    DelegateSkills_IDList,
            DelegatePassives,  DelegatePassives_IDList,
            DelegateKeywords,  DelegateEGOGifts_IDList,
            DelegateEGOGifts,  DelegateKeywords_IDList,
            DelegateUniversal, DelegateUniversal_IDList,
        ];

        internal protected static void ClearDelegates()
        {
            foreach (dynamic Delegate in Delegates) Delegate.Clear();
        }

        internal protected static void InitializeSkillsDelegateFrom(Type_Skills.Skills? Source)
        {
            if (Source != null)
            {
                if (Source.dataList != null)
                {
                    DelegateSkills.Clear();
                    DelegateSkills_IDList.Clear();
                    Mode_Skills.Skills_NameIDs.Clear();

                    foreach(Type_Skills.Skill CurrentSkill in Source.dataList)
                    {
                        DelegateSkills[CurrentSkill.ID] = new Dictionary<int, Type_Skills.UptieLevel>();
                        foreach(Type_Skills.UptieLevel CurrentUptieLevel in CurrentSkill.UptieLevels)
                        {
                            DelegateSkills[CurrentSkill.ID][CurrentUptieLevel.Uptie] = CurrentUptieLevel;

                            Mode_Skills.Skills_NameIDs[DelegateSkills[CurrentSkill.ID][CurrentUptieLevel.Uptie].Name.Trim()] = CurrentSkill.ID;
                        }
                    }

                    DelegateSkills_IDList = DelegateSkills.Keys.ToList();
                }
            }
        }

        internal protected static void InitializePassivesDelegateFrom(Type_Passives.Passives Source)
        {
            if (Source != null)
            {
                if (Source.dataList != null)
                {
                    DelegatePassives.Clear();
                    DelegatePassives_IDList.Clear();
                    Mode_Passives.Passives_NameIDs.Clear();

                    foreach (Type_Passives.Passive CurrentPassive in Source.dataList)
                    {
                        DelegatePassives[CurrentPassive.ID] = CurrentPassive;
                        Mode_Passives.Passives_NameIDs[CurrentPassive.Name.Trim()] = CurrentPassive.ID;
                    }

                    DelegatePassives_IDList = DelegatePassives.Keys.ToList();
                }
            }
        }

        internal protected static void InitializeKeywordsDelegateFrom(Type_Keywords.Keywords Source)
        {
            if (Source != null)
            {
                if (Source.dataList != null)
                {
                    DelegateKeywords.Clear();
                    DelegateKeywords_IDList.Clear();
                    Mode_Keywords.Keywords_NameIDs.Clear();

                    foreach (Type_Keywords.Keyword CurrentKeyword in Source.dataList)
                    {
                        DelegateKeywords[CurrentKeyword.ID] = CurrentKeyword;
                        Mode_Keywords.Keywords_NameIDs[CurrentKeyword.Name.Trim()] = CurrentKeyword.ID;
                    }

                    DelegateKeywords_IDList = DelegateKeywords.Keys.ToList();
                }
            }
        }

        internal protected static void InitializeEGOGiftsDelegateFrom(Type_EGOGifts.EGOGifts Source)
        {
            if (Source != null)
            {
                if (Source.dataList != null)
                {
                    DelegateEGOGifts.Clear();
                    DelegateEGOGifts_IDList.Clear();
                    Mode_EGOGifts.EGOGifts_NameIDs.Clear();

                    foreach (Type_EGOGifts.EGOGift CurrentKeyword in Source.dataList)
                    {
                        DelegateEGOGifts[CurrentKeyword.ID] = CurrentKeyword;
                        Mode_EGOGifts.EGOGifts_NameIDs[CurrentKeyword.Name.Trim()] = CurrentKeyword.ID;
                    }

                    DelegateEGOGifts_IDList = DelegateEGOGifts.Keys.ToList();
                }
            }
        }

        internal protected static void InitializeContentBasedUniversalDelegateFrom(Type_ContentBasedUniversal.ContentBasedUniversal? Source)
        {
            if (Source != null)
            {
                if (Source.dataList != null)
                {
                    if (Source.dataList.Count > 0)
                    {
                        DelegateUniversal.Clear();
                        DelegateUniversal_IDList.Clear();
                        Mode_Passives.Passives_NameIDs.Clear();

                        Type CheckItem = Source.dataList[0].GetType();
                        if (CheckItem.HasProperty("id") & CheckItem.HasProperty("content"))
                        {
                        
                            foreach (dynamic CurrentItem in Source.dataList)
                            {
                                DelegateUniversal[CurrentItem.id] = CurrentItem;
                                //Mode_Passives.Passives_NameIDs[CurrentPassive.Name.Trim()] = CurrentPassive.ID;
                            }

                            DelegatePassives_IDList = DelegatePassives.Keys.ToList();
                        }
                    }
                }
            }
        }
    }
}