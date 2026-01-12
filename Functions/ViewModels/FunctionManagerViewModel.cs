using Caliburn.Micro;
using Functions.Helpers;
using Functions.Services;
using System.Windows;

namespace Functions.ViewModels;

public class FunctionManagerViewModel : PropertyChangedBase, IDisposable
{
    public FunctionViewModel Function { get; private set; } = new();
    public bool IsInvertedFunctionDisplayed { get; set; }
    public bool CanBeInverted { get; set; }
    public event EventHandler<FunctionViewModel>? FunctionDataChanged;

    private readonly IFunctionsDataToJsonSaver _functionsDataToJsonSaver;

    public FunctionManagerViewModel(IFunctionsDataToJsonSaver functionsDataToJsonSaver, FunctionViewModel? function = null)
    {
        _functionsDataToJsonSaver = functionsDataToJsonSaver;
        if (function != null)
        {
            Function = function;
        }

        Function.Points.CollectionChanged += (o, e) => TriggerFunctionPointsDataChangedEvent();
    }

    public void CreateNewPoint() => AddPoint(new());

    public void AddPoint(FunctionPointViewModel point)
    {
        point.PropertyChanged += (o, e) => TriggerFunctionPointsDataChangedEvent();
        Function.Points.Add(point);
    }

    public void RemovePoint(FunctionPointViewModel pointToRemove)
    {
        Function.Points.Remove(pointToRemove);
        pointToRemove.PropertyChanged -= (o, e) => TriggerFunctionPointsDataChangedEvent();
    }

    public void RemoveAllPoints()
    {
        foreach (var functionPoint in Function.Points)
        {
            functionPoint.PropertyChanged -= (o, e) => TriggerFunctionPointsDataChangedEvent();
        }

        Function.Points.Clear();
    }

    public void CopyToClipboard()
    {
        Clipboard.SetText(string.Join("\r\n", Function.Points.Select(point => $"{point.X}\t{point.Y}")));
    }

    public void PasteFromClipboard()
    {
        if (!Clipboard.ContainsText())
            return;

        foreach (var functionPoint in Function.Points)
        {
            RemovePoint(functionPoint);
        }

        var clipboardText = Clipboard.GetText();
        var pointTexts = clipboardText.Split("\r\n");
        var points = pointTexts
            .Select(pointText =>
            {
                var splittedValues = pointText.Split("\t");
                if (splittedValues.Length != 2)
                    return null;

                return double.TryParse(splittedValues[0], out var x)
                    && double.TryParse(splittedValues[1], out var y)
                    ? new FunctionPointViewModel { X = x, Y = y }
                    : null;
            })
            .Where(x => x != null);

        foreach (var point in points)
        {
            AddPoint(point!);
        }
    }

    public void GetFromFile()
    {
        var function = _functionsDataToJsonSaver.GetFunctionsDataFromFile()?.FirstOrDefault();
        if (function == null)
            return;

        Function.Points.Clear();

        foreach (var point in function.Points)
        {
            AddPoint(point);
        }
    }

    public void OnIsInvertedFunctionDisplayedChanged() => TriggerFunctionPointsDataChangedEvent();

    private void TriggerFunctionPointsDataChangedEvent()
    {
        CanBeInverted = Function.Points.IsFunctionStrictlyMonotonic();
        FunctionDataChanged?.Invoke(this, Function);
    }

    public void Dispose()
    {
        Function.Points.CollectionChanged -= (o, e) => TriggerFunctionPointsDataChangedEvent();
        RemoveAllPoints();
    }
}
