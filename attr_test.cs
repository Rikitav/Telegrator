using System.Linq;
using Microsoft.CodeAnalysis;
public static class Test {
    public static string Render(AttributeData attr) {
        var args = string.Join(", ", attr.ConstructorArguments.Select(a => a.ToCSharpString()));
        var named = string.Join(", ", attr.NamedArguments.Select(n => $"{n.Key} = {n.Value.ToCSharpString()}"));
        var allArgs = args;
        if (!string.IsNullOrEmpty(named)) allArgs = string.IsNullOrEmpty(args) ? named : args + ", " + named;
        return $"new {attr.AttributeClass!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}({args})" + (string.IsNullOrEmpty(named) ? "" : $" {{ {named} }}");
    }
}
