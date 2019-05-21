/**
 *Enity，技能实体
 *创建者 fn
 *时间 2019/4/1
 *最后修改者 yuk
 *时间 2019/4/3
 */

using System;
using System.Collections.Generic;
using UnityEngine;
namespace MirRemakeBackend {
    class E_Skill {
        private DE_Skill m_skillDe;
        private DE_SkillData m_skillDataDe;
        public short m_id;
        public short m_level;
        // 技能熟练度
        public int m_masterly;
        public E_Skill () { }
        public void Reset (DE_Skill skillDe, DE_SkillData skillDataDe, DDO_Skill ddo) {
            m_skillDe = skillDe;
            m_skillDataDe = skillDataDe;
            m_id = ddo.m_skillId;
            m_level = ddo.m_skillLevel;
            m_masterly = ddo.m_masterly;
        }
        public void Reset (DE_Skill skillDe, DE_SkillData skillDataDe) {
            m_skillDe = skillDe;
            m_skillDataDe = skillDataDe;
        }
    }
}