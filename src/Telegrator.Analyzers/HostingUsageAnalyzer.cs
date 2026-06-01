using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Telegrator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HostingUsageAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor MissingUseTelegratorWarning = new(
        id: "TLG104",
        title: "Missing UseTelegrator call",
        messageFormat: "AddTelegrator was called but UseTelegrator is missing. Ensure you call UseTelegrator() on your built host.",
        category: "Telegrator.Hosting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    private static readonly DiagnosticDescriptor MissingReceivingModeWarning = new(
        id: "TLG105",
        title: "Missing receiving mode configuration",
        messageFormat: "AddTelegrator was called but no receiving mode was configured. Ensure you call WithPolling(), WithWeb(), or WithWide().",
        category: "Telegrator.Hosting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    private static readonly DiagnosticDescriptor MismatchedReceivingModesWarning = new(
        id: "TLG106",
        title: "Mismatched Telegrator receiving modes",
        messageFormat: "Do not mix {0} and {1}. Choose only one receiving mode for the bot.",
        category: "Telegrator.Hosting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    private static readonly DiagnosticDescriptor MissingAddTelegratorWarning = new(
        id: "TLG107",
        title: "Missing AddTelegrator call",
        messageFormat: "UseTelegrator was called but AddTelegrator is missing. Ensure you call AddTelegrator() when configuring services.",
        category: "Telegrator.Hosting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingUseTelegratorWarning, MissingReceivingModeWarning, MismatchedReceivingModesWarning, MissingAddTelegratorWarning);

    private enum CallKind
    {
        Add,
        Use,
        WithPolling,
        WithWeb,
        WithWide
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            var invocations = new ConcurrentBag<(ISymbol Method, Location Loc, CallKind Kind)>();

            compilationContext.RegisterOperationAction(operationContext =>
            {
                var invocation = (IInvocationOperation)operationContext.Operation;
                var methodName = invocation.TargetMethod.Name;
                var methodSymbol = operationContext.ContainingSymbol;

                CallKind? kind = methodName switch
                {
                    "AddTelegrator" => CallKind.Add,
                    "UseTelegrator" => CallKind.Use,
                    "WithPolling" => CallKind.WithPolling,
                    "WithWeb" => CallKind.WithWeb,
                    "WithWide" => CallKind.WithWide,
                    _ => null
                };

                if (kind.HasValue)
                {
                    invocations.Add((methodSymbol, invocation.Syntax.GetLocation(), kind.Value));
                }
            }, OperationKind.Invocation);

            compilationContext.RegisterCompilationEndAction(endContext =>
            {
                var allInvocations = invocations.ToList();
                if (allInvocations.Count == 0)
                    return;

                bool globalAdd = allInvocations.Any(x => x.Kind == CallKind.Add);
                bool globalUse = allInvocations.Any(x => x.Kind == CallKind.Use);
                bool globalWithPolling = allInvocations.Any(x => x.Kind == CallKind.WithPolling);
                bool globalWithWeb = allInvocations.Any(x => x.Kind == CallKind.WithWeb);
                bool globalWithWide = allInvocations.Any(x => x.Kind == CallKind.WithWide);

                if (globalAdd && !globalUse)
                    ReportMissing(endContext, allInvocations, CallKind.Add, MissingUseTelegratorWarning);

                if (globalUse && !globalAdd)
                    ReportMissing(endContext, allInvocations, CallKind.Use, MissingAddTelegratorWarning);

                if (globalAdd && !globalWithPolling && !globalWithWeb && !globalWithWide)
                    ReportMissing(endContext, allInvocations, CallKind.Add, MissingReceivingModeWarning);

                var methodGroups = allInvocations.GroupBy(x => x.Method, SymbolEqualityComparer.Default);

                foreach (var group in methodGroups)
                {
                    var localKinds = group.Select(x => x.Kind).ToImmutableHashSet();

                    bool hasWithPolling = localKinds.Contains(CallKind.WithPolling);
                    bool hasWithWeb = localKinds.Contains(CallKind.WithWeb);
                    bool hasWithWide = localKinds.Contains(CallKind.WithWide);

                    if (hasWithPolling && hasWithWeb)
                        ReportMismatch(endContext, group, CallKind.WithPolling, CallKind.WithWeb, "WithPolling", "WithWeb");

                    if (hasWithPolling && hasWithWide)
                        ReportMismatch(endContext, group, CallKind.WithPolling, CallKind.WithWide, "WithPolling", "WithWide");

                    if (hasWithWeb && hasWithWide)
                        ReportMismatch(endContext, group, CallKind.WithWeb, CallKind.WithWide, "WithWeb", "WithWide");
                }
            });
        });
    }

    private static void ReportMissing(CompilationAnalysisContext context, System.Collections.Generic.IEnumerable<(ISymbol Method, Location Loc, CallKind Kind)> invocations, CallKind targetKind, DiagnosticDescriptor descriptor)
    {
        foreach (var item in invocations.Where(x => x.Kind == targetKind))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, item.Loc));
        }
    }

    private static void ReportMismatch(CompilationAnalysisContext context, IGrouping<ISymbol?, (ISymbol Method, Location Loc, CallKind Kind)> group, CallKind kind1, CallKind kind2, string name1, string name2)
    {
        foreach (var item in group.Where(x => x.Kind == kind1 || x.Kind == kind2))
        {
            context.ReportDiagnostic(Diagnostic.Create(MismatchedReceivingModesWarning, item.Loc, name1, name2));
        }
    }
}
