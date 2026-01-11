using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace Functions.ViewModels;

public class FunctionPointsListViewModel : Screen
{
    public ObservableCollection<FunctionPointViewModel> FunctionPoints { get; set; } = [];

    public void CreateNewPoint()
    {
        FunctionPoints.Add(new FunctionPointViewModel());
    }
}
