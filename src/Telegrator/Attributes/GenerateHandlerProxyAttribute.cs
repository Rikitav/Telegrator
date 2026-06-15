namespace Telegrator.Attributes;

/// <summary>
/// Marks an extension method to be projected as a protected helper method
/// on generated Telegrator handler base classes.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class GenerateHandlerProxyAttribute(string? name = null) : Attribute
{
    /// <summary>
    /// Gets or sets the optional name to use for the generated proxy method.
    /// When <see langword="null"/>, the original method name is used.
    /// </summary>
    public string? Name { get; set; } = name;
}
