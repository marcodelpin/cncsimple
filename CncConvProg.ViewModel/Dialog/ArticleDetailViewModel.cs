using CncConvProg.Model;
using CncConvProg.Model.FileManageUtility;
using CncConvProg.ViewModel.MainViewModel;
using CncConvProg.ViewModel.MVVM_Library;

namespace CncConvProg.ViewModel.Dialog
{
    public class ArticleDetailViewModel : ViewModelBase, IDialog
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


            }
        }
        public ProgramPreference Source { get { return _preference; } }

        private MeasureUnit _measureUnitPrevious;

        public ArticleDetailViewModel()
        {
        }

        public void Save(IMainViewModel mainViewModel)
        {
            PathFolderHelper.SavePreferenceFile(_preference);

            if(_measureUnitPrevious != _preference.DefaultMeasureUnit)
            {
                // quando viene cambiato unità di misura viene richiesto di creare nuovo file
                mainViewModel.RequestNewFile();
            }

        }

        //public double SecureDistance
        //{
        //    get
        //    {
        //        if (_preference.DefaultMeasureUnit == MeasureUnit.Millimeter)
        //            return _preference.SecureDistanceMm;

        //        return _preference.SecureDistanceInch;
        //    }

        //    set
        //    {
        //        if (_preference.DefaultMeasureUnit == MeasureUnit.Millimeter)
        //            _preference.SecureDistanceMm = value;
        //        else
        //            _preference.SecureDistanceInch = value;

        //        OnPropertyChanged("SecureDistance");
        //    }
        //}

        //public double RapidSecureFeed
        //{
        //    get
        //    {
        //        if (_preference.DefaultMeasureUnit == MeasureUnit.Millimeter)
        //            return _preference.RapidSecureFeedMm;

        //        return _preference.RapidSecureFeedInch;
        //    }

        //    set
        //    {
        //        if (_preference.DefaultMeasureUnit == MeasureUnit.Millimeter)
        //            _preference.RapidSecureFeedMm = value;
        //        else
        //            _preference.RapidSecureFeedInch = value;

        //        OnPropertyChanged("RapidSecureFeed");
        //    }
        //}
    }
}