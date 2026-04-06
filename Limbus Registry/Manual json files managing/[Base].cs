using System.Windows.Markup;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        /// <summary>
        /// Open tech button context menu on button left click
        /// </summary>
        private void JsonManaging_TechButton_Click(object Sender, RoutedEventArgs Args)
        {
            (Sender as Button)!.ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// Don't open if main menu or switched to very last object just now (Little delay to not immediately trigger context menu on right click)
        /// </summary>
        private void RightMenu_NextObjectSwitchButton_JsonManagingContextMenuOpening(object Sender, ContextMenuEventArgs Args)
        {
            Args.Handled = (SwitchedToVeryLastJustNow | @EditorModesShelf.CurrentEditorMode == @EditorModesShelf.MainMenu);
        }
    }
}


// Custom UI elements and attached DependencyProperty to reduce XAML code amount
namespace LCLocalizationInterface.LimbusRegistry.ManualJsonFilesManaging
{
    /// <summary>
    /// Container for 2 manual json managing action <see cref="Options"/> (Context menus in form of <see cref="PopupButton_T1"/>): first is Remove, second is Add
    /// </summary>
    [ContentProperty(nameof(Options))]
    public class PairedOptions : DependencyObject
    {
        public FrameworkElement? ButtonStyleSource { get => (FrameworkElement?)GetValue(ButtonStyleSourceProperty); set => SetValue(ButtonStyleSourceProperty, value); }
        public static readonly DependencyProperty ButtonStyleSourceProperty = RegisterProperty<PairedOptions, FrameworkElement?>();

        public object ButtonStyleKey { get => GetValue(ButtonStyleKeyProperty); set => SetValue(ButtonStyleKeyProperty, value); }
        public static readonly DependencyProperty ButtonStyleKeyProperty = RegisterProperty<PairedOptions, object>(DefaultValue: typeof(Button));


        public List<PopupButton_T1> Options { get; } = [];
    }


    public static class Assistant
    {
        #region PairedButtons Attached Property
        /// <summary>
        /// Add a style to the <see cref="Button"/> based on the given <see cref="PairedOptions"/>: when IsEnabled is <see langword="false"/>, set the second item from <see cref="PairedOptions.Options"/> as the ContextMenu, otherwise the first item<br/><br/>
        /// Works as a dual context menu for description buttons in the right menu: when a button is unavailable, its context menu is "Add description", otherwise it is "Remove description" ((All this headache with styles because in WPF, all kinds of triggers other than <see cref="EventTrigger"/> can only be added to a style))
        /// </summary>
        public static readonly DependencyProperty PairedOptionsProperty = RegisterAttachedProperty(
            PropertyType: typeof(PairedOptions), OwnerType: typeof(Assistant), DefaultValue: null,
            PropertyChangedEvent: delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
            {
                if (Sender is Button TargetButton && Args.NewValue is PairedOptions GivenButtonsPair)
                {
                    ResourceDictionary ButtonBaseStyleLocation = GivenButtonsPair.ButtonStyleSource?.Resources ?? Application.Current.Resources;

                    if (ButtonBaseStyleLocation is not null && ButtonBaseStyleLocation.Contains(GivenButtonsPair.ButtonStyleKey))
                    {
                        TargetButton.Style = new Style()
                        {
                            TargetType = typeof(Button), BasedOn = (ButtonBaseStyleLocation[GivenButtonsPair.ButtonStyleKey] as Style)!,
                            Setters = { new Setter() { Property = Button.ContextMenuProperty, Value = GivenButtonsPair.Options[0] } },
                            Triggers =
                            {
                                new Trigger()
                                {
                                    Property = Button.IsEnabledProperty, Value = false,
                                    Setters = { new Setter() { Property = Button.ContextMenuProperty, Value = GivenButtonsPair.Options[1] } }
                                }
                            }
                        };
                    }
                    TargetButton.ContextMenuOpening += delegate (object Sender, ContextMenuEventArgs Args)
                    {
                        // Prevent all those context menus from opening if this option is globally disabled in config
                        Args.Handled = LoadedConfiguration.Internal.EnableManualJsonFilesManaging == false;
                    };
                }
            }
        );
        public static PairedOptions GetPairedOptions(DependencyObject Sender) => (PairedOptions)Sender.GetValue(PairedOptionsProperty);
        public static void SetPairedOptions(DependencyObject Sender, PairedOptions Value) => Sender.SetValue(PairedOptionsProperty, Value);
        #endregion
    }
}