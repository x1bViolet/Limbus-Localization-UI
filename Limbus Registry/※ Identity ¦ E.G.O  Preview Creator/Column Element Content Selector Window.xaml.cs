using LCLocalizationInterface.Internal.Abstractions;
using LCLocalizationInterface.LimbusRegistry.JsonTypes;
using LCLocalizationInterface.LimbusRegistry.JsonTypes.Specific;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage.ImageInfoJsonFile.TextColumns_PROP;

namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public partial class ColumnElementContentSelectorWindow : DialogWindowBase
    {
        #pragma warning disable CS8618
        public static ColumnElementContentSelectorWindow ColumnElementContentSelectorInstance { get; set; }
        #pragma warning restore CS8618

        public TextElementsColumn CurrentTargetColumn { get; set; } = null!;
        private string SelectedKeywordIcon_Path = "";
        private BitmapImage? SelectedKeywordIcon = null;


        public ColumnElementContentSelectorWindow()
        {
            InitializeComponent();
        }



        #region FadeableWindow
        public override List<Action> AdditionalFadeOutCompleteActions =>
        [
            delegate ()
            {
                ResetSelectedKeywordIcon(null!, null!);

                AssertOKButtonAvailability();

                _ = SkillLocalizationIDSelector.SelectedIndex
                  = SkillConstructorIDSelector.SelectedIndex
                  = PassiveLocalizationIDSelector.SelectedIndex
                  = KeywordLocalizationIDSelector.SelectedIndex
                  = -1;
            }
        ];
        #endregion



        private void ComboBoxes_SelectionChanged(object Sender, SelectionChangedEventArgs Args) => AssertOKButtonAvailability();
        
        private void AssertOKButtonAvailability()
        {
            switch (CurrentSelectorView.SelectedIndex)
            {
                case 0: // Skills
                    ConfirmSelection_Button.IsEnabled = SkillLocalizationIDSelector.SelectedItem is not null & SkillConstructorIDSelector.SelectedItem is not null; break;

                case 1: // Passives
                    ConfirmSelection_Button.IsEnabled = PassiveLocalizationIDSelector.SelectedItem is not null; break;

                case 2: // Keywords
                    ConfirmSelection_Button.IsEnabled = KeywordLocalizationIDSelector.SelectedItem is not null; break;
            }
        }

        private void SelectKeywordIcon(object Sender, RoutedEventArgs Args)
        {
            OpenFileDialog Select = NewOpenFileDialog("Image files", ["jpg", "png"]);
            if (Select.ShowDialog() == true)
            {
                @Languages.ExternElement(
                    UID: "[C] * [Column element creation dialog] Keyword Icon file (Label)",
                    VariableKey: "Selected",
                    ExternObject: Path.GetFileName(Select.FileName)
                );
                SelectedKeywordIcon = BitmapFromFile(Select.FileName);
                SelectedKeywordIcon_Path = Select.FileName;
            }
        }
        private void ResetSelectedKeywordIcon(object Sender, RoutedEventArgs Args)
        {
            @Languages.ExternElement(UID: "[C] * [Column element creation dialog] Keyword Icon file (Label)", VariableKey: "Default");
            SelectedKeywordIcon = null;
            SelectedKeywordIcon_Path = "";
        }






        private void ConfirmSelection_ButtonClick(object Sender, RoutedEventArgs Args)
        {
            switch (CurrentSelectorView.SelectedIndex)
            {
                case 0: // Skills
                    IntenseStareType1 SelectedSkillLabel = (SkillLocalizationIDSelector.SelectedItem as IntenseStareType1)!;
                    IntenseStareType1 SelectedConstructorLabel = (SkillConstructorIDSelector.SelectedItem as IntenseStareType1)!;

                    PreviewCreatorPage.PreviewCreatorPageInstance.AddTextElementToColumn(
                        TargetColumn: this.CurrentTargetColumn,
                        CreatedColumnElement: PreviewCreatorPage.PreviewCreatorPageInstance.CreateSkill(
                            GivenSkillText: (PlainSkill.UptieLevel)SelectedSkillLabel.DataContext,
                            Displaying: (SelectedConstructorLabel.DataContext as SkillConstructor)!,
                            GivenJsonData: new ColumnTextElementData()
                            {
                                Type = PreviewCreatorPage.ColumnTextElementType.Skill,
                                SelectedLocalizationID = $"{SelectedSkillLabel.Uid}",
                                SelectedSkillConstructorID = $"{SelectedConstructorLabel.Uid}"
                            }
                        )
                    );
                    break;


                case 1: // Passives
                    IntenseStareType1 SelectedPassiveLabel = (PassiveLocalizationIDSelector.SelectedItem as IntenseStareType1)!;

                    PreviewCreatorPage.PreviewCreatorPageInstance.AddTextElementToColumn(
                        TargetColumn: this.CurrentTargetColumn,
                        CreatedColumnElement: PreviewCreatorPage.PreviewCreatorPageInstance.CreatePassive(
                            GivenPassiveText: (PlainPassive)SelectedPassiveLabel.DataContext,
                            GivenJsonData: new ColumnTextElementData()
                            {
                                Type = PreviewCreatorPage.ColumnTextElementType.Passive,
                                SelectedLocalizationID = $"{SelectedPassiveLabel.Uid}"
                            }
                        )
                    );
                    break;


                case 2: // Keywords
                    IntenseStareType1 SelectedKeywordLabel = (KeywordLocalizationIDSelector.SelectedItem as IntenseStareType1)!;

                    PreviewCreatorPage.PreviewCreatorPageInstance.AddTextElementToColumn(
                        TargetColumn: this.CurrentTargetColumn,
                        CreatedColumnElement: PreviewCreatorPage.PreviewCreatorPageInstance.CreateKeyword(
                            GivenKeywordText: (PlainKeyword)SelectedKeywordLabel.DataContext,
                            Icon: this.SelectedKeywordIcon ?? ImageDictionaries.KeywordImages[((PlainKeyword)SelectedKeywordLabel.DataContext).ID!],
                            GivenJsonData: new ColumnTextElementData()
                            {
                                Type = PreviewCreatorPage.ColumnTextElementType.Keyword,
                                SelectedLocalizationID = $"{SelectedKeywordLabel.Uid}",
                                KeywordIcon_Path = SelectedKeywordIcon_Path!
                            },
                            TargetColumn: this.CurrentTargetColumn
                        )
                    );

                    SelectedKeywordIcon = null;
                    SelectedKeywordIcon_Path = null!;

                    break;
            }

            this.BeginFadeHiding();
        }
    }
}