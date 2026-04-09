using LCLocalizationInterface.Instruments.Classes;

namespace LCLocalizationInterface.LimbusRegistry.JsonTypes
{
    public ref struct @SkillsData
    {
        public static ReadOnlyDictionary<BigInteger, SkillsDataFileJson.SkillDataItem> ReadedSkillsData { get; private set; }
            = ReadOnlyDictionary<BigInteger, SkillsDataFileJson.SkillDataItem>.Empty;

        public static void ReadSkillsDataFiles()
        {
            Dictionary<BigInteger, SkillsDataFileJson.SkillDataItem> ReadedThere = [];
            if (Directory.Exists(@"[⇲] Assets Directory\Limbus Images\Skills\Skills Data"))
            {
                foreach (FileInfo SkillsDataFileInfo in new DirectoryInfo(@"[⇲] Assets Directory\Limbus Images\Skills\Skills Data").GetFiles("*.json", SearchOption.AllDirectories))
                {
                    if (SkillsDataFileInfo.TryDeserealizeJsonAs(out SkillsDataFileJson SkillsDataFile, out Exception Occurred))
                    {
                        foreach (SkillsDataFileJson.SkillDataItem SkillDataItem in SkillsDataFile.List.Where(SkillData => SkillData.ID is not null))
                        {
                            ReadedThere[(BigInteger)SkillDataItem.ID!] = SkillDataItem;
                        }
                    }
                    else
                    {
                        ErrorMessageWindow.ShowException(Occurred, @$"This exception occurred while trying to read skills data file ""<b>{SkillsDataFileInfo.Name}</b>"" from ""<b>[⇲] Assets Directory\Limbus Images\Skills\Skills Data</b>""");
                    }
                }
            }
            @SkillsData.ReadedSkillsData = new(ReadedThere);
        }


        private static FileEventsNotifier SkillsDataWatcher { get; } = new(@"[⇲] Assets Directory\Limbus Images\Skills\Skills Data", "*.json")
        {
            GeneralHandler = (_, _, _) =>
            {
                ReadSkillsDataFiles();
                if (@EditorModesShelf.Skills.CurrentFile is not null)
                {
                    @EditorModesShelf.Skills.ChangeSkillNameReplicaAppearance();
                }
            }
        };

        /// <summary>
        /// Limbus data files class about Skills with the selection of properties only needed to display their ornamented name replication through <see cref="@EditorModesShelf.Types.SkillsEditorMode.ChangeSkillNameReplicaAppearance"/>
        /// </summary>
        public record SkillsDataFileJson
        {
            [JsonProperty("list")]
            public List<SkillDataItem> List { get; set; } = [];

            public record SkillDataItem : IHasIdentifier<BigInteger?>
            {
                [JsonProperty("id")]
                public BigInteger? ID { get; set; }

                [JsonProperty("textID")]
                public BigInteger? LocalizationID { get; set; }

                [JsonProperty("skillTier")]
                public int Rank { get; set; } = 1;



                #region Technical things
                [JsonIgnore]
                public Dictionary<int, SkillDataItem_UptieLevel> UptieLevelsDictionary { get; private set; } = [];


                /// <summary>
                /// Extend the properties of each Uptie to next ones with subsequent additive layering so that each of them contains complete information<br/>
                /// (In the source data file, only first Uptie contains basic information, and all next Upties contains only changes)
                /// </summary>
                [OnDeserialized]
                public void OnDeserialized(StreamingContext Context)
                {
                    if ((Context.Context as string) == "Take no technical actions") return;


                    if (LocalizationID is not null) ID = LocalizationID;


                    if (UptieLevelsJsonList is not null)
                    {
                        string? Latest__Affinity = null;
                        string? Latest__DamageType = null;
                        string? Latest__SkillAction = null;
                        int? Latest__AttackWeight = null;

                        int? Latest__CoinPower = null;
                        int? Latest__BasePower = null;
                        string? Latest__CoinsSequence = null;

                        string? Latest__CoinMathOperator = null;


                        // "FirstEncountered" because Level Correction may be initially defined in the first Uptie in list, but then set as 0 in some following one
                        // Despite this fact it doesn't change in the game from initially defined
                        int? FirstEncountered__LevelCorrection = null;


                        ////////////////////////////////////////////////////////////////////
                        foreach (SkillDataItem_UptieLevel UptieLevel in UptieLevelsJsonList)
                        {
                            this/*Skill*/.UptieLevelsDictionary[UptieLevel.UptieNumber] = UptieLevel;


                            UptieLevel.Rank = this/*Skill*/.Rank;

                            
                            FirstEncountered__LevelCorrection ??= UptieLevel.LevelCorrection;
                            UptieLevel.LevelCorrection = (int)FirstEncountered__LevelCorrection;


                            
                            //                 If this not 'null'
                            // Update latest   in current Uptie       Else keep current value
                            Latest__Affinity = UptieLevel.Affinity ?? Latest__Affinity;

                            // Then if this
                            // 'null' in Uptie      Set as latest encountered
                            UptieLevel.Affinity ??= Latest__Affinity;



                            static T? Prolong<T>(ref T? LatestRegisteredValue, T? UptieProperty)
                            {
                                LatestRegisteredValue = UptieProperty ?? LatestRegisteredValue;
                                return UptieProperty is null ? LatestRegisteredValue : UptieProperty;
                            }

                            UptieLevel.DamageType       = Prolong(ref Latest__DamageType,       UptieLevel.DamageType);
                            UptieLevel.SkillType        = Prolong(ref Latest__SkillAction,      UptieLevel.SkillType);
                            UptieLevel.AttackWeight     = Prolong(ref Latest__AttackWeight,     UptieLevel.AttackWeight);
                            UptieLevel.CoinsSequence    = Prolong(ref Latest__CoinsSequence,    UptieLevel.CoinsSequence);
                            UptieLevel.CoinPower        = Prolong(ref Latest__CoinPower,        UptieLevel.CoinPower);
                            UptieLevel.BasePower        = Prolong(ref Latest__BasePower,        UptieLevel.BasePower);
                            UptieLevel.CoinMathOperator = Prolong(ref Latest__CoinMathOperator, UptieLevel.CoinMathOperator);



                            // Fallbacks for further enums if still null for some reason
                            UptieLevel.DamageType ??= "None";
                            UptieLevel.SkillType ??= "Attack";
                            UptieLevel.Affinity ??= "None";
                        }





                        // Fill all missing Uptie keys to create a monolithic linear list of each Uptie descriptions without gaps or possible mismatches between the numbers of Uptie levels from the localization files and those located in these files


                        // (Up to 6)

                        // { [1] = ..,  [4] = .. }      ->      { [1] = ..,  [2] = .. (Uptie [1]),  [3] = .. (Uptie [1]),  [4] = .. }

                        // { [3] = ..,  [4] = .. }      ->      { [1] = .. (Uptie [3]),  [2] = .. (Uptie [3]),  [3] = ..,  [4] = .. }

                        SkillDataItem_UptieLevel LatestEncounteredUptie = UptieLevelsDictionary.Values.First();

                        //                                   max 6 upties
                        for (int UptieNumber = 1; UptieNumber <= 6; UptieNumber++)
                        {
                            if (UptieLevelsDictionary.TryGetValue(UptieNumber, out SkillDataItem_UptieLevel? value)) LatestEncounteredUptie = value;
                            else UptieLevelsDictionary[UptieNumber] = LatestEncounteredUptie;
                        }
                        UptieLevelsDictionary = UptieLevelsDictionary.OrderBy(KeyValuePair => KeyValuePair.Key).ToDictionary();
                    }
                }
                #endregion



                [JsonProperty("skillData")]
                public List<SkillDataItem_UptieLevel>? UptieLevelsJsonList { get; set; }

                public record SkillDataItem_UptieLevel
                {
                    [JsonProperty("iconID")] // int or string (1030605 | "1030804_4")
                    public object? IconID { get; set; }

                    [JsonProperty("gaksungLevel")]
                    public int UptieNumber { get; set; } = 1;

                    [JsonProperty("attributeType")]
                    public string? Affinity { get; set; } // May vary from Uptie to Uptie

                    [JsonProperty("atkType")]
                    public string? DamageType { get; set; }

                    [JsonProperty("defType")]
                    public string? SkillType { get; set; }

                    [JsonProperty("targetNum")]
                    public int? AttackWeight { get; set; }

                    [JsonProperty("skillLevelCorrection")]
                    public int LevelCorrection { get; set; } = 0;

                    [JsonProperty("defaultValue")]
                    public int? BasePower { get; set; } // May vary from Uptie to Uptie



                    #region Technical things
                    [JsonIgnore]
                    public string? CoinMathOperator { get; set; }

                    [JsonIgnore]
                    public int? CoinPower { get; set; }

                    [JsonIgnore]
                    public int Rank { get; set; } = 1;

                    [JsonIgnore]
                    public string? CoinsSequence { get; set; }


                    [OnDeserialized]
                    public void OnDeserialized(StreamingContext Context)
                    {
                        if ((Context.Context as string) == "Take no technical actions") return;


                        CoinMathOperator = CoinsList?.Count > 0 ? CoinsList[0].CoinMathOperator : "+";
                        CoinPower = CoinsList?.Count > 0 ? CoinsList[0].CoinPower! : null;

                        if (CoinsList is not null)
                        {
                            CoinsSequence = string.Join(", ", CoinsList.Select(x => x.CoinType));
                            CoinsList = null;
                        }



                        Affinity = Affinity switch
                        {
                            null       => null,
                            "CRIMSON"  => "Wrath",
                            "SCARLET"  => "Lust",
                            "AMBER"    => "Sloth",
                            "SHAMROCK" => "Gluttony",
                            "AZURE"    => "Gloom",
                            "INDIGO"   => "Pride",
                            "VIOLET"   => "Envy",
                            "NEUTRAL"  => "None",
                            _          => "None",
                        };

                        DamageType = DamageType switch
                        {
                            "HIT"       => "Blunt",
                            "SLASH"     => "Slash",
                            "PENETRATE" => "Pierce",
                            _           => null,
                        };
                        
                        SkillType = SkillType switch
                        {
                            "GUARD"   => "Guard",
                            "ATTACK"  => "Attack",
                            "EVADE"   => "Evade",
                            "COUNTER" => "Counter",
                            _         => null,
                        };
                    }
                    #endregion



                    [JsonProperty("coinList")]
                    public List<SkillDataItem_CoinInfo>? CoinsList { get; set; }
                    
                    public record SkillDataItem_CoinInfo
                    {
                        [JsonProperty("operatorType")]
                        public string CoinMathOperator { get; set; } = "+"; //  ADD = "+" / SUB = "-"

                        [JsonProperty("scale")]
                        public int? CoinPower { get; set; } // May vary from Uptie to Uptie

                        [JsonProperty("color")]
                        public string CoinType { get; set; } = "Regular";



                        #region Technical things
                        [OnDeserialized]
                        public void OnDeserialized(StreamingContext Context)
                        {
                            if (Context.Context as string == "Take no technical actions") return;

                            CoinMathOperator = CoinMathOperator switch
                            {
                                "ADD" => "+",
                                "SUB" => "-",
                                _     => "+",
                            };

                            CoinType = CoinType switch
                            {
                                "GREY"    => "Unbreakable",
                                "GREEN"   => "Excision",
                                "PURPLE"  => "Purple",
                                _         => "Regular",
                            };
                        }
                        #endregion
                    }
                }
            }
        }
    }
}