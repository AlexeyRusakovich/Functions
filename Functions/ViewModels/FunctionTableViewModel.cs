using Caliburn.Micro;
using Functions.Helpers;

namespace Functions.ViewModels;

public class FunctionTableViewModel : PropertyChangedBase, IDisposable
{
    public FunctionViewModel Function { get; private set; } = new();
    public bool IsInvertFunctionSelected { get; set; }
    public bool CanBeInverted { get; set; }
    public bool IsSelected { get; set; }

    public event EventHandler<FunctionViewModel>? FunctionDataChanged;

    public FunctionTableViewModel(FunctionViewModel? function = null)
    {
        if (function != null)
        {
            Function = function;
            foreach (var point in Function.Points)
            {
                point.PropertyChanged += (o, e) => TriggerFunctionPointsDataChangedEvent();
            }
        }

        Function.Points.CollectionChanged += (o, e) => TriggerFunctionPointsDataChangedEvent();
        Function.PropertyChanged += (o, e) => CheckIfFunctionNameChanged(e?.PropertyName);
        CanBeInverted = Function.Points.IsFunctionStrictlyMonotonic();
    }

    public void CreateNewPoint() => AddPoint(new());

    public void RemovePoint(FunctionPointViewModel pointToRemove)
    {
        Function.Points.Remove(pointToRemove);
        pointToRemove.PropertyChanged -= (o, e) => TriggerFunctionPointsDataChangedEvent();
    }

    public void OnIsInvertFunctionSelectedChanged() => TriggerFunctionPointsDataChangedEvent();

    public void Dispose()
    {
        Function.Points.CollectionChanged -= (o, e) => TriggerFunctionPointsDataChangedEvent();
        RemoveAllPoints();
    }

    private void AddPoint(FunctionPointViewModel point)
    {
        point.PropertyChanged += (o, e) => TriggerFunctionPointsDataChangedEvent();
        Function.Points.Add(point);
    }

    private void RemoveAllPoints()
    {
        foreach (var functionPoint in Function.Points)
        {
            functionPoint.PropertyChanged -= (o, e) => TriggerFunctionPointsDataChangedEvent();
        }

        Function.Points.Clear();
    }

    private void CheckIfFunctionNameChanged(string? propertyName)
    {
        if (propertyName == nameof(Function.Name))
        {
            TriggerFunctionPointsDataChangedEvent();
        }
    }

    private void TriggerFunctionPointsDataChangedEvent()
    {
        CanBeInverted = Function.Points.IsFunctionStrictlyMonotonic();
        Function.IsInverted = CanBeInverted && IsInvertFunctionSelected;
        FunctionDataChanged?.Invoke(this, Function);
    }
}
