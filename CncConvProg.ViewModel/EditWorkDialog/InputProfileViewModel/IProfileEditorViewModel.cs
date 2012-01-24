using System;
using System.Collections.Generic;
using CncConvProg.Geometry.RawProfile2D;

namespace CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel
{
    public interface IProfileEditorViewModel
    {
        IEnumerable<RawEntity2D> Source { get; }
        event EventHandler OnSourceUpdated;

    }
}