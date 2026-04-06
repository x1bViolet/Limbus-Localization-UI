namespace LCLocalizationInterface.Internal
{
    namespace Abstractions
    {
        /// <summary>
        /// <see cref="BeginFadeShowing"/> / <see cref="BeginFadeHiding"/> <br/>
        /// </summary>
        public abstract class FadeableWindow : Window
        {
            public FadeableWindow()
            {
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!
                this.AllowsTransparency = true;
                this.WindowStyle = WindowStyle.None;
            }


            /// <summary>
            /// <c>[<see cref="FadeableWindow"/>]</c> If <see cref="PreventClosingByDefaultLogic"/> is <see langword="true"/>, <see cref="OnClosing"/> will be canceled and <see cref="BeginFadeHiding"/> will be called instead
            /// </summary>
            protected override void OnClosing(CancelEventArgs Args)
            {
                base.OnClosing(Args);
                if (this.PreventClosingByDefaultLogic)
                {
                    Args.Cancel = true;
                    this.BeginFadeHiding();
                }
            }



            /// <summary>
            /// <c>[<see cref="FadeableWindow"/>]</c> Preset button click event handler method that sets <see cref="Window.WindowState"/> to <see cref="WindowState.Minimized"/>
            /// </summary>
            protected virtual void Minimize(object Sender, RoutedEventArgs Args) => this.WindowState = WindowState.Minimized;

            /// <summary>
            /// <c>[<see cref="FadeableWindow"/>]</c> Preset button click event handler method that calls <see cref="BeginFadeHiding"/>
            /// </summary>
            protected virtual void FadeClose(object Sender, RoutedEventArgs Args) => this.BeginFadeHiding();


            #region Fade animations
            private abstract class FadeAnimation : DoubleAnimation
            {
                public FadeAnimation(bool IsReversed) => (this.From, this.To) = IsReversed ? (1, 0) : (0, 1);
            }
            private sealed class DoubleAnimation_FadeIn : FadeAnimation
            {
                public DoubleAnimation_FadeIn(FadeableWindow OwnerWindow) : base(IsReversed: false)
                {
                    this.Duration = TimeSpan.FromSeconds(Math.Max(0, OwnerWindow.FadeDurations.In));
                    this.SpeedRatio = Math.Max(0.01, OwnerWindow.FadeSpeedRatios.In);
                   (this.AccelerationRatio, this.DecelerationRatio) = (OwnerWindow.FadeKinematics.In.Acceleration, OwnerWindow.FadeKinematics.In.Deceleation);

                    this.Completed += (_, _) =>
                    {
                        OwnerWindow.AdditionalFadeInCompleteActions.ForEach(x => x?.Invoke());
                        OwnerWindow.IsFadeInRunning = false;
                    };
                }
            }
            private sealed class DoubleAnimation_FadeOut : FadeAnimation
            {
                public DoubleAnimation_FadeOut(FadeableWindow OwnerWindow) : base(IsReversed: true)
                {
                    this.Duration = TimeSpan.FromSeconds(Math.Max(0, OwnerWindow.FadeDurations.Out));
                    this.SpeedRatio = Math.Max(0.01, OwnerWindow.FadeSpeedRatios.Out);
                   (this.AccelerationRatio, this.DecelerationRatio) = (OwnerWindow.FadeKinematics.Out.Acceleration, OwnerWindow.FadeKinematics.Out.Deceleation);

                    this.Completed += (_, _) =>
                    {
                        OwnerWindow.Hide();
                        OwnerWindow.AdditionalFadeOutCompleteActions.ForEach(x => x?.Invoke());
                        OwnerWindow.IsFadeOutRunning = false;
                    };
                }
            }


            #region Animation timings
            /// <summary>Current timings from <see cref="@Themes.CurrentTheme"/></summary>
            public static @Themes.ThemeDefinition.Common_PROP.WindowsFadeAnimation_PROP ThemeTimings => @Themes.CurrentTheme.Common.WindowsFadeAnimationTimings;
            protected virtual (double In, double Out) FadeDurations => (In: 0.11, Out: 0.21);
            protected virtual (double In, double Out) FadeSpeedRatios => (In: 1.0, Out: 1.0);

            protected virtual ((double Acceleration, double Deceleation) In, (double Acceleration, double Deceleation) Out) FadeKinematics
                => (In: (Acceleration: 1.0, Deceleation: 1.0), Out: (Acceleration: 1.0, Deceleation: 1.0));


            /// Easy transforming for double[] arrays from <see cref="ThemeTimings"/>
            public static (double, double) AsPair(double[] Source) => (Source[0], Source[1]);
            public static ((double, double), (double, double)) AsPairPair(double[][] Source) => ((Source[0][0], Source[0][1]), (Source[1][0], Source[1][1]));
            #endregion
            
            #endregion



            /// <summary>Invoked on <see cref="Timeline.Completed"/> event of the window fade in animaton (<see cref="BeginFadeShowing"/>)</summary>
            public virtual List<Action> AdditionalFadeInCompleteActions { get; set; } = [];

            /// <summary>Invoked on <see cref="Timeline.Completed"/> event of the window fade out animaton (<see cref="BeginFadeHiding"/>)</summary>
            public virtual List<Action> AdditionalFadeOutCompleteActions { get; set; } = [];



            /// <summary>Returns new instance of <see cref="DoubleAnimation_FadeIn"/></summary>
            private DoubleAnimation_FadeIn WindowFadeInAnimReuseable => new(OwnerWindow: this);

            /// <summary>Returns new instance of <see cref="DoubleAnimation_FadeOut"/></summary>
            private DoubleAnimation_FadeOut WindowFadeOutAnimReuseable => new(OwnerWindow: this);



            /// <summary>
            /// <c>[<see cref="FadeableWindow"/>]</c> Specifies that <see cref="OnClosing"/> will be canceled and <see cref="BeginFadeHiding"/> will be called instead
            /// </summary>
            public virtual bool PreventClosingByDefaultLogic { get; set; } = true;

            /// <summary>
            /// <c>[<see cref="FadeableWindow"/>]</c> Use <see cref="Window.ShowDialog"/> instead of regular <see cref="Window.Show"/> in the <see cref="BeginFadeShowing"/> method
            /// </summary>
            public virtual bool UseShowDialog { get; set; } = false;

            /// <summary><c>[<see cref="FadeableWindow"/>]</c> Specifies that window will be centered on the screen in the <see cref="BeginFadeShowing"/> method (Via <see cref="Instruments.WPFTools.CenterOnScreen"/>)</summary>
            public virtual bool CenterOnScreenWhenShowing { get; set; } = false;


            private bool IsFadeInRunning = false;
            private bool IsFadeOutRunning = false;

            public virtual void BeginFadeShowing()
            {
                if (IsFadeInRunning) return;

                if (!this.IsVisible)
                {
                    this.Opacity = 0;
                    
                    if (!UseShowDialog) this.Show();

                    try
                    {
                        this.IsFadeInRunning = true;
                        this.BeginAnimation(Window.OpacityProperty, WindowFadeInAnimReuseable);
                        if (CenterOnScreenWhenShowing) this.CenterOnScreen();
                    }
                    catch (Exception Occurred)
                    {
                        ErrorMessageWindow.ShowException
                        (
                            Exception: Occurred,
                            ExceptionContext: $"This exception occured while trying to execute <u>fade in</u> animation for the following Window: {this.GetType().Name}",
                            UseFadeAnimation: this is not ErrorMessageWindow
                        );

                        this.StopAnimation(Window.OpacityProperty);
                        this.Opacity = 1;
                        this.IsFadeInRunning = false;

                        AdditionalFadeInCompleteActions.ForEach(x => x?.Invoke());
                    }
                    
                    if (UseShowDialog) this.ShowDialog();
                }
            }

            public virtual void BeginFadeHiding()
            {
                if (IsFadeOutRunning) return;

                try
                {
                    this.IsFadeOutRunning = true;
                    this.BeginAnimation(Window.OpacityProperty, WindowFadeOutAnimReuseable);
                }
                catch (Exception Occurred)
                {
                    ErrorMessageWindow.ShowException
                    (
                        Exception: Occurred,
                        ExceptionContext: $"This exception occured while trying to execute <u>fade out</u> animation for the following Window: {this.GetType().Name}",
                        UseFadeAnimation: this is not ErrorMessageWindow
                    );

                    this.StopAnimation(Window.OpacityProperty);
                    this.Opacity = 0;
                    this.IsFadeOutRunning = false;

                    this.Hide();
                    AdditionalFadeOutCompleteActions.ForEach(x => x?.Invoke());
                }
            }
        }
    }
}