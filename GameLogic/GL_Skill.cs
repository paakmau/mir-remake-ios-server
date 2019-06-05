using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理技能的学习, 升级
    /// </summary>
    class GL_Skill : GameLogicBase {
        public static GL_Skill s_instance;
        private IDDS_Skill m_skillDds;
        private IDDS_Character m_characterDds;
        private List<int> t_intList = new List<int> ();
        public GL_Skill (IDDS_Skill skillDds, IDDS_Character charDds, INetworkService netService) : base (netService) {
            m_skillDds = skillDds;
            m_characterDds = charDds;
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandInitCharacterId (int netId, int charId) {
            // 读取角色技能并创建实例
            var skillDdoList = m_skillDds.GetSkillListByCharacterId (charId);
            E_Skill[] skillArr = EM_Skill.s_instance.InitCharacterSkill (netId, charId, skillDdoList);

            // 发送初始技能信息
            t_intList.Clear ();
            t_intList.Add (netId);
            var skillIdAndLvAndMasterlyArr = new (short, short, int) [skillArr.Length];
            for (int i = 0; i < skillArr.Length; i++)
                skillIdAndLvAndMasterlyArr[i] = (skillArr[i].m_skillId, skillArr[i].m_skillLevel, skillArr[i].m_masterly);
            m_networkService.SendServerCommand (SC_InitSelfSkill.Instance (t_intList, skillIdAndLvAndMasterlyArr));
        }
        public void CommandRemoveCharacter (int netId) {
            EM_Skill.s_instance.RemoveCharacterSkill (netId);
        }
        public void CommandUpdateSkillLevel (int netId, short skillId, short targetLv) {
            var skill = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            var charObj = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
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
                t_intList.Clear ();
                t_intList.Add (netId);
                m_networkService.SendServerCommand (SC_ApplySelfUpdateSkillLevelAndMasterly.Instance (
                    t_intList, skill.m_skillId, skill.m_skillLevel, skill.m_masterly));
            }
        }
    }
}