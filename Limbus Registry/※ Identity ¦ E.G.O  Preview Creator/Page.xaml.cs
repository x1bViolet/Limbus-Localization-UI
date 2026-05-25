using ICSharpCode.AvalonEdit.Highlighting;
using LCLocalizationInterface.Internal.Abstractions;
using static LCLocalizationInterface.LimbusRegistry.PreviewCreator.PreviewCreatorPage;
using static RijnadelClassLibrary.SyntaxedTextEditorBase;

namespace LCLocalizationInterface.LimbusRegistry.PreviewCreator
{
    public partial class PreviewCreatorPage : Page
    {
        #pragma warning disable CS8618
        public static PreviewCreatorPage PreviewCreatorPageInstance;
        #pragma warning restore CS8618

        public PreviewCreatorPage()
        {
            InitializeComponent();
            PreviewCreatorPageInstance = this; // Neccessary for the CautionsTextElements below

            DecorativeCautions_TopStackPanel.Children.Clear();
            DecorativeCautions_BottomStackPanel.Children.Clear();
            for (int i = 0; i <= 50; i++)
            {
                DecorativeCautions_TopStackPanel.Children.Add(new CautionsTextElement());
                DecorativeCautions_BottomStackPanel.Children.Add(new CautionsTextElement());
            }

            bool AlreadyLoadedOnce = false;
            this.Loaded += (_, _) =>
            {
                if (!AlreadyLoadedOnce)
                {
                    ClearUndos();
                    SealCautions();
                    AlreadyLoadedOnce = true;
                }
            };


            RecentlySerializedImageInfoJson = CurrentImageInfoJson;


            {
                bool StyleAlreadyForced = false;
                this.Loaded += (_, _) =>
                {
                    if (!StyleAlreadyForced)
                    {
                        Internal.UIStyle.DefaultStyleDictionaryClass.OverrideDiffPlexScrollBarStyle(ImageInfoUnsavedChangesViewer);

                        StyleAlreadyForced = true;
                    }
                };
            }
        }








        #region UI switch process
        private static bool IsPreloadImageInfoWasLoaded = false;

        private static double PreviousWindowLocation_Left;
        private static double PreviousWindowLocation_Top;


        private static (double In, double Out) FadeDurations => FadeableWindow.AsPair(FadeableWindow.ThemeTimings.Duration.PreviewCreator);
        private static (double In, double Out) FadeSpeedRatios => FadeableWindow.AsPair(FadeableWindow.ThemeTimings.SpeedRatio.PreviewCreator);

        private static ((double Acceleration, double Deceleation) In, (double Acceleration, double Deceleation) Out) FadeKinematics
            => FadeableWindow.AsPairPair(FadeableWindow.ThemeTimings.AccelerationDecelerationRatios.PreviewCreator);


        private static DoubleAnimation ReuseableFadeIn => new()
        {
            From = -0.7, To = 1, Duration = TimeSpan.FromSeconds(FadeDurations.In),
            SpeedRatio = FadeSpeedRatios.In,
            AccelerationRatio = FadeKinematics.In.Acceleration,
            DecelerationRatio = FadeKinematics.In.Deceleation,
        };
        private static DoubleAnimation ReuseableFadeOut => new()
        {
            From = 1, To = -0.7, Duration = TimeSpan.FromSeconds(FadeDurations.Out),
            SpeedRatio = FadeSpeedRatios.Out,
            AccelerationRatio = FadeKinematics.Out.Acceleration,
            DecelerationRatio = FadeKinematics.Out.Deceleation,
        };

        public static bool RebuildTextElementsOnSwitch { get; set; } = false;

