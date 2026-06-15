using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Telegrator.Analyzers.RoslynExtensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Telegrator.Analyzers.Generators;

[Generator(LanguageNames.CSharp)]
public class AdaptiveHandlerSourceGenerator : IIncrementalGenerator
{
    private readonly record struct GeneratedSource(
        string HintName,
        string SourceText);

    private readonly record struct CompilationModel(
        Compilation Compilation,
        INamedTypeSymbol? UpdateTypeEnum,
        INamedTypeSymbol? UpdateHandlerBase,
        INamedTypeSymbol? BranchingUpdateHandler,
        INamedTypeSymbol? UpdateFilterAttributeBase,
        INamedTypeSymbol? IHandlerBuilder,
        INamedTypeSymbol? IHandlerContainer,
        INamedTypeSymbol? ProxyMarkerAttribute,
        ImmutableArray<string> UpdateTypeNames,
        ImmutableArray<IMethodSymbol> ProxyExtensionMethods);

    private const string ProxyMarkerAttributeName = "Telegrator.Attributes.GenerateHandlerProxyAttribute";

    private const string CoreHandlersNamespace = "Telegrator.Core.Handlers";
    private const string CoreAttributesNamespace = "Telegrator.Core.Attributes";
    private const string CoreFiltersNamespace = "Telegrator.Core.Filters";

    private const string UpdateTypeMetadataName = "Telegram.Bot.Types.Enums.UpdateType";
    private const string UpdateHandlerBaseMetadataName = "Telegrator.Core.Handlers.UpdateHandlerBase";
    private const string BranchingUpdateHandlerMetadataName = "Telegrator.Core.Handlers.BranchingUpdateHandler`1";
    private const string UpdateFilterAttributeBaseMetadataName = "Telegrator.Core.Attributes.UpdateFilterAttributeBase";
    private const string IHandlerBuilderMetadataName = "Telegrator.Core.Handlers.Building.IHandlerBuilder";
    private const string IHandlerContainerMetadataName = "Telegrator.Handlers.IHandlerContainer`1";

    private static CompilationModel BuildCompilationModel(Compilation compilation, CancellationToken cancellationToken)
    {
        INamedTypeSymbol? updateTypeEnum = compilation.GetTypeByMetadataName(UpdateTypeMetadataName);
        INamedTypeSymbol? updateHandlerBase = compilation.GetTypeByMetadataName(UpdateHandlerBaseMetadataName);
        INamedTypeSymbol? branchingUpdateHandler = compilation.GetTypeByMetadataName(BranchingUpdateHandlerMetadataName);
        INamedTypeSymbol? updateFilterAttributeBase = compilation.GetTypeByMetadataName(UpdateFilterAttributeBaseMetadataName);
        INamedTypeSymbol? iHandlerBuilder = compilation.GetTypeByMetadataName(IHandlerBuilderMetadataName);
        INamedTypeSymbol? iHandlerContainer = compilation.GetTypeByMetadataName(IHandlerContainerMetadataName);
        INamedTypeSymbol? proxyMarkerAttribute = compilation.GetTypeByMetadataName(ProxyMarkerAttributeName);

        ImmutableArray<string> updateTypeNames = updateTypeEnum is not null
            ? updateTypeEnum.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => f.HasConstantValue)
                .Select(f => f.Name)
                .ToImmutableArray()
            : ImmutableArray<string>.Empty;

