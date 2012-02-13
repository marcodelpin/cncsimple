using CncConvProg.Model;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.ViewModel.MainViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel
{
    /// <summary>
    /// Questo view model sara usato sia nel dialogo di selezione unita che nel dialogo di preferenze
    /// </summary>
    public class ProgramPreferenceViewModel : ViewModelBase, IDialog
    {
        private readonly ProgramPreference _preference;

        public string[] ComboBoxUnits
        {
            get
            {
                return new string[] { "Inch", "Millimeter" };
            }
        }

        public string UnitSelected
        {
            get
            {
                var mu = _preference.DefaultMeasureUnit;

                return mu == MeasureUnit.Inch ? "Inch" : "Millimeter";
            }
            set
            {
                _preference.DefaultMeasureUnit = value == "Inch" ? MeasureUnit.Inch : MeasureUnit.Millimeter;

                OnPropertyChanged("UnitSelected");
            }
        }
        public ProgramPreference Source { get { return _preference; } }

        private readonly MeasureUnit _measureUnitPrevious;

        public ProgramPreferenceViewModel(ProgramPreference preference)
        {
            _measureUnitPrevious = preference.DefaultMeasureUnit;

            _preference = preference;

            MmPreference = new UnitPreferenceViewModel(preference.GetPreference(MeasureUnit.Millimeter));
            InchPreference = new UnitPreferenceViewModel(preference.GetPreference(MeasureUnit.Inch));
        }

        public void Save(IMainViewModel mainViewModel)
        {
            Singleton.SetPreference(_preference);

            PathFolderHelper.SavePreferenceFile(_preference);

            if (_measureUnitPrevious != _preference.DefaultMeasureUnit)
            {
                // quando viene cambiato unità di misura viene richiesto di creare nuovo file
                mainViewModel.RequestNewFile();
            }

        }

        private UnitPreferenceViewModel _mmPreference;
        public UnitPreferenceViewModel MmPreference
        {
            get { return _mmPreference; }
            set { _mmPreference = value; OnPropertyChanged("MmPreference"); }
        }

        private UnitPreferenceViewModel _inchPreference;
        public UnitPreferenceViewModel InchPreference
        {
            get { return _inchPreference; }
            set { _inchPreference = value; OnPropertyChanged("InchPreference"); }
        }

    }

    public class UnitPreferenceViewModel : ViewModelBase
    {
        private readonly UnitPreference _model;
        public UnitPreferenceViewModel(UnitPreference model)
        {
            _model = model;
        }

        public double MillEntryExitSecureDistance
        {
            get
            {
                return _model.MillEntryExitSecureDistance;
            }
            set
            {
                _model.MillEntryExitSecureDistance = value;
                OnPropertyChanged("MillEntryExitSecureDistance");
            }
        }

        public double MillingRapidSecureFeedAsync
        {
            get
            {
                return _model.MillingRapidSecureFeedAsync;
            }
            set
            {
                _model.MillingRapidSecureFeedAsync = value;
                OnPropertyChanged("MillingRapidSecureFeedAsync");
            }
        }


        public double TurningSecureDistance
        {
            get
            {
                return _model.TurningSecureDistance;
            }
            set
            {
                _model.TurningSecureDistance = value;
                OnPropertyChanged("TurningSecureDistance");
            }
        }
        public double TurningRapidSecureFeedSync
        {
            get
            {
                return _model.TurningRapidSecureFeedSync;
            }
            set
            {
                _model.TurningRapidSecureFeedSync = value;
                OnPropertyChanged("TurningRapidSecureFeedSync");
            }
        }

        public double DistanzaSicurezzaCicliForatura
        {
            get
            {
                return _model.DistanzaSicurezzaCicliForatura;
            }
            set
            {
                _model.DistanzaSicurezzaCicliForatura = value;
                OnPropertyChanged("DistanzaSicurezzaCicliForatura");
            }
        }
    }
}