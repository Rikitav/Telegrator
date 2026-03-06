namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute that prevents a class from being automatically collected by the handler collection system.
    /// When applied to a class, it will be excluded from domain-wide handler collection operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DontCollectAttribute : Attribute
    {

    }
}
