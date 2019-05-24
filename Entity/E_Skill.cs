using System;
using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.DataEntity;

namespace MirRemakeBackend.Entity {
    class E_Skill {
        public DE_Skill m_skillDe;
        public DE_SkillData m_skillDataDe;
        public short m_id;
        public short m_level;
        // 技能熟练度
        public int m_masterly;
        public float m_SingAndCastFrontTime { get { return m_skillDataDe.m_singTime + m_skillDataDe.m_castFrontTime; } }
        public E_Skill () { }
        public void Reset (DE_Skill skillDe, DE_SkillData skillDataDe, DDO_Skill ddo) {
            m_skillDe = skillDe;
            m_skillDataDe = skillDataDe;
            m_id = ddo.m_skillId;
            m_level = ddo.m_skillLevel;
            m_masterly = ddo.m_masterly;
        }
    }
}