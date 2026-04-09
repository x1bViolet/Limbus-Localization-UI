using System.Windows.Markup;

namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    /// <summary>Special version with modified layout exclusively for Preview Creator</summary>
    [ContentProperty(nameof(SkillLocalizationTextView))]
    public partial class SkillNameReplicaUIElement_PCE : SkillNameReplicaUIElement
    {
        public UIElement SkillLocalizationTextView { get => (UIElement)GetValue(SkillLocalizationTextViewProperty); set => SetValue(SkillLocalizationTextViewProperty, value); }
        public static readonly DependencyProperty SkillLocalizationTextViewProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, UIElement>();
        

        public bool ShowAffinityIcon { get => (bool)GetValue(ShowAffinityIconProperty); set => SetValue(ShowAffinityIconProperty, value); }
        public static readonly DependencyProperty ShowAffinityIconProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, bool>(DefaultValue: true);

        public new AffinityName Affinity { get => base.Affinity; set => UpdateAffinityIcon(base.Affinity = value); }
        private void UpdateAffinityIcon(AffinityName NewAffinity)
        {
            InternalAffinityIcon = NewAffinity is not AffinityName.None
                ? BitmapFromResource($"UI/Limbus/Skills/Affinity Icons/{NewAffinity}.png")
                : new BitmapImage();
        }

        #region Technical DependencyProperties used in ControlTemplate Bindings
        public ImageSource InternalAffinityIcon { get => (ImageSource)GetValue(InternalAffinityIconProperty); set => SetValue(InternalAffinityIconProperty, value); }
        public static readonly DependencyProperty InternalAffinityIconProperty = RegisterProperty<SkillNameReplicaUIElement_PCE, ImageSource>();
        #endregion
    }
}