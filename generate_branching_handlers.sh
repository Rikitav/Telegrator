#!/bin/bash

function add_branching() {
    local NAME=$1
    local TARGET_TYPE=$2
    local UTYPE=$3
    local ATTR_CHECK=$4
    local FILE="src/Telegrator/Handlers/${NAME}Handler.cs"

    if grep -q "Branching${NAME}Handler" "$FILE"; then
        return
    fi

    echo -e "\n/// <summary>\n/// Abstract base class for branching handlers that process ${NAME}Handler updates.\n/// </summary>\npublic abstract class Branching${NAME}Handler() : BranchingUpdateHandler<${TARGET_TYPE}>(UpdateType.${UTYPE})\n{\n}\n" >> "$FILE"
}

add_branching "EditedMessage" "Message" "EditedMessage" "EditedMessage"
add_branching "ChannelPost" "Message" "ChannelPost" "ChannelPost"
add_branching "EditedChannelPost" "Message" "EditedChannelPost" "EditedChannelPost"
add_branching "BusinessMessage" "Message" "BusinessMessage" "BusinessMessage"
add_branching "EditedBusinessMessage" "Message" "EditedBusinessMessage" "EditedBusinessMessage"
add_branching "BusinessConnection" "BusinessConnection" "BusinessConnection" "BusinessConnection"
add_branching "BusinessMessagesDeleted" "BusinessMessagesDeleted" "DeletedBusinessMessages" "DeletedBusinessMessages"
add_branching "MessageReaction" "MessageReactionUpdated" "MessageReaction" "MessageReaction"
add_branching "MessageReactionCount" "MessageReactionCountUpdated" "MessageReactionCount" "MessageReactionCount"
add_branching "ShippingQuery" "ShippingQuery" "ShippingQuery" "ShippingQuery"
add_branching "PreCheckoutQuery" "PreCheckoutQuery" "PreCheckoutQuery" "PreCheckoutQuery"
add_branching "PurchasedPaidMedia" "PaidMediaPurchased" "PurchasedPaidMedia" "PurchasedPaidMedia"
add_branching "Poll" "Poll" "Poll" "Poll"
add_branching "PollAnswer" "PollAnswer" "PollAnswer" "PollAnswer"
add_branching "MyChatMember" "ChatMemberUpdated" "MyChatMember" "MyChatMember"
add_branching "ChatMember" "ChatMemberUpdated" "ChatMember" "ChatMember"
add_branching "ChatJoinRequest" "ChatJoinRequest" "ChatJoinRequest" "ChatJoinRequest"
add_branching "ChatBoost" "ChatBoostUpdated" "ChatBoost" "ChatBoost"
add_branching "RemovedChatBoost" "ChatBoostRemoved" "RemovedChatBoost" "RemovedChatBoost"
add_branching "AnyUpdate" "Update" "Unknown" ""
add_branching "CallbackQuery" "CallbackQuery" "CallbackQuery" "CallbackQuery"
add_branching "InlineQuery" "Update" "InlineQuery" "InlineQuery"

