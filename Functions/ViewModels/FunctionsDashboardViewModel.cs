using Caliburn.Micro;
using ScottPlot;
using ScottPlot.WPF;
using System.Collections.ObjectModel;

namespace Functions.ViewModels;

public class FunctionsDashboardViewModel : Screen
{
    public FunctionPointsListViewModel FunctionPointsListViewModel { get; set; } = new();
    public WpfPlot PlotControl { get; private set; } = new WpfPlot();

    public FunctionsDashboardViewModel()
    {
        FunctionPointsListViewModel.FunctionPointsDataChanged += OnFunctionPointsDataChanged;
        var plot = PlotControl.Multiplot.GetPlot(0);
        plot.XLabel("Абсолютная отметка [мм]");
        plot.YLabel("Температура [°C]");
    }

    private void OnFunctionPointsDataChanged(object? sender, ObservableCollection<FunctionPointViewModel> e)
    {
        var plot = PlotControl.Multiplot.GetPlot(0);
        var xs = e.Select(x => x.X).ToArray();
        var ys = e.Select(x => x.Y).ToArray();
        plot.Clear();
        plot.Add.ScatterLine(xs, ys);
        PlotControl.Refresh();
    }
}
