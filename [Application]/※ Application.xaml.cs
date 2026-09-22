using LCLocalizationInterface.Internal.Configuration;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using LCLocalizationInterface.LimbusRegistry.LocalizationFilesProcessing;
using LCLocalizationInterface.LimbusRegistry.PreviewCreator;

namespace LCLocalizationInterface
{
    public partial class App : Application
    {
        public static string ThisAssembly { get; } = Assembly.GetExecutingAssembly().GetName().Name!;
        public static bool ProgramFullyLoaded { get; private set; } = false;




        public record QuickJsonConfigurationFile
        {
            [JsonProperty("Internal")]
            public Internal_PROP Internal { get; set; } = new();

            public record Internal_PROP
            {
                [JsonProperty("Show startup progress")]
                public bool ShowStartupProgress { get; set; } = true;

                [JsonProperty("UI Language")]
                public string UILanguage { get; set; } = "";
            }
        }

        /// <summary>Content initialization order</summary>
        protected override async void OnStartup(StartupEventArgs Args)
        {
            base.OnStartup(Args);

            try   {          Console.OutputEncoding = Encoding.UTF8;            }
            catch { /* Then application output type is "Windows Application" */ }

            if (typeof(MainWindow) == typeof(MainWindow))
            {
                SetupExceptionsHandling();
                ErrorMessageWindow.ErrorMessageWindowInstance = new();
                SetupExternalStatics();

                // Sync window will freeze until last phrase during further operations
                SplashScreenWindow.SplashScreenWindowInstance = await CreateAnotherThreadWindow<SplashScreenWindow>();

                #region Quick config checks
                try
                {
                    if (new FileInfo(@"[⇲] Assets Directory\Configurazione^.json").TryDeserealizeJsonAs(out QuickJsonConfigurationFile QuickConfig, out _))
                    {
                        if (QuickConfig.Internal.ShowStartupProgress == false)
                        {
                            SplashScreenWindow.HideText();
                        }
                        if (File.Exists(@$"{QuickConfig.Internal.UILanguage}\Logo.png"))
                        {
                            SplashScreenWindow.SplashScreenWindowInstance.Dispatcher.Invoke(delegate ()
                            {
                                SplashScreenWindow.SplashScreenWindowInstance.ShownImage.Source = BitmapFromFile(@$"{QuickConfig.Internal.UILanguage}\Logo.png");
                            });
                        }
                        if (new FileInfo(@$"{QuickConfig.Internal.UILanguage}\Startup Steps.json").TryDeserealizeJsonAs(out @Languages.JsonClasses.StartupSteps ReadedStartupSteps, out _))
                        {
                            @Languages.VariableData.ReadedStartupSteps = ReadedStartupSteps;
                        }
                    }
                }
                catch { }
                #endregion

                SplashScreenWindow.SplashScreenWindowInstance.Dispatcher.Invoke(SplashScreenWindow.SplashScreenWindowInstance.Show);
                {
                    string WindowsInitalizingString = @Languages.VariableData.ReadedStartupSteps.MainStages.WindowsInitializing;

                    /**/SplashScreenWindow.ProgressObject = WindowsInitalizingString.Exform(1, 5);
                    MainWindowInstance = new();
                    Application.Current.MainWindow = MainWindowInstance;

                    /**/SplashScreenWindow.ProgressObject = WindowsInitalizingString.Exform(2, 5);
                    PreviewCreatorPage.PreviewCreatorPageInstance = new();
                    ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance = new();
                    MainWindowInstance.PreviewCreatorContentEmitter.Content = PreviewCreatorPage.PreviewCreatorPageInstance;

                    /**/SplashScreenWindow.ProgressObject = WindowsInitalizingString.Exform(3, 5);
                    SkillsDisplayInfoManagerWindow.SkillsDisplayInfoManagerWindowInstance = new();

                    /**/SplashScreenWindow.ProgressObject = WindowsInitalizingString.Exform(4, 5);
                    SettingsWindow.SettingsWindowInstance = new();

                    /**/SplashScreenWindow.ProgressObject = WindowsInitalizingString.Exform(5, 5);
                    LocalizationFilesProcessorWindow.LocalizationFilesProcessorWindowInstance = new();


                    /**/SplashScreenWindow.ProgressObject = WindowsInitalizingString.Exform(6, 5);
                    UpdateNoticeWindow.UpdateNoticeWindowInstance = new();
                    ConfirmDialog.ConfirmDialogInstance = new();
                    InputDialog.InputDialogInstance = new();


                    @Languages.StashDefaultValues();

                    /**/SplashScreenWindow.ProgressObject = @Languages.VariableData.ReadedStartupSteps.MainStages.Configuration;
                    @Configurazione.Load(); // Change DataContext values and load language with theme

                    // Misc assets directory things
                    ContextMenuHotkeys.ReadFile();
                    KeywordsEditorBackgrounds.ReadAndSetImages();

                    @EditorModesShelf.MainMenu.SwitchUI();


                    //////////////////////////
                    ProgramFullyLoaded = true;
                    //////////////////////////

                    /**/SplashScreenWindow.ProgressSubObject = "";
                    /**/SplashScreenWindow.ProgressObject = @Languages.VariableData.ReadedStartupSteps.MainStages.Final;
                }

                // SplashScreenWindow.Discard() execution with Topmost="False" for main window somehow moves it to the very back of windows z-order
                MainWindowInstance.Topmost = true;
                MainWindowInstance.AdditionalFadeInCompleteActions.Add(delegate ()
                {
                    SplashScreenWindow.Discard();
                    MainWindowInstance.Topmost = LoadedConfiguration.Internal.IsAlwaysOnTop;
                });

                MainWindowInstance.BeginFadeShowing();
            }
            else
            {
                /*
                 * Some other technical stuff to do with te program's code
                 */

                Dictionary<string, Action> Scenarios = new()
                {
                    ["Skills Data selection"] = delegate
                    {
                        string DownloadedDatamineJsonFilesFolder = @"";
                        string DestinationFolder = @"";
                        foreach (FileInfo JsonFile in new DirectoryInfo(DownloadedDatamineJsonFilesFolder).GetFiles())
                        {
                            if (JsonFile.Name.Contains("skill") && JsonFile.Name.StartsWith("ally-skill-buff") == false)
                            {
                                JsonFile.CopyTo(@$"{DestinationFolder}\{JsonFile.Name}");
                            }
                        }
                    },

                    ["Keyword things"] = delegate
                    {
                        // if (JsonFile.Name.Contains("buff") & JsonFile.Name.ContainsOneOf("by-formation", "railway-dungeon-buff") == false)

                        string WhatToDo = "Keyword Colors extraction";

                        LimbusDataFile<KeywordData> SelectedFile = new FileInfo(@"").DeserealizeJsonAs<LimbusDataFile<KeywordData>>()!;
                        switch (WhatToDo)
                        {
                            case "Keyword Colors extraction":

                                string Colors = "";
                                foreach (KeywordData Info in SelectedFile.DataList)
                                {
                                    Colors += $"{Info.ID} ¤ {Info.Color}\n";
                                }
                                Clipboard.SetText(Colors);

                                break;


                            case "Print as list for Sprites availability check":

                                string Total = "";
                                foreach (KeywordData Info in SelectedFile.DataList)
                                {
                                    Total += $"<sprite name=\\\"{Info.ID}\\\"\\n";
                                }
                                Clipboard.SetText(Total);

                                break;


                            default: break;
                        }
                    }
                };

                // Scenarios["..."].Invoke();

                Application.Current.Shutdown();
            }
        }

        private static void SetupExternalStatics()
        {
            RijnadelClassLibrary.FileEventsNotifier.ExceptionsHandler += delegate (Exception OccurredException, string Context)
            {
                ErrorMessageWindow.ShowException(OccurredException, Context, $"{nameof(FileEventsNotifier)} :: ");
            };

            TextMeshLarp.TagsPreset.DefaultRegistry.ImportTagsFromNewInstanceOf<ImportableLimbusTags>();
            TextMeshLarp.TagsPreset.DefaultRegistry.Add(@Languages.InlineImage);
        }
    }
}