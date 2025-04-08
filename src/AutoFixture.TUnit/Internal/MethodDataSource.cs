using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AutoFixture.TUnit.Internal
{
    /// <summary>
    /// Encapsulates access to a method that provides test data.
    /// </summary>
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented",
        Justification = "Type is not a collection.")]
    public class MethodDataSource : DataSource
    {
        private readonly object[] _arguments;

        /// <summary>
        /// Creates an instance of type <see cref="MethodDataSource" />.
        /// </summary>
        /// <param name="methodInfo">The source method.</param>
        /// <param name="arguments">The source method arguments.</param>
        public MethodDataSource(MethodInfo methodInfo, params object[] arguments)
        {
            this.MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            this._arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        /// <summary>
        /// Gets the source method info.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets the source method arguments.
        /// </summary>
        public IReadOnlyList<object> Arguments => Array.AsReadOnly(this._arguments);

        /// <inheritdoc/>
        public override IEnumerable<object[]> GetData(DataGeneratorMetadata dataGeneratorMetadata)
        {
            var value = this.MethodInfo.Invoke(null, this._arguments);
            if (value is not IEnumerable<object[]> enumerable)
            {
                throw new InvalidCastException("Member does not return an enumerable value.");
            }

            return enumerable;
        }
    }
}