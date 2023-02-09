using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Validation
{
    public static class ValidationExtensions
    {
        public static void NotNull<T>(this T value, [CallerArgumentExpression("value")] string? valueExpression = null)
        {
            if (value is null)
                ThrowNull(valueExpression);
        }

        public static void NotEmpty<T>(this IEnumerable<T> enumerable, [CallerArgumentExpression("enumerable")] string? enumerableExpression = null)
        {
            if (!enumerable.Any())
                ThrowEmpty(enumerableExpression);
        }

        public static void NotNullOrEmpty<T>(this IEnumerable<T> enumerable, [CallerArgumentExpression("enumerable")] string? enumerableExpression = null)
        {
            enumerable.NotNull(enumerableExpression);
            enumerable.NotEmpty(enumerableExpression);
        }

        public static void HasElementCountEqualTo<T>(this IEnumerable<T> enumerable, int elementCount, [CallerArgumentExpression("enumerable")] string? enumerableExpression = null)
        {
            var actualCount = enumerable.Count();
            if (actualCount != elementCount)
                ThrowElementCount(enumerableExpression, actualCount, elementCount);
        }

        public static void HasElementCount<T>(this IEnumerable<T> enumerable, Func<int, bool> countCheck, [CallerArgumentExpression("enumerable")] string? enumerableExpression = null)
        {
            var actualCount = enumerable.Count();
            if (!countCheck(actualCount))
                ThrowElementCount(enumerableExpression, countCheck, actualCount);
        }

        public static void FormsLoop<T>(this IEnumerable<(T Start, T End)> enumerable, [CallerArgumentExpression("enumerable")] string? enumerableExpression = null)
        {
            enumerable.NotNullOrEmpty();

            var enumerator = enumerable.GetEnumerator();
            T? first = default;
            T? chained = default;
            while (enumerator.MoveNext())
            {
                first ??= enumerator.Current.Start;
                if (chained is null || chained.Equals(enumerator.Current.Start))
                    chained = enumerator.Current.End;
                else
                    ThrowNoLoop(enumerableExpression);
            }

            if (chained?.Equals(first) != true)
                ThrowNoLoop(enumerableExpression);
        }

        [DoesNotReturn]
        private static void ThrowNull(string? paramName) => throw new ArgumentNullException(paramName);

        [DoesNotReturn]
        private static void ThrowEmpty(string? paramName) => throw new ArgumentOutOfRangeException(paramName);

        [DoesNotReturn]
        private static void ThrowNoLoop(string? paramName) => throw new ArgumentOutOfRangeException(paramName);

        [DoesNotReturn]
        private static void ThrowElementCount(string? paramName, int actualCount, int expected)
            => throw new ArgumentOutOfRangeException(paramName, $"The number of Elements does not match. Found {actualCount} Elements, {expected} Elements expected.");

        [DoesNotReturn]
        private static void ThrowElementCount(string? paramName, Func<int, bool> countCheck, int actualCount)
            => throw new ArgumentOutOfRangeException(paramName, $"The number of Elements was not valid for the Check {countCheck}. Found {actualCount} Elements");
    }
}