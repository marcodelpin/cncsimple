using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CncConvProg.View.Controls
{
    public class SilverlightishPopup
    {
        private readonly Rectangle _maskRectangle = new Rectangle
                                              {
                                                  Fill = new SolidColorBrush(Colors.DarkGray),
                                                  Opacity = 0.0
                                              };

        public FrameworkElement Parent { get; set; }
        public FrameworkElement Content { get; set; }

        public SilverlightishPopup()
        {
            var button = new Button {Width = 100, Height = 200, Content = "I am the popup!"};

            button.Click += delegate
                                {
                                    Close();
                                };
            Content = button;
        }

        public void Show()
        {
            Grid grid = GetRootGrid();
            if (grid != null)
            {
                var opacityAnimation = new DoubleAnimation(0.5, new Duration(TimeSpan.FromSeconds(0.5))); Storyboard opacityBoard = new Storyboard(); opacityBoard.Children.Add(opacityAnimation); Storyboard.SetTarget(opacityAnimation, _maskRectangle); Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("(Opacity)")); opacityBoard.Completed += delegate { ScaleTransform scaleTransform = new ScaleTransform(0.0, 0.0, Content.Width / 2.0, Content.Height / 2.0); Content.RenderTransform = scaleTransform; grid.Children.Add(Content); Storyboard scaleBoard = new Storyboard(); DoubleAnimation scaleXAnimation = new DoubleAnimation(1.0, TimeSpan.FromSeconds(0.5)); scaleBoard.Children.Add(scaleXAnimation); Storyboard.SetTarget(scaleXAnimation, Content); Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)")); DoubleAnimation scaleYAnimation = new DoubleAnimation(1.0, TimeSpan.FromSeconds(0.5)); scaleBoard.Children.Add(scaleYAnimation); Storyboard.SetTarget(scaleYAnimation, Content); Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)")); scaleBoard.Begin(); }; opacityBoard.Begin(); grid.Children.Add(_maskRectangle);
            }
        }
        public void Close()
        {
            var grid = GetRootGrid();

            if (grid != null)
            {
                var scaleTransform = new ScaleTransform(1.0, 1.0, Content.Width / 2.0, Content.Height / 2.0); Content.RenderTransform = scaleTransform; Storyboard scaleBoard = new Storyboard(); DoubleAnimation scaleXAnimation = new DoubleAnimation(0.0, TimeSpan.FromSeconds(0.5)); scaleBoard.Children.Add(scaleXAnimation); Storyboard.SetTarget(scaleXAnimation, Content); Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)")); DoubleAnimation scaleYAnimation = new DoubleAnimation(0.0, TimeSpan.FromSeconds(0.5)); scaleBoard.Children.Add(scaleYAnimation); Storyboard.SetTarget(scaleYAnimation, Content); Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)")); scaleBoard.Completed += delegate
                {
                    var opacityAnimation = new DoubleAnimation(0.5, 0.0, new Duration(TimeSpan.FromSeconds(0.5))); Storyboard opacityBoard = new Storyboard(); opacityBoard.Children.Add(opacityAnimation); Storyboard.SetTarget(opacityAnimation, _maskRectangle); Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("(Opacity)")); opacityBoard.Completed += delegate
                    {
                        grid.Children.Remove(_maskRectangle); grid.Children.Remove(Content);
                    };

                    opacityBoard.Begin();
                };

                scaleBoard.Begin();
            }
        }
        private Grid GetRootGrid()
        {
            var root = Parent;

            while (root is FrameworkElement && root.Parent != null)
            {
                var rootElement = root as FrameworkElement;

                if (rootElement.Parent is FrameworkElement)
                {
                    root = rootElement.Parent as FrameworkElement;
                }
            }
            var contentControl = root as ContentControl;

            return contentControl.Content as Grid;
        }
    }
}
