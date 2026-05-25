namespace LCLocalizationInterface.Internal.UIStyle
{
    public partial class DefaultStyleDictionaryClass : ResourceDictionary
    {
        public const double OverrideScrollViewerScrollStep = 35;

        /// <summary>
        /// Override mouse wheel scrolling for <see cref="ScrollViewer"/>s with adjustable step (<see cref="OverrideScrollViewerScrollStep"/>)<br/>
        /// (Also compatibility with <see cref="IntenseStareType3"/> as part of their content, normal mouse wheel scrolling suddenly stops when mouse enters <see cref="IntenseStareType3"/> (<see cref="ICSharpCode.AvalonEdit.TextEditor"/>))
        /// </summary>
        private void ScrollViewer_PreviewMouseWheel(object Sender, MouseWheelEventArgs Args)
        {
            ScrollViewer ActualSender = (Sender as ScrollViewer)!;

            List<ComboBox> ComboBoxes = ActualSender.FindVisualChildren<ComboBox>();
            if (ComboBoxes.Any(x => x.IsDropDownOpen) == false)
            {
                if (ActualSender is SurfaceScrollViewer SurfaceScroller && SurfaceScroller.IsSurfaceScrolling)
                {
                    Args.Handled = true;
                    return;
                }
                
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (Args.Delta > 0)
                    {
                        ActualSender.ScrollToHorizontalOffset(ActualSender.HorizontalOffset + OverrideScrollViewerScrollStep);
                    }
                    else
                    {
                        ActualSender.ScrollToHorizontalOffset(ActualSender.HorizontalOffset - OverrideScrollViewerScrollStep);
                    }
                }
                else
                {
                    if (Args.Delta > 0)
                    {
                        ActualSender.ScrollToVerticalOffset(ActualSender.VerticalOffset - OverrideScrollViewerScrollStep);
                    }
                    else
                    {
                        ActualSender.ScrollToVerticalOffset(ActualSender.VerticalOffset + OverrideScrollViewerScrollStep);
                    }
                }

                Args.Handled = true;
            }
        }


        private void BaseWindowLayout_PreviewKeyDown(object Sender, KeyEventArgs Args)
        {
            if (new[] { Key.LeftCtrl, Key.LeftShift, Key.F }.All(Keyboard.IsKeyDown))
            {
                (Sender as Window)!.RenderImage(@$"[⇲] Assets Directory\Scans\{DateTime.Now:HHːmmːss (dd.MM.yyyy)}.png", LoadedConfiguration.ScanParameters.ScaleFactor, DoLayoutUpdate: false);
            }
        }


        /// <summary>
        /// Style reverse engineering for DiffPlex.Wpf.Controls.InternalLinesViewer because it is AN 'INTERNAL' CLASS FOR SOME REASON (Means unable to change ControlTemplate that has its own ScrollBar styles for content.............)
        /// </summary>
        public static void OverrideDiffPlexScrollBarStyle(DiffPlex.Wpf.Controls.DiffViewer Target)
        {
            foreach (ScrollViewer InternalScrollViewer in Target.FindVisualChildren<ScrollViewer>())
            {
                if (InternalScrollViewer is { Name: "ValueScrollViewer" } FoundDiffTextScrollViewer)
                {
                    FoundDiffTextScrollViewer.Padding = new Thickness(0, 0, 8, 0); // Diff line offset from ScrollBar
                    FoundDiffTextScrollViewer.Template = @Themes.GetDefaultStyleDictionaryResource<ControlTemplate>("Theme:ControlTemplates:ControlStyles.ScrollViewer");
                    FoundDiffTextScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;


                    Button ActualCorner = new() { Opacity = 0 };
                    #pragma warning disable CS0618
                    ActualCorner.Click += GeneralActionToken;
                    #pragma warning restore CS0618

                    Grid Corner = new()
                    {
                        Width = 16,
                        Height = 16,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(0, 0, 1, 1),
                        Children =
                                    {
                                        ActualCorner,
                                        new Button() { Style = @Themes.GetDefaultStyleDictionaryResource<Style>("Theme:ControlStyles.SquareButton"), IsHitTestVisible = false },
                                    }
                    };

                    Grid.SetColumn(Corner, 2);

                    (FoundDiffTextScrollViewer.Parent as Grid)!.Children.Add(Corner);
                }
                else if (InternalScrollViewer is { Name: "NumberScrollViewer" } FoundLineNumbersScrollViewer)
                {
                    (FoundLineNumbersScrollViewer.Content as StackPanel)!.Resources.Add(key: typeof(TextBlock), value: new Style()
                    {
                        TargetType = typeof(TextBlock),
                        Setters = { new Setter() { Property = TextBlock.PaddingProperty, Value = new Thickness(0, 0, 4, 0) } } // Additional line numbers offset from right
                    });
                }
            }
            foreach (UserControl InternalUserControl in Target.FindVisualChildren<UserControl>())
            {
                if (InternalUserControl.GetType() is { Name: "InternalLinesViewer" })
                {
                    InternalUserControl.SetPropertyValue<ContextMenu>("LineContextMenu", null!);
                }
            }
        }
    }


    /// <summary>
    /// Visual for content of the Windows with <see cref="WindowStyle.None"/> with dedicated places for parts (<see cref="Title"/>, <see cref="Navigation"/>, and <see cref="ContentControl.Content"/>), Template defined inside the <c>`Default Style.xaml`</c> file
    /// </summary>
    public class InstancedWindowLayout : ContentControl // Deriving from just Control and custom Content property = huge freeze on tempalte applying
    {
        public InstancedWindowLayout()
        {
            Navigation = new ObservableCollection<UIElement>(); /// Elements shown twice in XAML Designer if this value is set in <see cref="FrameworkPropertyMetadata"/>
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (WindowDraggingWasAddedManually == false)
            {
                this.Template.FindTypeName<Border>(this, "PART_TitleBarTextArea")!.MouseLeftButtonDown += delegate (object Sender, MouseButtonEventArgs Args)
                {
                    this.FindVisualParent<Window>()!.DragMove();
                    Args.Handled = true;
                };
            }
        }


        public IntenseStareType1 Title { get => (IntenseStareType1)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public static readonly DependencyProperty TitleProperty = RegisterProperty<InstancedWindowLayout, IntenseStareType1>();


        public ObservableCollection<UIElement> Navigation { get => (ObservableCollection<UIElement>)GetValue(NavigationProperty); set => SetValue(NavigationProperty, value); }
        public static readonly DependencyProperty NavigationProperty = RegisterProperty<InstancedWindowLayout, ObservableCollection<UIElement>>(DefaultValue: null!); /// Set in <see cref="InstancedWindowLayout()"/>


        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
        public static readonly DependencyProperty CornerRadiusProperty = RegisterProperty<InstancedWindowLayout, CornerRadius>();


        public bool HasPaddedDropShadow { get => (bool)GetValue(HasPaddedDropShadowProperty); set => SetValue(HasPaddedDropShadowProperty, value); }
        public static readonly DependencyProperty HasPaddedDropShadowProperty = RegisterProperty<InstancedWindowLayout, bool>(DefaultValue: false);

        public bool PaddedDropShadowAllowedByTheme { get => (bool)GetValue(PaddedDropShadowAllowedByThemeProperty); set => SetValue(PaddedDropShadowAllowedByThemeProperty, value); }
        public static readonly DependencyProperty PaddedDropShadowAllowedByThemeProperty = RegisterProperty<InstancedWindowLayout, bool>(DefaultValue: true);


        public bool ShowTitleBarCursor { get => (bool)GetValue(ShowTitleBarCursorProperty); set => SetValue(ShowTitleBarCursorProperty, value); }
        public static readonly DependencyProperty ShowTitleBarCursorProperty = RegisterProperty<InstancedWindowLayout, bool>(DefaultValue: true);






        bool WindowDraggingWasAddedManually = false;
        public event MouseButtonEventHandler WindowDragging
        {
            add
            {
                WindowDraggingWasAddedManually = true;
                this.Loaded += delegate (object Sender, RoutedEventArgs Args)
                {
                    this.FindTypeNameFromTemplate<Border>("PART_TitleBarTextArea")!.PreviewMouseLeftButtonDown += value;
                };
            }
            remove
            {
                this.FindTypeNameFromTemplate<Border>("PART_TitleBarTextArea")!.PreviewMouseLeftButtonDown -= value;
            }
        }
    }


    public class Section : StackPanel;

    public class TwoColumned : Grid
    {
        public static readonly DependencyProperty Width1Property = RegisterProperty<TwoColumned, GridLength>(
            DefaultValue: new GridLength(1, GridUnitType.Star), PropertyChangedEvent: delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args
        ) {
            (Sender as TwoColumned)!.ColumnDefinitions[0].Width = (GridLength)Args.NewValue;
        });
        public static readonly DependencyProperty Width2Property = RegisterProperty<TwoColumned, GridLength>(
            DefaultValue: new GridLength(1, GridUnitType.Pixel), PropertyChangedEvent: delegate (DependencyObject Sender, DependencyPropertyChangedEventArgs Args
        ) {
            (Sender as TwoColumned)!.ColumnDefinitions[1].Width = (GridLength)Args.NewValue;
        });
        public GridLength Width1 { get => (GridLength)GetValue(Width1Property); set => SetValue(Width1Property, value); }
        public GridLength Width2 { get => (GridLength)GetValue(Width2Property); set => SetValue(Width2Property, value); }

        public TwoColumned()
        {
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition());
        }
    }


    /// <summary>
    /// Almost TabControl but without any nailed down keyboard shortcuts that i can't disable, only <see cref="SelectedIndex"/> property
    /// </summary>
    public class SingleViewer : Grid
    {
        public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }
        public static readonly DependencyProperty SelectedIndexProperty = RegisterProperty<SingleViewer, int>(PropertyChangedEvent: OnSelectedIndexChanged);
        private static void OnSelectedIndexChanged(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
        {
            (Sender as SingleViewer)!.CheckChildrenVisibility();
        }

        private void CheckChildrenVisibility()
        {
            foreach (UIElement Child in this.Children) Child?.Visibility = Visibility.Collapsed;

            if (this.SelectedIndex != -1 && this.SelectedIndex <= this.Children.Count - 1)
            {
                this.Children[this.SelectedIndex]?.Visibility = Visibility.Visible;
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject VisualAdded, DependencyObject VisualRemoved)
        {
            base.OnVisualChildrenChanged(VisualAdded, VisualRemoved);

            if (VisualAdded is UIElement AddedChild) AddedChild.Visibility = Visibility.Collapsed;
            CheckChildrenVisibility();
        }
    }




    /// <summary>
    /// <see cref="ScrollViewer"/> with scrolling by holding down the <see cref="SurfaceScrollDragKey"/> on Content (Its should be the <see cref="FrameworkElement"/>)
    /// </summary>
    public class SurfaceScrollViewer : ScrollViewer
    {
        public SurfaceScrollViewer()
        {
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            this.PreviewMouseUp += SurfaceScroll_MouseUp;
            this.PreviewMouseMove += SurfaceScrollContent_MouseMove;
        }

        public MouseButton SurfaceScrollDragKey { get => (MouseButton)GetValue(SurfaceScrollDragKeyProperty); set => SetValue(SurfaceScrollDragKeyProperty, value); }
        public static readonly DependencyProperty SurfaceScrollDragKeyProperty = RegisterProperty<SingleViewer, MouseButton>(DefaultValue: MouseButton.Left);

        public Cursor SurfaceScrollCursor { get => (Cursor)GetValue(SurfaceScrollCursorProperty); set => SetValue(SurfaceScrollCursorProperty, value); }
        public static readonly DependencyProperty SurfaceScrollCursorProperty = RegisterProperty<SingleViewer, Cursor>(DefaultValue: Cursors.Hand);


        public bool IsSurfaceScrolling { get => (bool)GetValue(IsSurfaceScrollingProperty); set => SetValue(IsSurfaceScrollingProperty, value); }
        public static readonly DependencyProperty IsSurfaceScrollingProperty = RegisterProperty<ZoomableScrollViewer, bool>();


        public event RoutedEventHandler SurfaceScrollingStarted { add => AddHandler(SurfaceScrollingStartedEvent, value); remove => RemoveHandler(SurfaceScrollingStartedEvent, value); }
        public static readonly RoutedEvent SurfaceScrollingStartedEvent = RegisterEvent<ZoomableScrollViewer, RoutedEventHandler>();

        public event RoutedEventHandler SurfaceScrollingEnded { add => AddHandler(SurfaceScrollingEndedEvent, value); remove => RemoveHandler(SurfaceScrollingEndedEvent, value); }
        public static readonly RoutedEvent SurfaceScrollingEndedEvent = RegisterEvent<ZoomableScrollViewer, RoutedEventHandler>();


        protected override void OnContentChanged(object OldContent, object NewContent)
        {
            base.OnContentChanged(OldContent, NewContent);

            if (NewContent is FrameworkElement NewSurfaceScrollArea)
            {
                NewSurfaceScrollArea.PreviewMouseDown += SurfaceScrollContent_MouseDown;
            }

            if (OldContent is FrameworkElement OldSurfaceScrollArea)
            {
                OldSurfaceScrollArea.PreviewMouseDown -= SurfaceScrollContent_MouseDown;
            }
        }


        #region Surface scroll
        private Point LastMousePosition;
        private void SurfaceScrollContent_MouseDown(object Sender, MouseButtonEventArgs Args)
        {
            if (Args.ChangedButton == this.SurfaceScrollDragKey)
            {
                LastMousePosition = Args.GetPosition(this);

                this.CaptureMouse();
                this.IsSurfaceScrolling = true;
                this.Cursor = this.SurfaceScrollCursor;
                RaiseEvent(new RoutedEventArgs(SurfaceScrollViewer.SurfaceScrollingStartedEvent, this));
            }
        }
        private void SurfaceScrollContent_MouseMove(object Sender, MouseEventArgs Args)
        {
            if (this.IsSurfaceScrolling)
            {
                System.Windows.Point CurrentPosition = Args.GetPosition(this);
                System.Windows.Vector Difference = LastMousePosition - CurrentPosition;

                this.ScrollToVerticalOffset(this.VerticalOffset + Difference.Y);
                this.ScrollToHorizontalOffset(this.HorizontalOffset + Difference.X);
                LastMousePosition = CurrentPosition;
            }
        }
        private void SurfaceScroll_MouseUp(object Sender, MouseButtonEventArgs Args)
        {
            if (Args.ChangedButton == this.SurfaceScrollDragKey)
            {
                (Sender as ScrollViewer)!.ReleaseMouseCapture();
                this.IsSurfaceScrolling = false;
                this.Cursor = Cursors.Arrow;
                RaiseEvent(new RoutedEventArgs(SurfaceScrollViewer.SurfaceScrollingEndedEvent, this));
            }
        }
        #endregion
    }




    #region ScrollViewers
    public class ZoomableScrollViewer : SurfaceScrollViewer
    {
        public ZoomableScrollViewer()
        {
            SurfaceScrollDragKey = MouseButton.Middle;
        }


        private FrameworkElement? CurrentElement;
        public ScaleTransform CurrentElementTransform { get; set; } = new(1.0, 1.0);



        public double ZoomStep { get => (double)GetValue(ZoomStepProperty); set => SetValue(ZoomStepProperty, value); }
        public static readonly DependencyProperty ZoomStepProperty = RegisterProperty<ZoomableScrollViewer, double>(DefaultValue: 0.1);

        public double MaxZoomScale { get => (double)GetValue(MaxZoomScaleProperty); set => SetValue(MaxZoomScaleProperty, value); }
        public static readonly DependencyProperty MaxZoomScaleProperty = RegisterProperty<ZoomableScrollViewer, double>(DefaultValue: 6.0);

        public double MinZoomScale { get => (double)GetValue(MinZoomScaleProperty); set => SetValue(MinZoomScaleProperty, value); }
        public static readonly DependencyProperty MinZoomScaleProperty = RegisterProperty<ZoomableScrollViewer, double>(DefaultValue: 1.0);


        protected override void OnContentChanged(object OldContent, object NewContent)
        {
            base.OnContentChanged(OldContent, NewContent);

            if (NewContent is FrameworkElement FrameworkElementContent)
            {
                FrameworkElementContent.LayoutTransform = CurrentElementTransform;
                FrameworkElementContent.RenderTransformOrigin = new Point(0, 0);
                CurrentElement = FrameworkElementContent;
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs Args)
        {
            base.OnPreviewMouseWheel(Args);

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (CurrentElement is not null)
                {
                    Args.Handled = true;
                    
                    // If RenderTransform instead of LayoutTransform, center at the mouse cursor
                    //Point MousePositionOnCurrentElement = Mouse.GetPosition(CurrentElement);
                    
                    //double NormalizedX = MousePositionOnCurrentElement.X / CurrentElement.RenderSize.Width;
                    //double NormalizedY = MousePositionOnCurrentElement.Y / CurrentElement.RenderSize.Height;

                    //CurrentElement.RenderTransformOrigin = new Point(NormalizedX, NormalizedY);

                    if (Args.Delta > 0 && CurrentElementTransform.ScaleX < this.MaxZoomScale)
                    {
                        CurrentElementTransform.ScaleX += this.ZoomStep;
                        CurrentElementTransform.ScaleY += this.ZoomStep;
                    }
                    else if (Args.Delta < 0 && CurrentElementTransform.ScaleX > this.MinZoomScale)
                    {
                        CurrentElementTransform.ScaleX -= this.ZoomStep;
                        CurrentElementTransform.ScaleY -= this.ZoomStep;
                    }
                }
            }
        }
    }
    #endregion



    /// <summary>With fix for Border content being drawn outside the rounded corners (<see href="https://stackoverflow.com/a/325003/22964624"/>)</summary>
    public class ClippingBorder : Border
    {
        protected override void OnRender(DrawingContext DrawingContext)
        {
            OnApplyChildClip();
            base.OnRender(DrawingContext);
        }

        public override UIElement Child
        {
            get => base.Child;
            set
            {
                if (this.Child != value)
                {
                    this.Child?.SetValue(ClipProperty, OldClip);
                    OldClip = value?.ReadLocalValue(ClipProperty);
                    base.Child = value;
                }
            }
        }

        protected virtual void OnApplyChildClip()
        {
            UIElement Child = this.Child;
            if (Child is not null)
            {
                OverrideClipRect.RadiusX = OverrideClipRect.RadiusY = Math.Max(0.0, this.CornerRadius.TopLeft - (this.BorderThickness.Left * 0.5));
                OverrideClipRect.Rect = new Rect(this.Child.RenderSize);
                Child.Clip = OverrideClipRect;
            }
        }

        private readonly RectangleGeometry OverrideClipRect = new();
        private object? OldClip;
    }
}

namespace LCLocalizationInterface
{
    namespace MarkupExtensions
    {
        public static partial class MarkupShrinking
        {
            public static readonly DependencyProperty DisableMenuItemMouseOverHighlighProperty = RegisterAttachedProperty(PropertyType: typeof(bool), OwnerType: typeof(MarkupShrinking), DefaultValue: false);

            public static bool GetDisableMenuItemMouseOverHighligh(DependencyObject Sender) => (bool)Sender.GetValue(DisableMenuItemMouseOverHighlighProperty);
            public static void SetDisableMenuItemMouseOverHighligh(DependencyObject Sender, bool Value) => Sender.SetValue(DisableMenuItemMouseOverHighlighProperty, Value);
        }
    }
}