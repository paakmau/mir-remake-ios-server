using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    struct MFSMS_CastSingAndFront : IMFSMState {
        public MFSMStateType m_Type { get { return MFSMStateType.CAST_SING_AND_FRONT; } }
        public E_Monster m_Self { get; set; }
        private E_Skill m_skill;
        private TargetPosition m_tarPos;
        private float m_timer;
        public MFSMS_CastSingAndFront (E_Monster self, E_Skill skill, TargetPosition tarPos) {
            m_Self = self;
            m_skill = skill;
            m_tarPos = tarPos;
            m_timer = 0f;
        }
        public void OnEnter (MFSMStateType prevType) {
            m_timer = m_skill.m_singTime + m_skill.m_castFrontTime;
            // TODO: 发送普通FSM状态机
        }
        public void OnTick (float dT) {
            m_timer -= dT;
        }
        public IMFSMState GetNextState () {
            // 咏唱结束
            if (m_timer <= 0f)
                return new MFSMS_CastBack (m_Self, m_skill.m_castBackTime);
            // 被沉默
            if (m_Self.m_IsSilent)
                return new MFSMS_Free (m_Self);
            return null;
        }
        public void OnExit (MFSMStateType nextType) {
            if (nextType == MFSMStateType.CAST_BACK) {
                m_Self.RequestCastSkill (m_skill, m_tarPos);
            }
        }
    }
}