        ImmutableArray<IMethodSymbol> proxyExtensionMethods = proxyMarkerAttribute is not null
            ? compilation.SourceModule.ReferencedAssemblySymbols
                .SelectMany(a => a.GetAllDefinedTypes())
                .Concat(compilation.Assembly.GetAllDefinedTypes())
                .SelectMany(t => t.GetMembers().OfType<IMethodSymbol>())
                .Where(m => m.IsStatic && m.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, proxyMarkerAttribute)))
                .ToImmutableArray()
            : ImmutableArray<IMethodSymbol>.Empty;

        return new CompilationModel(
            compilation,
            updateTypeEnum,
            updateHandlerBase,
            branchingUpdateHandler,
            updateFilterAttributeBase,
            iHandlerBuilder,
            iHandlerContainer,
            proxyMarkerAttribute,
            updateTypeNames,
            proxyExtensionMethods);
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<CompilationModel> model = context.CompilationProvider
            .Select(BuildCompilationModel);

        context.RegisterSourceOutput(model, (spc, source) => Execute(spc, source));
    }

    private static void Execute(SourceProductionContext context, CompilationModel model)
    {
        if (model.UpdateTypeEnum is null)
            return;

        foreach (string updateTypeName in model.UpdateTypeNames)
        {
            if (updateTypeName == "Unknown")
                continue;

            GeneratedSource? handlerSource = GenerateHandlerSource(model, updateTypeName);
            if (handlerSource.HasValue)
                context.AddSource(handlerSource.Value.HintName, handlerSource.Value.SourceText);
        }
    }

    private static GeneratedSource? GenerateHandlerSource(CompilationModel model, string updateTypeName)
    {
        string className = $"{updateTypeName}Handler";
        string branchingClassName = $"Branching{updateTypeName}Handler";
        string attributeName = $"{className}Attribute";

        bool attributeExists = model.Compilation.GetTypeByMetadataName($"Telegrator.Handlers.{attributeName}") is not null;
        bool handlerExists = model.Compilation.GetTypeByMetadataName($"Telegrator.Handlers.{className}") is not null;
        bool branchingHandlerExists = model.Compilation.GetTypeByMetadataName($"Telegrator.Handlers.{branchingClassName}") is not null;

        if (attributeExists && handlerExists && branchingHandlerExists)
            return null;

        string? payloadTypeName = AdaptiveGeneratorHelpers.UpdateTypeToPayloadName(updateTypeName);
        if (payloadTypeName is null)
            return null;

        ImmutableArray<MemberDeclarationSyntax> members = ImmutableArray<MemberDeclarationSyntax>.Empty;

        if (!attributeExists)
            members = members.Add(GenerateHandlerAttribute(attributeName, className, branchingClassName, updateTypeName));

        if (!handlerExists)
            members = members.Add(GenerateHandlerClass(className, updateTypeName, payloadTypeName, model.ProxyExtensionMethods, model.IHandlerContainer));

        if (!branchingHandlerExists)
            members = members.Add(GenerateBranchingHandlerClass(branchingClassName, updateTypeName, payloadTypeName, model.ProxyExtensionMethods, model.IHandlerContainer));

        NamespaceDeclarationSyntax namespaceDeclaration = NamespaceDeclaration(IdentifierName("Telegrator.Handlers"))
            .AddUsings(
                UsingDirective(IdentifierName("System")),
                UsingDirective(IdentifierName("System.Threading")),
                UsingDirective(IdentifierName("System.Threading.Tasks")),
                UsingDirective(IdentifierName("Telegram.Bot.Types")),
                UsingDirective(IdentifierName("Telegram.Bot.Types.Enums")),
                UsingDirective(IdentifierName(CoreHandlersNamespace)),
                UsingDirective(IdentifierName(CoreAttributesNamespace)),
                UsingDirective(IdentifierName(CoreFiltersNamespace)),
                UsingDirective(IdentifierName("Telegrator.Attributes")),
                UsingDirective(IdentifierName("Telegrator")))
            .AddMembers(members.ToArray());

        CompilationUnitSyntax compilationUnit = CompilationUnit()
            .AddMembers(namespaceDeclaration)
            .NormalizeWhitespace();

        string sourceText = compilationUnit.ToFullString();
        string hintName = $"Telegrator.Adaptive.{className}.g.cs";
        return new GeneratedSource(hintName, sourceText);
    }

    private static ClassDeclarationSyntax GenerateHandlerClass(
        string className,
        string updateTypeName,
        string payloadTypeName,
        ImmutableArray<IMethodSymbol> proxyMethods,
        INamedTypeSymbol? iHandlerContainer)
    {
        ClassDeclarationSyntax classDeclaration = ClassDeclaration(className)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AbstractKeyword), Token(SyntaxKind.PartialKeyword))
            .AddBaseListTypes(SimpleBaseType(
                GenericName("AbstractUpdateHandler")
                    .AddTypeArgumentListArguments(ParseTypeName(payloadTypeName))))
            .AddMembers(
                ConstructorDeclaration(className)
                    .AddModifiers(Token(SyntaxKind.ProtectedKeyword))
                    .WithParameterList(ParameterList())
                    .WithInitializer(
                        ConstructorInitializer(SyntaxKind.BaseConstructorInitializer)
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(
                                Argument(MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("UpdateType"),
                                    IdentifierName(updateTypeName)))))))
                    .WithBody(Block()));

        foreach (IMethodSymbol proxyMethod in proxyMethods)
        {
            if (ShouldGenerateProxyForHandler(proxyMethod, payloadTypeName, iHandlerContainer))
                classDeclaration = classDeclaration.AddMembers(GenerateProxyMethod(proxyMethod));
        }

        return classDeclaration;
    }

    private static ClassDeclarationSyntax GenerateBranchingHandlerClass(
        string className,
        string updateTypeName,
        string payloadTypeName,
        ImmutableArray<IMethodSymbol> proxyMethods,
        INamedTypeSymbol? iHandlerContainer)
    {
        ClassDeclarationSyntax classDeclaration = ClassDeclaration(className)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AbstractKeyword), Token(SyntaxKind.PartialKeyword))
            .AddBaseListTypes(SimpleBaseType(
                GenericName("BranchingUpdateHandler")
                    .AddTypeArgumentListArguments(ParseTypeName(payloadTypeName))))
            .AddMembers(
                ConstructorDeclaration(className)
                    .AddModifiers(Token(SyntaxKind.ProtectedKeyword))
                    .WithParameterList(ParameterList())
                    .WithInitializer(
                        ConstructorInitializer(SyntaxKind.BaseConstructorInitializer)
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(
                                Argument(MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("UpdateType"),
                                    IdentifierName(updateTypeName)))))))
                    .WithBody(Block()));

        foreach (IMethodSymbol proxyMethod in proxyMethods)
        {
            if (ShouldGenerateProxyForHandler(proxyMethod, payloadTypeName, iHandlerContainer))
                classDeclaration = classDeclaration.AddMembers(GenerateProxyMethod(proxyMethod));
        }

        return classDeclaration;
    }

    private static ClassDeclarationSyntax GenerateHandlerAttribute(
        string attributeName,
        string handlerClassName,
        string branchingHandlerClassName,
        string updateTypeName)
    {
        TypeSyntax handlerType = IdentifierName(handlerClassName);
        TypeSyntax branchingHandlerType = IdentifierName(branchingHandlerClassName);

        TypeSyntax updateHandlerAttributeType = GenericName("UpdateHandlerAttribute")
            .AddTypeArgumentListArguments(handlerType);

        ArrayCreationExpressionSyntax additionalTypesArray = ArrayCreationExpression(
            ArrayType(IdentifierName("Type"))
                .AddRankSpecifiers(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))))
            .WithInitializer(
                InitializerExpression(SyntaxKind.ArrayInitializerExpression,
                    SingletonSeparatedList<ExpressionSyntax>(TypeOfExpression(branchingHandlerType))));

        ConstructorInitializerSyntax baseInitializer = ConstructorInitializer(SyntaxKind.BaseConstructorInitializer)
            .WithArgumentList(ArgumentList(SeparatedList(new[]
            {
                Argument(additionalTypesArray),
                Argument(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("UpdateType"),
                    IdentifierName(updateTypeName))),
                Argument(IdentifierName("importance"))
            })));

        MethodDeclarationSyntax canPassMethod = MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), "CanPass")
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword))
            .AddParameterListParameters(
                Parameter(Identifier("context"))
                    .WithType(
                        GenericName("FilterExecutionContext")
                            .AddTypeArgumentListArguments(IdentifierName("Update"))))
            .WithExpressionBody(
            ArrowExpressionClause(
                BinaryExpression(
                    SyntaxKind.NotEqualsExpression,
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("context"),
                            IdentifierName("Input")),
                        IdentifierName(updateTypeName)),
                    LiteralExpression(SyntaxKind.NullLiteralExpression))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        return ClassDeclaration(attributeName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(SimpleBaseType(updateHandlerAttributeType))
            .AddMembers(
                ConstructorDeclaration(attributeName)
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .AddParameterListParameters(
                        Parameter(Identifier("importance"))
                            .WithType(PredefinedType(Token(SyntaxKind.IntKeyword)))
                            .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0)))))
                    .WithInitializer(baseInitializer)
                    .WithBody(Block()),
                canPassMethod);
    }

    private static bool ShouldGenerateProxyForHandler(IMethodSymbol method, string payloadTypeName, INamedTypeSymbol? iHandlerContainer)
    {
        if (!method.IsExtensionMethod || method.Parameters.IsDefaultOrEmpty || iHandlerContainer is null)
            return true;

        ITypeSymbol firstParamType = method.Parameters[0].Type;
        if (firstParamType is not INamedTypeSymbol named)
            return false;

        if (!named.OriginalDefinition.Equals(iHandlerContainer, SymbolEqualityComparer.Default))
            return false;

        if (named.TypeArguments.Length == 0)
            return false;

        string targetPayload = named.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return targetPayload == payloadTypeName;
    }

    private static MethodDeclarationSyntax GenerateProxyMethod(IMethodSymbol method)
    {
        string methodName = method.Name;
        ITypeSymbol returnType = method.ReturnType;

        var args = method.GetAttributes().First(x => x.AttributeClass?.Name == "GenerateHandlerProxyAttribute").ConstructorArguments;
        if (args.Length == 1)
            methodName = (string)args[0].Value!;

        TypeSyntax returnTypeSyntax = returnType.SpecialType == SpecialType.System_Void
            ? PredefinedType(Token(SyntaxKind.VoidKeyword))
            : ParseTypeName(returnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

        bool isExtension = method.IsExtensionMethod;
        ImmutableArray<IParameterSymbol> proxyParameters = isExtension && !method.Parameters.IsDefaultOrEmpty
            ? method.Parameters.RemoveAt(0)
            : method.Parameters;

        ImmutableArray<ParameterSyntax> parameters = proxyParameters
            .Select(p =>
            {
                ParameterSyntax parameter = Parameter(Identifier(p.Name))
                    .WithType(ParseTypeName(p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

                if (p.HasExplicitDefaultValue)
                {
                    ExpressionSyntax defaultExpression = p.ExplicitDefaultValue switch
                    {
                        null => p.Type.IsValueType
                            ? LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                            : LiteralExpression(SyntaxKind.NullLiteralExpression),
                        bool b => LiteralExpression(b ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
                        int i => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(i)),
                        string s => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(s)),
                        _ => ParseExpression(p.ExplicitDefaultValue.ToString()!)
                    };

                    parameter = parameter.WithDefault(EqualsValueClause(defaultExpression));
                }

                return parameter;
            })
            .ToImmutableArray();

        ImmutableArray<ArgumentSyntax> arguments = isExtension
            ? ImmutableArray.Create(Argument(IdentifierName("Container")))
                .AddRange(proxyParameters.Select(p => Argument(IdentifierName(p.Name))))
            : proxyParameters.Select(p => Argument(IdentifierName(p.Name))).ToImmutableArray();

        ExpressionSyntax invocation = InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(method.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                IdentifierName(method.Name)))
            .WithArgumentList(ArgumentList(SeparatedList(arguments)));

        StatementSyntax bodyStatement = returnType.SpecialType == SpecialType.System_Void
            ? ExpressionStatement(invocation)
            : ReturnStatement(invocation);

        MethodDeclarationSyntax methodDeclaration = MethodDeclaration(returnTypeSyntax, methodName)
            .AddModifiers(Token(SyntaxKind.ProtectedKeyword))
            .AddParameterListParameters(parameters.ToArray())
            .WithBody(Block(bodyStatement));

        return methodDeclaration;
    }
}
