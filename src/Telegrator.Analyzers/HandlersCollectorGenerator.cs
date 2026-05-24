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

        string fullTypeName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var attributesList = new List<string>();
        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass == null)
                continue;

            string attrType = attr.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            string args = string.Join(", ", attr.ConstructorArguments.Select(a => a.ToCSharpString()));
            string named = string.Join(", ", attr.NamedArguments.Select(n => $"{n.Key} = {n.Value.ToCSharpString()}"));
            string initString = $"new {attrType}({args})" + (string.IsNullOrEmpty(named) ? "" : $" {{ {named} }}");
            attributesList.Add(initString);
        }

        return new HandlerRegistrationModel(fullTypeName, attributesList.ToImmutableArray(), isValid);
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<HandlerRegistrationModel> handlers)
    {
        if (handlers.IsDefaultOrEmpty)
            return;

        List<StatementSyntax> statements = [];
        List<string> foundHandlersNames = [];
        foreach (HandlerRegistrationModel handler in handlers.Distinct())
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            foundHandlersNames.Add(handler.FullClassName);

            string attrsArr = "new System.Attribute[] { " + string.Join(", ", handler.Attributes) + " }";

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
