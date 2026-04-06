using ICSharpCode.AvalonEdit.Document;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using LCLocalizationInterface.LimbusRegistry.PreviewCreator;
using System.Windows.Threading;
using static LCLocalizationInterface.LimbusRegistry.@EditorModesShelf.Types;

namespace LCLocalizationInterface.LimbusRegistry
{
    public ref partial struct @EditorModesShelf
    {
        // Default mode set (Directly setting it via CurrentEditorMode from below instead of static constructor will make it null for some reason)
        static @EditorModesShelf() => CurrentEditorMode = MainMenu;


        public static @EditorModesShelf.Types.EditorModeIntermediator CurrentEditorMode { get; set; }





        /// <summary>
        /// Reflects the possible beginnings of the file name and the corresponding editor mode ("EN_", "KR_", "JP_" prefixes ignored, used in <see cref="MainWindow.OpenJsonFileButton_Click"/> to switch to the appropriate editor mode)
        /// </summary>
        public record ModeAssociation(string[] PossibleFileNameStarts, @EditorModesShelf.Types.EditorModeIntermediator AssociatedMode);

        #pragma warning disable CS8604
        /// <inheritdoc cref="ModeAssociation"/>
        public static List<ModeAssociation> ModesMapping { get; } = new()
        {
            new(["Skills" ], @EditorModesShelf.Skills),
            new(["Passive"], @EditorModesShelf.Passives),
            new(["EGOgift"], @EditorModesShelf.EGOGifts),
            new(["BattleKeywords", "Bufs"], @EditorModesShelf.Keywords),
            new(["AbnormalityGuides"], @EditorModesShelf.ObservationLogs),
        };
        #pragma warning restore CS8604





        /// <summary>Contains specific info about current file: is UTF-8 BOM, json Indentation size, Line Break mode</summary>
        public record FileSpecific(bool IsBOM, int IndentationSize, LineBreakMode LineBreaks);


        /// <inheritdoc cref="Types.EditorModeAbstraction{LocalizationDataType}.Dimensing"/>
        public record MainWindowDimensing
        {
            public required double MaxWidth; public required double MinWidth; public required double MinHeight;
            public required double Width;    public required double Height;

            public required GridLength RichTextViewsHeight;
            public required GridLength TextEditorAndRichTextViewsWidth;

            public required int RichTextViewsTabIndex;
            public required int RightMenuSectionsTabIndex;
        }
        private static FieldInfo[] DimensingFields => typeof(MainWindowDimensing).GetFields();


        public enum IDSwitchDirection { Back, Forward }






        public ref partial struct Types
        {
            public enum EditorModeKey
            {
                MainMenu,

                Skills,
                Passives,
                Keywords,
                EGOGifts,
                ObservationLogs,
            }


            /// <summary>
            /// Parent interface for <see cref="EditorModeAbstraction{LocalizationDataType}"/> <see langword="class"/> leading to options needed for general use (Generalized calls like <see cref="@EditorModesShelf.CurrentEditorMode"/>.Something()).
            /// <br/><br/>
            /// All editor modes can be considered as this type.
            /// </summary>
            public interface EditorModeIntermediator
            {
                public EditorModeKey Identifier { get; }




                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.TryValidateJsonAndSwitchMode"/>
                public void TryValidateJsonAndSwitchMode(FileInfo File, string OpenFile_TypeCheckName);








                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.RefreshRichText"/>
                public void RefreshRichText();


                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.ScreenshotRichText"/>
                public void ScreenshotRichText();








                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.CurrentFile"/>
                public FileInfo? CurrentFile { get; }


                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.CurrentFile"/>
                public FileSpecific? CurrentFileSpecific { get; }


                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.PresentedRightMenuSytaxedTextInputs"/>
                public List<IntenseStareType3> PresentedRightMenuSytaxedTextInputs { get; }


                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.RecentlySerializedJsonText"/>
                public string? RecentlySerializedJsonText { get; }




                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.CollectUnsavedChanges"/>
                public (bool IsAnyUnsavedChanges, string UnsavedChangesText) CollectUnsavedChanges();








                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.SwitchIDButtonClick"/>
                public void SwitchIDButtonClick(IDSwitchDirection Direction);


                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.SwitchIDButtonClick_ToVeryFirstOrLast"/>
                public void SwitchIDButtonClick_ToVeryFirstOrLast(IDSwitchDirection Direction);




                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.SwitchToObjectByInput"/>
                public void SwitchToObjectByInput(string Input);









                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.ChangeAllRightMenuUnsavedChangesMarkers_OnButtons"/>
                public void ChangeAllRightMenuUnsavedChangesMarkers_OnButtons();


                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs"/>
                public void ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();


                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc"/>
                public void ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(TextDocument CurrentDocument);








                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.WindowPreviewMouseDown"/>
                public void WindowPreviewMouseDown(MouseButtonEventArgs Args, ref bool CancelIDSwitchByMouseXButtons);


                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.WindowPreviewKeyDown"/>
                public void WindowPreviewKeyDown(KeyEventArgs Args, ref bool CancelIDSwitchByKeyboardButtons);








                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.SaveCurrentFile_Entry"/>
                public void SaveCurrentFile_Entry();








                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.AdjustMainWindow"/>
                public void AdjustMainWindow();







                /// <inheritdoc cref="EditorModeAbstraction{LocalizationDataType}.DiscardCurrentFile"/>
                public void DiscardCurrentFile();
            }








            /// <summary>(Just to make a visual distinction in code) Class contents required for editor mode beyond the core set provided by the <see cref="EditorModeIntermediator"/> interface or parent <see cref="EditorModeAbstraction{LocalizationDataType}"/> <see langword="class"/></summary>
            [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
            private class LayeredComponentAttribute : Attribute;








            /// <summary>
            /// Specific <see langword="abstract"/> <see langword="class"/> representing all the necessary things of interaction with the editor mode. Based on <see cref="EditorModeIntermediator"/> <see langword="interface"/>, so all derived classes will also automatically considered as it.
            /// <br/><br/>
            /// <see langword="Virtual"/> methods are optional and have an empty body by default.
            /// </summary>
            public abstract class EditorModeAbstraction<LocalizationDataType> : EditorModeIntermediator where LocalizationDataType : LimbusEditorJsonObject
            {
                public abstract EditorModeKey Identifier { get; }




                /// <summary>
                /// Predicate to use in <see cref="Enumerable.Any{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/> method on <see cref="LimbusLocalizationFile{LocalizationDataType}.DataList"/> to determine if there is at least one editable object which corresponds to the target type of editor mode
                /// <br/><br/>
                /// Also must be used to create IEnumerable of those editable objects between which the editor mode can switch by its own logic
                /// </summary>
                [LayeredComponent]
                protected abstract Func<LocalizationDataType, bool> DataListValidator { get; }


                /// <summary>
                /// <see cref="MainWindow"/> sizes adjustment values source when switching via <see cref="SwitchUI"/>
                /// <br/><br/>
                /// • <see cref="MainWindowDimensing.RichTextViewsHeight"/> is <see cref="MainWindow.RichTextViews__PARENT_Height"/> <see cref="GridLength"/>'s value (Height of current Rich Text View inside the <see cref="MainWindow.RichTextViews"/>)<br/>
                /// • <see cref="MainWindowDimensing.TextEditorAndRichTextViewsWidth"/> is <see cref="MainWindow.TextEditorAndRichTextViewsWidth"/> <see cref="GridLength"/>'s value (Width of both Text Editor and current Rich Text View according to the <see cref="MainWindow"/>.Width)
                /// <br/><br/>
                /// • <see cref="MainWindowDimensing.RichTextViewsTabIndex"/> is SelectedIndex for <see cref="MainWindow.RichTextViews"/>, <see cref="MainWindowDimensing.RightMenuSectionsTabIndex"/> for <see cref="MainWindow.RightMenuSections"/>
                /// </summary>
                [LayeredComponent]
                protected abstract MainWindowDimensing Dimensing { get; }


                /// <summary>
                /// Must contain all <see cref="IntenseStareType3"/>s from right menu for this mode to apply <see cref="JsonTextEditor.LimbusTextSyntax"/> to them when it updates (Only if they are used to display rich text at the <see cref="TMProEmitter"/>s)
                /// </summary>
                public abstract List<IntenseStareType3> PresentedRightMenuSytaxedTextInputs { get; }


                /// <summary>
                /// Called by <see cref="MainWindow.OpenJsonFileButton_Click"/> when confirmed that selected file is ok to be an <see cref="LimbusLocalizationFile{LocalizationDataType}"/> (<see langword="LocalizationDataType"/> as <see cref="object"/>) and it has at least 1 object in <see cref="LimbusLocalizationFile{LocalizationDataType}.DataList"/>
                /// <br/><br/> 
                /// Checks if there at least 1 suitable <see langword="LocalizationDataType"/> object in <see cref="LimbusLocalizationFile{LocalizationDataType}.DataList"/> using defined <see cref="DataListValidator"/>
                /// <br/><br/>
                /// If successful:<br/>
                /// 1. Sets <see cref="CurrentEditorMode"/> as this mode<br/>
                /// 2. Sets <see cref="CurrentFile"/> and <see cref="DeserializedLocalizationData"/> values<br/>
                /// 3. Calls <see cref="SwitchUI"/>
                /// </summary>
                public void TryValidateJsonAndSwitchMode(FileInfo TargetFile, string OpenFile_TypeCheckName /* Without "EN_", "KR_", "JP_" prefixes */)
                {
                    string Text = File.ReadAllText(TargetFile.FullName);
                    LimbusLocalizationFile<LocalizationDataType> TestDeserialized = Text.DeserealizeJsonAs<LimbusLocalizationFile<LocalizationDataType>>()!;

                    if (TestDeserialized.DataList.Any(DataListValidator))
                    {
                        @EditorModesShelf.CurrentEditorMode = this;

                        @Languages.PresentedTextFields["[Main UI] * Json Path"].Text = TargetFile.FullName;

                        this.CurrentFile = TargetFile;
                        this.CheckFileName = OpenFile_TypeCheckName.RemovePostfix(".json");
                        this.CurrentFileSpecific = new(TargetFile.EncodingHasBOM(), Text.GetJsonIndentationSize(), Text.DetermineLineBreakType());

                        this.DeserializedLocalizationData = TestDeserialized;

                        SwitchUI();
                    }
                }








                /// <summary>
                /// Must call <see cref="TMProEmitter.RefreshRichText"/> for all TMProEmitters defined in the rich text view for this mode.
                /// <br/><br/>
                /// Called via <see cref="@PartialStateUpdater.Limbus.FullyRefreshShownRichText"/> when factors affecting rich text change.<br/>
                /// <c>[!!!]</c> The above method also calls <see cref="@CurrentPreviewCreator.RebuildTextElements"/> to perform the same rich text refreshing in Preview Creator, <u>so it should be used in general</u> instead of just this method if the rich text update occurs outside the json file editor.
                /// </summary>
                public abstract void RefreshRichText();


                /// <summary>
                /// Called when titlebar button with canvas icon was pressed, can be performed via <see cref="Instruments.WPFTools.RenderImage(FrameworkElement, double, bool, bool)"/>
                /// </summary>
                public virtual void ScreenshotRichText() { }

                protected class ScreenshotBackgroundSetter : IDisposable
                {
                    private object TargetElement { get; }
                    public ScreenshotBackgroundSetter(object Target)
                    {
                        this.TargetElement = Target;
                        TargetElement.SetPropertyValue<Brush>("Background", ToSolidColorBrush(LoadedConfiguration.ScanParameters.BackgroundColor));
                    }
                    public void Dispose()
                    {
                        TargetElement.SetPropertyValue<Brush>("Background", Brushes.Transparent);
                        GC.SuppressFinalize(this);
                    }
                }

                [LayeredComponent]
                protected string ScanPathTemplate => @$"[⇲] Assets Directory\Scans\[$1], ID [$2] @ {DateTime.Now:HHːmmːss (dd.MM.yyyy)}.png";

                [LayeredComponent]
                protected double ScreenshotsUpscale => LoadedConfiguration.ScanParameters.ScaleFactor;








                /// <summary>
                /// Current file accepted by <see cref="TryValidateJsonAndSwitchMode"/> after successful validation
                /// </summary>
                public FileInfo? CurrentFile { get; private set; }


                /// <summary>
                /// File specific generated by <see cref="TryValidateJsonAndSwitchMode"/> when validation is successful and file was readed<br/><br/>
                /// Used in <see cref="SaveCurrentFile_Entry"/> method to keep original formatting when serializing json
                /// </summary>
                public FileSpecific? CurrentFileSpecific { get; private set; }


                /// <summary>
                /// Json file deserialized to <see cref="LimbusLocalizationFile{LocalizationDataType}"/> type
                /// </summary>
                [LayeredComponent]
                public LimbusLocalizationFile<LocalizationDataType>? DeserializedLocalizationData { get; private set; }


                /// <summary>
                /// Formed at the moment of type checking in <see cref="MainWindow.OpenJsonFileButton_Click"/>, may represent string returned by the <see cref="LimbusLocalizationFile{LocalizationDataType}.TryMatchManualFileType"/> method
                /// </summary>
                [LayeredComponent]
                public string? CheckFileName { get; private set; }


                /// <summary>
                /// Set from <see cref="SaveCurrentFile_Action"/> when <see cref="DeserializedLocalizationData"/> data was saved to json file
                /// </summary>
                public string? RecentlySerializedJsonText { get; private set; }




                /// <summary>
                /// For <see cref="MainWindow.UnsavedChangesDialog_ShowMenu"/>
                /// </summary>
                public abstract (bool IsAnyUnsavedChanges, string UnsavedChangesText) CollectUnsavedChanges();








                /// <summary>
                /// Sets IsEnabled for <see cref="MainWindow.RightMenu_PreviousObjectSwitchButton"/> and <see cref="MainWindow.RightMenu_NextObjectSwitchButton"/> based on the given conditions<br/>
                /// </summary>
                [LayeredComponent]
                protected void CheckSwitchIDButtonsAvailability(int IndexOfCurrentID, int IDsMaxIndex)
                {
                    MainWindowInstance.RightMenu_PreviousObjectSwitchButton.IsEnabled = IndexOfCurrentID > 0;
                    MainWindowInstance.RightMenu_NextObjectSwitchButton.IsEnabled = IndexOfCurrentID < IDsMaxIndex;
                }


                /// <summary>
                /// Called by <see cref="MainWindow.RightMenu_IDSwitch_NextOrPrevious"/> (ID switch buttons <b>PreviewMouseLeftButtonDown</b> event), must switch to next or previous object
                /// </summary>
                public abstract void SwitchIDButtonClick(IDSwitchDirection Direction);


                /// <summary>
                /// Called by <see cref="MainWindow.RightMenu_IDSwitch_NextOrPrevious_ToVeryFirstOrLast"/> (ID switch buttons <b>PreviewMouseRightButtonDown</b> event), must switch to the very first or last object
                /// </summary>
                public abstract void SwitchIDButtonClick_ToVeryFirstOrLast(IDSwitchDirection Direction);




                /// <summary>
                /// Called by <see cref="MainWindow.NavigationPanel_IDSwitch_ManualInput_Confirm"/> when <b>Enter</b> was pressed while <see cref="MainWindow.IDManage_ManualInput"/> is available by <see cref="MainWindow.RightMenu_ManualIDInput_Start"/> (Line breaks (<c>\n</c>, <c>\r</c>) are unescaped)
                /// <br/><br/>
                /// <paramref name="Input"/> is <see cref="MainWindow.IDManage_ManualInput"/>'s Text
                /// </summary>
                public abstract void SwitchToObjectByInput(string Input);








                /// <summary>
                /// Should recheck unsaved change markers on buttons from right menu (Switch to main desc, ...)<br/>
                /// Publicly used by theme loading when the color of this marker changes (<see cref="@Themes.LoadFromFile"/>)
                /// </summary>
                public abstract void ChangeAllRightMenuUnsavedChangesMarkers_OnButtons();


                /// <summary>
                /// Should recheck unsaved change markers on text inputs from right menu (Name input, ...)<br/>
                /// Publicly used by theme loading when the color of this marker changes (<see cref="@Themes.LoadFromFile"/>)
                /// </summary>
                public abstract void ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs();
                

                /// <summary>
                /// Called from <see cref="MainWindow.TextEditor_TextChanged"/>, assumes dynamic set of the unsaved changes marker on the buttons in the right menu depending on the current <see cref="TextDocument"/> and individual conditions of each mode
                /// <br/><br/>
                /// Also called by <see cref="SaveCurrentFile_Entry"/> after calling <see cref="SaveCurrentDescription"/> to automatically update unsaved changes markers in the Right Menu
                /// </summary>
                public abstract void ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(TextDocument CurrentDocument);








                /// <summary>
                /// Called from <see cref="MainWindow.MainWindow_PreviewMouseDown"/> before ID switch by mouse <see cref="MouseButton.XButton1"/> or  <see cref="MouseButton.XButton2"/>, it can be cancelled by <see langword="ref"/> <see langword="bool"/> <paramref name="CancelIDSwitchByMouseXButtons"/><br/>
                /// If id switch is not cancelled by <see langword="ref"/> <see langword="bool"/> <paramref name="CancelIDSwitchByMouseXButtons"/>, <see cref="SwitchIDButtonClick"/> will be called further
                /// </summary>
                public virtual void WindowPreviewMouseDown(MouseButtonEventArgs Args, ref bool CancelIDSwitchByMouseXButtons) { }


                /// <summary>
                /// Called from <see cref="MainWindow.MainWindow_PreviewKeyDown"/> before ID switch by keyboard <see cref="Key.Left"/> or <see cref="Key.Right"/> press, it can be cancelled by <see langword="ref"/> <see langword="bool"/> <paramref name="CancelIDSwitchByKeyboardButtons"/><br/>
                /// If id switch is not cancelled by <see langword="ref"/> <see langword="bool"/> <paramref name="CancelIDSwitchByKeyboardButtons"/>, <see cref="SwitchIDButtonClick"/> will be called further
                /// </summary>
                public virtual void WindowPreviewKeyDown(KeyEventArgs Args, ref bool CancelIDSwitchByKeyboardButtons) { }








                /// <summary>
                /// Called by <see cref="SaveCurrentFile_Entry"/> before the file is actually saved<br/>
                /// Must define a set of actions to perform on the <see cref="DeserializedLocalizationData"/> json data before it is saved to a file
                /// <br/><br/>
                /// After this method is executed in <see cref="SaveCurrentFile_Entry"/>, <see cref="ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc"/> is called to automatically update unsaved changes markers in the Right Menu and <see cref="SaveCurrentFile_Action"/> to actually save the file, execution of those 2 last methods in <see cref="SaveCurrentFile_Entry"/> will be prevented when <see langword="ref"/> <see langword="bool"/> <paramref name="CancelDefaultSave"/> from this method was set to <see langword="true"/>
                /// </summary>
                [LayeredComponent]
                public abstract void SaveCurrentDescription(TextDocument CurrentDocument, ref bool CancelDefaultSave);


                /// <summary>
                /// Called by <see cref="MainWindow.MainWindow_PreviewKeyDown"/> when both <b>LeftCtrl</b> and <b>S</b> is pressed (Only once per press, it doesn't jam)
                /// <br/><br/>
                /// Sequentially calls <see cref="SaveCurrentDescription"/> and <see cref="ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc"/> before saving file via <see cref="SaveCurrentFile_Action"/> (&lt;--- main saving method)<br/><br/>
                /// Methods after <see cref="SaveCurrentDescription"/> will not be executed if <see langword="ref"/> <see langword="bool"/> 'CancelDefaultSave' from it was set to <see langword="true"/>
                /// </summary>
                public void SaveCurrentFile_Entry()
                {
                    if (CurrentFile is not null /*Then 'Deserialized' is not null either*/)
                    {
                        bool CancelDefaultSave = false;
                        SaveCurrentDescription(MainWindowInstance.LimbusJsonTextEditor.Document, ref CancelDefaultSave);

                        if (CancelDefaultSave == false)
                        {
                            ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(MainWindowInstance.LimbusJsonTextEditor.Document);

                            SaveCurrentFile_Action();
                        }
                    }
                }


                /// <summary>
                /// Normally called at the end of <see cref="SaveCurrentFile_Entry"/> right after <see cref="ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc"/> if <see langword="ref"/> <see langword="bool"/> 'CancelDefaultSave' from <see cref="SaveCurrentDescription"/> was not set to <see langword="true"/>
                /// <br/><br/>
                /// Saves <see cref="DeserializedLocalizationData"/> to json file by <see cref="CurrentFile"/> fullname using <see cref="CurrentFileSpecific"/> for formatting
                /// <br/><br/>
                /// <see cref="DeserializedLocalizationData"/> and <see cref="CurrentFile"/> is considered as not <see langword="null"/> at this moment
                /// </summary>
                [LayeredComponent]
                public void SaveCurrentFile_Action()
                {
                    string JsonText = DeserializedLocalizationData!.SerializeToFormattedJsonText(
                        IndentationSize: CurrentFileSpecific!.IndentationSize,
                        LineBreakMode: CurrentFileSpecific!.LineBreaks
                    );

                    this.RecentlySerializedJsonText = JsonText;

                    if (CurrentFile!.Directory!.Exists == false)
                    {
                        CurrentFile!.Directory.Create();
                    }

                    File.WriteAllText(CurrentFile!.FullName, JsonText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: CurrentFileSpecific.IsBOM));
                }








                /// <summary>
                /// Called at the very start of <see cref="SwitchUI"/> before all actions<br/><br/>
                /// Assumes the content of individual init actions for each editor mode (Document set for <see cref="MainWindow.LimbusJsonTextEditor"/>, some buttons availability check, etc...)
                /// </summary>
                [LayeredComponent]
                public abstract void UISwitchPrecedingActions();


                /// <summary>
                /// Order of calls:<br/>
                /// 1. <see cref="UISwitchPrecedingActions"/><br/>
                /// 2. <see cref="AdjustMainWindow"/> (Adjust MainWindow according to the defined <see cref="Dimensing"/> values)<br/>
                /// <br/>
                /// Normally called by <see cref="TryValidateJsonAndSwitchMode"/> when confirmed that json is suitable for editing and values for <see cref="CurrentFile"/> and <see cref="DeserializedLocalizationData"/> was set
                /// </summary>
                [LayeredComponent]
                public void SwitchUI()
                {
                    UISwitchPrecedingActions();

                    AdjustMainWindow();
                }


                /// <summary>
                /// Adjust MainWindow according to the defined <see cref="Dimensing"/> values
                /// </summary>
                [LayeredComponent]
                public void AdjustMainWindow()
                {
                    // MainWindow's ResourceDictionary contains "DM:abc" values with corresponding types and names, which are used as {DynamicResource}
                    foreach (FieldInfo DimensingProperty in DimensingFields)
                    {
                        MainWindowInstance.Resources[$"DM:{DimensingProperty.Name}"] = DimensingProperty.GetValue(obj:Dimensing);
                    }

                    // Idk why it isn't applied according to the DynamicResources set
                    MainWindowInstance.MaxWidth = this.Dimensing.MaxWidth;
                    MainWindowInstance.MinWidth = this.Dimensing.MinWidth;
                    MainWindowInstance.MinHeight = this.Dimensing.MinHeight;

                    MainWindowInstance.Width = this.Dimensing.Width;
                    MainWindowInstance.Height = this.Dimensing.Height;
                }








                /// <summary>
                /// Used by <see cref="MainWindow.ModeSwitchContextMenu_Click"/> when clicked by <see cref="MainWindow.ModeSwitchButton_DiscardCurrentFile"/>
                /// </summary>
                public void DiscardCurrentFile()
                {
                    DeserializedLocalizationData = null;
                    CurrentFile = null;
                    CheckFileName = null;
                    ClearSpecificData();
                }

                /// <summary>
                /// Must clear any additional data from readed file outside the abstract class
                /// </summary>
                [LayeredComponent]
                protected abstract void ClearSpecificData();
            }
        }
    }
}


