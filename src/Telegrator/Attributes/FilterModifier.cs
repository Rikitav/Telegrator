namespace Telegrator.Attributes
{
    /// <summary>
    /// Enumeration of filter modifiers that can be applied to update filters.
    /// Defines how filters should be combined and applied in filter chains.
    /// </summary>
    [Flags]
    public enum FilterModifier
    {
        /// <summary>
        /// No modifier applied. Filter is applied as-is.
        /// </summary>
        None = 1,
        
        /// <summary>
        /// OR modifier. This filter or the next filter in the chain should match.
        /// </summary>
        OrNext = 2,
        
        /// <summary>
        /// NOT modifier. The inverse of this filter should match.
        /// </summary>
        Not = 4,
    }
}
