using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace Telegrator.Analyzers;

[Generator(LanguageNames.CSharp)]
public class DeveloperHelperAnalyzer : IIncrementalGenerator
{
    internal record class HandlerDeclarationModel(string ClassName, string NamespaceName, string? AttributeName, string? BaseClassName, Location Location);

    private static readonly DiagnosticDescriptor MissingBaseClassWarning = new(
        id: "TLG101",
        title: "Missing handlers base class",
        messageFormat: "Class '{0}' has attribute [{1}], but doesn't inherits {1}",
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
        IncrementalValueProvider<ImmutableArray<HandlerDeclarationModel>> pipeline = context.SyntaxProvider
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

    private static HandlerDeclarationModel Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        ClassDeclarationSyntax classSyntax = (ClassDeclarationSyntax)context.Node;
        string? foundAttribute = classSyntax.GetHandlerAttributeName();
        string? foundBaseClass = classSyntax.GetHandlerBaseClassName();

        if (foundAttribute == null && foundBaseClass == null)
            return null!;

        string namespaceName = classSyntax.GetNamespace();
        return new HandlerDeclarationModel(
            classSyntax.Identifier.Text,
            namespaceName,
            foundAttribute,
            foundBaseClass,
            classSyntax.Identifier.GetLocation()
        );
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<HandlerDeclarationModel> handlers)
    {
        if (handlers.IsDefaultOrEmpty)
            return;

        List<MemberDeclarationSyntax> members = [];
        foreach (HandlerDeclarationModel handler in handlers)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (handler.AttributeName != null && handler.BaseClassName == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingBaseClassWarning, handler.Location, handler.ClassName, handler.AttributeName));
                continue;
            }

            if (handler.AttributeName == null && handler.BaseClassName != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingAttributeWarning, handler.Location, handler.ClassName, handler.BaseClassName));
                continue;
            }

            if (handler.AttributeName != handler.BaseClassName)
            {
                context.ReportDiagnostic(Diagnostic.Create(MismatchedHandlerWarning, handler.Location, handler.ClassName, handler.AttributeName, handler.BaseClassName));
                continue;
            }

            FieldDeclarationSyntax fieldDeclaration = GenerateTypeField(handler);
            members.Add(fieldDeclaration);
        }

        if (members.Count == 0)
            return;

        // 4. Сборка итогового файла
        ClassDeclarationSyntax classDeclaration = SyntaxFactory.ClassDeclaration("AnalyzerExport")
            .WithModifiers(SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                SyntaxFactory.Token(SyntaxKind.PartialKeyword)))
            .WithMembers(SyntaxFactory.List(members));

        NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Telegrator.Analyzers"))
            .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(classDeclaration));

        CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit()
            .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(namespaceDeclaration))
            .NormalizeWhitespace();

        SourceText sourceText = SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8);
        context.AddSource("AnalyzerExport.g.cs", sourceText);
    }

    private static FieldDeclarationSyntax GenerateTypeField(HandlerDeclarationModel handler)
    {
        string fullTypeName = handler.NamespaceName == "Global"
            ? handler.ClassName : $"{handler.NamespaceName}.{handler.ClassName}";

        TypeOfExpressionSyntax typeofExpression = SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName(fullTypeName));
        VariableDeclaratorSyntax variableDeclarator = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier($"{handler.ClassName}Type"))
            .WithInitializer(SyntaxFactory.EqualsValueClause(typeofExpression));

        return SyntaxFactory.FieldDeclaration(
            SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("System.Type"))
            .WithVariables(SyntaxFactory.SingletonSeparatedList(variableDeclarator)))
            .WithModifiers(SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)));
    }
}

internal static class DeveloperHelperAnalyzerExtensions
{
    private static readonly string[] HandlersNames =
    [
        "AnyUpdateHandler",
        "CallbackQueryHandler",
        "CommandHandler",
        "WelcomeHandler",
        "MessageHandler"
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