// Common
namespace LCLocalizationInterface
{
    public partial class MainWindow
    {
        #region Active editor modes switching
        private void ModeSwitchContextMenu_OpenOnLeftClick(object Sender, RoutedEventArgs Args)
        {
            ModeSwitchContextMenu_Opening(null!, null!);
            (Sender as Button)!.ContextMenu.IsOpen = true;
        }
        private void ModeSwitchContextMenu_Opening(object Sender, ContextMenuEventArgs Args)
        {
            ModeSwitchButton_DiscardCurrentFile.IsEnabled = @EditorModesShelf.CurrentEditorMode != @EditorModesShelf.MainMenu & @CurrentPreviewCreator.ActiveState == false;

            ModeSwitchButton_Skills.IsEnabled = @EditorModesShelf.Skills.CurrentFile is not null;
            ModeSwitchButton_Passives.IsEnabled = @EditorModesShelf.Passives.CurrentFile is not null;
            ModeSwitchButton_Keywords.IsEnabled = @EditorModesShelf.Keywords.CurrentFile is not null;
            ModeSwitchButton_EGOGifts.IsEnabled = @EditorModesShelf.EGOGifts.CurrentFile is not null;
            ModeSwitchButton_ObservationLogs.IsEnabled = @EditorModesShelf.ObservationLogs.CurrentFile is not null;

            static void SetFileDetail(string UIDPart, @EditorModesShelf.Types.EditorModeIntermediator TargetMode)
            {
                @Languages.ExternElementConditional(
                    UID: $"[Main UI] * Current editor switch — {UIDPart}",
                    Condition: TargetMode.CurrentFile is not null,
                    FailureVariableKey: "Default",
                    SuccessVariableKey: "With file",
                    SuccessExternString: TargetMode.CurrentFile?.Name!
                );
            }
            SetFileDetail("Skills", @EditorModesShelf.Skills);
            SetFileDetail("Passives", @EditorModesShelf.Passives);
            SetFileDetail("Keywords", @EditorModesShelf.Keywords);
            SetFileDetail("E.G.O Gifts", @EditorModesShelf.EGOGifts);
            SetFileDetail("Observation Logs", @EditorModesShelf.ObservationLogs);
        }
        private void ModeSwitchContextMenu_Click(object Sender, RoutedEventArgs Args)
        {
            if (@CurrentPreviewCreator.ActiveState)
            {
                PreviewCreatorPage.SwitchUI_Return(CompleteAction: ModeSwitching);
            }
            else
            {
                ModeSwitching();
            }

            void ModeSwitching()
            {
                static void DoActualSwitch(@EditorModesShelf.Types.EditorModeIntermediator TargetMode, Action SwitchingToObject)
                {
                    @EditorModesShelf.CurrentEditorMode = TargetMode;
                    @Languages.PresentedTextFields["[Main UI] * Json Path"].Text = TargetMode.CurrentFile!.FullName;
                    using (TMProEmitter.DisabledRichTextDelay)
                    {
                        SwitchingToObject.Invoke();
                        @EditorModesShelf.CurrentEditorMode.RefreshRichText();
                    }
                    TargetMode.AdjustMainWindow();
                }
                switch ((Sender as MenuItem_T1)!.Name)
                {
                    case nameof(ModeSwitchButton_MainMenu):
                        @EditorModesShelf.CurrentEditorMode = @EditorModesShelf.MainMenu;
                        @Languages.PresentedTextFields["[Main UI] * Json Path"].Text = "";
                        MainWindowInstance.LimbusJsonTextEditor.Document = DataContextDomain.Editor.MainMenuDocument!;
                        DataContextDomain.Editor.CurrentObjectID = @Languages.VariableData.InsertionsDefaultValue;
                        RightMenu_PreviousObjectSwitchButton.IsEnabled = false;
                        RightMenu_NextObjectSwitchButton.IsEnabled = false;
                        @EditorModesShelf.MainMenu.AdjustMainWindow();
                        break;

                    case nameof(ModeSwitchButton_Skills):
                        DoActualSwitch(@EditorModesShelf.Skills, delegate () { @EditorModesShelf.Skills.SwitchToSkill(@EditorModesShelf.Skills.CurrentSkillID, @EditorModesShelf.Skills.CurrentUptieNumber); });
                        break;

                    case nameof(ModeSwitchButton_Passives):
                        DoActualSwitch(@EditorModesShelf.Passives, delegate () { @EditorModesShelf.Passives.SwitchToPassive(@EditorModesShelf.Passives.CurrentPassiveID); });
                        break;

                    case nameof(ModeSwitchButton_Keywords):
                        DoActualSwitch(@EditorModesShelf.Keywords, delegate () { @EditorModesShelf.Keywords.SwitchToKeyword(@EditorModesShelf.Keywords.CurrentKeywordID); });
                        break;

                    case nameof(ModeSwitchButton_EGOGifts):
                        DoActualSwitch(@EditorModesShelf.EGOGifts, delegate () { @EditorModesShelf.EGOGifts.SwitchToEGOGift(@EditorModesShelf.EGOGifts.CurrentEGOGiftID); });
                        break;

                    case nameof(ModeSwitchButton_ObservationLogs):
                        DoActualSwitch(@EditorModesShelf.ObservationLogs, delegate () { @EditorModesShelf.ObservationLogs.SwitchToObservationLog(@EditorModesShelf.ObservationLogs.CurrentObservationLogID); });
                        break;


                    case nameof(ModeSwitchButton_DiscardCurrentFile):
                        if (@EditorModesShelf.CurrentEditorMode != @EditorModesShelf.MainMenu)
                        {
                            string DialogTitle = @Languages.GetLocalizationTextFor("[Main UI] * Current editor switch — Discard current file <Confirm Dialog>", "Title");
                            string DialogText = @Languages.GetLocalizationTextFor("[Main UI] * Current editor switch — Discard current file <Confirm Dialog>", "Text");
                            ConfirmDialog.ConfirmDialogInstance.ShowConfirmDialog(DialogTitle, DialogText, delegate ()
                            {
                                void DiscardFileAndSwitchToMainMenu()
                                {
                                    EditorModeIntermediator PreviousEditor = @EditorModesShelf.CurrentEditorMode;
                                    ModeSwitchContextMenu_Click(Sender: ModeSwitchButton_MainMenu, Args: null!);
                                    PreviousEditor.DiscardCurrentFile(); // Current is main menu after the ModeSwitchContextMenu_Click
                                }

                                (bool IsAnyUnsavedChanges, string UnsavedChangesText) = @EditorModesShelf.CurrentEditorMode.CollectUnsavedChanges();
                                if (IsAnyUnsavedChanges)
                                {
                                    UnsavedChangesDialog_ShowMenu(Text: UnsavedChangesText, ProceedAction: DiscardFileAndSwitchToMainMenu);
                                }
                                else
                                {
                                    DiscardFileAndSwitchToMainMenu();
                                }
                            });
                        }
                        break;
                }
            }
        }
        #endregion



