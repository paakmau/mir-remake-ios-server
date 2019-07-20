using System.Collections.Generic;
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
        public void CommandUpdateSkillLevel (int netId, short skillId, short targetLv) {
            var skill = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            var charObj = EM_Unit.s_instance.GetCharacterByNetworkId (netId);
            if (skill == null || charObj == null) return;
            short oriLv = skill.m_skillLevel;
            long costTotal = 0;
            while (skill.m_skillLevel < targetLv && skill.m_skillLevel < skill.m_skillDe.m_skillMaxLevel) {
                if (costTotal + skill.m_skillDataDe.m_upgradeMoneyInNeed > charObj.m_virtualCurrency) break;
                if (skill.m_skillDataDe.m_upgradeCharacterLevelInNeed > charObj.m_Level) break;
                if (skill.m_skillDataDe.m_upgradeMasterlyInNeed > skill.m_masterly) break;
                costTotal += skill.m_skillDataDe.m_upgradeMoneyInNeed;
                skill.Upgrade ();
            }
            if (oriLv != skill.m_skillLevel) {
                GL_CharacterAttribute.s_instance.NotifyUpdateCurrency (charObj, CurrencyType.VIRTUAL, -costTotal);
                // 持久化 与 client
                EM_Skill.s_instance.CharacterUpdateSkill (charObj.m_characterId, skill);
                m_networkService.SendServerCommand (SC_ApplySelfUpdateSkillLevelAndMasterly.Instance (
                    netId, skill.m_SkillId, skill.m_skillLevel, skill.m_masterly));
                // log
                GL_MissionLog.s_instance.NotifyLog (MissionLogType.LEVEL_UP_SKILL, netId, skillId, skill.m_skillLevel);
            }
        }
        public void CommandGainMasterly (int netId, short skillId, int masterly) {
            int charId = EM_Unit.s_instance.GetCharIdByNetworkId (netId);
            var skObj = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            if (charId == -1 || skObj == null) return;
            skObj.m_masterly += masterly;
            // 持久化 与 client
            EM_Skill.s_instance.CharacterUpdateSkill (charId, skObj);
            m_networkService.SendServerCommand (SC_ApplySelfUpdateSkillLevelAndMasterly.Instance (
                netId, skObj.m_SkillId, skObj.m_skillLevel, skObj.m_masterly));
        }
        public void NotifyInitCharacter (int netId, int charId) {
            E_Skill[] skillArr = EM_Skill.s_instance.InitCharacter (netId, charId);
            // client
            var skillIdAndLvAndMasterlyArr = new (short, short, int) [skillArr.Length];
            for (int i = 0; i < skillArr.Length; i++)
                skillIdAndLvAndMasterlyArr[i] = (skillArr[i].m_SkillId, skillArr[i].m_skillLevel, skillArr[i].m_masterly);
            m_networkService.SendServerCommand (SC_InitSelfSkill.Instance (netId, skillIdAndLvAndMasterlyArr));
        }
        public void NotifyRemoveCharacter (int netId) {
            EM_Skill.s_instance.RemoveCharacter (netId);
        }
        public void NotifyCastSkill (E_Character charObj, E_Skill skill, List<E_Unit> tarList) {
            // xjb masterly
            skill.m_masterly += tarList.Count * 10;
            EM_Skill.s_instance.CharacterUpdateSkill (charObj.m_characterId, skill);
            m_networkService.SendServerCommand (SC_ApplySelfUpdateSkillLevelAndMasterly.Instance (charObj.m_networkId, skill.m_SkillId, skill.m_skillLevel, skill.m_masterly));
            // 通知战斗结算
            GL_BattleSettle.s_instance.NotifySkillSettle (charObj, skill, tarList);
        }
    }
}