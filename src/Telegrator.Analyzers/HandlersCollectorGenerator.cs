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
    internal record class HandlerRegistrationModel(string FullClassName, bool IsValid);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<HandlerRegistrationModel>> pipeline = context.SyntaxProvider
            .CreateSyntaxProvider(Provide, Transform)
            .Where(handler => handler != null && handler.IsValid)
            .Collect();

        context.RegisterSourceOutput(pipeline, Execute);
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

        bool isValid = foundAttribute != null && foundBaseClass != null && foundAttribute == foundBaseClass;

        if (!isValid)
            return null!;

        INamedTypeSymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classSyntax, cancellationToken);
        if (symbol == null)
            return null!;

        if (symbol.DeclaredAccessibility == Accessibility.Private)
            return null!;

        string fullTypeName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return new HandlerRegistrationModel(fullTypeName, isValid);
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<HandlerRegistrationModel> handlers)
    {
        if (handlers.IsDefaultOrEmpty)
            return;

        List<StatementSyntax> statements = [];
        foreach (HandlerRegistrationModel handler in handlers.Distinct())
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            TypeSyntax typeArg = SyntaxFactory.ParseTypeName(handler.FullClassName);
            GenericNameSyntax addHandlerMethod = SyntaxFactory.GenericName(SyntaxFactory.Identifier("AddHandler"))
                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(typeArg)));

            MemberAccessExpressionSyntax memberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("handlers"),
                addHandlerMethod);

            InvocationExpressionSyntax invocation = SyntaxFactory.InvocationExpression(memberAccess);
            statements.Add(SyntaxFactory.ExpressionStatement(invocation));
        }

        statements.Add(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("handlers")));
        ParameterSyntax parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("handlers"))
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
            .WithType(SyntaxFactory.ParseTypeName("IHandlersCollection"));

        SyntaxTriviaList methodTrivia = SyntaxFactory.ParseLeadingTrivia(
            "/// <summary>\n" +
            "/// Collects all generated Telegrator handlers statically to support Native AOT compilation.\n" +
            "/// </summary>\n");

        MethodDeclarationSyntax methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("IHandlersCollection"), "CollectHandlers")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(parameter)))
            .WithBody(SyntaxFactory.Block(statements))
            .WithLeadingTrivia(methodTrivia);

        ClassDeclarationSyntax classDeclaration = SyntaxFactory.ClassDeclaration("TelegratorHandlersCollectionExtensions")
            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
            .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(methodDeclaration));

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