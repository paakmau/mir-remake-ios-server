using LiteNetLib.Utils;
using MirRemakeBackend.GameLogic;

namespace MirRemakeBackend.Network {
    interface IClientCommand {
        NetworkToServerDataType m_DataType { get; }
        void Execute(NetDataReader reader, int netId);
    }
    class CC_InitCharacterId : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.INIT_CHARACTER_ID; } }
        public void Execute(NetDataReader reader, int netId) {
            int charId = reader.GetInt();
            GameLogicInit.s_instance.CommandInitCharacterId (netId, charId);
            GL_Effect.s_instance.CommandInitCharacterId (netId, charId);
            GL_Mission.s_instance.CommandInitCharacterId (netId, charId);
        }
    }
    class CC_SetPosition : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.SET_POSITION; } }
        public void Execute (NetDataReader reader, int netId) {
            var pos = reader.GetVector2 ();
            GL_CharacterAction.s_instance.CommandSetPosition (netId, pos);
        }
    }
    class CC_ApplyUseEquipmentItem : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_USE_EQUIPMENT_ITEM; } }
        public void Execute(NetDataReader reader, int netId) {
            int itemRealId = reader.GetInt ();
            GL_Item.s_instance.CommandApplyUseEquipmentItem (netId, itemRealId);
        }
    }
    class CC_ApplyUseConsumableItem : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_USE_CONSUMABLE_ITEM; } }
        public void Execute(NetDataReader reader, int netId) {
            int itemRealId = reader.GetInt ();
            GL_Item.s_instance.CommandApplyUseConsumableItem (netId, itemRealId);
        }
    }
    class CC_ApplyUpdateSkillLevel : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_UPDATE_SKILL_LEVEL; } }
        public void Execute (NetDataReader reader, int netId) {
            short skillId = reader.GetShort ();
            short targetSkillLevel = reader.GetShort ();
            GL_Skill.s_instance.CommandUpdateSkillLevel (netId, skillId, targetSkillLevel);
        }
    }
    class CC_ApplyCastSkillSingCancel : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_CAST_SKILL_SING_CANCEL; } }
        public void Execute (NetDataReader reader, int netId) {
            GL_CharacterAction.s_instance.CommandApplyCastSkillSingCancel (netId);
        }
    }
    class CC_ApplyCastSkillBegin : IClientCommand {
        public NetworkToServerDataType m_DataType { get { return NetworkToServerDataType.APPLY_CAST_SKILL_BEGIN; } }
        public void Execute(NetDataReader reader, int netId) {
            short skillId = reader.GetShort ();
            NO_SkillParam skillParm = reader.GetSkillParam ();
            GL_CharacterAction.s_instance.CommandApplyCastSkillBegin (netId, skillId, skillParm);
        }
    }
}