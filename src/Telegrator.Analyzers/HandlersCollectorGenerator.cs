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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace Telegrator.Analyzers;

[Generator(LanguageNames.CSharp)]
public class HandlersCollectorGenerator : IIncrementalGenerator
{
    internal record class HandlerRegistrationModel(string FullClassName, ImmutableArray<string> Attributes, bool IsValid);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<HandlerRegistrationModel>> pipeline = context.SyntaxProvider
            .CreateSyntaxProvider(Provide, Transform)
            .Where(handler => handler != null && handler.IsValid)
            .Collect();

        IncrementalValueProvider<(Compilation Left, ImmutableArray<HandlerRegistrationModel> Right)> compilationAndHandlers = context.CompilationProvider.Combine(pipeline);
        context.RegisterSourceOutput(compilationAndHandlers, (spc, source) => Execute(spc, source.Left, source.Right));
    }

    private static bool Provide(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (syntaxNode is not ClassDeclarationSyntax classSyntax)
            return false;

        if (classSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword))
            return false;

        if (classSyntax.BaseList?.Types.Count == 0 && classSyntax.AttributeLists.Count == 0)
            return false;

        return true;
    }

    private static HandlerRegistrationModel Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        ClassDeclarationSyntax classSyntax = (ClassDeclarationSyntax)context.Node;
        string? foundAttribute = classSyntax.GetHandlerAttributeName();
        string? foundBaseClass = classSyntax.GetHandlerBaseClassName();

        bool isValid = foundAttribute != null && foundBaseClass != null && foundAttribute == foundBaseClass.NormalizeBranchingName();

        if (!isValid)
            return null!;

        INamedTypeSymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classSyntax, cancellationToken);
        if (symbol == null)
            return null!;

        string fullTypeName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        List<string> attributesList = new List<string>();
        bool hasMightAwait = false;
        foreach (AttributeData attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass == null)
                continue;

            string ns = attr.AttributeClass.ContainingNamespace?.ToDisplayString() ?? string.Empty;
            if (ns.StartsWith("System.Runtime") || ns.StartsWith("System.Diagnostics"))
                continue;

            string attrType = attr.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (attr.AttributeClass.Name is "MightAwaitAttribute" or "MightAwait")
                hasMightAwait = true;

            IEnumerable<TypedConstant> ctorArgs = attr.ConstructorArguments.IsDefault ? Enumerable.Empty<TypedConstant>() : attr.ConstructorArguments;
            IEnumerable<KeyValuePair<string, TypedConstant>> namedArgs = attr.NamedArguments.IsDefault ? Enumerable.Empty<KeyValuePair<string, TypedConstant>>() : attr.NamedArguments;

            string args = string.Join(", ", ctorArgs.Select(FormatTypedConstant));
            string named = string.Join(", ", namedArgs.Select(n => $"{n.Key} = {FormatTypedConstant(n.Value)}"));

            string initString = $"new {attrType}({args})" + (string.IsNullOrEmpty(named) ? "" : $" {{ {named} }}");
            attributesList.Add(initString);
        }

        // Auto-inject MightAwait if awaiting calls are present but attribute is missing
        if (!hasMightAwait)
        {
            List<string> awaitedTypes = ExtractAwaitedUpdateTypes(classSyntax);
            if (awaitedTypes.Count > 0)
            {
                string mightAwaitArgs = string.Join(", ", awaitedTypes.Select(t => $"global::Telegram.Bot.Types.Enums.UpdateType.{t}"));
                attributesList.Add($"new global::Telegrator.Annotations.MightAwaitAttribute({mightAwaitArgs})");
            }
        }

        return new HandlerRegistrationModel(fullTypeName, attributesList.ToImmutableArray(), isValid);
    }

    private static List<string> ExtractAwaitedUpdateTypes(ClassDeclarationSyntax classSyntax)
    {
        List<string> result = new List<string>();
        Dictionary<string, string> awaitingMethods = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["AwaitAny"] = "Unknown",
            ["AwaitMessage"] = "Message",
            ["AwaitCallbackQuery"] = "CallbackQuery",
            ["AwaitInlineQuery"] = "InlineQuery",
            ["AwaitChosenInlineResult"] = "ChosenInlineResult",
            ["AwaitEditedMessage"] = "EditedMessage",
            ["AwaitChannelPost"] = "ChannelPost",
            ["AwaitEditedChannelPost"] = "EditedChannelPost",
            ["AwaitBusinessMessage"] = "BusinessMessage",
            ["AwaitEditedBusinessMessage"] = "EditedBusinessMessage",
            ["AwaitDeletedBusinessMessages"] = "DeletedBusinessMessages",
            ["AwaitBusinessConnection"] = "BusinessConnection",
            ["AwaitMessageReaction"] = "MessageReaction",
            ["AwaitMessageReactionCount"] = "MessageReactionCount",
            ["AwaitShippingQuery"] = "ShippingQuery",
            ["AwaitPreCheckoutQuery"] = "PreCheckoutQuery",
            ["AwaitPurchasedPaidMedia"] = "PurchasedPaidMedia",
            ["AwaitPoll"] = "Poll",
            ["AwaitPollAnswer"] = "PollAnswer",
            ["AwaitMyChatMember"] = "MyChatMember",
            ["AwaitChatMember"] = "ChatMember",
            ["AwaitChatJoinRequest"] = "ChatJoinRequest",
            ["AwaitChatBoost"] = "ChatBoost",
            ["AwaitRemovedChatBoost"] = "RemovedChatBoost",
            ["AwaitManagedBot"] = "ManagedBot",
            ["AwaitGuestMessage"] = "GuestMessage",
            ["CancellAllCallbacks"] = "CallbackQuery"
        };

        HashSet<string> parametrizedMethods = new HashSet<string>(StringComparer.Ordinal)
        {
            "CreateAbstract",
            "CreateDeleting",
            "AwaitUpdate"
        };

        foreach (SyntaxNode node in classSyntax.DescendantNodes())
        {
            if (node is not InvocationExpressionSyntax invocation)
                continue;

            string? methodName = invocation.Expression switch
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

            if (methodName == null || string.IsNullOrEmpty(methodName))
                continue;

            if (awaitingMethods.TryGetValue(methodName, out string? updateType) && !result.Contains(updateType))
            {
                result.Add(updateType);
                continue;
            }

            if (parametrizedMethods.Contains(methodName) && invocation.ArgumentList.Arguments.Count > 0)
            {
                ExpressionSyntax firstArg = invocation.ArgumentList.Arguments[0].Expression;
                string? resolved = firstArg switch
                {
                    MemberAccessExpressionSyntax memberAccess when
                        memberAccess.Expression is IdentifierNameSyntax id && id.Identifier.Text == "UpdateType"
                        => memberAccess.Name.Identifier.Text,
                    _ => null
                };

                if (resolved != null && !string.IsNullOrEmpty(resolved) && !result.Contains(resolved))
                    result.Add(resolved);
            }
        }

        return result;
    }

    private static string FormatTypedConstant(TypedConstant arg)
    {
        if (arg.IsNull)
            return "null";

        if (arg.Kind == TypedConstantKind.Array)
        {
            if (arg.Values.IsDefault)
                return "null";

            IEnumerable<string> values = arg.Values.Select(FormatTypedConstant);
            string typeName = arg.Type is IArrayTypeSymbol arrayType
                ? arrayType.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                : "object";

            return $"new {typeName}[] {{ {string.Join(", ", values)} }}";
        }

        return arg.ToCSharpString();
    }

    private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<HandlerRegistrationModel> handlers)
    {
        /* TheMakarik asked me to
        if (handlers.IsDefaultOrEmpty)
            return;
        */

        List<StatementSyntax> statements = [];
        List<string> foundHandlersNames = [];
        foreach (HandlerRegistrationModel handler in handlers.Distinct())
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            foundHandlersNames.Add(handler.FullClassName);

            string attrsArr = handler.Attributes.Length > 0
                ? "new System.Attribute[] { " + string.Join(", ", handler.Attributes) + " }"
                : "null";

            string code = $"handlers.AddDescriptor(handlers.CreateClassDescriptor(typeof({handler.FullClassName}), {attrsArr}));";
            statements.Add(SyntaxFactory.ParseStatement(code));
        }

        statements.Add(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("handlers")));
        ParameterSyntax parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("handlers"))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
            .WithType(SyntaxFactory.ParseTypeName("IHandlersCollection"));

        StringBuilder summaryBuilder = new StringBuilder();
        summaryBuilder.AppendLine("/// <summary>");
        summaryBuilder.AppendLine("/// Collects all generated Telegrator handlers statically to support Native AOT compilation.");
        summaryBuilder.AppendLine("/// <br/>");
        summaryBuilder.AppendLine("/// Found handlers:");
        summaryBuilder.AppendLine("/// <list type=\"bullet\">");
        foreach (string name in foundHandlersNames)
        {
            // remove global:: if present to make see cref work nicer
            string docName = name.StartsWith("global::") ? name.Substring(8) : name;
            summaryBuilder.AppendLine($"/// <item><description><see cref=\"{docName}\"/></description></item>");
        }

        summaryBuilder.AppendLine("/// </list>");
        summaryBuilder.AppendLine("/// </summary>");

        SyntaxTriviaList methodTrivia = SyntaxFactory.ParseLeadingTrivia(summaryBuilder.ToString());

        MethodDeclarationSyntax methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("IHandlersCollection"), "CollectHandlers")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(parameter)))
            .WithBody(SyntaxFactory.Block(statements))
            .WithLeadingTrivia(methodTrivia);

        List<MemberDeclarationSyntax> generatedMembers = [methodDeclaration];

        bool hasHosting = compilation.ReferencedAssemblyNames.Any(a => a.Name == "Telegrator.Hosting")
            || compilation.AssemblyName == "Telegrator.Hosting";

        if (hasHosting)
        {
            string builderType = "global::Telegrator.Hosting.ITelegramBotHostBuilder";
            if (compilation.GetTypeByMetadataName("Telegrator.Hosting.ITelegratorBuilder") != null)
                builderType = "global::Telegrator.Hosting.ITelegratorBuilder";

            string hostingMethodCode = $@"
        /// <summary>
        /// Collects all generated Telegrator handlers using the Hosting builder statically to support Native AOT compilation.
        /// </summary>
        public static {builderType} CollectHandlers(this {builderType} builder)
        {{
            CollectHandlers(builder.Handlers);
            return builder;
        }}";

            MethodDeclarationSyntax hostingMethod = (MethodDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(hostingMethodCode)!;
            generatedMembers.Add(hostingMethod);
        }

        ClassDeclarationSyntax classDeclaration = SyntaxFactory.ClassDeclaration("TelegratorHandlersCollectionExtensions")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
            .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(generatedMembers));

        NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Telegrator"))
            .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(classDeclaration));

        SyntaxList<UsingDirectiveSyntax> usings = SyntaxFactory.List(new[]
        {
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Telegrator")),
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Telegrator.Core"))
        });

        SyntaxTriviaList fileTrivia = SyntaxFactory.ParseLeadingTrivia("// <auto-generated />\n#pragma warning disable CS1591\n");
        CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit()
            .WithUsings(usings)
            .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(namespaceDeclaration))
            .WithLeadingTrivia(fileTrivia)
            .NormalizeWhitespace();

        context.AddSource("TelegratorHandlersCollectionExtensions.g.cs", SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
    }
}
