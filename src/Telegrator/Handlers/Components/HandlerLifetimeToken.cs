namespace Telegrator.Handlers.Components
{
    /// <summary>
    /// Represents a token that tracks the lifetime of a handler instance.
    /// </summary>
    public class HandlerLifetimeToken
    {
        /// <summary>
        /// Event triggered when the handler's lifetime has ended.
        /// </summary>
        public event Action<HandlerLifetimeToken>? OnLifetimeEnded;

        /// <summary>
        /// Gets a value indicating whether the handler's lifetime has ended.
        /// </summary>
        public bool IsEnded { get; private set; }

        /// <summary>
        /// Marks the handler's lifetime as ended and triggers the event.
        /// </summary>
        public void LifetimeEnded()
        {
            IsEnded = true;
            OnLifetimeEnded?.Invoke(this);
        }
    }
}
