using System.Net.Http;

namespace LCLocalizationInterface
{
    public partial class App : Application
    {
        public static readonly (sbyte Major, sbyte Minor, sbyte Patch) VersionHeading = (1, 4, 0);
        public static readonly string @Version = $"{VersionHeading.Major}.{VersionHeading.Minor}ː{VersionHeading.Patch}";
        
        public static async void CheckLatestVersion()
        {
            if (LoadedConfiguration.Internal.CheckForUpdates)
            {
                string GithubApiLink = "https://api.github.com/repos/x1bViolet/Limbus-Localization-UI/releases/latest";
                try
                {
                    using (HttpClient Client = new())
                    {
                        Client.DefaultRequestHeaders.UserAgent.ParseAdd($"LC Localization Interface ({@Version.Replace("ː", ":")}) version checker");

                        HttpResponseMessage Response = await Client.GetAsync(GithubApiLink);
                        if (Response.IsSuccessStatusCode)
                        {
                            JObject LatestReleaseData = JObject.Parse(await Response.Content.ReadAsStringAsync());
                            string LatestReleaseTag = LatestReleaseData["tag_name"]?.ToString()!;
                            string LatestReleaseUrl = LatestReleaseData["html_url"]?.ToString()!;
                            if (LatestReleaseTag != App.@Version)
                            {
                                UpdateNoticeWindow.UpdateNoticeWindowInstance.NewVersionLink = LatestReleaseUrl;
                                UpdateNoticeWindow.UpdateNoticeWindowInstance.OpenNewVersionLinkButton.ToolTip = LatestReleaseUrl;

                                @Languages.ExformElement(UID: "[Update notice] * Text", ExformObjects: [LatestReleaseTag, App.@Version]);

                                UpdateNoticeWindow.UpdateNoticeWindowInstance.BeginFadeShowing();
                            }
                        }
                    }
                }
                catch (Exception Occurred)
                {
                    ErrorMessageWindow.ShowException(Occurred, $"This exception occurred while trying to check out the latest version of the program on GitHub by <hyperlink=\"{GithubApiLink}\"><color=#569cd6><u>{GithubApiLink}</u></color></hyperlink> link");
                }
            }
        }
    }
}