        #region ID copying

        #region Copying out-in fade animation setup
        private static bool AlreadyAnimatingCopiedInfo = false;
        private const double AnimPauseTime = 0.35;
        private static readonly Duration FadeInOutTimeDuration = new(TimeSpan.FromSeconds(0.118));
        private static readonly DoubleAnimation FadeIn_IDSign = new() { From = 0, To = 1, Duration = FadeInOutTimeDuration };
        private static readonly DoubleAnimation FadeOut_IDCopiedText = new() { From = 1, To = 0, Duration = FadeInOutTimeDuration };
        private static readonly DoubleAnimation FadeIn_IDCopiedText = new() { From = 0, To = 1, Duration = FadeInOutTimeDuration };
        private static readonly DoubleAnimation FadeOut_IDSign = new() { From = 1, To = 0, Duration = FadeInOutTimeDuration };
        private void SetupIDCopyAnimation()
        {
            /// FadeOut_IDSign -> FadeIn_IDCopiedText -> Pause (AnimPauseTime) -> FadeOut_IDCopiedText -> FadeIn_IDSign

            // Hide ID, show 'ID Copied' text
            FadeOut_IDSign.Completed += (_, _) => IDManage_CopiedBlink.BeginAnimation(OpacityProperty, FadeIn_IDCopiedText);
            FadeIn_IDCopiedText.Completed += (_, _) => Await(AnimPauseTime, delegate () { IDManage_CopiedBlink.BeginAnimation(OpacityProperty, FadeOut_IDCopiedText); });

            // Hide 'ID Copied' text, show ID
            FadeOut_IDCopiedText.Completed += (_, _) => IDManage_Display.BeginAnimation(OpacityProperty, FadeIn_IDSign);
            FadeIn_IDSign.Completed += (_, _) => AlreadyAnimatingCopiedInfo = false;
        }
        #endregion


