using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Telegrator.Analyzers.RoslynExtensions
{
    public static class SyntaxNodesExtensions
    {
        public static T FindAncestor<T>(this SyntaxNode node) where T : SyntaxNode
        {
            if (node.Parent == null)
                throw new AncestorNotFoundException();

            if (node.Parent is T found)
                return found;

            return node.Parent.FindAncestor<T>();
        }

        public static bool TryFindAncestor<T>(this SyntaxNode node, out T syntax) where T : SyntaxNode
        {
            if (node.Parent == null)
            {
                syntax = null!;
                return false;
            }

            if (node.Parent is T found)
            {
                syntax = found;
                return true;
            }

            return node.Parent.TryFindAncestor(out syntax);
        }

        public static INamedTypeSymbol TryGetNamedType(this BaseTypeDeclarationSyntax syntax, Compilation compilation)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            return semanticModel.GetDeclaredSymbol(syntax)!;
        }

        public static string GetBaseTypeSyntaxName(this BaseTypeSyntax baseClassSyntax)
        {
            if (baseClassSyntax is PrimaryConstructorBaseTypeSyntax parimaryConstructor)
                return parimaryConstructor.Type.ToString();

            if (baseClassSyntax is SimpleBaseTypeSyntax simpleBaseType)
                return simpleBaseType.Type.ToString();

            throw new BaseClassTypeNotFoundException();
        }

        public static int CountParentTree(this SyntaxNode node)
        {
            int count = 0;
            SyntaxNode inspectNode = node;

            while (inspectNode.Parent != null)
            {
                inspectNode = inspectNode.Parent;
                count++;
            }

            return count;
        }

        public static SeparatedSyntaxList<TNode> ToSeparatedSyntaxList<TNode>(this IEnumerable<TNode> elements) where TNode : SyntaxNode
            => new SeparatedSyntaxList<TNode>().AddRange(elements);

        public static SyntaxList<TNode> ToSyntaxList<TNode>(this IEnumerable<TNode> elements) where TNode : SyntaxNode
            => new SyntaxList<TNode>().AddRange(elements);
    }
}
