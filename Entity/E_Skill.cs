using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class E_MonsterSkill {
        public DE_Skill m_skillDe;
        public DE_SkillData m_skillDataDe;
        public short m_skillId;
    }
    class E_Skill : E_MonsterSkill {
        public short m_level;
        // 技能熟练度
        public int m_masterly;
        public float m_SingAndCastFrontTime { get { return m_skillDataDe.m_singTime + m_skillDataDe.m_castFrontTime; } }
        public E_Skill () { }
        public void Reset (DE_Skill skillDe, DE_SkillData skillDataDe, DDO_Skill ddo) {
            m_skillDe = skillDe;
            m_skillDataDe = skillDataDe;
            m_skillId = ddo.m_skillId;
            m_level = ddo.m_skillLevel;
            m_masterly = ddo.m_masterly;
        }
        public void Upgrade () {
            if (m_level >= m_skillDe.m_skillMaxLevel) return;
            m_level++;
            m_masterly -= m_skillDataDe.m_upgradeMasterlyInNeed;
            m_skillDataDe = m_skillDe.m_skillDataAllLevel[m_level - 1];
        }
    }
}