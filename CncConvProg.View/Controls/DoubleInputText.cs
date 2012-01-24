using System.Windows;
using System.Windows.Controls;

namespace CncConvProg.View.Controls
{
    public class DoubleInputText : Control
    {
        public DoubleInputText()
        {
            IsTabStop = false;
        }
        static DoubleInputText()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleInputText), new FrameworkPropertyMetadata(typeof(DoubleInputText)));
        }

        #region DependencyProperty InputContent

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty InputContentProperty =
            DependencyProperty.Register("InputContent", typeof(object), typeof(DoubleInputText),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public object InputContent
        {
            get { return (string)GetValue(InputContentProperty); }
            set { SetValue(InputContentProperty, value); }
        }

        #endregion

        #region DependencyProperty InputLabel

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty InputLabelProperty =
            DependencyProperty.Register("InputLabel", typeof(string), typeof(DoubleInputText),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure
                  ));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public string InputLabel
        {
            get { return (string)GetValue(InputLabelProperty); }
            set { SetValue(InputLabelProperty, value); }
        }

        #endregion

        #region DependencyProperty ToolTip Text

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register("ToolTipText", typeof(string), typeof(DoubleInputText),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public string ToolTipText
        {
            get { return (string)GetValue(ToolTipTextProperty); }
            set { SetValue(ToolTipTextProperty, value); }
        }

        #endregion

        #region DependencyProperty Image Path

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(DoubleInputText),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        #endregion

        #region DependencyProperty Show Tooltip

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty ShowTooltipProperty =
            DependencyProperty.Register("ShowTooltip", typeof(bool), typeof(DoubleInputText),
            new FrameworkPropertyMetadata(false,
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public bool ShowTooltip
        {
            get { return (bool)GetValue(ShowTooltipProperty); }
            set { SetValue(ShowTooltipProperty, value); }
        }

        #endregion

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty InputContent1Property =
            DependencyProperty.Register("InputContent1", typeof(object), typeof(DoubleInputText),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public object InputContent1
        {
            get { return GetValue(InputContent1Property); }
            set { SetValue(InputContent1Property, value); }
        }

        #region DependencyProperty InputLabel

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty InputLabel1Property =
            DependencyProperty.Register("InputLabel1", typeof(string), typeof(DoubleInputText),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure
                  ));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public string InputLabel1
        {
            get { return (string)GetValue(InputLabel1Property); }
            set { SetValue(InputLabel1Property, value); }
        }

        #endregion

        #region DependencyProperty ToolTip Text

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty ToolTipText1Property =
            DependencyProperty.Register("ToolTipText1", typeof(string), typeof(DoubleInputText),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public string ToolTipText1
        {
            get { return (string)GetValue(ToolTipText1Property); }
            set { SetValue(ToolTipText1Property, value); }
        }

        #endregion

        #region DependencyProperty Image Path

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty ImagePath1Property =
            DependencyProperty.Register("ImagePath1", typeof(string), typeof(DoubleInputText),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public string ImagePath1
        {
            get { return (string)GetValue(ImagePath1Property); }
            set { SetValue(ImagePath1Property, value); }
        }

        #endregion

        #region DependencyProperty Show Tooltip

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty ShowTooltip1Property =
            DependencyProperty.Register("ShowTooltip1", typeof(bool), typeof(DoubleInputText),
            new FrameworkPropertyMetadata(false,
                  FrameworkPropertyMetadataOptions.AffectsRender |
                  FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public bool ShowTooltip1
        {
            get { return (bool)GetValue(ShowTooltip1Property); }
            set { SetValue(ShowTooltip1Property, value); }
        }

        #endregion

    }
}
