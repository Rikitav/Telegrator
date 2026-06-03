using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Telegrator.Analyzers;

[Generator(LanguageNames.CSharp)]
public class MightAwaitAnalyzer : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor MissingMightAwaitWarning = new(
        id: "TLG201",
        title: "Missing MightAwait attribute",
        messageFormat: "Handler '{0}' calls awaiting method '{1}' but is missing [MightAwait({2})]",
        category: "Telegrator.Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly string[] AwaitingMethodNames =
    [
        "AwaitAny",
        "AwaitMessage",
        "AwaitCallbackQuery",
        "AwaitInlineQuery",
        "AwaitChosenInlineResult",
        "AwaitEditedMessage",
        "AwaitChannelPost",
        "AwaitEditedChannelPost",
        "AwaitBusinessMessage",
        "AwaitEditedBusinessMessage",
        "AwaitDeletedBusinessMessages",
        "AwaitBusinessConnection",
        "AwaitMessageReaction",
        "AwaitMessageReactionCount",
        "AwaitShippingQuery",
        "AwaitPreCheckoutQuery",
        "AwaitPurchasedPaidMedia",
        "AwaitPoll",
        "AwaitPollAnswer",
        "AwaitMyChatMember",
        "AwaitChatMember",
        "AwaitChatJoinRequest",
        "AwaitChatBoost",
        "AwaitRemovedChatBoost",
        "AwaitManagedBot",
        "AwaitGuestMessage",
        "CancellAllCallbacks"
    ];

    private static readonly string[] ParametrizedAwaitingMethodNames =
    [
        "CreateAbstract",
        "CreateDeleting",
        "AwaitUpdate"
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var pipeline = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                Transform)
            .Where(static model => model != null)
            .Collect();

        context.RegisterSourceOutput(pipeline, Execute);
    }

    private static MightAwaitDiagnosticModel? Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var classSyntax = (ClassDeclarationSyntax)context.Node;

        // Only process valid handlers
        string? foundAttribute = classSyntax.GetHandlerAttributeName();
        string? foundBaseClass = classSyntax.GetHandlerBaseClassName();
        bool isValidHandler = foundAttribute != null && foundBaseClass != null && foundAttribute == foundBaseClass;

        if (!isValidHandler)
            return null;

        // Search for awaiting calls inside the class
        var awaitingCalls = new List<(string MethodName, string UpdateType)>();
        foreach (var node in classSyntax.DescendantNodes())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (node is not InvocationExpressionSyntax invocation)
                continue;

            string? methodName = ExtractMethodName(invocation);
            if (string.IsNullOrEmpty(methodName))
                continue;

            if (AwaitingMethodNames.Contains(methodName))
            {
                string updateType = ResolveUpdateType(methodName);
                awaitingCalls.Add((methodName, updateType));
            }
            else if (ParametrizedAwaitingMethodNames.Contains(methodName))
            {
                string? updateType = ExtractUpdateTypeFromArguments(invocation);
                if (updateType != null)
                    awaitingCalls.Add((methodName, updateType));
            }
        }

        if (awaitingCalls.Count == 0)
            return null;

        // Check if MightAwait attribute is already present
        bool hasMightAwait = classSyntax.AttributeLists
            .SelectMany(static list => list.Attributes)
            .Any(static attr => attr.Name.ToString() is "MightAwait" or "MightAwaitAttribute");

        if (hasMightAwait)
            return null;

        var symbol = context.SemanticModel.GetDeclaredSymbol(classSyntax, cancellationToken);
        string className = symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            ?? classSyntax.Identifier.Text;

        return new MightAwaitDiagnosticModel(className, awaitingCalls, classSyntax.Identifier.GetLocation());
    }

    private static string? ExtractMethodName(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name switch
            {
                IdentifierNameSyntax id => id.Identifier.Text,
                GenericNameSyntax generic => generic.Identifier.Text,
                _ => null
            },
            IdentifierNameSyntax identifier => identifier.Identifier.Text,
            _ => null
        };
    }

    private static string? ExtractUpdateTypeFromArguments(InvocationExpressionSyntax invocation)
    {
        if (invocation.ArgumentList.Arguments.Count == 0)
            return null;

        var firstArg = invocation.ArgumentList.Arguments[0].Expression;

        // Handles UpdateType.Message
        if (firstArg is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Expression is IdentifierNameSyntax id &&
            id.Identifier.Text == "UpdateType")
        {
            return $"UpdateType.{memberAccess.Name.Identifier.Text}";
        }

        // Handles global::Telegram.Bot.Types.Enums.UpdateType.Message
        if (firstArg is MemberAccessExpressionSyntax deepAccess &&
            deepAccess.Expression is MemberAccessExpressionSyntax)
        {
            // Try to resolve the rightmost identifier
            var rightmost = deepAccess.Name.Identifier.Text;
            if (!string.IsNullOrEmpty(rightmost))
                return $"UpdateType.{rightmost}";
        }

        return null;
    }

    private static string ResolveUpdateType(string methodName)
    {
        return methodName switch
        {
            "AwaitAny" => "UpdateType.Unknown",
            "AwaitMessage" => "UpdateType.Message",
            "AwaitCallbackQuery" => "UpdateType.CallbackQuery",
            "AwaitInlineQuery" => "UpdateType.InlineQuery",
            "AwaitChosenInlineResult" => "UpdateType.ChosenInlineResult",
            "AwaitEditedMessage" => "UpdateType.EditedMessage",
            "AwaitChannelPost" => "UpdateType.ChannelPost",
            "AwaitEditedChannelPost" => "UpdateType.EditedChannelPost",
            "AwaitBusinessMessage" => "UpdateType.BusinessMessage",
            "AwaitEditedBusinessMessage" => "UpdateType.EditedBusinessMessage",
            "AwaitDeletedBusinessMessages" => "UpdateType.DeletedBusinessMessages",
            "AwaitBusinessConnection" => "UpdateType.BusinessConnection",
            "AwaitMessageReaction" => "UpdateType.MessageReaction",
            "AwaitMessageReactionCount" => "UpdateType.MessageReactionCount",
            "AwaitShippingQuery" => "UpdateType.ShippingQuery",
            "AwaitPreCheckoutQuery" => "UpdateType.PreCheckoutQuery",
            "AwaitPurchasedPaidMedia" => "UpdateType.PurchasedPaidMedia",
            "AwaitPoll" => "UpdateType.Poll",
            "AwaitPollAnswer" => "UpdateType.PollAnswer",
            "AwaitMyChatMember" => "UpdateType.MyChatMember",
            "AwaitChatMember" => "UpdateType.ChatMember",
            "AwaitChatJoinRequest" => "UpdateType.ChatJoinRequest",
            "AwaitChatBoost" => "UpdateType.ChatBoost",
            "AwaitRemovedChatBoost" => "UpdateType.RemovedChatBoost",
            "AwaitManagedBot" => "UpdateType.ManagedBot",
            "AwaitGuestMessage" => "UpdateType.GuestMessage",
            "CancellAllCallbacks" => "UpdateType.CallbackQuery",
            _ => "UpdateType.Unknown"
        };
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<MightAwaitDiagnosticModel?> handlers)
    {
        if (handlers.IsDefaultOrEmpty)
            return;

        foreach (var handler in handlers)
        {
            if (handler == null)
                continue;

            context.CancellationToken.ThrowIfCancellationRequested();

            var firstCall = handler.AwaitingCalls[0];
            context.ReportDiagnostic(Diagnostic.Create(
                MissingMightAwaitWarning,
                handler.Location,
                handler.ClassName,
                firstCall.MethodName,
                firstCall.UpdateType));
        }
    }
}

internal sealed record MightAwaitDiagnosticModel(string ClassName, List<(string MethodName, string UpdateType)> AwaitingCalls, Location Location);
