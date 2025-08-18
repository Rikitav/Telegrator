﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;
using Telegrator.RoslynExtensions;
using Telegrator.RoslynGenerators.RoslynExtensions;

#if DEBUG
using System.Diagnostics;
#endif

namespace Telegrator.RoslynGenerators
{
    [Generator(LanguageNames.CSharp)]
    public class ImplicitHandlerBuilderExtensionsGenerator : IIncrementalGenerator
    {
        private static readonly string[] DefaultUsings =
        [
            "Telegrator.Handlers.Building",
            "Telegrator.Handlers.Building.Components"
        ];
         
        private static readonly ParameterSyntax ExtensionMethodThisParam = SyntaxFactory.Parameter(SyntaxFactory.Identifier("builder")).WithType(SyntaxFactory.IdentifierName("TBuilder").WithLeadingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ")).WithTrailingTrivia(WhitespaceTrivia)).WithModifiers([SyntaxFactory.Token(SyntaxKind.ThisKeyword)]);
        private static readonly MemberAccessExpressionSyntax BuilderAdderMethodAccessExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("builder"), SyntaxFactory.IdentifierName("AddTargetedFilters"));
        private static readonly IEqualityComparer<UsingDirectiveSyntax> UsingEqualityComparer = new UsingDirectiveEqualityComparer();

        private static SyntaxTrivia TabulationTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, "\t");
        private static SyntaxTrivia WhitespaceTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ");
        private static SyntaxTrivia NewLineTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\n");

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
#if DEBUG
            Debugger.Launch();
#endif
            IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> pipeline = context.SyntaxProvider
                .CreateSyntaxProvider(SyntaxPredicate, SyntaxTransform)
                .Where(declaration => declaration != null)
                .Collect();

