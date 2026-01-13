using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Functions.ViewModels;

public class FunctionViewModel : PropertyChangedBase
{
    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();

    public string Name { get; set; } = "Функция";

    public bool IsInverted { get; set; }

    public ObservableCollection<FunctionPointViewModel> Points { get; set; } = [ new() ];
}
