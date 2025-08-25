using Newtonsoft.Json;
using NLog;
using System.Runtime.Serialization;
using System.Windows;
using System.IO;
using static LC_Localization_Task_Absolute.Requirements;
using System.Text;


namespace LC_Localization_Task_Absolute;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>

internal abstract class QuickConfigAndLanguage
{
    internal protected class ConfigDelta
    {
        [JsonProperty("Internal")]
        public Internal Internal { get; set; } = new Internal();
    }
    internal protected class Internal
    {
        [JsonProperty("UI Language")]
        public string UILanguage { get; set; } = "";
    }
}

public partial class App : Application
{
    



    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        SetupExceptionHandling();

        try { Console.OutputEncoding = Encoding.UTF8; }
        catch { }

        string LogoPath = "UI/Logo.png";

        try
        {
            var quickconfig = JsonConvert.DeserializeObject<QuickConfigAndLanguage.ConfigDelta>(File.ReadAllText( @"⇲ Assets Directory\Configurazione^.json"));

            if (quickconfig.Internal.UILanguage.EndsWith("繁體中文.json"))
            {
                LogoPath = "UI/Logo (CN).png";
            }
        }
        catch (Exception ex) { rin(ex.ToString()); }

        SplashScreen StartupSplash = new SplashScreen(LogoPath);
        StartupSplash.Show(autoClose: true, topMost: false);

        Dispatcher.BeginInvoke(new Action(() =>
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Loaded += (s, args) => StartupSplash.Close(fadeoutDuration: new TimeSpan(minutes: 0, seconds: 0, hours: 0));
            mainWindow.Show();
        }), System.Windows.Threading.DispatcherPriority.Background);
    }

    private static Logger _logger = LogManager.GetCurrentClassLogger();

    private void SetupExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

        DispatcherUnhandledException += (s, e) =>
        {
            LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            e.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
            e.SetObserved();
        };
    }
    private void LogUnhandledException(Exception exception, string source)
    {
        string message = $"Unhandled exception ({source})";
        try
        {
            try
            {
                rin(exception.ToString());
            }
            catch { }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception in LogUnhandledException");
        }
        finally
        {
            _logger.Error(exception, message);
        }
    }
}

