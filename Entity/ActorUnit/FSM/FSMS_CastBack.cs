using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    struct FSMS_CastBack : IFSMState {
        public FSMStateType m_Type { get { return FSMStateType.CAST_BACK; } }
        public E_Monster m_Self { get; set; }
        private float m_timer;
        public FSMS_CastBack (E_Monster self, float backTime) {
            m_Self = self;
            m_timer = backTime;
        }
        public void OnEnter (FSMStateType prevType) { }
        public void OnTick (float dT) {
            m_timer -= dT;
        }
        public IFSMState GetNextState () {
            if (m_Self.m_IsDead)
                return new FSMS_Dead(m_Self);
            // 后摇结束
            if (m_timer <= 0f)
                return new FSMS_Free (m_Self);
            return null;
        }
        public void OnExit (FSMStateType nextType) { }
    }
}