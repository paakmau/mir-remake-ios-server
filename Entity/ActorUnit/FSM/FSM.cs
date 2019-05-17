using UnityEngine;
namespace MirRemakeBackend {
    class FSM {
        public IFSMState m_curState;
        public FSM (IFSMState initState) {
            m_curState = initState;
        }
        public void Tick (float dT) {
            m_curState.OnTick (dT);
            IFSMState nextState = m_curState.GetNextState();
            if(nextState != null) {
                // 自动转移
                m_curState.OnExit (nextState.m_Type);
                nextState.OnEnter (m_curState.m_Type);
                m_curState = nextState;
            }
        }
    }
}