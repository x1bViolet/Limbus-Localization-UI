using Newtonsoft.Json;
using System.Numerics;
using System.Runtime.Serialization;

namespace LC_Localization_Task_Absolute.Json
{
    internal abstract class BaseTypes
    {
        internal abstract class Type_Skills
        {
            public class Skills
            {
                public List<Skill> dataList { get; set; }

                [JsonProperty("Template Marker")]
                public string ScansTemplateMarker { get; set; }
            }

            public class Skill
            {
                [JsonProperty("id")]
                public int ID { get; set; }

                [JsonProperty("levelList")]
                public List<UptieLevel> UptieLevels { get; set; }
            }
            public class UptieLevel
            {
                [JsonProperty("level")]
                public int Uptie { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                public List<string> keywords { get; set; }

                [JsonProperty("abName")]
                public string EGOAbnormalityName { get; set; }

                [JsonProperty("desc")]
                public string Description { get; set; } = "";

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; }

                public string _comment { get; set; }

                [OnDeserialized]
                private void OnInit(StreamingContext context)
                {
                    EditorDescription = Description;
                }

                [JsonProperty("coinlist")]
                public List<Coin> Coins { get; set; }
            }
            public class Coin
            {
                [JsonProperty("coindescs")]
                public List<CoinDesc> CoinDescriptions { get; set; }
            }
            public class CoinDesc
            {
                [JsonProperty("desc")]
                public string Description { get; set; } = "";

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; }

                public string summary { get; set; }

                [OnDeserialized]
                private void OnInit(StreamingContext context)
                {
                    EditorDescription = Description;
                }
            }
        }

        internal abstract class Type_Passives
        {
            public class Passives
            {
                public List<Passive> dataList { get; set; }
            }

            public class Passive
            {
                [JsonProperty("id")]
                public int ID { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("desc")]
                public string Description { get; set; } = "";

                [JsonProperty("summary")]
                public string SummaryDescription { get; set; }

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; }
                [JsonIgnore] // For editor
                public string EditorSummaryDescription { get; set; }

                public string _comment { get; set; }

