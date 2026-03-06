namespace Telegrator.Annotations.StateKeeping
{
    /// <summary>
    /// Represents special states for state keeping logic.
    /// </summary>
    public enum SpecialState
    {
        /// <summary>
        /// No special state.
        /// </summary>
        None,
        /// <summary>
        /// Indicates that no state is present.
        /// </summary>
        NoState,
        /// <summary>
        /// Indicates that any state is acceptable.
        /// </summary>
        AnyState
    }
}