        public static void SwitchUI_Activate()
        {
            PreviousWindowLocation_Left = MainWindowInstance.Left;
            PreviousWindowLocation_Top = MainWindowInstance.Top;


            DoubleAnimation FadeOut = ReuseableFadeOut;
            FadeOut.Completed += async (_, _) =>
            {
                MainWindowInstance.WindowContentView.SelectedIndex = 1;

                MainWindowInstance.Topmost = false;
                MainWindowInstance.MaxWidth = double.PositiveInfinity;

                Rect ScreenSpace = SystemParameters.WorkArea;
                MainWindowInstance.Left = ScreenSpace.Left;
                MainWindowInstance.Top = ScreenSpace.Top;
                MainWindowInstance.Width = ScreenSpace.Width;
                MainWindowInstance.Height = ScreenSpace.Height;

                MainWindowInstance.VisualBorder.ShowTitleBarCursor = false;
                MainWindowInstance.CurrentWindowChrome.ResizeBorderThickness = new Thickness(0);
                MainWindowInstance.ResizeMode = ResizeMode.NoResize;

                MainWindowInstance.VisualBorder.BorderThickness = new Thickness(0);
                MainWindowInstance.VisualBorder.CornerRadius = new CornerRadius(0);
                MainWindowInstance.CanDragMove = false;

                MainWindowInstance.UpdateLayout();

                if (IsPreloadImageInfoWasLoaded == false)
                {
                    PreviewCreatorPageInstance.OpenImageInfo_Action(LoadedConfiguration.ScanParameters.PreloadedImageInfoForPreviewCreator);
                    PreviewCreatorPageInstance.RecentlySerializedImageInfoJson = PreviewCreatorPageInstance.CurrentImageInfoJson;
                    IsPreloadImageInfoWasLoaded = true;
                }

                @CurrentPreviewCreator.ActiveState = true;
                

                MainWindowInstance.BeginAnimation(MainWindow.OpacityProperty, ReuseableFadeIn);
                if (RebuildTextElementsOnSwitch)
                {
                    await Task.Delay(10);
                    PreviewCreatorPageInstance.RebuildTextElements();
                    RebuildTextElementsOnSwitch = false;
                }
            };

            MainWindowInstance.BeginAnimation(MainWindow.OpacityProperty, FadeOut);

        }
        public static void SwitchUI_Return(Action? CompleteAction = null)
        {
            ColumnElementContentSelectorWindow.ColumnElementContentSelectorInstance.BeginFadeHiding();

            DoubleAnimation FadeOut = ReuseableFadeOut;
            FadeOut.Completed += (_, _) =>
            {
                MainWindowInstance.WindowContentView.SelectedIndex = 0;

                @EditorModesShelf.CurrentEditorMode.AdjustMainWindow();

                MainWindowInstance.Left = PreviousWindowLocation_Left;
                MainWindowInstance.Top = PreviousWindowLocation_Top;

                MainWindowInstance.VisualBorder.ShowTitleBarCursor = true;
                MainWindowInstance.CurrentWindowChrome.ResizeBorderThickness = new Thickness(10);
                MainWindowInstance.ResizeMode = ResizeMode.CanResize;

                MainWindowInstance.VisualBorder.BorderThickness = new Thickness(1);
                MainWindowInstance.VisualBorder.CornerRadius = CornerRadiusFrom(@Themes.CurrentTheme.Common.MainWindow.BorderCornerRadius);
                MainWindowInstance.CanDragMove = true;

                MainWindowInstance.Topmost = LoadedConfiguration.Internal.IsAlwaysOnTop;

                MainWindowInstance.UpdateLayout();

                @CurrentPreviewCreator.ActiveState = false;

                CompleteAction?.Invoke();
                

                MainWindowInstance.BeginAnimation(MainWindow.OpacityProperty, ReuseableFadeIn);
            };

            MainWindowInstance.BeginAnimation(MainWindow.OpacityProperty, FadeOut);
        }
        #endregion






        #region Column elements context menu
        private ColumnTextElementContainer GetContextMenuItemTarget(object ContextMenuItem)
        {
            ColumnTextElementContainer TargetElement = (((ContextMenuItem as MenuItem)!.Parent as ContextMenu)!.PlacementTarget as ColumnTextElementContainer)!;
            return TargetElement;
        }

