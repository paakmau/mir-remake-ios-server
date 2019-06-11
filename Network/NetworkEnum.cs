using System;
namespace MirRemakeBackend.Network {
    enum NetworkToClientDataType : byte {
        // 初始化相关
        INIT_SELF_NETWORK_ID,
        INIT_SELF_ATTRIBUTE,
        INIT_SELF_SKILL,
        INIT_SELF_MISSION,
        INIT_SELF_ITEM,
        // 视野相关
        APPLY_OTHER_MONSTER_IN_SIGHT,
        APPLY_OTHER_CHARACTER_IN_SIGHT,
        APPLY_OTHER_ACTOR_UNIT_OUT_OF_SIGHT,
        // 位置 装备外观 属性
        SET_OTHER_POSITION,
        APPLY_ALL_CHANGE_EQUIPMENT,
        SET_ALL_HP_AND_MP,
        SET_SELF_CONCRETE_AND_SPECIAL_ATTRIBUTE,
        APPLY_SELF_LEVEL_AND_EXP,
        APPLY_SELF_MAIN_ATTRIBUTE,
        // 战斗状态与动画
        APPLY_OTHER_CAST_SKILL_BEGIN,
        APPLY_ALL_EFFECT,
        APPLY_ALL_STATUS,
        APPLY_ALL_DEAD,
        // 持有的物品 货币
        APPLY_SELF_UPDATE_ITEM_NUM,
        APPLY_SELF_GAIN_ITEM,
        APPLY_SELF_MOVE_ITEM,
        APPLY_SELF_UPDATE_EQUIPMENT,
        APPLY_SELF_CURRENCY,
        // 地面物品
        APPLY_GROUND_ITEM_SHOW,
        APPLY_GROUND_ITEM_DISAPPEAR,
        // 任务相关
        APPLY_SELF_ACCECPT_MISSION,
        APPLY_SELF_DELIVER_MISSION,
        APPLY_SELF_CANCEL_MISSION,
        APPLY_SELF_SET_MISSION_PROGRESS,
        // 技能相关
        APPLY_SELF_UPDATE_SKILL_LEVEL_AND_MASTERLY,
        // 商城
        SEND_SHOPPING_MALL_NORMAL,
        SEND_SHOPPING_MALL_COMPAIGN
    }
    enum NetworkToServerDataType : byte {
        // 创建角色
        CREATE_CHARACTER,
        // 初始化
        INIT_CHARACTER_ID,
        // 同步位置
        SET_POSITION,
        // 技能释放
        APPLY_CAST_SKILL_BEGIN,
        // 分配属性点
        APPLY_DISTRIBUTE_POINTS,
        // 技能升级
        APPLY_UPDATE_SKILL_LEVEL,
        // 地面物品扔拾; 背包物品 使用, 交易
        APPLY_PICK_UP_ITEM_ON_GROUND,
        APPLY_DROP_ITEM_ONTO_GROUND,
        APPLY_USE_CONSUMABLE_ITEM,
        APPLY_USE_EQUIPMENT_ITEM,
        APPLY_SELL_ITEM_IN_BAG,
        APPLY_BUY_ITEM_INTO_BAG,
        // 任务相关
        APPLY_ACCEPT_MISSION,
        APPLY_DELIVER_MISSION,
        APPLY_CANCEL_MISSION,
        APPLY_TALK_TO_MISSION_NPC,
        // 获取商城信息
        REQUIRE_SHOPPING_MALL_NORMAL,
        REQUIRE_SHOPPING_MALL_CAMPAIGN,
        // 测试相关
        TEST_GAIN_CY,
        TEST_GAIN_EXP,
        TEST_GAIN_MASTERLY
    }
}