using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    /// <summary>
    /// 数据型Entity的管理器  
    /// 怪物  
    /// </summary>
    class DEM_Skill {
        public static DEM_Skill s_instance;
        private Dictionary<short, IReadOnlyList<DE_Skill>> m_skillAllLevelDict;
        public DEM_Skill (IDS_Skill skillDs) {
            var skillDoAllLvArr = skillDs.GetAllSkill ();
            foreach (var skillDoAllLv in skillDoAllLvArr) {
                List<DE_Skill> deList = new List<DE_Skill> (skillDoAllLv.Length);
                for (int i=0; i<skillDoAllLv.Length; i++)
                    deList[i] = new DE_Skill (skillDoAllLv[i]);
                m_skillAllLevelDict.Add (deList[0].m_skillId, deList);
            }
        }
        public DE_Skill GetSkillByIdAndLevel (short skillId, short skillLv) {
            IReadOnlyList<DE_Skill> skillAllLv = null;
            if (!m_skillAllLevelDict.TryGetValue (skillId, out skillAllLv))
                return null;
            if (skillAllLv.Count < skillLv)
                return null;
            return skillAllLv[skillLv - 1];
        }
    }
}