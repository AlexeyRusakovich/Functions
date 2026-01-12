using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace Functions.ViewModels;

public class FunctionViewModel : PropertyChangedBase
{
    public Guid Id { get; } = Guid.NewGuid();

    public string Name { get; set; } = "Функция";

    public bool IsInverted { get; set; }

    public ObservableCollection<FunctionPointViewModel> Points { get; private set; } = [];
}
