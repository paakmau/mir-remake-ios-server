
using LiteNetLib.Utils;

namespace MirRemake {
    enum NetworkReceiveDataType : byte {
        SET_SELF_NETWORK_ID,
        SET_SELF_INIT_INFO,
        SET_OTHER_ACTOR_UNIT_IN_SIGHT,
        SET_OTHER_POSITION,
        SET_ALL_HP_AND_MP,
        APPLY_OTHER_FSM_STATE,
        APPLY_ALL_EFFECT,
        APPLY_ALL_DEAD
    }
    enum NetworkSendDataType : byte {
        SEND_PLAYER_ID,
        SEND_POSITION,
        APPLY_ACTIVE_ENTER_FSM_STATE,
        APPLY_CAST_SKILL,
        APPLY_USE_CONSUMABLE_ITEM,
        OPEN_MISSION_DIALOG,
        ACCEPT_MISSION,
        DELIVERING_MISSION,
        CANCEL_MISSION,
        REQUEST_SHOP_NPC_FUNCTION,
        BUY_ITEMS,
        SOLD_ITEMS,
    }
}