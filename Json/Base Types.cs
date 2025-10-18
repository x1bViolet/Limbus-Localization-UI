using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.IO;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;

#pragma warning disable IDE0079
#pragma warning disable CS0169
#pragma warning disable CA2211
#pragma warning disable CS1696

namespace LC_Localization_Task_Absolute.Json
{
    public abstract class BaseTypes
    {
        public abstract class Type_Skills
        {
            public record SkillsFile
            {
                [JsonProperty("Manual File Type")]
                public string ManualFileType { get; set; }

                [JsonProperty("dataList")]
                public List<Skill> dataList { get; set; }

                [JsonProperty("Template Marker")]
                public string ScansTemplateMarker { get; set; }
            }

            public record Skill
            {
                [JsonProperty("id")]
                public int? ID { get; set; }

                [JsonProperty("levelList")]
                public List<UptieLevel> UptieLevels { get; set; }

                public Skill(bool AutoAddUptie = false, bool AddAbNameToThisUptie = false)
                {
                    if (AutoAddUptie)
                    {
                        UptieLevels = new List<UptieLevel>()
                        {
                            new UptieLevel()
                            {
                                Uptie = 1,
                                EGOAbnormalityName = AddAbNameToThisUptie ? "" : null,
                                Coins = new List<Coin>()
                                {
                                    #pragma warning restore Absolutely brand new
                                    new() { CoinDescriptions = new() { new() } },
                                    new() { CoinDescriptions = new() { new() } },
                                    new() { CoinDescriptions = new() { new() } },
                                    new() { CoinDescriptions = new() { new() } },
                                    new() { CoinDescriptions = new() { new() } },
                                    new() { CoinDescriptions = new() { new() } },
                                }
                            }
                        };
                    }
                }
            }
            public record UptieLevel
            {
                [JsonProperty("Affinity")]
                public string OptionalAffinity { get; set; }


                [JsonProperty("level")]
                public int Uptie { get; set; }


                [JsonProperty("name")]
                public string Name { get; set; } = "";

                [JsonProperty("abName")]
                public string EGOAbnormalityName { get; set; }


                [JsonProperty("desc")]
                public string Description { get; set; } = "";

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; } = "";


                [JsonProperty("coinlist")]
                public List<Coin> Coins { get; set; }


                #region Not used
                public List<string> keywords { get; set; }
                public string _comment { get; set; }
                #endregion

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    EditorDescription = Description;
                }
            }
            public record Coin
            {
                [JsonProperty("coindescs")]
                public List<CoinDesc> CoinDescriptions { get; set; }
            }
            public record CoinDesc
            {
                [JsonProperty("desc")]
                public string Description { get; set; } = "";

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; } = "";


