using MirRemakeBackend.Entity;

namespace MirRemakeBackend.GameLogic {
    class GL_CharacterInit {
        public static GL_CharacterInit s_instance;
        public GL_CharacterInit () { }
        public void CommandInitCharacterId (int netId, int charId) {
            E_Character newChar = EM_Character.s_instance.InitCharacter (netId, charId);
            if (newChar == null)
                return;
            // 角色
            GL_CharacterAttribute.s_instance.NotifyInitCharacter (newChar);
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
            var charObj = EM_Character.s_instance.GetCharacterByNetworkId (netId);
            if (charObj == null) return;
            GL_CharacterAttribute.s_instance.NotifyRemoveCharacter (netId);
            GL_CharacterCombatEfct.s_instance.NotifyRemoveCharacter (charObj);
            GL_UnitBattleAttribute.s_instance.NotifyRemoveCharacter (netId);
            GL_CharacterSight.s_instance.NotifyRemoveCharacter (netId);
            GL_Item.s_instance.NotifyRemoveCharacter (netId);
            GL_Skill.s_instance.NotifyRemoveCharacter (netId);
            GL_Mission.s_instance.NotifyRemoveCharacter (netId);
        }
    }
}