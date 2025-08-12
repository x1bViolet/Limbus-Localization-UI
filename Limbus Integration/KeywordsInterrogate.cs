using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LC_Localization_Task_Absolute.Json;
using Newtonsoft.Json;
using static LC_Localization_Task_Absolute.SettingsWindow;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Keywords;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_SkillTag;
using System.Text.Json.Serialization;

namespace LC_Localization_Task_Absolute.Limbus_Integration
{
    internal abstract class KeywordsInterrogate
    {
        internal protected record KeywordSingleton
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string StringColor { get; set; }
        }

        internal protected record KeywordImagesIDInfo
        {
            [JsonProperty("ID Matches")]
            public List<KeywordImageIDInfo> GeneralInfo { get; set; }
        }
        internal protected record KeywordImageIDInfo
        {
            [JsonProperty("Base ID")]
            public string BaseID { get; set; }
            public string File { get; set; }
        }

        #region Keywords Multiple Meanings create/read
        internal protected static Dictionary<string, List<string>> KeywordsMultipleMeaningsDictionary = new();
        internal protected record KeywordsMultipleMeaningsDictionaryJson
        {
            public List<KeywordsMultipleMeanings> Info { get; set; }
        }
        internal protected record KeywordsMultipleMeanings
        {
            [JsonProperty("Keyword ID")]
            public string KeywordID { get; set; }

            public List<string> Meanings { get; set; }
        }
        internal protected static void ExportKeywordsMultipleMeaningsDictionary(string LocalizationWithKeywordsPath, bool IgnoreRailAndMirrorKeywords = true)
        {
            KeywordsMultipleMeaningsDictionaryJson Export = new KeywordsMultipleMeaningsDictionaryJson();
            Export.Info = new List<KeywordsMultipleMeanings>();
            int FoundCounter = 0;
            foreach (FileInfo LocalizeFile in new DirectoryInfo(LocalizationWithKeywordsPath)
                .GetFiles(searchPattern: "*.json", searchOption: SearchOption.AllDirectories)
                    .Where(file => file.Name.StartsWithOneOf([
                        "Passive",
                        "EGOgift",
                        "Bufs",
                        "BattleKeywords",
                        "Skills",
                        "PanicInfo"
                    ])
                )
            )
            {
                string TextToAnalyze = File.ReadAllText(LocalizeFile.FullName);
                foreach (Match keywordMatch in Regex.Matches(TextToAnalyze, @"<sprite name=\\""(?<ID>\w+)\\""><color=(?<Color>#[a-fA-F0-9]{6})><u><link=\\""\w+\\"">(?<Name>.*?)</link></u></color>"))
                {
                    string ID = keywordMatch.Groups["ID"].Value;
                    string VariativeName = keywordMatch.Groups["Name"].Value;

                    if (!KeywordsMultipleMeaningsDictionary.ContainsKey(ID)) KeywordsMultipleMeaningsDictionary[ID] = new();

                    if (KeywordsGlossary.ContainsKey(ID))
                    {
                        if (!KeywordsMultipleMeaningsDictionary[ID].Contains(KeywordsGlossary[ID].Name))
                        {
                            KeywordsMultipleMeaningsDictionary[ID].Add(KeywordsGlossary[ID].Name);
                        }
                    }

                    if (!KeywordsMultipleMeaningsDictionary[ID].Contains(VariativeName))
                    {
                        KeywordsMultipleMeaningsDictionary[ID].Add(VariativeName);
                    }

                    FoundCounter++;
                }
            }

            foreach (var i in KeywordsMultipleMeaningsDictionary)
            {
                Export.Info.Add(new KeywordsMultipleMeanings()
                {
                    KeywordID = i.Key,
                    Meanings = i.Value
                });
            }

            if (FoundCounter > 0)
            {
                Export.SerializeFormatted("Keywords Multiple Meanings.json");
                MessageBox.Show($"Keywords multiple meanings from \"{LocalizationWithKeywordsPath}\" dir exported as \"Keywords Multiple Meanings.json\" at program folder");
            }
        }
        internal protected static void ReadKeywordsMultipleMeanings(string Filepath)
        {
            if (File.Exists(Filepath))
            {
                KeywordsMultipleMeaningsDictionaryJson Readed = JsonConvert.DeserializeObject<KeywordsMultipleMeaningsDictionaryJson>(File.ReadAllText(Filepath));
                if (Readed != null)
                {
                    KeywordsMultipleMeaningsDictionary = new();
                    foreach(KeywordsMultipleMeanings KeywordMeaningsInfo in Readed.Info)
                    {
                        if (!KeywordsMultipleMeaningsDictionary.ContainsKey(KeywordMeaningsInfo.KeywordID)) KeywordsMultipleMeaningsDictionary[KeywordMeaningsInfo.KeywordID] = new();
                        foreach(string KeywordMeaning in KeywordMeaningsInfo.Meanings)
                        {
                            KeywordsMultipleMeaningsDictionary[KeywordMeaningsInfo.KeywordID].Add(KeywordMeaning);

                            Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter[KeywordMeaning] = KeywordMeaningsInfo.KeywordID;
                        }
                    }
                }
            }
        }
        #endregion

