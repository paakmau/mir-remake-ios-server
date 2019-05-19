using System.Collections.Generic;

namespace MirRemakeBackend {
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
        public static void LoadSkillList (int netId, List<E_Skill> skillList) {
            Dictionary<short, E_Skill> charLearnedSkillDict = new Dictionary<short, E_Skill> ();
            for (int i=0; i<skillList.Count; i++)
                charLearnedSkillDict.Add (skillList[i].m_id, skillList[i]);
            m_networkIdAndLearnedSkillDict[netId] = charLearnedSkillDict;
        }
        public static void UnloadSkillByNetworkId (int netId) {
            m_networkIdAndLearnedSkillDict.Remove (netId);
        }
    }
}