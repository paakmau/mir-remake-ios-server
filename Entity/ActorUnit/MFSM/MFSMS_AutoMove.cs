using System;
using UnityEngine;

namespace MirRemakeBackend {
    struct MFSMS_AutoMove : IMFSMState {
        public MFSMStateType m_Type { get { return MFSMStateType.AUTO_MOVE; } }
        public E_Monster m_Self { get; set; }
        private float m_moveTimeLeft;
        private Vector2 m_targetPos;
        public MFSMS_AutoMove (E_Monster self) {
            m_Self = self;
            m_targetPos = self.m_Position;
            m_moveTimeLeft = 0f;
        }
        public void OnEnter (MFSMStateType prevType) { }
        public void OnTick (float dT) {
            // 如果受到攻击
            if (m_Self.m_highestHatredTarget != null) {
                m_targetPos = m_Self.m_highestHatredTarget.m_Position;
                m_moveTimeLeft = 0f;
            }

            if (m_moveTimeLeft > 0f)
                m_moveTimeLeft -= dT;
            else {
                var dir = m_targetPos - m_Self.m_Position;
                var deltaP = dir.normalized * m_Self.m_Speed * dT / 100f;
                if (deltaP.magnitude >= dir.magnitude)
                    deltaP = dir;
                m_Self.m_Position = m_Self.m_Position + deltaP;
                if ((m_Self.m_Position - m_targetPos).sqrMagnitude <= 0.01f) {
                    m_moveTimeLeft = MyRandom.NextFloat (3f, 6f);
                    m_targetPos = m_Self.m_oriPosition + new Vector2 (MyRandom.NextFloat (0f, 2.5f), MyRandom.NextFloat (0f, 2.5f));
                }
            }
        }
        public IMFSMState GetNextState () {
            if (m_Self.m_IsFaint)
                return new MFSMS_Faint (m_Self);
            if (m_Self.m_IsDead)
                return new MFSMS_Dead (m_Self);
            if (m_Self.m_highestHatredTarget != null)
                return new MFSMS_AutoBattle (m_Self);
            return null;
        }
        public void OnExit (MFSMStateType nextType) { }
    }
}