using System.Windows.Threading;

namespace LCLocalizationInterface
{
    public partial class App : Application
    {
        /// <summary>
        /// <see langword="try"/>-<see langword="catch"/> with <see cref="System.Windows.Threading.Dispatcher.Invoke"/> to <see cref="Application.Current"/> and <see cref="ErrorMessageWindow.ShowException"/> on <see langword="catch"/><br/>
        /// (Default exceptions handling through <see cref="SetupExceptionsHandling"/> will not handle another thread exceptions)
        /// </summary>
        public static void HandledInvoke(Action DispatcherAction)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherAction);
            }
            catch (Exception Exception)
            {
                Application.Current.Dispatcher.Invoke(delegate ()
                {
                    ErrorMessageWindow.ShowException(Exception, "Calling a method from another thread [App.HandledInvoke(Action DispatcherAction)] (Most likely from FileSystemWatcher event)");
                });
            }
        }


        private void SetupExceptionsHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += delegate (object Sender, UnhandledExceptionEventArgs Args)
            {
                SplashScreenWindow.DiscardIfNotStarted();
                LogUnhandledException((Exception)Args.ExceptionObject, $"{nameof(AppDomain.CurrentDomain.UnhandledException)} :: ");
            };

            DispatcherUnhandledException += delegate (object Sender, DispatcherUnhandledExceptionEventArgs Args)
            {
                SplashScreenWindow.DiscardIfNotStarted();
                LogUnhandledException(Args.Exception, $"{nameof(Application.DispatcherUnhandledException)} :: ");
                Args.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += delegate (object? Sender, UnobservedTaskExceptionEventArgs Args)
            {
                SplashScreenWindow.DiscardIfNotStarted();
                LogUnhandledException(Args.Exception, $"{nameof(TaskScheduler.UnobservedTaskException)} :: ");
                Args.SetObserved();
            };
        }


        private void LogUnhandledException(Exception Exception, string HandlingSource)
        {
            ErrorMessageWindow.ShowException(Exception, HandlingSource: HandlingSource);
            if (ProgramFullyLoaded == false) Application.Current.Shutdown();
        }


        public const string ExceptionsWindowTestObsolete = "This is an Exceptions Info window test";
        [Obsolete(ExceptionsWindowTestObsolete)] public class ZenaException(string Message = "You scared Zena.") : Exception(Message);
        [Obsolete(ExceptionsWindowTestObsolete)] public static async void GeneralActionToken(object Sender, EventArgs Args) => await Task.Run(delegate () { throw new ZenaException(); });
    }
}

namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public partial class PreviewCreatorPage : Page
    {
        [Obsolete(ExceptionsWindowTestObsolete)] private void GeneralActionToken(object Sender, RoutedEventArgs Args) => App.GeneralActionToken(Sender, Args);
    }
}