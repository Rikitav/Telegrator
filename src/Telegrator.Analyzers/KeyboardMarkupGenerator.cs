using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace Telegrator.Analyzers;

[Generator(LanguageNames.CSharp)]
public class KeyboardMarkupGenerator : IIncrementalGenerator
{
    // Records
    private record class GeneratedMarkupMethodModel(MethodDeclarationSyntax OriginalMethod, FieldDeclarationSyntax GeneratedField, MethodDeclarationSyntax GeneratedMethod);
    private record class GeneratedMarkupPropertyModel(PropertyDeclarationSyntax OriginalProperty, PropertyDeclarationSyntax GeneratedProperty);

    // Return types
    private const string InlineReturnType = "InlineKeyboardMarkup";
    private const string ReplyReturnType = "ReplyKeyboardMarkup";

    // Attribute names
    private const string CallbackDataAttribute = "CallbackButton";
    private const string CallbackGameAttribute = "GameButton";
    private const string CopyTextAttribute = "CopyTextButton";
    private const string LoginRequestAttribute = "LoginRequestButton";
    private const string PayRequestAttribute = "PayRequestButton";
    private const string SwitchQueryAttribute = "SwitchQueryButton";
    private const string QueryChosenAttribute = "QueryChosenButton";
    private const string QueryCurrentAttribute = "QueryCurrentButton";
    private const string UrlRedirectAttribute = "UrlRedirectButton";
    private const string RequestChatAttribute = "RequestChatButton";
    private const string RequestContactAttribute = "RequestContactButton";
    private const string RequestLocationAttribute = "RequestLocationButton";
    private const string RequestPoolAttribute = "RequestPoolButton";
    private const string RequestUsersAttribute = "RequestUsersButton";
    private const string WebAppAttribute = "WebApp";

    // Markup lists
    private static readonly string[] InlineAttributes = [CallbackDataAttribute, CallbackGameAttribute, CopyTextAttribute, LoginRequestAttribute, PayRequestAttribute, UrlRedirectAttribute, WebAppAttribute, SwitchQueryAttribute, QueryChosenAttribute, QueryCurrentAttribute];
    private static readonly string[] ReplyAttributes = [RequestChatAttribute, RequestContactAttribute, RequestLocationAttribute, RequestPoolAttribute, RequestUsersAttribute, WebAppAttribute];

    // Usings
    private static readonly string[] DefaultUsings = ["Telegram.Bot.Types.ReplyMarkups"];

    // Markup layouts
    private static readonly Dictionary<string, MemberAccessExpressionSyntax> InlineKeyboardLayout = new Dictionary<string, MemberAccessExpressionSyntax>()
    {
        { CallbackDataAttribute, AccessExpression("InlineKeyboardButton", "WithCallbackData") },
        { CallbackGameAttribute, AccessExpression("InlineKeyboardButton", "WithCallbackGame") },
        { CopyTextAttribute, AccessExpression("InlineKeyboardButton", "WithCopyText") },
        { LoginRequestAttribute, AccessExpression("InlineKeyboardButton", "WithLoginUrl") },
        { PayRequestAttribute, AccessExpression("InlineKeyboardButton", "WithPay") },
        { SwitchQueryAttribute, AccessExpression("InlineKeyboardButton", "WithSwitchInlineQuery") },
        { QueryChosenAttribute, AccessExpression("InlineKeyboardButton", "WithSwitchInlineQueryChosenChat") },
        { QueryCurrentAttribute, AccessExpression("InlineKeyboardButton", "WithSwitchInlineQueryCurrentChat") },
        { UrlRedirectAttribute, AccessExpression("InlineKeyboardButton", "WithUrl") },
        { WebAppAttribute, AccessExpression("InlineKeyboardButton", "WithWebApp") },
    };

