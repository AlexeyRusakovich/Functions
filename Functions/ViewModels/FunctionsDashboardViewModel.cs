using Caliburn.Micro;
using Functions.Helpers;
using Functions.Interfaces;
using Functions.Services;
using ScottPlot;
using ScottPlot.WPF;
using System.Collections.ObjectModel;

namespace Functions.ViewModels;

public class FunctionsDashboardViewModel : Screen, IHasChanges
{
    public ObservableCollection<FunctionTableViewModel> FunctionsTables { get; private set; } = [];
    public ObservableCollection<FunctionTableViewModel> SelectedFunctionTables { get; private set; } = [];
    public WpfPlot PlotControl { get; private set; } = new WpfPlot();
    public bool HasChanges { get; private set; }

    private Dictionary<Guid, Color> _functionColorsDictionary = [];
    private readonly IFunctionsDataToJsonSaver _functionsDataToJsonSaver;
    private readonly IFunctionsDataToClipboardSaver _functionsDataToClipboardSaver;

    public FunctionsDashboardViewModel(
        IFunctionsDataToJsonSaver functionsDataToJsonSaver,
        IFunctionsDataToClipboardSaver functionsDataToClipboardSaver)
    {
        _functionsDataToJsonSaver = functionsDataToJsonSaver;
        _functionsDataToClipboardSaver = functionsDataToClipboardSaver;

        CreateNewFunction();
        SetPlotLabels();
    }

    public void AddNewItem()
    {
        if (!SelectedFunctionTables.Any())
        {
            CreateNewFunction();
        }
        else
        {
            foreach (var selectedFunctionTable in SelectedFunctionTables)
            {
                selectedFunctionTable.CreateNewPoint();
            }
        }
    }

    public void CopyToClipboard()
    {
        if (!SelectedFunctionTables.Any())
        {
            _functionsDataToClipboardSaver.CopyToClipboard(FunctionsTables.Select(x => x.Function));
        }
        else
        {
            _functionsDataToClipboardSaver.CopyToClipboard(SelectedFunctionTables.Select(x => x.Function));
        }
    }

    public void PasteFromClipboard()
    {
        var functions = _functionsDataToClipboardSaver.GetFunctionsFromClipboard();
        if (functions == null)
            return;

        foreach (var function in functions)
        {
            AddNewFunction(function);
        }

        OnFunctionPointsDataChanged(null, null!);
    }

    public void RemoveItems()
    {
        if (!SelectedFunctionTables.Any())
        {
            var items = FunctionsTables.ToArray();
            foreach (var functionTable in items)
            {
                functionTable.Dispose();
            }

            FunctionsTables.Clear();
        }
        else
        {
            var items = SelectedFunctionTables.ToArray();
            foreach (var functionTable in items)
            {
                FunctionsTables.Remove(functionTable);
                functionTable?.Dispose();
            }

            SelectedFunctionTables.Clear();
        }
    }

    public void OnFunctionTablesSelectionChanged(FunctionTableViewModel functionTable, bool isSelected)
    {
        if (isSelected)
        {
            SelectedFunctionTables.Add(functionTable);
        }
        else
        {
            SelectedFunctionTables.Remove(functionTable);
        }
    }

    public void UnselectFunctionTable(FunctionTableViewModel functionTable)
    {
        functionTable.IsSelected = false;
        SelectedFunctionTables.Remove(functionTable);
    }

    public void SaveToFile()
    {
        if (!SelectedFunctionTables.Any())
        {
            _functionsDataToJsonSaver.SaveToFile(FunctionsTables.Select(x => x.Function));
        }
        else
        {
            _functionsDataToJsonSaver.SaveToFile(SelectedFunctionTables.Select(x => x.Function));
        }
    }

    public void GetFromFile()
    {
        var functions = _functionsDataToJsonSaver.GetFunctionsDataFromFile();
        if (functions != null)
        {
            foreach (var function in functions)
            {
                AddNewFunction(function);
            }
        }

        OnFunctionPointsDataChanged(null, null!);
    }

    private void CreateNewFunction()
    {
        AddNewFunction(new FunctionViewModel());
    }

    private void AddNewFunction(FunctionViewModel function)
    {
        var newFunction = new FunctionTableViewModel(function);
        newFunction.FunctionDataChanged += OnFunctionPointsDataChanged;
        FunctionsTables.Add(newFunction);
        OnFunctionPointsDataChanged(null, null!);
    }

    private void OnFunctionPointsDataChanged(object? sender, FunctionViewModel e)
    {
        PlotControl.Plot.Clear();

        foreach (var functionTable in FunctionsTables)
        {
            var function = functionTable.Function;

            var xs = function.Points.Select(point => point.X).ToArray();
            var ys = function.Points.Select(point => point.Y).ToArray();

            AddScatterLine(xs, ys, function.Name, function.Id, function.IsInverted);
        }

        PlotControl.Refresh();
        HasChanges = true;
    }

    private void SetPlotLabels()
    {
        var plot = PlotControl.Plot;
        plot.XLabel("Абсолютная отметка [мм]");
        plot.YLabel("Температура [°C]");
    }

    private void AddScatterLine(double[] xs, double[] ys, string name, Guid id, bool isInverted = false)
    {
        var plot = PlotControl.Plot;

        if (!_functionColorsDictionary.TryGetValue(id, out var color))
        {
            color = ColorHelper.GetRandomColor();
            _functionColorsDictionary.Add(id, color);
        }

        var scatterLine = plot.Add.ScatterLine(xs, ys, color);
        scatterLine.MarkerShape = MarkerShape.FilledCircle;
        scatterLine.MarkerSize = 5;
        scatterLine.LegendText = name;

        if (isInverted)
        {
            var invertedScatterLine = plot.Add.ScatterLine(ys, xs, scatterLine.Color);
            invertedScatterLine.LineStyle = new LineStyle(1, scatterLine.Color, LinePattern.Dotted);
            invertedScatterLine.MarkerShape = MarkerShape.OpenCircle;
            invertedScatterLine.MarkerSize = 5;
            invertedScatterLine.LegendText = $"Обратная функция: {name}";
        }
    }
}
