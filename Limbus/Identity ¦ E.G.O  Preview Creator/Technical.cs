using LC_Localization_Task_Absolute.PreviewCreator;
using System.Windows;
using System.Windows.Controls;
using static LC_Localization_Task_Absolute.PreviewCreator.CompositionData_PROP.TextColumns_PROP;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.ᐁ_Interface_Localization_Loader;

namespace LC_Localization_Task_Absolute;

public partial class MainWindow
{
    private class CautionsTextElement : StackPanel
    {
        public CautionsTextElement(VerticalAlignment SlashCharsAlignment)
        {
            this.Orientation = Orientation.Horizontal;
            this.Margin = new Thickness(0, 0, 8.3, 0);


            TextBlock Part_SlashChars = new TextBlock()
            {
                FontFamily = FontFromResource("UI/Fonts/", "Sappy"),
                Margin = new Thickness(0, 0, 8.3, 0),
                FontSize = 10.5,
                Text = "////",
            };
            Part_SlashChars.BindSame(TextBlock.ForegroundProperty, MainControl.DecorativeCautions_PropertyBindingSource);

            TextBlock Part_Text = new TextBlock()
            {
                Margin = new Thickness(0, -0.6, 0, 0)
            };
            Part_Text.BindSame(TextBlock.ForegroundProperty, MainControl.DecorativeCautions_PropertyBindingSource);
            Part_Text.BindSame(TextBlock.FontFamilyProperty, MainControl.DecorativeCautions_PropertyBindingSource);
            Part_Text.BindSame(TextBlock.FontSizeProperty, MainControl.DecorativeCautions_PropertyBindingSource);
            Part_Text.BindSame(TextBlock.LineHeightProperty, MainControl.DecorativeCautions_PropertyBindingSource);
            Part_Text.BindSame(TextBlock.TextProperty, MainControl.DecorativeCautions_PropertyBindingSource);
            Part_Text.BindSame(TextBlock.RenderTransformProperty, MainControl.DecorativeCautions_PropertyBindingSource);


            this.Children.Add(Part_SlashChars);
            this.Children.Add(Part_Text);
        }
    }

    private void SetupPreviewCreator()
    {
        DecorativeCautions_TopStackPanel.Children.Clear();
        DecorativeCautions_BottomStackPanel.Children.Clear();
        for (int i = 0; i <= 50; i++)
        {
            DecorativeCautions_TopStackPanel.Children.Add(new CautionsTextElement(VerticalAlignment.Top));
            DecorativeCautions_BottomStackPanel.Children.Add(new CautionsTextElement(VerticalAlignment.Bottom));
        }

        @CurrentPreviewCreator.ImageInfoLoadingEvent = true;
        ExchangeJsonValues(@CurrentPreviewCreator.LoadedImageInfo, ExchangingType.Load);
        @CurrentPreviewCreator.ImageInfoLoadingEvent = false;

        VC_PortraitType.SelectedIndex = 1;
        VC_PortraitType.SelectedIndex = 0;
        VC_SinnerIcon.SelectedIndex = -1;
    }

