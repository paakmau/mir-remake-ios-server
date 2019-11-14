namespace MirRemakeBackend.Network {
    enum NetworkToClientDataType : byte {
        // 未进入游戏
        INIT_SELF_NETWORK_ID,
        INIT_SELF_REGISTER,
        INIT_SELF_LOGIN,
        INIT_SELF_MODIFY_PASSWORD,
        INIT_SELF_GET_PASSWORD_PROTECT_PROBLEM,
        INIT_SELF_FIND_PASSWORD,
        INIT_SELF_CREATE_CHARACTER,
        INIT_SELF_ATTRIBUTE,
        INIT_SELF_SKILL,
        INIT_SELF_MISSION,
        INIT_SELF_TITLE_MISSION,
        INIT_SELF_ITEM,
        INIT_SELF_SHORTCUT,
        // 视野相关
        APPLY_OTHER_MONSTER_IN_SIGHT,
        APPLY_OTHER_CHARACTER_IN_SIGHT,
        APPLY_OTHER_ACTOR_UNIT_OUT_OF_SIGHT,
        // 位置 装备外观 属性
        SET_OTHER_POSITION,
        APPLY_ALL_CHANGE_EQUIPMENT,
        SET_ALL_HP_AND_MP,
        APPLY_SHOW_ALL_CHARACTER_ATTRIBUTE,
        APPLY_SELF_SPECIAL_ATTRIBUTE,
        APPLY_SELF_LEVEL_AND_EXP,
        APPLY_SELF_MAIN_ATTRIBUTE,
        APPLY_ALL_RESPAWN,
        SET_ALL_BOSS_DAMAGE_CHARACTER_RANK,
        // 战力排行榜
        APPLY_SHOW_FIGHT_CAPACITY_RANK,
        // 战斗状态与动画
        APPLY_ALL_CAST_SKILL_BEGIN,
        APPLY_ALL_EFFECT,
        APPLY_ALL_STATUS,
        APPLY_ALL_DEAD,
        // 持有的物品 货币
        APPLY_SELF_UPDATE_ITEM,
        APPLY_SELF_UPDATE_EQUIPMENT,
        APPLY_SELF_UPDATE_ENCHANTMENT,
        APPLY_SELF_CURRENCY,
        // 地面物品
        APPLY_GROUND_ITEM_SHOW,
        APPLY_GROUND_ITEM_DISAPPEAR,
        // 摊位
        APPLY_SELF_SET_UP_MARKET,
        APPLY_SELF_PACK_UP_MARKET,
        APPLY_SELF_UPDATE_MARKET_ITEM,
        APPLY_OTHER_SET_UP_MARKET,
        APPLY_OTHER_PACK_UP_MARKET,
        APPLY_SELF_ENTER_OTHER_MARKET,
        APPLY_OTHER_UPDATE_MARKET_EQUIPMENT,
        APPLY_OTHER_UPDATE_MARKET_ENCHANTMENT,
        APPLY_OTHER_UPDATE_MARKET_ITEM,
        // 任务相关
        APPLY_SELF_ACCECPT_MISSION,
        APPLY_SELF_DELIVER_MISSION,
        APPLY_SELF_CANCEL_MISSION,
        APPLY_SELF_MISSION_PROGRESS,
        APPLY_SELF_MISSION_UNLOCK,
        APPLY_SELF_MISSION_ACCEPTABLE,
        // 称号
        APPLY_SELF_TITLE_MISSION_PROGRESS,
        APPLY_ALL_ATTACH_TITLE,
        APPLY_ALL_DETACH_TITLE,
        // 技能相关
        APPLY_SELF_UPDATE_SKILL_LEVEL_AND_MASTERLY,
        // 商城
        APPLY_SELF_SHOW_MALL,
        // 信息
        APPLY_ALL_RECEIVE_MESSAGE,
        // 邮箱
        APPLY_SELF_SHOW_MAIL_BOX,
        APPLY_SELF_READ_MAIL,
        APPLY_SELF_RECEIVE_MAIL,
        // 公告
        APPLY_SHOW_NOTICE,
        // 控制台
        CONSOLE_SUCCESS,
        CONSOLE_FAIL
    }
    enum NetworkToServerDataType : byte {
        // 未进入游戏阶段
        INIT_REGISTER,
        INIT_LOGIN,
        INIT_MODIFY_PASSWORD,
        INIT_GET_PASSWORD_PROTECT_PROBLEM,
        INIT_FIND_PASSWORD,
        INIT_CREATE_CHARACTER,
        INIT_CHARACTER_ID,
        // 同步位置
        SET_POSITION,
        // 快捷键
        APPLY_CHANGE_SHORTCUT,
        // 技能释放
        APPLY_CAST_SKILL_BEGIN,
        // 分配属性点
        APPLY_DISTRIBUTE_POINTS,
        APPLY_SHOW_CHARACTER_ATTRIBUTE,
        // 获取战力排行榜
        APPLY_REFRESH_FIGHT_CAPACITY_RANK,
        // 技能升级
        APPLY_UPDATE_SKILL_LEVEL,
        // 地面物品扔拾; 背包物品 使用, 交易等
        APPLY_PICK_UP_ITEM_ON_GROUND,
        APPLY_DROP_ITEM_ONTO_GROUND,
        APPLY_SAVE_ITEM_INTO_STORE_HOUSE,
        APPLY_TAKE_OUT_ITEM_FROM_STORE_HOUSE,
        APPLY_USE_CONSUMABLE_ITEM,
        APPLY_USE_EQUIPMENT_ITEM,
        APPLY_TAKE_OFF_EQUIPMENT_ITEM,
        APPLY_SELL_ITEM_IN_BAG,
        APPLY_BUY_ITEM_INTO_BAG,
        APPLY_BUILD_EQUIPMENT,
        APPLY_STRENGTHEN_EQUPMENT,
        APPLY_ENCHANT_EQUIPMENT,
        APPLY_INLAY_GEM_IN_EQUIPMENT,
        APPLY_MAKE_HOLE_IN_EQUIPMENT,
        APPLY_DISJOINT_EQUIPMENT,
        APPLY_AUTO_DISJOINT,
        APPLY_AUTO_PICK_ON,
        APPLY_AUTO_PICK_OFF,
        // 摆摊
        APPLY_SET_UP_MARKET,
        APPLY_PACK_UP_MARKET,
        APPLY_ENTER_MARKET,
        APPLY_BUY_ITEM_IN_MARKET,
        // 任务相关
        APPLY_ACCEPT_MISSION,
        APPLY_DELIVER_MISSION,
        APPLY_CANCEL_MISSION,
        APPLY_TALK_TO_MISSION_NPC,
        // 称号相关
        APPLY_ATTACH_TITLE,
        APPLY_DETACH_TITLE,
        // 获取商城信息
        APPLY_SHOW_MALL,
        APPLY_BUY_ITEM_IN_MALL,
        // 发送消息
        APPLY_SEND_MESSAGE,
        // 复活相关
        APPLY_RESPAWN_HOME,
        APPLY_RESPAWN_PLACE,
        // 邮件相关
        APPLY_SHOW_MAIL_BOX,
        APPLY_READ_MAIL,
        APPLY_READ_ALL_MAIL,
        APPLY_RECEIVE_MAIL,
        APPLY_RECEIVE_ALL_MAIL,
        // 公告相关
        APPLY_SHOW_NOTICE,
        APPLY_DELETE_NOTICE,
        // 帮派相关
        APPLY_CREATE_ALLIANCE,
        APPLY_DISSOLVE_ALLIANCE,
        APPLY_TRANSFER_ALLIANCE,
        APPLY_APPLY_TO_JOIN_ALLIANCE,
        APPLY_REFUSE_TO_JOIN_ALLIANCE,
        APPLY_APPROVE_TO_JOIN_ALLIANCE,
        APPLY_CHANGE_ALLIANCE_JOB,
        APPLY_GET_ALLIANCES_LIST,// TODO: 请求获取帮派列表(非排行榜)
        // 控制台相关
        CONSOLE_GAIN_CY_BY_NAME,
        CONSOLE_SEAL_CHARACTER_BY_NAME,
        CONSOLE_RELEASE_NOTICE,
        CONSOLE_CHARGE_MONEY,
        // 测试相关
        TEST_GAIN_CY,
        TEST_GAIN_EXP,
        TEST_GAIN_MASTERLY,
        TEST_GAIN_ITEM,
        TEST_SEND_MAIL_TO_ALL,
        TEST_SEND_MAIL_TO_ALL_ONLINE
    }
}