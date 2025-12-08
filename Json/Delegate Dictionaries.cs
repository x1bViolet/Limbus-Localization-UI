using LC_Localization_Task_Absolute.Mode_Handlers;
using System.Collections.Generic;
using static LC_Localization_Task_Absolute.Json.LimbusJsonTypes;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute.Json
{
    /// <summary>
    /// Dictionaries with object links to deserialized json records for simplified id access
    /// </summary>
    public static class DelegateDictionaries
    {
        /// <summary>
        /// Override method for default <see cref="Enumerable.ToDictionary"/>, does not throw the exception "An item with the same key has already been added" (For _NameIDs accessors)
        /// </summary>
        private static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) where TKey : notnull
        {
            Dictionary<TKey, TValue> Result = new();
            foreach (var KeyValuePair in source) if (!Result.ContainsKey(KeyValuePair.Key)) Result[KeyValuePair.Key] = KeyValuePair.Value;
            return Result;
        }

        #region Skills
        public static Dictionary<int, Dictionary<int, Type_Skills.UptieLevel>> DelegateSkills = [];
            public static List<int> DelegateSkills_IDList => [.. DelegateSkills.Keys];
            public static Dictionary<string, int> DelegateSkills_NameIDs
                => DelegateSkills.Select(x => new KeyValuePair<string, int>(x.Value.First().Value.Name, x.Key)).ToDictionary();
        #endregion


        #region Passives
        public static Dictionary<int, Type_Passives.Passive> DelegatePassives = [];
            public static List<int> DelegatePassives_IDList => [.. DelegatePassives.Keys];
            public static Dictionary<string, int> DelegatePassives_NameIDs
                => DelegatePassives.Select(x => new KeyValuePair<string, int>(x.Value.Name, x.Key)).ToDictionary();
        #endregion


        #region E.G.O Gifts
        public static Dictionary<int, Type_EGOGifts.EGOGift> DelegateEGOGifts = [];
            public static List<int> DelegateEGOGifts_IDList => [.. DelegateEGOGifts.Keys];
            public static Dictionary<string, int> DelegateEGOGifts_NameIDs
                => DelegateEGOGifts.Select(x => new KeyValuePair<string, int>(x.Value.Name, x.Key)).ToDictionary();
        #endregion
        

        #region Keywords
        public static Dictionary<string, Type_Keywords.Keyword> DelegateKeywords = [];
            public static List<string> DelegateKeywords_IDList => [.. DelegateKeywords.Keys];
            public static Dictionary<string, string> DelegateKeywords_NameIDs
                => DelegateKeywords.Select(x => new KeyValuePair<string, string>(x.Value.Name, x.Key)).ToDictionary();
        #endregion


        // LoadStructure checks dataList for correctness, InitializeDelegateFrom checks each object for correctness

        public static void InitializeSkillsDelegateFromDeserialized()
        {
            DelegateSkills.Clear();

            foreach (Type_Skills.Skill CurrentSkill in Mode_Skills.DeserializedInfo.dataList)
            {
                if (CurrentSkill.ID != null && CurrentSkill.UptieLevels.Count > 0 && CurrentSkill.UptieLevels.Any(UptieLevel => UptieLevel.Uptie != null))
                {
                    DelegateSkills[(int)CurrentSkill.ID] = new Dictionary<int, Type_Skills.UptieLevel>();
                    foreach (Type_Skills.UptieLevel CurrentUptieLevel in CurrentSkill.UptieLevels)
                    {
                        DelegateSkills[(int)CurrentSkill.ID][(int)CurrentUptieLevel.Uptie] = CurrentUptieLevel;
                    }
                }
            }
        }

        public static void InitializePassivesDelegateFromDeserialized()
        {
            DelegatePassives.Clear();

            foreach (Type_Passives.Passive CurrentPassive in Mode_Passives.DeserializedInfo.dataList)
            {
                if (CurrentPassive.ID != null) DelegatePassives[(int)CurrentPassive.ID] = CurrentPassive;
            }
        }

        public static void InitializeKeywordsDelegateFromDeserialized()
        {
            DelegateKeywords.Clear();

            foreach (Type_Keywords.Keyword CurrentKeyword in Mode_Keywords.DeserializedInfo.dataList)
            {
                if (!CurrentKeyword.ID.Trim().EqualsOneOf("NOTHING THERE \0 \0", "")) DelegateKeywords[CurrentKeyword.ID] = CurrentKeyword;
            }
        }

        public static void InitializeEGOGiftsDelegateFromDeserialized()
        {
            DelegateEGOGifts.Clear();

            foreach (Type_EGOGifts.EGOGift CurrentEGOGift in Mode_EGOGifts.DeserializedInfo.dataList)
            {
                if (CurrentEGOGift.ID != null) DelegateEGOGifts[(int)CurrentEGOGift.ID] = CurrentEGOGift;
            }
        }
    }
}