        private void CopyID_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            if (IDManage_ManualInput.Visibility == Visibility.Collapsed)
            {
                if (@EditorModesShelf.CurrentEditorMode != @EditorModesShelf.MainMenu)
                {
                    try
                    {
                        if (!AlreadyAnimatingCopiedInfo)
                        {
                            Clipboard.SetDataObject(IDManage_Display.Text); // SetText() causes exceptions sometimes
                            AlreadyAnimatingCopiedInfo = true;
                            IDManage_Display.BeginAnimation(OpacityProperty, FadeOut_IDSign);
                        }
                    }
                    catch (Exception Occurred)
                    {
                        AlreadyAnimatingCopiedInfo = false;
                        ErrorMessageWindow.ShowException(Occurred, "Exception occurred while copying the ID or animation of this process");
                    }
                }
            }
        }
        #endregion




        #region ID switch on mouse Forward/Back buttons
        private void MainWindow_PreviewMouseDown(object Sender, MouseButtonEventArgs Args)
        {
            bool CancelIDSwitchByMouseXButtons = false;
            @EditorModesShelf.CurrentEditorMode.WindowPreviewMouseDown(Args, ref CancelIDSwitchByMouseXButtons);

            // ID switch on Forward/Back mouse buttons
            if (CancelIDSwitchByMouseXButtons == false)
            {
                if (Args.ChangedButton is MouseButton.XButton1 & RightMenu_PreviousObjectSwitchButton.IsEnabled)
                {
                    @EditorModesShelf.CurrentEditorMode.SwitchIDButtonClick(@EditorModesShelf.IDSwitchDirection.Back);
                }
                else if (Args.ChangedButton is MouseButton.XButton2 & RightMenu_NextObjectSwitchButton.IsEnabled)
                {
                    @EditorModesShelf.CurrentEditorMode.SwitchIDButtonClick(@EditorModesShelf.IDSwitchDirection.Forward);
                }
            }
        }
        #endregion




