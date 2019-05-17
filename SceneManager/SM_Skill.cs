using System.Collections.Generic;

namespace MirRemakeBackend {
    class SM_Skill {
        public static SM_Skill s_instance;
        private Dictionary<short, List<E_Skill>> m_skillAllLevelDict = new Dictionary<short, List<E_Skill>> ();
        private Dictionary<int, Dictionary<short, short>> m_playerNetworkIdAndSkillIdLevelDict = new Dictionary<int, Dictionary<short, short>> ();
        public SM_Skill () { }
        /// <summary>
        /// 根据玩家networkId与技能id获取该玩家的对应技能
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public E_Skill GetSkillByIdAndPlayerNetworkId (int networkId, short skillId) {
            Dictionary<short, short> playerSkillIdLevelDict = null;
            if (!m_playerNetworkIdAndSkillIdLevelDict.TryGetValue (networkId, out playerSkillIdLevelDict))
                return null;
            short skillLv = -1;
            if (!playerSkillIdLevelDict.TryGetValue (skillId, out skillLv))
                return null;
            List<E_Skill> skillAllLevelList = null;
            if (!m_skillAllLevelDict.TryGetValue (skillId, out skillAllLevelList))
                return null;
            if (skillLv > skillAllLevelList.Count)
                return null;
            return skillAllLevelList[skillLv - 1];
        }
        public E_Skill GetSkillByIdAndLevel (short skillId, short level) {
            List<E_Skill> skillAllLevelList = null;
            if (!m_skillAllLevelDict.TryGetValue (skillId, out skillAllLevelList))
                return null;
            if (level > skillAllLevelList.Count)
                return null;
            return skillAllLevelList[level - 1];
        }
    }
}