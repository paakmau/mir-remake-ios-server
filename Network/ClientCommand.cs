using LiteNetLib.Utils;
using MirRemakeBackend.CharacterCreate;
using MirRemakeBackend.GameLogic;

namespace MirRemakeBackend.Network {
    interface IClientCommand {
        NetworkToServerDataType m_DataType { get; }
        void Execute (NetDataReader reader, int netId);
    }
    class CC_CreateCharacter : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.CREATE_CHARACTER; } }
        public void Execute (NetDataReader reader, int netId) {
            int playerId = reader.GetInt ();
            OccupationType ocp = (OccupationType) reader.GetByte ();
            CharacterCreator.s_instance.CommandCreateCharacter (playerId, ocp);
        }
    }
    /// <summary>
    /// 初始传入CharacterId
    /// </summary>
    class CC_InitCharacterId : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.INIT_CHARACTER_ID; } }
        public void Execute (NetDataReader reader, int netId) {
            int charId = reader.GetInt ();
            UnitInitializer.s_instance.CommandInitCharacterId (netId, charId);
        }
    }
    /// <summary>
    /// 同步位置
    /// </summary>
    class CC_SetPosition : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.SET_POSITION; } }
        public void Execute (NetDataReader reader, int netId) {
            var pos = reader.GetVector2 ();
            GL_CharacterAction.s_instance.CommandSetPosition (netId, pos);
        }
    }
    /// <summary>
    /// 释放技能
    /// </summary>
    class CC_ApplyCastSkillBegin : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_CAST_SKILL_BEGIN; } }
        public void Execute (NetDataReader reader, int netId) {
            short skillId = reader.GetShort ();
            NO_SkillParam skillParm = reader.GetSkillParam ();
            byte cnt = reader.GetByte ();
            int[] hitTargetArr = new int[cnt];
            for (int i = 0; i < cnt; i++) {
                hitTargetArr[i] = reader.GetInt ();
            }
            GL_CharacterAction.s_instance.CommandApplyCastSkillBegin (netId, skillId, skillParm, hitTargetArr);
        }
    }
    /// <summary>
    /// 属性点分配
    /// </summary>
    class CC_ApplyDistributePoints : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_DISTRIBUTE_POINTS; } }
        public void Execute (NetDataReader reader, int netId) {
            short strength = reader.GetShort ();
            short intelligence = reader.GetShort ();
            short agility = reader.GetShort ();
            short spirit = reader.GetShort ();
            GL_CharacterAttribute.s_instance.CommandApplyDistributePoints (netId, strength, intelligence, agility, spirit);
        }
    }
    /// <summary>
    /// 打开战力排行榜时请求刷新操作
    /// </summary>
    class CC_RequireRefreshFightCapacityRank : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.REQUIRE_REFRESH_FIGHT_CAPACITY_RANK; } }
        public void Execute (NetDataReader reader, int netId) {
            OccupationType ocp = (OccupationType) reader.GetByte ();
            GL_CharacterAttribute.s_instance.CommandGetCombatEffectivenessRank (netId);
        }
    }
    /// <summary>
    /// 技能升级
    /// </summary>
    class CC_ApplyUpdateSkillLevel : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_UPDATE_SKILL_LEVEL; } }
        public void Execute (NetDataReader reader, int netId) {
            short skillId = reader.GetShort ();
            short targetSkillLevel = reader.GetShort ();
            GL_Skill.s_instance.CommandUpdateSkillLevel (netId, skillId, targetSkillLevel);
        }
    }
    /// <summary>
    /// 使用消耗品
    /// </summary>
    class CC_ApplyUseConsumableItem : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_USE_CONSUMABLE_ITEM; } }
        public void Execute (NetDataReader reader, int netId) {
            int itemRealId = reader.GetInt ();
            GL_Item.s_instance.CommandApplyUseConsumableItem (netId, itemRealId);
        }
    }
    /// <summary>
    /// 使用背包中的装备
    /// </summary>
    class CC_ApplyUseEquipmentItem : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_USE_EQUIPMENT_ITEM; } }
        public void Execute (NetDataReader reader, int netId) {
            int itemRealId = reader.GetInt ();
            GL_Item.s_instance.CommandApplyUseEquipmentItem (netId, itemRealId);
        }
    }
    /// <summary>
    /// 出售背包中的物品
    /// </summary>
    class CC_ApplySellItemInBag : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_SELL_ITEM_IN_BAG; } }
        public void Execute (NetDataReader reader, int netId) {
            long realId = reader.GetLong ();
            short num = reader.GetShort ();
            GL_Mall.s_instance.CommandApplySellItemInBag (netId, realId, num);
        }
    }
    /// <summary>
    /// 购买物品放入背包
    /// </summary>
    class CC_ApplyBuyItemIntoBag : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_BUY_ITEM_INTO_BAG; } }
        public void Execute (NetDataReader reader, int netId) {
            short itemId = reader.GetShort ();
            short num = reader.GetShort ();
            GL_Mall.s_instance.CommandApplyBuyItemIntoBag (netId, itemId, num);
        }
    }
    /// <summary>
    /// 接受任务
    /// </summary>
    class CC_ApplyAcceptMission : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_ACCEPT_MISSION; } }
        public void Execute (NetDataReader reader, int netId) {
            short missionId = reader.GetShort ();
            GL_Mission.s_instance.CommandApplyAcceptMission (netId, missionId);
        }
    }
    /// <summary>
    /// 交付任务
    /// </summary>
    class CC_ApplyDeliverMission : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_DELIVER_MISSION; } }
        public void Execute (NetDataReader reader, int netId) {
            short missionId = reader.GetShort ();
            GL_Mission.s_instance.CommandApplyDeliveryMission (netId, missionId);
        }
    }
    /// <summary>
    /// 放弃任务
    /// </summary>
    class CC_ApplyCancelMission : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_CANCEL_MISSION; } }
        public void Execute (NetDataReader reader, int netId) {
            short missionId = reader.GetShort ();
            GL_Mission.s_instance.CommandCancelMission (netId, missionId);
        }
    }
    /// <summary>
    /// 与任务Npc交流
    /// </summary>
    class CC_ApplyTalkToMissionNpc : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_TALK_TO_MISSION_NPC; } }
        public void Execute (NetDataReader reader, int netId) {
            short npcId = reader.GetShort ();
            short missionId = reader.GetShort ();
            GL_Mission.s_instance.CommandApplyTalkToNpc (netId, npcId, missionId);
        }
    }
    /// <summary>
    /// 获取商场 常规商品列表
    /// </summary>
    class CC_RequireShoppingMallNormal : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.REQUIRE_SHOPPING_MALL_NORMAL; } }
        public void Execute (NetDataReader reader, int netId) {
            // TODO: 普通商城
        }
    }
    /// <summary>
    /// 获取商城 活动商品列表
    /// </summary>
    class CC_RequireShoppingMallCampaign : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.REQUIRE_SHOPPING_MALL_CAMPAIGN; } }
        public void Execute (NetDataReader reader, int netId) {
            // TODO: 活动商城
        }
    }
    /// <summary>
    /// 请求发送消息  
    /// 数据格式:  
    /// chanelType: ChattingChanelType,  
    /// messageContent: string,  
    /// to: int
    /// </summary>
    class CC_RequireSendMessage : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.REQUIRE_SEND_MESSAGE; } }
        public void Execute (NetDataReader reader, int netId) {
            ChattingChanelType channel = (ChattingChanelType) reader.GetByte ();
            string msg = reader.GetString ();
            int toCharId = reader.GetInt ();
            GL_Chat.s_instance.CommandSendMessage (netId, channel, msg, toCharId);
        }
    }
    class CC_TestGainExp : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.TEST_GAIN_EXP; } }
        public void Execute (NetDataReader reader, int netId) {
            int exp = reader.GetInt ();
            GL_CharacterAttribute.s_instance.CommandGainExperience (netId, exp);
        }
    }
    class CC_TestGainMasterly : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.TEST_GAIN_MASTERLY; } }
        public void Execute (NetDataReader reader, int netId) {
            short skId = reader.GetShort ();
            int masterly = reader.GetInt ();
            GL_Skill.s_instance.CommandGainMasterly (netId, skId, masterly);
        }
    }
    class CC_TestGainCy : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.TEST_GAIN_CY; } }
        public void Execute (NetDataReader reader, int netId) {
            CurrencyType cyType = (CurrencyType) reader.GetByte ();
            long cy = reader.GetLong ();
            GL_CharacterAttribute.s_instance.CommandGainCurrency (netId, cyType, cy);
        }
    }
    class CC_TestGainItem : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.TEST_GAIN_ITEM; } }
        public void Execute (NetDataReader reader, int netId) {
            short itemId = reader.GetShort ();
            short num = reader.GetShort ();
            GL_Item.s_instance.CommandGainItem (netId, itemId, num);
        }
    }
}