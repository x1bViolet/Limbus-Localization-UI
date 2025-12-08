using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;

namespace LC_Localization_Task_Absolute.Json.Manual_localization_files_managing
{
    public partial class ObjectIDInputDialog : Window
    {
        public enum StringCheckMode
        {
            Skill,
            Passive,
            Keyword
        }
        
        public string ResponseText { get; private set; } = string.Empty;

        private StringCheckMode CurrentMode { get; }
        private Regex ValidationPattern { get; }
        private bool CheckIDListsCondition { get; }
        private dynamic CheckIDList { get; } // List<int or string>

        public ObjectIDInputDialog(StringCheckMode Mode, bool CheckCurrentIDLists = true)
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;

            CurrentMode = Mode;
            CheckIDListsCondition = CheckCurrentIDLists;

            //lang=regex
            ValidationPattern = new(pattern: CurrentMode switch
            {
                StringCheckMode.Skill   => @"^(\-|\+)?\d+$",
                StringCheckMode.Passive => @"^(\-|\+)?\d+$",
                StringCheckMode.Keyword => @"^\w+$",
            });

            CheckIDList = CurrentMode switch
            {
                StringCheckMode.Skill   => DelegateSkills_IDList,
                StringCheckMode.Passive => DelegatePassives_IDList,
                StringCheckMode.Keyword => DelegateKeywords_IDList,
            };
        }

        private void ConfirmInput_Click(object RequestSender, RoutedEventArgs EventArgs)
        {
            ResponseText = ObjectIDInput.Text;
            this.DialogResult = true;
        }

        private void ObjectIDInput_TextChanged(object RequestSender, TextChangedEventArgs EventArgs)
        {
            if (IsLoaded)
            {
                bool Result = false;

                if (ValidationPattern.Match(ObjectIDInput.Text).Success)
                {
                    if (CurrentMode == StringCheckMode.Keyword && !CheckIDList.Contains(ObjectIDInput.Text))
                    {
                        Result = true;
                    }
                    else if (int.TryParse(ObjectIDInput.Text, out int IntID) && (CheckIDListsCondition ? !CheckIDList.Contains(IntID) : true))
                    {
                        Result = true;
                    }
                }

                ConfirmInput.IsEnabled = Result;
            }
        }
        private void ObjectIDInput_PreviewKeyDown(object RequestSender, KeyEventArgs EventArgs) // Enter confirm
        {
            if (EventArgs.Key == Key.Enter && ConfirmInput.IsEnabled) ConfirmInput_Click(null, null);
        }


        private void Window_Loaded(object RequestSender, RoutedEventArgs EventArgs)
        {
            ObjectIDInput.Focus();

        #region Sqare corners
            int Preference = 1;
            DwmSetWindowAttribute(new WindowInteropHelper(this).Handle, 33, ref Preference, sizeof(int));
        }
        [LibraryImport("dwmapi.dll")]
        static partial void DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);
        #endregion
    }
}
