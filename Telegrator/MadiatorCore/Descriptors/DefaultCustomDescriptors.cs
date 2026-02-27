using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes.Components;
using Telegrator.Filters.Components;
using Telegrator.Handlers;
using Telegrator.Handlers.Building;
using Telegrator.Handlers.Components;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// Descriptor for creating handlers from methods
    /// </summary>
    /// <typeparam name="TUpdate"></typeparam>
    public class MethodHandlerDescriptor<TUpdate> : HandlerDescriptor where TUpdate : class
    {
        private readonly MethodInfo Method;

        /// <summary>
        /// Initializes new instance of <see cref="MethodHandlerDescriptor{TUpdate}"/>
        /// </summary>
        /// <param name="action"></param>
        public MethodHandlerDescriptor(AbstractHandlerAction<TUpdate> action) : base(DescriptorType.General, typeof(MethodHandler), true)
        {
            UpdateHandlerAttributeBase handlerAttribute = HandlerInspector.GetHandlerAttribute(action.Method);
            StateKeeperAttributeBase? stateKeeperAttribute = HandlerInspector.GetStateKeeperAttribute(action.Method);
            IFilter<Update>[] filters = HandlerInspector.GetFilterAttributes(action.Method, handlerAttribute.Type).ToArray();

            UpdateType = handlerAttribute.Type;
            Indexer = handlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(handlerAttribute, stateKeeperAttribute, filters);
            DisplayString = HandlerInspector.GetDisplayName(action.Method) ?? action.Method.Name;
            Method = action.Method;
            InstanceFactory = () => new MethodHandler(UpdateType);
            LazyInitialization = handler =>
            {
                if (handler is not MethodHandler methodHandler)
                    throw new InvalidDataException();

                methodHandler.Method = Method;
            };
        }

        private class MethodHandler(UpdateType updateType) : AbstractUpdateHandler<TUpdate>(updateType)
        {
            internal MethodInfo Method = null!;

            public override async Task<Result> Execute(IHandlerContainer<TUpdate> container, CancellationToken cancellation)
            {
                if (Method is null)
                    throw new Exception();

                if (Method.ReturnType == typeof(void))
                {
                    Method.Invoke(this, [container, cancellation]);
                    return Result.Ok();
                }
                else
                {
                    object branchReturn = Method.Invoke(this, [container, cancellation]);
                    if (branchReturn is not Task<Result> branchTask)
                        throw new InvalidOperationException();

                    return await branchTask;
                }
            }
        }
    }
}
