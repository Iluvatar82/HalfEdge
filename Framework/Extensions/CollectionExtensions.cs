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
    }
}
