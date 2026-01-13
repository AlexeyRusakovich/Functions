using Functions.ViewModels;

namespace Functions.Services;

public interface IFunctionsDataToClipboardSaver
{
    public void CopyToClipboard(IEnumerable<FunctionViewModel> functionsToSave);

    public IEnumerable<FunctionViewModel>? GetFunctionsFromClipboard();
}
