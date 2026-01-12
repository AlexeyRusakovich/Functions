using Functions.ViewModels;

namespace Functions.Services
{
    public interface IFunctionsDataToJsonSaver
    {
        public bool SaveToFile(IEnumerable<FunctionPointViewModel> functionPoints);
        public IEnumerable<FunctionPointViewModel>? GetFunctionsDataFromFile();
    }
}
