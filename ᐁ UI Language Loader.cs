using LC_Localization_Task_Absolute.Json;
using LC_Localization_Task_Absolute.Limbus_Integration;
using LC_Localization_Task_Absolute.Mode_Handlers;
using Microsoft.Win32;
using RichText;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static LC_Localization_Task_Absolute.Configurazione;
using static LC_Localization_Task_Absolute.Json.BaseTypes.Type_Skills;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using static LC_Localization_Task_Absolute.Json.FilesIntegration;
using static LC_Localization_Task_Absolute.Requirements;
using static System.Globalization.NumberStyles;
using static System.Windows.Visibility;


namespace LC_Localization_Task_Absolute
{
    #region Custom UI Elements
    /// <summary>
    /// Parent interface for <c>&lt;UILocalization_Grocerius&gt;</c> and <c>&lt;UILocalization_Roseum&gt;</c> to unify their type in dictionary
    /// </summary>
    interface InterfaceTranslationEntry;

    public class UILocalization_Grocerius : RichTextBox, InterfaceTranslationEntry
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(UILocalization_Grocerius),
                new PropertyMetadata(string.Empty, SetRichTextFromProperty));

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(
                "TextAlignment",
                typeof(TextAlignment),
                typeof(UILocalization_Grocerius),
                new PropertyMetadata(System.Windows.TextAlignment.Left, SetTextAlignmentProperty));

        /// <summary>
        /// Set rich text
        /// </summary>
        public string Text
        {
            set {
                this.SetRichText(value);

                CurrentRichText = value;
            }
        }

        /// <summary>
        /// Rich text string that have been set for this RichTextBox last
        /// </summary>
        public string CurrentRichText { get; private set; }

        /// <summary>
        /// Raw text extracted by the TextRange (Use <c>CurrentRichText</c> property to get actual content)
        /// </summary>
        public string RawText { get => this.GetText(); }

        private static void SetRichTextFromProperty(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            (CurrentElement as UILocalization_Grocerius).SetRichText(ChangeArgs.NewValue as string);
        }

        private static void SetTextAlignmentProperty(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            foreach (Block TextBlock in (CurrentElement as UILocalization_Grocerius).Document.Blocks) TextBlock.TextAlignment = ChangeArgs.NewValue as dynamic;
        }

        /// <summary>
        /// For context menu in identity preview creator columns (Unable to normally get parent of ContextMenu with this RichTextBox as MenuItem Header)
        /// </summary>
        internal ItemRepresenter SpecProperty_ContextMenuParent { get; set; }

        public TextAlignment TextAlignment
        {
            get => this.TextAlignment;
            set {
                foreach (Block TextBlock in this.Document.Blocks) TextBlock.TextAlignment = value;
            }
        }

        public string PlainText // Simple text without processing rich text (E.g. sliders with high frequency on value changes = maybe high cpu load)
        {
            set
            {
                this.Document.Blocks.Clear();
                this.Document.Blocks.Add(new Paragraph(new Run(value)));
            }
        }

        public static readonly DependencyProperty UIDProperty =
            DependencyProperty.Register(
                "UID",
                typeof(string),
                typeof(UILocalization_Grocerius),
                new PropertyMetadata(null, OnUIDChanged));

        public string UID
        {
            get => (string)GetValue(UIDProperty);
            set => SetValue(UIDProperty, value);
        }

        private static void OnUIDChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            string DefinedUID = ChangeArgs.NewValue as string;

            if (!ᐁ_Interface_Localization_Loader.PresentedEntries.ContainsKey(DefinedUID))
            {
                if (!string.IsNullOrEmpty(DefinedUID))
                {
                    ᐁ_Interface_Localization_Loader.PresentedEntries[DefinedUID] = CurrentElement as UILocalization_Grocerius;
                    Console.WriteLine($"Known {DefinedUID}");
                }
            }
            else
            {
                throw new Exception("! Same UID already defined in this XAML document");
            }
        }

        // For something
        public Dictionary<string, dynamic> UniversalDataBindings = new Dictionary<string, dynamic>();

        /// <summary>
        /// Test RichTextBox element for ui translation, for now used only in custom identity preview creator
        /// <br/>
        /// RichTextBox with Text property that leads to the RichTextBoxApplicator.SetRichText() method for applying tags (+UID for dictionary)
        /// </summary>
        public UILocalization_Grocerius()
        {
            IsReadOnly = true;
            Focusable = false;
            BorderBrush = Background = Brushes.Transparent;
            BorderThickness = new Thickness(0);
            Foreground = Brushes.Beige;
            FontSize = 13;
        }
    }

    public class UILocalization_Roseum : TextBlock, InterfaceTranslationEntry
    {
        public static readonly DependencyProperty UIDProperty =
            DependencyProperty.Register(
                "UID",
                typeof(string),
                typeof(UILocalization_Roseum),
                new PropertyMetadata(null, OnUIDChanged));

        public string UID
        {
            get => (string)GetValue(UIDProperty);
            set => SetValue(UIDProperty, value);
        }

        private static void OnUIDChanged(DependencyObject CurrentElement, DependencyPropertyChangedEventArgs ChangeArgs)
        {
            var DefinedUID = ChangeArgs.NewValue as string;

            if (!ᐁ_Interface_Localization_Loader.PresentedEntries.ContainsKey(DefinedUID))
            {
                if (!string.IsNullOrEmpty(DefinedUID))
                {
                    ᐁ_Interface_Localization_Loader.PresentedEntries[DefinedUID] = CurrentElement as UILocalization_Roseum;
                    Console.WriteLine($"Known {DefinedUID}");
                }
            }
            else
            {
                throw new Exception("! Same UID already defined in this XAML document");
            }
        }

        // For something
        public Dictionary<string, dynamic> UniversalDataBindings = new Dictionary<string, dynamic>();

        /// <summary>
        /// Same with TextBlock (RichTextBox sometimes weird)
        /// </summary>
        public UILocalization_Roseum()
        {
            IsHitTestVisible = Focusable = false;
            Foreground = Brushes.Beige;
            FontSize = 13;
        }
    }
    #endregion

    internal abstract class InterfaceLocalizationModifiers
    {
        
    }

    internal abstract class ᐁ_Interface_Localization_Loader
    {
        internal protected static Dictionary<string, InterfaceTranslationEntry> PresentedEntries = new Dictionary<string, InterfaceTranslationEntry>();


    }
}
