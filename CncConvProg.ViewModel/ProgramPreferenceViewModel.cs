using CncConvProg.Model;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.ViewModel.MainViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel
{
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
                OnPropertyChanged("RapidSecureFeed");
                OnPropertyChanged("SecureDistance");

                OnPropertyChanged("DefaultSecureZ");



            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ProgramPreference Source { get { return _preference; } }

        private MeasureUnit _measureUnitPrevious;

        public ProgramPreferenceViewModel(ProgramPreference preference)
        {
            _measureUnitPrevious = preference.DefaultMeasureUnit;

            _preference = preference;
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

        
    }
}