        private void ColumnItem_MoveUp(object Sender, RoutedEventArgs Args)
        {
            ColumnTextElementContainer TargetElement = GetContextMenuItemTarget(Sender);
            TargetElement.ParentColumn.Children.MoveItemUp(TargetElement);
            ReEnumerateColumnItemsJsonData();
        }
        private void ColumnItem_Delete(object Sender, RoutedEventArgs Args)
        {
            ColumnTextElementContainer TargetElement = GetContextMenuItemTarget(Sender);
            TextElementsColumn ParentColumn = TargetElement.ParentColumn;
            TargetElement.UnsealLocalizationTextView();
            TargetElement.ParentColumn.Children.Remove(TargetElement);
            ReEnumerateColumnItemsJsonData();

            // Sealed view memory clean on element removal .. idk how
            if (TargetElement.RelatedJsonData.Type is not ColumnTextElementType.Keyword)
            {
                ReSealAllTextElementsInColumn(ParentColumn);

                // Scrolling to the bottom right corner and returning back magically cleans memory
                double OriginalHorizontalOffset = CompositionScrollViewer.HorizontalOffset;
                double OriginalVerticalOffset = CompositionScrollViewer.VerticalOffset;

                CompositionScrollViewer.ScrollToBottom();
                CompositionScrollViewer.ScrollToRightEnd();

                CompositionScrollViewer.ScrollToHorizontalOffset(OriginalHorizontalOffset);
                CompositionScrollViewer.ScrollToVerticalOffset(OriginalVerticalOffset);
            }
        }
        private void ColumnItem_MoveDown(object Sender, RoutedEventArgs Args)
        {
            ColumnTextElementContainer TargetElement = GetContextMenuItemTarget(Sender);
            TargetElement.ParentColumn.Children.MoveItemDown(TargetElement);
            ReEnumerateColumnItemsJsonData();
        }


        private static ContextMenu ColumnElemenetContextMenu => (PreviewCreatorPageInstance!.__TextColumnsCanvas__.Resources["ColumnItemContextMenu"] as ContextMenu)!;
        private void ColumnElemenetContextMenu_MakeSemiTransperent(object Sender, RoutedEventArgs Args)
        {
            ColumnElemenetContextMenu.BeginAnimation(ContextMenu.OpacityProperty, new DoubleAnimation() { From = 1, To = 0.245, Duration = TimeSpan.FromSeconds(0.11) });
        }
        private void ColumnElemenetContextMenu_ReturnOpacityBackToNormal(object Sender, RoutedEventArgs Args)
        {
            ColumnElemenetContextMenu.BeginAnimation(ContextMenu.OpacityProperty, new DoubleAnimation() { From = 0.245, To = 1, Duration = TimeSpan.FromSeconds(0.11) });
        }
        private async void ColumnElemenet_ContextMenuOpening_CheckPartsAvilabilityBasedOnType(object Sender, ContextMenuEventArgs Args)
        {
            ColumnTextElementContainer Target = (Sender as ColumnTextElementContainer)!;
            StackPanel OptionsPanel = (((Target.ContextMenu.Items[7] as MenuItem)!.Header as Grid)!.Children[0] as StackPanel)!;
            
            UIElement SignatureInput = OptionsPanel.Children[0];
            UIElement PassiveWidthAdjust = OptionsPanel.Children[3];
            UIElement SkillWidthsAdjust = OptionsPanel.Children[4];

            SignatureInput.Visibility = Target.RelatedJsonData.Type != ColumnTextElementType.Keyword ? Visibility.Visible : Visibility.Collapsed;
            PassiveWidthAdjust.Visibility = Target.RelatedJsonData.Type == ColumnTextElementType.Passive ? Visibility.Visible : Visibility.Collapsed;
            SkillWidthsAdjust.Visibility = Target.RelatedJsonData.Type == ColumnTextElementType.Skill ? Visibility.Visible : Visibility.Collapsed;

            SealAllTextElementsInBothColumns(Predicate: TextElement => TextElement != Target); // ContextMenuClosing event does not firing if another context menu is opened while the current one is active, so unsealed text elements would remain unsealed

            await Task.Delay(150);
            
            @Languages.PresentedTextFields["[C] * [Element context menu] Item signature"].Document.UndoStack.ClearAll();
        }
        #endregion






        #region Technical
        /// <summary>Sections collapse/expand</summary>
        private void ToggleSecondChildVisibility(object Sender, MouseButtonEventArgs Args)
        {
            UIElement Target = ((Sender as IntenseStareType1)!.Parent as StackPanel)!.Children[1];
            Target.Visibility = Target.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }


        /// <summary>Set Opacity to 1 and IsHitTestVisible to True</summary>
        public static void MakeAvailable(params UIElement[] Targets)
        {
            foreach (UIElement Target in Targets)
            {
                Target.Opacity = 1;
                Target.IsHitTestVisible = true;
            }
        }

        /// <summary>Set Opacity to 0.55 and IsHitTestVisible to False</summary>
        public static void MakeUnavailable(params UIElement[] Targets)
        {
            foreach (UIElement Target in Targets)
            {
                Target.Opacity = 0.55;
                Target.IsHitTestVisible = false;
            }
        }

