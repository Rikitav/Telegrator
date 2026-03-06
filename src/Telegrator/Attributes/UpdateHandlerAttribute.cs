using Telegram.Bot.Types.Enums;
using Telegrator.Attributes.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.Attributes
{
    /// <summary>
    /// Abstract base attribute for marking update handler classes.
    /// Provides a type-safe way to associate handler types with specific update types and importance settings.
    /// </summary>
    /// <typeparam name="T">The type of the update handler that this attribute is applied to.</typeparam>
    public abstract class UpdateHandlerAttribute<T> : UpdateHandlerAttributeBase where T : UpdateHandlerBase
    {
        /// <summary>
        /// Initializes new instance of <see cref="UpdateHandlerAttribute{T}"/>
        /// </summary>
        /// <param name="updateType">The type of update that this handler can process.</param>
        protected UpdateHandlerAttribute(UpdateType updateType)
            : base([typeof(T)], updateType, 0) { }

        /// <summary>
        /// Initializes new instance of <see cref="UpdateHandlerAttribute{T}"/>
        /// </summary>
        /// <param name="updateType">The type of update that this handler can process.</param>
        /// <param name="importance">The importance level for this handler</param>
        protected UpdateHandlerAttribute(UpdateType updateType, int importance)
            : base([typeof(T)], updateType, importance) { }

        /// <summary>
        /// Initializes new instance of <see cref="UpdateHandlerAttribute{T}"/>
        /// </summary>
        /// <param name="types">Additional suported types.</param>
        /// <param name="updateType">The type of update that this handler can process.</param>
        protected UpdateHandlerAttribute(Type[] types, UpdateType updateType)
            : base([..types, typeof(T)], updateType, 0) { }

        /// <summary>
        /// Initializes new instance of <see cref="UpdateHandlerAttribute{T}"/>
        /// </summary>
        /// <param name="types">Additional suported types.</param>
        /// <param name="updateType">The type of update that this handler can process.</param>
        /// <param name="importance">The importance level for this handler</param>
        protected UpdateHandlerAttribute(Type[] types, UpdateType updateType, int importance)
            : base([.. types, typeof(T)], updateType, importance) { }
    }
}
