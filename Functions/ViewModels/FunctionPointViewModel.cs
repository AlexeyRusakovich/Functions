using Caliburn.Micro;
using System.Text.Json.Serialization;

namespace Functions.ViewModels;

public class FunctionPointViewModel : PropertyChangedBase
{
    public double X { get; set; }
    public double Y { get; set; }

    [JsonIgnore]
    public override bool IsNotifying { get; set; }
}
