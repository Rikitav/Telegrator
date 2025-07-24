﻿using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes.Components;
using Telegrator.Filters.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.Providers;

namespace Telegrator.Handlers.Components
{
    /// <summary>
    /// Abstract base class for handlers that support branching execution based on different methods.
    /// Allows multiple handler methods to be defined in a single class, each with its own filters.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update being handled.</typeparam>
    public abstract class BranchingUpdateHandler<TUpdate> : AbstractUpdateHandler<TUpdate>, IHandlerContainerFactory, ICustomDescriptorsProvider where TUpdate : class
    {
        /// <summary>
        /// The method info for the current branch being executed.
        /// </summary>
        private MethodInfo? branchMethodInfo = null;

        /// <summary>
        /// Gets the binding flags used to discover branch methods.
        /// </summary>
        protected virtual BindingFlags BranchesBindingFlags => BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

        /// <summary>
        /// Gets the allowed return types for branch methods.
        /// </summary>
        protected virtual Type[] AllowedBranchReturnTypes => [typeof(void), typeof(Task)];

        /// <summary>
        /// Gets the cancellation token for the current execution.
        /// </summary>
        protected CancellationToken Cancellation { get; private set; } = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchingUpdateHandler{TUpdate}"/> class.
        /// </summary>
        /// <param name="handlingUpdateType">The type of update this handler processes.</param>
        protected BranchingUpdateHandler(UpdateType handlingUpdateType)
            : base(handlingUpdateType) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchingUpdateHandler{TUpdate}"/> class with a specific branch method.
        /// </summary>
        /// <param name="handlingUpdateType">The type of update this handler processes.</param>
        /// <param name="branch">The specific branch method to execute.</param>
        protected BranchingUpdateHandler(UpdateType handlingUpdateType, MethodInfo branch)
            : base(handlingUpdateType) => branchMethodInfo = branch;

        /// <summary>
        /// Describes all handler branches in this class.
        /// </summary>
        /// <returns>A collection of handler descriptors for each branch method.</returns>
        /// <exception cref="Exception">Thrown when no branch methods are found.</exception>
        public IEnumerable<HandlerDescriptor> DescribeHandlers()
        {
            Type thisType = GetType();
            UpdateHandlerAttributeBase updateHandlerAttribute = HandlerInspector.GetHandlerAttribute(thisType);
            IEnumerable<IFilter<Update>> handlerFilters = HandlerInspector.GetFilterAttributes(thisType, HandlingUpdateType);

            MethodInfo[] handlerBranches = thisType.GetMethods().Where(branch => branch.DeclaringType == thisType).ToArray();
            if (handlerBranches.Length == 0)
                throw new Exception();

            foreach (MethodInfo branch in handlerBranches)
                yield return DescribeBranch(branch, updateHandlerAttribute, handlerFilters);
        }

        /// <summary>
        /// Describes a specific branch method.
        /// </summary>
        /// <param name="branch">The branch method to describe.</param>
        /// <param name="handlerAttribute">The handler attribute for the class.</param>
        /// <param name="handlerFilters">The filters applied to the class.</param>
        /// <returns>A handler descriptor for the branch method.</returns>
        /// <exception cref="Exception">Thrown when the branch method has parameters or invalid return type.</exception>
        protected virtual HandlerDescriptor DescribeBranch(MethodInfo branch, UpdateHandlerAttributeBase handlerAttribute, IEnumerable<IFilter<Update>> handlerFilters)
        {
            Type thisType = GetType();

            if (branch.GetParameters().Length != 0)
                throw new Exception();

            if (!AllowedBranchReturnTypes.Any(branch.ReturnType.Equals))
                throw new Exception();

            List<IFilter<Update>> branchFiltersList = HandlerInspector.GetFilterAttributes(branch, HandlingUpdateType).ToList();
            branchFiltersList.AddRange(handlerFilters);

            DescriptorFiltersSet filtersSet = new DescriptorFiltersSet(
                handlerAttribute,
                HandlerInspector.GetStateKeeperAttribute(branch),
                branchFiltersList.ToArray());

            return new HandlerBranchDescriptor(branch, HandlingUpdateType, handlerAttribute.GetIndexer(), filtersSet);
        }

        /// <summary>
        /// Creates a handler container for this branching handler.
        /// </summary>
        /// <param name="awaitingProvider">The awaiting provider for the container.</param>
        /// <param name="handlerInfo">The handler information.</param>
        /// <returns>A handler container for this branching handler.</returns>
        /// <exception cref="Exception">Thrown when the awaiting provider is not of the expected type.</exception>
        public override IHandlerContainer CreateContainer(IAwaitingProvider awaitingProvider, DescribedHandlerInfo handlerInfo)
        {
            return new AbstractHandlerContainer<TUpdate>(awaitingProvider, handlerInfo);
        }

        /// <summary>
        /// Executes the current branch method.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <param name="cancellation">The cancellation token.</param>
        /// <exception cref="Exception">Thrown when no branch method is set.</exception>
        public override async Task Execute(IAbstractHandlerContainer<TUpdate> container, CancellationToken cancellation)
        {
            if (branchMethodInfo is null)
                throw new Exception();

            Cancellation = cancellation;
            await BranchExecuteWrapper(container, branchMethodInfo);
        }

        /// <summary>
        /// Wraps the execution of a branch method, handling both void and Task return types.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <param name="methodInfo">The method to execute.</param>
        protected virtual async Task BranchExecuteWrapper(IAbstractHandlerContainer<TUpdate> container, MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType == typeof(void))
            {
                methodInfo.Invoke(this, []);
                return;
            }
            else
            {
                object branchReturn = methodInfo.Invoke(this, []);
                if (branchReturn == null)
                    return;

                if (branchReturn is Task branchTask)
                    await branchTask;
            }
        }

        private class HandlerBranchDescriptor : HandlerDescriptor
        {
            public HandlerBranchDescriptor(MethodInfo method, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters)
                : base(DescriptorType.General, method.DeclaringType, updateType, indexer, filters)
            {
                DisplayString = string.Format("{0}+{1}", method.DeclaringType.Name, method.Name);
                InstanceFactory = () =>
                {
                    BranchingUpdateHandler<TUpdate> handler = (BranchingUpdateHandler<TUpdate>)Activator.CreateInstance(method.DeclaringType);
                    handler.branchMethodInfo = method;
                    return handler;
                };
            }
        }
    }
}
