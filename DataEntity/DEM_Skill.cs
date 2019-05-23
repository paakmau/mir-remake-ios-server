using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 技能  
    /// </summary>
    class DEM_Skill {
        public static DEM_Skill s_instance;
        private Dictionary<short, DE_Skill> m_skillAllLevelDict;
        public DEM_Skill (IDS_Skill skillDs) {
            var skillDoArr = skillDs.GetAllSkill ();
            foreach (var skillDo in skillDoArr)
                m_skillAllLevelDict.Add (skillDo.m_skillId, new DE_Skill (skillDo));
        }
        public bool GetSkillByIdAndLevel (short skillId, short skillLv, out DE_Skill skill, out DE_SkillData skillData) {
            skill = null;
            skillData = null;
            if (!m_skillAllLevelDict.TryGetValue (skillId, out skill))
                return false;
            if (skill.m_skillDataAllLevel.Count < skillLv)
                return false;
            skillData = skill.m_skillDataAllLevel[skillLv - 1];
            return true;
        }
    }
}