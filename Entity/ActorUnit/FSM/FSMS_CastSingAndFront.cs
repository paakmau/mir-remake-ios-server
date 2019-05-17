using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    struct FSMS_CastSingAndFront : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.CAST_SING_AND_FRONT; } }
        public E_Monster m_Self { get; set; }
        private E_Skill m_skill;
        private SkillParam m_skillParam;
        private float m_timer;
        public FSMS_CastSingAndFront (E_Monster self, E_Skill skill, SkillParam parm) {
            m_Self = self;
            m_skill = skill;
            m_skillParam = parm;
            m_timer = 0f;
        }
        public void OnEnter (FSMStateType prevType) {
            m_timer = m_skill.m_SingTime + m_skill.m_castFrontTime;
            m_Self.RequestCastSkillBegin (m_skill, new SkillParam ());
        }
        public void OnTick (float dT) {
            m_timer -= dT;
        }
        public IFSMState GetNextState () {
            if (m_Self.m_IsDead)
                return new FSMS_Dead(m_Self);
            // 咏唱结束
            if (m_timer <= 0f)
                return new FSMS_CastBack (m_Self, m_skill.m_castBackTime);
            // 被沉默
            if (m_Self.m_IsSilent)
                return new FSMS_Free (m_Self);
            return null;
        }
        public void OnExit (FSMStateType nextType) {
            if (nextType == FSMStateType.CAST_BACK) {
                m_Self.RequestCastSkillSettle (m_skill, m_skillParam);
            }
        }
    }
}