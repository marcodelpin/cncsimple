using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncConvProg.View.Dialog;
using CncConvProg.ViewModel;
using CncConvProg.ViewModel.EditWorkDialog.InputProfileViewModel;
using CncConvProg.ViewModel.MainViewModel;
using Framework.Abstractions.Wpf.Intefaces;
using Framework.Abstractions.Wpf.ServiceLocation;
using Framework.Implementors.Wpf.MVVM.Services;
using ServiceLocation.Silverlight.Unity;

/*
 * --- from tenac v2
 */
namespace CncConvProg.View
{
    public class Bootstrapper
    {
        public static void InitializeIoc()
        {
            SimpleServiceLocator.SetServiceLocatorProvider(new UnityServiceLocator());
            SimpleServiceLocator.Instance.Register<IModalDialogService, ModalDialogService>();
            SimpleServiceLocator.Instance.Register<IMessageBoxService, MessageBoxService>();
            SimpleServiceLocator.Instance.Register<IMainViewModel, MainViewModel>();
            SimpleServiceLocator.Instance.Register<IProfileEditorViewModel, ProfileEditorViewModel>();

            SimpleServiceLocator.Instance.Register<IModalWindow, EditWorkView>(Constants.EditUserModalDialog);
            SimpleServiceLocator.Instance.Register<IModalWindow, ArticleDetailView>(Constants.ArticleDetailDialog);
            SimpleServiceLocator.Instance.Register<IModalWindow, ThreadTableView>(Constants.TabellaFilettaturaModalDialog);
            SimpleServiceLocator.Instance.Register<IModalWindow, MachinesDialogView>(Constants.MacchineModalDialog);
            //SimpleServiceLocator.Instance.Register<IModalWindow, MaterialsDialogView>(Constants.MaterialiModalDialog);
            SimpleServiceLocator.Instance.Register<IModalWindow, ToolsDialogView>(Constants.UtensiliModalDialog);
            SimpleServiceLocator.Instance.Register<IModalWindow, EditPhaseDetailView>(Constants.PhaseDetailEditModalDialog);
            SimpleServiceLocator.Instance.Register<IModalWindow, SelectionUnitView>(Constants.PreferenceModalDialog);



        }
    }
}
