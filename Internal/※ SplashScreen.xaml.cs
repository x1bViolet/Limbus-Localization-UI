namespace LCLocalizationInterface.Internal
{
    public partial class SplashScreenWindow : Window
    {
        #pragma warning disable CS8618
        public static SplashScreenWindow SplashScreenWindowInstance;
        #pragma warning restore CS8618

        public SplashScreenWindow() => InitializeComponent();

        public static string ProgressObject
        {
            set => SplashScreenWindowInstance?.Dispatcher.Invoke(delegate () { SplashScreenWindowInstance.ProgressObjectTextBlock.RichText = value; });
        }
        public static string ProgressSubObject
        {
            set => SplashScreenWindowInstance?.Dispatcher.Invoke(delegate () { SplashScreenWindowInstance.ProgressSubObjectTextBlock.RichText = value; });
        }
        public static void HideText()
        {
            SplashScreenWindowInstance?.Dispatcher.Invoke(delegate () { SplashScreenWindowInstance.Texts.Visibility = Visibility.Collapsed; });
        }
        public static void Discard()
        {
            SplashScreenWindowInstance?.Dispatcher.Invoke(SplashScreenWindowInstance.Close);
            SplashScreenWindowInstance = null!;
        }
        public static void DiscardIfNotStarted()
        {
            if (ProgramFullyLoaded == false) Discard();
        }
    }
}