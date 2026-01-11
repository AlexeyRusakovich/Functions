using Caliburn.Micro;
using Functions.ViewModels;
using System.Windows;

namespace Functions;

public class Bootstrapper : BootstrapperBase
{
    public Bootstrapper()
    {
        Initialize();
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        DisplayRootViewForAsync<FunctionsDashboardViewModel>();
    }
}
