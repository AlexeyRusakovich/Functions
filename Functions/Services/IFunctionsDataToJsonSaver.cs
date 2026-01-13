using Functions.ViewModels;

namespace Functions.Services
{
    public interface IFunctionsDataToJsonSaver
    {
        public bool SaveToFile(IEnumerable<FunctionViewModel> function);
        public IEnumerable<FunctionViewModel>? GetFunctionsDataFromFile();
    }
}
