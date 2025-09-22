using Newtonsoft.Json;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization;
using System.Windows;
using static LC_Localization_Task_Absolute.Requirements;

#pragma warning disable IDE0079
#pragma warning disable CA2211

namespace LC_Localization_Task_Absolute.Json
{
    public abstract class Custom_Skills_Constructor
    {
        public static Dictionary<BigInteger, SkillContstructor> LoadedSkillConstructors = new Dictionary<BigInteger, SkillContstructor>();

        public static void ReadSkillConstructors()
        {
            if (Directory.Exists(@"[⇲] Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Constructor"))
            {
                string LogFile = @"[⇲] Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Constructor\Recognizing Log.txt";
                if (File.Exists(LogFile))
                {
                    File.SetAttributes(LogFile, FileAttributes.Normal);
                    File.Delete(LogFile);
                    File.WriteAllText(LogFile, "");
                    File.SetAttributes(LogFile, File.GetAttributes(LogFile) | FileAttributes.Hidden);
                }

                bool FirstLogPadding = true;

                foreach (FileInfo ConstructorFile in new DirectoryInfo(@"[⇲] Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Constructor").GetFiles("*.json", SearchOption.AllDirectories))
                {
                    LogCustomSkillsConstructor($"{(FirstLogPadding ? "" : "\n\n\n\n\n")}Checking File :: {ConstructorFile.Name}");
                    SkillsConstructorFile Deserialized = ConstructorFile.Deserealize<SkillsConstructorFile>();
                    FirstLogPadding = false;

                    if (Deserialized.List != null)
                    {
                        foreach (SkillContstructor Constructor in Deserialized.List)
                        {
                            if (Constructor.ID != null)
                            {
                                LoadedSkillConstructors[(BigInteger)Constructor.ID] = Constructor;
                            }
                        }
                    }
                }

                File.SetAttributes(LogFile, File.GetAttributes(LogFile) | FileAttributes.ReadOnly);
            }
        }

        public record SkillsConstructorFile
        {
            [JsonProperty("Skills Info")]
            public List<SkillContstructor> List { get; set; }
        }
        public record SkillContstructor
        {
            public BigInteger? ID { get; set; }

            [JsonProperty("Icon ID")]
            public string? IconID { get; set; }

            [JsonProperty("(Name)")]
            public string? SkillName { get; set; }

            [JsonProperty("Specific")]
            public SkillContstructor_Specific Specific { get; set; } = new SkillContstructor_Specific();

            [JsonProperty("Characteristics")]
            public SkillContstructor_Characteristics Characteristics { get; set; } = new SkillContstructor_Characteristics();

            [JsonProperty("Attributes")]
            public SkillContstructor_Attributes Attributes { get; set; } = new SkillContstructor_Attributes();

            [JsonProperty("Upties")]
            public Dictionary<string, SkillContstructor_Uptie>? Skill_Upties { get; set; }

            [OnDeserialized]
            private void TechnicalProcessing(StreamingContext ThisFilePathContext)
            {
                if (IconID != null) IconID = IconID.Replace(":Constructor:", $"{ThisFilePathContext.Context}");

                if (ID != null & Configurazione.SettingsLoadingEvent)
                {
                    LogCustomSkillsConstructor($"★ Skill with ID '{ID}'   ({SkillName})");

                    LogCustomSkillsConstructor($"  ⋉ Damage Type:{Specific.DamageType} | Action:{Specific.Action} | Rank:{Specific.Rank}");
                    LogCustomSkillsConstructor($"  ⋉ Coins:[{string.Join("-", Characteristics.CoinsList)}] | Coins Type:{Characteristics.CoinsType}");
                    LogCustomSkillsConstructor($"  ⋉ Atk Weight:{Characteristics.AttackWeight} | Level Correction:{Characteristics.LevelCorrection}");

                    bool SetDefaultNoneAffinityOnFirstEncounteredUptieIfItNotSet = false;

                    int?    LatestRegisteredCoinPowerValue = null;
                    int?    LatestRegisteredBasePowerValue = null;
                    string? LatestRegisteredAffinity       = null;


                    List<string> AddedUptieLevels = new List<string>();
                    if (Skill_Upties != null)
                    {
                        foreach (KeyValuePair<string, SkillContstructor_Uptie> Uptie in Skill_Upties)
                        {
                            if (Uptie.Key.EqualsOneOf("1", "2", "3", "4"))
                            {
                                if (!SetDefaultNoneAffinityOnFirstEncounteredUptieIfItNotSet)
                                {
                                    if (Uptie.Value.Affinity == null)
                                    {
                                        Uptie.Value.Affinity = "None";
                                    }
                                    SetDefaultNoneAffinityOnFirstEncounteredUptieIfItNotSet = true;
                                }

                                if (Uptie.Value.BasePower == null) Uptie.Value.BasePower = LatestRegisteredBasePowerValue;
                                if (Uptie.Value.CoinPower == null) Uptie.Value.CoinPower = LatestRegisteredCoinPowerValue;
                                if (Uptie.Value.Affinity  == null) Uptie.Value.Affinity  = LatestRegisteredAffinity;

                                LatestRegisteredCoinPowerValue = Uptie.Value.CoinPower;
                                LatestRegisteredBasePowerValue = Uptie.Value.BasePower;
                                LatestRegisteredAffinity       = Uptie.Value.Affinity;

                                AddedUptieLevels.Add(Uptie.Key);
                                LogCustomSkillsConstructor($"    - Uptie:{Uptie.Key.nullHandle()} ^ '{Uptie.Value.Affinity.nullHandle()}' — Base Power:{Uptie.Value.BasePower.nullHandle()} | Coin Power:{Uptie.Value.CoinPower.nullHandle()} | Another Icon:{Uptie.Value.IconID.nullHandle()}");
                            }
                        }
                        if (AddedUptieLevels.Count > 0)
                        {
                            SkillContstructor_Uptie QueueCloser = null;
                            foreach (string AppendUptieNumber in new List<string> {"1", "2", "3", "4"})
                            {
                                if (Skill_Upties.ContainsKey(AppendUptieNumber))
                                {
                                    if (!AppendUptieNumber.Equals("4"))
                                    {
                                        QueueCloser = Skill_Upties[AppendUptieNumber];
                                        //LogCustomSkillsConstructor($"    * Upties QueueCloser set as — Uptie:{AppendUptieNumber}");
                                    }
                                }
                                else
                                {
                                    Skill_Upties[AppendUptieNumber] = QueueCloser;
                                    //LogCustomSkillsConstructor($"    * Upties QueueCloser insert at — Uptie:{AppendUptieNumber}");
                                }
                            }
                        }
                    }
                    else // If skill does not have upties, take default values from Characteristics and Specific
                    {
                        SkillContstructor_Uptie SharedUptie = new SkillContstructor_Uptie()
                        {
                            CoinPower = this.Characteristics.CoinPower,
                            BasePower = this.Characteristics.BasePower,
                            Affinity  = this.Specific.Affinity,
                            IconID    = this.IconID,
                        };

                        LogCustomSkillsConstructor($"    * Shared options (No Upties) ^ '{this.Specific.Affinity}' — Base Power:{this.Characteristics.BasePower} | Coin Power:{this.Characteristics.CoinPower} | Another Icon:{this.IconID.nullHandle()}");
                        Skill_Upties = new Dictionary<string, SkillContstructor_Uptie>();
                        foreach (string Uptie in new List<string> { "1", "2", "3", "4" })
                        {
                            Skill_Upties[Uptie] = SharedUptie;
                        }
                    }
                    LogCustomSkillsConstructor("---------------------------------------------------------");
                    LogCustomSkillsConstructor();
                }
            }
        }

