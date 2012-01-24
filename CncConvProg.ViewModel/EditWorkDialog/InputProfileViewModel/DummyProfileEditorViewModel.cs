using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CncConvProg.Geometry.Entity;
using CncConvProg.Geometry.RawProfile2D;
using CncConvProg.ViewModel.MVVM_Library;
using Framework.Implementors.Wpf.MVVM;

namespace CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel
{
    public class DummyProfileEditorViewModel : IProfileEditorViewModel
    {
        static public List<RawEntity2D> MoveListViewModels { get; set; }

        static DummyProfileEditorViewModel()
        {
            MoveListViewModels = new List<RawEntity2D>();

            var profileSolver = new RawProfile(false);

            var ini = new RawInitPoint2D(profileSolver);
            ini.X.SetValue(true, 0);
            ini.Y.SetValue(true, 0);

            var line1 = new RawLine2D(profileSolver);
            line1.DeltaY.SetValue(true, 10);

            var line2 = new RawLine2D(profileSolver);
            line2.DeltaX.SetValue(true, -10);

            MoveListViewModels.Add(ini);
            MoveListViewModels.Add(line1);
            MoveListViewModels.Add(line2);

            SelectedMove = MoveListViewModels.LastOrDefault();

            // se cambio SelectedMove aggiorno currentMove
            CurrentMovement = new RawLineViewModel(line2,ProfileEditorViewModel.AxisSystem.Xy);


            //foreach (var rawEntity2D in MoveListViewModels)
            //    profileSolver.Add(rawEntity2D);

            //var profileSolved = profileSolver.GetProfileResult();

            //var listEntity = new List<IEntity2D>();

            //foreach (var entity2D in profileSolved.Source)
            //    listEntity.Add(entity2D);

            //Preview = new ObservableCollection<IEntity2D>(listEntity);
        }

        private static ObservableCollection<IEntity2D> Preview { get; set; }

        static public RawEntity2D SelectedMove { get; set; }

        static public ViewModelBase CurrentMovement
        {
            get;
            set;
        }



        public IEnumerable<RawEntity2D> Source
        {
            get { throw new System.NotImplementedException(); }
        }

        public event EventHandler OnSourceUpdated;
    }
}
