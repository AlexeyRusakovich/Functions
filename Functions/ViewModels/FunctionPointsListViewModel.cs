using Caliburn.Micro;
using Functions.Helpers;
using Functions.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Functions.ViewModels;

public class FunctionPointsListViewModel : PropertyChangedBase
{
    public event EventHandler<(bool IsInverted, ObservableCollection<FunctionPointViewModel>)>? FunctionPointsDataChanged;
    public ObservableCollection<FunctionPointViewModel> FunctionPoints { get; set; } = [];
    public bool IsInvertedFunctionDisplayed { get; set; }
    public bool CanBeInverted { get; set; }

    private IFunctionsDataToJsonSaver _functionsDataToJsonFileSaver { get; }

    public FunctionPointsListViewModel(IFunctionsDataToJsonSaver functionsDataToJsonFileSaver)
    {
        _functionsDataToJsonFileSaver = functionsDataToJsonFileSaver;

        FunctionPoints.CollectionChanged += (o, e) => TriggerFunctionPointsDataChangedEvent();
    }

    public void CreateNewPoint() => AddPoint(new());

    public void AddPoint(FunctionPointViewModel point)
    {
        point.PropertyChanged += (o, e) => TriggerFunctionPointsDataChangedEvent();
        FunctionPoints.Add(point);
    }

    public void RemovePoint(FunctionPointViewModel pointToRemove)
    {
        FunctionPoints.Remove(pointToRemove);
        pointToRemove.PropertyChanged -= (o, e) => TriggerFunctionPointsDataChangedEvent();
    }

    public void RemoveAllPoints()
    {
        foreach (var functionPoint in FunctionPoints)
        {
            functionPoint.PropertyChanged -= (o, e) => TriggerFunctionPointsDataChangedEvent();
        }

        FunctionPoints.Clear();
    }

    public void CopyToClipboard()
    {
        Clipboard.SetText(string.Join("\r\n", FunctionPoints.Select(point => $"{point.X}\t{point.Y}")));
    }

    public void PasteFromClipboard()
    {
        if (!Clipboard.ContainsText())
            return;

        foreach (var functionPoint in FunctionPoints)
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

    public void SaveToFile()
    {
        _functionsDataToJsonFileSaver.SaveToFile(FunctionPoints);
    }

    public void GetFromFile()
    {
        var points = _functionsDataToJsonFileSaver.GetFunctionsDataFromFile();
        if (points == null)
            return;

        FunctionPoints.Clear();

        foreach (var point in points)
        {
            AddPoint(point);
        }
    }

    public void OnIsInvertedFunctionDisplayedChanged() => TriggerFunctionPointsDataChangedEvent();

    private void TriggerFunctionPointsDataChangedEvent()
    {
        CanBeInverted = FunctionPoints.IsFunctionStrictlyMonotonic();
        FunctionPointsDataChanged?.Invoke(this, (IsInvertedFunctionDisplayed && CanBeInverted, FunctionPoints));
    }
}
