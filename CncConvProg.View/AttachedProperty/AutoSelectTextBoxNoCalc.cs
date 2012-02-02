using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CncConvProg.View.AuxClass;

namespace CncConvProg.View.AttachedProperty
{
    public class AttachedPropertiesNoCalc
    {
        /// <summary>
        /// AutoSelectText dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoSelectTextNoCalcProperty = DependencyProperty.RegisterAttached("AutoSelectTextNoCalc",
                                                                               typeof(Boolean),
                                                                               typeof(AttachedProperties),
                                                                               new PropertyMetadata(OnAutoSelectTextChanged));

        /// <summary>
        /// PreventAutoSelectText dependency property.
        /// </summary>
        public static readonly DependencyProperty PreventAutoSelectTextNoCalcProperty = DependencyProperty.RegisterAttached("PreventAutoSelectTextNoCalc",
                                                                              typeof(Boolean),
                                                                              typeof(AttachedProperties),
                                                                              null);


        /// <summary>
        /// This method is called when the value of the AutoSelectText property
        /// is set from the xaml
        /// </summary>
        /// <param name="d">The content control on which the property is set.</param>
        /// <param name="e"></param>
        private static void OnAutoSelectTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //This works because of event bubbling. 
            var textBox = d as TextBox;
            if (textBox != null)
            {
                if ((bool)e.NewValue)
                {
                    textBox.GotFocus += OnGotFocus;
                    textBox.PreviewMouseLeftButtonDown += SelectivelyIgnoreMouseButton;
                    textBox.PreviewKeyDown += textBox_PreviewKeyDown;
                }
                else
                {
                    textBox.GotFocus -= OnGotFocus;
                    textBox.PreviewMouseLeftButtonDown -= SelectivelyIgnoreMouseButton;

                }
            }
        }

        static void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Decimal &&

       NumberFormatInfo.CurrentInfo.NumberDecimalSeparator != ".")
            {

                var _this = (sender as TextBox);

                if (_this != null)
                {

                    e.Handled = true;



                    var eventArgs = new TextCompositionEventArgs(

                        Keyboard.PrimaryDevice,

                        new TextComposition(

                            InputManager.Current,

                            Keyboard.FocusedElement,

                            NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))

                    {

                        RoutedEvent = ContentElement.TextInputEvent

                    };



                    InputManager.Current.ProcessInput(eventArgs);

                }

            }


        }

        private static void SelectivelyIgnoreMouseButton(object sender,
                                                   MouseButtonEventArgs e)
        {
            // Find the TextBox 
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent == null) return;

            var textBox = (TextBox)parent;
            if (textBox.IsKeyboardFocusWithin) return;

            // If the text box is not yet focussed, give it the focus and 
            // stop further processing of this click event. 
            textBox.Focus();
            e.Handled = true;
        }


        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            //Since we are using routed events, the sender parameter will not be the textbox that currently has focus. 
            //It will the root level content control (Grid) which has the AutoSelectText attached property.
            //The FocusManager class is used to get a reference to the control that has the focus.
            var textBox = sender as TextBox;

            //if (textBox != null && !(bool)textBox.GetValue(PreventAutoSelectTextProperty))
            if (textBox == null) return;
            textBox.Select(0, textBox.Text.Length);

            /*Show tooltip if present */
            //if (textBox.ToolTip == null) return;

            //if (!(textBox.ToolTip is ToolTip)) return;

            //var tt = (ToolTip)textBox.ToolTip;

            //tt.PlacementTarget = textBox;
            //tt.Placement = PlacementMode.Right;

            //tt.IsOpen = true;


        }

        #region Dependency property Get/Set
        public static Boolean GetAutoSelectTextNoCalc(DependencyObject target)
        {
            return (Boolean)target.GetValue(AutoSelectTextNoCalcProperty);
        }
        public static void SetAutoSelectTextNoCalc(DependencyObject target, Boolean value)
        {
            target.SetValue(AutoSelectTextNoCalcProperty, value);
        }
        public static Boolean GetPreventAutoSelectTextNoCalc(DependencyObject target)
        {
            return (Boolean)target.GetValue(PreventAutoSelectTextNoCalcProperty);
        }
        public static void SetPreventAutoSelectTextNoCalc(DependencyObject target, Boolean value)
        {
            target.SetValue(PreventAutoSelectTextNoCalcProperty, value);
        }
        #endregion
    }
}