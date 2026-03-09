using Telegram.Bot.Types;
using Telegrator.Filters;
using Telegrator.Logging;

namespace Telegrator.Core.Filters;

/// <summary>
/// Represents a filter that applies a filter action to an anonymous target type extracted from an update.
/// </summary>
public class AnonymousTypeFilter : Filter<Update>, INamedFilter
{
    private static readonly Type[] IgnoreLog = [typeof(CompiledFilter<>), typeof(AnonymousCompiledFilter), typeof(AnonymousTypeFilter)];

    private readonly Func<FilterExecutionContext<Update>, object, bool> FilterAction;
    private readonly Func<Update, object?> GetFilterringTarget;
    private readonly string _name;

    /// <summary>
    /// Gets the name of this filter.
    /// </summary>
    public virtual string Name => _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnonymousTypeFilter"/> class.
    /// </summary>
    /// <param name="name">The name of the filter.</param>
    /// <param name="filterAction">The filter action delegate.</param>
    /// <param name="getFilterringTarget">The function to get the filtering target from an update.</param>
    public AnonymousTypeFilter(string name, Func<Update, object?> getFilterringTarget, Func<FilterExecutionContext<Update>, object, bool> filterAction)
    {
        FilterAction = filterAction;
        GetFilterringTarget = getFilterringTarget;
        _name = name;
    }

    /// <summary>
    /// Compiles a filter for a specific target type.
    /// </summary>
    /// <typeparam name="T">The type of the filtering target.</typeparam>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="getFilterringTarget">The function to get the filtering target from an update.</param>
    /// <returns>The compiled filter.</returns>
    public static AnonymousTypeFilter Compile<T>(IFilter<T> filter, Func<Update, T?> getFilterringTarget) where T : class
    {
        return new AnonymousTypeFilter(
            filter.GetType().Name, getFilterringTarget,
            (context, filterringTarget) => CanPassInternal(context, filter, filterringTarget));
    }

    /// <summary>
    /// Compiles a filter for a specific target type with a custom name.
    /// </summary>
    /// <typeparam name="T">The type of the filtering target.</typeparam>
    /// <param name="name">The custom name for the compiled filter.</param>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="getFilterringTarget">The function to get the filtering target from an update.</param>
    /// <returns>The compiled filter.</returns>
    public static AnonymousTypeFilter Compile<T>(string name, IFilter<T> filter, Func<Update, T?> getFilterringTarget) where T : class
    {
        return new AnonymousTypeFilter(
            name,
            getFilterringTarget,
            (context, filterringTarget) => CanPassInternal(context, filter, filterringTarget));
    }

    /// <summary>
    /// Determines whether the filter can pass for the given context and filtering target.
    /// </summary>
    /// <typeparam name="T">The type of the filtering target.</typeparam>
    /// <param name="updateContext">The filter execution context.</param>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="filterringTarget">The filtering target.</param>
    /// <returns>True if the filter passes; otherwise, false.</returns>
    private static bool CanPassInternal<T>(FilterExecutionContext<Update> updateContext, IFilter<T> filter, object filterringTarget) where T : class
    {
        FilterExecutionContext<T> context = updateContext.CreateChild((T)filterringTarget);
        if (!filter.CanPass(context))
        {
            if (IgnoreLog.Contains(filter.GetType().MakeGenericType()))
                TelegratorLogging.LogDebug("{0} filter of {1} didnt pass!", filter.GetType().Name, context.Data["handler_name"]);

            return false;
        }

        context.CompletedFilters.Add(filter);
        return true;
    }

    /// <summary>
    /// Determines whether the filter can pass for the given context by extracting the filtering target and applying the filter action.
    /// </summary>
    /// <param name="context">The filter execution context.</param>
    /// <returns>True if the filter passes; otherwise, false.</returns>
    public override bool CanPass(FilterExecutionContext<Update> context)
    {
        try
        {
            object? filterringTarget = GetFilterringTarget.Invoke(context.Input);
            if (filterringTarget == null)
                return false;

            return FilterAction.Invoke(context, filterringTarget);
        }
        catch
        {
            return false;
        }
    }
}
