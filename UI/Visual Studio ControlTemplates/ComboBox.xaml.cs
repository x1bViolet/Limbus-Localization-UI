namespace VisualStudioControlsTemplates
{
    public partial class ComboBoxStyleTechnical
    {
        private void ComboBox_PreventSelectScroll(object Sender, MouseWheelEventArgs Args)
        {
            if (((ComboBox)Sender).IsDropDownOpen == false) Args.Handled = true;
        }
    }
}