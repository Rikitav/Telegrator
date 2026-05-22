#!/bin/bash
mkdir -p src/Telegrator/Handlers

function generate_handler() {
    local NAME=$1
    local TYPE=$2
    local UTYPE=$3
    local EXTRA_USING=$4
    local ATTR_CHECK=$5
    local FILE="src/Telegrator/Handlers/${NAME}.cs"

    local USING="using Telegram.Bot.Types;\nusing Telegram.Bot.Types.Enums;\nusing Telegrator.Attributes;\nusing Telegrator.Core.Filters;\nusing Telegrator.Core.Handlers;"
    if [ ! -z "$EXTRA_USING" ]; then
        USING="${USING}\nusing ${EXTRA_USING};"
    fi

    echo -e "${USING}\n\nnamespace Telegrator.Handlers;\n\n/// <summary>\n/// Attribute that marks a handler to process ${NAME} updates.\n/// </summary>\npublic class ${NAME}Attribute(int importance = 0) : UpdateHandlerAttribute<${NAME}>(UpdateType.${UTYPE}, importance)\n{\n    /// <inheritdoc/>\n    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { ${ATTR_CHECK}: { } };\n}\n\n/// <summary>\n/// Abstract base class for handlers that process ${NAME} updates.\n/// </summary>\npublic abstract class ${NAME}() : AbstractUpdateHandler<${TYPE}>(UpdateType.${UTYPE})\n{\n}\n" > "$FILE"
}

generate_handler "EditedMessageHandler" "Message" "EditedMessage" "" "EditedMessage"
generate_handler "ChannelPostHandler" "Message" "ChannelPost" "" "ChannelPost"
generate_handler "EditedChannelPostHandler" "Message" "EditedChannelPost" "" "EditedChannelPost"
generate_handler "BusinessMessageHandler" "Message" "BusinessMessage" "" "BusinessMessage"
generate_handler "EditedBusinessMessageHandler" "Message" "EditedBusinessMessage" "" "EditedBusinessMessage"
generate_handler "BusinessConnectionHandler" "BusinessConnection" "BusinessConnection" "" "BusinessConnection"
generate_handler "DeletedBusinessMessagesHandler" "DeletedBusinessMessages" "DeletedBusinessMessages" "" "DeletedBusinessMessages"
generate_handler "MessageReactionHandler" "MessageReactionUpdated" "MessageReaction" "" "MessageReaction"
generate_handler "MessageReactionCountHandler" "MessageReactionCountUpdated" "MessageReactionCount" "" "MessageReactionCount"
generate_handler "ShippingQueryHandler" "ShippingQuery" "ShippingQuery" "Telegram.Bot.Types.Payments" "ShippingQuery"
generate_handler "PreCheckoutQueryHandler" "PreCheckoutQuery" "PreCheckoutQuery" "Telegram.Bot.Types.Payments" "PreCheckoutQuery"
generate_handler "PurchasedPaidMediaHandler" "PaidMediaPurchased" "PurchasedPaidMedia" "Telegram.Bot.Types.Payments" "PurchasedPaidMedia"
generate_handler "PollHandler" "Poll" "Poll" "" "Poll"
generate_handler "PollAnswerHandler" "PollAnswer" "PollAnswer" "" "PollAnswer"
generate_handler "MyChatMemberHandler" "ChatMemberUpdated" "MyChatMember" "" "MyChatMember"
generate_handler "ChatMemberHandler" "ChatMemberUpdated" "ChatMember" "" "ChatMember"
generate_handler "ChatJoinRequestHandler" "ChatJoinRequest" "ChatJoinRequest" "" "ChatJoinRequest"
generate_handler "ChatBoostHandler" "ChatBoostUpdated" "ChatBoost" "" "ChatBoost"
generate_handler "RemovedChatBoostHandler" "ChatBoostRemoved" "RemovedChatBoost" "" "RemovedChatBoost"

