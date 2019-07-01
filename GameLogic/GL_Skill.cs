using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理技能的学习, 升级
    /// </summary>
    class GL_Skill : GameLogicBase {
        public static GL_Skill s_instance;
        public GL_Skill (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void NotifyRemoveCharacter (E_Character charObj) {
            EM_Skill.s_instance.RemoveCharacter (charObj.m_networkId);
        }
        public void CommandUpdateSkillLevel (int netId, short skillId, short targetLv) {
            var skill = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (skill == null || charObj == null) return;
            short oriLv = skill.m_skillLevel;
            long costTotal = 0;
            while (skill.m_skillLevel < targetLv && skill.m_skillLevel < skill.m_skillDe.m_skillMaxLevel) {
                if (costTotal + skill.m_skillDataDe.m_upgradeMoneyInNeed > charObj.m_VirtualCurrency) break;
                if (skill.m_skillDataDe.m_upgradeCharacterLevelInNeed > charObj.m_Level) break;
                if (skill.m_skillDataDe.m_upgradeMasterlyInNeed > skill.m_masterly) break;
                costTotal += skill.m_skillDataDe.m_upgradeMoneyInNeed;
                skill.Upgrade ();
            }
            if (oriLv != skill.m_skillLevel) {
                GL_Property.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, -costTotal);
                // dds 与 client
                m_skillDds.UpdateSkill (skill.GetDdo (charObj.m_characterId));
                m_networkService.SendServerCommand (SC_ApplySelfUpdateSkillLevelAndMasterly.Instance (
                    netId, skill.m_SkillId, skill.m_skillLevel, skill.m_masterly));
                // log
                GL_Log.s_instance.NotifyLog (GameLogType.LEVEL_UP_SKILL, netId, skillId, skill.m_skillLevel);
            }
        }
        public void CommandGainMasterly (int netId, short skillId, int masterly) {
            int charId = EM_Unit.s_instance.GetCharIdByNetworkId (netId);
            var skObj = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            if (charId == -1 || skObj == null) return;
            skObj.m_masterly += masterly;
            // dds 与 client
            m_skillDds.UpdateSkill (skObj.GetDdo (charId));
            m_networkService.SendServerCommand (SC_ApplySelfUpdateSkillLevelAndMasterly.Instance (
                netId, skObj.m_SkillId, skObj.m_skillLevel, skObj.m_masterly));
        }
    }
}