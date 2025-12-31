using LC_Localization_Task_Absolute.Json;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using static LC_Localization_Task_Absolute.Requirements;


namespace LC_Localization_Task_Absolute;

public partial class App : Application
{
    private static class QuickConfigAndLanguage
    {
        public record ConfigDelta
        {
            [JsonProperty("Internal")]
            public Internal Internal { get; set; } = new Internal();
        }
        public record Internal
        {
            [JsonProperty("UI Language")]
            public string UILanguage { get; set; } = "";
        }
    }

    protected override void OnStartup(StartupEventArgs EventArgs)
    {
        base.OnStartup(EventArgs);
        SetupExceptionHandling();

        try   { Console.OutputEncoding = Encoding.UTF8; }
        catch { }

        string LogoPath = "UI/Logo.png";
        try
        {
            if (new FileInfo(@"[⇲] Assets Directory\Configurazione^.json").Deserealize<QuickConfigAndLanguage.ConfigDelta>().Internal.UILanguage.EndsWith("繁體中文"))
                LogoPath = "UI/Logo (CN).png";
        }
        catch { }

        new SplashScreen(LogoPath).Show(autoClose: true, topMost: false);

        // Init MainWindow
        new MainWindow().Show();
    }

    private void SetupExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (RequestSender, EventArgs) =>
        {
            LogUnhandledException((Exception)EventArgs.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
        };

        DispatcherUnhandledException += (RequestSender, EventArgs) =>
        {
            LogUnhandledException(EventArgs.Exception, "Application.Current.DispatcherUnhandledException");
            EventArgs.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (RequestSender, EventArgs) =>
        {
            LogUnhandledException(EventArgs.Exception, "TaskScheduler.UnobservedTaskException");
            EventArgs.SetObserved();
        };
    }
    private void LogUnhandledException(Exception Exception, string HandlingSource)
    {
        rin(FormattedStackTrace(Exception, HandlingSource));
        if (!LC_Localization_Task_Absolute.MainWindow.MainControl.IsLoaded) Application.Current.Shutdown();
    }
}

