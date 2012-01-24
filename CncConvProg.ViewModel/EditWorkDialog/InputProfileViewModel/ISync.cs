using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.Geometry.RawProfile2D;

namespace CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel
{
    internal interface ISync
    {
        List<RawEntity2D> GetProfile();

    }
}
