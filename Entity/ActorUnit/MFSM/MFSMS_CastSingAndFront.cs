using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    struct MFSMS_CastSingAndFront : IMFSMState {
        public MFSMStateType m_Type { get { return MFSMStateType.CAST_SING_AND_FRONT; } }
        public E_Monster m_Self { get; set; }
        private E_Skill m_skill;
        private SkillParam m_skillParam;
        private float m_timer;
        public MFSMS_CastSingAndFront (E_Monster self, E_Skill skill, SkillParam parm) {
            m_Self = self;
            m_skill = skill;
            m_skillParam = parm;
            m_timer = 0f;
        }
        public void OnEnter (MFSMStateType prevType) {
            m_timer = m_skill.m_singTime + m_skill.m_castFrontTime;
            m_Self.FSMCastSkillBegin (m_skill, new SkillParam ());
        }
        public void OnTick (float dT) {
            m_timer -= dT;
        }
        public IMFSMState GetNextState () {
            if (m_Self.m_IsFaint)
                return new MFSMS_Faint (m_Self);
            if (m_Self.m_IsDead)
                return new MFSMS_Dead(m_Self);
            // 咏唱结束
            if (m_timer <= 0f)
                return new MFSMS_CastBack (m_Self, m_skill.m_castBackTime);
            // 被沉默
            if (m_Self.m_IsSilent)
                return new MFSMS_AutoBattle (m_Self);
            return null;
        }
        public void OnExit (MFSMStateType nextType) {
            if (nextType == MFSMStateType.CAST_BACK) {
                m_Self.FSMCastSkillSettle (m_skill, m_skillParam);
            }
        }
    }
}