namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public partial class PreviewCreatorPage : Page
    {
        private void TextChanged_Shared(object Sender, RoutedEventArgs Args)
        {
            if (IsLoaded)
            {
                IntenseStareType3 ActualSender = (Sender as IntenseStareType3)!;
                string InputText = ActualSender.Text;

                switch (ActualSender.UID)
                {
                    case "[C] * [Section:Decorative cautions] Cautions color":
                        if (TryParseColor(InputText, out Color CautionsColor))
                        {
                            DecorativeCautions_StyleBindingSource.Foreground = new SolidColorBrush(CautionsColor);
                        }
                        break;

                    default: break;
                }
            }
        }
    }
}