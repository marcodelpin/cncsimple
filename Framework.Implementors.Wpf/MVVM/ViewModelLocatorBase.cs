using System.ComponentModel;
using System.Windows;
using Framework.Abstractions.Wpf.ServiceLocation;

namespace Framework.Implementors.Wpf.MVVM
{
  public abstract class ViewModelLocatorBase<TViewModel> : NotifyPropertyChangedEnabledBase where TViewModel : class 
  {
    private static bool? _isInDesignMode;
    private TViewModel _runtimeViewModel;
    private TViewModel _designtimeViewModel;

    /// <summary>
    /// Gets a value indicating whether the control is in design mode
    /// (running in Blend or Visual Studio).
    /// </summary>
    public static bool IsInDesignMode
    {
      get
      {
        if (!_isInDesignMode.HasValue)
        {
           var prop = DesignerProperties.IsInDesignModeProperty;
           _isInDesignMode = (bool)DependencyPropertyDescriptor
                            .FromProperty(prop, typeof(FrameworkElement))
                            .Metadata.DefaultValue;

        }
                
        return _isInDesignMode.Value;
      }
    }

    protected TViewModel RuntimeViewModel
    {
      get
      {
        if (this._runtimeViewModel == null)
        {
          this.RuntimeViewModel = SimpleServiceLocator.Instance.Get<TViewModel>();
        }
        return _runtimeViewModel;
      }

      set 
      { 
        _runtimeViewModel = value;
        this.OnPropertyChanged("ViewModel");
      }
    }

    public TViewModel ViewModel
    {
      get
      {
        return IsInDesignMode ? this.DesigntimeViewModel : this.RuntimeViewModel;
      }
    }

    public TViewModel DesigntimeViewModel
    {
      get
      {
        return _designtimeViewModel;
      }
      
      set 
      { 
        _designtimeViewModel = value;
        this.OnPropertyChanged("ViewModel");
      }
    }
  }
}