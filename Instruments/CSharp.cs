using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;

namespace LCLocalizationInterface.Instruments
{
    public static partial class CSharp
    {
        /// <summary>
        /// <see cref="Console.WriteLine"/>
        /// </summary>
        public static void rin(params object?[] Info) => Console.WriteLine(string.Join(", ", Info));




        #region System dialogues
        public static SaveFileDialog NewSaveFileDialog(string FilesHint, IEnumerable<string> Extensions, string FileDefaultName = "", string DefaultDirectory = "")
        {
            List<string> FileFilters_DefaultExt = [];
            List<string> FileFilters_Filter = [];

            foreach (string Filter in Extensions)
            {
                FileFilters_DefaultExt.Add($".{Filter}");
                FileFilters_Filter.Add($"*.{Filter}");
            }

            SaveFileDialog FileSaving = new()
            {
                DefaultExt = string.Join("|", FileFilters_DefaultExt), // .png|.jpg
                Filter = $"{FilesHint}|{string.Join(";", FileFilters_Filter)}",  // *.png;*.jpg
                FileName = FileDefaultName,
                DefaultDirectory = DefaultDirectory
            };

            return FileSaving;
        }


        public static OpenFileDialog NewOpenFileDialog(string FilesHint, IEnumerable<string> Extensions, string DefaultDirectory = "")
        {
            List<string> FileFilters_DefaultExt = [];
            List<string> FileFilters_Filter = [];

            foreach (string Filter in Extensions.Select(Extension => Extension.Trim()))
            {
                FileFilters_DefaultExt.Add($".{Filter}");
                FileFilters_Filter.Add($"*.{Filter}");
            }

            OpenFileDialog FileSelection = new()
            {
                DefaultExt = string.Join("|", FileFilters_DefaultExt), // .png|.jpg
                Filter = $"{FilesHint}|{string.Join(";", FileFilters_Filter)}",  // *.png;*.jpg,
                DefaultDirectory = DefaultDirectory
            };

            return FileSelection;
        }
        #endregion




        #region String shenanigans
        public static string RegexRemove(this string TargetString, Regex PartPattern)
        {
            TargetString = PartPattern.Replace(TargetString, Match => "");
            return TargetString;
        }
        public static string RegexReplace(this string TargetString, [StringSyntax(StringSyntaxAttribute.Regex)] string Pattern, string Repalcement)
        {
            TargetString = Regex.Replace(TargetString, Pattern, Repalcement);
            return TargetString;
        }
        public static string RegexRemove(this string TargetString, [StringSyntax(StringSyntaxAttribute.Regex)] string PartPattern)
        {
            TargetString = Regex.Replace(TargetString, PartPattern, "");
            return TargetString;
        }

        public static string RemovePrefix(this string Target, params string[] Prefixes)
        {
            if (Target.StartsWithOneOf(Prefixes))
            {
                foreach (string SinglePrefix in Prefixes)
                {
                    if (Target.StartsWith(SinglePrefix))
                    {
                        return Target[SinglePrefix.Length..];
                    }
                }
            }

            return Target;
        }
        public static string RemovePostfix(this string Target, params string[] Postfixes)
        {
            if (Target.EndsWithOneOf(Postfixes))
            {
                foreach (string SinglePostfix in Postfixes)
                {
                    if (Target.EndsWith(SinglePostfix))
                    {
                        return Target[0..^SinglePostfix.Length];
                    }
                }
            }

            return Target;
        }


        public static string Del(this string Target, params string[] FragmentsToRemove)
        {
            if (!string.IsNullOrEmpty(Target))
            {
                foreach (string? Fragment in FragmentsToRemove)
                {
                    if (!string.IsNullOrEmpty(Fragment)) Target = Target.Replace(Fragment, "");
                }
            }

            return Target;
        }


        /// <summary>Formatter for [$] insertions</summary>
        public static string Extern(this string TargetString, object? Replacement)
        {
            return TargetString.Replace("[$]", $"{Replacement}");
        }

        /// <summary>Formatter for enumerated [$n] insertions</summary>
        public static string Exform(this string TargetString, params object?[] Replacements)
        {
            for (int i = 0; i < Replacements.Length; i++)
            {
                TargetString = TargetString.Replace($"[${i + 1}]", $"{Replacements[i]}");
            }

            return TargetString;
        }
        #endregion




