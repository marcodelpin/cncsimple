using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;

namespace CncConvProg.View.Controls
{
    #region Directives



    #endregion

    /// <summary>
    /// ReorderableListBox, subclasses ListBox
    /// </summary>
    public class ListViewEx : ListView
    {
        #region Public Static Members
        /// <summary>
        /// ItemsReorderedEvent
        /// </summary>
        public static readonly RoutedEvent ItemsReorderedEvent;
        #endregion

        #region Protected Members
        /// <summary>
        /// dragStartPoint
        /// </summary>
        protected Point dragStartPoint;

        /// <summary>
        /// dragStarted
        /// </summary>
        protected bool dragging;

        /// <summary>
        /// dragItemSelected
        /// </summary>
        protected bool dragItemSelected;

        /// <summary>
        /// adornerLayer
        /// </summary>
        protected AdornerLayer adornerLayer;

        /// <summary>
        /// overlayElement
        /// </summary>
        protected DropPreviewAdorner overlayElement;

        /// <summary>
        /// originalItemIndex
        /// </summary>
        protected int originalItemIndex;

        #endregion

        #region Constructors

        static ListViewEx()
        {
            ItemsReorderedEvent = EventManager.RegisterRoutedEvent("ItemsReordered", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListViewEx));
        }
        #endregion

        #region Event Elements
        /// <summary>
        /// ItemsReordered
        /// </summary>
        public event RoutedEventHandler ItemsReordered
        {
            add { this.AddHandler(ItemsReorderedEvent, value); }
            remove { this.RemoveHandler(ItemsReorderedEvent, value); }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// OriginalItemIndex
        /// </summary>
        public int OriginalItemIndex
        {
            get { return this.originalItemIndex; }
            set { this.originalItemIndex = value; }
        }

        /// <summary>
        /// Gets the adorner layer.
        /// </summary>
        /// <value>The adorner layer.</value>
        public AdornerLayer AdornerLayer
        {
            get
            {
                if (this.adornerLayer != null)
                {
                    return this.adornerLayer;
                }
                else
                {
                    this.adornerLayer = AdornerLayer.GetAdornerLayer((Visual)this);
                    return AdornerLayer;
                }
            }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"></see> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"></see> is set to true internally.
        /// </summary>
        /// <param name="e">Arguments of the event.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.OnPreviewMouseLeftButtonDown);
            this.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.OnPreviewMouseLeftButtonUp);
            this.SelectionChanged += new SelectionChangedEventHandler(this.OnSelectionChanged);
            this.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.OnPreviewMouseMove);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the SelectionChanged event of the ReorderableListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dragging && !this.dragItemSelected)
            {
                this.dragItemSelected = true;
                this.originalItemIndex = this.SelectedIndex;

                ListBoxItem listBoxItem = (ListBoxItem)ItemContainerGenerator.ContainerFromIndex(SelectedIndex);

                this.overlayElement = new DropPreviewAdorner((UIElement)this, listBoxItem);

                this.AdornerLayer.Add(this.overlayElement);
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the ReorderableListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.dragStartPoint = e.GetPosition(this);
                this.dragging = true;
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonUp event of the ReorderableListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.dragging && this.dragItemSelected)
            {
                this.dragging = false;
                this.dragItemSelected = false;

                this.adornerLayer.Remove(this.overlayElement);

                object originalItem = this.Items[this.originalItemIndex];

                RoutedEventArgs routedEventArgs = new RoutedEventArgs(ItemsReorderedEvent, this);
                this.RaiseEvent(routedEventArgs);
            }
        }

        /// <summary>
        /// Handles the PreviewMouseMove event of the ReorderableListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && this.dragging)
            {
                if (this.overlayElement != null)
                {
                    Point currentPosition = (Point)e.GetPosition((IInputElement)this);
                    this.overlayElement.LeftOffset = currentPosition.X;
                    this.overlayElement.TopOffset = currentPosition.Y;
                }
            }
        }
        #endregion
    }




    /// <summary>
    /// DropPreviewAdorner
    /// </summary>
    public class DropPreviewAdorner : Adorner
    {
        #region Private Members
        /// <summary>
        /// child
        /// </summary>
        private Rectangle child;

        /// <summary>
        /// leftOffset
        /// </summary>
        private double leftOffset;

        /// <summary>
        /// topOffset
        /// </summary>
        private double topOffset;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DropPreviewAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="adorningElement">The adorning element.</param>
        public DropPreviewAdorner(UIElement adornedElement, UIElement adorningElement)
            : base(adornedElement)
        {
            VisualBrush brush = new VisualBrush(adorningElement);
            this.child = new Rectangle();
            this.child.Width = adorningElement.RenderSize.Width;
            this.child.Height = adorningElement.RenderSize.Height;
            this.child.Fill = brush;
            this.child.IsHitTestVisible = false;
            System.Windows.Media.Animation.DoubleAnimation animation;
            animation = new System.Windows.Media.Animation.DoubleAnimation(0.3, 1, new Duration(TimeSpan.FromSeconds(1)));
            animation.AutoReverse = true;
            animation.RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever;
            brush.BeginAnimation(System.Windows.Media.Brush.OpacityProperty, animation);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the left offset.
        /// </summary>
        /// <value>The left offset.</value>
        public double LeftOffset
        {
            get
            {
                return this.leftOffset;
            }

            set
            {
                this.leftOffset = value;
                this.UpdatePosition();
            }
        }

        /// <summary>
        /// Gets or sets the top offset.
        /// </summary>
        /// <value>The top offset.</value>
        public double TopOffset
        {
            get
            {
                return this.topOffset;
            }

            set
            {
                this.topOffset = value;
                this.UpdatePosition();
            }
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>Returns either zero (no child elements) or one (one child element).</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a <see cref="T:System.Windows.Media.Transform"></see> for the adorner, based on the transform that is currently applied to the adorned element.
        /// </summary>
        /// <param name="transform">The transform that is currently applied to the adorned element.</param>
        /// <returns>A transform to apply to the adorner.</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.LeftOffset, this.TopOffset));
            return result;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"></see> object representing the amount of layout space needed by the adorner.
        /// </returns>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        /// <summary>
        /// When implemented in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see>-derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            this.child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"></see>, and returns a child at the specified index from that element's collection of child elements. However, in this override, the only valid index is zero.
        /// </summary>
        /// <param name="index">Zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception should be raised.
        /// </returns>
        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            return this.child;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the position.
        /// </summary>
        private void UpdatePosition()
        {
            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            if (adornerLayer != null)
            {
                adornerLayer.Update(AdornedElement);
            }
        }
        #endregion
    }
}

