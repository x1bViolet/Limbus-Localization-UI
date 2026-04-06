namespace LCLocalizationInterface.Instruments
{
    namespace Classes
    {
        /// <summary>
        /// Transformed <see cref="FileSystemWatcher"/> for more convenient and shortened code:<br/>
        /// • Events is now properties with <see langword="init"/> accessor so that they can be instantly set upon <see cref="FileEventsNotifier"/> creation instead of having to find a place or a static constructor where this must be done manually<br/>
        /// • Those events are invoked through <see cref="App.HandledInvoke"/><br/>
        /// • <see cref="GeneralHandler"/> to handle all 4 events at once (Sets them)<br/>
        /// • <see cref="OnChangedCallTimeoutDuration"/> to prevent multiple raising of events on file save from programs like Paint 3D or VS Code<br/>
        /// • <see cref="EventsRaisingDelay"/> (Because sometimes json file saved from VS Code considered as null on deserialization right after Changed event)
        /// </summary>
        public class FileEventsNotifier : FileSystemWatcher
        {
            #region Constructors
            public FileEventsNotifier()
            {
                DefaultInit();
            }
            public FileEventsNotifier(string TargetDirectory, string? FileFilter = null, bool IncludeSubdirectories = true)
            {
                DefaultInit(); WatchDirectory(TargetDirectory, [FileFilter ?? "*.*"], IncludeSubdirectories);
            }
            public FileEventsNotifier(string TargetDirectory, Collection<string> FileFilters, bool IncludeSubdirectories = true)
            {
                DefaultInit(); WatchDirectory(TargetDirectory, FileFilters, IncludeSubdirectories);
            }
            public FileEventsNotifier(string TargetFile)
            {
                DefaultInit(); WatchFile(TargetFile);
            }
            #endregion



            private void DefaultInit()
            {
                base.Changed += async delegate (object Sender, FileSystemEventArgs Args)
                {
                    if (this.OnChangedCallTimeout == false)
                    {
                        this.OnChangedCallTimeout = true;

                        await Task.Delay(this.EventsRaisingDelay);
                        App.HandledInvoke(delegate () { this.Changed?.Invoke(Sender, Args); });

                        await Task.Delay(this.OnChangedCallTimeoutDuration);
                        this.OnChangedCallTimeout = false;
                    }
                };

                this.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            }
                
            public void WatchFile(string TargetFile)
            {
                Reset();

                if (File.Exists(TargetFile))
                {
                    this.Path = System.IO.Path.GetDirectoryName(TargetFile)!;
                    this.Filter = System.IO.Path.GetFileName(TargetFile)!;

                    this.IncludeSubdirectories = false;
                    this.EnableRaisingEvents = true;
                }
                else
                {
                    ErrorMessageWindow.ShowException(new FileNotFoundException("File not found"), $"This exception occured while trying to setup file watcher for \"{TargetFile}\"");
                }
            }
            public void WatchDirectory(string TargetDirectory, Collection<string> Filters, bool IncludeSubdirectories = true)
            {
                Reset();

                if (Directory.Exists(TargetDirectory))
                {
                    this.Path = TargetDirectory;
                    this.Filters.Clear();
                    foreach (string Filter in Filters)
                    {
                        this.Filters.Add(Filter);
                    }

                    this.IncludeSubdirectories = IncludeSubdirectories;
                    this.EnableRaisingEvents = true;
                }
                else
                {
                    ErrorMessageWindow.ShowException(new DirectoryNotFoundException("Directory not found"), $"This exception occured while trying to setup directory watcher for \"{TargetDirectory}\"");
                }
            }
            public void Reset()
            {
                this.EnableRaisingEvents = false;
                this.IncludeSubdirectories = false;
                this.Filters.Clear();
                this.Filter = "*.*";
            }


            public bool OnChangedCallTimeout { get; private set; } = false;
            public int OnChangedCallTimeoutDuration { get; set; } = 50;

            public int EventsRaisingDelay { get; set; } = 10;


            public new FileSystemEventHandler? Changed { private get; init; } /// On <see cref="DefaultInit"/>
            public new FileSystemEventHandler? Created
            {
                init { base.Created += delegate (object Sender, FileSystemEventArgs Args) { App.HandledInvoke(async delegate () { await Task.Delay(EventsRaisingDelay); value?.Invoke(Sender, Args); }); }; }
            }
            public new FileSystemEventHandler? Deleted
            {
                init { base.Deleted += delegate (object Sender, FileSystemEventArgs Args) { App.HandledInvoke(async delegate () { await Task.Delay(EventsRaisingDelay); value?.Invoke(Sender, Args); }); }; }
            }
            public new RenamedEventHandler? Renamed
            {
                init { base.Renamed += delegate (object Sender, RenamedEventArgs Args) { App.HandledInvoke(async delegate () { await Task.Delay(EventsRaisingDelay); value?.Invoke(Sender, Args); }); }; }
            }

            public delegate void GeneralFileSystemWatcherEventHandler(object Sender, FileSystemEventArgs? FileSystemArgs, RenamedEventArgs? RenameArgs);
            public GeneralFileSystemWatcherEventHandler GeneralHandler
            {
                init
                {
                    Changed = delegate (object Sender, FileSystemEventArgs Args) { value.Invoke(Sender, Args, null); };
                    Created = delegate (object Sender, FileSystemEventArgs Args) { value.Invoke(Sender, Args, null); };
                    Deleted = delegate (object Sender, FileSystemEventArgs Args) { value.Invoke(Sender, Args, null); };
                    Renamed = delegate (object Sender, RenamedEventArgs    Args) { value.Invoke(Sender, null, Args); };
                }
            }
        }
    }
}