        #region Files
        /// <summary>
        /// Find first file matched the <c>"<paramref name="SearchKeyName"/>.[<paramref name="Extensions"/>]"</c> pattern inside <paramref name="SearchDirectory"/>
        /// </summary>
        public static bool TryGetFileWithKeyName(string SearchDirectory, string SearchKeyName, string[] Extensions, [NotNullWhen(true)] out FileInfo? FoundFile)
        {
            try
            {
                List<FileInfo> Files = [.. new DirectoryInfo(SearchDirectory)
                    .GetFiles($"{SearchKeyName}.*", SearchOption.AllDirectories)
                    .Where(File => Extensions.Any(Extension => File.Extension.Equals(Extension, StringComparison.OrdinalIgnoreCase))).Reverse()];

                FoundFile = Files.Count > 0 ? Files[0] : null;
            }
            catch { FoundFile = null; }

            return FoundFile is not null;
        }


        public static bool EncodingHasBOM(this FileInfo TargetFile)
        {
            using (StreamReader Reader = new(TargetFile.FullName, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            {
                Reader.Read(); // Read the first character to trigger encoding detection
                return Reader.CurrentEncoding.GetPreamble().Length != 0;
            }
        }


        /// <summary>
        /// Instead of <see cref="File.ReadAllText"/> because the sometimes file may be locked by a text editor
        /// </summary>
        public static string StreamReadText(string FilePath)
        {
            string Text = "";

            using (FileStream FileStream = new(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader StreamReader = new(FileStream)) { while (!StreamReader.EndOfStream) Text = StreamReader.ReadToEnd()!; }

            return Text;
        }


        /// <summary>
        /// Instead of <see cref="File.ReadAllLines"/> because the sometimes file may be locked by a text editor
        /// </summary>
        public static List<string> StreamReadLines(string FilePath)
        {
            List<string> Lines = [];

            using (FileStream FileStream = new(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader StreamReader = new(FileStream)) { while (!StreamReader.EndOfStream) Lines.Add(StreamReader.ReadLine()!); }

            return Lines;
        }



        public static List<string> TryReadAllLines(string FilePath, List<string> Fallback)
        {
            if (File.Exists(FilePath))
            {
                try   { return [.. StreamReadLines(FilePath)]; }
                catch { return Fallback; }
            }
            else
            {
                return Fallback;
            }
        }

        public static string TryReadAllText(string FilePath, string Fallback = "")
        {
            if (File.Exists(FilePath))
            {
                try   { return StreamReadText(FilePath); }
                catch { return Fallback; }
            }
            else
            {
                return Fallback;
            }
        }
        #endregion




        #region Actions
        public static void Try(Action TryAction) { try { TryAction(); } catch { } }
        public static void Await(double Seconds, Action CompleteAction)
        {
            DispatcherTimer Timer = new() { Interval = TimeSpan.FromSeconds(Seconds) };
            Timer.Tick += (_, _) =>
            {
                Timer.Stop();
                CompleteAction.Invoke();
            };
            Timer.Start();
        }
        #endregion




        #region Dictionaries
        /// <summary>Without <see cref="ArgumentException"/> <c>"An item with the same key has already been added."</c></summary>
        public static Dictionary<TKey, TValue> ToDictionarySafe<TSource, TKey, TValue>(
            this IEnumerable<TSource> Source,
            Func<TSource, TKey>   KeySelector,
            Func<TSource, TValue> ValueSelector
        ) where TKey : notnull {
            Dictionary<TKey, TValue> Output = [];
            foreach (TSource Item in Source) Output.TryAdd(KeySelector(Item), ValueSelector(Item));
            return Output;
        }


        public static bool ContainsOneOfTheseKeys<TKey, TValue>(this IDictionary<TKey, TValue> CheckDictionary, params TKey[] Keys) where TKey : notnull
        {
            return Keys.Any(CheckDictionary.ContainsKey);
        }
        public static bool ContainsKeyCaseInsensitive<TValue>(this Dictionary<string, TValue> Source, string TargetKey, [NotNullWhen(true)] out string? FoundKey)
        {
            FoundKey = null;
            foreach (string Key in Source.Keys)
            {
                if (Key.Equals(TargetKey, StringComparison.OrdinalIgnoreCase))
                {
                    FoundKey = Key;
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetValueCaseInsensitive<TValue>(this Dictionary<string, TValue> Source, string TargetKey, out TValue? GettedValue)
        {
            GettedValue = default;
            foreach (string Key in Source.Keys)
            {
                if (Key.Equals(TargetKey, StringComparison.OrdinalIgnoreCase))
                {
                    GettedValue = Source[Key];
                    return true;
                }
            }

            return false;
        }
        public static Dictionary<string, string> RemoveItemWithValue(this Dictionary<string, string> TargetDictionary, string RemoveValue)
        {
            foreach (KeyValuePair<string, string> StringItem in TargetDictionary.Where(KeyValuePair => KeyValuePair.Value.Equals(RemoveValue)).ToList())
            {
                TargetDictionary.Remove(StringItem.Key);
            }

            return TargetDictionary;
        }

        public static Dictionary<string, TValue> ReorderDictionaryByStringKeysLength<TValue>(Dictionary<string, TValue> Target)
        {
            return Target.OrderByDescending(KeyValuePair => KeyValuePair.Key.Length).ToDictionary();
        }
        #endregion




        #region Reflection shenanigans
        public static bool HasAttribute<AttributeType>(this PropertyInfo Property) where AttributeType : Attribute => Property.GetCustomAttribute<AttributeType>() is not null;
        public static bool HasAttribute<AttributeType>(this PropertyInfo Property, out AttributeType AcquiredAttribute) where AttributeType : Attribute
        {
            AcquiredAttribute = Property.GetCustomAttribute<AttributeType>()!;
            return AcquiredAttribute is not null;
        }


        public static T? GetPropertyValue<T>(this object TargetObject, string PropertyName, BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance)
        {
            return (T?)TargetObject.GetType().GetProperty(name: PropertyName, Flags)?.GetValue(obj: TargetObject);
        }
        public static void SetPropertyValue<T>(this object TargetObject, string PropertyName, T? Value, BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance)
        {
            TargetObject.GetType().GetProperty(name: PropertyName, Flags)?.SetValue(obj: TargetObject,  value: Value);
        }


        public static List<T> GetTypedProperties<T>(this object Target, Func<PropertyInfo, bool>? Predicate = null)
        {
            Predicate ??= x => true;

            return [.. Target.GetType().GetProperties().Where(Predicate).Where(x => x.PropertyType == typeof(T)).Select(x => (T)x.GetValue(obj: Target)!)];
        }
        public static List<PropertyInfo> GetTypedPropertyInfos<T>(this object Target, Func<PropertyInfo, bool>? Predicate = null)
        {
            Predicate ??= x => true;

            return [.. Target.GetType().GetProperties().Where(Predicate).Where(x => x.PropertyType == typeof(T))];
        }
        public static List<T> GetBaseTypedProperties<T>(this object Target, Func<PropertyInfo, bool>? Predicate = null)
        {
            Predicate ??= x => true;

            return [.. Target.GetType().GetProperties().Where(Predicate).Where(x => x.PropertyType.BaseType == typeof(T)).Select(x => (T)x.GetValue(obj: Target)!)];
        }
        #endregion




        #region (Multi-)Equality expressions
        public static bool EqualsToOneOf(this object CheckObject, params object[] Comparsions) => Comparsions.Any(CheckObject.Equals);


        public static bool EndsWithOneOf(this string CheckString, params string[] Comparsions) => Comparsions.Any(CheckString.EndsWith);
        public static bool StartsWithOneOf(this string CheckString, params string[] Comparsions) => Comparsions.Any(CheckString.StartsWith);


        public static bool ContainsOneOf(this string CheckString, params string[] Comparsions) => Comparsions.Any(CheckString.Contains);
        public static bool ContainsOneOf(this IEnumerable<string> CheckStrings, params string[] Comparsions) => Comparsions.Any(CheckStrings.Contains);


        public static bool Matches(this string CheckString, [StringSyntax(StringSyntaxAttribute.Regex)] string Pattern) => Regex.IsMatch(CheckString, Pattern);
        public static bool MatchesOneOf(this string CheckString, [StringSyntax(StringSyntaxAttribute.Regex)] params string[] Patterns) => Patterns.Any(Pattern => Regex.IsMatch(CheckString, Pattern));
        #endregion
    }
}