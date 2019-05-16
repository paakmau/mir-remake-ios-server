using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    struct MFSMS_CastBack : IMFSMState {
        public MFSMStateType m_Type { get { return MFSMStateType.CAST_BACK; } }
        public E_Monster m_Self { get; set; }
        private float m_timer;
        public MFSMS_CastBack (E_Monster self, float backTime) {
            m_Self = self;
            m_timer = backTime;
        }
        public void OnEnter (MFSMStateType prevType) { }
        public void OnTick (float dT) {
            m_timer -= dT;
        }
        public IMFSMState GetNextState () {
            if (m_Self.m_IsDead)
                return new MFSMS_Dead(m_Self);
            // 后摇结束
            if (m_timer <= 0f)
                return new MFSMS_Free (m_Self);
            return null;
        }
        public void OnExit (MFSMStateType nextType) { }
    }
}