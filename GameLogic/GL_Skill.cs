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
        private IDDS_Character m_characterDds;
        public GL_Skill (IDDS_Skill skillDds, IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_skillDds = skillDds;
            m_characterDds = charDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandUpdateSkillLevel (int netId, short skillId, short targetLv) {
            var skill = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (skill == null || charObj == null) return;
            short oriLv = skill.m_skillLevel;
            while (skill.m_skillLevel < targetLv && skill.m_skillLevel < skill.m_skillDe.m_skillMaxLevel) {
                if (skill.m_skillDataDe.m_upgradeMoneyInNeed > charObj.m_VirtualCurrency) break;
                if (skill.m_skillDataDe.m_upgradeCharacterLevelInNeed > charObj.m_level) break;
                if (skill.m_skillDataDe.m_upgradeMasterlyInNeed > skill.m_masterly) break;
                charObj.m_VirtualCurrency -= skill.m_skillDataDe.m_upgradeMoneyInNeed;
                skill.Upgrade ();
            }
            if (oriLv != skill.m_skillLevel) {
                m_skillDds.UpdateSkill (skill.GetDdo (charObj.m_characterId));
                m_characterDds.UpdateCharacter (charObj.GetDdo ());
                var toClientList = new List<int> ();
                toClientList.Add (netId);
                m_networkService.SendServerCommand (SC_ApplySelfUpdateSkillLevelAndMasterly.Instance (
                    toClientList, skill.m_skillId, skill.m_skillLevel, skill.m_masterly));
            }
        }
    }
}