using System;
using System.Windows;
using ICSharpCode.AvalonEdit;
using System.Windows.Data;

namespace CncConvProg.View.AttachedProperty
{
   
    public static class AvalonEditorEx
    {
        public static readonly DependencyProperty BoundDocument =
           DependencyProperty.RegisterAttached("BoundDocument", typeof(string), typeof(AvalonEditorEx),
           new FrameworkPropertyMetadata(null,
               FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
               OnBoundDocumentChanged)
               );

        private static void OnBoundDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as TextEditor;
            if (box == null)
                return;

            RemoveEventHandler(box);

            string content = GetBoundDocument(d);

            box.Text = content;

            AttachEventHandler(box);

        }

        private static void RemoveEventHandler(TextEditor box)
        {
            Binding binding = BindingOperations.GetBinding(box, BoundDocument);


            if (binding != null)
            {
                if (binding.UpdateSourceTrigger == UpdateSourceTrigger.Default ||
                    binding.UpdateSourceTrigger == UpdateSourceTrigger.LostFocus)
                {
                    box.LostFocus -= box_LostFocus;
                }
                else
                {
                    box.TextChanged -= box_TextChanged;

                }
            }
        }



        private static void AttachEventHandler(TextEditor box)
        {
            Binding binding = BindingOperations.GetBinding(box, BoundDocument);

            if (binding != null)
            {
                if (binding.UpdateSourceTrigger == UpdateSourceTrigger.Default ||
                    binding.UpdateSourceTrigger == UpdateSourceTrigger.LostFocus)
                {

                    box.LostFocus += new RoutedEventHandler(box_LostFocus);
                }
                else
                {
                    box.TextChanged += new EventHandler(box_TextChanged);
                }
            }
        }

        static void box_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = sender as TextEditor;

            var content = box.Text;
            SetBoundDocument(box, content);
        }

        static void box_TextChanged(object sender, EventArgs e)
        {
            var box = sender as TextEditor;

            var content = box.Text;
            SetBoundDocument(box, content);
        }

        public static string GetBoundDocument(DependencyObject dp)
        {
            var value = dp.GetValue(BoundDocument) as string;

            return value;
        }

        public static void SetBoundDocument(DependencyObject dp, string value)
        {
            dp.SetValue(BoundDocument, value);
        }
    }


}