        public static void MakeAvailableConditional(bool Condition, params UIElement[] Targets)
        {
            if (Condition) MakeAvailable(Targets);
            else MakeUnavailable(Targets);
        }


        #region Vignettes highlight animations on mouse enter/leave on Sliders
        private void OpacityAnimation_In(string LinearGradientResourceName)
        {
            if (IsLoaded)
            {
                (CompositionGrid.Resources[LinearGradientResourceName] as LinearGradientBrush)!.BeginAnimation(LinearGradientBrush.OpacityProperty, new DoubleAnimation()
                {
                    From = 0, To = 1, Duration = new Duration(TimeSpan.FromSeconds(0.09))
                });
            }
        }
        private void OpacityAnimation_Out(string LinearGradientResourceName)
        {
            if (IsLoaded)
            {
                (CompositionGrid.Resources[LinearGradientResourceName] as LinearGradientBrush)!.BeginAnimation(LinearGradientBrush.OpacityProperty, new DoubleAnimation()
                {
                    From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(0.09))
                });
            }
        }

        private void HighlightVignette_LeftTopBottom(object Sender, MouseEventArgs EventArgs)
        {
            OpacityAnimation_In("VignettesViewBorder_Left");
            OpacityAnimation_In("VignettesViewBorder_Top");
            OpacityAnimation_In("VignettesViewBorder_Bottom");
        }
        private void UnHighlightVignette_LeftTopBottom(object Sender, MouseEventArgs EventArgs)
        {
            OpacityAnimation_Out("VignettesViewBorder_Left");
            OpacityAnimation_Out("VignettesViewBorder_Top");
            OpacityAnimation_Out("VignettesViewBorder_Bottom");
        }

        private void HighlightVignette_Right(object Sender, MouseEventArgs EventArgs) => OpacityAnimation_In("VignettesViewBorder_Right");
        private void UnHighlightVignette_Right(object Sender, MouseEventArgs EventArgs) => OpacityAnimation_Out("VignettesViewBorder_Right");

        private void HighlightVignette_Top(object Sender, MouseEventArgs EventArgs) => OpacityAnimation_In("VignettesViewBorder_Top");
        private void UnHighlightVignette_Top(object Sender, MouseEventArgs EventArgs) => OpacityAnimation_Out("VignettesViewBorder_Top");

        private void HighlightVignette_Bottom(object Sender, MouseEventArgs EventArgs) => OpacityAnimation_In("VignettesViewBorder_Bottom");
        private void UnHighlightVignette_Bottom(object Sender, MouseEventArgs EventArgs) => OpacityAnimation_Out("VignettesViewBorder_Bottom");

        private void HighlightVignette_Left(object Sender, MouseEventArgs EventArgs) => OpacityAnimation_In("VignettesViewBorder_Left");
        private void UnHighlightVignette_Left(object Sender, MouseEventArgs EventArgs) => OpacityAnimation_Out("VignettesViewBorder_Left");

        private void HighlightVignette_LeftBehindEGOPortrait(object Sender, MouseEventArgs EventArgs) => OpacityAnimation_In("VignettesViewBorder_LeftBehindEGOPortrait");
        private void UnHighlightVignette_LeftBehindEGOPortrait(object Sender, MouseEventArgs EventArgs) => OpacityAnimation_Out("VignettesViewBorder_LeftBehindEGOPortrait");
        #endregion

        #endregion

        public void RebuildTextElements()
        {
            ReconstructColumnItems(@DataContextDomain.PreviewCreator.ImageInfo.TextColumns.First.Items, TextColumn_1);
            ReconstructColumnItems(@DataContextDomain.PreviewCreator.ImageInfo.TextColumns.Second.Items, TextColumn_2);

            ReEnumerateColumnItemsJsonData();
        }

        public void SetIdentityOrEGONameLineBreakSyntaxColor()
        {
            @Languages.PresentedTextFields["[C] * [Section:Sinner and Identity/E.G.O specific] Identity/E.G.O name"].SyntaxHighlighting = new SyntaxedTextEditorBase.SyntaxHighlightDefinition()
            {
                MainRuleSet =
                {
                    Rules =
                    {
                        new HighlightingRule()
                        {
                            Regex = new Regex(@"\\n"),
                            Color = new HighlightingColor() { Foreground = new HighlightionBrush(Themes.CurrentTheme.UITextfields.Syntax.Highlight1) }
                        }
                    }
                }
            };
        }














