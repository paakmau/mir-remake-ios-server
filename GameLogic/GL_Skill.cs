using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理技能的学习, 升级
    /// </summary>
    class GL_Skill : GameLogicBase {
        private IDDS_Skill m_skillDds;
        public GL_Skill (IDDS_Skill skillDds, INetworkService netService) : base (netService) {
            m_skillDds = skillDds;
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
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
    }
}