/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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

            string? normalizedBaseClass = handler.BaseClassName?.NormalizeBranchingName();

            if (handler.AttributeName != null && handler.BaseClassName == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingBaseClassWarning, handler.Location, handler.ClassName, handler.AttributeName));
            }
            else if (handler.AttributeName == null && handler.BaseClassName != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingAttributeWarning, handler.Location, handler.ClassName, normalizedBaseClass ?? handler.BaseClassName));
            }
            else if (handler.AttributeName != null && handler.BaseClassName != null && handler.AttributeName != normalizedBaseClass)
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
        // Regular handlers
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
        "RemovedChatBoostHandler",
        "TextMessageHandler",
        "PhotoMessageHandler",
        "DocumentMessageHandler",
        "CallbackDataHandler",
        // Branching handlers
        "BranchingAnyUpdateHandler",
        "BranchingCallbackQueryHandler",
        "BranchingCommandHandler",
        "BranchingInlineQueryHandler",
        "BranchingMessageHandler",
        "BranchingEditedMessageHandler",
        "BranchingChannelPostHandler",
        "BranchingEditedChannelPostHandler",
        "BranchingBusinessMessageHandler",
        "BranchingEditedBusinessMessageHandler",
        "BranchingBusinessConnectionHandler",
        "BranchingDeletedBusinessMessagesHandler",
        "BranchingMessageReactionHandler",
        "BranchingMessageReactionCountHandler",
        "BranchingShippingQueryHandler",
        "BranchingPreCheckoutQueryHandler",
        "BranchingPurchasedPaidMediaHandler",
        "BranchingPollHandler",
        "BranchingPollAnswerHandler",
        "BranchingMyChatMemberHandler",
        "BranchingChatMemberHandler",
        "BranchingChatJoinRequestHandler",
        "BranchingChatBoostHandler",
        "BranchingRemovedChatBoostHandler",
        "BranchingTextMessageHandler",
        "BranchingPhotoMessageHandler",
        "BranchingDocumentMessageHandler",
        "BranchingCallbackDataHandler"
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

    /// <summary>
    /// Normalizes a branching handler base class name to its regular counterpart.
    /// e.g. "BranchingMessageHandler" → "MessageHandler"
    /// </summary>
    public static string? NormalizeBranchingName(this string? baseClassName)
    {
        if (baseClassName == null)
            return null;

        if (baseClassName.StartsWith("Branching"))
            return baseClassName.Substring("Branching".Length);

        return baseClassName;
    }

    // Достает namespace, в котором объявлен класс
    public static string GetNamespace(this ClassDeclarationSyntax classDeclaration)
    {
        BaseNamespaceDeclarationSyntax? namespaceDeclaration = classDeclaration.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>();
        return namespaceDeclaration?.Name.ToString() ?? "Global";
    }
}
