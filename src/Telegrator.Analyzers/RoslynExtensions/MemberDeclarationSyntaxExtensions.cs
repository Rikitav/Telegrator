/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
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
    public static class MemberDeclarationSyntaxExtensions
    {
        private static SyntaxTrivia TabulationTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, "\t");
        private static SyntaxTrivia WhitespaceTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ");
        private static SyntaxTrivia NewLineTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\n");

        public static SyntaxTokenList Decorate(this SyntaxTokenList tokens)
            => new SyntaxTokenList(tokens.Select(token => token.WithoutTrivia().WithTrailingTrivia(WhitespaceTrivia)).ToArray());

        public static BlockSyntax DecorateBlock(this BlockSyntax block, int times) => block
            .WithStatements([.. block.Statements.Select(statement => statement.DecorateStatememnt(times + 1))])
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithLeadingTrivia(TabulationTrivia.Repeat(times)).WithTrailingTrivia(NewLineTrivia))
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken).WithLeadingTrivia(TabulationTrivia.Repeat(times)).WithTrailingTrivia(NewLineTrivia));

        public static T DecorateStatememnt<T>(this T statememnt, int times) where T : StatementSyntax => statememnt
            .WithoutTrivia().WithLeadingTrivia(TabulationTrivia.Repeat(times)).WithTrailingTrivia(NewLineTrivia);

        public static T DecorateMember<T>(this T typeDeclaration, int times) where T : MemberDeclarationSyntax => typeDeclaration
            .WithoutTrivia().WithLeadingTrivia(TabulationTrivia.Repeat(times)).WithTrailingTrivia(NewLineTrivia);

        public static NamespaceDeclarationSyntax Decorate(this NamespaceDeclarationSyntax namespaceDeclaration) => namespaceDeclaration
            .WithName(namespaceDeclaration.Name.WithoutTrivia().WithLeadingTrivia(WhitespaceTrivia))
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithLeadingTrivia(NewLineTrivia).WithTrailingTrivia(NewLineTrivia))
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        public static T DecorateType<T>(this T typeDeclaration, int times = 1) where T : TypeDeclarationSyntax => (T)typeDeclaration
            .WithoutTrivia().WithLeadingTrivia(TabulationTrivia.Repeat(times))
            .WithIdentifier(typeDeclaration.Identifier.WithoutTrivia().WithLeadingTrivia(WhitespaceTrivia).WithTrailingTrivia(NewLineTrivia))
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithLeadingTrivia(TabulationTrivia.Repeat(times)).WithTrailingTrivia(NewLineTrivia))
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken).WithLeadingTrivia(TabulationTrivia.Repeat(times)).WithTrailingTrivia(NewLineTrivia));
    }
}