            context.RegisterImplementationSourceOutput(pipeline, GenerateSource);
        }

        private static bool SyntaxPredicate(SyntaxNode node, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return node is ClassDeclarationSyntax;
        }

        private static ClassDeclarationSyntax SyntaxTransform(GeneratorSyntaxContext context, CancellationToken _)
        {
            ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
            if (symbol is null)
                return null!;

            if (symbol is not ITypeSymbol typeSymbol)
                return null!;

            if (!typeSymbol.IsAssignableFrom("UpdateFilterAttribute"))
                return null!;

            return (ClassDeclarationSyntax)context.Node;
        }

        private static void GenerateSource(SourceProductionContext context, ImmutableArray<ClassDeclarationSyntax> declarations)
        {
            StringBuilder debugExport = new StringBuilder("/*");
            List<UsingDirectiveSyntax> usings = ParseUsings(DefaultUsings).ToList();

            /*
            Dictionary<string, string> targeters = [];
            List<string> usingDirectives =
            [
                "using Telegrator.Handlers.Building;",
                "using Telegrator.Handlers.Building.Components;"
            ];
            */

            /*
            StringBuilder sourceBuilder = new StringBuilder()
                .AppendLine("namespace Telegrator")
                .AppendLine("{")
                .Append("\t//").Append(string.Join(", ", declarations.Select(decl => decl.Identifier.ToString()))).AppendLine()
                .AppendLine("\tpublic static partial class HandlerBuilderExtensions")
                .AppendLine("\t{");
            */

            Dictionary<string, MethodDeclarationSyntax> targetters = [];
            foreach (ClassDeclarationSyntax classDeclaration in declarations)
            {
                try
                {
                    string className = classDeclaration.Identifier.ToString();
                    if (className == "FilterAnnotation")
                        continue;

                    MethodDeclarationSyntax? targeter = classDeclaration.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(IsTargeterMethod);
                    if (targeter != null)
                    {
                        try
                        {
                            MethodDeclarationSyntax genTargeter = GenerateTargetterMethod(classDeclaration, targeter);
                            targetters.Add(className, genTargeter);
                        }
                        catch (Exception exc)
                        {
                            string errorFormat = string.Format("\nFailed to generate for {0} : {1}\n", classDeclaration.Identifier.ToString(), exc.ToString());
                            debugExport.AppendLine(errorFormat);
                        }
                    }
                }
                catch (Exception exc)
                {
                    string errorFormat = string.Format("\nFailed to generate for {0} : {1}\n", classDeclaration.Identifier.ToString(), exc.ToString());
                    debugExport.AppendLine(errorFormat);
                }
            }
            
            List<MethodDeclarationSyntax> extensions = [];
            foreach (ClassDeclarationSyntax classDeclaration in declarations)
            {
                if (classDeclaration.Modifiers.HasModifiers("abstract"))
                    continue;

                usings.UnionAdd(classDeclaration.FindAncestor<CompilationUnitSyntax>().Usings, UsingEqualityComparer);
                MethodDeclarationSyntax targeter = FindTargetterMethod(targetters, classDeclaration);

                if (classDeclaration.ParameterList != null && classDeclaration.BaseList != null)
                {
                    try
                    {
                        PrimaryConstructorBaseTypeSyntax primaryConstructor = (PrimaryConstructorBaseTypeSyntax)classDeclaration.BaseList.Types.ElementAt(0);
                        MethodDeclarationSyntax genExtension = GeneratedExtensionsMethod(classDeclaration, classDeclaration.ParameterList, primaryConstructor.ArgumentList, targeter);
                        extensions.Add(genExtension);
                    }
                    catch (Exception exc)
                    {
                        string errorFormat = string.Format("\nFailed to generate for {0} : {1}\n", classDeclaration.Identifier.ToString(), exc.ToString());
                        debugExport.AppendLine(errorFormat);
                    }
                }

                foreach (ConstructorDeclarationSyntax ctor in GetConstructors(classDeclaration))
                {
                    try
                    {
                        if (ctor.Initializer == null)
                            continue;

                        MethodDeclarationSyntax genExtension = GeneratedExtensionsMethod(classDeclaration, ctor.ParameterList, ctor.Initializer.ArgumentList, targeter);
                        extensions.Add(genExtension);
                    }
                    catch (Exception exc)
                    {
                        string errorFormat = string.Format("\nFailed to generate for {0} : {1}\n", classDeclaration.Identifier.ToString(), exc.ToString());
                        debugExport.AppendLine(errorFormat);
                    }
                }
            }

            try
            {
                ClassDeclarationSyntax extensionsClass = SyntaxFactory.ClassDeclaration("HandlerBuilderExtensions")
                    .WithModifiers(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword))
                    .AddMembers([.. targetters.Values, .. extensions])
                    .DecorateType(1);

                NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Telegrator"))
                    .WithMembers([extensionsClass])
                    .Decorate();

                CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit()
                    .WithUsings([.. usings])
                    .WithMembers([namespaceDeclaration]);

                context.AddSource("GeneratedHandlerBuilderExtensions.cs", compilationUnit.ToFullString());
            }
            catch (Exception exc)
            {
                string errorFormat = string.Format("\nFailed to generate : {0}\n", exc.ToString());
                debugExport.AppendLine(errorFormat);
            }

            context.AddSource("GeneratedHandlerBuilderExtensions.Debug.cs", debugExport.AppendLine("*/").ToString());
        }

        private static MethodDeclarationSyntax GenerateTargetterMethod(ClassDeclarationSyntax classDeclaration, MethodDeclarationSyntax targetterMethod)
        {
            SyntaxToken identifier = SyntaxFactory.Identifier(classDeclaration.Identifier.ToString() + "_" + targetterMethod.Identifier.ToString());
            MethodDeclarationSyntax method = SyntaxFactory.MethodDeclaration(targetterMethod.ReturnType, identifier)
                .WithParameterList(targetterMethod.ParameterList)
                .WithModifiers(Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword));

            if (targetterMethod.Body != null)
                method = method.WithBody(targetterMethod.Body);

            if (targetterMethod.ExpressionBody != null)
                method = method.WithExpressionBody(targetterMethod.ExpressionBody).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return method.DecorateMember(2);
        }

        private static MethodDeclarationSyntax GeneratedExtensionsMethod(ClassDeclarationSyntax classDeclaration, ParameterListSyntax methodParameters, ArgumentListSyntax invokerArguments, MethodDeclarationSyntax targetterMethod)
        {
            ParameterListSyntax parameters = SyntaxFactory.ParameterList([ExtensionMethodThisParam, ..methodParameters.Parameters]);
            TypeParameterListSyntax typeParameters = SyntaxFactory.TypeParameterList([SyntaxFactory.TypeParameter("TBuilder")]);

            InvocationExpressionSyntax invocationExpression = SyntaxFactory.InvocationExpression(BuilderAdderMethodAccessExpression, AddTargeter(invokerArguments, targetterMethod));
            BlockSyntax body = SyntaxFactory.Block(new StatementSyntax[]
            {
                SyntaxFactory.ExpressionStatement(invocationExpression),
                SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("builder").WithLeadingTrivia(WhitespaceTrivia))
            });

            TypeParameterConstraintClauseSyntax typeParameterConstraint = SyntaxFactory.TypeParameterConstraintClause(SyntaxFactory.IdentifierName("TBuilder").WithLeadingTrivia(WhitespaceTrivia).WithTrailingTrivia(WhitespaceTrivia))
                .WithConstraints([SyntaxFactory.TypeConstraint(SyntaxFactory.ParseTypeName("IHandlerBuilder").WithLeadingTrivia(WhitespaceTrivia))])
                .WithLeadingTrivia(WhitespaceTrivia);

            string filterName = classDeclaration.Identifier.ToString().Replace("Attribute", string.Empty);
            if (filterName == "ChatType")
                filterName = "InChatType"; // Because it conflicting

            SyntaxToken identifier = SyntaxFactory.Identifier(filterName);
            TypeSyntax returnType = SyntaxFactory.ParseTypeName("TBuilder").WithTrailingTrivia(WhitespaceTrivia);
            SyntaxTriviaList xmlDoc = BuildExtensionXmlDocTrivia(classDeclaration, methodParameters);

            MethodDeclarationSyntax method = SyntaxFactory.MethodDeclaration(returnType, identifier)
                .WithParameterList(parameters)
                .WithBody(body.DecorateBlock(2))
                .WithTypeParameterList(typeParameters)
                .WithModifiers(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword))
                .WithConstraintClauses([typeParameterConstraint])
                .DecorateMember(2)
                .WithLeadingTrivia(xmlDoc);
             
            return method;
        }

        private static SyntaxTokenList Modifiers(params SyntaxKind[] kinds)
            => new SyntaxTokenList(kinds.Select(SyntaxFactory.Token).Select(mod => mod.WithTrailingTrivia(WhitespaceTrivia)));

        private static IEnumerable<UsingDirectiveSyntax> ParseUsings(params string[] names) => names
            .Select(name => SyntaxFactory.IdentifierName(name).WithLeadingTrivia(WhitespaceTrivia))
            .Select(name => SyntaxFactory.UsingDirective(name).WithTrailingTrivia(NewLineTrivia));

        private static ArgumentListSyntax AddTargeter(ArgumentListSyntax invokerArguments, MethodDeclarationSyntax targetterMethod)
            => SyntaxFactory.ArgumentList([SyntaxFactory.Argument(SyntaxFactory.IdentifierName(targetterMethod.Identifier)), ..invokerArguments.Arguments]);

        private static bool IsTargeterMethod(MethodDeclarationSyntax method)
            => method.Identifier.ToString() == "GetFilterringTarget";

        private static IEnumerable<ConstructorDeclarationSyntax> GetConstructors(ClassDeclarationSyntax classDeclaration)
            => classDeclaration.Members.OfType<ConstructorDeclarationSyntax>().Where(ctor => ctor.Modifiers.HasModifiers("public"));

        private static MethodDeclarationSyntax FindTargetterMethod(Dictionary<string, MethodDeclarationSyntax> targeters, ClassDeclarationSyntax classDeclaration)
        {
            if (targeters.TryGetValue(classDeclaration.Identifier.ValueText, out MethodDeclarationSyntax targeter))
                return targeter;

            if (classDeclaration.BaseList != null && targeters.TryGetValue(classDeclaration.BaseList.Types.ElementAt(0).Type.ToString(), out targeter))
                return targeter;

            throw new TargteterNotFoundException();
        }

        private static SyntaxTriviaList BuildExtensionXmlDocTrivia(ClassDeclarationSyntax classDeclaration, ParameterListSyntax methodParameters)
        {
            StringBuilder summaryBuilder = new StringBuilder();

            summaryBuilder
                .Append("\t\t/// <summary>\n")
                .Append("\t\t/// Adds a ").Append(classDeclaration.Identifier.ToString()).Append(" target filter to the handler builder.\n")
                .Append("\t\t/// </summary>\n");

            summaryBuilder
                .AppendLine("\t\t/// <typeparam name=\"TBuilder\">The builder type.</typeparam>")
                .AppendLine("\t\t/// <param name=\"builder\">The handler builder.</param>");

            foreach (ParameterSyntax param in methodParameters.Parameters)
            {
                string name = param.Identifier.ToString();
                summaryBuilder
                    .Append("\t\t/// <param name=\"").Append(name).Append("\">")
                    .Append("The ").Append(name)
                    .AppendLine(".</param>");
            }

            summaryBuilder.AppendLine("\t\t/// <returns>The same builder instance.</returns>");
            summaryBuilder.Append("\t\t");
            return SyntaxFactory.ParseLeadingTrivia(summaryBuilder.ToString());
        }

        private class UsingDirectiveEqualityComparer : IEqualityComparer<UsingDirectiveSyntax>
        {
            public bool Equals(UsingDirectiveSyntax x, UsingDirectiveSyntax y)
            {
                return x.ToString() == y.ToString();
            }

            public int GetHashCode(UsingDirectiveSyntax obj)
            {
                return obj.GetHashCode();
            }
        }

        /*
        private static void ParseClassDeclaration(StringBuilder sourceBuilder, ClassDeclarationSyntax classDeclaration, Dictionary<string, string> targeters)
        {
            string className = classDeclaration.Identifier.ToString();
            if (className == "FilterAnnotation")
                return;

            IEnumerable<MethodDeclarationSyntax> methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();
            MethodDeclarationSyntax? targeterMethod = methods.FirstOrDefault(method => method.Identifier.ToString() == "GetFilterringTarget");

            string filterName = className.Replace("Attribute", string.Empty);
            string classTargetterMethodName = filterName + "_GetFilterringTarget";

            if (targeterMethod != null)
            {
                targeters.Add(className, classTargetterMethodName);
                RenderTargeterMethod(sourceBuilder, classTargetterMethodName, targeterMethod);
                sourceBuilder.AppendLine();
            }
            else
            {
                if (classDeclaration.BaseList == null)
                    throw new Exception();

                string baseClassName = classDeclaration.BaseList.Types
                    .ElementAt(0).GetBaseTypeSyntaxName();

                if (!targeters.ContainsKey(baseClassName))
                    throw new TargteterNotFoundException();

                classTargetterMethodName = targeters[baseClassName];
            }

            if (classDeclaration.Modifiers.HasModifiers("abstract"))
                return;

            if (classDeclaration.ParameterList != null)
            {
                if (classDeclaration.BaseList != null)
                {
                    PrimaryConstructorBaseTypeSyntax primaryConstructor = (PrimaryConstructorBaseTypeSyntax)classDeclaration.BaseList.Types.ElementAt(0);
                    RenderExtensionMethod(sourceBuilder, filterName, classTargetterMethodName, classDeclaration.ParameterList.Parameters, primaryConstructor.ArgumentList.Arguments);
                }
                else
                {
                    RenderExtensionMethod(sourceBuilder, filterName, classTargetterMethodName, classDeclaration.ParameterList.Parameters, []);
                }

                sourceBuilder.AppendLine();
            }

            foreach (ConstructorDeclarationSyntax constructor in classDeclaration.Members.OfType<ConstructorDeclarationSyntax>())
            {
                if (constructor.Initializer == null)
                    continue;

                RenderExtensionMethod(sourceBuilder, filterName, classTargetterMethodName, constructor.ParameterList.Parameters, constructor.Initializer.ArgumentList.Arguments);
                sourceBuilder.AppendLine();
            }
        }
        */

        /*
        private static void RenderExtensionMethod(StringBuilder sourceBuilder, string filterName, string classTargetterMethodName, SeparatedSyntaxList<ParameterSyntax> parameters, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            if (filterName == "ChatType")
                filterName = "InChatType"; // Because it conflicting

            sourceBuilder
                .Append("\t\t/// <summary>").AppendLine()
                .Append("\t\t/// Adds ").Append(filterName).Append(" filter to implicit handler").AppendLine()
                .Append("\t\t/// </summary>").AppendLine();

            sourceBuilder.Append("\t\tpublic static TBuilder ").Append(filterName).Append("<TBuilder>(this TBuilder builder");

            if (parameters.Any())
                sourceBuilder.Append(", ").Append(parameters.ToFullString());

            sourceBuilder
                .Append(") where TBuilder : IHandlerBuilder").AppendLine()
                .Append("\t\t{").AppendLine()
                .Append("\t\t\tbuilder.AddTargetedFilter");

            if (arguments.Count > 1)
                sourceBuilder.Append("s");

            sourceBuilder.Append("(").Append(classTargetterMethodName);

            if (arguments.Any())
                sourceBuilder.Append(", ").Append(arguments.ToFullString());

            sourceBuilder
                .Append(");").AppendLine()
                .Append("\t\t\treturn builder;").AppendLine()
                .Append("\t\t}").AppendLine();
        }

        private static void RenderTargeterMethod(StringBuilder sourceBuilder, string classTargetterMethodName, MethodDeclarationSyntax targeterMethod)
        {
            sourceBuilder.Append("\t\tprivate static ").Append(targeterMethod.ReturnType.ToString()).Append(" ").Append(classTargetterMethodName).Append(targeterMethod.ParameterList.ToFullString());

            if (targeterMethod.ExpressionBody != null)
            {
                sourceBuilder.Append(targeterMethod.ExpressionBody.ToFullString()).Append(";").AppendLine();
            }
            else if (targeterMethod.Body != null)
            {
                sourceBuilder.Append(targeterMethod.Body.ToFullString());
            }
        }
        */
    }
}
