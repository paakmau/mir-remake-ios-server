using System.Collections.Generic;

namespace MirRemakeBackend {
    /// <summary>
    /// 存储Unit的所持技能
    /// 以NetworkId为索引
    /// </summary>
    static class EM_Skill {
        private static Dictionary<int, Dictionary<short, E_Skill>> m_networkIdAndLearnedSkillDict = new Dictionary<int, Dictionary<short, E_Skill>> ();
        public static E_Skill GetSkillByIdAndNetworkId (short skillId, int netId) {
            Dictionary<short, E_Skill> learnedSkill = null;
            if (!m_networkIdAndLearnedSkillDict.TryGetValue (netId, out learnedSkill))
                return null;
            E_Skill res = null;
            learnedSkill.TryGetValue (skillId, out res);
            return res;
        }
        public static void LoadUnitSkillList (int netId, List<E_Skill> skillList) {
            Dictionary<short, E_Skill> charLearnedSkillDict = new Dictionary<short, E_Skill> ();
            for (int i = 0; i < skillList.Count; i++)
                charLearnedSkillDict.Add (skillList[i].m_id, skillList[i]);
            m_networkIdAndLearnedSkillDict[netId] = charLearnedSkillDict;
        }
        public static void LoadUnitSkillArr (int netId, E_Skill[] skillArr) {
            Dictionary<short, E_Skill> charLearnedSkillDict = new Dictionary<short, E_Skill> ();
            foreach (var skill in skillArr)
                charLearnedSkillDict.Add (skill.m_id, skill);
            m_networkIdAndLearnedSkillDict[netId] = charLearnedSkillDict;
        }
        public static void UnloadUnitSkillByNetworkId (int netId) {
            m_networkIdAndLearnedSkillDict.Remove (netId);
        }
    }
}