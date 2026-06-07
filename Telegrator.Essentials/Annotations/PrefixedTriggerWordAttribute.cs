using Telegram.Bot.Types;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Annotations;

public class PrefixedTriggerWordAttribute : FilterAnnotation<Message>
{
    public string[] Prefixes { get; set; } = ["/"];

    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        if (context.Input.Text is { Length: > 0 })
        {
            string text = context.Input.Text;
            return Prefixes.Any(p => text.StartsWith(p));
        }

        return false;
    }
}
