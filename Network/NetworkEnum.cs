
using LiteNetLib.Utils;

namespace MirRemake {
    enum NetworkReceiveDataType : byte {
        SET_SELF_NETWORK_ID,
        SET_OTHER_ACTOR_UNIT_IN_SIGHT,
        SET_OTHER_POSITION,
        SET_ALL_HP_AND_MP,
        SET_ALL_FSM_STATE,
        APPLY_ALL_STATUS,
        APPLY_ALL_EFFECT
    }
    enum NetworkSendDataType : byte {
        SEND_PLAYER_ID,
        SEND_POSITION,
        APPLY_ACTIVE_ENTER_FSM_STATE,
        APPLY_CAST_SKILL,
        APPLY_USE_CONSUMABLE_ITEM
    }
}