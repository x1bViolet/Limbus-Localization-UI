using System.Windows;
using System.Windows.Controls;

namespace LC_Localization_Task_Absolute.BaseUIElements
{
    public class Section : StackPanel;
    public class TextfieldCorners : Border;
    public class TwoColumned : Grid
    {
        public double Length1 { set { this.ColumnDefinitions[0].Width = new GridLength(value, GridUnitType.Star); } }
        public double Length2 { set { this.ColumnDefinitions[1].Width = new GridLength(value, GridUnitType.Star); } }
        public double Width1 { set { this.ColumnDefinitions[0].Width = new GridLength(value); } }
        public double Width2 { set { this.ColumnDefinitions[1].Width = new GridLength(value); } }
        public TwoColumned()
        {
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition());
        }
    }


    namespace PreviewCreator
    {
        /// <summary>
        /// Grid with two columns (0.05*, 1*) and thin border at the left column, any elements or content must be at the Grid.Column="1"
        /// </summary>
        public class OrnamentedSection : TwoColumned
        {
            public OrnamentedSection()
            {
                this.Length1 = 0.05;
                this.Margin = new Thickness(0, 5, 0, 0);
                Border Ornament = new()
                {
                    CornerRadius = new CornerRadius(0),
                    Width = 5,
                    Style = ᐁ_Interface_Themes_Loader.ThemeKeysDictionary["Theme:ControlStyles.OtherBorderLikeThing"] as Style
                };
                this.Children.Add(Ornament);
            }
        }
    }
}
