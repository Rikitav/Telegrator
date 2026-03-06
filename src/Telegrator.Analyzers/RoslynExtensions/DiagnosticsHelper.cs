using Microsoft.CodeAnalysis;

namespace Telegrator.Analyzers.RoslynExtensions
{
    public static class DiagnosticsHelper
    {
        public static Diagnostic Create(this DiagnosticDescriptor descriptor, Location? location, params object[] messageArgs)
            => Diagnostic.Create(descriptor, location, messageArgs);

        public static void Report(this Diagnostic diagnostic, SourceProductionContext context)
            => context.ReportDiagnostic(diagnostic);

        public static void Report(this DiagnosticDescriptor descriptor, SourceProductionContext context, Location? location, params object[] messageArgs)
            => descriptor.Create(location, messageArgs).Report(context);
    }
}
