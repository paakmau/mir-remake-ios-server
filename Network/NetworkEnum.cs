
namespace MirRemake {
    enum NetworkToClientDataType : byte {
        INIT_SELF_NETWORK_ID,
        INIT_SELF_INFO,
        APPLY_OTHER_ACTOR_UNIT_IN_SIGHT,
        SET_OTHER_POSITION,
        SET_ALL_HP_AND_MP,
        APPLY_OTHER_CAST_SKILL_BEGIN,
        APPLY_OTHER_CAST_SKILL_SING_CANCEL,
        APPLY_ALL_EFFECT,
        APPLY_ALL_DEAD
    }
    enum NetworkToServerDataType : byte {
        INIT_PLAYER_ID,
        SET_POSITION,
        APPLY_CAST_SKILL_BEGIN,
        APPLY_CAST_SKILL_SETTLE,
        APPLY_CAST_SKILL_SING_CANCEL,
        APPLY_ACCEPT_MISSION,
        APPLY_FINISH_MISSION,
        APPLY_CANCEL_MISSION
    }
}