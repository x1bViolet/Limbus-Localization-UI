using System.Windows.Controls;
using System.Windows.Input;

namespace Visual_Studio_controls
{
    public partial class ComboBoxStyleTechnical
    {
        private void ComboBox_PreventSelectScroll(object RequestSender, MouseWheelEventArgs EventArgs)
        {
            if (((ComboBox)RequestSender).IsDropDownOpen == false) EventArgs.Handled = true;
        }
    }
}
