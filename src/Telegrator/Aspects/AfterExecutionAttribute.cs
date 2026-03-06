namespace Telegrator.Aspects
{
    /// <summary>
    /// Attribute that specifies a post-execution processor to be executed after the handler.
    /// The processor type must implement <see cref="IPostProcessor"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the post-processor that implements <see cref="IPostProcessor"/>.</typeparam>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AfterExecutionAttribute<T> : Attribute where T : IPostProcessor
    {
        /// <summary>
        /// Gets the type of the post-processor.
        /// </summary>
        public Type ProcessorType => typeof(T);
    }
}
