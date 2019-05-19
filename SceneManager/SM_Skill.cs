using System.Collections.Generic;

namespace MirRemakeBackend {
    class SM_Skill {
        private IDDS_Skill m_skillDynamicDataService;
        private IDS_Skill m_skillDataService;
        private Dictionary<int, Dictionary<short, short>> m_playerNetworkIdAndSkillIdLevelDict = new Dictionary<int, Dictionary<short, short>> ();
        public SM_Skill () { }
        /// <summary>
        /// 根据角色networkId与技能id获取该角色的对应技能
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public E_Skill GetSkillByIdAndNetworkId (short skillId, int netId) {
            return EM_Skill.GetSkillByIdAndNetworkId (skillId, netId);
        }
        /// <summary>
        /// 初始化角色的技能
        /// 依赖数据库读取
        /// 将角色所有技能存入EM中
        /// 返回角色的所有技能
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public List<E_Skill> InitCharacterSkill (int characterId) {
            List<E_Skill> res = new List<E_Skill> ();
            var skillListDdo = m_skillDynamicDataService.GetSkillListByCharacterId (characterId);
            for (int i=0; i<skillListDdo.Count; i++) {

            }
            return res;
        }
    }
}