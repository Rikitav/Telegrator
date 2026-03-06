using Microsoft.CodeAnalysis;

namespace Telegrator.Analyzers.RoslynExtensions
{
    public static class SyntaxTokenExtensions
    {
        public static bool HasModifiers(this SyntaxTokenList modifiers, params string[] expected)
        {
            return modifiers.Count(mod => expected.Contains(mod.ToString())) == expected.Length;
        }
    }
}
