using Caliburn.Micro;

namespace Functions.ViewModels;

public class FunctionsDashboardViewModel : Screen
{
    public FunctionPointsListViewModel FunctionPointsListViewModel { get; set; } = new();
}
