using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Globalization;
using System.Xml;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Telegrator.Analyzers.RoslynExtensions;

internal static class AdaptiveGeneratorHelpers
{
    internal static SyntaxTriviaList ExtractXmlDocumentation(ISymbol symbol)
    {
        string? xml = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrWhiteSpace(xml))
            return default;

        // Minimal extraction of <summary> and <param> elements for generated code.
        XmlDocument doc = new XmlDocument();
        try
        {
            doc.LoadXml($"<root>{xml}</root>");
        }
        catch
        {
            return default;
        }

        List<string> lines = new List<string>();
        XmlNode? summaryNode = doc.SelectSingleNode("//summary");
        if (summaryNode is not null)
        {
            string summaryText = System.Text.RegularExpressions.Regex.Replace(summaryNode.InnerXml, "\\s+", " ").Trim();
            if (!string.IsNullOrEmpty(summaryText))
                lines.Add($"/// <summary>{summaryText}</summary>");
        }

        XmlNodeList? paramNodes = doc.SelectNodes("//param");
        if (paramNodes is not null)
        {
            foreach (XmlNode paramNode in paramNodes.OfType<XmlNode>())
            {
                string? paramName = paramNode.Attributes?["name"]?.Value;
                if (string.IsNullOrEmpty(paramName))
                    continue;

                string paramText = System.Text.RegularExpressions.Regex.Replace(paramNode.InnerXml, "\\s+", " ").Trim();
                lines.Add($"/// <param name=\"{paramName}\">{paramText}</param>");
            }
        }

        if (lines.Count == 0)
            return default;

