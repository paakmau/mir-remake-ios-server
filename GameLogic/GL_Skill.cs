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
        private IDDS_Skill m_skillDds;
        public GL_Skill (IDDS_Skill skillDds, INetworkService netService) : base (netService) {
            m_skillDds = skillDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
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
                m_skillDds.UpdateSkill (skill.GetDdo (charObj.m_characterId));
                GL_Property.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, -costTotal);
                m_networkService.SendServerCommand (SC_ApplySelfUpdateSkillLevelAndMasterly.Instance (
                    new List<int> { netId }, skill.m_skillId, skill.m_skillLevel, skill.m_masterly));
            }
        }
    }
}