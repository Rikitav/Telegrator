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
