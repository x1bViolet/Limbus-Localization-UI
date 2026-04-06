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
                    case "[C] * [Section:Sinner and Identity/E.G.O specific] Header color":
                        if (TryParseColor(InputText, out Color HeaderColor))
                        {
                            Header_SinnerName.Source = BitmapFromResource("Limbus Registry/※ Identity ¦ E.G.O  Preview Creator/Images/Header/Sinner Name.png").Tint(HeaderColor);
                            Header_IdentityOrEGOName.Source = BitmapFromResource("Limbus Registry/※ Identity ¦ E.G.O  Preview Creator/Images/Header/Identity or E.G.O Name.png").Tint(HeaderColor);
                        }
                        break;

                    case "[C] * [Section:Image parameters / Portrait parameters [E.G.O]] Frame color":
                        if (TryParseColor(InputText, out Color EGOFrameColor))
                        {
                            EGOPortrait_Frame.Source = BitmapFromResource("Limbus Registry/※ Identity ¦ E.G.O  Preview Creator/Images/E.G.O Frame.png").Tint(EGOFrameColor);
                            EGOFrameColor_Bloom.Color = EGOFrameColor;

                            _ = @Languages.PresentedTextFields["[C] * [Section:Sinner and Identity/E.G.O specific] Header color"].Text
                              = @Languages.PresentedTextFields["[C] * [Section:Decorative cautions] Cautions color"].Text
                              = InputText;
                        }
                        break;

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