using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using CncConvProg.Geometry;
using CncConvProg.ViewModel.Interface;
using CncConvProg.ViewModel.MVVM_Library;
using CncConvProg.ViewModel.VisualModel;
using MVVM_Library;

/*
 * tutto il modello viene salvato con database..
 * nel modello cadono macchina.e controllo.
 * per fare prove posso estrarre interfaccia cosi da poter aver quello definitivo e quello da fare prove..
 * 
 * man mano che leggo lancio comandi..
 * 
 * le due cose devono essere disgiunte..
 * 
 * ok per interfaccia ,,
 * 
 * ma devo riuscire a verificare code processor anche in altro loco.. da console.
 * 
 * non deve avere un collegamento stretto con view.
 * 
 * 
 * 
 * 
 * es leggo m8
 * 
 * move tool -> xt
 * 
 * icncactionexecuter
 * 
 *  consoleExectter: iccn..
 *      stampo m8
 *      
 *  viewExexture :
 *      icona acqua
 *      
 *  immsgiasnio che controller sia
 *  ew 
 *  view
 *  
 * il controller da minima info.
 *  ovvero spostamento. e varie info.
 *  
 * deve esserci 
 *  ilathe2x
 *  ilahe3x
 *  mill2d
 * codeprocessorcore(imachine, icontroller)
 * {
 * 
 * }
 * 
 * saggio prima avanzare un'attimo con il code processor..
 *  
 * 
 */

namespace CncConvProg.ViewModel
{
    public class TurningInsert // Itool
    {
        public Profile2D InsertProfile;
    }
    public class ProgramSimulationViewModel : ViewModelBase, IScreen
    {
        public event EventHandler OnRefreshCalled;

        public void RequestDataRefresh()
        {
            var handler = OnRefreshCalled;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }


        private readonly GraphicLayer _graphic = new GraphicLayer();

        public ProgramSimulationViewModel()
        {
            ProgText = @"%
O100(TORNIO)G50 S2000 Q1000
G0G99G54
N1T0202(SGR.EST)G99G54
G96200M3
G0X150Z2
G1X-2F.2
G0X8Z2
G1A-90C3
A-45X12Z-2C3
G0X300Z200
%";

            var turningInsert = new TurningInsert();
            _graphic.CreateLatheStock(10, 10, -10);
            _graphic.ChangeTool(turningInsert);



            ModelVisual = _graphic.GetModel();
            RequestDataRefresh();


        }

        public ModelVisual3D ModelVisual { get; private set; }

        private string _progText;
        public string ProgText
        {
            get { return _progText; }

            set
            {
                _progText = value;
                OnPropertyChanged("ProgText");
            }
        }

        public IMainScreen Parent
        {
            get { throw new NotImplementedException(); }
        }

        public void Refresh()
        {
            ModelVisual = _graphic.GetModel();
            RequestDataRefresh();
        }

        #region Start Simulation

        RelayCommand _startSimulation;

        private void StartSimulation()
        {

        }

        public ICommand StartSimulationCmd
        {
            get
            {
                return _startSimulation ?? (_startSimulation = new RelayCommand(param => StartSimulation(),
                                                                                param => true));
            }
        }

        #endregion
    }
}
