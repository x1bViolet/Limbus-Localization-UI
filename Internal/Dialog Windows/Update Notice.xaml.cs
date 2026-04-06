using LCLocalizationInterface.Internal.Abstractions;
using System.Diagnostics;
using static LCLocalizationInterface.Internal.Configuration.SettingsWindow;

namespace LCLocalizationInterface.Internal
{
    public partial class UpdateNoticeWindow : DialogWindowBase
    {
        #pragma warning disable CS8618
        public static UpdateNoticeWindow UpdateNoticeWindowInstance;
        #pragma warning restore CS8618


        public UpdateNoticeWindow() => InitializeComponent();




        public string NewVersionLink { get; set; } = "";
        private void OpenNewVersionLinkButton_Click(object Sender, RoutedEventArgs Args)
        {
            _ = MainWindowInstance.WindowState
              = SettingsWindowInstance.WindowState
              = WindowState.Minimized;

            Process.Start(new ProcessStartInfo() { FileName = NewVersionLink, UseShellExecute = true });

            this.BeginFadeHiding();
        }
    }
}