        internal protected static Dictionary<string, KeywordSingleton> KeywordsGlossary = [];
        internal protected static Dictionary<string, string> Keywords_IDName = [];
        /// <summary>
        /// Contains matches of keyword names and their IDs in descending order of name length (For limbus preview formatter, only base keyword names)
        /// </summary>
        internal protected static Dictionary<string, string> Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter = [];
        /// <summary>
        /// Extended version with other keyword meanings (E.g. "Charge", "Charges" for same Charge keyword id)
        /// </summary>
        internal protected static Dictionary<string, string> Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter = [];
        internal protected static Dictionary<string, BitmapImage> KeywordImages = [];
        internal protected static Dictionary<string, BitmapImage> EGOGiftInlineImages = [];
        internal protected static Dictionary<string, string> SkillTags = [];
        internal protected static List<string> KnownID = [];
        
        internal protected static void LoadInlineImages()
        {
            //await Task.Run(async () =>
            //{
                
                //rin($"\n$ Loading keyword images");
            KeywordImages["Unknown"] = new BitmapImage(new Uri("pack://application:,,,/Default/Images/Unknown.png"));
            double Counter = 0;

            List<FileInfo> LoadSite = new DirectoryInfo(@"⇲ Assets Directory\[⇲] Limbus Images\Keywords").GetFiles("*.png", SearchOption.AllDirectories).ToList();
            double TotalCount = LoadSite.Count;

            foreach (FileInfo KeywordImage in LoadSite)
            {
                string TargetID = KeywordImage.Name.Replace(KeywordImage.Extension, "");
                KeywordImages[TargetID] = GenerateBitmapFromFile(KeywordImage.FullName);
                Counter++;
            }

            //    return;
            //});
            //rin($"  {Counter} images loaded from \"⇲ Assets Directory\\[⇲] Limbus Images\\Keywords\" directory");
        }
        internal protected static void InitializeGlossaryFrom(string KeywordsDirectory, bool WriteOverFallback = false, string FilesPrefix = "")
        {
            if (!WriteOverFallback)
            {
                KeywordsGlossary.Clear();
                SkillTags.Clear();
                KnownID.Clear();
            }

            
            int Counter = 0;

            if (Directory.Exists(KeywordsDirectory))
            {
                rin($" {(WriteOverFallback ? " " : "[Fallback] ")}Loading Keywords from \"{KeywordsDirectory}\"{(!FilesPrefix.Equals("") ? $" with files prefix \"{FilesPrefix}\"" : "")}");
                Dictionary<string, string> SkillTagColors = new Dictionary<string, string>();
                if (File.Exists(@"⇲ Assets Directory\[+] Keywords\SkillTag Colors.T[-]"))
                {
                    foreach (string Line in File.ReadAllLines(@"⇲ Assets Directory\[+] Keywords\SkillTag Colors.T[-]").Where(Line => Line.Contains(" ¤ ")))
                    {
                        string[] ColorPair = Line.Split(" ¤ ");
                        if (ColorPair.Count() == 2)
                        {
                            string SkillTagID = ColorPair[0].Trim();
                            string SkillTagColor = ColorPair[1].Trim();
                            //rin($"  Load {SkillTagID} -> {SkillTagColor}");
                            SkillTagColors[SkillTagID] = SkillTagColor;
                        }
                    }
                }

                List<FileInfo> SkillTag_Found = new DirectoryInfo(KeywordsDirectory).GetFiles("*SkillTag.Json").ToList();
                if (SkillTag_Found.Count > 0)
                {
                    BaseTypes.Type_SkillTag.SkillTags SkillTagsJson = JsonConvert.DeserializeObject<SkillTags>(File.ReadAllText(SkillTag_Found[0].FullName));
                    if (SkillTagsJson.dataList != null)
                    {
                        foreach (SkillTag SkillTag in SkillTagsJson.dataList)
                        {
                            if (!SkillTag.ID.Equals(""))
                            {
                                string DefinedColor = "#93f03f";
                                if (SkillTagColors.ContainsKey(SkillTag.ID))
                                {
                                    DefinedColor = SkillTagColors[SkillTag.ID];
                                }
                                else if (SkillTag.Color != null)
                                {
                                    DefinedColor = SkillTag.Color;
                                }

                                SkillTags[$"[{SkillTag.ID}]"] = $"<color={DefinedColor}>{SkillTag.Tag}</color>";
                                //rin($"{SkillTag.ID} -> {SkillTags[$"[{SkillTag.ID}]"]}");
                            }
                        }
                    }
                }

                //rin($"\n$ Loading keyword colors");
                Counter = 0;
                Dictionary<string, string> KeywordColors = [];
                try
                {
                    foreach(string ColorPair in File.ReadAllLines(@"⇲ Assets Directory\[+] Keywords\Keyword Colors.T[-]"))
                    {
                        try
                        {
                            KeywordColors[ColorPair.Split(" ¤ ")[0].Trim()] = ColorPair.Split(" ¤ ")[1].Trim();
                            //rin($"{ColorPair.Split(" ¤ ")[0].Trim()}: {ColorPair.Split(" ¤ ")[1].Trim()}");
                            Counter++;
                        } catch { }
                    }
                } catch { }
                //rin($"  Keyword colors loaded: {Counter}");

                //rin($"\n$ Loading keywords");
                Counter = 0;

                List<FileInfo> LoadSite = new DirectoryInfo(KeywordsDirectory).GetFiles(
                    searchPattern: "*Bufs*.json",
                    searchOption: SearchOption.AllDirectories
                ).ToList();
                double TotalFilesCount = LoadSite.Count;

                
                foreach (FileInfo KeywordFileInfo in LoadSite)
                {
                    var TargetSite = KeywordFileInfo.Deserealize<Keywords>();

                    if (TargetSite != null)
                    {
                        foreach(Keyword KeywordItem in TargetSite.dataList)
                        {
                            if (!KeywordItem.ID.Equals(""))
                            {
                                if (!KeywordItem.ID.ContainsOneOf(DeltaConfig.PreviewSettings.CustomLanguageProperties.KeywordsIgnore))
                                {
                                    string DefinedColor = "#9f6a3a";

                                    if (KeywordColors.ContainsKey(KeywordItem.ID))
                                    {
                                        DefinedColor = KeywordColors[KeywordItem.ID];
                                    }
                                    else if (KeywordItem.Color != null)
                                    {
                                        DefinedColor = KeywordItem.Color;
                                    }

                                    KeywordsGlossary[KeywordItem.ID] = new KeywordSingleton
                                    {
                                        Name = KeywordItem.Name,
                                        Description = KeywordItem.Description,
                                        StringColor = DefinedColor
                                    };

                                    Keywords_IDName[KeywordItem.ID] = KeywordItem.Name;
                                    if (!KeywordItem.ID.EndsWithOneOf(["_Re", "Re", "Mirror"]))
                                    {
                                        //Fallback overwrite
                                        if (Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter.ContainsValue(KeywordItem.ID) & WriteOverFallback)
                                        {
                                            Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter = Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter.RemoveItemWithValue(KeywordItem.ID);
                                        }
                                        Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter[KeywordItem.Name] = KeywordItem.ID;


                                    
                                        Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter[KeywordItem.Name] = KeywordItem.ID;
                                    }

                                    KnownID.Add(KeywordItem.ID);

                                    Counter++;
                                }
                            }
                        }
                    }
                }

                Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter = Keywords_NamesWithIDs_OrderByLength_ForLimbusPreviewFormatter.OrderBy(obj => obj.Key.Length).ToDictionary(obj => obj.Key, obj => obj.Value).Reverse().ToDictionary();

                Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter = Keywords_NamesWithIDs_OrderByLength_ForContextMenuUnevidentConverter.OrderBy(obj => obj.Key.Length).ToDictionary(obj => obj.Key, obj => obj.Value).Reverse().ToDictionary();
            }
            else
            {
                //rin($"\n[!] Keywords Directory \"{KeywordsDirectory}\" not found");
            }
        }
    }
}
