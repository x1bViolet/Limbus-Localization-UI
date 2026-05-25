using ICSharpCode.AvalonEdit.Document;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using static LCLocalizationInterface.LimbusRegistry.InputRichTextFormatter;

namespace LCLocalizationInterface.LimbusRegistry
{
    public ref partial struct @EditorModesShelf
    {
        /// <inheritdoc cref="Types.MainMenuEditorMode"/>
        public static readonly Types.MainMenuEditorMode MainMenu = new();

        public ref partial struct Types
        {
            /// <summary>
            /// Default mode without any jsons, only text from file <c>[⇲] Assets Directory\Default Text.txt</c> set by <see cref="UISwitchPrecedingActions"/><br/>
            /// (<see cref="EditorModeAbstraction{LocalizationDataType}.SwitchUI"/> is called from <see langword="override"/> <see cref="App.OnStartup"/> after <see cref="Configurazione.Load"/>)
            /// </summary>
            public class MainMenuEditorMode : EditorModeAbstraction<LimbusEditorJsonObject>
            {
                public override EditorModeKey Identifier => EditorModeKey.MainMenu;

                protected override MainWindowDimensing Dimensing { get; } = new()
                {
                    MaxWidth = 1000, MinWidth = 713, MinHeight = 425,
                    Width    = 1000, Height   = 600,

                    RichTextViewsHeight             = new GridLength(387),
                    TextEditorAndRichTextViewsWidth = new GridLength(706),

                    RichTextViewsTabIndex     = 0,
                    RightMenuSectionsTabIndex = 0,
                };




                private object SreenshotArea => MainWindowInstance.RichTextViews__MainMenu_COMPOSITION_SurfaceScrollViewer.Content;
                public override void ScreenshotRichText()
                {
                    using (new ScreenshotBackgroundSetter(this.SreenshotArea))
                    {
                        (this.SreenshotArea as FrameworkElement)!.RenderImage(@$"[⇲] Assets Directory\Scans\Main Menu @ {DateTime.Now:HHːmmːss (dd.MM.yyyy)}.png", ScreenshotsUpscale);
                    }
                }
                
                
                public override void RefreshRichText()
                {
                    MainWindowInstance.RichTextViews__MainMenu_DefaultDesc.RefreshRichText();
                }


                public override void UISwitchPrecedingActions()
                {
                    using (TMProEmitter.DisabledRichTextDelay)
                    {
                        // E.G.O Gifts Rich Text format is most neutral (Convert [KeywordID], highlight implicit keywords, do not turn [SomethingElse] to Unknown and ignore [SkillTagID])
                        @DataContextDomain.Editor.MainMenuDocument = LimbusEditorJsonObject.NewDedicatedDocument
                        (
                            Text: TryReadAllText(
                                FilePath: @"[⇲] Assets Directory\Default Text.txt",
                                Fallback: $"                    <font=\"BebasKai SDF\"><size=140%><u>Limbus Company Localization Interface</u> <color=#f8c200>'{App.@Version}</font>\n\nЧерти вылезли из омута"
                            ).Replace("{v:Major}", $"{App.VersionHeading.Major}").Replace("{v:Minor}", $"{App.VersionHeading.Minor}").Replace("{v:Patch}", $"{App.VersionHeading.Patch}"),
                            CarriedSyntaxKey: RichTextFormat.EGOGifts
                        );

                        MainWindowInstance.LimbusJsonTextEditor.Document = @DataContextDomain.Editor.MainMenuDocument;
                    }
                }




                #region Unused editor mode accessors
                protected override Func<LimbusEditorJsonObject, bool> DataListValidator => null!;
                public override List<IntenseStareType3> PresentedRightMenuSytaxedTextInputs => null!;
                public override void SwitchIDButtonClick(IDSwitchDirection Direction) { }
                public override void SwitchIDButtonClick_ToVeryFirstOrLast(IDSwitchDirection Direction) { }
                public override void SwitchToObjectByInput(string Input) { }
                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnButtons() { }
                public override void ChangeAllRightMenuUnsavedChangesMarkers_OnStringInputs() { }
                public override void ChangeRightMenuUnsavedChangesMarkers_OnSelectedDesc(TextDocument CurrentDocument) { }
                public override void SaveCurrentDescription(TextDocument CurrentDocument, ref bool CancelDefaultSave) { }
                public override (bool IsAnyUnsavedChanges, string UnsavedChangesText) CollectUnsavedChanges() => (false, "");
                protected override void ClearSpecificData() { }
                #endregion
            }
        }
    }
}