using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 技能  
    /// </summary>
    class DEM_Skill {
        private Dictionary<short, DE_Skill> m_skillAllLevelDict = new Dictionary<short, DE_Skill> ();
        private Dictionary<OccupationType, List<DE_Skill>> m_ocpSkillDict = new Dictionary<OccupationType, List<DE_Skill>> ();
        public DEM_Skill (IDS_Skill skillDs) {
            var skillDoArr = skillDs.GetAllSkill ();
            OccupationType[] ocpArr = new OccupationType[] { OccupationType.MAGE, OccupationType.ROGUE, OccupationType.TAOIST, OccupationType.WARRIOR };
            foreach (var ocp in ocpArr)
                m_ocpSkillDict.Add (ocp, new List<DE_Skill> ());
            foreach (var skillDo in skillDoArr) {
                var de = new DE_Skill (skillDo);
                m_skillAllLevelDict.Add (skillDo.m_skillId, de);
                m_ocpSkillDict[skillDo.m_occupation].Add (de);
            }
        }
        public bool GetSkillByIdAndLevel (short skillId, short skillLv, out DE_Skill skill, out DE_SkillData skillData) {
            skill = null;
            skillData = null;
            if (!m_skillAllLevelDict.TryGetValue (skillId, out skill))
                return false;
            if (skill.m_skillDataAllLevel.Count < skillLv)
                return false;
            skillData = skill.m_skillDataAllLevel[skillLv];
            return true;
        }
        public IReadOnlyList<DE_Skill> GetSkillsByOccupation (OccupationType ocp) {
            List<DE_Skill> res;
            m_ocpSkillDict.TryGetValue (ocp, out res);
            return res;
        }
    }
}