        string triviaText = string.Join("\n", lines) + "\n";
        return ParseLeadingTrivia(triviaText);
    }

    internal static string[]? PayloadNameToUpdateType(string payloadName)
    {
        return payloadName switch
        {
            "Update" => ["global::Telegram.Bot.Types.Enums.UpdateType.Unknown"],

            "Message" => [
                "global::Telegram.Bot.Types.Enums.UpdateType.Message",
                "global::Telegram.Bot.Types.Enums.UpdateType.EditedMessage",
                "global::Telegram.Bot.Types.Enums.UpdateType.ChannelPost",
                "global::Telegram.Bot.Types.Enums.UpdateType.EditedChannelPost",
                "global::Telegram.Bot.Types.Enums.UpdateType.BusinessMessage",
                "global::Telegram.Bot.Types.Enums.UpdateType.EditedBusinessMessage",
                "global::Telegram.Bot.Types.Enums.UpdateType.GuestMessage"
            ],

            "ChatMemberUpdated" => [
                "global::Telegram.Bot.Types.Enums.UpdateType.MyChatMember",
                "global::Telegram.Bot.Types.Enums.UpdateType.ChatMember"
            ],

            "BusinessConnection" => ["global::Telegram.Bot.Types.Enums.UpdateType.BusinessConnection"],
            "BusinessMessagesDeleted" => ["global::Telegram.Bot.Types.Enums.UpdateType.DeletedBusinessMessages"],
            "CallbackQuery" => ["global::Telegram.Bot.Types.Enums.UpdateType.CallbackQuery"],
            "InlineQuery" => ["global::Telegram.Bot.Types.Enums.UpdateType.InlineQuery"],
            "ChosenInlineResult" => ["global::Telegram.Bot.Types.Enums.UpdateType.ChosenInlineResult"],
            "ShippingQuery" => ["global::Telegram.Bot.Types.Enums.UpdateType.ShippingQuery"],
            "PreCheckoutQuery" => ["global::Telegram.Bot.Types.Enums.UpdateType.PreCheckoutQuery"],
            "Poll" => ["global::Telegram.Bot.Types.Enums.UpdateType.Poll"],
            "PollAnswer" => ["global::Telegram.Bot.Types.Enums.UpdateType.PollAnswer"],
            "ChatJoinRequest" => ["global::Telegram.Bot.Types.Enums.UpdateType.ChatJoinRequest"],
            "MessageReactionUpdated" => ["global::Telegram.Bot.Types.Enums.UpdateType.MessageReaction"],
            "MessageReactionCountUpdated" => ["global::Telegram.Bot.Types.Enums.UpdateType.MessageReactionCount"],
            "ChatBoostUpdated" => ["global::Telegram.Bot.Types.Enums.UpdateType.ChatBoost"],
            "ChatBoostRemoved" => ["global::Telegram.Bot.Types.Enums.UpdateType.RemovedChatBoost"],
            "PaidMediaPurchased" => ["global::Telegram.Bot.Types.Enums.UpdateType.PurchasedPaidMedia"],
            "ManagedBotUpdated" => ["global::Telegram.Bot.Types.Enums.UpdateType.ManagedBot"],

            _ => null
        };
    }

    internal static string? UpdateTypeToPayloadName(string updateTypeName)
    {
        return updateTypeName switch
        {
            "Unknown" => "global::Telegram.Bot.Types.Update",

            "Message" => "global::Telegram.Bot.Types.Message",
            "EditedMessage" => "global::Telegram.Bot.Types.Message",
            "ChannelPost" => "global::Telegram.Bot.Types.Message",
            "EditedChannelPost" => "global::Telegram.Bot.Types.Message",
            "BusinessMessage" => "global::Telegram.Bot.Types.Message",
            "EditedBusinessMessage" => "global::Telegram.Bot.Types.Message",
            "GuestMessage" => "global::Telegram.Bot.Types.Message",

            "MyChatMember" => "global::Telegram.Bot.Types.ChatMemberUpdated",
            "ChatMember" => "global::Telegram.Bot.Types.ChatMemberUpdated",

            "BusinessConnection" => "global::Telegram.Bot.Types.BusinessConnection",
            "DeletedBusinessMessages" => "global::Telegram.Bot.Types.BusinessMessagesDeleted",
            "CallbackQuery" => "global::Telegram.Bot.Types.CallbackQuery",
            "InlineQuery" => "global::Telegram.Bot.Types.InlineQuery",
            "ChosenInlineResult" => "global::Telegram.Bot.Types.ChosenInlineResult",
            "ShippingQuery" => "global::Telegram.Bot.Types.Payments.ShippingQuery",
            "PreCheckoutQuery" => "global::Telegram.Bot.Types.Payments.PreCheckoutQuery",
            "Poll" => "global::Telegram.Bot.Types.Poll",
            "PollAnswer" => "global::Telegram.Bot.Types.PollAnswer",
            "ChatJoinRequest" => "global::Telegram.Bot.Types.ChatJoinRequest",
            "MessageReaction" => "global::Telegram.Bot.Types.MessageReactionUpdated",
            "MessageReactionCount" => "global::Telegram.Bot.Types.MessageReactionCountUpdated",
            "ChatBoost" => "global::Telegram.Bot.Types.ChatBoostUpdated",
            "RemovedChatBoost" => "global::Telegram.Bot.Types.ChatBoostRemoved",
            "PurchasedPaidMedia" => "global::Telegram.Bot.Types.Payments.PaidMediaPurchased",
            "ManagedBot" => "global::Telegram.Bot.Types.ManagedBotUpdated",

            _ => null
        };
    }

    internal static string ToPascalCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        name = name.TrimStart('_');
        if (name.Length == 0)
            return name;
        if (char.IsUpper(name[0]))
            return name;

        return char.ToUpperInvariant(name[0]) + name.Substring(1);
    }

    internal static ParameterSyntax CreateParameterSyntax(IParameterSymbol parameter)
    {
        ParameterSyntax result = Parameter(Identifier(parameter.Name))
            .WithType(ParseTypeName(parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

        if (parameter.IsParams)
            result = result.AddModifiers(Token(SyntaxKind.ParamsKeyword));

        ExpressionSyntax? defaultValue = GetParameterDefaultExpression(parameter);
        if (defaultValue is not null)
            result = result.WithDefault(EqualsValueClause(defaultValue));

        return result;
    }

    internal static ExpressionSyntax? GetParameterDefaultExpression(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        object? value = parameter.ExplicitDefaultValue;
        ITypeSymbol type = parameter.Type;

        if (value is null)
            return LiteralExpression(SyntaxKind.NullLiteralExpression);

        if (type.TypeKind == TypeKind.Enum)
        {
            string typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return ParseExpression($"({typeName}){Convert.ToInt64(value, CultureInfo.InvariantCulture)}");
        }

        return type.SpecialType switch
        {
            SpecialType.System_Boolean => LiteralExpression((bool)value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
            SpecialType.System_String => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal((string)value)),
            SpecialType.System_Char => LiteralExpression(SyntaxKind.CharacterLiteralExpression, Literal((char)value)),
            SpecialType.System_Int32 => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)value)),
            SpecialType.System_Int64 => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((long)value)),
            SpecialType.System_UInt32 => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((uint)value)),
            SpecialType.System_UInt64 => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((ulong)value)),
            SpecialType.System_Single => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((float)value)),
            SpecialType.System_Double => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((double)value)),
            SpecialType.System_Decimal => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((decimal)value)),
            SpecialType.System_Byte => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((byte)value)),
            SpecialType.System_SByte => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((sbyte)value)),
            SpecialType.System_Int16 => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((short)value)),
            SpecialType.System_UInt16 => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((ushort)value)),
            _ => DefaultExpression(ParseTypeName(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
        };
    }
}
