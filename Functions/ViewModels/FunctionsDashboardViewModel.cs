using Caliburn.Micro;
using Functions.Interfaces;
using Functions.Services;
using ScottPlot;
using ScottPlot.WPF;
using System.Collections.ObjectModel;

namespace Functions.ViewModels;

public class FunctionsDashboardViewModel : Screen, IHasChanges
{
    public FunctionPointsListViewModel FunctionPointsListViewModel { get; private set; }
    public WpfPlot PlotControl { get; private set; } = new WpfPlot();
    public bool HasChanges { get; private set; }

    public FunctionsDashboardViewModel(IFunctionsDataToJsonSaver functionsDataToJsonSaver)
    {
        FunctionPointsListViewModel = new FunctionPointsListViewModel(functionsDataToJsonSaver);
        FunctionPointsListViewModel.FunctionPointsDataChanged += OnFunctionPointsDataChanged;
        FunctionPointsListViewModel.FunctionPointsDataChanged += (o ,e) => HasChanges = true;
        var plot = PlotControl.Multiplot.GetPlot(0);
        plot.XLabel("Абсолютная отметка [мм]");
        plot.YLabel("Температура [°C]");
    }

    private void OnFunctionPointsDataChanged(object? sender, (bool IsInverted, ObservableCollection<FunctionPointViewModel> Points) e)
    {
        var plot = PlotControl.Multiplot.GetPlot(0);
        
        var xs = e.Points.Select(point => point.X).ToArray();
        var ys = e.Points.Select(point => point.Y).ToArray();
        
        plot.Clear();
        var scatter = plot.Add.ScatterLine(xs, ys, Color.RandomHue());
        scatter.LegendText = "Function";

        if (e.IsInverted)
        {
            var invertedScatter = plot.Add.ScatterLine(ys, xs, scatter.Color);
            invertedScatter.LineStyle = new LineStyle(1, scatter.Color, LinePattern.Dotted);
            invertedScatter.LegendText = "Inverted function";
        }

        PlotControl.Refresh();
    }
}
