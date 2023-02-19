using Validation;

namespace Framework.Extensions
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            enumerable.NotNull();

            foreach (var item in enumerable)
                action(item);
        }

        public static bool IsCCW(this IList<(double X, double Y)> polygon)
        {
            int n = polygon.Count;
            double area = 0.0;
            for (int p = n - 1, q = 0; q < n; p = q++)
                area += polygon[p].X * polygon[q].Y - polygon[q].X * polygon[p].Y;

            return area > 0.0f;
        }

    }
}