        #region Right menu
        public void RightMenuTextfields_TextChanged(object Sender, RoutedEventArgs Args)
        {
            IntenseStareType3 ActualSender = (Sender as IntenseStareType3)!;

            if (ActualSender.UID != "[Main UI] * Manual ID Input" &
                ActualSender.UID != "[Main UI] * Json Path" &
                ActualSender.UID.StartsWith("[Keywords / Right Menu] * Format Insertion") == false
            ) {
                TryStartAutoSaveTimer();
            }
        }

        #region ID switch buttons
        private void RightMenu_IDSwitch_NextOrPrevious(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.IDSwitchDirection Direction = Enum.Parse<@EditorModesShelf.IDSwitchDirection>((Sender as Button)!.Uid);

            @EditorModesShelf.CurrentEditorMode.SwitchIDButtonClick(Direction);
        }

        private bool SwitchedToVeryLastJustNow = false;
        private async void RightMenu_IDSwitch_NextOrPrevious_ToVeryFirstOrLast(object Sender, RoutedEventArgs Args)
        {
            @EditorModesShelf.IDSwitchDirection Direction = Enum.Parse<@EditorModesShelf.IDSwitchDirection>((Sender as Button)!.Uid);

            @EditorModesShelf.CurrentEditorMode.SwitchIDButtonClick_ToVeryFirstOrLast(Direction);
            if (Direction == @EditorModesShelf.IDSwitchDirection.Forward)
            {
                SwitchedToVeryLastJustNow = true;
                await Task.Delay(250); /// Delay for right click and json managing context menu open when IsEnabled == false (<see cref="RightMenu_NextObjectSwitchButton_JsonManagingContextMenuOpening"/>)
                SwitchedToVeryLastJustNow = false;
            }
        }
        #endregion