    private static readonly Dictionary<string, MemberAccessExpressionSyntax> ReplyKeyboardLayout = new Dictionary<string, MemberAccessExpressionSyntax>()
    {
        { RequestChatAttribute, AccessExpression("KeyboardButton", "WithRequestChat") },
        { RequestContactAttribute, AccessExpression("KeyboardButton", "WithRequestContact") },
        { RequestLocationAttribute, AccessExpression("KeyboardButton", "WithRequestLocation") },
        { RequestPoolAttribute, AccessExpression("KeyboardButton", "WithRequestPoll") },
        { RequestUsersAttribute, AccessExpression("KeyboardButton", "WithRequestUsers") },
        { WebAppAttribute, AccessExpression("KeyboardButton", "WithWebApp") }
    };

    // Markup map
    private static readonly Dictionary<string, Dictionary<string, MemberAccessExpressionSyntax>> LayoutNames = new Dictionary<string, Dictionary<string, MemberAccessExpressionSyntax>>()
    {
        { InlineReturnType, InlineKeyboardLayout },
        { ReplyReturnType, ReplyKeyboardLayout }
    };

    // Diagnostic descriptors
    private static readonly DiagnosticDescriptor WrongReturnType = new DiagnosticDescriptor("TLG201", "Wrong return type", string.Empty, "Telegrator.Modelling", DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor UnsupportedAttribute = new DiagnosticDescriptor("TLG202", "Unsupported or invalid attribute", string.Empty, "Telegrator.Modelling", DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor NotPartialMethod = new DiagnosticDescriptor("TLG203", "Not a partial member", string.Empty, "Telegrator.Modelling", DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor UseBodylessMethod = new DiagnosticDescriptor("TLG204", "Use bodyless method", string.Empty, "Telegrator.Modelling", DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor UseParametrlessMethod = new DiagnosticDescriptor("TLG205", "Use parametrless method", string.Empty, "Telegrator.Modelling", DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor UseGetOnlyProperty = new DiagnosticDescriptor("TLG206", "Use property with only get accessor", string.Empty, "Telegrator.Modelling", DiagnosticSeverity.Error, true);
    private static readonly DiagnosticDescriptor UseBodylessGetAccessor = new DiagnosticDescriptor("TLG207", "Use bodyless get accessor", string.Empty, "Telegrator.Modelling", DiagnosticSeverity.Error, true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<MethodDeclarationSyntax>> methodsPipeline = context.SyntaxProvider.CreateSyntaxProvider(ProvideMethods, TransformMethods).Where(x => x != null).Collect();
        IncrementalValueProvider<ImmutableArray<PropertyDeclarationSyntax>> propertiesPipeline = context.SyntaxProvider.CreateSyntaxProvider(ProvideProperties, TransformProperties).Where(x => x != null).Collect();

        context.RegisterSourceOutput(methodsPipeline, ExecuteMethodsPipeline);
        context.RegisterSourceOutput(propertiesPipeline, ExecutePropertiesPipeline);
    }

    private static bool ProvideMethods(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (syntaxNode is not MethodDeclarationSyntax method)
            return false;

        if (!HasGenAttributes(method))
            return false;

        return true;
    }

    private static bool ProvideProperties(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (syntaxNode is not PropertyDeclarationSyntax property)
            return false;

        if (!HasGenAttributes(property))
            return false;

        return true;
    }

    private static MethodDeclarationSyntax TransformMethods(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return (MethodDeclarationSyntax)context.Node;
    }

    private static PropertyDeclarationSyntax TransformProperties(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return (PropertyDeclarationSyntax)context.Node;
    }

    private static void ExecutePropertiesPipeline(SourceProductionContext context, ImmutableArray<PropertyDeclarationSyntax> properties)
    {
        List<GeneratedMarkupPropertyModel> models = new List<GeneratedMarkupPropertyModel>();

        foreach (PropertyDeclarationSyntax prop in properties)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            try
            {
                string returnType = prop.Type.ToString();
                bool anyErrors = false;

                Dictionary<string, MemberAccessExpressionSyntax> layout;
                if (!LayoutNames.TryGetValue(returnType, out layout!))
                {
                    context.ReportDiagnostic(Diagnostic.Create(WrongReturnType, prop.Type.GetLocation()));
                    anyErrors = true;
                }

                if (!prop.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    context.ReportDiagnostic(Diagnostic.Create(NotPartialMethod, prop.Identifier.GetLocation()));
                    anyErrors = true;
                }

                if (prop.Initializer != null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(UseGetOnlyProperty, prop.Initializer.GetLocation()));
                    anyErrors = true;
                }

                if (prop.ExpressionBody != null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(UseGetOnlyProperty, prop.ExpressionBody.GetLocation()));
                    anyErrors = true;
                }

                if (prop.AccessorList != null)
                {
                    foreach (AccessorDeclarationSyntax accessor in prop.AccessorList.Accessors)
                    {
                        if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(UseGetOnlyProperty, accessor.GetLocation()));
                            anyErrors = true;
                            continue;
                        }

                        if (accessor.Body != null)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(UseBodylessGetAccessor, accessor.Body.GetLocation()));
                            anyErrors = true;
                            continue;
                        }

                        if (accessor.ExpressionBody != null)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(UseBodylessGetAccessor, accessor.ExpressionBody.GetLocation()));
                            anyErrors = true;
                            continue;
                        }
                    }
                }

                if (anyErrors || layout == null)
                    continue;

                SeparatedSyntaxList<CollectionElementSyntax> matrix = ParseAttributesMatrix(context, layout, prop);
                PropertyDeclarationSyntax genProp = GeneratedPropertyDeclaration(prop, SyntaxFactory.CollectionExpression(matrix));
                models.Add(new GeneratedMarkupPropertyModel(prop, genProp));
            }
            catch (Exception ex)
            {
                context.AddSource($"{prop.Identifier}_Error.g.cs", SourceText.From($"/* {ex} */", Encoding.UTF8));
            }
        }

        if (models.Count == 0)
            return;

        CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit();
        SyntaxList<UsingDirectiveSyntax> usingDirectives = SyntaxFactory.List(ParseUsings(DefaultUsings));

        foreach (GeneratedMarkupPropertyModel model in models)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            try
            {
                MemberDeclarationSyntax wrappedMember = WrapInParentDeclarations(model.OriginalProperty, new List<MemberDeclarationSyntax> { model.GeneratedProperty });
                compilationUnit = compilationUnit.AddMembers(wrappedMember);
            }
            catch (Exception ex)
            {
                context.AddSource($"{model.OriginalProperty.Identifier}_GenError.g.cs", SourceText.From($"/* {ex} */", Encoding.UTF8));
            }
        }

        compilationUnit = compilationUnit.WithUsings(usingDirectives).NormalizeWhitespace();
        context.AddSource("GeneratedKeyboards.Properties.g.cs", SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
    }

    private static void ExecuteMethodsPipeline(SourceProductionContext context, ImmutableArray<MethodDeclarationSyntax> methods)
    {
        List<GeneratedMarkupMethodModel> models = new List<GeneratedMarkupMethodModel>();

        foreach (MethodDeclarationSyntax method in methods)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            try
            {
                string methodName = method.Identifier.Text;
                string returnType = method.ReturnType.ToString();
                bool anyErrors = false;

                Dictionary<string, MemberAccessExpressionSyntax> layout;
                if (!LayoutNames.TryGetValue(returnType, out layout!))
                {
                    context.ReportDiagnostic(Diagnostic.Create(WrongReturnType, method.ReturnType.GetLocation()));
                    anyErrors = true;
                }

                if (!method.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    context.ReportDiagnostic(Diagnostic.Create(NotPartialMethod, method.Identifier.GetLocation()));
                    anyErrors = true;
                }

                if (method.ParameterList.Parameters.Count > 0)
                {
                    context.ReportDiagnostic(Diagnostic.Create(UseParametrlessMethod, method.ParameterList.GetLocation()));
                    anyErrors = true;
                }

                if (method.ExpressionBody != null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(UseBodylessMethod, method.ExpressionBody.GetLocation()));
                    anyErrors = true;
                }

                if (method.Body != null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(UseBodylessMethod, method.Body.GetLocation()));
                    anyErrors = true;
                }

                if (anyErrors || layout == null)
                    continue;

                SeparatedSyntaxList<CollectionElementSyntax> matrix = ParseAttributesMatrix(context, layout, method);
                FieldDeclarationSyntax genField = GeneratedFieldDeclaration(methodName, method.ReturnType, SyntaxFactory.CollectionExpression(matrix));
                MethodDeclarationSyntax genMethod = GeneratedMethodDeclaration(methodName, method.Modifiers, method.ReturnType, genField);
                models.Add(new GeneratedMarkupMethodModel(method, genField, genMethod));
            }
            catch (Exception ex)
            {
                context.AddSource($"{method.Identifier}_Error.g.cs", SourceText.From($"/* {ex} */", Encoding.UTF8));
            }
        }

        if (models.Count == 0)
            return;

        CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit();
        SyntaxList<UsingDirectiveSyntax> usingDirectives = SyntaxFactory.List(ParseUsings(DefaultUsings));

        foreach (GeneratedMarkupMethodModel model in models)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            try
            {
                MemberDeclarationSyntax wrappedMembers = WrapInParentDeclarations(model.OriginalMethod, new List<MemberDeclarationSyntax> { model.GeneratedField, model.GeneratedMethod });
                compilationUnit = compilationUnit.AddMembers(wrappedMembers);
            }
            catch (Exception ex)
            {
                context.AddSource($"{model.OriginalMethod.Identifier}_GenError.g.cs", SourceText.From($"/* {ex} */", Encoding.UTF8));
            }
        }

        compilationUnit = compilationUnit.WithUsings(usingDirectives).NormalizeWhitespace();
        context.AddSource("GeneratedKeyboards.Methods.g.cs", SourceText.From(compilationUnit.ToFullString(), Encoding.UTF8));
    }

    private static SeparatedSyntaxList<CollectionElementSyntax> ParseAttributesMatrix(SourceProductionContext context, Dictionary<string, MemberAccessExpressionSyntax> layout, MemberDeclarationSyntax member)
    {
        SeparatedSyntaxList<CollectionElementSyntax> vertical = new SeparatedSyntaxList<CollectionElementSyntax>();

        foreach (AttributeListSyntax attributeList in member.AttributeLists)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            SeparatedSyntaxList<CollectionElementSyntax> horizontal = new SeparatedSyntaxList<CollectionElementSyntax>();

            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                MemberAccessExpressionSyntax accessSyntax;
                if (!layout.TryGetValue(attribute.Name.ToString(), out accessSyntax!))
                {
                    context.ReportDiagnostic(Diagnostic.Create(UnsupportedAttribute, attribute.Name.GetLocation()));
                    continue;
                }

                InvocationExpressionSyntax expression = SyntaxFactory.InvocationExpression(accessSyntax, ConvertArguments(attribute.ArgumentList));
                horizontal = horizontal.Add(SyntaxFactory.ExpressionElement(expression));
            }

            ExpressionElementSyntax element = SyntaxFactory.ExpressionElement(SyntaxFactory.CollectionExpression(horizontal));
            vertical = vertical.Add(element);
        }

        return vertical;
    }

    private static PropertyDeclarationSyntax GeneratedPropertyDeclaration(PropertyDeclarationSyntax property, CollectionExpressionSyntax collection)
    {
        return SyntaxFactory.PropertyDeclaration(property.Type, property.Identifier)
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(collection))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }

    private static MethodDeclarationSyntax GeneratedMethodDeclaration(string identifier, SyntaxTokenList modifiers, TypeSyntax returnType, FieldDeclarationSyntax field)
    {
        VariableDeclaratorSyntax targetVariable = field.Declaration.Variables.First();

        return SyntaxFactory.MethodDeclaration(returnType, identifier)
            .WithModifiers(modifiers)
            .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.IdentifierName(targetVariable.Identifier)))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }

    private static FieldDeclarationSyntax GeneratedFieldDeclaration(string identifier, TypeSyntax returnType, CollectionExpressionSyntax collection)
    {
        ArgumentSyntax argument = SyntaxFactory.Argument(collection);
        ArgumentListSyntax arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(argument));
        ObjectCreationExpressionSyntax objectCreation = SyntaxFactory.ObjectCreationExpression(returnType, arguments, null);

        VariableDeclaratorSyntax declarator = SyntaxFactory.VariableDeclarator(identifier + "_generatedMarkup")
            .WithInitializer(SyntaxFactory.EqualsValueClause(objectCreation));

        SyntaxTokenList fieldModifiers = SyntaxFactory.TokenList(
            SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
            SyntaxFactory.Token(SyntaxKind.StaticKeyword),
            SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));

        return SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(returnType).AddVariables(declarator))
            .WithModifiers(fieldModifiers);
    }

    private static ArgumentListSyntax ConvertArguments(AttributeArgumentListSyntax? attributeArgs)
    {
        if (attributeArgs == null)
            return SyntaxFactory.ArgumentList();

        IEnumerable<ArgumentSyntax> arguments = attributeArgs.Arguments.Select(CastArgument);
        return SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments));
    }

    private static ArgumentSyntax CastArgument(AttributeArgumentSyntax argument)
    {
        if (argument.NameColon != null)
        {
            return SyntaxFactory.Argument(argument.Expression).WithNameColon(argument.NameColon);
        }

        return SyntaxFactory.Argument(argument.Expression);
    }

    private static MemberDeclarationSyntax WrapInParentDeclarations(MemberDeclarationSyntax originalMember, List<MemberDeclarationSyntax> generatedMembers)
    {
        SyntaxNode? parentNode = originalMember.Parent;

        if (parentNode is not ClassDeclarationSyntax)
        {
            throw new InvalidOperationException("Generated member must be contained within a class.");
        }

        MemberDeclarationSyntax currentDeclaration = SyntaxFactory.ClassDeclaration(((ClassDeclarationSyntax)parentNode).Identifier)
            .WithMembers(SyntaxFactory.List(generatedMembers))
            .WithModifiers(((ClassDeclarationSyntax)parentNode).Modifiers);

        if (!currentDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            currentDeclaration = ((ClassDeclarationSyntax)currentDeclaration).AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        }

        parentNode = parentNode.Parent;

        while (parentNode is TypeDeclarationSyntax typeDeclaration)
        {
            ClassDeclarationSyntax wrappingClass = SyntaxFactory.ClassDeclaration(typeDeclaration.Identifier)
                .WithMembers(SyntaxFactory.SingletonList(currentDeclaration))
                .WithModifiers(typeDeclaration.Modifiers);

            if (!wrappingClass.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                wrappingClass = wrappingClass.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            }

            currentDeclaration = wrappingClass;
            parentNode = parentNode.Parent;
        }

        if (parentNode is BaseNamespaceDeclarationSyntax namespaceDeclaration)
        {
            currentDeclaration = SyntaxFactory.NamespaceDeclaration(namespaceDeclaration.Name)
                .WithMembers(SyntaxFactory.SingletonList(currentDeclaration));
        }

        return currentDeclaration;
    }

    private static IEnumerable<UsingDirectiveSyntax> ParseUsings(params string[] names)
    {
        return names.Select(name => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(name)));
    }

    private static bool HasGenAttributes(MemberDeclarationSyntax member)
    {
        IEnumerable<string> memberAttributes = member.AttributeLists
            .SelectMany(x => x.Attributes)
            .Select(x => x.Name.ToString());

        IEnumerable<string> targetAttributes = InlineAttributes.Concat(ReplyAttributes);

        return memberAttributes.Intersect(targetAttributes).Any();
    }

    private static MemberAccessExpressionSyntax AccessExpression(string className, string methodName)
    {
        return SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName(className),
            SyntaxFactory.IdentifierName(methodName));
    }
}