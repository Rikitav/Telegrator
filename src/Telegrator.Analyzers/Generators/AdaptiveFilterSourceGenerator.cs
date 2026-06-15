using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Telegrator.Analyzers.RoslynExtensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Telegrator.Analyzers.Generators;

[Generator(LanguageNames.CSharp)]
public class AdaptiveFilterSourceGenerator : IIncrementalGenerator
{
    private readonly record struct FoundFilter(
        INamedTypeSymbol FilterImpl,
        INamedTypeSymbol FilterUpdatePayload);

    private readonly record struct CompilationModel(
        Compilation Compilation,
        INamedTypeSymbol? IFilter,
        ImmutableArray<FoundFilter> FoundFilters);

    private const string CoreHandlersNamespace = "Telegrator.Core.Handlers";
    private const string CoreAttributesNamespace = "Telegrator.Core.Attributes";
    private const string CoreFiltersNamespace = "Telegrator.Core.Filters";
    private const string AnnotationsNamespace = "Telegrator.Annotations";
    private const string BuilderExtensionsNamespace = "Telegrator.Handlers.Building";

    private const string IFilterMetadataName = "Telegrator.Core.Filters.IFilter`1";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<CompilationModel> model = context.CompilationProvider
            .Select(BuildCompilationModel);

        context.RegisterSourceOutput(model, Execute);
    }

    private static CompilationModel BuildCompilationModel(Compilation compilation, CancellationToken cancellationToken)
    {
        INamedTypeSymbol? IFilterType = compilation.GetTypeByMetadataName(IFilterMetadataName);
        ImmutableArray<FoundFilter> filterTypes = ImmutableArray<FoundFilter>.Empty;

        if (IFilterType != null)
        {
            filterTypes = SelectFilterImplementations(compilation.GetAllTypesFromCompilation(), IFilterType)
                .ToImmutableArray();
        }

        return new CompilationModel(compilation, IFilterType, filterTypes);
    }

    private static IEnumerable<FoundFilter> SelectFilterImplementations(IEnumerable<INamedTypeSymbol> symbols, INamedTypeSymbol IFilterType)
    {
        foreach (INamedTypeSymbol symbol in symbols)
        {
            if (symbol.TypeKind != TypeKind.Class || symbol.IsAbstract || symbol.IsStatic || symbol.IsGenericType)
                continue;

            if (symbol.ContainingNamespace.ToDisplayString() == CoreFiltersNamespace)
                continue;

            INamedTypeSymbol? updateTypeSymbol = symbol
                .Unfold(x => x.BaseType)
                .SelectMany(x => x.AllInterfaces)
                .FirstOrDefault(x => x.OriginalDefinition.Equals(IFilterType, SymbolEqualityComparer.Default))
                ?.TypeArguments.ElementAt(0) as INamedTypeSymbol;

            if (updateTypeSymbol == null)
                continue;

            yield return new FoundFilter(symbol, updateTypeSymbol);
        }
    }

    private static void Execute(SourceProductionContext context, CompilationModel model)
    {
        GenerateAttributes(context, model);
        GenerateExtensions(context, model);
    }

    private static void GenerateAttributes(SourceProductionContext context, CompilationModel model)
    {
        ImmutableArray<MemberDeclarationSyntax> attributes = ImmutableArray<MemberDeclarationSyntax>.Empty;
        HashSet<string> usedNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (FoundFilter foundFilter in model.FoundFilters)
        {
            if (foundFilter.FilterImpl.Name.EndsWith("AttributeFilter", StringComparison.Ordinal))
                continue;

            string attributeName = GetUniqueAttributeName(foundFilter.FilterImpl, foundFilter.FilterUpdatePayload, usedNames);
            if (AttributeTypeAlreadyExists(model.Compilation, attributeName))
                continue;

            attributes = attributes.Add(GenerateFilterAttribute(attributeName, foundFilter.FilterImpl, foundFilter.FilterUpdatePayload));
        }

        if (attributes.IsEmpty)
            return;

        NamespaceDeclarationSyntax namespaceDeclaration = NamespaceDeclaration(IdentifierName(AnnotationsNamespace))
            .AddMembers(attributes.ToArray());

        CompilationUnitSyntax compilationUnit = CompilationUnit()
            .AddUsings(
                UsingDirective(IdentifierName("System")),
                UsingDirective(IdentifierName("Telegram.Bot.Types")),
                UsingDirective(IdentifierName("Telegram.Bot.Types.Enums")),
                UsingDirective(IdentifierName(CoreFiltersNamespace)),
                UsingDirective(IdentifierName(CoreHandlersNamespace)),
                UsingDirective(IdentifierName(CoreAttributesNamespace)),
                UsingDirective(IdentifierName(AnnotationsNamespace)),
                UsingDirective(IdentifierName("Telegrator.Core.Handlers.Building")))
            .AddMembers(namespaceDeclaration)
            .NormalizeWhitespace();

        context.AddSource("Telegrator.Adaptive.Filters.Attributes.g.cs", compilationUnit.ToFullString());
    }

    private static void GenerateExtensions(SourceProductionContext context, CompilationModel model)
    {
        ImmutableArray<MemberDeclarationSyntax> extensions = ImmutableArray<MemberDeclarationSyntax>.Empty;
        HashSet<string> usedNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (FoundFilter foundFilter in model.FoundFilters)
        {
            if (foundFilter.FilterImpl.Name.EndsWith("AttributeFilter", StringComparison.Ordinal))
                continue;

            string attributeName = GetUniqueAttributeName(foundFilter.FilterImpl, foundFilter.FilterUpdatePayload, usedNames);
            string methodName = attributeName.EndsWith("Attribute", StringComparison.Ordinal)
                ? attributeName.Substring(0, attributeName.Length - "Attribute".Length)
                : attributeName;

            extensions = extensions.Add(GenerateBuilderExtension(methodName, foundFilter.FilterImpl, foundFilter.FilterUpdatePayload, model));
        }

        if (extensions.IsEmpty)
            return;

        ClassDeclarationSyntax extensionClass = ClassDeclaration("HandlerBuilderFilterExtensions")
            .AddModifiers(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.StaticKeyword))
            .AddMembers(extensions.ToArray());

        NamespaceDeclarationSyntax namespaceDeclaration = NamespaceDeclaration(IdentifierName(BuilderExtensionsNamespace))
            .AddMembers(extensionClass);

        CompilationUnitSyntax compilationUnit = CompilationUnit()
            .AddUsings(
                UsingDirective(IdentifierName("System")),
                UsingDirective(IdentifierName("Telegram.Bot.Types")),
                UsingDirective(IdentifierName("Telegram.Bot.Types.Enums")),
                UsingDirective(IdentifierName(CoreFiltersNamespace)),
                UsingDirective(IdentifierName(CoreHandlersNamespace)),
                UsingDirective(IdentifierName(CoreAttributesNamespace)),
                UsingDirective(IdentifierName(AnnotationsNamespace)),
                UsingDirective(IdentifierName("Telegrator.Core.Handlers.Building")))
            .AddMembers(namespaceDeclaration)
            .NormalizeWhitespace();

        context.AddSource("Telegrator.Adaptive.Filters.Extensions.g.cs", compilationUnit.ToFullString());
    }

    private static string GetUniqueAttributeName(INamedTypeSymbol filterType, INamedTypeSymbol updatePayload, HashSet<string> usedNames)
    {
        string strippedName = GetStrippedFilterName(filterType, updatePayload);
        string unstrippedName = filterType.Name.Replace("Filter", string.Empty);

        string[] candidates = { strippedName, unstrippedName };
        foreach (string candidate in candidates)
        {
            string attributeName = $"{candidate}Attribute";
            if (!usedNames.Contains(attributeName))
            {
                usedNames.Add(attributeName);
                return attributeName;
            }
        }

        int suffix = 2;
        while (usedNames.Contains($"{unstrippedName}{suffix}Attribute"))
            suffix++;

        string fallbackName = $"{unstrippedName}{suffix}Attribute";
        usedNames.Add(fallbackName);
        return fallbackName;
    }

    private static bool AttributeTypeAlreadyExists(Compilation compilation, string attributeName)
    {
        INamedTypeSymbol? symbol = compilation.GetTypeByMetadataName($"{AnnotationsNamespace}.{attributeName}");
        if (symbol is null)
            return false;

        // If the type lives in a referenced assembly, it is a real existing attribute.
        if (!symbol.ContainingAssembly.Equals(compilation.Assembly, SymbolEqualityComparer.Default))
            return true;

        // If it lives in the current assembly, make sure it is not a file generated by this
        // generator from a previous pass. Only skip user-written source attributes.
        foreach (Location location in symbol.Locations)
        {
            if (location.IsInSource)
            {
                string? path = location.SourceTree?.FilePath;
                if (path is null || !path.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        return false;
    }

    private static string GetStrippedFilterName(INamedTypeSymbol filterType, INamedTypeSymbol updatePayload)
    {
        string name = filterType.Name;
        string payloadName = updatePayload.Name;

        if (name.StartsWith(payloadName, StringComparison.Ordinal)
            && name.Length > payloadName.Length
            && char.IsUpper(name[payloadName.Length]))
        {
            string candidate = name.Substring(payloadName.Length);
            string firstWord = GetFirstWord(candidate);
            bool baseContains = filterType.BaseType?.Unfold(x => x.BaseType)
                .Any(b => b.Name.Contains(firstWord, StringComparison.Ordinal)) ?? false;

            if (baseContains)
                name = candidate;
        }

        return name.Replace("Filter", string.Empty);
    }

    private static string GetFirstWord(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        int i = 1;
        while (i < name.Length && !char.IsUpper(name[i]))
            i++;

        return name.Substring(0, i);
    }

    private static ClassDeclarationSyntax GenerateFilterAttribute(string attributeName, INamedTypeSymbol filterType, INamedTypeSymbol updatePayload)
    {
        string filterTypeName = filterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        string updatePayloadTypeName = updatePayload.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        ImmutableArray<IMethodSymbol> constructors = filterType.Constructors
            .Where(c => !c.IsStatic && c.DeclaredAccessibility == Accessibility.Public)
            .ToImmutableArray();

        HashSet<string> reservedNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (INamedTypeSymbol? type in filterType.BaseType.Unfold(x => x.BaseType))
        {
            foreach (ISymbol member in type.GetMembers())
                reservedNames.Add(member.Name);
        }
        reservedNames.Add(attributeName);

        Dictionary<string, string> parameterToProperty = new Dictionary<string, string>(StringComparer.Ordinal);
        List<(string PropertyName, ITypeSymbol Type)> properties = new List<(string, ITypeSymbol)>();

        foreach (IMethodSymbol constructor in constructors)
        {
            foreach (IParameterSymbol parameter in constructor.Parameters)
            {
                string key = $"{parameter.Name}|{parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}";
                if (parameterToProperty.ContainsKey(key))
                    continue;

                string propertyName = AdaptiveGeneratorHelpers.ToPascalCase(parameter.Name);
                if (reservedNames.Contains(propertyName) || properties.Any(p => p.PropertyName == propertyName))
                {
                    string baseName = AdaptiveGeneratorHelpers.ToPascalCase($"{parameter.Name}Value");
                    propertyName = baseName;
                    int suffix = 2;
                    while (reservedNames.Contains(propertyName) || properties.Any(p => p.PropertyName == propertyName))
                    {
                        propertyName = $"{baseName}{suffix}";
                        suffix++;
                    }
                }

                parameterToProperty[key] = propertyName;
                properties.Add((propertyName, parameter.Type));
            }
        }

        ClassDeclarationSyntax classDeclaration = ClassDeclaration(attributeName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(SimpleBaseType(
                GenericName("Telegrator.Attributes.UpdateFilterAttribute")
                    .AddTypeArgumentListArguments(ParseTypeName(updatePayloadTypeName))));

        foreach ((string propertyName, ITypeSymbol propertyType) in properties)
        {
            PropertyDeclarationSyntax property = PropertyDeclaration(
                attributeLists: [],
                modifiers: TokenList(Token(SyntaxKind.PublicKeyword)),
                type: ParseTypeName(propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
                explicitInterfaceSpecifier: null,
                identifier: Identifier(propertyName),
                accessorList: AccessorList([
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                ]),
                expressionBody: null,
                initializer: null,
                semicolonToken: Token(SyntaxKind.None));

            classDeclaration = classDeclaration.AddMembers(property);
        }

        foreach (IMethodSymbol constructor in constructors)
        {
            classDeclaration = classDeclaration.AddMembers(
                GenerateFilterAttributeConstructor(attributeName, constructor, filterTypeName, parameterToProperty));
        }

        string[]? updateTypes = AdaptiveGeneratorHelpers.PayloadNameToUpdateType(updatePayload.Name);
        if (updateTypes != null)
        {
            classDeclaration = classDeclaration.AddMembers(PropertyDeclaration(
                attributeLists: [],
                expressionBody: null,
                explicitInterfaceSpecifier: null,

                modifiers: TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)),
                type: ArrayType(ParseTypeName("UpdateType[]")),
                identifier: Identifier("AllowedTypes"),
                accessorList: AccessorList([AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))]),

                initializer: EqualsValueClause(ArrayCreationExpression(ArrayType(ParseTypeName("UpdateType[]")),
                    InitializerExpression(SyntaxKind.ArrayInitializerExpression, SeparatedList(updateTypes.Select(x => ParseExpression(x)))))),
                semicolonToken: Token(SyntaxKind.SemicolonToken)));
        }

        classDeclaration = classDeclaration.AddMembers(MethodDeclaration(ParseTypeName(updatePayloadTypeName), Identifier("GetFilterringTarget"))
            .WithParameterList(ParameterList([Parameter([], [], ParseTypeName("Update"), Identifier("update"), null)]))
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword))
            .WithBody(Block(ReturnStatement(ParseExpression($"update.GetActualUpdateObject<{updatePayloadTypeName}>()")))));

        SyntaxTriviaList typeDocs = AdaptiveGeneratorHelpers.ExtractXmlDocumentation(filterType);
        if (typeDocs.Count > 0)
            classDeclaration = classDeclaration.WithLeadingTrivia(typeDocs);

        return classDeclaration;
    }

    private static ConstructorDeclarationSyntax GenerateFilterAttributeConstructor(
        string attributeName,
        IMethodSymbol constructor,
        string filterTypeName,
        Dictionary<string, string> parameterToProperty)
    {
        ImmutableArray<ParameterSyntax> parameters = constructor.Parameters
            .Select(AdaptiveGeneratorHelpers.CreateParameterSyntax)
            .ToImmutableArray();

        ImmutableArray<ArgumentSyntax> arguments = constructor.Parameters
            .Select(p => Argument(IdentifierName(p.Name)))
            .ToImmutableArray();

        IEnumerable<StatementSyntax> assignments = constructor.Parameters
            .Select(p => ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ThisExpression(),
                        IdentifierName(parameterToProperty[$"{p.Name}|{p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}"])),
                    IdentifierName(p.Name))));

        ConstructorDeclarationSyntax constructorDeclaration = ConstructorDeclaration(Identifier(attributeName))
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(parameters.ToArray())
            .WithInitializer(
                ConstructorInitializer(SyntaxKind.BaseConstructorInitializer)
                    .WithArgumentList(ArgumentList(SingletonSeparatedList(
                        Argument(ObjectCreationExpression(ParseTypeName(filterTypeName))
                            .WithArgumentList(ArgumentList(SeparatedList(arguments))))))))
            .WithBody(Block(assignments));

        SyntaxTriviaList xmlDocs = AdaptiveGeneratorHelpers.ExtractXmlDocumentation(constructor);
        if (xmlDocs.Count > 0)
            constructorDeclaration = constructorDeclaration.WithLeadingTrivia(xmlDocs);

        return constructorDeclaration;
    }

    private static MethodDeclarationSyntax GenerateBuilderExtension(string methodName, INamedTypeSymbol filterType, INamedTypeSymbol updateType, CompilationModel model)
    {
        string filterTargetTypeName = ResolveFilterTargetTypeName(filterType);
        IMethodSymbol? primaryConstructor = filterType.Constructors.FirstOrDefault(c => !c.IsStatic && c.DeclaredAccessibility == Accessibility.Public);

        if (primaryConstructor is null)
            throw new InvalidOperationException($"Filter {filterType.Name} does not have a public constructor.");

        ImmutableArray<ParameterSyntax> parameters = primaryConstructor.Parameters
            .Select(AdaptiveGeneratorHelpers.CreateParameterSyntax)
            .ToImmutableArray();

        ImmutableArray<ArgumentSyntax> filterArguments = primaryConstructor.Parameters
            .Select(p => Argument(IdentifierName(p.Name)))
            .ToImmutableArray();

        TypeParameterSyntax builderTypeParameter = TypeParameter("TBuilder");

        ParameterSyntax builderParameter = Parameter(Identifier("builder"))
            .WithType(IdentifierName("TBuilder"))
            .AddModifiers(Token(SyntaxKind.ThisKeyword));

        ExpressionSyntax targetExtractorBody = filterTargetTypeName == "Update"
            ? IdentifierName("update")
            : MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("update"),
                IdentifierName(filterTargetTypeName));

        LambdaExpressionSyntax targetExtractor = SimpleLambdaExpression(
            Parameter(Identifier("update")),
            targetExtractorBody);

        ExpressionSyntax addTargetedFiltersInvocation = InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("builder"),
                GenericName("AddTargetedFilters")
                    .AddTypeArgumentListArguments(IdentifierName(filterTargetTypeName))))
            .WithArgumentList(ArgumentList(SeparatedList(new[]
            {
                Argument(targetExtractor),
                Argument(ObjectCreationExpression(ParseTypeName(filterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                    .WithArgumentList(ArgumentList(SeparatedList(filterArguments))))
            })));

        MethodDeclarationSyntax methodDeclaration = MethodDeclaration(IdentifierName("TBuilder"), methodName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .AddTypeParameterListParameters(builderTypeParameter)
            .AddParameterListParameters(builderParameter)
            .AddParameterListParameters(parameters.ToArray())
            .AddConstraintClauses(
                TypeParameterConstraintClause("TBuilder")
                    .AddConstraints(TypeConstraint(IdentifierName("IHandlerBuilder"))))
            .WithBody(Block(
                ExpressionStatement(addTargetedFiltersInvocation),
                ReturnStatement(IdentifierName("builder"))));

        SyntaxTriviaList xmlDocs = AdaptiveGeneratorHelpers.ExtractXmlDocumentation(filterType);
        if (xmlDocs.Count > 0)
            methodDeclaration = methodDeclaration.WithLeadingTrivia(xmlDocs);

        return methodDeclaration;
    }

    private static string ResolveFilterTargetTypeName(INamedTypeSymbol filterType)
    {
        INamedTypeSymbol? iFilterInterface = filterType.AllInterfaces
            .FirstOrDefault(i => i.OriginalDefinition.Name == "IFilter" && i.OriginalDefinition.ContainingNamespace.ToDisplayString() == "Telegrator.Core.Filters");

        if (iFilterInterface is null || iFilterInterface.TypeArguments.Length == 0)
            return "object";

        return iFilterInterface.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
    }
}

internal static class WackyExtensions
{
    public static IEnumerable<T> Unfold<T>(this T? target, Func<T, T?> action)
    {
        while (target != null)
        {
            yield return target;
            target = action(target);
        }
    }
}