    private void SwitchUIQuestion(object RequestSender, RoutedEventArgs EventArgs)
    {
        string Tilte = GetLocalizationTextFor("[C] [!] [&] * UI switch dialog", "Message window title");
        string Forward = GetLocalizationTextFor("[C] [!] [&] * UI switch dialog", "Switch to");
        string Back = GetLocalizationTextFor("[C] [!] [&] * UI switch dialog", "Return back");

        if (@CurrentPreviewCreator.IsActive)
        {
            MessageBoxResult UISwitchQuestionResult = MessageBox.Show(Back, Tilte, MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            if (UISwitchQuestionResult == MessageBoxResult.Yes)
            {
                SwitchUI_Deactivate();
            }
        }
        else
        {
            MessageBoxResult UISwitchQuestion = MessageBox.Show(Forward, Tilte, MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
            if (UISwitchQuestion == MessageBoxResult.Yes)
            {
                SwitchUI_Activate();
            }
        }
    }

    private double PreviousLocation_Left;
    private double PreviousLocation_Top;
    private bool ManuallyDisabledTopmost;
    private void SwitchUI_Activate(bool DisableTopmost = true)
    {
        if (this.Topmost & DisableTopmost)
        {
            this.Topmost = false;
            ManuallyDisabledTopmost = true;
        }

        LimbusJsonTextEditor.Visibility = Visibility.Collapsed;

        PreviousLocation_Left = this.Left;
        PreviousLocation_Top = this.Top;

        this.MaxWidth = 100000;
        this.MaxHeight = 100000;

        Rect ScreenSpace = SystemParameters.WorkArea; // *fullscreen*

        this.Left = ScreenSpace.Left;
        this.Top = ScreenSpace.Top;
        this.Width = ScreenSpace.Width;
        this.Height = ScreenSpace.Height;

        this.ResizeMode = ResizeMode.NoResize;

        CurrentWindowChrome.ResizeBorderThickness = new Thickness(0);
        CurrentWindowChrome.CaptionHeight = 0;

        MainWindowContentControl.BorderThickness = new Thickness(0);
        this.CanDragMove = false;

        IdentityPreviewCreator.Visibility = Visibility.Visible;

        @CurrentPreviewCreator.IsActive = true;
    }
    private void SwitchUI_Deactivate()
    {
        if (ManuallyDisabledTopmost)
        {
            this.Topmost = true;
        }

        IdentityPreviewCreator.Visibility = Visibility.Collapsed;
        
        Mode_Handlers.Upstairs.AdjustUIToRecent();

        this.ResizeMode = ResizeMode.CanResizeWithGrip;

        CurrentWindowChrome.ResizeBorderThickness = new Thickness(10);
        CurrentWindowChrome.CaptionHeight = 1;

        MainWindowContentControl.BorderThickness = new Thickness(1);
        this.CanDragMove = true;

        this.Top = PreviousLocation_Top;
        this.Left = PreviousLocation_Left;

        LimbusJsonTextEditor.Visibility = Visibility.Visible;

        @CurrentPreviewCreator.IsActive = false;
    }



    #region Column elements context menu
    private (Grid TargetElement, StackPanel Host) GetContextMenuItemTargets(object ContextMenuItem)
    {
        Grid TargetElement = (((ContextMenuItem as MenuItem).Parent as ContextMenu).PlacementTarget as Grid)!;
        StackPanel Host = (TargetElement.Parent as StackPanel)!;
        return (TargetElement, Host);
    }

    private void ColumnItem_MoveUp(object RequestSender, RoutedEventArgs EventArgs)
    {
        (Grid TargetElement, StackPanel Host) = GetContextMenuItemTargets(RequestSender);
        Host.MoveItemUp(TargetElement);
        @CurrentPreviewCreator.LoadedImageInfo.TextColumns.ReEnumerateColumnItems();
    }

    private void ColumnItem_Refresh(object RequestSender, RoutedEventArgs EventArgs)
    {
        (Grid TargetElement, StackPanel Host) = GetContextMenuItemTargets(RequestSender);

        FocusOnColumnElement(TargetElement);
        switch ((TargetElement.DataContext as TextItem_PROP).Type)
        {
            case "Skill": CheckBothSkillsIDSelectorsAndAddDisplaying(); break;
            case "Passive": CheckPassiveIDSelectorAndAddDisplaying(); break;
            case "Keyword": CheckKeywordIDSelectorAndAddDisplaying(); break;
        }
    }

    private void ColumnItem_MoveDown(object RequestSender, RoutedEventArgs EventArgs)
    {
        (Grid TargetElement, StackPanel Host) = GetContextMenuItemTargets(RequestSender);
        Host.MoveItemDown(TargetElement);
        @CurrentPreviewCreator.LoadedImageInfo.TextColumns.ReEnumerateColumnItems();
    }

    private void ColumnItem_Delete(object RequestSender, RoutedEventArgs EventArgs)
    {
        (Grid TargetElement, StackPanel Host) = GetContextMenuItemTargets(RequestSender);
        Host.Children.Remove(TargetElement);
        @CurrentPreviewCreator.LoadedImageInfo.TextColumns.ReEnumerateColumnItems();
    }
    #endregion
}