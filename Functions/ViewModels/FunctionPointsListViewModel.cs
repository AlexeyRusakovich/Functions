using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Windows;

namespace Functions.ViewModels;

public class FunctionPointsListViewModel : PropertyChangedBase
{
    public ObservableCollection<FunctionPointViewModel> FunctionPoints { get; set; } = [];
    public event EventHandler<ObservableCollection<FunctionPointViewModel>> FunctionPointsDataChanged;

    public FunctionPointsListViewModel()
    {
        FunctionPoints.CollectionChanged += (o, e) => TriggreFunctionPointsDataChangedEvent();
    }

    public void CreateNewPoint() => AddPoint(new());

    public void AddPoint(FunctionPointViewModel point)
    {
        point.PropertyChanged += (o, e) => TriggreFunctionPointsDataChangedEvent();
        FunctionPoints.Add(point);
    }

    public void RemovePoint(FunctionPointViewModel pointToRemove)
    {
        FunctionPoints.Remove(pointToRemove);
        pointToRemove.PropertyChanged -= (o, e) => TriggreFunctionPointsDataChangedEvent();
    }

    public void RemoveAllPoints()
    {
        foreach (var functionPoint in FunctionPoints)
        {
            functionPoint.PropertyChanged -= (o, e) => TriggreFunctionPointsDataChangedEvent();
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

    private void TriggreFunctionPointsDataChangedEvent()
    {
        FunctionPointsDataChanged?.Invoke(this, FunctionPoints);
    }
}
