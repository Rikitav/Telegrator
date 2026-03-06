using Telegram.Bot.Types;

namespace Telegrator.StateKeeping.Components
{
    /// <summary>
    /// Base class for managing state associated with updates and keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used for state resolution.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    public abstract class StateKeeperBase<TKey, TState> where TState : notnull where TKey : notnull
    {
        private readonly Dictionary<TKey, TState> States = [];

        /// <summary>
        /// Gets or sets the key resolver used to resolve keys from updates.
        /// </summary>
        public IStateKeyResolver<TKey> KeyResolver { get; set; } = null!;

        /// <summary>
        /// Gets the default state value.
        /// </summary>
        public abstract TState DefaultState { get; }

        /// <summary>
        /// Sets the state for the specified update.
        /// </summary>
        /// <param name="keySource">The update to use as a key source.</param>
        /// <param name="newState">The new state value.</param>
        public virtual void SetState(Update keySource, TState newState)
        {
            TKey key = KeyResolver.ResolveKey(keySource);
            States.Set(key, newState, DefaultState);
        }

        /// <summary>
        /// Gets the state for the specified update.
        /// </summary>
        /// <param name="keySource">The update to use as a key source.</param>
        /// <returns>The state value.</returns>
        public virtual TState GetState(Update keySource)
        {
            TKey key = KeyResolver.ResolveKey(keySource);
            return States[key];
        }

        /// <summary>
        /// Tries to get the state for the specified update.
        /// </summary>
        /// <param name="keySource">The update to use as a key source.</param>
        /// <param name="state">When this method returns, contains the state value if found; otherwise, the default value.</param>
        /// <returns>True if the state was found; otherwise, false.</returns>
        public virtual bool TryGetState(Update keySource, out TState? state)
        {
            TKey key = KeyResolver.ResolveKey(keySource);
            return States.TryGetValue(key, out state);
        }

        /// <summary>
        /// Determines whether a state exists for the specified update.
        /// </summary>
        /// <param name="keySource">The update to use as a key source.</param>
        /// <returns>True if the state exists; otherwise, false.</returns>
        public virtual bool HasState(Update keySource)
        {
            TKey key = KeyResolver.ResolveKey(keySource);
            return States.ContainsKey(key);
        }

        /// <summary>
        /// Creates a state for the specified update using the default state value.
        /// </summary>
        /// <param name="keySource">The update to use as a key source.</param>
        public virtual void CreateState(Update keySource)
        {
            TKey key = KeyResolver.ResolveKey(keySource);
            States.Set(key, DefaultState);
        }

        /// <summary>
        /// Deletes the state for the specified update.
        /// </summary>
        /// <param name="keySource">The update to use as a key source.</param>
        public virtual void DeleteState(Update keySource)
        {
            TKey key = KeyResolver.ResolveKey(keySource);
            States.Remove(key);
        }

        /// <summary>
        /// Moves the state forward for the specified update.
        /// </summary>
        /// <param name="keySource">The update to use as a key source.</param>
        public virtual void MoveForward(Update keySource)
        {
            TKey key = KeyResolver.ResolveKey(keySource);
            if (!States.TryGetValue(key, out TState currentState))
            {
                States.Set(key, DefaultState);
                currentState = DefaultState;
            }

            TState newState = MoveForward(currentState, key);
            States[key] = newState;
        }

        /// <summary>
        /// Moves the state backward for the specified update.
        /// </summary>
        /// <param name="keySource">The update to use as a key source.</param>
        public virtual void MoveBackward(Update keySource)
        {
            TKey key = KeyResolver.ResolveKey(keySource);
            if (!States.TryGetValue(key, out TState currentState))
            {
                States.Set(key, DefaultState);
                return;
            }

            TState newState = MoveBackward(currentState, key);
            States[key] = newState;
        }

        /*
        /// <summary>
        /// Gets the state keeper for the specified key.
        /// </summary>
        /// <typeparam name="TStateKeeper">The type of the state keeper.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The state keeper instance.</returns>
        protected virtual TStateKeeper GetKeeper<TStateKeeper>(TKey key) where TStateKeeper : StateKeeperBase<TKey, TState>
            => States[key] as TStateKeeper ?? throw new InvalidCastException();
        */

        /// <summary>
        /// Moves the state forward for the specified current state and key.
        /// </summary>
        /// <param name="currentState">The current state value.</param>
        /// <param name="currentKey">The key.</param>
        /// <returns>The new state value.</returns>
        protected abstract TState MoveForward(TState currentState, TKey currentKey);

        /// <summary>
        /// Moves the state backward for the specified current state and key.
        /// </summary>
        /// <param name="currentState">The current state value.</param>
        /// <param name="currentKey">The key.</param>
        /// <returns>The new state value.</returns>
        protected abstract TState MoveBackward(TState currentState, TKey currentKey);
    }
}
