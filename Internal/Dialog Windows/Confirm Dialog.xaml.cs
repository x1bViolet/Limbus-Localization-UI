using LCLocalizationInterface.Internal.Abstractions;

namespace LCLocalizationInterface.Internal
{
    public partial class ConfirmDialog : DialogWindowBase
    {
        #pragma warning disable CS8618
        public static ConfirmDialog ConfirmDialogInstance;
        #pragma warning restore CS8618

        public ConfirmDialog() => InitializeComponent();




        private Action? ConfirmAction { get; set; } = delegate () { };
        private Action? CancelAction { get; set; } = delegate () { };
        public void ShowConfirmDialog(string Title, string Message, Action? ConfirmAction = null, Action? CancelAction = null, bool UseShowDialog = false)
        {
            this.ConfirmAction = ConfirmAction;
            this.CancelAction = CancelAction;
            @Languages.PresentedTextElements["[Confirm Dialog] [-] * Text"].RichText = Message;
            @Languages.ExternElement(UID: "[Confirm Dialog] * Title", ExternObject: Title);
            @Languages.ExternElement(UID: "[Confirm Dialog] [!] [#] * Title (Window caption)", ExternObject: Title);

            if (UseShowDialog) this.UseShowDialog = true;
            base.BeginFadeShowing();
            if (UseShowDialog) this.UseShowDialog = false;
        }

        private void Confirm_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            ConfirmAction?.Invoke();

            base.BeginFadeHiding();
        }
        private void Cancel_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            CancelAction?.Invoke();

            base.BeginFadeHiding();
        }
    }
}