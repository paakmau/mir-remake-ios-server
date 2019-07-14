using System.Collections.Generic;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.GameLogic {
    class UnitInitializer {
        public static UnitInitializer s_instance;
        public UnitInitializer () { }
        public void CommandInitCharacterId (int netId, int charId) {
            // 角色
            var newChar = GL_CharacterAttribute.s_instance.NotifyInitCharacter (netId, charId);
            if (newChar == null)
                return;
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
            GL_CharacterAttribute.s_instance.NotifyRemoveCharacter (netId);
            GL_UnitBattleAttribute.s_instance.NotifyRemoveCharacter (netId);
            GL_CharacterSight.s_instance.NotifyRemoveCharacter (netId);
            GL_Item.s_instance.NotifyRemoveCharacter (netId);
            GL_Skill.s_instance.NotifyRemoveCharacter (netId);
            GL_Mission.s_instance.NotifyRemoveCharacter (netId);
        }
        public void InitAllMonster () {

        }
    }
}