using Functions.ViewModels;

namespace Functions.Helpers;

public static class FunctionHelper
{
    public static bool IsFunctionStrictlyMonotonic(this IEnumerable<FunctionPointViewModel> functionPoints)
    {
        if (functionPoints.DistinctBy(x => (x.X,x.Y)).Count() <= 1)
            return false;

        var xArr = functionPoints.Select(x => x.X).ToArray();
        var yArr = functionPoints.Select(x => x.Y).ToArray();

        foreach (var arr in new[] { xArr, yArr })
        {
            bool increasing = true;
            bool decreasing = true;

            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] > arr[i - 1])
                    decreasing = false;
                if (arr[i] < arr[i - 1])
                    increasing = false;

                if (!increasing && !decreasing)
                    return false;
            }
        }

        return true;
    }
}
