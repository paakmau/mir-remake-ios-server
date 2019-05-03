using System;
using UnityEngine;

namespace MirRemake {
    struct MFSMS_Free : IMFSMState {
        public MFSMStateType m_Type { get { return MFSMStateType.FREE; } }
        public E_Monster m_Self { get; set; }
        private float m_moveTimeLeft;
        private Vector2 m_targetPos;
        private E_Skill m_targetSkill;
        public MFSMS_Free (E_Monster self) {
            m_Self = self;
            m_targetPos = self.m_position;
            m_moveTimeLeft = 0f;
            m_targetSkill = null;
        }
        public void OnEnter (MFSMStateType prevType) { }
        public void OnTick (float dT) {
            // 如果受到攻击
            if (m_Self.m_highestHatredTarget != null) {
                m_targetPos = m_Self.m_highestHatredTarget.m_position;
                m_moveTimeLeft = 0f;
            }

            if (m_moveTimeLeft > 0f)
                m_moveTimeLeft -= dT;
            else {
                var dir = m_targetPos - m_Self.m_position;
                var deltaP = dir.normalized * m_Self.m_Speed * dT;
                if (deltaP.magnitude >= dir.magnitude)
                    deltaP = dir;
                m_Self.m_position = m_Self.m_position + deltaP;
                if ((m_Self.m_position - m_targetPos).sqrMagnitude <= 0.01f) {
                    m_moveTimeLeft = MyRandom.NextFloat (3f, 6f);
                    m_targetPos = m_Self.m_oriPosition + new Vector2 (MyRandom.NextFloat (0f, 2.5f), MyRandom.NextFloat(0f, 2.5f));
                }
            }
        }
        public void SetSelf (E_Monster self) {
            m_Self = self;
        }
        public IMFSMState GetNextState () {
            if (m_Self.m_highestHatredTarget != null) {
                if (m_targetSkill == null)
                    m_targetSkill = m_Self.GetRandomSkill();
                var tarPos = new TargetPosition (m_Self.m_highestHatredTarget);
                if (m_targetSkill.CheckInRange (m_Self, tarPos))
                    return new MFSMS_CastSingAndFront (m_Self, m_Self.GetRandomSkill(), new TargetPosition(m_Self.m_highestHatredTarget));
            }
            return null;
        }
        public void OnExit (MFSMStateType nextType) { }
    }
}