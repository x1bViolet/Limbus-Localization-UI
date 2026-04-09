using LCLocalizationInterface.Instruments.Classes;

namespace LCLocalizationInterface.LimbusRegistry.AssetsDirectoryLoaders
{
    public static partial class ImageDictionaries
    {
        public static readonly BitmapImage UnknownSpriteImage = BitmapFromResource(@"UI\Limbus\Unknown.png");

        private static readonly List<string> KeywordsPreloadList =
            [.. TryReadAllLines(@"[⇲] Assets Directory\Limbus Images\Keywords\Preload list.txt", []).Where(Line => !string.IsNullOrWhiteSpace(Line))];



        public static DynamicKeyImagesDictionary KeywordImages { get; } = new (SourceDirectory: @"[⇲] Assets Directory\Limbus Images\Keywords", PreloadList: KeywordsPreloadList)
        {
            NotifyDictionaryUpdateAction = delegate ()
            {
                @PartialStateUpdater.Limbus.FullyRefreshShownRichText();

                if (@EditorModesShelf.Keywords.CurrentFile is not null)
                {
                    @EditorModesShelf.Keywords.ChangeKeywordIconView();
                }
            }
        };


        public static DynamicKeyImagesDictionary SkillIcons { get; } = new(SourceDirectory: @"[⇲] Assets Directory\Limbus Images\Skills\Icons")
        {
            NotifyDictionaryUpdateAction = delegate ()
            {
                if (@EditorModesShelf.Skills.CurrentFile is not null && LoadedConfiguration.PreviewSettings.Base.EnableSkillNamesReplication)
                {
                    @EditorModesShelf.Skills.ChangeSkillNameReplicaAppearance();
                }
            }
        };



        /// <summary>
        /// Primary tries to get image from <see cref="LoadedImages"/> dictionary with already loaded images.
        /// <br/>
        /// Otherwise search image file in <see cref="ImagesDirectory"/>, then adds it to the <see cref="LoadedImages"/> and finally returns.
        /// <br/><br/>
        /// Also configures <see cref="FileEventsNotifier"/> to register changes of loaded images and update content dynamically.
        /// </summary>
        public class DynamicKeyImagesDictionary
        {
            public string ImagesDirectory { get; }

            /// <summary>
            /// Key is file name without extension, value is <see cref="BitmapImage"/> loaded from file
            /// </summary>
            private Dictionary<string, BitmapImage> LoadedImages = [];

            /// <summary>
            /// If failed to get image from <see cref="LoadedImages"/> or <see cref="ImagesDirectory"/>(By <see cref="TryGetFileWithKeyName"/>) on <see cref="this[string]"/>
            /// </summary>
            private List<string> UnsuccessfullyRequestedIDs = [];

            public BitmapImage this[string ImageKey]
            {
                get
                {
                    if (UnsuccessfullyRequestedIDs.Contains(ImageKey) == false)
                    {
                        if (!LoadedImages.ContainsKey(ImageKey) && TryGetFileWithKeyName(this.ImagesDirectory, ImageKey, [".png", ".jpg", ".jpeg"], out FileInfo? FoundImageFile))
                        {
                            // Dynamic load on demand
                            LoadedImages[ImageKey] = BitmapFromFile(FoundImageFile!.FullName);
                        }
                    }

                    BitmapImage ResolvedImage = LoadedImages.TryGetValue(ImageKey, out BitmapImage? FoundImage) ? FoundImage : UnknownSpriteImage;

                    if (ResolvedImage == UnknownSpriteImage && UnsuccessfullyRequestedIDs.Contains(ImageKey) == false)
                    {
                        UnsuccessfullyRequestedIDs.Add(ImageKey);
                    }

                    return ResolvedImage;
                }
            }


            public Action? NotifyDictionaryUpdateAction { get; init; }


