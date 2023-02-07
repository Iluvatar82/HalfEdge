using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Validation
{
    public static class ValidationExtensions
    {
        public static void NotNull<T>(this T value, [CallerArgumentExpression("value")] string valueExpression = null)
        {
            if (value is null)
                ThrowNull(valueExpression);
        }

        public static void NotEmpty<T>(this IEnumerable<T> enumerable, [CallerArgumentExpression("enumerable")] string enumerableExpression = null)
        {
            if (!enumerable.Any())
                ThrowEmpty(enumerableExpression);
        }

        public static void NotNullOrEmpty<T>(this IEnumerable<T> enumerable, [CallerArgumentExpression("enumerable")] string enumerableExpression = null)
        {
            enumerable.NotNull(enumerableExpression);
            enumerable.NotEmpty(enumerableExpression);
        }

        [DoesNotReturn]
        private static void ThrowNull(string? paramName) => throw new ArgumentNullException(paramName);

        [DoesNotReturn]
        private static void ThrowEmpty(string? paramName) => throw new ArgumentOutOfRangeException(paramName);
    }
}