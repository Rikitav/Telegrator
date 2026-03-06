namespace Telegrator.Aspects
{
    /// <summary>
    /// Attribute that specifies a pre-execution processor to be executed before the handler.
    /// The processor type must implement <see cref="IPreProcessor"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the pre-processor that implements <see cref="IPreProcessor"/>.</typeparam>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BeforeExecutionAttribute<T> : Attribute where T : IPreProcessor
    {
        /// <summary>
        /// Gets the type of the pre-processor.
        /// </summary>
        public Type ProcessorType => typeof(T);
    }
}
