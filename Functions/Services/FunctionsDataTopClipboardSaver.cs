using Functions.ViewModels;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Functions.Services;

public class FunctionsDataTopClipboardSaver : IFunctionsDataToClipboardSaver
{
    private const string _xyPairGroupName = "xy";
    private static Regex _doubleValueRegex = new(@"(-?\d+(?:\.\d+)?)");
    private static Regex _xyPairRegex = new($"({_doubleValueRegex}\t{_doubleValueRegex})");
    private static Regex _xyPairSplitterRegex = new(@"(\t\t)");
    private static Regex _xyPairOrEmptyRegex = new($"(?<{_xyPairGroupName}>({_xyPairRegex}|\t))");
    private static Regex _multipleXYPairsRegex = new($"(({_xyPairOrEmptyRegex}{_xyPairSplitterRegex})+{_xyPairOrEmptyRegex})");
    private static Regex functionsStringRegex = new ($"^({_xyPairOrEmptyRegex}|{_multipleXYPairsRegex})$");

    public void CopyToClipboard(IEnumerable<FunctionViewModel> functionsToSave)
    {
        var linesCount = functionsToSave.Max(x => x.Points.Count);
        StringBuilder sb = new();
        for (int i = 0; i < linesCount; i++)
        {
            int functionIndex = 0;
            foreach (var function in functionsToSave)
            {
                if (functionIndex != 0)
                {
                    sb.Append("\t\t");
                }

                if (function.Points.Count > i)
                {
                    var point = function.Points[i];
                    sb.Append($"{point.X}\t{point.Y}");
                }
                else
                {
                    sb.Append('\t');
                }

                functionIndex++;
            }

            sb.Append("\r\n");
        }

        Clipboard.SetText(sb.ToString());
    }

    public IEnumerable<FunctionViewModel>? GetFunctionsFromClipboard()
    {
        if (!Clipboard.ContainsText())
            return null;

        var clipboardText = Clipboard.GetText();
        var lines = clipboardText.Split("\r\n")
                                 .Where(x => !string.IsNullOrWhiteSpace(x))
                                 .ToArray();

        if (lines.Length == 0 || lines.Any(line => !functionsStringRegex.IsMatch(line)))
            return null;

        List<FunctionViewModel>? functions = null;

        foreach (var line in lines)
        {
            var matches = functionsStringRegex.Matches(line);
            var captures = matches[0].Groups[$"{_xyPairGroupName}"].Captures;

            functions ??= [.. Enumerable
                    .Range(0, captures.Count)
                    .Select(x => 
                    {
                        var function = new FunctionViewModel();
                        function.Points.Clear();
                        return function;
                    })];

            var index = 0;
            foreach (var _ in captures)
            {
                var splittedValues = captures[index].Value.Split('\t');
                if (splittedValues.Length < 2 ||
                    splittedValues.Any(x => string.IsNullOrEmpty(x)))
                {
                    index++;
                    continue;
                }

                var pointViewModel = new FunctionPointViewModel 
                {
                    X = double.Parse(splittedValues[0]),
                    Y = double.Parse(splittedValues[1]),
                };

                functions[index].Points.Add(pointViewModel);
                index++;
            }
        }

        return functions;
    }
}