        public record SkillContstructor_Specific
        {
            [JsonProperty("Damage Type")] public string DamageType { get; set; } = "None";
            [JsonProperty("Affinity"   )] public string Affinity   { get; set; } = "None";
            [JsonProperty("Action"     )] public string Action     { get; set; } = "Attack";
            [JsonProperty("Rank"       )] public int    Rank       { get; set; } = 1;
            [JsonProperty("Copies"     )] public int?   Copies     { get; set; }

            [OnDeserialized]
            private void TechnicalProcessing(StreamingContext Context)
            {
                if (!Affinity.EqualsOneOf("Wrath", "Lust", "Sloth", "Gluttony", "Gloom", "Pride", "Envy", "None")) Affinity = "None";

                if (Rank > 3) Rank = 3;
                if (Rank < 1) Rank = 1;
                if (!DamageType.EqualsOneOf("Pierce", "Blunt", "Slash")) DamageType = "None";
            }
        }

        public record SkillContstructor_Characteristics
        {
            [JsonProperty("Coin Power")] public int CoinPower { get; set; } = 0;
            [JsonProperty("Base Power")] public int BasePower { get; set; } = 0;

            [JsonProperty("Coins List")] public List<string> CoinsList { get; set; } = new List<string>();
            [JsonProperty("Coins Type")] public      string  CoinsType { get; set; } = "Plus";

            [JsonProperty("Attack Weight"   )] public int AttackWeight    { get; set; } = 1;
            [JsonProperty("Base Level"      )] public int BaseLevel       { get; set; } = 55;
            [JsonProperty("Level Correction")] public int LevelCorrection { get; set; } = 0;

            [OnDeserialized]
            private void OnDeserialized(StreamingContext Context)
            {
                CoinsType = CoinsType.Equals("Minus") ? "-" : "+";
                if (CoinsList.Count == 0) CoinsList.Add("Regular");
            }
        }

        public record SkillContstructor_Attributes
        {
            [JsonProperty("Unobservable")] public bool Unobservable  { get; set; } = false;

            [JsonProperty("Hide 'Copies'")] public bool HideSkillCopies { get; set; } = false;

            [JsonProperty("Hide 'Base Power'")] public bool HideBasePower { get; set; } = false;
            [JsonProperty("Hide 'Coin Power'")] public bool HideCoinPower { get; set; } = false;
            [JsonProperty("Hide 'Base Level'")] public bool HideBaseLevel { get; set; } = false;

            [JsonProperty("Hide 'Attack Weight'")] public bool HideAttackWeight { get; set; } = false;

            [JsonProperty("Show Affinity Icon")] public bool ShowAffinityIcon { get; set; } = false;

            [JsonProperty("Override 'Base Power'")] public string? OverrideBasePower { get; set; }
            [JsonProperty("Override 'Coin Power'")] public string? OverrideCoinPower { get; set; }
            [JsonProperty("Override 'Base Level'")] public string? OverrideBaseLevel { get; set; }
        }

        public record SkillContstructor_Uptie
        {
            [JsonProperty("Coin Power")] public int?    CoinPower { get; set; }
            [JsonProperty("Base Power")] public int?    BasePower { get; set; }
            [JsonProperty("Affinity"  )] public string  Affinity  { get; set; }
            [JsonProperty("Icon ID"   )] public string? IconID    { get; set; }
        }
    }
}
