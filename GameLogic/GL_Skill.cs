using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.DataEntity;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理技能的学习, 升级
    /// </summary>
    class GL_Skill : GameLogicBase {
        private IDDS_Skill m_skillDds;
        public GL_Skill (IDDS_Skill skillDds, INetworkService netService) : base (netService) {
            m_skillDds = skillDds;
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
            Messenger.AddListener<int> ("CommandRemoveCharacter", CommandRemoveCharacter);
            Messenger.AddListener<int, short, short> ("CommandUpdateSkillLevel", CommandUpdateSkillLevel);
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandInitCharacterId (int netId, int charId) {
            // 读取角色技能并创建实例
            var skillDdoList = m_skillDds.GetSkillListByCharacterId (charId);
            E_Skill[] skillArr = EM_Skill.s_instance.InitCharacterSkill (netId, charId, skillDdoList);
            short[] skillIdArr = new short[skillArr.Length];
            short[] skillLvArr = new short[skillArr.Length];
            int[] skillMasterlyArr = new int[skillArr.Length];
            for (int i = 0; i < skillArr.Length; i++) {
                skillIdArr[i] = skillArr[i].m_id;
                skillLvArr[i] = skillArr[i].m_level;
                skillMasterlyArr[i] = skillArr[i].m_masterly;
            }
            // TODO: 发送给客户端
        }
        public void CommandRemoveCharacter (int netId) {
            EM_Skill.s_instance.RemoveCharacterSkill (netId);
        }
        public void CommandUpdateSkillLevel (int netId, short skillId, short targetLv) {
            var skill = EM_Skill.s_instance.GetCharacterSkillByIdAndNetworkId (skillId, netId);
            var charObj = EM_ActorUnit.s_instance.GetCharacterByNetworkId (netId);
            if (skill == null || charObj == null) return;
            short oriLv = skill.m_level;
            while (skill.m_level < targetLv && skill.m_level < skill.m_skillDe.m_skillMaxLevel) {
                if (skill.m_skillDataDe.m_upgradeMoneyInNeed > charObj.m_coin) break;
                if (skill.m_skillDataDe.m_upgradeCharacterLevelInNeed > charObj.m_level) break;
                if (skill.m_skillDataDe.m_upgradeMasterlyInNeed > skill.m_masterly) break;
                charObj.m_coin -= skill.m_skillDataDe.m_upgradeMoneyInNeed;
                skill.Upgrade ();
            }
            if (oriLv != skill.m_level) {
                // TODO: 考虑数据库
                // TODO: 发给Client
            }
        }
    }
}


