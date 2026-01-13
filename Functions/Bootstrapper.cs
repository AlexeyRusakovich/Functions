using Autofac;
using Caliburn.Micro;
using Functions.Services;
using Functions.ViewModels;
using System.Windows;

namespace Functions;

public class Bootstrapper : BootstrapperBase
{
    private IContainer _container = null!;

    public Bootstrapper()
    {
        Initialize();
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        DisplayRootViewForAsync<FunctionsDashboardViewModel>();
    }

    protected override object GetInstance(Type serviceType, string key)
    {
        return string.IsNullOrWhiteSpace(key)
            ? _container.Resolve(serviceType)
            : _container.ResolveKeyed(key, serviceType);
    }

    protected override void Configure()
    {
        ContainerBuilder builder = new();

        builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();

        builder.RegisterType<FunctionsDashboardViewModel>().AsSelf().SingleInstance();

        builder.RegisterType<FunctionsDataToJsonSaver>().AsImplementedInterfaces();
        builder.RegisterType<FunctionsDataTopClipboardSaver>().AsImplementedInterfaces();

        _container = builder.Build();
    }

    protected override IEnumerable<object> GetAllInstances(Type serviceType)
    {
        var collectionType = typeof(IEnumerable<>).MakeGenericType(serviceType);
        return (IEnumerable<object>)_container.Resolve(collectionType);
    }

    protected override void BuildUp(object instance)
    {
        _container.InjectProperties(instance);
    }
}
