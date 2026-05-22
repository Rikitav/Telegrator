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

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            var addTelegratorInvocations = new ConcurrentBag<Location>();
            var addTelegratorWebInvocations = new ConcurrentBag<Location>();
            var addTelegratorWideInvocations = new ConcurrentBag<Location>();
            var useTelegratorInvocations = new ConcurrentBag<Location>();
            var useTelegratorWebInvocations = new ConcurrentBag<Location>();
            var useTelegratorWideInvocations = new ConcurrentBag<Location>();

            compilationContext.RegisterOperationAction(operationContext =>
            {
                var invocation = (IInvocationOperation)operationContext.Operation;
                var methodName = invocation.TargetMethod.Name;

                if (methodName == "AddTelegrator")
                {
                    addTelegratorInvocations.Add(invocation.Syntax.GetLocation());
                }
                else if (methodName == "AddTelegratorWeb")
                {
                    addTelegratorWebInvocations.Add(invocation.Syntax.GetLocation());
                }
                else if (methodName == "AddWideTelegrator")
                {
                    addTelegratorWideInvocations.Add(invocation.Syntax.GetLocation());
                }
                else if (methodName == "UseTelegrator")
                {
                    useTelegratorInvocations.Add(invocation.Syntax.GetLocation());
                }
                else if (methodName == "UseTelegratorWeb")
                {
                    useTelegratorWebInvocations.Add(invocation.Syntax.GetLocation());
                }
                else if (methodName == "UseWideTelegrator")
                {
                    useTelegratorWideInvocations.Add(invocation.Syntax.GetLocation());
                }
            }, OperationKind.Invocation);

            compilationContext.RegisterCompilationEndAction(endContext =>
            {
                bool hasAdd = !addTelegratorInvocations.IsEmpty;
                bool hasAddWeb = !addTelegratorWebInvocations.IsEmpty;
                bool hasAddWide = !addTelegratorWideInvocations.IsEmpty;

                bool hasUse = !useTelegratorInvocations.IsEmpty;
                bool hasUseWeb = !useTelegratorWebInvocations.IsEmpty;
                bool hasUseWide = !useTelegratorWideInvocations.IsEmpty;

                // Check mismatches
                if (hasAdd && (hasUseWeb || hasUseWide))
                {
                    foreach (var loc in addTelegratorInvocations)
                        endContext.ReportDiagnostic(Diagnostic.Create(MismatchedHostingMethodsWarning, loc, "AddTelegrator", hasUseWeb ? "UseTelegratorWeb" : "UseWideTelegrator"));

                    if (hasUseWeb)
                        foreach (var loc in useTelegratorWebInvocations)
                            endContext.ReportDiagnostic(Diagnostic.Create(MismatchedHostingMethodsWarning, loc, "AddTelegrator", "UseTelegratorWeb"));

                    if (hasUseWide)
                        foreach (var loc in useTelegratorWideInvocations)
                            endContext.ReportDiagnostic(Diagnostic.Create(MismatchedHostingMethodsWarning, loc, "AddTelegrator", "UseWideTelegrator"));
                }

                if (hasAddWeb && (hasUse || hasUseWide))
                {
                    foreach (var loc in addTelegratorWebInvocations)
                        endContext.ReportDiagnostic(Diagnostic.Create(MismatchedHostingMethodsWarning, loc, "AddTelegratorWeb", hasUse ? "UseTelegrator" : "UseWideTelegrator"));

                    if (hasUse)
                        foreach (var loc in useTelegratorInvocations)
                            endContext.ReportDiagnostic(Diagnostic.Create(MismatchedHostingMethodsWarning, loc, "AddTelegratorWeb", "UseTelegrator"));

                    if (hasUseWide)
                        foreach (var loc in useTelegratorWideInvocations)
                            endContext.ReportDiagnostic(Diagnostic.Create(MismatchedHostingMethodsWarning, loc, "AddTelegratorWeb", "UseWideTelegrator"));
                }

                if (hasAddWide && (hasUse || hasUseWeb))
                {
                    foreach (var loc in addTelegratorWideInvocations)
                        endContext.ReportDiagnostic(Diagnostic.Create(MismatchedHostingMethodsWarning, loc, "AddWideTelegrator", hasUse ? "UseTelegrator" : "UseTelegratorWeb"));

                    if (hasUse)
                        foreach (var loc in useTelegratorInvocations)
                            endContext.ReportDiagnostic(Diagnostic.Create(MismatchedHostingMethodsWarning, loc, "AddWideTelegrator", "UseTelegrator"));

                    if (hasUseWeb)
                        foreach (var loc in useTelegratorWebInvocations)
                            endContext.ReportDiagnostic(Diagnostic.Create(MismatchedHostingMethodsWarning, loc, "AddWideTelegrator", "UseTelegratorWeb"));
                }

                // Check missing 'Use'
                if (hasAdd && !hasUse && !hasUseWeb && !hasUseWide)
                {
                    foreach (var loc in addTelegratorInvocations)
                        endContext.ReportDiagnostic(Diagnostic.Create(MissingUseTelegratorWarning, loc));
                }

                if (hasAddWeb && !hasUseWeb && !hasUse && !hasUseWide)
                {
                    foreach (var loc in addTelegratorWebInvocations)
                        endContext.ReportDiagnostic(Diagnostic.Create(MissingUseTelegratorWebWarning, loc));
                }

                if (hasAddWide && !hasUseWide && !hasUse && !hasUseWeb)
                {
                    foreach (var loc in addTelegratorWideInvocations)
                        endContext.ReportDiagnostic(Diagnostic.Create(MissingUseTelegratorWideWarning, loc));
                }

                // Check missing 'Add'
                if (hasUse && !hasAdd && !hasAddWeb && !hasAddWide)
                {
                    foreach (var loc in useTelegratorInvocations)
                        endContext.ReportDiagnostic(Diagnostic.Create(MissingAddTelegratorWarning, loc));
                }

                if (hasUseWeb && !hasAddWeb && !hasAdd && !hasAddWide)
                {
                    foreach (var loc in useTelegratorWebInvocations)
                        endContext.ReportDiagnostic(Diagnostic.Create(MissingAddTelegratorWebWarning, loc));
                }

                if (hasUseWide && !hasAddWide && !hasAdd && !hasAddWeb)
                {
                    foreach (var loc in useTelegratorWideInvocations)
                        endContext.ReportDiagnostic(Diagnostic.Create(MissingAddTelegratorWideWarning, loc));
                }
            });
        });
    }
}