        private void ColumnTextElement_ContextMenuOpening_UnsealView(object Sender, ContextMenuEventArgs Args)
        {
            (Sender as ColumnTextElementContainer)!.UnsealLocalizationTextView();
        }

        private void ColumnTextElement_ContextMenuClosing_SealView(object Sender, ContextMenuEventArgs Args)
        {
            (Sender as ColumnTextElementContainer)!.SealLocalizationTextView();
        }



        public void SealAllTextElementsInColumn(TextElementsColumn TargetColumn, Func<ColumnTextElementContainer, bool>? Predicate = null)
        {
            Predicate ??= (TextElement => true);
            foreach (ColumnTextElementContainer TextElement in TargetColumn.Children.OfType<ColumnTextElementContainer>().Where(Predicate))
            {
                TextElement.SealLocalizationTextView();
            }
        }
        public void UnsealAllTextElementsInColumn(TextElementsColumn TargetColumn, Func<ColumnTextElementContainer, bool>? Predicate = null)
        {
            Predicate ??= (TextElement => true);
            foreach (ColumnTextElementContainer TextElement in TargetColumn.Children.OfType<ColumnTextElementContainer>().Where(Predicate))
            {
                TextElement.UnsealLocalizationTextView();
            }
        }
        public void SealAllTextElementsInBothColumns(Func<ColumnTextElementContainer, bool>? Predicate = null)
        {
            SealAllTextElementsInColumn(TextColumn_1, Predicate);
            SealAllTextElementsInColumn(TextColumn_2, Predicate);
        }
        public void UnsealAllTextElementsInBothColumns(Func<ColumnTextElementContainer, bool>? Predicate = null)
        {
            UnsealAllTextElementsInColumn(TextColumn_1, Predicate);
            UnsealAllTextElementsInColumn(TextColumn_2, Predicate);
        }

        public void ReSealAllTextElementsInColumn(TextElementsColumn TargetColumn, Func<ColumnTextElementContainer, bool>? Predicate = null)
        {
            Predicate ??= (TextElement => true);
            foreach (ColumnTextElementContainer TextElement in TargetColumn.Children.OfType<ColumnTextElementContainer>().Where(Predicate))
            {
                TextElement.UnsealLocalizationTextView();
                TextElement.SealLocalizationTextView();
            }
        }
        public void ReSealAllTextElementsInBothColumns(TextElementsColumn TargetColumn, Func<ColumnTextElementContainer, bool>? Predicate = null)
        {
            ReSealAllTextElementsInColumn(TextColumn_1, Predicate);
            ReSealAllTextElementsInColumn(TextColumn_2, Predicate);
        }

        private void ItemSignaturesOffset_MouseEnter_UnsealTextElements(object Sender, MouseEventArgs Args) => UnsealAllTextElementsInColumn(((Sender as StackPanel)!.Tag as TextElementsColumn)!);
        private void ItemSignaturesOffset_MouseLeave_SealTextElements(object Sender, MouseEventArgs Args) => SealAllTextElementsInColumn(((Sender as StackPanel)!.Tag as TextElementsColumn)!);

        private void DecorativeCautionsParametersPanel_MouseEnter_UnsealCautions(object Sender, MouseEventArgs Args) => UnsealCautions();
        private void DecorativeCautionsParametersPanel_MouseLeave_SealCautions(object Sender, MouseEventArgs Args)
        {
            if (@Languages.PresentedTextFields["[C] * [Section:Decorative cautions] Cautions text"].IsKeyboardFocusWithin == false & @Languages.PresentedTextFields["[C] * [Section:Decorative cautions] Cautions color"].IsKeyboardFocusWithin == false)
            {
                SealCautions();
            }
        }

        private void ImageWidthSlider_MouseEnter_UnsealCautions(object Sender, MouseEventArgs Args) => UnsealCautions();
        private void ImageWidthSlider_MouseLeave_SealCautions(object Sender, MouseEventArgs Args) => SealCautions();



