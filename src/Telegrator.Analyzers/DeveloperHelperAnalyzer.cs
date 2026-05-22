using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Telegrator.Analyzers;

[Generator(LanguageNames.CSharp)]
public class DeveloperHelperAnalyzer : IIncrementalGenerator
{
    internal record class HandlerDiagnosticModel(string ClassName, string? AttributeName, string? BaseClassName, Location Location);

    private static readonly DiagnosticDescriptor MissingBaseClassWarning = new(
        id: "TLG101",
        title: "Missing handlers base class",
        messageFormat: "Class '{0}' has attribute [{1}], but doesn't inherit '{1}'",
        category: "Telegrator.Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MissingAttributeWarning = new(
        id: "TLG102",
        title: "Missing handler annotation",
        messageFormat: "Class '{0}' inherits '{1}', but doesn't have required annotation [{1}]",
        category: "Telegrator.Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MismatchedHandlerWarning = new(
        id: "TLG103",
        title: "Handlers Annotation and BaseClass mismatch",
        messageFormat: "Class '{0}' has attribute [{1}], but inherits '{2}'",
        category: "Telegrator.Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<HandlerDiagnosticModel>> pipeline = context.SyntaxProvider
            .CreateSyntaxProvider(Provide, Transform)
            .Where(handler => handler != null)
            .Collect();

        context.RegisterSourceOutput(pipeline, Execute);
    }

    private static bool Provide(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (syntaxNode is not ClassDeclarationSyntax classSyntax)
            return false;

        if (classSyntax.BaseList?.Types.Count == 0 && classSyntax.AttributeLists.Count == 0)
            return false;

        return true;
    }

    private static HandlerDiagnosticModel Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        ClassDeclarationSyntax classSyntax = (ClassDeclarationSyntax)context.Node;
        string? foundAttribute = classSyntax.GetHandlerAttributeName();
        string? foundBaseClass = classSyntax.GetHandlerBaseClassName();

        if (foundAttribute == null && foundBaseClass == null)
            return null!;

        return new HandlerDiagnosticModel(
            classSyntax.Identifier.Text,
            foundAttribute,
            foundBaseClass,
            classSyntax.Identifier.GetLocation()
        );
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<HandlerDiagnosticModel> handlers)
    {
        if (handlers.IsDefaultOrEmpty)
            return;

        foreach (HandlerDiagnosticModel handler in handlers)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (handler.AttributeName != null && handler.BaseClassName == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingBaseClassWarning, handler.Location, handler.ClassName, handler.AttributeName));
            }
            else if (handler.AttributeName == null && handler.BaseClassName != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingAttributeWarning, handler.Location, handler.ClassName, handler.BaseClassName));
            }
            else if (handler.AttributeName != null && handler.BaseClassName != null && handler.AttributeName != handler.BaseClassName)
            {
                context.ReportDiagnostic(Diagnostic.Create(MismatchedHandlerWarning, handler.Location, handler.ClassName, handler.AttributeName, handler.BaseClassName));
            }
        }
    }
}

internal static class DeveloperHelperAnalyzerExtensions
{
    private static readonly string[] HandlersNames =
    [
        "AnyUpdateHandler",
        "CallbackQueryHandler",
        "CommandHandler",
        "InlineQueryHandler",
        "MessageHandler",
        "EditedMessageHandler",
        "ChannelPostHandler",
        "EditedChannelPostHandler",
        "BusinessMessageHandler",
        "EditedBusinessMessageHandler",
        "BusinessConnectionHandler",
        "DeletedBusinessMessagesHandler",
        "MessageReactionHandler",
        "MessageReactionCountHandler",
        "ShippingQueryHandler",
        "PreCheckoutQueryHandler",
        "PurchasedPaidMediaHandler",
        "PollHandler",
        "PollAnswerHandler",
        "MyChatMemberHandler",
        "ChatMemberHandler",
        "ChatJoinRequestHandler",
        "ChatBoostHandler",
        "RemovedChatBoostHandler"
    ];

    // Ищет атрибут и возвращает его нормализованное имя (без суффикса Attribute)
    public static string? GetHandlerAttributeName(this ClassDeclarationSyntax classSyntax)
    {
        string attributeName = classSyntax.AttributeLists
            .SelectMany(list => list.Attributes)
            .Select(attr => attr.Name.ToString())
            .FirstOrDefault(name => HandlersNames.Any(h => name == h || name == h + "Attribute"));

        return attributeName?.Replace("Attribute", "");
    }

    // Ищет базовый класс из нашего списка
    public static string? GetHandlerBaseClassName(this ClassDeclarationSyntax classSyntax)
    {
        if (classSyntax.BaseList == null)
            return null;

        return classSyntax.BaseList.Types
            .Select(t => t.Type.ToString())
            .FirstOrDefault(name => HandlersNames.Contains(name));
    }

    // Достает namespace, в котором объявлен класс
    public static string GetNamespace(this ClassDeclarationSyntax classDeclaration)
    {
        BaseNamespaceDeclarationSyntax? namespaceDeclaration = classDeclaration.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
        return namespaceDeclaration?.Name.ToString() ?? "Global";
    }
}
