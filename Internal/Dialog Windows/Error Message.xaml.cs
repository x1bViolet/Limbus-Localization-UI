using LCLocalizationInterface.Instruments.Classes;
using LCLocalizationInterface.Internal.Abstractions;

namespace LCLocalizationInterface.Internal
{
    public partial class ErrorMessageWindow : DialogWindowBase
    {
        #pragma warning disable CS8618, CS0618
        public static ErrorMessageWindow ErrorMessageWindowInstance;
        #pragma warning restore CS8618

        public ErrorMessageWindow() { InitializeComponent(); ReadIgnoredExceptionMessages(); }


        #region FadeableWindow things
        public override bool UseShowDialog => true;
        #endregion




        #region [⇲] Assets Directory\※ Internal\Ignored Exceptions.json
        private static List<string> IgnoredExceptionMessages = [];
        private static FileEventsNotifier IgnoredExceptionMessagesFileWatcher = new(@"[⇲] Assets Directory\※ Internal\Ignored Exceptions.json")
        {
            GeneralHandler = (_, _, _) => ReadIgnoredExceptionMessages()
        };
        private static void ReadIgnoredExceptionMessages()
        {
            if (new FileInfo(@"[⇲] Assets Directory\※ Internal\Ignored Exceptions.json").TryDeserealizeJsonAs(out List<string> Deserialized, out _))
            {
                IgnoredExceptionMessages = Deserialized;
            }
        }
        private void AddExceptionToIgnoreList_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            if (LatestException is not ZenaException)
            {
                IgnoredExceptionMessages.Add(LatestExceptionMessage!);
                IgnoredExceptionMessages.SerializeToFormattedJsonFile(@"[⇲] Assets Directory\※ Internal\Ignored Exceptions.json");
            }
            this.BeginFadeHiding();
        }
        #endregion




        #region Statics

        private static Exception? LatestException;
        private static string? LatestExceptionMessage;

        private static string FormatException(Exception TargetException)
        {
            string Message = TargetException.Message;
            string? SubMessage = TargetException.InnerException?.Message;

            // Only when PDB File is enabled
            static string GetCSProjectPath([CallerFilePath] string AttributeUsingPlaceholder = "") => AttributeUsingPlaceholder.RemovePostfix(@"Internal\Dialog Windows\Error Message.xaml.cs");

            static string? SortStackTrace(string? StackTrace, bool RemoveNonAssemblyNamespaceRoutes = true)
            {
                if (StackTrace is null) return null;

                List<string> Sorted = [.. StackTrace.Split('\n')];
                if (RemoveNonAssemblyNamespaceRoutes)
                {
                    Sorted = [.. StackTrace.Split('\n')
                        .Where(x => x.ContainsOneOf(nameof(LCLocalizationInterface), "--- End of stack trace from previous location ---"))
                        .Select(x => x.StartsWith("---") ? x = $"   {x}" : x)];
                }

                string Output = string.Join("\n", Sorted);
                Output = Regex.Replace(Output, @$"   at (?<ExceptionPoint>.*?) in {Regex.Escape(GetCSProjectPath())}(?<FilePath>.*?):line (?<LineNumber>\d+)", Match =>
                {
                    return $"at *<u>/{Match.Groups["FilePath"].Value.Replace("\\", "/")}</u> <b>(Line №{Match.Groups["LineNumber"].Value})</b>:\n   {Match.Groups["ExceptionPoint"].Value.Del($"{nameof(LCLocalizationInterface)}.")}";
                });
                return Output;
            }
            string? StackTtace = SortStackTrace(TargetException.StackTrace, TargetException is not ZenaException);
            string? SubStackTtace = SortStackTrace(TargetException.InnerException?.StackTrace, TargetException is not ZenaException);
            string? SubSubStackTtace = SortStackTrace(TargetException.InnerException?.InnerException?.StackTrace, TargetException is not ZenaException);

            string FinalMessage =
                $"[{TargetException.GetType().Name}] <images-size=25><image id=\"ZenaConfused\">\n{Message}{(string.IsNullOrWhiteSpace(StackTtace) ? "" : $"\n\n{StackTtace}")}" +
                (SubStackTtace is not null ? $"\n\n\n[InnerException]\n{SubMessage} [{TargetException.InnerException!.GetType().Name}]\n\n{SubStackTtace}" : "") +
                (SubSubStackTtace is not null ? $"\n\n\n[InnerException]\n{SubMessage} [{TargetException.InnerException!.GetType().Name}]\n\n{SubStackTtace}" : "");

            return FinalMessage;
        }

        public static void ShowException(Exception Exception, string ExceptionContext = "", string? HandlingSource = null, bool UseFadeAnimation = true)
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                ErrorMessageWindow.LatestException = Exception;

                static void HandleUnusual(Exception Occurred, string Message)
                {
                    rin(Occurred);
                    MessageBox.Show(Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                try
                {
                    if (ExceptionContext != "") ExceptionContext = $"Context: {ExceptionContext}\n\n";

                    string FullExceptionMessage = ExceptionContext + HandlingSource + FormatException(Exception);
                    ErrorMessageWindow.LatestExceptionMessage = FullExceptionMessage;

                    if (!IgnoredExceptionMessages.Contains(FullExceptionMessage))
                    {
                        try
                        {
                            @Languages.PresentedTextElements["[Error Message] [-] * Text"].RichText = FullExceptionMessage;
                            
                            List<string> ZenaPics = ["fu", "기둥 야옹", "눙물", "닌자", "으앙", "찌릿", "캬악", "훡유", "식빵"];
                            ErrorMessageWindowInstance.ZenaImage.Source = BitmapFromResource($"UI/Limbus/Zena Cat/{ZenaPics[Random.Shared.Next(0, ZenaPics.Count)]}.png");
                            ErrorMessageWindowInstance.AddExceptionToIgnoreList_Button.IsEnabled = LatestException is not ZenaException;
                            
                            if (UseFadeAnimation)
                            {
                                ErrorMessageWindowInstance.BeginFadeShowing();
                            }
                            else
                            {
                                ErrorMessageWindowInstance.Show();
                            }
                        }
                        catch (Exception Occurred) { HandleUnusual(Occurred, FullExceptionMessage); }
                    }
                }
                catch (Exception Occurred) { HandleUnusual(Occurred, Exception.ToString()); }
            });
        }

        #endregion
    }
}