        #region Manual ID input
        private async void RightMenu_ManualIDInput_Start(object Sender, MouseButtonEventArgs Args)
        {
            if (@EditorModesShelf.CurrentEditorMode != @EditorModesShelf.MainMenu)
            {
                IDManage_Display.Visibility = Visibility.Collapsed;
                IDManage_CopiedBlink.Visibility = Visibility.Collapsed;
                IDManage_ManualInput.Visibility = Visibility.Visible;

                await Task.Delay(50); // . . . .
                Keyboard.Focus(IDManage_ManualInput);
            }
        }
        private void NavigationPanel_IDSwitch_ManualInput_Stop()
        {
            IDManage_Display.Visibility = Visibility.Visible;
            IDManage_CopiedBlink.Visibility = Visibility.Visible;
            IDManage_ManualInput.Visibility = Visibility.Collapsed;
            IDManage_ManualInput.Text = "";

            FocusManager.SetFocusedElement(this, WindowFocusManagingPlaceholder);
        }
        private void NavigationPanel_IDSwitch_ManualInput_Confirm()
        {
            @EditorModesShelf.CurrentEditorMode.SwitchToObjectByInput(IDManage_ManualInput.Text.Replace("\\r", "\r").Replace("\\n", "\n"));
            NavigationPanel_IDSwitch_ManualInput_Stop();
        }
        #endregion



