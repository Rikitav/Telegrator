using System.Text;

namespace Telegrator.Analyzers.RoslynExtensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendTabs(this StringBuilder builder, int count)
            => builder.Append(new string('\t', count));
    }
}