        public void SealCautions()
        {
            if (LoadedConfiguration.Internal.DisableTextElementsSealingInPreviewCreator == false & PreviewCreatorPageInstance.IsLoaded)
            {
                _ = Cautions_Top_SealedView.Visibility
                  = Cautions_Bottom_SealedView.Visibility
                  = Visibility.Visible;

                Cautions_Top_SealedView.Source = CaptureElement(Cautions_Top_ActualView, Upscale: 2.0);
                Cautions_Bottom_SealedView.Source = CaptureElement(Cautions_Bottom_ActualView, Upscale: 2.0);

                _ = Cautions_Top_ActualView.Visibility
                  = Cautions_Bottom_ActualView.Visibility
                  = Visibility.Collapsed;
            }

        }
        public void UnsealCautions()
        {
            _ = Cautions_Top_SealedView.Visibility
              = Cautions_Bottom_SealedView.Visibility
              = Visibility.Collapsed;

            if (LoadedConfiguration.Internal.DisableTextElementsSealingInPreviewCreator == false)
            {
                _ = Cautions_Top_SealedView.Source
                  = Cautions_Bottom_SealedView.Source
                  = null;

                Cautions_Top_SealedView.UpdateLayout();
                Cautions_Bottom_SealedView.UpdateLayout();
            }

            _ = Cautions_Top_ActualView.Visibility
              = Cautions_Bottom_ActualView.Visibility
              = Visibility.Visible;
        }

        private void CautionsTextInput_LostKeyboardFocus(object Sender, KeyboardFocusChangedEventArgs Args)
        {
            if (DecorativeCautionsParametersPanel.IsMouseOver == false)
            {
                DecorativeCautionsParametersPanel_MouseLeave_SealCautions(null!, null!);
            }
        }
    }






    public ref struct @CurrentPreviewCreator
    {
        public static bool ActiveState { get; set; } = false;
        public static bool IsImageInfoLoadingEvent = false;

        public static void RebuildTextElements()
        {
            if (ActiveState == true)
            {
                PreviewCreatorPageInstance.RebuildTextElements();
            }
            else
            {
                PreviewCreatorPage.RebuildTextElementsOnSwitch = true; /// Be sure template is applied to all presented <see cref="ColumnTextElementContainer"/>s
            }
        }

        public ref struct TextColumnsDynamicResources
        {
            private static ResourceDictionary ResourceDictionary => PreviewCreatorPageInstance.CompositionGrid.Resources;

            public static FontFamily SignaturesFont { set => ResourceDictionary["TextColumns_ItemSignaturesFont"] = value; }
        }
    }




    #region Specific ui elements
    /// <summary>
    /// Grid with two columns (0.05*, 1*) and thin border at the left column, any elements or content must be at the Grid.Column="1"
    /// </summary>
    public class OrnamentedSection : LCLocalizationInterface.Internal.UIStyle.TwoColumned
    {
        public OrnamentedSection()
        {
            this.Width1 = new GridLength(0.05, GridUnitType.Star);
            this.Margin = new Thickness(0, 5, 0, 0);
            Border Ornament = new() { CornerRadius = new CornerRadius(0), Width = 5 };
            Ornament.SetResourceReference(Border.StyleProperty, "Theme:ControlStyles.OtherBorderLikeThing");
            this.Children.Add(Ornament);
        }
    }

    /// <summary>
    /// <c>"//// CAUTION "</c> text fragment with style properties binded to the <see cref="PreviewCreatorPage.DecorativeCautions_StyleBindingSource"/>
    /// </summary>
    public class CautionsTextElement : StackPanel
    {
        /// <inheritdoc cref="CautionsTextElement"/>
        public CautionsTextElement()
        {
            this.Orientation = Orientation.Horizontal;
            this.Margin = new Thickness(0, 0, 8.3, 0);


            TextBlock PART_SlashChars = new()
            {
                FontFamily = FontFromResource("UI/Fonts/#Sappy"),
                Margin = new Thickness(0, 0.6, 8.3, 0),
                FontSize = 10.5,
                Text = "////",
            };
            PART_SlashChars.BindSame(TextBlock.ForegroundProperty, PreviewCreatorPageInstance.DecorativeCautions_StyleBindingSource);

            TextBlock PART_Text = new();
            PART_Text.BindSameProperties(
                BindingSource: PreviewCreatorPageInstance.DecorativeCautions_StyleBindingSource,
                Properties: [
                    TextBlock.ForegroundProperty,
                    TextBlock.FontFamilyProperty,
                    TextBlock.FontSizeProperty,
                    TextBlock.LineHeightProperty,
                    TextBlock.TextProperty,
                    TextBlock.RenderTransformProperty,
                ]
            );


            this.Children.Add(PART_SlashChars);
            this.Children.Add(PART_Text);
        }
    }
    #endregion
}