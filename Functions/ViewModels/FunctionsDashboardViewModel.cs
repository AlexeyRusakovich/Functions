using Caliburn.Micro;
using Functions.Interfaces;
using Functions.Services;
using ScottPlot;
using ScottPlot.WPF;
using System.Collections.ObjectModel;

namespace Functions.ViewModels;

public class FunctionsDashboardViewModel : Screen, IHasChanges
{
    public ObservableCollection<FunctionManagerViewModel> FunctionsManagers { get; private set; } = [];
    public FunctionManagerViewModel? SelectedFunctionManager { get; set; }
    public WpfPlot PlotControl { get; private set; } = new WpfPlot();
    public bool HasChanges { get; private set; }

    private IFunctionsDataToJsonSaver _functionsDataToJsonSaver;

    public FunctionsDashboardViewModel(IFunctionsDataToJsonSaver functionsDataToJsonSaver)
    {
        _functionsDataToJsonSaver = functionsDataToJsonSaver;

        CreateNewFunction();

        var plot = PlotControl.Plot;
        plot.XLabel("Абсолютная отметка [мм]");
        plot.YLabel("Температура [°C]");
    }

    public void AddNewItem()
    {
        if (SelectedFunctionManager == null)
        {
            CreateNewFunction();
        }
        else
        {
            SelectedFunctionManager.CreateNewPoint();
        }
    }

    public void CreateNewFunction()
    {
        AddNewFunction(new FunctionViewModel());
    }

    public void AddNewFunction(FunctionViewModel function)
    {
        var newFunction = new FunctionManagerViewModel(_functionsDataToJsonSaver, function);
        newFunction.FunctionDataChanged += OnFunctionPointsDataChanged;
        FunctionsManagers.Add(newFunction);
    }

    public void RemoveFunction(FunctionManagerViewModel functionManager)
    {
        FunctionsManagers.Remove(functionManager);
        functionManager.Dispose();
    }

    public void ResetSelectedFunctionManager()
    {
        SelectedFunctionManager = null;
    }

    public void SaveToFile()
    {
        if (SelectedFunctionManager == null)
        {
            _functionsDataToJsonSaver.SaveToFile(FunctionsManagers.Select(x => x.Function));
        }
        else
        {
            _functionsDataToJsonSaver.SaveToFile([SelectedFunctionManager!.Function]);
        }
    }

    public void GetFromFile()
    {
        if (SelectedFunctionManager == null)
        {
            var functions = _functionsDataToJsonSaver.GetFunctionsDataFromFile();
            if (functions != null)
            {
                foreach (var function in functions)
                {
                    AddNewFunction(function);
                }
            }
        }
        else
        {
            var function = _functionsDataToJsonSaver.GetFunctionsDataFromFile()?.FirstOrDefault();
            if (function != null)
            {
                AddNewFunction(function);
            }
        }
    }

    private void OnFunctionPointsDataChanged(object? sender, FunctionViewModel e)
    {
        PlotControl.Plot.Clear();

        foreach (var functionManager in FunctionsManagers)
        {
            var xs = e.Points.Select(point => point.X).ToArray();
            var ys = e.Points.Select(point => point.Y).ToArray();

            AddScatterLine(xs, ys, functionManager.Function.Name);
        }

        PlotControl.Refresh();
        HasChanges = true;
    }

    private void AddScatterLine(double[] xs, double[] ys, string name, bool isInverted = false)
    {
        var plot = PlotControl.Plot;

        var scatterLine = plot.Add.ScatterLine(xs, ys, Color.RandomHue());
        scatterLine.MarkerShape = MarkerShape.FilledCircle;
        scatterLine.MarkerSize = 5;
        scatterLine.LegendText = name;

        if (isInverted)
        {
            var invertedScatterLine = plot.Add.ScatterLine(ys, xs, scatterLine.Color);
            invertedScatterLine.LineStyle = new LineStyle(1, scatterLine.Color, LinePattern.Dotted);
            invertedScatterLine.LegendText = $"Обратная функция: {name}";
        }
    }
}
