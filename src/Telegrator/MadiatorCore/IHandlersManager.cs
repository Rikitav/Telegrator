namespace Telegrator.MadiatorCore
{
    /// <summary>
    /// Combines <see cref="IHandlersCollection"/> and <see cref="IHandlersProvider"/>.
    /// Provides functionality of collecting, organizing and resolving handlers instances.
    /// </summary>
    public interface IHandlersManager : IHandlersCollection, IHandlersProvider
    {

    }
}
