using ScottPlot;

namespace Functions.Helpers;

public static class ColorHelper
{
    private static readonly Random random = new();

    public static Color GetRandomColor()
    {
        int r = random.Next(50, 200);
        int g = random.Next(50, 200);
        int b = random.Next(50, 200);

        return Color.FromColor(System.Drawing.Color.FromArgb(r, g, b));
    }
}
