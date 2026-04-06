using LCLocalizationInterface.Entanglement;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using LCLocalizationInterface.LimbusRegistry.JsonTypes.Specific;
using LCLocalizationInterface.LimbusRegistry.PreviewCreator;
using PropertyChanged;
using static LCLocalizationInterface.Entanglement.EntanglementModel;
using static LCLocalizationInterface.Internal.@Configurazione.JsonConfigurationFile;
using static LCLocalizationInterface.LimbusRegistry.JsonTypes.ObservationLog;
using static LCLocalizationInterface.LimbusRegistry.JsonTypes.Skill;

namespace LCLocalizationInterface
{
    /// <summary>
    /// <see cref="INotifyPropertyChanged"/> through PropertyChanged.Fody (<see href="https://www.nuget.org/packages/Fody"/>, this <see langword="abstract"/> <see langword="record"/> just derived from <see cref="INotifyPropertyChanged"/> <see langword="interface"/> using <see cref="PropertyChanged.AddINotifyPropertyChangedInterfaceAttribute"/>)
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public abstract record Explicit;
}

namespace LCLocalizationInterface
{
    public partial class App : Application
    {
        public static ResourceDictionary @EntanglementDictionary => Application.Current.Resources.MergedDictionaries.ByUriSource("/[Entanglement]/Entanglement Dictionary.xaml")!;
        public static EntanglementModel @EntanglementModel => (@EntanglementDictionary["ThatOnePeskyMvvmThing"] as EntanglementModel) ?? null!;
        public static DataContextDomainProperties @DataContextDomain => @EntanglementModel?.DataContextDomain!;
        public static LimbusCustomLangDefinition.LangProperties SelectedLimbusCustomLanguage => @DataContextDomain?.SelectedLimbusCustomLanguage!;
        public static @Configurazione.JsonConfigurationFile LoadedConfiguration => @DataContextDomain?.Configuration!;
    }
}

namespace LCLocalizationInterface.Entanglement
{
    /// <summary>
    /// View model. Instance of this class is created in <c>`Entanglement Dictionary.xaml`</c> resource dictionary (Main resource dictionary connected in <c>App.xaml</c> file)
    /// </summary>
    public record EntanglementModel : Explicit
    {
        /// <summary>
        /// Non-<see langword="static"/> object inside <see cref="EntanglementModel"/> <see langword="class"/> used in XAML {Bindings}<br/>
        /// <br/><br/>
        /// <see langword="static"/> version is <see cref="App.DataContextDomain"/> link to this object from <see cref="Application.Resources"/>
        /// </summary>
        public DataContextDomainProperties DataContextDomain { get; } = new();
        public record DataContextDomainProperties : Explicit
        {
            /// <summary>Formatted <see cref="App.@Version"/> with rich text tags for OneTime Binding in main menu</summary>
            public string ProgramVersion => App.@Version.Replace("ː", "<b><size=250%>ː</size><size=19%> </size></b>");


            public LimbusCustomLangDefinition.LangProperties? SelectedLimbusCustomLanguage { get; set; }
            public @Configurazione.JsonConfigurationFile? Configuration { get; set; }

            
            
            /// <summary>Limbus json editor variables</summary>
            public Editor_PROP Editor { get; } = new();
            public record Editor_PROP : Explicit
            {
                public string? CurrentObjectID { get; set; }
                private void DisplayedIDSetter<TIdentifier>(IHasIdentifier<TIdentifier>? IDObject)
                {
                    CurrentObjectID = IDObject is not null ? $"{IDObject.ID}" : @Languages.VariableData.InsertionsDefaultValue;
                }

                /// [PropertyChanged.DoNotCheckEquality] is needed because when switching between active editor modes using the button in the title bar, <see cref="DisplayedIDSetter"/> should be called anyway by switch to the same current object to update the displayed ID
                /// <see cref="MainWindow.ModeSwitchContextMenu_Click"/>

                [DoNotCheckEquality]
                public Skill? CurrentSkill { get; set => DisplayedIDSetter(field = value); }
                
                public Uptie? CurrentUptie { get; set { using (TMProEmitter.DisabledRichTextDelay) field = value; } }


                [DoNotCheckEquality]
                public Passive? CurrentPassive { get; set { using (TMProEmitter.DisabledRichTextDelay) DisplayedIDSetter(field = value); } }


                [DoNotCheckEquality]
                public Keyword? CurrentKeyword { get; set { using (TMProEmitter.DisabledRichTextDelay) DisplayedIDSetter(field = value); } }


                [DoNotCheckEquality]
                public EGOGift? CurrentEGOGift { get; set { using (TMProEmitter.DisabledRichTextDelay) DisplayedIDSetter(field = value); } }


                [DoNotCheckEquality]
                public ObservationLog? CurrentObservationLog { get; set => DisplayedIDSetter(field = value); }
                
                public ObservationStory? CurrentObservationStory { get; set { using (TMProEmitter.DisabledRichTextDelay) field = value; } }




                public ICSharpCode.AvalonEdit.Document.TextDocument? MainMenuDocument { get; set; }
            }



            #region TBD
            public SkillsDisplayInfoOrchestrator_PROP SkillsDisplayInfoOrchestrator { get; } = new();
            public record SkillsDisplayInfoOrchestrator_PROP : Explicit
            {
                public SkillConstructor CurrentConstructor { get; set; } = new();
            }
            #endregion



            public PreviewCreator_PROP PreviewCreator { get; } = new();
            public record PreviewCreator_PROP : Explicit
            {
                /// <summary>Currently loaded Identity/E.G.O Preview Creator's image info json file</summary>
                public PreviewCreatorPage.ImageInfoJsonFile ImageInfo { get; set; } = new();
            }
        }
    }
}