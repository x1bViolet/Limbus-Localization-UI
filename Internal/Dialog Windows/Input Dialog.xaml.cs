using LCLocalizationInterface.Internal.Abstractions;

namespace LCLocalizationInterface.Internal
{
    public partial class InputDialog : DialogWindowBase
    {
        #pragma warning disable CS8618
        public static InputDialog InputDialogInstance;
        #pragma warning restore CS8618

        public InputDialog() => InitializeComponent();


        #region FadeableWindow things
        public override bool UseShowDialog => true;
        public override List<Action> AdditionalFadeInCompleteActions => [delegate () { Keyboard.Focus(InputTextBox); }];
        #endregion




        private Func<string, bool> ConfirmButtonAvailabilityCondition = delegate (string InputText) { return true; };
        private Func<string, bool> ConfirmAction = delegate (string InputText) { return true; };
        public void ShowInputDialog(string Title, Func<string, bool> ConfirmButtonAvailabilityCondition, Func<string, bool> ConfirmAction)
        {
            ConfirmButton.IsEnabled = false;
            InputTextBox.Text = "";

            this.ConfirmAction = ConfirmAction;
            this.ConfirmButtonAvailabilityCondition = ConfirmButtonAvailabilityCondition;

            @Languages.ExternElement(UID: "[Input Dialog] * Title", ExternObject: Title);
            @Languages.ExternElement(UID: "[Input Dialog] [!] [#] * Title (Window caption)", ExternObject: Title);

            this.BeginFadeShowing();
        }


        private void Confirm_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            bool SuccessResult = ConfirmAction.Invoke(InputTextBox.Text);

            if (SuccessResult)
            {
                this.BeginFadeHiding();
            }
        }

        private void InputTextBox_TextChanged(object Sender, EventArgs Args)
        {
            ConfirmButton.IsEnabled = ConfirmButtonAvailabilityCondition(InputTextBox.Text);
        }

        private void InputDialog_PreviewKeyDown(object Sender, KeyEventArgs Args)
        {
            if (Args.Key is Key.Return & ConfirmButton.IsEnabled) Confirm_ButtonClick(null!, null!);
        }
    }
}