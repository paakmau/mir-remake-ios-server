using UnityEngine;
namespace MirRemakeBackend {
    class MFSM {
        public IMFSMState m_curState;
        public MFSM (IMFSMState initState) {
            m_curState = initState;
        }
        public void Tick (float dT) {
            m_curState.OnTick (dT);
            IMFSMState nextState = m_curState.GetNextState();
            if(nextState != null) {
                // 自动转移
                m_curState.OnExit (nextState.m_Type);
                nextState.OnEnter (m_curState.m_Type);
                m_curState = nextState;
            }
        }
    }
}