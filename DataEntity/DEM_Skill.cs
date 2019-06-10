using System.Collections.Generic;
using MirRemakeBackend.Data;

namespace MirRemakeBackend.DataEntity {
    /// <summary>
    /// 数据型Entity的容器  
    /// 技能  
    /// </summary>
    class DEM_Skill {
        private Dictionary<short, DE_Skill> m_skillAllLevelDict = new Dictionary<short, DE_Skill> ();
        private Dictionary<OccupationType, IReadOnlyList<short>> m_ocpSkillIdDict = new Dictionary<OccupationType, IReadOnlyList<short>> ();
        public DEM_Skill (IDS_Skill skillDs) {
            var skillDoArr = skillDs.GetAllSkill ();
            foreach (var skillDo in skillDoArr)
                m_skillAllLevelDict.Add (skillDo.m_skillId, new DE_Skill (skillDo));
            // TODO: 得到每种职业的所有技能, 当前为测试不考虑职业, 放入前6个
            List<short> allSkillIdList = new List<short> ();
            for (int i=0; i<6; i++)
                allSkillIdList.Add (skillDoArr[i].m_skillId);
            m_ocpSkillIdDict.Add (OccupationType.MAGE, allSkillIdList);
            m_ocpSkillIdDict.Add (OccupationType.ROGUE, allSkillIdList);
            m_ocpSkillIdDict.Add (OccupationType.TAOIST, allSkillIdList);
            m_ocpSkillIdDict.Add (OccupationType.WARRIOR, allSkillIdList);
        }
        public IReadOnlyList<short> GetSkillIdListByOccupation (OccupationType ocp) {
            IReadOnlyList<short> res;
            m_ocpSkillIdDict.TryGetValue (ocp, out res);
            return res;
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