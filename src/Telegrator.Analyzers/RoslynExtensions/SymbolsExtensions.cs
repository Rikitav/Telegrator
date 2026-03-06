using Microsoft.CodeAnalysis;

namespace Telegrator.Analyzers.RoslynExtensions;

public static class SymbolsExtensions
{
    public static bool IsAssignableFrom(this ITypeSymbol symbol, string className)
    {
        if (symbol.BaseType == null)
            return false;

        if (symbol.BaseType.Name == className)
            return true;

        return symbol.BaseType.IsAssignableFrom(className);
    }

    public static ITypeSymbol? Cast(this ITypeSymbol symbol, string className)
    {
        if (symbol.BaseType == null)
            return null;

        if (symbol.BaseType.Name == className)
            return symbol.BaseType;

        return symbol.BaseType.Cast(className);
    }
}
