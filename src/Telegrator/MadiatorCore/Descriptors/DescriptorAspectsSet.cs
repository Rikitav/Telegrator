using Telegrator.Aspects;
using Telegrator.Handlers.Components;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// Manages the execution of pre and post-execution aspects for a handler.
    /// This class coordinates between self-processing (handler implements interfaces) 
    /// and typed processing (external processor classes).
    /// </summary>
    public sealed class DescriptorAspectsSet
    {
        /// <summary>
        /// Gets the type of the external pre-processor, if specified via <see cref="BeforeExecutionAttribute{T}"/>.
        /// </summary>
        public Type? TypedPre { get; private set; }

        /// <summary>
        /// Gets the type of the external post-processor, if specified via <see cref="AfterExecutionAttribute{T}"/>.
        /// </summary>
        public Type? TypedPost { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorAspectsSet"/> class.
        /// </summary>
        /// <param name="typedPre">The type of external pre-processor, if any.</param>
        /// <param name="typedPost">The type of external post-processor, if any.</param>
        public DescriptorAspectsSet(Type? typedPre, Type? typedPost)
        {
            TypedPre = typedPre;
            TypedPost = typedPost;
        }

        /// <summary>
        /// Executes the pre-execution aspect for the handler.
        /// </summary>
        /// <param name="handler">The handler instance.</param>
        /// <param name="container">The handler container with update context.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="Result"/> indicating whether execution should continue.</returns>
        /// <exception cref="InvalidOperationException">Thrown when handler claims to implement <see cref="IPreProcessor"/> but doesn't.</exception>
        public async Task<Result> ExecutePre(UpdateHandlerBase handler, IHandlerContainer container, CancellationToken cancellationToken)
        {
            if (handler is  IPreProcessor preProcessor)
            {
                return await preProcessor.BeforeExecution(container, cancellationToken).ConfigureAwait(false);
            }
            else if (TypedPre != null)
            {
                preProcessor = (IPreProcessor)Activator.CreateInstance(TypedPre);
                return await preProcessor.BeforeExecution(container, cancellationToken).ConfigureAwait(false);
            }

            return Result.Ok();
        }

        /// <summary>
        /// Executes the post-execution aspect for the handler.
        /// </summary>
        /// <param name="handler">The handler instance.</param>
        /// <param name="container">The handler container with update context.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="Result"/> indicating the final execution result.</returns>
        /// <exception cref="InvalidOperationException">Thrown when handler claims to implement <see cref="IPostProcessor"/> but doesn't.</exception>
        public async Task<Result> ExecutePost(UpdateHandlerBase handler, IHandlerContainer container, CancellationToken cancellationToken)
        {
            if (handler is IPostProcessor postProcessor)
            {
                return await postProcessor.AfterExecution(container, cancellationToken).ConfigureAwait(false);
            }
            else if (TypedPost != null)
            {
                postProcessor = (IPostProcessor)Activator.CreateInstance(TypedPost);
                return await postProcessor.AfterExecution(container, cancellationToken).ConfigureAwait(false);
            }

            return Result.Ok();
        }
    }
}
