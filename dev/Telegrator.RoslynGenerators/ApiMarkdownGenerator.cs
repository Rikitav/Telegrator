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

#if DEBUG
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;
using Telegrator.RoslynGenerators.RoslynExtensions;

namespace Telegrator.RoslynGenerators
{
    /// <summary>
    /// Source Generator РґР»СЏ Р°РІС‚РѕРјР°С‚РёС‡РµСЃРєРѕР№ РіРµРЅРµСЂР°С†РёРё Markdown-РґРѕРєСѓРјРµРЅС‚Р°С†РёРё РїРѕ РїСѓР±Р»РёС‡РЅРѕРјСѓ API Telegrator.
    /// </summary>
    [Generator]
    public class ApiMarkdownGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValueProvider<ImmutableArray<BaseTypeDeclarationSyntax>> typeDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (node, _) => node is ClassDeclarationSyntax || node is InterfaceDeclarationSyntax || node is StructDeclarationSyntax || node is EnumDeclarationSyntax,
                    transform: (ctx, _) => (BaseTypeDeclarationSyntax)ctx.Node
                )
                .Where(typeDecl => typeDecl != null)
                .Collect();

            IncrementalValueProvider<(ImmutableArray<BaseTypeDeclarationSyntax> Left, Compilation Right)> combined = typeDeclarations.Combine(context.CompilationProvider);

            context.RegisterSourceOutput(combined, (spc, source) =>
            {
                IReadOnlyList<BaseTypeDeclarationSyntax> typeDecls = source.Left;
                Compilation compilation = source.Right;
                string markdown = GenerateMarkdown(typeDecls, compilation);
                spc.AddSource("TelegramReactive_Api.md", markdown);
            });
        }

        private string GenerateMarkdown(IReadOnlyList<BaseTypeDeclarationSyntax> typeDecls, Compilation compilation)
        {
            StringBuilder sourceBuilder = new StringBuilder("/*\n");

            // Writing caution message
            sourceBuilder.AppendLine("> [!CAUTION]");
            sourceBuilder.AppendLine("> This page was generated using hand-writen compiler's XML-Summaries output parser");
            sourceBuilder.AppendLine("> If you find any missing syntax, errors in structure, mis-formats or empty sections, please contact owner or open an issue\n");

            // Collecting only public types
            IEnumerable<INamedTypeSymbol> publicTypes = typeDecls
                .Select(type => type.TryGetNamedType(compilation))
                .Where(symbol => symbol != null)
                .Where(symbol => symbol.DeclaredAccessibility == Accessibility.Public);

            // Grouping by namespace
            IOrderedEnumerable<IGrouping<string, INamedTypeSymbol>> namespaces = publicTypes
                .GroupBy(t => t.ContainingNamespace.ToDisplayString())
                .OrderBy(g => g.Key);

            foreach (IGrouping<string, INamedTypeSymbol> nsGroup in namespaces)
            {
                string ns = nsGroup.Key == "Telegrator" ? nsGroup.Key : nsGroup.Key.Substring("Telegrator.".Length);
                sourceBuilder.AppendFormat("# {0}\n\n", ns);

                foreach (INamedTypeSymbol type in nsGroup.OrderBy(t => t.Name))
                {
                    // Р¤РѕСЂРјРёСЂСѓРµРј generic-РїР°СЂР°РјРµС‚СЂС‹ РґР»СЏ Р·Р°РіРѕР»РѕРІРєР° РєР»Р°СЃСЃР°
                    string genericArgs = type.FormatGenericTypes();
                    sourceBuilder.AppendFormat("## {0} `{1}{2}`\n\n", type.TypeKind, type.Name, genericArgs);

                    string? typeSummary = type.ExtractSummary();
                    if (!string.IsNullOrWhiteSpace(typeSummary))
                        sourceBuilder.AppendFormat("> {0}\n\n", typeSummary);

                    // Writing fields
                    if (type.TypeKind == TypeKind.Enum)
                    {
                        WriteEnumValues(sourceBuilder, type);
                    }
                    else
                    {
                        WriteConstructors(sourceBuilder, type);
                        WriteProperties(sourceBuilder, type);
                        WriteMethods(sourceBuilder, type);
                    }
                }
            }

            return sourceBuilder.AppendLine().AppendLine("*/").ToString();
        }

        private static void WriteConstructors(StringBuilder sourceBuilder, INamedTypeSymbol type)
        {
            if (type.TypeKind == TypeKind.Enum)
                return;
            // Getting ctors
            List<IMethodSymbol> ctors = type.Constructors
                .Where(c => c.DeclaredAccessibility == Accessibility.Public)
                .ToList();

            // Checking for any
            if (ctors.Count == 0)
                return;

            // Writing
            sourceBuilder.AppendLine("**Constructors:**");
            foreach (IMethodSymbol ctor in ctors)
            {
                // Formatting constructor signature
                string genericArgs = type.FormatGenericTypes();
                string parameters = string.Join(", ", ctor.Parameters.Select(p => p.Type.GetShortName()));
                string signature = string.Format("{0}{1}({2})", type.Name, genericArgs, parameters);
                sourceBuilder.Append(" - `").Append(signature).Append("`").AppendLine();

                // Writing summary
                string? propSummary = ctor.ExtractSummary();
                if (!string.IsNullOrWhiteSpace(propSummary))
                    sourceBuilder.Append("   > ").Append(propSummary);

                sourceBuilder.AppendLine();
            }

            sourceBuilder.AppendLine();
        }

        private static void WriteEnumValues(StringBuilder sourceBuilder, INamedTypeSymbol type)
        {
            // Getting enum values
            List<IFieldSymbol> fields = type
                .GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => f.HasConstantValue && f.DeclaredAccessibility == Accessibility.Public)
                .ToList();

            // Checking for any
            if (fields.Count == 0)
                return;

            // Writing
            sourceBuilder.AppendLine("**Values:**");
            foreach (IFieldSymbol field in fields)
            {
                // Writing value
                sourceBuilder.Append("- `").Append(field.Name).Append("`");

                // Writing summary
                string? summary = field.ExtractSummary();
                if (!string.IsNullOrWhiteSpace(summary))
                    sourceBuilder.Append(" вЂ” ").Append(summary);

                sourceBuilder.AppendLine();
            }

            sourceBuilder.AppendLine();
        }

        private static void WriteProperties(StringBuilder sourceBuilder, INamedTypeSymbol type)
        {
            // Getting properties
            List<IPropertySymbol> props = type
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.DeclaredAccessibility == Accessibility.Public)
                .ToList();

            // Checking for any
            if (props.Count == 0)
                return;

            // Writing
            sourceBuilder.AppendLine("**Properties:**");
            foreach (IPropertySymbol prop in props)
            {
                // Writing member
                sourceBuilder.AppendFormat("- `{0}`\n", prop.Name);

                // Writing summary
                string? propSummary = prop.ExtractSummary();
                if (!string.IsNullOrWhiteSpace(propSummary))
                    sourceBuilder.AppendFormat("   > {0}", propSummary);

                sourceBuilder.AppendLine();
            }

            sourceBuilder.AppendLine();
        }

        private static void WriteMethods(StringBuilder sourceBuilder, INamedTypeSymbol type)
        {
            // Getting methods
            List<IMethodSymbol> methods = type
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public)
                .Where(m => m.MethodKind == MethodKind.Ordinary)
                .ToList();

            // Checking for any
            if (methods.Count == 0)
                return;

            // Writing
            sourceBuilder.AppendLine("**Methods:**");
            foreach (IMethodSymbol method in methods)
            {
                // Formating method signature
                string genericArgs = method.FormatGenericTypes();
                string parameters = string.Join(", ", method.Parameters.Select(p => p.Type.GetShortName()).Where(p => !string.IsNullOrEmpty(p)));
                sourceBuilder.AppendFormat(" - `{0}{1}({2})`\n", method.Name, genericArgs, parameters);

                // Writing summary
                string? propSummary = method.ExtractSummary();
                if (!string.IsNullOrWhiteSpace(propSummary))
                    sourceBuilder.AppendFormat("   > {0}", propSummary);

                sourceBuilder.AppendLine();
            }

            sourceBuilder.AppendLine();
        }
    }

    internal static partial class TypesExtensions
    {
        public static string? ExtractSummary(this ISymbol symbol)
        {
            string? xmlDoc = symbol.GetDocumentationCommentXml();
            if (string.IsNullOrWhiteSpace(xmlDoc))
                return null;

            try
            {
                XDocument doc = XDocument.Parse(xmlDoc);
                XElement? xSummary = doc.Root?.Element("summary");

                if (xSummary == null)
                    return null;

                // РЈР±РёСЂР°РµРј Р»РёС€РЅРёРµ РїСЂРѕР±РµР»С‹ Рё РїРµСЂРµРЅРѕСЃС‹ СЃС‚СЂРѕРє
                string summary = xSummary.Value.Trim().Replace("\n", " ");
                while (summary.Contains("  "))
                    summary = summary.Replace("  ", " ");

                return summary;
            }
            catch
            {
                return null;
            }
        }

        public static string FormatGenericTypes(this INamedTypeSymbol methodSymbol)
        {
            if (methodSymbol.TypeParameters.Length == 0)
                return string.Empty;

            string typeParams = string.Join(", ", methodSymbol.TypeParameters.Select(tp => tp.Name));
            return string.Format("<{0}>", typeParams);
        }

        public static string FormatGenericTypes(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol.TypeParameters.Length == 0)
                return string.Empty;

            string typeParams = string.Join(", ", methodSymbol.TypeParameters.Select(tp => tp.Name));
            return string.Format("<{0}>", typeParams);
        }

        public static string GetShortName(this ITypeSymbol type)
        {
            if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
            {
                string genericArgs = string.Join(", ", namedType.TypeArguments.Select(GetShortName));
                return string.Format("{0}<{1}>", namedType.Name, genericArgs);
            }

            if (type.TypeKind == TypeKind.TypeParameter)
            {
                return type.Name;
            }

            return type.Name;
        }
    }
}
#endif
