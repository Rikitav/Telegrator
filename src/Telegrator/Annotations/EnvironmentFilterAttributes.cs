using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters;
using Telegrator.Attributes;
using Telegrator.Filters.Components;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Abstract base attribute for filtering updates based on environment conditions.
    /// Can process all types of updates and provides environment-specific filtering logic.
    /// </summary>
    /// <param name="filters">The environment filters to apply</param>
    public abstract class EnvironmentFilterAttribute(params IFilter<Update>[] filters) : UpdateFilterAttribute<Update>(filters)
    {
        /// <summary>
        /// Gets the allowed update types that this filter can process.
        /// Environment filters can process all update types.
        /// </summary>
        public override UpdateType[] AllowedTypes => Update.AllTypes;

        /// <summary>
        /// Gets the update as the filtering target.
        /// Environment filters work with the entire update object.
        /// </summary>
        /// <param name="update">The Telegram update</param>
        /// <returns>The update object itself</returns>
        public override Update? GetFilterringTarget(Update update)
            => update;
    }

    /// <summary>
    /// Attribute for filtering updates that occur in debug environment.
    /// Only allows updates when the application is running in debug mode.
    /// </summary>
    public class IsDebugEnvironmentAttribute()
        : EnvironmentFilterAttribute(new IsDebugEnvironmentFilter())
    { }

    /// <summary>
    /// Attribute for filtering updates that occur in release environment.
    /// Only allows updates when the application is running in release mode.
    /// </summary>
    public class IsReleaseEnvironmentAttribute()
        : EnvironmentFilterAttribute(new IsReleaseEnvironmentFilter())
    { }

    /// <summary>
    /// Attribute for filtering updates based on environment variable values.
    /// </summary>
    public class EnvironmentVariableAttribute : EnvironmentFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter based on an environment variable with a specific value and comparison method.
        /// </summary>
        /// <param name="variable">The name of the environment variable</param>
        /// <param name="value">The expected value of the environment variable</param>
        /// <param name="comparison">The string comparison method</param>
        public EnvironmentVariableAttribute(string variable, string? value, StringComparison comparison)
            : base(new EnvironmentVariableFilter(variable, value, comparison)) { }

        /// <summary>
        /// Initializes the attribute to filter based on an environment variable with a specific value.
        /// </summary>
        /// <param name="variable">The name of the environment variable</param>
        /// <param name="value">The expected value of the environment variable</param>
        public EnvironmentVariableAttribute(string variable, string? value)
            : base(new EnvironmentVariableFilter(variable, value)) { }

        /// <summary>
        /// Initializes the attribute to filter based on the existence of an environment variable.
        /// </summary>
        /// <param name="variable">The name of the environment variable</param>
        public EnvironmentVariableAttribute(string variable)
            : base(new EnvironmentVariableFilter(variable)) { }

        /// <summary>
        /// Initializes the attribute to filter based on an environment variable with a specific comparison method.
        /// </summary>
        /// <param name="variable">The name of the environment variable</param>
        /// <param name="comparison">The string comparison method</param>
        public EnvironmentVariableAttribute(string variable, StringComparison comparison)
            : base(new EnvironmentVariableFilter(variable, comparison)) { }
    }
}
