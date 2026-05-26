#!/bin/bash
mkdir -p src/Telegrator/Annotations

function generate_filter() {
    local NAME=$1
    local TARGET_TYPE=$2
    local UTYPE=$3
    local ATTR_CHECK=$4
    local EXTRA_USING=$5
    local FILE="src/Telegrator/Annotations/${NAME}FilterAttributes.cs"

    local USING="using Telegram.Bot.Types;\nusing Telegram.Bot.Types.Enums;\nusing Telegrator.Attributes;\nusing Telegrator.Core.Filters;\nusing Telegrator.Filters;"
    if [ ! -z "$EXTRA_USING" ]; then
        USING="${USING}\nusing ${EXTRA_USING};"
    fi

    echo -e "${USING}\n\nnamespace Telegrator.Annotations;\n\n/// <summary>\n/// Abstract base attribute for filtering ${NAME} updates.\n/// </summary>\npublic abstract class ${NAME}FilterAttribute(params IFilter<${TARGET_TYPE}>[] filters) : UpdateFilterAttribute<${TARGET_TYPE}>(filters)\n{\n    /// <inheritdoc/>\n    public override UpdateType[] AllowedTypes => [UpdateType.${UTYPE}];\n\n    /// <inheritdoc/>\n    public override ${TARGET_TYPE}? GetFilterringTarget(Update update)\n        => update.${ATTR_CHECK};\n}\n" > "$FILE"
}

generate_filter "EditedMessage" "Message" "EditedMessage" "EditedMessage" ""
generate_filter "ChannelPost" "Message" "ChannelPost" "ChannelPost" ""
generate_filter "EditedChannelPost" "Message" "EditedChannelPost" "EditedChannelPost" ""
generate_filter "BusinessMessage" "Message" "BusinessMessage" "BusinessMessage" ""
generate_filter "EditedBusinessMessage" "Message" "EditedBusinessMessage" "EditedBusinessMessage" ""
generate_filter "BusinessConnection" "BusinessConnection" "BusinessConnection" "BusinessConnection" ""
generate_filter "DeletedBusinessMessages" "DeletedBusinessMessages" "DeletedBusinessMessages" "DeletedBusinessMessages" ""
generate_filter "MessageReaction" "MessageReactionUpdated" "MessageReaction" "MessageReaction" ""
generate_filter "MessageReactionCount" "MessageReactionCountUpdated" "MessageReactionCount" "MessageReactionCount" ""
generate_filter "InlineQuery" "InlineQuery" "InlineQuery" "InlineQuery" ""
generate_filter "ChosenInlineResult" "ChosenInlineResult" "ChosenInlineResult" "ChosenInlineResult" ""
generate_filter "ShippingQuery" "ShippingQuery" "ShippingQuery" "ShippingQuery" "Telegram.Bot.Types.Payments"
generate_filter "PreCheckoutQuery" "PreCheckoutQuery" "PreCheckoutQuery" "PreCheckoutQuery" "Telegram.Bot.Types.Payments"
generate_filter "PurchasedPaidMedia" "PaidMediaPurchased" "PurchasedPaidMedia" "PurchasedPaidMedia" "Telegram.Bot.Types.Payments"
generate_filter "Poll" "Poll" "Poll" "Poll" ""
generate_filter "PollAnswer" "PollAnswer" "PollAnswer" "PollAnswer" ""
generate_filter "MyChatMember" "ChatMemberUpdated" "MyChatMember" "MyChatMember" ""
generate_filter "ChatMember" "ChatMemberUpdated" "ChatMember" "ChatMember" ""
generate_filter "ChatJoinRequest" "ChatJoinRequest" "ChatJoinRequest" "ChatJoinRequest" ""
generate_filter "ChatBoost" "ChatBoostUpdated" "ChatBoost" "ChatBoost" ""
generate_filter "RemovedChatBoost" "ChatBoostRemoved" "RemovedChatBoost" "RemovedChatBoost" ""

