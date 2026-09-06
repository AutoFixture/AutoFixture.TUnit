using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TUnit.Core.Helpers;

namespace AutoFixture.TUnit.Internal;

/// <summary>
/// The base class for test case sources.
/// </summary>
[SuppressMessage("Design", "CA1010:Generic interface should also be implemented",
    Justification = "The type is not a collection.")]
[SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix",
    Justification = "The type is not a collection.")]
public abstract class DataSource : BaseDataSourceAttribute
{
    /// <summary>
    /// Converts a TUnit-compatible data source result into async row factories.
    /// </summary>
    /// <param name="value">
    /// The raw data source result (for example a sequence, tuple, scalar, task, or async enumerable).
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel async enumeration of the data rows.
    /// </param>
    /// <returns>
    /// An asynchronous sequence of factories that each produce one <c>object?[]</c> test case row.
    /// </returns>
    protected static async IAsyncEnumerable<Func<Task<object?[]?>>> ToAsyncDataRows(
        object? value,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (value is Task task)
        {
            await task.ConfigureAwait(false);
            value = GetTaskResult(task);
        }

        if (value is null)
        {
            yield return static () => Task.FromResult<object?[]?>([null]);
            yield break;
        }

        if (TryGetAsyncElementType(value, out var elementType))
        {
            var convert = typeof(DataConversionHelper)
                .GetMethod(nameof(DataConversionHelper.ConvertAsyncEnumerableToObjectArrays))!
                .MakeGenericMethod(elementType);

            var rows = (IAsyncEnumerable<object[]>)convert
                .Invoke(null, [value, cancellationToken])!;

            await foreach (var row in rows.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                var captured = ExpandCollapsedRow(row);
                yield return () => Task.FromResult<object?[]?>(captured);
            }

            yield break;
        }

        if (IsRowSequence(value))
        {
            foreach (var row in DataConversionHelper.ConvertToObjectArrays(value))
            {
                var captured = ExpandCollapsedRow(row);
                yield return () => Task.FromResult<object?[]?>(captured);
            }

            yield break;
        }

        var singleRow = DataSourceHelpers.ToObjectArray(value);
        yield return () => Task.FromResult<object?[]?>(singleRow);
    }

    // TUnit Convert*ToObjectArrays can leave a ValueTuple or object[] as one cell.
    private static object?[] ExpandCollapsedRow(object?[] row)
    {
        if (row is not [var only])
        {
            return row;
        }

        if (DataSourceHelpers.IsTuple(only))
        {
            return DataSourceHelpers.ToObjectArray(only);
        }

        if (only is object[] alreadyRow)
        {
            return alreadyRow;
        }

        return row;
    }

    private static object? GetTaskResult(Task task)
    {
        var resultProperty = task.GetType().GetProperty("Result");
        return resultProperty?.GetValue(task);
    }

    private static bool IsRowSequence(object value)
    {
        if (value is string)
        {
            return false;
        }

        if (DataSourceHelpers.IsTuple(value))
        {
            return false;
        }

        return value is IEnumerable;
    }

    private static bool TryGetAsyncElementType(object value, out Type elementType)
    {
        var asyncEnumerable = (
            from type in value.GetType().GetInterfaces()
            where type.IsGenericType
               && type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)
            select type).FirstOrDefault();

        if (asyncEnumerable is null)
        {
            elementType = null!;
            return false;
        }

        elementType = asyncEnumerable.GetGenericArguments()[0];
        return true;
    }
}
