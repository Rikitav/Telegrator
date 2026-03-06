using Telegrator.Attributes.Components;

namespace Telegrator.Handlers.Diagnostics
{
    /// <summary>
    /// Provides extension methods for <see cref="ReportInspector"/>
    /// </summary>
    public static partial class ReportInspectorExtensions
    {
        /// <inheritdoc cref="ReportInspector.Whenever(string)"/>
        public static ReportInspector Whenever<TAttribute>(this ReportInspector inspector) where TAttribute : UpdateFilterAttributeBase
            => inspector.Whenever(nameof(TAttribute));

        /// <inheritdoc cref="ReportInspector.Except(string)"/>
        public static ReportInspector Except<TAttribute>(this ReportInspector inspector) where TAttribute : UpdateFilterAttributeBase
            => inspector.Except(nameof(TAttribute));
    }
}