        #region Keywords format insertions
        private void ChangeKeywordsFormatInsertions(object Sender, EventArgs Args)
        {
            IntenseStareType3 ActualSender = (IntenseStareType3)Sender;
            int Number = int.Parse($"{ActualSender.UID[^2]}");
            InputRichTextFormatter.FormatInsertions[Number] = ActualSender.Text != "" ? ActualSender.Text : $"{{{Number}}}";
            if (@EditorModesShelf.Keywords.CheckFileName!.StartsWith("Bufs")) @EditorModesShelf.Keywords.RefreshRichText();
        }
        private void ToggleKeywordsFormatInsertionsDropDown(object Sender, MouseButtonEventArgs Args)
        {
            bool IsAlreadyToggled = FormatInsertionsDropDownIndicator.Angle != 0;

            FormatInsertionsDropDownIndicator.Angle = IsAlreadyToggled ? 0 : 180;
            FormatInsertionsDropDownStackPanel.Visibility = IsAlreadyToggled ? Visibility.Collapsed : Visibility.Visible;
        }
        #endregion

        #endregion




        #region Json text editor (TextChanged event + Context menu buttons)
        private void TextEditor_TextChanged(object Sender, EventArgs Args)
        {
            @EditorModesShelf.CurrentEditorMode.ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(LimbusJsonTextEditor.Document);

            TryStartAutoSaveTimer();
        }


        public void TextEditor_ContextMenuClick(object Sender, RoutedEventArgs Args)
        {
            string SelectedTextToEdit = LimbusJsonTextEditor.SelectedText;
            string AutoDetectionPattern = SelectedLimbusCustomLanguage.Keywords_AutodetectionRegex;
            bool CanUseAutoDetection = !string.IsNullOrWhiteSpace(AutoDetectionPattern) && AutoDetectionPattern.Contains("KeywordNameWillBeHere");


            switch ((Sender as MenuItem_T1)!.Uid)
            {
                case "Insert Style":
                    SelectedTextToEdit = $"<style=\"{(@EditorModesShelf.CurrentEditorMode == @EditorModesShelf.EGOGifts ? "upgradeHighlight" : "highlight")}\">{SelectedTextToEdit}</style>";
                    break;



                /* ------------------------------------------------------------------------------------------------------------ */



                case "<TMPro> To [KeywordID]":

                    SelectedTextToEdit = InputRichTextFormatter.TMProKeywordPattern.Replace(SelectedTextToEdit, Match =>
                    {
                        string ID = Match.Groups["DescID"].Value;
                        return KeywordsLoader.LoadedKeywords_Bufs.ContainsKey(ID) ? $"[{ID}]" : Match.Value;
                    });

                    break;




                case "<TMPro> To Shorthands":

                    if (!string.IsNullOrWhiteSpace(SelectedLimbusCustomLanguage.Shorthands_InsertionShape))
                    {
                        SelectedTextToEdit = InputRichTextFormatter.TMProKeywordPattern.Replace(SelectedTextToEdit, Match =>
                        {
                            string ID = Match.Groups["DescID"].Value;

                            return KeywordsLoader.LoadedKeywords_Bufs.TryGetValue(ID, out PlainKeyword? FoundKeywordInfo)
                                ? SelectedLimbusCustomLanguage.Shorthands_InsertionShape
                                    .Replace("<KeywordID>", ID)
                                    .Replace("<KeywordName>", FoundKeywordInfo.Name)
                                : Match.Value;
                        });
                    }

                    break;




                case "Implicit To [KeywordID]":

                    if (CanUseAutoDetection)
                    {
                        foreach (KeyValuePair<string, string> KeywordNameAndIDPair in KeywordsLoader.LimbusKeywords_ImplicitConversionOrder)
                        {
                            string KeywordName = KeywordNameAndIDPair.Key;
                            string KeywordID = KeywordNameAndIDPair.Value;

                            if (SelectedTextToEdit.Contains(KeywordName))
                            {
                                SelectedTextToEdit = Regex.Replace(SelectedTextToEdit, AutoDetectionPattern.Replace(@"KeywordNameWillBeHere", Regex.Escape(KeywordName)), Match =>
                                {
                                    return $"[{KeywordID}]";
                                });
                            }
                        }
                    }

                    break;




                case "Implicit To Shorthands":

                    if (CanUseAutoDetection && !string.IsNullOrWhiteSpace(SelectedLimbusCustomLanguage.Shorthands_InsertionShape))
                    {
                        foreach (KeyValuePair<string, string> KeywordNameAndIDPair in KeywordsLoader.LimbusKeywords_ImplicitConversionOrder)
                        {
                            string KeywordName = KeywordNameAndIDPair.Key;
                            string KeywordID = KeywordNameAndIDPair.Value;

                            if (SelectedTextToEdit.Contains(KeywordName))
                            {
                                SelectedTextToEdit = Regex.Replace(SelectedTextToEdit, AutoDetectionPattern.Replace(@"KeywordNameWillBeHere", Regex.Escape(KeywordName)), Match =>
                                {
                                    string Shorthand = SelectedLimbusCustomLanguage.Shorthands_InsertionShape
                                        .Replace("<KeywordID>", KeywordID)
                                        .Replace("<KeywordName>", KeywordName.Replace(" ", "[\0 Placeholder to avoid repeated simillar keywords regex matches in this string \0]"));

                                    return Shorthand;
                                });
                            }
                        }

                        SelectedTextToEdit = SelectedTextToEdit.Replace("[\0 Placeholder to avoid repeated simillar keywords regex matches in this string \0]", " ");
                    }

                    break;




                case "[KeywordID] To Shorthands":

                    if (!string.IsNullOrWhiteSpace(SelectedLimbusCustomLanguage.Shorthands_InsertionShape))
                    {
                        foreach (PlainKeyword KeywordInfo in KeywordsLoader.LoadedKeywords_Bufs.Values)
                        {
                            if (SelectedTextToEdit.Contains($"[{KeywordInfo.ID}]"))
                            {
                                string Shorthand = SelectedLimbusCustomLanguage.Shorthands_InsertionShape
                                    .Replace("<KeywordID>", KeywordInfo.ID)
                                    .Replace("<KeywordName>", KeywordInfo.Name);

                                SelectedTextToEdit = SelectedTextToEdit.Replace($"[{KeywordInfo.ID}]", Shorthand);
                            }
                        }
                    }

                    break;




                case "[KeywordID] To <TMPro>":

                    foreach (PlainKeyword KeywordInfo in KeywordsLoader.LoadedKeywords_Bufs.Values)
                    {
                        if (SelectedTextToEdit.Contains($"[{KeywordInfo.ID}]"))
                        {
                            string ID = KeywordInfo.ID!;
                            string SpriteID = ID;
                            string Name = KeywordInfo.Name!;
                            string Color = ColorDictionaries.LoadedKeywordColors[KeywordInfo.ID!];

                            if (ImageDictionaries.NotSuitableForSpriteTagRedirections.TryGetValue(ID, out string? AnotherSpriteID))
                            {
                                SpriteID = AnotherSpriteID;
                            }

                            string TMProKeyword = $"<sprite name=\"{SpriteID}\"><color={Color}><u><link=\"{ID}\">{Name}</link></u></color>";

                            SelectedTextToEdit = SelectedTextToEdit.Replace($"[{KeywordInfo.ID}]", TMProKeyword);
                        }
                    }

                    break;




                default:

                    MenuItem_T1 ActualSender = (Sender as MenuItem_T1)!;

                    if (ActualSender.DataContext is List<ExtraReplacementsContextMenu.RegexReplaceOption> RegexReplacements) // Extra Replacements
                    {
                        foreach (ExtraReplacementsContextMenu.RegexReplaceOption RegexReplacement in RegexReplacements)
                        {
                            SelectedTextToEdit = RegexReplacement.RegularExpression.Replace(SelectedTextToEdit, RegexReplacement.Replacement);
                        }
                    }

                    break;
            }

            if (LimbusJsonTextEditor.SelectedText != SelectedTextToEdit)
            {
                LimbusJsonTextEditor.SelectedText = SelectedTextToEdit;
            }
        }
        #endregion




