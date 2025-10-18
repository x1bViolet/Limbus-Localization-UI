using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static LC_Localization_Task_Absolute.Requirements;
using static LC_Localization_Task_Absolute.Json.DelegateDictionaries;
using System.Text.RegularExpressions;

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
        private string ValidationPattern { get; }
        private bool CheckIDListsCondition { get; }
        private dynamic CheckIDList { get; }

        public ObjectIDInputDialog(StringCheckMode Mode, bool CheckCurrentIDLists = true)
        {
            InitializeComponent();

            this.Loaded += Window_Loaded;



            CurrentMode = Mode;
            CheckIDListsCondition = CheckCurrentIDLists;
            //lang=regex
            ValidationPattern = CurrentMode switch
            {
                StringCheckMode.Skill   => @"^\d+$",
                StringCheckMode.Passive => @"^\d+$",
                StringCheckMode.Keyword => @"^\w+$",
            };

            CheckIDList = CurrentMode switch
            {
                StringCheckMode.Skill   => DelegateSkills_IDList,
                StringCheckMode.Passive => DelegatePassives_IDList,
                StringCheckMode.Keyword => DelegateKeywords_IDList,
            };
        }

        private void ConfirmInput_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = ObjectIDInput.Text;
            this.DialogResult = true;
        }

        private void ObjectIDInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                bool Result = false;

                if (Regex.Match(ObjectIDInput.Text, ValidationPattern).Success)
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


        private void Window_Loaded(object RequestSender, RoutedEventArgs EventArgs)
        {
            ObjectIDInput.Focus();

        #region Sqare corners
            int Preference = 1;
            DwmSetWindowAttribute(new WindowInteropHelper(this).Handle, 33, ref Preference, sizeof(int));
        }
        [DllImport("dwmapi.dll")]
        private static extern void DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);
        #endregion
    }
}
