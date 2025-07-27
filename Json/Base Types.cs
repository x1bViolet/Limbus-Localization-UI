using Newtonsoft.Json;
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
                public string Description { get; set; }

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; }

                public string _comment { get; set; }

                [OnDeserialized]
                private void OnInit(StreamingContext context)
                {
                    if (Description.IsNull())
                    {
                        Description = "";
                    }
                    
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
                public string Description { get; set; }

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; }

                public string summary { get; set; }

                [OnDeserialized]
                private void OnInit(StreamingContext context)
                {
                    if (Description.IsNull())
                    {
                        Description = "";
                    }

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
                public string Description { get; set; }

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
                    if (Description.IsNull()) Description = "";

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

                [JsonIgnore]
                public List<int> UpgradeLevelsAssociativeIDs { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("desc")]
                public string Description { get; set; }

                [JsonIgnore] // For editor
                public string EditorDescription { get; set; }

                public string _comment { get; set; }

                [JsonIgnore] // For Preview
                public string UpgradeLevel { get; set; }

                [OnDeserialized]
                private void OnInit(StreamingContext context)
                {
                    if (Description.IsNull())
                    {
                        Description = "";
                    }

                    EditorDescription = Description;

                    UpgradeLevel = "1";
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
                public string ID { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("desc")]
                public string Description { get; set; }

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
                    if (Description.IsNull()) Description = "";
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
