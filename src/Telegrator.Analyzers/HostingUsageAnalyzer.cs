using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;

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

    private static readonly DiagnosticDescriptor MissingUseTelegratorWebWarning = new(
        id: "TLG105",
        title: "Missing UseTelegratorWeb call",
        messageFormat: "AddTelegratorWeb was called but UseTelegratorWeb is missing. Ensure you call UseTelegratorWeb() on your built web host.",
        category: "Telegrator.Hosting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    private static readonly DiagnosticDescriptor MismatchedHostingMethodsWarning = new(
        id: "TLG106",
        title: "Mismatched Telegrator hosting methods",
        messageFormat: "Do not mix {0} and {1}. Use matching Add and Use methods (e.g. AddTelegrator with UseTelegrator, or AddTelegratorWeb with UseTelegratorWeb).",
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

    private static readonly DiagnosticDescriptor MissingAddTelegratorWebWarning = new(
        id: "TLG108",
        title: "Missing AddTelegratorWeb call",
        messageFormat: "UseTelegratorWeb was called but AddTelegratorWeb is missing. Ensure you call AddTelegratorWeb() when configuring services.",
        category: "Telegrator.Hosting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    private static readonly DiagnosticDescriptor MissingUseTelegratorWideWarning = new(
        id: "TLG109",
        title: "Missing UseWideTelegrator call",
        messageFormat: "AddWideTelegrator was called but UseWideTelegrator is missing. Ensure you call UseWideTelegrator() on your built wide host.",
        category: "Telegrator.Hosting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    private static readonly DiagnosticDescriptor MissingAddTelegratorWideWarning = new(
        id: "TLG110",
        title: "Missing AddWideTelegrator call",
        messageFormat: "UseWideTelegrator was called but AddWideTelegrator is missing. Ensure you call AddWideTelegrator() when configuring services.",
        category: "Telegrator.Hosting",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingUseTelegratorWarning, MissingUseTelegratorWebWarning, MissingUseTelegratorWideWarning,
                              MismatchedHostingMethodsWarning,
                              MissingAddTelegratorWarning, MissingAddTelegratorWebWarning, MissingAddTelegratorWideWarning);

    private enum CallKind
    {
        Add,
        AddWeb,
        AddWide,
        Use,
        UseWeb,
        UseWide
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
                    "AddTelegratorWeb" => CallKind.AddWeb,
                    "AddWideTelegrator" => CallKind.AddWide,
                    "UseTelegrator" => CallKind.Use,
                    "UseTelegratorWeb" => CallKind.UseWeb,
                    "UseWideTelegrator" => CallKind.UseWide,
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

                // 1. Глобальная проверка: убеждаемся, что для каждого типа Add существует парный тип Use в проекте
                bool globalAdd = allInvocations.Any(x => x.Kind == CallKind.Add);
                bool globalAddWeb = allInvocations.Any(x => x.Kind == CallKind.AddWeb);
                bool globalAddWide = allInvocations.Any(x => x.Kind == CallKind.AddWide);

                bool globalUse = allInvocations.Any(x => x.Kind == CallKind.Use);
                bool globalUseWeb = allInvocations.Any(x => x.Kind == CallKind.UseWeb);
                bool globalUseWide = allInvocations.Any(x => x.Kind == CallKind.UseWide);

                if (globalAdd && !globalUse)
                    ReportMissing(endContext, allInvocations, CallKind.Add, MissingUseTelegratorWarning);

                if (globalUse && !globalAdd)
                    ReportMissing(endContext, allInvocations, CallKind.Use, MissingAddTelegratorWarning);

                if (globalAddWeb && !globalUseWeb)
                    ReportMissing(endContext, allInvocations, CallKind.AddWeb, MissingUseTelegratorWebWarning);

                if (globalUseWeb && !globalAddWeb)
                    ReportMissing(endContext, allInvocations, CallKind.UseWeb, MissingAddTelegratorWebWarning);

                if (globalAddWide && !globalUseWide)
                    ReportMissing(endContext, allInvocations, CallKind.AddWide, MissingUseTelegratorWideWarning);

                if (globalUseWide && !globalAddWide)
                    ReportMissing(endContext, allInvocations, CallKind.UseWide, MissingAddTelegratorWideWarning);

                var methodGroups = allInvocations.GroupBy(x => x.Method, SymbolEqualityComparer.Default);

                foreach (var group in methodGroups)
                {
                    var localKinds = group.Select(x => x.Kind).ToImmutableHashSet();

                    bool hasAdd = localKinds.Contains(CallKind.Add);
                    bool hasAddWeb = localKinds.Contains(CallKind.AddWeb);
                    bool hasAddWide = localKinds.Contains(CallKind.AddWide);

                    bool hasUse = localKinds.Contains(CallKind.Use);
                    bool hasUseWeb = localKinds.Contains(CallKind.UseWeb);
                    bool hasUseWide = localKinds.Contains(CallKind.UseWide);

                    if (hasAdd && hasUseWeb)
                        ReportMismatch(endContext, group, CallKind.Add, CallKind.UseWeb, "AddTelegrator", "UseTelegratorWeb");
                    
                    if (hasAdd && hasUseWide)
                        ReportMismatch(endContext, group, CallKind.Add, CallKind.UseWide, "AddTelegrator", "UseWideTelegrator");

                    if (hasAddWeb && hasUse)
                        ReportMismatch(endContext, group, CallKind.AddWeb, CallKind.Use, "AddTelegratorWeb", "UseTelegrator");
                    
                    if (hasAddWeb && hasUseWide)
                        ReportMismatch(endContext, group, CallKind.AddWeb, CallKind.UseWide, "AddTelegratorWeb", "UseWideTelegrator");

                    if (hasAddWide && hasUse)
                        ReportMismatch(endContext, group, CallKind.AddWide, CallKind.Use, "AddWideTelegrator", "UseTelegrator");
                    
                    if (hasAddWide && hasUseWeb)
                        ReportMismatch(endContext, group, CallKind.AddWide, CallKind.UseWeb, "AddWideTelegrator", "UseTelegratorWeb");
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
            context.ReportDiagnostic(Diagnostic.Create(MismatchedHostingMethodsWarning, item.Loc, name1, name2));
        }
    }
}