            public FileEventsNotifier ImagesWatcher { get; }
            public DynamicKeyImagesDictionary(string SourceDirectory, List<string>? PreloadList = null)
            {
                ImagesDirectory = SourceDirectory;

                if (PreloadList is not null)
                {
                    foreach (FileInfo ImageFile in new DirectoryInfo(ImagesDirectory)
                        .GetFiles("*.*", SearchOption.AllDirectories)
                        .Where(ImageFile => Path.GetFileNameWithoutExtension(ImageFile.Name).EqualsToOneOf([.. PreloadList]))
                    ) {
                        LoadedImages[Path.GetFileNameWithoutExtension(ImageFile.Name)] = BitmapFromFile(ImageFile.FullName);
                    }
                }

                ImagesWatcher = new FileEventsNotifier(SourceDirectory, ["*.png", "*.jpg", "*.jpeg"])
                {
                    EventsRaisingDelay = 250, // Images files saved from some editors is temporary locked by them at the moment of saving
                    Created = delegate (object Sender, FileSystemEventArgs Args)
                    {
                        string ImageKey = Path.GetFileNameWithoutExtension(Args.Name)!;
                        if (UnsuccessfullyRequestedIDs.Contains(ImageKey))
                        {
                            LoadedImages[ImageKey] = BitmapFromFile(Args.FullPath);
                            UnsuccessfullyRequestedIDs.Remove(ImageKey);
                            NotifyDictionaryUpdateAction?.Invoke();
                        }
                    },
                    Deleted = delegate (object Sender, FileSystemEventArgs Args)
                    {
                        string ImageKey = Path.GetFileNameWithoutExtension(Args.Name)!;
                        if (LoadedImages.Remove(ImageKey))
                        {
                            NotifyDictionaryUpdateAction?.Invoke();
                        }
                    },
                    Renamed = delegate (object Sender, RenamedEventArgs Args)
                    {
                        string OldImageKey = Path.GetFileNameWithoutExtension(Args.OldName)!;
                        string NewImageKey = Path.GetFileNameWithoutExtension(Args.Name)!;
                        if (LoadedImages.ContainsKey(OldImageKey))
                        {
                            LoadedImages.Remove(OldImageKey);
                            LoadedImages[NewImageKey] = BitmapFromFile(Args.FullPath);
                            NotifyDictionaryUpdateAction?.Invoke();
                        }
                        else if (UnsuccessfullyRequestedIDs.Contains(NewImageKey))
                        {
                            LoadedImages[NewImageKey] = BitmapFromFile(Args.FullPath);
                            UnsuccessfullyRequestedIDs.Remove(NewImageKey);
                            NotifyDictionaryUpdateAction?.Invoke();
                        }
                    },
                    Changed = delegate (object Sender, FileSystemEventArgs Args)
                    {
                        string ImageKey = Path.GetFileNameWithoutExtension(Args.Name)!;
                        if (LoadedImages.ContainsKey(ImageKey))
                        {
                            LoadedImages[ImageKey] = BitmapFromFile(Args.FullPath);
                            NotifyDictionaryUpdateAction?.Invoke();
                        }
                    },
                };
            }
        }




        public static ReadOnlyDictionary<string, string> NotSuitableForSpriteTagRedirections { get; private set; } = ReadOnlyDictionary<string, string>.Empty;

        private static FileEventsNotifier NotSuitableForSpriteTagRedirectionsWatcher { get; } = new(@"[⇲] Assets Directory\Limbus Images\Keywords (Not suitable for Sprite tag)", "*.json")
        {
            GeneralHandler = (_, _, _) => ReadKeywordIconsRedirectionJsonFiles()
        };

        public static void ReadKeywordIconsRedirectionJsonFiles()
        {
            Dictionary<string, string> ReadedThere = [];
            if (Directory.Exists(@"[⇲] Assets Directory\Limbus Images\Keywords (Not suitable for Sprite tag)"))
            {
                foreach (FileInfo RedirectionsFile in new DirectoryInfo(@"[⇲] Assets Directory\Limbus Images\Keywords (Not suitable for Sprite tag)").GetFiles("*.json", SearchOption.AllDirectories))
                {
                    if (RedirectionsFile.TryDeserealizeJsonAs(out Dictionary<string, List<string>> Deserialized, out Exception Occurred))
                    {
                        foreach (KeyValuePair<string, List<string>> Redirection in Deserialized)
                        {
                            string ID = Redirection.Key;
                            foreach (string SubIDThatUsesThisIDAsSprite in Redirection.Value)
                            {
                                ReadedThere[SubIDThatUsesThisIDAsSprite] = ID;
                            }
                        }
                    }
                    else
                    {
                        ErrorMessageWindow.ShowException(Occurred, @$"This exception occurred while trying to read keyword icon redirections file ""<b>{RedirectionsFile.Name}</b>"" from ""<b>[⇲] Assets Directory\Limbus Images\Keywords (Not suitable for Sprite tag)</b>""");
                    }
                }
            }
            else
            {
                // Probably optional
            }
            NotSuitableForSpriteTagRedirections = new(ReadedThere);

            @PartialStateUpdater.Limbus.FullyRefreshShownRichText();
            if (@EditorModesShelf.Keywords.CurrentFile is not null)
            {
                @EditorModesShelf.Keywords.ChangeKeywordIconView();
            }
        }
    }
}