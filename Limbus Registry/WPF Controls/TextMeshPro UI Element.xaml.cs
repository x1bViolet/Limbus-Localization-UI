using System.Windows.Threading;
using static LCLocalizationInterface.LimbusRegistry.InputRichTextFormatter;

namespace LCLocalizationInterface.LimbusRegistry
{
    public class TMProEmitter : TextBlock
    {
        public static List<TMProEmitter> AllInstances { get; } = [];

        public TMProEmitter() => AllInstances.Add(this);
        ~TMProEmitter() => AllInstances.Remove(this);

        /// <summary>
        /// Prevent endless keyword tooltips creation for keywords inside keywords tooltips inside tooltips for keywords inside tooltips
        /// </summary>
        public bool DisableKeyworLinksCreation { get; set; } = false;


        #region Font
        public required LimbusFontTypes FontType { get => (LimbusFontTypes)GetValue(FontTypeProperty); set => SetValue(FontTypeProperty, value); }
        public static readonly DependencyProperty FontTypeProperty = RegisterProperty<TMProEmitter, LimbusFontTypes>(DefaultValue: LimbusFontTypes.None);
        #endregion


        #region Text processing mode
        public required RichTextFormat TextProcessingMode { get => (RichTextFormat)GetValue(TextProcessingModeProperty); set => SetValue(TextProcessingModeProperty, value); }
        public static readonly DependencyProperty TextProcessingModeProperty = RegisterProperty<TMProEmitter, RichTextFormat>(DefaultValue: RichTextFormat.None);
        #endregion


        #region Rich text
        /// <summary>Its <b>not</b> RichText<i>Changed</i> !!!!</summary>
        public event RichTextSettedEventHandler? RichTextSetted;
        public delegate void RichTextSettedEventHandler(TMProEmitter Sender, string? SettedRichText);


        public void RefreshRichText() => this.RichText = this.CurrentRichText;



        #region Delayed RichText
        /// <summary>
        /// This property is <see langword="false"/> when rich text set occurres via editor object switch / file load / etc,,, anything except actual text typing
        /// </summary>
        public static bool IsRichTextDelayAllowed
        {
            get; set
            {
                // Reset running delayed rich text set timers when switching to another editor object (-> to not occasionaly set previous object text after delay time)
                if ((field = value) == false)
                {
                    TMProEmitter.AllInstances.Where(x => x.AcceptsRichTextDelay && x.DelayedRichTextContext.Pending).ToList().ForEach(ActiveTMProEmitter =>
                    {
                        ActiveTMProEmitter.DelayedRichTextContext.Timer.Stop();
                        ActiveTMProEmitter.DelayedRichTextContext.Pending = false;
                    });
                }
            }
        } = true;


        public static readonly DependencyProperty AcceptsRichTextDelayProperty = RegisterProperty<TMProEmitter, bool>(DefaultValue: true);
        public bool AcceptsRichTextDelay { get => (bool)GetValue(AcceptsRichTextDelayProperty); set => SetValue(AcceptsRichTextDelayProperty, value); }

        /// <summary>
        /// Returns new instance of <see cref="DelayDisabler"/>.
        /// <br/><br/>
        /// <inheritdoc cref="DelayDisabler"/>
        /// </summary>
        public static DelayDisabler DisabledRichTextDelay => new();
        
        /// <summary>
        /// Sets <see langword="static"/> <see cref="TMProEmitter.IsRichTextDelayAllowed"/> to <see langword="false"/> during <see langword="using"/>, then back to <see langword="true"/> on <see cref="DelayDisabler.Dispose"/>.
        /// </summary>
        public class DelayDisabler : IDisposable
        {
            public DelayDisabler() { IsRichTextDelayAllowed = false; }
            public void Dispose() { IsRichTextDelayAllowed = true; GC.SuppressFinalize(this); }
        }

        private DelayedRichTextContextData DelayedRichTextContext = new();
        private class DelayedRichTextContextData
        {
            private double RichTextDelay
            {
                get
                {
                    try   { return LoadedConfiguration.PreviewSettings.Base.PreviewUpdateDelay; }
                    catch { return 0; } // XAML Designer ........................................
                }
            }
            public DispatcherTimer Timer = new();  public bool Pending = false;  public string RequestedText = "";
            public void StartDelayedSet(Action<string> RichTextSet)
            {
                Timer.Stop(); Timer = new() { Interval = TimeSpan.FromSeconds(RichTextDelay) };
                Timer.Tick += (_, _) => { Timer.Stop(); RichTextSet.Invoke(RequestedText); Pending = false; };
                
                Pending = true; Timer.Start();
            }
        }
        #endregion


        // Not SetValue(DependencyProperty, value) there because rich text may be updated with the same value but then SetValue will ignore this
        public static readonly DependencyProperty RichTextProperty = RegisterProperty<TMProEmitter, string>(DefaultValue: "", PropertyChangedEvent: OnRichTextDependencyPropertyChanged);
        private static void OnRichTextDependencyPropertyChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
        {
            (Sender as TMProEmitter)!.RichText = (string)Args.NewValue; // From DataContextDomain {Bindings}
        }

        private string CurrentRichText { get; set; } = "";
        /// <summary>
        /// Not linked with <see cref="RichTextProperty"/> SetValue/GetValue because need to be set without equality check on <see cref="RefreshRichText"/>, the only issue is Visibility property style trigger based on rich text presence which is not active if value is set manually from the code.<br/><br/>
        /// So consider to use <see cref="DependencyObject.SetValue"/> with <see cref="TMProEmitter.RichTextProperty"/> to update Triggers and Bindings
        /// </summary>
        public string RichText
        {
            get => CurrentRichText;
            set
            {
                if (ProgramFullyLoaded &&
                    LoadedConfiguration.PreviewSettings.Base.PreviewUpdateDelay > 0 &&
                    this.AcceptsRichTextDelay && TMProEmitter.IsRichTextDelayAllowed
                ) {
                    DelayedRichTextContext.RequestedText = value; // It must be an external variable updated <independently!!!!!> of the timer!!!!!!!!! while its maybe RUNNING
                    if (DelayedRichTextContext.Pending is false) DelayedRichTextContext.StartDelayedSet(ActuallySetRichText);
                }
                else
                {
                    ActuallySetRichText(value);
                }

                void ActuallySetRichText(string RichText)
                {
                    RichText ??= "";

                    string FormattedLimbusRichText = RichText;

                    // XAML Designer ........................................................................
                    try
                    {
                        if (SelectedLimbusCustomLanguage is not null)
                        {
                            FormattedLimbusRichText = InputRichTextFormatter.Apply(LimbusText: RichText, SpecifiedRichTextFormat: this.TextProcessingMode);
                        }
                    }
                    catch { }

                        
                    Kaestarlyn.Actions.Apply(
                        Target: this,
                        RichText: FormattedLimbusRichText,
                        DividersMode: Kaestarlyn.@PostInfo.FullStopDividers.FullStopDividers_TMPro,
                        IgnoreTags: Kaestarlyn.@PostInfo.IgnoreTags_UnityTMProExclude,
                        DisableKeyworLinksCreation: this.DisableKeyworLinksCreation
                    );
                    
                    string? PreviousRichText = CurrentRichText;
                    CurrentRichText = RichText;

                    RichTextSetted?.Invoke(this, RichText); // Call after the CurrentRichText was set!!!!!!!!!!! !!  !!!!!! !
                }
            }
        }
        #endregion
    }
}