        #region Unsaved changes menu
        /// <summary><paramref name="ProceedAction"/> executed at the <see cref="UnsavedChangesDialog_ButtonsClick_Proceed"/></summary>
        private void UnsavedChangesDialog_ShowMenu(string Text, Action ProceedAction)
        {
            UnsavedChangesDialog_ProceedAction = ProceedAction;
            @Languages.PresentedTextElements["[Unsaved Changes] [-] * Information Text"].RichText = @Themes.CurrentTheme.ColorKeysForTranslation.Apply(Text);
            UnsavedChangesGrid.Visibility = Visibility.Visible;
            LocalizationEditorGrid.Effect = new BlurEffect() { Radius = 10 };
        }
        private void UnsavedChangesDialog_HideMenu()
        {
            UnsavedChangesGrid.Visibility = Visibility.Collapsed;
            LocalizationEditorGrid.Effect = null; // BlurEffect with Radius=0 still adds some annoying micro blur
        }


        /// <summary>Unsaved Changes Dialog - 'Proceed' click</summary>
        private Action UnsavedChangesDialog_ProceedAction = null!;
        private void UnsavedChangesDialog_ButtonsClick_Proceed(object Sender, RoutedEventArgs Args)
        {
            UnsavedChangesDialog_ProceedAction.Invoke();
            UnsavedChangesDialog_HideMenu();
        }
        /// <summary>Unsaved Changes Dialog - 'Cancel' click (Or click on blurred background)</summary>
        private void UnsavedChangesDialog_ButtonsClick_Cancel(object Sender, RoutedEventArgs Args)
        {
            UnsavedChangesDialog_HideMenu();
        }
        #endregion




        #region Autosave
        private static DispatcherTimer AutoSaveTimer = new();
        private static bool AutoSaveTimer_IsPending = false;
        public void TryStartAutoSaveTimer()
        {
            if (TMProEmitter.IsRichTextDelayAllowed)
            {
                if (ProgramFullyLoaded && LoadedConfiguration.Internal.EnableAutoSave == true & AutoSaveTimer_IsPending == false)
                {
                    AutoSaveTimer.Stop();
                    AutoSaveTimer = new() { Interval = TimeSpan.FromSeconds(LoadedConfiguration.Internal.AutosaveDelay) };
                    AutoSaveTimer.Tick += (_, _) =>
                    {
                        @EditorModesShelf.CurrentEditorMode.SaveCurrentFile_Entry();
                        AutoSaveTimer.Stop(); AutoSaveTimer_IsPending = false;
                    };

                    AutoSaveTimer_IsPending = true; AutoSaveTimer.Start();
                }
            }
        }
        #endregion




        #region Descs switch highlight
        // 'object' TargetTMProEmitter because callers is mostly MouseRightButtonDowns with 'object' Sender
        public static void RichTextViews__SHARED_DescriptionFastSwitchHighlight(object TargetTMProEmitter)
        {
            if (LoadedConfiguration.PreviewSettings.Base.HighlightDescsOnRightClick)
            {
                RichTextViews__SHARED_DoBackgroundFadeoutBlinkAnimation(TargetTMProEmitter);
            }
        }
        public static void RichTextViews__SHARED_DescriptionManualSwitchHighlight(object TargetTMProEmitter)
        {
            if (LoadedConfiguration.PreviewSettings.Base.HighlightDescsOnManualSwitch)
            {
                RichTextViews__SHARED_DoBackgroundFadeoutBlinkAnimation(TargetTMProEmitter);
            }
        }

        public static void RichTextViews__SHARED_DoBackgroundFadeoutBlinkAnimation(object TargetTMProEmitter)
        {
            TMProEmitter TargetDesc = (TargetTMProEmitter as TMProEmitter)!;

            TargetDesc.Background = ToSolidColorBrush("#FF262626");
            TargetDesc.Background.BeginAnimation(SolidColorBrush.OpacityProperty, new DoubleAnimation()
            {
                From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(0.6))
            });
            // Not Background ColorAnimation because "object is sealed or freezed"
        }
        #endregion
    }
}