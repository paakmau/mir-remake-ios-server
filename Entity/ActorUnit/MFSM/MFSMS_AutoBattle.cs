using System;
using UnityEngine;

namespace MirRemakeBackend {
    struct MFSMS_AutoBattle : IMFSMState {
        public MFSMStateType m_Type { get { return MFSMStateType.AUTO_BATTLE; } }
        public E_Monster m_Self { get; set; }
        private E_Skill m_targetSkill;
        public MFSMS_AutoBattle (E_Monster self) {
            m_Self = self;
            m_targetSkill = null;
        }
        public void OnEnter (MFSMStateType prevType) { }
        public void OnTick (float dT) {
            // 如果受到攻击
            if (m_Self.m_highestHatredTarget != null) {
                var dir = m_Self.m_highestHatredTarget.m_Position - m_Self.m_Position;
                var deltaP = dir.normalized * m_Self.m_Speed * dT / 100f;
                if (deltaP.magnitude >= dir.magnitude)
                    deltaP = dir;
                m_Self.m_Position = m_Self.m_Position + deltaP;
            }
        }
        public IMFSMState GetNextState () {
            if (m_Self.m_IsFaint)
                return new MFSMS_Faint (m_Self);
            if (m_Self.m_IsDead)
                return new MFSMS_Dead (m_Self);
            if (m_Self.m_highestHatredTarget == null)
                return new MFSMS_AutoMove (m_Self);

            if (m_targetSkill == null)
                m_targetSkill = m_Self.GetRandomValidSkill ();
            if (m_targetSkill != null) {
                SkillParam sp = m_targetSkill.CompleteSkillParam (m_Self, m_Self.m_highestHatredTarget, SkillParam.s_invalidSkillParam);
                if (m_targetSkill.InRange (m_Self.m_Position, sp))
                    return new MFSMS_CastSingAndFront (m_Self, m_targetSkill, sp);
            }
            return null;
        }
        public void OnExit (MFSMStateType nextType) { }
    }
}