namespace LCLocalizationInterface.Internal
{
    namespace Abstractions
    {
        public class DialogWindowContentPresenter : ContentControl
        {
            public Thickness ViewPadding { get => (Thickness)GetValue(ViewPaddingProperty); set => SetValue(ViewPaddingProperty, value); }
            public static readonly DependencyProperty ViewPaddingProperty = RegisterProperty<DialogWindowContentPresenter, Thickness>(DefaultValue: new Thickness(15, 8, 15, 15));
        }


        public abstract partial class DialogWindowBase : FadeableWindow
        {
            public DialogWindowBase()
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                this.Resources[typeof(Terminus.IntenseStareType1)] = new Style()
                {
                    BasedOn = @EntanglementDictionary.MergedDictionaries.ByUriSource("/Internal/Language.xaml")![typeof(Terminus.IntenseStareType1)] as Style,
                    TargetType = typeof(Terminus.IntenseStareType1),
                    Setters =
                    {
                        new Setter()
                        {
                            Property = Terminus.IntenseStareType1.ForegroundProperty,
                            Value = new DynamicResourceExtension("Theme:UIText.DialogWindows.Foreground")
                        }
                    }
                };
            }


            #region FadeableWindow things
            public override bool CenterOnScreenWhenShowing => true;


            protected override (double In, double Out) FadeDurations => AsPair(ThemeTimings.Duration.DialogWindows);
            protected override (double In, double Out) FadeSpeedRatios => AsPair(ThemeTimings.SpeedRatio.DialogWindows);

            protected override ((double Acceleration, double Deceleation) In, (double Acceleration, double Deceleation) Out) FadeKinematics
                => AsPairPair(ThemeTimings.AccelerationDecelerationRatios.DialogWindows);
            #endregion
        }
    }
}