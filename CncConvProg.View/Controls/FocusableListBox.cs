using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CncConvProg.View.Controls
{
    public class FocusableListBox : ListBox
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        { return (item is FocusableListBoxItem); } protected override System.Windows.DependencyObject GetContainerForItemOverride() { return new FocusableListBoxItem(); }
    }

    public class FocusableListBoxItem : ListBoxItem
    {
        public FocusableListBoxItem()
        {
            IsTabStop = false;
            GotFocus += FocusableListBoxItem_GotFocus;

        }
        void FocusableListBoxItem_GotFocus(object sender, RoutedEventArgs e)
        {
            object obj = ParentListBox.ItemContainerGenerator.ItemFromContainer(this); ParentListBox.SelectedItem = obj;
        }
        private ListBox ParentListBox { get { return (ItemsControl.ItemsControlFromItemContainer(this) as ListBox); } }
    }
}
