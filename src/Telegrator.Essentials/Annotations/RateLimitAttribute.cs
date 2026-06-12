using Telegrator.Aspects;
using Telegrator.Essentials.Aspects;

namespace Telegrator.Annotations;

/// <summary>
/// Convenience attribute that applies the default <see cref="RateLimitPreprocessor"/> to a handler.
/// The default policy allows 10 requests per 60-second sliding window per user.
/// </summary>
/// <remarks>
/// For custom limits, create a derived preprocessor from <see cref="RateLimitPreprocessor"/>
/// and apply it with <see cref="BeforeExecutionAttribute{T}"/>.
/// </remarks>
public class RateLimitAttribute : BeforeExecutionAttribute<RateLimitPreprocessor>
{
}