                #region Not used
                public string summary { get; set; } // Rare
                #endregion

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    EditorDescription = Description;
                }
            }
        }

        public abstract class Type_Passives
        {
            public record PassivesFile
            {
                [JsonProperty("Manual File Type")]
                public string ManualFileType { get; set; }

                [JsonProperty("dataList")]
                public List<Passive> dataList { get; set; }
            }

            public record Passive
            {
                [JsonProperty("id")]
                public int? ID { get; set; }


                [JsonProperty("name")]
                public string Name { get; set; } = "";


                [JsonProperty("desc")]
                public string Description { get; set; } = "";

                [JsonProperty("summary")]
                public string SummaryDescription { get; set; }


                [JsonIgnore] // For editor
                public string EditorDescription { get; set; } = "";

                [JsonIgnore] // For editor
                public string EditorSummaryDescription { get; set; } = "";


                #region Not used
                public string _comment { get; set; }
                #endregion

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    EditorDescription = Description;
                    EditorSummaryDescription = SummaryDescription;
                }
            }
        }

        public abstract class Type_EGOGifts
        {
            public record EGOGiftsFile
            {
                [JsonProperty("Manual File Type")]
                public string ManualFileType { get; set; }

                [JsonProperty("dataList")]
                public List<EGOGift> dataList { get; set; }
            }

            public record EGOGift
            {
                [JsonProperty("id")]
                public int ID { get; set; }


                [JsonProperty("name")]
                public string Name { get; set; }


                [JsonProperty("desc")]
                public string Description { get; set; } = "";

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; } = "";


                [JsonProperty("simpleDesc")]
                public List<SimpleDescription> SimpleDescriptions { get; set; }


                [JsonIgnore] // For Preview
                public string UpgradeLevel { get; set; } = "1";

                [JsonIgnore] // For Preview
                public List<int> UpgradeLevelsAssociativeIDs { get; set; }


                #region Not used
                public string _comment { get; set; }
                #endregion

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    EditorDescription = Description;

                    UpgradeLevelsAssociativeIDs = new List<int>();
                }
            }
            public record SimpleDescription
            {
                [JsonProperty("abilityID")]
                public int ID { get; set; }


                [JsonProperty("simpleDesc")]
                public string Description { get; set; } = "";

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; } = "";

                #region Not used
                public string _comment { get; set; }
                #endregion

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    EditorDescription = Description;
                }
            }
        }

        public abstract class Type_Keywords
        {
            public record KeywordsFile
            {
                [JsonProperty("Manual File Type")]
                public string ManualFileType { get; set; }

                [JsonProperty("dataList")]
                public List<Keyword> dataList { get; set; }
            }

            public record Keyword
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
                public string EditorDescription { get; set; } = "";
                [JsonIgnore] // For editor
                public string EditorSummaryDescription { get; set; } = "";


                [JsonProperty("Color")] // Special settings that can be used instead of "[⇲] Assets Directory\[+] Keywords\Keyword Colors.T[-]"
                public string? Color { get; set; }


                #region Not used
                public string iconId { get; set; }
                public string IconID { get; set; }
                public string _comment { get; set; }
                public string undefined { get; set; }
                #endregion

                [OnDeserialized]
                private void OnDeserialized(StreamingContext Context)
                {
                    EditorDescription = Description;
                    EditorSummaryDescription = SummaryDescription;
                }
            }
        }
    
        public abstract class Type_SkillTag
        {
            public record SkillTagsFile
            {
                [JsonProperty("dataList")]
                public List<SkillTag> dataList { get; set; }
            }

            public record SkillTag
            {
                [JsonProperty("id")]
                public string ID { get; set; }
                

                [JsonProperty("name")]
                public string Tag { get; set; }

                // Special settings that can be used instead of "[⇲] Assets Directory\[+] Keywords\SkillTag Colors.T[-]"
                [JsonProperty("Color")]
                public string Color { get; set; }
            }
        }

        public record ManualFileTypeCheckHandler
        {
            [JsonProperty("Manual File Type")] public string ManualFileType { get; set; }
        }
        public static string? TryAcquireManualFileType(string Filename)
        {
            string FileText = File.ReadAllText(Filename);
            ManualFileTypeCheckHandler FileHeader = JsonConvert.DeserializeObject<ManualFileTypeCheckHandler>(FileText);
            if (FileHeader != null && FileHeader.ManualFileType != null)
            {
                return FileHeader.ManualFileType switch
                {
                    "Skills" => "Skills",
                    "Skills (With upties)" => "Skills_personality-",
                    "Skills (With upties; With abName)" => "Skills_Ego_Personality-",
                    "Passives" => "Passive",
                    "Keywords" => "BattleKeywords",
                    "Keywords (Bufs)" => "Bufs",
                    "Keywords (BattleKeywords)" => "BattleKeywords",
                    "E.G.O Gifts" => "EGOgift",
                    _ => null
                };
            }
            else if (FileText.Contains(@"""Template Marker"": ""(Don't remove)"""))
            {
                return "Skills";
            }
            else
            {
                return null;
            }
        }

        public abstract class Type_RawSkillsDisplayInfo
        {
            public record SkillsDetailedInfo
            {
                [JsonProperty("list")]
                public List<DetailedInfoItem> List { get; set; }
            }

            public record DetailedInfoItem
            {
                [JsonProperty("id")]
                public int? ID { get; set; }

                [JsonProperty("textID")]
                public int? LocalizationItemID { get; set; }

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
                private void TechnicalProcessing(StreamingContext Context)
                {
                    // Associate for json files
                    if (LocalizationItemID != null) ID = LocalizationItemID;

                    if (UptieLevelsJsonList != null)
                    {
                        int? LatestRegisteredCoinPowerValue = null;
                        int? LatestRegisteredBasePowerValue = null;
                        string? LatestRegisteredAffinity = null;
                        string? LatestRegisteredCoinMathOperator = null;

                        foreach (DetailedInfoItem_UptieLevel JsonUptieGaksungLevel in UptieLevelsJsonList)
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

            public record DetailedInfoItem_UptieLevel
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
                public void OnDeserialized(StreamingContext context)
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

            public record CoinInfo
            {
                [JsonProperty("operatorType")]
                public string CoinMathOperator { get; set; } = "ADD"; //  Plus | Minus

                [JsonProperty("scale")]
                public int? CoinPower { get; set; }

                [JsonProperty("color")]
                public string CoinType { get; set; } = "Regular";

                [OnDeserialized]
                public void OnDeserialized(StreamingContext context)
                {
                    if (CoinType.Equals("GREY")) // Idk why GREY and why color
                    {
                        CoinType = "Unbreakable";
                    }
                }
            }
        
            public record abilityScriptListItem
            {
                public string scriptName { get; set; } // For recognizing "UnobservableSkill" to set ? instead of valeus
            }
        }


        /// <summary>
        /// For most other files with a simple structure in the form of "id" and "content" only objectsprobablyunusedidk
        /// </summary>
        public abstract class Type_ContentBasedUniversal_UNUSEDPROBABLYUSELESS
        {
            public record ContentBasedUniversal
            {
                [JsonProperty("dataList")]
                public List<dynamic> dataList { get; set; } // dynamic = "any"
            }
        }
    }
}
