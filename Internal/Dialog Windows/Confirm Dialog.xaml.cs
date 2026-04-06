using LCLocalizationInterface.Internal.Abstractions;

namespace LCLocalizationInterface.Internal
{
    public partial class ConfirmDialog : DialogWindowBase
    {
        #pragma warning disable CS8618
        public static ConfirmDialog ConfirmDialogInstance;
        #pragma warning restore CS8618

        public ConfirmDialog() => InitializeComponent();




        private Action ConfirmAction { get; set; } = delegate () { };
        public void ShowConfirmDialog(string Title, string Message, Action ConfirmAction)
        {
            this.ConfirmAction = ConfirmAction;
            @Languages.PresentedTextElements["[Confirm Dialog] [-] * Text"].RichText = Message;
            @Languages.ExternElement(UID: "[Confirm Dialog] * Title", ExternObject: Title);
            @Languages.ExternElement(UID: "[Confirm Dialog] [!] [#] * Title (Window caption)", ExternObject: Title);

            base.BeginFadeShowing();
        }

        private void Confirm_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            ConfirmAction.Invoke();

            base.BeginFadeHiding();
        }
    }
}