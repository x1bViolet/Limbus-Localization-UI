using LC_Localization_Task_Absolute.Mode_Handlers;
using static LC_Localization_Task_Absolute.Json.BaseTypes;

namespace LC_Localization_Task_Absolute.Json
{
    /// <summary>
    /// Dictionaries with links to deserialized json records for simplified id access
    /// </summary>
    public abstract class DelegateDictionaries
    {
        public static Dictionary<int, Dictionary<int, Type_Skills.UptieLevel>> DelegateSkills = [];
            public static List<int> DelegateSkills_IDList = [];
        
        public static Dictionary<int, Type_Passives.Passive> DelegatePassives = [];
            public static List<int> DelegatePassives_IDList = [];
        
        public static Dictionary<int, Type_EGOGifts.EGOGift> DelegateEGOGifts = [];
            public static List<int> DelegateEGOGifts_IDList = [];
        
        public static Dictionary<string, Type_Keywords.Keyword> DelegateKeywords = [];
            public static List<string> DelegateKeywords_IDList = [];

        public static Dictionary<dynamic, Type_ContentBasedUniversal_UNUSEDPROBABLYUSELESS.ContentBasedUniversal> DelegateUniversal = [];
            public static List<dynamic> DelegateUniversal_IDList = [];

        public static List<dynamic> Delegates = [
            DelegateSkills,    DelegateSkills_IDList,
            DelegatePassives,  DelegatePassives_IDList,
            DelegateKeywords,  DelegateEGOGifts_IDList,
            DelegateEGOGifts,  DelegateKeywords_IDList,
            DelegateUniversal, DelegateUniversal_IDList,
        ];

        public static void ClearDelegates()
        {
            foreach (dynamic Delegate in Delegates) Delegate.Clear();
        }

        public static void InitializeSkillsDelegateFrom(Type_Skills.Skills? Source)
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
                        if (CurrentSkill.ID != null)
                        {
                            DelegateSkills[(int)CurrentSkill.ID] = new Dictionary<int, Type_Skills.UptieLevel>();
                            foreach(Type_Skills.UptieLevel CurrentUptieLevel in CurrentSkill.UptieLevels)
                            {
                                DelegateSkills[(int)CurrentSkill.ID][CurrentUptieLevel.Uptie] = CurrentUptieLevel;

                                Mode_Skills.Skills_NameIDs[DelegateSkills[(int)CurrentSkill.ID][CurrentUptieLevel.Uptie].Name.Trim()] = (int)CurrentSkill.ID;
                            }
                        }
                    }

                    DelegateSkills_IDList = DelegateSkills.Keys.ToList();
                }
            }
        }

        public static void InitializePassivesDelegateFrom(Type_Passives.Passives Source)
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
                        if (CurrentPassive.ID != null)
                        {
                            DelegatePassives[(int)CurrentPassive.ID] = CurrentPassive;
                            Mode_Passives.Passives_NameIDs[CurrentPassive.Name.Trim()] = (int)CurrentPassive.ID;
                        }
                    }

                    DelegatePassives_IDList = DelegatePassives.Keys.ToList();
                }
            }
        }

        public static void InitializeKeywordsDelegateFrom(Type_Keywords.Keywords Source)
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

        public static void InitializeEGOGiftsDelegateFrom(Type_EGOGifts.EGOGifts Source)
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

        public static void InitializeContentBasedUniversalDelegateFrom(Type_ContentBasedUniversal_UNUSEDPROBABLYUSELESS.ContentBasedUniversal? Source)
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