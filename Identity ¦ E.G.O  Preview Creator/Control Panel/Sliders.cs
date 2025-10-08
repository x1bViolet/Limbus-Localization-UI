using LC_Localization_Task_Absolute.PreviewCreator; // <- struct CompositionReference
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static LC_Localization_Task_Absolute.MainWindow;
using static LC_Localization_Task_Absolute.Requirements;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    private void ImageWidth_CheckEachStepAvailable()
    {
        Slider First  = VC_ImageWidth_FirstStep;
        Slider Second = VC_ImageWidth_SecondStep;

        if (First.Value == First.Maximum) MakeAvailable(Second);
        else MakeUnavailable(Second);

        if (Second.Value == Second.Minimum) MakeAvailable(First);
        else MakeUnavailable(First);
    }

    private void ImageWidth_FirstStep(object RequestSender, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            CompositionGrid.Width = EventArgs.NewValue;
            ImageWidth_CheckEachStepAvailable();
        }
    }
    private void ImageWidth_SecondStep(object RequestSender, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            CompositionGrid.Width = EventArgs.NewValue;
            ImageWidth_CheckEachStepAvailable();
        }
    }


    private void TopVignettePlusLength(object RequestSender, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            if (EventArgs.NewValue < 0) TopVignettePlusLength_Identity_Parent.SetTopMargin(EventArgs.NewValue);
        }
    }
    private void LeftVignettePlusLength(object RequestSender, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            if (EventArgs.NewValue < 0) LeftVignettePlusLength_Identity_Parent.SetLeftMargin(EventArgs.NewValue);
        }
    }
    private void BottomVignettePlusLength(object RequestSender, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            if (EventArgs.NewValue < 0) BottomVignettePlusLength_Identity_Parent.SetBottomMargin(EventArgs.NewValue);
        }
    }
    private void RightVignettePlusLength(object RequestSender, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            if (EventArgs.NewValue < 0) RightVignettePlusLength_Identity_Parent.SetRightMargin(EventArgs.NewValue);
        }
    }
    private void LeftBehindEGOVignettePlusLength(object RequestSender, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded)
        {
            if (EventArgs.NewValue < 0) LeftBehindEGOPortraitVignettePlusLength_EGO_Parent.SetLeftMargin(EventArgs.NewValue);
        }
    }



    private void FirstColumnSignaturesOffset(object RequestSender, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded) CompositionResources.TextColumns.First.ItemSignatures_HorizontalOffset = EventArgs.NewValue;
    }
    private void SecondColumnSignaturesOffset(object RequestSender, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded) CompositionResources.TextColumns.Second.ItemSignatures_HorizontalOffset = EventArgs.NewValue;
    }
    private void KeywordContainersWidth(object RequestSender, RoutedPropertyChangedEventArgs<double> EventArgs)
    {
        if (IsLoaded) CompositionResources.TextColumns.KeywordContainersWidth = EventArgs.NewValue;
    }
}