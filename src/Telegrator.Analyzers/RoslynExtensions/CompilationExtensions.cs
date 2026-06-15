using Microsoft.CodeAnalysis;

namespace Telegrator.Analyzers.RoslynExtensions;

internal static class CompilationExtensions
{
    public static IEnumerable<INamedTypeSymbol> GetAllDefinedTypes(this IAssemblySymbol assembly)
    {
        return GetAllDefinedTypes(assembly.GlobalNamespace);
    }

    public static IEnumerable<INamedTypeSymbol> GetAllTypesFromCompilation(this Compilation compilation)
    {
        foreach (IAssemblySymbol assembly in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            foreach (INamedTypeSymbol type in assembly.GetAllDefinedTypes())
                yield return type;
        }

        foreach (INamedTypeSymbol type in compilation.Assembly.GetAllDefinedTypes())
            yield return type;
    }

    private static IEnumerable<INamedTypeSymbol> GetAllDefinedTypes(INamespaceSymbol namespaceSymbol)
    {
        foreach (INamedTypeSymbol type in namespaceSymbol.GetTypeMembers())
        {
            yield return type;
            foreach (INamedTypeSymbol nested in GetNestedTypes(type))
                yield return nested;
        }

        foreach (INamespaceSymbol nestedNamespace in namespaceSymbol.GetNamespaceMembers())
            foreach (INamedTypeSymbol type in GetAllDefinedTypes(nestedNamespace))
                yield return type;
    }

    private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
    {
        foreach (INamedTypeSymbol nested in type.GetTypeMembers())
        {
            yield return nested;
            foreach (INamedTypeSymbol recursive in GetNestedTypes(nested))
                yield return recursive;
        }
    }
}
