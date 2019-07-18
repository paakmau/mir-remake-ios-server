using MirRemakeBackend.Entity;

namespace MirRemakeBackend.GameLogic {
    class CharacterInitializer {
        public static CharacterInitializer s_instance;
        public CharacterInitializer () { }
        public void CommandInitCharacterId (int netId, int charId) {
            // 角色
            var newChar = GL_CharacterAttribute.s_instance.NotifyInitCharacter (netId, charId);
            if (newChar == null)
                return;
            GL_CharacterCombatEfct.s_instance.NotifyInitCharacter (newChar);
            // 单位战斗属性
            GL_UnitBattleAttribute.s_instance.NotifyInitCharacter (netId);
            // Sight
            GL_CharacterSight.s_instance.NotifyInitCharacter (newChar);
            // 道具
            GL_Item.s_instance.NotifyInitCharacter (netId, charId);
            // 技能
            GL_Skill.s_instance.NotifyInitCharacter (netId, charId);
            // 任务
            GL_Mission.s_instance.NotifyInitCharacter (netId, charId);
        }
        public void CommandRemoveCharacter (int netId) {
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            GL_CharacterAttribute.s_instance.NotifyRemoveCharacter (charObj);
            GL_CharacterCombatEfct.s_instance.NotifyRemoveCharacter (charObj);
            GL_UnitBattleAttribute.s_instance.NotifyRemoveCharacter (netId);
            GL_CharacterSight.s_instance.NotifyRemoveCharacter (netId);
            GL_Item.s_instance.NotifyRemoveCharacter (netId);
            GL_Skill.s_instance.NotifyRemoveCharacter (netId);
            GL_Mission.s_instance.NotifyRemoveCharacter (netId);
        }
    }
}