                [OnDeserialized]
                private void OnInit(StreamingContext context)
                {
                    EditorDescription = Description;
                    EditorSummaryDescription = SummaryDescription;
                }
            }
        }

        internal abstract class Type_EGOGifts
        {
            public class EGOGifts
            {
                public List<EGOGift> dataList { get; set; }
            }

            public class EGOGift
            {
                [JsonProperty("id")]
                public int ID { get; set; }

                [JsonIgnore] // For Preview
                public List<int> UpgradeLevelsAssociativeIDs { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("desc")]
                public string Description { get; set; } = "";

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; }

                public string _comment { get; set; }

                [JsonIgnore] // For Preview
                public string UpgradeLevel { get; set; } = "1";

                [OnDeserialized]
                private void OnInit(StreamingContext context)
                {
                    EditorDescription = Description;

                    UpgradeLevelsAssociativeIDs = new List<int>();
                }

                [JsonProperty("simpleDesc")]
                public List<SimpleDescription> SimpleDescriptions { get; set; }
            }
            public class SimpleDescription
            {
                [JsonProperty("abilityID")]
                public int ID { get; set; }

                [JsonProperty("simpleDesc")]
                public string Description { get; set; }

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; }

                public string _comment { get; set; }

                [OnDeserialized]
                private void OnInit(StreamingContext context) => EditorDescription = Description;
            }
        }

        internal abstract class Type_Keywords
        {
            public class Keywords
            {
                public List<Keyword> dataList { get; set; }
            }

            public class Keyword
            {
                [JsonProperty("id")]
                public string ID { get; set; } = "NOTHING THERE \0 \0";

                [JsonProperty("name")]
                public string Name { get; set; } = "NOTHING THERE \0 \0";

                [JsonProperty("desc")]
                public string Description { get; set; } = "";

                [JsonProperty("summary")]
                public string SummaryDescription { get; set; }

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; }
                [JsonIgnore] // For editor
                public string EditorSummaryDescription { get; set; }

                public string _comment { get; set; }

                [OnDeserialized]
                private void OnInit(StreamingContext context)
                {
                    EditorDescription = Description;
                    EditorSummaryDescription = SummaryDescription;
                }

                public string undefined { get; set; }


                // Special settings that can be used instead of "⇲ Assets Directory\[+] Keywords\Keyword Colors.T[-]"
                [JsonProperty("Color")]
                public string Color { get; set; }
            }
        }
    
        internal abstract class Type_SkillTag
        {
            public class SkillTags
            {
                public List<SkillTag> dataList { get; set; }
            }

            public class SkillTag
            {
                [JsonProperty("id")]
                public string ID { get; set; }
                
                [JsonProperty("name")]
                public string Tag { get; set; }


                // Special settings that can be used instead of "⇲ Assets Directory\[+] Keywords\SkillTag Colors.T[-]"
                [JsonProperty("Color")]
                public string Color { get; set; }
            }
        }

        internal abstract class Type_RawSkills
        {
            public class SkillsDetailedInfo
            {
                [JsonProperty("list")]
                public List<DetailedInfoItem> List { get; set; }
            }

            public class DetailedInfoItem
            {
                [JsonProperty("id")]
                public BigInteger? ID { get; set; }

                [JsonProperty("textID")]
                public BigInteger? LocalizationItemID { get; set; }

                [JsonProperty("skillTier")]
                public int Rank { get; set; } = 1;

                [JsonProperty("skillData")]
                public List<DetailedInfoItem_UptieLevel> UptieLevelsJsonList { get; set; }



                #region Technical
                [JsonIgnore]
                public int BaseLevel { get; set; } = 55;

                [JsonIgnore]
                public int? LevelCorrection { get; set; }

                [JsonIgnore] // For internal
                public Dictionary<int, DetailedInfoItem_UptieLevel> UptieLevelsDictionary { get; set; } = new Dictionary<int, DetailedInfoItem_UptieLevel>();

                [JsonIgnore]
                public string SkillAction { get; set; }

                [JsonIgnore]
                public string DamageType { get; set; }

                [JsonIgnore] // Means ["Regular", "Regular", "Unbreakable"] list to take it from data files (there to take from first uptie)
                public List<string> CoinTypesSequence { get; set; }

                [JsonIgnore]
                public string CoinMathOperator { get; set; }

                [JsonIgnore]
                public int? AttackWeight { get; set; }

                [JsonIgnore]
                public bool? Unobservable { get; set; }
                #endregion



                [OnDeserialized]
                internal void TechnicalProcessing(StreamingContext context)
                {
                    // Associate for json files
                    if (LocalizationItemID != null) ID = LocalizationItemID;

                    if (UptieLevelsJsonList != null)
                    {
                        int? LatestRegisteredCoinPowerValue = null;
                        int? LatestRegisteredBasePowerValue = null;
                        string? LatestRegisteredAffinity = null;
                        string? LatestRegisteredCoinMathOperator = null;

                        foreach(DetailedInfoItem_UptieLevel JsonUptieGaksungLevel in UptieLevelsJsonList)
                        {
                            if (SkillAction       == null) SkillAction       = JsonUptieGaksungLevel.SkillType_UPTIE;
                            if (DamageType        == null) DamageType        = JsonUptieGaksungLevel.DamageType_UPTIE;
                            if (AttackWeight      == null) AttackWeight      = JsonUptieGaksungLevel.AttackWeight_UPTIE;
                            if (Unobservable      == null) Unobservable      = JsonUptieGaksungLevel.Unobservable_UPTIE;
                            if (LevelCorrection   == null) LevelCorrection   = JsonUptieGaksungLevel.LevelCorrection_UPTIE;
                            if (CoinMathOperator  == null) CoinMathOperator  = JsonUptieGaksungLevel.CoinMathOperator_UPTIE;
                            if (CoinTypesSequence == null) CoinTypesSequence = JsonUptieGaksungLevel.CoinTypesSequence_UPTIE;

                            if (JsonUptieGaksungLevel.Affinity_UPTIE == null) JsonUptieGaksungLevel.Affinity_UPTIE = LatestRegisteredAffinity;
                            LatestRegisteredAffinity = JsonUptieGaksungLevel.Affinity_UPTIE;

                            // If switching to previous upties, don't keep higher values because null
                            if (JsonUptieGaksungLevel.CoinPower_UPTIE == null)
                            {
                                JsonUptieGaksungLevel.CoinPower_UPTIE = LatestRegisteredCoinPowerValue;
                            }
                            else LatestRegisteredCoinPowerValue = JsonUptieGaksungLevel.CoinPower_UPTIE;

                            if (JsonUptieGaksungLevel.BasePower == null)
                            {
                                JsonUptieGaksungLevel.BasePower = LatestRegisteredBasePowerValue;
                            }
                            else LatestRegisteredBasePowerValue = JsonUptieGaksungLevel.BasePower;

                            if (JsonUptieGaksungLevel.CoinMathOperator_UPTIE == null)
                            {
                                JsonUptieGaksungLevel.CoinMathOperator_UPTIE = LatestRegisteredCoinMathOperator;
                            }
                            else LatestRegisteredCoinMathOperator = JsonUptieGaksungLevel.CoinMathOperator_UPTIE;

                            UptieLevelsDictionary[(int)JsonUptieGaksungLevel.UptieLevel] = JsonUptieGaksungLevel;
                        }
                    }
                }
            }

            public class DetailedInfoItem_UptieLevel
            {
                [JsonProperty("iconID")] // int or string  |  1030605 \ "1030804_4"
                public dynamic? IconID { get; set; }

                [JsonProperty("gaksungLevel")]
                public int UptieLevel { get; set; } = 1;

                [JsonProperty("attributeType")]
                public string Affinity_UPTIE { get; set; }

                [JsonProperty("atkType")]
                public string DamageType_UPTIE { get; set; }

                [JsonProperty("defType")]
                public string SkillType_UPTIE { get; set; }

                [JsonProperty("targetNum")]
                public int? AttackWeight_UPTIE { get; set; }

                public string? skillMotion { get; set; } // Copies count affected?

                [JsonProperty("skillLevelCorrection")]
                public int? LevelCorrection_UPTIE { get; set; }

                [JsonProperty("defaultValue")]
                public int? BasePower { get; set; }

                [JsonProperty("coinList")]
                private List<CoinInfo> CoinList { get; set; }

                [JsonProperty("abilityScriptList")] // Check 'Unovservable'
                private List<abilityScriptListItem>? abilityScriptList { get; set; }



                #region Technical
                [JsonIgnore]
                public bool Unobservable_UPTIE { get; set; } = false;

                [JsonIgnore] // Set from first coin
                public int? CoinPower_UPTIE { get; set; }

                [JsonIgnore]
                public string CoinMathOperator_UPTIE { get; set; }

                [JsonIgnore] // ["Regular", "Regular", "Unbreakable"] list to take it from data files
                public List<string> CoinTypesSequence_UPTIE { get; set; } = new List<string>();
                #endregion



                [OnDeserialized]
                internal void OnDeserialized(StreamingContext context)
                {
                    if (!$"{IconID}".Equals(""))
                    {
                        if (IconID is not string)
                        {
                            IconID = $"{IconID}";
                        }
                    }

                    if (abilityScriptList != null)
                    {
                        if (abilityScriptList.Where(x => x.scriptName.Equals("UnobservableSkill")).Any())
                        {
                            Unobservable_UPTIE = true;
                        }
                    }

                    if (Affinity_UPTIE != null)
                    {
                        Affinity_UPTIE = Affinity_UPTIE switch
                        {
                            "CRIMSON"  => "Wrath",
                            "SCARLET"  => "Lust",
                            "AMBER"    => "Sloth",
                            "SHAMROCK" => "Gluttony",
                            "AZURE"    => "Gloom",
                            "INDIGO"   => "Pride",
                            "VIOLET"   => "Envy",
                            _ => "None"
                        };
                    }
                    if (DamageType_UPTIE != null)
                    {
                        DamageType_UPTIE = DamageType_UPTIE switch
                        {
                            "HIT"       => "Blunt",
                            "SLASH"     => "Slash",
                            "PENETRATE" => "Pierce",
                            _ => "-"
                        };
                    }
                    if (SkillType_UPTIE != null)
                    {
                        SkillType_UPTIE = SkillType_UPTIE switch
                        {
                            "GUARD"   => "Guard",
                            "ATTACK"  => "Attack",
                            "EVADE"   => "Evade",
                            "COUNTER" => "Counter",
                            _ => "-"
                        };
                    }
                    if (CoinList != null)
                    {
                        if (CoinList.Count > 0)
                        {
                            foreach (CoinInfo Coin in CoinList)
                            {
                                if (CoinPower_UPTIE == null) // if unset, take values from first coin
                                {
                                    if (Coin.CoinPower != null) CoinPower_UPTIE = (int)Coin.CoinPower;
                                    if (Coin.CoinMathOperator != null) CoinMathOperator_UPTIE = Coin.CoinMathOperator.Equals("ADD") ? "+" : "-";
                                }

                                CoinTypesSequence_UPTIE.Add(Coin.CoinType);
                            }
                        }
                    }
                }
            }

            public class CoinInfo
            {
                [JsonProperty("operatorType")]
                public string CoinMathOperator { get; set; } = "ADD"; //  Plus | Minus

                [JsonProperty("scale")]
                public int? CoinPower { get; set; }

                [JsonProperty("color")]
                public string CoinType { get; set; } = "Regular";

                [OnDeserialized]
                internal void OnDeserialized(StreamingContext context)
                {
                    if (CoinType.Equals("GREY")) // Idk why GREY and why color
                    {
                        CoinType = "Unbreakable";
                    }
                }
            }
        
            public class abilityScriptListItem
            {
                public string scriptName { get; set; } // For recognizing "UnobservableSkill" to set ? instead of valeus
            }
        }


        /// <summary>
        /// For most other files with a simple structure in the form of "id" and "content" only objects
        /// </summary>
        internal abstract class Type_ContentBasedUniversal
        {
            public class ContentBasedUniversal
            {
                public List<dynamic> dataList { get; set; } // dynamic = "any"
            }
        }
    }
}
