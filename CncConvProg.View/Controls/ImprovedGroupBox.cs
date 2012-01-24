using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CncConvProg.View.Controls
{
    public class ImprovedGroupBox : GroupBox
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var grid = GetVisualChild(0) as Grid;
            if (grid == null || grid.Children.Count <= 3)
                return;

            var bd = grid.Children[3] as Border;
            if (bd != null)
                bd.IsHitTestVisible = false;
        }
    }

}
