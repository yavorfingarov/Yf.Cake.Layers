namespace Yf.Cake.Layers
{
    internal static class ExtensionMethods
    {
        public static string ToPercent(this double value)
        {
            var rounded = (int)Math.Round(value);

            return $"{rounded}%";
        }

        public static string Shorten(this int value)
        {
            return value switch
            {
                < 1_000 => value.ToString(CultureInfo.InvariantCulture),
                < 10_000 => $"{(double)value / 1_000:0.0}k",
                < 1_000_000 => $"{(double)value / 1_000:0}k",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Got an unexpected value.")
            };
        }

        public static string MapToColor(
            this double value,
            double redThreshold,
            double yellowThreshold,
            double min = 0,
            double max = 100)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Got an unexpected value.");
            }
            else if (value > yellowThreshold)
            {
                return "brightgreen";
            }
            else if (value > redThreshold)
            {
                return "yellow";
            }
            else
            {
                return "red";
            }
        }
    }
}
