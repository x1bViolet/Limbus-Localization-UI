using LCLocalizationInterface.LimbusRegistry.PreviewCreator;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage;

namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        private bool CommonHotkeyExecutionLock { get; set; } = false;
        private bool OnLeftRightSwitchTimeout { get; set; } = false;

        private bool PreviewCreator_ColumnButtonsWasManuallyHidden { get; set; } = false;



        /// <summary>Hotkeys</summary>
        private async void MainWindow_PreviewKeyDown(object Sender, KeyEventArgs Args)
        {
            // User-defined editor hotkeys from `[⇲] Assets Directory\Context Menu Hotkeys.json`
            if (UserHotkeyExecutionLock == false && LimbusJsonTextEditor.TextArea.IsFocused)
            {
                CheckPressedEditorContextMenuHotkeys();
            }



            if (CommonHotkeyExecutionLock == false)
            {
                if (@CurrentPreviewCreator.ActiveState == false)
                {
                    // [Ctrl + S] Save
                    if (new[] { Key.LeftCtrl, Key.S }.All(Keyboard.IsKeyDown))
                    {
                        CommonHotkeyExecutionLock = true; @EditorModesShelf.CurrentEditorMode.SaveCurrentFile_Entry();
                    }

                    // Manual ID input Stop/Confirm
                    if (IDManage_ManualInput.Visibility is Visibility.Visible)
                    {
                        if (Keyboard.IsKeyDown(Key.Escape))
                        {
                            NavigationPanel_IDSwitch_ManualInput_Stop();
                        }
                        else if (Keyboard.IsKeyDown(Key.Enter))
                        {
                            NavigationPanel_IDSwitch_ManualInput_Confirm();
                        }
                    }
                }




                // [Left Shift + Left Ctrl + P] Screenshot
                if (new[] { Key.LeftCtrl, Key.LeftShift, Key.P }.All(Keyboard.IsKeyDown))
                {
                    CommonHotkeyExecutionLock = true; MakeLimbusTextPreviewScanButton_Click(null!, null!);
                }




                // [Esc] Unfocus textfields
                if (Keyboard.IsKeyDown(Key.Escape))
                {
                    void SetFocusToWindowInner()
                    {
                        FocusManager.SetFocusedElement(
                            element: this,
                            value: @CurrentPreviewCreator.ActiveState == true
                                ? PreviewCreatorPageInstance.CompositionScrollViewer
                                : WindowFocusManagingPlaceholder
                        );
                    }

                    if (Keyboard.FocusedElement is ICSharpCode.AvalonEdit.Editing.TextArea FocusedTextArea)
                    {
                        ICSharpCode.AvalonEdit.TextEditor TextEditorParent = FocusedTextArea.Parent.FindVisualParent<ICSharpCode.AvalonEdit.TextEditor>()!;
                        if (TextEditorParent.SelectionLength > 0)
                        {
                            TextEditorParent.Select(TextEditorParent.SelectionStart + TextEditorParent.SelectionLength, 0); // Clear selection first
                        }
                        else
                        {
                            SetFocusToWindowInner();
                        }
                    }
                    else if (Keyboard.FocusedElement is TextBox FocusedTextBox)
                    {
                        SetFocusToWindowInner();
                    }
                }




                if (@CurrentPreviewCreator.ActiveState == true)
                {
                    // [Ctrl + H] Hide/Show "[Column №1/2] Add.." buttons in Identity/E.G.O Preview Creator
                    if (new[] { Key.LeftCtrl, Key.H }.All(Keyboard.IsKeyDown))
                    {
                        CommonHotkeyExecutionLock = true;

                        PreviewCreator_ColumnButtonsWasManuallyHidden = !PreviewCreator_ColumnButtonsWasManuallyHidden;

                        _ = PreviewCreatorPageInstance.FirstColumnItemsSelector.Visibility
                          = PreviewCreatorPageInstance.SecondColumnItemsSelector.Visibility
                          = PreviewCreator_ColumnButtonsWasManuallyHidden ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }



            #region ID switching on Left/Right arrows press
            bool CancelIDSwitchByKeyboardButtons = false;
            @EditorModesShelf.CurrentEditorMode.WindowPreviewKeyDown(Args, ref CancelIDSwitchByKeyboardButtons);

            // But not when typing something
            if (CancelIDSwitchByKeyboardButtons == false && Keyboard.FocusedElement is not ICSharpCode.AvalonEdit.Editing.TextArea & Keyboard.FocusedElement is not TextBox)
            {
                if (OnLeftRightSwitchTimeout == false)
                {
                    if (Args.Key is Key.Left & RightMenu_PreviousObjectSwitchButton.IsEnabled)
                    {
                        @EditorModesShelf.CurrentEditorMode.SwitchIDButtonClick(@EditorModesShelf.IDSwitchDirection.Back);
                        OnLeftRightSwitchTimeout = true;
                        await Task.Delay(40);
                        OnLeftRightSwitchTimeout = false;
                    }
                    else if (Args.Key is Key.Right & RightMenu_NextObjectSwitchButton.IsEnabled)
                    {
                        @EditorModesShelf.CurrentEditorMode.SwitchIDButtonClick(@EditorModesShelf.IDSwitchDirection.Forward);
                        OnLeftRightSwitchTimeout = true;
                        await Task.Delay(40);
                        OnLeftRightSwitchTimeout = false;
                    }
                }
            }
            #endregion
        }




        private bool UserHotkeyExecutionLock = false;
        private void CheckPressedEditorContextMenuHotkeys()
        {
            foreach (ContextMenuHotkeys.HotkeysConfig.Hotkey Shortcut in ContextMenuHotkeys.LoadedHotkeys.HotkeysList.Where(Shortcut => Shortcut.Command is not null & Shortcut.Keys.Count > 0))
            {
                if (Shortcut.Keys.All(Keyboard.IsKeyDown))
                {
                    UserHotkeyExecutionLock = true;


                    // Extra replacement option
                    if (Shortcut.Command!.Matches(@"^Extra \d+$"))
                    {
                        int CommantNumber = int.Parse(Regex.Match(Shortcut.Command!, @"^Extra (\d+)$").Groups[1].Value);
                        if (CommantNumber <= ExtraReplacementsContextMenu.ContextMenuObject.Items.Count)
                        {
                            TextEditor_ContextMenuClick(ExtraReplacementsContextMenu.ContextMenuObject.Items[CommantNumber - 1], null!);
                        }
                    }

                    // Regular command name
                    else if (Shortcut.Command!.EqualsToOneOf(
                        "Insert Style",
                        "<TMPro> To [KeywordID]",
                        "<TMPro> To Shorthands",
                        "Implicit To [KeywordID]",
                        "Implicit To Shorthands",
                        "[KeywordID] To Shorthands",
                        "[KeywordID] To <TMPro>")
                    ) {
                        TextEditor_ContextMenuClick(new MenuItem_T1() { Uid = Shortcut.Command }, null!); // => switch ((Sender as MenuItem_T1)!.Uid) ...
                    }
                }
            }
        }




        /// <summary>Pressed hotkeys latch unlock</summary>
        private void MainWindow_PreviewKeyUp(object Sender, KeyEventArgs Args)
        {
            CommonHotkeyExecutionLock = UserHotkeyExecutionLock = false;
        }
    }
}