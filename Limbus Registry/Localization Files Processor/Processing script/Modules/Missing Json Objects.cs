using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using static LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing.Modules.Main;

namespace LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing
{
    namespace Modules
    {
        public static class MissingJsonObjects
        {
            /// <summary>
            /// Insert missing objects to <paramref name="TargetJson"/> dataList from <paramref name="ReferenceJson"/> by their IDs
            /// </summary>
            public static string CompareAppendDataList(string TargetJson, string ReferenceJson)
            {
                bool SomethingWasAdded = false;
                if (TargetJson.TryDeserealizeJsonAs(out LimbusLocalizationFile<dynamic> TargetJson_Deserialized, out _) && ReferenceJson.TryDeserealizeJsonAs(out LimbusLocalizationFile<dynamic> ReferenceJson_Deserialized, out _))
                {
                    if (ReferenceJson_Deserialized.DataList.Count > 0)
                    {
                        // Create list with IDs that TargetJson dataList currently has (string/int)
                        List<dynamic> TargetJson_KnownIDs = [.. TargetJson_Deserialized.DataList.Select(x => x.id)];
                        List<dynamic> ReferenceJson_KnownIDs = [.. ReferenceJson_Deserialized.DataList.Select(x => x.id)];

                        int ReferenceJsonEnumeratorStart = 0;

                        if (DataContextDomain.LocalizationFilesProcessor.Profile.ReferenceLocalization.MissingContentAppending.CountIDsAsMissedStartingFromLastOneFromSourceFile & TargetJson_Deserialized.DataList.Count > 0)
                        {
                            ReferenceJsonEnumeratorStart = ReferenceJson_KnownIDs.IndexOf(TargetJson_KnownIDs[^1]);
                            if (ReferenceJsonEnumeratorStart == -1) ReferenceJsonEnumeratorStart = 0;
                        }

                        // Then, check ReferenceJson dataList
                        foreach (dynamic ReferenceJson_dataList_Object in ReferenceJson_Deserialized.DataList[ReferenceJsonEnumeratorStart..])
                        {
                            // If list with TargetJson_Deserialized IDs does not contain current object ID, append it
                            if (ReferenceJson_dataList_Object.id != null && !TargetJson_KnownIDs.Contains(ReferenceJson_dataList_Object.id))
                            {
                                TargetJson_Deserialized.DataList.Add(ReferenceJson_dataList_Object);
                                SomethingWasAdded = true;
                            }
                        }
                    }
                }


                return SomethingWasAdded ? TargetJson_Deserialized.SerializeToFormattedJsonText() : TargetJson;
            }
        }
    }
}