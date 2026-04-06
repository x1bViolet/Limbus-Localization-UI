namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    #region Some specific sliders
    public partial class PreviewCreatorPage : Page
    {
        private void ResetSliderValue(object Sender, MouseButtonEventArgs Args)
        {
            Slider Target = (Sender as Slider)!;
            Target.Value = Target.TickFrequency; // TickFrequency used as default value
        }




        private void ImageWidth_CheckEachStepAvailable()
        {
            Slider First = VC_ImageWidth_FirstStep;
            Slider Second = VC_ImageWidth_SecondStep;

            MakeAvailableConditional(First.Value == First.Maximum, Second);
            MakeAvailableConditional(Second.Value == Second.Minimum, First);
        }

        private void ImageWidth_FirstStep_SliderValueChanged(object Sender, RoutedPropertyChangedEventArgs<double> Args)
        {
            if (IsLoaded)
            {
                CompositionGrid.Width = Args.NewValue;
                ImageWidth_CheckEachStepAvailable();
            }
        }
        private void ImageWidth_SecondStep_SliderValueChanged(object Sender, RoutedPropertyChangedEventArgs<double> Args)
        {
            if (IsLoaded && VC_ImageWidth_FirstStep.Value == VC_ImageWidth_FirstStep.Maximum)
            {
                CompositionGrid.Width = Args.NewValue;
                ImageWidth_CheckEachStepAvailable();
            }
        }




        private void TopVignettePlusLength_NegativeValueHandler(object Sender, RoutedPropertyChangedEventArgs<double> Args)
        {
            if (IsLoaded) TopVignettePlusLength_Identity_Parent.SetTopMargin(Args.NewValue < 0 ? Args.NewValue : 0);
        }
        private void LeftVignettePlusLength_NegativeValueHandler(object Sender, RoutedPropertyChangedEventArgs<double> Args)
        {
            if (IsLoaded) LeftVignettePlusLength_Identity_Parent.SetLeftMargin(Args.NewValue < 0 ? Args.NewValue : 0);
        }
        private void BottomVignettePlusLength_NegativeValueHandler(object Sender, RoutedPropertyChangedEventArgs<double> Args)
        {
            if (IsLoaded) BottomVignettePlusLength_Identity_Parent.SetBottomMargin(Args.NewValue < 0 ? Args.NewValue : 0);
        }
        private void RightVignettePlusLength_NegativeValueHandler(object Sender, RoutedPropertyChangedEventArgs<double> Args)
        {
            if (IsLoaded) RightVignettePlusLength_Identity_Parent.SetRightMargin(Args.NewValue < 0 ? Args.NewValue : 0);
        }
        private void LeftBehindEGOVignettePlusLength_NegativeValueHandler(object Sender, RoutedPropertyChangedEventArgs<double> Args)
        {
            if (IsLoaded) LeftBehindEGOPortraitVignettePlusLength_EGO_Parent.SetLeftMargin(Args.NewValue < 0 ? Args.NewValue : 0);
        }
    }
    #endregion
}