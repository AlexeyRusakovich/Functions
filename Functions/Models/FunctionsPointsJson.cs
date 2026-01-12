using Functions.ViewModels;

namespace Functions.Models;

public class FunctionsPointsJson
{
    public IEnumerable<FunctionPointViewModel> Data { get; set; } = [];
}
